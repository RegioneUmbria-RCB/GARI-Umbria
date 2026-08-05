Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.My.Resources

Public Class BilancioFertilizzazioni_XLS
    Inherits System.Web.UI.Page

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim DtImpianti As New DataTable
        Dim DrImpianti As DataRow

        Dim strXmlVariabilistampe As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement

        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim i, j As Integer

        Dim N_Massimo As Decimal
        Dim P_Massimo As Decimal
        Dim K_Massimo As Decimal
        Dim Mg_Massimo As Decimal

        Dim N_Distribuito_Ha As Decimal
        Dim P_Distribuito_Ha As Decimal
        Dim K_Distribuito_Ha As Decimal
        Dim Mg_Distribuito_Ha As Decimal

        Dim N_Distribuito_Imp As Decimal
        Dim P_Distribuito_Imp As Decimal
        Dim K_Distribuito_Imp As Decimal
        Dim Mg_Distribuito_Imp As Decimal

        Dim N_Residuo As Decimal
        Dim P_Residuo As Decimal
        Dim K_Residuo As Decimal
        Dim Mg_Residuo As Decimal

        Dim N_Totale As Decimal
        Dim P_Totale As Decimal
        Dim K_Totale As Decimal
        Dim Mg_Totale As Decimal

        'Dim Sup_Imp As Double
        'Dim Sup_Tot As Double

        '##############################################################
        'La Pagina deve essere visualizzata come un foglio Excel
        '##############################################################

        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = BilancioFertilizzazioni.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '##############################################################
        '#####  Recupero gli Impianti scelti  #########################
        '##############################################################

        DtImpianti.Columns.Add(New DataColumn("piva", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("appezza", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("id_reg", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("veg_cod", GetType(String)))

        DtImpianti.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("app_nome", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("sup_imp", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("progetto_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("veg_des", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("cul_des", GetType(String)))

        DtImpianti.Columns.Add(New DataColumn("progetto_validita_inizio", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("progetto_validita_fine", GetType(String)))

        DtImpianti.Columns.Add(New DataColumn("impianto_validita_inizio", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("impianto_validita_fine", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("grfi_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("grfi_des", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("regolamento_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("reg_des", GetType(String)))

        DtImpianti.Columns.Add(New DataColumn("campo_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("campo_des", GetType(String)))

        DtImpianti.Columns.Add(New DataColumn("n_massimo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("n_distribuito", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("n_distribuito_totale", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("n_residuo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("p2o5_massimo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("p2o5_distribuito", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("p2o5_distribuito_totale", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("p2o5_residuo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("k2o_massimo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("k2o_distribuito", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("k2o_distribuito_totale", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("k2o_residuo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("mgo_massimo", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("mgo_distribuito", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("mgo_distribuito_totale", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("mgo_residuo", GetType(String)))


        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            N_Totale = 0
            P_Totale = 0
            K_Totale = 0
            Mg_Totale = 0
            'Sup_Tot = 0

         
            Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                DrImpianti = DtImpianti.NewRow

                Dim pivaReale As String = objImp.Leggi_PivaReale(XML_VariabiliStampe.GetAttribute("piva"), objParametri_Server)
                DrImpianti.Item("piva") = pivaReale
                DrImpianti.Item("sa_cod") = XML_VariabiliStampe.GetAttribute("sa_cod")
                DrImpianti.Item("appezza") = XML_VariabiliStampe.GetAttribute("appezza")
                DrImpianti.Item("id_reg") = XML_VariabiliStampe.GetAttribute("id_reg")
                DrImpianti.Item("veg_cod") = XML_VariabiliStampe.GetAttribute("veg_cod")

                DrImpianti.Item("rag_soc") = XML_VariabiliStampe.GetAttribute("rag_soc")
                DrImpianti.Item("sa_nome") = XML_VariabiliStampe.GetAttribute("sa_nome")

                DrImpianti.Item("app_nome") = XML_VariabiliStampe.GetAttribute("app_nome")
                DrImpianti.Item("sup_imp") = XML_VariabiliStampe.GetAttribute("sup_imp")
                DrImpianti.Item("progetto_cod") = XML_VariabiliStampe.GetAttribute("progetto_cod")
                DrImpianti.Item("veg_des") = XML_VariabiliStampe.GetAttribute("veg_des")
                DrImpianti.Item("cul_des") = XML_VariabiliStampe.GetAttribute("cul_des")

                DrImpianti.Item("progetto_validita_inizio") = XML_VariabiliStampe.GetAttribute("progetto_validita_inizio")
                DrImpianti.Item("progetto_validita_fine") = XML_VariabiliStampe.GetAttribute("progetto_validita_fine")

                DrImpianti.Item("impianto_validita_inizio") = XML_VariabiliStampe.GetAttribute("impianto_validita_inizio")
                DrImpianti.Item("impianto_validita_fine") = XML_VariabiliStampe.GetAttribute("impianto_validita_fine")

                DrImpianti.Item("grfi_cod") = XML_VariabiliStampe.GetAttribute("grfi_cod")
                DrImpianti.Item("grfi_des") = XML_VariabiliStampe.GetAttribute("grfi_des")

                DrImpianti.Item("regolamento_cod") = XML_VariabiliStampe.GetAttribute("regolamento_cod")
                DrImpianti.Item("reg_des") = XML_VariabiliStampe.GetAttribute("reg_des")

                DrImpianti.Item("campo_cod") = XML_VariabiliStampe.GetAttribute("campo_cod")
                DrImpianti.Item("campo_des") = XML_VariabiliStampe.GetAttribute("campo_des")

                '===========================================================================================
                'Lettura dei massimi apporti macroelementi
                '-------------------------------------------------------------------------------------------
                N_Massimo = 0
                P_Massimo = 0
                K_Massimo = 0
                Mg_Massimo = 0

                Reg_Impianti_Codici_R.Leggi_ApportiMassimiMacroelementi( _
                                                  CStr(DrImpianti.Item("piva")), _
                                                  CInt(DrImpianti.Item("sa_cod")), _
                                                  CInt(DrImpianti.Item("appezza")), _
                                                  CInt(DrImpianti.Item("id_reg")), _
                                                  CInt(DrImpianti.Item("progetto_cod")), _
                                                  N_Massimo, _
                                                  P_Massimo, _
                                                  K_Massimo, _
                                                  Mg_Massimo, _
                                                  objParametri_Server)


                If Not (N_Massimo = 0 And P_Massimo = 0 And K_Massimo = 0 And Mg_Massimo = 0) Then

                    DrImpianti.Item("n_massimo") = Format(N_Massimo, "0.000")
                    DrImpianti.Item("p2o5_massimo") = Format(P_Massimo, "0.000")
                    DrImpianti.Item("k2o_massimo") = Format(K_Massimo, "0.000")
                    DrImpianti.Item("mgo_massimo") = Format(Mg_Massimo, "0.000")

                Else

                    DrImpianti.Item("n_massimo") = Gias.NonDefinito
                    DrImpianti.Item("p2o5_massimo") = Gias.NonDefinito
                    DrImpianti.Item("k2o_massimo") = Gias.NonDefinito
                    DrImpianti.Item("mgo_massimo") = Gias.NonDefinito

                    DrImpianti.Item("n_residuo") = Gias.NonDefinito
                    DrImpianti.Item("p2o5_residuo") = Gias.NonDefinito
                    DrImpianti.Item("k2o_residuo") = Gias.NonDefinito
                    DrImpianti.Item("mgo_residuo") = Gias.NonDefinito

                End If


                '===============================================================================
                'CONTROLLO MASSIMO APPORTO MACROELEMENTI
                '-------------------------------------------------------------------------------

                N_Distribuito_Ha = 0
                P_Distribuito_Ha = 0
                K_Distribuito_Ha = 0
                Mg_Distribuito_Ha = 0

                N_Distribuito_Imp = 0
                P_Distribuito_Imp = 0
                K_Distribuito_Imp = 0
                Mg_Distribuito_Imp = 0

                'Sup_Imp = CDbl(DrImpianti.Item("sup_imp"))

                Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim DtFert As DataTable
                DtFert = objFert.Leggi_Macroelementi_Distribuiti(N_Distribuito_Ha, _
                                                            P_Distribuito_Ha, _
                                                            K_Distribuito_Ha, _
                                                            Mg_Distribuito_Ha, _
                                                            0, _
                                                            N_Distribuito_Imp, _
                                                            P_Distribuito_Imp, _
                                                            K_Distribuito_Imp, _
                                                            Mg_Distribuito_Imp, _
                                                            0, _
                                                            CStr(DrImpianti.Item("piva")), _
                                                            CInt(DrImpianti.Item("sa_cod")), _
                                                            CInt(DrImpianti.Item("appezza")), _
                                                            CInt(DrImpianti.Item("id_reg")), _
                                                            CInt(DrImpianti.Item("Progetto_Cod")), _
                                                            CDate(DrImpianti.Item("progetto_validita_inizio")), _
                                                            CDate(DrImpianti.Item("progetto_validita_fine")), _
                                                            CInt(0), _
                                                            objParametri_Server)
                objFert = Nothing
                DtFert = Nothing
    
                DrImpianti.Item("n_distribuito") = Format(N_Distribuito_Ha, "0.000")
                DrImpianti.Item("p2o5_distribuito") = Format(P_Distribuito_Ha, "0.000")
                DrImpianti.Item("k2o_distribuito") = Format(K_Distribuito_Ha, "0.000")
                DrImpianti.Item("mgo_distribuito") = Format(Mg_Distribuito_Ha, "0.000")

                DrImpianti.Item("n_distribuito_totale") = Format(N_Distribuito_Imp, "0.000")
                DrImpianti.Item("p2o5_distribuito_totale") = Format(P_Distribuito_Imp, "0.000")
                DrImpianti.Item("k2o_distribuito_totale") = Format(K_Distribuito_Imp, "0.000")
                DrImpianti.Item("mgo_distribuito_totale") = Format(Mg_Distribuito_Imp, "0.000")


                'DrImpianti.Item("n_distribuito_totale") = Format(N_Distribuito * Sup_Imp, "0.000")
                'DrImpianti.Item("p2o5_distribuito_totale") = Format(P_Distribuito * Sup_Imp, "0.000")
                'DrImpianti.Item("k2o_distribuito_totale") = Format(K_Distribuito * Sup_Imp, "0.000")
                'DrImpianti.Item("mgo_distribuito_totale") = Format(Mg_Distribuito * Sup_Imp, "0.000")

                'N_Totale += CDbl(N_Distribuito * Sup_Imp)
                'P_Totale += CDbl(P_Distribuito * Sup_Imp)
                'K_Totale += CDbl(K_Distribuito * Sup_Imp)
                'Mg_Totale += CDbl(Mg_Distribuito * Sup_Imp)

                N_Totale += CDbl(N_Distribuito_Imp)
                P_Totale += CDbl(P_Distribuito_Imp)
                K_Totale += CDbl(K_Distribuito_Imp)
                Mg_Totale += CDbl(Mg_Distribuito_Imp)

                'Sup_Tot += Sup_Imp

                If Not (N_Massimo = 0 And P_Massimo = 0 And K_Massimo = 0 And Mg_Massimo = 0) Then

                    N_Residuo = N_Massimo - N_Distribuito_Ha
                    P_Residuo = P_Massimo - P_Distribuito_Ha
                    K_Residuo = K_Massimo - K_Distribuito_Ha
                    Mg_Residuo = Mg_Massimo - Mg_Distribuito_Ha

                    DrImpianti.Item("n_residuo") = Format(N_Residuo, "0.000")
                    DrImpianti.Item("p2o5_residuo") = Format(P_Residuo, "0.000")
                    DrImpianti.Item("k2o_residuo") = Format(K_Residuo, "0.000")
                    DrImpianti.Item("mgo_residuo") = Format(Mg_Residuo, "0.000")

                End If

                DtImpianti.Rows.Add(DrImpianti)

            Next

        End If


        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim Riga As HtmlTableRow

        Me.TableBilancio.Rows(0).Cells(0).InnerHtml = " BILANCIO FERTILIZZAZIONI "
        Me.TableBilancio.Rows(0).Cells(0).ColSpan = 27

        'Creo la prima riga con l'intestazione
        Riga = New HtmlTableRow

        For i = 0 To 26
            Riga.Cells.Add(New HtmlTableCell)
            AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(i), 2, "", "", "Gainsboro", "center", "top")
        Next

        Riga.Cells(0).InnerHtml = "Partita<br>Iva"
        Riga.Cells(1).InnerHtml = "Ragione<br>Sociale"
        Riga.Cells(2).InnerHtml = "Centro<br>Aziendale"
        Riga.Cells(3).InnerHtml = "Campo"
        Riga.Cells(4).InnerHtml = "Appezzamento"
        Riga.Cells(5).InnerHtml = "Inizio Impianto"
        Riga.Cells(6).InnerHtml = "Specie<br>Vegetale"
        Riga.Cells(7).InnerHtml = "Cultivar"
        Riga.Cells(8).InnerHtml = "Finalita"
        Riga.Cells(9).InnerHtml = "Regolamento"
        Riga.Cells(10).InnerHtml = "Sup.<br>[Ha]"

        Riga.Cells(11).InnerHtml = "N [Kg/Ha]<br>Massimo"
        Riga.Cells(12).InnerHtml = "N [Kg] Totale<br>Distribuito"
        Riga.Cells(13).InnerHtml = "N [Kg/Ha]<br>Distribuito"
        Riga.Cells(14).InnerHtml = "N [Kg/Ha]<br>Residuo"

        Riga.Cells(15).InnerHtml = "P2O5 [Kg/Ha]<br>Massimo"
        Riga.Cells(16).InnerHtml = "P2O5 [Kg] Totale<br>Distribuito"
        Riga.Cells(17).InnerHtml = "P2O5 [Kg/Ha]<br>Distribuito"
        Riga.Cells(18).InnerHtml = "P2O5 [Kg/Ha]<br>Residuo"

        Riga.Cells(19).InnerHtml = "K2O [Kg/Ha]<br>Massimo"
        Riga.Cells(20).InnerHtml = "K2O [Kg] Totale<br>Distribuito"
        Riga.Cells(21).InnerHtml = "K2O [Kg/Ha]<br>Distribuito"
        Riga.Cells(22).InnerHtml = "K2O [Kg/Ha]<br>Residuo"

        Riga.Cells(23).InnerHtml = "MgO [Kg/Ha]<br>Massimo"
        Riga.Cells(24).InnerHtml = "MgO [Kg] Totale<br>Distribuito"
        Riga.Cells(25).InnerHtml = "MgO [Kg/Ha]<br>Distribuito"
        Riga.Cells(26).InnerHtml = "MgO [Kg/Ha]<br>Residuo"

        TableBilancio.Rows.Add(Riga)

        For i = 0 To DtImpianti.Rows.Count - 1

            Riga = New HtmlTableRow

            For j = 0 To 26

                Riga.Cells.Add(New HtmlTableCell)

                Select Case j

                    Case 0
                        'aggiungo alla piva lo spazio x salvare gli zeri..
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("piva")), "&nbsp;" & DtImpianti.Rows(i).Item("piva"), "&nbsp;")

                    Case 1
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("rag_soc")), DtImpianti.Rows(i).Item("rag_soc"), "&nbsp;")

                    Case 2
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("sa_nome")), DtImpianti.Rows(i).Item("sa_nome"), "&nbsp;")

                    Case 3
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("campo_des")), DtImpianti.Rows(i).Item("campo_des"), "&nbsp;")

                    Case 4
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("app_nome")), DtImpianti.Rows(i).Item("app_nome"), "&nbsp;")

                    Case 5
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("impianto_validita_inizio")), DtImpianti.Rows(i).Item("impianto_validita_inizio"), "&nbsp;")

                    Case 6
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("veg_des")), DtImpianti.Rows(i).Item("veg_des"), "&nbsp;")

                    Case 7
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("cul_des")), DtImpianti.Rows(i).Item("cul_des"), "&nbsp;")

                    Case 8
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("grfi_des")), DtImpianti.Rows(i).Item("grfi_des"), "&nbsp;")

                    Case 9
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("reg_des")), DtImpianti.Rows(i).Item("reg_des"), "&nbsp;")


                    Case 10
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("sup_imp")), DtImpianti.Rows(i).Item("sup_imp"), "&nbsp;")
                        If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                            'Sup_Imp = CDbl(Riga.Cells(j).InnerHtml)
                            Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.0000")
                        Else
                            'Sup_Imp = 0
                        End If

                    Case 11
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("n_massimo")), DtImpianti.Rows(i).Item("n_massimo"), "&nbsp;")

                    Case 12
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("n_distribuito_totale")), DtImpianti.Rows(i).Item("n_distribuito_totale"), "&nbsp;")

                    Case 13
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("n_distribuito")), DtImpianti.Rows(i).Item("n_distribuito"), "&nbsp;")

                    Case 14
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("n_residuo")), DtImpianti.Rows(i).Item("n_residuo"), "&nbsp;")

                    Case 15
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("p2o5_massimo")), DtImpianti.Rows(i).Item("p2o5_massimo"), "&nbsp;")

                    Case 16
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("p2o5_distribuito_totale")), DtImpianti.Rows(i).Item("p2o5_distribuito_totale"), "&nbsp;")

                    Case 17
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("p2o5_distribuito")), DtImpianti.Rows(i).Item("p2o5_distribuito"), "&nbsp;")

                    Case 18
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("p2o5_residuo")), DtImpianti.Rows(i).Item("p2o5_residuo"), "&nbsp;")

                    Case 19
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("k2o_massimo")), DtImpianti.Rows(i).Item("k2o_massimo"), "&nbsp;")

                    Case 20
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("k2o_distribuito_totale")), DtImpianti.Rows(i).Item("k2o_distribuito_totale"), "&nbsp;")

                    Case 21
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("k2o_distribuito")), DtImpianti.Rows(i).Item("k2o_distribuito"), "&nbsp;")

                    Case 22
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("k2o_residuo")), DtImpianti.Rows(i).Item("k2o_residuo"), "&nbsp;")

                    Case 23
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("mgo_massimo")), DtImpianti.Rows(i).Item("mgo_massimo"), "&nbsp;")

                    Case 24
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("mgo_distribuito_totale")), DtImpianti.Rows(i).Item("mgo_distribuito_totale"), "&nbsp;")

                    Case 25
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("mgo_distribuito")), DtImpianti.Rows(i).Item("mgo_distribuito"), "&nbsp;")

                    Case 26
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("mgo_residuo")), DtImpianti.Rows(i).Item("mgo_residuo"), "&nbsp;")


                    Case Else
                        'Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item(j)), Dv.Item(i).Item(j), "&nbsp;")

                End Select

            Next

            Me.TableBilancio.Rows.Add(Riga)

        Next


        'Aggiungo la riga totali
        Riga = New HtmlTableRow

        For i = 0 To 26
            Riga.Cells.Add(New HtmlTableCell)
        Next

        Riga.Cells(9).InnerHtml = "TOTALI"
        Riga.Cells(10).InnerHtml = "" 'Sup_Tot

        Riga.Cells(12).InnerHtml = Format(N_Totale, "0.000")
        Riga.Cells(16).InnerHtml = Format(P_Totale, "0.000")
        Riga.Cells(20).InnerHtml = Format(K_Totale, "0.000")
        Riga.Cells(24).InnerHtml = Format(Mg_Totale, "0.000")

        TableBilancio.Rows.Add(Riga)

    End Sub

End Class