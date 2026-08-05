Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class PreferenzeUtente
    Public Chili_Litri As Boolean
    Public UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO As Boolean
    Public UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE As Boolean
    Public UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS As Boolean

    Public Sub New(ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Chili_Litri = False
        UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO = False
        UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = False
        UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS = False


        'leggo le eventuali IMPOSTAZIONI UTENTE
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtImpostazioni As New DataTable
        dtImpostazioni = objUtenti.Leggi(0,
                                         1,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "",
                                         "",
                                         objParametri_Utenti)


        If Not dtImpostazioni Is Nothing AndAlso dtImpostazioni.Rows.Count > 0 Then

            For i = 0 To dtImpostazioni.Rows.Count - 1

                'Impostazioni sia per scrittura che modifica
                Select Case CInt(dtImpostazioni.Rows(i).Item("Impostazione_Cod"))

                    Case enum_Impostazioni_Utenti.UTENTE_COD_CHILI_LITRI
                        Select Case dtImpostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Chili_Litri = False
                            Case Else
                                Chili_Litri = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO
                        Select Case dtImpostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO = False
                            Case Else
                                UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE
                        Select Case dtImpostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = False
                            Case Else
                                UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS
                        Select Case dtImpostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS = False
                            Case Else
                                UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS = True
                        End Select
                End Select
            Next
        End If
    End Sub

End Class


