using System;
using System.Collections.Generic;

namespace RestSharp.RequestBuilder.Interfaces
{
    public interface IRequestBuilder
    {
        int HeaderCount { get; }

        IRequestBuilder AddCookie(string name, string value);

        IRequestBuilder AddHeader(string name, string value);

        IRequestBuilder AddHeaders(Dictionary<string, string> headers);

        IRequestBuilder SetTimeout(TimeSpan timeout);

        IRequestBuilder SetMethod(Method method);

        IRequestBuilder AddParameter(Parameter parameter);

        IRequestBuilder AddParameters(Parameter[] parameters);

        IRequestBuilder RemoveHeaders();

        IRequestBuilder RemoveHeader(string name);

        IRequestBuilder RemoveCookies();

        IRequestBuilder RemoveParameters();

        IRequestBuilder RemoveParameter(Parameter parameter);

        IRequestBuilder SetFormat(DataFormat dataFormat);

        IRequestBuilder AddBody(object body);

        IRequestBuilder AddFile(string name, string path, string contentType = null);

        RestRequest Create();
    }
}