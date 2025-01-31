using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.RestClient.src.Response
{
    public class RestResponseWithData<T> : RestResponse 
    {
        /// <summary>
        /// Dato deserializzato della risposta.
        /// </summary>
        public T? Data { get; internal set; }
    }
}
