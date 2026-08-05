Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class EFContatti

    Public Shared Function CreateContattiEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            ByRef piva As String,
                                            ByRef sa_cod As String,
                                            ByRef cod_Contatto As String,
                                            ByRef id_CF As Integer,
                                            ByRef username As String
                                            ) As AgronicaCoreEntityFramework_POCO.Contatti

        Dim contatto As New AgronicaCoreEntityFramework_POCO.Contatti

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(piva) Then
            Throw New Exception("Rilevato carattere non valido nella PIVA:" & piva)
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(cod_Contatto) Then
            Throw New Exception("Rilevato carattere non valido nel codice contatto:" & piva)
        End If

        contatto.Piva = piva
        contatto.Sa_Cod = sa_cod
        contatto.Cod_Contatto = cod_Contatto
        contatto.Id_CF = id_CF

        contatto.Rag_Soc = ""
        contatto.Convenevoli = "Spett.le"
        contatto.Inviato = 0
        contatto.Data_Creazione = DateTime.Now
        contatto.Data_Modifica = DateTime.Now
        contatto.Validita_Inizio = AGRODATAINIZIO
        contatto.Validita_Fine = AGRODATAFINE
        contatto.Username_Creazione = username
        contatto.Username_Modifica = username
        contatto.Codice_Fiscale = ""
        contatto.Tipo_Indirizzo_Default = 0
        contatto.Nome = ""
        contatto.Cognome = ""
        contatto.Data_Nascita = AGRODATAINIZIO
        contatto.Sesso = ""
        contatto.Cod_Contatto_Referente = ""
        contatto.Tipo_Speditore = 0
        contatto.Tipo_Destinazione = 0
        contatto.Agente_Cod = 0
        contatto.Provvigione = 0
        contatto.Note = ""
        contatto.Id_Gestione_Note = 0
        contatto.Note2 = ""
        contatto.Note_Operazioni = ""
        contatto.Note2_Operazioni = ""
        contatto.Cod_Risum_Destinazione_Diversa = 0
        contatto.Tipo_Indirizzo_Default_Destinazione_Diversa = 0
        contatto.Fido = 0
        contatto.Limite_Posizioni = 0
        contatto.Limite_Giorni_Evasione = 0
        contatto.Orari_Ritiro = ""
        contatto.Filtro_Rimborsi = ""
        contatto.Vettore_Cod = 0
        contatto.CapoArea_Cod = 0
        contatto.Provvigione_CapoArea = 0
        contatto.Memo = ""
        contatto.Sconto_Contatto = 0
        contatto.Sconto_Testo = ""
        contatto.Modalita_Fatturazione = 0
        contatto.Cod_Iva_Contatto = -1
        contatto.Documento_Fatturazione = 0
        contatto.NrBadge = ""
        contatto.ChkFittizio = 0
        contatto.Cod_Conto_Econ = 0
        contatto.Cod_Conto_Pat = 0
        contatto.Nome_Breve = ""

        dal.Contatti.Add(contatto)
        dal.SaveChanges()

        Return contatto

    End Function

    Public Shared Function CreateIndirizzoContatto(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                   ByRef tipoIndirizzo As Integer,
                                                   ByRef username As String
                                                   ) As AgronicaCoreEntityFramework_POCO.Indirizzi

        Dim ind = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)

        Dim cxi = CreateContattiXIndirizzi(dal, objParametri, piva, sa_cod, contatto.Cod_Contatto, ind.cod_indirizzo, tipoIndirizzo, username)

        Return ind

    End Function

    Public Shared Function CreateIndirizzoContatto(ByRef dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                   ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                   ByRef tipoIndirizzo As Integer,
                                                   ByRef username As String
                                                   ) As AgronicaCoreEntityFramework_POCO.Indirizzi

        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)

        Dim cxi = CreateContattiXIndirizzi(dal, objParametri, piva, sa_cod, contatto.Cod_Contatto, indirizzo.cod_indirizzo, tipoIndirizzo, username)

        indirizzo.ind_des = dati_indirizzo.via
        indirizzo.frz_des = dati_indirizzo.frazione
        indirizzo.CAP = dati_indirizzo.cap
        If dati_indirizzo.istatComune.localita IsNot Nothing AndAlso dati_indirizzo.istatComune.localita <> "" Then
            indirizzo.com_des = dati_indirizzo.istatComune.localita
        End If

        If dati_indirizzo.istatComune.comuni_prov IsNot Nothing AndAlso dati_indirizzo.istatComune.comuni_prov <> "" Then
            indirizzo.pro_cod = dati_indirizzo.istatComune.comuni_prov
        End If
        indirizzo.stato = dati_indirizzo.stato.codice
        indirizzo.note = dati_indirizzo.note
        indirizzo.pro_cod_istat = dati_indirizzo.istatComune.prov
        indirizzo.com_cod_istat = dati_indirizzo.istatComune.com

        Return indirizzo

    End Function

    Private Shared Function CreateContattiXIndirizzi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     ByRef piva As String,
                                                     ByRef sa_cod As Integer,
                                                     ByRef cod_contatto As String,
                                                     ByRef cod_indirizzo As Integer,
                                                     ByRef tipo_Indirizzo As Integer,
                                                     ByRef username As String
                                                     ) As AgronicaCoreEntityFramework_POCO.ContattiXIndirizzi

        Dim cxi As New AgronicaCoreEntityFramework_POCO.ContattiXIndirizzi

        cxi.Piva = piva
        cxi.Sa_Cod = sa_cod
        cxi.Cod_Contatto = cod_contatto
        cxi.Cod_Indirizzo = cod_indirizzo
        cxi.Tipo_Indirizzo = tipo_Indirizzo

        cxi.Inviato = 0
        cxi.Data_Creazione = DateTime.Now
        cxi.Data_Modifica = DateTime.Now
        cxi.Validita_Inizio = AGRODATAINIZIO
        cxi.Validita_Fine = AGRODATAFINE
        cxi.Username_Creazione = username
        cxi.Username_Modifica = username

        dal.ContattiXIndirizzi.Add(cxi)
        dal.SaveChanges()

        Return cxi

    End Function

    Public Shared Function CreateRubricaContatto(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 ByRef piva As String,
                                                 ByRef sa_cod As Integer,
                                                 ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                 ByRef numero As String,
                                                 ByRef descr As String,
                                                 ByRef username As String
                                                 ) As AgronicaCoreEntityFramework_POCO.Rubrica

        Dim rubrica = EFRubrica.CreateRubricaEF(dal, objParametri, numero, descr, username)

        Dim conXRub = CreateContattiXRubrica(dal, objParametri, piva, sa_cod, contatto.Cod_Contatto, rubrica.cod_rubrica, username)

        Return rubrica

    End Function

    Private Shared Function CreateContattiXRubrica(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef cod_Contatto As String,
                                                   ByRef cod_Rubrica As Integer,
                                                   ByRef username As String
                                                   ) As AgronicaCoreEntityFramework_POCO.ContattiXRubrica

        Dim conXRub As New AgronicaCoreEntityFramework_POCO.ContattiXRubrica

        conXRub.Piva = piva
        conXRub.Sa_Cod = sa_cod
        conXRub.Cod_Contatto = cod_Contatto
        conXRub.Cod_Rubrica = cod_Rubrica

        conXRub.Inviato = 0
        conXRub.Data_Creazione = DateTime.Now
        conXRub.Data_Modifica = DateTime.Now
        conXRub.Validita_Inizio = AGRODATAINIZIO
        conXRub.Validita_Fine = AGRODATAFINE
        conXRub.Username_Creazione = username
        conXRub.Username_Modifica = username

        dal.ContattiXRubrica.Add(conXRub)
        dal.SaveChanges()

        Return conXRub

    End Function

    Public Shared Function CreateRisorse_Umane(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                               ByRef username As String
                                               ) As AgronicaCoreEntityFramework_POCO.Risorse_Umane

        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim risUm As New AgronicaCoreEntityFramework_POCO.Risorse_Umane

        risUm.Piva = contatto.Piva
        risUm.Sa_Cod = contatto.Sa_Cod
        risUm.Cod_RisUm = idGen.NuovoId_Tabella_EF(dal, "Risorse_Umane", 0, 2000000, objParametri)
        risUm.Cod_Contatto = contatto.Cod_Contatto

        risUm.Validita_Inizio = AGRODATAINIZIO
        risUm.Validita_Fine = AGRODATAFINE
        risUm.Settore_Des = ""
        risUm.Attivita_Des = ""
        risUm.Corrispettivo_Mensile = 0
        risUm.Corrispettivo_Orario = 0
        risUm.Occasionale = 0
        risUm.Ore_Settimanali = 0
        risUm.Giorni_Ferie = 0
        risUm.Ferie_Godute = 0
        risUm.Giorni_Malattia = 0
        risUm.Inviato = 0
        risUm.Data_Creazione = DateTime.Now
        risUm.Data_Modifica = DateTime.Now
        risUm.Username_Creazione = username
        risUm.Username_Modifica = username
        risUm.Patentino = ""
        risUm.Data_Rilascio_Patentino = AGRODATAINIZIO
        risUm.Data_Scadenza_Patentino = AGRODATAFINE
        risUm.Cod_RisUm_Origine = 0
        risUm.Piva_SuperUser_Origine = ""
        risUm.Ente_di_rilascio = ""
        risUm.Saldo_Iniziale_Crediti = 0
        risUm.Saldo_Iniziale_Debiti = 0
        risUm.ChkSpesometro = 1
        risUm.ChkBlocco = 0
        risUm.Blocco_Des = ""
        risUm.Qualifica_Cod = 0
        risUm.Mansione_Cod = 0
        risUm.Info_Famiglia = ""
        risUm.Classificazione_Cod = 0
        risUm.Ra_Cod = ""
        risUm.Cod_Iva_Contatto = -1
        risUm.Cod_Conto_Econ = 0
        risUm.Cod_Conto_Pat = 0

        dal.Risorse_Umane.Add(risUm)
        dal.SaveChanges()

        Return risUm

    End Function

    Public Shared Function Create_ContattiCodici(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef contatto As AgronicaCoreEntityFramework_POCO.Contatti,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Contatti_Codici

        Dim cont = Create_ContattiCodici(dal, objParametri, contatto.Piva, contatto.Sa_Cod, contatto.Cod_Contatto, id_cod, val_cod, username)

        Return cont
    End Function


    Private Shared Function Create_ContattiCodici(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As String,
                                                   ByRef cod_contatto As String,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Contatti_Codici

        Dim contatti As New AgronicaCoreEntityFramework_POCO.Contatti_Codici

        contatti.PIVA = piva
        contatti.Cod_Contatto = cod_contatto
        contatti.Sa_Cod = sa_cod

        contatti.Id_cod = id_cod
        contatti.Val_cod = val_cod

        contatti.inviato = 0
        contatti.datainvio = DateTime.Now

        contatti.Data_Creazione = DateTime.Now
        contatti.Data_Modifica = DateTime.Now

        contatti.Validita_Inizio = AGRODATAINIZIO
        contatti.Validita_Fine = AGRODATAFINE

        contatti.Username_Creazione = username
        contatti.Username_Modifica = username

        dal.SaveChanges()

        Return contatti
    End Function

End Class
