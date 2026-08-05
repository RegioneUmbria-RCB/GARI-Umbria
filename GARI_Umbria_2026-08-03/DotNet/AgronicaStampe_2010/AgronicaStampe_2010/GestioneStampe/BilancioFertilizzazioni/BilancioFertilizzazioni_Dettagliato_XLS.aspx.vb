Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class BilancioFertilizzazioni_Dettagliato_XLS
    Inherits System.Web.UI.Page

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strXmlVariabilistampe As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement

        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim piva As String
        Dim sa_cod, appezza, id_reg, id_agenda As Integer

        Dim i As Integer ' la i scorre gli impianti e le imprese
        Dim j As Integer ' la j scorre le righe del DT

        Dim num_elementi As Integer
        Dim num_righe_dt As Integer

        'Dim DT_Finale As New DataTable
        'Dim Rs As ADODB.Recordset
        Dim objSQL As New Codex_Utility.Sql
        'Dim objSQL As New Codex_Utility.Sql
        'Dim StrSQL As String

        Dim errore, msg As String
        Dim Messaggio As String
        Dim AgroMsg As String = ""
        Dim flag_campo As Boolean

        Dim stbQ As New System.Text.StringBuilder
        Dim Query1_TempTableCreazione As String = ""
        Dim Query2_TempTableIndice As String = ""
        Dim Query3_TempTableFill As String = ""
        Dim Query4_TempTableJoin As String = ""

        Dim Flag_FiltroDistinta As Boolean

        '##############################################################
        'La Pagina deve essere visualizzata come un foglio Excel
        '##############################################################

        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = BilancioFertilizzazioni.xls")

        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then
            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")
        End If

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '------------------------------------------------
        '------------ INIZIO CICLO IMPIANTI -------------
        '------------------------------------------------       

        Dim ElencoChiaviImpianto, ChiaveImpianto As String
        Dim ElencoChiaviAgenda, ChiaveAgenda As String

        flag_campo = False
        ElencoChiaviImpianto = ""

        Dim FiltroImpianti As Boolean = False
        Dim FiltroMovimenti As Boolean = False

        If XMLs_VariabiliStampe.Count > 0 Then

            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(0)

            If (IsNothing(XML_VariabiliStampe.GetAttribute("id_reg"))) Then
                FiltroImpianti = False
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_reg")) = "") Then
                FiltroImpianti = False
            Else
                FiltroImpianti = True
            End If

            If (IsNothing(XML_VariabiliStampe.GetAttribute("i"))) Then
                FiltroMovimenti = False
            ElseIf (CStr(XML_VariabiliStampe.GetAttribute("i")) = "") Then
                FiltroMovimenti = False
            Else
                FiltroMovimenti = True
            End If

        End If

        If FiltroImpianti Then
            '-----------------------------------------------
            ' la i scorre gli impianti
            '-----------------------------------------------
            For i = 0 To XMLs_VariabiliStampe.Count - 1

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
                    AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                End If

                'ricavo gli impianti
                piva = XML_VariabiliStampe.GetAttribute("piva")
                sa_cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                appezza = XML_VariabiliStampe.GetAttribute("appezza")
                id_reg = XML_VariabiliStampe.GetAttribute("id_reg")

                'Genero la chiave impianto
                ChiaveImpianto = piva & "_" & sa_cod & "_" & appezza & "_" & id_reg

                ElencoChiaviImpianto += ",'" & ChiaveImpianto & "'"

                '-------------------------------------------------

                Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                Query3_TempTableFill += " VALUES     (" + Agro_SQL_SaveText_NULL(piva) + "," + Agro_SQL_SaveNum(sa_cod) + "," + Agro_SQL_SaveNum(appezza) + "," + Agro_SQL_SaveNum(id_reg) + ")  " & vbCrLf


            Next

            'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
            ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)
        End If

        If FiltroMovimenti Then
            Dim listId_Agenda As New List(Of Integer)

            '-----------------------------------------------
            ' la i scorre i movimenti
            '-----------------------------------------------
            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                If (IsNothing(XML_VariabiliStampe.GetAttribute("p"))) Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("p")) = "") Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("s"))) Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("s")) = "") Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("i"))) Then
                    msg = "ID AGENDA E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("i")) = "") Then
                    msg = "ID AGENDA E' NULLO!!!" & vbCrLf
                End If

                If msg <> "" Then
                    AgroMsg = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                End If

                'ricavo gli impianti
                piva = XML_VariabiliStampe.GetAttribute("p")
                sa_cod = XML_VariabiliStampe.GetAttribute("s")
                id_agenda = XML_VariabiliStampe.GetAttribute("i")

                'Genero la chiave impianto
                ChiaveAgenda = piva & "_" & sa_cod & "_" & id_agenda

                ElencoChiaviAgenda += ",'" & ChiaveAgenda & "'"


                If Not listId_Agenda.Contains(CInt(id_agenda)) Then
                    listId_Agenda.Add(CInt(id_agenda))
                End If

            Next

            For Each id_agendaI In listId_Agenda
                Query3_TempTableFill += " INSERT INTO #tempAgenda (ID_Agenda)  " & vbCrLf
                Query3_TempTableFill += " VALUES     (" + Agro_SQL_SaveNum(id_agendaI) + ")  " & vbCrLf
            Next

            'Formatto correttamente l'elenco (tolgo la virgola iniziale ...)
            ElencoChiaviImpianto = Mid(ElencoChiaviImpianto, 2)
        End If

        Dim TabelleTemp_Mode As Integer
        Dim Str_TabelleTemp_RegolaConfronto As String

        TabelleTemp_Mode = Recupera_TabelleTemp_Mode()

        '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO 
        '2: TRAMITE CREATE TABLE
        Select Case TabelleTemp_Mode

            Case 1
                If FiltroImpianti Then
                    Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                    Query1_TempTableCreazione += "   INTO #tempimpianti   "
                    Query1_TempTableCreazione += "       FROM Reg_Impianti "
                    Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Query1_TempTableCreazione += vbCrLf
                End If
                If FiltroMovimenti Then
                    Query1_TempTableCreazione += " SELECT ID_Agenda "
                    Query1_TempTableCreazione += "   INTO #tempAgenda   "
                    Query1_TempTableCreazione += "       FROM Agenda "
                    Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Query1_TempTableCreazione += vbCrLf
                End If

            Case 2

                'Recupera regola di Confronto: collate sql_LATIN1_GENERAL_cp850_ci_as NOT NULL
                If FiltroImpianti Then
                    Str_TabelleTemp_RegolaConfronto = Recupera_Str_TabelleTemp_RegolaConfronto()

                    Query1_TempTableCreazione += "   CREATE TABLE #tempimpianti (	"
                    Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                    Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL ,"
                    Query1_TempTableCreazione += " [Appezza]    [int] 	        NOT NULL ,"
                    Query1_TempTableCreazione += " [Id_Reg]     [int] 		    NOT NULL ,"
                    Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Query1_TempTableCreazione += vbCrLf
                End If

                If FiltroMovimenti Then
                    Query1_TempTableCreazione += "   CREATE TABLE #tempAgenda (	"
                    Query1_TempTableCreazione += " [Id_Agenda]     [int] 		    NOT NULL ,"
                    Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Query1_TempTableCreazione += vbCrLf
                End If

        End Select

        If FiltroImpianti Then
            Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "
        End If

        If FiltroMovimenti Then
            Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempAgenda] ON [dbo].[#tempAgenda]([Id_Agenda]) "
        End If


        '-----------------------------------------------------------------------
        ' VAI CON LA POTENZA DELLA TERA-QUERY IMPIANTI !!!!!!!!
        '-----------------------------------------------------------------------
        ' NOTA: SE AL CENTRO AZIENDALE NON E' IMPOSTATO L'INDIRIZZO DI TIPO 1
        ' I RELATIVI IMPIANTI NON VENGONO TIRATI SU DALLA QUERY!
        '-----------------------------------------------------------------------

        stbQ.Length = 0
        stbQ.AppendLine("SELECT DISTINCT Agenda.PIVA, ")
        stbQ.AppendLine("	CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Agenda.PIVA ELSE Imprese.partitaIvaReale END AS PivaReale,  ")
        stbQ.AppendLine("	Imprese_Codici.val_Cod as CUAA,  ")
        stbQ.AppendLine("	ISNULL(Imprese.rag_soc, ' ') AS rag_soc,   ")
        stbQ.AppendLine("	Agenda.sa_cod,   ")
        stbQ.AppendLine("	ISNULL(Centri_Aziendali.sa_nome, ' ') AS sa_nome,   ")
        stbQ.AppendLine("	ISNULL(Campi.Campo_Des, ' ') AS campo_des,   ")
        stbQ.AppendLine("	ISNULL(Appezzamento.APP_NOME, ' ') AS app_nome, ")
        stbQ.AppendLine("	ISNULL(Reg_Impianti.Sup_Imp, -1.0000) AS sup_imp,  ")
        stbQ.AppendLine("	ISNULL(SpecieVegetali.Veg_Des, ' ') AS veg_des,   ")
        stbQ.AppendLine("	ISNULL(Cultivar.Cul_Des, ' ') AS cul_des,   ")
        stbQ.AppendLine("	ISNULL(GruppoFinalita.Grfi_Des, ' ') AS grfi_des,   ")
        stbQ.AppendLine("	ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Inizio, 103), ' ') AS inizio_impianto,  ")
        stbQ.AppendLine("	ISNULL(CONVERT(varchar(10), Reg_Impianti.Validita_Fine, 103), ' ') AS fine_impianto,   ")
        stbQ.AppendLine("	ISNULL(( SELECT    TOP 1 Regolamenti.Reg_Des               ")
        stbQ.AppendLine("			FROM    Imprese_Progetti               ")
        stbQ.AppendLine("				INNER JOIN  Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod               ")
        stbQ.AppendLine("			WHERE  (Imprese_Progetti.Piva = Reg_Impianti.Piva)               ")
        stbQ.AppendLine("			AND     (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod)             ")
        stbQ.AppendLine("			AND     (Imprese_Progetti.Appezza  = Reg_Impianti.Appezza)             ")
        stbQ.AppendLine("			AND     (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)               ")
        stbQ.AppendLine("			AND     (Imprese_Progetti.Validita_Inizio <= Movimenti.data_movimento )               ")
        stbQ.AppendLine("			AND     (Imprese_Progetti.Validita_Fine >= Movimenti.data_movimento )  ) , ' ') AS Reg_Des,   ")
        stbQ.AppendLine("	Agenda.id_agenda,  ")
        stbQ.AppendLine("	Agenda.lav_cod,  ")
        stbQ.AppendLine("	ISNULL(Operazioni.Lav_Des, ' ') AS des_lib,  ")
        stbQ.AppendLine("	ISNULL(Blocco_Flag, 0) AS Blocco_Flag,  ")
        stbQ.AppendLine("	' ' AS Bloccato,  ")
        stbQ.AppendLine("	ISNULL(Blocco_Data, ' ') AS Blocco_Data,  ")
        stbQ.AppendLine("	' ' AS Data_Bloccato,  ")
        stbQ.AppendLine("	ISNULL(Blocco_Username, ' ') AS Blocco_Username,   ")
        stbQ.AppendLine("	ISNULL(Movimenti.Data_Movimento, '01/01/1900') AS data_operazione,   ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod,  ")
        stbQ.AppendLine("	' ' AS Categoria,  ")
        stbQ.AppendLine("	' ' AS Prodotto,  ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod,  ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod,   ")
        stbQ.AppendLine("	ISNULL(FORMULATI.Fr_Des, '') AS Fr_Des,  ")
        stbQ.AppendLine("	ISNULL(FERTILIZZANTI.Fer_Des, '') AS Fer_Des,  ")
        stbQ.AppendLine("	ISNULL(TRAPPOLE.TRAP_DES, '') AS Trap_Des,   ")
        stbQ.AppendLine("	ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des,  ")
        stbQ.AppendLine("	ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo,   ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Udm_Cod, - 1) AS udm_cod,  ")
        stbQ.AppendLine("	ISNULL(UnitaMisura1.UDM_SIM, ' ') AS udm_sim_1,  ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Extra_Int, - 1 ) AS extra_int,  ")
        stbQ.AppendLine("	ISNULL(UnitaMisura2.UDM_SIM, ' ') AS udm_sim_2,  ")
        stbQ.AppendLine("	' ' AS Udm_Des,  ")
        'stbQ.AppendLine("	ISNULL(Movimenti_dettagli.Qta, 0) AS Qta,  ")
        stbQ.AppendLine("  CASE  " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 2 Then Movimenti_dettagli.qta " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 29 Then Movimenti_dettagli.qta " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 19 Then Movimenti_dettagli.qta*1000 " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 304 Then Movimenti_dettagli.qta*1000 " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 4 Then Movimenti_dettagli.qta*100 " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 104 Then Movimenti_dettagli.qta/1000 " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 101 Then Movimenti_dettagli.qta/1000 " & vbCrLf)
        stbQ.AppendLine("    WHEN Movimenti_dettagli.extra_int = 3 Then Movimenti_dettagli.qta/1000 " & vbCrLf)
        stbQ.AppendLine("    ELSE  0  " & vbCrLf)
        stbQ.AppendLine("  END  as Qta, " & vbCrLf)

        stbQ.AppendLine("	ISNULL(Movimenti.Mezzo, -1) AS mezzo  ,  ")
        stbQ.AppendLine("	ISNULL(Movimenti_dettagli.PrincipiAttivi, '' ) AS PrincipiAttivi  ,   ")
        stbQ.AppendLine("	ISNULL(Mov_Destinazioni.qta2, 0) AS sup_trattata ,   ")
        stbQ.AppendLine("	ISNULL(Mov_Destinazioni.qta, 0) AS qta_totale ,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.PROV, ' ') AS PROV,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.COM, ' ')AS COM,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.SEZIONE, ' ') AS SEZIONE,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.FOGLIO, -1) AS FOGLIO,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.NUMERO, -1) AS NUMERO,   ")
        stbQ.AppendLine("	ISNULL(AppezzamentiXParticelle.SUBALTERNO, ' ') AS SUBALTERNO,   ")
        stbQ.AppendLine("	CASE WHEN ParticelleCatastali.ETTARI is null ")
        stbQ.AppendLine("	 THEN -1 ")
        stbQ.AppendLine("	 ELSE CAST(ParticelleCatastali.ETTARI as decimal) + CAST(ParticelleCatastali.[ARE] as decimal)/100 + CAST(ParticelleCatastali.CENTIARE as decimal)/10000 ")
        stbQ.AppendLine("	 END as Sup_Particella, ")
        stbQ.AppendLine("	CASE WHEN ZonexParticelle.Zona_Cod is null ")
        stbQ.AppendLine("		THEN '' ")
        stbQ.AppendLine("		ELSE 'X' ")
        stbQ.AppendLine("	END as ZVN, ")
        stbQ.AppendLine("	ISNULL(ImpreseXParticelle.TitoloPossesso, -1) AS TitoloPossesso,   ")
        stbQ.AppendLine("	ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Inizio, 103), ' ') AS dal,  ")
        stbQ.AppendLine("	ISNULL(CONVERT(varchar(10), ImpreseXParticelle.Validita_Fine, 103), ' ') AS al,   ")
        stbQ.AppendLine("	ISNULL(dbo.AppezzamentiXParticelle.AREA, - 1) AS AREA   ")
        stbQ.AppendLine("	FROM Agenda   ")
        stbQ.AppendLine("	INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        stbQ.AppendLine("	INNER JOIN Movimenti  ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda   ")
        stbQ.AppendLine("	INNER JOIN Movimenti_dettagli  ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov   ")
        stbQ.AppendLine("	INNER JOIN dbo.Mov_Destinazioni  ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det   ")
        stbQ.AppendLine("	LEFT OUTER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod  ")
        stbQ.AppendLine("	LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD   ")
        stbQ.AppendLine("	LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN UnitaMisura UnitaMisura1 ON Movimenti_dettagli.Udm_Cod  = UnitaMisura1.UDM_COD   ")
        stbQ.AppendLine("	LEFT OUTER JOIN UnitaMisura UnitaMisura2 ON Movimenti_dettagli.Extra_Int = UnitaMisura2.UDM_COD   ")
        stbQ.AppendLine("	INNER JOIN Reg_Impianti  ON Reg_Impianti.PIVA = Agenda.PIVA AND Reg_Impianti.SA_COD = Agenda.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza   ")
        stbQ.AppendLine("	INNER JOIN Imprese ON Imprese.PIVA = Agenda.PIVA   ")
        stbQ.AppendLine("	LEFT JOIN Imprese_Codici ON Imprese.Piva = Imprese_Codici.Piva AND Imprese_Codici.ID_Cod = 1010 ")
        stbQ.AppendLine("	INNER JOIN Centri_Aziendali  ON Agenda.Sa_Cod = Centri_Aziendali.sa_cod AND Agenda.PIVA = Centri_Aziendali.PIVA   ")
        stbQ.AppendLine("	INNER JOIN Appezzamento  ON Mov_Destinazioni.Appezza = Appezzamento.Appezza AND Appezzamento.PIVA = Agenda.PIVA AND Appezzamento.SA_COD = Agenda.Sa_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN Campi ON Appezzamento.Campo_Cod = Campi.Campo_Cod  AND Appezzamento.piva = Campi.piva  AND Appezzamento.sa_cod = Campi.sa_cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod   ")
        stbQ.AppendLine("	LEFT OUTER JOIN AppezzamentiXParticelle  ON Reg_Impianti.PIVA = AppezzamentiXParticelle.PIVA  ")
        stbQ.AppendLine("											AND Reg_Impianti.SA_COD = AppezzamentiXParticelle.SA_COD  ")
        stbQ.AppendLine("											AND Reg_Impianti.Appezza = AppezzamentiXParticelle.Appezza   ")
        stbQ.AppendLine("	LEFT OUTER JOIN ImpreseXParticelle  ON ImpreseXParticelle.PIVA = AppezzamentiXParticelle.PIVA   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.sa_cod = AppezzamentiXParticelle.SA_COD   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.PROV = AppezzamentiXParticelle.PROV  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.COM = AppezzamentiXParticelle.COM   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.SEZIONE = AppezzamentiXParticelle.SEZIONE  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.FOGLIO = AppezzamentiXParticelle.FOGLIO   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.NUMERO = AppezzamentiXParticelle.NUMERO  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.SUBALTERNO = AppezzamentiXParticelle.SUBALTERNO   ")
        stbQ.AppendLine("	LEFT OUTER JOIN  ParticelleCatastali  ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.COM = ParticelleCatastali.COM   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO   ")
        stbQ.AppendLine("										AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO  ")
        stbQ.AppendLine("										AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO  ")
        stbQ.AppendLine("	LEFT JOIN ZonexParticelle ON ZonexParticelle.PROV = ParticelleCatastali.PROV  ")
        stbQ.AppendLine("										AND ZonexParticelle.COM = ParticelleCatastali.COM   ")
        stbQ.AppendLine("										AND ZonexParticelle.SEZIONE = ParticelleCatastali.SEZIONE  ")
        stbQ.AppendLine("										AND ZonexParticelle.FOGLIO = ParticelleCatastali.FOGLIO   ")
        stbQ.AppendLine("										AND ZonexParticelle.NUMERO = ParticelleCatastali.NUMERO  ")
        stbQ.AppendLine("										AND ZonexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO  ")
        stbQ.AppendLine("										AND ZonexParticelle.Zona_Cod = -17 ")
        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA

        '           --> AGGIUNGO QUESTA PARTE:

        If FiltroImpianti Then
            stbQ.AppendLine(" INNER JOIN  #tempimpianti " & vbCrLf)
            stbQ.AppendLine(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
        End If

        If FiltroMovimenti Then
            stbQ.AppendLine(" INNER JOIN  #tempAgenda " & vbCrLf)
            stbQ.AppendLine(" ON #tempAgenda.ID_Agenda = Agenda.ID_Agenda " & vbCrLf)
        End If

        '/*************************************************

        stbQ.AppendLine(" WHERE Agenda.Lav_Cod IN (14, 156, 124, 123, 26, 106) ")


        stbQ.AppendLine(" ORDER BY rag_soc  , data_operazione  ASC ")

        '/**************************************************
        Query4_TempTableJoin = stbQ.ToString

        stbQ = Nothing
        '/**************************************************



        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA  
        Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery
        Dim DtImpianti = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione, _
                                                                Query2_TempTableIndice, _
                                                                Query3_TempTableFill, _
                                                                Query4_TempTableJoin, _
                                                                 objParametri_Server.StringaConnessione, _
                                                                 Messaggio)
        '/**************************************************

        If IsNothing(Messaggio) Then

            '    If (Not IsNothing(Rs)) AndAlso _
            '        (Rs.State <> 0) AndAlso _
            '            (Not Rs.EOF) Then

            If Not IsNothing(DtImpianti) Then

                If DtImpianti.Rows.Count <> 0 Then
                    '##############################################################
                    '#####  Costruisco la tabella   ###############################
                    '##############################################################

                    Dim Riga As HtmlTableRow

                    Me.TableBilancio.Rows(0).Cells(0).InnerHtml = " BILANCIO FERTILIZZAZIONI "
                    Me.TableBilancio.Rows(0).Cells(0).ColSpan = 27

                    'Creo la prima riga con l'intestazione
                    Riga = New HtmlTableRow

                    For i = 0 To 29
                        Riga.Cells.Add(New HtmlTableCell)
                        AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(i), 2, "", "", "Gainsboro", "center", "top")
                    Next

                    Riga.Cells(0).InnerHtml = "Cuaa"
                    Riga.Cells(1).InnerHtml = "Ragione<br>Sociale"
                    Riga.Cells(2).InnerHtml = "Partita<br>Iva"
                    Riga.Cells(3).InnerHtml = "Centro<br>Aziendale"
                    Riga.Cells(4).InnerHtml = "Campo"
                    Riga.Cells(5).InnerHtml = "Appezzamento"
                    Riga.Cells(6).InnerHtml = "Sup.<br>[Ha]"
                    Riga.Cells(7).InnerHtml = "Specie<br>Vegetale"
                    Riga.Cells(8).InnerHtml = "Cultivar"
                    Riga.Cells(9).InnerHtml = "Finalita"
                    Riga.Cells(10).InnerHtml = "Inizio Impianto"
                    Riga.Cells(11).InnerHtml = "Fine Impianto"
                    Riga.Cells(12).InnerHtml = "Regolamento"
                    Riga.Cells(13).InnerHtml = "Data<br>Operazione"
                    Riga.Cells(14).InnerHtml = "Operazione"
                    Riga.Cells(15).InnerHtml = "Prodotto"
                    Riga.Cells(16).InnerHtml = "U.d.M."
                    Riga.Cells(17).InnerHtml = "Qta/[Ha]"
                    Riga.Cells(18).InnerHtml = "Sup.Trattata<br>[Ha]"
                    Riga.Cells(19).InnerHtml = "Qta<br>Totale"
                    Riga.Cells(20).InnerHtml = "PROV"
                    Riga.Cells(21).InnerHtml = "COM"
                    Riga.Cells(22).InnerHtml = "Sezione"
                    Riga.Cells(23).InnerHtml = "Foglio"
                    Riga.Cells(24).InnerHtml = "Numero"
                    Riga.Cells(25).InnerHtml = "Subalterno"
                    Riga.Cells(26).InnerHtml = "ZVN"
                    Riga.Cells(27).InnerHtml = "Sup.<br>Particella"
                    Riga.Cells(28).InnerHtml = "Titolo<br>Possesso"
                    Riga.Cells(29).InnerHtml = "Sup.<br>Intersezione"

                    TableBilancio.Rows.Add(Riga)

                    For i = 0 To DtImpianti.Rows.Count - 1

                        Riga = New HtmlTableRow

                        For j = 0 To 29

                            Riga.Cells.Add(New HtmlTableCell)

                            Select Case j

                                Case 0
                                    'aggiungo alla piva lo spazio x salvare gli zeri..
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("cuaa")), "&nbsp;" & DtImpianti.Rows(i).Item("cuaa"), "&nbsp;")

                                Case 1
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("rag_soc")), DtImpianti.Rows(i).Item("rag_soc"), "&nbsp;")

                                Case 2
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("PivaReale")), DtImpianti.Rows(i).Item("PivaReale"), "&nbsp;")

                                Case 3
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("sa_nome")), DtImpianti.Rows(i).Item("sa_nome"), "&nbsp;")

                                Case 4
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("campo_des")), DtImpianti.Rows(i).Item("campo_des"), "&nbsp;")

                                Case 5
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("app_nome")), DtImpianti.Rows(i).Item("app_nome"), "&nbsp;")

                                Case 6
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("sup_imp")), DtImpianti.Rows(i).Item("sup_imp"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        'Sup_Imp = CDbl(Riga.Cells(j).InnerHtml)
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.0000")
                                    Else
                                        'Sup_Imp = 0
                                    End If

                                Case 7
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("veg_des")), DtImpianti.Rows(i).Item("veg_des"), "&nbsp;")

                                Case 8
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("cul_des")), DtImpianti.Rows(i).Item("cul_des"), "&nbsp;")

                                Case 9
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("grfi_des")), DtImpianti.Rows(i).Item("grfi_des"), "&nbsp;")

                                Case 10
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("inizio_impianto")), DtImpianti.Rows(i).Item("inizio_impianto"), "&nbsp;")

                                Case 11
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("fine_impianto")), DtImpianti.Rows(i).Item("fine_impianto"), "&nbsp;")

                                Case 12
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("reg_des")), DtImpianti.Rows(i).Item("reg_des"), "&nbsp;")

                                Case 13
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("data_operazione")), DtImpianti.Rows(i).Item("data_operazione"), "&nbsp;")

                                Case 14
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("des_lib")), DtImpianti.Rows(i).Item("des_lib"), "&nbsp;")

                                Case 15
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("fer_des")), DtImpianti.Rows(i).Item("fer_des"), "&nbsp;")

                                Case 16
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("udm_sim_1")), DtImpianti.Rows(i).Item("udm_sim_1"), "&nbsp;")

                                Case 17
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Qta")), DtImpianti.Rows(i).Item("Qta"), "&nbsp;")

                                Case 18
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("sup_trattata")), DtImpianti.Rows(i).Item("sup_trattata"), "&nbsp;")

                                Case 19
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("qta_totale")), DtImpianti.Rows(i).Item("qta_totale"), "&nbsp;")

                                Case 20
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("PROV")), DtImpianti.Rows(i).Item("PROV"), "&nbsp;")

                                Case 21
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("COM")), DtImpianti.Rows(i).Item("COM"), "&nbsp;")

                                Case 22
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Sezione")), DtImpianti.Rows(i).Item("Sezione"), "&nbsp;")

                                Case 23
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Foglio")), DtImpianti.Rows(i).Item("Foglio"), "&nbsp;")

                                Case 24
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Numero")), DtImpianti.Rows(i).Item("Numero"), "&nbsp;")

                                Case 25
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Subalterno")), DtImpianti.Rows(i).Item("Subalterno"), "&nbsp;")

                                Case 26
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("ZVN")), DtImpianti.Rows(i).Item("ZVN"), "&nbsp;")

                                Case 27
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("Sup_Particella")), DtImpianti.Rows(i).Item("Sup_Particella"), "&nbsp;")

                                Case 28
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("TitoloPossesso")), DtImpianti.Rows(i).Item("TitoloPossesso"), "&nbsp;")

                                Case 29
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(DtImpianti.Rows(i).Item("AREA")), DtImpianti.Rows(i).Item("AREA"), "&nbsp;")

                                Case Else
                                    'Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dv.Item(i).Item(j)), Dv.Item(i).Item(j), "&nbsp;")

                            End Select

                        Next

                        Me.TableBilancio.Rows.Add(Riga)

                    Next
                End If

            End If

        End If


    End Sub

End Class