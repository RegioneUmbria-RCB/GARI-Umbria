using System;

namespace AgronicaCoreModelsSTD.exceptions
{
    public class CoreWSNotAuthenticatedException : Exception
    {
        public CoreWSNotAuthenticatedException() : base()
        { }
        public CoreWSNotAuthenticatedException(string message): base(message)
        { }
    }
}
