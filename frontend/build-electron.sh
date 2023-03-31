#!/bin/sh

set -e

export APP_VERSION=${APP_VERSION:-'0.0.0'}

echo "Building Electron App with build version $APP_VERSION"

rm -rf packages

./node_modules/.bin/electron-builder build --win dir:x64
mkdir -p ./packages/voting-stimmunterlagen-offline-win32-x64 && cp -r ./dist/win-unpacked/* ./packages/voting-stimmunterlagen-offline-win32-x64
