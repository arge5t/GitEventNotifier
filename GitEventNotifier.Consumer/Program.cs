using GitEventNotifier.Consumer;
using GitEventNotifier.Infrastructure.Implementations;
using GitEventNotifier.Infrastructure.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMessageSerializer, JsonMessageSerializer>();
builder.Services.AddSingleton<IRabbitMqConsumer>(sp =>
    new RabbitMqConsumer("localhost", sp.GetRequiredService<IMessageSerializer>()));

builder.Services.AddHostedService<GitEventHandlingService>();

var host = builder.Build();
await host.RunAsync();