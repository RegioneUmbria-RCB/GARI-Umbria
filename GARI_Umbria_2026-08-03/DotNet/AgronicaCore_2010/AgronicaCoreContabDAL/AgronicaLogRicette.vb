Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AgronicaLogRicette_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_UltimaOperazione(
                    ByVal SuperUser As String,
                    ByVal UltimaOperazione As enum_TipoOperazioneDB,
                    ByVal Tipo As String,
                    ByVal Chiave As String,
                    ByVal Param1 As String,
                    ByVal Param2 As String,
                    ByVal Param3 As String,
                    ByVal Param4 As String,
                    ByVal Param5 As String,
                    ByVal Param6 As String,
                    ByVal Raccoglitore_Cod As Integer,
                    ByVal Id_Servizio As Integer,
                    ByVal xFiltroAggiuntivo As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogRicette_R.Leggi_UltimaOperazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Agronica_Log_Ricette_UltimaOperazione ")
            stb.AppendLine(" WHERE 1=1 ")

            If SuperUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(SuperUser) & "' ")
            End If

            If UltimaOperazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND UltimaOperazione = " & Agro_SQL_SaveNum(UltimaOperazione) & " ")
            End If

            If Tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            End If


            If Chiave <> "" Then
                stb.AppendLine(" AND Chiave = '" & Agro_SQL_SaveText(Chiave) & "' ")
            End If

            If Param1 <> "" Then
                stb.AppendLine(" AND Param1 = '" & Agro_SQL_SaveText(Param1) & "' ")
            End If

            If Param2 <> "" Then
                stb.AppendLine(" AND Param2 = '" & Agro_SQL_SaveText(Param2) & "' ")
            End If

            If Param3 <> "" Then
                stb.AppendLine(" AND Param3 = '" & Agro_SQL_SaveText(Param3) & "' ")
            End If

            If Param4 <> "" Then
                stb.AppendLine(" AND Param4 = '" & Agro_SQL_SaveText(Param4) & "' ")
            End If

            If Param5 <> "" Then
                stb.AppendLine(" AND Param5 = '" & Agro_SQL_SaveText(Param5) & "' ")
            End If

            If Param6 <> "" Then
                stb.AppendLine(" AND Param6 = '" & Agro_SQL_SaveText(Param6) & "' ")
            End If

            If Raccoglitore_Cod <> 0 Then
                stb.AppendLine(" AND Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
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

    Public Function LeggiRicettaOperazioneDaRaccoglitoreCod(ByVal raccoglitore_cod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogRicette_R.LeggiRicettaOperazioneDaRaccoglitoreCod()"
        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     SuperUser as Ricetta_SuperUser, Param5 as Lav_Cod, Param1 as Ricetta_Cod, Chiave as Ricetta_Operazione_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Agronica_Log_Ricette_UltimaOperazione ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Raccoglitore_Cod = " & Agro_SQL_SaveNum(raccoglitore_cod) & " ")
            sb.AppendLine("     AND Tipo = 'Ricette_Operazioni'")
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function LeggiBrogliacciCancellatixApp(ByVal piva As String,
                                                  ByVal dataLavorazioneMin As Date,
                                                  ByVal tipiDemetra As List(Of String),
                                                  ByVal tipiApp As List(Of String),
                                                  ByVal dataUltimaSincro As Date,
                                                  ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogRicette_R.LeggiBrogliacciCancellati()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.Length = 0

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine("WITH #cteAppDatiDemetra as ( ")
            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Id, Tipo, SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento)  ) Ricetta_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     APP_Dati ad ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Tipo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", tipiDemetra), True) & ") ")
            sb.AppendLine("     AND CHARINDEX('|', riferimento) > 0 ")
            sb.AppendLine("     AND Piva = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine("), ")

            sb.AppendLine("")

            sb.AppendLine("#cteAppDatiApp as ( ")
            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Id, Tipo, SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento)  ) Ricetta_Operazione_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     APP_Dati ad ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Tipo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", tipiApp), True) & ") ")
            sb.AppendLine("     AND CHARINDEX('|', riferimento) > 0 ")
            sb.AppendLine("     AND Piva = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine("), ")

            sb.AppendLine("")

            sb.AppendLine("#logRicette AS ")
            sb.AppendLine(" (")
            sb.AppendLine("     select ")
            sb.AppendLine("         chiave as Ricetta_Operazione_Cod, param1 as Ricetta_Cod ")
            sb.AppendLine("     from ")
            sb.AppendLine("         Agronica_Log_Ricette_UltimaOperazione")
            sb.AppendLine("     where")
            sb.AppendLine("         Param3 = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine("         AND Tipo = 'Ricette_Operazioni'")
            sb.AppendLine("         AND ISDATE(Param6) = 1")
            sb.AppendLine("         AND CONVERT(datetime, Param6, 103) >= " & Agro_SQL_SaveDate(dataLavorazioneMin) & " ")
            sb.AppendLine(" 	      AND Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataUltimaSincro) & " ")
            sb.AppendLine("         AND Param5 IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            sb.AppendLine("         AND UltimaOperazione = 3 ") 'cancellazioni
            sb.AppendLine(" )")

            sb.AppendLine("")

            sb.AppendLine(" select ")
            sb.AppendLine(" 	DISTINCT COALESCE(a.Id, d.Id, '') as GuidRicetta, COALESCE(a.Tipo, d.Tipo, '') as Tipo ")
            sb.AppendLine(" from ")
            sb.AppendLine(" 	#logRicette lr ")
            sb.AppendLine(" left join ")
            sb.AppendLine(" 	#cteAppDatiApp a ")
            sb.AppendLine(" 	on a.Ricetta_Operazione_Cod = lr.Ricetta_Operazione_Cod ")
            sb.AppendLine(" left join ")
            sb.AppendLine(" 	#cteAppDatiDemetra d ")
            sb.AppendLine(" 	on d.Ricetta_Cod = lr.Ricetta_Cod ")
            sb.AppendLine(" where ")
            sb.AppendLine(" 	COALESCE(a.Id, d.Id, '') <> '' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
        End Try

        Return dt

    End Function

    Public Function EsistonoModificheDopoLaDataXBrogliacciApp(ByVal piva As String,
                                                ByVal dataLavorazioneMin As Date,
                                                ByVal dataModificheMin As Date,
                                                ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogRicette_R.EsistonoModificheDopoLaDataXAttivitaApp()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable
        Dim result = True

        Try

            sb.Length = 0

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT TOP 1 ")
            sb.AppendLine("     1 ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Agronica_Log_Ricette_UltimaOperazione")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Param3 = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine("     AND Tipo = 'Ricette_Operazioni'")
            sb.AppendLine("     AND ISDATE(Param6) = 1")
            sb.AppendLine("     AND CONVERT(datetime, Param6, 103) >= " & Agro_SQL_SaveDate(dataLavorazioneMin) & " ")
            sb.AppendLine("     AND Param5 IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            sb.AppendLine("     AND Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataModificheMin) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            result = dt IsNot Nothing AndAlso dt.Rows.Count > 0

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            result = True
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result

    End Function

End Class

Public Class AgronicaLogRicette_W
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
                           ByVal Optional object_data_XML As String = "",
                           ByVal Optional Raccoglitore_Cod As Integer = 0,
                           ByVal Optional guidRicetta As String = ""
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogRicette_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If param1 Is Nothing Then
                Throw New Exception("Almeno il primo parametro deve essere diverso da Nothing")
            End If

            Dim chiave As String = ""

            If tipo = "Ricette" Then

                'in questo caso la chiave è solo Ricetta_Cod (Param1)
                'ma salverò anche Piva (Param2) SaCod (Param3), Tipo_Ricetta (Param4), Programmazione_Cod (Param5)
                'perché possono essere utili (specie in caso di eliminazione)
                chiave = param1

            ElseIf tipo = "Ricette_Operazioni" Then

                If param2 Is Nothing Then
                    Throw New Exception("Anche il secondo parametro deve essere diverso da Nothing")
                End If

                'in questo caso la chiave è solo Ricetta_Operazione_Cod (Param2)
                'ma salverò anche Ricetta_Cod (Param1) Piva (Param3) Sa_Cod (Param4) Lav_Cod (Param5) Data (Param6)
                'perché possono essere utili (specie in caso di eliminazione) per capire a quale animale apparteneva la distinta
                chiave = param2

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

            strSql.AppendLine(" INSERT INTO Agronica_Log_Ricette ")
            strSql.AppendLine("             ( SuperUser, Utente, Tipo_Operazione, Tipo, ")
            strSql.AppendLine("               Chiave, Param1, Param2, Param3, Param4, Param5, Param6, ")
            strSql.AppendLine("               Note, Data_Ora_RegistrazioneLog, Id_Servizio, object_data, Raccoglitore_Cod, GuidRicetta) ")

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
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(object_data_XML) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Raccoglitore_Cod) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(guidRicetta) & "'  ")
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


End Class
