Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class LibroConferimenti
    Inherits System.Web.UI.Page

    '##################################################################
    '###############   LEGGIMI!!!!              #######################
    'da Agrisfera è linkato ancora il report sulle stampe 2003 
    'perchè su questo era stata aggiunta la colonna peso effettivo che in realtà non volevano
    '##################################################################




    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptStampa As Rpt_LibroConferimenti
    Private Log_Errori As String

    Dim Piva As String
    Dim validita_inizio As String
    Dim validita_fine As String

    Dim Param_Filtri As String = ""
    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CUAA As String = ""
    Dim Param_Indirizzo As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Const DEFAULT_NUM_RIGHE_DETTAGLI As Integer = 50

#Region ""

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


        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        rptStampa = New Rpt_LibroConferimenti

    End Sub

#End Region

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        validita_inizio = Stringa_Decodifica(CStr(Request.QueryString("di")),
                           AgroKey_EncoderDecoder,
                           Server)

        validita_fine = Stringa_Decodifica(CStr(Request.QueryString("df")),
                         AgroKey_EncoderDecoder,
                         Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "LibroConferimenti"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                CaricaDs_LibroConferimenti()

            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try

                'Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                'Data_Inizio_Allegati = Data_Inizio
                'Data_Fine_Allegati = Data_Fine
                'IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                '' leggo la sottocartella da CategorieDocumenti
                'Dim Sottocartella As String
                'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                'Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, "", "", objParametri_Server)

                '' salvo il report in formato PDF
                'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                'objGestFile.SalvaReportPdf(rptSchedaCatastoUtilizzi, _
                '                           enum_CategorieDocumenti., _
                '                           Sottocartella, _
                '                           Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                '                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                'Dim AllegatiDocumentiCod As Integer

                'AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                '                                                 enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, _
                '                                                 Nome_Documento, _
                '                                                 Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                '                                                 Sottocartella, _
                '                                                 "", "", "", "", _
                '                                                 Data_Inizio_Allegati, _
                '                                                 Data_Fine_Allegati, _
                '                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            'DSRegCorr.Dispose()
            'DSRegCorr = Nothing

            'rptRegCorrispettivi.Close()
            'rptRegCorrispettivi.Dispose()
            'rptRegCorrispettivi = Nothing

            GC.Collect()

            'Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), _
            '                                    AgroKey_EncoderDecoder, _
            '                                    Server)

            'If PDF = "1" Then
            '    Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            'Else
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                                "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            'End If

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Piva) +
                               vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Conferimento",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "LibroConferimenti",
                                                 Log_Errori)

            End If
        End If
    End Sub

    '#####################################################################################################
    Private Sub CaricaDs_LibroConferimenti()

        Dim DS As New DS_LibroConferimenti

        Try

            'Dati generali
            Dim Piva As String = ""
            Dim rag_soc As String = ""
            Dim Validita_Inizio As String = ""
            Dim Validita_Fine As String = ""
            'Dim peso_netto_complessivo As String
            'Dim peso_netto_complessivo_dbl As Double
            Dim contratto_des As String = ""

            '<DettaglioVariabiliStampe>
            Dim data_bolla As String
            Dim numero_bolla As String
            Dim conferente_piva As String
            Dim conferente_rag_soc As String
            Dim dettagli_coltura As String
            Dim udm_des As String = ""
            Dim peso_netto, peso_effettivo As Double
            Dim peso_netto_dbl_kg As Double = 0
            Dim peso_netto_dbl_lt As Double = 0
            Dim lotto_interno As String

            'Dataset
            Dim RigaDs As DS_LibroConferimenti.DS_LibroConferimentiRow

            Dim i As Integer

            Dim XmlDoc As New System.Xml.XmlDocument
            'Dim XML_FiltroStampa As System.Xml.XmlElement
            Dim XMLs_DettaglioVariabiliStampe As System.Xml.XmlNodeList
            Dim XML_DettaglioVariabiliStampe As System.Xml.XmlElement

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Try


                '##############################################################
                '#####  Recupero l'xml            ##############################
                '##############################################################

                Dim strXmlVariabilistampe As String
                strXmlVariabilistampe = Session("strXmlVariabilistampe")

                If strXmlVariabilistampe <> "" Then

                    'Carico la stringa xml in un nuovo documento
                    XmlDoc = New System.Xml.XmlDocument

                    XmlDoc.LoadXml(strXmlVariabilistampe)

                    If XmlDoc.HasChildNodes Then

                        '22/02/2019: 
                        'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
                        'Piva = XML_FiltroStampa.GetAttribute("piva")
                        'rag_soc = XML_FiltroStampa.GetAttribute("rag_soc")
                        'contratto_des = XML_FiltroStampa.GetAttribute("contratto_des")
                        'Validita_Inizio = XML_FiltroStampa.GetAttribute("validita_inizio")
                        'Validita_Fine = XML_FiltroStampa.GetAttribute("validita_fine")


                        'peso_netto_complessivo = XML_FiltroStampa.GetAttribute("peso_netto_complessivo")
                        'If XML_FiltroStampa.GetAttribute("peso_netto_complessivo") <> "" Then
                        '    peso_netto_complessivo_dbl = CDbl(XML_FiltroStampa.GetAttribute("peso_netto_complessivo"))
                        'Else
                        '    peso_netto_complessivo_dbl = 0
                        'End If

                        'XMLs_DettaglioVariabiliStampe = XML_FiltroStampa.GetElementsByTagName("DettaglioVariabiliStampe")
                        XMLs_DettaglioVariabiliStampe = XmlDoc.SelectNodes("//DettaglioVariabiliStampe")

                        'Dim j As Integer = 1
                        Dim Num_DettagliXML As Integer = XMLs_DettaglioVariabiliStampe.Count
                        Dim Num_Pagine As Integer
                        Dim TempPag As Integer = 1
                        Dim Totale_effettivo_kg As Double = 0
                        Dim Totale_kg As Double = 0
                        Dim Totale_lt As Double = 0
                        Dim Num_Pag_Corrente As Integer
                        Dim Num_Righe_DS As Integer
                        Dim Num_Righe_Riporti As Integer
                        Dim j As Integer = 0
                        Dim NUM_RIGHE_DETTAGLI As Integer
                        Dim num_righe_dettagli_config As Integer = 0

                        'If Not IsNothing(ConfigurationSettings.AppSettings("LibroConferimenti_NumRighePagina")) AndAlso _
                        '    (ConfigurationSettings.AppSettings("LibroConferimenti_NumRighePagina") <> "") AndAlso _
                        '        (ConfigurationSettings.AppSettings("LibroConferimenti_NumRighePagina") <> "0") Then
                        '    NUM_RIGHE_DETTAGLI = CInt(ConfigurationSettings.AppSettings("LibroConferimenti_NumRighePagina"))
                        'Else
                        '    NUM_RIGHE_DETTAGLI = DEFAULT_NUM_RIGHE_DETTAGLI
                        'End If

                        num_righe_dettagli_config = objConfigSiti.Recupera_Valore_ByChiave(Enum_SiteRedirector.Sito_AgronicaStampe_2010, "LibroConferimenti_NumRighePagina", objParametri_Server)

                        If num_righe_dettagli_config <> 0 Then
                            NUM_RIGHE_DETTAGLI = num_righe_dettagli_config
                        Else
                            NUM_RIGHE_DETTAGLI = DEFAULT_NUM_RIGHE_DETTAGLI
                        End If

                        Num_Pagine = Math.Ceiling(CDbl(Num_DettagliXML / NUM_RIGHE_DETTAGLI))
                        Num_Righe_Riporti = Num_Pagine - 1
                        Num_Righe_DS = Num_DettagliXML + Num_Righe_Riporti
                        'aggiorno il numero di pagine totali, contando anche le righe dei riporti
                        Num_Pagine = Math.Ceiling(CDbl(Num_Righe_DS / NUM_RIGHE_DETTAGLI))

                        For i = 0 To Num_DettagliXML - 1

                            j += 1

                            XML_DettaglioVariabiliStampe = XMLs_DettaglioVariabiliStampe.Item(i)

                            data_bolla = XML_DettaglioVariabiliStampe.GetAttribute("data_bolla")
                            numero_bolla = XML_DettaglioVariabiliStampe.GetAttribute("numero_bolla")
                            conferente_piva = XML_DettaglioVariabiliStampe.GetAttribute("conferente_piva")
                            conferente_rag_soc = XML_DettaglioVariabiliStampe.GetAttribute("conferente_rag_soc")

                            'calcolo la pagina corrente in base al numero di dettaglio a cui sono arrivato
                            Num_Pag_Corrente = Math.Ceiling(CDbl(j / NUM_RIGHE_DETTAGLI))

                            If Num_Pag_Corrente <> TempPag Then
                                'sono alla prima riga della nuova pagina
                                'aggiorno la  pagina temp
                                TempPag = Num_Pag_Corrente
                                'aggiungo la riga con il riporto
                                RigaDs = DS.DS_LibroConferimenti.NewDS_LibroConferimentiRow
                                RigaDs.Num_Pagina = Num_Pag_Corrente
                                RigaDs.piva = Piva
                                RigaDs.rag_soc = rag_soc
                                RigaDs.contratto_des = contratto_des
                                RigaDs.Validita_Inizio = Validita_Inizio
                                RigaDs.Validita_Fine = Validita_Fine
                                RigaDs.peso_netto_complessivo = 0 'peso_netto_complessivo
                                RigaDs.peso_netto_complessivo_dbl = 0 'peso_netto_complessivo_dbl
                                RigaDs.data_bolla = ""
                                RigaDs.numero_bolla = ""
                                RigaDs.conferente_piva = "" 'conferente_piva
                                RigaDs.conferente_rag_soc = ""
                                RigaDs.dettagli_coltura = "Riporto"
                                RigaDs.udm_des = udm_des
                                RigaDs.peso_netto = 0
                                RigaDs.peso_netto_dbl = Totale_kg
                                RigaDs.peso_effettivo_dbl = Totale_effettivo_kg
                                RigaDs.peso_netto_dbl_lt = Totale_lt
                                RigaDs.lotto_interno = ""

                                'Inserisco la riga
                                DS.DS_LibroConferimenti.Rows.Add(RigaDs)
                                j += 1
                            End If

                            dettagli_coltura = XML_DettaglioVariabiliStampe.GetAttribute("dettagli_coltura")
                            udm_des = XML_DettaglioVariabiliStampe.GetAttribute("udm_des")
                            'dettagli_coltura = dettagli_coltura & " (" & udm_des & ")"

                            'peso_netto = XML_DettaglioVariabiliStampe.GetAttribute("peso_netto")

                            'If XML_DettaglioVariabiliStampe.GetAttribute("peso_netto") <> "" Then
                            '    peso_netto_dbl = CDbl(XML_DettaglioVariabiliStampe.GetAttribute("peso_netto"))
                            'Else
                            '    peso_netto_dbl = 0
                            'End If

                            If XML_DettaglioVariabiliStampe.GetAttribute("peso_netto") <> "" Then
                                peso_netto = CDbl(XML_DettaglioVariabiliStampe.GetAttribute("peso_netto"))
                            Else
                                peso_netto = 0
                            End If

                            If XML_DettaglioVariabiliStampe.GetAttribute("peso_effettivo") <> "" Then
                                peso_effettivo = CDbl(XML_DettaglioVariabiliStampe.GetAttribute("peso_effettivo"))
                            Else
                                peso_effettivo = 0
                            End If

                            Select Case udm_des.ToLower
                                Case "kg"
                                    peso_netto_dbl_kg = peso_netto
                                    Totale_kg += peso_netto_dbl_kg
                                    peso_netto_dbl_lt = 0

                                    Totale_effettivo_kg += peso_effettivo

                                Case Else
                                    peso_netto_dbl_lt = peso_netto
                                    Totale_lt += peso_netto_dbl_lt
                                    peso_netto_dbl_kg = 0
                            End Select

                            lotto_interno = XML_DettaglioVariabiliStampe.GetAttribute("lotto_interno")

                            'Totale += peso_netto_dbl


                            ' --- inserisco la riga nel dataset..
                            RigaDs = DS.DS_LibroConferimenti.NewDS_LibroConferimentiRow

                            'Associo i dati

                            RigaDs.Num_Pagina = Num_Pag_Corrente
                            RigaDs.piva = Piva
                            RigaDs.rag_soc = rag_soc
                            RigaDs.contratto_des = contratto_des
                            RigaDs.Validita_Inizio = Validita_Inizio
                            RigaDs.Validita_Fine = Validita_Fine
                            RigaDs.peso_netto_complessivo = 0 'peso_netto_complessivo
                            RigaDs.peso_netto_complessivo_dbl = 0 'peso_netto_complessivo_dbl
                            RigaDs.data_bolla = data_bolla
                            RigaDs.numero_bolla = numero_bolla
                            RigaDs.conferente_piva = conferente_piva
                            RigaDs.conferente_rag_soc = conferente_rag_soc
                            RigaDs.dettagli_coltura = dettagli_coltura
                            RigaDs.udm_des = udm_des
                            RigaDs.peso_netto = peso_netto
                            ' RigaDs.peso_netto_dbl = peso_netto_dbl
                            RigaDs.peso_netto_dbl = peso_netto_dbl_kg
                            RigaDs.peso_netto_dbl_lt = peso_netto_dbl_lt
                            RigaDs.peso_effettivo_dbl = peso_effettivo
                            RigaDs.lotto_interno = lotto_interno

                            'Inserisco la riga
                            DS.DS_LibroConferimenti.Rows.Add(RigaDs)

                        Next

                    End If

                Else
                    Log_Errori += "- Lettura dei dati: " + vbCrLf + "Non sono arrivati dati dal GiasLan." + vbCrLf + vbCrLf
                End If

            Catch ex As Exception
                Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
            End Try

        Catch ex As Exception
            Log_Errori += "- elaborazione dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATI
            '--------------------------------------------
            rptStampa.SetDataSource(DS)

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################
    End Sub


End Class