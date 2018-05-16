using System;

namespace RedBear.Auth.ServiceClient.Exceptions
{
    /// <summary>
    /// Base exception for ServiceClient activities.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ServiceClientException : Exception
    {
        public ServiceClientException(string message) : base(message) { }
    }
}
