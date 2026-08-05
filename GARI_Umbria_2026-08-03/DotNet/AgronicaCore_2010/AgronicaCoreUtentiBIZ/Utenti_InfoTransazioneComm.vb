Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Provisioning.Retail

Public Class Utenti_InfoTransazioneComm_R

End Class
Public Class Utenti_InfoTransazioneComm_W

    Public Function ScriviDaObj(ByVal datiTransazioneCommerciale As Dati_Transazione_Commerciale,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim xResp As Boolean

        Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_InfoTransazioneComm_W

        xResp = xWrite.ScriviDaObj(datiTransazioneCommerciale, objParametri, objParametri_Utenti)

        Return xResp

    End Function

End Class
