Imports System.Linq
Imports System.Text
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.utente

Public Class Importa_GIAS
    Inherits AgronicaCoreDataProvider.LogProvider

    '########################################################################################
    Public Function GSB_Chiama_CreaMateriePrimeVegetali(ByVal Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
                                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                                        ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                        ByRef Messaggio_di_Ritorno_Opzionale As String
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.Cre_MateriePrimeVegetali()"

        Dim Esitofinale As Boolean = False

        Dim Log_Import As New Text.StringBuilder
        Dim Log_Errori As New Text.StringBuilder
        Dim Log_Riepilogo As New Text.StringBuilder

        Try

            'Ricavo i parametri extra :
            Dim ParametriExtra_Vet As String() = Configurazione_Servizio.Parametri_Extra.Split("|")
            Dim Parametri_Extra_HT As New Hashtable

            For Each parametro As String In ParametriExtra_Vet
                Dim Coppie As String() = parametro.Split("=")
                If Coppie.Length <> 2 Then
                    Throw New Exception("Errore nella lettura dei parametri extra: parametro.Split(=).Count <> 2: " & parametro)
                End If
                Dim key As String = parametro.Split("=")(0).Trim.ToLower
                Dim valore As String = parametro.Split("=")(1).Trim
                Parametri_Extra_HT.Add(key, valore)
            Next

            '------------------------------------------------------------------------------------------
            If IsNothing(Parametri_Extra_HT("Elem_Cod".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio Elem_Cod")
            Else
                If Not IsNumeric(Parametri_Extra_HT("Elem_Cod".ToLower)) Then
                    Throw New Exception("parametro Elem_Cod non configurato correttamente")
                End If
            End If

            If IsNothing(Parametri_Extra_HT("regolamento".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio regolamento")
            Else
                If Not IsNumeric(Parametri_Extra_HT("regolamento".ToLower)) Then
                    Throw New Exception("parametro regolamento non configurato correttamente")
                End If
            End If

            If IsNothing(Parametri_Extra_HT("Flag_1SpecieVarColtivate_2configura".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio Flag_1SpecieVarColtivate_2configura")
            Else
                If Not IsNumeric(Parametri_Extra_HT("Flag_1SpecieVarColtivate_2configura".ToLower)) Then
                    Throw New Exception("parametro Flag_1SpecieVarColtivate_2configura non configurato correttamente")
                End If
            End If

            If IsNothing(Parametri_Extra_HT("PivaSpecieColtivate".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio PivaSpecieColtivate")
            End If

            If IsNothing(Parametri_Extra_HT("Flag_0Tutte_1SoloColtivate".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio Flag_0Tutte_1SoloColtivate")
            Else
                If Not IsNumeric(Parametri_Extra_HT("Flag_0Tutte_1SoloColtivate".ToLower)) Then
                    Throw New Exception("parametro Flag_0Tutte_1SoloColtivate non configurato correttamente")
                End If
            End If

            If IsNothing(Parametri_Extra_HT("Flag_0Altre_1TutteVarieta".ToLower)) Then
                Throw New Exception("manca il parametro obbligatorio Flag_0Altre_1TutteVarieta")
            Else
                If Not IsNumeric(Parametri_Extra_HT("Flag_0Altre_1TutteVarieta".ToLower)) Then
                    Throw New Exception("parametro Flag_0Altre_1TutteVarieta non configurato correttamente")
                End If
            End If
            '------------------------------------------------------------------------------------------

            Dim Flag_Crea_Trasformati As Boolean = False
            Dim Flag_Crea_Semilavorati As Boolean = False
            Dim Flag_Crea_Sementi As Boolean = False

            Dim Flag_0Nessuno_1Bio_2Tutti As Integer

            Dim Flag_1SpecieVarColtivate_2configura As Integer
            Dim Piva_SpecieColtivate As String

            Dim Flag_0Tutte_1SoloColtivate As Integer
            Dim Flag_0Altre_1TutteVarieta As Integer


            Select Case Parametri_Extra_HT("Elem_Cod".ToLower)
                Case 0 'tutti
                    Flag_Crea_Semilavorati = True
                    Flag_Crea_Sementi = True
                    Flag_Crea_Trasformati = True
                Case -10210
                    'valore inventato per selezionare solo sementi e trasformati
                    Flag_Crea_Sementi = True
                    Flag_Crea_Trasformati = True
                Case SEMILAVORATI_VEGETALI
                    Flag_Crea_Semilavorati = True
                Case SEMENTI
                    Flag_Crea_Sementi = True
                Case TRASFORMATI_VEGETALI
                    Flag_Crea_Trasformati = True
            End Select

            Flag_0Nessuno_1Bio_2Tutti = Parametri_Extra_HT("regolamento".ToLower)

            Flag_1SpecieVarColtivate_2configura = Parametri_Extra_HT("Flag_1SpecieVarColtivate_2configura".ToLower)

            Piva_SpecieColtivate = Parametri_Extra_HT("PivaSpecieColtivate".ToLower)

            Flag_0Tutte_1SoloColtivate = Parametri_Extra_HT("Flag_0Tutte_1SoloColtivate".ToLower)

            Flag_0Altre_1TutteVarieta = Parametri_Extra_HT("Flag_0Altre_1TutteVarieta".ToLower)

            If Flag_1SpecieVarColtivate_2configura = 2 Then
                'userà:
                'Flag_0Tutte_1SoloColtivate 
                'Flag_0Altre_1TutteVarieta
            Else
                Flag_0Tutte_1SoloColtivate = -1
                'imposto 1 così poi non va a cercare la varietà altre, ma usa quella letta su db
                Flag_0Altre_1TutteVarieta = 1
            End If

            Crea_MateriePrimeVegetali(Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        objParametri_Server,
                                        objParametri_Utenti,
                                        Piva_SpecieColtivate,
                                        Flag_1SpecieVarColtivate_2configura,
                                        Flag_0Tutte_1SoloColtivate,
                                        Flag_0Altre_1TutteVarieta,
                                        Flag_0Nessuno_1Bio_2Tutti,
                                        Flag_Crea_Trasformati,
                                        Flag_Crea_Semilavorati,
                                        Flag_Crea_Sementi)

            Esitofinale = True

        Catch ex As Exception
            Messaggio_di_Ritorno_Opzionale = NomeRoutine & " - " & ex.Message
            Log_Errori.Append(Messaggio_di_Ritorno_Opzionale)
        Finally

            Try

                Dim objDP As New AgronicaCoreDataProvider.LogProvider
                Dim FileLOG As String

                If Log_Errori.ToString <> "" Then

                    FileLOG = "AgroGSB_CreaMPVegetali_" & objParametri_Server.SuperUserUsername & "_Errori_" & Format(Date.Now, "yyyy_MM_dd_HH_mm") & ".txt"

                    Dim customLOGParams As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
                        .LogDirectory = Configurazione_Servizio.DirectoryLOG,
                        .LogFileName = FileLOG
                    }

                    objDP.Scrivi_LOG(objParametri_Server,
                            NomeRoutine,
                            Log_Errori.ToString,
                       CustomLOGParams:=customLOGParams)

                End If

                If Log_Riepilogo.ToString <> "" Then

                    FileLOG = "AgroGSB_CreaMPVegetali_" & objParametri_Server.SuperUserUsername & "_Riepilogo_" & Format(Date.Now, "yyyy_MM_dd_HH_mm") & ".txt"

                    Dim customLOGParams As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
                        .LogDirectory = Configurazione_Servizio.DirectoryLOG,
                        .LogFileName = FileLOG
                    }

                    objDP.Scrivi_LOG(objParametri_Server,
                            NomeRoutine,
                            vbCrLf & vbCrLf & Log_Riepilogo.ToString,
                       CustomLOGParams:=customLOGParams)

                End If

            Catch ex2 As Exception
                Messaggio_di_Ritorno_Opzionale = NomeRoutine & " - " & ex2.Message
            End Try

        End Try

        Return Esitofinale

    End Function


    '########################################################################################
    Public Sub Crea_MateriePrimeVegetali(ByRef Log_Import As StringBuilder,
                                        ByRef Log_Errori As StringBuilder,
                                        ByRef Log_Riepilogo As StringBuilder,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        ByVal Piva_SpecieColtivate As String,
                                        ByVal Flag_1SpecieVarColtivate_2configura As Integer,
                                        ByVal Flag_0Tutte_1SoloColtivate As Integer,
                                        ByVal Flag_0Altre_1TutteVarieta As Integer,
                                        ByVal Flag_0Nessuno_1Bio_2Tutti As Integer,
                                        ByVal Flag_Crea_Trasformati As Boolean,
                                        ByVal Flag_Crea_Semilavorati As Boolean,
                                        ByVal Flag_Crea_Sementi As Boolean)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.Crea_MateriePrimeVegetali()"
        Dim MessaggioErrore As String = ""

        Try

            Dim Progressivo_Gias As Integer

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Progressivo_Gias = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)

            Dim Piva As String = objParametri_Server.PivaSuperUser


            If Flag_Crea_Sementi Then
                Crea_Sementi_Specie_Varieta_Bio(Log_Import,
                                                Log_Errori,
                                                Log_Riepilogo,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Progressivo_Gias,
                                                Piva,
                                                Piva_SpecieColtivate,
                                                Flag_1SpecieVarColtivate_2configura,
                                                Flag_0Tutte_1SoloColtivate,
                                                Flag_0Altre_1TutteVarieta,
                                                Flag_0Nessuno_1Bio_2Tutti)

            End If

            If Flag_Crea_Semilavorati Then
                Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio(Log_Import,
                                                                            Log_Errori,
                                                                            Log_Riepilogo,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti,
                                                                            Progressivo_Gias,
                                                                            Piva,
                                                                            Piva_SpecieColtivate,
                                                                            SEMILAVORATI_VEGETALI,
                                                                            Flag_1SpecieVarColtivate_2configura,
                                                                            Flag_0Tutte_1SoloColtivate,
                                                                            Flag_0Altre_1TutteVarieta,
                                                                            Flag_0Nessuno_1Bio_2Tutti)
            End If

            If Flag_Crea_Trasformati Then
                Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio(Log_Import,
                                                                            Log_Errori,
                                                                            Log_Riepilogo,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti,
                                                                            Progressivo_Gias,
                                                                            Piva,
                                                                            Piva_SpecieColtivate,
                                                                            TRASFORMATI_VEGETALI,
                                                                            Flag_1SpecieVarColtivate_2configura,
                                                                            Flag_0Tutte_1SoloColtivate,
                                                                            Flag_0Altre_1TutteVarieta,
                                                                            Flag_0Nessuno_1Bio_2Tutti)
            End If


        Catch ex As Exception
            MessaggioErrore = NomeRoutine & " - " & ex.Message
            Log_Riepilogo.Append(" ERRORE " & MessaggioErrore & vbCrLf)
            Log_Errori.Append(" ERRORE " & MessaggioErrore & vbCrLf)
        End Try

    End Sub

    '########################################################################################

    Public Function Crea_MateriaPrima_Specie_Varieta_Regolamento(cultivar As AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta,
                                                                 Flag_0NoBio_1SoloBio_2Entrambi As Integer,
                                                                 objParametri_server As AgronicaCoreParametri,
                                                                 objParametri_utenti As AgronicaCoreParametri,
                                                                 creaSementi As Boolean,
                                                                 creaTrasformati As Boolean,
                                                                 Optional defaultCrea_MateriaPrima As Integer = -1
                                                                 ) As AgronicaCoreModelsSTD.metaschema.CreaProdottiResult

        Dim creazioneResult As New AgronicaCoreModelsSTD.metaschema.CreaProdottiResult
        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        ' Check impostazioni
        ' 0 = Nessuno, 1 = Sementi, 2 =  Trasformati Vegetali, 3 = Tutti e due (default)
        Dim value As Integer
        If defaultCrea_MateriaPrima = -1 Then
            dim imp As Utente_Impostazioni = objImpostazioni.LeggiScalare(
                objParametri_utenti.UsernameOperazione,
                {enum_Impostazioni_Utenti.SUPERUSER_Creazione_Prodotti},
                objParametri_utenti
                ).FirstOrDefault
            value = If(imp IsNot nothing, Integer.Parse(imp.Valore), defaultCrea_MateriaPrima)
        Else
            value = defaultCrea_MateriaPrima
        End If

        If value > 0 Then

            Dim dtMp = objSpecie.Leggi_con_Cul_Des(cultivar.specie.codice, cultivar.codice, "", "", objParametri_server)
            If dtMp.Rows.Count = 1 Then
                cultivar.descrizione = dtMp.Rows(0).Item("Cul_Des")
                cultivar.specie.descrizione = dtMp.Rows(0).Item("Veg_Des")
            End If

            If cultivar.codice = 0 OrElse IsNothing(cultivar.specie) OrElse cultivar.specie.codice = 0 OrElse value = 0 Then
                Return creazioneResult
            End If

            If creaSementi AndAlso (value = 1 OrElse value = 3) Then
                Dim result As Tuple(Of Integer, Integer)
                If Flag_0NoBio_1SoloBio_2Entrambi = 0 OrElse Flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                    result = Crea_Sementi_SpecieVarietaRegolamento(cultivar, enum_Cod_Regolamento.Regolamento_Nessuno,
                                                          objParametri_server, objParametri_utenti)
                    creazioneResult.sementiCreati += result.Item1
                    creazioneResult.sementiEsistenti += result.Item2
                End If
                If Flag_0NoBio_1SoloBio_2Entrambi = 1 OrElse Flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                    result = Crea_Sementi_SpecieVarietaRegolamento(cultivar, enum_Cod_Regolamento.Regolamento_bio,
                                                          objParametri_server, objParametri_utenti)
                    creazioneResult.sementiCreati += result.Item1
                    creazioneResult.sementiEsistenti += result.Item2
                End If
            End If

            If creaTrasformati AndAlso (value = 2 OrElse value = 3) Then
                Dim tmp As Integer
                If Flag_0NoBio_1SoloBio_2Entrambi = 0 OrElse Flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                    tmp += Crea_Trasformati_SpecieVarietaRegolamento(cultivar, enum_Cod_Regolamento.Regolamento_Nessuno,
                                                          objParametri_server, objParametri_utenti)
                    creazioneResult.trasformatiCreati += tmp
                    creazioneResult.trasformatiEsistenti += If(tmp > 0, 0, 1)
                End If
                If Flag_0NoBio_1SoloBio_2Entrambi = 1 OrElse Flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                    tmp += Crea_Trasformati_SpecieVarietaRegolamento(cultivar, enum_Cod_Regolamento.Regolamento_bio,
                                                        objParametri_server, objParametri_utenti)
                    creazioneResult.trasformatiCreati += tmp
                    creazioneResult.trasformatiEsistenti += If(tmp > 0, 0, 1)
                End If
            End If

        End If

        Return creazioneResult
    End Function

    '########################################################################################


    ''' <returns>Una tupla contentente in posizione 1 il numero di sementi create, in posizione 2 quello di sementi trovate già esistenti.</returns>
    Public Function Crea_Sementi_SpecieVarietaRegolamento(cultivar As AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta,
                                                     regolamento As enum_Cod_Regolamento,
                                                     objParametri_server As AgronicaCoreParametri, objParametri_utenti As AgronicaCoreParametri) As Tuple(Of Integer, Integer)
        Dim objXML As New AgronicaCoreXML.XML_Anagrafe
        Dim objMP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objMP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim Cod_Articolo As String
        Dim Mat_Des As String
        Dim flag_BIO As Boolean = False 'If(regolamento = enum_Cod_Regolamento.Regolamento_bio, True, False)

        Dim flag_insert As Boolean
        Dim MatCod_OUT As Integer
        Dim created As Integer = 0
        Dim existing As Integer = 0

        '-- Calcola progressivo
        Dim Piva As String = objParametri_server.PivaSuperUser
        Dim Progressivo_Gias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_utenti)
        Dim Basecode As Integer
        Dim Topcode As Integer
        Calcola_BaseCode_TopCode(Basecode, Topcode, Progressivo_Gias)

        '-- Ricava tipologie sementi per varieta
        Dim dt_tipologieSementi = objMP_R.TipSementiSpecieVarieta_NON_su_MateriePrime(cultivar.codice, cultivar.specie.codice, 0,
                                                                         regolamento, " SpecieVegetali.Gru_Cod > 0 ", "",
                                                                         objParametri_server)

        For Each tipoSemente In dt_tipologieSementi.Rows
            If objMP_R.Esiste_Semente(Piva:="", cultivar.specie.codice, cultivar.codice, tipoSemente.item("Sem_Cod"),
                                      regolamento, " Sa_cod = -1 ", objParametri_server) Then
                existing += 1
                Continue For
            End If

            Cod_Articolo = Right("00" & tipoSemente.Item("Sem_Cod"), 2) &
                               "_" & Right("000" & tipoSemente.Item("Veg_Cod"), 3) &
                                "_" & Right("00000000" & CStr(cultivar.codice), 8)

            Mat_Des = tipoSemente.Item("Veg_Des") & " - " & cultivar.descrizione & " - " & tipoSemente.Item("Sem_Des")

            If regolamento = enum_Cod_Regolamento.Regolamento_bio Then
                Cod_Articolo = "BIO_" & Cod_Articolo
                Mat_Des = Mat_Des & " - BIO"
            End If

            Creazione_Automatica_Semente(objParametri_server, objXML, objMP_W,
                                        flag_insert, MatCod_OUT,
                                        Basecode, Topcode, Piva,
                                        PUBBLICO, Cod_Articolo, Mat_Des,
                                        cultivar.specie.codice, cultivar.codice, tipoSemente.item("Sem_Cod"),
                                        regolamento, flag_BIO)
            created += 1
        Next

        Return New Tuple(Of Integer, Integer)(created, existing)
    End Function

    '########################################################################################

    ''' <param name="cultivar">Cultivar per cui si vuole creare il trasfrmato. I campi descrizione devono essere valorizzati sia per la cultivar che la specie di riferiemnto.</param>
    ''' <param name="regolamento"></param>
    ''' <param name="objParametri_server"></param>
    ''' <param name="objParametri_utenti"></param>
    ''' <returns>1 se il trasformato è stato creato, 0 altrimenti.</returns>
    Public Function Crea_Trasformati_SpecieVarietaRegolamento(cultivar As AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta,
                                                              regolamento As enum_Cod_Regolamento,
                                                              objParametri_server As AgronicaCoreParametri,
                                                              objParametri_utenti As AgronicaCoreParametri) As Integer
        Dim objXML As New AgronicaCoreXML.XML_Anagrafe
        Dim objMP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objMP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim Cod_Articolo As String
        Dim Mat_Des As String
        Dim flag_BIO As Boolean = False 'If(regolamento = enum_Cod_Regolamento.Regolamento_bio, True, False)

        Dim flag_insert As Boolean
        Dim MatCod_OUT As Integer

        '-- Calcola progressivo
        Dim Piva As String = objParametri_server.PivaSuperUser
        Dim Progressivo_Gias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_utenti)
        Dim Basecode As Integer
        Dim Topcode As Integer
        Calcola_BaseCode_TopCode(Basecode, Topcode, Progressivo_Gias)

        If objMP_R.Esiste_TrasformatoVegetale(Piva, cultivar.specie.codice, cultivar.codice,
                                              regolamento, " Sa_cod = -1 ",
                                              objParametri_server) Then
            Return 0
        End If

        Cod_Articolo = Right("000" & cultivar.specie.codice, 3) &
                                   "_" & Right("00000000" & CStr(cultivar.codice), 8)

        Mat_Des = cultivar.specie.descrizione & " - " & cultivar.descrizione

        If regolamento = enum_Cod_Regolamento.Regolamento_bio Then
            Cod_Articolo = "BIO_" & Cod_Articolo
            Mat_Des = Mat_Des & " - BIO"
        End If

        Creazione_Automatica_TrasformatoVegetale(objParametri_server, objXML, objMP_W,
                                                 flag_insert, MatCod_OUT,
                                                 Basecode, Topcode, Piva,
                                                 PUBBLICO, Cod_Articolo, Mat_Des,
                                                 cultivar.specie.codice, cultivar.codice,
                                                 regolamento, flag_BIO)
        Return 1
    End Function

    '########################################################################################
    Public Sub Crea_Sementi_Specie_Varieta_Bio(ByRef Log_Import As StringBuilder,
                                               ByRef Log_Errori As StringBuilder,
                                               ByRef Log_Riepilogo As StringBuilder,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByVal Progressivo_Gias As Integer,
                                               ByVal Piva As String,
                                               ByVal Piva_SpecieColtivate As String,
                                               ByVal Flag_1SpecieVarColtivate_2configura As Integer,
                                               ByVal Flag_0Tutte_1SoloColtivate As Integer,
                                               ByVal Flag_0Altre_1TutteVarieta As Integer,
                                               ByVal Flag_0NoBio_1SoloBio_2Entrambi As Integer)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.Crea_Sementi_Specie_Varieta_Bio()"
        Dim MessaggioErrore As String = ""

        Dim Dt_Specie As DataTable = Nothing
        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R

        Dim num_speciextiposementi As Integer = 0
        Dim num_sementi_create As Integer = 0
        Dim num_sementiNONcreate As Integer = 0
        Dim num_sementi_presenti As Integer = 0

        Dim Basecode As Integer = 0
        Dim Topcode As Integer = 0

        Try

            Dim Regolamento_Cod As Integer = 0
            Select Case Flag_0NoBio_1SoloBio_2Entrambi
                Case 0 ' CONV
                    Regolamento_Cod = enum_Cod_Regolamento.Regolamento_Nessuno
                Case 1 ' BIO
                    Regolamento_Cod = enum_Cod_Regolamento.Regolamento_bio
                Case 2 ' ENTRAMBI
                    Regolamento_Cod = 0
            End Select


            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            If Flag_1SpecieVarColtivate_2configura = 1 Then
                '-----SOLO SPECIE E VARIETA' COLTIVATE----------
                Dt_Specie = objSpecie.Leggi3JoinVarietaColtivate(Piva_SpecieColtivate,
                                                                    0, 0, 0,
                                                                    "", "",
                                                                    objParametri_Server)

            Else
                'configura
                If Flag_0Altre_1TutteVarieta = 0 Then
                    '---------VARIETA' ALTRE -----------
                    If Flag_0Tutte_1SoloColtivate = 0 Then
                        ' TUTTE LE SPECIE
                        Dt_Specie = objSpecie.Leggi2(0, 0,
                                                " Gru_Cod > 0 ", "",
                                                objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE SEMENTI - TUTTE LE SPECIE - VARIETA' ALTRE " & vbCrLf)

                    ElseIf Flag_0Tutte_1SoloColtivate = 1 Then
                        'SPECIE COLTIVATE
                        Dt_Specie = objSpecie.Leggi2JoinSpecieColtivate(Piva_SpecieColtivate,
                                                                    0, 0,
                                                                    "", "",
                                                                    objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE SEMENTI - SPECIE COLTIVATE - VARIETA' ALTRE " & vbCrLf)
                    End If

                Else
                    '-------- TUTTE LE VARIETA' ----------
                    If Flag_0Tutte_1SoloColtivate = 0 Then
                        ' TUTTE LE SPECIE E TUTTE LE VARIETA'
                        '08/10/2019: OTTIMIZZAZIONE
                        'Dt_Specie = objSpecie.Leggi3(0, 0, 0,
                        '                             " Gru_Cod > 0 ", "",
                        '                        objParametri_Server)
                        Dim objVarieta As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                        Dt_Specie = objVarieta.TipSementiSpecieVarieta_NON_su_MateriePrime(0, 0, 0,
                                                                                           Regolamento_Cod,
                                                                                            " SpecieVegetali.Gru_Cod > 0 ", "",
                                                                                            objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE SEMENTI - TUTTE LE SPECIE - TUTTE LE VARIETA' " & vbCrLf)

                    ElseIf Flag_0Tutte_1SoloColtivate = 1 Then
                        'SPECIE COLTIVATE - TUTTE LE VARIETA'

                        '05/03/2021: OTTIMIZZAZIONE
                        'Dt_Specie = objSpecie.Leggi3JoinSpecieColtivateVarietaTutte(Piva_SpecieColtivate,
                        '                                            0, 0, 0,
                        '                                            "", "",
                        '                                            objParametri_Server)
                        Dt_Specie = objSpecie.Leggi3JoinSpecieColtivateVarietaTutte_NON_su_MateriePrime(Piva_SpecieColtivate,
                                                                                                        0, 0, 0,
                                                                                                        Regolamento_Cod,
                                                                                                        "", "",
                                                                                                        objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE SEMENTI - SPECIE COLTIVATE - TUTTE LE VARIETA'" & vbCrLf)
                    End If

                End If

            End If

            If Not IsNothing(Dt_Specie) AndAlso Dt_Specie.Rows.Count > 0 Then

                Log_Riepilogo.Append("P.iva " & Piva & ": " & vbCrLf)

                AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode,
                                                                                   Topcode,
                                                                                    Progressivo_Gias)

                num_speciextiposementi = Dt_Specie.Rows.Count

                Select Case Flag_0NoBio_1SoloBio_2Entrambi
                    Case 0
                        Crea_Sementi_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                       Log_Errori,
                                                       Log_Riepilogo,
                                                       objParametri_Server,
                                                       objParametri_Utenti,
                                                       Progressivo_Gias,
                                                       Piva,
                                                       Piva_SpecieColtivate,
                                                         Flag_0Altre_1TutteVarieta,
                                                         num_speciextiposementi,
                                                         Dt_Specie,
                                                       num_sementi_create,
                                                       num_sementiNONcreate,
                                                       num_sementi_presenti,
                                                       Basecode,
                                                       Topcode,
                                                       False)

                    Case 1
                        Crea_Sementi_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                       Log_Errori,
                                                       Log_Riepilogo,
                                                       objParametri_Server,
                                                       objParametri_Utenti,
                                                       Progressivo_Gias,
                                                       Piva,
                                                       Piva_SpecieColtivate,
                                                         Flag_0Altre_1TutteVarieta,
                                                         num_speciextiposementi,
                                                         Dt_Specie,
                                                       num_sementi_create,
                                                       num_sementiNONcreate,
                                                       num_sementi_presenti,
                                                       Basecode,
                                                       Topcode,
                                                       True)

                    Case 2
                        Crea_Sementi_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                       Log_Errori,
                                                       Log_Riepilogo,
                                                       objParametri_Server,
                                                       objParametri_Utenti,
                                                       Progressivo_Gias,
                                                       Piva,
                                                       Piva_SpecieColtivate,
                                                         Flag_0Altre_1TutteVarieta,
                                                         num_speciextiposementi,
                                                         Dt_Specie,
                                                       num_sementi_create,
                                                       num_sementiNONcreate,
                                                       num_sementi_presenti,
                                                       Basecode,
                                                       Topcode,
                                                        False)

                        Crea_Sementi_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                       Log_Errori,
                                                       Log_Riepilogo,
                                                       objParametri_Server,
                                                       objParametri_Utenti,
                                                       Progressivo_Gias,
                                                       Piva,
                                                       Piva_SpecieColtivate,
                                                         Flag_0Altre_1TutteVarieta,
                                                         num_speciextiposementi,
                                                         Dt_Specie,
                                                       num_sementi_create,
                                                       num_sementiNONcreate,
                                                       num_sementi_presenti,
                                                       Basecode,
                                                       Topcode,
                                                        True)

                End Select

                Log_Riepilogo.Append(vbCrLf)

            End If

            Log_Riepilogo.Append(vbCrLf & vbCrLf)
            Log_Riepilogo.Append("Num. specie vegetali x tipologia sementi: " & CStr(num_speciextiposementi) & vbCrLf)
            Log_Riepilogo.Append("Num. sementi creati: " & CStr(num_sementi_create) & vbCrLf)
            Log_Riepilogo.Append("Num. sementi non creati: " & CStr(num_sementiNONcreate) & vbCrLf)
            Log_Riepilogo.Append("Num. sementi già presenti: " & CStr(num_sementi_presenti) & vbCrLf)

            objParametri_Server.ResettaFinestra()

        Catch ex As Exception
            MessaggioErrore = NomeRoutine & " - " & ex.Message
            Log_Riepilogo.Append(" ERRORE " & MessaggioErrore & vbCrLf)
            Log_Errori.Append(" ERRORE " & MessaggioErrore & vbCrLf)
        End Try



    End Sub

    '########################################################################################
    Private Sub Crea_Sementi_Specie_Varieta_Bio_innerfunction(ByRef Log_Import As StringBuilder,
                                                              ByRef Log_Errori As StringBuilder,
                                                              ByRef Log_Riepilogo As StringBuilder,
                                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                              ByVal Progressivo_Gias As Integer,
                                                              ByVal Piva As String,
                                                              ByVal Piva_SpecieColtivate As String,
                                                              ByVal Flag_0Altre_1TutteVarieta As Integer,
                                                              ByVal num_speciextiposementi As Integer,
                                                              ByRef Dt_Specie As DataTable,
                                                              ByRef num_sementi_create As Integer,
                                                              ByRef num_sementiNONcreate As Integer,
                                                              ByRef num_sementi_presenti As Integer,
                                                              ByVal Basecode As Integer,
                                                              ByVal Topcode As Integer,
                                                              ByVal Flag_Bio As Boolean)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.Crea_Sementi_Specie_Varieta_Bio()"

        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Dim SaCod_Visibilita As Integer
        Dim Cod_Articolo As String
        Dim Mat_Des As String
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Cul_Des As String
        Dim Sem_Cod As Integer
        Dim Regolamento As enum_Cod_Regolamento
        Dim Flag_Biologico As Boolean

        Dim i As Integer
        Dim Flag_Insert As Boolean
        Dim flag_esiste As Boolean

        Dim OUTPUT_Mat_Cod As Integer = 0

        Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R

        For i = 0 To num_speciextiposementi - 1

            Try

                OUTPUT_Mat_Cod = 0

                Veg_Cod = Dt_Specie.Rows(i).Item("Veg_Cod")
                Sem_Cod = Dt_Specie.Rows(i).Item("Sem_Cod")

                If Flag_0Altre_1TutteVarieta = 0 Then
                    Cul_Cod = objVarieta.VarietaAltre(Veg_Cod, objParametri_Server)
                    Cul_Des = "Altre"
                Else
                    Cul_Cod = Dt_Specie.Rows(i).Item("Cul_Cod")
                    Cul_Des = Dt_Specie.Rows(i).Item("Cul_Des")
                End If

                If Cul_Cod <> 0 Then
                    'ok varietà altre

                    SaCod_Visibilita = PUBBLICO
                    Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
                    Flag_Biologico = False

                    Cod_Articolo = Right("00" & Dt_Specie.Rows(i).Item("Sem_Cod"), 2) &
                                   "_" & Right("000" & Dt_Specie.Rows(i).Item("Veg_Cod"), 3) &
                                    "_" & Right("00000000" & CStr(Cul_Cod), 8)

                    'Mat_Des = Dt_Specie.Rows(i).Item("Sem_Des") & " - " & Dt_Specie.Rows(i).Item("Veg_Des") & " - " & Cul_Des
                    Mat_Des = Dt_Specie.Rows(i).Item("Veg_Des") & " - " & Cul_Des & " - " & Dt_Specie.Rows(i).Item("Sem_Des")

                    If Flag_Bio Then
                        Cod_Articolo = "BIO_" & Cod_Articolo
                        Mat_Des = Mat_Des & " - BIO"
                        Regolamento = enum_Cod_Regolamento.Regolamento_bio
                    End If

                    flag_esiste = objCore_MP_R.Esiste_Semente(Piva,
                                                            Veg_Cod,
                                                            Cul_Cod,
                                                            Sem_Cod,
                                                              Regolamento,
                                                            " Sa_cod = -1 ",
                                                            objParametri_Server)

                    If Not flag_esiste Then

                        Creazione_Automatica_Semente(objParametri_Server,
                                                                objCore_XML_Anagrafe,
                                                                 objCore_MP_W,
                                                                 Flag_Insert,
                                                                 OUTPUT_Mat_Cod,
                                                                 Basecode,
                                                                 Topcode,
                                                                 Piva,
                                                                 SaCod_Visibilita,
                                                                 Cod_Articolo,
                                                                 Mat_Des,
                                                                 Veg_Cod,
                                                                 Cul_Cod,
                                                                 Sem_Cod,
                                                                 Regolamento,
                                                                 Flag_Biologico)

                        Log_Riepilogo.Append("Inserito: " & Mat_Des & " (" & Cod_Articolo & ") - mat_cod =" & CStr(OUTPUT_Mat_Cod) & vbCrLf)

                        num_sementi_create += 1

                    Else
                        num_sementi_presenti += 1
                        Log_Riepilogo.Append("Già presente: " & Mat_Des & vbCrLf)
                    End If

                Else
                    'varietà non trovata
                    Log_Riepilogo.Append(Dt_Specie.Rows(i).Item("Veg_Des") & ": Varietà non trovata. " & vbCrLf)
                    Log_Errori.Append(Dt_Specie.Rows(i).Item("Veg_Des") & ": Varietà non trovata. " & vbCrLf)
                    num_sementiNONcreate += 1
                End If

            Catch ex As Exception
                Log_Riepilogo.Append(" ERRORE " & ex.Message & vbCrLf)
                Log_Errori.Append(" ERRORE " & ex.Message & vbCrLf)
                num_sementiNONcreate += 1
            End Try

        Next

    End Sub

    '########################################################################################
    Public Sub Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio(ByRef Log_Import As StringBuilder,
                                                                       ByRef Log_Errori As StringBuilder,
                                                                       ByRef Log_Riepilogo As StringBuilder,
                                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                       ByVal Progressivo_Gias As Integer,
                                                                       ByVal Piva As String,
                                                                       ByVal Piva_SpecieColtivate As String,
                                                                       ByVal Elem_Cod As Integer,
                                                                       ByVal Flag_1SpecieVarColtivate_2configura As Integer,
                                                                       ByVal Flag_0Tutte_1SoloColtivate As Integer,
                                                                       ByVal Flag_0Altre_1TutteVarieta As Integer,
                                                                       ByVal Flag_0NoBio_1SoloBio_2Entrambi As Integer)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.Crea_SemilavoratiVegetali_Specie_Varieta_Bio()"
        Dim MessaggioErrore As String = ""

        Dim Dt_Specie As DataTable = Nothing

        Dim num_specie As Integer = 0
        Dim num_semilavorati_creati As Integer = 0
        Dim num_semilavoratiNONcreati As Integer = 0
        Dim num_semilavorati_presenti As Integer = 0

        Dim Basecode As Integer = 0
        Dim Topcode As Integer = 0
        Dim Elem_Des As String = ""

        Try

            Log_Riepilogo.Append(vbCrLf)
            Log_Riepilogo.Append(vbCrLf)
            Log_Riepilogo.Append("============================================================" & vbCrLf)

            Select Case Elem_Cod
                Case SEMILAVORATI_VEGETALI
                    Elem_Des = "Semilavorati"
                Case TRASFORMATI_VEGETALI
                    Elem_Des = "Trasformati"
            End Select

            Dim Regolamento_Cod As Integer = 0
            Select Case Flag_0NoBio_1SoloBio_2Entrambi
                Case 0 ' CONV
                    Regolamento_Cod = enum_Cod_Regolamento.Regolamento_Nessuno
                Case 1 ' BIO
                    Regolamento_Cod = enum_Cod_Regolamento.Regolamento_bio
                Case 2 ' ENTRAMBI
                    Regolamento_Cod = 0
            End Select


            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            If Flag_1SpecieVarColtivate_2configura = 1 Then
                '-----SOLO SPECIE E VARIETA' COLTIVATE----------
                Dim objSpecie As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dt_Specie = objSpecie.Distinct_SpecieVarietaColtivate(Piva_SpecieColtivate,
                                            "", "",
                                            objParametri_Server)
                Log_Riepilogo.Append("CREAZIONE " & Elem_Des.ToUpper & " VEGETALI - SOLO SPECIE E VARIETA' COLTIVATE " & vbCrLf)
            Else
                'CONFIGURA
                If Flag_0Altre_1TutteVarieta = 0 Then
                    '---------VARIETA' ALTRE -----------
                    If Flag_0Tutte_1SoloColtivate = 0 Then
                        'TUTTE LE SPECIE
                        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                        Dt_Specie = objSpecie.Leggi(0, 0, "", "",
                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                " Gru_Cod > 0 ", "",
                                                objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE " & Elem_Des.ToUpper & " VEGETALI - TUTTE LE SPECIE - VARIETA' ALTRE " & vbCrLf)

                    ElseIf Flag_0Tutte_1SoloColtivate = 1 Then
                        'SPECIE COLTIVATE
                        Dim objSpecie As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dt_Specie = objSpecie.Distinct_SpecieColtivate(Piva_SpecieColtivate,
                                                    "", "",
                                                    objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE " & Elem_Des.ToUpper & " VEGETALI - SPECIE COLTIVATE - VARIETA' ALTRE" & vbCrLf)
                    End If

                Else
                    '-------- TUTTE LE VARIETA' ----------
                    If Flag_0Tutte_1SoloColtivate = 0 Then
                        ' TUTTE LE SPECIE E TUTTE LE VARIETA'
                        '08/10/2019: OTTIMIZZAZIONE
                        'Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                        'Dt_Specie = objVarieta.Leggi(0, 0, "",
                        '                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                        '                        " Gru_Cod > 0 ", "",
                        '                        objParametri_Server)
                        Dim objVarieta As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                        Dt_Specie = objVarieta.SpecieVarieta_NON_su_MateriePrime(0, 0, "",
                                                                                  Elem_Cod,
                                                                                  Regolamento_Cod,
                                                                                " Gru_Cod > 0 ", "",
                                                                                objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE " & Elem_Des.ToUpper & " VEGETALI - TUTTE LE SPECIE - TUTTE LE VARIETA' " & vbCrLf)

                    ElseIf Flag_0Tutte_1SoloColtivate = 1 Then
                        'SPECIE COLTIVATE - TUTTE LE VARIETA'
                        Dim objSpecie As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        'Dt_Specie = objSpecie.Distinct_SpecieColtivateVarietaTutte(Piva_SpecieColtivate,
                        '                            "", "",
                        '                            objParametri_Server)
                        Dt_Specie = objSpecie.Distinct_SpecieColtivateVarietaTutte(Piva_SpecieColtivate,
                                                                                   Regolamento_Cod,
                                                                                    "", "",
                                                                                    objParametri_Server)

                        Log_Riepilogo.Append("CREAZIONE " & Elem_Des.ToUpper & " VEGETALI - SPECIE COLTIVATE - TUTTE LE VARIETA'" & vbCrLf)
                    End If
                End If
            End If

            Log_Riepilogo.Append("P.iva " & Piva & ": " & vbCrLf)

            If Not IsNothing(Dt_Specie) AndAlso Dt_Specie.Rows.Count > 0 Then

                AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode,
                                                                                   Topcode,
                                                                                    Progressivo_Gias)

                num_specie = Dt_Specie.Rows.Count

                Select Case Flag_0NoBio_1SoloBio_2Entrambi
                    Case 0
                        Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                         Log_Errori,
                                                         Log_Riepilogo,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            Progressivo_Gias,
                                                            Piva,
                                                            Piva_SpecieColtivate,
                                                            Elem_Cod,
                                                            Flag_0Altre_1TutteVarieta,
                                                                num_specie,
                                                               Dt_Specie,
                                                             num_semilavorati_creati,
                                                             num_semilavoratiNONcreati,
                                                             num_semilavorati_presenti,
                                                             Basecode,
                                                             Topcode,
                                                               False)

                    Case 1
                        Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                         Log_Errori,
                                                         Log_Riepilogo,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            Progressivo_Gias,
                                                            Piva,
                                                            Piva_SpecieColtivate,
                                                            Elem_Cod,
                                                            Flag_0Altre_1TutteVarieta,
                                                                num_specie,
                                                               Dt_Specie,
                                                             num_semilavorati_creati,
                                                             num_semilavoratiNONcreati,
                                                             num_semilavorati_presenti,
                                                             Basecode,
                                                             Topcode,
                                                               True)

                    Case 2
                        Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                         Log_Errori,
                                                         Log_Riepilogo,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            Progressivo_Gias,
                                                            Piva,
                                                            Piva_SpecieColtivate,
                                                            Elem_Cod,
                                                            Flag_0Altre_1TutteVarieta,
                                                                num_specie,
                                                               Dt_Specie,
                                                             num_semilavorati_creati,
                                                             num_semilavoratiNONcreati,
                                                             num_semilavorati_presenti,
                                                             Basecode,
                                                             Topcode,
                                                               False)

                        Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio_innerfunction(Log_Import,
                                                         Log_Errori,
                                                         Log_Riepilogo,
                                                            objParametri_Server,
                                                            objParametri_Utenti,
                                                            Progressivo_Gias,
                                                            Piva,
                                                            Piva_SpecieColtivate,
                                                            Elem_Cod,
                                                            Flag_0Altre_1TutteVarieta,
                                                                num_specie,
                                                               Dt_Specie,
                                                             num_semilavorati_creati,
                                                             num_semilavoratiNONcreati,
                                                             num_semilavorati_presenti,
                                                             Basecode,
                                                             Topcode,
                                                               True)
                End Select

                Log_Riepilogo.Append(vbCrLf)

            End If

            Log_Riepilogo.Append(vbCrLf & vbCrLf)
            Log_Riepilogo.Append("Num. specie vegetali coltivate: " & CStr(num_specie) & vbCrLf)
            Log_Riepilogo.Append("Num. " & Elem_Des.ToUpper & " creati: " & CStr(num_semilavorati_creati) & vbCrLf)
            Log_Riepilogo.Append("Num. " & Elem_Des.ToUpper & " non creati: " & CStr(num_semilavoratiNONcreati) & vbCrLf)
            Log_Riepilogo.Append("Num. " & Elem_Des.ToUpper & " già presenti: " & CStr(num_semilavorati_presenti) & vbCrLf)
            Log_Riepilogo.Append("============================================================" & vbCrLf)
            Log_Riepilogo.Append(vbCrLf)
            Log_Riepilogo.Append(vbCrLf)

            objParametri_Server.ResettaFinestra()

        Catch ex As Exception
            MessaggioErrore = NomeRoutine & " - " & ex.Message
            Log_Riepilogo.Append(" ERRORE " & MessaggioErrore & vbCrLf)
            Log_Errori.Append(" ERRORE " & MessaggioErrore & vbCrLf)
        End Try



    End Sub

    '########################################################################################
    Private Sub Crea_SemilavoratiTrasformatiVegetali_Specie_Varieta_Bio_innerfunction(ByRef Log_Import As StringBuilder,
                                                                    ByRef Log_Errori As StringBuilder,
                                                                    ByRef Log_Riepilogo As StringBuilder,
                                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                       ByVal Progressivo_Gias As Integer,
                                                                       ByVal Piva As String,
                                                                       ByVal Piva_SpecieColtivate As String,
                                                                       ByVal Elem_Cod As Integer,
                                                                       ByVal Flag_0Altre_1TutteVarieta As Integer,
                                                                           ByVal num_specie As Integer,
                                                                          ByRef Dt_Specie As DataTable,
                                                                        ByRef num_semilavorati_creati As Integer,
                                                                        ByRef num_semilavoratiNONcreati As Integer,
                                                                        ByRef num_semilavorati_presenti As Integer,
                                                                        ByVal Basecode As Integer,
                                                                        ByVal Topcode As Integer,
                                                                          ByVal Flag_Bio As Boolean)

        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim OUTPUT_Mat_Cod As Integer = 0

        Dim SaCod_Visibilita As Integer
        Dim Cod_Articolo As String
        Dim Mat_Des As String
        Dim Veg_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Cul_Des As String
        Dim Regolamento As enum_Cod_Regolamento
        Dim Flag_Biologico As Boolean

        Dim i As Integer
        Dim Flag_Insert As Boolean
        Dim flag_esiste As Boolean

        For i = 0 To num_specie - 1

            Try

                OUTPUT_Mat_Cod = 0

                Veg_Cod = Dt_Specie.Rows(i).Item("Veg_Cod")

                If Flag_0Altre_1TutteVarieta = 0 Then
                    Cul_Cod = objVarieta.VarietaAltre(Veg_Cod, objParametri_Server)
                    Cul_Des = "Altre"
                Else
                    Cul_Cod = Dt_Specie.Rows(i).Item("Cul_Cod")
                    Cul_Des = Dt_Specie.Rows(i).Item("Cul_Des")
                End If

                If Cul_Cod <> 0 Then
                    'ok varietà altre

                    SaCod_Visibilita = PUBBLICO
                    Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
                    Flag_Biologico = False

                    'Cod_Articolo = Left(Dt_Specie.Rows(i).Item("Veg_Des"), 4) & CStr(Veg_Cod)
                    Cod_Articolo = Right("000" & Dt_Specie.Rows(i).Item("Veg_Cod"), 3) &
                                   "_" & Right("00000000" & CStr(Cul_Cod), 8)

                    Mat_Des = Dt_Specie.Rows(i).Item("Veg_Des") & " - " & Cul_Des

                    If Flag_Bio Then
                        Cod_Articolo = "BIO_" & Cod_Articolo
                        Mat_Des = Mat_Des & " - BIO"
                        Regolamento = enum_Cod_Regolamento.Regolamento_bio
                    End If

                    Select Case Elem_Cod
                        Case SEMILAVORATI_VEGETALI
                            flag_esiste = objCore_MP_R.Esiste_SemilavoratoVegetale(Piva,
                                                                Veg_Cod,
                                                                Cul_Cod,
                                                                Regolamento,
                                                                 " Sa_cod = -1 ",
                                                                 objParametri_Server)

                        Case TRASFORMATI_VEGETALI
                            flag_esiste = objCore_MP_R.Esiste_TrasformatoVegetale(Piva,
                                                                Veg_Cod,
                                                                Cul_Cod,
                                                                Regolamento,
                                                                 " Sa_cod = -1 ",
                                                                 objParametri_Server)
                    End Select



                    If Not flag_esiste Then

                        Select Case Elem_Cod
                            Case SEMILAVORATI_VEGETALI
                                Creazione_Automatica_SemilavoratoVegetale(objParametri_Server,
                                                           objCore_XML_Anagrafe,
                                                            objCore_MP_W,
                                                            Flag_Insert,
                                                            OUTPUT_Mat_Cod,
                                                            Basecode,
                                                            Topcode,
                                                            Piva,
                                                            SaCod_Visibilita,
                                                            Cod_Articolo,
                                                            Mat_Des,
                                                            Veg_Cod,
                                                            Cul_Cod,
                                                            Regolamento,
                                                            Flag_Biologico)
                            Case TRASFORMATI_VEGETALI
                                Creazione_Automatica_TrasformatoVegetale(objParametri_Server,
                                                           objCore_XML_Anagrafe,
                                                            objCore_MP_W,
                                                            Flag_Insert,
                                                            OUTPUT_Mat_Cod,
                                                            Basecode,
                                                            Topcode,
                                                            Piva,
                                                            SaCod_Visibilita,
                                                            Cod_Articolo,
                                                            Mat_Des,
                                                            Veg_Cod,
                                                            Cul_Cod,
                                                            Regolamento,
                                                            Flag_Biologico)
                        End Select


                        Log_Riepilogo.Append("Inserito: " & Mat_Des & " (" & Cod_Articolo & ") - mat_cod =" & CStr(OUTPUT_Mat_Cod) & vbCrLf)

                        num_semilavorati_creati += 1

                    Else
                        num_semilavorati_presenti += 1
                        Log_Riepilogo.Append("Già presente: " & Mat_Des & vbCrLf)
                    End If

                Else
                    'varietà non trovata
                    Log_Riepilogo.Append(Dt_Specie.Rows(i).Item("Veg_Des") & ": Varietà non trovata. " & vbCrLf)
                    Log_Errori.Append(Dt_Specie.Rows(i).Item("Veg_Des") & ": Varietà non trovata. " & vbCrLf)
                    num_semilavoratiNONcreati += 1
                End If

            Catch ex As Exception
                Log_Riepilogo.Append(" ERRORE " & ex.Message & vbCrLf)
                Log_Errori.Append(" ERRORE " & ex.Message & vbCrLf)
                num_semilavoratiNONcreati += 1
            End Try

        Next


    End Sub


    '########################################################################################
    Public Sub Creazione_Automatica_Semente(ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                            ByRef objCore_MP_W As AgronicaCoreAnagrafeBIZ.Materie_Prime_W,
                                            ByRef Flag_Insert As Boolean,
                                            ByRef OUTPUT_Mat_Cod As Integer,
                                            ByVal Basecode As Integer,
                                            ByVal Topcode As Integer,
                                            ByVal Piva As String,
                                            ByVal SaCod_Visibilita As Integer,
                                            ByVal Cod_Articolo As String,
                                            ByVal Mat_Des As String,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Cul_Cod As Integer,
                                            ByVal Sem_Cod As Integer,
                                            ByVal Regolamento As enum_Cod_Regolamento,
                                            ByVal Flag_Biologico As Boolean)

        Dim XmlDoc As New XmlDocument
        Dim XmlDatiMateriePrime As XmlElement
        Dim Log As String = ""
        Dim ErroreMessaggio As String = ""

        XmlDatiMateriePrime = objCore_XML_Anagrafe.XML_MateriePrime(
                                ErroreMessaggio,
                                XmlDoc,
                                Basecode,
                                Topcode,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                objParametri_Server.PivaSuperUser,
                                Piva,
                                SEMENTI,
                                0,
                                Cod_Articolo,
                                Mat_Des,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                SaCod_Visibilita,
                                Sem_Cod,
                                Veg_Cod,
                                Cul_Cod,
                                , , , , , , , , , , , , ,
                                Regolamento,
                                ,
                                Flag_Biologico,
                                , , , , , , , , , , , , ,
                                "",
                                , , , , , , )


        If ErroreMessaggio <> "" Then

            Flag_Insert = False
            Throw New Exception("Creazione semente: errore durante la creazione dell'xml " & Log)
        Else

            Dim Str_XML_MP As String = ""

            Str_XML_MP = XmlDoc.OuterXml

            Flag_Insert = objCore_MP_W.Materia_Prima_Scrivi(Str_XML_MP,
                                                        OUTPUT_Mat_Cod,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        objParametri_Server)

        End If



    End Sub

    '########################################################################################
    Public Sub Creazione_Automatica_SemilavoratoVegetale(ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                                         ByRef objCore_MP_W As AgronicaCoreAnagrafeBIZ.Materie_Prime_W,
                                                         ByRef Flag_Insert As Boolean,
                                                         ByRef OUTPUT_Mat_Cod As Integer,
                                                         ByVal Basecode As Integer,
                                                         ByVal Topcode As Integer,
                                                         ByVal Piva As String,
                                                         ByVal SaCod_Visibilita As Integer,
                                                         ByVal Cod_Articolo As String,
                                                         ByVal Mat_Des As String,
                                                         ByVal Veg_Cod As Integer,
                                                         ByVal Cul_Cod As Integer,
                                                         ByVal Regolamento As enum_Cod_Regolamento,
                                                         ByVal Flag_Biologico As Boolean)




        Dim XmlDoc As New XmlDocument
        Dim XmlDatiMateriePrime As XmlElement
        Dim Log As String = ""
        Dim ErroreMessaggio As String = ""

        XmlDatiMateriePrime = objCore_XML_Anagrafe.XML_MateriePrime(
                                ErroreMessaggio,
                                XmlDoc,
                                Basecode,
                                Topcode,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                objParametri_Server.PivaSuperUser,
                                Piva,
                                SEMILAVORATI_VEGETALI,
                                0,
                                Cod_Articolo,
                                Mat_Des,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                SaCod_Visibilita,
                                0,
                                Veg_Cod,
                                Cul_Cod,
                                , , , , , , , , , , , , ,
                                Regolamento,
                                ,
                                Flag_Biologico,
                                , , , , , , , , , , , , ,
                                "",
                                , , , , , , )


        If ErroreMessaggio <> "" Then

            Flag_Insert = False
            Throw New Exception("Creazione semilavorato vegetale: errore durante la creazione dell'xml " & Log)
        Else

            Dim Str_XML_MP As String = ""

            Str_XML_MP = XmlDoc.OuterXml

            Flag_Insert = objCore_MP_W.Materia_Prima_Scrivi(Str_XML_MP,
                                                        OUTPUT_Mat_Cod,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        objParametri_Server)

        End If



    End Sub



    '########################################################################################
    Public Sub Creazione_Automatica_TrasformatoVegetale(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                                        ByRef objCore_MP_W As AgronicaCoreAnagrafeBIZ.Materie_Prime_W,
                                                        ByRef Flag_Insert As Boolean,
                                                        ByRef OUTPUT_Mat_Cod As Integer,
                                                        ByVal Basecode As Integer,
                                                        ByVal Topcode As Integer,
                                                        ByVal Piva As String,
                                                        ByVal SaCod_Visibilita As Integer,
                                                        ByVal Cod_Articolo As String,
                                                        ByVal Mat_Des As String,
                                                        ByVal Veg_Cod As Integer,
                                                        ByVal Cul_Cod As Integer,
                                                        ByVal Regolamento As enum_Cod_Regolamento,
                                                        ByVal Flag_Biologico As Boolean)




        Dim XmlDoc As New XmlDocument
        Dim XmlDatiMateriePrime As XmlElement
        Dim Log As String = ""
        Dim ErroreMessaggio As String = ""

        XmlDatiMateriePrime = objCore_XML_Anagrafe.XML_MateriePrime(
                                ErroreMessaggio,
                                XmlDoc,
                                Basecode,
                                Topcode,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_TipoOperazioneDB.Scrittura,
                                objParametri_Server.PivaSuperUser,
                                Piva,
                                TRASFORMATI_VEGETALI,
                                0,
                                Cod_Articolo,
                                Mat_Des,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                Nothing,
                                SaCod_Visibilita,
                                0,
                                Veg_Cod,
                                Cul_Cod,
                                , , , , , , , , , , , , ,
                                Regolamento,
                                ,
                                Flag_Biologico,
                                , , , , , , , , , , , , ,
                                "",
                                , , , , , , )


        If ErroreMessaggio <> "" Then

            Flag_Insert = False
            Throw New Exception("Creazione semilavorato vegetale: errore durante la creazione dell'xml " & Log)
        Else

            Dim Str_XML_MP As String = ""

            Str_XML_MP = XmlDoc.OuterXml

            Flag_Insert = objCore_MP_W.Materia_Prima_Scrivi(Str_XML_MP,
                                                        OUTPUT_Mat_Cod,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        objParametri_Server)

        End If



    End Sub



    Public Function Crea_Macchina(ByVal PIVA As String, ByVal Class_Code As String,
                            ByVal Mac_Des As String, ByVal Costo_Acquisto As Decimal,
                            ByVal Targa As String, ByVal Telaio As String, ByVal Ditta_Cod As Integer,
                            ByVal Modello As String, ByVal Potenza As String, ByVal Ammortamento As Decimal,
                            ByVal Ammortizzato As Decimal, ByVal Data_Immatricolazione As Date,
                            ByVal Ultima_Manutenzione As Date, ByVal Data_Revisione As Date,
                            ByVal Stato_Utilizzo As String, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                            ByVal BaseCode As Integer, ByVal TopCode As Integer, ByVal Note As String,
                            ByVal Tipo As Integer, ByVal N_Immatricolazione As String, ByVal N_Immatricolazione_Rimorchio As String,
                            ByVal N_Autorizzazione_Trasporto As String, ByVal Data_Rilascio_Autorizzazione As Date,
                            ByVal Peso As Decimal, ByVal Mac_Cod_Origine As Integer, ByVal Piva_SuperUser_Origine As String,
                            ByVal ChkDefault As Integer, ByVal Portata_Max As Decimal, ByVal Cod_Contatto As String,
                            ByVal Alimentazione_Cod As Integer, ByVal Potenza_Udm_Cod As Integer,
                            ByVal CUAA_Proprietario As String, ByVal Denominazione_Proprietario As String,
                            ByVal Tipo_Targa_Cod As Integer, ByVal Tipo_Trazione_Cod As Integer, ByVal N_Omologazione As String, ByVal Ditta_Cod_Motore As Integer,
                            ByVal Tipo_Motore As String, ByVal Matricola_Motore As String,
                            ByVal Data_Reimmatricolazione As Date, ByVal Data_Carico As Date,
                            ByVal Data_Scarico As Date, ByVal TitoloPossesso As Integer,
                            ByVal Flag_Attrezzatura_Macchina As String, ByVal Taratura_Ugello As Decimal,
                            ByRef objparametriserver As AgronicaCoreParametri,
                            Optional ByVal Sa_Cod As Integer = 0
                                  ) As Integer

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim OperazioneMacchina As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        '------------------------------------------------
        'Leggo il nuovo Mac_Cod
        '(mi serve dopo x salvare il record della modifica)
        '------------------------------------------------
        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim Mac_Cod As Integer = ObjSequenze.NuovoId_Tabella(
                                    "Parco_Macchine",
                                            CInt(BaseCode),
                                    CInt(TopCode),
                                objparametriserver)

        ObjSequenze = Nothing

        '---------------------------
        'nodo AGENDA
        Dim LogErrori As String = ""
        Dim XmlDatiAgenda As XmlElement
        Dim objXML_Contab As New AgronicaCoreXML.XML_Contab
        Dim Dt_Agenda As New DataTable
        XmlDatiAgenda = objXML_Contab.XML_Agenda_Agenda__DATI(LogErrori, XmlDoc, Dt_Agenda)


        Dt_Agenda = objXML_Contab.DtForXml_Genera_Agenda

        objXML_Contab.DtForXml_InserisciRiga_Agenda(Dt_Agenda,
                                                    enum_TipoOperazioneDB.Scrittura,
                                                    PIVA,
                                                    ,
                                                    ,
                                                    LAVCOD_ACQUISTO_BENI,
                                                    , , , , ,
                                                    , ,
                                                    BaseCode, TopCode)

        Dim XmlAgenda As System.Xml.XmlElement
        XmlAgenda = objXML_Contab.XML_Agenda_Agenda(LogErrori,
                                                    XmlDoc,
                                                    Dt_Agenda.Rows(0))

        XmlDatiAgenda.AppendChild(XmlAgenda)

        '---------------------------
        'nodo MOVIMENTO
        Dim XmlDatiMovimenti As System.Xml.XmlElement
        Dim Dt_Movimenti As New DataTable
        XmlDatiMovimenti = objXML_Contab.XML_Agenda_Movimenti__DATI(LogErrori, XmlDoc, Dt_Movimenti)

        XmlAgenda.AppendChild(XmlDatiMovimenti)


        Dt_Movimenti = objXML_Contab.DtForXml_Genera_Movimenti

        objXML_Contab.DtForXml_InserisciRiga_Movimenti(Dt_Movimenti,
                                                       enum_TipoOperazioneDB.Scrittura,
                                                       PIVA,
                                                       , , ,
                                                       CAU_CARICO,
                                                       , , , , , , , , , , , , , , , , , , , , , , , , , , , , , ,
                                                       AGRODATAINIZIO, AGRODATAFINE,
                                                       BaseCode, TopCode)

        Dim XmlMovimenti As System.Xml.XmlElement
        XmlMovimenti = objXML_Contab.XML_Agenda_Movimenti(LogErrori,
                                                          XmlDoc,
                                                          Dt_Movimenti.Rows(0))


        XmlDatiMovimenti.AppendChild(XmlMovimenti)

        '---------------------------
        'nodo DETTAGLIO

        Dim XmlDatiDettagli As System.Xml.XmlElement
        Dim Dt_Dettagli As New DataTable
        XmlDatiDettagli = objXML_Contab.XML_Agenda_MovimentiDettagli__DATI(LogErrori, XmlDoc, Dt_Dettagli)

        XmlMovimenti.AppendChild(XmlDatiDettagli)


        Dt_Dettagli = objXML_Contab.DtForXml_Genera_MovimentiDettagli

        objXML_Contab.DtForXml_InserisciRiga_MovimentiDettagli(Dt_Dettagli,
                                                       enum_TipoOperazioneDB.Scrittura,
                                                       PIVA,
                                                       , , , , ,
                                                       MACCHINE, ,
                                                       Mac_Cod, , , , ,
                                                       38,
                                                       ,
                                                       1,
                                                       , , , , , , , , , , , , ,
                                                       NONCONTABILE,
                                                       enum_Pendenza.GiacenzeIniziali,
                                                       , , ,
                                                       AGRODATAINIZIO, AGRODATAFINE,
                                                       , , , , , , ,
                                                       BaseCode, TopCode)

        Dim XmlDettagli As System.Xml.XmlElement
        XmlDettagli = objXML_Contab.XML_Agenda_MovimentiDettagli(LogErrori,
                                                          XmlDoc,
                                                          Dt_Dettagli.Rows(0))

        XmlDatiDettagli.AppendChild(XmlDettagli)


        '---------------------------
        'nodo PARCO MACCHINE
        Dim XmlDatiParcoMacchine As System.Xml.XmlElement
        Dim Dt_ParcoMacchine As New DataTable
        XmlDatiParcoMacchine = objXML_Contab.XML_ParcoMacchine__DATI(LogErrori, XmlDoc, Dt_ParcoMacchine)

        XmlDettagli.AppendChild(XmlDatiParcoMacchine)


        Dt_ParcoMacchine = objXML_Contab.DtForXml_Genera_ParcoMacchine

        objXML_Contab.DtForXml_InserisciRiga_ParcoMacchine(Dt_ParcoMacchine,
                                                           OperazioneMacchina,
                                                           PIVA,
                                                           Sa_Cod,
                                                           Mac_Cod,
                                                         Class_Code,
                                                         Mac_Des,
                                                          Costo_Acquisto,
                                                         Targa,
                                                         Telaio,
                                                         Ditta_Cod,
                                                         Modello,
                                                         Potenza,
                                                         Ammortamento,
                                                         Ammortizzato,
                                                         Data_Immatricolazione,
                                                         Ultima_Manutenzione,
                                                         Data_Revisione,
                                                         Stato_Utilizzo,
                                                         Validita_Inizio,
                                                         Validita_Fine,
                                                         BaseCode,
                                                         TopCode,
                                                         Note,
                                                         Tipo,
                                                         N_Immatricolazione,
                                                         N_Immatricolazione_Rimorchio,
                                                         N_Autorizzazione_Trasporto,
                                                         Data_Rilascio_Autorizzazione,
                                                         Peso,
                                                         Mac_Cod_Origine,
                                                         Piva_SuperUser_Origine,
                                                         ChkDefault,
                                                         Portata_Max,
                                                         Cod_Contatto,
                                                         Alimentazione_Cod,
                                                         Potenza_Udm_Cod,
                                                         CUAA_Proprietario,
                                                         Denominazione_Proprietario,
                                                         Tipo_Targa_Cod,
                                                         Tipo_Trazione_Cod,
                                                         N_Omologazione,
                                                         Ditta_Cod_Motore,
                                                         Tipo_Motore,
                                                         Matricola_Motore,
                                                         Data_Reimmatricolazione,
                                                         Data_Carico,
                                                         Data_Scarico,
                                                         TitoloPossesso,
                                                         Flag_Attrezzatura_Macchina,
                                                         Taratura_Ugello)


        Dim XmlParcoMacchine As System.Xml.XmlElement
        XmlParcoMacchine = objXML_Contab.XML_ParcoMacchine_ParcoMacchine(LogErrori,
                                                                          XmlDoc,
                                                                          Dt_ParcoMacchine.Rows(0))

        XmlDatiParcoMacchine.AppendChild(XmlParcoMacchine)


        '-------------------------------------------
        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiAgenda)

        'Estraggo la stringa XML complessiva
        Dim StrXmlMacchina As String = XmlDoc.InnerXml


        '------------------------------------------------
        '----- Scrivo nel DB
        '------------------------------------------------

        Dim objAgenda_W As New AgronicaCoreContabBIZ.Agenda_W
        Dim OUTPUT_ID_Agenda As Integer
        objAgenda_W.Agenda_Scrivi(StrXmlMacchina,
                                  OUTPUT_ID_Agenda,
                                  0,
                                  Id_Servizio_GiasOnline,
                                  0, "",
                                  objparametriserver)

        Return Mac_Cod

    End Function


    Public Function Crea_Atomizzatore(ByVal pivaret As String,
                                      ByVal DataTaraturaAtomizzatore As Date,
                                      ByVal Atomizzatore_Desc As String,
                                      ByVal TaraturaUgello As Decimal,
                                      ByRef objparametriserver As AgronicaCoreParametri
                                      ) As Integer
        'ByRef DT As DataTable
        'ByVal TipoOperazioneDB As enum_TipoOperazioneDB

        ' dim Piva As String = C_Piva_Default
        Dim Mac_Cod As Integer = 0
        Dim Class_Code As String = "06.02.01"
        Dim Mac_Des As String = ""
        Dim Costo_Acquisto As Decimal = 0
        Dim Targa As String = ""
        Dim Telaio As String = ""
        Dim Ditta_Cod As Integer = 0
        Dim Modello As String = ""
        Dim Potenza As String = ""
        Dim Ammortamento As Decimal = 0
        Dim Ammortizzato As Decimal = 0
        Dim Data_Immatricolazione As Date = AGRODATAINIZIO
        Dim Ultima_Manutenzione As Date = AGRODATAINIZIO
        Dim Data_Revisione As Date = AGRODATAINIZIO
        Dim Stato_Utilizzo As String = ""
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim BaseCode As Integer = BaseCode
        Dim TopCode As Integer = TopCode
        Dim Note As String = ""
        Dim Tipo As Integer = 0
        Dim N_Immatricolazione As String = ""
        Dim N_Immatricolazione_Rimorchio As String = ""
        Dim N_Autorizzazione_Trasporto As String = ""
        Dim Data_Rilascio_Autorizzazione As Date = AGRODATAINIZIO
        Dim Peso As Decimal = 0
        Dim Mac_Cod_Origine As Integer = 0
        Dim Piva_SuperUser_Origine As String = ""
        Dim ChkDefault As Integer = 0
        Dim Portata_Max As Decimal = 0
        Dim Cod_Contatto As String = ""
        Dim Alimentazione_Cod As Integer = 0
        Dim Potenza_Udm_Cod As Integer = 0
        Dim CUAA_Proprietario As String = ""
        Dim Denominazione_Proprietario As String = ""
        Dim Tipo_Targa_Cod As Integer = 0
        Dim Tipo_Trazione_Cod As Integer = 0
        Dim N_Omologazione As String = ""
        Dim Ditta_Cod_Motore As Integer = 0
        Dim Tipo_Motore As String = ""
        Dim Matricola_Motore As String = ""
        Dim Data_Reimmatricolazione As Date = AGRODATAINIZIO
        Dim Data_Carico As Date = AGRODATAINIZIO
        Dim Data_Scarico As Date = AGRODATAINIZIO
        Dim TitoloPossesso As Integer = 0
        Dim Flag_Attrezzatura_Macchina As String = "M"
        Dim Taratura_Ugello As Decimal = 0

        Mac_Des = Atomizzatore_Desc
        Data_Revisione = DataTaraturaAtomizzatore
        Taratura_Ugello = TaraturaUgello

        Mac_Cod = Crea_Macchina(pivaret, Class_Code, Mac_Des, Costo_Acquisto, Targa, Telaio, Ditta_Cod, Modello, Potenza, Ammortamento, Ammortizzato, Data_Immatricolazione, Ultima_Manutenzione, Data_Revisione, Stato_Utilizzo, Validita_Inizio, Validita_Fine, BaseCode, TopCode, Note, Tipo, N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, Data_Rilascio_Autorizzazione, Peso, Mac_Cod_Origine, Piva_SuperUser_Origine, ChkDefault, Portata_Max, Cod_Contatto, Alimentazione_Cod, Potenza_Udm_Cod, CUAA_Proprietario, Denominazione_Proprietario, Tipo_Targa_Cod, Tipo_Trazione_Cod, N_Omologazione, Ditta_Cod_Motore, Tipo_Motore, Matricola_Motore, Data_Reimmatricolazione, Data_Carico, Data_Scarico, TitoloPossesso, Flag_Attrezzatura_Macchina, Taratura_Ugello, objparametriserver)

        Return Mac_Cod
    End Function





    Public Function Crea_Macchina_Semplificata(ByVal piva As String,
                                               ByVal sa_cod As Integer,
                                               ByVal Class_Code As String,
                                               ByVal Ditta_Cod As Integer,
                                               ByVal Modello As String,
                                               ByVal Mac_Des As String,
                                               ByRef objparametriserver As AgronicaCoreParametri
                                               ) As Integer
        'ByRef DT As DataTable
        'ByVal TipoOperazioneDB As enum_TipoOperazioneDB
        ' dim Piva As String = C_Piva_Default
        'Dim Sa_Cod As Integer = 0
        Dim Mac_Cod As Integer = 0
        '  Dim Class_Code As String = "06.02.01"
        'Dim Mac_Des As String = ""
        Dim Costo_Acquisto As Decimal = 0
        Dim Targa As String = ""
        Dim Telaio As String = ""
        ' Dim Ditta_Cod As Integer = 0
        ' Dim Modello As String = ""
        Dim Potenza As String = ""
        Dim Ammortamento As Decimal = 0
        Dim Ammortizzato As Decimal = 0
        Dim Data_Immatricolazione As Date = AGRODATAINIZIO
        Dim Ultima_Manutenzione As Date = AGRODATAINIZIO
        Dim Data_Revisione As Date = AGRODATAINIZIO
        Dim Stato_Utilizzo As String = ""
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim BaseCode As Integer = BaseCode
        Dim TopCode As Integer = TopCode
        Dim Note As String = ""
        Dim Tipo As Integer = 0
        Dim N_Immatricolazione As String = ""
        Dim N_Immatricolazione_Rimorchio As String = ""
        Dim N_Autorizzazione_Trasporto As String = ""
        Dim Data_Rilascio_Autorizzazione As Date = AGRODATAINIZIO
        Dim Peso As Decimal = 0
        Dim Mac_Cod_Origine As Integer = 0
        Dim Piva_SuperUser_Origine As String = ""
        Dim ChkDefault As Integer = 0
        Dim Portata_Max As Decimal = 0
        Dim Cod_Contatto As String = ""
        Dim Alimentazione_Cod As Integer = 0
        Dim Potenza_Udm_Cod As Integer = 0
        Dim CUAA_Proprietario As String = ""
        Dim Denominazione_Proprietario As String = ""
        Dim Tipo_Targa_Cod As Integer = 0
        Dim Tipo_Trazione_Cod As Integer = 0
        Dim N_Omologazione As String = ""
        Dim Ditta_Cod_Motore As Integer = 0
        Dim Tipo_Motore As String = ""
        Dim Matricola_Motore As String = ""
        Dim Data_Reimmatricolazione As Date = AGRODATAINIZIO
        Dim Data_Carico As Date = AGRODATAINIZIO
        Dim Data_Scarico As Date = AGRODATAINIZIO
        Dim TitoloPossesso As Integer = 0
        Dim Flag_Attrezzatura_Macchina As String = "M"
        Dim Taratura_Ugello As Decimal = 0

        Mac_Cod = Crea_Macchina(piva, Class_Code, Mac_Des, Costo_Acquisto, Targa, Telaio, Ditta_Cod, Modello, Potenza, Ammortamento, Ammortizzato, Data_Immatricolazione, Ultima_Manutenzione, Data_Revisione, Stato_Utilizzo, Validita_Inizio, Validita_Fine, BaseCode, TopCode, Note, Tipo, N_Immatricolazione, N_Immatricolazione_Rimorchio, N_Autorizzazione_Trasporto, Data_Rilascio_Autorizzazione, Peso, Mac_Cod_Origine, Piva_SuperUser_Origine, ChkDefault, Portata_Max, Cod_Contatto, Alimentazione_Cod, Potenza_Udm_Cod, CUAA_Proprietario, Denominazione_Proprietario, Tipo_Targa_Cod, Tipo_Trazione_Cod, N_Omologazione, Ditta_Cod_Motore, Tipo_Motore, Matricola_Motore, Data_Reimmatricolazione, Data_Carico, Data_Scarico, TitoloPossesso, Flag_Attrezzatura_Macchina, Taratura_Ugello, objparametriserver, sa_cod)

        Return Mac_Cod
    End Function







    '########################################################################################
    'default: piva = "" (si cercano tutte le imprese),
    'altrimenti si cerca per un'impresa in particolare
    Public Sub CreaCentro_xImpresaSenzaCentri(ByRef Log_Import As StringBuilder,
                                              ByRef Log_Errori As StringBuilder,
                                              ByRef Log_Riepilogo As StringBuilder,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                              ByVal Progressivo_Gias As Integer,
                                              ByVal Piva As String)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb.CreaCentro_xImpresaSenzaCentri()"
        Dim MessaggioErrore As String = ""

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim Dt_Imprese As DataTable
        Dim i As Integer
        Dim FileInElaborazione As String = ""

        Dim objCore_XML_Utility As New AgronicaCoreXML.XML_Utility
        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim objCore_Centro_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
        Dim objFabbrtipi As New AgronicaCoreAnagrafeDAL.Fabbricati_Tipi_R
        Dim objCore_Fabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        Dim Flag_Insert As Boolean
        Dim Basecode As Integer = 0
        Dim Topcode As Integer = 0

        'Dim Piva As String
        Dim Rag_Soc As String
        Dim ind_des As String
        Dim frz_des As String
        Dim cap As String
        Dim pro_cod_istat As String
        Dim com_cod_istat As String
        Dim telefono As String
        Dim email As String
        Dim fax As String
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim OUTPUT_Sa_Cod As Integer = 0

        Dim Progr_Fabbricato As Integer = 1
        Dim Descr_Fabbricato As String = "Magazzino 01"
        Dim Foglio As Integer = 0
        Dim Numero As Integer = 0
        Dim Tipo_Fabbricato_Cod As Integer = MAGAZZINO
        Dim OUTPUT_Fabbricato_Cod As Integer = 0

        Dim num_imp_senzacentri As Integer = 0
        Dim num_centricreati As Integer = 0
        Dim num_magazzinicreati As Integer = 0
        Dim num_centrinoncreati As Integer = 0

        Try

            Dt_Imprese = objImp.ImpreseSenzaCentriAziendali(Piva,
                                                            True,
                                                            "", "",
                                                            objParametri_Server)

            If Not IsNothing(Dt_Imprese) Then

                AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode,
                                                                                   Topcode,
                                                                                    Progressivo_Gias)


                num_imp_senzacentri = Dt_Imprese.Rows.Count

                For i = 0 To num_imp_senzacentri - 1
                    'For i = 0 To 0

                    Try

                        With Dt_Imprese.Rows(i)
                            Piva = .Item("Piva")
                            Rag_Soc = .Item("Rag_Soc")
                            ind_des = .Item("ind_des")
                            frz_des = .Item("frz_des")
                            cap = .Item("cap")
                            pro_cod_istat = .Item("pro_cod_istat")
                            com_cod_istat = .Item("com_cod_istat")
                            telefono = ""
                            email = ""
                            fax = ""
                            Validita_Inizio = .Item("Validita_Inizio")
                            Validita_Fine = .Item("Validita_Fine")
                        End With

                        Log_Riepilogo.Append(Rag_Soc & ": ")

                        Creazione_Automatica_Centro(Log_Import,
                                                     Log_Errori,
                                                     Log_Riepilogo,
                                                     FileInElaborazione,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        objCore_XML_Utility,
                                                        objCore_XML_Anagrafe,
                                                        objCore_Centro_W,
                                                        Flag_Insert,
                                                        Basecode,
                                                          Topcode,
                                                        Piva,
                                                        Rag_Soc,
                                                        "",
                                                        ind_des,
                                                        frz_des,
                                                        cap,
                                                        pro_cod_istat,
                                                        com_cod_istat,
                                                        telefono,
                                                        email,
                                                        fax,
                                                        OUTPUT_Sa_Cod,
                                                        Validita_Inizio,
                                                        Validita_Fine)

                        num_centricreati += 1

                        Creazione_Automatica_Fabbricato(Log_Import,
                                                       Log_Errori,
                                                       Log_Riepilogo,
                                                       FileInElaborazione,
                                                          objParametri_Server,
                                                          objCore_XML_Utility,
                                                          objFabbrtipi,
                                                          objCore_XML_Anagrafe,
                                                          objCore_Fabbricato_W,
                                                          Flag_Insert,
                                                          Basecode,
                                                        Topcode,
                                                          Progr_Fabbricato,
                                                        Descr_Fabbricato,
                                                        Piva,
                                                        Rag_Soc,
                                                        ind_des,
                                                        frz_des,
                                                        cap,
                                                        pro_cod_istat,
                                                        com_cod_istat,
                                                        Foglio,
                                                        Numero,
                                                        Tipo_Fabbricato_Cod,
                                                         OUTPUT_Sa_Cod,
                                                         OUTPUT_Fabbricato_Cod,
                                                        Validita_Inizio,
                                                        Validita_Fine)

                        num_magazzinicreati += 1

                        Log_Riepilogo.Append(vbCrLf)

                    Catch ex As Exception
                        Log_Riepilogo.Append(" ERRORE " & ex.Message & vbCrLf)
                        num_centrinoncreati += 1
                    End Try

                Next

            End If

            Log_Riepilogo.Append(vbCrLf & vbCrLf)
            Log_Riepilogo.Append("Num. imprese senza centri: " & CStr(num_imp_senzacentri) & vbCrLf)
            Log_Riepilogo.Append("Num. centri creati: " & CStr(num_centricreati) & vbCrLf)
            Log_Riepilogo.Append("Num. magazzini creati: " & CStr(num_magazzinicreati) & vbCrLf)
            Log_Riepilogo.Append("Num. centri non creati: " & CStr(num_centrinoncreati) & vbCrLf)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Log_Riepilogo.Append(" ERRORE " & MessaggioErrore & vbCrLf)
        End Try

    End Sub

    '########################################################################################
    Public Sub Creazione_Automatica_Impresa(ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal FileInElaborazione As String,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                               ByRef objCore_XML_Utility As AgronicaCoreXML.XML_Utility,
                               ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                               ByRef objCore_Impresa_W As AgronicaCoreAnagrafeBIZ.Impresa_W,
                               ByRef Flag_Insert As Boolean,
                               ByVal Basecode As Integer,
                               ByVal Topcode As Integer,
                               ByVal Piva_Padre As String,
                               ByVal Piva As String,
                               ByVal CUAA As String,
                               ByVal Codice_Fiscale As String,
                               ByVal Rag_Soc As String,
                               ByVal ind_des As String,
                               ByVal frz_des As String,
                               ByVal cap As String,
                               ByVal pro_cod_istat As String,
                               ByVal com_cod_istat As String)


        'Tipo Impresa:
        '1 = impresa
        '2 = cooperativa
        '3 = consorzio
        '4 = op

        Dim XmlDoc As New XmlDocument
        Dim XmlDatiImprese As System.Xml.XmlElement
        Dim Xml_Impresa As XmlElement
        Dim Dt_RisUm As DataTable
        Dim Dt_CodiciImpresa As DataTable
        Dim Log As String = ""

        Dim Sa_Cod_Contatto As Integer = PRIVATO
        Dim Tipo_Indirizzo As Integer = 1
        Dim TipoImpresaGerarchia As Integer = 1
        Dim Id_CF As Integer = PERSONA_GIURIDICA

        '  DATI IMPRESE   
        XmlDatiImprese = XmlDoc.CreateElement("DatiImprese")

        XmlDoc.AppendChild(XmlDatiImprese)

        '------------------------------------
        '-------------- IMPRESA -------------
        '------------------------------------

        Dt_RisUm = objCore_XML_Utility.CaricaGriglia_RisUm_for_XML()
        Dt_CodiciImpresa = objCore_XML_Utility.CaricaGriglia_CodiciImpresa_for_XML()

        objCore_XML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(Dt_RisUm,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        Piva_Padre,
                                                        Piva,
                                                        Sa_Cod_Contatto,
                                                        0,
                                                        COD_FORNITORE,
                                                        "",
                                                        "",
                                                         , , , , , , , , , ,
                                                         , )

        'CUAA
        objCore_XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_CodiciImpresa,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    "",
                                                                    enum_CodiciAnagrafe.CodiceCUAA,
                                                                    Codice_Fiscale,
                                                                    , )

        'TITOLO POSSESSO
        objCore_XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_CodiciImpresa,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    "",
                                                                    enum_CodiciAnagrafe.TitoloPossesso,
                                                                    TITOLOPOSSESSO_PROPRIETA,
                                                                    , )

        'le imprese contatto hanno come piva contatto la piva del superuser
        Xml_Impresa = objCore_XML_Anagrafe.XML_2_Impresa(Log,
                                                        XmlDoc,
                                                        Basecode,
                                                        Topcode,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        enum_TipoOperazioneDB.Scrittura,
                                                        Piva,
                                                        Rag_Soc,
                                                        Piva_Padre,
                                                        objParametri_Server.PivaSuperUser,
                                                        TipoImpresaGerarchia,
                                                        Tipo_Indirizzo,
                                                        pro_cod_istat,
                                                        com_cod_istat,
                                                        0,
                                                        ind_des,
                                                        frz_des,
                                                        cap,
                                                        "IT",
                                                        "",
                                                        Dt_CodiciImpresa,
                                                        Dt_RisUm,
                                                        Nothing,
                                                        , , , , ,
                                                        "",
                                                        Sa_Cod_Contatto,
                                                        Id_CF,
                                                        "",
                                                        Codice_Fiscale,
                                                        0,
                                                        ,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE)

        If Log <> "" Then
            Flag_Insert = False
            '  Log_Errori.Append(Log)
            Throw New Exception("Importazione impresa: errore durante la creazione dell'xml " & Log)
        Else
            XmlDatiImprese.AppendChild(Xml_Impresa)

            Dim StrXmlCreazione As String
            Dim OUTPUT_Piva As String = ""

            StrXmlCreazione = XmlDatiImprese.OuterXml

            Flag_Insert = objCore_Impresa_W.Impresa_Scrivi(CStr(StrXmlCreazione),
                                    OUTPUT_Piva,
                                   objParametri_Server,
                                   objParametri_Utenti)
        End If



    End Sub


    '########################################################################################
    Public Sub Creazione_Automatica_Centro(ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal FileInElaborazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   ByRef objCore_XML_Utility As AgronicaCoreXML.XML_Utility,
                                   ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                   ByRef objCore_Centro_W As AgronicaCoreAnagrafeBIZ.CentroAziendale_W,
                                   ByRef Flag_Insert As Boolean,
                                   ByVal Basecode As Integer,
                                     ByVal Topcode As Integer,
                                   ByVal Piva As String,
                                   ByVal Rag_Soc As String,
                                   ByVal Sa_Nome As String,
                                   ByVal ind_des As String,
                                   ByVal frz_des As String,
                                   ByVal cap As String,
                                   ByVal pro_cod_istat As String,
                                   ByVal com_cod_istat As String,
                                   ByVal telefono As String,
                                   ByVal email As String,
                                   ByVal fax As String,
                                   ByRef OUTPUT_Sa_Cod As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date)


        Dim XmlDoc As New XmlDocument
        Dim XmlDatiCentriAziendali As System.Xml.XmlElement
        Dim Xml_Centro As XmlElement
        Dim Dt_Rubrica As DataTable = Nothing
        Dim Dt_CodiciCentro As DataTable
        Dim Log As String = ""

        Dim Tipo_Indirizzo As Integer = 1
        'Dim Sa_Nome As String


        '  DATI CENTRI AZIENDALI   
        XmlDatiCentriAziendali = XmlDoc.CreateElement("DatiCentriAziendali")

        XmlDoc.AppendChild(XmlDatiCentriAziendali)

        Flag_Insert = False

        '------------------------------------
        '---------- CENTRO AZIENDALE --------
        '------------------------------------
        If Sa_Nome = "" Then
            Sa_Nome = "Centro " & Rag_Soc
        End If

        Dt_CodiciCentro = objCore_XML_Utility.CaricaGriglia_CodiciCentro_for_XML()

        If telefono <> "" Then

            Dt_Rubrica = objCore_XML_Utility.CaricaGriglia_Rubrica_for_XML()

            objCore_XML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    0,
                                                                    telefono,
                                                                    "Tel",
                                                                    , )

        End If

        If email <> "" Then

            Dt_Rubrica = objCore_XML_Utility.CaricaGriglia_Rubrica_for_XML()

            objCore_XML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    0,
                                                                    email,
                                                                    "email",
                                                                    , )


        End If

        If fax <> "" Then

            Dt_Rubrica = objCore_XML_Utility.CaricaGriglia_Rubrica_for_XML()

            objCore_XML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    0,
                                                                    fax,
                                                                    "Fax",
                                                                    , )


        End If


        'TITOLO POSSESSO
        objCore_XML_Utility.Inserisci_Riga_Dt_CodiciCentro_for_XML(Dt_CodiciCentro,
                                                                    enum_TipoOperazioneDB.Scrittura,
                                                                    "", 0,
                                                                    enum_CodiciAnagrafe.TitoloPossesso,
                                                                    TITOLOPOSSESSO_PROPRIETA,
                                                                    , )

        ''CODICE CLIENTE
        'objCore_XML_Utility.Inserisci_Riga_Dt_CodiciCentro_for_XML(Dt_CodiciCentro, _
        '                                                            enum_TipoOperazioneDB.Scrittura, _
        '                                                            "", 0, _
        '                                                            enum_CodiciAnagrafe.CodiceCliente_FRUTTAGEL, _
        '                                                            f_Codice_JDE, _
        '                                                            , )


        Xml_Centro = objCore_XML_Anagrafe.XML_2_CentroAziendale(Log,
                                                                XmlDoc,
                                                                Basecode,
                                                                Topcode,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                Piva,
                                                                0,
                                                                Sa_Nome,
                                                                Tipo_Indirizzo,
                                                                pro_cod_istat,
                                                                com_cod_istat,
                                                                0,
                                                                ind_des,
                                                                frz_des,
                                                                cap,
                                                                "IT",
                                                                "",
                                                                Dt_CodiciCentro,
                                                                Dt_Rubrica,
                                                                , , , , , , , , ,
                                                                TITOLOPOSSESSO_PROPRIETA,
                                                                enum_TipoCentro.Sede_Legale,
                                                                , , , , , , , ,
                                                                Validita_Inizio,
                                                                Validita_Fine)

        XmlDatiCentriAziendali.AppendChild(Xml_Centro)



        If Log <> "" Then
            Flag_Insert = False
            ' Log_Errori.Append(Log)
            Throw New Exception("Importazione Centro Aziendale: errore durante la creazione dell'xml " & Log)
        Else

            Dim StrXmlCreazione As String
            Dim OUTPUT_Piva As String = ""

            StrXmlCreazione = XmlDatiCentriAziendali.OuterXml

            'Inserisco il centro
            Flag_Insert = objCore_Centro_W.CentroAziendale_Scrivi(CStr(StrXmlCreazione),
                                                                 OUTPUT_Piva,
                                                                 OUTPUT_Sa_Cod,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)


            Log_Riepilogo.Append("Creato il centro aziendale: " & pro_cod_istat & " - " & com_cod_istat & vbCrLf)

        End If



    End Sub


    '########################################################################################
    Public Sub Creazione_Automatica_Fabbricato(ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal FileInElaborazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objCore_XML_Utility As AgronicaCoreXML.XML_Utility,
                                   ByRef objFabbrtipi As AgronicaCoreAnagrafeDAL.Fabbricati_Tipi_R,
                                   ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                   ByRef objCore_Fabbricato_W As AgronicaCoreAnagrafeBIZ.Fabbricato_W,
                                    ByRef Flag_Insert As Boolean,
                                   ByVal Basecode As Integer,
                                     ByVal Topcode As Integer,
                                   ByVal Progr_Fabbricato As Integer,
                                   ByVal Descr_Fabbricato As String,
                                   ByVal Piva As String,
                                   ByVal Rag_Soc As String,
                                   ByVal ind_des As String,
                                   ByVal frz_des As String,
                                   ByVal cap As String,
                                   ByVal pro_cod_istat As String,
                                   ByVal com_cod_istat As String,
                                   ByVal Foglio As Integer,
                                   ByVal Numero As Integer,
                                   ByVal Tipo_Fabbricato_Cod As Integer,
                                    ByVal OUTPUT_Sa_Cod As Integer,
                                    ByRef OUTPUT_Fabbricato_Cod As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date)


        Dim XmlDoc As New XmlDocument
        Dim XmlDatiFabbricati As System.Xml.XmlElement
        Dim Xml_Fabbricato As XmlElement
        Dim Log As String = ""

        Dim Tipo_Indirizzo As Integer = 1
        Dim Fabbricato_Des As String
        'Dim Flag_Insert As Boolean
        Dim Dt_CodiciFabbricato As DataTable = Nothing


        '------------------------------------
        '------------- FABBRICATO -----------
        '------------------------------------

        '  DATI FABBRICATI   
        XmlDatiFabbricati = XmlDoc.CreateElement("DatiFabbricati")

        XmlDoc.AppendChild(XmlDatiFabbricati)

        If FileInElaborazione <> "" Then
            'sto venendo da agribio, modifico la descrizione
            If Descr_Fabbricato = "" Then
                Select Case Tipo_Fabbricato_Cod
                    Case 20
                        Fabbricato_Des = "(NotificaBIO) Magazzino " & CStr(Progr_Fabbricato)
                    Case Else
                        Fabbricato_Des = "(NotificaBIO) Fabbricato " & CStr(Progr_Fabbricato)
                End Select
            Else
                Fabbricato_Des = "(NotificaBIO) " & Descr_Fabbricato
            End If
        Else
            'altre importazioni
            Fabbricato_Des = Descr_Fabbricato
        End If

        ''Fabbricato_Des = "Fabbricato " & Rag_Soc
        'Fabbricato_Des = objFabbrtipi.TipoFabbricatoDes_from_TipoFabbricatoCod(Tipo_Fabbricato_Cod, objParametri_Server)

        'Dt_CodiciFabbricato = objCore_XML_Utility.CaricaGriglia_CodiciFabbricato_for_XML()

        ''CODICE CLIENTE
        'objCore_XML_Utility.Inserisci_Riga_Dt_CodiciFabbricato_for_XML(Dt_CodiciFabbricato, _
        '                                                                enum_TipoOperazioneDB.Scrittura, _
        '                                                                Piva, _
        '                                                                OUTPUT_Sa_Cod, _
        '                                                                0, _
        '                                                                enum_CodiciAnagrafe.CodiceCliente_FRUTTAGEL, _
        '                                                                f_Codice_JDE, _
        '                                                                , )


        Xml_Fabbricato = objCore_XML_Anagrafe.XML_2_Fabbricato(Log,
                                                                XmlDoc,
                                                                Basecode,
                                                                Topcode,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                Piva,
                                                                OUTPUT_Sa_Cod,
                                                                0,
                                                                Fabbricato_Des,
                                                                 Tipo_Fabbricato_Cod,
                                                                Tipo_Indirizzo,
                                                                pro_cod_istat,
                                                                com_cod_istat,
                                                                0,
                                                                ind_des,
                                                                frz_des,
                                                                cap,
                                                                "IT",
                                                                "",
                                                                Dt_CodiciFabbricato,
                                                                Nothing,
                                                                Nothing,
                                                                Nothing,
                                                                Nothing,
                                                                pro_cod_istat, com_cod_istat, "", Foglio, Numero, "",
                                                                , , , ,
                                                                TITOLOPOSSESSO_PROPRIETA,
                                                                , , , , , , , , , , , , , ,
                                                                Validita_Inizio,
                                                                Validita_Fine)

        XmlDatiFabbricati.AppendChild(Xml_Fabbricato)


        If Log <> "" Then
            '  Log_Errori.Append(Log)
            Throw New Exception("Importazione Fabbricato: errore durante la creazione dell'xml " & Log)
        Else

            Dim StrXmlCreazione As String
            Dim OUTPUT_Piva As String = ""

            StrXmlCreazione = XmlDatiFabbricati.OuterXml

            'inserisco il fabbricato
            Flag_Insert = objCore_Fabbricato_W.Fabbricato_Scrivi(CStr(StrXmlCreazione),
                            OUTPUT_Piva,
                            OUTPUT_Sa_Cod,
                            OUTPUT_Fabbricato_Cod,
                            objParametri_Server)

            Log_Riepilogo.Append("Importato il Fabbricato in anagrafica. " & vbCrLf)

        End If



    End Sub



    '########################################################################################
    Public Sub Creazione_Automatica_Catasto(ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal FileInElaborazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objCore_XML_Utility As AgronicaCoreXML.XML_Utility,
                                   ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                                   ByRef objCore_Particella_W As AgronicaCoreAnagrafeBIZ.Particella_W,
                                   ByRef Flag_Insert As Boolean,
                                   ByVal Basecode As Integer,
                                     ByVal Topcode As Integer,
                                   ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal PROV As String,
                                   ByVal COM As String,
                                   ByVal Sezione As String,
                                   ByVal Foglio As Integer,
                                   ByVal Numero As Integer,
                                   ByVal Subalterno As String,
                                   ByVal TitoloPossesso As Integer,
                                   ByVal Sup_Catastale As Decimal)


        Dim XmlDoc As New XmlDocument
        Dim XmlDatiParticelle As System.Xml.XmlElement
        Dim Xml_Particella As XmlElement
        Dim Log As String = ""
        'Dim Flag_Insert As Boolean = False

        Dim DT_ParticelleZone As DataTable = Nothing
        Dim DT_ParticelleMacrousi As DataTable = Nothing
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE
        Dim Validita_Inizio_Centro As Date = AGRODATAINIZIO
        Dim Validita_Fine_Centro As Date = AGRODATAFINE

        Dim Sup_Condotta As Decimal = Sup_Catastale
        Dim Ettari As Decimal
        Dim Are As Integer
        Dim Centiare As Integer

        AgronicaCoreDataProvider.UtilityProvider.EttariAreCentiare_from_Ettari(Sup_Catastale, Ettari, Are, Centiare)


        'DATI PARTICELLE
        XmlDatiParticelle = XmlDoc.CreateElement("DatiParticelle")

        XmlDoc.AppendChild(XmlDatiParticelle)



        '------------------------------------
        '---------- PARTICELLE --------------
        '------------------------------------

        Xml_Particella = objCore_XML_Anagrafe.XML_Particella(Log,
                                             XmlDoc,
                                            Basecode,
                                             Topcode,
                                            enum_TipoOperazioneDB.Scrittura,
                                             Piva,
                                             Sa_Cod,
                                              PROV,
                                             COM,
                                             Sezione,
                                             Foglio,
                                             Numero,
                                             Subalterno,
                                             Ettari,
                                             Are,
                                             Centiare,
                                             TitoloPossesso,
                                             Sup_Condotta,
                                             Validita_Inizio,
                                             Validita_Fine,
                                             Validita_Inizio_Centro,
                                             Validita_Fine_Centro,
                                             DT_ParticelleZone,
                                             DT_ParticelleMacrousi,
                                            0,
                                           "",
                                             0,
                                            "",
                                             0,
                                             0)

        XmlDatiParticelle.AppendChild(Xml_Particella)

        If Log <> "" Then
            Flag_Insert = False
            '  Log_Errori.Append(Log)
            Throw New Exception("Importazione catasto: errore durante la creazione dell'xml " & Log)
        Else

            Dim StrXmlCreazione As String
            StrXmlCreazione = XmlDatiParticelle.OuterXml

            Flag_Insert = objCore_Particella_W.Particella_Scrivi(CStr(StrXmlCreazione),
                                                                 objParametri_Server)

        End If

    End Sub

    '########################################################################################
    Public Sub Creazione_Automatica_Legale(ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal FileInElaborazione As String,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objCore_XML_Utility As AgronicaCoreXML.XML_Utility,
                               ByRef objCore_XML_Anagrafe As AgronicaCoreXML.XML_Anagrafe,
                               ByRef objCore_Contatti_W As AgronicaCoreAnagrafeBIZ.Contatti_W,
                                ByRef Flag_Insert As Boolean,
                                ByVal Basecode As Integer,
                                ByVal Topcode As Integer,
                               ByVal Piva As String,
                               ByVal Legale_Codice_Fiscale As String,
                               ByVal Nome As String,
                               ByVal Cognome As String,
                               ByVal Sesso As String,
                               ByVal Data_Nascita As Date,
                               ByVal ind_des As String,
                               ByVal frz_des As String,
                               ByVal cap As String,
                               ByVal pro_cod_istat As String,
                               ByVal com_cod_istat As String,
                               ByVal pro_cod_istat_nascita As String,
                               ByVal com_cod_istat_nascita As String,
                               ByRef OUTPUT_Cod_Contatto As String)


        Dim XmlDoc As New XmlDocument
        Dim XmlDatiContatti As System.Xml.XmlElement
        Dim Xml_Contatto As XmlElement
        Dim Dt_RisUm As DataTable
        Dim Dt_Indirizzi As DataTable
        Dim Log As String = ""
        Dim j As Integer
        ' Dim Flag_Insert As Boolean

        Dim Sa_Cod_Contatto As Integer = PRIVATO
        Dim Id_CF As Integer = PERSONA_FISICA

        Dim Tipo_Indirizzo As Integer() = {INDIRIZZO_RESIDENZA, INDIRIZZO_DOMICILIO, INDIRIZZO_LUOGO_NASCITA, INDIRIZZO_RESIDENZA_ESTIVA}

        '  DATI CONTATTI   
        XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

        XmlDoc.AppendChild(XmlDatiContatti)


        'al cambio del contatto, azzero i datatable
        Dt_RisUm = Nothing
        Dt_Indirizzi = Nothing
        Dt_RisUm = objCore_XML_Utility.CaricaGriglia_RisUm_for_XML()
        Dt_Indirizzi = objCore_XML_Utility.CaricaGriglia_Indirizzi_for_XML()

        objCore_XML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(Dt_RisUm,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Piva,
                                        Legale_Codice_Fiscale,
                                        Sa_Cod_Contatto,
                                        0,
                                        COD_LEGALE,
                                        "",
                                        "",
                                        , , , , , , , , , , , )

        For j = 0 To Tipo_Indirizzo.Length - 1

            Select Case Tipo_Indirizzo(j)

                Case INDIRIZZO_RESIDENZA, INDIRIZZO_DOMICILIO

                    objCore_XML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi,
                                              enum_TipoOperazioneDB.Scrittura,
                                              Piva,
                                              Legale_Codice_Fiscale,
                                              Tipo_Indirizzo(j),
                                              pro_cod_istat,
                                              com_cod_istat,
                                              0,
                                              ind_des,
                                              frz_des,
                                              cap,
                                              "IT",
                                              "",
                                              , )

                Case INDIRIZZO_LUOGO_NASCITA

                    objCore_XML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi,
                                             enum_TipoOperazioneDB.Scrittura,
                                             Piva,
                                             Legale_Codice_Fiscale,
                                             Tipo_Indirizzo(j),
                                             pro_cod_istat_nascita,
                                             com_cod_istat_nascita,
                                             0,
                                             "",
                                             "",
                                             "",
                                             "IT",
                                             "",
                                             , )

                Case INDIRIZZO_RESIDENZA_ESTIVA

                    objCore_XML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi,
                                       enum_TipoOperazioneDB.Scrittura,
                                       Piva,
                                       Legale_Codice_Fiscale,
                                       Tipo_Indirizzo(j),
                                       "000",
                                       "000",
                                       0,
                                       "",
                                       "",
                                       "",
                                       "IT",
                                       "",
                                       , )

            End Select




        Next


        Xml_Contatto = objCore_XML_Anagrafe.XML_2_Contatto(Log,
                                                            XmlDoc,
                                                            Basecode,
                                                            Topcode,
                                                            False,
                                                            enum_TipoOperazioneDB.Scrittura,
                                                            Piva,
                                                            Legale_Codice_Fiscale,
                                                            Dt_Indirizzi,
                                                            Dt_RisUm,
                                                            Sa_Cod_Contatto,
                                                            Id_CF,
                                                            "",
                                                            "",
                                                            "",
                                                            0,
                                                            Nome,
                                                            Cognome,
                                                            Data_Nascita,
                                                            Sesso,
                                                            "",
                                                            , ,
                                                            Nothing,
                                                            Nothing)

        XmlDatiContatti.AppendChild(Xml_Contatto)


        If Log <> "" Then
            Flag_Insert = False
            ' Log_Errori.Append(Log)
            Throw New Exception("Importazione Legale Rappresentante: errore durante la creazione dell'xml " & Log)
        Else

            Dim StrXmlCreazione As String
            Dim OUTPUT_Piva As String = ""

            StrXmlCreazione = XmlDoc.OuterXml

            Flag_Insert = objCore_Contatti_W.Contatto_Scrivi(CStr(StrXmlCreazione),
                                                            OUTPUT_Piva,
                                                            OUTPUT_Cod_Contatto,
                                                             objParametri_Server)

            Log_Riepilogo.Append("Importato il Legale Rappresentante." & vbCrLf)


        End If



    End Sub



    '============================================================================
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DatiAnagrafe"></param>
    ''' <param name="OUTPUT_Piva"></param>
    ''' <param name="Flag_Mirror"></param>
    ''' <param name="Id_Servizio"></param>
    ''' <param name="Rimappa_Codici"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	10/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Scrivi_Anagrafe(ByVal DatiAnagrafe As String,
                                    ByRef OUTPUT_Piva As String,
                                    ByVal Flag_Mirror As Integer,
                                    ByVal Id_Servizio As Integer,
                                    ByVal Rimappa_Codici As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                    Optional ByVal NoteLog As String = "Scrivi_Anagrafe XML"
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeBIZ.Importa_GIAS.Scrivi_Anagrafe()"


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        '-------------------------------------------
        Dim XmlDoc As XmlDocument
        Dim xmlDocClone As XmlDocument
        Dim xmlDocClone2 As XmlDocument

        Dim xDatiImprese As XmlNodeList
        Dim xDatiImpresa As XmlElement
        Dim xImprese As XmlNodeList
        Dim xImpresaClone As XmlElement
        Dim xImpresa As XmlElement

        Dim xFascicoli As XmlNodeList
        Dim xFascicolo As XmlElement = Nothing

        'Dim xDatiImprese_Progetti As XmlNodeList
        'Dim xImprese_Progetti As XmlElement

        Dim xNodoDummy As XmlElement
        Dim xDatiCentri As XmlNodeList
        Dim xDatiContatti As XmlNodeList
        Dim xDatiContatto As XmlElement
        Dim xDatiMacchine As XmlNodeList
        Dim xDatiMacchina As XmlElement
        Dim xDatiCentriClone As XmlNodeList
        Dim xDatiCentro As XmlElement
        Dim xCentri As XmlNodeList
        Dim xCentro As XmlElement
        'Dim xCentroClone As XmlElement
        Dim xDatiCampi As XmlNodeList
        Dim xDatiCampo As XmlElement
        Dim xParticelle As XmlNodeList
        Dim xFabbricati As XmlNodeList


        Dim xDatiAppezzamenti As XmlNodeList
        Dim xDatiAppezzamento As XmlElement
        Dim xCampi As XmlNodeList
        Dim xCampo As XmlElement
        Dim xAppezzamenti As XmlNodeList
        Dim xAppezzamentoClone As XmlElement
        Dim xAppezzamento As XmlElement

        Dim xDatiImpianti As XmlNodeList
        Dim xDatiImpiantiClone As XmlNodeList
        Dim xDatiImpianto As XmlElement
        Dim xImpianti As XmlNodeList
        Dim xImpianto As XmlElement
        Dim xContatti As XmlNodeList
        Dim xContatto As XmlElement
        Dim xMacchine As XmlNodeList
        Dim xMacchina As XmlElement

        Dim xDatiAgende As XmlNodeList
        Dim xDatiAgenda As XmlElement
        Dim xAgende As XmlNodeList
        Dim xAgenda As XmlElement

        Dim xMovimenti_Destinazione As XmlNodeList
        Dim xMovimento_Destinazione As XmlElement

        Dim xDatiParticella As XmlElement
        Dim xDatiFabbricato As XmlElement

        Dim xDatiParticelle As XmlNodeList
        Dim xParticella As XmlElement

        'Dim xZonexParticella As XmlNodeList
        'Dim xZona As XmlElement

        'Dim xParticellaxMacrousi As XmlNodeList
        'Dim xMacrouso As XmlElement

        'Dim xParticellaxMacrousixUtilizzi As XmlNodeList
        'Dim xUtilizzo As XmlElement

        Dim xDatiFabbricati As XmlNodeList
        Dim xFabbricato As XmlElement

        '------------------------------------

        Dim objImprese_W As New AgronicaCoreAnagrafeBIZ.Impresa_W
        Dim objStrutture_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
        Dim objCampi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
        Dim objAppezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim ObjImpianti_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
        Dim ObjAgenda_W As New AgronicaCoreContabBIZ.Agenda_W
        'Dim objProgetto_W As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim ObjParticelle_W As New AgronicaCoreAnagrafeBIZ.Particella_W
        Dim objContatto_W As New AgronicaCoreAnagrafeBIZ.Contatti_W
        Dim objMacchine_W As New AgronicaCoreContabBIZ.Parco_Macchine_W
        Dim ObjFabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        Dim objFabbricatixCodici As New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R
        'Dim objZonexParticelle_W As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
        'Dim objParticellexMacrousi_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W
        'Dim objParticellexMacrousixUtilizzo_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W
        Dim objFascicolo As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
        Dim objFascicoloEntita As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W

        '----------------------------------------------------------------

        Dim Dummy As Boolean
        Dim Cod_Struttura As Integer
        Dim Cod_Campo As Integer
        Dim Cod_Appezzamento As Integer
        Dim Cod_Impianto As Integer
        Dim Cod_Agenda As Integer
        'Dim Cod_Progetto As Integer

        '----------------------------------------------------------------

        Dim Lav_Cod As Integer
        Dim Fabbricato_Cod As Integer
        Dim Fabbricato_Cod_Cliente As Integer

        Dim i_DatiImpresa As Integer
        Dim i_Impresa As Integer

        Dim i_Fascicolo As Integer

        Dim i_DatiCentro As Integer
        Dim i_Centro As Integer

        Dim i_DatiContatto As Integer
        Dim i_Contatto As Integer

        Dim i_DatiMacchina As Integer
        Dim i_Macchina As Integer

        Dim i_DatiAppezzamento As Integer
        Dim i_Appezzamento As Integer

        Dim i_DatiCampo As Integer
        Dim i_Campo As Integer

        Dim i_DatiImpianto As Integer
        Dim i_Impianto As Integer

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer

        'Dim i_DatiImprese_Progetti As Integer

        Dim i_DatiParticelle As Integer
        Dim i_Particella As Integer

        'Dim i_Zona As Integer

        'Dim i_Macrouso As Integer
        'Dim i_Utilizzo As Integer

        Dim i_DatiFabbricati As Integer
        Dim i_Fabbricato As Integer

        '   Dim DatiEntita                  As String
        Dim DatiCampo As String
        Dim DatiAppezzamento As String
        Dim DatiImpianto As String
        Dim DatiAgenda As String
        Dim DatiParticella As String
        Dim DatiFabbricato As String

        'Dim i As Integer

        'Dim sImprese_Progetti As String

        Dim DtFabbricatixCodici As DataTable

        Dim Allegati_Documenti_Cod As Integer = 0
        Dim FascicoloPresente As Boolean = False

        '------------------------------------

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True

        '------------------------------

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri)
            ''Se la connessione è chiusa la apro
            'If objParametri.objConnessione Is Nothing Then
            '    objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
            '    objParametri.objConnessione.Open()
            '    FlagConnessioneLocale = True
            'End If
            'If objParametri.objTransazione Is Nothing Then
            '    'Inizializzo la transazione
            '    objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
            '    FlagTransazioneLocale = True
            'End If
            '------------------------------


            XmlDoc = New XmlDocument
            XmlDoc.LoadXml(DatiAnagrafe)

            xmlDocClone = New XmlDocument

            xmlDocClone2 = New XmlDocument

            '------------------------------

            i_DatiImpresa = 0

            xDatiImprese = XmlDoc.GetElementsByTagName("DatiImprese")

            Do While i_DatiImpresa < xDatiImprese.Count

                'Prelevo l'i-esimo blocco di DatiImprese (in realta' ne esiste uno solo)
                xDatiImpresa = xDatiImprese.Item(i_DatiImpresa)


                '------------------------------

                xImprese = xDatiImpresa.GetElementsByTagName("Impresa")

                i_Impresa = 0

                Do While i_Impresa < xImprese.Count

#Region "IMPRESA"
                    '----------------------------------------------------
                    '############################################################
                    '#####################     IMPRESA       ####################
                    '############################################################

                    'Prelevo l' i-esima Impresa
                    xImpresa = xImprese.Item(i_Impresa)

                    'Faccio una copia del nodo precedente
                    xmlDocClone.LoadXml("<DatiImprese>" & xImpresa.OuterXml & "</DatiImprese>")
                    xImpresaClone = xmlDocClone.GetElementsByTagName("Impresa")(0)

                    'Estraggo il nodo figlio "DatiCentriAziendali"
                    xDatiCentriClone = xImpresaClone.GetElementsByTagName("DatiCentriAziendali")

                    'Elimino i dati relativi ai Centri_Aziendali dal nodo clonato
                    If xDatiCentriClone.Count > 0 Then
                        xNodoDummy = xImpresaClone.RemoveChild(xDatiCentriClone.Item(0))
                    End If

                    'Estraggo il nodo figlio "DatiContatti"
                    xDatiCentriClone = xImpresaClone.GetElementsByTagName("DatiContatti")

                    'Elimino i dati relativi ai Contatti dal nodo clonato
                    If xDatiCentriClone.Count > 0 Then
                        xNodoDummy = xImpresaClone.RemoveChild(xDatiCentriClone.Item(0))
                    End If

                    'Set objImprese_W = CreateObject("Agro_Anagrafe.Impresa_W")

                    ' Dummy = objImprese_W.Impresa_Scrivi(xImpresaClone.OuterXml, OUTPUT_Piva, objParametri)
                    Dummy = objImprese_W.Impresa_Scrivi("<DatiImprese>" & xImpresaClone.OuterXml & "</DatiImprese>",
                                        OUTPUT_Piva,
                                        objParametri,
                                        objParametri_Utenti)
#End Region

#Region "CONTATTI"
                    '----------------------------------------------------
                    '############################################################
                    '#####################  CONTATTI ############################
                    '############################################################


                    xDatiContatti = xImpresa.GetElementsByTagName("DatiContatti")

                    i_DatiContatto = 0

                    Do While i_DatiContatto < xDatiContatti.Count


                        'Prelevo l'i-esimo blocco di DatiCentri (in realta' ne esiste uno solo)
                        xDatiContatto = xDatiContatti.Item(i_DatiContatto)

                        '------------------------------

                        xContatti = xDatiContatto.GetElementsByTagName("Contatto")

                        i_Contatto = 0

                        Do While i_Contatto < xContatti.Count


                            'Prelevo l' i-esimo Centro
                            xContatto = xContatti.Item(i_Contatto)

                            'Set objContatto_W = CreateObject("Agro_Contab.Contatti_W")

                            Dummy = objContatto_W.Contatto_Scrivi("<DatiContatti>" & xContatto.OuterXml & "</DatiContatti>",
                                                                    "", "", objParametri)

                            'Set objContatto_W = Nothing

                            i_Contatto = i_Contatto + 1

                        Loop

                        i_DatiContatto = i_DatiContatto + 1

                    Loop


#End Region

#Region "MACCHINE"
                    '----------------------------------------------------
                    '############################################################
                    '#####################  MACCHINE ############################
                    '############################################################

                    xDatiMacchine = xImpresa.GetElementsByTagName("DatiMacchine")

                    i_DatiMacchina = 0

                    Do While i_DatiMacchina < xDatiMacchine.Count


                        'Prelevo l'i-esimo blocco di DatiCentri (in realta' ne esiste uno solo)
                        xDatiMacchina = xDatiMacchine.Item(i_DatiMacchina)

                        '------------------------------

                        xMacchine = xDatiMacchina.GetElementsByTagName("Macchina")

                        i_Macchina = 0

                        Do While i_Macchina < xMacchine.Count


                            'Prelevo l' i-esimo Centro
                            xMacchina = xMacchine.Item(i_Macchina)

                            'Set objContatto_W = CreateObject("Agro_Contab.Contatti_W")

                            'Dummy = objContatto_W.Contatto_Scrivi("<DatiMacchine>" & xMacchina.OuterXml & "</DatiMacchine>",
                            '                                        "", "", objParametri)

                            Dummy = objMacchine_W.Macchina_Scrivi("<DatiMacchine>" & xMacchina.OuterXml & "</DatiMacchine>",
                                                                    "", "", objParametri)

                            'Set objContatto_W = Nothing

                            i_Macchina = i_Macchina + 1

                        Loop

                        i_DatiMacchina = i_DatiMacchina + 1

                    Loop

#End Region

#Region "FASCICOLO"
                    '----------------------------------------------------
                    '############################################################
                    '#####################  FASCICOLO  ##########################
                    '############################################################

                    Dim SchedaNumero As String = ""
                    Dim Validazione_Data_Fascicolo As Date = AGRODATAINIZIO

                    xFascicoli = xImpresa.GetElementsByTagName("Fascicolo")

                    i_Fascicolo = 0

                    If xFascicoli IsNot Nothing AndAlso xFascicoli.Count > 0 Then

                        FascicoloPresente = True

                        Dim objFascicoloxEntita_R As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                        Dim DtFascicoloxEntita As DataTable
                        Dim DrScheda As DataRow()
                        'leggo i fascicoli dell'azienda
                        DtFascicoloxEntita = objFascicoloxEntita_R.Leggi_con_Ente(0, OUTPUT_Piva, 0, 0, 0, 0, 0,
                                                    "", "", "", 0, 0, "",
                                                    "", "",
                                                    0, 0, 0,
                                                    " Allegati_Documenti_CatCod= " & enum_CategorieDocumenti.DomandaFascicolo,
                                                    "",
                                                    objParametri)

                        Dim Detentore As String
                        Dim EnteCod As Integer = 0
                        Dim SchedaEsistente As Boolean = False


                        For i_Fascicolo = 0 To xFascicoli.Count - 1

                            xFascicolo = xFascicoli.Item(i_Fascicolo)

                            Detentore = xFascicolo.GetAttribute("codice_detentore")

                            Dim objEnte As New AgronicaCoreMetaSchemaDAL.EnteTecnico_R
                            EnteCod = objEnte.EnteCod_from_ENTE_CODIFICA(Detentore, "", objParametri)

                            SchedaNumero = xFascicolo.GetAttribute("numero")

                            If IsDate(xFascicolo.GetAttribute("data_validazione")) Then
                                Validazione_Data_Fascicolo = CDate(xFascicolo.GetAttribute("data_validazione"))
                            End If

                            If Validazione_Data_Fascicolo <> AGRODATAINIZIO Then
                                DrScheda = DtFascicoloxEntita.Select("Allegati_Documenti_Numero='" & SchedaNumero.ToString & "' " &
                                                                 " AND Validazione_Data=#" & Validazione_Data_Fascicolo.ToString("MM-dd-yyyy") & "# ")
                            Else
                                DrScheda = DtFascicoloxEntita.Select("Allegati_Documenti_Numero='" & SchedaNumero.ToString & "' ")
                            End If

                            If DrScheda IsNot Nothing AndAlso DrScheda.Length > 0 Then
                                SchedaEsistente = True
                            End If

                            If SchedaEsistente Then

                            Else
                                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                                Dim dtCatDoc As DataTable = objCatDoc.Leggi(enum_CategorieDocumenti.DomandaFascicolo, "", "", objParametri)

                                Dim SottoCartella As String = String.Empty
                                If Not IsDBNull(dtCatDoc.Rows(0).Item("Sottocartella")) Then
                                    SottoCartella = dtCatDoc.Rows(0).Item("Sottocartella")
                                End If

                                Try

                                    objFascicolo.Scrivi(OUTPUT_Piva,
                                                    "Domanda costituzione/modifica Fascicolo",
                                                    enum_CategorieDocumenti.DomandaFascicolo,
                                                    xFascicolo.GetAttribute("nome_file"),
                                                    xFascicolo.GetAttribute("numero"),
                                                    EnteCod,
                                                    SottoCartella,
                                                    xFascicolo.GetAttribute("validita_inizio_mandato"),
                                                    xFascicolo.GetAttribute("validita_fine_mandato"),
                                                    Allegati_Documenti_Cod,
                                                    objParametri,
                                                    Validazione_Data:=Validazione_Data_Fascicolo,
                                                    strXml:=xFascicolo.GetAttribute("allegatiDocumentiXML"))


                                Catch ex As Exception

                                    objFascicolo.Scrivi(OUTPUT_Piva,
                                                    "Domanda costituzione/modifica Fascicolo",
                                                    enum_CategorieDocumenti.DomandaFascicolo,
                                                    xFascicolo.GetAttribute("nome_file"),
                                                    xFascicolo.GetAttribute("numero"),
                                                    EnteCod,
                                                    SottoCartella,
                                                    xFascicolo.GetAttribute("validita_inizio_mandato"),
                                                    xFascicolo.GetAttribute("validita_fine_mandato"),
                                                    Allegati_Documenti_Cod,
                                                    objParametri,
                                                    Validazione_Data:=Validazione_Data_Fascicolo)


                                End Try

                                objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                          0, OUTPUT_Piva,
                                                          0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "",
                                                          0, 0, 0,
                                                          xFascicolo.GetAttribute("validita_inizio"),
                                                          xFascicolo.GetAttribute("validita_fine"),
                                                          objParametri)

                            End If
                        Next
                    End If

#End Region

#Region "CENTRI AZIENDALI"
                    '----------------------------------------------------
                    '############################################################
                    '#####################  CENTRI_AZIENDALI ####################
                    '############################################################


                    xDatiCentri = xImpresa.GetElementsByTagName("DatiCentriAziendali")

                    i_DatiCentro = 0

                    Do While i_DatiCentro < xDatiCentri.Count


                        'Prelevo l'i-esimo blocco di DatiCentri (in realta' ne esiste uno solo)
                        xDatiCentro = xDatiCentri.Item(i_DatiCentro)

                        '------------------------------

                        xCentri = xDatiCentro.GetElementsByTagName("CentroAziendale")

                        i_Centro = 0

                        Do While i_Centro < xCentri.Count

                            'Prelevo l' i-esimo Centro
                            xCentro = xCentri.Item(i_Centro)

                            '-------------------------
                            '-------------------------
                            'commento del 04/05/2011:
                            'questo pezzo era nel codice vb6 dei COM+
                            'ma è stato commentato nell'archiviazione del 24/02/2011
                            'per un qualche motivo
                            'lasciare decommentato!

                            'xCentroClone = xCentro

                            'xDatiFabbricati = xCentroClone.GetElementsByTagName("DatiFabbricati")

                            ''Elimino i dati relativi ai Fabbricati dal nodo clonato
                            'If xDatiFabbricati.Count > 0 Then
                            '    xNodoDummy = xCentroClone.RemoveChild(xDatiFabbricati.Item(0))
                            'End If
                            '----------------------
                            '----------------------

                            'Set objStrutture_W = CreateObject("Agro_Anagrafe.CentroAziendale_W")

                            'Dummy = objStrutture_W.CentroAziendale_Scrivi( _
                            '                    "<DatiCentriAziendali>" & xCentroClone.OuterXml & "</DatiCentriAziendali>", _
                            '                    OUTPUT_Piva, Cod_Struttura, objParametri)

                            Dummy = objStrutture_W.CentroAziendale_Scrivi(
                                      "<DatiCentriAziendali>" & xCentro.OuterXml & "</DatiCentriAziendali>",
                                      OUTPUT_Piva,
                                      Cod_Struttura,
                                      objParametri,
                                        objParametri_Utenti)

                            If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                     0,
                                                     OUTPUT_Piva,
                                                     Cod_Struttura,
                                                     0, 0, 0, 0, "", "", "", 0, 0, "", "", "",
                                                     0, 0, 0,
                                                     xFascicolo.GetAttribute("validita_inizio"),
                                                     xFascicolo.GetAttribute("validita_fine"),
                                                     objParametri)
                            End If

#End Region

#Region "FABBRICATI"
                            '----------------------------------------------------
                            '###############################################################
                            '#####################     FABBRICATI       ####################
                            '###############################################################

                            'commento del 04/05/2011:
                            'questa riga era nel codice vb6 dei COM+
                            'ma è stata commentata nell'archiviazione del 24/02/2011
                            'per un qualche motivo
                            'lasciare decommentato!

                            xDatiFabbricati = xCentro.GetElementsByTagName("DatiFabbricati")

                            i_DatiFabbricati = 0

                            Do While i_DatiFabbricati < xDatiFabbricati.Count

                                i_Fabbricato = 0

                                xDatiFabbricato = xDatiFabbricati.Item(i_DatiFabbricati)

                                xFabbricati = xDatiFabbricato.GetElementsByTagName("Fabbricato")

                                Do While i_Fabbricato < xFabbricati.Count

                                    xFabbricato = xFabbricati.Item(i_Fabbricato)

                                    If Cod_Struttura <> 0 Then

                                        DatiFabbricato = Replace(xFabbricato.OuterXml,
                                                            "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                            "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                    Else

                                        DatiFabbricato = xFabbricato.OuterXml

                                    End If


                                    'Scrittura dei Fabbricati
                                    Dummy = ObjFabbricato_W.Fabbricato_Scrivi("<DatiFabbricati>" & DatiFabbricato & "</DatiFabbricati>",
                                                                    OUTPUT_Piva, Cod_Struttura, Fabbricato_Cod, objParametri)


                                    If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                        objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                             0,
                                                             OUTPUT_Piva,
                                                             Cod_Struttura,
                                                             0, 0, 0,
                                                             Fabbricato_Cod,
                                                             "", "", "", 0, 0, "", "", "",
                                                             0, 0, 0,
                                                             xFascicolo.GetAttribute("validita_inizio"),
                                                             xFascicolo.GetAttribute("validita_fine"),
                                                             objParametri)
                                    End If

                                    i_Fabbricato = i_Fabbricato + 1

                                Loop

                                i_DatiFabbricati = i_DatiFabbricati + 1

                            Loop

#End Region

#Region "PARTICELLA"
                            '----------------------------------------------------
                            '###############################################################
                            '#####################     PARTICELLA       ####################
                            '###############################################################

                            xDatiParticelle = xCentro.GetElementsByTagName("DatiParticelle")

                            i_DatiParticelle = 0

                            Do While i_DatiParticelle < xDatiParticelle.Count

                                i_Particella = 0

                                xDatiParticella = xDatiParticelle.Item(i_DatiParticelle)

                                xParticelle = xDatiParticella.GetElementsByTagName("Particella")

                                Do While i_Particella < xParticelle.Count

                                    xParticella = xParticelle.Item(i_Particella)

                                    If Cod_Struttura <> 0 Then

                                        DatiParticella = Replace(xParticella.OuterXml,
                                                            "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                            "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                    Else

                                        DatiParticella = xParticella.OuterXml

                                    End If


                                    '                           Set ObjParticelle_W = CreateObject("Agro_Anagrafe.Particella_W")

                                    'Scrittura della Particella Catastale in Locale
                                    Dummy = ObjParticelle_W.Particella_Scrivi(
                                                                        "<DatiParticelle>" & DatiParticella & "</DatiParticelle>",
                                                                        objParametri,
                                                                        SchedaNumero, Validazione_Data_Fascicolo, NoteLog:=NoteLog)

                                    If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                        objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                             0,
                                                             OUTPUT_Piva,
                                                             Cod_Struttura,
                                                             0, 0, 0,
                                                             0,
                                                             CStr(xParticella.GetAttribute("prov")),
                                                             CStr(xParticella.GetAttribute("com")),
                                                             CStr(xParticella.GetAttribute("sezione")),
                                                             CLng(xParticella.GetAttribute("foglio")),
                                                             CLng(xParticella.GetAttribute("numero")),
                                                             CStr(xParticella.GetAttribute("subalterno")),
                                                             "", "",
                                                             0, 0, 0,
                                                             xFascicolo.GetAttribute("validita_inizio"),
                                                             xFascicolo.GetAttribute("validita_fine"),
                                                             objParametri)
                                    End If

                                    'Set ObjParticelle_W = Nothing

                                    '---------------------------------------------------------
                                    '---------------------------------------------------------
                                    'ELIMINATO inserimento di zione,macrousi e utilizzi
                                    'Già PRESENTE nel BIZ Particella_Scrivi!!!!!!!!!!!!!!!!
                                    '---------------------------------------------------------
                                    '---------------------------------------------------------
#End Region

#Region "ZONE"
                                    '----------------------------------------------------
                                    ''###############################################################
                                    ''#####################     ZONE       ##########################
                                    ''###############################################################

                                    'xZonexParticella = xParticella.GetElementsByTagName("Zona")

                                    'i_Zona = 0

                                    'If Not xZonexParticella Is Nothing Then

                                    '    Do While i_Zona < xZonexParticella.Count

                                    '        xZona = xZonexParticella.Item(i_Zona)

                                    '        Select Case CInt(xZona.GetAttribute("TipoOperazioneDB"))

                                    '            Case enum_TipoOperazioneDB.Scrittura

                                    '                Try
                                    '                    Dummy = objZonexParticelle_W.Scrivi(CInt(xZona.GetAttribute("zona_cod")), _
                                    '                                        CStr(xZona.GetAttribute("prov")), _
                                    '                                        CStr(xZona.GetAttribute("com")), _
                                    '                                        CStr(xZona.GetAttribute("sezione")), _
                                    '                                        CInt(xZona.GetAttribute("foglio")), _
                                    '                                        CInt(xZona.GetAttribute("numero")), _
                                    '                                        CStr(xZona.GetAttribute("subalterno")), _
                                    '                                        CDbl(xZona.GetAttribute("area")), _
                                    '                                        AGRODATAINIZIO, AGRODATAFINE, objParametri)


                                    '                Catch ex As Exception

                                    '                End Try

                                    '            Case enum_TipoOperazioneDB.Modifica


                                    '                Try


                                    '                    Dummy = objZonexParticelle_W.Modifica(CInt(xZona.GetAttribute("zona_cod")), _
                                    '                                        CStr(xZona.GetAttribute("prov")), _
                                    '                                        CStr(xZona.GetAttribute("com")), _
                                    '                                        CStr(xZona.GetAttribute("sezione")), _
                                    '                                        CInt(xZona.GetAttribute("foglio")), _
                                    '                                        CInt(xZona.GetAttribute("numero")), _
                                    '                                        CStr(xZona.GetAttribute("subalterno")), _
                                    '                                        CDbl(xZona.GetAttribute("area")), _
                                    '                                        AGRODATAINIZIO, AGRODATAFINE, _
                                    '                                        objParametri)
                                    '                Catch ex As Exception

                                    '                End Try

                                    '        End Select

                                    '        i_Zona = i_Zona + 1

                                    '    Loop

                                    'End If

#End Region

#Region "MACROUSI"
                                    '----------------------------------------------------
                                    ''###############################################################
                                    ''#####################     MACROUSI    #########################
                                    ''###############################################################

                                    'xParticellaxMacrousi = xParticella.GetElementsByTagName("Macrouso")

                                    'i_Macrouso = 0

                                    'Dim Validita_Inizio_Macrouso As Date = AGRODATAINIZIO
                                    'Dim Validita_Fine_Macrouso As Date = AGRODATAFINE

                                    'If Not xParticellaxMacrousi Is Nothing Then

                                    '    Do While i_Macrouso < xParticellaxMacrousi.Count

                                    '        xMacrouso = xParticellaxMacrousi.Item(i_Macrouso)

                                    '        Validita_Inizio_Macrouso = AGRODATAINIZIO
                                    '        Validita_Fine_Macrouso = AGRODATAFINE

                                    '        If xMacrouso.HasAttribute("validita_inizio") = True Then
                                    '            Validita_Inizio_Macrouso = CDate(xMacrouso.GetAttribute("validita_inizio"))
                                    '        End If
                                    '        If xMacrouso.HasAttribute("validita_fine") = True Then
                                    '            Validita_Fine_Macrouso = CDate(xMacrouso.GetAttribute("validita_fine"))
                                    '        End If

                                    '        Select Case CInt(xMacrouso.GetAttribute("TipoOperazioneDB"))

                                    '            Case enum_TipoOperazioneDB.Scrittura

                                    '                Try

                                    '                    Dummy = objParticellexMacrousi_W.Scrivi(CStr(xMacrouso.GetAttribute("prov")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("com")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("sezione")), _
                                    '                                                            CLng(xMacrouso.GetAttribute("foglio")), _
                                    '                                                            CLng(xMacrouso.GetAttribute("numero")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("subalterno")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("macrouso_cod")), _
                                    '                                                            CDbl(xMacrouso.GetAttribute("superficie")), _
                                    '                                                            Validita_Inizio_Macrouso, Validita_Fine_Macrouso, objParametri)
                                    '                Catch ex As Exception

                                    '                End Try

                                    '            Case enum_TipoOperazioneDB.Modifica

                                    '                Try
                                    '                    Dummy = objParticellexMacrousi_W.Modifica(CStr(xMacrouso.GetAttribute("prov")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("com")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("sezione")), _
                                    '                                                            CLng(xMacrouso.GetAttribute("foglio")), _
                                    '                                                            CLng(xMacrouso.GetAttribute("numero")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("subalterno")), _
                                    '                                                            CStr(xMacrouso.GetAttribute("macrouso_cod")), _
                                    '                                                            CDbl(xMacrouso.GetAttribute("superficie")), _
                                    '                                                            Validita_Inizio_Macrouso, Validita_Fine_Macrouso, _
                                    '                                                            "", _
                                    '                                                            objParametri)
                                    '                Catch ex As Exception

                                    '                End Try

                                    '        End Select

#End Region

#Region "UTILIZZI"
                                    '----------------------------------------------------
                                    '        '###############################################################
                                    '        '#####################     UTILIZZI    #########################
                                    '        '###############################################################

                                    '        xParticellaxMacrousixUtilizzi = xMacrouso.GetElementsByTagName("Utilizzo")

                                    '        i_Utilizzo = 0

                                    '        Dim Validita_Inizio_Utilizzo As Date = AGRODATAINIZIO
                                    '        Dim Validita_Fine_Utilizzo As Date = AGRODATAFINE

                                    '        If Not xParticellaxMacrousixUtilizzi Is Nothing Then

                                    '            Do While i_Utilizzo < xParticellaxMacrousixUtilizzi.Count

                                    '                xUtilizzo = xParticellaxMacrousixUtilizzi.Item(i_Utilizzo)

                                    '                Validita_Inizio_Utilizzo = AGRODATAINIZIO
                                    '                Validita_Fine_Utilizzo = AGRODATAFINE

                                    '                If xUtilizzo.HasAttribute("validita_inizio") = True Then
                                    '                    Validita_Inizio_Utilizzo = CDate(xUtilizzo.GetAttribute("validita_inizio"))
                                    '                End If
                                    '                If xUtilizzo.HasAttribute("validita_fine") = True Then
                                    '                    Validita_Fine_Utilizzo = CDate(xUtilizzo.GetAttribute("validita_fine"))
                                    '                End If

                                    '                Select Case CInt(xUtilizzo.GetAttribute("TipoOperazioneDB"))

                                    '                    Case enum_TipoOperazioneDB.Scrittura

                                    '                        Try

                                    '                            Dummy = objParticellexMacrousixUtilizzo_W.Scrivi(CStr(xUtilizzo.GetAttribute("prov")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("com")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("sezione")), _
                                    '                                                                    CLng(xUtilizzo.GetAttribute("foglio")), _
                                    '                                                                    CLng(xUtilizzo.GetAttribute("numero")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("subalterno")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("macrouso_cod")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("veg_cod_agea")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("cul_cod_agea")), _
                                    '                                                                    CDbl(xUtilizzo.GetAttribute("superficie")), _
                                    '                                                                    Validita_Inizio_Utilizzo, Validita_Fine_Utilizzo, objParametri)
                                    '                        Catch ex As Exception

                                    '                        End Try

                                    '                    Case enum_TipoOperazioneDB.Modifica

                                    '                        Try
                                    '                            Dummy = objParticellexMacrousixUtilizzo_W.Modifica(CStr(xUtilizzo.GetAttribute("prov")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("com")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("sezione")), _
                                    '                                                                    CLng(xUtilizzo.GetAttribute("foglio")), _
                                    '                                                                    CLng(xUtilizzo.GetAttribute("numero")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("subalterno")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("macrouso_cod")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("veg_cod_agea")), _
                                    '                                                                    CStr(xUtilizzo.GetAttribute("cul_cod_agea")), _
                                    '                                                                    CDbl(xUtilizzo.GetAttribute("superficie")), _
                                    '                                                                    Validita_Inizio_Utilizzo, Validita_Fine_Utilizzo, _
                                    '                                                                    "", _
                                    '                                                                    objParametri)
                                    '                        Catch ex As Exception

                                    '                        End Try

                                    '                End Select

                                    '                i_Utilizzo = i_Utilizzo + 1

                                    '            Loop

                                    '        End If

                                    '        i_Macrouso = i_Macrouso + 1

                                    '    Loop

                                    'End If

                                    i_Particella = i_Particella + 1

                                Loop

                                i_DatiParticelle = i_DatiParticelle + 1

                            Loop

                            '
                            'Scrivi Dato Cartografico
                            '
                            'ObjParticelle_W.CrealayerGrafici(objParametri)

#End Region

#Region "CAMPO"
                            '----------------------------------------------------
                            '############################################################
                            '######################    CAMPO    #########################
                            '############################################################

                            '-----------------------------------------------------------
                            xDatiCampi = xCentro.GetElementsByTagName("DatiCampi")

                            i_DatiCampo = 0

                            Do While i_DatiCampo < xDatiCampi.Count

                                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                                xDatiCampo = xDatiCampi.Item(i_DatiCampo)

                                '------------------------------

                                xCampi = xDatiCampo.GetElementsByTagName("Campo")

                                i_Campo = 0

                                Do While i_Campo < xCampi.Count

                                    'Prelevo l' i-esimo Campo
                                    xCampo = xCampi.Item(i_Campo)

                                    'Invio al componente di gestione appezzamenti
                                    '                     Set objCampi_W = CreateObject("Agro_Anagrafe.Campo_W")

                                    If Cod_Struttura <> 0 Then

                                        DatiCampo = Replace(xCampo.OuterXml,
                                                         "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                         "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")
                                    Else

                                        DatiCampo = xCampo.OuterXml

                                    End If


                                    Dummy = objCampi_W.Campo_Scrivi("<DatiCampi>" & DatiCampo & "</DatiCampi>",
                                                                        OUTPUT_Piva,
                                                                        Cod_Struttura,
                                                                        Cod_Campo,
                                                                        False,
                                                                        objParametri,
                                                                        objParametri_Utenti)

                                    If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                        objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                             0,
                                                             OUTPUT_Piva,
                                                             Cod_Struttura,
                                                             Cod_Campo,
                                                             0, 0,
                                                             0,
                                                             "", "", "", 0, 0, "", "", "",
                                                             0, 0, 0,
                                                             xFascicolo.GetAttribute("validita_inizio"),
                                                             xFascicolo.GetAttribute("validita_fine"),
                                                             objParametri)
                                    End If

                                    'Set objCampi_W = Nothing

#End Region

#Region "APPEZZAMENTO"
                                    '----------------------------------------------------
                                    '############################################################
                                    '##################  APPEZZAMENTO  ##########################
                                    '############################################################

                                    xDatiAppezzamenti = xCampo.GetElementsByTagName("DatiAppezzamenti")

                                    i_DatiAppezzamento = 0

                                    Do While i_DatiAppezzamento < xDatiAppezzamenti.Count

                                        'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                                        xDatiAppezzamento = xDatiAppezzamenti.Item(i_DatiAppezzamento)

                                        '------------------------------

                                        xAppezzamenti = xDatiAppezzamento.GetElementsByTagName("Appezzamento")

                                        i_Appezzamento = 0

                                        Do While i_Appezzamento < xAppezzamenti.Count

                                            'Prelevo l' i-esimo Appezzamento
                                            xAppezzamento = xAppezzamenti.Item(i_Appezzamento)

                                            '---------------------------------------------


                                            'Invio al componente di gestione appezzamenti
                                            '                         Set objAppezzamenti_W = CreateObject("Agro_Anagrafe.Appezzamento_W")


                                            DatiAppezzamento = Replace(xAppezzamento.OuterXml,
                                                                         "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                                         "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                            DatiAppezzamento = Replace(DatiAppezzamento,
                                                                         "campo_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                                         "campo_cod=" & Chr(34) & CStr(Cod_Campo) & Chr(34) & "")

                                            'Faccio una copia del nodo precedente
                                            xmlDocClone2.LoadXml(DatiAppezzamento)
                                            xAppezzamentoClone = xmlDocClone2.GetElementsByTagName("Appezzamento")(0)

                                            'Estraggo il nodo figlio "DatiCentriAziendali"
                                            xDatiImpiantiClone = xAppezzamentoClone.GetElementsByTagName("DatiReg_Impianti")

                                            'Elimino i dati relativi ai Centri_Aziendali dal nodo clonato
                                            If xDatiImpiantiClone.Count > 0 Then
                                                xNodoDummy = xAppezzamentoClone.RemoveChild(xDatiImpiantiClone.Item(0))
                                            End If


                                            Dummy = objAppezzamenti_W.Appezzamento_Scrivi(
                                                                                "<DatiAppezzamenti>" & xAppezzamentoClone.OuterXml & "</DatiAppezzamenti>",
                                                                                OUTPUT_Piva,
                                                                                Cod_Struttura,
                                                                                Cod_Appezzamento,
                                                                                objParametri,
                                                                                objParametri_Utenti)

                                            If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                                objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                                     0,
                                                                     OUTPUT_Piva,
                                                                     Cod_Struttura,
                                                                     0,
                                                                     Cod_Appezzamento,
                                                                     0,
                                                                     0,
                                                                     "", "", "", 0, 0, "", "", "",
                                                                     0, 0, 0,
                                                                     xFascicolo.GetAttribute("validita_inizio"),
                                                                     xFascicolo.GetAttribute("validita_fine"),
                                                                     objParametri)
                                            End If

                                            'Set objAppezzamenti_W = Nothing

                                            i_Appezzamento = i_Appezzamento + 1

#End Region

#Region "IMPIANTO"
                                            '----------------------------------------------------
                                            '############################################################
                                            '####################     IMPIANTO       ####################
                                            '############################################################

                                            xDatiImpianti = xAppezzamento.GetElementsByTagName("DatiReg_Impianti")

                                            i_DatiImpianto = 0

                                            Do While i_DatiImpianto < xDatiImpianti.Count

                                                'Prelevo l'i-esimo blocco di DatiImpianti (in realta' ne esiste uno solo)
                                                xDatiImpianto = xDatiImpianti.Item(i_DatiImpianto)

                                                '------------------------------

                                                xImpianti = xDatiImpianto.GetElementsByTagName("Reg_Impianto")

                                                i_Impianto = 0

                                                Do While i_Impianto < xImpianti.Count

                                                    'Prelevo l' i-esimo Impianto
                                                    xImpianto = xImpianti.Item(i_Impianto)

                                                    'Sostituisco al sa_cod=0 il codice che mi ha restituito la chiamata al componente
                                                    'di scrittura del Centro_Aziendale

                                                    If Cod_Struttura <> 0 Then

                                                        DatiImpianto = Replace(xImpianto.OuterXml,
                                                                            "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                                            "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                                    Else

                                                        DatiImpianto = xImpianto.OuterXml

                                                    End If

                                                    'Sostituisco all'Appezza=0 il codice che mi ha restituito la chiamata al componente
                                                    'di scrittura dell'Appezzamento
                                                    DatiImpianto = Replace(DatiImpianto,
                                                                        "appezza=" & Chr(34) & "0" & Chr(34) & "",
                                                                        "appezza=" & Chr(34) & CStr(Cod_Appezzamento) & Chr(34) & "")

                                                    DatiImpianto = "<DatiReg_Impianti>" & DatiImpianto & "</DatiReg_Impianti>"

                                                    'Invio al componente di gestione Impianti
                                                    '                                  Set ObjImpianti_W = CreateObject("Agro_Anagrafe.Reg_Impianto_W")


                                                    Dummy = ObjImpianti_W.Reg_Impianto_Scrivi(DatiImpianto, OUTPUT_Piva,
                                                                                                      Cod_Struttura,
                                                                                                      Cod_Appezzamento,
                                                                                                      Cod_Impianto,
                                                                                                      "", objParametri)

                                                    If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                                        objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                                             0,
                                                                             OUTPUT_Piva,
                                                                             Cod_Struttura,
                                                                             0,
                                                                             Cod_Appezzamento,
                                                                             Cod_Impianto,
                                                                             0,
                                                                             "", "", "", 0, 0, "", "", "",
                                                                             0, 0, 0,
                                                                             xFascicolo.GetAttribute("validita_inizio"),
                                                                             xFascicolo.GetAttribute("validita_fine"),
                                                                             objParametri)
                                                    End If

                                                    'Set ObjImpianti_W = Nothing


                                                    'MODIFICA DEL 24/02/2011: 
                                                    'IMPRESE_PROGETTI VIENE GIA' GESTITO DAGLI IMPIANTI

#End Region

#Region "IMPRESE PROGETTI"
                                                    '----------------------------------------------------
                                                    '    '############################################################
                                                    '    '#####################   IMPRESE_PROGETTI  ##################
                                                    '    '############################################################

                                                    '    xDatiImprese_Progetti = xImpianto.GetElementsByTagName("Progetto")

                                                    '    i_DatiImprese_Progetti = 0

                                                    '    'Dovrebbe esserci un solo nodo
                                                    '    Do While i_DatiImprese_Progetti < xDatiImprese_Progetti.Count

                                                    '        xImprese_Progetti = xDatiImprese_Progetti(i_DatiImprese_Progetti)

                                                    '        'Invio al componente di gestione appezzamenti
                                                    '        sImprese_Progetti = xImprese_Progetti.OuterXml

                                                    '        If Cod_Struttura <> 0 Then
                                                    '            '                        'Rimpiazzo i codici 0 con i nuovi codici
                                                    '            sImprese_Progetti = Replace(sImprese_Progetti, _
                                                    '                                        "sa_cod=" & Chr(34) & "0" & Chr(34), _
                                                    '                                        "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34))
                                                    '        End If

                                                    '        sImprese_Progetti = Replace(sImprese_Progetti, _
                                                    '                                    "appezza=" & Chr(34) & "0" & Chr(34), _
                                                    '                                    "appezza=" & Chr(34) & CStr(Cod_Appezzamento) & Chr(34))
                                                    '        '
                                                    '        sImprese_Progetti = Replace(sImprese_Progetti, _
                                                    '                                    "id_reg=" & Chr(34) & "0" & Chr(34), _
                                                    '                                    "id_reg=" & Chr(34) & CStr(Cod_Impianto) & Chr(34))

                                                    '        Dummy = objProgetto_W.Impresa_Progetto_Scrivi( _
                                                    '                                            "<DatiProgetto>" & sImprese_Progetti & "</DatiProgetto>", _
                                                    '                                            OUTPUT_Piva, Cod_Progetto, objParametri)

                                                    '        xImprese_Progetti = Nothing

                                                    '        i_DatiImprese_Progetti = i_DatiImprese_Progetti + 1

                                                    '    Loop

                                                    '    xDatiImprese_Progetti = Nothing

                                                    '------------------------------------------------------

                                                    i_Impianto = i_Impianto + 1

                                                Loop

                                                i_DatiImpianto = i_DatiImpianto + 1

                                                '<\Impianti>
                                            Loop



                                        Loop

                                        i_DatiAppezzamento = i_DatiAppezzamento + 1

                                    Loop

                                    i_Campo = i_Campo + 1

                                Loop

                                '---------------------------------------------

                                i_DatiCampo = i_DatiCampo + 1

                            Loop


                            '----------------------------------------------------
                            'Rimuovo il nodo DatiCampi se esiste
                            If xDatiCampi.Count > 0 Then

                                xNodoDummy = xCentro.RemoveChild(xDatiCampi.Item(0))

                            End If
                            '----------------------------------------------------

#End Region


#Region "APPEZZAMENTO"

                            '----------------------------------------------------
                            '############################################################
                            '##################  APPEZZAMENTO  ##########################
                            '############################################################

                            xDatiAppezzamenti = xCentro.GetElementsByTagName("DatiAppezzamenti")

                            i_DatiAppezzamento = 0

                            Do While i_DatiAppezzamento < xDatiAppezzamenti.Count

                                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                                xDatiAppezzamento = xDatiAppezzamenti.Item(i_DatiAppezzamento)

                                '------------------------------

                                xAppezzamenti = xDatiAppezzamento.GetElementsByTagName("Appezzamento")

                                i_Appezzamento = 0

                                Do While i_Appezzamento < xAppezzamenti.Count

                                    'Prelevo l' i-esimo Appezzamento
                                    xAppezzamento = xAppezzamenti.Item(i_Appezzamento)

                                    '                     '---------------------------------------------

                                    'Invio al componente di gestione appezzamenti
                                    '                     Set objAppezzamenti_W = CreateObject("Agro_Anagrafe.Appezzamento_W")

                                    If Cod_Struttura <> 0 Then

                                        DatiAppezzamento = Replace(xAppezzamento.OuterXml,
                                                              "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                              "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                    Else

                                        DatiAppezzamento = xAppezzamento.OuterXml

                                    End If

                                    'Faccio una copia del nodo precedente
                                    xmlDocClone2.LoadXml(DatiAppezzamento)
                                    xAppezzamentoClone = xmlDocClone2.GetElementsByTagName("Appezzamento")(0)

                                    'Estraggo il nodo figlio "DatiReg_Impianti"
                                    xDatiImpiantiClone = xAppezzamentoClone.GetElementsByTagName("DatiReg_Impianti")

                                    'Elimino i dati relativi ai Centri_Aziendali dal nodo clonato
                                    If xDatiImpiantiClone.Count > 0 Then
                                        xNodoDummy = xAppezzamentoClone.RemoveChild(xDatiImpiantiClone.Item(0))
                                    End If


                                    Dummy = objAppezzamenti_W.Appezzamento_Scrivi(
                                                                "<DatiAppezzamenti>" & xAppezzamentoClone.OuterXml & "</DatiAppezzamenti>",
                                                                OUTPUT_Piva,
                                                                Cod_Struttura,
                                                                Cod_Appezzamento,
                                                                objParametri,
                                                                objParametri_Utenti)

                                    If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                        objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                             0,
                                                             OUTPUT_Piva,
                                                             Cod_Struttura,
                                                             0,
                                                             Cod_Appezzamento,
                                                             0,
                                                             0,
                                                             "", "", "", 0, 0, "", "", "",
                                                             0, 0, 0,
                                                             xFascicolo.GetAttribute("validita_inizio"),
                                                             xFascicolo.GetAttribute("validita_fine"),
                                                             objParametri)
                                    End If

                                    'Set objAppezzamenti_W = Nothing

                                    i_Appezzamento = i_Appezzamento + 1

#End Region

#Region "IMPIANTO"
                                    '----------------------------------------------------
                                    '############################################################
                                    '####################     IMPIANTO       ####################
                                    '############################################################


                                    xDatiImpianti = xAppezzamento.GetElementsByTagName("DatiReg_Impianti")

                                    i_DatiImpianto = 0

                                    Do While i_DatiImpianto < xDatiImpianti.Count

                                        'Prelevo l'i-esimo blocco di DatiImpianti (in realta' ne esiste uno solo)
                                        xDatiImpianto = xDatiImpianti.Item(i_DatiImpianto)

                                        '------------------------------

                                        xImpianti = xDatiImpianto.GetElementsByTagName("Reg_Impianto")

                                        i_Impianto = 0

                                        Do While i_Impianto < xImpianti.Count

                                            'Prelevo l' i-esimo Impianto
                                            xImpianto = xImpianti.Item(i_Impianto)

                                            If Cod_Struttura <> 0 Then

                                                'Sostituisco al sa_cod=0 il codice che mi ha restituito la chiamata al componente
                                                'di scrittura del Centro_Aziendale
                                                DatiImpianto = Replace(xImpianto.OuterXml,
                                                                  "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                                  "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                            Else

                                                DatiImpianto = xImpianto.OuterXml

                                            End If

                                            'Sostituisco all'Appezza=0 il codice che mi ha restituito la chiamata al componente
                                            'di scrittura dell'Appezzamento
                                            DatiImpianto = Replace(DatiImpianto,
                                                                "appezza=" & Chr(34) & "0" & Chr(34) & "",
                                                                "appezza=" & Chr(34) & CStr(Cod_Appezzamento) & Chr(34) & "")

                                            DatiImpianto = "<DatiReg_Impianti>" & DatiImpianto & "</DatiReg_Impianti>"


                                            'Invio al componente di gestione Impianti
                                            Dummy = ObjImpianti_W.Reg_Impianto_Scrivi(DatiImpianto, OUTPUT_Piva,
                                                                                    Cod_Struttura, Cod_Appezzamento, Cod_Impianto,
                                                                                    "", objParametri)

                                            If FascicoloPresente AndAlso Allegati_Documenti_Cod <> 0 Then
                                                objFascicoloEntita.Scrivi(Allegati_Documenti_Cod,
                                                                     0,
                                                                     OUTPUT_Piva,
                                                                     Cod_Struttura,
                                                                     0,
                                                                     Cod_Appezzamento,
                                                                     Cod_Impianto,
                                                                     0,
                                                                     "", "", "", 0, 0, "", "", "",
                                                                     0, 0, 0,
                                                                     xFascicolo.GetAttribute("validita_inizio"),
                                                                     xFascicolo.GetAttribute("validita_fine"),
                                                                     objParametri)
                                            End If
                                            'Set ObjImpianti_W = Nothing

                                            'MODIFICA DEL 24/02/2011: 
                                            'IMPRESE_PROGETTI VIENE GIA' GESTITO DAGLI IMPIANTI

#End Region

#Region "IMPRESE PROGETTI"
                                            '----------------------------------------------------
                                            ''############################################################
                                            ''#####################   IMPRESE_PROGETTI  ##################
                                            ''############################################################

                                            'xDatiImprese_Progetti = xImpianto.GetElementsByTagName("Progetto")

                                            'i_DatiImprese_Progetti = 0

                                            ''Dovrebbe esserci un solo nodo
                                            'Do While i_DatiImprese_Progetti < xDatiImprese_Progetti.Count

                                            '    xImprese_Progetti = xDatiImprese_Progetti(i_DatiImprese_Progetti)

                                            '    'Invio al componente di gestione appezzamenti
                                            '    sImprese_Progetti = xImprese_Progetti.OuterXml

                                            '    If Cod_Struttura <> 0 Then
                                            '        '                           'Rimpiazzo i codici 0 con i nuovi codici
                                            '        sImprese_Progetti = Replace(sImprese_Progetti, _
                                            '                                "sa_cod=" & Chr(34) & "0" & Chr(34), _
                                            '                                "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34))

                                            '    End If

                                            '    sImprese_Progetti = Replace(sImprese_Progetti, _
                                            '                                "appezza=" & Chr(34) & "0" & Chr(34), _
                                            '                                "appezza=" & Chr(34) & CStr(Cod_Appezzamento) & Chr(34))

                                            '    sImprese_Progetti = Replace(sImprese_Progetti, _
                                            '                                "id_reg=" & Chr(34) & "0" & Chr(34), _
                                            '                                "id_reg=" & Chr(34) & CStr(Cod_Impianto) & Chr(34))

                                            '    Dummy = objProgetto_W.Impresa_Progetto_Scrivi( _
                                            '                                "<DatiProgetto>" & sImprese_Progetti & "</DatiProgetto>", _
                                            '                                OUTPUT_Piva, Cod_Progetto, objParametri)

                                            '    xImprese_Progetti = Nothing

                                            '    i_DatiImprese_Progetti = i_DatiImprese_Progetti + 1

                                            'Loop

                                            'xDatiImprese_Progetti = Nothing


                                            '------------------------------------------------------

                                            i_Impianto = i_Impianto + 1

                                        Loop

                                        i_DatiImpianto = i_DatiImpianto + 1

                                        '<\Impianti>
                                    Loop



                                Loop

                                i_DatiAppezzamento = i_DatiAppezzamento + 1

                            Loop

                            '</Appezzamenti>
#End Region

#Region "AGENDA"
                            '----------------------------------------------------
                            '############################################################
                            '#######################    AGENDA       ####################
                            '############################################################


                            xDatiAgende = xCentro.GetElementsByTagName("DatiAgenda")

                            'Se esiste il nodo DatiAgenda
                            If xDatiAgende IsNot Nothing Then

                                i_DatiAgenda = 0

                                Do While i_DatiAgenda < xDatiAgende.Count

                                    'Prelevo l'i-esimo blocco di DatiAgende (in realtà ne esiste uno solo)
                                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

                                    '------------------------------

                                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                                    i_Agenda = 0

                                    Do While i_Agenda < xAgende.Count

                                        'Prelevo l' i-esimo Agenda
                                        xAgenda = xAgende.Item(i_Agenda)

                                        DatiAgenda = "<DatiAgenda>" & xAgenda.OuterXml & "</DatiAgenda>"

                                        If Cod_Struttura <> 0 Then

                                            DatiAgenda = Replace(DatiAgenda,
                                                                "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                                "sa_cod=" & Chr(34) & CStr(Cod_Struttura) & Chr(34) & "")

                                            '                           Else
                                            '
                                            '                                DatiAgenda = xAgenda.xml

                                        End If

                                        Lav_Cod = xAgenda.GetAttribute("lav_cod")

                                        Select Case Lav_Cod

                                            'operazioni zootecniche
                                            Case 3000 To 3999

                                                xMovimenti_Destinazione = xAgenda.GetElementsByTagName("Movimento_Destinazione")
                                                xMovimento_Destinazione = xMovimenti_Destinazione(0)

                                                Fabbricato_Cod_Cliente = xMovimento_Destinazione.GetAttribute("id_destinazione")

                                                'Ricavo da fabbricati_codici il fabbricato_cod relativo
                                                objFabbricatixCodici = New AgronicaCoreAnagrafeDAL.Fabbricati_Codici_R

                                                DtFabbricatixCodici = objFabbricatixCodici.Leggi(CStr(Agro_SQL_Load(xAgenda.GetAttribute("piva"))),
                                                                                    CInt(Agro_SQL_Load(Cod_Struttura)),
                                                                                    0,
                                                                                    0,
                                                                                    CStr(Agro_SQL_Load(Fabbricato_Cod_Cliente)),
                                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                    "",
                                                                                    "",
                                                                                    objParametri)

                                                If DtFabbricatixCodici IsNot Nothing Then

                                                    Dim iTemp As Integer

                                                    For iTemp = 0 To DtFabbricatixCodici.Rows.Count - 1

                                                        Fabbricato_Cod = DtFabbricatixCodici.Rows(iTemp).Item("fabbricato_cod")

                                                    Next iTemp

                                                    DtFabbricatixCodici.Dispose()

                                                End If

                                                DtFabbricatixCodici = Nothing
                                                objFabbricatixCodici = Nothing

                                                If Fabbricato_Cod <> 0 Then

                                                    DatiAgenda = Replace(DatiAgenda,
                                                                        "id_destinazione=" & Chr(34) & CStr(Fabbricato_Cod_Cliente) & Chr(34) & "",
                                                                        "id_destinazione=" & Chr(34) & CStr(Fabbricato_Cod) & Chr(34) & "")


                                                End If


                                        End Select

                                        'Invio al componente di gestione Agende
                                        '                          Set ObjAgenda_W = CreateObject("Agro_Contab.Agenda_W")

                                        Dummy = ObjAgenda_W.Agenda_Scrivi(DatiAgenda,
                                                                           Cod_Agenda,
                                                                           Flag_Mirror,
                                                                           Id_Servizio,
                                                                           Rimappa_Codici,
                                                                           objParametri.PivaSuperUser,
                                                                           objParametri)

                                        'Set ObjAgenda_W = Nothing

                                        i_Agenda = i_Agenda + 1

                                    Loop

                                    i_DatiAgenda = i_DatiAgenda + 1

                                    '<\Agende>

                                Loop


                            End If
#End Region

                            i_Centro = i_Centro + 1

                        Loop

                        i_DatiCentro = i_DatiCentro + 1


                    Loop

                    i_Impresa = i_Impresa + 1


                Loop

                i_DatiImpresa = i_DatiImpresa + 1

            Loop

            'Restituisco un valore Dummy
            xRisp = True

            ''Se ho la transazione è stata avviata in questa routine faccio il commit
            'If FlagTransazioneLocale = True Then
            '    objParametri.objTransazione.Commit()
            'End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)


        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = "(Piva=" & OUTPUT_Piva & ")" & " : " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (FlagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
            '    objParametri.objConnessione.Close()
            'End If
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

        Return xRisp

    End Function

End Class
