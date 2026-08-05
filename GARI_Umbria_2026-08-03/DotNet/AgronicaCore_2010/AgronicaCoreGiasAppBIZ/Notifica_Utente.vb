Public Class Notifica_Utente_R

    Public Function LeggiNotificheSottoscrivibili(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Dim xRead As New AgronicaCoreGiasAppDAL.Notifica_Utente_R
        Dim DT As New DataTable

        DT = xRead.LeggiNotificheSottoscrivibili(objParametri_Utenti.PivaSuperUser,
                                                 objParametri_Utenti.UtenteUsername,
                                                 objParametri,
                                                 objParametri_Utenti)

        Return DT

    End Function

End Class
Public Class Notifica_Utente_W

    Public Function PushNotifica(ByVal SubscriberId As String,
                                 ByVal IdServizio As Int32,
                                 ByVal Piattaforma As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim xRead As New AgronicaCoreGiasAppDAL.Notifica_Utente_R
        Dim DT As New DataTable

        DT = xRead.Leggi(objParametri_Utenti.PivaSuperUser,
                         SubscriberId,
                         IdServizio,
                         objParametri_Utenti.UtenteUsername,
                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                         "",
                         "",
                         objParametri,
                         objParametri_Utenti)

        If DT IsNot Nothing And DT.Rows.Count > 0 Then
            'Throw New Exception(String.Format("Notifica già sottoscritta dal subscriber {0} per il servizio {1}", SubscriberId, IdServizio.ToString))
            Return True
        End If

        Dim xWrite As New AgronicaCoreGiasAppDAL.Notifica_Utente_W

        Dim result = xWrite.PushNotifica(SubscriberId, IdServizio, Piattaforma, objParametri, objParametri_Utenti)

        Return result

    End Function

    Public Function PopNotifica(ByVal SubscriberId As String,
                                 ByVal IdServizio As Int32,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim xRead As New AgronicaCoreGiasAppDAL.Notifica_Utente_R
        Dim DT As New DataTable

        DT = xRead.Leggi(objParametri_Utenti.PivaSuperUser,
                         SubscriberId,
                         IdServizio,
                         objParametri_Utenti.UtenteUsername,
                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                         "",
                         "",
                         objParametri,
                         objParametri_Utenti)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            'Throw New Exception(String.Format("Sottoscrizione dal subscriber {0} per il servizio {1} non trovata.", SubscriberId, IdServizio.ToString))
            Return True
        End If

        Dim xWrite As New AgronicaCoreGiasAppDAL.Notifica_Utente_W

        Dim result = xWrite.PopNotifica(SubscriberId, IdServizio, objParametri, objParametri_Utenti)

        Return result

    End Function

End Class
