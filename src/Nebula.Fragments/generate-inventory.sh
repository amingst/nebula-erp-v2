#!/bin/bash

# Alternative approach for Inventory module
export PATH="$PATH:ts-gen/node_modules/.bin"

echo "Attempting to generate Inventory module files individually..."

# List of inventory proto files (excluding the problematic ProductRecord.proto for now)
inventory_files=(
    "BatchInterface.proto"
    "BatchRecord.proto"
    "InventoryInterface.proto"
    "LocationInterface.proto"
    "LocationRecord.proto"
    "MovementInterface.proto"
    "ProductSupplierLink.proto"
    "StockInterface.proto"
    "StockMovementRecord.proto"
    "StockRecord.proto"
    "SupplierInterface.proto"
    "SupplierRecord.proto"
)

for file in "${inventory_files[@]}"; do
    echo "Generating $file..."
    buf generate --path "Protos/Nebula/Services/Fragments/Inventory/$file"
    if [ $? -eq 0 ]; then
        echo "✓ $file generated successfully"
    else
        echo "✗ Failed to generate $file"
    fi
done

echo "Attempting ProductRecord.proto..."
buf generate --path "Protos/Nebula/Services/Fragments/Inventory/ProductRecord.proto"
if [ $? -eq 0 ]; then
    echo "✓ ProductRecord.proto generated successfully"
else
    echo "✗ Failed to generate ProductRecord.proto (likely duplicate issue)"
fi

echo "Inventory generation complete!"
