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
        /// Content of the response.
        /// </summary>
        public string? Content { get;  set; }    
    }
}
