import serial
import time
import logging


class DAQ34970A:
    def __init__(self, port: str, baudrate: int = 9600, timeout: float = 1.0,
                 logger: logging.Logger = None):
        self.port = port
        self.baudrate = baudrate
        self.timeout = timeout
        self.logger = logger or logging.getLogger(__name__)
        self.ser = None

    def connect(self):
        if self.ser and self.ser.is_open:
            return

        self.logger.info(f"Connecting to 34970A on {self.port} @ {self.baudrate}")
        self.ser = serial.Serial(
            port=self.port,
            baudrate=self.baudrate,
            timeout=self.timeout
        )
        time.sleep(0.5)  # instrument settle time

    def disconnect(self):
        if self.ser and self.ser.is_open:
            self.logger.info("Disconnecting 34970A")
            self.ser.close()

    def is_connected(self) -> bool:
        return self.ser is not None and self.ser.is_open

    def write(self, cmd: str):
        if not self.is_connected():
            raise RuntimeError("34970A not connected")

        full_cmd = cmd.strip() + "\n"
        self.logger.debug(f"34970A TX: {full_cmd.strip()}")
        self.ser.write(full_cmd.encode())

    def query(self, cmd: str) -> str:
        self.write(cmd)
        resp = self.ser.readline().decode(errors="ignore").strip()
        self.logger.debug(f"34970A RX: {resp}")
        return resp

    def identify(self) -> str:
        return self.query("*IDN?")
    
    def reset(self):
        # safe reset
        self.write("*CLS")
        self.write("*RST")

    def read_dc_voltage(self, channel: int) -> float:
        resp = self.query(f"MEAS:VOLT:DC? (@{channel})")
        return float(resp)
    
    def configure_scan(self, channels: list[int]):
        # Example channels: [101, 102, 103]
        scan = ",".join(str(ch) for ch in channels)

        # Configure scan and format
        self.write("*CLS")
        self.write("FORM:READ:TIME ON")
        self.write("FORM:READ:CHAN ON")

        # Set scan list
        self.write(f"ROUT:SCAN (@{scan})")

        # One reading per channel per trigger
        self.write("TRIG:SOUR IMM")
        self.write("TRIG:COUN 1")
        self.write(f"ROUT:SCAN:COUN 1")

    def read_scan(self):
        """
        Perform one scan read.
        Returns list of dicts: [{'channel': 101, 'value': 23.4, 'timestamp': '...'}, ...]
        """
        raw = self.query("READ?")
        if not raw:
            return []

        parts = [p.strip() for p in raw.split(",") if p.strip()]
        results = []

        # Expected pattern: value, timestamp, channel, ...
        i = 0
        while i + 2 < len(parts):
            try:
                value = float(parts[i])
                timestamp = parts[i + 1].strip('"')
                channel = int(float(parts[i + 2]))
                results.append({
                    "channel": channel,
                    "value": value,
                    "timestamp": timestamp
                })
            except Exception as e:
                self.logger.warning(f"DAQ parse error at index {i}: {e} ({parts[i:i+3]})")
            i += 3

        return results


    def read_scan(self) -> list[tuple[int, float, str]]:
        """
        Returns list of (channel, value, timestamp_string).
        Uses READ? which triggers + reads one scan cycle.
        """
        raw = self.query("READ?")

        self.logger.debug(f"RAW READ?: {raw}")

        #TODO: delete this later
        if not raw:
            err = self.query("SYST:ERR?")
            self.logger.error(f"34970A READ? returned empty. SYST:ERR? -> {err}")
            return []


        # Typical output is comma-separated blocks; exact format depends on FORM:READ settings.
        parts = [p.strip() for p in raw.split(",") if p.strip()]
        # With TIME ON and CHAN ON, you often get repeating groups like:
        # value, timestamp, channel, value, timestamp, channel, ...
        out = []
        i = 0
        while i + 2 < len(parts):
            value = float(parts[i])
            timestamp = parts[i + 1].strip('"')
            channel = int(float(parts[i + 2]))  # sometimes "101" arrives as "101"
            out.append((channel, value, timestamp))
            i += 3
        return out
    
        


