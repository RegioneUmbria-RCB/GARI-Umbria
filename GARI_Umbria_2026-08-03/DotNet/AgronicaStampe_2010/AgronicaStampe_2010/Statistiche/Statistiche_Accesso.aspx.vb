Imports System.Reflection
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.Varie
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Statistiche_Accesso
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Class ObjPermessiStatistiche
        Public Property ReportDettagliServiziAzienda As Boolean
    End Class

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetPermessiBottoni() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objPermessiStatistiche As New ObjPermessiStatistiche

        Try
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim StatistometroReportDettagliServiziAzienda As Boolean = False
            StatistometroReportDettagliServiziAzienda = objPermessi.Controlla_Permessi_Utente(
                                    HttpContext.Current.Session("ASG_Utente_Username"),
                                    HttpContext.Current.Session("ASG_IdServizio"),
                                    enum_Security_Attivita.StatistometroReportDettagliServiziAzienda,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
            If StatistometroReportDettagliServiziAzienda Then
                objPermessiStatistiche.ReportDettagliServiziAzienda = True
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(objPermessiStatistiche, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPathNetCoreApi()
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim pathNetCoreApi = objConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_NetCore_API", "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = pathNetCoreApi

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

#Region "Elabora Statistica"


    <WebMethod(EnableSession:=True)>
    Public Shared Function getDataGrafici(v_inizio As String, v_fine As String,
                                          piva As String, tipoFiltro As Integer) As RispostaStandard

        Dim r As RispostaStandard = New RispostaStandard()
        Try


            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri
            Dim isObjParametriSet = setObjParametri(objParametri_Server, objParametri_Utenti)
            If Not isObjParametriSet Then
                r.Sessione = False
                Return r
            End If

            Dim validita_inizio As Date? = If(String.IsNullOrWhiteSpace(v_inizio), AGRODATAINIZIO, CDate(v_inizio))
            Dim validita_fine As Date? = If(String.IsNullOrWhiteSpace(v_fine), AGRODATAFINE, CDate(v_fine))


            Dim agroStatistiche = New AgronicaCoreContabBIZ.Statistiche_Utilizzo_R
            Dim statistiche = agroStatistiche.getStatistiche(validita_inizio, validita_fine, piva, tipoFiltro, objParametri_Server, objParametri_Utenti)


            Dim datiStatistiche = New DatiStatistiche With {
                .transazioni = CreateDatiCharts(statistiche.transazioni),
                .aziende_movimentante = CreateDatiCharts(statistiche.aziende_movimentante)
            }

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(datiStatistiche)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    Private Shared Function CreateDatiCharts(Of T)(input As T) As List(Of DatiChart)
        Dim fields As FieldInfo() = GetType(T).GetFields(BindingFlags.Public Or BindingFlags.Instance)

        Dim charts As New List(Of DatiChart)

        For Each field As FieldInfo In fields
            'Dim fieldName As String = field.Name
            Dim fieldValue As Tuple(Of String, Integer) = CType(field.GetValue(input), Tuple(Of String, Integer))
            Dim chart As New DatiChart(fieldValue.Item1, New List(Of Double) From {fieldValue.Item2})
            charts.Add(chart)
        Next

        Return charts
    End Function


    Class DatiChart
        Public name As String
        Public data As List(Of Double)

        Public Sub New(n As String, d As List(Of Double))
            Me.name = n
            Me.data = d
        End Sub
    End Class

    Class DatiStatistiche
        Public transazioni As List(Of DatiChart)
        Public aziende_movimentante As List(Of DatiChart)
    End Class


#End Region

#Region "DropDown Aziende"
    <WebMethod(EnableSession:=True)>
    Public Shared Function caricaAziende() As RispostaStandard

        Dim r As RispostaStandard = New RispostaStandard()
        Try

            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri
            Dim isObjParametriSet = setObjParametri(objParametri_Server, objParametri_Utenti)
            If Not isObjParametriSet Then
                r.Sessione = False
                Return r
            End If


            Dim comboControl = CaricaComboImprese(objParametri_Server, objParametri_Utenti)

            Dim result = comboControl.Items.Cast(Of ListItem).
                Select(Function(item) New DropdownItem With {
                    .text = item.Text,
                    .value = item.Value
                }).ToList

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(result)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function


    Class DropdownItem
        Public text As String
        Public value As String
    End Class

    ' Codice da AgronicaCoreControlli.ComboImprese_no_Foglie, 
    Private Shared Function CaricaComboImprese(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As DropDownList

        Dim ddl_ComboImprese = New DropDownList
        'foglia = 0
        Dim objGer As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim DT As DataTable = objGer.Leggi("", 2, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim hashIm As New Hashtable
        For Each dr As DataRow In DT.Rows
            If Not hashIm.ContainsKey(dr.Item("figlio")) Then
                hashIm.Add(dr.Item("figlio"), dr.Item("figlio"))
            End If
        Next

        CaricaListControl.ImpreseConFiltroUtente(ddl_ComboImprese, True, "", "",
                                                 "", " ORDER BY Rag_Soc asc",
                                                 objParametri_Server,
                                                 objParametri_Utenti)

        'rimuovo quelle che non sono figlie
        For i As Integer = ddl_ComboImprese.Items.Count - 1 To 0 Step -1
            If Not hashIm.ContainsKey(ddl_ComboImprese.Items(i).Value) Then
                If ddl_ComboImprese.Items(i).Value <> "" Then
                    ddl_ComboImprese.Items.RemoveAt(i)
                End If
            End If
        Next
        Return ddl_ComboImprese
    End Function


#End Region

#Region "Elenco Sintetico Con/Senza dettagli"

    <WebMethod(EnableSession:=True)>
    Public Shared Function getElencoSinteticoMovimentiConDettagli(v_inizio As String,
                                                                  v_fine As String,
                                                                  piva As String,
                                                                  tipoFiltro As Integer
                                                                  ) As RispostaStandard

        Dim r As RispostaStandard = New RispostaStandard()
        Try
            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri
            Dim isObjParametriSet = setObjParametri(objParametri_Server, objParametri_Utenti)

            If Not isObjParametriSet Then
                r.Sessione = False
                Return r
            End If

            Dim validita_inizio As Date = If(String.IsNullOrWhiteSpace(v_inizio), AGRODATAINIZIO, CDate(v_inizio))
            Dim validita_fine As Date = If(String.IsNullOrWhiteSpace(v_fine), AGRODATAFINE, CDate(v_fine))

            Dim ObjAllegato As New objAllegato
            Dim file_path = getElencoSintetico(validita_inizio, validita_fine, piva, tipoFiltro, True, objParametri_Server, objParametri_Utenti, objAllegato:=ObjAllegato)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(ObjAllegato, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getElencoSinteticoMovimenti(v_inizio As String, v_fine As String,
                                                       piva As String, tipoFiltro As Integer) As RispostaStandard

        Dim r As RispostaStandard = New RispostaStandard()
        Try

            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri
            Dim isObjParametriSet = setObjParametri(objParametri_Server, objParametri_Utenti)
            If Not isObjParametriSet Then
                r.Sessione = False
                Return r
            End If

            Dim validita_inizio As Date = If(String.IsNullOrWhiteSpace(v_inizio), AGRODATAINIZIO, CDate(v_inizio))
            Dim validita_fine As Date = If(String.IsNullOrWhiteSpace(v_fine), AGRODATAFINE, CDate(v_fine))

            Dim ObjAllegato As New objAllegato
            Dim file_path = getElencoSintetico(validita_inizio, validita_fine, piva, tipoFiltro, False, objParametri_Server, objParametri_Utenti, objAllegato:=ObjAllegato)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(ObjAllegato, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    ' flag dettagli_qdc
    ' true Elenco sintetico con dettagli operazioni colturali
    ' false Elenco sintetico movimenti per azienda
    Private Shared Function getElencoSintetico(v_inizio As Date, v_fine As Date,
                                               piva As String, tipoFiltro As Integer,
                                               ByVal Dettagli_QDC As Boolean,
                                               objParametri_Server As AgronicaCoreParametri,
                                               objParametri_Utenti As AgronicaCoreParametri,
                                                  Optional ByRef objAllegato As objAllegato = Nothing
                                               ) As String

        ' La combo utenti e' disattiva dal 2019, lascio vuota
        Dim username = ""

        '--------------------------------------------------------
        'AGENDA TOTALE + DISTINCT
        '--------------------------------------------------------
        Dim FiltroAggiuntivo_Imprese As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda1 As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda2 As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda3 As String = ""

        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

        Dim Applica_VisibilitaUtente As Boolean = False

        If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
            Applica_VisibilitaUtente = True
        End If

        'genero filtro delle piva
        If piva <> "" AndAlso piva <> objParametri_Server.PivaSuperUser Then
            Dim objger As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            objger.LeggiFigliNodoGerarchiaImprese(piva, FiltroAggiuntivo_Imprese, objParametri_Server)
            If FiltroAggiuntivo_Imprese <> "" Then
                FiltroAggiuntivo_Imprese = FiltroAggiuntivo_Imprese & ",'" & piva & "'"
            Else
                FiltroAggiuntivo_Imprese = "'" & piva & "'"
            End If
        End If

        If FiltroAggiuntivo_Imprese <> "" Then
            FiltroAggiuntivo_Imprese_Agenda = " imprese.PIVA in (" & FiltroAggiuntivo_Imprese & ")"
            FiltroAggiuntivo_Imprese_Agenda1 = " a.PIVA in (" & FiltroAggiuntivo_Imprese & ")"
            FiltroAggiuntivo_Imprese_Agenda2 = " p.PIVA in (" & FiltroAggiuntivo_Imprese & ")"
            FiltroAggiuntivo_Imprese_Agenda3 = " p.PC_Dettagli_PIVA in (" & FiltroAggiuntivo_Imprese & ")"
        End If

        Dim DT_Esporta_Dettagli As New DataTable
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim objPua As New AgronicaCorePUA_DAL.PUA_Testata_R
        Dim objPCon As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R
        Dim objPratica As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim objPraticaBiZ As New AgronicaCoreProfilazioneBIZ.Pratiche_R
        Dim objAWS As New AgronicaCoreUtentiDAL.AWS_log_R


        Dim piveInsertString = AgronicaCoreUtility.QueryBuilderUtility.GeneraStringaInsertDaList("#pive", FiltroAggiuntivo_Imprese.Split(",").ToList)

        Dim debugHelper As New DebugHelper()

        debugHelper.DebugWriteLine("Btn_Esporta_Esegui - Dettagli_QDC: " & Dettagli_QDC.ToString() & " - Piva: " & piva)

        debugHelper.WatchStartNew()

        Dim esisteServizioQdCBluArancio = objPraticaBiZ.VerificaEsistenzaServizioByPivaServizio("", enum_Servizi.QDemetraQdCBluarancio, "", objParametri_Server)


        DT_Esporta_Dettagli = objAgenda.Leggi_NumOperazioni_xStatistiche_DistinctPiva_New(tipoFiltro,
                                                                                      v_inizio,
                                                                                      v_fine,
                                                                                      username,
                                                                                      piveInsertString,
                                                                                      "",
                                                                                      "",
                                                                                      objParametri_Server,
                                                                                      Applica_VisibilitaUtente,
                                                                                      Dettagli_QDC,
                                                                                      Solo_Movimentati:=If(esisteServizioQdCBluArancio, False, True)
                                                                                      )

        debugHelper.DebugWriteLineElapsedMs("Leggi_NumOperazioni_xStatistiche_DistinctPiva_New")

        DT_Esporta_Dettagli.Columns.Add(New DataColumn("N_Utenti", GetType(Integer)))
        DT_Esporta_Dettagli.Columns.Add(New DataColumn("Utenti", GetType(String)))

        Dim DtOpCamp As DataTable
        Dim DtOpMag As DataTable
        Dim DtPua As DataTable
        Dim DtPCon As DataTable
        Dim DtPraticheSottoscrizioneQdC As DataTable = Nothing
        Dim DtPraticheStatoAttuale As DataTable = Nothing
        Dim DtAccessi As DataTable = Nothing

        DtOpCamp = objAgenda.Leggi_xStatistiche_OperazioniCampagna_Utenti_New(tipoFiltro,
                                                                          v_inizio,
                                                                          v_fine,
                                                                          username,
                                                                          piveInsertString,
                                                                          "",
                                                                          "",
                                                                          objParametri_Server,
                                                                          objParametri_Utenti,
                                                                          Applica_VisibilitaUtente)

        debugHelper.DebugWriteLineElapsedMs("Leggi_xStatistiche_OperazioniCampagna_Utenti_New")

        DtOpMag = objAgenda.Leggi_xStatistiche_OperazioniMagazzino_Utenti_New(tipoFiltro,
                                                                          v_inizio,
                                                                          v_fine,
                                                                          username,
                                                                          False,
                                                                          piveInsertString,
                                                                          "",
                                                                          "",
                                                                          objParametri_Server,
                                                                          objParametri_Utenti,
                                                                          False,
                                                                          "",
                                                                          Applica_VisibilitaUtente,
                                                                          FiltroAggiuntivo_Imprese,
                                                                          False)

        debugHelper.DebugWriteLineElapsedMs("Leggi_xStatistiche_OperazioniMagazzino_Utenti_New")

        DtPua = objPua.Leggi_xStatistiche_PUA_Utenti(tipoFiltro,
                                                 v_inizio,
                                                 v_fine,
                                                 username,
                                                 FiltroAggiuntivo_Imprese_Agenda2,
                                                 "",
                                                 objParametri_Server,
                                                 objParametri_Utenti,
                                                 Applica_VisibilitaUtente)

        debugHelper.DebugWriteLineElapsedMs("Leggi_xStatistiche_PUA_Utenti")

        DtPCon = objPCon.Leggi_xStatistiche_PianoConcimazione_Utenti(tipoFiltro,
                                                                 v_inizio,
                                                                 v_fine,
                                                                 username,
                                                                 FiltroAggiuntivo_Imprese_Agenda3,
                                                                 "",
                                                                 objParametri_Server,
                                                                 objParametri_Utenti,
                                                                 Applica_VisibilitaUtente)

        debugHelper.DebugWriteLineElapsedMs("Leggi_xStatistiche_PianoConcimazione_Utenti")

        If esisteServizioQdCBluArancio Then

            DtPraticheSottoscrizioneQdC = objPratica.Leggi_conStoricoTransizioniDiStato(0,
                      "", "", "",
                      0, 0, 0, enum_Servizi.QDemetraQdCBluarancio, enum_WWorflow_WAnagraficaStati.QdC_Bluarancio_Demetra_Azienda_Attivata,
                      v_inizio, v_fine,
                      "",
                      "",
                      objParametri_Server)

            DT_Esporta_Dettagli.Columns.Add(New DataColumn("Data_sottoscrizione_QdC", GetType(Date)))

            DtPraticheStatoAttuale = objPratica.Leggi_conStatoAttuale(0,
                            "", "", "",
                           0, 0, 0,
                           enum_Servizi.QDemetraQdCBluarancio,
                           Stato_Cod:=0,
                           v_inizio, v_fine,
                           "",
                           "",
                           objParametri_Server, 0, 0)

            DT_Esporta_Dettagli.Columns.Add(New DataColumn("Modalita_attivazione", GetType(String)))

            DtAccessi = objAWS.LeggiMinMaxTutti(xFiltroAggiuntivo:="", xOrderBy:="", objParametri_Utenti)

            DT_Esporta_Dettagli.Columns.Add(New DataColumn("Data_primo_login", GetType(Date)))
            DT_Esporta_Dettagli.Columns.Add(New DataColumn("Data_ultimo_login", GetType(Date)))

        End If

        For Each dr As DataRow In DT_Esporta_Dettagli.Rows

            Dim strUtenti As String = ""
            Dim HashUtenti As New Hashtable
            Dim DrOpM() As DataRow
            Dim DrOpC() As DataRow
            Dim DrPua() As DataRow
            Dim DrPCon() As DataRow
            Dim DrPraticheSottoscrizioneQdC() As DataRow
            Dim DrPraticheStatoAttuale() As DataRow
            Dim DrAccessi() As DataRow
            Dim N_Utenti As Integer = 0

            If DtOpMag IsNot Nothing Then
                DrOpM = DtOpMag.Select("piva='" & dr.Item("piva") & "'")
                If DrOpM IsNot Nothing Then
                    For Each dro As DataRow In DrOpM
                        If Not HashUtenti.ContainsKey(dro.Item("username_creazione")) Then
                            strUtenti &= dro.Item("utente") & ","
                            HashUtenti.Add(dro.Item("username_creazione"), "")
                            N_Utenti += 1
                        End If
                    Next
                End If
            End If

            If DtOpCamp IsNot Nothing Then
                DrOpC = DtOpCamp.Select("piva='" & dr.Item("piva") & "'")
                If DrOpC IsNot Nothing Then
                    For Each dro As DataRow In DrOpC
                        If Not HashUtenti.ContainsKey(dro.Item("username_creazione")) Then
                            strUtenti &= dro.Item("utente") & ","
                            HashUtenti.Add(dro.Item("username_creazione"), "")
                            N_Utenti += 1
                        End If
                    Next
                End If
            End If

            If DtPua IsNot Nothing Then
                DrPua = DtPua.Select("piva='" & dr.Item("piva") & "'")
                If DrPua IsNot Nothing Then
                    For Each dro As DataRow In DrPua
                        If Not HashUtenti.ContainsKey(dro.Item("username_creazione")) Then
                            strUtenti &= dro.Item("utente") & ","
                            HashUtenti.Add(dro.Item("username_creazione"), "")
                            N_Utenti += 1
                        End If
                    Next
                End If
            End If

            If DtPCon IsNot Nothing Then
                DrPCon = DtPCon.Select("piva='" & dr.Item("piva") & "'")
                If DrPCon IsNot Nothing Then
                    For Each dro As DataRow In DrPCon
                        If Not HashUtenti.ContainsKey(dro.Item("username_creazione")) Then
                            strUtenti &= dro.Item("utente") & ","
                            HashUtenti.Add(dro.Item("username_creazione"), "")
                            N_Utenti += 1
                        End If
                    Next
                End If
            End If

            If strUtenti <> "" Then
                strUtenti = Left(strUtenti, strUtenti.Length - 1)
            End If

            dr.Item("N_Utenti") = N_Utenti
            dr.Item("Utenti") = strUtenti

            If esisteServizioQdCBluArancio Then

                If DtPraticheSottoscrizioneQdC IsNot Nothing AndAlso Not IsDBNull(dr.Item("piva")) AndAlso dr.Item("piva") <> "" Then

                    Dim maxDataSottoscrizione = AGRODATAINIZIO

                    'DT: Sviluppo ad hoc Coldiretti: prendere la data inizio dello stato "Azienda attivata" (in caso di più pratiche esistenti nell'intervallo selezionato, si prende la più recente
                    DrPraticheSottoscrizioneQdC = DtPraticheSottoscrizioneQdC.Select("Piva='" & dr.Item("piva") & "'")
                    If DrPraticheSottoscrizioneQdC IsNot Nothing Then
                        For Each dro As DataRow In DrPraticheSottoscrizioneQdC

                            If Not IsDBNull(dro.Item("Validita_Inizio_Stato")) Then
                                Dim dataCorrente = CDate(dro.Item("Validita_Inizio_Stato"))

                                If dataCorrente > maxDataSottoscrizione Then
                                    maxDataSottoscrizione = dataCorrente
                                End If
                            End If
                        Next

                        If maxDataSottoscrizione <> AGRODATAINIZIO Then
                            dr.Item("Data_sottoscrizione_QdC") = maxDataSottoscrizione.ToShortDateString
                        End If

                    End If
                End If

                If DtPraticheStatoAttuale IsNot Nothing AndAlso Not IsDBNull(dr.Item("piva")) AndAlso dr.Item("piva") <> "" Then

                    Dim maxDataStatoAttuale = AGRODATAINIZIO
                    Dim statoAttuale = ""

                    'DT: Sviluppo ad hoc Coldiretti: prendere lo stato attuale (in caso di più pratiche esistenti nell'intervallo selezionato, si prende la più recente)
                    DrPraticheStatoAttuale = DtPraticheStatoAttuale.Select("Piva='" & dr.Item("piva") & "'")
                    If DrPraticheStatoAttuale IsNot Nothing Then
                        For Each dro As DataRow In DrPraticheStatoAttuale

                            If Not IsDBNull(dro.Item("Validita_Inizio_Stato")) Then
                                Dim dataCorrente = CDate(dro.Item("Validita_Inizio_Stato"))

                                If dataCorrente > maxDataStatoAttuale Then
                                    maxDataStatoAttuale = dataCorrente
                                    statoAttuale = dro.Item("Stato_Des")
                                End If
                            End If
                        Next

                        If maxDataStatoAttuale <> AGRODATAINIZIO Then
                            dr.Item("Modalita_attivazione") = statoAttuale
                        End If

                    End If
                End If

                If DtAccessi IsNot Nothing AndAlso Not IsDBNull(dr.Item("cuaa")) AndAlso dr.Item("cuaa") <> "" Then

                    'DT: Sviluppo ad hoc Coldiretti: esiste un solo utente per ogni azienda agricola, ed ha il codice fiscale coincidente con il CUAA
                    DrAccessi = DtAccessi.Select("CodFisc='" & dr.Item("cuaa") & "'")
                    If DrAccessi IsNot Nothing AndAlso DrAccessi.Length > 0 Then

                        Dim dro As DataRow = DrAccessi(0)

                        If Not IsDBNull(dro.Item("Data_PrimoAccesso")) Then
                            dr.Item("Data_primo_login") = CDate(dro.Item("Data_PrimoAccesso")).ToShortDateString
                        End If

                        If Not IsDBNull(dro.Item("Data_UltimoAccesso")) Then
                            dr.Item("Data_ultimo_login") = CDate(dro.Item("Data_UltimoAccesso")).ToShortDateString
                        End If

                    End If
                End If
            End If
        Next

        DT_Esporta_Dettagli.Columns.Remove("Piva")
        DT_Esporta_Dettagli.Columns("PivaReale").ColumnName = "Piva"

        debugHelper.DebugWriteLineElapsedMs("Loop_Aggiorna_Utenti")

        Dim nomeFile = "ElencoSintetico_Movimenti_PerAzienda"
        If Dettagli_QDC Then
            nomeFile = "ElencoSintetico_Movimenti_PerAziendaDettagli"
        End If

        nomeFile = getNomeFile(nomeFile)

        Dim complete_file_path = AgronicaCoreGestioneRichieste.Esporta.EsportaExcelPath(DT_Esporta_Dettagli, nomeFile, objAllegato:=objAllegato)

        Return Stringa_Codifica(complete_file_path, AgroKey_EncoderDecoder)

    End Function

#End Region

#Region "Report dettagli servizi"

    <WebMethod(EnableSession:=True)>
    Public Shared Function getReportDettagliServiziPerAzienda(v_inizio As String,
                                                              v_fine As String,
                                                              piva As String,
                                                              tipoFiltro As Integer
                                                              ) As RispostaStandard

        Dim r As RispostaStandard = New RispostaStandard()
        Try

            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri

            Dim isObjParametriSet = setObjParametri(objParametri_Server, objParametri_Utenti)

            If Not isObjParametriSet Then
                r.Sessione = False
                Return r
            End If

            Dim validita_inizio As Date = If(String.IsNullOrWhiteSpace(v_inizio), AGRODATAINIZIO, CDate(v_inizio))
            Dim validita_fine As Date = If(String.IsNullOrWhiteSpace(v_fine), AGRODATAFINE, CDate(v_fine))

            Dim ObjAllegato As New objAllegato
            Dim file_path = getReportDettagliServizi(validita_inizio, validita_fine, piva, tipoFiltro, objParametri_Server, objParametri_Utenti, objAllegato:=ObjAllegato)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(ObjAllegato, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    'Report dettagli servizi per azienda
    Private Shared Function getReportDettagliServizi(v_inizio As Date, v_fine As Date,
                                                     piva As String, tipoFiltro As Integer,
                                                     objParametri_Server As AgronicaCoreParametri,
                                                     objParametri_Utenti As AgronicaCoreParametri,
                                                        Optional ByRef objAllegato As objAllegato = Nothing
                                                     ) As String
        Dim username = ""
        '--------------------------------------------------------
        'AGENDA TOTALE + DISTINCT
        '--------------------------------------------------------
        Dim FiltroAggiuntivo_Imprese As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda1 As String = ""

        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

        Dim Applica_VisibilitaUtente As Boolean = False

        If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
            Applica_VisibilitaUtente = True
        End If

        'genero filtro delle piva
        If piva <> "" AndAlso piva <> objParametri_Server.PivaSuperUser Then
            Dim objger As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            objger.LeggiFigliNodoGerarchiaImprese(piva, FiltroAggiuntivo_Imprese, objParametri_Server)
            If FiltroAggiuntivo_Imprese <> "" Then
                FiltroAggiuntivo_Imprese = FiltroAggiuntivo_Imprese & ",'" & piva & "'"
            Else
                FiltroAggiuntivo_Imprese = "'" & piva & "'"
            End If
        End If

        Dim DT_Esporta_Dettagli As New DataTable
        Dim objStat As New AgronicaCoreContabDAL.Statistiche_Utilizzo_R

        Dim debugHelper As New DebugHelper()

        debugHelper.DebugWriteLine("Btn_Esporta_Dettaglio_Click - Piva: " & piva)

        debugHelper.WatchStartNew()

        DT_Esporta_Dettagli = objStat.Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod_ConUtenti(tipoFiltro,
                                                                                                v_inizio,
                                                                                                v_fine,
                                                                                                username,
                                                                                                FiltroAggiuntivo_Imprese_Agenda,
                                                                                                FiltroAggiuntivo_Imprese_Agenda1,
                                                                                                "",
                                                                                                objParametri_Server,
                                                                                                objParametri_Utenti,
                                                                                                Applica_VisibilitaUtente,
                                                                                                FiltroAggiuntivo_Imprese)

        debugHelper.DebugWriteLineElapsedMs("Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod_ConUtenti")

        DT_Esporta_Dettagli.Columns.Remove("veg_cod")

        Dim nomeFile = "ReportDettagli_Servizi_PerAzienda"
        nomeFile = getNomeFile(nomeFile)

        Dim complete_file_path = AgronicaCoreGestioneRichieste.Esporta.EsportaExcelPath(DT_Esporta_Dettagli, nomeFile, objAllegato:=objAllegato)

        Return Stringa_Codifica(complete_file_path, AgroKey_EncoderDecoder)

    End Function
#End Region

    Public Shared Function getNomeFile(nomeFile) As String
        Return nomeFile & "__" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "")
    End Function

    Private Shared Function setObjParametri(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            Return False
        End If

        objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            Return False
        End If

        Return True
    End Function

End Class