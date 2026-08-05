Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class SchedaTracciabilita
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_SchedaTracciabilita
    Private Log_Errori As String = ""

    Private DSAppezzamenti As DS_Appezzamenti
    Private DSAcquisti As DS_Acquisti
    Private DSOperazioniColturali As DS_OperazioniColturali
    Private DSVenditeColturali As DS_Vendite


    Dim Piva As String
    Dim Sa_Cod As String
    Dim Appezza As String
    Dim Id_Reg As String

    Dim Veg_Cod As String

    Dim strFiltroImpianti As String
    Dim strFiltroImpianto As String

    Dim strFiltroImpiantoMovimenti As String
    Dim strFiltroImpiantiMovimenti As String

    Dim LinkPaginaStampa As String
    Dim BaseCode As Integer


    '----- Gestione Querystring
    Dim Qs_DataInizio As String
    Dim Qs_DataFine As String
    Dim Qs_Arrotondamento As Integer
    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        'istanzio l'oggetto report
        rptStampa = New Rpt_SchedaTracciabilita

    End Sub

#End Region



    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim rag_soc As String
        Dim sa_nome As String
        Dim ind_des As String
        Dim frz_des As String
        Dim CAP As String
        Dim com_des As String
        Dim pro_cod As String
        Dim pro_cod_istat As String
        Dim com_cod_istat As String

        '##############################################################
        '#####  Recupero le date dalla querystring           ##########
        '##############################################################

        Qs_DataInizio = Stringa_Decodifica(Request.QueryString("dI").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_DataFine = Stringa_Decodifica(Request.QueryString("dF").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        If Not IsNothing(Request.QueryString("arr")) Then
            Qs_Arrotondamento = CInt(Stringa_Decodifica(Request.QueryString("arr").ToString,
                                      AgroKey_EncoderDecoder,
                                      Server))
        Else
            Qs_Arrotondamento = 4
        End If


        '##############################################################
        '#####  Recupero piva e veg_cod  ##############################
        '##############################################################

        Dim ElencoChiaviImpianto, ChiaveImpianto As String

        Dim strXmlVariabilistampe As String

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim Matrice_Variabili(0, 0) As String

        Dim i As Integer

        Try

            strXmlVariabilistampe = Session("strXmlVariabilistampe")

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabilistampe)

            If XmlDoc.HasChildNodes Then

                'Ricavo i parametri che servono

                XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

                XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 4)

                For i = 0 To XMLs_VariabiliStampe.Count - 1

                    XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                    Matrice_Variabili(i, 0) = XML_VariabiliStampe.GetAttribute("piva")
                    Matrice_Variabili(i, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")
                    Matrice_Variabili(i, 2) = XML_VariabiliStampe.GetAttribute("appezza")
                    Matrice_Variabili(i, 3) = XML_VariabiliStampe.GetAttribute("id_reg")
                    Matrice_Variabili(i, 4) = XML_VariabiliStampe.GetAttribute("veg_cod")

                    If i = 0 Then

                        Piva = Matrice_Variabili(i, 0)
                        Veg_Cod = Matrice_Variabili(i, 4)

                        Sa_Cod = Matrice_Variabili(i, 1)

                    End If

                    'se appezza=0 e id_reg=0 la stampa riguarda tutti gli impianti del centro
                    If Matrice_Variabili(i, 2) <> "0" AndAlso Matrice_Variabili(i, 3) <> "0" Then

                        strFiltroImpianto = " (Reg_Impianti.PIVA='" & XML_VariabiliStampe.GetAttribute("piva") & "' " &
                                        " AND Reg_Impianti.SA_COD=" & XML_VariabiliStampe.GetAttribute("sa_cod") &
                                        " AND Reg_Impianti.APPEZZA=" & XML_VariabiliStampe.GetAttribute("appezza") &
                                        " AND Reg_Impianti.ID_REG=" & XML_VariabiliStampe.GetAttribute("id_reg") &
                                        " ) OR"

                        strFiltroImpianti = strFiltroImpianti & strFiltroImpianto

                        strFiltroImpiantoMovimenti = " (Mov_Destinazioni.PIVA='" & XML_VariabiliStampe.GetAttribute("piva") & "' " &
                                                " AND Mov_Destinazioni.SA_COD=" & XML_VariabiliStampe.GetAttribute("sa_cod") &
                                                " AND Mov_Destinazioni.APPEZZA=" & XML_VariabiliStampe.GetAttribute("appezza") &
                                                " AND Mov_Destinazioni.Id_Destinazione=" & XML_VariabiliStampe.GetAttribute("id_reg") &
                                                " ) OR"

                        strFiltroImpiantiMovimenti = strFiltroImpiantiMovimenti & strFiltroImpiantoMovimenti


                        'piu' CENTRI...
                        If Sa_Cod <> Matrice_Variabili(i, 1) Then
                            Sa_Cod = "0"
                        End If

                    End If



                    'Per la sezione vendite
                    'Modificata in data 25/08/2010
                    'Mi calcolo un'altra stinga per il filtro sugli impianti
                    'per utilizzare l'IN invece dell'OR

                    'ricavo gli impianti
                    Piva = XML_VariabiliStampe.GetAttribute("piva")
                    Sa_Cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                    Appezza = XML_VariabiliStampe.GetAttribute("appezza")
                    Id_Reg = XML_VariabiliStampe.GetAttribute("id_reg")

                    'Genero la chiave impianto
                    ChiaveImpianto = Piva & "_" & Sa_Cod & "_" & Appezza & "_" & Id_Reg

                    ElencoChiaviImpianto &= ",'" & ChiaveImpianto & "'"

                Next

                'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
                ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)


                'tolgo l'ultimo OR
                strFiltroImpianti = Left(strFiltroImpianti, strFiltroImpianti.Length - 2)
                strFiltroImpianti = " AND (" & strFiltroImpianti & ")"

                strFiltroImpiantiMovimenti = Left(strFiltroImpiantiMovimenti, strFiltroImpiantiMovimenti.Length - 2)
                strFiltroImpiantiMovimenti = " AND (" & strFiltroImpiantiMovimenti & ")"

                'If str_Cultivar <> "" Then
                '    str_Cultivar = Left(str_Cultivar, str_Cultivar.Length - 2)
                'End If


            End If

        Catch ex As Exception
            Log_Errori &= "Elaborazione strXmlVariabilistampe: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "SchedaTracciabilita_Vegetale"

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            Try

                'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

                'CrystalReportViewer1.Style.Add("LEFT", "-275px")
                'CrystalReportViewer1.Style.Add("TOP", "0px")
                'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                'calcolo il basecode x l'utente
                AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, Nothing, Session("ASG_ProgressivoGIAS"))

                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Piva, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale

                If Sa_Cod <> "0" Then
                    Dim objInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                    objInd.Indirizzo_from_PivaSaCod2(Piva, Sa_Cod, rag_soc, sa_nome, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, objParametri_Server)
                    'Indirizzo_from_Piva(Server, Session, Page, Piva, Sa_Cod, rag_soc, sa_nome, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat)
                    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = sa_nome
                Else
                    Dim objInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                    objInd.Indirizzo_from_Piva(Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, objParametri_Server)
                    'Indirizzo_from_Piva(Server, Session, Page, Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat)
                End If

                Dim objLegale As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim Cognome, Nome As String

                objLegale.LegaleRappresentanteDati_from_PivaImpresa("", Nome, Cognome, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", Piva, "", objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextResponsabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Cognome & " " & Nome

                ' Qs_Sa_Cod non è mai valorizzato
                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = rag_soc
                CType(rptStampa.Section1.ReportObjects("TextIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ind_des
                CType(rptStampa.Section1.ReportObjects("TextFrazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = frz_des
                CType(rptStampa.Section1.ReportObjects("TextComune"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = com_des
                CType(rptStampa.Section1.ReportObjects("TextProvincia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pro_cod
                CType(rptStampa.Section1.ReportObjects("TextCap"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CAP

                CType(rptStampa.Section1.ReportObjects("TextCodice"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Imprese_Codici_Read.Leggi_Codice_from_Imprese_Codici(Piva, 1033, objParametri_Server)

            Catch ex As Exception
                Log_Errori &= "Valorizzazione intestazione report: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Dim DSAppezzamenti As New DS_Appezzamenti
            Dim DSAcquisti As New DS_Acquisti
            Dim DSOperazioniColturali As New DS_OperazioniColturali
            Dim DSVendite As New DS_Vendite
            Dim DSQtaVarietaVendite As New DS_QtaVarietaVendite
            Dim Veg_Des As String = ""

            'carico i dati nel datatable 
            Dim Hash_SupVarieta As New Hashtable
            CaricaDs_Appezzamenti(DSAppezzamenti, Veg_Des, Hash_SupVarieta, ElencoChiaviImpianto, objParametri_Server)
            CaricaDs_Acquisti(DSAcquisti)
            CaricaDs_OperazioniColturali(DSOperazioniColturali, ElencoChiaviImpianto)
            CaricaDs_Vendite(DSVendite, DSQtaVarietaVendite, Veg_Des, Hash_SupVarieta, ElencoChiaviImpianto, objParametri_Server)

            Try
                'If Not DSAppezzamenti Is Nothing AndAlso DSAppezzamenti.DS_Appezzamenti.Rows.Count > 0 Then
                CType(rptStampa.Section6.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Veg_Des ' DSAppezzamenti.DS_Appezzamenti.Rows(0).Item("veg_des") 'VegDes_from_VegCod(Server, Session, Page, Veg_Cod)
                'CType(rptStampa.Section6.ReportObjects("TextVarieta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DSAppezzamenti.DS_Appezzamenti.Rows(0).Item("cul_des")
                'End If
            Catch ex As Exception
                Log_Errori &= "Valorizzazione specie su intestazione report: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try
                rptStampa.OpenSubreport("Appezzamenti.rpt").SetDataSource(DSAppezzamenti)
            Catch ex As Exception
                Log_Errori &= "SetDataSource DSAppezzamenti: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try
                rptStampa.OpenSubreport("Rpt_TracciabilitaAcquisti.rpt").SetDataSource(DSAcquisti)
            Catch ex As Exception
                Log_Errori &= "SetDataSource DSAcquisti: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try
                rptStampa.OpenSubreport("Rpt_TracciabilitaOperazioni.rpt").SetDataSource(DSOperazioniColturali)
            Catch ex As Exception
                Log_Errori &= "SetDataSource DSOperazioniColturali: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try
                rptStampa.OpenSubreport("Rpt_TracciabilitaVendite.rpt").SetDataSource(DSVendite)
            Catch ex As Exception
                Log_Errori &= "SetDataSource DSVendite: " & vbCrLf & ex.Message & vbCrLf
            End Try

            Try
                rptStampa.OpenSubreport("Rpt_TracciabilitaVendite_Riepilogo.rpt").SetDataSource(DSQtaVarietaVendite)
            Catch ex As Exception
                Log_Errori &= "SetDataSource DSQtaVarietaVendite: " & vbCrLf & ex.Message & vbCrLf
            End Try


            ''faccio il databind col visualizzatore dei reports...
            'CrystalReportViewer1.ReportSource = rptStampa
            'CrystalReportViewer1.DataBind()

            ''array di dataset e data table
            'Dim dsRpt() As DataSet = {DSAppezzamenti, DSAcquisti, DSOperazioniColturali, DSVendite}

            ''salvo il report nella sessione
            'Session("DS") = dsRpt


            'NON SI PUO' USARE IL SALVATAGGIO DEL REPORT TEMPORANEO PERCHE' NON FUNZIONA
            ''MS Eliminato passaggio in session per passaggio report su file: Session("Report") = rptStampa
            'Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            'Try
            '    rptStampa.SaveAs(reportTemporaneo, True)
            'Catch ex As Exception
            '    Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            'End Try

            ' GC.Collect()

            Session("Report") = rptStampa

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            '03/03/2021: il log_errori non è stato ancora gestito
            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento & ", " & vbCrLf &
                                "Data Inizio = " & CStr(Qs_DataInizio) & ", " & vbCrLf &
                                "Data Fine = " & CStr(Qs_DataFine) & ", " & vbCrLf &
                                "Arrotondamento = " & CStr(Qs_Arrotondamento) & ", " & vbCrLf &
                                vbCrLf & vbCrLf & vbCrLf & vbCrLf &
                                Log_Errori

                Nome_File = "Log_Errori_" & Nome_Documento & "_" & CStr(Session("ASG_Utente_Username")) & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Tracciabilita",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaTracciabilita.aspx",
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))

            'NON SI PUO' USARE IL SALVATAGGIO DEL REPORT TEMPORANEO PERCHE' NON FUNZIONA
            'Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
            '                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
            '                    "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))


            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))

            '        ''faccio il databind col visualizzatore dei reports...
            '        'CrystalReportViewer1.ReportSource = rptStampa
            '        'CrystalReportViewer1.DataBind()

            '    End If

        End If 'postback



    End Sub


    '###########################################################################
    Public Sub CaricaDs_Appezzamenti(ByRef DSAppezzamenti As DS_Appezzamenti,
                                     ByRef Veg_Des As String,
                                     ByRef Hash_SupVarieta As Hashtable,
                                     ByVal ElencoChiaviImpianto As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Try

            Dim dt As DataTable = SchedaTracciabilitaVegetale_SezioneAppezzamenti_LeggiDS(objParametri,
                                                                        Piva,
                                                                        ElencoChiaviImpianto,
                                                                        Qs_DataInizio,
                                                                        Qs_DataFine)

            'dt.TableName = DSAppezzamenti.DS_Appezzamenti.TableName
            'DSAppezzamenti.Tables.Remove(DSAppezzamenti.DS_Appezzamenti.TableName)
            'DSAppezzamenti.Tables.Add(dt)
            'DSAppezzamenti.DS_Appezzamenti

            Dim dr As DS_Appezzamenti.DS_AppezzamentiRow
            For i = 0 To dt.Rows.Count - 1
                dr = DSAppezzamenti.DS_Appezzamenti.NewDS_AppezzamentiRow
                dr.App_Nome = dt.Rows(i).Item("App_Nome")
                dr.App_Nome_Breve = dt.Rows(i).Item("App_Nome_Breve")
                dr.Appezza = dt.Rows(i).Item("Appezza")
                dr.AREA = If(dt.Rows(i).Item("AREA") = "0", "", dt.Rows(i).Item("AREA"))
                dr.Campo_Cod = dt.Rows(i).Item("Campo_Cod")
                dr.Campo_Des = dt.Rows(i).Item("Campo_Des")
                dr.COM = dt.Rows(i).Item("COM")
                dr.Cul_Cod = dt.Rows(i).Item("Cul_Cod")
                dr.Cul_Des = dt.Rows(i).Item("Cul_Des")
                dr.FOGLIO = If(dt.Rows(i).Item("FOGLIO") = "0", "", dt.Rows(i).Item("FOGLIO"))
                dr.Grva_Cod = dt.Rows(i).Item("Grva_Cod")
                dr.Grva_Des = dt.Rows(i).Item("Grva_Des")
                dr.Id_Reg = dt.Rows(i).Item("Id_Reg")
                dr.MetodoProduzione_Appezzamento = dt.Rows(i).Item("MetodoProduzione_Appezzamento")
                dr.NUMERO = If(dt.Rows(i).Item("NUMERO") = "0", "", dt.Rows(i).Item("NUMERO"))
                dr.Piva = dt.Rows(i).Item("Piva")
                dr.Progetto = dt.Rows(i).Item("Progetto")
                dr.PROV = dt.Rows(i).Item("PROV")
                dr.Regolamento = dt.Rows(i).Item("Regolamento")
                dr.Sa_Cod = dt.Rows(i).Item("Sa_Cod")
                dr.SEZIONE = dt.Rows(i).Item("SEZIONE")
                dr.SUBALTERNO = dt.Rows(i).Item("SUBALTERNO")
                dr.Sup_App = dt.Rows(i).Item("Sup_App")
                dr.Sup_Imp = dt.Rows(i).Item("Sup_Imp")
                dr.Veg_Cod = dt.Rows(i).Item("Veg_Cod")
                dr.Veg_Des = dt.Rows(i).Item("Veg_Des")

                DSAppezzamenti.DS_Appezzamenti.Rows.Add(dr)
            Next
            DSAppezzamenti.DS_Appezzamenti.AcceptChanges()

            If Not IsNothing(DSAppezzamenti.DS_Appezzamenti) AndAlso DSAppezzamenti.DS_Appezzamenti.Rows.Count > 0 Then

                Dim Num_Appezzamento As Double
                Dim i As Integer
                Dim Num_Campo As Double
                Dim drR As DS_Appezzamenti.DS_AppezzamentiRow


                For i = 0 To DSAppezzamenti.DS_Appezzamenti.Rows.Count - 1

                    drR = DSAppezzamenti.DS_Appezzamenti.Rows(i)

                    If drR.App_Nome_Breve <> "" Then
                        drR.App_Nome = drR.App_Nome_Breve
                    Else
                        'se il nome di APPEZZAMENTO e CAMPO contengono un numero indico questo
                        'altrimenti lascio appezza-basecode
                        Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                        Num_Appezzamento = objNum.Numero_from_Stringa(drR.App_Nome)

                        'Num_Appezzamento = AppezzamentoNumero_from_AppezzamentoNome(Server, Session, Page, drR.App_Nome)
                        If Num_Appezzamento <> -1 Then
                            drR.App_Nome = CStr(Num_Appezzamento)
                        Else
                            drR.App_Nome = drR.Appezza - BaseCode
                            'se è un valore alto lo tronco alle ultime 3 cifre..
                            If drR.App_Nome.Length > 3 Then
                                drR.App_Nome = Right(drR.App_Nome, 3)
                            End If
                        End If

                    End If

                    'se l'appezzamento è dentro ad un campo
                    If drR.Campo_Cod <> 0 Then

                        Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                        Num_Campo = objNum.Numero_from_Stringa(drR.Campo_Des)

                        'Num_Campo = AppezzamentoNumero_from_AppezzamentoNome(Server, Session, Page, drR.Campo_Des)

                        If Num_Campo <> -1 Then
                            drR.Campo_Des = CStr(Num_Campo)
                        Else
                            drR.Campo_Des = drR.Campo_Cod - BaseCode
                            'se è un valore alto lo tronco alle ultime 3 cifre..
                            If drR.Campo_Des.Length > 3 Then
                                drR.Campo_Des = Right(drR.Campo_Des, 3)
                            End If
                        End If

                    End If

                    '-- tipologia varietale --
                    If Session("VisualizzaTipologieVarietali") IsNot Nothing AndAlso Session("VisualizzaTipologieVarietali") = True Then
                        If drR.Grva_Des <> "" Then
                            drR.Cul_Des = drR.Cul_Des & " - " & drR.Grva_Des
                        End If
                    End If

                Next


                '-----------------------------------------------------
                Dim objSqlDis As New Codex_Utility_Sql_DistinctOnDT
                Dim Hash_Temp As New Hashtable
                Dim Dt_Dist As DataTable
                Dim vet_chiave() As String = {"PIVA", "SA_COD", "APPEZZA", "ID_REG"}

                Dt_Dist = DSAppezzamenti.DS_Appezzamenti
                'faccio un distinct per avere gli impianti non ripetuti per le intersezioni con la particella
                Hash_Temp = objSqlDis.SelectDistinctWithHash_Base2(Dt_Dist, vet_chiave, Nothing)

                'scorro gli impianti per sommare la superficie dell'impianto x varietà
                If Not IsNothing(Dt_Dist) AndAlso Dt_Dist.Rows.Count > 0 Then

                    Dim Cul_Cod As Integer
                    Dim Sup_Imp, Tot_Sup_Imp As Double

                    For i = 0 To Dt_Dist.Rows.Count - 1

                        Veg_Des = Dt_Dist.Rows(i).Item("Veg_Des")
                        Cul_Cod = Dt_Dist.Rows(i).Item("Cul_Cod")
                        Sup_Imp = Dt_Dist.Rows(i).Item("Sup_Imp")

                        'Totali sup_imp per varietà 
                        If Not Hash_SupVarieta.ContainsKey(Cul_Cod) Then
                            'nuova varietà -> inserisco la sup
                            Hash_SupVarieta.Add(Cul_Cod, Sup_Imp)
                        Else
                            'già presente varietà -> aggiungo la sup al totale
                            Tot_Sup_Imp = CDbl(Hash_SupVarieta(Cul_Cod))
                            Tot_Sup_Imp += Sup_Imp
                            Hash_SupVarieta(Cul_Cod) = Tot_Sup_Imp
                        End If

                    Next

                End If

            End If


        Catch ex As Exception
            Log_Errori &= "CaricaDs_Appezzamenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '###########################################################################
    Public Sub CaricaDs_Acquisti(ByRef DSAcquisti As DS_Acquisti)

        Dim i As Integer

        Dim RsMaterie As DataTable
        Dim RsBolle As DataTable

        Dim RigaDs As DS_Acquisti.DS_AcquistiRow

        Dim Doc_Numero As String
        Dim Id_Agenda As Integer
        Dim Data As String
        Dim Data_Movimento As Date
        Dim Fornitore As String
        Dim Indirizzo As String

        Dim ArrayIdAgenda() As Integer
        Dim ArrayRsBolla As DataTable

        Try

            'dato l'impianto cerco le materie prime seminate o trapiantate...
            RsMaterie = MateriePrime_SeminateTrapiantate_from_Impianti(objParametri_Server, strFiltroImpiantiMovimenti)

            For Each drMaterie As DataRow In RsMaterie.Rows
                'data la materia prima ricavo la bolla di acquisto...
                RsBolle = BolleRicevute_from_MateriaPrima(objParametri_Server,
                                                      Piva,
                                                      drMaterie.Item("Mat_Cod"),
                                                      ArrayIdAgenda,
                                                      Qs_DataInizio,
                                                      Qs_DataFine)

                If ArrayIdAgenda IsNot Nothing AndAlso RsBolle IsNot Nothing Then

                    For i = 0 To UBound(ArrayIdAgenda)

                        Dim Id_Agenda_Corrente As Integer = ArrayIdAgenda(i)
                        ArrayRsBolla = (From x As DataRow In RsBolle Where x.Item("Id_Agenda") = Id_Agenda_Corrente).CopyToDataTable()

                        For Each drArrayRsBolla As DataRow In ArrayRsBolla.Rows

                            ' MESSAGGIO BY MAGA: 
                            ' C'E' DA GESTIRE IL
                            'ArrayRsBolla(i).Fields("cau_mov").Value = CAU_CONFERIMENTO !!!!!

                            'dati bolla ricevuta 
                            If drArrayRsBolla.Item("cau_mov") = CAU_REGISTRAZIONI Then

                                Doc_Numero = "DDT N° " & drArrayRsBolla.Item("Doc_Numero")
                                Id_Agenda = drArrayRsBolla.Item("Id_Agenda")
                                Data_Movimento = drArrayRsBolla.Item("Data_Movimento")
                                Data = CDate(drArrayRsBolla.Item("Data_Movimento")).ToShortDateString
                                Fornitore = drArrayRsBolla.Item("Fornitore")
                                Indirizzo = drArrayRsBolla.Item("Indirizzo")

                            Else 'dati carico in magazzino
                                'relativi solo alla materia prima seminata/trapiantata...

                                If drArrayRsBolla.Item("Mat_Cod") = drMaterie.Item("Mat_Cod") Then

                                    RigaDs = DSAcquisti.DS_Acquisti.NewDS_AcquistiRow

                                    RigaDs.Doc_Numero = Doc_Numero
                                    RigaDs.Id_Agenda = Id_Agenda
                                    RigaDs.Data_Movimento = Data_Movimento
                                    RigaDs.Data = Data
                                    RigaDs.Fornitore = Fornitore
                                    RigaDs.Indirizzo = Indirizzo

                                    RigaDs.Elem_Cod = drArrayRsBolla.Item("Elem_Cod")
                                    RigaDs.Mat_Cod = drArrayRsBolla.Item("Mat_Cod")
                                    RigaDs.Prodotto = drArrayRsBolla.Item("Mat_Des")

                                    RigaDs.Etichetta = "Cod." & drArrayRsBolla.Item("Etichetta")
                                    If CStr(drMaterie.Item("Lotto")).ToLower <> "indefinito" And drMaterie.Item("Lotto") <> "" Then
                                        RigaDs.Etichetta &= " - Lotto: " & drMaterie.Item("Lotto")
                                    End If

                                    RigaDs.Qta_Bio = drArrayRsBolla.Item("Qta")
                                    RigaDs.Udm_Cod = drArrayRsBolla.Item("Udm_Cod")
                                    RigaDs.Udm_Sim = drArrayRsBolla.Item("Udm_Sim")
                                    RigaDs.Sem_Cod = drMaterie.Item("Sem_Cod")

                                    DSAcquisti.DS_Acquisti.Rows.Add(RigaDs)

                                End If

                            End If
                        Next



                    Next

                End If
            Next


            '--------------------------------------------------------------------------------------
            'ORDINO IL DATASET
            '(per farlo importo nell'ordine le datarow ordinate e poi elimino le vecchie righe)
            '--------------------------------------------------------------------------------------

            Dim Righe_Old As Integer
            Dim dr As DataRow

            If Not IsNothing(DSAcquisti.DS_Acquisti) Then

                Righe_Old = DSAcquisti.DS_Acquisti.Count

                If Righe_Old > 0 Then

                    Try

                        '08/04/2019: gli faccio fare solo l'ordinamento, le operazioni le ho filtrate per date a monte sulla query
                        For Each dr In DSAcquisti.DS_Acquisti.Select("", "Data_Movimento")
                            'Dim dtAcq As DataTable = (From drAcq As DataRow In DSAcquisti.DS_Acquisti Where CDate(drAcq.Item("Data_Movimento")) <= CDate(Qs_DataFine) AndAlso CDate(drAcq.Item("Data_Movimento")) >= CDate(Qs_DataInizio)).CopyToDataTable()
                            ''Dim strFiltro As String = "Data_Movimento<='" & Qs_DataFine & "' AND Data_Movimento>='" & Qs_DataInizio & "'"

                            ''For Each dr In DSAcquisti.DS_Acquisti.Select(strFiltro, "Data_Movimento")
                            'For Each dr In dtAcq.Rows
                            DSAcquisti.DS_Acquisti.ImportRow(dr)
                        Next

                        'rendo permanenti le modifiche sul ds
                        DSAcquisti.DS_Acquisti.AcceptChanges()

                        For i = 0 To Righe_Old - 1
                            DSAcquisti.DS_Acquisti.Rows(i).Delete()
                        Next

                        'rendo permanenti le modifiche sul ds
                        DSAcquisti.DS_Acquisti.AcceptChanges()

                    Catch ex As Exception

                    End Try

                End If

            End If

        Catch ex As Exception
            Log_Errori &= "CaricaDs_Acquisti: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '###########################################################################
    Public Sub CaricaDs_OperazioniColturali(ByRef DSOperazioniColturali As DS_OperazioniColturali,
                                            ByVal ElencoChiaviImpianto As String)

        Dim objSqlDis As New Codex_Utility_Sql_DistinctOnDT

        Dim Dt As New DataTable
        Dim DtOperazione As DataTable
        Dim Righe() As DataRow
        Dim strId_Agenda() As String

        Dim DtApp As New DataTable
        Dim DrApp() As DataRow
        Dim strApp_Nome As String
        Dim App_Nome As String
        Dim Num_Appezzamento As Double
        Dim objNum As New AgronicaCoreDataProvider.UtilityProvider

        Dim Qta As Double
        Dim Qta_Tot As Double

        Dim DtProdotti As New DataTable

        Dim Elem_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Pro_Cod As Integer

        Dim RigaDs As DS_OperazioniColturali.DS_OperazioniColturaliRow
        Dim i, j, x As Integer

        Try

            Dim objStampaTracc As New AgronicaCoreStampeDAL.Tracciabilita

            Dt = objStampaTracc.SchedaTracciabilitaVegetale_SezioneOpColturali(Piva,
                                                                            ElencoChiaviImpianto,
                                                                            Qs_DataInizio,
                                                                            Qs_DataFine,
                                                                            "",
                                                                            "",
                                                                            objParametri_Server)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                'ottengo gli id_agenda e gli app_nome distinti
                strId_Agenda = objSqlDis.SelectDistinct(Dt, "id_agenda", False)

                If Not IsNothing(strId_Agenda) Then

                    'ciclo su tutti gli id_agenda
                    For i = 0 To strId_Agenda.Length - 1

                        Righe = Dt.Select("id_agenda=" & strId_Agenda(i))

                        DtOperazione = Dt.Clone()

                        For j = 0 To Righe.Length - 1
                            DtOperazione.ImportRow(Righe(j))
                        Next

                        '-----------------------------------------------------
                        'estraggo gli appezzamenti coinvolti
                        'e creo la stringa contenente tutti i nomi
                        DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "APPEZZA", False)

                        strApp_Nome = ""

                        If DtApp IsNot Nothing Then
                            For j = 0 To DtApp.Rows.Count - 1
                                If DtApp.Rows(j).Item("App_Nome_Breve") <> "" Then
                                    App_Nome = DtApp.Rows(j).Item("App_Nome_Breve")
                                Else
                                    'se il nome dell'appezzamento contiene un numero indico questo
                                    'altrimenti lascio appezza-basecode
                                    'Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                                    Num_Appezzamento = objNum.Numero_from_Stringa(DtApp.Rows(j).Item("app_nome"))


                                    'Num_Appezzamento = AppezzamentoNumero_from_AppezzamentoNome(Server, Session, Page, DtApp.Rows(j).Item("app_nome"))
                                    If Num_Appezzamento <> -1 Then
                                        App_Nome = CStr(Num_Appezzamento)
                                    Else
                                        App_Nome = DtApp.Rows(j).Item("appezza") - BaseCode
                                        'se è un valore alto lo tronco alle ultime 3 cifre..
                                        If CStr(strApp_Nome).Length > 3 Then
                                            App_Nome = Right(CStr(strApp_Nome), 3)
                                        End If
                                    End If
                                End If
                                strApp_Nome &= App_Nome & ","
                            Next
                        End If

                        'elimino l'ultima virgola
                        If strApp_Nome <> "" Then
                            strApp_Nome = Left(strApp_Nome, strApp_Nome.Length - 1)
                        End If

                        '-------------------------------------------------------------------
                        'RIEMPIO il DS
                        If DtOperazione.Rows.Count > 0 Then

                            'nella PRIMA RIGA metto tutti i dati dell'intervento
                            RigaDs = DSOperazioniColturali.DS_OperazioniColturali.NewDS_OperazioniColturaliRow

                            RigaDs.Id_Agenda = DtOperazione.Rows(0).Item("Id_Agenda")
                            RigaDs.Lav_Cod = DtOperazione.Rows(0).Item("Lav_Cod")
                            RigaDs.Lav_Des = DtOperazione.Rows(0).Item("Lav_Des")
                            RigaDs.Data = CDate(DtOperazione.Rows(0).Item("Data_Movimento")).ToShortDateString
                            RigaDs.Piva = DtOperazione.Rows(0).Item("Piva")
                            RigaDs.Sa_Cod = DtOperazione.Rows(0).Item("Sa_Cod")

                            RigaDs.App_Nome = strApp_Nome

                            RigaDs.Cau_Mov = DtOperazione.Rows(0).Item("Cau_Mov")
                            RigaDs.Elem_Cod = DtOperazione.Rows(0).Item("Elem_Cod")

                            Elem_Cod = DtOperazione.Rows(0).Item("Elem_Cod")
                            Pro_Cod = DtOperazione.Rows(0).Item("Pro_Cod")
                            Mat_Cod = DtOperazione.Rows(0).Item("Mat_Cod")

                            RigaDs.Pro_Cod = DtOperazione.Rows(0).Item("Pro_Cod")
                            RigaDs.Mat_Cod = DtOperazione.Rows(0).Item("Mat_Cod")

                            Qta = 0
                            Qta_Tot = 0

                            Select Case RigaDs.Elem_Cod

                                Case FERTILIZZANTI     'fertilizzanti

                                    Select Case RigaDs.Pro_Cod

                                        Case 0
                                            RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Mat_Des")

                                            DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "mat_cod", False)

                                            DrApp = DtOperazione.Select("mat_cod=" & Mat_Cod)

                                        Case Else
                                            RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Fer_Des")

                                            DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "pro_cod", False)

                                            DrApp = DtOperazione.Select("pro_cod=" & Pro_Cod)

                                    End Select

                                    For x = 0 To DrApp.Length - 1
                                        Qta = DrApp(x).Item("Qta")
                                        Qta_Tot = Qta_Tot + Qta
                                    Next

                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                    'RigaDs.Qta = Qta_Tot
                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)

                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                '-------------------------------------------------------

                                Case FORMULATI   'prodotti fitosanitari

                                    RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Fr_Des")

                                    DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "pro_cod", False)

                                    DrApp = DtOperazione.Select("pro_cod=" & Pro_Cod)

                                    For x = 0 To DrApp.Length - 1
                                        Qta = DrApp(x).Item("Qta")
                                        Qta_Tot = Qta_Tot + Qta
                                    Next

                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                    'RigaDs.Qta = Qta_Tot
                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)

                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                '-------------------------------------------------------

                                Case TRAPPOLE   'trappole

                                    Select Case RigaDs.Lav_Cod

                                        Case 107 'Installazione Trappole

                                            RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Trap_Des")
                                            'RigaDs.Qta = Format(DtOperazione.Rows(0).Item("Qta_Tot"), "0.0000")
                                            Qta_Tot = DtOperazione.Rows(0).Item("Qta_Tot")
                                            RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                            RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                            DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "pro_cod", False)

                                        Case 110 'Rilievo Avversita nelle Trappole

                                        Case 150 'Reinnesco Trappole

                                    End Select

                                '-------------------------------------------------------

                                Case SEMENTI     'sementi

                                    If Not IsDBNull(DtOperazione.Rows(0).Item("Mat_Des")) Then
                                        RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Mat_Des") &
                                                        " Cod." & DtOperazione.Rows(0).Item("Cod_Articolo")

                                        If CStr(DtOperazione.Rows(0).Item("Lotto")).ToLower <> "indefinito" And DtOperazione.Rows(0).Item("Lotto") <> "" Then
                                            RigaDs.Materia_Prima &= " - Lotto: " & DtOperazione.Rows(0).Item("Lotto")
                                        End If
                                    End If

                                    DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "mat_cod", False)

                                    DrApp = DtOperazione.Select("mat_cod=" & Mat_Cod)

                                    For x = 0 To DrApp.Length - 1
                                        Qta = DrApp(x).Item("Qta")
                                        Qta_Tot = Qta_Tot + Qta
                                    Next

                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                    'RigaDs.Qta = Qta_Tot
                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                '-------------------------------------------------------

                                Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI   'lavorati

                                    If Not IsDBNull(DtOperazione.Rows(0).Item("Mat_Des")) Then
                                        RigaDs.Materia_Prima = DtOperazione.Rows(0).Item("Mat_Des")
                                    End If

                                    DtProdotti = objSqlDis.SelectDistinct("Prodotti", DtOperazione, "mat_cod", False)

                                    DrApp = DtOperazione.Select("mat_cod=" & Mat_Cod)

                                    For x = 0 To DrApp.Length - 1
                                        Qta = DrApp(x).Item("Qta")
                                        Qta_Tot = Qta_Tot + Qta
                                    Next

                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                    'RigaDs.Qta = Qta_Tot
                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                    '-------------------------------------------------------

                            End Select

                            DSOperazioniColturali.DS_OperazioniColturali.Rows.Add(RigaDs)

                        End If

                        If DtProdotti.Rows.Count > 1 Then

                            For j = 0 To DtProdotti.Rows.Count - 1

                                Qta = 0
                                Qta_Tot = 0

                                Select Case Elem_Cod

                                'Case FERTILIZZANTI



                                    Case FORMULATI, FERTILIZZANTI, TRAPPOLE

                                        If (Pro_Cod <> 0 And Pro_Cod <> DtProdotti.Rows(j).Item("pro_cod")) Or
                                        (Mat_Cod <> 0 And Mat_Cod <> DtProdotti.Rows(j).Item("mat_cod")) Then

                                            RigaDs = DSOperazioniColturali.DS_OperazioniColturali.NewDS_OperazioniColturaliRow

                                            RigaDs.Lav_Des = ""
                                            RigaDs.Data = ""
                                            RigaDs.App_Nome = ""

                                            RigaDs.Elem_Cod = DtProdotti.Rows(j).Item("Elem_Cod")
                                            RigaDs.Pro_Cod = DtProdotti.Rows(j).Item("Pro_Cod")
                                            RigaDs.Mat_Cod = DtProdotti.Rows(j).Item("Mat_Cod")


                                            Select Case RigaDs.Elem_Cod

                                                Case FERTILIZZANTI     'fertilizzanti

                                                    Select Case RigaDs.Pro_Cod

                                                        Case 0
                                                            RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Mat_Des")
                                                            DrApp = DtOperazione.Select("mat_cod=" & RigaDs.Mat_Cod)

                                                        Case Else
                                                            RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Fer_Des")
                                                            DrApp = DtOperazione.Select("pro_cod=" & RigaDs.Pro_Cod)

                                                    End Select

                                                    For x = 0 To DrApp.Length - 1
                                                        'Qta = Format(DrApp(x).Item("Qta"), "0.0000")
                                                        Qta = DrApp(x).Item("Qta")
                                                        Qta_Tot = Qta_Tot + Qta
                                                    Next

                                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                                    'RigaDs.Qta = Qta_Tot
                                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                                '-------------------------------------------------------

                                                Case FORMULATI   'prodotti fitosanitari

                                                    RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Fr_Des")

                                                    DrApp = DtOperazione.Select("pro_cod=" & RigaDs.Pro_Cod)

                                                    For x = 0 To DrApp.Length - 1
                                                        'Qta = Format(DrApp(x).Item("Qta"), "0.0000")
                                                        Qta = DrApp(x).Item("Qta")
                                                        Qta_Tot = Qta_Tot + Qta
                                                    Next

                                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                                    'RigaDs.Qta = Qta_Tot
                                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                                '-------------------------------------------------------

                                                Case TRAPPOLE   'trappole

                                                    Select Case RigaDs.Lav_Cod

                                                        Case 107 'Installazione Trappole

                                                            RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Trap_Des")
                                                            'RigaDs.Qta = Format(DtProdotti.Rows(j).Item("Qta_Tot"), "0.0000")
                                                            Qta_Tot = DtProdotti.Rows(j).Item("Qta_Tot")
                                                            RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                                            RigaDs.Udm_Des = DtProdotti.Rows(j).Item("Udm_Sim")

                                                        Case 110 'Rilievo Avversita nelle Trappole

                                                        Case 150 'Reinnesco Trappole

                                                    End Select


                                            End Select

                                            DSOperazioniColturali.DS_OperazioniColturali.Rows.Add(RigaDs)

                                        End If

                                    '=====================================================

                                    Case SEMENTI, SEMILAVORATI_VEGETALI

                                        If Mat_Cod <> DtProdotti.Rows(j).Item("mat_cod") Then

                                            RigaDs = DSOperazioniColturali.DS_OperazioniColturali.NewDS_OperazioniColturaliRow

                                            RigaDs.Lav_Des = ""
                                            RigaDs.Data = ""
                                            RigaDs.App_Nome = ""

                                            RigaDs.Elem_Cod = DtProdotti.Rows(j).Item("Elem_Cod")
                                            RigaDs.Pro_Cod = DtProdotti.Rows(j).Item("Pro_Cod")
                                            RigaDs.Mat_Cod = DtProdotti.Rows(j).Item("Mat_Cod")

                                            Select Case RigaDs.Elem_Cod

                                                Case SEMENTI    'sementi

                                                    If Not IsDBNull(DtProdotti.Rows(j).Item("Mat_Des")) Then
                                                        RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Mat_Des") &
                                                                            "Cod." & DtProdotti.Rows(j).Item("Cod_Articolo")
                                                        If CStr(DtOperazione.Rows(0).Item("Lotto")).ToLower <> "indefinito" And DtOperazione.Rows(0).Item("Lotto") <> "" Then
                                                            RigaDs.Materia_Prima &= " - Lotto: " & DtOperazione.Rows(0).Item("Lotto")
                                                        End If
                                                    End If

                                                    DrApp = DtOperazione.Select("mat_cod=" & RigaDs.Mat_Cod)

                                                    For x = 0 To DrApp.Length - 1
                                                        'Qta = Format(DrApp(x).Item("Qta"), "0.0000")
                                                        Qta = DrApp(x).Item("Qta")
                                                        Qta_Tot = Qta_Tot + Qta
                                                    Next

                                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                                    'RigaDs.Qta = Qta_Tot
                                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                                '-------------------------------------------------------

                                                Case SEMILAVORATI_VEGETALI   'lavorati

                                                    RigaDs.Materia_Prima = DtProdotti.Rows(j).Item("Mat_Des")
                                                    RigaDs.Qta = Format(DtProdotti.Rows(j).Item("Qta_Tot"), "0.0000")
                                                    RigaDs.Udm_Des = DtProdotti.Rows(j).Item("Udm_Sim")

                                                    DrApp = DtOperazione.Select("mat_cod=" & RigaDs.Mat_Cod)

                                                    For x = 0 To DrApp.Length - 1
                                                        'Qta = Format(DrApp(x).Item("Qta"), "0.0000")
                                                        Qta = DrApp(x).Item("Qta")
                                                        Qta_Tot = Qta_Tot + Qta
                                                    Next

                                                    'RigaDs.Qta = Format(Qta_Tot, "0.0000")
                                                    'RigaDs.Qta = Qta_Tot
                                                    RigaDs.Qta = Arrotonda(Qta_Tot, Qs_Arrotondamento)
                                                    RigaDs.Udm_Des = DtOperazione.Rows(0).Item("Udm_Sim")

                                                    '-------------------------------------------------------

                                            End Select

                                            DSOperazioniColturali.DS_OperazioniColturali.Rows.Add(RigaDs)

                                        End If

                                        '=====================================================


                                End Select



                            Next

                        End If

                    Next


                End If


            End If


        Catch ex As Exception
            Log_Errori &= "CaricaDs_OperazioniColturali: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '###########################################################################
    'Nuova routine creata il 25/08/2010
    Public Sub CaricaDs_Vendite(ByRef DSVendite As DS_Vendite,
                                ByRef DSQtaVarietaVendite As DS_QtaVarietaVendite,
                                ByVal Veg_Des As String,
                                ByVal Hash_SupVarieta As Hashtable,
                                ByVal ElencoChiaviImpianto As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dati gli impianti ->
        'leggere tutte le operazioni di agenda di uscita dei semilavorati/trasformati raccolti da quegli impianti
        'riempire il dataset
        'sommare le qta per varietà e calibro

        'Dim DT_Lavorati As DataTable
        Dim DT_Agenda As DataTable
        Dim DT_Auto As DataTable
        Dim i As Integer
        Dim Mat_Cod, Elem_Cod, Pro_Cod, Cod_Progetto, Fase_Cod, Udm_Cod As Integer
        Dim Cal_Cod_Campionatura As Integer
        Dim Cal_Cod_Calibro As Integer = 0
        Dim Codice_Indice As Integer = 0
        Dim Codice_Danno As Integer = 0
        Dim Desc_Calibro As String = ""
        Dim Desc_Indice As String = ""
        Dim Desc_Danno As String = ""
        Dim Valore_Indice As String = ""
        Dim UdmCod_Indice As Integer = 0
        Dim Lotto As String
        Dim Mat_Des, Cod_Articolo As String
        Dim Cul_Cod, Veg_Cod As Integer
        Dim Calibro As String
        Dim Mac_Cod As Integer
        Dim Cul_Des As String

        'Dim Hash_TotVarieta As New Hashtable
        Dim Hash_Varieta As New Hashtable
        Dim Hash_TotVarietaCalibro As New Hashtable
        Dim Hash_Calibro As New Hashtable
        Dim Key_VarietaCalibro As String
        'Dim QtaVarieta As Double
        Dim QtaVarietaCalibro As Double

        Dim Numero_Doc As String
        Dim Numero_Doc_Allegato As String
        Dim Id_Agenda As Integer
        Dim Data_Movimento As Date
        Dim Data_Movimento_Allegato As Date
        Dim Lav_Cod As Integer
        Dim Acquirente As String
        Dim Indirizzo As String
        Dim Qta As String
        Dim Udm_Sim As String
        Dim Mezzo As Integer
        Dim Automezzo As String

        Dim RigaDs As DS_Vendite.DS_VenditeRow
        Dim RigaDsQta As DS_QtaVarietaVendite.DS_QtaVarietaVenditeRow

        Dim objStampaTracc As New AgronicaCoreStampeDAL.Tracciabilita

        Try

            DT_Agenda = objStampaTracc.SchedaTracciabilitaVegetale_SezioneVendite(Piva,
                                                                            ElencoChiaviImpianto,
                                                                            Qs_DataInizio,
                                                                            Qs_DataFine,
                                                                            "",
                                                                            "",
                                                                            objParametri_Server)


            If Not IsNothing(DT_Agenda) AndAlso DT_Agenda.Rows.Count > 0 Then

                For i = 0 To DT_Agenda.Rows.Count - 1

                    '----------------------------------------------------------
                    'Ricavo i dati
                    Lav_Cod = DT_Agenda.Rows(i).Item("Lav_Cod")

                    'chiave della materia prima
                    Elem_Cod = DT_Agenda.Rows(i).Item("Elem_Cod")
                    Pro_Cod = DT_Agenda.Rows(i).Item("Pro_Cod")
                    Mat_Cod = DT_Agenda.Rows(i).Item("Mat_Cod")
                    Cod_Progetto = DT_Agenda.Rows(i).Item("Cod_Progetto")
                    Fase_Cod = DT_Agenda.Rows(i).Item("Fase_Cod")
                    Lotto = DT_Agenda.Rows(i).Item("Lotto")
                    If Lotto.ToLower = "indefinito" Then
                        Lotto = ""
                    End If
                    Cul_Des = DT_Agenda.Rows(i).Item("Cul_Des")
                    Cal_Cod_Campionatura = DT_Agenda.Rows(i).Item("Cal_Cod")
                    Calibro = " Campionatura: " & Leggi_CampionaturaRaccolto(objParametri,
                                                                            Cal_Cod_Calibro,
                                                                            Desc_Calibro,
                                                                            Codice_Indice,
                                                                            Desc_Indice,
                                                                            Codice_Danno,
                                                                            Desc_Danno,
                                                                            Valore_Indice,
                                                                            UdmCod_Indice,
                                                                            Cal_Cod_Campionatura,
                                                                            "", 0, 0,
                                                                            0, "", True)
                    Udm_Cod = DT_Agenda.Rows(i).Item("Udm_Cod")

                    Mat_Des = DT_Agenda.Rows(i).Item("Mat_Des")
                    Cod_Articolo = DT_Agenda.Rows(i).Item("Cod_Articolo")
                    Cul_Cod = DT_Agenda.Rows(i).Item("Cul_Cod")
                    Veg_Cod = DT_Agenda.Rows(i).Item("Veg_Cod")

                    Key_VarietaCalibro = CStr(Cul_Cod) & "|" & CStr(Cal_Cod_Calibro)

                    Id_Agenda = DT_Agenda.Rows(i).Item("Id_Agenda")

                    Select Case Lav_Cod

                        Case LAVCOD_ACCETTAZIONE_DIVERSI

                            Data_Movimento = DT_Agenda.Rows(i).Item("Data_DDTConf") 'data del ddt di conf del produttore
                            Data_Movimento_Allegato = DT_Agenda.Rows(i).Item("Data") 'data della bolla di accett

                            Numero_Doc_Allegato = Ricava_NumeroDocumento_Con_Sequenza(
                                                    DT_Agenda.Rows(i).Item("Doc_Numero_Sin"),
                                                    DT_Agenda.Rows(i).Item("Doc_Numero"),
                                                    DT_Agenda.Rows(i).Item("Doc_Numero_Des"),
                                                    3,
                                                    5,
                                                    0,
                                                    "0")

                            Numero_Doc = Ricava_NumeroDocumento_Senza_Sequenza(DT_Agenda.Rows(i).Item("Doc_Numero_Sin_DDTConf"),
                                                                          DT_Agenda.Rows(i).Item("Doc_Numero_DDTConf"),
                                                                          DT_Agenda.Rows(i).Item("Doc_Numero_Des_DDTConf"))


                        Case Else
                            Data_Movimento = DT_Agenda.Rows(i).Item("Data")
                            Numero_Doc = Ricava_NumeroDocumento_Senza_Sequenza(DT_Agenda.Rows(i).Item("Doc_Numero_Sin"),
                                                                            DT_Agenda.Rows(i).Item("Doc_Numero"),
                                                                            DT_Agenda.Rows(i).Item("Doc_Numero_Des"))
                    End Select

                    Acquirente = Componi_RagSocNome_Contatto(DT_Agenda.Rows(i).Item("Rag_Soc"), DT_Agenda.Rows(i).Item("Nome"), DT_Agenda.Rows(i).Item("Cognome"))
                    Indirizzo = Componi_Indirizzo(DT_Agenda.Rows(i).Item("ind_des"), DT_Agenda.Rows(i).Item("frz_des"), DT_Agenda.Rows(i).Item("cap"), DT_Agenda.Rows(i).Item("com_des"), DT_Agenda.Rows(i).Item("comuni_prov"), DT_Agenda.Rows(i).Item("stato"), False)
                    Qta = DT_Agenda.Rows(i).Item("Qta")
                    Udm_Sim = DT_Agenda.Rows(i).Item("Udm_Sim")
                    Mezzo = DT_Agenda.Rows(i).Item("Mezzo")

                    Mac_Cod = DT_Agenda.Rows(i).Item("Mac_Cod")

                    Select Case Mac_Cod
                        Case 0
                            Automezzo = DT_Agenda.Rows(i).Item("mezzo_trasporto") & " " & DT_Agenda.Rows(i).Item("targa")
                        Case Else
                            'leggi macchine
                            DT_Auto = ParcoMacchine_Leggi(objParametri, Piva, Mac_Cod, True, , , , , , , , , False)

                            If Not IsNothing(DT_Auto) AndAlso DT_Auto.Rows.Count > 0 Then
                                Automezzo = DT_Auto.Rows(i).Item("mac_des") & " " & DT_Auto.Rows(i).Item("targa")
                            End If
                    End Select
                    '----------------------------------------------------------

                    '----------------------------------------------------------
                    'Letture aggiuntive per ricavare operazioni collegate
                    Select Case Lav_Cod

                        Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
                            Indirizzo = ""
                            Acquirente = "Autoconsumo"
                            Numero_Doc = ""

                        '-------------------------

                        Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
                            Indirizzo = ""
                            Acquirente = "Vendita"
                            Numero_Doc = ""

                        '-------------------------

                        Case LAVCOD_FATTURA_EMESSA '(immediata)

                            Numero_Doc = "Fattura n." & Numero_Doc

                        '-------------------------

                        Case LAVCOD_RICEVUTA_EMESSA
                            Numero_Doc = "Ricevuta n." & Numero_Doc

                        '-------------------------

                        Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                            'leggere se c'è fattura allegata
                            Numero_Doc = "DDT n." & Numero_Doc

                        '-------------------------

                        Case LAVCOD_CONFERIMENTO_DIVERSI 'like agrisfera/gentili

                            'leggere se c'è accettazione per aggiornare la qta
                            'leggere se c'è fattura
                            Numero_Doc = "DDT n." & Numero_Doc

                        '-------------------------

                        Case LAVCOD_CONFERIMENTO 'like agribologna

                            Numero_Doc = "DDT n." & Numero_Doc

                        '-------------------------

                        Case LAVCOD_ACCETTAZIONE_DIVERSI 'like fruttagel
                            Numero_Doc = "DDT n." & Numero_Doc & " (Rif. Bolla Accett. n." & Numero_Doc_Allegato & " del " & Data_Movimento_Allegato & ")"

                            '-------------------------


                    End Select
                    '----------------------------------------------------------

                    '----------------------------------------------------------
                    'Calcolo delle quantità

                    'Totali Quantità per varietà e calibro
                    If Not Hash_TotVarietaCalibro.ContainsKey(Key_VarietaCalibro) Then
                        'nuova varietà-calibro -> inserisco la qta
                        Hash_TotVarietaCalibro.Add(Key_VarietaCalibro, Qta)
                    Else
                        'già presente varietà-calibro -> aggiungo la qta al totale
                        QtaVarietaCalibro = CDbl(Hash_TotVarietaCalibro(Key_VarietaCalibro))
                        QtaVarietaCalibro += Qta
                        Hash_TotVarietaCalibro(Key_VarietaCalibro) = QtaVarietaCalibro
                    End If

                    'varietà 
                    If Not Hash_Varieta.ContainsKey(Cul_Cod) Then
                        'nuova varietà -> inserisco la riga
                        Hash_Varieta.Add(Cul_Cod, Cul_Des)
                    End If

                    'calibro 
                    If Not Hash_Calibro.ContainsKey(Cal_Cod_Calibro) Then
                        'nuovo calibro -> inserisco la riga
                        Hash_Calibro.Add(Cal_Cod_Calibro, Desc_Calibro)
                    End If

                    ''Totali Quantità per varietà 
                    'If Not Hash_TotVarieta.ContainsKey(Cul_Cod) Then
                    '    'nuova varietà -> inserisco la qta
                    '    Hash_TotVarieta.Add(Cul_Cod, Qta)
                    'Else
                    '    'già presente varietà -> aggiungo la qta al totale
                    '    QtaVarieta = CDbl(Hash_TotVarietaCalibro(Cul_Cod))
                    '    QtaVarieta += Qta
                    '    Hash_TotVarieta(Cul_Cod) = QtaVarieta
                    'End If
                    '----------------------------------------------------------

                    '----------------------------------------------------------
                    'Riempimento dataset vendite
                    RigaDs = DSVendite.DS_Vendite.NewDS_VenditeRow

                    RigaDs.Doc_Numero = Numero_Doc
                    RigaDs.Id_Agenda = Id_Agenda
                    RigaDs.Data_Movimento = Data_Movimento
                    RigaDs.Data = Data_Movimento.ToShortDateString
                    RigaDs.Acquirente = Acquirente
                    RigaDs.Indirizzo = Indirizzo
                    RigaDs.Mezzo = Mezzo
                    RigaDs.Note = ""
                    RigaDs.Elem_Cod = Elem_Cod
                    RigaDs.Mat_Cod = Mat_Cod
                    RigaDs.Prodotto = Mat_Des & " - Campionatura: " & Desc_Calibro & " (Cod. Articolo: " & Cod_Articolo & ")"
                    RigaDs.Etichetta = Lotto
                    RigaDs.Qta = Qta
                    RigaDs.Udm_Cod = Udm_Cod
                    RigaDs.Udm_Sim = Udm_Sim
                    RigaDs.Note = Automezzo

                    DSVendite.DS_Vendite.Rows.Add(RigaDs)
                    '----------------------------------------------------------

                Next

                If Not IsNothing(Hash_TotVarietaCalibro) AndAlso Hash_TotVarietaCalibro.Keys.Count > 0 Then

                    Dim MyKeys As ICollection
                    Dim Cod_Varieta As Integer
                    Dim Cod_Calibro As Integer
                    Dim Chiave As String
                    Dim Qta_Calibro As Double

                    MyKeys = Hash_TotVarietaCalibro.Keys()

                    For Each Chiave In MyKeys

                        Cod_Varieta = Chiave.Split("|")(0)
                        Cod_Calibro = Chiave.Split("|")(1)
                        Qta_Calibro = Hash_TotVarietaCalibro(Chiave)

                        '----------------------------------------------------------
                        'Riempimento dataset riepilogo quantità raccolte
                        RigaDsQta = DSQtaVarietaVendite.DS_QtaVarietaVendite.NewDS_QtaVarietaVenditeRow

                        RigaDsQta.Veg_Cod = Veg_Cod
                        RigaDsQta.Veg_Des = Veg_Des
                        RigaDsQta.Cul_Cod = Cod_Varieta
                        RigaDsQta.Cul_Des = Hash_Varieta(Cod_Varieta)
                        RigaDsQta.Cod_Calibro = Cod_Calibro
                        RigaDsQta.Desc_Calibro = Hash_Calibro(Cod_Calibro)
                        RigaDsQta.Qta_Calibro = Qta_Calibro
                        If Not IsNothing(Hash_SupVarieta) AndAlso Hash_SupVarieta.Keys.Count > 0 Then
                            If Not IsNothing(Hash_SupVarieta(Cod_Varieta)) AndAlso Hash_SupVarieta(Cod_Varieta) <> 0 Then
                                RigaDsQta.Tot_SupImp_Varieta = Hash_SupVarieta(Cod_Varieta)
                            Else
                                'questo caso capita se la varietà del semilavorato raccolto non coincide con la avrietà dell'impianto
                                'quindi non si riesce a risalire alla superficie dell'impianto per calcolare la resa
                                'non metto 0 altrimenti la formula resa_ettaro del report da errore
                                RigaDsQta.Tot_SupImp_Varieta = 0.0001
                            End If
                        Else
                            'questo caso capita se la varietà del semilavorato raccolto non coincide con la avrietà dell'impianto
                            'quindi non si riesce a risalire alla superficie dell'impianto per calcolare la resa
                            'non metto 0 altrimenti la formula resa_ettaro del report da errore
                            RigaDsQta.Tot_SupImp_Varieta = 0.0001
                        End If

                        DSQtaVarietaVendite.DS_QtaVarietaVendite.Rows.Add(RigaDsQta)
                        '----------------------------------------------------------

                    Next


                End If


            End If



        Catch ex As Exception
            Log_Errori &= "CaricaDs_Vendite: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Public Function Arrotonda(ByVal Valore As Double, ByVal TipoArrotondamento As Integer) As Double

        Select Case TipoArrotondamento

            Case -1 'nessuno

                Return Valore

            Case Else 'da 0 = unità a 4 = 4 decimali

                'in questo caso TipoArrotondamento corrisponde al numero di cifre decimali
                Return Math.Round(Valore, TipoArrotondamento)

        End Select

    End Function

    Public Function Componi_RagSocNome_Contatto(ByVal Rag_Soc As String,
                                            ByVal Nome As String,
                                            ByVal Cognome As String) As String

        Dim RagSocNome As String

        If Cognome <> "" Then
            RagSocNome = Nome & " " & Cognome
        Else
            RagSocNome = Rag_Soc
        End If


        Return RagSocNome

    End Function


    '#####################################################################################################
    Public Function Componi_Indirizzo(ByVal Ind_Des As String,
                                        ByVal Frz_Des As String,
                                        ByVal Cap As String,
                                        ByVal Comune As String,
                                        ByVal Provincia As String,
                                        ByVal Stato As String,
                                        ByVal Flag_Stato As Boolean) As String

        Dim Indirizzo As String

        Sistema_Comune_Provincia(Comune, Provincia)

        Indirizzo = Ind_Des & " " & Cap & " " & Comune & " (" & Provincia & ")"

        If Flag_Stato Then
            Indirizzo &= " " & Stato
        End If

        Return Indirizzo

    End Function

    Public Function MateriePrime_SeminateTrapiantate_from_Impianti(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                   ByVal strFiltroImpianti As String) _
                                                                   As DataTable

        Dim strQuery As String
        Dim MessaggioErrore As String
        Dim NomeRoutine As String = "MateriePrime_SeminateTrapiantate_from_Impianti"
        Dim dt As DataTable

        Try


            'Preparo la query
            strQuery = " "

            strQuery &= " SELECT DISTINCT Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, "
            strQuery &= " Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, Materie_Prime.Sem_Cod "
            strQuery &= " FROM Agenda INNER JOIN "
            strQuery &= " Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND "
            strQuery &= " Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN "
            strQuery &= " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
            strQuery &= " Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN "
            strQuery &= " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND "
            strQuery &= " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND "
            strQuery &= " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND "
            strQuery &= " Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det INNER JOIN "
            strQuery &= " Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND "
            strQuery &= " Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod "
            strQuery &= " WHERE (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) "
            strQuery &= strFiltroImpianti

            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            dt = dp.EseguiQuery_Lettura(objParametri, strQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt


    End Function

    Public Function BolleRicevute_from_MateriaPrima(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal Piva As String,
                                                    ByVal Mat_Cod As Integer,
                                                    ByRef ArrayId_Agenda() As Integer,
                                                    ByVal Data_inizio As Date,
                                                    ByVal Data_fine As Date) As DataTable


        Dim NomeRoutine As String = "BolleRicevute_from_MateriaPrima"

        Dim DT As DataTable

        Dim strQuery As String
        Dim MessaggioErrore As String
        Dim i As Integer

        Dim strQuery_Agenda As String
        Dim strId_Agenda As String = ""

        'dato il mat_cod ricavo gli id_agenda

        Try

            strQuery_Agenda = " SELECT DISTINCT Agenda.Id_Agenda"
            strQuery_Agenda &= " FROM  Agenda INNER JOIN"
            strQuery_Agenda &= " Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN"
            strQuery_Agenda &= " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND"
            strQuery_Agenda &= " Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov"

            '1000 = Ricevimento fattura
            '1021 = Acquisto
            '1025 = Ricevimento Bolla
            '1050 = Conferimento beni
            strQuery_Agenda &= " WHERE Agenda.Lav_Cod IN (1000, 1021, 1025, 1050) "
            strQuery_Agenda &= " AND Agenda.PIVA = '" & SQL_SaveText(Piva) & "'"
            strQuery_Agenda &= " AND  Movimenti_dettagli.Mat_Cod = " & SQL_SaveNum(Mat_Cod)
            strQuery_Agenda &= " AND  Movimenti.Data_movimento <= " & SQL_SaveDate(Data_fine)
            strQuery_Agenda &= " AND  Movimenti.Data_movimento >= " & SQL_SaveDate(Data_inizio)
            strQuery_Agenda &= " ORDER BY Agenda.Id_Agenda"

            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            DT = dp.EseguiQuery_Lettura(objParametri, strQuery_Agenda.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If MessaggioErrore <> "" Then
            Throw New Exception(MessaggioErrore)
        Else

            For i = 0 To DT.Rows.Count - 1

                ReDim Preserve ArrayId_Agenda(i)
                ArrayId_Agenda(i) = DT.Rows(i).Item("Id_Agenda")

                strId_Agenda &= DT.Rows(i).Item("Id_Agenda") & ","
            Next

        End If

        DT = Nothing

        Try

            If strId_Agenda <> "" Then

                strId_Agenda = Left(strId_Agenda, strId_Agenda.Length - 1)

                strQuery = " "
                strQuery &= " SELECT DISTINCT"
                strQuery &= " Agenda.Id_Agenda, Movimenti.Data_Movimento, Movimenti.Cau_Mov, Agenda.Lav_Cod, "
                strQuery &= " ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod, "
                strQuery &= " ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod, "
                strQuery &= " ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod, "
                strQuery &= " ISNULL(Movimenti_dettagli.Lotto, '') AS Lotto, "
                strQuery &= " Materie_Prime.Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Etichetta, "
                strQuery &= " ISNULL(Movimenti_dettagli.Qta, '') AS Qta, ISNULL(Movimenti_dettagli.Udm_Cod, 0) AS Udm_Cod, "
                strQuery &= " ISNULL(UnitaMisura.UDM_SIM, '') AS Udm_Sim, (Movimenti.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Movimenti.Doc_Numero)) + Movimenti.Doc_Numero_Des) AS Doc_Numero, "
                strQuery &= " '' AS Prodotto, ISNULL(Contatti.Rag_Soc, '') AS Fornitore, "
                strQuery &= " ISNULL(Indirizzi.ind_des, '') + ' ' + ISNULL(Indirizzi.frz_des, '') + ' ' + ISNULL('-' + ISTAT.LOCALITA + '-', '') + ' ' + ISNULL('(' + ISTAT.COMUNI_PROV + ')', '') AS Indirizzo, "
                strQuery &= " ISNULL(Contatti.Cod_Contatto, '') AS Cod_Fisc, ISNULL(Rapporti_Contabili.Rapporto_Des, '') AS Qualifica "

                strQuery &= " FROM Mov_Destinazioni INNER JOIN"
                strQuery &= " Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND"
                strQuery &= " Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND "
                strQuery &= " Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN"
                strQuery &= " UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD LEFT OUTER JOIN"
                strQuery &= " Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND "
                strQuery &= " Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod RIGHT OUTER JOIN"
                strQuery &= " Rapporti_Contabili INNER JOIN"
                strQuery &= " Risorse_Umane INNER JOIN"
                strQuery &= " Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva ON "
                strQuery &= " Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto RIGHT OUTER JOIN"
                strQuery &= " Indirizzi RIGHT OUTER JOIN"
                strQuery &= " Agenda INNER JOIN"
                'strQuery &= " Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON "
                strQuery &= " Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ON "
                strQuery &= " Indirizzi.cod_indirizzo = Movimenti.Cod_IndirizzoRisUm LEFT OUTER JOIN"
                strQuery &= " ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ON Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm ON"
                strQuery &= " Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND "
                strQuery &= " Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov"

                '1000 = Ricevimento fattura
                '1021 = Acquisto
                '1025 = Ricevimento Bolla
                '1050 = Conferimento beni
                strQuery &= " WHERE Agenda.Lav_Cod IN (1000, 1021, 1025, 1050) "
                strQuery &= " AND Agenda.Piva = '" & SQL_SaveText(Piva) & "'"
                strQuery &= " AND Movimenti.Id_Agenda IN (" & strId_Agenda & ") "
                strQuery &= " ORDER BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Movimenti.Cau_Mov "


                '--------------------------------------------------------------------------
                Dim dp As New AgronicaCoreDataProvider.DataProvider
                DT = dp.EseguiQuery_Lettura(objParametri, strQuery, NomeRoutine)
                '--------------------------------------------------------------------------

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '################################################################################
    'nuova versione:
    'AgronicaCoreContabDAL.Contabilita_R.Leggi_CampionaturaRaccolto()
    Public Function Leggi_CampionaturaRaccolto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef Codice_Calibro As Integer,
                                                ByRef Desc_Calibro As String,
                                                ByRef Codice_Indice As Integer,
                                                ByRef Desc_Indice As String,
                                                ByRef Codice_Danno As Integer,
                                                ByRef Desc_Danno As String,
                                                ByRef Valore_Indice As String,
                                                ByRef UdmCod_Indice As Integer,
                                                ByVal Codice As Integer,
                                                Optional ByVal Tipo As String = "",
                                                Optional ByVal Tipo_Cod As Long = 0,
                                                Optional ByVal Udm_Cod As Long = 0,
                                                Optional ByVal Progressivo_Origine As Integer = 0,
                                                Optional ByVal Piva_SuperUser_Origine As String = "",
                                                Optional ByVal Flag_AncheImportati As Boolean = False) As String

        Dim Str_CampionaturaRaccolto As String = ""


        Select Case Codice

            Case Is > 0 'cal_cod

                'Calibro
                Str_CampionaturaRaccolto = Leggi_CalibroFrutto(objParametri, Codice, , , , )

                Codice_Calibro = Codice
                Codice_Indice = 0
                Codice_Danno = 0
                Valore_Indice = ""
                UdmCod_Indice = 0


            Case Is <= 0 ' progressivo

                'Campionatura

                Str_CampionaturaRaccolto = Leggi_MateriaPrima_Campionature(objParametri,
                                                                            Codice_Calibro,
                                                                            Desc_Calibro,
                                                                            Codice_Indice,
                                                                            Desc_Indice,
                                                                            Codice_Danno,
                                                                            Desc_Danno,
                                                                            Valore_Indice,
                                                                            UdmCod_Indice,
                                                                            Codice,
                                                                            Tipo,
                                                                            Tipo_Cod,
                                                                            Udm_Cod,
                                                                            Progressivo_Origine,
                                                                            Piva_SuperUser_Origine,
                                                                            Flag_AncheImportati,
                                                                            , , , )


        End Select

        Return Str_CampionaturaRaccolto


    End Function

    '################################################################################
    Public Function Leggi_CalibroFrutto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Cal_Cod As Integer,
                                        Optional ByVal TestoRicerca As String = "",
                                        Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                        Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
                                        Optional ByVal Ordinamento As String = "") As String

        Dim Calibro_Des As String = ""
        Dim Dt As DataTable = CalibriFrutti_Leggi(objParametri, Cal_Cod, , , , )

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then

                Calibro_Des = CStr(Dt.Rows(0).Item("Cal_Des"))

            End If

        End If

        Return Calibro_Des


    End Function


    '################################################################################
    'nuova versione:
    'AgronicaCoreContabDAL.Materie_prime_Campion_R.Leggi_MateriaPrima_Campionature()
    Public Function Leggi_MateriaPrima_Campionature(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef Codice_Calibro As Integer,
                                                    ByRef Desc_Calibro As String,
                                                    ByRef Codice_Indice As Integer,
                                                    ByRef Desc_Indice As String,
                                                    ByRef Codice_Danno As Integer,
                                                    ByRef Desc_Danno As String,
                                                    ByRef Valore_Indice As String,
                                                    ByRef UdmCod_Indice As Integer,
                                                    ByVal Progressivo As Integer,
                                                    Optional ByVal Tipo As String = "",
                                                    Optional ByVal Tipo_Cod As Integer = 0,
                                                    Optional ByVal Udm_Cod As Integer = 0,
                                                    Optional ByVal Progressivo_Origine As Integer = 0,
                                                    Optional ByVal Piva_SuperUser_Origine As String = "",
                                                    Optional ByVal Flag_AncheImportati As Boolean = False,
                                                    Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                                    Optional ByVal FiltroAggiuntivo As String = "",
                                                    Optional ByVal Ordinamento As String = "") As String

        Dim Campionatura_Des As String = ""
        Dim Dt As DataTable
        Dim i As Integer

        Dt = MateriePrime_Campionature_Leggi(objParametri,
                                                    Progressivo,
                                                    Tipo,
                                                    Tipo_Cod,
                                                    Udm_Cod,
                                                    "",
                                                    "",
                                                    Progressivo_Origine,
                                                    Piva_SuperUser_Origine,
                                                    True,
                                                    ,
                                                    ,
                                                    ,
                                                    )

        If Not IsNothing(Dt) Then

            For i = 0 To Dt.Rows.Count - 1

                Select Case CStr(Dt.Rows(i).Item("tipo")).ToLower

                    Case "indice"
                        Codice_Indice = Dt.Rows(i).Item("tipo_cod")
                        Valore_Indice = Dt.Rows(i).Item("val_cod")
                        UdmCod_Indice = Dt.Rows(i).Item("udm_cod")
                        Desc_Indice = Dt.Rows(i).Item("Descrizione")
                        Campionatura_Des &= If(Trim(Campionatura_Des) = "", "", ", ") & CStr(Dt.Rows(i).Item("Descrizione"))

                    Case "calibro"
                        Codice_Calibro = Dt.Rows(i).Item("tipo_cod")
                        Desc_Calibro = Dt.Rows(i).Item("Descrizione")
                        Campionatura_Des &= If(Trim(Campionatura_Des) = "", "", ", ") & CStr(Dt.Rows(i).Item("Descrizione"))

                    Case "danno"
                        Codice_Danno = Dt.Rows(i).Item("tipo_cod")
                        Desc_Danno = Dt.Rows(i).Item("Descrizione")
                        Campionatura_Des &= If(Trim(Campionatura_Des) = "", "", ", ") & CStr(Dt.Rows(i).Item("Descrizione"))

                    Case "analisi"
                        'non devo visualizzare l'analisi nella descrizione

                End Select

            Next

        End If


        Return Campionatura_Des


    End Function

    Public Function MateriePrime_Campionature_Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByVal Progressivo As Integer = 0,
                                                        Optional ByVal Tipo As String = "",
                                                        Optional ByVal Tipo_Cod As Integer = 0,
                                                        Optional ByVal Udm_Cod As Integer = 0,
                                                        Optional ByVal Val_Cod As String = "",
                                                        Optional ByVal Descrizione As String = "",
                                                        Optional ByVal Progressivo_Origine As Integer = 0,
                                                        Optional ByVal Piva_SuperUser_Origine As String = "",
                                                        Optional ByVal Flag_AncheImportati As Boolean = False,
                                                        Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
                                                        Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
                                                        Optional ByVal FiltroAggiuntivo As String = "",
                                                        Optional ByVal Ordinamento As String = "") As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "MateriePrime_Campionature_Leggi"

        Dim StrSQL As String
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try


            StrSQL = ""
            StrSQL &= " SELECT * "
            StrSQL &= " FROM    Materie_Prime_Campionature "
            StrSQL &= " WHERE   Validita_Inizio <= " & SQL_SaveDate(FinestraTemp_Fine) & " "
            StrSQL &= " AND     Validita_Fine >= " & SQL_SaveDate(FinestraTemp_Inizio) & " "

            'Progressivo, Tipo, Tipo_Cod, Udm_Cod, Val_Cod, Descrizione, Progressivo_Origine, Piva_SuperUser_Origine

            If Progressivo <> 0 Then
                StrSQL &= " AND     (Progressivo = " & SQL_SaveNum(Progressivo) & ")   "
            End If

            If Tipo <> "" Then
                StrSQL &= " AND   (Tipo = '" & SQL_SaveText(Tipo) & "') "
            End If

            If Tipo_Cod <> 0 Then
                StrSQL &= " AND     (Tipo_Cod = " & SQL_SaveNum(Tipo_Cod) & ")   "
            End If

            If Udm_Cod <> 0 Then
                StrSQL &= " AND     (Udm_Cod = " & SQL_SaveNum(Udm_Cod) & ")   "
            End If

            If Val_Cod <> "" Then
                StrSQL &= " AND   (Val_Cod = '" & SQL_SaveText(Val_Cod) & "') "
            End If

            If Descrizione <> "" Then
                StrSQL &= " AND   (Descrizione = '" & SQL_SaveText(Descrizione) & "') "
            End If

            'gias2gias
            If Progressivo_Origine <> 0 Then
                StrSQL &= " AND (Progressivo_Origine = " & SQL_SaveNum(Progressivo_Origine) & ")   "
            End If
            If Piva_SuperUser_Origine <> "" Then
                StrSQL &= " AND (Piva_SuperUser_Origine = '" & SQL_SaveText(Piva_SuperUser_Origine) & "') "
            End If
            If Not Flag_AncheImportati Then
                StrSQL &= " AND (Progressivo_Origine = " & SQL_SaveNum(0) & ")"
            End If
            'fine gias2gias


            If Ordinamento = "" Then
                StrSQL &= " ORDER BY Descrizione "
            Else
                StrSQL &= Ordinamento
            End If

            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            DT = dp.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '#############################################################################################
    'copiata da giasonline
    Public Function CalibriFrutti_Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Cal_Cod As Integer = 0,
                                                Optional ByVal TestoRicerca As String = "",
                                                Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                                Optional ByVal FinestraTemp_Fine As String = "31/12/2100",
                                                Optional ByVal Ordinamento As String = "") As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "CalibriFrutti_Leggi"

        Dim StrSQL As String
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim i As Integer = 0

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try

            StrSQL = ""
            StrSQL &= " SELECT * "
            StrSQL &= " FROM    CalibriFrutti "

            If Cal_Cod <> 0 Then
                If i = 0 Then
                    StrSQL &= " WHERE     (Cal_Cod = " & SQL_SaveNum(Cal_Cod) & ")   "
                    i = i + 1
                Else
                    StrSQL &= " AND     (Cal_Cod = " & SQL_SaveNum(Cal_Cod) & ")   "
                End If
            End If

            If TestoRicerca <> "" Then
                If i = 0 Then
                    StrSQL &= " WHERE   (CAL_DES LIKE '%" & TestoRicerca & "%') "
                    i = i + 1
                Else
                    StrSQL &= " AND   (CAL_DES LIKE '%" & TestoRicerca & "%') "
                End If
            End If

            If Ordinamento = "" Then
                StrSQL &= " ORDER BY CAL_DES "
            Else
                StrSQL &= Ordinamento
            End If


            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            DT = dp.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function SQL_SaveNum(ByVal Testo As String) As String

        'Funzione per formattare i numeri reali prima di essere
        'inserite in una query SQL per SQL Server

        Dim Stringa As String

        'Preparo il formato opportuno
        Stringa = Replace(Testo, ".", "")
        Stringa = Replace(Stringa, ",", ".")

        'Restituisco il risultato
        Return Stringa

    End Function

    Public Function SQL_SaveText(ByVal Testo As String) As String

        'Funzione per formattare le stringhe prima di essere
        'inserite in una query SQL

        Dim Stringa As String

        'Trasformo il singolo apice in due singoli apici 
        Stringa = Replace(Testo, "'", "''")

        'Restituisco il risultato
        Return Stringa

    End Function

    Public Function SQL_SaveDate(ByVal DataItaliana As Date) As String

        'Funzione per formattare le date prima di essere
        'inserite in una query SQL per SQL Server

        Dim Stringa As String

        'Preparo il formato opportuno
        Stringa = "CONVERT(DateTime,'" & Format(DataItaliana, "yyyy/MM/dd") & "',120)"

        'Restituisco il risultato
        Return Stringa

    End Function

    Public Function ParcoMacchine_Leggi(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Piva As String = "",
                                Optional ByVal Mac_Cod As Integer = 0,
                                Optional ByVal FlagPubblico As Boolean = False,
                                Optional ByVal Class_Code As String = "",
                                Optional ByVal Targa As String = "",
                                Optional ByVal Telaio As String = "",
                                Optional ByVal Modello As String = "",
                                Optional ByVal Potenza As String = "",
                                Optional ByVal Ditta_Cod As Integer = 0,
                                Optional ByVal FiltroAggiuntivo As String = "",
                                Optional ByVal Ordinamento As String = "",
                                Optional ByVal Flag_Costi As Boolean = False
                               ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "ParcoMacchine_Leggi"

        Dim StrSQL As String
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try


            StrSQL = "   "
            StrSQL &= " SELECT  Imprese.Rag_Soc AS Impresa,    Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Piva, Parco_Macchine.Sa_Cod, Parco_Macchine.Mac_Cod,   "
            StrSQL &= "  Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Modello, Parco_Macchine.Telaio, Parco_Macchine.Potenza, Parco_Macchine.Tipo,  "
            StrSQL &= "  Parco_Macchine.Costo_Acquisto, Parco_Macchine.Ammortamento, Parco_Macchine.Ammortizzato,  "
            StrSQL &= "  Parco_Macchine.Data_Immatricolazione, Parco_Macchine.Ultima_Manutenzione, Parco_Macchine.Ultima_Revisione, Parco_Macchine.Stato_Utilizzo,   "
            StrSQL &= "  Parco_Macchine.Note, UtentiXImprese.[USER], Parco_Macchine.Ditta_Cod, ISNULL(Ditte.Ditta_Des, '') AS Ditta_Des, "
            StrSQL &= "  Parco_Macchine.validita_inizio AS Data_Inizio_Utilizzo, Parco_Macchine.validita_fine AS Data_Dismissione,  "

            StrSQL &= " N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, Data_Rilascio_Autorizzazione, Peso "

            If Flag_Costi Then
                StrSQL &= " , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  "
                StrSQL &= "  ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  "
            End If

            StrSQL &= " FROM Parco_Macchine INNER JOIN   "
            StrSQL &= " UtentiXImprese ON Parco_Macchine.Piva = UtentiXImprese.PIVA INNER JOIN  "
            StrSQL &= " Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE  "
            StrSQL &= " LEFT OUTER JOIN  Ditte ON Parco_Macchine.Ditta_Cod = Ditte.Ditta_Cod "

            'JOIN IMPRESE 
            StrSQL &= " INNER JOIN Imprese ON Imprese.Piva = Parco_Macchine.Piva "

            If Flag_Costi Then
                StrSQL &= " LEFT OUTER JOIN Prodotti_Costi ON Parco_Macchine.Mac_Cod = Prodotti_Costi.Mat_Cod  "
            End If

            StrSQL &= "   "
            StrSQL &= "   "
            StrSQL &= "   "
            StrSQL &= "   "

            StrSQL &= " WHERE UtentiXImprese.[USER] = '" & SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & "' "

            If Not FlagPubblico Then
                If Piva <> "" Then
                    StrSQL &= " AND    (Parco_Macchine.Piva = '" & SQL_SaveText(Piva) & "')   "
                End If
                If Mac_Cod <> 0 Then
                    StrSQL &= " AND    (Parco_Macchine.Mac_Cod = " & SQL_SaveNum(Mac_Cod) & ")   "
                End If
            Else
                If Piva <> "" Then
                    StrSQL &= " AND    (Parco_Macchine.Piva = '" & SQL_SaveText(Piva) & "' OR Parco_Macchine.Sa_Cod = -1 )  "
                End If
            End If

            If Class_Code <> "" Then
                StrSQL &= " AND    (Parco_Macchine.Class_Code = '" & SQL_SaveText(Class_Code) & "')   "
            End If

            If Targa <> "" Then
                StrSQL &= " AND    (Parco_Macchine.Targa = '" & SQL_SaveText(Targa) & "')   "
            End If

            If Modello <> "" Then
                StrSQL &= " AND    (Parco_Macchine.Modello = '" & SQL_SaveText(Modello) & "')   "
            End If

            If Telaio <> "" Then
                StrSQL &= " AND    (Parco_Macchine.Telaio = '" & SQL_SaveText(Telaio) & "')   "
            End If

            If Potenza <> "" Then
                StrSQL &= " AND    (Parco_Macchine.Potenza = '" & SQL_SaveText(Potenza) & "')   "
            End If

            If Ditta_Cod <> 0 Then
                StrSQL &= " AND    (Parco_Macchine.Ditta_Cod = " & SQL_SaveNum(Ditta_Cod) & ")   "
            End If

            StrSQL &= "   "
            StrSQL &= "   "
            StrSQL &= "   "
            StrSQL &= "   "

            If FiltroAggiuntivo <> "" Then
                StrSQL &= FiltroAggiuntivo
            End If

            If Ordinamento = "" Then
                StrSQL &= " ORDER BY Parco_Macchine.Mac_Des  "
            Else
                StrSQL &= Ordinamento
            End If

            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            DT = dp.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '###############################################################################
    Public Function SchedaTracciabilitaVegetale_SezioneAppezzamenti_LeggiDS(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                                ByVal Piva As String,
                                                                                ByVal ElencoChiaviImpianto As String,
                                                                                ByVal Validita_Inizio As Date,
                                                                                ByVal Validita_Fine As Date,
                                                                                Optional ByVal FiltroAggiuntivo As String = "",
                                                                                Optional ByVal Ordinamento As String = "") As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "SchedaTracciabilitaVegetale_SezioneAppezzamenti_LeggiDS"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Reg_Impianti.PIVA, Reg_Impianti.SA_COD, " & vbCrLf)
            StrSQL.Append(" Reg_Impianti.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.ID_REG, " & vbCrLf)
            StrSQL.Append(" Reg_Impianti.CUL_COD, Cultivar.Cul_Des, SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, " & vbCrLf)
            StrSQL.Append(" ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS Grva_Cod, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, " & vbCrLf)
            StrSQL.Append(" Appezzamento.SUP_APP, Reg_Impianti.REGOLAMENTO, " & vbCrLf)

            StrSQL.Append(" ISNULL( (SELECT TOP 1 Appezzamento_Codici.val_cod  " & vbCrLf)
            StrSQL.Append(" FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append(" WHERE id_cod = " & SQL_SaveNum(CStr(enum_CodiciAnagrafe.MetodoDiProduzione)) & " " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.PIVA = Appezzamento.PIVA  " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.SA_COD =  Appezzamento.SA_COD  " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.APPEZZA = Appezzamento.APPEZZA " & vbCrLf)
            StrSQL.Append(" ),'1') AS MetodoProduzione_Appezzamento, " & vbCrLf)

            StrSQL.Append(" ISNULL( (SELECT TOP 1 Appezzamento_Codici.val_cod  " & vbCrLf)
            StrSQL.Append(" FROM Appezzamento_Codici " & vbCrLf)
            StrSQL.Append(" WHERE id_cod = " & SQL_SaveNum(CStr(enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)) & " " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.PIVA = Appezzamento.PIVA " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.sa_cod = Appezzamento.sa_cod " & vbCrLf)
            StrSQL.Append(" AND Appezzamento_Codici.appezza = Appezzamento.appezza " & vbCrLf)
            StrSQL.Append(" ), '') AS App_Nome_Breve, " & vbCrLf)

            StrSQL.Append(" ISNULL(AppezzamentiXParticelle.PROV,'') AS PROV, ISNULL(AppezzamentiXParticelle.COM,'') AS COM, ISNULL(AppezzamentiXParticelle.SEZIONE,'') AS SEZIONE, " & vbCrLf)
            StrSQL.Append(" ISNULL(AppezzamentiXParticelle.FOGLIO,'') AS FOGLIO, ISNULL(AppezzamentiXParticelle.NUMERO,'') AS NUMERO, ISNULL(AppezzamentiXParticelle.SUBALTERNO, '') AS SUBALTERNO, " & vbCrLf)
            StrSQL.Append(" ISNULL(AppezzamentiXParticelle.AREA, '') as AREA, Reg_Impianti.Sup_Imp, Imprese_Progetti.Progetto_Nome AS Progetto, " & vbCrLf)
            StrSQL.Append(" Appezzamento.Campo_Cod, ISNULL(Campi.Campo_Des, '') AS campo_des " & vbCrLf)

            StrSQL.Append(" FROM Reg_Impianti  " & vbCrLf)
            StrSQL.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Appezzamento ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StrSQL.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA  " & vbCrLf)
            StrSQL.Append(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod AND " & vbCrLf)
            StrSQL.Append(" Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza And Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Campi ON Appezzamento.Campo_Cod = Campi.Campo_Cod AND Appezzamento.PIVA = Campi.Piva AND " & vbCrLf)
            StrSQL.Append(" Appezzamento.SA_COD = Campi.Sa_Cod " & vbCrLf)
            'sSql.Append(" LEFT OUTER JOIN Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND " & vbCrLf)
            'sSql.Append(" Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND " & vbCrLf)
            'sSql.Append(" Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN AppezzamentiXParticelle ON Appezzamento.PIVA = AppezzamentiXParticelle.PIVA AND " & vbCrLf)
            StrSQL.Append(" Appezzamento.SA_COD = AppezzamentiXParticelle.SA_COD AND " & vbCrLf)
            StrSQL.Append(" Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA " & vbCrLf)

            StrSQL.Append(" WHERE Reg_Impianti.PIVA = '" & SQL_SaveText(Piva) & "'" & vbCrLf)

            StrSQL.Append(" AND                (Reg_Impianti.Piva  " & vbCrLf)
            StrSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Sa_Cod)  " & vbCrLf)
            StrSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Appezza)  " & vbCrLf)
            StrSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Id_Reg)  " & vbCrLf)
            StrSQL.Append("                     IN (" & ElencoChiaviImpianto & "))  " & vbCrLf)

            'sSql.Append(strFiltroImpianti)

            StrSQL.Append(" AND Reg_Impianti.Validita_Inizio <= " & SQL_SaveDate(Validita_Fine) & vbCrLf)
            StrSQL.Append(" AND Reg_Impianti.Validita_Fine >= " & SQL_SaveDate(Validita_Inizio) & vbCrLf)
            StrSQL.Append(" AND Imprese_Progetti.Validita_Inizio <= " & SQL_SaveDate(Validita_Fine) & vbCrLf)
            StrSQL.Append(" AND Imprese_Progetti.Validita_Fine >= " & SQL_SaveDate(Validita_Inizio) & vbCrLf)

            StrSQL.Append(" ORDER BY App_Nome_Breve, App_Nome " & vbCrLf)


            '--------------------------------------------------------------------------
            Dim dp As New AgronicaCoreDataProvider.DataProvider
            dt = dp.EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function


