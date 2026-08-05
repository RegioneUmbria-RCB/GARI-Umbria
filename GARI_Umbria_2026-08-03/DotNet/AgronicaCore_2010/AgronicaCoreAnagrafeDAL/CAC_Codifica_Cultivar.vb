Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class CAC_Codifica_Cultivar_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Cultivar_Coltiva As String,
                            ByVal Cultivar_Gias As Integer,
                            ByVal Veg_Cod_Gias As Integer,
                            ByVal Data_Modifica As Date,
                            ByVal Grfi_Cod As Integer,
                            ByVal Reg_Cod As Integer,
                            ByVal Grva_Cod As Integer,
                            ByVal Metodo_Produzione As Integer,
                            ByVal Cultivar_Coltiva_2 As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CAC_Codifica_Cultivar ")
            StrSQL.Append(" WHERE 1=1 ")

            If Cultivar_Coltiva <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "'   ")
            End If

            If Cultivar_Gias <> 0 Then
                StrSQL.Append(" AND Cultivar_Gias = " & Agro_SQL_SaveNum(Cultivar_Gias) & "   ")
            End If

            If Veg_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_Gias) & "   ")
            End If

            If Data_Modifica <> #1/1/1900# Then
                StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
            End If

            If Grva_Cod <> 0 Then
                StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
            End If

            If Metodo_Produzione <> 0 Then
                StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
            End If

            If Cultivar_Coltiva_2 <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva_2 = '" & Agro_SQL_SaveText(Cultivar_Coltiva_2) & "'   ")
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
    Public Sub VegCulCodGias_from_CulCodCliente(ByVal Cul_Cod_Cliente As String,
                                                ByRef Cul_Cod As Integer,
                                                ByRef Veg_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.VegCulCodGias_from_CulCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Cul_Cod = 0
        Veg_Cod = 0

        Try

            DT = Leggi(Cul_Cod_Cliente,
                         0, 0,
                         AGRODATAINIZIO,
                         0, 0, 0, 0, "",
                            "", "",
                            objParametri,
                            Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '###################################################################################
    Public Sub VegCulGrvaCodGias_from_CulCodCliente(ByVal Cul_Cod_Cliente As String,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.VegCulGrvaCodGias_from_CulCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Cul_Cod = 0
        Veg_Cod = 0
        Grva_Cod = 0

        Try

            DT = Leggi(Cul_Cod_Cliente,
                         0, 0,
                         AGRODATAINIZIO,
                         0, 0, 0, 0, "",
                            "", "",
                            objParametri,
                            Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
                Grva_Cod = DT.Rows(0).Item("Grva_Cod")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '###################################################################################
    Public Sub VegCulRegCodDesGias_from_CulCodCliente(ByVal Cul_Cod_Cliente As String,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Cul_Des As String,
                                                    ByRef Veg_Des As String,
                                                      ByRef Reg_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.VegCulRegCodDesGias_from_CulCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Cul_Cod = 0
        Veg_Cod = 0
        Cul_Des = ""
        Veg_Des = ""
        Reg_Cod = 0

        Try

            DT = Leggi_3(Cul_Cod_Cliente,
                         0, 0,
                         AGRODATAINIZIO,
                         0, 0, 0, 0, "",
                            "", "",
                            objParametri,
                            Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
                Cul_Des = DT.Rows(0).Item("Cul_Des")
                Veg_Des = DT.Rows(0).Item("Veg_Des")
                Reg_Cod = DT.Rows(0).Item("Reg_Cod")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '###################################################################################
    Public Sub VegCulGrvaCodGrfiCodGias_from_CulCodCliente(ByVal Cul_Cod_Cliente As String,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Veg_Cod As Integer,
                                                    ByRef Grva_Cod As Integer,
                                                    ByRef Grfi_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.VegCulGrvaCodGias_from_CulCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Cul_Cod = 0
        Veg_Cod = 0
        Grva_Cod = 0

        Try

            DT = Leggi(Cul_Cod_Cliente,
                         0, 0,
                         AGRODATAINIZIO,
                         0, 0, 0, 0, "",
                            "", "",
                            objParametri,
                            Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
                Grva_Cod = DT.Rows(0).Item("Grva_Cod")
                Grfi_Cod = DT.Rows(0).Item("Grfi_Cod")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '##############################################################################################
    'mette in join cultivar
    Public Function Leggi_2(ByVal Cultivar_Coltiva As String,
                            ByVal Cultivar_Gias As Integer,
                            ByVal Veg_Cod_Gias As Integer,
                            ByVal Data_Modifica As Date,
                            ByVal Grfi_Cod As Integer,
                            ByVal Reg_Cod As Integer,
                            ByVal Grva_Cod As Integer,
                            ByVal Metodo_Produzione As Integer,
                            ByVal Cultivar_Coltiva_2 As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.Leggi_2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT CAC_Codifica_Cultivar.*, cul_des ")
            StrSQL.Append(" FROM  CAC_Codifica_Cultivar ")
            StrSQL.Append(" INNER JOIN Cultivar ON Cultivar.cul_Cod = CAC_Codifica_Cultivar.cultivar_gias ")
            StrSQL.Append(" WHERE 1=1 ")

            If Cultivar_Coltiva <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "'   ")
            End If

            If Veg_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_Gias) & "   ")
            End If

            If Data_Modifica <> #1/1/1900# Then
                StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
            End If

            If Grva_Cod <> 0 Then
                StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
            End If

            If Metodo_Produzione <> 0 Then
                StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
            End If

            If Cultivar_Coltiva_2 <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva_2 = '" & Agro_SQL_SaveText(Cultivar_Coltiva_2) & "'   ")
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

    '##############################################################################################
    'mette in join cultivar e specievegetali
    Public Function Leggi_3(ByVal Cultivar_Coltiva As String,
                            ByVal Cultivar_Gias As Integer,
                            ByVal Veg_Cod_Gias As Integer,
                            ByVal Data_Modifica As Date,
                            ByVal Grfi_Cod As Integer,
                            ByVal Reg_Cod As Integer,
                            ByVal Grva_Cod As Integer,
                            ByVal Metodo_Produzione As Integer,
                            ByVal Cultivar_Coltiva_2 As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.Leggi_3()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT CAC_Codifica_Cultivar.*, cul_des, veg_des ")
            StrSQL.Append(" FROM  CAC_Codifica_Cultivar ")
            StrSQL.Append(" INNER JOIN Cultivar ON Cultivar.cul_Cod = CAC_Codifica_Cultivar.cultivar_gias  ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = CAC_Codifica_Cultivar.Veg_Cod_Gias  ")
            StrSQL.Append(" WHERE 1=1 ")

            If Cultivar_Coltiva <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "'   ")
            End If

            If Veg_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_Gias) & "   ")
            End If

            If Data_Modifica <> #1/1/1900# Then
                StrSQL.Append(" AND Data_Modifica = " & Agro_SQL_SaveDate(Data_Modifica) & "   ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
            End If

            If Reg_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Cod = " & Agro_SQL_SaveNum(Reg_Cod) & "   ")
            End If

            If Grva_Cod <> 0 Then
                StrSQL.Append(" AND Grva_Cod = " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
            End If

            If Metodo_Produzione <> 0 Then
                StrSQL.Append(" AND Metodo_Produzione = " & Agro_SQL_SaveNum(Metodo_Produzione) & "   ")
            End If

            If Cultivar_Coltiva_2 <> "" Then
                StrSQL.Append(" AND Cultivar_Coltiva_2 = '" & Agro_SQL_SaveText(Cultivar_Coltiva_2) & "'   ")
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
    '###################################################################
    Public Sub CulCodCulDes_from_CulCodCliente(ByVal Cul_Cod_Cliente As String,
                                                    ByRef Cul_Cod As Integer,
                                                    ByRef Cul_Des As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal Tipo_Codifica As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.CulCodCulDes_from_CulCodCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Cul_Cod = 0
        Cul_Des = ""

        Try
            '---------------------------------------------

            DT = Leggi_2(Cul_Cod_Cliente,
                   0, 0,
                   AGRODATAINIZIO,
                   0, 0, 0, 0, "",
                      "", "",
                      objParametri,
                      Tipo_Codifica)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
                Cul_Des = DT.Rows(0).Item("Cul_Des")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

    End Sub

    '##############################################################################################
    Public Function LeggiCodificaPomodoro(ByVal Cod_Varieta As String,
                            ByVal Cul_Cod_Gias As Integer,
                            ByVal Veg_Cod_Gias As Integer,
                            ByVal Grva_Cod_Gias As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Tipo_Codifica As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R.LeggiCodificaPomodoro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Codifica_Varieta_OIPomodorodaIndustriaNordItalia ")
            StrSQL.Append(" WHERE 1=1 ")

            If Cod_Varieta <> "" Then
                StrSQL.Append(" AND Cod_Varieta = '" & Agro_SQL_SaveText(Cod_Varieta) & "'   ")
            End If

            If Cul_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Cul_Cod_Gias = " & Agro_SQL_SaveNum(Cul_Cod_Gias) & "   ")
            End If

            If Veg_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Veg_Cod_GIAS = " & Agro_SQL_SaveNum(Veg_Cod_Gias) & "   ")
            End If

            If Grva_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Grva_Cod_Gias = " & Agro_SQL_SaveNum(Grva_Cod_Gias) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Desc_Varieta ")
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

    '###################################################################
    Public Function Leggi_Varieta(ByVal Veg_Cod_Coltiva As String,    '// CAMBIA QUI
 ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
 ByRef objParametri As AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar.Leggi_Varieta()"

        ' ------- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT CAC.ID, CAC.Cultivar_Coltiva as Chiave, CAC.Cultivar_Coltiva_2, CAC.Data_Modifica AS Data_Modifica, CAC.Descrizione AS Descrizione, ")

            StrSQL.AppendLine("   CAC.Cultivar_Coltiva, ")
            StrSQL.AppendLine("   CAC.Cultivar_Gias AS cul_cod, Cultivar.cul_des AS cul_des, ")
            StrSQL.AppendLine("   CAC.Veg_Cod_Gias AS veg_cod, SpecieVegetali.veg_des AS veg_des,   ")

            StrSQL.AppendLine("   ISNULL(CAC.grfi_cod,0) AS grfi_cod, ISNULL(GruppoFinalita.Grfi_Des,'Non Specificato') AS grfi_des, ")
            StrSQL.AppendLine("   ISNULL(CAC.Grva_Cod,0) AS Grva_Cod, ISNULL(GruppoVarietale.Grva_Des, 'Non Specificato') AS Grva_Des,   ")
            StrSQL.AppendLine("   ISNULL(CAC.Foral_Cod, 0) AS Foral_Cod, ISNULL(FormeAllevamento.Foral_Des, 'Nessuno') AS Foral_Des, ")
            StrSQL.AppendLine("   ISNULL(CAC.Port_Cod,0) AS Port_Cod, ISNULL(Portinnesti.Port_Des, 'Non Specificato') AS Port_Des,   ")
            StrSQL.AppendLine("   CAC.Reg_Cod AS Reg_Cod, Regolamenti.Reg_Des AS Reg_Des,    ")
            StrSQL.AppendLine("   CAC.Tipo_Codifica,    ")

            StrSQL.AppendLine("   CAC.Metodo_Produzione AS Metodo_Produzione_Cod, ")
            StrSQL.AppendLine("   CASE CAC.Metodo_Produzione WHEN 1 THEN 'Integrato' ")
            StrSQL.AppendLine(" WHEN 2 THEN 'In Conversione'    ")
            StrSQL.AppendLine(" WHEN 3 THEN 'Biologico'     ")
            StrSQL.AppendLine(" ELSE ''   ")
            StrSQL.AppendLine(" END AS Metodo_Produzione_Des    ")

            StrSQL.AppendLine("FROM CAC_Codifica_Cultivar CAC   ")

            'Join su Cultivar per cul_des   
            StrSQL.AppendLine("LEFT JOIN Cultivar    ")
            StrSQL.AppendLine("ON CAC.Cultivar_Gias = Cultivar.cul_cod ")

            'Join su SpecieVegetali per veg_des
            StrSQL.AppendLine("LEFT JOIN SpecieVegetali     ")
            StrSQL.AppendLine("ON CAC.Veg_Cod_Gias = SpecieVegetali.veg_cod   ")

            'Join su GruppoFinalita per grifi_des
            StrSQL.AppendLine("LEFT JOIN GruppoFinalita     ")
            StrSQL.AppendLine("ON CAC.Grfi_Cod = GruppoFinalita.grfi_cod   ")

            'Join su GruppoVarietale per Grva_Des
            StrSQL.AppendLine("LEFT JOIN GruppoVarietale    ")
            StrSQL.AppendLine("ON CAC.Grva_Cod = GruppoVarietale.Grva_Cod  ")

            'Join su FormeAllevamento per Foral_Des
            StrSQL.AppendLine("LEFT JOIN FormeAllevamento  ")
            StrSQL.AppendLine("ON CAC.Foral_Cod = FormeAllevamento.Foral_Cod ")

            'Join su Portinnesti per Port_Des
            StrSQL.AppendLine("LEFT JOIN Portinnesti   ")
            StrSQL.AppendLine("ON CAC.Port_Cod = Portinnesti.Port_Cod ")

            'Join su Regolamenti per Reg_Des
            StrSQL.AppendLine("LEFT JOIN Regolamenti   ")
            StrSQL.AppendLine("ON CAC.Reg_Cod = Regolamenti.Reg_Cod   ")

            'If Not String.IsNullOrEmpty(Veg_Cod_Coltiva) Then
            '    StrSQL.AppendLineLine(" WHERE CAC.Veg_Cod_Coltiva = '" + Agro_SQL_SaveText(Veg_Cod_Coltiva) & "' ")
            'End If

            If Tipo_Codifica <> -1 Then
                StrSQL.AppendLine(" WHERE (CAC.Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & ")   ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            Return DT

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Function

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CAC_Codifica_Cultivar_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Cultivar_Coltiva As String,
                           ByVal Cultivar_Gias As Integer _
                            , ByVal Veg_Cod_Gias As Integer _
                            , ByVal Grfi_Cod As Integer _
                            , ByVal Reg_Cod As Integer _
                            , ByVal Grva_Cod As Integer _
                            , ByVal Metodo_Produzione As Integer _
                            , ByVal Descrizione As String _
                            , ByVal Cultivar_Coltiva_2 As String _
                            , ByVal Data_Modifica As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_Cultivar ")
            StrSQL.Append(" ( ")
            StrSQL.Append("  [Cultivar_Coltiva] " & vbCrLf)
            StrSQL.Append("  ,[Cultivar_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Grfi_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Reg_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Grva_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Metodo_Produzione] " & vbCrLf)
            StrSQL.Append("  ,[Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Cultivar_Coltiva_2] " & vbCrLf)
            StrSQL.Append("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.Append("  ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "' " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Cultivar_Gias) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Veg_Cod_Gias) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grfi_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Reg_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Grva_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Metodo_Produzione) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cultivar_Coltiva_2) & "'" & vbCrLf)
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

    Public Function Scrivi_Varieta(ByVal Cultivar_Coltiva As String _
                                   , ByVal Cultivar_Gias As Integer _
                                   , ByVal Veg_Cod_Gias As Integer _
                                   , ByVal Descrizione As String _
                                   , ByVal Grfi_Cod As Integer _
                                   , ByVal Reg_Cod As Integer _
                                   , ByVal Grva_Cod As Integer _
                                   , ByVal Metodo_Produzione As Integer _
                                   , ByVal Cultivar_Coltiva_2 As String _
                                   , ByVal Port_Cod As Integer _
                                   , ByVal Foral_Cod As Integer _
                                   , ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_W.Scrivi_Varieta()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO CAC_Codifica_Cultivar ")
            StrSQL.AppendLine("  ( ")
            StrSQL.AppendLine("    [Cultivar_Coltiva] ")
            StrSQL.AppendLine("  , [Cultivar_Gias] ")
            StrSQL.AppendLine("  , [Veg_Cod_Gias] ")
            StrSQL.AppendLine("  , [Descrizione] ")
            StrSQL.AppendLine("  , [Data_Modifica] ")
            StrSQL.AppendLine("  , [Grfi_Cod] ")
            StrSQL.AppendLine("  , [Reg_Cod] ")
            StrSQL.AppendLine("  , [Grva_Cod] ")
            StrSQL.AppendLine("  , [Metodo_Produzione] ")
            StrSQL.AppendLine("  , [Cultivar_Coltiva_2] ")
            StrSQL.AppendLine("  , [Port_Cod] ")
            StrSQL.AppendLine("  , [Foral_Cod] ")
            StrSQL.AppendLine("  , [Tipo_Codifica] ")
            StrSQL.AppendLine("   ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "'")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Cultivar_Gias) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Veg_Cod_Gias) & " ")
            StrSQL.AppendLine(", '" & Agro_SQL_SaveText(Descrizione) & "'")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Reg_Cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Grva_Cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Metodo_Produzione) & " ")
            StrSQL.AppendLine(", '" & Agro_SQL_SaveText(Cultivar_Coltiva_2) & "'")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Port_Cod) & "")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Foral_Cod) & "")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Tipo_Codifica) & "")
            StrSQL.AppendLine(") ")

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

    Public Function Modifica_Varieta(ByVal ID As Integer,
                                    ByVal Cultivar_Coltiva As String,
                                     ByVal Cultivar_Coltiva_Old As String,
                                     ByVal Cultivar_Gias As Integer,
                                     ByVal Veg_Cod_Gias As Integer,
                                     ByVal Descrizione As String,
                                     ByVal Grfi_Cod As Integer,
                                     ByVal Reg_Cod As Integer,
                                     ByVal Grva_Cod As Integer,
                                     ByVal Metodo_Produzione As Integer,
                                     ByVal Cultivar_Coltiva_2 As String,
                                     ByVal Port_Cod As Integer,
                                     ByVal Foral_Cod As Integer,
                                     ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_Varieta,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_W.Modifica_Varieta()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0


            strSql.AppendLine("UPDATE CAC_Codifica_Cultivar SET ")

            strSql.AppendLine("   Cultivar_Coltiva           = '" + Agro_SQL_SaveText(Trim(Cultivar_Coltiva)) + "' ")
            strSql.AppendLine(" , Cultivar_Gias              = " + Agro_SQL_SaveNum(Trim(Cultivar_Gias)) + " ")
            strSql.AppendLine(" , Veg_Cod_Gias              = " + Agro_SQL_SaveNum(Trim(Veg_Cod_Gias)) + " ")

            strSql.AppendLine(" , Descrizione               = '" + Agro_SQL_SaveText(Trim(Descrizione)) + "' ")

            strSql.AppendLine(" , Data_Modifica             = " + Agro_SQL_SaveDateTime(Date.Now))

            strSql.AppendLine(" , Grfi_Cod                  = " + Agro_SQL_SaveNum(Trim(Grfi_Cod)) + " ")
            strSql.AppendLine(" , Reg_Cod                   = " + Agro_SQL_SaveNum(Trim(Reg_Cod)) + " ")
            strSql.AppendLine(" , Grva_Cod                  = " + Agro_SQL_SaveNum(Trim(Grva_Cod)) + " ")
            strSql.AppendLine(" , Metodo_Produzione         = " + Agro_SQL_SaveNum(Trim(Metodo_Produzione)) + " ")

            strSql.AppendLine(" , Cultivar_Coltiva_2         = '" + Agro_SQL_SaveText(Trim(Cultivar_Coltiva_2)) + "' ")

            strSql.AppendLine(" , Port_Cod                  =  " + Agro_SQL_SaveNum(Trim(Port_Cod)) + " ")
            strSql.AppendLine(" , Foral_Cod                 =  " + Agro_SQL_SaveNum(Trim(Foral_Cod)) + " ")
            strSql.AppendLine(" , Tipo_Codifica             =  " + Agro_SQL_SaveNum(Trim(Tipo_Codifica)) + " ")

            strSql.AppendLine(" WHERE   Cultivar_Coltiva     = '" + Agro_SQL_SaveText(Trim(Cultivar_Coltiva_Old)) + "'  ")
            strSql.AppendLine(" AND   Tipo_Codifica     =  " + Agro_SQL_SaveNum(Trim(Tipo_Codifica)) + " ")
            strSql.AppendLine(" AND   ID     =  " + Agro_SQL_SaveNum(ID) + " ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Cancella_Varieta(ByVal ID As Integer,
                                     ByVal Cultivar_Coltiva As String,
                                     ByVal Tipo_Codifica As enum_Tipo_CAC_Codifica_ProdottiAziendali,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_W.Cancella_Varieta()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            '---------------------------------------------

            strSql.Length = 0
            strSql.AppendLine(" DELETE ")
            strSql.AppendLine(" FROM    CAC_Codifica_Cultivar ")
            strSql.AppendLine(" WHERE   (Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cultivar_Coltiva) & "')  ")

            If Tipo_Codifica <> 0 Then
                strSql.AppendLine(" AND     (Tipo_Codifica = " & Agro_SQL_SaveNum(Tipo_Codifica) & ")   ")
            End If

            If ID <> 0 Then
                strSql.AppendLine(" AND     (ID = " & Agro_SQL_SaveNum(ID) & ")   ")
            End If

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function
End Class