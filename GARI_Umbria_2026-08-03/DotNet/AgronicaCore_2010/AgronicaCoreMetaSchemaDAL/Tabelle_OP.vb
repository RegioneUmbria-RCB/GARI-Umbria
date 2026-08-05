Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Tabelle_OP_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function UnioneOP_from_CodiceUnioneOP(ByVal CodiceUnioneOP As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Select Case CodiceUnioneOP

            Case "03"
                Return "UIAPOA"
            Case "04"
                Return "UNACOA"
            Case "05"
                Return "UNAPROA"
            Case "06"
                Return "UNAGRO"
            Case "07"
                Return "FEDAGRI"
            Case Else
                Return ""
        End Select

    End Function

    '##############################################################################################
    Public Function UnioneOPEstesa_from_CodiceUnioneOP(ByVal CodiceUnioneOP As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        Select Case CodiceUnioneOP

            Case "03"
                Return "UIAPOA - ASS.NE NAZ.LE COOPERATIVE AGROALIMENTARI"
            Case "04"
                Return "UNACOA"
            Case "05"
                Return "UNAPROA - UNIONE NAZ.LE ASS.PROD.ORTOFR.AGRUM.FR.GU"
            Case "06"
                Return "UNAGRO"
            Case "07"
                Return "Fedagri-Agri rete Service"
            Case Else
                Return ""
        End Select

    End Function

    '##############################################################################################
    Public Function AssociazioneIndutriale_from_CodiceAssociazioneIndutriale(ByVal CodiceAssociazioneIndutriale As String, _
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                            ) As String

        Select Case CodiceAssociazioneIndutriale

            Case "01"
                Return "ANICAV"
            Case "02"
                Return "AIIPA"
            Case "03"
                Return "ASSITRAPA"
            Case "04"
                Return "AGCI"
            Case "05"
                Return "CONFCOOPERATIVE"
            Case "06"
                Return "UNCI"
            Case "07"
                Return "ANCA LEGACOOP - ASSOCIAZIONE NAZIONALE COOPERATIVE AGROALIMENTARI"
            Case "08"
                Return "ANITAO"
            Case Else
                Return ""
        End Select

    End Function


    '###################################################################
    Public Function VegCodGias_from_SpecieCodOPGest(ByVal Specie_Cod_OPGest As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.VegCodGias_from_SpecieCodOPGest()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Veg_Cod As Integer = 0

        Try
            '---------------------------------------------


            DT = xDecoder_Specie_OP2007_Leggi(0, 0, "", _
                                                0, "", 0, "", _
                                                Specie_Cod_OPGest, _
                                                "", _
                                                "", _
                                                "", _
                                                objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

        Return Veg_Cod

    End Function

    '###################################################################
    Public Sub VegCodVegDes_from_SpecieCodOPGest(ByVal Specie_Cod_OPGest As String, _
                                                    ByRef Veg_Cod As Integer, _
                                                    ByRef Veg_Des As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    )

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.VegCodVegDes_from_SpecieCodOPGest()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Veg_Cod = 0
        Veg_Des = ""

        Try

            '---------------------------------------------

            DT = xDecoder_Specie_OP2007_Leggi(0, 0, "", _
                                                0, "", 0, "", _
                                                Specie_Cod_OPGest, _
                                                "", _
                                                "", _
                                                "", _
                                                objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Veg_Cod = DT.Rows(0).Item("Veg_Cod")
                Veg_Des = DT.Rows(0).Item("Veg_Des")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

    End Sub


    '###################################################################
    'codifiche OP_GEST
    'le codifiche sono valide sempre, non solo per il 2007
    Public Function xDecoder_Specie_OP2007_Leggi( _
                            ByVal Veg_Cod_ORACOLO As Integer, _
                            ByVal Veg_Cod As Integer, _
                            ByVal Veg_Des As String, _
                            ByVal GRVA_Cod_VEG As Int32, _
                            ByVal GRVA_Des As String, _
                            ByVal Grfi_Cod As Int32, _
                            ByVal Grfi_Des As String, _
                            ByVal Specie_Cod As Integer, _
                            ByVal Specie_Des As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.xDecoder_Specie_OP2007_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        'Veg_Cod_ORACOLO=0
        'Veg_Cod=0
        'Veg_Des=''
        'GRVA_Cod_VEG=0
        'GRVA_Des=''
        'Grfi_Cod=0
        'Grfi_Des=''
        'Specie_Cod=0
        'Specie_Des=''
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    xDecoder_Specie_OP2007 ")
            StrSQL.Append(" WHERE   1=1 ")

            StrSQL.Append(" AND      Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Veg_Cod_ORACOLO <> 0 Then
                StrSQL.Append(" AND Veg_Cod_ORACOLO = " & Agro_SQL_SaveNum(Veg_Cod_ORACOLO) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Veg_Des <> "" Then
                StrSQL.Append(" AND Veg_Des LIKE '" & Agro_SQL_SaveText(Veg_Des) & "'   ")
            End If

            If GRVA_Cod_VEG <> 0 Then
                StrSQL.Append(" AND GRVA_Cod_VEG = " & Agro_SQL_SaveNum(GRVA_Cod_VEG) & "   ")
            End If

            If GRVA_Des <> "" Then
                StrSQL.Append(" AND GRVA_Des LIKE '" & Agro_SQL_SaveText(GRVA_Des) & "'   ")
            End If

            If Grfi_Cod <> 0 Then
                StrSQL.Append(" AND Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & "   ")
            End If

            If Grfi_Des <> "" Then
                StrSQL.Append(" AND grfi_des LIKE '" & Agro_SQL_SaveText(Grfi_Des) & "'   ")
            End If

            If Specie_Cod <> 0 Then
                StrSQL.Append(" AND Specie_Cod = " & Agro_SQL_SaveNum(Specie_Cod) & "   ")
            End If

            If Specie_Des <> "" Then
                StrSQL.Append(" AND Specie_Des LIKE '" & Agro_SQL_SaveText(Specie_Des) & "'   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Specie_cod ASC")
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
    'codifiche OP_GEST
    'le codifiche sono valide sempre, non solo per il 2007
    Public Function xDecoder_Varieta_OP2007_Leggi( _
                            ByVal Cul_Cod_ORACOLO As Integer, _
                            ByVal Cul_Cod As Integer, _
                            ByVal Cul_Des As String, _
                            ByVal Specie_Cod As Integer, _
                            ByVal Specie_Des As String, _
                            ByVal Varieta_Cod As Integer, _
                            ByVal Varieta_Des As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.xDecoder_Varieta_OP2007_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        'Cul_Cod_ORACOLO=0
        'Cul_Cod=0
        'Cul_Des=''
        'Specie_Cod=0
        'Varieta_Cod=0
        'Varieta_Des=''
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    xDecoder_Varieta_OP2007 ")
            StrSQL.Append(" WHERE   1=1 ")

            StrSQL.Append(" AND     Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Cul_Cod_ORACOLO <> 0 Then
                StrSQL.Append(" AND Cul_Cod_ORACOLO = " & Agro_SQL_SaveNum(Cul_Cod_ORACOLO) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Cul_Des <> "" Then
                StrSQL.Append(" AND Cul_Des LIKE '" & Agro_SQL_SaveText(Cul_Des) & "'   ")
            End If

            If Specie_Cod <> 0 Then
                StrSQL.Append(" AND Specie_Cod = " & Agro_SQL_SaveNum(Specie_Cod) & "   ")
            End If

            If Specie_Des <> "" Then
                StrSQL.Append(" AND Specie_Des LIKE '" & Agro_SQL_SaveText(Specie_Des) & "'   ")
            End If

            If Varieta_Cod <> 0 Then
                StrSQL.Append(" AND Varieta_Cod = " & Agro_SQL_SaveNum(Varieta_Cod) & "   ")
            End If

            If Varieta_Des <> "" Then
                StrSQL.Append(" AND Varieta_Des LIKE '" & Agro_SQL_SaveText(Varieta_Des) & "'   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Specie_cod, Varieta_Cod ASC")
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
    'codifiche OP_GEST
    'le codifiche sono valide sempre, non solo per il 2007
    'IN join con la codifica delle specie 
    Public Function xDecoder_Varieta_OP2007_Leggi_2( _
                            ByVal Cul_Cod_ORACOLO As Integer, _
                            ByVal Cul_Cod As Integer, _
                            ByVal Cul_Des As String, _
                            ByVal Specie_Cod As Integer, _
                            ByVal Specie_Des As String, _
                            ByVal Varieta_Cod As Integer, _
                            ByVal Varieta_Des As String, _
                            ByVal Data As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.xDecoder_Varieta_OP2007_Leggi_2()"

        '====================================================================================
        'Parametri opzionali :
        'Cul_Cod_ORACOLO=0
        'Cul_Cod=0
        'Cul_Des=''
        'Specie_Cod=0
        'Varieta_Cod=0
        'Varieta_Des=''
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    xDecoder_Varieta_OP2007 INNER JOIN xDecoder_Specie_OP2007 ON xDecoder_Varieta_OP2007.Specie_Cod=xDecoder_Specie_OP2007.Specie_Cod")
            StrSQL.Append(" WHERE   1=1 ")

            StrSQL.Append(" AND     xDecoder_Varieta_OP2007.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND     xDecoder_Varieta_OP2007.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")

            If Cul_Cod_ORACOLO <> 0 Then
                StrSQL.Append(" AND Cul_Cod_ORACOLO = " & Agro_SQL_SaveNum(Cul_Cod_ORACOLO) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Cul_Des <> "" Then
                StrSQL.Append(" AND Cul_Des LIKE '" & Agro_SQL_SaveText(Cul_Des) & "'   ")
            End If

            If Specie_Cod <> 0 Then
                StrSQL.Append(" AND xDecoder_Varieta_OP2007.Specie_Cod = " & Agro_SQL_SaveNum(Specie_Cod) & "   ")
            End If

            If Specie_Des <> "" Then
                StrSQL.Append(" AND xDecoder_Specie_OP2007.Specie_Des LIKE '" & Agro_SQL_SaveText(Specie_Des) & "'   ")
            End If

            If Varieta_Cod <> 0 Then
                StrSQL.Append(" AND Varieta_Cod = " & Agro_SQL_SaveNum(Varieta_Cod) & "   ")
            End If

            If Varieta_Des <> "" Then
                StrSQL.Append(" AND Varieta_Des LIKE '" & Agro_SQL_SaveText(Varieta_Des) & "'   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY xDecoder_Varieta_OP2007.Specie_cod, xDecoder_Varieta_OP2007.Varieta_Cod ASC")
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
    Public Function CulCodGias_from_CodiciOPGest(ByVal Specie_Cod_OPGest As Integer, _
                                                    ByVal Varieta_Cod_OPGest As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.CulCodGias_from_CodiciOPGest()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Cul_Cod As Integer = 0

        Try
            '---------------------------------------------


            DT = xDecoder_Varieta_OP2007_Leggi(0, 0, "", _
                                                Specie_Cod_OPGest, _
                                                "", _
                                                Varieta_Cod_OPGest, _
                                                "", _
                                                "", "", _
                                                objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Cul_Cod = DT.Rows(0).Item("Cul_Cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing

        Return Cul_Cod

    End Function

    '###################################################################
    Public Sub CulCodCulDes_from_CodiciOPGest(ByVal Specie_Cod_OPGest As Integer, _
                                                    ByVal Varieta_Cod_OPGest As Integer, _
                                                    ByRef Cul_Cod As Integer, _
                                                    ByRef Cul_Des As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    )

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Tabelle_OP_R.CulCodCulDes_from_CodiciOPGest()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            DT = xDecoder_Varieta_OP2007_Leggi(0, 0, "", _
                                                Specie_Cod_OPGest, _
                                                "", _
                                                Varieta_Cod_OPGest, _
                                                "", _
                                                "", "", _
                                                objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Cul_Cod = DT.Rows(0).Item("Cul_Cod")
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



End Class

