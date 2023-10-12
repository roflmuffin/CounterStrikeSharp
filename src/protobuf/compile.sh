#!/bin/bash

rm -rf temp/ generated/
mkdir -p temp generated
mkdir -p generated

cp ../../libraries/GameTracking-CS2/Protobufs/*.proto temp/
cp -r google/ temp/

for file in temp/*.proto; do
    echo 'syntax = "proto2";' | cat - $file > temp/tempfile && mv temp/tempfile $file;
    protoc --cpp_out=generated -I temp $file
done

rm -r temp
