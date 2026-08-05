Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEFatturaDAL.SDI_Log_Helper

Namespace Persisters
    Public Interface IPersister

        Function Persist(ByVal fattura As FatturaGias, ByVal nomeFile As String, ByVal stato As StatoFattura_Gias)

        Function Persist(ByVal fattura As IFatturaElettronica,
                         ByVal fatturaType As AgronicaCoreEFatturaDAL.FatturaElettronicaType,
                         Optional ByVal nomeFile As String = "") As Boolean

        Function Persist(ByVal nomeFile As String, ByVal data As Byte())

        Function Persist(ByVal nomeFile As String, ByVal data As Byte(), ByVal dataSDI As Date)

        Function Update(ByVal entita As AgronicaCoreEntityFramework_POCO.SDI_Log) As String

    End Interface

End Namespace



