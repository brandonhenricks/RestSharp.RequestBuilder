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
        /// Adds a JSON body to the request.
        /// Sets the request format to JSON and adds the body.
        /// </summary>
        /// <typeparam name="T">The type of the body.</typeparam>
        /// <param name="body">The body to add. Will be serialized as JSON.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddJsonBody<T>(T body) where T : class;

        /// <summary>
        /// Adds an XML body to the request.
        /// Sets the request format to XML and adds the body.
        /// </summary>
        /// <typeparam name="T">The type of the body.</typeparam>
        /// <param name="body">The body to add. Will be serialized as XML.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddXmlBody<T>(T body) where T : class;

        /// <summary>
        /// Adds a form URL encoded body to the request.
        /// Adds each key-value pair as a GetOrPost parameter.
        /// </summary>
        /// <param name="data">Dictionary of form data to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFormUrlEncodedBody(IDictionary<string, string> data);

        /// <summary>
        /// Adds a file to the request.
        /// </summary>
        /// <param name="name">The name of the file parameter.</param>
        /// <param name="path">The path to the file.</param>
        /// <param name="contentType">The content type of the file. Optional.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFile(string name, string path, string contentType = null);

        /// <summary>
        /// Adds multiple files to the request.
        /// </summary>
        /// <param name="files">Array of tuples containing (name, path, contentType).</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFiles(params (string name, string path, string contentType)[] files);

        /// <summary>
        /// Adds a file from a byte array to the request.
        /// </summary>
        /// <param name="name">The name of the file parameter.</param>
        /// <param name="bytes">The file content as byte array.</param>
        /// <param name="fileName">The file name to use.</param>
        /// <param name="contentType">The content type of the file. Optional.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFileBytes(string name, byte[] bytes, string fileName, string contentType = null);

        /// <summary>
        /// Adds a file from a stream to the request.
        /// </summary>
        /// <param name="name">The name of the file parameter.</param>
        /// <param name="stream">The stream containing file data.</param>
        /// <param name="fileName">The file name to use.</param>
        /// <param name="contentType">The content type of the file. Optional.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddFileStream(string name, System.IO.Stream stream, string fileName, string contentType = null);

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
        /// Adds a query string parameter to the request.
        /// </summary>
        /// <param name="name">The name of the query parameter.</param>
        /// <param name="value">The value of the query parameter. Will be converted to string using InvariantCulture.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddQueryParameter(string name, object value);

        /// <summary>
        /// Adds multiple query string parameters to the request.
        /// </summary>
        /// <param name="parameters">A dictionary of query parameters to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddQueryParameters(IDictionary<string, object> parameters);

        /// <summary>
        /// Adds a URL segment parameter to the request.
        /// </summary>
        /// <param name="name">The name of the URL segment.</param>
        /// <param name="value">The value of the URL segment.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder AddUrlSegment(string name, string value);

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
        /// Adds a Bearer token to the Authorization header.
        /// </summary>
        /// <param name="token">The bearer token to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithBearerToken(string token);

        /// <summary>
        /// Adds Basic authentication to the Authorization header.
        /// Credentials are Base64-encoded as "username:password".
        /// </summary>
        /// <param name="username">The username for basic authentication.</param>
        /// <param name="password">The password for basic authentication.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithBasicAuth(string username, string password);

        /// <summary>
        /// Adds an API key to a specified header.
        /// </summary>
        /// <param name="key">The API key to add.</param>
        /// <param name="headerName">The name of the header. Defaults to "X-API-Key".</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithApiKey(string key, string headerName = "X-API-Key");

        /// <summary>
        /// Adds an OAuth2 access token to the Authorization header as a Bearer token.
        /// </summary>
        /// <param name="accessToken">The OAuth2 access token to add.</param>
        /// <returns>The current instance of <see cref="IRequestBuilder"/>.</returns>
        IRequestBuilder WithOAuth2(string accessToken);

        /// <summary>
        /// Creates the <see cref="RestRequest"/> based on the configured parameters.
        /// </summary>
        /// <returns>A new instance of <see cref="RestRequest"/>.</returns>
        RestRequest Create();
    }
}
