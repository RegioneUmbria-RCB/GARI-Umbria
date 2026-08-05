

Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreFiltroneBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello.ParametriInvestimentoCatasto_Temp
Imports System.Runtime.Remoting
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello

Public Class InvestimentoCatasto
    Inherits System.Web.UI.Page


#Region "Metodi Web"


    <WebMethod(EnableSession:=True)>
    Public Shared Function InvestimentoCatastoElabora(
        piva As String,
        txtDataIniDist As Date,
        txtDataFinDist As Date,
        mostraRipartoAppezzamenti As Boolean,
        mostraAppezzaSenzaRiparto As Boolean,
        mostraParticelleSenzaRiparto As Boolean,
        selezioneSuiCampi As Boolean,
        sintesiCUAAEstremiCatastali As Boolean,
        tuttoIlCatastoInArchivio As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r.RispostaOK = True

            Dim idTestataTemp As Integer
            Dim objParametriInvestimentoCatasto As New ParametriInvestimentoCatasto
            Dim objParametriAgenda As New ParametriAgenda

            If selezioneSuiCampi Then
                idTestataTemp = PopolaFiltroCampi(objParametriInvestimentoCatasto, objParametri_Server)
            Else
                idTestataTemp = PopolaFiltroImpianti(objParametriAgenda, objParametri_Server)
            End If

            If idTestataTemp > 0 Then
                piva = ""
            End If

            Dim xLeggiBiz As New AgronicaCoreFiltroneBIZ.Filtrone
            Dim dtRiparto As New DataTable

            If Not selezioneSuiCampi Then
                If objParametriAgenda.Impianti.Count > 0 Then
                    dtRiparto = xLeggiBiz.DT_to_AppezzamentoRipartoCatasto(
                        piva,
                        mostraRipartoAppezzamenti:=mostraRipartoAppezzamenti,
                        mostraAppezzamentoSenzaRiparto:=mostraAppezzaSenzaRiparto,
                        mostraParticelleSenzaRiparto:=mostraParticelleSenzaRiparto,
                        DataDa:=txtDataIniDist.Date(),
                        DataA:=txtDataFinDist.Date(),
                        IDTestataTemp:=idTestataTemp,
                        dtFiltroAppezza:=Nothing,
                        objParametri_Server:=objParametri_Server,
                        objParametri_utenti:=objParametri_Utenti)
                End If
            Else
                If objParametriInvestimentoCatasto.Campi.Count > 0 OrElse tuttoIlCatastoInArchivio Then
                    dtRiparto = xLeggiBiz.DT_to_CampoRipartoCatasto(
                        piva,
                        campiConRiparto:=mostraRipartoAppezzamenti,
                        campiSenzaRiparto:=mostraAppezzaSenzaRiparto,
                        particelleInConduzioneSenzaRipartoSuiCampi:=mostraParticelleSenzaRiparto,
                        DataDa:=txtDataIniDist.Date(),
                        DataA:=txtDataFinDist.Date(),
                        IDTestataTemp:=idTestataTemp,
                        dtFiltroCampi:=Nothing,
                        tuttoIlCatastoInArchivio:=tuttoIlCatastoInArchivio,
                        sintesiCUAAEstremiCatastali,
                        objParametri_Server:=objParametri_Server,
                        objParametri_utenti:=objParametri_Utenti)
                End If
            End If

            If sintesiCUAAEstremiCatastali Then
                Dim invCat As New InvestimentoCatasto
                Dim dt As New DataTable

                invCat.RaggruppaPerParticelle(dt, dtRiparto, selezioneSuiCampi)
                If Not selezioneSuiCampi Then
                    r.RispostaStringa = AgronicaCoreFiltroneBIZ.Filtrone.DT_to_Json_AppezzamentoRipartoCatasto(dt, True)
                Else
                    r.RispostaStringa = AgronicaCoreFiltroneBIZ.Filtrone.DT_to_Json_CampoRipartoCatasto(dt, tuttoIlCatastoInArchivio, True)
                End If
            Else
                If Not selezioneSuiCampi Then
                    r.RispostaStringa = AgronicaCoreFiltroneBIZ.Filtrone.DT_to_Json_AppezzamentoRipartoCatasto(dtRiparto)
                Else
                    r.RispostaStringa = AgronicaCoreFiltroneBIZ.Filtrone.DT_to_Json_CampoRipartoCatasto(dtRiparto, tuttoIlCatastoInArchivio)
                End If
            End If


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Function PopolaFiltroImpianti(objParametriAgenda As ParametriAgenda, objParametri_Server As AgronicaCoreParametri) As Integer

        Dim numeroAziende As Integer = (From ii In objParametriAgenda.Impianti Distinct Select ii.Piva).Distinct.Count
        Dim numeroImpianti As Integer = objParametriAgenda.Impianti.Count

        Dim IDTestataTemp As Integer = 0

        If numeroImpianti > 0 Then

            Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

            Dim OperazioneCorrente_FiltroImpianti As String = ""

            FiltroneImpiantiDbTemp.OperazioneCorrente_EstraiFiltrone(IDTestataTemp, objParametriAgenda.Impianti, OperazioneCorrente_FiltroImpianti)
            FiltroneImpiantiDbTemp.PopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri_Server)

        End If

        Return IDTestataTemp

    End Function

    Private Shared Function PopolaFiltroCampi(objParametriInvestimentoCatasto As ParametriInvestimentoCatasto, objParametri_Server As AgronicaCoreParametri) As Integer

        Dim numeroAziende As Integer = (From ii In objParametriInvestimentoCatasto.Campi Distinct Select ii.Piva).Distinct.Count
        Dim numeroImpianti As Integer = objParametriInvestimentoCatasto.Campi.Count

        Dim IDTestataTemp As Integer = 0

        If numeroImpianti > 0 Then

            Dim xAgrosequenze As New Agro_Sequenze

            'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

            Dim OperazioneCorrente_FiltroCampi As String = ""

            FiltroneCampiDbTemp.OperazioneCorrente_EstraiFiltrone(IDTestataTemp, objParametriInvestimentoCatasto.Campi, OperazioneCorrente_FiltroCampi)
            FiltroneCampiDbTemp.PopolaTabellaFiltroCampi(OperazioneCorrente_FiltroCampi, objParametri_Server)

        End If

        Return IDTestataTemp

    End Function

    Private Function DT_to_AppezzamentoRipartoCatasto_ScriviFiltro(dtFiltroAppezza As DataTable, objParametri_Server As AgronicaCoreParametri) As Integer

        Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        'Dim cliIDTestataTemp As Integer = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
        Dim cliIDTestataTemp As Integer = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

        Dim OperazioneCorrente_FiltroImpianti As String = "insert into __tmp_FiltroImpianti (piva, sa_cod, appezza, id_reg, IDTestataTemp) values "

        Dim listaFiltroImpianti As New List(Of String)
        For Each rr As DataRow In dtFiltroAppezza.Rows

            Dim ci = rr("Chiave").Split("_")

            Dim daAggiungere As String = "('" &
            ci(0) & "'," &
            ci(1) & "," &
            ci(2) & ", 0, " &
            cliIDTestataTemp &
            ")"

            If Not listaFiltroImpianti.Contains(daAggiungere) Then
                listaFiltroImpianti.Add(daAggiungere)
            End If

        Next

        OperazioneCorrente_FiltroImpianti &= String.Join(",", listaFiltroImpianti)

        If dtFiltroAppezza.Rows.Count > 0 Then
            AgronicaCoreFiltroneBIZ.Filtrone.PopolaTabellaFiltro(OperazioneCorrente_FiltroImpianti, objParametri_Server)
        End If
        Return cliIDTestataTemp
    End Function


