using GitEventNotifier.Infrastructure.Implementations;
using GitEventNotifier.Infrastructure.Interfaces;
using GitEventNotifier.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();
builder.Services.AddSingleton<IRabbitMqProducer>(sp =>
    new RabbitMqProducer("localhost", sp.GetRequiredService<IMessageSerializer>()));

builder.Services.AddHostedService<GitHubEventPublisherService>();

var host = builder.Build();
await host.RunAsync();