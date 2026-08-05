Imports System.Data.Entity
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.LogProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports Newtonsoft.Json

Public Class EFMacchine
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Const VisibilitaPubblica = -1
    Public Const VisibilitaPrivata = 0

    Private Shared Function GenerateNewMacCod_EF(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
                                                ) As Int32

        Dim gefutils As New Gias_EF_Utility
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim bCloseContext As Boolean = False


        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        Dim MessaggioErrore As String = String.Empty
        Dim idSeq As Integer = Nothing

        Try
            idSeq = ObjSequenze.NuovoId_Tabella_EF(
                GiasContext,
                "Parco_Macchine",
                0,
                2000000000,
                objParametri
                )

        Catch ex As Exception
            idSeq = -1
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return idSeq

    End Function

    Private Shared Function generateNewMacCod(ByVal objParametri As AgronicaCoreParametri) As Int32
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim MessaggioErrore As String = String.Empty
        Dim idSeq As Integer = Nothing

        Try
            idSeq = ObjSequenze.NuovoId_Tabella(
                "Parco_Macchine",
                0,
                2000000000,
                objParametri
                )

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception(MessaggioErrore)
        End Try

        Return idSeq
    End Function

    Public Shared Function CreateParco_Macchine(ByRef piva As String,
                                                ByVal Mac_Cod As Integer,
                                                ByRef username As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                               ) As AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim parco_macchine As New AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim codice = 0
        If Mac_Cod = 0 OrElse Mac_Cod = Nothing Then
            'lavez - 24/01/2024 per stack contatore thread safe
            'codice = GenerateNewMacCod_EF(objParametri)
            codice = generateNewMacCod(objParametri)
        Else
            codice = Mac_Cod
        End If

        parco_macchine.Mac_Cod = codice

        parco_macchine.Piva = piva
        parco_macchine.Sa_Cod = 0
        parco_macchine.Mac_Des = ""
        parco_macchine.Costo_Acquisto = 0
        parco_macchine.Targa = ""
        parco_macchine.Telaio = ""
        parco_macchine.Ditta_Cod = 0
        parco_macchine.Modello = ""
        parco_macchine.Potenza = ""
        parco_macchine.Ammortamento = 0
        parco_macchine.Ammortizzato = 0
        parco_macchine.Data_Immatricolazione = AGRODATAINIZIO
        parco_macchine.Ultima_Manutenzione = AGRODATAINIZIO
        parco_macchine.Ultima_Revisione = AGRODATAINIZIO
        parco_macchine.Stato_Utilizzo = ""
        parco_macchine.inviato = 0

        'lavez - 22/06/2023 - va lasciata null
        'parco_macchine.datainvio = DateTime.Now
        parco_macchine.data_creazione = DateTime.Now
        parco_macchine.data_modifica = DateTime.Now
        parco_macchine.username_creazione = username
        parco_macchine.username_modifica = username
        parco_macchine.validita_inizio = AGRODATAINIZIO
        parco_macchine.validita_fine = AGRODATAFINE
        parco_macchine.Note = ""
        parco_macchine.Tipo = 0
        parco_macchine.N_Immatricolazione = ""
        parco_macchine.N_Immatricolazione_Rimorchio = ""
        parco_macchine.N_Autorizzazione_Trasporto = ""
        parco_macchine.Data_Rilascio_Autorizzazione = AGRODATAINIZIO

        parco_macchine.Peso = 0
        parco_macchine.Mac_Cod_Origine = 0
        parco_macchine.Piva_SuperUser_Origine = ""
        parco_macchine.ChkDefault = 0
        parco_macchine.Portata_Max = 0
        parco_macchine.Cod_Contatto = ""
        parco_macchine.CUAA_Proprietario = ""
        parco_macchine.Denominazione_Proprietario = ""
        parco_macchine.Alimentazione_Cod = 0
        parco_macchine.Potenza_Udm_Cod = 0

        parco_macchine.Tipo_Targa_Cod = 0
        parco_macchine.Tipo_Trazione_Cod = 0
        parco_macchine.N_Omologazione = ""
        parco_macchine.Ditta_Cod_Motore = 0
        parco_macchine.Tipo_Motore = ""
        parco_macchine.Matricola_Motore = ""
        parco_macchine.Data_Reimmatricolazione = AGRODATAINIZIO
        parco_macchine.Data_Carico = DateTime.Now
        parco_macchine.Data_Scarico = DateTime.Now
        parco_macchine.TitoloPossesso = 0
        parco_macchine.Flag_Attrezzatura_Macchina = ""
        parco_macchine.Taratura_Ugello = 0
        parco_macchine.Codice = ""
        parco_macchine.Visibile_ctrl_gestione = 0
        parco_macchine.Img_Large = New Byte(0) {}
        parco_macchine.Img_Large_Extension = ""
        parco_macchine.Img_Large_FileName = ""
        parco_macchine.Img_Thumbnail = New Byte(0) {}
        parco_macchine.Img_Thumbnail_Extension = ""
        parco_macchine.Img_Thumbnail_FileName = ""

        parco_macchine.Validita_Taratura_Inizio = AGRODATAINIZIO
        parco_macchine.Validita_Taratura_Fine = AGRODATAFINE

        parco_macchine.Distinta_Installazione = ""
        parco_macchine.Contratto_Installazione = ""
        parco_macchine.Tipologia_Installazione = ""
        parco_macchine.Data_Inizio_Installazione = AGRODATAINIZIO
        parco_macchine.Data_Fine_Installazione = AGRODATAFINE
        parco_macchine.Stato_Installazione = ""
        parco_macchine.Provincia_Istat_Installazione = ""
        parco_macchine.Comune_Istat_Installazione = ""
        parco_macchine.Indirizzo_Installazione = ""
        parco_macchine.Latitudine_Installazione = 0.00000
        parco_macchine.Longitudine_Installazione = 0.00000

        Return parco_macchine

    End Function

    Public Shared Function Macchina_Scrivi_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                              ByRef objParametriServer As AgronicaCoreParametri,
                                              ByVal username As String,
                                              Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal NewTransaction As Boolean = True
                                              ) As Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.Macchina_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Dim macchina = CreateParco_Macchine(
            DatiMacchina.partitaIva,
            DatiMacchina.codice,
            username,
            objParametriServer
            )

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            macchina.Sa_Cod = If(DatiMacchina.visibilitaPubblica, VisibilitaPubblica, If(IsNothing(DatiMacchina.centroPK) OrElse DatiMacchina.centroPK.codice = 0, VisibilitaPrivata, DatiMacchina.centroPK.codice))
            macchina.Class_Code = (If(DatiMacchina.tipo.codice = "", "", DatiMacchina.tipo.codice) &
                If(DatiMacchina.dettaglio_1.codice = "", "", "." & DatiMacchina.dettaglio_1.codice) &
                If(DatiMacchina.dettaglio_2.codice = "", "", "." & DatiMacchina.dettaglio_2.codice))
            macchina.Mac_Des = DatiMacchina.descrizione
            macchina.Costo_Acquisto = 0
            macchina.Targa = DatiMacchina.targa
            macchina.Telaio = DatiMacchina.telaio
            macchina.Ditta_Cod = DatiMacchina.marca.codice
            macchina.Modello = DatiMacchina.modello
            macchina.Potenza = DatiMacchina.potenza
            macchina.Ammortamento = 0
            macchina.Ammortizzato = 0
            macchina.Data_Immatricolazione = DatiMacchina.data_Immatricolazione
            macchina.Stato_Utilizzo = DatiMacchina.stato_Utilizzo
            macchina.inviato = 0
            macchina.Note = DatiMacchina.note
            macchina.Tipo = DatiMacchina.finalita.codice
            macchina.N_Immatricolazione = DatiMacchina.n_Immatricolazione
            macchina.N_Immatricolazione_Rimorchio = DatiMacchina.n_Immatricolazione_Rimorchio
            macchina.N_Autorizzazione_Trasporto = DatiMacchina.N_Autorizzazione_Trasporto
            macchina.Data_Rilascio_Autorizzazione = DatiMacchina.data_Rilascio_Autorizzazione
            macchina.CUAA_Proprietario = DatiMacchina.CUAA_Proprietario
            macchina.Denominazione_Proprietario = DatiMacchina.proprietario
            macchina.Alimentazione_Cod = DatiMacchina.alimentazione.codice
            macchina.Potenza_Udm_Cod = DatiMacchina.unita_Misura.codice
            macchina.Tipo_Targa_Cod = DatiMacchina.tipo_Targa.codice
            macchina.Data_Carico = DatiMacchina.Data_Carico
            macchina.Data_Scarico = DatiMacchina.Data_Scarico
            macchina.TitoloPossesso = DatiMacchina.titolo_Possesso.codice
            macchina.Taratura_Ugello = DatiMacchina.taratura_Ugello
            macchina.Codice = DatiMacchina.codice_stringa
            macchina.validita_inizio = DatiMacchina.validita.inizio
            macchina.validita_fine = DatiMacchina.validita.fine

            macchina.Ultima_Revisione = DatiMacchina.data_Ultima_Revisione
            macchina.Ultima_Manutenzione = DatiMacchina.data_Ultima_Manutenzione

            macchina.Visibile_ctrl_gestione = If(DatiMacchina.visibileControlloGestione, 1, 0)
            If (DatiMacchina.immagineGrande IsNot Nothing) Then
                macchina.Img_Large = Convert.FromBase64String(DatiMacchina.immagineGrande.immagine)
                macchina.Img_Large_Extension = DatiMacchina.immagineGrande.estensione
                macchina.Img_Large_FileName = DatiMacchina.immagineGrande.nome
            Else
                macchina.Img_Large = Nothing
                macchina.Img_Large = Nothing
                macchina.Img_Large = Nothing
            End If
            If (DatiMacchina.immaginePiccola IsNot Nothing) Then
                macchina.Img_Thumbnail = Convert.FromBase64String(DatiMacchina.immaginePiccola.immagine)
                macchina.Img_Thumbnail_Extension = DatiMacchina.immaginePiccola.estensione
                macchina.Img_Thumbnail_FileName = DatiMacchina.immaginePiccola.nome
            Else
                macchina.Img_Thumbnail = Nothing
                macchina.Img_Thumbnail_Extension = Nothing
                macchina.Img_Thumbnail_FileName = Nothing
            End If

            If DatiMacchina.ageaCod IsNot Nothing Then
                macchina.Agea_Cod = DatiMacchina.ageaCod.codice
            End If

            macchina.Portata = DatiMacchina.portata
            macchina.Efficienza = DatiMacchina.efficienza
            macchina.IMP_COD = DatiMacchina.codice_impianto

            If DatiMacchina.contatto IsNot Nothing Then
                macchina.Cod_Contatto = DatiMacchina.contatto.primaryKey.codice
            End If

            macchina.VIN = DatiMacchina.VIN
            macchina.BTM_Serial = DatiMacchina.BTM_Serial
            macchina.ExternalAPIKey = DatiMacchina.ExternalAPIKey


            macchina.HubIoT_PlatformDestination = If(IsNothing(DatiMacchina.HubIoT_PlatformDestination), macchina.HubIoT_PlatformDestination, CShort(DatiMacchina.HubIoT_PlatformDestination.codice))

            macchina.Validita_Taratura_Inizio = DatiMacchina.data_Ultima_Taratura
            macchina.Validita_Taratura_Fine = DatiMacchina.scadenza_Taratura

            '----------------------------------------------------------------
            ' Scrittura parti Stazione Meteo
            '----------------------------------------------------------------
            macchina.Distinta_Installazione = DatiMacchina.Distinta_Installazione
            macchina.Contratto_Installazione = DatiMacchina.Contratto_Installazione
            macchina.Tipologia_Installazione = DatiMacchina.Tipologia_Installazione
            macchina.Data_Inizio_Installazione = DatiMacchina.Data_Inizio_Installazione
            macchina.Data_Fine_Installazione = DatiMacchina.Data_Fine_Installazione
            macchina.Stato_Installazione = DatiMacchina.Stato_Installazione
            macchina.Provincia_Istat_Installazione = DatiMacchina.Provincia_Istat_Installazione
            macchina.Comune_Istat_Installazione = DatiMacchina.Comune_Istat_Installazione
            macchina.Indirizzo_Installazione = DatiMacchina.Indirizzo_Installazione
            macchina.Latitudine_Installazione = DatiMacchina.Latitudine_Installazione
            macchina.Longitudine_Installazione = DatiMacchina.Longitudine_Installazione

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura dei Costi della Macchina
            '----------------------------------------------------------------
            If DatiMacchina.costi IsNot Nothing Then
                dal.gestisciCosti(DatiMacchina, objParametriServer, username, GiasContext, NewTransaction)
            End If
            '----------------------------------------------------------------

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura delle Caratteristiche della Macchina
            '----------------------------------------------------------------
            If DatiMacchina.caratteristiche IsNot Nothing Then
                dal.gestisciCaratteristiche(DatiMacchina, objParametriServer, username, macchina.Mac_Cod, GiasContext, NewTransaction)
            End If
            '----------------------------------------------------------------

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura della Gerarchia della Macchina
            '----------------------------------------------------------------
            If DatiMacchina.gerarchiaFigli IsNot Nothing Then
                dal.gestisciGerarchia(DatiMacchina, objParametriServer, username, macchina.Mac_Cod, GiasContext, NewTransaction)
            End If
            '----------------------------------------------------------------

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura dei codici della Macchina
            '----------------------------------------------------------------
            Dim pcmObj As New EFParcoMacchineCodici
            DatiMacchina.codice = macchina.Mac_Cod
            Dim success = pcmObj.Write(DatiMacchina, objParametriServer, GiasContext, NewTransaction)

            If Not success Then
                Throw New Exception("Errore durante il salvataggio dei Parco_macchine_Codici")
            End If
            '----------------------------------------------------------------

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura dei ratei tempo
            '----------------------------------------------------------------
            'If DatiMacchina.rateiTempo IsNot Nothing Then
            '    dal.gestisciRateiTempo(DatiMacchina, objParametriServer)
            'End If
            '----------------------------------------------------------------

            GiasContext.Parco_Macchine.Add(macchina)
            GiasContext.SaveChanges()

            Dim DatiMacchinaStr = ""
            If DatiMacchina IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                DatiMacchinaStr = JsonConvert.SerializeObject(DatiMacchina, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                enum_TipoEntita_Des.ParcoMacchine,
                CStr(macchina.Piva),
                CStr(macchina.Sa_Cod),
                CStr(macchina.Mac_Cod), Nothing,
                Nothing, Nothing,
                enum_TipoOperazioneDB.Scrittura,
                objParametriServer,
                enum_Id_Servizio.GiasOnline,
                "",
                DatiMacchinaStr
                )

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            macchina = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            macchina = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return macchina
    End Function

    Public Sub impostaDefaultMacchina(ByRef DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

        If DatiMacchina.partitaIva Is Nothing Then

        End If

        If DatiMacchina.marca Is Nothing Then
            DatiMacchina.marca = New metaschema.DittaMacchina(0)
        End If

        If DatiMacchina.descrizione Is Nothing Then
            DatiMacchina.descrizione = ""
        End If

        If DatiMacchina.modello Is Nothing Then
            DatiMacchina.modello = ""
        End If

        If DatiMacchina.finalita Is Nothing Then
            DatiMacchina.finalita = New metaschema.FinalitaMacchina(0)
        End If

        If DatiMacchina.tipo Is Nothing Then
            DatiMacchina.tipo = New metaschema.Macchine(0)
        End If

        If DatiMacchina.dettaglio_1 Is Nothing Then
            DatiMacchina.dettaglio_1 = New metaschema.MacchineDettaglio1(0)
        End If

        If DatiMacchina.dettaglio_2 Is Nothing Then
            DatiMacchina.dettaglio_2 = New metaschema.MacchineDettaglio2(0)
        End If

        If DatiMacchina.codice_stringa Is Nothing Then
            DatiMacchina.codice_stringa = ""
        End If

        If DatiMacchina.validita Is Nothing Then
            DatiMacchina.validita = New IntervalloTemporale(AGRODATAINIZIO, AGRODATAFINE)
        End If

        If DatiMacchina.Data_Carico < AGRODATAINIZIO Then
            DatiMacchina.Data_Carico = AGRODATAINIZIO
        End If

        If DatiMacchina.Data_Scarico < AGRODATAINIZIO Then
            DatiMacchina.Data_Scarico = AGRODATAFINE
        End If

        If DatiMacchina.titolo_Possesso Is Nothing Then
            DatiMacchina.titolo_Possesso = New metaschema.TitoloDiPossesso(0)
        End If

        If DatiMacchina.proprietario Is Nothing Then
            DatiMacchina.proprietario = ""
        End If

        If DatiMacchina.CUAA_Proprietario Is Nothing Then
            DatiMacchina.CUAA_Proprietario = ""
        End If

        If DatiMacchina.targa Is Nothing Then
            DatiMacchina.targa = ""
        End If

        If DatiMacchina.tipo_Targa Is Nothing Then
            DatiMacchina.tipo_Targa = New metaschema.TipoTarga(0)
        End If

        If DatiMacchina.telaio Is Nothing Then
            DatiMacchina.telaio = ""
        End If

        If DatiMacchina.n_Immatricolazione Is Nothing Then
            DatiMacchina.n_Immatricolazione = ""
        End If

        If DatiMacchina.data_Immatricolazione < AGRODATAINIZIO Then
            DatiMacchina.data_Immatricolazione = AGRODATAINIZIO
        End If

        If DatiMacchina.n_Immatricolazione_Rimorchio Is Nothing Then
            DatiMacchina.n_Immatricolazione_Rimorchio = ""
        End If

        If DatiMacchina.N_Autorizzazione_Trasporto Is Nothing Then
            DatiMacchina.N_Autorizzazione_Trasporto = ""
        End If

        If DatiMacchina.data_Rilascio_Autorizzazione < AGRODATAINIZIO Then
            DatiMacchina.data_Rilascio_Autorizzazione = AGRODATAINIZIO
        End If

        If DatiMacchina.alimentazione Is Nothing Then
            DatiMacchina.alimentazione = New metaschema.Carburante(0)
        End If

        If DatiMacchina.data_Ultima_Taratura < AGRODATAINIZIO Then
            DatiMacchina.data_Ultima_Taratura = AGRODATAINIZIO
        End If

        If DatiMacchina.scadenza_Taratura < AGRODATAINIZIO Then
            DatiMacchina.scadenza_Taratura = AGRODATAFINE
        End If

        If DatiMacchina.stato_Utilizzo Is Nothing Then
            DatiMacchina.stato_Utilizzo = ""
        End If

        If DatiMacchina.potenza Is Nothing Then
            DatiMacchina.potenza = ""
        End If

        If DatiMacchina.unita_Misura Is Nothing Then
            DatiMacchina.unita_Misura = New metaschema.UnitaDiMisura(0)
        End If

        If DatiMacchina.note Is Nothing Then
            DatiMacchina.note = ""
        End If

    End Sub

    Public Shared Function Macchina_Modifica_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal username As String,
                                                    ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal NewTransaction As Boolean = True
                                                    ) As Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.Macchina_Modifica_EF"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim mac = (From macchina In GiasContext.Parco_Macchine
                   Where macchina.Piva = DatiMacchina.partitaIva AndAlso
                       macchina.Mac_Cod = DatiMacchina.codice).FirstOrDefault()

        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Try
            'Essendo il sa_cod parte della chiave, cancello l'intera riga e la riscrivo
            If (Not IsNothing(DatiMacchina.centroPK) AndAlso DatiMacchina.centroPK.codice <> mac.Sa_Cod) OrElse
                (CInt(DatiMacchina.visibilitaPubblica) <> mac.Sa_Cod AndAlso (mac.Sa_Cod = 0 OrElse mac.Sa_Cod = -1)) Then

                GiasContext.Parco_Macchine.Remove(mac)
                GiasContext.SaveChanges()

                dal.Macchina_Scrivi_EF(DatiMacchina, objParametriServer, objParametriServer.UsernameOperazione)
            Else

                mac.Class_Code = (DatiMacchina.tipo.codice &
                If(DatiMacchina.dettaglio_1.codice = "", "", "." & DatiMacchina.dettaglio_1.codice) &
                If(DatiMacchina.dettaglio_2.codice = "", "", "." & DatiMacchina.dettaglio_2.codice))
                mac.Mac_Des = If(DatiMacchina.descrizione Is Nothing, mac.Mac_Des, DatiMacchina.descrizione)
                'mac.Costo_Acquisto = 0 non viene gestito dall'interfaccia angular, perciò non lo tocco
                mac.Targa = If(DatiMacchina.targa Is Nothing, mac.Targa, DatiMacchina.targa)
                mac.Telaio = If(DatiMacchina.telaio Is Nothing, mac.Telaio, DatiMacchina.telaio)
                mac.Ditta_Cod = If(DatiMacchina.marca Is Nothing, mac.Ditta_Cod, DatiMacchina.marca.codice)
                mac.Modello = If(DatiMacchina.modello Is Nothing, mac.Modello, DatiMacchina.modello)
                mac.Potenza = If(DatiMacchina.potenza Is Nothing, mac.Potenza, DatiMacchina.potenza)
                mac.Ammortamento = 0
                mac.Ammortizzato = 0
                mac.Data_Immatricolazione = If(DatiMacchina.data_Immatricolazione = Nothing, mac.Data_Immatricolazione, DatiMacchina.data_Immatricolazione)
                mac.Stato_Utilizzo = If(DatiMacchina.stato_Utilizzo Is Nothing, mac.Stato_Utilizzo, DatiMacchina.stato_Utilizzo)
                mac.inviato = 0
                mac.Note = If(DatiMacchina.note Is Nothing, mac.Note, DatiMacchina.note)
                mac.Tipo = If(DatiMacchina.finalita Is Nothing, mac.Tipo, DatiMacchina.finalita.codice)
                mac.N_Immatricolazione = If(DatiMacchina.n_Immatricolazione Is Nothing, mac.N_Immatricolazione, DatiMacchina.n_Immatricolazione)
                mac.N_Immatricolazione_Rimorchio = If(DatiMacchina.n_Immatricolazione_Rimorchio Is Nothing, mac.N_Immatricolazione_Rimorchio, DatiMacchina.n_Immatricolazione_Rimorchio)
                mac.N_Autorizzazione_Trasporto = If(DatiMacchina.N_Autorizzazione_Trasporto Is Nothing, mac.N_Autorizzazione_Trasporto, DatiMacchina.N_Autorizzazione_Trasporto)
                mac.Data_Rilascio_Autorizzazione = If(DatiMacchina.data_Rilascio_Autorizzazione = Nothing, mac.Data_Rilascio_Autorizzazione, DatiMacchina.data_Rilascio_Autorizzazione)
                mac.CUAA_Proprietario = If(DatiMacchina.CUAA_Proprietario Is Nothing, mac.CUAA_Proprietario, DatiMacchina.CUAA_Proprietario)
                mac.Denominazione_Proprietario = If(DatiMacchina.proprietario Is Nothing, mac.Denominazione_Proprietario, DatiMacchina.proprietario)
                mac.Alimentazione_Cod = If(DatiMacchina.alimentazione Is Nothing, mac.Alimentazione_Cod, DatiMacchina.alimentazione.codice)
                mac.Potenza_Udm_Cod = If(DatiMacchina.unita_Misura Is Nothing, mac.Potenza_Udm_Cod, DatiMacchina.unita_Misura.codice)
                mac.Tipo_Targa_Cod = If(DatiMacchina.tipo_Targa Is Nothing, mac.Tipo_Targa_Cod, DatiMacchina.tipo_Targa.codice)
                mac.Data_Carico = If(DatiMacchina.Data_Carico = Nothing, mac.Data_Carico, DatiMacchina.Data_Carico)
                mac.Data_Scarico = If(DatiMacchina.Data_Scarico = Nothing, mac.Data_Scarico, DatiMacchina.Data_Scarico)
                mac.TitoloPossesso = If(DatiMacchina.titolo_Possesso Is Nothing, mac.TitoloPossesso, DatiMacchina.titolo_Possesso.codice)
                mac.Taratura_Ugello = If(DatiMacchina.taratura_Ugello = Nothing, mac.Taratura_Ugello, DatiMacchina.taratura_Ugello)
                mac.Codice = If(DatiMacchina.codice_stringa = Nothing, mac.Codice, DatiMacchina.codice_stringa)

                mac.validita_inizio = If(DatiMacchina.validita.inizio = Nothing, mac.validita_inizio, DatiMacchina.validita.inizio)
                mac.validita_fine = If(DatiMacchina.validita.fine = Nothing, mac.validita_fine, DatiMacchina.validita.fine)
                If (Not IsNothing(DatiMacchina.visibileControlloGestione)) Then
                    mac.Visibile_ctrl_gestione = If(DatiMacchina.visibileControlloGestione, 1, 0)
                End If

                If (DatiMacchina.immagineGrande IsNot Nothing) Then
                    mac.Img_Large = If(DatiMacchina.immagineGrande.immagine Is Nothing, mac.Img_Large, Convert.FromBase64String(DatiMacchina.immagineGrande.immagine))
                    mac.Img_Large_Extension = If(DatiMacchina.immagineGrande.estensione Is Nothing, mac.Img_Large_Extension, DatiMacchina.immagineGrande.estensione)
                    mac.Img_Large_FileName = If(DatiMacchina.immagineGrande.nome Is Nothing, mac.Img_Large_FileName, DatiMacchina.immagineGrande.nome)
                End If
                If (DatiMacchina.immaginePiccola IsNot Nothing) Then
                    mac.Img_Thumbnail = If(DatiMacchina.immaginePiccola.immagine Is Nothing, mac.Img_Thumbnail, Convert.FromBase64String(DatiMacchina.immaginePiccola.immagine))
                    mac.Img_Thumbnail_Extension = If(DatiMacchina.immaginePiccola.estensione Is Nothing, mac.Img_Thumbnail_Extension, DatiMacchina.immaginePiccola.estensione)
                    mac.Img_Thumbnail_FileName = If(DatiMacchina.immaginePiccola.nome Is Nothing, mac.Img_Thumbnail_FileName, DatiMacchina.immaginePiccola.nome)
                End If

                mac.VIN = If(DatiMacchina.VIN Is Nothing, mac.VIN, DatiMacchina.VIN)
                mac.BTM_Serial = If(DatiMacchina.BTM_Serial Is Nothing, mac.BTM_Serial, DatiMacchina.BTM_Serial)
                mac.ExternalAPIKey = If(DatiMacchina.ExternalAPIKey Is Nothing, mac.ExternalAPIKey, DatiMacchina.ExternalAPIKey)

                mac.HubIoT_PlatformDestination = If(IsNothing(DatiMacchina.HubIoT_PlatformDestination), mac.HubIoT_PlatformDestination, CShort(DatiMacchina.HubIoT_PlatformDestination.codice))

                mac.Validita_Taratura_Inizio = If(DatiMacchina.data_Ultima_Taratura = Nothing, mac.Validita_Taratura_Inizio, DatiMacchina.data_Ultima_Taratura)
                mac.Validita_Taratura_Fine = If(DatiMacchina.scadenza_Taratura = Nothing, mac.Validita_Taratura_Fine, DatiMacchina.scadenza_Taratura)

                mac.data_modifica = DateTime.Now()

                mac.Ultima_Manutenzione = DatiMacchina.data_Ultima_Manutenzione
                mac.Ultima_Revisione = DatiMacchina.data_Ultima_Revisione

                If DatiMacchina.ageaCod IsNot Nothing Then
                    mac.Agea_Cod = DatiMacchina.ageaCod.codice
                End If

                mac.Portata = DatiMacchina.portata
                mac.Efficienza = DatiMacchina.efficienza
                mac.IMP_COD = DatiMacchina.codice_impianto

                '----------------------------------------------------------------
                ' Scrittura parti Stazione Meteo
                '----------------------------------------------------------------
                mac.Distinta_Installazione = DatiMacchina.Distinta_Installazione
                mac.Contratto_Installazione = DatiMacchina.Contratto_Installazione
                mac.Tipologia_Installazione = DatiMacchina.Tipologia_Installazione
                mac.Data_Inizio_Installazione = DatiMacchina.Data_Inizio_Installazione
                mac.Data_Fine_Installazione = DatiMacchina.Data_Fine_Installazione
                mac.Stato_Installazione = DatiMacchina.Stato_Installazione
                mac.Provincia_Istat_Installazione = DatiMacchina.Provincia_Istat_Installazione
                mac.Comune_Istat_Installazione = DatiMacchina.Comune_Istat_Installazione
                mac.Indirizzo_Installazione = DatiMacchina.Indirizzo_Installazione
                mac.Latitudine_Installazione = DatiMacchina.Latitudine_Installazione
                mac.Longitudine_Installazione = DatiMacchina.Longitudine_Installazione

                If DatiMacchina.contatto IsNot Nothing Then
                    mac.Cod_Contatto = DatiMacchina.contatto.primaryKey.codice
                End If

                '----------------------------------------------------------------
                ' Inizio Parte riguardante la scrittura dei Costi della Macchina
                '----------------------------------------------------------------
                If DatiMacchina.costi IsNot Nothing Then
                    dal.gestisciCosti(DatiMacchina, objParametriServer, username, GiasContext, NewTransaction)
                End If

                '----------------------------------------------------------------
                ' Inizio Parte riguardante la scrittura delle Caratteristiche della Macchina
                '----------------------------------------------------------------
                If DatiMacchina.caratteristiche IsNot Nothing Then
                    dal.gestisciCaratteristiche(DatiMacchina, objParametriServer, username, mac.Mac_Cod, GiasContext, NewTransaction)
                End If


                '----------------------------------------------------------------
                ' Inizio Parte riguardante la scrittura della Gerarchia della Macchina
                '----------------------------------------------------------------
                If DatiMacchina.gerarchiaFigli IsNot Nothing Then
                    dal.gestisciGerarchia(DatiMacchina, objParametriServer, username, mac.Mac_Cod, GiasContext, NewTransaction)
                End If

                '----------------------------------------------------------------
                ' Inizio Parte riguardante la scrittura dei ratei tempo
                '----------------------------------------------------------------
                If DatiMacchina.rateiTempo IsNot Nothing Then
                    dal.gestisciRateiTempo(DatiMacchina, objParametriServer, False, GiasContext)
                End If
                '----------------------------------------------------------------

                '----------------------------------------------------------------
                ' Inizio Parte riguardante la scrittura dei codici della Macchina
                '----------------------------------------------------------------
                Dim pcmObj As New EFParcoMacchineCodici
                Dim success = pcmObj.Update(DatiMacchina, objParametriServer)

                If Not success Then
                    Throw New Exception("Errore durante il salvataggio dei Parco_macchine_Codici")
                End If
                '----------------------------------------------------------------

                GiasContext.Parco_Macchine.Attach(mac)
                GiasContext.Entry(mac).State = EntityState.Modified
                GiasContext.SaveChanges()
            End If

            Dim DatiMacchinaStr = ""
            If DatiMacchina IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                DatiMacchinaStr = JsonConvert.SerializeObject(DatiMacchina, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ParcoMacchine,
                                                                                CStr(mac.Piva),
                                                                                CStr(mac.Sa_Cod),
                                                                                CStr(mac.Mac_Cod), Nothing,
                                                                                Nothing, Nothing,
                                                                                enum_TipoOperazioneDB.Modifica,
                                                                                objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                "", DatiMacchinaStr)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As GiasException
            mac = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw ex
        Catch ex As Exception
            mac = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return mac
    End Function

    Public Shared Sub Macchina_Cancella_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                           ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                           Optional ByVal NewTransaction As Boolean = True
                                           )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.Macchina_Cancella_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False
        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try
            Dim macchine = From macchina In GiasContext.Parco_Macchine
                           Where macchina.Piva = DatiMacchina.partitaIva AndAlso
                                    macchina.Mac_Cod = DatiMacchina.codice
                           Select macchina

            Dim mac = macchine.FirstOrDefault()

            Dim EFCosti = New EFCostoUnitario

            If Not (DatiMacchina.costi Is Nothing) Then
                For Each costo In DatiMacchina.costi
                    EFCosti.ProdottiCosti_Cancella_EF(costo, objParametriServer)
                Next
            End If

            If Not (DatiMacchina.caratteristiche Is Nothing) Then
                For Each caratteristica In DatiMacchina.caratteristiche
                    EFCaratteristiche.Caratteristiche_Cancella_EF(caratteristica, objParametriServer)
                Next
            End If

            ' rimuove associazione macchina-impianto
            Dim impiantiMacchineDaEliminare As New List(Of AgronicaCoreEntityFramework_POCO.Reg_ImpiantiXParcoMacchine)
            impiantiMacchineDaEliminare = (From i In GiasContext.Reg_ImpiantiXParcoMacchine Where i.Mac_Cod = DatiMacchina.codice Select i).ToList()
            If impiantiMacchineDaEliminare.Count > 0 Then
                GiasContext.Reg_ImpiantiXParcoMacchine.RemoveRange(impiantiMacchineDaEliminare)
                GiasContext.SaveChanges()
            End If

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la scrittura dei ratei tempo
            '----------------------------------------------------------------
            dal.gestisciRateiTempo(DatiMacchina, objParametriServer, True)
            '----------------------------------------------------------------

            '----------------------------------------------------------------
            ' Inizio Parte riguardante la rimozione dei codici della Macchina
            '----------------------------------------------------------------
            Dim pcmObj As New EFParcoMacchineCodici
            Dim success = pcmObj.Remove(DatiMacchina, objParametriServer)

            If Not success Then
                Throw New Exception("Errore durante il salvataggio dei Parco_macchine_Codici")
            End If
            '----------------------------------------------------------------

            GiasContext.Parco_Macchine.Attach(mac)
            GiasContext.Parco_Macchine.Remove(mac)
            GiasContext.SaveChanges()

            Dim DatiMacchinaStr = ""
            If DatiMacchina IsNot Nothing Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                DatiMacchinaStr = JsonConvert.SerializeObject(DatiMacchina, a)
            End If

            'Scrittura tabella Agronica_Log_Anagrafe
            Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
            Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ParcoMacchine,
                                                                                 CStr(mac.Piva),
                                                                                 CStr(mac.Sa_Cod),
                                                                                 CStr(mac.Mac_Cod), Nothing,
                                                                                 Nothing, Nothing,
                                                                                 enum_TipoOperazioneDB.Cancellazione,
                                                                                 objParametriServer, enum_Id_Servizio.GiasOnline,
                                                                                 "", DatiMacchinaStr)

            GiasContext.Agronica_Log_Anagrafe.Add(log)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Private Sub gestisciCaratteristiche(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal username As String,
                                    ByVal mac_cod As Integer,
                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                    Optional ByVal NewTransaction As Boolean = True
                                    )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.gestisciCaratteristiche"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Try
            ' Inizio Parte riguardante la scrittura delle Caratterisitche della Macchina
            '----------------------------------------------------------------
            Dim EFCaratteristiche = New EFCaratteristiche

            Dim caratteristiche_old = (From caratteristicaItem In GiasContext.Parco_MacchinexCaratteristiche
                                       Where caratteristicaItem.Mac_Cod = DatiMacchina.codice).ToList()

            If (DatiMacchina.caratteristiche IsNot Nothing) Then
                Dim caratteristiche_new = (From caratteristica In DatiMacchina.caratteristiche Select caratteristica.codice).ToList
                For Each c In caratteristiche_old
                    Dim caratteristicaunitaria = dal.transformParco_MacchineXCaratteristicheToParcoMacchineCaratteristiche(c)
                    EFCaratteristiche.Caratteristiche_Cancella_EF(caratteristicaunitaria, objParametriServer)
                Next

                For Each caratteristica In DatiMacchina.caratteristiche
                    EFCaratteristiche.CreateOrUpdateCaratteristiche(caratteristica, objParametriServer, username, mac_cod)

                Next
                'Else
                'For Each c In caratteristiche_old
                'Dim caratteristicaUnitaria = dal.transformParco_MacchineXCaratteristicheToParcoMacchineCaratteristiche(c)
                'EFCaratteristiche.Caratteristiche_Cancella_EF(caratteristicaUnitaria, objParametriServer)
                'Next
            End If
            ' Fine Parte riguardante la scrittura dei Costi della Macchina
            '----------------------------------------------------------------

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Private Sub gestisciGerarchia(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal username As String,
                                    ByVal mac_cod As Integer,
                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                    Optional ByVal NewTransaction As Boolean = True
                                    )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.gestisciGerarchia"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Try
            ' Inizio Parte riguardante la scrittura della Gerarchia della Macchina
            '----------------------------------------------------------------
            Dim EFGerarchia = New EFGerarchiaMacchina
            Dim listaMacchine = checkFigli_Gerarchia(DatiMacchina.codice, DatiMacchina.gerarchiaFigli.Select(Function(g) g.macchina.codice).ToList(), GiasContext)
            If listaMacchine.Count > 0 Then
                Dim stringamacchine = ""
                For Each codice In listaMacchine
                    stringamacchine = stringamacchine.Concat(CStr(codice) + " ")
                Next
                Throw New GiasException("La macchina figlio " + stringamacchine + " comprende la macchina corrente (" & DatiMacchina.codice & ") tra i suoi figli e perciò non è stato possibile salvare.")
            End If
            If (DatiMacchina.gerarchiaFigli IsNot Nothing) Then

                Dim cancellati = New List(Of GerarchiaParco_Macchine)

                For Each gerarchiaItem In From gerarchia In GiasContext.GerarchiaParco_Macchine Where gerarchia.Mac_Cod_Padre.Equals(DatiMacchina.codice)
                    If Not DatiMacchina.gerarchiaFigli.Select(Function(g) g.ID).ToList().Contains(gerarchiaItem.ID) Then
                        cancellati.Add(gerarchiaItem)
                    End If
                Next

                For Each gerarchia In cancellati
                    EFGerarchia.Gerarchia_Cancella_EF(transformParco_MacchineXGerarchiaToParcoMacchineGerarchia(gerarchia), objParametriServer)
                Next

                For Each gerarchia In DatiMacchina.gerarchiaFigli
                    EFGerarchia.CreateOrUpdateGerarchia(gerarchia, objParametriServer, username, mac_cod)
                Next

            End If
            ' Fine Parte riguardante la scrittura dei Costi della Macchina
            '----------------------------------------------------------------

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New GiasException(messaggioErrore)
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Private Sub gestisciCosti(ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal username As String,
                                    Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                    Optional ByVal NewTransaction As Boolean = True
                                    )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.gestisciCosti"
        Dim messaggioErrore As String = String.Empty
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim dal As New AgronicaCoreContabDAL.EFMacchine()

        Try
            ' Inizio Parte riguardante la scrittura dei Costi della Macchina
            '----------------------------------------------------------------
            Dim EFCosti = New EFCostoUnitario

            Dim costi_old = (From costoItem In GiasContext.Prodotti_Costi
                             Where costoItem.Piva = DatiMacchina.partitaIva AndAlso costoItem.Id_Budget = 0 AndAlso
                                 costoItem.Mat_Cod = DatiMacchina.codice).ToList()

            If (DatiMacchina.costi IsNot Nothing) AndAlso DatiMacchina.costi.Count <> 0 Then
                Dim costi_new = (From costo In DatiMacchina.costi Select costo.codice).ToList
                For Each c In costi_old
                    If Not costi_new.Contains(c.ID) Then
                        Dim costounitario = dal.transformProdotto_CostoToCostoUnitario(c)
                        EFCosti.ProdottiCosti_Cancella_EF(costounitario, objParametriServer)
                    End If
                Next

                For Each costo In DatiMacchina.costi
                    EFCosti.CreateOrUpdateCosts(costo,
                                                    objParametriServer,
                                                    DatiMacchina.partitaIva,
                                                    username,
                                                    DatiMacchina.codice,
                                                    0, 0)

                Next
            Else
                For Each c In costi_old
                    Dim costounitario = dal.transformProdotto_CostoToCostoUnitario(c)
                    EFCosti.ProdottiCosti_Cancella_EF(costounitario, objParametriServer)
                Next
            End If
            ' Fine Parte riguardante la scrittura dei Costi della Macchina
            '----------------------------------------------------------------

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

    Public Sub gestisciRateiTempo(
        ByVal DatiMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
        ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional fromDelete As Boolean = False,
        Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing
    )
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.gestisciRateiTempo"
        Dim bCloseContext = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If DatiMacchina.rateiTempo Is Nothing OrElse fromDelete Then
            DatiMacchina.rateiTempo = New List(Of RateoTempo)()
        End If

        Try
            Dim linkedRatiosEF As List(Of RateiTempo) = GiasContext.Parco_MacchinexRateiTempo.Join(
                    GiasContext.RateiTempo,
                    Function(pm) pm.Rateo_Cod,
                    Function(ratio) ratio.Rateo_Cod,
                    Function(pm, ratio) New With {.Mac_Cod = pm.Mac_Cod, .ratio = ratio}
                ).Where(Function(x) x.Mac_Cod = DatiMacchina.codice).
                Select(Function(x) x.ratio).ToList

            Dim IsDataEqual = Function(a As RateoTempo, b As RateiTempo) a.DataInizio = b.DataInizio AndAlso
                                  a.DataFine = b.DataFine AndAlso a.OraInizio = b.OraInizio AndAlso a.OraFine = b.OraFine AndAlso
                                  a.Rotazione = b.Rotazione

            Dim newRatios As List(Of RateoTempo) = DatiMacchina.rateiTempo.Where(Function(r) r.Rateo_Cod = 0).ToList
            Dim deleted As List(Of RateiTempo) = linkedRatiosEF.Where(Function(ratio) Not DatiMacchina.rateiTempo.Exists(Function(r) r.Rateo_Cod = ratio.Rateo_Cod)).ToList
            Dim editedRatios As List(Of RateoTempo) = DatiMacchina.rateiTempo.Where(Function(r) r.Rateo_Cod <> 0).
                Where(Function(r) Not IsDataEqual(r, linkedRatiosEF.Find(Function(l) l.Rateo_Cod = r.Rateo_Cod))).ToList

            For Each edited In editedRatios
                Dim record As RateiTempo = linkedRatiosEF.Find(Function(r) r.Rateo_Cod = edited.Rateo_Cod)
                record.DataInizio = edited.DataInizio
                record.DataFine = edited.DataFine
                record.OraInizio = edited.OraInizio
                record.OraFine = edited.OraFine
                record.Rotazione = edited.Rotazione
                record.Data_Modifica = Now
                record.Username_Modifica = objParametriServer.UsernameOperazione
            Next

            Dim rateoCodsToDelete As List(Of Integer) = deleted.Select(Function(r) r.Rateo_Cod).ToList
            Dim pmToDelete = GiasContext.Parco_MacchinexRateiTempo.Where(Function(pm) rateoCodsToDelete.Contains(pm.Rateo_Cod)).ToList
            GiasContext.Parco_MacchinexRateiTempo.RemoveRange(pmToDelete)
            GiasContext.RateiTempo.RemoveRange(deleted)

            Dim seq As New Agro_Sequenze()
            For Each ratio In newRatios
                Dim newRatio = New RateiTempo() With {
                    .Rateo_Cod = seq.NuovoId_Tabella("RateiTempo", 0, 2000000000, objParametriServer),
                    .DataInizio = ratio.DataInizio,
                    .DataFine = ratio.DataFine,
                    .OraInizio = ratio.OraInizio,
                    .OraFine = ratio.OraFine,
                    .Rotazione = ratio.Rotazione,
                    .Username_Creazione = objParametriServer.UsernameOperazione,
                    .Username_Modifica = objParametriServer.UsernameOperazione,
                    .Data_Creazione = Now,
                    .Data_Modifica = Now,
                    .inviato = 0,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE
                }
                Dim linked = New Parco_MacchinexRateiTempo() With {
                    .Rateo_Cod = newRatio.Rateo_Cod,
                    .Mac_Cod = DatiMacchina.codice,
                    .Username_Creazione = objParametriServer.UsernameOperazione,
                    .Username_Modifica = objParametriServer.UsernameOperazione,
                    .Data_Creazione = Now,
                    .Data_Modifica = Now,
                    .inviato = 0,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE
                }
                GiasContext.RateiTempo.Add(newRatio)
                GiasContext.Parco_MacchinexRateiTempo.Add(linked)
            Next

            GiasContext.SaveChanges()

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If
    End Sub

    Private Function transformProdotto_CostoToCostoUnitario(ByVal prottoCosto As Prodotti_Costi) As CostoUnitario
        Dim costounitario = New CostoUnitario()
        costounitario.codice = prottoCosto.ID

        Return costounitario
    End Function

    Private Function transformParco_MacchineXCaratteristicheToParcoMacchineCaratteristiche(ByVal caratteristica As Parco_MacchinexCaratteristiche) As ParcoMacchineCaratteristiche
        Dim caratteristicaunitaria = New ParcoMacchineCaratteristiche()
        caratteristicaunitaria.codice = caratteristica.ID

        Return caratteristicaunitaria
    End Function

    Private Function transformParco_MacchineXGerarchiaToParcoMacchineGerarchia(ByVal gerarchia As GerarchiaParco_Macchine) As MacchinaGerarchia

        Dim caratteristicaunitaria = New MacchinaGerarchia()
        caratteristicaunitaria.ID = gerarchia.ID

        Return caratteristicaunitaria
    End Function

    Private Function checkFigli_Gerarchia(ByVal CodMacchinaPadre As Integer,
                                          ByVal gerarchiaFigli As List(Of Integer),
                                          ByRef GiasContext As Gias_DeveloperServer_Entities) As List(Of Integer)
        Dim risultato As New List(Of Integer)
        For Each codice In gerarchiaFigli
            Dim listaGerarchia = GiasContext.GerarchiaParco_Macchine.Select(Function(m) m.Mac_Cod_Padre).ToList()
            If codice.Equals(CodMacchinaPadre) Then
                risultato.Add(codice)
            ElseIf listaGerarchia.Contains(codice) Then
                If checkFigli_Gerarchia(CodMacchinaPadre, GiasContext.GerarchiaParco_Macchine.Where(
                                        Function(c) c.Mac_Cod_Padre = codice).Select(
                                        Function(m) m.Mac_Cod_Figlio).ToList(), GiasContext).Count > 0 Then
                    risultato.Add(codice)
                End If
            End If
        Next
        Return risultato
    End Function

