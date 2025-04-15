using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using WebApi.RestClient.src.HttpFactory;

namespace WebApi.RestClient.src.ExtensionMethods
{
    /// <summary>
    /// Extension method for IServiceCollection
    /// </summary>
    public static class ServiceExtensionMethod
    {
        /// <summary>
        /// Adds the REST client to the <see cref="IServiceCollection"/>, configuring it for either 
        /// Blazor WebAssembly or Blazor Server depending on the context.
        /// </summary>
        /// <param name="services">The application's service collection.</param>
        /// <param name="builder">
        /// The Blazor WebAssembly host builder (optional). If provided, it is used to configure 
        /// the <see cref="HttpClient"/> instance on the client side.
        /// </param>
        /// <param name="configureOptions">
        /// Optional action to configure the REST client options (<see cref="RestClientOptions"/>).
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRestClient(this IServiceCollection services, WebAssemblyHostBuilder? builder = null, Action<RestClientOptions>? configureOptions = null)
        {
            var options = new RestClientOptions();
            configureOptions?.Invoke(options); 

            if (builder != null)
            {
                // Use when the application is running on the client side (Blazor WebAssembly)
                services.AddScoped<IHttpClientBuilderFactory>(sp =>
                {
                    var httpClient = sp.GetService<HttpClient>();
                    if (httpClient == null)
                    {
                        httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                    }
                    return new HttpClientBuilderFactory(httpClient, options);
                });
            }
            else
            {
                // Use when the application is running on the server side (Blazor Server)
                services.AddScoped<IHttpClientBuilderFactory, HttpClientBuilderFactory>(sp =>
                {
                    var httpClientFactory = sp.GetService<IHttpClientFactory>();
                    if (httpClientFactory != null)
                    {
                        return new HttpClientBuilderFactory(httpClientFactory, options);
                    }
                    else
                    {
                        var httpClient = sp.GetRequiredService<HttpClient>();
                        return new HttpClientBuilderFactory(httpClient, options);
                    }
                });
            }

            return services;
        }

        /// <summary>
        /// Adds the REST client to the <see cref="IServiceCollection"/> with optional configuration.
        /// Defaults to server-side setup.
        /// </summary>
        /// <param name="services">The application's service collection.</param>
        /// <param name="configureOptions">
        /// Optional action to configure the REST client options (<see cref="RestClientOptions"/>).
        /// </param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>

        public static IServiceCollection AddRestClient(this IServiceCollection services, Action<RestClientOptions> configureOptions)
        {
            return AddRestClient(services, null, configureOptions);
        }
    }

    public class RestClientOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = null!;
    }

}