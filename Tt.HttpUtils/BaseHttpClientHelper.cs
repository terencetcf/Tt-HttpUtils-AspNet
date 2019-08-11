namespace Tt.HttpUtils
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Tt.HttpUtils.Interfaces;

    public abstract class BaseHttpClientHelper : IDisposable
    {
        private bool alreadyDisposed;

        protected BaseHttpClientHelper()
            : this(null)
        {
        }

        protected BaseHttpClientHelper(ISessionHelper helper)
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Helper = helper;
        }

        public string ServiceUri
        {
            get
            {
                return Client.BaseAddress.ToString();
            }

            set
            {
                Client.BaseAddress = new Uri(value.EndsWith("/") ? value : value + "/");
            }
        }

        protected HttpClient Client { get; private set; }

        protected ISessionHelper Helper { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (alreadyDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (Client != null)
                {
                    Client.Dispose();
                }
            }

            alreadyDisposed = true;
        }

        protected virtual void SetUsernameAndSession()
        {
            if (Helper == null)
            {
                return;
            }

            var username = Helper.Username;
            var session = Helper.Session;

            if (string.IsNullOrWhiteSpace(session) || string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            Client.DefaultRequestHeaders.Remove("Session");
            Client.DefaultRequestHeaders.Remove("Username");

            Client.DefaultRequestHeaders.Add("Session", session);
            Client.DefaultRequestHeaders.Add("Username", username);
        }

        protected virtual void SmartEnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if ((int)response.StatusCode >= 400)
            {
                throw new HttpClientHelperException(response.StatusCode, response.ReasonPhrase, response.Content);
            }
        }
    }
}