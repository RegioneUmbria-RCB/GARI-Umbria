Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaUMA.Resources
Imports Newtonsoft.Json.Linq

Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreXML.XML_Stampe
Imports System.Web
Imports System.Globalization
Imports AgronicaCoreVarieDAL
Imports System.Web.Caching

Public Class ReportControllo
    Inherits System.Web.UI.Page

    Public QS_Avanzamento As Integer = 0
    Public QS_Piva As String = ""

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        inizializzoParametriPagina()


        Me.Master.flag_MostraBtnIndietro = False

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0

        Dim attivitaReportControllo = enum_Security_Attivita.UMA_Report_Controllo

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            attivitaReportControllo,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           attivitaReportControllo,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)


        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_2017.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        hdId_Agenda.Value = 0
        hdId_Mov.Value = 0
        hdId_Mov_Det.Value = 0
        hdId_Agenda_CDG.Value = 0
        hdModalita.Value = Entrata_Diretta
        hdId_CDG.Value = 0
        hdLav_Cod.Value = 0
        hdVeg_Cod.Value = 0
        hdDes_Lib.Value = ""
        hdMov_Desc.Value = ""
        hdSplit.Value = Split
        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))
        hdPivaSuperUser.Value = objParametri_Server.PivaSuperUser

        'Automatico (Impostazione da QDC)
        hdAutomatico.Value = 1 'Default

        If Not Page.IsPostBack Then


            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()
                hdPiva.Value = objParametriAgenda_2010.Piva
                hdId_Agenda.Value = Val(objParametriAgenda_2010.Id_Agenda)

            End If

            'If Entrata_Diretta = 0 Then

            'Introdotta per non rieseguirlo se appena creata con provenienza da APP
            Dim bombardinoDaEseguire = True

            If Val(hdId_Agenda.Value) = 0 Then

            End If

        End If

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaReportELAS(ByVal anno As Integer,
                                           ByVal bimestre As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Report_Controllo_R

        '----------------------------------------------------------------

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dt = objTestate.Leggi_ReportELAS(bimestre,
                                         anno,
                                         "",
                                         "",
                                         objParametri_Server,
                                         objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaElencoInadempienti(ByVal anno As Integer,
                                                   ByVal prov As String,
                                                   ByVal com As String,
                                                   ByVal statoPrat As String,
                                                   ByVal conto As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Report_Controllo_R

        '----------------------------------------------------------------

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dt = objTestate.Leggi_ElencoInadempienti(anno, prov, com, statoPrat, conto,
                                                 "",
                                                 "",
                                                 objParametri_Server,
                                                 objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaElencoTrasferimenti(ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Report_Controllo_R

        '----------------------------------------------------------------

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dt = objTestate.Leggi_ElencoTrasferimenti(anno,
                                                  "",
                                                  "",
                                                  objParametri_Server,
                                                  objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaElencoSegnalazioni(anno As Integer,
                                                   prov As String,
                                                   com As String,
                                                   tipoPrat As Integer,
                                                   conto As Integer,
                                                   giaSegnalate As Boolean,
                                                   segnalateDal As String,
                                                   rimanenze As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings With {
            .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        }

        Dim objTestate As New AgronicaCoreUmaDal.UMA_Report_Controllo_R

        '----------------------------------------------------------------

        'Dim dtRichiesteUmaTestat = objTestate.Leggi_Elenco_Con_Indirizzi(piva, anno, "", "", objParametri_Server, rendicontazioni:=rendicontazioni, statoCod:=statoCod, citta:=citta, prov:=prov, conto:=conto)
        Dt = objTestate.Leggi_SegnalazioniAccise(anno, prov, com, tipoPrat, conto, giaSegnalate, segnalateDal, rimanenze,
                                                  "",
                                                  "",
                                                  objParametri_Server,
                                                  objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SegnalaSelezionate(anno As Integer, data As String, righeSegnalate As String) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim rec_Acc_BIZ = New AgronicaCoreUmaBiz.UMA_Report_Controllo

            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim res = rec_Acc_BIZ.SegnalaSelezionati(righeSegnalate, anno, data, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim strRes = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

            r.RispostaStringa = strRes
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AnnullaSegnalazioneSelezionati(anno As Integer, righeSegnalate As String) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim rec_Acc_BIZ = New AgronicaCoreUmaBiz.UMA_Report_Controllo

            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim res = rec_Acc_BIZ.AnnullaSegnalazioneSelezionati(righeSegnalate, anno, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim strRes = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

            r.RispostaStringa = strRes
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_UMA_Setup(ByVal anno As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objUMA_Setup As New AgronicaCoreUmaDal.UMASetup_R
            Dim setDt = objUMA_Setup.LeggiSetup(anno, objParametri_Server)

            Dim cache As New Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = objParametri_Server.PivaSuperUser & "UMA_Leggi_UMA_Setup_" & anno
            Dim strResult = "{}"

            If cache IsNot Nothing AndAlso cache.Item(key) Is Nothing Then

                If setDt.Rows.Count > 0 Then
                    Dim serializerSettings As New JsonSerializerSettings()
                    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    strResult = JsonConvert.SerializeObject(setDt, Formatting.None, serializerSettings)
                End If

                cache.Item(key) = strResult
                r.RispostaStringa = strResult
                r.RispostaOK = True

            Else
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiTermineUltimoRendicontazione(anno As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim dal As New AgronicaCoreUmaDal.UMAConfigurazioneDateRendicontazioni_R
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objUMA_Setup As New AgronicaCoreUmaDal.UMASetup_R
            Dim tipo1 As New RispostaStandard
            Dim tipo2 As New RispostaStandard
            Dim oltreUltimaData As Boolean
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            Dim dt = dal.LeggiDateRendicontazioni(objParametri_Server, anno, "", " Termine_Ultimo_Rendicontazione DESC ")

            If dt.Rows.Count > 0 Then

                Dim data = CDate(CStr(dt.Rows.Item(0).Item("Termine_Ultimo_Rendicontazione")))

                If Date.Now < data Then
                    oltreUltimaData = False
                Else
                    oltreUltimaData = True
                End If

            Else
                oltreUltimaData = False
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(oltreUltimaData, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function LeggiPercorsoAllegati(ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim LeggiConfSiti As New Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")
        Return FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)
    End Function

    Private Sub inizializzoParametriPagina()


    End Sub


    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

End Class