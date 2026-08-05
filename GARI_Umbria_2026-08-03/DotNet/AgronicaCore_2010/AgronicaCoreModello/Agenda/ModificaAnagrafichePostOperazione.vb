'(es: gestione abbattimento, ovvero chiusura appezzamento; modifica dell’impianto quando semino con l’apposita opzione)  da chiamare dopo il salvataggio dell’operazione (scriviAgenda in transazione)
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.metaschema

Public Class ModificaAnagrafichePostOperazione
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Agenda"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Shared Function ModificaAnagrafichePostOperazione(
        agenda As Operazione_Agenda,
        infoOperazione As InfoOperazione,
        eseguiSoloVerificheConformita As Boolean,
        currentAttivitaDes As String,
        opzioniOperazione As Integer,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of ErroreGias)
        Dim listaErrori As New List(Of ErroreGias)

        '-- OPZIONI RACCOLTA
        If infoOperazione.IsRaccolta Then
            listaErrori = PostOperazione_Raccolta(
                agenda, eseguiSoloVerificheConformita,
                currentAttivitaDes, opzioniOperazione,
                objParametri_Server, objParametri_Utenti
            )
        End If

        '-- OPZIONI ABBATTIMENTO
        If infoOperazione.IsAbbattimento Then
            listaErrori = PostOperazione_Abbattimento(
                agenda, eseguiSoloVerificheConformita,
                currentAttivitaDes, opzioniOperazione,
                objParametri_Server, objParametri_Utenti
            )
        End If

        Return listaErrori
    End Function


