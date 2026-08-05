Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class SchedaAutoCertificazione
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_SchedaAutoCertificazione
    Private DSAttoNotorio As DS_AttoNotorio

    Dim Piva As String
    Dim anno, validita_inizio, validita_fine As String
    Dim Str_FiltroImpianti As String
    Dim log_errori As String = ""

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub SchedaAutoCertificazione_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        rptStampa = New Rpt_SchedaAutoCertificazione
    End Sub

 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim Vet_Intestazione(25) As String
        Dim Log As String
        Dim rag_soc As String

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        rag_soc = Stringa_Decodifica(Request.QueryString("r").ToString, _
                         AgroKey_EncoderDecoder, _
                         Server)

        anno = Stringa_Decodifica(Request.QueryString("a").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        If Not IsNothing(Request.QueryString("vi")) Then
            validita_inizio = Stringa_Decodifica(Request.QueryString("vi").ToString, _
                      AgroKey_EncoderDecoder, Server)
        Else
            validita_inizio = "01/01/" + CStr(anno)
        End If

        If Not IsNothing(Request.QueryString("vf")) Then
            validita_fine = Stringa_Decodifica(Request.QueryString("vf").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)
        Else
            validita_fine = "31/12/" + CStr(anno)
        End If


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            Dim DSAttoNotorio As New DS_AttoNotorio

            Carica_DS_SchedaAutoCertificazione(DSAttoNotorio)

            PulisciDoppi(DSAttoNotorio)

            Try
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Piva, objParametri_Server)

                'prelevo i dati dell'intestazione
                Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
                objStampe.DatiIntestazioneSocio(Piva, rag_soc, Vet_Intestazione, Log, objParametri_Server)

                'CType(rptStampa.Section1.ReportObjects("TextSuperUser"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(23).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = anno

                CType(rptStampa.Section1.ReportObjects("TextRapprLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(1).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextComNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(2).ToUpper

                If Vet_Intestazione(3).ToUpper <> "0" Then
                    CType(rptStampa.Section1.ReportObjects("TextProvNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(3).ToUpper
                End If

                CType(rptStampa.Section1.ReportObjects("TextDataNascita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(4).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextIndResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(5).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextComResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(6).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextProvResidenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(7).ToUpper
                CType(rptStampa.Section1.ReportObjects("TextQualifica"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(8).ToUpper

                CType(rptStampa.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(10)
                CType(rptStampa.Section1.ReportObjects("TextIndImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(11)
                CType(rptStampa.Section1.ReportObjects("TextComImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(12)
                CType(rptStampa.Section1.ReportObjects("TextProvImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(13)
                CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale

                CType(rptStampa.Section1.ReportObjects("TextCUAA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(25)

                CType(rptStampa.Section1.ReportObjects("TextCooperativa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(15)
                CType(rptStampa.Section1.ReportObjects("TextIndCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(16)
                CType(rptStampa.Section1.ReportObjects("TextCapCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(17)
                CType(rptStampa.Section1.ReportObjects("TextComCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(18)
                CType(rptStampa.Section1.ReportObjects("TextProvCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(19)
                CType(rptStampa.Section1.ReportObjects("TextNumIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(20)
                CType(rptStampa.Section1.ReportObjects("TextDataIscr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Vet_Intestazione(21)



            Catch ex As Exception
                log_errori += "- lettura e gestione dell'intestazione: " + vbCrLf + ex.Message + vbCrLf
            End Try


            Try

                '--------------------------------------------
                ' AGGANCIO DATI
                '--------------------------------------------
                rptStampa.SetDataSource(DSAttoNotorio)

            Catch ex As Exception
                log_errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try


            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.AttoNotorio, "", "", objParametri_Server)
            objCatDoc = Nothing

            Dim Nome_Documento As String = "AttoNotorio"

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa, _
                                       enum_CategorieDocumenti.AttoNotorio, _
                                       Sottocartella, _
                                       Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf", _
                                       objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer


            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                                                             enum_CategorieDocumenti.AttoNotorio, _
                                                             "Atto Notorio", _
                                                             Nome_Documento + "_p" + Piva + "_d" + anno + ".pdf", _
                                                             Sottocartella, _
                                                             "", "", "", "", _
                                                             CDate("01/01/" & anno), _
                                                             CDate("31/12/" & anno), _
                                                             objParametri_Server)


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If log_errori <> "" Then

                log_errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + log_errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + anno + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & "AttoNotorio"
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_OP"
                End If

                Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = Path_Errore,
                    .LogFileName = Nome_File
                }

                'objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Qs_Piva.Page_Load", log_errori)
                objLog.Scrivi_LOG(objParametri_Server, "Qs_Piva.Page_Load", log_errori, CustomLOGParams:=CustomLOGParams)

            End If
            '-----------------------------------------

            Session("Report") = rptStampa
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))


        End If



    End Sub


    '#########################################################################################################
    Private Sub PulisciDoppi(ByRef DSAttoNotorio As DS_AttoNotorio)
        Dim i As Integer
        Dim precedente As String = ""
        For i = 0 To DSAttoNotorio.DT_AttoNotorio.Rows.Count - 1
            Dim app As String
            app = DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("piva") & _
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sa_cod") & _
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("appezza") & _
                        "_" & DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("id_reg")
            If app <> precedente Then
                precedente = app
            Else
                DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("copia") = "1"
            End If
        Next
    End Sub


    '#########################################################################################################
    Private Sub Carica_DS_SchedaAutoCertificazione(ByRef DSAttoNotorio As DS_AttoNotorio)

        Dim i As Integer
        Dim DT As DataTable

        Try

            'filtro impianti con AND
            Str_FiltroImpianti = Session("strParametri").ToString()

            Dim objCatasto As New AgronicaCoreStampeDAL.Catasto

            DT = objCatasto.AttoNotorio_Semplificato(Piva, _
                                                  validita_inizio, _
                                                  validita_fine, _
                                                  Str_FiltroImpianti,
                                                  objParametri_Server)



        Catch ex As Exception
            log_errori &= "query AttoNotorio_Semplificato: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim DR As DS_AttoNotorio.DT_AttoNotorioRow

                For i = 0 To DT.Rows.Count - 1

                    DR = DSAttoNotorio.DT_AttoNotorio.NewRow

                    DR.piva = DT.Rows(i).Item("piva")
                    DR.sa_cod = DT.Rows(i).Item("sa_cod")
                    DR.appezza = DT.Rows(i).Item("appezza")
                    DR.id_reg = DT.Rows(i).Item("id_reg")
                    DR.campo_cod = DT.Rows(i).Item("campo_cod")

                    DR.sa_nome = DT.Rows(i).Item("sa_nome")

                    If DT.Rows(i).Item("frz_des") <> "" Then
                        DR.ind_des = DT.Rows(i).Item("ind_des") & " " & DT.Rows(i).Item("frz_des") & " " & DT.Rows(i).Item("cap") & _
                        " - " & DT.Rows(i).Item("localita") & "(" & DT.Rows(i).Item("comuni_prov") & ")" & _
                         "[" & DT.Rows(i).Item("pro_cod_istat") & "-" & DT.Rows(i).Item("com_cod_istat") & "]"
                    Else
                        DR.ind_des = DT.Rows(i).Item("ind_des") & " " & DT.Rows(i).Item("cap") & _
                        " - " & DT.Rows(i).Item("localita") & "(" & DT.Rows(i).Item("comuni_prov") & ")" & _
                         "[" & DT.Rows(i).Item("pro_cod_istat") & "-" & DT.Rows(i).Item("com_cod_istat") & "]"
                    End If

                    DR.veg_cod = DT.Rows(i).Item("veg_cod")
                    DR.veg_des = DT.Rows(i).Item("veg_des")
                    DR.cul_cod = DT.Rows(i).Item("cul_cod")
                    If DT.Rows(i).Item("cul_cod") = 0 Then
                        DR.cul_des = DT.Rows(i).Item("dest_uso")
                    Else
                        DR.cul_des = DT.Rows(i).Item("cul_des")
                    End If
                    If DT.Rows(i).Item("grva_des") <> "" Then
                        DR.cul_des &= " - " & DT.Rows(i).Item("grva_des")
                    End If
                    DR.sup_imp = DT.Rows(i).Item("sup_imp")
                    DR.Inizio_Impianto = DT.Rows(i).Item("Inizio_Impianto")

                    DR.Prov = DT.Rows(i).Item("Prov")
                    DR.Com = DT.Rows(i).Item("Com")
                    DR.Sezione = DT.Rows(i).Item("Sezione")
                    DR.Foglio = DT.Rows(i).Item("Foglio")
                    DR.Numero = DT.Rows(i).Item("Numero")
                    DR.Subalterno = DT.Rows(i).Item("Subalterno")
                    DR.Area = DT.Rows(i).Item("area")

                    DR.Sup_Cat = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(DT.Rows(i).Item("ETTARI_Sup_Cat"), DT.Rows(i).Item("ARE_Sup_Cat"), DT.Rows(i).Item("CENTIARE_Sup_Cat"))

                    DSAttoNotorio.DT_AttoNotorio.Rows.Add(DR)

                    '(12/12/14) fede visualizzo sempre tutto su indicazione di Valerio
                    ''===================================================================================================
                    ''se l'impianto è lo stesso elimino i dati nelle righe successive
                    ''per farlo metto cul_des=-1 ed elimino dopo le stringhe
                    'For i = 1 To DSAttoNotorio.DT_AttoNotorio.Rows.Count - 1

                    '    If DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("piva") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("piva") And _
                    '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sa_cod") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("sa_cod") And _
                    '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("appezza") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("appezza") And _
                    '       DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("id_reg") = DSAttoNotorio.DT_AttoNotorio.Rows(i - 1).Item("id_reg") Then

                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("veg_des") = ""
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("cul_des") = ""

                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("su_fila") = ""
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("tra_fila") = ""
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("formaallevamento") = ""

                    '        'metto poi le formule nel report per omettere i campi sup_imp, @numpiante, @anno
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("p_ha") = 0
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("sup_imp") = -1
                    '        DSAttoNotorio.DT_AttoNotorio.Rows(i).Item("inizio_impianto") = #1/1/1900#

                    '    End If

                    'Next

                Next

            End If

            If DSAttoNotorio.DT_AttoNotorio.Rows.Count = 0 Then
                rptStampa.Section3.SectionFormat.EnableSuppress = True
            End If

        Catch ex As Exception
            log_errori &= "elaborazione DT: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


    End Sub



End Class