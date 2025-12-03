# JWT RSA Keys

This directory contains the RSA public and private keys used for JWT token signing.

## Files:
- `private.pem` - Private key for signing tokens (keep secret!)
- `public.pem` - Public key for verifying tokens

## Generate New Keys:

Run the script to generate a new key pair:
```bash
cd /path/to/project/root
./scripts/generate-jwt-keys.sh
```

Or manually with OpenSSL:
```bash
# Generate private key
openssl genrsa -out private.pem 2048

# Extract public key
openssl rsa -in private.pem -pubout -out public.pem
```

## Security Notes:
- Never commit `private.pem` to version control
- The `.gitignore` file should exclude `*.pem` files
- In production, store keys securely (e.g., Azure Key Vault, AWS Secrets Manager)