#Region "APERTURE E CHIUSURE ANAGRAFICA"
    Public Shared Function ChiusureAperture_PostOperazione(
        agenda As Operazione_Agenda,
        eseguiSoloVerificheConformita As Boolean,
        currentAttivitaDes As String,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri,
        Optional chiudiEsercizi As Boolean = False,
        Optional chiudiImpianti As Boolean = False,
        Optional chiudiAppezzamenti As Boolean = False,
        Optional apriNuoviEsercizi As Boolean = False,
        Optional apriNuoviImpianti As Boolean = False,
        Optional NoteLog As String = ""
    ) As List(Of ErroreGias)
        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.ChiusureAperture_PostOperazione()]"
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim listaErrori As New List(Of ErroreGias)

        Try
            Dim scriviGIS As New GisHelper
            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            '----- RECUPERO MOVIMENTO CAMPAGNA
            Dim attivita As New AgronicaCoreModelsSTD.attivita.Attivita
            Dim InfoOperazione As InfoOperazione = GetInfoOperazione(agenda.Lav_Cod, attivita.Tipo_Attivita.QuadernoDiCampagna)
            Dim MovimentoCampagna As Movimento = GetMovimentoCampagna(agenda, InfoOperazione)

            Dim PIVA, DesEsercizio, DesImpianto As String
            Dim SA_COD, APPEZZA, ID_REG, Progetto_Cod, Campo_Cod As Integer
            Dim DataChiusura As Date

            If chiudiEsercizi OrElse chiudiImpianti OrElse chiudiAppezzamenti OrElse apriNuoviEsercizi OrElse apriNuoviImpianti Then
                'Ciclo fino a Movimenti_Destinazioni per ottenere piva - sa_cod - appezza
                For Each movimentoDettaglio In MovimentoCampagna.Movimenti_Dettagli
                    For Each movimentoDestinazione In movimentoDettaglio.Movimenti_Destinazioni

                        Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim dt_impiantoDaChiudere As DataTable = objRegImpianti.Leggi(
                            movimentoDestinazione.Piva, movimentoDestinazione.Sa_Cod,
                            movimentoDestinazione.Appezza, movimentoDestinazione.Id_Destinazione,
                            enumSelezioneVariabile.Selezione_JoinCompleta,
                            "", "", objParametri_Server
                        )

                        DataChiusura = movimentoDestinazione.Data

                        If dt_impiantoDaChiudere IsNot Nothing AndAlso dt_impiantoDaChiudere.Rows.Count > 0 Then
                            Dim impianto = dt_impiantoDaChiudere.Select.Where(Function(row) CDate(row("Validita_fine_Distinta")) >= DataChiusura).FirstOrDefault

                            'Se la sup_imp è uguale alla superficie trattata, 
                            If impianto IsNot Nothing AndAlso impianto.Item("Sup_Imp") = CDec(movimentoDestinazione.Qta2) Then
                                PIVA = impianto.Item("PIVA")
                                SA_COD = impianto.Item("SA_COD")
                                APPEZZA = impianto.Item("APPEZZA")
                                ID_REG = impianto.Item("ID_REG")
                                Progetto_Cod = impianto.Item("Progetto_Cod")

                                DesImpianto = impianto.Item("Veg_Des") & "-" & impianto.Item("Cul_Des") & " (App. " & impianto.Item("APP_NOME") & ")"
                                DesEsercizio = "[" & impianto.Item("Validita_inizio_Distinta") & "-" & impianto.Item("Validita_fine_Distinta") & "]"

                                'verifico che non ci siano operazioni registrate con data successiva
                                listaErrori = VerificaProcedi_chiusuraImpiantoEsercizio(PIVA, SA_COD, APPEZZA, ID_REG, Progetto_Cod,
                                                                                        DataChiusura, DesEsercizio, DesImpianto,
                                                                                        currentAttivitaDes,
                                                                                        objParametri_Server, objParametri_Utenti)
                                If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
                                    Return listaErrori
                                End If

                                If Not (eseguiSoloVerificheConformita) Then
                                    Dim objApp As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                    Dim DT_Appezzamento = objApp.Leggi(PIVA, SA_COD, APPEZZA,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "",
                                                                   objParametri_Server)

                                    'Recupero CAMPO_COD
                                    If DT_Appezzamento IsNot Nothing AndAlso DT_Appezzamento.Rows.Count > 0 Then
                                        If Not IsDBNull(DT_Appezzamento.Rows(0).Item("Campo_Cod")) Then
                                            Campo_Cod = DT_Appezzamento.Rows(0).Item("Campo_Cod")
                                        Else
                                            Campo_Cod = 0
                                        End If
                                    End If


                                    '--CHIUSURA/APERTURA ESERCIZIO
                                    If apriNuoviEsercizi OrElse chiudiEsercizi Then
                                        ApriChiudiEsercizio(impianto, PIVA, SA_COD, APPEZZA, ID_REG, Progetto_Cod, DataChiusura, apriNuoviEsercizi, chiudiEsercizi, objParametri_Server, NoteLog)
                                    End If


                                    '--CHIUSURA/APERTURA IMPIANTO
                                    If apriNuoviImpianti OrElse chiudiImpianti Then
                                        Dim appezzamento = DT_Appezzamento.Rows(0)
                                        ApriChiudiImpianto(appezzamento, PIVA, SA_COD, APPEZZA, Campo_Cod, ID_REG, DataChiusura, apriNuoviImpianti, chiudiImpianti, objParametri_Server, objParametri_Utenti, NoteLog)
                                    End If


                                    '--CHIUSURA APPEZZAMENTO
                                    If chiudiAppezzamenti Then
                                        ChiudiAppezzamento(PIVA, SA_COD, APPEZZA, Campo_Cod, DataChiusura, objParametri_Server, objParametri_Utenti, NoteLog)
                                    End If
                                End If
                            End If
                        End If
                    Next
                Next
            End If

            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
            Throw New Exception(nomeRoutine & ": " & ex.Message)
        End Try

        Return listaErrori
    End Function

    Private Shared Sub ApriChiudiEsercizio(impianto As DataRow,
                                           piva As String,
                                           sa_cod As Integer,
                                           appezza As Integer,
                                           id_reg As Integer,
                                           progetto_cod As Integer,
                                           data_chiusura As Date,
                                           apriNuovo As Boolean,
                                           chiudi As Boolean,
                                           objParametri_Server As AgronicaCoreParametri,
                                           Optional NoteLog As String = ""
                                           )

        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.apriChiudiEsercizio()]"
        Dim objEsercizio_W As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W


        Dim centroPK = New anagrafiche.CentroAziendale.PK(sa_cod, piva)
        Dim appezzamentoPK = New anagrafiche.Appezzamento.PK(appezza, centroPK)
        Dim impiantoPK = New anagrafiche.Impianto.PK(id_reg, appezzamentoPK)

        Dim esercizio = New anagrafiche.Esercizio(progetto_cod, "") With {
            .impiantoPK = impiantoPK
        }

        RicavaEsercizio(esercizio, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)

        Try

            If chiudi Then
                esercizio.validita.fine = data_chiusura
                AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Modifica_EF(esercizio, objParametri_Server, objParametri_Server.UsernameOperazione, NoteLog:=NoteLog)
            End If

            If apriNuovo AndAlso CanOpenNewEsercizio(esercizio, objParametri_Server) Then
                Dim Validita_Fine_Impianto As Date = impianto.Item("Validita_Fine")

                '---IMPOSTO I NUOVI DATI
                'imposto la data inizio come la data fine della distinta precedente piu un giorno
                esercizio.validita.inizio = DateAdd(DateInterval.Day, 1, esercizio.validita.fine)

                If esercizio.validita.inizio > Validita_Fine_Impianto Then
                    'errore!!!! non dovrebbe mai finire qui
                    Throw New Exception(Gias.DataInizioEsercizioSuperaDataFineImpianto)
                End If

                If esercizio.validita.fine <= esercizio.validita.inizio Then
                    esercizio.validita.fine = DateAdd(DateInterval.Year, 1, esercizio.validita.fine)
                End If
                If esercizio.validita.fine > Validita_Fine_Impianto Then
                    'la data fine della distinta non può superare la data fine dell'impianto
                    'quindi la imposto uguale alla data fine impianto
                    esercizio.validita.fine = Validita_Fine_Impianto
                End If

                'data semina prevista
                If esercizio.data_Semina_Trapianto_Prevista <> AGRODATAINIZIO Then
                    esercizio.data_Semina_Trapianto_Prevista = DateAdd(DateInterval.Year, 1, esercizio.data_Semina_Trapianto_Prevista)
                Else
                    esercizio.data_Semina_Trapianto_Prevista = AGRODATAINIZIO
                End If

                'data raccolta prevista
                If esercizio.data_Raccolta_Prevista <> AGRODATAFINE Then
                    esercizio.data_Raccolta_Prevista = DateAdd(DateInterval.Year, 1, esercizio.data_Raccolta_Prevista)
                Else
                    esercizio.data_Raccolta_Prevista = AGRODATAFINE
                End If

                'data fioritura prevista
                If esercizio.data_Fioritura_Prevista <> AGRODATAINIZIO Then
                    esercizio.data_Fioritura_Prevista = DateAdd(DateInterval.Year, 1, esercizio.data_Fioritura_Prevista)
                Else
                    esercizio.data_Fioritura_Prevista = AGRODATAINIZIO
                End If

                'lotto
                If esercizio.validita.inizio.Year <> esercizio.validita.fine.Year Then
                    esercizio.lotto = "Lotto " + CStr(esercizio.validita.inizio.Year) + "/" + CStr(esercizio.validita.fine.Year)
                Else
                    esercizio.lotto = "Lotto " + CStr(esercizio.validita.inizio.Year)
                End If

                esercizio.descrizione = esercizio.lotto



                AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Scrivi_EF(esercizio,
                                                                       objParametri_Server,
                                                                       objParametri_Server.UsernameOperazione,
                                                                       NoteLog:=NoteLog)

            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Private Shared Function CanOpenNewEsercizio(esercizio As Esercizio, objParametri_Server As AgronicaCoreParametri) As Boolean
        'Dim progetto_cod = esercizio.codice
        Dim id_reg = esercizio.impiantoPK.codice
        Dim appezza = esercizio.impiantoPK.appezzamentoPK.codice
        Dim sa_cod = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
        Dim piva = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        Dim expectedEnd = DateAdd(DateInterval.Day, 1, esercizio.validita.fine)
        expectedEnd = DateAdd(DateInterval.Year, 1, expectedEnd)

        Dim getDateOrDefault = Function(d As DataRow, f As String) If(IsDBNull(d(f)), AGRODATAINIZIO, CDate(d(f)))
        Dim overlapsValidiy = Function(e As DataRow, d As Date) getDateOrDefault(e, "Validita_Inizio") <= d AndAlso getDateOrDefault(e, "Validita_Fine") >= d

        Dim objEsercizio_R As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim esercizi = objEsercizio_R.LeggiMinimal(
            piva, sa_cod, appezza, id_reg, 0,
            AGRODATAINIZIO, AGRODATAFINE, String.Empty, String.Empty,
            objParametri_Server
        ).Select
        If esercizi.Count > 1 AndAlso esercizi.Any(Function(e) overlapsValidiy(e, expectedEnd)) Then
            Return False 'Dates would overlap
        End If
        Return True
    End Function

    Private Shared Sub ChiudiEserciziCollegati(impianto As Impianto, objParametri_Server As AgronicaCoreParametri, Optional NoteLog As String = "")
        Dim id_reg = impianto.primaryKey.codice
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim getIntOrDefault = Function(d As DataRow, f As String) If(IsDBNull(d(f)), 0, CInt(d(f)))

        Dim objEsercizio_R As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim daChiudere = objEsercizio_R.LeggiMinimal(
            piva, sa_cod, appezza, id_reg, 0,
            impianto.validita.fine, AGRODATAFINE, String.Empty, String.Empty,
            objParametri_Server
        ).Select.
        Select(Function(row) New Esercizio(getIntOrDefault(row, "Progetto_Cod"), "") With {
            .impiantoPK = impianto.primaryKey
        })

        For Each esercizio In daChiudere
            RicavaEsercizio(esercizio, piva, sa_cod, appezza, id_reg, esercizio.codice, objParametri_Server)
            esercizio.validita.fine = impianto.validita.fine
            AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Modifica_EF(esercizio, objParametri_Server, objParametri_Server.UsernameOperazione, NoteLog:=NoteLog)
        Next
    End Sub

    Private Shared Sub ApriChiudiImpianto(appezzamento As DataRow,
                                          piva As String,
                                          sa_cod As Integer,
                                          appezza As Integer,
                                          campo_cod As Integer,
                                          id_reg As Integer,
                                          data_chiusura As Date,
                                          apriNuovo As Boolean,
                                          chiudi As Boolean,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri,
                                          Optional NoteLog As String = ""
                                          )

        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.apriChiudiImpianto()]"
        Dim objImpianti_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write

        Dim centroPK = New anagrafiche.CentroAziendale.PK(sa_cod, piva)
        Dim appezzamentoPK = New anagrafiche.Appezzamento.PK(appezza, centroPK)
        'Dim impiantoPK = New anagrafiche.Impianto.PK(id_reg, appezzamentoPK)
        'Dim impianto = New anagrafiche.Impianto(impiantoPK)

        Try

            Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            Dim impianto = objImpianti_R.Leggi_Impianto_Anagrafica(piva, sa_cod, appezza, id_reg,
                                                                   False, False,
                                                                   AGRODATAINIZIO, filtroData:=False, False,
                                                                   Nothing, objParametri_Server, objParametri_Utenti)

            If chiudi Then
                impianto.validita.fine = data_chiusura
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.Impianto_Modifica_EF(impianto, objParametri_Server, objParametri_Utenti, objParametri_Server.UsernameOperazione, NoteLog:=NoteLog)

                ChiudiEserciziCollegati(impianto, objParametri_Server)
            End If


            If apriNuovo AndAlso CanOpenNewImpianto(impianto, objParametri_Server) Then
                Dim impiantoPK = New anagrafiche.Impianto.PK(0, appezzamentoPK)
                Dim impiantoNEW = New anagrafiche.Impianto(impiantoPK)

                impiantoNEW.validita = impianto.validita
                Dim Validita_Fine_Appezzamento As Date = appezzamento.Item("Validita_Fine")

                '---IMPOSTO I NUOVI DATI
                'imposto la data inizio come la data fine della distinta precedente piu un giorno
                impiantoNEW.validita.inizio = DateAdd(DateInterval.Day, 1, impianto.validita.fine)

                If impiantoNEW.validita.inizio > Validita_Fine_Appezzamento Then
                    'errore!!!! non dovrebbe mai finire qui
                    Throw New Exception(Gias.DataInizioImpiantoSuperaDataFineAppezzamento)
                End If

                'La data fine la setto uguale alla data fine dell'appezzamento
                impiantoNEW.validita.fine = Validita_Fine_Appezzamento

                'If impiantoNEW.validita.fine > Validita_Fine_Appezzamento Then
                '    'la data fine della distinta non può superare la data fine dell'impianto
                '    'quindi la imposto uguale alla data fine impianto
                '    impiantoNEW.validita.fine = Validita_Fine_Appezzamento
                'End If

                impiantoNEW.superficie = impianto.superficie

                '--Preparo l'esercizio vuoto
                Dim esercizio = New anagrafiche.Esercizio(0, "") With {
                    .impiantoPK = impiantoPK
                }

                Dim progetto_nome As String = ""
                Dim progetto_des As String = ""
                If impiantoNEW.validita.inizio.Year <> impianto.validita.fine.Year Then
                    esercizio.lotto = Gias.Lotto & " " & CStr(impiantoNEW.validita.inizio.Year) & "/" & CStr(impiantoNEW.validita.fine.Year)
                Else
                    esercizio.lotto = Gias.Lotto & " " & CStr(impiantoNEW.validita.inizio.Year)
                End If

                esercizio.descrizione = esercizio.lotto
                esercizio.validita = New anagrafiche.IntervalloTemporale(impiantoNEW.validita.inizio, impiantoNEW.validita.fine)

                '--IMPOSTO IL TERRENO NUDO
                impiantoNEW.utilizzoTerreno = New utilizzi.DestinazioneUso(enum_CodiciAnagrafe.UtilizzoImpianto_DaDefinire) With {
                    .descrizione = ""
                }

                Dim imp As Reg_Impianti = AgronicaCoreAnagrafeDAL.EFReg_Impianti.Impianto_Scrivi_EF(impiantoNEW,
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti,
                                                                                                    objParametri_Server.UsernameOperazione,
                                                                                                    NoteLog:=NoteLog)
                impiantoNEW.primaryKey.codice = imp.ID_REG

                '--SCRIVO CODICE TERRENO NUDO
                AgronicaCoreAnagrafeDAL.EFReg_Impianti.ScriviModificaEliminaDestinazioneUsoxImpianto(enum_TipoOperazioneDB.Scrittura,
                                                                                                     impiantoNEW.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                                                                                                     impiantoNEW.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                                                                                                     impiantoNEW.primaryKey.appezzamentoPK.codice,
                                                                                                     impiantoNEW.primaryKey.codice,
                                                                                                     impiantoNEW.utilizzoTerreno.codice,
                                                                                                     "",
                                                                                                     False,
                                                                                                     objParametri_Server.UsernameOperazione,
                                                                                                     objParametri_Server, Nothing, False, True)

                '--APRO UN NUOVO ESERCIZIO
                AgronicaCoreAnagrafeDAL.EFEsercizi.Esercizio_Scrivi_EF(esercizio,
                                                                       objParametri_Server,
                                                                       objParametri_Server.UsernameOperazione,
                                                                       NoteLog:=NoteLog)

            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Private Shared Function CanOpenNewImpianto(impianto As Impianto, objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim id_reg = impianto.primaryKey.codice
        Dim appezza = impianto.primaryKey.appezzamentoPK.codice
        Dim sa_cod = impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice
        Dim piva = impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
        Dim expectedEnd = DateAdd(DateInterval.Day, 1, impianto.validita.fine)

        Dim getDateOrDefault = Function(d As DataRow, f As String) If(IsDBNull(d(f)), AGRODATAINIZIO, CDate(d(f)))
        Dim overlapsValidiy = Function(e As DataRow, d As Date) getDateOrDefault(e, "Validita_Inizio") <= d AndAlso getDateOrDefault(e, "Validita_Fine") >= d

        Dim objImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim impiantiFuturi = objImpianti_R.Leggi(
            piva, sa_cod, appezza, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            String.Empty, String.Empty, objParametri_Server
        ).Select.Where(Function(r) getDateOrDefault(r, "Validita_Inizio") > impianto.validita.fine)
        If impiantiFuturi.Any Then
            Return False
        End If
        Return True
    End Function

    Private Shared Sub ChiudiImpiantiCollegati(appezza As anagrafiche.Appezzamento, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri, Optional NoteLog As String = "")
        Dim appezzaCod = appezza.primaryKey.codice
        Dim sa_cod = appezza.primaryKey.centroAziendalePK.codice
        Dim piva = appezza.primaryKey.centroAziendalePK.partitaIva
        Dim getIntOrDefault = Function(d As DataRow, f As String) If(IsDBNull(d(f)), 0, CInt(d(f)))

        Dim objImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objImpiantiBIZ_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
        Dim daChiudere = objImpianti_R.Leggi(
            piva, sa_cod, appezzaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
            String.Empty, String.Empty, objParametri_Server
        ).Select

        For Each row In daChiudere
            Dim impianto = objImpiantiBIZ_R.Leggi_Impianto_Anagrafica(
                piva, sa_cod, appezzaCod, getIntOrDefault(row, "Id_Reg"),
                False, False, AGRODATAINIZIO, filtroData:=False, False,
                Nothing, objParametri_Server, objParametri_Utenti
            )
            impianto.validita.fine = appezza.validita.fine
            ChiudiEserciziCollegati(impianto, objParametri_Server)
            AgronicaCoreAnagrafeDAL.EFReg_Impianti.Impianto_Modifica_EF(impianto, objParametri_Server, objParametri_Utenti, objParametri_Server.UsernameOperazione, NoteLog:=NoteLog)
        Next
    End Sub

    Private Shared Sub ChiudiAppezzamento(piva As String,
                                          sa_cod As Integer,
                                          appezza As Integer,
                                          campo_cod As Integer,
                                          data_chiusura As Date,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri,
                                          Optional NoteLog As String = ""
                                          )

        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.chiudiAppezzamento()]"

        Dim centroPK = New anagrafiche.CentroAziendale.PK(sa_cod, piva)
        Dim appezzamentoPK = New anagrafiche.Appezzamento.PK(appezza, centroPK)

        Dim appezzamento = New anagrafiche.Appezzamento(appezzamentoPK)


        Try

            Dim objModifica As New ModificaAnagrafichePostOperazione
            objModifica.RicavaAppezzamento(appezzamento, piva, sa_cod, appezza, campo_cod, objParametri_Server)
            appezzamento.validita.fine = data_chiusura

            ChiudiImpiantiCollegati(appezzamento, objParametri_Server, objParametri_Utenti)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.Appezzamento_Modifica_EF(appezzamento,
                                                                            objParametri_Server,
                                                                            objParametri_Server.UsernameOperazione,
                                                                            NoteLog:=NoteLog)


            AgronicaCoreAnagrafeDAL.EFAppezzamento.UtentiXAppezzamenti_Modifica_EF(appezzamento,
                                                                                   objParametri_Server,
                                                                                   objParametri_Server.UsernameOperazione,
                                                                                   objParametri_Server.PivaSuperUser,
                                                                                   NoteLog:=NoteLog)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.AppezzamentiXParticelle_Modifica_EF(appezzamento,
                                                                                       objParametri_Server,
                                                                                       objParametri_Server.UsernameOperazione,
                                                                                       NoteLog:=NoteLog)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.AppezzamentiXParticellexMacrousi_Modifica_EF(appezzamento,
                                                                                                objParametri_Server,
                                                                                                objParametri_Server.UsernameOperazione,
                                                                                                NoteLog:=NoteLog)

            AgronicaCoreAnagrafeDAL.EFAppezzamento.AppezzamentiXParticellexMacrousixUtilizzo_Modifica_EF(appezzamento,
                                                                                                         objParametri_Server,
                                                                                                         objParametri_Server.UsernameOperazione,
                                                                                                         NoteLog:=NoteLog)

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub
#End Region

