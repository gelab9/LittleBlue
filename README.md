# 🌡️ LittleBlue - Getting Started 🌡️

**Disclaimer:** Please review the `requirements.txt` for all libraries and imports necessary for running this application.

## Table of Contents
- [Initial Setup](#initial-setup)
- [Branch Management](#branch-management)
- [Testing and Development](#testing-and-development)

---

## Initial Setup

Follow these steps to get up and running quickly:

### 1. Clone the Project using SSH

For security and easy authentication, we recommend using SSH to clone the repository.

```bash
git clone git@github.com:gelab9/LittleBlue.git
```

![Git Clone](gitclone.png)

> **Note:** Depending on admin rights and IT blockage, you will highly likely need to generate this key using PowerShell as administrator.

### 2. Generate SSH Key Pair

```bash
ssh-keygen -t ed25519 -C "your_email@example.com"
```

![Git Admin Key](gitadminkey.png)

### 3. Add SSH Key to GitHub

1. Once the key has generated, log into GitHub using your browser
2. Go to **Settings** → **SSH and GPG keys**
3. Click **New SSH key**
4. Paste your generated public key contents

![Git SSH Key](gitsshkey.png)

### 4. Confirm SSH Connection

```bash
ssh -T git@github.com
```

![SSH Confirm](sshconfirm.png)

You should see a message confirming you're authenticated.

### 5. Start the SSH Agent

```bash
eval "$(ssh-agent -s)"
```

![SSH Agent](sshagent.png)

### 6. Add Your Key to the SSH Agent

```bash
ssh-add ~/.ssh/id_ed25519
```

![Add Key](addkey.png)

### 7. Clone the Repository

1. Go to your repository on GitHub
2. Click **Code**
3. Copy the SSH link

![Git Adding SSH Key](gitaddingsshkey.png)

```bash
git clone git@github.com:username/repository.git
```

---

## Branch Management

Within our repository, there are two main branches:

- **main** - Contains the BigBlue code that was recreated for new sources
- **develop** - Contains the code for the reformatted Temperature Rise application

### Fetching develop from Remote

If you just cloned the repository, you won't have `develop` locally. Here's how to get it from GitHub:

```bash
# Make sure your branch is clean
git branch
git status

# Fetch and checkout
git fetch origin
git checkout -b develop origin/develop
```

### Checking Out the develop Branch

```bash
# Check what branch you're in
git branch
git status

# Switch to the develop branch
git switch develop

# Or alternatively
git checkout develop
```

### Switching Back to main

```bash
# Make sure your branch is clean
git branch
git status

# If you don't have main locally yet
git fetch origin

# Switch to the main branch
git switch main
```

### Force Push and Pull (Use with Caution)

> **Warning:** Force pushing will overwrite remote changes with your current local files.

#### Force Push

```bash
# Force push to main
git push --force origin main

# Or force push to another branch
git push --force origin <branch-name>
```

#### Reset Changes

```bash
git reset --hard <commit-hash>
git push --force origin <branch-name>
```

#### Pull Forced Changes

```bash
# Make sure you're in the repo
git status
# You should see 'On branch develop'

# Confirm the remote
git remote -v
# You should see:
# origin  git@github.com:your-org/your-project.git (fetch)
# origin  git@github.com:your-org/your-project.git (push)

# Fetch everything fresh from the remote
git fetch origin

# Checkout the develop branch
git checkout develop

# Or if you don't have it locally
git checkout -b develop origin/develop

# Reset your local branch to match the remote branch
git reset --hard origin/develop

# Verify
git status
# You should see:
# On branch develop
# Your branch is up to date with 'origin/develop'.
# nothing to commit, working tree clean
```

---

## Testing and Development

### Connecting the .NET Application and Checking Connections

#### Step 1: Connect to COM Port

```bash
curl.exe -i -X POST "http://127.0.0.1:5055/daq/connect" \
  -H "Content-Type: application/json" \
  --data-binary "@connect.json"
```

#### Step 2: Call DAQ IDN to Verify Communication

```bash
curl.exe -i "http://127.0.0.1:5055/daq/idn"
```

These commands check the DAQ 34970A instrument and verify a serial connection, returning:
- Manufacturer
- Serial number
- Firmware version (FW)

**Expected Response:**
```json
{"idn":"HEWLETT-PACKARD,34970A,0,13-2-2"}
```

### Identifying the 34970A (Agilent)

```bash
# Connect to the device
curl.exe -i -X POST "http://127.0.0.1:5055/daq/connect" \
  -H "Content-Type: application/json" \
  --data-binary "@connect.json"

# Get identification
curl.exe -i "http://127.0.0.1:5055/daq/idn"
```

---

## Additional Resources

For installation instructions on Linux Debian environments, see [README_INSTALL.md](README_INSTALL.md)

# Installation Instructions for Debian/Ubuntu Linux

## Prerequisites
- Debian 12 (Bookworm) or Ubuntu 22.04+ recommended
- Sudo access

## Quick Install

1. Make the install script executable:
```bash
chmod +x install_debian.sh
```

2. Run the installation script:
```bash
./install_debian.sh
```

3. Activate the virtual environment:
```bash
source venv/bin/activate
```

## Manual Installation

### 1. Install System Dependencies

```bash
# Update package list
sudo apt-get update

# Install Python development tools
sudo apt-get install -y python3 python3-pip python3-venv python3-dev build-essential

# Install Qt6 libraries (required for PyQt6)
sudo apt-get install -y qt6-base-dev libqt6core6 libqt6gui6 libqt6widgets6

# Install .NET SDK 8.0
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### 2. Create Virtual Environment

```bash
python3 -m venv venv
source venv/bin/activate
```

### 3. Install Python Packages

```bash
pip install --upgrade pip
pip install -r requirements.txt
```

## Verification

Verify installations:

```bash
# Check Python version
python --version

# Check .NET version
dotnet --version

# Check installed packages
pip list
```

## Notes

- **Python 3.14**: If Python 3.14 is not available in your distribution's repositories, you may need to:
  - Use [deadsnakes PPA](https://launchpad.net/~deadsnakes/+archive/ubuntu/ppa) (Ubuntu)
  - Build from source
  - Use pyenv

- **Qt6**: If you encounter issues with PyQt6, ensure Qt6 libraries are installed system-wide

- **Virtual Environment**: Always activate the virtual environment before running your application:
  ```bash
  source venv/bin/activate
  ```

## Troubleshooting

### PyQt6 Installation Issues
If PyQt6 fails to install, try:
```bash
sudo apt-get install -y python3-pyqt6
```

### Missing Development Headers
If you get compilation errors:
```bash
sudo apt-get install -y libpython3-dev
```