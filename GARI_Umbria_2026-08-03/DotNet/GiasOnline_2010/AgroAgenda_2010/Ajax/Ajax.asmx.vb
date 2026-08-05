Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
'<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
<Script.Services.ScriptService()> _
Public Class Ajax
    Inherits System.Web.Services.WebService



#Region "DPI"
    <WebMethod(EnableSession:=True)> _
    Public Function ok_DPI1() As String

        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

        Session("WS_DPI") = connessione
        Session("Collegamento_DPI") = True
        Return "true"
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function ok_DPI2() As String

        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2

        Session("Collegamento_DPI") = True
        Return "true"
    End Function

#End Region

#Region "FITO"

    <WebMethod(EnableSession:=True)> _
    Public Function ok_FITO1() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

        Session("WS_FITO") = connessione

        Session("Collegamento_FITO") = True
        Return "true"
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function ok_FITO2() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2

        Session("WS_FITO") = connessione

        Session("Collegamento_FITO") = True
        Return "true"
    End Function

#End Region


#Region "METEO"
    <WebMethod(EnableSession:=True)> _
    Public Function ok_METEO1() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Meteo_Meteo

        Session("WS_METEO") = connessione

        Session("Collegamento_METEO") = True
        Return "true"
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function ok_METEO2() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_Meteo_Meteo_2

        Session("WS_METEO") = connessione

        Session("Collegamento_METEO") = True
        Return "true"
    End Function
#End Region

#Region "cap cli"
    <WebMethod(EnableSession:=True)> _
    Public Function ok_CAP1() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente

        Session("WS_CAPITOLATO") = connessione

        Session("Collegamento_CAP") = True
        Return "true"
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function ok_CAP2() As String
        Dim objagrowebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim connessione As String = objagrowebconfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2

        Session("WS_CAPITOLATO") = connessione

        Session("Collegamento_CAP") = True
        Return "true"
    End Function
#End Region

End Class

 