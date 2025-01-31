using System.Net;

namespace WebApi.RestClient.src.Response
{
    public class RestResponse
    {
        /// <summary>
        /// Indica se lo stato della risposta è positivo o no.
        /// </summary>
        public bool IsSuccessful { get; internal set; }

        /// <summary>
        /// Status code della risposta.
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Descrizione dello stato della risposta.
        /// </summary>
        public string? StatusDescription { get; internal set; }

        /// <summary>
        /// Eventuale messaggio d'errore.
        /// </summary>
        public string? ErrorBody { get; internal set; }
    }
}