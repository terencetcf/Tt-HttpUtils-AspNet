namespace Tt.HttpUtils.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IHttpClientHelper : IDisposable
    {
        string ServiceUri { get; set; }

        TimeSpan Timeout { get; set; }

        Task Delete(string relativeUrl);

        Task<T> Get<T>(string relativeUrl);

        Task<T> Get<T>(string relativeUrl, object parameters);

        Task<T> Get<T>(string relativeUrl, bool includeUsernameAndSession);

        Task<T> Get<T>(string relativeUrl, object parameters, bool includeUsernameAndSession);

        Task Patch(string relativeUrl, object patchObject);

        Task Patch(string relativeUrl, object patchObject, bool includeUsernameAndSession);

        Task<T> Post<T>(string relativeUrl, object postObject);

        Task<T> Post<T>(string relativeUrl, object postObject, bool includeUsernameAndSession);

        Task<T> Put<T>(string relativeUrl, object putObject);

        Task<T> Put<T>(string relativeUrl, object putObject, bool includeUsernameAndSession);

        Task Put(string relativeUrl, object putObject);

        Task Put(string relativeUrl, object putObject, bool includeUsernameAndSession);
    }
}