#End Region

    Private objParametriAgenda As ParametriAgenda
    Private objParametriInvestimentoCatasto As ParametriInvestimentoCatasto

    Public piva As String
    Public avviaAuto As String
    Public descrizioneFiltroSemplice As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametriAgenda = New ParametriAgenda
        objParametriInvestimentoCatasto = New ParametriInvestimentoCatasto
        piva = objParametriAgenda.Piva

        Dim provenienzaDaVerificaDP As Boolean = Not Session("Elemento_Verifica_Disciplinare") Is Nothing
        If provenienzaDaVerificaDP Then
            avviaAuto = "true"
        Else
            avviaAuto = "false"
        End If

        Dim permessi As New PermessiUtente
        Dim xP As Boolean = permessi.getPermesso(enum_Security_Attivita.Gest_Stampe).Scrittura

        If xP Then
            'divBtnElaboraDati.Visible = True
            enableBtnElaboraDati.Value = True
        Else
            'divBtnElaboraDati.Visible = provenienzaDaVerificaDP
            enableBtnElaboraDati.Value = provenienzaDaVerificaDP
        End If

        Dim numeroAziende As Integer = 0
        Dim numeroImpianti As Integer = 0
        Dim numeroMovimenti As Integer = 0
        Dim numeroCampi As Integer = 0
        If objParametriAgenda.Impianti.Count > 0 Then
            numeroAziende = (From ii In objParametriAgenda.Impianti Distinct Select ii.Piva).Distinct.Count
        Else
            numeroAziende = (From ii In objParametriInvestimentoCatasto.Campi Distinct Select ii.Piva).Distinct.Count
        End If

        If objParametriAgenda.Impianti.Count > 0 Then
            numeroImpianti = objParametriAgenda.Impianti.Count
            numeroMovimenti = objParametriAgenda.Movimenti.Count
        Else
            numeroCampi = objParametriInvestimentoCatasto.Campi.Count
        End If

        Dim RagSocImpresaSelezionata As String = ""
        If objParametriAgenda.Impianti.Count > 0 Then
            RagSocImpresaSelezionata = objParametriAgenda.RagSoc.Replace("""", "\""")
        Else
            RagSocImpresaSelezionata = objParametriInvestimentoCatasto.RagSoc.Replace("""", "\""")
        End If

        descrizioneFiltroSemplice = "Filtro impostato su Impresa: " & RagSocImpresaSelezionata

        Dim usaFiltroRicercaNG As Boolean

        If Not Page.IsPostBack Then
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            usaFiltroRicercaNG = objFiltroRicerca.usaFiltroRicercaNG(objParametri_Utenti)
            hd_usaFiltroRicercaNG.Value = usaFiltroRicercaNG
            hd_CurrentPiva.Value = objParametriAgenda.Piva
        End If

        Dim filtroneImpostato As Boolean
        If String.IsNullOrEmpty(Request.QueryString("f")) Then
            filtroneImpostato = usaFiltroRicercaNG And (numeroImpianti <> 0 Or numeroCampi <> 0)
        Else
            filtroneImpostato = True
        End If

        If String.IsNullOrEmpty(RagSocImpresaSelezionata) And (Not filtroneImpostato) Then
            lblFiltroImpostato.Text = "Selezionare un Azienda oppure una lista di impianti o campi."
            enableBtnElaboraDati.Value = False
            'divBtnElaboraDati.Visible = True
        Else
            If filtroneImpostato Then
                If numeroImpianti > 0 Then
                    lblFiltroImpostato.Text = "Filtro Avanzato: N. Aziende: " & numeroAziende & ", N. Impianti: " & numeroImpianti
                ElseIf numeroAziende > 0 Then
                    lblFiltroImpostato.Text = "Filtro Avanzato: N. Aziende: " & numeroAziende & ", N. Campi: " & numeroCampi
                End If

                If numeroMovimenti > 0 Then
                    lblFiltroImpostato.Text = "Filtro Avanzato: N. Aziende: " & numeroAziende & ", N. Operazioni: " & numeroMovimenti
                End If
            Else
                lblFiltroImpostato.Text = descrizioneFiltroSemplice
            End If
        End If

    End Sub

    Private Sub Btn_FiltraImpianti_Click(sender As Object, e As EventArgs) Handles Btn_FiltraImpianti.Click

        Dim cat = estrazionePerCampi.Value
        objParametriAgenda.Impianti.Clear()
        objParametriInvestimentoCatasto.Campi.Clear()

        Dim TargetRedirect As String

        TargetRedirect = "../../Filtrone/Filtrone_nuovo.aspx?p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DuplicaOperazione, AgroKey_EncoderDecoder, Server) &
            "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
            "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_InvestimentoCatasto, AgroKey_EncoderDecoder, Server) &
            "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Server) &
            "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.OperazioniMultiAziendali, AgroKey_EncoderDecoder, Server) &
            "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
            "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
            "&cat=" & Stringa_Codifica(cat, AgroKey_EncoderDecoder, Server)

        Response.Redirect(TargetRedirect)

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_FiltroRicercaNG(piva As String, cat As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objParametriAgenda As New ParametriAgenda
            Dim objFiltroRicerca As New AgronicaCoreFiltroneBIZ.FiltroRicerca

            Dim enumTipoMostra As Enum_TipoMostra_FiltroRicerca
            If cat = "" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Impianti
            ElseIf cat = "azienda" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende
            ElseIf cat = "centro" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.CentriAziendali
            ElseIf cat = "appezzamento" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Appezzamenti
            ElseIf cat = "impianto" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Impianti
            ElseIf cat = "movimento" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Movimenti
            ElseIf cat = "campo" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Campi
            ElseIf cat = "esercizio" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Esercizi
            ElseIf cat = "fabbricato" Then
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Fabbricati
            Else
                enumTipoMostra = Enum_TipoMostra_FiltroRicerca.Impianti
            End If

            Dim parametriFiltroRicercaNG As New ParametriFiltroRicercaNG With {
                            .PaginaProvenienza = enum_PagineAgenda_2010.Pagina_InvestimentoCatasto,
                            .Piva = objParametriAgenda.Piva,
                            .TipoMostraGestitiChiamante = New List(Of Enum_TipoMostra_FiltroRicerca) From {enumTipoMostra},
                            .TipoComportamentoFiltroRicercaNG = Enum_TipoComportamento_FiltroRicerca.SelezionamentoEntita,
                            .SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            .FiltriTemporali = objFiltroRicerca.Imposta_FiltroEntitaAttivaAllaData(Date.Now, Enum_Entita_FiltroRicerca.Esercizio)
                }

            r.RispostaStringa = objFiltroRicerca.Link_Pagina_FiltroRicercaNG(piva, parametriFiltroRicercaNG)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaEntitaDaChiavi(chiavi As List(Of String), tipoEntita As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim entita As Integer = CInt(tipoEntita)

        Try

            Dim objParametriAgenda As New ParametriAgenda
            Dim objParametriInvestimentoCatasto As New ParametriInvestimentoCatasto

            objParametriAgenda.Impianti.Clear()
            objParametriInvestimentoCatasto.Campi.Clear()

            If (entita = Enum_TipoMostra_FiltroRicerca.Impianti) Then

                For Each chiave As String In chiavi
                    Dim objImpianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                    Dim piva As String = chiave.Split("_")(0)
                    Dim sa_cod As String = chiave.Split("_")(1)
                    Dim appezza As String = chiave.Split("_")(2)
                    Dim id_reg As String = chiave.Split("_")(3)
                    Dim veg_cod As String = chiave.Split("_")(4)
                    Dim progetto_cod As String = chiave.Split("_")(5)

                    objImpianto.Piva = piva
                    objImpianto.Sa_Cod = sa_cod
                    objImpianto.Appezza = appezza
                    objImpianto.ID_Reg = id_reg
                    objImpianto.Veg_Cod = veg_cod
                    objImpianto.Progetto_Cod = progetto_cod

                    objParametriAgenda.Impianti.Add(objImpianto)
                Next

            ElseIf (entita = Enum_TipoMostra_FiltroRicerca.Campi) Then

                For Each chiave As String In chiavi

                    Dim campo As New ParametriInvestimentoCatasto_Temp.Campo
                    Dim piva As String = chiave.Split("_")(0)
                    Dim sa_cod As String = chiave.Split("_")(1)
                    Dim campo_cod As String = chiave.Split("_")(2)

                    campo.Piva = piva
                    campo.Sa_Cod = sa_cod
                    campo.Campo_Cod = campo_cod

                    objParametriInvestimentoCatasto.Campi.Add(campo)
                Next

            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Sub RaggruppaPerParticelle(ByRef dt As DataTable, dtRiparto As DataTable, ByVal campi As Boolean)
        Try
            addColumnsToDt(dtRiparto)
            dt = dtRiparto.Clone()

            Dim dtRow As DataRow
            Dim Piva = ""
            Dim Prov As String = ""
            Dim Com As String = ""
            Dim sezione As String = ""
            Dim Foglio As Integer = 0
            Dim Numero As Integer = 0
            Dim subalterno As String = ""

            For Each row In dtRiparto.Rows
                If row("PIVA") = Piva AndAlso
                    row("PROV") = Prov AndAlso
                    row("COM") = Com AndAlso
                    row("SEZIONE") = sezione AndAlso
                    row("FOGLIO") = Foglio AndAlso
                    row("NUMERO") = Numero AndAlso
                    row("SUBALTERNO") = subalterno AndAlso
                    Not campi Then

                    dtRow("Sup_imp") += row("Sup_imp")
                    dtRow("sup_app") += row("sup_app")

                ElseIf Not campi Then

                    If Not isEmpty(dtRow) Then
                        setEttariAreCentiare(dtRow, campi)
                        dt.ImportRow(dtRow)
                    End If

                    dtRow = row
                    Piva = row("PIVA")
                    Prov = row("PROV")
                    Com = row("COM")
                    sezione = row("SEZIONE")
                    Foglio = row("FOGLIO")
                    Numero = row("NUMERO")
                    subalterno = row("SUBALTERNO")
                End If

                If campi Then
                    dtRow = row
                    setEttariAreCentiare(dtRow, campi)
                    dt.ImportRow(dtRow)
                End If
            Next

            'necessario, altrimenti l'ultimo inserimento per appezzamenti non viene effettuato
            If Not isEmpty(dtRow) AndAlso Not campi Then
                setEttariAreCentiare(dtRow, campi)
                dt.ImportRow(dtRow)
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function isEmpty(dr As DataRow) As Boolean
        If dr Is Nothing Then
            Return True
        Else
            For Each value In dr.ItemArray
                If value IsNot Nothing Then
                    Return False
                End If
            Next
            Return True
        End If
    End Function

    Private Sub addColumnsToDt(ByRef dt As DataTable)
        Try
            dt.Columns.Add(New DataColumn("sup_app_Ha", GetType(Integer)))
            dt.Columns.Add(New DataColumn("sup_app_Are", GetType(Integer)))
            dt.Columns.Add(New DataColumn("sup_app_Centiare", GetType(Integer)))
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Sub setEttariAreCentiare(ByRef dtRow As DataRow, ByVal campi As Boolean)
        Dim Ha As Integer
        Dim Are As Integer
        Dim Centiare As Integer
        Dim sup As Double

        Try
            If Not campi Then
                sup = dtRow("sup_app")
                UtilityProvider.EttariAreCentiare_from_Ettari(sup, Ha, Are, Centiare)

                If IsDBNull(dtRow("sup_app_Ha")) Then
                    dtRow("sup_app_Ha") = 0
                End If
                If IsDBNull(dtRow("sup_app_Are")) Then
                    dtRow("sup_app_Are") = 0
                End If
                If IsDBNull(dtRow("sup_app_Centiare")) Then
                    dtRow("sup_app_Centiare") = 0
                End If

                dtRow("sup_app_Ha") = Ha
                dtRow("sup_app_Are") = Are
                dtRow("sup_app_Centiare") = Centiare
            Else
                sup = dtRow("SemTrap_Superficie")
                UtilityProvider.EttariAreCentiare_from_Ettari(sup, Ha, Are, Centiare)

                If IsDBNull(dtRow("SemTrap_Ha")) Then
                    dtRow("SemTrap_Ha") = 0
                End If
                If IsDBNull(dtRow("SemTrap_Are")) Then
                    dtRow("SemTrap_Are") = 0
                End If
                If IsDBNull(dtRow("SemTrap_Centiare")) Then
                    dtRow("SemTrap_Centiare") = 0
                End If

                dtRow("SemTrap_Ha") = Ha
                dtRow("SemTrap_Are") = Are
                dtRow("SemTrap_Centiare") = Centiare
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#Region "Viste personalizzate"

    'salvataggio parametri vista
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaVista(ByVal nome As String, ByVal vista As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteVista As Integer = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_VISTE_INVESTIMENTO_CATASTO

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteVista, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaViste As New JArray()
            Dim jObjectVista = JsonConvert.DeserializeObject(vista)
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    If impostazioni("nome") = nome Then
                        jArrayListaViste.Add(jObjectVista)
                        trovato = True
                    Else
                        jArrayListaViste.Add(impostazioni)
                    End If
                Next
            End If
            If Not trovato Then
                jArrayListaViste.Add(jObjectVista)
            End If

            Dim impostazioniReport = JsonConvert.SerializeObject(jArrayListaViste, Formatting.None)

            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(impostazioneUtenteVista, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(impostazioneUtenteVista, impostazioniReport, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

            If r.RispostaOK = True Then
                r.RispostaStringa = "true"
            Else
                r.RispostaStringa = "Problema nel salvataggio della vista"
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'legge lista viste dell'utente
    <WebMethod(EnableSession:=True)>
    Public Shared Function ListaViste(ByVal categoriaVista As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteVista As Integer = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_VISTE_INVESTIMENTO_CATASTO

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteVista, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaViste As New JArray()
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    jArrayListaViste.Add(New JObject(New JProperty("cod", impostazioni("nome")), New JProperty("desc", impostazioni("nome"))))
                Next
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaViste, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    'legge i parametri della vista selezionata
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiVista(ByVal nome As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteVista As Integer = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_VISTE_INVESTIMENTO_CATASTO

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteVista, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim trovato As Boolean = False
            Dim jArrayListaViste As New JArray()
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

    'elimina la vista selezionato
    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaVista(ByVal nome As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim impostazioneUtenteVista As Integer = enum_Impostazioni_Utenti.UTENTE_COD_PREFERITI_VISTE_INVESTIMENTO_CATASTO

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(impostazioneUtenteVista, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim jArrayListaViste As New JArray()
            If dtImpostazioni.Rows.Count > 0 Then
                Dim jArrayListaImpostazioni = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each impostazioni In jArrayListaImpostazioni
                    If impostazioni("nome") <> nome Then
                        jArrayListaViste.Add(impostazioni)
                    End If
                Next
            End If

            Dim impostazioniVista = JsonConvert.SerializeObject(jArrayListaViste, Formatting.None)

            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(impostazioneUtenteVista, "", objParametri_Utenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(impostazioneUtenteVista, impostazioniVista, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)

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



End Class