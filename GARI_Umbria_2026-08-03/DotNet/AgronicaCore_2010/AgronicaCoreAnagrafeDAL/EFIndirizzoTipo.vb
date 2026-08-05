Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class EFIndirizzoTipo
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function CreateIndirizzoTipoEF(ByRef dal As Gias_DeveloperServer_Entities,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 ByVal piva As String,
                                                 ByVal codContatto As String,
                                                 ByRef username As String
                                                 ) As IndirizzoTipo

        Dim ind As New IndirizzoTipo
        Dim idGen As New Agro_Sequenze

        ind.IndirizzoTipo_Cod = idGen.NuovoId_Tabella_EF(dal, "IndirizzoTipo", 2000, 2000000000, objParametri)
        ind.cod_contatto = codContatto
        ind.piva = piva

        ind.Descrizione = ""
        ind.Contatto_tipo = 0

        ind.inviato = 0
        ind.Data_Creazione = Now
        ind.Data_Modifica = Now
        ind.Validita_Inizio = AGRODATAINIZIO
        ind.Validita_Fine = AGRODATAFINE
        ind.Username_Creazione = username
        ind.Username_Modifica = username

        dal.IndirizzoTipo.Add(ind)
        dal.SaveChanges()

        Return ind

    End Function

End Class
