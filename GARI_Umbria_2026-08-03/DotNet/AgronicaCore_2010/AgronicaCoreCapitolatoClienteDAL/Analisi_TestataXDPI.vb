

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text

Public Class Analisi_TestataXDPI_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDPI( _
                            ByVal DPI_Flag_PrivatoPubblico As Integer, _
                            ByVal DPI_Cod_Regolamento As Integer, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.LeggiDPI()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder

        'da sql: gias_capitolatoCliente.__analisi_verifica_RMA_principi_attivi.sql

        Stb.Append("select * " & vbCrLf)
        Stb.Append(" from DPI_Regolamenti " & vbCrLf)
        Stb.Append(" where Flag_Privato_Pubblico = " & DPI_Flag_PrivatoPubblico & vbCrLf)
        Stb.Append(" and Cod_regolamento = " & DPI_Cod_Regolamento & vbCrLf)

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

    Public Function VerificaPA_AmmessoDedottoDA_RMA( _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaPA_AmmessoDedottoDA_RMA()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim Stb As New StringBuilder

        'da sql: gias_capitolatoCliente.__analisi_verifica_RMA_principi_attivi.sql

        Stb.Append(" -- " & NomeRoutine & vbCrLf)
        Stb.Append("select distinct T.veg_cod, T.pivaSuperUser, T.progressivo, T.pa_cod, T.isFamiglia, T.qtaRilevata, case when T.pa_o_Fam_cod is null then -1 else cast(T.RMA as real) end as RMA " & vbCrLf)
        Stb.Append(" from Analisi_Testata TES  " & vbCrLf)
        Stb.Append("    inner join Analisi_TestataXDPI ATCC " & vbCrLf)
        Stb.Append("        on TES.UniqueID = ATCC.UniqueID " & vbCrLf)
        Stb.Append("        and TES.PivaSuperUser = ATCC.PivaSuperUser " & vbCrLf)
        Stb.Append("        and TES.Progressivo = ATCC.Progressivo " & vbCrLf)
        Stb.Append("    inner join DPI_Regolamenti CC " & vbCrLf)
        Stb.Append("        on ATCC.Cod_Regolamento = CC.Cod_Regolamento " & vbCrLf)
        Stb.Append("        and ATCC.Flag_Privato_Pubblico = CC.Flag_Privato_Pubblico " & vbCrLf)
        Stb.Append("    inner join RMAXAnalisi T " & vbCrLf)
        Stb.Append("        on TES.UniqueID = T.UniqueID  " & vbCrLf)
        Stb.Append("        and T.PivaSuperUser = TES.PivaSuperUser  " & vbCrLf)
        Stb.Append("        and T.Progressivo = TES.Progressivo          " & vbCrLf)
        Stb.Append("        and T.validita_inizio < TES.Data_Analisi  " & vbCrLf)
        Stb.Append("    inner join ( " & vbCrLf)
        Stb.Append("        select veg_cod, PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod, max(Validita_inizio) as validita_inizio " & vbCrLf)
        Stb.Append("        from RMAXAnalisi " & vbCrLf)
        Stb.Append("        where uniqueID = " & UniqueID & " " & vbCrLf)
        Stb.Append("        group by veg_cod, PivaSuperUser, Progressivo, PA_COD, TabellaRMA_cod " & vbCrLf)
        Stb.Append("    ) t1 " & vbCrLf)
        Stb.Append(" on t1.veg_cod = t.veg_cod " & vbCrLf)
        Stb.Append(" and t1.pivaSuperUser = t.pivaSuperUser " & vbCrLf)
        Stb.Append(" and t1.Progressivo = t.Progressivo  " & vbCrLf)
        Stb.Append(" and t1.PA_Cod = t.PA_Cod " & vbCrLf)
        Stb.Append(" and t1.TabellaRMA_COD = t.TabellaRMA_COD  " & vbCrLf)
        Stb.Append(" and t1.validita_inizio = T.validita_inizio  " & vbCrLf)
        Stb.Append(" and t1.TabellaRMA_COD = CC.TabellaRMA_COD  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" where TES.uniqueID =" & UniqueID & " " & vbCrLf)
        Stb.Append("and ( ")
        Stb.Append("		T.RMA is null or ")
        Stb.Append("		cast(T.QtaRilevata as real )> cast(T.RMA as real)")
        Stb.Append(") ")
        Stb.Append("  " & vbCrLf)
        'Stb.Append(" union " & vbCrLf)
        'Stb.Append("  " & vbCrLf)
        'Stb.Append(" select veg_cod, pivaSuperUser, progressivo, pa_cod, qtaRilevata, rma " & vbCrLf)
        'Stb.Append(" from RMAXAnalisi " & vbCrLf)
        'Stb.Append(" where validita_inizio is null " & vbCrLf)
        'Stb.Append(" and UniqueID = " & UniqueID)

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

    Public Function VerificaPA_DPI( _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaPA_DPI()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim StrSql As String

        Try

            'da sql: __analisi_verifica_DP_principi_attivi.sql

            Dim stb As New StringBuilder

            'stb.Append("exec sp_executesql N'" & vbCrLf)
            stb.Append(" --  " & NomeRoutine & vbCrLf)

            ''  Vanni, 21/08/2015 16:18:40: 'gestione PA Ammessi Bio: i PA Presenti in tabella PrincipiAttivi_BIO sono ammessi da tutti i disciplinari, quindi vanno esclusi dal risultato della query..
            ''  VAnni: 22/01/2021: Gestione dell'esclusione di contesti di principi attivi dalla verifica dei DPI
            stb.Append("select * " & vbCrLf)
            stb.Append("from ( " & vbCrLf)
            '' fine  Vanni, 21/08/2015 16:18:40:
            '' fine  VAnni: 22/01/2021: 

            stb.Append(" select  " & vbCrLf)
            stb.Append("      T.UniqueID   " & vbCrLf)
            stb.Append("    , T.PivaSuperUser " & vbCrLf)
            stb.Append("    , T.Progressivo " & vbCrLf)
            stb.Append("    , ATdpi.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("    , ATdpi.cod_regolamento " & vbCrLf)
            stb.Append("    , ATPa.PA_Cod as PrincipioAttivoNonDPI   " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXPrincipiAttiviRilevati ATPa " & vbCrLf)
            stb.Append("        on T.PivaSuperUser = ATPa.PivaSuperUser " & vbCrLf)
            stb.Append("        and T.Progressivo = ATPa.Progressivo " & vbCrLf)
            stb.Append("        and T.UniqueID = ATPa.UniqueID " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXDPI ATdpi " & vbCrLf)
            stb.Append("        on T.PivaSuperUser =  ATdpi.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.Progressivo = ATdpi.Progressivo " & vbCrLf)
            stb.Append("        and T.UniqueID = ATdpi.UniqueID " & vbCrLf)
            stb.Append(" where  " & vbCrLf)
            stb.Append("    T.UniqueID = " & UniqueID & " " & vbCrLf)
            stb.Append("    and ATdpi.Flag_Privato_Pubblico = 1 " & vbCrLf)
            stb.Append("    and not exists ( " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("        T.PivaSuperUser " & vbCrLf)
            stb.Append("        , T.Progressivo " & vbCrLf)
            stb.Append("        , T.UniqueID " & vbCrLf)
            stb.Append("        , veg.veg_cod " & vbCrLf)
            stb.Append("        , PAA.PA_COD " & vbCrLf)
            stb.Append("    from Analisi_Testata T " & vbCrLf)
            stb.Append("        inner join Analisi_TestataXDPI ATDPI  " & vbCrLf)
            stb.Append("            on T.PivaSuperUser = ATDPI.PivaSuperUser " & vbCrLf)
            stb.Append("            and T.Progressivo = ATDPI.Progressivo " & vbCrLf)
            stb.Append("            and T.UniqueID = ATDPI.UniqueID " & vbCrLf)
            stb.Append("        inner join DPI_RaggruppamentiColturaliDPIXRegolamenti RCDPI " & vbCrLf)
            stb.Append("            on RCDPI.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and RCDPI.Cod_Regolamento = ATDPI.Cod_Regolamento " & vbCrLf)
            stb.Append("        inner join DPI_RaggruppamentiDPIXSpecieVegetali veg " & vbCrLf)
            stb.Append("            on veg.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and veg.ID_RCDPI = RCDPI.ID_RCDPI " & vbCrLf)
            stb.Append("            and veg.veg_cod = T.veg_cod      " & vbCrLf)
            stb.Append("        inner join DPI_RegolamentiXDifesaTestata DPI_RDT  " & vbCrLf)
            stb.Append("            on DPI_RDT.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and DPI_RDT.Cod_Regolamento = ATDPI.Cod_Regolamento " & vbCrLf)
            stb.Append("        inner join DPI_DifesaTestata DFT " & vbCrLf)
            stb.Append("            on DFT.DFT_COD = DPI_RDT.DFT_COD " & vbCrLf)
            stb.Append("            and DFT.ID_RCDPI = veg.ID_RCDPI " & vbCrLf)
            stb.Append("            and DFT.Flag_Privato_Pubblico = veg.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("        inner join DPI_DifesaRighe DFR " & vbCrLf)
            stb.Append("            on DFR.DFT_COD = DFT.DFT_COD " & vbCrLf)
            stb.Append("            and DFR.Flag_Privato_Pubblico = DFT.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("        inner join DPI_PrincipiAttivi_Gruppi PAA " & vbCrLf)
            stb.Append("            on DFR.DFR_COD = PAA.DFR_COD " & vbCrLf)
            stb.Append("            and DFR.Flag_Privato_Pubblico = PAA.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("            and PAA.PA_COD is not null           " & vbCrLf)
            stb.Append("    WHERE PAA.PA_COD =  ATPa.PA_COD      " & vbCrLf)
            stb.Append("    AND T.UniqueID = " & UniqueID & " " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" union " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      T.UniqueID   " & vbCrLf)
            stb.Append("    , T.PivaSuperUser " & vbCrLf)
            stb.Append("    , T.Progressivo " & vbCrLf)
            stb.Append("    , ATdpi.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("    , ATdpi.cod_regolamento " & vbCrLf)
            stb.Append("    , ATPa.PA_Cod as PrincipioAttivoNonDPI   " & vbCrLf)
            stb.Append(" from Analisi_Testata T " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXPrincipiAttiviRilevati ATPa " & vbCrLf)
            stb.Append("        on T.PivaSuperUser = ATPa.PivaSuperUser " & vbCrLf)
            stb.Append("        and T.Progressivo = ATPa.Progressivo " & vbCrLf)
            stb.Append("        and T.UniqueID = ATPa.UniqueID " & vbCrLf)
            stb.Append("    inner join Analisi_TestataXDPI ATdpi " & vbCrLf)
            stb.Append("        on T.PivaSuperUser =  ATdpi.PivaSuperUser  " & vbCrLf)
            stb.Append("        and T.Progressivo = ATdpi.Progressivo " & vbCrLf)
            stb.Append("        and T.UniqueID = ATdpi.UniqueID " & vbCrLf)
            stb.Append(" where  " & vbCrLf)
            stb.Append("    T.UniqueID = " & UniqueID & " " & vbCrLf)
            stb.Append(" and ATdpi.Flag_Privato_Pubblico = 2 " & vbCrLf)
            stb.Append(" and not exists ( " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("        T.PivaSuperUser " & vbCrLf)
            stb.Append("        , T.Progressivo " & vbCrLf)
            stb.Append("        , T.UniqueID " & vbCrLf)
            stb.Append("        , veg.veg_cod " & vbCrLf)
            stb.Append("        , PAA.PA_COD " & vbCrLf)
            stb.Append("    from Analisi_Testata T " & vbCrLf)
            stb.Append("        inner join Analisi_TestataXDPI ATDPI  " & vbCrLf)
            stb.Append("            on T.PivaSuperUser = ATDPI.PivaSuperUser " & vbCrLf)
            stb.Append("            and T.Progressivo = ATDPI.Progressivo " & vbCrLf)
            stb.Append("            and T.UniqueID = ATDPI.UniqueID " & vbCrLf)
            stb.Append("        inner join DPI_RaggruppamentiColturaliDPIXRegolamenti RCDPI " & vbCrLf)
            stb.Append("            on RCDPI.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and RCDPI.Cod_Regolamento = ATDPI.Cod_Regolamento " & vbCrLf)
            stb.Append("        inner join DPI_RaggruppamentiDPIXSpecieVegetali veg " & vbCrLf)
            stb.Append("            on veg.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and veg.ID_RCDPI = RCDPI.ID_RCDPI " & vbCrLf)
            stb.Append("            and veg.veg_cod = T.veg_cod      " & vbCrLf)
            stb.Append("        inner join DPI_RegolamentiXDifesaTestata DPI_RDT  " & vbCrLf)
            stb.Append("            on DPI_RDT.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico " & vbCrLf)
            stb.Append("            and DPI_RDT.Cod_Regolamento = ATDPI.Cod_Regolamento " & vbCrLf)
            stb.Append("        inner join DPI_DifesaTestata DFT " & vbCrLf)
            stb.Append("            on DFT.DFT_COD = DPI_RDT.DFT_COD " & vbCrLf)
            stb.Append("            and DFT.ID_RCDPI = veg.ID_RCDPI " & vbCrLf)
            stb.Append("            and DFT.Flag_Privato_Pubblico = veg.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("        inner join DPI_DifesaRighe DFR " & vbCrLf)
            stb.Append("            on DFR.DFT_COD = DFT.DFT_COD " & vbCrLf)
            stb.Append("            and DFR.Flag_Privato_Pubblico = DFT.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("        inner join P_DPI_PrincipiAttivi_Gruppi PAA " & vbCrLf)
            stb.Append("            on DFR.DFR_COD = PAA.DFR_COD " & vbCrLf)
            stb.Append("            and DFR.Flag_Privato_Pubblico = PAA.Flag_Privato_Pubblico  " & vbCrLf)
            stb.Append("            and PAA.PA_COD is not null           " & vbCrLf)
            stb.Append("    WHERE PAA.PA_COD =  ATPa.PA_COD      " & vbCrLf)
            stb.Append("    AND T.UniqueID = " & UniqueID & " " & vbCrLf)
            stb.Append(" ) " & vbCrLf)

            ''  Vanni, 21/08/2015 16:18:40: PA BIO
            ''  VAnni: 22/1/2021: Gestione dell'esclusione di contesti di principi attivi dalla verifica dei DPI
            stb.Append(" " & vbCrLf)
            stb.Append("              ) a " & vbCrLf)

            stb.AppendLine(" where Not exists( ")
            stb.AppendLine("     Select 1  ")
            stb.AppendLine("     from( ")
            stb.AppendLine("         select pa_cod   ")
            stb.AppendLine("         From PrincipiAttivi_BIO pabio ")
            stb.AppendLine("         union ")
            stb.AppendLine("         Select pa_Cod ")
            stb.AppendLine("         From PrincipiAttiviXPrincipiAttivi_Contesto papac ")
            stb.AppendLine("         Where PrincipiAttivi_Contesto_COD <> 1 ")
            stb.AppendLine("     ) esc         ")
            stb.AppendLine("     where esc.PA_cod = a.PrincipioAttivoNonDPI ")
            stb.AppendLine(" )")

            ''FINE  Vanni, 21/08/2015 16:18:40: PA BIO
            ''FINE  VAnni: 22/1/2021: Gestione dell'esclusione di contesti di principi attivi dalla verifica dei DPI

            'stb.Append("', N'@uniqueID int', @UniqueID = " & UniqueID & " ") 

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


    Public Function VerificaFamigliePA_DPI( _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_R.VerificaFamigliePA_DPI()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable = Nothing
        Dim stb As New StringBuilder

        Try

            'da sql: GIAS_CapitolatoCliente.__analisi_verifica_DP_Famiglie_principi_attivi.sql

            'stb.Append("exec sp_executesql N'" & vbCrLf)

            stb.Append(" -- " & NomeRoutine & vbCrLf)

            ''  Vanni, 21/08/2015 16:18:40: 'gestione PA Ammessi Bio: i PA Presenti in tabella PrincipiAttivi_BIO sono ammessi da tutti i disciplinari, quindi vanno esclusi dal risultato della query..
            'stb.Append("select * " & vbCrLf)
            'stb.Append("from ( " & vbCrLf)
            '' fine  Vanni, 21/08/2015 16:18:40:

            Famiglia_GetQuery(stb, UniqueID, 1)

            stb.Append("union " & vbCrLf)

            Famiglia_GetQuery(stb, UniqueID, 2)

            ''  Vanni, 21/08/2015 16:18:40: PA BIO
            'stb.Append(" " & vbCrLf  ) 
            'stb.Append("              ) a " & vbCrLf)
            'stb.Append("  " & vbCrLf)
            'stb.Append("  where not exists ( " & vbCrLf)
            'stb.Append("  Select 1 " & vbCrLf)
            'stb.Append("    from principiattivi  " & vbCrLf)
            'stb.Append("    where fam_cod = A.FamigliaPrincipioAttivoNonDPI  " & vbCrLf)
            'stb.Append("    and pa_cod in (select pa_cod  from PrincipiAttivi_BIO) " & vbCrLf)
            'stb.Append(" ) " & vbCrLf)
            ''FINE  Vanni, 21/08/2015 16:18:40: PA BIO


                'stb.Append("', N'@uniqueID int', @UniqueID = " & UniqueID & " " & vbCrLf)


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


    Private Sub Famiglia_GetQuery(ByRef stb As StringBuilder, ByVal UniqueID As Integer, ByVal Privato_1_Pubblico_2 As Integer)


        stb.Append(" Select " & vbCrLf)
        stb.Append("      T.UniqueID    " & vbCrLf)
        stb.Append("     , T.PivaSuperUser  " & vbCrLf)
        stb.Append("     , T.Progressivo  " & vbCrLf)
        stb.Append("     , ATdpi.Flag_Privato_Pubblico  " & vbCrLf)
        stb.Append("     , ATdpi.cod_regolamento  " & vbCrLf)
        stb.Append("     , ATPa.fam_Cod as FamigliaPrincipioAttivoNonDPI       " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  from Analisi_Testata T  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     inner join Analisi_TestataXFamigliePrincipiAttiviRilevati ATPa  " & vbCrLf)
        stb.Append("         on T.PivaSuperUser = ATPa.PivaSuperUser  " & vbCrLf)
        stb.Append("         and T.Progressivo = ATPa.Progressivo  " & vbCrLf)
        stb.Append("         and T.UniqueID = ATPa.UniqueID  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     inner join (  " & vbCrLf)
        stb.Append("         select pa_Cod, fam_cod   " & vbCrLf)
        stb.Append("         from PrincipiAttiviXFamigliePrincipiAttivi   " & vbCrLf)
        stb.Append("         where FamigliePrincipiAttivi_contesto_cod = 2   " & vbCrLf)
        stb.Append("     ) PAFAM  " & vbCrLf)
        stb.Append("         on PAFAM.Fam_cod = ATPa.Fam_cod  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     inner join PrincipiAttivi PA  " & vbCrLf)
        stb.Append("         on PAFAM.PA_Cod = Pa.Pa_cod --collate Latin1_General_CI_AS  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     inner join Analisi_TestataXDPI ATdpi  " & vbCrLf)
        stb.Append("         on T.PivaSuperUser =  ATdpi.PivaSuperUser   " & vbCrLf)
        stb.Append("         and T.Progressivo = ATdpi.Progressivo  " & vbCrLf)
        stb.Append("         and T.UniqueID = ATdpi.UniqueID  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  where T.UniqueID = " & UniqueID & vbCrLf)
        stb.Append("  and ATdpi.Flag_Privato_Pubblico =  " & Privato_1_Pubblico_2 & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  and not exists (  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     select  1 " & vbCrLf)
        stb.Append("     from Analisi_Testata T1  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join Analisi_TestataXDPI ATDPI1   " & vbCrLf)
        stb.Append("             on T1.PivaSuperUser = ATDPI1.PivaSuperUser  " & vbCrLf)
        stb.Append("             and T1.Progressivo = ATDPI1.Progressivo  " & vbCrLf)
        stb.Append("             and T1.UniqueID = ATDPI1.UniqueID  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_RaggruppamentiColturaliDPIXRegolamenti RCDPI  " & vbCrLf)
        stb.Append("             on RCDPI.Flag_Privato_Pubblico = ATDPI1.Flag_Privato_Pubblico  " & vbCrLf)
        stb.Append("             and RCDPI.Cod_Regolamento = ATDPI1.Cod_Regolamento  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_RaggruppamentiDPIXSpecieVegetali veg  " & vbCrLf)
        stb.Append("             on veg.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico  " & vbCrLf)
        stb.Append("             and veg.ID_RCDPI = RCDPI.ID_RCDPI  " & vbCrLf)
        stb.Append("             and veg.veg_cod = T1.veg_cod       " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_RegolamentiXDifesaTestata DPI_RDT   " & vbCrLf)
        stb.Append("             on DPI_RDT.Flag_Privato_Pubblico = ATDPI.Flag_Privato_Pubblico  " & vbCrLf)
        stb.Append("             and DPI_RDT.Cod_Regolamento = ATDPI.Cod_Regolamento  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_DifesaTestata DFT  " & vbCrLf)
        stb.Append("             on DFT.DFT_COD = DPI_RDT.DFT_COD  " & vbCrLf)
        stb.Append("             and DFT.ID_RCDPI = veg.ID_RCDPI  " & vbCrLf)
        stb.Append("             and DFT.Flag_Privato_Pubblico = veg.Flag_Privato_Pubblico  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_DifesaRighe DFR  " & vbCrLf)
        stb.Append("             on DFR.DFT_COD = DFT.DFT_COD  " & vbCrLf)
        stb.Append("             and DFR.Flag_Privato_Pubblico = DFT.Flag_Privato_Pubblico   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join DPI_PrincipiAttivi_Gruppi PAA  " & vbCrLf)
        stb.Append("             on DFR.DFR_COD = PAA.DFR_COD  " & vbCrLf)
        stb.Append("             and DFR.Flag_Privato_Pubblico = PAA.Flag_Privato_Pubblico                     " & vbCrLf)
        stb.Append("             and PAA.PA_COD is not null        " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("         inner join (  " & vbCrLf)
        stb.Append("                 select pa_Cod, fam_cod   " & vbCrLf)
        stb.Append("                 from PrincipiAttiviXFamigliePrincipiAttivi   " & vbCrLf)
        stb.Append("                 where FamigliePrincipiAttivi_contesto_cod = 2   " & vbCrLf)
        stb.Append("             ) PA_in  " & vbCrLf)
        stb.Append("             on  PAA.PA_COD = PA_in.Pa_Cod   " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("      where  " & vbCrLf)
        stb.Append("         PA_in.Fam_cod = PAFAM.Fam_cod                             " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     and t.pivasuperuser = t1.pivaSuperUser " & vbCrLf)
        stb.Append("     and t.UniqueID = t1.UniqueID  " & vbCrLf)
        stb.Append("     and t.Progressivo = t1.Progressivo " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("     and ATDPI.PivaSuperUser = ATDPI1.pivasuperuser " & vbCrLf)
        stb.Append("     and ATDPI.UniqueID = ATDPI1.UniqueId " & vbCrLf)
        stb.Append("     and ATDPI.Progressivo = ATDPI1.Progressivo " & vbCrLf)
        stb.Append("     and ATDPI.Cod_Regolamento = ATDPI1.Cod_Regolamento " & vbCrLf)
        stb.Append("     and ATDPI.Flag_Privato_Pubblico = ATDPI1.Flag_Privato_Pubblico " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  ) " & vbCrLf)
        stb.Append(" ")



    End Sub

End Class

Public Class Analisi_TestataXDPI_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub Cancella(ByVal PivaSuperUser As String, _
                      ByVal Progressivo As Integer, _
                      ByVal Flag_Privato_Pubblico As Integer, _
                      ByVal Regolamento_cod As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try

            StrSql = _
               "     DELETE FROM Analisi_TestataXCapitolatoCliente " & _
               "    WHERE PivaSuperUser = '" & PivaSuperUser & "'" & _
               "    AND Progressivo = " & Progressivo & _
               "    AND Flag_Privato_Pubblico = " & Flag_Privato_Pubblico & _
               "    AND Regolamento_cod = " & Regolamento_cod & _
               "    AND UniqueID = '" & UniqueID & "'"



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
                      ByVal Flag_Privato_Pubblico As Integer, _
                      ByVal Regolamento_cod As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
           "     INSERT INTO Analisi_TestataXDPI " & _
 "           ([PivaSuperUser] " & _
 "           ,[Progressivo] " & _
 "           ,[Flag_Privato_Pubblico] " & _
 "           ,[Cod_Regolamento] " & _
 "           ,[UniqueID]) " & _
 "            VALUES " & _
 "           ('" & PivaSuperUser & "'" & _
 "           ," & Progressivo & "" & _
 "           ," & Flag_Privato_Pubblico & "" & _
 "           ," & Regolamento_cod & "" & _
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


    Public Sub CopiaDPI_IndicatiSuCapitolatoSuIstanzaTemporanea(ByVal PivaSuperUser As String, _
                      ByVal Progressivo As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXDPI_W.CopiaDPI_IndicatiSuCapitolatoSuIstanzaTemporanea"

        'query da: __analisi_salva_IstanzaAnalisi_CopiaDP.sql

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim Stb As New StringBuilder

        Try

            Stb.Append(" insert Analisi_TestataXDPI " & vbCrLf)
            Stb.Append("select distinct " & vbCrLf)
            Stb.Append("      AtXCC.UniqueID " & vbCrLf)
            Stb.Append("    , ATXCC.PivaSuperUSER " & vbCrLf)
            Stb.Append("    , ATXCC.Progressivo " & vbCrLf)
            Stb.Append("    , CC.Flag_Privato_Pubblico_DPI " & vbCrLf)
            Stb.Append("    , CC.DPI_COD_REGOLAMENTO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    FROM Analisi_Testata T " & vbCrLf)
            Stb.Append("    inner join Analisi_TestataXCapitolatoCliente ATXCC  " & vbCrLf)
            Stb.Append("        on ATXCC.UniqueID = T.UniqueID   " & vbCrLf)
            Stb.Append("         and ATXCC.PivaSuperUser = T.PivaSuperUser  " & vbCrLf)
            Stb.Append("         and ATXCC.Progressivo = T.Progressivo                 " & vbCrLf)

            Stb.Append("    inner join capitolatoCliente CC " & vbCrLf)
            Stb.Append("        on ATXCC.Capitolato_Cod = CC.Capitolato_COD  " & vbCrLf)
            Stb.Append("        and ATXCC.UniqueID = " & UniqueID & vbCrLf)
            Stb.Append("        and CC.DPI_Impianto = 0 " & vbCrLf)
            Stb.Append("    left join            " & vbCrLf)
            Stb.Append("        Analisi_TestataXDPI A  " & vbCrLf)
            Stb.Append("            on A.UniqueID = ATXCC.UniqueID  " & vbCrLf)
            Stb.Append("            and A.PivaSuperUser = ATXCC.PivaSuperUser " & vbCrLf)
            Stb.Append("            and A.Progressivo = ATXCC.Progressivo                " & vbCrLf)
            Stb.Append("            and A.Flag_Privato_Pubblico = CC.Flag_Privato_Pubblico_DPI  " & vbCrLf)
            Stb.Append("            and A.Cod_Regolamento  = CC.DPI_COD_REGOLAMENTO  " & vbCrLf)
            Stb.Append(" where  ATXCC.UniqueID = " & UniqueID & vbCrLf)
            Stb.Append("      and A.UniqueId is null " & vbCrLf)
            Stb.Append("      and CC.dpi_cod_Regolamento is not null " & vbCrLf)

                '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub



End Class
