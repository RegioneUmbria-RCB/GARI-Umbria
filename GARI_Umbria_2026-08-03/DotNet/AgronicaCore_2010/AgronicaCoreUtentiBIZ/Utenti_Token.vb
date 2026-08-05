Imports System.Security.Cryptography
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class Utenti_Token_R

    Public Function VerificaEsistenzaToken(ByRef objParametri As AgronicaCoreParametri,
                                           ByRef objParametri_Super_Server As AgronicaCoreParametri
                                           ) As RispostaStandard

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim DT As DataTable
        Dim resp As New RispostaStandard

        Try

            objParametri_Super_Server.SuperUserUsername = objParametri.SuperUserUsername

            DT = xRead.LeggiSuperUser("", "", objParametri_Super_Server)


            If DT Is Nothing OrElse DT.Rows.Count > 1 Then
                resp.RispostaOK = False
                resp.Errore = "Si è verificato un errore nel recupero dei token utente."

                Return resp
            ElseIf DT.Rows.Count > 0 Then
                resp.RispostaOK = True
                resp.RispostaStringa = DT.Rows(0)("Token_ID").ToString

                Return resp
            Else
                resp.RispostaOK = True
                resp.RispostaStringa = ""

                Return resp
            End If
        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = ex.Message

            Return resp
        End Try

    End Function

    Public Function VerificaValiditaToken(ByRef objParametri_Super_Server As AgronicaCoreParametri
                                          ) As Boolean

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_R
        Dim DT As DataTable
        Dim resp As New RispostaStandard

        DT = xRead.Leggi(objParametri_Super_Server)

        If DT Is Nothing OrElse DT.Rows.Count > 1 Then
            Throw New Exception("Errore nella verifica della validità del token")
        End If

        If DT.Rows.Count = 0 Then
            Return False
        End If

        Dim temporaryToken = Convert.ToInt32(DT.Rows(0)("Validita_Minuti"))

        If temporaryToken = -1 Then
            Return Convert.ToDateTime(DT.Rows(0)("Validita_Fine")) > Date.Now
        Else
            Return Convert.ToDateTime(DT.Rows(0)("Validita_Inizio")).AddMinutes(temporaryToken) > Date.Now
        End If

    End Function

    Public Function LeggiDatiUtenteDaToken(ByVal token As String,
                                           ByRef objParametri_Super_Server As AgronicaCoreParametri
                                           ) As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim DT As DataTable
        Dim resp As New RispostaStandard


        DT = xRead.Leggi_Token(token, "", objParametri_Super_Server)


        If DT Is Nothing OrElse DT.Rows.Count > 1 Then

            Throw New Exception("Si è verificato un errore nel recupero dei token utente.")

        End If

        Return DT

    End Function

End Class
Public Class Utenti_Token_W
    Public Function SalvaNuovoToken(dB_Utenti As Integer,
                                    dB_Server As Integer,
                                    objParametri As AgronicaCoreParametri,
                                    objParametri_Utenti As AgronicaCoreParametri,
                                    objParametri_Super_Server As AgronicaCoreParametri
                                    ) As RispostaStandard

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Token_R
        Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Token_W

        Dim resp As New RispostaStandard

        Try

            Dim generatoreToken As New GeneratoreToken
            Dim token As String

            Dim nomeProvider As String = objParametri_Super_Server.StringaConnessione.Split(";")(0).Split("=")(1)
            Dim nomeServer As String = objParametri_Super_Server.StringaConnessione.Split(";")(1).Split("=")(1)
            Dim userId As String = objParametri_Super_Server.StringaConnessione.Split(";")(3).Split("=")(1)
            Dim pivaSuperUser As String = objParametri.PivaSuperUser

            'recupera ID_DB: Server
            If dB_Server = 0 Then
                Dim TIPO_DB_SERVER As Integer = 1
                Dim nomeDB_Server As String = objParametri.StringaConnessione.Split(";")(2).Split("=")(1)

                dB_Server = xRead.Leggi_idDB(TIPO_DB_SERVER, nomeProvider, nomeServer, nomeDB_Server,
                                             userId, pivaSuperUser, objParametri_Super_Server)
                If dB_Server = 0 Then
                    Throw New Exception("Errore nel salvataggio del token: ID DB Server mancante")
                End If
            End If

            'recupera ID_DB: Utenti
            If dB_Utenti = 0 Then
                Dim TIPO_DB_UTENTI As Integer = 2
                Dim nomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

                dB_Utenti = xRead.Leggi_idDB(TIPO_DB_UTENTI, nomeProvider, nomeServer, nomeDB_Utenti,
                                             userId, pivaSuperUser, objParametri_Super_Server)
                If dB_Utenti = 0 Then
                    Throw New Exception("Errore nel salvataggio del token: ID DB Utenti mancante")
                End If
            End If

            Dim parametri As New JObject
            parametri.Add("idDB_Server", dB_Server.ToString)
            parametri.Add("idDB_Utenti", dB_Utenti.ToString)

            token = GeneratoreToken.Genera

            resp.RispostaOK = xWrite.Scrivi(token,
                                            objParametri.PivaSuperUser,
                                            objParametri.SuperUserUsername,
                                            "",
                                            objParametri.SuperUserUsername,
                                            "",
                                            TipiEnumerativi.enum_AWS_ApplicazioneRichiedente.AgronicaWebApiProfilatore_G2G_API,
                                            "",
                                            0,
                                            parametri.ToString,
                                            objParametri_Super_Server
                                            )

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nel salvataggio del token")
            End If

            resp.RispostaOK = SalvaScadenzaToken(-1,
                                                 objParametri,
                                                 objParametri_Utenti,
                                                 objParametri_Super_Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nel salvataggio della validità del token ")
            End If

            resp.RispostaStringa = String.Format("OK^{0}", token)

            Return resp

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = ex.Message

            Return resp
        End Try

    End Function

    Private Function SalvaScadenzaToken(scadenza As Int32,
                                        objParametri As AgronicaCoreParametri,
                                        objParametri_Utenti As AgronicaCoreParametri,
                                        objParametri_Super_Server As AgronicaCoreParametri
                                       ) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreUtentiDAL.Utenti_Validita_Token_W

        resp = xWrite.Scrivi(objParametri.SuperUserUsername,
                             "",
                             scadenza,
                             objParametri_Super_Server)

        Return resp

    End Function

    Public Function SalvaTokenImpostazioniUtenti(token As String,
                                                 objParametri As AgronicaCoreParametri,
                                                 objParametri_Utenti As AgronicaCoreParametri,
                                                 objParametri_Super_Server As AgronicaCoreParametri
                                                 ) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim DT As DataTable

        Dim xRead As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        DT = xRead.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI,
                         2,
                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                         "",
                         "",
                         objParametri_Utenti)

        If DT Is Nothing OrElse DT.Rows.Count <> 1 Then
            resp.RispostaOK = False
            resp.Errore = "Si è verificato un errore nel recupero delle impostazioni utente."

            Return resp
        End If

        'Dim imp As New ImpostazioniAPP

        Return resp
    End Function

    Private Class GeneratoreToken

        Public Shared Function Genera() As String

            Dim g1() As Byte = New Byte(25) {}
            Dim gen As RandomNumberGenerator = RandomNumberGenerator.Create()
            gen.GetBytes(g1)


            Dim s As String =
        Convert.ToBase64String(g1).Replace("+", "A").Replace("/", "B").Replace("=", "C")


            Return s

        End Function

    End Class


End Class
