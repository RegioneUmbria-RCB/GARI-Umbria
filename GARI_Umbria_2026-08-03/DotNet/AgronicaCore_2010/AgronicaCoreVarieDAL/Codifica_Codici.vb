
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Codifica_Codici
    Inherits AgronicaCoreDataProvider.DataProvider



    '################################################################################
    'Public Function ConvertiZona(ByVal Zona_Cod_Cliente As String,
    '                             ByVal FinestraTemp_Inizio As Date,
    '                             ByVal FinestraTemp_Fine As Date,
    '                             ByRef objConnessione As DbConnection,
    '                             ByVal StringaConnessione As String,
    '                             ByVal FlagVisibilita As Int32,
    '                             ByVal DirectoryLOG As String,
    '                             ByVal FileLOG As String,
    '                             ByVal IdentificatoreUtente As String
    '                             ) As Integer

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiZona()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim Zona_Cod As Integer = 0

    '    Try
    '        '---------------------------------------------


    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * FROM  CAC_Codifica_Zone ")
    '        StrSQL.Append(" WHERE Zona_Cod_Cliente = '" & Agro_SQL_SaveText(Zona_Cod_Cliente) & "'")
    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        If Not DT Is Nothing Then
    '            If DT.Rows.Count > 0 Then
    '                Zona_Cod = DT.Rows(0).Item("Zona_Cod_Gias")
    '            Else
    '                Zona_Cod = 0
    '            End If
    '        Else
    '            Zona_Cod = 0
    '        End If

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Zona_Cod = 0
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing
    '    Return Zona_Cod

    'End Function


    Public Function ConvertiZona(ByVal Zona_Cod_Cliente As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiZona()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Zona_Cod As Integer = 0

        Try
            '---------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Zone ")
            StrSQL.Append(" WHERE Zona_Cod_Cliente = '" & Agro_SQL_SaveText(Zona_Cod_Cliente) & "'")
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    Zona_Cod = DT.Rows(0).Item("Zona_Cod_Gias")
                Else
                    Zona_Cod = 0
                End If
            Else
                Zona_Cod = 0
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Zona_Cod = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing
        Return Zona_Cod

    End Function


    '################################################################################
    'Public Sub ConvertiVarieta(ByVal Cul_Cod_Cliente As String,
    '                           ByRef Cul_Cod As Integer,
    '                           ByRef Veg_Cod As Integer,
    '                           ByVal FinestraTemp_Inizio As Date,
    '                           ByVal FinestraTemp_Fine As Date,
    '                           ByRef objConnessione As DbConnection,
    '                           ByVal StringaConnessione As String,
    '                           ByVal FlagVisibilita As Int32,
    '                           ByVal DirectoryLOG As String,
    '                           ByVal FileLOG As String,
    '                           ByVal IdentificatoreUtente As String)

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiVarieta()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * FROM  CAC_Codifica_Cultivar ")
    '        StrSQL.Append(" WHERE Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "'")

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        Cul_Cod = 0
    '        Veg_Cod = 0
    '        If Not DT Is Nothing Then

    '            If DT.Rows.Count > 0 Then
    '                If DT.Rows.Count > 0 Then
    '                    Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
    '                    Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing



    'End Sub



    Public Sub ConvertiVarieta(ByVal Cul_Cod_Cliente As String, _
                               ByRef Cul_Cod As Integer, _
                               ByRef Veg_Cod As Integer, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    )

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiVarieta()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Cultivar ")
            StrSQL.Append(" WHERE Cultivar_Coltiva = '" & Agro_SQL_SaveText(Cul_Cod_Cliente) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Cul_Cod = 0
            Veg_Cod = 0
            If Not DT Is Nothing Then

                If DT.Rows.Count > 0 Then
                    If DT.Rows.Count > 0 Then
                        Cul_Cod = DT.Rows(0).Item("Cultivar_Gias")
                        Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                    End If
                End If
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing



    End Sub


    '################################################################################
    'Public Sub ConvertiSpecie(ByVal Veg_Cod_Cliente As String,
    '                          ByRef Veg_Cod As Integer,
    '                          ByRef Id_Cod As Integer,
    '                          ByRef Grfi_Cod As Integer,
    '                          ByRef Grva_Cod As Integer,
    '                          ByRef Reg_Cod As Integer,
    '                          ByRef Metodo_Produzione As Integer,
    '                          ByVal FinestraTemp_Inizio As Date,
    '                          ByVal FinestraTemp_Fine As Date,
    '                          ByRef objConnessione As DbConnection,
    '                          ByVal StringaConnessione As String,
    '                          ByVal FlagVisibilita As Int32,
    '                          ByVal DirectoryLOG As String,
    '                          ByVal FileLOG As String,
    '                          ByVal IdentificatoreUtente As String)

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiSpecie()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * FROM  CAC_Codifica_Veg_Cod ")
    '        StrSQL.Append(" WHERE Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "'")

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        Veg_Cod = 0
    '        Id_Cod = 0
    '        Grfi_Cod = 0
    '        Grva_Cod = 0
    '        Reg_Cod = 0
    '        Metodo_Produzione = 0

    '        If Not DT Is Nothing Then
    '            If DT.Rows.Count > 0 Then
    '                Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
    '                Id_Cod = DT.Rows(0).Item("Id_Cod")
    '                Grfi_Cod = DT.Rows(0).Item("Grfi_Cod")
    '                Grva_Cod = DT.Rows(0).Item("Grva_Cod")
    '                Reg_Cod = DT.Rows(0).Item("Reg_Cod")
    '                Metodo_Produzione = DT.Rows(0).Item("Metodo_Produzione")

    '            End If
    '        End If


    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing

    'End Sub



    Public Sub ConvertiSpecie(ByVal Veg_Cod_Cliente As String, _
                              ByRef Veg_Cod As Integer, _
                              ByRef Id_Cod As Integer, _
                              ByRef Grfi_Cod As Integer, _
                              ByRef Grva_Cod As Integer, _
                              ByRef Reg_Cod As Integer, _
                              ByRef Metodo_Produzione As Integer, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    )

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiSpecie()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Veg_Cod ")
            StrSQL.Append(" WHERE Veg_Cod_Coltiva = '" & Agro_SQL_SaveText(Veg_Cod_Cliente) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            Veg_Cod = 0
            Id_Cod = 0
            Grfi_Cod = 0
            Grva_Cod = 0
            Reg_Cod = 0
            Metodo_Produzione = 0

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    Veg_Cod = DT.Rows(0).Item("Veg_Cod_Gias")
                    Id_Cod = DT.Rows(0).Item("Id_Cod")
                    Grfi_Cod = DT.Rows(0).Item("Grfi_Cod")
                    Grva_Cod = DT.Rows(0).Item("Grva_Cod")
                    Reg_Cod = DT.Rows(0).Item("Reg_Cod")
                    Metodo_Produzione = DT.Rows(0).Item("Metodo_Produzione")

                End If
            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing

    End Sub



    '################################################################################
    'Public Function VarietaAltre(ByVal Veg_Cod As Integer,
    '                             ByVal FinestraTemp_Inizio As Date,
    '                             ByVal FinestraTemp_Fine As Date,
    '                             ByRef objConnessione As DbConnection,
    '                             ByVal StringaConnessione As String,
    '                             ByVal FlagVisibilita As Int32,
    '                             ByVal DirectoryLOG As String,
    '                             ByVal FileLOG As String,
    '                             ByVal IdentificatoreUtente As String) As Integer

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.VarietaAltre()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Dim CulCod As Integer = 0

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT *  FROM  Cultivar ")
    '        StrSQL.Append(" WHERE Veg_Cod = " & Veg_Cod)
    '        StrSQL.Append(" AND Cul_Des LIKE '%altre%' ")

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        If Not DT Is Nothing Then
    '            If DT.Rows.Count > 0 Then
    '                CulCod = DT.Rows(0).Item("Cul_Cod")
    '            End If
    '        End If

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        CulCod = 0
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing

    '    Return CulCod

    'End Function



    Public Function VarietaAltre(ByVal Veg_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.VarietaAltre()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim CulCod As Integer = 0

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT *  FROM  Cultivar ")
            StrSQL.Append(" WHERE Veg_Cod = " & Veg_Cod)
            StrSQL.Append(" AND Cul_Des LIKE '%altre%' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    CulCod = DT.Rows(0).Item("Cul_Cod")
                End If
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            CulCod = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing

        Return CulCod

    End Function



    '################################################################################
    'Public Sub ConvertiFabbricati(ByVal Tipo_Fabbricato_Cod_Cliente As String,
    '                                ByRef Tipo_Fabbricato_Cod_Gias As Integer,
    '                                ByVal FinestraTemp_Inizio As Date,
    '                                ByVal FinestraTemp_Fine As Date,
    '                                ByRef objConnessione As DbConnection,
    '                                ByVal StringaConnessione As String,
    '                                ByVal FlagVisibilita As Int32,
    '                                ByVal DirectoryLOG As String,
    '                                ByVal FileLOG As String,
    '                                ByVal IdentificatoreUtente As String)

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiFabbricati()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * FROM  CAC_Codifica_Fabbricati ")
    '        StrSQL.Append(" WHERE Tipo_Fabbricato_Cod_Cliente = '" & Agro_SQL_SaveText(Tipo_Fabbricato_Cod_Cliente) & "'")

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        Tipo_Fabbricato_Cod_Gias = 0

    '        If Not DT Is Nothing Then
    '            If DT.Rows.Count > 0 Then
    '                Tipo_Fabbricato_Cod_Gias = DT.Rows(0).Item("Tipo_Fabbricato_Cod_Gias")
    '            End If
    '        End If


    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing

    'End Sub



    Public Sub ConvertiFabbricati(ByVal Tipo_Fabbricato_Cod_Cliente As String, _
                                   ByRef Tipo_Fabbricato_Cod_Gias As Integer, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        )

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.ConvertiFabbricati()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Fabbricati ")
            StrSQL.Append(" WHERE Tipo_Fabbricato_Cod_Cliente = '" & Agro_SQL_SaveText(Tipo_Fabbricato_Cod_Cliente) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            Tipo_Fabbricato_Cod_Gias = 0

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    Tipo_Fabbricato_Cod_Gias = DT.Rows(0).Item("Tipo_Fabbricato_Cod_Gias")
                End If
            End If


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing

    End Sub





    '################################################################################
    'Public Sub Converti_Audit_Domande_Interviste(ByVal Codice_Cliente As String,
    '                                            ByRef Raccoglitore_Cod As Integer,
    '                                            ByRef Audit_Tipo As Integer,
    '                                            ByRef Domanda_Cod As Integer,
    '                                            ByRef Descrizione As String,
    '                                            ByVal FinestraTemp_Inizio As Date,
    '                                            ByVal FinestraTemp_Fine As Date,
    '                                            ByRef objConnessione As DbConnection,
    '                                            ByVal StringaConnessione As String,
    '                                            ByVal FlagVisibilita As Int32,
    '                                            ByVal DirectoryLOG As String,
    '                                            ByVal FileLOG As String,
    '                                            ByVal IdentificatoreUtente As String)

    '    Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.Converti_Audit_Domande_Interviste()"


    '    '====================================================================================
    '    'Parametri opzionali :
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * FROM  CAC_Codifica_Audit_Domande_Interviste ")
    '        StrSQL.Append(" WHERE Codice_Cliente = '" & Agro_SQL_SaveText(Codice_Cliente) & "'")

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '        Raccoglitore_Cod = 0
    '        Audit_Tipo = 0
    '        Domanda_Cod = 0
    '        Descrizione = ""

    '        If Not DT Is Nothing Then
    '            If DT.Rows.Count > 0 Then
    '                Raccoglitore_Cod = DT.Rows(0).Item("Raccoglitore_Cod")
    '                Audit_Tipo = DT.Rows(0).Item("Audit_Tipo")
    '                Domanda_Cod = DT.Rows(0).Item("Domanda_Cod")
    '                Descrizione = DT.Rows(0).Item("Descrizione")
    '            End If
    '        End If


    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    DT = Nothing

    'End Sub



    Public Sub Converti_Audit_Domande_Interviste(ByVal Codice_Cliente As String, _
                                                ByRef Raccoglitore_Cod As Integer, _
                                                ByRef Audit_Tipo As Integer, _
                                                ByRef Domanda_Cod As Integer, _
                                                ByRef Descrizione As String, _
                                                    ByVal xFiltroAggiuntivo As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    )

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.Converti_Audit_Domande_Interviste()"


        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * FROM  CAC_Codifica_Audit_Domande_Interviste ")
            StrSQL.Append(" WHERE Codice_Cliente = '" & Agro_SQL_SaveText(Codice_Cliente) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Raccoglitore_Cod = 0
            Audit_Tipo = 0
            Domanda_Cod = 0
            Descrizione = ""


            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    Raccoglitore_Cod = DT.Rows(0).Item("Raccoglitore_Cod")
                    Audit_Tipo = DT.Rows(0).Item("Audit_Tipo")
                    Domanda_Cod = DT.Rows(0).Item("Domanda_Cod")
                    Descrizione = DT.Rows(0).Item("Descrizione")
                End If
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
