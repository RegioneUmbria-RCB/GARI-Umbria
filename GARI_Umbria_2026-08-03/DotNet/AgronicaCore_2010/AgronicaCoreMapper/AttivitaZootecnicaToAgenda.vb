Imports System.Text
Imports System.Transactions
Imports AgronicaCoreContabDAL
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreMapper.My.Resources.AgronicaCoreMapper
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.costanti
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.exceptions

Public Class AttivitaZootecnicaToAgenda

    Public Function ScriviAttivitaZootecnicaToAgenda(ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                     ByVal objParametri_Server As AgronicaCoreParametri,
                                                     Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                                     Optional ByVal listaParamsAggiuntiviAttivita As List(Of Parametri_Aggiuntivi_Attivita) = Nothing,
                                                     Optional ByVal idAgenda As Integer = 0,
                                                     Optional ByVal origine As String = "",
                                                     Optional ByVal verificaGiacenzeSpostamento As Boolean = True) As Integer
        Dim Id_Agenda As Integer
        Dim Lav_Cod As Integer = CInt(attivita.job.primaryKey.codice)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
        transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim StartTransaction As DateTime
        Dim EndTransaction As DateTime

        Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
        StartTransaction = DateTime.Now
        Dim confermaTransazione = True

        Dim Agenda As New AgronicaCoreEntityFramework_POCO.Agenda

        Try
            If Not IsNothing(attivita.centriDiCosto) Then

                'Ricavo il Des_Lib tramite Lav_Cod
                Dim objLavorazione = New AgronicaCoreAnagrafeBIZ.Lavorazione_R
                Dim Des_Lib As String = objLavorazione.GetLavDes(Lav_Cod, objParametri_Server)

                '-------------------------------------------------------------------------
                'AGENDA
                '-------------------------------------------------------------------------
                Agenda.PIVA = attivita.centroAziendale.primaryKey.partitaIva
                Agenda.Sa_Cod = If(IsNothing(attivita.centroAziendale.primaryKey.codice), 0, attivita.centroAziendale.primaryKey.codice)
                Agenda.Sta_Num = If(IsNothing(attivita.fabbricatoCod), 0, attivita.fabbricatoCod)
                Agenda.Id_Agenda = idAgenda
                Agenda.Lav_Cod = Lav_Cod
                Agenda.des_lib = Des_Lib
                Agenda.Blocco_Data = AGRODATAINIZIO
                Agenda.Data_Creazione = DateTime.Now
                Agenda.Data_Modifica = DateTime.Now
                Agenda.Validita_Inizio = attivita.inizio
                Agenda.Validita_Fine = attivita.fine
                Agenda.Origine = origine
                Agenda.inviato = 0

                Dim objModello4 As New JObject
                If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso listaParamsAggiuntiviAttivita.Count > 0 Then
                    For Each p In listaParamsAggiuntiviAttivita
                        If p.key = Key_Parametri_Aggiuntivi_Attivita.sincro_modello4 Then
                            objModello4 = JsonConvert.DeserializeObject(Of JObject)(p.value)
                            Agenda.Blocco_Flag = 1
                            Agenda.Tipo_Accettazione = 1
                            Exit For
                        End If
                    Next
                End If

                Dim objScrivi_Agenda As New AgronicaCoreContabBIZ.Agenda_W
                Id_Agenda = objScrivi_Agenda.Scrivi_Modifica(Agenda,
                                                             GiasContext,
                                                             objParametri_Server)
                'GiasContext.SaveChanges()

                '-------------------------------------------------------------------------
                'AGENDA LOG
                '-------------------------------------------------------------------------
                Dim logAgenda As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Agenda
                logAgenda.ID = Id_Agenda
                logAgenda.Sa_Cod = Agenda.Sa_Cod
                logAgenda.SuperUser = objParametri_Server.PivaSuperUser
                logAgenda.Utente = Agenda.PIVA
                logAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                logAgenda.Data_Ora_Lavorazione = Agenda.Validita_Inizio
                logAgenda.Data_Ora_RegistrazioneLog = DateTime.Now
                logAgenda.Lav_Cod = Lav_Cod
                logAgenda.Des_lib = Des_Lib
                logAgenda.Id_Servizio = enum_Id_Servizio.GiasOnline

                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim jsonAgenda = JsonConvert.SerializeObject(Agenda, a)
                logAgenda.object_data = jsonAgenda.ToString

                GiasContext.Entry(logAgenda).State = Entity.EntityState.Added
                'GiasContext.SaveChanges()

                Select Case Lav_Cod
                    '----------------------------ZOO_CARICO----------------------------
                    Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO,
                         LAVCOD_NASCITA_ANIMALI,
                         LAVCOD_ACQUISTO_ANIMALI

                        '-------------------------------------------------------------------------
                        'MOVIMENTI----CARICO
                        '-------------------------------------------------------------------------
                        Dim Movimenti_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti
                        Movimenti_Carico.PIVA = Agenda.PIVA
                        Movimenti_Carico.Sa_Cod = 0
                        Movimenti_Carico.Id_Agenda = Id_Agenda
                        Movimenti_Carico.Cau_Mov = CAU_CARICO_CONSISTENZE
                        Movimenti_Carico.Mov_Desc = Des_Lib
                        Movimenti_Carico.Mezzo = 1
                        Movimenti_Carico.Data_Movimento = attivita.inizio
                        Movimenti_Carico.Scadenza = AGRODATAFINE
                        Movimenti_Carico.Username_Creazione = Agenda.Username_Creazione
                        Movimenti_Carico.Data_Creazione = DateTime.Now
                        Movimenti_Carico.Username_Modifica = Agenda.Username_Modifica
                        Movimenti_Carico.Data_Modifica = DateTime.Now
                        Movimenti_Carico.Ora = DateTime.Now
                        Movimenti_Carico.Data_Registrazione = DateTime.Now
                        Movimenti_Carico.Validita_Inizio = AGRODATAINIZIO
                        Movimenti_Carico.Validita_Fine = AGRODATAFINE
                        Movimenti_Carico.Extra_Date = AGRODATAINIZIO
                        Movimenti_Carico.Scadenza_Extra = AGRODATAFINE
                        Movimenti_Carico.TipoDocumento = 0
                        Movimenti_Carico.inviato = 0

                        Dim objScrivi_Movimenti_Carico As New AgronicaCoreContabBIZ.Movimenti_W
                        Dim Id_Mov_Carico As Integer = objScrivi_Movimenti_Carico.Scrivi_Modifica(Movimenti_Carico,
                                                                                                  GiasContext,
                                                                                                  objParametri_Server)
                        'GiasContext.SaveChanges()

                        If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso Not IsNothing(objModello4) AndAlso objModello4.Item("numModello") <> "" Then
                            Dim numModello As String = objModello4.Item("numModello")
                            Dim prenotazioneId As Integer = CInt(objModello4.Item("prenotazioneId"))

                            Dim numAutorizzazione As String = If(objModello4.Item("numAutorizzazione").Type = JTokenType.Null, "", objModello4.Item("numAutorizzazione"))
                            Dim codAzienda_Dest As String = objModello4.Item("codAzienda_Dest")
                            Dim idFiscaleAzienda_Dest As String = objModello4.Item("idFiscaleAzienda_Dest")
                            Dim targa As String = If(objModello4.Item("targa").Type = JTokenType.Null, "", objModello4.Item("targa"))
                            Dim flagMezzoProprio As String = objModello4.Item("flagMezzoProprio")
                            Dim durataViaggio As String = objModello4.Item("durataViaggio")
                            Dim codAsl_Trasp As String = objModello4.Item("codAsl_Trasp")

                            Dim Movimenti_Registrazione As New AgronicaCoreEntityFramework_POCO.Movimenti
                            Movimenti_Registrazione.PIVA = Agenda.PIVA
                            Movimenti_Registrazione.Sa_Cod = 0
                            Movimenti_Registrazione.Id_Agenda = Id_Agenda
                            Movimenti_Registrazione.Cau_Mov = CAU_REGISTRAZIONI
                            Movimenti_Registrazione.Mov_Desc = Des_Lib
                            Movimenti_Registrazione.Causale_Trasporto = Des_Lib
                            Movimenti_Registrazione.Mezzo = 1
                            Movimenti_Registrazione.Data_Movimento = attivita.inizio
                            Movimenti_Registrazione.Scadenza = attivita.inizio
                            Movimenti_Registrazione.Username_Creazione = Agenda.Username_Creazione
                            Movimenti_Registrazione.Data_Creazione = DateTime.Now
                            Movimenti_Registrazione.Username_Modifica = Agenda.Username_Modifica
                            Movimenti_Registrazione.Data_Modifica = DateTime.Now
                            Movimenti_Registrazione.Ora = DateTime.Now
                            Movimenti_Registrazione.Data_Registrazione = attivita.inizio
                            Movimenti_Registrazione.Validita_Inizio = attivita.inizio
                            Movimenti_Registrazione.Validita_Fine = AGRODATAFINE
                            Movimenti_Registrazione.Extra_Str = numModello
                            Movimenti_Registrazione.Extra_Date = attivita.inizio
                            Movimenti_Registrazione.Ora = attivita.inizio
                            Movimenti_Registrazione.Scadenza_Extra = AGRODATAFINE
                            Movimenti_Registrazione.TipoDocumento = 0
                            Movimenti_Registrazione.inviato = 0

                            Dim Id_Mov_Registrazione As Integer = objScrivi_Movimenti_Carico.Scrivi_Modifica(Movimenti_Registrazione,
                                                                                                             GiasContext,
                                                                                                             objParametri_Server)

                            Dim Mov_Dettaglio_TecnExtra As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra
                            'Dim idRegDTE As Integer = (From m In GiasContext.Mov_Dettaglio_Tecnico_Extra
                            '                           Order By m.Id_Reg_Dettaglio Descending
                            '                           Select m.Id_Reg_Dettaglio).FirstOrDefault
                            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

                            Dim idRegDTE As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "movimenti_dettagli_tecnici_extra",
                                                                                    0, 200000000, objParametri_Server)

                            Mov_Dettaglio_TecnExtra.Id_Agenda = Agenda.Id_Agenda
                            Mov_Dettaglio_TecnExtra.Sa_Cod = 0
                            Mov_Dettaglio_TecnExtra.Piva = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.Id_Mov = Id_Mov_Registrazione
                            Mov_Dettaglio_TecnExtra.Id_Mov_Det = 0
                            Mov_Dettaglio_TecnExtra.Id_Reg_Dettaglio = idRegDTE
                            Mov_Dettaglio_TecnExtra.validita_inizio = attivita.inizio
                            Mov_Dettaglio_TecnExtra.validita_fine = AGRODATAFINE
                            Mov_Dettaglio_TecnExtra.Num_Riferimento = prenotazioneId
                            Mov_Dettaglio_TecnExtra.data_creazione = DateTime.Now
                            Mov_Dettaglio_TecnExtra.username_creazione = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.data_modifica = DateTime.Now
                            Mov_Dettaglio_TecnExtra.username_modifica = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.Regione = ""
                            Mov_Dettaglio_TecnExtra.Trasportatore = ""
                            Mov_Dettaglio_TecnExtra.Mezzo_Trasporto = ""
                            Mov_Dettaglio_TecnExtra.Targa = targa
                            Mov_Dettaglio_TecnExtra.N_Autorizzazione_Trasporto = numAutorizzazione
                            Mov_Dettaglio_TecnExtra.Annotazioni = If(IsNothing(flagMezzoProprio), "", flagMezzoProprio)
                            Mov_Dettaglio_TecnExtra.Durata_Viaggio = durataViaggio
                            Mov_Dettaglio_TecnExtra.ASL = ""
                            Mov_Dettaglio_TecnExtra.Serie = ""
                            Mov_Dettaglio_TecnExtra.Numero = ""
                            Mov_Dettaglio_TecnExtra.N_Immatricolazione = ""
                            Mov_Dettaglio_TecnExtra.N_Immatricolazione_Rimorchio = ""
                            Mov_Dettaglio_TecnExtra.Data_Rilascio_Autorizzazione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Peso = 0
                            Mov_Dettaglio_TecnExtra.inviato = 0
                            Mov_Dettaglio_TecnExtra.Codice_Prodotto = 0
                            Mov_Dettaglio_TecnExtra.Colore = 0
                            Mov_Dettaglio_TecnExtra.Zona_Viticola = ""
                            Mov_Dettaglio_TecnExtra.Manipolazioni = 0
                            Mov_Dettaglio_TecnExtra.Precisazioni = ""
                            Mov_Dettaglio_TecnExtra.Num_Contenitori = 0
                            Mov_Dettaglio_TecnExtra.Marche_Contenitori = ""
                            Mov_Dettaglio_TecnExtra.Des_Contenitori = ""
                            Mov_Dettaglio_TecnExtra.Tipo_Documento = ""
                            Mov_Dettaglio_TecnExtra.Id_Cod_Autorita = 0
                            Mov_Dettaglio_TecnExtra.Luogo_Partenza = ""
                            Mov_Dettaglio_TecnExtra.Luogo_Consegna = ""
                            Mov_Dettaglio_TecnExtra.Data_Spedizione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Indicazioni_Complementari = ""
                            Mov_Dettaglio_TecnExtra.Titolo_Alcol = 0
                            Mov_Dettaglio_TecnExtra.Codice_NC = ""
                            Mov_Dettaglio_TecnExtra.Data_Dichiarazione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Garanzia = ""
                            Mov_Dettaglio_TecnExtra.Certificati = ""
                            Mov_Dettaglio_TecnExtra.Peso_Lordo = 0
                            Mov_Dettaglio_TecnExtra.Num_Colli = 0
                            Mov_Dettaglio_TecnExtra.Contenitore_Cod = 0
                            Mov_Dettaglio_TecnExtra.Imballaggio_Cod = 0
                            Mov_Dettaglio_TecnExtra.Agente_Cod = 0
                            Mov_Dettaglio_TecnExtra.Provvigione = 0
                            Mov_Dettaglio_TecnExtra.Tipo_Trasporto = 0
                            Mov_Dettaglio_TecnExtra.Unita_Trasporto = 0
                            Mov_Dettaglio_TecnExtra.Codice_Alternativo = ""
                            Mov_Dettaglio_TecnExtra.Id_Gestione_Vettore = 0
                            Mov_Dettaglio_TecnExtra.Ritenuta_Acconto_Cod = 0
                            Mov_Dettaglio_TecnExtra.Ritenuta_Acconto = 0
                            Mov_Dettaglio_TecnExtra.Enasarco_Cod = 0
                            Mov_Dettaglio_TecnExtra.Enasarco = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinatario = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinazione = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
                            Mov_Dettaglio_TecnExtra.CapoArea_Cod = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_CapoArea = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_Pagata_Agente = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_Pagata_CapoArea = 0
                            Mov_Dettaglio_TecnExtra.N_Doc_Cliente = ""
                            Mov_Dettaglio_TecnExtra.Data_Doc_Cliente = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.N_Nota_Fattura = ""
                            Mov_Dettaglio_TecnExtra.Data_Nota_Fattura = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.N_Nota_DDT = ""
                            Mov_Dettaglio_TecnExtra.N_Nota_Riga_DDT = ""
                            Mov_Dettaglio_TecnExtra.Data_Nota_DDT = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Causale_Fattura = 0
                            Mov_Dettaglio_TecnExtra.N_Doc_Ente = ""
                            Mov_Dettaglio_TecnExtra.Anno_Doc_Ente = 2100
                            Mov_Dettaglio_TecnExtra.Num_Conf_Riscontrate = -1
                            Mov_Dettaglio_TecnExtra.Num_Colli_Riscontrati = -1
                            Mov_Dettaglio_TecnExtra.Num_Imballi_Riscontrati = -1
                            Mov_Dettaglio_TecnExtra.Peso_Netto_Riscontrato = 0
                            Mov_Dettaglio_TecnExtra.Peso_Lordo_Riscontrato = 0
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Conf_Riscontrata = -1
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Collo_Riscontrata = -1
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Imballo_Riscontrata = -1

                            GiasContext.Mov_Dettaglio_Tecnico_Extra.Add(Mov_Dettaglio_TecnExtra)
                            'GiasContext.SaveChanges()
                        End If

                        Dim Lista_IdMov_Dett As New List(Of Integer)
                        Dim listResult = New List(Of String)

                        For Each cdc In attivita.centriDiCosto
                            If cdc.classType.Equals(costanti.ClassType.CapoAnimaleCDC) Then
                                Dim start = DateTime.Now

                                Dim CapoAnimaleCDC As centri_di_costo.CapoAnimaleCDC = CType(cdc, centri_di_costo.CapoAnimaleCDC)
                                Dim CapoAnimale As CapoAnimale = CapoAnimaleCDC.capoAnimale

                                Dim Sa_Cod As Integer = CapoAnimaleCDC.codice.intValue

                                '-------------------------------------------------------------------------
                                'CAPO_ANIMALE
                                '-------------------------------------------------------------------------
                                CapoAnimale.validita.inizio = Agenda.Validita_Inizio
                                CapoAnimale.validita.fine = AGRODATAFINE
                                CapoAnimale.flagCancellazione = False

                                Dim objScrivi_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
                                Dim Cod_Progetto As Integer = objScrivi_Zoo.Converti_Animale_DT(CapoAnimale,
                                                                                                objParametri_Server,
                                                                                                GiasContext,
                                                                                                False, "ScriviAttivitaZootecnicaToAgenda ID_Agenda=" & Agenda.Id_Agenda & " Lav_Cod=" & Agenda.Lav_Cod)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOVIMENTI_DETTAGLI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Movimenti_Dettagli_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                                Movimenti_Dettagli_Carico.PIVA = Movimenti_Carico.PIVA
                                Movimenti_Dettagli_Carico.Sa_Cod = Sa_Cod
                                Movimenti_Dettagli_Carico.Cod_Progetto = Cod_Progetto
                                Movimenti_Dettagli_Carico.Elem_Cod = ZOO_CONSISTENZA
                                Movimenti_Dettagli_Carico.Id_Agenda = Id_Agenda
                                Movimenti_Dettagli_Carico.Id_Mov = Movimenti_Carico.Id_Mov
                                Movimenti_Dettagli_Carico.Mov_Det_Des = CapoAnimale.specie.descrizione & "- [" & CapoAnimale.razza.descrizione & "]"
                                Movimenti_Dettagli_Carico.Udm_Cod = 38
                                Movimenti_Dettagli_Carico.Qta = 1
                                Movimenti_Dettagli_Carico.Cod_Iva = 98
                                Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooConsistenzeIniziali
                                Select Case Lav_Cod
                                    Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooConsistenzeIniziali
                                    Case LAVCOD_NASCITA_ANIMALI
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooNascita
                                    Case LAVCOD_ACQUISTO_ANIMALI
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooAcquistoAnimali
                                End Select

                                Movimenti_Dettagli_Carico.Contabilizzato = CONTABILE
                                Movimenti_Dettagli_Carico.Username_Creazione = Agenda.Username_Creazione
                                Movimenti_Dettagli_Carico.Data_Creazione = Date.Now
                                Movimenti_Dettagli_Carico.Username_Modifica = Agenda.Username_Modifica
                                Movimenti_Dettagli_Carico.Data_Modifica = Date.Now
                                Movimenti_Dettagli_Carico.Anno = Date.Now.Year
                                Movimenti_Dettagli_Carico.Validita_Inizio = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Validita_Fine = AGRODATAFINE
                                Movimenti_Dettagli_Carico.Extra_Date = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Ric_Cod = 2
                                Movimenti_Dettagli_Carico.Ric_Cod_Pat = 2
                                Movimenti_Dettagli_Carico.Cod_Conto_Pat = 75
                                Movimenti_Dettagli_Carico.Dettagli_Blocco_Data = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Prezzo_Livello = -1
                                Movimenti_Dettagli_Carico.Mat_Cod_Alias = 0
                                Movimenti_Dettagli_Carico.Dettagli_Blocco_Username = 0
                                Movimenti_Dettagli_Carico.PrincipiAttiviPercAbb = ""
                                Movimenti_Dettagli_Carico.inviato = 0
                                Movimenti_Dettagli_Carico.Jolly_Int = 0
                                Movimenti_Dettagli_Carico.Id_Mov_Esterno = CapoAnimaleCDC.id_movimentazione_BDN

                                Dim objScrivi_MovimentiDett_Carico As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                                Dim Id_Mov_Det As Integer = objScrivi_MovimentiDett_Carico.Scrivi_Modifica(Movimenti_Dettagli_Carico,
                                                                                                           GiasContext,
                                                                                                           objParametri_Server)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOV_DESTINAZIONI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Mov_Destinazioni_Carico As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                                Mov_Destinazioni_Carico.Piva = Movimenti_Dettagli_Carico.PIVA
                                Mov_Destinazioni_Carico.Sa_Cod = Movimenti_Dettagli_Carico.Sa_Cod
                                Mov_Destinazioni_Carico.Id_Agenda = Id_Agenda
                                Mov_Destinazioni_Carico.Id_Mov = Id_Mov_Carico
                                Mov_Destinazioni_Carico.Id_Mov_Det = Id_Mov_Det
                                Mov_Destinazioni_Carico.Appezza = 0
                                Mov_Destinazioni_Carico.Id_Destinazione = CapoAnimaleCDC.sottogruppoStalla_ingresso.codice
                                Mov_Destinazioni_Carico.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                                Mov_Destinazioni_Carico.Qta = Movimenti_Dettagli_Carico.Qta
                                Mov_Destinazioni_Carico.username_creazione = Agenda.Username_Creazione
                                Mov_Destinazioni_Carico.data_creazione = Date.Now
                                Mov_Destinazioni_Carico.username_modifica = Agenda.Username_Modifica
                                Mov_Destinazioni_Carico.data_modifica = Date.Now
                                Mov_Destinazioni_Carico.validita_inizio = AGRODATAINIZIO
                                Mov_Destinazioni_Carico.validita_fine = AGRODATAFINE
                                Mov_Destinazioni_Carico.Sa_Cod_Riferimento = 0
                                Mov_Destinazioni_Carico.Id_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Carico.Tipo_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Carico.Sup_Riduzione_BufferZone = 0
                                Mov_Destinazioni_Carico.Perc_Riduzione_Deriva = 0
                                Mov_Destinazioni_Carico.inviato = 0

                                Dim objScrivi_MovDestinazioni_Carico As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                                objScrivi_MovDestinazioni_Carico.Scrivi_Modifica(Mov_Destinazioni_Carico,
                                                                                 GiasContext,
                                                                                 objParametri_Server)
                                'GiasContext.SaveChanges()

                                Lista_IdMov_Dett.Add(Id_Mov_Det)
                                Dim Movimenti_Dettagli_Carico_Del = From c In GiasContext.Movimenti_dettagli
                                                                    Where c.Id_Agenda = Id_Agenda _
                                                                        AndAlso c.Id_Mov = Id_Mov_Carico _
                                                                        AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                                                                    Select c
                                'GiasContext.SaveChanges()

                                Dim endIteration = DateTime.Now
                                listResult.Add((start - endIteration).TotalMilliseconds)

                            End If
                        Next

                        '----------------------------ZOO_SCARICO----------------------------
                    Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO,
                         LAVCOD_MORTE_ANIMALI,
                         LAVCOD_MACELLAZIONE_ANIMALI,
                         LAVCOD_VENDITA_ANIMALI

                        '-------------------------------------------------------------------------
                        'MOVIMENTI----SCARICO
                        '-------------------------------------------------------------------------
                        Dim Movimenti_Scarico As New AgronicaCoreEntityFramework_POCO.Movimenti

                        Movimenti_Scarico.PIVA = Agenda.PIVA
                        Movimenti_Scarico.Sa_Cod = 0
                        Movimenti_Scarico.Id_Agenda = Id_Agenda
                        Movimenti_Scarico.Cau_Mov = CAU_SCARICO_CONSISTENZE
                        Movimenti_Scarico.Mov_Desc = Des_Lib
                        Movimenti_Scarico.Mezzo = 1
                        Movimenti_Scarico.Modalita = attivita.modalita
                        Movimenti_Scarico.Data_Movimento = attivita.inizio
                        Movimenti_Scarico.Scadenza = AGRODATAFINE
                        Movimenti_Scarico.Username_Creazione = Agenda.Username_Creazione
                        Movimenti_Scarico.Data_Creazione = DateTime.Now
                        Movimenti_Scarico.Username_Modifica = Agenda.Username_Modifica
                        Movimenti_Scarico.Data_Modifica = DateTime.Now
                        Movimenti_Scarico.Ora = DateTime.Now
                        Movimenti_Scarico.Data_Registrazione = DateTime.Now
                        Movimenti_Scarico.Validita_Inizio = attivita.inizio
                        Movimenti_Scarico.Validita_Fine = AGRODATAFINE
                        Movimenti_Scarico.Extra_Date = AGRODATAINIZIO
                        Movimenti_Scarico.Scadenza_Extra = AGRODATAFINE
                        Movimenti_Scarico.TipoDocumento = 0
                        Movimenti_Scarico.inviato = 0

                        Dim objScrivi_Movimenti_Scarico As New AgronicaCoreContabBIZ.Movimenti_W
                        Dim Id_Mov_Scarico As Integer = objScrivi_Movimenti_Scarico.Scrivi_Modifica(Movimenti_Scarico,
                                                                                                    GiasContext,
                                                                                                    objParametri_Server)
                        'GiasContext.SaveChanges()

                        If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso Not IsNothing(objModello4) AndAlso objModello4.Item("numModello") <> "" Then
                            Dim numModello As String = objModello4.Item("numModello")
                            Dim prenotazioneId As Integer = CInt(objModello4.Item("prenotazioneId"))
                            Dim numAutorizzazione As String = objModello4.Item("numAutorizzazione")
                            Dim codAzienda_Dest As String = objModello4.Item("codAzienda_Dest")
                            Dim idFiscaleAzienda_Dest As String = objModello4.Item("idFiscaleAzienda_Dest")
                            Dim targa As String = objModello4.Item("targa")
                            Dim flagMezzoProprio As String = objModello4.Item("flagMezzoProprio")
                            Dim durataViaggio As String = objModello4.Item("durataViaggio")
                            Dim codAsl_Trasp As String = objModello4.Item("codAsl_Trasp")

                            'TODO

                            '-----CONTATTO DESTINATARIO-----
                            Dim tipoDestinazione As Integer = 0
                            If objModello4.Item("tipoDestinazione") = "MACELLO" Then
                                tipoDestinazione = COD_MACELLO
                            Else
                                tipoDestinazione = COD_ALLEVATORE
                            End If

                            Dim objContatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
                            Dim dtContatto As DataTable =
                                objContatti_R.Leggi(Agenda.PIVA, idFiscaleAzienda_Dest,
                                                    0, tipoDestinazione, True, True,
                                                    0, 0, False, 0, CInt(-99),
                                                    0, "", False, 0,
                                                    0, 0, 0, 0, AGRODATAINIZIO,
                                                    AGRODATAFINE, False,
                                                    "", "", objParametri_Server)

                            Dim codDestinazione As Integer = 0
                            If Not IsNothing(dtContatto) OrElse dtContatto.Rows.Count > 0 Then
                                Dim objRisorseUmane_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                                Dim dtRisUm As DataTable =
                                    objRisorseUmane_R.Leggi3(Agenda.PIVA, idFiscaleAzienda_Dest,
                                                             0, tipoDestinazione, "", "",
                                                             objParametri_Server)

                                If Not IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count > 0 Then
                                    codDestinazione = dtRisUm(0)("Cod_RisUm")
                                End If
                            End If

                            '-----CONTATTO TRASPORTATORE-----
                            Dim pivaAsl_Trasp As String = ""
                            Dim denom_Trasp As String = ""
                            Dim macCod As Integer = 0
                            Dim codVettore As Integer = 0
                            Dim parcoMacchine = (From m In GiasContext.Parco_Macchine
                                                 Where m.Piva = Agenda.PIVA AndAlso m.Targa = objModello4.Item("targa")
                                                 Select m).First
                            If Not IsNothing(parcoMacchine) Then
                                pivaAsl_Trasp = parcoMacchine.Cod_Contatto
                                macCod = parcoMacchine.Mac_Cod

                                dtContatto =
                                    objContatti_R.LeggiContattoSpecifico(Agenda.PIVA, pivaAsl_Trasp,
                                                                        0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "", "", objParametri_Server)

                                'If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
                                '	denom_Trasp = dtContatto(0)("Rag_Soc")

                                '	Dim objRisorseUmane_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                                '	Dim dtRisUm As DataTable =
                                '		objRisorseUmane_R.Leggi3(Agenda.PIVA, pivaAsl_Trasp,
                                '								 0, COD_TRASPORTATORE, "", "",
                                '								 objParametri_Server)
                                '	If IsNothing(dtRisUm) OrElse dtRisUm.Rows.Count = 0 Then
                                '		'creo la risorsa umana

                                '	Else
                                '		codVettore = dtRisUm(0)("Cod_RisUm")
                                '	End If

                                'End If
                            End If

                            Dim Movimenti_Registrazione As New AgronicaCoreEntityFramework_POCO.Movimenti
                            Movimenti_Registrazione.PIVA = Agenda.PIVA
                            Movimenti_Registrazione.Sa_Cod = 0
                            Movimenti_Registrazione.Id_Agenda = Id_Agenda
                            Movimenti_Registrazione.Cau_Mov = CAU_REGISTRAZIONI
                            Movimenti_Registrazione.Mov_Desc = Des_Lib
                            Movimenti_Registrazione.Causale_Trasporto = Des_Lib
                            Movimenti_Registrazione.Mezzo = 1
                            Movimenti_Registrazione.Cod_Destinazione = codDestinazione
                            Movimenti_Registrazione.Mezzo = macCod
                            Movimenti_Registrazione.Cod_Vettore = codVettore
                            Movimenti_Registrazione.Data_Movimento = attivita.inizio
                            Movimenti_Registrazione.Scadenza = attivita.inizio
                            Movimenti_Registrazione.Username_Creazione = Agenda.Username_Creazione
                            Movimenti_Registrazione.Data_Creazione = DateTime.Now
                            Movimenti_Registrazione.Username_Modifica = Agenda.Username_Modifica
                            Movimenti_Registrazione.Data_Modifica = DateTime.Now
                            Movimenti_Registrazione.Ora = DateTime.Now
                            Movimenti_Registrazione.Data_Registrazione = attivita.inizio
                            Movimenti_Registrazione.Validita_Inizio = attivita.inizio
                            Movimenti_Registrazione.Validita_Fine = AGRODATAFINE
                            Movimenti_Registrazione.Extra_Str = numModello
                            Movimenti_Registrazione.Extra_Date = attivita.inizio
                            Movimenti_Registrazione.Ora = attivita.inizio
                            Movimenti_Registrazione.Scadenza_Extra = AGRODATAFINE
                            Movimenti_Registrazione.TipoDocumento = 0
                            Movimenti_Registrazione.inviato = 0

                            Dim Id_Mov_Registrazione As Integer = objScrivi_Movimenti_Scarico.Scrivi_Modifica(Movimenti_Registrazione,
                                                                                                              GiasContext,
                                                                                                              objParametri_Server)

                            Dim Mov_Dettaglio_TecnExtra As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra
                            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

                            Dim idRegDTE As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "mov_dettaglio_tecnico_extra",
                                                                                    0, 200000000, objParametri_Server)
                            Mov_Dettaglio_TecnExtra.Id_Agenda = Agenda.Id_Agenda
                            Mov_Dettaglio_TecnExtra.Sa_Cod = 0
                            Mov_Dettaglio_TecnExtra.Piva = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.Id_Mov = Id_Mov_Registrazione
                            Mov_Dettaglio_TecnExtra.Id_Mov_Det = 0
                            Mov_Dettaglio_TecnExtra.Id_Reg_Dettaglio = idRegDTE + 1
                            Mov_Dettaglio_TecnExtra.validita_inizio = attivita.inizio
                            Mov_Dettaglio_TecnExtra.validita_fine = AGRODATAFINE
                            Mov_Dettaglio_TecnExtra.Num_Riferimento = prenotazioneId
                            Mov_Dettaglio_TecnExtra.data_creazione = DateTime.Now
                            Mov_Dettaglio_TecnExtra.username_creazione = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.data_modifica = DateTime.Now
                            Mov_Dettaglio_TecnExtra.username_modifica = Movimenti_Registrazione.PIVA
                            Mov_Dettaglio_TecnExtra.Regione = ""
                            Mov_Dettaglio_TecnExtra.Trasportatore = pivaAsl_Trasp
                            Mov_Dettaglio_TecnExtra.Mezzo_Trasporto = IIf(denom_Trasp <> "", denom_Trasp & " " & targa, targa)
                            Mov_Dettaglio_TecnExtra.Targa = targa
                            Mov_Dettaglio_TecnExtra.N_Autorizzazione_Trasporto = numAutorizzazione
                            Mov_Dettaglio_TecnExtra.Annotazioni = flagMezzoProprio
                            Mov_Dettaglio_TecnExtra.Durata_Viaggio = durataViaggio
                            Mov_Dettaglio_TecnExtra.ASL = ""
                            Mov_Dettaglio_TecnExtra.Serie = ""
                            Mov_Dettaglio_TecnExtra.Numero = ""
                            Mov_Dettaglio_TecnExtra.N_Immatricolazione = ""
                            Mov_Dettaglio_TecnExtra.N_Immatricolazione_Rimorchio = ""
                            Mov_Dettaglio_TecnExtra.Data_Rilascio_Autorizzazione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Peso = 0
                            Mov_Dettaglio_TecnExtra.inviato = 0
                            Mov_Dettaglio_TecnExtra.Codice_Prodotto = 0
                            Mov_Dettaglio_TecnExtra.Colore = 0
                            Mov_Dettaglio_TecnExtra.Zona_Viticola = ""
                            Mov_Dettaglio_TecnExtra.Manipolazioni = 0
                            Mov_Dettaglio_TecnExtra.Precisazioni = ""
                            Mov_Dettaglio_TecnExtra.Num_Contenitori = 0
                            Mov_Dettaglio_TecnExtra.Marche_Contenitori = ""
                            Mov_Dettaglio_TecnExtra.Des_Contenitori = ""
                            Mov_Dettaglio_TecnExtra.Tipo_Documento = ""
                            Mov_Dettaglio_TecnExtra.Id_Cod_Autorita = 0
                            Mov_Dettaglio_TecnExtra.Luogo_Partenza = ""
                            Mov_Dettaglio_TecnExtra.Luogo_Consegna = ""
                            Mov_Dettaglio_TecnExtra.Data_Spedizione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Indicazioni_Complementari = ""
                            Mov_Dettaglio_TecnExtra.Titolo_Alcol = 0
                            Mov_Dettaglio_TecnExtra.Codice_NC = ""
                            Mov_Dettaglio_TecnExtra.Data_Dichiarazione = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Garanzia = ""
                            Mov_Dettaglio_TecnExtra.Certificati = ""
                            Mov_Dettaglio_TecnExtra.Peso_Lordo = 0
                            Mov_Dettaglio_TecnExtra.Num_Colli = 0
                            Mov_Dettaglio_TecnExtra.Contenitore_Cod = 0
                            Mov_Dettaglio_TecnExtra.Imballaggio_Cod = 0
                            Mov_Dettaglio_TecnExtra.Agente_Cod = 0
                            Mov_Dettaglio_TecnExtra.Provvigione = 0
                            Mov_Dettaglio_TecnExtra.Tipo_Trasporto = 0
                            Mov_Dettaglio_TecnExtra.Unita_Trasporto = 0
                            Mov_Dettaglio_TecnExtra.Codice_Alternativo = ""
                            Mov_Dettaglio_TecnExtra.Id_Gestione_Vettore = 0
                            Mov_Dettaglio_TecnExtra.Ritenuta_Acconto_Cod = 0
                            Mov_Dettaglio_TecnExtra.Ritenuta_Acconto = 0
                            Mov_Dettaglio_TecnExtra.Enasarco_Cod = 0
                            Mov_Dettaglio_TecnExtra.Enasarco = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinatario = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinazione = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
                            Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
                            Mov_Dettaglio_TecnExtra.CapoArea_Cod = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_CapoArea = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_Pagata_Agente = 0
                            Mov_Dettaglio_TecnExtra.Provvigione_Pagata_CapoArea = 0
                            Mov_Dettaglio_TecnExtra.N_Doc_Cliente = ""
                            Mov_Dettaglio_TecnExtra.Data_Doc_Cliente = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.N_Nota_Fattura = ""
                            Mov_Dettaglio_TecnExtra.Data_Nota_Fattura = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.N_Nota_DDT = ""
                            Mov_Dettaglio_TecnExtra.N_Nota_Riga_DDT = ""
                            Mov_Dettaglio_TecnExtra.Data_Nota_DDT = AGRODATAINIZIO
                            Mov_Dettaglio_TecnExtra.Causale_Fattura = 0
                            Mov_Dettaglio_TecnExtra.N_Doc_Ente = ""
                            Mov_Dettaglio_TecnExtra.Anno_Doc_Ente = 2100
                            Mov_Dettaglio_TecnExtra.Num_Conf_Riscontrate = -1
                            Mov_Dettaglio_TecnExtra.Num_Colli_Riscontrati = -1
                            Mov_Dettaglio_TecnExtra.Num_Imballi_Riscontrati = -1
                            Mov_Dettaglio_TecnExtra.Peso_Netto_Riscontrato = 0
                            Mov_Dettaglio_TecnExtra.Peso_Lordo_Riscontrato = 0
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Conf_Riscontrata = -1
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Collo_Riscontrata = -1
                            Mov_Dettaglio_TecnExtra.Tara_Unit_Imballo_Riscontrata = -1

                            GiasContext.Mov_Dettaglio_Tecnico_Extra.Add(Mov_Dettaglio_TecnExtra)
                            'GiasContext.SaveChanges()
                        End If

                        Dim Lista_IdMov_Dett As New List(Of Integer)

                        For Each cdc In attivita.centriDiCosto
                            If cdc.classType.Equals(costanti.ClassType.CapoAnimaleCDC) Then
                                Dim CapoAnimaleCDC As centri_di_costo.CapoAnimaleCDC = CType(cdc, centri_di_costo.CapoAnimaleCDC)
                                Dim CapoAnimale As CapoAnimale = CapoAnimaleCDC.capoAnimale

                                Dim Sa_Cod As Integer = CapoAnimaleCDC.codice.intValue

                                '-------------------------------------------------------------------------
                                'CAPO_ANIMALE
                                '-------------------------------------------------------------------------

                                'If IsNothing(CapoAnimale.validita) Then
                                '	CapoAnimale.validita = New IntervalloTemporale
                                'End If

                                'CapoAnimale.validita.fine = Agenda.Validita_Inizio
                                CapoAnimale.flagCancellazione = False

                                If IsNothing(CapoAnimale.codice) OrElse CapoAnimale.codice = 0 Then
                                    CapoAnimale.codice = (From z In GiasContext.Zoo_Animali
                                                          Select z
                                                          Where z.Matricola = CapoAnimale.matricola).First.Cod_Progetto
                                End If

                                Dim objScrivi_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
                                objScrivi_Zoo.Converti_Animale_DT(CapoAnimale,
                                                                  objParametri_Server,
                                                                  GiasContext, False, "ScriviAttivitaZootecnicaToAgenda ID_Agenda=" & Agenda.Id_Agenda & " Lav_Cod=" & Agenda.Lav_Cod)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOVIMENTI_DETTAGLI----SCARICO
                                '-------------------------------------------------------------------------
                                Dim Movimenti_Dettagli_Scarico As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                                Movimenti_Dettagli_Scarico.PIVA = Movimenti_Scarico.PIVA
                                Movimenti_Dettagli_Scarico.Sa_Cod = Sa_Cod
                                Movimenti_Dettagli_Scarico.Cod_Progetto = CapoAnimale.codice
                                Movimenti_Dettagli_Scarico.Elem_Cod = ZOO_CONSISTENZA
                                Movimenti_Dettagli_Scarico.Id_Agenda = Id_Agenda
                                Movimenti_Dettagli_Scarico.Id_Mov = Movimenti_Scarico.Id_Mov
                                Movimenti_Dettagli_Scarico.Mov_Det_Des = CapoAnimale.specie.descrizione & "- [" & CapoAnimale.razza.descrizione & "]"
                                Movimenti_Dettagli_Scarico.Udm_Cod = 38
                                Movimenti_Dettagli_Scarico.Qta = 1
                                Movimenti_Dettagli_Scarico.Cod_Iva = 98
                                Movimenti_Dettagli_Scarico.Contabilizzato = CONTABILE
                                Movimenti_Dettagli_Scarico.Username_Creazione = Agenda.Username_Creazione
                                Movimenti_Dettagli_Scarico.Data_Creazione = Date.Now
                                Movimenti_Dettagli_Scarico.Username_Modifica = Agenda.Username_Modifica
                                Movimenti_Dettagli_Scarico.Data_Modifica = Date.Now
                                Movimenti_Dettagli_Scarico.Anno = Date.Now.Year
                                Movimenti_Dettagli_Scarico.Validita_Inizio = Movimenti_Scarico.Data_Movimento
                                Movimenti_Dettagli_Scarico.Validita_Fine = AGRODATAFINE
                                Movimenti_Dettagli_Scarico.Extra_Date = AGRODATAINIZIO
                                Movimenti_Dettagli_Scarico.Ric_Cod = 2
                                Movimenti_Dettagli_Scarico.Ric_Cod_Pat = 2
                                Movimenti_Dettagli_Scarico.Cod_Conto_Pat = 75
                                Movimenti_Dettagli_Scarico.Dettagli_Blocco_Data = AGRODATAINIZIO
                                Movimenti_Dettagli_Scarico.Prezzo_Livello = -1
                                Movimenti_Dettagli_Scarico.Mat_Cod_Alias = 0
                                Movimenti_Dettagli_Scarico.Dettagli_Blocco_Username = 0
                                Movimenti_Dettagli_Scarico.PrincipiAttiviPercAbb = ""
                                Movimenti_Dettagli_Scarico.inviato = 0
                                Movimenti_Dettagli_Scarico.Jolly_Int = 0
                                Movimenti_Dettagli_Scarico.Id_Mov_Esterno = CapoAnimaleCDC.id_movimentazione_BDN

                                Dim objScrivi_MovimentiDett_Scarico As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                                Dim Id_Mov_Det As Integer = objScrivi_MovimentiDett_Scarico.Scrivi_Modifica(Movimenti_Dettagli_Scarico,
                                                                                                            GiasContext,
                                                                                                            objParametri_Server)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOV_DESTINAZIONI----SCARICO
                                '-------------------------------------------------------------------------
                                Dim Mov_Destinazioni_Scarico As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                                Mov_Destinazioni_Scarico.Piva = Movimenti_Dettagli_Scarico.PIVA
                                Mov_Destinazioni_Scarico.Sa_Cod = Movimenti_Dettagli_Scarico.Sa_Cod
                                Mov_Destinazioni_Scarico.Id_Agenda = Id_Agenda
                                Mov_Destinazioni_Scarico.Id_Mov = Id_Mov_Scarico
                                Mov_Destinazioni_Scarico.Id_Mov_Det = Id_Mov_Det
                                Mov_Destinazioni_Scarico.Appezza = 0
                                Mov_Destinazioni_Scarico.Id_Destinazione = CapoAnimaleCDC.sottogruppoStalla_uscita.codice
                                Mov_Destinazioni_Scarico.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                                Mov_Destinazioni_Scarico.Qta = Movimenti_Dettagli_Scarico.Qta
                                Mov_Destinazioni_Scarico.username_creazione = Agenda.Username_Creazione
                                Mov_Destinazioni_Scarico.data_creazione = Date.Now
                                Mov_Destinazioni_Scarico.username_modifica = Agenda.Username_Modifica
                                Mov_Destinazioni_Scarico.data_modifica = Date.Now
                                Mov_Destinazioni_Scarico.validita_inizio = Movimenti_Scarico.Data_Movimento
                                Mov_Destinazioni_Scarico.validita_fine = AGRODATAFINE
                                Mov_Destinazioni_Scarico.Sa_Cod_Riferimento = 0
                                Mov_Destinazioni_Scarico.Id_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Scarico.Tipo_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Scarico.Sup_Riduzione_BufferZone = 0
                                Mov_Destinazioni_Scarico.Perc_Riduzione_Deriva = 0
                                Mov_Destinazioni_Scarico.inviato = 0

                                Dim objScrivi_MovDestinazioni_Scarico As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                                objScrivi_MovDestinazioni_Scarico.Scrivi_Modifica(Mov_Destinazioni_Scarico,
                                                                                  GiasContext,
                                                                                  objParametri_Server)
                                'GiasContext.SaveChanges()

                                Lista_IdMov_Dett.Add(Id_Mov_Det)
                                Dim Movimenti_Dettagli_Scarico_Del = From c In GiasContext.Movimenti_dettagli
                                                                     Where c.Id_Agenda = Id_Agenda _
                                                                        AndAlso c.Id_Mov = Id_Mov_Scarico _
                                                                        AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                                                                     Select c

                            End If
                        Next

                        '----------------------------ZOO_TRATTAMENTI----------------------------
                    Case LAVCOD_CUREMEDICAMENTI_ANIMALI

                        ScriviTrattamentoFromAttivitaZoo(attivita, Agenda, objParametri_Server, objParametri_Utenti, GiasContext)
