#!/usr/bin/env bash
#
dotnet run -- generator docker --solution-dir .. --project-path ./Prober.csproj --target-file Prober.dll --dotnet-tag 6.0 -o Dockerfile
dotnet run -- generator crd -o ./config/crds
dotnet run -- generator installer -o ./config/installer
dotnet run -- generator operator -o ./config/operator
dotnet run -- generator rbac -o ./config/rbac
