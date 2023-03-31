#!/bin/sh

set -e

if [ ! -z "$PRINCE_LICENSE" ]; then 
  echo "Override Prince License"
  PRINCE_PATH=./backend/3rdPartyLibs/prince
  mkdir -p "${PRINCE_PATH}/win32/license"
  mkdir -p "${PRINCE_PATH}/win64/license"
  echo "${PRINCE_LICENSE}" > "${PRINCE_PATH}/win32/license/license.dat"
  echo "${PRINCE_LICENSE}" > "${PRINCE_PATH}/win64/license/license.dat"
else
  echo "Prince License not overriden"
fi

cd frontend
npm ci
npm run create-package
