Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports AgronicaCoreEntityFramework_POCO
Imports System.Data.Entity
Imports System.Dynamic
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.baseClass

Public Class Gruppi_Raccolta_Read
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiGruppiRaccoltaValidi(
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal xOrderBy As String,
                        ByVal xFiltroAggiuntivo As String,
                        Optional dataAtt As Date = Nothing) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.Gruppi_Raccolta_Read.LeggiGruppiRaccoltaValidi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Gruppi_Raccolta")
            If dataAtt <> Nothing Then
                StrSQL.AppendLine("WHERE Validita_inizio <= " & Agro_SQL_SaveDate(dataAtt))
                StrSQL.AppendLine("AND Validita_Fine >= " & Agro_SQL_SaveDate(dataAtt))
                StrSQL.AppendLine("AND Validita_Inizio Is Not Null ")
            Else
                StrSQL.AppendLine("WHERE Validita_inizio >= " & Agro_SQL_SaveDate(AGRODATAINIZIO))
                StrSQL.AppendLine("AND Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE))
                StrSQL.AppendLine("AND Validita_Inizio Is Not Null ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy = "" Then
                StrSQL.AppendLine("ORDER BY GruppoRaccolta_Cod DESC ")
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

    Public Function LeggiGruppoRaccoltaImpresa(
                        ByVal piva As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByVal xFiltroAggiuntivo As String) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.GruppiRaccolta_Read.LeggiGruppoRaccoltaImpresa()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT gr.*")
            StrSQL.AppendLine("FROM Imprese imp")
            StrSQL.AppendLine("INNER JOIN Gruppi_Raccolta gr ON gr.GruppoRaccolta_Cod = imp.GruppoRaccolta_Cod")
            StrSQL.AppendLine("WHERE imp.PIVA = '" & Agro_SQL_SaveText(Trim(piva)) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function LeggiGruppiRaccolta(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.GruppiRaccolta_Read.LeggiGruppiRaccolta()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Gruppi_Raccolta")

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

    Public Function LeggiGruppoRaccolta(
                        ByVal cod As Integer,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeDAL.GruppiRaccolta_Read.LeggiGruppoRaccolta()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_Superuser obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Gruppi_Raccolta")
            StrSQL.AppendLine("WHERE GruppoRaccolta_Cod = '" & Agro_SQL_SaveNum(cod) & "'")

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

Public Class Gruppi_Raccolta_Write

End Class

