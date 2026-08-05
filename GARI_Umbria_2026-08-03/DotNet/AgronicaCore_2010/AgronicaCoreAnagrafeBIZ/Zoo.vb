Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Zoo
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Modifica_Zoo_Animali(ByVal Zoo_Animali As AgronicaCoreEntityFramework_POCO.Zoo_Animali,
                                                ByRef Zoo_Animali_Distinte As AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte(),
                                                ByRef Zoo_Animali_Stati_Accrescimento As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento(),
                                                ByRef objParametriServer As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                Optional OpenNewTransaction As Boolean = True,
                                                Optional ByRef Zoo_Animali_Anomalie As AgronicaCoreEntityFramework_POCO.Zoo_AnimalixAnomalie() = Nothing,
                                                Optional ByVal Edit_in_Griglia As Boolean = False,
                                                Optional ByVal ScriviLog As Boolean = True,
                                                Optional ByVal NoteLog As String = "") As RispostaStandard
        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Zoo.Scrivi_Modifica_Zoo_Animali()"
        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If

        Try

            Dim obj_rispostaStringa As New JObject

            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            Dim obj_zoo_animali_distinte As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte
            Dim obj_zoo_animali_stati As New AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento
            Dim obj_zoo_animali_anomalie As New AgronicaCoreAnagrafeDAL.Zoo_AnimalixAnomalie

            Dim dateSovrapposteDistinte As Boolean = False
            Dim dateSovrapposteStati As Boolean = False

            'Controllo le date delle distinte
            For i = 0 To Zoo_Animali_Distinte.Length - 1

                For j = 0 To Zoo_Animali_Distinte.Length - 1

                    If i <> j Then

                        Dim dataInizioCheck As DateTime = Zoo_Animali_Distinte(i).Validita_Inizio
                        Dim dataFineCheck As DateTime = Zoo_Animali_Distinte(i).Validita_Fine

                        Dim dataInizio As DateTime = Zoo_Animali_Distinte(j).Validita_Inizio
                        Dim dataFine As DateTime = Zoo_Animali_Distinte(j).Validita_Fine

                        If dataInizioCheck > dataInizio AndAlso dataInizioCheck < dataFine Then
                            'ERRORE LE DATE SI SOVRAPPONGONO
                            dateSovrapposteDistinte = True
                            Exit For
                        End If

                        If dataFineCheck > dataInizio AndAlso dataFineCheck < dataFine Then
                            'ERRORE LE DATE SI SOVRAPPONGONO
                            dateSovrapposteDistinte = True
                            Exit For
                        End If

                    End If

                Next

                If dateSovrapposteDistinte Then
                    Exit For
                End If

            Next

            If Not IsNothing(Zoo_Animali_Stati_Accrescimento) AndAlso Zoo_Animali_Stati_Accrescimento.Count > 0 Then
                For i = 0 To Zoo_Animali_Stati_Accrescimento.Length - 1

                    For j = 0 To Zoo_Animali_Stati_Accrescimento.Length - 1

                        If i <> j Then

                            Dim dataInizioCheck As DateTime = AGRODATAINIZIO
                            If (Zoo_Animali_Stati_Accrescimento(i).Validita_Inizio IsNot Nothing) Then
                                dataInizioCheck = Zoo_Animali_Stati_Accrescimento(i).Validita_Inizio
                            End If

                            Dim dataFineCheck As DateTime = AGRODATAFINE
                            If Zoo_Animali_Stati_Accrescimento(i).Validita_Fine IsNot Nothing Then
                                dataFineCheck = Zoo_Animali_Stati_Accrescimento(i).Validita_Fine
                            End If


                            Dim dataInizio As DateTime = AGRODATAINIZIO
                            If Zoo_Animali_Stati_Accrescimento(j).Validita_Inizio IsNot Nothing Then
                                dataInizio = Zoo_Animali_Stati_Accrescimento(j).Validita_Inizio
                            End If

                            Dim dataFine As DateTime = AGRODATAFINE
                            If Zoo_Animali_Stati_Accrescimento(j).Validita_Fine IsNot Nothing Then
                                dataFine = Zoo_Animali_Stati_Accrescimento(j).Validita_Fine
                            End If

                            If dataInizioCheck > dataInizio AndAlso dataInizioCheck < dataFine Then
                                'ERRORE LE DATE SI SOVRAPPONGONO
                                dateSovrapposteStati = True
                                Exit For
                            End If

                            If dataFineCheck > dataInizio AndAlso dataFineCheck < dataFine Then
                                'ERRORE LE DATE SI SOVRAPPONGONO
                                dateSovrapposteStati = True
                                Exit For
                            End If

                        End If

                    Next

                    If dateSovrapposteStati Then
                        Exit For
                    End If

                Next
            End If


            If dateSovrapposteDistinte OrElse dateSovrapposteStati Then
                obj_rispostaStringa.Item("Salvataggio_Effettuato") = False
                Dim strMess As String = ""
                If dateSovrapposteDistinte Then
                    strMess += "Le date delle distinte si sovrappongono, non è possibile procedere col salvataggio." & vbCrLf
                End If

                If dateSovrapposteStati Then
                    strMess += "Le date degli stati di accrescimento si sovrappongono, non è possibile procedere col salvataggio."
                End If

                obj_rispostaStringa.Item("Messaggio") = strMess
                obj_rispostaStringa.Item("Cod_Progetto") = "-1"
                r.RispostaStringa = obj_rispostaStringa.ToString
                r.RispostaOK = True
                Return r
            End If


            ' controllo movimenti collegati a distinte da cancellare
            Dim distinte_salvate = Zoo_Animali_Distinte.Select(Function(t) t.Cod_Progetto).ToList

            If Not Edit_in_Griglia Then
                Dim distinte_cancellate = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = Zoo_Animali.PIVA AndAlso a.Cod_Animale = Zoo_Animali.Cod_Progetto AndAlso Not distinte_salvate.Contains(a.Cod_Progetto))

                Dim objCDG As New AgronicaCoreContabDAL.CDG_DAL_R
                For Each distinta In distinte_cancellate
                    If objCDG.Verifica_CDG_Animali(Zoo_Animali.PIVA, Zoo_Animali.sa_cod, Zoo_Animali.Cod_Progetto, distinta.Cod_Progetto, objParametriServer) Then
                        obj_rispostaStringa.Item("Salvataggio_Effettuato") = False
                        obj_rispostaStringa.Item("Messaggio") = "Non è possibile cancellare esercizi con movimenti collegati!"
                        obj_rispostaStringa.Item("Cod_Progetto") = "-1"
                        r.RispostaStringa = obj_rispostaStringa.ToString
                        r.RispostaOK = True
                        Return r
                    End If
                Next
            End If

            'ANIMALE
            Dim tipo_Operazione As enum_TipoOperazioneDB
            Dim Cod_Progetto = 0
            If Zoo_Animali.Cod_Progetto = 0 Then

                Cod_Progetto = agroDP.NuovoId_Tabella_EF(GiasContext, "Zoo_Animali", 0, 200000000, objParametriServer)

                Zoo_Animali.Cod_Progetto = Cod_Progetto
                obj_zoo_animali.Scrivi(Zoo_Animali, GiasContext, objParametriServer)
                tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            Else
                Cod_Progetto = Zoo_Animali.Cod_Progetto

                Select Case Edit_in_Griglia
                    Case False
                        obj_zoo_animali.Modifica(Zoo_Animali, GiasContext, objParametriServer)
                    Case True
                        obj_zoo_animali.ModificaGriglia(Zoo_Animali, objParametriServer)
                End Select
                tipo_Operazione = enum_TipoOperazioneDB.Modifica



            End If

            'GiasContext.SaveChanges()

            'DISTINTE

            Dim algoritmo_codifica = Replica_GIAS.LeggiAlgoritmoCodifica(Zoo_Animali.PIVA, objParametriServer)
            Dim list_distinte As New List(Of Integer)

            For Each distinta In Zoo_Animali_Distinte

                ' generazione automatica codice per distinta
                If algoritmo_codifica <> "" AndAlso String.IsNullOrEmpty(distinta.Codice_Distinta) Then
                    Dim progetto_anno = Zoo_Animali.Validita_Inizio.Year
                    If distinta.Validita_Inizio IsNot Nothing Then
                        progetto_anno = CDate(distinta.Validita_Inizio).Year
                    End If
                    Dim progetto_codice = Replica_GIAS.LeggiCodiceProgressivo(Zoo_Animali.PIVA, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, progetto_anno, objParametriServer)
                    distinta.Codice_Distinta = progetto_codice
                End If

                distinta.Cod_Animale = Cod_Progetto
                Dim cod_distinta = 0
                If distinta.Cod_Progetto = 0 Then
                    'SCRIVO
                    cod_distinta = agroDP.NuovoId_Tabella_EF(GiasContext, "Zoo_Animali_Distinte", 0, 200000000, objParametriServer)

                    distinta.Cod_Progetto = cod_distinta
                    obj_zoo_animali_distinte.Scrivi(distinta, GiasContext, objParametriServer)

                Else
                    'MODIFICO
                    cod_distinta = distinta.Cod_Progetto

                    Select Case Edit_in_Griglia
                        Case False
                            obj_zoo_animali_distinte.Modifica(distinta, GiasContext, objParametriServer)
                        Case True
                            obj_zoo_animali_distinte.ModificaGriglia(distinta.PIVA, distinta.Cod_Animale, distinta.Cod_Progetto, distinta.Codice_Distinta, objParametriServer)
                    End Select

                End If
                list_distinte.Add(cod_distinta)
                'GiasContext.SaveChanges()
            Next

            If Not Edit_in_Griglia Then
                Dim distinte_del = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = Zoo_Animali.PIVA AndAlso a.Cod_Animale = Zoo_Animali.Cod_Progetto AndAlso Not list_distinte.Contains(a.Cod_Progetto))

                For Each distinta In distinte_del
                    GiasContext.Zoo_Animali_Distinte.Remove(distinta)
                Next
            End If



            'STATI ACCRESCIMENTO 
            Dim list_stati As New List(Of Integer)
            If Not IsNothing(Zoo_Animali_Stati_Accrescimento) AndAlso Zoo_Animali_Stati_Accrescimento.Count > 0 Then

                For Each stato In Zoo_Animali_Stati_Accrescimento

                    If stato.Cod_Progetto = 0 Then
                        'SCRIVO

                        stato.Cod_Progetto = Cod_Progetto
                        obj_zoo_animali_stati.Scrivi(stato, GiasContext, objParametriServer)

                    Else

                        Dim stati = From sa In GiasContext.Zoo_AnimalixStati_Accrescimento
                                    Where sa.PIVA = stato.PIVA AndAlso
                                        sa.sa_cod = stato.sa_cod AndAlso
                                        sa.Cod_Progetto = stato.Cod_Progetto AndAlso
                                        sa.GEN_COD = stato.GEN_COD AndAlso
                                        sa.SPE_COD = stato.SPE_COD AndAlso
                                        sa.TIPO_COD = stato.TIPO_COD AndAlso
                                        sa.STATO_COD = stato.STATO_COD
                                    Select sa

                        If stati.Count = 0 Then
                            'SCRIVO
                            obj_zoo_animali_stati.Scrivi(stato, GiasContext, objParametriServer)

                        Else
                            'MODIFICO
                            obj_zoo_animali_stati.Modifica(stato, GiasContext, objParametriServer)
                        End If

                    End If
                    list_stati.Add(stato.STATO_COD)
                    'GiasContext.SaveChanges()
                Next



                Dim stati_del = (From a In GiasContext.Zoo_AnimalixStati_Accrescimento
                                 Where a.PIVA = Zoo_Animali.PIVA AndAlso
                                       a.sa_cod = Zoo_Animali.sa_cod AndAlso
                                       a.Cod_Progetto = Zoo_Animali.Cod_Progetto AndAlso
                                       a.GEN_COD = Zoo_Animali.GEN_COD AndAlso
                                       a.SPE_COD = Zoo_Animali.SPE_COD AndAlso
                                       a.TIPO_COD = Zoo_Animali.TIPO_COD AndAlso
                                       Not list_stati.Contains(a.STATO_COD))

                For Each stato In stati_del
                    GiasContext.Zoo_AnimalixStati_Accrescimento.Remove(stato)
                    'GiasContext.SaveChanges()
                Next

            End If


            'ANOMALIE 
            Dim list_anomalie As New List(Of Integer)
            If Not IsNothing(Zoo_Animali_Anomalie) Then

                If Zoo_Animali_Anomalie.Count > 0 Then
                    For Each anomalia In Zoo_Animali_Anomalie
                        If anomalia.Cod_Animale = 0 Then
                            'SCRIVO
                            anomalia.Cod_Animale = Cod_Progetto
                            obj_zoo_animali_anomalie.Scrivi(anomalia, GiasContext, objParametriServer)
                        Else
                            Dim anomalie = From an In GiasContext.Zoo_AnimalixAnomalie
                                           Where an.Piva = anomalia.Piva AndAlso
                                               an.Sa_Cod = anomalia.Sa_Cod AndAlso
                                               an.Cod_Animale = anomalia.Cod_Animale AndAlso
                                               an.Codice = anomalia.Codice
                                           Select an
                            If anomalie.Count = 0 Then
                                'SCRIVO
                                obj_zoo_animali_anomalie.Scrivi(anomalia, GiasContext, objParametriServer)
                            Else
                                'MODIFICO
                                obj_zoo_animali_anomalie.Modifica(anomalia, GiasContext, objParametriServer)
                            End If
                        End If
                        list_anomalie.Add(anomalia.Codice)
                    Next
                End If

                Dim anomalie_del = (From a In GiasContext.Zoo_AnimalixAnomalie
                                    Where a.Piva = Zoo_Animali.PIVA AndAlso
                                       a.Sa_Cod = Zoo_Animali.sa_cod AndAlso
                                       a.Cod_Animale = Zoo_Animali.Cod_Progetto AndAlso
                                       Not list_anomalie.Contains(a.Codice))

                For Each anomalia In anomalie_del
                    GiasContext.Zoo_AnimalixAnomalie.Remove(anomalia)
                    'GiasContext.SaveChanges()
                Next

            End If

            GiasContext.SaveChanges()
            obj_rispostaStringa.Item("Salvataggio_Effettuato") = True
            obj_rispostaStringa.Item("Messaggio") = "Salvataggio effettuato correttamente."
            obj_rispostaStringa.Item("Cod_Progetto") = CStr(Cod_Progetto)

            r.RispostaStringa = obj_rispostaStringa.ToString
            r.RispostaOK = True

            If ScriviLog Then
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}
                Dim DatiCapo As String = ""
                Dim zoo_animale_str = ""
                Dim zoo_animaliAnomalie_str = ""
                Dim zoo_animaliDistinte_str = ""
                Dim zoo_animaliStati_str = ""

                If Zoo_Animali IsNot Nothing Then
                    zoo_animale_str = JsonConvert.SerializeObject(Zoo_Animali, a)
                Else
                    zoo_animale_str = "null"
                End If

                If Zoo_Animali_Distinte IsNot Nothing Then
                    zoo_animaliDistinte_str = JsonConvert.SerializeObject(Zoo_Animali_Distinte, a)
                Else
                    zoo_animaliDistinte_str = "null"
                End If

                If Zoo_Animali_Stati_Accrescimento IsNot Nothing Then
                    zoo_animaliStati_str = JsonConvert.SerializeObject(Zoo_Animali_Stati_Accrescimento, a)
                Else
                    zoo_animaliStati_str = "null"
                End If

                If Zoo_Animali_Anomalie IsNot Nothing Then
                    zoo_animaliAnomalie_str = JsonConvert.SerializeObject(Zoo_Animali_Anomalie, a)
                Else
                    zoo_animaliAnomalie_str = "null"
                End If

                DatiCapo = "{ ""Zoo_Animali"" : " & zoo_animale_str & "," &
                            """Zoo_Animali_Distinte"" : " & zoo_animaliDistinte_str & "," &
                            """Zoo_AnimalixStati_Accrescimento"" : " & zoo_animaliStati_str & "," &
                            """Zoo_AnimalixAnomalie"" : " & zoo_animaliAnomalie_str & " }"
                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.Zoo_Animali,
                                                                                     CStr(Zoo_Animali.PIVA), CStr(Zoo_Animali.Cod_Progetto),
                                                                                     Nothing, Nothing,
                                                                                     Nothing, Nothing,
                                                                                     tipo_Operazione,
                                                                                     objParametriServer,
                                                                                     enum_Id_Servizio.GiasOnline,
                                                                                     NoteLog,
                                                                                     DatiCapo)

                GiasContext.Agronica_Log_Anagrafe.Add(log)
                GiasContext.SaveChanges()
            End If
            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If

            Dim exStrJSON As String = ""
            Try
                exStrJSON = JsonConvert.SerializeObject(ex)
            Catch ex1 As Exception
                Scrivi_LOG(objParametriServer, nomeRoutine, "Errore in JSON Serialize Object")
            End Try

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore &= "Exception Serializzata: " & exStrJSON
            r.RispostaOK = False
            Throw New Exception(r.Errore)

        Finally

            If bCloseContext Then
                'scope.Dispose()
                GiasContext.Dispose()
            End If


        End Try

        Return r


    End Function

    'Public Function Leggi_Giacenze(Piva As String,
    '                               Sa_Cod As Integer,
    '                               STA_NUM As Integer,
    '                               Raggruppamento_Cod As Integer,
    '                               Cod_Animale As Integer,
    '                               Data As DateTime,
    '                               ByRef objParametri As AgronicaCoreParametri
    '                               ) As DataTable

    '    Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Zoo.Leggi_Giacenze()"

    '    Dim messaggioErrore As String = ""
    '    Dim stb As New StringBuilder
    '    Dim dt As DataTable

    '    Try

    '        stb.Length = 0
    '        stb.AppendLine("SELECT * FROM ")
    '        stb.AppendLine("            (SELECT  ")
    '        stb.AppendLine("    Imprese.Piva, Imprese.Rag_Soc As Impresa, ")
    '        stb.AppendLine("    Movimenti_dettagli.Cod_Progetto AS Cod_Animale, ")
    '        stb.AppendLine("    Zoo_Animali_Distinte.Cod_Progetto, ")
    '        stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Des, ")
    '        stb.AppendLine("    Zoo_AnimalixStati_Accrescimento.Stato_Cod, ")
    '        stb.AppendLine("    Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des, ")

    '        stb.AppendLine("    Lista_Specie_Animali.SPE_DES, ")
    '        stb.AppendLine("    Lista_Razze_Animali.RAZ_DES, ")
    '        stb.AppendLine("    Zoo_Animali_Lista_Tipi.Tipo_Des, ")
    '        stb.AppendLine("    Zoo_Animali.Mat_Madre, ")
    '        stb.AppendLine("    Zoo_Animali.Mat_Padre, ")
    '        stb.AppendLine("    CASE Zoo_Animali.Metodo_Produzione WHEN 1 THEN 'Convenzionale' WHEN 2 THEN 'In Conversione' WHEN 3 THEN 'Biologico' END as Metodo_Produzione, ")
    '        stb.AppendLine("    Lista_IndirizziProd_Animali.IPRO_DES, ")
    '        stb.AppendLine("    Zoo_Animali_Distinte.Progetto_Nome, ")
    '        stb.AppendLine("    Zoo_Animali_Distinte.Codice_Distinta, ")
    '        stb.AppendLine("    Zoo_Animali.Lotto_Fornitore, ")
    '        stb.AppendLine("    Contatti.Rag_Soc, ")
    '        stb.AppendLine("    Contatti.Cod_Contatto,")

    '        stb.AppendLine("    Movimenti_dettagli.Lotto, ")
    '        stb.AppendLine("    Movimenti_dettagli.Udm_Cod, ")
    '        stb.AppendLine("    Mov_Destinazioni.Sa_Cod, ")
    '        stb.AppendLine("    Centri_Aziendali.sa_nome, ")
    '        stb.AppendLine("    Mov_Destinazioni.Id_Destinazione, ")
    '        stb.AppendLine("    Mov_Destinazioni.Tipo_Destinazione, ")
    '        stb.AppendLine("    UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, ")
    '        stb.AppendLine("    CASE  ")
    '        stb.AppendLine("    WHEN Mov_Destinazioni.Tipo_Destinazione = 15 ")
    '        stb.AppendLine("    THEN (SELECT Fabbricato_Des FROM Fabbricati WHERE Fabbricati.piva = Imprese.Piva And Fabbricati.SA_COD=Mov_Destinazioni.Sa_Cod And Fabbricati.Fabbricato_Cod=Mov_Destinazioni.Id_Destinazione) ")
    '        stb.AppendLine("    Else ( ")
    '        stb.AppendLine("      Select Fabbricato_Des FROM Stalla_Raggruppamenti ")
    '        stb.AppendLine("           INNER Join Fabbricati ON Fabbricati.Fabbricato_Cod = Stalla_Raggruppamenti.STA_NUM And Fabbricati.piva = Stalla_Raggruppamenti.piva And Fabbricati.SA_COD= Stalla_Raggruppamenti.sa_cod ")
    '        stb.AppendLine("           WHERE Stalla_Raggruppamenti.Raggruppamento_Cod = Mov_Destinazioni.Id_Destinazione ")
    '        stb.AppendLine("      And Stalla_Raggruppamenti.sa_cod = Mov_Destinazioni.Sa_Cod ")
    '        stb.AppendLine("      And Stalla_Raggruppamenti.piva = Imprese.Piva ")
    '        stb.AppendLine("        ) ")
    '        stb.AppendLine("  End As STA_DES, ")
    '        stb.AppendLine("   Case  ")
    '        stb.AppendLine("  WHEN Mov_Destinazioni.Tipo_Destinazione = 15 ")
    '        stb.AppendLine("  THEN (SELECT Fabbricato_Cod FROM Fabbricati WHERE Fabbricati.piva = Imprese.Piva And Fabbricati.SA_COD=Mov_Destinazioni.Sa_Cod And Fabbricati.Fabbricato_Cod=Mov_Destinazioni.Id_Destinazione) ")
    '        stb.AppendLine("  Else ( ")
    '        stb.AppendLine("        Select Fabbricato_Cod FROM Stalla_Raggruppamenti ")
    '        stb.AppendLine("           INNER Join Fabbricati ON Fabbricati.Fabbricato_Cod = Stalla_Raggruppamenti.STA_NUM And Fabbricati.piva = Stalla_Raggruppamenti.piva And Fabbricati.SA_COD= Stalla_Raggruppamenti.sa_cod ")
    '        stb.AppendLine("           WHERE Stalla_Raggruppamenti.Raggruppamento_Cod = Mov_Destinazioni.Id_Destinazione ")
    '        stb.AppendLine("        And Stalla_Raggruppamenti.sa_cod = Mov_Destinazioni.Sa_Cod ")
    '        stb.AppendLine("        And Stalla_Raggruppamenti.piva = Imprese.Piva ")
    '        stb.AppendLine("        ) ")
    '        stb.AppendLine("  End As STA_NUM, ")
    '        stb.AppendLine("   Stalla_Raggruppamenti.Raggruppamento_Des, ")
    '        stb.AppendLine("   Stalla_Raggruppamenti.Raggruppamento_Cod, ")
    '        stb.AppendLine("   Zoo_Animali.Matricola, ")
    '        stb.AppendLine("   Zoo_Animali.GEN_COD, ")
    '        stb.AppendLine("   Zoo_Animali.SPE_COD, ")
    '        stb.AppendLine("   Zoo_Animali.TIPO_COD, ")
    '        stb.AppendLine("   Zoo_Animali.Progetto, ")
    '        stb.AppendLine("   Zoo_Animali.Validita_Inizio, ")
    '        stb.AppendLine("   Zoo_Animali.Validita_Fine, ")
    '        stb.AppendLine("   Zoo_Animali.Sesso, ")
    '        stb.AppendLine("   Zoo_Animali.Dat_Nascita, ")
    '        stb.AppendLine("   Convert(INTEGER, SUM( Case When Movimenti.CAU_MOV In ('7350','3750')             ")
    '        stb.AppendLine("          THEN -(Mov_Destinazioni.qta)            ")
    '        stb.AppendLine("          Else Mov_Destinazioni.qta            ")
    '        stb.AppendLine("          End)) As Giacenza  ")
    '        stb.AppendLine(" From Agenda ")
    '        stb.AppendLine(" INNER Join Imprese ON Imprese.Piva = Agenda.Piva  ")
    '        stb.AppendLine(" INNER Join Movimenti ON Agenda.Id_Agenda = Movimenti.Id_Agenda ")
    '        stb.AppendLine("                      AND Agenda.Piva = Movimenti.PIVA ")
    '        stb.AppendLine(" INNER Join Movimenti_Dettagli ON Movimenti_Dettagli.PIVA = Movimenti.piva  ")
    '        stb.AppendLine("                               AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
    '        stb.AppendLine("                               AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")
    '        stb.AppendLine(" INNER Join Mov_Destinazioni ON Mov_Destinazioni.piva = Movimenti_Dettagli.piva ")
    '        stb.AppendLine("                             AND Mov_Destinazioni.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
    '        stb.AppendLine("                             AND Mov_Destinazioni.Id_Mov = Movimenti_Dettagli.Id_Mov ")
    '        stb.AppendLine("                             AND Mov_Destinazioni.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
    '        stb.AppendLine(" INNER Join Zoo_Animali ON Zoo_Animali.piva = Movimenti_dettagli.piva ")
    '        stb.AppendLine("                        AND Zoo_Animali.Cod_Progetto = Movimenti_dettagli.Cod_Progetto ")
    '        stb.AppendLine(" Left Join Stalla_Raggruppamenti ON Mov_Destinazioni.Sa_Cod = Stalla_Raggruppamenti.sa_cod ")
    '        stb.AppendLine("                                AND Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
    '        stb.AppendLine("                                AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
    '        stb.AppendLine(" INNER Join UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod  ")
    '        stb.AppendLine(" LEFT JOIN Centri_Aziendali ON Mov_Destinazioni.Piva = Centri_Aziendali.Piva and Mov_Destinazioni.Sa_Cod=Centri_Aziendali.sa_cod ")
    '        stb.AppendLine(" LEFT JOIN Zoo_Animali_Distinte ON Zoo_Animali.PIVA = Zoo_Animali_Distinte.PIVA AND Zoo_animali.Cod_Progetto = Zoo_Animali_Distinte.Cod_Animale And Zoo_Animali_Distinte.Validita_Inizio <=  " & Agro_SQL_SaveDateTime(Data) & "  And Zoo_Animali_Distinte.Validita_Fine >=  " & Agro_SQL_SaveDateTime(Data) & "     ")
    '        stb.AppendLine(" LEFT JOIN Zoo_AnimalixStati_Accrescimento ON Zoo_Animali.PIVA = Zoo_AnimalixStati_Accrescimento.PIVA AND Zoo_animali.Cod_Progetto = Zoo_AnimalixStati_Accrescimento.Cod_Progetto And Zoo_AnimalixStati_Accrescimento.Validita_Inizio <=  " & Agro_SQL_SaveDateTime(Data) & "  And Zoo_AnimalixStati_Accrescimento.Validita_Fine >=  " & Agro_SQL_SaveDateTime(Data) & "     ")
    '        stb.AppendLine(" LEFT JOIN Zoo_Animali_Lista_Stati_Accrescimento ON Zoo_AnimalixStati_Accrescimento.GEN_COD = Zoo_Animali_Lista_Stati_Accrescimento.GEN_COD AND Zoo_AnimalixStati_Accrescimento.SPE_COD = Zoo_Animali_Lista_Stati_Accrescimento.SPE_COD AND Zoo_AnimalixStati_Accrescimento.STATO_COD = Zoo_Animali_Lista_Stati_Accrescimento.STATO_COD AND Zoo_AnimalixStati_Accrescimento.TIPO_COD = Zoo_Animali_Lista_Stati_Accrescimento.TIPO_COD ")

    '        stb.AppendLine(" INNER JOIN Lista_Specie_Animali ON Zoo_Animali.GEN_COD = Lista_Specie_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Specie_Animali.SPE_COD ")
    '        stb.AppendLine(" INNER JOIN Lista_Razze_Animali ON Zoo_Animali.GEN_COD = Lista_Razze_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_Razze_Animali.SPE_COD And Zoo_Animali.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")
    '        stb.AppendLine(" INNER JOIN Zoo_Animali_Lista_Tipi ON Zoo_Animali.GEN_COD = Zoo_Animali_Lista_Tipi.GEN_COD And Zoo_Animali.SPE_COD = Zoo_Animali_Lista_Tipi.SPE_COD And Zoo_Animali.TIPO_COD = Zoo_Animali_Lista_Tipi.TIPO_COD ")
    '        stb.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali ON Zoo_Animali.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Zoo_Animali.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Zoo_Animali.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD ")
    '        stb.AppendLine(" LEFT  JOIN Contatti ON Zoo_Animali.CF_Fornitore = Contatti.Cod_Contatto")


    '        stb.AppendLine(" WHERE Agenda.lav_Cod > 0 ")
    '        stb.AppendLine(" And Movimenti_Dettagli.Elem_Cod = 300 ")
    '        stb.AppendLine(" And Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDateTime(Data) & "   ")
    '        stb.AppendLine(" And Movimenti.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)    ")
    '        stb.AppendLine(" And Movimenti.Cau_Mov In ('7300', '7350', '3700', '3750')    ")
    '        stb.AppendLine(" And Movimenti_Dettagli.Jolly_Int = 0 ")
    '        stb.AppendLine(" And Mov_Destinazioni.Tipo_Destinazione IN (15, 21) ")
    '        stb.AppendLine(" And Movimenti_Dettagli.Piva = '01704430519'      ")
    '        stb.AppendLine(" And Zoo_Animali.Validita_Inizio <= " & Agro_SQL_SaveDateTime(Data) & "   ")
    '        stb.AppendLine(" And Zoo_Animali.Validita_Fine >= " & Agro_SQL_SaveDateTime(Data) & "   ")
    '        stb.AppendLine(" GROUP BY Imprese.Piva, Imprese.Rag_Soc, ")
    '        stb.AppendLine(" Movimenti_dettagli.Cod_Progetto, ")
    '        stb.AppendLine(" Movimenti_dettagli.Lotto, ")
    '        stb.AppendLine(" Movimenti_dettagli.Udm_Cod, ")
    '        stb.AppendLine(" Mov_Destinazioni.Sa_Cod, ")
    '        stb.AppendLine(" Centri_Aziendali.sa_nome, ")
    '        stb.AppendLine(" Mov_Destinazioni.Id_Destinazione, ")
    '        stb.AppendLine(" Mov_Destinazioni.Tipo_Destinazione, ")
    '        stb.AppendLine(" UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim, ")
    '        stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Des, ")
    '        stb.AppendLine(" Stalla_Raggruppamenti.Raggruppamento_Cod, ")
    '        stb.AppendLine(" Zoo_Animali.Matricola, ")
    '        stb.AppendLine(" Zoo_Animali.GEN_COD, ")
    '        stb.AppendLine(" Zoo_Animali.SPE_COD, ")
    '        stb.AppendLine(" Zoo_Animali.TIPO_COD, ")
    '        stb.AppendLine(" Zoo_Animali.Progetto, ")
    '        stb.AppendLine(" Zoo_Animali.Validita_Inizio, ")
    '        stb.AppendLine(" Zoo_Animali.Validita_Fine, ")
    '        stb.AppendLine(" Zoo_Animali.Sesso, ")
    '        stb.AppendLine(" Zoo_Animali.Dat_Nascita, ")

    '        stb.AppendLine(" Lista_Specie_Animali.SPE_DES, ")
    '        stb.AppendLine(" Lista_Razze_Animali.RAZ_DES, ")
    '        stb.AppendLine(" Zoo_Animali_Lista_Tipi.Tipo_Des, ")
    '        stb.AppendLine(" Zoo_Animali.Mat_Madre, ")
    '        stb.AppendLine(" Zoo_Animali.Mat_Padre, ")
    '        stb.AppendLine(" Zoo_Animali.Metodo_Produzione, ")
    '        stb.AppendLine(" Lista_IndirizziProd_Animali.IPRO_DES, ")
    '        stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Nome, ")
    '        stb.AppendLine(" Zoo_Animali_Distinte.Codice_Distinta, ")
    '        stb.AppendLine(" Zoo_Animali.Lotto_Fornitore, ")
    '        stb.AppendLine(" Contatti.Rag_Soc, ")
    '        stb.AppendLine(" Contatti.Cod_Contatto,")

    '        stb.AppendLine(" Zoo_Animali_Distinte.Cod_Progetto, ")
    '        stb.AppendLine(" Zoo_Animali_Distinte.Progetto_Des, ")
    '        stb.AppendLine(" Zoo_AnimalixStati_Accrescimento.Stato_Cod, ")
    '        stb.AppendLine(" Zoo_Animali_Lista_Stati_Accrescimento.Stato_Des ")
    '        stb.AppendLine(" HAVING Convert(INTEGER, SUM(Case When Movimenti.Cau_Mov In ('7350','3750') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END)) <> 0  ")
    '        stb.AppendLine(" ) a ")
    '        stb.AppendLine(" WHERE 1=1 ")

    '        If Piva <> "" Then
    '            stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            stb.AppendLine(" And a.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
    '        End If

    '        If STA_NUM <> 0 Then
    '            stb.AppendLine(" AND a.Sta_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
    '        End If

    '        If Raggruppamento_Cod <> 0 Then
    '            stb.AppendLine(" AND a.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "  ")
    '        End If

    '        stb.AppendLine("  ORDER BY Impresa")

    '        '--------------------------------------------------------------------------
    '        dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return dt

    'End Function

    Public Function Elimina_Operazione(_Piva As String, ID_Agenda As Integer, objParametriServer As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Using scope As New TransactionScope()

            Try

                Dim Agenda = (From a In GiasContext.Agenda Where a.PIVA = _Piva AndAlso a.Id_Agenda = ID_Agenda).First

                If Agenda IsNot Nothing Then

                    Select Case Agenda.Lav_Cod
                        Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_NASCITA_ANIMALI

                            If (AnimaliMovimentati_Next(_Piva, ID_Agenda, GiasContext, objParametriServer)) Then
                                r.RispostaOK = True
                                r.RispostaConferma = False
                                r.RispostaStringa = "Animali movimentati dopo l'operazione di incremento consistenze, bisogna prima eliminare tutte le movimentazioni"
                                Return r
                            End If

                            Dim Movimenti_Animali = (From Movimenti In GiasContext.Movimenti
                                                     Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                                                                                                                  Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                  Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                     Where (Movimenti.Cau_Mov = CAU_CARICO_CONSISTENZE OrElse Movimenti.Cau_Mov = CAU_ANIMALE) AndAlso
                                                         Movimenti.PIVA = _Piva AndAlso
                                                         Movimenti.Id_Agenda = ID_Agenda AndAlso
                                                         Movimenti_Dettagli.Cod_Progetto <> 0
                                                     Select Movimenti_Dettagli.PIVA, Movimenti_Dettagli.Cod_Progetto).Distinct

                            For Each mov_an In Movimenti_Animali
                                Elimina_Animale(mov_an.PIVA, mov_an.Cod_Progetto, objParametriServer)
                            Next

                            Dim obj_AgendaBIZ As New AgronicaCoreContabBIZ.Agenda_W
                            obj_AgendaBIZ.Elimina_InteraOperazione(Agenda.PIVA, Agenda.Id_Agenda, objParametriServer, GiasContext)

                        Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_MORTE_ANIMALI



                        Case LAVCOD_SPOSTAMENTI_ZOO

                            If (AnimaliMovimentati_Next(_Piva, ID_Agenda, GiasContext, objParametriServer)) Then
                                r.RispostaOK = True
                                r.RispostaConferma = False
                                r.RispostaStringa = "Animali movimentati dopo l'operazione di spostamento, bisogna prima eliminare tutte le movimentazioni"
                                Return r
                            End If

                            Dim Movimenti_Animali = (From Movimenti In GiasContext.Movimenti
                                                     Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On Movimenti.PIVA Equals Movimenti_Dettagli.PIVA And
                                                                                                                  Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda And
                                                                                                                  Movimenti.Id_Mov Equals Movimenti_Dettagli.Id_Mov
                                                     Where (Movimenti.Cau_Mov = CAU_SCARICO_CONSISTENZE OrElse Movimenti.Cau_Mov = CAU_ANIMALE) AndAlso
                                                         Movimenti.PIVA = _Piva AndAlso
                                                         Movimenti.Id_Agenda = ID_Agenda AndAlso
                                                         Movimenti_Dettagli.Cod_Progetto <> 0
                                                     Select Movimenti_Dettagli.PIVA, Movimenti_Dettagli.Cod_Progetto).Distinct

                            'For Each mov_an In Movimenti_Animali
                            '    Elimina_Animale(mov_an.PIVA, mov_an.Cod_Progetto, objParametriServer)
                            'Next

                            Dim obj_AgendaBIZ As New AgronicaCoreContabBIZ.Agenda_W
                            obj_AgendaBIZ.Elimina_InteraOperazione(Agenda.PIVA, Agenda.Id_Agenda, objParametriServer, GiasContext)

                        Case LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI

                            Dim obj_AgendaBIZ As New AgronicaCoreContabBIZ.Agenda_W
                            If Agenda.Raccoglitore_Cod IsNot Nothing AndAlso Agenda.Raccoglitore_Cod <> 0 Then

                                Dim agendas = (From a In GiasContext.Agenda Where a.Raccoglitore_Cod = Agenda.Raccoglitore_Cod).ToList
                                For Each Agenda_del In agendas
                                    obj_AgendaBIZ.Elimina_InteraOperazione(Agenda_del.PIVA, Agenda_del.Id_Agenda, objParametriServer, GiasContext)
                                Next

                            Else
                                obj_AgendaBIZ.Elimina_InteraOperazione(Agenda.PIVA, Agenda.Id_Agenda, objParametriServer, GiasContext)
                            End If

                    End Select

                End If

                r.RispostaStringa = "Operazione Eliminata correttamente"
                r.RispostaConferma = True
                r.RispostaOK = True

                scope.Complete()

            Catch ex As Exception

                r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                r.RispostaOK = False
                scope.Dispose()
                Throw New Exception(r.Errore)

            Finally

                'scope.Dispose()
                GiasContext.Dispose()

            End Try

        End Using

        Return r

    End Function

    Public Function AnimaliMovimentati_Next(Piva As String, ID_Agenda As Integer, GiasContext As Gias_DeveloperServer_Entities, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim animaliMovimentati As Boolean = False

        Dim obj_Zoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtMovimenti_Animali As DataTable = obj_Zoo.Movimenti_AnimalixAgenda(Piva, ID_Agenda, objParametri_Server)
        'Dim Movimenti_Animali = (
        '    From a In GiasContext.Movimenti_dettagli
        '    Join b In GiasContext.Movimenti On a.PIVA Equals b.PIVA And a.Id_Agenda Equals b.Id_Agenda And a.Id_Mov Equals b.Id_Mov
        '    Where a.PIVA = Piva AndAlso
        '          a.Id_Agenda = ID_Agenda AndAlso
        '          a.Cod_Progetto <> 0 AndAlso
        '          a.Elem_Cod = 300
        '    Select a, b.Data_Movimento)

        For Each Mov_animale In dtMovimenti_Animali.Rows

            If Animale_Movimentato(Mov_animale("PIVA"), Mov_animale("Cod_Progetto"), Mov_animale("Data_Movimento"), objParametri_Server) Then
                animaliMovimentati = True
                Exit For
            End If

        Next

        Return animaliMovimentati

    End Function

    Public Function AnimaliMovimentati_Next_List(Piva As String,
                                                 ID_Agenda As Integer,
                                                 list_IdAgenda_DaCancellare As List(Of Integer),
                                                 objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of String)

        Dim listAnimaliMovimentati As New List(Of String)
        'Dim gefutils As New Gias_EF_Utility
        'Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Try
            Dim obj_Zoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            Dim dtMovimenti_Animali As DataTable = obj_Zoo.Movimenti_AnimalixAgenda(Piva, ID_Agenda, objParametri_Server)
            'Dim Movimenti_Animali = (
            '    From a In GiasContext.Movimenti_dettagli
            '    Join b In GiasContext.Movimenti On a.PIVA Equals b.PIVA And a.Id_Agenda Equals b.Id_Agenda And a.Id_Mov Equals b.Id_Mov
            '    Where a.PIVA = Piva AndAlso
            '          a.Id_Agenda = ID_Agenda AndAlso
            '          a.Cod_Progetto <> 0 AndAlso
            '          a.Elem_Cod = 300
            '    Select a, b.Data_Movimento)

            If dtMovimenti_Animali.Rows.Count > 0 Then
                Dim ZooDAL As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                Dim listCodProgetti As List(Of Integer) = (From r In dtMovimenti_Animali.Rows Select CInt(r("Cod_Progetto"))).ToList()
                listAnimaliMovimentati = ZooDAL.Animali_Movimentati(dtMovimenti_Animali(0)("Piva"), listCodProgetti, dtMovimenti_Animali(0)("Data_Movimento"), list_IdAgenda_DaCancellare, objParametri_Server)
            End If

        Catch ex As Exception
            Throw ex
        Finally

            'GiasContext.Dispose()

        End Try

        Return listAnimaliMovimentati

    End Function


    Public Function Elimina_Animale(ByVal Piva As String,
                                    ByVal Cod_Progetto As Integer,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                    Optional OpenNewTransaction As Boolean = True) As RispostaStandard

        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If


        Try

            Dim Zoo_Animali_Distinte = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = Piva AndAlso a.Cod_Animale = Cod_Progetto).ToArray
            Dim Zoo_AnimalixStati_Accrescimento = (From a In GiasContext.Zoo_AnimalixStati_Accrescimento Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).ToArray
            Dim zooAnimale = (From a In GiasContext.Zoo_Animali Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).First

            If zooAnimale IsNot Nothing AndAlso zooAnimale.Cod_Progetto <> 0 Then

                Dim obj_Zoo_Animali_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                Dim obj_Zoo_Animali_Distinte_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte
                Dim obj_Zoo_AnimalixStati_Accrescimento_W As New AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento

                For Each distinta In Zoo_Animali_Distinte
                    obj_Zoo_Animali_Distinte_W.Elimina(distinta, GiasContext, objParametriServer)
                Next

                For Each accrescimento In Zoo_AnimalixStati_Accrescimento
                    obj_Zoo_AnimalixStati_Accrescimento_W.Elimina(accrescimento, GiasContext, objParametriServer)
                Next

                obj_Zoo_Animali_W.Elimina(zooAnimale, GiasContext, objParametriServer)

                GiasContext.SaveChanges()

            End If

            r.RispostaStringa = ""
            r.RispostaOK = True

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            Throw New Exception(r.Errore)

        Finally

            If bCloseContext Then
                GiasContext.Dispose()
            End If

        End Try


        Return r

    End Function

    Public Function Elimina_Animale_Con_Operazione_Carico(ByVal Piva As String,
                                    ByVal Cod_Progetto As Integer,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                    Optional OpenNewTransaction As Boolean = True) As RispostaStandard

        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If


        Try

            Dim Zoo_Animali_Distinte = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = Piva AndAlso a.Cod_Animale = Cod_Progetto).ToArray
            Dim Zoo_AnimalixStati_Accrescimento = (From a In GiasContext.Zoo_AnimalixStati_Accrescimento Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).ToArray
            Dim zooAnimale = (From a In GiasContext.Zoo_Animali Where a.PIVA = Piva AndAlso a.Cod_Progetto = Cod_Progetto).First

            If zooAnimale IsNot Nothing AndAlso zooAnimale.Cod_Progetto <> 0 Then

                Dim obj_Zoo_Animali_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                Dim obj_Zoo_Animali_Distinte_W As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte
                Dim obj_Zoo_AnimalixStati_Accrescimento_W As New AgronicaCoreAnagrafeDAL.Zoo_AnimalixStati_Accrescimento

                For Each distinta In Zoo_Animali_Distinte
                    obj_Zoo_Animali_Distinte_W.Elimina(distinta, GiasContext, objParametriServer)
                Next

                For Each accrescimento In Zoo_AnimalixStati_Accrescimento
                    obj_Zoo_AnimalixStati_Accrescimento_W.Elimina(accrescimento, GiasContext, objParametriServer)
                Next

                obj_Zoo_Animali_W.Elimina(zooAnimale, GiasContext, objParametriServer)

            End If

            Dim movimenti_dettagliAnimale = (From m In GiasContext.Movimenti
                                             Join a In GiasContext.Agenda On m.Id_Agenda Equals a.Id_Agenda
                                             Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And
                                              m.Id_Mov Equals md.Id_Mov
                                             Where md.Elem_Cod = 300 And md.Cod_Progetto = Cod_Progetto And m.Cau_Mov = CAU_CARICO_CONSISTENZE Order By m.Data_Movimento Ascending).FirstOrDefault

            Dim lav_Cod_Carico As New List(Of Integer) From {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI}

            'If movimenti_dettagliAnimale IsNot Nothing AndAlso lav_Cod_Scarico.Contains(movimenti_dettagliAnimale.a.Lav_Cod) Then

            If movimenti_dettagliAnimale IsNot Nothing AndAlso lav_Cod_Carico.Contains(movimenti_dettagliAnimale.a.Lav_Cod) Then
                Dim id_agenda As Integer = movimenti_dettagliAnimale.m.Id_Agenda
                Dim movimenti_dettagli = (From m In GiasContext.Movimenti
                                          Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And
                                           m.Id_Mov Equals md.Id_Mov
                                          Where m.Cau_Mov = CAU_CARICO_CONSISTENZE And m.Id_Agenda = id_agenda).ToList



                If movimenti_dettagli.Count = 1 Then
                    'Elimino Intera Operazione
                    Dim agenda = (From a In GiasContext.Agenda Where a.Id_Agenda = id_agenda).FirstOrDefault
                    If agenda IsNot Nothing Then
                        GiasContext.Agenda.Remove(agenda)

                        Dim movimenti = (From a In GiasContext.Movimenti Where a.Id_Agenda = id_agenda).ToList
                        If movimenti IsNot Nothing Then
                            GiasContext.Movimenti.RemoveRange(movimenti)

                            Dim movimentid = (From a In GiasContext.Movimenti_dettagli Where a.Id_Agenda = id_agenda).ToList
                            If movimentid IsNot Nothing Then
                                GiasContext.Movimenti_dettagli.RemoveRange(movimentid)

                                Dim mov_dest = (From a In GiasContext.Mov_Destinazioni Where a.Id_Agenda = id_agenda).ToList
                                If mov_dest IsNot Nothing Then
                                    GiasContext.Mov_Destinazioni.RemoveRange(mov_dest)
                                End If

                            End If

                        End If

                    End If
                Else
                    'Elimino solo Movimenti e Mov_Destinazioni
                    Dim mov_det_capoAnimale As Movimenti_dettagli = (From m In movimenti_dettagli Where m.md.Cod_Progetto = Cod_Progetto And m.md.Elem_Cod = 300 Select m.md).FirstOrDefault
                    If mov_det_capoAnimale IsNot Nothing Then
                        Dim Id_mov As Integer = mov_det_capoAnimale.Id_Mov
                        Dim Id_Mov_det As Integer = mov_det_capoAnimale.Id_Mov_Det

                        Dim movimentid = (From a In GiasContext.Movimenti_dettagli
                                          Where a.Id_Agenda = id_agenda And
                                              a.Id_Mov = Id_mov And
                                              a.Id_Mov_Det = Id_Mov_det).ToList

                        If movimentid IsNot Nothing Then
                            GiasContext.Movimenti_dettagli.RemoveRange(movimentid)

                            Dim mov_dest = (From a In GiasContext.Mov_Destinazioni
                                            Where a.Id_Agenda = id_agenda And
                                                a.Id_Mov = Id_mov And
                                                a.Id_Mov_Det = Id_Mov_det).ToList

                            If mov_dest IsNot Nothing Then
                                GiasContext.Mov_Destinazioni.RemoveRange(mov_dest)
                            End If

                        End If
                    End If


                End If

            End If

            GiasContext.SaveChanges()

            r.RispostaStringa = ""
            r.RispostaOK = True

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            Throw New Exception(r.Errore)

        Finally

            If bCloseContext Then
                GiasContext.Dispose()
            End If

        End Try


        Return r

    End Function

    Public Function Riapri_Animale_Con_Del_Operazione_Scarico(ByVal Piva As String,
                                    ByVal Cod_Progetto As Integer,
                                    ByRef objParametriServer As AgronicaCoreParametri,
                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                    Optional OpenNewTransaction As Boolean = True) As RispostaStandard

        Dim r As New RispostaStandard

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If


        Try

            Riapri_Validita_Animale_Fine(Piva, Cod_Progetto, GiasContext, objParametriServer)
            Aggiorna_Causale_Morte(Piva, Cod_Progetto, 0, GiasContext, objParametriServer)
            Aggiorna_Patologia_Capo(Piva, Cod_Progetto, 0, GiasContext, objParametriServer)
            Aggiorna_Note_Capo(Piva, Cod_Progetto, "", GiasContext, objParametriServer)
            'Aggiorna_Causale_Morte(Piva, Cod_Progetto, 0, GiasContext, objParametriServer)

            Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault
            If Animale IsNot Nothing Then
                Animale.Causale_Morte = 0
                Animale.Codice_Azienda_Uscita = ""
                Animale.Modello4_Uscita = ""
                Animale.Modello4_Uscita_Numero = ""
                Animale.Modello4_Uscita_Prenotazione = ""
                Animale.Data_DDT_UScita = AGRODATAFINE
                Animale.Data_Documento_Uscita = AGRODATAFINE
                Animale.N_Bolla_Uscita = ""
                GiasContext.Entry(Animale).State = EntityState.Modified
            End If

            Dim movimenti_dettagliAnimale = (From m In GiasContext.Movimenti
                                             Join a In GiasContext.Agenda On m.Id_Agenda Equals a.Id_Agenda
                                             Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And
                                              m.Id_Mov Equals md.Id_Mov
                                             Where md.Elem_Cod = 300 And md.Cod_Progetto = Cod_Progetto And m.Cau_Mov = CAU_SCARICO_CONSISTENZE Order By m.Data_Movimento Descending).FirstOrDefault

            Dim lav_Cod_Scarico As New List(Of Integer) From {LAVCOD_MORTE_ANIMALI, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_VENDITA_ANIMALI}

            If movimenti_dettagliAnimale IsNot Nothing AndAlso lav_Cod_Scarico.Contains(movimenti_dettagliAnimale.a.Lav_Cod) Then
                Dim id_agenda As Integer = movimenti_dettagliAnimale.m.Id_Agenda
                Dim movimenti_dettagli = (From m In GiasContext.Movimenti
                                          Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And
                                           m.Id_Mov Equals md.Id_Mov
                                          Where m.Cau_Mov = CAU_SCARICO_CONSISTENZE And m.Id_Agenda = id_agenda).ToList



                If movimenti_dettagli.Count = 1 Then
                    'Elimino Intera Operazione
                    Dim agenda = (From a In GiasContext.Agenda Where a.Id_Agenda = id_agenda).FirstOrDefault
                    If agenda IsNot Nothing Then
                        GiasContext.Agenda.Remove(agenda)

                        Dim movimenti = (From a In GiasContext.Movimenti Where a.Id_Agenda = id_agenda).ToList
                        If movimenti IsNot Nothing Then
                            GiasContext.Movimenti.RemoveRange(movimenti)

                            Dim movimentid = (From a In GiasContext.Movimenti_dettagli Where a.Id_Agenda = id_agenda).ToList
                            If movimentid IsNot Nothing Then
                                GiasContext.Movimenti_dettagli.RemoveRange(movimentid)

                                Dim mov_dest = (From a In GiasContext.Mov_Destinazioni Where a.Id_Agenda = id_agenda).ToList
                                If mov_dest IsNot Nothing Then
                                    GiasContext.Mov_Destinazioni.RemoveRange(mov_dest)
                                End If

                            End If

                        End If

                    End If
                Else
                    'Elimino solo Movimenti e Mov_Destinazioni
                    Dim movimentoSel = (From m In movimenti_dettagli Where m.md.Cod_Progetto = Cod_Progetto).FirstOrDefault
                    Dim Id_mov As Integer = movimentoSel.md.Id_Mov
                    Dim Id_Mov_det As Integer = movimentoSel.md.Id_Mov_Det

                    Dim movimentid = (From a In GiasContext.Movimenti_dettagli
                                      Where a.Id_Agenda = id_agenda And
                                          a.Id_Mov = Id_mov And
                                          a.Id_Mov_Det = Id_Mov_det).ToList

                    If movimentid IsNot Nothing Then
                        GiasContext.Movimenti_dettagli.RemoveRange(movimentid)

                        Dim mov_dest = (From a In GiasContext.Mov_Destinazioni
                                        Where a.Id_Agenda = id_agenda And
                                            a.Id_Mov = Id_mov And
                                            a.Id_Mov_Det = Id_Mov_det).ToList

                        If mov_dest IsNot Nothing Then
                            GiasContext.Mov_Destinazioni.RemoveRange(mov_dest)
                        End If

                    End If

                End If

            End If

            GiasContext.SaveChanges()

            r.RispostaStringa = ""
            r.RispostaOK = True

            If OpenNewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If

        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            Throw New Exception(r.Errore)

        Finally

            If bCloseContext Then
                GiasContext.Dispose()
            End If

        End Try


        Return r

    End Function

    Public Sub Aggiorna_Validita_Animale(Piva As String,
                                         Cod_Progetto As Integer,
                                         Data_Fine As Date,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Validita_Fine = Data_Fine

        Dim Animale_Distinte = (From Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte Where Zoo_Animali_Distinte.PIVA = Piva AndAlso Zoo_Animali_Distinte.Cod_Animale = Cod_Progetto).ToArray

        For Each distinta In Animale_Distinte
            If distinta.Validita_Inizio <= Data_Fine AndAlso distinta.Validita_Fine >= Data_Fine Then
                distinta.Validita_Fine = Data_Fine
            End If
        Next

        Dim Animale_Stati_Accrescimento = (From Zoo_AnimalixStati_Accrescimento In GiasContext.Zoo_AnimalixStati_Accrescimento Where Zoo_AnimalixStati_Accrescimento.PIVA = Piva AndAlso Zoo_AnimalixStati_Accrescimento.Cod_Progetto = Cod_Progetto).ToArray

        obj_Animale.Scrivi_Modifica_Zoo_Animali(Animale, Animale_Distinte, Animale_Stati_Accrescimento, objParametri_Server, GiasContext, False, Nothing, False, True, "Aggiorna_Validita_Animale")

        GiasContext.SaveChanges()

    End Sub

    Public Sub Aggiorna_Causale_Morte(Piva As String,
                                         Cod_Progetto As Integer,
                                         CausaleMorte As Integer,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Causale_Morte = CausaleMorte


        If Animale.Cod_Progetto = 0 Then

            Cod_Progetto = agroDP.NuovoId_Tabella_EF(GiasContext, "Zoo_Animali", 0, 200000000, objParametriServer)

            Animale.Cod_Progetto = Cod_Progetto
            obj_zoo_animali.Scrivi(Animale, GiasContext, objParametriServer)

        Else
            Cod_Progetto = Animale.Cod_Progetto
            obj_zoo_animali.Modifica(Animale, GiasContext, objParametriServer)

        End If

        'GiasContext.SaveChanges()

    End Sub

    Public Sub Aggiorna_Causale_Morte_Massiva(Zoo_AnimaliList As List(Of Zoo_Animali),
                                              Piva As String,
                                              Cod_Progetto As Integer,
                                              CausaleMorte As Integer,
                                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In Zoo_AnimaliList Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Causale_Morte = CausaleMorte
        GiasContext.Entry(Animale).State = EntityState.Modified

    End Sub

    Public Sub Aggiorna_Patologia_Capo(ByVal Piva As String,
                                       ByVal Cod_Progetto As Integer,
                                       ByVal IdPatologia As Integer,
                                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Id_Patologia = IdPatologia

        If Animale.Cod_Progetto = 0 Then
            Cod_Progetto = agroDP.NuovoId_Tabella_EF(GiasContext, "Zoo_Animali", 0, 200000000, objParametriServer)
            Animale.Cod_Progetto = Cod_Progetto
            obj_zoo_animali.Scrivi(Animale, GiasContext, objParametriServer)
        Else
            Cod_Progetto = Animale.Cod_Progetto
            obj_zoo_animali.Modifica(Animale, GiasContext, objParametriServer)
        End If

        'GiasContext.SaveChanges()

    End Sub

    Public Sub Aggiorna_Patologia_Capo_Massiva(Zoo_AnimaliList As List(Of Zoo_Animali),
                                               ByVal Piva As String,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal IdPatologia As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In Zoo_AnimaliList Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Id_Patologia = IdPatologia
        GiasContext.Entry(Animale).State = EntityState.Modified

    End Sub

    Public Sub Aggiorna_Note_Capo(ByVal Piva As String,
                                  ByVal Cod_Progetto As Integer,
                                  ByVal Note As String,
                                  ByRef GiasContext As Gias_DeveloperServer_Entities,
                                  ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Note = Note

        If Animale.Cod_Progetto = 0 Then
            Cod_Progetto = agroDP.NuovoId_Tabella_EF(GiasContext, "Zoo_Animali", 0, 200000000, objParametriServer)
            Animale.Cod_Progetto = Cod_Progetto
            obj_zoo_animali.Scrivi(Animale, GiasContext, objParametriServer)
        Else
            Cod_Progetto = Animale.Cod_Progetto
            obj_zoo_animali.Modifica(Animale, GiasContext, objParametriServer)
        End If

        'GiasContext.SaveChanges()

    End Sub

    Public Sub Aggiorna_Note_Capo_Massiva(Zoo_AnimaliList As List(Of Zoo_Animali),
                                          ByVal Piva As String,
                                          ByVal Cod_Progetto As Integer,
                                          ByVal Note As String,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities,
                                          ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim obj_zoo_animali As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale = (From Zoo_Animali In Zoo_AnimaliList Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Note = Note
        GiasContext.Entry(Animale).State = EntityState.Modified
    End Sub

    Public Function Aggiorna_CapoAnimale(ByVal Capo As CapoAnimale,
                                         ByRef GiasContext As Gias_DeveloperServer_Entities,
                                         ByRef objParametriServer As AgronicaCoreParametri,
                                         Optional ByVal Cod_Gruppo As Integer = 0, Optional ByVal SaveChange As Boolean = True) As Boolean

        Dim Animale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Capo.partitaIva AndAlso Zoo_Animali.Cod_Progetto = Capo.codice).FirstOrDefault

        If Animale Is Nothing Then
            Return False
        End If

        If Cod_Gruppo <> 0 Then

            Dim Gruppo = (From Raggruppamento In GiasContext.Stalla_Raggruppamenti Where Raggruppamento.PIVA = Capo.partitaIva AndAlso Raggruppamento.Raggruppamento_Cod = Cod_Gruppo).FirstOrDefault

            ' imposto flag validato solo se box origine = ricevimento capi
            If Gruppo IsNot Nothing AndAlso Gruppo.Flag_BDN = 1 Then
                Animale.Validato = 1
            End If

        End If

        If Capo.anomalieNote IsNot Nothing Then
            Animale.Anomalie_Note = Capo.anomalieNote
        End If

        If Capo.anomalie IsNot Nothing Then

            Dim anomalie = (From x In GiasContext.Zoo_AnimalixAnomalie Where x.Piva = Animale.PIVA And x.Sa_Cod = Animale.sa_cod And x.Cod_Animale = Animale.Cod_Progetto).ToList

            For Each anomalia In anomalie
                GiasContext.Zoo_AnimalixAnomalie.Remove(anomalia)
            Next

            For Each anomalia In Capo.anomalie

                Dim Zoo_AnimalixAnomalie As New Zoo_AnimalixAnomalie

                Zoo_AnimalixAnomalie.Piva = Animale.PIVA
                Zoo_AnimalixAnomalie.Sa_Cod = Animale.sa_cod
                Zoo_AnimalixAnomalie.Cod_Animale = Animale.Cod_Progetto
                Zoo_AnimalixAnomalie.Codice = anomalia.codice

                Zoo_AnimalixAnomalie.inviato = 0
                Zoo_AnimalixAnomalie.datainvio = Date.Now
                Zoo_AnimalixAnomalie.Data_Creazione = Date.Now
                Zoo_AnimalixAnomalie.Data_Modifica = Date.Now
                Zoo_AnimalixAnomalie.Username_Creazione = objParametriServer.UsernameOperazione
                Zoo_AnimalixAnomalie.Username_Modifica = objParametriServer.UsernameOperazione
                Zoo_AnimalixAnomalie.Validita_Inizio = AGRODATAINIZIO
                Zoo_AnimalixAnomalie.Validita_Fine = AGRODATAFINE

                GiasContext.Zoo_AnimalixAnomalie.Add(Zoo_AnimalixAnomalie)

            Next

        End If

        If SaveChange Then
            GiasContext.SaveChanges()
        End If

        Return True

    End Function

    Public Sub Riapri_Validita_Animale_Fine(Piva As String,
                                            Cod_Progetto As Integer,
                                            ByRef GiasContext As Gias_DeveloperServer_Entities,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale As Zoo_Animali = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Validita_Fine = AGRODATAFINE
        Animale.Modello4_Uscita = ""
        Animale.Modello4_Uscita_Numero = ""
        Animale.Modello4_Uscita_Prenotazione = ""
        Animale.Codice_Azienda_Uscita = ""
        GiasContext.Entry(Animale).State = EntityState.Modified

        Dim Animale_Distinte As Zoo_Animali_Distinte() = (From Zoo_Animali_Distinte In GiasContext.Zoo_Animali_Distinte Where Zoo_Animali_Distinte.PIVA = Piva AndAlso Zoo_Animali_Distinte.Cod_Animale = Cod_Progetto).ToArray

        Dim maxDistinta = 0
        Dim maxData = AGRODATAINIZIO
        For Each distinta In Animale_Distinte
            If distinta.Validita_Fine > maxData Then
                maxData = distinta.Validita_Fine
                maxDistinta = distinta.Cod_Progetto
            End If
        Next

        For Each distinta In Animale_Distinte
            If distinta.Cod_Progetto = maxDistinta Then
                distinta.Validita_Fine = AGRODATAFINE
                GiasContext.Entry(distinta).State = EntityState.Modified
            End If
        Next

        Dim Animale_Stati_Accrescimento = (From Zoo_AnimalixStati_Accrescimento In GiasContext.Zoo_AnimalixStati_Accrescimento Where Zoo_AnimalixStati_Accrescimento.PIVA = Piva AndAlso Zoo_AnimalixStati_Accrescimento.Cod_Progetto = Cod_Progetto).ToArray

        obj_Animale.Scrivi_Modifica_Zoo_Animali(Animale, Animale_Distinte, Animale_Stati_Accrescimento, objParametri_Server, GiasContext, False, Nothing, False, True, "Riapri_Validita_Animale_Fine")

        'GiasContext.SaveChanges()

    End Sub

    Public Sub Riapri_Validita_Animale_Fine2(Animali_List As List(Of Zoo_Animali),
                                            Animale_Distinte_List As List(Of Zoo_Animali_Distinte),
                                            Piva As String,
                                            Cod_Progetto As Integer,
                                            ByRef GiasContext As Gias_DeveloperServer_Entities,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim obj_Animale As New AgronicaCoreAnagrafeBIZ.Zoo

        Dim Animale As Zoo_Animali = (From Zoo_Animali In Animali_List Where Zoo_Animali.PIVA = Piva AndAlso Zoo_Animali.Cod_Progetto = Cod_Progetto).FirstOrDefault

        Animale.Validita_Fine = AGRODATAFINE
        Animale.Modello4_Uscita = ""
        Animale.Modello4_Uscita_Numero = ""
        Animale.Modello4_Uscita_Prenotazione = ""
        Animale.Codice_Azienda_Uscita = ""
        GiasContext.Entry(Animale).State = EntityState.Modified

        Dim Animale_Distinte As Zoo_Animali_Distinte() = (From Zoo_Animali_Distinte In Animale_Distinte_List Where Zoo_Animali_Distinte.PIVA = Piva AndAlso Zoo_Animali_Distinte.Cod_Animale = Cod_Progetto).ToArray

        Dim maxDistinta = 0
        Dim maxData = AGRODATAINIZIO
        For Each distinta In Animale_Distinte
            If distinta.Validita_Fine > maxData Then
                maxData = distinta.Validita_Fine
                maxDistinta = distinta.Cod_Progetto
            End If
        Next

        For Each distinta In Animale_Distinte
            If distinta.Cod_Progetto = maxDistinta Then
                distinta.Validita_Fine = AGRODATAFINE
                GiasContext.Entry(distinta).State = EntityState.Modified
            End If
        Next
    End Sub

    Public Function Animale_Movimentato(Piva As String, Cod_Progetto As Integer, Data As DateTime, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Return objZoo.Animale_Movimentato(Piva, Cod_Progetto, Data, objParametri_Server)

        'Dim gefutils As New Gias_EF_Utility
        'Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        ''Dim scope As New TransactionScope()
        'Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        'Dim lav_cod_carico As New List(Of Integer) From {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI}

        'Dim Movimenti_Dettagli_List = (From Movimenti In GiasContext.Movimenti
        '                               Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On
        '                                                    Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda
        '                               Join Agenda In GiasContext.Agenda On Movimenti.Id_Agenda Equals Agenda.Id_Agenda
        '                               Where Movimenti.Data_Movimento > Data And
        '                                    Movimenti_Dettagli.Elem_Cod = 300 And
        '                                    Movimenti_Dettagli.PIVA = Piva And
        '                                    Not lav_cod_carico.Contains(Agenda.Lav_Cod) And
        '                                    Movimenti_Dettagli.Cod_Progetto = Cod_Progetto).ToList

        'If Movimenti_Dettagli_List.Count > 0 Then
        '    Return True
        'End If

        'Dim Mov_Destinazioni_List = (From Movimenti In GiasContext.Movimenti
        '                             Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On
        '                                                    Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda
        '                             Join Mov_Destinazioni In GiasContext.Mov_Destinazioni On
        '                                                    Movimenti_Dettagli.Id_Agenda Equals Mov_Destinazioni.Id_Agenda And
        '                                                    Movimenti_Dettagli.Id_Mov Equals Mov_Destinazioni.Id_Mov And
        '                                                    Movimenti_Dettagli.Id_Mov_Det Equals Mov_Destinazioni.Id_Mov_Det
        '                             Join Agenda In GiasContext.Agenda On Movimenti.Id_Agenda Equals Agenda.Id_Agenda
        '                             Where Movimenti.Data_Movimento > Data And
        '                                    Mov_Destinazioni.Tipo_Destinazione = TIPO_DESTINAZIONE_ANIMALE And
        '                                    Mov_Destinazioni.Id_Destinazione = Cod_Progetto And
        '                                    Not lav_cod_carico.Contains(Agenda.Lav_Cod) And
        '                                    Mov_Destinazioni.Piva = Piva).ToList

        'If Mov_Destinazioni_List.Count > 0 Then
        '    Return True
        'End If

        'Return False

    End Function

    Public Function Animali_Movimentati(Piva As String, Cod_Progetto_list As List(Of Integer), Data As DateTime, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of Integer)
        Dim list_AnimaliMovimentati As New List(Of Integer)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim Movimenti_Dettagli_List = (From Movimenti In GiasContext.Movimenti
                                       Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On
                                                            Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda
                                       Where Movimenti.Data_Movimento > Data And
                                            Movimenti_Dettagli.Elem_Cod = 300 And
                                            Movimenti_Dettagli.PIVA = Piva And
                                            Cod_Progetto_list.Contains(Movimenti_Dettagli.Cod_Progetto)).ToList

        If Movimenti_Dettagli_List.Count > 0 Then
            Dim movDetCodProgetti As List(Of Integer) = Movimenti_Dettagli_List.
                Select(Function(m) m.Movimenti_Dettagli.Cod_Progetto).
                Where(Function(codProg) codProg.HasValue).
                Select(Function(codProg) codProg.Value).
                Distinct.ToList

            '(From a In Movimenti_Dettagli_List Select a.Movimenti_Dettagli.Cod_Progetto).Distinct().ToList()

            If movDetCodProgetti.Count > 0 Then list_AnimaliMovimentati.AddRange(movDetCodProgetti)
        End If

        Dim Mov_Destinazioni_List = (From Movimenti In GiasContext.Movimenti
                                     Join Movimenti_Dettagli In GiasContext.Movimenti_dettagli On
                                                            Movimenti.Id_Agenda Equals Movimenti_Dettagli.Id_Agenda
                                     Join Mov_Destinazioni In GiasContext.Mov_Destinazioni On
                                                            Movimenti_Dettagli.Id_Agenda Equals Mov_Destinazioni.Id_Agenda And
                                                            Movimenti_Dettagli.Id_Mov Equals Mov_Destinazioni.Id_Mov And
                                                            Movimenti_Dettagli.Id_Mov_Det Equals Mov_Destinazioni.Id_Mov_Det
                                     Where Movimenti.Data_Movimento > Data And
                                            Mov_Destinazioni.Tipo_Destinazione = TIPO_DESTINAZIONE_ANIMALE And
                                            Cod_Progetto_list.Contains(Mov_Destinazioni.Id_Destinazione) And
                                            Mov_Destinazioni.Piva = Piva).ToList

        If Mov_Destinazioni_List.Count > 0 Then
            Dim cod_progettoMancanti As List(Of Integer) = Mov_Destinazioni_List.
                Select(Function(m) m.Mov_Destinazioni.Id_Destinazione).
                Distinct.
                Where(Function(codAnimale) Not list_AnimaliMovimentati.Contains(codAnimale)).
                ToList

            '(From a In Mov_Destinazioni_List Select a.Mov_Destinazioni.Id_Destinazione).Distinct().ToList()
            'Dim cod_progettoMancanti As List(Of Integer?) = (From a In movDestCodProgetti Where Not list_AnimaliMovimentati.Contains(a)).ToList()

            If cod_progettoMancanti.Count > 0 Then list_AnimaliMovimentati.AddRange(cod_progettoMancanti)
        End If

        Return list_AnimaliMovimentati

    End Function

    'Spostata dal vecchio ZooBIZ
    Public Function Zoo_Animale_Anagrafe_Scrivi(
                                    ByVal DatiZoo_Animale_Anagrafe As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                            As Integer

        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Zoo_Animale_Anagrafe_W.Zoo_Animale_Anagrafe_Scrivi()"

        '----------------------------------------------------------------------

        Dim Dummy As Boolean
        Dim XmlDoc As XmlDocument

        'Dim ObjSequenze               As Agro_Contab_AD.Agro_Sequenze
        'Dim ObjZoo_Anagrafiche        As Agro_Zoo_AD.Zoo_Animali_W

        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze
        Dim ObjZoo_Anagrafiche As AgronicaCoreZooDAL.Zoo_Animali_W

        Dim xZoo_Anagrafiche As XmlNodeList             'IXMLDOMNodeList
        Dim xZoo_Anagrafica As XmlElement               'IXMLDOMElement

        Dim i_Zoo_Anagrafica As Integer

        Dim OpeDB_Zoo_Anagrafica As String

        Dim Cod_Progetto As Integer

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------


        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                FlagConnessioneLocale = True
            End If

            If objParametri.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                FlagTransazioneLocale = True
            End If

            '------------------------------

            XmlDoc = New Xml.XmlDocument
            'XmlDoc.async = False
            XmlDoc.LoadXml(DatiZoo_Animale_Anagrafe)

            '------------------------------
            '------------------------------
            '------------------------------



            '##################################################
            '###############  ANAGRAFE ZOOTECNICA  ############
            '##################################################

            'Prelevo l'elenco delle Anagrafe Zootecniche
            xZoo_Anagrafiche = XmlDoc.GetElementsByTagName("Zoo_Animale_Anagrafe")

            i_Zoo_Anagrafica = 0

            Do While i_Zoo_Anagrafica < xZoo_Anagrafiche.Count

                'Prelevo l'i-esima zoo anagrafica
                xZoo_Anagrafica = xZoo_Anagrafiche.Item(i_Zoo_Anagrafica)

                'Prelevo gli attributi della macchina/attrezzatura selezionata
                OpeDB_Zoo_Anagrafica = xZoo_Anagrafica.GetAttribute("TipoOperazioneDB")

                'Inizializzo Preventivamente il Codice dell'Animale
                Cod_Progetto = CInt(xZoo_Anagrafica.GetAttribute("cod_progetto"))

                'Creo l'oggetto COM

                'ObjZoo_Anagrafiche = CreateObject("Agro_Zoo_AD.Zoo_Animali_W")
                ObjZoo_Anagrafiche = New AgronicaCoreZooDAL.Zoo_Animali_W

                'Verifico l'operazione richiesta
                Select Case OpeDB_Zoo_Anagrafica
                    '
                    Case "0"    'LEGGI -------------------------------------------------------
                        '
                    Case "1"    'SALVA -------------------------------------------------------
                        '
                        'Richiedo un nuovo codice del progetto zootecnico

                        If Cod_Progetto <= 0 Then

                            'ObjSequenze = CreateObject("Agro_Contab_AD.Agro_Sequenze")
                            ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                            Cod_Progetto = ObjSequenze.NuovoId_Tabella(
                                                "Impresa_Progetto",
                                                CInt(xZoo_Anagrafica.GetAttribute("basecode")),
                                                CInt(xZoo_Anagrafica.GetAttribute("topcode")),
                                                objParametri)

                            ObjSequenze = Nothing

                        Else

                            'Esportazione in Locale

                        End If

                        'Salvo l'anagrafica zootecnica
                        Dummy = ObjZoo_Anagrafiche.Scrivi(
                                                CStr(xZoo_Anagrafica.GetAttribute("piva")),
                                                0,
                                                Cod_Progetto,
                                                CStr(xZoo_Anagrafica.GetAttribute("matricola")),
                                                CInt(xZoo_Anagrafica.GetAttribute("gen_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("spe_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("raz_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("ipro_cod")),
                                                CStr(xZoo_Anagrafica.GetAttribute("nome")),
                                                CStr(xZoo_Anagrafica.GetAttribute("collare")),
                                                CStr(xZoo_Anagrafica.GetAttribute("nome_aia")),
                                                CStr(xZoo_Anagrafica.GetAttribute("matricola_aia")),
                                                CDate(xZoo_Anagrafica.GetAttribute("dat_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("prov_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("stato_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("aua_azi_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("ausl_azi_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("sesso")),
                                                CStr(xZoo_Anagrafica.GetAttribute("mat_padre")),
                                                CStr(xZoo_Anagrafica.GetAttribute("mat_madre")),
                                                CStr(xZoo_Anagrafica.GetAttribute("cf_proprietario")),
                                                CStr(xZoo_Anagrafica.GetAttribute("cf_detentore")),
                                                CInt(xZoo_Anagrafica.GetAttribute("presente")),
                                                CInt(xZoo_Anagrafica.GetAttribute("cat_cod")),
                                                CDbl(xZoo_Anagrafica.GetAttribute("peso")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("data_pesa")), AGRODATAINIZIO, xZoo_Anagrafica.GetAttribute("data_pesa")), CInt(xZoo_Anagrafica.GetAttribute("metodo_produzione")),
                                                CInt(xZoo_Anagrafica.GetAttribute("regolamento_cod")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("conversione_inizio")), AGRODATAINIZIO, xZoo_Anagrafica.GetAttribute("conversione_inizio")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("conversione_fine")), AGRODATAFINE, xZoo_Anagrafica.GetAttribute("conversione_fine")),
                                                If(xZoo_Anagrafica.HasAttribute("chk_batteria") = False, 0, xZoo_Anagrafica.GetAttribute("chk_batteria")),
                                                CDate(xZoo_Anagrafica.GetAttribute("validita_inizio")),
                                                CDate(xZoo_Anagrafica.GetAttribute("validita_fine")),
                                                objParametri)

                        '
                    Case "2"    'MODIFICA -------------------------------------------------------
                        '
                        ObjZoo_Anagrafiche.Modifica(
                                                CStr(xZoo_Anagrafica.GetAttribute("piva")),
                                                0,
                                                Cod_Progetto,
                                                CStr(xZoo_Anagrafica.GetAttribute("matricola")),
                                                CInt(xZoo_Anagrafica.GetAttribute("gen_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("spe_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("raz_cod")),
                                                CInt(xZoo_Anagrafica.GetAttribute("ipro_cod")),
                                                CStr(xZoo_Anagrafica.GetAttribute("nome")),
                                                CStr(xZoo_Anagrafica.GetAttribute("collare")),
                                                CStr(xZoo_Anagrafica.GetAttribute("nome_aia")),
                                                CStr(xZoo_Anagrafica.GetAttribute("matricola_aia")),
                                                CDate(xZoo_Anagrafica.GetAttribute("dat_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("prov_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("stato_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("aua_azi_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("ausl_azi_nascita")),
                                                CStr(xZoo_Anagrafica.GetAttribute("sesso")),
                                                CStr(xZoo_Anagrafica.GetAttribute("mat_padre")),
                                                CStr(xZoo_Anagrafica.GetAttribute("mat_madre")),
                                                CStr(xZoo_Anagrafica.GetAttribute("cf_proprietario")),
                                                CStr(xZoo_Anagrafica.GetAttribute("cf_detentore")),
                                                CInt(xZoo_Anagrafica.GetAttribute("presente")),
                                                CInt(xZoo_Anagrafica.GetAttribute("cat_cod")),
                                                CDbl(xZoo_Anagrafica.GetAttribute("peso")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("data_pesa")), AGRODATAINIZIO, xZoo_Anagrafica.GetAttribute("data_pesa")),
                                                CInt(xZoo_Anagrafica.GetAttribute("metodo_produzione")),
                                                CInt(xZoo_Anagrafica.GetAttribute("regolamento_cod")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("conversione_inizio")), AGRODATAINIZIO, xZoo_Anagrafica.GetAttribute("conversione_inizio")),
                                                If(Not IsDate(xZoo_Anagrafica.GetAttribute("conversione_fine")), AGRODATAFINE, xZoo_Anagrafica.GetAttribute("conversione_fine")),
                                                If(xZoo_Anagrafica.HasAttribute("chk_batteria") = False, 0, xZoo_Anagrafica.GetAttribute("chk_batteria")),
                                                CDate(xZoo_Anagrafica.GetAttribute("validita_inizio")),
                                                CDate(xZoo_Anagrafica.GetAttribute("validita_fine")),
                                                "",
                                                objParametri)



                    Case "3"    'ELIMINA -------------------------------------------------------
                        '

                        ObjZoo_Anagrafiche.Cancella(
                                                CStr(xZoo_Anagrafica.GetAttribute("piva")),
                                                0,
                                                Cod_Progetto,
                                                "",
                                                objParametri)

                End Select



                'Elimino l'oggetto
                ObjZoo_Anagrafiche = Nothing
                'Incremento l'indice
                i_Zoo_Anagrafica += 1

            Loop


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            ObjSequenze = Nothing

            XmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If FlagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If FlagConnessioneLocale = True AndAlso objParametri.objConnessione IsNot Nothing Then
                objParametri.objConnessione.Close()
            End If

        End Try

        Return Cod_Progetto

    End Function

    Public Function Get_Animale(Cod_Animale As Integer, objParametri_Server As AgronicaCoreParametri)
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        'Dim scope As New TransactionScope()
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim obj_animale = (From a In GiasContext.Zoo_Animali Where a.Cod_Progetto = Cod_Animale Select a).FirstOrDefault

        Return obj_animale

    End Function

    ''' <summary>
    ''' Permette di chiamare le altre funzioni passando 
    ''' un obj AgronicaCoreModelsSTD.anagrafiche.Animale
    ''' </summary>
    ''' <param name="obj_CapoAnimale"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function Converti_Animale_DT(ByVal obj_CapoAnimale As anagrafiche.CapoAnimale,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                        Optional OpenNewTransaction As Boolean = True, Optional NoteLog As String = "Converti_Animale_DT") As String
        Dim r As New RispostaStandard

        If OpenNewTransaction Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
        End If

        Dim modifica As Boolean = False

        'Se impostato a True va in cancellazione dell'elemento
        If obj_CapoAnimale.flagCancellazione = False Then

            Dim Zoo_Animali As New AgronicaCoreEntityFramework_POCO.Zoo_Animali
            Dim Zoo_Animali_Distinte As New List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte)
            Dim Zoo_Animali_StatiAccr As New List(Of AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento)

            'SCRITTURA
            If IsNothing(obj_CapoAnimale.codice) OrElse obj_CapoAnimale.codice = 0 Then

                'ZOO_ANIMALI
                Zoo_Animali = New AgronicaCoreEntityFramework_POCO.Zoo_Animali

                Zoo_Animali.PIVA = obj_CapoAnimale.partitaIva
                Zoo_Animali.Cod_Progetto = obj_CapoAnimale.codice
                Zoo_Animali.sa_cod = 0
                Zoo_Animali.GEN_COD = obj_CapoAnimale.genere.codice
                Zoo_Animali.SPE_COD = obj_CapoAnimale.specie.codice
                Zoo_Animali.IPRO_COD = obj_CapoAnimale.indirizzoProd.codice
                Zoo_Animali.RAZ_COD = obj_CapoAnimale.razza.codice
                Zoo_Animali.CAT_COD = obj_CapoAnimale.categoria.codice
                Zoo_Animali.TIPO_COD = obj_CapoAnimale.tipologia.codice
                Zoo_Animali.Matricola = obj_CapoAnimale.matricola
                Zoo_Animali.Nome = obj_CapoAnimale.nome
                Zoo_Animali.Collare = obj_CapoAnimale.collare
                Zoo_Animali.Progetto = obj_CapoAnimale.matricola
                Zoo_Animali.Id_Capo_BDN = obj_CapoAnimale.idCapo_BDN
                Zoo_Animali.Certificato = obj_CapoAnimale.numCertificato
                Zoo_Animali.Sesso = obj_CapoAnimale.sesso
                Zoo_Animali.DAT_NASCITA = obj_CapoAnimale.dataNascita
                Zoo_Animali.Validita_Inizio = obj_CapoAnimale.validita.inizio
                Zoo_Animali.Validita_Fine = obj_CapoAnimale.validita.fine
                Zoo_Animali.MAT_MADRE = obj_CapoAnimale.madre.matricola
                Zoo_Animali.Razza_Madre = obj_CapoAnimale.madre.razza.codice
                Zoo_Animali.Razza_Padre = obj_CapoAnimale.padre.razza.codice
                Zoo_Animali.MAT_PADRE = obj_CapoAnimale.padre.matricola
                Zoo_Animali.Cod_Progetto_Madre = obj_CapoAnimale.madre.codice
                Zoo_Animali.Cod_Progetto_Padre = obj_CapoAnimale.padre.codice
                Zoo_Animali.CF_Fornitore = obj_CapoAnimale.fornitore.primaryKey.partitaIva
                Zoo_Animali.Lotto_Fornitore = obj_CapoAnimale.lottoFornitore
                Zoo_Animali.Metodo_Produzione = obj_CapoAnimale.metodoProduzione.codice
                Zoo_Animali.Conversione_Inizio = obj_CapoAnimale.validitaConversione.inizio
                Zoo_Animali.Conversione_Fine = obj_CapoAnimale.validitaConversione.fine
                Zoo_Animali.MATRICOLA_AIA = ""
                Zoo_Animali.NOME_AIA = ""
                Zoo_Animali.PROV_NASCITA = ""
                Zoo_Animali.AUA_AZI_NASCITA = ""
                Zoo_Animali.AUSL_AZI_NASCITA = obj_CapoAnimale.codiceAziendaNascita
                Zoo_Animali.STATO_NASCITA = ""
                Zoo_Animali.DATA_PESA = DateTime.Now
                Zoo_Animali.DT_Variazione = DateTime.Now
                Zoo_Animali.CF_DETENTORE = obj_CapoAnimale.codiceFiscaleDetentore
                Zoo_Animali.CF_PROPRIETARIO = obj_CapoAnimale.codiceFiscaleProprietario
                Zoo_Animali.Modello4_Ingresso = obj_CapoAnimale.ingresso_mm_id
                Zoo_Animali.Modello4_Uscita = obj_CapoAnimale.uscita_mm_id
                Zoo_Animali.Modello4_Ingresso_Numero = obj_CapoAnimale.ingresso_modello4_numero
                Zoo_Animali.Modello4_Ingresso_Prenotazione = obj_CapoAnimale.ingresso_modello4_prenotazione
                Zoo_Animali.Data_Documento_Ingresso = AGRODATAINIZIO
                If obj_CapoAnimale.ingresso_modello4_data_prenotazione > AGRODATAINIZIO Then
                    Zoo_Animali.Data_Documento_Ingresso = obj_CapoAnimale.ingresso_modello4_data_prenotazione
                End If
                Zoo_Animali.Modello4_Uscita_Numero = obj_CapoAnimale.uscita_modello4_numero
                Zoo_Animali.Modello4_Uscita_Prenotazione = obj_CapoAnimale.uscita_modello4_prenotazione

                Zoo_Animali.Data_Documento_Uscita = AGRODATAFINE
                If obj_CapoAnimale.uscita_modello4_data_prenotazione > AGRODATAINIZIO Then
                    Zoo_Animali.Data_Documento_Uscita = obj_CapoAnimale.uscita_modello4_data_prenotazione
                End If

                If obj_CapoAnimale.causaleMorte > 0 Then
                    Zoo_Animali.Causale_Morte = obj_CapoAnimale.causaleMorte
                End If

                Zoo_Animali.Codice_Azienda_Uscita = obj_CapoAnimale.Codice_Azienda_Uscita
                Zoo_Animali.Codice_Azienda_Fornitore = obj_CapoAnimale.codiceAziendaFornitore
                Zoo_Animali.Stalla_Svezzamento = obj_CapoAnimale.stallaSvezzamento
                Zoo_Animali.inviato = 0

                'DISTINTE
                If Not IsNothing(obj_CapoAnimale.esercizi) OrElse obj_CapoAnimale.esercizi.Count > 0 Then
                    For Each es In obj_CapoAnimale.esercizi
                        Dim Distinta As New AgronicaCoreEntityFramework_POCO.Zoo_Animali_Distinte

                        Distinta.Codice_Distinta = ""
                        Distinta.Cod_Progetto = es.codice_capo_animale
                        Distinta.Progetto_Nome = es.progettoNome
                        Distinta.Progetto_Des = es.descrizione
                        Distinta.Validita_Inizio = es.validita.inizio
                        Distinta.Validita_Fine = es.validita.fine
                        Distinta.PIVA = obj_CapoAnimale.partitaIva
                        Distinta.sa_cod = 0
                        Distinta.Cod_Animale = obj_CapoAnimale.codice
                        Distinta.Username_Creazione = ""
                        Distinta.Username_Modifica = ""
                        Distinta.Distinta_Chiusa = 0
                        Distinta.inviato = 0

                        Zoo_Animali_Distinte.Add(Distinta)
                    Next
                End If

                'STATI ACCRESCIMENTO
                If Not IsNothing(obj_CapoAnimale.statiAccrescimento) OrElse obj_CapoAnimale.statiAccrescimento.Count > 0 Then
                    For Each sa In obj_CapoAnimale.statiAccrescimento
                        Dim StatoAccr As New AgronicaCoreEntityFramework_POCO.Zoo_AnimalixStati_Accrescimento

                        StatoAccr.STATO_COD = sa.codice
                        StatoAccr.Validita_Inizio = sa.validita.inizio
                        StatoAccr.Validita_Fine = sa.validita.fine
                        StatoAccr.PIVA = obj_CapoAnimale.partitaIva
                        StatoAccr.sa_cod = 0
                        StatoAccr.Cod_Progetto = obj_CapoAnimale.codice
                        StatoAccr.GEN_COD = obj_CapoAnimale.genere.codice
                        StatoAccr.SPE_COD = obj_CapoAnimale.specie.codice
                        StatoAccr.TIPO_COD = obj_CapoAnimale.tipologia.codice
                        StatoAccr.Username_Creazione = ""
                        StatoAccr.Username_Modifica = ""
                        StatoAccr.inviato = 0

                        Zoo_Animali_StatiAccr.Add(StatoAccr)
                    Next
                End If

                modifica = True

                'MODIFICA
            Else

                'ZOO_ANIMALI
                Zoo_Animali = (From capo In GiasContext.Zoo_Animali
                               Where capo.PIVA = obj_CapoAnimale.partitaIva And capo.Cod_Progetto = obj_CapoAnimale.codice And capo.Matricola = obj_CapoAnimale.matricola
                               Select capo).First

                'controllo sui campi modificati
                If Not IsNothing(obj_CapoAnimale.nome) AndAlso obj_CapoAnimale.nome <> "" AndAlso Zoo_Animali.Nome <> obj_CapoAnimale.nome Then
                    Zoo_Animali.Nome = obj_CapoAnimale.nome
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.collare) AndAlso obj_CapoAnimale.collare <> "" AndAlso Zoo_Animali.Collare <> obj_CapoAnimale.collare Then
                    Zoo_Animali.Collare = obj_CapoAnimale.collare
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.sesso) AndAlso obj_CapoAnimale.sesso <> "" AndAlso Zoo_Animali.Sesso <> obj_CapoAnimale.sesso Then
                    Zoo_Animali.Sesso = obj_CapoAnimale.sesso
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.idCapo_BDN) AndAlso obj_CapoAnimale.idCapo_BDN <> 0 AndAlso Zoo_Animali.Id_Capo_BDN <> obj_CapoAnimale.idCapo_BDN Then
                    Zoo_Animali.Id_Capo_BDN = obj_CapoAnimale.idCapo_BDN
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.numCertificato) AndAlso obj_CapoAnimale.numCertificato <> "" AndAlso Zoo_Animali.Certificato <> obj_CapoAnimale.numCertificato Then
                    Zoo_Animali.Certificato = obj_CapoAnimale.numCertificato
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.madre) Then
                    If Not IsNothing(obj_CapoAnimale.madre.matricola) AndAlso obj_CapoAnimale.madre.matricola <> "" AndAlso Zoo_Animali.MAT_MADRE <> obj_CapoAnimale.madre.matricola Then
                        Zoo_Animali.MAT_MADRE = obj_CapoAnimale.madre.matricola
                        modifica = True
                    End If

                    If Not IsNothing(obj_CapoAnimale.madre.razza.codice) AndAlso Zoo_Animali.Razza_Madre <> obj_CapoAnimale.madre.razza.codice Then
                        Zoo_Animali.Razza_Madre = obj_CapoAnimale.madre.razza.codice
                        modifica = True
                    End If
                End If

                If Not IsNothing(obj_CapoAnimale.padre) Then
                    If Not IsNothing(obj_CapoAnimale.padre.matricola) AndAlso obj_CapoAnimale.padre.matricola <> "" AndAlso Zoo_Animali.MAT_PADRE <> obj_CapoAnimale.padre.matricola Then
                        Zoo_Animali.MAT_PADRE = obj_CapoAnimale.padre.matricola
                        modifica = True
                    End If

                    If Not IsNothing(obj_CapoAnimale.padre.razza.codice) AndAlso Zoo_Animali.Razza_Padre <> obj_CapoAnimale.padre.razza.codice Then
                        Zoo_Animali.Razza_Padre = obj_CapoAnimale.padre.razza.codice
                        modifica = True
                    End If
                End If

                If Not IsNothing(obj_CapoAnimale.dataNascita) AndAlso obj_CapoAnimale.dataNascita > AGRODATAINIZIO AndAlso Zoo_Animali.DAT_NASCITA <> obj_CapoAnimale.dataNascita Then
                    Zoo_Animali.DAT_NASCITA = obj_CapoAnimale.dataNascita
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.validitaConversione) Then
                    If Not IsNothing(obj_CapoAnimale.validitaConversione.inizio) AndAlso Zoo_Animali.Conversione_Inizio <> obj_CapoAnimale.validitaConversione.inizio Then
                        Zoo_Animali.Conversione_Inizio = obj_CapoAnimale.validitaConversione.inizio
                        modifica = True
                    End If

                    If Not IsNothing(obj_CapoAnimale.validitaConversione.fine) AndAlso Zoo_Animali.Conversione_Fine <> obj_CapoAnimale.validitaConversione.fine Then
                        Zoo_Animali.Conversione_Fine = obj_CapoAnimale.validitaConversione.fine
                        modifica = True
                    End If
                End If

                If Not IsNothing(obj_CapoAnimale.codiceFiscaleProprietario) AndAlso obj_CapoAnimale.codiceFiscaleProprietario <> "" AndAlso Zoo_Animali.CF_PROPRIETARIO <> obj_CapoAnimale.codiceFiscaleProprietario Then
                    Zoo_Animali.CF_PROPRIETARIO = obj_CapoAnimale.codiceFiscaleProprietario
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.codiceFiscaleDetentore) AndAlso obj_CapoAnimale.codiceFiscaleDetentore <> "" AndAlso Zoo_Animali.CF_DETENTORE <> obj_CapoAnimale.codiceFiscaleDetentore Then
                    Zoo_Animali.CF_DETENTORE = obj_CapoAnimale.codiceFiscaleDetentore
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.codiceAziendaNascita) AndAlso obj_CapoAnimale.codiceAziendaNascita <> "" AndAlso Zoo_Animali.AUSL_AZI_NASCITA <> obj_CapoAnimale.codiceAziendaNascita Then
                    Zoo_Animali.AUSL_AZI_NASCITA = obj_CapoAnimale.codiceAziendaNascita
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.validita) Then
                    If Not IsNothing(obj_CapoAnimale.validita.inizio) AndAlso Zoo_Animali.Validita_Inizio <> obj_CapoAnimale.validita.inizio Then
                        Zoo_Animali.Validita_Inizio = obj_CapoAnimale.validita.inizio
                        modifica = True
                    End If

                    If Not IsNothing(obj_CapoAnimale.validita.fine) AndAlso Zoo_Animali.Validita_Fine <> obj_CapoAnimale.validita.fine Then
                        Zoo_Animali.Validita_Fine = obj_CapoAnimale.validita.fine
                        modifica = True
                    End If
                End If

                If Not IsNothing(obj_CapoAnimale.ingresso_mm_id) AndAlso obj_CapoAnimale.ingresso_mm_id <> "" AndAlso Zoo_Animali.Modello4_Ingresso <> obj_CapoAnimale.ingresso_mm_id Then
                    Zoo_Animali.Modello4_Ingresso = obj_CapoAnimale.ingresso_mm_id
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.uscita_mm_id) AndAlso obj_CapoAnimale.uscita_mm_id <> "" AndAlso Zoo_Animali.Modello4_Uscita <> obj_CapoAnimale.uscita_mm_id Then
                    Zoo_Animali.Modello4_Uscita = obj_CapoAnimale.uscita_mm_id
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.ingresso_modello4_numero) AndAlso obj_CapoAnimale.ingresso_modello4_numero <> "" AndAlso Zoo_Animali.Modello4_Ingresso_Numero <> obj_CapoAnimale.ingresso_modello4_numero Then
                    Zoo_Animali.Modello4_Ingresso_Numero = obj_CapoAnimale.ingresso_modello4_numero
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.ingresso_modello4_prenotazione) AndAlso Zoo_Animali.Modello4_Ingresso_Prenotazione <> obj_CapoAnimale.ingresso_modello4_prenotazione Then
                    Zoo_Animali.Modello4_Ingresso_Prenotazione = obj_CapoAnimale.ingresso_modello4_prenotazione
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.ingresso_modello4_data_prenotazione) AndAlso obj_CapoAnimale.ingresso_modello4_data_prenotazione > AGRODATAINIZIO AndAlso Zoo_Animali.Data_Documento_Ingresso <> obj_CapoAnimale.ingresso_modello4_data_prenotazione Then
                    Zoo_Animali.Data_Documento_Ingresso = obj_CapoAnimale.ingresso_modello4_data_prenotazione
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.uscita_modello4_numero) AndAlso obj_CapoAnimale.uscita_modello4_numero <> "" AndAlso Zoo_Animali.Modello4_Uscita_Numero <> obj_CapoAnimale.uscita_modello4_numero Then
                    Zoo_Animali.Modello4_Uscita_Numero = obj_CapoAnimale.uscita_modello4_numero
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.uscita_modello4_prenotazione) AndAlso obj_CapoAnimale.uscita_modello4_prenotazione <> "" AndAlso Zoo_Animali.Modello4_Uscita_Prenotazione <> obj_CapoAnimale.uscita_modello4_prenotazione Then
                    Zoo_Animali.Modello4_Uscita_Prenotazione = obj_CapoAnimale.uscita_modello4_prenotazione
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.uscita_modello4_data_prenotazione) AndAlso Zoo_Animali.Data_Documento_Uscita <> obj_CapoAnimale.uscita_modello4_data_prenotazione Then
                    Zoo_Animali.Data_Documento_Uscita = obj_CapoAnimale.uscita_modello4_data_prenotazione
                    modifica = True
                End If

                If obj_CapoAnimale.causaleMorte > 0 Then
                    Zoo_Animali.Causale_Morte = obj_CapoAnimale.causaleMorte
                End If

                If Not IsNothing(obj_CapoAnimale.Codice_Azienda_Uscita) AndAlso obj_CapoAnimale.Codice_Azienda_Uscita <> "" AndAlso Zoo_Animali.Codice_Azienda_Uscita <> obj_CapoAnimale.Codice_Azienda_Uscita Then
                    Zoo_Animali.Codice_Azienda_Uscita = obj_CapoAnimale.Codice_Azienda_Uscita
                    modifica = True
                End If

                If Not IsNothing(obj_CapoAnimale.stallaSvezzamento) AndAlso obj_CapoAnimale.stallaSvezzamento <> "" AndAlso Zoo_Animali.Stalla_Svezzamento <> obj_CapoAnimale.stallaSvezzamento Then
                    Zoo_Animali.Stalla_Svezzamento = obj_CapoAnimale.stallaSvezzamento
                    modifica = True
                End If

                'DISTINTE
                Zoo_Animali_Distinte = (From distinte In GiasContext.Zoo_Animali_Distinte
                                        Where distinte.PIVA = obj_CapoAnimale.partitaIva And distinte.Cod_Animale = obj_CapoAnimale.codice
                                        Select distinte).ToList()

                If Not IsNothing(obj_CapoAnimale.esercizi) Then
                    For i = 0 To Zoo_Animali_Distinte.Count - 1
                        If Not IsNothing(obj_CapoAnimale.esercizi(i)) AndAlso Not IsNothing(obj_CapoAnimale.esercizi(i).validita) Then
                            If Not IsNothing(obj_CapoAnimale.esercizi(i).validita.inizio) AndAlso Zoo_Animali_Distinte(i).Validita_Inizio <> obj_CapoAnimale.esercizi(i).validita.inizio Then
                                Zoo_Animali_Distinte(i).Validita_Inizio = obj_CapoAnimale.esercizi(i).validita.inizio
                                modifica = True
                            End If

                            If Not IsNothing(obj_CapoAnimale.esercizi(i).validita.fine) AndAlso Zoo_Animali_Distinte(i).Validita_Fine <> obj_CapoAnimale.esercizi(i).validita.fine Then
                                Zoo_Animali_Distinte(i).Validita_Fine = obj_CapoAnimale.esercizi(i).validita.fine
                                modifica = True
                            End If
                        End If
                    Next
                End If

                'STATI ACCRESCIMENTO
                Zoo_Animali_StatiAccr = (From stati In GiasContext.Zoo_AnimalixStati_Accrescimento
                                         Where stati.PIVA = obj_CapoAnimale.partitaIva And stati.Cod_Progetto = obj_CapoAnimale.codice
                                         Select stati).ToList()

                If Not IsNothing(obj_CapoAnimale.statiAccrescimento) Then
                    For i = 0 To Zoo_Animali_StatiAccr.Count - 1
                        If Not IsNothing(obj_CapoAnimale.statiAccrescimento(i)) AndAlso Not IsNothing(obj_CapoAnimale.statiAccrescimento(i).validita) Then
                            If Not IsNothing(obj_CapoAnimale.statiAccrescimento(i).validita.inizio) AndAlso Zoo_Animali_StatiAccr(i).Validita_Inizio <> obj_CapoAnimale.statiAccrescimento(i).validita.inizio Then
                                Zoo_Animali_StatiAccr(i).Validita_Inizio = obj_CapoAnimale.statiAccrescimento(i).validita.inizio
                                modifica = True
                            End If

                            If Not IsNothing(obj_CapoAnimale.statiAccrescimento(i).validita.fine) AndAlso Zoo_Animali_StatiAccr(i).Validita_Fine <> obj_CapoAnimale.statiAccrescimento(i).validita.fine Then
                                Zoo_Animali_StatiAccr(i).Validita_Fine = obj_CapoAnimale.statiAccrescimento(i).validita.fine
                                modifica = True
                            End If
                        End If
                    Next
                End If

            End If

            If modifica Then

                r = Scrivi_Modifica_Zoo_Animali(Zoo_Animali, Zoo_Animali_Distinte.ToArray, Zoo_Animali_StatiAccr.ToArray,
                                                objParametri_Server, GiasContext, OpenNewTransaction, Nothing, False, True, NoteLog)

                Dim obj_RispostaStringa As JObject = JsonConvert.DeserializeObject(Of JObject)(r.RispostaStringa)
                Dim Cod_Progetto As String = obj_RispostaStringa("Cod_Progetto")

                r.RispostaOK = True
                r.RispostaStringa = Cod_Progetto

            Else
                r.RispostaOK = True
                r.RispostaStringa = ""

            End If

            'CANCELLAZIONE
        Else

            r = Elimina_Animale(obj_CapoAnimale.partitaIva,
                                obj_CapoAnimale.codice,
                                objParametri_Server,
                                GiasContext,
                                OpenNewTransaction)

            r.RispostaStringa = obj_CapoAnimale.codice

        End If

        'chiudo la transazione
        If OpenNewTransaction And modifica Then
            GiasContext.Core.AcceptAllChanges()
        End If

        Return r.RispostaStringa

    End Function

    Public Function LeggiListaSpecieDaRegolamento(Regolamento_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As DataTable
        Dim objDAL As New AgronicaCoreZooDAL.Lista_Specie_Animali_R
        Return objDAL.LeggiDaRegolamento(Regolamento_Cod, objParametri_Server)
    End Function

    ''' <summary>
    ''' Legge le categorie animali assicurando che ad esse sia collegata almeno una stabulazione.
    ''' </summary>
    Public Function LeggiListaCategorieDaRegolamentoSpecieConStabulazione(
        Regolamento_Cod As Integer,
        Spe_Cod As Integer, Gen_Cod As Integer,
        objParametri_Server As AgronicaCoreParametri
    ) As DataTable
        Dim objDAL As New AgronicaCoreMetaSchemaDAL.Lista_Categorie_Animali_R
        Return objDAL.LeggiDaRegolamentoSpecieConStabulazione(Regolamento_Cod, Spe_Cod, Gen_Cod, objParametri_Server)
    End Function

    Public Function LeggiListaCategorieDaRegolamentoSpecie(
        Regolamento_Cod As Integer,
        Spe_Cod As Integer, Gen_Cod As Integer,
        objParametri_Server As AgronicaCoreParametri
    ) As DataTable
        Dim objDAL As New AgronicaCoreZooDAL.Lista_Categorie_Animali_R
        Return objDAL.LeggiDaRegolamentoSpecie(Regolamento_Cod, Spe_Cod, Gen_Cod, objParametri_Server)
    End Function

    Public Function LeggiStabulazioneCategoria(
        Regolamento_Cod As Integer,
        Spe_Cod As Integer, Gen_Cod As Integer,
        Cat_Cod As Integer, IPro_Cod As Integer,
        objParametri_Server As AgronicaCoreParametri
    ) As DataTable
        Dim objDAL As New AgronicaCoreMetaSchemaDAL.Lista_tipi_stalla_R
        Return objDAL.LeggiDaRegolamentoCategoria(Regolamento_Cod, Spe_Cod, Gen_Cod, Cat_Cod, IPro_Cod, objParametri_Server)
    End Function


    Public Function OttieniDatiNogmo(ByVal listCod_Animali As List(Of String), ByRef objparametri_server As AgronicaCoreParametri)
        Dim Zoo_AnimaliDAL = New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dt = Zoo_AnimaliDAL.OttieniDatiNogmo(listCod_Animali, objparametri_server)
        Return dt
    End Function

    Public Function ottieniRazzeNogmo(ByRef objparametri_server As AgronicaCoreParametri)
        Dim Zoo_AnimaliDAL = New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dt = Zoo_AnimaliDAL.ottieniRazzeNogmo(objparametri_server)
        Return dt
    End Function
End Class
