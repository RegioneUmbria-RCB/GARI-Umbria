Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class EFCentri_Aziendali
    Public Shared Function CreateCentri_AziendaliEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef piva As String,
                                                    ByRef sa_nome As String,
                                                    ByRef username As String,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                    Optional NoteLog As String = "") As AgronicaCoreEntityFramework_POCO.Centri_Aziendali
        Dim centro As New AgronicaCoreEntityFramework_POCO.Centri_Aziendali
        centro.PIVA = piva
        centro.sa_cod = NuovoSa_Cod(piva, objParametri, objParametri_Utenti)
        centro.sa_nome = sa_nome
        centro.X = 0
        centro.Y = 0
        centro.ZSLM = 0
        centro.long = 0
        centro.lat = 0
        centro.area = 0
        centro.ca_sipi = ""
        centro.AT_Prevalente = ""
        centro.Forma_Possesso = ""
        centro.TitoloPossesso = 1
        centro.Sup_Totale = 0
        centro.Sup_Bosco = 0
        centro.Sup_SAU = 0
        centro.Sup_Prati = 0
        centro.Sup_SAU_Biologico = 0
        centro.Sup_SAU_Convenzionale = 0
        centro.Sup_SAU_Conversione = 0
        centro.inviato = 0
        centro.Data_Creazione = DateTime.Now
        centro.Data_Modifica = DateTime.Now
        centro.Validita_Inizio = AGRODATAINIZIO
        centro.Validita_Fine = AGRODATAFINE
        centro.Username_Creazione = username
        centro.Username_Modifica = username
        centro.Validazione = 0
        centro.Data_Validazione = DateTime.Now
        centro.UserName_Validazione = ""
        dal.Centri_Aziendali.Add(centro)
        dal.SaveChanges()

        'Innesto l'aggiornamento della visibilità utente all'inserimento del centro az
        Dim handleCentriAz As New CentriAziendali_Write
        'handleCentriAz.AggiornaUtentiProfili(objParametri, objParametri_Utenti)
        handleCentriAz.AggiornaUtentiProfilixCentro(objParametri, objParametri_Utenti, piva)

        Return centro
    End Function

    Private Shared Function NuovoSa_Cod(piva As String,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri) As Integer
        Dim idGen As New Agro_Sequenze

        Dim base_code As Long
        Dim top_code As Long

        Dim dal As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        dal.Calcola_BaseCode_TopCode(base_code, top_code, objParametri_Utenti)

        Dim sa_cod = idGen.NuovoId_CentriAziendali(piva, base_code, top_code, objParametri_Server)
        Return sa_cod
    End Function

    Public Shared Function CreateCentri_AziendaliEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef impresa As AgronicaCoreEntityFramework_POCO.Imprese,
                                                    ByRef sa_nome As String,
                                                    ByRef username As String,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreEntityFramework_POCO.Centri_Aziendali
        Dim centro = CreateCentri_AziendaliEF(dal, objParametri, impresa.PIVA, sa_nome, username, objParametri_Utenti)
        'impresa.Centri_Aziendali.Add(centro)
        'dal.SaveChanges()
        Return centro
    End Function

    Private Shared Function CreateCentri_AziendaliCodici(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici
        Dim centroCod As New AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici
        centroCod.PIVA = piva
        centroCod.sa_cod = sa_cod ' NuovoSa_Cod(piva, objParametri)
        centroCod.id_cod = id_cod
        centroCod.val_cod = val_cod
        centroCod.inviato = 0
        centroCod.Data_Creazione = DateTime.Now
        centroCod.Data_Modifica = DateTime.Now
        centroCod.Validita_Inizio = AGRODATAINIZIO
        centroCod.Validita_Fine = AGRODATAFINE
        centroCod.Username_Creazione = username
        centroCod.Username_Modifica = username
        centroCod.Validazione = 0
        centroCod.Data_Validazione = DateTime.Now
        centroCod.UserName_Validazione = ""

        dal.Centri_Aziendali_Codici.Add(centroCod)
        dal.SaveChanges()
        Return centroCod
    End Function

    Public Shared Function CreateCentri_AziendaliCodici(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Centri_Aziendali_Codici
        Dim centroCod = CreateCentri_AziendaliCodici(dal, objParametri, centro.PIVA, centro.sa_cod, id_cod, val_cod, username)
        Return centroCod
    End Function

    Public Shared Function CreateRubricaCentro(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                   ByRef numero As String,
                                                   ByRef descr As String,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Rubrica
        If numero Is Nothing Then
            numero = ""
        End If
        If descr Is Nothing Then
            descr = ""
        End If
        Dim rubrica = EFRubrica.CreateRubricaEF(dal, objParametri, numero, descr, username)
        Dim cxi = CreateRubricaxCentri(dal, objParametri, centro, rubrica, username)
        Return rubrica
    End Function

    Private Shared Function CreateRubricaxCentri(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                    ByRef rubrica As AgronicaCoreEntityFramework_POCO.Rubrica,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.CentrixRubrica
        Dim cxr As New AgronicaCoreEntityFramework_POCO.CentrixRubrica
        cxr.PIVA = centro.PIVA
        cxr.sa_cod = centro.sa_cod
        cxr.cod_rubrica = rubrica.cod_rubrica
        cxr.inviato = 0
        cxr.Data_Creazione = DateTime.Now
        cxr.Data_Modifica = DateTime.Now
        cxr.Validita_Inizio = AGRODATAINIZIO
        cxr.Validita_Fine = AGRODATAFINE
        cxr.Username_Creazione = username
        cxr.Username_Modifica = username
        cxr.Validazione = 0
        cxr.Data_Validazione = DateTime.Now
        cxr.UserName_Validazione = ""
        dal.CentrixRubrica.Add(cxr)
        dal.SaveChanges()
        Return cxr
    End Function

    Public Shared Function CreateCentriAziendaliXParticelle(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                    ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.ImpreseXParticelle
        Dim impxPart As New AgronicaCoreEntityFramework_POCO.ImpreseXParticelle
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        impxPart.ID = idGen.NuovoId_Tabella_EF(dal, "ImpreseXParticelle", 0, 2000000, objParametri)
        impxPart.PIVA = centro.PIVA
        impxPart.sa_cod = centro.sa_cod
        impxPart.PROV = particella.PROV
        impxPart.COM = particella.COM
        impxPart.SEZIONE = particella.SEZIONE
        impxPart.FOGLIO = particella.FOGLIO
        impxPart.NUMERO = particella.NUMERO
        impxPart.SUBALTERNO = particella.SUBALTERNO
        impxPart.TitoloPossesso = particella.TitoloPossesso
        impxPart.PARTITA_CATASTALE = particella.PARTITA_CATASTALE
        impxPart.inviato = 0
        impxPart.Data_Creazione = DateTime.Now
        impxPart.Data_Modifica = DateTime.Now
        impxPart.Validita_Inizio = AGRODATAINIZIO
        impxPart.Validita_Fine = AGRODATAFINE
        impxPart.Username_Creazione = username
        impxPart.Username_Modifica = username
        impxPart.Validazione = 0
        impxPart.Data_Validazione = DateTime.Now
        impxPart.UserName_Validazione = ""
        impxPart.Sup_Condotta = 0
        impxPart.Sup_Spandibile = 0
        impxPart.Sup_Divieto = 0
        impxPart.Irrigabilita = ""
        impxPart.RotazioneColturale = ""
        impxPart.biologico = ""
        impxPart.flagAnomaliaMacrouso = ""
        impxPart.flagContenzioso = ""
        impxPart.flagSupero = ""
        impxPart.Contratto_Cod = 0
        dal.ImpreseXParticelle.Add(impxPart)
        dal.SaveChanges()
        Return impxPart
    End Function

    Public Shared Function CreateIndirizzoCentro(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                    ByRef tipo_indirizzo As Integer,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.Indirizzi
        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)
        Dim cxi = CreateCentrixIndirizzi(dal, objParametri, centro.PIVA, centro.sa_cod, indirizzo.cod_indirizzo, tipo_indirizzo, username)
        Return indirizzo
    End Function

    Public Shared Function CreateIndirizzoCentro(ByRef dati_indirizzo As AgronicaCoreModelsSTD.anagrafiche.Indirizzo,
                                                 ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                 ByRef tipo_indirizzo As Integer,
                                                 ByRef username As String) As AgronicaCoreEntityFramework_POCO.Indirizzi
        Dim indirizzo = EFIndirizzi.CreateIndirizziEF(dal, objParametri, username)
        Dim cxi = CreateCentrixIndirizzi(dal, objParametri, centro.PIVA, centro.sa_cod, indirizzo.cod_indirizzo, tipo_indirizzo, username)

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

        dal.SaveChanges()

        Return indirizzo
    End Function

    Private Shared Function CreateCentrixIndirizzi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef piva As String,
                                                    ByRef sa_cod As Integer,
                                                    ByRef cod_Indirizzo As Integer,
                                                    ByRef tipo_indirizzo As Integer,
                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.CentrixIndirizzi
        Dim centro As New AgronicaCoreEntityFramework_POCO.CentrixIndirizzi
        centro.PIVA = piva
        centro.sa_cod = sa_cod
        centro.cod_indirizzo = cod_Indirizzo
        centro.Tipo_Indirizzo = tipo_indirizzo
        centro.inviato = 0
        centro.Data_Creazione = DateTime.Now
        centro.Data_Modifica = DateTime.Now
        centro.Validita_Inizio = AGRODATAINIZIO
        centro.Validita_Fine = AGRODATAFINE
        centro.Username_Creazione = username
        centro.Username_Modifica = username
        centro.Validazione = 0
        centro.Data_Validazione = DateTime.Now
        centro.UserName_Validazione = ""
        dal.CentrixIndirizzi.Add(centro)
        dal.SaveChanges()
        Return centro
    End Function

    Public Shared Function CreateLiquiditaCentri_Aziendali(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef Centro_Aziendale As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                           ByRef riferimento As String,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.Liquidita
        Dim liq = EFLiquidita.CreateLiquidita(dal, objParametri, Centro_Aziendale.PIVA, Centro_Aziendale.sa_cod, riferimento, username)
        Return liq
    End Function

    'cancellare?
    'Public Shared Function CreateUtentixStrutture(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
    '                                                ByRef objParametri As AgronicaCoreParametri,
    '                                               ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
    '                                               ByRef user As String,
    '                                               ByRef piva As String,
    '                                               ByRef sa_cod As Integer,
    '                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.UtentiXStrutture
    '    If user Is Nothing Then
    '        user = ""
    '    End If
    '    If piva Is Nothing Then
    '        piva = ""
    '    End If
    '    'Dim UtentixStrutture = UtentixStrutture.
    '    Dim rubrica = EFRubrica.CreateRubricaEF(dal, objParametri, numero, descr, username)
    '    Dim cxi = CreateRubricaxCentri(dal, objParametri, centro, rubrica, username)
    '    Return rubrica
    'End Function

    Public Shared Function CreateUtentixStrutture(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                           ByRef objParametri As AgronicaCoreParametri,
                                           ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                           ByRef username As String) As AgronicaCoreEntityFramework_POCO.UtentiXStrutture

        Dim uxs As New AgronicaCoreEntityFramework_POCO.UtentiXStrutture

        uxs.USER = objParametri.PivaSuperUser
        uxs.PIVA = centro.PIVA
        uxs.SA_COD = centro.sa_cod

        uxs.inviato = 0
        uxs.datainvio = DateTime.Now

        uxs.Data_Creazione = DateTime.Now
        uxs.Data_Modifica = DateTime.Now

        uxs.Username_Creazione = username
        uxs.Username_Modifica = username

        uxs.Validita_Inizio = AGRODATAINIZIO
        uxs.Validita_Fine = AGRODATAFINE

        uxs.Validazione = 0
        uxs.Data_Validazione = DateTime.Now
        uxs.UserName_Validazione = ""

        dal.UtentiXStrutture.Add(uxs)
        dal.SaveChanges()
        Return uxs
    End Function

    Public Shared Function CentroExist(ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                       ByVal PartitaIva As String,
                                       ByVal CentroAziendale As Integer) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CentroExist()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            'Lavez - 29/05/2025 - refactoring
            'Dim centriList = From centro In GiasContext.Centri_Aziendali
            '                 Where centro.PIVA = PartitaIva AndAlso
            '                     centro.sa_cod = CentroAziendale
            '                 Select centro

            'Dim centr = centriList.FirstOrDefault
            'If (centr IsNot Nothing) Then
            '    ret = True
            'End If
            Dim Exists = (From centro In GiasContext.Centri_Aziendali
                          Where centro.PIVA = PartitaIva AndAlso
                              centro.sa_cod = CentroAziendale
                          Select 1).Any()
            If Exists Then
                ret = True
            End If
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret

    End Function
End Class
