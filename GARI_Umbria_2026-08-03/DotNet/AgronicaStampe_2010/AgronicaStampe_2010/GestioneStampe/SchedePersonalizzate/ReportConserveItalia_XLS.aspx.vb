Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreContabDAL

Public Class ReportConserveItalia_XLS
    Inherits System.Web.UI.Page

    Dim Sql_Filtro As String
    Dim Xml_Filtro As String
    Dim Filtro As String

    Dim ParametriAgronicaStampe_2010 As ParametriAgronicaStampe_2010

    Dim Dt As DataTable
    Dim Dv As DataView

    'oggetto objparametri x server e utenti
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents TableReportConserveItalia As System.Web.UI.HtmlControls.HtmlTable

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Inserire qui il codice utente necessario per inizializzare la pagina


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

        '----------------
        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = RiepilogoSemineTrapianti.xls")


        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Gest_Stampe, _
        '                            enum_Security_Operazione.Lettura, _
        '                            strDummy)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.


        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------


        If Not UtenteAbilitato Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If


        '##############################################################
        '#####  Recupero i filtri scelti  #############################
        '##############################################################

        ParametriAgronicaStampe_2010 = New ParametriAgronicaStampe_2010()
        ParametriAgronicaStampe_2010.Leggi()

        CreaNewDtRisultati()

        'Sql_Filtro = Session("Sql_Filtro")
        'Xml_Filtro = Session("Xml_Filtro")

        Dim str_errore As String = ""
        'If Sql_Filtro <> "" Then
        '    CreaDtRisultati()
        'Else
        '    str_errore = "filtro SQL non valorizzato."
        'End If

        'If Xml_Filtro <> "" Then
        '    Crea_Stringa_Filtro()
        'Else
        '    Filtro = "Filtro non impostato."
        'End If

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim Riga As HtmlTableRow
        Dim i, j As Integer

        'Me.TableReportConserveItalia.Rows(0).Cells(0).InnerHtml = Filtro
        'Me.TableReportConserveItalia.Rows(0).Cells(0).ColSpan = 14

        'Creo la prima riga con l'intestazione
        Riga = New HtmlTableRow

        For j = 0 To 14
            Riga.Cells.Add(New HtmlTableCell)
            'UtilityProvider.ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
            UtilityProvider.ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
        Next

        Riga.Cells(0).InnerHtml = "Cooperativa<br>Referente"
        Riga.Cells(1).InnerHtml = "Partita<br>Iva"
        Riga.Cells(2).InnerHtml = "Ragione<br>Sociale"
        Riga.Cells(3).InnerHtml = "Centro<br>Aziendale"
        Riga.Cells(4).InnerHtml = "Appezzamento"
        Riga.Cells(5).InnerHtml = "Sup.<br>[Ha]"
        Riga.Cells(6).InnerHtml = "Specie"
        Riga.Cells(7).InnerHtml = "Varieta'"
        Riga.Cells(8).InnerHtml = "Operazione<br>Colturale"
        Riga.Cells(9).InnerHtml = "Data"
        Riga.Cells(10).InnerHtml = "Cod Articolo"
        Riga.Cells(11).InnerHtml = "Lotto"
        Riga.Cells(12).InnerHtml = "Quantita'<br>Seme"
        Riga.Cells(13).InnerHtml = "Unita'<br>Misura"
        Riga.Cells(14).InnerHtml = "Piano<br>Semina"

        TableReportConserveItalia.Rows.Add(Riga)

        If str_errore <> "" Then
            'visualizzo l'errore
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).InnerHtml = str_errore
            TableReportConserveItalia.Rows.Add(Riga)
        End If

        If Not IsNothing(Dv) AndAlso Dv.Count > 0 Then

            For i = 0 To Dv.Count - 1

                Riga = New HtmlTableRow

                For j = 0 To 14

                    Dim dummy = Dv.Item(i).Item("qta")
                    'aggiungo la cella
                    Riga.Cells.Add(New HtmlTableCell)

                    Select Case j


                        Case 0
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("coop_referente")), Dv.Item(i).Item("coop_referente"), "&nbsp;")

                        Case 1
                            'aggiungo alla piva lo spazio x salvare gli zeri..
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("PivaReale")), "&nbsp;" & Dv.Item(i).Item("PivaReale"), "&nbsp;")

                        Case 2
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("rag_soc")), Dv.Item(i).Item("rag_soc"), "&nbsp;")

                        Case 3
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("sa_nome")), Dv.Item(i).Item("sa_nome"), "&nbsp;")

                        Case 4
                            Riga.Cells(j).InnerHtml = "'" & IIf(Not IsDBNull(Dv.Item(i).Item("app_nome")), Dv.Item(i).Item("app_nome"), "&nbsp;")

                        Case 5
                            'Divido la Superficie totale per il numero di prodotti associati
                            Dim Dr = Dt.Select("id_agenda = " & Dt.Rows(i).Item("id_agenda") & " And APPEZZA =" & Dt.Rows(i).Item("APPEZZA") & " And ID_REG =" & Dt.Rows(i).Item("ID_REG"))

                            Riga.Cells(j).InnerHtml = "&nbsp;"

                            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                                Dim sup_imp = Math.Round(CDbl(Dt.Rows(i).Item("sup_trattata") / Dr.Length), 2)

                                If sup_imp > 0 Then
                                    Riga.Cells(j).InnerHtml = sup_imp
                                End If
                            End If

                        Case 6
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("veg_des")), Dv.Item(i).Item("veg_des"), "&nbsp;")
                            If Riga.Cells(j).InnerHtml = "&nbsp;" Then
                                'se esiste l'impianto
                                If Not IsDBNull(Dv.Item(i).Item("id_reg")) Then
                                    Riga.Cells(j).InnerHtml = "Terreno Nudo"
                                End If
                            End If

                        Case 7
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("cul_des")), Dv.Item(i).Item("cul_des"), "&nbsp;")

                        Case 8
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("lav_des")), Dv.Item(i).Item("lav_des"), "&nbsp;")

                        Case 9
                            'stampo la data nel formato short
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("data_movimento")), Dv.Item(i).Item("data_movimento"), "&nbsp;")
                            If Riga.Cells(j).InnerHtml <> "&nbsp;" Then
                                Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                            End If

                        Case 10
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("cod_articolo")), Dv.Item(i).Item("cod_articolo") & "&nbsp;", "&nbsp;")
                            
                        Case 11
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("lotto")), Dv.Item(i).Item("lotto") & "&nbsp;", "&nbsp;")

                        Case 12
                            Dim qtaDB As Decimal = If(Not IsDBNull(Dv.Item(i).Item("qta")), Dv.Item(i).Item("qta"), 0)

                            Dim qtaExcel As String = qtaDB
                            If Dv.Item(i).Item("udm_cod") = enum_UnitaMisura.KG Then
                                qtaExcel = Decimal.Round(qtaDB, 3, MidpointRounding.AwayFromZero).ToString("0.###")
                            ElseIf Dv.Item(i).Item("udm_cod") = enum_UnitaMisura.Unita_Seme OrElse
                                   Dv.Item(i).Item("udm_cod") = enum_UnitaMisura.Num_Piante Then
                                qtaExcel = Decimal.Round(qtaDB, 3, MidpointRounding.AwayFromZero).ToString("0")
                            End If
                            Riga.Cells(j).InnerHtml = qtaExcel

                        Case 13
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("udm_sim")), Dv.Item(i).Item("udm_sim"), "&nbsp;")

                        Case 14
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item("piano_semina")), Dv.Item(i).Item("piano_semina"), "&nbsp;")

                        Case Else
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item(j)), Dv.Item(i).Item(j), "&nbsp;")

                    End Select

                Next

                'Aggiungo la Riga alla Tabella 
                Me.TableReportConserveItalia.Rows.Add(Riga)

            Next

        End If

    End Sub

    Private Sub CreaNewDtRisultati()
        Dv = New DataView

        Dim handleMovDestinazioni As New Mov_Destinazioni_R

        Dim listaImpianti As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti)

        Dim xmlParametriStampe As New System.Xml.XmlDocument()

        xmlParametriStampe.LoadXml(ParametriAgronicaStampe_2010.Xml_Generico.ToString)

        Dim xmlVarStampa = xmlParametriStampe.SelectNodes("FiltroStampa/VariabiliStampe")

        For i = 0 To xmlVarStampa.Count - 1

            Dim nodeXml = CType(xmlVarStampa.Item(i), System.Xml.XmlElement)

            listaImpianti.Add(New AgronicaCoreEntityFramework_POCO.Reg_Impianti With {
                .PIVA = nodeXml.GetAttribute("piva"),
                .SA_COD = nodeXml.GetAttribute("sa_cod"),
                .APPEZZA = nodeXml.GetAttribute("appezza"),
                .ID_REG = nodeXml.GetAttribute("id_reg")
            })
        Next

        Dt = handleMovDestinazioni.LeggiSemineStampaXLS(objParametri_Server, ParametriAgronicaStampe_2010.Piva, listaImpianti)
        Dt.TableName = "Movimenti"
        Dv.Table = Dt

    End Sub

    '##############################################################################################################
    Private Sub CreaDtRisultati()

        Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dv = New DataView
        Dim Dr_Temp As DataRow()
        Dim i, j As Integer
        Dim Num_Lotti As Integer

        Dim RsLotti As New ADODB.Recordset
        Dim Coop As String
        Dim strCoop As String
        Dim objJoin As New AgronicaCoreUtility.JoinFiltrone
        Dim objFiltrone As New AgronicaCoreUtility.Filtrone
        Dim handleImpresa As New Imprese_Read

        objFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(Sql_Filtro, objJoin)
        objFiltrone.ImpostaVariabiliJOIN(objJoin)

        'eseguo la query del filtro
        'Dim Rs As ADODB.Recordset = OBJ.FiltroneSuperNova(Session("ASG_SuperUser_CodFiscale"), 0, , , , , CStr(Session("ASG_Connessione_Server")), Sql_Filtro)
        Dim DT As DataTable = objFiltrone.CreaDTFiltrone(objParametri_Server, Sql_Filtro, enum_TipoSelect_FiltroneSuperNova.Base, "", objJoin)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            'aggiungo le colonne che mi servono...
            DT.Columns.Add("coop_referente", GetType(String))
            DT.Columns.Add("dett_mat_prima", GetType(String))
            DT.Columns.Add("qta", GetType(String))
            DT.Columns.Add("udm_sim", GetType(String))
            DT.Columns.Add("piano_semina", GetType(String))

            For i = 0 To DT.Rows.Count - 1

                strCoop = ""

                ''leggo i padri
                ''Leggo le imprese associate al profilo selezionato			
                'RsC = objCOMC.LeggiXFiglio(Dt.Rows(i).Item("piva"), , , _
                '                         , _
                '                         , _
                '                         CStr(Session("ASG_Connessione_Server")))

                'If RsC.State <> 0 Then
                '    Do While Not RsC.EOF
                '        strCoop &= RagSoc_from_Piva(CStr(RsC.Fields("padre").Value), Server) & ", "
                '        RsC.MoveNext()
                '    Loop
                'End If

                'If strCoop <> "" Then
                '    strCoop = Left(strCoop, strCoop.Length - 2)
                'End If

                'Dt.Rows(i).Item("coop_referente") = strCoop

                'leggo la cooperativa associata all'impianto..
                If Not IsDBNull(DT.Rows(i).Item("id_reg")) Then
                    Coop = Reg_Impianti_Codici_R.Leggi_Codice_from_Reg_Impianti_Codici_Distinta(DT.Rows(i).Item("piva"), DT.Rows(i).Item("sa_cod"), DT.Rows(i).Item("appezza"), DT.Rows(i).Item("id_reg"), enum_CodiciAnagrafe.Impianto_Cooperativa, objParametri_Server)
                    If Coop <> "" Then
                        DT.Rows(i).Item("coop_referente") = handleImpresa.RagSoc_from_Piva(Coop, objParametri_Server)
                    End If
                End If

                'leggo il piano semina..
                If Not IsDBNull(DT.Rows(i).Item("appezza")) AndAlso Not IsDBNull(DT.Rows(i).Item("id_reg")) Then
                    DT.Rows(i).Item("piano_semina") = Reg_Impianti_Codici_R.Leggi_Codice_from_Reg_Impianti_Codici_Distinta(DT.Rows(i).Item("piva"), DT.Rows(i).Item("sa_cod"), DT.Rows(i).Item("appezza"), DT.Rows(i).Item("id_reg"), enum_CodiciAnagrafe.Impianto_PianoSemina, objParametri_Server)
                End If


            Next

            For i = 0 To DT.Rows.Count - 1

                'se ho una semina leggo i lotti ed eventualmente duplico la riga
                If Not IsDBNull(DT.Rows(i).Item("lav_cod")) Then

                    If CInt(DT.Rows(i).Item("lav_cod")) = 2 Then

                        RsLotti = Leggi_Lotti_Seminati(Server, Session, Page, CStr(DT.Rows(i).Item("piva")), CInt(DT.Rows(i).Item("sa_cod")), CInt(DT.Rows(i).Item("appezza")), CInt(DT.Rows(i).Item("id_reg")), CInt(DT.Rows(i).Item("id_agenda")))

                        If RsLotti.State <> 0 Then

                            If Not RsLotti.EOF Then

                                'copio i dati del lotto seminato nella riga corrente
                                RsLotti.MoveFirst()

                                DT.Rows(i).Item("dett_mat_prima") = "Cod." & RsLotti.Fields("Cod_Articolo").Value.ToString

                                If CStr(RsLotti.Fields("lotto").Value).ToLower <> "indefinito" AndAlso RsLotti.Fields("lotto").Value <> "" Then
                                    DT.Rows(i).Item("dett_mat_prima") &= " - Lotto: " & RsLotti.Fields("lotto").Value
                                End If
                                DT.Rows(i).Item("qta") = Math.Round(CDbl(RsLotti.Fields("Qta").Value), 2)
                                DT.Rows(i).Item("udm") = RsLotti.Fields("UDM_SIM").Value

                                RsLotti.MoveNext()

                                'se per la semina sono stati impiegati più lotti aggiungo al dt una nuova riga..
                                For j = 1 To RsLotti.RecordCount - 1

                                    DT.ImportRow(DT.Rows(i))

                                    DT.Rows(DT.Rows.Count - 1).Item("dett_mat_prima") = "Cod." & RsLotti.Fields("Cod_Articolo").Value.ToString
                                    If CStr(RsLotti.Fields("lotto").Value).ToLower <> "indefinito" And RsLotti.Fields("lotto").Value <> "" Then
                                        DT.Rows(DT.Rows.Count - 1).Item("dett_mat_prima") &= " - Lotto: " & RsLotti.Fields("lotto").Value
                                    End If

                                    DT.Rows(DT.Rows.Count - 1).Item("qta") = Math.Round(CDbl(RsLotti.Fields("Qta").Value), 2)
                                    DT.Rows(DT.Rows.Count - 1).Item("udm_sim") = RsLotti.Fields("UDM_SIM").Value

                                    RsLotti.MoveNext()

                                Next

                            End If

                        End If

                    End If

                End If

            Next

            'ciclo per dividere eventuali sup doppie...
            For i = 0 To DT.Rows.Count - 1
                If Not IsDBNull(DT.Rows(i).Item("id_agenda")) Then
                    'Dr_Temp = Dt.Select("id_agenda = " & Dt.Rows(i).Item("id_agenda"))
                    Dr_Temp = DT.Select("id_agenda = " & DT.Rows(i).Item("id_agenda") & " And APPEZZA =" & DT.Rows(i).Item("APPEZZA") & " And ID_REG =" & DT.Rows(i).Item("ID_REG"))
                    Num_Lotti = Dr_Temp.Length
                    DT.Rows(i).Item("sup_imp") = Math.Round(CDbl(DT.Rows(i).Item("sup_imp") / Num_Lotti), 2)
                End If
            Next

            'ordino il dt..
            DT.TableName = "Movimenti"

            Dv.Table = DT
            Dv.Sort = "coop_referente, rag_soc, sa_nome, app_nome, id_agenda"

            'Elimino dal datatable le righe senza appezza
            For i = 0 To DT.Rows.Count - 1
                If IsDBNull(DT.Rows(i).Item("appezza")) Then
                    DT.Rows(i).Delete()
                End If
            Next

        End If

    End Sub


    '########################################################################################
    'spacchetta la stringa xml dei permessi e genera la stringa 
    'di riassunto del filtro selezionato

    Private Sub Crea_Stringa_Filtro()

        Dim Categorie_Magazzino_R As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim GruppiOperazioni_R As New AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R
        Dim GruppoAvversitaAttive_R As New AgronicaCoreMetaSchemaDAL.GruppoAvversitaAttive_R
        Dim GerarchiaImprese_R As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim Operazioni_R As New AgronicaCoreAnagrafeDAL.Operazioni_R
        Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim GruppoAvversita_R As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
        Dim Avversita_R As New AgronicaCoreMetaSchemaDAL.Avversita_R
        Dim Codice_Anagrafe_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim Istat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
        Dim Ote As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
        Dim GruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim Finanziamenti As New AgronicaCoreMetaSchemaDAL.Finanziamenti_R
        Dim CategorieMagazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiFiltri As System.Xml.XmlElement
        Dim XML_Filtro As System.Xml.XmlElement
        Dim XML_Impresa As System.Xml.XmlElement
        Dim XML_Struttura As System.Xml.XmlElement
        Dim XML_Appezzamento As System.Xml.XmlElement
        Dim XML_Impianto As System.Xml.XmlElement
        Dim XML_Agenda As System.Xml.XmlElement
        Dim XML_DatiGerarchiaImprese As System.Xml.XmlElement
        Dim XML_GerarchiaImprese As System.Xml.XmlElement
        Dim XMLs_GerarchiaImprese As System.Xml.XmlNodeList

        Dim i, j, x As Integer
        Dim Diserbo As Boolean = False
        Dim TrattamentoAntiparassitario As Boolean = False

        Dim Num_Cooperative As Integer

        Dim Cultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim GruppoVegetale As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R
        Dim Regolamento As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim handleImpresa As New Imprese_Read

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Session("Xml_Filtro"))

        '----- Tag DatiFiltri

        XML_DatiFiltri = XmlDoc.SelectSingleNode("DatiFiltri")

        '----- Tag Filtro

        XML_Filtro = XML_DatiFiltri.SelectSingleNode("Filtro")

        Filtro = "<b>FILTRO SELEZIONATO : </b><br>"

        '--------------------------------------------------
        '-------------- IMPRESA ---------------------------
        '--------------------------------------------------

        XML_Impresa = XML_Filtro.SelectSingleNode("Impresa")

        XML_DatiGerarchiaImprese = XML_Filtro.SelectSingleNode("DatiGerarchiaImprese")


        'Se esiste il nodo DatiPive..
        If XML_DatiGerarchiaImprese IsNot Nothing Then

            'e se ha figli..
            If XML_DatiGerarchiaImprese.HasChildNodes Then

                'recupero il numero di cooperative
                Num_Cooperative = GerarchiaImprese_R.Numero_Cooperative(objParametri_Server)

                'Recupero la collezione dei nodi
                XMLs_GerarchiaImprese = XML_DatiGerarchiaImprese.GetElementsByTagName("GerarchiaImprese")

                'se il numero di cooperative <> numero di nodi cooperative creo la stringa
                'altrimenti significa che le ho tutte quindi non le indico nella stringa filtro...
                If Num_Cooperative <> XMLs_GerarchiaImprese.Count Then

                    Dim strPadri As String = " <b>Cooperative: </b>"

                    For i = 0 To XMLs_GerarchiaImprese.Count - 1

                        'Prendo l'i-esimo nodo della collezione
                        XML_GerarchiaImprese = XMLs_GerarchiaImprese.Item(i)

                        If XML_GerarchiaImprese.GetAttribute("padre") <> "" Then
                            strPadri += handleImpresa.RagSoc_from_Piva(XML_GerarchiaImprese.GetAttribute("padre"), objParametri_Server) & "; "
                        End If
                    Next

                    If strPadri.Length > 21 Then
                        strPadri = Left(strPadri, strPadri.Length - 2)
                        Filtro += strPadri & "<br>"
                    End If

                End If

            End If

        End If


        ''cooperativa
        'If XML_Impresa.GetAttribute("cooperativa") <> "" Then
        '    Filtro += " <b>Cooperativa: </b>" & RagSoc_from_Piva(XML_Impresa.GetAttribute("cooperativa"), Server) & "<br>"
        'End If

        'piva
        If XML_Impresa.GetAttribute("piva") <> "" Then
            Filtro += " <b>Partita Iva: </b>" & XML_Impresa.GetAttribute("piva") & "<br>"
        End If

        'ragione sociale
        If XML_Impresa.GetAttribute("ragione_sociale") <> "" Then
            Filtro += " <b>Ragione sociale: </b>" & XML_Impresa.GetAttribute("ragione_sociale") & "<br>"
        End If


        'regione
        Dim strRegioni As String = " <b>Regioni: </b>"
        For i = 1 To 21
            If XML_Impresa.GetAttribute("reg" & i) <> "" Then
                strRegioni += Province.RegioneDes_from_RegioneCod(XML_Impresa.GetAttribute("reg" & i), Nothing) & "; "
            Else
                Exit For
            End If
        Next
        If strRegioni.Length > 17 Then
            strRegioni = Left(strRegioni, strRegioni.Length - 2)
            Filtro += strRegioni & "<br>"
        End If


        'provincia
        Dim strProvincie As String = " <b>Provincie: </b>"
        Dim N_Provincie As Integer = Province.Numero_Totale_Province(objParametri_Server)
        For i = 1 To N_Provincie
            If XML_Impresa.GetAttribute("pro_cod_istat" & i) <> "" Then

                strProvincie += Istat.Provincia_from_CodIstat(XML_Impresa.GetAttribute("pro_cod_istat" & i), Nothing, objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strProvincie.Length > 19 Then
            strProvincie = Left(strProvincie, strProvincie.Length - 2)
            Filtro += strProvincie & "<br>"
        End If

        'comune
        If XML_Impresa.GetAttribute("com_cod_istat") <> "" Then
            Filtro += " <b>Comune: </b>" & LCase(Istat.Comune_from_CodIstat(Mid(XML_Impresa.GetAttribute("com_cod_istat"), 4, 3), Left(XML_Impresa.GetAttribute("com_cod_istat"), 3), objParametri_Server)) & "<br>"
        End If


        'data costituzione
        Dim strDataCostituzione As String = " <b>Data Costituzione: </b>"
        If XML_Impresa.GetAttribute("validita_inizio_inferiore_az") <> "" Then
            strDataCostituzione += " Precedente al " & XML_Impresa.GetAttribute("validita_inizio_inferiore_az") & "<br>"
        ElseIf XML_Impresa.GetAttribute("validita_inizio_superiore_az") <> "" Then
            strDataCostituzione += " Successiva al " & XML_Impresa.GetAttribute("validita_inizio_superiore_az") & "<br>"
        End If
        If strDataCostituzione.Length > 27 Then
            Filtro += strDataCostituzione
        End If

        'data cessazione
        Dim strDataCessazione As String = " <b>Data Cessazione: </b>"
        If XML_Impresa.GetAttribute("validita_fine_inferiore_az") <> "" Then
            strDataCessazione += " Precedente al " & XML_Impresa.GetAttribute("validita_fine_inferiore_az") & "<br>"
        ElseIf XML_Impresa.GetAttribute("validita_fine_superiore_az") <> "" Then
            strDataCessazione += " Successiva al " & XML_Impresa.GetAttribute("validita_fine_superiore_az") & "<br>"
        End If
        If strDataCessazione.Length > 25 Then
            Filtro += strDataCessazione
        End If

        'codici dell'azienda
        If XML_Impresa.GetAttribute("codice") <> "" Then
            Filtro += " <b>Codice Azienda: </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(XML_Impresa.GetAttribute("codice"), objParametri_Server)
        End If
        If XML_Impresa.GetAttribute("codice_valore") <> "" Then
            Filtro += " <b>Valore: </b>" & XML_Impresa.GetAttribute("codice_valore") & "<br>"
        End If

        'titolo possesso
        If XML_Impresa.GetAttribute("titolopossesso") <> "" Then
            Filtro += " <b>Titolo Possesso Azienda: </b>" & UtilityProvider.TitoloPossessoDes_from_TitoloPossessoCod(CInt(XML_Impresa.GetAttribute("titolopossesso_valore"))) & "<br>"
        End If

        '--------------------------------------------------
        '-------------- CENTRO AZIENDALE ------------------
        '--------------------------------------------------

        XML_Struttura = XML_Impresa.SelectSingleNode("Struttura")

        'centro aziendale
        If XML_Struttura.GetAttribute("sa_nome") <> "" Then
            Filtro += " <b>Centro Aziendale: </b>" & XML_Struttura.GetAttribute("sa_nome") & "<br>"
        End If

        'codici della struttura
        If XML_Struttura.GetAttribute("codice") <> "" Then
            Filtro += " <b>Codice Struttura: </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(XML_Struttura.GetAttribute("codice"), objParametri_Server)
        End If
        If XML_Struttura.GetAttribute("codice_valore") <> "" Then
            Filtro += " <b>Valore: </b>" & XML_Struttura.GetAttribute("codice_valore") & "<br>"
        End If

        'Tipo Centro
        If XML_Struttura.GetAttribute("tipocentro") <> "" Then
            Filtro += " <b>Tipo Centro: </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(XML_Struttura.GetAttribute("tipocentro_valore")), objParametri_Server) & "<br>"
        End If

        'Tipo Attivita
        If XML_Struttura.GetAttribute("tipoattivita") <> "" Then
            Filtro += " <b>Tipo Attivita' : </b>" & Attivita.TipoAttivitaDes_from_TipoAttivitaCod(XML_Struttura.GetAttribute("tipoattivita_valore")) & "<br>"
        End If

        'OTE
        If XML_Struttura.GetAttribute("ote") <> "" Then
            Filtro += " <b>OTE : </b>" & Ote.OteDes_from_OteCod(XML_Struttura.GetAttribute("ote_valore"), objParametri_Server) & "<br>"
        End If

        'Titolo Possesso
        If XML_Struttura.GetAttribute("titolopossesso") <> "" Then
            Filtro += " <b>Titolo Possesso Struttura: </b>" & UtilityProvider.TitoloPossessoDes_from_TitoloPossessoCod(CInt(XML_Struttura.GetAttribute("titolopossesso_valore"))) & "<br>"
        End If

        'Organismi di controllo
        If XML_Struttura.GetAttribute("organismicontrollo") <> "" Then
            Filtro += " <b>Organismi di Controllo : </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(XML_Struttura.GetAttribute("organismicontrollo"), objParametri_Server) & "<br>"
        End If


        '--------------------------------------------------
        '-------------- APPEZZAMENTO ----------------------
        '--------------------------------------------------

        XML_Appezzamento = XML_Struttura.SelectSingleNode("Appezzamento")

        Dim strSuperficie As String = " <b>Superficie: </b>"

        'superficie
        If XML_Appezzamento.GetAttribute("superficie_inferiore_ap") <> "" Then
            strSuperficie += "Inferiore a " & CStr(XML_Appezzamento.GetAttribute("superficie_inferiore_ap")) & "[Ha] <br>"
        ElseIf XML_Appezzamento.GetAttribute("superficie_superiore_ap") <> "" Then
            strSuperficie += "Superiore a " & CStr(XML_Appezzamento.GetAttribute("superficie_superiore_ap")) & "[Ha] <br>"
        End If
        If strSuperficie.Length > 20 Then
            Filtro += strSuperficie
        End If

        'codici dell'appezzamento
        If XML_Appezzamento.GetAttribute("codice") <> "" Then
            Filtro += " <b>Codice Appezzamento: </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(XML_Appezzamento.GetAttribute("codice"), objParametri_Server)
        End If
        If XML_Appezzamento.GetAttribute("codice_valore") <> "" Then
            Filtro += " <b>Valore: </b>" & XML_Appezzamento.GetAttribute("codice_valore") & "<br>"
        End If

        'Titolo Possesso
        If XML_Appezzamento.GetAttribute("titolopossesso") <> "" Then
            Filtro += " <b>Titolo Possesso Appezzamento: </b>" & UtilityProvider.TitoloPossessoDes_from_TitoloPossessoCod(CInt(XML_Appezzamento.GetAttribute("titolopossesso_valore"))) & "<br>"
        End If

        'Metodo Produzione
        If XML_Appezzamento.GetAttribute("metodoproduzione") <> "" Then
            Select Case XML_Appezzamento.GetAttribute("metodoproduzione_valore")
                Case 1
                    Filtro += " <b>Metodo Produzione Appezzamento: </b> Convenzionale <br>"
                Case 2
                    Filtro += " <b>Metodo Produzione Appezzamento: </b> Conversione <br>"
                Case 3
                    Filtro += " <b>Metodo Produzione Appezzamento: </b> Biologico <br>"
            End Select
        End If

        'Fine Impiego
        If XML_Appezzamento.GetAttribute("fineimpiego") <> "" Then
            Filtro += " <b>Fine Impiego: </b>" & XML_Appezzamento.GetAttribute("fineimpiego_valore") & "<br>"
        End If

        'sabbia
        If XML_Appezzamento.GetAttribute("sabbia") <> "" Then
            Filtro += " <b>Sabbia min: </b>" & XML_Appezzamento.GetAttribute("sabbia") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("limo") <> "" Then
            Filtro += " <b>Limo min: </b>" & XML_Appezzamento.GetAttribute("limo") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("argilla") <> "" Then
            Filtro += " <b>Argilla min: </b>" & XML_Appezzamento.GetAttribute("argilla") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("ph") <> "" Then
            Filtro += " <b>PH min: </b>" & XML_Appezzamento.GetAttribute("ph") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("calatt") <> "" Then
            Filtro += " <b>Calcare Attivo min: </b>" & XML_Appezzamento.GetAttribute("calatt") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("caltot") <> "" Then
            Filtro += " <b>Calcare Totale min: </b>" & XML_Appezzamento.GetAttribute("caltot") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("ntot") <> "" Then
            Filtro += " <b>Azoto Totale min: </b>" & XML_Appezzamento.GetAttribute("ntot") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("p2o5ass") <> "" Then
            Filtro += " <b>Fosforo min: </b>" & XML_Appezzamento.GetAttribute("p2o5ass") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("k2oass") <> "" Then
            Filtro += " <b>Potassio min: </b>" & XML_Appezzamento.GetAttribute("k2oass") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("mg") <> "" Then
            Filtro += " <b>Magnesio min: </b>" & XML_Appezzamento.GetAttribute("mg") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("csc") <> "" Then
            Filtro += " <b>Cap. Scambio Catonico min: </b>" & XML_Appezzamento.GetAttribute("csc") & "<br>"
        End If

        If XML_Appezzamento.GetAttribute("sostorg") <> "" Then
            Filtro += " <b>Sostanza Organica min: </b>" & XML_Appezzamento.GetAttribute("sostorg") & "<br>"
        End If

        '--------------------------------------------------
        '-------------- IMPIANTO --------------------------
        '--------------------------------------------------

        XML_Impianto = XML_Appezzamento.SelectSingleNode("Impianto")

        'includi terreni nudi
        If XML_Impianto.GetAttribute("terreninudi") <> "" Then
            Filtro += " Terreni Nudi Inclusi <br>"
        End If


        'gruppo colturale
        Dim strGruppiVeg As String = " <b>Gruppi Vegetali: </b>"
        For i = 1 To 3
            If XML_Impianto.GetAttribute("gru_cod" & i) <> "" Then
                strGruppiVeg += GruppoVegetale.GruDes_from_GruCod(XML_Impianto.GetAttribute("gru_cod" & i), objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strGruppiVeg.Length > 25 Then
            strGruppiVeg = Left(strGruppiVeg, strGruppiVeg.Length - 2)
            Filtro += strGruppiVeg & "<br>"
        End If

        'specie vegetale
        Dim handleSpecieVeg As New SpecieVegetali_R
        Dim strSpecieVeg As String = " <b>Specie Vegetali: </b>"
        For i = 1 To SpecieVegetali_R.Numero_Totale_Specie(objParametri_Server)
            If XML_Impianto.GetAttribute("veg_cod" & i) <> "" Then
                strSpecieVeg += handleSpecieVeg.VegDes_from_VegCod(XML_Impianto.GetAttribute("veg_cod" & i), objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strSpecieVeg.Length > 25 Then
            strSpecieVeg = Left(strSpecieVeg, strSpecieVeg.Length - 2)
            Filtro += strSpecieVeg & "<br>"
        End If


        'cultivar
        Dim strCultivar As String = " <b>Varieta': </b>"
        'si possono avere cultivar solo se è stata selezionata una sola specie vegetale
        If XML_Impianto.GetAttribute("veg_cod1") <> "" Then
            For i = 1 To Cultivar_R.Numero_Cultivar_xSpecie(XML_Impianto.GetAttribute("veg_cod1"), objParametri_Server)
                If XML_Impianto.GetAttribute("cul_cod" & i) <> "" Then
                    strCultivar += Cultivar.CulDes_from_CulCod(XML_Impianto.GetAttribute("cul_cod" & i), objParametri_Server) & "; "
                Else
                    Exit For
                End If
            Next
            If strCultivar.Length > 18 Then
                strCultivar = Left(strCultivar, strCultivar.Length - 2)
                Filtro += strCultivar & "<br>"
            End If
        End If

        'solo terreno nudo
        If XML_Impianto.GetAttribute("cul_cod1") = "0" Then
            Filtro += " Solo Terreni Nudi <br>"
        End If


        'finalita
        If XML_Impianto.GetAttribute("grfi_cod") <> "" Then
            'si può avere la finalità solo se è stata selezionata una sola specie vegetale
            Filtro += " <b>Finalita' Produttive/Commerciali: </b>" & GruppoFinalita.GrfiDes_from_GrfiCod(CInt(XML_Impianto.GetAttribute("grfi_cod")), CInt(XML_Impianto.GetAttribute("veg_cod1")), objParametri_Server) & "<br>"
        End If

        'regolamento
        If XML_Impianto.GetAttribute("regolamento") <> "" Then
            Filtro += " <b>Regolamento: </b>" & Regolamento.RegDes_from_RegCod(XML_Impianto.GetAttribute("regolamento"), objParametri_Server) & "<br>"
        End If

        'finanziamento
        If XML_Impianto.GetAttribute("finanziamento") <> "" Then
            Filtro += " <b>Finanziamento: </b>" & Finanziamenti.FinDes_from_FinCod(XML_Impianto.GetAttribute("finanziamento"), objParametri_Server) & "<br>"
        End If

        'data inizio impianto
        Dim strDataInizioImpianto As String = " <b>Data Inizio Impianto: </b>"
        If XML_Impianto.GetAttribute("validita_inizio_inferiore_im") <> "" Then
            strDataInizioImpianto += " Precedente al " & XML_Impianto.GetAttribute("validita_inizio_inferiore_im") & "<br>"
        ElseIf XML_Impianto.GetAttribute("validita_inizio_superiore_im") <> "" Then
            strDataInizioImpianto += " Successiva al " & XML_Impianto.GetAttribute("validita_inizio_superiore_im") & "<br>"
        End If
        If strDataInizioImpianto.Length > 30 Then
            Filtro += strDataInizioImpianto
        End If

        'data fine impianto
        Dim strDataFineImpianto As String = " <b>Data Fine Impianto: </b>"
        If XML_Impianto.GetAttribute("validita_fine_inferiore_im") <> "" Then
            strDataFineImpianto += " Precedente al " & XML_Impianto.GetAttribute("validita_fine_inferiore_im") & "<br>"
        ElseIf XML_Impianto.GetAttribute("validita_fine_superiore_im") <> "" Then
            strDataFineImpianto += " Successiva al " & XML_Impianto.GetAttribute("validita_fine_superiore_im") & "<br>"
        End If
        If strDataFineImpianto.Length > 28 Then
            Filtro += strDataFineImpianto
        End If

        'codici dell'impianto
        If XML_Impianto.GetAttribute("codice") <> "" Then
            Filtro += " <b>Codice Impianto: </b>" & Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(XML_Impianto.GetAttribute("codice"), objParametri_Server)
        End If
        If XML_Impianto.GetAttribute("codice_valore") <> "" Then
            Filtro += " <b>Valore: </b>" & XML_Impianto.GetAttribute("codice_valore") & "<br>"
        End If

        'Titolo Possesso
        If XML_Impianto.GetAttribute("titolopossesso") <> "" Then
            Filtro += " <b>Titolo Possesso Impianto: </b>" & UtilityProvider.TitoloPossessoDes_from_TitoloPossessoCod(CInt(XML_Impianto.GetAttribute("titolopossesso_valore"))) & "<br>"
        End If

        'Metodo Produzione
        If XML_Impianto.GetAttribute("metodoproduzione") <> "" Then
            Select Case XML_Impianto.GetAttribute("metodoproduzione_valore")
                Case 1
                    Filtro += " <b>Metodo Produzione Impianto: </b> Convenzionale <br>"
                Case 2
                    Filtro += " <b>Metodo Produzione Impianto: </b> Conversione <br>"
                Case 3
                    Filtro += " <b>Metodo Produzione Impianto: </b> Biologico <br>"
            End Select
        End If

        '--------------------------------------------------
        '-------------- AGENDA ----------------------------
        '--------------------------------------------------

        XML_Agenda = XML_Impianto.SelectSingleNode("Agenda")

        'data inizio movimenti
        If XML_Agenda.GetAttribute("data_movimento_inizio") <> "" Then
            Filtro += " <b>Data Inizio Movimenti: </b>" & CStr(XML_Agenda.GetAttribute("data_movimento_inizio")) & "<br>"
        End If

        'data fine movimenti
        If XML_Agenda.GetAttribute("data_movimento_fine") <> "" Then
            Filtro += " <b>Data Fine Movimenti: </b>" & CStr(XML_Agenda.GetAttribute("data_movimento_fine")) & "<br>"
        End If

        'gruppo operazioni
        Dim strGroppiOper As String = " <b>Gruppi Operazioni: </b>"
        For i = 1 To 4
            If XML_Agenda.GetAttribute("gru_op" & i) <> "" Then
                strGroppiOper += GruppiOperazioni_R.GruOper_Des_from_GruOper_Cod(XML_Agenda.GetAttribute("gru_op" & i), objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strGroppiOper.Length > 27 Then
            strGroppiOper = Left(strGroppiOper, strGroppiOper.Length - 2)
            Filtro += strGroppiOper & "<br>"
        End If

        'operazioni colturali
        Dim handleOperazioniGias As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim strOper As String = " <b>Operazioni: </b>"
        For i = 1 To Operazioni_R.Numero_Totale_Operazioni(objParametri_Server)
            If XML_Agenda.GetAttribute("lav_cod" & i) <> "" Then
                strOper += handleOperazioniGias.LavDes_from_LavCod(XML_Agenda.GetAttribute("lav_cod" & i), objParametri_Server) & "; "
                'se ho un diserbo o un trattamento antiparassitario
                'metto a true delle variabili..mi serve in seguito per le avversità
                If XML_Agenda.GetAttribute("lav_cod" & i) = "18" Then
                    Diserbo = True
                ElseIf XML_Agenda.GetAttribute("lav_cod" & i) = "74" Then
                    TrattamentoAntiparassitario = True
                End If
            Else
                Exit For
            End If
        Next
        If strOper.Length > 20 Then
            strOper = Left(strOper, strOper.Length - 2)
            Filtro += strOper & "<br>"
        End If

        'tipo prodotto
        If XML_Agenda.GetAttribute("elem_cod") <> "" Then
            Filtro += " <b>Categoria Prodotto: </b>" & Categorie_Magazzino_R.CategoriaMagazzino_from_Elem_Cod(XML_Agenda.GetAttribute("elem_cod"), objParametri_Server) & "<br>"
        End If

        'prodotto
        Dim strProdotti As String = " <b>Prodotti: </b>"
        If XML_Agenda.GetAttribute("elem_cod") <> "" Then
            For i = 1 To Categorie_Magazzino_R.Numero_Prodotti_xCategoria(XML_Agenda.GetAttribute("elem_cod"), objParametri_Server)
                If XML_Agenda.GetAttribute("pro_cod" & i) <> "" Then
                    strProdotti += CategorieMagazzino.ProDes_from_ProCod(XML_Agenda.GetAttribute("elem_cod"), XML_Agenda.GetAttribute("pro_cod" & i), objParametri_Server) & "; "
                Else
                    Exit For
                End If
            Next
            If strProdotti.Length > 18 Then
                strProdotti = Left(strProdotti, strProdotti.Length - 2)
                Filtro += strProdotti & "<br>"
            End If
        End If

        'gruppi infestanti    (possono essere stati salvati solo per una specie e per un diserbo)
        Dim strGruppiInfestanti As String = " <b>Gruppi Infestanti: </b>"
        For i = 1 To GruppoAvversitaAttive_R.Numero_Gruppi_Infestanti(objParametri_Server)
            If XML_Agenda.GetAttribute("av_gru" & i) <> "" Then
                strGruppiInfestanti += GruppoAvversita_R.AvGruDes_from_AvGruCod(XML_Agenda.GetAttribute("av_gru" & i), Nothing, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strGruppiInfestanti.Length > 27 Then
            strGruppiInfestanti = Left(strGruppiInfestanti, strGruppiInfestanti.Length - 2)
            Filtro += strGruppiInfestanti & "<br>"
        End If

        ''infestanti singole o avversità
        ''(possono essere stati salvati solo per una specie e per un diserbo le prime 
        ''o per un trattamento antiparassitario le seconde)

        Dim strAvversita As String = ""

        'controllo se ho un trattamento antiparassitario o un diserbo
        If TrattamentoAntiparassitario Then
            strAvversita = " <b>Avversita': </b>"
        ElseIf Diserbo Then
            strAvversita = " <b>Infestanti: </b>"
        End If

        For i = 1 To Avversita_R.Numero_Avversita(objParametri_Server)
            If XML_Agenda.GetAttribute("av_cod" & i) <> "" Then
                strAvversita += Avversita_R.AvDes_from_AvCod(XML_Agenda.GetAttribute("av_cod" & i), Nothing, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server) & "; "
            Else
                Exit For
            End If
        Next
        If strAvversita.Length > 20 Then
            strAvversita = Left(strAvversita, strAvversita.Length - 2)
            Filtro += strAvversita & "<br>"
        End If



    End Sub

    '################################################################################
    'dati un'id_agenda ed un'impianto legge i lotti seminati e le rispettive quantità
    Public Function Leggi_Lotti_Seminati(ByRef objServer As System.Web.HttpServerUtility,
                                         ByRef objSession As System.Web.SessionState.HttpSessionState,
                                         ByRef objPage As System.Web.UI.Page,
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Appezza As Integer,
                                         ByVal Id_Reg As Integer,
                                         ByVal Id_Agenda As Integer) As ADODB.Recordset

        Dim strQuery As String
        Dim objCU As Codex_Utility.Sql
        Dim Rs As ADODB.Recordset
        Dim strErr As String

        'Preparo la query
        strQuery = " SELECT DISTINCT Mov_Destinazioni.Qta, UnitaMisura.UDM_SIM, Materie_Prime.Cod_Articolo, Movimenti_dettagli.Lotto "
        strQuery &= " FROM Materie_Prime RIGHT OUTER JOIN "
        strQuery &= " Movimenti_dettagli INNER JOIN "
        strQuery &= " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND "
        strQuery &= " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND "
        strQuery &= " Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND "
        strQuery &= " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov INNER JOIN "
        strQuery &= " UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ON "
        strQuery &= " Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod And Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod  "

        strQuery &= " WHERE Mov_Destinazioni.PIVA = '" & Piva & "'"
        strQuery &= " AND Mov_Destinazioni.Sa_Cod = " & Sa_Cod & " "
        strQuery &= " AND Mov_Destinazioni.appezza = " & Appezza & " "
        strQuery &= " AND Mov_Destinazioni.Id_Destinazione = " & Id_Reg & " "
        strQuery &= " AND Mov_Destinazioni.id_agenda = " & Id_Agenda & " "

        objCU = New Codex_Utility.Sql
        Dim objParametri_Server As New AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
        Rs = objCU.SqlSelect(objParametri_Server.StringaConnessione,
                                objSession("ASG_Connessione_Server"),
                                strQuery,
                                0,
                                strErr)

        If Not IsNothing(strErr) Then

            Return Nothing

        Else

            Return Rs

        End If


    End Function

End Class
