Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreGHGBIZ
Imports AgronicaCoreModelsSTD.attivita

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Attivita
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Attivita_APP(ByVal piva_superuser As String,
                                     ByVal objP_super_server As String,
                                ByVal objP_server As String,
                                ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim leggi As New AgronicaCoreContabDAL.Attivita_R

            Dim Dt As DataTable = leggi.Leggi_APP("", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each dr As DataRow In Dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("ID_Attivita", dr("ID_Attivita")), New JProperty("Desc", dr("Desc")), New JProperty("Attivita_Extra_Campagna", dr("Attivita_Extra_Campagna")), New JProperty("Piva", dr("Piva")), New JProperty("Sa_Cod", dr("Sa_Cod"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiungi(ByVal objP_server As String, Lav_Cod As Integer, ID_Attivita As Integer, Tariffa_Cod As Integer, Descrizione As String, Sigla As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If Not IsNumeric(Lav_Cod) OrElse Lav_Cod = 0 Then
            r.Errore = "Lav_Cod non valorizzato"
            Return r
        End If

        If Not IsNumeric(ID_Attivita) Then
            r.Errore = "ID_Attivita non valorizzato"
            Return r
        End If

        If Trim(Descrizione) = "" Then
            r.Errore = "Descrizione non valorizzato"
            Return r
        End If

        If Trim(Sigla) = "" Then
            r.Errore = "Sigla non valorizzata"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objConnessione As New AgronicaCoreDataProvider.ConnessioniTransazioni

        Try

            Dim res As Boolean = False

            Dim objAttivita_R As New AgronicaCoreContabDAL.Attivita_R
            Dim ObjSequenze_R As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim FiltroAggiuntivo As String = ""

            objConnessione.ApriConnessione(True, objParametri_Server)



            'Se non mi è passato l'ID_Attivita, lo genero io in maniera automatica

            '    Dim DtAttivitaTutte As DataTable = objAttivita_R.Leggi(0, "", "", objParametri_Server)
            '    Dim max As Integer = (From dr As DataRow In DtAttivitaTutte.Rows Select dr("ID_Attivita")).Max()
            '    ID_Attivita = max + 1
            'End If

            'Controllo Univocità Sigla
            FiltroAggiuntivo = "(Upper(A.Sigla) = '" & Trim(UCase(Sigla)) & "')"
            Dim DtAttivita As DataTable = objAttivita_R.Leggi(ID_Attivita, FiltroAggiuntivo, "", objParametri_Server)

            If DtAttivita.Rows.Count <> 0 Then

                r.Errore = "Sigla già presente in archivio"
                Return r

            End If

            If ID_Attivita = 0 Then

                ID_Attivita = ObjSequenze_R.NuovoId_Tabella("Attivita",
                                                            1,
                                                            2000000000,
                                                            objParametri_Server)
            End If


            Dim objAttivita_W As New AgronicaCoreContabDAL.Attivita_W
            res = objAttivita_W.ScriviCompleta(objParametri_Server.PivaSuperUser, ID_Attivita, Descrizione, Tariffa_Cod, Sigla, 0, "", "|0|", "|0|", 1, 0, -1, 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore durante la scrittura dell'Attività")
            End If



            Dim objAttivitaxOperazioni As New AgronicaCoreContabDAL.AttivitaXOperazioni_W
            res = objAttivitaxOperazioni.Scrivi(ID_Attivita, Lav_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore durante la scrittura dell'AttivitàXOperazioni")
            End If

            ' commit
            objConnessione.ChiudiTransazione(1, objParametri_Server)
            objConnessione.ChiudiConnessione(objParametri_Server)

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            ' rollback
            objConnessione.ChiudiTransazione(2, objParametri_Server)
            objConnessione.ChiudiConnessione(objParametri_Server)

            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaDescrizioneAttivita(ByVal objP_server As String, ID_Attivita As Integer, Descrizione As String, Sigla As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If Not IsNumeric(ID_Attivita) OrElse ID_Attivita = 0 Then
            r.Errore = "ID_Attivita non valorizzato"
            Return r
        End If

        If Trim(Descrizione) = "" Then
            r.Errore = "Descrizione non valorizzato"
            Return r
        End If

        If Trim(Sigla) = "" Then
            r.Errore = "Sigla non valorizzata"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim objAttivita_R As New AgronicaCoreContabDAL.Attivita_R
            Dim FiltroAggiuntivo As String = ""

            'Controllo Univocità Sigla
            FiltroAggiuntivo = "(A.Id_Attivita <> " & ID_Attivita & " And Upper(A.Sigla) = '" & Trim(UCase(Sigla)) & "')"
            Dim DtAttivita As DataTable = objAttivita_R.Leggi(0, FiltroAggiuntivo, "", objParametri_Server)

            If DtAttivita.Rows.Count <> 0 Then

                r.Errore = "Sigla già presente in archivio"
                Return r

            End If

            Dim objAttivita_W As New AgronicaCoreContabDAL.Attivita_W
            Dim res As Boolean = objAttivita_W.ModificaDescrizione(ID_Attivita, Descrizione, Sigla, objParametri_Server)

            If Not res Then
                Throw New Exception("Errore durante la modifica dell'Attività")
            End If

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal objP_server As String, ID_Attivita As Integer, Lav_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If Not IsNumeric(ID_Attivita) OrElse ID_Attivita = 0 Then
            r.Errore = "ID_Attivita non valorizzato"
            Return r
        End If

        If Not IsNumeric(Lav_Cod) OrElse Lav_Cod = 0 Then
            r.Errore = "Lav_Cod non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim res As Boolean = False

            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            ' controllo se sono già state registrate delle attività - Come Operazioni
            Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
            Dim DtAg As DataTable = objAgenda.Leggi("", 0, 0, 0,
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "Id_Attivita=" & ID_Attivita.ToString, "", objParametri_Server)

            If DtAg.Rows.Count > 0 Then
                Throw New Exception("L'attività selezionata è associata a delle operazioni GIAS (come Altra Lavorazione).")
            End If

            ' controllo se sono già state registrate delle attività - Come costi accessori
            Dim objMovDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim DtMov As DataTable = objMovDettagli.Leggi("", 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0,
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "Id_Attivita=" & ID_Attivita.ToString, "", objParametri_Server)

            If DtMov.Rows.Count > 0 Then
                Throw New Exception("L'attività selezionata è associata a delle operazioni GIAS (nei costi accessori).")
            End If

            'Cancello OperazioneXAttività
            Dim objAttivitaxOperazioni_W As New AgronicaCoreContabDAL.AttivitaXOperazioni_W
            res = objAttivitaxOperazioni_W.Cancella(ID_Attivita, Lav_Cod, "", objParametri_Server)

            If Not res Then
                Throw New Exception("Cancellazione OperazioneXAttività")
            End If

            'Controllo se vi sono altre associazioni:
            Dim objAttivitaxOperazioni_R As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim dtAxO As DataTable = objAttivitaxOperazioni_R.Leggi(ID_Attivita, 0, "", "", objParametri_Server)

            'Se vi sono altre associazioni, non cancello l'attività altrimenti sì
            If dtAxO.Rows.Count = 0 Then

                'Cancello l'attività
                Dim objAttivita As New AgronicaCoreContabDAL.Attivita_W
                res = objAttivita.Cancella(ID_Attivita, "", objParametri_Server)

                If Not res Then
                    Throw New Exception("Cancellazione Attività")
                End If
            End If


            ' commit
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            ' rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_AttivitaPersonalizzata_Modello(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata))

        Try
            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InDataAttivitaPersonalizzata As CoreWS_Generic(Of LeggiAttivitaPersonalizzata) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAttivitaPersonalizzata))(JsonConvert.SerializeObject(InData, settings), settings)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataAttivitaPersonalizzata.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataAttivitaPersonalizzata.objP.objP_utenti)

            Dim objAttivita As New AgronicaCoreContabBIZ.AttivitaPersonalizzata

            Dim attivitaPersonalizzataList = objAttivita.LeggiAttivitaPersonalizzata(0, (InDataAttivitaPersonalizzata.InData).Operazioni_scelte(0).primaryKey.codice, objParametri_Server)

            r.RispostaStringa = attivitaPersonalizzataList
            r.RispostaOK = True

        Catch ex As Exception

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
End Class