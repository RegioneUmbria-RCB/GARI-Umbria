Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreMVVCommon

Public Class StampaMVV
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument
    Private _logErrori As String = ""
    Private _catCod As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtente As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _nomeFileReport As String = "MVV.rpt"
    Private _pathRpt As String = "~/GestioneStampe/Contabilita/MVV/Report"
    Private _nomeDocumento As String

    Private _piva As String
    Private _id_Agenda As Integer

#Region " MVV "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        'istanzio l'oggetto report


    End Sub

#End Region


    '##################################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        If Not Request.QueryString("i") Is Nothing Then
            _id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))
        Else
            _id_Agenda = 0
        End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtente = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        _nomeDocumento = "StampaMVV"

        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper

        'lettura da file system
        If _nomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath(_pathRpt)
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & _nomeFileReport) Then
                _rptStampa = crHlp.getReportDaFile(pFile, _nomeFileReport)
            End If

        End If


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not IsPostBack Then

            Dim dsMVV As New DS_MVV
            Dim identificazioneDocumento As String = _nomeDocumento + "_" + _id_Agenda.ToString() + "_" + _piva.ToString()
            Dim controller = New AgronicaCoreMVVBIZ.MVVService(_objParametriServer, _objParametriUtente)

            Try

                Dim mvv = controller.PreparaDatiStampa(_piva, _id_Agenda)


                'documento
                Dim drDocNew As DS_MVV.DT_DocumentoRow = dsMVV.DT_Documento.NewDT_DocumentoRow
                drDocNew.CausaleTrasporto = mvv.Documento.CausaleTrasporto
                drDocNew.DataInizioTrasporto = mvv.Documento.DataInizioTrasporto
                drDocNew.DocNumero = mvv.ToString()
                drDocNew.OraInizioTrasporto = mvv.Documento.OraInizioTrasporto
                drDocNew.LuogoSpedizione = mvv.Documento.LuogoSpedizione
                drDocNew.Note = mvv.Note
                dsMVV.DT_Documento.Rows.Add(drDocNew)

                ' Autorita competente
                Dim drAutNew As DS_MVV.DT_AutCompRow = dsMVV.DT_AutComp.NewDT_AutCompRow
                drAutNew.CAP = mvv.AutoritaComepetente.Indirizzo.CAP
                drAutNew.Comune_Provincia = mvv.AutoritaComepetente.Indirizzo.Comune_Provincia.ToUpper()
                drAutNew.Stato = mvv.AutoritaComepetente.Indirizzo.Stato.ToUpper()
                drAutNew.UfficioICQRF = mvv.AutoritaComepetente.ICQRF
                drAutNew.Indirizzo = String.Format("{0} - {1} {2}", mvv.AutoritaComepetente.Indirizzo.Indirizzo.Trim(), mvv.AutoritaComepetente.Indirizzo.CAP, mvv.AutoritaComepetente.Indirizzo.Comune_Provincia.Trim())
                dsMVV.DT_AutComp.Rows.Add(drAutNew)

                ' speditore
                Dim drSpedNew As DS_MVV.DT_SpeditoreRow = dsMVV.DT_Speditore.NewDT_SpeditoreRow()
                drSpedNew.Denominazione = mvv.Speditore.Denominazione.ToUpper()
                drSpedNew.PIVA = mvv.Speditore.PIVA.ToUpper()
                drSpedNew.Indirizzo = mvv.Speditore.Indirizzo.Indirizzo.ToUpper()
                drSpedNew.CAP = mvv.Speditore.Indirizzo.CAP.ToUpper()
                drSpedNew.Comune_Provincia = mvv.Speditore.Indirizzo.Comune_Provincia.Trim().ToUpper()
                drSpedNew.Stato = mvv.Speditore.Indirizzo.Stato.ToUpper()
                drSpedNew.CodiceAccisa = mvv.Speditore.CodiceAccisa.ToUpper()
                dsMVV.DT_Speditore.Rows.Add(drSpedNew)

                ' destinatario
                Dim drDestNew As DS_MVV.DT_DestinatarioRow = dsMVV.DT_Destinatario.NewDT_DestinatarioRow()
                drDestNew.Denominazione = mvv.Destinatario.Denominazione.ToUpper()
                drDestNew.PIVA = mvv.Destinatario.PIVA.ToUpper()
                drDestNew.Indirizzo = mvv.Destinatario.Indirizzo.Indirizzo.ToUpper()
                drDestNew.CAP = mvv.Destinatario.Indirizzo.CAP.ToUpper()
                drDestNew.Comune_Provincia = mvv.Destinatario.Indirizzo.Comune_Provincia.Trim().ToUpper()
                drDestNew.Stato = mvv.Destinatario.Indirizzo.Stato.ToUpper()
                drDestNew.CodiceAccisa = mvv.Destinatario.CodiceAccisa.ToUpper()
                dsMVV.DT_Destinatario.Rows.Add(drDestNew)

                ' destinatario diverso
                Dim drDestDivNew As DS_MVV.DT_DestinatarioDiversoRow = dsMVV.DT_DestinatarioDiverso.NewDT_DestinatarioDiversoRow
                drDestDivNew.Denominazione = mvv.DestinatarioDiverso.Denominazione.ToUpper()
                drDestDivNew.Indirizzo = mvv.DestinatarioDiverso.Indirizzo.Indirizzo.ToUpper()
                drDestDivNew.CAP = mvv.DestinatarioDiverso.Indirizzo.CAP.ToUpper()
                drDestDivNew.Comune_Provincia = mvv.DestinatarioDiverso.Indirizzo.Comune_Provincia.Trim().ToUpper()
                drDestDivNew.Stato = mvv.DestinatarioDiverso.Indirizzo.Stato.ToUpper()
                dsMVV.DT_DestinatarioDiverso.Rows.Add(drDestDivNew)

                ' trasportatore
                Dim drTrasNew As DS_MVV.DT_VettoreRow = dsMVV.DT_Vettore.NewDT_VettoreRow()
                drTrasNew.Denominazione = mvv.Vettore.Denominazione.ToUpper()
                drTrasNew.Indirizzo = mvv.Vettore.Indirizzo.Indirizzo.ToUpper()
                drTrasNew.CAP = mvv.Vettore.Indirizzo.CAP.ToUpper()
                drTrasNew.Comune_Provincia = mvv.Vettore.Indirizzo.Comune_Provincia.Trim().ToUpper()
                drTrasNew.Stato = mvv.Vettore.Indirizzo.Stato.ToUpper()
                drTrasNew.PIVA = mvv.Vettore.PIVA.ToUpper()
                drTrasNew.Mezzo = mvv.Vettore.Mezzo
                dsMVV.DT_Vettore.Rows.Add(drTrasNew)

                ' mezzo trasporto
                Dim drMezzoTrasNew As DS_MVV.DT_MezzoTrasportoRow = dsMVV.DT_MezzoTrasporto.NewDT_MezzoTrasportoRow()
                drMezzoTrasNew.Tipo = mvv.MezzoTrasoprto.Tipo
                drMezzoTrasNew.Targa = mvv.MezzoTrasoprto.Targa
                drMezzoTrasNew.TargaRimorchio = mvv.MezzoTrasoprto.TargaRimorchio
                drMezzoTrasNew.NumAut = mvv.MezzoTrasoprto.NumeroAutorizzazione
                drMezzoTrasNew.DataAut = mvv.MezzoTrasoprto.DataAutorizzazione
                dsMVV.DT_MezzoTrasporto.Rows.Add(drMezzoTrasNew)

                ' dettagli
                Dim drDettaglioNew As DS_MVV.DT_DettagliRow

                For Each d As MVV_Dettaglio In mvv.Dettagli
                    drDettaglioNew = dsMVV.DT_Dettagli.NewDT_DettagliRow()
                    drDettaglioNew.CodCategoria = d.CodCategoria
                    drDettaglioNew.CodiceNC = d.CodiceNC
                    drDettaglioNew.CodiceZonaVit = d.CodiceZonaVit
                    drDettaglioNew.CodiceOperazioneVit = d.CodiceOperazioneVit
                    drDettaglioNew.Descrizione = d.ToString()
                    'drDettaglioNew.TitoloAlcol = d.TitoloAlcol
                    drDettaglioNew.TitoloAlcolTot = d.TitoloAlcolTot
                    drDettaglioNew.TitoloAlcolPot = d.TitoloAlcolPot
                    drDettaglioNew.TitoloAlcolEff = d.TitoloAlcolEff
                    drDettaglioNew.Densita = d.Densita
                    drDettaglioNew.NumeroColliImb = If(d.UsaColli, d.NumeroColli, d.NumeroImballi)
                    drDettaglioNew.DescrizioneColliImb = If(d.UsaColli, d.DescrizioneColli, d.DescrizioneImballi)
                    drDettaglioNew.UnitaMisura = d.UnitaMisura
                    drDettaglioNew.Quantita = d.Quantita
                    dsMVV.DT_Dettagli.Rows.Add(drDettaglioNew)
                Next

                '' impostazione datasource
                _rptStampa.SetDataSource(dsMVV)

            Catch ex As Exception
                If Not ex Is Nothing Then
                    SalvaLogErrori(ex.Message, _nomeDocumento, identificazioneDocumento)
                End If
            End Try

            'MS La nuova gestione salva il report con i dati su disco con un nome univoco per ogni esecuzione di stampa e passa a
            'VisualizzatoreReport il nome del pathname da riaprire.
            'Questi files temporanei sono cancellati periodicamente in entrata su GestioneRichieste e non in VisualizzatoreReport perché
            'possono essere eseguite una serie di PostBack es. export su pdf e il report deve rimanere disponibile.
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                SalvaLogErrori(ex.Message, _nomeDocumento, identificazioneDocumento)
            End Try

            Try
                'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
                'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.
                dsMVV.Dispose()
                dsMVV = Nothing

                _rptStampa.Close()
                _rptStampa.Dispose()
                _rptStampa = Nothing

                GC.Collect()

            Catch ex As Exception
                If Not ex Is Nothing Then
                    SalvaLogErrori(ex.Message, _nomeDocumento, identificazioneDocumento)
                End If
            End Try

            'MS Il controllo passa a VisualizzatoreReport passando in QueryString il pathname del report temporaneo su disco in modo che 
            'rimanga disponibile anche nelle PostBack per esportazione ecc.. 
            'Non si può passare sulla session altrimenti l'esecuzione di 2 o più report dalla stessa postazione andrebbe in conflitto
            'Per discriminare fra vecchio e nuovo giro il visualizzatore testa se in QueryString viene passato tmpReportPath

            Dim urlRedirect = "..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                              "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server)

            Response.Redirect(urlRedirect)


        End If


    End Sub

    Private Sub SalvaLogErrori(logErrori As String, nomeDocumento As String, identificazioneDocumento As String)
        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim nomeFile As String

        If logErrori <> "" Then

            logErrori = nomeDocumento & vbCrLf & vbCrLf & logErrori

            nomeFile = "Log_Errori_" & identificazioneDocumento & CStr(Session("ASG_Utente_Username"))

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer, _
                                             "Stampe_Contabilita", _
                                             nomeFile & ".txt", _
                                             Session("ASG_Utente_Username"), _
                                             "StampaMVV.aspx", _
                                             logErrori)
        End If

    End Sub

End Class
