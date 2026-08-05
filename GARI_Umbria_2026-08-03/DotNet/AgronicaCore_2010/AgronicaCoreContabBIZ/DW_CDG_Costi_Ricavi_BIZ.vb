Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility



Public Class DW_CDG_Costi_Ricavi_BIZ
    Inherits AgronicaCoreDataProvider.DataProvider

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _Configurazione_Servizio As Configurazione_Servizio

    Public Sub New()
    End Sub

    Public Sub New(
            ByVal objParametriServer As AgronicaCoreParametri,
            ByVal objParametriUtente As AgronicaCoreParametri,
            ByVal objParametriSuperServer As AgronicaCoreParametri,
            ByVal configurazioneServizio As Configurazione_Servizio
        )

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _Configurazione_Servizio = configurazioneServizio

    End Sub


    '##############################################################################################
    Public Function Leggi_Tabellone_DW(ByVal _aziende As String,
                                   ByVal preset As Integer,
                                   ByVal budget As Integer,
                                   ByVal costi_ricavi As Integer,
                                   ByVal dataDal As String,
                                   ByVal dataAl As String,
                                   ByVal dataRifProgetto As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal filtro_codice_appezzamento As String,
                                   ByVal filtro_codice_impianto As String,
                                   ByVal filtro_azienda_padre As String,
                                   ByVal lancioPivot As Integer,
                                   ByRef errorString As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal bIncludiFiltroVisibilitaImprese As Boolean = False
                                   ) As String

        Dim risposta As String = ""
        Dim dtTipologie As DataTable
        Dim objTipologie As New AgronicaCoreContabBIZ.Attivita_Gruppi_Tipologia_R
        Dim gruppo1 As String = String.Empty
        Dim gruppo2 As String = String.Empty
        Dim gruppo3 As String = String.Empty
        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ.Leggi_Globale()"

        Dim dtCostiRicavi As DataTable
        Dim objCostiRicavi As New DW_CDG_Costi_Ricavi_DAL_R

        Dim strQueryOutput As String = ""
        Dim xOrderBy = ""
        If lancioPivot = 1 Then
            xOrderBy = " Ordine_Attivita, Attivita"
        End If
        dtCostiRicavi = objCostiRicavi.Leggi_Tabellone_DW(_aziende,
                                                           preset, budget, costi_ricavi, dataDal,
                                                           dataAl, dataRifProgetto,
                                                           xFiltroAggiuntivo,
                                                           filtro_codice_appezzamento,
                                                           filtro_codice_impianto,
                                                           filtro_azienda_padre,
                                                           xOrderBy,
                                                           objParametri,
                                                           bIncludiFiltroVisibilitaImprese)


        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Id_DW", "Id_DW_CDG", "number")
        c._Editabile = False
        c._Filtrabile = False
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Id_Agenda_QdC", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_Id_Agenda_QdC, "number")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Id_Agenda", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_Id_Agenda_CdG, "number")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Id_CDG", "Id_CDG", "number")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Id_CDG_Dettagli", "Id_CDG_Dettagli", "number")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Data_Inserimento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DataInserimento, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Piva", Gias.PartitaIva, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("PivaReale", Gias.PartitaIva, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descr_Modalita_Imputazione", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_Imputazione, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Lotto", Gias.Lotto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Operazione", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizioneOperazione, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Macchina", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizioneMacchina, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Proprietario_Macchina", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_ProprietarioMacchina, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Nome_Cognome", Gias.Nominativo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("NrBadge", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_NrBadge, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Categoria", Gias.Categoria, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Cod_Articolo", Gias.CodiceProdotto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Prodotto", Gias.DescrizioneProdotto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Fabbricato_Des", Gias.Magazzino, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Id_Attivita", "Id_Attivita", "string")
        c._Filtrabile = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Attivita", Gias.Attivita, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Qualifica", Gias.Qualifica, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Tariffa", Gias.Tariffa, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Turno", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_Turno, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Conto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_Conto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Ragione_Sociale_Azienda", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_RagioneSocialeAzienda, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Indirizzo_Azienda", Gias.IndirizzoAzienda, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Localita_Azienda", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_LocalitaAzienda, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Prov_Azienda", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_ProvAzienda, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Cap_Azienda", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_CAPAzienda, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Centro", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizioneCentro, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Indirizzo_Centro", Gias.IndirizzoCentro, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Localita_Centro", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_LocalitaCentro, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Prov_Centro", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_ProvCentro, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Cap_Centro", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_CAPCentro, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Campo", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizioneCampo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Codice_Appezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_CodiceAppezzamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Nome_Appezzamento", Gias.NomeAppezzamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Sup_App", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_SuperficieAppezzamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = False
        c._Display = True
        l.Add(c)

        c = New ColonneNome("MetodoProduzioneAppezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_MetodoProduzione, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Inizio_Appezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_InizioAppezzamento, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Fine_Appezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_FineAppezzamento, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("App_BIO", "App BIO", "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Gruppo_Vegetale", Gias.GruppoVegetale, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Specie_impianto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_SpecieImpianto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Varieta_impianto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_VarietaImpianto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Sup_Imp", Gias.SuperficieImpianto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = False
        c._Display = True
        l.Add(c)

        'c = New ColonneNome("Sup_Prog", "Superficie Esercizio", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        c = New ColonneNome("Descrizione_Finalita", Gias.Finalita, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Destinazione_Uso", Gias.DestinazioneUso, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Terreno_Nudo", Gias.TerrenoNudo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Inizio_Impianto", Gias.InizioImpianto, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Fine_Impianto", Gias.FineImpianto, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("codice_impianto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_CodiceImpianto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Progetto_Nome", Gias.Esercizio, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Progetto_Des", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrEsercizio, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Progetto_Validita_Inizio", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_InizioEsercizio, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Progetto_Validita_Fine", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_FineEsercizio, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Data_Chiusura_Esercizio", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_ChiusuraEsercizio, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Regolamento", Gias.Regolamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Disciplinare", Gias.Disciplinare, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Esposizione_Appezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_EsposizioneAppezzamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Ubicazione_Appezzamento", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_UbicazioneAppezzamento, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Portinnesti", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizionePortinnesti, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descrizione_Irrigazioni", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DescrizioneIrrigazioni, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        ' INIZIO Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle
        'c = New ColonneNome("Regione_Impianto", "Regione Impianto", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Prov_Impianto", "Prov. Impianto", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Comune_Impianto", "Comune Impianto", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Sezione", "Sezione", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Foglio", "Foglio", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Numero", "Numero", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        'c = New ColonneNome("Subalterno", "Subalterno", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)
        ' FINE Con queste sotto ci sarebbe duplicazione di righe se l'appezzamento è su più particelle

        c = New ColonneNome("Descrizione_Macchina_Input_Costi", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_MacchinaDestCostiRicavi, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Progetto", Gias.Progetto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Classe_Progetto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_ClasseProgetto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Tipo_Progetto", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_TipoProgetto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Linea_Produzione", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_LineaProduzione, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Lotto_Input_Costi", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_LottoDestCostiRicavi, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Mezzo_Des", Gias.UnitaDiMisuraSigla, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        'Zoo
        c = New ColonneNome("Specie_Animale_des", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_SpecieCapo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Razza_Animale_Des", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_RazzaCapo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Tipo_Animale_Des", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_TipoCapo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Stalla_Des", Gias.Stalla, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._width = "100px"
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Raggruppamento_Des", Gias.Gruppo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._width = "200px"
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Animale_progetto", Gias.Capo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Animale_Distinta", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_EsercizioCapo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Prezzo_Unitario", Gias.PrezzoUnitario, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n6"
        l.Add(c)

        c = New ColonneNome("Qta", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_QuantitaSommabile, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        c._sum = True
        l.Add(c)

        c = New ColonneNome("QtaString", Gias.Quantita, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("PesoPagato", Gias.PesoPagato, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("PesoArrivo", Gias.PesoArrivo, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("PesoUscito", Gias.PesoUscito, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        l.Add(c)

        'estraggo i nomi dei tre gruppi dalla rispettiva tabella
        dtTipologie = objTipologie.Trova_Tipologie(IIf(_aziende.Contains("|"), "", _aziende))

        For Each dtRow As DataRow In dtTipologie.Rows
            Select Case dtRow.Item("Tipologia_Attivita")
                Case 1 : gruppo1 = dtRow.Item("Tipologia_Attivita_Desc")
                Case 2 : gruppo2 = dtRow.Item("Tipologia_Attivita_Desc")
                Case Else : gruppo3 = dtRow.Item("Tipologia_Attivita_Desc")
            End Select
        Next

        c = New ColonneNome("Des_Attivita_Gruppo1", gruppo1, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Des_Attivita_Gruppo2", gruppo2, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Des_Attivita_Gruppo3", gruppo3, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        'c = New ColonneNome("Costi_Ricavi_Des", "Costo - Ricavo", "string")
        'c._Editabile = False
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = False
        'l.Add(c)

        c = New ColonneNome("Ordine_Attivita", My.Resources.AgronicaCoreContabBIZ.OrdineAttivita, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Piva_Padre", My.Resources.AgronicaCoreContabBIZ.PartitaIvaAziendaPrincipale, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Azienda_Padre", My.Resources.AgronicaCoreContabBIZ.AziendaPrincipale, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        If costi_ricavi = 0 Then
            c = New ColonneNome("Valore", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_CostoTotale, "number")
        Else
            c = New ColonneNome("Valore", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_RicavoTotale, "number")
        End If
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        c._sum = True
        l.Add(c)

        c = New ColonneNome("Percentuale_Ripart", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_PercentualeRiparto, "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n2"
        c._sum = False
        l.Add(c)

        c = New ColonneNome("Data_Split", My.Resources.AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ_DataSplit, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        c._width = "90px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Costi_Ricavi_Des", My.Resources.AgronicaCoreContabBIZ.CostoRicavo, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        l.Add(c)

        xOrderBy = "Ordine_Attivita, Attivita"

        Dim js As New JSON_DataTable
        js.Editabile_Deafault = False
        risposta = js.JSON_DataTable_KendoOpt(dtCostiRicavi, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True,
                                              tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa,
                                              inParallelo:=True, errorString:=errorString, filtro_Ordinamento:=xOrderBy)

        Return risposta

    End Function


    Public Function Esegui_Aggiorna_Motorino_Visite(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim listaPar As List(Of String) = _Configurazione_Servizio.Parametri_Extra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)
        Next

        Dim piva = ht("piva")

        If piva = "" Then
            'Lettura di tutte le aziende
            Dim ClassJoin As New JoinFiltrone
            Dim classFiltrone As New AgronicaCoreUtility.Filtrone

            ClassJoin.bGerarchiaImprese = True
            classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)

            Dim Dt_Imprese = classFiltrone.CreaDTFiltrone(_objParametriServer,
                                                        "",
                                                        enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                        "",
                                                        ClassJoin)
            If Not IsNothing(Dt_Imprese) Then
                If Dt_Imprese.Rows.Count <> 0 Then
                    piva = ""
                    For i = 0 To Dt_Imprese.Rows.Count - 1
                        If piva <> "" Then
                            piva &= "|"
                        End If
                        piva &= Dt_Imprese.Rows(i).Item("PIVA")
                    Next
                End If
            End If
        End If


        Dim errori = New List(Of String)
        Dim errMessage = ""

        'Consuntivo
        errMessage = Aggiorna_Motorino_Visite(piva, _objParametriServer, _objParametriUtente)


        If Not String.IsNullOrEmpty(errMessage) Then
            Messaggio_di_Ritorno_Opzionale = errMessage
            Return False
        End If
        Return True

    End Function


    Public Function Aggiorna_Motorino_Visite(ByVal _aziende As String, ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim msgErrore As String = ""
        Dim scrivi_BIZ As New CDG_BIZ_W

        ' Filtro Aziende: se presenti prevalgono sulla partita iva
        If Not String.IsNullOrEmpty(_aziende) Then
            Dim Aziende As String() = _aziende.Split("|")
            For Each piva In Aziende
                If msgErrore <> "" Then
                    msgErrore &= "<br/>"
                End If
                msgErrore &= scrivi_BIZ.AllineaCosti_BombardinoMultiploVisite(piva, objParametri_Server, objParametri_Utenti)
            Next
        End If

        Return msgErrore

    End Function



    Public Function Esegui_Aggiorna_DW_Costi(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean
        Dim bGestioneMotorino As Boolean = False
        Dim EseguiMotorino As Boolean = True

        Dim listaPar As List(Of String) = _Configurazione_Servizio.Parametri_Extra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)

            If LCase(key) = "motorino" Then
                bGestioneMotorino = True
            End If

        Next

        Dim piva = ht("piva")
        Dim budget = 0
        If Not String.IsNullOrEmpty(("budget")) Then
            budget = CInt(ht("budget"))
        End If

        If bGestioneMotorino Then
            EseguiMotorino = CBool(ht("motorino"))
        Else
            'Se non specificato il parametro --> avvia sempre il motorino
        End If


        If piva = "" Then
            'Lettura di tutte le aziende
            Dim ClassJoin As New JoinFiltrone
            Dim classFiltrone As New AgronicaCoreUtility.Filtrone

            ClassJoin.bGerarchiaImprese = True
            classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)

            Dim Dt_Imprese = classFiltrone.CreaDTFiltrone(_objParametriServer,
                                                        "",
                                                        enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                        "",
                                                        ClassJoin)
            If Not IsNothing(Dt_Imprese) Then
                If Dt_Imprese.Rows.Count <> 0 Then
                    piva = ""
                    For i = 0 To Dt_Imprese.Rows.Count - 1
                        If piva <> "" Then
                            piva &= "|"
                        End If
                        piva &= Dt_Imprese.Rows(i).Item("PIVA")
                    Next
                End If
            End If
        End If


        Dim errori = New List(Of String)
        Dim errMessage = ""

        Select Case budget

            Case -2

                'Consuntivo
                errMessage = Aggiorna_DW_Costi(piva, 0, _objParametriServer, _objParametriUtente, EseguiMotorino)

                'Budget
                errMessage = errMessage & Aggiorna_DW_Costi(piva, -1, _objParametriServer, _objParametriUtente, False)

            Case 0

                'Consuntivo
                errMessage = Aggiorna_DW_Costi(piva, 0, _objParametriServer, _objParametriUtente, EseguiMotorino)

            Case Else

                'Budget Puntuale
                errMessage = Aggiorna_DW_Costi(piva, budget, _objParametriServer, _objParametriUtente, False)

        End Select



        If Not String.IsNullOrEmpty(errMessage) Then
            Messaggio_di_Ritorno_Opzionale = errMessage
            Return False
        End If
        Return True

    End Function


    Public Function Aggiorna_DW_Costi(ByVal _aziende As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri,
             ByRef objParametri_Utenti As AgronicaCoreParametri,
             Optional ByVal EseguiMotorino As Boolean = True
             ) As String

        Dim msgErrore As String = ""

        ' Filtro Aziende: se presenti prevalgono sulla partita iva
        If Not String.IsNullOrEmpty(_aziende) Then
            Dim Aziende As String() = _aziende.Split("|")
            For Each piva In Aziende
                If msgErrore <> "" Then
                    msgErrore &= "<br/>"
                End If
                msgErrore &= Esegui_Aggiornamento(piva, budget, objParametri_Server, objParametri_Utenti,, EseguiMotorino)
            Next
        End If

        Return msgErrore

    End Function

    Public Function Esegui_Aggiornamento(ByVal piva As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri,
             ByRef objParametri_Utenti As AgronicaCoreParametri, Optional ByVal _aziende As String = "", Optional ByVal EseguiMotorino As Boolean = True
                                  ) As String

        Dim msgErrore As String = ""
        Dim Dummy As String

        Try
            'Creazione Oggetti
            Dim leggi As New CDG_DAL_R
            Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
            Dim scrivi_BIZ As New CDG_BIZ_W
            Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W
            Dim scrivi_DAL As New CDG_DAL_W
            Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim objAttivita As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim prezzo_unitario As Decimal
            Dim data_riferimento_prezzi As DateTime
            Dim dt As DataTable
            Dim dt_Costi_Anagrafica_Macchine As DataTable
            Dim dt_Costi_Anagrafica_Macchine_Budget As DataTable
            Dim dt_QualifichexTariffe As DataTable
            Dim dt_QualifichexTariffe_Budget As DataTable
            Dim Dr_Costi_List() As DataRow
            Dim Id_Cfg_DW As Integer
            Dim Tipo_Valorizzazione As String

            Dim Costi_Ricavi As Integer

            Id_Cfg_DW = 0
            Costi_Ricavi = 0



            '##################################################################################################################################################
            '##########################  Allineamento con dati di campagna ##################################################################################
            '##################################################################################################################################################

            ' Il bombardino e l'allineamento del DW vengono eseguiti solo se è abilitato il permesso di Salva e vai ai costi del QdC che di fatto indica che 
            ' è attiva la nuova versione dei costi. Altrimenti li andremmo a creare doppi
            ' 10/3/2022 Nessuna esecuzione se stiamo gestendo il budget

            'Dim leggi_CDG_R As New CDG_BIZ_R
            Dim scrivi_CDG_W As New CDG_BIZ_W
            'Dim tipoCdG = leggi_CDG_R.GetTipoCdG(piva, objParametri_Server, objParametri_Utenti)

            'Sospeso Controllo Tipo CDG
            'If tipoCdG = enum_TipoCdG.NuovoTipo Then

            If budget = 0 And EseguiMotorino Then
                scrivi_BIZ.AllineaCosti_BombardinoMultiplo(piva, objParametri_Server, objParametri_Utenti)
                scrivi_BIZ.AllineaCosti_BombardinoMultiploVisite(piva, objParametri_Server, objParametri_Utenti)
            End If

                '##################################################################################################################################################
                '###################################### TRAVASO ###################################################################################################
                '##################################################################################################################################################                

                'Delete vecchia importazione dati
                Dummy = scrivi_DW.DW_Delete(piva, 0, budget, objParametri_Server)

                'Travaso Da Tabelle CDG a DW
                Dummy = scrivi_DW.DW_Travaso_CDG(piva, Id_Cfg_DW, budget, objParametri_Server, objParametri_Utenti)

                '##################################################################################################################################################
                '########################## VALORIZZAZIONE PRODOTTI ###############################################################################################
                '##################################################################################################################################################

                '---------------------------------------------
                'Considero solo i movimenti degli ultimi due anni
                '---------------------------------------------
                Dim DW_DataUltimaModifica As Date = Date.Now.AddYears(-2)

                Dim parteFissaFiltroAggiuntivo As String = " (Budget_Cons = " & budget.ToString & " And (prezzo_unitario = 0 Or valore = 0 Or prezzo_unitario * qta != valore Or data_inserimento >= '" & CStr(DW_DataUltimaModifica) & "'))"

                'Lettura dati prodotti Presenti in Tabelle CDG di cui aggiornare il costo
                Dim xFiltroAggiuntivo As String = " ( " & parteFissaFiltroAggiuntivo & " And mac_cod = 0 And elem_Cod <> 0 And cod_risum = 0 )"
                dt = leggi_DW.Leggi_DW_CDG(piva, Id_Cfg_DW, objParametri_Server, xFiltroAggiuntivo, budget)

                If dt.Rows.Count > 0 Then

                    ''TIPO_VALORIZZAZIONE (Impostazione_Cod = 846)
                    '1 = COSTO MEDIA PONDERATA
                    '2 = RICAVO MEDIO PONDERATO
                    '3 = Costo / Ricavo da tabella Prodotti_Costi (non gestita qui)
                    '4 = ULTIMO COSTO
                    '5 = ULTIMO RICAVO

                    'Cerco il tipo valorizzazione costi da applicare 
                    '(default da tabella Prodotti_Costi)
                    Dim Tipo_Valorizzazione_Costi As String() = {"3"}

                    Dim objImpost As New Utenti_Impostazioni_Read
                    Dim Dt_Impost = objImpost.Leggi2(
                                            2, objParametri_Server.SuperUserUsername,
                                            enum_Impostazioni_Utenti.SUPERUSER_TipoValorizzazioneCostiCdG,
                                            "", "", objParametri_Utenti)
                    If Not IsNothing(Dt_Impost) AndAlso Dt_Impost.Rows.Count > 0 Then
                        Tipo_Valorizzazione_Costi = CStr(Dt_Impost.Rows(0).Item("Impostazione_Valore_1")).Split(",")
                    End If

                    'DEBUG Dim stopWatch As New Stopwatch

                    For Each dr As DataRow In dt.Rows

                        'DEBUG stopWatch = New Stopwatch
                        'DEBUG stopwatch.Start()

                        Try

                            If dr.Item("Elem_Cod") = 501 Then

                            Else

                                prezzo_unitario = 0

                                Select Case CInt(dr.Item("Costi_Ricavi"))

                                    Case 0

                                        ' Se siamo nel budget cerca prima sui costi di anagrafica (Prodotti_Costi) con la versione di budget
                                        ' Se non trova niente cerca poi con l'algoritmo da impostazione_utenti sui costi reali
                                        If budget <> 0 Then
                                            data_riferimento_prezzi = dr("Data_Inserimento")
                                            prezzo_unitario = leggi_DW.Leggi_ValorizzazioneProdotto(3, piva,
                                                 dr.Item("Elem_Cod"), dr.Item("Pro_Cod"),
                                                 dr.Item("Mat_Cod"), dr.Item("Udm_Cod"), dr.Item("Lotto"),
                                                 AGRODATAINIZIO, data_riferimento_prezzi, budget,
                                                 objParametri_Server)
                                        End If

                                        If prezzo_unitario = 0 Then
                                            ' Per il c.m.p. utilizzo DateTime.Now al posto di dr("Data_Inserimento"), altrimenti non riuscirei a trovare il c.m.p. 
                                            ' nei casi in cui la fattura è arrivata dopo all'utilizzo prodotto e il DDT non era valorizzato
                                            For Each Tipo_Valorizzazione In Tipo_Valorizzazione_Costi
                                                If CInt(Tipo_Valorizzazione) = 3 Then
                                                    data_riferimento_prezzi = dr("Data_Inserimento")
                                                Else
                                                    data_riferimento_prezzi = DateTime.Now
                                                End If
                                                prezzo_unitario = leggi_DW.Leggi_ValorizzazioneProdotto(CInt(Tipo_Valorizzazione), piva,
                                                     dr.Item("Elem_Cod"), dr.Item("Pro_Cod"),
                                                     dr.Item("Mat_Cod"), dr.Item("Udm_Cod"), dr.Item("Lotto"),
                                                     AGRODATAINIZIO, data_riferimento_prezzi, 0,
                                                     objParametri_Server)
                                                If prezzo_unitario <> 0 Then
                                                    Exit For
                                                End If
                                            Next
                                        End If

                                    Case 1
                                        'TODO Ancora da gestire


                                End Select

                                'Aggiornamento Valore
                                If CDec(dr.Item("Prezzo_Unitario")) <> prezzo_unitario OrElse
                                    CDec(dr.Item("Valore")) <> CDec(prezzo_unitario * dr.Item("Qta")) Then
                                    Dummy = scrivi_DW.DW_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_Cfg_DW"), dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)
                                End If
                            End If

                            'DEBUG If stopWatch.IsRunning Then
                            'DEBUG     stopWatch.Stop()
                            'DEBUG     Dim ts = stopWatch.Elapsed
                            'DEBUG End If

                        Catch ex As Exception

                            msgErrore = msgErrore & " Errore di Valorizzazione Id_CDG : " & CInt(dr.Item("Id_CDG"))

                        End Try

                    Next

                End If

                '##################################################################################################################################################
                '################################## VALORIZZAZIONE PERSONE ########################################################################################
                '##################################################################################################################################################


                'Lettura dati persone Presenti in Tabelle CDG di cui aggiornare il costo
                xFiltroAggiuntivo = " ( " & parteFissaFiltroAggiuntivo & " And mac_cod = 0 And elem_Cod = 0  And cod_risum <> 0 )"
                dt = leggi_DW.Leggi_DW_CDG(piva, Id_Cfg_DW, objParametri_Server, xFiltroAggiuntivo, budget)

                If dt.Rows.Count > 0 Then

                    'Lettura QualifichexTariffe
                    If budget <> 0 Then
                        'Lettura Tariffe Budget
                        dt_QualifichexTariffe_Budget = objQualifichexTariffe.Leggi(piva, 0, 0, 0, "", "", objParametri_Server, 1, budget)
                    Else
                        dt_QualifichexTariffe_Budget = Nothing
                    End If

                    'Lettura Tariffe Effettivi
                    dt_QualifichexTariffe = objQualifichexTariffe.Leggi(piva, 0, 0, 0, "", "", objParametri_Server)

                    CalcolaTariffaPersona(piva, dt, dt_QualifichexTariffe_Budget, dt_QualifichexTariffe, objParametri_Server)
                End If

                '##################################################################################################################################################
                '################################## VALORIZZAZIONE MACCHINE #######################################################################################
                '##################################################################################################################################################

                'Lettura dati macchine Presenti in Tabelle CDG di cui aggiornare il costo
                xFiltroAggiuntivo = " ( " & parteFissaFiltroAggiuntivo & " And mac_cod <> 0 And elem_Cod = 0  And cod_risum = 0 )"
                dt = leggi_DW.Leggi_DW_CDG(piva, Id_Cfg_DW, objParametri_Server, xFiltroAggiuntivo, budget)

                If dt.Rows.Count > 0 Then

                    If budget <> 0 Then
                        'Lettura Costi Macchina Budget
                        dt_Costi_Anagrafica_Macchine_Budget = leggi_DW.Ipno_Costo_Anagrafica(piva, 1, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", budget, objParametri_Server)
                    Else
                        dt_Costi_Anagrafica_Macchine_Budget = Nothing
                    End If

                    'Lettura Costi Macchina Effettivi
                    dt_Costi_Anagrafica_Macchine = leggi_DW.Ipno_Costo_Anagrafica(piva, 1, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", 0, objParametri_Server)

                    CalcolaTariffaMacchina(piva, dt, dt_Costi_Anagrafica_Macchine_Budget, dt_Costi_Anagrafica_Macchine, objParametri_Server)

                End If

            ' End If

            'If tipoCdG <> enum_TipoCdG.NuovoTipo Then
            '    If tipoCdG = 1 Then
            '        'Import costi vecchia modalità di inserimento
            '        Dim strJSON As String = leggi_CDG_R.GeneraJSON_OperazioniCampagna_AggiuntiModificati(piva, budget, objParametri_Server)

            '        Dim dtDaCancellare As DataTable = leggi_CDG_R.GeneraDT_OperazioniCampagna_Cancellati(piva, budget, objParametri_Server)

            '        msgErrore = scrivi_CDG_W.ExportCDGVecchioTipoBIZ(piva, strJSON, dtDaCancellare, objParametri_Server, objParametri_Utenti)

            '    Else
            '        msgErrore = "Non è stato impostato su tabella Profilazione_Dati il gruppo tipoCdG: contattare l'assistenza "

            '    End If
            'End If

        Catch ex As Exception

            msgErrore = msgErrore & " Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return msgErrore

    End Function

    Private Function CalcolaTariffaPersona(piva, dt_Elenco_Persone, dt_QualifichexTariffe_Budget, dt_QualifichexTariffe, objParametri_Server) As Decimal

        Dim prezzo_unitario As Decimal = 0
        Dim Dr_Costi_List As DataRow()

        Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W

        For Each dr As DataRow In dt_Elenco_Persone.Rows
            prezzo_unitario = 0

            If Not dt_QualifichexTariffe_Budget Is Nothing AndAlso dt_QualifichexTariffe_Budget.Rows.Count > 0 Then
                Dr_Costi_List = dt_QualifichexTariffe_Budget.Select("Tariffa_Cod = " & dr.Item("Tariffa_Cod") & " And Qualifica_Cod = " & dr.Item("Qualifica_Cod") & " ") 'FILTER
                prezzo_unitario = Aggiorna_Riga_Costo_Persona(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server)
            End If

            If prezzo_unitario = 0 AndAlso Not dt_QualifichexTariffe Is Nothing AndAlso dt_QualifichexTariffe.Rows.Count > 0 Then
                Dr_Costi_List = dt_QualifichexTariffe.Select("Tariffa_Cod = " & dr.Item("Tariffa_Cod") & " And Qualifica_Cod = " & dr.Item("Qualifica_Cod") & " ") 'FILTER
                prezzo_unitario = Aggiorna_Riga_Costo_Persona(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server)
            End If

        Next

    End Function

    Private Function Aggiorna_Riga_Costo_Persona(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server) As Decimal

        Dim prezzo_unitario As Decimal = 0
        If Not IsNothing(Dr_Costi_List) AndAlso Dr_Costi_List.Length > 0 Then
            For j = 0 To Dr_Costi_List.Length - 1

                If CDate(dr.Item("Data_Inserimento")) >= CDate(Dr_Costi_List(j).Item("Validita_Inizio")) And
                            CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Validita_Fine")) Then

                    'Aggiorno solo se il costo è cambiato dopo la data di inserimento CdG oppure se il costo è a 0
                    If dr.Item("Valore") = 0 OrElse dr.Item("Prezzo_Unitario") = 0 OrElse
                                CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Data_Modifica")) Then

                        prezzo_unitario = CDec(Dr_Costi_List(j).Item("Valore"))

                        'Aggiornamento Valore
                        If CDec(dr.Item("Prezzo_Unitario")) <> prezzo_unitario OrElse
                                    CDec(dr.Item("Valore")) <> CDec(prezzo_unitario * dr.Item("Qta")) Then
                            Dim Dummy = scrivi_DW.DW_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_Cfg_DW"), dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)
                        End If

                        Return prezzo_unitario

                    End If

                End If

            Next
        End If

    End Function

    Private Function CalcolaTariffaMacchina(piva, dt_Elenco_Macchine, dt_Costi_Anagrafica_Macchine_Budget, dt_Costi_Anagrafica_Macchine, objParametri_Server)

        Dim prezzo_unitario As Decimal = 0
        Dim Dr_Costi_List As DataRow()

        Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W

        For Each dr As DataRow In dt_Elenco_Macchine.Rows
            prezzo_unitario = 0

            If Not dt_Costi_Anagrafica_Macchine_Budget Is Nothing AndAlso dt_Costi_Anagrafica_Macchine_Budget.Rows.Count > 0 Then
                Dr_Costi_List = dt_Costi_Anagrafica_Macchine_Budget.Select("Mat_Cod = " & dr.Item("Mac_Cod") & " ") 'FILTER
                prezzo_unitario = Aggiorna_Riga_Costo_Macchina(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server)
            End If

            If prezzo_unitario = 0 AndAlso Not dt_Costi_Anagrafica_Macchine Is Nothing AndAlso dt_Costi_Anagrafica_Macchine.Rows.Count > 0 Then
                Dr_Costi_List = dt_Costi_Anagrafica_Macchine.Select("Mat_Cod = " & dr.Item("Mac_Cod") & " ") 'FILTER
                prezzo_unitario = Aggiorna_Riga_Costo_Macchina(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server)
            End If

        Next

    End Function

    Private Function Aggiorna_Riga_Costo_Macchina(piva, Dr_Costi_List, dr, scrivi_DW, objParametri_Server) As Decimal

        Dim prezzo_unitario As Decimal = 0

        If Not IsNothing(Dr_Costi_List) AndAlso Dr_Costi_List.Length > 0 Then
            For j = 0 To Dr_Costi_List.Length - 1

                If CDate(dr.Item("Data_Inserimento")) >= CDate(Dr_Costi_List(j).Item("Validita_Inizio")) And
                    CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Validita_Fine")) Then

                    'Aggiorno solo se il costo è cambiato dopo la data di inserimento CdG oppure se il costo è a 0
                    If dr.Item("Valore") = 0 OrElse dr.Item("Prezzo_Unitario") = 0 OrElse
                                    CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Data_Modifica")) Then

                        prezzo_unitario = CDec(Dr_Costi_List(j).Item("Prezzo_Unitario"))

                        'Aggiornamento Valore
                        If CDec(dr.Item("Prezzo_Unitario")) <> prezzo_unitario OrElse
                                                   CDec(dr.Item("Valore")) <> CDec(prezzo_unitario * dr.Item("Qta")) Then
                            Dim Dummy = scrivi_DW.DW_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_Cfg_DW"), dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)
                        End If

                        Return prezzo_unitario

                    End If

                End If
            Next

        End If

    End Function

    Public Function Esegui_Aggiorna_CdG_BI_Esterna(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean


        Dim listaPar As List(Of String) = _Configurazione_Servizio.Parametri_Extra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)
        Next

        Dim piva = ht("piva")
        Dim errori = New List(Of String)
        Dim errMessage = ""
        errMessage = Aggiorna_CdG_BI_Esterna(piva, _objParametriServer, _objParametriUtente)
        If Not String.IsNullOrEmpty(errMessage) Then
            Messaggio_di_Ritorno_Opzionale = errMessage
            Return False
        End If
        Return True

    End Function

    Public Function Aggiorna_CdG_BI_Esterna(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri,
             ByRef objParametri_Utenti As AgronicaCoreParametri
                                  ) As String

        Dim msgErrore As String = ""
        Dim risAllineaCostiDaCampagna As String = ""

        Dim Dummy As String

        Try
            'Creazione Oggetti
            Dim leggi As New CDG_DAL_R
            Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
            Dim scrivi_BIZ As New CDG_BIZ_W
            Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W
            Dim scrivi_DAL As New CDG_DAL_W
            Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim objAttivita As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim prezzo_unitario As Decimal
            Dim dt As DataTable
            Dim dt_Costi_Anagrafica_Macchine As DataTable
            Dim dt_QualifichexTariffe As DataTable
            Dim dt_Costi_Anagrafica_Prodotti As New DataTable
            Dim Dr_Costi_List() As DataRow
            Dim Tipo_Valorizzazione As Integer

            Dim Costi_Ricavi As Integer

            Costi_Ricavi = 0

            '##################################################################################################################################################
            '##########################  Allineamento con dati di campagna ##################################################################################
            '##################################################################################################################################################

            Dim leggi_CDG_R As New CDG_BIZ_R
            Dim scrivi_CDG_W As New CDG_BIZ_W
            Dim tipoCdG = leggi_CDG_R.GetTipoCdG(piva, objParametri_Server, objParametri_Utenti)

            If tipoCdG = enum_TipoCdG.NuovoTipo Then
                scrivi_BIZ.AllineaCosti_BombardinoMultiplo(piva, objParametri_Server, objParametri_Utenti)
                scrivi_BIZ.AllineaCosti_BombardinoMultiploVisite(piva, objParametri_Server, objParametri_Utenti)

                '##################################################################################################################################################
                '###################################### TRAVASO ###################################################################################################
                '##################################################################################################################################################

                'Delete vecchia importazione dati
                Dummy = scrivi_DW.CDG_BI_Esterna_Delete(piva, objParametri_Server)


                'Travaso Da Tabelle CDG a BI
                Dummy = scrivi_DW.CDG_BI_Esterna_Estrazione(piva, objParametri_Server, objParametri_Utenti)

                '##################################################################################################################################################
                '################################## VALORIZZAZIONE ################################################################################################
                '##################################################################################################################################################

                'Lettura dati Presenti in Tabelle CDG
                dt = leggi_DW.Leggi_CDG_BI_Esterna(piva, objParametri_Server)

                If dt.Rows.Count > 0 Then

                    'Cerco il tipo valorizzazione costi da applicare 
                    '(default da tabella Prodotti_Costi)
                    Dim Tipo_Valorizzazione_Costi = 3
                    Dim objImpost As New Utenti_Impostazioni_Read
                    Dim Dt_Impost = objImpost.Leggi2(
                                            2, objParametri_Server.SuperUserUsername,
                                            enum_Impostazioni_Utenti.SUPERUSER_TipoValorizzazioneCostiCdG,
                                            "", "", objParametri_Utenti)

                    If Not IsNothing(Dt_Impost) AndAlso Dt_Impost.Rows.Count > 0 Then
                        Tipo_Valorizzazione_Costi = CInt(Dt_Impost.Rows(0).Item("Impostazione_Valore_1"))
                    End If

                    'Lettura dt Costi Prodotti solo se utilizzato Tipo Valorizzazione relativo
                    If Tipo_Valorizzazione_Costi = 3 Then
                        dt_Costi_Anagrafica_Prodotti = leggi_DW.Ipno_Costo_Anagrafica(piva, 0, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, " Elem_Cod <> 0 And Elem_Cod <> 1 ", 0, objParametri_Server)
                    End If

                    'Lettura dt Costi Anagrafica
                    dt_Costi_Anagrafica_Macchine = leggi_DW.Ipno_Costo_Anagrafica(piva, 1, "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", 0, objParametri_Server)

                    'Lettura QualifichexTariffe
                    dt_QualifichexTariffe = objQualifichexTariffe.Leggi(piva, 0, 0, 0, "", "", objParametri_Server)

                    'Scorro il dt
                    'dt = JsonConvert.DeserializeObject(Risultato)
                    'dt.Select(" ") 'FILTER
                    For Each dr As DataRow In dt.Rows


                        If dr.Item("Elem_Cod") <> 0 Then


                            If dr.Item("Elem_Cod") = 501 Then


                            Else
                                Tipo_Valorizzazione = Tipo_Valorizzazione_Costi


                                If Tipo_Valorizzazione = 1 Then
                                    prezzo_unitario = leggi_DW.Ipno_Valorizzazione_Prodotto(Tipo_Valorizzazione, piva, dr.Item("Elem_Cod"), dr.Item("Pro_Cod"), dr.Item("Mat_Cod"), dr.Item("Udm_Cod"), 0, 0, 0, dr.Item("Lotto"), "", AGRODATAINIZIO, dr("Data_Inserimento"), objParametri_Server)

                                    'Aggiornamento Valore
                                    Dummy = scrivi_DW.CDG_BI_Esterna_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)

                                End If

                                If Tipo_Valorizzazione = 3 Then

                                    If dt_Costi_Anagrafica_Prodotti.Rows.Count > 0 Then

                                        Dr_Costi_List = dt_Costi_Anagrafica_Prodotti.Select("Elem_Cod = " & dr.Item("Elem_Cod") & " And Pro_Cod = " & dr.Item("Pro_Cod") & " And Mat_Cod = " & dr.Item("Mat_Cod") & " ") 'FILTER

                                        If Not IsNothing(Dr_Costi_List) AndAlso Dr_Costi_List.Length > 0 Then
                                            For j = 0 To Dr_Costi_List.Length - 1

                                                If CDate(dr.Item("Data_Inserimento")) >= CDate(Dr_Costi_List(j).Item("Validita_Inizio")) And CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Validita_Fine")) Then

                                                    'Aggiorno solo se il costo è cambiato dopo la data di inserimento CdG oppure se il costo è a 0
                                                    If dr.Item("Prezzo_Unitario") = 0 OrElse CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Data_Modifica")) Then

                                                        prezzo_unitario = Dr_Costi_List(j).Item("Prezzo_Unitario")

                                                        'Aggiornamento Valore
                                                        Dummy = scrivi_DW.CDG_BI_Esterna_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)

                                                        Exit For

                                                    End If

                                                End If


                                            Next
                                        End If

                                    End If


                                End If

                            End If

                        Else
                            If dr.Item("Cod_RisUm") <> 0 Then

                                If dt_QualifichexTariffe.Rows.Count > 0 Then

                                    Dr_Costi_List = dt_QualifichexTariffe.Select("Tariffa_Cod = " & dr.Item("Tariffa_Cod") & " And Qualifica_Cod = " & dr.Item("Qualifica_Cod") & " ") 'FILTER

                                    If Not IsNothing(Dr_Costi_List) AndAlso Dr_Costi_List.Length > 0 Then
                                        For j = 0 To Dr_Costi_List.Length - 1

                                            If CDate(dr.Item("Data_Inserimento")) >= CDate(Dr_Costi_List(j).Item("Validita_Inizio")) And CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Validita_Fine")) Then

                                                'Aggiorno solo se il costo è cambiato dopo la data di inserimento CdG oppure se il costo è a 0
                                                If dr.Item("Prezzo_Unitario") = 0 OrElse CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Data_Modifica")) Then

                                                    prezzo_unitario = CDec(Dr_Costi_List(j).Item("Valore"))

                                                    'Aggiornamento Valore
                                                    Dummy = scrivi_DW.CDG_BI_Esterna_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)

                                                    Exit For

                                                End If

                                            End If

                                        Next
                                    End If

                                End If

                            Else

                                If dr.Item("Mac_Cod") <> 0 Then

                                    'Parco Macchine

                                    If dt_Costi_Anagrafica_Macchine.Rows.Count > 0 Then

                                        Dr_Costi_List = dt_Costi_Anagrafica_Macchine.Select("Mat_Cod = " & dr.Item("Mac_Cod") & " ") 'FILTER

                                        If Not IsNothing(Dr_Costi_List) AndAlso Dr_Costi_List.Length > 0 Then
                                            For j = 0 To Dr_Costi_List.Length - 1

                                                If CDate(dr.Item("Data_Inserimento")) >= CDate(Dr_Costi_List(j).Item("Validita_Inizio")) And CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Validita_Fine")) Then

                                                    'Aggiorno solo se il costo è cambiato dopo la data di inserimento CdG oppure se il costo è a 0
                                                    If dr.Item("Prezzo_Unitario") = 0 OrElse CDate(dr.Item("Data_Inserimento")) <= CDate(Dr_Costi_List(j).Item("Data_Modifica")) Then

                                                        prezzo_unitario = Dr_Costi_List(j).Item("Prezzo_Unitario")

                                                        'Aggiornamento Valore
                                                        Dummy = scrivi_DW.CDG_BI_Esterna_Imposta_Valorizzazione_Prodotto(piva, dr.Item("Id_CDG"), dr.Item("Id_CDG_Dettagli"), prezzo_unitario, objParametri_Server)

                                                        Exit For

                                                    End If

                                                End If
                                            Next

                                        End If

                                    End If

                                End If
                            End If
                        End If

                    Next

                End If


            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            msgErrore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return msgErrore

    End Function



    Public Function Valorizzazione_Conferimento(ByVal Piva As String,
                                                ByVal Progetto_Cod As Integer,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Priorita As Integer,
                                                ByVal validita_inizio As String,
                                                ByVal validita_fine As String,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As Decimal


        Const nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Valorizzazione_Conferimento()"
        Dim messaggioErrore As String = ""

        Dim xFiltroAggiuntivo As String = ""
        Dim xOrderBy As String = ""

        Dim dtCostiRicavi As New DataTable
        Dim objCostiRicavi As New DW_CDG_Costi_Ricavi_DAL_R
        Dim ObjAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim ObjReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim Filtro_Aggiuntivo As String = ""
        Dim Produzione As Decimal = 0
        Dim Costo_Totale As Decimal = 0
        Dim Costo_Unitario As Decimal = 0

        Try

            'Controllo Impostazione Date
            If Not IsDate(validita_inizio) Then
                validita_inizio = AGRODATAINIZIO
            End If
            If Not IsDate(validita_fine) Then
                validita_fine = AGRODATAFINE
            End If


            dtCostiRicavi = objCostiRicavi.Leggi_Tabellone_DW_Valorizzazione(Piva,
                                                                             Progetto_Cod,
                                                                             Elem_Cod,
                                                                             Mat_Cod,
                                                                             Priorita,
                                                                             validita_inizio,
                                                                             validita_fine,
                                                                             Filtro_Aggiuntivo,
                                                                             True,
                                                                             xOrderBy,
                                                                             objParametri_Server)


            If dtCostiRicavi.Rows.Count > 0 Then

                'Determinazione Produzione della Distinta
                Filtro_Aggiuntivo = "Imprese_Progetti.Progetto_Cod = " & Progetto_Cod


                Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(Piva, 0, -1000, 0, validita_inizio, validita_fine, objParametri_Server, Filtro_Aggiuntivo) +
                               ObjReg_Impianti.SommaProduzione_SPV(Piva, -1000, 0, validita_inizio, validita_fine, objParametri_Server, Mat_Cod, Filtro_Aggiuntivo)


                'Controllo che la produzione a TON sia valorizzata
                If Produzione <> 0 Then

                    For Each dr As DataRow In dtCostiRicavi.Rows
                        Costo_Totale = Costo_Totale + dr("Valore")
                    Next

                    Costo_Unitario = (Costo_Totale / Produzione) / 1000

                End If

            End If


            Return Costo_Unitario
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


    End Function



    Public Function Ricerca_DT_SPV(ByRef dtReport1 As DataTable,
                                   ByRef dtReport2 As DataTable,
                                   ByRef dtReport3 As DataTable,
                                   ByVal _azienda As String,
                                   ByVal validita_inizio As String,
                                   ByVal validita_fine As String,
                                   ByVal validita_fine_cdg As String,
                                   ByVal id_report As Integer,
                                   ByVal personale As Integer,
                                   ByVal includiAziendeFiglie As Integer,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByVal DettaglioSpecieVarieta As Integer,
                                   ByVal includiLav_Des As Integer,
                                   Optional ByVal Id_Budget As Integer = 0,
                                   Optional ByVal Filtro_Aggiuntivo As String = ""
                             )

        Const nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Ricerca_DT_SPV()"
        Dim messaggioErrore As String = ""

        Dim filtro_azienda_padre As String
        Dim filtro_azienda As String
        Dim budget As Integer = 0
        Dim costi_ricavi As Integer = 0
        Dim DataRifProgetto As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim filtro_codice_appezzamento As String = ""
        Dim filtro_codice_impianto As String = ""
        Dim lancioPivot As Integer = 0
        Dim xOrderBy As String = ""
        Dim xOrderByDW As String = ""
        Dim Count As Integer = 0
        Dim strKey As String = ""
        Dim strKey_sub As String = ""
        Dim Veg_Cod As Integer = 0
        Dim Cul_Cod As Integer = 0
        Dim Valore_Ripartito As Decimal = 0

        Dim dtCostiRicavi As New DataTable
        Dim objCostiRicavi As New DW_CDG_Costi_Ricavi_DAL_R
        Dim ObjAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim ObjReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim DtDati As New DataTable
        Dim DtDati2 As New DataTable
        Dim dr_search() As DataRow
        Dim dr_search2() As DataRow
        Dim dr_attivita As DataRow
        Dim dr_search_attivita() As DataRow
        Dim dr_search_agenda() As DataRow
        Dim dr_search_mat() As DataRow
        Dim dr_search_impianto() As DataRow
        Dim dr_Intest As DataRow
        Dim dr_SAU As DataRow

        Dim dataStep As New DataView
        Dim dataStepDest As New DataTable

        Dim Piva As String = ""
        Dim Piva_Attivita As String = ""

        Dim Filtro_Piva As String = ""
        Dim Filtro_Veg_Cod As String = ""
        Dim Filtro_Veg_Cod_Residui As String = ""
        Dim Intest(,) As String
        Dim Id_Attivita As Integer = 0

        Dim DtProduzione As New DataTable
        Dim DtProduzioneAll As New DataTable
        Dim DtImpianti As New DataTable
        Dim Produzione As Decimal = 0

        Dim Des_Attivita_Gruppo1_Last As String = "ZZZ"
        Dim Des_Attivita_Gruppo2_Last As String = "ZZZ"

        Dim Des_Attivita_Gruppo1 As String = ""
        Dim Des_Attivita_Gruppo2 As String = ""

        Dim Mat_Cod As Integer = 0
        Dim Mat_Des As String = ""
        Dim Priorita As Integer = 0
        Dim Mat_Cod_Raccolta As Integer = 0

        Dim Totale_Qta_Raccolta As Decimal = 0
        Dim Totale_Sup_Raccolta As Decimal = 0
        Dim Totale_Sup_Imp As Decimal = 0
        Dim Totale_Sup_Imp_Raccolti As Decimal = 0

        Dim Totale_Qta_Raccolta_Mat As Decimal = 0
        Dim Totale_Sup_Raccolta_Mat As Decimal = 0

        Dim num_record As Integer = 0
        Dim i_record As Integer = 0
        Dim i_count As Integer = 0
        Dim bInsertColtura_NO_Costi As Boolean = False
        Dim bOk As Boolean = False
        Dim bLeggiDW As Boolean = False
        Dim strErrore As String = ""
        Dim Last_Impianto As String = ""
        Dim Search_Impianto As String = ""
        Dim strFiltro As String = ""
        Dim dtIntestazione As New DataTable
        Dim dtFinale As New DataTable
        Dim dtMercato As New DataTable
        Dim dtEnergia As New DataTable
        Dim dtSAU As New DataTable

        Dim bBypassDistinteNonPresenti As Boolean = False
        Dim Prodotto_Raccolto As String = ""
        Dim Data_Validita_Inizio As String = ""
        Try


            'personale = 1
            'includiLav_Des = 1 'Raggruppamento per lav_des


            'Controllo Impostazione Date
            If Not IsDate(validita_inizio) Then
                validita_inizio = AGRODATAINIZIO
            End If
            If Not IsDate(validita_fine) Then
                validita_fine = AGRODATAFINE
            End If


            Select Case id_report

                Case 1 'SPV
                    xOrderByDW = "Ordine_Attivita, Ragione_Sociale_Azienda"
                    bLeggiDW = True
                    bBypassDistinteNonPresenti = True
                    bOk = False
                Case 2 'SPV per Coltura
                    xOrderByDW = "Ordine_Attivita, Ragione_Sociale_Azienda, Piva, Veg_Cod, Id_Imputazione"
                    bLeggiDW = True
                    bBypassDistinteNonPresenti = False 'Viene compilato un campo di errore
                    bOk = False
                Case 3 'SPV Produzione Coltura Campo
                    bLeggiDW = True
                    bOk = True
                    bBypassDistinteNonPresenti = True
                Case 4 'SPV Piano colturale
                    bLeggiDW = True
                    bOk = True
                    bBypassDistinteNonPresenti = True
            End Select


            Select Case includiAziendeFiglie
                Case 0 '-->niente figli
                    filtro_azienda = _azienda
                    filtro_azienda_padre = ""
                Case Else
                    filtro_azienda = ""
                    filtro_azienda_padre = _azienda
            End Select


            If bLeggiDW Then

                xFiltroAggiuntivo = " Des_Attivita_Gruppo1 <> '' "

                'Concatenazione Filtro Aggiuntivo
                If Trim(Filtro_Aggiuntivo) <> "" Then
                    xFiltroAggiuntivo = xFiltroAggiuntivo & " And " & Filtro_Aggiuntivo
                End If


                Data_Validita_Inizio = objCostiRicavi.Determina_Validita_Inizio_Report(filtro_azienda, filtro_azienda_padre, Id_Budget, costi_ricavi, validita_inizio, validita_fine_cdg, objParametri_Server)

                dtCostiRicavi = objCostiRicavi.Leggi_Tabellone_DW_Report(filtro_azienda,
                                                                        Id_Budget, costi_ricavi, Data_Validita_Inizio,
                                                                        validita_fine_cdg, DataRifProgetto,
                                                                        xFiltroAggiuntivo,
                                                                        filtro_codice_appezzamento,
                                                                        filtro_codice_impianto,
                                                                        filtro_azienda_padre,
                                                                        bBypassDistinteNonPresenti,
                                                                        xOrderBy,
                                                                        objParametri_Server)


                If dtCostiRicavi.Rows.Count > 0 Then
                    bOk = True
                End If

            End If

            If bOk Then

                Select Case id_report

                    Case 1

                        '####################################################################################################
                        '################################### REPORT SPV #####################################################
                        '####################################################################################################


                        'Costruzione DataTable Intestazione
                        dtIntestazione.Columns.Add(New DataColumn("Des_Attivita_Gruppo1", GetType(String)))
                        dtIntestazione.Columns.Add(New DataColumn("Des_Attivita_Gruppo2", GetType(String)))
                        dtIntestazione.Columns.Add(New DataColumn("Id_Attivita", GetType(Integer)))
                        dtIntestazione.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
                        dtIntestazione.Columns.Add(New DataColumn("Lav_Cod_Agenda", GetType(Integer)))
                        dtIntestazione.Columns.Add(New DataColumn("Lav_Des_Agenda", GetType(String)))

                        'Inserimento Record HA  
                        dr_Intest = dtIntestazione.NewRow

                        dr_Intest.Item("Des_Attivita_Gruppo1") = ""
                        dr_Intest.Item("Des_Attivita_Gruppo2") = ""

                        dr_Intest.Item("Id_Attivita") = 0
                        dr_Intest.Item("Attivita_Des") = "HA"

                        dtIntestazione.Rows.Add(dr_Intest)

                        'Inserimento Record TON
                        dr_Intest = dtIntestazione.NewRow

                        dr_Intest.Item("Des_Attivita_Gruppo1") = ""
                        dr_Intest.Item("Des_Attivita_Gruppo2") = ""

                        dr_Intest.Item("Id_Attivita") = 0
                        dr_Intest.Item("Attivita_Des") = "TON"

                        dtIntestazione.Rows.Add(dr_Intest)


                        'Costruzione DataTable
                        dtFinale.Columns.Add(New DataColumn("Des_Attivita_Gruppo1", GetType(String)))
                        dtFinale.Columns.Add(New DataColumn("Des_Attivita_Gruppo2", GetType(String)))
                        dtFinale.Columns.Add(New DataColumn("Id_Attivita", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
                        'dtFinale.Columns.Add(New DataColumn("Frazionabile", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Lav_Cod_Agenda", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Lav_Des_Agenda", GetType(String)))


                        '=====================================================================================================
                        'Step 0 Definizione Intestazione
                        '-----------------------------------------------------------------------------------------------------

                        ReDim Intest(3, 0)

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = dtCostiRicavi.Select("Piva = '" & _azienda & "'")

                        If dr_search.Length <> 0 Then

                            Filtro_Piva = Filtro_Piva & IIf(Filtro_Piva = "", "", ", ") & "'" & Agro_SQL_SaveText(_azienda) & "'"

                            ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)

                            Intest(0, UBound(Intest, 2) - 1) = dr_search(0).Item("Piva")
                            Intest(1, UBound(Intest, 2) - 1) = dr_search(0).Item("Ragione_Sociale_Azienda")

                            'Inserimento Colonna
                            dtIntestazione.Columns.Add(New DataColumn(_azienda, GetType(Decimal)))
                            dtFinale.Columns.Add(New DataColumn(_azienda, GetType(Decimal)))

                            'Impostazione  Valori
                            dtIntestazione(0).Item("Des_Attivita_Gruppo1") = ""
                            dtIntestazione(0).Item("Des_Attivita_Gruppo2") = ""
                            dtIntestazione(1).Item("Des_Attivita_Gruppo1") = ""
                            dtIntestazione(1).Item("Des_Attivita_Gruppo2") = ""

                            dtIntestazione(0).Item(dr_search(0).Item("Piva")) = ObjReg_Impianti.SommaSuperficie_SPV(dr_search(0).Item("Piva"), -1000, 0, validita_inizio, validita_fine, 0, False, objParametri_Server)

                            Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(dr_search(0).Item("Piva"), Id_Budget, -1000, 0, validita_inizio, validita_fine, objParametri_Server) +
                                         IIf(Id_Budget = 0, ObjReg_Impianti.SommaProduzione_SPV(dr_search(0).Item("Piva"), -1000, 0, validita_inizio, validita_fine, objParametri_Server, 0), 0)


                            dtIntestazione(1).Item(dr_search(0).Item("Piva")) = Produzione


                        Else

                            Filtro_Piva = Filtro_Piva & IIf(Filtro_Piva = "", "", ", ") & "'" & Agro_SQL_SaveText(_azienda) & "'"

                            ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)

                            Intest(0, UBound(Intest, 2) - 1) = _azienda
                            Intest(1, UBound(Intest, 2) - 1) = ""


                            'Inserimento Colonna
                            dtIntestazione.Columns.Add(New DataColumn(_azienda, GetType(Decimal)))
                            dtFinale.Columns.Add(New DataColumn(_azienda, GetType(Decimal)))


                        End If

                        For Each dr As DataRow In dtCostiRicavi.Rows

                            Piva = dr.Item("Piva")

                            If InStr(Filtro_Piva, "'" & Piva & "'") = 0 Then

                                'Costruzione Filtro_Piva
                                Filtro_Piva = Filtro_Piva & IIf(Filtro_Piva = "", "", ", ") & "'" & Agro_SQL_SaveText(Piva) & "'"

                                'Inserimento Array Intestazioni
                                ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)

                                Intest(0, UBound(Intest, 2) - 1) = Piva
                                Intest(1, UBound(Intest, 2) - 1) = dr("Ragione_Sociale_Azienda")

                                'Inserimento Colonna in DataTable
                                dtIntestazione.Columns.Add(New DataColumn(Piva, GetType(Decimal)))
                                dtFinale.Columns.Add(New DataColumn(Piva, GetType(Decimal)))

                                'Impostazione  Valori
                                dtIntestazione(0).Item("Des_Attivita_Gruppo1") = ""
                                dtIntestazione(0).Item("Des_Attivita_Gruppo2") = ""
                                dtIntestazione(1).Item("Des_Attivita_Gruppo1") = ""
                                dtIntestazione(1).Item("Des_Attivita_Gruppo2") = ""

                                dtIntestazione(0).Item(Piva) = ObjReg_Impianti.SommaSuperficie_SPV(Piva, -1000, 0, validita_inizio, validita_fine, 0, False, objParametri_Server)

                                Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(Piva, Id_Budget, -1000, 0, validita_inizio, validita_fine, objParametri_Server) +
                                             IIf(Id_Budget = 0, ObjReg_Impianti.SommaProduzione_SPV(Piva, -1000, 0, validita_inizio, validita_fine, objParametri_Server, 0), 0)


                                dtIntestazione(1).Item(Piva) = Produzione

                            End If

                        Next
                        '=====================================================================================================



                        '=====================================================================================================
                        'Step 1 Inserimento Costi Personale
                        '-----------------------------------------------------------------------------------------------------
                        If personale = 1 Then

                            xFiltroAggiuntivo = "Cod_Risum <> 0"

                            'Passaggio a DataView per Ordinamento
                            dataStep = dtCostiRicavi.DefaultView
                            dataStep.RowFilter = xFiltroAggiuntivo
                            dataStep.Sort = xOrderByDW
                            dataStepDest = dataStep.ToTable()

                            If dataStepDest.Rows.Count <> 0 Then

                                For Each dr As DataRow In dataStepDest.Rows

                                    'Nota: Il personale viene inserito con id_attivita -1000
                                    'Controllo Presenza Attività 
                                    Id_Attivita = -1000


                                    Select Case includiLav_Des
                                        Case 0
                                            xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita

                                        Case 1
                                            xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita & " And Lav_Cod_Agenda = " & dr("Lav_Cod_Agenda")
                                    End Select


                                    Valore_Ripartito = Impostazione_Valore(dr("FlagSecondoRaccolto"),
                                                                           dr("Frazionabile"),
                                                                           dr("Id_Imputazione"),
                                                                           dr("Id_Reg"),
                                                                           dr("Data_Inserimento"),
                                                                           dr("Progetto_Validita_Inizio"),
                                                                           dr.Item("Progetto_Validita_Fine"),
                                                                           validita_inizio,
                                                                           validita_fine,
                                                                           validita_fine_cdg,
                                                                           CDbl(dr("Valore")),
                                                                           objParametri_Server)

                                    dr_search_attivita = dtFinale.Select(xFiltroAggiuntivo)

                                    Select Case dr_search_attivita.Length

                                        Case 0

                                            'Inserimento Record Attività                                            
                                            dr_attivita = dtFinale.NewRow


                                            If Des_Attivita_Gruppo1_Last <> "Costi Fissi" Then
                                                dr_attivita.Item("Des_Attivita_Gruppo1") = "Costi Fissi"
                                                Des_Attivita_Gruppo1_Last = "Costi Fissi"
                                            End If
                                            'If Des_Attivita_Gruppo2_Last <> Des_Attivita_Gruppo2_Last Then
                                            dr_attivita.Item("Des_Attivita_Gruppo2") = ""
                                            Des_Attivita_Gruppo2_Last = ""
                                            'End If

                                            dr_attivita.Item("Id_Attivita") = Id_Attivita
                                            dr_attivita.Item("Attivita_Des") = "Personale"
                                            'dr_attivita.Item("Frazionabile") = dr("Frazionabile")

                                            If includiLav_Des = 1 Then
                                                dr_attivita.Item("Lav_Cod_Agenda") = dr("Lav_Cod_Agenda")
                                                dr_attivita.Item("Lav_Des_Agenda") = dr("Lav_Des_Agenda")
                                            End If

                                            dr_attivita.Item(_azienda) = 0

                                            'Impostazione Iniziale Valori
                                            For Count = 1 To UBound(Intest, 2) - 1

                                                Piva = Intest(0, Count)
                                                dr_attivita.Item(Piva) = 0

                                            Next

                                            'Aggiornamento Valore                                   
                                            dr_attivita.Item(dr.Item("Piva")) = CDbl(dr_attivita.Item(dr.Item("Piva"))) + Valore_Ripartito

                                            dtFinale.Rows.Add(dr_attivita)

                                        Case Else

                                            'Do Nothing
                                            Piva_Attivita = dr("Piva")

                                            'Aggiornamento Valore                                   
                                            dr_search_attivita(0).Item(Piva_Attivita) = CDbl(dr_search_attivita(0).Item(Piva_Attivita)) + Valore_Ripartito

                                    End Select

                                Next

                            End If

                        End If


                        '=====================================================================================================
                        'Step 2 Inserimento Costi Non Asociati ad Elem_Cod o Personale
                        '-----------------------------------------------------------------------------------------------------
                        xFiltroAggiuntivo = "Elem_Cod Not In (10, 3, 191)" 'Semente, Concimi, Fito

                        If personale = 1 Then
                            'I Costi del personale avranno una loro attività dedicata
                            xFiltroAggiuntivo = xFiltroAggiuntivo & " And Cod_Risum = 0 "
                        End If

                        'Passaggio a DataView per Ordinamento
                        dataStep = dtCostiRicavi.DefaultView
                        dataStep.RowFilter = xFiltroAggiuntivo
                        dataStep.Sort = xOrderByDW
                        dataStepDest = dataStep.ToTable()

                        If dataStepDest.Rows.Count <> 0 Then

                            For Each dr As DataRow In dataStepDest.Rows

                                'Controllo Presenza Attività
                                Select Case includiLav_Des
                                    Case 0
                                        xFiltroAggiuntivo = "Id_Attivita = " & dr("Id_Attivita")

                                    Case 1
                                        xFiltroAggiuntivo = "Id_Attivita = " & dr("Id_Attivita") & " And Lav_Cod_Agenda = " & dr("Lav_Cod_Agenda")
                                End Select

                                Valore_Ripartito = Impostazione_Valore(dr("FlagSecondoRaccolto"),
                                                                       dr("Frazionabile"),
                                                                       dr("Id_Imputazione"),
                                                                       dr("Id_Reg"),
                                                                       dr("Data_Inserimento"),
                                                                       dr("Progetto_Validita_Inizio"),
                                                                       dr.Item("Progetto_Validita_Fine"),
                                                                       validita_inizio,
                                                                       validita_fine,
                                                                       validita_fine_cdg,
                                                                       CDbl(dr("Valore")),
                                                                       objParametri_Server)

                                dr_search_attivita = dtFinale.Select(xFiltroAggiuntivo)

                                Select Case dr_search_attivita.Length

                                    Case 0

                                        'Inserimento Record Attività                                            
                                        dr_attivita = dtFinale.NewRow

                                        'If dr("Des_Attivita_Gruppo1") = "" Then
                                        '    dr("Des_Attivita_Gruppo1") = "-"
                                        '    dr_attivita.Item("Des_Attivita_Gruppo1") = dr("Des_Attivita_Gruppo1")
                                        'Else

                                        If Des_Attivita_Gruppo1_Last <> dr("Des_Attivita_Gruppo1") Then
                                            dr_attivita.Item("Des_Attivita_Gruppo1") = dr("Des_Attivita_Gruppo1")
                                            Des_Attivita_Gruppo1_Last = dr("Des_Attivita_Gruppo1")
                                        End If
                                        ' End If


                                        'If dr("Des_Attivita_Gruppo2") = "" Then
                                        '    dr("Des_Attivita_Gruppo2") = "-"
                                        '    dr_attivita.Item("Des_Attivita_Gruppo2") = dr("Des_Attivita_Gruppo2")
                                        'Else

                                        If Des_Attivita_Gruppo2_Last <> dr("Des_Attivita_Gruppo2") Then
                                            dr_attivita.Item("Des_Attivita_Gruppo2") = dr("Des_Attivita_Gruppo2")
                                            Des_Attivita_Gruppo2_Last = dr("Des_Attivita_Gruppo2")
                                        End If
                                        'End If

                                        dr_attivita.Item("Id_Attivita") = dr("Id_Attivita")
                                        dr_attivita.Item("Attivita_Des") = dr("Attivita")
                                        'dr_attivita.Item("Frazionabile") = dr("Frazionabile")

                                        If includiLav_Des = 1 Then
                                            dr_attivita.Item("Lav_Cod_Agenda") = dr("Lav_Cod_Agenda")
                                            dr_attivita.Item("Lav_Des_Agenda") = dr("Lav_Des_Agenda")
                                        End If

                                        'Inizializzazione Valori
                                        dr_attivita.Item(_azienda) = 0

                                        For Count = 1 To UBound(Intest, 2) - 1

                                            Piva = Intest(0, Count)
                                            dr_attivita.Item(Piva) = 0

                                        Next

                                        'Aggiornamento Valore                                                                          
                                        dr_attivita.Item(dr.Item("Piva")) = CDbl(dr_attivita.Item(dr.Item("Piva"))) + Valore_Ripartito

                                        dtFinale.Rows.Add(dr_attivita)

                                    Case Else

                                        'Do Nothing
                                        Piva_Attivita = dr("Piva")

                                        'Aggiornamento Valore                                        
                                        dr_search_attivita(0).Item(Piva_Attivita) = CDbl(dr_search_attivita(0).Item(Piva_Attivita)) + Valore_Ripartito

                                End Select

                            Next


                        End If




                        '=====================================================================================================
                        'Step 3 Inserimento Costi Associati ad Elem_Cod (Mezzi Tecnici)
                        '-----------------------------------------------------------------------------------------------------
                        xFiltroAggiuntivo = "Elem_Cod In (10, 3, 191)" 'Semente, Concimi, Fito

                        If personale = 1 Then
                            'I Costi del personale avranno una loro attività dedicata
                            xFiltroAggiuntivo = xFiltroAggiuntivo & " And Cod_Risum = 0 "
                        End If

                        'Passaggio a DataView per Ordinamento
                        dataStep = dtCostiRicavi.DefaultView
                        dataStep.RowFilter = xFiltroAggiuntivo
                        dataStep.Sort = xOrderByDW
                        dataStepDest = dataStep.ToTable()

                        If dataStepDest.Rows.Count <> 0 Then

                            For Each dr As DataRow In dataStepDest.Rows

                                'Nota: Questi elem_cod vengono inseriti come id_attività negativa
                                'Controllo Presenza Attività 
                                Id_Attivita = -dr("Elem_Cod")

                                Select Case includiLav_Des
                                    Case 0
                                        xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita

                                    Case 1
                                        xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita & " And Lav_Cod_Agenda = " & dr("Lav_Cod_Agenda")
                                End Select

                                Valore_Ripartito = Impostazione_Valore(dr("FlagSecondoRaccolto"),
                                                                       dr("Frazionabile"),
                                                                       dr("Id_Imputazione"),
                                                                       dr("Id_Reg"),
                                                                       dr("Data_Inserimento"),
                                                                       dr("Progetto_Validita_Inizio"),
                                                                       dr.Item("Progetto_Validita_Fine"),
                                                                       validita_inizio,
                                                                       validita_fine,
                                                                       validita_fine_cdg,
                                                                       CDbl(dr("Valore")),
                                                                       objParametri_Server)


                                dr_search_attivita = dtFinale.Select(xFiltroAggiuntivo)

                                Select Case dr_search_attivita.Length

                                    Case 0

                                        'Inserimento Record Attività                                            
                                        dr_attivita = dtFinale.NewRow

                                        If Des_Attivita_Gruppo1_Last <> "Costi Variabili" Then
                                            dr_attivita.Item("Des_Attivita_Gruppo1") = "Costi Variabili"
                                            Des_Attivita_Gruppo1_Last = "Costi Variabili"
                                        End If
                                        If Des_Attivita_Gruppo2_Last <> "Mezzi Tecnici" Then
                                            dr_attivita.Item("Des_Attivita_Gruppo2") = "Mezzi Tecnici"
                                            Des_Attivita_Gruppo2_Last = "Mezzi Tecnici"
                                        End If

                                        dr_attivita.Item("Id_Attivita") = Id_Attivita
                                        dr_attivita.Item("Attivita_Des") = dr("Categoria")
                                        'dr_attivita.Item("Frazionabile") = dr("Frazionabile")

                                        If includiLav_Des Then
                                            dr_attivita.Item("Lav_Cod_Agenda") = dr("Lav_Cod_Agenda")
                                            dr_attivita.Item("Lav_Des_Agenda") = dr("Lav_Des_Agenda")
                                        End If

                                        dr_attivita.Item(_azienda) = 0

                                        'Impostazione Iniziale Valori
                                        For Count = 1 To UBound(Intest, 2) - 1

                                            Piva = Intest(0, Count)
                                            dr_attivita.Item(Piva) = 0

                                        Next

                                        'Aggiornamento Valore                                   
                                        dr_attivita.Item(dr.Item("Piva")) = CDbl(dr_attivita.Item(dr.Item("Piva"))) + Valore_Ripartito

                                        dtFinale.Rows.Add(dr_attivita)

                                    Case Else

                                        'Do Nothing
                                        Piva_Attivita = dr("Piva")

                                        'Aggiornamento Valore                                   
                                        dr_search_attivita(0).Item(Piva_Attivita) = CDbl(dr_search_attivita(0).Item(Piva_Attivita)) + Valore_Ripartito

                                End Select

                            Next

                        End If

                        dtReport1 = dtIntestazione
                        dtReport2 = dtFinale


                    Case 2

                        '####################################################################################################
                        '################################### REPORT SPV COLTURA #############################################
                        '####################################################################################################


                        'Costruzione DataTable Intestazione
                        dtIntestazione.Columns.Add(New DataColumn("Des_Attivita_Gruppo1", GetType(String)))
                        dtIntestazione.Columns.Add(New DataColumn("Des_Attivita_Gruppo2", GetType(String)))
                        dtIntestazione.Columns.Add(New DataColumn("Id_Attivita", GetType(Integer)))
                        dtIntestazione.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
                        'dtIntestazione.Columns.Add(New DataColumn("Frazionabile", GetType(Integer)))
                        dtIntestazione.Columns.Add(New DataColumn("Lav_Cod_Agenda", GetType(Integer)))
                        dtIntestazione.Columns.Add(New DataColumn("Lav_Des_Agenda", GetType(String)))


                        'Inserimento Record HA  
                        dr_Intest = dtIntestazione.NewRow

                        dr_Intest.Item("Des_Attivita_Gruppo1") = ""
                        dr_Intest.Item("Des_Attivita_Gruppo2") = ""

                        dr_Intest.Item("Id_Attivita") = 0
                        dr_Intest.Item("Attivita_Des") = "HA"

                        dr_Intest.Item("Lav_Cod_Agenda") = 0
                        dr_Intest.Item("Lav_Des_Agenda") = ""

                        dtIntestazione.Rows.Add(dr_Intest)

                        'Inserimento Record HA Sommabili
                        dr_Intest = dtIntestazione.NewRow

                        dr_Intest.Item("Des_Attivita_Gruppo1") = ""
                        dr_Intest.Item("Des_Attivita_Gruppo2") = ""

                        dr_Intest.Item("Id_Attivita") = 0
                        dr_Intest.Item("Attivita_Des") = "HA Sommabili"

                        dr_Intest.Item("Lav_Cod_Agenda") = 0
                        dr_Intest.Item("Lav_Des_Agenda") = ""

                        dtIntestazione.Rows.Add(dr_Intest)


                        'Inserimento Record TON
                        dr_Intest = dtIntestazione.NewRow

                        dr_Intest.Item("Des_Attivita_Gruppo1") = ""
                        dr_Intest.Item("Des_Attivita_Gruppo2") = ""

                        dr_Intest.Item("Id_Attivita") = 0
                        dr_Intest.Item("Attivita_Des") = "TON"

                        dr_Intest.Item("Lav_Cod_Agenda") = 0
                        dr_Intest.Item("Lav_Des_Agenda") = ""

                        dtIntestazione.Rows.Add(dr_Intest)


                        'Costruzione DataTable
                        dtFinale.Columns.Add(New DataColumn("Des_Attivita_Gruppo1", GetType(String)))
                        dtFinale.Columns.Add(New DataColumn("Des_Attivita_Gruppo2", GetType(String)))
                        dtFinale.Columns.Add(New DataColumn("Id_Attivita", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
                        'dtFinale.Columns.Add(New DataColumn("Frazionabile", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Lav_Cod_Agenda", GetType(Integer)))
                        dtFinale.Columns.Add(New DataColumn("Lav_Des_Agenda", GetType(String)))

                        '=====================================================================================================
                        'Step 0 Definizione Intestazione
                        '-----------------------------------------------------------------------------------------------------

                        ReDim Intest(3, 0)

                        For iAzienda = 0 To 1

                            Select Case iAzienda
                                Case 0
                                    'Inserimento SPV come prima azienda in Array Intestazione
                                    dr_search = dtCostiRicavi.Select("Piva = '" & _azienda & "'")

                                Case Else
                                    dr_search = dtCostiRicavi.Select("Piva <> '" & _azienda & "'")

                            End Select

                            i_record = 0 'Contatore Record per verifica cambia di piva
                            Filtro_Veg_Cod = ""

                            '==========================================================================================================
                            'Inserimento dati relativi ai costi
                            '----------------------------------------------------------------------------------------------------------                            
                            If dr_search.Length <> 0 Then

                                num_record = dr_search.Length

                                For Each dr As DataRow In dr_search

                                    i_record = i_record + 1


                                    If dr.Item("Id_Imputazione") <> 0 Then
                                        'Imputazioni
                                        strKey_sub = dr.Item("Piva") & "_I_" & dr.Item("Id_Imputazione")
                                        Veg_Cod = -1000
                                    Else
                                        'Agricoli
                                        strKey_sub = dr.Item("Piva") & "_A_" & dr.Item("Veg_Cod")
                                        Veg_Cod = dr.Item("Veg_Cod")

                                        'Controllo la modalità di impostazione specie/coltura
                                        Select Case DettaglioSpecieVarieta
                                            Case 0 'Specie
                                                Cul_Cod = 0
                                            Case Else 'Coltura
                                                'Aggiungo il cul_cod
                                                Cul_Cod = dr.Item("Cul_Cod")

                                        End Select

                                        strKey_sub = strKey_sub & "_" & Cul_Cod

                                    End If

                                    If Veg_Cod <> -1000 Then

                                        'Controllo Filtro_Veg_Cod
                                        If InStr(Filtro_Veg_Cod, "'" & strKey_sub & "'") = 0 Then

                                            'Costruzione Filtro_Veg_Cod
                                            Filtro_Veg_Cod = Filtro_Veg_Cod & IIf(Filtro_Veg_Cod = "", "", ", ") & "'" & strKey_sub & "'"

                                            'Lettura Raccolte
                                            If Id_Budget = 0 Then
                                                DtProduzione = ObjReg_Impianti.LeggiProduzione_SPV(dr.Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, objParametri_Server)
                                            End If

                                            If DtProduzione.Rows.Count > 0 Then

                                                'Costruzione Filtro_Veg_Cod Residui
                                                Filtro_Veg_Cod_Residui = Filtro_Veg_Cod_Residui & IIf(Filtro_Veg_Cod_Residui = "", "", ", ") & "'" & strKey_sub & "'"

                                                Totale_Sup_Raccolta = 0
                                                Totale_Qta_Raccolta = 0

                                                For Each drPro As DataRow In DtProduzione.Rows

                                                    Mat_Cod = drPro.Item("Mat_Cod")
                                                    Mat_Des = drPro.Item("Mat_Des")
                                                    Priorita = drPro.Item("Priorita")

                                                    'Sommo Qta Raccolta
                                                    'Totale_Qta_Raccolta = Totale_Qta_Raccolta + drPro.Item("Qta")

                                                    'Accordo il Mat_Cod 
                                                    strKey = strKey_sub & "_" & Mat_Cod

                                                    If InStr(Filtro_Piva, "'" & strKey & "'") = 0 Then

                                                        'Costruzione Filtro_Piva
                                                        Filtro_Piva = Filtro_Piva & IIf(Filtro_Piva = "", "", ", ") & "'" & strKey & "'"

                                                        'Inserimento Array Intestazioni
                                                        ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)
                                                        Intest(0, UBound(Intest, 2) - 1) = strKey

                                                        'Inserimento Colonna
                                                        dtIntestazione.Columns.Add(New DataColumn(strKey, GetType(Decimal)))
                                                        dtFinale.Columns.Add(New DataColumn(strKey, GetType(Decimal)))

                                                        'Filtro tutte le raccolte con il Mat_Cod                                            
                                                        dr_search_mat = DtProduzione.Select("Mat_Cod = " & Mat_Cod & " ")


                                                        Totale_Qta_Raccolta_Mat = 0

                                                        If dr_search_mat.Length <> 0 Then

                                                            For Each dr_mat As DataRow In dr_search_mat

                                                                'Sommo Qta Raccolta
                                                                Totale_Qta_Raccolta_Mat = Totale_Qta_Raccolta_Mat + (dr_mat.Item("Qta") / 1000)

                                                            Next

                                                        End If

                                                        'Lettura Superficie Impianti Raccolti con il Mat_Cod
                                                        Totale_Sup_Raccolta_Mat = ObjReg_Impianti.SommaSuperficie_SPV(dr.Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, Mat_Cod, False, objParametri_Server)

                                                        'Impostazione Superficie
                                                        dtIntestazione(0).Item(strKey) = Totale_Sup_Raccolta_Mat

                                                        'Impostazione Superficie Sommabile
                                                        Select Case drPro("Priorita")
                                                            Case 0
                                                                dtIntestazione(1).Item(strKey) = Totale_Sup_Raccolta_Mat
                                                            Case Else
                                                                dtIntestazione(1).Item(strKey) = 0
                                                        End Select


                                                        'Impostazione Produzione
                                                        dtIntestazione(2).Item(strKey) = Totale_Qta_Raccolta_Mat



                                                    End If

                                                Next

                                            End If


                                        End If

                                    Else

                                        'Progetto
                                        If InStr(Filtro_Piva, "'" & strKey_sub & "'") = 0 Then

                                            'Costruzione Filtro_Piva
                                            Filtro_Piva = Filtro_Piva & IIf(Filtro_Piva = "", "", ", ") & "'" & strKey_sub & "'"

                                            'Inserimento Colonna
                                            dtIntestazione.Columns.Add(New DataColumn(strKey_sub, GetType(Decimal)))
                                            dtFinale.Columns.Add(New DataColumn(strKey_sub, GetType(Decimal)))

                                            dtIntestazione(0).Item(strKey_sub) = 0
                                            dtIntestazione(1).Item(strKey_sub) = 0
                                            dtIntestazione(2).Item(strKey_sub) = 0

                                        End If


                                    End If

                                    '==============================================================================================================================
                                    'Controllo variazione di piva per inserimento coltura non presenti nei costi
                                    '------------------------------------------------------------------------------------------------------------------------------
                                    bInsertColtura_NO_Costi = False
                                    If i_record = dr_search.Length Then
                                        'Ultima Riga
                                        bInsertColtura_NO_Costi = True
                                    Else
                                        If dr_search(i_record - 1).Item("PIva") <> dr_search(i_record).Item("PIva") Then
                                            bInsertColtura_NO_Costi = True
                                        End If
                                    End If

                                    If bInsertColtura_NO_Costi Then

                                        'Lettura degli impianti senza costi associati o impianti senza raccolta
                                        Select Case Id_Budget

                                            Case 0

                                                DtImpianti = ObjReg_Impianti.LeggiImpianti_Residuo(dr_search(i_record - 1).Item("Piva"), validita_inizio, validita_fine, objParametri_Server)

                                            Case Else

                                                DtImpianti = ObjReg_Impianti.LeggiImpianti_Residuo_Budget(dr_search(i_record - 1).Item("Piva"), Id_Budget, validita_inizio, validita_fine, objParametri_Server)

                                        End Select


                                        If DtImpianti.Rows.Count <> 0 Then

                                            For Each drImpianti As DataRow In DtImpianti.Rows

                                                'Agricoli
                                                strKey_sub = dr_search(i_record - 1).Item("Piva") & "_A_" & drImpianti.Item("Veg_Cod")
                                                Veg_Cod = drImpianti.Item("Veg_Cod")

                                                'Controllo la modalità di impostazione specie/coltura
                                                Select Case DettaglioSpecieVarieta
                                                    Case 0 'Specie
                                                        Cul_Cod = 0
                                                    Case Else 'Coltura
                                                        'Aggiungo il cul_cod
                                                        Cul_Cod = drImpianti.Item("Cul_Cod")
                                                End Select

                                                strKey_sub = strKey_sub & "_" & Cul_Cod


                                                'Controllo Filtro_Veg_Cod
                                                If InStr(Filtro_Veg_Cod_Residui, "'" & strKey_sub & "'") = 0 Then

                                                    'Costruzione Filtro_Veg_Cod Residui
                                                    Filtro_Veg_Cod_Residui = Filtro_Veg_Cod_Residui & IIf(Filtro_Veg_Cod_Residui = "", "", ", ") & "'" & strKey_sub & "'"


                                                    Totale_Sup_Imp = ObjReg_Impianti.SommaSuperficie_SPV(dr_search(i_record - 1).Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, 0, False, objParametri_Server)

                                                    If Id_Budget = 0 Then
                                                        Produzione = ObjReg_Impianti.SommaProduzione_SPV(dr_search(i_record - 1).Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, objParametri_Server, 0)
                                                    Else
                                                        Produzione = 0
                                                    End If

                                                    If Produzione = 0 Then
                                                        Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(dr_search(i_record - 1).Item("Piva"), Id_Budget, Veg_Cod, Cul_Cod, validita_inizio, validita_fine, objParametri_Server)
                                                    End If

                                                    'Inserisco il Mat_Cod Nullo
                                                    strKey_sub = strKey_sub & "_0"

                                                    'Inserimento Colonna
                                                    dtIntestazione.Columns.Add(New DataColumn(strKey_sub, GetType(Decimal)))
                                                    dtFinale.Columns.Add(New DataColumn(strKey_sub, GetType(Decimal)))

                                                    'Impostazione Superficie 
                                                    dtIntestazione(0).Item(strKey_sub) = Totale_Sup_Imp

                                                    'Impostazione Superficie Sommabile
                                                    dtIntestazione(1).Item(strKey_sub) = Totale_Sup_Imp

                                                    'Impostazione Qta Resa Prevista
                                                    dtIntestazione(2).Item(strKey_sub) = Produzione

                                                    'Inserimento Array Intestazioni
                                                    ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)
                                                    Intest(0, UBound(Intest, 2) - 1) = strKey_sub


                                                End If

                                            Next

                                        End If

                                    End If
                                    '==============================================================================================================================

                                Next

                            End If

                        Next


                        '=======================================================================================================================
                        'Lettura Raccolte
                        If Id_Budget = 0 Then
                            DtProduzione = ObjReg_Impianti.LeggiProduzione_SPV("", -1000, 0, validita_inizio, validita_fine, objParametri_Server)
                        End If

                        For iAttivita = 1 To 3

                            xFiltroAggiuntivo = ""

                            Select Case iAttivita

                                Case 1
                                    '=====================================================================================================
                                    'Step 1 Inserimento Costi Personale
                                    '-----------------------------------------------------------------------------------------------------
                                    If personale = 1 Then
                                        xFiltroAggiuntivo = "Cod_Risum <> 0"
                                    End If

                                    Des_Attivita_Gruppo1 = "Costi Fissi"
                                    Des_Attivita_Gruppo2 = ""


                                Case 2
                                    '==============================================================================================
                                    'Step 2 Inserimento Costi Non Asociati ad Elem_Cod o Personale
                                    '-----------------------------------------------------------------------------------------------------
                                    xFiltroAggiuntivo = "Elem_Cod Not In (10, 3, 191)" 'Semente, Concimi, Fito

                                    If personale = 1 Then
                                        'I Costi del personale avranno una loro attività dedicata
                                        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Cod_Risum = 0 "
                                    End If

                                    'Parametri Dinamici
                                    Des_Attivita_Gruppo1 = ""
                                    Des_Attivita_Gruppo2 = ""

                                Case 3
                                    '=====================================================================================================
                                    'Step 3 Inserimento Costi Associati ad Elem_Cod (Mezzi Tecnici)
                                    '-----------------------------------------------------------------------------------------------------
                                    xFiltroAggiuntivo = "Elem_Cod In (10, 3, 191)" 'Semente, Concimi, Fito

                                    If personale = 1 Then
                                        'I Costi del personale avranno una loro attività dedicata
                                        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Cod_Risum = 0 "
                                    End If

                                    Des_Attivita_Gruppo1 = "Costi Variabili"
                                    Des_Attivita_Gruppo2 = "Mezzi Tecnici"

                            End Select

                            If Trim(xFiltroAggiuntivo) <> "" Then

                                'Passaggio a DataView per Ordinamento
                                dataStep = dtCostiRicavi.DefaultView
                                dataStep.RowFilter = xFiltroAggiuntivo
                                dataStep.Sort = xOrderByDW
                                dataStepDest = dataStep.ToTable()

                                If dataStepDest.Rows.Count <> 0 Then

                                    For Each dr As DataRow In dataStepDest.Rows

                                        'Nota: Il personale viene inserito con id_attivita -1000
                                        'Controllo Presenza Attività 

                                        Select Case iAttivita
                                            Case 1
                                                Id_Attivita = -1000
                                            Case 2
                                                Id_Attivita = dr("Id_Attivita")

                                            Case Else
                                                Id_Attivita = -dr("Elem_Cod")
                                        End Select


                                        Select Case includiLav_Des
                                            Case 0
                                                xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita

                                            Case 1
                                                xFiltroAggiuntivo = "Id_Attivita = " & Id_Attivita & " And Lav_Cod_Agenda = " & dr("Lav_Cod_Agenda")
                                        End Select

                                        dr_search_attivita = dtFinale.Select(xFiltroAggiuntivo)

                                        If dr.Item("Id_Imputazione") <> 0 Then
                                            'Imputazioni
                                            strKey = dr.Item("Piva") & "_I_" & dr.Item("Id_Imputazione")
                                            Veg_Cod = -1000
                                            Mat_Cod = 0
                                            bOk = True
                                        Else
                                            'Agricoli
                                            strKey_sub = dr.Item("Piva") & "_A_" & dr.Item("Veg_Cod")
                                            Veg_Cod = dr.Item("Veg_Cod")

                                            'Controllo la modalità di impostazione specie/coltura
                                            Select Case DettaglioSpecieVarieta
                                                Case 0 'Specie
                                                    Cul_Cod = 0
                                                Case Else 'Coltura
                                                    'Aggiungo il cul_cod
                                                    Cul_Cod = dr.Item("Cul_Cod")
                                            End Select

                                            'Impostazione Mat_Cod
                                            'Verifica Esistenza Raccolta per quell'impianto
                                            If DtProduzione.Rows.Count > 0 Then

                                                Mat_Cod = 0
                                                Priorita = 0

                                                'Controllo il lav_cod
                                                Select Case dr.Item("Lav_Cod_Agenda")

                                                    Case 125

                                                        'In caso di raccolta il costo è associato al prodotto relativo all'operazione di raccolta
                                                        dr_search_agenda = DtProduzione.Select("Piva = '" & dr.Item("Piva") & "' And " &
                                                                                       "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                                                                       "Appezza = " & dr.Item("Appezza") & " And " &
                                                                                       "Id_Destinazione = " & dr.Item("Id_Reg") & " And " &
                                                                                       "Id_Agenda = " & dr.Item("Id_Agenda_Agenda"))

                                                    Case Else

                                                        'Cerco il prodotto raccolta con priorita 0                                                        
                                                        dr_search_agenda = DtProduzione.Select("Piva = '" & dr.Item("Piva") & "' And " &
                                                                                       "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                                                                       "Appezza = " & dr.Item("Appezza") & " And " &
                                                                                       "Id_Destinazione = " & dr.Item("Id_Reg") & " And " &
                                                                                       "Priorita = 0")

                                                        If dr_search_agenda.Length = 0 Then

                                                            'Nessuna raccolta con prodotto avente priorita 0 -->Rilasso il vincolo                                                       
                                                            dr_search_agenda = DtProduzione.Select("Piva = '" & dr.Item("Piva") & "' And " &
                                                                                           "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                                                                           "Appezza = " & dr.Item("Appezza") & " And " &
                                                                                           "Id_Destinazione = " & dr.Item("Id_Reg"))


                                                        End If

                                                End Select


                                                Select Case dr_search_agenda.Length

                                                    Case 0 'Nessuna Raccolta --> Mat_Cod 0


                                                    Case Else

                                                        'Controllo coerenza
                                                        If Veg_Cod <> 0 Then

                                                            Mat_Cod = dr_search_agenda(0)("Mat_Cod")
                                                            Priorita = dr_search_agenda(0)("Priorita")

                                                        End If

                                                End Select

                                            End If

                                            'Controllo presenza chiave x gestione eccezioni (per esempio impianto presente nei costi ma cancellato da anagrafica
                                            strKey = strKey_sub & "_" & Cul_Cod & "_" & Mat_Cod

                                            bOk = False
                                            For i_count = 0 To UBound(Intest, 2) - 1

                                                If Intest(0, i_count) = strKey Then

                                                    bOk = True
                                                    Exit For
                                                End If

                                            Next

                                            If Not bOk And Mat_Cod = 0 Then

                                                'Inserimento Array Intestazioni
                                                ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)
                                                Intest(0, UBound(Intest, 2) - 1) = strKey

                                                'Inserimento Colonna
                                                dtIntestazione.Columns.Add(New DataColumn(strKey, GetType(Decimal)))
                                                dtFinale.Columns.Add(New DataColumn(strKey, GetType(Decimal)))

                                                'Lettura Superficie Impianti Raccolti con il Mat_Cod
                                                Totale_Sup_Raccolta_Mat = ObjReg_Impianti.SommaSuperficie_SPV(dr.Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, Mat_Cod, True, objParametri_Server)

                                                Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(dr.Item("Piva"), Id_Budget, Veg_Cod, Cul_Cod, validita_inizio, validita_fine, objParametri_Server)


                                                'Impostazione Superficie
                                                dtIntestazione(0).Item(strKey) = Totale_Sup_Raccolta_Mat
                                                'Impostazione Superficie Somambile
                                                dtIntestazione(1).Item(strKey) = Totale_Sup_Raccolta_Mat
                                                'Impostazione Produzione
                                                dtIntestazione(2).Item(strKey) = Produzione

                                                bOk = True

                                            End If




                                            If Not bOk And Mat_Cod = 0 Then

                                                'Inserimento Array Intestazioni
                                                ReDim Preserve Intest(UBound(Intest, 1), UBound(Intest, 2) + 1)
                                                Intest(0, UBound(Intest, 2) - 1) = strKey

                                                'Inserimento Colonna
                                                dtIntestazione.Columns.Add(New DataColumn(strKey, GetType(Decimal)))
                                                dtFinale.Columns.Add(New DataColumn(strKey, GetType(Decimal)))

                                                'Lettura Superficie Impianti Raccolti con il Mat_Cod
                                                Totale_Sup_Raccolta_Mat = ObjReg_Impianti.SommaSuperficie_SPV(dr.Item("Piva"), Veg_Cod, Cul_Cod, validita_inizio, validita_fine, Mat_Cod, True, objParametri_Server)

                                                Produzione = ObjReg_Impianti.SommaResaPrevista_SPV(dr.Item("Piva"), Id_Budget, Veg_Cod, Cul_Cod, validita_inizio, validita_fine, objParametri_Server)


                                                'Impostazione Superficie
                                                dtIntestazione(0).Item(strKey) = Totale_Sup_Raccolta_Mat
                                                'Impostazione Superficie Sommabile
                                                dtIntestazione(1).Item(strKey) = Totale_Sup_Raccolta_Mat
                                                'Impostazione Produzione
                                                dtIntestazione(2).Item(strKey) = Produzione

                                                bOk = True

                                            End If

                                        End If



                                        If bOk Then

                                            Valore_Ripartito = Impostazione_Valore(dr("FlagSecondoRaccolto"),
                                                                                   dr("Frazionabile"),
                                                                                   dr.Item("Id_Imputazione"),
                                                                                   dr.Item("Id_Reg"),
                                                                                   dr("Data_Inserimento"),
                                                                                   dr("Progetto_Validita_Inizio"),
                                                                                   dr.Item("Progetto_Validita_Fine"),
                                                                                   validita_inizio,
                                                                                   validita_fine,
                                                                                   validita_fine_cdg,
                                                                                   CDbl(dr("Valore")),
                                                                                   objParametri_Server)

                                            Select Case dr_search_attivita.Length

                                                Case 0

                                                    'Inserimento Record Attività                                            
                                                    dr_attivita = dtFinale.NewRow

                                                    Select Case iAttivita
                                                        Case 1, 3

                                                            If Des_Attivita_Gruppo1_Last <> Des_Attivita_Gruppo1 Then
                                                                dr_attivita.Item("Des_Attivita_Gruppo1") = Des_Attivita_Gruppo1
                                                                Des_Attivita_Gruppo1_Last = Des_Attivita_Gruppo1
                                                            End If

                                                            'If dr("Des_Attivita_Gruppo2") = "" Then
                                                            '    dr("Des_Attivita_Gruppo2") = "-"
                                                            '    dr_attivita.Item("Des_Attivita_Gruppo2") = dr("Des_Attivita_Gruppo2")
                                                            'Else

                                                            If Des_Attivita_Gruppo2_Last <> Des_Attivita_Gruppo2 Then
                                                                'If Des_Attivita_Gruppo2 = "" Then
                                                                '    Des_Attivita_Gruppo2 = "-"
                                                                'End If
                                                                dr_attivita.Item("Des_Attivita_Gruppo2") = Des_Attivita_Gruppo2
                                                                Des_Attivita_Gruppo2_Last = Des_Attivita_Gruppo2
                                                            End If

                                                            'End If


                                                        Case Else

                                                            If Des_Attivita_Gruppo1_Last <> dr("Des_Attivita_Gruppo1") Then
                                                                dr_attivita.Item("Des_Attivita_Gruppo1") = dr("Des_Attivita_Gruppo1")
                                                                Des_Attivita_Gruppo1_Last = dr("Des_Attivita_Gruppo1")
                                                            End If

                                                            'If dr("Des_Attivita_Gruppo2") = "" Then
                                                            '    dr("Des_Attivita_Gruppo2") = "-"
                                                            '    dr_attivita.Item("Des_Attivita_Gruppo2") = dr("Des_Attivita_Gruppo2")
                                                            'Else

                                                            If Des_Attivita_Gruppo2_Last <> dr("Des_Attivita_Gruppo2") Then
                                                                'If dr("Des_Attivita_Gruppo2") = "" Then
                                                                '    dr("Des_Attivita_Gruppo2") = "-"
                                                                'End If
                                                                dr_attivita.Item("Des_Attivita_Gruppo2") = dr("Des_Attivita_Gruppo2")
                                                                Des_Attivita_Gruppo2_Last = dr("Des_Attivita_Gruppo2")
                                                            End If
                                                            'End If

                                                    End Select

                                                    Select Case iAttivita
                                                        Case 1
                                                            dr_attivita.Item("Id_Attivita") = Id_Attivita
                                                            dr_attivita.Item("Attivita_Des") = "Personale"
                                                        Case 2
                                                            dr_attivita.Item("Id_Attivita") = dr("Id_Attivita")
                                                            dr_attivita.Item("Attivita_Des") = dr("Attivita")
                                                        Case Else
                                                            dr_attivita.Item("Id_Attivita") = Id_Attivita
                                                            dr_attivita.Item("Attivita_Des") = dr("Categoria")
                                                    End Select

                                                    If includiLav_Des Then
                                                        dr_attivita.Item("Lav_Cod_Agenda") = dr("Lav_Cod_Agenda")
                                                        dr_attivita.Item("Lav_Des_Agenda") = dr("Lav_Des_Agenda")
                                                    End If

                                                    'dr_attivita.Item("Frazionabile") = dr("Frazionabile")


                                                    'Impostazione Iniziale Valori
                                                    For Count = 1 To UBound(Intest, 2) - 1
                                                        dr_attivita.Item(Intest(0, Count)) = 0
                                                    Next

                                                    'Aggiornamento Valore                                   
                                                    dr_attivita.Item(strKey) = Valore_Ripartito

                                                    dtFinale.Rows.Add(dr_attivita)

                                                Case Else


                                                    If Not IsNumeric(dr_search_attivita(0).Item(strKey)) Then
                                                        dr_search_attivita(0).Item(strKey) = 0
                                                    End If '


                                                    'Aggiornamento Valore                                   
                                                    dr_search_attivita(0).Item(strKey) = CDbl(dr_search_attivita(0).Item(strKey)) + Valore_Ripartito

                                            End Select


                                        Else

                                            strErrore = strErrore & "- Attivita " & dr.Item("Attivita") & "- " & dr.Item("Lav_Des_Agenda") & " Chiave " & strKey & " inconsistente."

                                        End If

                                    Next

                                End If

                            End If

                        Next

                        dtReport1 = dtIntestazione
                        dtReport2 = dtFinale




                    Case 3


                        '####################################################################################################
                        '################################### REPORT COLTURA E CAMPO #########################################
                        '####################################################################################################



                        generazioneDataTable3(_azienda, Id_Budget, validita_inizio, validita_fine, Data_Validita_Inizio, validita_fine_cdg, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, xOrderBy, ObjReg_Impianti, DtDati, DtProduzione, Totale_Qta_Raccolta, Last_Impianto, Search_Impianto, dtMercato, dtEnergia, dtCostiRicavi)


                        Dim datav As New DataView
                        datav = dtMercato.DefaultView
                        datav.Sort = "Prodotto_Raccolto ASC, Coltura_DES ASC"
                        dtMercato = datav.ToTable()

                        datav = dtEnergia.DefaultView
                        datav.Sort = "Prodotto_Raccolto ASC, Coltura_DES ASC"
                        dtEnergia = datav.ToTable()

                        dtReport2 = dtMercato
                        dtReport1 = dtEnergia





                    Case 4


                        '####################################################################################################
                        '################################### Report Piano Colturale #########################################
                        '####################################################################################################

                        'Costruzione DataTable SAU
                        dtSAU.Columns.Add(New DataColumn("Intestazione", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Appezzamento", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("SAU", GetType(Decimal)))
                        dtSAU.Columns.Add(New DataColumn("SAU_Sommabile", GetType(Decimal)))
                        dtSAU.Columns.Add(New DataColumn("Primo_Raccolto", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Prodotto_Raccolto1", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Destinazione1", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Secondo_Raccolto", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Prodotto_Raccolto2", GetType(String)))
                        dtSAU.Columns.Add(New DataColumn("Destinazione2", GetType(String)))


                        'Lettura Raccolte
                        If Id_Budget = 0 Then
                            'Ordinamento per priorità e mat_cod (per evitare duplicati)
                            xOrderBy = "Priorita, Materie_Prime.Mat_Cod Asc"
                            DtProduzione = ObjReg_Impianti.LeggiProduzione_SPV("", -1000, 0, validita_inizio, validita_fine, objParametri_Server, xOrderBy)
                        End If


                        'Lettura Appezzamenti                        
                        DtDati = ObjAppezzamento.LeggixReportPianoColturale(_azienda, Id_Budget, includiAziendeFiglie, validita_inizio, validita_fine, objParametri_Server)

                        'Lettura Impianti                        
                        'Nota: Ordinamento temporale per determinare il numero di raccolto
                        xOrderBy = " Piva, Sa_Cod, Appezza, Id_Reg, Val_Inizio_Impianto "

                        DtDati2 = ObjReg_Impianti.LeggixReportProduzioneColturaCampo(_azienda, Id_Budget, includiAziendeFiglie, validita_inizio, validita_fine, xOrderBy, objParametri_Server)

                        If DtDati.Rows.Count <> 0 Then

                            For Each dr As DataRow In DtDati.Rows

                                If DtDati2.Rows.Count > 0 Then

                                    Search_Impianto = "Piva = '" & dr.Item("Piva") & "' And " &
                                                      "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                                      "Appezza = " & dr.Item("Appezza") & " And " &
                                                      "Id_Reg = " & dr.Item("Id_Reg")

                                    dr_search_impianto = DtDati2.Select(Search_Impianto)

                                    If DtProduzione.Rows.Count > 0 Then

                                        'Filtro le raccolte dell'impianto
                                        dr_search_mat = DtProduzione.Select("Piva = '" & dr.Item("Piva") & "' And " &
                                                                            "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                                                            "Appezza = " & dr.Item("Appezza") & " And " &
                                                                            "Id_Destinazione = " & dr("Id_Reg"))



                                        Select Case dr_search_mat.Length

                                            Case 0 ' Nessuna Raccolta

                                                'Inserimento Record  
                                                dr_SAU = dtSAU.NewRow

                                                dr_SAU.Item("Intestazione") = dr.Item("Rag_Soc")
                                                dr_SAU.Item("Appezzamento") = dr.Item("App_Nome") & IIf(Trim(dr.Item("Codice_Impianto")) <> "", " (" & dr.Item("Codice_Impianto") & ")", "")
                                                dr_SAU.Item("SAU") = dr_search_impianto(0)("Sup_Imp")
                                                dr_SAU.Item("Primo_Raccolto") = ""
                                                dr_SAU.Item("Destinazione1") = ""
                                                dr_SAU.Item("Prodotto_Raccolto1") = ""
                                                dr_SAU.Item("Secondo_Raccolto") = ""
                                                dr_SAU.Item("Destinazione2") = ""
                                                dr_SAU.Item("Prodotto_Raccolto2") = ""

                                                Select Case dr_search_impianto(0)("FlagSecondoRaccolto")
                                                    Case 0
                                                        dr_SAU.Item("SAU_Sommabile") = dr_search_impianto(0)("Sup_Imp")
                                                    Case Else
                                                        dr_SAU.Item("SAU_Sommabile") = 0
                                                End Select


                                                dtSAU.Rows.Add(dr_SAU)

                                            Case Else

                                                Mat_Cod_Raccolta = 0
                                                Count = 0

                                                'Inserimento di tante righe quanti sono i prodotti raccolti
                                                For Each drw As DataRow In dr_search_mat

                                                    If Mat_Cod_Raccolta <> drw("Mat_Cod") Then

                                                        Mat_Cod_Raccolta = drw("Mat_Cod")

                                                        Count = Count + 1

                                                        'Inserimento Record  
                                                        dr_SAU = dtSAU.NewRow

                                                        dr_SAU.Item("Intestazione") = dr.Item("Rag_Soc")
                                                        dr_SAU.Item("Appezzamento") = dr.Item("App_Nome") & IIf(Trim(dr.Item("Codice_Impianto")) <> "", " (" & dr.Item("Codice_Impianto") & ")", "")
                                                        dr_SAU.Item("SAU") = dr_search_impianto(0)("Sup_Imp")

                                                        Select Case dr_search_impianto(0)("FlagSecondoRaccolto")

                                                            Case 0
                                                                dr_SAU.Item("Primo_Raccolto") = dr_search_impianto(0)("Specie")
                                                                dr_SAU.Item("Destinazione1") = dr_search_impianto(0)("Finalita")
                                                                dr_SAU.Item("Prodotto_Raccolto1") = drw("Mat_Des")
                                                                dr_SAU.Item("Secondo_Raccolto") = ""
                                                                dr_SAU.Item("Destinazione2") = ""
                                                                dr_SAU.Item("Prodotto_Raccolto2") = ""
                                                            Case Else
                                                                dr_SAU.Item("Primo_Raccolto") = ""
                                                                dr_SAU.Item("Destinazione1") = ""
                                                                dr_SAU.Item("Prodotto_Raccolto1") = ""
                                                                dr_SAU.Item("Secondo_Raccolto") = dr_search_impianto(0)("Specie")
                                                                dr_SAU.Item("Destinazione2") = dr_search_impianto(0)("Finalita")
                                                                dr_SAU.Item("Prodotto_Raccolto2") = drw("Mat_Des")

                                                        End Select

                                                        If dr_search_impianto(0)("FlagSecondoRaccolto") = 0 And drw("Priorita") = 0 And Count = 1 Then
                                                            'Primo raccolto, prodotto principale e primo prodotto raccolto. 
                                                            dr_SAU.Item("SAU_Sommabile") = dr_search_impianto(0)("Sup_Imp")

                                                        Else
                                                            'Sommabile Nulla
                                                            dr_SAU.Item("SAU_Sommabile") = 0
                                                        End If

                                                        dtSAU.Rows.Add(dr_SAU)

                                                    End If

                                                Next

                                        End Select


                                    End If


                                End If


                            Next

                        End If


                        'Costruzione DataTable dtEnergia
                        Dim dtEnergiaTemp As New DataTable
                        Dim dtMercatoTemp As New DataTable
                        Dim dtReport3Temp As New DataTable

                        generazioneDataTable3(_azienda, Id_Budget, validita_inizio, validita_fine, Data_Validita_Inizio, validita_fine_cdg, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, xOrderBy, ObjReg_Impianti, DtDati, DtProduzione, Totale_Qta_Raccolta, Last_Impianto, Search_Impianto, dtMercatoTemp, dtEnergiaTemp, dtCostiRicavi)

                        dtEnergia.Columns.Add(New DataColumn("Coltura_Des", GetType(String)))
                        dtEnergia.Columns.Add(New DataColumn("ProdottoRaccolto", GetType(String)))
                        dtEnergia.Columns.Add(New DataColumn("SuperficieTot", GetType(Decimal)))
                        dtEnergia.Columns.Add(New DataColumn("SommabileTot", GetType(Decimal)))
                        dtEnergia.Columns.Add(New DataColumn("ProduzioneTot", GetType(Decimal)))
                        dtEnergia.Columns.Add(New DataColumn("m3Tot", GetType(Decimal)))
                        dtEnergia.Columns.Add(New DataColumn("CostoTot", GetType(Decimal)))


                        Dim dictionaryProd As New Dictionary(Of String, Decimal)
                        Dim dictionarySup As New Dictionary(Of String, Decimal)
                        Dim dictionarySommabile As New Dictionary(Of String, Decimal)
                        Dim dictionaryM3 As New Dictionary(Of String, Decimal)
                        Dim dictionaryCosto As New Dictionary(Of String, Decimal)


                        For Each row As DataRow In dtEnergiaTemp.Rows
                            Dim coltura = row.Item("Coltura_Des")
                            Dim prodottoRaccolto = row.Item("Prodotto_Raccolto")
                            Dim sup = row.Item("Superficie")
                            Dim sommabile = row.Item("Sommabile")
                            Dim prod = (row.Item("Produzione"))
                            Dim m3 = row.Item("M3")
                            Dim Costo = row.Item("Costo")

                            Dim key = coltura + "|" + If(IsNothing(prodottoRaccolto), "", prodottoRaccolto)
                            If dictionaryProd.ContainsKey(key) Then
                                dictionaryProd(key) += prod
                            Else
                                dictionaryProd.Add(key, prod)
                            End If

                            If dictionarySup.ContainsKey(key) Then
                                dictionarySup(key) += sup
                            Else
                                dictionarySup.Add(key, sup)
                            End If

                            If dictionarySommabile.ContainsKey(key) Then
                                dictionarySommabile(key) += sommabile
                            Else
                                dictionarySommabile.Add(key, sommabile)
                            End If

                            If dictionaryM3.ContainsKey(key) Then
                                dictionaryM3(key) += m3
                            Else
                                dictionaryM3.Add(key, m3)
                            End If

                            If dictionaryCosto.ContainsKey(key) Then
                                dictionaryCosto(key) += Costo
                            Else
                                dictionaryCosto.Add(key, Costo)
                            End If



                        Next

                        For Each key In dictionaryProd.Keys()
                            dr_Intest = dtEnergia.NewRow
                            Dim keys = key.Split("|")
                            dr_Intest.Item("Coltura_Des") = keys(0)
                            If keys(1) <> "" Then
                                dr_Intest.Item("ProdottoRaccolto") = keys(1)
                            End If
                            dr_Intest.Item("SuperficieTot") = Math.Round(dictionarySup(key), 3)
                            dr_Intest.Item("SommabileTot") = Math.Round(dictionarySommabile(key), 3)
                            dr_Intest.Item("ProduzioneTot") = Math.Round(dictionaryProd(key), 3)
                            dr_Intest.Item("m3Tot") = Math.Round(dictionaryM3(key), 3)
                            dr_Intest.Item("CostoTot") = Math.Round(dictionaryCosto(key), 2)


                            dtEnergia.Rows.Add(dr_Intest)
                        Next








                        'dt mercato


                        'Costruzione DataTable dtMercato
                        dtMercato.Columns.Add(New DataColumn("Coltura_Des", GetType(String)))
                        dtMercato.Columns.Add(New DataColumn("ProdottoRaccolto", GetType(String)))
                        dtMercato.Columns.Add(New DataColumn("SuperficieTot", GetType(Decimal)))
                        dtMercato.Columns.Add(New DataColumn("SommabileTot", GetType(Decimal)))
                        dtMercato.Columns.Add(New DataColumn("ProduzioneTot", GetType(Decimal)))

                        dtMercato.Columns.Add(New DataColumn("CostoTot", GetType(Decimal)))


                        dictionarySup.Clear()
                        dictionarySommabile.Clear()
                        dictionaryProd.Clear()
                        dictionaryCosto.Clear()

                        For Each row As DataRow In dtMercatoTemp.Rows
                            Dim coltura = row.Item("Coltura_Des")
                            Dim sup = row.Item("Superficie")
                            Dim sommabile = row.Item("Sommabile")
                            Dim prodottoRaccolto = row.Item("Prodotto_Raccolto")
                            Dim prod = (row.Item("Produzione"))
                            Dim Costo = row.Item("Costo")
                            Dim key = coltura + "|" + If(IsNothing(prodottoRaccolto), "", prodottoRaccolto)

                            If dictionaryProd.ContainsKey(key) Then
                                dictionaryProd(key) += prod
                            Else
                                dictionaryProd.Add(key, prod)
                            End If

                            If dictionarySup.ContainsKey(key) Then
                                dictionarySup(key) += sup
                            Else
                                dictionarySup.Add(key, sup)
                            End If

                            If dictionarySommabile.ContainsKey(key) Then
                                dictionarySommabile(key) += sommabile
                            Else
                                dictionarySommabile.Add(key, sommabile)
                            End If

                            If dictionaryCosto.ContainsKey(key) Then
                                dictionaryCosto(key) += Costo
                            Else
                                dictionaryCosto.Add(key, Costo)
                            End If

                        Next

                        For Each key In dictionaryProd.Keys()
                            dr_Intest = dtMercato.NewRow
                            Dim keys = key.Split("|")
                            dr_Intest.Item("Coltura_Des") = keys(0)
                            If keys(1) <> "" Then
                                dr_Intest.Item("ProdottoRaccolto") = keys(1)
                            End If
                            dr_Intest.Item("SuperficieTot") = Math.Round(dictionarySup(key), 3)
                            dr_Intest.Item("SommabileTot") = Math.Round(dictionarySommabile(key), 3)
                            dr_Intest.Item("ProduzioneTot") = Math.Round(dictionaryProd(key), 3)
                            dr_Intest.Item("CostoTot") = Math.Round(dictionaryCosto(key), 2)

                            dtMercato.Rows.Add(dr_Intest)
                        Next

                        dtReport1 = dtEnergia
                        dtReport2 = dtMercato
                        dtReport3 = dtSAU

                End Select


            End If


            Return strErrore
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


    End Function


    Private Shared Sub generazioneDataTable3(_azienda As String, id_budget As Integer, validita_inizio As String, validita_fine As String, data_validita_inizio As String, validita_fine_cdg As String, includiAziendeFiglie As Integer, ByRef objParametri_Server As AgronicaCoreParametri, DettaglioSpecieVarieta As Integer, ByRef xOrderBy As String, ObjReg_Impianti As Reg_Impianti_Read, ByRef DtDati As DataTable, ByRef DtProduzione As DataTable, ByRef Totale_Qta_Raccolta As Decimal, ByRef Last_Impianto As String, ByRef Search_Impianto As String, dtMercato As DataTable, dtEnergia As DataTable, dtCostiRicavi As DataTable)
        'Dim dr_Energia As DataRow
        'Dim dr_Mercato As DataRow
        Dim dr_search_agenda() As DataRow
        Dim dr_search_costi_ricavi() As DataRow
        Dim dr_search_agenda_dettaglio() As DataRow
        Dim Prodotto_Raccolto As String = ""
        Dim Mat_Cod_Last As Integer = 0
        Dim Mat_Cod_Costo As Integer = 0
        Dim Costo As Decimal = 0
        Dim bOk As Boolean = False
        Dim ht As New Hashtable
        Dim Key As String = ""
        Dim Valore_Ripartito As Decimal = 0

        'Costruzione DataTable dtEnergia
        dtEnergia.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Cul_Cod", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Coltura_Des", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Prodotto_Raccolto", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Priorita", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Intestazione", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Piva", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Appezzamento", GetType(String)))
        dtEnergia.Columns.Add(New DataColumn("Superficie", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("Sommabile", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("Produzione", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("T_Ha", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("M3_T", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("M3", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("Costo", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("Costo_Ton", GetType(Decimal)))
        dtEnergia.Columns.Add(New DataColumn("Costo_Ha", GetType(Decimal)))


        'Costruzione DataTable dtMercato
        dtMercato.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Cul_Cod", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Coltura_Des", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Prodotto_Raccolto", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Priorita", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Intestazione", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Piva", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Appezzamento", GetType(String)))
        dtMercato.Columns.Add(New DataColumn("Superficie", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("Sommabile", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("Produzione", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("T_Ha", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("Costo", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("Costo_Ton", GetType(Decimal)))
        dtMercato.Columns.Add(New DataColumn("Costo_Ha", GetType(Decimal)))



        'Lettura Raccolte
        If id_budget = 0 Then
            DtProduzione = ObjReg_Impianti.LeggiProduzione_SPV("", -1000, 0, data_validita_inizio, validita_fine, objParametri_Server, "Mat_Cod")
        End If

        'lettura Dati
        'Nota: Ordinamento importante per controllo duplicati su tabella imprese_progetti
        xOrderBy = " Veg_Cod, Padre, Piva, Cul_Cod, App_Nome, Sa_Cod, Appezza, Id_Reg "

        DtDati = ObjReg_Impianti.LeggixReportProduzioneColturaCampo(_azienda, id_budget, includiAziendeFiglie, validita_inizio, validita_fine, xOrderBy, objParametri_Server)

        If DtDati.Rows.Count <> 0 Then

            For Each dr As DataRow In DtDati.Rows

                Totale_Qta_Raccolta = 0
                bOk = False

                If DtProduzione.Rows.Count > 0 Then

                    Prodotto_Raccolto = ""

                    Search_Impianto = "Piva = '" & dr.Item("Piva") & "' And " &
                                      "Sa_Cod = " & dr.Item("Sa_Cod") & " And " &
                                      "Appezza = " & dr.Item("Appezza") & " And " &
                                      "Id_Destinazione = " & dr.Item("Id_Reg")

                    If Trim(Last_Impianto) <> Trim(Search_Impianto) Then

                        Mat_Cod_Last = 0
                        Costo = 0
                        Mat_Cod_Costo = 0
                        ht.Clear()


                        '=====================================================================================================================
                        'Determinazione costo
                        '---------------------------------------------------------------------------------------------------------------------
                        If dtCostiRicavi.Rows.Count > 0 Then

                            dr_search_costi_ricavi = dtCostiRicavi.Select(Replace(Search_Impianto, "Id_Destinazione", "Id_Reg"))

                            If dr_search_costi_ricavi.Length > 0 Then



                                For Each dr_costi_ricavi In dr_search_costi_ricavi

                                    Select Case dr_costi_ricavi("Lav_Cod_Agenda")

                                        Case 125

                                            'In caso di raccolta il costo è associato al prodotto relativo all'operazione di raccolta
                                            dr_search_agenda = DtProduzione.Select(Search_Impianto & " And Id_Agenda = " & dr_costi_ricavi.Item("Id_Agenda_Agenda"))
                                            '^^^^^^^^ RIGA CRUSH CRUSH CRUSH
                                        Case Else

                                            'Cerco il prodotto raccolta con priorita 0                                                        
                                            dr_search_agenda = DtProduzione.Select(Search_Impianto & " And Priorita = 0")

                                            If dr_search_agenda.Length = 0 Then

                                                'Nessuna raccolta con prodotto avente priorita 0 -->Rilasso il vincolo                                                       
                                                dr_search_agenda = DtProduzione.Select(Search_Impianto)

                                            End If

                                    End Select


                                    Valore_Ripartito = Impostazione_Valore(dr_costi_ricavi("FlagSecondoRaccolto"),
                                                                           dr_costi_ricavi("Frazionabile"),
                                                                           dr_costi_ricavi("Id_Imputazione"),
                                                                           dr_costi_ricavi("Id_Reg"),
                                                                           dr_costi_ricavi("Data_Inserimento"),
                                                                           dr_costi_ricavi("Progetto_Validita_Inizio"),
                                                                           dr_costi_ricavi("Progetto_Validita_Fine"),
                                                                           validita_inizio,
                                                                           validita_fine,
                                                                           validita_fine_cdg,
                                                                           CDbl(dr_costi_ricavi("Valore")),
                                                                           objParametri_Server)



                                    If dr_search_agenda.Length <> 0 Then

                                        Key = dr_search_agenda(0)("Mat_Cod") & "_" & dr_search_agenda(0)("Priorita")

                                        If Not ht.ContainsKey(Key) Then
                                            ht.Add(Key, Valore_Ripartito)
                                        Else
                                            ht.Item(Key) = ht.Item(Key) + Valore_Ripartito
                                        End If
                                    Else

                                        'Costo senza raccolta
                                        Costo = Costo + Valore_Ripartito

                                    End If

                                Next

                            End If

                        End If
                        '=====================================================================================================================

                    End If

                    Last_Impianto = Search_Impianto

                    dr_search_agenda = DtProduzione.Select(Search_Impianto)

                    Select Case dr_search_agenda.Length

                        Case 0 'Qta Prevista Kg/Ha

                            Totale_Qta_Raccolta = dr("Produzione_Prevista") * dr("Sup_Imp")
                            Mat_Cod_Last = 0

                            AggiungiRecordReport3(dtEnergia, dtMercato, dr, DettaglioSpecieVarieta, Totale_Qta_Raccolta, "", Costo, 0)

                        Case Else

                            For Each dr_agenda In dr_search_agenda

                                If Mat_Cod_Last <> dr_agenda.Item("Mat_Cod") Then

                                    Mat_Cod_Last = dr_agenda.Item("Mat_Cod")

                                    dr_search_agenda_dettaglio = DtProduzione.Select(Search_Impianto & " And Mat_Cod = " & dr_agenda.Item("Mat_Cod"))

                                    Prodotto_Raccolto = dr_search_agenda_dettaglio(0).Item("Mat_Des")
                                    Totale_Qta_Raccolta = 0

                                    For Each dr_raccolta In dr_search_agenda_dettaglio
                                        Totale_Qta_Raccolta = Totale_Qta_Raccolta + dr_raccolta("Qta")
                                    Next

                                    'Valore in TON
                                    Totale_Qta_Raccolta = Totale_Qta_Raccolta / 1000

                                    'Determinazione Costo del Mat_Cod
                                    Key = dr_agenda("Mat_Cod") & "_" & dr_agenda("Priorita")

                                    If ht.ContainsKey(Key) Then
                                        AggiungiRecordReport3(dtEnergia, dtMercato, dr, DettaglioSpecieVarieta, Totale_Qta_Raccolta, Prodotto_Raccolto, ht.Item(Key), CInt(dr_agenda("Priorita")))
                                    Else
                                        Key = ""
                                    End If



                                End If

                            Next

                    End Select

                End If

            Next

        End If
    End Sub

    Private Shared Sub AggiungiRecordReport3(ByRef dtEnergia As DataTable,
                                             ByRef dtMercato As DataTable,
                                             ByVal dr As DataRow,
                                             ByVal DettaglioSpecieVarieta As Integer,
                                             ByVal Totale_Qta_Raccolta As Decimal,
                                             ByVal Prodotto_Raccolto As String,
                                             ByVal Costo As Decimal,
                                             ByVal Priorita As Integer)
        Dim dr_Energia As DataRow
        Dim dr_Mercato As DataRow


        'Smistamento in base alla finalità produttiva
        Select Case dr.Item("GRFI_COD")

            Case 24 'Energia

                'Inserimento Record HA  
                dr_Energia = dtEnergia.NewRow

                dr_Energia.Item("Veg_Cod") = dr.Item("Veg_Cod")
                dr_Energia.Item("Cul_Cod") = dr.Item("Cul_Cod")
                dr_Energia.Item("Coltura_Des") = dr.Item("Specie")
                dr_Energia.Item("Prodotto_Raccolto") = Prodotto_Raccolto
                dr_Energia.Item("Priorita") = Priorita

                If DettaglioSpecieVarieta = 1 Then
                    dr_Energia.Item("Coltura_Des") = dr_Energia.Item("Coltura_Des") & " - " & dr.Item("Varieta")
                End If

                dr_Energia.Item("Piva") = dr.Item("Piva")
                dr_Energia.Item("Intestazione") = dr.Item("Rag_Soc")

                dr_Energia.Item("Appezzamento") = dr.Item("App_Nome")
                dr_Energia.Item("Superficie") = dr.Item("Sup_Imp")


                Select Case Priorita
                    Case 0
                        dr_Energia.Item("Sommabile") = dr.Item("Sup_Imp")
                    Case Else
                        dr_Energia.Item("Sommabile") = 0
                End Select

                dr_Energia.Item("Produzione") = Totale_Qta_Raccolta
                dr_Energia.Item("T_Ha") = Totale_Qta_Raccolta / dr.Item("Sup_Imp")

                dr_Energia.Item("M3") = dr.Item("Potenziale_Metanigeno") * dr_Energia.Item("Produzione")
                dr_Energia.Item("M3_T") = dr.Item("Potenziale_Metanigeno")

                dr_Energia.Item("Costo") = Costo
                If Totale_Qta_Raccolta <> 0 Then
                    dr_Energia.Item("Costo_Ton") = Costo / Totale_Qta_Raccolta
                Else
                    dr_Energia.Item("Costo_Ton") = 0
                End If
                If dr.Item("Sup_Imp") <> 0 Then
                    dr_Energia.Item("Costo_Ha") = Costo / dr.Item("Sup_Imp")
                Else
                    dr_Energia.Item("Costo_Ha") = 0
                End If


                dtEnergia.Rows.Add(dr_Energia)


            Case Else 'Mercato


                'Inserimento Record HA  
                dr_Mercato = dtMercato.NewRow

                dr_Mercato.Item("Veg_Cod") = dr.Item("Veg_Cod")
                dr_Mercato.Item("Cul_Cod") = dr.Item("Cul_Cod")

                dr_Mercato.Item("Coltura_Des") = dr.Item("Specie")
                dr_Mercato.Item("Prodotto_Raccolto") = Prodotto_Raccolto
                dr_Mercato.Item("Priorita") = Priorita

                If DettaglioSpecieVarieta = 1 Then
                    dr_Mercato.Item("Coltura_Des") = dr_Mercato.Item("Coltura_Des") & " - " & dr.Item("Varieta")
                End If


                dr_Mercato.Item("Piva") = dr.Item("Piva")
                dr_Mercato.Item("Intestazione") = dr.Item("Rag_Soc")

                dr_Mercato.Item("Appezzamento") = dr.Item("App_Nome")
                dr_Mercato.Item("Superficie") = dr.Item("Sup_Imp")

                Select Case Priorita
                    Case 0
                        dr_Mercato.Item("Sommabile") = dr.Item("Sup_Imp")
                    Case Else
                        dr_Mercato.Item("Sommabile") = 0
                End Select

                dr_Mercato.Item("Produzione") = Totale_Qta_Raccolta
                dr_Mercato.Item("T_Ha") = Totale_Qta_Raccolta / dr.Item("Sup_Imp")

                dr_Mercato.Item("Costo") = Costo
                If Totale_Qta_Raccolta <> 0 Then
                    dr_Mercato.Item("Costo_Ton") = Costo / Totale_Qta_Raccolta
                Else
                    dr_Mercato.Item("Costo_Ton") = 0
                End If
                If dr.Item("Sup_Imp") <> 0 Then
                    dr_Mercato.Item("Costo_Ha") = Costo / dr.Item("Sup_Imp")
                Else
                    dr_Mercato.Item("Costo_Ha") = 0
                End If

                dtMercato.Rows.Add(dr_Mercato)

        End Select

    End Sub

    Private Shared Function Impostazione_Valore(ByVal Primo_Raccolto As Boolean,
                                                ByVal Frazionabile As Integer,
                                                ByVal Id_Imputazione As Integer,
                                                ByVal Id_Reg As Integer,
                                                ByVal Data_Intervento As String,
                                                ByVal Validita_Inizio_Impianto As String,
                                                ByVal Validita_Fine_Impianto As String,
                                                ByVal Validita_Inizio_AA As String,
                                                ByVal Validita_Fine_AA As String,
                                                ByVal Validita_Fine_CDG As String,
                                                ByVal Valore As Decimal,
                                                ByVal objParametri_Server As AgronicaCoreParametri) As Decimal

        Const nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Impostazione_Valore()"
        Dim messaggioErrore As String = ""

        Dim Valore_Ripartito As Decimal = 0
        Dim Giorni_Validita_Impianto As Integer = 0
        Dim Giorni_Target As Integer = 0

        Try

            If Valore <> 0 Then

                'Controllo Imputazione Impianto
                If Id_Imputazione = 0 And Id_Reg <> 0 Then


                    '########################################################################################################
                    '################################# CONTROLLO FRAZIONABILE ###############################################
                    '########################################################################################################

                    Select Case Frazionabile

                        Case 1

                            Select Case Primo_Raccolto

                                Case True

                                    'Controllo che la data di fine impianto sia entro la data fine di AA
                                    If CDate(Validita_Fine_Impianto) <= CDate(Validita_Fine_AA) Then

                                        'Determinazione Valore Ripartito                            
                                        Giorni_Validita_Impianto = DateDiff("d", CDate(Validita_Inizio_Impianto), CDate(Validita_Fine_Impianto))
                                        Giorni_Target = DateDiff("d", CDate(Validita_Inizio_Impianto), CDate(Validita_Fine_CDG))

                                        Valore_Ripartito = Valore * Giorni_Target / Giorni_Validita_Impianto

                                    End If

                                Case False

                                    'Controllo che la data di fine impianto sia entro la data fine di AA
                                    If CDate(Validita_Fine_Impianto) <= CDate(Validita_Fine_AA) Then

                                        'Determinazione Valore Ripartito                            
                                        Giorni_Validita_Impianto = DateDiff("d", CDate(Validita_Inizio_Impianto), CDate(Validita_Fine_Impianto))
                                        Giorni_Target = DateDiff("d", CDate(Validita_Inizio_Impianto), CDate(Validita_Fine_CDG))

                                        Valore_Ripartito = Valore * Giorni_Target / Giorni_Validita_Impianto


                                    End If

                            End Select


                        Case Else

                            Select Case Primo_Raccolto

                                Case True

                                    'Controllo che la data di fine impianto sia entro la data fine di AA
                                    If CDate(Validita_Fine_Impianto) <= CDate(Validita_Fine_AA) Then

                                        Valore_Ripartito = Valore

                                    End If


                                Case False

                                    'Controllo che la data sia entro la data fine
                                    If CDate(Validita_Fine_Impianto) <= CDate(Validita_Fine_AA) Then

                                        Valore_Ripartito = Valore

                                    End If

                            End Select


                    End Select

                Else

                    'Progetto
                    Valore_Ripartito = Valore

                End If

                'Controllo Coerenza
                If Valore_Ripartito < 0 Then
                    Throw New Exception
                End If

            End If

            Return Format(Valore_Ripartito, "#######0.00")


        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


    End Function

End Class

