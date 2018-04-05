using System;
using System.Runtime.Serialization;

namespace Nap.Logger.WebApi.ObjectsFromVri
{
    [Serializable]
    public sealed class VriException : Exception
    {
        public VriException() : base()
        {

        }

        public VriException(string message) : base(message)
        {

        }

        public VriException(string message, Exception innerException) : base(message, innerException)
        {

        }

        private VriException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}