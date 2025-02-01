using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApi.RestClient.src.HttpFactory;

namespace WebApi.RestClient.src.ExtensionMethods
{
    /// <summary>
    /// Extension method for IServiceCollection
    /// </summary>
    public static class ServiceExtensionMethod
    {
        /// <summary>
        /// Add rest client to IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddRestClient(this IServiceCollection services, WebAssemblyHostBuilder? builder = null)
        {
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
                    return new HttpClientBuilderFactory(httpClient);
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
                        return new HttpClientBuilderFactory(httpClientFactory);
                    }
                    else
                    {
                        var httpClient = sp.GetRequiredService<HttpClient>();
                        return new HttpClientBuilderFactory(httpClient);
                    }
                });
            }

            return services;
        }
    }
}