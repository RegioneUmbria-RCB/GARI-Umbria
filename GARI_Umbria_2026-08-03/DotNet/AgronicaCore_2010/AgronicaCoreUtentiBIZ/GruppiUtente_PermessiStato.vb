Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GruppiUtente_PermessiStato

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Private _impGruppiPermessiStato As Boolean?

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)

        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

        If IsNothing(objParametriServer) And IsNothing(objParametriUtenti) Then
            Throw New Exception("Parametri Server e Utenti non definiti.")
        End If

    End Sub

    Public Function IsUtenteSuperUser() As Boolean

        Dim utenteSuperUser As Boolean

        If Not IsNothing(_objParametriServer) Then

            utenteSuperUser = IsUtenteSuperUser(_objParametriServer)

        Else

            utenteSuperUser = IsUtenteSuperUser(_objParametriUtenti)

        End If

        Return utenteSuperUser

    End Function

    Private Function IsUtenteSuperUser(ByVal objParametri As AgronicaCoreParametri) As Boolean

        Return (objParametri.UtenteUsername = objParametri.SuperUserUsername)

    End Function

    Public Function LeggiGruppoUtente(ByVal messaggioBase As String) As Integer

        Dim gruppoUtenteCod As Integer = 0

        Dim messaggioAggiuntivo As String = ""

        If IsNothing(_objParametriUtenti) Then

            messaggioAggiuntivo = "Impossibile determinare gruppo utente: Contattare assistenza."

            LanciaEccezione(messaggioBase, messaggioAggiuntivo)

        End If

        Dim objGruppoUtente As New Utenti_xGruppi_Utente_R

        Dim dtGruppoUtente = objGruppoUtente.Leggi(_objParametriUtenti.UtenteUsername,
                                                   0,
                                                   "",
                                                   "",
                                                   _objParametriUtenti)

        Select Case True

            Case IsNothing(dtGruppoUtente) OrElse dtGruppoUtente.Rows.Count = 0

                messaggioAggiuntivo = "Utente non appartenente a nessun gruppo: Contattare assistenza."

            Case dtGruppoUtente.Rows.Count = 1

                gruppoUtenteCod = dtGruppoUtente.Rows(0)("Gruppi_Utente_Cod")

            Case Else

                messaggioAggiuntivo = "Utente appartenente a più gruppi: Contattare assistenza."

        End Select

        If Not String.IsNullOrEmpty(messaggioAggiuntivo) Then

            LanciaEccezione(messaggioBase, messaggioAggiuntivo)

        End If

        Return gruppoUtenteCod

    End Function

    Private Sub LanciaEccezione(ByVal messaggioBase As String,
                                ByVal messaggioAggiuntivo As String)

        Dim messaggioEccezione = String.Format("{0} {1}",
                                               messaggioBase,
                                               messaggioAggiuntivo)

        Throw New Exception(messaggioEccezione)

    End Sub

    Public Function LeggiPermessiStato(ByVal GruppoUtente As Integer,
                                       ByVal ServizioCod As Integer,
                                       ByVal StatoCod As Integer
                                       ) As PermessiStato

        Dim permessiStato As New PermessiStato

        If Not _impGruppiPermessiStato.HasValue Then
            Dim handleUtentiImp As New Utenti_Impostazioni_Read
            _impGruppiPermessiStato = handleUtentiImp.LeggiConDefault(enum_Impostazioni_Utenti.Workflow_GruppiUtente_PermessiStato, 2, "1", _objParametriUtenti)
        End If

        Select Case True

            Case IsUtenteSuperUser(), _impGruppiPermessiStato.Value = False

                permessiStato.Visualizza = True
                permessiStato.Modifica = True
                permessiStato.Cancella = True

            Case IsNothing(_objParametriUtenti)

                Throw New Exception("Impossibile leggere i permessi utente per stato: Contattare Assistenza.")

            Case Else

                permessiStato = LetturaEffettivaPermessiStato(GruppoUtente, ServizioCod, StatoCod)

        End Select

        Return permessiStato

    End Function

    Private Function LetturaEffettivaPermessiStato(ByVal GruppoUtente As Integer,
                                                   ByVal ServizioCod As Integer,
                                                   ByVal StatoCod As Integer
                                                   ) As PermessiStato

        Dim permessiStato As New PermessiStato

        Dim objPermessiStato As New GruppiUtente_PermessiStato_R

        Dim dtPermessiStato = objPermessiStato.Leggi(GruppoUtente,
                                                     ServizioCod,
                                                     StatoCod,
                                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                     "",
                                                     _objParametriUtenti)

        If Not IsNothing(dtPermessiStato) AndAlso dtPermessiStato.Rows.Count > 0 Then

            If (dtPermessiStato.Rows(0).Item("Visualizza")) = 1 Then
                permessiStato.Visualizza = True
            End If

            If (dtPermessiStato.Rows(0).Item("Modifica")) = 1 Then
                permessiStato.Modifica = True
            End If

            If (dtPermessiStato.Rows(0).Item("Cancella")) = 1 Then
                permessiStato.Cancella = True
            End If

        End If

        Return permessiStato

    End Function

    Public Function DeterminaPermessiStatoUtente(ByVal permessiStatoOrigine As PermessiStato,
                                                 ByVal utenteAbilitatoLettura As Boolean,
                                                 ByVal utenteAbilitatoScrittura As Boolean
                                                 ) As PermessiStato

        Dim permessiStato As New PermessiStato

        Select Case True

            Case utenteAbilitatoScrittura

                permessiStato.Visualizza = permessiStatoOrigine.Visualizza
                permessiStato.Modifica = permessiStatoOrigine.Modifica
                permessiStato.Cancella = permessiStatoOrigine.Cancella

            Case utenteAbilitatoLettura

                permessiStato.Visualizza = permessiStatoOrigine.Visualizza
                permessiStato.Modifica = False
                permessiStato.Cancella = False

            Case Else

                permessiStato.Visualizza = False
                permessiStato.Modifica = False
                permessiStato.Cancella = False

        End Select

        Return permessiStato

    End Function

End Class

Public Class PermessiStato

    Public Visualizza As Boolean = False
    Public Modifica As Boolean = False
    Public Cancella As Boolean = False

End Class
