# Prober

Running [Xabaril's AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
using Kubernetes CRD's

## Installation

## Usage

Create Probe usage with required Probe Type like

```yaml
apiVersion: d-teknoloji.com.tr/v1alpha1
kind: Probe
metadata:
  name: rabbitmq-probe
spec:
  period: 1m
  description: rabbitmq probe
  type: RabbitMq
  parameters: |
    connectionString: "amqp://localhost:5672"
```

Than you can use kubectl to check connection to different services on which application running inside the cluster is
dependent

```shell
➜ kubectl get probes.d-teknoloji.com.tr
NAME             PROBE STATUS   PROBE TYPE
rabbitmq-probe   1/1            RabbitMq
```

Or get more detailed status with node names and timestamp for each node

```shell
➜ kubectl get probes.d-teknoloji.com.tr rabbitmq-probe -o jsonpath='{.status}'
{"nodeStatus":[{"name":"unknown","status":"Healthy","timestamp":"2023-04-17T11:52:06.7702830+03:00"}],"status":"1/1"}
```

## Todo

- [ ] Add support for running probes from different cluster nodes
