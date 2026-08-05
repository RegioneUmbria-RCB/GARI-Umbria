Imports System.Xml
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class RisultatoAnalisiConformita
    Inherits System.Web.UI.Page

    Private DSControlliConformitaInterventi As DS_ControlloConformita_Interventi
    Private rptStampa As Rpt_RisultatoAnalisiConformita

    Private DSControlloConformitaMagazzini As DS_ControlloConformita_Magazzini
    Private DSControlloConformitaImpiantiIAF As DS_ControlloConformita_ImpiantiIAF
    Private DSControlloConformitaImpiantiAggiuntivi As DS_ControlloConformita_ImpiantiAggiuntivi

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Qs_Data_Da As String
    Private Qs_Data_A As String
    Private Qs_Veg_Cod As Integer
    Private Qs_Piva As String
    Private Qs_Sa_Cod As String

    Private Sub RisultatoAnalisiConformita_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        rptStampa = New Rpt_RisultatoAnalisiConformita
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Expires = 0
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim Veg_Des As String = ""
        Dim Rag_Soc As String = ""
        Dim Sa_Nome As String = ""

        If Not Me.IsPostBack Then
            Try
                Dim strJson As String
                Dim strJSONOperazioni As String
                Dim strJSONDettagli As String
                Dim strJSONOperazioniMagazzino As String
                Dim strJSONDettagliMagazzino As String
                Dim strJSONAppIAF As String
                Dim strJSONAppIAFDettagli As String
                Dim strJSONAppControlli As String
                Dim strJSONAppControlliDettagli As String

                ' TESTATA
                Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString, AgroKey_EncoderDecoder, Server)
                Integer.TryParse(Stringa_Decodifica(Request.QueryString("veg_cod").ToString, AgroKey_EncoderDecoder, Server), Qs_Veg_Cod)
                Qs_Data_Da = Stringa_Decodifica(Request.QueryString("data_da").ToString, AgroKey_EncoderDecoder, Server)
                Qs_Data_A = Stringa_Decodifica(Request.QueryString("data_a").ToString, AgroKey_EncoderDecoder, Server)
                Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString, AgroKey_EncoderDecoder, Server)

                If Qs_Veg_Cod <> 0 Then
                    Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    Veg_Des = objSpecie.VegDes_from_VegCod(Qs_Veg_Cod, objParametri_Server)
                Else
                    Veg_Des = Resources.AgronicaStampe_2010.TutteSpecie
                End If
                If Qs_Piva <> "" Then
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Rag_Soc = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                End If
                If Qs_Sa_Cod <> "0" And Qs_Sa_Cod <> "" Then
                    Dim ArraySaCod() As String = Split(Qs_Sa_Cod, ",")
                    If Not ArraySaCod Is Nothing Then
                        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        For c = 0 To ArraySaCod.Length - 1
                            Sa_Nome &= objCentri.SaNome_from_SaCod(Qs_Piva, ArraySaCod(c), objParametri_Server) & ","
                        Next
                        If Sa_Nome <> "" Then
                            Sa_Nome = Left(Sa_Nome, Sa_Nome.Length - 1)
                        End If
                    End If
                Else if Qs_Sa_Cod = "0" Then
                    Sa_Nome = Resources.AgronicaStampe_2010.TuttiCentri
                End If

                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
                CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sa_Nome
                CType(rptStampa.Section1.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Veg_Des
                CType(rptStampa.Section1.ReportObjects("TextPeriodo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_Data_Da & " - " & Qs_Data_A

                DSControlliConformitaInterventi = New DS_ControlloConformita_Interventi
                DSControlloConformitaMagazzini = New DS_ControlloConformita_Magazzini
                DSControlloConformitaImpiantiIAF = New DS_ControlloConformita_ImpiantiIAF
                DSControlloConformitaImpiantiAggiuntivi = New DS_ControlloConformita_ImpiantiAggiuntivi

                ' DATI
                If Not Session("ParametriAgronicaStampe_2010_Stringa_JSon") Is Nothing Then
                    strJson = Session("ParametriAgronicaStampe_2010_Stringa_JSon")
                    If strJson <> "" Then
                        strJSONOperazioni = Split(strJson, "||||||||||")(0)
                        strJSONDettagli = Split(strJson, "||||||||||")(1)
                        If Split(strJson, "||||||||||").Length = 8 Then
                            strJSONOperazioniMagazzino = Split(strJson, "||||||||||")(2)
                            strJSONDettagliMagazzino = Split(strJson, "||||||||||")(3)
                            strJSONAppIAF = Split(strJson, "||||||||||")(4)
                            strJSONAppIAFDettagli = Split(strJson, "||||||||||")(5)
                            strJSONAppControlli = Split(strJson, "||||||||||")(6)
                            strJSONAppControlliDettagli = Split(strJson, "||||||||||")(7)
                        End If

                        CaricaDsControlliInterventi(strJSONOperazioni, strJSONDettagli)

                        If strJSONAppControlli <> "" Then
                            CaricaDsControlliAppAggiuntivi(strJSONAppControlli, strJSONAppControlliDettagli)
                        End If

                        If strJSONOperazioniMagazzino <> "" Then
                            CaricaDsControlliMagazzini(strJSONOperazioniMagazzino, strJSONDettagliMagazzino)
                        End If

                        If strJSONAppIAF <> "" Then
                            CaricaDsControlliAppIAF(strJSONAppIAF, strJSONAppIAFDettagli)
                        End If
                    End If
                End If

                rptStampa.Section3.SectionFormat.EnableSuppress = True
                rptStampa.DetailSection3.SectionFormat.EnableSuppress = True
                rptStampa.DetailSection1.SectionFormat.EnableSuppress = True
                rptStampa.DetailSection2.SectionFormat.EnableSuppress = True

                If CType(DSControlliConformitaInterventi, DataSet).Tables(0).Rows.Count <> 0 Then
                    rptStampa.OpenSubreport("Rpt_ControlloConformita_Interventi_Verticale.rpt").SetDataSource(DSControlliConformitaInterventi)
                    rptStampa.Section3.SectionFormat.EnableSuppress = False
                End If

                If CType(DSControlloConformitaImpiantiAggiuntivi, DataSet).Tables(0).Rows.Count <> 0 Then
                    rptStampa.OpenSubreport("Rpt_ControlloConformita_ImpiantiAggiuntivi_Verticale.rpt").SetDataSource(DSControlloConformitaImpiantiAggiuntivi)
                    rptStampa.DetailSection3.SectionFormat.EnableSuppress = False
                End If

                If CType(DSControlloConformitaMagazzini, DataSet).Tables(0).Rows.Count <> 0 Then
                    rptStampa.OpenSubreport("Rpt_ControlloConformita_Magazzini_Verticale.rpt").SetDataSource(DSControlloConformitaMagazzini)
                    rptStampa.DetailSection1.SectionFormat.EnableSuppress = False
                End If

                If CType(DSControlloConformitaImpiantiIAF, DataSet).Tables(0).Rows.Count <> 0 Then
                    rptStampa.OpenSubreport("Rpt_ControlloConformita_ImpiantiIAF_Verticale.rpt").SetDataSource(DSControlloConformitaImpiantiIAF)
                    rptStampa.DetailSection2.SectionFormat.EnableSuppress = False
                End If

                Session("Report") = rptStampa
                'Dim RPT As ReportDocument = Session("Report")
                Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
                'RPT.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, False, "RisultatoAnalisiConformita")
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub CaricaDsControlliInterventi(ByVal strJSONOperazioni As String, ByVal strJSONDettagli As String)

        Dim RigaDs As DS_ControlloConformita_Interventi.DT_ControlloConformita_InterventiRow

        Dim OperazioniArray As JArray
        Dim DettagliArray As JArray

        If strJSONOperazioni <> "" Then
            OperazioniArray = JArray.Parse(strJSONOperazioni)
        End If
        If strJSONDettagli <> "" Then
            DettagliArray = JArray.Parse(strJSONDettagli)
        End If


        If Not OperazioniArray Is Nothing Then

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(OperazioniArray)

            For Each r In OperazioniArray

                If Not String.IsNullOrEmpty(Trim(r("id_agenda"))) AndAlso IsNumeric(Trim(r("id_agenda"))) Then

                    Dim Dati = (
                        From d In DettagliArray
                        Where CInt(d("id_agenda")) = CInt(r("id_agenda"))
                        )

                    For Each dett In Dati

                        RigaDs = DSControlliConformitaInterventi.DT_ControlloConformita_Interventi.NewDT_ControlloConformita_InterventiRow

                        RigaDs.ID_Agenda = CInt(Trim(r("id_agenda")))
                        RigaDs.Lav_Cod = CInt(Trim(r("lav_cod")))
                        RigaDs.Lav_Des = Trim(r("des_lib"))
                        If Not r("sa_nome") Is Nothing Then
                            RigaDs.Des_Lib = Trim(r("data_movimento")) & " - Disciplinare: " & Trim(r("disciplinare_des")) & " - " & Trim(r("des_lib")) & " - " & Trim(r("sa_nome")) & " - App.: " & Trim(r("appezzamenti")) & " - Sup.:" & Trim(r("superficie")) & " Ha"
                        Else
                            RigaDs.Des_Lib = Trim(r("data_movimento")) & " - Disciplinare: " & Trim(r("disciplinare_des")) & " - " & Trim(r("des_lib")) & " - App.: " & Trim(r("appezzamenti")) & " - Sup.:" & Trim(r("superficie")) & " Ha"
                        End If
                        RigaDs.Data = Trim(r("data_movimento"))
                        RigaDs.Data_Movimento = CDate(Trim(r("data_movimento")))

                        RigaDs.App_Conformi = ""
                        RigaDs.App_NON_Conformi = ""

                        RigaDs.ID_Controllo = dett("err_code")
                        RigaDs.Controllo_Des = dett("err_des")
                        RigaDs.Dettaglio_NON_Conformita = dett("err_nota")

                        Select Case LCase(Trim(dett("BooleanRisVer")))
                            Case "true"
                                RigaDs.Conforme_SI = "X"
                                RigaDs.Conforme_NO = ""
                            Case "false"
                                RigaDs.Conforme_SI = ""
                                RigaDs.Conforme_NO = "X"
                        End Select

                        DSControlliConformitaInterventi.DT_ControlloConformita_Interventi.Rows.Add(RigaDs)

                    Next

                End If

            Next

        End If

    End Sub

    Private Sub CaricaDsControlliMagazzini(ByVal strJSONOperazioniMagazzino As String, ByVal strJSONDettagliMagazzino As String)

        Dim RigaDs As DS_ControlloConformita_Magazzini.DT_ControlloConformita_MagazziniRow

        Dim OperazioniArray As JArray
        Dim DettagliArray As JArray

        If strJSONOperazioniMagazzino <> "" Then
            OperazioniArray = JArray.Parse(strJSONOperazioniMagazzino)
        End If
        If strJSONDettagliMagazzino <> "" Then
            DettagliArray = JArray.Parse(strJSONDettagliMagazzino)
        End If

        If Not OperazioniArray Is Nothing Then

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(OperazioniArray)

            For Each r In OperazioniArray

                If Not String.IsNullOrEmpty(Trim(r("id_agenda"))) AndAlso IsNumeric(Trim(r("id_agenda"))) Then

                    Dim Dati = (
                        From d In DettagliArray
                        Where CInt(d("id_agenda")) = CInt(r("id_agenda"))
                        )

                    For Each dett In Dati

                        RigaDs = DSControlloConformitaMagazzini.DT_ControlloConformita_Magazzini.NewDT_ControlloConformita_MagazziniRow

                        RigaDs.ID_Agenda = CInt(Trim(r("id_agenda")))
                        RigaDs.Lav_Cod = CInt(Trim(r("lav_cod")))
                        RigaDs.Lav_Des = Trim(r("des_lib"))
                        RigaDs.Des_Lib = CDate(Trim(r("data_movimento"))).ToShortDateString & " - " & Trim(r("des_lib")) & " - " & Trim(r("fabbricato_des"))
                        RigaDs.Data = Trim(r("data_movimento"))
                        RigaDs.Data_Movimento = CDate(Trim(r("data_movimento")))

                        RigaDs.Prodotto = dett("prodotto")
                        RigaDs.Qta = dett("qta")
                        RigaDs.Qta_Presente = dett("qta_presente")
                        RigaDs.UdmSim = dett("udm_sim")

                        RigaDs.Dettaglio_NON_Conformita = ""

                        Select Case LCase(Trim(dett("BooleanRisVer")))
                            Case "true"
                                RigaDs.Conforme_SI = "X"
                                RigaDs.Conforme_NO = ""
                            Case "false"
                                RigaDs.Conforme_SI = ""
                                RigaDs.Conforme_NO = "X"
                        End Select

                        DSControlloConformitaMagazzini.DT_ControlloConformita_Magazzini.Rows.Add(RigaDs)

                    Next

                End If

            Next

        End If

    End Sub

    Private Sub CaricaDsControlliAppIAF(ByVal strJSONAppIAF As String, ByVal strJSONAppIAFDettagli As String)

        Dim RigaDs As DS_ControlloConformita_ImpiantiIAF.DT_ControlloConformita_ImpiantiIAFRow

        Dim OperazioniArray As JArray
        Dim DettagliArray As JArray

        If strJSONAppIAF <> "" Then
            OperazioniArray = JArray.Parse(strJSONAppIAF)
        End If
        If strJSONAppIAFDettagli <> "" Then
            DettagliArray = JArray.Parse(strJSONAppIAFDettagli)
        End If

        If Not OperazioniArray Is Nothing Then

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(OperazioniArray)

            For Each r In OperazioniArray

                If Not String.IsNullOrEmpty(Trim(r("chiave"))) Then

                    Dim Dati = (
                        From d In DettagliArray
                        Where CStr(d("chiave")) = CStr(r("chiave"))
                        )

                    For Each dett In Dati

                        RigaDs = DSControlloConformitaImpiantiIAF.DT_ControlloConformita_ImpiantiIAF.NewDT_ControlloConformita_ImpiantiIAFRow

                        RigaDs.chiave = Trim(r("chiave"))
                        RigaDs.App_Nome = Trim(r("sa_nome")) & " - " & Trim(r("app_nome"))
                        RigaDs.iaf_des = dett("iaf_des")

                        Select Case LCase(Trim(dett("BooleanRisVer")))
                            Case "true"
                                RigaDs.Conforme_SI = "X"
                                RigaDs.Conforme_NO = ""
                            Case "false"
                                RigaDs.Conforme_SI = ""
                                RigaDs.Conforme_NO = "X"
                        End Select

                        DSControlloConformitaImpiantiIAF.DT_ControlloConformita_ImpiantiIAF.Rows.Add(RigaDs)

                    Next

                End If

            Next

        End If

    End Sub

    Private Sub CaricaDsControlliAppAggiuntivi(ByVal strJSONAppControlli As String, ByVal strJSONAppControlliDettagli As String)

        Dim RigaDs As DS_ControlloConformita_ImpiantiAggiuntivi.DT_ControlloConformita_ImpiantiAggiuntiviRow

        Dim OperazioniArray As JArray
        Dim DettagliArray As JArray

        If strJSONAppControlli <> "" Then
            OperazioniArray = JArray.Parse(strJSONAppControlli)
        End If
        If strJSONAppControlliDettagli <> "" Then
            DettagliArray = JArray.Parse(strJSONAppControlliDettagli)
        End If

        If Not OperazioniArray Is Nothing Then

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(OperazioniArray)

            For Each r In OperazioniArray

                If Not String.IsNullOrEmpty(Trim(r("chiave"))) Then

                    Dim Dati = (
                        From d In DettagliArray
                        Where CStr(d("chiave")) = CStr(r("chiave"))
                        )

                    For Each dett In Dati

                        RigaDs = DSControlloConformitaImpiantiAggiuntivi.DT_ControlloConformita_ImpiantiAggiuntivi.NewDT_ControlloConformita_ImpiantiAggiuntiviRow

                        RigaDs.chiave = Trim(r("chiave"))
                        RigaDs.App_Nome = Trim(r("sa_nome")) & " - " & Trim(r("app_nome"))
                        RigaDs.iaf_des = dett("iaf_des")

                        Select Case LCase(Trim(dett("BooleanRisVer")))
                            Case "true"
                                RigaDs.Conforme_SI = "X"
                                RigaDs.Conforme_NO = ""
                            Case "false"
                                RigaDs.Conforme_SI = ""
                                RigaDs.Conforme_NO = "X"
                        End Select

                        DSControlloConformitaImpiantiAggiuntivi.DT_ControlloConformita_ImpiantiAggiuntivi.Rows.Add(RigaDs)

                    Next

                End If

            Next

        End If

    End Sub

End Class