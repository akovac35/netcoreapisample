using System;

namespace Domain.Exceptions
{
    public class SampleUnauthorizedException: SampleException
    {
        public SampleUnauthorizedException(string message) : base(message)
        {
        }

        public SampleUnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SampleUnauthorizedException()
        {
        }
    }
}
