
Imports <xmlns="http://www.agronica.it/track/">

Imports System.Data
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json

Public Class FF_LiquidazioneSoci
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Scrivi_Acconti_Liquidazioni(ByVal piva As String,
                                                ByVal Id_Acconto_Liquidazione As Integer,
                                                ByVal StatoCampionamento_0 As String,
                                                ByVal TipoCampionamento_0 As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal docNumeroSin As String, ByVal docNumero As Integer, ByVal docNumeroDes As String,
                                                ByVal docNumeroSinAutof As String, ByVal docNumeroAutof As Integer, ByVal docNumeroDesAutof As String
                                                ) As String

        Const nomeRoutine = "AgronicaCoreContabBIZ.FF_LiquidazioneSoci.Scrivi_Liquidazioni()"
        Dim risposta As String = ""

        Dim campConf_R As New FF_CampionamentoConferimento_R
        Dim ru As New Risorse_Umane_R
        Dim contatto As New Contatti_R
        Dim leggi_liquidazioneSoci As New FF_LiquidazioneSoci_R

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim Liquidazione_Mov_Dati_Generali_ToInsert As New List(Of Liquid_Mov_Dati_Generali_CampionamentoConferito)
        Dim Liquidazione_Mov_ToInsert As New List(Of Liquid_Mov_CampionamentoConferito)
        Dim Liquidazione_Mov_Calibri_ToInsert As New List(Of Liquid_Mov_PerCalibro_CampionamentoConferito)
        Dim Liquidazione_Mov_FattVar_ToInsert As New List(Of Liquid_Mov_FattVariaz_CampionamentoConferito)

        Try

            Dim AnagAccontoLiquidazione =
            campConf_R.LeggiElem_AnagAccontiLiquidazioni(
                            piva, Id_Acconto_Liquidazione, objParametri)

            If Not AnagAccontoLiquidazione Is Nothing Then

                Dim separators() As String = {"|"}
                Dim specieStringArray As String()
                Dim specieDaFiltrareArrayInt As Integer() = {}
                If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.specie) AndAlso
                    Not AnagAccontoLiquidazione.specie = "-1" Then
                    specieStringArray = AnagAccontoLiquidazione.specie.Split(separators,
                          StringSplitOptions.RemoveEmptyEntries)
                    specieDaFiltrareArrayInt = Array.ConvertAll(specieStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                End If

                Dim varietaStringArray As String()
                Dim varietaDaFiltrareArrayInt As Integer() = {}
                If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.varieta) Then
                    varietaStringArray = AnagAccontoLiquidazione.varieta.Split(separators,
                          StringSplitOptions.RemoveEmptyEntries)
                    varietaDaFiltrareArrayInt = Array.ConvertAll(varietaStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                End If

                Dim operazioniStringArray As String()
                Dim operazioniDaFiltrareArrayInt As Integer() = {}
                If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.causale_movimento) Then
                    operazioniStringArray = AnagAccontoLiquidazione.causale_movimento.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    operazioniDaFiltrareArrayInt = Array.ConvertAll(operazioniStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                End If

                Dim fornitoriStringArray As String() = {}
                If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.fornitori) Then
                    fornitoriStringArray = AnagAccontoLiquidazione.fornitori.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                End If

                Dim gruppoFattStringArray As String()
                Dim gruppoFattDaFiltrareArrayInt As Integer() = {}
                If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.gruppo_fatturazione) Then
                    gruppoFattStringArray = AnagAccontoLiquidazione.gruppo_fatturazione.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    gruppoFattDaFiltrareArrayInt = Array.ConvertAll(gruppoFattStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                End If

                Dim prodottiStringArray As String()
                Dim prodottiDaFiltrareArrayInt As Integer() = {}
                If AnagAccontoLiquidazione.tipo_filtro_mat_cod <> 0 AndAlso
                    Not String.IsNullOrEmpty(AnagAccontoLiquidazione.filtro_mat_cod) Then

                    prodottiStringArray = AnagAccontoLiquidazione.filtro_mat_cod.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    prodottiDaFiltrareArrayInt = Array.ConvertAll(prodottiStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                End If

                ' Acconto Una Tantum  
                'TODO Stefano Al momento non è usata e non è detto che lo gestiremo così
                '  Rimandata scelta a quando allineeremp la contabilità da liquidazioni                  
                If AnagAccontoLiquidazione.tipo_acconto = "U" Then

                        Dim Liquid_Mov_CampConf As New Liquid_Mov_CampionamentoConferito
                        Liquid_Mov_CampConf.Piva_SuperUser = Piva_SuperUser
                        Liquid_Mov_CampConf.PIVA = piva
                        Liquid_Mov_CampConf.Id_acconto_liquidazione = AnagAccontoLiquidazione.id_anagrafica
                        Liquid_Mov_CampConf.Id_Mov_Det = 0
                        Liquid_Mov_CampConf.Doc_Numero_Sin = ""
                        Liquid_Mov_CampConf.Doc_Numero = 0
                        Liquid_Mov_CampConf.Doc_Numero_Des = ""
                        Liquid_Mov_CampConf.NrRiga = ""
                        Liquid_Mov_CampConf.Data_Documento = AGRODATAINIZIO
                        Liquid_Mov_CampConf.Cod_RisUm = 0 'TODO
                        Liquid_Mov_CampConf.Grp_Fatt_Cod = 0
                        Liquid_Mov_CampConf.Grp_Fatt_Descr = ""
                        Liquid_Mov_CampConf.Grp_Fatt_Sigla = ""
                        Liquid_Mov_CampConf.Mat_Cod = 0
                        Liquid_Mov_CampConf.KgNetti = 1
                        Liquid_Mov_CampConf.Degrado = 0
                        Liquid_Mov_CampConf.DegradoPerc = 0
                        Liquid_Mov_CampConf.Mat_Des = ""
                        Liquid_Mov_CampConf.Qual_Cod = 0
                        Liquid_Mov_CampConf.Qual_Descr = ""
                        Liquid_Mov_CampConf.Qual_Sigla = ""
                        Liquid_Mov_CampConf.Certif_Cod = 0
                        Liquid_Mov_CampConf.Certif_Descr = ""
                        Liquid_Mov_CampConf.Certif_Sigla = ""
                        Liquid_Mov_CampConf.Calibro_Entrata_Cod = 0
                        Liquid_Mov_CampConf.Calibro_Entrata_Descr = ""
                        Liquid_Mov_CampConf.Calibro_Entrata_Sigla = ""
                        Liquid_Mov_CampConf.PrezzoBaseAlKg = AnagAccontoLiquidazione.perc_valore_acconto
                        Liquid_Mov_CampConf.PrezzoTotaleFattVarAlKg = 0.0
                        Liquid_Mov_CampConf.PrezzoTotaleAlKg = AnagAccontoLiquidazione.perc_valore_acconto
                        Liquid_Mov_CampConf.Cod_Conto_Econ = AnagAccontoLiquidazione.cod_conto_economico
                        Liquid_Mov_CampConf.perc_valore_acconto = 0
                        Liquid_Mov_CampConf.Data_Creazione = Date.Now
                        Liquid_Mov_CampConf.Username_Creazione = objParametri.UsernameOperazione
                        Liquid_Mov_CampConf.Data_Modifica = Date.Now
                        Liquid_Mov_CampConf.Username_Modifica = objParametri.UsernameOperazione
                        Liquid_Mov_CampConf.inviato = 0
                        Liquid_Mov_CampConf.Validita_Inizio = AGRODATAINIZIO
                        Liquid_Mov_CampConf.Validita_Fine = AGRODATAFINE
                        '=======================================================================
                        Liquidazione_Mov_ToInsert.Add(Liquid_Mov_CampConf)
                        '=======================================================================
                    End If

                    ' Acconto calcolato su conferito / campionato
                    If AnagAccontoLiquidazione.tipo_acconto = "C" OrElse
                        AnagAccontoLiquidazione.tipo_anagrafica = "L" Then

                        Dim DtValorizzazione As New DataTable

                        Dim trovatoErrore As Boolean = False

                        Dim dtTrattenute As DataTable = Nothing

                        Dim strElencoTrattenute = leggi_liquidazioneSoci.ElencoTrattenute(piva, Id_Acconto_Liquidazione, 0, dtTrattenute, objParametri)

                        Dim centri_aziendaliStringArray As String() = {}
                        Dim centri_aziendaliIntArray As Integer() = {}
                        If Not String.IsNullOrEmpty(AnagAccontoLiquidazione.centri_aziendali) Then
                            centri_aziendaliStringArray = AnagAccontoLiquidazione.centri_aziendali.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                            centri_aziendaliIntArray = Array.ConvertAll(centri_aziendaliStringArray, New Converter(Of String, Integer)(AddressOf StringToInteger))
                        End If

                    ' Qui eseguo tutti i calcoli (stessa funzione lanciata dalla valorizzazione)
                    Dim r As String = campConf_R.Estrai_Righe_Conferimento_Valorizzate(piva, centri_aziendaliIntArray, "", 0, "", 0,
                                                AnagAccontoLiquidazione.Validita_Inizio,
                                                AnagAccontoLiquidazione.Validita_Fine,
                                                specieDaFiltrareArrayInt,
                                                varietaDaFiltrareArrayInt, operazioniDaFiltrareArrayInt, fornitoriStringArray,
                                                gruppoFattDaFiltrareArrayInt, AnagAccontoLiquidazione.listino_cod, True,
                                                StatoCampionamento_0, TipoCampionamento_0,
                                                AnagAccontoLiquidazione.tipo_filtro_mat_cod, prodottiDaFiltrareArrayInt,
                                                objParametri, DtValorizzazione, trovatoErrore)

                    If trovatoErrore Then
                            MessaggioErrore = "Non è stato possibile eseguire l'elaborazione perché sono stati riscontrati errori durante la fase di calcolo.  Eseguire la valorizzazione movimenti e verificare i messaggi."
                        End If

                        If Not trovatoErrore Then

                            Dim w_IdMovDet As Integer = 0
                            Dim w_Cod_RisUm As Integer = 0
                            Dim w_Doc_Numero_Sin_Confer As String = ""
                            Dim w_Doc_Numero_Confer As Integer = 0
                            Dim w_Doc_Numero_Des_Confer As String = ""
                            Dim w_NrRiga_Confer As String = ""
                        Dim w_Data_Movimento_Confer = AGRODATAINIZIO
                        Dim w_Data_Riferimento_Prezzi = AGRODATAINIZIO
                        Dim w_Tipo_Data_Riferimento_Prezzi As String = ""
                        Dim w_Grp_Fatt_Cod As Integer = 0
                        Dim w_Grp_Fatt_Descr As String = ""
                        Dim w_Grp_Fatt_Sigla As String = ""
                        Dim w_Veg_Cod As Integer = 0
                        Dim w_Cul_Cod As Integer = 0
                        Dim w_Mat_Cod As Integer = 0
                        Dim w_Mat_Des As String = ""
                        Dim w_Qual_Cod As Integer = 0
                        Dim w_Qual_Descr As String = ""
                        Dim w_Qual_Sigla As String = ""
                        Dim w_Certif_Cod As Integer = 0
                        Dim w_Certif_Descr As String = ""
                        Dim w_Certif_Sigla As String = ""
                        Dim w_Calibro_Entrata_Cod As Integer = 0
                        Dim w_Calibro_Entrata_Descr = ""
                        Dim w_Calibro_Entrata_Sigla = ""
                        Dim w_Kgnetti As Decimal = 0.0
                        Dim w_Degrado As Decimal = 0.0
                        Dim w_DegradoPerc As Decimal = 0.0
                        Dim w_Tara As Decimal = 0.0
                        Dim w_Prezzo As Decimal = 0.0
                        Dim w_PrezzoFattoriVariazione As Decimal = 0.0
                        Dim w_Prezzo_Totale As Decimal = 0.0
                        Dim w_Imponibile As Decimal = 0.0
                        Dim w_ValoreTrasporto As Decimal = 0.0
                        Dim w_PrezzoSuConferito As Boolean = False
                        Dim w_perc_acconto As Decimal? = Nothing
                        Dim w_prezzo_da_riga_conferimento As Integer = 0
                        Dim w_messaggio_errore As String = ""
                        Dim w_messaggio_errore_riga_camp As String = ""

                        Dim dt_elenco_CodRisUm As New DataTable
                        dt_elenco_CodRisUm.Columns.Add("Cod_RisUm", GetType(Integer))
                        dt_elenco_CodRisUm.Columns.Add("Rag_Soc", GetType(String))
                        dt_elenco_CodRisUm.Columns.Add("Valore", GetType(Decimal))
                        dt_elenco_CodRisUm.Columns.Add("FattAutofatt", GetType(String))
                        dt_elenco_CodRisUm.Columns.Add("Cod_Rapporto", GetType(Integer))

                        Dim dtElencoPercAccGrpFatt As DataTable = Nothing
                        Dim strElencoPercAccGrpFatt = leggi_liquidazioneSoci.ElencoPercAccGrpFatt(piva, Id_Acconto_Liquidazione, 0, dtElencoPercAccGrpFatt, objParametri)

                        DtValorizzazione.DefaultView.Sort = "Id_Mov_Det"


                        For Each row In DtValorizzazione.DefaultView

                            'Introdotto il test sotto per evitare che in liquidazione escano delle righe legate alla nuova campagna
                            '  Questo perché ad es. durante la campagna 1/6/17 - 31/5/18, a fine maggio sono entrate delle albicocche che andranno liquidate
                            '  nella campagna 18/19
                            '  Chiaramente con questa impostazione deve essere fatta una griglia per ogni campagna
                            ' 18/6/2020 - SOSPESA QUESTA MODIFICA PERCHE' ORA SCRIVIAMO I MESSAGGI SULLE TABELLE DI LIQUIDAZIONE
                            'If row("Messaggi") <> "Non trovata griglia di campionamento per il prodotto: prezzo non determinabile" Then

                            If w_IdMovDet <> row("Id_Mov_Det") Then

                                    ' Cambiata riga: preparo riga Liquid_Mov_CampionamentoConferito
                                    If w_IdMovDet <> 0 Then

                                        Dim Liquid_Mov_CampConf =
                                                    Valorizza_Liquid_Mov_CampConf(
                                                        AnagAccontoLiquidazione,
                                                        Piva_SuperUser, piva,
                                                        w_IdMovDet, w_PrezzoSuConferito, w_Cod_RisUm,
                                                        w_Doc_Numero_Sin_Confer, w_Doc_Numero_Confer, w_Doc_Numero_Des_Confer,
                                                        w_NrRiga_Confer, w_Data_Movimento_Confer,
                                                        w_Data_Riferimento_Prezzi, w_Tipo_Data_Riferimento_Prezzi,
                                                        w_Grp_Fatt_Cod, w_Grp_Fatt_Descr, w_Grp_Fatt_Sigla,
                                                        w_Veg_Cod, w_Cul_Cod, w_Mat_Cod,
                                                        w_Kgnetti, w_Degrado, w_DegradoPerc, w_Tara, w_Mat_Des,
                                                        w_Qual_Cod, w_Qual_Descr, w_Qual_Sigla,
                                                        w_Certif_Cod, w_Certif_Descr, w_Certif_Sigla,
                                                        w_Calibro_Entrata_Cod, w_Calibro_Entrata_Descr, w_Calibro_Entrata_Sigla,
                                                        w_Prezzo, w_PrezzoFattoriVariazione, w_Prezzo_Totale,
                                                        Decimal.Round(w_Imponibile, 2), w_ValoreTrasporto,
                                                        w_perc_acconto, w_prezzo_da_riga_conferimento, w_messaggio_errore, objParametri)

                                        '=======================================================================
                                        Liquidazione_Mov_ToInsert.Add(Liquid_Mov_CampConf)
                                        '=======================================================================

                                        For Each dr_CodRisUm In dt_elenco_CodRisUm.Rows
                                            If dr_CodRisUm("Cod_RisUm") = Liquid_Mov_CampConf.Cod_RisUm Then
                                                dr_CodRisUm("Valore") = dr_CodRisUm("Valore") + w_Imponibile
                                            End If
                                        Next

                                    w_messaggio_errore = ""

                                End If

                                    'Azzero totali ed imposto i campi per il nuovo elemento che andrà a scrivere

                                    Dim cr As Integer = CInt(row("Cod_RisUm"))

                                    'Verifico se ho già trattato il socio

                                    Dim trovato_Cod_RisUm As Integer = (
                                            From elenco_CodRisUm In dt_elenco_CodRisUm
                                            Where elenco_CodRisUm("Cod_RisUm") = cr
                                            Select elenco_CodRisUm).Count()

                                    If trovato_Cod_RisUm = 0 Then
                                        Dim dr_CodRisUm = dt_elenco_CodRisUm.NewRow

                                        Dim obj_ru = ru.Leggi_RisorseUmane(cr, objParametri)
                                    Dim dt_contatto = contatto.LeggiContattoSpecifico(piva, obj_ru.Cod_Contatto, 0,
                                                                                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri, 0, True)

                                    dr_CodRisUm("Cod_RisUm") = cr
                                        dr_CodRisUm("Rag_Soc") = dt_contatto.Rows(0)("Rag_Soc")
                                        dr_CodRisUm("Valore") = 0.0
                                        dr_CodRisUm("FattAutofatt") = ""
                                        'Fattura
                                        If dt_contatto.Rows(0)("Documento_Fatturazione") = 0 Then
                                            dr_CodRisUm("FattAutofatt") = "F"
                                        End If
                                        'AutoFattura
                                        If dt_contatto.Rows(0)("Documento_Fatturazione") = 1 Then
                                            dr_CodRisUm("FattAutofatt") = "A"
                                        End If
                                        dr_CodRisUm("Cod_Rapporto") = obj_ru.Cod_Rapporto

                                        dt_elenco_CodRisUm.Rows.Add(dr_CodRisUm)

                                    End If

                                    w_Kgnetti = 0.0
                                    w_Degrado = 0.0
                                    w_Tara = 0.0
                                    w_Imponibile = 0.0
                                    w_ValoreTrasporto = 0.0

                                    w_IdMovDet = row("Id_Mov_Det")
                                    w_Cod_RisUm = row("Cod_RisUm")
                                    w_Doc_Numero_Sin_Confer = row("Doc_Numero_Sin")
                                    w_Doc_Numero_Confer = row("Doc_Numero")
                                    w_Doc_Numero_Des_Confer = row("Doc_Numero_Des")
                                    w_NrRiga_Confer = row("NrRiga")
                                    w_Data_Movimento_Confer = row("Data_Movimento")
                                    w_Data_Riferimento_Prezzi = row("Data_Riferimento_Prezzi")
                                    w_Tipo_Data_Riferimento_Prezzi = row("Tipo_Data_Riferimento_Prezzi")
                                    w_Grp_Fatt_Cod = row("Grp_Fatt_Cod")
                                    w_Grp_Fatt_Descr = row("Grp_Fatt_Descr")
                                    w_Grp_Fatt_Sigla = row("Grp_Fatt_Sigla")
                                    w_Veg_Cod = row("Veg_Cod")
                                    w_Cul_Cod = row("Cul_Cod")
                                    w_Mat_Cod = row("Mat_Cod")
                                    w_Mat_Des = row("Mat_Des")
                                    w_Qual_Cod = row("Qual_Cod")
                                    w_Qual_Descr = row("Qual_Descr")
                                    w_Qual_Sigla = row("Qual_Sigla")
                                    w_Certif_Cod = row("Certif_Cod")
                                    w_Certif_Descr = row("Certif_Descr")
                                    w_Certif_Sigla = row("Certif_Sigla")
                                    w_Calibro_Entrata_Cod = row("Calibro_Entrata_Cod")
                                    w_Calibro_Entrata_Descr = row("Calibro_Entrata_Descr")
                                    w_Calibro_Entrata_Sigla = row("Calibro_Entrata_Sigla")
                                    w_prezzo_da_riga_conferimento = CInt(row("Prezzo_da_riga_conferimento"))
                                    If row("TipoPrezzo") = "prezzo_su_conferito" Then
                                        w_PrezzoSuConferito = True
                                        w_Prezzo = row("Prezzo")
                                        w_PrezzoFattoriVariazione = row("PrezzoFattoriVariazione")
                                        w_Prezzo_Totale = row("TotalePrezzo")
                                    Else
                                        w_PrezzoSuConferito = False
                                        w_Prezzo = 0.0
                                        w_PrezzoFattoriVariazione = 0.0
                                        w_Prezzo_Totale = 0.0
                                    End If
                                    w_DegradoPerc = row("DegradoPerc")
                                    w_perc_acconto = 0.0
                                w_messaggio_errore_riga_camp = row("Messaggi")
                                'Se il messaggio è su una riga di campionamento lo metto anche in testata
                                If row("Messaggi") <> "" Then
                                    w_messaggio_errore = row("Messaggi")
                                End If
                                ' In caso di acconto il prezzo viene rapportato alla %
                                '  Questa % viene determinata se presente dal gruppo fatturazione, diversamente 
                                '  dall'anagrafica
                                ' 16/7/2018 - richiesto di applicare le % anche in sede di liquidazione
                                'If AnagAccontoLiquidazione.tipo_anagrafica = "A" Then
                                If Not dtElencoPercAccGrpFatt Is Nothing AndAlso dtElencoPercAccGrpFatt.Rows.Count > 0 Then
                                        w_perc_acconto = (
                                                    From p_a_grFatt In dtElencoPercAccGrpFatt
                                                    Where p_a_grFatt("key_gruppo_Fatturazione") = w_Grp_Fatt_Cod
                                                    Select p_a_grFatt("perc_valore_acconto")).FirstOrDefault()
                                    End If
                                    If w_perc_acconto Is Nothing OrElse w_perc_acconto = 0.0 Then
                                        w_perc_acconto = AnagAccontoLiquidazione.perc_valore_acconto
                                    End If
                                    'End If
                                End If

                                Dim w_Kgnetti_questa_riga As Decimal = 0.0
                                Dim w_Degrado_questa_riga As Decimal = 0.0
                                Dim w_Imponibile_questa_riga As Decimal = 0.0
                                Dim w_Tara_questa_riga As Decimal = 0.0
                                Dim w_ValoreTrasporto_questa_riga As Decimal = 0.0
                                Dim w_PrezzoBaseAlKg_questa_riga As Decimal = 0.0
                                Dim w_PrezzoFattVarAlKg_questa_riga As Decimal = 0.0

                                If AnagAccontoLiquidazione.tipo_valorizzazione = "P" Then

                                    'N.B.  Questi due campi se il prezzo non è legato ai campioni equivalgono a quelli sotto

                                    ' Viene pagato solo il campionato
                                    w_Kgnetti_questa_riga = row("QtaCampionata")
                                    w_Tara_questa_riga = row("TaraCampionata")
                                    w_Degrado_questa_riga = CDec(row("QtaCampionata")) / 100 * w_DegradoPerc

                                Else
                                    ' Viene pagato tutto il conferito
                                    w_Kgnetti_questa_riga = row("KgPerCalibro")
                                    w_Tara_questa_riga = row("TaraKgPerCalibro")
                                    w_Degrado_questa_riga = row("Degrado")

                                End If

                                ' 16/7/2018 - richiesto di applicare le % anche in sede di liquidazione
                                'If AnagAccontoLiquidazione.tipo_anagrafica = "A" Then
                                w_PrezzoBaseAlKg_questa_riga = row("Prezzo") / 100 * w_perc_acconto
                                w_PrezzoFattVarAlKg_questa_riga = row("PrezzoFattoriVariazione") / 100 * w_perc_acconto
                                'Else
                                '    w_PrezzoBaseAlKg_questa_riga = row("Prezzo")
                                '    w_PrezzoFattVarAlKg_questa_riga = row("PrezzoFattoriVariazione")
                                'End If
                                w_Imponibile_questa_riga = w_Kgnetti_questa_riga *
                                                    (w_PrezzoBaseAlKg_questa_riga + w_PrezzoFattVarAlKg_questa_riga)


                                If Not String.IsNullOrWhiteSpace(AnagAccontoLiquidazione.tipo_applicazione_trasporto) Then

                                    'Calcolo il prezzo del trasporto
                                    If Not AnagAccontoLiquidazione.tipo_valorizzazione_trasporto Is Nothing Then

                                        ' Valorizzazione trasporto sui kg Netti
                                        If AnagAccontoLiquidazione.tipo_valorizzazione_trasporto = "N" Then
                                            w_ValoreTrasporto_questa_riga = AnagAccontoLiquidazione.prezzo_trasporto *
                                                        w_Kgnetti_questa_riga
                                        Else
                                            ' Valorizzazione trasporto sui kg Lordi
                                            w_ValoreTrasporto_questa_riga = AnagAccontoLiquidazione.prezzo_trasporto *
                                                        (w_Kgnetti_questa_riga + w_Tara_questa_riga)
                                        End If

                                    End If

                                    ' Viene accreditato l'importo se a cura del cedente (= 0)
                                    If AnagAccontoLiquidazione.tipo_applicazione_trasporto = "C" And
                                                    row("TrasportoACura") = 0 Then
                                        'w_ValoreTrasporto_questa_riga
                                    End If

                                    ' Viene addebitato l'importo se a cura del cedente (= 1) o vettore (=2)
                                    If AnagAccontoLiquidazione.tipo_applicazione_trasporto = "D" And
                                                    (row("TrasportoACura") = 1 Or row("TrasportoACura") = 2) Then
                                        w_ValoreTrasporto_questa_riga = w_ValoreTrasporto_questa_riga * -1
                                    End If

                                End If

                                If Not w_PrezzoSuConferito Then
                                    Dim Liquid_Mov_PerCalibro_CampConf As New Liquid_Mov_PerCalibro_CampionamentoConferito
                                    Liquid_Mov_PerCalibro_CampConf.Piva_SuperUser = Piva_SuperUser
                                    Liquid_Mov_PerCalibro_CampConf.PIVA = piva
                                    Liquid_Mov_PerCalibro_CampConf.Id_acconto_liquidazione = AnagAccontoLiquidazione.id_anagrafica
                                    Liquid_Mov_PerCalibro_CampConf.Cod_RisUm = w_Cod_RisUm
                                    Liquid_Mov_PerCalibro_CampConf.Id_Mov_Det = row("Id_Mov_Det")
                                    Liquid_Mov_PerCalibro_CampConf.Id_Calibro = row("Id_Calibro")
                                    Liquid_Mov_PerCalibro_CampConf.Qual_Descr = row("Descr_Qualita_Camp")
                                    Liquid_Mov_PerCalibro_CampConf.Calibro_Descr = row("Descr_Calibro_Camp")
                                    Liquid_Mov_PerCalibro_CampConf.Qual_Calibro_Descr = row("Descr_QualCalibro_Camp")
                                    Liquid_Mov_PerCalibro_CampConf.Ordinam_Calibro = row("Ordinamento_Calibro")
                                    Liquid_Mov_PerCalibro_CampConf.KgNetti = Decimal.Round(w_Kgnetti_questa_riga, 3)
                                    Liquid_Mov_PerCalibro_CampConf.Degrado = Decimal.Round(w_Degrado_questa_riga, 3)
                                    Liquid_Mov_PerCalibro_CampConf.Tara = Decimal.Round(w_Tara_questa_riga, 3)

                                    Liquid_Mov_PerCalibro_CampConf.PrezzoBaseAlKg = w_PrezzoBaseAlKg_questa_riga
                                    Liquid_Mov_PerCalibro_CampConf.PrezzoTotaleFattVarAlKg = w_PrezzoFattVarAlKg_questa_riga

                                    Liquid_Mov_PerCalibro_CampConf.Imponibile = Decimal.Round(w_Imponibile_questa_riga, 2)
                                    Liquid_Mov_PerCalibro_CampConf.ValoreTrasporto = Decimal.Round(w_ValoreTrasporto_questa_riga, 2)

                                    Liquid_Mov_PerCalibro_CampConf.Data_Creazione = Date.Now
                                    Liquid_Mov_PerCalibro_CampConf.Username_Creazione = objParametri.UsernameOperazione
                                    Liquid_Mov_PerCalibro_CampConf.Data_Modifica = Date.Now
                                    Liquid_Mov_PerCalibro_CampConf.Username_Modifica = objParametri.UsernameOperazione
                                    Liquid_Mov_PerCalibro_CampConf.inviato = 0
                                    Liquid_Mov_PerCalibro_CampConf.Validita_Inizio = AGRODATAINIZIO
                                    Liquid_Mov_PerCalibro_CampConf.Validita_Fine = AGRODATAFINE
                                Liquid_Mov_PerCalibro_CampConf.Messaggio_Errore = w_messaggio_errore_riga_camp
                                '=======================================================================
                                Liquidazione_Mov_Calibri_ToInsert.Add(Liquid_Mov_PerCalibro_CampConf)
                                    '=======================================================================
                                End If

                                w_Kgnetti = w_Kgnetti + Decimal.Round(w_Kgnetti_questa_riga, 3)
                                w_Degrado = w_Degrado + Decimal.Round(w_Degrado_questa_riga, 3)
                                w_Tara = w_Tara + Decimal.Round(w_Tara_questa_riga, 3)
                                w_Imponibile = w_Imponibile + w_Imponibile_questa_riga
                                w_ValoreTrasporto = w_ValoreTrasporto + Decimal.Round(w_ValoreTrasporto_questa_riga, 2)

                                ' Cerco tutti i fattori di variazione attivi
                                Dim FattoreVar_ParamQual_Grouped = campConf_R.Leggi_Fattori_Variazione_Raggruppati(piva, objParametri)

                                For Each param In FattoreVar_ParamQual_Grouped
                                    Dim Liquid_Mov_FattVariaz_CampConf As New Liquid_Mov_FattVariaz_CampionamentoConferito
                                    Liquid_Mov_FattVariaz_CampConf.Piva_SuperUser = Piva_SuperUser
                                    Liquid_Mov_FattVariaz_CampConf.PIVA = piva
                                    Liquid_Mov_FattVariaz_CampConf.Id_acconto_liquidazione = AnagAccontoLiquidazione.id_anagrafica
                                    Liquid_Mov_FattVariaz_CampConf.Cod_RisUm = w_Cod_RisUm
                                    Liquid_Mov_FattVariaz_CampConf.Id_Mov_Det = row("Id_Mov_Det")
                                    If w_PrezzoSuConferito Then
                                        Liquid_Mov_FattVariaz_CampConf.Id_Calibro = 0
                                    Else
                                        Liquid_Mov_FattVariaz_CampConf.Id_Calibro = row("Id_Calibro")
                                    End If
                                    Liquid_Mov_FattVariaz_CampConf.Fatt_Var_Cod = row(param.Tabella_Cod_Des + "_Codice")
                                    Liquid_Mov_FattVariaz_CampConf.Fatt_Var_Descr = row(param.Tabella_Cod_Des + "_Descrizione")
                                    Liquid_Mov_FattVariaz_CampConf.Fatt_Var_Sigla = row(param.Tabella_Cod_Des + "_Sigla")
                                    Liquid_Mov_FattVariaz_CampConf.Id_fattore_variazione = param.Id_fatt_var
                                    ' 16/7/2018 - richiesto di applicare le % anche in sede di liquidazione
                                    'If AnagAccontoLiquidazione.tipo_anagrafica = "A" Then
                                    Liquid_Mov_FattVariaz_CampConf.PrezzoAlKg = row(param.Tabella_Cod_Des + "_VariazionePrezzo") / 100 * w_perc_acconto
                                    'Else
                                    '    Liquid_Mov_FattVariaz_CampConf.PrezzoAlKg = row(param.Tabella_Cod_Des + "_VariazionePrezzo")
                                    'End If
                                    Liquid_Mov_FattVariaz_CampConf.Data_Creazione = Date.Now
                                    Liquid_Mov_FattVariaz_CampConf.Username_Creazione = objParametri.UsernameOperazione
                                    Liquid_Mov_FattVariaz_CampConf.Data_Modifica = Date.Now
                                    Liquid_Mov_FattVariaz_CampConf.Username_Modifica = objParametri.UsernameOperazione
                                    Liquid_Mov_FattVariaz_CampConf.inviato = 0
                                    Liquid_Mov_FattVariaz_CampConf.Validita_Inizio = AGRODATAINIZIO
                                    Liquid_Mov_FattVariaz_CampConf.Validita_Fine = AGRODATAFINE
                                    '=======================================================================
                                    If Liquid_Mov_FattVariaz_CampConf.PrezzoAlKg <> 0 Then
                                        Liquidazione_Mov_FattVar_ToInsert.Add(Liquid_Mov_FattVariaz_CampConf)
                                    End If
                                    '=======================================================================
                                Next

                            'End If
                        Next

                        ' Scrittura ultima riga
                        If w_IdMovDet <> 0 Then

                            Dim Liquid_Mov_CampConf =
                                    Valorizza_Liquid_Mov_CampConf(
                                            AnagAccontoLiquidazione,
                                            Piva_SuperUser, piva,
                                            w_IdMovDet, w_PrezzoSuConferito, w_Cod_RisUm,
                                            w_Doc_Numero_Sin_Confer, w_Doc_Numero_Confer, w_Doc_Numero_Des_Confer,
                                            w_NrRiga_Confer, w_Data_Movimento_Confer,
                                            w_Data_Riferimento_Prezzi, w_Tipo_Data_Riferimento_Prezzi,
                                            w_Grp_Fatt_Cod, w_Grp_Fatt_Descr, w_Grp_Fatt_Sigla,
                                            w_Veg_Cod, w_Cul_Cod, w_Mat_Cod,
                                            w_Kgnetti, w_Degrado, w_DegradoPerc, w_Tara, w_Mat_Des,
                                            w_Qual_Cod, w_Qual_Descr, w_Qual_Sigla,
                                            w_Certif_Cod, w_Certif_Descr, w_Certif_Sigla,
                                            w_Calibro_Entrata_Cod, w_Calibro_Entrata_Descr, w_Calibro_Entrata_Sigla,
                                            w_Prezzo, w_PrezzoFattoriVariazione, w_Prezzo_Totale,
                                            Decimal.Round(w_Imponibile, 2), w_ValoreTrasporto,
                                            w_perc_acconto, w_prezzo_da_riga_conferimento, w_messaggio_errore, objParametri)

                            '=======================================================================
                            Liquidazione_Mov_ToInsert.Add(Liquid_Mov_CampConf)
                            '=======================================================================

                            For Each dr_CodRisUm In dt_elenco_CodRisUm.Rows
                                If dr_CodRisUm("Cod_RisUm") = Liquid_Mov_CampConf.Cod_RisUm Then
                                    dr_CodRisUm("Valore") = dr_CodRisUm("Valore") + w_Imponibile
                                End If
                            Next

                            w_messaggio_errore = ""

                        End If


                        ' Scrittura dati generali (i numeri sono destinati a scomparire in futuro quando scriveremo noi le fatture)

                        Dim assegnaNumeroFatt As Boolean = False
                        If docNumero <> 0 Then
                            assegnaNumeroFatt = True
                        End If

                        Dim assegnaNumeroAutoFatt As Boolean = False
                        If docNumeroAutof <> 0 Then
                            assegnaNumeroAutoFatt = True
                        End If


                        dt_elenco_CodRisUm.DefaultView.Sort = "Rag_Soc"
                        For Each dr_CodRisUm In dt_elenco_CodRisUm.DefaultView

                            Dim Liquid_Mov_Dati_Generali As New Liquid_Mov_Dati_Generali_CampionamentoConferito
                            Liquid_Mov_Dati_Generali.FattAutofatt = dr_CodRisUm("FattAutofatt")
                            ' Il flag sotto se messo a Nothing sta ad indicare che la fattura non va generata per nulla perchè
                            ' il totale trattenute supera il maturato
                            Dim totaleTrattenuteInFattura As Decimal = (
                                From trattenute In dtTrattenute
                                Where trattenute("Detrarre_Da_Fattura") = 1 And trattenute("Cod_RisUm") = dr_CodRisUm("Cod_RisUm")
                                Select trattenute).Sum(Function(tratt) CDec(tratt("Imponibile")))

                            If dr_CodRisUm("Valore") <= totaleTrattenuteInFattura Then
                                Liquid_Mov_Dati_Generali.FattAutofatt = Nothing
                            End If

                            Liquid_Mov_Dati_Generali.Piva_SuperUser = Piva_SuperUser
                            Liquid_Mov_Dati_Generali.PIVA = piva
                            Liquid_Mov_Dati_Generali.Id_acconto_liquidazione = AnagAccontoLiquidazione.id_anagrafica
                            Liquid_Mov_Dati_Generali.Cod_RisUm = dr_CodRisUm("Cod_RisUm")

                            'Non assegno il nr fattura per i fornitori

                            Liquid_Mov_Dati_Generali.Doc_Numero_Sin = ""
                            Liquid_Mov_Dati_Generali.Doc_Numero = 0
                            Liquid_Mov_Dati_Generali.Doc_Numero_Des = ""
                            Liquid_Mov_Dati_Generali.Data_Documento = Nothing

                            If dr_CodRisUm("Cod_Rapporto") <> COD_FORNITORE_ORTOFRUTTA Then

                                If assegnaNumeroFatt AndAlso Liquid_Mov_Dati_Generali.FattAutofatt = "F" AndAlso
                                        dr_CodRisUm("Valore") > 0 Then

                                    Liquid_Mov_Dati_Generali.Doc_Numero_Sin = docNumeroSin
                                    Liquid_Mov_Dati_Generali.Doc_Numero = docNumero
                                    Liquid_Mov_Dati_Generali.Doc_Numero_Des = docNumeroDes

                                    docNumero = docNumero + 1

                                End If

                                If assegnaNumeroAutoFatt AndAlso Liquid_Mov_Dati_Generali.FattAutofatt = "A" AndAlso
                                        dr_CodRisUm("Valore") > 0 Then

                                    Liquid_Mov_Dati_Generali.Doc_Numero_Sin = docNumeroSinAutof
                                    Liquid_Mov_Dati_Generali.Doc_Numero = docNumeroAutof
                                    Liquid_Mov_Dati_Generali.Doc_Numero_Des = docNumeroDesAutof

                                    docNumeroAutof = docNumeroAutof + 1

                                End If

                                Liquid_Mov_Dati_Generali.Data_Documento = AnagAccontoLiquidazione.data_documento

                            End If

                            Liquid_Mov_Dati_Generali.Data_Creazione = Date.Now
                            Liquid_Mov_Dati_Generali.Username_Creazione = objParametri.UsernameOperazione
                            Liquid_Mov_Dati_Generali.Data_Modifica = Date.Now
                            Liquid_Mov_Dati_Generali.Username_Modifica = objParametri.UsernameOperazione
                            Liquid_Mov_Dati_Generali.inviato = 0
                            Liquid_Mov_Dati_Generali.Validita_Inizio = AGRODATAINIZIO
                            Liquid_Mov_Dati_Generali.Validita_Fine = AGRODATAFINE
                            '=======================================================================
                            Liquidazione_Mov_Dati_Generali_ToInsert.Add(Liquid_Mov_Dati_Generali)
                            '=======================================================================

                        Next

                        Dim campConf_W As New FF_LiquidazioneSoci_W
                        MessaggioErrore = campConf_W.Scrivi_Acconti_Liquidazioni(
                                piva, Id_Acconto_Liquidazione,
                                Liquidazione_Mov_Dati_Generali_ToInsert, Liquidazione_Mov_ToInsert,
                                Liquidazione_Mov_Calibri_ToInsert, Liquidazione_Mov_FattVar_ToInsert,
                                objParametri)

                        'TODO Creare una griglia con le righe che hanno dato errore 
                        'Dim serializerSettings As New JsonSerializerSettings()
                        'serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        'risposta = JsonConvert.SerializeObject(DtValorizzazione.Select("Messaggi <> ''").ToList(), Formatting.None, serializerSettings)


                    End If
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return risposta

    End Function

    Private Function Valorizza_Liquid_Mov_CampConf(
            ByVal AnagAccontiLiquidazioni As AnagAccontiLiquidazioni_CampionamentoConferito,
            ByVal Piva_SuperUser As String,
            ByVal piva As String,
            ByVal w_IdMovDet As Integer,
            ByVal w_PrezzoSuConferito As Boolean,
            ByVal w_Cod_RisUm As Integer,
            ByVal w_Doc_Numero_Sin_Confer As String,
            ByVal w_Doc_Numero_Confer As Integer,
            ByVal w_Doc_Numero_Des_Confer As String,
            ByVal w_NrRiga_Confer As String,
            ByVal w_Data_Movimento_Confer As Date,
            ByVal w_Data_Riferimento_Prezzi As Date,
            ByVal w_Tipo_Data_Riferimento_Prezzi As String,
            ByVal w_Grp_Fatt_Cod As Integer,
            ByVal w_Grp_Fatt_Descr As String,
            ByVal w_Grp_Fatt_Sigla As String,
            ByVal w_Veg_Cod As Integer,
            ByVal w_Cul_Cod As Integer,
            ByVal w_Mat_Cod As Integer,
            ByVal w_Kgnetti As Decimal,
            ByVal w_Degrado As Decimal,
            ByVal w_DegradoPerc As Decimal,
            ByVal w_Tara As Decimal,
            ByVal w_Mat_Des As String,
            ByVal w_Qual_Cod As Integer,
            ByVal w_Qual_Descr As String,
            ByVal w_Qual_Sigla As String,
            ByVal w_Certif_Cod As Integer,
            ByVal w_Certif_Descr As String,
            ByVal w_Certif_Sigla As String,
            ByVal w_Calibro_Entrata_Cod As Integer,
            ByVal w_Calibro_Entrata_Descr As String,
            ByVal w_Calibro_Entrata_Sigla As String,
            ByVal w_Prezzo As Decimal,
            ByVal w_PrezzoFattoriVariazione As Decimal,
            ByVal w_Prezzo_Totale As Decimal,
            ByVal w_Imponibile As Decimal,
            ByVal w_ValoreTrasporto As Decimal,
            ByVal w_perc_acconto As Decimal,
            ByVal w_prezzo_da_riga_conferimento As Integer,
            ByVal w_messaggio_errore As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Liquid_Mov_CampionamentoConferito


        'TODO In futuro qualcuno ci chiederà di usare i decimali!
        Dim nrDecimaliPesi = 0

        Dim Liquid_Mov_CampConf As New Liquid_Mov_CampionamentoConferito
        Liquid_Mov_CampConf.Piva_SuperUser = Piva_SuperUser
        Liquid_Mov_CampConf.PIVA = piva
        Liquid_Mov_CampConf.Id_acconto_liquidazione = AnagAccontiLiquidazioni.id_anagrafica
        Liquid_Mov_CampConf.Id_Mov_Det = w_IdMovDet
        If w_PrezzoSuConferito Then
            Liquid_Mov_CampConf.Prezzo_Su_Conferito = 1
        Else
            Liquid_Mov_CampConf.Prezzo_Su_Conferito = 0
        End If
        Liquid_Mov_CampConf.Cod_RisUm = w_Cod_RisUm
        Liquid_Mov_CampConf.Doc_Numero_Sin = w_Doc_Numero_Sin_Confer
        Liquid_Mov_CampConf.Doc_Numero = w_Doc_Numero_Confer
        Liquid_Mov_CampConf.Doc_Numero_Des = w_Doc_Numero_Des_Confer
        Liquid_Mov_CampConf.NrRiga = w_NrRiga_Confer
        Liquid_Mov_CampConf.Data_Documento = w_Data_Movimento_Confer
        Liquid_Mov_CampConf.Data_Riferimento_Prezzi = w_Data_Riferimento_Prezzi
        Liquid_Mov_CampConf.Tipo_Data_Riferimento_Prezzi = w_Tipo_Data_Riferimento_Prezzi
        Liquid_Mov_CampConf.Grp_Fatt_Cod = w_Grp_Fatt_Cod
        Liquid_Mov_CampConf.Grp_Fatt_Descr = w_Grp_Fatt_Descr
        Liquid_Mov_CampConf.Grp_Fatt_Sigla = w_Grp_Fatt_Sigla
        Liquid_Mov_CampConf.Veg_Cod = w_Veg_Cod
        Liquid_Mov_CampConf.Cul_Cod = w_Cul_Cod
        Liquid_Mov_CampConf.Mat_Cod = w_Mat_Cod
        Liquid_Mov_CampConf.KgNetti = Decimal.Round(w_Kgnetti, nrDecimaliPesi)
        Liquid_Mov_CampConf.Degrado = Decimal.Round(w_Degrado, 0)
        Liquid_Mov_CampConf.DegradoPerc = Decimal.Round(w_DegradoPerc, 2)
        Liquid_Mov_CampConf.Tara = Decimal.Round(w_Tara, 3)
        Liquid_Mov_CampConf.Mat_Des = w_Mat_Des
        Liquid_Mov_CampConf.Qual_Cod = w_Qual_Cod
        Liquid_Mov_CampConf.Qual_Descr = w_Qual_Descr
        Liquid_Mov_CampConf.Qual_Sigla = w_Qual_Sigla
        Liquid_Mov_CampConf.Certif_Cod = w_Certif_Cod
        Liquid_Mov_CampConf.Certif_Descr = w_Certif_Descr
        Liquid_Mov_CampConf.Certif_Sigla = w_Certif_Sigla
        Liquid_Mov_CampConf.Calibro_Entrata_Cod = w_Calibro_Entrata_Cod
        Liquid_Mov_CampConf.Calibro_Entrata_Descr = w_Calibro_Entrata_Descr
        Liquid_Mov_CampConf.Calibro_Entrata_Sigla = w_Calibro_Entrata_Sigla

        ' I prossimi campi vengono valorizzati solo per il prodotto che non viene campionato
        ' perchè in caso di somma di più calibri di campionamento non sono significativi
        If w_PrezzoSuConferito Then
            Liquid_Mov_CampConf.PrezzoBaseAlKg = w_Prezzo
            Liquid_Mov_CampConf.PrezzoTotaleFattVarAlKg = w_PrezzoFattoriVariazione
            Liquid_Mov_CampConf.PrezzoTotaleAlKg = w_Prezzo_Totale
        Else
            Liquid_Mov_CampConf.PrezzoBaseAlKg = 0.0
            Liquid_Mov_CampConf.PrezzoTotaleFattVarAlKg = 0.0
            If w_Kgnetti = 0 Then
                Liquid_Mov_CampConf.PrezzoTotaleAlKg = 0.0
            Else
                Liquid_Mov_CampConf.PrezzoTotaleAlKg = Decimal.Round(w_Imponibile / w_Kgnetti, 5)
            End If
        End If

        Liquid_Mov_CampConf.ValoreTrasporto = w_ValoreTrasporto
        'Ricalcolo l'imponibile per uniformarmi alla gestione documenti contabili
        Liquid_Mov_CampConf.Imponibile = Decimal.Round(Liquid_Mov_CampConf.PrezzoTotaleAlKg * w_Kgnetti, 2)
        'TODO tutto per l'IVA, quando si farà occorre gestire a 4 decimali come nella gestione documenti contabili
        Liquid_Mov_CampConf.Cod_Iva = 4  'TODO Stefano
        Liquid_Mov_CampConf.Iva = Decimal.Round(Liquid_Mov_CampConf.Imponibile / 100 * 4, 2)  'TODO Stefano

        Liquid_Mov_CampConf.Cod_Conto_Econ = AnagAccontiLiquidazioni.cod_conto_economico
        Liquid_Mov_CampConf.perc_valore_acconto = w_perc_acconto
        Liquid_Mov_CampConf.Data_Creazione = Date.Now
        Liquid_Mov_CampConf.Username_Creazione = objParametri.UsernameOperazione
        Liquid_Mov_CampConf.Data_Modifica = Date.Now
        Liquid_Mov_CampConf.Username_Modifica = objParametri.UsernameOperazione
        Liquid_Mov_CampConf.inviato = 0
        Liquid_Mov_CampConf.Validita_Inizio = AGRODATAINIZIO
        Liquid_Mov_CampConf.Validita_Fine = AGRODATAFINE
        Liquid_Mov_CampConf.Prezzo_da_riga_conferim = w_prezzo_da_riga_conferimento
        Liquid_Mov_CampConf.Messaggio_Errore = w_messaggio_errore
        Return Liquid_Mov_CampConf

    End Function

    Public Function CalcoloSelezione_OttieniImporti( _
             ByVal piva As String, _
             ByVal filtro As String, _
             ByVal xFiltroAggiuntivo As String, _
             ByVal xOrderBy As String, _
             ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
             ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
         ) As DataTable



        Dim NomeRoutine As String = String.Empty
        Dim MessaggioErrore As String = String.Empty
        Dim RisultatoFunzione As New DataTable
        Dim drRis As DataRow


        Try

            Dim leggiLiq As New FF_LiquidazioneSoci_R

            Dim trk As New AgroTrack.Track
            trk.Url = getTrackWS_URL()



            Dim dtTrKOperazioniConf As String
            Dim md As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

            Dim B_1_Confezionamento As Decimal

            '1. recupero dei ricavi e di altri importi non legati a rintracciata
            Dim dt As DataTable = _
                leggiLiq.CalcoloSelezione_OttieniImporti(filtro, "", "", objParametri_server)

            RisultatoFunzione = dt.Clone

            Dim totaleRicavi As Decimal = ( _
                From a In dt.AsEnumerable _
                Select CDbl(a("A_Ricavi")) _
            ).Sum


            '2. scorro i vari lotti..
            For Each drr As DataRow In dt.Rows

                If drr("prodotto") <> "" Then


                    '2a. per ciascun lotto cerca tutte le operazioni dove vengono scaricati materiali eseguendo algoritmo di rintracciata.
                    Dim xDtrack As XDocument
                    Dim sD As String

                    sD = "<track><lotsToTrack><lot><inputData><lotto>" & drr("Fornitore_lotto") & "</lotto><tipolotto>1</tipolotto></inputData></lot></lotsToTrack></track>"

                    Dim credenziali As String

                    credenziali = Get_Str_Credenziali_WS(objParametri_server, objParametri_utenti)

                    dtTrKOperazioniConf = trk.TrackMe( _
                       credenziali, _
                       sD _
                    )

                    xDtrack = XDocument.Parse(dtTrKOperazioniConf)

                    '2b. per ciascun operazione leggo i costi scaricati


                    Dim lXDtrack As IEnumerable(Of XElement) = xDtrack.<track>.<lotsToTrack>.<lot>.<outputData>.<trasformazioni>.<trasformazione_dati>

                    For Each oRigaConf In lXDtrack

                        If oRigaConf.<datiGias>.<Elem_Cod>.Value = 205 Then

                            Dim przUni As Decimal = _
                            md.ValorizzazioneProdotto_New( _
                                1, _
                                piva, _
                                oRigaConf.<datiGias>.<Elem_Cod>.Value, _
                                oRigaConf.<datiGias>.<pro_cod>.Value, _
                                oRigaConf.<datiGias>.<mat_cod>.Value, _
                                oRigaConf.<datiGias>.<udm_cod>.Value, _
                                oRigaConf.<datiGias>.<cal_cod>.Value, _
                                0, _
                                0, _
                                oRigaConf.<datiGias>.<Lotto>.Value, _
                                AGRODATAINIZIO, _
                                AGRODATAFINE, _
                                objParametri_server _
                            )

                            B_1_Confezionamento += przUni * oRigaConf.<datiGias>.<qta>.Value

                        End If
                    Next

                End If

                '3. clono il record iniziale ed imposto il valore dei costi recuperati.
                drRis = RisultatoFunzione.NewRow
                drRis.ItemArray = drr.ItemArray

                If drr("prodotto") = "" Then
                    B_1_Confezionamento = 0
                End If

                drRis("B_1_Confezionamento") = B_1_Confezionamento * (drRis("A_Ricavi") / (totaleRicavi))

                'conteggio dei totali
                drRis("B_Costi") = _
                    drRis("B_1_Confezionamento") + _
                    drRis("B_2_Ore_Manodopera") + _
                    drRis("B_3_Sfrido") + _
                    drRis("B_4_Inefficienze") + _
                    drRis("B_5_SpeseTrasporto")

                drRis("AB_Totale_Liquidare") = _
                    drRis("A_Ricavi") - _
                    drRis("B_Costi")

                B_1_Confezionamento = 0

                RisultatoFunzione.Rows.Add(drRis)

            Next


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


        End Try

        Return RisultatoFunzione


    End Function

    Private Function getTrackWS_URL() As String

        Dim agrowebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim rval As String = agrowebConfig.LinkAgronicaAgenda2010

        rval = "http://localhost/" & rval.ToLower.Replace("gestionerichieste.aspx", "") & "/Track/Track.asmx"

        Return rval

    End Function

    Private Function Get_Str_Credenziali_WS(
                ByVal objparametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal objparametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As String


        Dim xCredenziali As New AgronicaCoreXML.XML_WS_Importa_Gias

        Return _
            xCredenziali.Genera_Stringa_Credenziali( _
                True, _
                Nothing, _
                objparametri_server.SuperUserUsername, _
                "", _
                objparametri_server.PivaSuperUser, _
                False, "", "", "", "", "", "", _
                objparametri_server.StringaConnessione, _
                objparametri_utenti.StringaConnessione)

    End Function

    Public Shared Function StringToInteger(st As String) As Integer
        Return CInt(st)
    End Function
End Class


