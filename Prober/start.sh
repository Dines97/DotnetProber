#!/usr/bin/env bash

dotnet run -- generator crd
dotnet run -- install

kubectl apply -f ./config/samples/rabbitmq-probe.yml

dotnet run