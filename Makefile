PROJECT_ROOT := $(shell dirname $(realpath $(lastword $(MAKEFILE_LIST))))

PROJECT := ${PROJECT_ROOT}/Prober/Prober.csproj
DOTNET_RUN := dotnet run --project ${PROJECT}

install: 
	${DOTNET_RUN} -- install

check-generated-yaml: generate-config print-diffo

generate-config:
	${DOTNET_RUN} -- generator docker --solution-dir .. --project-path ./Prober.csproj --target-file Prober.dll --dotnet-tag 7.0 -o Dockerfile
	${DOTNET_RUN} -- generator crd -o ./config/crds
	${DOTNET_RUN} -- generator installer -o ./config/install
	${DOTNET_RUN} -- generator operator -o ./config/operator
	${DOTNET_RUN} -- generator rbac -o ./config/rbac

print-diff: 
	-diff --color -r ${PROJECT_ROOT}/Prober/config ${PROJECT_ROOT}/k8s

delete-webhooks:
	kubectl delete validatingwebhookconfigurations validators.prober
	kubectl delete mutatingwebhookconfigurations mutators.prober

apply-samples:
	kubectl apply -k ${PROJECT_ROOT}/samples

apply-k8s: generate-config
	kubectl apply -k ${PROJECT_ROOT}/k8s/install

reset-minikube:
	minikube delete
	minikube start
