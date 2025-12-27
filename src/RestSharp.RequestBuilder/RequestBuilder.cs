using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RestSharp.RequestBuilder.Interfaces;
using RestSharp.RequestBuilder.Models;

namespace RestSharp.RequestBuilder
{
    /// <summary>
    /// RestRequestBuilder is a helper class library that utilizes Fluent Syntax in order to create
    /// RestRequest objects.
    /// </summary>
    public sealed class RequestBuilder : IRequestBuilder
    {
        #region Private Properties

        /// <summary>
        /// Represents a file attachment with various source types.
        /// </summary>
        private sealed class FileAttachment
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string ContentType { get; set; }
            public byte[] Bytes { get; set; }
            public Stream Stream { get; set; }
            public string FileName { get; set; }
        }

        private readonly string _resource;
        private readonly Dictionary<string, string> _headers;
        private readonly HashSet<CookieValue> _cookieValues;
        private readonly List<Parameter> _parameters;
        private readonly List<FileAttachment> _files;
        private DataFormat _dataFormat;
        private Method _method;
        private object _body;
        private TimeSpan? _timeOut;

        #endregion Private Properties

        #region Public Properties

        /// <inheritdoc/>
        public int HeaderCount => _headers.Count;

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Public Constructor with string as argument.
        /// </summary>
        /// <param name="resource"></param>
        public RequestBuilder(string resource)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = resource;
            _headers = new Dictionary<string, string>();
            _parameters = new List<Parameter>();
            _files = new List<FileAttachment>();
            _method = Method.Get;
            _dataFormat = DataFormat.Json;
            _cookieValues = new HashSet<CookieValue>(new CookieValueComparer());
            _timeOut = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Public Constructor with a Uri argument.
        /// Uri argument is stored as string.
        /// </summary>
        /// <param name="resource"></param>
        public RequestBuilder(Uri resource)
        {
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = resource.ToString();
            _headers = new Dictionary<string, string>();
            _parameters = new List<Parameter>();
            _files = new List<FileAttachment>();
            _method = Method.Get;
            _dataFormat = DataFormat.Json;
            _cookieValues = new HashSet<CookieValue>(new CookieValueComparer());
            _timeOut = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Public Constructor with a Uri argument.
        /// Uri argument is stored as string.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        public RequestBuilder(Uri resource, Method method)
        {
            if (resource is null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = resource.ToString();
            _headers = new Dictionary<string, string>();
            _parameters = new List<Parameter>();
            _files = new List<FileAttachment>();
            _method = method;
            _dataFormat = DataFormat.Json;
            _cookieValues = new HashSet<CookieValue>(new CookieValueComparer());
            _timeOut = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// RequestBuilder Constructor with <see cref="Method"/> argument.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        public RequestBuilder(string resource, Method method)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = resource;
            _headers = new Dictionary<string, string>();
            _parameters = new List<Parameter>();
            _files = new List<FileAttachment>();
            _method = method;
            _dataFormat = DataFormat.Json;
            _cookieValues = new HashSet<CookieValue>(new CookieValueComparer());
            _timeOut = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// RequestBuilder Constructor with <see cref="Method"/> and <see cref="DataFormat"/> arguments.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <param name="format"></param>
        public RequestBuilder(string resource, Method method, DataFormat format)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentNullException(nameof(resource));
            }

            _resource = resource;
            _headers = new Dictionary<string, string>();
            _parameters = new List<Parameter>();
            _files = new List<FileAttachment>();
            _method = method;
            _dataFormat = format;
            _cookieValues = new HashSet<CookieValue>(new CookieValueComparer());
            _timeOut = TimeSpan.FromSeconds(30);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public IRequestBuilder AddBody(object body)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddBody<T>(T body, DataFormat dataFormat) where T : class
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;

            _dataFormat = dataFormat;

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddJsonBody<T>(T body) where T : class
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;
            _dataFormat = DataFormat.Json;

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddXmlBody<T>(T body) where T : class
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;
            _dataFormat = DataFormat.Xml;

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddFormUrlEncodedBody(IDictionary<string, string> data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            foreach (var kvp in data.Where(kvp => kvp.Value != null))
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    throw new ArgumentException("Dictionary keys cannot be null or empty.", nameof(data));
                }

                var parameter = new GetOrPostParameter(kvp.Key, kvp.Value);
                AddParameter(parameter);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddFile(string name, string path, string contentType = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            _files.Add(new FileAttachment
            {
                Name = name,
                Path = path,
                ContentType = contentType
            });

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddFiles(params (string name, string path, string contentType)[] files)
        {
            if (files == null || files.Length == 0)
            {
                return this;
            }

            foreach (var (name, path, contentType) in files)
            {
                AddFile(name, path, contentType);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddFileBytes(string name, byte[] bytes, string fileName, string contentType = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length == 0)
            {
                throw new ArgumentException("Byte array cannot be empty.", nameof(bytes));
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            _files.Add(new FileAttachment
            {
                Name = name,
                Bytes = bytes,
                FileName = fileName,
                ContentType = contentType
            });

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddFileStream(string name, Stream stream, string fileName, string contentType = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            _files.Add(new FileAttachment
            {
                Name = name,
                Stream = stream,
                FileName = fileName,
                ContentType = contentType
            });

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder SetFormat(DataFormat dataFormat)
        {
            _dataFormat = dataFormat;
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddHeader(string name, string value)
        {
            string headerValue = string.Empty;

            if (_headers.TryGetValue(name, out headerValue))
            {
                if (value != headerValue)
                {
                    _headers[name] = value;
                }

                return this;
            }

            _headers.Add(name, value);
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddCookie(string name, string value, string path, string domain)
        {
            var cookieValue = new CookieValue(name, value, path, domain);

            _cookieValues.Add(cookieValue);

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                string value = string.Empty;

                if (_headers.TryGetValue(header.Key, out value))
                {
                    if (value != header.Value)
                    {
                        _headers[header.Key] = header.Value;
                    }

                    continue;
                }

                _headers.Add(header.Key, header.Value);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder SetMethod(Method method)
        {
            _method = method;
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder SetTimeout(TimeSpan timeout)
        {
            if (timeout == null)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            _timeOut = timeout;

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddParameter(Parameter parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var existingIndex = _parameters.FindIndex(p =>
                string.Equals(p.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase));

            if (existingIndex >= 0)
            {
                _parameters[existingIndex] = parameter; // Replace existing
            }
            else
            {
                _parameters.Add(parameter);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddParameters(Parameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return this;

            // Build a lookup for existing parameters by name
            var existingLookup = new Dictionary<string, int>(
                StringComparer.InvariantCultureIgnoreCase);

            for (int i = 0; i < _parameters.Count; i++)
            {
                // Only store the first occurrence of each name
                if (!existingLookup.ContainsKey(_parameters[i].Name))
                {
                    existingLookup[_parameters[i].Name] = i;
                }
            }

            // Process new parameters
            foreach (var parameter in parameters)
            {
                if (parameter == null)
                    continue;

                if (existingLookup.TryGetValue(parameter.Name, out int index))
                {
                    // Replace existing
                    _parameters[index] = parameter;
                }
                else
                {
                    // Add new
                    _parameters.Add(parameter);
                    // Track this parameter to handle duplicates within the input array
                    existingLookup[parameter.Name] = _parameters.Count - 1;
                }
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddQueryParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var stringValue = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            var parameter = new QueryParameter(name, stringValue);
            return AddParameter(parameter);
        }

        /// <inheritdoc/>
        public IRequestBuilder AddQueryParameters(IDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return this;

            foreach (var kvp in parameters)
            {
                if (kvp.Value is null)
                    continue;

                var stringValue = Convert.ToString(kvp.Value, System.Globalization.CultureInfo.InvariantCulture);
                var parameter = new QueryParameter(kvp.Key, stringValue);
                AddParameter(parameter);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder AddUrlSegment(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var parameter = new UrlSegmentParameter(name, value);
            return AddParameter(parameter);
        }

        /// <inheritdoc/>
        public IRequestBuilder RemoveHeader(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (_headers.ContainsKey(name))
            {
                _headers.Remove(name);
            }

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder RemoveHeaders()
        {
            _headers.Clear();
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder RemoveCookies()
        {
            _cookieValues.Clear();
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder RemoveParameters()
        {
            _parameters.Clear();
            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder RemoveParameter(Parameter parameter)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var param = _parameters.Find(p => string.Equals(p.Name, parameter.Name, StringComparison.InvariantCultureIgnoreCase));

            if (param is null)
            {
                return this;
            }

            _parameters.Remove(param);

            return this;
        }

        /// <inheritdoc/>
        public IRequestBuilder WithBearerToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            return AddHeader("Authorization", $"Bearer {token}");
        }

        /// <inheritdoc/>
        public IRequestBuilder WithBasicAuth(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var credentials = $"{username}:{password}";
            var encodedCredentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials));

            return AddHeader("Authorization", $"Basic {encodedCredentials}");
        }

        /// <inheritdoc/>
        public IRequestBuilder WithApiKey(string key, string headerName = "X-API-Key")
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentNullException(nameof(headerName));
            }

            return AddHeader(headerName, key);
        }

        /// <inheritdoc/>
        public IRequestBuilder WithOAuth2(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return WithBearerToken(accessToken);
        }

        /// <inheritdoc/>
        public RestRequest Create()
        {
            var request = new RestRequest(_resource, _method);

            foreach (var param in _parameters)
            {
                request.AddParameter(param);
            }

            if (_body != null)
            {
                request.AddBody(_body);
            }

            request.RequestFormat = _dataFormat;

            foreach (var header in _headers)
            {
                request.AddHeader(header.Key, header.Value);
            }

            foreach (var cookie in _cookieValues)
            {
                request.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }

            // Add all files to the request
            foreach (var file in _files)
            {
                if (!string.IsNullOrEmpty(file.Path))
                {
                    request.AddFile(file.Name, file.Path, file.ContentType);
                }
                else if (file.Bytes != null && file.Bytes.Length > 0)
                {
                    request.AddFile(file.Name, file.Bytes, file.FileName, file.ContentType);
                }
                else if (file.Stream != null)
                {
                    request.AddFile(file.Name, () => file.Stream, file.FileName, file.ContentType);
                }
            }

            if (_timeOut != null)
            {
                request.Timeout = _timeOut;
            }

            return request;
        }

        #endregion Public Methods
    }
}
