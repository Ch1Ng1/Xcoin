#!/usr/bin/env python3
"""Verify that docs/_framework/ files match the hashes in blazor.boot.json."""

import base64
import hashlib
import json
import os
import sys

DOCS = os.path.join(os.path.dirname(__file__), "..", "docs")
FRAMEWORK = os.path.join(DOCS, "_framework")
BOOT_JSON = os.path.join(FRAMEWORK, "blazor.boot.json")


def main():
    errors = []

    # Check .nojekyll
    if not os.path.exists(os.path.join(DOCS, ".nojekyll")):
        errors.append("docs/.nojekyll is missing")

    # Check base href
    index_path = os.path.join(DOCS, "index.html")
    if os.path.exists(index_path):
        with open(index_path, "r", encoding="utf-8") as f:
            if '<base href="/Xcoin/" />' not in f.read():
                errors.append('docs/index.html does not contain <base href="/Xcoin/" />')
    else:
        errors.append("docs/index.html is missing")

    # Check blazor.boot.json
    if not os.path.exists(BOOT_JSON):
        errors.append("docs/_framework/blazor.boot.json is missing")
        for e in errors:
            print(f"ERROR: {e}")
        return 1

    with open(BOOT_JSON, "r", encoding="utf-8") as f:
        boot = json.load(f)

    # Collect all resources with integrity hashes
    def collect(obj, prefix=""):
        items = {}
        if isinstance(obj, dict):
            for key, val in obj.items():
                if isinstance(val, dict) and "hash" in val:
                    items[key] = val["hash"]
                elif isinstance(val, str) and val.startswith("sha256-"):
                    items[key] = val
                elif isinstance(val, dict):
                    items.update(collect(val, prefix))
        return items

    resources = collect(boot)

    for name, expected_hash in resources.items():
        filepath = os.path.join(FRAMEWORK, name)
        if not os.path.exists(filepath):
            errors.append(f"Missing file: _framework/{name}")
            continue
        with open(filepath, "rb") as f:
            digest = hashlib.sha256(f.read()).digest()
        actual = "sha256-" + base64.b64encode(digest).decode()
        if actual != expected_hash:
            errors.append(
                f"Hash mismatch: _framework/{name}\n"
                f"  expected: {expected_hash}\n"
                f"  actual:   {actual}"
            )

    if errors:
        for e in errors:
            print(f"ERROR: {e}")
        return 1

    print(f"OK: {len(resources)} resources verified, all hashes match.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
