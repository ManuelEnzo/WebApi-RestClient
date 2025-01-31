using System.Net;

namespace WebApi.RestClient.src.Response
{
    public class RestResponse
    {
        /// <summary>
        /// Indicate if the request was successful.
        /// </summary>
        public bool IsSuccessful { get; internal set; }

        /// <summary>
        /// Indicate status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Description of the status code.
        /// </summary>
        public string? StatusDescription { get; internal set; }

        /// <summary>
        /// Error message if the request was not successful.
        /// </summary>
        public string? ErrorBody { get; internal set; }
    }
}