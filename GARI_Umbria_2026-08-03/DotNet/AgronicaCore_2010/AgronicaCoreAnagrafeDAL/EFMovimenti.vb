Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFMovimenti
    Public Shared Function CreateMovimenti(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As String,
                                                   ByRef id_agenda As Integer,
                                                   ByRef cod_RisUm As Integer,
                                                   ByRef cau_Mov As String,
                                                   ByRef Mov_Desc As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Movimenti
        Dim mov = New AgronicaCoreEntityFramework_POCO.Movimenti
        mov.PIVA = piva
        mov.Sa_Cod = sa_cod
        mov.Id_Agenda = id_agenda
        Dim idGen = New AgronicaCoreDataProvider.Agro_Sequenze
        mov.Id_Mov = idGen.NuovoId_Tabella_EF(dal, "Movimenti", 0, 2000000, objParametri)
        mov.Cod_RisUm = cod_RisUm
        mov.Cau_Mov = cau_Mov
        mov.Mov_Desc = Mov_Desc
        mov.Data_Movimento = DateTime.Now
        mov.Scadenza = DateTime.Now
        mov.Doc_Numero = 0
        mov.Num_Protocollo = 0
        mov.inviato = 0
        mov.Data_Creazione = DateTime.Now
        mov.Data_Modifica = DateTime.Now
        mov.Username_Creazione = username
        mov.Username_Modifica = username
        mov.Validita_Inizio = AGRODATAINIZIO
        mov.Validita_Fine = AGRODATAFINE
        mov.Cod_IndirizzoRisUm = 0
        mov.Cod_Destinazione = 0
        mov.Cod_IndirizzoDestinazione = 0
        mov.Cod_Vettore = 0
        mov.Cod_IndirizzoVettore = 0
        mov.Causale_Trasporto = ""
        mov.Aspetto = ""
        mov.Peso = 0
        mov.Ora = DateTime.Now
        mov.Colli = 0
        mov.Extra_Str = ""
        mov.Extra_Int = 0
        mov.Extra_Date = AGRODATAINIZIO
        mov.Tipo_Sconto = 0
        mov.Doc_Numero_Des = ""
        mov.Natura_Beni = ""
        mov.Tara_Veicolo = 0
        mov.Tara_Imballi = 0
        mov.Tipo_Peso = 0
        mov.Modalita = 0
        mov.Username_Note = ""
        mov.Scadenza_Extra = AGRODATAINIZIO
        mov.Doc_Numero_Sin = ""
        mov.Progr_Protocollo = 0
        mov.Progr_Registrazione = 0
        mov.Data_Registrazione = AGRODATAINIZIO
        mov.ChkLayOut_Bypass_Fatturato = 0
        mov.ChkLayOut_Join_Prodotti = 0
        mov.Cod_RisUm_Altro = 0
        mov.ChkLayOut_Peso = 0
        mov.ChkLayOut_Prezzo = 0
        mov.ChkFiltro_Varietale = 0
        mov.Disciplinare_PubblicoPrivato = 0
        mov.Sezionale_Cod = 0
        mov.Causale_Trasporto_Cod = 0
        mov.ChkLayOut_Litri = 0
        dal.Movimenti.Add(mov)
        dal.SaveChanges()
        Return mov
    End Function

    Public Shared Function CreateMovimentiDettagli(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef movimento As AgronicaCoreEntityFramework_POCO.Movimenti,
                                                   ByRef Elem_cod As Integer,
                                                   ByRef Pro_cod As Integer,
                                                   ByRef Mat_Cod As Integer,
                                                   ByRef Mov_Det_Des As String,
                                                   ByRef Udm_Cod As Integer,
                                                   ByRef Qta As Integer,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli
        Dim movDet = EFMovimenti_Dettagli.CreateMovimenti_Dettagli(dal, objParametri, movimento.PIVA, movimento.Sa_Cod, movimento.Id_Agenda, movimento.Id_Mov, Elem_cod, Pro_cod, Mat_Cod, Mov_Det_Des, Udm_Cod, Qta, username)
        Return movDet
    End Function

End Class
