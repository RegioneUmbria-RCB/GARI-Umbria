Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class CAC_Codifica_Animali_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Cod_Cliente As String, _
                            ByVal Gen_Cod_Gias As Integer, _
                            ByVal Spe_Cod_Gias As Integer, _
                            ByVal Ipro_Cod_Gias As Integer, _
                            ByVal Cat_Cod_Gias As Integer, _
                            ByVal Raz_Cod_Gias As Integer, _
                            ByVal Descrizione As String, _
                            ByVal Sesso As String, _
                            ByVal Metodo_Produzione As Integer, _
                            ByVal Reg_Cod As Integer, _
                            ByVal Data_Modifica As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Animali_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CAC_Codifica_Animali ")
            StrSQL.Append(" WHERE 1=1 ")

            If Cod_Cliente <> "" Then
                StrSQL.Append(" AND Cod_Cliente = '" & Agro_SQL_SaveText(Cod_Cliente) & "'   ")
            End If

            If Gen_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Gen_Cod_Gias = " & Agro_SQL_SaveNum(Gen_Cod_Gias) & "   ")
            End If

            If Spe_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Spe_Cod_Gias = " & Agro_SQL_SaveNum(Spe_Cod_Gias) & "   ")
            End If

            If Ipro_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Ipro_Cod_Gias = " & Agro_SQL_SaveNum(Ipro_Cod_Gias) & "   ")
            End If

            If Cat_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Cat_Cod_Gias = " & Agro_SQL_SaveNum(Cat_Cod_Gias) & "   ")
            End If

            If Raz_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Raz_Cod_Gias = " & Agro_SQL_SaveNum(Raz_Cod_Gias) & "   ")
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND Descrizione LIKE '%" & Agro_SQL_SaveText(Descrizione) & "%'   ")
            End If

            If Sesso <> "" Then
                StrSQL.Append(" AND Sesso = '" & Agro_SQL_SaveText(Sesso) & "'   ")
            End If

            If Metodo_Produzione <> 0 Then
                StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
            End If

            If Data_Modifica <> #1/1/1900# Then
                StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
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
    Public Sub GenSpeIproCatRazCodGias_from_CodCliente(ByVal Cod_Cliente As String,
                                                        ByRef Gen_Cod As Integer,
                                                        ByRef Spe_Cod As Integer,
                                                        ByRef Ipro_Cod As Integer,
                                                        ByRef Cat_Cod As Integer,
                                                        ByRef Raz_Cod As Integer,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.GenSpeIproCatRazCodGias_from_CodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Gen_Cod = -1
            Spe_Cod = -1
            Ipro_Cod = -1
            Cat_Cod = -1
            Raz_Cod = -1

            DT = Leggi(Cod_Cliente,
                          0, 0, 0, 0, 0,
                          "", "",
                          0, 0,
                         AGRODATAINIZIO,
                         "", "",
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Gen_Cod = DT.Rows(0).Item("Gen_Cod")
                Spe_Cod = DT.Rows(0).Item("Spe_Cod")
                Ipro_Cod = DT.Rows(0).Item("Ipro_Cod")
                Cat_Cod = DT.Rows(0).Item("Cat_Cod")
                Raz_Cod = DT.Rows(0).Item("Raz_Cod")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

End Class
