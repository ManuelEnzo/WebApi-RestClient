using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.RestClient.src
{
    public enum AuthorizationEnum
    {
        None,
        Basic,
        Bearer,
        Digest,
        Hawk,
        AWS4HMACSHA256,
        AWS4HMACSHA256Query,
    }
}
