"""Configuration loader for the application."""

import os
import yaml
from pathlib import Path
from typing import Dict, Any


class ConfigLoader:
    """Loads and manages application configuration."""
    
    def __init__(self, config_path: str = None):
        """
        Initialize the config loader.
        
        Args:
            config_path: Path to the YAML config file. If None, uses default.yaml
        """
        if config_path is None:
            config_path = Path(__file__).parent.parent.parent / "config" / "default.yaml"
        
        self.config_path = Path(config_path)
        self.config = self._load_config()
        
        self.base_dir, self.logs_dir, self.data_dir = self._ensure_directories()

    
    def _load_config(self) -> Dict[str, Any]:
        """Load configuration from YAML file."""
        if not self.config_path.exists():
            raise FileNotFoundError(f"Config file not found: {self.config_path}")
        
        with open(self.config_path, 'r') as f:
            return yaml.safe_load(f) or {}
    
    def _ensure_directories(self):
        """Create necessary output directories if they don't exist."""
        base_dir = Path("output").resolve()
        logs_dir = base_dir / "logs"
        data_dir = base_dir / "data"
        logs_dir.mkdir(parents=True, exist_ok=True)
        data_dir.mkdir(parents=True, exist_ok=True)
        return base_dir, logs_dir, data_dir

    def get(self, key: str, default=None) -> Any:
        """
        Get config value using dot notation.
        
        Args:
            key: Dot-separated config key (e.g., "output.base_dir")
            default: Default value if key not found
        
        Returns:
            Configuration value
        """
        keys = key.split(".")
        value = self.config
        
        for k in keys:
            if isinstance(value, dict):
                value = value.get(k)
                if value is None:
                    return default
            else:
                return default
        
        return value if value is not None else default
    
    def get_output_dir(self) -> Path:
        return self.base_dir

    def get_logs_dir(self) -> Path:
        return self.logs_dir

    def get_data_dir(self) -> Path:
        return self.data_dir

    def is_mock_mode(self) -> bool:
        """Check if running in mock mode."""
        return self.get("app.mock_mode", False)

    def get_poll_interval_ms(self) -> int:
        """Get polling interval in milliseconds."""
        return self.get("app.poll_interval_ms", 500)