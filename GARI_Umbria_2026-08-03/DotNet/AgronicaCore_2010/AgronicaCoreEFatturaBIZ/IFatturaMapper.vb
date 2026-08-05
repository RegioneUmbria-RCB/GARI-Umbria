Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL

Public Interface IFatturaMapper

    ReadOnly Property DecodificheMapper() As IDecodificheMapper
    ReadOnly Property ModuloGenerazione() As enum_Omni_Modulo_Generazione

    ReadOnly Property TipoImpresaGerarchia() As enum_TipoImpresaGerarchia
    Sub Inizializza(ByVal Piva As String, ByVal agendeXLavCod As List(Of AgendaXLavCod))
    Function MappaFatturaAttiva(ByVal fatturaGias As FatturaGias, ByVal tipoFattura As FatturaElettronicaType, ByVal id_cod_cliente As Integer) As IFatturaElettronica
    Function ContattoCodiciPerPIVA(ByVal PIVA As String, ByVal cod_Contatto As String) As List(Of Contatto_Codice)
    Function RegimeFiscaleXSezionale(ByVal PIVA As String, ByVal sezionale As Integer?) As Integer
End Interface
