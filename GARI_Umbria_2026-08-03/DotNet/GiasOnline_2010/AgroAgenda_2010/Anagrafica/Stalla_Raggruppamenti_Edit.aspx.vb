Imports System.Transactions
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgroAgenda_2010.Resources

Public Class Stalla_Raggruppamenti_Edit
    Inherits System.Web.UI.Page

    Dim Qs_Visibilita As Integer = 0

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_MostraBtnIndietro = True

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function get_permesso(permesso_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim permessi = New PermessiUtente()
            Dim permesso = permessi.getPermesso(permesso_cod)
            Dim objPerm As New JObject()
            objPerm.Add(New JProperty("Scrittura", permesso.Scrittura))
            objPerm.Add(New JProperty("Lettura", permesso.Lettura))
            Dim permessiStr = objPerm.ToString
            r.RispostaStringa = permessiStr
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function get_stalla() As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim piva = objParametri_Agenda.Piva
            Dim sa_cod = objParametri_Agenda.Sa_Cod
            Dim STA_NUM = CInt(objParametri_Agenda.Fabbricato)

            Dim obj_stalla = From s In GiasContext.Stalla
                             Join c In GiasContext.Centri_Aziendali On s.PIVA Equals c.PIVA And s.sa_cod Equals c.sa_cod
                             Where s.STA_NUM = STA_NUM And
                                 s.PIVA = piva And
                                 s.sa_cod = sa_cod
                             Select s, c.sa_nome


            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(obj_stalla.FirstOrDefault)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally
            GiasContext.Dispose()
        End Try

        scope.Complete()
        scope.Dispose()

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function get_raggruppamento_stalla() As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim piva = objParametri_Agenda.Piva
            Dim sa_cod = objParametri_Agenda.Sa_Cod
            Dim STA_NUM = CInt(objParametri_Agenda.Fabbricato)
            Dim Raggrupamento_Cod = CInt(objParametri_Agenda.Raggruppamento_Cod)

            Dim obj_stalla_raggru = From rs In GiasContext.Stalla_Raggruppamenti Where rs.PIVA = piva And
                                                                                    rs.sa_cod = sa_cod And
                                                                                    rs.STA_NUM = STA_NUM And
                                                                                    rs.Raggruppamento_Cod = Raggrupamento_Cod
                                    Select rs

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(obj_stalla_raggru.FirstOrDefault)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally
            GiasContext.Dispose()
        End Try

        scope.Complete()
        scope.Dispose()

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function get_lista_tipi_raggruppamenti() As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim piva = objParametri_Agenda.Piva
            Dim sa_cod = objParametri_Agenda.Sa_Cod
            Dim STA_NUM = CInt(objParametri_Agenda.Fabbricato)

            Dim obj_stalla_raggru = From rs In GiasContext.Lista_Tipi_Raggruppamento_Stalla
                                    Select rs.Raggruppamento_Cod, rs.Raggruppamento_Des  ', tr.Raggruppamento_Des

            Dim list = obj_stalla_raggru.FirstOrDefault
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(obj_stalla_raggru.ToList)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally
            GiasContext.Dispose()
        End Try

        scope.Complete()
        scope.Dispose()

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function salva(obj_str_raggruppamento_Stalla As String, RetOp As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim timezone As New JsonSerializerSettings With {
                .DateTimeZoneHandling = DateTimeZoneHandling.Local
            }

            Dim obj_raggr = JsonConvert.DeserializeObject(Of JObject)(obj_str_raggruppamento_Stalla, timezone)
            Dim objRaggr_stallaBIZ As New AgronicaCoreAnagrafeBIZ.Stalla_Raggruppamenti

            objRaggr_stallaBIZ.ScriviModifica(obj_raggr, objParametri_Server)


            r.RispostaStringa = AgronicaAgenda_2010.SalvataggioEffettuatoCorrettamente
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally
            GiasContext.Dispose()
        End Try

        scope.Complete()
        scope.Dispose()

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function impostaTipoOperazioneObj_Agenda(tipoOperazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard


        Try
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            objParametri_Agenda.Tipo_Operazione = tipoOperazione
            If tipoOperazione = 1 Then
                objParametri_Agenda.Raggruppamento_Cod = 0
            End If

            r.RispostaStringa = AgronicaAgenda_2010.SalvataggioEffettuatoCorrettamente
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Zoo_Animali_Lista_Stati_Accrescimento(GEN_COD As Integer, SPE_COD As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim gefutils As New Gias_EF_Utility
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim lista_tipi = From zalt In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento
                             Where zalt.GEN_COD = GEN_COD And zalt.SPE_COD = SPE_COD
                             Select zalt.STATO_COD, zalt.Stato_Des, zalt.Ordine
                             Order By Ordine


            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(lista_tipi.ToList)
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

            scope.Complete()
            scope.Dispose()
            GiasContext.Dispose()

        End Try

        Return r

    End Function

End Class