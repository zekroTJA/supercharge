using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Crawler.Exceptions
{
    class ResponseException : Exception
    {
        public readonly HttpResponseMessage Response;

        public ResponseException()
        {

        }

        public ResponseException(HttpResponseMessage msg) 
            : base($"Code {msg.StatusCode}: ${msg.Content.ToString()}")
        {
            Response = msg;
        }
    }
}
