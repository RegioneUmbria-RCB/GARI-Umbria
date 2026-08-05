namespace AgronicaNetCore.Base.Models
{
    public class AgronicaCoreParametriTriple : AgronicaCoreParametriDouble
    {
        public AgronicaCoreParametriTriple(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriSuperServer objParametriSuperServer,int idDbServer = 0) : base(objParametriServer, objParametriUtenti)
        {
            ObjParametriSuperServer = objParametriSuperServer;
            IdDbServer = idDbServer;
        }

        public AgronicaCoreParametriSuperServer ObjParametriSuperServer { get; set; }
        public int IdDbServer { get; set; }
    }
}
