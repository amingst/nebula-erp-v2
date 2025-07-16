#!/bin/bash

# Set JWT environment variables for development
export JWT_PRIV_KEY="$(cat dev.priv.jwk)"
export JWT_PUB_KEY="$(cat dev.pub.jwk)"

echo "JWT environment variables set successfully"
echo "JWT_PRIV_KEY: ${JWT_PRIV_KEY:0:50}..."
echo "JWT_PUB_KEY: ${JWT_PUB_KEY:0:50}..."
