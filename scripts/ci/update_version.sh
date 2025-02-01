#!/usr/bin/env bash

version="$1"

set -eou pipefail

xmlstarlet ed -L -u '//Project/PropertyGroup/VersionPrefix' -v "$version" ./Lithium/Lithium.csproj
xmlstarlet ed -L -u '//Project/PropertyGroup/VersionPrefix' -v "$version" ./LithiumTestHelper/LithiumTestHelper.csproj
