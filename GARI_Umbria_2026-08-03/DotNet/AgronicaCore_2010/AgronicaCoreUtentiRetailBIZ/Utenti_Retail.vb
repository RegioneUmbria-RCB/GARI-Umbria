Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Provisioning.Retail
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Converters
Imports Newtonsoft.Json.Linq

Public Class Utenti_Retail_R

    Public Function LeggiDaRiportare(ByVal codiceAttivazione As String,
                                     ByRef objParametri As AgronicaCoreParametri) As Nuovo_Utente_Retail_In

        Dim jsonString As String
        Dim DT As DataTable
        Dim daRiportare As New Nuovo_Utente_Retail_In

        Dim xRead As New AgronicaCoreUtentiRetailDAL.Utenti_Retail_R

        DT = xRead.LeggiDaRiportare(codiceAttivazione, objParametri)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Impossibile reperire le informazioni desiderate.")
        End If

        jsonString = DT.Rows(0)("JsonData").ToString

        Dim jsonObj = JObject.Parse(jsonString)

        daRiportare.Utente = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dati_Utente_Retail)(jsonObj("Utente").ToString)
        daRiportare.transazioneCommerciale = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dati_Transazione_Commerciale)(jsonObj("TransazioneCommerciale").ToString)
        daRiportare.dataFineValiditaUtente = Convert.ToDateTime(jsonObj("fineValidita").ToString).Date

        Return daRiportare

    End Function

End Class
Public Class Utenti_Retail_W
    Public Function Scrivi(ByVal codiceAttivazione As String,
                           ByVal nuovoUtenteRetail As Nuovo_Utente_Retail_In,
                           ByRef objParametri As AgronicaCoreParametri) As RispostaStandard
        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreUtentiRetailDAL.Utenti_Retail_W

        Dim datiObj As New JObject
        datiObj.Add("Utente", Newtonsoft.Json.JsonConvert.SerializeObject(nuovoUtenteRetail.Utente))
        datiObj.Add("fineValidita", nuovoUtenteRetail.dataFineValiditaUtente.ToString("dd/MM/yyyy"))
        datiObj.Add("TransazioneCommerciale", Newtonsoft.Json.JsonConvert.SerializeObject(nuovoUtenteRetail.transazioneCommerciale))

        resp.RispostaOK = xWrite.Scrivi(codiceAttivazione, datiObj.ToString, objParametri)

        If Not resp.RispostaOK Then
            resp.Errore = "Errore nel salvataggio dei dati del nuovo utente retail."
        Else
            resp.RispostaStringa = "Operazione terminata con successo."
        End If

        Return resp
    End Function

    Public Function CancellaLogicamenteUtenteCreato(ByVal codiceAttivazione As String,
                                                    ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreUtentiRetailDAL.Utenti_Retail_W

        resp = xWrite.CancellaLogicamenteUtenteCreato(codiceAttivazione, objParametri)

        Return resp
    End Function

    Public Function LoggaCambioMail(ByVal vecchiaEmail As String,
                                    ByVal nuovaEmail As String,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreUtentiRetailDAL.Utenti_Retail_W

        resp = xWrite.LoggaCambioMail(vecchiaEmail, nuovaEmail, objParametri)

        Return resp
    End Function
End Class
