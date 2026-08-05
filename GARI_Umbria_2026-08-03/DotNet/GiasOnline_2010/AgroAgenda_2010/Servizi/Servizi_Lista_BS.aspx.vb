Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUmaDal
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports <xmlns="http://G2G">
Imports AgronicaCoreUtility
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreDemetraBIZ
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreProfilazioneDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreMetaSchemaDAL

Public Class Servizi_Lista_BS
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda_2010

    Private Const _UpperBoundTabelle As Integer = 2000000000

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        objParametriAgenda = New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objParametriAgenda.Leggi()

        Master.Lbl_Titolo.Text = Resources.AgronicaAgenda_2010.GestionePraticheServizi
        Master.LblRag_Soc.Text = ""
        HD_piva.Value = ""


        If objParametriAgenda.Piva <> "" Then
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim DTImpresa = objImprese.Leggi(objParametriAgenda.Piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If DTImpresa IsNot Nothing AndAlso DTImpresa.Rows.Count > 0 Then
                Master.LblRag_Soc.Text = CStr(DTImpresa.Rows(0)("Rag_Soc"))
            End If
            HD_piva.Value = objParametriAgenda.Piva
        End If

        HD_Username.Value = objParametri_Server.UtenteUsername

        If (Not IsPostBack) Then

            Dim UtenteAbilitato_R As Boolean
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_R = objPermessi.Controlla_Permessi_Utente(
                                       Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gestione_Servizi,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti) OrElse objPermessi.Controlla_Permessi_Utente(
                                        Session("ASG_Utente_Username"),
                                        Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Gestione_Servizi_NEW,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)



            If Not UtenteAbilitato_R Then
                Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            End If

        Else
            Exit Sub
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Pratiche(ByVal FiltroImprese_str As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt_str As String = ""

            Dim RagSoc As String = ""
            Dim Piva As String = ""
            Dim Cuaa As String = ""
            Dim Servizio_Cod As Integer = 0
            Dim Stato_Cod As Integer = 0
            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE
            Dim FiltroStato As String = ""
            Dim Filtro As String = ""
            Dim FiltroImprese As String = ""


            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------


            Filtro = GetFiltro(FiltroImprese_str, objParametri_Server, objParametri_Utenti)

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim DTProfilo As DataTable
            Dim Sql_Permessi As String = ""


            DTProfilo = objProfilo.Leggi(
                HttpContext.Current.Session("ASG_Utente_Username"),
                CInt(HttpContext.Current.Session("ASG_IdServizio")),
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Utenti
            )



            If DTProfilo.Rows.Count > 0 Then
                Sql_Permessi = DTProfilo.Rows(0).Item("Descrizione_2") & ""
            End If
            '----------------------------------------------------------------

            Dim Filtro_Visibilita_Utente = True
            If Sql_Permessi = "" Then
                Filtro_Visibilita_Utente = False
            End If

            Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim Dt_Pratiche As DataTable
            Dt_Pratiche = ObjPratiche.Leggi_conStoricoTransizioniDiStato_FiltroUtente(0,
                               RagSoc, Piva, Cuaa,
                               0, 0, 0,
                               Servizio_Cod,
                               Stato_Cod,
                               DataInizio, DataFine,
                               Filtro,
                               " Rag_Soc, Servizio_Des, Validita_Inizio_Stato, Ordine ",
                               Filtro_Visibilita_Utente,
                               objParametri_Server,
                               objParametri_Utenti)


            'aggiungo le colonne necessarie al gridview
            Dim Dt As New DataTable
            Dim Dr As DataRow
            Dt.Columns.Add(New DataColumn("Pratica_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
            Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cuaa", GetType(String)))
            Dt.Columns.Add(New DataColumn("Servizio_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Servizio_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Data_Inizio", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Data_Fine", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Stato", GetType(String)))
            Dt.Columns.Add(New DataColumn("Colore", GetType(String)))
            Dt.Columns.Add(New DataColumn("stato_Origine_cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("PassaggioDiStato_cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Note", GetType(String)))
            Dt.Columns.Add(New DataColumn("StatoAttuale_DES", GetType(String)))
            Dt.Columns.Add(New DataColumn("StatoAttuale_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
            Dt.Columns.Add(New DataColumn("dtStati", GetType(String)))
            Dt.Columns.Add(New DataColumn("Utente", GetType(String)))
            Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Blocco_Data", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Blocco_Username", GetType(String)))

            If Dt_Pratiche IsNot Nothing AndAlso Dt_Pratiche.Rows.Count > 0 Then

                Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                Dim DrPratica As DataRow()
                Dim PraticheCod As String() = objDistinct.SelectDistinct(Dt_Pratiche, "Pratica_Cod")
                Dim Pratica_Cod As String
                Dim strStati As String = ""
                Dim UltimoStato As String = ""
                Dim Inizio_Stato As String = ""
                Dim Descrizione As String = ""

                If PraticheCod IsNot Nothing Then

                    For i As Integer = 0 To PraticheCod.Length - 1

                        Pratica_Cod = PraticheCod(i)
                        DrPratica = Dt_Pratiche.Select("Pratica_Cod=" & Pratica_Cod)


                        If DrPratica IsNot Nothing AndAlso DrPratica.Length > 0 Then


                            Dr = Dt.NewRow

                            Dr.Item("Pratica_Cod") = Pratica_Cod
                            Dr.Item("Rag_Soc") = DrPratica(0).Item("Rag_Soc")
                            Dr.Item("Piva") = DrPratica(0).Item("Piva")
                            Dr.Item("Cuaa") = DrPratica(0).Item("Cuaa")
                            Dr.Item("Servizio_Des") = DrPratica(0).Item("Servizio_Des")
                            Dr.Item("Servizio_Cod") = DrPratica(0).Item("Servizio_Cod")
                            Dr.Item("colore") = DrPratica(0).Item("colore")
                            Dr.Item("stato_Origine_cod") = DrPratica(0).Item("stato_Origine_cod")
                            Dr.Item("PassaggioDiStato_cod") = DrPratica(0).Item("PassaggioDiStato_cod")
                            Dr.Item("note") = DrPratica(0).Item("note")
                            Dr.Item("StatoAttuale_DES") = DrPratica(0).Item("StatoAttuale_DES")

                            Dr.Item("Data_Inizio") = DrPratica(0).Item("Validita_Inizio")
                            Dr.Item("Data_Fine") = DrPratica(0).Item("Validita_Fine")

                            Dr.Item("Stato") = DrPratica(0).Item("StatoFinale_Des")
                            Dr.Item("StatoAttuale_Cod") = DrPratica(0).Item("StatoFinale_Cod")

                            Dr.Item("Blocco_Flag") = DrPratica(0).Item("Blocco_Flag")
                            Dr.Item("Blocco_Data") = DrPratica(0).Item("Blocco_Data")
                            Dr.Item("Blocco_Username") = DrPratica(0).Item("Blocco_Username")

                            If DrPratica(0).Item("Stato_Cod") <> 0 Then
                                Dim objStati As New JArray
                                For j As Integer = 0 To DrPratica.Length - 1
                                    Dim objStato As New JObject
                                    'strStati = DrPratica(j).Item("Stato_Des")
                                    objStato("Stato_Des") = CStr(DrPratica(j).Item("Stato_Des"))
                                    objStato("Stato_Cod") = CStr(DrPratica(j).Item("Stato_Cod"))
                                    objStato("Validita_Inizio_Stato") = CDate(DrPratica(j).Item("Validita_Inizio_Stato")).ToShortDateString
                                    objStato("note") = CStr(DrPratica(j).Item("note"))
                                    objStato("colore") = CStr(DrPratica(j).Item("colore"))
                                    objStato("Pratica_Cod") = CInt(DrPratica(j).Item("Pratica_Cod"))
                                    objStato("PassaggioDiStato_Cod") = CInt(DrPratica(j).Item("PassaggioDiStato_Cod"))
                                    objStato("stato_Origine_cod") = CInt(DrPratica(j).Item("stato_Origine_cod"))
                                    If Not IsDBNull(DrPratica(j).Item("Utente")) Then
                                        objStato("Utente") = CStr(DrPratica(j).Item("Utente"))
                                    Else
                                        objStato("Utente") = ""
                                    End If

                                    Dim ModificaStato As Boolean = UtenteModificaTransizioneDiStato(CInt(DrPratica(j).Item("stato_Origine_cod")), CInt(DrPratica(j).Item("Stato_Cod")), CInt(DrPratica(0).Item("Servizio_Cod")), objParametri_Utenti, objParametri_Server)
                                    objStato("modificastato") = ModificaStato

                                    objStati.Add(objStato)
                                Next
                                Dr.Item("dtStati") = objStati.ToString
                            End If
                            Dr.Item("Numero") = DrPratica(0).Item("Numero")

                            Dr.Item("Utente") = DrPratica(0).Item("Utente")

                            Dt.Rows.Add(Dr)

                        End If
                    Next


                End If

            End If

            dt_str = Newtonsoft.Json.JsonConvert.SerializeObject(Dt)

            r.RispostaOK = True
            r.RispostaStringa = dt_str

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Pratiche_Light(ByVal FiltroImprese_str As String, dataFiltro As Date, filtroServiziStati As String, filtroServizi As List(Of Integer)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt_str As String = ""

            Dim RagSoc As String = ""
            Dim Piva As String = ""
            Dim Cuaa As String = ""
            Dim Servizio_Cod As Integer = 0
            Dim Stato_Cod As Integer = 0
            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE
            Dim FiltroStato As String = ""
            Dim Filtro As String = ""
            Dim FiltroImprese As String = ""


            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------


            Filtro = GetFiltro(UtilityProvider.Agro_SQL_SaveText(FiltroImprese_str), objParametri_Server, objParametri_Utenti)

            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim DTProfilo As DataTable
            Dim Sql_Permessi As String = ""


            DTProfilo = objProfilo.Leggi(
                HttpContext.Current.Session("ASG_Utente_Username"),
                CInt(HttpContext.Current.Session("ASG_IdServizio")),
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Utenti
            )



            If DTProfilo.Rows.Count > 0 Then
                Sql_Permessi = DTProfilo.Rows(0).Item("Descrizione_2") & ""
            End If
            '----------------------------------------------------------------

            Dim Filtro_Visibilita_Utente = True
            If Sql_Permessi = "" Then
                Filtro_Visibilita_Utente = False
            End If

            If dataFiltro <> AGRODATAINIZIO Then
                DataInizio = dataFiltro
                DataFine = dataFiltro
            End If

            If filtroServiziStati <> "" Then
                'Lavez - 21/11/2024 - Fix segnalazione mail errore su ricerca pratiche se si attiva il filtro Aziende per Pratiche e non si valorizza l'elenco dei servizi e relativi stati
                Dim strFiltroServiziStati As String = " AND (" & vbCrLf
                Dim objFiltroServiziStati = JArray.Parse(filtroServiziStati)
                Dim i = 0
                For Each objFiltro In objFiltroServiziStati
                    If i <> 0 Then
                        'Select Case CStr(objFiltro("Operatore"))
                        '    Case "e"
                        '        Filtro &= " AND "
                        '    Case "o"
                        '        Filtro &= " OR "
                        'End Select

                        strFiltroServiziStati &= " AND "

                    End If

                    Dim FServizio_Cod = CInt(objFiltro("Servizio_Cod"))

                    Dim filtroStati As Boolean = False
                    Dim FStato_Cod As Integer

                    If CInt(objFiltro("Stati_Cod")) <> 0 Then
                        filtroStati = True
                        FStato_Cod = CInt(objFiltro("Stati_Cod"))
                    End If

                    strFiltroServiziStati &= " ( Pratiche.Piva IN (SELECT DISTINCT Piva FROM Pratiche " & vbCrLf
                    If filtroStati Then
                        strFiltroServiziStati &= " JOIN Pratiche_Stati_Attuali ON Pratiche.Pratica_Cod = Pratiche_Stati_Attuali.Pratica_Cod " & vbCrLf
                    End If
                    strFiltroServiziStati &= " WHERE Servizio_Cod = " & FServizio_Cod & " "

                    If filtroStati Then
                        strFiltroServiziStati &= " AND Pratiche_Stati_Attuali.Stato_Cod = " & FStato_Cod
                    End If

                    strFiltroServiziStati &= " ) ) " & vbCrLf


                    i += 1
                Next

                strFiltroServiziStati &= " ) "
                'Lavez - 21/11/2024 - Fix segnalazione mail errore su ricerca pratiche se si attiva il filtro Aziende per Pratiche e non si valorizza l'elenco dei servizi e relativi stati
                If i > 0 Then
                    Filtro &= strFiltroServiziStati
                End If

            End If

            If filtroServizi.Count > 0 Then
                Filtro &= " AND Pratiche.Servizio_Cod IN ("
                For i = 0 To filtroServizi.Count - 1
                    Filtro &= " " & filtroServizi(i) & " "
                    If i <> filtroServizi.Count - 1 Then
                        Filtro &= ","
                    End If
                Next

                Filtro &= " ) "

            End If

            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim visualizza_Kpin_BlockName As Boolean = False

            Dim dtVKPIN = objImpost.Leggi2(2, objParametri_Server.SuperUserUsername,
                         enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName,
                         "", "", objParametri_Utenti)

            If dtVKPIN.Rows.Count > 0 AndAlso dtVKPIN.Rows(0)("Impostazione_Valore_1") = "1" Then
                visualizza_Kpin_BlockName = True
            End If

            Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim Dt_Pratiche As DataTable
            Dt_Pratiche = ObjPratiche.Leggi_FiltroUtente(0,
                               RagSoc, Piva, Cuaa,
                               0, 0, 0,
                               Servizio_Cod,
                               Stato_Cod,
                               DataInizio, DataFine,
                               Filtro,
                               " Rag_Soc, Servizio_Des, Pratiche.Validita_Inizio, Ordine ",
                               Filtro_Visibilita_Utente,
                               objParametri_Server,
                               objParametri_Utenti,
                               visualizza_Kpin_BlockName)

            dt_str = Newtonsoft.Json.JsonConvert.SerializeObject(Dt_Pratiche)

            r.RispostaOK = True
            r.RispostaStringa = dt_str

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_PassaggiDiStato(ByVal Pratica_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt_str As String = ""

            Dim RagSoc As String = ""
            Dim Piva As String = ""
            Dim Cuaa As String = ""
            Dim Servizio_Cod As Integer = 0
            Dim Stato_Cod As Integer = 0
            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE
            Dim FiltroStato As String = ""
            Dim Filtro As String = ""
            Dim FiltroImprese As String = ""


            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------

            Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim Dt_Pratiche As DataTable
            Dt_Pratiche = ObjPratiche.Leggi_conStoricoTransizioniDiStato_FiltroUtente(Pratica_Cod,
                               RagSoc, Piva, Cuaa,
                               0, 0, 0,
                               Servizio_Cod,
                               Stato_Cod,
                               DataInizio, DataFine,
                               Filtro,
                               " Rag_Soc, Servizio_Des, Validita_Inizio_Stato, PassaggioDiStato_cod ",
                               False,
                               objParametri_Server,
                               objParametri_Utenti)


            'aggiungo le colonne necessarie al gridview
            Dim Dt As New DataTable

            Dim objStati As New JArray
            For Each Pratica_Row In Dt_Pratiche.Rows
                Dim objStato As New JObject
                objStato("Stato_Des") = CStr(Pratica_Row.Item("Stato_Des"))
                objStato("Stato_Cod") = CStr(Pratica_Row.Item("Stato_Cod"))
                objStato("Validita_Inizio_Stato") = CDate(Pratica_Row.Item("Validita_Inizio_Stato")).ToShortDateString
                objStato("note") = CStr(Pratica_Row.Item("note"))
                objStato("colore") = CStr(Pratica_Row.Item("colore"))
                objStato("Pratica_Cod") = CInt(Pratica_Row.Item("Pratica_Cod"))
                objStato("PassaggioDiStato_Cod") = CInt(Pratica_Row.Item("PassaggioDiStato_Cod"))
                objStato("stato_Origine_cod") = CInt(Pratica_Row.Item("stato_Origine_cod"))
                If Not IsDBNull(Pratica_Row.Item("Utente")) Then
                    objStato("Utente") = CStr(Pratica_Row.Item("Utente"))
                Else
                    objStato("Utente") = ""
                End If

                Dim ModificaStato As Boolean = UtenteModificaTransizioneDiStato(CInt(Pratica_Row.Item("stato_Origine_cod")), CInt(Pratica_Row.Item("Stato_Cod")), CInt(Pratica_Row.Item("Servizio_Cod")), objParametri_Utenti, objParametri_Server)
                objStato("modificastato") = ModificaStato

                objStati.Add(objStato)

            Next


            dt_str = objStati.ToString

            r.RispostaOK = True
            r.RispostaStringa = dt_str

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Public Shared Function UtenteModificaTransizioneDiStato(Stato_Origine As Integer, Stato_Destinazione As Integer, Servizio_Cod As Integer, objParametri_Utenti As AgronicaCoreParametri, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim Gruppo_Utente_Cod As Integer
        Dim gruppo_Utente_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim dt_Gruppo = gruppo_Utente_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
        If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
            Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
        End If

        Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim rval As DataTable

        rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                    0,
                    Servizio_Cod,
                    Gruppo_Utente_Cod,
                    "",
                    "",
                    objParametri_Server,
                    objParametri_Utenti,
                    AGRODATAINIZIO,
                    AGRODATAFINE
                    )
        If rval.Rows.Count > 0 Then
            rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                    Stato_Origine,
                    Servizio_Cod,
                    Gruppo_Utente_Cod,
                    "",
                    "",
                    objParametri_Server,
                    objParametri_Utenti,
                    AGRODATAINIZIO,
                    AGRODATAFINE
                    )
        Else
            rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                    Stato_Origine,
                    Servizio_Cod,
                    "",
                    "",
                    objParametri_Server,
                    AGRODATAINIZIO,
                    AGRODATAFINE
                    )
        End If

        If rval.Select(" WAnagraficaStati_Cod = " & Stato_Destinazione & " ").ToArray.Count > 0 Then
            Return True
        Else
            Return False
        End If


    End Function

    Public Shared Function GetFiltro(FiltroImprese_str As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As String
        Dim Filtro As String
        Dim FiltroImprese As String = ""

        If FiltroImprese_str <> "" AndAlso FiltroImprese_str <> "-1" Then

            'Dim lArrPiva = JArray.Parse(FiltroImprese_str)

            'If lArrPiva.Count > 0 Then
            '    FiltroImprese =
            '        String.Join(
            '            ",",
            '            lArrPiva
            '        )

            '    FiltroImprese = "Imprese.Piva IN (" & FiltroImprese & ") "
            FiltroImprese = " Imprese.Piva = '" & FiltroImprese_str & "' "

        Else

            'Dim DT_Imprese = Carica_Imprese(objParametri_Server)
            'Dim listaPive As String() = (From dr As DataRow In DT_Imprese Select "'" & CStr(dr.Item("piva")) & "'").ToArray()
            'FiltroImprese = " Imprese.Piva IN (" & String.Join(",", listaPive) & ") "
            'If DT_Imprese.Rows.Count > 0 Then
            '    'For i = 0 To DT_Imprese.Rows.Count - 1
            '    '    FiltroImprese &= "'" & DT_Imprese.Rows(i)("piva") & "'"
            '    '    If i <> DT_Imprese.Rows.Count - 1 Then
            '    '        FiltroImprese &= ", "
            '    '    End If
            '    'Next
            '    'FiltroImprese = " Imprese.Piva IN (" & FiltroImprese & ") "
            'Else
            '    FiltroImprese = " Imprese.Piva IS NULL "
            'End If

            FiltroImprese = " 1 = 1 "

        End If

        Dim FiltroServizi = ""

        Dim objImpUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim elencoPratiche As New List(Of Integer)
        Dim dtImp = objImpUtenti.Leggi(enum_Impostazioni_Utenti.UTENTE_PRATICHE_DA_ATTIVARE_SCARICO_FASCICOLO, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If dtImp IsNot Nothing AndAlso dtImp.Rows.Count > 0 Then

            Dim strWS As String = dtImp.Rows(0)("Impostazione_Valore_1")
            If strWS <> "" Then
                Dim strWSArr = strWS.Split("|")
                For Each wss In strWSArr
                    If IsNumeric(wss) Then
                        elencoPratiche.Add(CInt(wss))
                    End If
                Next
            End If

        End If

        'If elencoPratiche.Count > 0 Then

        '    'FiltroServizi =
        '    '        String.Join(
        '    '            ",",
        '    '            elencoPratiche
        '    '        )

        '    'FiltroServizi = " Servizi.Servizio_Cod IN ( " & FiltroServizi & " ) "

        'Else
        '    ''Non devo vedere niente, serizio_cod = 0
        '    'FiltroServizi = " Servizi.Servizio_Cod IS NULL "
        'End If

        FiltroServizi = " 1=1 "

        Filtro = FiltroImprese & " AND " & FiltroServizi

        Return Filtro

    End Function

    Public Shared Function Carica_Imprese(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As DataTable

        'Dim FiltroxUtente As String = ""
        'Dim Num_Totale As Integer = 0
        'Dim TipoSelect As enum_TipoSelect_FiltroneSuperNova
        'Dim Piva As String = ""

        'Dim ClassJoin As New JoinFiltrone
        'Dim Dt_Imprese As DataTable

        'Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        ''se ha un filtro
        'classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)


        'ClassJoin.bGerarchiaImprese = True
        'ClassJoin.bImpreseCodici_CodiceCuaa = True

        ''End If

        ''TipoSelect = TipoSelect_FiltroneSuperNova_from_TipoFiltroImprese(enTipoFiltro)
        'TipoSelect = enum_TipoSelect_FiltroneSuperNova.Imprese
        'Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server,
        '                                        "",
        '                                        TipoSelect,
        '                                        "ORDER BY Imprese.rag_soc",
        '                                        ClassJoin)

        ''''''Dt_Imprese.Columns.Add(New DataColumn("CodiceSocio", GetType(String)))
        ''For j = 0 To Dt_Imprese.Rows.Count - 1
        ''    Dim ic As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        ''    Dt_Imprese.Rows(j).Item("CodiceSocio") = ic.Leggi_Codice_Socio(Dt_Imprese.Rows(j).Item("piva"), objParametri_Server)
        ''Next

        'Dim view = New DataView(Dt_Imprese)
        'Dim distinctValues = view.ToTable(True, "piva", "Rag_Soc", "CodiceCuaa")

        ''Creo un unica stringa
        'distinctValues.Columns.Add("strUnicaImpresa", Type.GetType("System.String"))
        'For i = 0 To distinctValues.Rows.Count - 1
        '    distinctValues.Rows(i).Item("strUnicaImpresa") = distinctValues.Rows(i).Item("Rag_Soc") & " (" & String.Join(" - ", {distinctValues.Rows(i).Item("CodiceCuaa")}.Where(Function(s) Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s) AndAlso s <> "0")) & ")"
        'Next

        'Return distinctValues

        Dim objAnagrafeBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_R

        Dim dt = objAnagrafeBIZ.Imprese_Leggi_VisibilitaUtente_CUAA(objParametri_Server, objParametri_Utenti)

        Return dt

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function BloccaPratiche(blocca As Integer, pratiche As List(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W

            For Each pratica_cod In pratiche
                objPratica_W.Blocca(pratica_cod, blocca, "", objParametri_Server)
            Next

            r.RispostaOK = True
            r.RispostaStringa = "Fatto!"
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Cmb_Imprese() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dt_imprese = Carica_Imprese(objParametri_Server, objParametri_Utenti)

            Dim JArrayLista As New JArray()
            For Each dr In dt_imprese.Rows
                JArrayLista.Add(New JObject(New JProperty("Piva", dr.Item("piva")), New JProperty("Rag_Soc", dr.Item("rag_soc"))))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JArrayLista.ToString
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Permesso_Elenco_Pratiche() As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If
            Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
                r.Sessione = False
                Return r
            End If

            Dim Imposta_Pratica As Boolean = False

            Dim objPratiche As New JArray

            Dim objImpUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim elencoPratiche As New List(Of Integer)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim wizardComportamentoWSstr = objConfSiti.Leggi_Valore(0, "enum_PC_Anteprima_wizardComportamentoWS", "", "", objParametri_Server)
            Try
                Dim wizardComportamentoWS_JSON = JObject.Parse(wizardComportamentoWSstr)
                Imposta_Pratica = CBool(wizardComportamentoWS_JSON("Imposta_Pratica"))
            Catch ex As Exception

            End Try


            Dim dtImp = objImpUtenti.Leggi(enum_Impostazioni_Utenti.UTENTE_PRATICHE_DA_ATTIVARE_SCARICO_FASCICOLO, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_utenti)
            If dtImp IsNot Nothing AndAlso dtImp.Rows.Count > 0 Then

                Dim strWS As String = dtImp.Rows(0)("Impostazione_Valore_1")
                If strWS <> "" Then
                    Dim strWSArr = strWS.Split("|")
                    For Each wss In strWSArr
                        If IsNumeric(wss) Then
                            elencoPratiche.Add(CInt(wss))
                        End If
                    Next
                End If

            End If

            Dim objServizi As New AgronicaCoreMetaSchemaDAL.Servizi_R

            If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then

                Dim dtServ As DataTable = objServizi.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                For Each row In dtServ.Rows
                    Dim objServizio As New JObject
                    objServizio("Servizio_Cod") = row("Servizio_Cod").ToString
                    objServizio("Servizio_Des") = row("Servizio_Des").ToString
                    objPratiche.Add(objServizio)
                Next

            Else
                If elencoPratiche.Count > 0 Then
                    Imposta_Pratica = True
                    Dim strFiltro = " Servizio_Cod IN ("

                    For i = 0 To elencoPratiche.Count - 1

                        If i = 0 Then
                            strFiltro &= " " & elencoPratiche.ElementAt(i)
                        Else
                            strFiltro &= ", " & elencoPratiche.ElementAt(i)
                        End If

                    Next

                    strFiltro &= ")"


                    Dim dtServ As DataTable = objServizi.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, strFiltro, "", objParametri_Server)

                    For Each row In dtServ.Rows
                        Dim objServizio As New JObject
                        objServizio("Servizio_Cod") = row("Servizio_Cod").ToString
                        objServizio("Servizio_Des") = row("Servizio_Des").ToString
                        objPratiche.Add(objServizio)
                    Next

                Else

                    Dim dtServ As DataTable = objServizi.Leggi(0, "", AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                    For Each row In dtServ.Rows
                        Dim objServizio As New JObject
                        objServizio("Servizio_Cod") = row("Servizio_Cod").ToString
                        objServizio("Servizio_Des") = row("Servizio_Des").ToString
                        objPratiche.Add(objServizio)
                    Next

                End If
            End If



            r.RispostaOK = True
            r.RispostaConferma = Imposta_Pratica
            r.RispostaStringa = objPratiche.ToString

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function trovaStatiServizio(Servizio_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If
            Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
                r.Sessione = False
                Return r
            End If

            Dim objStati As New AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R

            Dim DTStati = objStati.StatiDaServizio(Servizio_Cod, "", "", objParametri_Server)

            Dim objStatiJ As New JArray
            For Each statoR In DTStati.Rows
                Dim objStatoJ As New JObject
                objStatoJ("Stato_Cod") = CInt(statoR("Stato_Origine_Cod"))
                objStatoJ("Stato_Des") = CStr(statoR("WAnagraficaStati_Des"))
                objStatiJ.Add(objStatoJ)
            Next

            r.RispostaOK = True
            r.RispostaStringa = objStatiJ.ToString

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaPratiche(str_pratiche As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim pratiche_insert = JsonConvert.DeserializeObject(Of JArray)(str_pratiche, settingLoc)
            Dim objPraticaBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_W
            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W

            Dim listaPratiche As New JArray

            For Each pratica_insert In pratiche_insert

                Dim pratica_Cod As Integer = CInt(pratica_insert("Pratica_Cod"))
                'Dim Data_Inizio = AgronicaCoreUtility.DataOra.LeggiDataDaStringa_ISO8601(CStr(pratica_insert("Data_Inizio")), DateTimeKind.Local, DateTimeKind.Utc, "+1")
                Dim Data_Inizio = CDate(pratica_insert("Data_Inizio"))
                Dim Data_Fine = CDate(pratica_insert("Data_Fine"))
                Dim Numero = CStr(pratica_insert("Numero"))
                objPratica_W.AggiornaDate(pratica_Cod, Data_Inizio, Data_Fine, "", objParametri_Server)
                objPratica_W.AggiornaNumero(pratica_Cod, Numero, "", objParametri_Server)

            Next



            r.RispostaOK = True
            r.RispostaStringa = listaPratiche.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaPratiche(str_pratiche As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim pratiche_delete = JArray.Parse(str_pratiche)

            Dim objPratica_W As New AgronicaCoreProfilazioneBIZ.Pratiche_W


            Dim Messaggio_Errore = ""


            Dim listaPratiche As New JArray
            Dim EliminataPratica As Boolean = False
            For Each pratica_delete In pratiche_delete

                Dim pratica_Cod As Integer = CInt(pratica_delete("Pratica_Cod"))

                ' objParametri_utenti passato per invalidare la cache visibilita' appoggio (ex trigger tr_up_cascade_update)
                EliminataPratica = objPratica_W.Elimina_Pratica(pratica_Cod, objParametri_Server, Messaggio_Errore, objParametri_utenti)

            Next

            Dim messaggioFinale = ""
            If EliminataPratica Then
                If pratiche_delete.Count = 1 Then
                    messaggioFinale = "La pratica selezionata è stata eliminata correttamente"
                Else
                    messaggioFinale = "Le pratiche selezionate sono state eliminate correttamente"
                End If
            Else
                messaggioFinale = "Si è verificato un errore durante l'eliminazione della pratica: " & Messaggio_Errore
            End If

            r.RispostaOK = True
            r.RispostaStringa = messaggioFinale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UndoPratiche(str_pratiche As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Dim pratiche_undo = JArray.Parse(str_pratiche)

            Dim objPratica_W As New AgronicaCoreProfilazioneBIZ.Pratiche_W



            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                objParametri_Server)

            Dim messaggioFinale = ""

            Dim listaPratiche As New JArray
            For Each pratica_delete In pratiche_undo

                Dim pratica_Cod As Integer = CInt(pratica_delete)

                objPratica_W.UndoPratica(pratica_Cod, objParametri_Server, objParametri_utenti, messaggioFinale)

            Next

            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            r.RispostaOK = True
            r.RispostaStringa = messaggioFinale

        Catch ex As Exception

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try



        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AttivaServizio(ByVal piva As String, Servizio_Cod As Integer, Data_Inizio_str As String, Data_Fine_str As String, Data_Inizio_Pratica_str As String, Numero As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim objPraticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W

        r = objPraticheW.GeneraPratica(piva,
                                       Servizio_Cod,
                                       Data_Inizio_str,
                                       Data_Fine_str,
                                       Data_Inizio_Pratica_str,
                                       Numero,
                                       objParametri_Server,
                                       objParametri_utenti)

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaProcedura(Pratica_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            HttpContext.Current.Session.Remove("dtStatiPerPassaggio")
            HttpContext.Current.Session.Remove("Servizi_Lista_dtPacchetti")


            Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R

            Dim DTWorkflow = New DataView(PredisponiPerPassaggioDiStato(Pratica_Cod, objParametri_Server)).ToTable(True, "WorkFlow_Cod", "WorkFlow_Des")

            Dim dt_str = Newtonsoft.Json.JsonConvert.SerializeObject(DTWorkflow)

            r.RispostaOK = True
            r.RispostaStringa = dt_str

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaPacchettiDSS() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim ss As String = xWS.ModelliPrevisionaliRaggruppamenti_Lista(objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            ss = ss.Replace("{""ModelliPrevisionaliRaggruppamenti_ElencoResult"":", "")
            ss = ss.Substring(0, ss.Length - 1)
            Dim rs As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelliPrevisionaliRaggruppamenti)) =
                    jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelliPrevisionaliRaggruppamenti)))(ss)

            Dim str_Res As String = ""

            str_Res = Newtonsoft.Json.JsonConvert.SerializeObject(rs.RispostaStringa)

            r.RispostaOK = True
            r.RispostaStringa = str_Res

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function StatiDestinazione(Stato_Cod As Integer, Servizio_Cod As Integer, elencoPratiche As Integer()) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Gruppo_Utente_Cod As Integer
            Dim gruppo_Utente_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            Dim dt_Gruppo = gruppo_Utente_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
            If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
            End If

            Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim rval As DataTable
            Dim maxDateObj = MaxMinDateInizioeFinePratiche(elencoPratiche, objParametri_Server)

            rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        0,
                        Servizio_Cod,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        maxDateObj.maxDataInizio,
                        maxDateObj.minDataFine
                        )
            If rval.Rows.Count > 0 Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        Stato_Cod,
                        Servizio_Cod,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        maxDateObj.maxDataInizio,
                        maxDateObj.minDataFine
                        )
            Else
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                        Stato_Cod,
                        Servizio_Cod,
                        "",
                        "",
                        objParametri_Server,
                        maxDateObj.maxDataInizio,
                        maxDateObj.minDataFine
                        )
            End If

            If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                                                                                    Stato_Cod,
                                                                                    Servizio_Cod,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri_Server,
                                                                                    maxDateObj.maxDataInizio,
                                                                                    maxDateObj.minDataFine
                                                                                    )
            End If

            'Lavez - 26/09/2025 - Filtraggio stati destinazione in base alla destinazione
            rval = FiltraStatiDestinazione(Servizio_Cod, elencoPratiche(0), rval, objParametri_Server, objParametri_Utenti)

            Dim arrayStati As New JArray
            For Each row In rval.Rows
                Dim obj_Stato As New JObject

                obj_Stato("Stato_Cod") = CInt(row("WAnagraficaStati_cod"))
                obj_Stato("Stato_Des") = CStr(row("WAnagraficaStati_Des")) & " [" & CStr(row("WTransizioniDiStatoConfigurazione_Des")) & "]"

                arrayStati.Add(obj_Stato)
            Next

            r.RispostaOK = True
            r.RispostaStringa = arrayStati.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StatiDestinazioneUMA(Pratica_Cod As Integer, Stato_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Gruppo_Utente_Cod As Integer
            Dim coordinatore As Boolean = False
            Dim gruppo_Utente_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Dim gruppo_R As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
            Dim uma_testata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            Dim approvatore As String = ""
            Dim dt_Gruppo = gruppo_Utente_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_Utenti)
            If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")
                Dim DTConfigurazioneGruppo = gruppo_R.Leggi(Gruppo_Utente_Cod, "", "", objParametri_Utenti)
                If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                    Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                    If ConfigurazioneGruppoStr <> "" Then
                        Try
                            Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                            If ConfigurazioneGruppo("CoordinatoreAfor") IsNot Nothing Then
                                If ConfigurazioneGruppo("CoordinatoreAfor") Then
                                    coordinatore = True
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If

            Dim dt_testata = uma_testata.Leggi("", 0, Pratica_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
            If dt_testata IsNot Nothing AndAlso dt_testata.Rows.Count > 0 AndAlso Not IsDBNull(dt_testata.Rows.Item(0).Item("Approvatore")) Then
                approvatore = CStr(dt_testata.Rows.Item(0).Item("Approvatore"))
            End If

            Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim rval As DataTable

            rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        0,
                        enum_Servizi.Gestione_UMA,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            If rval.Rows.Count > 0 Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStatoxGruppoUtente(
                        Stato_Cod,
                        enum_Servizi.Gestione_UMA,
                        Gruppo_Utente_Cod,
                        "",
                        "",
                        objParametri_Server,
                        objParametri_Utenti,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            Else
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                        Stato_Cod,
                        enum_Servizi.Gestione_UMA,
                        "",
                        "",
                        objParametri_Server,
                        AGRODATAINIZIO,
                        AGRODATAFINE
                        )
            End If

            If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
                rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
                                                                                    Stato_Cod,
                                                                                    enum_Servizi.Gestione_UMA,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri_Server,
                                                                                    AGRODATAINIZIO,
                                                                                    AGRODATAFINE
                                                                                    )
            End If
            'If rval.Rows.Count = 0 Then
            '    rval = LeggiStatiPerPassaggio.Leggi_TransizioniDisponibiliDatoStato(
            '        Stato_Cod,
            '        Servizio_Cod,
            '        "",
            '        "",
            '        objParametri_Server
            '        )
            'End If


            Dim arrayStati As New JArray
            For Each row In rval.Rows
                Dim obj_Stato As New JObject

                If Not (coordinatore AndAlso Stato_Cod = 2002 AndAlso approvatore <> "" AndAlso
                    approvatore <> objParametri_Server.UtenteUsername AndAlso CInt(row("WAnagraficaStati_cod")) <> 2010) Then

                    obj_Stato("Stato_Cod") = CInt(row("WAnagraficaStati_cod"))
                    obj_Stato("Stato_Des") = CStr(row("WAnagraficaStati_Des")) & " [" & CStr(row("WTransizioniDiStatoConfigurazione_Des")) & "]"

                    arrayStati.Add(obj_Stato)

                End If
            Next

            r.RispostaOK = True
            r.RispostaStringa = arrayStati.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function MaxMinDateInizioeFinePratiche(elencoPratiche As Integer(), objParametri_Server As AgronicaCoreParametri) As MaxDate
        Dim dtPratiche = creaDTPratiche(elencoPratiche, objParametri_Server)
        Dim maxDateObj As New MaxDate
        Dim maxDate As Date = dtPratiche.AsEnumerable().Max(Function(r) CDate((r.Field(Of Date)("Validita_Inizio"))))
        Dim minDate As Date = dtPratiche.AsEnumerable().Min(Function(r) CDate((r.Field(Of Date)("Validita_Fine"))))
        maxDateObj.maxDataInizio = maxDate
        maxDateObj.minDataFine = minDate
        Return maxDateObj
    End Function

    Private Shared Function creaDTPratiche(elencoPratiche As Integer(), objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim objPratiche_DAL As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim dtPratiche As DataTable
        Dim first = True
        For Each pratica_cod In elencoPratiche
            Dim dtPratica As DataTable = objPratiche_DAL.Leggi(pratica_cod, "", "", "", 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, 0)
            If first Then
                dtPratiche = dtPratica.Clone
            End If
            first = False
            dtPratiche.Merge(dtPratica)
        Next
        Return dtPratiche
    End Function

    Private Shared _locker As Object = New Object()

    <WebMethod(EnableSession:=True)>
    Public Shared Function PassaggioStato(Pratiche_Str As String,
                                          Stato_Cod As Integer,
                                          Data_Riferimento_str As String,
                                          Note As String,
                                          WorkFlow_Cod As Integer,
                                          PassaggioDiStato_cod As Integer,
                                          objLista_DSS_Selezionati As String,
                                          controlli As Boolean
                                          ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        SyncLock _locker
            Try
                Lingua.Gias_InizializzaCultura_DaSession()

                Dim pratiche = JArray.Parse(Pratiche_Str)

                Dim NuovoPassaggioDiStato_cod As Integer
                Dim LeggiSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim Servizio_Cod As Integer = 0
                Dim LeggiPratica As New AgronicaCoreProfilazioneDAL.Pratiche_R
                Dim ScriviPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_W
                Dim leggiPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R
                Dim scriviPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
                Dim scriviPraticheStati_Attuale As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
                Dim LeggiPraticheStati_attuale As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
                Dim leggiDocumentiObbligatori As New AgronicaCoreScadenziario.SchemaDocumenti_R
                Dim praticheCods As List(Of Integer) = New List(Of Integer)

                Dim MessaggioErrore As String = ""
                Dim Data_Riferimento = DateTime.Now
                Dim utenteApertura As String = ""
                Dim mailUtenteApertura As String = ""
                Dim inviaMailChiusuraUMA As Boolean = False
                Dim oggettoMail As String = ""
                Dim corpoMail As String = ""

                Dim pratiche_arr_String As New List(Of String)
                For Each Pratica In pratiche
                    pratiche_arr_String.Add(CStr(Pratica("Pratica_Cod")))
                Next
                VerificaEvantualiDateNonCoerenti(MessaggioErrore, Data_Riferimento, pratiche_arr_String.ToArray, Stato_Cod, objParametri_Server)

                If MessaggioErrore <> "" Then
                    r.RispostaConferma = False
                    r.RispostaOK = True
                    r.RispostaStringa = MessaggioErrore
                    Return r
                End If
                Dim _webProvisioningColdiretti As ColdirettiProvisioner = Nothing
                Dim _UtenteTipologiaQDC_Impresa_Verde As Integer = 0
                Dim _UtenteTipologiaQDC_4_Mani As Integer = 0
                If WorkFlow_Cod = enum_Servizi.QDemetraQdCBluarancio Then
                    'lavez - 06/01/2024 - per evitare di mettere una chiave doppione, ed essendo che l'import delle anagrafiche da datapublish e l'avanzamento di stato delle pratiche sono legate a doppio filo
                    '                     recupero il base url del provisioning coldiretti dai parametri del GSB (Tipo_sincro 139/143)

                    Dim _baseurl As String = ""
                    _baseurl = ConfigurationManager.AppSettings("BaseUrlColdirettiWebServicePortaleSocio")
                    If _baseurl = "" Then
                        Throw New Exception("Servizio DemetraQDCBluarancio - Link a provisionig coldiretti non trovato in configurazione appsettings. impossibile proseguire")
                    End If

                    _webProvisioningColdiretti = New ColdirettiProvisioner(_baseurl, New Tuple(Of String, String)("", ""), objParametri_Server)

                    _UtenteTipologiaQDC_Impresa_Verde = IIf(ConfigurationManager.AppSettings("TipologiaCod_QDC_ImpresaVerde") = "", 0, CInt(ConfigurationManager.AppSettings("TipologiaCod_QDC_ImpresaVerde")))
                    If _UtenteTipologiaQDC_Impresa_Verde = 0 Then
                        Throw New Exception("Tipologia utente per passaggio qdc a impresa verde non impostato. impossibile proseguire")
                    End If
                    _UtenteTipologiaQDC_4_Mani = IIf(ConfigurationManager.AppSettings("TipologiaCod_QDC_4_Mani") = "", 0, CInt(ConfigurationManager.AppSettings("TipologiaCod_QDC_4_Mani")))
                    If _UtenteTipologiaQDC_4_Mani = 0 Then
                        Throw New Exception("Tipologia utente per passaggio qdc a impresa verde non impostato. impossibile proseguire")
                    End If
                End If

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Apro la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)
                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                Dim Stato_iniziale = StatoDatoWorkflow(WorkFlow_Cod, "WanagraficaStati_COD", objParametri_Server)

                Dim Gruppo_Utente_Cod As Integer
                Dim Utente_xGruppi_R As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
                Dim Gruppi_Utente_R As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
                Dim Configurazione_Gruppo_str As String = ""

                Dim dt_Gruppo = Utente_xGruppi_R.Leggi(objParametri_Server.UtenteUsername, 0, "", "", objParametri_utenti)
                If dt_Gruppo IsNot Nothing AndAlso dt_Gruppo.Rows.Count > 0 Then
                    Gruppo_Utente_Cod = dt_Gruppo.Rows(0)("Gruppi_Utente_Cod")

                    Dim dtGruppo_Utente = Gruppi_Utente_R.Leggi(Gruppo_Utente_Cod, "", "", objParametri_utenti)
                    If dtGruppo_Utente.Rows.Count > 0 AndAlso Not IsDBNull(dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive")) AndAlso dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive") <> "" Then
                        Configurazione_Gruppo_str = dtGruppo_Utente.Rows(0)("ConfigurazioniAggiuntive")
                    End If

                End If

                If PassaggioDiStato_cod = 0 Then
                    For Each Pratica In pratiche

                        Dim Pratica_Cod As Integer = Pratica("Pratica_Cod")
                        praticheCods.Add(Pratica_Cod)
                        'If Stato_iniziale Is Nothing AndAlso EsisteStatoAttuale.Rows.Count > 0 Then
                        '    Stato_iniziale = EsisteStatoAttuale.Rows(0)("Stato_Cod")
                        'End If

                        Servizio_Cod = LeggiPratica.Leggi_Servizio_DaPratica(Pratica_Cod, "", "", objParametri_Server)

                        Dim StatoAttuale = LeggiPratica.LeggiXServiziStatoAttuale(WorkFlow_Cod, "", " p.pratica_cod = " & Pratica_Cod.ToString, "", objParametri_Server, True).Rows.Item(0).Item("WAnagraficaStati_Cod")

                        Dim Anno = LeggiPratica.Leggi(Pratica_Cod, "", "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, 0).Rows.Item(0).Item("Anno")

                        '----------------------------------------------------------------------------------------------------
                        'Controlli UMA 
                        '----------------------------------------------------------------------------------------------------
                        If Servizio_Cod = enum_Servizi.Gestione_UMA Then
                            ControlliUMA(Stato_Cod,
                                         controlli,
                                         r,
                                         objParametri_Server,
                                         objParametri_utenti,
                                         FlagConnessioneLocale,
                                         Servizio_Cod,
                                         leggiDocumentiObbligatori,
                                         praticheCods,
                                         Pratica_Cod,
                                         StatoAttuale,
                                         Anno,
                                         inviaMailChiusuraUMA,
                                         oggettoMail,
                                         corpoMail,
                                         utenteApertura,
                                         mailUtenteApertura)
                            If r.Errore.Length > 0 Then
                                If objParametri_Server.objTransazione IsNot Nothing Then
                                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                                End If
                                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                                Return r
                            End If

                        End If
                        '----------------------------------------------------------------------------------------------------

                        'Controllo Documenti obbligatori
                        ControlloDocumentiObbligatori(Stato_Cod, r, objParametri_Server, FlagConnessioneLocale, Servizio_Cod, leggiDocumentiObbligatori, Pratica_Cod, StatoAttuale, Anno)

                        If r.Errore.Length > 0 Then
                            If objParametri_Server.objTransazione IsNot Nothing Then
                                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                            End If

                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

                            Return r
                        End If

                        If Not PassaggioDiStatoValido(Pratica_Cod, Servizio_Cod, Stato_iniziale, Stato_Cod, objParametri_Server) Then
                            If objParametri_Server.objTransazione IsNot Nothing Then
                                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                            End If

                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
                            r.RispostaOK = False
                            r.RispostaStringa = Resources.AgronicaAgenda_2010.PassaggioDiStatoNonValido
                            Return r
                        End If

                        Dim EsisteStatoAttuale As DataTable =
                            LeggiPraticheStati_attuale.Leggi(
                                Pratica_Cod,
                                "",
                                "",
                                objParametri_Server
                            )

                        For Each pra_cod In praticheCods
                            NuovoPassaggioDiStato_cod =
                                LeggiSequenze.NuovoId_Tabella(
                                 "PassaggioDiStato_cod",
                                 0,
                                 _UpperBoundTabelle,
                                 objParametri_Server
                             )

                            If EsisteStatoAttuale.Rows.Count = 0 Then

                                scriviPraticheStati_Attuale.Scrivi(
                                pra_cod,
                                Stato_Cod,
                                Note,
                                Data_Riferimento,
                                AGRODATAFINE,
                                objParametri_Server, #2/1/1900#, #2/1/1900#, "", ""
                            )
                            Else

                                scriviPraticheStati_Attuale.Modifica(
                                pra_cod,
                                Stato_Cod,
                                Stato_iniziale,
                                Note,
                                Data_Riferimento,
                                AGRODATAFINE,
                                "",
                                objParametri_Server
                            )

                            End If
                            scriviPraticheStati.Scrivi(
                                pra_cod,
                                Stato_Cod,
                                Servizio_Cod,
                                Data_Riferimento,
                                AGRODATAFINE,
                                Note,
                                Stato_iniziale,
                                NuovoPassaggioDiStato_cod,
                                objParametri_Server, #2/1/1900#, #2/1/1900#, "", ""
                        )
                        Next

                        If inviaMailChiusuraUMA Then
                            InviaMailChiusura(utenteApertura,
                                              mailUtenteApertura,
                                              oggettoMail,
                                              corpoMail,
                                              objParametri_Server,
                                              objParametri_utenti)
                        End If

                        Select Case Stato_Cod
                            Case enum_Servizi_Stati.Pratica_Chiusa,
                                enum_Servizi_Stati.Pratica_Chiusa_QdC
                                ScriviPratiche.AggiornaDate(
                                    Pratica_Cod,
                                    #2/1/1900#,
                                    Data_Riferimento,
                                    "",
                                    objParametri_Server
                                )

                            ' VAnni: 9/1/2019: a seguito di conferma definitiva (stato), si bloccano le operazioni di agenda.

                            Case enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_Completata
                                Dim leggiOperazioniPerBlocco As New AgronicaCoreContabDAL.Agenda_R
                                Dim bloccaAgenda As New AgronicaCoreContabDAL.Agenda_W
                                Dim pivaDTPerBlocco As DataTable =
                                    LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                                If pivaDTPerBlocco.Rows.Count > 0 Then

                                    Dim pivaPerBlocco As String = pivaDTPerBlocco(0)("piva").ToString
                                    If Not String.IsNullOrEmpty(pivaPerBlocco) Then

                                        Dim dtOp As DataTable =
                                            leggiOperazioniPerBlocco.Leggi(pivaPerBlocco, 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                                        Dim idAgendaPerBlocco As String =
                                            String.Join(",", (From a In dtOp.AsEnumerable Select CStr(a("id_agenda"))).ToArray())
                                        If Not String.IsNullOrEmpty(idAgendaPerBlocco) Then
                                            bloccaAgenda.Agenda_Blocca(pivaPerBlocco, 0, idAgendaPerBlocco, "", objParametri_Server)
                                        End If

                                    End If
                                End If

                            Case enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_in_corso

                                'Grilli 12/09/2019: Blocco le operazioni d'agenda dalla notte dei tempi fino alla data in cui l'utente ha messo lo stato "Compilazione alla data completata e verificata" (2007)
                                If EsisteStatoAttuale.Rows(0).Item("stato_cod") <> enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Compilazione_Alla_Data_Completata_e_Verificata AndAlso Servizio_Cod <> enum_Servizi.Gestione_UMA Then
                                    Throw New Exception("Lo stato attuale doveva essere 'Compilazione alla data completata e verificata' ")
                                End If

                                Dim dataInizioBlocco As DateTime = AGRODATAINIZIO
                                Dim dataFineBlocco As DateTime = EsisteStatoAttuale.Rows(0).Item("validita_inizio")

                                Dim pivaDTPerBlocco As DataTable = LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                                If pivaDTPerBlocco.Rows.Count > 0 Then
                                    Dim pivaPerBlocco As String = pivaDTPerBlocco(0)("piva").ToString

                                    If IsDate(pivaDTPerBlocco(0)("validita_fine")) AndAlso CDate(pivaDTPerBlocco(0)("validita_fine")) <> AGRODATAFINE AndAlso dataFineBlocco > CDate(pivaDTPerBlocco(0)("validita_fine")) Then
                                        dataFineBlocco = CDate(pivaDTPerBlocco(0)("validita_fine"))
                                    End If

                                    Select Case Servizio_Cod
                                        Case enum_Servizi.Quaderno_Campagna_Caa
                                            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                            Dim res As Boolean = objAgenda.Agenda_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not res Then
                                                Throw New Exception("Impossibile bloccare le operazioni d'agenda")
                                            End If
                                        Case enum_Servizi.QuadernoCampagnaBio
                                            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                            Dim res As Boolean = objAgenda.Agenda_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not res Then
                                                Throw New Exception("Impossibile bloccare le operazioni d'agenda")
                                            End If

                                        Case enum_Servizi.PianoConcimazione
                                            Dim objPC As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W
                                            Dim resPC As Boolean = objPC.PianoConcimazione_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not resPC Then
                                                Throw New Exception("Impossibile bloccare i piani concimazione")
                                            End If

                                            Dim objRic As New AgronicaCoreContabDAL.Ricette_W
                                            Dim resRic As Boolean = objRic.RicettePC_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not resRic Then
                                                Throw New Exception("Impossibile bloccare le ricette")
                                            End If

                                        Case enum_Servizi.PUA
                                            Dim objPUA As New AgronicaCorePUA_DAL.PUA_Testata_W
                                            Dim resPUA As Boolean = objPUA.Pua_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not resPUA Then
                                                Throw New Exception("Impossibile bloccare i pua")
                                            End If

                                            Dim objRic As New AgronicaCoreContabDAL.Ricette_W
                                            Dim resRic As Boolean = objRic.RicettePUA_Blocca(pivaPerBlocco, dataInizioBlocco, dataFineBlocco, True, "", objParametri_Server)

                                            If Not resRic Then
                                                Throw New Exception("Impossibile bloccare le ricette")
                                            End If

                                        Case enum_Servizi.ACA_2, enum_Servizi.ACA_4, enum_Servizi.ACA_12, enum_Servizi.ACA_13, enum_Servizi.ACA_24_01, enum_Servizi.ACA_24_02
                                            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                            Dim chiaviDomandaAca = AssociataServizioADomandaACA(Servizio_Cod)

                                            Dim contributi As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_R

                                            Dim idAgenda = contributi.Leggi_Disticit_IdAgenda(objParametri_Server,
                                                                                               chiaviDomandaAca.Item1,
                                                                                               chiaviDomandaAca.Item2,
                                                                                                pivaPerBlocco,
                                                                                                dataInizioBlocco,
                                                                                                dataFineBlocco)

                                            Dim idAgendaPerBlocco As String =
                                                String.Join(",", (From a In idAgenda.AsEnumerable Select CStr(a("id_agenda"))).ToArray())

                                            If Not String.IsNullOrEmpty(idAgendaPerBlocco) Then
                                                Dim bloccaAgenda As New AgronicaCoreContabDAL.Agenda_W
                                                bloccaAgenda.Agenda_Blocca(pivaPerBlocco, 0, idAgendaPerBlocco, "", objParametri_Server)
                                            End If
                                    End Select
                                End If

                            Case enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione

                                'Grilli 30/09/2019: Sblocco le operazioni d'agenda dalla data di ultima approvazione fino alla data in cui l'utente ha messo lo stato "Compilazione alla data completata e verificata" (2007)
                                If EsisteStatoAttuale.Rows(0).Item("stato_cod") = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_Verifica_Completata_Con_Riserva Then

                                    'Estraggo la piva
                                    Dim pivaDTPerSblocco As DataTable = LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                                    'Cerco la data dalla quale devo iniziare a sbloccare:
                                    Dim dtStatixSbloccoInizio As DataTable = leggiPraticheStati.Leggi(Pratica_Cod, 2001, AGRODATAINIZIO, AGRODATAFINE, " passaggiodistato_cod <> " & NuovoPassaggioDiStato_cod, " Pratiche_Stati.Validita_inizio DESC", objParametri_Server, 0)
                                    Dim dataInizioSblocco As DateTime = dtStatixSbloccoInizio.Rows(0).Item("Validita_inizio")

                                    'Cerco la data dalla quale devo finire di sbloccare:
                                    Dim dtStatixSbloccoFine As DataTable = leggiPraticheStati.Leggi(Pratica_Cod, 2007, AGRODATAINIZIO, AGRODATAFINE, "", " Pratiche_Stati.Validita_inizio DESC", objParametri_Server, 0)
                                    Dim dataFineSblocco As DateTime = dtStatixSbloccoFine.Rows(0).Item("Validita_inizio")

                                    If pivaDTPerSblocco.Rows.Count > 0 Then
                                        Dim pivaPersBlocco As String = pivaDTPerSblocco(0)("piva").ToString

                                        Select Case Servizio_Cod
                                            Case enum_Servizi.Quaderno_Campagna_Caa
                                                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                                Dim res As Boolean = objAgenda.Agenda_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not res Then
                                                    Throw New Exception("Impossibile sbloccare le operazioni d'agenda")
                                                End If
                                            Case enum_Servizi.QuadernoCampagnaBio
                                                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                                Dim res As Boolean = objAgenda.Agenda_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not res Then
                                                    Throw New Exception("Impossibile sbloccare le operazioni d'agenda")
                                                End If
                                            Case enum_Servizi.PianoConcimazione
                                                Dim objPC As New AgronicaCoreAnagrafeDAL.PianoConcimazione_Testata_W
                                                Dim resPC As Boolean = objPC.PianoConcimazione_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not resPC Then
                                                    Throw New Exception("Impossibile sbloccare i piani concimazione")
                                                End If

                                                Dim objRic As New AgronicaCoreContabDAL.Ricette_W
                                                Dim resRic As Boolean = objRic.RicettePC_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not resRic Then
                                                    Throw New Exception("Impossibile sbloccare le ricette")
                                                End If

                                            Case enum_Servizi.PUA
                                                Dim objPUA As New AgronicaCorePUA_DAL.PUA_Testata_W
                                                Dim resPUA As Boolean = objPUA.Pua_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not resPUA Then
                                                    Throw New Exception("Impossibile sbloccare i pua")
                                                End If

                                                Dim objRic As New AgronicaCoreContabDAL.Ricette_W
                                                Dim resRic As Boolean = objRic.RicettePUA_Sblocca(pivaPersBlocco, dataInizioSblocco, dataFineSblocco, "", objParametri_Server)

                                                If Not resRic Then
                                                    Throw New Exception("Impossibile sbloccare le ricette")
                                                End If
                                            Case enum_Servizi.ACA_2, enum_Servizi.ACA_4, enum_Servizi.ACA_12, enum_Servizi.ACA_13, enum_Servizi.ACA_24_01, enum_Servizi.ACA_24_02
                                                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W
                                                Dim chiaviDomandaAca = AssociataServizioADomandaACA(Servizio_Cod)


                                                Dim contributi As New AgronicaCoreAnagrafeDAL.ProgettiXContributi_R

                                                Dim idAgenda = contributi.Leggi_Disticit_IdAgenda(objParametri_Server,
                                                                                                   chiaviDomandaAca.Item1,
                                                                                                   chiaviDomandaAca.Item2,
                                                                                                    pivaPersBlocco,
                                                                                                    dataInizioSblocco,
                                                                                                    dataFineSblocco)

                                                Dim idAgendaPerSblocco As String =
                                                    String.Join(",", (From a In idAgenda.AsEnumerable Select CStr(a("id_agenda"))).ToArray())

                                                If Not String.IsNullOrEmpty(idAgendaPerSblocco) Then
                                                    Dim bloccaAgenda As New AgronicaCoreContabDAL.Agenda_W
                                                    bloccaAgenda.Agenda_Sblocca(pivaPersBlocco, 0, idAgendaPerSblocco, "", objParametri_Server)
                                                End If

                                        End Select

                                    End If

                                End If
                            Case enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__Pagante

                                Select Case Servizio_Cod
                                    Case 1009
                                        Dim dtPacchetti = JArray.Parse(objLista_DSS_Selezionati)
                                        Dim idle As Integer = 0
                                        Dim pivaDTPerBlocco As DataTable =
                                            LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                                        Dim pivaPacchetto As String = pivaDTPerBlocco(0)("piva").ToString
                                        Dim pivaSuperUserPacchetto As String = pivaDTPerBlocco(0)("piva_SuperUser").ToString
                                        Dim ScriviPacchetti As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_ModelliPrevisionali_W

                                        For Each drP In dtPacchetti

                                            If pivaDTPerBlocco.Rows.Count > 0 Then

                                                'scrittura locale
                                                Dim pacchettoCommercialeCod As Integer = drP("ModelliPrevisionaliRaggruppamenti_Cod")
                                                Dim Data_Scadenza_i = CDate(drP("Data_Scadenza"))
                                                Dim Data_Scadenza = New Date(Data_Scadenza_i.Year, Data_Scadenza_i.Month, Data_Scadenza_i.Day)
                                                Dim lDataScadenaPacchetto As DateTime = Data_Scadenza

                                                ScriviPacchetti.Scrivi(
                                                    pivaSuperUserPacchetto, pivaPacchetto, NuovoPassaggioDiStato_cod, Pratica_Cod, Stato_Cod, 0, Stato_iniziale, pacchettoCommercialeCod, lDataScadenaPacchetto, "", objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")

                                                'chiamata a web service
                                                SalvaPassaggioDiStato_ChiamaProvisioning(pivaPacchetto, pivaSuperUserPacchetto, pacchettoCommercialeCod, lDataScadenaPacchetto, objParametri_Super_Server)
                                            End If
                                        Next

                                End Select

                        End Select

                        Select Case Servizio_Cod
                            Case enum_Servizi.QDemetraQdCBluarancio
                                Dim cuaa As String = ""
                                Dim pivaDT As DataTable =
                                            LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)
                                If pivaDT.Rows.Count > 0 Then
                                    cuaa = pivaDT.Rows(0)("CUAA")
                                Else
                                    Throw New Exception(String.Format("Pratica ({0}) non trovata", Pratica_Cod))
                                End If

                                Select Case Stato_Cod
                                    Case enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Azienda_Attivata

                                    Case enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Servizio_Impresa_Verde
                                        If _webProvisioningColdiretti IsNot Nothing Then
                                            If _webProvisioningColdiretti.NotificaQDC_IV(cuaa) Then
                                                Dim user_tipo = New gestione_utenti_permessi
                                                user_tipo.AggiornaUtentiTipologiaCambioGestioneQDC(cuaa, _UtenteTipologiaQDC_Impresa_Verde, Stato_Cod, objParametri_Super_Server, objParametri_Server, objParametri_utenti)
                                            End If

                                        End If
                                    Case enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Servizio_4_Mani
                                        If _webProvisioningColdiretti IsNot Nothing Then
                                            If _webProvisioningColdiretti.NotificaQDC_4Mani(cuaa) Then
                                                Dim user_tipo = New gestione_utenti_permessi
                                                user_tipo.AggiornaUtentiTipologiaCambioGestioneQDC(cuaa, _UtenteTipologiaQDC_4_Mani, Stato_Cod, objParametri_Super_Server, objParametri_Server, objParametri_utenti)
                                            End If
                                        End If
                                End Select
                        End Select

                        If Stato_Cod = enum_Servizi_Stati.Pratica_Validata Then
                            Dim EsisteStatoGiacenzeCompleto As DataTable =
                               LeggiPraticheStati_attuale.Leggi(
                                   Pratica_Cod,
                                   " Stato_Cod = " & enum_Servizi_Stati.Rilievo_giacenze_completato,
                                   "",
                                   objParametri_Server
                               )

                            If EsisteStatoGiacenzeCompleto.Rows.Count = 0 Then

                                NuovoPassaggioDiStato_cod =
                                        LeggiSequenze.NuovoId_Tabella(
                                         "PassaggioDiStato_cod",
                                         0,
                                         _UpperBoundTabelle,
                                         objParametri_Server
                                     )

                                scriviPraticheStati.Scrivi(
                                    Pratica_Cod,
                                    enum_Servizi_Stati.Rilievo_giacenze_completato,
                                    Servizio_Cod,
                                    Data_Riferimento,
                                    AGRODATAFINE,
                                    Note,
                                    enum_Servizi_Stati.Rilievo_giacenze_in_corso,
                                    NuovoPassaggioDiStato_cod,
                                    objParametri_Server, #2/1/1900#, #2/1/1900#, "", ""
                                )

                                scriviPraticheStati_Attuale.Modifica(
                                    Pratica_Cod,
                                    enum_Servizi_Stati.Rilievo_giacenze_completato,
                                    enum_Servizi_Stati.Rilievo_giacenze_in_corso,
                                    Note,
                                    Data_Riferimento,
                                    AGRODATAFINE,
                                    "",
                                    objParametri_Server
                                )


                            End If


                        End If

                        If Configurazione_Gruppo_str <> "" Then
                            Dim Configurazione_Gruppo = JObject.Parse(Configurazione_Gruppo_str)
                            If Configurazione_Gruppo("ConfigurazioneG2G_Local") IsNot Nothing AndAlso Configurazione_Gruppo("ConfigurazioneG2G_Local").HasValues Then
                                Dim ConfigurazioneG2G_Local = JArray.Parse(Configurazione_Gruppo("ConfigurazioneG2G_Local").ToString)
                                Dim Servizio = From p In ConfigurazioneG2G_Local Where CInt(p("Servizio_Cod")) = Servizio_Cod Select p
                                If Servizio.Count > 0 Then
                                    '(07/08/2021 fede) modificata perché non arrivano piu dal client i dati della pratica
                                    'Pratica("StatoAttuale_Cod"), Pratica("Piva"), CStr(Pratica("Rag_Soc")
                                    Dim Piva As String = ""
                                    Dim Rag_Soc As String = ""
                                    Dim DtPraticaAttuale As DataTable = LeggiPratica.LeggiXServiziStatoAttuale(WorkFlow_Cod, "", " p.pratica_cod = " & Pratica_Cod.ToString, "", objParametri_Server, True)
                                    If DtPraticaAttuale IsNot Nothing AndAlso DtPraticaAttuale.Rows.Count > 0 Then
                                        Piva = DtPraticaAttuale.Rows(0).Item("piva")
                                    End If
                                    'Dim StatoAttuale_Cod As Integer = CInt(Pratica("StatoAttuale_Cod"))
                                    Dim StatoAttuale_Cod As Integer = Stato_iniziale
                                    Dim Transizioni_Arr = JArray.Parse(Servizio(0)("Transizioni").ToString)
                                    Dim G2GLocalConfigurazioni_COD = CInt(Servizio(0)("G2GLocalConfigurazioni_COD"))
                                    Dim Transizione = From p In Transizioni_Arr Where CInt(p("Stato_A")) = Stato_Cod AndAlso CInt(p("Stato_Da")) = StatoAttuale_Cod Select p
                                    If Transizione.Count > 0 Then
                                        'InserisciAziendaG2G(G2GLocalConfigurazioni_COD, CStr(Pratica("Piva")), CStr(Pratica("Rag_Soc")), "00000000000", objParametri_Server, objParametri_utenti)
                                        InserisciAziendaG2G(G2GLocalConfigurazioni_COD, Piva, Rag_Soc, "00000000000", objParametri_Server, objParametri_utenti)
                                    End If
                                End If
                            End If
                        End If
                        '----------------------------------------------------------------------------------------------------
                        'Aggiornamenti UMA 
                        '----------------------------------------------------------------------------------------------------
                        If Servizio_Cod = enum_Servizi.Gestione_UMA Then
                            'Azzeramento assegnato quando si ritorna in compilazione
                            If StatoAttuale = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Riserva And Stato_Cod = enum_WAnagraficaStati.In_Compilazione Then
                                Dim richieste As New UMA_Richieste_Testata_W
                                richieste.Azzera_Assegnato_Su_Passaggio_Di_Stato(praticheCods.First, objParametri_Server)
                                richieste.Azzera_Confermato_Gestione_Rimanenze(praticheCods.First, objParametri_Server)
                            End If
                            'Aggiornamento rimanenze se esiste legame rendicontazione/richiesta
                            If StatoAttuale = enum_WAnagraficaStati.In_Compilazione AndAlso Stato_Cod = enum_WAnagraficaStati.Compilazione_Alla_Data_Completata_e_Verificata AndAlso praticheCods.Count = 2 Then
                                Dim richieste As New UMA_Richieste_Testata_W
                                richieste.Sincronizza_Rimanenze_Su_Passaggio_Stato(praticheCods.Last, praticheCods.First, objParametri_Server)
                            End If
                            'Generazione movimenti vendita gestione rimanenze
                            If StatoAttuale = enum_WAnagraficaStati.Verifica_In_Corso AndAlso Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo Then
                                Dim richieste As New UMA_Richieste
                                richieste.GenerazioneAutomaticaMovimentiRimanenze(Pratica_Cod, objParametri_Server)
                            End If
                            'Eliminazione movimenti vendita gestione rimanenze
                            If StatoAttuale = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo AndAlso Stato_Cod <> enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo Then
                                Dim richieste As New UMA_Richieste
                                richieste.UndoGenerazioneAutomaticaMovimentiRimanenze(Pratica_Cod, objParametri_Server)
                            End If
                        End If
                        '----------------------------------------------------------------------------------------------------
                        praticheCods = New List(Of Integer)
                    Next
                Else


                    Dim Stato_Iniziale_Cod = 0
                    Dim Stato_Finale_Cod = Stato_Cod
                    Dim Pratica_Cod = 0
                    For Each Pratica In pratiche
                        Servizio_Cod = CInt(Pratica("Servizio_Cod"))
                        Pratica_Cod = CInt(Pratica("Pratica_Cod"))
                    Next

                    'modifica
                    Dim lTransizioneDiStato_cod As Integer = PassaggioDiStato_cod
                    Dim lservizio_Cod As Integer = Servizio_Cod
                    Dim lStatoIniziale As Integer = Stato_Iniziale_Cod

                    Dim isLastState As Boolean = False

                    Dim dtStati As DataTable = leggiPraticheStati.Leggi(Pratica_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, "", " Pratiche_Stati.Validita_Inizio DESC ", objParametri_Server, 0)
                    If dtStati.Rows.Count > 0 Then
                        Dim pds_c = dtStati.Rows(0)("PassaggioDiStato_Cod")
                        If pds_c = PassaggioDiStato_cod Then
                            isLastState = True
                        End If
                    End If

                    Dim DT = leggiPraticheStati.Leggi(0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, PassaggioDiStato_cod)

                    If DT.Rows.Count > 0 Then
                        Data_Riferimento = DT.Rows(0)("Validita_inizio")
                    Else
                        Throw New Exception("Passaggio di Stato non Trovato")
                    End If

                    scriviPraticheStati.Modifica(
                        Pratica_Cod,
                        Stato_Finale_Cod,
                        0,
                        lTransizioneDiStato_cod,
                        Data_Riferimento,
                        Note,
                        "",
                        objParametri_Server
                    )

                    If isLastState Then
                        scriviPraticheStati_Attuale.ModificaNote(Pratica_Cod, Note, "", objParametri_Server)
                    End If

                    Select Case Stato_Finale_Cod
                        Case enum_Servizi_Stati.Pratica_Chiusa,
                            enum_Servizi_Stati.Pratica_Chiusa_QdC
                            ScriviPratiche.AggiornaDate(
                                    Pratica_Cod,
                                    #2/1/1900#,
                                    Data_Riferimento,
                                    "",
                                    objParametri_Server
                                )

                        Case enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__Pagante

                            Select Case lservizio_Cod
                                Case 1009
                                    Dim dtPacchetti = JArray.Parse(objLista_DSS_Selezionati)
                                    Dim idle As Integer = 0
                                    Dim pivaDTPerBlocco As DataTable =
                                        LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                                    Dim pivaPacchetto As String = pivaDTPerBlocco(0)("piva").ToString
                                    Dim pivaSuperUserPacchetto As String = pivaDTPerBlocco(0)("piva_SuperUser").ToString
                                    Dim ScriviPacchetti As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_ModelliPrevisionali_W
                                    ScriviPacchetti.CancellaPerPassaggioDiStato_Cod(lTransizioneDiStato_cod, "", objParametri_Server)

                                    For Each drP In dtPacchetti

                                        If pivaDTPerBlocco.Rows.Count > 0 Then

                                            Dim pacchettoCommercialeCod As Integer = drP("ModelliPrevisionaliRaggruppamenti_Cod")
                                            Dim Data_Scadenza_i = CDate(drP("Data_Scadenza"))
                                            Dim Data_Scadenza = New Date(Data_Scadenza_i.Year, Data_Scadenza_i.Month, Data_Scadenza_i.Day + 1)
                                            Dim lDataScadenaPacchetto As DateTime = Data_Scadenza

                                            'scrittura locale
                                            ScriviPacchetti.Scrivi(
                                                pivaSuperUserPacchetto, pivaPacchetto, lTransizioneDiStato_cod, Pratica_Cod, Stato_Finale_Cod, 0, lStatoIniziale, pacchettoCommercialeCod, lDataScadenaPacchetto, "", objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")

                                            'chiamata a provisioning web
                                            SalvaPassaggioDiStato_ChiamaProvisioning(pivaPacchetto, pivaSuperUserPacchetto, pacchettoCommercialeCod, lDataScadenaPacchetto, objParametri_Super_Server)

                                        End If
                                    Next

                            End Select

                    End Select

                End If



                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Chiudo la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                r.RispostaConferma = True
                r.RispostaOK = True
                r.Tipo = "Profilazione_PassaggioStato"

                'r.RispostaStringa = "Passaggio di Stato Completato"
                r.RispostaStringa = If(r.RispostaStringa = "", Resources.AgronicaAgenda_2010.PassaggioDiStatoCompletato, r.RispostaStringa)

            Catch ex As ColdirettiPDSException
                r.RispostaOK = False

                'uso questa funzione per ottenere il Messaggio..:
                'Lavez - 13/11/2024 - per Ticket Coldiretti #156908
                r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & ex.Message & vbCrLf & ex.InnerException.Message

                If objParametri_Server.objTransazione IsNot Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

                End If
            Catch ex As Exception

                r.RispostaOK = False

                'uso questa funzione per ottenere il Messaggio..:
                r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                    AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

                If objParametri_Server.objTransazione IsNot Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

                End If

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try
        End SyncLock
        Return r

    End Function


    Private Shared Function PassaggioDiStatoValido(Pratica_Cod As Integer,
                                                   Servizio_Cod As Integer,
                                                   Stato_Origine_Cod As Integer,
                                                   Stato_Destinazione_Cod As Integer,
                                                   ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Dim objTransizioni As New AgronicaCoreMetaSchemaDAL.WTransizioniDiStatoConfigurazione_R
        If Pratica_Cod = 0 Then
            Return True
        End If
        Dim stato_attuale As Integer = 0
        Dim dtPratiche = objPraticheStati.Leggi(Pratica_Cod, "", "", ObjParametri_Server)
        If dtPratiche.Rows.Count = 0 Then
            Return True
        End If

        stato_attuale = dtPratiche(0)("Stato_Cod")

        Dim dtTransizioni = objPratiche.Leggi_TransizioniDisponibiliDatoStato(stato_attuale, Servizio_Cod, "", "", ObjParametri_Server, AGRODATAINIZIO, AGRODATAFINE)
        If dtTransizioni IsNot Nothing AndAlso dtTransizioni.Rows.Count > 0 Then
            Dim drTransizioni = dtTransizioni.Select(" WAnagraficaStati_Cod = " & Stato_Destinazione_Cod & " ")
            If drTransizioni.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End If
        Return False


    End Function

    Private Shared Sub InviaMailChiusura(utente_Apertura As String,
                                         mail_utente_Apertura As String,
                                         oggettoMail As String,
                                         corpoMail As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim objMail As New Mail
        Dim emailDest As String = ""
        Dim configSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        'Mail Test
        Dim emailTest = configSiti.Leggi_Valore(0, "TestEmailUMA", "", "", objParametri_Server)
        If String.IsNullOrWhiteSpace(emailTest) Then
            emailTest = "{}"
        End If
        Dim objEmailTest = JsonConvert.DeserializeObject(emailTest)

        'Mail Mittente
        Dim emailFrom = configSiti.Leggi_Valore(0, "MailFrom_smtp", "", "", objParametri_Server)
        If String.IsNullOrWhiteSpace(emailFrom) Then
            emailFrom = "service@agronicagroup.it"
        End If

        'Mail Destinatario
        If String.IsNullOrWhiteSpace(objEmailTest("Mittente")) Then
            emailDest = mail_utente_Apertura
        Else
            emailDest = objEmailTest("Mittente")
            corpoMail += vbCrLf & "Mail Destinatario Originale: " & mail_utente_Apertura
        End If

        'Log Invio
        If objEmailTest("LogInvio") = 1 Then
            Dim objScriviLog As New LogProvider

            Dim nomeDirLogInvioMail = objParametri_Server.LogDirectory & "\CarburantiUMA"
            Dim nomeFileLogInvioMail = String.Format("Log_InvioMail_{0}_{1}_{2}.txt",
                                                            Trim(objParametri_Server.SuperUserUsername),
                                                            Now().ToString("yyyy"),
                                                            Now().ToString("MM"))

            Dim messaggioLog = ""
            If String.IsNullOrWhiteSpace(objEmailTest("Mittente")) Then
                messaggioLog = String.Format("Mittente:{0} - Destinatario:{1} - Oggetto:{2}",
                                                    emailFrom,
                                                    emailDest,
                                                    oggettoMail)
            Else
                messaggioLog = String.Format("Mittente:{0} - Destinatario:{1} - Oggetto:{2} - Dest.Originale:{3}",
                                                    emailFrom,
                                                    emailDest,
                                                    oggettoMail,
                                                    mail_utente_Apertura)
            End If

            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = objParametri_Server.UtenteUsername,
                .LogDirectory = nomeDirLogInvioMail,
                .LogFileName = nomeFileLogInvioMail
            }

            objScriviLog.Scrivi_LOG(objParametri_Server,
                                    "InviaMailChiusura",
                                    messaggioLog,
                                    CustomLOGParams:=customLOGParams)
        End If

        'Invio Mail
        objMail.invia(objParametri_Server,
                      emailFrom,
                      emailDest,
                      Nothing,
                      Nothing,
                      oggettoMail,
                      corpoMail,
                      False,
                      Nothing)

    End Sub

    ''' <summary>
    ''' Metodo dedicato ai controlli da eseguire per le pratiche UMA al momento del passaggio di stato da 'In compilazione' a 
    ''' 'Compilazione alla data completata e verificata' e da 'Verifica in corso' ad uno dei tre stati successivi ('Verifica intermedia completata con successo',
    ''' 'Verifica intermedia completata con riserva' o 'Verifica intermedia non superata')
    ''' </summary>
    ''' <param name="Stato_Cod"> Lo stato successivo della pratica</param>
    ''' <param name="controlli"> Sempre a true, diventa false nel momento in cui uno procede sapendo di andare in recupero accise</param>
    ''' <param name="r"> La RispostaStandard che viene popolata con eventuali errori</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_utenti"></param>
    ''' <param name="FlagConnessioneLocale"></param>
    ''' <param name="Servizio_Cod"> Codice dell'area di servizio, per l'UMA 2007</param>
    ''' <param name="leggiDocumentiObbligatori"> Riferimento a SchemaDocumenti_R per leggere e operare sui documenti obbligatori</param>
    ''' <param name="praticheCods"> Lista di pratiche che verranno avanzata all'interno di questa transazione (di solito sempre una tranne nel caso di un avanzamento contemporaneo di una rendicontazione e una richiesta dell'anno successivo)</param>
    ''' <param name="Pratica_Cod"> Il codice della pratica su cui si sta svolgendo il passaggio di stato</param>
    ''' <param name="StatoAttuale"> Lo stato attuale della pratica su cui si sta svolgendo il passaggio di stato</param>
    ''' <param name="Anno"> L'anno della pratica su cui si sta svolgendo il passaggio di stato</param>

    Private Shared Sub ControlliUMA(Stato_Cod As Integer,
                                    controlli As Boolean,
                                    ByRef r As RispostaStandard,
                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByRef objParametri_utenti As AgronicaCoreParametri,
                                    FlagConnessioneLocale As Boolean,
                                    Servizio_Cod As Integer,
                                    leggiDocumentiObbligatori As AgronicaCoreScadenziario.SchemaDocumenti_R,
                                    ByRef praticheCods As List(Of Integer),
                                    Pratica_Cod As Integer,
                                    StatoAttuale As Integer,
                                    Anno As Integer,
                                    ByRef invioMailChiusura As Boolean,
                                    ByRef oggettoMail As String,
                                    ByRef corpoMail As String,
                                    ByRef utenteApertura As String,
                                    ByRef mailUtenteApertura As String)

        Dim richieste As New UMA_Richieste_Testata_R
        Dim uma_Richieste As New UMA_Richieste_R
        Dim checkRimanenze As New UMA_Richieste
        Dim lavorazioni As New UMA_Richieste_Lavorazioni_R
        Dim imprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim vendite As New UMA_Vendite_R
        Dim setup As New UMASetup_R
        Dim umaUF As New UMA_UF
        Dim erroriCapiallevabili As String
        Dim leggiGruppi As New Gruppi_Utente_R
        Dim pratiche As New Pratiche_R
        Dim WanagraficaStato As New WAnagrafica_Stati_R
        Dim statiPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R
        Dim legaRichiestaRend As Boolean
        Dim percentualeRiduzione = 23
        Dim gestioneRimanenze = 0
        Dim statiAutorizzatiPerMail = ""
        Dim gruppiAutorizzatiPerMail = ""

        Dim dtRiduzione = setup.LeggiSetup(Anno, objParametri_Server)
        If dtRiduzione.Rows.Count > 0 Then
            percentualeRiduzione = dtRiduzione.Rows(0).Item("Per_Riduzione")
            legaRichiestaRend = dtRiduzione.Rows(0).Item("Altre_Cfg")
            gestioneRimanenze = dtRiduzione.Rows(0).Item("Gestione_Rimanenze")
            Dim objStatiAutorizzatiPerMail = dtRiduzione.Rows(0).Item("stati_invio_mail_avanz_pratica")
            statiAutorizzatiPerMail = If(IsDBNull(objStatiAutorizzatiPerMail), "", objStatiAutorizzatiPerMail)
            Dim objGruppiAutorizzatiPerMail = dtRiduzione.Rows(0).Item("gruppi_utenti_invio_mail_avanz_pratica")
            gruppiAutorizzatiPerMail = If(IsDBNull(objGruppiAutorizzatiPerMail), "", objGruppiAutorizzatiPerMail)
        End If

        Const StatoInCompilazione = "In compilazione"

        Dim warningControlloDestinatariTrasf = False

        '====================================================================================================
        'STATO DA COMPILAZIONE A COMPILAZIONE COMPLETATA
        '====================================================================================================

        If StatoAttuale = enum_WAnagraficaStati.In_Compilazione AndAlso Stato_Cod = enum_WAnagraficaStati.Compilazione_Alla_Data_Completata_e_Verificata Then

            Dim testata As DataTable = richieste.Leggi("",
                                                       0,
                                                       Pratica_Cod,
                                                       AGRODATAINIZIO,
                                                       AGRODATAFINE,
                                                       objParametri_Server,
                                                       avanzamento:=enum_UMA_Avanzamento.Rendicontazione)

            Dim testataIntegrativa As DataTable = richieste.Leggi("",
                                                                  0,
                                                                  Pratica_Cod,
                                                                  AGRODATAINIZIO,
                                                                  AGRODATAFINE,
                                                                  Anno,
                                                                  "",
                                                                  enum_UMA_Avanzamento.Richiesta,
                                                                  objParametri_Server,
                                                                  xFiltroAggiuntivo:=" Richiesta_Integrativa = 1 ")

            Dim testataAnticipo As DataTable = richieste.Leggi("",
                                                               0,
                                                               Pratica_Cod,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               objParametri_Server,
                                                               avanzamento:=enum_UMA_Avanzamento.Richiesta_Anticipo)

            Dim testataR As DataTable

            '----------------------------------------------------------------------------------------------------
            'SIAMO IN UNA RENDICONTAZIONE
            '----------------------------------------------------------------------------------------------------

            'Controlli acquistato, lavorazioni, ecc.
            If (testata.Rows.Count > 0) Then
                Dim rigaRend = testata.Rows.Item(0)

                'Verifico se esistono richieste integrative in corso, ovvero in uno stato diverso da:
                '- 2003 : Verifica completata
                '- 2005 : Verifica intermedia completata con Successo
                '- 2006 : Verifica intermedia non Superata
                '- 2008 : Inserimento Completato per il periodo di competenza
                '- 2009 : Rinuncia
                Dim esistonoIntegrativeInCorso = richieste.Leggi(rigaRend.Item("Piva"),
                                                                 0,
                                                                 0,
                                                                 AGRODATAINIZIO,
                                                                 AGRODATAFINE,
                                                                 Anno,
                                                                 "",
                                                                 enum_UMA_Avanzamento.Richiesta,
                                                                 objParametri_Server,
                                                                 isTerzista:=CInt(rigaRend.Item("Tipo_Richiesta")) = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                                 " Richiesta_Integrativa = 1 AND Pratiche_Stati_Attuali.Stato_Cod NOT IN (2003, 2005, 2006, 2008, 2009) ")

                If Not IsNothing(esistonoIntegrativeInCorso) AndAlso esistonoIntegrativeInCorso.Rows.Count > 0 Then

                    r.RispostaOK = False

                    r.RispostaConferma = False

                    Dim statoRichiestaIntegrativa = esistonoIntegrativeInCorso(0).Item("Stato_Cod")

                    Dim descrizioneStatoRichiestaIntegrativa = [Enum].GetName(GetType(enum_WAnagraficaStati), statoRichiestaIntegrativa)

                    Select Case statoRichiestaIntegrativa
                        Case enum_WAnagraficaStati.In_Compilazione
                            descrizioneStatoRichiestaIntegrativa = "In Compilazione"
                        Case enum_WAnagraficaStati.Verifica_In_Corso
                            descrizioneStatoRichiestaIntegrativa = "Verifica in corso"
                        Case enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Riserva
                            descrizioneStatoRichiestaIntegrativa = "Verifica intermedia completata con Riserva"
                        Case enum_WAnagraficaStati.Compilazione_Alla_Data_Completata_e_Verificata
                            descrizioneStatoRichiestaIntegrativa = "Compilazione alla data completata e verificata"
                    End Select

                    r.Errore = "Non è possibile completare la rendicontazione, in quanto è presente una richiesta integrativa nello stato " & descrizioneStatoRichiestaIntegrativa

                    Exit Sub

                End If

                If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then
                    If Not checkRimanenze.CalcolaRecuperoAccise(rigaRend.Item("Piva"),
                                                                rigaRend.Item("Richiesta_Cod"),
                                                                StatoInCompilazione,
                                                                rigaRend.Item("Avanzamento_Richiesta"),
                                                                objParametri_Server,
                                                                aggiornaRecuperoAccise:=False).Item1 Then
                        r.RispostaOK = False
                        r.RispostaConferma = False
                        r.Errore = "Il carburante non utilizzato dichiarato è inferiore alla somma di trasferimenti, restituzioni e rimanenze riassegnabili"
                        Exit Sub
                    End If
                End If

                'Lettura richiesta anno successivo

                testataR = richieste.Leggi(rigaRend.Item("Piva"),
                                           0,
                                           0,
                                           AGRODATAINIZIO,
                                           AGRODATAFINE,
                                           Anno + 1,
                                           "",
                                           enum_UMA_Avanzamento.Richiesta,
                                           objParametri_Server,
                                           isTerzista:=CInt(rigaRend.Item("Tipo_Richiesta")) = enum_UMA_TipoRichiesta.Conto_Terzi,
                                           " Richiesta_Integrativa = 0 AND Pratiche_Stati_Attuali.Stato_Cod IN (2001) ")

                'Ci entro solo se non sono COOPERATIVA, oppure se RINUNCIO NUOVA RICHIESTA ANNO SUCCESSIVO
                If (testataR.Rows.Count > 0 OrElse Not legaRichiestaRend) OrElse rigaRend.Item("Rinuncia_Nuova_Richiesta_Anno_Successivo") = 1 OrElse rigaRend.Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Cooperativa_Agricola Then

                    Dim ltRichiesti = rigaRend("Carburante_Richiesto_Gasolio") + rigaRend("Carburante_Richiesto_Benzina") + rigaRend("Carburante_Richiesto_Gasolio_Serra")

                    Dim dtRimanenza = richieste.Leggi_rimanenza_iniziale(rigaRend.Item("piva"),
                                                                     enum_UMA_Avanzamento.Richiesta,
                                                                     rigaRend.Item("Tipo_Richiesta"),
                                                                     Anno,
                                                                     objParametri_Server)

                    Dim acquistatoGasolio = vendite.LeggiCarburanteVenduto(rigaRend.Item("piva"),
                                                                       Anno,
                                                                       rigaRend.Item("Tipo_Richiesta"),
                                                                       " UMA_Vendite.Tipo_Carburante = 2 ",
                                                                       objParametri_Server)

                    Dim acquistatoBenzina = vendite.LeggiCarburanteVenduto(rigaRend.Item("piva"),
                                                                       Anno,
                                                                       rigaRend.Item("Tipo_Richiesta"),
                                                                       " UMA_Vendite.Tipo_Carburante = 3 ",
                                                                       objParametri_Server)

                    Dim acquistatoGasolioSerra = vendite.LeggiCarburanteVenduto(rigaRend.Item("piva"),
                                                                            Anno,
                                                                            rigaRend.Item("Tipo_Richiesta"),
                                                                            " UMA_Vendite.Tipo_Carburante = 8 ",
                                                                            objParametri_Server)

                    'Controlli è sempre TRUE, tranne se l'utente decide di proseguire in recupero accise ("Hai rendicontato meno del minimo...")

                    If (controlli) Then

                        If rigaRend("Tipo_Richiesta") = 0 Then

                            Dim terzisti = lavorazioni.Controllo_Lavorazioni_Da_Terzista_Rendicontazioni(rigaRend.Item("Piva"),
                                                                                                     Anno,
                                                                                                     "",
                                                                                                     "",
                                                                                                     objParametri_Server)

                            Dim dateRendicontazioneR As New AgronicaCoreUmaDal.UMAConfigurazioneDateRendicontazioni_R
                            Dim dtDateRendicontazione = dateRendicontazioneR.LeggiDateRendicontazioni(objParametri_Server, anno:=Anno, "Tipo_Azienda = " & enum_TipoAzienda_UMA.Azienda_Agricola_Privata)
                            Dim dataFineBloccoRendicContoProprio As Date = If(dtDateRendicontazione.Rows.Count > 0, dtDateRendicontazione.Rows(0).Item("Data_Fine_Blocco_Rendic_Conto_Proprio"), CostantiPersonalizzate.AGRODATAFINE)
                            If IsNothing(dataFineBloccoRendicContoProprio) OrElse IsDBNull(dataFineBloccoRendicContoProprio) Then
                                dataFineBloccoRendicContoProprio = CostantiPersonalizzate.AGRODATAFINE
                            End If

                            If terzisti.Rows.Count > 0 And Today < dataFineBloccoRendicContoProprio Then

                                Dim err As String = ""

                                For Each row As DataRow In terzisti.Rows

                                    err = err & row.Item("Terzisti") & ", "

                                Next

                                err = err.Remove(err.LastIndexOf(", "), 1)

                                r.RispostaOK = False

                                r.RispostaConferma = False

                                r.Errore = " I seguenti terzisti che hanno dichiarato delle lavorazioni per questa azienda hanno ancora delle rendicontazioni aperte: " & err & " "

                                Exit Sub

                            End If

                        End If

                        'Controlli macrouso biologico

                        Dim richiesteBioErrate = uma_Richieste.ControlloMacrousiBio(rigaRend.Item("Richiesta_Cod"),
                                                                                Anno,
                                                                                objParametri_Server,
                                                                                rigaRend("Tipo_Richiesta") = -1,
                                                                                True)

                        If richiesteBioErrate.Rows.Count > 0 Then
                            Dim err As String = ""

                            For Each row As DataRow In richiesteBioErrate.Rows

                                err = err & row.Item("rag_soc") & " con " & row.Item("Macrouso_UMA_Des") & " , "

                            Next

                            err = err.Remove(err.LastIndexOf(", "), 1)

                            r.RispostaOK = False

                            r.RispostaConferma = False

                            r.Errore = " I seguenti " & If(rigaRend("Tipo_Richiesta") = -1, " conti propri ", " terzisti ") &
                            " hanno specificato alcuni macrousi con un diverso regolamento (Convenzionale/Biologico): " & err & " "

                            Exit Sub

                        End If

                    End If

                    If (controlli) OrElse (gestioneRimanenze = 1) OrElse (gestioneRimanenze = 2) Then
                        Dim GiacenzaRendicontazione = 0 'record("Rimanenza_Gasolio") + record("Rimanenza_Benzina") + record("Rimanenza_Gasolio_Serra")

                        Dim rimanenzaGasolio = 0
                        Dim rimanenzaBenzina = 0
                        Dim rimanenzaGasolioSerra = 0

                        If (dtRimanenza.Rows.Count > 0) Then
                            Dim recordRimanenze = dtRimanenza.Rows.Item(0)
                            rimanenzaGasolio = recordRimanenze.Item("Rimanenza_Gasolio")
                            rimanenzaBenzina = recordRimanenze.Item("Rimanenza_Benzina")
                            rimanenzaGasolioSerra = recordRimanenze.Item("Rimanenza_Gasolio_Serra")
                        End If

                        Dim GasolioAcq = 0
                        Dim BenzinaAcq = 0
                        Dim GasolioSerraAcq = 0

                        If acquistatoGasolio.Rows.Count > 0 Then
                            GasolioAcq = acquistatoGasolio.Rows.Item(0).Item("Totale_Carb")
                        End If

                        If acquistatoBenzina.Rows.Count > 0 Then
                            BenzinaAcq = acquistatoBenzina.Rows.Item(0).Item("Totale_Carb")
                        End If

                        If acquistatoGasolioSerra.Rows.Count > 0 Then
                            GasolioSerraAcq = acquistatoGasolioSerra.Rows.Item(0).Item("Totale_Carb")
                        End If

                        Dim msgRimanenze = "rimanenza dichiarata"
                        Dim msgFinale = "I litri in difetto dovranno essere oggetto di recupero accisa."
                        Dim rimDichGasolio = rigaRend("Rimanenza_Gasolio")
                        Dim rimDichBenzina = rigaRend("Rimanenza_Benzina")
                        Dim rimDichGasolioSerra = rigaRend("Rimanenza_Gasolio_Serra")

                        'Se gestione rimanenze abilitata, considero le rimanenze dichiarate non utilizzate

                        If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then
                            rimDichGasolio = getRimDichGasolio(rigaRend)
                            rimDichBenzina = getRimDichBenzina(rigaRend)
                            rimDichGasolioSerra = getRimDichGasolioSerra(rigaRend)
                            msgRimanenze = "carburante non utilizzato dichiarato"
                            msgFinale = ""
                        End If

                        'Controllo quadradura rendicontato

                        Dim checkGasolio As Boolean = (
                        Math.Round(rigaRend("Carburante_Richiesto_Gasolio") * (100 - percentualeRiduzione) / 100) _
                        < (GasolioAcq + rimanenzaGasolio - rimDichGasolio)
                        )

                        Dim checkBenzina As Boolean = (
                        Math.Round(rigaRend("Carburante_Richiesto_Benzina") * (100 - percentualeRiduzione) / 100) _
                        < (BenzinaAcq + rimanenzaBenzina - rimDichBenzina)
                        )

                        Dim checkGasolioSerra As Boolean = (
                        Math.Round(rigaRend("Carburante_Richiesto_Gasolio_Serra") * (100 - percentualeRiduzione) / 100) _
                        < (GasolioSerraAcq + rimanenzaGasolioSerra - rimDichGasolioSerra)
                        )

                        If (checkGasolio OrElse checkBenzina OrElse checkGasolioSerra) Then

                            Dim Totale_Carb = 0
                            Dim rimanenza = 0
                            Dim Carb As String = ""

                            If checkGasolio Then
                                Totale_Carb = GasolioAcq
                                rimanenza = rimanenzaGasolio
                                GiacenzaRendicontazione = rimDichGasolio
                                ltRichiesti = rigaRend("Carburante_Richiesto_Gasolio") * (100 - percentualeRiduzione) / 100
                                Carb = getDescCarb(enum_TipoCarburante_UMA.Gasolio)
                            ElseIf checkBenzina Then
                                Totale_Carb = BenzinaAcq
                                rimanenza = rimanenzaBenzina
                                GiacenzaRendicontazione = rimDichBenzina
                                ltRichiesti = rigaRend("Carburante_Richiesto_Benzina") * (100 - percentualeRiduzione) / 100
                                Carb = getDescCarb(enum_TipoCarburante_UMA.Benzina)
                            Else
                                Totale_Carb = GasolioSerraAcq
                                rimanenza = rimanenzaGasolioSerra
                                GiacenzaRendicontazione = rimDichGasolioSerra
                                ltRichiesti = rigaRend("Carburante_Richiesto_Gasolio_Serra") * (100 - percentualeRiduzione) / 100
                                Carb = getDescCarb(enum_TipoCarburante_UMA.Gasolio_Serra)
                            End If

                            'Controllo rendicontato per passaggio allo stato completato

                            If (Not (Totale_Carb = 0 AndAlso ltRichiesti = 0 AndAlso rimanenza = 0)) Then

                                If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then
                                    'Bloccante
                                    r.RispostaOK = False
                                Else
                                    'Warning
                                    r.RispostaOK = True
                                End If

                                r.RispostaConferma = False

                                r.ParametroDue_stringa = percentualeRiduzione.ToString

                                If percentualeRiduzione = 0 Then

                                    r.Errore = String.Format("Errore, {0} rendicontato ({1}L) inferiore a: " &
                                                         "acquistato ({2}L) " &
                                                         "+ rimanenza anno precedente ({3}L) " &
                                                         "- {6} in rendicontazione ({4}L) " &
                                                         "= {5}L. " &
                                                         "{7}",
                                                         Carb,
                                                         Math.Round(ltRichiesti),
                                                         Totale_Carb,
                                                         rimanenza,
                                                         GiacenzaRendicontazione,
                                                         Totale_Carb + rimanenza - GiacenzaRendicontazione,
                                                         msgRimanenze,
                                                         msgFinale)

                                Else

                                    r.Errore = String.Format("Errore, {0} rendicontato (decurtato del {1}% : {2}L) inferiore a: " &
                                                         "acquistato ({3}L) " &
                                                         "+ rimanenza anno precedente ({4}L) " &
                                                         "- {7} in rendicontazione ({5}L) " &
                                                         "= {6}L. " &
                                                         "{8}",
                                                         Carb,
                                                         percentualeRiduzione,
                                                         Math.Round(ltRichiesti),
                                                         Totale_Carb,
                                                         rimanenza,
                                                         GiacenzaRendicontazione,
                                                         Totale_Carb + rimanenza - GiacenzaRendicontazione,
                                                         msgRimanenze,
                                                         msgFinale)

                                End If

                                'r.Errore = "Errore, " + Carb + " rendicontato (decurtato del " & percentualeRiduzione & "%:  " & Math.Round(ltRichiesti) & " L) è inferiore a: acquistato(" & Totale_Carb & "L) + rimanenza anno precedente(" & rimanenza & "L) - rimanenza dichiarata in rendicontazione(" & GiacenzaRendicontazione & "L) " &
                                '    " ( " & Totale_Carb + rimanenza - GiacenzaRendicontazione & ". I litri in difetto dovranno essere oggetto di recupero accisa ) "

                                Exit Sub

                            End If

                        End If

                    End If

                    'Controllo carburante non utilizzato rendicontazione

                    If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then

                        Dim erroreRimanenzaDichiarata = ControlloCarbNonUtilizzatoDich(r,
                                                                                       rigaRend,
                                                                                       dtRimanenza,
                                                                                       acquistatoGasolio,
                                                                                       acquistatoBenzina,
                                                                                       acquistatoGasolioSerra)
                        If erroreRimanenzaDichiarata Then
                            Exit Sub
                        End If

                    End If

                    'Controllo destinatari trasferimenti rendicontazione

                    If (controlli) Then

                        If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then

                            Dim objTrasferimenti As New UMA_Richieste_Trasferimenti_R

                            warningControlloDestinatariTrasf = ControllaDestinatariTrasferimenti(r,
                                                                                                 objParametri_Server,
                                                                                                 objParametri_utenti,
                                                                                                 richieste,
                                                                                                 rigaRend.Item("piva"),
                                                                                                 rigaRend.Item("Richiesta_Cod"),
                                                                                                 objTrasferimenti,
                                                                                                 StatoAttuale)

                            If warningControlloDestinatariTrasf Then
                                Exit Sub
                            End If

                        End If

                    End If

                    'Esite una richiesta anno+1 e NON siamo una cooperativa, faccio i controlli sulla richiesta anno+1

                    If legaRichiestaRend AndAlso testataR.Rows.Count > 0 AndAlso rigaRend.Item("Tipo_Azienda") <> enum_TipoAzienda_UMA.Cooperativa_Agricola Then

                        'Controllo validità lavorazioni per la richiesta del prossimo anno

                        Dim richiestaTestata = testataR.Rows.Item(0)

                        Dim VaLav As DataTable = richieste.CheckValiditaLavorazioniDaPratica(richiestaTestata.Item("Pratica_Cod"), "", objParametri_Server)

                        If VaLav.Rows.Count > 0 Then

                            Dim elencoLav As String = String.Empty

                            For Each lav In VaLav.Rows

                                elencoLav &= Environment.NewLine & " Lavorazione: " & lav.Item("Lav_UMA_Des") & " per il macrouso: " & lav.Item("Macrouso_UMA_Des") & " nell'azienda: " & lav.Item("rag_soc")

                            Next

                            r.RispostaOK = False

                            r.RispostaConferma = False

                            r.Errore = " Le seguenti lavorazioni nella richiesta del " & (Anno + 1).ToString & " non sono più valide alla data di inserimento: " & elencoLav

                            Exit Sub

                        End If

                        If richiestaTestata.Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Azienda_Terzista AndAlso
                        Math.Round(richiestaTestata.Item("Richiesta_Iniziale_Gasolio") + richiestaTestata.Item("Richiesta_Iniziale_Benzina") +
                          richiestaTestata.Item("Richiesta_Iniziale_Gasolio_Serra"), 0) > Math.Round(rigaRend.Item("Carburante_Richiesto_Gasolio") +
                          rigaRend.Item("Carburante_Richiesto_Benzina") + rigaRend.Item("Carburante_Richiesto_Gasolio_Serra"), 0) AndAlso
                          Not (Math.Round(Math.Round(OperazionePercentuale(richiestaTestata.Item("Richiesta_Iniziale_Gasolio"), percentualeRiduzione, True), 0) - richiestaTestata.Item("Rimanenza_Gasolio"), 0) = 0 AndAlso
                          Math.Round(Math.Round(OperazionePercentuale(richiestaTestata.Item("Richiesta_Iniziale_Benzina"), percentualeRiduzione, True), 0) - richiestaTestata.Item("Rimanenza_Benzina"), 0) = 0 AndAlso
                          Math.Round(Math.Round(OperazionePercentuale(richiestaTestata.Item("Richiesta_Iniziale_Gasolio_Serra"), percentualeRiduzione, True), 0) - richiestaTestata.Item("Rimanenza_Gasolio_Serra"), 0) = 0) Then

                            r.RispostaOK = False

                            r.RispostaConferma = False

                            r.Errore = "Nella richiesta dell'anno " & (Anno + 1).ToString & " sono presenti un numero di litri maggiori di quelli rendicontati nel " & Anno.ToString

                            Exit Sub
                        End If

                        ControlloAnticipo(richiestaTestata.Item("Richiesta_Cod"),
                                          objParametri_Server,
                                          richiestaTestata.Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                          r.Errore,
                                          richiestaTestata,
                                          0,
                                          0,
                                          0)

                        If r.Errore.Length > 0 Then
                            r.RispostaOK = False
                            r.RispostaConferma = False
                            r.Errore &= " nella richiesta dell'anno successivo"
                            Exit Sub
                        End If

                        'Controllo documenti per la richiesta del prossimo anno

                        ControlloDocumentiObbligatori(Stato_Cod,
                                                  r,
                                                  objParametri_Server,
                                                  FlagConnessioneLocale,
                                                  Servizio_Cod,
                                                  leggiDocumentiObbligatori,
                                                  richiestaTestata.Item("Pratica_Cod"),
                                                  StatoAttuale,
                                                  richiestaTestata.Item("Anno"))

                        If r.Errore.Length > 0 Then

                            r.Errore = String.Concat(r.Errore, " nella richiesta dell'anno " & (Anno + 1).ToString)

                            Exit Sub

                        End If

                        praticheCods.Add(CInt(richiestaTestata.Item("Pratica_Cod")))

                    End If

                    'Per il conto proprio, eseguo il controllo sui capi allevabili
                    If rigaRend.Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Azienda_Agricola_Privata Then

                        erroriCapiallevabili = umaUF.UF_ControlloCapiAllevabili(rigaRend.Item("Piva"),
                                                                                rigaRend.Item("Richiesta_Cod"),
                                                                                objParametri_Server)

                        If erroriCapiallevabili <> "" Then

                            r.RispostaOK = False

                            r.RispostaConferma = False

                            r.Errore = erroriCapiallevabili

                            Exit Sub

                        End If

                    End If

                Else

                    r.RispostaOK = False

                    r.RispostaConferma = False

                    r.Errore = " Per proseguire con la rendicontazione è necessario compilare la prima richiesta dell'anno " & (Anno + 1).ToString

                    Exit Sub

                End If

            Else

                '----------------------------------------------------------------------------------------------------
                'NON SIAMO IN RENDICONTAZIONE, CONTROLLO SE E' UNA RICHIESTA
                '----------------------------------------------------------------------------------------------------

                Dim richiesta As DataTable = richieste.Leggi("",
                                                             0,
                                                             Pratica_Cod,
                                                             AGRODATAINIZIO,
                                                             AGRODATAFINE,
                                                             Anno,
                                                             "",
                                                             enum_UMA_Avanzamento.Richiesta,
                                                             objParametri_Server)

                If richiesta.Rows.Count = 0 Then
                    'SE NON E CONTO PROPRIO, CERCO I CONTO TERZI
                    richiesta = richieste.Leggi("",
                                                0,
                                                Pratica_Cod,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                Anno,
                                                "",
                                                enum_UMA_Avanzamento.Richiesta,
                                                objParametri_Server,
                                                True)
                End If

                Dim richiestePrecedenti As DataTable = richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                                       0,
                                                                       0,
                                                                       AGRODATAINIZIO,
                                                                       AGRODATAFINE,
                                                                       Anno - 1,
                                                                       "",
                                                                       enum_UMA_Avanzamento.Richiesta,
                                                                       objParametri_Server,
                                                                       richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi)

                Dim redicontazioniPrecedentiAperte As DataTable = richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                                                  0,
                                                                                  0,
                                                                                  AGRODATAINIZIO,
                                                                                  AGRODATAFINE,
                                                                                  Anno - 1,
                                                                                  "",
                                                                                  enum_UMA_Avanzamento.Rendicontazione,
                                                                                  objParametri_Server,
                                                                                  richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                                                  " Pratiche_Stati_Attuali.Stato_Cod NOT IN (2003, 2005, 2008, 2009) ")

                Dim redicontazioniPrecedentiChiuse As DataTable = richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                                                  0,
                                                                                  0,
                                                                                  AGRODATAINIZIO,
                                                                                  AGRODATAFINE,
                                                                                  Anno - 1,
                                                                                  "",
                                                                                  enum_UMA_Avanzamento.Rendicontazione,
                                                                                  objParametri_Server,
                                                                                  richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                                                  " Pratiche_Stati_Attuali.Stato_Cod = 2005 ")

                Dim redicontazioniPrecedenti As DataTable = richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                                            0,
                                                                            0,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            Anno - 1,
                                                                            "",
                                                                            enum_UMA_Avanzamento.Rendicontazione,
                                                                            objParametri_Server,
                                                                            richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi)

                Dim statoRiserva = statiPratiche.Leggi(Pratica_Cod,
                                                       enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Riserva,
                                                       AGRODATAINIZIO,
                                                       AGRODATAFINE,
                                                       "",
                                                       "",
                                                       objParametri_Server,
                                                       0)

                Dim StatoRinuncia = richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                    0,
                                                    0,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    Anno,
                                                    "",
                                                    enum_UMA_Avanzamento.Richiesta,
                                                    objParametri_Server,
                                                    richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                    "Pratiche_Stati_Attuali.Stato_Cod IN (2006, 2009)",
                                                    richiesta.Rows.Item(0).Item("Tipo_Azienda"))

                Dim primaRichiesta = richiestePrecedenti.Rows.Count = 0 AndAlso redicontazioniPrecedenti.Rows.Count = 0

                If gestioneRimanenze = 1 Then
                    If Not checkRimanenze.CalcolaRecuperoAccise(richiesta.Rows.Item(0).Item("Piva"),
                                                                richiesta.Rows.Item(0).Item("Richiesta_Cod"),
                                                                StatoInCompilazione,
                                                                richiesta.Rows.Item(0).Item("Avanzamento_Richiesta"),
                                                                objParametri_Server,
                                                                aggiornaRecuperoAccise:=False).Item1 Then
                        r.RispostaOK = False
                        r.RispostaConferma = False
                        r.Errore = "Il carburante non utilizzato dichiarato è inferiore alla somma di trasferimenti e restituzioni"
                        Exit Sub
                    End If
                End If

                If Not legaRichiestaRend AndAlso redicontazioniPrecedentiChiuse.Rows.Count = 0 AndAlso richiestePrecedenti.Select("stato_cod not IN (2006, 2009)").Count <> 0 Then
                    r.RispostaOK = False
                    r.RispostaConferma = False
                    r.Errore = "Per proseguire e' necessaria la presenza di una rendicontazione approvata per l'anno precedente"
                    Exit Sub
                End If

                'In sintesi, NON ti blocca se:
                'SEI UNA COOPERATIVA
                'o se non hai MAI fatto richieste anno-1
                'o se stai facendo un anticipo
                'o se stai avanzando una richiesta integrativa
                'o se sei in compilazione proveniente da una richiesta approvata con riserva
                'o se sei in una richiesta creata a seguito di una bocciatura di una precedente richiesta
                If legaRichiestaRend AndAlso
                    testataAnticipo.Rows.Count = 0 AndAlso
                    testataIntegrativa.Rows.Count = 0 AndAlso
                    richiesta.Rows(0).Item("Tipo_Azienda") <> enum_TipoAzienda_UMA.Cooperativa_Agricola AndAlso (
                    (redicontazioniPrecedentiAperte.Rows.Count > 0 OrElse (richiestePrecedenti.Rows.Count > 0 AndAlso redicontazioniPrecedenti.Rows.Count = 0)) OrElse
                    ((statoRiserva.Rows.Count > 0 OrElse StatoRinuncia.Rows.Count > 0) AndAlso (redicontazioniPrecedentiChiuse.Rows.Count = 0 OrElse primaRichiesta))) Then
                    'CASI IN CUI L'AVANZAMENTO DELLA RICHIESTA VIENE BLOCCATO:
                    'SE NON SONO IN ANTICIPO o INTEGRATIVA
                    'e NON E' COOPERATIVA
                    'e CI SONO RENDICONTAZIONI ANNO-1 APERTE
                    '  oppure ESISTONO RICHIESTE ANNO-1 e NON ESISTONO RENDICONTAZIONI ANNO-1
                    r.RispostaOK = False
                    r.RispostaConferma = False
                    r.Errore = "L’avanzamento della richiesta verrà effettuato in automatico effettuando il passaggio di stato sulla rendicontazione " & (Anno - 1).ToString
                    Exit Sub
                End If

                Dim dtRichieste = uma_Richieste.Leggi(richiesta.Rows.Item(0).Item("Piva"),
                                                      richiesta.Rows.Item(0).Item("Richiesta_Cod"),
                                                      "",
                                                      0,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      objParametri_Server)

                If richiesta.Rows(0).Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Cooperativa_Agricola AndAlso dtRichieste.Rows.Count = 0 Then
                    r.RispostaOK = False
                    r.RispostaConferma = False
                    r.Errore = "E' necessario inserire almeno un CUAA procedere"
                    Exit Sub
                End If

                Dim anticipoGasolio = 0
                Dim anticipoBenzina = 0
                Dim anticipoGasolioSerra = 0

                ControlloAnticipo(richiesta.Rows.Item(0).Item("Richiesta_Cod"),
                                  objParametri_Server,
                                  richiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                  r.Errore,
                                  richiesta.Rows(0),
                                  anticipoGasolio,
                                  anticipoBenzina,
                                  anticipoGasolioSerra)

                If r.Errore.Length > 0 Then
                    r.RispostaOK = False
                    r.RispostaConferma = False
                    Exit Sub
                End If

                'Controllo carburante richiesto maggiore del rendicontato precedente per le prime richieste terzisti

                If richiesta.Rows.Item(0).Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Azienda_Terzista AndAlso richiesta.Rows.Item(0).Item("Richiesta_Integrativa") = 0 AndAlso redicontazioniPrecedenti.Rows.Count > 0 Then
                    Dim richiestaInizialeTerzista = Math.Round(richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Gasolio") +
                                                               richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Benzina") +
                                                               richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Gasolio_Serra"), 0)
                    Dim approvatoRendicontazioneTerzista = Math.Round(redicontazioniPrecedenti.Rows.Item(0).Item("Carburante_Approvato"), 0)
                    If richiestaInizialeTerzista > approvatoRendicontazioneTerzista AndAlso Not (
                        Math.Round(Math.Round(OperazionePercentuale(richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Gasolio"), percentualeRiduzione, True), 0) - richiesta.Rows.Item(0).Item("Rimanenza_Gasolio"), 0) = 0 AndAlso
                        Math.Round(Math.Round(OperazionePercentuale(richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Benzina"), percentualeRiduzione, True), 0) - richiesta.Rows.Item(0).Item("Rimanenza_Benzina"), 0) = 0 AndAlso
                        Math.Round(Math.Round(OperazionePercentuale(richiesta.Rows.Item(0).Item("Richiesta_Iniziale_Gasolio_Serra"), percentualeRiduzione, True), 0) - richiesta.Rows.Item(0).Item("Rimanenza_Gasolio_Serra"), 0) = 0
                        ) Then

                        r.RispostaOK = False

                        r.RispostaConferma = False

                        r.Errore = String.Format("Sono stati richiesti più litri di carburante ({0}L) rispetto a quelli rendicontati nel {1} ({2}L)",
                                                 richiestaInizialeTerzista,
                                                 Anno - 1,
                                                 approvatoRendicontazioneTerzista)

                        Exit Sub

                    End If

                End If

                'Controllo carburante non utilizzato richiesta

                If gestioneRimanenze = 1 Then

                    Dim rigaRichiesta = richiesta.Rows.Item(0)

                    Dim dtRimanenza = richieste.Leggi_rimanenza_iniziale(rigaRichiesta.Item("piva"),
                                                                         enum_UMA_Avanzamento.Richiesta,
                                                                         rigaRichiesta.Item("Tipo_Richiesta"),
                                                                         Anno,
                                                                         objParametri_Server)

                    Dim acquistatoGasolio = vendite.LeggiCarburanteVenduto(rigaRichiesta.Item("piva"),
                                                                           Anno,
                                                                           rigaRichiesta.Item("Tipo_Richiesta"),
                                                                           " UMA_Vendite.Tipo_Carburante = 2 ",
                                                                           objParametri_Server)

                    Dim acquistatoBenzina = vendite.LeggiCarburanteVenduto(rigaRichiesta.Item("piva"),
                                                                           Anno,
                                                                           rigaRichiesta.Item("Tipo_Richiesta"),
                                                                           " UMA_Vendite.Tipo_Carburante = 3 ",
                                                                           objParametri_Server)

                    Dim acquistatoGasolioSerra = vendite.LeggiCarburanteVenduto(rigaRichiesta.Item("piva"),
                                                                                Anno,
                                                                                rigaRichiesta.Item("Tipo_Richiesta"),
                                                                                " UMA_Vendite.Tipo_Carburante = 8 ",
                                                                                objParametri_Server)

                    '----------------------------------------------------------------------
                    'Se si vuole considerare l'anticipo, commentare le seguenti righe:
                    '----------------------------------------------------------------------
                    anticipoGasolio = 0
                    anticipoBenzina = 0
                    anticipoGasolioSerra = 0
                    '----------------------------------------------------------------------

                    Dim erroreRimanenzaDichiarata = ControlloCarbNonUtilizzatoDich(r,
                                                                                   rigaRichiesta,
                                                                                   dtRimanenza,
                                                                                   acquistatoGasolio,
                                                                                   acquistatoBenzina,
                                                                                   acquistatoGasolioSerra,
                                                                                   anticipoGasolio,
                                                                                   anticipoBenzina,
                                                                                   anticipoGasolioSerra)

                    If erroreRimanenzaDichiarata Then
                        Exit Sub
                    End If

                End If

                'Controllo destinatari trasferimenti richiesta

                If (controlli) Then

                    If gestioneRimanenze = 1 Then

                        Dim objTrasferimenti As New UMA_Richieste_Trasferimenti_R

                        warningControlloDestinatariTrasf = ControllaDestinatariTrasferimenti(r,
                                                                                             objParametri_Server,
                                                                                             objParametri_utenti,
                                                                                             richieste,
                                                                                             richiesta.Rows.Item(0).Item("Piva"),
                                                                                             richiesta.Rows.Item(0).Item("Richiesta_Cod"),
                                                                                             objTrasferimenti,
                                                                                             StatoAttuale)

                        If warningControlloDestinatariTrasf Then
                            Exit Sub
                        End If

                    End If

                End If

                'Per il conto proprio, eseguo il controllo sui capi allevabili
                If richiesta.Rows.Count > 0 AndAlso richiesta.Rows.Item(0).Item("Tipo_Azienda") = enum_TipoAzienda_UMA.Azienda_Agricola_Privata Then

                    erroriCapiallevabili = umaUF.UF_ControlloCapiAllevabili(richiesta.Rows.Item(0).Item("Piva"),
                                                                            richiesta.Rows.Item(0).Item("Richiesta_Cod"),
                                                                            objParametri_Server)

                    If erroriCapiallevabili <> "" Then

                        r.RispostaOK = False

                        r.RispostaConferma = False

                        r.Errore = erroriCapiallevabili

                        Exit Sub

                    End If

                End If

            End If

        End If

        '====================================================================================================
        'STATO DA COMPILAZIONE A COMPILAZIONE COMPLETATA -- CONTROLLO VALIDITA' LAVORAZIONI e CONGRUENZA CARBURANTE RICHIESTE INTEGRAZIONI
        '====================================================================================================

        If StatoAttuale = enum_WAnagraficaStati.In_Compilazione AndAlso Stato_Cod = enum_WAnagraficaStati.Compilazione_Alla_Data_Completata_e_Verificata Then

            Dim VaLav As DataTable = richieste.CheckValiditaLavorazioniDaPratica(Pratica_Cod, "", objParametri_Server)

            If VaLav.Rows.Count > 0 Then

                Dim elencoLav As String = String.Empty

                For Each lav In VaLav.Rows

                    elencoLav += Environment.NewLine & " Lavorazione: " & lav.Item("Lav_UMA_Des") & " per il macrouso: " & lav.Item("Macrouso_UMA_Des") & " nell'azienda: " & lav.Item("rag_soc")

                Next

                r.RispostaOK = False

                r.RispostaConferma = False

                r.Errore = " Le seguenti lavorazioni non sono più valide alla data di inserimento: " & elencoLav

                Exit Sub

            End If

            Dim strErrore As String = ""
            Dim testata As DataTable = richieste.Leggi("",
                                           0,
                                           Pratica_Cod,
                                           AGRODATAINIZIO,
                                           AGRODATAFINE,
                                           objParametri_Server,
                                           anno:=Anno)

            If testata.Rows.Count > 0 Then
                Dim testa = testata.Rows.Item(0)
                Dim richiestaCod = testata.Rows.Item(0).Item("Richiesta_Cod")
                Dim macrousi As DataTable = uma_Richieste.Leggi("", richiestaCod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

                If macrousi.Rows.Count > 0 Then
                    For Each macrouso In macrousi.Rows
                        Dim macrousoCod = macrouso.Item("Gruppo_Colturale_UMA")

                        Dim lav As DataTable = lavorazioni.Leggi(macrouso.Item("Piva"), macrousoCod, macrouso.Item("Programmazione_Cod"),
                                                                 testa.Item("Richiesta_Cod"), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

                        If lav.Rows.Count > 0 Then

                            For Each lavorazio As DataRow In lav.Rows

                                Dim inc As DataTable = lavorazioni.Leggi_Per_Controllo_Incrociato(macrouso.Item("Piva"),
                                                                   macrouso.Item("Programmazione_Cod"),
                                                                   testa.Item("Piva"),
                                                                   macrousoCod,
                                                                   Anno,
                                                                   "",
                                                                   "",
                                                                   objParametri_Server,
                                                                   isTerzista:=testa.Item("Tipo_Richiesta") = -1,
                                                                   testa.Item("Richiesta_Cod"),
                                                                   avanzamento:=testa.Item("Avanzamento_Richiesta"),
                                                                   integrativa:=testa.Item("Richiesta_Integrativa") = 1,
                                                                   lavorazioneUMA:=lavorazio.Item("Lavorazione_UMA"))

                                If inc.Rows.Count > 0 Then

                                    Dim superficie_Interno_Tot As Double = lav.AsEnumerable().
                                                                    Where(Function(row) row.Field(Of String)("Lavorazione_UMA") = CStr(lavorazio.Item("Lavorazione_UMA"))).
                                                                    Sum(Function(row) row.Field(Of Double)("Totale_Superficie_UMA"))

                                    Dim superficie_Esterno_Tot As Double = inc.AsEnumerable().
                                                                   Where(Function(row) row.Field(Of Integer)("Stato_Cod") = 2005).
                                                                    Sum(Function(row) row.Field(Of Double)("Totale_Superficie_UMA"))

                                    If superficie_Interno_Tot + superficie_Esterno_Tot > macrouso.Item("Totale_Superficie_UMA") * lavorazio.Item("Nr_Lavorazioni_Previste") Then

                                        If lavorazio.Item("Nr_Lavorazioni_Previste") > 1 Then

                                            Dim richiesto_interno_Tot As Double = lav.AsEnumerable().
                                                                    Where(Function(row) row.Field(Of String)("Lavorazione_UMA") = CStr(lavorazio.Item("Lavorazione_UMA"))).
                                                                    Sum(Function(row) row.Field(Of Double)("Fabbisogno_Richiesto"))

                                            Dim assegnato_esterno_Tot As Double = inc.AsEnumerable().
                                                                  Where(Function(row) row.Field(Of Integer)("Stato_Cod") = 2005).
                                                                  Sum(Function(row) row.Field(Of Double)("Fabbisogno_Assegnato"))

                                            Dim calcolato As Double = lav.AsEnumerable().
                                                                Where(Function(row) row.Field(Of Double)("Totale_Superficie_UMA") = macrouso.Item("Totale_Superficie_UMA")).
                                                                Where(Function(row) row.Field(Of String)("Lavorazione_UMA") = CStr(lavorazio.Item("Lavorazione_UMA"))).
                                                                Select(Function(row) row.Field(Of Double)("Fabbisogno_Calcolato")).
                                                                ToList().FirstOrDefault()

                                            Dim calcolatoExt As Double = inc.AsEnumerable().
                                                                Where(Function(row) row.Field(Of Double)("Totale_Superficie_UMA") = macrouso.Item("Totale_Superficie_UMA")).
                                                                Where(Function(row) row.Field(Of String)("Lavorazione_UMA") = CStr(lavorazio.Item("Lavorazione_UMA"))).
                                                                Select(Function(row) row.Field(Of Double)("Fabbisogno_Calcolato")).
                                                                ToList().FirstOrDefault()

                                            Dim maxRichiedibile As Double = Math.Max(calcolato * lavorazio.Item("Nr_Lavorazioni_Previste"), calcolatoExt * lavorazio.Item("Nr_Lavorazioni_Previste"))

                                            If (richiesto_interno_Tot + assegnato_esterno_Tot) >= maxRichiedibile Then

                                                Dim rag_soc As String = ""

                                                If testa.Item("Piva") <> macrouso.Item("Piva") Then
                                                    rag_soc = " per l'impresa: " & imprese.RagSoc_from_Piva(macrouso.Item("Piva"), objParametri_Server) & " "
                                                End If

                                                strErrore += " " & lavorazio.Item("Lav_UMA_Des") & " all'interno di: " & macrouso.Item("Macrouso_UMA_Des") & " " & rag_soc & " ; " & vbCrLf

                                            End If

                                        Else


                                            Dim assegnato_Sovrapp As Double = inc.AsEnumerable().
                                                                  Where(Function(row) row.Field(Of Integer)("Stato_Cod") = 2005).
                                                                  Sum(Function(row) row.Field(Of Double)("Fabbisogno_Assegnato"))

                                            Dim terreno_Assegnato_Sovrapp As Double = inc.AsEnumerable().
                                                                          Where(Function(row) row.Field(Of Integer)("Stato_Cod") = 2005).
                                                                          Sum(Function(row) row.Field(Of Double)("Totale_Superficie_UMA"))

                                            Dim terreno_Assegnabile_Sovrapp As Double = inc.AsEnumerable().
                                                                            Where(Function(row) row.Field(Of Integer)("Stato_Cod") = 2005).
                                                                            Sum(Function(row) row.Field(Of Double)("Totale_Richiedibile"))

                                            If (assegnato_Sovrapp + lavorazio.Item("Fabbisogno_Richiesto") > Math.Round(lavorazio.Item("Fabbisogno_Calcolato")) AndAlso
                                    terreno_Assegnato_Sovrapp = terreno_Assegnabile_Sovrapp AndAlso
                                    lavorazio.Item("Totale_Superficie_UMA") = macrouso.Item("Totale_Superficie_UMA")) Then

                                                Dim rag_soc As String = ""

                                                If testa.Item("Piva") <> macrouso.Item("Piva") Then
                                                    rag_soc = " per l'impresa: " & imprese.RagSoc_from_Piva(macrouso.Item("Piva"), objParametri_Server) & " "
                                                End If

                                                strErrore += " " & lavorazio.Item("Lav_UMA_Des") & " all'interno di: " & macrouso.Item("Macrouso_UMA_Des") & " " & rag_soc & " ; " & vbCrLf

                                            End If
                                        End If
                                    End If

                                End If

                            Next
                        End If

                    Next

                    If strErrore <> "" Then
                        r.RispostaOK = False
                        r.RispostaConferma = False

                        r.Errore = " Nelle seguenti lavorazioni è stato richiesto più carburante di quanto se ne avrebbe diritto: " & strErrore
                        Exit Sub
                    End If

                End If
            End If

        End If

        'Anna 13-08-21: aggiornamento colonna Approvatore
        If ((StatoAttuale = enum_WAnagraficaStati.Compilazione_Alla_Data_Completata_e_Verificata OrElse StatoAttuale = enum_WAnagraficaStati.Verifica_In_Corso_Da_Assegnare) AndAlso
        Stato_Cod = enum_WAnagraficaStati.Verifica_In_Corso) Then
            Dim testata As DataTable = richieste.Leggi("", 0, Pratica_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, anno:=Anno)

            If (testata.Rows.Count > 0) Then
                Dim record = testata.Rows.Item(0)
                Dim richiestaCod = record.Item("Richiesta_cod")
                Dim piva = record.Item("piva")

                Dim objApprovatore As New UMA_Richieste_Testata_W

                objApprovatore.Modifica_Campo_Richiesta("Approvatore", objParametri_utenti.UtenteUsername, piva, richiestaCod, objParametri_Server)
            End If
        End If

        '====================================================================================================
        'STATO DA VERIFICA IN CORSO A VERIFICA COMPLETATA CON SUCCESSO
        '====================================================================================================

        If StatoAttuale = enum_WAnagraficaStati.Verifica_In_Corso AndAlso Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo Then

            'ESCLUDIAMO SEMPRE LE RICHIESTE INTEGRATIVE 
            Dim testataRichiesta = richieste.Leggi("",
                                                   0,
                                                   Pratica_Cod,
                                                   AGRODATAINIZIO,
                                                   AGRODATAFINE,
                                                   Anno,
                                                   "",
                                                   enum_UMA_Avanzamento.Richiesta,
                                                   objParametri_Server,
                                                   xFiltroAggiuntivo:=" Richiesta_Integrativa = 0 ")

            If testataRichiesta.Rows.Count = 0 Then
                testataRichiesta = richieste.Leggi("",
                                                   0,
                                                   Pratica_Cod,
                                                   AGRODATAINIZIO,
                                                   AGRODATAFINE,
                                                   Anno,
                                                   "",
                                                   enum_UMA_Avanzamento.Richiesta,
                                                   objParametri_Server,
                                                   xFiltroAggiuntivo:=" Richiesta_Integrativa = 0 ",
                                                   isTerzista:=True)
            End If

            If testataRichiesta.Rows.Count > 0 Then

                Dim richiestePrecedenti As DataTable = richieste.Leggi(testataRichiesta.Rows.Item(0).Item("Piva"),
                                                                       0,
                                                                       0,
                                                                       AGRODATAINIZIO,
                                                                       AGRODATAFINE,
                                                                       Anno - 1,
                                                                       "",
                                                                       enum_UMA_Avanzamento.Richiesta,
                                                                       objParametri_Server,
                                                                       testataRichiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi)

                Dim redicontazioniPrecedentiAperte As DataTable = richieste.Leggi(testataRichiesta.Rows.Item(0).Item("Piva"),
                                                                                  0,
                                                                                  0,
                                                                                  AGRODATAINIZIO,
                                                                                  AGRODATAFINE,
                                                                                  Anno - 1,
                                                                                  "",
                                                                                  enum_UMA_Avanzamento.Rendicontazione,
                                                                                  objParametri_Server,
                                                                                  testataRichiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                                                  " Pratiche_Stati_Attuali.Stato_Cod Not In (2003, 2005, 2006, 2008, 2009)")

                Dim redicontazioniPrecedenti As DataTable = richieste.Leggi(testataRichiesta.Rows.Item(0).Item("Piva"),
                                                                            0,
                                                                            0,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            Anno - 1,
                                                                            "",
                                                                            enum_UMA_Avanzamento.Rendicontazione,
                                                                            objParametri_Server,
                                                                            testataRichiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi)

                'SE ESITONO RENDICONTAZIONI APERTE DELL'ANNO PRECEDENTE, NON PUOI APPROVARE LA RICHIESTA
                If legaRichiestaRend AndAlso (redicontazioniPrecedentiAperte.Rows.Count > 0 OrElse (richiestePrecedenti.Rows.Count > 0 AndAlso redicontazioniPrecedenti.Rows.Count = 0)) Then

                    r.RispostaOK = False

                    r.RispostaConferma = False

                    r.Errore = " Non è possibile approvare una richiesta prima della rendicontazione dell'anno precedente "

                    Exit Sub

                End If

            End If

            'Controllo quadratura dati confermati da AFOR

            If gestioneRimanenze = 1 OrElse gestioneRimanenze = 2 Then

                Dim testataRichiestaRendicontazione = richieste.Leggi("",
                                                                      0,
                                                                      Pratica_Cod,
                                                                      AGRODATAINIZIO,
                                                                      AGRODATAFINE,
                                                                      Anno,
                                                                      "",
                                                                      UMA_Richieste_Testata_R.AVANZAMENTO_RICHIESTA_FITTIZIO,
                                                                      objParametri_Server,
                                                                      xFiltroAggiuntivo:="",
                                                                      isTerzista:=False)

                If testataRichiestaRendicontazione.Rows.Count = 0 Then
                    testataRichiestaRendicontazione = richieste.Leggi("",
                                                                      0,
                                                                      Pratica_Cod,
                                                                      AGRODATAINIZIO,
                                                                      AGRODATAFINE,
                                                                      Anno,
                                                                      "",
                                                                      UMA_Richieste_Testata_R.AVANZAMENTO_RICHIESTA_FITTIZIO,
                                                                      objParametri_Server,
                                                                      xFiltroAggiuntivo:="",
                                                                      isTerzista:=True)
                End If

                If testataRichiestaRendicontazione.Rows.Count > 0 Then

                    Dim rigaTestRichRend = testataRichiestaRendicontazione.Rows(0)
                    Dim avanzamento = rigaTestRichRend.Item("Avanzamento_Richiesta")

                    If gestioneRimanenze = 1 OrElse (gestioneRimanenze = 2 And avanzamento = 1) Then

                        Dim piva = rigaTestRichRend.Item("Piva")
                        Dim richiestaCod = rigaTestRichRend.Item("Richiesta_Cod")

                        'Rimanenze dichiarate non utilizzate
                        Dim rimDichGasolio = getRimDichGasolio(rigaTestRichRend)
                        Dim rimDichBenzina = getRimDichBenzina(rigaTestRichRend)
                        Dim rimDichGasolioSerra = getRimDichGasolioSerra(rigaTestRichRend)

                        'Trasferimenti
                        Dim trasfConfGasolio = 0
                        Dim trasfConfBenzina = 0
                        Dim trasfConfGasolioSerra = 0
                        Dim objTrasferimenti As New UMA_Richieste_Trasferimenti_R
                        Dim dtTrasferimenti = objTrasferimenti.LeggiTotali(piva, richiestaCod, objParametri_Server)
                        If dtTrasferimenti.Rows.Count > 0 Then
                            trasfConfGasolio = dtTrasferimenti.Rows(0).Item("Confermato_Gasolio")
                            trasfConfBenzina = dtTrasferimenti.Rows(0).Item("Confermato_Benzina")
                            trasfConfGasolioSerra = dtTrasferimenti.Rows(0).Item("Confermato_Gasolio_Serra")
                        End If

                        'Restituzioni
                        Dim restConfGasolio = 0
                        Dim restConfBenzina = 0
                        Dim restConfGasolioSerra = 0
                        Dim objRestituzioni As New UMA_Richieste_Restituzioni_R
                        Dim dtRestituzioni = objRestituzioni.LeggiTotali(piva, richiestaCod, objParametri_Server)
                        If dtRestituzioni.Rows.Count > 0 Then
                            restConfGasolio = dtRestituzioni.Rows(0).Item("Confermato_Gasolio")
                            restConfBenzina = dtRestituzioni.Rows(0).Item("Confermato_Benzina")
                            restConfGasolioSerra = dtRestituzioni.Rows(0).Item("Confermato_Gasolio_Serra")
                        End If

                        'Rimanenze riassegnabili
                        Dim rimRiassConfGasolio = 0
                        Dim rimRiassConfBenzina = 0
                        Dim rimRiassConfGasolioSerra = 0
                        If avanzamento = 1 Then
                            rimRiassConfGasolio = getRimRiassConfGasolio(rigaTestRichRend)
                            rimRiassConfBenzina = getRimRiassConfBenzina(rigaTestRichRend)
                            rimRiassConfGasolioSerra = getRimRiassConfGasolioSerra(rigaTestRichRend)
                        End If

                        'Recupero accise
                        Dim recAccConfGasolio = getRecAccConfGasolio(rigaTestRichRend)
                        Dim recAccConfBenzina = getRecAccConfBenzina(rigaTestRichRend)
                        Dim recAccConfGasolioSerra = getRecAccConfGasolioSerra(rigaTestRichRend)

                        'Controllo quadratura

                        Dim ripartRimGasolio = trasfConfGasolio + restConfGasolio + rimRiassConfGasolio + recAccConfGasolio
                        Dim ripartRimBenzina = trasfConfBenzina + restConfBenzina + rimRiassConfBenzina + recAccConfBenzina
                        Dim ripartRimGasolioSerra = trasfConfGasolioSerra + restConfGasolioSerra + rimRiassConfGasolioSerra + recAccConfGasolioSerra

                        Dim msgCarb = ""
                        Dim rimDich = 0
                        Dim ripartRim = 0

                        If rimDichGasolio <> ripartRimGasolio Then
                            msgCarb = getDescCarb(enum_TipoCarburante_UMA.Gasolio)
                            rimDich = rimDichGasolio
                            ripartRim = ripartRimGasolio
                        ElseIf rimDichBenzina <> ripartRimBenzina Then
                            msgCarb = getDescCarb(enum_TipoCarburante_UMA.Benzina)
                            rimDich = rimDichBenzina
                            ripartRim = ripartRimBenzina
                        ElseIf rimDichGasolioSerra <> ripartRimGasolioSerra Then
                            msgCarb = getDescCarb(enum_TipoCarburante_UMA.Gasolio_Serra)
                            rimDich = rimDichGasolioSerra
                            ripartRim = ripartRimGasolioSerra
                        End If

                        If msgCarb <> "" Then
                            r.RispostaOK = False
                            r.RispostaConferma = False
                            r.Errore = String.Format("Il carburante non utilizzato dichiarato di tipo {0} ({1}L) non corrisponde con il totale confermato ({2}L)",
                                                     msgCarb, rimDich, ripartRim)
                            Exit Sub
                        End If

                        'Controllo minore assegnato

                        If rimDichGasolio > 0 OrElse rimDichBenzina > 0 OrElse rimDichGasolioSerra > 0 Then

                            Dim tipoRichiesta = rigaTestRichRend.Item("Tipo_Richiesta")

                            Dim dtRichiestoAssegnato = richieste.Leggi_Elenco_Con_Indirizzi(piva,
                                                                                            Anno,
                                                                                            "t.richiesta_cod = " & richiestaCod.ToString(),
                                                                                            "",
                                                                                            objParametri_Server,
                                                                                            objParametri_utenti,
                                                                                            False,
                                                                                            "",
                                                                                            If(avanzamento = 1, True, False),
                                                                                            0,
                                                                                            "",
                                                                                            "-1",
                                                                                            tipoRichiesta,
                                                                                            False,
                                                                                            False,
                                                                                            False,
                                                                                            0,
                                                                                            True)

                            If dtRichiestoAssegnato.Rows.Count > 0 Then
                                Dim rigaRichiestoAssegnato = dtRichiestoAssegnato.Rows(0)

                                msgCarb = ""
                                rimDich = 0
                                Dim richiestoNetto = 0
                                Dim assegnato = 0
                                Dim acquistato = 0
                                Dim rimanenzaIniziale = 0

                                Dim rimanenzaGasolio = 0
                                Dim rimanenzaBenzina = 0
                                Dim rimanenzaGasolioSerra = 0

                                Dim dtRimanenza = richieste.Leggi_rimanenza_iniziale(piva,
                                                                                     enum_UMA_Avanzamento.Richiesta,
                                                                                     tipoRichiesta,
                                                                                     Anno,
                                                                                     objParametri_Server)

                                If (dtRimanenza.Rows.Count > 0) Then
                                    Dim recordRimanenze = dtRimanenza.Rows.Item(0)
                                    rimanenzaGasolio = recordRimanenze.Item("Rimanenza_Gasolio")
                                    rimanenzaBenzina = recordRimanenze.Item("Rimanenza_Benzina")
                                    rimanenzaGasolioSerra = recordRimanenze.Item("Rimanenza_Gasolio_Serra")
                                End If

                                If rimDichGasolio > 0 Then
                                    richiestoNetto = Math.Round(rigaRichiestoAssegnato("Richiesto_Gasolio") * (100 - percentualeRiduzione) / 100)
                                    assegnato = rigaRichiestoAssegnato("assegnato_gasolio")
                                    acquistato = getAcquistato(piva, Anno, tipoRichiesta, enum_TipoCarburante_UMA.Gasolio, vendite, objParametri_Server)
                                    If richiestoNetto > assegnato AndAlso (rimanenzaGasolio + acquistato > assegnato + rimDichGasolio) Then
                                        msgCarb = getDescCarb(enum_TipoCarburante_UMA.Gasolio)
                                        rimDich = rimDichGasolio
                                        rimanenzaIniziale = rimanenzaGasolio
                                    End If
                                End If

                                If rimDichBenzina > 0 AndAlso msgCarb = "" Then
                                    richiestoNetto = Math.Round(rigaRichiestoAssegnato("Richiesto_Benzina") * (100 - percentualeRiduzione) / 100)
                                    assegnato = rigaRichiestoAssegnato("assegnato_benzina")
                                    acquistato = getAcquistato(piva, Anno, tipoRichiesta, enum_TipoCarburante_UMA.Benzina, vendite, objParametri_Server)
                                    If richiestoNetto > assegnato AndAlso (rimanenzaBenzina + acquistato > assegnato + rimDichGasolio) Then
                                        msgCarb = getDescCarb(enum_TipoCarburante_UMA.Benzina)
                                        rimDich = rimDichBenzina
                                        rimanenzaIniziale = rimanenzaBenzina
                                    End If
                                End If

                                If rimDichGasolioSerra > 0 AndAlso msgCarb = "" Then
                                    richiestoNetto = Math.Round(rigaRichiestoAssegnato("Richiesto_Gasolio_Serra") * (100 - percentualeRiduzione) / 100)
                                    assegnato = rigaRichiestoAssegnato("assegnato_gasolio_serra")
                                    acquistato = getAcquistato(piva, Anno, tipoRichiesta, enum_TipoCarburante_UMA.Gasolio_Serra, vendite, objParametri_Server)
                                    If richiestoNetto > assegnato AndAlso (rimanenzaGasolioSerra + acquistato > assegnato + rimDichGasolio) Then
                                        msgCarb = getDescCarb(enum_TipoCarburante_UMA.Gasolio_Serra)
                                        rimDich = rimDichGasolioSerra
                                        rimanenzaIniziale = rimanenzaGasolioSerra
                                    End If
                                End If

                                If msgCarb <> "" Then
                                    r.RispostaOK = False
                                    r.RispostaConferma = False
                                    r.Errore = String.Format("Il carburante non utilizzato dichiarato di tipo {0} ({1}L) va rivisto " &
                                                             "a fronte di una minore assegnazione ({2}L) rispetto a quanto richiesto ({3}L). " &
                                                             "Rimanenza iniziale + acquistato ({4}L) > assegnato + non utilizzato dichiarato ({5}L). " &
                                                             "La pratica deve essere messa in stato 'Verifica intermedia completata con Riserva'",
                                                             msgCarb,
                                                             rimDich,
                                                             Math.Round(assegnato, 0),
                                                             Math.Round(richiestoNetto, 0),
                                                             Math.Round(rimanenzaIniziale + acquistato, 0),
                                                             Math.Round(assegnato + rimDich, 0))
                                    Exit Sub
                                End If

                            End If

                        End If

                        'Controllo destinatari trasferimenti

                        If controlli AndAlso (trasfConfGasolio > 0 OrElse trasfConfBenzina > 0 OrElse trasfConfGasolioSerra > 0) Then

                            warningControlloDestinatariTrasf = ControllaDestinatariTrasferimenti(r,
                                                                                                 objParametri_Server,
                                                                                                 objParametri_utenti,
                                                                                                 richieste,
                                                                                                 piva,
                                                                                                 richiestaCod,
                                                                                                 objTrasferimenti,
                                                                                                 StatoAttuale)

                            If warningControlloDestinatariTrasf Then
                                Exit Sub
                            End If

                        End If

                    End If

                End If

            End If

        End If

        '====================================================================================================
        'STATO DA VERIFICA IN CORSO A VERIFICA NON SUPERATA / CON RISERVA
        '====================================================================================================

        Dim testataRend As DataTable = richieste.Leggi("",
                                                       0,
                                                       Pratica_Cod,
                                                       AGRODATAINIZIO,
                                                       AGRODATAFINE,
                                                       objParametri_Server,
                                                       avanzamento:=enum_UMA_Avanzamento.Rendicontazione)

        'Quando AFOR manda indietro una rendicontazione, avvisiamo che bisogna portare indietro anche eventuale richiesta collegata

        If legaRichiestaRend AndAlso testataRend.Rows.Count > 0 AndAlso testataRend.Rows(0).Item("Tipo_Azienda") <> enum_TipoAzienda_UMA.Cooperativa_Agricola AndAlso
            StatoAttuale = enum_WAnagraficaStati.Verifica_In_Corso AndAlso
            (Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Riserva OrElse
            Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata) Then

            Dim testataRichiesta = richieste.Leggi(testataRend.Rows(0).Item("Piva"),
                                                   0,
                                                   0,
                                                   AGRODATAINIZIO,
                                                   AGRODATAFINE,
                                                   Anno + 1,
                                                   "",
                                                   enum_UMA_Avanzamento.Richiesta,
                                                   objParametri_Server,
                                                   isTerzista:=CInt(testataRend.Rows(0).Item("Tipo_Richiesta")) = enum_UMA_TipoRichiesta.Conto_Terzi,
                                                   " Richiesta_Integrativa = 0 ")

            If testataRichiesta.Rows.Count > 0 Then
                r.RispostaStringa = "Passaggio di Stato Completato. <BR> <B> ATTENZIONE! </B> Per poter completare l'iter è necessario riportare lo stesso stato nella richiesta dell'anno successivo. "
            End If

        End If

        '====================================================================================================
        'BLOCCO RITORNO IN VERIFICA IN CORSO X RICHIESTE ANTICIPO
        '====================================================================================================

        If StatoAttuale = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo AndAlso Stato_Cod = enum_WAnagraficaStati.Verifica_In_Corso Then
            Dim testataRichiestaAnticipo = richieste.Leggi("",
                                                           0,
                                                           Pratica_Cod,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           objParametri_Server,
                                                           avanzamento:=enum_UMA_Avanzamento.Richiesta_Anticipo)
            If testataRichiestaAnticipo.Rows.Count > 0 Then
                r.RispostaOK = False
                r.RispostaConferma = False
                r.Errore = "Passaggio di stato non ammesso su richiesta di anticipo"
            End If
        End If

        '====================================================================================================
        ' INVIO MAIL PASSAGGIO DI STATO
        '====================================================================================================

        SePreparaInvioMailChiusura(Stato_Cod,
                                   r,
                                   objParametri_Server,
                                   objParametri_utenti,
                                   Pratica_Cod,
                                   Anno,
                                   invioMailChiusura,
                                   oggettoMail,
                                   corpoMail,
                                   utenteApertura,
                                   mailUtenteApertura,
                                   richieste,
                                   leggiGruppi,
                                   pratiche,
                                   WanagraficaStato,
                                   statiAutorizzatiPerMail,
                                   gruppiAutorizzatiPerMail)

    End Sub

    Private Shared Sub SePreparaInvioMailChiusura(Stato_Cod As Integer,
                                                  r As RispostaStandard,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_utenti As AgronicaCoreParametri,
                                                  Pratica_Cod As Integer,
                                                  Anno As Integer,
                                                  ByRef invioMailChiusura As Boolean,
                                                  ByRef oggettoMail As String,
                                                  ByRef corpoMail As String,
                                                  ByRef utenteApertura As String,
                                                  ByRef mailUtenteApertura As String,
                                                  richieste As UMA_Richieste_Testata_R,
                                                  leggiGruppi As Gruppi_Utente_R,
                                                  pratiche As Pratiche_R,
                                                  WanagraficaStato As WAnagrafica_Stati_R,
                                                  statiAutorizzatiPerMail As String,
                                                  gruppiAutorizzatiPerMail As String)

        If r.Errore = "" AndAlso statiAutorizzatiPerMail.Split("|").ToList().Contains(Stato_Cod.ToString()) Then

            Dim dtRichiesta = richieste.Leggi("",
                                              0,
                                              Pratica_Cod,
                                              AGRODATAINIZIO,
                                              AGRODATAFINE,
                                              Anno,
                                              "",
                                              UMA_Richieste_Testata_R.AVANZAMENTO_RICHIESTA_FITTIZIO,
                                              objParametri_Server,
                                              xFiltroAggiuntivo:=" Avanzamento_Richiesta <> -1",
                                              disabilitaFiltroTipoRichiesta:=True)

            Dim avanzamento As String = ""
            Dim numeroPratica As String = ""
            Dim azienda As String = ""
            Dim stato As String = ""
            Dim conto As String = ""
            Dim str As New StringBuilder With {
                .Length = 0
            }

            If dtRichiesta.Rows.Count > 0 Then
                utenteApertura = CStr(dtRichiesta.Rows.Item(0).Item("Username_Creazione"))

                Dim gruppoUtente = leggiGruppi.LeggiGruppiDaUtente(objParametri_utenti, utenteApertura)

                If gruppoUtente.Rows.Count > 0 Then

                    Dim utenteAutorizzatoRicezioneMail = gruppiAutorizzatiPerMail.Split("|").ToList().Contains(gruppoUtente.Rows.Item(0).Item("Gruppi_Utente_cod").ToString())

                    If utenteAutorizzatoRicezioneMail Then

                        mailUtenteApertura = GetMailUtente(objParametri_utenti, utenteApertura)

                        If Not String.IsNullOrWhiteSpace(mailUtenteApertura) Then

                            avanzamento = If(CInt(dtRichiesta.Rows.Item(0).Item("Avanzamento_Richiesta")) = 0, "Richiesta", "Rendicontazione")
                            conto = If(CInt(dtRichiesta.Rows.Item(0).Item("Tipo_Richiesta")) = 0, "Conto Proprio", "Conto Terzi")

                            Dim dtPratica = pratiche.Leggi_conStatoAttuale(Pratica_Cod, "", "", "", 0, 0, 0, 0, 0,
                                                   AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, 0)

                            If dtPratica.Rows.Count > 0 Then
                                numeroPratica = CStr(dtPratica.Rows.Item(0).Item("Numero"))
                                azienda = CStr(dtPratica.Rows.Item(0).Item("Rag_Soc"))
                            End If

                            Dim dtStato = WanagraficaStato.Leggi(Stato_Cod, 0, "", "", objParametri_Server)

                            If dtStato.Rows.Count > 0 Then
                                stato = CStr(dtStato.Rows.Item(0).Item("WAnagraficaStati_Des"))
                            End If

                            oggettoMail = String.Concat("Avanzamento ", avanzamento, " ", numeroPratica, "/", Anno, " - ", azienda, " - ", stato)

                            str.AppendLine("Con la presente mail si comunica l’avvenuto passaggio di stato della pratica in oggetto.")
                            str.AppendLine("")
                            str.AppendLine("Pratica: " & avanzamento & " " & numeroPratica & "/" & Anno)
                            str.AppendLine("Tipo: " & conto)
                            str.AppendLine("Azienda: " & azienda)
                            str.AppendLine("Stato Pratica: " & stato)

                            corpoMail = str.ToString()

                            invioMailChiusura = True

                        End If

                    End If

                End If

            End If

        End If

    End Sub

    Private Shared Function GetMailUtente(ByRef objParametri_utenti As AgronicaCoreParametri, utenteApertura As String) As String

        Dim emailUtente = ""

        Dim utentiDettagli As New Utenti_Dettagli_R

        Dim utenteDT = utentiDettagli.Leggi(utenteApertura,
                                            0,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri_utenti)

        If utenteDT.Rows.Count > 0 Then

            Dim emailUtenteDt = utenteDT.Rows.Item(0).Item("Email")

            If Not IsDBNull(emailUtenteDt) AndAlso Not String.IsNullOrWhiteSpace(emailUtenteDt) Then

                emailUtente = CStr(emailUtenteDt)

            End If

        End If

        Return emailUtente

    End Function

    Private Shared Function ControlloCarbNonUtilizzatoDich(r As RispostaStandard,
                                                           ByRef rigaPratica As DataRow,
                                                           dtRimanenza As DataTable,
                                                           acquistatoGasolio As DataTable,
                                                           acquistatoBenzina As DataTable,
                                                           acquistatoGasolioSerra As DataTable,
                                                           Optional anticipoGasolio As Integer = 0,
                                                           Optional anticipoBenzina As Integer = 0,
                                                           Optional anticipoGasolioSerra As Integer = 0) As Boolean

        Dim erroreRimanenzaDichiarata = False

        Dim rimDichGasolio = getRimDichGasolio(rigaPratica)
        Dim rimDichBenzina = getRimDichBenzina(rigaPratica)
        Dim rimDichGasolioSerra = getRimDichGasolioSerra(rigaPratica)

        If rimDichGasolio > 0 OrElse rimDichBenzina > 0 OrElse rimDichGasolioSerra > 0 Then

            Dim rimanenzaGasolio = 0
            Dim rimanenzaBenzina = 0
            Dim rimanenzaGasolioSerra = 0

            If (dtRimanenza.Rows.Count > 0) Then
                Dim recordRimanenze = dtRimanenza.Rows.Item(0)
                rimanenzaGasolio = recordRimanenze.Item("Rimanenza_Gasolio")
                rimanenzaBenzina = recordRimanenze.Item("Rimanenza_Benzina")
                rimanenzaGasolioSerra = recordRimanenze.Item("Rimanenza_Gasolio_Serra")
            End If

            Dim GasolioAcqAnt = 0
            Dim BenzinaAcqAnt = 0
            Dim GasolioSerraAcqAnt = 0

            Dim SeGasolioAnt = False
            Dim SeBenzinaAnt = False
            Dim SeGasolioSerraAnt = False

            If acquistatoGasolio.Rows.Count > 0 Then
                GasolioAcqAnt = acquistatoGasolio.Rows.Item(0).Item("Totale_Carb")
            End If
            If anticipoGasolio > GasolioAcqAnt Then
                GasolioAcqAnt = anticipoGasolio
                SeGasolioAnt = True
            End If

            If acquistatoBenzina.Rows.Count > 0 Then
                BenzinaAcqAnt = acquistatoBenzina.Rows.Item(0).Item("Totale_Carb")
            End If
            If anticipoBenzina > BenzinaAcqAnt Then
                BenzinaAcqAnt = anticipoBenzina
                SeBenzinaAnt = True
            End If

            If acquistatoGasolioSerra.Rows.Count > 0 Then
                GasolioSerraAcqAnt = acquistatoGasolioSerra.Rows.Item(0).Item("Totale_Carb")
            End If
            If anticipoGasolioSerra > GasolioSerraAcqAnt Then
                GasolioSerraAcqAnt = anticipoGasolioSerra
                SeGasolioSerraAnt = True
            End If

            Select Case True
                Case rimDichGasolio > (rimanenzaGasolio + GasolioAcqAnt)
                    erroreRimanenzaDichiarata = ErroreRimanenzaNonUtilizzataDichiarata(r,
                                                                                       rimDichGasolio,
                                                                                       rimanenzaGasolio,
                                                                                       GasolioAcqAnt,
                                                                                       SeGasolioAnt,
                                                                                       enum_TipoCarburante_UMA.Gasolio)
                Case rimDichBenzina > (rimanenzaBenzina + BenzinaAcqAnt)
                    erroreRimanenzaDichiarata = ErroreRimanenzaNonUtilizzataDichiarata(r,
                                                                                       rimDichBenzina,
                                                                                       rimanenzaBenzina,
                                                                                       BenzinaAcqAnt,
                                                                                       SeBenzinaAnt,
                                                                                       enum_TipoCarburante_UMA.Benzina)

                Case rimDichGasolioSerra > (rimanenzaGasolioSerra + GasolioSerraAcqAnt)
                    erroreRimanenzaDichiarata = ErroreRimanenzaNonUtilizzataDichiarata(r,
                                                                                       rimDichGasolioSerra,
                                                                                       rimanenzaGasolioSerra,
                                                                                       GasolioSerraAcqAnt,
                                                                                       SeGasolioSerraAnt,
                                                                                       enum_TipoCarburante_UMA.Gasolio_Serra)
            End Select

        End If

        Return erroreRimanenzaDichiarata

    End Function

    Private Shared Function ErroreRimanenzaNonUtilizzataDichiarata(r As RispostaStandard,
                                                                   rimDichiarata As Double,
                                                                   rimAnnoPrec As Double,
                                                                   acquistatoAnticipo As Double,
                                                                   seAnticipo As Boolean,
                                                                   tipoCarb As enum_TipoCarburante_UMA) As Boolean

        r.RispostaOK = False

        r.RispostaConferma = False

        Dim descAcqAnt = ""
        If seAnticipo Then
            descAcqAnt = "anticipo"
        Else
            descAcqAnt = "acquistato"
        End If

        r.Errore = String.Format("Errore, {0} non utilizzato dichiarato ({1}L) superiore ad " &
                                 "{2} ({3}L) " &
                                 "+ rimanenza anno precedente ({4}L) ",
                                 getDescCarb(tipoCarb),
                                 rimDichiarata,
                                 descAcqAnt,
                                 acquistatoAnticipo,
                                 rimAnnoPrec)

        Return True

    End Function

    Private Shared Function ControllaDestinatariTrasferimenti(ByRef r As RispostaStandard,
                                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                                              objParametri_utenti As AgronicaCoreParametri,
                                                              ByRef richieste As UMA_Richieste_Testata_R,
                                                              piva As Object,
                                                              richiestaCod As Object,
                                                              objTrasferimenti As UMA_Richieste_Trasferimenti_R,
                                                              StatoAttuale As Integer) As Boolean

        Dim warningControlloDestinatariTrasf = False

        Dim dtElencoTrasferimenti = objTrasferimenti.Leggi(piva, richiestaCod, objParametri_Server)

        Dim campoTrasfTipoRich = "Confermato_Tipo_Rich"
        Dim campoTrasfGasolio = "Confermato_Gasolio"
        Dim campoTrasfBenzina = "Confermato_Benzina"
        Dim campoTrasfGasolioSerra = "Confermato_Gasolio_Serra"

        If StatoAttuale = enum_WAnagraficaStati.In_Compilazione Then
            campoTrasfTipoRich = "Tipo_Richiesta"
            campoTrasfGasolio = "Gasolio"
            campoTrasfBenzina = "Benzina"
            campoTrasfGasolioSerra = "Gasolio_Serra"
        End If

        If dtElencoTrasferimenti.Rows.Count > 0 Then

            For Each rigaTrasf In dtElencoTrasferimenti.Rows

                Dim tipoRichiesta = rigaTrasf.item(campoTrasfTipoRich)
                Dim dataTrasf As Date = rigaTrasf.item("Data_Trasferimento")
                Dim annoTrasf = Year(dataTrasf)
                Dim pivaRicevente As String = rigaTrasf.item("Piva_Ricevente")
                Dim cuaaRicevente = rigaTrasf.item("CUAA")
                Dim ragsocRicevente = rigaTrasf.item("rag_soc")

                Dim objVenditaCarb As New Vendita_Carburanti(objParametri_Server, objParametri_utenti)

                Dim paramLtAcquistabili = New FiltriDettaglioLtAcquistabili()
                paramLtAcquistabili.Anno = annoTrasf
                paramLtAcquistabili.Cuaa = cuaaRicevente
                paramLtAcquistabili.PivaCliente = pivaRicevente

                'Controllo Gasolio
                If rigaTrasf.item(campoTrasfGasolio) > 0 Then
                    controllaLtAcquistabiliTrasf(objVenditaCarb,
                                                 paramLtAcquistabili,
                                                 rigaTrasf.item(campoTrasfGasolio),
                                                 tipoRichiesta,
                                                 enum_TipoCarburante_UMA.Gasolio,
                                                 ragsocRicevente,
                                                 r,
                                                 warningControlloDestinatariTrasf)
                End If

                'Controllo Benzina
                If rigaTrasf.item(campoTrasfBenzina) > 0 Then
                    controllaLtAcquistabiliTrasf(objVenditaCarb,
                                                 paramLtAcquistabili,
                                                 rigaTrasf.item(campoTrasfBenzina),
                                                 tipoRichiesta,
                                                 enum_TipoCarburante_UMA.Benzina,
                                                 ragsocRicevente,
                                                 r,
                                                 warningControlloDestinatariTrasf)
                End If

                'Controllo Gasolio Serra
                If rigaTrasf.item(campoTrasfGasolioSerra) > 0 Then
                    controllaLtAcquistabiliTrasf(objVenditaCarb,
                                                 paramLtAcquistabili,
                                                 rigaTrasf.item(campoTrasfGasolioSerra),
                                                 tipoRichiesta,
                                                 enum_TipoCarburante_UMA.Gasolio_Serra,
                                                 ragsocRicevente,
                                                 r,
                                                 warningControlloDestinatariTrasf)
                End If

                'Controllo se nell'anno del trasferimento è presente una richiesta o un anticipo
                controllaPresenzaRichiesteTrasf(richieste,
                                                pivaRicevente,
                                                cuaaRicevente,
                                                ragsocRicevente,
                                                annoTrasf,
                                                tipoRichiesta,
                                                objParametri_Server,
                                                r,
                                                warningControlloDestinatariTrasf)

            Next

        End If

        Return warningControlloDestinatariTrasf

    End Function

    Private Shared Function getAcquistato(ByVal piva As String,
                                          ByVal anno As Integer,
                                          ByVal tipoRichiesta As Integer,
                                          ByVal tipoCarb As Integer,
                                          ByRef vendite As UMA_Vendite_R,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As Double

        Dim acquistato = 0

        Dim filtroAgg = " UMA_Vendite.Tipo_Carburante = " & tipoCarb.ToString()

        Dim dtAcquistato = vendite.LeggiCarburanteVenduto(piva,
                                                          anno,
                                                          tipoRichiesta,
                                                          filtroAgg,
                                                          objParametri_Server)

        If dtAcquistato.Rows.Count > 0 Then
            acquistato = dtAcquistato.Rows.Item(0).Item("Totale_Carb")
        End If

        Return acquistato

    End Function

    Private Shared Function getDescCarb(ByVal tipoCarb As Integer) As String
        Dim descCarb = ""
        Select Case tipoCarb
            Case enum_TipoCarburante_UMA.Gasolio
                descCarb = "gasolio"
            Case enum_TipoCarburante_UMA.Benzina
                descCarb = "benzina"
            Case enum_TipoCarburante_UMA.Gasolio_Serra
                descCarb = "gasolio serra"
        End Select
        Return descCarb
    End Function

    Private Shared Sub controllaLtAcquistabiliTrasf(ByRef objVenditaCarb As Vendita_Carburanti,
                                                    ByRef paramLtAcquistabili As FiltriDettaglioLtAcquistabili,
                                                    ByVal ltTrasf As Double,
                                                    ByVal tipoRichiesta As Integer,
                                                    ByVal tipoCarb As Integer,
                                                    ByVal ragsoc As String,
                                                    ByRef r As RispostaStandard,
                                                    ByRef warningControlloDestinatariTrasf As Boolean)

        paramLtAcquistabili.Tipo_Carburante = tipoCarb

        Dim rispLtAcquistabili = objVenditaCarb.DettaglioLtAcquistabili(paramLtAcquistabili)
        Dim objLtAcq As Object = JsonConvert.DeserializeObject(rispLtAcquistabili.RispostaStringa)

        Dim ltAcquistabili = 0
        Dim descConto = ""
        Select Case tipoRichiesta
            Case enum_UMA_TipoRichiesta.Conto_Proprio
                ltAcquistabili = objLtAcq("ltAcquistabiliProprio")
                descConto = "proprio"
            Case enum_UMA_TipoRichiesta.Conto_Terzi
                ltAcquistabili = objLtAcq("ltAcquistabiliTerzi")
                descConto = "terzi"
        End Select

        If ltTrasf > ltAcquistabili Then
            Dim descCarb = getDescCarb(tipoCarb)
            r.RispostaOK = True
            r.RispostaConferma = False
            r.Errore &= String.Format("I litri {0} trasferiti ({1}L) a {2} in conto {3} sono superiori ai litri acquistabili ({4}L) per il {5}. ",
                                     descCarb,
                                     ltTrasf,
                                     ragsoc,
                                     descConto,
                                     ltAcquistabili,
                                     paramLtAcquistabili.Anno)
            warningControlloDestinatariTrasf = True
        End If

    End Sub

    Private Shared Sub controllaPresenzaRichiesteTrasf(ByRef richieste As UMA_Richieste_Testata_R,
                                                       ByVal piva As String,
                                                       ByVal cuaa As String,
                                                       ByVal ragsoc As String,
                                                       ByVal anno As Integer,
                                                       ByVal tipoRichiesta As Integer,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef r As RispostaStandard,
                                                       ByRef warningControlloDestinatariTrasf As Boolean)

        Dim testataAnt = richieste.Leggi(piva,
                                         0,
                                         0,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         objParametri_Server,
                                         isTerzista:=tipoRichiesta,
                                         anno:=anno,
                                         avanzamento:=enum_UMA_Avanzamento.Richiesta_Anticipo)

        If testataAnt.Rows.Count = 0 Then

            Dim testataRich = richieste.Leggi(piva,
                                              0,
                                              0,
                                              AGRODATAINIZIO,
                                              AGRODATAFINE,
                                              objParametri_Server,
                                              isTerzista:=tipoRichiesta,
                                              anno:=anno,
                                              avanzamento:=enum_UMA_Avanzamento.Richiesta)

            If testataRich.Rows.Count = 0 Then

                Dim descConto = ""
                Select Case tipoRichiesta
                    Case enum_UMA_TipoRichiesta.Conto_Proprio
                        descConto = "proprio"
                    Case enum_UMA_TipoRichiesta.Conto_Terzi
                        descConto = "terzi"
                End Select

                r.Errore &= String.Format("{0} non presenta richieste in conto {1} per il {2}. ",
                                          ragsoc,
                                          descConto,
                                          anno)

                warningControlloDestinatariTrasf = True

            End If

        End If

    End Sub

    ''' <summary>
    ''' Metodo di controllo UMA, verifica che i litri richiesti siano inferiori o uguali ai litri anticipati + la rimanenza dell'anno precedente
    ''' </summary>
    ''' <param name="richiesta_Cod"> Il codice della richiesta che si sta esaminando</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="isTerzista"> True se è una richiesta conto terzi (Azienda Agromeccanica o Cooperativa)</param>
    ''' <param name="MessaggioErrore"> Riferimento alla variabile usata per accumulare i messaggi di errore</param>
    Private Shared Sub ControlloAnticipo(ByVal richiesta_Cod As Integer,
                                         ByVal objParametri_Server As AgronicaCoreParametri,
                                         ByVal isTerzista As Boolean,
                                         ByRef MessaggioErrore As String,
                                         ByRef datiRichiesta As DataRow,
                                         ByRef anticipoCarburanteGasolio As Integer,
                                         ByRef anticipoCarburanteBenzina As Integer,
                                         ByRef anticipoCarburanteGasolio_Serra As Integer)

        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R

        Try

            anticipoCarburanteGasolio = 0
            anticipoCarburanteBenzina = 0
            anticipoCarburanteGasolio_Serra = 0
            Dim richiestoGasolio As Integer = 0
            Dim richiestoBenzina As Integer = 0
            Dim richiestoGasolio_Serra As Integer = 0
            Dim mancanteGasolio As Integer = 0
            Dim mancanteBenzina As Integer = 0
            Dim mancanteGasolio_Serra As Integer = 0
            Dim testata = testate.Leggi("", richiesta_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", 0, objParametri_Server, isTerzista).Rows.Item(0)

            If testata.Item("Richiesta_Integrativa") = 0 Then

                Dim anticipo = testate.Leggi(testata.Item("Piva"),
                                             0,
                                             0,
                                             AGRODATAINIZIO,
                                             AGRODATAFINE,
                                             objParametri_Server,
                                             testata.Item("Tipo_Richiesta") = -1,
                                             testata.Item("Anno"),
                                             -1)

                Dim dtSetup = setup.LeggiSetup(testata.Item("Anno"), objParametri_Server)

                Dim perc = dtSetup.Rows.Item(0).Item("Per_Riduzione")
                Dim gestioneRimanenze = dtSetup.Rows.Item(0).Item("Gestione_Rimanenze")

                If anticipo.Rows.Count > 0 Then
                    anticipoCarburanteGasolio += anticipo.Rows.Item(0).Item("Carburante_Richiesto_Gasolio")
                    anticipoCarburanteBenzina += anticipo.Rows.Item(0).Item("Carburante_Richiesto_Benzina")
                    anticipoCarburanteGasolio_Serra += anticipo.Rows.Item(0).Item("Carburante_Richiesto_Gasolio_Serra")
                End If

                If isTerzista Then

                    richiestoGasolio = testata.Item("Richiesta_Iniziale_Gasolio")
                    richiestoBenzina = testata.Item("Richiesta_Iniziale_Benzina")
                    richiestoGasolio_Serra = testata.Item("Richiesta_Iniziale_Gasolio_Serra")

                Else

                    richiestoGasolio = testata.Item("Carburante_Richiesto_Gasolio")
                    richiestoBenzina = testata.Item("Carburante_Richiesto_Benzina")
                    richiestoGasolio_Serra = testata.Item("Carburante_Richiesto_Gasolio_Serra")

                End If

                mancanteGasolio = Math.Round(anticipoCarburanteGasolio + testata.Item("Rimanenza_Gasolio") -
                               OperazionePercentuale(richiestoGasolio, perc, True), 0)

                mancanteBenzina = Math.Round(anticipoCarburanteBenzina + testata.Item("Rimanenza_Benzina") -
                               OperazionePercentuale(richiestoBenzina, perc, True), 0)

                mancanteGasolio_Serra = Math.Round(anticipoCarburanteGasolio_Serra + testata.Item("Rimanenza_Gasolio_Serra") -
                               OperazionePercentuale(richiestoGasolio_Serra, perc, True), 0)

                'In caso di gestione rimanenze abilitata, nettifico i mancanti con le rimanenze dichiarate come non utilizzate

                If gestioneRimanenze = 1 Then

                    If mancanteGasolio > 0 Then
                        Dim rimDichGasolio = getRimDichGasolio(datiRichiesta)
                        mancanteGasolio -= rimDichGasolio
                    End If

                    If mancanteBenzina > 0 Then
                        Dim rimDichBenzina = getRimDichBenzina(datiRichiesta)
                        mancanteBenzina -= rimDichBenzina
                    End If

                    If mancanteGasolio_Serra > 0 Then
                        Dim rimDichGasolioSerra = getRimDichGasolioSerra(datiRichiesta)
                        mancanteGasolio_Serra -= rimDichGasolioSerra
                    End If

                End If

                'Esiste carburante mancante

                If mancanteGasolio > 0 OrElse mancanteBenzina > 0 OrElse mancanteGasolio_Serra > 0 Then

                    Dim tipo As String = ""
                    Dim mancante As Integer = 0

                    If mancanteGasolio > 0 Then
                        tipo = getDescCarb(enum_TipoCarburante_UMA.Gasolio)
                        mancante = OperazionePercentuale(OperazionePercentuale(richiestoGasolio, perc, True) + mancanteGasolio, perc, False) - richiestoGasolio
                    ElseIf mancanteBenzina > 0 Then
                        tipo = getDescCarb(enum_TipoCarburante_UMA.Benzina)
                        mancante = OperazionePercentuale(OperazionePercentuale(richiestoBenzina, perc, True) + mancanteBenzina, perc, False) - richiestoBenzina
                    ElseIf mancanteGasolio_Serra > 0 Then
                        tipo = getDescCarb(enum_TipoCarburante_UMA.Gasolio_Serra)
                        mancante = OperazionePercentuale(OperazionePercentuale(richiestoGasolio_Serra, perc, True) + mancanteGasolio_Serra, perc, False) - richiestoGasolio_Serra
                    End If

                    MessaggioErrore = "Per procedere occorre richiedere un quantitativo di carburante coincidente o maggiore rispetto all'anticipo sommato alle rimanenze. " &
                                      "E' necessario richiedere almeno altri " & Math.Round(mancante, 0).ToString & " litri di " & tipo
                End If

            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Shared Function getRimDichGasolio(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Dich_Gasolio")), 0, rigaTestata.Item("Rim_Dich_Gasolio"))
    End Function

    Private Shared Function getRimDichBenzina(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Dich_Benzina")), 0, rigaTestata.Item("Rim_Dich_Benzina"))
    End Function

    Private Shared Function getRimDichGasolioSerra(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Dich_Gasolio_Serra")), 0, rigaTestata.Item("Rim_Dich_Gasolio_Serra"))
    End Function

    Private Shared Function getRimRiassConfGasolio(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Riass_Conf_Gasolio")), 0, rigaTestata.Item("Rim_Riass_Conf_Gasolio"))
    End Function

    Private Shared Function getRimRiassConfBenzina(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Riass_Conf_Benzina")), 0, rigaTestata.Item("Rim_Riass_Conf_Benzina"))
    End Function

    Private Shared Function getRimRiassConfGasolioSerra(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rim_Riass_Conf_Gasolio_Serra")), 0, rigaTestata.Item("Rim_Riass_Conf_Gasolio_Serra"))
    End Function

    Private Shared Function getRecAccConfGasolio(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rec_Acc_Conf_Gasolio")), 0, rigaTestata.Item("Rec_Acc_Conf_Gasolio"))
    End Function

    Private Shared Function getRecAccConfBenzina(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rec_Acc_Conf_Benzina")), 0, rigaTestata.Item("Rec_Acc_Conf_Benzina"))
    End Function

    Private Shared Function getRecAccConfGasolioSerra(ByRef rigaTestata As DataRow)
        Return IIf(IsDBNull(rigaTestata.Item("Rec_Acc_Conf_Gasolio_Serra")), 0, rigaTestata.Item("Rec_Acc_Conf_Gasolio_Serra"))
    End Function

    ''' <summary>
    ''' Funzione di supporto per svolgere rapidamente queste due operazioni:
    ''' - Sottrazione di una percentuale da un numero (100 - il 23% di esso)
    ''' - Trovare il numero che corrisponde al 100% sapendo che il numero indicato corrisponde al (100 - percentuale) % (Ho 100 e so che è il 77%, devo trovare il 100%)
    ''' </summary>
    ''' <param name="base"> Numero di partenza per il calcolo</param>
    ''' <param name="percentuale"> Numero compreso tra 0 e 100</param>
    ''' <param name="sottrai"> True nel caso si desideri l'operazione 1, false altrimenti</param>
    ''' <returns></returns>
    Private Shared Function OperazionePercentuale(ByVal base As Integer, ByVal percentuale As Integer, ByVal sottrai As Boolean) As Double
        Return IIf(percentuale > 0, base * IIf(sottrai, (100 - percentuale) / 100, 100 / (100 - percentuale)), base)
    End Function

    ''' <summary>
    ''' Metodo dedicato al controllo sulla presenza dei documenti obbligatori per far avanzare la pratica selezionata allo stato desiderato
    ''' </summary>
    ''' <param name="Stato_Cod"> Lo stato successivo della pratica</param>
    ''' <param name="r"> La RispostaStandard che viene popolata con eventuali errori</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="FlagConnessioneLocale"></param>
    ''' <param name="Servizio_Cod"> Codice dell'area di servizio</param>
    ''' <param name="leggiDocumentiObbligatori"> Riferimento a SchemaDocumenti_R per leggere e operare sui documenti obbligatori</param>
    ''' <param name="Pratica_Cod"> Il codice della pratica su cui si sta svolgendo il passaggio di stato</param>
    ''' <param name="StatoAttuale"> Lo stato attuale della pratica su cui si sta svolgendo il passaggio di stato</param>
    ''' <param name="Anno"> L'anno della pratica su cui si sta svolgendo il passaggio di stato</param>
    Private Shared Sub ControlloDocumentiObbligatori(Stato_Cod As Integer,
                                                          r As RispostaStandard,
                                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                                          FlagConnessioneLocale As Boolean,
                                                          Servizio_Cod As Integer,
                                                          leggiDocumentiObbligatori As AgronicaCoreScadenziario.SchemaDocumenti_R,
                                                          Pratica_Cod As Integer,
                                                          StatoAttuale As Integer,
                                                          Anno As Integer)

        Dim documentiObbligatori = leggiDocumentiObbligatori.CheckDocumentiObbligatori(Pratica_Cod, Servizio_Cod, Anno, StatoAttuale, Stato_Cod, "", "sdt.ID_Schema_Template", objParametri_Server)
        Dim docDict As New Dictionary(Of Integer, Boolean)
        If (documentiObbligatori.Rows.Count > 0) Then
            Dim actId As Integer = documentiObbligatori.Rows.Item(0).Item("Id_Schema_Template")
            For Each doc As DataRow In documentiObbligatori.Rows
                If (actId <> doc.Item("Id_Schema_Template") AndAlso Not docDict.ContainsKey(actId)) Then
                    r.RispostaOK = False

                    r.Errore = "Errore, mancano dei documenti obbligatori "

                    Return
                Else
                    actId = doc.Item("Id_Schema_Template")
                    If (Not docDict.ContainsKey(actId)) Then
                        If (Not IsDBNull(doc.Item("Pratica_Cod")) AndAlso doc.Item("Pratica_Cod") = Pratica_Cod) Then
                            docDict.Add(actId, True)
                        End If
                    End If
                End If
            Next

            If (Not docDict.ContainsKey(actId)) Then
                r.RispostaOK = False

                r.Errore = "Errore, mancano dei documenti obbligatori "

            End If

        End If
    End Sub

    Public Shared Sub InserisciAziendaG2G(G2GLocalConfigurazioni_COD As Integer, Piva As String, Rag_Soc As String, Padre As String, ObjParametri_Server As AgronicaCoreParametri, ObjParametri_Utenti As AgronicaCoreParametri)
        Dim G2GLocal_R As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim G2GLocal_W As New AgronicaCoreG2GLocalDal.G2GLocal_W

        Dim dt_G2G = G2GLocal_R.LeggiConfigurazioni(G2GLocalConfigurazioni_COD, ObjParametri_Server)

        If dt_G2G.Rows.Count > 0 Then
            Dim configurazione = CStr(dt_G2G.Rows(0)("G2GLocalConfigurazioni_CFG"))
            Dim des = CStr(dt_G2G.Rows(0)("G2GLocalConfigurazioni_DES"))
            Dim xml = XDocument.Parse(configurazione)
            Dim xElemCfg As XElement = xml.<dati>.First
            Dim imprese = xElemCfg.<imprese>.First
            Dim impresa = imprese.<filtronerisultato_azienda>.FirstOrDefault.Value
            If String.IsNullOrEmpty(impresa) Then
                impresa = "{ ""tipo"": ""azienda"", ""chiavi"": [] }"
            End If
            Dim objFiltrone = JObject.Parse(impresa)
            Dim jAziende = JArray.Parse(objFiltrone("chiavi").ToString)
            Dim esisteImpresa = (From p In jAziende Where p = Piva).ToArray.Count
            If esisteImpresa = 0 Then
                jAziende.Add(Piva)
                objFiltrone("chiavi") = jAziende
                'Dim result = XDocUtils.IniettaSottoAlberoDaStringaXml(configurazione, impresaEl.ToString, "//*[local-name()='dati']/*[local-name()='imprese']", "")
                xml.<dati>.First.<imprese>.FirstOrDefault.<filtronerisultato_azienda>.First.Value = objFiltrone.ToString
                For Each node In xml.Root.Descendants
                    If node.Name.NamespaceName = "" Then
                        node.Attributes("xmlns").Remove
                        node.Name = node.Parent.Name.Namespace + node.Name.LocalName
                    End If
                Next
                G2GLocal_W.Modifica(G2GLocalConfigurazioni_COD, des, xml.ToString, "", ObjParametri_Server)
            End If
        End If
    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaStato(Pratica_Cod As String,
                                         Stato_Cod As Integer,
                                         Data_Riferimento_str As String,
                                         Note As String,
                                         WorkFlow_Cod As Integer,
                                         PassaggioDiStato_Cod As Integer,
                                         Servizio_Cod As Integer,
                                         Stato_Origine As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim LeggiSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim LeggiPratica As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim ScriviPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_W
            Dim scriviPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
            Dim scriviPraticheStati_Attuale As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
            Dim LeggiPraticheStati_attuale As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R


            Dim MessaggioErrore As String = ""
            Dim Data_Riferimento = CDate(Data_Riferimento_str)

            Dim pratiche_arr_String As New List(Of String) From {
                CStr(Pratica_Cod)
            }
            VerificaEvantualiDateNonCoerenti(MessaggioErrore, Data_Riferimento, pratiche_arr_String.ToArray, Stato_Cod, objParametri_Server)

            If MessaggioErrore <> "" Then
                r.RispostaConferma = False
                r.RispostaOK = True
                r.RispostaStringa = MessaggioErrore
                Return r
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'Dim Stato_iniziale = StatoDatoWorkflow(WorkFlow_Cod, "WanagraficaStati_COD", objParametri_Server)

            Dim lTransizioneDiStato_cod As Integer = PassaggioDiStato_Cod
            Dim lservizio_Cod As Integer = Servizio_Cod
            Dim lStatoIniziale As Integer = Stato_Origine
            Dim lstato_A_cod As Integer = Stato_Cod

            scriviPraticheStati.Modifica(
                        Pratica_Cod,
                        lstato_A_cod,
                        0,
                        lTransizioneDiStato_cod,
                        Data_Riferimento,
                        Note,
                        "",
                        objParametri_Server
                    )

            Select Case lstato_A_cod
                Case enum_Servizi_Stati.Pratica_Chiusa,
                            enum_Servizi_Stati.Pratica_Chiusa_QdC
                    ScriviPratiche.AggiornaDate(
                                    Pratica_Cod,
                                    #2/1/1900#,
                                    Data_Riferimento,
                                    "",
                                    objParametri_Server
                                )

                Case enum_WWorflow_WAnagraficaStati.Servizi_Agronica_2017_Attivo__Pagante

                    Select Case lservizio_Cod
                        Case 1009
                            Dim dtPacchetti As DataTable = HttpContext.Current.Session("Servizi_Lista_dtPacchetti")
                            Dim idle As Integer = 0
                            Dim pivaDTPerBlocco As DataTable =
                                        LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)

                            Dim pivaPacchetto As String = pivaDTPerBlocco(0)("piva").ToString
                            Dim pivaSuperUserPacchetto As String = pivaDTPerBlocco(0)("piva_SuperUser").ToString
                            Dim ScriviPacchetti As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_ModelliPrevisionali_W
                            ScriviPacchetti.CancellaPerPassaggioDiStato_Cod(lTransizioneDiStato_cod, "", objParametri_Server)

                            For Each drP In dtPacchetti.Rows

                                If pivaDTPerBlocco.Rows.Count > 0 Then

                                    Dim pacchettoCommercialeCod As Integer = drP("ModelliPrevisionali_Gruppi_COD")
                                    Dim lDataScadenaPacchetto As DateTime = drP("Scadenza")

                                    'scrittura locale
                                    ScriviPacchetti.Scrivi(
                                                pivaSuperUserPacchetto, pivaPacchetto, lTransizioneDiStato_cod, Pratica_Cod, lstato_A_cod, 0, lStatoIniziale, pacchettoCommercialeCod, lDataScadenaPacchetto, "", objParametri_Server, #2/1/1900#, #2/1/1900#, "", "")

                                    'chiamata a provisioning web
                                    SalvaPassaggioDiStato_ChiamaProvisioning(pivaPacchetto, pivaSuperUserPacchetto, pacchettoCommercialeCod, lDataScadenaPacchetto, objParametri_Super_Server)

                                End If
                            Next

                    End Select

            End Select

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            r.RispostaConferma = True
            r.RispostaOK = True
            r.RispostaStringa = "Passaggio di Stato Completato"

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDSS_PassaggioStato(PassaggioDiStato_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim UtenteAbilitato_Provisioning_R As Boolean
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Provisioning_R = objPermessi.Controlla_Permessi_Utente(
                                       HttpContext.Current.Session("ASG_Utente_Username"),
                                        HttpContext.Current.Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Provisioning_agronica,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_utenti)
            Dim dtPacchetti As New DataTable
            If UtenteAbilitato_Provisioning_R Then

                dtPacchetti = DSS_btn_Aggiungi_GetDataTable(0, PassaggioDiStato_Cod, objParametri_Server)

            End If

            Dim str_Res As String = ""

            str_Res = Newtonsoft.Json.JsonConvert.SerializeObject(dtPacchetti)

            r.RispostaOK = True
            r.RispostaStringa = str_Res
        Catch ex As Exception

        End Try

        Return r

    End Function

    Private Shared Function DSS_btn_Aggiungi_GetDataTable(ByVal Pratica_Cod As Integer, ByVal Passaggio_Di_Stato_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim DSS As New DataTable()

        DSS.Columns.Add(New DataColumn("ModelliPrevisionaliRaggruppamenti_Cod", GetType(Integer)))
        DSS.Columns.Add(New DataColumn("ModelliPrevisionaliRaggruppamenti_Des", GetType(String)))
        DSS.Columns.Add(New DataColumn("Data_Scadenza", GetType(Date)))

        Dim xLetturaPacchetti As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_ModelliPrevisionali_R
        Dim dtPacchetti = xLetturaPacchetti.Leggi(Pratica_Cod, 0, Passaggio_Di_Stato_Cod, "", "", objParametri_Server)

        Dim DtKeys(8) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = dtPacchetti.Columns("Piva_SuperUser")
        DtKeys(1) = dtPacchetti.Columns("Piva")
        DtKeys(2) = dtPacchetti.Columns("PassaggioDiStato_cod")
        DtKeys(3) = dtPacchetti.Columns("Pratica_Cod")
        DtKeys(4) = dtPacchetti.Columns("Stato_Cod")
        DtKeys(5) = dtPacchetti.Columns("Mod_Cod")
        DtKeys(6) = dtPacchetti.Columns("Stato_Origine_Cod")
        DtKeys(7) = dtPacchetti.Columns("ModelliPrevisionali_Gruppi_COD")


        'Assegno il vettore delle chiavi al DataTable
        dtPacchetti.PrimaryKey = DtKeys

        Dim xWS As New AgronicaCoreWebService.Meteo

        Dim ss As String = xWS.ModelliPrevisionaliRaggruppamenti_Lista(objParametri_Server)

        Dim jss = New JavaScriptSerializer()
        ss = ss.Replace("{""ModelliPrevisionaliRaggruppamenti_ElencoResult"":", "")
        ss = ss.Substring(0, ss.Length - 1)
        Dim rs As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelliPrevisionaliRaggruppamenti)) =
                    jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelliPrevisionaliRaggruppamenti)))(ss)

        For Each dRowPac As DataRow In dtPacchetti.Rows
            Dim row = DSS.NewRow
            Dim curValue As String = dRowPac("ModelliPrevisionali_Gruppi_COD")
            row("ModelliPrevisionaliRaggruppamenti_Cod") = dRowPac("ModelliPrevisionali_Gruppi_COD")

            Dim DesPacchetto As String = ""
            For Each modello In rs.RispostaStringa
                If modello.ModelliPrevisionaliRaggruppamenti_Cod = curValue Then
                    DesPacchetto = modello.ModelliPrevisionaliRaggruppamenti_Des
                    Exit For
                End If
            Next

            row("ModelliPrevisionaliRaggruppamenti_Des") = DesPacchetto
            row("Data_Scadenza") = dRowPac("Scadenza")

            DSS.Rows.Add(row)

        Next

        Return DSS
    End Function

    Private Shared Sub VerificaEvantualiDateNonCoerenti(ByRef MessaggioErrore As String, ByVal DataRiferimentoTransizioneDiStato As Date, ByRef ArrayPratiche As String(), ByVal lStato As Integer, objParametri_Server As AgronicaCoreParametri)

        Dim i As Integer

        Dim pratiche_filter = ""

        For i = 0 To ArrayPratiche.Length - 1
            pratiche_filter &= ArrayPratiche(i)
            If i <> ArrayPratiche.Length - 1 Then
                pratiche_filter &= ", "
            End If
        Next

        Dim FiltroPratiche As String = " Pratiche.Pratica_Cod IN (" & pratiche_filter & ")"
        Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim Dt_Pratiche As DataTable
        Dt_Pratiche = ObjPratiche.Leggi_conStoricoTransizioniDiStato(0,
                               "", "", "",
                               0, 0, 0,
                               0,
                               0,
                               AGRODATAINIZIO, AGRODATAFINE,
                               FiltroPratiche,
                               " Pratiche_Stati.Validita_Inizio desc, Rag_Soc, Servizio_Des, Ordine ",
                               objParametri_Server)

        Dim DataInizioCompetenza As Date
        Dim DataFineCompetenza As Date
        Dim DataApertura As Date
        Dim DrPratica As DataRow()
        Dim DrApertura As DataRow()
        Dim DrValidazione As DataRow()
        Dim Servizio_Des As String
        Dim RagSoc As String

        'ArrayPratiche = Split(condPratiche, ",")

        Dim lStatoXverifica As Integer = -1
        Select Case lStato
            Case enum_Servizi_Stati.Pratica_Aperta
            Case enum_Servizi_Stati.Pratica_Validata
                lStatoXverifica = enum_Servizi_Stati.Pratica_Aperta
            Case enum_Servizi_Stati.Pratica_Validata_QdC
                lStatoXverifica = enum_Servizi_Stati.Pratica_Aperta_QdC
            Case enum_Servizi_Stati.Pratica_Chiusa
                lStatoXverifica = 0
            Case enum_Servizi_Stati.Pratica_Chiusa_QdC
                lStatoXverifica = 0
        End Select

        If lStatoXverifica = -1 Then
            MessaggioErrore = ""
            Exit Sub
        End If

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'CONTROLLO COERENZA DATE
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If DataRiferimentoTransizioneDiStato <> AGRODATAINIZIO Then
            If ArrayPratiche IsNot Nothing AndAlso ArrayPratiche.Length > 0 Then
                For i = 0 To ArrayPratiche.Length - 1
                    If ArrayPratiche(i) <> "" AndAlso ArrayPratiche(i) <> "0" Then
                        DrPratica = Dt_Pratiche.Select("Pratica_Cod=" & ArrayPratiche(i))
                        If DrPratica IsNot Nothing AndAlso DrPratica.Length > 0 Then
                            RagSoc = DrPratica(0).Item("rag_soc")
                            Servizio_Des = DrPratica(0).Item("Servizio_Des")
                            DataInizioCompetenza = DrPratica(0).Item("validita_inizio")
                            DataFineCompetenza = DrPratica(0).Item("validita_fine")


                            DrApertura = Dt_Pratiche.Select("Pratica_Cod=" & ArrayPratiche(i) & " AND Stato_Cod=" & enum_Servizi_Stati.Pratica_Aperta)
                            If DrApertura IsNot Nothing AndAlso DrApertura.Length > 0 Then
                                DataApertura = DrApertura(0).Item("Validita_Inizio_Stato")
                            End If

                            'verifiche su tutti gli stati
                            If (DataRiferimentoTransizioneDiStato > DataFineCompetenza OrElse DataRiferimentoTransizioneDiStato < DataInizioCompetenza) Then
                                MessaggioErrore &= "Non è possibile inserire la data per il servizio " & Servizio_Des & " per l'impresa " & RagSoc & " poiché la data selezionata è esterna all'intervallo di competenza del servizio!" & vbCrLf
                            End If

                            If (DataRiferimentoTransizioneDiStato < DataApertura) Then
                                MessaggioErrore &= "Non è possibile inserire la data per il servizio " & Servizio_Des & " per l'impresa " & RagSoc & " poiché la data selezionata è antecedente alla data di apertura del servizio!" & vbCrLf
                            End If


                            If lStato = enum_Servizi_Stati.Pratica_Chiusa Then
                                Dim DataValidazione As Date
                                DrValidazione = Dt_Pratiche.Select("Pratica_Cod=" & ArrayPratiche(i) & " AND Stato_Cod=" & enum_Servizi_Stati.Pratica_Validata)
                                If DrValidazione IsNot Nothing AndAlso DrValidazione.Length > 0 Then
                                    DataValidazione = DrValidazione(0).Item("Validita_Inizio_Stato")
                                End If
                                If DataRiferimentoTransizioneDiStato <> AGRODATAINIZIO Then
                                    If (DataRiferimentoTransizioneDiStato < DataValidazione) Then
                                        MessaggioErrore &= "Non è possibile inserire la data di chiusura del servizio " & Servizio_Des & " per l'impresa " & RagSoc & "poichè la data selezionata è antecedente alla data di validazione del servizio!" & vbCrLf
                                    End If
                                End If
                            End If

                            If lStato = enum_Servizi_Stati.Pratica_Chiusa_QdC Then
                                Dim DataValidazione As Date
                                DrValidazione = Dt_Pratiche.Select("Pratica_Cod=" & ArrayPratiche(i) & " AND Stato_Cod=" & enum_Servizi_Stati.Pratica_Validata_QdC)
                                If DrValidazione IsNot Nothing AndAlso DrValidazione.Length > 0 Then
                                    DataValidazione = DrValidazione(0).Item("Validita_Inizio_Stato")
                                End If
                                If DataRiferimentoTransizioneDiStato <> AGRODATAINIZIO Then
                                    If (DataRiferimentoTransizioneDiStato < DataValidazione) Then
                                        MessaggioErrore &= "Non è possibile inserire la data di chiusura del servizio " & Servizio_Des & " per l'impresa " & RagSoc & "poichè la data selezionata è antecedente alla data di validazione del servizio!" & vbCrLf
                                    End If
                                End If
                            End If

                        End If

                    End If
                Next
            End If
        End If
    End Sub

    Private Shared Function StatoDatoWorkflow(ByVal workFlow_Cod As Integer, ByVal Colonna As String, objParametri_Server As AgronicaCoreParametri) As String
        Dim dt As DataTable =
            PredisponiPerPassaggioDiStato("", objParametri_Server)
        Return (
            From d In dt.AsEnumerable
            Where d("WorkFlow_Cod") = workFlow_Cod
            Select CStr(d(Colonna))).FirstOrDefault

    End Function

    Private Shared Function PredisponiPerPassaggioDiStato(ByVal FiltroPratiche As String, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim rval As DataTable
        If HttpContext.Current.Session("dtStatiPerPassaggio") Is Nothing Then

            Dim LeggiStatiPerPassaggio As New AgronicaCoreProfilazioneDAL.Pratiche_R
            rval = LeggiStatiPerPassaggio.Leggi_StatiPerPassaggioDiStato(
                FiltroPratiche,
                "",
                "",
                objParametri_Server
                )
            HttpContext.Current.Session("dtStatiPerPassaggio") = rval
        Else
            rval = HttpContext.Current.Session("dtStatiPerPassaggio")
        End If

        Return rval

    End Function

    Private Shared Sub SalvaPassaggioDiStato_ChiamaProvisioning(ByVal piva As String, ByVal piva_superUser As String, ByVal PacchettoCommerciale_Cod As Integer, ByVal scadenza As DateTime, objParametri_Super_Server As AgronicaCoreParametri)

        Dim s2017 As New AgronicaCoreProfilazioneBIZ.Servizi2017

        'con i dati di Piva e cuaa inizializzo.
        Dim xRichiestaWS_Post As String = ""

        xRichiestaWS_Post = s2017.ProvisioningDSSClientGeneraRichiesta(piva, piva_superUser, PacchettoCommerciale_Cod, scadenza)

        Dim risultatoChiamataAttivazione As String =
                        ChiamataGenericaWebApiProfilatore("/provisioningDSS", xRichiestaWS_Post, objParametri_Super_Server)

        Dim risultatoChiamataAttivazioneElaborato As RispostaStandard =
                        s2017.ProvisioningDSSResponseElaboraRispostaStandard(risultatoChiamataAttivazione)

        If Not risultatoChiamataAttivazioneElaborato.RispostaOK Then
            Throw New Exception(risultatoChiamataAttivazioneElaborato.Errore)
        End If


    End Sub

    Private Shared Function ChiamataGenericaWebApiProfilatore(ByVal metodo As String, ByVal PostDataRichiesta As String, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim hlpHttp As New AgronicaCoreUtility.Http
        Dim rval As String = ""

        Dim LetturaConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim url As String = LetturaConfig.Leggi_Valore(0, "GiasOnline_WS_AgronicaWebApiProfilatore", "", "", objParametri)
        Dim token As String = LetturaConfig.Leggi_Valore(0, "AgronicaWebApiProfilatore_Token", "", "", objParametri)


        Return ChiamataGenerica(PostDataRichiesta, url & metodo, "bearer: " & token)

    End Function

    Private Shared Function ChiamataGenerica(ByVal PostDataRichiesta As String, ByVal url As String, ByVal auth As String) As String

        Dim hlpHttp As New AgronicaCoreUtility.Http
        Dim rval As String = ""

        Dim hdr As New System.Net.WebHeaderCollection
        If auth <> "" Then
            hdr = New System.Net.WebHeaderCollection From {
                {"Authorization", auth}
            }
        End If

        rval = hlpHttp.chiamaWS(PostDataRichiesta, Nothing, url, "application/json", "POST", "application/json", "", hdr)

        Return rval

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EsportazioneZespri() As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriSincronizzatore_2010 With {
                .Pagina_Richiesta = enum_PagineAgronicaSincro.EsportazioneZESPRI
            }


            Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(
                           Enum_SiteRedirector.Sito_AgronicaPlanning,
                           objGiasOnline
                           )

            r.RispostaOK = True
            r.RispostaStringa = strOpen
        Catch ex As Exception

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EsportazioneRegione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim ObjPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dim RagSoc As String = ""
            Dim Piva As String = ""
            Dim Cuaa As String = ""
            Dim Servizio_Cod As Integer = 0
            Dim Stato_Cod As Integer = 0
            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE
            Dim FiltroStato As String = ""
            Dim Filtro As String = ""
            Dim FiltroImprese As String = ""


            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------


            Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_DPI") = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            Dim GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
            HttpContext.Current.Session("WS_FITO") = GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
            Dim ddl As New DropDownList
            x.IAF_Elenco(ddl, False, "", "", HttpContext.Current.Session,
                         objParametri_Server, objParametri_utenti, 0, 0,
                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = False, .Flag_DisciplinareAttivo = True})

            objParametri_Server.ResettaFinestra()



            'Filtro = GetFiltro(RagSoc, Piva, Cuaa, Servizio_Cod, Stato_Cod, DataInizio, DataFine, FiltroStato, FiltroImprese, False)

            'Filtro = GetFiltroRegione(RagSoc, Piva, Cuaa, Servizio_Cod, Stato_Cod, DataInizio, DataFine, FiltroStato, FiltroImprese, False)
            Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim DTProfilo As DataTable
            Dim Sql_Permessi As String = ""


            DTProfilo = objProfilo.Leggi(
                HttpContext.Current.Session("ASG_Utente_Username"),
                CInt(HttpContext.Current.Session("ASG_IdServizio")),
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_utenti
            )



            If DTProfilo.Rows.Count > 0 Then
                Sql_Permessi = DTProfilo.Rows(0).Item("Descrizione_2") & ""
            End If
            '----------------------------------------------------------------

            Dim Filtro_Visibilita_Utente = True
            If Sql_Permessi = "" Then
                Filtro_Visibilita_Utente = False
            End If

            Dim Dt_Pratiche As New DataTable
            Dt_Pratiche = ObjPratiche.Leggi_conDatiImpianti_FiltroUtente(0, "",
                               RagSoc, Piva, Cuaa,
                               0, 0, 0,
                               Servizio_Cod,
                               Stato_Cod,
                               DataInizio, DataFine,
                               Filtro_Visibilita_Utente,
                               Filtro,
                               " Rag_Soc, Servizio_Des, Ordine ",
                               objParametri_Server,
                               objParametri_utenti)

            Dim dtServiziLista As New DataTable
            Dim drServiziLista As DataRow

            dtServiziLista.Columns.Add(New DataColumn("ANNO", GetType(Integer)))
            dtServiziLista.Columns.Add(New DataColumn("STACP", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Tecnico Istruttore", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Ragione Sociale", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Cuaa", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Specie", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Varietà", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Norma di Riferimento", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Impegni aggiuntivi (IAF)", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Esito", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Stato Verifica", GetType(String)))
            dtServiziLista.Columns.Add(New DataColumn("Descrizione anomalie", GetType(String)))

            For Each drPratiche In Dt_Pratiche.Rows
                drServiziLista = dtServiziLista.NewRow

                drServiziLista.Item("ANNO") = drPratiche.Item("ANNO")
                drServiziLista.Item("STACP") = drPratiche.Item("Impresa Padre")
                drServiziLista.Item("Tecnico Istruttore") = drPratiche.Item("Utente")
                drServiziLista.Item("Ragione Sociale") = drPratiche.Item("Rag_Soc")
                drServiziLista.Item("Cuaa") = "'" & drPratiche.Item("CUAA")

                drServiziLista.Item("Specie") = "'" & drPratiche.Item("Veg_des")
                drServiziLista.Item("Varietà") = "'" & drPratiche.Item("Cul_Des")
                'drServiziLista.Item("Norma di Riferimento") = "'" & drPratiche.Item("Norma di Riferimento")
                'drServiziLista.Item("Impegni aggiuntivi (IAF)") = "'" & drPratiche.Item("Impegni aggiuntivi (IAF)")
                'drServiziLista.Item("Esito") = "'" & drPratiche.Item("Esito")
                drServiziLista.Item("Stato Verifica") = "'" & drPratiche.Item("WAnagraficaStati_Des")
                drServiziLista.Item("Descrizione anomalie") = "'" & drPratiche.Item("note")
                If Not IsDBNull(drPratiche.Item("CodiciIAF")) Then
                    Dim strIAF = CStr(drPratiche.Item("CodiciIAF"))
                    Dim resIAF As String = ""
                    For Each IAF In strIAF.Split("|")
                        If IAF <> "" Then
                            resIAF += ddl.Items.FindByValue(IAF).Text & System.Environment.NewLine
                        End If
                    Next
                    drServiziLista.Item("Impegni aggiuntivi (IAF)") = resIAF
                End If

                dtServiziLista.Rows.Add(drServiziLista)

            Next

            Dim gw As New GridView With {
                .DataSource = dtServiziLista,
                .AutoGenerateColumns = True
            }
            gw.DataBind()


            HttpContext.Current.Session("GridView_xls") = gw
            HttpContext.Current.Session("Nome_xls") = "ListaServizi"

            r.RispostaOK = True
            r.RispostaStringa = "../excel/Esporta_XSL_AllGridview.aspx"
        Catch ex As Exception

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CambiaServizio(Pratica_Cod As Integer, Servizio_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            If objParametri_Server.UtenteUsername <> objParametri_Server.SuperUserUsername Then
                r.RispostaOK = False
                r.Errore = "Bisogna essere superuser"
                Return r
            End If

            Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_W

            objPratiche.AggiornaServizio(Pratica_Cod, Servizio_Cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = "OK!"
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function isSuperUser() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim boolSuper As Boolean = False

            If objParametri_Server.UtenteUsername = objParametri_Server.SuperUserUsername Then
                boolSuper = True
            End If

            r.RispostaOK = True
            If boolSuper Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "false"
            End If

        Catch ex As Exception

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function gestionePassaggioDiStato(Pratica_Cod As Integer, Passaggio_Di_Stato_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaStringa = "PassaggioDiStato.aspx?pratica=" & CStr(Pratica_Cod) & "&passaggiodistato=" & CStr(Passaggio_Di_Stato_Cod)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiDatiPratica(Pratica_Cod)
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objProfilazione_R As New AgronicaCoreProfilazioneDAL.Pratiche_R

            Dim DTPratiche = objProfilazione_R.Leggi_FiltroUtente(Pratica_Cod,
                                                 "", "", "",
                                                 0, 0, 0, 0, 0,
                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                 "", "", False, objParametri_Server, objParametri_utenti,
                                                 False)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(DTPratiche, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiPassaggioDiStato(PassaggioDiStato_Cod)
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objProfilazione_R As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_R

            Dim DTPratiche = objProfilazione_R.Leggi(0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, PassaggioDiStato_Cod)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(DTPratiche, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    '''
    ''' <summary>
    ''' Dato un servizio ACA, restituisce una tupla composta da tipoCod e contributoCod, chiavi della tabella di metaschema Contributi
    ''' </summary>
    ''' <param name="servizio">Servizio ACA</param>
    ''' <returns>Tuple(tipoCod, contributoCod)</returns>
    '''
    Private Shared Function AssociataServizioADomandaACA(servizio As enum_Servizi) As Tuple(Of Integer, Integer)

        Dim tipoCod = 0
        Dim contributoCod = 0
        Select Case servizio
            Case enum_Servizi.ACA_2
                tipoCod = 1
                contributoCod = 2
            Case enum_Servizi.ACA_4
                tipoCod = 1
                contributoCod = 4
            Case enum_Servizi.ACA_12
                tipoCod = 1
                contributoCod = 12
            Case enum_Servizi.ACA_13
                tipoCod = 1
                contributoCod = 13
            Case enum_Servizi.ACA_24_01 ' azione 1
                tipoCod = 1
                contributoCod = 24
            Case enum_Servizi.ACA_24_02 ' azione 2
                tipoCod = 1
                contributoCod = 25
        End Select


        Return New Tuple(Of Integer, Integer)(tipoCod, contributoCod)

    End Function


    Private Shared Function FiltraStatiDestinazione(ByVal Servizio_Cod As Integer,
                                                    ByVal Pratica_Cod As Integer,
                                                    ByVal dtStati As DataTable,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri
                                                    ) As DataTable
        Dim dt As DataTable = dtStati.Clone()
        Dim LeggiPratica As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim xUtenti_Dettagli_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

        Select Case Servizio_Cod
            Case enum_Servizi.QDemetraQdCBluarancio
                For Each row In dtStati.Rows
                    'Lo stato gestione a cura azienda deve essere selezionabile solo se è presente l'utente a sistema
                    'nel caso di impresa verde non è vincolante
                    If row("WAnagraficaStati_Cod") = enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Servizio_4_Mani Then
                        Dim cuaa As String = ""
                        Dim pivaDT As DataTable =
                                    LeggiPratica.Leggi_2(Pratica_Cod, "", "", 0, 0, 0, Servizio_Cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, False)
                        If pivaDT.Rows.Count > 0 Then
                            cuaa = pivaDT.Rows(0)("CUAA")
                        Else
                            Throw New Exception(String.Format("Pratica ({0}) non trovata", Pratica_Cod))
                        End If

                        Dim dt1 = xUtenti_Dettagli_R.Leggi("", 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Utenti_Dettagli.CodFisc='" & cuaa & "' ", "", objParametri_Utenti)
                        If dt1.Rows.Count > 0 Then
                            dt.ImportRow(row)
                        Else
                            'skip stato
                        End If
                    Else
                        dt.ImportRow(row)
                    End If
                Next
            Case Else
                For Each row As DataRow In dtStati.Rows
                    dt.ImportRow(row)
                Next

        End Select

        Return dt
    End Function

End Class

Public Class MaxDate
    Public maxDataInizio As Date
    Public minDataFine As Date
End Class