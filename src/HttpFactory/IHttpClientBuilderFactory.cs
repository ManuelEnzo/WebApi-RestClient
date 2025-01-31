using System;
using System.Net.Http;
using WebApi.RestClient.src.Builder;

namespace WebApi.RestClient.src.HttpFactory
{
    public interface IHttpClientBuilderFactory
    {
        /// <summary>
        /// Factory to create a customized HttpClient, 
        /// returning an IHttpRequestBuilder for building and configuring HTTP requests.
        /// </summary>
        /// <param name="name">The name of the client registered in IHttpClientFactory, 
        /// which defines the HttpClient configuration to use (e.g., "default", "custom", etc.).</param>
        /// <returns>An IHttpRequestBuilder to build the HTTP request with the specified client.</returns>
        IHttpRequestBuilder CreateClient(string name);
    }

    public class HttpClientBuilderFactory(IHttpClientFactory httpClientFactory) : IHttpClientBuilderFactory
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public IHttpRequestBuilder CreateClient(string name)
        {
            var client = _httpClientFactory.CreateClient(name);
            return new HttpRequestBuilder(client);
        }
    }
}
