#!/bin/bash

# Set up PATH with local node_modules
export PATH="$PATH:ts-gen/node_modules/.bin"

echo "Generating TypeScript definitions for all proto modules..."

# Generate for each module individually to avoid duplicate issues
modules=("Organizations" "Authentication" "Accounting" "HR" "Shared")

for module in "${modules[@]}"; do
    echo "Generating $module..."
    buf generate --path "Protos/Nebula/Services/Fragments/$module"
    if [ $? -eq 0 ]; then
        echo "✓ $module generated successfully"
    else
        echo "✗ Failed to generate $module"
    fi
done

echo "Attempting to generate Inventory module (may have case sensitivity issues)..."
buf generate --path "Protos/Nebula/Services/Fragments/Inventory" 2>/dev/null || echo "⚠ Inventory module skipped due to case sensitivity issues"

echo "Generation complete!"
echo "Generated files are in: ts-gen/gen/"
