using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPiChallenge.Core.DTOs
{
    public class LoginResDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
