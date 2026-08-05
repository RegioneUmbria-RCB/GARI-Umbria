Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Flex2B_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <param name="Stato">Filtro default = -99</param>
    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_PDC_Dettagli As Integer,
                          ByVal Stato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Flex2B_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM PDC_Flex2B ")
            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Stato <> -99 Then
                StrSQL.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(Stato))
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


Public Class PDC_Flex2B_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_PDC_Dettagli As Integer,
                           ByVal Stato As Integer,
                           ByVal Payload As String,
                           ByVal Data_Operazione As DateTime,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Flex2B_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO PDC_Flex2B ")
            StrSQL.AppendLine(" (       PivaSuperUser, ID_PDC_Testata, ID_PDC_Dettagli,  ")
            StrSQL.AppendLine("         Stato, Payload, Data_Operazione, ")

            StrSQL.AppendLine("         inviato, datainvio, ")
            StrSQL.AppendLine("         Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("         Username_Creazione, Username_Modifica, ")
            StrSQL.AppendLine("         Validita_Inizio, Validita_Fine")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Stato))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Payload) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Operazione))

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
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Stato As Integer? = Nothing,
                                     Optional ByVal Payload As String = Nothing,
                                     Optional ByVal Data_Operazione As DateTime? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Flex2B_W.ModificaPuntuale()"

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

            If ID_PDC_Dettagli = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Dettagli = 0)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE PDC_Flex2B ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Stato) Then
                strSql.AppendLine("   , Stato = " & Agro_SQL_SaveNum(Stato) & " ")
            End If

            If Not IsNothing(Payload) Then
                strSql.AppendLine("   , Payload = '" & Agro_SQL_SaveText(Payload) & "'   ")
            End If

            If Not IsNothing(Data_Operazione) Then
                strSql.AppendLine("   , Data_Operazione = " & Agro_SQL_SaveDateTime(Data_Operazione) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE PivaSuperUser     = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            strSql.AppendLine(" AND ID_PDC_Dettagli   =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & " ")

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

    Public Function ScriviAggiornaRecord(ByVal ID_PDC_Testata As Integer,
                                         ByVal ID_PDC_Dettagli As Integer,
                                         ByVal Stato As Integer,
                                         ByVal Payload As String,
                                         ByVal Data_Operazione As DateTime,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_MarketAccess_W.ScriviAggiornaRecord()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Try
            
            'Scrivo il record (se c'era già lo aggiorno)
            Dim objFlexLogR As new AgronicaCorePianidiCampionamentoDAL.PDC_Flex2B_R
            Dim dt As DataTable = objFlexLogR.Leggi(ID_PDC_Testata, ID_PDC_Dettagli, -99, "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                xRisp = ModificaPuntuale(ID_PDC_Testata, ID_PDC_Dettagli, objParametri, Stato, Payload, Data_Operazione)
            Else
                xRisp = Scrivi(ID_PDC_Testata, ID_PDC_Dettagli, Stato, Payload, Data_Operazione, objParametri)
            End If
            
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
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Flex2B_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Flex2B ")

            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
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
