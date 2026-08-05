Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data.Entity
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.exceptions

Public Class EFEsercizi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function Create_EserciziCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef esercizio As Imprese_Progetti,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As Reg_Impianti_Codici

        Dim reg = Create_EserciziCodici(dal, objParametri, esercizio.Piva, esercizio.Sa_Cod, esercizio.Appezza, esercizio.Id_Reg, esercizio.Progetto_Cod, id_cod, val_cod, username)

        Return reg
    End Function


    Private Shared Function Create_EserciziCodici(ByRef dal As Gias_DeveloperServer_Entities,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef piva As String,
                                                   ByRef sa_cod As Integer,
                                                   ByRef appezza As Integer,
                                                   ByRef id_reg As Integer,
                                                   ByRef progetto_cod As Integer,
                                                   ByRef id_cod As Integer,
                                                   ByRef val_cod As String,
                                                   ByRef username As String) As Reg_Impianti_Codici

        Dim impianto As New Reg_Impianti_Codici

        impianto.PIVA = piva
        impianto.sa_cod = sa_cod
        impianto.appezza = appezza
        impianto.Id_Reg = id_reg
        impianto.Progetto_Cod = progetto_cod

        impianto.id_cod = id_cod
        impianto.val_cod = val_cod

        impianto.inviato = 0
        impianto.datainvio = DateTime.Now

        impianto.Data_Creazione = DateTime.Now
        impianto.Data_Modifica = DateTime.Now

        impianto.Validita_Inizio = AGRODATAINIZIO
        impianto.Validita_Fine = AGRODATAFINE

        impianto.Username_Creazione = username
        impianto.Username_Modifica = username

        impianto.Validazione = 0
        impianto.Data_Validazione = DateTime.Now
        impianto.UserName_Validazione = ""

        dal.Reg_Impianti_Codici.Add(impianto)
        dal.SaveChanges()

        Return impianto
    End Function


    Public Shared Function NuovoProgetto_Cod(ByRef GiasContext As Gias_DeveloperServer_Entities, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim idGen As New Agro_Sequenze
        Dim esercizio = idGen.NuovoId_Tabella_EF(GiasContext, "Impresa_Progetto", 0, 2000000000, objParametri)

        Return esercizio
    End Function

    Private Shared Function Create_Esercizio(ByRef dal As Gias_DeveloperServer_Entities,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                ByRef piva As String,
                                                ByRef sa_cod As Integer,
                                                ByRef appezza As Integer,
                                                ByRef id_reg As Integer,
                                                ByRef username As String,
                                             Optional usernameCreazioneOriginale_xToolCopiaSposta As String = "",
                                             Optional dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO
                                              ) As Imprese_Progetti

        Dim esercizio As New Imprese_Progetti

        esercizio.Piva = piva
        esercizio.Sa_Cod = sa_cod
        esercizio.Appezza = appezza
        esercizio.Id_Reg = id_reg
        esercizio.Progetto_Cod = NuovoProgetto_Cod(dal, objParametri)

        esercizio.Progetto_Nome = ""
        esercizio.Progetto_Des = ""
        esercizio.Cod_Contratto = 0
        esercizio.Cod_Conto = 0
        esercizio.Ricavi_Previsti = 0
        esercizio.Produzione_Prevista = 0
        esercizio.Cau_Progetto = CAU_PROGETTO_PRODUZIONE
        esercizio.Giudizio = ""
        esercizio.Data_Inizio_Prevista = AGRODATAINIZIO
        esercizio.Data_Fine_Prevista = AGRODATAINIZIO

        esercizio.Validita_Inizio = AGRODATAINIZIO
        esercizio.Validita_Fine = AGRODATAFINE
        esercizio.inviato = 0
        esercizio.Datainvio = AGRODATAINIZIO
        esercizio.Data_Creazione = If(dataCreazioneOriginale_xToolCopiaSposta <> AGRODATAINIZIO, dataCreazioneOriginale_xToolCopiaSposta, DateTime.Now)
        esercizio.Data_Modifica = DateTime.Now
        esercizio.Username_Creazione = If(usernameCreazioneOriginale_xToolCopiaSposta <> "", usernameCreazioneOriginale_xToolCopiaSposta, username)
        esercizio.Username_Modifica = username
        esercizio.Veg_Cod = 0
        esercizio.Grfi_Cod = 0
        esercizio.CSProgetto_Cod = 0
        esercizio.Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione
        esercizio.Regolamento_Cod = 1
        esercizio.Disciplinare_Cod = 0
        esercizio.P_HA = 0
        esercizio.Data_Fioritura_Prevista = AGRODATAINIZIO
        esercizio.Disciplinare_PubblicoPrivato = 0
        esercizio.Regolamento_Concimazioni_Cod = 0
        esercizio.Sup_Prog = 0
        esercizio.FlagSecondoRaccolto = 0
        esercizio.P_HA_Femmine = 0
        esercizio.P_HA_Maschi = 0
        esercizio.GruppoRaccolta_Cod = 0

        Return esercizio
    End Function

    Public Shared Function Esercizio_Scrivi_EF(ByVal Dati_esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                               ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal username As String,
                                               Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                               Optional ByVal NewTransaction As Boolean = True,
                                               Optional ByVal NoteLog As String = "",
                                               Optional ScriviLog As Boolean = True,
                                               Optional usernameCreazioneOriginale_xToolCopiaSposta As String = "",
                                               Optional dataCreazioneOriginale_xToolCopiaSposta As Date = AGRODATAINIZIO
                                               ) As Imprese_Progetti

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Scrivi_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        ''COMMENTATO PERCHÉ NON SERVE PIÙ, FACCIO GIA LO STESSO CONTROLLO NELL'APPEZZAMENTO
        'If EFImprese.ImpresaExist(GiasContext, Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva) = False Then
        '    Throw New GiasException("Partita Iva (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + ") non anagrafica imprese. Operazione annullata")
        'End If


        'If EFCentri_Aziendali.CentroExist(GiasContext,
        '                                  Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
        '                                  Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice) = False Then
        '    Throw New GiasException("Centro aziendale (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
        '                                               Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + ") non anagrafica. Operazione annullata")
        'End If


        'If EFAppezzamento.AppezzamentoExist(GiasContext,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.codice) = False Then
        '    Throw New GiasException("Appezzamento (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
        '                                           Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
        '                                           Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() +
        '                                       ") non trovato in anagrafica. Impossibile proseguire")
        'End If

        'If EFReg_Impianti.ImpiantoExist(GiasContext,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
        '                                    Dati_esercizio.impiantoPK.appezzamentoPK.codice,
        '                                    Dati_esercizio.impiantoPK.codice) = False Then
        '    Throw New GiasException("Impianto (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
        '                                           Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
        '                                           Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() + "/" +
        '                                           Dati_esercizio.impiantoPK.codice.ToString() +
        '                                       ") non trovato in anagrafica. Impossibile proseguire")
        'End If


        Dim esercizio = Create_Esercizio(GiasContext,
                                         objParametriServer,
                                         Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                         Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                         Dati_esercizio.impiantoPK.appezzamentoPK.codice,
                                         Dati_esercizio.impiantoPK.codice,
                                         username,
                                         usernameCreazioneOriginale_xToolCopiaSposta:=usernameCreazioneOriginale_xToolCopiaSposta,
                                         dataCreazioneOriginale_xToolCopiaSposta:=dataCreazioneOriginale_xToolCopiaSposta
                                         )

        Try

            If Dati_esercizio.vincolo IsNot Nothing Then
                Dati_esercizio.disciplinare = Dati_esercizio.vincolo.disciplinare
                Dati_esercizio.regolamento = Dati_esercizio.vincolo.regolamento
                Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione = Dati_esercizio.vincolo.disciplinare.regolamentoConcimazione
            End If

            If Dati_esercizio.prodotto IsNot Nothing Then
                esercizio.Mat_Cod = Dati_esercizio.prodotto.codice
            End If

            esercizio.Progetto_Nome = If(Dati_esercizio.lotto IsNot Nothing, Dati_esercizio.lotto, "")
            esercizio.Progetto_Des = Dati_esercizio.descrizione
            esercizio.Produzione_Prevista = Dati_esercizio.resa_prevista
            esercizio.Data_Inizio_Prevista = If(Dati_esercizio.data_Semina_Trapianto_Prevista < AGRODATAINIZIO, AGRODATAINIZIO, If(Dati_esercizio.data_Semina_Trapianto_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Semina_Trapianto_Prevista))
            esercizio.Data_Fine_Prevista = If(Dati_esercizio.data_Raccolta_Prevista < AGRODATAINIZIO, AGRODATAFINE, If(Dati_esercizio.data_Raccolta_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Raccolta_Prevista))
            esercizio.Validita_Inizio = Dati_esercizio.validita.inizio
            esercizio.Validita_Fine = Dati_esercizio.validita.fine
            If (Dati_esercizio.apportiMassimiMacroelementi IsNot Nothing) Then
                If (Dati_esercizio.apportiMassimiMacroelementi.fase IsNot Nothing) Then
                    esercizio.Stato_Impianto = Dati_esercizio.apportiMassimiMacroelementi.fase.codice
                End If
            End If

            esercizio.P_HA = Dati_esercizio.piante_Ha
            esercizio.Data_Fioritura_Prevista = If(Dati_esercizio.data_Fioritura_Prevista < AGRODATAINIZIO, AGRODATAINIZIO, If(Dati_esercizio.data_Fioritura_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Fioritura_Prevista))

            If Dati_esercizio.disciplinare Is Nothing Then
                esercizio.Disciplinare_Cod = 0
            ElseIf IsNumeric(Dati_esercizio.disciplinare.codice) Then
                esercizio.Disciplinare_Cod = Integer.Parse(Dati_esercizio.disciplinare.codice)
            ElseIf Dati_esercizio.disciplinare.codice.Split("/").Length = 4 Then
                esercizio.Disciplinare_Cod = Integer.Parse(Dati_esercizio.disciplinare.codice.Split("/")(0))
            End If

            esercizio.Disciplinare_PubblicoPrivato = If(Dati_esercizio.disciplinare Is Nothing, 0, Dati_esercizio.disciplinare.disciplinarePubblicoPrivato)

            If Dati_esercizio.flagSecondoRaccolto Then
                esercizio.FlagSecondoRaccolto = 1
            Else
                esercizio.FlagSecondoRaccolto = 0
            End If

            If Dati_esercizio.apportiMassimiMacroelementi IsNot Nothing Then
                esercizio.Regolamento_Concimazioni_Cod = If(Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione Is Nothing, 0, Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione.codice)
            End If

            If (Dati_esercizio.regolamento IsNot Nothing AndAlso Dati_esercizio.regolamento.codice > 0) Then
                esercizio.Regolamento_Cod = Dati_esercizio.regolamento.codice
            Else
                esercizio.Regolamento_Cod = 1
            End If

            esercizio.Sup_Prog = Dati_esercizio.superficie
            esercizio.P_HA_Femmine = Dati_esercizio.piante_Ha_Femmine
            esercizio.P_HA_Maschi = Dati_esercizio.Piante_Ha_Maschi

            esercizio.GruppoRaccolta_Cod = If(Dati_esercizio.gruppoRaccolta IsNot Nothing, Dati_esercizio.gruppoRaccolta.codice, 0)

            GiasContext.Imprese_Progetti.Add(esercizio)
            GiasContext.SaveChanges()

            If ScriviLog Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim DatiEsercizioStr = JsonConvert.SerializeObject(Dati_esercizio, a)

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                            esercizio.Piva, CStr(esercizio.Progetto_Cod),
                                                            CStr(esercizio.Sa_Cod), CStr(esercizio.Appezza),
                                                            CStr(esercizio.Id_Reg), Nothing,
                                                            enum_TipoOperazioneDB.Scrittura,
                                                            objParametriServer, enum_Id_Servizio.GiasOnline,
                                                            NoteLog, DatiEsercizioStr)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()
            End If
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

        Return esercizio

    End Function

    Public Shared Function Esercizio_Modifica_EF(ByVal Dati_esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                                 ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByVal username As String,
                                                 Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal NewTransaction As Boolean = True,
                                                 Optional NoteLog As String = "",
                                                 Optional AggiornaSoloValidita As Boolean = False,
                                                 Optional ScriviLog As Boolean = True
                                                ) As Imprese_Progetti

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Progetti_Write.Esercizio_Modifica_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If



        If EFImprese.ImpresaExist(GiasContext, Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva) = False Then
            Throw New GiasException("Partita Iva (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + ") non anagrafica imprese. Operazione annullata")
        End If


        If EFCentri_Aziendali.CentroExist(GiasContext,
                                          Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                          Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice) = False Then
            Throw New GiasException("Centro aziendale (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                       Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + ") non anagrafica. Operazione annullata")
        End If


        If EFAppezzamento.AppezzamentoExist(GiasContext,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.codice) = False Then
            Throw New GiasException("Appezzamento (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        If EFReg_Impianti.ImpiantoExist(GiasContext,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                                            Dati_esercizio.impiantoPK.appezzamentoPK.codice,
                                            Dati_esercizio.impiantoPK.codice) = False Then
            Throw New GiasException("Impianto (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Dim esercizi = From esercizio In GiasContext.Imprese_Progetti
                       Where esercizio.Piva = Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                 esercizio.Sa_Cod = Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice AndAlso
                                 esercizio.Appezza = Dati_esercizio.impiantoPK.appezzamentoPK.codice AndAlso
                                 esercizio.Id_Reg = Dati_esercizio.impiantoPK.codice AndAlso
                                 esercizio.Progetto_Cod = Dati_esercizio.codice
                       Select esercizio

        Dim ese = esercizi.FirstOrDefault
        If ese Is Nothing Then
            Throw New GiasException("Esercizio (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.codice.ToString() + "/" +
                                                   Dati_esercizio.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Try
            If AggiornaSoloValidita Then
                ese.Validita_Inizio = Dati_esercizio.validita.inizio
                ese.Validita_Fine = Dati_esercizio.validita.fine
            Else
                If Dati_esercizio.vincolo IsNot Nothing Then
                    Dati_esercizio.disciplinare = Dati_esercizio.vincolo.disciplinare
                    Dati_esercizio.regolamento = Dati_esercizio.vincolo.regolamento
                    Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione = Dati_esercizio.vincolo.disciplinare.regolamentoConcimazione
                End If

                If Dati_esercizio.prodotto IsNot Nothing Then
                    ese.Mat_Cod = Dati_esercizio.prodotto.codice
                End If

                If Dati_esercizio.lotto IsNot Nothing Then
                    ese.Progetto_Nome = Dati_esercizio.lotto
                End If
                ese.Progetto_Des = Dati_esercizio.descrizione
                ese.Produzione_Prevista = Dati_esercizio.resa_prevista
                ese.Data_Inizio_Prevista = If(Dati_esercizio.data_Semina_Trapianto_Prevista < AGRODATAINIZIO, AGRODATAINIZIO, If(Dati_esercizio.data_Semina_Trapianto_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Semina_Trapianto_Prevista))
                ese.Data_Fine_Prevista = If(Dati_esercizio.data_Raccolta_Prevista < AGRODATAINIZIO, AGRODATAINIZIO, If(Dati_esercizio.data_Raccolta_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Raccolta_Prevista))
                ese.Validita_Inizio = Dati_esercizio.validita.inizio
                ese.Validita_Fine = Dati_esercizio.validita.fine
                ese.Data_Modifica = DateTime.Now
                ese.Username_Modifica = username

                If Dati_esercizio.apportiMassimiMacroelementi IsNot Nothing Then
                    If Dati_esercizio.apportiMassimiMacroelementi.fase IsNot Nothing Then
                        ese.Stato_Impianto = Dati_esercizio.apportiMassimiMacroelementi.fase.codice
                    End If
                End If

                'If Not ((Dati_esercizio.apportiMassimiMacroelementi Is Nothing) Or (Dati_esercizio.apportiMassimiMacroelementi.fase Is Nothing)) Then
                '    ese.Stato_Impianto = Dati_esercizio.apportiMassimiMacroelementi.fase.codice
                'End If

                If Dati_esercizio.flagSecondoRaccolto Then
                    ese.FlagSecondoRaccolto = 1
                Else
                    ese.FlagSecondoRaccolto = 0
                End If

                If (Dati_esercizio.regolamento IsNot Nothing AndAlso Dati_esercizio.regolamento.codice > 0) Then
                    ese.Regolamento_Cod = Dati_esercizio.regolamento.codice
                Else
                    ese.Regolamento_Cod = 1
                End If
                If Dati_esercizio.disciplinare Is Nothing Then
                    ese.Disciplinare_Cod = 0
                ElseIf IsNumeric(Dati_esercizio.disciplinare.codice) Then
                    ese.Disciplinare_Cod = Integer.Parse(Dati_esercizio.disciplinare.codice)
                ElseIf Dati_esercizio.disciplinare.codice.Split("/").Length = 4 Then
                    ese.Disciplinare_Cod = Integer.Parse(Dati_esercizio.disciplinare.codice.Split("/")(0))
                End If
                ese.P_HA = Dati_esercizio.piante_Ha
                ese.Data_Fioritura_Prevista = If(Dati_esercizio.data_Fioritura_Prevista < AGRODATAINIZIO, AGRODATAINIZIO, If(Dati_esercizio.data_Fioritura_Prevista > AGRODATAFINE, AGRODATAFINE, Dati_esercizio.data_Fioritura_Prevista))
                ese.Disciplinare_PubblicoPrivato = If(Dati_esercizio.disciplinare Is Nothing, 0, Dati_esercizio.disciplinare.disciplinarePubblicoPrivato)
                If (Dati_esercizio.apportiMassimiMacroelementi IsNot Nothing AndAlso Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione IsNot Nothing) Then
                    ese.Regolamento_Concimazioni_Cod = Dati_esercizio.apportiMassimiMacroelementi.pianoConcimazione.codice
                End If
                ese.Sup_Prog = Dati_esercizio.superficie
                ese.P_HA_Femmine = Dati_esercizio.piante_Ha_Femmine
                ese.P_HA_Maschi = Dati_esercizio.Piante_Ha_Maschi
                ese.GruppoRaccolta_Cod = If(Dati_esercizio.gruppoRaccolta IsNot Nothing, Dati_esercizio.gruppoRaccolta.codice, 0)
            End If



            GiasContext.Imprese_Progetti.Attach(ese)
            GiasContext.Entry(ese).State = EntityState.Modified
            GiasContext.SaveChanges()

            If ScriviLog Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim DatiEsercizioStr = JsonConvert.SerializeObject(Dati_esercizio, a)

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                            ese.Piva, CStr(ese.Progetto_Cod),
                                                            CStr(ese.Sa_Cod), CStr(ese.Appezza),
                                                            CStr(ese.Id_Reg), Nothing,
                                                            enum_TipoOperazioneDB.Modifica,
                                                            objParametriServer, enum_Id_Servizio.GiasOnline,
                                                            NoteLog, DatiEsercizioStr)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()
            End If
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As GiasException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw ex
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

        Return ese

    End Function

    Public Shared Sub Esercizio_Cancella_EF(ByVal Dati_esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio,
                                            ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal NewTransaction As Boolean = True,
                                            Optional ScriviLog As Boolean = True
                                                )

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Cancella_EF()"
        Dim messaggioErrore As String = ""

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            Dim esercizi = From esercizio In GiasContext.Imprese_Progetti
                           Where esercizio.Piva = Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva AndAlso
                                 esercizio.Sa_Cod = Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice AndAlso
                                 esercizio.Appezza = Dati_esercizio.impiantoPK.appezzamentoPK.codice AndAlso
                                 esercizio.Id_Reg = Dati_esercizio.impiantoPK.codice AndAlso
                                 esercizio.Progetto_Cod = Dati_esercizio.codice
                           Select esercizio

            Dim ese = esercizi.FirstOrDefault

            If ese Is Nothing Then
                Throw New Exception("Esercizio (" + Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.appezzamentoPK.codice.ToString() + "/" +
                                                   Dati_esercizio.impiantoPK.codice.ToString() + "/" +
                                                   Dati_esercizio.codice.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
            End If


            GiasContext.Imprese_Progetti.Attach(ese)
            GiasContext.Imprese_Progetti.Remove(ese)
            GiasContext.SaveChanges()

            If ScriviLog Then
                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Progetti,
                                                            ese.Piva, CStr(ese.Progetto_Cod),
                                                            CStr(ese.Sa_Cod), CStr(ese.Appezza),
                                                            CStr(ese.Id_Reg), Nothing,
                                                            enum_TipoOperazioneDB.Cancellazione,
                                                            objParametriServer, enum_Id_Servizio.GiasOnline)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()
            End If
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
End Class
