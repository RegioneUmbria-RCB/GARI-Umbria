Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO

Public Interface IDecodificheMapper

    ReadOnly Property ModuloGenerazione() As enum_Omni_Modulo_Generazione

    ReadOnly Property TipoImpresaGerarchia() As enum_TipoImpresaGerarchia
    Function Inizializza(ByVal PIVA As String, ByVal agendeXLavCod As List(Of AgendaXLavCod), ByRef errori As String) As Boolean
    Function GetDecodificaValue(ByVal nomeTabella As String, ByVal valoreGias As String) As Integer

    Function ContattoCodiciPerPIVA(ByVal PIVA As String, ByVal cod_Contatto As String) As List(Of Contatto_Codice)
    
    Function DecodificaTipoDocumento(ByVal codiceTipoDoc As Integer) As FatturaPa.TipoDocumentoType

    Function RegimeFiscaleXSezionale(ByVal PIVA As String, ByVal sezionale As Integer?) As Integer

    Function DecodificaRegimeFiscale(ByVal PIVA As String, ByVal codice_Sezionale As Integer, ByVal codice_RegimeFiscale As Integer) As FatturaPa.RegimeFiscaleType

    Function DecodificaUnitaMisura(ByVal codice As Integer) As String

    Function DecodificaAliquotaIva(ByVal codice As Integer) As Decimal

    Function DecodificaNaturaEsclusione(ByVal codiceIva As Integer) As FatturaPa.NaturaType

    Function DecodificaNaturaEsclusionePerDichiarazioneIntento(ByVal codiceIva As Integer) As FatturaPa.NaturaType

    Function DecodificaRiferimentoNormativo(ByVal codiceIva As Integer) As String
    Function DecodificaEsigibilitaIVA(ByVal codiceSezionale As Integer) As FatturaPa.EsigibilitaIVAType

    Function OttieniStabileOrganizzazione(ByVal cod_Contatto As String, ByVal cod_RisUM As Integer) As StabileOrganizzazione

    Function OttieniRappresentanteFiscale(ByVal cod_RisUm As Integer) As RappresentanteFiscale

    Function OttieniProvincia(ByVal pro_cod_istat As String) As Lista_Province

    Function OttieniComune(ByVal pro_cod_istat As String, ByVal com_cod_istat As String) As ISTAT

    Function DecodificaModalitaPagamento(ByVal tipo As enum_PagamentiCausali) As FatturaPa.ModalitaPagamentoType

    Function OttieniSezionale(ByVal sezionale_Cod As Integer) As Imprese_Sezionali

End Interface


