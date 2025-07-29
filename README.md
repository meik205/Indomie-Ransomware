# Indomie Ransomware

**Indomie Ransomware** is a basic hybrid ransomware project written in **C#**, designed for educational and research purposes only.

## ⚙️ Features

- ✅ Hybrid encryption using:
  - **AES-256**: For fast and secure file encryption.
  - **RSA-2048**: For encrypting the AES key.
- ✅ Can **bypass Windows Defender** using simple evasion techniques.
- ✅ Written entirely in **C#** — easy to understand, modify, and extend.

## 🚫 Disclaimer

> ⚠️ This project is intended strictly for **educational**, **red team**, or **research** use only.  
> **Do not use** this code for malicious purposes.  
> The author is **not responsible** for any damage or legal consequences resulting from misuse of this tool.

## 🔐 Encryption Process

1. Generates a random **AES-256 key** for each victim.
2. Encrypts files in selected directories using AES.
3. Encrypts the AES key with an embedded **RSA-2048 public key**.
4. Optionally drops a ransom note with system info and encrypted key ID.

## 🧪 Test Environment

- Recommended to test inside a **virtual machine** (VM).
- Works on **Windows 10/11**, x64 architecture.

---

> ⚠️ For legal and ethical use only. Use at your own risk.

## To DO
- API Hashing
- Sysalls to evade EDR