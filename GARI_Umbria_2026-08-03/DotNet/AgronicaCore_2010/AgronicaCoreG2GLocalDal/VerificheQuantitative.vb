

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class VerificheQuantitative_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function LeggiColonneDataTabella( _
                                    ByVal nomeDatabase As String _
                                    , ByVal nomeTabella As String _
                                , ByVal NomeColonna As String _
                                , ByVal IncludiColonneGias As Boolean _
                                , ByVal xFiltroAggiuntivo As String _
                                , ByVal xOrderBy As String _
                                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append(" set nocount on " & vbCrLf)
            stb.Append(" " & vbCrLf)
            stb.Append(" select " & vbCrLf)
            stb.Append("      t.name as TableName  " & vbCrLf)
            stb.Append("    , col.name as ColName " & vbCrLf)
            stb.Append("    , ty.name as tipodato " & vbCrLf)
            stb.Append("    , case when ty.system_type_id in (167, 231, 175, 239) then cast(col.max_length as nvarchar(100)) else '' end as lunghezza " & vbCrLf)
            stb.Append("    , col.is_nullable as AmmettiNull " & vbCrLf)
            stb.Append("    , col.collation_name  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from " & nomeDatabase & ".sys.columns col " & vbCrLf)
            stb.Append("    inner join " & nomeDatabase & ".sys.tables t on col.object_id = t.object_id  " & vbCrLf)
            stb.Append("    inner join " & nomeDatabase & ".sys.types ty on ty.system_type_id = col.system_type_id " & vbCrLf)
            stb.Append(" where 1=1 " & vbCrLf)
            If nomeTabella <> "" Then
                stb.Append(" and t.name = '" & Agro_SQL_SaveText(nomeTabella) & "' " & vbCrLf)
            Else
                If nomeDatabase <> "" Then
                    stb.AppendLine(" and t.name in (select nome_tabella from G2G_Verifiche_Configurazione) ")
                End If
            End If
            stb.Append("  " & vbCrLf)
            If NomeColonna <> "" Then
                stb.Append(" and col.name like '%" & NomeColonna & "%' " & vbCrLf)
            End If

            If Not IncludiColonneGias Then
                stb.Append(" and col.name not in ( " & vbCrLf)
                stb.Append("                 'Inviato' " & vbCrLf)
                stb.Append(" ,'DataInvio' " & vbCrLf)
                stb.Append(" ,'Data_Creazione' " & vbCrLf)
                stb.Append(" ,'Data_Modifica' " & vbCrLf)
                stb.Append(" ,'UserName_Creazione' " & vbCrLf)
                stb.Append(" ,'UserName_Modifica' " & vbCrLf)
                stb.Append(" ,'Validita_Inizio' " & vbCrLf)
                stb.Append(" ,'Validita_Fine' " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append(" ) " & vbCrLf)
            End If

            stb.Append(" and ty.name<> 'sysname' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" order by col.column_id " & vbCrLf)


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


    '##############################################################################################
    Public Function VerificaDB(
                                    ByVal dbOrigine As String _
                                , ByVal dbDestinazione As String _
                                , ByVal xFiltroAggiuntivo As String _
                                , ByVal xOrderBy As String _
                                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            Dim dtVerifiche As DataTable
            dtVerifiche = LeggiCfgVerificaDB(
                dbOrigine _
                , dbDestinazione _
                , "" _
                , "" _
                , objParametri
            )

            stb.Append(" set nocount on " & vbCrLf)
            stb.Append(" declare @DbNameOrigine varchar(1000) " & vbCrLf)
            stb.Append(" declare @DbNameDestinazione varchar(1000) " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" set @DbNameOrigine = '" & Agro_SQL_SaveText(dbOrigine) & "' " & vbCrLf)
            stb.Append(" set @DbNameDestinazione = '" & Agro_SQL_SaveText(dbDestinazione) & "' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" create table #pivas(piva varchar(1000), ragSoc varchar(4000)) " & vbCrLf)
            stb.Append(" create table #result(dbname varchar(1000), nomeTabella nvarchar(1000), conteggio int) " & vbCrLf)

            stb.Append(" " & vbCrLf)
            stb.Append(" declare @nrecPivas int " & vbCrLf)
            stb.Append(" select @nrecPivas = COUNT(*) " & vbCrLf)
            stb.Append(" from #pivas  " & vbCrLf)

            Dim lcount As Integer = dtVerifiche.Rows.Count
            Dim i As Integer = 0
            stb.Append(" INSERT  #result  " & vbCrLf)

            For Each idtVerifiche In dtVerifiche.Rows

                i += 1

                Dim lNomeColonnaPiva As String = idtVerifiche("nome_colonna_piva")

                stb.Append(" " & vbCrLf)
                stb.Append("  select " & vbCrLf)
                stb.Append("  @DbNameOrigine as dbname " & vbCrLf)
                stb.Append("  , '" & Agro_SQL_SaveText(idtVerifiche("nome_tabella")) & "' as nome " & vbCrLf)
                stb.Append("  , count(*) as conteggio " & vbCrLf)
                stb.Append("  --, * " & vbCrLf)
                stb.Append(" from " & dbOrigine & ".dbo." & idtVerifiche("nome_tabella") & vbCrLf)
                If lNomeColonnaPiva <> "" Then


                    stb.Append(" where ( " & lNomeColonnaPiva & " in (select PIVA collate SQL_Latin1_General_CP850_CI_AS from  #pivas ) " & getPar(idtVerifiche("nome_colonna_piva")) & vbCrLf)
                    stb.Append(" and @nrecPivas>0  ) " & vbCrLf)
                    stb.Append(" or @nrecPivas = 0 " & vbCrLf)

                End If
                stb.Append(" union " & vbCrLf)
                stb.Append(" select " & vbCrLf)
                stb.Append("  @DbNameDestinazione as dbname " & vbCrLf)
                stb.Append("  , '" & Agro_SQL_SaveText(idtVerifiche("nome_tabella")) & "' as nome " & vbCrLf)
                stb.Append("  , count(*) as conteggio " & vbCrLf)
                stb.Append("  --, * " & vbCrLf)
                stb.Append(" from " & dbDestinazione & ".dbo." & idtVerifiche("nome_tabella") & vbCrLf)
                If lNomeColonnaPiva <> "" Then
                    stb.Append(" where ( " & lNomeColonnaPiva & " in (select PIVA collate SQL_Latin1_General_CP850_CI_AS from  #pivas ) " & getPar(idtVerifiche("nome_colonna_piva")) & vbCrLf)
                    stb.Append(" and @nrecPivas>0  ) " & vbCrLf)
                    stb.Append(" or @nrecPivas = 0 " & vbCrLf)
                End If

                If i < lcount Then
                    stb.Append(" union " & vbCrLf)
                End If

            Next

            stb.Append(" " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" order by 2,1 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" " & vbCrLf)


            stb.Append(" select   " & vbCrLf)
            stb.Append("       dbFrom.nomeTabella as  Nome_Tabella " & vbCrLf)
            stb.Append("     , dbFrom.conteggio as  n_record_da_query_origine " & vbCrLf)
            stb.Append("     , dbTo.conteggio as n_record_da_query_destinazione " & vbCrLf)
            stb.Append("     , 0 as Differenza_Aspettata " & vbCrLf)
            stb.Append("     , dbTo.conteggio - dbFrom.conteggio as Differenza_Rilevata " & vbCrLf)

            stb.Append(" from ( " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("    nomeTabella  " & vbCrLf)
            stb.Append("    , conteggio  " & vbCrLf)
            stb.Append(" from #result  " & vbCrLf)
            stb.Append(" where dbname = @DbNameOrigine " & vbCrLf)
            stb.Append(" ) dbFrom " & vbCrLf)
            stb.Append(" inner join  " & vbCrLf)
            stb.Append(" (select  " & vbCrLf)
            stb.Append("    nomeTabella  " & vbCrLf)
            stb.Append("    , conteggio " & vbCrLf)
            stb.Append(" from #result  " & vbCrLf)
            stb.Append(" where dbname = @DbNameDestinazione  " & vbCrLf)
            stb.Append(" ) dbTo on dbFrom.NomeTabella = dbTo.nomeTabella  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            'stb.Append(" where dbTo.conteggio - dbFrom.conteggio<>0  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" drop table #pivas " & vbCrLf)
            stb.Append(" drop table #result " & vbCrLf)
            stb.Append("  " & vbCrLf)



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

    Private Function getPar(ByVal s As String) As String
        Return "".PadLeft(CountCharacter(s, "(")).Replace(" ", ")")
    End Function

    Public Function CountCharacter(ByVal value As String, ByVal ch As Char) As Integer
        Return value.Count(Function(c As Char) c = ch)
    End Function

    '##############################################################################################
    Public Function LeggiCfgVerificaDB(
                                    ByVal dbOrigine As String _
                                , ByVal dbDestinazione As String _
                                , ByVal xFiltroAggiuntivo As String _
                                , ByVal xOrderBy As String _
                                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" SELECT     * " & vbCrLf)
            stb.Append(" FROM         G2G_Verifiche_Configurazione " & vbCrLf)
            stb.Append(" where AttivoVerificheQuantitative = 1 " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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





'#################################################################
'#################################################################
'#################################################################

Public Class VerificheQuantitative_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#################################################################
    Public Function AggiornaColonneVisibili(
                                            ByVal Tabella As String _
                                            , ByVal Colonne As String _
                                            , ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AggiornaColonneVisibili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------

            StrSQL.Append(" UPDATE G2G_Verifiche_Configurazione ")
            StrSQL.Append(" SET ")
            StrSQL.Append("         colonne_Da_Escludere = '" & Agro_SQL_SaveText(Colonne) & "' ")
            StrSQL.Append(" WHERE   nome_tabella ='" & Agro_SQL_SaveText(Tabella) & "'")


            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class


