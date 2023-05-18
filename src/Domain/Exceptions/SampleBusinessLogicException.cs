using System;

namespace Domain.Exceptions
{
    public class SampleNotFoundException : SampleException
    {
        public SampleNotFoundException(string message) : base(message)
        {
        }

        public SampleNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public SampleNotFoundException() : base()
        {
        }
    }
}
