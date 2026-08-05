Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class PannelloDiControllo_scriptService_NC_OPTA1
    Inherits System.Web.Services.WebService

    Dim CHIAVE_ARRAY_MACCHINE As String() = {"Audit_Cod", "Audit_SuperUser", "Audit_Tipo", "regolamento_cod", "Mac_Cod", "ID_Dettaglio", "ID_Dettaglio2"}
    Dim CHIAVE_ARRAY_FORNI As String() = {"Audit_Cod", "Audit_SuperUser", "Audit_Tipo", "regolamento_cod", "Piva", "Sa_Cod", "Fabbricato_Cod", "ID_Dettaglio", "ID_Dettaglio2"}
    Dim CHIAVE_ARRAY_ZONE As String() = {"Audit_Cod", "Audit_SuperUser", "Audit_Tipo", "regolamento_cod", "Elemento_Cod", "ID_Dettaglio", "ID_Dettaglio2"}
    Const TipoAudit As TipiEnumerativi.enum_AuditPuaTipo = TipiEnumerativi.enum_AuditPuaTipo.Audit_SchedaControlliTTI
    Const Regolamento_Cod As Integer = 1

    <WebMethod(EnableSession:=True)> _
    Public Function LeggiConformitaOpta() As RispostaStandard

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        'leggo tutte le non conformita OPTA
        Dim elemR As New AgronicaCorePannelloDiControlloDAL.NonConformita_R()
        Dim dt As DataTable = elemR.Leggi_NonConformita("", "", objParametri_Server)

        Dim stb As New StringBuilder()

        stb.Append("SELECT ID_NC, c.Area, c.Tipologia, i.rag_soc as 'Azienda', ud.Nome + ' ' + ud.Cognome as 'Utente', Data, s.Nome as 'Stato', pnl.Descrizione, pnl.Note " & vbCrLf)
        stb.Append(" FROM PnlCtrl_NonConformita pnl " & vbCrLf)
        stb.Append(" INNER JOIN " & vbCrLf)
        stb.Append("      ( SELECT ID_NC AS 'ID_NC2', MAX(data) AS 'MaxData' " & vbCrLf)
        stb.Append("        FROM PnlCtrl_NonConformita " & vbCrLf)
        stb.Append("        GROUP BY ID_NC " & vbCrLf)
        stb.Append("      ) pnlMax " & vbCrLf)
        stb.Append(" ON pnl.ID_NC = pnlMax.ID_NC2 AND pnl.data=pnlMax.MaxData " & vbCrLf)
        stb.Append(" LEFT JOIN PnlCtrl_Categorie c " & vbCrLf)
        stb.Append(" ON pnl.ID_Categoria = c.ID " & vbCrLf)
        stb.Append(" INNER JOIN imprese i " & vbCrLf)
        stb.Append(" ON pnl.piva=i.PIVA " & vbCrLf)
        stb.Append(" INNER JOIN opta_utenti.dbo.utenti_dettagli ud " & vbCrLf)
        stb.Append(" ON ud.CodFisc=pnl.Utente " & vbCrLf)
        stb.Append(" INNER JOIN PnlCtrl_Stati s " & vbCrLf)
        stb.Append(" ON s.ID=pnl.ID_Stato " & vbCrLf)

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        dt = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c As New ColonneNome("ID_NC", "Azioni", "string")

        Dim listaBtn = New List(Of btnAzioni)
        listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", ""))
        listaBtn.Add(New btnAzioni("fa-trash-o del_elem", ""))

        Dim tool As New ToolStandard(listaBtn)
        'tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        'c._ColonnaDiSelezione = True
        l.Add(c)

        'aggiungo le colonne della tabella che mi interessano
        For Each dc As DataColumn In dt.Columns
            Dim cn As New ColonneNome(dc, dc.ColumnName.Replace("_", " "))
            Select Case dc.ColumnName
                Case "ID_NC"
                    cn._ColonnaDiSelezione = True
            End Select

            l.Add(cn)
        Next

        'Aggiungo la tabella in sessione
        Dim nomeVarDtInSession As String = "WAExport_NC_AuditTTI"
        HttpContext.Current.Session.Add(nomeVarDtInSession, dt)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        r.RispostaStringa = js.JSON_DataTable(dt, l)
        r.opzioniWatable.nomeVarDtInSession = nomeVarDtInSession
        r.opzioniWatable.PrefissoNomeFileExport = "PnlCtrl_Scad"
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function AggiornaListaNC_OPTA()
        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim NC_W As New AgronicaCorePannelloDiControlloBIZ.NonConformita_W()
        Dim NC_Det_W As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_W()
        Dim log As New DataProvider()
        Dim seq As New Agro_Sequenze()
        Dim esito As Boolean

        Dim Id_NC, ID_NC_Det, categoria As Integer
        Dim tabDettaglio_Nome, tabDettaglio_Chiave, descrizione, note As String
        Dim tabDettaglio_ChiaveArray As String() = {}

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'creo in core query che mi da tutte le nc NUOVE del tabacco
            Dim dtMacchine As DataTable = LEGGI_NCTabacco_Macchine_Nuove()
            Dim dtForni As DataTable = LEGGI_NCTabacco_Forni_Nuove()
            Dim dtZone As DataTable = LEGGI_NCTabacco_Zone_Nuove()

            Dim cat_R As New AgronicaCorePannelloDiControlloBIZ.Categorie_R
            Dim listaCat As List(Of AgronicaCorePannelloDiControlloBIZ.PnlCtrl_Categorie) = _
                cat_R.leggi_PnlCtrl_Categorie(objParametri_Server, Nothing, Nothing, Nothing, Nothing)

            'SALVATAGGIO DEI DATI
            For Each dtGenerico As DataTable In {dtMacchine, dtForni, dtZone}

                Dim listaCat_Filtrata As New List(Of AgronicaCorePannelloDiControlloBIZ.PnlCtrl_Categorie)

                'in base alla tabella estraggo il nome della tabella ed il nome delle chiavi
                tabDettaglio_Nome = dtGenerico.TableName
                Select Case dtGenerico.TableName
                    Case dtMacchine.TableName
                        tabDettaglio_ChiaveArray = CHIAVE_ARRAY_MACCHINE
                        listaCat_Filtrata = listaCat.Where(Function(x) x.Area = "Macchine").ToList()
                    Case dtForni.TableName
                        tabDettaglio_ChiaveArray = CHIAVE_ARRAY_FORNI
                        listaCat_Filtrata = listaCat.Where(Function(x) x.Area = "Forni").ToList()
                    Case dtZone.TableName
                        tabDettaglio_ChiaveArray = CHIAVE_ARRAY_ZONE
                        listaCat_Filtrata = listaCat.Where(Function(x) x.Area = "Zone").ToList()
                End Select

                ' per ogni riga del datatable in oggetto
                For Each dr As DataRow In dtGenerico.Rows
                    ' creo i nuovi ID
                    Id_NC = seq.NuovoId_Tabella("PnlCtrl_ID_NC", 1, 2000000000, objParametri_Server)
                    ID_NC_Det = seq.NuovoId_Tabella("PnlCtrl_ID_NC_Dettaglio", 1, 2000000000, objParametri_Server)
                    descrizione = UtilityProvider.DBNullToNothing(dr("descrizione"))
                    note = UtilityProvider.DBNullToNothing(dr("note"))
                    If IsDBNull(dr("Area")) Then
                        categoria = Nothing
                    Else
                        categoria = (From x In listaCat_Filtrata
                                Where x.Tipologia = dr("Area")
                                Select x.ID).First()
                    End If


                    'segno il la chiave ed il relativo valore
                    tabDettaglio_Chiave = ""
                    For Each x As String In tabDettaglio_ChiaveArray
                        tabDettaglio_Chiave &= (x & "=" & dr(x) & "~")
                    Next
                    tabDettaglio_Chiave = tabDettaglio_Chiave.TrimEnd("~")

                    'scrivo il Dettaglio di Apertura della NC
                    esito = NC_Det_W.aggiungi(objParametri_Server, _
                        ID_NC_Det, Id_NC, dr("Username_Creazione"), dr("Data_Creazione"), _
                        enum_PnlCtrl_Stati.Aperto, Nothing, descrizione, note)

                    If Not esito Then
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                        Dim msg As String = "Errore durante la scrittura del dettaglio, nessun salvataggio effettuato"
                        'Messaggi.AgroMsgBox(msg, Me.Page)
                        'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
                        Exit Function
                    End If

                    'scrivo la NC
                    esito = NC_W.aggiungi(objParametri_Server, Id_NC, categoria, dr("piva"), _
                                           Nothing, tabDettaglio_Nome, tabDettaglio_Chiave)

                    If Not esito Then
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                        Dim msg As String = "Errore durante la scrittura del dettaglio, nessun salvataggio effettuato"
                        'Messaggi.AgroMsgBox(msg, Me.Page)
                        'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
                        Exit Function
                    End If
                Next
            Next

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            'Messaggi.AgroMsgBuonFine("Dati salvati correttamente", Me.Page)
            'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "transazione terminata correttamente: Creato la nuova NC")
        Catch e As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            Dim msg As String = "Errore durante la scrittura, nessun salvataggio effettuato"
            'Messaggi.AgroMsgBox(msg, Me.Page)
            'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
        End Try

        'faccio quindi una ricerca???

    End Function

    Public Function LEGGI_NCTabacco_Macchine_Nuove() As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "LEGGI_NCTabacco_Macchine_Nuove"

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim Stb As New StringBuilder
        Dim DT As New DataTable()

        Stb.Append("   SELECT a.Audit_Cod,a.Audit_SuperUser,a.Audit_Tipo,a.regolamento_cod, " & vbCrLf)
        Stb.Append("          a.piva,c.SA_COD,a.audit_Responsabile,a.note AS 'audit_Note',a.datalock,a.audit_stato, a.Username_Creazione, a.Data_Creazione, " & vbCrLf)
        Stb.Append("          m.Mac_Cod,m.Valore,m.Note AS 'note',d.Descrizione AS 'Area', d2.Descrizione AS 'Tipologia', " & vbCrLf)
        Stb.Append("          CASE WHEN d2.Descrizione IS NULL THEN d.Descrizione ELSE d.Descrizione + ': ' + d2.Descrizione END AS 'Descrizione', " & vbCrLf)
        Stb.Append("          d.ID_Dettaglio, d2.ID_Dettaglio2 " & vbCrLf)

        Stb.Append("   FROM audit a " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Centri c " & vbCrLf)
        Stb.Append("   ON a.Audit_SuperUser=c.Audit_SuperUser AND a.Audit_Tipo=c.Audit_Tipo AND a.Audit_Cod=c.Audit_Cod AND a.Regolamento_Cod=c.Regolamento_Cod " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Macchine m  " & vbCrLf)
        Stb.Append("   ON a.Audit_SuperUser=m.Audit_SuperUser AND a.Audit_Tipo=m.Audit_Tipo AND a.Audit_Cod=m.Audit_Cod AND a.Regolamento_Cod=m.Regolamento_Cod " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_MacchineXDettagli mxd " & vbCrLf)
        Stb.Append("   ON m.Audit_SuperUser=mxd.Audit_SuperUser AND m.Audit_Tipo=mxd.Audit_Tipo AND m.Audit_Cod=mxd.Audit_Cod AND m.Regolamento_Cod=mxd.Regolamento_Cod AND m.Mac_Cod=mxd.mac_Cod " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Dettagli d " & vbCrLf)
        Stb.Append("   ON mxd.ID_Dettaglio = d.ID_Dettaglio " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_MacchineXDettagli2 mxd2 " & vbCrLf)
        Stb.Append("   ON m.Audit_SuperUser=mxd2.Audit_SuperUser AND m.Audit_Tipo=mxd2.Audit_Tipo AND m.Audit_Cod=mxd2.Audit_Cod AND m.Regolamento_Cod=mxd2.Regolamento_Cod AND m.Mac_Cod=mxd2.mac_Cod " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Dettagli2 d2 " & vbCrLf)
        Stb.Append("   ON mxd2.ID_Dettaglio = d2.ID_Dettaglio AND mxd2.ID_Dettaglio2 = d2.ID_Dettaglio2 " & vbCrLf)
        Stb.Append("   LEFT JOIN ( " & vbCrLf)
        Stb.Append("        SELECT * FROM PnlCtrl_Elementi " & vbCrLf)
        Stb.Append("        WHERE PnlCtrl_Elementi.ID_Categoria = -1 " & vbCrLf)
        Stb.Append("        AND PnlCtrl_Elementi.TabDettaglio_Nome = 'AUDIT_Macchine' " & vbCrLf)
        Stb.Append("   ) pnl " & vbCrLf)
        Stb.Append("   ON pnl.PivaSuperUser=a.Audit_SuperUser " & vbCrLf)
        Stb.Append("   AND pnl.TabDettaglio_Chiave = CONCAT('Audit_Cod=' , m.Audit_Cod , '~Audit_SuperUser=' , m.Audit_SuperUser , '~Audit_Tipo=', m.Audit_Tipo , '~regolamento_cod=' , m.regolamento_cod , '~Mac_Cod=' , m.Mac_Cod,'~ID_Dettaglio=' , d.ID_Dettaglio,'~ID_Dettaglio2=' , d2.ID_Dettaglio2) " & vbCrLf)

        Stb.Append("   WHERE NonConformita = 1 " & vbCrLf)
        Stb.Append("   AND pnl.id IS NULL " & vbCrLf)

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        DT = dp.EseguiQuery_Lettura(objParametri_Server, Stb.ToString(), DescrizioneFunzione)
        DT.TableName = "AUDIT_Macchine"

        If False Then
            Throw New Exception("Modulo Condizionalita : " & DescrizioneFunzione & " : " & DescrizioneFunzione)
            Return Nothing
        Else
            Return DT
        End If

    End Function

    Public Function LEGGI_NCTabacco_Zone_Nuove() As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "LEGGI_NCTabacco_Zone_Nuove"

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim Stb As New StringBuilder
        Dim DT As New DataTable()

        Stb.Append("  SELECT a.Audit_Cod,a.Audit_SuperUser,a.Audit_Tipo,a.regolamento_cod, " & vbCrLf)
        Stb.Append("         a.piva,c.SA_COD,a.audit_Responsabile,a.note AS 'audit_Note',a.datalock,a.audit_stato, a.Username_Creazione, a.Data_Creazione,  " & vbCrLf)
        Stb.Append("         e.Elemento_Cod,e.Valore,e.Note AS 'note',  " & vbCrLf)
        Stb.Append("         d.Descrizione AS 'Area', d2.Descrizione AS 'Tipologia', " & vbCrLf)
        Stb.Append("         CASE WHEN d2.Descrizione IS NULL THEN d.Descrizione ELSE d.Descrizione + ': ' + d2.Descrizione END AS 'Descrizione', " & vbCrLf)
        Stb.Append("         d.ID_Dettaglio, d2.ID_Dettaglio2 " & vbCrLf)

        Stb.Append("   FROM audit a " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Centri c " & vbCrLf)
        Stb.Append("   ON a.Audit_SuperUser=c.Audit_SuperUser AND a.Audit_Tipo=c.Audit_Tipo AND a.Audit_Cod=c.Audit_Cod AND a.Regolamento_Cod=c.Regolamento_Cod " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Elementi e  " & vbCrLf)
        Stb.Append("   ON a.Audit_SuperUser=e.Audit_SuperUser AND a.Audit_Tipo=e.Audit_Tipo AND a.Audit_Cod=e.Audit_Cod AND a.Regolamento_Cod=e.Regolamento_Cod " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_ElementiXDettagli exd " & vbCrLf)
        Stb.Append("   ON e.Audit_SuperUser=exd.Audit_SuperUser AND e.Audit_Tipo=exd.Audit_Tipo AND e.Audit_Cod=exd.Audit_Cod AND e.Regolamento_Cod=exd.Regolamento_Cod AND e.Elemento_Cod=exd.Elemento_Cod " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_Dettagli d " & vbCrLf)
        Stb.Append("   ON exd.ID_Dettaglio = d.ID_Dettaglio " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_ElementiXDettagli2 exd2 " & vbCrLf)
        Stb.Append("   ON e.Audit_SuperUser=exd2.Audit_SuperUser AND e.Audit_Tipo=exd2.Audit_Tipo AND e.Audit_Cod=exd2.Audit_Cod AND e.Regolamento_Cod=exd2.Regolamento_Cod AND e.Elemento_Cod=exd2.Elemento_Cod " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_Dettagli2 d2 " & vbCrLf)
        Stb.Append("   ON exd2.ID_Dettaglio = d2.ID_Dettaglio AND exd2.ID_Dettaglio2 = d2.ID_Dettaglio2" & vbCrLf)
        Stb.Append("   LEFT JOIN ( " & vbCrLf)
        Stb.Append("        SELECT * FROM PnlCtrl_Elementi " & vbCrLf)
        Stb.Append("        WHERE PnlCtrl_Elementi.ID_Categoria = -1 " & vbCrLf)
        Stb.Append("        AND PnlCtrl_Elementi.TabDettaglio_Nome = 'AUDIT_Elementi' " & vbCrLf)
        Stb.Append("   ) pnl " & vbCrLf)
        Stb.Append("   ON pnl.PivaSuperUser=a.Audit_SuperUser " & vbCrLf)
        Stb.Append("   AND pnl.TabDettaglio_Chiave = CONCAT('Audit_Cod=' , e.Audit_Cod , '~Audit_SuperUser=' , e.Audit_SuperUser , '~Audit_Tipo=', e.Audit_Tipo , '~regolamento_cod=' , e.regolamento_cod , '~Elemento_Cod=' , e.Elemento_Cod, '~ID_Dettaglio=' , d.ID_Dettaglio,'~ID_Dettaglio2=' , d2.ID_Dettaglio2) " & vbCrLf)

        Stb.Append("   WHERE NonConformita = 1 " & vbCrLf)
        Stb.Append("   AND pnl.id IS NULL " & vbCrLf)

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        DT = dp.EseguiQuery_Lettura(objParametri_Server, Stb.ToString(), DescrizioneFunzione)
        DT.TableName = "AUDIT_Elementi"

        If False Then
            Throw New Exception("Modulo Condizionalita : " & DescrizioneFunzione & " : " & DescrizioneFunzione)
            Return Nothing
        Else
            Return DT
        End If
    End Function

    Public Function LEGGI_NCTabacco_Forni_Nuove() As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "LEGGI_NCTabacco_Forni_Nuove"

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim Stb As New StringBuilder
        Dim DT As New DataTable()

        Stb.Append("  SELECT a.Audit_Cod,a.Audit_SuperUser,a.Audit_Tipo,a.regolamento_cod, " & vbCrLf)
        Stb.Append("         a.piva,a.audit_Responsabile,a.note AS 'audit_Note',a.datalock,a.audit_stato, a.Username_Creazione, a.Data_Creazione,  " & vbCrLf)
        Stb.Append("         f.sa_cod,f.Fabbricato_Cod,f.Valore,f.Note AS 'note',  " & vbCrLf)
        Stb.Append("         d.Descrizione AS 'Area', d2.Descrizione AS 'Tipologia', " & vbCrLf)
        Stb.Append("         CASE WHEN d2.Descrizione IS NULL THEN d.Descrizione ELSE d.Descrizione + ': ' + d2.Descrizione END AS 'Descrizione', " & vbCrLf)
        Stb.Append("         d.ID_Dettaglio, d2.ID_Dettaglio2 " & vbCrLf)

        Stb.Append("   FROM audit a " & vbCrLf)
        Stb.Append("   INNER JOIN AUDIT_Fabbricati f  " & vbCrLf)
        Stb.Append("   ON a.Audit_SuperUser=f.Audit_SuperUser AND a.Audit_Tipo=f.Audit_Tipo AND a.Audit_Cod=f.Audit_Cod AND a.Regolamento_Cod=f.Regolamento_Cod " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_FabbricatiXDettagli fxd " & vbCrLf)
        Stb.Append("   ON f.Audit_SuperUser=fxd.Audit_SuperUser AND f.Audit_Tipo=fxd.Audit_Tipo AND f.Audit_Cod=fxd.Audit_Cod AND f.Regolamento_Cod=fxd.Regolamento_Cod AND f.SA_COD=fxd.SA_COD AND f.Fabbricato_Cod=fxd.Fabbricato_Cod " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_Dettagli d " & vbCrLf)
        Stb.Append("   ON fxd.ID_Dettaglio = d.ID_Dettaglio " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_FabbricatiXDettagli2 fxd2 " & vbCrLf)
        Stb.Append("   ON f.Audit_SuperUser=fxd2.Audit_SuperUser AND f.Audit_Tipo=fxd2.Audit_Tipo AND f.Audit_Cod=fxd2.Audit_Cod AND f.Regolamento_Cod=fxd2.Regolamento_Cod AND f.SA_COD=fxd2.SA_COD AND f.Fabbricato_Cod=fxd2.Fabbricato_Cod  " & vbCrLf)
        Stb.Append("   LEFT JOIN AUDIT_Dettagli2 d2 " & vbCrLf)
        Stb.Append("   ON fxd2.ID_Dettaglio = d2.ID_Dettaglio AND fxd2.ID_Dettaglio2 = d2.ID_Dettaglio2" & vbCrLf)
        Stb.Append("   LEFT JOIN ( " & vbCrLf)
        Stb.Append("        SELECT * FROM PnlCtrl_Elementi " & vbCrLf)
        Stb.Append("        WHERE PnlCtrl_Elementi.ID_Categoria = -1 " & vbCrLf)
        Stb.Append("        AND PnlCtrl_Elementi.TabDettaglio_Nome = 'AUDIT_Fabbricati' " & vbCrLf)
        Stb.Append("   ) pnl " & vbCrLf)
        Stb.Append("   ON pnl.PivaSuperUser=a.Audit_SuperUser " & vbCrLf)
        Stb.Append("   AND pnl.TabDettaglio_Chiave = CONCAT('Audit_Cod=' , f.Audit_Cod , '~Audit_SuperUser=' , f.Audit_SuperUser , '~Audit_Tipo=', f.Audit_Tipo , '~regolamento_cod=' , f.regolamento_cod , '~Piva=' , f.Piva  , '~Sa_Cod=' , f.Sa_Cod ,'~Fabbricato_Cod=' , f.Fabbricato_Cod,'~ID_Dettaglio=' , d.ID_Dettaglio,'~ID_Dettaglio2=' , d2.ID_Dettaglio2) " & vbCrLf)

        Stb.Append("   WHERE NonConformita = 1 " & vbCrLf)
        Stb.Append("   AND pnl.id IS NULL " & vbCrLf)

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        DT = dp.EseguiQuery_Lettura(objParametri_Server, Stb.ToString(), DescrizioneFunzione)
        DT.TableName = "AUDIT_Fabbricati"

        If False Then
            Throw New Exception("Modulo Condizionalita : " & DescrizioneFunzione & " : " & DescrizioneFunzione)
            Return Nothing
        Else
            Return DT
        End If

    End Function

End Class