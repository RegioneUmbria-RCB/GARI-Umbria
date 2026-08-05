Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche.Appezzamento

Public Class Operazioni_Sementieri_R

End Class
Public Class Operazioni_Sementieri_W

    Public Function CancellaConflittiConLogSementieri(ByVal objDatiSementieri As DatiSementieri,
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim res As Boolean = False
        'Cancello eventuali conflitti

        Dim Interferenze As New AgronicaCoreGisBIZ.Interferenze
        Interferenze.GeneraDescrizione_Casella_Conflitto(CType(objDatiSementieri.Entita_Cod, Integer),
                                                         objParametri_Server, objParametri_Utenti)

        'Log operazione

        res = LoggaOperazioneDB(objParametri_Server,
                                objDatiSementieri.Entita_Cod,
                                3,
                                objDatiSementieri.DatiPassaggio,
                                objDatiSementieri.Sementi)

        If Not res Then
            Throw New Exception("Errore nel salvataggio del LOG Operazione.")
        End If

        'Metto in cache tutti i log relativi all'entità

        Dim leggiDati As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_R
        Dim dt As DataTable = leggiDati.Leggi(0,
                                              CType(objDatiSementieri.Entita_Cod, Integer),
                                              objParametri_Server, objParametri_Utenti)

        Dim aggiorna As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_W

        For Each riga As DataRow In dt.Rows

            res = aggiorna.ScriviInCache(CInt(riga("Sementieri_Sportello_LogOperazioni_COD")),
                                         0,
                                         riga,
                                         objParametri_Server)

            If Not res Then
                Throw New Exception("Errore nel salvataggio dei dati in cache.")
            End If
        Next

        Return res
    End Function
    Public Function LoggaOperazioneDB(ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByVal Ultimo_Entita_Cod As Integer,
                                       ByVal tipoOperazioneDB As Integer,
                                       ByVal DatiPassaggio As String,
                                       ByVal Sementi As String) As Boolean

        Dim res As Boolean = False

        If Not String.IsNullOrEmpty(DatiPassaggio) Then
            If DatiPassaggio.ToString.Split("|")(6) = 1 Then

                Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'Dim LogCod As Integer = AgroSequenze.Agronica_SequenzaTabelle_NuovoID("Sementieri_Sportello_LogOperazioni", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim LogCod As Integer = AgroSequenze.NuovoId_Tabella("Sementieri_Sportello_LogOperazioni", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                Dim SportelloCod As Integer = Sementi.ToString.Split("|")(4)

                Dim LoggaOperazioni As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_W
                res = LoggaOperazioni.Scrivi(LogCod, SportelloCod, tipoOperazioneDB, Ultimo_Entita_Cod, objParametri_Server)

            End If
        End If

        Return res
    End Function
End Class
