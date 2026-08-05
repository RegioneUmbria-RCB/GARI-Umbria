
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class SiscoQDC_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiPerEsportazione(
        ByVal PredisponiKendoGrid As Boolean,
        ByVal Piva As String,
        ByVal RichiediPraticaValida As Boolean,
        ByVal DataInizio As String,
        ByVal DataFine As String,
        ByVal CodificaAgeaDaGias_livelloDettaglio As List(Of enum_CodificaAgeaDaGias_livelloDettaglio),
        ByVal SostiuisciSpecieSpecificaConSpecieGenerica As Boolean,
        ByVal IgnoraCodiceUtilizzo As Boolean,
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

        Dim leggiCodificheAgea As New AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R
        Dim leggiAppezzamentoTerr As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Try

            Stb.Length = 0

            Stb.AppendLine("  ")


            Stb.AppendLine("   Select ")
            Stb.AppendLine("        DENSE_RANK() over (order by  ")
            Stb.AppendLine("          id_agenda ")
            Stb.AppendLine("        , COD_PRODOTTO ")
            Stb.AppendLine("        , COD_USO_VARIETA ")
            Stb.AppendLine("        , data_trattamento ")
            Stb.AppendLine("        , N_REGFO ")
            Stb.AppendLine("        , COD_PROVINCIA ")
            Stb.AppendLine("        , COD_COMUNE ")
            Stb.AppendLine("    ) as kendoKey ")
            Stb.AppendLine("    ")

            If Debugger.IsAttached Then
                Stb.AppendLine("  , Id_Agenda  ")
            End If


            If PredisponiKendoGrid Then


                'Altri campi griglia
                Stb.AppendLine("  --, FattoreProporzione  ")
                Stb.AppendLine("    ")
                Stb.AppendLine("  --descrizioni  ")
                Stb.AppendLine("  --, '' as DES_PRODOTTO  ")
                Stb.AppendLine("  --, '' as DES_USO_VARIETA  ")
                Stb.AppendLine("  --, ISTAT.COMUNI_PROV as DES_PROVINCIA  ")
                Stb.AppendLine("  --, ISTAT.LOCALITA  as DES_COMUNE ")
                Stb.AppendLine("  --, f.Fr_Des as FORMULATO  ")


                Stb.AppendLine("  , COD_PRODOTTO  ")
                Stb.AppendLine("  , COD_USO_VARIETA  ")
                Stb.AppendLine("  , COD_PROVINCIA  ")
                Stb.AppendLine("  , COD_COMUNE  ")
                Stb.AppendLine("  , DATA_TRATTAMENTO  ")
                Stb.AppendLine("  , ROUND(SUM(Superficie_Trattata), 4) as SUPERFICIE_TRATTATA ")
                Stb.AppendLine("  , N_REGFO ")
                Stb.AppendLine("  , case when Udm_Cod = 29 then 'L' else 'G' end As UNITA_DI_MISURA ")
                Stb.AppendLine("  , case when Udm_Cod = 29 then 1 else 1000 end * ROUND(SUM(Quantita), 4) as Quantita ")

            Else

                'CAMPI ESPORTAZIONE, Rispettare nomenclature del tracciato

                Stb.AppendLine("  , COD_PRODOTTO  ")
                Stb.AppendLine("  , COD_USO_VARIETA  ")
                Stb.AppendLine("  , COD_PROVINCIA  ")
                Stb.AppendLine("  , COD_COMUNE  ")
                Stb.AppendLine("  , DATA_TRATTAMENTO  ")
                Stb.AppendLine("  , ROUND(SUM(Superficie_Trattata), 4) as SUPERFICIE_TRATTATA ")
                Stb.AppendLine("  , N_REGFO ")
                Stb.AppendLine("  , case when Udm_Cod = 29 then 'L' else 'G' end As UNITA_DI_MISURA ")
                Stb.AppendLine("  , case when Udm_Cod = 29 then 1 else 1000 end * ROUND(SUM(Quantita), 4) as Quantita ")
            End If


            Stb.AppendLine("  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  from(  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("      Select ")
            Stb.AppendLine("       A.Id_Agenda ")
            Stb.AppendLine("     , Mov.Data_Movimento as Data_Trattamento  ")
            Stb.AppendLine("     , Det.Pro_Cod as N_REGFO  ")
            Stb.AppendLine("     , app.PROV as COD_PROVINCIA  ")
            Stb.AppendLine("     , app.COM as COD_COMUNE ")


            Stb.AppendLine("     , isnull( case when pl.Specie_Uso is not null and len(pl.Specie_Uso) = 7  then substring(pl.Specie_Uso, 1, 3) else  veg_Cod_agea end, '')  as COD_PRODOTTO ")

            If IgnoraCodiceUtilizzo Then
                Stb.AppendLine("  , '000' as COD_USO_VARIETA ")
            Else
                Stb.AppendLine("  ,  isnull( case when pl.Specie_Uso is not null and len(pl.Specie_Uso) = 7 then substring(pl.Specie_Uso, 5, 3) else  A2015.Uso_cod end, '') as COD_USO_VARIETA ")
            End If


            'Stb.AppendLine("  , A2015.Cul_Cod_Agea as COD_USO_VARIETA ")
            Stb.AppendLine("     --, Dest.Qta2 / imp.Sup_Imp as FattoreProporzione  ")
            Stb.AppendLine("     --, case when app.AREA = 0 then imp.Sup_Imp else app.AREA end as SuperficiePerCalcolo  ")

            Stb.AppendLine("   , case when app.AREA = 0 then imp.Sup_Imp else app.AREA end * Dest.FattoreProporzione as Superficie_Trattata  ")
            Stb.AppendLine("   , case when app.AREA = 0 then imp.Sup_Imp else app.AREA end * Dest.FattoreProporzione * Det.Qta as Quantita ")

            Stb.AppendLine("  , Det.Udm_Cod ")
            Stb.AppendLine("     --, Dest.Qta as Quantita ")
            Stb.AppendLine("     --, imp.SA_COD  ")
            Stb.AppendLine("     --, imp.APPEZZA  ")
            Stb.AppendLine("     --, imp.ID_REG    ")
            Stb.AppendLine("  --, Dest.Id_Mov_Det ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  From(  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("     Select ")
            Stb.AppendLine("           d.piva ")
            Stb.AppendLine("      , d.sa_cod ")
            Stb.AppendLine("      , d.appezza ")
            Stb.AppendLine("      , d.Id_Destinazione as id_reg ")
            Stb.AppendLine("      , d.Id_Agenda ")
            Stb.AppendLine("      , d.Id_Mov ")
            Stb.AppendLine("      , d.Id_Mov_Det ")
            Stb.AppendLine("      , Qta2 / imp.Sup_Imp as FattoreProporzione --per riproporzionare le superfici in base alla sup trattata ")
            Stb.AppendLine("      From Mov_Destinazioni d ")
            Stb.AppendLine("          inner Join Reg_Impianti imp ")
            Stb.AppendLine("              On d.PIVA = imp.PIVA  ")
            Stb.AppendLine("              And d.SA_COD = imp.SA_COD  ")
            Stb.AppendLine("              And d.APPEZZA = imp.APPEZZA ")
            Stb.AppendLine("              And d.Id_Destinazione = imp.id_reg ")
            Stb.AppendLine("      where d.Tipo_Destinazione = 0 ")
            Stb.AppendLine("      And imp.Sup_Imp <> 0 ")
            Stb.AppendLine("  ) Dest  ")

            Stb.AppendLine("   --lettura codifiche agea per impianto ")
            Stb.AppendLine("   left Join ( ")

            leggiCodificheAgea.LeggiCodificheSuImpiantiOppurePlanning(
                stb:=Stb,
                piva:="",
                FiltroImpiantiConAND:="",
                Veg_Cod:=0,
                Cul_Cod:=0,
                Grfi_Cod:=0,
                Grva_Cod:=0,
                Metodo_Produzione_Cod:=0,
                Reg_Cod:=0,
                Id_Cod:=0,
                Grsp_Cod:=0,
                Cul_Cod_Agea:="",
                Uso_Cod_Agea:="",
                Occupazione_Cod_Agea:="",
                Destinazione_Cod_Agea:="",
                CodificaAgeaDaGias_livelloDettaglio:=CodificaAgeaDaGias_livelloDettaglio,
                SostiuisciSpecieSpecificaConSpecieGenerica:=SostiuisciSpecieSpecificaConSpecieGenerica,
                Qualita_Cod_Agea:="",
                xFiltroAggiuntivo:=" Veg_Cod_Agea <> '000' ",
                xOrderBy:="",
                objParametri:=objParametri
            )

            Stb.AppendLine(" ) A2015 ")


            Stb.AppendLine("  On dest.PIVA = A2015.PIVA      ")
            Stb.AppendLine("     And dest.Sa_Cod = A2015.Sa_Cod      ")
            Stb.AppendLine("  And dest.Appezza = A2015.APPEZZA ")
            Stb.AppendLine("  And dest.id_reg = A2015.ID_REG      ")
            Stb.AppendLine("  ")
            Stb.AppendLine("   --fine lettura codifiche agea per impianto ")
            Stb.AppendLine("  ")

            Stb.AppendLine(" left join  ( ")

            Stb.AppendLine("   select iprog.piva, iprog.Sa_Cod, iprog.Appezza, iprog.Id_Reg, min(e.Veg_Cod_Agea + '-' + e.Uso_Cod_Agea) as Specie_Uso ")
            Stb.AppendLine("         from Reg_Impianti_Programmazioni iProg ")
            Stb.AppendLine("         inner join Programmazione_Entita e ")
            Stb.AppendLine("         on iProg.Programmazione_Cod = e.Programmazione_Cod ")
            Stb.AppendLine("         and iProg.Programmazione_Entita_Cod = e.Programmazione_Entita_Cod ")
            Stb.AppendLine("         where iprog.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.AppendLine("             group by iprog.piva, iprog.Sa_Cod, iprog.Appezza, iprog.Id_Reg")
            Stb.AppendLine(" ) PL ")

            Stb.AppendLine("  On dest.PIVA = pl.PIVA      ")
            Stb.AppendLine("     And dest.Sa_Cod = pl.Sa_Cod      ")
            Stb.AppendLine("  And dest.Appezza = pl.APPEZZA ")
            Stb.AppendLine("  And dest.id_reg = pl.ID_REG      ")
            Stb.AppendLine("  ")
            Stb.AppendLine("   --fine lettura codifiche agea memorizzate su planning ")
            Stb.AppendLine("  ")

            Stb.AppendLine("   INNER Join Movimenti_dettagli Det  ")
            Stb.AppendLine("            On dest.PIVA = det.PIVA      ")
            Stb.AppendLine("            And dest.Sa_Cod = det.Sa_Cod      ")
            Stb.AppendLine("            And dest.Id_Agenda = det.Id_Agenda     ")
            Stb.AppendLine("         And Dest.Id_Mov = det.Id_Mov   ")
            Stb.AppendLine("         And Dest.Id_Mov_Det = det.Id_Mov_Det  ")
            Stb.AppendLine("        ")
            Stb.AppendLine("    INNER Join Movimenti Mov      ")
            Stb.AppendLine("            On dest.PIVA = Mov.PIVA      ")
            Stb.AppendLine("            And dest.Sa_Cod = Mov.Sa_Cod      ")
            Stb.AppendLine("            And dest.Id_Agenda = Mov.Id_Agenda     ")
            Stb.AppendLine("         And Dest.Id_Mov = Mov.Id_Mov   ")
            Stb.AppendLine("      ")
            Stb.AppendLine("    inner Join Reg_Impianti imp  ")
            Stb.AppendLine("           On imp.PIVA = Dest.PIVA      ")
            Stb.AppendLine("           And imp.Sa_Cod = Dest.Sa_Cod      ")
            Stb.AppendLine("           And imp.APPEZZA  = Dest.Appezza     ")
            Stb.AppendLine("           And imp.ID_REG = Dest.id_reg   ")
            Stb.AppendLine(" ")

            Stb.AppendLine("  -- superfici lette da 1. Riparto catasto, 2. Indirizzo Appezzamento, 3. Indirizzo Centro ")
            Stb.AppendLine("  inner Join( ")
            Stb.AppendLine("  ")

            leggiAppezzamentoTerr.LeggiPerTerritorio(Stb, "", 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            Stb.AppendLine(" ) app ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" On app.Piva = imp.PIVA  ")
            Stb.AppendLine(" And app.sa_cod = imp.SA_COD ")
            Stb.AppendLine(" And app.Appezza = imp.APPEZZA ")


            Stb.AppendLine("   ")
            Stb.AppendLine("     inner Join Agenda A ")
            Stb.AppendLine("         On A.Id_Agenda = Mov.Id_Agenda ")
            Stb.AppendLine("      And A.PIVA = Mov.PIVA ")

            Stb.AppendLine("    WHERE   Mov.Data_Movimento <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            Stb.AppendLine("     AND     Mov.Data_Movimento >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")

            Stb.AppendLine("     AND   A.Lav_Cod IN ( " &
                            CStr(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO) & ", " &
                            CStr(LAVCOD_TRATTAMENTO_FITOREGOLATORE) & ", " &
                            CStr(LAVCOD_DISERBO) & ", " &
                            CStr(LAVCOD_GEODISINFESTAZIONE) & ", " &
                            CStr(LAVCOD_CONCIA_SEME) & ", " &
                            CStr(LAVCOD_DISSECCAMENTO) &
                        " )  ")

            Stb.AppendLine("   And   A.PIVA = '" & Agro_SQL_SaveText(Piva) & "'  ")
            Stb.AppendLine("   ")
            Stb.AppendLine("      And   A.Inviato >=0  ")
            Stb.AppendLine("   And Det.Elem_Cod = 191 ")
            Stb.AppendLine("   And Det.Pro_Cod < 100000 --escludo coadiuvanti, coformulati ecc.. ")

            'solo province lombarde
            Stb.AppendLine("  And app.PROV  in ( '016','013','020','012','098','015','014','017','019','002','018','097') ")

            Stb.AppendLine(" ) opExport ")

            Stb.AppendLine("  inner Join ISTAT  ")
            Stb.AppendLine("      On ISTAT.PROV = opExport.COD_PROVINCIA ")
            Stb.AppendLine("      And ISTAT.COM = opExport.COD_COMUNE ")
            Stb.AppendLine(" ")

            Stb.AppendLine("  inner Join Formulati f ")
            Stb.AppendLine("     On f.Fr_Cod = opExport.N_REGFO ")
            Stb.AppendLine(" ")

            Stb.AppendLine(" group by  ")
            Stb.AppendLine("    Id_Agenda ")
            Stb.AppendLine("  , COD_PRODOTTO  ")
            Stb.AppendLine("  , COD_USO_VARIETA  ")
            Stb.AppendLine("  , Data_Trattamento  ")
            Stb.AppendLine("  , N_REGFO  ")
            Stb.AppendLine("  , COD_PROVINCIA  ")
            Stb.AppendLine("  , COD_COMUNE ")
            Stb.AppendLine("  , Udm_Cod")



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


'#################################################################
'#################################################################
'#################################################################

Public Class SiscoQdc_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT ... " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine, ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")



            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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
