Public Class qryVarieDAL
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function VerificaEsistenzaCampo(ByVal campo As String, ByVal tabella As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "CancellaVecchiRecordPerDataCreazione()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try



            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Select 1 from sys.tables tt  ")
            Stb.AppendLine(" inner Join sys.columns cc on tt.object_id = cc.object_id  ")
            Stb.AppendLine(" where tt.name = '" & Agro_SQL_SaveText(tabella) & "'  ")
            Stb.AppendLine(" And cc.name = '" & Agro_SQL_SaveText(campo) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                xRisp = True
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function
End Class
