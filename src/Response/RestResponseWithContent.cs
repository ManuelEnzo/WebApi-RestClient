using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.RestClient.src.Response
{
    public class RestResponseWithContent : RestResponse
    {
        /// <summary>
        /// Contenuto della risposta.
        /// </summary>
        public string? Content { get;  set; }    
    }
}
