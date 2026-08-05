Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal

Public Class VerbaleIstruttoriaRichCarb
    Inherits System.Web.UI.Page

    Private rptIstruttoriaRichCarb As Rpt_VerbaleIstruttoriaRichCarb

    Private Qs_Piva As String
    Private Qs_CodRichiestaTestata As Integer
    Private Qs_ModificaDati As Boolean
    Private Qs_SegnalazioniMacchine As Boolean
    Private Qs_Esito As Integer
    Private Qs_NoteEsito As String

    Private Log_Errori As String

    Private Dim objParametri_Server As New AgronicaCoreParametri
    Private Dim objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""

    Private Tipo_Azienda As Integer
    Private isLavorazioneParzialeRichiesta As Boolean = False

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
        rptIstruttoriaRichCarb = New Rpt_VerbaleIstruttoriaRichCarb
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Qs_Piva = Stringa_Decodifica(Request.QueryString("p"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_CodRichiestaTestata = Stringa_Decodifica(Request.QueryString("crt"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_ModificaDati = Stringa_Decodifica(Request.QueryString("md"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_SegnalazioniMacchine = Stringa_Decodifica(Request.QueryString("sm"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Esito = Stringa_Decodifica(Request.QueryString("e"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_NoteEsito = Stringa_Decodifica(Request.QueryString("ne"),
                                   AgroKey_EncoderDecoder,
                                   Server)



        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "VerbaleIstruttoriaRichCarb"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_IstruttoriaCarb()

            Catch ex As Exception
                Log_Errori &= "- Lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try


            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                Nome_Documento &= " " & nomeDocIdentifPratica
                Dim Nome_Documento_Estensione = Nome_Documento & ".pdf"

                'Salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptIstruttoriaRichCarb,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)

                rptIstruttoriaRichCarb.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

                Nome_File_Log = "Log_Errori_" & Nome_Documento & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "CarburantiUMA",
                                                 Nome_File_Log,
                                                 Session("ASG_Utente_Username"),
                                                 Nome_Documento,
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                            "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If

    End Sub

    Private Sub Stampa_IstruttoriaCarb()
        'DataSet Report
        Dim dsIstruttoria As New DS_VerbaleIstruttoriaRichCarb()
        Dim dsRimanenzeInutilizzo As New DS_GestioneRimanenzeInutilizzo

        'Datatable Query
        Dim dtImpresa As New DataTable
        Dim dtCompilatore As New DataTable
        Dim dtRichiestePratiche As New DataTable
        Dim drRichiestaCorr As DataRow = Nothing
        Dim richiestaAnno As Integer
        Dim richiestaNumero As Integer
        Dim drRendicontazioneAnnoPrecProprio As DataRow = Nothing
        Dim drRendicontazioneAnnoPrecTerzi As DataRow = Nothing
        Dim totaliCarburante As New List(Of CarburanteHelper)
        Dim dtUmaSetup As New DataTable
        '''0 = non abilitata, 1 = abilitata, 2 = abilitata solo rendicontazioni
        Dim setupGestioneRimanenze As Integer
        Dim flagRichiestaProprio As Boolean = False

        'Oggetti BIZ e DAL
        Dim handleImprese As New Imprese_Read()
        Dim handleCompilatore As New Utenti_xGruppi_Utente_R()
        Dim handleRichiesteTestate As New UMA_Richieste_Testata_R()
        Dim handleBizUmaRichieste As New UMA_Richieste()
        Dim handleUmaSetup As New UMASetup_R()
        Dim helperReport As New HelperStampeCarburantiUMA(objParametri_Server, objParametri_Utenti)

        '-----------------------------------------
        '---- Query di lettura  -------------
        '----------------------------------------- 
        Try
            'Ottengo i dati dell'azienda
            dtImpresa = handleImprese.DatiIntestazioneImpresa(Qs_Piva, "", "", objParametri_Server)

            'Ottengo i dati del compilatore
            dtCompilatore = handleCompilatore.LeggiUtenti(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)

            'Ottengo i dati della testata della richiesta e relativi numero/anno pratica
            dtRichiestePratiche = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, False)
            Dim dtRichiestePraticheT = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, True)
            dtRichiestePratiche.Merge(dtRichiestePraticheT)

            drRichiestaCorr = dtRichiestePratiche.Select(
                String.Format("Richiesta_Cod = {0}", Qs_CodRichiestaTestata)
            ).FirstOrDefault()
            richiestaAnno = drRichiestaCorr("Anno")
            richiestaNumero = drRichiestaCorr("Numero")
            nomeDocIdentifPratica = richiestaAnno & "-" & richiestaNumero
            Tipo_Azienda = drRichiestaCorr.Field(Of Integer)("Tipo_Azienda")

            If Tipo_Azienda = enum_TipoAzienda_UMA.Azienda_Terzista AndAlso drRichiestaCorr.Field(Of Integer)("Avanzamento_Richiesta") = enum_UMA_Avanzamento.Richiesta Then
                isLavorazioneParzialeRichiesta = True
            End If

            flagRichiestaProprio = drRichiestaCorr.Field(Of Integer)("Tipo_Richiesta") = 0

            drRendicontazioneAnnoPrecProprio = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = 0 And Anno = " & richiestaAnno - 1
            ).FirstOrDefault()
            drRendicontazioneAnnoPrecTerzi = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = -1 And Anno = " & richiestaAnno - 1
            ).FirstOrDefault()

            'Ottengo i dati di riepilogo dei carburanti usati per le lavorazioni della richiesta
            totaliCarburante = handleBizUmaRichieste.LeggiTotaliCarburantePerTipo(Qs_CodRichiestaTestata, objParametri_Server, isLavorazioneParzialeRichiesta)

            dtUmaSetup = handleUmaSetup.LeggiSetup(richiestaAnno, objParametri_Server)
            setupGestioneRimanenze = dtUmaSetup.Rows(0)("Gestione_Rimanenze")

        Catch ex As Exception
            Log_Errori &= "query Stampa_IstruttoriaCarb: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        End Try

        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------
        Try


            Dim drIstruttoria = dsIstruttoria.DT_VerbaleIstruttoriaRichCarb.NewDT_VerbaleIstruttoriaRichCarbRow

            drIstruttoria.ComunitaMontanaDen = "AFOR"
            drIstruttoria.StrutturaDesc = ""
            drIstruttoria.CompilatoreDen = dtCompilatore.Rows(0)("Dettagli")
            drIstruttoria.RichiestaNumero = richiestaNumero
            drIstruttoria.RichiestaAnno = richiestaAnno
            drIstruttoria.ImpresaRagSoc = dtImpresa.Rows(0)("rag_soc")
            drIstruttoria.ImpresaCuaa = dtImpresa.Rows(0)("codice_cuaa")
            drIstruttoria.DataFineAnnoPrec = New Date(richiestaAnno - 1, 12, 31)
            drIstruttoria.DataIstruttoria = Date.Now

            drIstruttoria.ModDatiRichiesta = IIf(Qs_ModificaDati = True, "SI", "NO")
            drIstruttoria.SegnalazioniMacchine = IIf(Qs_SegnalazioniMacchine = True, "SI", "NO")

            Dim carbHelperDefault As New CarburanteHelper() With {
                .Tipo = 0, .Calcolato = 0, .Richiesto = 0, .Assegnato = 0
            }

            Dim carbRiepilogoDefault As New RiepilogoStampeCarbUMA()

            'If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
            '    drIstruttoria.AnnoPrecRimanenzeProprioBenzina = drRendicontazioneAnnoPrecProprio("Rimanenza_Benzina")
            '    drIstruttoria.AnnoPrecRimanenzeProprioGasolio = drRendicontazioneAnnoPrecProprio("Rimanenza_Gasolio")
            '    drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra = drRendicontazioneAnnoPrecProprio("Rimanenza_Gasolio_Serra")
            '    drIstruttoria.AnnoPrecRimanenzeProprioTotale = drIstruttoria.AnnoPrecRimanenzeProprioBenzina +
            '        drIstruttoria.AnnoPrecRimanenzeProprioGasolio + drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra

            'Else
            '    drIstruttoria.AnnoPrecRimanenzeProprioBenzina = 0
            '    drIstruttoria.AnnoPrecRimanenzeProprioGasolio = 0
            '    drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra = 0
            '    drIstruttoria.AnnoPrecRimanenzeProprioTotale = 0

            'End If

            'If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
            '    drIstruttoria.AnnoPrecRimanenzeTerziBenzina = drRendicontazioneAnnoPrecTerzi("Rimanenza_Benzina")
            '    drIstruttoria.AnnoPrecRimanenzeTerziGasolio = drRendicontazioneAnnoPrecTerzi("Rimanenza_Gasolio")
            '    drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra = drRendicontazioneAnnoPrecTerzi("Rimanenza_Gasolio_Serra")
            '    drIstruttoria.AnnoPrecRimanenzeTerziTotale = drIstruttoria.AnnoPrecRimanenzeTerziBenzina +
            '        drIstruttoria.AnnoPrecRimanenzeTerziGasolio + drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra

            'Else
            '    drIstruttoria.AnnoPrecRimanenzeTerziBenzina = 0
            '    drIstruttoria.AnnoPrecRimanenzeTerziGasolio = 0
            '    drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra = 0
            '    drIstruttoria.AnnoPrecRimanenzeTerziTotale = 0
            'End If

            Dim listaRiepiloghiCarb As New List(Of RiepilogoStampeCarbUMA)()

            If setupGestioneRimanenze = 1 Then
                listaRiepiloghiCarb = helperReport.ValorizzaSR_Inutilizzati_IstruttoriaRich(rptIstruttoriaRichCarb, dsRimanenzeInutilizzo,
                    drRichiestaCorr, Qs_Piva, Qs_CodRichiestaTestata, flagRichiestaProprio,
                    drIstruttoria.DataIstruttoria)
            Else
                rptIstruttoriaRichCarb.HeaderSectionGestRimInu.SectionFormat.EnableSuppress = True
            End If

            Dim carbBenz = totaliCarburante.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            Dim carbGas = totaliCarburante.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
            Dim carbSer = totaliCarburante.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato

            If Not flagRichiestaProprio Then

                'Per le richieste terzisti ho delle colonne specifiche per i totali di carburante, perché le aziende che
                'lavorano in conto terzi possono richiedere dei quantitativi di carburante senza specificare le lavorazioni
                'che andranno ad effettuare in previsione nell'anno

                If Not drRichiestaCorr("Approvazione_Iniziale_Benzina").Equals(DBNull.Value) Then
                    carbBenz = drRichiestaCorr("Approvazione_Iniziale_Benzina")
                End If

                If Not drRichiestaCorr("Approvazione_Iniziale_Gasolio").Equals(DBNull.Value) Then
                    carbGas = drRichiestaCorr("Approvazione_Iniziale_Gasolio")
                End If

                If Not drRichiestaCorr("Approvazione_Iniziale_Gasolio_Serra").Equals(DBNull.Value) Then
                    carbSer = drRichiestaCorr("Approvazione_Iniziale_Gasolio_Serra")
                End If

            End If

            Dim carbBenzNetto = If(carbBenz > 0, carbBenz - drRichiestaCorr("Rimanenza_Benzina"), 0)
            Dim carbGasNetto = If(carbGas > 0, carbGas - drRichiestaCorr("Rimanenza_Gasolio"), 0)
            Dim carbSerNetto = If(carbSer > 0, carbSer - drRichiestaCorr("Rimanenza_Gasolio_Serra"), 0)

            If listaRiepiloghiCarb.Count > 0 Then

                Dim benzRiep = listaRiepiloghiCarb.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbRiepilogoDefault).First()
                carbBenzNetto += benzRiep.Trasferimenti + benzRiep.Restituzioni + benzRiep.Accise

                Dim gasRiep = listaRiepiloghiCarb.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbRiepilogoDefault).First()
                carbGasNetto += gasRiep.Trasferimenti + gasRiep.Restituzioni + gasRiep.Accise

                Dim serRiep = listaRiepiloghiCarb.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbRiepilogoDefault).First()
                carbSerNetto += serRiep.Trasferimenti + serRiep.Restituzioni + serRiep.Accise

                carbBenzNetto = If(carbBenz = 0, carbBenzNetto - drRichiestaCorr("Rimanenza_Benzina"), carbBenzNetto)
                carbGasNetto = If(carbGas = 0, carbGasNetto - drRichiestaCorr("Rimanenza_Gasolio"), carbGasNetto)
                carbSerNetto = If(carbSer = 0, carbSerNetto - drRichiestaCorr("Rimanenza_Gasolio_Serra"), carbSerNetto)

            End If


            If flagRichiestaProprio Then
                'Conto Proprio
                drIstruttoria.ContoProprioTerzi = "PROPRIO"

                drIstruttoria.AnnoPrecRimanenzeProprioBenzina = drRichiestaCorr("Rimanenza_Benzina")
                drIstruttoria.AnnoPrecRimanenzeProprioGasolio = drRichiestaCorr("Rimanenza_Gasolio")
                drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra = drRichiestaCorr("Rimanenza_Gasolio_Serra")
                drIstruttoria.AnnoPrecRimanenzeProprioTotale = drIstruttoria.AnnoPrecRimanenzeProprioBenzina +
                    drIstruttoria.AnnoPrecRimanenzeProprioGasolio + drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra

                drIstruttoria.AnnoCorrAssegnatoProprioGasolioSerra = carbSerNetto
                drIstruttoria.AnnoCorrAssegnatoProprioGasolio = carbGasNetto
                drIstruttoria.AnnoCorrAssegnatoProprioBenzina = carbBenzNetto
                drIstruttoria.AnnoCorrAssegnatoProprioTotale = carbBenzNetto + carbGasNetto + carbSerNetto

                drIstruttoria.AnnoCorrAssegnatoTerziGasolioSerra = 0
                drIstruttoria.AnnoCorrAssegnatoTerziGasolio = 0
                drIstruttoria.AnnoCorrAssegnatoTerziBenzina = 0
                drIstruttoria.AnnoCorrAssegnatoTerziTotale = 0

                drIstruttoria.AnnoPrecRimanenzeTerziBenzina = 0
                drIstruttoria.AnnoPrecRimanenzeTerziGasolio = 0
                drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra = 0
                drIstruttoria.AnnoPrecRimanenzeTerziTotale = 0
            Else
                'Conto Terzi
                drIstruttoria.ContoProprioTerzi = "TERZI"

                drIstruttoria.AnnoPrecRimanenzeTerziBenzina = drRichiestaCorr("Rimanenza_Benzina")
                drIstruttoria.AnnoPrecRimanenzeTerziGasolio = drRichiestaCorr("Rimanenza_Gasolio")
                drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra = drRichiestaCorr("Rimanenza_Gasolio_Serra")
                drIstruttoria.AnnoPrecRimanenzeTerziTotale = drIstruttoria.AnnoPrecRimanenzeTerziBenzina +
                    drIstruttoria.AnnoPrecRimanenzeTerziGasolio + drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra

                drIstruttoria.AnnoCorrAssegnatoTerziGasolioSerra = carbSerNetto
                drIstruttoria.AnnoCorrAssegnatoTerziGasolio = carbGasNetto
                drIstruttoria.AnnoCorrAssegnatoTerziBenzina = carbBenzNetto
                drIstruttoria.AnnoCorrAssegnatoTerziTotale = carbBenzNetto + carbGasNetto + carbSerNetto

                drIstruttoria.AnnoCorrAssegnatoProprioGasolioSerra = 0
                drIstruttoria.AnnoCorrAssegnatoProprioGasolio = 0
                drIstruttoria.AnnoCorrAssegnatoProprioBenzina = 0
                drIstruttoria.AnnoCorrAssegnatoProprioTotale = 0

                drIstruttoria.AnnoPrecRimanenzeProprioBenzina = 0
                drIstruttoria.AnnoPrecRimanenzeProprioGasolio = 0
                drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra = 0
                drIstruttoria.AnnoPrecRimanenzeProprioTotale = 0
            End If

            If drIstruttoria.AnnoPrecRimanenzeProprioBenzina = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeProprioGasolio = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeProprioGasolioSerra = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeTerziBenzina = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeTerziGasolio = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeTerziGasolioSerra = 0 Then

                drIstruttoria.RimanenzeCarburante = "NO"

            Else
                drIstruttoria.RimanenzeCarburante = "SI"
            End If


            If Qs_Esito = 1 Then
                rptIstruttoriaRichCarb.HeaderSectionRigettato.SectionFormat.EnableSuppress = True

                drIstruttoria.EsitoPositivo = "X"
                drIstruttoria.NoteAssegnazione = Qs_NoteEsito

                If setupGestioneRimanenze = 1 Then
                    drIstruttoria.DichiarApprovazione = String.Format("per l'anno {0} dei quantitativi di carburanti di seguito indicati, " &
                        "al netto dell'eventuale quantitativo residuo di cui al punto 5 e in considerazione dei quantitativi di cui al punto 6.", richiestaAnno)
                Else
                    drIstruttoria.DichiarApprovazione = String.Format("per l'anno {0} dei quantitativi di carburanti di seguito indicati, al netto dell'eventuale quantitativo residuo di cui al punto 5.", richiestaAnno)
                End If

            ElseIf Qs_Esito = 2 Then
                rptIstruttoriaRichCarb.HeaderSectionApprovato.SectionFormat.EnableSuppress = True

                drIstruttoria.EsitoNegativo = "X"
                drIstruttoria.NoteRigetto = Qs_NoteEsito
            End If

            '-----------------------------------------------------------------------------
            dsIstruttoria.DT_VerbaleIstruttoriaRichCarb.Rows.Add(drIstruttoria)
            '-----------------------------------------------------------------------------

        Catch ex As Exception
            Log_Errori &= "- caricamento dataset: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            '-----------------------------------------------------------------------------
            rptIstruttoriaRichCarb.SetDataSource(dsIstruttoria)
            rptIstruttoriaRichCarb.OpenSubreport("Rpt_GestioneRimanenzeInutilizzo.rpt").SetDataSource(dsRimanenzeInutilizzo)
            '-----------------------------------------------------------------------------
        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

    End Sub


End Class