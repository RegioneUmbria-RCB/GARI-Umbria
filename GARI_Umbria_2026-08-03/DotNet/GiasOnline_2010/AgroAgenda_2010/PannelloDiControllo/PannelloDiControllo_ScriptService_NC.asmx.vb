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
Public Class PannelloDiControllo_ScriptService_NC_OPTA
    Inherits System.Web.Services.WebService


    '<WebMethod(EnableSession:=True)> _
    'Public Function Edit_NC(chiave As Integer) As RispostaStandard
    '    Dim r As New rispostaStandard()

    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        r.Sessione = False
    '        Return r
    '    End If

    '    ' salvo la chiave nella Session
    '    HttpContext.Current.Session.Add("PnlCtrl_ID_NC", chiave)

    '    r.RispostaOK = True
    '    Return r
    'End Function

    <WebMethod(EnableSession:=True)> _
    Public Function Del_NC(chiave As Integer) As RispostaStandard
        Dim r As New rispostaStandard()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim elemW As New AgronicaCorePannelloDiControlloBIZ.NonConformita_W()

        r.RispostaOK = elemW.cancellaNC(objParametri_Server, chiave)
        If Not r.RispostaOK Then
            r.Errore = "Errore durante la cancellazione della Scadenza"
        End If

        Return r
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Function LeggiNC(ID_NC As Integer) As rispostaStandard(Of List(Of PnlCtrl_NonConformita))

        Dim r As New rispostaStandard(Of List(Of PnlCtrl_NonConformita))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim elemR As New AgronicaCorePannelloDiControlloBIZ.NonConformita_R()

        Try
            Dim listaDetNC As List(Of PnlCtrl_NonConformita) = elemR.leggi_NonConformita(objParametri_Server, ID_NC)
            r.RispostaStringa = listaDetNC
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function SalvaNC(controlli As String, nDettagli As Integer) As rispostaStandard

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim NC_R As New AgronicaCorePannelloDiControlloBIZ.NonConformita_R()
        Dim NC_W As New AgronicaCorePannelloDiControlloBIZ.NonConformita_W()
        Dim NC_Det_R As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_R()
        Dim NC_Det_W As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_W()

        Dim seq As New Agro_Sequenze()
        Dim esito As Boolean = False

        'VERIFICA DEI DATI
        '????????????????????????

        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'interpreto la stringa come un json
            Dim jSonDatiControlli As JObject = JObject.Parse(controlli)

            'AREA COMUNE
            Dim ID_NC As Integer = CInt(jSonDatiControlli("txbID_NC").ToString())
            Dim ID_Gravita As Integer = CInt(jSonDatiControlli("ddlGravita").ToString())
            Dim ID_Categoria As Integer = CInt(jSonDatiControlli("ddlTipologia").ToString())
            Dim Piva As String = jSonDatiControlli("ddlAzienda").ToString()

            'se l'ID_NC è -1 e quindi l'intera non conformità è nuova, creo un nuovo ID
            If ID_NC = -1 Then
                'Creo la nuova Chiave
                ID_NC = seq.NuovoId_Tabella("PnlCtrl_Elementi_ID_NC", 1, 2000000000, objParametri_Server)

                'Salvo su DB la nuova NC
                esito = NC_W.aggiungi(objParametri_Server, ID_NC, ID_Categoria, Piva, _
                                      ID_Gravita, Nothing, Nothing)
            Else
                'NB: non faccio direttamente la modifica perché i campi tabDettaglio non li modifico ed il nothing è convertito in DBNULL del core DAL
                'leggo la NC
                Dim NC As PnlCtrl_NonConformita = NC_R.leggi_NonConformita(objParametri_Server, ID_NC).First()

                'cambio i campi che mi interessano della NC
                NC.ID_Categoria = ID_Categoria
                NC.ID_Gravita = ID_Gravita
                NC.Piva = Piva

                'Modifico la NC
                esito = NC_W.modifica(objParametri_Server, NC)
            End If

            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                r.RispostaOK = False
                r.Errore = "Errore durante la scrittura della testata della NC, nessun salvataggio effettuato."
                Exit Try
            End If

            'DETTAGLI
            For i As Integer = 1 To nDettagli
                Dim ID_NC_Det As Integer = CInt(jSonDatiControlli("PnlDet" & i & "_hfID_Elem").ToString())
                Dim ID_Stato As Integer = CInt(jSonDatiControlli("PnlDet" & i & "_ddlStato").ToString())
                Dim data As DateTime = CDate(jSonDatiControlli("PnlDet" & i & "_txbData").ToString())
                Dim utente As String = jSonDatiControlli("PnlDet" & i & "_ddlUtente").ToString()
                Dim descrizione As String = jSonDatiControlli("PnlDet" & i & "_txbDescrizione").ToString()
                Dim note As String = jSonDatiControlli("PnlDet" & i & "_txbNote").ToString()
                Dim ID_ListaAllegati As Integer? = If(IsNumeric(jSonDatiControlli("PnlDet" & i & "_hfID_ListaAllegati").ToString()), CInt(jSonDatiControlli("PnlDet" & i & "_hfID_ListaAllegati").ToString()), Nothing)

                'se l'ID è -1 e quindi l'elemento non esisteva, allora creo la nuova chiave
                If ID_NC_Det = -1 Then
                    'Creo la nuova chiave
                    ID_NC_Det = seq.NuovoId_Tabella("PnlCtrl_Elementi_ID", 1, 2000000000, objParametri_Server)

                    'aggiungo il dettaglio della NC
                    esito = NC_Det_W.aggiungi(objParametri_Server, ID_NC_Det, ID_NC, utente, data, ID_Stato, ID_ListaAllegati, descrizione, note)
                Else 'dato che la NC esiste già, la modifico soltanto
                    'qui faccio direttamente la modifica perché cambio tutti i campi dell'oggetto
                    esito = NC_Det_W.modifica(objParametri_Server, ID_NC_Det, utente, data, ID_Stato, _
                                              ID_ListaAllegati, descrizione, note)
                End If

                If Not esito Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                    r.RispostaOK = False
                    r.Errore = "Errore durante la scrittura del dettaglio, nessun salvataggio effettuato."
                    Exit Try
                End If

            Next

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            'Messaggi.AgroMsgBuonFine("Dati salvati correttamente", Me.Page)
            'log.Scrivi_LOG(objParametri_Server, Me.GetType.Name & "." & System.Reflection.MethodBase.GetCurrentMethod.Name, "transazione terminata correttamente: Creato la nuova NC")
            r.RispostaOK = True

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            r.RispostaOK = False
            r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Dim listaAllegati As New List(Of PnlCtrl_Allegati)

        Return r

    End Function


    <WebMethod(EnableSession:=True)> _
    Public Function LeggiTitoloAreaComune(ID_Categoria As Integer, ID_NC As Integer) As RispostaStandard

        Dim r As New rispostaStandard()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim stringaTitolo As String

        Dim cat_R As New AgronicaCorePannelloDiControlloBIZ.Categorie_R
        Dim cat As PnlCtrl_Categorie = cat_R.leggi_PnlCtrl_Categorie( _
                                            objParametri_Server, ID_Categoria).First()

        Dim elem_R As New AgronicaCorePannelloDiControlloBIZ.NonConformita_R
        Dim nc As PnlCtrl_NonConformita = elem_R.leggi_NonConformita( _
                                        objParametri_Server, ID_NC).First()

        Select Case cat.Area
            Case "Macchine"
                Dim elencoChiaviTab As String() = nc.TabDettaglio_Chiave.Split("~")
                Dim mac_cod, sa_cod As Integer
                Dim mac_codStr, azienda, centroAz, NomeMacchina As String
                Dim dt As DataTable

                'Cerco il Mac_Cod
                mac_codStr = elencoChiaviTab.Where(Function(x) x.StartsWith("Mac_Cod")).First()
                mac_cod = CInt(mac_codStr.Split("=")(1))

                'leggo la macchina ed il sa_cod
                Dim parcoM_R As New AgronicaCoreContabDAL.Parco_Macchine_R()
                dt = parcoM_R.Leggi2(nc.Piva, mac_cod, CostantiPersonalizzate.SACOD_NOFILTRO, 0, "", False, "", "", objParametri_Server)
                NomeMacchina = dt.Rows(0).Item("mac_des")
                sa_cod = dt.Rows(0).Item("sa_cod")

                'leggo il nome dell'azienda
                Dim imp_R As New AgronicaCoreAnagrafeDAL.Imprese_Read()
                dt = imp_R.Leggi(nc.Piva, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                azienda = dt.Rows(0).Item("rag_soc")

                'Azienda / Centro Aziendale (vale solo se > 0 , altrimenti indica la visibilità) / Nome Macchina
                If sa_cod > 0 Then
                    'leggo il nome del centro aziendale
                    Dim centroAz_R As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()
                    dt = centroAz_R.Leggi(nc.Piva, sa_cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    centroAz = dt.Rows(0).Item("sa_nome")

                    stringaTitolo = "Azienda:" & azienda & " \ Centro Aziendale:" & centroAz & " \ Macchina:" & NomeMacchina
                Else
                    stringaTitolo = "Azienda:" & azienda & " \ Macchina:" & NomeMacchina
                End If

            Case "Zone"
                stringaTitolo = "Zone"
            Case "Forni"
                stringaTitolo = "Forni"
        End Select

        r.RispostaOK = True
        r.RispostaStringa = stringaTitolo

        Return r
    End Function

End Class