# Build the operator
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /operator

COPY ./ ./
RUN dotnet publish -c Release -o out Prober/Prober.csproj

# The runner for the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as final

RUN addgroup --gid 10000 k8s-operator && useradd --uid 10000 -G k8s-operator operator-user

WORKDIR /operator
COPY --from=build /operator/out/ ./
RUN chown operator-user:k8s-operator -R .

USER operator-user

ENTRYPOINT [ "dotnet", "Prober.dll" ]
