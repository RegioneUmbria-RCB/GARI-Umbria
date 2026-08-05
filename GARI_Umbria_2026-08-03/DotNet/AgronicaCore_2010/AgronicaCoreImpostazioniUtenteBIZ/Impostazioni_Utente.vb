Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreVarieBIZ

Public Class Impostazioni_Utente_R

    Public Function LeggiImpostazioniUtente(ByVal codiceImpostazione As Int32,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Username_1Utente_o_2SuperUser As Integer = 2) As String

        Dim DT As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim xReadToken As New AgronicaCoreUtentiDAL.Utenti_Token_R

        DT = xRead.Leggi(
            codiceImpostazione,
            Username_1Utente_o_2SuperUser,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri
            )

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            Throw New Exception("Si è verificato un errore nel recupero delle impostazioni utente.")
        End If

        Return DT.Rows(0)("Impostazione_Valore_1")
    End Function

End Class
Public Class Impostazioni_Utente_W
    Public Function SalvaTokenImpostazioniUtenti(ByVal token As String,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                 ByRef objParametri_Super_Server As AgronicaCoreParametri
                                                 ) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim DT As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim xReadToken As New AgronicaCoreUtentiDAL.Utenti_Token_R

        Try

            DT = xRead.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                         2,
                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                         "",
                         "",
                         objParametri_Utenti)

            If DT Is Nothing OrElse DT.Rows.Count > 1 Then
                Throw New Exception("Si è verificato un errore nel recupero delle impostazioni utente.")
            End If

            Dim impostazioni As New ImpostazioniAPP

            Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            If DT IsNot Nothing AndAlso DT.Rows.Count = 1 Then

                impostazioni = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ImpostazioniAPP)(DT.Rows(0)("Impostazione_Valore_1"))

                impostazioni.Token = token

                resp.RispostaOK = xWrite.Modifica2(objParametri.SuperUserUsername,
                                               TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                               Newtonsoft.Json.JsonConvert.SerializeObject(impostazioni),
                                               "",
                                               "",
                                               "",
                                               CostantiPersonalizzate.AGRODATAINIZIO,
                                               CostantiPersonalizzate.AGRODATAFINE,
                                               objParametri_Utenti)
            Else
                'TODO: Impostare i valori di default delle impostazioni

                impostazioni.Token = token

                resp.RispostaOK = xWrite.Scrivi2(objParametri.SuperUserUsername,
                                             TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                                             Newtonsoft.Json.JsonConvert.SerializeObject(impostazioni),
                                             "",
                                             "",
                                             "",
                                             CostantiPersonalizzate.AGRODATAINIZIO,
                                             CostantiPersonalizzate.AGRODATAFINE,
                                             objParametri_Utenti)

            End If

            If resp.RispostaOK Then
                resp.RispostaStringa = "Operazione terminata con successo."
            Else
                Throw New Exception("Errore nel salvataggio del token.")
            End If

            Return resp

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = ex.Message

            Return resp
        End Try

    End Function

End Class
