#!/bin/bash

# TypeScript generation script for all Nebula proto files
export PATH="$PATH:ts-gen/node_modules/.bin"

echo "🚀 Starting TypeScript generation for all proto files..."

# Clean existing generated files
echo "🧹 Cleaning existing generated files..."
rm -rf ts-gen/gen/*

# Generate for each module individually to avoid case sensitivity issues
modules=("Organizations" "Authentication" "Accounting" "HR" "Shared")

echo "📦 Generating standard modules..."
for module in "${modules[@]}"; do
    echo "  → Generating $module..."
    buf generate --path "Protos/Nebula/Services/Fragments/$module"
    if [ $? -eq 0 ]; then
        echo "  ✓ $module generated successfully"
    else
        echo "  ✗ Failed to generate $module"
        exit 1
    fi
done

# Generate inventory files individually to avoid duplicate issues
echo "📦 Generating Inventory module (individual files)..."
inventory_files=(
    "BatchInterface.proto"
    "BatchRecord.proto"
    "InventoryInterface.proto"
    "LocationInterface.proto"
    "LocationRecord.proto"
    "MovementInterface.proto"
    "ProductRecord.proto"
    "ProductSupplierLink.proto"
    "StockInterface.proto"
    "StockMovementRecord.proto"
    "StockRecord.proto"
    "SupplierInterface.proto"
    "SupplierRecord.proto"
)

for file in "${inventory_files[@]}"; do
    echo "  → Generating $file..."
    buf generate --path "Protos/Nebula/Services/Fragments/Inventory/$file"
    if [ $? -eq 0 ]; then
        echo "  ✓ $file generated successfully"
    else
        echo "  ✗ Failed to generate $file"
        exit 1
    fi
done

# Fix case sensitivity issues in generated files
echo "🔧 Fixing case sensitivity issues..."
find ts-gen/gen -name "*.ts" -type f -exec sed -i 's|../inventory/|../Inventory/|g' {} \;
echo "  ✓ Case sensitivity fixes applied"

# Count generated files
total_files=$(find ts-gen/gen -name "*.ts" -type f | wc -l)
echo "🎉 Generation complete! Generated $total_files TypeScript files."

# List generated modules
echo "📁 Generated modules:"
find ts-gen/gen -type d -name "*" | grep -v "^ts-gen/gen$" | sort | sed 's|ts-gen/gen/||g' | sed 's|^|  - |g'
