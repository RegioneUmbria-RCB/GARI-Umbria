Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class LFO_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_LFO As Integer,
                          ByVal LFO_Des As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.LFO_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  PDC_LFO")
            StrSQL.AppendLine(" WHERE  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_LFO <> 0 Then
                StrSQL.AppendLine(" AND ID_LFO = " & Agro_SQL_SaveNum(ID_LFO))
            End If
            If LFO_Des <> "" Then
                StrSQL.AppendLine(" AND LFO_Des = '" & Agro_SQL_SaveText(LFO_Des) & "'")
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

    Public Function Exist_Dettaglio(ByVal ID_PDC_Testata As Integer,
                                    ByVal ID_LFO As Integer,
                                    ByVal LFO_Des As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.PDC_R.Exist_Dettaglio()"

        Dim mMessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM PDC_LFO ")

            StrSQL.AppendLine(" WHERE 1=1")
            StrSQL.AppendLine(" AND   PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_LFO <> 0 Then
                StrSQL.AppendLine(" AND ID_LFO = " & Agro_SQL_SaveNum(ID_LFO))
            End If


            If LFO_Des <> "" Then
                StrSQL.AppendLine(" AND LFO_Des = '" & Agro_SQL_SaveText(LFO_Des) & "'")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            mMessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, mMessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & mMessaggioErrore)
        End Try

        If dt.Rows.Count > 0 Then
            Return True
        End If

        Return False

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class LFO_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_LFO As Integer,
                           ByVal LFO_Des As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.LFO_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_LFO (PivaSuperUser , ID_PDC_Testata, ID_LFO, ")
            StrSQL.AppendLine("                                 LFO_Des) ")

            StrSQL.AppendLine("   VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_LFO))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(LFO_Des) & "'")
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

    Public Function Modifica(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_LFO As Integer,
                             ByVal LFO_Des As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaPianidiCampionamentoDAL.LFO_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_LFO SET ")
            StrSQL.AppendLine("   LFO_Des         =  '" & Agro_SQL_SaveText(LFO_Des) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_LFO    =  " & Agro_SQL_SaveNum(ID_LFO) & "   ")

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

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_LFO As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCorePianidiCampionamentoDAL.LFO_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_LFO ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            If ID_LFO <> 0 Then
                StrSQL.AppendLine("     AND ID_LFO = " & Agro_SQL_SaveNum(ID_LFO))
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
