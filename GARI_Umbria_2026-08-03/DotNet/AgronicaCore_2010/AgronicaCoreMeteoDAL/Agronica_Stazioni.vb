

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Agronica_Stazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiDaCodificaFornitore(
        ByVal Fornitore_Cod As Integer,
        ByVal Stazione_Cod_Fornitore As String,
        ByVal Stazione_Cod_TabellaAnagrafica As String,
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

            Stb.AppendLine(" Select z.* ")
            Stb.AppendLine(" from Agronica_Stazioni z ")
            Stb.AppendLine(" where z.Fornitore_Cod =  " & Fornitore_Cod)
            Stb.AppendLine(" And z.Stazione_Cod_Fornitore = '" & Agro_SQL_SaveText(Stazione_Cod_Fornitore) & "' ")
            Stb.AppendLine(" And z.Stazione_Cod_TabellaAnagrafica = '" & Agro_SQL_SaveText(Stazione_Cod_TabellaAnagrafica) & "'")


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

    Public Function LeggiListaSensoriSuStazioniCod(xFiltroAggiuntivo As String, xOrderBy As String, objParametri As AgronicaCoreParametri) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" Select ns.sensorId, Stazione_Cod, ISNULL( system_time_zone_info, 'W. Europe Standard Time') as system_time_zone_info ")
            Stb.AppendLine(" From Agronica_Stazioni z ")
            Stb.AppendLine("  inner Join WiNet_Nodi_Sensori ns ")
            Stb.AppendLine("         On z.Stazione_Cod_Fornitore = ns.nodeId ")
            Stb.AppendLine(" where Stazione_Cod_TabellaAnagrafica = 'winet_nodi' ")
            Stb.AppendLine(" And z.AttivoPerScarico <> 0 ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" union ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" Select ns.sensorid, Stazione_Cod, ISNULL( system_time_zone_info, 'W. Europe Standard Time') as system_time_zone_info ")
            Stb.AppendLine(" From Agronica_Stazioni z ")
            Stb.AppendLine("  inner Join WiNet_Reti_Nodi rn ")
            Stb.AppendLine("         On z.Stazione_Cod_Fornitore = rn.netId ")
            Stb.AppendLine("  inner Join WiNet_Nodi_Sensori ns ")
            Stb.AppendLine("         On rn.nodeId = ns.nodeId ")
            Stb.AppendLine(" where Stazione_Cod_TabellaAnagrafica = 'winet_reti' ")
            Stb.AppendLine(" And z.AttivoPerScarico <> 0 ")
            Stb.AppendLine(" ")

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
