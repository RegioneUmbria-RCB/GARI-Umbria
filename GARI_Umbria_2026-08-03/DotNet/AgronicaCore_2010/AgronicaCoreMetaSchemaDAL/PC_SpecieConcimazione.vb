Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_SpecieConcimazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Int32, _
                          ByVal Veg_Cod As Int32, _
                          ByVal Id_Ciclo As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_SpecieConcimazione_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT sc.*,s.veg_des  ")
            StrSQL.Append(" FROM  PC_SpecieConcimazione sc ")
            StrSQL.Append(" INNER JOIN    SpecieVegetali s ON s.Veg_Cod = sc.Veg_Cod ")

            StrSQL.Append(" WHERE Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   sc.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   sc.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Id_Ciclo <> 0 Then
                StrSQL.Append(" AND sc.Id_Ciclo =  " & Agro_SQL_SaveNum(Id_Ciclo) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND sc.Veg_Cod =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   sc.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   sc.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            '--------------------------------------------------------------------------ù
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY s.veg_des ")
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

    '######################################################################################################################
    Public Function IdCiclo_From_VegCod(ByVal Regolamento_Cod As Int32, _
                                        ByVal Veg_Cod As Int32, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_SpecieConcimazione_R.IdCiclo_From_VegCod.()"
        Dim intRet As Integer

        Dim dt As DataTable

        dt = Leggi(Regolamento_Cod, _
                   Veg_Cod, _
                   0, _
                   "", _
                   "", _
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                intRet = dt.Rows(0).Item("Id_Ciclo")
            Else
                intRet = 0
            End If
        Else
            intRet = 0
        End If

        Return intRet

    End Function

End Class
