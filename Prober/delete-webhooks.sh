#!/usr/bin/env bash

kubectl delete validatingwebhookconfigurations validators.prober
kubectl delete mutatingwebhookconfigurations mutators.prober