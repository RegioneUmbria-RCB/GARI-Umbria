Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Quadro_P
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_Quadro_P
    Private DsQuadroP As DS_Quadro_P

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim QS_Data As String
    Dim QS_Flag_Centro As Integer
    Dim QS_Flag_SoloOccupate As Integer

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_Quadro_P

    End Sub

    '###############################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '#################################################################################
        '#####  Recupero la data di stampa dalla QueryString 
        '#################################################################################

        QS_Data = Stringa_Decodifica(Request.QueryString("ds").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        QS_Flag_SoloOccupate = 0
        If Not IsNothing(CStr(Request.QueryString("so"))) AndAlso IsNumeric(Stringa_Decodifica(CStr(Request.QueryString("so")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)) Then
            QS_Flag_SoloOccupate = CInt(Stringa_Decodifica(CStr(Request.QueryString("so")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        End If

        QS_Flag_Centro = 0
        If Not IsNothing(CStr(Request.QueryString("c"))) AndAlso IsNumeric(Stringa_Decodifica(CStr(Request.QueryString("c")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)) Then
            QS_Flag_Centro = CInt(Stringa_Decodifica(CStr(Request.QueryString("c")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        End If
     


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '##############################################################
        '#####  Recupero piva e sa_cod dalla stringa xml
        '##############################################################

        Dim Log_Errori As String = ""

        'Dim strXmlVariabilistampe As String
        'Dim htVariabiliStampe As System.Collections.Hashtable
        'Dim strErr As String
        'Dim XmlDoc As New System.Xml.XmlDocument
        'Dim XML_FiltroStampa As System.Xml.XmlElement

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer
        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "QuadroP"


        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DsQuadroP As New DS_Quadro_P

            Dim Data As String = Format(CDate(QS_Data), "dd-MM-yyyy")

            Try
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Qs_Piva, objParametri_Server)

                CType(rptStampa.Sezione1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                CType(rptStampa.Sezione1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale

                If Not Qs_Sa_Cod Is Nothing AndAlso Qs_Sa_Cod <> "0" Then
                    Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    CType(rptStampa.Sezione1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                End If

                CType(rptStampa.Sezione1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CDate(QS_Data).ToShortDateString 'CStr(CDate(QS_Data).Year)

                If QS_Flag_Centro = 1 Then
                    CType(rptStampa.Sezione1.ReportObjects("Text1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Centro/Coltura"
                End If

                objImprese = Nothing

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Carica_DsQuadroP(DsQuadroP, Log_Errori)


                Try

                    '--------------------------------------------
                    ' AGGANCIO DATI
                    '--------------------------------------------
                    rptStampa.SetDataSource(DsQuadroP)

                Catch ex As Exception
                    Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
                End Try




                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.QuadroP, "", "", objParametri_Server)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptStampa, _
                                           enum_CategorieDocumenti.QuadroP, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" + Qs_Piva + "_d" + Data + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer


                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva, _
                                                                 enum_CategorieDocumenti.QuadroP, _
                                                                 "Quadro P", _
                                                                 Nome_Documento + "_p" + Qs_Piva + "_d" + Data + ".pdf", _
                                                                 Sottocartella, _
                                                                 "", "", "", "", _
                                                                 CDate("01/01/" & CDate(QS_Data).Year.ToString), _
                                                                 CDate("31/12/" & CDate(QS_Data).Year.ToString), _
                                                                 objParametri_Server)


            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Qs_Piva) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Qs_Piva + "_d" + Data + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "QuadroP"
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_Contabilita"
                End If

                Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = Path_Errore,
                    .LogFileName = Nome_File
                }

                'objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Qs_Piva.Page_Load", Log_Errori)
                objLog.Scrivi_LOG(objParametri_Server, "Qs_Piva.Page_Load", Log_Errori, CustomLOGParams:=CustomLOGParams)


            End If
            '-----------------------------------------

            Session("Report") = rptStampa
            Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        End If

    
    End Sub




    '###########################################################################
    'riempie il ds x il Quadro P...
    Private Sub Carica_DsQuadroP(ByRef DsQuadroP As DS_Quadro_P, _
                                        ByRef Log_Errori As String)


        Dim Codice_Anagrafe_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim i As Integer
        Dim DestinazioneUso As String
        Dim DT As DataTable
        Dim objQuadroP As New AgronicaCoreStampeDAL.AnagraficaAziendale

        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String
        Dim Sup_Condotta As Double
        Dim Sup_Utilizzata As Double
        Dim Sup_Residua As Double
        Dim Ettari_Sup_Residua As Integer
        Dim Are_Sup_Residua As Integer
        Dim Centiare_Sup_Residua As Integer

        Dim Flag_Centro As Boolean = False

        If QS_Flag_Centro = 1 Then
            Flag_Centro = True
        End If

        Try

            DT = objQuadroP.Quadro_P(Qs_Piva, _
                                            Qs_Sa_Cod, _
                                            QS_Data, _
                                            Flag_Centro, _
                                            QS_Flag_SoloOccupate, _
                                            True, _
                                            False, _
                                            False, _
                                            "", _
                                            "", _
                                            "", _
                                            "", _
                                            objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Esecuzione query: " + vbCrLf + ex.Message + vbCrLf
        End Try

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Try

                Dim DtUtilizzi As New DataTable
                DtUtilizzi.Columns.Add(New DataColumn("prov", GetType(String)))
                DtUtilizzi.Columns.Add(New DataColumn("com", GetType(String)))
                DtUtilizzi.Columns.Add(New DataColumn("sezione", GetType(String)))
                DtUtilizzi.Columns.Add(New DataColumn("foglio", GetType(Integer)))
                DtUtilizzi.Columns.Add(New DataColumn("numero", GetType(Integer)))
                DtUtilizzi.Columns.Add(New DataColumn("subalterno", GetType(String)))
                DtUtilizzi.Columns.Add(New DataColumn("sup_condotta", GetType(Double)))
                DtUtilizzi.Columns.Add(New DataColumn("sup_utilizzata", GetType(Double)))
                DtUtilizzi.Columns.Add(New DataColumn("sup_residua", GetType(Double)))

                DT.Columns.Add(New DataColumn("sup_residua", GetType(Double)))
                DT.Columns.Add(New DataColumn("ettari_sup_residua", GetType(Integer)))
                DT.Columns.Add(New DataColumn("are_sup_residua", GetType(Integer)))
                DT.Columns.Add(New DataColumn("centiare_sup_residua", GetType(Integer)))

                If Flag_Centro = False Then
                    DT.Columns.Add(New DataColumn("sa_nome", GetType(String)))
                End If

                'per gli appezzamenti intersecati con particelle controllo le validita degli impianti 
                'e lascio la descrizione solo su quelli attivi alla data di stampa
                For i = 0 To DT.Rows.Count - 1

                    If Flag_Centro = False Then
                        DT.Rows(i).Item("sa_nome") = ""
                    End If

                    If CStr(DT.Rows(i).Item("Inizio_Impianto")) <> "" And _
                       CStr(DT.Rows(i).Item("Fine_Impianto")) <> "" Then
                        If Not (CDate(DT.Rows(i).Item("Inizio_Impianto")) <= CDate(QS_Data) And _
                                CDate(DT.Rows(i).Item("Fine_Impianto")) >= CDate(QS_Data)) Then
                            DT.Rows(i).Item("Veg_Des") = ""
                            DT.Rows(i).Item("Cul_Des") = ""
                        End If
                    End If

                    If (DT.Rows(i).Item("Appezza") <> 0 And _
                       DT.Rows(i).Item("Id_Reg") <> 0) And _
                       IsDBNull(DT.Rows(i).Item("Cul_Des")) Then

                        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                        Dim DtDest As DataTable
                        DtDest = objRegImpianti.Leggi_DestinazioneUso_Impianto2(CStr(DT.Rows(i).Item("Piva")), _
                                                                                CInt(DT.Rows(i).Item("sa_cod")), _
                                                                                CInt(DT.Rows(i).Item("Appezza")), _
                                                                                CInt(DT.Rows(i).Item("Id_Reg")), _
                                                                                "", "", objParametri_Server)

                        objRegImpianti = Nothing

                        If DtDest.Rows.Count = 1 Then
                            DestinazioneUso = DtDest.Rows(0).Item("descrizione")
                        End If

                        DT.Rows(i).Item("Veg_Des") = "Terreno nudo"

                        DT.Rows(i).Item("Cul_Des") = DestinazioneUso

                    End If


                    Prov = DT.Rows(i).Item("prov")
                    Com = DT.Rows(i).Item("com")
                    Sezione = DT.Rows(i).Item("sezione")
                    Foglio = DT.Rows(i).Item("foglio")
                    Numero = DT.Rows(i).Item("numero")
                    Subalterno = DT.Rows(i).Item("subalterno")

                    Sup_Utilizzata = 0
                    Sup_Residua = 0
                    Sup_Condotta = DT.Rows(i).Item("sup_condotta")

                    Dim DrUtilizzi() As DataRow = DtUtilizzi.Select("prov='" & Prov & "'" & _
                                                            " AND com='" & Com & "'" & _
                                                            " AND sezione='" & Sezione & "'" & _
                                                            " AND foglio=" & Foglio.ToString & "" & _
                                                            " AND numero=" & Numero.ToString & "" & _
                                                            " AND subalterno='" & Subalterno & "'")
                    'già calcolata
                    If Not DrUtilizzi Is Nothing AndAlso DrUtilizzi.Length > 0 Then
                        Sup_Utilizzata = DrUtilizzi(0).Item("sup_utilizzata")
                        Sup_Residua = DrUtilizzi(0).Item("Sup_Residua")
                    Else
                        'calcolo sup utilizzata tot
                        Dim DrParticella() As DataRow
                        Dim p As Integer
                        DrParticella = DT.Select("prov='" & Prov & "'" & _
                                                 " AND com='" & Com & "'" & _
                                                 " AND sezione='" & Sezione & "'" & _
                                                 " AND foglio=" & Foglio.ToString & "" & _
                                                 " AND numero=" & Numero.ToString & "" & _
                                                 " AND subalterno='" & Subalterno & "'")
                        If Not DrParticella Is Nothing AndAlso DrParticella.Length > 0 Then
                            For p = 0 To DrParticella.Length - 1
                                Sup_Utilizzata += DrParticella(p).Item("sup_util")
                            Next
                        End If

                        Sup_Residua = Sup_Condotta - Sup_Utilizzata

                        'aggiungo la riga
                        Dim Dr As DataRow
                        Dr = DtUtilizzi.NewRow
                        Dr.Item("prov") = Prov
                        Dr.Item("com") = Com
                        Dr.Item("sezione") = Sezione
                        Dr.Item("foglio") = Foglio
                        Dr.Item("numero") = Numero
                        Dr.Item("subalterno") = Subalterno
                        Dr.Item("sup_condotta") = Sup_Condotta
                        Dr.Item("Sup_Utilizzata") = Sup_Utilizzata
                        Dr.Item("Sup_Residua") = Sup_Residua
                        DtUtilizzi.Rows.Add(Dr)

                    End If

                    Ettari_Sup_Residua = 0
                    Are_Sup_Residua = 0
                    Centiare_Sup_Residua = 0
                    EttariAreCentiare_from_Ettari(Sup_Residua, Ettari_Sup_Residua, Are_Sup_Residua, Centiare_Sup_Residua)

                    DT.Rows(i).Item("sup_residua") = Sup_Residua
                    DT.Rows(i).Item("ettari_sup_residua") = Ettari_Sup_Residua
                    DT.Rows(i).Item("are_sup_residua") = Are_Sup_Residua
                    DT.Rows(i).Item("centiare_sup_residua") = Centiare_Sup_Residua

                Next

            Catch ex As Exception
                Log_Errori += "- Elaborazione datatable: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                'imposto il nome del datatable  
                DT.TableName = "DT_Quadro_P"
                DsQuadroP.Merge(DT)
                DsQuadroP.AcceptChanges()

            Catch ex As Exception
                Log_Errori += "- Riempimento dataset dal datable: " + vbCrLf + ex.Message + vbCrLf
            End Try

        Else
            'non ci sono dati
            'nascondo le sezioni
            rptStampa.Sezione2.SectionFormat.EnableSuppress = True
            rptStampa.Sezione3.SectionFormat.EnableSuppress = True
            rptStampa.Sezione4.SectionFormat.EnableSuppress = True
            rptStampa.Sezione5.SectionFormat.EnableSuppress = True
        End If

    End Sub


End Class