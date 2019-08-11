namespace Tt.HttpUtils
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Services.WebApi;

    using Newtonsoft.Json;

    using Tt.HttpUtils.Interfaces;

    /// <summary>
    /// HttpClient in a fire-and-forget manner. Client will timeout after 500 milliseconds - enough to send the request to the server
    /// </summary>
    public class NoWaitHttpClientHelper : BaseHttpClientHelper, INoWaitHttpClientHelper
    {
        public NoWaitHttpClientHelper()
            : base(null)
        {
            Client.Timeout = TimeSpan.FromMilliseconds(500);
        }

        public NoWaitHttpClientHelper(ISessionHelper helper)
            : base(helper)
        {
            Client.Timeout = TimeSpan.FromMilliseconds(500);
        }

        public virtual async Task Delete(string relativeUrl)
        {
            await ExecuteMinWait(c => c.DeleteAsync(relativeUrl));
        }

        public virtual async Task Patch(string relativeUrl, object patchObject)
        {
            await Patch(relativeUrl, patchObject, true);
        }

        public virtual async Task Patch(string relativeUrl, object patchObject, bool includeUsernameAndSession)
        {
            var serialisedPatchObject = JsonConvert.SerializeObject(patchObject);
            var content = new StringContent(serialisedPatchObject, Encoding.UTF8, "application/json");

            await ExecuteMinWait(c => c.PatchAsync(relativeUrl, content), includeUsernameAndSession);
        }

        public virtual async Task Post(string relativeUrl, object postObject)
        {
            await Post(relativeUrl, postObject, true);
        }

        public virtual async Task Post(string relativeUrl, object postObject, bool includeUsernameAndSession)
        {
            await ExecuteMinWait(c => c.PostAsync(relativeUrl, postObject, new JsonMediaTypeFormatter()), includeUsernameAndSession);
        }

        public virtual async Task Put(string relativeUrl, object putObject)
        {
            await Put(relativeUrl, putObject, true);
        }

        public virtual async Task Put(string relativeUrl, object putObject, bool includeUsernameAndSession)
        {
            await ExecuteMinWait(c => c.PostAsync(relativeUrl, putObject, new JsonMediaTypeFormatter()), includeUsernameAndSession);
        }

        protected override void SmartEnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response == null)
            {
                return;
            }

            base.SmartEnsureSuccessStatusCode(response);
        }

        private async Task ExecuteMinWait(
            Func<HttpClient, Task<HttpResponseMessage>> func,
            bool includeUsernameAndSession = true)
        {
            if (includeUsernameAndSession)
            {
                SetUsernameAndSession();
            }

            HttpResponseMessage response;

            try
            {
                response = await func(Client);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            SmartEnsureSuccessStatusCode(response);
        }
    }
}
