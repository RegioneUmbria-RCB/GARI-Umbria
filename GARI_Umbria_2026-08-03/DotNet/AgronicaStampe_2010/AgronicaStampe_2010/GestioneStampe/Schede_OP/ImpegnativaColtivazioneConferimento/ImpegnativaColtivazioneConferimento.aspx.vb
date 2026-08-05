Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreAnagrafeDAL
Imports System.IO

Public Class ImpegnativaColtivazioneConferimento
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_ImpegnativaColtivazioneConferimento
    Private DSImpegnativaColtivazioneConferimento As DS_ImpegnativaColtivazioneConferimento

    '----- Gestione Querystring
    Dim Piva As String
    Dim anno, validita_inizio, validita_fine As String
    Dim Tipo_Selezione As String
    'Dim Str_FiltroImpianti As String
    Dim fronteRetro As Boolean

    Dim LinkPaginaStampa As String

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private Log_Errori As String

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_ImpegnativaColtivazioneConferimento

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim Log As String
        Dim rag_soc As String
        Dim vuota As Boolean

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

        rag_soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                         AgroKey_EncoderDecoder,
                         Server)

        anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Tipo_Selezione = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        fronteRetro = Stringa_Decodifica(Request.QueryString("fronteretro").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        If Not IsNothing(Request.QueryString("vi")) Then
            validita_inizio = Stringa_Decodifica(Request.QueryString("vi").ToString,
                      AgroKey_EncoderDecoder, Server)
        Else
            validita_inizio = "01/01/" + CStr(anno)
        End If

        If Not IsNothing(Request.QueryString("vf")) Then
            validita_fine = Stringa_Decodifica(Request.QueryString("vf").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            validita_fine = "31/12/" + CStr(anno)
        End If

        If Not IsNothing(Request.QueryString("s")) Then
            vuota = Stringa_Decodifica(Request.QueryString("s"),
                      AgroKey_EncoderDecoder, Server)
        End If

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            Dim DSImpegnativaColtivazioneConferimento As New DS_ImpegnativaColtivazioneConferimento
            Dim DSSpeciexParticelle As New DS_SuperficieSpeciexParticelle
            Dim DSLoghi As New Ds_Loghi
            Dim DSImprese As New Ds_Imprese_StampaMassiva

            'carico i dati nel datatable se il flag non è true
            Dim strParametri = Session("strParametri")
            Carica_DSImpegnativaColtivazioneConferimento(DSImpegnativaColtivazioneConferimento, vuota, strParametri)
            Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
            Session("strParametri") = Nothing

            Dim text_allegato = "Mod. PP/DG/03-1 Rev.2"
            CType(rptStampa.Section1.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = text_allegato


            'prelevo i dati dell'intestazione

            If DSImpegnativaColtivazioneConferimento.DT_ImpegnativaColtivazioneConferimento.Rows.Count = 0 Then
                rptStampa.Section2.SectionFormat.EnableSuppress = True
                rptStampa.Section2.SectionFormat.EnableSuppress = True
            End If

            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                rptStampa.Database.Tables("DT_ImpegnativaColtivazioneConferimento").SetDataSource(DSImpegnativaColtivazioneConferimento)
                rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                'rptStampa.OpenSubreport("Rpt_SpecieParticelle.rpt").SetDataSource(DSSpeciexParticelle)

            Catch ex As Exception
                Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try


            ' imposto il parametro del fronte-retro
            rptStampa.SetParameterValue("fronteRetro", fronteRetro)

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.ImpegnativaColtivazioneConferimento, "", "", objParametri_Server)
            objCatDoc = Nothing

            Dim Nome_Documento As String = "ImpegnativaColtivazioneConferimento"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa,
                                   enum_CategorieDocumenti.ImpegnativaColtivazioneConferimento,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                         enum_CategorieDocumenti.ImpegnativaColtivazioneConferimento,
                                                         "ImpegnativaColtivazioneConferimento",
                                                         Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                                         Sottocartella,
                                                         "", "", "", "",
                                                         CDate("01/01/" & anno),
                                                         CDate("31/12/" & anno),
                                                         objParametri_Server)


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + anno + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "ImpegnativaColtivazioneConferimento"
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_OP"
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
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


        End If



    End Sub

    '#########################################################################################################
    Private Sub Carica_DSImpegnativaColtivazioneConferimento(ByRef DSImpegnativaColtivazioneConferimento As DS_ImpegnativaColtivazioneConferimento, ByVal vuota As Boolean, ByVal Str_FiltroImpianti As String)

        Dim dsLoghi As New Ds_Loghi()
        Dim handleStampeOP As New Stampe_OP()
        Dim dtSuperficieSpecie As New DataTable()

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            ConnessioniTransazioni.ApriConnessione(False, objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            CaricaDs_Loghi(dsLoghi)


            If vuota Then
                Dim listaPive As New List(Of String)
                Dim piva_split As String() = Str_FiltroImpianti.Split(New String() {" OR "}, StringSplitOptions.RemoveEmptyEntries)

                For Each item As String In piva_split
                    Dim startPos As Integer = item.IndexOf("Imprese.PIVA =") + "Imprese.PIVA =".Length
                    Dim endPos As Integer = item.IndexOf(")", startPos)
                    Dim piva As String = item.Substring(startPos, endPos - startPos).Trim()
                    listaPive.Add(piva.Replace("'", ""))
                Next

                For Each Piva In listaPive
                    For i As Integer = 0 To 4
                        Dim newRow As DataRow = DSImpegnativaColtivazioneConferimento.DT_ImpegnativaColtivazioneConferimento.NewRow()
                        newRow.Item("piva") = Piva
                        newRow.Item("veg_cod") = i
                        newRow.Item("veg_des") = ""
                        newRow.Item("blob_logo") = dsLoghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

                        DSImpegnativaColtivazioneConferimento.DT_ImpegnativaColtivazioneConferimento.AddDT_ImpegnativaColtivazioneConferimentoRow(newRow)
                    Next
                Next
            Else
                dtSuperficieSpecie = handleStampeOP.SuperficieSpecieConsuntivoFiltroImpianti(objParametri_Server, Piva, Str_FiltroImpianti, 0, validita_inizio, validita_fine)

                For Each row In dtSuperficieSpecie.Rows()
                    Dim newRow As DataRow = DSImpegnativaColtivazioneConferimento.DT_ImpegnativaColtivazioneConferimento.NewRow()

                    newRow.Item("piva") = row.Item("PIVA")
                    newRow.Item("veg_cod") = dtSuperficieSpecie.Rows.IndexOf(row)
                    newRow.Item("veg_des") = row.Item("Veg_Des")
                    newRow.Item("sup_imp") = row.Item("Str_Sup_Specie")
                    newRow.Item("blob_logo") = dsLoghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

                    DSImpegnativaColtivazioneConferimento.DT_ImpegnativaColtivazioneConferimento.AddDT_ImpegnativaColtivazioneConferimentoRow(newRow)
                Next
            End If


        Catch ex As Exception
            Log_Errori &= "- lettura dati: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf & vbCrLf
        Finally
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End Try


    End Sub

    Sub CaricaDs_Loghi(ByVal DsLoghi As Ds_Loghi)

        'DsLoghi.LoghiDataTable.NewLoghiRow()
        'Dim r As Ds_Loghi.LoghiRow = DsLoghi.Loghi.NewLoghiRow()
        Dim r As DataRow = DsLoghi.Loghi.NewLoghiRow()
        Dim x(0) As Byte
        x(0) = 0
        Dim bmpFx As System.Drawing.Bitmap = New System.Drawing.Bitmap(1, 1)
        bmpFx.SetPixel(0, 0, System.Drawing.Color.White)
        Dim logox() As Byte
        Dim cx As New System.Drawing.ImageConverter
        logox = cx.ConvertTo(bmpFx, GetType(Byte()))
        r.Item("Blob_Logo") = logox


        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                If Not objWebConfig.Path_Directory_Loghi_Cliente.EndsWith("\") Then
                    objWebConfig.Path_Directory_Loghi_Cliente &= "\"
                End If
                Dim inserito As Boolean = False
                ' metto l'immagine1

                Try
                    Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard.bmp"
                    Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                    Dim logo() As Byte
                    Dim c As New System.Drawing.ImageConverter
                    logo = c.ConvertTo(bmpF, GetType(Byte()))
                    Dim i As Integer = 0
                    'For i = 0 To DsLoghi.Loghi.Rows.Count - 1
                    'DsLoghi.Loghi.Rows(i).Item("Blob_Logo") = logo
                    r.Item("Blob_Logo") = logo
                    inserito = True
                    ' Next
                Catch ex As Exception

                End Try

                'If inserito Then
                DsLoghi.Loghi.Rows.Add(r)
                DsLoghi.Loghi.AcceptChanges()
                'End If

            End If

        Catch ex As Exception

        End Try

    End Sub

End Class