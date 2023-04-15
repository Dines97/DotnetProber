using System.Text.Json.Serialization;
using AutoMapper;
using KubeOps.Operator;
using Prober;
using Prober.Probe;
using Prober.ProbeManager;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKubernetesOperator(x => { x.EnableLeaderElection = false; });

builder.Services.AddTransient<IProbeManager, ProbeManager>();
builder.Services.AddTransient<IDeserializer>(_ =>
  new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()
);

builder.Services.AddTransient<IProbe, RabbitMqProbe>();
builder.Services.AddTransient<IProbe, PostgresqlProbe>();
builder.Services.AddTransient<IProbe, DnsProbe>();


builder.Services.AddControllers().AddJsonOptions(opts => {
  var enumConverter = new JsonStringEnumConverter();
  opts.JsonSerializerOptions.Converters.Add(enumConverter);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddHealthChecks().AddKubernetes();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var app = builder.Build();
app.UseKubernetesOperator();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunOperatorAsync(args);