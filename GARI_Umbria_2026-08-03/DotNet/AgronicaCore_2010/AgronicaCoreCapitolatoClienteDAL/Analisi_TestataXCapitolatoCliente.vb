

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Analisi_TestataXCapitolatoCliente_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Sub CancellaByUniqueId(ByVal UniqueID As String, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
               "     DELETE FROM Analisi_TestataXCapitolatoCliente " & _
               "    WHERE UniqueID  = '" & UniqueID & "'"

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Sub

    Public Sub Cancella(ByVal PivaSuperUser As String, _
                      ByVal Progressivo As Integer, _
                      ByVal CapitolatoCliente_ID As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
               "     DELETE FROM Analisi_TestataXCapitolatoCliente " & _
               "    WHERE PivaSuperUser = '" & PivaSuperUser & "'" & _
               "    AND Progressivo = " & Progressivo & _
               "    AND Capitolato_Cod = " & CapitolatoCliente_ID & _
               "    AND UniqueId = '" & UniqueID & "'"

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Sub Scrivi(ByVal PivaSuperUser As String, _
                      ByVal Progressivo As Integer, _
                      ByVal CapitolatoCliente_ID As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
           "     INSERT INTO Analisi_TestataXCapitolatoCliente" & _
             "           ([PivaSuperUser] " & _
             "           ,[Progressivo] " & _
             "           ,[Capitolato_Cod] " & _
             "           ,[UniqueID]) " & _
             "            VALUES " & _
             "           ('" & PivaSuperUser & "' " & _
             "           ," & Progressivo & "" & _
             "           ," & CapitolatoCliente_ID & "" & _
            "           ,'" & UniqueID & "')"

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub
End Class

Public Class Analisi_TestataXCapitolatoCliente_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function verifica_passa_DPI_se_capitolato_ha_indicato_flagUsaImpianto( _
                            ByVal UniqueID As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.verifica_passa_DPI_se_capitolato_ha_indicato_flagUsaImpianto()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim StB As New StringBuilder

        'da sql: __Analisi_verifica_passa_DPI_se_capitolato_indicato_flag.sql

        StB.Append("    --  " & NomeRoutine & vbCrLf)
        StB.Append("    select  " & vbCrLf)
        StB.Append("      T.UniqueID " & vbCrLf)
        StB.Append("    , T.PivaSuperUser " & vbCrLf)
        StB.Append("    , T.Progressivo  " & vbCrLf)
        StB.Append("    , CC.Capitolato_COD " & vbCrLf)
        StB.Append("    , CC.Flag_Privato_Pubblico_DPI  " & vbCrLf)
        StB.Append("    , CC.DPI_COD_REGOLAMENTO " & vbCrLf)
        StB.Append("    , R.NomeEsteso as DPI_Descrizione " & vbCrLf)
        StB.Append("    , CC.DPI_Impianto " & vbCrLf)
        StB.Append("  " & vbCrLf)
        StB.Append(" from analisi_testata T  " & vbCrLf)
        StB.Append("    inner join Analisi_TestataXCapitolatoCliente ATCC " & vbCrLf)
        StB.Append("        on T.PivaSuperUser = ATCC.PivaSuperUser " & vbCrLf)
        StB.Append("            and T.Progressivo = ATCC.Progressivo " & vbCrLf)
        StB.Append("            and T.UniqueID = ATCC.UniqueID  " & vbCrLf)
        StB.Append("    inner join CapitolatoCliente CC " & vbCrLf)
        StB.Append("        on  CC.Capitolato_COD = ATCC.Capitolato_Cod  " & vbCrLf)
        StB.Append("    left join DPI_Regolamenti R " & vbCrLf)
        StB.Append("        on CC.Flag_Privato_Pubblico_DPI = R.Flag_Privato_Pubblico " & vbCrLf)
        StB.Append("        and CC.DPI_COD_REGOLAMENTO = R.Cod_regolamento " & vbCrLf)
        StB.Append("  " & vbCrLf)
        StB.Append(" where T.uniqueID = " & UniqueID & vbCrLf)


        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StB.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function



    Public Function VerificaCapitolato_PercentualeSommaPa_FAST(ByVal UniqueID As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_PercentualeSommaPa()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim StB As New StringBuilder

        'da sql: __analisi_verifica_Capitolato_PA_sommaPerc.sql

        StB.Append("select  " & vbCrLf)
        StB.Append("      PerSommatoria.PivaSuperUser " & vbCrLf)
        StB.Append("    , PerSommatoria.Progressivo  " & vbCrLf)
        StB.Append("    , PerSommatoria.Capitolato_Cod  " & vbCrLf)
        StB.Append("    , PerSommatoria.Sum_Perc_Max_RMA  " & vbCrLf)
        StB.Append("    , sum(PerSommatoria.sumQtaRilevata) as sumQtaRilevata " & vbCrLf)
        StB.Append("    , case when PerSommatoria.Sum_Perc_Max_RMA < sum(PerSommatoria.sumQtaRilevata) then 1 else 0 end as PercSuperata  " & vbCrLf)
        StB.Append(" from ( " & vbCrLf)
        StB.Append(" select distinct " & vbCrLf)
        StB.Append("      T.PivaSuperUser " & vbCrLf)
        StB.Append("    , T.Progressivo  " & vbCrLf)
        StB.Append("    , T.Capitolato_COD " & vbCrLf)
        StB.Append("    , T.Sum_Perc_Max_RMA       " & vbCrLf)
        StB.Append("    , case when T.RMA = 0 then T.QtaRilevata else cast( ( cast(T.QtaRilevata as real) / cast(T.RMA as real) ) as real) end  * 100 as sumQtaRilevata          " & vbCrLf)
        StB.Append("  " & vbCrLf)
        StB.Append(" from Analisi_Testata TES  " & vbCrLf)
        StB.Append("    inner join SommaPercentuali_TEMP  T " & vbCrLf)
        StB.Append("        on TES.UniqueID = T.UniqueID  " & vbCrLf)
        StB.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
        StB.Append("        and T.Progressivo = TES.Progressivo                          " & vbCrLf)
        StB.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)
        StB.Append("    inner join ( " & vbCrLf)
        StB.Append("        select PivaSuperUser, Progressivo, PA_COD, isFamiglia, TabellaRMA_cod, Capitolato_Cod, max(Validita_inizio) as validita_inizio " & vbCrLf)
        StB.Append("        from SommaPercentuali_TEMP " & vbCrLf)
        StB.Append("        where uniqueID = " & UniqueID & " " & vbCrLf)
        StB.Append("        group by PivaSuperUser, Progressivo, PA_COD, isFamiglia, TabellaRMA_cod, Capitolato_Cod  " & vbCrLf)
        StB.Append("    ) t1 " & vbCrLf)
        StB.Append(" on t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
        StB.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
        StB.Append(" and t1.Capitolato_Cod = t.capitolato_COD " & vbCrLf)
        StB.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
        StB.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
        StB.Append(" and t1.isFamiglia = t.isFamiglia " & vbCrLf)
        StB.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
        StB.Append("  " & vbCrLf)
        StB.Append(" where TES.uniqueID = " & UniqueID & " " & vbCrLf)
        'StB.Append(" and T.Sum_Perc_Max_RMA is not null     " & vbCrLf)
        StB.Append(" ) PerSommatoria     " & vbCrLf)
        StB.Append("  " & vbCrLf)
        StB.Append(" group by PerSommatoria.PivaSuperUser, PerSommatoria.Progressivo, PerSommatoria.Capitolato_COD, PerSommatoria.Sum_Perc_Max_RMA " & vbCrLf)
        'StB.Append(" having PerSommatoria.Sum_Perc_Max_RMA < sum(PerSommatoria.sumQtaRilevata) " & vbCrLf)



        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StB.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function


    'Public Function VerificaCapitolato_PercentualeSommaPa(ByVal UniqueID As String, _
    '                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaCapitolato_PercentualeSommaPa()"

    '    Dim MessaggioErrore As String = ""
    '    Dim DT As DataTable = Nothing
    '    Dim StB As New StringBuilder

    '    'da sql: __analisi_verifica_Capitolato_PA_sommaPerc.sql

    '    StB.Append("select  " & vbCrLf)
    '    StB.Append("      PerSommatoria.PivaSuperUser " & vbCrLf)
    '    StB.Append("    , PerSommatoria.Progressivo  " & vbCrLf)
    '    StB.Append("    , PerSommatoria.Capitolato_Cod  " & vbCrLf)
    '    StB.Append("    , PerSommatoria.Sum_Perc_Max_RMA  " & vbCrLf)
    '    StB.Append("    , sum(PerSommatoria.sumQtaRilevata) as sumQtaRilevata " & vbCrLf)
    '    StB.Append("    , case when PerSommatoria.Sum_Perc_Max_RMA < sum(PerSommatoria.sumQtaRilevata) then 1 else 0 end as PercSuperata  " & vbCrLf)
    '    StB.Append(" from ( " & vbCrLf)
    '    StB.Append(" select distinct " & vbCrLf)
    '    StB.Append("      T.PivaSuperUser " & vbCrLf)
    '    StB.Append("    , T.Progressivo  " & vbCrLf)
    '    StB.Append("    , T.Capitolato_COD " & vbCrLf)
    '    StB.Append("    , T.Sum_Perc_Max_RMA       " & vbCrLf)
    '    StB.Append("    , case when T.RMA = 0 then T.QtaRilevata else cast( ( cast(T.QtaRilevata as real) / cast(T.RMA as real) ) as real) end  * 100 as sumQtaRilevata          " & vbCrLf)
    '    StB.Append("  " & vbCrLf)
    '    StB.Append(" from Analisi_Testata TES  " & vbCrLf)
    '    StB.Append("    inner join SommaPercentuali  T " & vbCrLf)
    '    StB.Append("        on TES.UniqueID = T.UniqueID  " & vbCrLf)
    '    StB.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
    '    StB.Append("        and T.Progressivo = TES.Progressivo                          " & vbCrLf)
    '    StB.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)
    '    StB.Append("    inner join ( " & vbCrLf)
    '    StB.Append("        select PivaSuperUser, Progressivo, PA_COD, isFamiglia, TabellaRMA_cod, Capitolato_Cod, max(Validita_inizio) as validita_inizio " & vbCrLf)
    '    StB.Append("        from SommaPercentuali " & vbCrLf)
    '    StB.Append("        where uniqueID = " & UniqueID & " " & vbCrLf)
    '    StB.Append("        group by PivaSuperUser, Progressivo, PA_COD, isFamiglia, TabellaRMA_cod, Capitolato_Cod  " & vbCrLf)
    '    StB.Append("    ) t1 " & vbCrLf)
    '    StB.Append(" on t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
    '    StB.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
    '    StB.Append(" and t1.Capitolato_Cod = t.capitolato_COD " & vbCrLf)
    '    StB.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
    '    StB.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
    '    StB.Append(" and t1.isFamiglia = t.isFamiglia " & vbCrLf)
    '    StB.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
    '    StB.Append("  " & vbCrLf)
    '    StB.Append(" where TES.uniqueID = " & UniqueID & " " & vbCrLf)
    '    'StB.Append(" and T.Sum_Perc_Max_RMA is not null     " & vbCrLf)
    '    StB.Append(" ) PerSommatoria     " & vbCrLf)
    '    StB.Append("  " & vbCrLf)
    '    StB.Append(" group by PerSommatoria.PivaSuperUser, PerSommatoria.Progressivo, PerSommatoria.Capitolato_COD, PerSommatoria.Sum_Perc_Max_RMA " & vbCrLf)
    '    'StB.Append(" having PerSommatoria.Sum_Perc_Max_RMA < sum(PerSommatoria.sumQtaRilevata) " & vbCrLf)



    '    Try

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StB.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT
    'End Function

    Public Function VerificaCapitolato_NumeroPrincipiAttiviAmmessi(ByVal UniqueID As String, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_NumeroPrincipiAttiviAmmessi()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing

        Dim stb As New StringBuilder

        Try
            'da sql: Gias_capitolatoCliente.__analisi_verifica_Capitolato_PA_Conteggio.sql

            stb.Append("select T.PivaSuperUser, T.Progressivo, T.UniqueID, CC.Capitolato_Cod, PR.Conteggio, CC.N_Max_PA " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.progressivo = ACC.Progressivo      " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
            stb.Append(" inner join ( " & vbCrLf)
            stb.Append("    select T.PivaSuperUser, T.Progressivo, T.UniqueID, COUNT(*) as Conteggio " & vbCrLf)
            stb.Append("    from Analisi_Testata T " & vbCrLf)
            stb.Append("        inner join Analisi_TestataXResiduiRilevati paRil " & vbCrLf)
            stb.Append("        on paRil.PivaSuperUser = T.PivaSuperUser  " & vbCrLf)
            stb.Append("        and paRil.Progressivo = T.Progressivo " & vbCrLf)
            stb.Append("        and paRil.UniqueID = T.UniqueID " & vbCrLf)
            stb.Append("    group by T.PivaSuperUser, T.Progressivo, T.UniqueID  " & vbCrLf)
            stb.Append(" ) PR  " & vbCrLf)
            stb.Append("    on PR.PivaSuperUser = T.PivaSuperUser  " & vbCrLf)
            stb.Append("    and PR.progressivo = T.Progressivo " & vbCrLf)
            stb.Append("    and PR.UniqueID = T.UniqueID " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where T.UniqueID = " & UniqueID & vbCrLf)
            stb.Append(" and PR.Conteggio > CC.N_Max_PA  " & vbCrLf)
            stb.Append(" and CC.N_MAX_PA is not null " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_RMAPrincipio(ByVal UniqueID As String) As String

        Dim Stb As New StringBuilder
        Stb.Length = 0

        Stb.Append("        Select distinct " & vbCrLf)
        Stb.Append("      T.pivaSuperUser " & vbCrLf)
        Stb.Append("    , T.progressivo  " & vbCrLf)
        Stb.Append("    , T.pa_cod " & vbCrLf)
        Stb.Append("    , T.isFamiglia  " & vbCrLf)
        Stb.Append("    , T.QtaRilevata " & vbCrLf)
        Stb.Append("    , T.RMA " & vbCrLf)
        Stb.Append("    , T.Capitolato_cod " & vbCrLf)
        Stb.Append("    , T.Perc_max_rma " & vbCrLf)
        Stb.Append("    , T.N_Max_PA  " & vbCrLf)
        Stb.Append("    , T.Sum_Perc_Max_RMA  " & vbCrLf)
        Stb.Append("    , case when T.RMA = 0 then 0 else cast(T.QtaRilevata * 100 / cast(T.RMA as real) as real) end as percQtaRilevata  " & vbCrLf)
        Stb.Append("    , T.validita_inizio  " & vbCrLf)
        Stb.Append("     , case when T.RMA = 0 then 1 else case when cast(T.QtaRilevata * 100 / cast(T.RMA as real) as real) > T.Perc_Max_RMA    then 1 else 0 end end as PercSuperata ")
        Stb.Append("    , T.RMA_REG_DES as Regolamento " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" FROM  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" Analisi_Testata AS TES   " & vbCrLf)
        Stb.Append(" inner join  " & vbCrLf)
        Stb.Append(" ( " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    select  " & vbCrLf)
        Stb.Append("        T.UniqueID  " & vbCrLf)
        Stb.Append("        , T.PivaSuperUser " & vbCrLf)
        Stb.Append("        , T.Progressivo " & vbCrLf)
        Stb.Append("        , ATPF.pa_o_fam_COD as PA_COD " & vbCrLf)
        Stb.Append("        , ATPF.isFamiglia " & vbCrLf)
        Stb.Append("        , rRma.TabellaRma_COD " & vbCrLf)
        Stb.Append("        , CC.Capitolato_Cod " & vbCrLf)
        Stb.Append("        , ATPF.QtaRilevata " & vbCrLf)
        Stb.Append("        , paRMA.RMA " & vbCrLf)
        Stb.Append("        , CC.Perc_Max_RMA " & vbCrLf)
        Stb.Append("        , CC.Sum_Perc_Max_RMA " & vbCrLf)
        Stb.Append("        , rRma.Validita_inizio  " & vbCrLf)
        Stb.Append("        , rRma.RMA_REG_DES collate Latin1_General_CI_AS + '  - ' + isnull(paRMA.note, '') collate Latin1_General_CI_AS as RMA_REG_DES " & vbCrLf)
        Stb.Append("        , cc.N_Max_PA " & vbCrLf)
        Stb.Append(" " & vbCrLf)

        Stb.Append(VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_RMAPrincipioInnerQRY(UniqueID) & vbCrLf)

        Stb.Append(" ) T " & vbCrLf)


        Stb.Append(" on TES.UniqueID = T.UniqueID  " & vbCrLf)
        Stb.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
        Stb.Append("        and T.Progressivo = TES.Progressivo                  " & vbCrLf)
        Stb.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("         inner Join " & vbCrLf)
        Stb.Append(" ( " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    select  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("          T.PivaSuperUser " & vbCrLf)
        Stb.Append("        , T.Progressivo " & vbCrLf)
        Stb.Append("        , ATPF.pa_o_fam_COD as PA_COD " & vbCrLf)
        Stb.Append("        , ATPF.isFamiglia " & vbCrLf)
        Stb.Append("        , rRma.TabellaRma_COD " & vbCrLf)
        Stb.Append("        , CC.Capitolato_Cod " & vbCrLf)
        Stb.Append("        , max(rRma.Validita_inizio ) as Validita_inizio " & vbCrLf)
        Stb.Append(" " & vbCrLf)

        Stb.Append(VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_RMAPrincipioInnerQRY(UniqueID) & vbCrLf)

        Stb.Append("        and T.uniqueID = " & UniqueID & " " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    group by T.PivaSuperUser " & vbCrLf)
        Stb.Append("        , T.Progressivo " & vbCrLf)
        Stb.Append("        , ATPF.pa_o_fam_COD " & vbCrLf)
        Stb.Append("        , ATPF.isFamiglia " & vbCrLf)
        Stb.Append("        , rRma.TabellaRma_COD " & vbCrLf)
        Stb.Append("        , CC.Capitolato_Cod " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" ) T1 " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" on t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
        Stb.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
        Stb.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
        Stb.Append(" and t1.isFamiglia = t1.isFamiglia " & vbCrLf)
        Stb.Append(" and t1.Capitolato_Cod = t.capitolato_COD " & vbCrLf)
        Stb.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
        Stb.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" where TES.uniqueID =  " & UniqueID & " ")



        Return Stb.ToString
    End Function
    Public Function VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_RMAPrincipioInnerQRY(ByVal UniqueID As String) As String


        Dim Stb As New StringBuilder
        Stb.Length = 0

       Stb.Append(" from Analisi_Testata AS T   " & vbCrLf  ) 
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN Analisi_TestataXCapitolatoCliente AS ACC   " & vbCrLf)
        Stb.Append("        ON T.UniqueID = ACC.UniqueID   " & vbCrLf)
        Stb.Append("        AND T.PivaSuperUser = ACC.PivaSuperUser   " & vbCrLf)
        Stb.Append("        AND T.Progressivo = ACC.Progressivo   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN CapitolatoCliente AS CC   " & vbCrLf)
        Stb.Append("        ON CC.Capitolato_COD = ACC.Capitolato_Cod   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN Analisi_TestataXResiduiRilevati AS ATPF   " & vbCrLf)
        Stb.Append("        ON T.PivaSuperUser = ATPF.pivaSuperUser   " & vbCrLf)
        Stb.Append("        AND T.Progressivo = ATPF.Progressivo   " & vbCrLf)
        Stb.Append("        AND T.UniqueID = ATPF.UniqueID   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN TrascodificaFamiglie_e_Principi AS papaRMA   " & vbCrLf)
        Stb.Append("        ON papaRMA.pa_o_Fam_cod = ATPF.pa_o_fam_COD   " & vbCrLf)
        Stb.Append("        AND papaRMA.isFamiglia = ATPF.isFamiglia   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN RegolamentiRMA AS rRma   " & vbCrLf)
        Stb.Append("        ON rRma.RMA_REG_COD = papaRMA.rma_reg_cod   " & vbCrLf)
        Stb.Append("        AND rRma.TabellaRMA_COD = CC.TabellaRMA_COD      " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    INNER JOIN PrincipiAttiviRMA AS paRMA   " & vbCrLf)
        Stb.Append("        ON papaRMA.pa_COD_RMA = paRMA.PA_COD   " & vbCrLf)
        Stb.Append("        AND papaRMA.rma_reg_cod = paRMA.RMA_REG_COD   " & vbCrLf)
        Stb.Append("         where paRMA.rma Is Not null " & vbCrLf)


        Return Stb.ToString

    End Function

    Public Function VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_FAST(ByVal UniqueID As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_FAST()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder


        'query da: GIAS_capitolatoCliente.__analisi_verifica_Capitolato_PA_PercSingolo.sql

        Stb.Append(" --  " & NomeRoutine & vbCrLf)
        Stb.Append(" select  " & vbCrLf)
        Stb.Append("      T.pivaSuperUser " & vbCrLf)
        Stb.Append("    , T.progressivo  " & vbCrLf)
        Stb.Append("    , T.PA_Cod " & vbCrLf)
        Stb.Append("    , T.isFamiglia " & vbCrLf)
        Stb.Append("    , T.QtaRilevata " & vbCrLf)
        Stb.Append("    , coalesce(T.RMA, -1) as RMA  " & vbCrLf)
        Stb.Append("    , T.Capitolato_cod " & vbCrLf)
        Stb.Append("    , T.Perc_max_rma " & vbCrLf)
        Stb.Append("    , T.N_Max_PA  " & vbCrLf)
        Stb.Append("    , T.Sum_Perc_Max_RMA  " & vbCrLf)
        Stb.Append("    , T.percQtaRilevata " & vbCrLf)
        Stb.Append("    , T.validita_inizio  " & vbCrLf)
        Stb.Append("    , case when cast(t.RMA as real) = 0 or case when t.RMA = 0 then 1 else cast(t.QtaRilevata * 100 / cast(t.RMA as real) as real) end > T.Perc_Max_RMA	then 1 else 0 end as PercSuperata " & vbCrLf)
        'Stb.Append("    , case when T.TabellaRMA_COD = 1 then T.RMA_REG_DES else '-' end as Regolamento  " & vbCrLf)
        Stb.Append("    , T.RMA_REG_DES as Regolamento  " & vbCrLf)
        Stb.Append(" from Analisi_Testata TES  " & vbCrLf)

        Stb.Append("    inner join PercentualiPA_TEMP  T " & vbCrLf)
        Stb.Append("        on TES.UniqueID = T.UniqueID  " & vbCrLf)
        Stb.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
        Stb.Append("        and T.Progressivo = TES.Progressivo                  " & vbCrLf)
        Stb.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)


        Stb.Append("    inner join ( " & vbCrLf)
        Stb.Append("        select PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod, Capitolato_Cod, max(Validita_inizio) as validita_inizio " & vbCrLf)
        Stb.Append("        from PercentualiPA_TEMP " & vbCrLf)
        Stb.Append("        where uniqueID = " & UniqueID & " " & vbCrLf)
        Stb.Append("        group by PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod, Capitolato_Cod  " & vbCrLf)
        Stb.Append("    ) t1 " & vbCrLf)
        Stb.Append(" on t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
        Stb.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
        Stb.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
        Stb.Append(" and t1.Capitolato_Cod = t.capitolato_COD " & vbCrLf)
        Stb.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
        Stb.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" where TES.uniqueID = " & UniqueID & " " & vbCrLf)
        'Stb.Append(" and ( " & vbCrLf)
        'Stb.Append("    cast(t.RMA as real) = 0 or " & vbCrLf)
        'Stb.Append("    case when t.RMA = 0 then (t.QtaRilevata * 100) else cast(t.QtaRilevata * 100 / cast(t.RMA as real) as real) end > T.Perc_Max_RMA" & vbCrLf)
        'Stb.Append("  )" & vbCrLf)
        Stb.Append(" union " & vbCrLf)

        Stb.Append(" --compendio prima query (principi attivi che non hanno rma per derrata ma lo hanno generico) " & vbCrLf)
        Stb.Append(VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali_RMAPrincipio(UniqueID) & vbCrLf)

        Stb.Append(" union " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" --seconda query (principi attivi esplicitamente vietati) " & vbCrLf)
        Stb.Append(" select         " & vbCrLf)
        Stb.Append("      T.pivaSuperUser " & vbCrLf)
        Stb.Append("    , T.progressivo  " & vbCrLf)
        Stb.Append("    , ATP.PA_o_Fam_cod as Pa_Cod " & vbCrLf)
        Stb.Append("    , ATP.isFamiglia " & vbCrLf)
        Stb.Append("    , ATP.QtaRilevata " & vbCrLf)
        Stb.Append("    , -1 as RMA " & vbCrLf)
        Stb.Append("    , CC.Capitolato_cod " & vbCrLf)
        Stb.Append("    , -1 as Perc_max_rma " & vbCrLf)
        Stb.Append("    , CC.N_Max_PA  " & vbCrLf)
        Stb.Append("    , CC.Sum_Perc_Max_RMA  " & vbCrLf)
        Stb.Append("    , -1 as percQtaRilevata " & vbCrLf)
        Stb.Append("    , null as validita_inizio    " & vbCrLf)
        Stb.Append("    , 1 as percSuperata " & vbCrLf)
        Stb.Append("    , 'principo attivo vietato da Capitolato Cliente'  collate Latin1_General_CI_AS  as Regolamento " & vbCrLf)
        Stb.Append(" from Analisi_Testata T  " & vbCrLf)
        Stb.Append(" inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
        Stb.Append("    on T.uniqueID = acc.uniqueID " & vbCrLf)
        Stb.Append("    and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
        Stb.Append("    and T.progressivo = ACC.Progressivo      " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" inner join Analisi_TestataXResiduiRilevati ATP  " & vbCrLf)
        Stb.Append("    on T.pivaSuperUser = ATP.pivaSuperUser " & vbCrLf)
        Stb.Append("    and T.progressivo = ATP.Progressivo " & vbCrLf)
        Stb.Append("    and T.UniqueID = ATP.UniqueID  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" inner join capitolatoclienteXPaVietati CCPAR " & vbCrLf)
        Stb.Append("    on CCPAR.capitolato_cod = CC.Capitolato_COD " & vbCrLf)
        Stb.Append("    and CCPAR.pa_o_fam_COD  = ATP.pa_o_fam_COD " & vbCrLf)
        Stb.Append("    and CCPAR.isFamiglia = ATP.isFamiglia " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" where  " & vbCrLf)
        Stb.Append("    t.UniqueID = " & UniqueID & " " & vbCrLf)


        Try

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
    Private Shared Sub create_Tabelle_appggio_DerrateRMA_FAST(ByVal UniqueID As String, ByRef Stb As StringBuilder)



        Stb.Append("            select drpa.* " & vbCrLf)
        Stb.Append("            from ( " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                select distinct " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                         paRMA.PA_COD  " & vbCrLf)
        Stb.Append("                        ,paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("                        ,der.DERR_COD  " & vbCrLf)
        Stb.Append("                        ,der.DERR_CODIFICA  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                from Analisi_Testata T " & vbCrLf)
        Stb.Append("                    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
        Stb.Append("                        on T.uniqueID = acc.uniqueID " & vbCrLf)
        Stb.Append("                        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
        Stb.Append("                        and T.progressivo = ACC.Progressivo                              " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join Analisi_TestataXResiduiRilevati ATP  " & vbCrLf)
        Stb.Append("                        on T.pivaSuperUser = ATP.pivaSuperUser " & vbCrLf)
        Stb.Append("                        and T.progressivo = ATP.Progressivo " & vbCrLf)
        Stb.Append("                        and T.UniqueID = ATP.UniqueID  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join TrascodificaFamiglie_e_Principi papaRMA " & vbCrLf)
        Stb.Append("                        on papaRMA.pa_o_Fam_cod  = ATP.pa_o_fam_COD  " & vbCrLf)
        Stb.Append("                        and papaRMA.isFamiglia = ATP.isFamiglia  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join RegolamentiRMA rRMA " & vbCrLf)
        Stb.Append("                        on rRMA.RMA_REG_COD = papaRMA.RMA_REG_COD " & vbCrLf)
        Stb.Append("                        and rRMA.TabellaRMA_COD  = CC.TabellaRMA_COD " & vbCrLf)
        Stb.Append("                        and rRma.validita_inizio < T.data_analisi " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join PrincipiAttiviRMA  paRMA  " & vbCrLf)
        Stb.Append("                        on papaRMA.PA_COD_RMA = paRMA.PA_Cod  " & vbCrLf)
        Stb.Append("                        and papaRMA.RMA_REG_COD = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join DerrateCodifica  derCodifica      " & vbCrLf)
        Stb.Append("                        on derCodifica.TabellaRMA_COD = cc.TabellaRMA_COD " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join DerrateRMAXSpecieVegetali derVeg      " & vbCrLf)
        Stb.Append("                        on derVeg.veg_cod = T.Veg_cod " & vbCrLf)
        Stb.Append("                        and derVeg.DERR_CODIFICA = derCodifica.DERR_CODIFICA                 " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                    inner join DerrateRMA der  " & vbCrLf)
        Stb.Append("                        on der.DERR_COD = derVeg.DERR_COD  " & vbCrLf)
        Stb.Append("                        and der.DERR_Codifica = derVeg.DERR_Codifica " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                where t.uniqueID =  " & UniqueID & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("            ) paRMA " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("                inner join DerrateRMAXPrincipiAttiviRMA drPA  " & vbCrLf)
        Stb.Append("                    on drPA.PA_COD = paRMA.PA_COD  " & vbCrLf)
        Stb.Append("                    and drPA.RMA_REG_COD = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("                    and drPA.DERR_COD = paRMA.DERR_COD  " & vbCrLf)
        Stb.Append("                    and drPA.DERR_CODIFICA = paRMA.DERR_CODIFICA  " & vbCrLf)


    End Sub
    Public Function completa_Tabelle_appggio(ByVal UniqueID As String,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali()"

        Dim MessaggioErrore As String = ""
        Dim rval As Boolean
        Dim Stb As New StringBuilder

        'tabella Temporanea PercentualiPA
        Stb.Length = 0
        Stb.Append(" insert into PercentualiPA_TEMP " & vbCrLf)


        Stb.AppendLine(" Select ")
        Stb.AppendLine("       T.UniqueID ")
        Stb.AppendLine("     , T.PivaSuperUser  ")
        Stb.AppendLine("     , T.Progressivo  ")
        Stb.AppendLine("     , ATPF.pa_o_fam_COD AS PA_COD  ")
        Stb.AppendLine("     , ATPF.qtaRilevata  ")
        Stb.AppendLine("     , ATPF.isFamiglia  ")
        Stb.AppendLine("     , 0 AS RMA  ")
        Stb.AppendLine("     , CC.Capitolato_COD  ")
        Stb.AppendLine("     , 0 as  Perc_Max_RMA     ")
        Stb.AppendLine("  , CC.N_Max_PA  ")
        Stb.AppendLine("  , CC.Sum_Perc_Max_RMA  ")
        Stb.AppendLine("  , 101 as  percQtaRilevata  ")
        Stb.AppendLine("  , '01/01/1900' Validita_inizio  ")
        Stb.AppendLine("  , CC.TabellaRMA_COD  ")
        Stb.AppendLine("  , 'SA/RMA Non Presente.'  as RMA_REG_DES  ")
        Stb.AppendLine("            ")
        Stb.AppendLine("            ")
        Stb.AppendLine(" From ")
        Stb.AppendLine(" Analisi_Testata As T   ")
        Stb.AppendLine("  ")
        Stb.AppendLine(" INNER Join Analisi_TestataXCapitolatoCliente AS ACC   ")
        Stb.AppendLine(" On T.UniqueID = ACC.UniqueID   ")
        Stb.AppendLine(" And T.PivaSuperUser = ACC.PivaSuperUser   ")
        Stb.AppendLine(" And T.Progressivo = ACC.Progressivo   ")
        Stb.AppendLine("            ")
        Stb.AppendLine(" INNER Join CapitolatoCliente AS CC   ")
        Stb.AppendLine(" On CC.Capitolato_COD = ACC.Capitolato_Cod   ")
        Stb.AppendLine("            ")
        Stb.AppendLine(" INNER Join Analisi_TestataXResiduiRilevati AS ATPF   ")
        Stb.AppendLine(" On T.PivaSuperUser = ATPF.pivaSuperUser   ")
        Stb.AppendLine(" And T.Progressivo = ATPF.Progressivo   ")
        Stb.AppendLine(" And T.UniqueID = ATPF.UniqueID   ")
        Stb.AppendLine("  ")
        Stb.AppendLine(" where Not exists( ")
        Stb.AppendLine("     select 1 ")
        Stb.AppendLine("  From [dbo].[PercentualiPA_TEMP] tmp ")
        Stb.AppendLine("  Where ")
        Stb.AppendLine("         tmp.[UniqueID] = T.[UniqueID] ")
        Stb.AppendLine("  And tmp.[Progressivo] = ACC.Progressivo  ")
        Stb.AppendLine("  And tmp.[PA_COD] = ATPF.pa_o_Fam_cod ")
        Stb.AppendLine("  And tmp.[isFamiglia] = ATPF.isFamiglia ")
        Stb.AppendLine("  And tmp.[Capitolato_COD] = ACC.CApitolato_Cod ")
        Stb.AppendLine(" )")


        Stb.Append(" AND T.uniqueID = " & UniqueID & vbCrLf)
        Stb.Append(" ")


        Stb.Append(" " & vbCrLf)

        Try

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        Return rval

    End Function


    Public Function create_Tabelle_appggio(ByVal UniqueID As String, _
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali()"

        Dim MessaggioErrore As String = ""
        Dim rval As Boolean
        Dim Stb As New StringBuilder

        'tabella Temporanea PercentualiPA
        Stb.Length = 0
        Stb.Append(" insert into PercentualiPA_TEMP " & vbCrLf)


        Stb.Append("SELECT    " & vbCrLf)
        Stb.Append("      T.UniqueID " & vbCrLf)
        Stb.Append("    , T.PivaSuperUser " & vbCrLf)
        Stb.Append("    , T.Progressivo " & vbCrLf)
        Stb.Append("    , ATPF.pa_o_fam_COD AS PA_COD " & vbCrLf)
        Stb.Append("    , ATPF.qtaRilevata " & vbCrLf)
        Stb.Append("    , ATPF.isFamiglia " & vbCrLf)
        Stb.Append("    , CAST(drPA1.RMA AS real) AS RMA " & vbCrLf)
        Stb.Append("    , CC.Capitolato_COD " & vbCrLf)

        Stb.Append("    ,  " & vbCrLf)
        Stb.Append("        case when ATPF.isFamiglia = 0 then " & vbCrLf)
        Stb.Append("            case when ccpar.LMR is null then  " & vbCrLf)
        Stb.Append("                cc.Perc_Max_RMA " & vbCrLf)
        Stb.Append("            else  " & vbCrLf)
        Stb.Append("                ccpar.LMR " & vbCrLf)
        Stb.Append("         End " & vbCrLf)
        Stb.Append("        else " & vbCrLf)
        Stb.Append("            case when ccparfam.LMR is null then  " & vbCrLf)
        Stb.Append("                cc.Perc_Max_RMA " & vbCrLf)
        Stb.Append("            else  " & vbCrLf)
        Stb.Append("                ccparfam.LMR " & vbCrLf)
        Stb.Append("         End " & vbCrLf)
        Stb.Append("        end as Perc_Max_RMA" & vbCrLf)


        Stb.Append("    , CC.N_Max_PA " & vbCrLf)
        Stb.Append("    , CC.Sum_Perc_Max_RMA " & vbCrLf)
        'Stb.Append("    , CASE WHEN drPA1.RMA = 0 THEN CAST((ATPF.QtaRilevata * 100) AS real)  " & vbCrLf)
        Stb.Append("    , CASE WHEN drPA1.RMA = 0 THEN 0  " & vbCrLf)
        Stb.Append("        ELSE CAST((ATPF.QtaRilevata * 100 / drPA1.RMA) AS real)  " & vbCrLf)
        Stb.Append("    END AS percQtaRilevata " & vbCrLf)
        Stb.Append("    , rRma.Validita_inizio " & vbCrLf)
        Stb.Append("    , rRma.TabellaRMA_COD " & vbCrLf)
        Stb.Append("    , rRma.RMA_REG_DES + isnull(' - ' + drPa1.Note collate latin1_general_ci_as, '')  as RMA_REG_DES " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" FROM " & vbCrLf)
        Stb.Append("    Analisi_Testata AS T  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN Analisi_TestataXCapitolatoCliente AS ACC  " & vbCrLf)
        Stb.Append("    ON T.UniqueID = ACC.UniqueID  " & vbCrLf)
        Stb.Append("    AND T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
        Stb.Append("    AND T.Progressivo = ACC.Progressivo  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN CapitolatoCliente AS CC  " & vbCrLf)
        Stb.Append("    ON CC.Capitolato_COD = ACC.Capitolato_Cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN Analisi_TestataXResiduiRilevati AS ATPF  " & vbCrLf)
        Stb.Append("    ON T.PivaSuperUser = ATPF.pivaSuperUser  " & vbCrLf)
        Stb.Append("    AND T.Progressivo = ATPF.Progressivo  " & vbCrLf)
        Stb.Append("    AND T.UniqueID = ATPF.UniqueID  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN TrascodificaFamiglie_e_Principi AS papaRMA  " & vbCrLf)
        Stb.Append("    ON papaRMA.pa_o_Fam_cod = ATPF.pa_o_fam_COD  " & vbCrLf)
        Stb.Append("    AND papaRMA.isFamiglia = ATPF.isFamiglia  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN RegolamentiRMA AS rRma  " & vbCrLf)
        Stb.Append("    ON rRma.RMA_REG_COD = papaRMA.rma_reg_cod  " & vbCrLf)
        Stb.Append("    AND rRma.TabellaRMA_COD = CC.TabellaRMA_COD  " & vbCrLf)
        Stb.Append("    AND rRma.Validita_inizio < T.Data_Analisi  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN PrincipiAttiviRMA AS paRMA  " & vbCrLf)
        Stb.Append("    ON papaRMA.pa_COD_RMA = paRMA.PA_COD  " & vbCrLf)
        Stb.Append("    AND papaRMA.rma_reg_cod = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN DerrateCodifica AS derCodifica  " & vbCrLf)
        Stb.Append("    ON derCodifica.TabellaRMA_COD = CC.TabellaRMA_COD  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN DerrateRMAXSpecieVegetali AS derVeg  " & vbCrLf)
        Stb.Append("    ON derVeg.Veg_cod = T.Veg_Cod  " & vbCrLf)
        Stb.Append("    AND derVeg.DERR_CODIFICA = derCodifica.DERR_CODIFICA  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN DerrateRMA AS der  " & vbCrLf)
        Stb.Append("    ON der.DERR_COD = derVeg.DERR_COD  " & vbCrLf)
        Stb.Append("    AND der.DERR_CODIFICA = derVeg.DERR_CODIFICA  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("            ( " & vbCrLf)

        create_Tabelle_appggio_DerrateRMA_FAST(UniqueID, Stb)

        Stb.Append("        ) drPa1 " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("        on drPA1.PA_COD = paRMA.PA_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.RMA_REG_COD = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.DERR_COD = der.DERR_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.DERR_CODIFICA = der.DERR_CODIFICA  " & vbCrLf)
        Stb.Append("  " & vbCrLf)


        Stb.Append("left join CapitolatoClienteXPrincipiAttiviRilevati ccpar  " & vbCrLf)
        Stb.Append("        on cc.capitolato_cod = ccpar.capitolato_cod          " & vbCrLf)
        Stb.Append("        AND ATPF.pa_o_fam_COD = ccpar.PA_COD " & vbCrLf)
        Stb.Append("        and ATPF.isFamiglia = 0 " & vbCrLf)
        Stb.Append("        and ccpar.LMR<>0     " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" left join CapitolatoClienteXFamigliePrincipiAttiviRilevati ccparFam  " & vbCrLf)
        Stb.Append("        on cc.capitolato_cod = ccparFam.capitolato_cod          " & vbCrLf)
        Stb.Append("        AND ATPF.pa_o_fam_COD = ccparFam.FAM_COD " & vbCrLf)
        Stb.Append("        and ATPF.isFamiglia = 1 " & vbCrLf)
        Stb.Append("        and ccparFam.LMR<>0     " & vbCrLf)
        Stb.Append(" ")


        Stb.Append(" where t.uniqueID = " & UniqueID & vbCrLf)
        Stb.Append(" ")


        Stb.Append(" " & vbCrLf)

        Try

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


        Stb.Length = 0
        Stb.Append("  insert into SommaPercentuali_TEMP " & vbCrLf)


        Stb.Append("select  " & vbCrLf)
        Stb.Append("      T.UniqueID   " & vbCrLf)
        Stb.Append("    , T.PivaSuperUser " & vbCrLf)
        Stb.Append("    , T.Progressivo  " & vbCrLf)
        Stb.Append("    , CC.Capitolato_COD " & vbCrLf)
        Stb.Append("    , CC.Sum_Perc_Max_RMA " & vbCrLf)
        Stb.Append("    , drPA1.RMA " & vbCrLf)
        Stb.Append("    , case when coalesce(ccxppa.partecipaSommatoria, 1) = 0 then 0 else ATP.QtaRilevata end as QtaRilevata " & vbCrLf)
        Stb.Append("    , ATP.PA_o_Fam_Cod as PA_COD     " & vbCrLf)
        Stb.Append("    , ATP.isFamiglia " & vbCrLf)
        Stb.Append("    , rRMA.TabellaRMA_COD  " & vbCrLf)
        Stb.Append("    , rRMA.validita_inizio " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" from Analisi_Testata T " & vbCrLf)
        Stb.Append("    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
        Stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
        Stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
        Stb.Append("        and T.progressivo = ACC.Progressivo                              " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join Analisi_TestataXResiduiRilevati ATP  " & vbCrLf)
        Stb.Append("        on T.pivaSuperUser = ATP.pivaSuperUser " & vbCrLf)
        Stb.Append("        and T.progressivo = ATP.Progressivo " & vbCrLf)
        Stb.Append("        and T.UniqueID = ATP.UniqueID  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join TrascodificaFamiglie_e_Principi papaRMA " & vbCrLf)
        Stb.Append("        on papaRMA.pa_o_Fam_cod  = ATP.pa_o_fam_COD  " & vbCrLf)
        Stb.Append("        and papaRMA.isFamiglia = ATP.isFamiglia  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join RegolamentiRMA rRMA " & vbCrLf)
        Stb.Append("        on rRMA.RMA_REG_COD = papaRMA.RMA_REG_COD " & vbCrLf)
        Stb.Append("        and rRMA.TabellaRMA_COD  = CC.TabellaRMA_COD " & vbCrLf)
        Stb.Append("        and rRma.validita_inizio < T.data_analisi " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join PrincipiAttiviRMA  paRMA  " & vbCrLf)
        Stb.Append("        on papaRMA.PA_COD_RMA = paRMA.PA_Cod  " & vbCrLf)
        Stb.Append("        and papaRMA.RMA_REG_COD = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join DerrateCodifica  derCodifica      " & vbCrLf)
        Stb.Append("        on derCodifica.TabellaRMA_COD = cc.TabellaRMA_COD " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join DerrateRMAXSpecieVegetali derVeg      " & vbCrLf)
        Stb.Append("        on derVeg.veg_cod = T.Veg_cod " & vbCrLf)
        Stb.Append("        and derVeg.DERR_CODIFICA = derCodifica.DERR_CODIFICA                 " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join DerrateRMA der  " & vbCrLf)
        Stb.Append("        on der.DERR_COD = derVeg.DERR_COD  " & vbCrLf)
        Stb.Append("        and der.DERR_Codifica = derVeg.DERR_Codifica " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    inner join  " & vbCrLf)



        Stb.Append("            ( " & vbCrLf)

        'vanni, 20/02/2014..: gestione rapida estrazione da tabella DerrateRMAXPrincipiAttiviRMA
        create_Tabelle_appggio_DerrateRMA_FAST(UniqueID, Stb)


        Stb.Append("        ) drPa1 " & vbCrLf)


        Stb.Append("  " & vbCrLf)
        Stb.Append("        on drPA1.PA_COD = paRMA.PA_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.RMA_REG_COD = paRMA.RMA_REG_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.DERR_COD = der.DERR_COD  " & vbCrLf)
        Stb.Append("                    and drPA1.DERR_CODIFICA = der.DERR_CODIFICA  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("    left join   CapitolatoClienteXPercentuali ccxppa     " & vbCrLf)
        Stb.Append("        on ccxppa.capitolato_Cod = cc.Capitolato_COD  " & vbCrLf)
        Stb.Append("    and cast(ccxppa.pa_o_fam_COD as nvarchar(100)) = cast(ATP.PA_o_fam_COD as nvarchar(100)) " & vbCrLf)
        Stb.Append("    and ccxppa.isFamiglia = ATP.isFamiglia   " & vbCrLf)
        Stb.Append("    and ccxppa.partecipaSommatoria = 0  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" where T.uniqueID = " & UniqueID & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" ")


        Try

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try



        'tabella Temporanea #x_SenzaFormulatoValido
        Stb.Length = 0
        Stb.Append(" insert  into SenzaFormulatoValido_TEMP " & vbCrLf)
        Stb.Append(" select distinct C.PivaSuperUser, C.Capitolato_Cod, c.Progressivo, c.UniqueID  " & vbCrLf)
        Stb.Append("         , case when ATfP.FAM_COD is not null then ATfP.FAM_COD  else C.PA_COD  end as pa_o_fam_COD  " & vbCrLf)
        Stb.Append("         , case when ATfP.FAM_COD is not null then 1 else 0 end as isFamiglia  " & vbCrLf)

        Stb.Append("     from   " & vbCrLf)
        Stb.Append("     (  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("         select  distinct   " & vbCrLf)
        Stb.Append("               T.PivaSuperUser  " & vbCrLf)
        Stb.Append("             , T.Progressivo  " & vbCrLf)
        Stb.Append("             , T.UniqueID  " & vbCrLf)
        Stb.Append("             , ACC.Capitolato_Cod   " & vbCrLf)
        Stb.Append("             , paRil.PA_COD   " & vbCrLf)
        Stb.Append("         from Analisi_Testata T  " & vbCrLf)
        Stb.Append("             inner join Analisi_Testata_AnnataAgraria AnAG  " & vbCrLf)
        Stb.Append("                 on T.uniqueID = AnAG.uniqueID  " & vbCrLf)
        Stb.Append("                 and T.PivaSuperUser = AnAG.PivaSuperUser   " & vbCrLf)
        Stb.Append("                 and T.progressivo = AnAG.Progressivo          " & vbCrLf)
        Stb.Append("                 and T.UniqueID =  " & UniqueID & " " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             inner join Analisi_TestataXCapitolatoCliente ACC   " & vbCrLf)
        Stb.Append("                 on T.uniqueID = acc.uniqueID  " & vbCrLf)
        Stb.Append("                 and T.PivaSuperUser = ACC.PivaSuperUser   " & vbCrLf)
        Stb.Append("                 and T.progressivo = ACC.Progressivo  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             inner join Analisi_TestataXResiduiRilevati_FamiglieEsploseInPA paRil  " & vbCrLf)
        Stb.Append("                 on paRil.PivaSuperUser = T.PivaSuperUser   " & vbCrLf)
        Stb.Append("                 and paRil.Progressivo = T.Progressivo  " & vbCrLf)
        Stb.Append("                 and paRil.UniqueID = T.UniqueID  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             inner join (  " & vbCrLf)

        ' VAnni: 25/1/2021: sui principi attivi di un contesto diverso da 1 seleziono un formulato validi "fittizio"
        Stb.AppendLine("        Select  ")
        Stb.AppendLine("       -1 as Fr_Cod ")
        Stb.AppendLine("     , paxpac.Pa_Cod ")
        Stb.AppendLine("     , 0 as Revocato ")
        Stb.AppendLine("     , null as Data_Revo ")
        Stb.AppendLine("     , null as DataSospensioneDA ")
        Stb.AppendLine("     , null as DataSospensioneA ")
        Stb.AppendLine("     , (select top 1 veg_cod ")
        Stb.AppendLine("         From Analisi_Testata att ")
        Stb.AppendLine("         Where UniqueID =  " & UniqueID)
        Stb.AppendLine("         ) Veg_Cod ")
        Stb.AppendLine("      , '01/01/1900' as Validita_Inizio  ")
        Stb.AppendLine("      , '31/12/2100' as Validita_Fine ")
        Stb.AppendLine(" From PrincipiAttiviXPrincipiAttivi_Contesto paxpac ")
        Stb.AppendLine(" Where paxpac.PrincipiAttivi_Contesto_COD <> 1")
        Stb.AppendLine(" union ")

        Stb.Append("                 select FPA.Fr_Cod, fpa.Pa_Cod, F.Revocato, F.Data_Revo, FS.DataSospensioneDA, FS.DataSospensioneA, FVEG.Veg_Cod, FVEG.Validita_Inizio , FVEG.Validita_Fine " & vbCrLf)
        Stb.Append("                 from formulatiXPrincipiAttivi FPA  " & vbCrLf)

        'Non serve ma tengo commentata
        'Stb.Append("                 from  ( " & vbCrLf)
        'Stb.Append("                     select Fr_Cod, fxpa1.pa_cod " & vbCrLf)
        'Stb.Append("                     from formulatiXprincipiAttivi fxpa1 " & vbCrLf)
        'Stb.Append("                     inner Join PrincipiAttiviXPrincipiAttivi_Contesto paxpac " & vbCrLf)
        'Stb.Append("                         On fxpa1.Pa_Cod = paxpac.PA_COD " & vbCrLf)
        'Stb.Append("                         And paxpac.PrincipiAttivi_Contesto_COD = 1" & vbCrLf)
        'Stb.Append("                 ) FPA  " & vbCrLf)

        Stb.Append("                     inner join formulati F   " & vbCrLf)
        Stb.Append("                         on FPA.fr_cod = F.fr_Cod  " & vbCrLf)
        Stb.Append("                     left join FormulatiXPeriodoSospensione FS   " & vbCrLf)
        Stb.Append("                         on FS.fr_cod = F.fr_cod       " & vbCrLf)
        Stb.Append("                     inner join FormulatiXSpecieVegetali_GruppiEsplosiInSpecie FVEG                " & vbCrLf)
        Stb.Append("                         on F.Fr_Cod = FVEG.Fr_Cod   " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             ) F on F.Pa_Cod = paRil.pa_cod        " & vbCrLf)
        Stb.Append("             and F.Veg_cod = T.Veg_cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("             and (F.Revocato = 0 or  " & vbCrLf)
        Stb.Append("                 coalesce(F.Data_Revo, cast('01/01/2100' as datetime)) > T.Data_Analisi  " & vbCrLf)
        Stb.Append("             )  " & vbCrLf)
        Stb.Append("             and (  " & vbCrLf)
        Stb.Append("                 coalesce(F.DataSospensioneA, cast('01/01/1900' as datetime)) < AnAG.InizioAnnataAgraria  and  " & vbCrLf)
        Stb.Append("                 coalesce(F.DataSospensioneDA, cast('01/01/2100' as datetime)) > AnAG.FineAnnataAgraria  " & vbCrLf)
        Stb.Append("                 )  " & vbCrLf)
        Stb.Append("             and (  " & vbCrLf)
        Stb.Append("                 coalesce(F.Validita_Inizio, cast('01/01/1900' as datetime)) < T.Data_Analisi  and  " & vbCrLf)
        Stb.Append("                 coalesce(F.Validita_Fine, cast('01/01/2100' as datetime)) > T.Data_Analisi  " & vbCrLf)
        Stb.Append("                 )  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        'Stb.Append("             inner join FormulatiXSpecieVegetali_GruppiEsplosiInSpecie FVEG   " & vbCrLf)
        Stb.AppendLine("        inner Join( ")
        Stb.AppendLine("                     select * ")
        Stb.AppendLine("                     From FormulatiXSpecieVegetali_GruppiEsplosiInSpecie ")
        Stb.AppendLine("                     union all ")
        Stb.AppendLine("                     Select -1, ( ")
        Stb.AppendLine("                         Select top 1 veg_cod  ")
        Stb.AppendLine("                         From Analisi_Testata att  ")
        Stb.AppendLine("                         Where UniqueID =  " & UniqueID)
        Stb.AppendLine("                     ) Veg_Cod  ")
        Stb.AppendLine("                     , '01/01/1900' as validita_inizio ")
        Stb.AppendLine("                     , '31/12/2100' as validita_fine ")
        Stb.AppendLine("                 ) FVEG   ")

        Stb.Append("             on F.fr_Cod = FVEG.fr_cod  " & vbCrLf)
        Stb.Append("             and FVEG.Veg_cod = T.Veg_cod          " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append("     ) C  " & vbCrLf)
        Stb.Append("     left join PrincipiAttiviXFamigliePrincipiAttivi xDecodeFAm  " & vbCrLf)
        'Stb.Append("     inner join PrincipiAttivi xDecodeFAm  " & vbCrLf)
        Stb.Append("         on C.pa_cod = xDecodeFAm.pa_cod          " & vbCrLf)
        Stb.Append("     left join Analisi_TestataXFamigliePrincipiAttiviRilevati ATfP  " & vbCrLf)
        Stb.Append("         on xDecodeFAm.Fam_COD = ATfP.FAM_COD   " & vbCrLf)
        Stb.Append("             and c.PivaSuperUser = ATfP.PivaSuperUser   " & vbCrLf)
        Stb.Append("             and c.UniqueID = ATfP.UniqueID   " & vbCrLf)
        Stb.Append("             and c.Progressivo = ATFP.Progressivo   " & vbCrLf)
        Stb.Append("     left join Analisi_TestataXPrincipiAttiviRilevati ATP  " & vbCrLf)
        Stb.Append("         on C.pa_cod = ATP.PA_COD   " & vbCrLf)
        Stb.Append("             and c.PivaSuperUser = ATfP.PivaSuperUser   " & vbCrLf)
        Stb.Append("             and c.UniqueID = ATfP.UniqueID   " & vbCrLf)
        Stb.Append("             and c.Progressivo = ATFP.Progressivo " & vbCrLf)
        Stb.Append(" where C.uniqueID =  " & UniqueID & " " & vbCrLf)
        Stb.Append(" " & vbCrLf)

        Try

            '--------------------------------------------------------------------------
            rval = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            rval = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return rval

    End Function
    Public Function drop_Tabella_appggio(ByVal UniqueID As String, _
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.drop_Tabella_appggio()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder

        'tabella Temporanea



        Stb.Append("    delete from PercentualiPA_TEMP where UniqueID = " & UniqueID)
        Stb.Append("    delete from SenzaFormulatoValido_TEMP where UniqueID = " & UniqueID)


        Stb.Append("    delete from SommaPercentuali_TEMP where UniqueID = " & UniqueID)




        Try

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


    'Public Function VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali(ByVal UniqueID As String, _
    '                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaCapitolato_ElencoPrincipiAttiviRMAPercentuali()"

    '    Dim MessaggioErrore As String = ""
    '    Dim DT As DataTable = Nothing
    '    Dim Stb As New StringBuilder

    '    'query da: GIAS_capitolatoCliente.__analisi_verifica_Capitolato_PA_PercSingolo.sql


    '    Stb.Append(" select  " & vbCrLf)
    '    Stb.Append("      T.pivaSuperUser " & vbCrLf)
    '    Stb.Append("    , T.progressivo  " & vbCrLf)
    '    Stb.Append("    , T.PA_Cod " & vbCrLf)
    '    Stb.Append("    , T.isFamiglia " & vbCrLf)
    '    Stb.Append("    , T.QtaRilevata " & vbCrLf)
    '    Stb.Append("    , coalesce(T.RMA, -1) as RMA  " & vbCrLf)
    '    Stb.Append("    , T.Capitolato_cod " & vbCrLf)
    '    Stb.Append("    , T.Perc_max_rma " & vbCrLf)
    '    Stb.Append("    , T.N_Max_PA  " & vbCrLf)
    '    Stb.Append("    , T.Sum_Perc_Max_RMA  " & vbCrLf)
    '    Stb.Append("    , T.percQtaRilevata " & vbCrLf)
    '    Stb.Append("    , T.validita_inizio  " & vbCrLf)
    '    Stb.Append("    , case when cast(t.RMA as real) = 0 or case when t.RMA = 0 then (t.QtaRilevata * 100) else cast(t.QtaRilevata * 100 / cast(t.RMA as real) as real) end > T.Perc_Max_RMA	then 1 else 0 end as PercSuperata " & vbCrLf)
    '    Stb.Append("    , case when T.TabellaRMA_COD = 1 then T.RMA_REG_DES else '-' end as Regolamento  " & vbCrLf)
    '    Stb.Append(" from Analisi_Testata TES  " & vbCrLf)

    '    Stb.Append("    inner join PercentualiPA  T " & vbCrLf)
    '    Stb.Append("        on TES.UniqueID = T.UniqueID  " & vbCrLf)
    '    Stb.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
    '    Stb.Append("        and T.Progressivo = TES.Progressivo                  " & vbCrLf)
    '    Stb.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)


    '    Stb.Append("    inner join ( " & vbCrLf)
    '    Stb.Append("        select PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod, Capitolato_Cod, max(Validita_inizio) as validita_inizio " & vbCrLf)
    '    Stb.Append("        from PercentualiPA " & vbCrLf)
    '    Stb.Append("        where uniqueID = " & UniqueID & " " & vbCrLf)
    '    Stb.Append("        group by PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod, Capitolato_Cod  " & vbCrLf)
    '    Stb.Append("    ) t1 " & vbCrLf)
    '    Stb.Append(" on t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
    '    Stb.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
    '    Stb.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
    '    Stb.Append(" and t1.Capitolato_Cod = t.capitolato_COD " & vbCrLf)
    '    Stb.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
    '    Stb.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" where TES.uniqueID = " & UniqueID & " " & vbCrLf)
    '    'Stb.Append(" and ( " & vbCrLf)
    '    'Stb.Append("    cast(t.RMA as real) = 0 or " & vbCrLf)
    '    'Stb.Append("    case when t.RMA = 0 then (t.QtaRilevata * 100) else cast(t.QtaRilevata * 100 / cast(t.RMA as real) as real) end > T.Perc_Max_RMA" & vbCrLf)
    '    'Stb.Append("  )" & vbCrLf)
    '    Stb.Append(" union " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" --seconda query (principi attivi esplicitamente vietati) " & vbCrLf)
    '    Stb.Append(" select         " & vbCrLf)
    '    Stb.Append("      T.pivaSuperUser " & vbCrLf)
    '    Stb.Append("    , T.progressivo  " & vbCrLf)
    '    Stb.Append("    , ATP.PA_o_Fam_cod as Pa_Cod " & vbCrLf)
    '    Stb.Append("    , ATP.isFamiglia " & vbCrLf)
    '    Stb.Append("    , ATP.QtaRilevata " & vbCrLf)
    '    Stb.Append("    , -1 as RMA " & vbCrLf)
    '    Stb.Append("    , CC.Capitolato_cod " & vbCrLf)
    '    Stb.Append("    , -1 as Perc_max_rma " & vbCrLf)
    '    Stb.Append("    , CC.N_Max_PA  " & vbCrLf)
    '    Stb.Append("    , CC.Sum_Perc_Max_RMA  " & vbCrLf)
    '    Stb.Append("    , -1 as percQtaRilevata " & vbCrLf)
    '    Stb.Append("    , null as validita_inizio    " & vbCrLf)
    '    Stb.Append("    , 1 as percSuperata " & vbCrLf)
    '    Stb.Append("    , 'principo attivo vietato da Capitolato Cliente' as Regolamento " & vbCrLf)
    '    Stb.Append(" from Analisi_Testata T  " & vbCrLf)
    '    Stb.Append(" inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
    '    Stb.Append("    on T.uniqueID = acc.uniqueID " & vbCrLf)
    '    Stb.Append("    and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
    '    Stb.Append("    and T.progressivo = ACC.Progressivo      " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" inner join Analisi_TestataXResiduiRilevati ATP  " & vbCrLf)
    '    Stb.Append("    on T.pivaSuperUser = ATP.pivaSuperUser " & vbCrLf)
    '    Stb.Append("    and T.progressivo = ATP.Progressivo " & vbCrLf)
    '    Stb.Append("    and T.UniqueID = ATP.UniqueID  " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" inner join capitolatoclienteXPaVietati CCPAR " & vbCrLf)
    '    Stb.Append("    on CCPAR.capitolato_cod = CC.Capitolato_COD " & vbCrLf)
    '    Stb.Append("    and CCPAR.pa_o_fam_COD  = ATP.pa_o_fam_COD " & vbCrLf)
    '    Stb.Append("    and CCPAR.isFamiglia = ATP.isFamiglia " & vbCrLf)
    '    Stb.Append("  " & vbCrLf)
    '    Stb.Append(" where  " & vbCrLf)
    '    Stb.Append("    t.UniqueID = " & UniqueID & " ")

    '    Try

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function


    Public Function VerificaCapitolato_VerificaPABIO(ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente.VerificaCapitolato_VerificaPABIO()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder

        'query da: GIAS_capitolatoCliente.__analisi_verifica_Capitolato_PA_EsisteInFormulato.sql


        Try



            Stb.Append("select distinct A.PivaSuperuser, A.Progressivo, A.UniqueID, A.Capitolato_cod, A.pa_o_fam_COD, A.isFamiglia " & vbCrLf)
            Stb.Append("from ( " & vbCrLf)
            Stb.Append("select T.PivaSuperuser, T.Progressivo, T.UniqueID, T.pa_o_fam_COD, T.isFamiglia, ACC.Capitolato_cod " & vbCrLf)
            Stb.Append("from Analisi_TestataXResiduiRilevati T " & vbCrLf)
            Stb.Append("inner join Analisi_TestataXCapitolatoCliente ACC " & vbCrLf)
            Stb.Append("on T.uniqueID = acc.uniqueID " & vbCrLf)
            Stb.Append("and T.PivaSuperUser = ACC.PivaSuperUser " & vbCrLf)
            Stb.Append("and T.progressivo = ACC.Progressivo " & vbCrLf)
            Stb.Append("inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod " & vbCrLf)

            Stb.Append("where T.UniqueID = " & UniqueID & "   " & vbCrLf)
            Stb.Append("and cc.flagbio = 1 " & vbCrLf)
            Stb.Append(") A " & vbCrLf)

            Stb.Append(" inner join Analisi_TestataXResiduiRilevati_FamiglieEsploseInPA paRil " & vbCrLf)
            Stb.Append(" on paRil.PivaSuperUser = A.PivaSuperUser  " & vbCrLf)
            Stb.Append(" and paRil.Progressivo = A.Progressivo " & vbCrLf)
            Stb.Append(" and paRil.UniqueID = A.UniqueID " & vbCrLf)
            Stb.Append(" and A.pa_o_fam_cod = paRil.pa_cod " & vbCrLf)

            Stb.Append("inner join PrincipiAttivi xDecodeFAm " & vbCrLf)
            Stb.Append("on paRil.pa_cod = xDecodeFAm.pa_cod  " & vbCrLf)
            Stb.Append("left join PrincipiAttivi_BIO PB " & vbCrLf)
            Stb.Append("on pb.pa_cod = xDecodeFAm.pa_cod " & vbCrLf)
            Stb.Append("where( " & vbCrLf)
            Stb.Append(" pb.pa_cod Is null and isFamiglia = 0)  " & vbCrLf)



            Stb.Append(" union  " & vbCrLf)



            Stb.Append("select distinct A.PivaSuperuser, A.Progressivo, A.UniqueID, A.Capitolato_cod, A.pa_o_fam_COD, A.isFamiglia " & vbCrLf)
            Stb.Append("from ( " & vbCrLf)
            Stb.Append("select T.PivaSuperuser, T.Progressivo, T.UniqueID, T.pa_o_fam_COD, T.isFamiglia, ACC.Capitolato_cod " & vbCrLf)
            Stb.Append("from Analisi_TestataXResiduiRilevati T " & vbCrLf)
            Stb.Append("inner join Analisi_TestataXCapitolatoCliente ACC " & vbCrLf)
            Stb.Append("on T.uniqueID = acc.uniqueID " & vbCrLf)
            Stb.Append("and T.PivaSuperUser = ACC.PivaSuperUser " & vbCrLf)
            Stb.Append("and T.progressivo = ACC.Progressivo " & vbCrLf)
            Stb.Append("inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod " & vbCrLf)

            Stb.Append("where T.UniqueID = " & UniqueID & "   " & vbCrLf)
            Stb.Append("and cc.flagbio = 1 " & vbCrLf)
            Stb.Append(") A " & vbCrLf)

            Stb.Append(" inner join Analisi_TestataXResiduiRilevati_FamiglieEsploseInPA paRil " & vbCrLf)
            Stb.Append(" on paRil.PivaSuperUser = A.PivaSuperUser  " & vbCrLf)
            Stb.Append(" and paRil.Progressivo = A.Progressivo " & vbCrLf)
            Stb.Append(" and paRil.UniqueID = A.UniqueID " & vbCrLf)

            Stb.Append("inner join PrincipiAttivi xDecodeFAm " & vbCrLf)
            Stb.Append("on paRil.pa_cod = xDecodeFAm.pa_cod  " & vbCrLf)
            Stb.Append("left join PrincipiAttivi_BIO PB " & vbCrLf)
            Stb.Append("on pb.pa_cod = xDecodeFAm.pa_cod " & vbCrLf)
            Stb.Append("where " & vbCrLf)
            Stb.Append(" ( isFamiglia = 1 and (1 = (select top 1 1 from principiattivi where fam_cod = A.pa_o_fam_COD and pa_cod not in (select pa_cod  from PrincipiAttivi_BIO)))  " & vbCrLf)
            Stb.Append(" )  " & vbCrLf)
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


    Public Function VerificaCapitolato_ElencoPA_SenzaFormulatoValido_FAST(ByVal UniqueID As String, _
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_ElencoPA_SenzaFormulatoValido_FAST"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder

        'query da: GIAS_capitolatoCliente.__analisi_verifica_Capitolato_PA_EsisteInFormulato.sql


        Try
            Stb.Append("--  " & NomeRoutine & vbCrLf)
            Stb.Append("select distinct A.PivaSuperuser, A.Progressivo, A.UniqueID, A.Capitolato_cod, A.pa_o_fam_COD, A.isFamiglia  " & vbCrLf)
            Stb.Append(" from ( " & vbCrLf)
            Stb.Append(" select T.PivaSuperuser, T.Progressivo, T.UniqueID, T.pa_o_fam_COD, T.isFamiglia, ACC.Capitolato_cod  " & vbCrLf)
            Stb.Append(" from Analisi_TestataXResiduiRilevati T " & vbCrLf)
            Stb.Append(" inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            Stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
            Stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            Stb.Append("        and T.progressivo = ACC.Progressivo " & vbCrLf)
            Stb.Append(" inner join CapitolatoCliente c " & vbCrLf)
            Stb.Append("        on c.Capitolato_Cod = ACC.Capitolato_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where T.UniqueID = " & UniqueID & "  " & vbCrLf)

            Stb.Append("  and C.attivaverificaformulatovalido = 1" & vbCrLf)

            Stb.Append("    ) A " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" left join ( " & vbCrLf)
            Stb.Append("  " & vbCrLf)

            Stb.Append("        select  * from SenzaFormulatoValido_TEMP  " & vbCrLf)
            Stb.Append(" ) B  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" on A.PivasuperUSer = B.PivasuperUser " & vbCrLf)
            Stb.Append(" AND A.progressivo = B.Progressivo  " & vbCrLf)
            Stb.Append(" AND A.UniqueID = B.UniqueID  " & vbCrLf)
            Stb.Append(" AND A.pa_o_fam_COD = B.pa_o_fam_COD  " & vbCrLf)
            Stb.Append(" AND A.isFamiglia = B.isFamiglia " & vbCrLf)
            Stb.Append(" AND A.capitolato_cod = B.capitolato_cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where  B.pa_o_fam_COD is null " & vbCrLf)





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

    'Public Function VerificaCapitolato_ElencoPA_SenzaFormulatoValido(ByVal UniqueID As String, _
    '                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaCapitolato_ElencoPA_SenzaFormulatoValido()"

    '    Dim MessaggioErrore As String = ""
    '    Dim DT As DataTable = Nothing
    '    Dim Stb As New StringBuilder

    '    'query da: GIAS_capitolatoCliente.__analisi_verifica_Capitolato_PA_EsisteInFormulato.sql


    '    Try

    '        Stb.Append("select distinct A.PivaSuperuser, A.Progressivo, A.UniqueID, A.Capitolato_cod, A.pa_o_fam_COD, A.isFamiglia  " & vbCrLf)
    '        Stb.Append(" from ( " & vbCrLf)
    '        Stb.Append(" select T.PivaSuperuser, T.Progressivo, T.UniqueID, T.pa_o_fam_COD, T.isFamiglia, ACC.Capitolato_cod  " & vbCrLf)
    '        Stb.Append(" from Analisi_TestataXResiduiRilevati T " & vbCrLf)
    '        Stb.Append(" inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
    '        Stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
    '        Stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("        and T.progressivo = ACC.Progressivo " & vbCrLf)
    '        Stb.Append(" where T.UniqueID = " & UniqueID & "  " & vbCrLf)
    '        Stb.Append("    ) A " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append(" left join ( " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("    select C.PivaSuperUser, C.Capitolato_Cod, c.Progressivo, c.UniqueID " & vbCrLf)
    '        Stb.Append("        , case when ATfP.FAM_COD is not null then ATfP.FAM_COD  else C.PA_COD  end as pa_o_fam_COD " & vbCrLf)
    '        Stb.Append("        , case when ATfP.FAM_COD is not null then 1 else 0 end as isFamiglia " & vbCrLf)
    '        Stb.Append("    from  " & vbCrLf)
    '        Stb.Append("    ( " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("        select  distinct  " & vbCrLf)
    '        Stb.Append("              T.PivaSuperUser " & vbCrLf)
    '        Stb.Append("            , T.Progressivo " & vbCrLf)
    '        Stb.Append("            , T.UniqueID " & vbCrLf)
    '        Stb.Append("            , ACC.Capitolato_Cod  " & vbCrLf)
    '        Stb.Append("            , paRil.PA_COD  " & vbCrLf)
    '        Stb.Append("        from Analisi_Testata T " & vbCrLf)
    '        Stb.Append("            inner join Analisi_Testata_AnnataAgraria AnAG " & vbCrLf)
    '        Stb.Append("                on T.uniqueID = AnAG.uniqueID " & vbCrLf)
    '        Stb.Append("                and T.PivaSuperUser = AnAG.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("                and T.progressivo = AnAG.Progressivo         " & vbCrLf)
    '        Stb.Append("                and T.UniqueID =  " & UniqueID & " " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
    '        Stb.Append("                on T.uniqueID = acc.uniqueID " & vbCrLf)
    '        Stb.Append("                and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("                and T.progressivo = ACC.Progressivo " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            inner join Analisi_TestataXResiduiRilevati_FamiglieEsploseInPA paRil " & vbCrLf)
    '        Stb.Append("                on paRil.PivaSuperUser = T.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("                and paRil.Progressivo = T.Progressivo " & vbCrLf)
    '        Stb.Append("                and paRil.UniqueID = T.UniqueID " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            inner join ( " & vbCrLf)
    '        Stb.Append("                select FPA.Fr_Cod, fpa.Pa_Cod, F.Revocato, F.Data_Revo, FS.DataSospensioneDA, FS.DataSospensioneA, FVEG.Veg_Cod  " & vbCrLf)
    '        Stb.Append("                from formulatiXPrincipiAttivi FPA " & vbCrLf)
    '        Stb.Append("                    inner join formulati F  " & vbCrLf)
    '        Stb.Append("                        on FPA.fr_cod = F.fr_Cod " & vbCrLf)
    '        Stb.Append("                    left join FormulatiXPeriodoSospensione FS  " & vbCrLf)
    '        Stb.Append("                        on FS.fr_cod = F.fr_cod      " & vbCrLf)
    '        Stb.Append("                    inner join FormulatiXSpecieVegetali_GruppiEsplosiInSpecie FVEG               " & vbCrLf)
    '        Stb.Append("                        on F.Fr_Cod = FVEG.Fr_Cod  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            ) F on F.Pa_Cod = paRil.pa_cod       " & vbCrLf)
    '        Stb.Append("            and F.Veg_cod = T.Veg_cod " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            and (F.Revocato = 0 or " & vbCrLf)
    '        Stb.Append("                coalesce(F.Data_Revo, cast('01/01/2100' as datetime)) > T.Data_Analisi " & vbCrLf)
    '        Stb.Append("            ) " & vbCrLf)
    '        Stb.Append("            and ( " & vbCrLf)
    '        Stb.Append("                coalesce(F.DataSospensioneA, cast('01/01/1900' as datetime)) < AnAG.InizioAnnataAgraria  and " & vbCrLf)
    '        Stb.Append("                coalesce(F.DataSospensioneDA, cast('01/01/2100' as datetime)) > AnAG.FineAnnataAgraria " & vbCrLf)
    '        Stb.Append("                ) " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("            inner join FormulatiXSpecieVegetali_GruppiEsplosiInSpecie FVEG  " & vbCrLf)
    '        Stb.Append("            on F.fr_Cod = FVEG.fr_cod " & vbCrLf)
    '        Stb.Append("            and FVEG.Veg_cod = T.Veg_cod         " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append("    ) C " & vbCrLf)
    '        Stb.Append("    inner join PrincipiAttivi xDecodeFAm " & vbCrLf)
    '        Stb.Append("        on C.pa_cod = xDecodeFAm.pa_cod         " & vbCrLf)
    '        Stb.Append("    left join Analisi_TestataXFamigliePrincipiAttiviRilevati ATfP " & vbCrLf)
    '        Stb.Append("        on xDecodeFAm.Fam_COD = ATfP.FAM_COD  " & vbCrLf)
    '        Stb.Append("            and c.PivaSuperUser = ATfP.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("            and c.UniqueID = ATfP.UniqueID  " & vbCrLf)
    '        Stb.Append("            and c.Progressivo = ATFP.Progressivo  " & vbCrLf)
    '        Stb.Append("    left join Analisi_TestataXPrincipiAttiviRilevati ATP " & vbCrLf)
    '        Stb.Append("        on C.pa_cod = ATP.PA_COD  " & vbCrLf)
    '        Stb.Append("            and c.PivaSuperUser = ATfP.PivaSuperUser  " & vbCrLf)
    '        Stb.Append("            and c.UniqueID = ATfP.UniqueID  " & vbCrLf)
    '        Stb.Append("            and c.Progressivo = ATFP.Progressivo  " & vbCrLf)
    '        Stb.Append(" ) B  " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append(" on A.PivasuperUSer = B.PivasuperUser " & vbCrLf)
    '        Stb.Append(" and A.progressivo = B.Progressivo  " & vbCrLf)
    '        Stb.Append(" and A.UniqueID = B.UniqueID  " & vbCrLf)
    '        Stb.Append(" and A.pa_o_fam_COD = B.pa_o_fam_COD  " & vbCrLf)
    '        Stb.Append(" and A.isFamiglia = B.isFamiglia " & vbCrLf)
    '        Stb.Append(" and A.capitolato_cod = B.capitolato_cod " & vbCrLf)
    '        Stb.Append("  " & vbCrLf)
    '        Stb.Append(" where B.pa_o_fam_COD is null " & vbCrLf)





    '                            '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
    '                            '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function



    Public Function VerificaCapitolato_ElencoVarietaAmmesse(ByVal UniqueID As String, _
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXCapitolatoCliente_R.VerificaCapitolato_ElencoVarietaAmmesse()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim StrSql As String


        'query da: Gias_CapitolatoCliente.__analisi_verifica_Capitolato_VarietaAmmesse.sql

        Try

            Dim stb As New StringBuilder

            stb.Append(" -- " & NomeRoutine & vbCrLf)

            stb.Append(" --prima query: tutto è ammesso in quanto nessuna varietà in elenco (si sottintende CC.GestioneVarieta = 1 'elenco varietà ammesse' selezionato, unica opzione che ha senso) " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append(" '1' as Ammesso " & vbCrLf)
            stb.Append(" , t.PivaSuperUser " & vbCrLf)
            stb.Append(" , T.Progressivo  " & vbCrLf)
            stb.Append(" , CC.capitolato_cod      " & vbCrLf)
            stb.Append(" , TT.veg_cod " & vbCrLf)
            stb.Append(" , TT.cul_cod " & vbCrLf)
            stb.Append(" , T.UniqueID  " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("            inner join Analisi_TestataXLFSCultivarRilevate tt   " & vbCrLf)
            stb.Append("        on TT.uniqueID = t.uniqueID " & vbCrLf)
            stb.Append("        and TT.PivaSuperUser = t.PivaSuperUser  " & vbCrLf)
            stb.Append("        and TT.progressivo = t.Progressivo               " & vbCrLf)
            stb.Append(" inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.progressivo = ACC.Progressivo " & vbCrLf)
            stb.Append(" inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod " & vbCrLf)
            stb.Append(" where CC.Capitolato_cod not in ( " & vbCrLf)
            stb.Append("    select distinct Capitolato_Cod  " & vbCrLf)
            stb.Append("    from CapitolatoClienteXCultivar  " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" and T.uniqueID = " & UniqueID & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" Union " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append("--seconda query: varietà escluse quelle che vengono esplicitamente vietate (CC.GestioneVarieta = 0) 'elenco varietà Rifiutate' " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("                 '2' as Ammesso " & vbCrLf)
            stb.Append(" , t.PivaSuperUser " & vbCrLf)
            stb.Append(" , T.Progressivo  " & vbCrLf)
            stb.Append(" , CC.capitolato_cod      " & vbCrLf)
            stb.Append(" , TT.veg_cod " & vbCrLf)
            stb.Append(" , TT.cul_cod " & vbCrLf)
            stb.Append(" , T.UniqueID  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.progressivo = ACC.Progressivo      " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXLFSCultivarRilevate tt   " & vbCrLf)
            stb.Append("        on TT.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and TT.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and TT.progressivo = ACC.Progressivo                 " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where  " & vbCrLf)
            stb.Append("    T.UniqueID = " & UniqueID & vbCrLf)

            '  Vanni, 20/05/2013 13:16:13: TFS:317	problema su varietà ammesse in verifica capitolato
            '- nei piani di campionamento ad esempio delle albicocche o pesche, in blocca/sblocca, il programma mi dice che le varietà ad oggi oggetto di analisi non sono accettate dalla tesco, quando non è così! Le varietà analizzate fino ad oggi sono conformi, ho anche controllato il capitolato da me caricato e mi sembra sia caricato correttamente.
            stb.Append("    and CC.Capitolato_cod in ( " & vbCrLf)
            stb.Append("        select distinct c1.Capitolato_Cod  " & vbCrLf)
            stb.Append("        from CapitolatoClienteXCultivar c1 " & vbCrLf)
            stb.Append("            inner join CapitolatoCliente c " & vbCrLf)
            stb.Append("            on c1.Capitolato_COD = c.Capitolato_COD  " & vbCrLf)
            stb.Append("            and c.GestioneVarieta = 0  " & vbCrLf)
            stb.Append("    ) " & vbCrLf)


            stb.Append("  " & vbCrLf)
            stb.Append("    and not exists ( " & vbCrLf)
            stb.Append("        select ccc.Cul_COD, ccc.capitolato_Cod " & vbCrLf)
            stb.Append("        from CapitolatoClienteXCultivar CCC   " & vbCrLf)
            stb.Append("        where " & vbCrLf)
            stb.Append("                CC.GestioneVarieta = 0           " & vbCrLf)
            stb.Append("            and CCC.Capitolato_COD = CC.Capitolato_COD                                   " & vbCrLf)
            stb.Append("            and tt.cul_cod = ccc.cul_cod " & vbCrLf)
            stb.Append("    ) " & vbCrLf)
            '  Vanni, 20/05/2013 13:16:13: TFS:317	problema su varietà ammesse in verifica capitolato 
            'stb.Append("    and exists (  " & vbCrLf)
            'stb.Append("    select ccc.Cul_COD, ccc.capitolato_Cod " & vbCrLf)
            'stb.Append("        from CapitolatoClienteXCultivar CCC   " & vbCrLf)
            'stb.Append("        where " & vbCrLf)
            'stb.Append("                CC.GestioneVarieta <> 0 " & vbCrLf)
            'stb.Append("            and CCC.Capitolato_COD = CC.Capitolato_COD                                   " & vbCrLf)
            'stb.Append("            and tt.cul_cod = ccc.cul_cod " & vbCrLf)
            'stb.Append("    ) " & vbCrLf)
            stb.Append(" union " & vbCrLf)
            stb.Append("  " & vbCrLf)

            stb.Append(" --terza query: varietà esplicitamente consentite (CC.GestioneVarieta <>0) check 'elenco varietà ammesse' con lista popolata " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("    '3' as Ammesso " & vbCrLf)
            stb.Append("    , tT.PivaSuperUser " & vbCrLf)
            stb.Append("    , TT.Progressivo  " & vbCrLf)
            stb.Append("    , ACC.capitolato_cod         " & vbCrLf)
            stb.Append("    , Ammesse.veg_cod " & vbCrLf)
            stb.Append("    , Ammesse.cul_cod " & vbCrLf)
            stb.Append("    , TT.UniqueID  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from Analisi_Testata TT " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            stb.Append("        on TT.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and TT.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and TT.progressivo = ACC.Progressivo " & vbCrLf)
            stb.Append("    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
            stb.Append("        and CC.GestioneVarieta <>0 " & vbCrLf)
            stb.Append(" inner join " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append("    select    " & vbCrLf)
            stb.Append("    t.PivaSuperUser " & vbCrLf)
            stb.Append("    , T.Progressivo  " & vbCrLf)
            stb.Append("    , CC.capitolato_cod      " & vbCrLf)
            stb.Append("    , TT1.veg_cod " & vbCrLf)
            stb.Append("    , TT1.cul_cod " & vbCrLf)
            stb.Append("    , T.UniqueID  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXCapitolatoCliente ACC  " & vbCrLf)
            stb.Append("        on T.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and T.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.progressivo = ACC.Progressivo " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join CapitolatoCliente CC on CC.Capitolato_COD = Acc.Capitolato_Cod  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXLFSCultivarRilevate tt1   " & vbCrLf)
            stb.Append("        on TT1.uniqueID = acc.uniqueID " & vbCrLf)
            stb.Append("        and TT1.PivaSuperUser = ACC.PivaSuperUser  " & vbCrLf)
            stb.Append("        and TT1.progressivo = ACC.Progressivo        " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    inner join CapitolatoClienteXCultivar CCC on CC.Capitolato_COD = CCC.Capitolato_COD  " & vbCrLf)
            stb.Append("        and CC.GestioneVarieta <> 0 " & vbCrLf)
            stb.Append("            and CCC.Capitolato_COD = CC.Capitolato_COD       " & vbCrLf)
            stb.Append("            and ccc.cul_cod = tt1.cul_cod    " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where  " & vbCrLf)
            stb.Append("    T.UniqueID =  " & UniqueID & vbCrLf)
            stb.Append("    ) Ammesse " & vbCrLf)
            stb.Append(" on  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    Ammesse.Progressivo = TT.Progressivo  " & vbCrLf)
            stb.Append(" and Ammesse.PivaSuperUser = TT.PivaSuperUser  " & vbCrLf)
            stb.Append(" and Ammesse.UniqueID = TT.UniqueID " & vbCrLf)
            stb.Append(" and Ammesse.capitolato_cod = ACC.Capitolato_Cod " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" where TT.uniqueID =  " & UniqueID & vbCrLf)
            stb.Append(" --and Ammesse.Capitolato_COD is null " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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
