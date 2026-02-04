import logging

class DeviceBase:
    def __init__(self, logger: logging.Logger = None):
        self.logger = logger or logging.getLogger(self.__class__.__name__)

    def connect(self): raise NotImplementedError
    def disconnect(self): raise NotImplementedError
    def is_connected(self) -> bool: raise NotImplementedError
