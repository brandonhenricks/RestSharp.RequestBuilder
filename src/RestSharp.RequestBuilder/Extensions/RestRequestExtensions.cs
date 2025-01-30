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
    }
}
