using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// This exception is thrown when the user is not authorized to perform the requested action.
    /// </summary>
    public class SampleForbiddenException: SampleException
    {
        public SampleForbiddenException(string message) : base(message)
        {
        }

        public SampleForbiddenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SampleForbiddenException()
        {
        }
    }
}
