Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class CAC_Codifica_FormeAllevamento_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Foral_Cod_Cliente As String, _
                            ByVal Foral_Cod_Gias As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Optional ByVal Tipo_Codifica As Integer = 0 _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CAC_Codifica_FormeAllevamento ")
            StrSQL.Append(" WHERE 1=1 ")

            If Foral_Cod_Cliente <> "" Then
                StrSQL.Append(" AND Foral_Cod_Cliente = '" & Agro_SQL_SaveText(Foral_Cod_Cliente) & "' ")
            End If

            If Foral_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Foral_Cod_Gias = " & Agro_SQL_SaveNum(Foral_Cod_Gias) & " ")
            End If

            If Tipo_Codifica <> 0 Then
                StrSQL.Append(" AND Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descrizione ")
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


    '###################################################################################
    Public Function ForalCodGias_from_ForalCodCliente(ByVal Foral_Cod_Cliente As String, _
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                        Optional ByVal Tipo_Codifica As Integer = 0 _
                                                        ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_R.ForalCodGias_from_ForalCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Foral_Cod_Gias As Integer = -1

        Try

            DT = Leggi(Foral_Cod_Cliente, _
                        0, _
                        "", "", _
                        objParametri, _
                        Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Foral_Cod_Gias = DT.Rows(0).Item("Foral_Cod_Gias")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Foral_Cod_Gias

    End Function










End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CAC_Codifica_FormeAllevamento_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Descrizione As String _
                            , ByVal Foral_Cod_Cliente As String _
                            , ByVal Foral_Cod_Gias As Integer _
                            , ByVal Piva_SuperUser As String _
                            , ByVal Data_Modifica As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_FormeAllevamento ")
            StrSQL.Append(" ( ")
            StrSQL.Append("  [Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Foral_Cod_Cliente] " & vbCrLf)
            StrSQL.Append("  ,[Foral_Cod_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Piva_SuperUser] " & vbCrLf)
            StrSQL.Append("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.Append("  ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Foral_Cod_Cliente) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Foral_Cod_Gias) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva_SuperUser) & "'" & vbCrLf)
            StrSQL.Append("," & Agro_SQL_SaveDate(Data_Modifica) & "" & vbCrLf)
            StrSQL.Append(") ")

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