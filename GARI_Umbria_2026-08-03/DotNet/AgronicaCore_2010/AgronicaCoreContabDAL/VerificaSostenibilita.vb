Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports System.Text
Imports AgronicaCoreDataProvider

Public Class VerificaSostenibilita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Private NumeroIndefinito As String = " -999999 "

    '##############################################################################################
    Public Function Leggi(
        ByVal SostenibilitaTipoAnalisi As enum_SostenibilitaTipoAnalisi,
        ByVal principiAttivi As String,
        ByVal includiFertilizzazioni As Boolean,
        ByVal idTestataTemp As Integer,
        ByVal piva As String,
        ByVal Veg_Cod As String,
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal chiamataDaOperazioneAgenda As Boolean,
        ByVal OperazioneCorrente_id_Agenda_Escludi As Integer,
        ByVal OperazioneCorrente_InsertStmt As String,
        ByVal OperazioneCorrente_Lav_Cod As Integer,
        ByVal ListaAttivita_Lav_Cod As List(Of Integer),
        ByVal OperazioneCorrente_Fr_Cod As String,
        ByVal OperazioneCorrente_PrincipiAttivi As String,
        ByVal OperazioneCorrente_PrincipiAttiviPesi As String,
        ByVal OperazioneCorrente_FiltroImpianti As String,
        ByVal OperazioneCorrente_FiltroProdotti As String,
        ByVal OperazioneCorrente_FiltroOperazioni As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New StringBuilder
        Dim DT As DataTable

        'Dim chiamataDaOperazioneAgenda As Boolean = (OperazioneCorrente_InsertStmt <> "")

        Dim OperazioneCorrenteFertilizzazione As Boolean = {14, 156, 124, 26, 123, 106}.Contains(OperazioneCorrente_Lav_Cod)
        Dim OperazioneCorrenteTrattamento As Boolean = {74, 103, 18, 155, 13, 158}.Contains(OperazioneCorrente_Lav_Cod)
        Dim isMultiAttivita As Boolean = False

        'Scorro tutti i lav_cod della lista attività
        For Each lav_cod In ListaAttivita_Lav_Cod
            If OperazioneCorrenteFertilizzazione = False Then
                OperazioneCorrenteFertilizzazione = {14, 156, 124, 26, 123, 106}.Contains(lav_cod)
            End If
            If OperazioneCorrenteTrattamento = False Then
                OperazioneCorrenteTrattamento = {74, 103, 18, 155, 13, 158}.Contains(lav_cod)
            End If
        Next

        If ListaAttivita_Lav_Cod.Count > 1 Then
            isMultiAttivita = True
        End If

        Try

            Stb.Length = 0

            Const aliasDescrizioneOperazione As String = " AS Data"
            Const aliasOperazione As String = " AS Operazione "



            'la connessione è globale, deve permettere di popolare le tabelle temporanee.
            objParametri.objConnessione = ApriConnessione(objParametri, MessaggioErrore)

            Dim filtroImpiantiAttivo As Boolean = Not (OperazioneCorrente_FiltroImpianti = "")

            'verifico se è diverso da stringa vuota, in questo caso esegue query
            If OperazioneCorrente_InsertStmt <> "" Then
                Stb.AppendLine(OperazioneCorrente_InsertStmt)
            End If

            ' VAnni: 1/2/2019: anticipo creazione tabella filtro impianti
            If OperazioneCorrente_FiltroImpianti <> "" Then
                leggiPopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri)
            End If

            Dim filtroAgendaAttivo As Boolean = Not (OperazioneCorrente_FiltroOperazioni = "")

            If OperazioneCorrente_FiltroOperazioni <> "" Then
                leggiPopolaTabellaFiltroAgenda(OperazioneCorrente_FiltroOperazioni, objParametri)
            End If

            ' VAnni: 15/1/2020: se esiste qualcosa da scrivere si trova qui: in tal caso procedo con scrittura!
            If Stb.Length > 0 Then
                Dim idle As Boolean = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            End If


            Dim DT_FormulatiPA As DataTable


            'solo se non ho un'operazione corrente, viceversa i PA saranno letti da lì.
            'If OperazioneCorrente_PrincipiAttivi = "" Then
            Dim FiltroAgenda = False
            If OperazioneCorrente_FiltroOperazioni <> "" Then
                FiltroAgenda = True
            End If
            DT_FormulatiPA = leggiPopolaTabellaFormulatiPA_LeggiDT(SostenibilitaTipoAnalisi,
                                                                   idTestataTemp,
                                                                   piva,
                                                                   OperazioneCorrente_FiltroImpianti,
                                                                   OperazioneCorrente_FiltroOperazioni,
                                                                   Veg_Cod,
                                                                   DataDa,
                                                                   DataA,
                                                                   objParametri,
                                                                   FiltroAgenda)
            'End If

            leggiPopolaTabellaFormulatiPA(principiAttivi, idTestataTemp, OperazioneCorrente_Fr_Cod, OperazioneCorrente_PrincipiAttivi, OperazioneCorrente_PrincipiAttiviPesi, DT_FormulatiPA, objParametri)

            If includiFertilizzazioni Then

                Stb.Length = 0

                ''il nocount è importate per avere solo una tabella in output
                'Stb.AppendLine("SET NOCOUNT ON ")

                Stb.AppendLine(" INSERT __Agenda_Rpt_Conc ( ")

                Stb.AppendLine("  [IDTestataTemp],  ")
                Stb.AppendLine("  [Fr_Des],  ")

                Stb.AppendLine("  [QtaTotImpianto_N],  ")
                Stb.AppendLine("  [QtaTotImpianto_P],  ")
                Stb.AppendLine("  [QtaTotImpianto_K],  ")
                Stb.AppendLine("  [QtaTotImpianto_Cu],  ")

                Stb.AppendLine("  MassimaleImpianto_N, ")
                Stb.AppendLine("  MassimaleImpianto_P, ")
                Stb.AppendLine("  MassimaleImpianto_K, ")
                Stb.AppendLine("  MassimaleImpianto_Cu, ")

                Stb.AppendLine("  [SupTrattataImpiantoMedia],  ")
                Stb.AppendLine("  appezzamento, ")
                Stb.AppendLine("  specie, ")
                Stb.AppendLine("  varieta, ")
                Stb.AppendLine("  localita, ")
                Stb.AppendLine("  provincia, ")

                Stb.AppendLine("  DescrizioneOperazione,")
                Stb.AppendLine("  Operazione,")
                Stb.AppendLine("  OpCorrente")


                Stb.AppendLine(" ) ")
                Stb.AppendLine(" SELECT ")

                Stb.AppendLine(" 	   " & idTestataTemp & " AS idTestataTemp ")
                Stb.AppendLine(" 	 , ff.fer_Des ")

                Stb.AppendLine(" 	, QtaProdotto.QtaTotImpianto_N ")
                Stb.AppendLine(" 	, QtaProdotto.QtaTotImpianto_P ")
                Stb.AppendLine(" 	, QtaProdotto.QtaTotImpianto_K ")
                Stb.AppendLine(" 	, QtaProdotto.QtaTotImpianto_Cu ")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine(" 	, ISNULL(CAST(REPLACE(Massimale_N.val_cod, ',', '.') AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_N --KG o Lt. ")
                    Stb.AppendLine(" 	, ISNULL(CAST(REPLACE(Massimale_P.val_cod, ',', '.') AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_P --KG o Lt. ")
                    Stb.AppendLine(" 	, ISNULL(CAST(REPLACE(Massimale_K.val_cod, ',', '.') AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_K --KG o Lt. ")
                    Stb.AppendLine(" 	, QtaProdotto.MassimaleImpianto_Cu --KG o Lt. ")
                Else
                    Stb.AppendLine(" 	, " & NumeroIndefinito & " AS MassimaleImpianto_N --KG o Lt. ")
                    Stb.AppendLine(" 	, " & NumeroIndefinito & " AS MassimaleImpianto_P --KG o Lt. ")
                    Stb.AppendLine(" 	, " & NumeroIndefinito & " AS MassimaleImpianto_K --KG o Lt. ")
                    Stb.AppendLine(" 	, " & NumeroIndefinito & " AS MassimaleImpianto_CU --KG o Lt. ")
                End If



                Stb.AppendLine(" 	, SupMedia.SupTrattataImpianto ")

                appezzaDescrizioniQry(Stb) ' As Appezzamento

                specieVarietàLocalitàQry(Stb, True)

                Stb.AppendLine(" 	, qtaProdotto.DescrizioneOperazione ")
                Stb.AppendLine(" 	, Op.Lav_Des AS Operazione ")
                Stb.AppendLine(" 	, qtaProdotto.OpCorrente ")

                Stb.AppendLine(" 	 ")
                Stb.AppendLine(" FROM ( ")
                Stb.AppendLine(" 	SELECT  ")


                appezzaQry(Stb)



                Stb.AppendLine("     , " & descrizioneOperazioneQRY() & " AS DescrizioneOperazione")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine(" 		 , AGG.id_Agenda ")
                    Stb.AppendLine(" 		 , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
                    Stb.AppendLine(" 		 , AGG.id_mov_det ")
                End If

                Stb.AppendLine(" 	 , AGG.Lav_Cod ")
                Stb.AppendLine(" 	 , AGG.OpCorrente ")
                Stb.AppendLine(" 	 , fer.Fer_Cod ")



                Stb.AppendLine(" 		 , SUM(AGG.qta * (AGG.TecN_N / 100) ) AS QtaTotImpianto_N --KG o Lt. ")
                Stb.AppendLine(" 		 , SUM(AGG.qta * (AGG.TecP_P / 100) ) AS QtaTotImpianto_P --KG o Lt. ")
                Stb.AppendLine(" 		 , SUM(AGG.qta * (AGG.TecK_K / 100) ) AS QtaTotImpianto_K --KG o Lt. ")
                Stb.AppendLine(" 		 , SUM(AGG.qta * (AGG.TecCu_Cu / 100) ) AS QtaTotImpianto_Cu --KG o Lt. ")


                'Stb.AppendLine(" 		 , ISNULL(CAST(Massimale_N.val_cod AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_N --KG o Lt. ")
                'Stb.AppendLine(" 		 , ISNULL(CAST(Massimale_P.val_cod AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_P --KG o Lt. ")
                'Stb.AppendLine(" 		 , ISNULL(CAST(Massimale_K.val_cod AS REAL), " & NumeroIndefinito & ") AS MassimaleImpianto_K --KG o Lt. ")

                'Stb.AppendLine(" 		 , CASE WHEN AGG.Num_Protocollo in (0, -1) THEN " & NumeroIndefinito & " else 6 end AS MassimaleImpianto_Cu --KG o Lt. ")
                'Stb.AppendLine(" 		 , CASE WHEN AGG.Num_Protocollo in (0, -1) THEN " & NumeroIndefinito & "  else agg.rame end  AS MassimaleImpianto_Cu --KG o Lt. ")
                ' Stb.AppendLine(" 		 , CASE WHEN AGG.Num_Protocollo = 0 THEN " & NumeroIndefinito & "  else agg.rame end  AS MassimaleImpianto_Cu --KG o Lt. ")
                Stb.AppendLine(" 	     , CASE WHEN AGG.data_movimento < '01/02/2019' THEN 6 ELSE 4 END AS MassimaleImpianto_Cu ")


                'Stb.AppendLine(" 		 , count(*) AS conteggio ")
                Stb.AppendLine("")
                Stb.AppendLine(" 	  ")

                LeggiAgenda(SostenibilitaTipoAnalisi, idTestataTemp, chiamataDaOperazioneAgenda, filtroImpiantiAttivo, filtroAgendaAttivo, 3, Stb)

                Stb.AppendLine(" 	WHERE 1=1 ")

                If Not filtroImpiantiAttivo Then
                    Stb.AppendLine(" 	AND AGG.piva = '" & Agro_SQL_SaveText(piva) & "' ")
                End If

                If OperazioneCorrente_id_Agenda_Escludi > 0 Then
                    Stb.AppendLine(" 	AND AGG.id_Agenda <> " & OperazioneCorrente_id_Agenda_Escludi & " ")
                    Stb.AppendLine(" 	AND ISNULL(AGG.Raccoglitore_Cod, 0) <> ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0)  > 0 THEN Raccoglitore_Cod ELSE -999 END FROM agenda WHERE Piva = reg.piva  AND Id_Agenda = " & OperazioneCorrente_id_Agenda_Escludi & "), -999)  -- 25/05/23 GESTIONE MULTI ATTIVITA ")
                End If

                Stb.AppendLine(" 	AND AGG.lav_cod in ( 14, 156, 124, 26, 123, 106) ")
                Stb.AppendLine("")
                Stb.AppendLine(" 	AND AGG.Cau_Mov IN ('2300') ")

                'vanni, 15/01/2020: sulla tipologia Massimali impianti si attiva il filtro per distinte che intersecano il periodo indicato dall'utente (o letto da op. corrente).
                '     altrimenti occorre attivare il filtro sulle operazioni di agenda incluse nell'intervallo.
                'If Not chiamataDaOperazioneAgenda Then
                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine("    AND AGG.Data_Movimento Between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "  ")
                End If


                Stb.AppendLine(" 	GROUP BY  ")

                appezzaQry(Stb)

                Stb.AppendLine(" 		 , fer.fer_cod ")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine(" 		 , AGG.id_Agenda ")
                    Stb.AppendLine(" 		 , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
                    Stb.AppendLine(" 		 , AGG.id_mov_det ")
                End If

                Stb.AppendLine(" 		 , AGG.lav_Cod ")
                Stb.AppendLine(" 		 , AGG.OpCorrente ")
                'Stb.AppendLine(" 		 , CASE WHEN AGG.Num_Protocollo in (0, -1) THEN " & NumeroIndefinito & " else AGG.Rame end ")
                'Stb.AppendLine(" 		 , CASE WHEN AGG.Num_Protocollo = 0 THEN " & NumeroIndefinito & " else AGG.Rame end ")
                Stb.AppendLine(" 	     , CASE WHEN AGG.data_movimento < '01/02/2019' THEN 6 else 4 end ")

                Stb.AppendLine(" 		 , " & descrizioneOperazioneQRY() & " ")

                Stb.AppendLine(" ) QtaProdotto ")
                Stb.AppendLine(" 	 ")
                Stb.AppendLine(" 	inner join 	 ")
                Stb.AppendLine(" 	( ")

                Stb.AppendLine("SELECT   ")
                Stb.AppendLine("      fer.Fer_Cod, ")
                appezzaQry(Stb)
                Stb.AppendLine("    , " & descrizioneOperazioneQRY() & " AS DescrizioneOperazione")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine(" 	, AGG.id_Agenda ")
                    Stb.AppendLine(" 	, AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
                    Stb.AppendLine(" 	, AGG.id_mov_det ")
                End If

                Stb.AppendLine(" 	, AGG.lav_Cod ")
                Stb.AppendLine("    , AGG.OpCorrente ")
                Stb.AppendLine("    , SUM(qta2) AS SupTrattataImpianto  ")


                LeggiAgenda(SostenibilitaTipoAnalisi, idTestataTemp, chiamataDaOperazioneAgenda, filtroImpiantiAttivo, filtroAgendaAttivo, -3, Stb)

                Stb.AppendLine(" 			WHERE 1=1 ")

                If Not filtroImpiantiAttivo AndAlso Not filtroAgendaAttivo Then
                    Stb.AppendLine(" 			AND AGG.piva = '" & Agro_SQL_SaveText(piva) & "' ")
                End If

                If OperazioneCorrente_id_Agenda_Escludi > 0 Then
                    Stb.AppendLine(" 			AND AGG.id_Agenda <> " & OperazioneCorrente_id_Agenda_Escludi & " ")
                    Stb.AppendLine(" 			AND ISNULL(AGG.Raccoglitore_Cod, 0) <> ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0) > 0 THEN Raccoglitore_Cod ELSE -999 END FROM agenda WHERE Piva = reg.piva AND Id_Agenda = " & OperazioneCorrente_id_Agenda_Escludi & "), -999)  -- 25/05/23 GESTIONE MULTI ATTIVITA ")

                End If

                Stb.AppendLine(" 			AND AGG.lav_cod in (14, 156, 124, 26, 123, 106) ")
                Stb.AppendLine(" 			AND AGG.Tipo_Destinazione = 0 ")
                Stb.AppendLine(" 			AND AGG.Cau_Mov IN ('2300') ")

                'vanni, 15/01/2020: sulla tipologia Massimali impianti si attiva il filtro per distinte che intersecano il periodo indicato dall'utente (o letto da op. corrente).
                '     altrimenti occorre attivare il filtro sulle operazioni di agenda incluse nell'intervallo.
                'If Not chiamataDaOperazioneAgenda Then
                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine("    AND AGG.Data_Movimento Between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "  ")
                End If

                Stb.AppendLine("")
                Stb.AppendLine(" 		GROUP BY  ")
                Stb.AppendLine("")

                appezzaQry(Stb)

                Stb.AppendLine(" 		 , fer.fer_cod ")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine(" 		 , AGG.id_Agenda ")
                    Stb.AppendLine(" 		 , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
                    Stb.AppendLine(" 		 , AGG.id_mov_det ")
                End If

                Stb.AppendLine(" 	     , AGG.lav_Cod ")
                Stb.AppendLine(" 		 , AGG.OpCorrente  ")
                Stb.AppendLine(" 		 , " & descrizioneOperazioneQRY() & " ")

                'alias tabella
                Stb.AppendLine(")  SupMedia on   ")

                Stb.AppendLine("      QtaProdotto.Fer_Cod = SupMedia.Fer_Cod ")

                Stb.AppendLine("  And QtaProdotto.piva = supMedia.piva ")
                Stb.AppendLine("  And QtaProdotto.Sa_cod = supMedia.sa_cod ")
                Stb.AppendLine("  And QtaProdotto.Appezza = supMedia.appezza ")
                Stb.AppendLine("  And QtaProdotto.Id_Reg = supMedia.id_Reg ")

                Stb.AppendLine("    AND QtaProdotto.DescrizioneOperazione = SupMedia.DescrizioneOperazione  ")
                Stb.AppendLine("    AND QtaProdotto.lav_cod = SupMedia.lav_cod  ")
                Stb.AppendLine("    AND QtaProdotto.OpCorrente = SupMedia.OpCorrente  ")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine("    AND QtaProdotto.id_Agenda = SupMedia.id_agenda  ")
                    Stb.AppendLine("    AND QtaProdotto.id_mov_det = SupMedia.id_mov_det  ")
                End If


                Stb.AppendLine("  --decodifica delle varie chiavi e recupero delle descrizioni ")

                decodificaChiaviRecuperoDescrizioni(Stb, "Fertilizzanti")

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then

                    Stb.AppendLine(" left join __Tmp_Movimenti_Destinazioni_DateDistinta ddd ")
                    Stb.AppendLine(" 	on ddd.piva = reg.piva  ")
                    Stb.AppendLine(" 	AND ddd.sa_cod = reg.sa_cod  ")
                    Stb.AppendLine(" 	AND ddd.appezza = reg.appezza ")
                    Stb.AppendLine(" 	AND ddd.id_reg = reg.id_reg ")
                    Stb.AppendLine(" 	AND ddd.idTestataTemp =  " & idTestataTemp)

                    Stb.AppendLine("              left join reg_impianti_codici Massimale_N  ")
                    Stb.AppendLine("                on Massimale_N.piva = reg.Piva  ")
                    Stb.AppendLine("                  AND Massimale_N.sa_cod = reg.sa_cod  ")
                    Stb.AppendLine("                  AND Massimale_N.appezza = reg.appezza  ")
                    Stb.AppendLine("                  AND Massimale_N.id_reg = reg.id_reg  ")
                    Stb.AppendLine("                  AND Massimale_N.id_cod = 1050  ")
                    Stb.AppendLine("                  AND Massimale_N.progetto_Cod = ddd.progetto_cod  ")


                    Stb.AppendLine("              left join reg_impianti_codici Massimale_P  ")
                    Stb.AppendLine("                on Massimale_P.piva = reg.Piva  ")
                    Stb.AppendLine("                  AND Massimale_P.sa_cod = reg.sa_cod  ")
                    Stb.AppendLine("                  AND Massimale_P.appezza = reg.appezza  ")
                    Stb.AppendLine("                  AND Massimale_P.id_reg = reg.id_reg  ")
                    Stb.AppendLine("                  AND Massimale_P.id_cod = 1051  ")
                    Stb.AppendLine("                  AND Massimale_P.progetto_Cod = ddd.progetto_cod  ")

                    Stb.AppendLine("              left join reg_impianti_codici Massimale_K  ")
                    Stb.AppendLine("                on Massimale_K.piva = reg.Piva  ")
                    Stb.AppendLine("                  AND Massimale_K.sa_cod = reg.sa_cod  ")
                    Stb.AppendLine("                  AND Massimale_K.appezza = reg.appezza  ")
                    Stb.AppendLine("                  AND Massimale_K.id_reg = reg.id_reg  ")
                    Stb.AppendLine("                  AND Massimale_K.id_cod = 1052  ")
                    Stb.AppendLine("                  AND Massimale_K.progetto_Cod = ddd.progetto_cod  ")
                End If


                If Veg_Cod <> -1 Then
                    Stb.AppendLine(" 	WHERE veg.veg_cod = " & Veg_Cod & " ")
                End If

                'Stb.AppendLine(" order by ff.fer_des ")


                'scrivo la tabella temporanea, poi procedo con la selezione.
                EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine & ", delete temp")

            End If


            Stb.Length = 0


            Stb.AppendLine("SELECT * ")
            Stb.AppendLine("FROM ( ")


            'parte di selezione...finalmente .. !
            If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.AnalisiPerProdottiFertilizzanti Then
                Stb = Leggi_EstrazioneAgendaQry(SostenibilitaTipoAnalisi, idTestataTemp, piva, Veg_Cod, DataDa, DataA, OperazioneCorrente_id_Agenda_Escludi, Stb, chiamataDaOperazioneAgenda, OperazioneCorrenteFertilizzazione, aliasDescrizioneOperazione, aliasOperazione, filtroImpiantiAttivo, filtroAgendaAttivo, 1, isMultiAttivita:=isMultiAttivita)
            End If

            If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottiFertilizzanti Then

                Stb = Leggi_EstrazioneAgendaQry(SostenibilitaTipoAnalisi, idTestataTemp, piva, Veg_Cod, DataDa, DataA, OperazioneCorrente_id_Agenda_Escludi, Stb, chiamataDaOperazioneAgenda, OperazioneCorrenteFertilizzazione, aliasDescrizioneOperazione, aliasOperazione, filtroImpiantiAttivo, filtroAgendaAttivo, 3)

            Else

                'in seguito la selezione per le fertilizzazioni
                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then



                    Stb.AppendLine("")
                    Stb.AppendLine(" UNION ALL ")
                    Stb.AppendLine("")

                    'Rame (letto sia in fert. che trattamenti)
                    Stb.AppendLine("    SELECT  ")
                    Stb.AppendLine("        'Fertilizzante' AS Sostanza_Tipo ")
                    Stb.AppendLine("        , 'Rame' AS Sostanza ")
                    Stb.AppendLine("        , Appezzamento ")
                    Stb.AppendLine("        , Specie ")
                    Stb.AppendLine("        , Varieta AS Varietà ")
                    Stb.AppendLine("        , Localita AS Località ")
                    Stb.AppendLine("        , Provincia ")
                    Stb.AppendLine("        , DescrizioneOperazione " & aliasDescrizioneOperazione)
                    Stb.AppendLine("        , Operazione " & aliasOperazione)
                    Stb.AppendLine("        , OpCorrente ")
                    Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_Cu), 3) AS QtaTotImpianto ")
                    Stb.AppendLine("        , ROUND(SUM(SupTrattataImpiantoMedia), 3) AS SupTrattataImpiantoMedia ")

                    If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                        Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_Cu / SupTrattataImpiantoMedia), 3) AS DoseHa ")
                    Else
                        Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_Cu) / SUM(SupTrattataImpiantoMedia), 3) AS DoseHa ")
                    End If

                    Stb.AppendLine("         , MAX(MassimaleImpianto_Cu) AS Massimale ")
                    Stb.AppendLine("         --QtaTotImpianto		DoseHa_SuMediaSup ")
                    Stb.AppendLine("    FROM [__Agenda_Rpt_Conc] ")
                    Stb.AppendLine("    WHERE qtaTotImpianto_Cu<>0 ")
                    Stb.AppendLine("    AND IDTestataTemp = " & idTestataTemp)
                    Stb.AppendLine("    GROUP BY Appezzamento, Specie, Varieta, Provincia, Localita, DescrizioneOperazione, Operazione, OpCorrente ")


                    If OperazioneCorrente_Lav_Cod = 0 Or OperazioneCorrenteFertilizzazione Then

                        Stb.AppendLine("")
                        Stb.AppendLine(" UNION ALL ")
                        Stb.AppendLine("")

                        'Azoto
                        Stb.AppendLine("    SELECT  ")
                        Stb.AppendLine("         'Fertilizzante' AS Sostanza_Tipo ")
                        Stb.AppendLine("         , 'Azoto' AS Sostanza ")
                        Stb.AppendLine("         , Appezzamento ")
                        Stb.AppendLine("         , Specie ")
                        Stb.AppendLine("         , Varieta AS Varietà ")
                        Stb.AppendLine("         , Localita AS Località ")
                        Stb.AppendLine("         , Provincia ")
                        Stb.AppendLine("         , DescrizioneOperazione " & aliasDescrizioneOperazione)
                        Stb.AppendLine("         , Operazione " & aliasOperazione)
                        Stb.AppendLine("         , OpCorrente ")
                        Stb.AppendLine("         , ROUND(SUM(qtaTotImpianto_N), 3) AS QtaTotImpianto ")
                        Stb.AppendLine("         , ROUND(SUM(SupTrattataImpiantoMedia), 3) AS SupTrattataImpiantoMedia ")

                        If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                            Stb.AppendLine("         , ROUND(SUM(qtaTotImpianto_N / SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        Else
                            Stb.AppendLine("         , ROUND(SUM(qtaTotImpianto_N) / SUM(SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        End If

                        Stb.AppendLine("         , MAX(MassimaleImpianto_N) AS Massimale ")
                        Stb.AppendLine("         --QtaTotImpianto		DoseHa_SuMediaSup ")
                        Stb.AppendLine("    FROM [__Agenda_Rpt_Conc] ")
                        Stb.AppendLine("    WHERE qtaTotImpianto_N<>0 ")
                        Stb.AppendLine("    AND IDTestataTemp = " & idTestataTemp)
                        Stb.AppendLine("    GROUP BY Appezzamento, Specie, Varieta, Provincia, Localita, DescrizioneOperazione, Operazione, OpCorrente ")

                        Stb.AppendLine("")
                        Stb.AppendLine(" UNION ALL ")
                        Stb.AppendLine("")

                        'Fosforo
                        Stb.AppendLine("    SELECT  ")
                        Stb.AppendLine("        'Fertilizzante' AS Sostanza_Tipo ")
                        Stb.AppendLine("        , 'Fosforo' AS Sostanza ")
                        Stb.AppendLine("        , Appezzamento ")
                        Stb.AppendLine("        , Specie ")
                        Stb.AppendLine("        , Varieta AS Varietà ")
                        Stb.AppendLine("        , Localita AS Località ")
                        Stb.AppendLine("        , Provincia ")
                        Stb.AppendLine("        , DescrizioneOperazione " & aliasDescrizioneOperazione)
                        Stb.AppendLine("        , Operazione " & aliasOperazione)
                        Stb.AppendLine("        , OpCorrente ")
                        Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_P), 3) AS Valore ")
                        Stb.AppendLine("        , ROUND(SUM(SupTrattataImpiantoMedia) , 3) AS SupTrattataImpiantoMedia ")

                        If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                            Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_P / SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        Else
                            Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_P) / SUM(SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        End If

                        Stb.AppendLine("        , MAX(MassimaleImpianto_P) AS Massimale ")
                        Stb.AppendLine("        --QtaTotImpianto		DoseHa_SuMediaSup ")
                        Stb.AppendLine("    FROM [__Agenda_Rpt_Conc] ")
                        Stb.AppendLine("    WHERE qtaTotImpianto_P<>0 ")
                        Stb.AppendLine("    AND IDTestataTemp = " & idTestataTemp)
                        Stb.AppendLine("    GROUP BY Appezzamento, Specie, Varieta, Provincia, Localita, DescrizioneOperazione, Operazione, OpCorrente ")
                        Stb.AppendLine("    --GROUP BY Fr_Des ")

                        Stb.AppendLine("")
                        Stb.AppendLine(" UNION ALL ")
                        Stb.AppendLine("")

                        'Potassio
                        Stb.AppendLine("    SELECT  ")
                        Stb.AppendLine("        'Fertilizzante' AS Sostanza_Tipo ")
                        Stb.AppendLine("        , 'Potassio' AS Sostanza ")
                        Stb.AppendLine("        , Appezzamento ")
                        Stb.AppendLine("        , Specie ")
                        Stb.AppendLine("        , Varieta AS Varietà ")
                        Stb.AppendLine("        , Localita AS Località ")
                        Stb.AppendLine("        , Provincia ")
                        Stb.AppendLine("        , DescrizioneOperazione " & aliasDescrizioneOperazione)
                        Stb.AppendLine("        , Operazione " & aliasOperazione)
                        Stb.AppendLine("        , OpCorrente ")
                        Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_K), 3) AS Valore ")
                        Stb.AppendLine("        , ROUND(SUM(SupTrattataImpiantoMedia), 3) AS SupTrattataImpiantoMedia ")

                        If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                            Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_K / SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        Else
                            Stb.AppendLine("        , ROUND(SUM(qtaTotImpianto_K) / SUM(SupTrattataImpiantoMedia), 3) AS DoseHa ")
                        End If

                        Stb.AppendLine("        , MAX(MassimaleImpianto_K) AS Massimale ")
                        Stb.AppendLine("        --QtaTotImpianto		DoseHa_SuMediaSup ")
                        Stb.AppendLine("    FROM [__Agenda_Rpt_Conc] ")
                        Stb.AppendLine("    WHERE qtaTotImpianto_K<>0 ")
                        Stb.AppendLine("    AND IDTestataTemp = " & idTestataTemp)
                        Stb.AppendLine("    GROUP BY Appezzamento, Specie, Varieta, Provincia, Localita, DescrizioneOperazione, Operazione, OpCorrente ")
                        Stb.AppendLine("    --GROUP BY Fr_Des ")


                    End If

                End If
                'solo se non è un prodotto commerciale


            End If
            'test su tipo analisi (formulato comm.le o meno...)

            Stb.AppendLine(" ) Risultato ")

            Stb.AppendLine(" ORDER BY Sostanza_Tipo, Sostanza, Data  ")

            Stb.AppendLine(" SET NOCOUNT OFF ")

            'PER FEDE PER DEBUG !!!!!!!!!
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Stb.Length = 0

            Dim tableDistinct = DT.DefaultView.ToTable(True, "Sostanza", "Appezzamento")

            For Each distinctVal In tableDistinct.Rows

                Dim rows = DT.Select(" Sostanza = '" & UtilityProvider.Agro_SQL_SaveText(distinctVal("Sostanza")) &
                          "' AND Appezzamento = '" & UtilityProvider.Agro_SQL_SaveText(distinctVal("Appezzamento")) & "' ")

                Dim i = 0
                Dim lastMax As Decimal = 0
                Dim lastAvgDoseHa As Decimal = 0
                For Each row In rows
                    If i = 0 Then
                        If row("Massimale") = 0 Then
                            Exit For
                        End If
                        lastMax = row("Massimale")
                        lastAvgDoseHa = row("AvgDoseHa")
                    Else
                        row("Massimale") = lastMax - lastAvgDoseHa
                        lastAvgDoseHa = row("AvgDoseHa")
                        lastMax = row("Massimale")
                    End If
                    i += 1
                Next

            Next


            'elimino la tabella temporanea.
            puliziaTabelleTemporanee(idTestataTemp, objParametri, NomeRoutine)

            ' VAnni: 1/2/2019: da prossima versione non ci sono più tabelle temp
            'drop tabelle temp
            'tabelle di lettura dati temporanei op. agenda in corso.
            'Stb.AppendLine(" drop table #__Tmp_Agenda  ")
            'Stb.AppendLine(" drop table #__Tmp_Movimenti  ")
            'Stb.AppendLine(" drop table #__Tmp_Movimenti_dettagli  ")
            'Stb.AppendLine(" drop table #__Tmp_Mov_dettaglioTecnico  ")
            'Stb.AppendLine(" drop table #__Tmp_Movimenti_Destinazioni ")
            'Stb.AppendLine(" drop table #__Tmp_Movimenti_Destinazioni_DateDistinta ")

            'EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine & ", drop temp")

            ConnessioniTransazioni.ChiudiConnessione(objParametri)

            'objParametri.objConnessione.Dispose()

        Catch ex As Exception

            puliziaTabelleTemporanee(idTestataTemp, objParametri, NomeRoutine)

            MessaggioErrore = ex.Message
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        End Try

        Return DT





    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TipoAnalisi"></param>
    ''' <param name="idTestataTemp"></param>
    ''' <param name="piva"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="DataDa"></param>
    ''' <param name="DataA"></param>
    ''' <param name="OperazioneCorrente_id_Agenda_Escludi"></param>
    ''' <param name="Stb"></param>
    ''' <param name="chiamataDaOperazioneAgenda"></param>
    ''' <param name="OperazioneCorrenteFertilizzazione"></param>
    ''' <param name="aliasDescrizioneOperazione"></param>
    ''' <param name="aliasOperazione"></param>
    ''' <param name="filtroImpiantiAttivo"></param>
    ''' <param name="tipoLetturaAgenda">1 - prodotto fitosanitario, 3 - fertilizzante</param>
    ''' <returns></returns>
    Private Function Leggi_EstrazioneAgendaQry(
        TipoAnalisi As enum_SostenibilitaTipoAnalisi,
        idTestataTemp As Integer,
        piva As String,
        Veg_Cod As String,
        DataDa As Date,
        DataA As Date,
        OperazioneCorrente_id_Agenda_Escludi As Integer,
        Stb As StringBuilder,
        chiamataDaOperazioneAgenda As Boolean,
        OperazioneCorrenteFertilizzazione As Boolean,
        aliasDescrizioneOperazione As String,
        aliasOperazione As String,
        filtroImpiantiAttivo As Boolean,
        filtroAgendaAttivo As Boolean,
        tipoLetturaAgenda As Integer,
        Optional isMultiAttivita As Boolean = False
     ) As StringBuilder


        Stb.AppendLine("    SELECT")
        If tipoLetturaAgenda = 1 Then
            Stb.AppendLine("        'Prodotto Fitosanitario'  AS Sostanza_Tipo ")
        Else
            Stb.AppendLine("        'Fertilizzante'  AS Sostanza_Tipo ")
        End If

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottiFertilizzanti Then
            TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale
        End If

        If tipoLetturaAgenda = 1 Then
            If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
                Stb.AppendLine("    , ff.fr_Des AS Sostanza ")
            Else
                Stb.AppendLine("        , CASE WHEN ff.pa_Cod IN " & PaRameici_str & " then 'Rame' ELSE ff.pa_Des END AS Sostanza ")
            End If
        Else
            Stb.AppendLine("        , ff.fer_Des AS Sostanza ")
        End If

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale And tipoLetturaAgenda = 1 Then
            Stb.AppendLine("        , ff.fr_cod")
        End If

        appezzaDescrizioniQry(Stb) 'AS Appezzamento

        specieVarietàLocalitàQry(Stb, True)

        Stb.AppendLine("        , CAST(QtaProdotto.DescrizioneOperazione AS DATE)  " & aliasDescrizioneOperazione)
        Stb.AppendLine("        , op.lav_des  " & aliasOperazione)
        Stb.AppendLine("        , QtaProdotto.OpCorrente  AS OpCorrente ")
        Stb.AppendLine("        , ROUND( QtaProdotto.QtaTotImpianto, 3 ) AS QtaTotImpianto")
        Stb.AppendLine("        , ROUND( SupMedia.SupTrattataImpianto, 3) AS SupTrattataImpianto ")
        Stb.AppendLine("        , ROUND(QtaProdotto.QtaTotImpianto / SupMedia.SupTrattataImpianto, 3) AS AvgDoseHa ")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
            Stb.AppendLine("        , " & NumeroIndefinito & " AS Massimale")
        Else
            Stb.AppendLine("        , CASE WHEN ff.pa_Cod IN " & PaRameici_str & " THEN rame ELSE 0 END AS Massimale")
        End If


        Stb.AppendLine("    FROM ( ")

        Stb.AppendLine("        SELECT  ")
        appezzaQry(Stb)

        FormulatoPrincipioFertilizzante(TipoAnalisi, Stb, tipoLetturaAgenda)


        Stb.AppendLine("            , agg.rame ")
        Stb.AppendLine("            , " & descrizioneOperazioneQRY() & " AS DescrizioneOperazione ")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("            , AGG.id_Agenda ")
            Stb.AppendLine("            , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
            Stb.AppendLine("            , AGG.id_mov_det ")
        End If

        Stb.AppendLine("            , Agg.lav_cod ")
        Stb.AppendLine("            , AGG.OpCorrente ")

        'Stb.AppendLine("       , SUM(AGG.qta * (frPa.Titolo / 100) ) AS QtaTotImpianto --KG o Lt.       ")
        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
            Stb.AppendLine("            , SUM(AGG.qta) AS QtaTotImpianto --KG o Lt.       ")
        Else
            Stb.AppendLine("            , SUM(AGG.qta * CASE WHEN frPA.peso = 0 THEN (frPa.Titolo / 100) ELSE (frPa.Peso / 1000) END)  AS QtaTotImpianto --KG o Lt.       ")
        End If

        LeggiAgenda(TipoAnalisi, idTestataTemp, chiamataDaOperazioneAgenda, filtroImpiantiAttivo, filtroAgendaAttivo, tipoLetturaAgenda, Stb)

        Stb.AppendLine("		WHERE 1=1 ")

        If Not filtroImpiantiAttivo AndAlso Not filtroAgendaAttivo Then
            Stb.AppendLine("		AND AGG.piva = '" & Agro_SQL_SaveText(piva) & "' ")
        End If


        If OperazioneCorrente_id_Agenda_Escludi > 0 Then
            Stb.AppendLine("		AND AGG.id_Agenda <> " & OperazioneCorrente_id_Agenda_Escludi & " ")
            Stb.AppendLine("		AND ISNULL(AGG.Raccoglitore_Cod, 0) <> ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0)  > 0 THEN Raccoglitore_Cod ELSE -999 END FROM agenda where Piva = reg.piva  AND Id_Agenda = " & OperazioneCorrente_id_Agenda_Escludi & "), -999)  -- 25/05/23 GESTIONE MULTI ATTIVITA ")
        End If

        Stb.AppendLine("")

        'fito o fertilizzanti
        If tipoLetturaAgenda = 1 Then
            Stb.AppendLine("		AND AGG.lav_cod IN ( 74, 103, 18, 155, 13, 158) ")
            Stb.AppendLine("		AND AGG.Cau_Mov IN ('2050')   ")
        Else
            Stb.AppendLine("		AND AGG.lav_cod IN ( 14, 156, 124, 26, 123, 106) ")
            Stb.AppendLine("		AND AGG.Cau_Mov IN ('2300')   ")
        End If


        'se sono su una concimazione, leggo i soli trattamenti con principi attivi rameici            
        If OperazioneCorrenteFertilizzazione AndAlso isMultiAttivita = False Then
            Stb.AppendLine("    AND pa.pa_Cod IN " & PaRameici_str & " ")
        End If

        'vanni, 15/01/2020: sulla tipologia Massimali impianti si attiva il filtro per distinte che intersecano il periodo indicato dall'utente (o letto da op. corrente).
        '     altrimenti occorre attivare il filtro sulle operazioni di agenda incluse nell'intervallo.
        'If Not chiamataDaOperazioneAgenda Then
        If TipoAnalisi <> enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("		AND AGG.Data_Movimento Between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "  ")
        End If


        Stb.AppendLine("")
        Stb.AppendLine("		GROUP BY  ")
        Stb.AppendLine("")

        appezzaQry(Stb)

        'specieVarietàLocalitàQry(Stb, False)


        FormulatoPrincipioFertilizzante(TipoAnalisi, Stb, tipoLetturaAgenda)

        Stb.AppendLine("        , agg.rame ")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("        , AGG.id_Agenda ")
            Stb.AppendLine("        , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
            Stb.AppendLine("        , AGG.id_mov_det ")
        End If

        Stb.AppendLine("        , AGG.Lav_cod ")
        Stb.AppendLine("        , AGG.OpCorrente ")
        Stb.AppendLine("        , " & descrizioneOperazioneQRY() & "")

        Stb.AppendLine("    ) QtaProdotto ")
        Stb.AppendLine(" 	 ")
        Stb.AppendLine("    INNER JOIN 	 ")
        Stb.AppendLine("    ( ")

        Stb.AppendLine("        SELECT   ")

        appezzaQry(Stb)
        FormulatoPrincipioFertilizzante(TipoAnalisi, Stb, tipoLetturaAgenda)


        Stb.AppendLine("            , " & descrizioneOperazioneQRY() & " AS DescrizioneOperazione")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("            , AGG.id_Agenda ")
            Stb.AppendLine("            , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
            Stb.AppendLine("            , AGG.Id_Mov_Det ")
        End If

        Stb.AppendLine("            , AGG.lav_Cod ")
        Stb.AppendLine("            , AGG.OpCorrente ")
        Stb.AppendLine("            , SUM(qta2) AS SupTrattataImpianto  ")


        LeggiAgenda(TipoAnalisi, idTestataTemp, chiamataDaOperazioneAgenda, filtroImpiantiAttivo, filtroAgendaAttivo, -tipoLetturaAgenda, Stb)


        Stb.AppendLine("            WHERE 1=1 ")

        If Not filtroImpiantiAttivo And Not filtroAgendaAttivo Then
            Stb.AppendLine("            AND AGG.piva = '" & Agro_SQL_SaveText(piva) & "' ")
        End If

        If OperazioneCorrente_id_Agenda_Escludi > 0 Then
            Stb.AppendLine("            AND AGG.id_Agenda <> " & OperazioneCorrente_id_Agenda_Escludi & " ")
            Stb.AppendLine("            AND ISNULL(AGG.Raccoglitore_Cod, 0) <> ISNULL((SELECT CASE WHEN ISNULL(Raccoglitore_Cod, 0)  > 0 THEN Raccoglitore_Cod ELSE -999 END FROM agenda where Piva = reg.piva  AND Id_Agenda = " & OperazioneCorrente_id_Agenda_Escludi & "), -999)  -- 25/05/23 GESTIONE MULTI ATTIVITA ")
        End If

        'fito o fertilizzanti
        If tipoLetturaAgenda = 1 Then
            Stb.AppendLine("            AND AGG.lav_cod IN ( 74, 103, 18, 155, 13, 158) ")
            Stb.AppendLine("            AND AGG.Cau_Mov IN ('2050')   ")
        Else
            Stb.AppendLine("            AND AGG.lav_cod IN ( 14, 156, 124, 26, 123, 106) ")
            Stb.AppendLine("            AND AGG.Cau_Mov IN ('2300')   ")
        End If

        Stb.AppendLine("            AND AGG.Tipo_Destinazione = 0        ")

        'vanni, 15/01/2020: sulla tipologia Massimali impianti si attiva il filtro per distinte che intersecano il periodo indicato dall'utente (o letto da op. corrente).
        '     altrimenti occorre attivare il filtro sulle operazioni di agenda incluse nell'intervallo.
        'If Not chiamataDaOperazioneAgenda Then
        If TipoAnalisi <> enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("            AND AGG.Data_Movimento Between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "  ")
        End If

        Stb.AppendLine("")
        Stb.AppendLine("            GROUP BY  ")


        appezzaQry(Stb)

        'specieVarietàLocalitàQry(Stb, False)

        FormulatoPrincipioFertilizzante(TipoAnalisi, Stb, tipoLetturaAgenda)


        Stb.AppendLine("                , agg.rame ")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("                , AGG.id_Agenda ")
            Stb.AppendLine("                , AGG.RACCOGLITORE_COD -- 25/05/23 GESTIONE MULTI ATTIVITA ")
            Stb.AppendLine("                , AGG.id_Mov_Det ")
        End If

        Stb.AppendLine("                , AGG.lav_Cod ")
        Stb.AppendLine("                , AGG.OpCorrente  ")
        Stb.AppendLine("                , " & descrizioneOperazioneQRY() & " ")

        'alias tabella            
        Stb.AppendLine("    )  SupMedia ON  ")


        If tipoLetturaAgenda = 1 Then
            If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
                Stb.AppendLine("        QtaProdotto.fr_cod = SupMedia.fr_cod  ")
            Else
                Stb.AppendLine("        QtaProdotto.pa_cod = SupMedia.pa_cod  ")
            End If
        Else
            Stb.AppendLine("        QtaProdotto.fer_cod = SupMedia.fer_cod  ")
        End If


        Stb.AppendLine("        AND QtaProdotto.piva = supMedia.piva ")
        Stb.AppendLine("        AND QtaProdotto.Sa_cod = supMedia.sa_cod ")
        Stb.AppendLine("        AND QtaProdotto.Appezza = supMedia.appezza ")
        Stb.AppendLine("        AND QtaProdotto.Id_Reg = supMedia.id_Reg ")

        Stb.AppendLine("        AND QtaProdotto.DescrizioneOperazione = SupMedia.DescrizioneOperazione  ")
        Stb.AppendLine("        AND QtaProdotto.lav_cod = SupMedia.lav_cod  ")
        Stb.AppendLine("        AND QtaProdotto.OpCorrente = SupMedia.OpCorrente  ")

        If TipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
            Stb.AppendLine("        AND QtaProdotto.id_Agenda = SupMedia.id_Agenda ")
            Stb.AppendLine("        AND QtaProdotto.id_mov_det =  SupMedia.id_mov_det ")
        End If

        If tipoLetturaAgenda = 1 Then
            If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
                decodificaChiaviRecuperoDescrizioni(Stb, "Formulati")
            Else
                decodificaChiaviRecuperoDescrizioni(Stb, "PrincipiAttivi")
            End If
        Else
            decodificaChiaviRecuperoDescrizioni(Stb, "Fertilizzanti")
        End If


        Stb.AppendLine("    WHERE QtaProdotto.QtaTotImpianto <> 0 ")


        If Veg_Cod <> -1 Then
            Stb.AppendLine("    AND veg.veg_cod = " & Veg_Cod & " ")
        End If

        Return Stb
    End Function

    Private Shared Sub FormulatoPrincipioFertilizzante(TipoAnalisi As enum_SostenibilitaTipoAnalisi, Stb As StringBuilder, tipoLetturaAgenda As Integer)
        If tipoLetturaAgenda = 1 Then
            If TipoAnalisi = enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then
                Stb.AppendLine("                , fr.fr_cod ")
            Else
                Stb.AppendLine("                , pa.pa_cod ")
            End If
        Else
            Stb.AppendLine("                , fer.fer_cod ")
        End If
    End Sub

    Private Shared Sub decodificaChiaviRecuperoDescrizioni(Stb As StringBuilder, ByVal tabellaFertilizzantiFormulati As String)
        Stb.AppendLine("")
        Stb.AppendLine("    INNER JOIN reg_impianti reg ")
        Stb.AppendLine("        ON  reg.piva = QtaProdotto.piva ")
        Stb.AppendLine("        AND reg.Sa_cod = QtaProdotto.sa_cod ")
        Stb.AppendLine("        AND reg.Appezza = QtaProdotto.appezza ")
        Stb.AppendLine("        AND reg.Id_Reg = QtaProdotto.id_Reg ")
        Stb.AppendLine("")

        Stb.AppendLine("")
        Stb.AppendLine("    INNER JOIN Appezzamento app  ")
        Stb.AppendLine("       ON reg.piva = app.Piva  ")
        Stb.AppendLine("       AND reg.sa_cod = app.sa_cod  ")
        Stb.AppendLine("       AND reg.appezza = app.appezza  ")
        Stb.AppendLine("")

        Stb.AppendLine("    INNER JOIN Centri_Aziendali sa  ")
        Stb.AppendLine("        ON sa.piva = QtaProdotto.piva  ")
        Stb.AppendLine("        AND sa.sa_cod = QtaProdotto.sa_cod  ")
        Stb.AppendLine("")


        Stb.AppendLine("    INNER JOIN Operazioni oP ")
        Stb.AppendLine("        ON oP.Lav_Cod = QtaProdotto.lav_cod ")
        Stb.AppendLine("")
        Stb.AppendLine("    INNER JOIN Cultivar cul   ")
        Stb.AppendLine("        ON cul.cul_cod = reg.cul_cod   ")
        Stb.AppendLine("")
        Stb.AppendLine("    INNER JOIN SpecieVegetali veg   ")
        Stb.AppendLine("        ON veg.veg_cod = cul.veg_cod  ")

        Dim colonnaFertForCod As String = "        ON SupMedia.pa_Cod = ff.pa_Cod "
        If tabellaFertilizzantiFormulati = "Fertilizzanti" Then
            colonnaFertForCod = "        ON SupMedia.Fer_Cod = ff.Fer_Cod "
        End If

        If tabellaFertilizzantiFormulati = "Formulati" Then
            colonnaFertForCod = "        ON SupMedia.Fr_Cod = ff.Fr_Cod "
        End If

        Stb.AppendLine("    INNER JOIN " & tabellaFertilizzantiFormulati & " ff ")
        Stb.AppendLine(colonnaFertForCod)
        Stb.AppendLine("")

        Stb.AppendLine("    LEFT JOIN ( ")

        Stb.AppendLine("        SELECT   ")
        Stb.AppendLine("            ci.piva  ")
        Stb.AppendLine("            , ci.sa_cod  ")
        Stb.AppendLine("            , icom.LOCALITA as Località  ")
        Stb.AppendLine("            , p.Provincia  ")
        Stb.AppendLine("        FROM  ( ")

        'vanni, fede, 30/10/2018: in diverse installazioni, per esempio LAN potrebbero esserci più indirizzi per centro
        Stb.AppendLine("            SELECT MAX(ci1.cod_indirizzo) AS Cod_indirizzo, ci1.piva, ci1.sa_cod ")
        Stb.AppendLine("            FROM centriXindirizzi ci1 ")
        Stb.AppendLine("            GROUP BY ci1.piva, ci1.sa_cod ")

        Stb.AppendLine("        ) ci ")

        Stb.AppendLine("        INNER JOIN Indirizzi ii  ")
        Stb.AppendLine("            ON ci.cod_indirizzo = ii.cod_indirizzo  ")
        Stb.AppendLine("        INNER JOIN istat iCom  ")
        Stb.AppendLine("            ON iCom.prov = ii.pro_cod_istat  ")
        Stb.AppendLine("            AND iCom.com = ii.com_cod_istat  ")
        Stb.AppendLine("        INNER JOIN Lista_Province p  ")
        Stb.AppendLine("            ON p.PROV = iCom.PROV ")
        Stb.AppendLine("    ) iCom")

        Stb.AppendLine("        ON iCom.Piva = sa.Piva ")
        Stb.AppendLine("        AND iCom.Sa_Cod = sa.Sa_cod")
    End Sub

    Private Sub puliziaTabelleTemporanee(idTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri, NomeRoutine As String)

        Dim stb As New StringBuilder

        stb.AppendLine(" delete from  __Tmp_Agenda  where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from  __Tmp_Movimenti  where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from  __Tmp_Movimenti_dettagli  where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from  __Tmp_Mov_dettaglioTecnico  where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from  __Tmp_Movimenti_Destinazioni where IDTestatatemp = " & idTestataTemp & " ")

        stb.AppendLine(" delete from __Agenda_Rpt_Conc where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from __tmp_FiltroImpianti where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from __tmp_FormulatiXPrincipiAttivi where IDTestatatemp = " & idTestataTemp & " ")
        stb.AppendLine(" delete from __Tmp_Movimenti_Destinazioni_DateDistinta where IDTestatatemp = " & idTestataTemp & " ")

        EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine & ", delete temp")

    End Sub

    Private Shared Sub specieVarietàLocalitàQry(Stb As StringBuilder, ByVal aliases As Boolean)

        Stb.AppendLine("        , veg.Veg_Des ")
        If aliases Then Stb.AppendLine("        AS Specie")

        Stb.AppendLine("        , veg.Veg_Des + ' - ' + cul.Cul_Des ")
        If aliases Then Stb.AppendLine("        AS Varietà")

        Stb.AppendLine("        , coalesce( iCom.Località, 'Non Specificata') ")
        If aliases Then Stb.AppendLine("        AS Località")

        Stb.AppendLine("        , coalesce( iCom.Provincia, 'Non Specificata') ")
        If aliases Then Stb.AppendLine("         AS Provincia")

    End Sub

    Private Shared Sub specieVarietàLocalitàQryOLD(Stb As StringBuilder, ByVal aliases As Boolean)

        Stb.AppendLine(" 	 , veg.Veg_Des ")
        If aliases Then Stb.AppendLine(" as Specie")

        Stb.AppendLine(" 	 , veg.Veg_Des + ' - ' + cul.Cul_Des ")
        If aliases Then Stb.AppendLine(" as Varietà")

        Stb.AppendLine(" 	 , coalesce( iCom.Località, 'Non Specificata') ")
        If aliases Then Stb.AppendLine(" as Località")

        Stb.AppendLine(" 	 , coalesce( iCom.Provincia, 'Non Specificata') ")
        If aliases Then Stb.AppendLine(" as Provincia")

    End Sub

    Private Shared Sub appezzaDescrizioniQry(Stb As StringBuilder)


        Stb.AppendLine("        , reg.piva + ' - ' + ")
        Stb.AppendLine("        CAST(reg.sa_cod AS varchar(100)) + ' - ' + ")
        Stb.AppendLine("        CAST(reg.appezza AS varchar(100)) + ' - ' + ")
        Stb.AppendLine("        CAST(reg.id_reg AS varchar(100)) + ' - ' + ")


        Stb.AppendLine("        sa.sa_nome + ' - ' +  ")
        Stb.AppendLine("        app.APP_NOME + ' - ' +  ")
        Stb.AppendLine("        veg.Veg_Des + ' - ' +   ")
        Stb.AppendLine("        cul.Cul_Des ")

        'infine l'alias.
        Stb.AppendLine("        AS Appezzamento")
    End Sub

    Private Shared Sub appezzaQry(Stb As StringBuilder)


        Stb.AppendLine("            reg.piva ")
        Stb.AppendLine("            , reg.sa_cod ")
        Stb.AppendLine("            , reg.appezza ")
        Stb.AppendLine("            , reg.id_reg ")



    End Sub


    Private Shared Sub appezzaQryOLD(Stb As StringBuilder, ByVal filtroImpiantiAttivo As Boolean, ByVal aliases As Boolean)

        If filtroImpiantiAttivo Then
            Stb.AppendLine(" 	   sa.piva + ' - ' +  ")
        End If

        Stb.AppendLine(" 	   sa.sa_nome + ' - ' +  ")
        Stb.AppendLine(" 	   app.APP_NOME + ' - ' +  ")
        Stb.AppendLine(" 	   veg.Veg_Des + ' - ' +   ")
        Stb.AppendLine(" 	   cul.Cul_Des ")

        If aliases Then Stb.AppendLine(" as Appezzamento")
    End Sub

    Private Shared Sub Leggi_CreaTabelleTemp_agenda(Stb As StringBuilder)
        'Stb.AppendLine("create table #__Tmp_Agenda ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	id_agenda int ,  ")
        'Stb.AppendLine(" 	lav_cod int   ")
        'Stb.AppendLine(" )  ")
        'Stb.AppendLine("")
        'Stb.AppendLine(" create table #__Tmp_Movimenti ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	id_agenda int,  ")
        'Stb.AppendLine(" 	id_mov int,  ")
        'Stb.AppendLine(" 	cau_mov varchar(100) collate SQL_Latin1_General_CP850_CI_AS NULL, ")
        'Stb.AppendLine(" 	Num_Protocollo int, ")
        'Stb.AppendLine(" 	Data_Movimento datetime ")
        'Stb.AppendLine(" )  ")
        'Stb.AppendLine("")
        'Stb.AppendLine(" create table #__Tmp_Movimenti_dettagli ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	id_agenda int,  ")
        'Stb.AppendLine(" 	id_mov int,  ")
        'Stb.AppendLine(" 	id_mov_det int, ")
        'Stb.AppendLine(" 	elem_cod int, ")
        'Stb.AppendLine(" 	pro_cod int, ")
        'Stb.AppendLine(" 	PrincipiAttivi varchar(100) collate SQL_Latin1_General_CP850_CI_AS NULL, ")
        'Stb.AppendLine(" 	PrincipiAttiviPesi varchar(100) collate SQL_Latin1_General_CP850_CI_AS NULL ")
        'Stb.AppendLine(" )  ")
        'Stb.AppendLine("")
        'Stb.AppendLine("")
        'Stb.AppendLine(" create table #__Tmp_Mov_dettaglioTecnico ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	id_agenda int,  ")
        'Stb.AppendLine(" 	id_mov int ,  ")
        'Stb.AppendLine(" 	id_mov_det int,  ")
        'Stb.AppendLine(" 	N real,  ")
        'Stb.AppendLine(" 	P real,  ")
        'Stb.AppendLine(" 	K real,  ")
        'Stb.AppendLine(" 	Cu real,   ")
        'Stb.AppendLine(" 	Efficienza real   ")
        'Stb.AppendLine(" )  ")
        'Stb.AppendLine("")
        'Stb.AppendLine(" create table #__Tmp_Movimenti_Destinazioni ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	id_agenda int,  ")
        'Stb.AppendLine(" 	id_mov int ,  ")
        'Stb.AppendLine(" 	id_mov_det int,  ")
        'Stb.AppendLine(" 	sa_cod int,  ")
        'Stb.AppendLine(" 	appezza int,  ")
        'Stb.AppendLine(" 	id_destinazione int,  ")
        'Stb.AppendLine(" 	tipo_destinazione int,  ")
        'Stb.AppendLine(" 	qta real,  ")
        'Stb.AppendLine(" 	qta2 real  ")
        'Stb.AppendLine(" )  ")
        'Stb.AppendLine(" create table #__Tmp_Movimenti_Destinazioni_DateDistinta ( ")
        'Stb.AppendLine(" 	piva varchar(50) collate SQL_Latin1_General_CP850_CI_AS NULL,  ")
        'Stb.AppendLine(" 	sa_cod int,  ")
        'Stb.AppendLine(" 	appezza int,  ")
        'Stb.AppendLine(" 	id_reg int,  ")
        'Stb.AppendLine(" 	Progetto_cod int,  ")
        'Stb.AppendLine(" 	Validita_Inizio datetime,  ")
        'Stb.AppendLine(" 	Validita_Fine datetime  ")
        'Stb.AppendLine(" )  ")
    End Sub

    Private Function descrizioneOperazioneQRYOLD() As String

        Return " convert(varchar(100),  AGG.Data_Movimento, 103)  "

    End Function

    Private Function descrizioneOperazioneQRY() As String

        Return " AGG.Data_Movimento "

    End Function

    Public Sub leggiPopolaTabellaFiltroImpianti(operazioneCorrente_FiltroImpianti As String, ByVal objParametri As AgronicaCoreParametri)

        Dim vQry As String() = operazioneCorrente_FiltroImpianti.Split("|")

        For Each stmt In vQry

            Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, stmt, "leggiPopolaTabellaFiltroImpianti")

        Next

    End Sub

    Private Sub leggiPopolaTabellaFiltroAgenda(operazioneCorrente_FiltroAgenda As String, ByVal objParametri As AgronicaCoreParametri)

        Dim vQry As String() = operazioneCorrente_FiltroAgenda.Split("|")

        For Each stmt In vQry

            Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, stmt, "leggiPopolaTabellaFiltroAgenda")

        Next

    End Sub

    Private Sub leggiPopolaTabellaFormulatiPA(ByVal principiAttivi As String, ByVal IDTestataTemp As Integer, ByVal OperazioneCorrente_Fr_Cod As String, ByVal OperazioneCorrente_PrincipiAttivi As String, ByVal OperazioneCorrente_PrincipiAttiviPesi As String, ByVal DT_FormulatiPA As DataTable, ByRef objParametri As AgronicaCoreParametri)

        Dim stb As New StringBuilder


        ' VAnni: 25/2/2019: nel caso di op.corrente, inserisco in tabella i soli prodotti il cui principio attivo si trova anche nell'operazione corrente
        Dim vOpCorrenteRicercaPA As String() = {}

        If OperazioneCorrente_PrincipiAttivi <> "" And OperazioneCorrente_PrincipiAttivi <> "?" Then
            vOpCorrenteRicercaPA =
                    OperazioneCorrente_PrincipiAttivi.Split("?")


            For Each dd In DT_FormulatiPA.Rows

                Dim PP As String = dd("PrincipiAttivi")
                Dim fr_cod As String = dd("pro_cod")
                Dim PPPesi As String = dd("PrincipiAttiviPesi")

                Dim ppTrovato As Boolean = False
                For Each vPP As String In vOpCorrenteRicercaPA

                    Dim doveCercare As String() = vPP.Split("§")

                    For Each pp2 As String In PP.Split("?")
                        Dim cosaCercare As String() = pp2.Split("§")

                        If doveCercare(0) = cosaCercare(0) Then
                            ppTrovato = True
                        End If

                    Next 'Principio da tabella

                    'se il principio attivo corrente è stato trovato
                    If ppTrovato Then
                        leggiPopolaTabellaFormulatiPA_Insert(principiAttivi, IDTestataTemp, objParametri, stb, PP, PPPesi, fr_cod)
                    End If

                Next 'Principio da OP Corrente

            Next 'Prossimo elemento in tabella

        Else

            For Each dd In DT_FormulatiPA.Rows

                Dim PP As String = dd("PrincipiAttivi")
                Dim fr_cod As String = dd("pro_cod")
                Dim PPPesi As String = dd("PrincipiAttiviPesi")

                leggiPopolaTabellaFormulatiPA_Insert(principiAttivi, IDTestataTemp, objParametri, stb, PP, PPPesi, fr_cod)
            Next


        End If


        'la tabella non contiene l'operazione corrente, se questa non è stata ancora memorizzata, quindi si va a riempire
        ' la tabella temporanea con questi dati ... 
        If OperazioneCorrente_PrincipiAttivi <> "" Then

            Dim vOperazioneCorrente_Fr_Cod() As String = OperazioneCorrente_Fr_Cod.Split(",")
            Dim vOperazioneCorrente_PA() As String = OperazioneCorrente_PrincipiAttivi.Split("?")
            Dim vOperazioneCorrente_PAPesi() As String = OperazioneCorrente_PrincipiAttiviPesi.Split("?")

            For i = 0 To vOperazioneCorrente_Fr_Cod.Length - 1

                leggiPopolaTabellaFormulatiPA_Insert(principiAttivi, IDTestataTemp, objParametri, stb, vOperazioneCorrente_PA(i), vOperazioneCorrente_PAPesi(i), vOperazioneCorrente_Fr_Cod(i))

            Next

        End If


    End Sub

    Private Sub leggiPopolaTabellaFormulatiPA_Insert(ByVal principiAttivi As String, ByVal IDTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri, ByRef stb As StringBuilder, ByVal sPrincipiAttivi As String, ByVal sPrincipiAttiviPesi As String, ByVal fr_cod As Integer)

        If sPrincipiAttivi = "" Then
            Exit Sub
        End If

        Dim vSplitAP As String() = sPrincipiAttivi.Split("|")
        Dim vSplitAPP As String() = sPrincipiAttiviPesi.Split("|")

        Dim vPrincipiAttivi As String() = principiAttivi.Split(",")

        Dim p As Integer = 0

        For Each sSplitAP In vSplitAP

            Dim vPaTitolo As String() = sSplitAP.Split("§")

            Dim vPaPeso As String()
            If p < vSplitAPP.Count() Then
                vPaPeso = vSplitAPP(p).Split("§")
            Else 'ci possono essere dei casi dove ci sono più elementi in PrincipiAttivi rispetto a PrincipiAttiviPesi, esempio PrincipiAttivi = 883§15.5|333§3|368§0, PrincipiAttiviPesi = 883§0|333§0
                vPaPeso = {"0", "0"} 'per gestire questi casi imposto il peso uguale a 0 (ticket 216811)
            End If

            If principiAttivi = "" OrElse vPrincipiAttivi.Contains(vPaTitolo(0)) Then

                stb.Length = 0
                stb.AppendLine(" if not exists ( select  1 from  __tmp_FormulatiXPrincipiAttivi where IDTestataTemp = " & IDTestataTemp & " and  pa_cod = " & vPaTitolo(0) & " and  fr_cod = " & fr_cod & " )")
                stb.AppendLine(" insert __tmp_FormulatiXPrincipiAttivi(IDTestataTemp, pa_cod, fr_cod, titolo, peso) ")
                stb.AppendLine(" values (" & IDTestataTemp & ", " & vPaTitolo(0) & "," & fr_cod & "," & vPaTitolo(1).Replace(",", ".") & "," & vPaPeso(1).Replace(",", ".") & ")")

                '--------------------------------------------------------------------------
                Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, stb.ToString, "leggiPopolaTabellaFormulatiPA")
                '--------------------------------------------------------------------------

            End If

            p += 1

        Next

    End Sub

    Private Function leggiPopolaTabellaFormulatiPA_LeggiDT(
        ByVal SostenibilitaTipoAnalisi As enum_SostenibilitaTipoAnalisi,
        ByVal IDTestataTemp As Integer,
        ByVal piva As String,
        ByVal filtroImpianti As String,
        ByVal filtroOperazioni As String,
        ByVal veg_cod As Integer,
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByRef objParametri As AgronicaCoreParametri,
        ByRef FiltroAgenda As Boolean
    ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("  Select distinct det.piva, det.sa_cod, det.id_mov, det.id_Mov_det, det.pro_cod, det.PrincipiAttivi, det.PrincipiAttiviPesi ")
            Stb.AppendLine(" from movimenti_dettagli det ")


            Stb.AppendLine("  inner join movimenti m ")
            Stb.AppendLine("        On det.piva = m.piva ")
            Stb.AppendLine("       And det.id_agenda = m.id_agenda  ")
            Stb.AppendLine("       And det.id_mov = m.id_mov ")

            If filtroImpianti <> "" Or SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then

                Stb.AppendLine(" inner join  mov_destinazioni dest ")
                Stb.AppendLine(" 	On det.id_agenda = dest.id_agenda ")
                Stb.AppendLine(" 	And det.id_mov_det = dest.id_mov_det ")
                Stb.AppendLine(" 	And det.id_mov = dest.id_mov ")
                Stb.AppendLine(" 	And dest.tipo_destinazione = 0 ")
            End If



            If filtroImpianti <> "" Then

                Stb.AppendLine(" inner join  __tmp_FiltroImpianti reg ")
                Stb.AppendLine(" 	On reg.piva = dest.piva ")
                Stb.AppendLine(" 	And reg.sa_cod = dest.sa_cod ")
                Stb.AppendLine(" 	And reg.appezza = dest.appezza ")
                Stb.AppendLine(" 	And reg.id_reg = dest.id_destinazione ")
                Stb.AppendLine(" 	And reg.IDTestataTemp = " & IDTestataTemp)

            ElseIf filtroOperazioni <> "" Then

                Stb.AppendLine(" inner join  __Tmp_Agenda filtroAgn ")
                Stb.AppendLine(" 	On det.piva = filtroAgn.piva ")
                Stb.AppendLine(" 	And det.ID_Agenda = filtroAgn.ID_Agenda ")
                Stb.AppendLine(" 	And filtroAgn.IDTestataTemp = " & IDTestataTemp)

            Else

                If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti And Not FiltroAgenda Then

                    Stb.AppendLine("inner join __Tmp_Movimenti_Destinazioni_DateDistinta ddd ")
                    Stb.AppendLine(" 	on ddd.piva = dest.piva  ")
                    Stb.AppendLine(" 	and ddd.sa_cod = dest.sa_cod  ")
                    Stb.AppendLine(" 	and ddd.appezza = dest.appezza ")
                    Stb.AppendLine(" 	and ddd.id_reg = dest.id_destinazione ")
                    Stb.AppendLine(" 	and ddd.idTestataTemp =  " & IDTestataTemp)
                    Stb.AppendLine(" 	and m.data_movimento >= ddd.Validita_Inizio  ")
                    Stb.AppendLine(" 	and m.data_movimento <= ddd.Validita_Fine")

                End If

            End If



            Stb.AppendLine("    inner join Agenda A ")
            Stb.AppendLine("      On A.piva = m.piva ")
            Stb.AppendLine("        And A.id_Agenda = m.id_agenda ")

            Stb.AppendLine("          where A.lav_cod In ( 74, 103, 18, 155, 13, 158)  ")

            ' VAnni: 26/2/2019: commento per avere la garanzia di leggere tutti i principi attivi quando esamino una singola operazione.
            '      Anche se computazionalmente più complesso ho così la certezza di avere una decodifica.
            '      se invece non viene passato il filtro impianti allora un filtro per data serve, altrimenti leggerebbe tutta l'agenda.
            '      la variabile filtroImpianti è valorizzata in due casi: analisi singola operazione oppure quando si imposta un filtro da apposito pulsante.
            If filtroImpianti = "" Then
                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti Then
                    Stb.AppendLine("          AND m.Data_Movimento Between " & Agro_SQL_SaveDate(DataDa) & " AND " & Agro_SQL_SaveDate(DataA) & "  ")
                    Stb.AppendLine("          And A.piva = '" & Agro_SQL_SaveText(piva) & "' ")
                End If


            End If


            Stb.AppendLine("          AND m.Cau_Mov IN ('2050')   ")


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

    Public Sub LeggiAgenda(ByVal SostenibilitaTipoAnalisi As enum_SostenibilitaTipoAnalisi, ByVal IDTestataTemp As Integer, ByVal chiamataDaOperazioneAgenda As Boolean, ByVal filtroImpianti As Boolean, ByVal filtroAgenda As Boolean, ByVal Tipo As Int16, ByRef stb As StringBuilder)



        stb.AppendLine("        FROM  ( ")

        LeggiAgenda_QryAgendaInnestata(SostenibilitaTipoAnalisi, IDTestataTemp, chiamataDaOperazioneAgenda, Tipo, stb, False, filtroAgenda)

        If chiamataDaOperazioneAgenda Then
            stb.AppendLine("")
            stb.AppendLine("        UNION ALL ")
            stb.AppendLine("")
            LeggiAgenda_QryAgendaInnestata(SostenibilitaTipoAnalisi, IDTestataTemp, chiamataDaOperazioneAgenda, Tipo, stb, True, False)

        End If

        stb.AppendLine("        ) AGG ")
        stb.AppendLine("")

        Select Case Tipo
            Case -3

                stb.AppendLine("        INNER JOIN fertilizzanti fer  ")
                stb.AppendLine("            ON fer.fer_Cod = AGG.pro_cod  ")

            Case 3

                stb.AppendLine("        INNER JOIN fertilizzanti fer  ")
                stb.AppendLine("            ON fer.fer_Cod = AGG.pro_cod  ")


            Case 1
                stb.AppendLine("        INNER JOIN Formulati fr  ")
                stb.AppendLine("            ON AGG.Pro_Cod = fr.Fr_Cod           ")

                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then

                    stb.AppendLine("        INNER JOIN __tmp_FormulatiXPrincipiAttivi frPa  ")
                    stb.AppendLine("            ON frPa.Fr_Cod = fr.Fr_Cod  ")
                    stb.AppendLine("            AND frPa.IDTestataTemp = " & IDTestataTemp)
                    stb.AppendLine("        INNER JOIN PrincipiAttivi pa  ")
                    stb.AppendLine("            ON pa.pa_cod = frPa.pa_cod ")
                End If



            Case -1
                stb.AppendLine("        INNER JOIN Formulati fr  ")
                stb.AppendLine("            ON AGG.Pro_Cod = fr.Fr_Cod           ")

                If SostenibilitaTipoAnalisi <> enum_SostenibilitaTipoAnalisi.AnalisiPerProdottoCommerciale Then

                    stb.AppendLine("        INNER JOIN __tmp_FormulatiXPrincipiAttivi frPa  ")
                    stb.AppendLine("            ON frPa.Fr_Cod = fr.Fr_Cod  ")
                    stb.AppendLine("            AND frPa.IDTestataTemp = " & IDTestataTemp)
                    stb.AppendLine("")
                    stb.AppendLine("        INNER JOIN PrincipiAttivi pa  ")
                    stb.AppendLine("            ON pa.pa_cod = frPa.pa_cod ")
                End If




        End Select

        'stb.AppendLine("              INNER JOIN Centri_Aziendali sa  ")
        'stb.AppendLine("                ON sa.piva = AGG.piva  ")
        'stb.AppendLine("                  AND sa.sa_cod = AGG.sa_cod  ")
        'Stb.AppendLine("")

        stb.AppendLine("        INNER JOIN reg_impianti reg  ")
        stb.AppendLine("            ON reg.piva = AGG.Piva  ")
        stb.AppendLine("            AND reg.sa_cod = AGG.sa_cod  ")
        stb.AppendLine("            AND reg.appezza = AGG.appezza  ")
        stb.AppendLine("            AND reg.id_reg = AGG.Id_Destinazione  ")
        stb.AppendLine("")

        If filtroImpianti Then
            stb.AppendLine("        INNER JOIN __tmp_FiltroImpianti regFiltro  ")
            stb.AppendLine("            ON reg.piva = regFiltro.Piva  ")
            stb.AppendLine("            AND reg.sa_cod = regFiltro.sa_cod  ")
            stb.AppendLine("            AND reg.appezza = regFiltro.appezza  ")
            stb.AppendLine("            AND reg.id_reg = regFiltro.id_reg  ")
            stb.AppendLine("            AND regFiltro.IDTestataTemp = " & IDTestataTemp)
            stb.AppendLine("")
        End If

        If filtroAgenda Then
            stb.AppendLine("        INNER JOIN __tmp_Agenda agnFiltro  ")
            stb.AppendLine("            ON AGG.piva = agnFiltro.Piva  ")
            stb.AppendLine("            AND AGG.id_agenda = agnFiltro.id_agenda  ")
            stb.AppendLine("            AND agnFiltro.IDTestataTemp = " & IDTestataTemp)
            stb.AppendLine("")
        End If

        'stb.AppendLine(" left join ( ")

        'stb.AppendLine(" select   ")
        'stb.AppendLine("         ci.piva  ")
        'stb.AppendLine("     , ci.sa_cod  ")
        'stb.AppendLine("     , icom.LOCALITA as Località  ")
        'stb.AppendLine("     , p.Provincia  ")
        'stb.AppendLine(" from centriXindirizzi ci  ")
        'stb.AppendLine("     INNER JOIN Indirizzi ii  ")
        'stb.AppendLine("         ON ci.cod_indirizzo = ii.cod_indirizzo  ")
        'stb.AppendLine("     INNER JOIN istat iCom  ")
        'stb.AppendLine("         ON iCom.prov = ii.pro_cod_istat  ")
        'stb.AppendLine("         AND iCom.com = ii.com_cod_istat  ")
        'stb.AppendLine("     INNER JOIN Lista_Province p  ")
        'stb.AppendLine("         ON p.PROV = iCom.PROV ")
        'stb.AppendLine(" ) iCom")

        'stb.AppendLine(" ON iCom.Piva = sa.Piva ")
        'stb.AppendLine(" AND iCom.Sa_Cod = sa.Sa_cod")


        'stb.AppendLine("              left join reg_impianti_codici Massimale_N  ")
        'stb.AppendLine("                ON Massimale_N.piva = AGG.Piva  ")
        'stb.AppendLine("                  AND Massimale_N.sa_cod = AGG.sa_cod  ")
        'stb.AppendLine("                  AND Massimale_N.appezza = AGG.appezza  ")
        'stb.AppendLine("                  AND Massimale_N.id_reg = AGG.Id_Destinazione  ")
        'stb.AppendLine("                  AND Massimale_N.id_cod = 1050  ")

        'stb.AppendLine("              left join reg_impianti_codici Massimale_P  ")
        'stb.AppendLine("                ON Massimale_P.piva = AGG.Piva  ")
        'stb.AppendLine("                  AND Massimale_P.sa_cod = AGG.sa_cod  ")
        'stb.AppendLine("                  AND Massimale_P.appezza = AGG.appezza  ")
        'stb.AppendLine("                  AND Massimale_P.id_reg = AGG.Id_Destinazione  ")
        'stb.AppendLine("                  AND Massimale_P.id_cod = 1051  ")

        'stb.AppendLine("              left join reg_impianti_codici Massimale_K  ")
        'stb.AppendLine("                ON Massimale_K.piva = AGG.Piva  ")
        'stb.AppendLine("                  AND Massimale_K.sa_cod = AGG.sa_cod  ")
        'stb.AppendLine("                  AND Massimale_K.appezza = AGG.appezza  ")
        'stb.AppendLine("                  AND Massimale_K.id_reg = AGG.Id_Destinazione  ")
        'stb.AppendLine("                  AND Massimale_K.id_cod = 1052  ")

        'Stb.AppendLine("")
        'stb.AppendLine("              INNER JOIN Appezzamento app  ")
        'stb.AppendLine("                ON reg.piva = app.Piva  ")
        'stb.AppendLine("                  AND reg.sa_cod = app.sa_cod  ")
        'stb.AppendLine("                AND reg.appezza = app.appezza  ")
        'Stb.AppendLine("")

        'terreno nudo .. ?
        'stb.AppendLine("            INNER JOIN Cultivar cul  ")
        'stb.AppendLine("                  ON cul.cul_cod = reg.cul_cod  ")
        'Stb.AppendLine("")
        'stb.AppendLine("              INNER JOIN SpecieVegetali veg  ")
        'stb.AppendLine("                ON veg.veg_cod = cul.veg_cod  ")
        'stb.AppendLine(" ")

    End Sub

    Private Shared Sub LeggiAgenda_QryAgendaInnestata(
        ByVal SostenibilitaTipoAnalisi As enum_SostenibilitaTipoAnalisi,
        ByVal IDTestataTemp As Integer,
        ByVal chiamataDaOperazioneAgenda As Boolean,
        Tipo As Short,
        stb As StringBuilder,
        ByVal TabelleTemp As Boolean,
        FiltroAgenda As Boolean
    )

        Dim tAgenda As String = "Agenda"
        Dim tMovimenti As String = "Movimenti"
        Dim tMovimentiDettagli As String = "movimenti_Dettagli"
        Dim tTecnico As String = "Mov_Dettaglio_Tecnico"
        Dim tDestinazioni As String = "mov_destinazioni"
        Dim selectOpCorrente As String = " 'Altre' "

        If TabelleTemp Then
            tAgenda = "__Tmp_Agenda"
            tMovimenti = "__Tmp_Movimenti"
            tMovimentiDettagli = "__Tmp_Movimenti_dettagli"
            tTecnico = "__Tmp_Mov_dettaglioTecnico"
            tDestinazioni = "__Tmp_Movimenti_Destinazioni"
            selectOpCorrente = " 'Questa' "
        End If

        stb.AppendLine("            SELECT ")

        'Agenda
        stb.AppendLine("                a.piva AS piva,  ")
        stb.AppendLine("                a.id_agenda,  ")
        stb.AppendLine("                a.Raccoglitore_Cod,  -- 25/05/23 GESTIONE MULTI ATTIVITA  ")
        stb.AppendLine("                a.lav_cod, ")

        'Movimenti
        stb.AppendLine("                m.id_mov ,  ")
        stb.AppendLine("                m.cau_mov AS cau_mov, ")
        stb.AppendLine("                m.Data_Movimento,  ")
        stb.AppendLine("                CASE WHEN m.data_movimento < '01/02/2019' THEN 6 ELSE 4 END AS rame, ")
        stb.AppendLine("                m.Num_Protocollo,  ")

        'Dettagli        
        stb.AppendLine("                d.id_mov_det, ")
        stb.AppendLine("                d.elem_cod, ")
        stb.AppendLine("                d.pro_cod, ")
        stb.AppendLine("                d.PrincipiAttivi AS PrincipiAttivi, ")

        'Destinazioni
        stb.AppendLine("                destImp.sa_cod,  ")
        stb.AppendLine("                destImp.appezza,  ")
        stb.AppendLine("                destImp.id_destinazione,  ")
        stb.AppendLine("                destImp.tipo_destinazione,  ")
        stb.AppendLine("                destImp.qta,  ")
        stb.AppendLine("                destImp.qta2, ")
        stb.AppendLine("                " & selectOpCorrente & " AS OpCorrente  ")

        Select Case Tipo
            Case 3
                stb.AppendLine("                , (tecN.N * isnull(tecN.Efficienza,1)) AS tecN_N ,  ")
                stb.AppendLine("                tecP.P AS tecP_P,  ")
                stb.AppendLine("                tecK.K AS tecK_K ,  ")
                stb.AppendLine("                tecCu.Cu AS TecCU_CU ")
        End Select

        stb.AppendLine("            FROM " & tDestinazioni & " destImp  ")

        stb.AppendLine("            INNER JOIN " & tMovimenti & " m  ")
        stb.AppendLine("                ON destImp.id_Agenda = m.id_Agenda    ")
        stb.AppendLine("                AND destImp.id_mov = m.id_mov  ")
        stb.AppendLine("                AND destImp.piva = m.piva  ")

        If TabelleTemp Then
            stb.AppendLine("                AND m.IDTestataTemp = destImp.IDTestataTemp")
        End If

        If SostenibilitaTipoAnalisi = enum_SostenibilitaTipoAnalisi.MassimalisuSingoliImpianti And Not TabelleTemp And Not FiltroAgenda Then

            stb.AppendLine("            INNER JOIN __Tmp_Movimenti_Destinazioni_DateDistinta ddd ")
            stb.AppendLine("                ON ddd.piva = destImp.piva  ")
            stb.AppendLine("                AND ddd.sa_cod = destImp.sa_cod  ")
            stb.AppendLine("                AND ddd.appezza = destImp.appezza ")
            stb.AppendLine("                AND ddd.id_reg = destImp.id_destinazione ")
            stb.AppendLine("                AND ddd.idTestataTemp = " & IDTestataTemp)
            stb.AppendLine("                AND m.data_movimento >= ddd.Validita_Inizio  ")
            stb.AppendLine("                AND m.data_movimento <= ddd.Validita_Fine")

        End If

        stb.AppendLine("")
        stb.AppendLine("            INNER JOIN " & tMovimentiDettagli & " d  ")
        stb.AppendLine("                ON destImp.id_agenda = d.id_agenda  ")
        stb.AppendLine("                AND destImp.Id_Mov = d.Id_Mov  ")
        stb.AppendLine("                AND destImp.id_Mov_det = d.id_mov_det  ")
        stb.AppendLine("                AND destImp.PIVA = d.PIVA  ")

        If TabelleTemp Then
            stb.AppendLine("                AND d.IDTestataTemp = destImp.IDTestataTemp")
        End If

        stb.AppendLine("")
        stb.AppendLine("            INNER JOIN " & tAgenda & " a ")
        stb.AppendLine("                ON a.id_Agenda = destImp.id_Agenda ")
        stb.AppendLine("                AND a.piva = destImp.piva  ")

        If TabelleTemp Then
            stb.AppendLine("                AND a.IDTestataTemp = destImp.IDTestataTemp")
        End If

        If Tipo = 3 Then
            stb.AppendLine("            INNER JOIN " & tTecnico & " tecN  ")
            stb.AppendLine("                ON tecN.Id_Mov_Det = d.Id_Mov_Det  ")
            stb.AppendLine("                AND tecN.id_agenda = d.id_agenda ")
            stb.AppendLine("                AND tecN.Id_Mov = d.Id_Mov ")
            stb.AppendLine("                AND tecN.Id_Mov_Det = d.Id_Mov_Det ")
            If TabelleTemp Then
                stb.AppendLine("                AND tecN.IDTestataTemp = d.IDTestataTemp")
            End If

            stb.AppendLine("            INNER JOIN " & tTecnico & " tecP  ")
            stb.AppendLine("                ON tecP.Id_Mov_Det = d.Id_Mov_Det  ")
            stb.AppendLine("                AND tecP.id_agenda = d.id_agenda ")
            stb.AppendLine("                AND tecP.Id_Mov = d.Id_Mov ")
            stb.AppendLine("                AND tecP.Id_Mov_Det = d.Id_Mov_Det ")
            If TabelleTemp Then
                stb.AppendLine("                AND tecP.IDTestataTemp = d.IDTestataTemp")
            End If

            stb.AppendLine("            INNER JOIN " & tTecnico & " tecK  ")
            stb.AppendLine("                ON tecK.Id_Mov_Det = d.Id_Mov_Det  ")
            stb.AppendLine("                AND tecK.id_agenda = d.id_agenda ")
            stb.AppendLine("                AND tecK.Id_Mov = d.Id_Mov ")
            stb.AppendLine("                AND tecK.Id_Mov_Det = d.Id_Mov_Det ")
            If TabelleTemp Then
                stb.AppendLine("                AND tecK.IDTestataTemp = d.IDTestataTemp")
            End If

            stb.AppendLine("            INNER JOIN " & tTecnico & " tecCu  ")
            stb.AppendLine("                ON tecCu.Id_Mov_Det = d.Id_Mov_Det  ")
            stb.AppendLine("                AND tecCu.id_agenda = d.id_agenda ")
            stb.AppendLine("                AND tecCu.Id_Mov = d.Id_Mov ")
            stb.AppendLine("                AND tecCu.Id_Mov_Det = d.Id_Mov_Det ")
            If TabelleTemp Then
                stb.AppendLine("                AND tecCU.IDTestataTemp = d.IDTestataTemp")
            End If

        End If


        stb.AppendLine("            WHERE destImp.tipo_destinazione = 0 ")

        If TabelleTemp Then
            stb.AppendLine("            AND a.IDTestataTemp = " & IDTestataTemp)
        End If

    End Sub


End Class

