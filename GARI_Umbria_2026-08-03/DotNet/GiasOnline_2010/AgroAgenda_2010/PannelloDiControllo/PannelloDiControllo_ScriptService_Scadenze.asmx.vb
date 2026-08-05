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
Public Class PannelloDiControllo_ScriptService_Scadenze
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function LeggiScadenza(ID_Elem As Integer) As rispostaStandard(Of PnlCtrl_Scadenze)
        Dim r As New rispostaStandard(Of PnlCtrl_Scadenze)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim elemR As New AgronicaCorePannelloDiControlloBIZ.Scadenze_R()

        Try
            r.RispostaStringa = elemR.leggi_PnlCtrl_Elementi(objParametri_Server, ID_Elem).First()
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Function LeggiScadenze() As RispostaStandard

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim stb As New StringBuilder()

        stb.Append("SELECT s.ID, c.Area, c.Tipologia, i.rag_soc as 'Azienda', ud.Nome + ' ' + ud.Cognome as 'Utente Creazione', s.DataScadenza as 'Data Scadenza', s.Descrizione, s.Note " & vbCrLf)
        stb.Append(" FROM PnlCtrl_Scadenze s " & vbCrLf)
        stb.Append(" LEFT JOIN PnlCtrl_Categorie c " & vbCrLf)
        stb.Append(" ON s.ID_Categoria = c.ID " & vbCrLf)
        stb.Append(" INNER JOIN imprese i " & vbCrLf)
        stb.Append(" ON s.piva=i.PIVA " & vbCrLf)
        stb.Append(" LEFT JOIN opta_utenti.dbo.utenti_dettagli ud " & vbCrLf)
        stb.Append(" ON ud.CodFisc=s.Utente " & vbCrLf)

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c As New ColonneNome("ID", "Azioni", "string")

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
                Case "ID"
                    cn._ColonnaDiSelezione = True
            End Select

            l.Add(cn)
        Next

        'Aggiungo la tabella in sessione
        Dim nomeVarDtInSession As String = "WAExport_Scadenze"
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
    Public Function Edit_Scadenza(chiave As Integer) As RispostaStandard
        Dim r As New rispostaStandard()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' salvo la chiave nella Session
        HttpContext.Current.Session.Add("PnlCtrl_ID_Elem", chiave)

        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function Del_Scadenza(chiave As Integer) As RispostaStandard
        Dim r As New rispostaStandard()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim elemW As New AgronicaCorePannelloDiControlloBIZ.Scadenze_W()

        r.RispostaOK = elemW.cancella(objParametri_Server, chiave)
        If Not r.RispostaOK Then
            r.Errore = "Errore durante la cancellazione della Scadenza"
        End If

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function AggiornaListaScadenze() As RispostaStandard

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim Scad_W As New AgronicaCorePannelloDiControlloBIZ.Scadenze_W()
        Dim Scad_R As New AgronicaCorePannelloDiControlloBIZ.Scadenze_R()
        Dim log As New DataProvider()
        Dim seq As New Agro_Sequenze()
        Dim esito As Boolean

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'ricerco tutti i patentini non ancora aggiunti
            Dim scadDaAgg As List(Of PnlCtrl_Scadenze) = Scad_R.Leggi_Scadenze_Patentino_DaAggiungereInPnlCtrl("", "", objParametri_Server)

            'ricerco i patentini la cui data di scadenza è stata modificata
            Dim scadDaMod As List(Of PnlCtrl_Scadenze) = Scad_R.Leggi_Scadenze_Patentino_DaModificareInPnlCtrl("", "", objParametri_Server)

            'SALVATAGGIO DEI DATI

            'aggiungo gli elementi nuovi
            For Each elem As PnlCtrl_Scadenze In scadDaAgg
                ' creo il nuovo ID
                elem.ID = seq.NuovoId_Tabella("PnlCtrl_Elementi_ID", 1, 2000000000, objParametri_Server)

                'scrivo la scadenza
                esito = Scad_W.aggiungi(objParametri_Server, elem)

                If Not esito Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                    Dim msg As String = "Errore durante la scrittura di un nuovo elemento, nessun salvataggio effettuato"
                    r.RispostaOK = False
                    r.Errore = msg
                    'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
                    Exit Try
                End If
            Next

            'modifico gli elementi già esistenti (di cui ho rilevato modifiche
            For Each elem As PnlCtrl_Scadenze In scadDaMod

                'scrivo la scadenza
                esito = Scad_W.modifica(objParametri_Server, elem)

                If Not esito Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                    Dim msg As String = "Errore durante la modifica di un elemento, nessun salvataggio effettuato"
                    r.RispostaOK = False
                    r.Errore = msg
                    'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
                    Exit Try
                End If
            Next

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            r.RispostaOK = True
            'Messaggi.AgroMsgBuonFine("Dati salvati correttamente", Me.Page)
            'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "transazione terminata correttamente: Creato la nuova NC")
        Catch e As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            Dim msg As String = "Errore durante la scrittura, nessun salvataggio effettuato. Dettagli: " & e.Message
            r.RispostaOK = False
            r.Errore = msg
            'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "errore durante la transazione: " & msg)
        End Try

        Return r

    End Function


End Class