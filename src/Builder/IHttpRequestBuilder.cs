using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebApi.RestClient.src.ExtensionMethods;
using WebApi.RestClient.src.SendRequest;

namespace WebApi.RestClient.src.Builder
{
    public interface IHttpRequestBuilder : IDisposable
    {
        /// <summary>
        /// Add a header to the request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithHeader(string key, string value);
        /// <summary>
        /// Add a query parameter to the request.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithQueryParameter(string key, string value);
        /// <summary>
        /// Add multiple query parameters to the request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="bodyType"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithQueryParameters(Dictionary<string, string> parameters);
        /// <summary>
        /// Add a body to the request.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="bodyType"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithBody<T>(T content, BodyType bodyType = BodyType.Json);
        /// <summary>
        /// Add an endpoint to the request.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithEndpoint(string endpoint);
        /// <summary>
        /// Add a method to the request.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithMethod(HttpMethod method);

        /// <summary>
        /// Add a time out to the request.
        /// </summary>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithTimeOut(TimeSpan? timeOut);

        /// <summary>
        /// Set the authorization for the request.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        IHttpRequestBuilder WithAuthorization(AuthorizationEnum type, string token);

        /// <summary>
        /// Build the request.
        /// </summary>
        /// <returns></returns>
        IHttpRequestExecutor BuildRequest();
    }

    public class HttpRequestBuilder(HttpClient httpClient, RestClientOptions? clientOptions) : IHttpRequestBuilder
    {
        private string? _resource;
        private HttpMethod _method;
        private readonly Dictionary<string, string> _parameters = new();
        private readonly Dictionary<string, string> _headers = new();
        private HttpContent? _body;
        private BodyType _bodyType;
        private readonly HttpClient _httpClient = httpClient;
        private readonly RestClientOptions? _clientOptions = clientOptions;
        private TimeSpan? _timeOut;
        public IHttpRequestBuilder WithHeader(string key, string value)
        {
            if (value == null)
                throw new ArgumentException("L'header non può essere null", nameof(value));
            _headers[key] = value;
            return this;
        }

        public IHttpRequestBuilder WithQueryParameters(Dictionary<string, string> parameters)
        {
            foreach (var (key, value) in parameters)
            {
                WithQueryParameter(key, value);
            }
            return this;
        }

        public IHttpRequestBuilder WithQueryParameter(string key, string value)
        {
            _parameters[key] = value;
            return this;
        }

        public IHttpRequestBuilder WithBody<T>(T content, BodyType bodyType = BodyType.Json)
        {
            _bodyType = bodyType;
            _body = bodyType switch
            {
                BodyType.Json => new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
                BodyType.Xml => new StringContent(ToXml(content), Encoding.UTF8, "application/xml"),
                _ => throw new NotSupportedException("Formato body non supportato")
            };
            return this;
        }

        public IHttpRequestBuilder WithEndpoint(string endpoint)
        {
            _resource = endpoint;
            return this;
        }

        public IHttpRequestBuilder WithMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        public IHttpRequestBuilder WithTimeOut(TimeSpan? timeOut)
        {
            _timeOut = timeOut;
            return this;
        }

        public IHttpRequestExecutor BuildRequest()
        {
            return new HttpRequestExecutor(_httpClient, BuildHttpRequestMessage(), _bodyType, _clientOptions);
        }


        private HttpRequestMessage BuildHttpRequestMessage()
        {
            // Check if the method is null
            if (_method == null)
                throw new InvalidOperationException("Il metodo HTTP non può essere null");

            // Build the URI (BaseAddress is already set in the HttpClient)
            var uriBuilder = new StringBuilder(_resource ?? string.Empty);

            // Add query parameters to request
            if (_parameters.Count != 0)
            {
                var query = string.Join("&", _parameters.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));
                uriBuilder.Append('?').Append(query);
            }

            var request = new HttpRequestMessage(_method, uriBuilder.ToString());

            // Add headers to request
            foreach (var header in _headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // Add body to request if it's a GET or DELETE request
            if (_body != null && !(_method == HttpMethod.Get || _method == HttpMethod.Delete))
            {
                request.Content = _body;
            }

            // Add time out to request
            if (_timeOut != null)
            {
                _httpClient.Timeout = _timeOut.Value;
            }

            return request;
        }

        private static string ToXml<T>(T obj)
        {
            var xml = new XElement("root");
            foreach (var prop in obj!.GetType().GetProperties())
            {
                xml.Add(new XElement(prop.Name, prop.GetValue(obj)));
            }
            return xml.ToString();
        }

        public IHttpRequestBuilder WithAuthorization(AuthorizationEnum type, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Il token non può essere null o vuoto", nameof(token));
            if (_httpClient.DefaultRequestHeaders.Authorization != null)
                throw new InvalidOperationException("Authorization is already set");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(type.ToString(), token);
            return this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
       
    }

    public enum BodyType
    {
        Json,
        Xml
    }
}
