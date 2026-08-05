Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFMovimenti_Dettagli
    Public Shared Function CreateMovimenti_Dettagli(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As String,
                                                   ByRef id_agenda As Integer,
                                                   ByRef id_mov As Integer,
                                                   ByRef Elem_cod As Integer,
                                                   ByRef Pro_cod As Integer,
                                                   ByRef Mat_Cod As Integer,
                                                   ByRef Mov_Det_Des As String,
                                                   ByRef Udm_Cod As Integer,
                                                   ByRef Qta As Integer,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli
        Dim mov As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli
        mov.PIVA = piva
        mov.Sa_Cod = sa_cod
        mov.Id_Agenda = id_agenda
        mov.Id_Mov = id_mov
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        mov.Id_Mov_Det = idGen.NuovoId_Tabella_EF(dal, "Movimenti_dettagli", 0, 2000000, objParametri)
        mov.Elem_Cod = Elem_cod
        mov.Pro_Cod = Pro_cod
        mov.Mat_Cod = Mat_Cod
        mov.Mov_Det_Des = Mov_Det_Des
        mov.Udm_Cod = Udm_Cod
        mov.Qta = Qta
        mov.Cod_Iva = 0
        mov.Sconto = 0
        mov.Prezzo_Unitario = 0
        mov.Cod_Conto = 0
        mov.Cod_Progetto = 0
        mov.Fase_Cod = 0
        mov.Contabilizzato = 0
        mov.Pendente = 0
        mov.inviato = 0
        mov.Data_Creazione = DateTime.Now
        mov.Data_Modifica = DateTime.Now
        mov.Username_Creazione = username
        mov.Username_Modifica = username
        mov.Validita_Inizio = AGRODATAINIZIO
        mov.Validita_Fine = AGRODATAFINE
        mov.Cal_Cod = 0
        mov.Extra_Str = ""
        mov.Extra_Int = 0
        mov.Extra_Date = AGRODATAINIZIO
        mov.Anno = 0
        mov.Ric_Cod = 0
        mov.Imponibile = 0
        mov.Iva = 0
        mov.Lotto = ""
        mov.Jolly_Int = 0
        mov.Imponibile_Netto = 0
        mov.Prezzo_Unitario_Netto = 0
        mov.UDM_COD_EXTRA = 0
        mov.QTA_EXTRA = 0
        mov.Prezzo_Effettivo = 0
        mov.ChkIva_Manuale = 0
        mov.Cod_IvaIndetraibile = 0
        mov.Qta_Extra_Totale = 0
        mov.Tara = 0
        mov.ChkLayOut_Hide = 0
        mov.Variazione = 0
        mov.Listino_Cod = 0
        mov.Sconto_Listino = 0
        mov.Sconto_Modalita = 0
        mov.Mat_Cod_Alias = 0
        mov.Mezzo_Det = 0
        mov.Sconto_Testo = ""
        mov.Ric_Cod_Pat = 0
        mov.Cod_Conto_Pat = 0
        mov.TempoCarenza = 0
        mov.DoseEtichetta = ""
        mov.Turno_Cod = 0
        mov.ID_Attivita = 0
        mov.Dettaglio_VegCod = 0
        mov.Iva_Indetraibile = 0
        mov.Iva_Indetraibile_Perc = 0
        mov.PrincipiAttivi = ""
        mov.ClassiTossicologiche = ""
        mov.DoseEtichetta_Value = ""
        mov.Iva_Deto_Cod = 0
        mov.Qta_Dettaglio1 = 0
        mov.Qta_Dettaglio2 = 0
        mov.Dettagli_Blocco_Flag = 0
        mov.Dettagli_Blocco_Username = ""
        mov.Dettagli_Blocco_Data = AGRODATAINIZIO
        mov.Qualifica_Cod = 0
        mov.Tariffa_Cod = 0
        mov.Polverulento = 0
        dal.Movimenti_dettagli.Add(mov)
        dal.SaveChanges()
        Return mov
    End Function

    Public Shared Function CreateMov_Destinazioni(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                                           ByRef Appezza As Integer,
                                           ByRef Id_Destinazione As Integer,
                                           ByRef Tipo_Destinazione As Integer,
                                           ByRef Qta As Integer,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Mov_Destinazioni
        Dim dest = EFMov_Destinazioni.CreateMov_Destinazioni(dal, objParametri, movimenti_dettagli.PIVA, movimenti_dettagli.Sa_Cod, movimenti_dettagli.Id_Agenda, movimenti_dettagli.Id_Mov, movimenti_dettagli.Id_Mov_Det, Appezza, Id_Destinazione, Tipo_Destinazione, Qta, username)
        Return dest
    End Function

End Class
