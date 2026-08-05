Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Bilancio_R
    Inherits DataProvider

    Public Enum BilancioDiMassa_TipoIntervalliLettura
        Nessuno = 0
        Trimestri = 1
    End Enum

    ''' <summary>
    ''' Report sul bilancio di massa per azienda (o tutte le aziende) sui moviemnti dei lavorati vegetali: somma le quantità dei prodotti per quarto dell'anno, specie e varietà dei prodotti vegetali, in un periodo di tempo indicato.
    ''' Suddivide le quantità in colonna per giacenze/stock iniziali e finali, quantità in ingresso ed in uscita, nel periodo di tempo indicato.
    ''' Se specificato un parametro qualitativo, applica una ulteriore suddivisione per ciascuna delle quattro colonne sopra indicate, per tutti i valori che il parametro può assumere (o solo per i valori specificati).
    ''' </summary>
    ''' <param name="piva">PIVA dell'azienda per la quale considerare movimenti e calcolare il bilancio di massa, se vuoto viene effettuato il bilancio di massa per tutte le imprese</param>
    ''' <param name="data_inizio">Data inizio del periodo da considerare per il report</param>
    ''' <param name="data_fine">Data fine del periodo da considerare per il report</param>
    ''' <param name="tipoIntervalliLettura">Tipo di intervallo di tempo per il quale raggrupare le operazioni sui prodotti, da riportare in report bilancio</param>
    ''' <param name="parametroQualitativo">Nome del parametro qualitativo (campo Tipo in tabella Materie_Prime_Campionature)</param>
    ''' <param name="parametroQualitativoCod">Codice relativo al parametro qualitativo (così come salvato per il parametro in OTabelle), se 0 viene ricavato</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="ObjParametri_Server"></param>
    ''' <param name="leggiTuttiValoriParametro">Applica suddivisione per valori parametro qualitativo e considera tutti i suoi possibili valori</param>
    ''' <param name="valoriParametrDaLeggere_Cod">Se "leggiTuttiValoriParametro" è False, applica suddivisione colonne per parametro qualitativo e considera solo valori indicati (elencati per codice)</param>
    ''' <param name="valoriParametrDaLeggere_Desc">Se "leggiTuttiValoriParametro" è False, applica suddivisione colonne per parametro qualitativo e considera solo valori indicati (elencati per descrizione)</param>
    ''' <param name="InitialStockColumnPrefix">Prefisso colonne relative a quantità/stock di prodotti iniziale</param>
    ''' <param name="FinalStockColumnPrefix">Prefisso colonne relative a quantità/stock di prodotti finale</param>
    ''' <param name="InQtaColumnPrefix">Prefisso colonne relative a quantità di prodotti in entrata</param>
    ''' <param name="OutQtaColumnPrefix">Prefisso colonne relative a quantità di prodotti in uscita</param>
    ''' <param name="EmptyParamValueColumnSuffix">Suffisso colonne per quantità senza un valore, per il parametro qualitativo selezionato, impostato (Tipo_Cod = 0 o il valore su OTabelle_Parametri con descrizione vuota)</param>
    ''' <returns></returns>
    Public Function LeggiBilancioDiMassa(ByVal piva As String,
                                         ByVal data_inizio As Date,
                                         ByVal data_fine As Date,
                                         ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura,
                                         ByVal parametroQualitativo As String,
                                         ByVal parametroQualitativoCod As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef ObjParametri_Server As AgronicaCoreParametri,
                                         Optional ByVal leggiTuttiValoriParametro As Boolean = True,
                                         Optional ByVal valoriParametrDaLeggere_Cod As Integer() = Nothing,
                                         Optional ByVal valoriParametrDaLeggere_Desc As String() = Nothing,
                                         Optional ByVal InitialStockColumnPrefix As String = "InitialStock",
                                         Optional ByVal FinalStockColumnPrefix As String = "FinalStock",
                                         Optional ByVal InQtaColumnPrefix As String = "InQta",
                                         Optional ByVal OutQtaColumnPrefix As String = "OutQta",
                                         Optional ByVal EmptyParamValueColumnSuffix As String = "null") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Bilancio.LeggiBilancioDiMassa"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            Dim arrCauMov As Integer() = New Integer() {
                CAU_CONFERIMENTO,
                CAU_CONFERIMENTO_DIVERSI,
                CAU_CARICO,
                CAU_SCARICO,
                CAU_ACCETTAZIONE_BENI,
                CAU_ACCETTAZIONE_BENI_DA_DIVERSI
            }

            ' Causali di movimento per calcolo somma entrate a magazzino, in un determinato intervallo di tempo
            Dim arrCauMovCarichi As Integer() = New Integer() {
                CAU_CONFERIMENTO,
                CAU_CARICO,
                CAU_ACCETTAZIONE_BENI_DA_DIVERSI
            }

            ' Causali di movimento per calcolo somma uscite da magazzino, in un determinato intervallo di tempo
            Dim arrCauMovScarichi As Integer() = New Integer() {
                CAU_CONFERIMENTO_DIVERSI,
                CAU_SCARICO,
                CAU_ACCETTAZIONE_BENI
            }

            Dim arrLavCodDaEscluderePerInOut As Integer() = New Integer() {
                LAVCOD_TRASFERIMENTO,
                LAVCOD_TRASFORMAZIONI
            }

            Dim strCauMov As String = String.Join(", ", arrCauMov)
            Dim strCauMovCarichi As String = String.Join(", ", arrCauMovCarichi)
            Dim strCauMovScarichi As String = String.Join(", ", arrCauMovScarichi)
            Dim strLavCodDaEscluderePerInOut As String = String.Join(", ", arrLavCodDaEscluderePerInOut)

            Dim StrCTEs As New StringBuilder
            Dim StrSelectCase_Initial As New StringBuilder
            Dim StrSelectCase_In As New StringBuilder
            Dim StrSelectCase_Out As New StringBuilder
            Dim StrSelectCase_Final As New StringBuilder
            Dim StrJoinCTE As New StringBuilder
            Dim StrNotNullWhereClause As New StringBuilder

            Dim StrPeriodClause As String = ""
            Select Case tipoIntervalliLettura
                Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                    StrPeriodClause = "Anno, Anno_Quarter, "
            End Select

            ' Se almeno uno fra "parametroQualitativo" e "parametroQualitativoCod" è valorizzato, valorizzo anche l'altro 
            Dim dtParametriQual = LeggiParametriQualitativiPerBilancio(piva, parametroQualitativoCod, parametroQualitativo, ObjParametri_Server)

            If String.IsNullOrEmpty(parametroQualitativo) And parametroQualitativoCod <> 0 And dtParametriQual.Rows.Count > 0 Then
                ' Nome parametro qualitativo deve essere come salvato in Materie_Prime_Campionature
                parametroQualitativo = "o" & LCase(dtParametriQual.Rows(0).Item("Tabella_Cod_Des"))
            ElseIf Not String.IsNullOrEmpty(parametroQualitativo) And parametroQualitativoCod = 0 And dtParametriQual.Rows.Count > 0 Then
                parametroQualitativoCod = dtParametriQual.Rows(0).Item("Tabella_Cod")
            End If

            Dim Tabella_Cod_Des As String = ""
            If dtParametriQual.Rows.Count > 0 Then
                Tabella_Cod_Des = dtParametriQual(0).Item("Tabella_Cod_Des")
            End If

            Dim letturaParametroQualitativo As Boolean = False

            ' Capita che su tabella Materie_Prime_Campionature alcuni valori vengano erroneamente salvati con Tipo_Cod = 0, anziché con corrispettivo corretto su tabella OTabelle_Parametri,
            ' quando viene salvato il valore di default con descrizione vuota:
            ' In questa variabile viene salvato il valore corretto atteso ed utilizzato in fase di report. 
            Dim paramTipoCodZeroPlaceholder As Integer = 0

            Dim presentiValoriParametroDaLeggere As Boolean = False
            Dim leggiValoreParametroNonSettato As Boolean = False

            If leggiTuttiValoriParametro Then
                presentiValoriParametroDaLeggere = True
            ElseIf Not IsNothing(valoriParametrDaLeggere_Cod) Then
                If valoriParametrDaLeggere_Cod.Length > 0 Then
                    presentiValoriParametroDaLeggere = True

                    leggiValoreParametroNonSettato = valoriParametrDaLeggere_Cod.Contains(0)
                End If
            ElseIf Not IsNothing(valoriParametrDaLeggere_Desc) Then
                If valoriParametrDaLeggere_Desc.Length > 0 Then
                    presentiValoriParametroDaLeggere = True
                End If
            End If

            ' Se non viene indicato un parametro qualitativo o non sono riportati i valori del parametro da considerare (se non indicato di considerarli tutti)
            ' non si applica la suddivisione per valori di parametro. 
            If Not String.IsNullOrEmpty(parametroQualitativo) And presentiValoriParametroDaLeggere Then

                'If parametroQualitativoCod = 0 Then
                '    Dim dtOTabelleCod = Leggi_Da_OTabelle(0, parametroQualitativo, ObjParametri_Server)

                '    If dtOTabelleCod.Rows.Count > 0 Then
                '        parametroQualitativoCod = dtOTabelleCod.Rows(0).Item("Tabella_Cod")
                '    End If
                'End If

                Dim dtOTabelleParametriCod As DataTable = LeggiValoriParametroQualitativoPerBilancio(piva, parametroQualitativoCod, leggiTuttiValoriParametro, valoriParametrDaLeggere_Cod, valoriParametrDaLeggere_Desc, ObjParametri_Server)

                ' Se viete indicato valore parametro con codice/descrizione nullo è perchè si vogliono leggere la quantità con parametro selezionato non valorizzato,
                ' si aggiunge riga "vuota" se non esiste una con Codice_Origine vuoto 
                If dtOTabelleParametriCod.Select("Codice_Origine = ''").Length = 0 And (leggiValoreParametroNonSettato Or leggiTuttiValoriParametro) Then
                    Dim emptyParamRow = dtOTabelleParametriCod.NewRow()
                    emptyParamRow("Tabella_Cod_Des") = Tabella_Cod_Des
                    emptyParamRow("Tabella_Par_Cod") = 0
                    emptyParamRow("Codice_Origine") = ""
                    emptyParamRow("Descrizione") = ""
                    dtOTabelleParametriCod.Rows.Add(emptyParamRow)
                End If

                If dtOTabelleParametriCod.Rows.Count > 0 Then

                    Dim emptyParamRows = dtOTabelleParametriCod.Select("Codice_Origine = ''")
                    If emptyParamRows.Length > 0 Then
                        paramTipoCodZeroPlaceholder = emptyParamRows(0).Item("Tabella_Par_Cod")
                    End If

                    Dim param_TabellaParCod As Integer
                    Dim param_TabellaCodDes As String
                    Dim param_Codice_Origine As String
                    Dim param_TabellaCodDes_Codice_Origine As String

                    Dim CTE_Name_InitialStock As String
                    Dim CTE_Name_InQta As String
                    Dim CTE_Name_OutQta As String
                    Dim CTE_Name_FinalStock As String

                    Dim QtaColumnName_InitialStock As String
                    Dim QtaColumnName_InQta As String
                    Dim QtaColumnName_OutQta As String
                    Dim QtaColumnName_FinalStock As String

                    For Each paramRow In dtOTabelleParametriCod.Rows

                        param_TabellaCodDes = paramRow("Tabella_Cod_Des")
                        param_TabellaParCod = paramRow("Tabella_Par_Cod")
                        param_Codice_Origine = paramRow("Codice_Origine")

                        If String.IsNullOrEmpty(param_Codice_Origine) And Not String.IsNullOrEmpty(EmptyParamValueColumnSuffix) Then
                            param_Codice_Origine = EmptyParamValueColumnSuffix
                        End If
                        param_TabellaCodDes_Codice_Origine = param_TabellaCodDes & "_" & param_Codice_Origine

                        CTE_Name_InitialStock = "[" & InitialStockColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine & "_CTE" & "]"
                        CTE_Name_InQta = "[" & InQtaColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine & "_CTE" & "]"
                        CTE_Name_OutQta = "[" & OutQtaColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine & "_CTE" & "]"
                        CTE_Name_FinalStock = "[" & FinalStockColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine & "_CTE" & "]"

                        BilancioDiMassa_AppendCTE_ForParam(StrCTEs, CTE_Name_InitialStock, StrPeriodClause, "InitialStock_CTE", param_TabellaParCod)
                        BilancioDiMassa_AppendCTE_ForParam(StrCTEs, CTE_Name_InQta, StrPeriodClause, "InMov_CTE", param_TabellaParCod)
                        BilancioDiMassa_AppendCTE_ForParam(StrCTEs, CTE_Name_OutQta, StrPeriodClause, "OutMov_CTE", param_TabellaParCod)
                        BilancioDiMassa_AppendCTE_ForParam(StrCTEs, CTE_Name_FinalStock, StrPeriodClause, "FinalStock_CTE", param_TabellaParCod)

                        QtaColumnName_InitialStock = InitialStockColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine
                        QtaColumnName_InQta = InQtaColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine
                        QtaColumnName_OutQta = OutQtaColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine
                        QtaColumnName_FinalStock = FinalStockColumnPrefix & "_" & param_TabellaCodDes_Codice_Origine

                        BilancioDiMassa_AppendQtaSelectCaseForParam(StrSelectCase_Initial, CTE_Name_InitialStock, QtaColumnName_InitialStock)
                        BilancioDiMassa_AppendQtaSelectCaseForParam(StrSelectCase_In, CTE_Name_InQta, QtaColumnName_InQta)
                        BilancioDiMassa_AppendQtaSelectCaseForParam(StrSelectCase_Out, CTE_Name_OutQta, QtaColumnName_OutQta)
                        BilancioDiMassa_AppendQtaSelectCaseForParam(StrSelectCase_Final, CTE_Name_FinalStock, QtaColumnName_FinalStock)
                        ' La giacenza finale non è garantito che sia pari alla somma della giacenza iniziale più quantità in entrata e meno quantità in uscita 
                        'BilancioDiMassa_AppendQtaSelectCaseForFinalStockParam(StrSelectCase_Final, CTE_Name_InitialStock, CTE_Name_InQta, CTE_Name_OutQta, QtaColumnName_FinalStock)

                        BilancioDiMassa_AppendJoinForParam(StrJoinCTE, CTE_Name_InitialStock, tipoIntervalliLettura)
                        BilancioDiMassa_AppendJoinForParam(StrJoinCTE, CTE_Name_InQta, tipoIntervalliLettura)
                        BilancioDiMassa_AppendJoinForParam(StrJoinCTE, CTE_Name_OutQta, tipoIntervalliLettura)
                        BilancioDiMassa_AppendJoinForParam(StrJoinCTE, CTE_Name_FinalStock, tipoIntervalliLettura)

                        BilancioDiMassa_AppendNotNullClauseForParam(StrNotNullWhereClause, CTE_Name_InitialStock)
                        BilancioDiMassa_AppendNotNullClauseForParam(StrNotNullWhereClause, CTE_Name_InQta)
                        BilancioDiMassa_AppendNotNullClauseForParam(StrNotNullWhereClause, CTE_Name_OutQta)
                        BilancioDiMassa_AppendNotNullClauseForParam(StrNotNullWhereClause, CTE_Name_FinalStock)

                    Next

                    letturaParametroQualitativo = True
                End If
            End If

            Dim CTE_Iniziale_Campi As String
            Dim CTE_Secondarie_Campi As String

            If letturaParametroQualitativo Then
                CTE_Iniziale_Campi = "Lav_Cod, Cau_Mov, Data_Movimento, " & StrPeriodClause & "Mat_Cod, Veg_Des, Cul_Des, Tipo_Cod, Qta"
                CTE_Secondarie_Campi = StrPeriodClause & "Veg_Des, Cul_Des, Tipo_Cod, QtaTot"
            Else
                CTE_Iniziale_Campi = "Lav_Cod, Cau_Mov, Data_Movimento, " & StrPeriodClause & "Mat_Cod, Veg_Des, Cul_Des, Qta"
                CTE_Secondarie_Campi = StrPeriodClause & "Veg_Des, Cul_Des, QtaTot"
            End If

            ' CTE principale (base per le altre CTE)
            StrSQL.AppendLine(" ;with Report_CTE (" & CTE_Iniziale_Campi & ") ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" select  ")
            StrSQL.AppendLine(" 		Agenda.Lav_Cod, ")
            StrSQL.AppendLine(" 		Movimenti.Cau_Mov, ")
            StrSQL.AppendLine(" 		Movimenti.Data_Movimento, ")
            Select Case tipoIntervalliLettura
                Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                    StrSQL.AppendLine(" 		YEAR(Movimenti.Data_Movimento) as Data_Movimento_Anno, ")
                    StrSQL.AppendLine(" 		(MONTH(Movimenti.Data_Movimento) - 1)/3 + 1 as Data_Movimento_Quarter, ")
            End Select
            StrSQL.AppendLine(" 		Materie_Prime.Mat_Cod, ")
            StrSQL.AppendLine(" 		SpecieVegetali.Veg_Des, ")
            StrSQL.AppendLine(" 		Cultivar.Cul_Des, ")
            If letturaParametroQualitativo Then
                StrSQL.AppendLine(" 		case when (Materie_Prime_Campionature.Tipo_Cod = 0) then " & Agro_SQL_SaveNum(paramTipoCodZeroPlaceholder) & " else Materie_Prime_Campionature.Tipo_Cod end as Tipo_Cod, ")
            End If
            StrSQL.AppendLine(" 		Movimenti_dettagli.Qta ")
            StrSQL.AppendLine(" 	from Agenda ")
            StrSQL.AppendLine(" 	inner join Movimenti on agenda.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	inner join Movimenti_dettagli on movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine(" 	inner join Materie_Prime on Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod and Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")
            StrSQL.AppendLine(" 	inner join SpecieVegetali on Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod  ")
            StrSQL.AppendLine(" 	inner join Cultivar on SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod and Materie_Prime.Cul_Cod = Cultivar.Cul_Cod  ")
            If letturaParametroQualitativo Then
                StrSQL.AppendLine(" 	left join Materie_Prime_Campionature on Movimenti_dettagli.cal_cod = Materie_Prime_Campionature.Progressivo ")
            End If
            StrSQL.AppendLine(" 	where 1 = 1 ")
            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" 	and Agenda.PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            StrSQL.AppendLine("     and Movimenti.Cau_Mov in (" & strCauMov & ") ")
            StrSQL.AppendLine(" 	and Movimenti_dettagli.Elem_Cod = " & TRASFORMATI_VEGETALI & " ")
            StrSQL.AppendLine(" 	and Movimenti_Dettagli.Jolly_Int = " & MagazzinoMovimentato & " ")
            StrSQL.AppendLine(" 	and Movimenti_Dettagli.Contabilizzato >= 0 ")
            If letturaParametroQualitativo Then
                StrSQL.AppendLine(" 	and Materie_Prime_Campionature.Tipo = '" & Agro_SQL_SaveText(parametroQualitativo) & "' ")
            End If
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" 	and " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE relativa lo stock iniziale di magazzino (somma quantità prodotti all'inizio del periodo di tempo)
            StrSQL.AppendLine(" InitialStock_CTE (" & CTE_Secondarie_Campi & ") ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" 	select  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            StrSQL.AppendLine(" 		Cul_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Tipo_Cod, ")
            End If
            StrSQL.AppendLine(" 		(select SUM(case when Cau_Mov in (" & strCauMovCarichi & ") then Qta else -Qta end) ")
            StrSQL.AppendLine(" 		    From Report_CTE As r2 ")
            StrSQL.AppendLine(" 		    Where ")
            StrSQL.AppendLine(" 		        r2.Mat_Cod = r1.Mat_Cod ")
            Select Case tipoIntervalliLettura
                Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                    StrSQL.AppendLine(" 		and (YEAR(r2.Data_Movimento) < r1.Anno or (YEAR(r2.Data_Movimento) = r1.Anno and ((MONTH(r2.Data_Movimento) - 1)/3 + 1) < r1.Anno_Quarter)) ")
            End Select
            StrSQL.AppendLine("         ) ")
            StrSQL.AppendLine(" 	from Report_CTE as r1 ")
            StrSQL.AppendLine(" 	where ")
            StrSQL.AppendLine(" 		Data_Movimento >= " & Agro_SQL_SaveDateTime(data_inizio) & " ")
            StrSQL.AppendLine(" 		And Data_Movimento <= " & Agro_SQL_SaveDateTime(data_fine) & " ")
            StrSQL.AppendLine(" 	group by  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Mat_Cod, ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Cul_Des, ")
                StrSQL.AppendLine(" 		Tipo_Cod ")
            Else
                StrSQL.AppendLine(" 		Cul_Des ")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE relativa la quantità totale di prodotti in ingresso a magazzino nel periodo di tempo
            StrSQL.AppendLine(" InMov_CTE (" & CTE_Secondarie_Campi & ") ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" 	select  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            StrSQL.AppendLine(" 		Cul_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Tipo_Cod, ")
            End If
            StrSQL.AppendLine(" 		SUM(Qta) ")
            StrSQL.AppendLine(" 	from Report_CTE  ")
            StrSQL.AppendLine(" 	where  ")
            StrSQL.AppendLine(" 		Data_Movimento >= " & Agro_SQL_SaveDateTime(data_inizio) & " ")
            StrSQL.AppendLine(" 		and Data_Movimento <= " & Agro_SQL_SaveDateTime(data_fine) & " ")
            StrSQL.AppendLine(" 		and Cau_Mov in (" & strCauMovCarichi & ") ")
            StrSQL.AppendLine(" 		and Lav_Cod not in (" & strLavCodDaEscluderePerInOut & ") ")
            StrSQL.AppendLine(" 	group by  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Cul_Des, ")
                StrSQL.AppendLine(" 		Tipo_Cod ")
            Else
                StrSQL.AppendLine(" 		Cul_Des ")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE relativa la quantità totale di prodotti in uscita da magazzino nel periodo di tempo
            StrSQL.AppendLine(" OutMov_CTE (" & CTE_Secondarie_Campi & ") ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" 	select  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            StrSQL.AppendLine(" 		Cul_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Tipo_Cod, ")
            End If
            StrSQL.AppendLine(" 		SUM(Qta) ")
            StrSQL.AppendLine(" 	from Report_CTE  ")
            StrSQL.AppendLine(" 	where  ")
            StrSQL.AppendLine(" 		Data_Movimento >= " & Agro_SQL_SaveDateTime(data_inizio) & " ")
            StrSQL.AppendLine(" 		and Data_Movimento <= " & Agro_SQL_SaveDateTime(data_fine) & " ")
            StrSQL.AppendLine(" 		and Cau_Mov in (" & strCauMovScarichi & ") ")
            StrSQL.AppendLine(" 		and Lav_Cod not in (" & strLavCodDaEscluderePerInOut & ") ")
            StrSQL.AppendLine(" 	group by  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Cul_Des, ")
                StrSQL.AppendLine(" 		Tipo_Cod ")
            Else
                StrSQL.AppendLine(" 		Cul_Des ")
            End If
            StrSQL.AppendLine(" ), ")

            ' CTE relativa lo stock finale di magazzino (somma quantità prodotti alla fine del periodo di tempo)
            StrSQL.AppendLine(" FinalStock_CTE (" & CTE_Secondarie_Campi & ") ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" 	select  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            StrSQL.AppendLine(" 		Cul_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Tipo_Cod, ")
            End If
            StrSQL.AppendLine(" 		(select SUM(case when Cau_Mov in (" & strCauMovCarichi & ") then Qta else -Qta end) ")
            StrSQL.AppendLine(" 		    From Report_CTE As r2 ")
            StrSQL.AppendLine(" 		    Where ")
            StrSQL.AppendLine(" 		        r2.Mat_Cod = r1.Mat_Cod ")
            Select Case tipoIntervalliLettura
                Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                    StrSQL.AppendLine(" 		and (YEAR(r2.Data_Movimento) < r1.Anno or (YEAR(r2.Data_Movimento) = r1.Anno and ((MONTH(r2.Data_Movimento) - 1)/3 + 1) <= r1.Anno_Quarter)) ")
            End Select
            StrSQL.AppendLine("         ) ")
            StrSQL.AppendLine(" 	from Report_CTE as r1 ")
            StrSQL.AppendLine(" 	where ")
            StrSQL.AppendLine(" 		Data_Movimento >= " & Agro_SQL_SaveDateTime(data_inizio) & " ")
            StrSQL.AppendLine(" 		And Data_Movimento <= " & Agro_SQL_SaveDateTime(data_fine) & " ")
            StrSQL.AppendLine(" 	group by  ")
            StrSQL.AppendLine(" 		" & StrPeriodClause & " ")
            StrSQL.AppendLine(" 		Mat_Cod, ")
            StrSQL.AppendLine(" 		Veg_Des, ")
            If (letturaParametroQualitativo) Then
                StrSQL.AppendLine(" 		Cul_Des, ")
                StrSQL.AppendLine(" 		Tipo_Cod ")
            Else
                StrSQL.AppendLine(" 		Cul_Des ")
            End If
            StrSQL.AppendLine(" ), ")

            If letturaParametroQualitativo Then
                StrSQL.AppendLine(StrCTEs.ToString())
            End If

            ' CTE di tutti i prodotti movimentati (divisi per specie, varietà e periodo movimento)
            StrSQL.AppendLine(" Prodotti_CTE (" & StrPeriodClause & "Veg_Des, Cul_Des) ")
            StrSQL.AppendLine(" as ( ")
            StrSQL.AppendLine(" 	select distinct " & StrPeriodClause & "Veg_Des, Cul_Des from Report_CTE ")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine("")

            ' Select finale
            StrSQL.AppendLine(" select ")
            Select Case tipoIntervalliLettura
                Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                    StrSQL.AppendLine("     row_number() over (order by Prodotti_CTE.Anno, Prodotti_CTE.Anno_Quarter, Prodotti_CTE.Veg_Des, Prodotti_CTE.Cul_Des) as ID ")
                    StrSQL.AppendLine(" 	, (Cast(Prodotti_CTE.Anno as varchar(4)) + ' Q' + Cast(Prodotti_CTE.Anno_Quarter as varchar(2))) as Periodo ")
                Case Else
                    StrSQL.AppendLine("     row_number() over (order by Prodotti_CTE.Veg_Des, Prodotti_CTE.Cul_Des) as ID ")
            End Select
            StrSQL.AppendLine(" 	, Prodotti_CTE.Veg_Des ")
            StrSQL.AppendLine(" 	, Prodotti_CTE.Cul_Des ")
            If letturaParametroQualitativo Then
                StrSQL.Append(StrSelectCase_Initial.ToString())
                StrSQL.Append(StrSelectCase_In.ToString())
                StrSQL.Append(StrSelectCase_Out.ToString())
                StrSQL.Append(StrSelectCase_Final.ToString())
            Else
                BilancioDiMassa_AppendQtaSelectCaseForParam(StrSQL, "InitialStock_CTE", InitialStockColumnPrefix)
                BilancioDiMassa_AppendQtaSelectCaseForParam(StrSQL, "InMov_CTE", InQtaColumnPrefix)
                BilancioDiMassa_AppendQtaSelectCaseForParam(StrSQL, "OutMov_CTE", OutQtaColumnPrefix)
                BilancioDiMassa_AppendQtaSelectCaseForParam(StrSQL, "FinalStock_CTE", FinalStockColumnPrefix)
                ' La giacenza finale non è garantito che sia pari alla somma della giacenza iniziale più quantità in entrata e meno quantità in uscita 
                'BilancioDiMassa_AppendQtaSelectCaseForFinalStockParam(StrSQL, "InitialStock_CTE", "InMov_CTE", "OutMov_CTE", FinalStockColumnPrefix)
            End If
            StrSQL.AppendLine(" from Prodotti_CTE  ")
            If letturaParametroQualitativo Then
                StrSQL.Append(StrJoinCTE.ToString())
            Else
                BilancioDiMassa_AppendJoinForParam(StrSQL, "InitialStock_CTE", tipoIntervalliLettura)
                BilancioDiMassa_AppendJoinForParam(StrSQL, "InMov_CTE", tipoIntervalliLettura)
                BilancioDiMassa_AppendJoinForParam(StrSQL, "OutMov_CTE", tipoIntervalliLettura)
                BilancioDiMassa_AppendJoinForParam(StrSQL, "FinalStock_CTE", tipoIntervalliLettura)
            End If
            StrSQL.AppendLine(" where 1 = 0 ")
            If letturaParametroQualitativo Then
                StrSQL.Append(StrNotNullWhereClause.ToString())
            Else
                BilancioDiMassa_AppendNotNullClauseForParam(StrSQL, "InitialStock_CTE")
                BilancioDiMassa_AppendNotNullClauseForParam(StrSQL, "InMov_CTE")
                BilancioDiMassa_AppendNotNullClauseForParam(StrSQL, "OutMov_CTE")
                BilancioDiMassa_AppendNotNullClauseForParam(StrSQL, "FinalStock_CTE")
            End If
            StrSQL.Append(" order by ")
            If Not tipoIntervalliLettura = BilancioDiMassa_TipoIntervalliLettura.Nessuno Then
                StrSQL.Append("Periodo, ")
            End If
            StrSQL.AppendLine("Veg_Des, Cul_Des ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub BilancioDiMassa_AppendCTE_ForParam(ByRef StrSQL As StringBuilder, ByVal CTE_dest As String, ByVal Period_Clause As String, ByVal CTE_from As String, ByVal Tipo_Cod As Integer)

        StrSQL.AppendLine(" " & CTE_dest & " (" & Period_Clause & "Veg_Des, Cul_Des, Tipo_Cod, QtaTot) ")
        StrSQL.AppendLine(" as ( ")
        StrSQL.AppendLine(" 	select * from " & CTE_from & " where Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & " ")
        StrSQL.AppendLine(" ), ")

    End Sub

    Private Sub BilancioDiMassa_AppendQtaSelectCaseForParam(ByRef StrSQL As StringBuilder, ByVal CTE_select As String, ByVal columnName As String)

        StrSQL.AppendLine(" 	, case when (" & CTE_select & ".QtaTot is null) then 0 else " & CTE_select & ".QtaTot end as [" & columnName & "] ")

    End Sub

    'Private Sub BilancioDiMassa_AppendQtaSelectCaseForFinalStockParam(ByRef StrSQL As StringBuilder, ByVal CTE_initial As String, ByVal CTE_in As String, ByVal CTE_out As String, ByVal columnName As String)

    '    StrSQL.AppendLine(" 	, (case when (" & CTE_initial & ".QtaTot Is null) then 0 else " & CTE_initial & ".QtaTot end) + (case when (" & CTE_in & ".QtaTot Is null) then 0 else " & CTE_in & ".QtaTot end) - (Case when (" & CTE_out & ".QtaTot Is null) then 0 else " & CTE_out & ".QtaTot end) as [" & columnName & "] ")

    'End Sub

    Private Sub BilancioDiMassa_AppendJoinForParam(ByRef StrSQL As StringBuilder, ByVal CTE_join As String, ByVal tipoIntervalli As BilancioDiMassa_TipoIntervalliLettura)

        StrSQL.AppendLine(" left join " & CTE_join & " on  ")
        Select Case tipoIntervalli
            Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                StrSQL.AppendLine(" 	Prodotti_CTE.Anno = " & CTE_join & ".Anno  ")
                StrSQL.AppendLine(" 	and Prodotti_CTE.Anno_Quarter = " & CTE_join & ".Anno_Quarter  ")
                StrSQL.Append(" 	and ")
        End Select
        StrSQL.AppendLine(" 	Prodotti_CTE.Veg_Des = " & CTE_join & ".Veg_Des ")
        StrSQL.AppendLine(" 	and Prodotti_CTE.Cul_Des = " & CTE_join & ".Cul_Des ")

    End Sub

    Private Sub BilancioDiMassa_AppendNotNullClauseForParam(ByRef StrSQL As StringBuilder, ByVal CTE_where As String)

        StrSQL.AppendLine(" 	or (" & CTE_where & ".QtaTot is NOT null and " & CTE_where & ".QtaTot > 0) ")

    End Sub

    Public Function LeggiParametriQualitativiPerBilancio(ByVal Piva As String, ByVal Tabella_Cod As Integer, ByVal Tabella_Cod_Des As String, ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Bilancio.Leggi_Da_OTabelle"

        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        StrSQL.AppendLine(" select Tabella_Cod, Tabella_Cod_Des ")
        StrSQL.AppendLine(" from OTabelle ")
        StrSQL.AppendLine(" where (OTabelle.Piva = 'AAAAAAAAAAA' OR OTabelle.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")

        If Tabella_Cod <> 0 Then
            StrSQL.AppendLine(" and OTabelle.Tabella_Cod = " & Agro_SQL_SaveNum(Tabella_Cod) & " ")
        ElseIf Not String.IsNullOrEmpty(Tabella_Cod_Des) Then
            StrSQL.AppendLine(" and (Tabella_Cod_Des = '" & Agro_SQL_SaveText(Tabella_Cod_Des) & "' OR ('o' + Tabella_Cod_Des) = '" & Agro_SQL_SaveText(Tabella_Cod_Des) & "') ")
        End If

        '--------------------------------------------------------------------------
        DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        Return DT

    End Function
    Public Function LeggiValoriParametroQualitativoPerBilancio(ByVal Piva As String, ByVal Tabella_Cod As Integer,
                                                                ByVal leggiTuttiValori As Boolean, ByRef valoriDaLeggere_Cod As Integer(), ByRef valoriDaLeggere_Desc As String(),
                                                                ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Bilancio.Leggi_Da_OTabelle_Parametri"

        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        StrSQL.AppendLine(" select OTabelle.Tabella_Cod_Des, OTabelle_Parametri.Tabella_Par_Cod, OTabelle_Parametri.Codice_Origine, OTabelle_Parametri.Descrizione, OTabelle_Parametri.Valore_Min, OTabelle_Parametri.Valore_Max ")
        StrSQL.AppendLine(" from OTabelle ")
        StrSQL.AppendLine(" inner join OTabelle_Parametri on OTabelle.Tabella_Cod = OTabelle_Parametri.Tabella_Cod  ")
        StrSQL.AppendLine(" where (OTabelle_Parametri.Piva = 'AAAAAAAAAAA' OR OTabelle_Parametri.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
        StrSQL.AppendLine(" and OTabelle.Tabella_Cod = " & Agro_SQL_SaveNum(Tabella_Cod) & " ")

        If Not leggiTuttiValori Then
            If Not IsNothing(valoriDaLeggere_Cod) Then
                If valoriDaLeggere_Cod.Length > 0 Then
                    StrSQL.AppendLine(" and OTabelle_Parametri.Tabella_Par_Cod in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", valoriDaLeggere_Cod)) & ") ")
                End If
            ElseIf Not IsNothing(valoriDaLeggere_Desc) Then
                If valoriDaLeggere_Desc.Length > 0 Then
                    For i = 0 To valoriDaLeggere_Desc.Length - 1
                        If (valoriDaLeggere_Desc(i) = "") Then
                            valoriDaLeggere_Desc(i) = "''"
                        End If
                    Next
                    StrSQL.AppendLine(" and OTabelle_Parametri.Descrizione in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", valoriDaLeggere_Desc), valoriStringa:=True) & ") ")
                End If
            End If
        End If

        StrSQL.AppendLine(" order by OTabelle_Parametri.Tabella_Par_Cod ")

        '--------------------------------------------------------------------------
        DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        Dim isCalibro As Boolean = If(DT.Rows.Count > 0, DT.Rows(0)("Tabella_Cod_Des").ToString().ToLower() = "calibro", False)

        Dim lsCods As List(Of String) = (From row In DT Select row.Field(Of String)("Codice_Origine")).Distinct().ToList()
        For Each CodiceOrigine In lsCods
            If DT.Select("Codice_Origine = '" & CodiceOrigine & "'").Length > 1 Then
                Dim duplicates = True

                If isCalibro Then
                    For Each row In DT.Select("Codice_Origine = '" & CodiceOrigine & "'")
                        Dim MinMaxSuffix = row("Valore_Min") & "-" & row("Valore_Max")
                        CodiceOrigine = CodiceOrigine & "_" & MinMaxSuffix
                        row("Codice_Origine") = CodiceOrigine
                    Next

                    duplicates = DT.Select("Codice_Origine = '" & CodiceOrigine & "'").Length > 1
                End If

                If duplicates Then
                    Dim i As Integer = 0
                    For Each row In DT.Select("Codice_Origine = '" & CodiceOrigine & "'")
                        i = i + 1
                        row("Codice_Origine") = CodiceOrigine & i.ToString()
                    Next
                End If
            End If
        Next

        Return DT

    End Function

End Class
