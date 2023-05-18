using System;

namespace Domain.Exceptions
{
    public class SampleException : Exception
    {
        public SampleException(string message) : base(message)
        {
        }

        public SampleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SampleException()
        {
        }
    }
}
