
Imports System.Web
Imports AgronicaCoreDataProvider.TipiEnumerativi





Public Class profitosan
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function getLink2023(ByVal nuovaPagina As Boolean,
                                       ByVal objwebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                       ByVal fr_cod As String,
                                       ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim link As String
        Dim objvaiaProfitosan As New AgronicaCoreVarieDAL.vaiaProfitosan
        link = objwebconfig.LinkProfitosan_2023 & "tunnel_2?"
        link = link & AgronicaCoreVarieDAL.vaiaProfitosan.getUrlProdotto(objParametri_Server, objParametri_Utenti, fr_cod, nuovaPagina, True)

        Return link
    End Function

    Public Shared Function getLinkSimple(ByVal nuovaPagina As Boolean, ByVal request As System.Web.HttpRequest, ByVal objwebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                   ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim link As String
        If request.Browser.Browser = "IE" Then
            'LINK AL VECCHIO PROFITOSAN
            link = objwebconfig.LinkProfitosan
            link = link & IIf(link.EndsWith("/"), "", "/") & "x_Tunnel/Tunnel_GiasOnLine.aspx?p=" & enum_PagineProFitoSan.FiltroRicercaSchedaPFS
        Else
            'Profitosan new
            Dim objvaiaProfitosan As New AgronicaCoreVarieDAL.vaiaProfitosan
            link = objwebconfig.LinkProfitosan_WS & "tunnel_2.html?"
            link = link & AgronicaCoreVarieDAL.vaiaProfitosan.getUrlSimple(objParametri_Server, objParametri_Utenti, nuovaPagina, False)
        End If

        Return link
    End Function


    <Obsolete("Dal 2023 si usa la funzione getLink2023()" &
              "Unico riferimento nel GiasOnline_2010 (probabilmente comunque inutilizzato)")>
    Public Shared Function getLinkFRCOD(ByVal Fr_COD As String, ByVal nuovo As Boolean,
                                        ByVal request As System.Web.HttpRequest, ByVal objwebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                  ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim link As String
        If request.Browser.Browser = "IE" Then
            'LINK AL VECCHIO PROFITOSAN
            link = objwebconfig.LinkProfitosan
            link = link & IIf(link.EndsWith("/"), "", "/") & "x_Tunnel/Tunnel_GiasOnLine.aspx?p=1&f=" & Fr_COD

        Else
            'Profitosan new
            Dim objvaiaProfitosan As New AgronicaCoreVarieDAL.vaiaProfitosan
            link = objwebconfig.LinkProfitosan_WS & "tunnel_2.html?"
            link = link & AgronicaCoreVarieDAL.vaiaProfitosan.getUrlProdotto(objParametri_Server, objParametri_Utenti, Fr_COD, nuovo, False)
        End If

        Return link
    End Function

    <Obsolete("Dal 2023 si usa la funzione getLink2023()")>
    Public Shared Function getLinkFRCOD_x_trattamento(ByVal nuovo As Boolean, ByVal request As System.Web.HttpRequest, ByVal objwebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig,
                                 ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim link As String
        If request.Browser.Browser = "IE" Then
            'LINK AL VECCHIO PROFITOSAN
            link = objwebconfig.LinkProfitosan
            link = link & IIf(link.EndsWith("/"), "", "/") & "x_Tunnel/Tunnel_GiasOnLine.aspx?p=1&f="
        Else
            'Profitosan new
            Dim objvaiaProfitosan As New AgronicaCoreVarieDAL.vaiaProfitosan
            link = objwebconfig.LinkProfitosan_WS & "tunnel_2.html?"
            link = link & AgronicaCoreVarieDAL.vaiaProfitosan.getUrlProdotto(objParametri_Server, objParametri_Utenti, "", nuovo, False)
        End If

        Return link
    End Function

End Class
