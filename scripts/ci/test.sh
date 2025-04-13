#!/usr/bin/env bash

set -eou pipefail

echo "⬇️ Installing Dependencies..."
dotnet restore
echo "✅ Done!"

# build the project
echo "🏗️ Building..."
dotnet build --no-incremental
echo "✅ Done!"

# run tests
echo "🧪 Running and Coverage..."
dotnet test
echo "✅ Done!"
