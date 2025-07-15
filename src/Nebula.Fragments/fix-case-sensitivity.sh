#!/bin/bash

# Fix case sensitivity issues in generated TypeScript files
echo "🔧 Fixing case sensitivity issues in generated files..."

# Fix imports in Inventory module files
find ts-gen/gen -name "*.ts" -type f -exec sed -i 's|../inventory/|../Inventory/|g' {} \;
find ts-gen/gen -name "*.ts" -type f -exec sed -i 's|from "../inventory/|from "../Inventory/|g' {} \;

echo "✅ Case sensitivity fixes applied to all generated files"
