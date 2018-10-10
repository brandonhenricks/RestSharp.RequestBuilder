using RestSharp.RequestBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestSharp.RequestBuilder
{
    /// <summary>
    /// RestRequest Builder Class
    /// </summary>
    public class RequestBuilder : IRequestBuilder
    {
        #region Private Properties

        private readonly string _resource;
        private readonly Dictionary<string, string> _headers;
        private readonly Dictionary<string, string> _cookies;
        private readonly List<Parameter> _parameters;
        private DataFormat _dataFormat;
        private Method _method;
        private object _body;
        private int _timeOut;

        #endregion Private Properties

        #region Public Properties

        public int HeaderCount => _headers.Count;

        #endregion Public Properties

        #region Public Constructor

        /// <summary>
        /// RequestBuilder Constructor
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
            _method = Method.GET;
            _dataFormat = DataFormat.Json;
            _cookies = new Dictionary<string, string>();
            _timeOut = 0;
        }

        #endregion Public Constructor

        #region Public Methods

        /// <summary>
        /// Add a serialized object to the IRestRequest.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public IRequestBuilder AddBody(object body)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            _body = body;
            return this;
        }

        /// <summary>
        /// Set the DataFormat of the IRestRequest.
        /// </summary>
        /// <param name="dataFormat"></param>
        /// <returns></returns>
        public IRequestBuilder SetFormat(DataFormat dataFormat)
        {
            _dataFormat = dataFormat;
            return this;
        }

        /// <summary>
        /// Add a Header to the IRestRequest.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add Cookie to Request.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IRequestBuilder AddCookie(string name, string value)
        {
            string cookieValue = string.Empty;

            if (_cookies.TryGetValue(name, out cookieValue))
            {
                if (value != cookieValue)
                {
                    _cookies[name] = value;
                }

                return this;
            }

            _cookies.Add(name, value);
            return this;
        }

        /// <summary>
        /// Add Headers to the IRestRequest.
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IRequestBuilder AddHeaders(Dictionary<string, string> headers)
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

        /// <summary>
        /// Set the Method of the IRestRequest.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public IRequestBuilder SetMethod(Method method)
        {
            _method = method;
            return this;
        }

        /// <summary>
        /// Set the IRestRequest Timeout value.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IRequestBuilder SetTimeout(int timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            _timeOut = timeout;
            return this;
        }

        /// <summary>
        /// Add a Parameter to the IRestRequest.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add Parameters to the <see cref="IRestRequest"/> IRestRequest.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IRequestBuilder AddParameters(Parameter[] parameters)
        {
            // TODO: Revisit, this doesn't seem like the best approach.

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
                var param = Array.Find(parameters, x => x.Name == dup.Name);

                if (param is null) continue;

                _parameters.Remove(dup);
                _parameters.Add(param);
            }

            return this;
        }

        /// <summary>
        /// Creates the IRestRequest object.
        /// </summary>
        /// <returns>IRestRequest</returns>
        public IRestRequest Create()
        {
            var request = new RestRequest(_resource, _method, _dataFormat);

            foreach (var param in _parameters)
            {
                request.AddParameter(param);
            }

            if (_body != null)
            {
                request.AddBody(_body);
            }

            foreach (var header in _headers)
            {
                request.AddHeader(header.Key, header.Value);
            }

            foreach (var cookie in _cookies)
            {
                request.AddCookie(cookie.Key, cookie.Value);
            }

            request.Timeout = _timeOut;

            return request;
        }

        #endregion Public Methods
    }
}