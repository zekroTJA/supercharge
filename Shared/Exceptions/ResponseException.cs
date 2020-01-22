using System;
using System.Net.Http;

namespace RiotAPIAccessLayer.Exceptions
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
