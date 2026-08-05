Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources

Public Class GuidaImpostazioniDAL
    Inherits DataProvider

    Public Function AggiungiImpostazioneTabellaGuida(codice As Integer, descrizione As String,
                                                    flagSuperUser As Integer, flagUser As Integer,
                                                    flagImpresaCentro As Integer, flagInApp As Integer,
                                                    sezione As Integer, sottoSezione As Integer,
                                                    livello As Integer, objParametri As AgronicaCoreParametri)
        Dim nomeRoutine As String = "AgronicaCoreMetaschemaDAL.GuidaImpostazioniDAL.AggiungiImpostazioneTabellaGuida()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Guida_Impostazioni ( ")
            strSql.AppendLine("     Impostazione_Cod,")
            strSql.AppendLine("     Label,")
            strSql.AppendLine("     Tipo_Campo, ")
            strSql.AppendLine("     Note,")
            strSql.AppendLine("     Valore_Default,")

            strSql.AppendLine("     Sezione,")
            strSql.AppendLine("     SottoSezione,")
            strSql.AppendLine("     Livello,")
            strSql.AppendLine("     Ordine,")
            strSql.AppendLine("     inviato,")

            strSql.AppendLine("     Impostazione_SuperUser,")
            strSql.AppendLine("     Impostazione_Utente,")
            strSql.AppendLine("     Impostazione_Azienda_Centro,")
            strSql.AppendLine("     Impostazione_Azienda_Centro_Specie,")
            strSql.AppendLine("     Flag_InApp")

            strSql.AppendLine(" ) VALUES (")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(codice) & ",")
            strSql.AppendLine("     '" & Agro_SQL_SaveText(descrizione) & "',")
            strSql.AppendLine("     '0', ")
            strSql.AppendLine("     '','',")

            strSql.AppendLine("     " & Agro_SQL_SaveNum(sezione) & ",")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(sottoSezione) & ",")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(livello) & ",")
            strSql.AppendLine("     0, 0,")

            strSql.AppendLine("     " & Agro_SQL_SaveNum(flagSuperUser) & ",")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(flagUser) & ",")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(flagImpresaCentro) & ",")
            strSql.AppendLine("     0,")
            strSql.AppendLine("     " & Agro_SQL_SaveNum(livello) & " ")
            strSql.AppendLine(" )")

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

    Public Function LeggiSezioniGuida(xFiltroAggiuntivo As String, xOrderBy As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.GuidaImpostazioniDAL.LeggiSezioniGuida()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT * FROM Guida_Impostazioni_Sezioni")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiImpostazioni(xFiltroAggiuntivo As String, xOrderBy As String, objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.GuidaImpostazioniDAL.LeggiImpostazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT * FROM Guida_Impostazioni")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
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
