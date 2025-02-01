#!/usr/bin/env bash

[ "${ATOMI_SERVICE}" = '' ] && echo "❌ 'ATOMI_SERVICE' env var not set" && exit 1

threshold=${1:-50}

set -eou pipefail

# export path
export PATH="$PATH:$HOME/.dotnet/tools"

echo "⬇️ Installing Dependencies..."
dotnet restore
dotnet tool restore
echo "✅ Done!"

# build the project
echo "🏗️ Building..."
dotnet build --no-incremental
echo "✅ Done!"

# run tests
echo "🧪 Running and Coverage..."
dotnet dotcover test --dcReportType=Json
echo "✅ Done!"

# print coverage
coverage=$(jq --arg name "${ATOMI_SERVICE}" '.Children[] | select(.Name == $name and .Kind == "Assembly") | .CoveragePercent' ./dotCover.Output.json)

echo "🧪 Current test coverage: ${coverage}%"

if (($(echo "$coverage > $threshold" | bc -l))); then
  echo "✅ Coverage threshold of ${threshold}% met"
else
  echo "❌ Failed:️ current test coverage is below threshold of ${threshold}%."
  exit 1
fi
