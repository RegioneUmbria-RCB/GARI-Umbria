Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class Analisi_Conformita_Capitolato_Cliente_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Analisi_Testata_Cod As Integer,
                          ByVal CapitolatoCliente_Cod As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "Analisi_Conformita_Capitolato_Cliente_R.Leggi()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT  *  ")
            StrSQL.AppendLine(" FROM  Analisi_Conformita_Capitolato_Cliente ")

            StrSQL.AppendLine(" WHERE 1=1 ")

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If
            If CapitolatoCliente_Cod <> -99 Then
                StrSQL.AppendLine(" AND CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
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




'#################################################################
'#################################################################
'#################################################################

Public Class Analisi_Conformita_Capitolato_Cliente_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    '##############################################################################################
    Public Function Scrivi(ByVal Analisi_Testata_Cod As Integer,
                           ByVal CapitolatoCliente_cod As Integer,
                           ByVal Esito As Integer,
                           ByVal Descrizione_Esito As String,
                           ByVal Capitolato_Des As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.Analisi_Conformita_Capitolato_Cliente_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.AppendLine(" INSERT INTO Analisi_Conformita_Capitolato_Cliente ")

            StrSQL.AppendLine(" (")
            StrSQL.AppendLine("   PivaSuperUser, Analisi_Testata_Cod, CapitolatoCliente_Cod ")
            StrSQL.AppendLine("  , Esito, Descrizione_Esito, Capitolato_Des ")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(CapitolatoCliente_cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Esito) & " ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(Descrizione_Esito) & "' ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(Capitolato_Des) & "' ")
            
            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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
