﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PixelHotel.Infra.Options;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace PixelHotel.Infra.Logger;

internal static class SerilogConfiguration
{
    public static IServiceCollection AddSerilog(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = GetElasticsearchOptions(configuration);
        var elasticsearchUri = new Uri(options.Uri);

        var elasticsearchSinkOptions = new ElasticsearchSinkOptions(elasticsearchUri)
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = options.IndexFormat
        };

        Log.Logger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .WriteTo
            .Elasticsearch(elasticsearchSinkOptions)
            .WriteTo
            .Console()
            .CreateLogger();

        return services.AddSingleton(Log.Logger);
    }

    private static ElasticsearchOptions GetElasticsearchOptions(IConfiguration configuration)
    {
        var options = new ElasticsearchOptions();
        configuration.Bind(ElasticsearchOptions.Elasticsearch, options);

        return options;
    }
}
