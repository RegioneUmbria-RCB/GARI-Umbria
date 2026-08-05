Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports CrystalDecisions.CrystalReports.Engine



Public Class BollaCampionatura
    Inherits System.Web.UI.Page

    Private rptStampa As ReportClass

    Dim Piva As String
    Dim arr_Id_Mov_Det() As Integer
    Dim Log_Errori As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Report As Integer

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        'Parametro str_Id_Mov_Det -> stringa formattata con un array di valori Id_Mov_Det
        Dim str_Id_Mov_Det As String = Stringa_Decodifica(CStr(Request.QueryString("idm")), AgroKey_EncoderDecoder, Server)
        '******************************************
        'DEBUG
        'str_Id_Mov_Det = "88083|88190|91524|91526"
        '******************************************
        Dim arr_str_Id_Mov_Det() As String = str_Id_Mov_Det.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
        arr_Id_Mov_Det = Array.ConvertAll(arr_str_Id_Mov_Det, Function(str) Int32.Parse(str))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "BollaCampionatura"
        Select Case objParametri_Server.PivaSuperUser
            Case "01271980391"
                ' FRUTTAGL  
                rptStampa = New Bolla_Campionatura_FRUTTAGEL_FF
            Case "00040710295"
                ' COFRUTA
                rptStampa = New Bolla_Campionatura
            Case Else
                rptStampa = New Bolla_Campionatura
        End Select

        If Not Me.IsPostBack Then

            Dim DSCampionamento As New DS_Campionamento

            Try

                Log_Errori = ""

                Stampa_BollaCampionatura(DSCampionamento)

            Catch exc As Exception
                Log_Errori += "- Stampa: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try
                'Dim IdentificazioneDocumento As String

                'IdentificazioneDocumento = "_" + DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")

                '' leggo la sottocartella da CategorieDocumenti
                'Dim Sottocartella As String
                'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                'Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Conferimento, "", "", objParametri_Server)

                '' salvo il report in formato PDF
                'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                'objGestFile.SalvaReportPdf(rptStampa,
                '                           enum_CategorieDocumenti.Conferimento,
                '                           Sottocartella,
                '                           Nome_Documento + IdentificazioneDocumento + ".pdf",
                '                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'Dim agroWebCfg As AgronicaCoreGestioneRichieste.AgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim pdfPath As String = agroWebCfg.PathFileTemporanei
            Dim pdfPath As String = CrystalHelper.getCartellaReportTemporanei()
            Dim pdfTmp As String
            pdfTmp = pdfPath & Path.DirectorySeparatorChar & Nome_Documento & "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff") & ".pdf"

            ''MS Eliminato passaggio report in session per giro su file: Session("Report") = rptStampa
            'Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try

                'rptStampa.SaveAs(reportTemporano, True)
                rptStampa.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pdfTmp)

            Catch ex As Exception
                Log_Errori += "- Generazione pdf temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose del report per evitare problema deallocazione.
            DSCampionamento.Dispose()
            DSCampionamento = Nothing

            rptStampa.Close()
            rptStampa.Dispose()
            rptStampa = Nothing

            GC.Collect()

            'Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
            '                  "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))

            Response.Redirect("..\..\VisualizzatoreReport.aspx?pdf=" + Stringa_Codifica(pdfTmp, AgroKey_EncoderDecoder, Server))

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento +
                            vbCrLf(+vbCrLf + Log_Errori)

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_FreshAndFood", _
                                                 Nome_File & ".txt", _
                                                 Session("ASG_Utente_Username"), _
                                                 "BollaCampionatura.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

        End If

    End Sub

    Private Class TestataBolla
        Public ID As Integer
        Public Data_InizioLavorazione As String
        Public Data_FineLavorazione As String
        Public NrBolla As String
        Public Programma As String
        Public Produttore As String
        Public Prodotto As String
        Public RifDoc As String
        Public DataDoc As String
        Public Lotto As String
        Public Note As String
    End Class

    '#####################################################################
    Private Sub Stampa_BollaCampionatura(ByRef DSCampionamento As DS_Campionamento)

        Try

            'Dim DT_Intestazione As DataTable

            'CType(rptStampa.Section1.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "" 'str_filtro
            'CType(rptStampa.Section2.ReportObjects("TxtAnnoContabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(Anno)
            'CType(rptStampa.Section2.ReportObjects("TxtIntervalloTemporale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_inttemp

            '            Dim objIntest As New FF_CampionamentoConferimento_R
            '            Dim camp_conf_mov As CampionamentoConferito_Movimenti
            '            camp_conf_mov = objIntest.LeggiElem_CampionamentoRigaConferito(Piva, 91526, objParametri_Server)
            '                ByVal piva As String,
            '                ByVal Id_Mov_Det As Integer,
            '                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            '                ) As CampionamentoConferito_Movimenti
            'DT_Intestazione = objIntest.Leggi_Righe_Conferimento(
            'If Not IsNothing(DT_Intestazione) AndAlso DT_Intestazione.Rows.Count > 0 Then
            '    CType(rptStampa.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
            '    CType(rptStampa.Section2.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
            '    CType(rptStampa.Section2.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
            '    CType(rptStampa.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa") & " " & DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "
            'End If


        Catch ex As Exception
            Log_Errori += "- Intestazione report: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '==============================================
        '====== QUERY E CARICAMENTO DATASET ===========
        '==============================================

        Try

            For iTest As Integer = 0 To arr_Id_Mov_Det.Length - 1

                Dim Testata_Bolla As TestataBolla = LeggiTestata(arr_Id_Mov_Det(iTest))
                LeggiRighe(DSCampionamento, Testata_Bolla)

            Next


            'imposto il dataset sul report
            rptStampa.SetDataSource(DSCampionamento)

        Catch exc As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + exc.Message + vbCrLf
        End Try

    End Sub

    '###################################################
    Private Function LeggiTestata(ByVal Id_Mov_Det As Integer) As TestataBolla

        Dim objReader As New FF_CampionamentoConferimento_R
        Dim strTestata As String

        strTestata = objReader.Leggi_Righe_Conferimento(Piva, "", 0, "", 0, "", "", "", "", "", "", -1, {}, {}, {}, {}, "", "", 0, Id_Mov_Det, objParametri_Server)

        Dim objTestata As JObject = JArray.Parse(strTestata)(0)

        Dim Testata_Bolla As New TestataBolla

        Testata_Bolla.ID = objTestata("Id_Mov_Det")
        Testata_Bolla.Data_InizioLavorazione = Convert.ToDateTime(objTestata("Data_InizioLavorazione")).ToString("dd/MM/yyyy HH:mm")
        Testata_Bolla.Data_FineLavorazione = Convert.ToDateTime(objTestata("Data_FineLavorazione")).ToString("dd/MM/yyyy HH:mm")
        Testata_Bolla.NrBolla = objTestata("NrBolla")
        Testata_Bolla.Programma = ""
        Testata_Bolla.Produttore = objTestata("Rag_Soc")
        Testata_Bolla.Prodotto = objTestata("Mat_Des").ToString() & " " &
                            objTestata("Qualita").ToString() & " " &
                            objTestata("Certificazione").ToString() & " " &
                            objTestata("Calibro").ToString() & " " &
                            objTestata("Rugginosita").ToString()
        Testata_Bolla.RifDoc = objTestata("Doc_Numero").ToString()
        If Not objTestata("NrRiga") Is Nothing Then
            Testata_Bolla.RifDoc = Testata_Bolla.RifDoc & objTestata("NrRiga").ToString()
        End If

        Testata_Bolla.DataDoc = Convert.ToDateTime(objTestata("Data_Movimento")).ToString("dd/MM/yyyy")
        Testata_Bolla.Lotto = objTestata("Lotto")
        Testata_Bolla.Note = objTestata("Note")

        Return Testata_Bolla

    End Function

    Private Sub LeggiRighe(ByRef DSCampionamento As DS_Campionamento, ByVal Testata_Bolla As TestataBolla)

        Dim objReader As New FF_CampionamentoConferimento_R
        Dim strRighe As String

        Dim Id_Testata_Griglia_Trovata As Integer
        Dim Id_Testata_Griglia_Prod_Trovata As Integer
        Dim strTestata As String

        strTestata = objReader.Leggi_Id_Testata_Griglia_Da_Movim_Conferimento(Piva,
                            Testata_Bolla.ID, Id_Testata_Griglia_Trovata, Id_Testata_Griglia_Prod_Trovata,
                            False, False, objParametri_Server)

        Dim objTestata As JObject = JObject.Parse(strTestata)

        Testata_Bolla.Programma = objTestata("des_TestataGriglia")

        strRighe = objReader.Leggi_CampionamentoConferito_Movimenti_Righe(Piva,
                        Testata_Bolla.ID,
                        Id_Testata_Griglia_Prod_Trovata,
                        Id_Testata_Griglia_Trovata,
                        objParametri_Server)

        Dim arrRighe As JArray = JArray.Parse(strRighe)

        Dim DR As DS_Campionamento.DT_CampionamentoRow

        For Each objRiga As JObject In arrRighe

            DR = DSCampionamento.DT_Campionamento.NewRow()

            DR.ID = Testata_Bolla.ID
            DR.Data_InizioLavorazione = Testata_Bolla.Data_InizioLavorazione
            DR.Data_FineLavorazione = Testata_Bolla.Data_FineLavorazione
            DR.NrBolla = Testata_Bolla.NrBolla
            DR.Programma = Testata_Bolla.Programma
            DR.Produttore = Testata_Bolla.Produttore
            DR.Prodotto = Testata_Bolla.Prodotto
            DR.RifDoc = Testata_Bolla.RifDoc & " del " & Testata_Bolla.DataDoc
            DR.DataDoc = Testata_Bolla.DataDoc
            DR.Lotto = Testata_Bolla.Lotto
            DR.Note = Testata_Bolla.Note

            DR.Qualita = objRiga("Qualita")
            DR.Calibro = objRiga("Calibro")
            DR.DescrPeso = objRiga("DescrPeso")
            DR.KgCampione = Math.Round(CDec(objRiga("SviluppoCampionato")), 3)
            DR.Perc = CDec(objRiga("PercentualeCampionato"))
            DR.SviluppoTotale = Math.Round(CDec(objRiga("SviluppoTotale")), 0)

            DSCampionamento.DT_Campionamento.Rows.Add(DR)

        Next

        'Try

        'Catch exc As Exception
        '    DT = Nothing
        '    Log_Errori += "- Query BollaCampionatura: " + vbCrLf + exc.Message + vbCrLf
        'End Try

        'Try

        'Catch ex As Exception
        '    Log_Errori += "- CaricaDataset_BollaCampionatura: " + vbCrLf + ex.Message & vbCrLf
        'End Try

    End Sub

End Class