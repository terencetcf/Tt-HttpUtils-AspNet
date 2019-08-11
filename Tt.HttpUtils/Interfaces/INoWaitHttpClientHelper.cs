namespace Tt.HttpUtils.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface INoWaitHttpClientHelper : IDisposable
    {
        string ServiceUri { get; set; }

        Task Delete(string relativeUrl);

        Task Patch(string relativeUrl, object patchObject);

        Task Patch(string relativeUrl, object patchObject, bool includeUsernameAndSession);

        Task Post(string relativeUrl, object postObject);

        Task Post(string relativeUrl, object postObject, bool includeUsernameAndSession);

        Task Put(string relativeUrl, object putObject);

        Task Put(string relativeUrl, object putObject, bool includeUsernameAndSession);
    }
}