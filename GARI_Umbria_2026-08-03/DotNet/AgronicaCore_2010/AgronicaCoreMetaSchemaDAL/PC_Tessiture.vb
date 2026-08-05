Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_Tessiture_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi(ByVal Id_Tessitura As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_Tessiture_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  PC_Tessiture ")
            StrSQL.Append(" WHERE 1=1 ")

            If Id_Tessitura <> 0 Then
                StrSQL.Append(" AND Id_Tessitura =  " & Agro_SQL_SaveNum(Id_Tessitura) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_Tessiture.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_Tessiture.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_IdGruppoTessitura_Da_SabbiaArgilla(ByVal Sabbia As Int32,
                                                             ByVal Argilla As Int32,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_Tessiture_R.Leggi_IdGruppoTessitura_Da_SabbiaArgilla()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '--------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Id_GruppoTessitura ")
            StrSQL.Append(" FROM  PC_Tessiture t inner join PC_TriangoloTessitura tt on t.Regolamento_Cod = tt.Regolamento_Cod and t.Id_Tessitura=tt.Id_Tessitura ")
            StrSQL.Append(" WHERE Sabbia = " & Agro_SQL_SaveNum(Sabbia) & " ")
            StrSQL.Append(" AND   Argilla = " & Agro_SQL_SaveNum(Argilla) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   t.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   t.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Dim rval As Integer = -1
        If DT.Rows.Count > 0 Then
            rval = DT.Rows(0)("Id_GruppoTessitura")
        End If

        Return rval

    End Function

    Public Function Leggi_IdGruppoTessitura_Da_SabbiaArgilla_DT(ByVal Sabbia As Int32,
                                                             ByVal Argilla As Int32,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_Tessiture_R.Leggi_IdGruppoTessitura_Da_SabbiaArgilla()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '--------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT distinct Id_GruppoTessitura,Sabbia,argilla  ")
            StrSQL.Append(" FROM  PC_Tessiture t inner join PC_TriangoloTessitura tt on t.Regolamento_Cod = tt.Regolamento_Cod and t.Id_Tessitura=tt.Id_Tessitura ")

            StrSQL.Append(" WHERE 1=1 ")
            If Sabbia <> -99 Then
                StrSQL.Append(" AND Sabbia = " & Agro_SQL_SaveNum(Sabbia) & " ")
            End If
            If Argilla <> -99 Then
                StrSQL.Append(" AND Argilla = " & Agro_SQL_SaveNum(Argilla) & " ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   t.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   t.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

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

    Public Function Leggi_Id_ClasseTessitura_Da_SabbiaArgilla(ByVal Sabbia As Int32,
                                                              ByVal Argilla As Int32,
                                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                 Optional ByVal verbose As Boolean = False,
                                                                 Optional xFiltroAggiuntivo As String = ""
                                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_Tessiture_R.Leggi_Id_ClasseTessitura_Da_SabbiaArgilla()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '--------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT t.Id_ClasseTessitura, Sabbia, Argilla ")
            If verbose Then
                StrSQL.Append("     , ct.Descrizione ")
            End If
            StrSQL.Append(" FROM  PC_Tessiture t ")
            StrSQL.Append(" INNER JOIN PC_TriangoloTessitura tt ON ")
            StrSQL.Append("     t.Regolamento_Cod = tt.Regolamento_Cod AND t.Id_Tessitura = tt.Id_Tessitura ")

            If verbose Then
                StrSQL.Append(" INNER JOIN PC_ClassiTessitura ct ON ")
                StrSQL.Append("     ct.Id_ClasseTessitura = t.Id_ClasseTessitura ")
            End If

            StrSQL.Append(" WHERE 1 = 1 ")

            If Sabbia <> -99 Then
                StrSQL.Append(" AND Sabbia = " & Agro_SQL_SaveNum(Sabbia) & " ")
            End If
            If Argilla <> -99 Then
                StrSQL.Append(" AND Argilla = " & Agro_SQL_SaveNum(Argilla) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   t.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   t.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

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



End Class
