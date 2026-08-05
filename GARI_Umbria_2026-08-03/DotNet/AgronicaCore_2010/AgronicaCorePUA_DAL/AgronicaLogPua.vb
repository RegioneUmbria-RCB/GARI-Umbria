Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class AgronicaLogPua_W
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
                           ByVal param7 As String,
                           ByVal param8 As String,
                           ByVal param9 As String,
                           ByVal param10 As String,
                           ByVal note As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCorePUA_DAL.AgronicaLogPua_R.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If param1 Is Nothing Then
                Throw New Exception("Il primo parametro deve essere diverso da Nothing")
            End If

            If param2 Is Nothing Then
                Throw New Exception("Il secondo parametro deve essere diverso da Nothing")
            End If

            Dim chiave As String = ""

            Select Case tipo

                Case "PUA_Testata"

                    'in questo caso la chiave è PuaCod_RegolamentoCod (Param1_Param2)
                    'ma salverò anche Piva (Param3), SaCod (Param4), PuaAnno (Param5), PuaTipo (Param6)
                    'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                    chiave = param1 & "_" & param2

                Case "PUA_Effluente"

                    If param3 Is Nothing Then
                        Throw New Exception("Il terzo parametro deve essere diverso da Nothing")
                    End If
                    'in questo caso la chiave è ID (Param3)
                    'ma salverò anche PuaCod (Param1), RegolamentoCod (Param2), Eff_Cod (Param4), Azoto_Titoli (Param5), Flag_ProvenienzaEsterna (Param6)
                    'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                    chiave = param3

                Case "Anagrafe_VincoliAgronomici"

                    If param3 Is Nothing Then
                        Throw New Exception("Il terzo parametro deve essere diverso da Nothing")
                    End If
                    'in questo caso la chiave è ID (Param3)
                    'ma salverò anche PuaCod (Param1), RegolamentoCod (Param2), Piva (Param4), SaCod (Param5), Appezza (Param6), IdReg (Param7), ProgettoCod (Param8)
                    'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                    chiave = param3

                Case "PUA_LetamazioniPrecedenti"

                    If param3 Is Nothing Then
                        Throw New Exception("Il terzo parametro deve essere diverso da Nothing")
                    End If
                    'in questo caso la chiave è ID (Param3)
                    'ma salverò anche PuaCod (Param1), RegolamentoCod (Param2), Piva (Param4), SaCod (Param5), Appezza (Param6), IdReg (Param7), ProgettoCod (Param8)
                    'perché possono essere utili (specie in caso di eliminazione) per capire a quale impianto apparteneva la distinta
                    chiave = param3

                Case Else

                    Throw New Exception("Casistica non prevista")

            End Select



            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Pua ")
            strSql.AppendLine("             ( SuperUser, Utente, Tipo_Operazione, Tipo, ")
            strSql.AppendLine("               Chiave, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10,")
            strSql.AppendLine("               Note, Data_Ora_RegistrazioneLog) ")

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
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param7) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param8) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param9) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(param10) & " ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(note) & "'  ")
            strSql.AppendLine("         , GETDATE() ")
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
