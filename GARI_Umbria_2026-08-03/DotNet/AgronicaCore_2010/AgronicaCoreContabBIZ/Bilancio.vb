Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text
Imports AgronicaCoreContabDAL.Bilancio_R
Imports AgronicaCoreMetaSchemaDAL
Imports SpecieVegetali_R = AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
Imports DocumentFormat.OpenXml.Drawing.Diagrams

Public Class Bilancio
    Inherits DataProvider

    ''' <summary>
    ''' Report sul bilancio di massa per azienda (o tutte le aziende) sui moviemnti dei lavorati vegetali: somma le quantità dei prodotti per quarto dell'anno, specie e varietà dei prodotti vegetali, in un periodo di tempo indicato.
    ''' Suddivide le quantità in colonna per giacenze/stock iniziali e finali, quantità in ingresso ed in uscita, nel periodo di tempo indicato.
    ''' Se specificato un parametro qualitativo, applica una ulteriore suddivisione per ciascuna delle quattro colonne sopra indicate, per tutti i valori che il parametro può assumere (o solo per i valori specificati).
    ''' NOTA: a differenza della query in AgronicaCoreContabDAL.Bilancio_R, questa non crea una query se stante ma riutilizza le funzioni di calcolo giacenze e movimenti di magazzino 
    ''' </summary>
    ''' <param name="piva">PIVA dell'azienda per la quale considerare movimenti e calcolare il bilancio di massa, se vuoto viene effettuato il bilancio di massa per tutte le imprese</param>
    ''' <param name="data_inizio">Data inizio del periodo da considerare per il report</param>
    ''' <param name="data_fine">Data fine del periodo da considerare per il report</param>
    ''' <param name="tipoIntervalliLettura">Tipo di intervallo di tempo per il quale raggrupare le operazioni sui prodotti, da riportare in report bilancio</param>
    ''' <param name="parametroQualitativo">Nome del parametro qualitativo (campo Tipo in tabella Materie_Prime_Campionature)</param>
    ''' <param name="parametroQualitativoCod">Codice relativo al parametro qualitativo (così come salvato per il parametro in OTabelle), se 0 viene ricavato</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="ObjParametri_Server"></param>
    ''' <param name="riportaRigheSenzaMovimenti">Se la DataTable di ritorno, deve contenere o meno righe senza quantità in entrata o in uscita. In tal caso riporta tutti i periodi di tempo del tipo selezionato, anche se contenenti solo giacenze e stesse giacenze</param>
    ''' <param name="leggiTuttiValoriParametro">Applica suddivisione per valori parametro qualitativo e considera tutti i suoi possibili valori</param>
    ''' <param name="valoriParametrDaLeggere_Cod">Se "leggiTuttiValoriParametro" è False, applica suddivisione colonne per parametro qualitativo e considera solo valori indicati (elencati per codice)</param>
    ''' <param name="valoriParametrDaLeggere_Desc">Se "leggiTuttiValoriParametro" è False, applica suddivisione colonne per parametro qualitativo e considera solo valori indicati (elencati per descrizione)</param>
    ''' <param name="InitialStockColumnPrefix">Prefisso colonne relative a quantità/stock di prodotti iniziale</param>
    ''' <param name="FinalStockColumnPrefix">Prefisso colonne relative a quantità/stock di prodotti finale</param>
    ''' <param name="InQtaColumnPrefix">Prefisso colonne relative a quantità di prodotti in entrata</param>
    ''' <param name="OutQtaColumnPrefix">Prefisso colonne relative a quantità di prodotti in uscita</param>
    ''' <param name="EmptyParamValueColumnSuffix">Suffisso colonne per quantità senza un valore, per il parametro qualitativo selezionato, impostato (Tipo_Cod = 0 o il valore su OTabelle_Parametri con descrizione vuota)</param>
    ''' <param name="MaxParameterValuesToRead">Se selezionato parametro qualitativo, numero massimo di valori da considerare, per evitare un numero eccessivo di colonne</param>
    ''' <returns></returns>
    Public Function CalcoloBilancioDiMassa(ByVal piva As String,
                                           ByVal data_inizio As Date,
                                           ByVal data_fine As Date,
                                           ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura,
                                           ByVal parametroQualitativo As String,
                                           ByVal parametroQualitativoCod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef ObjParametri_Server As AgronicaCoreParametri,
                                           ByRef ObjParametri_Utente As AgronicaCoreParametri,
                                           Optional ByVal riportaRigheSenzaMovimenti As Boolean = True,
                                           Optional ByVal leggiTuttiValoriParametro As Boolean = True,
                                           Optional ByVal valoriParametrDaLeggere_Cod As Integer() = Nothing,
                                           Optional ByVal valoriParametrDaLeggere_Desc As String() = Nothing,
                                           Optional ByVal InitialStockColumnPrefix As String = "InitialStock",
                                           Optional ByVal FinalStockColumnPrefix As String = "FinalStock",
                                           Optional ByVal InQtaColumnPrefix As String = "InQta",
                                           Optional ByVal OutQtaColumnPrefix As String = "OutQta",
                                           Optional ByVal EmptyParamValueColumnSuffix As String = "null",
                                           Optional ByVal MaxParameterValuesToRead As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Bilancio.CalcoloBilancioDiMassa"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            Dim arrLavCodMovimentiInterni As Integer() = New Integer() {
                LAVCOD_TRASFERIMENTO,
                LAVCOD_TRASFORMAZIONI
            }

            Dim arrCauMovCarichi As Integer() = New Integer() {
                CAU_CONFERIMENTO,
                CAU_CARICO,
                CAU_ACCETTAZIONE_BENI_DA_DIVERSI
            }

            Dim arrCauMovScarichi As Integer() = New Integer() {
                CAU_CONFERIMENTO_DIVERSI,
                CAU_SCARICO,
                CAU_ACCETTAZIONE_BENI
            }

            Dim strLavCodMovimentiInterni = String.Join(",", arrLavCodMovimentiInterni)

            Dim strCauMovCarichi = String.Join("','", arrCauMovCarichi)
            Dim strCauMovScarichi = String.Join("','", arrCauMovScarichi)

            Dim objBilancioDal As New AgronicaCoreContabDAL.Bilancio_R

            ' Se almeno uno fra "parametroQualitativo" e "parametroQualitativoCod" è valorizzato, valorizzo anche l'altro 
            Dim dtParametriQual = objBilancioDal.LeggiParametriQualitativiPerBilancio(piva, parametroQualitativoCod, parametroQualitativo, ObjParametri_Server)

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

            Dim leggiValoriParametroQual As Boolean = False
            Dim leggiValoreParametroNonDeterm As Boolean = False
            Dim esisteValoreParametroVuoto As Boolean = False
            Dim valoreParametroVuoto As Integer = 0
            Dim dtParametroValori As DataTable = Nothing

            If parametroQualitativoCod <> 0 Then

                dtParametroValori = objBilancioDal.LeggiValoriParametroQualitativoPerBilancio(piva, parametroQualitativoCod, leggiTuttiValoriParametro, valoriParametrDaLeggere_Cod, valoriParametrDaLeggere_Desc, ObjParametri_Server)

                If MaxParameterValuesToRead > 0 And dtParametroValori.Rows.Count > MaxParameterValuesToRead Then
                    dtParametroValori = dtParametroValori.AsEnumerable().Take(MaxParameterValuesToRead).CopyToDataTable()
                End If

                leggiValoriParametroQual = dtParametroValori.Rows.Count > 0

                If valoriParametrDaLeggere_Cod.Contains(0) Then
                    leggiValoriParametroQual = True
                    leggiValoreParametroNonDeterm = True
                End If

                If dtParametroValori.Select("Codice_Origine = ''").Length > 0 Then
                    esisteValoreParametroVuoto = True
                    valoreParametroVuoto = dtParametroValori.Select("Codice_Origine = ''").ElementAt(0)("Tabella_Par_Cod")
                End If

            End If

            ' Se filtro aggiuntivo impostato deve iniziare con "AND" perchè le funzioni sotto non ne fanno l'append 
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                If Not xFiltroAggiuntivo.TrimStart({" "c}).StartsWith("AND", StringComparison.OrdinalIgnoreCase) Then
                    xFiltroAggiuntivo = " AND " & xFiltroAggiuntivo
                End If
            End If

            ' Occorre considerare il giorno precedente per la giacenza iniziale effettiva (altrimenti considera i movimenti della data iniziale)
            Dim dataGiacenzaInizioEffettiva As Date = DateAdd(DateInterval.Day, -1, CDate(data_inizio))

            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
            Dim dtGiacenzeIniziali As DataTable = objGiacenze.SchedaGiacenzeMagazzino(dataGiacenzaInizioEffettiva, piva, 0, 0, TRASFORMATI_VEGETALI,
                                                                              0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False,
                                                                              xFiltroAggiuntivo,
                                                                              "", "", "", "", "", "", "", "", "", "", "",
                                                                              ObjParametri_Server, ObjParametri_Utente,
                                                                              isFreshAndFood:=True)

            Dim xFiltroAggiuntivo_PerMovimenti = " AND Agenda.Lav_Cod not in (" & LAVCOD_TRASFERIMENTO & ", " & LAVCOD_TRASFORMAZIONI & ") "
            Dim xFiltroAggiuntivo_PerMovimenti_Interni = " AND Agenda.Lav_Cod not in (" & LAVCOD_TRASFERIMENTO & ", " & LAVCOD_TRASFORMAZIONI & ") "

            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMovimenti As DataTable = objMovimenti.SchedaMovimentiMagazzino(CDate(data_inizio), CDate(data_fine), piva, 0, 0, TRASFORMATI_VEGETALI,
                                                                                 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                                 xFiltroAggiuntivo, 'xFiltroAggiuntivo & Environment.NewLine & xFiltroAggiuntivo_PerMovimenti,
                                                                                 "", "", "", "", "", "", "", "", "", "", "",
                                                                                 ObjParametri_Server, ObjParametri_Utente,
                                                                                 isFreshAndFood:=True)

            ' Movimenti in entrata/uscita NON interni (contribuiscono nel calcolo delle giacenze e nelle quantità in entrata/uscita per periodo)
            Dim drMovimentiIn = dtMovimenti.Select("Cau_Mov in ('" & strCauMovCarichi & "') AND Lav_Cod not in (" & strLavCodMovimentiInterni & ") ")
            Dim drMovimentiOut = dtMovimenti.Select("Cau_Mov in ('" & strCauMovScarichi & "') AND Lav_Cod not in (" & strLavCodMovimentiInterni & ") ")
            Dim dtMovimentiIn As DataTable = If(drMovimentiIn.Length > 0, drMovimentiIn.CopyToDataTable(), dtMovimenti.Clone())
            Dim dtMovimentiOut As DataTable = If(drMovimentiOut.Length > 0, drMovimentiOut.CopyToDataTable(), dtMovimenti.Clone())

            ' Movimenti in entrata/uscita interni (contribuiscono nel calcolo delle giacenze ma non nelle quantità in entrata/uscita per periodo)
            Dim drMovimentiInterniIn = dtMovimenti.Select("Cau_Mov in ('" & strCauMovCarichi & "') AND Lav_Cod in (" & strLavCodMovimentiInterni & ") ")
            Dim drMovimentiInterniOut = dtMovimenti.Select("Cau_Mov in ('" & strCauMovScarichi & "') AND Lav_Cod in (" & strLavCodMovimentiInterni & ") ")
            Dim dtMovimentiInterniIn As DataTable = If(drMovimentiInterniIn.Length > 0, drMovimentiInterniIn.CopyToDataTable(), dtMovimenti.Clone())
            Dim dtMovimentiInterniOut As DataTable = If(drMovimentiInterniOut.Length > 0, drMovimentiInterniOut.CopyToDataTable(), dtMovimenti.Clone())

            ' Quantita totali per giacenze e movimenti, nel caso di lettura anche per parametro qualitativo 
            ' (i periodi/intervalli di tempo sono convertiti e memorizzati come valori interi in base al tipo selezionato)
            Dim giacenzeInizialiPerParametroSpecieVarieta As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
            Dim movimentiInEntrataPerParametroPeriodoSpecieVarieta As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
            Dim movimentiInUscitaPerParametroPeriodoSpecieVarieta As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
            Dim movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
            Dim movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))

            ' Quantita totali per giacenze e movimenti, nel caso di non lettura di parametro qualitativo 
            ' (i periodi/intervalli di tempo sono convertiti e memorizzati come valori interi in base al tipo selezionato)
            Dim giacenzeInizialiPerSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal)) = Nothing
            Dim movimentiInEntrataPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))) = Nothing
            Dim movimentiInUscitaPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))) = Nothing
            Dim movimentiInterniInEntrataPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))) = Nothing
            Dim movimentiInterniInUscitaPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))) = Nothing

            Dim Veg_Cod As Integer
            Dim Cul_Cod As Integer
            Dim Tipo_Cod As Integer
            Dim paramComunNameSuffix As String

            Dim nomeColonnaParametroTipoCod As String = ""
            If leggiValoriParametroQual Then
                Dim nomeParametroReale = If(parametroQualitativo.StartsWith("o"), parametroQualitativo.Substring(1), parametroQualitativo)
                nomeColonnaParametroTipoCod = "FF_" & nomeParametroReale & "_Tipo_Cod"
            End If

            ' Tutte le combinazioni di specie vegetale - varietà rilevate sia da giacenze che da movimenti 
            Dim dtSpecieVarieta As DataTable
            dtSpecieVarieta = dtGiacenzeIniziali.DefaultView.ToTable(True, "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des")
            dtSpecieVarieta.Merge(dtMovimentiIn.DefaultView.ToTable(True, "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des"))
            dtSpecieVarieta.Merge(dtMovimentiOut.DefaultView.ToTable(True, "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des"))
            dtSpecieVarieta.DefaultView.RowFilter = "Veg_Cod <> 0 AND Cul_Cod <> 0"
            dtSpecieVarieta.DefaultView.Sort = "Veg_Cod ASC, Cul_Cod ASC"
            dtSpecieVarieta = dtSpecieVarieta.DefaultView.ToTable(True, "Veg_Cod", "Veg_Des", "Cul_Cod", "Cul_Des")

            ' I metodi SchedaGiacenzeMagazzino e SchedaMovimentiMagazzino non valorizzano le colonne Veg_Des e Cul_Des: valorizzo tramite letture separate (utilizzati dizionari per valori già letti) 
            Dim dictSpecie As New Dictionary(Of Integer, String)
            Dim dictVarieta As New Dictionary(Of Integer, String)
            Dim objSpecieVegetali As New SpecieVegetali_R
            Dim objCultivar As New Cultivar_R
            For Each row In dtSpecieVarieta.Rows
                row("Veg_Des") = LeggiSpecieVegetaleDes(row("Veg_Cod"), dictSpecie, objSpecieVegetali, ObjParametri_Server)
                row("Cul_Des") = LeggiVarietaVegetaleDes(row("Cul_Cod"), dictVarieta, objCultivar, ObjParametri_Server)
            Next

            Dim elencoPeriodi As List(Of Integer) = GetListaPeriodiInt(data_inizio, data_fine, tipoIntervalliLettura)

            ' DataTable di uscita
            DT = New DataTable()

            DT.Columns.Add("ID", GetType(Integer))
            DT.Columns.Add("Periodo", GetType(String))
            DT.Columns.Add("Veg_Cod", GetType(Integer))
            DT.Columns.Add("Veg_Des", GetType(String))
            DT.Columns.Add("Cul_Cod", GetType(Integer))
            DT.Columns.Add("Cul_Des", GetType(String))

            ' Le colonne del DataTable di uscita sono definite in base alla lettura o meno di parametro qualitativo (e di quali valori per questo vengono considerati)
            If leggiValoriParametroQual Then

                For Each paramRow In dtParametroValori.Rows

                    Tipo_Cod = paramRow("Tabella_Par_Cod")
                    paramComunNameSuffix = GetParametroQualColumnNameSuffix(paramRow, Tabella_Cod_Des, EmptyParamValueColumnSuffix)

                    DT.Columns.Add(InitialStockColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(InQtaColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(OutQtaColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(FinalStockColumnPrefix & paramComunNameSuffix, GetType(Decimal))

                    giacenzeInizialiPerParametroSpecieVarieta.Add(Tipo_Cod, New Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
                    movimentiInEntrataPerParametroPeriodoSpecieVarieta.Add(Tipo_Cod, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInUscitaPerParametroPeriodoSpecieVarieta.Add(Tipo_Cod, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta.Add(Tipo_Cod, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta.Add(Tipo_Cod, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))

                Next

                If leggiTuttiValoriParametro Or leggiValoreParametroNonDeterm Then
                    giacenzeInizialiPerParametroSpecieVarieta.Add(0, New Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
                    movimentiInEntrataPerParametroPeriodoSpecieVarieta.Add(0, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInUscitaPerParametroPeriodoSpecieVarieta.Add(0, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta.Add(0, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                    movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta.Add(0, New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))
                End If

            Else

                DT.Columns.Add(InitialStockColumnPrefix, GetType(Decimal))
                DT.Columns.Add(InQtaColumnPrefix, GetType(Decimal))
                DT.Columns.Add(OutQtaColumnPrefix, GetType(Decimal))
                DT.Columns.Add(FinalStockColumnPrefix, GetType(Decimal))

                giacenzeInizialiPerSpecieVarieta = New Dictionary(Of Integer, Dictionary(Of Integer, Decimal))
                movimentiInEntrataPerPeriodoSpecieVarieta = New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
                movimentiInUscitaPerPeriodoSpecieVarieta = New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
                movimentiInterniInEntrataPerPeriodoSpecieVarieta = New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
                movimentiInterniInUscitaPerPeriodoSpecieVarieta = New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))

            End If

            CalcolaQuantitaTotali_PerGiacenze(dtGiacenzeIniziali, nomeColonnaParametroTipoCod, valoreParametroVuoto, giacenzeInizialiPerSpecieVarieta, giacenzeInizialiPerParametroSpecieVarieta)
            CalcolaQuantitaTotali_PerMovimenti(dtMovimentiIn, nomeColonnaParametroTipoCod, valoreParametroVuoto, tipoIntervalliLettura, movimentiInEntrataPerPeriodoSpecieVarieta, movimentiInEntrataPerParametroPeriodoSpecieVarieta)
            CalcolaQuantitaTotali_PerMovimenti(dtMovimentiOut, nomeColonnaParametroTipoCod, valoreParametroVuoto, tipoIntervalliLettura, movimentiInUscitaPerPeriodoSpecieVarieta, movimentiInUscitaPerParametroPeriodoSpecieVarieta)
            CalcolaQuantitaTotali_PerMovimenti(dtMovimentiInterniIn, nomeColonnaParametroTipoCod, valoreParametroVuoto, tipoIntervalliLettura, movimentiInterniInEntrataPerPeriodoSpecieVarieta, movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta)
            CalcolaQuantitaTotali_PerMovimenti(dtMovimentiInterniOut, nomeColonnaParametroTipoCod, valoreParametroVuoto, tipoIntervalliLettura, movimentiInterniInUscitaPerPeriodoSpecieVarieta, movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta)

            If leggiValoriParametroQual And leggiTuttiValoriParametro Then
                If giacenzeInizialiPerParametroSpecieVarieta(0).Count > 0 Or
                    movimentiInEntrataPerParametroPeriodoSpecieVarieta(0).Count > 0 Or
                    movimentiInUscitaPerParametroPeriodoSpecieVarieta(0).Count > 0 Or
                    movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta(0).Count > 0 Or
                    movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta(0).Count > 0 Then

                    leggiValoreParametroNonDeterm = True
                End If
            End If

            ' Si aggiungono le colonne in più, per i parametri qualitativi non impostati 
            If leggiValoriParametroQual And leggiValoreParametroNonDeterm Then
                paramComunNameSuffix = "_" & Tabella_Cod_Des & "_" & EmptyParamValueColumnSuffix
                ' Le colonne con questo suffisso sotto potrebbero essere già state aggiunte (valore parametro qualitativo con Descrizione/Sigla/Codice Origine vuoto on OTabelle_Parametri) 
                If Not DT.Columns.Contains(InitialStockColumnPrefix & paramComunNameSuffix) Then
                    DT.Columns.Add(InitialStockColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(InQtaColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(OutQtaColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                    DT.Columns.Add(FinalStockColumnPrefix & paramComunNameSuffix, GetType(Decimal))
                End If
            End If

            Dim NuovaRiga As DataRow

            Dim prevVegCod As Integer = 0
            Dim prevCulCod As Integer = 0
            Dim currGiacenza As Decimal = 0
            Dim dictCurrGiacenza As New Dictionary(Of Integer, Decimal?)
            Dim movimentiPresenti As Boolean
            Dim movimentiPresentiPerParam As Boolean

            ' Inserimento riga per riga dei valori del DataTable d'uscita 
            For Each rowSpecieVarieta In dtSpecieVarieta.Rows

                For Each PeriodoInt In elencoPeriodi

                    NuovaRiga = DT.NewRow()

                    Veg_Cod = rowSpecieVarieta("Veg_Cod")
                    Cul_Cod = rowSpecieVarieta("Cul_Cod")

                    NuovaRiga("Periodo") = GetPeriodoString(PeriodoInt, tipoIntervalliLettura)
                    NuovaRiga("Veg_Cod") = Veg_Cod
                    NuovaRiga("Veg_Des") = rowSpecieVarieta("Veg_Des")
                    NuovaRiga("Cul_Cod") = Cul_Cod
                    NuovaRiga("Cul_Des") = rowSpecieVarieta("Cul_Des")

                    If leggiValoriParametroQual Then

                        movimentiPresenti = False

                        For Each paramRow In dtParametroValori.Rows

                            Tipo_Cod = paramRow("Tabella_Par_Cod")
                            paramComunNameSuffix = GetParametroQualColumnNameSuffix(paramRow, Tabella_Cod_Des, EmptyParamValueColumnSuffix)

                            currGiacenza = If(dictCurrGiacenza.ContainsKey(Tipo_Cod), dictCurrGiacenza(Tipo_Cod), 0)

                            SetValoriRiga(NuovaRiga, PeriodoInt, Veg_Cod, Cul_Cod, prevVegCod, prevCulCod,
                                          currGiacenza, movimentiPresentiPerParam,
                                          InitialStockColumnPrefix & paramComunNameSuffix, InQtaColumnPrefix & paramComunNameSuffix, OutQtaColumnPrefix & paramComunNameSuffix, FinalStockColumnPrefix & paramComunNameSuffix,
                                          giacenzeInizialiPerParametroSpecieVarieta(Tipo_Cod),
                                          movimentiInEntrataPerParametroPeriodoSpecieVarieta(Tipo_Cod), movimentiInUscitaPerParametroPeriodoSpecieVarieta(Tipo_Cod),
                                          movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta(Tipo_Cod), movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta(Tipo_Cod))

                            dictCurrGiacenza(Tipo_Cod) = currGiacenza

                            movimentiPresenti = movimentiPresenti Or movimentiPresentiPerParam

                        Next

                        ' La colonna extra, per i parametri non settati, solo se non esiste valore parametro con Codice_Origine vuoto (nel caso vanno sotto stessa colonna) 
                        If leggiValoreParametroNonDeterm And Not esisteValoreParametroVuoto Then

                            Tipo_Cod = 0
                            paramComunNameSuffix = "_" & Tabella_Cod_Des & "_" & EmptyParamValueColumnSuffix

                            currGiacenza = If(dictCurrGiacenza.ContainsKey(0), dictCurrGiacenza(0), 0)

                            SetValoriRiga(NuovaRiga, PeriodoInt, Veg_Cod, Cul_Cod, prevVegCod, prevCulCod,
                                          currGiacenza, movimentiPresentiPerParam,
                                          InitialStockColumnPrefix & paramComunNameSuffix, InQtaColumnPrefix & paramComunNameSuffix, OutQtaColumnPrefix & paramComunNameSuffix, FinalStockColumnPrefix & paramComunNameSuffix,
                                          giacenzeInizialiPerParametroSpecieVarieta(0),
                                          movimentiInEntrataPerParametroPeriodoSpecieVarieta(0), movimentiInUscitaPerParametroPeriodoSpecieVarieta(0),
                                          movimentiInterniInEntrataPerParametroPeriodoSpecieVarieta(0), movimentiInterniInUscitaPerParametroPeriodoSpecieVarieta(0))

                            dictCurrGiacenza(0) = currGiacenza

                            movimentiPresenti = movimentiPresenti Or movimentiPresentiPerParam

                        End If

                    Else

                        SetValoriRiga(NuovaRiga, PeriodoInt, Veg_Cod, Cul_Cod, prevVegCod, prevCulCod,
                                      currGiacenza, movimentiPresenti,
                                      InitialStockColumnPrefix, InQtaColumnPrefix, OutQtaColumnPrefix, FinalStockColumnPrefix,
                                      giacenzeInizialiPerSpecieVarieta,
                                      movimentiInEntrataPerPeriodoSpecieVarieta, movimentiInUscitaPerPeriodoSpecieVarieta,
                                      movimentiInterniInEntrataPerPeriodoSpecieVarieta, movimentiInterniInUscitaPerPeriodoSpecieVarieta)

                    End If

                    If movimentiPresenti Or riportaRigheSenzaMovimenti Then
                        DT.Rows.Add(NuovaRiga)
                    End If

                    prevVegCod = Veg_Cod
                    prevCulCod = Cul_Cod

                Next

            Next

            DT.DefaultView.Sort = "Periodo ASC, Veg_Des ASC, Cul_Des ASC"
            DT = DT.DefaultView.ToTable()

            Dim RigaID As Integer = 0
            For Each row In DT.Rows
                row("ID") = RigaID
                RigaID = RigaID + 1
            Next

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Function GetParametroQualColumnNameSuffix(ByRef paramRow As DataRow, ByRef Tabella_Cod_Des As String, ByVal paramCodiceOrigineEmptyPlaceholder As String) As String

        Dim Codice_Origine = paramRow("Codice_Origine")

        If Codice_Origine = "" Then
            Codice_Origine = paramCodiceOrigineEmptyPlaceholder
        End If

        Return "_" & Tabella_Cod_Des & "_" & Codice_Origine

    End Function

    Private Sub SetValoriRiga(ByRef Riga As DataRow,
                              ByVal periodoInt As Integer, ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer, ByVal prevVegCod As Integer, ByVal prevCulCod As Integer,
                              ByRef giacenzaFinale As Decimal, ByRef movimentiPresenti As Boolean,
                              ByVal InitialStockColumnName As String, ByVal InQtaColumnName As String, ByVal OutQtaColumnName As String, ByVal FinalStockColumnName As String,
                              ByRef giacenzeInizialiPerSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal)),
                              ByRef movimentiInEntrataPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))),
                              ByRef movimentiInUscitaPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))),
                              ByRef movimentiInterniInEntrataPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))),
                              ByRef movimentiInterniInUscitaPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))

        Dim giacenzaIniziale As Decimal
        Dim movIn As Decimal
        Dim movOut As Decimal
        Dim movInterniIn As Decimal
        Dim movInterniOut As Decimal

        If Veg_Cod = prevVegCod And Cul_Cod = prevCulCod Then
            giacenzaIniziale = giacenzaFinale
        Else
            giacenzaIniziale = GetQuantitaTotali_PerGiacenze(Veg_Cod, Cul_Cod, giacenzeInizialiPerSpecieVarieta)
        End If

        movIn = GetQuantitaTotali_PerMovimenti(periodoInt, Veg_Cod, Cul_Cod, movimentiInEntrataPerPeriodoSpecieVarieta)
        movOut = GetQuantitaTotali_PerMovimenti(periodoInt, Veg_Cod, Cul_Cod, movimentiInUscitaPerPeriodoSpecieVarieta)

        movInterniIn = GetQuantitaTotali_PerMovimenti(periodoInt, Veg_Cod, Cul_Cod, movimentiInterniInEntrataPerPeriodoSpecieVarieta)
        movInterniOut = GetQuantitaTotali_PerMovimenti(periodoInt, Veg_Cod, Cul_Cod, movimentiInterniInUscitaPerPeriodoSpecieVarieta)

        giacenzaFinale = giacenzaIniziale + movIn - movOut + movInterniIn - movInterniOut

        movimentiPresenti = movIn <> 0 Or movOut <> 0

        Riga(InitialStockColumnName) = giacenzaIniziale
        Riga(InQtaColumnName) = movIn
        Riga(OutQtaColumnName) = movOut
        Riga(FinalStockColumnName) = giacenzaFinale

    End Sub

    Private Function GetQuantitaTotali_PerGiacenze(ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer,
                                                   ByRef giacenzePerSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal))) As Decimal

        If Not IsNothing(giacenzePerSpecieVarieta) Then
            If giacenzePerSpecieVarieta.ContainsKey(Veg_Cod) Then
                If giacenzePerSpecieVarieta(Veg_Cod).ContainsKey(Cul_Cod) Then
                    Return giacenzePerSpecieVarieta(Veg_Cod)(Cul_Cod)
                End If
            End If
        End If

        Return 0

    End Function

    Private Function GetQuantitaTotali_PerMovimenti(ByVal periodoInt As Integer, ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer,
                                                    ByRef movimentiPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))) As Decimal

        If Not IsNothing(movimentiPerPeriodoSpecieVarieta) Then
            If movimentiPerPeriodoSpecieVarieta.ContainsKey(periodoInt) Then
                If movimentiPerPeriodoSpecieVarieta(periodoInt).ContainsKey(Veg_Cod) Then
                    If movimentiPerPeriodoSpecieVarieta(periodoInt)(Veg_Cod).ContainsKey(Cul_Cod) Then
                        Return movimentiPerPeriodoSpecieVarieta(periodoInt)(Veg_Cod)(Cul_Cod)
                    End If
                End If
            End If
        End If

        Return 0

    End Function

    Private Sub CalcolaQuantitaTotali_PerGiacenze(ByRef dtGiacenze As DataTable,
                                                  ByVal nomeColonnaParametroTipoCod As String,
                                                  ByVal valoreParametroVuoto As Integer,
                                                  ByRef giacenzePerSpecieVarietaDefault As Dictionary(Of Integer, Dictionary(Of Integer, Decimal)),
                                                  ByRef giacenzePerParametroSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))))

        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Qta As Decimal
        Dim Tipo_Cod As Integer

        Dim giacenzePerSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal))
        Dim giacenzePerVarieta As Dictionary(Of Integer, Decimal)

        For Each row In dtGiacenze.Rows

            Veg_Cod = row("Veg_Cod")
            Cul_Cod = row("Cul_Cod")
            Qta = row("Giacenza")

            If Veg_Cod = 0 Or Cul_Cod = 0 Then
                Continue For
            End If

            If nomeColonnaParametroTipoCod <> "" And Not IsNothing(giacenzePerParametroSpecieVarieta) And giacenzePerParametroSpecieVarieta.Count > 0 Then
                Tipo_Cod = row(nomeColonnaParametroTipoCod)

                If Tipo_Cod = 0 And valoreParametroVuoto <> 0 Then
                    Tipo_Cod = valoreParametroVuoto
                End If

                If Not giacenzePerParametroSpecieVarieta.ContainsKey(Tipo_Cod) Then
                    Continue For
                End If

                giacenzePerSpecieVarieta = giacenzePerParametroSpecieVarieta(Tipo_Cod)
            Else
                giacenzePerSpecieVarieta = giacenzePerSpecieVarietaDefault
            End If

            If Not giacenzePerSpecieVarieta.ContainsKey(Veg_Cod) Then
                giacenzePerSpecieVarieta.Add(Veg_Cod, New Dictionary(Of Integer, Decimal))
            End If

            giacenzePerVarieta = giacenzePerSpecieVarieta(Veg_Cod)

            If Not giacenzePerVarieta.ContainsKey(Cul_Cod) Then
                giacenzePerVarieta.Add(Cul_Cod, 0)
            End If

            Dim currQta As Decimal = giacenzePerVarieta(Cul_Cod)
            giacenzePerVarieta(Cul_Cod) = currQta + Qta
        Next

    End Sub

    Private Sub CalcolaQuantitaTotali_PerMovimenti(ByRef dtMovimenti As DataTable,
                                                   ByVal nomeColonnaParametroTipoCod As String,
                                                   ByVal valoreParametroVuoto As Integer,
                                                   ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura,
                                                   ByRef movimentiPerPeriodoSpecieVarietaDefault As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal))),
                                                   ByRef movimentiPerParametroPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))))

        Dim Data As DateTime
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Qta As Decimal
        Dim Tipo_Cod As Integer
        Dim periodoInt As Integer

        Dim movimentiPerPeriodoSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
        Dim movimentiPerSpecieVarieta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal))
        Dim movimentiPerVarieta As Dictionary(Of Integer, Decimal)

        For Each row In dtMovimenti.Rows

            Data = row("Data_Movimento")
            Veg_Cod = row("Veg_Cod")
            Cul_Cod = row("Cul_Cod")
            Qta = row("Qta")

            If Veg_Cod = 0 Or Cul_Cod = 0 Then
                Continue For
            End If

            periodoInt = GetPeriodoInt(Data, tipoIntervalliLettura)

            If nomeColonnaParametroTipoCod <> "" And Not IsNothing(movimentiPerParametroPeriodoSpecieVarieta) And movimentiPerParametroPeriodoSpecieVarieta.Count > 0 Then
                Tipo_Cod = row(nomeColonnaParametroTipoCod)

                If Tipo_Cod = 0 And valoreParametroVuoto <> 0 Then
                    Tipo_Cod = valoreParametroVuoto
                End If

                If Not movimentiPerParametroPeriodoSpecieVarieta.ContainsKey(Tipo_Cod) Then
                    Continue For
                End If

                movimentiPerPeriodoSpecieVarieta = movimentiPerParametroPeriodoSpecieVarieta(Tipo_Cod)
            Else
                movimentiPerPeriodoSpecieVarieta = movimentiPerPeriodoSpecieVarietaDefault
            End If

            If Not movimentiPerPeriodoSpecieVarieta.ContainsKey(periodoInt) Then
                movimentiPerPeriodoSpecieVarieta.Add(periodoInt, New Dictionary(Of Integer, Dictionary(Of Integer, Decimal)))
            End If

            movimentiPerSpecieVarieta = movimentiPerPeriodoSpecieVarieta(periodoInt)

            If Not movimentiPerSpecieVarieta.ContainsKey(Veg_Cod) Then
                movimentiPerSpecieVarieta.Add(Veg_Cod, New Dictionary(Of Integer, Decimal))
            End If

            movimentiPerVarieta = movimentiPerSpecieVarieta(Veg_Cod)

            If Not movimentiPerVarieta.ContainsKey(Cul_Cod) Then
                movimentiPerVarieta.Add(Cul_Cod, 0)
            End If

            Dim currQta As Decimal = movimentiPerVarieta(Cul_Cod)
            movimentiPerVarieta(Cul_Cod) = currQta + Qta
        Next

    End Sub

    Private Function LeggiSpecieVegetaleDes(ByVal Veg_Cod As Integer, ByRef dictSpecie As Dictionary(Of Integer, String),
                                            ByRef objSpecieVegetali As SpecieVegetali_R, ByRef ObjParametri_Server As AgronicaCoreParametri) As String

        If Not dictSpecie.ContainsKey(Veg_Cod) Then
            Dim dtSpecieVegetale = objSpecieVegetali.Leggi(Veg_Cod, 0, "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametri_Server)
            Dim Veg_Des = If(dtSpecieVegetale.Rows.Count > 0, dtSpecieVegetale(0).Item("Veg_Des"), "")
            dictSpecie.Add(Veg_Cod, Veg_Des)

            Return Veg_Des
        Else
            Return dictSpecie(Veg_Cod)
        End If

    End Function

    Private Function LeggiVarietaVegetaleDes(ByVal Cul_Cod As Integer, ByRef dictVarieta As Dictionary(Of Integer, String),
                                             ByRef objCultivar As Cultivar_R, ByRef ObjParametri_Server As AgronicaCoreParametri) As String

        If Not dictVarieta.ContainsKey(Cul_Cod) Then
            Dim dtVarieta = objCultivar.Leggi(Cul_Cod, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", ObjParametri_Server)
            Dim Cul_Des = If(dtVarieta.Rows.Count > 0, dtVarieta(0).Item("Cul_Des"), "")
            dictVarieta.Add(Cul_Cod, Cul_Des)

            Return Cul_Des
        Else
            Return dictVarieta(Cul_Cod)
        End If

    End Function

    Private Function GetListaPeriodiInt(ByVal dataInizio As Date, ByVal dataFine As Date, ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura) As List(Of Integer)

        Dim result As New List(Of Integer)

        Select Case tipoIntervalliLettura

            Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                Dim tr As Integer
                Dim trEnd = GetPeriodoInt(dataFine, tipoIntervalliLettura)
                Dim ti = CInt(Math.Floor((dataInizio.Month - 1) / 3) + 1)

                For y = dataInizio.Year To dataFine.Year
                    For t = ti To 4
                        tr = y * 100 + t

                        If tr > trEnd Then
                            Return result
                        End If

                        result.Add(tr)
                    Next

                    ti = 1
                Next

            Case Else
                result.Add(0)

        End Select

        Return result

    End Function

    Private Function GetPeriodoInt(ByVal data As Date, ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura) As Integer

        Select Case tipoIntervalliLettura
            Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                Return data.Year * 100 + CInt(Math.Floor((data.Month - 1) / 3) + 1)
            Case Else
                Return 0
        End Select

    End Function

    Private Function GetPeriodoString(ByVal periodoInt As Integer, ByVal tipoIntervalliLettura As BilancioDiMassa_TipoIntervalliLettura) As String

        Select Case tipoIntervalliLettura
            Case BilancioDiMassa_TipoIntervalliLettura.Trimestri
                Return CStr(CInt(periodoInt / 100)) + " Q" + CStr(periodoInt Mod 100)
            Case Else
                Return 0
        End Select

    End Function

End Class
