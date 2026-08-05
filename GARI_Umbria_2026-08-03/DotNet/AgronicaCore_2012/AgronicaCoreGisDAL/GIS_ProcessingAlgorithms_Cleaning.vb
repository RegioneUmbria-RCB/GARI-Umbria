Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis
Public Class GIS_ProcessingAlgorithms_Cleaning_R
    Inherits DataProvider2010

    Public Function LeggiDatiDaAggiornare(ByVal algCode As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_R.LeggiDatiDaAggiornare()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append(" Select ")
            stb.Append("       ChiaveGrafica, rr.Algoritmo, PropostaModifica_WKT,ConfigurazioneEsecuzione, Poligono_Duplicato ")
            stb.Append("            From [GIS_ProcessingAlgorithms_Cleaning_Risultati] rr ")
            stb.Append("                inner Join [dbo].[GIS_ProcessingAlgorithms_Cleaning_Avanzamento] z ")
            stb.Append("                    On rr.Algoritmo = z.Algoritmo  ")
            stb.Append(" where z.Algoritmo = '" & Agro_SQL_SaveText(algCode) & "' ")
            stb.Append(" And z.inviato = 0 ")
            stb.Append(" And rr.inviato = 0  ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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

    Public Function LeggiAlgoritmoDaApplicare(ByVal algCode As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_R.LeggiAlgoritmoDaApplicare()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append(" Select ")
            stb.Append("       isnull( z.algoritmo + ' [' + Convert(varchar(10), min(z.DataOraUltimoPassaggio), 3) + ' - ' + CONVERT(varchar(10),min(z.DataOraUltimoPassaggio),8)  + ']' , '') as LayerDescr, ")
            stb.Append("       z.algoritmo as algoritmo_fase ")
            stb.Append("            From [GIS_ProcessingAlgorithms_Cleaning_Risultati] rr ")
            stb.Append("                inner Join [dbo].[GIS_ProcessingAlgorithms_Cleaning_Avanzamento] z ")
            stb.Append("                    On rr.Algoritmo = z.Algoritmo  ")
            stb.Append("                Left Join [dbo].[GIS_ProcessingAlgorithms_Cleaning_Algorithm] z1 ")
            stb.Append("                                    On z.AlgoritmoPadre = z1.Algoritmo ")
            stb.Append(" where z1.Algoritmo = '" & Agro_SQL_SaveText(algCode) & "' ")
            stb.Append(" And z1.inviato = 0 ")
            stb.Append(" And rr.inviato = 0  ")
            stb.Append(" group by z.Algoritmo  ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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

Public Class GIS_ProcessingAlgorithms_Cleaning_W
    Inherits DataProvider2010

    Public Function AggiornaStato(ByVal AlgCode As String,
                                  ByVal stato As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStato"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSql.Length = 0

            StrSql.AppendLine("UPDATE GIS_ProcessingAlgorithms_Cleaning_Algorithm")
            StrSql.AppendLine(String.Format("SET inviato = {0}", Agro_SQL_SaveNum(stato)))
            StrSql.AppendLine(String.Format("   ,Data_Modifica = {0}", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSql.AppendLine(String.Format("   ,Username_Modifica = '{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine("WHERE 1 = 1")
            StrSql.AppendLine(String.Format("    AND Algoritmo = '{0}'", Agro_SQL_SaveText(AlgCode)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function AggiornaStatoFase(ByVal AlgCode As String,
                                  ByVal stato As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStatoFase"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSql.Length = 0

            StrSql.AppendLine("UPDATE GIS_ProcessingAlgorithms_Cleaning_Avanzamento")
            StrSql.AppendLine(String.Format("SET inviato = {0}", Agro_SQL_SaveNum(stato)))
            StrSql.AppendLine(String.Format("   ,Data_Modifica = {0}", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSql.AppendLine(String.Format("   ,Username_Modifica = '{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine("WHERE 1 = 1")
            StrSql.AppendLine(String.Format("    AND Algoritmo = '{0}'", Agro_SQL_SaveText(AlgCode)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function AggiornaStatoDettaglio(ByVal ChiaveGrafica As String,
                                           ByVal AlgCode As String,
                                           ByVal stato As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStatoDettaglio"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            StrSql.Length = 0

            StrSql.AppendLine("UPDATE GIS_ProcessingAlgorithms_Cleaning_Risultati")
            StrSql.AppendLine(String.Format("SET inviato = {0}", Agro_SQL_SaveNum(stato)))
            StrSql.AppendLine(String.Format("   ,Data_Modifica = {0}", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSql.AppendLine(String.Format("   ,Username_Modifica = '{0}'", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine("WHERE 1 = 1")
            StrSql.AppendLine(String.Format("    AND ChiaveGrafica = '{0}'", Agro_SQL_SaveText(ChiaveGrafica)))
            StrSql.AppendLine(String.Format("    AND Algoritmo = '{0}'", Agro_SQL_SaveText(AlgCode)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

End Class
