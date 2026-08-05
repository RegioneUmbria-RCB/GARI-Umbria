Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ElaborazioneDati_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function OperazioneAgendaElaborabiliLista(
            ByVal DataDa As DateTime,
            ByVal DataA As DateTime,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select *  ")
            Stb.AppendLine(" from ( ")

            Stb.AppendLine(" Select  1 as DSS_Analisi_tipologia_elaborazione, o.lav_cod, 'Operazioni di Agenda: ' +  o.LAV_DES as LAV_DES  ")
            Stb.AppendLine(" From agenda a ")
            Stb.AppendLine("  inner Join operazioni o  ")
            Stb.AppendLine("         On a.lav_cod = o.lav_cod ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" where o.gru_op In (1, 2) ")
            Stb.AppendLine(" And o.lav_cod in (" & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA)
            Stb.AppendLine(", " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
            Stb.AppendLine(", " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO)
            Stb.AppendLine(", " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_DANNI_RACCOLTA & ") ")
            'Stb.AppendLine(" And o.lav_cod = " & AgronicaCoreDataProvider.CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA)
            'Stb.AppendLine(" And o.lav_cod Not in (126) ")
            Stb.AppendLine(" group by o.lav_cod, o.LAV_DES")
            Stb.AppendLine(") a ")

            Stb.AppendLine(" union ")
            Stb.AppendLine(" Select 2, t.Analisi_Tipologia_Cod, 'Piani di Campionamento / Analisi: ' +  Analisi_Tipologia_des ")
            Stb.AppendLine(" From Analisi_Tipologia t ")
            Stb.AppendLine("  inner Join PDC_Analisi pp ")
            Stb.AppendLine("         On t.Analisi_Tipologia_Cod = pp.Analisi_Tipologia_Cod ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" union ")
            Stb.AppendLine(" Select 3, -1000 As Analisi_Tipologia_Cod, 'Analisi del Terreno'")


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
    '##############################################################################################
    Public Function AnalisiLetteConJoinPDC(
            ByVal DataDa As DateTime,
            ByVal DataA As DateTime,
            ByVal Analisi_Tipologia_cod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal IDTestataTemp As Integer = 0
        ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("    Select  ")

            Stb.AppendLine("  --chiave  Impianto ")

            Stb.AppendLine("    PDC_Dettagli.piva ")
            Stb.AppendLine("  , PDC_Dettagli.sa_cod ")
            Stb.AppendLine("  , PDC_Dettagli.appezza ")
            Stb.AppendLine("  , PDC_Dettagli.id_reg ")


            Stb.AppendLine("  --chiave  Analisi ")
            Stb.AppendLine("     , Analisi_Testata.Analisi_Testata_Cod as chiaveAnalisi ")

            Stb.AppendLine("  --descrizione Impianto ")


            Stb.AppendLine("    , '' AS kPIN ")
            Stb.AppendLine("	, '' AS BlockName ")

            Stb.AppendLine("  , PDC_Dettagli.Rag_Soc as RagioneSociale ")
            Stb.AppendLine("  , PDC_Dettagli.Sa_Nome ")
            Stb.AppendLine("  , SpecieVegetali.veg_des ")
            Stb.AppendLine("  , Cultivar.Cul_des ")
            Stb.AppendLine("  , pdc_dettagli.Sup_Imp as sup_ha ")
            Stb.AppendLine("  , pdc_dettagli.Sup_Imp ")
            Stb.AppendLine("  , '' as Appezzamento_indirizzo ")
            Stb.AppendLine("  , PDC_Dettagli.App_Nome as Appezzamento_Descrizione ")

            Stb.AppendLine("  --descrizione operazione ")

            Stb.AppendLine("  , Analisi_Tipologia.Analisi_Tipologia_Des as Descrizione_Operazione ")

            Stb.AppendLine("    --descrizione rilievo ")
            Stb.AppendLine("  , Analisi_Testata.Analisi_Testata_Data_Fine AS DataOperazione ")
            Stb.AppendLine("  , Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod as indGenericCod ")
            Stb.AppendLine("  , isNull(analisi_parametro_des , '')  as indGenericDes ")
            Stb.AppendLine("  , Analisi_Tipologia_Dettagli.UDM_Cod ")
            Stb.AppendLine("  , UnitaMisura.UDM_DES ")
            Stb.AppendLine("  , isNull(Analisi_Dettagli.Analisi_Dettaglio_Valore_1, 0) as Qta ")

            Stb.AppendLine(" , (SELECT TOP 1 CONVERT(VARCHAR, Movimenti.Data_Movimento, 103) ")
            Stb.AppendLine("     FROM    Agenda ")
            Stb.AppendLine("             INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine(" 			 INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine(" 			 INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            Stb.AppendLine(" 			 INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND Imprese_Progetti.sa_cod = Mov_Destinazioni.sa_cod AND Imprese_Progetti.appezza = Mov_Destinazioni.APPEZZA AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("     WHERE   Agenda.Lav_Cod = 125 AND Movimenti.Cau_Mov = '2200' ")
            Stb.AppendLine(" 			 AND Mov_Destinazioni.Piva = pdc_dettagli.PIVA AND Mov_Destinazioni.sa_cod = pdc_dettagli.sa_cod AND Mov_Destinazioni.appezza = pdc_dettagli.APPEZZA AND Mov_Destinazioni.Id_Destinazione = pdc_dettagli.ID_REG ")
            Stb.AppendLine("             AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
            Stb.AppendLine("             AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")
            Stb.AppendLine("     ORDER BY Movimenti.Data_Movimento) AS Data_Raccolta ")

            Stb.AppendLine("  From PDC_Dettagli    ")


            If IDTestataTemp <> 0 Then
                Stb.AppendLine("             INNER Join __tmp_FiltroImpianti f  ")
                Stb.AppendLine("                On pdc_dettagli.piva = f.piva  ")
                Stb.AppendLine("                And PDC_Dettagli.Sa_Cod = f.sa_cod  ")
                Stb.AppendLine("                And PDC_Dettagli.Appezza = f.appezza  ")
                Stb.AppendLine("                And PDC_Dettagli.Id_Reg = f. id_reg  ")
                Stb.AppendLine("                And f.idTestataTemp = " & IDTestataTemp)
            End If


            Stb.AppendLine("             INNER Join PDC_Campioni  ")
            Stb.AppendLine("                 On PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser  ")
            Stb.AppendLine("              And PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata  ")
            Stb.AppendLine("              And PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          INNER Join PDC_Analisi  ")
            Stb.AppendLine("                 On PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser  ")
            Stb.AppendLine("              And PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata  ")
            Stb.AppendLine("              And PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli  ")
            Stb.AppendLine("              And PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          INNER Join Analisi_Testata  ")
            Stb.AppendLine("                 On PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod  ")
            Stb.AppendLine("              And PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          INNER Join Analisi_Tipologia  ")
            Stb.AppendLine("                 On PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod  ")
            Stb.AppendLine("              And PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          INNER Join Analisi_Tipologia_Dettagli  ")
            Stb.AppendLine("                 On Analisi_Tipologia.PivaSuperUser = Analisi_Tipologia_Dettagli.PivaSuperUser  ")
            Stb.AppendLine("              And Analisi_Tipologia.Analisi_Tipologia_Cod = Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left Join UnitaMisura  ")
            Stb.AppendLine("                 On Analisi_Tipologia_Dettagli.UDM_Cod = UnitaMisura.UDM_COD    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left Join PDC_Dettagli AS PDC_Dettagli_1  ")
            Stb.AppendLine("                 On PDC_Campioni.PivaSuperUser = PDC_Dettagli_1.PivaSuperUser  ")
            Stb.AppendLine("              And PDC_Campioni.ID_PDC_Testata = PDC_Dettagli_1.ID_PDC_Testata  ")
            Stb.AppendLine("              And PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli_1.ID_PDC_Dettagli    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left Join SpecieVegetali  ")
            Stb.AppendLine("                 On PDC_Dettagli_1.Veg_Cod = SpecieVegetali.Veg_Cod    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left Join Cultivar  ")
            Stb.AppendLine("                 On PDC_Dettagli_1.Cul_Cod = Cultivar.Cul_Cod    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left OUTER JOIN Analisi_Parametri  ")
            Stb.AppendLine("                 On Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod    ")
            Stb.AppendLine("  ")
            Stb.AppendLine("          Left OUTER JOIN Analisi_Dettagli  ")
            Stb.AppendLine("                 On Analisi_Testata.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser  ")
            Stb.AppendLine("              And Analisi_Testata.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod  ")
            Stb.AppendLine("              And Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Dettagli.Analisi_Parametro_Cod   ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  WHERE 1 = 1  ")

            Stb.AppendLine("  And Analisi_Testata.Analisi_Testata_Data_Fine <=  " & Agro_SQL_SaveDate(DataA))
            Stb.AppendLine("  And Analisi_Testata.Analisi_Testata_Data_Fine >=  " & Agro_SQL_SaveDate(DataDa))

            If Analisi_Tipologia_cod <> 0 Then
                Stb.AppendLine("  and Analisi_Tipologia.Analisi_Tipologia_cod = " & Analisi_Tipologia_cod)
            End If


            Stb.AppendLine("  order by Analisi_Testata.Analisi_Testata_Data_Fine, PDC_Dettagli.PIVA, PDC_Dettagli.sa_cod, PDC_Dettagli.appezza, PDC_Dettagli.id_reg")




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

    '##############################################################################################
    Public Function AnalisiLetteSenzaPDC(
            ByVal DataDa As DateTime,
            ByVal DataA As DateTime,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal IDTestataTemp As Integer = 0
        ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Dim NuovaQuery As Boolean = True
            If NuovaQuery Then

                Dim StbSelect As New System.Text.StringBuilder
                StbSelect.Length = 0
                StbSelect.AppendLine("  --descrizione rilievo ")
                StbSelect.AppendLine("  , a_t.Analisi_Testata_DES AS Descrizione_Analisi ")
                StbSelect.AppendLine("  , a_t.Analisi_Testata_Data_Inizio AS DataOperazione ")
                StbSelect.AppendLine("  , a_t.Analisi_Testata_Data_Fine AS DataOperazioneFine ")
                StbSelect.AppendLine("  , ap.Analisi_Parametro_Cod AS indGenericCod ")
                StbSelect.AppendLine("  , ISNULL(analisi_parametro_des, '') AS indGenericDes ")
                StbSelect.AppendLine("  , ap.Analisi_Parametro_UdM AS udm_cod ")
                StbSelect.AppendLine("  , udm.UDM_DES ")
                StbSelect.AppendLine("  , ISNULL(d.Analisi_Dettaglio_Valore_1, 0) AS qta ")

                Dim StbFrom As New System.Text.StringBuilder
                StbFrom.Length = 0
                StbFrom.AppendLine("FROM Analisi_Testata a_t ")
                StbFrom.AppendLine("INNER JOIN Analisi_Tipologia tt				ON tt.analisi_tipologia_tipo = a_t.Analisi_Testata_Tipo ")
                StbFrom.AppendLine("INNER JOIN Analisi_Dettagli d				ON a_t.Analisi_Testata_cod = d.Analisi_Testata_Cod AND a_t.Analisi_SuperUser = d.Analisi_SuperUser ")
                StbFrom.AppendLine("INNER JOIN Analisi_Parametri ap				ON ap.Analisi_Parametro_Cod = d.Analisi_Parametro_Cod ")
                StbFrom.AppendLine("INNER JOIN UnitaMisura udm					ON udm.udm_cod = ap.Analisi_Parametro_UdM")
                StbFrom.AppendLine("INNER JOIN Analisi_EntitaxTestata aXapp		ON a_t.Analisi_Testata_cod = aXapp.Analisi_Testata_Cod AND a_t.Analisi_SuperUser = aXapp.Analisi_SuperUser ")
                StbFrom.AppendLine("INNER JOIN Imprese i						ON i.piva = aXapp.piva ")

                Dim StrWhere As String = "( (a_t.Analisi_Testata_Data_Inizio <= @DATA_F) AND (a_t.Analisi_Testata_Data_Fine >= @DATA_I) )"

                Stb.AppendLine("DECLARE @DATA_I DATETIME, @DATA_F DATETIME ")
                Stb.AppendLine("SET @DATA_I = " & Agro_SQL_SaveDate(DataDa))
                Stb.AppendLine("SET @DATA_F = " & Agro_SQL_SaveDate(DataA))
                Stb.AppendLine()
                Stb.AppendLine("SELECT * FROM ( ")
                Stb.AppendLine()
                Stb.AppendLine("SELECT  ")
                Stb.AppendLine("    --chiave impianto ")
                Stb.AppendLine("    aXapp.piva ")
                Stb.AppendLine("    , axApp.sa_cod ")
                Stb.AppendLine("    , axApp.appezza ")
                Stb.AppendLine("    , reg.id_reg ")
                Stb.AppendLine("    --chiave analisi ")
                Stb.AppendLine("    , a_t.Analisi_Testata_Cod AS chiaveAnalisi ")
                Stb.AppendLine("    --descrizione impianto ")

                Stb.AppendLine("    , '' AS kPIN ")
                Stb.AppendLine("	, '' AS BlockName ")

                Stb.AppendLine("    , i.Rag_Soc AS RagioneSociale ")
                Stb.AppendLine("    , sa.Sa_Nome ")
                Stb.AppendLine("    , veg.veg_des ")
                Stb.AppendLine("    , cul.Cul_des ")
                Stb.AppendLine("    , reg.Sup_Imp ")
                Stb.AppendLine("    , app.App_Nome AS Appezzamento_Descrizione ")
                Stb.Append(StbSelect.ToString())
                Stb.AppendLine()
                Stb.Append(StbFrom.ToString())
                Stb.AppendLine("INNER JOIN Centri_Aziendali sa				ON sa.piva = aXapp.piva AND sa.sa_cod = aXapp.sa_Cod ")
                Stb.AppendLine("INNER JOIN Appezzamento app					ON app.piva = aXapp.Piva AND app.sa_cod = aXapp.sa_cod AND app.appezza = aXapp.appezza ")
                Stb.AppendLine("INNER JOIN reg_impianti reg					ON app.piva = reg.Piva And app.sa_cod = reg.sa_cod AND app.appezza = reg.appezza ")
                Stb.AppendLine("INNER JOIN Cultivar cul						ON cul.Cul_Cod = reg.CUL_COD ")
                Stb.AppendLine("INNER JOIN specieVegetali veg				ON veg.Veg_Cod = cul.Veg_Cod ")
                Stb.AppendLine()
                Stb.AppendLine("WHERE " & StrWhere)
                Stb.AppendLine()
                Stb.AppendLine("UNION ")
                Stb.AppendLine()
                Stb.AppendLine("--rilasso il vincolo su appezza")
                Stb.AppendLine("SELECT")
                Stb.AppendLine("    --chiave impianto ")
                Stb.AppendLine("    aXapp.piva ")
                Stb.AppendLine("    , axApp.sa_cod ")
                Stb.AppendLine("    , 0 AS appezza ")
                Stb.AppendLine("    , 0 AS id_reg ")
                Stb.AppendLine("    --chiave analisi ")
                Stb.AppendLine("    , a_t.Analisi_Testata_Cod AS chiaveAnalisi ")
                Stb.AppendLine("    --descrizione impianto ")

                Stb.AppendLine("    , '' AS kPIN ")
                Stb.AppendLine("	, '' AS BlockName ")

                Stb.AppendLine("    , i.Rag_Soc AS RagioneSociale ")
                Stb.AppendLine("    , sa.Sa_Nome ")
                Stb.AppendLine("    , '' AS veg_des ")
                Stb.AppendLine("    , '' AS Cul_des ")
                Stb.AppendLine("    , -1 AS Sup_Imp ")
                Stb.AppendLine("    , '' AS Appezzamento_Descrizione ")
                Stb.Append(StbSelect.ToString())
                Stb.AppendLine()
                Stb.Append(StbFrom.ToString())
                Stb.AppendLine("INNER JOIN Centri_Aziendali sa				ON sa.piva = aXapp.piva AND sa.sa_cod = aXapp.sa_Cod ")
                Stb.AppendLine()
                Stb.AppendLine("WHERE " & StrWhere)
                Stb.AppendLine("    AND aXapp.appezza = 0")
                Stb.AppendLine()
                Stb.AppendLine("UNION ")
                Stb.AppendLine()
                Stb.AppendLine("--rilasso anche il vincolo su sa_cod")
                Stb.AppendLine("SELECT")
                Stb.AppendLine("    --chiave impianto ")
                Stb.AppendLine("    aXapp.piva ")
                Stb.AppendLine("    , axApp.sa_cod ")
                Stb.AppendLine("    , 0 AS appezza ")
                Stb.AppendLine("    , 0 AS id_reg ")
                Stb.AppendLine("    --chiave analisi ")
                Stb.AppendLine("    , a_t.Analisi_Testata_Cod AS chiaveAnalisi ")
                Stb.AppendLine("    --descrizione impianto ")

                Stb.AppendLine("    , '' AS kPIN ")
                Stb.AppendLine("	, '' AS BlockName ")

                Stb.AppendLine("    , i.Rag_Soc AS RagioneSociale ")
                Stb.AppendLine("    , '' AS Sa_Nome ")
                Stb.AppendLine("    , '' AS veg_des ")
                Stb.AppendLine("    , '' AS Cul_des ")
                Stb.AppendLine("    , -1 AS Sup_Imp ")
                Stb.AppendLine("    , '' AS Appezzamento_Descrizione ")
                Stb.Append(StbSelect.ToString())
                Stb.AppendLine()
                Stb.Append(StbFrom.ToString())
                Stb.AppendLine()
                Stb.AppendLine("WHERE " & StrWhere)
                Stb.AppendLine("    AND aXapp.appezza = 0")
                Stb.AppendLine("    AND aXapp.sa_cod = 0")
                Stb.AppendLine()
                Stb.AppendLine(") tab")
                Stb.AppendLine()

                If IDTestataTemp <> 0 Then
                    Stb.AppendLine("INNER JOIN __tmp_FiltroImpianti f ")
                    Stb.AppendLine("    ON tab.piva = f.piva ")
                    Stb.AppendLine("    AND tab.Sa_Cod = f.sa_cod ")
                    Stb.AppendLine("    AND tab.Appezza = f.appezza ")
                    Stb.AppendLine("    AND tab.Id_Reg = f. id_reg ")
                    Stb.AppendLine("    AND f.idTestataTemp = " & IDTestataTemp)
                    Stb.AppendLine()
                End If

                Stb.AppendLine("ORDER BY tab.DataOperazione, tab.PIVA, tab.sa_cod, tab.appezza, tab.id_reg")

            Else

                Stb.AppendLine(" Select  ")

                Stb.AppendLine("  --chiave  Impianto ")

                Stb.AppendLine("    reg.piva ")
                Stb.AppendLine("  , reg.sa_cod ")
                Stb.AppendLine("  , reg.appezza ")
                Stb.AppendLine("  , reg.id_reg ")

                Stb.AppendLine("  --chiave  Analisi ")
                Stb.AppendLine("     , aT.Analisi_Testata_Cod as chiaveAnalisi ")


                Stb.AppendLine("  --descrizione Impianto ")
                Stb.AppendLine("  , i.Rag_Soc as RagioneSociale ")
                Stb.AppendLine("  , sa.Sa_Nome ")
                Stb.AppendLine("  , veg.veg_des ")
                Stb.AppendLine("  , cul.Cul_des ")
                Stb.AppendLine("  , reg.Sup_Imp as sup_ha ")
                Stb.AppendLine("  , reg.Sup_Imp ")
                Stb.AppendLine("  , app.via_stringa ")
                Stb.AppendLine("  , app.App_Nome + ' - ' + aT.Analisi_Testata_DES  as Appezzamento_Descrizione ")

                Stb.AppendLine("  --descrizione operazione ")
                Stb.AppendLine("  , tt.Analisi_Tipologia_Des as Descrizione_Operazione ")

                Stb.AppendLine("    --descrizione rilievo ")
                Stb.AppendLine("  , aT.Analisi_Testata_Data_Fine AS DataOperazione ")
                Stb.AppendLine("  , ap.Analisi_Parametro_Cod as indGenericCod ")
                Stb.AppendLine("  , isNull(analisi_parametro_des , '')  as indGenericDes ")
                Stb.AppendLine("  , ap.[Analisi_Parametro_UdM] as udm_cod ")
                Stb.AppendLine("  , udm.UDM_DES ")
                Stb.AppendLine("  , isNull(d.Analisi_Dettaglio_Valore_1, 0) as qta ")
                Stb.AppendLine("   ")
                Stb.AppendLine(" , (SELECT TOP 1 CONVERT(VARCHAR, Movimenti.Data_Movimento, 103) ")
                Stb.AppendLine("     FROM    Agenda ")
                Stb.AppendLine("             INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                Stb.AppendLine(" 			 INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
                Stb.AppendLine(" 			 INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
                Stb.AppendLine(" 			 INNER JOIN Imprese_Progetti ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND Imprese_Progetti.sa_cod = Mov_Destinazioni.sa_cod AND Imprese_Progetti.appezza = Mov_Destinazioni.APPEZZA AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
                Stb.AppendLine("     WHERE   Agenda.Lav_Cod = 125 AND Movimenti.Cau_Mov = '2200' ")
                Stb.AppendLine(" 			 AND Mov_Destinazioni.Piva = reg.PIVA AND Mov_Destinazioni.sa_cod = reg.sa_cod AND Mov_Destinazioni.appezza = reg.APPEZZA AND Mov_Destinazioni.Id_Destinazione = reg.ID_REG ")
                Stb.AppendLine("             AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
                Stb.AppendLine("             AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")
                Stb.AppendLine("     ORDER BY Movimenti.Data_Movimento) AS Data_Raccolta ")

                Stb.AppendLine("  From Analisi_Testata aT ")
                Stb.AppendLine("       ")
                Stb.AppendLine("      inner Join [dbo].[Analisi_Tipologia] tt ")
                Stb.AppendLine("             On tt.analisi_tipologia_tipo = aT.Analisi_Testata_Tipo ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join Analisi_Dettagli d ")
                Stb.AppendLine("             On aT.Analisi_Testata_cod = d.Analisi_Testata_Cod ")
                Stb.AppendLine("          And aT.Analisi_SuperUser = d.Analisi_SuperUser ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join [dbo].[Analisi_Parametri] ap ")
                Stb.AppendLine("             On ap.[Analisi_Parametro_Cod] = d.Analisi_Parametro_Cod ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join Analisi_EntitaxTestata aXapp ")
                Stb.AppendLine("             On aT.Analisi_Testata_cod = aXapp.Analisi_Testata_Cod ")
                Stb.AppendLine("          And aT.Analisi_SuperUser = aXapp.Analisi_SuperUser ")
                Stb.AppendLine("       ")
                Stb.AppendLine("      inner Join Appezzamento app ")
                Stb.AppendLine("             On app.piva = aXapp.Piva ")
                Stb.AppendLine("          And app.sa_cod = aXapp.sa_cod ")
                Stb.AppendLine("          And app.appezza = aXapp.appezza ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join reg_impianti reg ")
                Stb.AppendLine("             On app.piva = reg.Piva ")
                Stb.AppendLine("          And app.sa_cod = reg.sa_cod ")
                Stb.AppendLine("          And app.appezza = reg.appezza ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join imprese i ")
                Stb.AppendLine("             On i.piva = reg.piva ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join Centri_Aziendali sa ")
                Stb.AppendLine("             On sa.piva = reg.piva  ")
                Stb.AppendLine("          And sa.sa_cod = reg.sa_Cod ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join Cultivar cul ")
                Stb.AppendLine("             On cul.Cul_Cod = reg.CUL_COD ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join specieVegetali veg ")
                Stb.AppendLine("             On veg.Veg_Cod = cul.Veg_Cod ")
                Stb.AppendLine("  ")
                Stb.AppendLine("      inner Join UnitaMisura udm ")
                Stb.AppendLine("             On udm.udm_cod = ap.[Analisi_Parametro_UdM] ")
                Stb.AppendLine("  ")


                If IDTestataTemp <> 0 Then
                    Stb.AppendLine("    inner Join __tmp_FiltroImpianti f ")
                    Stb.AppendLine("        On reg.piva = f.piva ")
                    Stb.AppendLine("        And reg.Sa_Cod = f.sa_cod ")
                    Stb.AppendLine("        And reg.Appezza = f.appezza ")
                    Stb.AppendLine("        And reg.Id_Reg = f. id_reg ")
                    Stb.AppendLine("        And f.idTestataTemp = " & IDTestataTemp)
                End If

                Stb.AppendLine(" ")
                Stb.AppendLine("  WHERE 1 = 1  ")
                Stb.AppendLine(" ")

                Stb.AppendLine("  And ( ( aT.Analisi_Testata_Data_Fine <=  " & Agro_SQL_SaveDate(DataA))
                Stb.AppendLine("  And aT.Analisi_Testata_Data_Fine >=  " & Agro_SQL_SaveDate(DataDa) & ") or aT.Analisi_Testata_Data_Fine = " & Agro_SQL_SaveDate(AGRODATAFINE) & " ) ")
                Stb.AppendLine("  order by aT.Analisi_Testata_Data_Fine, reg.PIVA, reg.sa_cod, reg.appezza, reg.id_reg")

            End If

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


    '##############################################################################################
    Public Function ElaborazioneDatiQuadernoDiCampagna(
            ByVal DataDa As DateTime,
            ByVal DataA As DateTime,
            ByVal lav_cod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal IDTestataTemp As Integer = 0
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            'Dim NomeDB_Utenti As String
            'NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            'NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Stb.AppendLine("SELECT ")
            Stb.AppendLine("--chiave  Impianto ")
            Stb.AppendLine("    op.PIVA ")
            Stb.AppendLine("    , op.sa_cod ")
            Stb.AppendLine("    , op.appezza ")
            Stb.AppendLine("    , op.id_reg ")
            Stb.AppendLine("--chiave  Analisi ")
            Stb.AppendLine("    , op.id_agenda AS chiaveAnalisi ")
            Stb.AppendLine("--descrizione Impianto ")
            Stb.AppendLine("    , ISNULL(i.rag_soc, '') AS RagioneSociale ")
            Stb.AppendLine("    , ISNULL(sa.sa_nome, '') AS Sa_Nome ")
            Stb.AppendLine("    , ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des ")
            Stb.AppendLine("    , ISNULL(Cultivar.Cul_Des, '') AS Cul_Des ")
            Stb.AppendLine("    , CAST(op.Sup_Imp AS integer) AS Sup_Ha ")
            Stb.AppendLine("    , op.Sup_Imp ")
            Stb.AppendLine("    , COALESCE(op.Via_Stringa, '') AS Appezzamento_Indirizzo ")
            Stb.AppendLine("    , op.APP_NOME AS Appezzamento_Descrizione  ")

            Stb.AppendLine("    , COALESCE(COD.kPIN, '') AS kPIN ")
            Stb.AppendLine("	, COALESCE(COD.BlockName, '') AS BlockName ")

            Stb.AppendLine("--descrizione operazione ")
            Stb.AppendLine("    , op.lav_des AS DescrizioneOperazione ")
            Stb.AppendLine("--descrizione rilievo ")
            Stb.AppendLine("    , op.Data_Movimento AS DataOperazione  ")
            Stb.AppendLine("    , mav.indGenericCod ")
            Stb.AppendLine("    , mav.indGenericDes ")
            Stb.AppendLine("    , mav.udm_cod ")
            Stb.AppendLine("    , mav.Udm_Des ")
            Stb.AppendLine("    , op.Qta ")
            Stb.AppendLine("--data raccolta ")
            Stb.AppendLine("    , (SELECT TOP 1 CONVERT(VARCHAR, Movimenti.Data_Movimento, 103) ")
            Stb.AppendLine("        FROM Agenda ")
            Stb.AppendLine("            INNER JOIN Movimenti            ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("            INNER JOIN Movimenti_dettagli   ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine("            INNER JOIN Mov_Destinazioni     ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            Stb.AppendLine("            INNER JOIN Imprese_Progetti     ON Imprese_Progetti.Piva = Mov_Destinazioni.PIVA AND Imprese_Progetti.sa_cod = Mov_Destinazioni.sa_cod AND Imprese_Progetti.appezza = Mov_Destinazioni.APPEZZA AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")
            Stb.AppendLine("        WHERE Agenda.Lav_Cod = 125 ")
            Stb.AppendLine("            AND Movimenti.Cau_Mov = '2200' ")
            Stb.AppendLine("            AND Mov_Destinazioni.Piva = op.PIVA ")
            Stb.AppendLine("            AND Mov_Destinazioni.sa_cod = op.sa_cod ")
            Stb.AppendLine("            AND Mov_Destinazioni.appezza = op.APPEZZA ")
            Stb.AppendLine("            AND Mov_Destinazioni.Id_Destinazione = op.ID_REG ")
            Stb.AppendLine("            AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
            Stb.AppendLine("            AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")
            Stb.AppendLine("        ORDER BY Movimenti.Data_Movimento) AS Data_Raccolta ")
            Stb.AppendLine("FROM ")
            Stb.AppendLine("( ")

            If lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then

                Stb.AppendLine("    SELECT ")
                Stb.AppendLine("        --mav.cod ")
                Stb.AppendLine("        id1.ind_mat_Cod AS indGenericCod ")
                Stb.AppendLine("        , id1.IND_MAT_DES AS indGenericDes ")
                Stb.AppendLine("        , UnitaMisura.UDM_COD ")
                Stb.AppendLine("        , UnitaMisura.UDM_DES ")
                Stb.AppendLine("        , unitaMisura.udm_sim ")
                Stb.AppendLine("        , 1 AS ordine ")
                Stb.AppendLine("    FROM MisuraxIndiciMaturita mXid ")
                Stb.AppendLine("        INNER JOIN indiciMaturita id1   ON mXid.IND_MAT_COD = id1.IND_MAT_COD ")
                Stb.AppendLine("        INNER JOIN UnitaMisura          ON mXid.Udm_Cod = UnitaMisura.Udm_Cod ")
                Stb.AppendLine()
                Stb.AppendLine("    UNION ") 'union per dati calcolati, es.: indice di ravaz
                Stb.AppendLine()
                Stb.AppendLine("    SELECT ")
                Stb.AppendLine("        -1 AS indGenericCod ")
                Stb.AppendLine("        , 'Indice di Ravaz' AS indGenericDes ")
                Stb.AppendLine("        , -1 AS udm_Cod ")
                Stb.AppendLine("        , '' AS Udm_des ")
                Stb.AppendLine("        , '' AS udm_sim ")
                Stb.AppendLine("        , -1 AS ordine")

            ElseIf lav_cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO Then

                Stb.AppendLine("    SELECT ")
                Stb.AppendLine("        avv.Av_Cod AS indGenericCod ")
                Stb.AppendLine("        , avv.Av_Des_Vol AS indGenericDes ")
                Stb.AppendLine("        , um.UDM_COD ")
                Stb.AppendLine("        , um.UDM_DES ")
                Stb.AppendLine("        , um.UDM_SIM ")
                Stb.AppendLine("    FROM MisuraxAvversita mxa ")
                Stb.AppendLine("        INNER JOIN Avversita avv	ON avv.Av_Cod = mxa.AV_COD ")
                Stb.AppendLine("        INNER JOIN UnitaMisura um	ON um.UDM_COD = mxa.UDM_COD")

            ElseIf lav_cod = LAVCOD_DANNI_RACCOLTA Then

                Stb.AppendLine("    SELECT ")
                Stb.AppendLine("        dr.DR_Cod AS indGenericCod ")
                Stb.AppendLine("        , dr.DR_DES AS indGenericDes ")
                Stb.AppendLine("        , um.UDM_COD ")
                Stb.AppendLine("        , um.UDM_DES ")
                Stb.AppendLine("        , um.UDM_SIM ")
                Stb.AppendLine("    FROM MisuraxDanniRaccolta mxdr ")
                Stb.AppendLine("        INNER JOIN DanniRaccolta dr	ON dr.DR_COD = mxdr.DR_COD ")
                Stb.AppendLine("        INNER JOIN UnitaMisura um	ON um.UDM_COD = mxdr.UDM_COD")

            End If

            Stb.AppendLine()
            Stb.AppendLine(") mav  ")
            Stb.AppendLine()
            Stb.AppendLine("INNER JOIN ( ")
            Stb.AppendLine("    SELECT -- o.LAV_DES as operazione, max(a.id_Agenda) as id_Agenda ")
            Stb.AppendLine("        a.piva ")
            Stb.AppendLine("        , a.Sa_Cod ")
            Stb.AppendLine("        , a.id_agenda ")
            Stb.AppendLine("        , a.lav_cod ")
            Stb.AppendLine("        , a.inviato ")
            Stb.AppendLine("        , Movimenti_dettagli.id_Mov_Det  ")
            Stb.AppendLine("        , Mov_Destinazioni.mov_destinazioni_graphickey ")
            Stb.AppendLine("        , Mov_Dettaglio_Tecnico.dett_Cod  ")
            Stb.AppendLine("        , Movimenti_dettagli.Udm_Cod ")
            Stb.AppendLine("        , Reg_Impianti.CUL_COD ")

            If lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then

                Stb.AppendLine("        , Mov_Dettaglio_Tecnico.FF_Classe ")

            ElseIf lav_cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO Then

                Stb.AppendLine("        , Mov_Dettaglio_Tecnico.Av_Cod ")

            ElseIf lav_cod = LAVCOD_DANNI_RACCOLTA Then

                Stb.AppendLine("        , Mov_Dettaglio_Tecnico.FF_Classe ")

            End If

            Stb.AppendLine("        , Movimenti.Data_Movimento ")
            Stb.AppendLine("        , Reg_Impianti.Sup_Imp ")
            Stb.AppendLine("        , Mov_Destinazioni.Qta ")
            Stb.AppendLine("        , Appezzamento.Via_Stringa ")
            Stb.AppendLine("        , Appezzamento.APP_NOME ")
            Stb.AppendLine("        , a.Username_Modifica ")
            Stb.AppendLine("        , mov_dettaglio_tecnico.Piezo1 ")
            Stb.AppendLine("        , mov_dettaglio_tecnico.Piezo2 as FF_Classe2 ")
            Stb.AppendLine("        , a.Des_Lib  ")
            Stb.AppendLine("        , Reg_Impianti.Appezza ")
            Stb.AppendLine("        , Reg_Impianti.id_reg ")
            Stb.AppendLine("        , Reg_Impianti.Validita_Inizio ")
            Stb.AppendLine("        , Reg_Impianti.Validita_Fine ")
            Stb.AppendLine("        , a.Validita_Inizio AS Agenda_Validita_Inizio ")
            Stb.AppendLine("        , a.Validita_Fine AS Agenda_Validita_Fine ")
            Stb.AppendLine("        , Movimenti.Mov_Desc AS NoteGenerali ")
            Stb.AppendLine("        , o.lav_des ")
            Stb.AppendLine("    FROM Agenda a ")
            Stb.AppendLine("        INNER JOIN Movimenti                ON a.Id_Agenda = Movimenti.Id_Agenda ")
            Stb.AppendLine("                                            AND a.PIVA = Movimenti.PIVA ")
            Stb.AppendLine("        INNER JOIN Movimenti_dettagli       ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine("                                            AND Movimenti.PIVA = Movimenti_dettagli.PIVA ")
            Stb.AppendLine("                                            AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            Stb.AppendLine("                                            AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
            Stb.AppendLine("        INNER JOIN Mov_Dettaglio_Tecnico    ON Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")
            Stb.AppendLine("        INNER JOIN Mov_Destinazioni         ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine("                                            AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            Stb.AppendLine("        INNER JOIN Appezzamento             ON Mov_Destinazioni.Piva = Appezzamento.PIVA ")
            Stb.AppendLine("                                            AND Mov_Destinazioni.Sa_Cod = Appezzamento.SA_COD ")
            Stb.AppendLine("                                            AND Mov_Destinazioni.Appezza = Appezzamento.APPEZZA ")
            Stb.AppendLine("        INNER JOIN Reg_Impianti             ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA ")
            Stb.AppendLine("                                            AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD ")
            Stb.AppendLine("                                            AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG ")
            Stb.AppendLine("                                            AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA ")
            Stb.AppendLine("        INNER JOIN operazioni o             ON a.lav_cod = o.lav_cod ")
            Stb.AppendLine("    WHERE o.lav_Cod = " & lav_cod)

            ' VAnni: 24/4/2018: capire un attimo meglio questo filtro...
            'Stb.AppendLine("              And Mov_Dettaglio_Tecnico.FF_Classe Not in (32, 33) ")

            If lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then

                Stb.AppendLine()
                Stb.AppendLine("    UNION --per calcolo indice ravaz ")
                Stb.AppendLine()
                Stb.AppendLine("    SELECT --o.LAV_DES As operazione, max(a.id_Agenda) AS id_Agenda ")
                Stb.AppendLine("         a.piva ")
                Stb.AppendLine("        , a.Sa_Cod ")
                Stb.AppendLine("        , a.id_agenda ")
                Stb.AppendLine("        , a.lav_cod ")
                Stb.AppendLine("        , a.inviato ")
                Stb.AppendLine("        , dPota.id_mov_det ")
                Stb.AppendLine("        , destProd.mov_destinazioni_graphickey ")
                Stb.AppendLine("        , -1 AS dett_Cod  --indice di ravaz è calcolato ")
                Stb.AppendLine("        , -1 AS Udm_Cod ")
                Stb.AppendLine("        , Reg_Impianti.CUL_COD ")
                Stb.AppendLine("        , -1 AS FF_Classe ")
                Stb.AppendLine("        , Movimenti.Data_Movimento ")
                Stb.AppendLine("        , Reg_Impianti.Sup_Imp ")
                Stb.AppendLine("        , CASE WHEN destPota.Qta = 0 THEN -1 ELSE destProd.qta / destPota.Qta END AS Qta ")
                Stb.AppendLine("        , Appezzamento.Via_Stringa ")
                Stb.AppendLine("        , Appezzamento.APP_NOME ")
                Stb.AppendLine("        , a.Username_Modifica ")
                Stb.AppendLine("        , tecProd.Piezo1 ")
                Stb.AppendLine("        , tecProd.Piezo2 AS FF_Classe2 ")
                Stb.AppendLine("        , a.Des_Lib ")
                Stb.AppendLine("        , Reg_Impianti.Appezza ")
                Stb.AppendLine("        , Reg_Impianti.id_reg ")
                Stb.AppendLine("        , Reg_Impianti.Validita_Inizio ")
                Stb.AppendLine("        , Reg_Impianti.Validita_Fine ")
                Stb.AppendLine("        , a.Validita_Inizio AS Agenda_Validita_Inizio ")
                Stb.AppendLine("        , a.Validita_Fine AS Agenda_Validita_Fine ")
                Stb.AppendLine("        , Movimenti.Mov_Desc AS NoteGenerali ")
                Stb.AppendLine("        , o.lav_des ")
                Stb.AppendLine("    FROM Agenda a ")
                Stb.AppendLine("        INNER JOIN Movimenti                        ON a.Id_Agenda = Movimenti.Id_Agenda ")
                Stb.AppendLine("                                                    AND a.PIVA = Movimenti.PIVA ")
                Stb.AppendLine("        INNER JOIN Movimenti_dettagli dPota         ON Movimenti.Id_Mov = dPota.Id_Mov ")
                Stb.AppendLine("                                                    AND Movimenti.PIVA = dPota.PIVA ")
                Stb.AppendLine("                                                    AND Movimenti.Sa_Cod = dPota.Sa_Cod ")
                Stb.AppendLine("                                                    AND Movimenti.Id_Agenda = dPota.Id_Agenda ")
                Stb.AppendLine("        INNER JOIN Movimenti_dettagli dProd         ON Movimenti.Id_Mov = dProd.Id_Mov ")
                Stb.AppendLine("                                                    AND Movimenti.PIVA = dProd.PIVA ")
                Stb.AppendLine("                                                    AND Movimenti.Sa_Cod = dProd.Sa_Cod ")
                Stb.AppendLine("                                                    AND Movimenti.Id_Agenda = dProd.Id_Agenda ")
                Stb.AppendLine("        INNER JOIN Mov_Dettaglio_Tecnico tecPota    ON dPota.Id_Mov = tecPota.Id_Mov ")
                Stb.AppendLine("                                                    AND dPota.PIVA = tecPota.Piva ")
                Stb.AppendLine("                                                    AND dPota.Sa_Cod = tecPota.Sa_Cod ")
                Stb.AppendLine("                                                    AND dPota.Id_Agenda = tecPota.Id_Agenda ")
                Stb.AppendLine("                                                    AND dPota.Id_Mov_Det = tecPota.Id_Mov_Det ")
                Stb.AppendLine("                                                    AND tecPota.ff_classe = 33 ")
                Stb.AppendLine("        INNER JOIN Mov_Dettaglio_Tecnico tecProd    ON dProd.Id_Mov = tecProd.Id_Mov ")
                Stb.AppendLine("                                                    AND dProd.PIVA = tecProd.Piva ")
                Stb.AppendLine("                                                    AND dProd.Sa_Cod = tecProd.Sa_Cod ")
                Stb.AppendLine("                                                    AND dProd.Id_Agenda = tecProd.Id_Agenda ")
                Stb.AppendLine("                                                    AND dProd.Id_Mov_Det = tecProd.Id_Mov_Det ")
                Stb.AppendLine("                                                    AND tecProd.ff_classe = 32 ")
                Stb.AppendLine("        INNER JOIN Mov_Destinazioni destPota        ON tecPota.PIVA = destPota.Piva ")
                Stb.AppendLine("                                                    AND tecPota.Sa_Cod = destPota.Sa_Cod ")
                Stb.AppendLine("                                                    AND tecPota.Id_Agenda = destPota.Id_Agenda ")
                Stb.AppendLine("                                                    AND tecPota.Id_Mov = destPota.Id_Mov ")
                Stb.AppendLine("                                                    AND tecPota.Id_Mov_Det = destPota.Id_Mov_Det ")
                Stb.AppendLine("        INNER JOIN Mov_Destinazioni destProd        ON tecProd.PIVA = destProd.Piva ")
                Stb.AppendLine("                                                    AND tecProd.Sa_Cod = destProd.Sa_Cod ")
                Stb.AppendLine("                                                    AND tecProd.Id_Agenda = destProd.Id_Agenda ")
                Stb.AppendLine("                                                    AND tecProd.Id_Mov = destProd.Id_Mov ")
                Stb.AppendLine("                                                    AND tecProd.Id_Mov_Det = destProd.Id_Mov_Det ")
                Stb.AppendLine("        INNER JOIN Appezzamento                     ON destProd.Piva = Appezzamento.PIVA ")
                Stb.AppendLine("                                                    AND destProd.Sa_Cod = Appezzamento.SA_COD ")
                Stb.AppendLine("                                                    AND destProd.Appezza = Appezzamento.APPEZZA ")
                Stb.AppendLine("        INNER JOIN Reg_Impianti                     ON destProd.Piva = Reg_Impianti.PIVA ")
                Stb.AppendLine("                                                    AND destProd.Sa_Cod = Reg_Impianti.SA_COD ")
                Stb.AppendLine("                                                    AND destProd.Id_Destinazione = Reg_Impianti.ID_REG ")
                Stb.AppendLine("                                                    AND destProd.Appezza = Reg_Impianti.APPEZZA ")
                Stb.AppendLine("        INNER JOIN operazioni o                     ON a.lav_cod = o.lav_cod ")
                Stb.AppendLine("    WHERE o.lav_Cod = " & lav_cod)

            End If

            Stb.AppendLine()
            Stb.AppendLine(") op ")
            Stb.AppendLine()

            If lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then

                Stb.AppendLine("ON op.ff_Classe = mav.indGenericCod AND op.dett_cod = mav.udm_cod ")

            ElseIf lav_cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO Then

                Stb.AppendLine("ON op.av_cod = mav.indGenericCod AND op.dett_cod = mav.udm_cod ")

            ElseIf lav_cod = LAVCOD_DANNI_RACCOLTA Then

                Stb.AppendLine("ON op.ff_Classe = mav.indGenericCod AND op.dett_cod = mav.udm_cod ")

            End If

            Stb.AppendLine()

            Stb.AppendLine("LEFT JOIN ( ")
            Stb.AppendLine()
            Stb.AppendLine("	SELECT TT.*, ipro.Validita_Inizio, ipro.Validita_Fine FROM ( ")
            Stb.AppendLine("		SELECT progetto_cod, piva, sa_cod, appezza, id_reg, [1287] AS kPIN, [1288] AS BlockName FROM ( ")
            Stb.AppendLine("			SELECT Progetto_Cod, PIVA, sa_cod, appezza, Id_Reg, id_cod, val_cod ")
            Stb.AppendLine("			FROM Reg_Impianti_Codici ")
            Stb.AppendLine("			WHERE (id_cod = 1287 --kPIN ")
            Stb.AppendLine("				OR id_cod = 1288) -- Block Name ")
            Stb.AppendLine("		) AS X PIVOT (MAX(val_cod) FOR id_cod IN ([1287], [1288])) TTT ")
            Stb.AppendLine("	) TT ")
            Stb.AppendLine("	INNER JOIN Imprese_Progetti ipro ON ipro.Progetto_Cod = TT.Progetto_Cod ")
            Stb.AppendLine()
            Stb.AppendLine(") COD ON COD.PIVA = op.PIVA	AND COD.sa_cod = op.Sa_Cod AND COD.appezza = op.APPEZZA AND COD.id_reg = op.ID_REG ")
            Stb.AppendLine("	AND COD.Validita_Inizio <= op.Data_Movimento AND op.Data_Movimento <= COD.Validita_Fine ")

            Stb.AppendLine()

            If IDTestataTemp <> 0 Then

                Stb.AppendLine("INNER JOIN __tmp_FiltroImpianti f   ON op.piva = f.piva ")
                Stb.AppendLine("                                    AND op.Sa_Cod = f.sa_cod ")
                Stb.AppendLine("                                    AND op.Appezza = f.appezza ")
                Stb.AppendLine("                                    AND op.Id_Reg = f. id_reg ")
                Stb.AppendLine("                                    AND f.idTestataTemp = " & IDTestataTemp)
                Stb.AppendLine()

            End If

            'Stb.AppendLine("   Left Join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli  ")
            'Stb.AppendLine("   On Dettagli.CodFisc = op.Username_Modifica  ")
            Stb.AppendLine("LEFT JOIN Cultivar              ON Cultivar.Cul_Cod = op.CUL_COD ")
            Stb.AppendLine("LEFT JOIN SpecieVegetali        ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            Stb.AppendLine("LEFT JOIN Imprese i             ON i.PIVA = op.PIVA ")
            Stb.AppendLine("LEFT JOIN Centri_Aziendali sa   ON sa.PIVA = op.PIVA AND sa.sa_cod = op.sa_cod ")

            If lav_cod = LAVCOD_RILIEVO_INDICI_MATURITA Then

                Stb.AppendLine("  LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe  ")
                Stb.AppendLine("  LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2  ")

            End If

            Stb.AppendLine()
            Stb.AppendLine("WHERE 1 = 1 ")
            Stb.AppendLine("    AND op.Data_Movimento <= " & Agro_SQL_SaveDate(DataA))
            Stb.AppendLine("    AND op.Data_Movimento >= " & Agro_SQL_SaveDate(DataDa))
            Stb.AppendLine("    AND op.Inviato >= 0 ")
            Stb.AppendLine()
            Stb.AppendLine("ORDER BY op.data_movimento, op.PIVA, op.sa_cod, op.appezza, op.id_reg")

            If lav_cod = LAVCOD_RILIEVO_AVVERSITA_CAMPO Then
                Stb.AppendLine(", chiaveAnalisi, indGenericCod, Udm_Cod")
            End If

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

