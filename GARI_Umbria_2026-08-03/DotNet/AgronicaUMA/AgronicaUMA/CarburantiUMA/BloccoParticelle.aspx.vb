

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

Public Class BloccoParticelle
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

#Region "script services Carica Griglia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Cerca_Dettagli_Azienda(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim ind_des As String = ""
        Dim com_des As String = ""
        Dim CUAA As String = ""
        Dim pro_cod As String = ""
        Dim comDesIstat As String = ""
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                leggiIndirizzo.Indirizzo_from_Piva(piva, "", ind_des, "", "", com_des, "",
                                      pro_cod, comDesIstat, objParametri_Server)

                CUAA = leggiCUAA.Leggi_CUAA(piva, objParametri_Server)
            End If
            'Dt.Columns.Add(New DataColumn("indDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("comDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("CUAA", GetType(String)))
            Dt.Columns.Add(New DataColumn("pro_cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("comDesIstat", GetType(String)))

            Dim d0 As DataRow
            d0 = Dt.NewRow
            'd0("indDes") = ind_des
            d0("comDes") = com_des
            d0("CUAA") = CUAA
            d0("pro_cod") = pro_cod
            d0("comDesIstat") = comDesIstat
            Dt.Rows.Add(d0)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaPivaReale(ByVal piva As String) As RispostaStandard


        Dim r As New RispostaStandard
        Dim PartitaIvaReale As String = ""
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                PartitaIvaReale = leggiPivaReale.Leggi_PivaReale(piva, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(PartitaIvaReale, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Blocchi(ByVal piva As String,
                                         ByVal citta As String,
                                         ByVal prov As String,
                                         ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim dtgroup As New DataTable

        Dim uma_Richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim EFrichieste As New AgronicaCoreUmaBiz.UMA_Richieste
        Dim dtRichieste As New DataTable

        Dim dtTable_Pendenza As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objBlocco As New AgronicaCoreUmaDal.UMA_Blocco_Particelle_R
        Dim bloccoBiz As New AgronicaCoreUmaBiz.UMA_Blocco_Particelle

        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Bloccato", GetType(Boolean)))
        'dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If piva = "-1" Then
                piva = ""
            End If

            Dim FiltroUtente As Boolean = False
            Dim FiltroGruppo As Boolean = False
            Dim Gruppo As Integer = 0
            Dim VisibilitaTotale As Boolean = False

            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objParametri_Utenti, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

            Dim dt As DataTable = objBlocco.Leggi(piva, anno, "", "", objParametri_Server)

            If dt.Rows.Count <= 0 Then

                If anno < 2025 Then
                    dtTable_Pendenza = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(0, objParametri_Server, " t.piva like '" & piva & "' ", particelle:=True)

                    dtTable_Pendenza.DefaultView.Sort = "Programmazione_Cod DESC, Prov ASC, com ASC, macrouso_UMA_Cod DESC"
                    dtTable_Pendenza = dtTable_Pendenza.DefaultView.ToTable
                Else

                    dtTable_Pendenza = uma_Richieste.TrovaColtureAppezzamentiSenzaFascicolo(piva, 0, objParametri_Server, True, anno:=anno)
                    dtTable_Pendenza.DefaultView.Sort = "Prov ASC, com ASC, macrouso_UMA_Cod DESC"
                    dtTable_Pendenza = dtTable_Pendenza.DefaultView.ToTable

                End If

                bloccoBiz.Inserisci_Da_DataTable(piva, anno, dtTable_Pendenza, objParametri_Server)
                dt = objBlocco.Leggi(piva, anno, "", "", objParametri_Server)

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Blocchi_UMA(ByVal piva As String,
                                                ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim blocchi As New AgronicaCoreUmaBiz.UMA_Blocco_Particelle

            blocchi.Aggiorna_Blocchi_Da_Griglia(piva, righeModificate, objParametri_Server)

            r.RispostaStringa = 1

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RiempiCitta(ByVal prov As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim istat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Dt As DataTable
        Dim DtRes As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        DtRes.Columns.Add(New DataColumn("proCom", GetType(String)))
        DtRes.Columns.Add(New DataColumn("citta", GetType(String)))

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = istat.Leggi(prov, "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, " PROV NOT LIKE '%E%' ", " PROV ", objParametri_Server)

            For Each row As DataRow In Dt.Rows

                Dim d0 As DataRow = DtRes.NewRow()
                If (prov IsNot "") Then
                    d0.Item("proCom") = row.Item("COM")
                Else
                    d0.Item("proCom") = row.Item("LOCALITA")
                End If
                d0.Item("citta") = row.Item("LOCALITA")
                DtRes.Rows.Add(d0)

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(DtRes, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RiempiProv() As RispostaStandard

        Dim r As New RispostaStandard
        Dim istat As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim Dt As DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = istat.GetProvince(objParametri_Server, "IT")

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

#Region "Edit"

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditRichiesta(ByVal chiave As String,
                                         ByVal richiestaCod As Integer) As RispostaStandard
        Dim objParametriAgenda = New ParametriAgenda

        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaUma
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_UMA_Elenco
        'objParametriAgenda.Piva = chiave
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica
        Dim risp As New RispostaStandard
        Dim queryString As New With {Key .piva = Stringa_Codifica(chiave, AgroKey_EncoderDecoder, Nothing).ToString,
                                         .richiestaCod = Stringa_Codifica(richiestaCod, AgroKey_EncoderDecoder, Nothing).ToString,
                                         .fromPage = enum_PagineAgenda_2010.Pagina_UMA_Elenco}
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        risp.RispostaOK = True
        risp.RispostaStringa = JsonConvert.SerializeObject(queryString, Formatting.None, serializerSettings)

        Return risp
    End Function

#End Region

#Region "Delete"

    <WebMethod(EnableSession:=True)>
    Public Shared Function DeleteRichiesta(ByVal Piva As String,
                                           ByVal richiestaCod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim richieste As New AgronicaCoreUmaBiz.UMA_Richieste

            r.RispostaStringa = richieste.EliminaRichiesta(Piva, richiestaCod, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Rinuncia"

    <WebMethod(EnableSession:=True)>
    Public Shared Function RinunciaRichiesta(ByVal Piva As String,
                                             ByVal Cuaa As String,
                                             ByVal richiestaCod As String,
                                             ByVal pratica_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim pratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W

            r = pratiche.impostaPratica(0,
                                        Piva,
                                        Cuaa,
                                        objParametri_Server.UtenteUsername,
                                        enum_Servizi.Gestione_UMA,
                                        2009,
                                        objParametri_Server,
                                        objParametri_Utenti, 0,
                                        "", pratica_cod,
                                        False, "",
                                        AGRODATAINIZIO,
                                        AGRODATAFINE, 0, 0)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Stampa"

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaRichiesta(ByVal piva As String,
                                           ByVal richiestaAvanz As Integer,
                                           ByVal richiestaCod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If richiestaAvanz = 1 Then
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RendicontazioneCarb
        Else
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RichiestaCarbPrevisioneLav
        End If

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim vVarStampe(1) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = piva

            vVarStampe(1).Nome = "richiesta_cod"
            vVarStampe(1).Valore = richiestaCod


            Dim objVS As New AgronicaCoreXML.XML_Stampe
            Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
            Dim StrNodiVariabili As String = StrNodo

            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            Dim link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                    Enum_SiteRedirector.Sito_AgronicaUma, objAgronicaStampe)



            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Caricamento"

    Private Sub caricaControlli()

        'Select Case objParametriAgenda.Tipo_Operazione

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
        '        RipristinaControlliDaAgenda()

        'End Select
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPerc(ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R
        Dim Dt As DataTable
        Dim perc As Integer

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = setup.LeggiSetup(anno, objParametri_Server)

            If Dt.Rows.Count > 0 Then
                perc = Dt.Rows.Item(0).Item("Per_Riduzione")
            Else
                perc = 23
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(perc, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        inizializzoParametriPagina()


        Me.Master.flag_MostraBtnIndietro = False

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0

        QS_Avanzamento = 0
        If Not IsNothing(Request.QueryString("avanzamento")) Then
            QS_Avanzamento = CInt(Request.QueryString("avanzamento"))
        End If

        If Not IsNothing(Request.QueryString("p")) Then
            QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
        End If

        Dim enumAppro = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Approvazione_Richiesta_UMA, enum_Security_Attivita.Approvazione_Rendicontazione_UMA)
        Dim enumBase = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Richiesta_UMA, enum_Security_Attivita.Rendicontazione_UMA)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumAppro,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        If Not UtenteAbilitatoLettura Then
            UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enumAppro,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If Not UtenteAbilitatoScrittura Then
            UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

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


    Private Sub inizializzoParametriPagina()


    End Sub


    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

End Class