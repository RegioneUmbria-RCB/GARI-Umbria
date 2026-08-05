Imports System.Data.Entity
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class EFParticelle
    Public Shared Function CreateParticelleCatastaliEF(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef prov As String,
                                                   ByRef com As String,
                                                   ByRef sezione As String,
                                                   ByRef foglio As Integer,
                                                   ByRef numero As Integer,
                                                   ByRef subalterno As String,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastali
        Dim particellaCatastale As New AgronicaCoreEntityFramework_POCO.ParticelleCatastali
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim part_cod = idGen.NuovoId_Tabella_EF(dal, "ParticelleCatastali", 0, 20000000, objParametri)
        particellaCatastale.PART_COD = part_cod
        particellaCatastale.PROV = prov
        particellaCatastale.COM = com
        particellaCatastale.SEZIONE = sezione
        particellaCatastale.FOGLIO = foglio
        particellaCatastale.NUMERO = numero
        particellaCatastale.SUBALTERNO = subalterno
        particellaCatastale.PARTITA_CATASTALE = ""
        particellaCatastale.ETTARI = 0
        particellaCatastale.ARE = 0
        particellaCatastale.CENTIARE = 0
        particellaCatastale.TitoloPossesso = 0
        particellaCatastale.QUALITA_COD = 0
        particellaCatastale.CLASSE = ""
        particellaCatastale.REDDITO_AGRARIO = 0
        particellaCatastale.REDDITO_DOMINICALE = 0
        particellaCatastale.inviato = 0
        particellaCatastale.Data_Creazione = DateTime.Now
        particellaCatastale.Data_Modifica = DateTime.Now
        particellaCatastale.Validita_Inizio = AGRODATAINIZIO
        particellaCatastale.Validita_Fine = AGRODATAFINE
        particellaCatastale.Username_Creazione = username
        particellaCatastale.Username_Modifica = username
        particellaCatastale.Validazione = 0
        particellaCatastale.Data_Validazione = DateTime.Now
        particellaCatastale.UserName_Validazione = ""
        particellaCatastale.casiParticolari = ""
        particellaCatastale.fasciaAltimetrica = ""
        particellaCatastale.fasciaAltimetricaDescr = ""
        particellaCatastale.Fonte = ""
        particellaCatastale.FonteDescr = ""
        particellaCatastale.tipoDocumento = ""
        particellaCatastale.tipoDocumentoDescr = ""
        particellaCatastale.utilizzo = ""
        particellaCatastale.IDParticellaOrig = ""
        dal.ParticelleCatastali.Add(particellaCatastale)
        'dal.ObjectStateManager.ChangeObjectState(particellaCatastale, EntityState.Added)
        dal.Entry(particellaCatastale).State = EntityState.Added
        Return particellaCatastale
    End Function

    Public Shared Function CreaParticelleCatastalixMacrousi(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                               ByRef macrouso_Cod As String,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastalixMacrousi
        Dim macrouso = EFMacrousi.CreateParticelleCatastalixMacrousi(dal, objParametri, particella.PROV, particella.COM, particella.SEZIONE, particella.FOGLIO, particella.NUMERO, particella.SUBALTERNO, macrouso_Cod, username)
        dal.SaveChanges()
        Return macrouso
    End Function

    Public Shared Function CreaZonexParticelle(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                               ByRef zona_Cod As Integer,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ZonexParticelle

        Dim zona = EFZone.CreateZonexParticelle(dal,
                                                objParametri,
                                                particella.PROV,
                                                particella.COM,
                                                particella.SEZIONE,
                                                particella.FOGLIO,
                                                particella.NUMERO,
                                                particella.SUBALTERNO,
                                                zona_Cod,
                                                username)
        dal.SaveChanges()
        Return zona

    End Function


    Public Shared Function CreaParticelleCatastaliClassamento(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                               ByRef qualita_cod As Integer,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastaliClassamento
        Dim classamento = EFClassamento.CreateParticelleCatastaliClassamento(dal,
                                                objParametri,
                                                particella.PROV,
                                                particella.COM,
                                                particella.SEZIONE,
                                                particella.FOGLIO,
                                                particella.NUMERO,
                                                particella.SUBALTERNO,
                                                qualita_cod,
                                                username)
        dal.SaveChanges()
        Return classamento
    End Function


    Public Shared Function CreateParticelleCatastali_MetodoProduzione(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef prov As String,
                                               ByRef com As String,
                                               ByRef sezione As String,
                                               ByRef foglio As String,
                                               ByRef numero As String,
                                               ByRef subalterno As String,
                                               ByRef MetodoProduzione_Cod As String,
                                               ByRef Validita_Inizio As Date,
                                               ByRef Validita_Fine As Date,
                                               ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastali_MetodoProduzione

        If sezione = "" Then
            sezione = "0"
        End If

        If subalterno = "" Then
            subalterno = "0"
        End If
        Dim metodoProduzione As New AgronicaCoreEntityFramework_POCO.ParticelleCatastali_MetodoProduzione
        metodoProduzione.MetodoProduzione_Cod = MetodoProduzione_Cod
        metodoProduzione.PROV = prov
        metodoProduzione.COM = com
        metodoProduzione.SEZIONE = sezione
        metodoProduzione.FOGLIO = foglio
        metodoProduzione.NUMERO = numero
        metodoProduzione.SUBALTERNO = subalterno
        metodoProduzione.inviato = 0
        metodoProduzione.Data_Creazione = DateTime.Now
        metodoProduzione.Data_Modifica = DateTime.Now
        metodoProduzione.Validita_Inizio = Validita_Inizio
        metodoProduzione.Validita_Fine = Validita_Fine
        metodoProduzione.Username_Creazione = username
        metodoProduzione.Username_Modifica = username
        dal.ParticelleCatastali_MetodoProduzione.Add(metodoProduzione)
        dal.SaveChanges()
        Return metodoProduzione
    End Function

    Public Shared Function ParticelleCatastalixEleggibilitaParticelle(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                   ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                                                   ByRef eleggibilitaCod As String,
                                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.ParticelleCatastalixEleggibilitaParticelle
        Dim eleggibilita As New AgronicaCoreEntityFramework_POCO.ParticelleCatastalixEleggibilitaParticelle
        eleggibilita.PROV = particella.PROV
        eleggibilita.COM = particella.COM
        eleggibilita.SEZIONE = particella.SEZIONE
        eleggibilita.FOGLIO = particella.FOGLIO
        eleggibilita.NUMERO = particella.NUMERO
        eleggibilita.SUBALTERNO = particella.SUBALTERNO
        eleggibilita.Eleggibilita_Cod = eleggibilitaCod
        eleggibilita.Data_Creazione = DateTime.Now
        eleggibilita.Data_Modifica = DateTime.Now
        eleggibilita.Validita_Inizio = AGRODATAINIZIO
        eleggibilita.Validita_Fine = AGRODATAFINE
        eleggibilita.Username_Creazione = username
        eleggibilita.Username_Modifica = username
        dal.ParticelleCatastalixEleggibilitaParticelle.Add(eleggibilita)
        dal.SaveChanges()
        Return eleggibilita
    End Function

    Public Shared Function CreateImpresexParticelle_Contatti(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                   ByRef centro As AgronicaCoreEntityFramework_POCO.Centri_Aziendali,
                                                                   ByRef particella As AgronicaCoreEntityFramework_POCO.ParticelleCatastali,
                                                                   ByRef risorsaUmana As AgronicaCoreEntityFramework_POCO.Risorse_Umane,
                                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.ImpresexParticelle_Contatti
        Dim pxc As New AgronicaCoreEntityFramework_POCO.ImpresexParticelle_Contatti
        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        pxc.ID = idGen.NuovoId_Tabella_EF(dal, "ImpresexParticelle_Contatti", 0, 2000000, objParametri)
        pxc.Tipo_Contatto = 0
        pxc.Piva = centro.PIVA
        pxc.Sa_Cod = centro.sa_cod
        pxc.Cod_RisUm = risorsaUmana.Cod_RisUm
        pxc.PROV = particella.PROV
        pxc.COM = particella.COM
        pxc.SEZIONE = particella.SEZIONE
        pxc.FOGLIO = particella.FOGLIO
        pxc.NUMERO = particella.NUMERO
        pxc.SUBALTERNO = particella.SUBALTERNO
        pxc.Quota = 0
        pxc.inviato = 0
        pxc.Data_Creazione = DateTime.Now
        pxc.Data_Modifica = DateTime.Now
        pxc.Validita_Inizio = AGRODATAINIZIO
        pxc.Validita_Fine = AGRODATAFINE
        pxc.Username_Creazione = username
        pxc.Username_Modifica = username
        pxc.cuaaProprietario = ""
        dal.ImpresexParticelle_Contatti.Add(pxc)
        dal.SaveChanges()
        Return pxc
    End Function

    Public Shared Function CreateImpresexParticelle(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                   Piva As String,
                                                                   Sa_Cod As Integer,
                                                                   Prov As String,
                                                                   Com As String,
                                                                   Sezione As String,
                                                                   Foglio As Integer,
                                                                   Numero As Integer,
                                                                   Subalterno As String,
                                                                   ByRef username As String) As AgronicaCoreEntityFramework_POCO.ImpreseXParticelle
        Dim ixp As New AgronicaCoreEntityFramework_POCO.ImpreseXParticelle
        ixp.PIVA = Piva
        ixp.sa_cod = Sa_Cod
        ixp.PROV = Prov
        ixp.COM = Com
        ixp.SEZIONE = Sezione
        ixp.FOGLIO = Foglio
        ixp.NUMERO = Numero
        ixp.SUBALTERNO = Subalterno
        ixp.TitoloPossesso = 0
        ixp.PARTITA_CATASTALE = ""
        ixp.inviato = 0
        ixp.Data_Creazione = DateTime.Now
        ixp.Data_Modifica = DateTime.Now
        ixp.Validita_Inizio = AGRODATAINIZIO
        ixp.Validita_Fine = AGRODATAFINE
        ixp.Username_Creazione = username
        ixp.Username_Modifica = username

        ixp.Validazione = 0
        ixp.Data_Validazione = AGRODATAINIZIO
        ixp.UserName_Validazione = ""
        ixp.Sup_Condotta = 0
        ixp.Sup_Divieto = 0
        ixp.Sup_Spandibile = 0
        ixp.Irrigabilita = ""
        ixp.RotazioneColturale = ""
        ixp.biologico = ""
        ixp.flagAnomaliaMacrouso = ""
        ixp.flagContenzioso = ""
        ixp.flagSupero = ""
        ixp.Contratto_Cod = 0

        dal.ImpreseXParticelle.Add(ixp)
        dal.SaveChanges()
        Return ixp
    End Function

End Class
