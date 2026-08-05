Imports System.Reflection
Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieDAL

Public Class GiasBaseHelper
    Public Shared Sub Formatta_Link_GiasBase(ByRef link As String)

        If IsNothing(link) OrElse String.IsNullOrEmpty(link) OrElse String.IsNullOrWhiteSpace(link) Then
            link = AggiungiSlashSeNonEsiste(GIAS_BASE_DEFAULT_LINK)
        Else
            link = AggiungiSlashSeNonEsiste(link)
        End If
        If Debugger.IsAttached Then
            If Not link.ToLowerInvariant.Contains("http") Then
                link = "http://localhost" & AggiungiSlashSeNonEsiste(link)
            End If
        Else
            link = AggiungiSlashSeNonEsiste(link)
        End If

    End Sub

    Public Shared Sub Setta_Link_GiasBase(ByVal chiave_ObjParametri As String, ByRef linkAttuale As String)

        If IsNothing(HttpContext.Current) OrElse IsNothing(HttpContext.Current.Session) Then

            ' NON ESISTE IL CONTEXT O LA SESSION

            If IsNothing(linkAttuale) OrElse String.IsNullOrEmpty(linkAttuale) OrElse String.IsNullOrWhiteSpace(linkAttuale) Then
                linkAttuale = AggiungiSlashSeNonEsiste(GIAS_BASE_DEFAULT_LINK)
            End If
            If Debugger.IsAttached Then
                If Not linkAttuale.ToLowerInvariant.Contains("http") Then
                    linkAttuale = "http://localhost" & AggiungiSlashSeNonEsiste(linkAttuale)
                End If
            Else
                linkAttuale = AggiungiSlashSeNonEsiste(linkAttuale)
            End If
        Else

            Dim obj_parametri As AgronicaCoreParametri = HttpContext.Current.Session(chiave_ObjParametri)
            Dim helper As New AgronicaCoreParametri_Helper

            If Not IsNothing(obj_parametri) Then
                If String.IsNullOrEmpty(obj_parametri.LinkGiasBase) OrElse String.IsNullOrWhiteSpace(obj_parametri.LinkGiasBase) Then

                    Dim leggiAncheSuperServer As Boolean = Not chiave_ObjParametri.Equals("ASG_objParametri_Super_Server")
                    obj_parametri.LinkGiasBase = AggiungiSlashSeNonEsiste(Leggi_Link_Gias_Base(obj_parametri, leggiAncheSuperServer))

                End If
                If Debugger.IsAttached Then
                    If Not obj_parametri.LinkGiasBase.ToLowerInvariant.Contains("http") Then
                        obj_parametri.LinkGiasBase = "http://localhost" & AggiungiSlashSeNonEsiste(obj_parametri.LinkGiasBase)
                    End If
                Else
                    obj_parametri.LinkGiasBase = AggiungiSlashSeNonEsiste(obj_parametri.LinkGiasBase)
                End If
                linkAttuale = obj_parametri.LinkGiasBase
                HttpContext.Current.Session(chiave_ObjParametri) = obj_parametri
            Else
                If IsNothing(linkAttuale) OrElse String.IsNullOrEmpty(linkAttuale) OrElse String.IsNullOrWhiteSpace(linkAttuale) Then
                    linkAttuale = AggiungiSlashSeNonEsiste(GIAS_BASE_DEFAULT_LINK)
                End If
                If Debugger.IsAttached Then
                    If Not linkAttuale.ToLowerInvariant.Contains("http") Then
                        linkAttuale = "http://localhost" & AggiungiSlashSeNonEsiste(linkAttuale)
                    End If
                Else
                    linkAttuale = AggiungiSlashSeNonEsiste(linkAttuale)
                End If
            End If

        End If

    End Sub

    Public Shared Sub Setta_Link_GiasBase(ByVal objParametri As AgronicaCoreParametri, ByRef linkAttuale As String)

        If Not IsNothing(objParametri) Then
            If String.IsNullOrEmpty(objParametri.LinkGiasBase) OrElse String.IsNullOrWhiteSpace(objParametri.LinkGiasBase) Then
                objParametri.LinkGiasBase = AggiungiSlashSeNonEsiste(Leggi_Link_Gias_Base(objParametri))
            End If
            If Debugger.IsAttached Then
                If Not objParametri.LinkGiasBase.ToLowerInvariant.Contains("http") Then
                    objParametri.LinkGiasBase = "http://localhost" & AggiungiSlashSeNonEsiste(objParametri.LinkGiasBase)
                End If
            Else
                objParametri.LinkGiasBase = AggiungiSlashSeNonEsiste(objParametri.LinkGiasBase)
            End If
            linkAttuale = objParametri.LinkGiasBase
        Else
            If IsNothing(linkAttuale) OrElse String.IsNullOrEmpty(linkAttuale) OrElse String.IsNullOrWhiteSpace(linkAttuale) Then
                linkAttuale = AggiungiSlashSeNonEsiste(GIAS_BASE_DEFAULT_LINK)
            End If
            If Debugger.IsAttached Then
                If Not linkAttuale.ToLowerInvariant.Contains("http") Then
                    linkAttuale = "http://localhost" & AggiungiSlashSeNonEsiste(linkAttuale)
                End If
            Else
                linkAttuale = AggiungiSlashSeNonEsiste(linkAttuale)
            End If
        End If

    End Sub


    Public Shared Sub WarmUp_GestioneRichieste()

        Dim objParametri_Super_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim linkGiasBase As String = String.Empty

        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            Setta_Link_GiasBase("ASG_objParametri_Super_Server", linkGiasBase)
        End If

        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            Setta_Link_GiasBase("ASG_objParametri_Server", linkGiasBase)
            objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        End If
        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Utenti")) Then
            If Not IsNothing(objParametri_Server) Then
                objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
                objParametri_Utenti.LinkGiasBase = objParametri_Server.LinkGiasBase
                HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti
            End If
        End If


    End Sub

    Private Shared Function Leggi_Link_Gias_Base(ByVal objParamtri As AgronicaCoreParametri, Optional ByVal leggiAncheSuperServer As Boolean = False) As String

        ' 1) Tento lettura da configurazione siti
        Dim retVal As String = Leggi_Link_Gias_Base_Da_Configurazione_Siti(objParamtri)
        If Not String.IsNullOrEmpty(retVal) Then
            Return retVal
        End If

        ' 2) Tento di fare fallback su AgroWbConfig che contiene link giasbase del superserver
        retVal = Leggi_Link_Gias_Base_Da_AgroWebConfig()
        If Not String.IsNullOrEmpty(retVal) Then
            Return retVal
        End If

        '3) Se previsto tenta di recuperarlo dal superserver
        If leggiAncheSuperServer Then
            If Not IsNothing(HttpContext.Current) AndAlso Not IsNothing(HttpContext.Current.Session) Then
                If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
                    Dim objPSS As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
                    retVal = Leggi_Link_Gias_Base_Da_Configurazione_Siti(objPSS)
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(retVal) Then
            Return retVal
        End If

        Return GIAS_BASE_DEFAULT_LINK

    End Function

    Private Shared Function Leggi_Link_Gias_Base_Da_Configurazione_Siti(ByVal objParametri As AgronicaCoreParametri) As String

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' SFRUTTA LA CACHE DI CONF_SITI
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim retVal As String = String.Empty

        If String.IsNullOrEmpty(objParametri.StringaConnessione) Then
            Return retVal
        End If

        Try

            If Not Esiste_Tabella_Configurazione_Siti(objParametri) Then
                Return retVal
            End If

            Dim confR As New Configurazione_Siti_R
            Dim strSql As String = "select valore from configurazione_siti where chiave = 'LinkGiasBase' "

            Dim dt As DataTable = confR.Leggi(objParametri, strSql)
            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                retVal = If(dt.Rows(0).Item("Valore") Is DBNull.Value, "", dt.Rows(0).Item("Valore").ToString)
            End If

            Return retVal

        Catch ex As Exception
            retVal = ""
        End Try

        Return retVal

    End Function

    Private Shared Function Esiste_Tabella_Configurazione_Siti(ByVal objParametri As AgronicaCoreParametri) As Boolean

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' SFRUTTA LA CACHE DI CONF_SITI
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Dim retVal As Boolean = False

        If String.IsNullOrEmpty(objParametri.StringaConnessione) Then
            Return retVal
        End If

        Try
            Dim confR As New Configurazione_Siti_R
            Dim strSql As String = "select count(1) as Conteggio from sys.tables tt where tt.name = 'Configurazione_Siti'"
            Dim dt As DataTable = confR.Leggi(objParametri, strSql)
            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                retVal = If(dt.Rows(0).Item("Conteggio") Is DBNull.Value, False, CInt(dt.Rows(0).Item("Conteggio")) > 0)
            End If
        Catch ex As Exception

        End Try

        Return retVal


    End Function

    Private Shared Function Leggi_Link_Gias_Base_Da_AgroWebConfig()

        Dim retVal As String = ""

        Try
            If Not IsNothing(HttpContext.Current) AndAlso Not IsNothing(HttpContext.Current.Session) Then
                Dim agroWebConfig As Object = HttpContext.Current.Session?("AgroWebConfig")
                If Not IsNothing(agroWebConfig) Then
                    Dim pi As PropertyInfo = agroWebConfig.GetType().GetProperty("LinkGiasBase")
                    If Not IsNothing(pi) Then
                        Dim valore = pi.GetValue(agroWebConfig)
                        If Not IsNothing(valore) Then
                            If Not String.IsNullOrEmpty(valore.ToString) AndAlso Not String.IsNullOrWhiteSpace(valore.ToString) Then
                                retVal = valore.ToString
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try

        Return retVal

    End Function

    Private Shared Function AggiungiSlashSeNonEsiste(ByVal Percorso As String) As String

        If String.IsNullOrEmpty(Percorso) Then
            Return String.Empty
        End If

        If Not Percorso.ToLowerInvariant.Contains("http") Then
            If Not Percorso.StartsWith("/") Then
                Percorso = "/" & Percorso
            End If
        End If

        If Not Percorso.EndsWith("/") Then
            Return Percorso & "/"
        Else
            Return Percorso
        End If
    End Function

End Class
