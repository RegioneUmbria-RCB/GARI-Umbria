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
Public Class PannelloDiControllo_ScriptService_Allegati
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)> _
    Public Function LeggiAllegatiDaLista(ID_Lista As Integer) As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim all_R As New Allegati_R()
            Dim listaAll As List(Of PnlCtrl_Allegati) = all_R.leggi_PnlCtrl_AllegatiDaIDLista( _
                objParametri_Server, ID_Lista)

            'Creo e popolo una lista di listItem per popolare il controllo
            Dim lista As New List(Of ListItem)
            lista.Add(New ListItem("Selezionare (" & listaAll.Count & ")...", -1)) 'elemento vuoto iniziale

            For Each a As PnlCtrl_Allegati In listaAll
                lista.Add(New ListItem(a.NomeFile, a.ID))
            Next

            r.RispostaOK = True
            r.RispostaStringa = lista

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function LeggiAllegato(ID_Allegato As Integer) As rispostaStandard(Of PnlCtrl_Allegati)
        Dim r As New rispostaStandard(Of PnlCtrl_Allegati)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim all_R As New Allegati_R()
            Dim listaAll As List(Of PnlCtrl_Allegati) = all_R.leggi_PnlCtrl_Allegati( _
                                                                    objParametri_Server, ID_Allegato)

            If listaAll.Count <= 0 Then
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: Nessun allegato trovato con ID " & ID_Allegato
                Exit Try
            End If

            r.RispostaOK = True
            r.RispostaStringa = listaAll.First()

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    'PER SALVARE GLI ALLEGATI
    <WebMethod(EnableSession:=True)> _
    Public Function salvaAllegato(idDet As Integer, idElem As Integer, idLista As Integer, _
                                  idCategoria As Integer, nomeFile As String, fileBase64Enc As String, _
                                  descrizione As String) As RispostaStandard

        Dim r As New rispostaStandard()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Const PERCORSOALLEGATI As String = "C:\GIASLAN\_Allegati\"
        Dim all_W As New AgronicaCorePannelloDiControlloBIZ.Allegati_W
        Dim seq As New Agro_Sequenze()

        'VERIFICA DEI DATI
        Try
            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'se il file è vuoto...
            If fileBase64Enc = "" Then
                If idElem = -1 Then
                    'se il file caricato è vuoto e NON sono in modifica esco perché vuol dire che c'è stato un errore
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                    r.RispostaOK = False
                    r.Errore = "Errore durante il salvataggio dell'allegato, nessun file caricato."
                    Exit Try
                End If
            Else 'se il file è stato caricato, lo salvo
                Dim fileByteArray As Byte() = Convert.FromBase64String(fileBase64Enc)
                System.IO.File.WriteAllBytes(PERCORSOALLEGATI & nomeFile, fileByteArray)
            End If

            Dim esito As Boolean = False 'var su cui memorizzo se la scrittura è andata a buon fine

            'Se non esisteva la lista di allegati per l'elemento, la creo
            If idLista = -1 Then
                idLista = seq.NuovoId_Tabella("PnlCtrl_Allegati_ID_Lista", 1, 2000000000, objParametri_Server)
            End If

            ' se l'allegato è nuovo (id=-1), genero l'id dalle agrosequenze e poi lo aggiungo
            If idElem = -1 Then
                idElem = seq.NuovoId_Tabella("PnlCtrl_Allegati_ID", 1, 2000000000, objParametri_Server)

                'aggiungo l'allegato alla tabella
                esito = all_W.aggiungi(objParametri_Server, idElem, idLista, _
                                                      nomeFile, descrizione, idCategoria)
            Else
                'se l'allegato era già esistente, lo modifico
                esito = all_W.modifica(objParametri_Server, idElem, idLista, _
                                                      nomeFile, descrizione, idCategoria)
            End If

            'controllo che l'inserimento/modifica sia andata a buon fine
            If Not esito Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                r.RispostaOK = False
                r.Errore = "Errore durante il salvataggio dell'allegato, nessun salvataggio effettuato"
                Exit Try
            End If

            If Not IsNothing(idDet) AndAlso idDet <> -1 Then
                'leggo l'elemento
                Dim N_Det_R As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_R
                Dim det As PnlCtrl_NonConformita_Dettagli = N_Det_R.leggi_DettaglioNC(objParametri_Server, idDet).First()
                det.ID_ListaAllegati = idLista

                'aggancio l'allegato al dettaglio della NC e salvo
                Dim NC_Det_W As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_W
                esito = NC_Det_W.modifica(objParametri_Server, det)

                If Not esito Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                    r.RispostaOK = False
                    r.Errore = "Errore durante l'aggancio dell'allegato all'elemento, nessun salvataggio effettuato"
                    Exit Try
                End If
            End If

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2
            r.RispostaOK = True

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            r.RispostaOK = False
            r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato"
        End Try

        Return r
    End Function


End Class