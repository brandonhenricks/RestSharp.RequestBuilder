using System;

namespace RestSharp.RequestBuilder.Models
{
    internal sealed class CookieValue : IEquatable<CookieValue>
    {
        public string Name { get; }

        public string Value { get; }

        public string Path { get; }

        public string Domain { get; }

        public CookieValue(string name, string value, string path, string domain)
        {
            Name = name;
            Value = value;
            Path = path;
            Domain = domain;
        }

        public bool Equals(CookieValue other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!string.Equals(Domain, other.Domain, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is CookieValue cookieValue)
            {
                return Equals(cookieValue);
            }

            return false;
        }

        public static bool operator ==(CookieValue first, CookieValue second)
        {
            if ((object)first == null)
            {
                return (object)second == null;
            }

            return first.Equals(second);
        }

        public static bool operator !=(CookieValue first, CookieValue second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            if (ReferenceEquals(this, null))
            {
                return 0;
            }

            unchecked
            {
                int hashCode = 47;
                hashCode = (hashCode * 53) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 53) ^ (Value?.GetHashCode() ?? 0);
                hashCode = (hashCode * 53) ^ (Path?.GetHashCode() ?? 0);
                hashCode = (hashCode * 53) ^ (Domain?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}