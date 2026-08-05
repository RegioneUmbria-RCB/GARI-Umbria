Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class CatastoOIPomodoroIndustriaNordItalia_XLS
    Inherits System.Web.UI.Page

    Protected WithEvents TableExcel As System.Web.UI.HtmlControls.HtmlTable


    '#Region " Web Form Designer Generated Code "

    '    'This call is required by the Web Form Designer.
    '    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    '    End Sub

    '    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    '    'Do not delete or move it.
    '    Private designerPlaceholderDeclaration As System.Object

    '    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
    '        'CODEGEN: This method call is required by the Web Form Designer
    '        'Do not modify it using the code editor.
    '        InitializeComponent()
    '    End Sub

    '#End Region

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = RisultatoEsportazione.xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            TipiEnumerativi.enum_Security_Attivita.Stampe_Esportatore_Universale, _
        '                            TipiEnumerativi.enum_Security_Operazione.Modifica, _
        '                            strDummy)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

        '----- !!!!!!!!!!! -------------
        'Attivazione forzata provvisoria

        UtenteAbilitato = True
        '----- !!!!!!!!!!! -------------

        If Not UtenteAbilitato Then
            'Response.Redirect("../../../Messaggi/AccessoNegato.htm")
            Page.FindControl("Form1").Controls.Add( _
             New LiteralControl("<script language='javascript'>window.close();</script>"))
        End If


        'Dim scheda, nomefileoutput As String

        'nomefileoutput = Stringa_Decodifica(Request.QueryString("a").ToString, _
        '                               AgroKey_EncoderDecoder, _
        '                               Server)

        'scheda = Stringa_Decodifica(Request.QueryString("t").ToString, _
        '                               AgroKey_EncoderDecoder, _
        '                               Server)

        Try

            '##############################################################
            '#####  leggi i dati   ###############################
            '##############################################################

            Dim Dt As DataTable
            Dt = RecuperaDatiPomodoro()


            '##############################################################
            '#####  Costruisco la tabella   ###############################
            '##############################################################

            Costruisci_Tabella_Excel(Dt)

        Catch ex As Exception
            Dim Riga As New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = 15
            Riga.Cells(0).InnerHtml = "ERRORE: " & ex.Message
            Me.TableExcel.Rows.Add(Riga)
        End Try


    End Sub

    '##################################################################
    Private Function RecuperaDatiPomodoro() As DataTable

        Dim strXmlVariabili As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement
        Dim DT As New DataTable
        Dim Log As String = ""
        Dim msg As String = ""
        Dim Messaggio As String = ""
        Dim num_elementi As Integer
        Dim piva As String
        Dim sa_cod, appezza, id_reg As Integer
        Dim ElencoChiaviImpianto As String = ""
        Dim ChiaveImpianto As String

        Dim Query1_TempTableCreazione As String = ""
        Dim Query2_TempTableIndice As String = ""
        Dim Query3_TempTableFill As String = ""
        Dim Query4_TempTableJoin As String = ""

        strXmlVariabili = Session("strXmlVariabilistampe")
        strXmlVariabili = strXmlVariabili.Replace("<VS", "<VariabiliStampe")

        strXmlVariabili = strXmlVariabili.Replace("s=", "sa_cod=")
        strXmlVariabili = strXmlVariabili.Replace("a=", "appezza=")
        strXmlVariabili = strXmlVariabili.Replace("r=", "id_reg=")
        strXmlVariabili = strXmlVariabili.Replace("p=", "piva=")

        If (Not IsNothing(strXmlVariabili)) AndAlso (strXmlVariabili <> "") Then

            'Carico la stringa xml in un nuovo documento xml
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabili)

            If XmlDoc.HasChildNodes Then

                XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
                If XML_FiltroStampa Is Nothing Then
                    XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")
                End If

                XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                num_elementi = XMLs_VariabiliStampe.Count

                For i = 0 To num_elementi - 1

                    XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                    If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                        msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                    ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                        msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                    End If
                    If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
                        msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                    ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
                        msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                    End If
                    If (IsNothing(XML_VariabiliStampe.GetAttribute("appezza"))) Then
                        msg = "APPEZZA E' NULLO!!!" & vbCrLf
                    ElseIf (CStr(XML_VariabiliStampe.GetAttribute("appezza")) = "") Then
                        msg = "APPEZZA E' NULLO!!!" & vbCrLf
                    End If
                    If (IsNothing(XML_VariabiliStampe.GetAttribute("id_reg"))) Then
                        msg = "ID_REG E' NULLO!!!" & vbCrLf
                    ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_reg")) = "") Then
                        msg = "ID_REG E' NULLO!!!" & vbCrLf
                    End If

                    If msg <> "" Then
                        AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg, Page)
                        Exit Function
                    End If

                    'ricavo gli impianti
                    piva = XML_VariabiliStampe.GetAttribute("piva")
                    sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                    appezza = XML_VariabiliStampe.GetAttribute("appezza")
                    id_reg = XML_VariabiliStampe.GetAttribute("id_reg")

                    'Genero la chiave impianto
                    ChiaveImpianto = piva & "_" & sa_cod & "_" & appezza & "_" & id_reg

                    ElencoChiaviImpianto &= ",'" & ChiaveImpianto & "'"

                    '-------------------------------------------------

                    Query3_TempTableFill &= " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                    Query3_TempTableFill &= " VALUES     (" & Agro_SQL_SaveText_NULL(piva) & "," & Agro_SQL_SaveNum(sa_cod) & "," & Agro_SQL_SaveNum(appezza) & "," & Agro_SQL_SaveNum(id_reg) & ")  " & vbCrLf

                Next 'chiavi impianto


                'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
                ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)

                Dim TabelleTemp_Mode As Integer
                Dim Str_TabelleTemp_RegolaConfronto As String

                TabelleTemp_Mode = Recupera_TabelleTemp_Mode()

                '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO 
                '2: TRAMITE CREATE TABLE
                Select Case TabelleTemp_Mode

                    Case 1
                        Query1_TempTableCreazione &= " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                        Query1_TempTableCreazione &= "   INTO #tempimpianti   "
                        Query1_TempTableCreazione &= "       FROM Reg_Impianti "
                        Query1_TempTableCreazione &= "           WHERE 1 = 0   "
                        Query1_TempTableCreazione &= vbCrLf

                    Case 2

                        'Recupera regola di Confronto: collate sql_LATIN1_GENERAL_cp850_ci_as NOT NULL

                        Str_TabelleTemp_RegolaConfronto = Recupera_Str_TabelleTemp_RegolaConfronto()

                        Query1_TempTableCreazione &= "   CREATE TABLE #tempimpianti (	"
                        Query1_TempTableCreazione &= " [Piva]       [nvarchar] (25) " & Str_TabelleTemp_RegolaConfronto & " NOT NULL ,"
                        Query1_TempTableCreazione &= " [Sa_Cod]     [int] 		    NOT NULL ,"
                        Query1_TempTableCreazione &= " [Appezza]    [int] 	        NOT NULL ,"
                        Query1_TempTableCreazione &= " [Id_Reg]     [int] 		    NOT NULL ,"
                        Query1_TempTableCreazione &= " ) ON [PRIMARY]"
                        Query1_TempTableCreazione &= vbCrLf
                        'Non creo la chiave, perché poi mi da dei problemi
                        'creo l'indice dopo
                        'Query1_TempTableCreazione += " ALTER TABLE #tempimpianti ADD "
                        'Query1_TempTableCreazione += " CONSTRAINT [PK_Temp_Impianti] PRIMARY KEY  CLUSTERED "
                        'Query1_TempTableCreazione += " ("
                        'Query1_TempTableCreazione += " [Piva], "
                        'Query1_TempTableCreazione += " [Sa_Cod], "
                        'Query1_TempTableCreazione += " [Appezza], "
                        'Query1_TempTableCreazione += " [Id_Reg] "
                        'Query1_TempTableCreazione += " )  ON [PRIMARY] "
                        'Query1_TempTableCreazione += vbCrLf

                End Select

                Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "


                Query4_TempTableJoin = Genera_Stringa_Query()

                '/**************************************************
                '         NUOVO METODO CON TABELLA TEMPORANEA  
                Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
                DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione, _
                                                                        Query2_TempTableIndice, _
                                                                        Query3_TempTableFill, _
                                                                        Query4_TempTableJoin, _
                                                                         objParametri_Server.StringaConnessione, _
                                                                         Messaggio)
                '/**************************************************

                If Messaggio <> "" Then
                    Log &= CStr(Date.Now) & "   Errore nella QUERY IMPIANTI: " & Messaggio & vbCrLf & vbCrLf
                End If


            End If 'xml

        End If 'strXmlVariabili


        Return DT

    End Function

    '##################################################################
    Private Function Genera_Stringa_Query() As String

        Dim stbQ As New System.Text.StringBuilder
        Dim Query4_TempTableJoin As String = ""

        stbQ.Append(" ")
        stbQ.Append(" SELECT DISTINCT Reg_Impianti.Piva " & vbCrLf)
        '    stbQ.Append(" ISNULL(Appezzamento.Campo_Cod, -1) AS campo_cod, " & vbCrLf)
        '
        'stbQ.Append("  Reg_Impianti.Sa_Cod, Reg_Impianti.Appezza, Reg_Impianti.Id_Reg " & vbCrLf)
        stbQ.Append(", ")
        stbQ.Append(" ISNULL(Imprese.rag_soc, ' ') AS rag_soc" & vbCrLf)

        'query interna per recuperare il Codice Socio
        '    stbQ.Append(", ")
        '    stbQ.Append(" ISNULL((SELECT TOP 1 val_cod " & vbCrLf)
        '    stbQ.Append("         FROM Imprese_Codici AS Imprese_Codici_Interno " & vbCrLf)
        '    stbQ.Append("         WHERE Imprese_Codici_Interno.piva = Reg_Impianti.piva " & vbCrLf)
        '    stbQ.Append("         AND Imprese_Codici_Interno.id_cod = 1033), ' ') AS Codice_Socio" & vbCrLf)
        '
        ' query interna per recuperare il CUAA
        stbQ.Append(", ")
        stbQ.Append(" ISNULL((SELECT TOP 1 val_cod " & vbCrLf)
        stbQ.Append("         FROM Imprese_Codici AS Imprese_Codici_CUAA " & vbCrLf)
        stbQ.Append("         WHERE Imprese_Codici_CUAA.piva = Reg_Impianti.piva " & vbCrLf)
        stbQ.Append("         AND Imprese_Codici_CUAA.id_cod = 1010), ' ') AS CUAA" & vbCrLf)

        ''controllo selezione indirizzo impresa
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(IndImp.ind_des, ' ') AS imp_ind_des, ISNULL(IndImp.frz_des, ' ') AS imp_frz_des, " & vbCrLf)
        'stbQ.Append(" ISNULL(IndImp.CAP, '') AS imp_cap, ISNULL(IstatImp.LOCALITA, '') AS imp_com_des, ISNULL(IstatImp.COMUNI_PROV, '') AS imp_pro_cod " & vbCrLf)

        ''controllo se sono state selezionate le cooperative padre
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Imprese_1.Piva, ' ') AS PIVA_padre" & vbCrLf)
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Imprese_1.rag_soc, ' ') AS coop_padre" & vbCrLf)

        ''tecnico riferimento
        'stbQ.Append(", ISNULL(    ")
        'stbQ.Append(" (SELECT  TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS tecnico  " & vbCrLf)
        'stbQ.Append(" FROM    Imprese_Codici " & vbCrLf)
        'stbQ.Append(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod   " & vbCrLf)
        'stbQ.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva " & vbCrLf)
        'stbQ.Append(" WHERE  (Imprese_Codici.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Codici.Id_Cod = " + CStr(enum_CodiciAnagrafe.Tecnico) + ") " & vbCrLf)
        'stbQ.Append(" AND     (UtentiXImprese.[USER] = '" & CStr(Session("ASG_SuperUser_CodFiscale")) & "') " & vbCrLf)
        'stbQ.Append(" )   ")
        'stbQ.Append(", '') AS tecnico " & vbCrLf)

        stbQ.Append(", ")
        stbQ.Append(" ISNULL(IndCentro.ind_des, ' ') AS cen_ind_des, ISNULL(IndCentro.frz_des, ' ') AS cen_frz_des, " & vbCrLf)
        stbQ.Append(" ISNULL(IndCentro.CAP, '') AS cen_cap, ISNULL(ISTATCentro.LOCALITA, '') AS cen_com_des, ISNULL(ISTATCentro.COMUNI_PROV, '') AS cen_pro_cod " & vbCrLf)
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(IndCentro.pro_cod_istat, ' ') AS pro_cod_istat, ISNULL(IndCentro.com_cod_istat, ' ') AS com_cod_istat" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Centri_Aziendali.sa_cod, 0) AS sa_cod" & vbCrLf)
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome" & vbCrLf)

        stbQ.Append(", ")
        stbQ.Append(" ISNULL(Campi.Campo_Des, ' ') AS campo_des" & vbCrLf)

        stbQ.Append(", ")
        stbQ.Append(" ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Appezzamento.Sup_App, 0) AS sup_app, ISNULL(Appezzamento.Validita_Inizio, '01/01/1900') AS Inizio_Appezza, ISNULL(Appezzamento.Validita_Fine, '31/12/2100') AS Fine_Appezza, " & vbCrLf)
        'stbQ.Append(" (SELECT TOP 1 val_cod FROM [Appezzamento_Codici] WHERE Piva = Appezzamento.Piva AND Sa_Cod = Appezzamento.Sa_Cod AND Appezza = Appezzamento.Appezza AND appezzamento_codici.id_cod=1018) As [Metodo di Produzione] " & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(GruppoVegetale.Gru_Des, ' ') AS gru_des" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(SpecieVegetali.Veg_Cod, ' ') AS Veg_Cod" & vbCrLf)
        stbQ.Append(", ")
        stbQ.Append(" ISNULL(SpecieVegetali.Veg_Des, ' ') AS veg_des" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Cultivar.Cul_Cod, ' ') AS cul_cod" & vbCrLf)
        stbQ.Append(", ")
        stbQ.Append(" ISNULL(Cultivar.Cul_Des, ' ') AS cul_des" & vbCrLf)

        'stbQ.Append(", ")
        ' stbQ.Append(" ISNULL(GruppoVarietale.Grva_Des,ISNULL( (select grva_des + ' -- Ibrido ' from GruppoVarietale where GruppoVarietale.Grva_cod = (0- Reg_Impianti.GRVA_Cod_VEG)), ' ')) AS grva_des " & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(( SELECT TOP 1 Codici_Anagrafe.Descrizione " & vbCrLf)
        'stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
        'stbQ.Append(" INNER JOIN Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice " & vbCrLf)
        'stbQ.Append(" WHERE  (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND   (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND   (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND   (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND   (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
        'stbQ.Append(" AND   (Codici_Anagrafe.gruppo = 'TERRENO') " & vbCrLf)
        'stbQ.Append(" ) , '') AS dest_uso " & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Copertura.Cop_Des, ' ') AS cop_des" & vbCrLf)

        stbQ.Append(", ISNULL(Reg_Impianti.Sup_Imp, 0.0000) AS sup_imp" & vbCrLf)
        'stbQ.Append(", ")
        'stbQ.Append(" CAST(Reg_Impianti.Appezza as varchar(10)) + '_' + CAST(Reg_Impianti.ID_Reg as varchar(10)) as id_Impianto" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Inizio, 103), ' ') AS inizio_impianto, " & vbCrLf)
        'stbQ.Append(" ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), ' ') AS fine_impianto " & vbCrLf)

        '' query interna per recuperare la data di trapianto/semina
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL( (SELECT TOP 1 CONVERT(varchar(50), Agenda.lav_cod) + '|' + CONVERT(varchar(10), (Agenda.Validita_Inizio), 103)" & vbCrLf)
        'stbQ.Append("         FROM Agenda " & vbCrLf)
        'stbQ.Append("         INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva " & vbCrLf)
        'stbQ.Append("         AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
        'stbQ.Append("         AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
        'stbQ.Append("         INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva " & vbCrLf)
        'stbQ.Append("         AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod " & vbCrLf)
        'stbQ.Append("         AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda " & vbCrLf)
        'stbQ.Append("         AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
        'stbQ.Append("         INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva " & vbCrLf)
        'stbQ.Append("         AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod " & vbCrLf)
        'stbQ.Append("         AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda " & vbCrLf)
        'stbQ.Append("         AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov " & vbCrLf)
        'stbQ.Append("         AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
        'stbQ.Append("         WHERE (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) " & vbCrLf)
        'stbQ.Append("         AND Movimenti.Cau_Mov = '2300' " & vbCrLf)
        'stbQ.Append("         AND Mov_Destinazioni.Piva = Reg_Impianti.Piva " & vbCrLf)
        'stbQ.Append("         AND Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod " & vbCrLf)
        'stbQ.Append("         AND Mov_Destinazioni.appezza = Reg_Impianti.Appezza " & vbCrLf)
        'stbQ.Append("         AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg " & vbCrLf)
        'stbQ.Append("         ORDER BY Agenda.Validita_Inizio), '-1|01/01/1900') AS LavCod_Data_Semina, -1 AS lav_cod_imp, '01/01/1900' AS data_semina " & vbCrLf)

        ''controllo se sono stati selezionati forma allevamento e dettaglio specie personalizzato
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(FormeAllevamento.Foral_Des, ' ') AS foral_des " & vbCrLf)

        'stbQ.Append(", ISNULL(( SELECT     TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des " & vbCrLf)
        'stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
        'stbQ.Append(" INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod" & vbCrLf)
        'stbQ.Append(" WHERE  (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) + ") " & vbCrLf)
        'stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(CStr(Session("ASG_SuperUser_CodFiscale"))) & ") " & vbCrLf)
        'stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 2) " & vbCrLf)
        'stbQ.Append(" ) , '') AS dett_specie_pers " & vbCrLf)

        ''controllo se sono stati selezionati portinnesto e imp irrigazione
        'stbQ.Append(", ISNULL(Portinnesti.Port_Des, ' ') AS port_des " & vbCrLf)
        'stbQ.Append(", ISNULL(ImpiantiIrrigazioni.Imp_Des, ' ') AS imp_des " & vbCrLf)

        ''se è stato selezionato il sesto d'impianto
        'stbQ.Append(", ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod " & vbCrLf)
        'stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
        'stbQ.Append(" WHERE   (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_TraFila_Maschio) + ") " & vbCrLf)
        'stbQ.Append(" ) , 0) AS tra_fila_maschio " & vbCrLf)

        'stbQ.Append(", ISNULL(( SELECT TOP 1  Reg_Impianti_Codici.val_cod " & vbCrLf)
        'stbQ.Append(" FROM     Reg_Impianti_Codici " & vbCrLf)
        'stbQ.Append(" WHERE   (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Progetto_Cod = 0) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_SuFila_Maschio) + ") " & vbCrLf)
        'stbQ.Append(" ) , 0) AS su_fila_maschio " & vbCrLf)


        ''controllo se sono stati selezionati piante e resa
        'stbQ.Append(", 0 AS P_HA, 0 AS P_Tot, 0 AS resa_prevista " & vbCrLf)

        'stbQ.Append(", ISNULL(( SELECT TOP 1 CONVERT(varchar(250), ISNULL(Imprese_Progetti.P_HA, 0) ) + '|' + CONVERT(varchar(250), ISNULL(Imprese_Progetti.Produzione_Prevista, 0)) " & vbCrLf)
        'stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
        'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" ) , '0|0') AS piante_resa " & vbCrLf)

        'controllo se sono stati selezionati lotto e date distinta
        'stbQ.Append(", ISNULL(( SELECT    TOP 1 Imprese_Progetti.Progetto_Nome + '|' + CONVERT(varchar(10), Imprese_Progetti.Validita_Inizio, 120) + '|' + CONVERT(varchar(10), Imprese_Progetti.Validita_Fine, 120) " & vbCrLf)
        'stbQ.Append("             FROM    Imprese_Progetti " & vbCrLf)
        'stbQ.Append("             WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" ) , '|01/01/1900|31/12/2100') AS date_distinta, '' AS lotto_distinta, '' AS inizio_distinta, '' AS fine_distinta " & vbCrLf)

        ''controllo se sono stati selezionati il capitolato privato 
        'stbQ.Append(", ISNULL(( SELECT     TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des " & vbCrLf)
        'stbQ.Append(" FROM         Imprese_Progetti " & vbCrLf)
        'stbQ.Append(" INNER JOIN Reg_Impianti_Codici ON Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod AND  " & vbCrLf)
        'stbQ.Append(" Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg AND  " & vbCrLf)
        'stbQ.Append(" Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
        'stbQ.Append(" INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod" & vbCrLf)
        'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND (Reg_Impianti_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato) + ") " & vbCrLf)
        'stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Piva_SuperUser = " & Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) & ") " & vbCrLf)
        'stbQ.Append(" AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 1) " & vbCrLf)
        'stbQ.Append(" AND (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" AND (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" ) , '') AS capitolato_privato " & vbCrLf)

        stbQ.Append(", ISNULL(( SELECT  TOP 1  " & vbCrLf) 'Regolamenti.Reg_Des
        stbQ.Append("           CASE WHEN  Imprese_Progetti.Regolamento_Cod= 4 then 'BIO' " & vbCrLf)
        stbQ.Append("            ELSE '' END " & vbCrLf)
        stbQ.Append("             FROM  Imprese_Progetti " & vbCrLf)
        'stbQ.Append("             INNER JOIN  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod " & vbCrLf)
        stbQ.Append("             WHERE (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        stbQ.Append("             AND   (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        stbQ.Append("             AND   (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        stbQ.Append("             AND   (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        stbQ.Append("             ORDER BY Imprese_Progetti.validita_fine DESC " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append("             AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        stbQ.Append(" ) , ' ') AS Regolamento " & vbCrLf)


        ''controllo se sono stati selezionati organismo referente e magazzino conferimento
        ''organismo referente
        'stbQ.Append(", ISNULL(    ")
        'stbQ.Append(" (SELECT  TOP 1 Contatti.rag_soc AS coop_referente  " & vbCrLf)
        'stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
        'stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
        'stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
        'stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
        'stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
        'stbQ.Append(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Reg_Impianti_Codici.val_cod   " & vbCrLf)
        'stbQ.Append(" INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva " & vbCrLf)
        'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") " & vbCrLf)
        'stbQ.Append(" AND     (UtentiXImprese.[USER] = '" & CStr(Session("ASG_SuperUser_CodFiscale")) & "') " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" )   ")
        'stbQ.Append(", '') AS org_referente " & vbCrLf)

        ''magazzino conferimento
        'stbQ.Append(", ISNULL(    " & vbCrLf)
        'stbQ.Append("( SELECT TOP 1 Fabbricati.Fabbricato_des " & vbCrLf)
        'stbQ.Append(" FROM    Imprese_Progetti " & vbCrLf)
        'stbQ.Append(" INNER JOIN    Reg_Impianti_Codici ON " & vbCrLf)
        'stbQ.Append(" Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod " & vbCrLf)
        'stbQ.Append(" AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   " & vbCrLf)
        'stbQ.Append(" AND  Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    " & vbCrLf)
        'stbQ.Append(" INNER JOIN Fabbricati ON CONVERT(varchar(50),Fabbricati.Fabbricato_Cod) +'|'+ CONVERT(varchar(50),FABBRICATI.sa_COD) +'|'+ Fabbricati.piva = Reg_Impianti_Codici.val_cod  " & vbCrLf)
        'stbQ.Append("    " & vbCrLf)
        'stbQ.Append(" WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza) " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) " & vbCrLf)
        'stbQ.Append(" AND     (Reg_Impianti_Codici.Id_cod = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) + ") " & vbCrLf)
        ''stbQ.Append(" AND     Fabbricati.Piva = " & vbCrLf)
        ''stbQ.Append("                               ISNULL( (    " & vbCrLf)
        ''stbQ.Append("                                   SELECT  TOP 1 val_cod AS piva_org " & vbCrLf)
        ''stbQ.Append("                                   FROM    Reg_Impianti_Codici RIC " & vbCrLf)
        ''stbQ.Append("                                   WHERE  (RIC.Piva = Imprese_Progetti.Piva) " & vbCrLf)
        ''stbQ.Append("                                   AND     (RIC.Sa_Cod = Imprese_Progetti.sa_cod) " & vbCrLf)
        ''stbQ.Append("                                   AND     (RIC.Appezza  = Imprese_Progetti.Appezza) " & vbCrLf)
        ''stbQ.Append("                                   AND     (RIC.Id_Reg = Imprese_Progetti.Id_Reg) " & vbCrLf)
        ''stbQ.Append("                                   AND     (RIC.Progetto_Cod = Imprese_Progetti.Progetto_Cod) " & vbCrLf)
        ''stbQ.Append("                                   AND     (RIC.Id_cod = " + CStr(enum_CodiciAnagrafe.Impianto_Cooperativa) + ") ), '')  " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Me.Txt_Data.Text) & ") " & vbCrLf)
        'stbQ.Append(" )   ")
        'stbQ.Append(", ' ') AS magazzino_conf " & vbCrLf)


        ''controllo se sono state selezionate le particelle
        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(Lista_Regioni.Regione_Des, '') AS part_regione, ISNULL(ISTATParticelle.COMUNI_PROV, '') AS part_pro_cod, ISNULL(ISTATParticelle.LOCALITA, '') AS part_com_des, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.PROV, ' ') AS PROV, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.COM, ' ')AS COM, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.SEZIONE, ' ') AS SEZIONE, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.FOGLIO, -1) AS FOGLIO, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.NUMERO, -1) AS NUMERO, " & vbCrLf)
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.SUBALTERNO, ' ') AS SUBALTERNO, " & vbCrLf)
        'stbQ.Append(" ISNULL(ParticelleCatastali.ETTARI, -1) AS ETTARI, " & vbCrLf)
        'stbQ.Append(" ISNULL(ParticelleCatastali.[ARE], -1) AS ARE, " & vbCrLf)
        'stbQ.Append(" ISNULL(ParticelleCatastali.CENTIARE, -1) AS CENTIARE" & vbCrLf)

        'stbQ.Append(", ")
        'stbQ.Append(" ISNULL(AppezzamentiXParticelle.AREA, -1.0000)AS AREA" & vbCrLf)

        stbQ.Append(" , ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cod_Varieta, '') AS Cod_Varieta_OI_NordEst " & vbCrLf)
        stbQ.Append(" , ISNULL(Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Desc_Varieta, '') AS Desc_Varieta_OI_NordEst " & vbCrLf)

        ' stbQ.Append( " Reg_Impianti.CUL_COD, Reg_Impianti.GRFI_COD" 'non serve al momento

        'fine SELECT
        stbQ.Append(" ")
        stbQ.Append(" ")
        stbQ.Append(" FROM Reg_Impianti ")

        'campo cod, app nome, campo nome, data semina?
        stbQ.Append(" INNER JOIN Appezzamento ON Reg_Impianti.Appezza = Appezzamento.Appezza " & vbCrLf)
        stbQ.Append(" AND Reg_Impianti.Sa_Cod = Appezzamento.Sa_Cod " & vbCrLf)
        stbQ.Append(" AND Reg_Impianti.Piva = Appezzamento.Piva " & vbCrLf)

        stbQ.Append(" LEFT OUTER JOIN  Campi ON Appezzamento.Piva = Campi.Piva " & vbCrLf)
        stbQ.Append(" AND Appezzamento.Sa_Cod = Campi.Sa_Cod " & vbCrLf)
        stbQ.Append(" AND Appezzamento.Campo_Cod = Campi.Campo_Cod " & vbCrLf)

        'indirizzo centro, nome centro, codici istat
        stbQ.Append(" INNER JOIN Centri_Aziendali ON Reg_Impianti.Sa_Cod = Centri_Aziendali.sa_cod " & vbCrLf)
        stbQ.Append(" AND Reg_Impianti.Piva = Centri_Aziendali.Piva " & vbCrLf)

        stbQ.Append(" INNER JOIN Imprese ON Reg_Impianti.Piva = Imprese.Piva " & vbCrLf)

        'indirizzo centro, codici istat
        stbQ.Append(" INNER JOIN CentrixIndirizzi " & vbCrLf)
        stbQ.Append(" ON Centri_Aziendali.Piva = CentrixIndirizzi.Piva " & vbCrLf)
        stbQ.Append(" AND Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod " & vbCrLf)
        stbQ.Append(" INNER JOIN Indirizzi IndCentro ON CentrixIndirizzi.cod_indirizzo = IndCentro.cod_indirizzo " & vbCrLf)
        stbQ.Append(" INNER JOIN ISTAT IstatCentro ON IndCentro.pro_cod_istat = IstatCentro.PROV AND IndCentro.com_cod_istat = IstatCentro.COM " & vbCrLf)

        ''indirizzo impresa, codici istat
        'stbQ.Append(" INNER JOIN ImpresexIndirizzi " & vbCrLf)
        'stbQ.Append(" ON Reg_Impianti.Piva = ImpresexIndirizzi.Piva " & vbCrLf)
        'stbQ.Append(" INNER JOIN Indirizzi IndImp ON ImpresexIndirizzi.cod_indirizzo = IndImp.cod_indirizzo " & vbCrLf)
        'stbQ.Append(" INNER JOIN ISTAT IstatImp ON IndImp.pro_cod_istat = IstatImp.PROV AND IndImp.com_cod_istat = IstatImp.COM " & vbCrLf)+

        ' coop padre, piva padre
        'stbQ.Append(" LEFT OUTER JOIN Imprese Imprese_1 " & vbCrLf)
        'stbQ.Append(" INNER JOIN GerarchiaImprese ON Imprese_1.Piva = GerarchiaImprese.Padre " & vbCrLf)
        'stbQ.Append(" ON Imprese.Piva = GerarchiaImprese.Figlio " & vbCrLf)

        'stbQ.Append(" LEFT OUTER JOIN AppezzamentiXParticelle " & vbCrLf)
        'stbQ.Append(" ON Reg_Impianti.Piva = AppezzamentiXParticelle.Piva " & vbCrLf)
        'stbQ.Append(" AND Reg_Impianti.Sa_Cod = AppezzamentiXParticelle.Sa_Cod " & vbCrLf)
        'stbQ.Append(" AND Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza " & vbCrLf)

        'stbQ.Append(" LEFT OUTER JOIN ParticelleCatastali " & vbCrLf)
        'stbQ.Append(" ON ParticelleCatastali.PROV = AppezzamentiXParticelle.PROV " & vbCrLf)
        'stbQ.Append(" AND ParticelleCatastali.COM = AppezzamentiXParticelle.COM " & vbCrLf)
        'stbQ.Append(" AND ParticelleCatastali.SEZIONE = AppezzamentiXParticelle.SEZIONE " & vbCrLf)
        'stbQ.Append(" AND ParticelleCatastali.FOGLIO = AppezzamentiXParticelle.FOGLIO " & vbCrLf)
        'stbQ.Append(" AND ParticelleCatastali.NUMERO = AppezzamentiXParticelle.NUMERO " & vbCrLf)
        'stbQ.Append(" AND ParticelleCatastali.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO " & vbCrLf)
        ''Decodifica regione + istat particelle
        'stbQ.Append(" LEFT OUTER JOIN ISTAT IstatParticelle ON AppezzamentiXParticelle.PROV = ISTATParticelle.PROV AND AppezzamentiXParticelle.COM = ISTATParticelle.COM " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN Lista_Province ON Lista_Province.SIGLA = ISTATParticelle.Comuni_Prov " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN Lista_Regioni ON Lista_Regioni.REG = Lista_Province.REG " & vbCrLf)

        'stbQ.Append(" LEFT OUTER JOIN Copertura ON Reg_Impianti.COP_COD = Copertura.Cop_Cod " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN Portinnesti ON Reg_Impianti.Port_COD = Portinnesti.Port_Cod " & vbCrLf)
        'stbQ.Append(" LEFT OUTER JOIN ImpiantiIrrigazioni ON Reg_Impianti.Imp_COD = ImpiantiIrrigazioni.Imp_Cod " & vbCrLf)
        ' stbQ.Append(" LEFT OUTER JOIN GruppoVegetale ON SpecieVegetali.Gru_Cod = GruppoVegetale.Gru_Cod " & vbCrLf)
        '        stbQ.Append(" LEFT OUTER JOIN  FormeAllevamento ON Reg_Impianti.FORAL_COD = FormeAllevamento.Foral_Cod " & vbCrLf)

        stbQ.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
        stbQ.Append(" INNER JOIN SpecieVegetali " & vbCrLf)
        stbQ.Append(" ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)

        stbQ.Append(" LEFT OUTER JOIN Codifica_Varieta_OIPomodorodaIndustriaNordItalia ON Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cul_Cod_Gias = Cultivar.Cul_Cod " & vbCrLf)

        stbQ.Append(" ")
        stbQ.Append(" ")
        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA
        '           --> AGGIUNGO QUESTA PARTE:
        stbQ.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
        stbQ.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
        '/**************************************************
        stbQ.Append(" ")
        stbQ.Append(" ")
        ' stbQ.Append( " AND CentrixIndirizzi.Tipo_Indirizzo = 1 " & vbCrLf)
        stbQ.Append(" WHERE CentrixIndirizzi.Tipo_Indirizzo = 1 " & vbCrLf)

        'ORDINAMENTO
        stbQ.Append(" ORDER BY rag_soc ASC " & vbCrLf)

        stbQ.Append(vbCrLf)

        '/**************************************************
        Query4_TempTableJoin = stbQ.ToString

        stbQ = Nothing
        '/**************************************************

        Return Query4_TempTableJoin

    End Function


    '##################################################################
    Private Sub Costruisci_Tabella_Excel(ByVal Dt_Finale As DataTable)

        Dim i, j, k As Integer
        Dim Riga As HtmlTableRow

        'AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Me.TableExcel.Rows(0).Cells(0), 2, "", "", "Yellow", "left", "middle")

        'Select Case scheda

        '    Case "impianti"
        '        Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Impianti"
        '    Case "imprese"
        '        Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Imprese"
        '    Case "centri"
        '        Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Centri Aziendali"
        '    Case "agenda"
        '        Me.TableExcel.Rows(0).Cells(0).InnerHtml = "Esportazione Operazioni d'Agenda"

        'End Select

        'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-weight") = "bold"
        'Me.TableExcel.Rows(0).Cells(0).Style.Item("font-size") = "18px"
        'Me.TableExcel.Rows(0).Cells(0).Style.Item("vertical-align") = "middle"
        'Me.TableExcel.Rows(0).Cells(0).Style.Item("border-top-width") = "1px"

        If Not IsNothing(Dt_Finale) Then

            Me.TableExcel.Rows(0).Cells(0).ColSpan = CInt(Dt_Finale.Columns.Count)

            '------------------------------------------
            '------------- INTESTAZIONE ---------------
            '------------------------------------------
            Riga = New HtmlTableRow

            For k = 0 To Dt_Finale.Columns.Count - 1

                Riga.Cells.Add(New HtmlTableCell)
                AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(k),
                                                                          2, "", "",
                                                                          "Gainsboro",
                                                                          "center", "middle")
                Riga.Cells(k).Style.Item("vertical-align") = "middle"
                Riga.Cells(k).Style.Item("font-weight") = "bold"
                Riga.Cells(k).Style.Item("font-size") = "12px"
                'Riga.Cells(k).Height = "26"

                'Riga.Cells(k).InnerHtml = Dt_Finale.Rows(CInt(Dt_Finale.Rows.Count - 1)).Item(k)
                Riga.Cells(k).InnerHtml = Dt_Finale.Columns.Item(k).Caption

                Select Case CStr(Riga.Cells(k).InnerHtml).ToLower

                    Case "PIVA", "piva"
                        Riga.Cells(k).InnerHtml = "Partita IVA"
                    Case "SA_COD", "sa_cod"
                        Riga.Cells(k).InnerHtml = "Codice Centro"
                    Case "campo_cod"
                        Riga.Cells(k).InnerHtml = "Codice Campo"
                    Case "APPEZZA", "appezza"
                        Riga.Cells(k).InnerHtml = "Codice<br/>Appezzamento"
                    Case "ID_REG", "id_dest"
                        Riga.Cells(k).InnerHtml = "Codice Impianto"
                    Case "rag_soc"
                        Riga.Cells(k).InnerHtml = "Ragione Sociale"
                    Case "Codice_Socio", "codice_socio"
                        Riga.Cells(k).InnerHtml = "Codice Socio"
                    Case "coop_padre"
                        Riga.Cells(k).InnerHtml = "Impresa Padre"
                    Case "PIVA_padre", "piva_padre"
                        Riga.Cells(k).InnerHtml = "Partita IVA<br/>Impresa Padre"
                    Case "imp_ind_des"
                        Riga.Cells(k).InnerHtml = "Indirizzo<br/>Impresa"
                    Case "imp_frz_des"
                        Riga.Cells(k).InnerHtml = "Frazione<br/>Impresa"
                    Case "imp_cap"
                        Riga.Cells(k).InnerHtml = "CAP<br/>Impresa"
                    Case "imp_com_des"
                        Riga.Cells(k).InnerHtml = "Comune<br/>Impresa"
                    Case "imp_pro_cod"
                        Riga.Cells(k).InnerHtml = "Prov<br/>Impresa"
                    Case "cen_ind_des"
                        Riga.Cells(k).InnerHtml = "Indirizzo<br/>Campo"
                    Case "cen_frz_des"
                        Riga.Cells(k).InnerHtml = "Frazione<br/>Campo"
                    Case "cen_cap"
                        Riga.Cells(k).InnerHtml = "CAP<br/>Campo"
                    Case "cen_com_des"
                        Riga.Cells(k).InnerHtml = "Comune<br/>Campo"
                    Case "cen_pro_cod"
                        Riga.Cells(k).InnerHtml = "Prov<br/>Campo"
                    Case "pro_cod_istat"
                        Riga.Cells(k).InnerHtml = "Prov ISTAT<br/>Campo"
                    Case "com_cod_istat"
                        Riga.Cells(k).InnerHtml = "Com ISTAT<br/>Campo"
                    Case "sa_nome"
                        Riga.Cells(k).InnerHtml = "Nome Centro Az."
                    Case "campo_des"
                        Riga.Cells(k).InnerHtml = "Nome Campo"
                    Case "app_nome"
                        Riga.Cells(k).InnerHtml = "Nome<br/>Appezzamento"
                    Case "sup_app"
                        Riga.Cells(k).InnerHtml = "Superficie<br/>Appezzamento"
                    Case "gru_cod"
                        Riga.Cells(k).InnerHtml = "Codice<br />Gruppo Vegetale"
                    Case "gru_des"
                        Riga.Cells(k).InnerHtml = "Gruppo Vegetale"
                    Case "veg_cod"
                        Riga.Cells(k).InnerHtml = "Codice<br/>Specie Vegetale"
                    Case "veg_des"
                        Riga.Cells(k).InnerHtml = "Specie Vegetale"
                    Case "cul_cod"
                        Riga.Cells(k).InnerHtml = "Codice Varieta'"
                    Case "cul_des"
                        Riga.Cells(k).InnerHtml = "Varieta'"
                    Case "grva_des"
                        Riga.Cells(k).InnerHtml = "Tipologia Varietale"
                    Case "grfi_des"
                        Riga.Cells(k).InnerHtml = "Finalita'"
                    Case "cop_des"
                        Riga.Cells(k).InnerHtml = "Tipo Copertura"
                    Case "sup_imp"
                        Riga.Cells(k).InnerHtml = "Superficie<br />Impianto [Ha]"
                    Case "inizio_impianto"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Impianto"
                    Case "fine_impianto"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Impianto"
                    Case "inizio_appezza"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Appezzamento"
                    Case "fine_appezza"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Appezzamento"
                    Case "lav_cod_imp"
                        Riga.Cells(k).InnerHtml = "Operazione<br/>Semina / Trapianto"
                    Case "lav_cod"
                        Riga.Cells(k).InnerHtml = "Codice Operazione"
                    Case "cau_mov"
                        Riga.Cells(k).InnerHtml = "Causale Movimento"
                    Case "id_agenda"
                        Riga.Cells(k).InnerHtml = "Codice Agenda"
                    Case "des_lib"
                        Riga.Cells(k).InnerHtml = "Descrizione Operazione"
                    Case "gruppo_operazione"
                        Riga.Cells(k).InnerHtml = "Gruppo Operazione"
                    Case "data_operazione"
                        Riga.Cells(k).InnerHtml = "Data Operazione"
                    Case "data_semina"
                        Riga.Cells(k).InnerHtml = "Data<br/>Semina / Trapianto"
                    Case "part_regione"
                        Riga.Cells(k).InnerHtml = "Regione"
                    Case "part_pro_cod"
                        Riga.Cells(k).InnerHtml = "Cod<br/>Prov"
                    Case "part_com_des"
                        Riga.Cells(k).InnerHtml = "Descr<br/>Comune"
                    Case "PROV", "prov"
                        Riga.Cells(k).InnerHtml = "Provincia"
                    Case "COM", "com"
                        Riga.Cells(k).InnerHtml = "Comune"
                    Case "SEZIONE", "sezione"
                        Riga.Cells(k).InnerHtml = "Sezione"
                    Case "FOGLIO", "foglio"
                        Riga.Cells(k).InnerHtml = "Foglio"
                    Case "NUMERO", "numero"
                        Riga.Cells(k).InnerHtml = "Numero"
                    Case "SUBALTERNO", "subalterno"
                        Riga.Cells(k).InnerHtml = "Subalterno"
                    Case "AREA", "area"
                        Riga.Cells(k).InnerHtml = "Superficie d'Intersezione<br/>con Particella [Ha]"
                    Case "rappr_legale"
                        Riga.Cells(k).InnerHtml = "Rappresentante Legale"
                    Case "CF_legale", "cf_legale"
                        Riga.Cells(k).InnerHtml = "Codice Fiscale<br/>Rappresentante Legale"
                    Case "com_legale"
                        Riga.Cells(k).InnerHtml = "Comune Nascita<br/>Rappresentante Legale"
                    Case "pro_legale"
                        Riga.Cells(k).InnerHtml = "Provincia Nascita<br/>Rappresentante Legale"
                    Case "tipo_impresa"
                        Riga.Cells(k).InnerHtml = "Tipo Impresa"
                    Case "tipo_centro"
                        Riga.Cells(k).InnerHtml = "Tipo Centro"
                    Case "inizio_impresa"
                        Riga.Cells(k).InnerHtml = "Data Inizio Impresa"
                    Case "fine_impresa"
                        Riga.Cells(k).InnerHtml = "Data Fine Impresa"
                    Case "inizio_centro"
                        Riga.Cells(k).InnerHtml = "Data Inizio Centro"
                    Case "fine_centro"
                        Riga.Cells(k).InnerHtml = "Data Fine Centro"
                    Case "TitoloPossesso", "titolopossesso"
                        Riga.Cells(k).InnerHtml = "Titolo Possesso<br/>Particella"
                    Case "possesso_centro"
                        Riga.Cells(k).InnerHtml = "Titolo Possesso<br/>Centro"
                    Case "tipo_attivita"
                        Riga.Cells(k).InnerHtml = "Tipo Attivita'<br/>"
                    Case "dal"
                        Riga.Cells(k).InnerHtml = "Data Inizio Possesso<br />Particella"
                    Case "al"
                        Riga.Cells(k).InnerHtml = "Data Fine Possesso<br />Particella"
                    Case "ETTARI", "ettari"
                        Riga.Cells(k).InnerHtml = "Superficie [HA]<br />Particella"
                    Case "ARE", "are"
                        Riga.Cells(k).InnerHtml = "Superficie [AA]<br />Particella"
                    Case "CENTIARE", "centiare"
                        Riga.Cells(k).InnerHtml = "Superficie [CA]<br />Particella"
                    Case "cod_ote"
                        Riga.Cells(k).InnerHtml = "Orientamento<br/>Tenico Economico"
                    Case "cod_operatore"
                        Riga.Cells(k).InnerHtml = "Codice Operatore"
                    Case "cod_zoo"
                        Riga.Cells(k).InnerHtml = "Codice Zooprofilattico"
                    Case "cod_cerpl"
                        Riga.Cells(k).InnerHtml = "Codice CERPL"
                    Case "cod_aua"
                        Riga.Cells(k).InnerHtml = "Codice AUA"
                    Case "cod_ausl"
                        Riga.Cells(k).InnerHtml = "Codice AUSL"
                    Case "cod_cnal"
                        Riga.Cells(k).InnerHtml = "Codice CNAL"
                    Case "fabbricato"
                        Riga.Cells(k).InnerHtml = "Nome Fabbricato"
                    Case "tipo_fabbricato"
                        Riga.Cells(k).InnerHtml = "Tipo Fabbricato"
                    Case "sup_totale"
                        Riga.Cells(k).InnerHtml = "Superficie Totale [Ha]<br/>(somma particelle)"
                    Case "sup_sau"
                        Riga.Cells(k).InnerHtml = "SAU Totale [Ha]<br/>(somma appezzamenti)"
                    Case "sup_tara"
                        Riga.Cells(k).InnerHtml = "Tara [Ha]<br/>(Sup Totale - SAU Totale)"
                    Case "sau_convenz"
                        Riga.Cells(k).InnerHtml = "SAU Convenzionale [Ha]"
                    Case "sau_convers"
                        Riga.Cells(k).InnerHtml = "SAU in Conversione [Ha]"
                    Case "sau_bio"
                        Riga.Cells(k).InnerHtml = "SAU Biologico [Ha]"
                    Case "sup_bosco"
                        Riga.Cells(k).InnerHtml = "Superficie Bosco [Ha]"
                    Case "sup_prato"
                        Riga.Cells(k).InnerHtml = "Superficie Prato [Ha]"
                    Case "mappa"
                        Riga.Cells(k).InnerHtml = "Mappe associate al<br/>Centro Aziendale"
                    Case "organismi"
                        Riga.Cells(k).InnerHtml = "Organismi di Controllo Biologico"
                    Case "capitolato_privato"
                        Riga.Cells(k).InnerHtml = "Capitolato<br/>Privato"
                    Case "dett_specie_pers"
                        Riga.Cells(k).InnerHtml = "Dettaglio<br/>Specie<br/>Personalizzato"
                    Case "foral_des"
                        Riga.Cells(k).InnerHtml = "Forma<br/>Allevamento"
                    Case "resa_prevista"
                        Riga.Cells(k).InnerHtml = "Resa<br/>Prevista [Kg]"
                    Case "P_HA", "p_ha"
                        Riga.Cells(k).InnerHtml = "Num.<br/>Piante/Ha"
                    Case "P_Tot", "p_tot"
                        Riga.Cells(k).InnerHtml = "Num.<br/>Piante Tot"
                    Case "tra_fila_maschio"
                        Riga.Cells(k).InnerHtml = "Distanza<br/>Tra Fila"
                    Case "su_fila_maschio"
                        Riga.Cells(k).InnerHtml = "Distanza<br/>Su Fila"
                    Case "coop_referente"
                        Riga.Cells(k).InnerHtml = "Organismo<br/>Referente"
                    Case "lotto_distinta"
                        Riga.Cells(k).InnerHtml = "Lotto<br/>Distinta"
                    Case "inizio_distinta"
                        Riga.Cells(k).InnerHtml = "Data Inizio<br/>Distinta"
                    Case "fine_distinta"
                        Riga.Cells(k).InnerHtml = "Data Fine<br/>Distinta"
                    Case "Bloccato", "bloccato"
                        Riga.Cells(k).InnerHtml = "Operazione<br/>Bloccata"
                    Case "Data_Bloccato", "data_bloccato"
                        Riga.Cells(k).InnerHtml = "Data Blocco<br/>Operazione"
                    Case "Tecnico_Blocco", "tecnico_blocco"
                        Riga.Cells(k).InnerHtml = "Tecnico Blocco<br/>Operazione"
                    Case "Categoria", "categoria"
                        Riga.Cells(k).InnerHtml = "Categoria Prodotto"
                    Case "Prodotto", "prodotto"
                        Riga.Cells(k).InnerHtml = "Prodotto / Materia Prima"
                    Case "Udm_Des", "udm_des"
                        Riga.Cells(k).InnerHtml = "Unita' di<br/>Misura"
                    Case "Qta", "qta"
                        Riga.Cells(k).InnerHtml = "Quantita'"
                    Case "tecnico"
                        Riga.Cells(k).InnerHtml = "Tecnico di<br/>Riferimento"
                    Case "dest_uso"
                        Riga.Cells(k).InnerHtml = "Destinazione<br/>d'uso"
                    Case "port_des"
                        Riga.Cells(k).InnerHtml = "Portinnesto"
                    Case "imp_des"
                        Riga.Cells(k).InnerHtml = "Impianto Irrigazione"
                    Case "magazzino_conf"
                        Riga.Cells(k).InnerHtml = "Magazzino<br/>Conferimento"
                    Case "Cod_Varieta_OI_NordEst".ToLower
                        Riga.Cells(k).InnerHtml = "Cod.Varieta<br/>OI Nord-Est"
                    Case "Desc_Varieta_OI_NordEst".ToLower
                        Riga.Cells(k).InnerHtml = "Desc.Varieta<br/>OI Nord-Est"
                End Select

            Next

            Me.TableExcel.Rows.Add(Riga)
            '----------------------------------
            '------- FINE INTESTAZIONE --------
            '----------------------------------

            '----------------------------------
            '------ RIEMPIMENTO TABELLA -------
            '----------------------------------

            'scorro le righe
            For i = 0 To Dt_Finale.Rows.Count - 1

                Riga = New HtmlTableRow

                'scorro le colonne
                For j = 0 To Dt_Finale.Columns.Count - 1

                    'aggiungo la cella
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(j).Style.Item("text-align") = "center"
                    Riga.Cells(j).Style.Item("vertical-align") = "middle"
                    'Riga.Cells(j).Height = "26"

                    Select Case Dt_Finale.Columns.Item(j).Caption

                        Case "PIVA", "Piva", "piva", "SA_COD", "sa_cod", "APPEZZA", "appezza", "ID_REG", "id_dest", "CUAA", "PIVA_padre", "Codice_Socio", "pro_cod_istat", "com_cod_istat", "PROV", "COM"
                            'aggiungo uno spazio davanti x salvare gli zeri...
                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), "&nbsp;" & Dt_Finale.Rows(i).Item(j), "&nbsp;")

                        Case "lav_cod_imp"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case 71

                                    Riga.Cells(j).InnerHtml = "Trapianto"

                                Case 2

                                    Riga.Cells(j).InnerHtml = "Semina"

                                Case -1

                                    Riga.Cells(j).InnerHtml = " "

                            End Select


                        Case "FOGLIO", "NUMERO", "ETTARI", "ARE", "CENTIARE", "sup_imp", "veg_cod", "campo_cod", "AREA", _
                        "sup_totale", "sup_sau", "sup_tara", "sau_convenz", "sau_convers", "sau_bio", "sup_bosco", "sup_prato", _
                        "gru_cod", "cul_cod"

                            If Dt_Finale.Rows(i).Item(j) = -1 Then
                                Riga.Cells(j).InnerHtml = " "
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                            End If

                        Case "tipo_impresa"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case -1
                                    Riga.Cells(j).InnerHtml = " "

                                Case 1
                                    Riga.Cells(j).InnerHtml = "Impresa"

                                Case 2
                                    Riga.Cells(j).InnerHtml = "Cooperativa"

                                Case 3
                                    Riga.Cells(j).InnerHtml = "Consorzio"

                                Case 4
                                    Riga.Cells(j).InnerHtml = "OP"

                            End Select

                        Case "tipo_attivita"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case "PV"
                                    Riga.Cells(j).InnerHtml = "Produzione vegetale"

                                Case "PZ"
                                    Riga.Cells(j).InnerHtml = "Produzione zootecnica"

                                Case "PVZ"
                                    Riga.Cells(j).InnerHtml = "Produzione vegetale e zootecnica"

                                Case "TPV"
                                    Riga.Cells(j).InnerHtml = "Preparazione vegetale"

                                Case "TPZ"
                                    Riga.Cells(j).InnerHtml = "Preparazione zootecnica"

                                Case "TPVZ"
                                    Riga.Cells(j).InnerHtml = "Preparazione vegetale e zootecnica"

                                Case "I"
                                    Riga.Cells(j).InnerHtml = "Importazione"

                                Case "RS"
                                    Riga.Cells(j).InnerHtml = "Raccolta spontanea"

                                Case "P/TP"
                                    Riga.Cells(j).InnerHtml = "Produzione / Preparazione"

                                Case "TP/I"
                                    Riga.Cells(j).InnerHtml = "Preparazione / Importazione"

                                Case "@"
                                    Riga.Cells(j).InnerHtml = "Altro"

                            End Select


                        Case "TitoloPossesso", "possesso_centro"

                            Select Case Dt_Finale.Rows(i).Item(j)

                                Case -1
                                    Riga.Cells(j).InnerHtml = " "

                                Case 0
                                    Riga.Cells(j).InnerHtml = "Altro"

                                Case 1
                                    Riga.Cells(j).InnerHtml = "Proprieta'"

                                Case 2
                                    Riga.Cells(j).InnerHtml = "Comodato d'uso"

                                Case 3
                                    Riga.Cells(j).InnerHtml = "Affitto con contratto"

                                Case 4
                                    Riga.Cells(j).InnerHtml = "Affitto senza contratto"

                                Case 5
                                    Riga.Cells(j).InnerHtml = "In conto terzi"

                            End Select

                        Case "pro_legale"

                            If (Dt_Finale.Rows(i).Item(j) = "0") Then

                                Riga.Cells(j).InnerHtml = " "

                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)

                            End If

                        Case "organismi"

                            Riga.Cells(j).Width = "480"
                            Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)

                            'Case "veg_des"
                            '    If Dt_Finale.Rows(i).Item("cul_des") = " " Then
                            '        'se c'è la superficie, ma non la varietà, significa che è terreno nudo
                            '        If CStr(Dt_Finale.Rows(i).Item("sup_imp")) <> " " Then
                            '            Riga.Cells(j).InnerHtml = "Terreno Nudo"
                            '        End If
                            '    Else
                            '        Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                            '    End If

                            'Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "Terreno Nudo")
                            'Riga.Cells(j).Align = "center"

                        Case "inizio_distinta", "inizio_impianto", "inizio_centro", "inizio_impresa", "inizio_appezza", "data_semina"
                            If Dt_Finale.Rows(i).Item(j) = "01/01/1900" Then
                                Riga.Cells(j).InnerHtml = "..."
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                            End If

                        Case "fine_distinta", "fine_impianto", "fine_centro", "fine_impresa", "fine_appezza"
                            If Dt_Finale.Rows(i).Item(j) = "31/12/2100" Then
                                Riga.Cells(j).InnerHtml = "..."
                            Else
                                Riga.Cells(j).InnerHtml = Dt_Finale.Rows(i).Item(j)
                                'Dt_Finale.Rows(i).Item(j) = Format(Dt_Finale.Rows(i).Item(j), "dd/MM/yyyy")
                            End If

                        Case Else

                            Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt_Finale.Rows(i).Item(j)), Dt_Finale.Rows(i).Item(j), "&nbsp;")

                            '    Case 3
                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                            '        Riga.Cells(j).Align = "center"                        

                            '    Case 6

                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "Non Definita")


                            '    Case 9
                            '        'stampo la data nel formato short
                            '        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                            '        If Riga.Cells(j).InnerHtml <> "&nbsp;" Then
                            '            Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                            '        End If                    

                    End Select

                Next

                'Aggiungo la Riga alla Tabella 
                Me.TableExcel.Rows.Add(Riga)

            Next
            '-------------------------------
            '--------- FINE TABELLA --------
            '-------------------------------

        End If


    End Sub

End Class