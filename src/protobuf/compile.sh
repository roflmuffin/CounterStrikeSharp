#!/bin/bash

rm -rf temp/ generated/
mkdir -p temp generated
mkdir -p generated

cp ../../libraries/GameTracking-CS2/Protobufs/*.proto temp/

for file in temp/*.proto; do
    echo 'syntax = "proto2";' | cat - $file > temp/tempfile && mv temp/tempfile $file
done

./protoc --cpp_out=../generated -I=temp -I=. temp/*.proto > /dev/null 2>&1

rm -r temp
