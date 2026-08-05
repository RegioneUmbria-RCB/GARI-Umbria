Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SchedaCatastoUtilizzi
    Inherits System.Web.UI.Page

    Private rptSchedaCatastoUtilizzi As Rpt_SchedaCatastoUtilizzi
    Private Log_Errori As String

    Dim Piva As String
    Dim validita_inizio As String
    Dim validita_fine As String

    Dim Param_Filtri As String = ""
    Dim Param_Rag_Soc As String = ""
    Dim Param_Piva_CUAA As String = ""
    Dim Param_Indirizzo As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region ""

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub RegistroCorrispettiviNew_AbortTransaction(sender As Object, e As System.EventArgs) Handles Me.AbortTransaction

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        rptSchedaCatastoUtilizzi = New Rpt_SchedaCatastoUtilizzi

    End Sub

#End Region

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        validita_inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), _
                           AgroKey_EncoderDecoder, _
                           Server)

        validita_fine = Stringa_Decodifica(CStr(Request.QueryString("df")), _
                         AgroKey_EncoderDecoder, _
                         Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "SchedaCatastoUtilizzi"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_SchedaCatastoUtilizzi()

            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try

                'Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                'Data_Inizio_Allegati = Data_Inizio
                'Data_Fine_Allegati = Data_Fine
                'IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                '' leggo la sottocartella da CategorieDocumenti
                'Dim Sottocartella As String
                'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                'Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, "", "", objParametri_Server)

                '' salvo il report in formato PDF
                'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                'objGestFile.SalvaReportPdf(rptSchedaCatastoUtilizzi, _
                '                           enum_CategorieDocumenti., _
                '                           Sottocartella, _
                '                           Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                '                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                'Dim AllegatiDocumentiCod As Integer

                'AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                '                                                 enum_CategorieDocumenti.RegistroCorrispettivi_Vendita, _
                '                                                 Nome_Documento, _
                '                                                 Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                '                                                 Sottocartella, _
                '                                                 "", "", "", "", _
                '                                                 Data_Inizio_Allegati, _
                '                                                 Data_Fine_Allegati, _
                '                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptSchedaCatastoUtilizzi.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Dispose dei dataset e del report per evitare problema deallocazione.
            'DSRegCorr.Dispose()
            'DSRegCorr = Nothing

            'rptRegCorrispettivi.Close()
            'rptRegCorrispettivi.Dispose()
            'rptRegCorrispettivi = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Piva) +
                               vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Catasto",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaCatastoUtilizzi",
                                                 Log_Errori)

            End If

            '-----------------------------------------

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))


        End If
    End Sub

    '#####################################################################################################
    Private Sub Stampa_SchedaCatastoUtilizzi()

        Dim DS_Catasto As New DS_SchedaCatastoUtilizzi
        Dim DR_Catasto As DS_SchedaCatastoUtilizzi.DT_SchedaCatastoUtilizziRow
        Dim DT As DataTable

        Try

            Dim strXmlVariabili As String = Session("strXmlVariabilistampe")

            Dim XmlDoc As New System.Xml.XmlDocument
            ' Dim XML_FiltroStampa As System.Xml.XmlElement
            Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
            Dim XML_VariabiliStampe As System.Xml.XmlElement

            Dim sa_cod, appezza, id_reg As Integer
            Dim ChiaveImpianto, ElencoChiaviImpianto As String
            ElencoChiaviImpianto = ""

            'filtro impianti con AND
            Dim Str_FiltroImpianti As String = ""

            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabili)

            'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
            'If XML_FiltroStampa Is Nothing Then
            '    XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
            'End If

            '' XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")
            XMLs_VariabiliStampe = XmlDoc.SelectNodes("//VariabiliStampe")

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                If (IsNothing(XML_VariabiliStampe.GetAttribute("p"))) Then
                    Log_Errori = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("p")) = "") Then
                    Log_Errori = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("s"))) Then
                    Log_Errori = "IL SA_COD E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("s")) = "") Then
                    Log_Errori = "IL SA_COD E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("a"))) Then
                    Log_Errori = "APPEZZA E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("a")) = "") Then
                    Log_Errori = "APPEZZA E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("r"))) Then
                    Log_Errori = "ID_REG E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("r")) = "") Then
                    Log_Errori = "ID_REG E' NULLO!!!" & vbCrLf
                End If

                If Log_Errori <> "" Then
                    Exit Sub
                End If

                'ricavo gli impianti
                Piva = XML_VariabiliStampe.GetAttribute("p")
                sa_cod = XML_VariabiliStampe.GetAttribute("s")
                appezza = XML_VariabiliStampe.GetAttribute("a")
                id_reg = XML_VariabiliStampe.GetAttribute("r")

                'Genero la chiave impianto
                ChiaveImpianto = Piva & "_" & sa_cod & "_" & appezza & "_" & id_reg

                ElencoChiaviImpianto += ",'" & ChiaveImpianto & "'"

                '-------------------------------------------------

                'Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                'Query3_TempTableFill += " VALUES     (" + Agro_SQL_SaveText_NULL(Piva) + "," + Agro_SQL_SaveNum(sa_cod) + "," + Agro_SQL_SaveNum(appezza) + "," + Agro_SQL_SaveNum(id_reg) + ")  " & vbCrLf


            Next

            'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
            ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)

            Str_FiltroImpianti = " AND               ((Reg_Impianti.Piva  " & vbCrLf & _
             "                    +  '_' + CONVERT(varchar(10), Reg_Impianti.Sa_Cod)  " & vbCrLf & _
             "                    + '_' + CONVERT(varchar(10), Reg_Impianti.Appezza)  " & vbCrLf & _
            "                     + '_' + CONVERT(varchar(10), Reg_Impianti.Id_Reg))  " & vbCrLf & _
             "                     IN (" & ElencoChiaviImpianto & "))  " & vbCrLf


            Dim objCatasto As New AgronicaCoreStampeDAL.Catasto

            DT = objCatasto.SchedaCatastoUtilizzi(Piva, _
                                                  validita_inizio, _
                                                  validita_fine, _
                                                  Str_FiltroImpianti,
                                                  objParametri_Server)



        Catch ex As Exception
            Log_Errori &= "query SchedaCatastoUtilizzi: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim dati_distinta As String
                Dim p_ha, resa As Decimal
                Dim reg_cod As Integer
                Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
                Dim chiave_impianto As String
                Dim TMP_chiave_impianto As String = ""

                For i = 0 To DT.Rows.Count - 1

                    DR_Catasto = DS_Catasto.DT_SchedaCatastoUtilizzi.NewRow

                    DR_Catasto.piva = DT.Rows(i).Item("piva")
                    DR_Catasto.PivaReale = DT.Rows(i).Item("PivaReale")
                    DR_Catasto.sa_cod = DT.Rows(i).Item("sa_cod")
                    DR_Catasto.campo_cod = DT.Rows(i).Item("campo_cod")
                    DR_Catasto.appezza = DT.Rows(i).Item("appezza")
                    DR_Catasto.id_reg = DT.Rows(i).Item("id_reg")

                    DR_Catasto.sa_nome = DT.Rows(i).Item("sa_nome")
                    DR_Catasto.ind_des = DT.Rows(i).Item("ind_des")
                    If DT.Rows(i).Item("frz_des") <> "" Then
                        DR_Catasto.ind_des = DT.Rows(i).Item("ind_des") & " " & DT.Rows(i).Item("frz_des") & " " & DT.Rows(i).Item("cap") & _
                        " - " & DT.Rows(i).Item("localita") & "(" & DT.Rows(i).Item("comuni_prov") & ")" & _
                         "[" & DT.Rows(i).Item("pro_cod_istat") & "-" & DT.Rows(i).Item("com_cod_istat") & "]"
                    Else
                        DR_Catasto.ind_des = DT.Rows(i).Item("ind_des") & " " & DT.Rows(i).Item("cap") & _
                        " - " & DT.Rows(i).Item("localita") & "(" & DT.Rows(i).Item("comuni_prov") & ")" & _
                         "[" & DT.Rows(i).Item("pro_cod_istat") & "-" & DT.Rows(i).Item("com_cod_istat") & "]"
                    End If

                    DR_Catasto.frz_des = ""
                    DR_Catasto.cap = ""
                    DR_Catasto.localita = ""
                    DR_Catasto.comuni_prov = ""

                    DR_Catasto.TitoloPossessocentroDesc = TitoloPossessoDes_from_TitoloPossessoCod(DT.Rows(i).Item("TitoloPossessoCentro"))

                    DR_Catasto.campo_nome = DT.Rows(i).Item("campo_nome")
                    DR_Catasto.app_nome = DT.Rows(i).Item("app_nome")
                    DR_Catasto.TitoloPossessoDesc = ""
                    DR_Catasto.Anno_impianto = CDate(DT.Rows(i).Item("Inizio_Impianto")).Year
                    DR_Catasto.su_fila = DT.Rows(i).Item("su_fila")
                    DR_Catasto.tra_fila = DT.Rows(i).Item("tra_fila")
                    DR_Catasto.port_des = DT.Rows(i).Item("port_des")
                    DR_Catasto.foral_des = DT.Rows(i).Item("foral_des")
                    DR_Catasto.imp_des = DT.Rows(i).Item("imp_des")
                    DR_Catasto.cop_des = DT.Rows(i).Item("cop_des")
                    DR_Catasto.grfi_des = DT.Rows(i).Item("grfi_des")

                    dati_distinta = DT.Rows(i).Item("dati_distinta")
                    If dati_distinta.Contains("§") Then
                        p_ha = dati_distinta.Split("§")(0)
                        resa = dati_distinta.Split("§")(1)
                        reg_cod = dati_distinta.Split("§")(2)
                    Else
                        p_ha = 0
                        resa = 0
                        reg_cod = 0
                    End If
                    DR_Catasto.p_ha = p_ha
                    DR_Catasto.Resa_Ha = resa
                    DR_Catasto.resa_tot = Math.Round(resa * CDec(DT.Rows(i).Item("Sup_Imp")), 0)

                    DR_Catasto.veg_cod = DT.Rows(i).Item("veg_cod")
                    DR_Catasto.veg_des = DT.Rows(i).Item("veg_des")
                    DR_Catasto.cul_des = DT.Rows(i).Item("cul_des")
                    DR_Catasto.cul_cod = DT.Rows(i).Item("cul_cod")
                    If DT.Rows(i).Item("cul_cod") = 0 Then
                        DR_Catasto.Coltura = DT.Rows(i).Item("dest_uso")
                        DR_Catasto.veg_des = DT.Rows(i).Item("dest_uso")
                        DR_Catasto.cul_des = ""
                    Else
                        DR_Catasto.Coltura = DT.Rows(i).Item("veg_des") & " " & DT.Rows(i).Item("cul_des")
                    End If
                    If DT.Rows(i).Item("grva_des") <> "" Then
                        DR_Catasto.Coltura &= " - " & DT.Rows(i).Item("grva_des")
                    End If

                    Select Case reg_cod
                        Case 0, 1
                            DR_Catasto.regolamento = "Conv."
                        Case 4
                            DR_Catasto.regolamento = "BIO"
                        Case Else
                            DR_Catasto.regolamento = objReg.RegDes_from_RegCod(reg_cod, objParametri_Server)
                    End Select

                    chiave_impianto = DR_Catasto.piva & "|" & DR_Catasto.sa_cod & "|" & DR_Catasto.campo_cod & "|" & DR_Catasto.appezza & "|" & DR_Catasto.id_reg

                    If TMP_chiave_impianto <> chiave_impianto Then
                        'nuovo impianto
                        TMP_chiave_impianto = chiave_impianto

                        DR_Catasto.sup_imp = DT.Rows(i).Item("sup_imp")
                        DR_Catasto.piante_tot = Math.Round(p_ha * CDec(DT.Rows(i).Item("Sup_Imp")), 0)
                    Else
                        'stesso impianto
                        DR_Catasto.sup_imp = 0
                        DR_Catasto.piante_tot = 0
                    End If

                    DR_Catasto.Sup_Imp_Str = Format(DT.Rows(i).Item("sup_imp"), "#,###,##0.0000")
                    DR_Catasto.Num_Piante_Tot_Str = Format(Math.Round(p_ha * CDec(DT.Rows(i).Item("Sup_Imp")), 0), "#,###,##0")

                    DR_Catasto.Area = DT.Rows(i).Item("area")
                    DR_Catasto.Prov = DT.Rows(i).Item("prov")
                    DR_Catasto.Com = DT.Rows(i).Item("com")
                    If DT.Rows(i).Item("sezione") = "0" Then
                        DT.Rows(i).Item("sezione") = ""
                    End If
                    DR_Catasto.Sezione = DT.Rows(i).Item("sezione")
                    DR_Catasto.Foglio = DT.Rows(i).Item("foglio")
                    DR_Catasto.Numero = DT.Rows(i).Item("numero")
                    DR_Catasto.Subalterno = DT.Rows(i).Item("subalterno")
                    If DT.Rows(i).Item("subalterno") = "0" Then
                        DR_Catasto.Subalterno = ""
                    End If
                    DR_Catasto.Sup_Cat = Ettari_from_EttariAreCentiare(DT.Rows(i).Item("ETTARI_Sup_Cat"), DT.Rows(i).Item("ARE_Sup_Cat"), DT.Rows(i).Item("CENTIARE_Sup_Cat"))
        

                    'DR_Catasto. = DT.Rows(i).Item("")

                    DS_Catasto.DT_SchedaCatastoUtilizzi.Rows.Add(DR_Catasto)

                Next

            End If

        Catch ex As Exception
            Log_Errori += "- elaborazione dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATI
            '--------------------------------------------
            DS_Catasto.DT_SchedaCatastoUtilizzi.Columns("Piva").ColumnName = "Piva_old"
            DS_Catasto.DT_SchedaCatastoUtilizzi.Columns("PivaReale").ColumnName = "Piva"
            DS_Catasto.DT_SchedaCatastoUtilizzi.Columns("Piva_old").ColumnName = "PivaReale"

            rptSchedaCatastoUtilizzi.SetDataSource(DS_Catasto)

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################

        Try

            '/************* PARAMETRI X INTESTAZIONE DOCUMENTI ***************************/
            Dim ObjQDC As New AgronicaCoreStampeDAL.Stampe_QDC
            ObjQDC.Prepara_Parametri_Intestazione_ReportQDC(Piva, _
                                                            AGRODATAINIZIO, _
                                                            AGRODATAFINE, _
                                                            Param_Rag_Soc, _
                                                            Param_Piva_CUAA, _
                                                            Param_Indirizzo, _
                                                            Nothing, _
                                                            objParametri_Server)

            rptSchedaCatastoUtilizzi.SetParameterValue("Filtri", Param_Filtri)
            rptSchedaCatastoUtilizzi.SetParameterValue("Rag_Soc", Param_Rag_Soc)
            rptSchedaCatastoUtilizzi.SetParameterValue("Piva_CUAA", Param_Piva_CUAA)
            rptSchedaCatastoUtilizzi.SetParameterValue("Indirizzo", Param_Indirizzo)


        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub



End Class