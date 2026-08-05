Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ConnessioneWS
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Sub NewWS(ByRef objWS As Object, _
                                ByVal URL As String, _
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                )


        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.ConnessioneWS.NewWS()"
        Dim MessaggioErrore As String

        Try

            objWS.Url = URL

            Dim User, Pass, Host As String
            'controllo se è impostato il proxy
            ' leggo le impostazioni dell utente
            Dim objProxy As New AgronicaCoreVarieDAL.Proxy
            If objProxy.Leggi(User, Pass, Host, objParametri_Utenti) = True Then
                Dim cr As New System.Net.NetworkCredential(User, Pass)
                Dim pr As New System.Net.WebProxy(Host, 80)
                objWS.Credentials = cr
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


End Class


Public Class Proxy
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByRef Username As String, _
                            ByRef Password As String, _
                            ByRef Host As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim Flag As Boolean = False
        Dim App As String

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'recupero il flag
        App = objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_PROXY, objParametri)

        If App <> "" Then
            Flag = True
        End If

        If Flag = True Then
            'se c'è il flag allora posso procedere con il caricamento di password user e host
            'User
            Username = objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_USERNAME_PROXY, objParametri)

            'Passw
            Password = objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_PASSWORD_PROXY, objParametri)

            'host
            Host = objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_HOST_PROXY, objParametri)

        End If

        Return Flag

    End Function


End Class
