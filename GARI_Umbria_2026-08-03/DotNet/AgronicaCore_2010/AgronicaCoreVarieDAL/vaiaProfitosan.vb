Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider

Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Web
Imports System

Public Class vaiaProfitosan
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' uu --> utente superuser
    ''' pp --> password superuser
    ''' uu2 --> utente
    ''' pp2 --> password 
    ''' nuovo --> nuova finestra o no
    Public Shared Function getUrlSimple(objParametri_Server, objParametri_Utenti, nuovo, flag_2023)
        'codifico

        Dim utente As String = ""

        If flag_2023 Then
            utente = Stringa_Codifica(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") & "|" & RandomNumberWithPad() & "|" & objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
        Else
            utente = Stringa_Codifica(objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
        End If

        Dim objutente As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim dt As DataTable = objutente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti, objParametri_Server.SuperUserUsername)

        'codifico
        Dim pass As String = Stringa_Codifica(dt.Rows(0).Item("password"), AgroKey_EncoderDecoder)

        'codifico
        Dim utente2 As String = Stringa_Codifica(objParametri_Server.UtenteUsername, AgroKey_EncoderDecoder)
        dt = objutente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti, objParametri_Server.UtenteUsername)

        'codifico
        Dim pass2 As String = Stringa_Codifica(dt.Rows(0).Item("password"), AgroKey_EncoderDecoder)

        'codifico
        nuovo = Stringa_Codifica(nuovo, AgroKey_EncoderDecoder)

        'Utente e password superuser codificati
        'Utente e password utente codificati
        Dim strConnessione As String = "uu=" & utente & "&pp=" & pass & "&uu2=" & utente2 & "&pp2=" & pass2 & "&nuovo=" & nuovo

        Return strConnessione
    End Function


    'frCod non codificato
    Public Shared Function getUrlProdotto(objParametri_Server, objParametri_Utenti, fr_cod, nuovo, flag_2023)
        Return getUrlSimple(objParametri_Server, objParametri_Utenti, nuovo, flag_2023) & "&f=" & fr_cod
    End Function


    Private Shared Function RandomNumberWithPad() As String
        Randomize()

        Dim random_int As Integer = CInt(Int((999.999 * Rnd()) + 0))

        Dim NumberWithPad As String = "000" & random_int.ToString()

        Return NumberWithPad
    End Function



End Class