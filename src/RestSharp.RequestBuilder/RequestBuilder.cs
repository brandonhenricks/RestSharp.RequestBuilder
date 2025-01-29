using RestSharp.RequestBuilder.Interfaces;
using RestSharp.RequestBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestSharp.RequestBuilder
{
    /// <summary>
    /// RestRequestBuilder is a helper class library that utilizes Fluent Syntax in order to create
    /// RestRequest objects.
    /// </summary>
    public sealed class RequestBuilder : IRequestBuilder
    {
        #region Private Properties

        private readonly string _resource;
        private readonly Dictionary<string, string> _headers;
        private readonly HashSet<CookieValue> _cookieValues;
        private readonly List<Parameter> _parameters;
        private DataFormat _dataFormat;
        private Method _method;
        private object _body;
        private TimeSpan? _timeOut;

        private string _fileName;
        private string _filePath;
        private string _fileType;

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
            _method = Method.Get;
            _dataFormat = DataFormat.Json;
            _cookieValues =  new HashSet<CookieValue>(new CookieValueComparer());
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
            _method = Method.Get;
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
        public IRequestBuilder AddBody<T>(T body, DataFormat dataFormat)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;

            _dataFormat = dataFormat;

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

            _fileName = name;
            _filePath = path;
            _fileType = contentType;

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

            if (!_parameters.Contains(parameter))
            {
                _parameters.Add(parameter);
            }

            return this;
        }


        /// <inheritdoc/>
        public IRequestBuilder AddParameters(Parameter[] parameters)
        {
            var duplicates = _parameters.Select(x => x).Intersect(parameters);

            // Check for duplicates.
            if (!duplicates.Any())
            {
                _parameters.AddRange(parameters);
                return this;
            }

            // Iterate over duplicate items.
            foreach (var dup in duplicates)
            {
                var param = Array.Find(parameters, x => string.Equals(x.Name, dup.Name, StringComparison.InvariantCultureIgnoreCase));

                if (param is null) continue;

                _parameters.Remove(dup);
                _parameters.Add(param);
            }

            return this;
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
            
            var param = _parameters.Find(p => string.Equals(p.Name,  parameter.Name, StringComparison.InvariantCultureIgnoreCase));

            if (param is null)
            {
                return this;
            }

            _parameters.Remove(param);

            return this;
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

            if (!string.IsNullOrEmpty(_fileName) && !string.IsNullOrEmpty(_filePath))
            {
                request.AddFile(_fileName, _filePath, _fileType);
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