#Region "ParcoMacchine_D2G"

    Private Function CreateParcoMacchineD2G(
                                           ByVal piva As String,
                                           ByVal sa_cod As Integer,
                                           ByVal macCod As Integer,
                                           ByVal username As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.CreateParcoMacchineD2G()"

        Dim parco_macchine As New AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim codice = 0
        If macCod = 0 OrElse macCod = Nothing Then
            'lavez - 24/01/2024 per stack contatore thread safe
            'codice = GenerateNewMacCod_EF(objParametri)
            codice = generateNewMacCod(objParametri)
        Else
            codice = macCod
        End If

        parco_macchine.Mac_Cod = codice

        parco_macchine.Piva = piva
        parco_macchine.Sa_Cod = sa_cod      'Lavez - 31/07/2024 - uniformato a standard con gestione pubblica\privata
        parco_macchine.Mac_Des = ""
        parco_macchine.Costo_Acquisto = 0
        parco_macchine.Targa = ""
        parco_macchine.Telaio = ""
        parco_macchine.Ditta_Cod = 0
        parco_macchine.Modello = ""
        parco_macchine.Potenza = ""
        parco_macchine.Ammortamento = 0
        parco_macchine.Ammortizzato = 0
        parco_macchine.Data_Immatricolazione = AGRODATAINIZIO
        parco_macchine.Ultima_Manutenzione = AGRODATAINIZIO
        parco_macchine.Ultima_Revisione = AGRODATAINIZIO
        parco_macchine.Stato_Utilizzo = ""
        parco_macchine.inviato = 0

        parco_macchine.data_creazione = DateTime.Now
        parco_macchine.data_modifica = DateTime.Now
        parco_macchine.username_creazione = username
        parco_macchine.username_modifica = username
        parco_macchine.validita_inizio = AGRODATAINIZIO
        parco_macchine.validita_fine = AGRODATAFINE
        parco_macchine.Note = ""
        parco_macchine.Tipo = 0
        parco_macchine.N_Immatricolazione = ""
        parco_macchine.N_Immatricolazione_Rimorchio = ""
        parco_macchine.N_Autorizzazione_Trasporto = ""
        parco_macchine.Data_Rilascio_Autorizzazione = AGRODATAINIZIO

        parco_macchine.Peso = 0
        parco_macchine.Mac_Cod_Origine = 0
        parco_macchine.Piva_SuperUser_Origine = ""
        parco_macchine.ChkDefault = 0
        parco_macchine.Portata_Max = 0
        parco_macchine.Cod_Contatto = ""
        parco_macchine.CUAA_Proprietario = ""
        parco_macchine.Denominazione_Proprietario = ""
        parco_macchine.Alimentazione_Cod = 0
        parco_macchine.Potenza_Udm_Cod = 0

        parco_macchine.Tipo_Targa_Cod = 0
        parco_macchine.Tipo_Trazione_Cod = 0
        parco_macchine.N_Omologazione = ""
        parco_macchine.Ditta_Cod_Motore = 0
        parco_macchine.Tipo_Motore = ""
        parco_macchine.Matricola_Motore = ""
        parco_macchine.Data_Reimmatricolazione = AGRODATAINIZIO
        parco_macchine.Data_Carico = DateTime.Now
        parco_macchine.Data_Scarico = DateTime.Now
        parco_macchine.TitoloPossesso = 0
        parco_macchine.Flag_Attrezzatura_Macchina = ""
        parco_macchine.Taratura_Ugello = 0
        parco_macchine.Codice = ""
        parco_macchine.Visibile_ctrl_gestione = 0
        parco_macchine.Img_Large = New Byte(0) {}
        parco_macchine.Img_Large_Extension = ""
        parco_macchine.Img_Large_FileName = ""
        parco_macchine.Img_Thumbnail = New Byte(0) {}
        parco_macchine.Img_Thumbnail_Extension = ""
        parco_macchine.Img_Thumbnail_FileName = ""
        parco_macchine.Agea_Cod = ""
        parco_macchine.Portata = 0
        parco_macchine.Efficienza = 0
        parco_macchine.IMP_COD = 0
        parco_macchine.ExternalAPIKey = ""

        parco_macchine.Validita_Taratura_Inizio = AGRODATAINIZIO
        parco_macchine.Validita_Taratura_Fine = AGRODATAFINE

        Return parco_macchine

    End Function

    Public Function PrepareEditParcoMacchineD2G(
                                               ByVal macchina As ParcoMacchine,
                                               ByVal codice_esterno As String,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareEditParcoMacchineD2G()"
        Dim parco_macchine = (From mac In GiasContext.Parco_Macchine
                              Where mac.Piva = macchina.partitaIva AndAlso
                                    mac.Sa_Cod = macchina.centroPK.codice AndAlso
                                      mac.Mac_Cod = macchina.codice
                              Select mac).FirstOrDefault()

        If parco_macchine Is Nothing Then
            Throw New Exception(String.Format("Nessuna macchina trovata per il codice {0}", macchina.codice))
        End If

        Try

            parco_macchine.Class_Code = (macchina.tipo.codice &
                If(macchina.dettaglio_1.codice = "", "", "." & macchina.dettaglio_1.codice) &
                If(macchina.dettaglio_2.codice = "", "", "." & macchina.dettaglio_2.codice))

            parco_macchine.Telaio = macchina.telaio
            parco_macchine.Targa = macchina.targa
            parco_macchine.Mac_Des = macchina.descrizione
            parco_macchine.Modello = macchina.modello
            parco_macchine.Alimentazione_Cod = If(macchina.alimentazione Is Nothing, parco_macchine.Alimentazione_Cod, macchina.alimentazione.codice)
            parco_macchine.Agea_Cod = If(macchina.ageaCod Is Nothing, parco_macchine.Agea_Cod, macchina.ageaCod.codice)
            parco_macchine.Validita_Taratura_Inizio = macchina.data_Ultima_Taratura
            parco_macchine.Validita_Taratura_Fine = macchina.scadenza_Taratura
            parco_macchine.validita_inizio = macchina.validita.inizio
            parco_macchine.validita_fine = macchina.validita.fine
            parco_macchine.data_modifica = DateTime.Now()
            parco_macchine.username_modifica = objParametri_Server.UsernameOperazione

            GiasContext.Parco_Macchine.Attach(parco_macchine)
            GiasContext.Entry(parco_macchine).State = EntityState.Modified

            If ScriviLog Then
                Dim DatiMacchinaStr = ""
                If macchina IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    DatiMacchinaStr = JsonConvert.SerializeObject(macchina, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    enum_TipoEntita_Des.ParcoMacchine,
                    CStr(parco_macchine.Piva),
                    CStr(parco_macchine.Sa_Cod),
                    CStr(parco_macchine.Mac_Cod), Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Modifica,
                    objParametri_Server,
                    enum_Id_Servizio.Nessuno,
                    "Import Demetra",
                    DatiMacchinaStr, Origine:=enum_SistemiEsterni.demetra
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return parco_macchine

    End Function

    Public Function PrepareCreateParcoMacchineD2G(
                                                 ByVal macchina As ParcoMacchine,
                                                 ByVal codice_esterno As String,
                                                 ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                 ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Optional ByVal ScriviLog As Boolean = True
                                                 ) As AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareCreateParcoMacchineD2G()"

        Dim parco_macchine = CreateParcoMacchineD2G(
            macchina.partitaIva,
            macchina.centroPK.codice,
            macchina.codice,
            objParametri_Server.UsernameOperazione,
            objParametri_Server
            )

        Try
            parco_macchine.Class_Code = (macchina.tipo.codice &
                If(macchina.dettaglio_1.codice = "", "", "." & macchina.dettaglio_1.codice) &
                If(macchina.dettaglio_2.codice = "", "", "." & macchina.dettaglio_2.codice))

            parco_macchine.Telaio = macchina.telaio
            parco_macchine.Targa = macchina.targa
            parco_macchine.Mac_Des = macchina.descrizione
            parco_macchine.Modello = macchina.modello
            parco_macchine.Alimentazione_Cod = If(macchina.alimentazione Is Nothing, parco_macchine.Alimentazione_Cod, macchina.alimentazione.codice)
            parco_macchine.Agea_Cod = If(macchina.ageaCod Is Nothing, parco_macchine.Agea_Cod, macchina.ageaCod.codice)
            parco_macchine.Validita_Taratura_Inizio = macchina.data_Ultima_Taratura
            parco_macchine.Validita_Taratura_Fine = macchina.scadenza_Taratura
            parco_macchine.validita_inizio = macchina.validita.inizio
            parco_macchine.validita_fine = macchina.validita.fine
            parco_macchine.username_modifica = objParametri_Server.UsernameOperazione

            GiasContext.Parco_Macchine.Add(parco_macchine)

            If ScriviLog Then
                Dim DatiMacchinaStr = ""
                If macchina IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    DatiMacchinaStr = JsonConvert.SerializeObject(macchina, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    enum_TipoEntita_Des.ParcoMacchine,
                    CStr(parco_macchine.Piva),
                    CStr(parco_macchine.Sa_Cod),
                    CStr(parco_macchine.Mac_Cod), Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Scrittura,
                    objParametri_Server,
                    enum_Id_Servizio.Nessuno,
                    "Import Demetra",
                    DatiMacchinaStr, Origine:=enum_SistemiEsterni.demetra
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return parco_macchine

    End Function

    Public Function PrepareDeleteParcoMacchineD2G(
                                                 ByVal macchina As ParcoMacchine,
                                                 ByVal codice_esterno As String,
                                                 ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                 ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Optional ByVal ScriviLog As Boolean = True
                                                 ) As AgronicaCoreEntityFramework_POCO.Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareDeleteParcoMacchineD2G()"
        Dim parco_macchine = (From mac In GiasContext.Parco_Macchine
                              Where mac.Piva = macchina.partitaIva AndAlso
                                    mac.Mac_Cod = macchina.codice
                              Select mac).FirstOrDefault

        If parco_macchine Is Nothing Then
            Throw New Exception(String.Format("Nessuna macchina trovata per il codice {0}", macchina.codice))
        End If

        Try
            Dim EFCosti = New EFCostoUnitario

            If Not (macchina.costi Is Nothing) Then
                For Each costo In macchina.costi
                    EFCosti.ProdottiCosti_Cancella_EF(costo, objParametri_Server, GiasContext, False, False)
                Next
            End If

            If Not (macchina.caratteristiche Is Nothing) Then
                For Each caratteristica In macchina.caratteristiche
                    EFCaratteristiche.Caratteristiche_Cancella_EF(caratteristica, objParametri_Server, GiasContext, False, False)
                Next
            End If

            GiasContext.Parco_Macchine.Attach(parco_macchine)
            GiasContext.Parco_Macchine.Remove(parco_macchine)

            If ScriviLog Then
                Dim DatiMacchinaStr = ""
                If macchina IsNot Nothing Then
                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    DatiMacchinaStr = JsonConvert.SerializeObject(macchina, a)
                End If

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                    enum_TipoEntita_Des.ParcoMacchine,
                    CStr(parco_macchine.Piva),
                    CStr(parco_macchine.Sa_Cod),
                    CStr(parco_macchine.Mac_Cod), Nothing,
                    Nothing, Nothing,
                    enum_TipoOperazioneDB.Cancellazione,
                    objParametri_Server, enum_Id_Servizio.GiasOnline,
                    "Import Demetra",
                    DatiMacchinaStr, Origine:=enum_SistemiEsterni.demetra
                    )

                GiasContext.Agronica_Log_Anagrafe.Add(log)
            End If

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return parco_macchine

    End Function

#End Region

End Class