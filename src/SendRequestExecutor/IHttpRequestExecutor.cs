using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XSerializer;
using WebApi.RestClient.src.Response;
using WebApi.RestClient.src.Builder;
using System.Text.Json.Serialization.Metadata;
using WebApi.RestClient.src.ExtensionMethods;

namespace WebApi.RestClient.src.SendRequest
{
    public interface IHttpRequestExecutor
    {
        /// <summary>
        /// Send the HTTP request asynchronously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RestResponseWithContent> SendAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Send the HTTP request asynchronously with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RestResponseWithData<TOut>> SendAsync<TOut>(CancellationToken cancellationToken = default);
    }

    public class HttpRequestExecutor : IHttpRequestExecutor
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _requestMessage;
        private readonly BodyType _bodyType;
        private readonly RestClientOptions? _restClientOptions;

        public HttpRequestExecutor(HttpClient httpClient, HttpRequestMessage requestMessage, BodyType bodyType = BodyType.Json, RestClientOptions? jsonPropertyInfo = null!)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _requestMessage = requestMessage ?? throw new ArgumentNullException(nameof(requestMessage));
            _bodyType = bodyType;
            _restClientOptions = jsonPropertyInfo;
        }

        public async Task<RestResponseWithContent> SendAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateRequest();
                var response = await _httpClient.SendAsync(_requestMessage, cancellationToken);
                return await ResponseMapper.MapResponseAsync(response);
            }
            catch (Exception ex)
            {
                return new RestResponseWithContent
                {
                    IsSuccessful = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    StatusDescription = "Request failed",
                    ErrorBody = ex.Message
                };
            }
        }

        public async Task<RestResponseWithData<TOut>> SendAsync<TOut>(CancellationToken cancellationToken = default)
        {
            ValidateRequest();

            TOut? data = default;
            string content = string.Empty;
            HttpResponseMessage? httpResponseMessage = null;

            try
            {
                // Send the request
                httpResponseMessage = await _httpClient.SendAsync(_requestMessage, cancellationToken);
                content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    data = TryDeserialize<TOut>(content);
                }
            }
            catch (Exception ex)
            {
                return new RestResponseWithData<TOut>
                {
                    IsSuccessful = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    StatusDescription = "Request failed",
                    ErrorBody = ex.Message
                };
            }

            return ResponseMapper.MapResponseWithData(httpResponseMessage, data, content);
        }


        /// <summary>
        /// Validate the request before sending it.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ValidateRequest()
        {
            if (_requestMessage.Method == HttpMethod.Post || _requestMessage.Method == HttpMethod.Put)
            {
                if (_requestMessage.Content == null)
                    throw new InvalidOperationException("Content must be set for POST and PUT requests.");
            }
            if (_httpClient.BaseAddress == null)
            {
                throw new InvalidOperationException("BaseAddress must be set in the HttpClient.");
            }
        }

        private T? TryDeserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return default;
            JsonSerializerOptions jsonSerializer = _restClientOptions?.JsonSerializerOptions ?? new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return _bodyType switch
            {
                BodyType.Json => System.Text.Json.JsonSerializer.Deserialize<T>(content, jsonSerializer),
                BodyType.Xml => new XmlSerializer<T>().Deserialize(content),
                _ => default
            };
        }

    }

    public static class ResponseMapper
    {
        public static async Task<RestResponseWithContent> MapResponseAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return new RestResponseWithContent
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                StatusDescription = response.ReasonPhrase,
                Content = content
            };
        }

        public static RestResponseWithData<T> MapResponseWithData<T>(HttpResponseMessage response, T? data, string? errorBody)
        {
            return new RestResponseWithData<T>
            {
                Data = data,
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                StatusDescription = response.ReasonPhrase,
                ErrorBody = (!response.IsSuccessStatusCode ? errorBody : string.Empty)
            };
        }
    }
}
