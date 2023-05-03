# Prober

Running [Xabaril's AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
using Kubernetes CRD's

## Installation

```shell
git clone https://github.com/Dines97/DotnetProber
kubectl apply -k DotnetProber/k8s/install
```

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
$ kubectl get probes.d-teknoloji.com.tr rabbitmq-probe
NAME             PROBE STATUS   PROBE TYPE
rabbitmq-probe   1/3            RabbitMq
```

Or get more detailed status with node names and timestamp for each node

```shell
$ kubectl get events --field-selector involvedObject.name=rabbitmq-probe
LAST SEEN   TYPE      REASON      OBJECT                 MESSAGE
2m17s       Warning   Reconcile   probe/rabbitmq-probe   Node: other-node | Status: Degraded | Exception: None of the specified endpoints were reachable
5s          Warning   Reconcile   probe/rabbitmq-probe   Node: minikube-m02 | Status: Degraded | Exception: None of the specified endpoints were reachable
13s         Warning   Reconcile   probe/rabbitmq-probe   Node: minikube | Status: Degraded | Exception: None of the specified endpoints were reachable
17s         Normal    Reconcile   probe/rabbitmq-probe   Node: other-node | Status: Healthy | Exception:
```

## Todo

- [ ] Add support for running probes from different cluster nodes