#Region "OLD Scrivi Trattamenti"
                        ''-------------------------------------------------------------------------
                        ''MOVIMENTI ---- SCARICO FARMACO ANIMALE
                        ''-------------------------------------------------------------------------
                        'Dim Movimenti_ScaricoFarm_Capo As New AgronicaCoreEntityFramework_POCO.Movimenti

                        'Movimenti_ScaricoFarm_Capo.PIVA = Agenda.PIVA
                        'Movimenti_ScaricoFarm_Capo.Sa_Cod = 0
                        'Movimenti_ScaricoFarm_Capo.Id_Agenda = Id_Agenda
                        'Movimenti_ScaricoFarm_Capo.Cau_Mov = CAU_TRATTAMENTO_ZOO
                        'Movimenti_ScaricoFarm_Capo.Mov_Desc = Des_Lib
                        'Movimenti_ScaricoFarm_Capo.Mezzo = 0
                        'Movimenti_ScaricoFarm_Capo.Data_Movimento = attivita.inizio
                        'Movimenti_ScaricoFarm_Capo.Scadenza = AGRODATAFINE
                        'Movimenti_ScaricoFarm_Capo.Username_Creazione = Agenda.Username_Creazione
                        'Movimenti_ScaricoFarm_Capo.Data_Creazione = DateTime.Now
                        'Movimenti_ScaricoFarm_Capo.Username_Modifica = Agenda.Username_Modifica
                        'Movimenti_ScaricoFarm_Capo.Data_Modifica = DateTime.Now
                        'Movimenti_ScaricoFarm_Capo.Ora = DateTime.Now
                        'Movimenti_ScaricoFarm_Capo.Data_Registrazione = DateTime.Now
                        'Movimenti_ScaricoFarm_Capo.Validita_Inizio = attivita.inizio
                        'Movimenti_ScaricoFarm_Capo.Validita_Fine = AGRODATAFINE
                        'Movimenti_ScaricoFarm_Capo.Extra_Date = AGRODATAINIZIO
                        'Movimenti_ScaricoFarm_Capo.Scadenza_Extra = AGRODATAFINE
                        'Movimenti_ScaricoFarm_Capo.TipoDocumento = 0
                        'Movimenti_ScaricoFarm_Capo.inviato = 0

                        'Dim objScrivi_Movimenti_ScaricoFarm As New AgronicaCoreContabBIZ.Movimenti_W
                        'Dim Id_Mov_ScaricoFarm_Capo As Integer = objScrivi_Movimenti_ScaricoFarm.Scrivi_Modifica(Movimenti_ScaricoFarm_Capo,
                        '                                                                                         GiasContext,
                        '                                                                                         objParametri_Server)
                        'GiasContext.SaveChanges()

                        'Dim Lista_IdMov_Dett As New List(Of Integer)

                        'For Each prod In attivita.risorse
                        '    If prod.classType.Equals(costanti.ClassType.DettaglioRegistroSomministrazioni) Then
                        '        Dim Somministrazione As dettagli.DettaglioRegistroSomministrazioni = CType(prod, dettagli.DettaglioRegistroSomministrazioni)
                        '        Agenda.Validita_Inizio = Somministrazione.validita.inizio
                        '        Agenda.Validita_Fine = Somministrazione.validita.fine

                        '        GiasContext.SaveChanges()

                        '        'recupera Pro_Cod nella tabella Farmaci
                        '        Dim objFarmaci_R As New AgronicaCoreMetaSchemaDAL.Farmaci
                        '        Dim dtFarmaco As DataTable = objFarmaci_R.leggi(objParametri_Server,
                        '                                                        0, Somministrazione.codiceAIC)

                        '        Dim objUnitaMisura_R As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                        '        If Not IsNothing(dtFarmaco) AndAlso dtFarmaco.Rows.Count > 0 Then
                        '            Dim Udm_Cod As Integer = 0
                        '            Dim quantitaConvertita As Decimal = 0
                        '            Dim Udm_Selezionata As Integer = 0
                        '            Select Case Somministrazione.unitaDiMisura.codice
                        '                Case enum_UnitaMisura.Litri
                        '                    'quantitaConvertita = objUnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, Somministrazione.quantitaTotaleReale,
                        '                    '											   enum_UnitaMisura.Litri)
                        '                    Udm_Cod = enum_UnitaMisura.Litri
                        '                    Udm_Selezionata = enum_UnitaMisura.Litri
                        '                Case enum_UnitaMisura.Numero
                        '                    Udm_Cod = enum_UnitaMisura.Numero
                        '                    Udm_Selezionata = enum_UnitaMisura.Numero
                        '                    quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '                Case enum_UnitaMisura.Grammi
                        '                    Udm_Cod = enum_UnitaMisura.KG
                        '                    Udm_Selezionata = enum_UnitaMisura.Grammi
                        '                    quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '                Case enum_UnitaMisura.Millilitri
                        '                    quantitaConvertita = objUnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, Somministrazione.quantitaTotaleReale,
                        '                                                                   enum_UnitaMisura.Litri)
                        '                    Udm_Cod = enum_UnitaMisura.Litri
                        '                    Udm_Selezionata = enum_UnitaMisura.Millilitri
                        '                Case Else
                        '                    Udm_Cod = Somministrazione.unitaDiMisura.codice
                        '                    quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '            End Select

                        '            'calcola la percentuale di prodotto scaricata sul singolo animale
                        '            Dim percSomministrazione As Decimal = 1 / attivita.centriDiCosto.Count
                        '            Dim Dose_Animale_Reale As Decimal = Somministrazione.quantitaTotaleReale / attivita.centriDiCosto.Count
                        '            '-------------------------------------------------------------------------
                        '            'MOVIMENTI_DETTAGLI ---- SCARICO FARMACO ANIMALE
                        '            '-------------------------------------------------------------------------
                        '            Dim Movimenti_Dettagli_ScaricoFarm_Capo As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                        '            Movimenti_Dettagli_ScaricoFarm_Capo.PIVA = Movimenti_ScaricoFarm_Capo.PIVA
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Sa_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Id_Agenda = Id_Agenda
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Id_Mov = Movimenti_ScaricoFarm_Capo.Id_Mov
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Elem_Cod = FARMACI
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Pro_Cod = dtFarmaco(0)("Farm_Cod")
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Mat_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Mov_Det_Des = dtFarmaco(0)("Denominazione") & " (" & dtFarmaco(0)("Confezione") & " - " & quantitaConvertita & ")"
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Udm_Cod = Udm_Cod
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Qta = Dose_Animale_Reale
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Qta_Extra_Totale = Somministrazione.quantitaTotaleReale
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Cod_Iva = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Sconto = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Jolly_Int = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Id_Mov_Esterno = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Cod_Conto = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Cod_Progetto = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Fase_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Contabilizzato = NONCONTABILE
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Pendente = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.inviato = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Username_Creazione = Agenda.Username_Creazione
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Data_Creazione = Date.Now
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Username_Modifica = Agenda.Username_Modifica
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Data_Modifica = Date.Now
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Validita_Inizio = AGRODATAINIZIO
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Validita_Fine = AGRODATAFINE
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Cal_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Extra_Str = Somministrazione.codice
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Extra_Int = Udm_Selezionata
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Extra_Date = Somministrazione.dataPrescrizione
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Anno = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Ric_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Imponibile = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Iva = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Lotto = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Prezzo_Unitario = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Imponibile_Netto = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Prezzo_Unitario_Netto = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.UDM_COD_EXTRA = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.QTA_EXTRA = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Prezzo_Effettivo = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.ChkIva_Manuale = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Cod_IvaIndetraibile = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Tara = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.ChkLayOut_Hide = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Variazione = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Listino_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Sconto_Listino = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Sconto_Modalita = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Mat_Cod_Alias = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Mezzo_Det = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Sconto_Testo = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Ric_Cod_Pat = 0
                        '            'Movimenti_Dettagli_ScaricoFarm.TempoCarenza = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.DoseEtichetta = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Turno_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.ID_Attivita = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Dettaglio_VegCod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Iva_Indetraibile = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Iva_Indetraibile_Perc = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.PrincipiAttivi = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.ClassiTossicologiche = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.DoseEtichetta_Value = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Iva_Deto_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Qta_Dettaglio1 = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Qta_Dettaglio2 = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Dettagli_Blocco_Flag = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Dettagli_Blocco_Username = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Dettagli_Blocco_Data = AGRODATAINIZIO
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Ordine_Det = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Deroga_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Prezzo_Livello = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Qualifica_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Tariffa_Cod = 0
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.PrincipiAttiviPesi = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Buffer = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Rif_Esterno = Somministrazione.numTrattamento
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.Rif_Esterno_2 = ""
                        '            Movimenti_Dettagli_ScaricoFarm_Capo.PrincipiAttiviPercAbb = ""

                        '            Dim objScrivi_MovimentiDett_ScaricoFarm_Capo As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                        '            Dim Id_Mov_Det As Integer = objScrivi_MovimentiDett_ScaricoFarm_Capo.Scrivi_Modifica(Movimenti_Dettagli_ScaricoFarm_Capo,
                        '                                                                                                 GiasContext, objParametri_Server)

                        '            If Somministrazione.sospensione IsNot Nothing AndAlso Somministrazione.sospensione.Count > 0 Then
                        '                For Each sospensione In Somministrazione.sospensione
                        '                    Dim mov_dettaglio_tecnico As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico
                        '                    mov_dettaglio_tecnico.Piva = Movimenti_ScaricoFarm_Capo.PIVA
                        '                    mov_dettaglio_tecnico.Sa_Cod = 0
                        '                    mov_dettaglio_tecnico.Id_Agenda = Id_Agenda
                        '                    mov_dettaglio_tecnico.Id_Mov = Movimenti_ScaricoFarm_Capo.Id_Mov
                        '                    mov_dettaglio_tecnico.Id_Mov_Det = Id_Mov_Det

                        '                    mov_dettaglio_tecnico.Dett_Cod = sospensione.Alimento.codice
                        '                    mov_dettaglio_tecnico.Extra_Int = sospensione.tempoSospensione
                        '                    Dim objScrivi_MovimentiDett_tecnico As New AgronicaCoreContabBIZ.Mov_Dettaglio_Tecnico_W
                        '                    objScrivi_MovimentiDett_tecnico.Scrivi_Modifica(mov_dettaglio_tecnico, GiasContext, objParametri_Server)
                        '                Next
                        '            End If

                        '            GiasContext.SaveChanges()

                        '            For Each cdc In attivita.centriDiCosto
                        '                If cdc.classType.Equals(costanti.ClassType.CapoAnimaleCDC) Then
                        '                    Dim CapoAnimaleCDC As centri_di_costo.CapoAnimaleCDC = CType(cdc, centri_di_costo.CapoAnimaleCDC)
                        '                    Dim CapoAnimale As CapoAnimale = CapoAnimaleCDC.capoAnimale
                        '                    'If CapoAnimale.codice = -1 Then
                        '                    '    Continue For
                        '                    'End If
                        '                    Dim QtaSuAnimale As Decimal = quantitaConvertita * percSomministrazione

                        '                    '-------------------------------------------------------------------------
                        '                    'MOV_DESTINAZIONI ---- SCARICO FARMACO ANIMALE
                        '                    '-------------------------------------------------------------------------

                        '                    Dim Mov_Destinazioni_ScaricoFarm_Capo As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Piva = Movimenti_Dettagli_ScaricoFarm_Capo.PIVA
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Sa_Cod = Movimenti_Dettagli_ScaricoFarm_Capo.Sa_Cod
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Id_Agenda = Id_Agenda
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Id_Mov = Id_Mov_ScaricoFarm_Capo
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Id_Mov_Det = Id_Mov_Det
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Appezza = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Id_Destinazione = CapoAnimale.codice
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Tipo_Destinazione = TIPO_DESTINAZIONE_ANIMALE
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Qta = QtaSuAnimale
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Qta2 = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.inviato = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.username_creazione = Agenda.Username_Creazione
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.data_creazione = Date.Now
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.username_modifica = Agenda.Username_Modifica
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.data_modifica = Date.Now
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.validita_inizio = Movimenti_ScaricoFarm_Capo.Data_Movimento
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.validita_fine = AGRODATAFINE
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Tipo_Scorta = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Scorta_Min = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.mov_destinazioni_graphickey = CapoAnimale.matricola
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Qta_Dest1 = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Qta_Dest2 = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.QuotaDistribuzione = percSomministrazione
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Sa_Cod_Riferimento = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Id_Destinazione_Riferimento = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Tipo_Destinazione_Riferimento = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Sup_Riduzione_BufferZone = 0
                        '                    Mov_Destinazioni_ScaricoFarm_Capo.Perc_Riduzione_Deriva = 0

                        '                    Dim objScrivi_MovDestinazioni_ScaricoFarm_Capo As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                        '                    objScrivi_MovDestinazioni_ScaricoFarm_Capo.Scrivi_Modifica(Mov_Destinazioni_ScaricoFarm_Capo,
                        '                                                                               GiasContext,
                        '                                                                               objParametri_Server)
                        '                    GiasContext.SaveChanges()

                        '                    Lista_IdMov_Dett.Add(Id_Mov_Det)
                        '                    Dim Movimenti_Dettagli_Scarico_Del = From c In GiasContext.Movimenti_dettagli
                        '                                                         Where c.Id_Agenda = Id_Agenda _
                        '                                                            AndAlso c.Id_Mov = Id_Mov_ScaricoFarm_Capo _
                        '                                                            AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                        '                                                         Select c

                        '                End If
                        '            Next
                        '        Else
                        '            Throw New Exception(FarmacoNonConfiguratoGIAS)

                        '        End If

                        '    ElseIf prod.classType.Equals(costanti.ClassType.RisorsaProdotto) Then
                        '        Dim Somministrazione As risorse.RisorsaProdotto = CType(prod, risorse.RisorsaProdotto)
                        '        Dim DettaglioSomministrazione As dettagli.DettaglioRegistroSomministrazioni = New dettagli.DettaglioRegistroSomministrazioni
                        '        Dim effettuaScaricoMagazzino As Boolean = False

                        '        'controllo che ci sia uno scarico farmaco su animale che corrisponda allo scarico farmaco da magazzino da effettuare
                        '        For Each ris In attivita.risorse
                        '            If ris.classType.Equals(costanti.ClassType.DettaglioRegistroSomministrazioni) Then
                        '                DettaglioSomministrazione = CType(ris, dettagli.DettaglioRegistroSomministrazioni)

                        '                If DettaglioSomministrazione.codice = Somministrazione.prodotto.codice_alfanumerico Then
                        '                    effettuaScaricoMagazzino = True
                        '                    Exit For

                        '                End If
                        '            End If
                        '        Next

                        '        If effettuaScaricoMagazzino Then

                        '            Dim Magazzino As Fabbricato = (Somministrazione.MagazziniMovimentazioni(0)).Magazzino
                        '            Dim Lotto As String = (Somministrazione.MagazziniMovimentazioni(0)).Lotto

                        '            'recupera Pro_Cod nella tabella Farmaci utilizzando DettaglioSomministrazione
                        '            Dim objFarmaci_R As New AgronicaCoreMetaSchemaDAL.Farmaci
                        '            Dim dtFarmaco As DataTable = objFarmaci_R.leggi(objParametri_Server,
                        '                                                            0, DettaglioSomministrazione.codiceAIC)

                        '            If Not IsNothing(dtFarmaco) AndAlso dtFarmaco.Rows.Count > 0 Then

                        '                Dim objUnitaMisura_R As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                        '                Dim Udm_Cod As Integer = 0
                        '                Dim quantitaConvertita As Decimal = 0
                        '                Dim Udm_Selezionata As Integer = 0
                        '                Select Case Somministrazione.unitaDiMisura.codice
                        '                    Case enum_UnitaMisura.Litri
                        '                        quantitaConvertita = objUnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, Somministrazione.quantitaTotaleReale,
                        '                                                                   enum_UnitaMisura.Litri)
                        '                        Udm_Cod = enum_UnitaMisura.Litri
                        '                        Udm_Selezionata = enum_UnitaMisura.Millilitri
                        '                    Case enum_UnitaMisura.Numero
                        '                        Udm_Cod = enum_UnitaMisura.Numero
                        '                        Udm_Selezionata = enum_UnitaMisura.Numero
                        '                        quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '                    Case enum_UnitaMisura.Grammi
                        '                        quantitaConvertita = objUnitaMisura_R.Converti(enum_UnitaMisura.Grammi, Somministrazione.quantitaTotaleReale,
                        '                                                                   enum_UnitaMisura.KG)
                        '                        Udm_Cod = enum_UnitaMisura.KG
                        '                        Udm_Selezionata = enum_UnitaMisura.Grammi
                        '                        quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '                    Case Else
                        '                        Udm_Cod = Somministrazione.unitaDiMisura.codice
                        '                        quantitaConvertita = Somministrazione.quantitaTotaleReale
                        '                End Select

                        '                Dim objGiacenzeProdotto_R As New AgronicaCoreContabDAL.Giacenze_R
                        '                Dim giacenzaFarmaco As Decimal = objGiacenzeProdotto_R.Verifica_Giacenza((Date.Now).Date,
                        '                                                                                         Magazzino.primaryKey.centroAziendalePK.partitaIva,
                        '                                                                                         Magazzino.primaryKey.centroAziendalePK.codice,
                        '                                                                                         0, FARMACI,
                        '                                                                                         dtFarmaco(0)("Farm_Cod"),
                        '                                                                                         0, 0, 0, 0,
                        '                                                                                         Udm_Cod, "",
                        '                                                                                         objParametri_Server,
                        '                                                                                         objParametri_Utenti)

                        '                '-------------------------------------------------------------------------
                        '                'MOVIMENTI ---- SCARICO FARMACO MAGAZZINO
                        '                '-------------------------------------------------------------------------
                        '                Dim Movimenti_ScaricoFarm_Magazzino As New AgronicaCoreEntityFramework_POCO.Movimenti

                        '                Movimenti_ScaricoFarm_Magazzino.PIVA = Agenda.PIVA
                        '                Movimenti_ScaricoFarm_Magazzino.Sa_Cod = 0
                        '                Movimenti_ScaricoFarm_Magazzino.Id_Agenda = Id_Agenda
                        '                Movimenti_ScaricoFarm_Magazzino.Cau_Mov = CAU_SCARICO
                        '                Movimenti_ScaricoFarm_Magazzino.Mov_Desc = Des_Lib
                        '                Movimenti_ScaricoFarm_Magazzino.Mezzo = 0
                        '                Movimenti_ScaricoFarm_Magazzino.Data_Movimento = attivita.inizio
                        '                Movimenti_ScaricoFarm_Magazzino.Scadenza = AGRODATAFINE
                        '                Movimenti_ScaricoFarm_Magazzino.Username_Creazione = Agenda.Username_Creazione
                        '                Movimenti_ScaricoFarm_Magazzino.Data_Creazione = DateTime.Now
                        '                Movimenti_ScaricoFarm_Magazzino.Username_Modifica = Agenda.Username_Modifica
                        '                Movimenti_ScaricoFarm_Magazzino.Data_Modifica = DateTime.Now
                        '                Movimenti_ScaricoFarm_Magazzino.Ora = DateTime.Now
                        '                Movimenti_ScaricoFarm_Magazzino.Data_Registrazione = DateTime.Now
                        '                Movimenti_ScaricoFarm_Magazzino.Validita_Inizio = attivita.inizio
                        '                Movimenti_ScaricoFarm_Magazzino.Validita_Fine = AGRODATAFINE
                        '                Movimenti_ScaricoFarm_Magazzino.Extra_Date = AGRODATAINIZIO
                        '                Movimenti_ScaricoFarm_Magazzino.Scadenza_Extra = AGRODATAFINE
                        '                Movimenti_ScaricoFarm_Magazzino.TipoDocumento = 0
                        '                Movimenti_ScaricoFarm_Magazzino.inviato = 0

                        '                Dim objScrivi_Movimenti_ScaricoFarm_Magazz As New AgronicaCoreContabBIZ.Movimenti_W
                        '                Dim Id_Mov_ScaricoFarm_Magazz As Integer = objScrivi_Movimenti_ScaricoFarm.Scrivi_Modifica(Movimenti_ScaricoFarm_Magazzino,
                        '                                                                                                           GiasContext,
                        '                                                                                                           objParametri_Server)
                        '                GiasContext.SaveChanges()

                        '                '-------------------------------------------------------------------------
                        '                'MOVIMENTI_DETTAGLI ---- SCARICO FARMACO MAGAZZINO
                        '                '-------------------------------------------------------------------------
                        '                Dim Movimenti_Dettagli_ScaricoFarm_Magazz As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.PIVA = Movimenti_ScaricoFarm_Magazzino.PIVA
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Sa_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Id_Agenda = Id_Agenda
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Id_Mov = Movimenti_ScaricoFarm_Magazzino.Id_Mov
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Elem_Cod = FARMACI
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Pro_Cod = dtFarmaco(0)("Farm_Cod")
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Mat_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Mov_Det_Des = dtFarmaco(0)("Denominazione") & " (" & dtFarmaco(0)("Confezione") & " - " & quantitaConvertita & ")"
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Udm_Cod = Udm_Cod
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Qta = quantitaConvertita
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Cod_Iva = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Sconto = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Jolly_Int = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Id_Mov_Esterno = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Cod_Conto = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Cod_Progetto = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Fase_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Contabilizzato = NONCONTABILE
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Pendente = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.inviato = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Username_Creazione = Agenda.Username_Creazione
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Data_Creazione = Date.Now
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Username_Modifica = Agenda.Username_Modifica
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Data_Modifica = Date.Now
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Validita_Inizio = AGRODATAINIZIO
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Validita_Fine = AGRODATAFINE
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Cal_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Extra_Str = Somministrazione.prodotto.codice_alfanumerico
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Extra_Int = DettaglioSomministrazione.quantitaTotaleReale
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Extra_Date = AGRODATAINIZIO
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Anno = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Ric_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Imponibile = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Iva = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Lotto = Lotto
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Prezzo_Unitario = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Imponibile_Netto = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Prezzo_Unitario_Netto = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.UDM_COD_EXTRA = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.QTA_EXTRA = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Prezzo_Effettivo = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.ChkIva_Manuale = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Cod_IvaIndetraibile = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Qta_Extra_Totale = quantitaConvertita
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Tara = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.ChkLayOut_Hide = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Variazione = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Listino_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Sconto_Listino = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Sconto_Modalita = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Mat_Cod_Alias = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Mezzo_Det = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Sconto_Testo = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Ric_Cod_Pat = 0
                        '                'Movimenti_Dettagli_ScaricoFarm_Magazz.TempoCarenza = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.DoseEtichetta = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Turno_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.ID_Attivita = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Dettaglio_VegCod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Iva_Indetraibile = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Iva_Indetraibile_Perc = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.PrincipiAttivi = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.ClassiTossicologiche = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.DoseEtichetta_Value = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Iva_Deto_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Qta_Dettaglio1 = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Qta_Dettaglio2 = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Dettagli_Blocco_Flag = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Dettagli_Blocco_Username = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Dettagli_Blocco_Data = AGRODATAINIZIO
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Ordine_Det = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Deroga_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Prezzo_Livello = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Qualifica_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Tariffa_Cod = 0
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.PrincipiAttiviPesi = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Buffer = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Rif_Esterno = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.Rif_Esterno_2 = ""
                        '                Movimenti_Dettagli_ScaricoFarm_Magazz.PrincipiAttiviPercAbb = ""

                        '                Dim objScrivi_MovimentiDett_ScaricoFarm_Magazz As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                        '                Dim Id_Mov_Det As Integer = objScrivi_MovimentiDett_ScaricoFarm_Magazz.Scrivi_Modifica(Movimenti_Dettagli_ScaricoFarm_Magazz,
                        '                                                                                                       GiasContext, objParametri_Server)
                        '                GiasContext.SaveChanges()

                        '                '-------------------------------------------------------------------------
                        '                'MOV_DESTINAZIONI ---- SCARICO FARMACO ANIMALE
                        '                '-------------------------------------------------------------------------
                        '                Dim Mov_Destinazioni_ScaricoFarm_Magazz As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Piva = Movimenti_Dettagli_ScaricoFarm_Magazz.PIVA
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Sa_Cod = Magazzino.primaryKey.centroAziendalePK.codice
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Id_Agenda = Id_Agenda
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Id_Mov = Id_Mov_ScaricoFarm_Magazz
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Id_Mov_Det = Id_Mov_Det
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Appezza = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Id_Destinazione = Magazzino.primaryKey.codice
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Tipo_Destinazione = Magazzino.tipo
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Qta = quantitaConvertita
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Qta2 = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.inviato = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.username_creazione = Agenda.Username_Creazione
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.data_creazione = Date.Now
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.username_modifica = Agenda.Username_Modifica
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.data_modifica = Date.Now
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.validita_inizio = Movimenti_ScaricoFarm_Magazzino.Data_Movimento
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.validita_fine = AGRODATAFINE
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Tipo_Scorta = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Scorta_Min = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.mov_destinazioni_graphickey = ""
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Qta_Dest1 = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Qta_Dest2 = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.QuotaDistribuzione = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Sa_Cod_Riferimento = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Id_Destinazione_Riferimento = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Tipo_Destinazione_Riferimento = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Sup_Riduzione_BufferZone = 0
                        '                Mov_Destinazioni_ScaricoFarm_Magazz.Perc_Riduzione_Deriva = 0

                        '                Dim objScrivi_MovDestinazioni_ScaricoFarm_Magazz As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                        '                objScrivi_MovDestinazioni_ScaricoFarm_Magazz.Scrivi_Modifica(Mov_Destinazioni_ScaricoFarm_Magazz,
                        '                                                                             GiasContext,
                        '                                                                             objParametri_Server)
                        '                GiasContext.SaveChanges()

                        '            End If

                        '        End If

                        '    End If

                        'Next
#End Region

                    ' X SINCRO OPERAZIONE SPOSTAMENTO CAPI APP
                    Case LAVCOD_SPOSTAMENTI_ZOO

                        If attivita.centriDiCosto Is Nothing OrElse Not attivita.centriDiCosto.Any() Then
                            Throw New GiasException(NessunCapoPresente)
                        End If

                        Dim stalla = (From cdc In attivita.centriDiCosto Where cdc.classType.Equals(ClassType.CapoAnimaleCDC)
                                      Select CType(cdc, CapoAnimaleCDC).sottogruppoStalla_uscita.stallaPK).FirstOrDefault

                        Dim ZooDAL As New AgronicaCoreAnagrafeDAL.Zoo_Animali

                        Dim listaCapi As List(Of String)
                        If verificaGiacenzeSpostamento Then
                            Dim dt = ZooDAL.Leggi_Giacenze(stalla.centroAziendalePK.partitaIva, stalla.centroAziendalePK.codice, stalla.codice, 0, 0, DateTime.Now, objParametri_Server)
                            listaCapi = (From dr In dt.Rows Select CStr(dr("Raggruppamento_Cod") & "_" & dr("Cod_Animale"))).ToList
                        End If

                        Dim listaErrori As New StringBuilder

                        '-------------------------------------------------------------------------
                        'MOVIMENTI----SCARICO
                        '-------------------------------------------------------------------------
                        Dim Movimenti_Scarico As New AgronicaCoreEntityFramework_POCO.Movimenti

                        Movimenti_Scarico.PIVA = Agenda.PIVA
                        Movimenti_Scarico.Sa_Cod = 0
                        Movimenti_Scarico.Id_Agenda = Id_Agenda
                        Movimenti_Scarico.Cau_Mov = CAU_SCARICO_CONSISTENZE
                        Movimenti_Scarico.Mov_Desc = "Scarico Magazzino Zootecnico" 'Des_Lib
                        Movimenti_Scarico.Mezzo = 0
                        Movimenti_Scarico.Extra_Int = stalla.codice
                        Movimenti_Scarico.Modalita = attivita.modalita
                        Movimenti_Scarico.Data_Movimento = attivita.inizio
                        Movimenti_Scarico.Scadenza = attivita.fine
                        Movimenti_Scarico.Username_Creazione = Agenda.Username_Creazione
                        Movimenti_Scarico.Data_Creazione = DateTime.Now
                        Movimenti_Scarico.Username_Modifica = Agenda.Username_Modifica
                        Movimenti_Scarico.Data_Modifica = DateTime.Now
                        Movimenti_Scarico.Ora = AGRODATAINIZIO
                        Movimenti_Scarico.Data_Registrazione = AGRODATAINIZIO
                        Movimenti_Scarico.Validita_Inizio = AGRODATAINIZIO
                        Movimenti_Scarico.Validita_Fine = AGRODATAFINE
                        Movimenti_Scarico.Extra_Date = AGRODATAINIZIO
                        Movimenti_Scarico.Scadenza_Extra = AGRODATAINIZIO
                        'Movimenti_Scarico.TipoDocumento = 0
                        Movimenti_Scarico.inviato = 0

                        Dim objScrivi_Movimenti_Scarico As New AgronicaCoreContabBIZ.Movimenti_W
                        Dim Id_Mov_Scarico As Integer = objScrivi_Movimenti_Scarico.Scrivi_Modifica(Movimenti_Scarico,
                                                                                                    GiasContext,
                                                                                                    objParametri_Server)

                        'GiasContext.SaveChanges()
                        '----------------------------------------

                        '-------------------------------------------------------------------------
                        'MOVIMENTI----CARICO
                        '-------------------------------------------------------------------------
                        Dim Movimenti_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti
                        Movimenti_Carico.PIVA = Agenda.PIVA
                        Movimenti_Carico.Sa_Cod = 0
                        Movimenti_Carico.Id_Agenda = Id_Agenda
                        Movimenti_Carico.Cau_Mov = CAU_CARICO_CONSISTENZE
                        Movimenti_Carico.Mov_Desc = "Carico Magazzino Zootecnico" 'Des_Lib
                        Movimenti_Carico.Mezzo = 0
                        Movimenti_Carico.Extra_Int = stalla.codice
                        Movimenti_Carico.Data_Movimento = attivita.inizio
                        Movimenti_Carico.Scadenza = attivita.fine
                        Movimenti_Carico.Username_Creazione = Agenda.Username_Creazione
                        Movimenti_Carico.Data_Creazione = DateTime.Now
                        Movimenti_Carico.Username_Modifica = Agenda.Username_Modifica
                        Movimenti_Carico.Data_Modifica = DateTime.Now
                        Movimenti_Carico.Ora = AGRODATAINIZIO
                        Movimenti_Carico.Data_Registrazione = AGRODATAINIZIO
                        Movimenti_Carico.Validita_Inizio = AGRODATAINIZIO
                        Movimenti_Carico.Validita_Fine = AGRODATAFINE
                        Movimenti_Carico.Extra_Date = AGRODATAINIZIO
                        Movimenti_Carico.Scadenza_Extra = AGRODATAINIZIO
                        'Movimenti_Carico.TipoDocumento = 0
                        Movimenti_Carico.inviato = 0

                        Dim objScrivi_Movimenti_Carico As New AgronicaCoreContabBIZ.Movimenti_W
                        Dim Id_Mov_Carico As Integer = objScrivi_Movimenti_Carico.Scrivi_Modifica(Movimenti_Carico,
                                                                                                  GiasContext,
                                                                                                  objParametri_Server)

                        'GiasContext.SaveChanges()
                        '----------------------------------------
                        Dim Lista_IdMov_Dett As New List(Of Integer)

                        For Each cdc In attivita.centriDiCosto

                            If cdc.classType.Equals(ClassType.CapoAnimaleCDC) Then

                                '-------------------------------------------------------------------------
                                'CAPO_ANIMALE
                                '-------------------------------------------------------------------------
                                Dim CapoAnimaleCDC As CapoAnimaleCDC = CType(cdc, CapoAnimaleCDC)
                                Dim CapoAnimale As CapoAnimale = CapoAnimaleCDC.capoAnimale
                                Dim Sa_Cod As Integer = CapoAnimaleCDC.codice.intValue
                                Dim Cod_Gruppo As Integer = CapoAnimaleCDC.sottogruppoStalla_uscita.codice 'gruppo di partenza
                                Dim Cod_Progetto As Integer = 0
                                Dim Matricola As String = ""


                                ' se non è indicato il capo animale scarto la riga
                                If CapoAnimale Is Nothing AndAlso CapoAnimaleCDC.capoAnimaleNonPresente Is Nothing Then
                                    Continue For
                                End If
                                ' controllo se l'animale è presente
                                If CapoAnimale Is Nothing Then
                                    Matricola = CapoAnimaleCDC.capoAnimaleNonPresente.descrizione
                                Else
                                    CapoAnimale.validato = True
                                    Cod_Progetto = CapoAnimale.codice
                                    'Dim Cod_Gruppo = CapoAnimaleCDC.sottogruppoStalla_uscita.codice 'gruppo di partenza
                                    Dim Nome_Gruppo = CapoAnimaleCDC.sottogruppoStalla_uscita.nome

                                    ' aggiorna dati anagrafici animale
                                    Dim objScrivi_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
                                    'ottiene il gruppo di partenza in base alla giacenza
                                    If verificaGiacenzeSpostamento Then
                                        Cod_Gruppo = ottieniGruppoPartenza(listaCapi,
                                                                       Cod_Gruppo,
                                                                       CapoAnimale.codice)

                                        If (Cod_Gruppo = CapoAnimaleCDC.sottogruppoStalla_ingresso.codice OrElse Cod_Gruppo = "-1") Then
                                            Continue For 'se gruppo destinazione e partenza coincidono -> skip
                                        End If

                                        If Not objScrivi_Zoo.Aggiorna_CapoAnimale(CapoAnimale, GiasContext, objParametri_Server,
                                                                                  Cod_Gruppo, False) Then
                                            listaErrori.AppendLine(String.Format(CapoAnimaleErrato, CapoAnimale.matricola))
                                        End If



                                        If Not verificaGiacenza(listaCapi,
                                                               Cod_Gruppo,
                                                               CapoAnimale.codice
                                                               ) Then

                                            listaErrori.AppendLine(String.Format(GruppoCapoErrato, CapoAnimale.matricola, If(String.IsNullOrEmpty(Nome_Gruppo), Cod_Gruppo, Nome_Gruppo)))
                                        End If
                                    Else
                                        If Not objScrivi_Zoo.Aggiorna_CapoAnimale(CapoAnimale, GiasContext, objParametri_Server, Cod_Gruppo, False) Then
                                            listaErrori.AppendLine(String.Format(CapoAnimaleErrato, CapoAnimale.matricola))
                                        End If

                                    End If

                                End If

                                '-------------------------------------------------------------------------
                                'MOVIMENTI_DETTAGLI----SCARICO
                                '-------------------------------------------------------------------------
                                Dim Movimenti_Dettagli_Scarico As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                                Movimenti_Dettagli_Scarico.PIVA = Movimenti_Scarico.PIVA
                                Movimenti_Dettagli_Scarico.Sa_Cod = Sa_Cod
                                Movimenti_Dettagli_Scarico.Cod_Progetto = Cod_Progetto
                                Movimenti_Dettagli_Scarico.Elem_Cod = ZOO_CONSISTENZA
                                Movimenti_Dettagli_Scarico.Id_Agenda = Id_Agenda
                                Movimenti_Dettagli_Scarico.Id_Mov = Movimenti_Scarico.Id_Mov
                                Movimenti_Dettagli_Scarico.Mov_Det_Des = Matricola
                                Movimenti_Dettagli_Scarico.Udm_Cod = 38
                                Movimenti_Dettagli_Scarico.Qta = 1
                                Movimenti_Dettagli_Scarico.Cod_Iva = 0
                                Movimenti_Dettagli_Scarico.Contabilizzato = CONTABILE
                                Movimenti_Dettagli_Scarico.Username_Creazione = Agenda.Username_Creazione
                                Movimenti_Dettagli_Scarico.Data_Creazione = Date.Now
                                Movimenti_Dettagli_Scarico.Username_Modifica = Agenda.Username_Modifica
                                Movimenti_Dettagli_Scarico.Data_Modifica = Date.Now
                                Movimenti_Dettagli_Scarico.Validita_Inizio = AGRODATAINIZIO
                                Movimenti_Dettagli_Scarico.Validita_Fine = AGRODATAFINE
                                Movimenti_Dettagli_Scarico.Extra_Date = AGRODATAINIZIO
                                Movimenti_Dettagli_Scarico.inviato = 0

                                Dim objScrivi_MovimentiDett_Scarico As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                                Dim Id_Mov_Det_Scarico As Integer = objScrivi_MovimentiDett_Scarico.Scrivi_Modifica(Movimenti_Dettagli_Scarico,
                                                                                                            GiasContext,
                                                                                                            objParametri_Server)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOV_DESTINAZIONI----SCARICO
                                '-------------------------------------------------------------------------
                                Dim Mov_Destinazioni_Scarico As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                                Mov_Destinazioni_Scarico.Piva = Movimenti_Dettagli_Scarico.PIVA
                                Mov_Destinazioni_Scarico.Sa_Cod = Movimenti_Dettagli_Scarico.Sa_Cod
                                Mov_Destinazioni_Scarico.Id_Agenda = Id_Agenda
                                Mov_Destinazioni_Scarico.Id_Mov = Id_Mov_Scarico
                                Mov_Destinazioni_Scarico.Id_Mov_Det = Id_Mov_Det_Scarico
                                Mov_Destinazioni_Scarico.Appezza = 0
                                Mov_Destinazioni_Scarico.Id_Destinazione = Cod_Gruppo
                                Mov_Destinazioni_Scarico.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                                Mov_Destinazioni_Scarico.Qta = Movimenti_Dettagli_Scarico.Qta
                                Mov_Destinazioni_Scarico.username_creazione = Agenda.Username_Creazione
                                Mov_Destinazioni_Scarico.data_creazione = Date.Now
                                Mov_Destinazioni_Scarico.username_modifica = Agenda.Username_Modifica
                                Mov_Destinazioni_Scarico.data_modifica = Date.Now
                                Mov_Destinazioni_Scarico.validita_inizio = AGRODATAINIZIO
                                Mov_Destinazioni_Scarico.validita_fine = AGRODATAFINE
                                Mov_Destinazioni_Scarico.inviato = 0

                                Dim objScrivi_MovDestinazioni_Scarico As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                                objScrivi_MovDestinazioni_Scarico.Scrivi_Modifica(Mov_Destinazioni_Scarico,
                                                                                  GiasContext,
                                                                                  objParametri_Server)
                                'GiasContext.SaveChanges()

                                Lista_IdMov_Dett.Add(Id_Mov_Det_Scarico)
                                Dim Movimenti_Dettagli_Scarico_Del = From c In GiasContext.Movimenti_dettagli
                                                                     Where c.Id_Agenda = Id_Agenda _
                                                                        AndAlso c.Id_Mov = Id_Mov_Scarico _
                                                                        AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                                                                     Select c

                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOVIMENTI_DETTAGLI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Movimenti_Dettagli_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                                Movimenti_Dettagli_Carico.PIVA = Movimenti_Carico.PIVA
                                Movimenti_Dettagli_Carico.Sa_Cod = Sa_Cod
                                Movimenti_Dettagli_Carico.Cod_Progetto = Cod_Progetto
                                Movimenti_Dettagli_Carico.Elem_Cod = ZOO_CONSISTENZA
                                Movimenti_Dettagli_Carico.Id_Agenda = Id_Agenda
                                Movimenti_Dettagli_Carico.Id_Mov = Movimenti_Carico.Id_Mov
                                Movimenti_Dettagli_Carico.Mov_Det_Des = Matricola
                                Movimenti_Dettagli_Carico.Udm_Cod = 38
                                Movimenti_Dettagli_Carico.Qta = 1
                                Movimenti_Dettagli_Carico.Cod_Iva = 0
                                Movimenti_Dettagli_Carico.Contabilizzato = CONTABILE
                                Movimenti_Dettagli_Carico.Username_Creazione = Agenda.Username_Creazione
                                Movimenti_Dettagli_Carico.Data_Creazione = Date.Now
                                Movimenti_Dettagli_Carico.Username_Modifica = Agenda.Username_Modifica
                                Movimenti_Dettagli_Carico.Data_Modifica = Date.Now
                                Movimenti_Dettagli_Carico.Validita_Inizio = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Validita_Fine = AGRODATAFINE
                                Movimenti_Dettagli_Carico.Extra_Date = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.inviato = 0

                                Dim objScrivi_MovimentiDett_Carico As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                                Dim Id_Mov_Det_Carico As Integer = objScrivi_MovimentiDett_Carico.Scrivi_Modifica(Movimenti_Dettagli_Carico,
                                                                                                           GiasContext,
                                                                                                           objParametri_Server)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOV_DESTINAZIONI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Mov_Destinazioni_Carico As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                                Mov_Destinazioni_Carico.Piva = Movimenti_Dettagli_Carico.PIVA
                                Mov_Destinazioni_Carico.Sa_Cod = Movimenti_Dettagli_Carico.Sa_Cod
                                Mov_Destinazioni_Carico.Id_Agenda = Id_Agenda
                                Mov_Destinazioni_Carico.Id_Mov = Id_Mov_Carico
                                Mov_Destinazioni_Carico.Id_Mov_Det = Id_Mov_Det_Carico
                                Mov_Destinazioni_Carico.Appezza = 0
                                Mov_Destinazioni_Carico.Id_Destinazione = CapoAnimaleCDC.sottogruppoStalla_ingresso.codice
                                Mov_Destinazioni_Carico.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                                Mov_Destinazioni_Carico.Qta = Movimenti_Dettagli_Carico.Qta
                                Mov_Destinazioni_Carico.username_creazione = Agenda.Username_Creazione
                                Mov_Destinazioni_Carico.data_creazione = Date.Now
                                Mov_Destinazioni_Carico.username_modifica = Agenda.Username_Modifica
                                Mov_Destinazioni_Carico.data_modifica = Date.Now
                                Mov_Destinazioni_Carico.validita_inizio = AGRODATAINIZIO
                                Mov_Destinazioni_Carico.validita_fine = AGRODATAFINE
                                Mov_Destinazioni_Carico.inviato = 0

                                Dim objScrivi_MovDestinazioni_Carico As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                                objScrivi_MovDestinazioni_Carico.Scrivi_Modifica(Mov_Destinazioni_Carico,
                                                                                 GiasContext,
                                                                                 objParametri_Server)
                                ' GiasContext.SaveChanges()

                                Lista_IdMov_Dett.Add(Id_Mov_Det_Carico)
                                Dim Movimenti_Dettagli_Carico_Del = From c In GiasContext.Movimenti_dettagli
                                                                    Where c.Id_Agenda = Id_Agenda _
                                                                        AndAlso c.Id_Mov = Id_Mov_Carico _
                                                                        AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                                                                    Select c




                            End If
                        Next

                        If GiasContext.ChangeTracker.Entries(Of AgronicaCoreEntityFramework_POCO.Mov_Destinazioni).Any() OrElse
                                        GiasContext.ChangeTracker.Entries(Of AgronicaCoreEntityFramework_POCO.Movimenti_dettagli).Any() Then
                            GiasContext.SaveChanges()
                        Else
                            confermaTransazione = False
                        End If

                        If listaErrori.Length > 0 Then
                            Throw New GiasException(listaErrori.ToString)
                        End If

                    Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI
                    Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI
                    Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI
                    Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI
                    Case LAVCOD_PESATURA_ANIMALI
                    Case LAVCOD_ALTRE_LAVORAZIONI_ZOO

                End Select

            End If

            GiasContext.SaveChanges()
            GiasContext.Core.AcceptAllChanges()

            If confermaTransazione Then
                scope.Complete()
            End If

            scope.Dispose()

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex

        Catch ex As Exception
            scope.Dispose()

            Dim attivita_Txt = ""
            Dim exMessage = ""
            If attivita IsNot Nothing Then
                attivita_Txt = JsonConvert.SerializeObject(attivita)
            End If
            exMessage = ex.Message & " Attivita:" & ex.Message
            Throw New Exception(exMessage)

        Finally
            GiasContext.Dispose()

        End Try


        Try
            If Agenda IsNot Nothing Then
                Dim scrivi_BIZ As New AgronicaCoreContabBIZ.CDG_BIZ_W
                scrivi_BIZ.AllineaCostiDaCampagna_Bombardino(Agenda.PIVA,
                                                             Agenda.Id_Agenda,
                                                             Date.Now,
                                                             False,
                                                             objParametri_Server,
                                                             objParametri_Utenti)
            End If

        Catch ex As Exception

        End Try

        Return Id_Agenda

    End Function

    Private Function verificaGiacenza(listaCapi As List(Of String), 'RaggruppamentoCod_CodiceAnimale
                                                codGruppoPartenza As String,
                                                codiceAnimale As String
                                             ) As Boolean
        'animale in giacenza
        Return listaCapi.Contains($"{codGruppoPartenza}_{codiceAnimale}")
    End Function

    Private Function ottieniGruppoPartenza(listaCapi As List(Of String), 'RaggruppamentoCod_CodiceAnimale
                                                codGruppoPartenza As String,
                                                codiceAnimale As String
                                             ) As String
        Try
            'animale in giacenza
            Dim codGruppoCorrente = listaCapi.First(Function(c) c.EndsWith(codiceAnimale)).Split("_").First()
            Return codGruppoCorrente
        Catch ex As Exception
            'Se la ricerca del codGruppoCorrente va in errore allora imposto -1 per evitare lo spostamento
            Return "-1"
        End Try

    End Function


    ''' <summary>
    ''' Crea le operazioni di agenda a partire da una ricetta zoo (attivita.tipo = Tipo_Attivita.Ricetta)
    ''' </summary>
    ''' <param name="attivitaRicetta"></param>
    ''' <param name="objP_Server"></param>
    ''' <param name="objP_Utenti"></param>
    ''' <param name="paramsExtraAttivita"></param>
    ''' <param name="_idRicetta"></param>
    ''' <param name="origine"></param>
    ''' <returns></returns>
    Public Function ScriviAgendaFromRicettaZootecnica(ByVal attivitaRicetta As AgronicaCoreModelsSTD.attivita.Attivita,
                                                      ByVal objP_Server As AgronicaCoreParametri,
                                                      Optional ByVal objP_Utenti As AgronicaCoreParametri = Nothing,
                                                      Optional ByVal paramsExtraAttivita As List(Of Parametri_Aggiuntivi_Attivita) = Nothing,
                                                      Optional ByVal _idRicetta As Integer = 0,
                                                      Optional ByVal origine As String = "") As Integer
        Dim ricettaCod As Integer = _idRicetta
        Dim lavCod As Integer = CInt(attivitaRicetta.job.primaryKey.codice)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objP_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
        transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim startTransaction As DateTime
        Dim endTransaction As DateTime

        Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
        startTransaction = DateTime.Now

        Try
            Select Case lavCod

                Case LAVCOD_CUREMEDICAMENTI_ANIMALI
                    '-------------------------------------------------------------------------
                    'RICETTE
                    '-------------------------------------------------------------------------

                    Dim agroDP As New Agro_Sequenze
                    Dim Ricetta As AgronicaCoreEntityFramework_POCO.Ricette
                    Dim ricettaCodice As String = ""

                    If ricettaCod = 0 Then
                        Ricetta = New AgronicaCoreEntityFramework_POCO.Ricette
                        Ricetta.Ricetta_Cod = agroDP.NuovoId_Tabella_EF(GiasContext, "ricette", 0, 200000000, objP_Server)
                        Ricetta.Piva = attivitaRicetta.centroAziendale.primaryKey.partitaIva
                        Ricetta.Sa_Cod = attivitaRicetta.centroAziendale.primaryKey.codice
                        Ricetta.Tipo_Ricetta = enum_TipoRicetta.Zoo
                        Ricetta.Note = ""
                        Ricetta.Veg_Cod = -1
                        Ricetta.Ricetta_Des = ricettaCodice
                        Ricetta.Ricetta_Numero = ricettaCodice
                        Ricetta.Ricetta_Des_Long = ricettaCodice
                        Ricetta.Data_Creazione = DateTime.Now
                        Ricetta.Data_Modifica = DateTime.Now
                        Ricetta.Ricetta_SuperUser = objP_Server.PivaSuperUser
                        Ricetta.Username_Creazione = objP_Server.UsernameOperazione
                        Ricetta.Username_Modifica = objP_Server.UsernameOperazione
                        Ricetta.inviato = 0
                        Ricetta.Validita_Inizio = AGRODATAINIZIO
                        Ricetta.Validita_Fine = AGRODATAFINE
                        Ricetta.DataLock = 0
                        Ricetta.Imputazione_Cod = 0
                        Ricetta.Programmazione_Cod = 0
                        Ricetta.Imputazione_Fase_Cod = 0
                        Ricetta.Origine = ""
                        Ricetta.Blocco_Flag = 0
                        Ricetta.Blocco_Username = ""
                        Ricetta.Blocco_Data = AGRODATAINIZIO
                        GiasContext.Ricette.Add(Ricetta)
                    Else
                        Ricetta = GiasContext.Ricette.Where(Function(ric) ric.Ricetta_Cod = ricettaCod).First
                        Ricetta.Data_Modifica = DateTime.Now
                        Ricetta.Username_Modifica = objP_Server.UsernameOperazione
                        GiasContext.Entry(Ricetta).State = Entity.EntityState.Modified
                    End If

                    ricettaCod = Ricetta.Ricetta_Cod
                    GiasContext.SaveChanges()

                    'Essendo una ricetta, le attivita' legate alle operazioni di agenda sono in attivitaCollegate
                    If Not IsNothing(attivitaRicetta.attivitaCollegate) AndAlso attivitaRicetta.attivitaCollegate.Count > 0 Then

                        'Scrive un'operazione di trattamento zoo partendo dalle attivita collegate alla ricetta zoo
                        For Each attivitaRigaRicetta In attivitaRicetta.attivitaCollegate

                            Dim esiste = Not (IsNothing(attivitaRigaRicetta.codice) OrElse attivitaRigaRicetta.codice = "" OrElse attivitaRigaRicetta.codice = "0")

                            Dim lavCodCollegata As Integer = CInt(attivitaRigaRicetta.job.primaryKey.codice)

                            Dim lavorazione_R As New AgronicaCoreAnagrafeBIZ.Lavorazione_R
                            Dim desLib As String = lavorazione_R.GetLavDes(lavCodCollegata, objP_Server)
                            Dim piva As String = attivitaRigaRicetta.centroAziendale.primaryKey.partitaIva
                            Dim saCod As Integer = If(IsNothing(attivitaRigaRicetta.centroAziendale.primaryKey.codice), 0, attivitaRigaRicetta.centroAziendale.primaryKey.codice)

                            '-------------------------------------------------------------------------
                            'AGENDA
                            '-------------------------------------------------------------------------
                            Dim Agenda As New AgronicaCoreEntityFramework_POCO.Agenda
                            Agenda.PIVA = piva
                            Agenda.Sa_Cod = saCod
                            Agenda.Sta_Num = If(IsNothing(attivitaRigaRicetta.fabbricatoCod), 0, attivitaRigaRicetta.fabbricatoCod)
                            Agenda.Id_Agenda = If(esiste, CInt(attivitaRigaRicetta.codice), 0)
                            Agenda.Lav_Cod = lavCodCollegata
                            Agenda.des_lib = desLib
                            Agenda.Blocco_Data = AGRODATAINIZIO
                            Agenda.Validita_Inizio = attivitaRigaRicetta.inizio
                            Agenda.Validita_Fine = attivitaRigaRicetta.fine
                            Agenda.Origine = origine
                            Agenda.inviato = 0

                            Dim agenda_W As New AgronicaCoreContabBIZ.Agenda_W
                            Dim idAgenda = agenda_W.Scrivi_Modifica(Agenda, GiasContext, objP_Server)
                            GiasContext.SaveChanges()

                            '-------------------------------------------------------------------------
                            'RICETTEXAGENDA
                            '-------------------------------------------------------------------------
                            Dim RicettaxAgenda As New AgronicaCoreEntityFramework_POCO.RicettexAgenda

                            If esiste Then
                                RicettaxAgenda = GiasContext.RicettexAgenda.Where(Function(rcxa) rcxa.Ricetta_Cod = ricettaCod AndAlso rcxa.Id_Agenda = idAgenda).FirstOrDefault
                                RicettaxAgenda.Data_Modifica = DateTime.Now
                                RicettaxAgenda.Username_Modifica = objP_Server.UsernameOperazione
                                GiasContext.Entry(RicettaxAgenda).State = Entity.EntityState.Modified
                            Else
                                RicettaxAgenda = New AgronicaCoreEntityFramework_POCO.RicettexAgenda
                                RicettaxAgenda.Ricetta_Cod = ricettaCod
                                RicettaxAgenda.Ricetta_Operazione_Cod = 0
                                RicettaxAgenda.Id_Agenda = idAgenda
                                RicettaxAgenda.Ricetta_SuperUser = objP_Server.PivaSuperUser
                                RicettaxAgenda.Data_Creazione = DateTime.Now
                                RicettaxAgenda.Data_Modifica = DateTime.Now
                                RicettaxAgenda.Validita_Inizio = attivitaRigaRicetta.inizio
                                RicettaxAgenda.Validita_Fine = attivitaRigaRicetta.fine
                                RicettaxAgenda.inviato = 0
                                RicettaxAgenda.DataLock = 0

                                GiasContext.RicettexAgenda.Add(RicettaxAgenda)
                            End If
                            GiasContext.SaveChanges()

                            '-------------------------------------------------------------------------
                            'RICETTE_ZOOXAGENDA
                            '-------------------------------------------------------------------------
                            Dim Ricetta_ZooxAgenda As New AgronicaCoreEntityFramework_POCO.Ricette_ZooxAgenda
                            Ricetta_ZooxAgenda.Id_Ricetta = attivitaRicetta.codiceOperazioneRicetta
                            Ricetta_ZooxAgenda.Id_RigaRicetta = attivitaRigaRicetta.codiceOperazioneRicetta
                            Ricetta_ZooxAgenda.Id_Agenda = idAgenda
                            Ricetta_ZooxAgenda.SuperUser_Ricetta = objP_Server.PivaSuperUser

                            Dim ricetteZooxAgenda_W As New AgronicaCoreContabBIZ.Ricette_ZooxAgenda_W
                            ricetteZooxAgenda_W.Scrivi_Modifica(Ricetta_ZooxAgenda, GiasContext, objP_Server)
                            GiasContext.SaveChanges()

                            '-------------------------------------------------------------------------
                            'AGENDA LOG
                            '-------------------------------------------------------------------------
                            Dim logAgenda As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Agenda

                            If Not esiste Then
                                logAgenda.ID = idAgenda
                                logAgenda.Sa_Cod = Agenda.Sa_Cod
                                logAgenda.SuperUser = objP_Server.PivaSuperUser
                                logAgenda.Utente = Agenda.PIVA
                                logAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                                logAgenda.Data_Ora_Lavorazione = Agenda.Validita_Inizio
                                logAgenda.Data_Ora_RegistrazioneLog = DateTime.Now
                                logAgenda.Lav_Cod = lavCodCollegata
                                logAgenda.Des_lib = desLib
                                logAgenda.Id_Servizio = enum_Id_Servizio.GiasOnline

                                Dim jsonAgenda = JsonConvert.SerializeObject(Agenda, New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local})
                                logAgenda.object_data = jsonAgenda.ToString

                                GiasContext.Entry(logAgenda).State = Entity.EntityState.Added
                                GiasContext.SaveChanges()
                            End If

                            ScriviTrattamentoFromAttivitaZoo(attivitaRigaRicetta, Agenda, objP_Server, objP_Utenti, GiasContext, esiste)

                        Next
                    End If

            End Select

            GiasContext.Core.AcceptAllChanges()
            scope.Complete()
            scope.Dispose()

        Catch ex As GiasException
            If scope IsNot Nothing Then scope.Dispose()
            Throw ex
        Catch ex As Exception
            scope.Dispose()
            Dim attivita_Txt As String = ""
            Dim exMessage As String = ""
            If attivitaRicetta IsNot Nothing Then attivita_Txt = JsonConvert.SerializeObject(attivitaRicetta)
            exMessage = ex.Message & " Attivita:" & ex.Message
            Throw New Exception(exMessage)
        Finally
            GiasContext.Dispose()
            endTransaction = DateTime.Now
        End Try

        Return ricettaCod

    End Function

    'Per aggiornare i dati relativi a un modello 4 gia' importato
    Public Function AggiornaAttivitaZootecnica(ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                 ByVal datiModello As DataRow,
                                                 ByVal objParametri_Server As AgronicaCoreParametri,
                                                 Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing,
                                                 Optional ByVal listaParamsAggiuntiviAttivita As List(Of Parametri_Aggiuntivi_Attivita) = Nothing,
                                                 Optional ByVal origine As String = "") As Integer
        Dim Id_Agenda As Integer
        Dim Lav_Cod As Integer = CInt(attivita.job.primaryKey.codice)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = Transactions.IsolationLevel.ReadUncommitted
        transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim StartTransaction As DateTime
        Dim EndTransaction As DateTime

        Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
        StartTransaction = DateTime.Now

        Try
            If Not IsNothing(attivita.centriDiCosto) Then
                Dim objModello4 As New JObject
                If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso listaParamsAggiuntiviAttivita.Count > 0 Then
                    For Each p In listaParamsAggiuntiviAttivita
                        If p.key = Key_Parametri_Aggiuntivi_Attivita.sincro_modello4 Then
                            objModello4 = JsonConvert.DeserializeObject(Of JObject)(p.value)
                            Exit For
                        End If
                    Next
                End If
                Dim idAgendaDB = CInt(datiModello("Id_Agenda"))
                Dim Id_Mov_RegistrazioneDB = CInt(datiModello("Id_Mov"))
                Dim Id_Reg_DettaglioDB = CInt(datiModello("Id_Reg_Dettaglio"))
                Dim data = DateTime.Now

                'Ricavo il Des_Lib tramite Lav_Cod
                Dim objLavorazione = New AgronicaCoreAnagrafeBIZ.Lavorazione_R
                Dim Des_Lib As String = objLavorazione.GetLavDes(Lav_Cod, objParametri_Server)

                Dim Agenda = GiasContext.Agenda.Where(Function(row) row.Id_Agenda = idAgendaDB).Single
                Agenda.Data_Modifica = data

                '-------------------------------------------------------------------------
                'AGENDA
                '-------------------------------------------------------------------------
                'Dim Agenda As New AgronicaCoreEntityFramework_POCO.Agenda
                'Agenda.PIVA = attivita.centroAziendale.primaryKey.partitaIva
                'Agenda.Sa_Cod = 0
                'Agenda.Id_Agenda = idAgenda
                'Agenda.Lav_Cod = Lav_Cod
                'Agenda.des_lib = Des_Lib
                'Agenda.Blocco_Data = AGRODATAINIZIO
                'Agenda.Data_Creazione = DateTime.Now
                'Agenda.Data_Modifica = DateTime.Now
                'Agenda.Validita_Inizio = attivita.inizio
                'Agenda.Validita_Fine = attivita.fine
                'Agenda.Origine = origine
                'Agenda.inviato = 0

                'If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso listaParamsAggiuntiviAttivita.Count > 0 Then
                '    For Each p In listaParamsAggiuntiviAttivita
                '        If p.key = Key_Parametri_Aggiuntivi_Attivita.sincro_modello4 Then
                '            objModello4 = JsonConvert.DeserializeObject(Of JObject)(p.value)
                '            Agenda.Blocco_Flag = 1
                '            Agenda.Tipo_Accettazione = 1
                '            Exit For
                '        End If
                '    Next
                'End If

                Dim objScrivi_Agenda As New AgronicaCoreContabBIZ.Agenda_W
                Id_Agenda = objScrivi_Agenda.Scrivi_Modifica(Agenda,
                                                             GiasContext,
                                                             objParametri_Server)
                GiasContext.SaveChanges()

                '-------------------------------------------------------------------------
                'AGENDA LOG
                '-------------------------------------------------------------------------
                Dim logAgenda As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Agenda
                logAgenda.ID = Id_Agenda
                logAgenda.Sa_Cod = Agenda.Sa_Cod
                logAgenda.SuperUser = objParametri_Server.PivaSuperUser
                logAgenda.Utente = Agenda.PIVA
                logAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                logAgenda.Data_Ora_Lavorazione = Agenda.Validita_Inizio
                logAgenda.Data_Ora_RegistrazioneLog = DateTime.Now
                logAgenda.Lav_Cod = Lav_Cod
                logAgenda.Des_lib = Des_Lib
                logAgenda.Id_Servizio = enum_Id_Servizio.GiasOnline

                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim jsonAgenda = JsonConvert.SerializeObject(Agenda, a)
                logAgenda.object_data = jsonAgenda.ToString

                GiasContext.Entry(logAgenda).State = Entity.EntityState.Added
                GiasContext.SaveChanges()

                Select Case Lav_Cod
                    '----------------------------ZOO_CARICO----------------------------
                    Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO,
                     LAVCOD_NASCITA_ANIMALI,
                     LAVCOD_ACQUISTO_ANIMALI

                        '-------------------------------------------------------------------------
                        'MOVIMENTI----CARICO
                        '-------------------------------------------------------------------------
                        'Dim Movimenti_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti
                        'Movimenti_Carico.PIVA = Agenda.PIVA
                        'Movimenti_Carico.Sa_Cod = 0
                        'Movimenti_Carico.Id_Agenda = Id_Agenda
                        'Movimenti_Carico.Cau_Mov = CAU_CARICO_CONSISTENZE
                        'Movimenti_Carico.Mov_Desc = Des_Lib
                        'Movimenti_Carico.Mezzo = 1
                        'Movimenti_Carico.Data_Movimento = attivita.inizio
                        'Movimenti_Carico.Scadenza = AGRODATAFINE
                        'Movimenti_Carico.Username_Creazione = Agenda.Username_Creazione
                        'Movimenti_Carico.Data_Creazione = DateTime.Now
                        'Movimenti_Carico.Username_Modifica = Agenda.Username_Modifica
                        'Movimenti_Carico.Data_Modifica = DateTime.Now
                        'Movimenti_Carico.Ora = DateTime.Now
                        'Movimenti_Carico.Data_Registrazione = DateTime.Now
                        'Movimenti_Carico.Validita_Inizio = AGRODATAINIZIO
                        'Movimenti_Carico.Validita_Fine = AGRODATAFINE
                        'Movimenti_Carico.Extra_Date = AGRODATAINIZIO
                        'Movimenti_Carico.Scadenza_Extra = AGRODATAFINE
                        'Movimenti_Carico.TipoDocumento = 0
                        'Movimenti_Carico.inviato = 0

                        Dim Movimenti_Carico = GiasContext.Movimenti.Where(Function(row) row.Id_Agenda = idAgendaDB AndAlso row.Cau_Mov = CAU_CARICO_CONSISTENZE).First
                        Movimenti_Carico.Data_Modifica = data

                        Dim objScrivi_Movimenti_Carico As New AgronicaCoreContabBIZ.Movimenti_W
                        Dim Id_Mov_Carico As Integer = objScrivi_Movimenti_Carico.Scrivi_Modifica(Movimenti_Carico,
                                                                                          GiasContext,
                                                                                          objParametri_Server)
                        GiasContext.SaveChanges()

                        If Not IsNothing(listaParamsAggiuntiviAttivita) AndAlso Not IsNothing(objModello4) AndAlso objModello4.Item("numModello") <> "" Then
                            'Dim numModello As String = objModello4.Item("numModello")
                            'Dim prenotazioneId As Integer = CInt(objModello4.Item("prenotazioneId"))

                            'Dim numAutorizzazione As String = objModello4.Item("numAutorizzazione")
                            'Dim codAzienda_Dest As String = objModello4.Item("codAzienda_Dest")
                            'Dim idFiscaleAzienda_Dest As String = objModello4.Item("idFiscaleAzienda_Dest")
                            'Dim targa As String = objModello4.Item("targa")
                            'Dim flagMezzoProprio As String = objModello4.Item("flagMezzoProprio")
                            'Dim durataViaggio As String = objModello4.Item("durataViaggio")
                            'Dim codAsl_Trasp As String = objModello4.Item("codAsl_Trasp")

                            'Dim Movimenti_Registrazione As New AgronicaCoreEntityFramework_POCO.Movimenti
                            'Movimenti_Registrazione.PIVA = Agenda.PIVA
                            'Movimenti_Registrazione.Sa_Cod = 0
                            'Movimenti_Registrazione.Id_Agenda = Id_Agenda
                            'Movimenti_Registrazione.Cau_Mov = CAU_REGISTRAZIONI
                            'Movimenti_Registrazione.Mov_Desc = Des_Lib
                            'Movimenti_Registrazione.Causale_Trasporto = Des_Lib
                            'Movimenti_Registrazione.Mezzo = 1
                            'Movimenti_Registrazione.Data_Movimento = attivita.inizio
                            'Movimenti_Registrazione.Scadenza = attivita.inizio
                            'Movimenti_Registrazione.Username_Creazione = Agenda.Username_Creazione
                            'Movimenti_Registrazione.Data_Creazione = DateTime.Now
                            'Movimenti_Registrazione.Username_Modifica = Agenda.Username_Modifica
                            'Movimenti_Registrazione.Data_Modifica = DateTime.Now
                            'Movimenti_Registrazione.Ora = DateTime.Now
                            'Movimenti_Registrazione.Data_Registrazione = attivita.inizio
                            'Movimenti_Registrazione.Validita_Inizio = attivita.inizio
                            'Movimenti_Registrazione.Validita_Fine = AGRODATAFINE
                            'Movimenti_Registrazione.Extra_Str = numModello
                            'Movimenti_Registrazione.Extra_Date = attivita.inizio
                            'Movimenti_Registrazione.Ora = attivita.inizio
                            'Movimenti_Registrazione.Scadenza_Extra = AGRODATAFINE
                            'Movimenti_Registrazione.TipoDocumento = 0
                            'Movimenti_Registrazione.inviato = 0


                            'Dim Id_Mov_Registrazione As Integer = objScrivi_Movimenti_Carico.Scrivi_Modifica(Movimenti_Registrazione,
                            '                                                                         GiasContext,
                            '                                                                         objParametri_Server)

                            Dim Movimenti_Registrazione = GiasContext.Movimenti.Where(Function(row) row.Id_Agenda = idAgendaDB AndAlso row.Cau_Mov = CAU_REGISTRAZIONI AndAlso row.Id_Mov = Id_Mov_RegistrazioneDB).First
                            Movimenti_Registrazione.Data_Modifica = data
                            GiasContext.SaveChanges()

                            'Dim Mov_Dettaglio_TecnExtra As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra
                            'Dim idRegDTE As Integer = (From m In GiasContext.Mov_Dettaglio_Tecnico_Extra
                            '                           Order By m.Id_Reg_Dettaglio Descending
                            '                           Select m.Id_Reg_Dettaglio).FirstOrDefault
                            'Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

                            'Dim idRegDTE As Integer = agroDP.NuovoId_Tabella_EF(GiasContext, "mov_dettaglio_tecnico_extra",
                            '                                                0, 200000000, objParametri_Server)
                            'Mov_Dettaglio_TecnExtra.Id_Agenda = Agenda.Id_Agenda
                            'Mov_Dettaglio_TecnExtra.Sa_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Piva = Movimenti_Registrazione.PIVA
                            'Mov_Dettaglio_TecnExtra.Id_Mov = Id_Mov_Registrazione
                            'Mov_Dettaglio_TecnExtra.Id_Mov_Det = 0
                            'Mov_Dettaglio_TecnExtra.Id_Reg_Dettaglio = idRegDTE
                            'Mov_Dettaglio_TecnExtra.validita_inizio = attivita.inizio
                            'Mov_Dettaglio_TecnExtra.validita_fine = AGRODATAFINE
                            'Mov_Dettaglio_TecnExtra.Num_Riferimento = prenotazioneId
                            'Mov_Dettaglio_TecnExtra.data_creazione = data
                            'Mov_Dettaglio_TecnExtra.username_creazione = Movimenti_Registrazione.PIVA
                            'Mov_Dettaglio_TecnExtra.data_modifica = data
                            'Mov_Dettaglio_TecnExtra.username_modifica = Movimenti_Registrazione.PIVA
                            'Mov_Dettaglio_TecnExtra.Regione = ""
                            'Mov_Dettaglio_TecnExtra.Trasportatore = ""
                            'Mov_Dettaglio_TecnExtra.Mezzo_Trasporto = ""
                            'Mov_Dettaglio_TecnExtra.Targa = targa
                            'Mov_Dettaglio_TecnExtra.N_Autorizzazione_Trasporto = numAutorizzazione
                            'Mov_Dettaglio_TecnExtra.Annotazioni = flagMezzoProprio
                            'Mov_Dettaglio_TecnExtra.Durata_Viaggio = durataViaggio
                            'Mov_Dettaglio_TecnExtra.ASL = ""
                            'Mov_Dettaglio_TecnExtra.Serie = ""
                            'Mov_Dettaglio_TecnExtra.Numero = ""
                            'Mov_Dettaglio_TecnExtra.N_Immatricolazione = ""
                            'Mov_Dettaglio_TecnExtra.N_Immatricolazione_Rimorchio = ""
                            'Mov_Dettaglio_TecnExtra.Data_Rilascio_Autorizzazione = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.Peso = 0
                            'Mov_Dettaglio_TecnExtra.inviato = 0
                            'Mov_Dettaglio_TecnExtra.Codice_Prodotto = 0
                            'Mov_Dettaglio_TecnExtra.Colore = 0
                            'Mov_Dettaglio_TecnExtra.Zona_Viticola = ""
                            'Mov_Dettaglio_TecnExtra.Manipolazioni = 0
                            'Mov_Dettaglio_TecnExtra.Precisazioni = ""
                            'Mov_Dettaglio_TecnExtra.Num_Contenitori = 0
                            'Mov_Dettaglio_TecnExtra.Marche_Contenitori = ""
                            'Mov_Dettaglio_TecnExtra.Des_Contenitori = ""
                            'Mov_Dettaglio_TecnExtra.Tipo_Documento = ""
                            'Mov_Dettaglio_TecnExtra.Id_Cod_Autorita = 0
                            'Mov_Dettaglio_TecnExtra.Luogo_Partenza = ""
                            'Mov_Dettaglio_TecnExtra.Luogo_Consegna = ""
                            'Mov_Dettaglio_TecnExtra.Data_Spedizione = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.Indicazioni_Complementari = ""
                            'Mov_Dettaglio_TecnExtra.Titolo_Alcol = 0
                            'Mov_Dettaglio_TecnExtra.Codice_NC = ""
                            'Mov_Dettaglio_TecnExtra.Data_Dichiarazione = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.Garanzia = ""
                            'Mov_Dettaglio_TecnExtra.Certificati = ""
                            'Mov_Dettaglio_TecnExtra.Peso_Lordo = 0
                            'Mov_Dettaglio_TecnExtra.Num_Colli = 0
                            'Mov_Dettaglio_TecnExtra.Contenitore_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Imballaggio_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Agente_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Provvigione = 0
                            'Mov_Dettaglio_TecnExtra.Tipo_Trasporto = 0
                            'Mov_Dettaglio_TecnExtra.Unita_Trasporto = 0
                            'Mov_Dettaglio_TecnExtra.Codice_Alternativo = ""
                            'Mov_Dettaglio_TecnExtra.Id_Gestione_Vettore = 0
                            'Mov_Dettaglio_TecnExtra.Ritenuta_Acconto_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Ritenuta_Acconto = 0
                            'Mov_Dettaglio_TecnExtra.Enasarco_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Enasarco = 0
                            'Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinatario = 0
                            'Mov_Dettaglio_TecnExtra.ACCDAA_Cod_Risum_Destinazione = 0
                            'Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
                            'Mov_Dettaglio_TecnExtra.ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
                            'Mov_Dettaglio_TecnExtra.CapoArea_Cod = 0
                            'Mov_Dettaglio_TecnExtra.Provvigione_CapoArea = 0
                            'Mov_Dettaglio_TecnExtra.Provvigione_Pagata_Agente = 0
                            'Mov_Dettaglio_TecnExtra.Provvigione_Pagata_CapoArea = 0
                            'Mov_Dettaglio_TecnExtra.N_Doc_Cliente = ""
                            'Mov_Dettaglio_TecnExtra.Data_Doc_Cliente = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.N_Nota_Fattura = ""
                            'Mov_Dettaglio_TecnExtra.Data_Nota_Fattura = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.N_Nota_DDT = ""
                            'Mov_Dettaglio_TecnExtra.N_Nota_Riga_DDT = ""
                            'Mov_Dettaglio_TecnExtra.Data_Nota_DDT = AGRODATAINIZIO
                            'Mov_Dettaglio_TecnExtra.Causale_Fattura = 0
                            'Mov_Dettaglio_TecnExtra.N_Doc_Ente = ""
                            'Mov_Dettaglio_TecnExtra.Anno_Doc_Ente = 2100
                            'Mov_Dettaglio_TecnExtra.Num_Conf_Riscontrate = -1
                            'Mov_Dettaglio_TecnExtra.Num_Colli_Riscontrati = -1
                            'Mov_Dettaglio_TecnExtra.Num_Imballi_Riscontrati = -1
                            'Mov_Dettaglio_TecnExtra.Peso_Netto_Riscontrato = 0
                            'Mov_Dettaglio_TecnExtra.Peso_Lordo_Riscontrato = 0
                            'Mov_Dettaglio_TecnExtra.Tara_Unit_Conf_Riscontrata = -1
                            'Mov_Dettaglio_TecnExtra.Tara_Unit_Collo_Riscontrata = -1
                            'Mov_Dettaglio_TecnExtra.Tara_Unit_Imballo_Riscontrata = -1

                            'GiasContext.Mov_Dettaglio_Tecnico_Extra.Add(Mov_Dettaglio_TecnExtra)
                            Dim Mov_Dettaglio_TecnExtra = GiasContext.Mov_Dettaglio_Tecnico_Extra.Where(Function(row) row.Id_Agenda = idAgendaDB AndAlso row.Id_Mov = Id_Mov_RegistrazioneDB).First
                            Mov_Dettaglio_TecnExtra.data_modifica = data

                            GiasContext.SaveChanges()
                        End If

                        Dim Lista_IdMov_Dett As New List(Of Integer)
                        'Dim listResult = New List(Of String)
                        'Dim i = 0

                        For Each cdc In attivita.centriDiCosto
                            If cdc.classType.Equals(costanti.ClassType.CapoAnimaleCDC) Then
                                'Dim start = DateTime.Now

                                Dim CapoAnimaleCDC As centri_di_costo.CapoAnimaleCDC = CType(cdc, centri_di_costo.CapoAnimaleCDC)
                                Dim CapoAnimale As CapoAnimale = CapoAnimaleCDC.capoAnimale

                                Dim Sa_Cod As Integer = CapoAnimaleCDC.codice.intValue

                                '-------------------------------------------------------------------------
                                'CAPO_ANIMALE
                                '-------------------------------------------------------------------------
                                CapoAnimale.validita.inizio = Agenda.Validita_Inizio
                                CapoAnimale.validita.fine = AGRODATAFINE
                                CapoAnimale.flagCancellazione = False

                                Dim objScrivi_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
                                Dim Cod_Progetto As Integer = objScrivi_Zoo.Converti_Animale_DT(CapoAnimale,
                                                                                        objParametri_Server,
                                                                                        GiasContext,
                                                                                        False, "AggiornaAttivitaZootecnica ID_Agenda=" & Agenda.Lav_Cod)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOVIMENTI_DETTAGLI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Movimenti_Dettagli_Carico As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli

                                Movimenti_Dettagli_Carico.PIVA = Movimenti_Carico.PIVA
                                Movimenti_Dettagli_Carico.Sa_Cod = Sa_Cod
                                Movimenti_Dettagli_Carico.Cod_Progetto = Cod_Progetto
                                Movimenti_Dettagli_Carico.Elem_Cod = ZOO_CONSISTENZA
                                Movimenti_Dettagli_Carico.Id_Agenda = Id_Agenda
                                Movimenti_Dettagli_Carico.Id_Mov = Movimenti_Carico.Id_Mov
                                Movimenti_Dettagli_Carico.Mov_Det_Des = CapoAnimale.specie.descrizione & "- [" & CapoAnimale.razza.descrizione & "]"
                                Movimenti_Dettagli_Carico.Udm_Cod = 38
                                Movimenti_Dettagli_Carico.Qta = 1
                                Movimenti_Dettagli_Carico.Cod_Iva = 98
                                Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooConsistenzeIniziali
                                Select Case Lav_Cod
                                    Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooConsistenzeIniziali
                                    Case LAVCOD_NASCITA_ANIMALI
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooNascita
                                    Case LAVCOD_ACQUISTO_ANIMALI
                                        Movimenti_Dettagli_Carico.Pendente = enum_Pendenza.ZooAcquistoAnimali
                                End Select

                                Movimenti_Dettagli_Carico.Contabilizzato = CONTABILE
                                Movimenti_Dettagli_Carico.Username_Creazione = Agenda.Username_Creazione
                                Movimenti_Dettagli_Carico.Data_Creazione = data
                                Movimenti_Dettagli_Carico.Username_Modifica = Agenda.Username_Modifica
                                Movimenti_Dettagli_Carico.Data_Modifica = data
                                Movimenti_Dettagli_Carico.Anno = data.Year
                                Movimenti_Dettagli_Carico.Validita_Inizio = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Validita_Fine = AGRODATAFINE
                                Movimenti_Dettagli_Carico.Extra_Date = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Ric_Cod = 2
                                Movimenti_Dettagli_Carico.Ric_Cod_Pat = 2
                                Movimenti_Dettagli_Carico.Cod_Conto_Pat = 75
                                Movimenti_Dettagli_Carico.Dettagli_Blocco_Data = AGRODATAINIZIO
                                Movimenti_Dettagli_Carico.Prezzo_Livello = -1
                                Movimenti_Dettagli_Carico.Mat_Cod_Alias = 0
                                Movimenti_Dettagli_Carico.Dettagli_Blocco_Username = 0
                                Movimenti_Dettagli_Carico.PrincipiAttiviPercAbb = ""
                                Movimenti_Dettagli_Carico.inviato = 0
                                Movimenti_Dettagli_Carico.Jolly_Int = 0
                                Movimenti_Dettagli_Carico.Id_Mov_Esterno = CapoAnimaleCDC.id_movimentazione_BDN

                                Dim objScrivi_MovimentiDett_Carico As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                                Dim Id_Mov_Det As Integer = objScrivi_MovimentiDett_Carico.Scrivi_Modifica(Movimenti_Dettagli_Carico,
                                                                                                   GiasContext,
                                                                                                   objParametri_Server)
                                'GiasContext.SaveChanges()

                                '-------------------------------------------------------------------------
                                'MOV_DESTINAZIONI----CARICO
                                '-------------------------------------------------------------------------
                                Dim Mov_Destinazioni_Carico As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                                Mov_Destinazioni_Carico.Piva = Movimenti_Dettagli_Carico.PIVA
                                Mov_Destinazioni_Carico.Sa_Cod = Movimenti_Dettagli_Carico.Sa_Cod
                                Mov_Destinazioni_Carico.Id_Agenda = Id_Agenda
                                Mov_Destinazioni_Carico.Id_Mov = Id_Mov_Carico
                                Mov_Destinazioni_Carico.Id_Mov_Det = Id_Mov_Det
                                Mov_Destinazioni_Carico.Appezza = 0
                                Mov_Destinazioni_Carico.Id_Destinazione = CapoAnimaleCDC.sottogruppoStalla_ingresso.codice
                                Mov_Destinazioni_Carico.Tipo_Destinazione = TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA
                                Mov_Destinazioni_Carico.Qta = Movimenti_Dettagli_Carico.Qta
                                Mov_Destinazioni_Carico.username_creazione = Agenda.Username_Creazione
                                Mov_Destinazioni_Carico.data_creazione = data
                                Mov_Destinazioni_Carico.username_modifica = Agenda.Username_Modifica
                                Mov_Destinazioni_Carico.data_modifica = data
                                Mov_Destinazioni_Carico.validita_inizio = AGRODATAINIZIO
                                Mov_Destinazioni_Carico.validita_fine = AGRODATAFINE
                                Mov_Destinazioni_Carico.Sa_Cod_Riferimento = 0
                                Mov_Destinazioni_Carico.Id_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Carico.Tipo_Destinazione_Riferimento = 0
                                Mov_Destinazioni_Carico.Sup_Riduzione_BufferZone = 0
                                Mov_Destinazioni_Carico.Perc_Riduzione_Deriva = 0
                                Mov_Destinazioni_Carico.inviato = 0

                                Dim objScrivi_MovDestinazioni_Carico As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
                                objScrivi_MovDestinazioni_Carico.Scrivi_Modifica(Mov_Destinazioni_Carico,
                                                                         GiasContext,
                                                                         objParametri_Server)
                                'GiasContext.SaveChanges()

                                Lista_IdMov_Dett.Add(Id_Mov_Det)
                                Dim Movimenti_Dettagli_Carico_Del = From c In GiasContext.Movimenti_dettagli
                                                                    Where c.Id_Agenda = Id_Agenda _
                                                                AndAlso c.Id_Mov = Id_Mov_Carico _
                                                                AndAlso Not Lista_IdMov_Dett.Contains(c.Id_Mov_Det)
                                                                    Select c
                                GiasContext.SaveChanges()

                                'Dim endIteration = DateTime.Now
                                'listResult.Add((start - endIteration).TotalMilliseconds)

                                'i = i + 1
                            End If
                        Next


                End Select
            End If

            GiasContext.Core.AcceptAllChanges()
            scope.Complete()
            scope.Dispose()

        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex

        Catch ex As Exception
            scope.Dispose()

            Dim attivita_Txt = ""
            Dim exMessage = ""
            If attivita IsNot Nothing Then
                attivita_Txt = JsonConvert.SerializeObject(attivita)
            End If
            exMessage = ex.Message & " Attivita:" & ex.Message
            Throw New Exception(exMessage)

        Finally
            GiasContext.Dispose()

        End Try

        Return Id_Agenda



    End Function

    ''' <summary>
    ''' Scrive le righe necessarie di Movimenti per l'operazione di Trattamento Zoo
    ''' </summary>
    ''' <param name="attivita"></param>
    ''' <param name="agenda"></param>
    ''' <param name="objP_Server"></param>
    ''' <param name="objP_Utenti"></param>
    ''' <param name="GiasContext"></param>
    Public Sub ScriviTrattamentoFromAttivitaZoo(ByVal attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                ByRef agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                                                ByRef objP_Server As AgronicaCoreParametri,
                                                ByRef objP_Utenti As AgronicaCoreParametri,
                                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                Optional ByVal esiste As Boolean = False)
        Dim movimenti_W As New AgronicaCoreContabBIZ.Movimenti_W
        Dim movimentiZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
        Dim movDettagli_W As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
        Dim movDestinazioni_W As New AgronicaCoreContabBIZ.Mov_Destinazioni_W
        Dim objFarmaci_R As New AgronicaCoreMetaSchemaDAL.Farmaci

        Dim piva As String = agenda.PIVA
        Dim idAgenda As Integer = agenda.Id_Agenda

        '-------------------------------------------------------------------------
        'MOVIMENTI ---- SCARICO FARMACO ANIMALE
        '-------------------------------------------------------------------------
        Dim mov_ScaricoFarmCapo As New AgronicaCoreEntityFramework_POCO.Movimenti

        If esiste Then mov_ScaricoFarmCapo = GiasContext.Movimenti.
            Where(Function(mov) mov.PIVA = piva AndAlso mov.Id_Agenda = idAgenda AndAlso mov.Cau_Mov = CAU_TRATTAMENTO_ZOO).First

        mov_ScaricoFarmCapo.PIVA = piva
        mov_ScaricoFarmCapo.Sa_Cod = 0
        mov_ScaricoFarmCapo.Id_Agenda = idAgenda
        mov_ScaricoFarmCapo.Cau_Mov = CAU_TRATTAMENTO_ZOO
        mov_ScaricoFarmCapo.Mov_Desc = agenda.des_lib
        mov_ScaricoFarmCapo.Data_Movimento = attivita.inizio
        mov_ScaricoFarmCapo.Scadenza = AGRODATAFINE
        mov_ScaricoFarmCapo.Data_Registrazione = agenda.Data_Creazione
        mov_ScaricoFarmCapo.Validita_Inizio = attivita.inizio

        Dim idMov_ScaricoFarmCapo As Integer = movimenti_W.Scrivi_Modifica(mov_ScaricoFarmCapo, GiasContext, objP_Server)
        'GiasContext.SaveChanges()

        '-------------------------------------------------------------------------
        'MOVIMENTI_ZOO ---- SCARICO FARMACO ANIMALE
        '-------------------------------------------------------------------------
        'recupero il numero di prescrizione, il tipo di prescrizione e il numero di riga della prescrizione
        Dim presCodici = getPresNumeroFromAgenda(idAgenda, GiasContext)

        If presCodici.Pres_Numero <> "" AndAlso presCodici.PresRiga_Numero <> "" Then
            Dim presNumero As String = presCodici.Pres_Numero
            Dim presTipo As String = presCodici.Pres_Tipo
            Dim presRigaNumero As String = presCodici.PresRiga_Numero

            Dim movZoo_TestataScaricoFarmCapo As New AgronicaCoreEntityFramework_POCO.Movimenti_Zoo
            movZoo_TestataScaricoFarmCapo.Piva = agenda.PIVA
            movZoo_TestataScaricoFarmCapo.Sa_Cod = 0
            movZoo_TestataScaricoFarmCapo.Id_Agenda = idAgenda
            movZoo_TestataScaricoFarmCapo.Id_Mov = idMov_ScaricoFarmCapo
            movZoo_TestataScaricoFarmCapo.Pres_Numero = presNumero
            movZoo_TestataScaricoFarmCapo.PresRiga_Numero = presRigaNumero
            movZoo_TestataScaricoFarmCapo.Tipo_Trattamento = presTipo
            movZoo_TestataScaricoFarmCapo.Stato_Trattamento = 0

            movimentiZoo_W.Scrivi_Modifica(movZoo_TestataScaricoFarmCapo, GiasContext, objP_Server)
            'GiasContext.SaveChanges()
        End If

        Dim listIdMovDett As New List(Of Integer)
        For Each prod In attivita.risorse
            If prod.classType.Equals(costanti.ClassType.DettaglioRegistroSomministrazioni) Then
                Dim somministrazione As dettagli.DettaglioRegistroSomministrazioni = CType(prod, dettagli.DettaglioRegistroSomministrazioni)
                agenda.Validita_Inizio = somministrazione.validita.inizio
                agenda.Validita_Fine = somministrazione.validita.fine

                If String.IsNullOrEmpty(somministrazione.codiceAIC) Then Continue For

                'recupera Pro_Cod nella tabella Farmaci
                Dim dtFarmaco As DataTable = objFarmaci_R.leggi(objP_Server, 0, {somministrazione.codiceAIC})

                If Not IsNothing(dtFarmaco) AndAlso dtFarmaco.Rows.Count > 0 Then
                    Dim farmCod As Integer = dtFarmaco(0)("Farm_Cod")

                    Dim udmCod As Integer = 0
                    Dim selUdm As Integer = 0
                    Dim convQta As Decimal = 0

                    Select Case somministrazione.unitaDiMisura.codice
                        Case enum_UnitaMisura.Litri
                            udmCod = enum_UnitaMisura.Litri
                            selUdm = enum_UnitaMisura.Litri
                        Case enum_UnitaMisura.Numero
                            udmCod = enum_UnitaMisura.Numero
                            selUdm = enum_UnitaMisura.Numero
                            convQta = somministrazione.quantitaTotaleReale
                        Case enum_UnitaMisura.Grammi
                            udmCod = enum_UnitaMisura.KG
                            selUdm = enum_UnitaMisura.Grammi
                            convQta = somministrazione.quantitaTotaleReale
                        Case enum_UnitaMisura.Millilitri
                            convQta = UnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, somministrazione.quantitaTotaleReale, enum_UnitaMisura.Litri)
                            udmCod = enum_UnitaMisura.Litri
                            selUdm = enum_UnitaMisura.Millilitri
                        Case Else
                            udmCod = somministrazione.unitaDiMisura.codice
                            convQta = somministrazione.quantitaTotaleReale
                    End Select

                    Dim percSomm As Decimal = 0
                    Dim doseAnimaleReale As Decimal = 0

                    'calcola la percentuale di prodotto scaricata sul singolo animale
                    If attivita.centriDiCosto.Count > 0 Then
                        percSomm = 1 / attivita.centriDiCosto.Count
                        doseAnimaleReale = somministrazione.quantitaTotaleReale / attivita.centriDiCosto.Count
                    End If

                    '-------------------------------------------------------------------------
                    'MOVIMENTI_DETTAGLI ---- SCARICO FARMACO ANIMALE
                    '-------------------------------------------------------------------------
                    Dim movDett_ScaricoFarmCapo As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli
                    If esiste Then movDett_ScaricoFarmCapo = GiasContext.Movimenti_dettagli.
                        Where(Function(mdtt) mdtt.PIVA = piva AndAlso mdtt.Id_Agenda = idAgenda AndAlso mdtt.Id_Mov = idMov_ScaricoFarmCapo AndAlso mdtt.Pro_Cod = farmCod).FirstOrDefault

                    movDett_ScaricoFarmCapo.PIVA = mov_ScaricoFarmCapo.PIVA
                    movDett_ScaricoFarmCapo.Sa_Cod = 0
                    movDett_ScaricoFarmCapo.Id_Agenda = idAgenda
                    movDett_ScaricoFarmCapo.Id_Mov = idMov_ScaricoFarmCapo
                    movDett_ScaricoFarmCapo.Elem_Cod = CostantiPersonalizzate.FARMACI
                    movDett_ScaricoFarmCapo.Pro_Cod = farmCod
                    movDett_ScaricoFarmCapo.Mov_Det_Des = dtFarmaco(0)("Denominazione") & " (" & dtFarmaco(0)("Confezione") & " - " & convQta & ")"
                    movDett_ScaricoFarmCapo.Udm_Cod = udmCod
                    movDett_ScaricoFarmCapo.Qta = somministrazione.quantitaTotaleReale
                    movDett_ScaricoFarmCapo.Qta_Extra_Totale = somministrazione.quantitaTotaleReale
                    movDett_ScaricoFarmCapo.Contabilizzato = NONCONTABILE
                    movDett_ScaricoFarmCapo.RegSco_Numero = somministrazione.regSco_Numero
                    movDett_ScaricoFarmCapo.Extra_Str = somministrazione.codice
                    movDett_ScaricoFarmCapo.Extra_Int = selUdm
                    movDett_ScaricoFarmCapo.Extra_Date = somministrazione.dataPrescrizione
                    movDett_ScaricoFarmCapo.UDM_COD_EXTRA = 0
                    movDett_ScaricoFarmCapo.QTA_EXTRA = 0
                    movDett_ScaricoFarmCapo.Prezzo_Effettivo = 0
                    movDett_ScaricoFarmCapo.ChkIva_Manuale = 0
                    movDett_ScaricoFarmCapo.Cod_IvaIndetraibile = 0
                    movDett_ScaricoFarmCapo.Tara = 0
                    movDett_ScaricoFarmCapo.ChkLayOut_Hide = 0
                    movDett_ScaricoFarmCapo.Variazione = 0
                    movDett_ScaricoFarmCapo.Listino_Cod = 0
                    movDett_ScaricoFarmCapo.Sconto_Listino = 0
                    movDett_ScaricoFarmCapo.Sconto_Modalita = 0
                    movDett_ScaricoFarmCapo.Rif_Esterno = somministrazione.numTrattamento

                    Dim idMovDett As Integer = movDettagli_W.Scrivi_Modifica(movDett_ScaricoFarmCapo, GiasContext, objP_Server)

                    If somministrazione.sospensione IsNot Nothing AndAlso somministrazione.sospensione.Count > 0 Then
                        For Each sospensione In somministrazione.sospensione
                            Dim alimentoCod As Integer = sospensione.Alimento.codice

                            Dim movDettTecn As New AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico
                            If esiste Then movDettTecn = GiasContext.Mov_Dettaglio_Tecnico.
                                Where(Function(mdt) mdt.Piva = piva AndAlso mdt.Id_Agenda = idAgenda AndAlso mdt.Id_Mov = idMov_ScaricoFarmCapo AndAlso mdt.Id_Mov_Det = idMovDett AndAlso
                                    mdt.Dett_Cod = alimentoCod).FirstOrDefault

                            movDettTecn.Piva = mov_ScaricoFarmCapo.PIVA
                            movDettTecn.Sa_Cod = 0
                            movDettTecn.Id_Agenda = idAgenda
                            movDettTecn.Id_Mov = idMov_ScaricoFarmCapo
                            movDettTecn.Id_Mov_Det = idMovDett
                            movDettTecn.Dett_Cod = alimentoCod
                            movDettTecn.Extra_Int = sospensione.tempoSospensione

                            Dim movDettTecninco_W As New AgronicaCoreContabBIZ.Mov_Dettaglio_Tecnico_W
                            movDettTecninco_W.Scrivi_Modifica(movDettTecn, GiasContext, objP_Server)
                        Next
                    End If

                    'GiasContext.SaveChanges()

                    For Each cdc In attivita.centriDiCosto
                        If cdc.classType.Equals(costanti.ClassType.CapoAnimaleCDC) Then
                            Dim capoAnimaleCDC As centri_di_costo.CapoAnimaleCDC = CType(cdc, centri_di_costo.CapoAnimaleCDC)
                            Dim capo As CapoAnimale = capoAnimaleCDC.capoAnimale
                            Dim qtaCapo As Decimal = somministrazione.quantitaTotaleReale * percSomm

                            '-------------------------------------------------------------------------
                            'MOV_DESTINAZIONI ---- SCARICO FARMACO ANIMALE
                            '-------------------------------------------------------------------------
                            Dim movDest_ScaricoFarmCapo As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                            movDest_ScaricoFarmCapo.Piva = movDett_ScaricoFarmCapo.PIVA
                            movDest_ScaricoFarmCapo.Sa_Cod = movDett_ScaricoFarmCapo.Sa_Cod
                            movDest_ScaricoFarmCapo.Id_Agenda = idAgenda
                            movDest_ScaricoFarmCapo.Id_Mov = idMov_ScaricoFarmCapo
                            movDest_ScaricoFarmCapo.Id_Mov_Det = idMovDett
                            movDest_ScaricoFarmCapo.Appezza = 0
                            movDest_ScaricoFarmCapo.Id_Destinazione = capo.codice
                            movDest_ScaricoFarmCapo.Tipo_Destinazione = TIPO_DESTINAZIONE_ANIMALE
                            movDest_ScaricoFarmCapo.Qta = qtaCapo
                            movDest_ScaricoFarmCapo.Qta2 = 0
                            movDest_ScaricoFarmCapo.inviato = 0
                            movDest_ScaricoFarmCapo.username_creazione = agenda.Username_Creazione
                            movDest_ScaricoFarmCapo.data_creazione = Date.Now
                            movDest_ScaricoFarmCapo.username_modifica = agenda.Username_Modifica
                            movDest_ScaricoFarmCapo.data_modifica = Date.Now
                            movDest_ScaricoFarmCapo.validita_inizio = mov_ScaricoFarmCapo.Data_Movimento
                            movDest_ScaricoFarmCapo.validita_fine = AGRODATAFINE
                            movDest_ScaricoFarmCapo.Tipo_Scorta = 0
                            movDest_ScaricoFarmCapo.Scorta_Min = 0
                            movDest_ScaricoFarmCapo.mov_destinazioni_graphickey = capo.matricola
                            movDest_ScaricoFarmCapo.Qta_Dest1 = 0
                            movDest_ScaricoFarmCapo.Qta_Dest2 = 0
                            movDest_ScaricoFarmCapo.QuotaDistribuzione = percSomm
                            movDest_ScaricoFarmCapo.Sa_Cod_Riferimento = 0
                            movDest_ScaricoFarmCapo.Id_Destinazione_Riferimento = 0
                            movDest_ScaricoFarmCapo.Tipo_Destinazione_Riferimento = 0
                            movDest_ScaricoFarmCapo.Sup_Riduzione_BufferZone = 0
                            movDest_ScaricoFarmCapo.Perc_Riduzione_Deriva = 0

                            movDestinazioni_W.Scrivi_Modifica(movDest_ScaricoFarmCapo, GiasContext, objP_Server)
                            'GiasContext.SaveChanges()

                            listIdMovDett.Add(idMovDett)
                            Dim movDettToDelete = GiasContext.Movimenti_dettagli.Where(Function(md) md.Id_Agenda = idAgenda AndAlso md.Id_Mov = idMov_ScaricoFarmCapo AndAlso listIdMovDett.Contains(md.Id_Mov_Det)).ToList
                        End If
                    Next
                Else
                    Throw New Exception(FarmacoNonConfiguratoGIAS)
                End If

            ElseIf prod.classType.Equals(costanti.ClassType.RisorsaProdotto) Then
                Dim somministrazione As risorse.RisorsaProdotto = CType(prod, risorse.RisorsaProdotto)
                Dim dettSomministrazione As New dettagli.DettaglioRegistroSomministrazioni
                Dim effettuaScaricoMagazzino As Boolean = False

                'controllo che ci sia uno scarico farmaco su animale che corrisponda allo scarico farmaco da magazzino da effettuare
                For Each ris In attivita.risorse
                    If ris.classType.Equals(costanti.ClassType.DettaglioRegistroSomministrazioni) Then
                        dettSomministrazione = CType(ris, dettagli.DettaglioRegistroSomministrazioni)

                        If dettSomministrazione.codice = somministrazione.prodotto.codice_alfanumerico Then
                            effettuaScaricoMagazzino = True
                            Exit For
                        End If
                    End If
                Next

                If effettuaScaricoMagazzino Then
                    Dim magazzino As Fabbricato = (somministrazione.MagazziniMovimentazioni(0)).Magazzino
                    Dim lotto As String = (somministrazione.MagazziniMovimentazioni(0)).Lotto

                    'recupera Pro_Cod nella tabella Farmaci utilizzando DettaglioSomministrazione
                    Dim dtFarmaco As DataTable = objFarmaci_R.leggi(objP_Server, 0, {dettSomministrazione.codiceAIC})

                    If Not IsNothing(dtFarmaco) AndAlso dtFarmaco.Rows.Count > 0 Then
                        Dim farmCod As Integer = dtFarmaco(0)("Farm_Cod")

                        Dim udmCod As Integer = 0
                        Dim qtaConv As Decimal = 0
                        Dim selUdm As Integer = 0
                        If somministrazione.unitaDiMisura IsNot Nothing Then
                            Select Case somministrazione.unitaDiMisura.codice
                                Case enum_UnitaMisura.Litri
                                    qtaConv = UnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, somministrazione.quantitaTotaleReale, enum_UnitaMisura.Litri)
                                    udmCod = enum_UnitaMisura.Litri
                                    selUdm = enum_UnitaMisura.Millilitri
                                Case enum_UnitaMisura.Numero
                                    udmCod = enum_UnitaMisura.Numero
                                    selUdm = enum_UnitaMisura.Numero
                                    qtaConv = somministrazione.quantitaTotaleReale
                                Case enum_UnitaMisura.KG
                                    qtaConv = UnitaMisura_R.Converti(enum_UnitaMisura.Grammi, somministrazione.quantitaTotaleReale, enum_UnitaMisura.KG)
                                    udmCod = enum_UnitaMisura.KG
                                    selUdm = enum_UnitaMisura.Grammi
                                    qtaConv = somministrazione.quantitaTotaleReale
                                Case Else
                                    udmCod = somministrazione.unitaDiMisura.codice
                                    qtaConv = somministrazione.quantitaTotaleReale
                            End Select
                        Else
                            Select Case somministrazione.MagazziniMovimentazioni(0).udm.codice
                                Case enum_UnitaMisura.Litri
                                    qtaConv = UnitaMisura_R.Converti(enum_UnitaMisura.Millilitri, somministrazione.quantitaTotaleReale, enum_UnitaMisura.Litri)
                                    udmCod = enum_UnitaMisura.Litri
                                    selUdm = enum_UnitaMisura.Millilitri
                                Case enum_UnitaMisura.Numero
                                    udmCod = enum_UnitaMisura.Numero
                                    selUdm = enum_UnitaMisura.Numero
                                    If somministrazione.quantitaTotaleReale = 0 Then
                                        qtaConv = somministrazione.MagazziniMovimentazioni(0).Qta
                                    Else
                                        qtaConv = somministrazione.quantitaTotaleReale
                                    End If
                                Case enum_UnitaMisura.KG
                                    qtaConv = UnitaMisura_R.Converti(enum_UnitaMisura.Grammi, somministrazione.quantitaTotaleReale, enum_UnitaMisura.KG)
                                    udmCod = enum_UnitaMisura.KG
                                    selUdm = enum_UnitaMisura.Grammi
                                    qtaConv = somministrazione.quantitaTotaleReale
                                Case Else
                                    udmCod = somministrazione.MagazziniMovimentazioni(0).udm.codice
                                    qtaConv = somministrazione.MagazziniMovimentazioni(0).Qta
                            End Select
                        End If


                        Dim giacenzeProd_R As New AgronicaCoreContabDAL.Giacenze_R
                        Dim giacenzaFarm As Decimal =
                            giacenzeProd_R.Verifica_Giacenza((Date.Now).Date, magazzino.primaryKey.centroAziendalePK.partitaIva,
                                                             magazzino.primaryKey.centroAziendalePK.codice, 0, CostantiPersonalizzate.FARMACI,
                                                             dtFarmaco(0)("Farm_Cod"), 0, 0, 0,
                                                             0, udmCod, "", objP_Server, objP_Utenti)

                        '-------------------------------------------------------------------------
                        'MOVIMENTI ---- SCARICO FARMACO MAGAZZINO
                        '-------------------------------------------------------------------------
                        Dim mov_ScaricoFarmMagaz As New AgronicaCoreEntityFramework_POCO.Movimenti
                        If esiste Then mov_ScaricoFarmMagaz = GiasContext.Movimenti.
                            Where(Function(mov) mov.PIVA = piva AndAlso mov.Id_Agenda = idAgenda AndAlso mov.Cau_Mov = CAU_SCARICO).First

                        mov_ScaricoFarmMagaz.PIVA = agenda.PIVA
                        mov_ScaricoFarmMagaz.Sa_Cod = 0
                        mov_ScaricoFarmMagaz.Id_Agenda = idAgenda
                        mov_ScaricoFarmMagaz.Cau_Mov = CAU_SCARICO
                        mov_ScaricoFarmMagaz.Mov_Desc = agenda.des_lib
                        mov_ScaricoFarmMagaz.Data_Movimento = attivita.inizio
                        mov_ScaricoFarmMagaz.Scadenza = AGRODATAFINE
                        mov_ScaricoFarmMagaz.Username_Creazione = agenda.Username_Creazione
                        mov_ScaricoFarmMagaz.Data_Registrazione = agenda.Data_Creazione
                        mov_ScaricoFarmMagaz.Validita_Inizio = attivita.inizio

                        Dim idMov_ScaricoFarmMagaz As Integer = movimenti_W.Scrivi_Modifica(mov_ScaricoFarmMagaz, GiasContext, objP_Server)
                        'GiasContext.SaveChanges()

                        '-------------------------------------------------------------------------
                        'MOVIMENTI_DETTAGLI ---- SCARICO FARMACO MAGAZZINO
                        '-------------------------------------------------------------------------
                        Dim movDett_ScaricoFarmMagaz As New AgronicaCoreEntityFramework_POCO.Movimenti_dettagli
                        If esiste Then movDett_ScaricoFarmMagaz = GiasContext.Movimenti_dettagli.
                            Where(Function(mdtt) mdtt.PIVA = piva AndAlso mdtt.Id_Agenda = idAgenda AndAlso mdtt.Id_Mov = idMov_ScaricoFarmMagaz AndAlso mdtt.Pro_Cod = farmCod).FirstOrDefault

                        movDett_ScaricoFarmMagaz.PIVA = mov_ScaricoFarmMagaz.PIVA
                        movDett_ScaricoFarmMagaz.Sa_Cod = magazzino.primaryKey.centroAziendalePK.codice
                        movDett_ScaricoFarmMagaz.Id_Agenda = idAgenda
                        movDett_ScaricoFarmMagaz.Id_Mov = mov_ScaricoFarmMagaz.Id_Mov
                        movDett_ScaricoFarmMagaz.Elem_Cod = CostantiPersonalizzate.FARMACI
                        movDett_ScaricoFarmMagaz.Pro_Cod = farmCod
                        movDett_ScaricoFarmMagaz.Mov_Det_Des = dtFarmaco(0)("Denominazione") & " (" & dtFarmaco(0)("Confezione") & " - " & qtaConv & ")"
                        movDett_ScaricoFarmMagaz.Udm_Cod = udmCod
                        movDett_ScaricoFarmMagaz.Qta = qtaConv
                        movDett_ScaricoFarmMagaz.Qta_Extra_Totale = qtaConv
                        movDett_ScaricoFarmMagaz.Contabilizzato = NONCONTABILE
                        movDett_ScaricoFarmMagaz.Extra_Str = somministrazione.prodotto.codice_alfanumerico
                        movDett_ScaricoFarmMagaz.Extra_Int = dettSomministrazione.quantitaTotaleReale
                        movDett_ScaricoFarmMagaz.Extra_Date = AGRODATAINIZIO
                        movDett_ScaricoFarmMagaz.Lotto = lotto
                        movDett_ScaricoFarmMagaz.UDM_COD_EXTRA = 0
                        movDett_ScaricoFarmMagaz.QTA_EXTRA = 0
                        movDett_ScaricoFarmMagaz.Prezzo_Effettivo = 0
                        movDett_ScaricoFarmMagaz.ChkIva_Manuale = 0
                        movDett_ScaricoFarmMagaz.Cod_IvaIndetraibile = 0
                        movDett_ScaricoFarmMagaz.Tara = 0
                        movDett_ScaricoFarmMagaz.ChkLayOut_Hide = 0
                        movDett_ScaricoFarmMagaz.Variazione = 0
                        movDett_ScaricoFarmMagaz.Listino_Cod = 0
                        movDett_ScaricoFarmMagaz.Sconto_Listino = 0
                        movDett_ScaricoFarmMagaz.Sconto_Modalita = 0

                        Dim idMovDet As Integer = movDettagli_W.Scrivi_Modifica(movDett_ScaricoFarmMagaz, GiasContext, objP_Server)
                        'GiasContext.SaveChanges()

                        '-------------------------------------------------------------------------
                        'MOV_DESTINAZIONI ---- SCARICO FARMACO MAGAZZINO
                        '-------------------------------------------------------------------------
                        Dim movDest_ScaricoFarmMagaz As New AgronicaCoreEntityFramework_POCO.Mov_Destinazioni

                        movDest_ScaricoFarmMagaz.Piva = movDett_ScaricoFarmMagaz.PIVA
                        movDest_ScaricoFarmMagaz.Sa_Cod = magazzino.primaryKey.centroAziendalePK.codice
                        movDest_ScaricoFarmMagaz.Id_Agenda = idAgenda
                        movDest_ScaricoFarmMagaz.Id_Mov = idMov_ScaricoFarmMagaz
                        movDest_ScaricoFarmMagaz.Id_Mov_Det = idMovDet
                        movDest_ScaricoFarmMagaz.Appezza = 0
                        movDest_ScaricoFarmMagaz.Id_Destinazione = magazzino.primaryKey.codice
                        movDest_ScaricoFarmMagaz.Tipo_Destinazione = magazzino.tipo
                        movDest_ScaricoFarmMagaz.Qta = qtaConv
                        movDest_ScaricoFarmMagaz.Qta2 = 0
                        movDest_ScaricoFarmMagaz.validita_inizio = mov_ScaricoFarmMagaz.Data_Movimento
                        movDest_ScaricoFarmMagaz.validita_fine = AGRODATAFINE
                        movDest_ScaricoFarmMagaz.Tipo_Scorta = 0
                        movDest_ScaricoFarmMagaz.Scorta_Min = 0
                        movDest_ScaricoFarmMagaz.QuotaDistribuzione = 0
                        movDest_ScaricoFarmMagaz.Sa_Cod_Riferimento = 0
                        movDest_ScaricoFarmMagaz.Id_Destinazione_Riferimento = 0
                        movDest_ScaricoFarmMagaz.Tipo_Destinazione_Riferimento = 0
                        movDest_ScaricoFarmMagaz.Sup_Riduzione_BufferZone = 0
                        movDest_ScaricoFarmMagaz.Perc_Riduzione_Deriva = 0

                        Dim idMovDest As Integer = movDestinazioni_W.Scrivi_Modifica(movDest_ScaricoFarmMagaz, GiasContext, objP_Server)
                        'GiasContext.SaveChanges()

                    End If

                End If

            End If

        Next

    End Sub

    Private Function getPresNumeroFromAgenda(ByVal idAgenda As Integer,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities)
        Dim agxRZ = GiasContext.Ricette_ZooxAgenda.Where(Function(rzxa) rzxa.Id_Agenda = idAgenda).FirstOrDefault
        If agxRZ Is Nothing Then Return New With {Key .Pres_Numero = "", Key .Pres_Tipo = 0, Key .PresRiga_Numero = ""}

        Dim idRicetta As Integer = agxRZ.Id_Ricetta
        Dim idRiga As Integer = agxRZ.Id_RigaRicetta

        Dim ricZoo = GiasContext.Ricette_Zoo.Where(Function(rz) rz.IdRicetta = idRicetta).FirstOrDefault
        If ricZoo Is Nothing Then Return New With {Key .Pres_Numero = "", Key .Pres_Tipo = 0, Key .PresRiga_Numero = ""}

        Dim rigaRicZoo = GiasContext.Ricette_Zoo_Agenda.Where(Function(rza) rza.IdRicetta = idRicetta AndAlso rza.IdAgenda = idRiga).FirstOrDefault
        If ricZoo Is Nothing Then Return New With {Key .Pres_Numero = ricZoo.Numero, Key .Pres_Tipo = ricZoo.TipoCodice, Key .PresRiga_Numero = ""}

        Return New With {Key .Pres_Numero = ricZoo.Numero, Key .Pres_Tipo = ricZoo.TipoCodice, Key .PresRiga_Numero = rigaRicZoo.Numero}

    End Function

End Class
