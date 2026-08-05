Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreContabBIZ

Public Class ObiettivoDiProduzione
    Inherits System.Web.UI.Page

    Private rptObiettivoDiProduzione As Rpt_ObiettivoDiProduzione
    Private Log_Errori As String

    Dim Qs_Piva As String
    Dim Qs_DataInizio As Date
    Dim Qs_DataFine As Date

    Dim Param_Dichiarazione As String = ""
    Dim Param_Rag_Soc As String = ""


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        rptObiettivoDiProduzione = New Rpt_ObiettivoDiProduzione

    End Sub

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)
        Qs_DataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")),
                                   AgroKey_EncoderDecoder,
                                   Server)
        Qs_DataFine = Stringa_Decodifica(CStr(Request.QueryString("df")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        If False And String.IsNullOrEmpty(Qs_Piva) Then
            Throw New Exception("Azienda non selezionata")
        End If


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim ParametriAgronicaStampe_2010 As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010 = Session("ParametriAgronicaStampe_2010")
        Dim XmlDoc = New System.Xml.XmlDocument


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "Obiettivo_Produzione"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_ObiettivoProduzione(Qs_Piva)

            Catch exc As Exception
                Log_Errori += "- Lettura dati: " + vbCrLf + exc.Message + vbCrLf
            End Try



            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                'TODO Creare o scegliere una enum_CategorieDocumenti specifica
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

                Dim Nome_Documento_Estensione = Nome_Documento & ".pdf"

                'Salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptObiettivoDiProduzione,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Salvo l'rpt su filesystem per poterlo ricaricare dal VisualizzatoreReport.aspx
                rptObiettivoDiProduzione.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
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
                                                 "MaterialeVivaistico",
                                                 Nome_File_Log,
                                                 Session("ASG_Utente_Username"),
                                                 "Stampa_ObiettivoProduzione",
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                            "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))



        End If

    End Sub
    Private Function changeIfEmpty(Of T)(element As Object, defaultValue As T) As T
        If IsDBNull(element) OrElse String.IsNullOrEmpty(element) Then
            Return defaultValue
        End If
        Return element
    End Function
    '#####################################################################################################
    Private Sub Stampa_ObiettivoProduzione(piva As String)

        Dim DS_ObiettivoDiProduzione As New DS_ObiettivoDiProduzione
        Dim DR_ObiettivoDiProduzione As DS_ObiettivoDiProduzione.DT_DATIRow
        Dim DR_ObiettivoDiProduzione2 As DS_ObiettivoDiProduzione.DT_DATI2Row
        Dim DR_DatiGenerali As DS_ObiettivoDiProduzione.DT_Dati_GeneraliRow
        Dim DT As DataTable = Nothing
        Dim DTGenerali As DataTable = Nothing


        '-----------------------------------------
        '---- Query di lettura  -------------
        '-----------------------------------------   
        Dim expDT As List(Of IDictionary(Of String, Object))
        Dim expDT2 As List(Of IDictionary(Of String, Object))
        Dim objTerz As New AgronicaCoreStampeDAL.MaterialeVivaistico

        Try


            'DT = objTerz.LeggiMovimentiTerzisti(Qs_Piva,
            '                                    "", "",
            '                                    objParametri_Server)

            'DT = objTerz.leggiDatiConferimentoDettaglio(piva, objParametri_Server.FinestraTemporaleInizio, objParametri_Server.FinestraTemporaleFine, False, objParametri_Server)
            Dim impreseContrattiR As New Imprese_Contratti_BIZ_R
            'piva = "02099120202"
            'expDT = impreseContrattiR.readContracts(0, objParametri_Server.FinestraTemporaleInizio, objParametri_Server.FinestraTemporaleFine, 0, True, objParametri_Server, 0, piva).ToExpandoObject

            expDT2 = impreseContrattiR.readDettaglioAziendale(piva, 0, 0, Qs_DataInizio, Qs_DataFine, objParametri_Server).ToExpandoObject
            'expDT = objTerz.leggiContratti(piva, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server).ToExpandoObject

            DTGenerali = objTerz.leggiDatiConferimentoGenerale(piva, objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_ModelloTerzistiBio: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            Dim anno = Year(Now).ToString
            DR_DatiGenerali = DS_ObiettivoDiProduzione.DT_Dati_Generali.NewRow
            DR_DatiGenerali.Anno = anno
            DR_DatiGenerali.Annata_Agraria = Qs_DataInizio.Year.ToString + "/" + (Qs_DataFine.Year + 1).ToString
            DR_DatiGenerali.logo = OttieniLogo()


            If DTGenerali.Rows.Count = 0 Then
                'rptObiettivoDiProduzione.ReportDefinition.Sections("Section5").SectionFormat.EnableSuppress = True
                'rptObiettivoDiProduzione.ReportDefinition.Sections("PageFooterSection5").SectionFormat.EnableSuppress = False
            Else
                DR_DatiGenerali.rag_soc = changeIfEmpty(DTGenerali(0)("rag_soc"), "")
                DR_DatiGenerali.ind = changeIfEmpty(DTGenerali(0)("ind_des"), "")
                DR_DatiGenerali.ind2 = changeIfEmpty(DTGenerali(0)("cap") & " " & DTGenerali(0)("com_des") & " (" & DTGenerali(0)("pro_cod") & ")", "")
                DR_DatiGenerali.rag_soc = changeIfEmpty(DTGenerali(0)("rag_soc"), "")
                DR_DatiGenerali.Telefono = changeIfEmpty(DTGenerali(0)("Telefono"), "")
                DR_DatiGenerali.Cellulare = changeIfEmpty(DTGenerali(0)("Cellulare"), "")
                DR_DatiGenerali.Fax = changeIfEmpty(DTGenerali(0)("Fax"), "")
                DR_DatiGenerali.Email = changeIfEmpty(DTGenerali(0)("Email"), "")
                DR_DatiGenerali.Pec = changeIfEmpty(DTGenerali(0)("Pec"), "")
                DR_DatiGenerali.UfficioRea = changeIfEmpty(DTGenerali(0)("UfficioRea"), "")
                DR_DatiGenerali.NumeroRea = changeIfEmpty(DTGenerali(0)("numeroRea"), "")
                DR_DatiGenerali.CUAA = changeIfEmpty(DTGenerali(0)("Cuaa"), "")
                DR_DatiGenerali.piva = changeIfEmpty(DTGenerali(0)("PivaReale"), "")
            End If



            DS_ObiettivoDiProduzione.DT_Dati_Generali.Rows.Add(DR_DatiGenerali)

            Dim objCentriXIndirzzi = New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            expDT2.
                GroupBy(Function(row) row("Prodotto")).
                Select(Function(g)
                           Dim newRow As DS_ObiettivoDiProduzione.DT_DATIRow = DS_ObiettivoDiProduzione.DT_DATI.NewRow()
                           newRow.Coltura = g.Key
                           newRow.Ha = g.Sum(Function(row) CDbl(row("Superficie")))
                           Return newRow
                       End Function).
                ToList.
                ForEach(Sub(row) DS_ObiettivoDiProduzione.DT_DATI.Rows.Add(row))

            Dim dictTrasporto = objTerz.leggiFasiCod(
                expDT2.Select(Of Integer)(Function(x) x("Fase_Cod")).ToList, objParametri_Server) _
                .ToExpandoObject() _
                .Where(Function(x) Not IsDBNull(x("tipo_trasporto"))) _
                .ToDictionary(Of String, String)(Function(x) x("fase_cod"), Function(x) x("tipo_trasporto"))

            expDT2.
                Select(Function(row)
                           Dim newRow As DS_ObiettivoDiProduzione.DT_DATI2Row = DS_ObiettivoDiProduzione.DT_DATI2.NewRow()
                           newRow.Coltura = row("Prodotto")
                           Dim dati = objCentriXIndirzzi.Leggi(row("Piva_Azienda"), row("Sa_Cod_Azienda"), 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

                           newRow.Ditta_ricevente = row("Rag_Soc_Azienda")
                           If dati.Rows.Count > 0 Then
                               newRow.Ditta_ricevente += " stabilimento di " + dati(0)("com_des") + " - " + dati(0)("ind_des") + If(dati(0)("com_des") <> dati(0)("frz_des"), " - " + dati(0)("frz_des"), "")
                           End If
                           newRow.Previsione_Produttiva = CDbl(row("QtaPrevista")) / 1000.0F


                           If dictTrasporto.ContainsKey(row("Fase_Cod")) Then

                               Select Case dictTrasporto(row("Fase_Cod"))
                                   Case 0
                                       newRow.Trasporto = ""
                                   Case 1
                                       newRow.Trasporto = "Franco Fabbrica"
                                   Case 2
                                       newRow.Trasporto = "Trasportato"
                                   Case 3
                                       newRow.Trasporto = "Misto"
                               End Select
                           End If
                           Return newRow
                       End Function).
                         ToList.
                         ForEach(Sub(row) DS_ObiettivoDiProduzione.DT_DATI2.Rows.Add(row))



        Catch ex As Exception
            Log_Errori += "- caricamento dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATASET AL REPORT
            '--------------------------------------------
            DS_ObiettivoDiProduzione.DT_DATI.Select()
            DS_ObiettivoDiProduzione.DT_Dati_Generali.Select()
            rptObiettivoDiProduzione.SetDataSource(DS_ObiettivoDiProduzione)
            'rptStampaOrdini.ReportDefinition.Sections("Section4").SectionFormat.EnableSuppress = True

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################

        Try

            '--------------------------------------------
            ' IMPOSTAZIONE PARAMETRI
            '(va fatto dopo il SetDataSource altrimenti da errore! )
            '--------------------------------------------

            'rptImpegnativaConferimento.SetParameterValue("anno", "2023")

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub
    Private Function OttieniLogo() As Byte()

        'di default inserisco un'immagine bianca come logo
        Dim bmpImg As New Drawing.Bitmap(1, 1)
        bmpImg.SetPixel(0, 0, Drawing.Color.White)

        Dim cx As New Drawing.ImageConverter
        Dim logoBianco() As Byte
        logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))

        Dim byteLogo As Byte() = logoBianco

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

            objWebConfig.Path_Directory_Loghi_Cliente = FileSystemHelper.AggiungiSlashSeNonEsiste(objWebConfig.Path_Directory_Loghi_Cliente)

            Dim path = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Asipo_2023.jpg"

            If IO.File.Exists(path) = True Then
                byteLogo = My.Computer.FileSystem.ReadAllBytes(path)
            End If

        End If

        Return byteLogo

    End Function

End Class