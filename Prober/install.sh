#!/usr/bin/env bash

dotnet run -- install

kubectl apply -f ../samples/rabbitmq-probe.yml
