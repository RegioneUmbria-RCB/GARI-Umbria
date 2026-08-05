Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCoreDataProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreAnagrafeDAL
Imports System.IO

Public Class QuestionarioValutazioneAzienda_Aggiornamento
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_QuestionarioValutazioneAzienda_Aggiornamento

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

        rptStampa = New Rpt_QuestionarioValutazioneAzienda_Aggiornamento

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim Log As String
        Dim rag_soc As String

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

        'filtro impianti con AND
        'Str_FiltroImpianti = Session("strParametri").ToString()



        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            Dim DSLoghi As New Ds_Loghi
            Dim DSImprese As New Ds_Imprese_StampaMassiva

            Dim DSQuestionarioValutazioneAzienda_Aggiornamento As New DS_QuestionarioValutazioneAzienda_Aggiornamento

            'prelevo i dati dell'intestazione

            CaricaDs_QuestionarioValutazioneAzienda_Aggiornamento(DSQuestionarioValutazioneAzienda_Aggiornamento, DSLoghi)
            Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)


            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                rptStampa.Database.Tables("DT_QuestionarioValutazioneAzienda_Aggiornamento").SetDataSource(DSQuestionarioValutazioneAzienda_Aggiornamento)
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
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.QuestionarioValutazioneAzienda_Aggiornamento, "", "", objParametri_Server)
            objCatDoc = Nothing

            Dim Nome_Documento As String = "QuestionarioValutazioneAziendaAggiornamento"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa,
                                   enum_CategorieDocumenti.QuestionarioValutazioneAzienda_Aggiornamento,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                         enum_CategorieDocumenti.QuestionarioValutazioneAzienda_Aggiornamento,
                                                         "QuestionarioValutazioneAziendaAggiornamento",
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
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "QuestionarioValutazioneAziendaAggiornamento"
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

    Sub CaricaDs_QuestionarioValutazioneAzienda_Aggiornamento(ByVal DSQuestionarioValutazioneAzienda_Aggiornamento As DS_QuestionarioValutazioneAzienda_Aggiornamento, ByVal DS_Loghi As Ds_Loghi)

        CaricaDs_Loghi(DS_Loghi)

        Dim newRow As DataRow = DSQuestionarioValutazioneAzienda_Aggiornamento.DT_QuestionarioValutazioneAzienda_Aggiornamento.NewRow()

        newRow.Item("blob_logo") = DS_Loghi.Loghi.Rows(0).Field(Of Byte())("Blob_Logo")

        DSQuestionarioValutazioneAzienda_Aggiornamento.DT_QuestionarioValutazioneAzienda_Aggiornamento.AddDT_QuestionarioValutazioneAzienda_AggiornamentoRow(newRow)

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