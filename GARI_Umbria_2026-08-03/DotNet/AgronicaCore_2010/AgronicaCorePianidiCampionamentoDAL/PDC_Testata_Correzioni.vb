Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProviders
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class PDC_Testata_Correzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal Analisi_Parametro_Cod As Integer,
                          ByVal ID_Correzione_Testata As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Testata_Correzioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Analisi_Correzioni_Testata.ID_Correzione_Testata, Analisi_Correzioni_Testata.Nome, ")
            StrSQL.AppendLine(" Analisi_Parametri.Analisi_Parametro_Cod, Analisi_Parametri.Analisi_Parametro_Des, Analisi_Parametri.Analisi_Parametro_Simbolo ")
            StrSQL.AppendLine(" FROM  PDC_Testata_Correzioni  ")
            StrSQL.AppendLine(" Inner Join  Analisi_Correzioni_Testata On (Analisi_Correzioni_Testata.ID_Correzione_Testata =  PDC_Testata_Correzioni.ID_Correzione_Testata) ")
            StrSQL.AppendLine(" Inner Join  Analisi_Parametri On (Analisi_Parametri.Analisi_Parametro_Cod =  PDC_Testata_Correzioni.Analisi_Parametro_Cod) ")


            StrSQL.AppendLine(" WHERE PDC_Testata_Correzioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod))
            End If
            If ID_Correzione_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.ID_Correzione_Testata = " & Agro_SQL_SaveNum(ID_Correzione_Testata))
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

Public Class PDC_Testata_Correzioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrivi"

    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal Analisi_Parametro_Cod As Integer,
                           ByVal ID_Correzione_Testata As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal Username_Creazione As String = "",
                           Optional ByVal Username_Modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Testata_Correzioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If Username_Creazione = "" Then
            Username_Creazione = objParametri.UsernameOperazione
        End If

        If Username_Modifica = "" Then
            Username_Modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Testata_Correzioni(PivaSuperUser, ID_PDC_Testata, Analisi_Parametro_Cod, ID_Correzione_Testata, ")
            StrSQL.AppendLine("  Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Parametro_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Correzione_Testata))

            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(Username_Modifica) & "' ")

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


#End Region

#Region "Modifica"

    <Obsolete("Usare PDC_Analisi_W.ModificaPuntuale")>
    Public Function Modifica(ByVal ID_PDC_Testata As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal ID_Correzione_Testata As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Testata_Correzioni_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Testata_Correzioni SET ")

            StrSQL.AppendLine("   Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   , Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")
            StrSQL.AppendLine("   , ID_Correzione_Testata   =  " & Agro_SQL_SaveNum(ID_Correzione_Testata) & "   ")

            StrSQL.AppendLine(" WHERE PDC_Testata_Correzioni.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod))
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


#End Region

#Region "Cancellazione"

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal ID_Correzione_Testata As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianidiCampionamentoDAL.PDC_Testata_Correzioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Testata_Correzioni ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If
            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod))
            End If
            If ID_Correzione_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Testata_Correzioni.ID_Correzione_Testata = " & Agro_SQL_SaveNum(ID_Correzione_Testata))
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



#End Region

End Class
