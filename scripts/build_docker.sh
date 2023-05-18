#!/bin/bash

# Run this script to build docker image locally
# ./build_docker.sh

scriptdir=$(dirname "$0")

# Load common code
source "$scriptdir"/common.sh

fn_say_wrn "**********************"
fn_say_wrn "Building docker images"
fn_say_wrn "**********************"
echo

docker_file="$scriptdir/../src/WebApi/Dockerfile"

docker build -t "sample/dotnet-api" -f "$docker_file" "$scriptdir/.." \
	--build-arg BUILD_IMAGE="mcr.microsoft.com/dotnet/sdk:7.0-alpine" \
	--build-arg RUNTIME_IMAGE="mcr.microsoft.com/dotnet/aspnet:7.0-alpine"

fn_say_success "*******"
fn_say_success "Success"
fn_say_success "*******"