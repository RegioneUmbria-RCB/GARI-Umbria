Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text


Public Class RMA_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Private Function Popola_TabellaRMA_GetQuery(ByVal piva As String) As String
        Dim rval As String =
            "SELECT rma.TAbellaRMA_COD, rma.TAbellaRMA_DES  " &
            "from StrutturaGerarchicaTabellaRMA st " &
            "inner join TabellaRMA RMA on RMA.TabellaRMA_COD = st.TabellaRMA_COD " &
            " WHERE 1=1 " &
                "AND st.Operazione = 1 " &
                "and st.Impresa in ( " &
                "select  '" & Agro_SQL_SaveText(piva) & "' as impresa )"

        Return rval

    End Function

    Public Function Get_TabellaRMA( _
                  ByVal PivaSuperUser As String _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.RMA_R.Get_TabellaRMA()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing



        Try
            
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Popola_TabellaRMA_GetQuery(PivaSuperUser), NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function Get_RMA( _
                  ByVal TAbellaRMA_COD As Integer _
                , ByVal Pa_cod As Integer _
                , ByVal Fam_cod As String _
                , ByVal Veg_Cod As Integer _
                , ByVal data As Date _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.RMA_R.Get_RMA()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder


        Try
            Stb.Append("select   " & vbCrLf)
            Stb.Append("    CAST (T.RMA as real) as RMA " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" from RMA_ConDate T " & vbCrLf)
            Stb.Append("    inner join ( " & vbCrLf)
            Stb.Append("        select TabellaRMA_COD, DERR_CODIFICA, DERR_COD, Veg_cod, Pa_Cod_RMA, Pa_o_fam_cod, isFamiglia, max(Validita_Inizio) as Validita_inizio " & vbCrLf)
            Stb.Append("        from RMA_ConDate " & vbCrLf)
            Stb.Append("        where validita_inizio < " & Agro_SQL_SaveDate(data) & vbCrLf)
            Stb.Append("        group by TabellaRMA_COD, DERR_CODIFICA, DERR_COD, Veg_cod, Pa_Cod_RMA, Pa_o_fam_cod, isFamiglia " & vbCrLf)
            Stb.Append("    ) T1 " & vbCrLf)
            Stb.Append(" on T.TabellaRMA_COD = T1.TabellaRMA_COD " & vbCrLf)
            Stb.Append(" and T.DERR_Codifica = T1.DERR_Codifica " & vbCrLf)
            Stb.Append(" and T.Derr_Cod = T1.Derr_Cod " & vbCrLf)
            Stb.Append(" and T.veg_cod = T1.Veg_cod " & vbCrLf)
            Stb.Append(" and T.pa_cod_rma = T1.pa_cod_rma " & vbCrLf)
            Stb.Append(" and T.pa_o_Fam_cod = T1.pa_o_fam_Cod " & vbCrLf)
            Stb.Append(" and T.isFamiglia = T1.isFamiglia " & vbCrLf)
            Stb.Append(" and T.Validita_Inizio = T1.Validita_inizio " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where  " & vbCrLf)
            Stb.Append(" 1=1 " & vbCrLf)            
            Stb.Append(" and T.TabellaRMA_COD = " & TAbellaRMA_COD & " " & vbCrLf)
            Stb.Append(" and T.Veg_cod =  " & Veg_Cod & " " & vbCrLf)
            Stb.Append(" and (  " & vbCrLf)
            Stb.Append("        (T.isFamiglia = 0 and T.PA_o_Fam_Cod =  " & Pa_cod & ") " & vbCrLf)
            Stb.Append("    or  (T.isFamiglia <> 0 and T.PA_o_Fam_Cod = '" & Agro_SQL_SaveText(Fam_cod) & "') " & vbCrLf)
            Stb.Append(" ) " & vbCrLf)

            '  Vanni, 22/11/2016 17:06:27: TODO: La clausola serve per il caso di un principio attivo che cambia il proprio nome nel tempo.
            Stb.Append(" order by T.validita_inizio desc " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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
