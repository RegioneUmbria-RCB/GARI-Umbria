Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Blocco_Particelle_R
    Inherits DataProvider

    ''' <summary>
    ''' Alias di tabella usati: bp per UMA_Blocco_Particelle
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="Anno"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri">Vengono aggiunti filtri su piva_superuser e visibilità</param>
    ''' <returns></returns>
    Public Function Leggi(
            ByVal piva As String,
            ByVal Anno As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Allevamenti_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT ROW_NUMBER() OVER(ORDER BY bp.Programmazione_Cod DESC) AS ID, ")
            strSql.AppendLine(" bp.*, p.PROVINCIA, c.Descrizione As COMUNE, CASE WHEN Bloccato = 0 THEN 'Attivo' ELSE 'Bloccato' END As Bloccato_Des, ")

            If Anno < 2025 Then
                strSql.AppendLine(" pt.Programmazione_Des, ")
            Else
                strSql.AppendLine(" 'Piano Colturale' As Programmazione_Des, ")
            End If

            strSql.AppendLine(" m.macrouso_UMA_Des FROM UMA_Blocco_Particelle bp ")
            strSql.AppendLine(" JOIN Lista_Province p on p.PROV = bp.PROV ")
            strSql.AppendLine(" JOIN ISTAT_Comuni c on c.Pro_Cod_Istat = bp.PROV AND c.Com_Cod_Istat = bp.COM ")
            If Anno < 2025 Then
                strSql.AppendLine(" JOIN Programmazione_Testata pt on pt.Programmazione_Cod = bp.Programmazione_Cod ")
            End If
            strSql.AppendLine(" JOIN (Select DISTINCT Macrouso_UMA_COD, Macrouso_UMA_Des From Codifica_SpecieVegetali_Agea_2015_2020) m on m.Macrouso_UMA_Cod = bp.Gruppo_Colturale_UMA ")
            strSql.AppendLine(" WHERE bp.anno = " & Agro_SQL_SaveNum(Anno) & " ")

            If piva <> "" Then
                strSql.AppendLine("AND bp.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine("AND bp.Piva_SuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   bp.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   bp.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Programmazione_Cod DESC, Prov ASC, com ASC, macrouso_UMA_Cod DESC ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

End Class

Public Class UMA_Blocco_Particelle_W
    Inherits DataProvider

    Public Function Aggiungi(
            ByVal listaInsert As List(Of UMA_Blocco_Particelle),
            ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Blocco_Particelle_W.Aggiungi()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each ins In listaInsert
                    GiasContext.UMA_Blocco_Particelle.Add(ins)
                Next

                GiasContext.SaveChanges()
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato
    End Function

    Public Function Aggiorna(
            ByVal UpdateArray As ArrayList,
            ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Aggiorna_Lavorazioni()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each up As UMA_Blocco_Particelle In UpdateArray
                    GiasContext.UMA_Blocco_Particelle.Attach(up)
                    GiasContext.Entry(up).State = EntityState.Modified
                Next

                GiasContext.SaveChanges()
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato
    End Function


End Class