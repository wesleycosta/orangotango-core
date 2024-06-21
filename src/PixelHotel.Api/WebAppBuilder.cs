﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PixelHotel.Infra;
using PixelHotel.Infra.Configurations;
using PixelHotel.Infra.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PixelHotel.Api;

public sealed class WebAppBuilder
{
    private WebApplication _app;
    private WebApplicationBuilder _builder;

    public WebAppBuilder BuildDefault(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        return this;
    }

    public WebAppBuilder WithDefaultServices()
    {
        _builder.Services.AddApiConfiguration(_builder.Configuration);
        _builder.Services.AddBaseOptions(_builder.Configuration);
        _builder.Services.AddLogger(_builder.Configuration);

        return this;
    }

    public WebAppBuilder WithCustomConfiguration(Action<ConfigurationManager> customConfig)
    {
        customConfig.Invoke(_builder.Configuration);

        return this;
    }

    public WebAppBuilder WithServicesFromAssemblies(IEnumerable<Assembly> assemblies)
    {
        _builder.Services.AddServicesDependencies(_builder.Configuration, assemblies);
        return this;
    }

    public WebAppBuilder WithServicesFromAssemblies(params Assembly[] assemblies)
        => WithServicesFromAssemblies(assemblies);

    public WebAppBuilder WithDefaultAppConfig(Action<IApplicationBuilder> customConfig = null)
    {
        _app = _builder.Build();
        _app.UseApiConfiguration(_builder.Configuration, customConfig);
        _app.MapControllers();

        return this;
    }

    public WebAppBuilder WithApplyMigrate<TDbContext>() where TDbContext : DbContext
    {
        _app.ApplyMigrate<TDbContext>();

        return this;
    }

    public WebApplication Create()
        => _app;
}
