namespace AgronicaNetCore.Base.Models
{
    public class AgronicaCoreParametriDouble
    {
        public AgronicaCoreParametriDouble(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            ObjParametriServer = objParametriServer;
            ObjParametriUtenti = objParametriUtenti;
        }

        public AgronicaCoreParametriServer ObjParametriServer { get; set; }
        public AgronicaCoreParametriUtenti ObjParametriUtenti { get; set; }
    }
}
