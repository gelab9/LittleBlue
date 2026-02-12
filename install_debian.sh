#!/bin/bash
# Installation script for Debian/Ubuntu Linux

echo "Installing system dependencies..."

# Update package list
sudo apt-get update

# Install Python 3.14 (if not available in repos, you may need to build from source or use deadsnakes PPA)
# For now, checking if Python 3 is installed
if ! command -v python3 &> /dev/null; then
    sudo apt-get install -y python3 python3-pip python3-venv
fi

# Install Qt6 system dependencies
sudo apt-get install -y \
    qt6-base-dev \
    libqt6core6 \
    libqt6gui6 \
    libqt6widgets6

# Install .NET SDK 8.0
# Check Debian version
DEBIAN_VERSION=$(cat /etc/debian_version | cut -d. -f1)

if [ "$DEBIAN_VERSION" -ge 13 ]; then
    echo "Detected Debian 13 (Trixie) or newer - using dotnet-install script..."
    # Use official install script for Debian 13+
    wget https://dot.net/v1/dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 8.0
    rm dotnet-install.sh
    
    # Add to PATH
    echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
    export PATH=$PATH:$HOME/.dotnet
else
    echo "Detected Debian 12 or older - using apt repository..."
    # Add Microsoft package repository for Debian 12
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-8.0
fi

# Install Python build dependencies
sudo apt-get install -y \
    python3-dev \
    build-essential \
    pkg-config

echo "Creating Python virtual environment..."
python3 -m venv venv

echo "Activating virtual environment..."
source venv/bin/activate

echo "Installing Python packages..."
pip install --upgrade pip
pip install -r requirements.txt

echo ""
echo "Installation complete!"
echo ""
echo "To activate the virtual environment, run:"
echo "  source venv/bin/activate"