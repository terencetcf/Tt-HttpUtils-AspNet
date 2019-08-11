using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Tt.HttpUtils
{
    [Serializable]
    public class HttpClientHelperException : HttpResponseException
    {
        private readonly string _message;

        public override string Message { get { return _message; } }

        public HttpClientHelperException(HttpStatusCode statusCode, string reasonPhrase, HttpContent content = null)
            : base(statusCode)
        {
            _message = reasonPhrase;
            Response.ReasonPhrase = reasonPhrase;
            Response.Content = content;
        }
    }
}