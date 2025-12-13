using System.Collections.Generic;

namespace RestSharp.RequestBuilder.Models
{
    internal sealed class CookieValueComparer : IEqualityComparer<CookieValue>
    {
        public bool Equals(CookieValue x, CookieValue y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(CookieValue obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
