Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Categorie_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(ByVal COD As String, _
                            ByVal Padre As String, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Categorie_r.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Categorie ")
            StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Padre <> "" Then
                StrSQL.Append(" AND Padre =  '" & Agro_SQL_SaveText(Padre) & "'  ")
            End If
            If COD <> "" Then
                StrSQL.Append(" AND  COD = '" & Agro_SQL_SaveText(COD) & "'  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Categorie.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Categorie.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descr ")
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


    Public Function Descr_From_Padre_Cod(ByVal Padre As String, _
                                         ByVal Cod As String, _
                                         ByVal Cod_Integer As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Categorie_r.Descr_From_Padre_Cod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim descrizione As String = ""

        Try

            If Padre = "" Then
                Throw New Exception("Parametro non corretto nella query (Padre obbligatorio)")
            End If

            If Cod = "" Then
                If Cod_Integer = 0 Then
                    Throw New Exception("Parametro non corretto nella query (Cod obbligatorio)")
                Else
                    'esempio: S000032
                    Cod = "S" & Right("000000" & CStr(Cod_Integer), 6)
                End If
            End If

            DT = Leggi(Cod, _
                       Padre, _
                        "", "", objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                descrizione = DT.Rows(0).Item("DESCR")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return descrizione

    End Function












End Class



