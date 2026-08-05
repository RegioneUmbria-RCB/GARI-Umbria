Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class PDC_PuntoDiPrelievo_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal ID_PuntoDiPrelievo As Int32, _
                          ByVal TipoPrelievo As Int32,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_PuntoDiPrelievo_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   TipoPrelievo  1 -- acquisti  2 -- campione
         
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT     *   ")
            StrSQL.AppendLine(" FROM         PDC_PuntoDiPrelievo  ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If ID_PuntoDiPrelievo <> 0 Then
                StrSQL.AppendLine(" AND PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo = " & Agro_SQL_SaveNum(ID_PuntoDiPrelievo))
            End If

            If TipoPrelievo <> 0 Then
                StrSQL.AppendLine(" AND PDC_PuntoDiPrelievo.TipoPrelievo = " & Agro_SQL_SaveNum(TipoPrelievo))
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

