Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility

Public Class PianoColturalePreventivo
    Inherits System.Web.UI.Page

    Private rptStampaOrdini As Rpt_OrdiniVivaio
    Private Log_Errori As String

    Dim Qs_Piva As String

    Dim Param_Dichiarazione As String = ""
    Dim Param_Rag_Soc As String = ""


    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        rptStampaOrdini = New Rpt_OrdiniVivaio

    End Sub

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim ParametriAgronicaStampe_2010 As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010 = Session("ParametriAgronicaStampe_2010")
        Dim XmlDoc = New System.Xml.XmlDocument


        XmlDoc.LoadXml(ParametriAgronicaStampe_2010.Xml_Generico.ToString)
        Dim listaProgrammazione_Cod As New List(Of Integer)
        If XmlDoc.HasChildNodes Then

            Dim XML_Parametri As System.Xml.XmlElement
            'identifico il nodo di destinazione
            XML_Parametri = XmlDoc.SelectSingleNode("VariabiliStampe")
            If Not XML_Parametri Is Nothing Then

                Dim sListaProgrammazione_Cod As String = XML_Parametri.GetAttribute("listaprogrammazione_cod")
                listaProgrammazione_Cod.AddRange(sListaProgrammazione_Cod.Remove(sListaProgrammazione_Cod.Length - 1, 1).Remove(0, 1).Split(",").Select(Of Integer)(Function(s) s).ToList())
            End If
        End If
        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not listaProgrammazione_Cod.Any() Then

        End If
        Dim Nome_Documento As String = "Piano_Colturale_Preventivo"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_PianoColturalePreventivo(listaProgrammazione_Cod)

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
                objGestFile.SalvaReportPdf(rptStampaOrdini,
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               Nome_Documento_Estensione,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Salvo l'rpt su filesystem per poterlo ricaricare dal VisualizzatoreReport.aspx
                rptStampaOrdini.SaveAs(reportTemporaneo, True)
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
                                                 "Stampa_VivaioOrdini",
                                                 Log_Errori)

            End If


            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                            "?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                            "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
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
    Private Sub Stampa_PianoColturalePreventivo(listaProgrammazione_Cod As List(Of Integer))

        Dim DS_StampaOrdini As New DS_PianoColturalePreventivo
        Dim DR_StampaOrdini As DS_StampaOrdini.DT_StampaOrdiniRow
        Dim DR_DatiGenerali As DS_StampaOrdini.DT_DatiGeneraliRow
        Dim DT As DataTable = Nothing


        '-----------------------------------------
        '---- Query di lettura  -------------
        '-----------------------------------------   

        Try

            Dim objTerz As New AgronicaCoreStampeDAL.MaterialeVivaistico

            'DT = objTerz.LeggiMovimentiTerzisti(Qs_Piva,
            '                                    "", "",
            '                                    objParametri_Server)

            DT = objTerz.OrdiniVivaio(listaProgrammazione_Cod, objParametri_Server)

        Catch ex As Exception
            Log_Errori &= "query Stampa_ModelloTerzistiBio: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        'Try

        '    DR_DatiGenerali = DS_StampaOrdini.DT_DatiGenerali.NewRow
        '    DR_DatiGenerali.Anno = "2023"
        '    DR_DatiGenerali.Logo = OttieniLogo()
        '    DS_StampaOrdini.DT_DatiGenerali.Rows.Add(DR_DatiGenerali)
        '    If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

        '        For i = 0 To DT.Rows.Count - 1

        '            DR_StampaOrdini = DS_StampaOrdini.DT_StampaOrdini.NewRow

        '            DS_StampaOrdini.DT_StampaOrdini.Rows.Add(DR_StampaOrdini)

        '        Next
        '    End If

        'Catch ex As Exception
        '    Log_Errori += "- caricamento dataset: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        'Try

        '    '--------------------------------------------
        '    ' AGGANCIO DATASET AL REPORT
        '    '--------------------------------------------
        '    DS_StampaOrdini.DT_StampaOrdini.Select()
        '    rptStampaOrdini.SetDataSource(DS_StampaOrdini)
        '    rptStampaOrdini.ReportDefinition.Sections("Section4").SectionFormat.EnableSuppress = True

        'Catch ex As Exception
        '    Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
        'End Try

        ''#########################################################

        'Try

        '    '--------------------------------------------
        '    ' IMPOSTAZIONE PARAMETRI
        '    '(va fatto dopo il SetDataSource altrimenti da errore! )
        '    '--------------------------------------------

        '    rptStampaOrdini.SetParameterValue("anno", "2023")

        'Catch ex As Exception
        '    Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        'End Try

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