Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMetaSchemaDAL
Imports NUglify.Helpers
Imports DocumentFormat.OpenXml.Drawing.Diagrams
Imports DocumentFormat.OpenXml.Office2016.Excel
Imports System.Web.UI.WebControls.Expressions
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG

Public Class ReportQdC
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Private Sub inizializzoParametriPagina()

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'Master().Lbl_Titolo.Text = "Analisi dati commerciali"

        inizializzoObjParametri()

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        hdPreferiti.Value = True
        hdLingua.Value = 1

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim qsType = Request.QueryString("type")

        'Report Campagna

        'Controllo se l'utente ha i permessi per accedere

        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                                      Session("ASG_IdServizio"),
                                                                                      enum_Security_Attivita.ReportCampagna,
                                                                                      enum_Security_Operazione.Lettura,
                                                                                      Date.Now,
                                                                                      "",
                                                                                      objParametri_Utenti)

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hdType.Value = qsType

        'Attivo Gestione Cubo
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Report_Vendite_Cubo", "", "", objParametri_Server)

        hdLingua.Value = objParametri_Server.Lingua_Cod

        If objParametriAgenda.Piva <> "" Then
            hdPiva.Value = objParametriAgenda.Piva
        End If

        If Not IsPostBack Then
            inizializzoParametriPagina()
        End If

    End Sub

