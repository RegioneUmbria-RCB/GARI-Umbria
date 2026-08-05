Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class CAC_Codifica_Zone
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function Leggi(ByVal Zona_Cod_Cliente As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Codifica_Codici.Leggi()"

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
            If Zona_Cod_Cliente <> "" Then
                StrSQL.Append(" WHERE Zona_Cod_Cliente = '" & Agro_SQL_SaveText(Zona_Cod_Cliente) & "'")
            End If
            '---------------------------------------------

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            Return DT

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Zona_Cod = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


    End Function


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
    '        If Zona_Cod_Cliente <> "" Then
    '            StrSQL.Append(" WHERE Zona_Cod_Cliente = '" & Agro_SQL_SaveText(Zona_Cod_Cliente) & "'")
    '        End If
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


End Class


Public Class CAC_Codifica_Zone_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function scrivi( _
                      ByVal Zona_Cod_Cliente As String _
                    , ByVal Descrizione As String _
                    , ByVal Zona_Cod_Gias As Integer _
                    , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
           , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CAC_Codifica_Zone ")
            StrSQL.Append(" ( ")
            StrSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            StrSQL.Append("  ,[Zona_Cod_Cliente] " & vbCrLf)
            StrSQL.Append("  ,[Descrizione] " & vbCrLf)
            StrSQL.Append("  ,[Zona_Cod_Gias] " & vbCrLf)
            StrSQL.Append("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.Append("  ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("      '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            StrSQL.Append("     ,'" & Agro_SQL_SaveText(Zona_Cod_Cliente) & "'" & vbCrLf)
            StrSQL.Append("     ,'" & Agro_SQL_SaveText(Descrizione) & "'" & vbCrLf)
            StrSQL.Append("     , " & Agro_SQL_SaveNum(Zona_Cod_Gias) & " " & vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")

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
