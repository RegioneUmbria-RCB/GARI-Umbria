Public Class GerarchiaMacchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################
    Public Function Leggi(ByVal Mac_Cod_Padre As Int32,
                          ByVal Mac_Cod_Figlio As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.GerarchiaMacchine_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT *  ")
            StrSQL.AppendLine(" FROM   GerarchiaParco_Macchine ")

            StrSQL.AppendLine(" WHERE  Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Mac_Cod_Padre <> 0 Then
                StrSQL.AppendLine(" AND Mac_Cod_Padre = " & Agro_SQL_SaveNum(Mac_Cod_Padre) & " ")
            End If
            If Mac_Cod_Figlio <> 0 Then
                StrSQL.AppendLine(" AND Mac_Cod_Figlio = " & Agro_SQL_SaveNum(Mac_Cod_Figlio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function LeggiConDettagliPadreFiglio(ByVal Mac_Cod_Padre As Int32,
                                                ByVal Mac_Cod_Figlio As Int32,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.GerarchiaMacchine_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT a.*  ")
            StrSQL.AppendLine(" , p.Class_Code as Class_Code_Padre  ")
            StrSQL.AppendLine(" , p.Mac_Des as Mac_Des_Padre  ")
            StrSQL.AppendLine(" , p.ExternalAPIKey as ExternalAPIKey_Padre  ")
            StrSQL.AppendLine(" , p.Targa as Targa_Padre  ")
            StrSQL.AppendLine(" , p.Telaio as Telaio_Padre  ")
            StrSQL.AppendLine(" , p.Modello as Modello_Padre  ")
            StrSQL.AppendLine(" , p.N_Immatricolazione as N_Immatricolazione_Padre  ")
            StrSQL.AppendLine(" , p.Codice as Codice_Padre  ")
            StrSQL.AppendLine(" , p.VIN as VIN_Padre  ")
            StrSQL.AppendLine(" , f.Class_Code as Class_Code_Figlio  ")
            StrSQL.AppendLine(" , f.Mac_Des as Mac_Des_Figlio  ")
            StrSQL.AppendLine(" , f.ExternalAPIKey as ExternalAPIKey_Figlio  ")
            StrSQL.AppendLine(" , f.Targa as Targa_Figlio  ")
            StrSQL.AppendLine(" , f.Telaio as Telaio_Figlio  ")
            StrSQL.AppendLine(" , f.Modello as Modello_Figlio  ")
            StrSQL.AppendLine(" , f.N_Immatricolazione as N_Immatricolazione_Figlio  ")
            StrSQL.AppendLine(" , f.Codice as Codice_Figlio  ")
            StrSQL.AppendLine(" , f.VIN as VIN_Figlio  ")
            StrSQL.AppendLine(" FROM   GerarchiaParco_Macchine a ")
            StrSQL.AppendLine(" left join Parco_Macchine p ")
            StrSQL.AppendLine(" on (a.Mac_Cod_Padre=p.Mac_Cod) ")
            StrSQL.AppendLine(" left join Parco_Macchine f ")
            StrSQL.AppendLine(" on (a.Mac_Cod_Figlio=f.Mac_Cod) ")

            StrSQL.AppendLine(" WHERE  a.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    a.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Mac_Cod_Padre <> 0 Then
                StrSQL.AppendLine(" AND a.Mac_Cod_Padre = " & Agro_SQL_SaveNum(Mac_Cod_Padre) & " ")
            End If
            If Mac_Cod_Figlio <> 0 Then
                StrSQL.AppendLine(" AND a.Mac_Cod_Figlio = " & Agro_SQL_SaveNum(Mac_Cod_Figlio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
End Class