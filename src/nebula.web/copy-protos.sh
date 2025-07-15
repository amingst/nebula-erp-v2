#!/bin/bash

# Copy protobuf generated files to nebula.web
echo "Copying protobuf generated files to nebula.web..."

# Create the destination directory
mkdir -p src/lib/protos

# Copy the generated files
cp -r ../Nebula.Fragments/ts-gen/gen/* src/lib/protos/
cp ../Nebula.Fragments/ts-gen/index.ts src/lib/protos/

echo "Protobuf files copied successfully!"
echo "Files available in: src/lib/protos/"
