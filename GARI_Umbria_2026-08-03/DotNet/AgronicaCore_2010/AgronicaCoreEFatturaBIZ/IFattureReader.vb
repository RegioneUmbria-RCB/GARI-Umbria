Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL

Public Interface IDataReader

    Function LeggiAgendeXLavCod(ByVal filtro As AgendaFiltroLettura) As GenerazioneFatture
    Function LeggiAgenda(ByVal agenda As AgendaXLavCod, ByVal tipoImpresa As enum_TipoImpresaGerarchia) As FatturaGiasFromDB
    Function CaricaProvince() As List(Of Lista_Province)
    Function CaricaComuni() As List(Of ISTAT)
    Function CaricaTipologieDocumento() As List(Of TipologiaDocumento)
    Function CaricaRegimiFiscali(ByVal PIVA As String) As List(Of RegimeFiscaleXSezionale)
    Function CaricaContattiCodici(ByVal PIVA As String) As List(Of Contatto_Codice)
    Function CaricaUnitaMisura() As List(Of UnitaMisura)
    Function CaricaAliquteIVA() As List(Of IVA)
    Function CaricaRappresentantiFiscali(ByVal PIVA As String) As List(Of RappresentanteFiscale)
    Function CaricaStabiliOrganizzazioni(ByVal PIVA As String, ByVal risorseUmane As List(Of Integer))

    Function CaricaSezionali(ByVal PIVA As String) As List(Of Imprese_Sezionali)
    Function LeggiModuloGenerazione() As enum_Omni_Modulo_Generazione

    Function LeggiTipoImpresaGerarchia(ByVal PIVA As String) As enum_TipoImpresaGerarchia
    Function LeggiDescrizioneBeneServizio(ByVal movimento As Movimenti_dettagli, ByRef flag_extra As Boolean, ByVal modulo_generazione As enum_Omni_Modulo_Generazione) As String
    Function LeggiDataAttivazione(ByVal PIVA As String) As DateTime

    Function LeggiConfigurazioneServizi(ByVal pivaSuperUser As String, ByVal tipoServizio As enum_Tipi_Servizi_Background) As List(Of Configurazione_Servizio)

    Function LeggiAgendeErrateDopoXGiorni(ByVal PIVA As String, ByVal periodoControlloGG As Integer) As ControlloPreliminare


End Interface


