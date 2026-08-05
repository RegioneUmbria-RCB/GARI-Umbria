Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MandatoTrasmissioneDati
    Inherits System.Web.UI.Page

    '----- Gestione Querystring
    Dim Piva As String
    Dim Anno As String
    Dim Rag_Soc As String

    Private rptStampa As Rpt_MandatoTrasmissioneDati

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_MandatoTrasmissioneDati

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Vet_Intestazione(25) As String
        Dim Log As String

        Dim strNomeCooperativa As String = ""

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

        Rag_Soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        If Not Me.IsPostBack Then

            'prelevo i dati dell'intestazione
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.DatiIntestazioneSocio(Piva, Rag_Soc, Vet_Intestazione, Log, objParametri_Server)

            CType(rptStampa.Section1.ReportObjects("TextRappresentanteLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(1)
            CType(rptStampa.Section1.ReportObjects("TextCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(24)
            CType(rptStampa.Section1.ReportObjects("TextCUAA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(25)
            CType(rptStampa.Section1.ReportObjects("TxtRagSocAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(10)

            If objParametri_Server.PivaSuperUser <> "00069880391" Then
                rptStampa.Section1.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = True
            Else
                rptStampa.Section1.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = False
            End If
        End If

        Dim CatCod As enum_CategorieDocumenti = enum_CategorieDocumenti.MandatoTrasmissioneTelematica
        Dim Nome_Documento As String = "MandatoTrasmissioneTelematica"


        ' leggo la sottocartella da CategorieDocumenti
        Dim Sottocartella As String
        Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
        Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri_Server)
        objCatDoc = Nothing

        ' salvo il report in formato PDF
        Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
        objGestFile.SalvaReportPdf(rptStampa,
                                   CatCod,
                                   Sottocartella,
                                   Nome_Documento + "_p" + Piva + "_d" + Anno + ".pdf",
                                   objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

        Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
        Dim AllegatiDocumentiCod As Integer


        AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                         CatCod,
                                                         Nome_Documento,
                                                         Nome_Documento + "_p" + Piva + "_d" + Anno + ".pdf",
                                                         Sottocartella,
                                                         "", "", "", "",
                                                         CDate("01/01/" & Anno),
                                                         CDate("31/12/" & Anno),
                                                         objParametri_Server)


        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim Path_Errore, Nome_File As String
        Dim Log_Errori As String = ""

        If Log_Errori <> "" Then

            Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + Log_Errori

            Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + Anno + CStr(Session("ASG_Utente_Username")) + ".txt"

            Dim objLog As New AgronicaCoreDataProvider.LogProvider
            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If objAgroWeb.PathDirectoryLOG <> "" Then
                objParametri_Server.LogDirectory = ""
                Path_Errore = objAgroWeb.PathDirectoryLOG & Nome_Documento
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



    End Sub

End Class