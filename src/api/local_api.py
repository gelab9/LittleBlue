from __future__ import annotations
from dataclasses import dataclass
from typing import Any, Optional, Dict
import requests
from PyQt6.QtCore import QObject, pyqtSignal, pyqtSlot

@dataclass
class ApiResult:
    ok: bool
    status: int
    data: Optional[Dict[str, Any]] = None
    error: Optional[str] = None

def _safe_json(r: requests.Response) -> Dict[str, Any]:
    try:
        return r.json()
    except Exception:
        return {"raw": r.text}

class LocalApiClient:
    def __init__(self, base_url: str = "http://127.0.0.1:5055", timeout_s: float = 5.0):
        self.base_url = base_url.rstrip("/")
        self.timeout_s = timeout_s
        self.session = requests.Session()

    def get(self, path: str) -> ApiResult:
        try:
            r = self.session.get(self.base_url + path, timeout=self.timeout_s)
            return ApiResult(ok=r.ok, status=r.status_code, data=_safe_json(r), error=None if r.ok else r.text)
        except Exception as e:
            return ApiResult(ok=False, status=0, data=None, error=str(e))

    def post(self, path: str, payload: Dict[str, Any]) -> ApiResult:
        try:
            r = self.session.post(self.base_url + path, json=payload, timeout=self.timeout_s)
            return ApiResult(ok=r.ok, status=r.status_code, data=_safe_json(r), error=None if r.ok else r.text)
        except Exception as e:
            return ApiResult(ok=False, status=0, data=None, error=str(e))

class ApiWorker(QObject):
    finished = pyqtSignal(str, object)
    progress = pyqtSignal(str)

    def __init__(self, api: LocalApiClient):
        super().__init__()
        self.api = api

    @pyqtSlot(str, dict)
    def do_post(self, action: str, payload: dict):
        self.progress.emit(f"{action}...")
        res = self.api.post(payload["path"], payload.get("json", {}))
        self.finished.emit(action, res)

    @pyqtSlot(str, str)
    def do_get(self, action: str, path: str):
        self.progress.emit(f"{action}...")
        res = self.api.get(path)
        self.finished.emit(action, res)
