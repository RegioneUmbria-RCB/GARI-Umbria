Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility

Public Class ImpegnativaConferimentoProdotti
    Inherits System.Web.UI.Page

    Private rptImpegnativaConferimento As Rpt_ImpegnativaConferimentoProdotti
    Private Log_Errori As String

    Dim Qs_Piva As String

    Dim Param_Dichiarazione As String = ""
    Dim Param_Rag_Soc As String = ""


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        rptImpegnativaConferimento = New Rpt_ImpegnativaConferimentoProdotti

    End Sub

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        If String.IsNullOrEmpty(Qs_Piva) Then
            Throw New Exception("Azienda non selezionata")
        End If


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim ParametriAgronicaStampe_2010 As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010 = Session("ParametriAgronicaStampe_2010")
        Dim XmlDoc = New System.Xml.XmlDocument


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "Impegnativa_Conferimento"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_Conferimento(Qs_Piva)

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
                objGestFile.SalvaReportPdf(rptImpegnativaConferimento,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Salvo l'rpt su filesystem per poterlo ricaricare dal VisualizzatoreReport.aspx
                rptImpegnativaConferimento.SaveAs(reportTemporaneo, True)
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
                                                 "Stampa_ImpegnativaConferimento",
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
    Private Sub Stampa_Conferimento(piva As String)

        Dim DS_ConferimentoDet As New DS_ImpegnativaConferimentoProdotti
        Dim DR_ConferimentoDet As DS_ImpegnativaConferimentoProdotti.DT_DATIRow
        Dim DR_DatiGenerali As DS_ImpegnativaConferimentoProdotti.DT_DATI_GENERALIRow
        Dim DT As DataTable = Nothing
        Dim DTGenerali As DataTable = Nothing


        '-----------------------------------------
        '---- Query di lettura  -------------
        '-----------------------------------------   

        Try

            Dim objTerz As New AgronicaCoreStampeDAL.MaterialeVivaistico

            'DT = objTerz.LeggiMovimentiTerzisti(Qs_Piva,
            '                                    "", "",
            '                                    objParametri_Server)

            DT = objTerz.leggiDatiConferimentoDettaglio(piva, objParametri_Server.FinestraTemporaleInizio, objParametri_Server.FinestraTemporaleFine, False, objParametri_Server)
            DTGenerali = objTerz.leggiDatiConferimentoGenerale(piva, objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_ModelloTerzistiBio: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            If DTGenerali.Rows.Count = 0 Then
                Throw New Exception("Errore nella stampe")
            End If
            Dim anno = Year(Now).ToString
            DR_DatiGenerali = DS_ConferimentoDet.DT_DATI_GENERALI.NewRow

            DR_DatiGenerali.Anno = anno
            DR_DatiGenerali.Annata_Agraria = "2023/2024"
            DR_DatiGenerali.Logo = OttieniLogo()
            DR_DatiGenerali.Rag_Soc = changeIfEmpty(DTGenerali(0)("rag_soc"), "")
            DR_DatiGenerali.Indirizzo = changeIfEmpty(DTGenerali(0)("ind_des"), "")
            DR_DatiGenerali.Indirizzo2 = changeIfEmpty(DTGenerali(0)("cap") & " " & DTGenerali(0)("com_des") & " (" & DTGenerali(0)("pro_cod") & ")", "")
            DR_DatiGenerali.Rag_Soc = changeIfEmpty(DTGenerali(0)("rag_soc"), "")
            DR_DatiGenerali.Telefono = changeIfEmpty(DTGenerali(0)("Telefono"), "")
            DR_DatiGenerali.Cellulare = changeIfEmpty(DTGenerali(0)("Cellulare"), "")
            DR_DatiGenerali.Fax = changeIfEmpty(DTGenerali(0)("Fax"), "")
            DR_DatiGenerali.Email = changeIfEmpty(DTGenerali(0)("Email"), "")
            DR_DatiGenerali.Pec = changeIfEmpty(DTGenerali(0)("Pec"), "")
            DR_DatiGenerali.UfficioRea = changeIfEmpty(DTGenerali(0)("UfficioRea"), "")
            DR_DatiGenerali.NumeroRea = changeIfEmpty(DTGenerali(0)("numeroRea"), "")
            DR_DatiGenerali.CUAA = changeIfEmpty(DTGenerali(0)("Cuaa"), "")

            DS_ConferimentoDet.DT_DATI_GENERALI.Rows.Add(DR_DatiGenerali)
            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1

                    DR_ConferimentoDet = DS_ConferimentoDet.DT_DATI.NewRow

                    ' DR_Terzisti.nome_campo = DT.Rows(i).Item("nome_colonna")
                    ' DR_Terzisti.nome_campo = DT.Rows(i).Item("nome_colonna")
                    ' DR_Terzisti.nome_campo = DT.Rows(i).Item("nome_colonna")

                    DR_ConferimentoDet.Coltura = changeIfEmpty(DT.Rows(i).Item("NomeProdotto"), " ")
                    DR_ConferimentoDet.Ha_Fascicolo = changeIfEmpty(DT.Rows(i).Item("HaFascicolo"), 0.0)
                    DR_ConferimentoDet.Previsione_Produttiva = changeIfEmpty(DT.Rows(i).Item("Previsione_Produttiva"), 0.0)

                    DS_ConferimentoDet.DT_DATI.Rows.Add(DR_ConferimentoDet)

                Next
            End If

        Catch ex As Exception
            Log_Errori += "- caricamento dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATASET AL REPORT
            '--------------------------------------------
            DS_ConferimentoDet.DT_DATI.Select()
            DS_ConferimentoDet.DT_DATI_GENERALI.Select()
            rptImpegnativaConferimento.SetDataSource(DS_ConferimentoDet)
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

            Dim path = objWebConfig.Path_Directory_Loghi_Cliente & objParametri_Server.PivaSuperUser & "_logoOrdiniVivaio.jpg"

            If IO.File.Exists(path) = True Then
                byteLogo = My.Computer.FileSystem.ReadAllBytes(path)
            End If

        End If

        Return byteLogo

    End Function

End Class