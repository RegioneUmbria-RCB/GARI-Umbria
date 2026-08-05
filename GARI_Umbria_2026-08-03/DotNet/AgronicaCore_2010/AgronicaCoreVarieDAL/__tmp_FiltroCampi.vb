Public Class __tmp_FiltroCampi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CancellaRecordDaIDTestataTemp(ByVal IDTestataTemp As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "CancellaRecordDaUsername()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            Stb.Length = 0
            Stb.AppendLine(" Delete  ")
            Stb.AppendLine(" From __tmp_FiltroCampi ")
            Stb.AppendLine(" Where IDTestataTemp = " & IDTestataTemp)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
