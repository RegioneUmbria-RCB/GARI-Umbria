

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class VerificheQualitative_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' verifica
    ''' </summary>
    ''' <param name="NomeDatabaseDa"></param>
    ''' <param name="NomeDatabaseA"></param>
    ''' <param name="NomeTabella"></param>
    ''' <param name="CondizioneTabellaDa"></param>
    ''' <param name="CondizioneTabellaA"></param>
    ''' <param name="ColonneDaEscludere"></param>
    ''' <param name="da"></param>
    ''' <param name="a"></param>
    ''' <param name="limite">esempio: "TOP 100"</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Verifica(
                            ByVal NomeDatabaseDa As String _
                            , ByVal NomeDatabaseA As String _
                            , ByVal NomeTabella As String _
                            , ByVal CondizioneTabellaDa As String _
                            , ByVal CondizioneTabellaA As String _
                            , ByVal ColonneDaEscludere As String _
                            , ByVal da As Integer _
                            , ByVal a As Integer _
                            , ByVal limite As String _
                            , ByVal xFiltroAggiuntivo As String _
                            , ByVal xOrderBy As String _
                            , ByVal NomeColonnaPiva As String _
                            , ByVal PIVA_da_filtrare As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            ''piva_superUser,data_creazione,data_modifica'
            ColonneDaEscludere = "'" & ColonneDaEscludere.Replace(",", "','") & "'"

            stb.Append(" " & vbCrLf)
            stb.Append(" " & vbCrLf)
            stb.Append(" declare @SqlStatemt varchar(max) " & vbCrLf)
            stb.Append(" set @SqlStatemt = '' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" select @SqlStatemt = @SqlStatemt + ' " & vbCrLf)

            stb.Append(" select " & limite & " dense_rank() over(order by ' + dati.colonne + ') as conta, MIN(NomeTabella) as NomeTabella, ' + dati.colonne " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" + '  " & vbCrLf)
            stb.Append(" from ( " & vbCrLf)
            stb.Append(" select dense_rank() over(order by ' + dati.colonne + ') as conta, ''' + dati.TabellaDa +  ''' as NomeTabella, ' + dati.colonne + ' " & vbCrLf)
            stb.Append(" from ' + dati.TabellaDa + ' " & vbCrLf)
            stb.Append(" Where 1 = 1 " & vbCrLf)
            If NomeColonnaPiva <> "" AndAlso PIVA_da_filtrare <> "" Then
                stb.Append(" And " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(PIVA_da_filtrare) & "' " & vbCrLf)
            End If
            If CondizioneTabellaDa <> "" Then
                stb.Append("                     '" & Agro_SQL_SaveText(CondizioneTabellaDa) & "'     " & vbCrLf)
            End If
            stb.Append(" union all " & vbCrLf)
            stb.Append(" select dense_rank() over(order by ' + dati.colonne + ') as conta, ''' + dati.TabellaA +  ''' as NomeTabella,' + dati.colonne  + ' " & vbCrLf)
            stb.Append(" from ' + dati.TabellaA + ' " & vbCrLf)
            stb.Append(" Where 1 = 1 " & vbCrLf)
            If NomeColonnaPiva <> "" AndAlso PIVA_da_filtrare <> "" Then
                stb.Append(" And " & NomeColonnaPiva & " = '" & Agro_SQL_SaveText(PIVA_da_filtrare) & "' " & vbCrLf)
            End If
            If CondizioneTabellaA <> "" Then
                stb.Append("                     '" & Agro_SQL_SaveText(CondizioneTabellaA) & "'     " & vbCrLf)
            End If
            stb.Append(" ) tmp " & vbCrLf)
            stb.Append(" where ( '+ cast( " & da & " as varchar(1000)) + ' = 0 or conta BETWEEN ' + cast(" & da & " as varchar(1000)) + ' AND ' + cast(" & a & "as varchar(1000)) + ') " & vbCrLf)
            stb.Append(" group by ' + dati.colonne + ' " & vbCrLf)
            stb.Append(" having count(*) = 1' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from (  " & vbCrLf)
            stb.Append("    select   " & vbCrLf)
            stb.Append("          '" & Agro_SQL_SaveText(NomeDatabaseDa) & "' + '.dbo.'+ '" & Agro_SQL_SaveText(NomeTabella) & "' as TabellaDa " & vbCrLf)
            stb.Append("        , '" & Agro_SQL_SaveText(NomeDatabaseA) & "' + '.dbo.'+ '" & Agro_SQL_SaveText(NomeTabella) & "' as TabellaA " & vbCrLf)
            stb.Append("        , left(colonne.cols , len(colonne.cols )-1) as colonne " & vbCrLf)
            stb.Append("    from ( " & vbCrLf)
            stb.Append("        select ( " & vbCrLf)
            stb.Append("            select distinct         " & vbCrLf)
            stb.Append("                  col.name + ', ' as [text()] " & vbCrLf)
            stb.Append("            from " & NomeDatabaseDa & ".sys.columns col " & vbCrLf)
            stb.Append("                inner join " & NomeDatabaseDa & ".sys.tables t on col.object_id = t.object_id               " & vbCrLf)
            stb.Append("            where t.name = '" & Agro_SQL_SaveText(NomeTabella) & "' " & vbCrLf)
            stb.Append("            and col.name not in ( " & vbCrLf)
            stb.Append("                " & Agro_SQL_Save_Clausola_IN(ColonneDaEscludere, True) & "  " & vbCrLf)
            stb.Append("            ) " & vbCrLf)
            stb.Append("            for xml path('') " & vbCrLf)
            stb.Append("        ) cols " & vbCrLf)
            stb.Append("    ) colonne " & vbCrLf)
            stb.Append(" ) dati " & vbCrLf)


            stb.Append(" exec(@SqlStatemt) " + vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


End Class


