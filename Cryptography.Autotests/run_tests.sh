#!/bin/bash

APP_UNDER_TEST_NAME=Cryptography.DemoApplication
ASSEMBLY_LOCATION=bin/Release/net5.0
APP_UNDER_TEST_BINARIES_DIR=app_under_test_binary

dotnet publish ../$APP_UNDER_TEST_NAME/$APP_UNDER_TEST_NAME.csproj -c Release
mkdir $APP_UNDER_TEST_BINARIES_DIR
cp -r ../$APP_UNDER_TEST_NAME/$ASSEMBLY_LOCATION/* ./$APP_UNDER_TEST_BINARIES_DIR/
python3 ./demontraiting_app_tests.py ./$APP_UNDER_TEST_BINARIES_DIR/$APP_UNDER_TEST_NAME.dll
rm -r ./$APP_UNDER_TEST_BINARIES_DIR
