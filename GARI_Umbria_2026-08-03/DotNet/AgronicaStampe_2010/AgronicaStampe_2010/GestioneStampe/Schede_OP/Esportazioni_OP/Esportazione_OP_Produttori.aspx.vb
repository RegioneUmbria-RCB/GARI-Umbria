Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Esportazione_OP_Produttori
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private Qs_Data As String
    Private Qs_Cod As String
    Private Qs_Reg As String
    Private Qs_Cuaa As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Qs_Data = Stringa_Decodifica(Request.QueryString("data").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Qs_Cod = Stringa_Decodifica(Request.QueryString("cod").ToString, _
                                 AgroKey_EncoderDecoder, _
                                 Server)
        Qs_Reg = Stringa_Decodifica(Request.QueryString("reg").ToString, _
                                 AgroKey_EncoderDecoder, _
                                 Server)
        Qs_Cuaa = Stringa_Decodifica(Request.QueryString("cuaa").ToString, _
                                 AgroKey_EncoderDecoder, _
                                 Server)


        Dim Dt As New DataTable
        Dim Dt_Terreni_Fuori_Reg As New DataTable
        Dim Dt_Terreni_Dentro_Reg As New DataTable
        Dim Dt_Prodotti As New DataTable
        Dim Dt_RapprLeg As New DataTable

        Dim strPive As String
        Dim i, r As Integer
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim strXmlVariabilistampe As String
        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Dim Dt_Esport As New DataTable
        Dim Dr_Export As DataRow

        Dt_Esport.Columns.Add(New DataColumn("codice_unione", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("codice_regione", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("cuaa_op", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("cuaa", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("legale", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("data_nascita", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_prov_nascita", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_com_nascita", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("prov_nascita", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("com_nascita", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("sesso", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("indirizzo", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("cap", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_prov", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_com", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("prov", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("com", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("telefono", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("data_iscrizione", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("terreni_fuori_regione", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_reg_fuori1", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_reg_fuori2", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("istat_reg_fuori3", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("socio_diretto", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("cuaa1", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("cuaa2", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("cuaa3", GetType(String)))

        Dt_Esport.Columns.Add(New DataColumn("tipo_socio", GetType(String)))
        Dt_Esport.Columns.Add(New DataColumn("data_revoca", GetType(String)))

        For i = 1 To 20
            Dt_Esport.Columns.Add(New DataColumn("id_prodotto" & i, GetType(String)))
            Dt_Esport.Columns.Add(New DataColumn("des_prodotto" & i, GetType(String)))
            Dt_Esport.Columns.Add(New DataColumn("tipo_prodotto" & i, GetType(String)))
            Dt_Esport.Columns.Add(New DataColumn("qta_prodotto" & i, GetType(String)))
        Next

        Dim Piva As String
        Dim N_Prod As Integer = 1

        'carico i dati
        If XmlDoc.HasChildNodes Then

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                Piva = XML_VariabiliStampe.GetAttribute("piva")

                strPive &= "'" & XML_VariabiliStampe.GetAttribute("piva") & "',"

            Next

            If strPive <> "" Then

                strPive = "(" & Left(strPive, strPive.Length - 1) & ")"

                Dt_Terreni_Fuori_Reg = Terreni_Fuori_Regione(strPive, Qs_Reg)
                Dt_Terreni_Dentro_Reg = Terreni_Dentro_Regione(strPive, Qs_Reg)

                Dt_Prodotti = Leggi_Prodotti(strPive, Qs_Data)
                Dim objRL As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dt_RapprLeg = objRL.Contatti_Contatto_Leggi("", "", 0, -1, True, True, enum_IndirizzoTipo.LuogoNascita, 0, True, 0, enum_Contatti_IdCf.PersonaFisica, 0, "", True, 0, 0, 0, 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                            " Risorse_Umane.Piva IN " & strPive, "", objParametri_Server)

                Dt = Leggi_Produttori(strPive)

                For i = 0 To Dt.Rows.Count - 1

                    Dr_Export = Dt_Esport.NewRow

                    Dr_Export.Item("codice_unione") = Qs_Cod
                    Dr_Export.Item("codice_regione") = Qs_Reg
                    Dr_Export.Item("cuaa_op") = Qs_Cuaa

                    Dr_Export.Item("cuaa") = Dt.Rows(i).Item("cuaa")
                    Dr_Export.Item("piva") = Dt.Rows(i).Item("piva")
                    Dr_Export.Item("rag_soc") = Dt.Rows(i).Item("rag_soc")
                    If Dt.Rows(i).Item("validita_inizio") <> AGRODATAINIZIO Then
                        Dr_Export.Item("validita_inizio") = CDate(Dt.Rows(i).Item("validita_inizio")).ToShortDateString
                    End If

                    Dim Dr_LR() As DataRow
                    If Not Dt_RapprLeg Is Nothing Then
                        Dr_LR = Dt_RapprLeg.Select("piva='" & Dt.Rows(i).Item("piva") & "'")
                        If Not Dr_LR Is Nothing AndAlso Dr_LR.Length > 0 Then
                            Dr_Export.Item("legale") = Dr_LR(0).Item("Rag_Soc") & Dr_LR(0).Item("Cognome") & " " & Dr_LR(0).Item("Nome")
                            If Dr_LR(0).Item("data_nascita") <> AGRODATAINIZIO Then
                                Dr_Export.Item("data_nascita") = CDate(Dr_LR(0).Item("data_nascita")).ToShortDateString
                            Else
                                Dr_Export.Item("data_nascita") = " "
                            End If
                            Dr_Export.Item("sesso") = Dr_LR(0).Item("Sesso")
                            If Dr_LR(0).Item("pro_cod_istat") <> "000" Then
                                Dr_Export.Item("istat_prov_nascita") = Dr_LR(0).Item("pro_cod_istat")
                            Else
                                Dr_Export.Item("istat_prov_nascita") = " "
                            End If
                            If Dr_LR(0).Item("pro_cod") <> "" Then
                                Dr_Export.Item("prov_nascita") = Dr_LR(0).Item("pro_cod")
                            Else
                                Dr_Export.Item("prov_nascita") = " "
                            End If
                            If Dr_LR(0).Item("com_cod_istat") <> "000" Then
                                Dr_Export.Item("istat_com_nascita") = Dr_LR(0).Item("com_cod_istat")
                            Else
                                Dr_Export.Item("istat_com_nascita") = " "
                            End If
                            If Dr_LR(0).Item("com_des") <> "" Then
                                Dr_Export.Item("com_nascita") = Dr_LR(0).Item("com_des")
                            Else
                                Dr_Export.Item("com_nascita") = " "
                            End If
                            Dr_Export.Item("telefono") = Dr_LR(0).Item("numero")
                        End If
                    End If

                    Dr_Export.Item("istat_prov") = Dt.Rows(i).Item("pro_cod_istat")
                    Dr_Export.Item("istat_com") = Dt.Rows(i).Item("com_cod_istat")

                    Dr_Export.Item("prov") = Dt.Rows(i).Item("pro_cod")
                    Dr_Export.Item("com") = Dt.Rows(i).Item("com_des")

                    Dr_Export.Item("indirizzo") = Dt.Rows(i).Item("ind_des")
                    Dr_Export.Item("cap") = Dt.Rows(i).Item("cap")

                    If Dt.Rows(i).Item("data_libro_soci") <> AGRODATAINIZIO Then
                        Dr_Export.Item("data_iscrizione") = CDate(Dt.Rows(i).Item("data_libro_soci")).ToShortDateString
                    Else
                        Dr_Export.Item("data_iscrizione") = " "
                    End If

                    Dr_Export.Item("terreni_fuori_regione") = " "
                    Dr_Export.Item("istat_reg_fuori1") = " "
                    Dr_Export.Item("istat_reg_fuori2") = " "
                    Dr_Export.Item("istat_reg_fuori3") = " "

                    'verifico se HA terreni FUORI
                    Dim Dr_TFReg() As DataRow
                    If Not Dt_Terreni_Fuori_Reg Is Nothing Then
                        Dr_TFReg = Dt_Terreni_Fuori_Reg.Select("piva='" & Dt.Rows(i).Item("piva") & "'")
                        If Not Dr_TFReg Is Nothing AndAlso Dr_TFReg.Length > 0 Then
                            Dr_Export.Item("terreni_fuori_regione") = "S"
                            For r = 0 To Dr_TFReg.Length - 1
                                If r = 0 Then
                                    Dr_Export.Item("istat_reg_fuori1") = Dr_TFReg(r).Item("reg")
                                End If
                                If r = 1 Then
                                    Dr_Export.Item("istat_reg_fuori2") = Dr_TFReg(r).Item("reg")
                                End If
                                If r = 2 Then
                                    Dr_Export.Item("istat_reg_fuori3") = Dr_TFReg(r).Item("reg")
                                End If
                            Next
                        End If
                    End If

                    'se non ha terreni fuori ...
                    'verifico se HA terreni in regione
                    If Dr_Export.Item("terreni_fuori_regione") = " " Then
                        Dim Dr_TDReg() As DataRow
                        If Not Dt_Terreni_Dentro_Reg Is Nothing Then
                            Dr_TDReg = Dt_Terreni_Dentro_Reg.Select("piva='" & Dt.Rows(i).Item("piva") & "'")
                            If Not Dr_TDReg Is Nothing AndAlso Dr_TDReg.Length > 0 Then
                                Dr_Export.Item("terreni_fuori_regione") = "N"
                            End If
                        End If
                    End If


                    Dr_Export.Item("socio_diretto") = "N" 'soci terremerse non diretti OP (pempa)
                    Dr_Export.Item("cuaa1") = Dt.Rows(i).Item("piva_padre")
                    Dr_Export.Item("cuaa2") = " "
                    Dr_Export.Item("cuaa3") = " "

                    Dr_Export.Item("tipo_socio") = "P" 'produttore
                    Dr_Export.Item("data_revoca") = " "

                    Dim Dr_Prod() As DataRow
                    N_Prod = 1
                    If Not Dt_Prodotti Is Nothing Then
                        Dr_Prod = Dt_Prodotti.Select("piva='" & Dt.Rows(i).Item("piva") & "'")
                        If Not Dr_Prod Is Nothing AndAlso Dr_Prod.Length > 0 Then
                            For r = 0 To Dr_Prod.Length - 1
                                Dr_Export.Item("id_prodotto" & N_Prod) = Dr_Prod(r).Item("nc_cod")
                                Dr_Export.Item("des_prodotto" & N_Prod) = Dr_Prod(r).Item("nc_des")
                                Select Case Dr_Prod(r).Item("Grfi_cod")
                                    Case 1, 4, 7, 11, 20, 21, 22, 24
                                        Dr_Export.Item("tipo_prodotto" & N_Prod) = "T"
                                    Case 2, 3, 6, 8, 9, 23
                                        Dr_Export.Item("tipo_prodotto" & N_Prod) = "F"
                                    Case 10
                                        Dr_Export.Item("tipo_prodotto" & N_Prod) = " "
                                End Select
                                'Dr_Export.Item("tipo_prodotto" & N_Prod) = Dr_Prod(r).Item("Grfi_Des")
                                Dr_Export.Item("qta_prodotto" & N_Prod) = 0
                                N_Prod += 1
                            Next
                        End If
                    End If

                    Dt_Esport.Rows.Add(Dr_Export)

                Next


                Dt_Esport.Columns(0).ColumnName = "Codice Unione"
                Dt_Esport.Columns(1).ColumnName = "Codice ISTAT Regione"
                Dt_Esport.Columns(2).ColumnName = "CUAA OP"

                Dt_Esport.Columns(3).ColumnName = "CUAA Produttore"
                Dt_Esport.Columns(4).ColumnName = "Partita IVA Produttore"
                Dt_Esport.Columns(5).ColumnName = "Ragione Sociale"
                Dt_Esport.Columns(6).ColumnName = "Legale Rappresentante"

                Dt_Esport.Columns(7).ColumnName = "Data Costituzione"
                Dt_Esport.Columns(8).ColumnName = "Data Nascita"
                Dt_Esport.Columns(9).ColumnName = "Istat Provincia Nascita"
                Dt_Esport.Columns(10).ColumnName = "Istat Comune Nascita"
                Dt_Esport.Columns(11).ColumnName = "Provincia Nascita"
                Dt_Esport.Columns(12).ColumnName = "Comune Nascita"
                Dt_Esport.Columns(13).ColumnName = "Sesso"

                Dt_Esport.Columns(14).ColumnName = "Indirizzo o sede sociale"
                Dt_Esport.Columns(15).ColumnName = "Cap"
                Dt_Esport.Columns(16).ColumnName = "Istat Provincia"
                Dt_Esport.Columns(17).ColumnName = "Istat Comune"
                Dt_Esport.Columns(18).ColumnName = "Provincia"
                Dt_Esport.Columns(19).ColumnName = "Comune"
                Dt_Esport.Columns(20).ColumnName = "Telefono"

                Dt_Esport.Columns(21).ColumnName = "Data adesione Produttore"
                Dt_Esport.Columns(22).ColumnName = "Flag Terreni fuori regione"
                Dt_Esport.Columns(23).ColumnName = "Istat prima Regione dei terreni fuori regione"
                Dt_Esport.Columns(24).ColumnName = "Istat seconda Regione dei terreni fuori regione"
                Dt_Esport.Columns(25).ColumnName = "Istat terza Regione dei terreni fuori regione"
                Dt_Esport.Columns(26).ColumnName = "Flag Socio Diretto"
                Dt_Esport.Columns(27).ColumnName = "Primo CUAA Socio Indiretto"
                Dt_Esport.Columns(28).ColumnName = "Secondo CUAA Socio Indiretto"
                Dt_Esport.Columns(29).ColumnName = "Terzo CUAA Socio Indiretto"
                Dt_Esport.Columns(30).ColumnName = "Tipo Socio"
                Dt_Esport.Columns(31).ColumnName = "Data Revoca Adesione"

                N_Prod = 1
                For c = 32 To 111
                    If InStr(Dt_Esport.Columns(c).ColumnName.ToLower, "id_prodotto") Then
                        Dt_Esport.Columns(c).ColumnName = "ID Prodotto" & N_Prod
                    End If
                    If InStr(Dt_Esport.Columns(c).ColumnName.ToLower, "des_prodotto") Then
                        Dt_Esport.Columns(c).ColumnName = "Descrizione Prodotto" & N_Prod
                    End If
                    If InStr(Dt_Esport.Columns(c).ColumnName.ToLower, "tipo_prodotto") Then
                        Dt_Esport.Columns(c).ColumnName = "Tipologia Conferimento (fresco o trasformato)" & N_Prod
                    End If
                    If InStr(Dt_Esport.Columns(c).ColumnName.ToLower, "qta_prodotto") Then
                        Dt_Esport.Columns(c).ColumnName = "Quantita di Prodotto" & N_Prod
                        N_Prod += 1
                    End If
                Next

                AgronicaCoreGestioneRichieste.Esporta.EsportaExcel(Dt_Esport, "EsportazioneOProduttori", Page)

            End If

        End If

    End Sub

    Private Function Leggi_Produttori(ByVal FiltroPive As String) As DataTable

        Dim strErr As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        StrSQL.Length = 0
        StrSQL.Append(" SELECT  Imprese.Rag_soc, Imprese.piva, Imprese.validita_inizio, ")
        StrSQL.Append("         ISNULL  ((  SELECT    TOP 1 val_cod FROM    Imprese_Codici ")
        StrSQL.Append("                     WHERE   Imprese_Codici.piva = Imprese.piva AND Imprese_Codici.id_cod = " + CStr(CA_CUAA) + "), '') AS CUAA, ")
        StrSQL.Append("         ISNULL  ((  SELECT    TOP 1 padre FROM    GerarchiaImprese ")
        StrSQL.Append("                     WHERE   GerarchiaImprese.figlio = Imprese.piva ), '') AS piva_padre, ")
        StrSQL.Append(" Indirizzi.ind_des, Indirizzi.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, Indirizzi.com_des, Indirizzi.pro_cod, ")

        StrSQL.Append("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
        StrSQL.Append("                         FROM    Imprese_Codici AS Imprese_Codici_1  ")
        StrSQL.Append("                         WHERE   Imprese_Codici_1.piva = Imprese.piva AND Imprese_Codici_1.id_cod = 1087), '01/01/1900') AS data_libro_soci ")

        StrSQL.Append(" FROM imprese INNER JOIN ")
        StrSQL.Append(" ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA INNER JOIN ")
        StrSQL.Append(" Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")
        StrSQL.Append(" WHERE     Imprese.Piva IN " & Agro_SQL_Save_Clausola_IN(FiltroPive, True) & "   ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            DT = objSQL.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, "Esportazione_OP_Produttori.Crea_Dt_Produttori")
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
        End Try

        Return DT

    End Function

    Private Function Terreni_Fuori_Regione(ByVal Pive As String, ByVal Reg As String) As DataTable

        Dim strErr As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Reg = Right("000" & Reg, 3)

        StrSQL.Length = 0
        StrSQL.Append(" SELECT distinct Lista_Province.REG, ImpreseXParticelle.piva ")
        StrSQL.Append(" FROM  ImpreseXParticelle INNER JOIN Lista_Province ON ImpreseXParticelle.PROV = Lista_Province.PROV ")
        StrSQL.Append(" WHERE   ImpreseXParticelle.Piva IN " & Agro_SQL_Save_Clausola_IN(Pive, True) & "   ")
        StrSQL.Append(" AND     Lista_Province.REG <>'" & Agro_SQL_SaveText(Reg) & "'   ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            DT = objSQL.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, "Esportazione_OP_Produttori.Verifica_Regioni_Terreni")
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
        End Try

        Return DT


    End Function

    Private Function Terreni_Dentro_Regione(ByVal Pive As String, ByVal Reg As String) As DataTable

        Dim strErr As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Reg = Right("000" & Reg, 3)

        StrSQL.Length = 0
        StrSQL.Append(" SELECT distinct Lista_Province.REG, ImpreseXParticelle.piva ")
        StrSQL.Append(" FROM  ImpreseXParticelle INNER JOIN Lista_Province ON ImpreseXParticelle.PROV = Lista_Province.PROV ")
        StrSQL.Append(" WHERE   ImpreseXParticelle.Piva IN " & Agro_SQL_Save_Clausola_IN(Pive, True) & "   ")
        StrSQL.Append(" AND     Lista_Province.REG ='" & Agro_SQL_SaveText(Reg) & "'   ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            DT = objSQL.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, "Esportazione_OP_Produttori.Verifica_Regioni_Terreni")
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
        End Try

        Return DT


    End Function

    Private Function Leggi_Prodotti(ByVal Pive As String, ByVal Data As String) As DataTable

        Dim strErr As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        StrSQL.Length = 0
        StrSQL.Append(" SELECT distinct r.piva, fin.Grfi_Des, fin.Grfi_cod, cod.nc_cod, cod.NC_Des ")
        StrSQL.Append(" from Reg_Impianti r inner join cultivar cul on cul.Cul_Cod=r.CUL_COD inner join Codifica_SpecieVegetali_Dogane cod on cod.Veg_cod=cul.Veg_Cod inner join GruppoFinalita fin on fin.Grfi_Cod=r.GRFI_COD ")
        StrSQL.Append(" WHERE   r.Piva IN " & Agro_SQL_Save_Clausola_IN(Pive, True) & "   ")
        StrSQL.Append(" AND     r.validita_inizio <=" & Agro_SQL_SaveDate(Data) & "   ")
        StrSQL.Append(" AND     r.validita_fine >=" & Agro_SQL_SaveDate(Data) & "   ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            DT = objSQL.EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, "Esportazione_OP_Produttori.Leggi_Prodotti")
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
        End Try

        Return DT

    End Function

End Class