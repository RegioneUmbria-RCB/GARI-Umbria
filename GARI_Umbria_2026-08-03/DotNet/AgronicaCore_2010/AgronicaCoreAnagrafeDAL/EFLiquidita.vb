Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFLiquidita
    Public Shared Function CreateLiquidita(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef piva As String,
                                           ByRef sa_cod As Integer,
                                           ByRef riferimento As String,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Liquidita
        Dim liq As New AgronicaCoreEntityFramework_POCO.Liquidita
        liq.Piva = piva
        liq.Sa_Cod = sa_cod
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim cod_liquidita = idGen.NuovoId_Tabella_EF(dal, "Liquidita", 0, 20000000, objParametri)
        liq.Cod_Liquidita = cod_liquidita
        liq.Riferimento = riferimento
        liq.Numero = ""
        liq.Abi = ""
        liq.Cab = ""
        liq.Interbancario = ""
        liq.Saldo_Attuale = 0
        liq.Saldo_Iniziale = 0
        liq.Cau_Risorsa = 0
        liq.Cod_Istituto = 0
        liq.Avviso = 0
        liq.Importo_Avviso = 0
        liq.Note = ""
        liq.Cin = ""
        liq.Cifre_Controllo = ""
        liq.Nazione = ""
        liq.Bic = ""
        liq.Cod_Contatto = riferimento
        liq.Rilevamento = 0
        liq.Data_Rilevamento = AGRODATAINIZIO
        liq.Offset = 0
        liq.ChkAbilitazione = 1
        liq.ChkDefault = 0
        liq.Inviato = 0
        liq.Validita_Inizio = AGRODATAINIZIO
        liq.Validita_Fine = AGRODATAFINE
        liq.Username_Creazione = username
        liq.Username_Modifica = username
        dal.Liquidita.Add(liq)
        dal.SaveChanges()
        Return liq
    End Function
End Class
