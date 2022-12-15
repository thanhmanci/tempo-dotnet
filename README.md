# Introduction
This repository contains an example about how to use opentelemetry for tracing when we have a bunch of distributed applications

# Content

The repository contains the following applications:

![Alt Text](https://raw.githubusercontent.com/thanhmanci/tempo-dotnet/master/components-diagram.png)

- **App1** is a **NET6 Web API** with 2 endpoints.
    - The **/http** endpoint makes an HTTP request to the App2 _"/dummy"_ endpoint.
    - The **/publish-message** endpoint queues a message into a Rabbit queue named _"sample"_.
    
- **App2** is a **NET6 console** application. 
  - Dequeues messages from the Rabbit _"sample"_ queue and makes a HTTP request to the **App3** _"/sql-to-event"_ endpoint with the content of the message.

- **App3** is a **NET6 Web API** with 2 endpoints
    - The **/dummy** endpoint returns a fixed _"Ok"_ response.
    - The **/sql-to-event** endpoint receives a message via HTTP POST, stores it in a MSSQL Server and afterwards publishes the message as an event into a RabbitMq queue named _"sample_2"_.

- **App4** is a **NET6 Worker Service**.
  - A Hosted Service reads the messages from the Rabbitmq _"sample_2"_ queue and stores it into a Redis cache database.

# OpenTelemetry .NET Client

The apps are using the following package versions:

```xml
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.0" />
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.17.0" />
    <PackageReference Include="MongoDB.Driver" Version="2.18.0" />
    <PackageReference Include="Portable.BouncyCastle" Version="1.9.0" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.0.0-rc9.9" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.4.0-beta.3" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.0.0-rc9.9" />
    <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.4.0-beta.3" />
    <PackageReference Include="Grpc.Core" Version="2.46.5" />
    <PackageReference Include="MongoDB.Driver.Core.Extensions.OpenTelemetry" Version="1.0.0" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.0.0-rc9.9" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.0.0-rc9.9" />
```

# External Dependencies

- OpenTelemetry 
- MSSQL Server
- RabbitMq
- Redis Cache


# Output

If you open jaeger you are going to see something like this

![Alt Text](https://raw.githubusercontent.com/thanhmanci/tempo-dotnet/master/tempo.png)
