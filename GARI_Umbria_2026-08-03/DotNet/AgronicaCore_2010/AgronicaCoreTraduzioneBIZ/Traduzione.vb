Imports AgronicaCoreVarieBIZ

Public Class Traduzione_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function InviaTerminiTraduzione(ByVal Connessione_Matrice As String,
                                         ByVal Api_key As String,
                                          ByVal id_Progetto As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As RispostaStandard

        Dim messaggioErrore As String
        Dim xRead As New AgronicaCoreTraduzioneDAL.TraduzionePOEditor_R
        'Dim xWrite As New AgronicaCoreScadenzaProdottiDAL.TraduzionePOEditor_W

        Dim DT As New DataTable
        Dim xResp As New RispostaStandard

        Try


        Catch ex As Exception
            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            xResp.RispostaOK = False
            xResp.Errore = messaggioErrore

        End Try

        If String.IsNullOrWhiteSpace(xResp.Errore) Then
            xResp.RispostaStringa = "Operazione Terminata con successo"
        End If

        Return xResp

    End Function
End Class

