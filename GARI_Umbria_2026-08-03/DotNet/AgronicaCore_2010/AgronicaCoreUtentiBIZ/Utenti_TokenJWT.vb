Imports System.Security.Cryptography
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class Utenti_TokenJWT_W

End Class

Public Class Utenti_TokenJWT_R

    Public Function LeggiConTokenID(ByVal Token_ID As String,
                                    ByVal PivaSuperUser As String,
                                    ByVal Username As String,
                                    ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable
        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
        Dim DT As DataTable

        DT = xRead.Leggi(Token_ID, "", PivaSuperUser, Username, "", "", "", "", "", objParametri_Super_Server)

        Return DT

    End Function

    'Commentato poiché è cambiata la logica di autenticazione e la combinazione di PivaSuperUser, username, objP_server e objP_Super_Server non basta più per individuare un singolo token
    'Public Function LeggiPerConnessione(ByVal PivaSuperUser As String,
    '                                    ByVal Username As String,
    '                                    ByVal objP_Server As String,
    '                                    ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable
    '    Dim xRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
    '    Dim DT As DataTable

    '    DT = xRead.Leggi("", "", PivaSuperUser, Username, objP_Server, "", "", "", "", objParametri_Super_Server)

    '    Return DT

    'End Function

    Public Function LeggiConRefreshToken(ByVal Refresh_Token As String,
                                         ByVal PivaSuperUser As String,
                                         ByVal Username As String,
                                         ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable
        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
        Dim DT As DataTable

        DT = xRead.Leggi("", Refresh_Token, PivaSuperUser, Username, "", "", "", "", "", objParametri_Super_Server)

        Return DT

    End Function

    'Commentato poiché è cambiata la logica di autenticazione e non è più necessario verificare l'esistenza dei token
    'Questo metodo darebbe problemi con la nuova logica di autenticazione poiché possono esistere più token con lo stesso PivaSuperUser, username, objP_server e objP_Super_Server

    'Public Function VerificaEsistenzaToken(ByVal PivaSuperUser As String,
    '                                       ByVal Username As String,
    '                                       ByVal objP_Server As String,
    '                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As String
    '    Dim xRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
    '    Dim DT As DataTable
    '    Dim resp As String

    '    DT = LeggiPerConnessione(PivaSuperUser, Username, objP_Server, objParametri_Super_Server)

    '    If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
    '        resp = DT.Rows(0)("Token_ID").ToString
    '    ElseIf Not IsNothing(DT) AndAlso DT.Rows.Count > 1 Then
    '        Throw New Exception("Sono presenti più token con lo stesso ID.")
    '    Else
    '        resp = ""
    '    End If

    '    Return resp

    'End Function

    Public Function ControllaValiditaToken(ByVal Token_ID As String,
                                           ByVal PivaSuperUser As String,
                                           ByVal Username As String,
                                           ByRef objParametri_Super_Server As AgronicaCoreParametri) As Boolean
        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
        Dim DT As DataTable
        Dim resp As Boolean = False

        DT = LeggiConTokenID(Token_ID, PivaSuperUser, Username, objParametri_Super_Server)

        If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
            Dim validitaToken As Date = DT.Rows(0)("Validita_Fine")
            If validitaToken >= Date.Now Then
                resp = True
            End If

        ElseIf Not IsNothing(DT) AndAlso DT.Rows.Count > 1 Then
            Throw New Exception("Sono presenti più token con lo stesso ID.")
        Else
            Throw New Exception("Token non presente.")
        End If

        Return resp

    End Function

End Class
