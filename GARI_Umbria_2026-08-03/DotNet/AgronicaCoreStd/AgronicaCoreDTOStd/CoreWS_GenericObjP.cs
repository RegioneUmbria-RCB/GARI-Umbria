public class CoreWS_GenericObjP
{
    public string objP_super_server { get; set; }
    public string objP_server { get; set; }
    public string objP_utenti { get; set; }
    public string user_Agent { get; set; }
    public string host { get; set; }

    public CoreWS_GenericObjP()
    {

    }

    public CoreWS_GenericObjP(string objP_super_server, string objP_server, string objP_utenti)
    {
        this.objP_super_server = objP_super_server;
        this.objP_server = objP_server;
        this.objP_utenti = objP_utenti;
    }

}