End Class

Public Class Codex_Utility_Sql_DistinctOnDT
    Inherits Codex_Utility.Sql 'eredita in modo da avere a disposizione anche i metodi della classe padre...

    Public Sub New() ' costruttore non prende nulla in ingresso

    End Sub

    'controlla l'uguaglianza di 2 oggetti
    Private Function ColonneUguali(ByVal A As Object, ByVal B As Object) As Boolean

        'confronta 2 valori x vedere se sono uguali, confronta anche il dbnull 
        If IsDBNull(A) AndAlso IsDBNull(B) Then
            Return True 'entrambi db null
        End If

        If IsDBNull(A) OrElse IsDBNull(B) Then
            Return False 'solo uno è db null
        End If

        Return A.Equals(B) 'confronta i 2 oggetti e ritorna un booleano x indicare se sono uguali o meno..

    End Function


    Public Function CopiaDatiRiga(ByVal drNuova As DataRow, ByVal drOrigine As DataRow) As DataRow
        'ciclo su tutti le colonne
        Dim Dc As DataColumn
        For Each Dc In drOrigine.Table.Columns
            'copio i dati
            drNuova.Item(Dc.ColumnName) = drOrigine.Item(Dc)
        Next
        Return drNuova
    End Function


    'ritorna un datatable con un'unica colonna distinta...
    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable,
                ByVal FieldName As String, Optional ByVal blnRitornaSingolaColonna As Boolean = True) As DataTable

        Dim dt As New DataTable(TableName) 'creo la nuova tabella da ritornare con nome impostato dal parametro
        'controllo se devo ritornare solo un campo.....
        If blnRitornaSingolaColonna Then
            'aggiungo il nuovo campo x il distinct prendendo il tipo corrispondente..
            dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)
        Else 'in questo caso li devo ritornare tutti clono la struttura della tabella
            dt = SourceTable.Clone()
        End If

        Dim LastValue As Object
        Dim dr As DataRow

        'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
        For Each dr In SourceTable.Select("", FieldName)
            'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
            If LastValue = Nothing Then
                'assegno l'ultimo valore...
                LastValue = dr(FieldName)
                'anche qui se devo ritornare un campo solo aggiungo solo quello..
                If blnRitornaSingolaColonna Then
                    'aggiungo la nuova riga....
                    dt.Rows.Add(New Object() {LastValue})
                Else 'aggiungo tutta la riga..
                    Dim drN As DataRow = dt.NewRow
                    dt.Rows.Add(CopiaDatiRiga(drN, dr))
                End If
            Else
                If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'anche qui se devo ritornare un campo solo aggiungo solo quello..
                    If blnRitornaSingolaColonna Then
                        'aggiungo la nuova riga....
                        dt.Rows.Add(New Object() {LastValue})
                    Else 'aggiungo tutta la riga..
                        Dim drN As DataRow = dt.NewRow
                        dt.Rows.Add(CopiaDatiRiga(drN, dr))
                    End If
                End If
            End If

        Next

        Return dt 'ritorno altrimenti solo la tabella

    End Function

    'ritorna un array di stringhe...
    Public Function SelectDistinct(ByVal SourceTable As DataTable, ByVal FieldName As String, Optional ByVal Ordinato As Boolean = True) As String()

        'array da ritornare...
        Dim strRet() As String
        Dim LastValue As Object
        Dim dr As DataRow
        'indice x l'array...
        Dim i As Integer = 0

        If Ordinato Then

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select("", FieldName)

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If LastValue = Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo....
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo....
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        Else

            'ciclo filtro vuoto ordinando x il nome campo richiesto x il distinct
            For Each dr In SourceTable.Select()

                'se l'ultimo valori è nothing prendo il valore....(succede solo la prima volta)
                If LastValue = Nothing Then
                    'assegno l'ultimo valore...
                    LastValue = dr(FieldName)
                    'aggiungo la nuova riga....
                    ReDim Preserve strRet(i)
                    strRet(i) = LastValue.ToString 'assegno il valore del campo....
                    'incremento l'indice...
                    i += 1
                Else

                    If Not ColonneUguali(LastValue, dr(FieldName)) Then 'le volte successive controllo se sono uguali
                        'assegno l'ultimo valore...
                        LastValue = dr(FieldName)
                        'aggiungo la nuova riga....
                        ReDim Preserve strRet(i)
                        strRet(i) = LastValue.ToString 'assegno il valore del campo....
                        'incremento l'indice...
                        i += 1
                    End If

                End If

            Next

        End If

        Return strRet 'ritorno altrimenti solo la tabella

    End Function


    '=========================================================================================================
    '----------------------------- SELECT DISTINCT SU DATATABLE BY METAL MAGA --------------------------------
    '==========================================0==============================================================

    '#######################################################################################################
    Public Function SelectDistinctWithHash_Base2(ByRef DT As DataTable, ByVal vet_campi_chiave() As String, Optional ByVal vet_campi_valore() As String = Nothing) As Hashtable

        ' prende in input un datatable DT passato byref (su cui viene fatto il distinct)
        ' e un vettore di stringhe vet_campi_chiave() contenenti i nomi delle colonne su cui fare il distinct 
        '(usati quindi per costruire la chiave)

        'la chiave può essere formata da un unico elemento (equivale al distinct su un campo)
        'se è necessario fare il distinct su più campi, si crea la chiave separando i campi da "|"

        'se viene passato il vettore vet_campi_valore() viene costruito il valore da attribuire alla chiave
        '(la costruzione avviene come per la chiave,separando i campi da "~")

        'restituisce una tabella hash con il distinct
        '(sul datatable c'è il distinct)

        'a differenza della SelectDistinctWithHash_Base, se la chiave è già presente, non viene fatto nulla

        Dim i, j As Integer
        Dim DT_Finale As DataTable
        Dim Dr_finale As DataRow
        Dim table_hash As New Hashtable

        Dim chiave As String = ""
        Dim valore As String = ""

        'copio la struttura
        DT_Finale = DT.Clone

        'scorro il DT di input
        For i = 0 To DT.Rows.Count - 1

            chiave = ""
            valore = ""

            'creo la stringa CHIAVE
            ''esempio
            'chiave = CStr(DT.Rows(i).Item("piva")) & "|" & CStr(DT.Rows(i).Item("sa_cod")) & "|" & CStr(DT.Rows(i).Item("appezza")) & "|" & CStr(DT.Rows(i).Item("id_reg"))
            For j = 0 To vet_campi_chiave.Length - 1
                If j = 0 Then
                    chiave = CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(0))))
                Else
                    chiave &= "|" & CStr(DT.Rows(i).Item(CStr(vet_campi_chiave(j))))
                End If
            Next


            'creo la stringa VALORE
            If vet_campi_valore IsNot Nothing Then
                For j = 0 To vet_campi_valore.Length - 1
                    If j = 0 Then
                        valore = CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    Else
                        'separo i campi del valore con ~
                        'la pipe | la utilizzo per separare i valori (nel caso ce ne siano più di uno)
                        valore &= "~" & CStr(DT.Rows(i).Item(CStr(vet_campi_valore(j))))
                    End If
                Next
            End If


            Try

                If Not table_hash.ContainsKey(chiave) Then

                    table_hash.Add(chiave, valore)

                    'importo la riga nel DT finale
                    Dr_finale = DT_Finale.NewRow
                    Dr_finale = CopiaDatiRiga(Dr_finale, DT.Rows(i))
                    DT_Finale.Rows.Add(Dr_finale)

                End If


            Catch ae As ArgumentException

                'eccezione

            End Try


        Next

        DT = Nothing
        DT = DT_Finale.Copy

        Return table_hash



    End Function


End Class
