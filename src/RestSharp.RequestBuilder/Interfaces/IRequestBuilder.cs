using System;
using System.Collections.Generic;

namespace RestSharp.RequestBuilder.Interfaces
{
    /// <summary>
    /// Interface for building REST requests.
    /// </summary>
    public interface IRequestBuilder
    {
        /// <summary>
        /// Gets the count of headers added to the request.
        /// </summary>
        int HeaderCount { get; }

        /// <summary>
        /// Adds a cookie to the request.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddCookie(string name, string value, string path, string domain);

        /// <summary>
        /// Adds a header to the request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddHeader(string name, string value);

        /// <summary>
        /// Adds multiple headers to the request.
        /// </summary>
        /// <param name="headers">A dictionary of headers to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddHeaders(IDictionary<string, string> headers);

        /// <summary>
        /// Adds a body to the request.
        /// </summary>
        /// <param name="body">The body to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddBody(object body);

        /// <summary>
        /// Adds a body to the request with a specified data format.
        /// </summary>
        /// <typeparam name="T">The type of the body.</typeparam>
        /// <param name="body">The body to add.</param>
        /// <param name="dataFormat">The data format of the body.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddBody<T>(T body, DataFormat dataFormat) where T : class;

        /// <summary>
        /// Adds a file to the request.
        /// </summary>
        /// <param name="name">The name of the file parameter.</param>
        /// <param name="path">The path to the file.</param>
        /// <param name="contentType">The content type of the file. Optional.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFile(string name, string path, string contentType = null);

        /// <summary>
        /// Adds a parameter to the request.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddParameter(Parameter parameter);

        /// <summary>
        /// Adds multiple parameters to the request.
        /// </summary>
        /// <param name="parameters">An array of parameters to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddParameters(Parameter[] parameters);

        /// <summary>
        /// Removes all headers from the request.
        /// </summary>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder RemoveHeaders();

        /// <summary>
        /// Removes a specific header from the request.
        /// </summary>
        /// <param name="name">The name of the header to remove.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder RemoveHeader(string name);

        /// <summary>
        /// Removes all cookies from the request.
        /// </summary>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder RemoveCookies();

        /// <summary>
        /// Removes all parameters from the request.
        /// </summary>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder RemoveParameters();

        /// <summary>
        /// Removes a specific parameter from the request.
        /// </summary>
        /// <param name="parameter">The parameter to remove.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder RemoveParameter(Parameter parameter);

        /// <summary>
        /// Sets the data format for the request.
        /// </summary>
        /// <param name="dataFormat">The data format to set.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder SetFormat(DataFormat dataFormat);

        /// <summary>
        /// Sets the timeout for the request.
        /// </summary>
        /// <param name="timeout">The timeout duration.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder SetTimeout(TimeSpan timeout);

        /// <summary>
        /// Sets the HTTP method for the request.
        /// </summary>
        /// <param name="method">The HTTP method to set.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder SetMethod(Method method);

        /// <summary>
        /// Creates the <see cref="RestRequest"/> based on the configured parameters.
        /// </summary>
        /// <returns>A new instance of <see cref="RestRequest"/>.</returns>
        RestRequest Create();
    }
}