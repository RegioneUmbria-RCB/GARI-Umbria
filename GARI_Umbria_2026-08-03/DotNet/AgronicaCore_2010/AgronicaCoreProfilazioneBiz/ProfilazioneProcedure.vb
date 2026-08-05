
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Public Class ProfilazioneProcedure

    Public Function ProceduraDiSbloccoUtenteViaToken(
        t As String,
        objParametri_Utenti As AgronicaCoreParametri,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_SuperServer As AgronicaCoreParametri
        ) As RispostaStandard

        Dim rval As New RispostaStandard

        Dim utentiHlp As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim dtToken As DataTable =
            utentiHlp.Leggi_Token(t, "", objParametri_SuperServer)


        If dtToken.Rows.Count > 0 Then

            objParametri_Utenti.SuperUserUsername = dtToken.Rows(0)("SuperUser_Username")


            Dim datatoken As DateTime =
                dtToken.Rows(0)("Data_Rilascio")

            Dim letturaValiditaToken As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R
            Dim minutiValiditaToken As Integer =
                letturaValiditaToken.Leggi_Validita("", objParametri_SuperServer)

            'se il token è valido..:
            If Math.Abs(DateDiff(DateInterval.Minute, Now, datatoken)) < minutiValiditaToken Then


                objParametri_Utenti.UtenteUsername = dtToken.Rows(0)("Utente_Username")

                Dim scritturaSblocco As New AgronicaCoreUtentiDAL.Utenti_Write
                rval.RispostaOK =
                    scritturaSblocco.ModificaFlag(objParametri_Utenti.UtenteUsername, 0, objParametri_Utenti)


            End If

        End If


        Return rval

    End Function


End Class
