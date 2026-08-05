using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Text;

namespace AgronicaCoreModelsSTD.exceptions
{
    [Serializable]
    public class GiasException : Exception
    {
        public GiasException() { }

        public GiasException(string message) : base(message) { }

        public GiasException(string message, System.Exception innerException) : base(message, innerException) { }

        [SecuritySafeCritical]
        public GiasException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
