Imports System.Text


Public Class ClassiDiSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function SpecieAgronicaFromClasseDiSpecie(
        ByVal ID_Specie As Integer,
        ByVal ID_SottoSpecie As Integer,
        ByVal ID_Gruppo As Integer,
        ByVal ID_Genotipo As Integer,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "ClassiDiSpecie_R.SpecieAgronicaFromClasseDiSpecie"
        Dim Stb As New StringBuilder
        Try
            Stb.AppendLine("select distinct veg.veg_cod, veg.veg_des ")
            Stb.AppendLine(" from Sementieri_ClassiDiSpecieVegetali ss ")
            Stb.AppendLine("    inner join Mappatura_Specie mp  ")
            Stb.AppendLine("        on  mp.ID_Specie = ss.id_specie ")
            Stb.AppendLine("        and mp.ID_Sottospecie = ss.ID_Sottospecie ")
            Stb.AppendLine("        and mp.ID_Gruppo = ss.ID_Gruppo  ")
            Stb.AppendLine("        and mp.ID_Genotipo = ss.ID_Genotipo  ")
            Stb.AppendLine("    inner join SpecieVegetali  veg ")
            Stb.AppendLine("        on veg.veg_cod = mp.veg_cod ")
            Stb.AppendLine(" where 1=1 ")

            If ID_Specie <> 0 Then
                Stb.AppendLine(" and mp.ID_Specie = " & ID_Specie)
            End If
            If ID_SottoSpecie <> 0 Then
                Stb.AppendLine(" and mp.ID_SottoSpecie = " & ID_SottoSpecie)
            End If
            If ID_Gruppo <> 0 Then
                Stb.AppendLine(" and mp.ID_Gruppo = " & ID_Gruppo)
            End If
            If ID_Genotipo <> 0 Then
                Stb.AppendLine(" and mp.ID_Genotipo = @ID_Genotipo ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function distinct_sementieri_classidispecievegetali_des(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "ClassiDiSpecie_R.SpecieAgronicaFromClasseDiSpecie"
        Dim Stb As New StringBuilder
        Try
            Stb.AppendLine("select distinct id_specie, sementieri_classidispecievegetali_des ")
            Stb.AppendLine(" from Sementieri_ClassiDiSpecieVegetali ")
            Stb.AppendLine(" order by sementieri_classidispecievegetali_des ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    ' Aggiunta 30/01/2026, utilizzata in AgronicaCore_2010
    Public Function GetClasseSpecieXUtente(
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim DT As DataTable
        Dim NomeRoutine As String = "ClassiDiSpecie_R.GetClasseSpecieXUtente"
        Dim Stb As New StringBuilder
        Try
            Stb.AppendLine("SELECT DISTINCT")
            Stb.AppendLine("  cs.id_specie, cs.sementieri_classidispecievegetali_des,")
            Stb.AppendLine("  u.UserName, u.Nome, u.Cognome")
            Stb.AppendLine("FROM Mappatura_Specie_x_Utente")
            Stb.AppendLine("LEFT JOIN Sementieri_ClassiDiSpecieVegetali cs on cs.ID_Specie = Mappatura_Specie_x_Utente.Id_Specie")
            Stb.AppendLine($"LEFT JOIN [{objParametri_Utenti.Recupera_NomeDB()}].[dbo].[Utenti_Dettagli] u on u.UserName = Mappatura_Specie_x_Utente.Username")
            Stb.AppendLine("WHERE u.UserName IS NOT NULL")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

End Class



Public Class ClassiDiSpecie_W 
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function RemoveClasseSpecieXUtente(
        usernames As IEnumerable(Of String),
        vegCod As IEnumerable(Of Integer),
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )
        Dim NomeRoutine As String = "ClassiDiSpecie_R.RemoveClasseSpecieXUtente"
        Dim Stb As New StringBuilder
        usernames = usernames.Select(Function(u) If(u.StartsWith("'"), u, "'" & u)).
            Select(Function(u) If(u.EndsWith("'"), u, u & "'"))
        Try
            Stb.AppendLine("DELETE FROM Mappatura_Specie_x_Utente")
            Stb.AppendLine("WHERE UserName IN (" & String.Join(",", usernames) & ")")
            Stb.AppendLine("  AND id_specie IN (" & String.Join(",", vegCod) & ")")

            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function AddClasseSpecieXUtente(
        username As String,
        vegCod As IEnumerable(Of Integer),
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    )
        Dim NomeRoutine As String = "ClassiDiSpecie_R.RemoveClasseSpecieXUtente"
        Dim Stb As New StringBuilder
        username = If(username.StartsWith("'"), username, "'" & username)
        username = If(username.EndsWith("'"), username, username & "'")
        Try
            Stb.AppendLine("INSERT INTO Mappatura_Specie_x_Utente (id_specie, username)")
            Stb.AppendLine("VALUES")
            vegCod.select(Function(veg, i) $"({veg}, {username})" & If(i < vegCod.Count - 1, "," & vbCrLf, "")).ToList.
                ForEach(Sub(value) Stb.AppendLine(value))
            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class
