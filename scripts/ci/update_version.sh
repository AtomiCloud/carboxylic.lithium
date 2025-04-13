#!/usr/bin/env bash

version="$1"
targets="$2"

set -eou pipefail

[ -z "$version" ] && echo "Error: Version argument is required" && exit 1
[ -z "$targets" ] && echo "Error: Targets argument is required" && exit 1

# Split comma-delimited targets into array
IFS=',' read -ra target_array <<<"$targets"

# Loop through targets and update version for each
for target in "${target_array[@]}"; do
  echo "Updating version to $version for $target"
  xmlstarlet ed -L -u '//Project/PropertyGroup/VersionPrefix' -v "$version" "$target/$target.csproj"
done
