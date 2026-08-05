Public Class Credenziali_Irriframe_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################
    Public Function UserName_IF_from_SuperUser(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim UserName_IF As String = ""

        Dim DT = Leggi(objParametri.SuperUserUsername, objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            UserName_IF = DT.Rows(0).Item("UserName_IF")
        End If

        Return UserName_IF

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal UserName As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Credenziali_Irriframe_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Credenziali_Irriframe ")
            StrSQL.Append(" WHERE   1=1  ")

            If UserName <> "" Then
                StrSQL.Append(" AND UserName_GIAS = '" & Agro_SQL_SaveText(UserName) & "' ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class Credenziali_Irriframe_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal UserName_GIAS As String, ByVal UserName_IF As String, ByVal UserId_IF As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Credenziali_Irriframe_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Append("INSERT INTO Credenziali_Irriframe (UserName_GIAS, UserName_IF, UserId_IF) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(LCase(UserName_GIAS)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(LCase(UserName_IF)) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(UserId_IF) & " ")
            StrSQL.Append(")")

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