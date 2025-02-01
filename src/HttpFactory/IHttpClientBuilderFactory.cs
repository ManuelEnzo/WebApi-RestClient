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

        /// <summary>
        /// Factory to create a default HttpClient, this method is used for application that 
        /// not using IHttpClientFactory, like Blazor WebAssembly.
        /// </summary>
        /// <returns></returns>
        IHttpRequestBuilder CreateClient();
    }

    public class HttpClientBuilderFactory : IHttpClientBuilderFactory
    {
        private readonly IHttpClientFactory? _httpClientFactory;
        private readonly HttpClient? _httpClient;

        /// <summary>
        /// constructor for application that using IHttpClientFactory
        /// </summary>
        /// <param name="httpClientFactory"></param>
        public HttpClientBuilderFactory(IHttpClientFactory? httpClientFactory) => _httpClientFactory = httpClientFactory;

        /// <summary>
        /// Costructor for application that not using IHttpClientFactory
        /// </summary>
        /// <param name="httpClient"></param>
        public HttpClientBuilderFactory(HttpClient? httpClient) => _httpClient = httpClient;


        public IHttpRequestBuilder CreateClient(string name)
        {
            if (_httpClientFactory is null)
            {
                throw new ArgumentNullException(nameof(_httpClientFactory));
            }
            return new HttpRequestBuilder(_httpClientFactory.CreateClient(name));
        }

        public IHttpRequestBuilder CreateClient()
        {
            if (_httpClient is null)
            {
                throw new ArgumentNullException(nameof(_httpClient));
            }
            return new HttpRequestBuilder(_httpClient);
        }
    }
}
