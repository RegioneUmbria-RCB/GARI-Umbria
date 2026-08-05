Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Utenti_Validita_Token_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Validita_Token_R.Leggi_()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Validita_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & objParametri.SuperUserUsername & "'")

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

    Public Function Leggi_Validita(ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Validita_Token_R.Leggi_Validita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Validita As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Validita_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & objParametri.SuperUserUsername & "'")
            StrSQL.Append(" AND Utente_Username ='" & objParametri.UtenteUsername & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Validita = DT.Rows(0).Item("Validita_Minuti")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Validita

    End Function

    Public Function Leggi_Impostazioni_Abusi_AWS( _
                                ByVal SuperUserUsername As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Validita_Token_R.Leggi_Validita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Validita As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Validita_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & Agro_SQL_SaveText(SuperUserUsername) & "'")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Return DT

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return DT

    End Function


End Class
Public Class Utenti_Validita_Token_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal SuperUser_Username As String, _
                           ByVal Utente_Username As String, _
                           ByVal Validita_Minuti As Int32, _
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Validita_Token_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Utenti_Validita_Token (SuperUser_Username, ")
            StrSQL.Append("                    Utente_Username, ")
            StrSQL.Append("                    Validita_Minuti, ")

            StrSQL.Append("                    inviato,  ")
            StrSQL.Append("                    datainvio ")
            StrSQL.Append("                    ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(SuperUser_Username) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Utente_Username) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Validita_Minuti) & " ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append(")")
            '---------------------------------------------

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
