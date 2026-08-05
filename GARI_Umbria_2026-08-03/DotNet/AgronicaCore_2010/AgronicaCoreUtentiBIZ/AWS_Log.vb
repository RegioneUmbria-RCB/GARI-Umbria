
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility


Public Class AWS_Log_R

    Public Function UltimoAccesso(ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim rval As String = ""

        Try

            Dim xLeggi As New AgronicaCoreUtentiDAL.AWS_log_R
            Dim dtLeggi As DataTable = xLeggi.Leggi(" utente_username = '" & UtilityProvider.Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ", " data_Modifica desc ", objParametri_Utenti)

            Dim dDataUltimoAccesso As DateTime = CostantiPersonalizzate.AGRODATAINIZIO
            If dtLeggi.Rows.Count > 0 Then
                dDataUltimoAccesso = dtLeggi(0)("data_richiesta")
            End If

            If dDataUltimoAccesso <> CostantiPersonalizzate.AGRODATAINIZIO Then

                rval = dDataUltimoAccesso.ToString()

            End If

        Catch ex As Exception

            'in ogni caso bypasso gli errori.

        End Try


        Return rval
    End Function

    ''' <summary>
    ''' Carica l'ultimo accesso e il numero di accessi effettuati relativi a ogni utente.
    ''' </summary>
    ''' <param name="objParametri_Utenti"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns>Una lista contenente oggetti di tipo { UserName, Cognome, Nome, UltimoAccesso, NumeroAccessi }.</returns>
    Public Function UltimoAccessoTutti(
        ByRef objParametri_Utenti As AgronicaCoreParametri
    ) As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.Utente_DettagliAWS)
        Dim objAWS As New AgronicaCoreUtentiDAL.AWS_log_R
        Return objAWS.LeggiTutti(xFiltroAggiuntivo:="", xOrderBy:="", objParametri_Utenti).
            AsEnumerable.
            Select(Function(row) New AgronicaCoreModelsSTD.profilazione.Utente_DettagliAWS With {
                .UserName = row.Item("Utente_Username"),
                .Cognome = row.Item("Cognome"),
                .Nome = row.Item("Nome"),
                .UltimoAccesso = row.Item("Data_UltimoAccesso"),
                .NumeroAccessi = row.Item("Numero_Accessi")
            })
    End Function

    Public Function IsConteggioAccessiAbilitato(objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim xconf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtConf As DataTable = xconf.Leggi(0, "LoginLogAccessoTutti", "", "", objParametri_Server)
        Return (dtConf.Rows.Count > 0 AndAlso CStr(dtConf(0)("valore")).ToLower = "true")
    End Function

End Class


Public Class AWS_Log_W


    Public Sub AccessoLog(
        ByVal sitoOrigine As Enum_SiteRedirector,
        ByVal sitoDestinazione As Enum_SiteRedirector,
        ByVal paginaOrigine As Integer,
        ByVal paginaDestinazione As Integer,
        ByVal ImpostaData As Boolean,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri
    )
        Try

            Dim xconf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtConf As DataTable = xconf.Leggi(0, "LoginLogAccessoTutti", "", "", objParametri_Server)

            Dim loggaTutto As Boolean = False
            Dim richiediUpdate As Boolean = False

            If dtConf.Rows.Count > 0 AndAlso CStr(dtConf(0)("valore")).ToLower = "true" Then
                loggaTutto = True
            End If

            'log dell'accesso.
            Dim objUtenti As New AgronicaCoreUtentiDAL.AWS_log_W

            Dim data As DateTime = Now()
            Dim IP_Richiedente As String = GetIP_Richiedente()

            If Not loggaTutto Then
                Dim xLeggi As New AgronicaCoreUtentiDAL.AWS_log_R
                Dim dtLeggi As DataTable = xLeggi.Leggi(" utente_username = '" & UtilityProvider.Agro_SQL_SaveText(objParametri_Server.UtenteUsername) & "' ", " data_modifica desc ", objParametri_Utenti)
                If dtLeggi.Rows.Count > 0 Then
                    richiediUpdate = True
                    data = dtLeggi(0)("data_modifica")
                End If
            End If

            If Not richiediUpdate Then

                objUtenti.Scrivi(
                    PivaSuperUser:=objParametri_Server.PivaSuperUser,
                    SuperUser_Username:=objParametri_Server.SuperUserUsername,
                    SuperUser_Password:="",
                    Utente_Username:=objParametri_Server.UtenteUsername,
                    Utente_Password:="",
                    IP_Richiedente:=IP_Richiedente,
                    Applicazione_Richiedente:=enum_AWS_ApplicazioneRichiedente.AgronicaAgenda_2010,
                    Funzione_Richiesta:=sitoDestinazione,
                    Url_Richiesto:="",
                    Risposta_Richiesta:=paginaDestinazione,
                    Risposta_Errore:="",
                    Data_Richiesta:=data,
                    Veg_Cod:=0,
                    Dpi_Cod:=0,
                    Flag_DPI_Privato_Pubblico:=0,
                    ID_RCDPI:=0,
                    Grfi_Cod:=0,
                    Flag_Protetto:=0,
                    Tipo_Testata:=0,
                    Tipo_Richiesto:=0,
                    Testo_Ricerca:="",
                    StrPa:="",
                    Av_Cod:=0,
                    Av_Gru:=0,
                    StrSort:="",
                    objParametri:=objParametri_Utenti
            )

            Else

                If Not ImpostaData Then
                    data = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
                End If

                objUtenti.Aggiorna(
                        PivaSuperUser:=objParametri_Server.PivaSuperUser,
                        SuperUser_Username:=objParametri_Server.SuperUserUsername,
                        SuperUser_Password:="",
                        Utente_Username:=objParametri_Server.UtenteUsername,
                        Utente_Password:="",
                        IP_Richiedente:=IP_Richiedente,
                        Applicazione_Richiedente:=enum_AWS_ApplicazioneRichiedente.AgronicaAgenda_2010,
                        Funzione_Richiesta:=sitoDestinazione,
                        Url_Richiesto:="",
                        Risposta_Richiesta:=paginaDestinazione,
                        Risposta_Errore:="",
                        Data_Richiesta:=data,
                        Veg_Cod:=0,
                        Dpi_Cod:=0,
                        Flag_DPI_Privato_Pubblico:=0,
                        ID_RCDPI:=0,
                        Grfi_Cod:=0,
                        Flag_Protetto:=0,
                        Tipo_Testata:=0,
                        Tipo_Richiesto:=0,
                        Testo_Ricerca:="",
                        StrPa:="",
                        Av_Cod:=0,
                        Av_Gru:=0,
                        StrSort:="",
                        objParametri:=objParametri_Utenti
                    )
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Function GetIP_Richiedente() As String

        Dim webH As New Http
        Return webH.IndirizzoIpChiamante

    End Function

End Class
