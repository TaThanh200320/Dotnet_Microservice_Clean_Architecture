#!/bin/bash

# Script to generate RSA key pair for JWT signing
# Usage: ./generate-jwt-keys.sh

set -e

# Define paths
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CERTS_DIR="$BASE_DIR/src/Identity/Api/certs"
PRIVATE_KEY="$CERTS_DIR/private.pem"
PUBLIC_KEY="$CERTS_DIR/public.pem"

echo "===================================="
echo "JWT RSA Key Pair Generator"
echo "===================================="
echo ""

# Check if OpenSSL is installed
if ! command -v openssl &> /dev/null; then
    echo "❌ Error: OpenSSL is not installed"
    echo "Please install OpenSSL first:"
    echo "  - Ubuntu/Debian: sudo apt-get install openssl"
    echo "  - macOS: brew install openssl"
    echo "  - Windows: Download from https://slproweb.com/products/Win32OpenSSL.html"
    exit 1
fi

echo "✓ OpenSSL found: $(openssl version)"
echo ""

# Create certs directory if it doesn't exist
if [ ! -d "$CERTS_DIR" ]; then
    echo "Creating certs directory..."
    mkdir -p "$CERTS_DIR"
    echo "✓ Directory created: $CERTS_DIR"
else
    echo "✓ Certs directory exists: $CERTS_DIR"
fi
echo ""

# Check if keys already exist
if [ -f "$PRIVATE_KEY" ] || [ -f "$PUBLIC_KEY" ]; then
    echo "⚠️  Warning: Key files already exist!"
    echo "  - Private key: $PRIVATE_KEY"
    echo "  - Public key: $PUBLIC_KEY"
    echo ""
    read -p "Do you want to overwrite existing keys? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Operation cancelled."
        exit 0
    fi
    echo ""
fi

# Generate private key (2048 bits)
echo "Generating RSA private key (2048 bits)..."
openssl genrsa -out "$PRIVATE_KEY" 2048 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✓ Private key generated: $PRIVATE_KEY"
else
    echo "❌ Failed to generate private key"
    exit 1
fi
echo ""

# Extract public key from private key
echo "Extracting public key..."
openssl rsa -in "$PRIVATE_KEY" -pubout -out "$PUBLIC_KEY" 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✓ Public key extracted: $PUBLIC_KEY"
else
    echo "❌ Failed to extract public key"
    exit 1
fi
echo ""

# Set appropriate permissions
chmod 600 "$PRIVATE_KEY"
chmod 644 "$PUBLIC_KEY"
echo "✓ File permissions set"
echo ""

# Display key information
echo "===================================="
echo "Key Generation Complete!"
echo "===================================="
echo ""
echo "📁 Key Files:"
echo "  Private Key: $PRIVATE_KEY"
echo "  Public Key:  $PUBLIC_KEY"
echo ""
echo "🔒 Security Notes:"
echo "  • Private key has restricted permissions (600)"
echo "  • Never commit private.pem to version control"
echo "  • Add *.pem to .gitignore if not already present"
echo ""
echo "🔑 Key Details:"
openssl rsa -in "$PRIVATE_KEY" -text -noout 2>/dev/null | head -n 3
echo ""
echo "✅ You can now start the Identity service!"
echo ""
