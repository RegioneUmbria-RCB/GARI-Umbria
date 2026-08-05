Imports AgronicaCoreDataProvider

<CachedDataProviderAttribute("Utenti_CodiciGiasPRO_R")>
Public Class Utenti_CodiciGiasPRO_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    '###############################################################
    <Cacheable(True)>
    Public Function ProgressivoGias_from_Superuser(ByRef objParametri_Utenti As AgronicaCoreParametri) As Integer

        Dim dt As DataTable
        Dim codiceGias As Integer = 0

        If objParametri_Utenti.SuperUserUsername = "" Then
            Throw New Exception("ProgressivoGias_from_Superuser: objParametri_Utenti.SuperUserUsername non valorizzata, impossibile trovare il progressivo")
        End If

        dt = Leggi("", "", objParametri_Utenti)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            codiceGias = dt.Rows(0).Item("ProgressivoGias")
        Else
            Throw New Exception("DT.Rows.Count = 0")
        End If

        If codiceGias = 0 Then
            Throw New Exception("ProgressivoGias_from_Superuser: Codice_Gias = 0")
        End If

        Return codiceGias

    End Function

    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal RestituisciTutto As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_CodiciGiasPro ")
            StrSQL.Append(" WHERE   1=1  ")
            If RestituisciTutto Then
                If objParametri.UtenteUsername <> "" Then
                    StrSQL.Append(" and Username =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                End If
            Else
                StrSQL.Append(" and Username =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '-------------------------------------------------------------------------- 
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi_Superuser(Username As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R.Leggi_Superuser()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti, Utenti_Dettagli, Utenti_CodiciGiasPro ")
            StrSQL.Append(" WHERE   Utenti.Username = Utenti_Dettagli.Username  ")
            StrSQL.Append(" AND     Utenti.Username = Utenti_CodiciGiasPro.Username  ")
            StrSQL.Append(" AND     Utenti.Username =  '" & Agro_SQL_SaveText(Username) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_LicenzeWS(ByVal urlWS As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal RestituisciTutto As Boolean = False
                                    ) As String

        Dim yWS As New ws_ppt.WS_Ppt
        Dim elenco As String = ""

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 4000

        elenco = yWS.LeggiLicenze(objParametri.SuperUserUsername, "Utenti_CodiciGiasPro.Username")

        Return elenco

    End Function


    Public Function Leggi_UtentiWS(ByVal urlWS As String,
                                   ByVal username As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As String

        Dim yWS As New ws_ppt.WS_Ppt
        Dim elenco As String = ""

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 4000

        elenco = yWS.LeggiUtenti(objParametri.SuperUserUsername, username)

        Return elenco

    End Function


    Public Function Leggi_SuperuserWS(ByVal urlWS As String,
                                      ByVal username As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As String

        Dim yWS As New ws_ppt.WS_Ppt
        Dim elenco As String = ""

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 4000

        elenco = yWS.Leggi_Superuser(objParametri.SuperUserUsername, username)

        Return elenco

    End Function


    '##############################################################################################
    Public Function Leggi_Licenze(ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  Optional ByVal RestituisciTutto As Boolean = False
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R.Leggi_Licenze()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            'StrSQL.Append(" SELECT Distinct Utenti_CodiciGiasPro.*, PIVA, Provincia, Rag_Soc  ")
            'StrSQL.Append(" FROM    Utenti_CodiciGiasPro, Utenti_Dettagli ")
            'StrSQL.Append(" WHERE   Utenti_CodiciGiasPro.UserName = Utenti_Dettagli.Username ")

            StrSQL.Append(" SELECT Distinct Utenti_CodiciGiasPro.*, PIVA, Provincia, Rag_Soc, GiasAppKey, Versione  ")
            StrSQL.Append(" FROM    (Utenti_CodiciGiasPro inner join Utenti_Dettagli ")
            StrSQL.Append(" on Utenti_CodiciGiasPro.UserName = Utenti_Dettagli.Username) ")
            StrSQL.Append(" left join CoreWsEndPointConfig ")
            StrSQL.Append(" on Utenti_Dettagli.PIVA = CoreWsEndPointConfig.Piva_SuperUser and CoreWsEndPointConfig.Versione='Release' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '-------------------------------------------------------------------------- 
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :   " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function LeggiChiave(ByVal Usename As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R.LeggiChiave()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT GiasOnline_Key, CD_Key ")
            StrSQL.Append(" FROM   Utenti_CodiciGiasPro ")
            StrSQL.Append(" WHERE  Username =  '" & Agro_SQL_SaveText(Usename) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Sub Calcola_BaseCode_TopCode(ByRef BaseCode As Integer,
                                        ByRef TopCode As Integer,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim dt = Leggi("", "", objParametri)
        If dt.Rows.Count = 0 Then
            Throw New Exception("Utenti_CodiciGiasPro Non trovata per username:" & objParametri.UtenteUsername)
        End If

        Dim IndiceProgressivoGIAS = dt.Rows(0)("ProgressivoGias")
        BaseCode = IndiceProgressivoGIAS * (2 ^ CostantiPersonalizzate.AgroCode_BitPerCodice)
        TopCode = BaseCode + (2 ^ CostantiPersonalizzate.AgroCode_BitPerCodice) - 1

    End Sub

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Utenti_CodiciGiasPro_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal UserName As String,
                           ByVal Progressivo As Integer,
                           ByVal CD_Key As String,
                           ByVal Data_Attivazione As Date,
                           ByVal Durata As Integer,
                           ByVal Aziende As Integer,
                           ByVal Classe As Integer,
                           ByVal Moduli As Integer,
                           ByVal UsernameCommerciale As String,
                           ByVal Flag As Integer,
                           ByVal Data_Creazione As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal GiasOnline_Key As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_CodiciGiasPro_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Utenti_CodiciGiasPro ")
            StrSQL.Append("       (UserName,       ProgressivoGias,   CD_Key, GiasOnline_Key, Data_Attivazione, Durata,")
            StrSQL.Append("        Aziende,        Classe,Moduli, UsernameCommerciale,      Flag, ")
            StrSQL.Append("        Data_Creazione, Data_Modifica")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(LCase(UserName)) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Progressivo) & " ")
            StrSQL.Append("          ,'" & Agro_SQL_SaveText(CD_Key) & "' ")
            StrSQL.Append("          ,'" & Agro_SQL_SaveText(GiasOnline_Key) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Attivazione) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Durata) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Aziende) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Classe) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Moduli) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UsernameCommerciale) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Flag) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal UserName As String,
                             ByVal CD_Key As String,
                             ByVal Data_Attivazione As Date,
                             ByVal Durata As Integer,
                             ByVal Aziende As Integer,
                             ByVal Classe As Integer,
                             ByVal Moduli As Integer,
                             ByVal UsernameCommerciale As String,
                             ByVal Flag As Integer,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal GiasOnline_Key As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_CodiciGiasPro_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_CodiciGiasPro ")
            StrSQL.Append(" SET ")
            StrSQL.Append("   CD_Key           = '" & Agro_SQL_SaveText(CD_Key) & "' ")
            StrSQL.Append(" , GiasOnline_Key   = '" & Agro_SQL_SaveText(GiasOnline_Key) & "' ")
            StrSQL.Append(" , Data_Attivazione = " & Agro_SQL_SaveDate(Data_Attivazione) & "  ")
            StrSQL.Append(" , Durata           = " & Agro_SQL_SaveNum(Durata) & " ")
            StrSQL.Append(" , Aziende          = " & Agro_SQL_SaveNum(Aziende) & " ")
            StrSQL.Append(" , Classe           = " & Agro_SQL_SaveNum(Classe) & " ")
            StrSQL.Append(" , Moduli           = " & Agro_SQL_SaveNum(Moduli) & " ")
            StrSQL.Append(" , UsernameCommerciale = '" & Agro_SQL_SaveText(UsernameCommerciale) & "' ")
            StrSQL.Append(" , Flag             = " & Agro_SQL_SaveNum(Flag) & " ")
            StrSQL.Append(" , Data_Modifica    = " & Agro_SQL_SaveDate(CDate(Now)) & "  ")

            StrSQL.Append("  WHERE Username =  '" & Agro_SQL_SaveText(LCase(UserName)) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Aggiorna_LicenzeWS(ByVal urlWS As String,
                                       ByVal righeInseriteGrid_Licenze As String,
                                       ByVal righeModificateGrid_Licenze As String,
                                       ByVal righeCancellateGrid_Licenze As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Integer

        Dim yWS As New ws_ppt.WS_Ppt

        Dim dummy As Integer

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 4000

        dummy = yWS.AggiornaLicenze(objParametri.SuperUserUsername,
                                    righeInseriteGrid_Licenze,
                                    righeModificateGrid_Licenze,
                                    righeCancellateGrid_Licenze)

        Return dummy

    End Function


    Public Function Aggiorna_LicenzaWS(ByVal Operazione As Integer,
                                       ByVal urlWS As String,
                                       ByVal Username As String,
                                       ByVal Progressivo_Gias As Integer,
                                       ByVal GiasLan_Key As String,
                                       ByVal GiasOnLine_Key As String,
                                       ByVal Durata As Integer,
                                       ByVal Versione As Integer,
                                       ByVal Aziende As Integer,
                                       ByVal Sup As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        Dim yWS As New ws_ppt.WS_Ppt

        Dim bOk As Boolean = False

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 4000

        bOk = yWS.AggiornaLicenza(Operazione,
                                  objParametri.SuperUserUsername,
                                  Username,
                                  Progressivo_Gias,
                                  GiasLan_Key,
                                  GiasOnLine_Key,
                                  Durata,
                                  Versione,
                                  Aziende,
                                  Sup)

        Return bOk

    End Function

    Public Function Aggiorna_UtentiWS(ByVal Operazione As Integer,
                                      ByVal urlWS As String,
                                      ByVal username As String,
                                      ByVal password As String,
                                      ByVal tipo_utente As Integer,
                                      ByVal piva As String,
                                      ByVal ragione_sociale As String,
                                      ByVal cognome As String,
                                      ByVal nome As String,
                                      ByVal cod_fisc As String,
                                      ByVal via As String,
                                      ByVal numero As String,
                                      ByVal cap As String,
                                      ByVal citta As String,
                                      ByVal email As String,
                                      ByVal telefono As String,
                                      ByVal fax As String,
                                      ByVal provincia As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Integer

        Dim yWS As New ws_ppt.WS_Ppt
        Dim progressivoGias As Integer = 0

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 8000

        progressivoGias = yWS.AggiornaSuperUser(Operazione,
                                                objParametri.SuperUserUsername,
                                                username,
                                                password,
                                                tipo_utente,
                                                piva,
                                                ragione_sociale,
                                                cognome,
                                                nome,
                                                cod_fisc,
                                                via,
                                                numero,
                                                cap,
                                                citta,
                                                email,
                                                telefono,
                                                fax,
                                                provincia)

        Return progressivoGias

    End Function

    Public Function Aggiorna_CodiceClienteSincro(
                                      ByVal urlWS As String,
                                      ByVal username As String,
                                      ByVal ragione_sociale As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Dim yWS As New ws_ppt.WS_Ppt
        Dim bOk As Boolean = False

        yWS.Url = urlWS
        'lavez - 16/03/2022 - disattivato perché in cloud va in timeout n volte se il pool è spento
        'yWS.Timeout = 8000

        bOk = yWS.AggiornaCodiceSincroCliente(objParametri.SuperUserUsername,
                                                username,
                                                ragione_sociale)

        Return bOk

    End Function

    Public Sub AggiornaChiave(ByVal GiasOnline_Key As String,
                              ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_CodiciGiasPro_W.AggiornaChiave()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Update Utenti_CodiciGiasPro Set GiasOnline_Key = '" & Agro_SQL_SaveText(GiasOnline_Key) & "'")
            StrSQL.Append(" Where UserName = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function Aggiorna_Licenze(ByVal ProgressivoGIAS As Integer,
                                     ByVal Username As String,
                                     ByVal GiasOnline_Key As String,
                                     ByVal CD_Key As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_CodiciGiasPro_W.Aggiorna_Licenze()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dummy As Integer

        Try

            StrSQL.Length = 0

            StrSQL.Append("  Update Utenti_CodiciGiasPro Set ")
            StrSQL.Append("  GiasOnline_Key = '" & Agro_SQL_SaveText(GiasOnline_Key) & "'")
            StrSQL.Append(" ,CD_Key = '" & Agro_SQL_SaveText(CD_Key) & "'")
            StrSQL.Append(" ,Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append(" Where UserName = '" & Agro_SQL_SaveText(Username) & "' ")
            StrSQL.Append(" And ProgressivoGias = " & Agro_SQL_SaveNum(ProgressivoGIAS) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            dummy = 0

        Catch ex As Exception
            messaggioErrore = ex.Message
            dummy = -1
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dummy

    End Function

End Class
