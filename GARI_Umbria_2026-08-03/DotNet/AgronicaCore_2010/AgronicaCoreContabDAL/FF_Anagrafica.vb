Imports AgronicaCoreDataProvider.UtilityProvider

Public Class FF_Anagrafica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' in risposta alla piva passata il campo "pivaRiferimento" indica quale p.iva ha le anagrafiche F&F, può essere: la piva passata, il padre oppure nessuno (in tal caso vale stringa vuota)
    ''' viene altresì restituito il padre della piva passata (se non radice, in tal caso il conteggio record è zero)
    ''' </summary>
    ''' <param name="Piva">Piva su cui effettuare la verifica</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiImpresaConParametriFFDaGerarchia(
        ByVal Piva As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  select  ")
            Stb.AppendLine("  case when otfiglio.piva is not null then  ")
            Stb.AppendLine("      otfiglio.piva ")
            Stb.AppendLine("  else  ")
            Stb.AppendLine("      case when otPadre.piva is not null then              ")
            Stb.AppendLine("          otPadre.piva ")
            Stb.AppendLine("      else ")
            Stb.AppendLine("          '' ")
            Stb.AppendLine("      end ")
            Stb.AppendLine("  end as pivaRiferimento ")
            Stb.AppendLine("  , gi.Padre ")
            Stb.AppendLine(" from gerarchiaImprese gi ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  left join ( ")
            Stb.AppendLine("      select distinct piva  ")
            Stb.AppendLine("      from OTabelle_Parametri ")
            Stb.AppendLine("  ) otPadre ")
            Stb.AppendLine(" on otPadre.Piva = gi.padre ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  left join ( ")
            Stb.AppendLine("      select distinct piva  ")
            Stb.AppendLine("      from OTabelle_Parametri ")
            Stb.AppendLine("  ) otfiglio ")
            Stb.AppendLine(" on otfiglio.Piva = gi.figlio ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" where gi.Figlio = '" & Agro_Sql_SaveText(Piva) & "'")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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
