Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreProfilazioneDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal

Public Class IstruttoriaRendCarb
    Inherits System.Web.UI.Page

    Private rptIstruttoriaRendCarb As Rpt_IstruttoriaRendCarb

    Private Qs_Piva As String
    Private Qs_CodRendicontazioneTestata As Integer
    Private Qs_ModificaDati As Boolean
    Private Qs_SegnalazioniMacchine As Boolean
    Private Qs_Esito As Integer
    Private Qs_NoteEsito As String

    Private Log_Errori As String

    Private objParametri_Server As New AgronicaCoreParametri
    Private objParametri_Utenti As New AgronicaCoreParametri

    Private nomeDocIdentifPratica As String = ""

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
        rptIstruttoriaRendCarb = New Rpt_IstruttoriaRendCarb
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Qs_Piva = Stringa_Decodifica(Request.QueryString("p"),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_CodRendicontazioneTestata = Stringa_Decodifica(Request.QueryString("crt"),
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

        Dim Nome_Documento As String = "IstruttoriaRendCarb"
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
                Dim pathPdfGenerato = objGestFile.SalvaReportPdf(rptIstruttoriaRendCarb,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
                IO.File.Delete(pathPdfGenerato)

                rptIstruttoriaRendCarb.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + MessaggioCompletoDataEccezione(ex, True, source:=True) + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File_Log As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

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
                            "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If
    End Sub

    Private Sub Stampa_IstruttoriaCarb()
        'DataSet Report
        Dim dsIstruttoria As New DS_IstruttoriaRendCarb()
        Dim dsRimanenzeInutilizzo As New DS_GestioneRimanenzeInutilizzo

        'Datatable Query
        Dim dtImpresa As New DataTable
        Dim dtCompilatore As New DataTable
        Dim dtRichiestePratiche As New DataTable
        Dim drRendicontazioneCorr As DataRow = Nothing
        Dim rendicontazioneAnno As Integer
        Dim rendicontazioneNumero As Integer
        Dim drRichiestaIniziale As DataRow = Nothing
        Dim drRendicontazioneAnnoPrecProprio As DataRow = Nothing
        Dim drRendicontazioneAnnoPrecTerzi As DataRow = Nothing
        Dim totaliCarburante As New List(Of CarburanteHelper)
        Dim dtTotaliCarburanteAcquistato As New DataTable
        Dim dataRendicontazionePratica As New DateTime
        Dim dtUmaSetup As New DataTable
        '''0 = non abilitata, 1 = abilitata, 2 = abilitata solo rendicontazioni
        Dim setupGestioneRimanenze As Integer
        Dim flagRichiestaProprio As Boolean = False
        Dim dtTotaliCarburanteRicevuto As New DataTable
        Dim dtTotaliCarburanteIndisponibile As New DataTable
        Dim dtTrasferimentiRendCorr As New DataTable
        Dim dtRestituzioniRendCorr As New DataTable

        'Oggetti BIZ e DAL
        Dim handleImprese As New Imprese_Read()
        Dim handleCompilatore As New Utenti_xGruppi_Utente_R()
        Dim handleRichiesteTestate As New UMA_Richieste_Testata_R()
        Dim handleBizUmaRichieste As New AgronicaCoreUmaBiz.UMA_Richieste()
        Dim handleUmaVendite As New UMA_Vendite_R()
        Dim handlePraticheStati As New Pratiche_Stati_R()
        Dim handleUmaSetup As New UMASetup_R()
        Dim handleTrasferimenti As New UMA_Richieste_Trasferimenti_R()
        Dim handleRestituzioni As New UMA_Richieste_Restituzioni_R()
        Dim handleCausaliInutilizzo As New UMA_Causali_R()

        '-----------------------------------------
        '---- Query di lettura  -------------
        '----------------------------------------- 
        Try
            'Ottengo i dati dell'azienda
            dtImpresa = handleImprese.DatiIntestazioneImpresa(Qs_Piva, "", "", objParametri_Server)

            'Ottengo i dati del compilatore
            dtCompilatore = handleCompilatore.LeggiUtenti(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)

            'Ottengo i dati della testata della rendicontazione e relativi numero/anno pratica
            dtRichiestePratiche = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, False)
            Dim dtRichiestePraticheT = handleRichiesteTestate.Leggi(Qs_Piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, True)
            dtRichiestePratiche.Merge(dtRichiestePraticheT)

            drRendicontazioneCorr = dtRichiestePratiche.Select(
                String.Format("Richiesta_Cod = {0}", Qs_CodRendicontazioneTestata)
            ).FirstOrDefault()
            rendicontazioneAnno = drRendicontazioneCorr("Anno")
            rendicontazioneNumero = drRendicontazioneCorr("Numero")
            nomeDocIdentifPratica = rendicontazioneAnno & "-" & rendicontazioneNumero
            flagRichiestaProprio = drRendicontazioneCorr.Field(Of Integer)("Tipo_Richiesta") = 0

            Dim dtStatoPratica = handlePraticheStati.Leggi(drRendicontazioneCorr("Pratica_Cod"), 2007, AGRODATAINIZIO, AGRODATAFINE, "", "Pratiche_Stati.Validita_Inizio DESC", objParametri_Server, 0)
            dataRendicontazionePratica = If(dtStatoPratica.Rows.Count = 0, Date.Now, dtStatoPratica.Rows(0).Field(Of DateTime)("Validita_Inizio"))

            'Individuo la prima richiesta dell'anno per le colonne di rimanenza
            drRichiestaIniziale = (
                From dr In dtRichiestePratiche.AsEnumerable()
                Where dr.Field(Of Integer)("Avanzamento_Richiesta") = 0 AndAlso
                dr.Field(Of Integer)("Anno") = rendicontazioneAnno AndAlso
                dr.Field(Of Integer)("Tipo_Richiesta") = drRendicontazioneCorr.Item("Tipo_Richiesta") AndAlso
                Not New Integer() {enum_WWorflow_WAnagraficaStati.Gestione_UMA_Rinuncia, 2006}.Contains(dr.Field(Of Integer)("Stato_Cod")) AndAlso
                (dr.Field(Of Integer)("Richiesta_Integrativa") = 0 OrElse dr.Field(Of Integer)("Richiesta_Integrativa").Equals(DBNull.Value))
            ).FirstOrDefault()

            drRendicontazioneAnnoPrecProprio = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = 0 And Anno = " & rendicontazioneAnno - 1
            ).FirstOrDefault()
            drRendicontazioneAnnoPrecTerzi = dtRichiestePratiche.Select(
                "Avanzamento_Richiesta = 1 And Tipo_Richiesta = -1 And Anno = " & rendicontazioneAnno - 1
            ).FirstOrDefault()

            'Ottengo dati di configurazione come la percentuale di decurtamento da applicare alle quantità di carburante
            dtUmaSetup = handleUmaSetup.LeggiSetup(rendicontazioneAnno, objParametri_Server)
            setupGestioneRimanenze = dtUmaSetup.Rows(0)("Gestione_Rimanenze")

            'Ottengo i dati di riepilogo dei carburanti usati per le lavorazioni della rendicontazione
            totaliCarburante = handleBizUmaRichieste.LeggiTotaliCarburantePerTipo(Qs_CodRendicontazioneTestata, objParametri_Server)

            'Acquistato dai fornitori
            dtTotaliCarburanteAcquistato = handleUmaVendite.LeggiCarburanteVenduto(
                Qs_Piva,
                rendicontazioneAnno,
                drRendicontazioneCorr.Field(Of Integer)("Tipo_Richiesta"),
                0,
                "ISNULL(Richiesta_Cod, 0) = 0 AND Lt > 0",
                objParametri_Server)

            'Ricevuto da trasferimenti da altre aziende
            dtTotaliCarburanteRicevuto = handleUmaVendite.LeggiCarburanteVenduto(
                Qs_Piva,
                rendicontazioneAnno,
                drRendicontazioneCorr.Field(Of Integer)("Tipo_Richiesta"),
                0,
                "Richiesta_Cod IS NOT NULL AND Richiesta_Cod <> 0 AND Lt > 0",
                objParametri_Server)

            'Emesso tramite: trasferimenti, restituzioni, accise (durante l'anno).
            'NB: non considera la rendicontazione corrente in quanto le vendite derivanti dalla stessa
            'vengono registrate solo dopo l'approvazione, della quale questa stampa è propedeutica
            dtTotaliCarburanteIndisponibile = handleUmaVendite.LeggiCarburanteVenduto(
                Qs_Piva,
                rendicontazioneAnno,
                drRendicontazioneCorr.Field(Of Integer)("Tipo_Richiesta"),
                0,
                "Richiesta_Cod IS NOT NULL AND Richiesta_Cod <> 0 AND Lt < 0",
                objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_IstruttoriaRendCarb: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        End Try

        '------------------------------------------------------------
        '---- Impostazione dei dati trovati nei report  -------------
        '------------------------------------------------------------
        Try


            Dim drIstruttoria = dsIstruttoria.DT_IstruttoriaRendCarb.NewDT_IstruttoriaRendCarbRow

            drIstruttoria.ComunitaMontanaDen = "AFOR"
            drIstruttoria.StrutturaDesc = ""
            drIstruttoria.CompilatoreDen = dtCompilatore.Rows(0)("Dettagli")
            drIstruttoria.RendicontazioneNumero = rendicontazioneNumero
            drIstruttoria.RendicontazioneAnno = rendicontazioneAnno
            drIstruttoria.ImpresaRagSoc = dtImpresa.Rows(0)("rag_soc")
            drIstruttoria.ImpresaCuaa = dtImpresa.Rows(0)("codice_cuaa")
            drIstruttoria.DataFineAnnoPrec = New Date(rendicontazioneAnno - 1, 12, 31)
            drIstruttoria.RendicontazioneDataPratica = dataRendicontazionePratica
            drIstruttoria.DataIstruttoria = Date.Now

            drIstruttoria.ModDatiRendicontazione = IIf(Qs_ModificaDati = True, "SI", "NO")
            drIstruttoria.SegnalazioniMacchine = IIf(Qs_SegnalazioniMacchine = True, "SI", "NO")

            Dim carbHelperDefault As New CarburanteHelper() With {
                    .Tipo = 0, .Calcolato = 0, .Richiesto = 0, .Assegnato = 0
            }

            If flagRichiestaProprio Then
                Dim mexProprioTerzi = "conto proprio"
                drIstruttoria.TitoloContoProprioTerzi = mexProprioTerzi.ToUpper
                drIstruttoria.ContoProprioTerzi = mexProprioTerzi

                'If drRendicontazioneAnnoPrecProprio IsNot Nothing Then
                '    drIstruttoria.AnnoPrecRimanenzeBenzina = drRendicontazioneAnnoPrecProprio("Rimanenza_Benzina")
                '    drIstruttoria.AnnoPrecRimanenzeGasolio = drRendicontazioneAnnoPrecProprio("Rimanenza_Gasolio")
                '    drIstruttoria.AnnoPrecRimanenzeGasolioSerra = drRendicontazioneAnnoPrecProprio("Rimanenza_Gasolio_Serra")
                'Else
                '    drIstruttoria.AnnoPrecRimanenzeBenzina = 0
                '    drIstruttoria.AnnoPrecRimanenzeGasolio = 0
                '    drIstruttoria.AnnoPrecRimanenzeGasolioSerra = 0
                'End If

            Else
                Dim mexProprioTerzi = "conto terzi"
                drIstruttoria.TitoloContoProprioTerzi = mexProprioTerzi.ToUpper
                drIstruttoria.ContoProprioTerzi = mexProprioTerzi

                'If drRendicontazioneAnnoPrecTerzi IsNot Nothing Then
                '    drIstruttoria.AnnoPrecRimanenzeBenzina = drRendicontazioneAnnoPrecTerzi("Rimanenza_Benzina")
                '    drIstruttoria.AnnoPrecRimanenzeGasolio = drRendicontazioneAnnoPrecTerzi("Rimanenza_Gasolio")
                '    drIstruttoria.AnnoPrecRimanenzeGasolioSerra = drRendicontazioneAnnoPrecTerzi("Rimanenza_Gasolio_Serra")
                'Else
                '    drIstruttoria.AnnoPrecRimanenzeBenzina = 0
                '    drIstruttoria.AnnoPrecRimanenzeGasolio = 0
                '    drIstruttoria.AnnoPrecRimanenzeGasolioSerra = 0
                'End If

            End If

            If drRichiestaIniziale IsNot Nothing Then
                drIstruttoria.AnnoPrecRimanenzeBenzina = If(drRichiestaIniziale.Field(Of Double)("Rimanenza_Benzina").Equals(DBNull.Value), 0, drRichiestaIniziale.Field(Of Double)("Rimanenza_Benzina"))
                drIstruttoria.AnnoPrecRimanenzeGasolio = If(drRichiestaIniziale.Field(Of Double)("Rimanenza_Gasolio").Equals(DBNull.Value), 0, drRichiestaIniziale.Field(Of Double)("Rimanenza_Gasolio"))
                drIstruttoria.AnnoPrecRimanenzeGasolioSerra = If(drRichiestaIniziale.Field(Of Double)("Rimanenza_Gasolio_Serra").Equals(DBNull.Value), 0, drRichiestaIniziale.Field(Of Double)("Rimanenza_Gasolio_Serra"))
            Else
                drIstruttoria.AnnoPrecRimanenzeBenzina = 0
                drIstruttoria.AnnoPrecRimanenzeGasolio = 0
                drIstruttoria.AnnoPrecRimanenzeGasolioSerra = 0
            End If
            drIstruttoria.AnnoPrecRimanenzeTotale = drIstruttoria.AnnoPrecRimanenzeBenzina +
                        drIstruttoria.AnnoPrecRimanenzeGasolio + drIstruttoria.AnnoPrecRimanenzeGasolioSerra

            If drIstruttoria.AnnoPrecRimanenzeBenzina = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeGasolio = 0 AndAlso
                drIstruttoria.AnnoPrecRimanenzeGasolioSerra = 0 Then

                drIstruttoria.RimanenzeCarburante = "NO"

            Else
                drIstruttoria.RimanenzeCarburante = "SI"
            End If

            Dim carbAcqBenz As Integer = 0
            Dim carbAcqGas As Integer = 0
            Dim carbAcqSer As Integer = 0
            If dtTotaliCarburanteAcquistato.Rows.Count > 0 Then

                If dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 3").Length > 0 Then
                    carbAcqBenz = dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 3")(0).Field(Of Double)("Totale_Carb")
                End If

                If dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 2").Length > 0 Then
                    carbAcqGas = dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 2")(0).Field(Of Double)("Totale_Carb")
                End If

                If dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 8").Length > 0 Then
                    carbAcqSer = dtTotaliCarburanteAcquistato.Select("Tipo_Carburante = 8")(0).Field(Of Double)("Totale_Carb")
                End If

            End If
            'Carburante Acquistato
            drIstruttoria.AnnoCorrAcquistiBenzina = carbAcqBenz
            drIstruttoria.AnnoCorrAcquistiGasolio = carbAcqGas
            drIstruttoria.AnnoCorrAcquistiGasolioSerra = carbAcqSer
            drIstruttoria.AnnoCorrAcquistiTotale = carbAcqBenz + carbAcqGas + carbAcqSer

            Dim puntoDichiarDaRendicontare = 5

            drIstruttoria.TrasferitoRicevutoBenzina = 0
            drIstruttoria.TrasferitoRicevutoGasolio = 0
            drIstruttoria.TrasferitoRicevutoGasolioSerra = 0
            drIstruttoria.TrasferitoRicevutoTotale = 0

            Dim benzTotIndisp = 0
            Dim gasTotIndisp = 0
            Dim serTotIndisp = 0

            Dim benzColRimRiassegnata = "Rimanenza_Benzina"
            Dim gasColRimRiassegnata = "Rimanenza_Gasolio"
            Dim serColRimRiassegnata = "Rimanenza_Gasolio_Serra"

            Dim attivoSetupGestioneRimanenze = If(setupGestioneRimanenze = 1 OrElse setupGestioneRimanenze = 2, True, False)

            If attivoSetupGestioneRimanenze Then

                'Nascondo la vecchia sezione di riepilogo della seconda pagina in favore di quella nuova
                rptIstruttoriaRendCarb.DetailSectionFinale.SectionFormat.EnableSuppress = True

                benzColRimRiassegnata = "Rim_Riass_Conf_Benzina"
                gasColRimRiassegnata = "Rim_Riass_Conf_Gasolio"
                serColRimRiassegnata = "Rim_Riass_Conf_Gasolio_Serra"

                If dtTotaliCarburanteRicevuto.Rows.Count > 0 Then
                    'Mostro la sezione apposita come punto n. 5 perché concorre al calcolo dei quantitativi da rendicontare
                    rptIstruttoriaRendCarb.DetailSectionTrasfRicevuti.SectionFormat.EnableSuppress = False
                    puntoDichiarDaRendicontare += 1

                    If dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 3").Length > 0 Then
                        drIstruttoria.TrasferitoRicevutoBenzina = dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 3")(0).Field(Of Double)("Totale_Carb")
                    End If

                    If dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 2").Length > 0 Then
                        drIstruttoria.TrasferitoRicevutoGasolio = dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 2")(0).Field(Of Double)("Totale_Carb")
                    End If

                    If dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 8").Length > 0 Then
                        drIstruttoria.TrasferitoRicevutoGasolioSerra = dtTotaliCarburanteRicevuto.Select("Tipo_Carburante = 8")(0).Field(Of Double)("Totale_Carb")
                    End If

                    drIstruttoria.TrasferitoRicevutoTotale = drIstruttoria.TrasferitoRicevutoBenzina + drIstruttoria.TrasferitoRicevutoGasolio + drIstruttoria.TrasferitoRicevutoGasolioSerra

                End If

                If dtTotaliCarburanteIndisponibile.Rows.Count > 0 Then
                    'Sezione di carburante che mostra il riepilogo dei carburanti che l'azienda durante l'anno ha "perso",
                    'concorre al calcolo dei quantitativi da rendicontare
                    rptIstruttoriaRendCarb.DetailSectionCarbIndisponibile.SectionFormat.EnableSuppress = False
                    drIstruttoria.PuntoElencoDichiarCarbIndisp = puntoDichiarDaRendicontare.ToString() 'Solo visualizzazione
                    puntoDichiarDaRendicontare += 1

                    If dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 3").Length > 0 Then
                        benzTotIndisp = dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 3")(0).Field(Of Double)("Totale_Carb")
                    End If

                    If dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 2").Length > 0 Then
                        gasTotIndisp = dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 2")(0).Field(Of Double)("Totale_Carb")
                    End If

                    If dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 8").Length > 0 Then
                        serTotIndisp = dtTotaliCarburanteIndisponibile.Select("Tipo_Carburante = 8")(0).Field(Of Double)("Totale_Carb")
                    End If

                    drIstruttoria.TotIndispAnnoBenzina = Math.Abs(benzTotIndisp)
                    drIstruttoria.TotIndispAnnoGasolio = Math.Abs(gasTotIndisp)
                    drIstruttoria.TotIndispAnnoGasolioSerra = Math.Abs(serTotIndisp)
                    drIstruttoria.TotIndispAnnoTotale = drIstruttoria.TotIndispAnnoBenzina + drIstruttoria.TotIndispAnnoGasolio + drIstruttoria.TotIndispAnnoGasolioSerra

                End If

                'La ditta ha dichiarato questi quantitativi di carburante rimasti: [Nota A]
                Dim carbNonUtilizzatoBenz = If(IsDBNull(drRendicontazioneCorr("Rim_Dich_Benzina")), 0, drRendicontazioneCorr.Field(Of Double)("Rim_Dich_Benzina"))
                Dim carbNonUtilizzatoGas = If(IsDBNull(drRendicontazioneCorr("Rim_Dich_Gasolio")), 0, drRendicontazioneCorr.Field(Of Double)("Rim_Dich_Gasolio"))
                Dim carbNonUtilizzatoSer = If(IsDBNull(drRendicontazioneCorr("Rim_Dich_Gasolio_Serra")), 0, drRendicontazioneCorr.Field(Of Double)("Rim_Dich_Gasolio_Serra"))

                'Sezioni di riepilogo finale, non concorrono al calcolo del carburante da rendicontare e sono mostrate dopo la sezione di approvazione o rigetto.
                'Rappresentano i dati della rendicontazione stessa e non quelli precedenti dell'anno
                If carbNonUtilizzatoBenz > 0 OrElse carbNonUtilizzatoGas > 0 OrElse carbNonUtilizzatoSer > 0 Then

                    Dim causaliInutilizzoReport As String = ""
                    Dim listaCausaliInutilizzo As New List(Of String)()
                    Dim causaliInutilizzotestata = If(IsDBNull(drRendicontazioneCorr("Causale_Non_Utilizzo")), "", drRendicontazioneCorr.Field(Of String)("Causale_Non_Utilizzo"))

                    If causaliInutilizzotestata <> "" Then
                        Dim dtCausaliInutilizzo = handleCausaliInutilizzo.Leggi(objParametri_Server)

                        For Each item As String In causaliInutilizzotestata.Split("|")
                            Dim rowcausaleDes = dtCausaliInutilizzo.Select("Causale_Cod = " & item).FirstOrDefault()
                            If rowcausaleDes IsNot Nothing Then
                                listaCausaliInutilizzo.Add(rowcausaleDes("Causale_Des"))
                            End If
                        Next

                        If listaCausaliInutilizzo.Count > 0 Then
                            causaliInutilizzoReport = " a causa di " & String.Join(", ", listaCausaliInutilizzo)
                        End If
                    End If

                    drIstruttoria.DichiarCarbInutilizzato = String.Format("Si attesta inoltre che la Ditta ha dichiarato nella rendicontazione le seguenti rimanenze di carburante agricolo inutilizzato{0}:",
                                                                        causaliInutilizzoReport)

                    drIstruttoria.InutilizzatoBenzina = carbNonUtilizzatoBenz
                    drIstruttoria.InutilizzatoGasolio = carbNonUtilizzatoGas
                    drIstruttoria.InutilizzatoGasolioSerra = carbNonUtilizzatoSer
                    drIstruttoria.InutilizzatoTotale = carbNonUtilizzatoBenz + carbNonUtilizzatoGas + carbNonUtilizzatoSer

                    Dim colonnaRimBenz = "Confermato_Benzina"
                    Dim colonnaRimGas = "Confermato_Gasolio"
                    Dim colonnaRimSer = "Confermato_Gasolio_Serra"

                    Dim colonnaAccBenz = "Rec_Acc_Conf_Benzina"
                    Dim colonnaAccGas = "Rec_Acc_Conf_Gasolio"
                    Dim colonnaAccSer = "Rec_Acc_Conf_Gasolio_Serra"

                    Dim nascondiTrasferimentiVuoti As Boolean = True

                    '[Nota A] Dei quantitativi rimasti, l'approvatore ne conferma questi come trasferiti ad altre aziende:
                    dtTrasferimentiRendCorr = handleTrasferimenti.Leggi(Qs_Piva, Qs_CodRendicontazioneTestata, objParametri_Server)

                    If dtTrasferimentiRendCorr.Rows.Count > 0 Then

                        nascondiTrasferimentiVuoti = False

                        Dim carbTrasferitoBenz = dtTrasferimentiRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimBenz)), 0, row.Field(Of Double)(colonnaRimBenz))
                            End Function)

                        Dim carbTrasferitoGas = dtTrasferimentiRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimGas)), 0, row.Field(Of Double)(colonnaRimGas))
                            End Function)

                        Dim carbTrasferitoSer = dtTrasferimentiRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimSer)), 0, row.Field(Of Double)(colonnaRimSer))
                            End Function)

                        drIstruttoria.TrasferitoEmessoBenzina = carbTrasferitoBenz
                        drIstruttoria.TrasferitoEmessoGasolio = carbTrasferitoGas
                        drIstruttoria.TrasferitoEmessoGasolioSerra = carbTrasferitoSer
                        drIstruttoria.TrasferitoEmessoTotale = carbTrasferitoBenz + carbTrasferitoGas + carbTrasferitoSer

                    End If

                    If nascondiTrasferimentiVuoti = True Then
                        rptIstruttoriaRendCarb.DetailSectionTrasferimenti.SectionFormat.EnableSuppress = True
                    End If


                    Dim nascondiRestituzioniVuote As Boolean = True

                    '[Nota A] Dei quantitativi rimasti, l'approvatore ne conferma questi come restituiti ai fornitori:
                    dtRestituzioniRendCorr = handleRestituzioni.Leggi(Qs_Piva, Qs_CodRendicontazioneTestata, objParametri_Server)

                    If dtRestituzioniRendCorr.Rows.Count > 0 Then
                        nascondiRestituzioniVuote = False

                        'Sommo perché i record sono divisi per i fornitori beneficiari del reso
                        Dim qtaRestituzioniBenzina = dtRestituzioniRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimBenz)), 0, row.Field(Of Double)(colonnaRimBenz))
                            End Function)

                        Dim qtaRestituzioniGasolio = dtRestituzioniRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimGas)), 0, row.Field(Of Double)(colonnaRimGas))
                            End Function)

                        Dim qtaRestituzioniGasolioSerra = dtRestituzioniRendCorr.AsEnumerable().Sum(
                            Function(row)
                                Return If(IsDBNull(row(colonnaRimSer)), 0, row.Field(Of Double)(colonnaRimSer))
                            End Function)

                        drIstruttoria.RestituitoBenzina = qtaRestituzioniBenzina
                        drIstruttoria.RestituitoGasolio = qtaRestituzioniGasolio
                        drIstruttoria.RestituitoGasolioSerra = qtaRestituzioniGasolioSerra
                        drIstruttoria.RestituitoTotale = qtaRestituzioniBenzina + qtaRestituzioniGasolio + qtaRestituzioniGasolioSerra

                    End If

                    If nascondiRestituzioniVuote = True Then
                        rptIstruttoriaRendCarb.DetailSectionRestituzioni.SectionFormat.EnableSuppress = True
                    End If


                    Dim nascondiAcciseVuote As Boolean = True

                    '[Nota A] Dei quantitativi rimasti, l'approvatore ne conferma questi per recupero accise:
                    Dim carbAcciseBenz = If(IsDBNull(drRendicontazioneCorr(colonnaAccBenz)), 0, drRendicontazioneCorr.Field(Of Double)(colonnaAccBenz))
                    Dim carbAcciseGas = If(IsDBNull(drRendicontazioneCorr(colonnaAccGas)), 0, drRendicontazioneCorr.Field(Of Double)(colonnaAccGas))
                    Dim carbAcciseSer = If(IsDBNull(drRendicontazioneCorr(colonnaAccSer)), 0, drRendicontazioneCorr.Field(Of Double)(colonnaAccSer))

                    If carbAcciseBenz <> 0 OrElse carbAcciseGas <> 0 OrElse carbAcciseSer <> 0 Then
                        nascondiAcciseVuote = False

                        drIstruttoria.AcciseBenzina = carbAcciseBenz
                        drIstruttoria.AcciseGasolio = carbAcciseGas
                        drIstruttoria.AcciseGasolioSerra = carbAcciseSer
                        drIstruttoria.AcciseTotale = carbAcciseBenz + carbAcciseGas + carbAcciseSer

                    End If

                    If nascondiAcciseVuote = True Then
                        rptIstruttoriaRendCarb.DetailSectionAccise.SectionFormat.EnableSuppress = True
                    End If

                Else
                    drIstruttoria.DichiarCarbInutilizzato = String.Format("Si attesta inoltre che la Ditta ha dichiarato nella rendicontazione di non avere rimanenze di carburante agricolo inutilizzato")
                    rptIstruttoriaRendCarb.DetailSectionCarbInutilizzato.SectionFormat.EnableSuppress = True
                    rptIstruttoriaRendCarb.DetailSectionTrasferimenti.SectionFormat.EnableSuppress = True
                    rptIstruttoriaRendCarb.DetailSectionRestituzioni.SectionFormat.EnableSuppress = True
                    rptIstruttoriaRendCarb.DetailSectionRimanenze.SectionFormat.EnableSuppress = True
                    rptIstruttoriaRendCarb.DetailSectionAccise.SectionFormat.EnableSuppress = True
                End If

            Else
                rptIstruttoriaRendCarb.DetailSectionTitoloCarbInutilizzato.SectionFormat.EnableSuppress = True
                rptIstruttoriaRendCarb.DetailSectionCarbInutilizzato.SectionFormat.EnableSuppress = True
                rptIstruttoriaRendCarb.DetailSectionTrasferimenti.SectionFormat.EnableSuppress = True
                rptIstruttoriaRendCarb.DetailSectionRestituzioni.SectionFormat.EnableSuppress = True
                rptIstruttoriaRendCarb.DetailSectionRimanenze.SectionFormat.EnableSuppress = True
                rptIstruttoriaRendCarb.DetailSectionAccise.SectionFormat.EnableSuppress = True
            End If

            drIstruttoria.DichiarCarbDaRendicontare = String.Format("{0}) alla data di compilazione della rendicontazione la Ditta aveva le seguenti quantità totali di " &
                                                                    "carburante agricolo effettivamente da rendicontare per l'anno {1}:", puntoDichiarDaRendicontare, rendicontazioneAnno)


            'Nell'rpt le due sezioni di riepilogo delle rimanenze per l'anno nuovo, mostrate alternativamente in base al setup Gestione_Rimanenze,
            'utilizzano le stesse colonne del dataset, quindi le valorizzo una volta sola ma utilizzando le colonne corrette dal database
            'dalla tabella Uma_Richieste_Testata grazie alle variabili "*ColRimRiassegnata"
            drIstruttoria.AnnoCorrRimanenzeBenzina = If(drRendicontazioneCorr.Field(Of Double)(benzColRimRiassegnata).Equals(DBNull.Value), 0, drRendicontazioneCorr.Field(Of Double)(benzColRimRiassegnata))
            drIstruttoria.AnnoCorrRimanenzeGasolio = If(drRendicontazioneCorr.Field(Of Double)(gasColRimRiassegnata).Equals(DBNull.Value), 0, drRendicontazioneCorr.Field(Of Double)(gasColRimRiassegnata))
            drIstruttoria.AnnoCorrRimanenzeGasolioSerra = If(drRendicontazioneCorr.Field(Of Double)(serColRimRiassegnata).Equals(DBNull.Value), 0, drRendicontazioneCorr.Field(Of Double)(serColRimRiassegnata))

            drIstruttoria.AnnoCorrRimanenzeTotale = drIstruttoria.AnnoCorrRimanenzeBenzina +
                drIstruttoria.AnnoCorrRimanenzeGasolio + drIstruttoria.AnnoCorrRimanenzeGasolioSerra

            If drIstruttoria.AnnoCorrRimanenzeTotale = 0 Then
                rptIstruttoriaRendCarb.DetailSectionRimanenze.SectionFormat.EnableSuppress = True
            End If

            'Il carburante da rendicontare è inteso come la qta di cui la ditta disponeva e di cui deve aver giustificato l'utilizzo attraverso l'elenco delle lavorazioni [NOTA B]
            'Rimanenze Anno Precedente + Carburante Acquistato + Ricevuto da trasferimenti - Restituito per inutilizzo = Carburante da rendicontare
            'NB: Il carburante "Restituito per inutilizzo" lo sommo comunque e non lo sottraggo perché è già in valore negativo
            drIstruttoria.AnnoCorrDaRendicontareBenzina = drIstruttoria.AnnoPrecRimanenzeBenzina + drIstruttoria.AnnoCorrAcquistiBenzina + drIstruttoria.TrasferitoRicevutoBenzina + benzTotIndisp
            drIstruttoria.AnnoCorrDaRendicontareGasolio = drIstruttoria.AnnoPrecRimanenzeGasolio + drIstruttoria.AnnoCorrAcquistiGasolio + drIstruttoria.TrasferitoRicevutoGasolio + gasTotIndisp
            drIstruttoria.AnnoCorrDaRendicontareGasolioSerra = drIstruttoria.AnnoPrecRimanenzeGasolioSerra + drIstruttoria.AnnoCorrAcquistiGasolioSerra + drIstruttoria.TrasferitoRicevutoGasolioSerra + serTotIndisp
            drIstruttoria.AnnoCorrDaRendicontareTotale = drIstruttoria.AnnoCorrDaRendicontareBenzina +
                drIstruttoria.AnnoCorrDaRendicontareGasolio + drIstruttoria.AnnoCorrDaRendicontareGasolioSerra


            Dim carbAssegBenz As Integer = 0
            Dim carbAssegGas As Integer = 0
            Dim carbAssegSer As Integer = 0

            If Qs_Esito = 1 Then
                rptIstruttoriaRendCarb.DetailSectionRigettato.SectionFormat.EnableSuppress = True

                drIstruttoria.EsitoPositivo = "X"
                drIstruttoria.NoteAssegnazione = Qs_NoteEsito

                '[NOTA B] Carburante Assegnato (alias Rendicontato) ovvero la qta data dalla somma delle lavorazioni confermate dall'approvatore
                If totaliCarburante.Count > 0 Then
                    carbAssegBenz = totaliCarburante.Where(Function(elem) elem.Tipo = 3).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                    carbAssegGas = totaliCarburante.Where(Function(elem) elem.Tipo = 2).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                    carbAssegSer = totaliCarburante.Where(Function(elem) elem.Tipo = 8).DefaultIfEmpty(carbHelperDefault).First().Assegnato
                End If

                'Queste colonne del dataset del report sono utilizzate sia per questa sezione dell'approvazione
                'sia per la sezione di riepilogo finale mostrata nel caso il setup Gestione_Rimanenze sia disattivato
                drIstruttoria.AnnoCorrRendicontatoBenzina = carbAssegBenz
                drIstruttoria.AnnoCorrRendicontatoGasolio = carbAssegGas
                drIstruttoria.AnnoCorrRendicontatoGasolioSerra = carbAssegSer
                drIstruttoria.AnnoCorrRendicontatoTotale = carbAssegBenz + carbAssegGas + carbAssegSer

            ElseIf Qs_Esito = 2 Then
                rptIstruttoriaRendCarb.DetailSectionApprovato.SectionFormat.EnableSuppress = True

                drIstruttoria.EsitoNegativo = "X"
                drIstruttoria.NoteRigetto = Qs_NoteEsito

                '[NOTA B] Carburante Assegnato (alias Rendicontato) ovvero la qta data dalla somma delle lavorazioni confermate dall'approvatore
                drIstruttoria.AnnoCorrRendicontatoBenzina = 0
                drIstruttoria.AnnoCorrRendicontatoGasolio = 0
                drIstruttoria.AnnoCorrRendicontatoGasolioSerra = 0
                drIstruttoria.AnnoCorrRendicontatoTotale = 0

            End If

            If Not attivoSetupGestioneRimanenze Then
                Dim ingBenz As Integer = 0
                Dim ingGas As Integer = 0
                Dim ingSer As Integer = 0

                'Carburante non giustificato nella rendicontazione, corrisponde al carburante da rendicontare - carburante rimasto - carburante effettivamente assegnato/rendicontato
                'Nel caso in cui questa differenza sia minore o uguale a zero allora non c'è carburante ingiustificato
                'NOTA: Questo specchietto in caso il setup Gestione_Rimanenze sia attivo non viene mostrato
                ingBenz = drIstruttoria.AnnoCorrDaRendicontareBenzina - drIstruttoria.AnnoCorrRimanenzeBenzina - carbAssegBenz
                ingGas = drIstruttoria.AnnoCorrDaRendicontareGasolio - drIstruttoria.AnnoCorrRimanenzeGasolio - carbAssegGas
                ingSer = drIstruttoria.AnnoCorrDaRendicontareGasolioSerra - drIstruttoria.AnnoCorrRimanenzeGasolioSerra - carbAssegSer

                'Carburante non giustificato nella rendicontazione, corrisponde al carburante da rendicontare - carburante rimasto - carburante effettivamente assegnato/rendicontato
                'Nel caso in cui questa differenza sia minore o uguale a zero allora non c'è carburante ingiustificato
                drIstruttoria.AnnoCorrIngiustBenzina = If(ingBenz < 0, 0, ingBenz)
                drIstruttoria.AnnoCorrIngiustGasolio = If(ingGas < 0, 0, ingGas)
                drIstruttoria.AnnoCorrIngiustGasolioSerra = If(ingSer < 0, 0, ingSer)

                drIstruttoria.AnnoCorrIngiustTotale = drIstruttoria.AnnoCorrIngiustBenzina +
                    drIstruttoria.AnnoCorrIngiustGasolio + drIstruttoria.AnnoCorrIngiustGasolioSerra
            End If

            '-----------------------------------------------------------------------------
            dsIstruttoria.DT_IstruttoriaRendCarb.Rows.Add(drIstruttoria)
            '-----------------------------------------------------------------------------

        Catch ex As Exception
            Log_Errori &= "- caricamento dataset: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '--------------------------------------------
        ' AGGANCIO DATASET AL REPORT
        '--------------------------------------------
        Try
            '-----------------------------------------------------------------------------
            rptIstruttoriaRendCarb.SetDataSource(dsIstruttoria)
            '-----------------------------------------------------------------------------
        Catch ex As Exception
            Log_Errori &= "- Aggancio dataset al report: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

    End Sub

End Class