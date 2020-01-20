using System;
using System.Net.Http;

namespace Shared.Exceptions
{
    public class ResponseException : Exception
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
