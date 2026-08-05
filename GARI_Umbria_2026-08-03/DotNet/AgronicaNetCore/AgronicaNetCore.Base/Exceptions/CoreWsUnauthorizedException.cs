
namespace AgronicaNetCore.Base.Exceptions
{
    public class CoreWsUnauthorizedException : Exception
    {
        public CoreWsUnauthorizedException() : base()
        { }
        public CoreWsUnauthorizedException(string message) : base(message)
        { }
    }
}
