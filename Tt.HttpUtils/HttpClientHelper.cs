namespace Tt.HttpUtils
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.Services.WebApi;

    using Newtonsoft.Json;

    using Tt.HttpUtils.Interfaces;

    /// <summary>
    ///     A wrapper of HttpClient for a web api REST service that optionally allows
    ///     authentication to be added to the header of the request that later to be
    ///     checked using the ActionFilter in the web api controller methods.
    ///     Example Usage:
    ///     var httpClientHelper = new HttpClientHelper();
    ///     Example method calls:
    ///     var getSingleResult = httpClientHelper.Get("ApiMethod/1").Result;
    ///     var postResult = httpClientHelper.Post("ApiMethod/", object).Result;
    ///     httpClientHelper.Put("ApiMethod/3", Tobject).Wait();
    ///     httpClientHelper.Delete("ApiMethod/3").Wait();
    /// </summary>
    public class HttpClientHelper : BaseHttpClientHelper, IHttpClientHelper
    {
        public HttpClientHelper()
            : base(null)
        {
        }

        public HttpClientHelper(ISessionHelper helper)
            : base(helper)
        {
        }

        /// <summary>
        ///     Gets or sets the timespan to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return Client.Timeout;
            }

            set
            {
                Client.Timeout = value;
            }
        }

        /// <summary>
        ///     For deleting an existing item over a web api using DELETE
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api delete method, e.g. "DTOs/3" to delete DTO with id of 3
        /// </param>
        public virtual async Task Delete(string relativeUrl)
        {
            SetUsernameAndSession();

            HttpResponseMessage response;

            try
            {
                response = await Client.DeleteAsync(relativeUrl);
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientHelperException(HttpStatusCode.GatewayTimeout, ex.Message);
            }

            SmartEnsureSuccessStatusCode(response);
        }

        /// <summary>
        ///     For getting a single item from a web api using GET
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api get method, e.g. "DTOs/1" to get a DTO with an id of 1
        /// </param>
        /// <returns>The item requested</returns>
        public virtual async Task<TResult> Get<TResult>(string relativeUrl)
        {
            return await Get<TResult>(relativeUrl, null, true);
        }

        /// <summary>
        ///     For getting a single item from a web api using GET
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api get method, e.g. "DTOs/1" to get a DTO with an id of 1
        /// </param>
        /// <param name="parameters">The query string parameters</param>
        /// <returns>The item requested</returns>
        public virtual async Task<TResult> Get<TResult>(string relativeUrl, object parameters)
        {
            return await Get<TResult>(relativeUrl, parameters, true);
        }

        /// <summary>
        ///     For getting a single item from a web api using GET
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api get method, e.g. "DTOs/1" to get a DTO with an id of 1
        /// </param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        /// <returns>The item requested</returns>
        public virtual async Task<TResult> Get<TResult>(string relativeUrl, bool includeUsernameAndSession)
        {
            return await Get<TResult>(relativeUrl, null, includeUsernameAndSession);
        }

        /// <summary>
        ///     For getting a single item from a web api using GET
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api get method, e.g. "DTOs/1" to get a DTO with an id of 1
        /// </param>
        /// <param name="parameters">The QueryString parameters</param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        /// <returns>The item requested</returns>
        public virtual async Task<TResult> Get<TResult>(
            string relativeUrl,
            object parameters,
            bool includeUsernameAndSession)
        {
            if (includeUsernameAndSession)
            {
                SetUsernameAndSession();
            }

            HttpResponseMessage response;

            try
            {
                response = await Client.GetAsync(relativeUrl + parameters.ToQueryString());
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientHelperException(HttpStatusCode.GatewayTimeout, ex.Message);
            }

            SmartEnsureSuccessStatusCode(response);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(jsonString);
        }

        /// <summary>
        ///     Sends a request to execute a partial update on the resource.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="patchObject">The partial resource to be updated.</param>
        /// <returns>A task with the status of the operation.</returns>
        public virtual async Task Patch(string relativeUrl, object patchObject)
        {
            await Patch(relativeUrl, patchObject, true);
        }

        /// <summary>
        ///     Sends a request to execute a partial update on the resource.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="patchObject">The partial resource to be updated.</param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        /// <returns>A task with the status of the operation.</returns>
        public virtual async Task Patch(string relativeUrl, object patchObject, bool includeUsernameAndSession)
        {
            if (includeUsernameAndSession)
            {
                SetUsernameAndSession();
            }

            HttpResponseMessage response;

            try
            {
                var serialisedPatchObject = JsonConvert.SerializeObject(patchObject);
                var content = new StringContent(serialisedPatchObject, Encoding.UTF8, "application/json");

                response = await Client.PatchAsync(relativeUrl, content);
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientHelperException(HttpStatusCode.GatewayTimeout, ex.Message);
            }

            SmartEnsureSuccessStatusCode(response);
        }

        /// <summary>
        ///     For creating a new item over a web api using POST allowing the client to not set username and session
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api post method, e.g. "DTOs" to add DTOs
        /// </param>
        /// <param name="postObject">The object to be created</param>
        /// <returns>The item created</returns>
        public virtual async Task<TResult> Post<TResult>(string relativeUrl, object postObject)
        {
            return await Post<TResult>(relativeUrl, postObject, true);
        }

        /// <summary>
        ///     For creating a new item over a web api using POST allowing the client to not set username and session
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api post method, e.g. "DTOs" to add DTOs
        /// </param>
        /// <param name="postObject">The object to be created</param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        /// <returns>The item created</returns>
        public virtual async Task<TResult> Post<TResult>(
            string relativeUrl,
            object postObject,
            bool includeUsernameAndSession)
        {
            if (includeUsernameAndSession)
            {
                SetUsernameAndSession();
            }

            HttpResponseMessage response;

            try
            {
                response = await Client.PostAsync(relativeUrl, postObject, new JsonMediaTypeFormatter());
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientHelperException(HttpStatusCode.GatewayTimeout, ex.Message);
            }

            SmartEnsureSuccessStatusCode(response);

            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(jsonString);
        }

        /// <summary>
        ///     For updating an existing item over a web api using PUT
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api put method, e.g. "DTOs/3" to update DTO with id of 3
        /// </param>
        /// <param name="putObject">The object to be edited</param>
        public virtual async Task<TResult> Put<TResult>(string relativeUrl, object putObject)
        {
            return await Put<TResult>(relativeUrl, putObject, true);
        }

        /// <summary>
        ///     For updating an existing item over a web api using PUT
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api put method, e.g. "DTOs/3" to update DTO with id of 3
        /// </param>
        /// <param name="putObject">The object to be edited</param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        public virtual async Task<TResult> Put<TResult>(
            string relativeUrl,
            object putObject,
            bool includeUsernameAndSession)
        {
            var response = await SendPutRequestAsync(relativeUrl, putObject, includeUsernameAndSession);

            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        ///     For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api put method, e.g. "DTOs/3" to update DTO with id of 3
        /// </param>
        /// <param name="putObject">The object to be edited</param>
        public virtual async Task Put(string relativeUrl, object putObject)
        {
            await Put(relativeUrl, putObject, true);
        }

        /// <summary>
        ///     For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="relativeUrl">
        ///     Added to the base address to make the full url of the
        ///     api put method, e.g. "DTOs/3" to update DTO with id of 3
        /// </param>
        /// <param name="putObject">The object to be edited</param>
        /// <param name="includeUsernameAndSession">
        ///     A value indicating whether the username and session should be included in the
        ///     header of the request.
        /// </param>
        public virtual async Task Put(string relativeUrl, object putObject, bool includeUsernameAndSession)
        {
            await SendPutRequestAsync(relativeUrl, putObject, includeUsernameAndSession);
        }

        private async Task<HttpResponseMessage> SendPutRequestAsync(
            string relativeUrl,
            object putObject,
            bool includeUsernameAndSession)
        {
            if (includeUsernameAndSession)
            {
                SetUsernameAndSession();
            }

            HttpResponseMessage response;

            try
            {
                response = await Client.PutAsync(relativeUrl, putObject, new JsonMediaTypeFormatter());
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientHelperException(HttpStatusCode.GatewayTimeout, ex.Message);
            }

            SmartEnsureSuccessStatusCode(response);
            return response;
        }
    }
}