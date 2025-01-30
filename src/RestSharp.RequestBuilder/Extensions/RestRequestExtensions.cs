using System;
using RestSharp.RequestBuilder.Interfaces;

namespace RestSharp.RequestBuilder.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="RestRequest"/> to use <see cref="IRequestBuilder"/>.
    /// </summary>
    public static class RestRequestExtensions
    {
        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> for the specified resource.
        /// </summary>
        /// <param name="request">The <see cref="RestRequest"/> instance.</param>
        /// <param name="resource">The resource as a string.</param>
        /// <returns>A new instance of <see cref="IRequestBuilder"/>.</returns>
        public static IRequestBuilder WithBuilder(this RestRequest request, string resource)
        {
            return new RequestBuilder(resource);
        }

        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> for the specified resource.
        /// </summary>
        /// <param name="request">The <see cref="RestRequest"/> instance.</param>
        /// <param name="resource">The resource as a <see cref="Uri"/>.</param>
        /// <returns>A new instance of <see cref="IRequestBuilder"/>.</returns>
        public static IRequestBuilder WithBuilder(this RestRequest request, Uri resource)
        {
            return new RequestBuilder(resource);
        }

        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> for the specified resource and HTTP method.
        /// </summary>
        /// <param name="request">The <see cref="RestRequest"/> instance.</param>
        /// <param name="resource">The resource as a string.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <returns>A new instance of <see cref="IRequestBuilder"/>.</returns>
        public static IRequestBuilder WithBuilder(this RestRequest request, string resource, Method method)
        {
            return new RequestBuilder(resource, method);
        }

        /// <summary>
        /// Creates a new <see cref="IRequestBuilder"/> for the specified resource and HTTP method.
        /// </summary>
        /// <param name="request">The <see cref="RestRequest"/> instance.</param>
        /// <param name="resource">The resource as a <see cref="Uri"/>.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <returns>A new instance of <see cref="IRequestBuilder"/>.</returns>
        public static IRequestBuilder WithBuilder(this RestRequest request, Uri resource, Method method)
        {
            return new RequestBuilder(resource, method);
        }
    }
}