#Region "UTILITY RICAVA ANAGRAFICA"
    Public Shared Sub RicavaEsercizio(ByRef esercizio As anagrafiche.Esercizio,
                                       piva As String,
                                       sa_cod As Integer,
                                       appezza As Integer,
                                       id_reg As Integer,
                                       progetto_cod As Integer,
                                       objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.RicavaEsercizio()]"


        '--RICAVO L'ESERCIZIO
        Dim objEsercizio_R As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R


        Try
            Dim oldEsercizio As DataTable = objEsercizio_R.LeggiMinimal(piva, sa_cod, appezza, id_reg, progetto_cod,
                                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                                   "", "",
                                                                   objParametri_Server)


            If (oldEsercizio) IsNot Nothing AndAlso oldEsercizio.Rows.Count > 0 Then

                With oldEsercizio.Rows(0)

                    esercizio.lotto = .Item("Progetto_Nome")
                    esercizio.descrizione = .Item("Progetto_Des")

                    Dim objRegolamento As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
                    Dim Regolamento_Des = ""
                    If .Item("Regolamento_Cod") <> 0 Then
                        Dim dtReg = objRegolamento.Leggi(.Item("Regolamento_Cod"), enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                        If dtReg.Rows.Count > 0 Then
                            Regolamento_Des = dtReg.Rows(0)("Reg_DES")
                        End If
                    End If
                    Dim disciplinare_pubblicoprivato = .Item("disciplinare_pubblicoprivato")

                    esercizio.regolamento = New metaschema.Regolamenti(.Item("Regolamento_Cod")) With {.descrizione = Regolamento_Des}
                    esercizio.disciplinare = New Disciplinare(.Item("Disciplinare_cod")) With {
                    .disciplinarePubblicoPrivato = disciplinare_pubblicoprivato,
                    .regolamentoConcimazione = New RegolamentoConcimazione(0),
                    .raggruppamentiColturaliDPI = New RaggruppamentiColturaliDPI(0)
                }

                    esercizio.disciplinare.codice = .Item("Disciplinare_cod")

                    Dim apportoMacroElementi As New ApportoMacroelementi

                    apportoMacroElementi.pianoConcimazione = New RegolamentoConcimazione(.Item("Regolamento_Concimazioni_Cod"))
                    apportoMacroElementi.tipologia = New FinalitaPianoConcimazione(.Item("Regolamento_Concimazioni_Cod"))
                    apportoMacroElementi.fase = New FaseCicloColturale(.Item("Stato_Impianto"))

                    esercizio.piante_Ha = .Item("p_ha")
                    esercizio.data_Fioritura_Prevista = CDate(.Item("data_fioritura_prevista"))
                    esercizio.data_Raccolta_Prevista = CDate(.Item("data_fine_prevista"))
                    esercizio.data_Semina_Trapianto_Prevista = CDate(.Item("data_inizio_prevista"))
                    esercizio.id_tr = 3
                    esercizio.resa_prevista = .Item("produzione_prevista")

                    If Not IsDBNull(.Item("P_HA_Femmine")) Then
                        esercizio.piante_Ha_Femmine = .Item("P_HA_Femmine")
                    End If

                    If Not IsDBNull(.Item("P_HA_Maschi")) Then
                        esercizio.Piante_Ha_Maschi = .Item("P_HA_Maschi")
                    End If

                    esercizio.data_Semina_Trapianto_Prevista = .Item("data_inizio_prevista")
                    esercizio.data_Raccolta_Prevista = .Item("data_fine_prevista")

                    esercizio.validita = New anagrafiche.IntervalloTemporale(.Item("validita_inizio"), .Item("validita_fine"))

                    esercizio.superficie = .Item("Sup_Prog")
                End With
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Public Sub RicavaAppezzamento(ByRef appezzamento As anagrafiche.Appezzamento,
                                          piva As String,
                                          sa_cod As Integer,
                                          appezza As Integer,
                                          campio_cod As Integer,
                                          objParametri_Server As AgronicaCoreParametri)

        Dim nomeRoutine As String = "[ModificaAnagrafichePostOperazione.RicavaAppezzamento()]"

        '--RICAVO L'APPEZZAMENTO
        Dim objAppezzamento_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R


        Try
            Dim app As DataTable = objAppezzamento_R.Leggi(piva, sa_cod, appezza,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "",
                                                                   objParametri_Server)


            If (app) IsNot Nothing AndAlso app.Rows.Count > 0 Then

                With app.Rows(0)

                    appezzamento.campoPK = New AgronicaCoreModelsSTD.anagrafiche.Campo.PK(.Item("Campo_Cod"),
                                                                                          New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(sa_cod, piva))

                    appezzamento.descrizione = IIf(IsDBNull(.Item("App_Nome")), "", .Item("App_Nome"))
                    appezzamento.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(.Item("Validita_Inizio"), .Item("Validita_Fine"))
                    appezzamento.superficie = .Item("sup_app")


                    appezzamento.pendenza = .Item("pende")
                    appezzamento.esposizione = New baseClass.BaseCodeDescrStr(.Item("esposiz"), .Item("esposiz"))
                    appezzamento.ubicazione = New baseClass.BaseCodeDescrStr(.Item("ubicazione"), .Item("ubicazione"))

                    appezzamento.supBZ_Riduzione = .Item("SupBZ_Riduzione")
                    appezzamento.distBZ_CorpiIdrici = .Item("DistBZ_CorpiIdrici")
                    appezzamento.distBZ_AreeResPub = .Item("DistBZ_AreeResPub")
                    appezzamento.distBZ_Allevamenti = .Item("DistBZ_Allevamenti")
                    appezzamento.distBZ_VegNatNonColt = .Item("DistBZ_VegNatNonColt")

                    appezzamento.lat = .Item("x")
                    appezzamento.lng = .Item("y")
                    appezzamento.altitudine = .Item("zslm")

                    appezzamento.n_App_Bio = ""
                    appezzamento.confini_A_Rischio = ""
                    appezzamento.utilizzo_Terreno = New List(Of baseClass.BaseCodeDescr)
                    appezzamento.fine_Impiego_Prod_Non_Conformi = AGRODATAINIZIO

                    appezzamento.metodo_Produzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(enum_MetodoProduzione.Integrato) With {.descrizione = Gias.Integrato}

                End With
            End If

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Sub
#End Region

#Region "CONTROLLI"
    Private Shared Function VerificaProcedi_chiusuraImpiantoEsercizio(PIVA As String,
                                                                      SA_COD As Integer,
                                                                      APPEZZA As Integer,
                                                                      ID_REG As Integer,
                                                                      Progetto_Cod As Integer,
                                                                      DataChiusura As Date,
                                                                      DesEsercizio As String,
                                                                      DesImpianto As String,
                                                                      currentAttivitaDes As String,
                                                                      objParametri_Server As AgronicaCoreParametri,
                                                                      objParametri_Utenti As AgronicaCoreParametri
                                                                      ) As List(Of ErroreGias)

        Dim lista_Errori As New List(Of ErroreGias)
        Dim messaggioErrore As String
        Dim _DataChiusura = Format(DataChiusura, "dd/MM/yyyy")


        '-- OPERAZIONI REGISTRATE
        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DTAgenda As DataTable
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
        DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(PIVA, SA_COD, APPEZZA, ID_REG,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      " Data_Movimento > " & Agro_SQL_SaveDate(CDate(DataChiusura)) & " ",
                                                      "Data_Movimento DESC ",
                                                      objParametri_Server)
        objParametri_Server.ResettaFinestra()

        'Se c'è almeno una riga, aggiungo un warning
        If DTAgenda.Rows.Count > 0 Then

            Dim Des_Lib, Data_Movimento
            Des_Lib = DTAgenda.Rows(0)("Des_Lib")
            Data_Movimento = Format(DTAgenda.Rows(0)("Data_Movimento"), "dd/MM/yyyy")

            If DTAgenda.Rows.Count = 1 Then
                messaggioErrore = String.Format(Gias.ImpossibileProcedereChiusuraImpiantoEsisteOperazioneDesDelSuccessivaChiusura, DesImpianto, _DataChiusura, Des_Lib, Data_Movimento)
            Else
                messaggioErrore = String.Format(Gias.ImpossibileProcedereChiusuraImpiantoEsistonoOperazioniSuccessiveChiusura, DesImpianto, _DataChiusura)
            End If

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggioErrore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
            Return lista_Errori
        End If


        '-- CdG 
        Dim objCdG As New AgronicaCoreContabDAL.CDG_DAL_R
        Dim dtCdG As New DataTable
        dtCdG = objCdG.LeggiCronologia_CdG(PIVA, SA_COD, APPEZZA, ID_REG, Progetto_Cod,
                                           " Data_Movimento > " & Agro_SQL_SaveDate(CDate(DataChiusura)) & " ",
                                           " Data_Movimento DESC ",
                                           objParametri_Server,
                                           joinAttivita:=True)
        objParametri_Server.ResettaFinestra()

        'Se c'è almeno una riga, aggiungo un warning
        If dtCdG.Rows.Count > 0 Then

            Dim Des_Lib, Data_Inserimento
            Des_Lib = dtCdG.Rows(0)("Des_Lib")
            Data_Inserimento = Format(dtCdG.Rows(0)("Data_Movimento"), "dd/MM/yyyy")

            If dtCdG.Rows.Count = 1 Then
                messaggioErrore = String.Format(Gias.ImpossibileProcedereChiusuraEsercizioEsisteCdGDesDelSuccessivaChiusura, DesEsercizio, _DataChiusura, Des_Lib, Data_Inserimento)
            Else
                messaggioErrore = String.Format(Gias.ImpossibileProcedereChiusuraEsercizioEsistonoCdGSuccessiveChiusura, DesEsercizio, _DataChiusura)
            End If

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggioErrore, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
            Return lista_Errori
        End If

        Return lista_Errori

    End Function
#End Region

#Region "POST OPERAZIONE"

    Private Shared Function PostOperazione_Raccolta(
        agenda As Operazione_Agenda,
        eseguiSoloVerificheConformita As Boolean,
        currentAttivitaDes As String,
        opzioniOperazione As enum_Opzioni_Raccolta_Aggiornamento_Anagrafica,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of ErroreGias)
        Dim chiudiEsercizi As Boolean = False
        Dim apriNuoviEsercizi As Boolean = False
        Dim chiudiImpianti As Boolean = False
        Dim apriNuoviImpianti As Boolean = False
        Dim chiudiAppezzamenti As Boolean = False

        Dim NoteLog As String = "Raccolta (NG): "
        Select Case opzioniOperazione
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI
                    'non faccio niente
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_ESERCIZI
                chiudiEsercizi = True
                NoteLog += " chiusura esercizi"
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_ESERCIZI
                chiudiEsercizi = True
                apriNuoviEsercizi = True
                NoteLog += " chiusura e apertura esercizi"
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_IMPIANTI_ESERCIZI
                chiudiEsercizi = True
                chiudiImpianti = True
                NoteLog += " chiusura esercizi, chiusura impianti"
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_IMPIANTI_TERRENO_NUDO
                chiudiEsercizi = True
                chiudiImpianti = True
                apriNuoviImpianti = True
                NoteLog += " chiusura esercizi, chiusura e apertura impianti"
            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI
                chiudiEsercizi = True
                chiudiImpianti = True
                chiudiAppezzamenti = True
                NoteLog += " chiusura esercizi, chiusura impianti, chiusura appezzamenti"

        End Select

        Return ChiusureAperture_PostOperazione(
            agenda, eseguiSoloVerificheConformita, currentAttivitaDes,
            objParametri_Server, objParametri_Utenti,
            chiudiEsercizi, chiudiImpianti, chiudiAppezzamenti,
            apriNuoviEsercizi, apriNuoviImpianti, NoteLog
        )
    End Function

    Private Shared Function PostOperazione_Abbattimento(
        agenda As Operazione_Agenda,
        eseguiSoloVerificheConformita As Boolean,
        currentAttivitaDes As String,
        opzioniOperazione As enum_Opzioni_Raccolta_Aggiornamento_Anagrafica,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_Utenti As AgronicaCoreParametri
    ) As List(Of ErroreGias)
        Dim chiudiEsercizi As Boolean = True
        Dim chiudiImpianti As Boolean = True
        Dim chiudiAppezzamenti As Boolean = True
        Dim apriNuoviEsercizi As Boolean = False
        Dim apriNuoviImpianti As Boolean = False
        Dim NoteLog As String = "Abbattimento Impianti (NG): "
        Select Case opzioniOperazione
            'Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.LASCIA_ATTIVI
            '    chiudiEsercizi = False
            '    chiudiImpianti = False
            '    chiudiAppezzamenti = False
            '    NoteLog += " lascia attivi"

            'Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_ESERCIZI
            '    chiudiImpianti = False
            '    chiudiAppezzamenti = False
            '    NoteLog += " chiusura esercizi"

            'Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_ESERCIZI
            '    chiudiEsercizi = True
            '    apriNuoviEsercizi = True
            '    chiudiImpianti = False
            '    chiudiAppezzamenti = False
            '    NoteLog += " chiusura e apertura esercizi"

            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_IMPIANTI_ESERCIZI
                chiudiAppezzamenti = False
                NoteLog += " chiusura esercizi, chiusura impianti"

            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APRI_IMPIANTI_TERRENO_NUDO
                chiudiAppezzamenti = False
                apriNuoviImpianti = True
                NoteLog += " chiusura esercizi, chiusura e apertura impianti"

            Case enum_Opzioni_Raccolta_Aggiornamento_Anagrafica.CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI
                'non faccio nulla
                NoteLog += " chiusura esercizi, chiusura impianti, chiusura appezzamenti"
        End Select

        Return ChiusureAperture_PostOperazione(
            agenda, eseguiSoloVerificheConformita, currentAttivitaDes,
            objParametri_Server, objParametri_Utenti,
            chiudiEsercizi, chiudiImpianti, chiudiAppezzamenti,
            apriNuoviEsercizi, apriNuoviImpianti, NoteLog
        )
    End Function

#End Region

#Region "MODIFICHE ANAGRAFICA"
    Public Shared Sub AggiornamentoAnagrafica(risorsaProdotto As RisorsaProdotto,
                                              piva As String,
                                              sa_cod As Integer,
                                              appezza As Integer,
                                              id_reg As Integer,
                                              progetto_cod As Integer,
                                              objParametri_Server As AgronicaCoreParametri,
                                              objParametri_Utenti As AgronicaCoreParametri,
                                              Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                              Optional ByVal OpenNewTransaction As Boolean = True
                                              )

        Dim nomeRoutine As String = "[ValiditaFormaleOperazione.AggiornamentoAnagrafica()]"



        Dim objAppezzamento_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim appezzamento = objAppezzamento_R.Leggi_Appezzamento_Anagrafica(piva, sa_cod, appezza, IdReg:=0,
                                                                           Leggi_Impianti:=True,
                                                                           Leggi_Indirizzi:=False,
                                                                           Leggi_Catasto:=True,
                                                                           data:=AGRODATAINIZIO,
                                                                           filtroData:=False,
                                                                           Leggi_Distinte:=True,
                                                                           Leggi_Cartografia:=False,
                                                                           objParametri_Super_Server:=Nothing,
                                                                           objParametri_Server,
                                                                           objParametri_Utenti)

        Try

            Modifica_Appezzamento("", 0, Nothing, appezzamento, id_reg, progetto_cod, risorsaProdotto, 0,
                                          objParametri_Server, objParametri_Utenti, modificaSoloVarietaSpecieFinalita:=True,
                                          GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                          NoteLog:="Semina con aggiornamento anagrafica (NG)")


        Catch ex As GiasException
            Throw New GiasException(ex.Message)
        Catch ex As Exception
            Throw New Exception(nomeRoutine & ": " & ex.Message)
        End Try

    End Sub

    Public Shared Sub Modifica_Appezzamento(descrizioneAppezzamento As String,
                                            ByRef nrApp As Integer,
                                            ByRef esercizioCDC As Esercizio,
                                            appezzamento As anagrafiche.Appezzamento,
                                            id_reg As Integer,
                                            progetto_cod As Integer,
                                            RisorsaProdotto As RisorsaProdotto,
                                            sup_app As Decimal,
                                            objParametri_Server As AgronicaCoreParametri,
                                            objParametri_Utenti As AgronicaCoreParametri,
                                                 Optional modificaSoloVarietaSpecieFinalita As Boolean = False,
                                                 Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal OpenNewTransaction As Boolean = True,
                                                 Optional NoteLog As String = ""
                                             )

        Dim nomeroutine As String = "[ValiditaFormaleOperazione.modifica_Appezzamento()]"

        Dim impianto = appezzamento.impianti.Where(Function(c) c.primaryKey.codice = id_reg).FirstOrDefault()
        Dim esercizio = impianto.esercizi.Where(Function(c) c.codice = progetto_cod).FirstOrDefault()

        Dim DettaglioSemina = CType(RisorsaProdotto, dettagli.DettaglioSemina)

        Dim Veg_Cod As Integer = DettaglioSemina.varieta.specie.codice
        Dim Cul_Cod As Integer = DettaglioSemina.varieta.codice
        Dim MetodoProduzione As Integer = IIf(DettaglioSemina.regolamento = 4, 3, DettaglioSemina.regolamento)

        Dim Regolamento As Integer
        Select Case MetodoProduzione
            Case enum_MetodoProduzione.Biologico, enum_MetodoProduzione.InConversione
                Regolamento = enum_Cod_Regolamento.Regolamento_bio
            Case Else
                Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
        End Select


        Dim Udm_Cod As Decimal = RisorsaProdotto.unitaDiMisura.codice
        Dim Qta As Decimal = RisorsaProdotto.quantitaTotaleReale
        Dim P_HA As Decimal = 0
        'Se l'UDM è N.Piante modifico il Numero piante sulla distinta
        Select Case CInt(Udm_Cod)
            Case enum_UnitaMisura.Num_Piante
                P_HA = RisorsaProdotto.doseHaReale
        End Select


        Dim objFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R
        Dim Grfi_Cod As Integer = objFinalita.PrimoGrfiCod_from_Vegcod(Veg_Cod, objParametri_Server)

        Dim objCopertura As New AgronicaCoreMetaSchemaDAL.Copertura_R
        Dim Cop_Cod As Integer = objCopertura.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)



        Try


            '=======================================
            '   APPEZZAMENTO
            '---------------------------------------
            appezzamento.metodo_Produzione.codice = MetodoProduzione

            '=======================================
            '   IMPIANTO
            '---------------------------------------
            Dim varieta As New utilizzi.Varieta(DettaglioSemina.varieta.codice)
            varieta.specie = New utilizzi.Specie(Veg_Cod)
            impianto.utilizzoTerreno = varieta
            impianto.utilizzoTerreno.classType = ClassType.Varieta

            impianto.gruppoFinalita = New utilizzi.GruppoFinalita(Grfi_Cod)


            '=======================================
            '   ESERCIZIO
            '---------------------------------------
            esercizio.regolamento.codice = Regolamento


            If Not (modificaSoloVarietaSpecieFinalita) Then
                '=======================================
                '   APPEZZAMENTO
                '---------------------------------------
                appezzamento.superficie = sup_app
                appezzamento.descrizione = descrizioneAppezzamento + " - " + nrApp.ToString()

                CancellaCollegamentoParticelle(appezzamento, GiasContext, OpenNewTransaction)


                '=======================================
                '   IMPIANTO
                '---------------------------------------
                impianto.superficie = sup_app
                impianto.copertura = New metaschema.Copertura(Cop_Cod)


                '=======================================
                '   ESERCIZIO
                '---------------------------------------
                esercizio.piante_Ha = P_HA
                esercizioCDC = esercizio
            End If


            'alla fine della fiera chiamo questa questo
            Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            objAppezzamento_W.Appezzamento_ScriviModifica(appezzamento,
                                                          objParametri_Server, objParametri_Utenti,
                                                          GiasContext, OpenNewTransaction,
                                                          isFromOperazioneAgenda:=True,
                                                          NoteLog:=NoteLog)


            nrApp += 1
        Catch ex As GiasException
            Throw New GiasException(ex.Message)
        Catch ex As Exception
            Throw New Exception(nomeroutine & ": " & ex.Message)
        End Try


    End Sub

    Public Shared Sub Crea_Appezzamento(ByRef nrApp As Integer,
                                         ByRef esercizioCDC As Esercizio,
                                         appezzamentoOLD As anagrafiche.Appezzamento,
                                         piva As String,
                                         sa_cod As Integer,
                                         campo_cod As Integer,
                                         validita As IntervalloTemporale,
                                         risorsaProdotto As RisorsaProdotto,
                                         descrizioneAppezzamento As String,
                                         sup_app As Decimal,
                                         isTerrenoNudo As Boolean,
                                         objParametri_Server As AgronicaCoreParametri,
                                         objParametri_Utenti As AgronicaCoreParametri,
                                            Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                            Optional ByVal OpenNewTransaction As Boolean = True
                                         )

        Dim centroPK = New CentroAziendale.PK(sa_cod, piva)
        Dim appezzamentoPK = New anagrafiche.Appezzamento.PK(0, centroPK)
        Dim appezzamentoNEW = New anagrafiche.Appezzamento(appezzamentoPK)

        Dim impiantoPK = New Impianto.PK(0, appezzamentoPK)
        Dim impianto = New Impianto(impiantoPK)

        Dim esercizio = New Esercizio(0, "")

        appezzamentoNEW.impianti = New List(Of Impianto)
        appezzamentoNEW.impianti.Add(impianto)

        impianto.esercizi = New List(Of Esercizio)
        impianto.esercizi.Add(esercizio)


        Try

            appezzamentoNEW.descrizione = descrizioneAppezzamento + " - " + nrApp.ToString()
            appezzamentoNEW.validita = New IntervalloTemporale(validita.inizio, validita.fine)
            appezzamentoNEW.superficie = sup_app

            If campo_cod <> 0 Then
                Dim campoPK = New Campo.PK(campo_cod, centroPK)
                appezzamentoNEW.campoPK = campoPK
            End If

            If appezzamentoOLD.catastoAppezzamento.Count = 1 Then
                appezzamentoNEW.catastoAppezzamento = appezzamentoOLD.catastoAppezzamento
            End If
            'Dim AssociaCatasto As Boolean = False

            'Dim ListaPart As New List(Of Particella)
            'ListaPart = ListaImpianti(0).ListaParticelle
            'If ListaPart.Count = 1 Then
            '    AssociaCatasto = True
            'End If


            impianto.validita = New IntervalloTemporale(validita.inizio, validita.fine)
            impianto.superficie = sup_app

            esercizio.validita = New IntervalloTemporale(validita.inizio, validita.fine)

            If isTerrenoNudo Then

                impianto.utilizzoTerreno = New utilizzi.DestinazioneUso(enum_CodiciAnagrafe.UtilizzoImpianto_DaDefinire) With {
                    .descrizione = Str_TerrenoNudo
                }

                impianto.gruppoVarietale = New utilizzi.GruppoVarietale(0, "")

                esercizio.lotto = Str_TerrenoNudo
                esercizio.descrizione = Str_TerrenoNudo
            Else
                Dim DettaglioSemina = CType(risorsaProdotto, dettagli.DettaglioSemina)
                Dim Veg_Cod As Integer = DettaglioSemina.varieta.specie.codice
                Dim Cul_Cod As Integer = DettaglioSemina.varieta.codice
                Dim MetodoProduzione As Integer = IIf(DettaglioSemina.regolamento = 4, 3, DettaglioSemina.regolamento)

                Dim objFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpecieVegetali_R
                Dim Grfi_Cod As Integer = objFinalita.PrimoGrfiCod_from_Vegcod(Veg_Cod, objParametri_Server)

                Dim objCopertura As New AgronicaCoreMetaSchemaDAL.Copertura_R
                Dim Cop_Cod As Integer = objCopertura.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)

                impianto.utilizzoTerreno = New utilizzi.Varieta(DettaglioSemina.varieta.codice) With {
                    .specie = New utilizzi.Specie(Veg_Cod)
                    }
                impianto.utilizzoTerreno.classType = ClassType.Varieta
                impianto.gruppoVarietale = New utilizzi.GruppoVarietale(DettaglioSemina.varieta.codice, "")
                impianto.gruppoFinalita = New utilizzi.GruppoFinalita(Grfi_Cod)
                impianto.copertura = New metaschema.Copertura(Cop_Cod)

                appezzamentoNEW.metodo_Produzione = New MetodoProduzione(MetodoProduzione)
                Dim Regolamento As Integer
                Select Case MetodoProduzione
                    Case enum_MetodoProduzione.Biologico, enum_MetodoProduzione.InConversione
                        Regolamento = enum_Cod_Regolamento.Regolamento_bio
                    Case Else
                        Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
                End Select

                Dim Udm_Cod As Decimal = risorsaProdotto.unitaDiMisura.codice
                Dim Qta As Decimal = risorsaProdotto.quantitaTotaleReale
                Dim P_HA As Decimal = 0
                'Se l'UDM è N.Piante modifico il Numero piante sulla distinta
                Select Case CInt(Udm_Cod)
                    Case enum_UnitaMisura.Num_Piante
                        P_HA = risorsaProdotto.doseHaReale
                End Select

                esercizio.regolamento = New metaschema.Regolamenti(Regolamento)
            End If
            esercizioCDC = esercizio

            'alla fine della fiera chiamo questa funzione
            Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
            objAppezzamento_W.Appezzamento_ScriviModifica(appezzamentoNEW,
                                                          objParametri_Server,
                                                          objParametri_Utenti,
                                                          GiasContext, OpenNewTransaction,
                                                          NoteLog:="Semina con frazionamento (NG)")

            nrApp += 1

        Catch ex As Exception
            Throw New Exception(ex.Message)

        End Try

    End Sub

    Private Shared Sub CancellaCollegamentoParticelle(appezzamento As anagrafiche.Appezzamento,
                                                        Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal OpenNewTransaction As Boolean = True)

        Dim Piva = appezzamento.primaryKey.centroAziendalePK.partitaIva
        Dim Sa_Cod = appezzamento.primaryKey.centroAziendalePK.codice
        Dim Appezza = appezzamento.primaryKey.codice

        Try

            Dim AppezzamentixParticellexMacrousi = (From c In GiasContext.AppezzamentiXParticellexMacrousi
                                                    Where c.Piva = Piva And c.Sa_cod = Sa_Cod And c.Appezza = Appezza).ToList
            GiasContext.AppezzamentiXParticellexMacrousi.RemoveRange(AppezzamentixParticellexMacrousi)

            Dim AppezzamentixParticellexMacrousixUtilizzi = (From c In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo
                                                             Where c.Piva = Piva And c.Sa_cod = Sa_Cod And c.Appezza = Appezza).ToList
            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.RemoveRange(AppezzamentixParticellexMacrousixUtilizzi)

            GiasContext.SaveChanges()

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

#End Region

End Class





