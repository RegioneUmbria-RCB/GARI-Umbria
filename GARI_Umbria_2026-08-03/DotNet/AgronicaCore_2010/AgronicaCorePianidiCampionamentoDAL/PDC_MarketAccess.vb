Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_MarketAccess_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <param name="Esito">Filtro default = -99</param>
    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_PDC_Dettagli As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal CapitolatoCliente_Cod As Integer,
                          ByVal Esito As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM PDC_MarketAccess ")
            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If CapitolatoCliente_Cod <> 0 Then
                StrSQL.AppendLine(" AND CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            End If

            If Esito <> -99 Then
                StrSQL.AppendLine(" AND Esito = " & Agro_SQL_SaveNum(Esito))
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    ''' <summary>
    ''' Lettura per invio market access a F2B
    ''' </summary>
    ''' <param name="ID_PDC_Testata"></param>
    ''' <param name="ID_PDC_Dettagli"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiF2B(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_R.LeggiF2B()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("  SELECT ")
            stb.AppendLine("  ")
            stb.AppendLine("   ma.Codice_campione AS samplenr ")
            stb.AppendLine(" , ISNULL(icFLEX.val_cod,'') AS facility ")
            stb.AppendLine(" , ISNULL(d.Grower_Number, '') AS kpin ")
            stb.AppendLine(" , ISNULL(d.[block], '') AS [block] ")
            stb.AppendLine(" , 'HW' AS variety ")
            stb.AppendLine(" , ISNULL(d.Rag_Soc,'') AS grower ")
            stb.AppendLine(" , ISNULL(d.Ind_Des, '') + ', ' + ISNULL(d.Com_Des,'') + ISNULL(d.Frz_Des, '') + ")
            stb.AppendLine("      CASE WHEN d.Stato IN ('IT','ITALIA','ITALY') THEN ' (' + ISNULL(d.Pro_Cod, '') + ')' ELSE '' END + ")
            stb.AppendLine("      ', ' + ISNULL(d.Stato, '') AS blockdescription ")
            stb.AppendLine("  , CASE WHEN d.Stato = 'EL' THEN 'GR' ")
            stb.AppendLine("         WHEN d.Stato IN ('IT','ITALIA','ITALY') THEN 'IT' ")
            stb.AppendLine("         ELSE UPPER(ISNULL(d.Stato, 'IT')) END AS countryoforigin")
            stb.AppendLine(" , CASE WHEN d.Regolamento_Cod = 4 THEN 'OB' ELSE 'CK' END AS growingmethod ")
            stb.AppendLine(" , ISNULL(ggn.val_cod, '') AS ggn ")
            stb.AppendLine(" , ROUND(ISNULL(ma.Resa, 0), 2) AS ton ")
            stb.AppendLine(" , CASE WHEN ma.Esito = 1 THEN 'PASS' ELSE 'BLOCK' END AS Esito ")
            stb.AppendLine(" , ma.Decision AS decision ")
            stb.AppendLine(" , cca.Sigla_Capitolato_Privato AS Market ")
            stb.AppendLine("  ")
            stb.AppendLine(" FROM PDC_MarketAccess ma ")
            stb.AppendLine("  INNER JOIN PDC_Testata PDC_T ON ma.PivaSuperUser = PDC_T.PivaSuperUser AND ma.ID_PDC_Testata = PDC_T.ID_PDC_Testata ")
            stb.AppendLine("  INNER JOIN PDC_CapitolatiCliente_Attivi cca ON ma.PivaSuperUser = cca.PivaSuperUser AND ma.ID_PDC_Testata = cca.ID_PDC_Testata AND ma.CapitolatoCliente_Cod = cca.ID_CapitolatoPrivato ")
            'stb.AppendLine("  INNER JOIN ( ")
            'stb.AppendLine("      SELECT DISTINCT ID_CapitolatoPrivato, Des_Capitolato_Privato, Sigla_Capitolato_Privato ")
            'stb.AppendLine("         FROM PDC_CapitolatiCliente_Attivi  ")
            'stb.AppendLine("      ) cca ")
            'stb.AppendLine("      ON ma.CapitolatoCliente_Cod = cca.ID_CapitolatoPrivato ")
            stb.AppendLine("  ")
            stb.AppendLine("  INNER JOIN PDC_Dettagli d ")
            stb.AppendLine("         ON d.PivaSuperUser = ma.PivaSuperUser AND d.ID_PDC_Testata = ma.ID_PDC_Testata AND d.ID_PDC_Dettagli = ma.ID_PDC_Dettagli ")
            stb.AppendLine("  ")
            stb.AppendLine("  LEFT JOIN Imprese_Codici AS ggn ON ggn.piva = d.piva AND ggn.id_cod = 1261 ")
            stb.AppendLine("  LEFT JOIN Imprese_Codici AS icFLEX ON icFLEX.piva = PDC_T.PivaOwner AND icFLEX.id_cod = 1329 ")
            stb.AppendLine("  ")
            
            stb.AppendLine(" WHERE ma.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'la sigla identifica il market, occorre quindi escludere quelli vuoti
            stb.AppendLine(" AND cca.Sigla_Capitolato_Privato <> '' ")

            If ID_PDC_Testata <> 0 Then
                stb.AppendLine(" AND ma.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                stb.AppendLine(" AND ma.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" ORDER BY cca.Sigla_Capitolato_Privato")
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

    ''' <param name="Blocco_Flag">Filtro default = -99</param>
    Public Function LeggiDistinctBloccati(ByVal ID_PDC_Testata As Integer,
                                          ByVal ID_PDC_Dettagli As Integer,
                                          ByVal Blocco_Flag As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_R.LeggiDistinctBloccati()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT PivaSuperUser, ID_PDC_Testata, ID_PDC_Dettagli, Blocco_Flag, Blocco_Data, Blocco_Username ")
            StrSQL.AppendLine(" FROM PDC_MarketAccess ")
            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Blocco_Flag <> -99 Then
                StrSQL.AppendLine(" AND Blocco_Flag = " & Agro_SQL_SaveNum(Blocco_Flag))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class PDC_MarketAccess_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_PDC_Dettagli As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal ID_Reg As Integer,
                           ByVal CapitolatoCliente_Cod As Integer,
                           ByVal Esito As Integer,
                           ByVal Decision As String,
                           ByVal Codice_Campione As String,
                           ByVal Resa As Decimal,
                           ByVal Blocco_Flag As Integer,
                           ByVal Blocco_Data As Date,
                           ByVal Blocco_Username As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_MarketAccess ")
            StrSQL.AppendLine(" (       PivaSuperUser, ID_PDC_Testata, ID_PDC_Dettagli,  ")
            StrSQL.AppendLine("         Piva, Sa_Cod, Appezza, Id_Reg, ")
            StrSQL.AppendLine("         CapitolatoCliente_Cod, Esito, Decision, ")
            StrSQL.AppendLine("         Codice_Campione, Resa, ")

            StrSQL.AppendLine("         Blocco_Flag, Blocco_Data, Blocco_Username, ")

            StrSQL.AppendLine("         inviato, datainvio, ")
            StrSQL.AppendLine("         Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("         Username_Creazione, Username_Modifica, ")
            StrSQL.AppendLine("         Validita_Inizio, Validita_Fine")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))

            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Reg))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Esito))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Decision) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Codice_Campione) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Resa))

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Blocco_Flag))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Blocco_Data))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Blocco_Username) & "'")

            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , NULL ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_creazione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_modifica) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAINIZIO))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(AGRODATAFINE))
            StrSQL.AppendLine(")")

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

    Public Function ModificaPuntuale(ByVal ID_PDC_Testata As Integer,
                                     ByVal ID_PDC_Dettagli As Integer,
                                     ByVal CapitolatoCliente_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Piva As String = Nothing,
                                     Optional ByVal Sa_Cod As Integer? = Nothing,
                                     Optional ByVal Appezza As Integer? = Nothing,
                                     Optional ByVal Id_Reg As Integer? = Nothing,
                                     Optional ByVal Esito As Integer? = Nothing,
                                     Optional ByVal Decision As String = Nothing,
                                     Optional ByVal Codice_Campione As String = Nothing,
                                     Optional ByVal Resa As Decimal? = Nothing,
                                     Optional ByVal Blocco_Flag As Integer? = Nothing,
                                     Optional ByVal Blocco_Data As Date? = Nothing,
                                     Optional ByVal Blocco_Username As String = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If ID_PDC_Testata = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Testata = 0)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE PDC_MarketAccess ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Piva) Then
                strSql.AppendLine(" AND Piva   =  '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not IsNothing(Appezza) Then
                strSql.AppendLine("   , Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Not IsNothing(ID_Reg) Then
                strSql.AppendLine("   , Id_Reg = " & Agro_SQL_SaveNum(ID_Reg) & " ")
            End If

            If Not IsNothing(Esito) Then
                strSql.AppendLine("   , Esito = " & Agro_vb_SaveNum(Esito) & " ")
            End If

            If Not IsNothing(Decision) Then
                strSql.AppendLine("   , Decision = '" & Agro_SQL_SaveText(Decision) & "' ")
            End If

            If Not IsNothing(Codice_Campione) Then
                strSql.AppendLine("   , Codice_Campione = '" & Agro_SQL_SaveText(Codice_Campione) & "' ")
            End If

            If Not IsNothing(Resa) Then
                strSql.AppendLine("   , Resa = " & Agro_SQL_SaveNum(Resa) & " ")
            End If

            If Not IsNothing(Blocco_Flag) Then
                strSql.AppendLine("   , Blocco_Flag = " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            End If

            If Not IsNothing(Blocco_Data) Then
                strSql.AppendLine("   , Blocco_Data = " & Agro_SQL_SaveDate(Blocco_Data) & " ")
            End If

            If Not IsNothing(Blocco_Username) Then
                strSql.AppendLine("   , Blocco_Username = '" & Agro_SQL_SaveText(Blocco_Username) & "' ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE PivaSuperUser     = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")

            If ID_PDC_Dettagli <> 0 Then
                strSql.AppendLine(" AND ID_PDC_Dettagli   =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")
            End If

            If CapitolatoCliente_Cod <> -1 Then
                strSql.AppendLine(" AND CapitolatoCliente_Cod   =  " & Agro_SQL_SaveNum(CapitolatoCliente_Cod) & "   ")
            End If

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

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal EscludiBloccati As Boolean = False
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_MarketAccess ")

            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If EscludiBloccati = True Then
                StrSQL.AppendLine(" AND Blocco_Flag <> 1 ")
            End If

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

End Class
