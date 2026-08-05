Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class AgronicaLogAnagrafe_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal id As Integer,
                          ByVal superUser As String,
                          ByVal utente As String,
                          ByVal tipoOperazione As enum_TipoOperazioneDB,
                          ByVal tipo As String,
                          ByVal chiave As String,
                          ByVal param1 As String,
                          ByVal param2 As String,
                          ByVal param3 As String,
                          ByVal param4 As String,
                          ByVal param5 As String,
                          ByVal param6 As String,
                          ByVal idServizio As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Id_Budget As Integer = -1
                          ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   id = 0
        '   superUser = ""
        '   utente = ""
        '   tipoOperazione = 0
        '   tipo = ""
        '   chiave = ""
        '   param1 = ""
        '   param2 = ""
        '   param3 = ""
        '   param4 = ""
        '   param5 = ""
        '   param6 = 0
        '   idServizio = 0
        '====================================================================================

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Agronica_Log_Anagrafe ")
            stb.AppendLine(" WHERE 1=1 ")

            If id <> 0 Then
                stb.AppendLine(" AND ID = " & Agro_SQL_SaveNum(id) & " ")
            End If

            If superUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(superUser) & "' ")
            End If

            If utente <> "" Then
                stb.AppendLine(" AND Utente = '" & Agro_SQL_SaveText(utente) & "' ")
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND Tipo_Operazione = " & Agro_SQL_SaveNum(tipoOperazione) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If chiave <> "" Then
                stb.AppendLine(" AND Chiave = '" & Agro_SQL_SaveText(chiave) & "' ")
            End If

            If param1 <> "" Then
                stb.AppendLine(" AND Param1 = '" & Agro_SQL_SaveText(param1) & "' ")
            End If

            If param2 <> "" Then
                stb.AppendLine(" AND Param2 = '" & Agro_SQL_SaveText(param2) & "' ")
            End If

            If param3 <> "" Then
                stb.AppendLine(" AND Param3 = '" & Agro_SQL_SaveText(param3) & "' ")
            End If

            If param4 <> "" Then
                stb.AppendLine(" AND Param4 = '" & Agro_SQL_SaveText(param4) & "' ")
            End If

            If param5 <> "" Then
                stb.AppendLine(" AND Param5 = '" & Agro_SQL_SaveText(param5) & "' ")
            End If

            If param6 <> "" Then
                stb.AppendLine(" AND Param6 = '" & Agro_SQL_SaveText(param6) & "' ")
            End If

            If idServizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(idServizio) & " ")
            End If

            If Id_Budget <> -1 Then
                stb.AppendLine(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiLogAnagrafeJoinInvio(ByVal id As Integer,
                                              ByVal superUser As String,
                                              ByVal utente As String,
                                              ByVal tipoEsportazione As enum_Esportazioni_Sistema_Cod,
                                              ByVal tipoOperazione As enum_TipoOperazioneDB,
                                              ByVal Origine As enum_SistemiEsterni,
                                              ByVal tipo As String,
                                              ByVal chiave As String,
                                              ByVal param1 As String,
                                              ByVal param2 As String,
                                              ByVal param3 As String,
                                              ByVal param4 As String,
                                              ByVal param5 As String,
                                              ByVal param6 As String,
                                              ByVal idServizio As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Id_Budget As Integer = -1
                                              ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   id = 0
        '   superUser = ""
        '   utente = ""
        '   tipoOperazione = 0
        '   tipoEsportazione = 0
        '   tipo = ""
        '   chiave = ""
        '   param1 = ""
        '   param2 = ""
        '   param3 = ""
        '   param4 = ""
        '   param5 = ""
        '   param6 = 0
        '   idServizio = 0
        '====================================================================================

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_R.LeggiLogAnagrafeJoinInvio()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" with cte_key_export_type as ( ")
            stb.AppendLine(" SELECT Chiave, Tipo_Esportazione, Max(Id_Log_Invio) idLogInvio, MAX(datainvio) Data_Invio ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Anagrafe ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione) & " ")
            stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            stb.AppendLine(" GROUP BY Chiave, Tipo_Esportazione ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" cte_max_id_export_key_type as ( ")
            stb.AppendLine(" SELECT Chiave, Tipo, MAX(ID) as max_id FROM Agronica_Log_Anagrafe ")
            stb.AppendLine(" WHERE Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            stb.AppendLine(" AND Origine <> " & Agro_SQL_SaveNum(Origine) & " ")
            stb.AppendLine(" group by Chiave,Tipo ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" cte_max_id_esito_export_key_type as ( ")
            stb.AppendLine(" SELECT ID, Esito, Data_Invio FROM Agronica_Log_Invio_Chiamate ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione) & " ")
            stb.AppendLine(" ) ")

            stb.AppendLine(" SELECT logAnagrafe.Chiave, logAnagrafe.Tipo_Operazione, ISNULL(logInvioEsito.Esito, '') As Esito, ISNULL(logInvioEsito.ID, '') As ID_Chiamata ")
            stb.AppendLine(" FROM Agronica_Log_Anagrafe logAnagrafe ")
            stb.AppendLine(" inner join cte_max_id_export_key_type logAnagrafej ")
            stb.AppendLine(" on (logAnagrafe.Chiave = logAnagrafej.Chiave and logAnagrafe.Tipo=logAnagrafej.Tipo and logAnagrafe.ID=logAnagrafej.max_id) ")
            stb.AppendLine(" LEFT OUTER JOIN cte_key_export_type logInvio ")
            stb.AppendLine(" ON (logAnagrafej.Chiave = logInvio.Chiave) ")
            stb.AppendLine(" LEFT OUTER JOIN cte_max_id_esito_export_key_type logInvioEsito ")
            stb.AppendLine(" ON (logInvio.idLogInvio = logInvioEsito.ID) ")

            stb.AppendLine(" WHERE 1 = 1 ")

            stb.AppendLine(" AND (logInvioEsito.Data_Invio IS NULL OR (logAnagrafe.Data_Ora_RegistrazioneLog >= logInvioEsito.Data_Invio AND logInvioEsito.Esito IN ('OK','BLK')) OR (logInvioEsito.Esito = 'KO')) ")

            If id <> 0 Then
                stb.AppendLine(" AND logAnagrafe.ID = " & Agro_SQL_SaveNum(id) & " ")
            End If

            If superUser <> "" Then
                stb.AppendLine(" AND logAnagrafe.SuperUser = '" & Agro_SQL_SaveText(superUser) & "' ")
            End If

            If utente <> "" Then
                stb.AppendLine(" AND logAnagrafe.Utente = '" & Agro_SQL_SaveText(utente) & "' ")
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND logAnagrafe.Tipo_Operazione = " & Agro_SQL_SaveNum(tipoOperazione) & " ")
            End If

            If chiave <> "" Then
                stb.AppendLine(" AND logAnagrafe.Chiave = '" & Agro_SQL_SaveText(chiave) & "' ")
            End If

            If param1 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param1 = '" & Agro_SQL_SaveText(param1) & "' ")
            End If

            If param2 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param2 = '" & Agro_SQL_SaveText(param2) & "' ")
            End If

            If param3 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param3 = '" & Agro_SQL_SaveText(param3) & "' ")
            End If

            If param4 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param4 = '" & Agro_SQL_SaveText(param4) & "' ")
            End If

            If param5 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param5 = '" & Agro_SQL_SaveText(param5) & "' ")
            End If

            If param6 <> "" Then
                stb.AppendLine(" AND logAnagrafe.Param6 = '" & Agro_SQL_SaveText(param6) & "' ")
            End If

            If idServizio <> 0 Then
                stb.AppendLine(" AND logAnagrafe.Id_Servizio = " & Agro_SQL_SaveNum(idServizio) & " ")
            End If

            If Id_Budget <> -1 Then
                stb.AppendLine(" AND logAnagrafe.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiLogAllegatoInvio(ByVal Piva As String,
                                          ByVal SaCod As Integer,
                                          ByVal IdAlertEntita As Integer,
                                          ByVal DocumentiCod As Integer,
                                          ByVal TipoEsportazione As Integer,
                                          ByVal TipologiaAllegato As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_R.LeggiLogAllegatoInvio()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            'creo CTE per prendere l'ultimo LOG
            stb.AppendLine(" WITH AnagrafeUltima AS ( ")
            stb.AppendLine("     SELECT *, ")
            stb.AppendLine("            ROW_NUMBER() OVER ( ")
            stb.AppendLine("                PARTITION BY Piva, Sa_Cod, Mac_Cod ")
            stb.AppendLine("                ORDER BY ID_Log_Invio DESC")
            stb.AppendLine("            ) AS rn")
            stb.AppendLine("     FROM Agronica_Log_Invio_Anagrafe WITH (NOLOCK)")
            stb.AppendLine(" ) ")

            stb.AppendLine(" SELECT LogEsitoAllegato.ID As IdInvioAllegato, Allegati_Documenti.Allegati_Documenti_SuperUser, Allegati_Documenti.Allegati_Documenti_Piva, Allegati_Documenti.Allegati_Documenti_Cod, AnagMac.Codice As serialNumberDispositivo, AnagMac.Mac_Cod, AnagMac.Tipologia_Installazione As codiceModelloDispositivo, AnagMac.Contratto_Installazione As codiceContratto ")
            stb.AppendLine(" FROM Alert_Entita (nolock) ")
            stb.AppendLine("    INNER JOIN Allegati_Documenti (nolock) ON Alert_Entita.PivaSuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            stb.AppendLine("    INNER JOIN Alert_Elenco (nolock) ON Alert_Entita.PivaSuperUser = Alert_Elenco.PivaSuperUser AND Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            stb.AppendLine("    LEFT OUTER JOIN Agronica_Log_Invio_Chiamate LogEsitoAllegato ON (LogEsitoAllegato.Dettaglio1 = CONCAT(Allegati_Documenti.Allegati_Documenti_SuperUser, '_', Allegati_Documenti.Allegati_Documenti_Piva, '_', Allegati_Documenti.Allegati_Documenti_Cod) ")
            If TipoEsportazione <> 0 Then
                stb.AppendLine(" AND LogEsitoAllegato.Tipo_Esportazione = " & Agro_SQL_SaveNum(TipoEsportazione) & " ")
            End If
            stb.AppendLine(" ) ")
            stb.AppendLine("    LEFT OUTER JOIN AnagrafeUltima logAnagrafe ON (Alert_Entita.Piva = logAnagrafe.Piva AND Alert_Entita.Sa_Cod = logAnagrafe.Sa_Cod AND Alert_Entita.Mac_Cod = logAnagrafe.Mac_Cod AND rn = 1) ")
            stb.AppendLine("    LEFT OUTER JOIN Agronica_Log_Invio_Chiamate logInvioEsitoAnag ON ( logAnagrafe.ID_Log_Invio = logInvioEsitoAnag.ID ")
            If TipoEsportazione <> 0 Then
                stb.AppendLine(" AND logInvioEsitoAnag.Tipo_Esportazione = " & Agro_SQL_SaveNum(TipoEsportazione) & " ")
            End If
            stb.AppendLine(" ) ")
            stb.AppendLine("    LEFT OUTER JOIN Parco_Macchine AnagMac ON (AnagMac.Piva = logAnagrafe.Piva AND AnagMac.Sa_Cod = logAnagrafe.Sa_Cod AND AnagMac.Mac_Cod = logAnagrafe.Mac_Cod) ")
            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine("    AND (ISNULL(logInvioEsitoAnag.Esito, '') = 'OK') ")                                     'se l'anagrafica delle Macchine è stata esportata correttamente 
            stb.AppendLine("    AND (LogEsitoAllegato.Data_Invio IS NULL OR ((Allegati_Documenti.Data_Modifica > LogEsitoAllegato.Data_Invio AND ISNULL(LogEsitoAllegato.Esito, '') IN ('OK', 'BLK')) OR ISNULL(LogEsitoAllegato.Esito, '') IN ('', 'KO'))) ") 'solite condizioni di reinvio

            If Piva <> "" Then
                stb.AppendLine(" AND Alert_Entita.Piva = " & Agro_SQL_SaveText(Piva) & " ")
            End If

            If SaCod <> 0 Then
                stb.AppendLine(" AND Alert_Entita.Sa_Cod = " & Agro_SQL_SaveText(SaCod) & " ")
            End If

            If IdAlertEntita <> 0 Then
                stb.AppendLine(" AND Alert_Entita.ID_Alert_Entita = " & Agro_SQL_SaveNum(IdAlertEntita) & " ")
            End If

            If DocumentiCod <> 0 Then
                stb.AppendLine(" AND Allegati_Documenti.Allegati_Documenti_Cod = " & Agro_SQL_SaveNum(DocumentiCod) & " ")
            End If

            If TipologiaAllegato <> 0 Then
                stb.AppendLine(" AND Alert_Elenco.ID_Tipologia = " & Agro_SQL_SaveNum(TipologiaAllegato) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################


Public Class AgronicaLogAnagrafe_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    ''' <summary>
    ''' Scrive Agronica_Log_Anagrafe; la colonna chiave viene creata concatenando tutti i parametri param* divisi da underscore
    ''' <para>Reg_Impianti = Piva_SaCod_Appezza_IdReg, </para>
    ''' <para>Imprese_Progetti = Piva_ProgettoCod, </para>
    ''' <para>Zoo_Animali = Piva_SaCod_CodProgetto</para>
    ''' <para>Zoo_Animali_Distinte = Piva_CodProgetto</para>
    ''' </summary>
    ''' <param name="tipoOperazione"></param>
    ''' <param name="tipo">Nome della tabella cui questo log si riferisce (es: Reg_Impianti)</param>
    ''' <param name="param1">Deve sempre essere diverso da Nothing</param>
    ''' <param name="param2">Se non usato passare Nothing</param>
    ''' <param name="param3">Se non usato passare Nothing</param>
    ''' <param name="param4">Se non usato passare Nothing</param>
    ''' <param name="param5">Se non usato passare Nothing</param>
    ''' <param name="param6">Se non usato passare Nothing</param>
    ''' <param name="idServizio"></param>
    ''' <param name="note">Default = ""</param>
    ''' <param name="objParametri"></param>
    Public Function Scrivi(ByVal tipoOperazione As Integer,
                           ByVal tipo As String,
                           ByVal param1 As String,
                           ByVal param2 As String,
                           ByVal param3 As String,
                           ByVal param4 As String,
                           ByVal param5 As String,
                           ByVal param6 As String,
                           ByVal note As String,
                           ByVal idServizio As enum_Id_Servizio,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal object_data As String = "",
                           Optional ByVal Id_Budget As Integer = 0
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If param1 Is Nothing Then
                Throw New Exception("Almeno il primo parametro deve essere diverso da Nothing")
            End If

            Dim chiave As String = ""

            If tipo = "Imprese_Progetti" Then
                'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
                'ma salverò anche SaCod (Param3), Appezza (Param4), IdReg (Param5)
                'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                chiave = param1 &
                         If(param2 Is Nothing, "", "_" & param2)
            ElseIf tipo = "Zoo_Animali_Distinte" Then
                'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
                'ma salverò anche SaCod (Param3), CodAnimale (Param4)
                'perché possono essere utili (specie in caso di eliminazione) per capire a quale animale apparteneva la distinta
                chiave = param1 &
                         If(param2 Is Nothing, "", "_" & param2)
            Else
                chiave = param1 &
                         If(param2 Is Nothing, "", "_" & param2) &
                         If(param3 Is Nothing, "", "_" & param3) &
                         If(param4 Is Nothing, "", "_" & param4) &
                         If(param5 Is Nothing, "", "_" & param5) &
                         If(param6 Is Nothing, "", "_" & param6)
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Anagrafe ")
            strSql.AppendLine("             ( SuperUser, Utente, Tipo_Operazione, Tipo, ")
            strSql.AppendLine("               Chiave, Param1, Param2, Param3, Param4, Param5, Param6, ")
            strSql.AppendLine("               Note, Data_Ora_RegistrazioneLog, Id_Servizio, object_data, Id_Budget) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("           '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(tipoOperazione) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(tipo) & "'  ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(chiave) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param1) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param2) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param3) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param4) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param5) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param6) & " ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(note) & "'  ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(CInt(idServizio)) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(object_data) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(CInt(Id_Budget)) & "  ")
            strSql.AppendLine("         )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Crea oggetto EF per scrivere Agronica_Log_Anagrafe; la colonna chiave viene creata concatenando tutti i parametri param* divisi da underscore 
    ''' <para>Reg_Impianti = Piva_SaCod_Appezza_IdReg, </para>
    ''' <para>Imprese_Progetti = Piva_ProgettoCod, </para>
    ''' <para>Zoo_Animali = Piva_SaCod_CodProgetto</para>
    ''' <para>Zoo_Animali_Distinte = Piva_CodProgetto</para>
    ''' </summary>
    ''' <param name="tipo">Nome della tabella cui questo log si riferisce (es: Reg_Impianti)</param>
    ''' <param name="param1">Deve sempre essere diverso da Nothing</param>
    ''' <param name="param2">Se non usato passare Nothing</param>
    ''' <param name="param3">Se non usato passare Nothing</param>
    ''' <param name="param4">Se non usato passare Nothing</param>
    ''' <param name="param5">Se non usato passare Nothing</param>
    ''' <param name="param6">Se non usato passare Nothing</param>
    ''' <param name="tipoOperazione"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="idServizio">Default = enum_Id_Servizio.GiasOnline</param>
    ''' <param name="note">Default = ""</param>
    ''' <returns>Oggetto Agronica_Log_Anagrafe</returns>
    Public Function CreaLogAnagrafeEF(ByVal tipo As String,
                                      ByVal param1 As String,
                                      ByVal param2 As String,
                                      ByVal param3 As String,
                                      ByVal param4 As String,
                                      ByVal param5 As String,
                                      ByVal param6 As String,
                                      ByVal tipoOperazione As enum_TipoOperazioneDB,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline,
                                      Optional ByVal note As String = "",
                                      Optional ByVal objectData As String = "",
                                      Optional ByVal Id_Budget As Integer = 0,
                                      Optional ByVal Origine As Integer = -1
                                      ) As AgronicaCoreEntityFramework_POCO.Agronica_Log_Anagrafe

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W.CreaLogAnagrafeEF"

        Dim log As AgronicaCoreEntityFramework_POCO.Agronica_Log_Anagrafe = Nothing

        Try

            If param1 Is Nothing Then
                Throw New Exception("Almeno il primo parametro deve essere diverso da Nothing")
            End If

            Dim chiave As String = ""

            If tipo = "Imprese_Progetti" Then
                'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
                'ma salverò anche SaCod (Param3), Appezza (Param4), IdReg (Param5)
                'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                chiave = param1 &
                         If(String.IsNullOrEmpty(param2), "", "_" & param2)
            ElseIf tipo = "Zoo_Animali_Distinte" Then
                'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
                'ma salverò anche SaCod (Param3), CodAnimale (Param4)
                'perché possono essere utili (specie in caso di eliminazione) per capire a quale animale apparteneva la distinta
                chiave = param1 &
                         If(String.IsNullOrEmpty(param2), "", "_" & param2)
            Else
                chiave = param1 &
                         If(String.IsNullOrEmpty(param2), "", "_" & param2) &
                         If(String.IsNullOrEmpty(param3), "", "_" & param3) &
                         If(String.IsNullOrEmpty(param4), "", "_" & param4) &
                         If(String.IsNullOrEmpty(param5), "", "_" & param5) &
                         If(String.IsNullOrEmpty(param6), "", "_" & param6)
            End If


            log = New AgronicaCoreEntityFramework_POCO.Agronica_Log_Anagrafe With {
                .Tipo = tipo,
                .Chiave = chiave,
                .Param1 = param1,
                .Param2 = param2,
                .Param3 = param3,
                .Param4 = param4,
                .Param5 = param5,
                .Param6 = param6,
                .Tipo_Operazione = tipoOperazione,
                .Data_Ora_RegistrazioneLog = DateTime.Now,
                .SuperUser = objParametri.PivaSuperUser,
                .Utente = objParametri.UsernameOperazione,
                .Id_Servizio = idServizio,
                .Note = note,
                .object_data = objectData,
                .Id_Budget = Id_Budget,
                .Origine = Origine
            }

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return log

    End Function


#Region "Massivi per Modifica Multipla"
    Public Function ScriviLog_Massivo_ModificaMultipla(listaChiaviAppezzamento As List(Of (String, Integer, Integer)),
                                                       listaChiaviImpianto As List(Of (String, Integer, Integer, Integer)),
                                                       listaChiaviEsercizio As List(Of (String, Integer, Integer, Integer, Integer)),
                                                       tipoEntita As Enum_EntitaModificaMultiplaPianoColturale,
                                                       timestamp As DateTime,
                                                       note As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W.ScriviLog_Massivo_ModificaMultipla()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Dim flagConnessione, flagTransazione As Boolean

        Try
            Select Case tipoEntita
                Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                    If listaChiaviAppezzamento Is Nothing OrElse listaChiaviAppezzamento.Count = 0 Then
                        Throw New Exception("listaChiaviAppezzamento deve essere diverso da Nothing")
                    End If
                Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                    If listaChiaviImpianto Is Nothing OrElse listaChiaviImpianto.Count = 0 Then
                        Throw New Exception("listaChiaviImpianto deve essere diverso da Nothing")
                    End If
                Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                    If listaChiaviEsercizio Is Nothing OrElse listaChiaviEsercizio.Count = 0 Then
                        Throw New Exception("listaChiaviEsercizio deve essere diverso da Nothing")
                    End If
            End Select

            'If tipo = "Imprese_Progetti" Then
            '    'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
            '    'ma salverò anche SaCod (Param3), Appezza (Param4), IdReg (Param5)
            '    'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
            '    chiave = "tmp.Piva + tmp."
            'ElseIf tipo = "Zoo_Animali_Distinte" Then
            '    'in questo caso la chiave è solo Piva_ProgettoCod (Param1_Param2)
            '    'ma salverò anche SaCod (Param3), CodAnimale (Param4)
            '    'perché possono essere utili (specie in caso di eliminazione) per capire a quale animale apparteneva la distinta
            'Else
            '    chiave = param1 &
            '             If(param2 Is Nothing, "", "_" & param2) &
            '             If(param3 Is Nothing, "", "_" & param3) &
            '             If(param4 Is Nothing, "", "_" & param4) &
            '             If(param5 Is Nothing, "", "_" & param5) &
            '             If(param6 Is Nothing, "", "_" & param6)
            'End If

            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            Select Case tipoEntita
                Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                    TempChiaviMassivo.CreaTabellaTemp_FiltroAppezzamenti(listaChiaviAppezzamento, nomeRoutine, objParametri)
                Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                    TempChiaviMassivo.CreaTabellaTemp_FiltroImpianti(listaChiaviImpianto, nomeRoutine, objParametri)
                Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                    TempChiaviMassivo.CreaTabellaTemp_FiltroEsercizi(listaChiaviEsercizio, nomeRoutine, objParametri)
            End Select

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Anagrafe ( ")
            strSql.AppendLine("             SuperUser ")
            strSql.AppendLine("           , Utente ")
            strSql.AppendLine("           , Tipo_Operazione ")
            strSql.AppendLine("           , Tipo ")
            strSql.AppendLine("           , Chiave ")
            strSql.AppendLine("           , Param1 ")
            strSql.AppendLine("           , Param2 ")
            strSql.AppendLine("           , Param3 ")
            strSql.AppendLine("           , Param4 ")
            strSql.AppendLine("           , Param5 ")
            strSql.AppendLine("           , Param6 ")
            strSql.AppendLine("           , Note ")
            strSql.AppendLine("           , Data_Ora_RegistrazioneLog ")
            strSql.AppendLine("           , Id_Servizio")
            strSql.AppendLine("           , object_data ")
            strSql.AppendLine("           , Id_Budget ")
            strSql.AppendLine(" ) ")

            strSql.AppendLine(" SELECT ")
            strSql.AppendLine($"      '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}'  ")
            strSql.AppendLine($"    , '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}'  ")
            strSql.AppendLine($"    , {Agro_SQL_SaveNum(CInt(enum_TipoOperazioneDB.Modifica))} ")

            'Valorizzo il tipo, la chiave e i vari parametri
            Select Case tipoEntita
                Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                    strSql.AppendLine($"    , '{Agro_SQL_SaveText(enum_TipoEntita_Des.Appezza)}' ")
                    strSql.AppendLine($"    , tmp.Piva + '_' + CAST(tmp.Sa_Cod AS VARCHAR(100)) + '_' + CAST(tmp.Appezza AS VARCHAR(100)) ")
                    strSql.AppendLine($"    , tmp.Piva  ")
                    strSql.AppendLine($"    , tmp.Sa_Cod ")
                    strSql.AppendLine($"    , tmp.Appezza ")
                    strSql.AppendLine($"    , NULL ")
                    strSql.AppendLine($"    , NULL ")
                    strSql.AppendLine($"    , NULL ")
                Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                    strSql.AppendLine($"    , '{Agro_SQL_SaveText(enum_TipoEntita_Des.Impianti)}' ")
                    strSql.AppendLine($"    , tmp.Piva + '_' + CAST(tmp.Sa_Cod AS VARCHAR(100)) + '_' + CAST(tmp.Appezza AS VARCHAR(100)) + '_' + CAST(tmp.Id_Reg AS VARCHAR(100)) ")
                    strSql.AppendLine($"    , tmp.Piva  ")
                    strSql.AppendLine($"    , tmp.Sa_Cod ")
                    strSql.AppendLine($"    , tmp.Appezza ")
                    strSql.AppendLine($"    , tmp.Id_Reg ")
                    strSql.AppendLine($"    , NULL ")
                    strSql.AppendLine($"    , NULL ")
                Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                    strSql.AppendLine($"    , '{Agro_SQL_SaveText(enum_TipoEntita_Des.Progetti)}' ")
                    strSql.AppendLine($"    , tmp.Piva + '_' + CAST(tmp.Progetto_Cod AS VARCHAR(100)) ")
                    strSql.AppendLine($"    , tmp.Piva  ")
                    strSql.AppendLine($"    , tmp.Progetto_Cod  ")
                    strSql.AppendLine($"    , tmp.Sa_Cod ")
                    strSql.AppendLine($"    , tmp.Appezza ")
                    strSql.AppendLine($"    , tmp.Id_Reg ")
                    strSql.AppendLine($"    , NULL ")
            End Select
            strSql.AppendLine($"    , '{Agro_SQL_SaveText(note)}' ")
            strSql.AppendLine($"    , {Agro_SQL_SaveDateTime(timestamp)} ")
            strSql.AppendLine($"    , {Agro_SQL_SaveNum(CInt(enum_Id_Servizio.GiasOnline))} ")
            strSql.AppendLine("    , '' ")
            strSql.AppendLine("    , 0 ")

            strSql.Append(" FROM ")

            Select Case tipoEntita
                Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                    strSql.Append(" #TempAppezzamento tmp ")
                Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                    strSql.Append(" #TempImpianto tmp ")
                Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                    strSql.Append(" #TempEsercizio tmp ")
            End Select
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Select Case tipoEntita
                Case Enum_EntitaModificaMultiplaPianoColturale.Appezzamenti
                    TempChiaviMassivo.EliminaTabellaTemp_FiltroAppezzamenti(nomeRoutine, objParametri)
                Case Enum_EntitaModificaMultiplaPianoColturale.Impianti
                    TempChiaviMassivo.EliminaTabellaTemp_FiltroImpianti(nomeRoutine, objParametri)
                Case Enum_EntitaModificaMultiplaPianoColturale.Esercizi
                    TempChiaviMassivo.EliminaTabellaTemp_FiltroEsercizi(nomeRoutine, objParametri)
            End Select

            'commit transazione
            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False

            'rollback transazione
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

#End Region

End Class
