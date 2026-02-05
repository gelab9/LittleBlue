class NullCsvLogger:
    def initialize_csv(self, headers): 
        pass
    def append_row(self, row): 
        pass

class NullStatistics:
    def reset(self): 
        pass
    def add_reading(self, reading): 
        pass
    def get_summary(self): 
        return {}

class NullReportGenerator:
    def generate_report(self, test_name, report_data): 
        pass

class NullTestController:
    MODE_FREE = "FREE"
    MODE_READINGS = "READINGS"
    MODE_DURATION = "DURATION"

    def __init__(self):
        self.is_running = False
        self.reading_count = 0
        self.mode = self.MODE_FREE
        self.daq = None

    def set_mode(self, mode, target): 
        self.mode = mode
    def start(self): 
        self.is_running = True
    def stop(self): 
        self.is_running = False
    def get_reading(self): 
        return None
