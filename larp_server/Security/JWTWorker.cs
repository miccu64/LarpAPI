using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using larp_server.Others;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace larp_server.Security
{
    public class JWTWorker
    {
        private IJwtEncoder Encoder;
        public JWTWorker()
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            Encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
        }
        //create token
        public string Encode(string name, string email)
        {
            //this is what token will contain
            var payload = new Dictionary<string, string>
            {
                { "Name", name },
                { "Email", email }
            };
            var token = Encoder.Encode(payload, Constants.JWTSecret);
            return token;
        }
    }
}