#Region "script services Carica Griglia Report Vendite"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaReportQdC(ByVal piva_Aziende As List(Of String),
                                                  ByVal centri_Aziendali As List(Of String),
                                                  ByVal specie As List(Of Integer),
                                                  ByVal dataInizioOp As String, ByVal dataFineOp As String, ByVal periodoDataOperazioni As Integer,
                                                  ByVal dataInizioImp As String, ByVal dataFineImp As String, ByVal periodoData As Integer,
                                                  ByVal tipoOperazioni As List(Of Integer), ByVal operazioni As List(Of Integer),
                                                  ByVal nazioni As List(Of String), ByVal regioni As List(Of String),
                                                  ByVal province As List(Of String), ByVal comuni As List(Of String), ByVal impianti As Boolean,
                                                  ByVal prodotti As Boolean, ByVal operazioniColt As Boolean,
                                                  ByVal estrazione As Integer, ByVal aziende_referenti As List(Of String), ByVal datiTecnici As Boolean
            ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            Dim attivitaStatistiche As New AgronicaCoreContabBIZ.AgendaStatistiche

            Dim dati = attivitaStatistiche.CaricaOperazioniStatistichePivot(piva_Aziende, centri_Aziendali,
                                                                            specie, dataInizioOp, dataFineOp, periodoDataOperazioni,
                                                                            dataInizioImp, dataFineImp, periodoData,
                                                                            tipoOperazioni, operazioni,
                                                                            nazioni, regioni, province,
                                                                            comuni, impianti,
                                                                            prodotti, operazioniColt,
                                                                            estrazione, aziende_referenti, datiTecnici,
                                                                            objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dati, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaCuboReportVendite(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim statistiche As New Statistiche_W
            Dim risposta = statistiche.Aggiorna_Cubo_Report_Vendite(piva, dataDal, dataAl, objParametri_Server)

            r.RispostaStringa = "Procedura eseguita correttamente"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Parametri_Qualitativi(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New Statistiche_R
            Dim dtParamQual = leggi.Leggi_Parametri_Qualitativi(piva, objParametri_Server)


            Dim parametro As String = ""
            Dim campo As String = ""
            Dim tabellaDes As String = ""
            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtParamQual.Rows
                If dr.Item("Tipo") = 1 OrElse
                    dr.Item("Tipo") = 3 OrElse
                    dr.Item("Tipo") = 4 OrElse
                    dr.Item("Tipo") = 5 Then

                    parametro = CStr(dr.Item("Tabella_Key"))

                    If parametro <> "" Then
                        tabellaDes = CStr(dr.Item("Tabella_Des"))

                        If dr.Item("Tipo") = 1 Then
                            campo = parametro & "_Descrizione"
                        End If
                        If dr.Item("Tipo") = 3 OrElse
                            dr.Item("Tipo") = 4 OrElse
                            dr.Item("Tipo") = 5 Then
                            campo = parametro & "_Val_Cod"
                        End If

                        'Dim nome As String = parametro.Substring(0, 1).ToUpper() + parametro.Substring(1)
                        jArrayParamQual.Add(New JObject(New JProperty("campo", campo), New JProperty("nome", tabellaDes), New JProperty("tipo", dr.Item("Tipo"))))
                    End If

                End If
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Magazzino() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCM As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim dtCategMag = objCM.Leggi(0, "", False, " Elem_Cod NOT IN (1,4,300) ", "", objParametri_Server)

            Dim jArrayCategorie As New JArray()
            For Each dr As DataRow In dtCategMag.Rows
                jArrayCategorie.Add(New JObject(New JProperty("Elem_Cod", CStr(dr.Item("Elem_Cod"))), New JProperty("Elem_Des", dr.Item("NomeComune"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCategorie, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Commerciali(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCC As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R
            Dim dtCategCommle = objCC.Leggi(piva, 0, "", "", objParametri_Server)

            Dim jArrayCategorieComm As New JArray()
            For Each dr As DataRow In dtCategCommle.Rows
                jArrayCategorieComm.Add(New JObject(New JProperty("Linea_Classe_Cod", CStr(dr.Item("Linea_Classe_Cod"))), New JProperty("Linea_Classe_Des", dr.Item("Linea_Classe_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayCategorieComm, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim filtro_aggiuntivo As String = ""
            If Trim(filtro_specie) <> "" Then
                filtro_aggiuntivo = " Cultivar.Veg_Cod IN (" & filtro_specie & ") "
            End If

            Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim dtVarieta = Cultivar_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_aggiuntivo, "", objParametri_Server)

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtVarieta.Rows
                jArrayParamQual.Add(New JObject(New JProperty("Cul_Cod", dr.Item("Cul_Cod")), New JProperty("Cul_Des", dr.Item("Veg_Des") & " - " & dr.Item("Cul_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contatti_Clienti_Agenti(piva As String, cliente As Integer, agente As Integer, fornitore As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = objContatti.Contatti_Contatto_Leggi(piva,
                "", 0, 0, False, False, 0, 0, False, 0, ID_CF_NOFILTRO, 0, "", False, cliente, fornitore, 0, 0, 0,
                enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "Contatti.Rag_Soc, Cognome, Nome", objParametri_Server, Agente:=agente)

            Dim jArrayListaClientiAgenti As New JArray()
            For Each dr As DataRow In dtContatti.Rows
                Dim ragSoc As String = CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome"))
                ragSoc = ragSoc & " (" & CStr(dr.Item("Rapporto_Des")) & ")"
                jArrayListaClientiAgenti.Add(New JObject(New JProperty("cod_contatto", dr.Item("cod_contatto")), New JProperty("nome", ragSoc.Replace("""", "'"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaClientiAgenti, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Dati_Grafico(ByVal piva As String, ByVal report As String, ByVal cubo As Boolean, ByVal tipo As String,
                                              ByVal _dataRegDal As String, ByVal _dataRegAl As String,
                                              ByVal _specie As String, ByVal _varieta As String,
                                              ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String,
                                              ByVal _clienti As String,
                                              ByVal _agenti As String,
                                              ByVal _causali As String,
                                              ByVal _includiCorrispettivi As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            Dim _causali_trasp As String = ""

            If report = "Report_Vendite" OrElse report = "Report_Ordini_Vendita" Then
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Report_Vendite_Causali", "", "", objParametri_Server)
                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                    _causali_trasp = DTConfigSiti.Rows(0)("valore").ToString
                End If
            End If

            Dim leggi As New Statistiche_R
            Dim dati = leggi.Leggi_Grafico_Report_Vendite(piva, report, cubo, tipo, _dataRegDal, _dataRegAl, _clienti, _agenti, _causali, _specie, _varieta, _prodotti, _categorie, _categcommerciali, _causali_trasp, _includiCorrispettivi, objParametri_Server, objParametriUtenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dati, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaDocumento(ByVal piva As String, ByVal Id_Agenda As Integer, ByVal Lav_Cod As Integer,
                                           ByVal Modulo As Integer,
                                           ByVal Tipo_Accettazione As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim vVarStampe(4) As ElementoStampe
        vVarStampe(0).Nome = "piva"
        vVarStampe(0).Valore = piva
        vVarStampe(1).Nome = "id_agenda"
        vVarStampe(1).Valore = Id_Agenda
        vVarStampe(2).Nome = "printcode"
        vVarStampe(2).Valore = Lav_Cod
        vVarStampe(3).Nome = "printtoprinter"
        vVarStampe(3).Valore = 0
        vVarStampe(4).Nome = "printname"
        vVarStampe(4).Valore = ""

        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
        Dim StrNodiVariabili As String = StrNodo
        Dim Report As Integer = 0

        Select Case Lav_Cod

            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                Report = enum_CodificaStampe.DDT_Contabilizzato_Emesso

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                Report = enum_CodificaStampe.Bolle

            Case LAVCOD_FATTURA_EMESSA
                Report = enum_CodificaStampe.Fatture

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                Report = enum_CodificaStampe.Nota_Accredito

            Case LAVCOD_RICEVUTA_EMESSA
                Report = enum_CodificaStampe.RicevuteFiscali

            Case LAVCOD_ORDINE_VENDITA
                Report = enum_CodificaStampe.Ordine

            Case LAVCOD_ORDINE_ACQUISTO
                Report = enum_CodificaStampe.Ordine_Acquisto

            Case LAVCOD_ACCETTAZIONE_DIVERSI
                ' Accettazione DDT ricevuto
                If Tipo_Accettazione = -1 Then
                    Report = enum_CodificaStampe.Certificato_Pomodoro
                Else
                    Select Case Modulo
                        Case enum_Omni_Modulo_Generazione.Cantine
                            Report = enum_CodificaStampe.ConferimentoUva_DDTRicevuto
                        Case enum_Omni_Modulo_Generazione.FreshFood,
                            enum_Omni_Modulo_Generazione.Tabacco
                            Report = enum_CodificaStampe.FreshFood_BollaAccettazione

                        Case Else

                            'Visto che ora non viene più scritto sull'Agenda il modulo, vado a vedere quello che è installato
                            'TODO: da riguardare quando si gestiranno anche le Cantine se sta ancora in piedi

                            'Leggo i moduli installati
                            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                            Dim listModuli As List(Of Integer) = objO.Recupera_Moduli_Cliente(piva, objParametri_Server)

                            If listModuli IsNot Nothing AndAlso listModuli.Count > 0 AndAlso
                               (listModuli.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Tabacco) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Zoo)) Then
                                Report = enum_CodificaStampe.FreshFood_BollaAccettazione
                            Else
                                Report = enum_CodificaStampe.Buono_Accettazione_Diversi
                            End If

                    End Select
                End If

            Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                Select Case Modulo
                    Case enum_Omni_Modulo_Generazione.Cantine
                        Report = enum_CodificaStampe.ConferimentoUva_AutoDDT
                    Case enum_Omni_Modulo_Generazione.FreshFood,
                        enum_Omni_Modulo_Generazione.Tabacco
                        Report = enum_CodificaStampe.FreshFood_AutoDDT_Accettazione
                    Case Else
                        ' Errore
                        Report = 0
                End Select

            Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                Report = enum_CodificaStampe.FreshFood_DistintaCarico_Accettazione
        End Select

        If Report <> 0 Then

            Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
            objAgronicaStampe.report = Report
            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
            r.RispostaOK = True

        Else

            r.RispostaOK = False
            r.Errore = "Tipo di stampa non disponibile"

        End If

        Return r

    End Function

    'salvataggio parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaReport(ByVal nome As String, ByVal report As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            'Codifica per evitare HTML/JS injection
            Varie.SanitizeTesto_MantieniVirgoletteECaratteriAccentati(nome)

            Dim impostazioneUtenteReport = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_REPORT_STATISTICHE_QDC

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteReport, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaReport As New JArray()
            Dim jObjectReport = JsonConvert.DeserializeObject(report)

            'Codifica per evitare HTML/JS injection
            Varie.SanitizeTesto_MantieniVirgoletteECaratteriAccentati(jObjectReport("nome"))

            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    If impostazioni("nome") = nome Then
                        jArrayListaReport.Add(jObjectReport)
                        trovato = True
                    Else
                        jArrayListaReport.Add(impostazioni)
                    End If
                Next
            End If
            If Not trovato Then
                jArrayListaReport.Add(jObjectReport)
            End If

            Dim impostazioniReport = JsonConvert.SerializeObject(jArrayListaReport, Formatting.None)

            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(impostazioneUtenteReport, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(impostazioneUtenteReport, impostazioniReport, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If r.RispostaOK = True Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nel salvataggio del report"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'legge lista report
    <WebMethod(EnableSession:=True)>
    Public Shared Function ListaReport() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteReport As Integer
            impostazioneUtenteReport = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_REPORT_STATISTICHE_QDC

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteReport, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaReport As New JArray()
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    jArrayListaReport.Add(New JObject(New JProperty("cod", impostazioni("nome")), New JProperty("desc", impostazioni("nome"))))
                Next
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaReport, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'legge parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiReport(ByVal nome As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteReport As Integer
            impostazioneUtenteReport = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_REPORT_STATISTICHE_QDC

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteReport, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaReport As New JArray()
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    If impostazioni("nome") = nome Then
                        r.RispostaStringa = JsonConvert.SerializeObject(impostazioni, Formatting.None)
                        Exit For
                    End If
                Next
            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'legge parametri report
    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaReport(ByVal nome As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteReport As Integer
            impostazioneUtenteReport = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_REPORT_STATISTICHE_QDC

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteReport, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayListaReport As New JArray()
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    If impostazioni("nome") <> nome Then
                        jArrayListaReport.Add(impostazioni)
                    End If
                Next
            End If

            Dim impostazioniReport = JsonConvert.SerializeObject(jArrayListaReport, Formatting.None)

            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(impostazioneUtenteReport, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(impostazioneUtenteReport, impostazioniReport, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If r.RispostaOK = True Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nella cancellazione del report"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

#End Region

#Region "filtri"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiOperazioni(ByVal tipoOp As Integer()) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dt As New DataTable

        Dim objUtentiImpostazioniMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim operazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            Dim dtO = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objParametri_Utenti)
            Dim Filtro_Utente_Lavorazioni = ""
            If dtO.Rows.Count > 0 Then
                Filtro_Utente_Lavorazioni = " OPERAZIONI.Lav_Cod in ("
                For i = 0 To dtO.Rows.Count - 1

                    If i <> 0 Then
                        Filtro_Utente_Lavorazioni &= " ,"
                    End If
                    Filtro_Utente_Lavorazioni &= dtO.Rows(i).Item("ID_0")
                Next
                Filtro_Utente_Lavorazioni &= " )  "

            End If

            For Each tipo In tipoOp

                Dim dtt = operazioni.Leggi(0, tipo, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            Filtro_Utente_Lavorazioni, "", objParametri_Server)

                If IsNothing(dt) Then
                    dt = dtt
                Else
                    dt.Merge(dtt)
                End If
            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRegioni(ByVal nazioni As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim regioni As New Lista_Regioni_R
            Dim dt As New DataTable
            Dim dtTemp As New DataTable

            For Each nazione As String In nazioni.Split("|")

                If nazione IsNot String.Empty Then

                    dtTemp = regioni.Leggi("", "", "", objParametri_Server, nazione)

                    dt.Merge(dtTemp)

                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiProvince(ByVal regioni As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim province As New Lista_Province_R
            Dim dt As New DataTable
            Dim dtTemp As New DataTable

            For Each regione As String In regioni.Split("|")

                If regione IsNot String.Empty Then

                    dtTemp = province.Leggi("", regione, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, "")

                    dt.Merge(dtTemp)

                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiComuni(ByVal province As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim comuni As New Istat_R
            Dim dt As New DataTable
            Dim dtTemp As New DataTable

            For Each provincia As String In province.Split("|")

                If provincia IsNot String.Empty Then

                    dtTemp = comuni.Leggi(provincia, "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", objParametri_Server)

                    dt.Merge(dtTemp)

                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiReferenti() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim gerarchia As New GerarchiaImprese_R
            Dim dt As New DataTable
            Dim dtTemp As New DataTable
            Dim imprese As New Imprese_Read

            Dim imprese_Visibili = imprese.Leggi_ImpreseCUAA_Visibilita_Utente(objParametri_Server, objParametri_Utenti, True)
            Dim imprese_Lista As New List(Of String)
            For Each impresa As DataRow In imprese_Visibili.Rows
                imprese_Lista.Add(impresa.Item("Piva"))
            Next

            Dim listaFigli As String = String.Empty

            If imprese_Lista.Count > 0 Then
                listaFigli = " GerarchiaImprese.figlio IN " & QueryBuilderUtility.GeneraClausolaINDaList(imprese_Lista)
            End If

            dt = gerarchia.LeggiPadriGerarchia("", "", 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                              listaFigli, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAziende(ByVal referenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim gerarchia As New GerarchiaImprese_R
            Dim dt As New DataTable
            Dim imprese As New Imprese_Read
            Dim imprese_Visibili = imprese.Leggi_ImpreseCUAA_Visibilita_Utente(objParametri_Server, objParametri_Utenti, True)
            Dim imprese_Lista As New List(Of String)
            For Each impresa As DataRow In imprese_Visibili.Rows
                imprese_Lista.Add(impresa.Item("Piva"))
            Next

            Dim filtroAgg As String = String.Empty

            If imprese_Lista.Count > 0 Then
                filtroAgg = "GerarchiaImprese.Figlio IN " & QueryBuilderUtility.GeneraClausolaINDaList(imprese_Lista)
            End If

            If referenti.Length > 0 Then
                Dim refList = referenti.Split("|").ToList()
                refList.RemoveAt(referenti.Split("|").Length - 1)
                If imprese_Lista.Count > 0 Then
                    filtroAgg &= " AND "
                End If
                filtroAgg &= " GerarchiaImprese.Padre IN " & QueryBuilderUtility.GeneraClausolaINDaList(refList)
            End If

            dt = gerarchia.LeggiFigli("",
                                      "",
                                      0,
                                      0,
                                      enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                      filtroAgg,
                                      "",
                                      objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiCentriAziendali(ByVal aziende As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim centri As New CentriAziendali_Read
            Dim dt As New DataTable
            Dim azList = aziende.Split("|").ToList()
            azList.RemoveAt(aziende.Split("|").Length - 1)

            dt = centri.Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                              " Centri_Aziendali.PIVA IN " & QueryBuilderUtility.GeneraClausolaINDaList(azList), "", objParametri_Server)

            dt.Columns.Add(New DataColumn("PivaSa", GetType(String)))

            For Each centro In dt.Rows
                centro.item("PivaSa") = centro.item("PIVA") & "|" & centro.item("Sa_Cod")
            Next


            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objUtentiImpostazioniMono As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim dt = objUtentiImpostazioniMono.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI,
                                                           0,
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "", "", objParametri_Utenti)
            Dim filtroSpecieVeg As String = ""
            If dt.Rows.Count > 0 Then
                filtroSpecieVeg = " SpecieVegetali.Veg_Cod in ("
                For i = 0 To dt.Rows.Count - 1

                    If i <> 0 Then
                        filtroSpecieVeg &= " ,"
                    End If
                    filtroSpecieVeg &= dt.Rows(i).Item("ID_0")
                Next
                filtroSpecieVeg &= " ) "

            End If

            Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecie = SpecieVegetali_R.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtroSpecieVeg, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtSpecie, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


#End Region

#Region "script services Report su PDF"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Lancia_Report_PDF(ByVal piva As String,
            ByVal analisi As String, ByVal cubo As Boolean,
            ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
            ByVal _docNumeroDes As String, ByVal _nrRiga As String,
            ByVal _dataRegDal As String, ByVal _dataRegAl As String,
            ByVal _specie As String, ByVal _varieta As String,
            ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String,
            ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
            ByVal tipo_report As String, ByVal titolo As String, ByVal livelli As String,
            ByVal da_mese As String, ByVal da_anno As String,
            ByVal a_mese As String, ByVal a_anno As String, ByVal confronto_anno As String,
            ByVal tipo_valore As String, ByVal tipo_secondo_valore As String,
            ByVal decimali_qta As Integer,
            ByVal scost_totale As Boolean, ByVal salto_pag_1_liv As Boolean, ByVal cb_ordin_x_valore As Boolean, ByVal mese_prev_scost As String,
            ByVal _rapportiContabili As String, ByVal _nazioniFatturazione As String, ByVal _includiCorrispettivi As Boolean
        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim _causali_trasp As String = ""

        If analisi = "Report_Vendite" OrElse analisi = "Report_Ordini_Vendita" Then
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Report_Vendite_Causali", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                _causali_trasp = DTConfigSiti.Rows(0)("valore").ToString
            End If
        End If

        Dim vVarStampe(34) As ElementoStampe

        vVarStampe(0).Nome = "p"
        vVarStampe(0).Valore = piva
        vVarStampe(1).Nome = "numSin"
        vVarStampe(1).Valore = _docNumeroSin
        vVarStampe(2).Nome = "num"
        vVarStampe(2).Valore = _docNumero
        vVarStampe(3).Nome = "numoDes"
        vVarStampe(3).Valore = _docNumeroDes
        vVarStampe(4).Nome = "nrRiga"
        vVarStampe(4).Valore = _nrRiga
        vVarStampe(5).Nome = "dataDal"
        vVarStampe(5).Valore = _dataRegDal
        vVarStampe(6).Nome = "dataAl"
        vVarStampe(6).Valore = _dataRegAl
        vVarStampe(7).Nome = "cli"
        vVarStampe(7).Valore = _clienti
        vVarStampe(8).Nome = "age"
        vVarStampe(8).Valore = _agenti
        vVarStampe(9).Nome = "causali"
        vVarStampe(9).Valore = _causali
        vVarStampe(10).Nome = "spe"
        vVarStampe(10).Valore = _specie
        vVarStampe(11).Nome = "var"
        vVarStampe(11).Valore = _varieta
        vVarStampe(12).Nome = "prod"
        vVarStampe(12).Valore = _prodotti
        vVarStampe(13).Nome = "categ"
        vVarStampe(13).Valore = _categorie
        vVarStampe(14).Nome = "categcommerciali"
        vVarStampe(14).Valore = _categcommerciali

        vVarStampe(15).Nome = "cau_trasp"
        vVarStampe(15).Valore = _causali_trasp

        vVarStampe(16).Nome = "tipo_report"
        vVarStampe(16).Valore = tipo_report
        vVarStampe(17).Nome = "titolo"
        vVarStampe(17).Valore = titolo
        vVarStampe(18).Nome = "liv"
        vVarStampe(18).Valore = livelli
        vVarStampe(19).Nome = "da_mese"
        vVarStampe(19).Valore = da_mese
        vVarStampe(20).Nome = "da_anno"
        vVarStampe(20).Valore = da_anno
        vVarStampe(21).Nome = "confr_anno"
        vVarStampe(21).Valore = confronto_anno
        vVarStampe(22).Nome = "tipo_val"
        vVarStampe(22).Valore = tipo_valore
        vVarStampe(23).Nome = "scost_tot"
        vVarStampe(23).Valore = scost_totale
        vVarStampe(24).Nome = "saltopag_1liv"
        vVarStampe(24).Valore = salto_pag_1_liv
        vVarStampe(25).Nome = "mese_prev_scost"
        vVarStampe(25).Valore = mese_prev_scost
        vVarStampe(26).Nome = "analisi"
        vVarStampe(26).Valore = analisi
        vVarStampe(27).Nome = "cb_ordin_x_valore"
        vVarStampe(27).Valore = cb_ordin_x_valore
        vVarStampe(28).Nome = "rapportiContabili"
        vVarStampe(28).Valore = _rapportiContabili
        vVarStampe(29).Nome = "nazioniFatturazione"
        vVarStampe(29).Valore = _nazioniFatturazione
        vVarStampe(30).Nome = "a_mese"
        vVarStampe(30).Valore = a_mese
        vVarStampe(31).Nome = "a_anno"
        vVarStampe(31).Valore = a_anno
        vVarStampe(32).Nome = "tipo_val_2"
        vVarStampe(32).Valore = tipo_secondo_valore
        vVarStampe(33).Nome = "decimali_qta"
        vVarStampe(33).Valore = decimali_qta
        vVarStampe(34).Nome = "includi_corr"
        vVarStampe(34).Valore = _includiCorrispettivi


        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
        Dim StrNodiVariabili As String = StrNodo

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        objAgronicaStampe.report = enum_CodificaStampe.Report_Vendita_PDF
        objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
        objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
        objAgronicaStampe.Xml_Generico.Length = 0
        objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                        objAgronicaStampe)

        Dim strSplit As String() = strJS.Split(New Char() {"'"c}, StringSplitOptions.RemoveEmptyEntries)

        r.RispostaOK = True
        r.RispostaStringa = strSplit(3)

        Return r

    End Function

#End Region


End Class