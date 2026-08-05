Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreUtility



Public Class Dpi_Abaco_R

    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggixAbaco_Disciplinari(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixAbaco_Disciplinari()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" ( Select Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaRighe.DFT_COD, DifesaRighe.DFR_COD, DifesaRighe.DescrizioneInfestanti, Infestanti.Inf_Cod, PA_Ausiliari.ID_PAA, PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, isNull(PA_Ausiliari.ID_PAA_MISC, 0) as ID_PAA_MISC,  ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" Isnull(GruppoFinalita.GRFI_Cod, '') as Grfi_Cod,  Isnull(GruppoFinalita.GRFI_DES, '') as Grfi_Des, ")
            StrSQL.Append(" Isnull(Avversita.AV_COD, 0) As Avversita_Cod, Isnull(Avversita.AV_DES_VOL, '') as Avversita_Des, Isnull(Avversita.AV_DES_LAT, '') as Avversita_Des_Lat, ")
            StrSQL.Append(" Isnull(GruppoAvversita.AV_GRU, 0) As Gruppo_Avversita_Cod, Isnull(GruppoAvversita.AV_GRU_DES, '') as Gruppo_Avversita_Des, Isnull(GruppoAvversita.AV_GRU_DES_LAT, '') as Gruppo_Avversita_Des_Lat ")

            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti ")
            StrSQL.Append(" Left Outer Join Avversita On (Infestanti.Av_Cod = Avversita.Av_Cod) ")
            StrSQL.Append(" Left Outer Join GruppoAvversita On (Infestanti.AV_GRU = GruppoAvversita.AV_GRU), ")

            StrSQL.Append(" RaggruppamentiDPIXSpecieVegetali ")
            StrSQL.Append(" Left outer join GruppoFinalita On (Isnull(RaggruppamentiDPIXSpecieVegetali.Grfi_COD, 0) = Isnull(GruppoFinalita.GRFI_COD,0)) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            'Filtro Difesa e Diserbo
            StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")

            'StrSQL.Append(" And specievegetali.veg_cod = 1 ")


            StrSQL.Append(" ) Union ")


            StrSQL.Append(" ( Select Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaRighe.DFT_COD, DifesaRighe.DFR_COD, DifesaRighe.DescrizioneInfestanti, 0 as Inf_Cod, PA_Ausiliari.ID_PAA, PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, isNull(PA_Ausiliari.ID_PAA_MISC, 0) as ID_PAA_MISC,  ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" Isnull(GruppoFinalita.GRFI_Cod, '') as Grfi_Cod,  Isnull(GruppoFinalita.GRFI_DES, '') as Grfi_Des, ")
            StrSQL.Append(" 0 as Avversita_Cod, '' as Avversita_Des, '' as Avversita_Des_Lat, ")
            StrSQL.Append(" 0 As Gruppo_Avversita_Cod, '' as Gruppo_Avversita_Des, '' as Gruppo_Avversita_Des_Lat  ")

            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, RaggruppamentiDPIXSpecieVegetali ")
            StrSQL.Append(" Left outer join GruppoFinalita On (Isnull(RaggruppamentiDPIXSpecieVegetali.Grfi_COD, 0) = Isnull(GruppoFinalita.GRFI_COD,0)) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And PA_Ausiliari.DFT_COD = DifesaRighe.DFT_COD ")
            StrSQL.Append(" And PA_Ausiliari.DFR_COD = DifesaRighe.DFR_COD   ")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            'Filtro Fitoregolatori
            StrSQL.Append(" And TipoTestata = 2  ")


            'StrSQL.Append(" And specievegetali.veg_cod = 1 ")

            StrSQL.Append(" )")


            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, Avversita_Cod, Gruppo_Avversita_Cod, DFT_COD, DFR_COD, ID_PAA_MISC ")

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


    Public Function LeggixVincoli_Disciplinari_DISC_DOSE_MAX(
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_DOSE_MAX()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaRighe.DFT_COD, DifesaRighe.DFR_COD, DifesaRighe.DescrizioneInfestanti, PA_Ausiliari.ID_PAA, PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" IsNull(PA_Ausiliari.Dose_UDM, 0) as Dose_UDM, Isnull(PA_Ausiliari.Dose_Max_Anno, 0) as Dose_Max_Anno, isnull(PA_Ausiliari.PercPA, 0) as PercPA,  isnull(PA_Ausiliari.PesoPA, 0) as PesoPA, isNull(PA_Ausiliari.ID_PAA_MISC, 0) as ID_PAA_MISC  ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            'Filtro i dosaggi valorizzati e le miscele
            StrSQL.Append(" And (Isnull(PA_Ausiliari.Dose_Max_Anno, 0) <> 0 OR isNull(PA_Ausiliari.ID_PAA_MISC, 0) <> 0 ) ")

            'Filtro Difesa e Diserbo
            StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")


            StrSQL.Append(" ) Union ")



            StrSQL.Append(" (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaRighe.DFT_COD, DifesaRighe.DFR_COD, DifesaRighe.DescrizioneInfestanti, PA_Ausiliari.ID_PAA, PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" IsNull(PA_Ausiliari.Dose_UDM, 0) as Dose_UDM, Isnull(PA_Ausiliari.Dose_Max_Anno, 0) as Dose_Max_Anno, isnull(PA_Ausiliari.PercPA, 0) as PercPA,  isnull(PA_Ausiliari.PesoPA, 0) as PesoPA, isNull(PA_Ausiliari.ID_PAA_MISC, 0) as ID_PAA_MISC  ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, RaggruppamentiDPIXSpecieVegetali ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And PA_Ausiliari.DFT_COD = DifesaRighe.DFT_COD ")
            StrSQL.Append(" And PA_Ausiliari.DFR_COD = DifesaRighe.DFR_COD   ")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            'Filtro i dosaggi valorizzati e le miscele
            StrSQL.Append(" And (Isnull(PA_Ausiliari.Dose_Max_Anno, 0) <> 0 OR isNull(PA_Ausiliari.ID_PAA_MISC, 0) <> 0 ) ")

            'Filtro Fitoregolatori
            StrSQL.Append(" And TipoTestata = 2  ")

            StrSQL.Append(" )")


            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, DFT_COD, DFR_COD, ID_PAA_MISC ")





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





    Public Function LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV(ByVal Tipo As String,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                      Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
            StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append("  Isnull(Avversita.AV_COD, 0) As Avversita_Cod, Isnull(Avversita.AV_DES_VOL, '') as Avversita_Des,  ")
            StrSQL.Append("  Isnull(Avversita.AV_DES_LAT, '') as Avversita_Des_Lat,  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) as NmaxTratt ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) as NmaxTratt ")
            End Select


            StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
            StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
            StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali, Infestanti  Inner Join Avversita On (Infestanti.Av_Cod = Avversita.Av_Cod) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And LimitazioniUso.Id_PAA = PA_Ausiliari.Id_Paa ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0  ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select


            'Filtro Difesa e Diserbo
            StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")

            StrSQL.Append(" Group By ")
            StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" Avversita.AV_COD, Avversita.AV_DES_VOL, ")
            StrSQL.Append(" Avversita.AV_DES_LAT ")

            StrSQL.Append(" ) ")


            If Tipo = "A" Then

                StrSQL.Append(" Union ")

                StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
                StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
                StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
                StrSQL.Append("  Isnull(Avversita.AV_COD, 0) As Avversita_Cod, Isnull(Avversita.AV_DES_VOL, '') as Avversita_Des,  ")
                StrSQL.Append("  Isnull(Avversita.AV_DES_LAT, '') as Avversita_Des_Lat,  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

                StrSQL.Append("  Max(LimitazioniUso.NmaxTratt) as NmaxTratt ")

                StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
                StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
                StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali, Infestanti  Inner Join Avversita On (Infestanti.Av_Cod = Avversita.Av_Cod) ")

                StrSQL.Append(" Where 1 = 1 ")

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------

                StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
                StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
                StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
                StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
                StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And LimitazioniUso.Id_PAA = PA_Ausiliari.Id_Paa ")

                StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt, 0) > 0  ")

                'Filtro Difesa e Diserbo
                StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")

                StrSQL.Append(" Group By ")
                StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
                StrSQL.Append(" Avversita.AV_COD, Avversita.AV_DES_VOL, ")
                StrSQL.Append(" Avversita.AV_DES_LAT ")

                StrSQL.Append(" ) ")

            End If

            StrSQL.Append(" Union ")

            StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
            StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" 0 as Avversita_Cod, '' as Avversita_Des, '' as Avversita_Des_Lat,  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) As NmaxTratt ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) As NmaxTratt ")
            End Select


            StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
            StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
            StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And PA_Ausiliari.DFT_COD = DifesaRighe.DFT_COD ")
            StrSQL.Append(" And PA_Ausiliari.DFR_COD = DifesaRighe.DFR_COD   ")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0  ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select

            'Filtro Fitoregolatori
            StrSQL.Append(" And TipoTestata = 2  ")


            StrSQL.Append(" Group By ")
            StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome ")


            StrSQL.Append(" )")

            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, Avversita_Cod ")

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



    Public Function LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV(ByVal Tipo As String,
                                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                             Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try



            StrSQL.Length = 0

            StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
            StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append("  Isnull(GruppoAvversita.AV_GRU, 0) As Gruppo_Avversita_Cod, Isnull(GruppoAvversita.AV_GRU_DES, '') as Gruppo_Avversita_Des, Isnull(GruppoAvversita.AV_GRU_DES_LAT, '') as Gruppo_Avversita_Des_Lat,   ")
            StrSQL.Append("  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) as NmaxTratt ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) as NmaxTratt ")
            End Select


            StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
            StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
            StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali, Infestanti  Inner Join GruppoAvversita On (Infestanti.Av_Gru = GruppoAvversita.Av_Gru) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And LimitazioniUso.Id_PAA = PA_Ausiliari.Id_Paa ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0  ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select


            'Filtro Difesa e Diserbo
            StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")

            StrSQL.Append(" Group By ")
            StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" GruppoAvversita.AV_GRU, GruppoAvversita.AV_GRU_DES, ")
            StrSQL.Append(" GruppoAvversita.AV_GRU_DES_LAT ")

            StrSQL.Append(" ) ")


            StrSQL.Append(" Union ")


            If Tipo = "A" Then

                StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
                StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
                StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
                StrSQL.Append("  Isnull(GruppoAvversita.AV_GRU, 0) As Gruppo_Avversita_Cod, Isnull(GruppoAvversita.AV_GRU_DES, '') as Gruppo_Avversita_Des, Isnull(GruppoAvversita.AV_GRU_DES_LAT, '') as Gruppo_Avversita_Des_Lat,   ")
                StrSQL.Append("  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

                StrSQL.Append("  Max(LimitazioniUso.NmaxTratt) as NmaxTratt ")

                StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
                StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
                StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali, Infestanti  Inner Join GruppoAvversita On (Infestanti.Av_Gru = GruppoAvversita.Av_Gru) ")

                StrSQL.Append(" Where 1 = 1 ")

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------

                StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
                StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
                StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
                StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
                StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And LimitazioniUso.Id_PAA = PA_Ausiliari.Id_Paa ")
                StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt, 0) > 0   ")



                'Filtro Difesa e Diserbo
                StrSQL.Append(" And ((TipoTestata = 0 And (Infestanti.AV_GRU Is Not null Or Infestanti.AV_Cod Is Not null))  Or tipotestata = 1) ")

                StrSQL.Append(" Group By ")
                StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
                StrSQL.Append(" GruppoAvversita.AV_GRU, GruppoAvversita.AV_GRU_DES, ")
                StrSQL.Append(" GruppoAvversita.AV_GRU_DES_LAT ")

                StrSQL.Append(" ) ")

                StrSQL.Append(" Union ")

            End If


            StrSQL.Append("  (Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod,  ")
            StrSQL.Append("  SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append("  DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" 0 as Gruppo_Avversita_Cod, '' as Gruppo_Avversita_Des, '' as Gruppo_Avversita_Des_Lat,  Min(PA_Ausiliari.Id_PAA) as Chiave, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) As NmaxTratt ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) As NmaxTratt ")
            End Select


            StrSQL.Append("  From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, ")
            StrSQL.Append("  Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, LimitazioniUso, ")
            StrSQL.Append("  RaggruppamentiDPIXSpecieVegetali ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And PA_Ausiliari.DFT_COD = DifesaRighe.DFT_COD ")
            StrSQL.Append(" And PA_Ausiliari.DFR_COD = DifesaRighe.DFR_COD   ")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And ( Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0 or Isnull(LimitazioniUso.NmaxTratt, 0) > 0 )  ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select

            'Filtro Fitoregolatori
            StrSQL.Append(" And TipoTestata = 2  ")


            StrSQL.Append(" Group By ")
            StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod,   ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome ")


            StrSQL.Append(" )")

            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, Gruppo_Avversita_Cod ")

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




    Public Function LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA(ByVal Tipo As String,
                                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                     Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" ( Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) as NmaxTratt, ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) as NmaxTratt, ")
            End Select

            StrSQL.Append(" PrincipiAttivi.PA_COD as Codice, ")
            StrSQL.Append(" Min(LimitazioniUso.Id_Lu) as ID_DISC_N_MAX_TRATT_X_SA ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
            StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0   ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select

            StrSQL.Append(" Group by Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome,  ")
            StrSQL.Append(" PrincipiAttivi.PA_Cod ")
            StrSQL.Append(" ) ")

            If Tipo = "A" Then

                StrSQL.Append(" Union ")
                StrSQL.Append(" ( Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")

                StrSQL.Append("  Max(LimitazioniUso.NmaxTratt) as NmaxTratt, ")

                StrSQL.Append(" PrincipiAttivi.PA_COD as Codice, ")
                StrSQL.Append(" Min(LimitazioniUso.Id_Lu) as ID_DISC_N_MAX_TRATT_X_SA ")
                StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
                StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
                StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

                StrSQL.Append(" Where 1 = 1 ")

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------

                StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
                StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
                StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
                StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
                StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

                StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
                StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
                StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

                StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt, 0) > 0  ")


                StrSQL.Append(" Group by Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome,  ")
                StrSQL.Append(" PrincipiAttivi.PA_Cod ")
                StrSQL.Append(" ) ")

            End If




            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, Codice ")

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





    Public Function LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA(ByVal Tipo As String,
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                            Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" ( Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Anno) as NmaxTratt, ")
                Case "C"
                    StrSQL.Append("  Max(LimitazioniUso.NmaxTratt_Ciclo_colt) as NmaxTratt, ")
            End Select

            StrSQL.Append(" Min (PA_Ausiliari.Id_PAA) as ID_DISC_N_MAX_TRATT_X_GRUPPO_SA , ")
            StrSQL.Append(" GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_Des ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
            StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

            Select Case Tipo
                Case "A"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0   ")
                Case "C"
                    StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 ")
            End Select

            StrSQL.Append(" Group by Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
            StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
            StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
            StrSQL.Append(" GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GPAI_PA_Ausiliari_Des ")
            StrSQL.Append(" ) ")

            If Tipo = "A" Then

                StrSQL.Append(" Union ( ")

                StrSQL.Append(" Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")

                StrSQL.Append("  Max(LimitazioniUso.NmaxTratt) as NmaxTratt, ")

                StrSQL.Append(" Min (PA_Ausiliari.Id_PAA) as ID_DISC_N_MAX_TRATT_X_GRUPPO_SA , ")
                StrSQL.Append(" GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_Des ")
                StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
                StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
                StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

                StrSQL.Append(" Where 1 = 1 ")

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo <> "" Then
                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                '--------------------------------------------------------------------------

                StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
                StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
                StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
                StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
                StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
                StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
                StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
                StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
                StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

                StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
                StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
                StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

                StrSQL.Append("  And Isnull(LimitazioniUso.NmaxTratt, 0) > 0  ")


                StrSQL.Append(" Group by Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, ''), Lista_Regioni.REG, ")
                StrSQL.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, RaggruppamentiDPIXSpecieVegetali.Grfi_Cod, ")
                StrSQL.Append(" DifesaTestata.TipoTestata, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome, ")
                StrSQL.Append(" GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GPAI_PA_Ausiliari_Des ) ")



            End If




            StrSQL.Append(" Order by Anno, Regione_Cod, TipoTestata, Veg_Cod, GPAI_PA_Ausiliari_COD ")

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


    Public Function LeggixVincoli_Disciplinari_DISC_SCADENZA_SA(ByVal Cod_Regolamento As Integer,
                                                                ByVal Anno As Integer,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_SCADENZA_SA()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome,  ")
            StrSQL.Append(" Max(formulati.Data_Fine_UsoScorte) as Data_Fine_UsoScorte, ")
            StrSQL.Append(" Max(formulati.Data_Revo) as Data_Revo , Max( PA_Ausiliari.Id_Paa) as ID_DISC_SCADENZA_SA ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali,  Formulati, FormulatixPrincipiAttivi, FormulatixSpecieVegetali ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            StrSQL.Append(" And Regolamenti.Cod_regolamento = " & Cod_Regolamento & " ")
            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")


            StrSQL.Append(" And isNull(PA_Ausiliari.ID_PAA_MISC, 0) = 0  ")

            StrSQL.Append(" And FormulatixSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And FormulatixPrincipiAttivi.Pa_Cod = principiattivi.pa_cod ")
            StrSQL.Append(" And FormulatixPrincipiAttivi.Fr_Cod = Formulati.Fr_Cod ")
            StrSQL.Append(" And FormulatixSpecieVegetali.Fr_Cod = Formulati.Fr_Cod ")

            StrSQL.Append(" And ((convert(datetime, formulati.Data_Fine_UsoScorte, 120) >= '01/01/" & Anno & "' And convert(datetime, formulati.Data_Fine_UsoScorte, 120) <= '31/12/" & Anno & "' And formulati.Data_Fine_UsoScorte Is Not null) ")
            StrSQL.Append(" Or (convert(datetime, formulati.Data_Revo, 120) >= '01/01/" & Anno & "' And convert(datetime, formulati.Data_Revo, 120) <= '31/12/" & Anno & "' And formulati.Data_Revo is not null)) ")

            StrSQL.Append(" Group By ")
            StrSQL.Append(" Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') , Lista_Regioni.REG,  ")
            StrSQL.Append(" PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome  ")

            StrSQL.Append(" Order by ID_DISC_SCADENZA_SA ")


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




    Public Function LeggixVincoli_Disciplinari_DISC_SCADENZA_SA_MISCELE(ByVal Cod_Regolamento As Integer,
                                                                        ByVal Anno As Integer,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                        Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_DISC_SCADENZA_SA_MISCELE()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0


            StrSQL.Append(" Select Distinct Regolamenti.Anno, Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') as Regione, Lista_Regioni.REG as Regione_Cod, ")
            StrSQL.Append(" RaggruppamentiColturaliDPI.Id_RCDPI, RaggruppamentiColturaliDPI.Nome,  PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES,  ")
            StrSQL.Append(" PA_Ausiliari.ID_PAA_MISC as ID_DISC_SCADENZA_SA, ")

            StrSQL.Append("  (Select Max( F2.Data_Fine_UsoScorte)  From Formulati F2, PA_Ausiliari PA2, FormulatixSpecieVegetali FxSV2, ")
            StrSQL.Append("  FormulatixPrincipiAttivi FxFPA2, SpecieVegetali SV2 ")
            StrSQL.Append("  Where PA2.ID_PAA = PA_Ausiliari.Id_PAA_MISC")
            StrSQL.Append("  And SpecieVegetali.Veg_Cod = SV2.Veg_Cod")
            StrSQL.Append("  And FxSV2.Veg_Cod = SV2.Veg_Cod")
            StrSQL.Append("  And FxFPA2.Pa_Cod = PA_Ausiliari.Pa_cod ")
            StrSQL.Append("  And FxFPA2.Fr_Cod = F2.Fr_Cod ")
            StrSQL.Append("  And FxSV2.Fr_Cod = F2.Fr_Cod)   as Data_Fine_UsoScorte, ")

            StrSQL.Append("  (Select Max( F3.Data_Revo)  From Formulati F3, PA_Ausiliari PA3, FormulatixSpecieVegetali FxSV3,")
            StrSQL.Append("  FormulatixPrincipiAttivi FxFPA, SpecieVegetali SV3 ")
            StrSQL.Append("  Where PA3.ID_PAA = PA_Ausiliari.Id_PAA_MISC ")
            StrSQL.Append("  And SpecieVegetali.Veg_Cod = SV3.Veg_Cod ")
            StrSQL.Append("  And FxSV3.Veg_Cod = SV3.Veg_Cod ")
            StrSQL.Append("  And FxFPA.Pa_Cod = PA_Ausiliari.pa_cod ")
            StrSQL.Append("  And FxFPA.Fr_Cod = F3.Fr_Cod ")
            StrSQL.Append("  And FxSV3.Fr_Cod = F3.Fr_Cod)   as Data_Revo ")


            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, DifesaTestata, DifesaRighe, PA_Ausiliari, ")
            StrSQL.Append(" PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, Formulati, FormulatixPrincipiAttivi, FormulatixSpecieVegetali")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = " & Cod_Regolamento & " ")
            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.Append(" And PA_Ausiliari.ID_PAA_MISC in (Select Id_PAA From PA_Ausiliari Where ")

            StrSQL.Append(" ((convert(datetime, formulati.Data_Fine_UsoScorte, 120) >= '01/01/" & Anno & "' And convert(datetime, formulati.Data_Fine_UsoScorte, 120) <= '31/12/" & Anno & "' And formulati.Data_Fine_UsoScorte Is Not null) ")
            StrSQL.Append(" Or (convert(datetime, formulati.Data_Revo, 120) >= '01/01/" & Anno & "' And convert(datetime, formulati.Data_Revo, 120) <= '31/12/" & Anno & "' And formulati.Data_Revo is not null))) ")

            StrSQL.Append(" And FormulatixSpecieVegetali.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And FormulatixPrincipiAttivi.Pa_Cod = principiattivi.pa_cod ")
            StrSQL.Append(" And FormulatixPrincipiAttivi.Fr_Cod = Formulati.Fr_Cod ")
            StrSQL.Append(" And FormulatixSpecieVegetali.Fr_Cod = Formulati.Fr_Cod ")


            StrSQL.Append(" Order by ID_DISC_SCADENZA_SA ")


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



    Public Function LeggixVincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                             Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct Isnull(GruppoAvversita.AV_GRU, 0) As Gruppo_Avversita_Cod, Isnull(GruppoAvversita.AV_GRU_DES, '') as Gruppo_Avversita_Des, ")
            StrSQL.Append(" Isnull(GruppoAvversita.AV_GRU_DES_LAT, '') as Gruppo_Avversita_Des_Lat, RaggruppamentiColturaliDPI.Id_RCDPI ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti ")
            StrSQL.Append(" Left Outer Join Avversita On (Infestanti.Av_Cod = Avversita.Av_Cod) ")
            StrSQL.Append(" Left Outer Join GruppoAvversita On (Infestanti.AV_GRU = GruppoAvversita.AV_GRU), ")

            StrSQL.Append(" RaggruppamentiDPIXSpecieVegetali ")
            StrSQL.Append(" Left outer join GruppoFinalita On (Isnull(RaggruppamentiDPIXSpecieVegetali.Grfi_COD, 0) = Isnull(GruppoFinalita.GRFI_COD,0)) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And GruppoAvversita.AV_GRU <> 0 ")

            StrSQL.Append(" Order by Gruppo_Avversita_Cod")


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



    Public Function LeggixVincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                            Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct Isnull(Avversita.AV_COD, 0) As Avversita_Cod, Isnull(Avversita.AV_DES_VOL, '') as Avversita_Des, Isnull(Avversita.AV_DES_LAT, '') as Avversita_Des_Lat, ")
            StrSQL.Append(" Isnull(GruppoAvversita.AV_GRU, 0) As Gruppo_Avversita_Cod, Isnull(GruppoAvversita.AV_GRU_DES, '') as Gruppo_Avversita_Des, Isnull(GruppoAvversita.AV_GRU_DES_LAT, '') as Gruppo_Avversita_Des_Lat ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti ")
            StrSQL.Append(" Left Outer Join Avversita On (Infestanti.Av_Cod = Avversita.Av_Cod) ")
            StrSQL.Append(" Left Outer Join GruppoAvversita On (Infestanti.AV_GRU = GruppoAvversita.AV_GRU), ")

            StrSQL.Append(" RaggruppamentiDPIXSpecieVegetali ")
            StrSQL.Append(" Left outer join GruppoFinalita On (Isnull(RaggruppamentiDPIXSpecieVegetali.Grfi_COD, 0) = Isnull(GruppoFinalita.GRFI_COD,0)) ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And Avversita.AV_COD <> 0 ")

            StrSQL.Append(" Order by Gruppo_Avversita_Cod, Avversita_Cod")


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





    Public Function LeggixVincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT(
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                            Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct PrincipiAttivi.PA_COD, PrincipiAttivi.PA_DES, ")
            StrSQL.Append(" GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_Des ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
            StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

            StrSQL.Append("  And (Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0  or Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 or Isnull(LimitazioniUso.NmaxTratt, 0) > 0)   ")


            StrSQL.Append(" Order by GPAI_PA_Ausiliari_COD, Pa_COD ")

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



    Public Function LeggixVincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI(
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                            Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixVincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD, GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_Des ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, ")
            StrSQL.Append(" LimitazioniUso, GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari, GruppiPAInterazioni_PA_Ausiliari ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")

            StrSQL.Append(" And (LimitazioniUso.Id_paa = PA_Ausiliari.ID_PAA Or LimitazioniUso.Id_Paa = PA_Ausiliari.Id_Paa_Misc) ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD = LimitazioniUso.GPAI_PA_Ausiliari_COD ")
            StrSQL.Append(" And GruppiPAInterazioni_PA_Ausiliari.GPAI_PA_Ausiliari_COD = GruppiPAInterazioni_PA_AusiliariXPA_Ausiliari.GPAI_PA_Ausiliari_COD ")

            StrSQL.Append("  And (Isnull(LimitazioniUso.NmaxTratt_Anno, 0) > 0  or Isnull(LimitazioniUso.NmaxTratt_Ciclo_colt, 0) > 0 or Isnull(LimitazioniUso.NmaxTratt, 0) > 0)   ")


            StrSQL.Append(" Order by GPAI_PA_Ausiliari_COD ")

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




    Public Function LeggiTabellaAbacoDPI(ByVal Suffisso_Tabella As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggiTabellaAbacoDPI()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select * ")
            StrSQL.Append(" FROM  Abaco_" & Suffisso_Tabella & " ")
            StrSQL.Append(" WHERE    Inviato = 0  ")

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





    Public Function LeggiTabellaAbacoUtility(ByVal Tabella As String,
                                             ByVal SelectList As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggiTabellaAbacoUtility()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select " & SelectList & "  ")
            StrSQL.Append(" FROM  " & Tabella & " ")


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



    Public Function LeggixUMDosi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                 Optional ByVal xFiltroAggiuntivo As String = "") As DataTable
        '
        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_R.LeggixUMDosi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0


            StrSQL.Append(" Select Distinct IsNull(PA_Ausiliari.Dose_UDM, 0) as Dose_UDM, unitaMisuraDPI.Udm_Sim  ")
            StrSQL.Append(" From Regolamenti, RaggruppamentiColturaliDPI, RaggruppamentiColturaliDPIXRegolamenti, SpecieVegetali, Lista_Regioni, RegolamentixDifesaTestata, ")
            StrSQL.Append(" DifesaTestata, DifesaRighe, PA_Ausiliari, PrincipiAttivi, Infestanti, RaggruppamentiDPIXSpecieVegetali, unitaMisuraDPI ")

            StrSQL.Append(" Where 1 = 1 ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.Append(" And Regolamenti.Cod_regolamento = RaggruppamentiColturaliDPIXRegolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiColturaliDPIXRegolamenti.ID_RCDPI = RaggruppamentiDPIXSpecieVegetali.ID_RCDPI ")
            StrSQL.Append(" And RaggruppamentiDPIXSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" And Lista_Regioni.Regione_Des = Replace(Regolamenti.NomeEsteso, Regolamenti.Anno, '') COLLATE SQL_Latin1_General_CP1_CI_AS ")
            StrSQL.Append(" And RegolamentixDifesaTestata.COD_REGOLAMENTO = Regolamenti.COD_REGOLAMENTO ")
            StrSQL.Append(" And DifesaTestata.DFT_COD = RegolamentixDifesaTestata.DFT_COD ")
            StrSQL.Append(" And DifesaRighe.DFT_COD = DifesaTestata.DFT_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = DifesaRighe.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = DifesaRighe.DFR_COD")
            StrSQL.Append(" And Infestanti.DFT_COD = PA_Ausiliari.DFT_COD")
            StrSQL.Append(" And Infestanti.DFR_COD = PA_Ausiliari.DFR_COD")
            StrSQL.Append(" And PA_Ausiliari.PA_COD = PrincipiAttivi.PA_COD")
            StrSQL.Append(" And Difesatestata.ID_RCDPI = RaggruppamentiColturaliDPI.ID_RCDPI ")
            StrSQL.Append(" And PA_Ausiliari.Dose_UDM = unitaMisuraDPI.UDM_COD ")

            'Filtro i dosaggi valorizzati e le miscele
            StrSQL.Append(" And (Isnull(PA_Ausiliari.Dose_Max_Anno, 0) <> 0 Or isNull(PA_Ausiliari.ID_PAA_MISC, 0) <> 0 ) ")

            StrSQL.Append(" Order by Dose_UDM ")

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


Public Class Dpi_Abaco_W

    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi_Abaco_Disciplinari(
                            ByVal ID_DISCIPLINARE As Integer,
                            ByVal COD_REGIONE As Integer, ByVal DECO_REGIONE As String,
                            ByVal COD_COLT_DISC As Integer, ByVal DECO_COLT_DISC As String,
                            ByVal COD_COLT As Integer, ByVal DECO_COLT As String,
                            ByVal COD_AVV_DISC As String, ByVal DECO_AVV_DISC As String,
                            ByVal COD_AVV As Integer, ByVal DECO_AVV As String,
                            ByVal COD_SA1 As Integer, ByVal DECO_SA1 As String,
                            ByVal COD_SA2 As Integer, ByVal DECO_SA2 As String,
                            ByVal COD_SA3 As Integer, ByVal DECO_SA3 As String,
                            ByVal ANNO As Integer,
                            ByVal NOTE As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Disciplinari()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Disciplinari " &
                    "     (ID_DISCIPLINARE,     COD_REGIONE,   DECO_REGIONE, " &
                    "      COD_COLT_DISC,       DECO_COLT_DISC,      " &
                    "      COD_COLT,            DECO_COLT,  " &
                    "      COD_AVV_DISC,        DECO_AVV_DISC, " &
                    "      COD_AVV,             DECO_AVV, " &
                    "      COD_SA1,             DECO_SA1, " &
                    "      COD_SA2,             DECO_SA2, " &
                    "      COD_SA3,             DECO_SA3, " &
                    "      ANNO,                NOTE, " &
                    "      LastUpdate,  " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")



            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISCIPLINARE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_REGIONE) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_COLT_DISC) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_COLT) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_AVV_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_AVV_DISC) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_AVV) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_AVV) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_SA1) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_SA1) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_SA2) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_SA2) & "' " &
                   "       , " & Agro_SQL_SaveNum(COD_SA3) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECO_SA3) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveDate(Now) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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

    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX(ByVal ID_DISC_DOSE_MAX As Integer,
                                                                    ByVal COD_REGIONE As Integer,
                                                                    ByVal COD_COLT_DISC As Integer,
                                                                    ByVal CODICE_PA1 As Integer,
                                                                    ByVal CODICE_PA2 As Integer,
                                                                    ByVal CODICE_PA3 As Integer,
                                                                    ByVal DOSE As Integer, ByVal COD_UM_DOSE As Integer,
                                                                    ByVal NOTE As String,
                                                                    ByVal ANNO As Integer,
                                                                    ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                    ByVal PERC_SA As Decimal, ByVal DOSE_GL As Integer,
                                                                    ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX " &
                    "     (ID_DISC_DOSE_MAX,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      CODICE_PA1, " &
                    "      CODICE_PA2, " &
                    "      CODICE_PA3, " &
                    "      DOSE,  COD_UM_DOSE, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      PERC_SA,    DOSE_GL, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_DOSE_MAX) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA1) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA2) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA3) & "  " &
                   "       , " & Agro_SQL_SaveNum(DOSE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_UM_DOSE) & "  " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(PERC_SA) & "  " &
                   "       , " & Agro_SQL_SaveNum(DOSE_GL) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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


    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV(ByVal ID_DISC_N_MAX_TRATT_X_AVV As Integer,
                                                                             ByVal COD_REGIONE As Integer,
                                                                             ByVal COD_COLT_DISC As Integer,
                                                                             ByVal COD_AVVERSITA As String,
                                                                             ByVal NUM_MAX_TRATT As Integer,
                                                                             ByVal MAX_TRATT_PER As String,
                                                                             ByVal NOTE As String,
                                                                             ByVal ANNO As Integer,
                                                                             ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                             ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                             ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_AVV " &
                    "     (ID_DISC_N_MAX_TRATT_X_AVV,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      COD_AVVERSITA, " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_AVV) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(COD_AVVERSITA) & "' " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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


    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV(ByVal ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV As Integer,
                                                                                    ByVal COD_REGIONE As Integer,
                                                                                    ByVal COD_COLT_DISC As Integer,
                                                                                    ByVal ID_GRUPPO_AVV_DISC As Integer,
                                                                                    ByVal NUM_MAX_TRATT As Integer,
                                                                                    ByVal MAX_TRATT_PER As String,
                                                                                    ByVal NOTE As String,
                                                                                    ByVal ANNO As Integer,
                                                                                    ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                    ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV " &
                    "     (ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      ID_GRUPPO_AVV_DISC, " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_GRUPPO_AVV) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                    "       , " & Agro_SQL_SaveNum(ID_GRUPPO_AVV_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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



    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA(ByVal ID_DISC_N_MAX_TRATT_X_GRUPPO_SA As Integer,
                                                                                   ByVal COD_REGIONE As Integer,
                                                                                   ByVal COD_COLT_DISC As Integer,
                                                                                   ByVal ID_GRUPPO_SA_DISC As Integer,
                                                                                   ByVal NUM_MAX_TRATT As Integer,
                                                                                   ByVal MAX_TRATT_PER As String,
                                                                                   ByVal NOTE As String,
                                                                                   ByVal ANNO As Integer,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_AVV()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPO_SA " &
                    "     (ID_DISC_N_MAX_TRATT_X_GRUPPO_SA,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      ID_GRUPPO_SA_DISC, " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_GRUPPO_SA) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                    "       , " & Agro_SQL_SaveNum(ID_GRUPPO_SA_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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



    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA(ByVal ID_DISC_N_MAX_TRATT_X_SA As Integer,
                                                                            ByVal COD_REGIONE As Integer,
                                                                            ByVal ID_GRUPPO_SA_DISC As Integer,
                                                                            ByVal NUM_MAX_TRATT As Integer,
                                                                            ByVal MAX_TRATT_PER As String,
                                                                            ByVal NOTE As String,
                                                                            ByVal ANNO As Integer,
                                                                            ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                            ByVal CODICE_PA1 As Integer,
                                                                            ByVal CODICE_PA2 As Integer,
                                                                            ByVal CODICE_PA3 As Integer,
                                                                            ByVal CodRegione As Integer,
                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA " &
                    "     (ID_DISC_N_MAX_TRATT_X_SA,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      CODICE_PA1,  CODICE_PA2, CODICE_PA3,  CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_SA) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                    "       , " & Agro_SQL_SaveNum(ID_GRUPPO_SA_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA1) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA2) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA3) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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

    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO(ByVal ID_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO As Integer,
                                                                                       ByVal COD_REGIONE As Integer,
                                                                                       ByVal COD_COLT_DISC As Integer,
                                                                                       ByVal COD_GRUPPOCHIMICO As Integer,
                                                                                       ByVal NUM_MAX_TRATT As Integer,
                                                                                       ByVal MAX_TRATT_PER As String,
                                                                                       ByVal NOTE As String,
                                                                                       ByVal ANNO As Integer,
                                                                                       ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                       ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                       ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO " &
                    "     (ID_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      COD_GRUPPOCHIMICO, " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_GRUPPOCHIMICO) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                    "       , " & Agro_SQL_SaveNum(COD_GRUPPOCHIMICO) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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




    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA(ByVal ID_DISC_N_MAX_TRATT_X_SA As Integer,
                                                                    ByVal COD_REGIONE As Integer,
                                                                    ByVal COD_COLT_DISC As Integer,
                                                                    ByVal CODICE_PA1 As Integer,
                                                                    ByVal CODICE_PA2 As Integer,
                                                                    ByVal CODICE_PA3 As Integer,
                                                                    ByVal NUM_MAX_TRATT As Integer, ByVal MAX_TRATT_PER As String,
                                                                    ByVal NOTE As String,
                                                                    ByVal ANNO As Integer,
                                                                    ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                    ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_SA " &
                    "     (ID_DISC_N_MAX_TRATT_X_SA,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      CODICE_PA1, " &
                    "      CODICE_PA2, " &
                    "      CODICE_PA3, " &
                    "      NUM_MAX_TRATT,  MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_SA) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA1) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA2) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA3) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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




    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_TIPOLOGIA(ByVal ID_DISC_N_MAX_TRATT_X_TIPOLOGIA As Integer,
                                                                                   ByVal COD_REGIONE As Integer,
                                                                                   ByVal COD_COLT_DISC As Integer,
                                                                                   ByVal COD_TIPO As Integer,
                                                                                   ByVal NUM_MAX_TRATT As Integer,
                                                                                   ByVal MAX_TRATT_PER As String,
                                                                                   ByVal NOTE As String,
                                                                                   ByVal ANNO As Integer,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_TIPOLOGIA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_N_MAX_TRATT_X_TIPOLOGIA " &
                    "     (ID_DISC_N_MAX_TRATT_X_TIPOLOGIA,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      COD_TIPO, " &
                    "      NUM_MAX_TRATT, " &
                    "      MAX_TRATT_PER, " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_N_MAX_TRATT_X_TIPOLOGIA) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                    "       , " & Agro_SQL_SaveNum(COD_TIPO) & "  " &
                   "       , " & Agro_SQL_SaveNum(NUM_MAX_TRATT) & "  " &
                   "       ,'" & Agro_SQL_SaveText(MAX_TRATT_PER) & "' " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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





    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_DISC_SCADENZA_SA(ByVal ID_DISC_SCADENZA_SA As Integer,
                                                                    ByVal COD_REGIONE As Integer,
                                                                    ByVal COD_COLT_DISC As Integer,
                                                                    ByVal CODICE_PA1 As Integer,
                                                                    ByVal CODICE_PA2 As Integer,
                                                                    ByVal CODICE_PA3 As Integer,
                                                                    ByVal SCADENZA As DateTime,
                                                                    ByVal NOTE As String,
                                                                    ByVal ANNO As Integer,
                                                                    ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                    ByVal Codice As Integer, ByVal CodRegione As Integer,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_SCADENZA_SA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_DISC_SCADENZA_SA " &
                    "     (ID_DISC_SCADENZA_SA,     COD_REGIONE, " &
                    "      COD_COLT_DISC,      " &
                    "      CODICE_PA1, " &
                    "      CODICE_PA2, " &
                    "      CODICE_PA3, " &
                    "      SCADENZA,  " &
                    "      NOTE,   ANNO, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Codice,    CodRegione, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_DISC_SCADENZA_SA) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_REGIONE) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA1) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA2) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA3) & "  " &
                  "        , " & Agro_SQL_SaveDate(SCADENZA) & "  " &
                   "       ,'" & Agro_SQL_SaveText(NOTE) & "' " &
                   "       , " & Agro_SQL_SaveNum(ANNO) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , " & Agro_SQL_SaveNum(Codice) & "  " &
                   "       , " & Agro_SQL_SaveNum(CodRegione) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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



    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT(ByVal ID_GRUPPO_AVV_DISC As Integer,
                                                                                   ByVal CODICE_AVV_COLT As String,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_GRUPPI_AVV_DISC_PER_AVV_COLT " &
                    "     (ID_GRUPPO_AVV_DISC,     CODICE_AVV_COLT, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_GRUPPO_AVV_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(CODICE_AVV_COLT) & "' " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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




    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI(ByVal ID_GRUPPO_AVV_DISC As Integer,
                                                                                   ByVal COD_COLT_DISC As Integer,
                                                                                   ByVal NOME As String,
                                                                                   ByVal DESCRIZIONE As String,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_GRUPPI_AVVERSITA_DISCIPLINARI " &
                    "     (ID_GRUPPO_AVV_DISC,  COD_COLT_DISC, " &
                    "      NOME,      " &
                    "      DESCRIZIONE, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_GRUPPO_AVV_DISC) & "  " &
                   "       , " & Agro_SQL_SaveNum(COD_COLT_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(NOME) & "' " &
                   "       ,'" & Agro_SQL_SaveText(DESCRIZIONE) & "' " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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




    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT(ByVal ID_GRUPPO_SA_DISC As Integer,
                                                                                   ByVal CODICE_PA As Integer,
                                                                                   ByVal CODICE_PA2 As Integer,
                                                                                   ByVal CODICE_PA3 As Integer,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISC_PER_PRINC_ATT " &
                    "     (ID_GRUPPO_SA_DISC, " &
                   "       CODICE_PA, " &
                    "      CODICE_PA2, " &
                    "      CODICE_PA3, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_GRUPPO_SA_DISC) & "  " &
                  "       , " & Agro_SQL_SaveNum(CODICE_PA) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA2) & "  " &
                   "       , " & Agro_SQL_SaveNum(CODICE_PA3) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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





    '============================================================================
    Public Function Modifica_CampoInteger(ByVal Suffisso_Tabella As String,
                                          ByVal Campo As String,
                                          ByVal Valore As Integer,
                                          ByVal Filtro As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Modifica_CampoInteger()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Abaco_" & Suffisso_Tabella & " ")
            StrSQL.Append(" SET ")
            StrSQL.Append("  " & Campo & " = " & Agro_SQL_SaveNum(Valore) & " ")

            StrSQL.Append(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(Filtro, , objParametri))

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


    '============================================================================
    Public Function Cancella_Duplicati_Abaco_Disciplinari(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Cancella_Duplicati_Abaco_Disciplinari()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Delete From Abaco_Disciplinari ")
            StrSQL.Append(" Where ID_DISCIPLINARE In ( ")
            StrSQL.Append(" Select id_Disciplinare from Abaco_Disciplinari Ab ")
            StrSQL.Append(" where(exists(select * from Abaco_Disciplinari Ab2 where Ab.ID_DISCIPLINARE > Ab2.ID_DISCIPLINARE And  Ab.cod_sa1 = Ab2.cod_sa2   And Ab.cod_sa2 = Ab2.cod_sa1 ")
            StrSQL.Append("   And ab.COD_REGIONE = ab2.COD_REGIONE ")
            StrSQL.Append("   And ab.cod_colt_disc = ab2.cod_colt_disc ")
            StrSQL.Append("   And ab.cod_avv = ab2.cod_avv ")
            StrSQL.Append("   And ab.cod_avv_disc = ab2.cod_avv_disc ")
            StrSQL.Append("    And ab.anno = ab2.anno )) ")
            StrSQL.Append("  Or ID_DISCIPLINARE In ( ")
            StrSQL.Append("  Select id_Disciplinare from Abaco_Disciplinari Ab")
            StrSQL.Append("  where exists(select * from Abaco_Disciplinari Ab2 where Ab.ID_DISCIPLINARE > Ab2.ID_DISCIPLINARE And  Ab.cod_sa1 = Ab2.cod_sa1   And Ab.cod_sa2 = Ab2.cod_sa2 And Ab.cod_sa3 = Ab2.cod_sa3 ")
            StrSQL.Append("  And ab.COD_REGIONE = ab2.COD_REGIONE ")
            StrSQL.Append("  And ab.cod_colt_disc = ab2.cod_colt_disc ")
            StrSQL.Append("  And ab.cod_avv = ab2.cod_avv ")
            StrSQL.Append("  And ab.cod_avv_disc = ab2.cod_avv_disc ")
            StrSQL.Append("  And ab.anno = ab2.anno ))) ")

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




    '============================================================================
    Public Function Cancella_Duplicati_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Cancella_Duplicati_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Delete From Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX ")
            StrSQL.Append(" Where ID_DISC_DOSE_MAX In ( ")
            StrSQL.Append(" Select ID_DISC_DOSE_MAX from Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX Ab ")
            StrSQL.Append(" where(exists(select * from Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX Ab2 where Ab.ID_DISC_DOSE_MAX > Ab2.ID_DISC_DOSE_MAX And ")
            StrSQL.Append(" Ab.codice_pa1 = Ab2.codice_pa1   and Ab.codice_pa2 = Ab2.codice_pa2 and Ab.codice_pa3 = Ab2.codice_pa3 ")
            StrSQL.Append("   And ab.COD_REGIONE = ab2.COD_REGIONE ")
            StrSQL.Append("   And ab.cod_colt_disc = ab2.cod_colt_disc ")
            StrSQL.Append("   And ab.COD_UM_DOSE = ab2.COD_UM_DOSE   ")
            StrSQL.Append("   And ab.dose = ab2.dose   ")
            StrSQL.Append("   And ab.anno = ab2.anno ))) ")

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





    '============================================================================
    Public Function Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI(ByVal ID_GRUPPO_SA_DISC As Integer,
                                                                                  ByVal NOME As String,
                                                                                   ByVal DESCRIZIONE As String,
                                                                                   ByVal DATA_INS As DateTime, ByVal DATA_UPD As DateTime,
                                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_Vincoli_Disciplinari_GRUPPI_SA_DISCIPLINARI " &
                    "     (ID_GRUPPO_SA_DISC, " &
                    "      NOME,      " &
                    "      DESCRIZIONE, " &
                    "      DATA_INS,    DATA_UPD, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(ID_GRUPPO_SA_DISC) & "  " &
                   "       ,'" & Agro_SQL_SaveText(NOME) & "' " &
                   "       ,'" & Agro_SQL_SaveText(DESCRIZIONE) & "' " &
                   "       , " & Agro_SQL_SaveDate(DATA_INS) & "  " &
                   "       , " & Agro_SQL_SaveDate(DATA_UPD) & "  " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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

    '============================================================================
    Public Function Modifica_CampoString(ByVal Suffisso_Tabella As String,
                                          ByVal Campo As String,
                                          ByVal Valore As String,
                                          ByVal Filtro As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Modifica_CampoString()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Abaco_" & Suffisso_Tabella & " ")
            StrSQL.Append(" SET ")
            StrSQL.Append("  " & Campo & " = '" & Agro_SQL_SaveText(Valore) & "' ")

            StrSQL.Append(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(Filtro, , objParametri))

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




    '============================================================================
    Public Function Cancella(ByVal Suffisso_Tabella As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM  Abaco_" & Suffisso_Tabella & " ")
            StrSQL.Append(" WHERE    Inviato = 0  ")

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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




    '============================================================================
    Public Function Scrivi_Abaco_UMDosi(ByVal CODICE As Integer,
                                        ByVal DECODIFICA As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Abaco_W.Scrivi_Abaco_Vincoli_Disciplinari_DISC_DOSE_MAX()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Abaco_UMDosi " &
                    "     (CODICE,     DECODIFICA, " &
                    "      Inviato,             DataInvio,  " &
                    "      Data_Creazione,      Data_Modifica,  " &
                    "      Username_Creazione,  Username_Modifica,  " &
                    "      Validita_Inizio,     Validita_Fine  " &
                     "          )")

            StrSQL.Append(" VALUES (" &
                   "         " & Agro_SQL_SaveNum(CODICE) & "  " &
                   "       ,'" & Agro_SQL_SaveText(DECODIFICA) & "' " &
                   "       , 0 " &
                   "       , Null " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "       , " & Agro_SQL_SaveDate(Now.Today) & "  " &
                   "         , 'agronauta'" &
                   "         , 'agronauta'" &
                   "         , '01/01/1900'" &
                   "         , '31/12/2100')")

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