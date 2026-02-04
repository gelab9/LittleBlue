"""Logging configuration for the application."""

import logging
import logging.handlers
from pathlib import Path
from typing import Optional


class LoggingSetup:
    """Configure and manage application logging."""
    
    def __init__(self, logs_dir: Path, level: str = "DEBUG", log_name: str = "developer.log"):
        """
        Initialize logging configuration.
        
        Args:
            logs_dir: Directory to store log files
            level: Logging level (DEBUG, INFO, WARNING, ERROR, CRITICAL)
            log_name: Name of the log file
        """
        self.logs_dir = Path(logs_dir)
        self.logs_dir.mkdir(parents=True, exist_ok=True)
        self.log_file = self.logs_dir / log_name
        self.level = getattr(logging, level.upper(), logging.DEBUG)
    
    def setup_logger(self, name: str = "SmartMeter") -> logging.Logger:
        """
        Configure and return a logger instance.
        
        Args:
            name: Logger name
        
        Returns:
            Configured logger instance
        """
        logger = logging.getLogger(name)
        logger.setLevel(self.level)
        
        # Only add handlers if they don't exist
        if logger.handlers:
            return logger
        
        # Create formatter
        formatter = logging.Formatter(
            '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
        
        # File handler
        file_handler = logging.handlers.RotatingFileHandler(
            self.log_file,
            maxBytes=5*1024*1024,  # 5MB
            backupCount=5
        )
        file_handler.setLevel(self.level)
        file_handler.setFormatter(formatter)
        logger.addHandler(file_handler)
        
        # Console handler
        console_handler = logging.StreamHandler()
        console_handler.setLevel(self.level)
        console_handler.setFormatter(formatter)
        logger.addHandler(console_handler)
        
        return logger