Imports System.Data.Entity
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUtility
Imports Newtonsoft.Json

Public Class RibaltamentoAnagraficheColturali_R
    Public Shared Function Get_AppezzamentoRibaltato(Id_Budget As Integer,
                                                     AppezzamentoOrigineEF As Budget_Appezzamento,
                                                     ByRef existsRibaltato As Boolean,
                                                     ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                                     ByRef msg As String,
                                                     objParametri_Server As AgronicaCoreParametri,
                                                     GiasContext As Gias_DeveloperServer_Entities
                                                     ) As Appezzamento

        Dim AppezzamentoDestinazioneEF As New Appezzamento

        Dim piva_Origine As String = AppezzamentoOrigineEF.PIVA
        Dim sa_cod_Origine As Integer = AppezzamentoOrigineEF.SA_COD
        Dim appezza_Origine As Integer = AppezzamentoOrigineEF.APPEZZA
        Dim campo_cod_Origine As Integer = AppezzamentoOrigineEF.Campo_Cod

        Dim dummyRibaltamento_Appezzamento = (From rib In GiasContext.Ribaltamento_Appezzamento
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                  rib.Budget_Piva = piva_Origine AndAlso
                                                  rib.Budget_Sa_Cod = sa_cod_Origine AndAlso
                                                  rib.Budget_Appezza = appezza_Origine).FirstOrDefault()

        If dummyRibaltamento_Appezzamento IsNot Nothing Then
            existsRibaltato = True

            Dim piva_Destinazione As String = dummyRibaltamento_Appezzamento.Reale_Piva
            Dim sa_Cod_Destinazione As Integer = dummyRibaltamento_Appezzamento.Reale_Sa_Cod
            Dim appezza_Destinazione As Integer = dummyRibaltamento_Appezzamento.Reale_Appezza


            '-------------------------------
            '   APPEZZAMENTO
            '-------------------------------
            AppezzamentoDestinazioneEF = (From app In GiasContext.Appezzamento
                                          Where app.PIVA = piva_Destinazione AndAlso
                                              app.SA_COD = sa_Cod_Destinazione AndAlso
                                              app.APPEZZA = appezza_Destinazione).FirstOrDefault()

            msg = EsistonoMovimenti(AppezzamentoDestinazioneEF, objParametri_Server, GiasContext)
            If msg <> "" Then
                Exit Function
            End If

            '-------------------------------
            '   CAMPO
            '-------------------------------
            If AppezzamentoDestinazioneEF.Campo_Cod <> 0 Then
                Dim dummyRibaltamento_Campo = (From rib In GiasContext.Ribaltamento_Campi
                                               Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                   rib.Reale_Piva = piva_Destinazione AndAlso
                                                   rib.Reale_Sa_Cod = sa_Cod_Destinazione AndAlso
                                                   rib.Reale_Campo_Cod = AppezzamentoDestinazioneEF.Campo_Cod).FirstOrDefault()

                If Not dicAnagrafica_Ribaltata.dicCampi.ContainsKey((dummyRibaltamento_Campo.Budget_Piva, dummyRibaltamento_Campo.Budget_Sa_Cod, dummyRibaltamento_Campo.Budget_Campo_Cod)) Then
                    dicAnagrafica_Ribaltata.dicCampi.Add((dummyRibaltamento_Campo.Budget_Piva, dummyRibaltamento_Campo.Budget_Sa_Cod, dummyRibaltamento_Campo.Budget_Campo_Cod), ((dummyRibaltamento_Campo.Reale_Piva, dummyRibaltamento_Campo.Reale_Sa_Cod, dummyRibaltamento_Campo.Reale_Campo_Cod), False))
                End If
            End If


            '-------------------------------
            '   APPEZZAMENTI X INDIRIZZI
            '-------------------------------
            Dim dummyRibaltamento_AppezzamentoxIndirizzi = (From rib In GiasContext.Ribaltamento_AppezzamentixIndirizzi
                                                            Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                                rib.Reale_Piva = piva_Destinazione AndAlso
                                                                rib.Reale_Sa_Cod = sa_Cod_Destinazione AndAlso
                                                                rib.Reale_Appezza = appezza_Destinazione).ToList()
            For Each indirizzo In dummyRibaltamento_AppezzamentoxIndirizzi
                dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.Add((indirizzo.Budget_Piva, indirizzo.Budget_Sa_Cod, indirizzo.Budget_Appezza, indirizzo.Budget_Cod_Indirizzo), ((indirizzo.Reale_Piva, indirizzo.Reale_Sa_Cod, indirizzo.Reale_Appezza, indirizzo.Reale_Cod_Indirizzo), False))
            Next


            '-------------------------------
            '   REG_IMPIANTI
            '-------------------------------
            Dim dummyRibaltamento_Impianti = (From rib In GiasContext.Ribaltamento_Reg_Impianti
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                      rib.Reale_Piva = piva_Destinazione AndAlso
                                                      rib.Reale_Sa_Cod = sa_Cod_Destinazione AndAlso
                                                      rib.Reale_Appezza = appezza_Destinazione).ToList()
            For Each impianto In dummyRibaltamento_Impianti
                dicAnagrafica_Ribaltata.dicImpianti.Add((impianto.Budget_Piva, impianto.Budget_Sa_Cod, impianto.Budget_Appezza, impianto.Budget_Id_Reg), ((impianto.Reale_Piva, impianto.Reale_Sa_Cod, impianto.Reale_Appezza, impianto.Reale_Id_Reg), False))
            Next

            '-------------------------------
            '   IMPRESE_PROGETTI
            '-------------------------------
            Dim dummyRibaltamento_Esercizi = (From rib In GiasContext.Ribaltamento_Imprese_Progetti
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                  rib.Reale_Piva = piva_Destinazione AndAlso
                                                  rib.Reale_Sa_Cod = sa_Cod_Destinazione AndAlso
                                                  rib.Reale_Appezza = appezza_Destinazione).ToList()
            For Each esercizio In dummyRibaltamento_Esercizi
                dicAnagrafica_Ribaltata.dicEsercizi.Add((esercizio.Budget_Piva, esercizio.Budget_Sa_Cod, esercizio.Budget_Appezza, esercizio.Budget_Id_Reg, esercizio.Budget_Progetto_Cod), ((esercizio.Reale_Piva, esercizio.Reale_Sa_Cod, esercizio.Reale_Appezza, esercizio.Reale_Id_Reg, esercizio.Reale_Progetto_Cod), False))
            Next

        End If

        Return AppezzamentoDestinazioneEF

    End Function

    Private Shared Function EsistonoMovimenti(Appezzamento As Appezzamento,
                                              objParametri_Server As AgronicaCoreParametri,
                                              GiasContext As Gias_DeveloperServer_Entities) As String

        Dim agenda_list = (From a In GiasContext.Agenda
                           Join m In GiasContext.Movimenti
                               On a.PIVA Equals m.PIVA And a.Sa_Cod Equals m.Sa_Cod And a.Id_Agenda Equals m.Id_Agenda
                           Join md In GiasContext.Mov_Destinazioni
                               On md.Piva Equals m.PIVA And md.Sa_Cod Equals m.Sa_Cod And md.Id_Agenda Equals m.Id_Agenda And md.Id_Mov Equals m.Id_Mov
                           Where md.Piva = Appezzamento.PIVA AndAlso md.Sa_Cod = Appezzamento.SA_COD AndAlso md.Appezza = Appezzamento.APPEZZA).ToList()

        Dim ricette_list = (From rd In GiasContext.Ricette_Destinazioni
                            Join r In GiasContext.Ricette
                               On rd.Ricetta_Cod Equals r.Ricetta_Cod
                            Join ro In GiasContext.Ricette_Operazioni
                               On ro.Ricetta_Operazione_Cod Equals rd.Ricetta_Operazione_Cod
                            Where rd.Ricetta_SuperUser = objParametri_Server.PivaSuperUser AndAlso rd.Piva = Appezzamento.PIVA AndAlso rd.Sa_Cod = Appezzamento.SA_COD AndAlso rd.Appezza = Appezzamento.APPEZZA).ToList()

        Dim cdg_list = (From t In GiasContext.CDG_Testata
                        Join d In GiasContext.CDG_Dettagli
                            On t.Id_CDG Equals d.Id_CDG
                        Join a In GiasContext.Agenda
                            On a.Id_Agenda Equals t.Id_Agenda
                        Join m In GiasContext.Movimenti
                            On m.Id_Agenda Equals a.Id_Agenda
                        Where t.Piva = Appezzamento.PIVA AndAlso t.Sa_Cod = Appezzamento.SA_COD AndAlso d.Appezza = Appezzamento.APPEZZA).ToList()

        If agenda_list.Count > 0 OrElse ricette_list.Count > 0 OrElse cdg_list.Count > 0 Then
            Return "- " & Appezzamento.APP_NOME
        End If

        Return ""
    End Function

End Class

Public Class RibaltamentoAnagraficheColturali_W
    Public Shared chiavi_da_escludere As String() = {"id_budget", "appezza", "id_reg", "progetto_cod", "campo_cod", "id", "cod_indirizzo"}

    Public Shared Sub Ribalta_BudgetReale(Id_Budget As Integer,
                                          AppezzamentoOrigine As AgronicaCoreModelsSTD.anagrafiche.Appezzamento,
                                          dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                          ByRef ribaltati_con_successo As Integer,
                                          ByRef msg_gestito As String,
                                          isFirst As Boolean,
                                          isLast As Boolean,
                                          objParametri_Server As AgronicaCoreParametri,
                                          objParametri_Utenti As AgronicaCoreParametri,
                                          GiasContext As Gias_DeveloperServer_Entities)

        If isFirst Then
            Clean_Tabelle_Ribaltamento_Globale(GiasContext)
        End If

        Dim username As String = objParametri_Server.UsernameOperazione

        Dim piva_origine As String = AppezzamentoOrigine.primaryKey.centroAziendalePK.partitaIva
        Dim sa_cod_origine As Integer = AppezzamentoOrigine.primaryKey.centroAziendalePK.codice
        Dim appezza_Origine As Integer = AppezzamentoOrigine.primaryKey.codice
        Dim campo_cod_Origine As Integer = 0

        Dim AppezzamentoOrigineEF = (From app In GiasContext.Budget_Appezzamento
                                     Where app.Id_Budget = Id_Budget AndAlso
                                         app.PIVA = piva_origine AndAlso
                                         app.SA_COD = sa_cod_origine AndAlso
                                         app.APPEZZA = appezza_Origine).FirstOrDefault()

        Dim existsRibaltato As Boolean = False
        Dim msg As String = ""
        Dim AppezzamentoDestinazioneEF As Appezzamento = RibaltamentoAnagraficheColturali_R.Get_AppezzamentoRibaltato(Id_Budget, AppezzamentoOrigineEF,
                                                                                                                      existsRibaltato, dicAnagrafica_Ribaltata, msg_gestito,
                                                                                                                      objParametri_Server, GiasContext)
        If msg_gestito <> "" Then
            Exit Sub
        End If

        '-------------------------------
        '   CAMPO
        '-------------------------------
        If AppezzamentoOrigineEF.Campo_Cod <> 0 Then
            Gestione_Campo(Id_Budget, AppezzamentoOrigineEF, dicAnagrafica_Ribaltata, username, objParametri_Server, objParametri_Utenti, GiasContext)
        End If

        '-------------------------------
        '   APPEZZAMENTO
        '-------------------------------
        Gestione_Appezzamento(Id_Budget, AppezzamentoOrigineEF, AppezzamentoDestinazioneEF, Not (existsRibaltato), dicAnagrafica_Ribaltata, username, objParametri_Server, objParametri_Utenti, GiasContext)

        '-------------------------------
        '   APPEZZAMENTI X INDIRIZZI
        '-------------------------------
        Dim AppezzamentixIndirizziOrigine_List = (From ind In GiasContext.Budget_AppezzamentixIndirizzi
                                                  Where ind.Id_Budget = Id_Budget AndAlso
                                                          ind.PIVA = piva_origine AndAlso
                                                          ind.sa_cod = sa_cod_origine AndAlso
                                                          ind.appezza = appezza_Origine).ToList()

        For Each AppezzamentixIndirizziOrigineEF In AppezzamentixIndirizziOrigine_List
            Gestione_AppezzamentixIndirizzi(Id_Budget, AppezzamentoDestinazioneEF, AppezzamentixIndirizziOrigineEF, dicAnagrafica_Ribaltata, username, objParametri_Server, GiasContext)
        Next

        '-------------------------------
        '   REG_IMPIANTI
        '-------------------------------
        Dim ImpiantiOrigine_List = (From imp In GiasContext.Budget_Reg_Impianti
                                    Where imp.Id_Budget = Id_Budget AndAlso
                                        imp.PIVA = piva_origine AndAlso
                                        imp.SA_COD = sa_cod_origine AndAlso
                                        imp.APPEZZA = appezza_Origine).ToList()

        For Each ImpiantoOrigineEF In ImpiantiOrigine_List

            Dim ImpiantoDestinazioneEF = Gestione_Impianto(Id_Budget, AppezzamentoDestinazioneEF, ImpiantoOrigineEF, dicAnagrafica_Ribaltata, username, objParametri_Server, objParametri_Utenti, GiasContext)

            '-------------------------------
            '   IMPRESE_PROGETTI
            '-------------------------------
            Dim EserciziOrigine_List = (From ese In GiasContext.Budget_Imprese_Progetti
                                        Where ese.Id_Budget = Id_Budget AndAlso
                                            ese.Piva = piva_origine AndAlso
                                            ese.Sa_Cod = sa_cod_origine AndAlso
                                            ese.Appezza = appezza_Origine AndAlso
                                            ese.Id_Reg = ImpiantoOrigineEF.ID_REG).ToList()

            For Each EsercizioOrigineEF In EserciziOrigine_List
                Gestione_Esercizio(Id_Budget, ImpiantoDestinazioneEF, EsercizioOrigineEF, dicAnagrafica_Ribaltata, username, objParametri_Server, GiasContext)
            Next
        Next

        'PULISCO LE TABELLE RIBALTAMENTO
        Clean_Tabelle_da_Ribaltamento_Corrente(Id_Budget, dicAnagrafica_Ribaltata, isLast, objParametri_Server, GiasContext)

        ribaltati_con_successo += 1

    End Sub

    Private Shared Sub Gestione_Campo(Id_Budget As Integer,
                                      AppezzamentoOrigineEF As Budget_Appezzamento,
                                      ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                      username As String,
                                      objParametri_Server As AgronicaCoreParametri,
                                      objParametri_Utenti As AgronicaCoreParametri,
                                      GiasContext As Gias_DeveloperServer_Entities)

        Dim isNew As Boolean = True
        Dim doThings As Boolean = True

        Dim CampoOrigineEF As New Budget_Campi
        Dim CampoDestinazioneEF As New Campi
        Dim Ribaltamento_Campi As New Ribaltamento_Campi

        CampoOrigineEF = (From campo In GiasContext.Budget_Campi
                          Where campo.Id_Budget = Id_Budget AndAlso
                              campo.Piva = AppezzamentoOrigineEF.PIVA AndAlso
                              campo.Sa_Cod = AppezzamentoOrigineEF.SA_COD AndAlso
                              campo.Campo_Cod = AppezzamentoOrigineEF.Campo_Cod).FirstOrDefault()

        'Se l'elemento non esiste nel dictionary ne creo uno nuovo, altrimenti imposto la proprietà BOOLEAN (StillExisists) a TRUE
        If Not dicAnagrafica_Ribaltata.dicCampi.ContainsKey((AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, AppezzamentoOrigineEF.Campo_Cod)) Then

            CampoDestinazioneEF.Campo_Cod = EFCampi.CreateNuovo_Campo_Cod(AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, objParametri_Server, objParametri_Utenti)

        Else

            Dim campo = dicAnagrafica_Ribaltata.dicCampi.Item((AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, AppezzamentoOrigineEF.Campo_Cod))

            If campo.Item2 = True Then
                'Signfica che siamo già 'passati' per questo campo, salto tutti i passatti
                doThings = False
            Else
                'IMPOSTO KEEP ELEMENT --> TRUE
                campo.Item2 = True
                dicAnagrafica_Ribaltata.dicCampi.Item((AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, AppezzamentoOrigineEF.Campo_Cod)) = campo

                isNew = False

                CampoDestinazioneEF = (From c In GiasContext.Campi
                                       Where c.Piva = campo.Item1.Item1 AndAlso
                                           c.Sa_Cod = campo.Item1.Item2 AndAlso
                                           c.Campo_Cod = campo.Item1.Item3).FirstOrDefault()

                Ribaltamento_Campi = (From rib In GiasContext.Ribaltamento_Campi
                                      Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                          rib.Budget_Piva = CampoOrigineEF.Piva AndAlso
                                          rib.Budget_Sa_Cod = CampoOrigineEF.Sa_Cod AndAlso
                                          rib.Budget_Campo_Cod = CampoOrigineEF.Campo_Cod AndAlso
                                          rib.Reale_Piva = campo.Item1.Item1 AndAlso
                                          rib.Reale_Sa_Cod = campo.Item1.Item2 AndAlso
                                          rib.Reale_Campo_Cod = campo.Item1.Item3).FirstOrDefault()
            End If
        End If

        If doThings Then
            CampoDestinazioneEF = CampoOrigineEF.PropertyCopier(CampoDestinazioneEF, chiavi_da_escludere)
            CampoDestinazioneEF.RiempiCampi_StandardGias(username, isNew)

            If isNew Then
                GiasContext.Campi.Add(CampoDestinazioneEF)
            Else
                GiasContext.Campi.Attach(CampoDestinazioneEF)
                GiasContext.Entry(CampoDestinazioneEF).State = EntityState.Modified
            End If
            GiasContext.SaveChanges()


            '-------------------------------
            '   RIBALTAMENTO CAMPO
            '-------------------------------
            Ribaltamento_Campi.RiempiCampi_StandardGias(username, isNew)
            If isNew Then
                Ribaltamento_Campi.RiempiChiavi_Ribaltamento(CampoOrigineEF, CampoDestinazioneEF,
                                                                     Id_Budget, CampoDestinazioneEF.Piva, CampoDestinazioneEF.Sa_Cod,
                                                                     enum_TipoEntita_Des.Campi,
                                                                     username)

                GiasContext.Ribaltamento_Campi.Add(Ribaltamento_Campi)
            Else
                Ribaltamento_Campi.Data_Ribaltamento = DateTime.Now
                Ribaltamento_Campi.RiempiCampi_StandardGias(username, False)

                GiasContext.Ribaltamento_Campi.Attach(Ribaltamento_Campi)
                GiasContext.Entry(Ribaltamento_Campi).State = EntityState.Modified
            End If
            GiasContext.SaveChanges()


            '-------------------------------
            '   LOG CAMPO
            '-------------------------------
            Scrivi_Log(Id_Budget, CampoDestinazioneEF, enum_TipoEntita_Des.Campi, If(isNew, enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                       CampoDestinazioneEF.Piva, CampoDestinazioneEF.Sa_Cod, CampoDestinazioneEF.Campo_Cod, Nothing, Nothing,
                       objParametri_Server, GiasContext)


            '-------------------------------
            '   UTENTI X CAMPI
            '-------------------------------
            ScriviAnagraficaxUtenti(enum_TipoEntita_Des.Campi,
                                    CampoDestinazioneEF.Piva, CampoDestinazioneEF.Sa_Cod, 0, CampoDestinazioneEF.Campo_Cod,
                                    cancellaRiscrivi:=Not (isNew), username,
                                    objParametri_Server, GiasContext)


            '-------------------------------
            '   CAMPI CODICI
            '-------------------------------
            If Not isNew Then
                'CANCELLO PER RISCRIVERE
                Delete_Codici(enum_TipoEntita_Des.Campi, CampoDestinazioneEF.Piva, CampoDestinazioneEF.Sa_Cod, 0, 0, 0, CampoDestinazioneEF.Campo_Cod, GiasContext)
            End If
            Dim CampiCodiciOrigine_List = (From codici In GiasContext.Budget_Campi_Codici
                                           Where codici.Id_Budget = Id_Budget AndAlso
                                               codici.PIVA = CampoOrigineEF.Piva AndAlso
                                               codici.sa_cod = CampoOrigineEF.Sa_Cod AndAlso
                                               codici.campo_cod = CampoOrigineEF.Campo_Cod).ToList()
            For Each CodiceCampoOrigineEF In CampiCodiciOrigine_List
                Dim CodiceCampoDestinazioneEF As New Campi_Codici With {
                    .campo_cod = CampoDestinazioneEF.Campo_Cod
                }

                CodiceCampoDestinazioneEF = CodiceCampoOrigineEF.PropertyCopier(CodiceCampoDestinazioneEF, chiavi_da_escludere)
                CodiceCampoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.Campi_Codici.Add(CodiceCampoDestinazioneEF)
                GiasContext.SaveChanges()
            Next


            '-------------------------------
            '   CAMPI X PARTICELLE
            '-------------------------------
            If Not isNew Then
                'CANCELLO E RISCRIVO
                Dim CampiParticelle_List = (From particelle In GiasContext.CampiXParticelle
                                            Where particelle.PIVA = CampoDestinazioneEF.Piva AndAlso
                                                   particelle.SA_COD = CampoDestinazioneEF.Sa_Cod AndAlso
                                                   particelle.CAMPO_COD = CampoDestinazioneEF.Campo_Cod).ToList()
                GiasContext.CampiXParticelle.RemoveRange(CampiParticelle_List)
                GiasContext.SaveChanges()
            End If
            Dim CampoParticelleOrigine_List = (From particelle In GiasContext.Budget_CampiXParticelle
                                               Where particelle.Id_Budget = Id_Budget AndAlso
                                                      particelle.PIVA = CampoOrigineEF.Piva AndAlso
                                                      particelle.SA_COD = CampoOrigineEF.Sa_Cod AndAlso
                                                      particelle.CAMPO_COD = CampoOrigineEF.Campo_Cod).ToList()
            For Each ParticellaCampoOrigineEF In CampoParticelleOrigine_List
                Dim ParticellaCampoDestinazioneEF As New CampiXParticelle With {
                    .CAMPO_COD = CampoDestinazioneEF.Campo_Cod
                }

                ParticellaCampoDestinazioneEF = ParticellaCampoOrigineEF.PropertyCopier(ParticellaCampoDestinazioneEF, chiavi_da_escludere)
                ParticellaCampoDestinazioneEF.RiempiCampi_StandardGias(username)

                GiasContext.CampiXParticelle.Add(ParticellaCampoDestinazioneEF)
                GiasContext.SaveChanges()
            Next


            If Not dicAnagrafica_Ribaltata.dicCampi.ContainsKey((AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, AppezzamentoOrigineEF.Campo_Cod)) Then
                dicAnagrafica_Ribaltata.dicCampi.Add((Ribaltamento_Campi.Budget_Piva, Ribaltamento_Campi.Budget_Sa_Cod, Ribaltamento_Campi.Budget_Campo_Cod), ((Ribaltamento_Campi.Reale_Piva, Ribaltamento_Campi.Reale_Sa_Cod, Ribaltamento_Campi.Reale_Campo_Cod), True))
            End If
        End If

    End Sub

    Private Shared Sub Gestione_Appezzamento(Id_Budget As Integer,
                                             AppezzamentoOrigineEF As Budget_Appezzamento,
                                             AppezzamentoDestinazioneEF As Appezzamento,
                                             isNew As Boolean,
                                             ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                             username As String,
                                             objParametri_Server As AgronicaCoreParametri,
                                             objParametri_Utenti As AgronicaCoreParametri,
                                             GiasContext As Gias_DeveloperServer_Entities)

        Dim Ribaltamento_Appezzamento As New Ribaltamento_Appezzamento

        AppezzamentoDestinazioneEF = AppezzamentoOrigineEF.PropertyCopier(AppezzamentoDestinazioneEF, chiavi_da_escludere)

        If isNew Then
            AppezzamentoDestinazioneEF.APPEZZA = EFAppezzamento.NuovoAppezzamento_Cod(AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD, objParametri_Server, objParametri_Utenti)
        Else
            Ribaltamento_Appezzamento = (From rib In GiasContext.Ribaltamento_Appezzamento
                                         Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                             rib.Budget_Piva = AppezzamentoOrigineEF.PIVA AndAlso
                                             rib.Budget_Sa_Cod = AppezzamentoOrigineEF.SA_COD AndAlso
                                             rib.Budget_Appezza = AppezzamentoOrigineEF.APPEZZA AndAlso
                                             rib.Reale_Piva = AppezzamentoDestinazioneEF.PIVA AndAlso
                                             rib.Reale_Sa_Cod = AppezzamentoDestinazioneEF.SA_COD AndAlso
                                             rib.Reale_Appezza = AppezzamentoDestinazioneEF.APPEZZA).FirstOrDefault()
        End If

        If AppezzamentoOrigineEF.Campo_Cod <> 0 Then
            Dim dummyCampo = dicAnagrafica_Ribaltata.dicCampi((AppezzamentoOrigineEF.PIVA, AppezzamentoOrigineEF.SA_COD, AppezzamentoOrigineEF.Campo_Cod))
            AppezzamentoDestinazioneEF.Campo_Cod = dummyCampo.Item1.Item3
        Else
            AppezzamentoDestinazioneEF.Campo_Cod = 0
        End If

        AppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username, isNew)

        If isNew Then
            GiasContext.Appezzamento.Add(AppezzamentoDestinazioneEF)
        Else
            GiasContext.Appezzamento.Attach(AppezzamentoDestinazioneEF)
            GiasContext.Entry(AppezzamentoDestinazioneEF).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '   RIBALTAMENTO APPEZZAMENTO
        '-------------------------------
        If isNew Then
            Ribaltamento_Appezzamento.RiempiChiavi_Ribaltamento(AppezzamentoOrigineEF, AppezzamentoDestinazioneEF,
                                                                Id_Budget, AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD,
                                                                enum_TipoEntita_Des.Appezza,
                                                                username)

            GiasContext.Ribaltamento_Appezzamento.Add(Ribaltamento_Appezzamento)
        Else
            Ribaltamento_Appezzamento.Data_Ribaltamento = DateTime.Now
            Ribaltamento_Appezzamento.RiempiCampi_StandardGias(username, False)

            GiasContext.Ribaltamento_Appezzamento.Attach(Ribaltamento_Appezzamento)
            GiasContext.Entry(Ribaltamento_Appezzamento).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '   LOG APPEZZAMENTO
        '-------------------------------
        Scrivi_Log(Id_Budget, AppezzamentoDestinazioneEF, enum_TipoEntita_Des.Appezza, If(isNew, enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                   AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD, AppezzamentoDestinazioneEF.APPEZZA, Nothing, Nothing,
                   objParametri_Server, GiasContext)


        '-------------------------------
        '   UTENTI X APPEZZAMENTI
        '-------------------------------
        ScriviAnagraficaxUtenti(enum_TipoEntita_Des.Appezza,
                                AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD, AppezzamentoDestinazioneEF.APPEZZA, 0,
                                cancellaRiscrivi:=Not (isNew), username,
                                objParametri_Server, GiasContext)


        '-------------------------------
        '   APPEZZAMENTI CODICI
        '-------------------------------
        If Not isNew Then
            'CANCELLO PER RISCRIVERE
            Delete_Codici(enum_TipoEntita_Des.Appezza, AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD, AppezzamentoDestinazioneEF.APPEZZA, 0, 0, 0, GiasContext)
        End If
        Dim AppezzamentiCodiciOrigine_List = (From codici In GiasContext.Budget_Appezzamento_Codici
                                              Where codici.Id_Budget = Id_Budget AndAlso
                                                  codici.PIVA = AppezzamentoOrigineEF.PIVA AndAlso
                                                  codici.sa_cod = AppezzamentoOrigineEF.SA_COD AndAlso
                                                  codici.appezza = AppezzamentoOrigineEF.APPEZZA).ToList()
        For Each CodiceAppezzamentoOrigineEF In AppezzamentiCodiciOrigine_List
            Dim CodiceAppezzamentoDestinazioneEF As New Appezzamento_Codici With {
                .appezza = AppezzamentoDestinazioneEF.APPEZZA
            }

            CodiceAppezzamentoDestinazioneEF = CodiceAppezzamentoOrigineEF.PropertyCopier(CodiceAppezzamentoDestinazioneEF, chiavi_da_escludere)
            CodiceAppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.Appezzamento_Codici.Add(CodiceAppezzamentoDestinazioneEF)
            GiasContext.SaveChanges()
        Next


        '-------------------------------
        '   APPEZZAMENTI X PARTICELLE
        '-------------------------------
        If Not isNew Then
            'CANCELLO E RISCRIVO
            Dim AppezzamentoParticelle_List = (From particelle In GiasContext.AppezzamentiXParticelle
                                               Where particelle.PIVA = AppezzamentoDestinazioneEF.PIVA AndAlso
                                                   particelle.SA_COD = AppezzamentoDestinazioneEF.SA_COD AndAlso
                                                   particelle.APPEZZA = AppezzamentoDestinazioneEF.APPEZZA).ToList()
            GiasContext.AppezzamentiXParticelle.RemoveRange(AppezzamentoParticelle_List)
            GiasContext.SaveChanges()
        End If
        Dim AppezzamentoParticelleOrigine_List = (From particelle In GiasContext.Budget_AppezzamentiXParticelle
                                                  Where particelle.Id_Budget = Id_Budget AndAlso
                                                      particelle.PIVA = AppezzamentoOrigineEF.PIVA AndAlso
                                                      particelle.SA_COD = AppezzamentoOrigineEF.SA_COD AndAlso
                                                      particelle.APPEZZA = AppezzamentoOrigineEF.APPEZZA).ToList()
        For Each ParticellaAppezzamentoOrigineEF In AppezzamentoParticelleOrigine_List
            Dim ParticellaAppezzamentoDestinazioneEF As New AppezzamentiXParticelle With {
                .APPEZZA = AppezzamentoDestinazioneEF.APPEZZA
            }

            ParticellaAppezzamentoDestinazioneEF = ParticellaAppezzamentoOrigineEF.PropertyCopier(ParticellaAppezzamentoDestinazioneEF, chiavi_da_escludere)
            ParticellaAppezzamentoDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.AppezzamentiXParticelle.Add(ParticellaAppezzamentoDestinazioneEF)
            GiasContext.SaveChanges()
        Next

    End Sub

    Private Shared Sub Gestione_AppezzamentixIndirizzi(Id_Budget As Integer,
                                                       AppezzamentoDestinazioneEF As Appezzamento,
                                                       AppezzamentixIndirizziOrigineEF As Budget_AppezzamentixIndirizzi,
                                                       ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                                       username As String,
                                                       objParametri_Server As AgronicaCoreParametri,
                                                       GiasContext As Gias_DeveloperServer_Entities)

        Dim isNew As Boolean = True

        Dim Ribaltamento_AppezzamentixIndirizzi As New Ribaltamento_AppezzamentixIndirizzi
        Dim AppezzamentixIndirizziDestinazioneEF As New AppezzamentixIndirizzi With {
            .appezza = AppezzamentoDestinazioneEF.APPEZZA
        }

        'Se l'elemento non esiste nel dictionary ne creo uno nuovo, altrimenti imposto la proprietà BOOLEAN (StillExisists) a TRUE
        If Not dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.ContainsKey((AppezzamentixIndirizziOrigineEF.PIVA, AppezzamentixIndirizziOrigineEF.sa_cod, AppezzamentixIndirizziOrigineEF.appezza, AppezzamentixIndirizziOrigineEF.cod_indirizzo)) Then

            Dim idGen As New Agro_Sequenze
            AppezzamentixIndirizziDestinazioneEF.cod_indirizzo = idGen.NuovoId_Tabella_EF(GiasContext, "Indirizzi", 0, 2000000, objParametri_Server)

        Else

            Dim appxind = dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.Item((AppezzamentixIndirizziOrigineEF.PIVA, AppezzamentixIndirizziOrigineEF.sa_cod, AppezzamentixIndirizziOrigineEF.appezza, AppezzamentixIndirizziOrigineEF.cod_indirizzo))

            'IMPOSTO KEEP ELEMENT --> TRUE
            appxind.Item2 = True
            dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.Item((AppezzamentixIndirizziOrigineEF.PIVA, AppezzamentixIndirizziOrigineEF.sa_cod, AppezzamentixIndirizziOrigineEF.appezza, AppezzamentixIndirizziOrigineEF.cod_indirizzo)) = appxind

            isNew = False

            AppezzamentixIndirizziDestinazioneEF = (From c In GiasContext.AppezzamentixIndirizzi
                                                    Where c.PIVA = appxind.Item1.Item1 AndAlso
                                                        c.sa_cod = appxind.Item1.Item2 AndAlso
                                                        c.appezza = appxind.Item1.Item3 AndAlso
                                                        c.cod_indirizzo = appxind.Item1.Item4).FirstOrDefault()

            Ribaltamento_AppezzamentixIndirizzi = (From rib In GiasContext.Ribaltamento_AppezzamentixIndirizzi
                                                   Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                       rib.Budget_Piva = AppezzamentixIndirizziOrigineEF.PIVA AndAlso
                                                       rib.Budget_Sa_Cod = AppezzamentixIndirizziOrigineEF.sa_cod AndAlso
                                                       rib.Budget_Appezza = AppezzamentixIndirizziOrigineEF.appezza AndAlso
                                                       rib.Budget_Cod_Indirizzo = AppezzamentixIndirizziOrigineEF.cod_indirizzo AndAlso
                                                       rib.Reale_Piva = appxind.Item1.Item1 AndAlso
                                                       rib.Reale_Sa_Cod = appxind.Item1.Item2 AndAlso
                                                       rib.Reale_Appezza = appxind.Item1.Item3 AndAlso
                                                       rib.Reale_Cod_Indirizzo = appxind.Item1.Item4).FirstOrDefault()
        End If

        AppezzamentixIndirizziDestinazioneEF = AppezzamentixIndirizziOrigineEF.PropertyCopier(AppezzamentixIndirizziDestinazioneEF, chiavi_da_escludere)
        AppezzamentixIndirizziDestinazioneEF.RiempiCampi_StandardGias(username)

        If isNew Then
            GiasContext.AppezzamentixIndirizzi.Add(AppezzamentixIndirizziDestinazioneEF)
        Else
            GiasContext.AppezzamentixIndirizzi.Attach(AppezzamentixIndirizziDestinazioneEF)
            GiasContext.Entry(AppezzamentixIndirizziDestinazioneEF).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '   RIBALTAMENTO APPEZZAMENTI X INDIRIZZI
        '-------------------------------
        If isNew Then
            Ribaltamento_AppezzamentixIndirizzi.RiempiChiavi_Ribaltamento(AppezzamentixIndirizziOrigineEF, AppezzamentixIndirizziDestinazioneEF,
                                                                          Id_Budget, AppezzamentixIndirizziDestinazioneEF.PIVA, AppezzamentixIndirizziDestinazioneEF.sa_cod,
                                                                          enum_TipoEntita_Des.AppezzamentiXIndirizzi,
                                                                          username)

            GiasContext.Ribaltamento_AppezzamentixIndirizzi.Add(Ribaltamento_AppezzamentixIndirizzi)
        Else
            Ribaltamento_AppezzamentixIndirizzi.Data_Ribaltamento = DateTime.Now
            Ribaltamento_AppezzamentixIndirizzi.RiempiCampi_StandardGias(username, False)

            GiasContext.Ribaltamento_AppezzamentixIndirizzi.Attach(Ribaltamento_AppezzamentixIndirizzi)
            GiasContext.Entry(Ribaltamento_AppezzamentixIndirizzi).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '    INDIRIZZO
        '-------------------------------
        Dim IndirizziDestinazioneEF As New Indirizzi
        Dim IndirizzoOrigineEF = (From ind In GiasContext.Indirizzi
                                  Where ind.cod_indirizzo = Ribaltamento_AppezzamentixIndirizzi.Budget_Cod_Indirizzo).FirstOrDefault()
        If isNew Then
            IndirizziDestinazioneEF.cod_indirizzo = Ribaltamento_AppezzamentixIndirizzi.Reale_Cod_Indirizzo
        Else
            IndirizziDestinazioneEF = (From ind In GiasContext.Indirizzi
                                       Where ind.cod_indirizzo = Ribaltamento_AppezzamentixIndirizzi.Reale_Cod_Indirizzo).FirstOrDefault()
        End If

        IndirizziDestinazioneEF = IndirizzoOrigineEF.PropertyCopier(IndirizziDestinazioneEF, chiavi_da_escludere)
        IndirizziDestinazioneEF.RiempiCampi_StandardGias(username, isNew)

        If isNew Then
            GiasContext.Indirizzi.Add(IndirizziDestinazioneEF)
        Else
            IndirizziDestinazioneEF.RiempiCampi_StandardGias(username, False)

            GiasContext.Indirizzi.Attach(IndirizziDestinazioneEF)
            GiasContext.Entry(IndirizziDestinazioneEF).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        If Not dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.ContainsKey((AppezzamentixIndirizziOrigineEF.PIVA, AppezzamentixIndirizziOrigineEF.sa_cod, AppezzamentixIndirizziOrigineEF.appezza, AppezzamentixIndirizziOrigineEF.cod_indirizzo)) Then
            dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.Add((AppezzamentixIndirizziOrigineEF.PIVA, AppezzamentixIndirizziOrigineEF.sa_cod, AppezzamentixIndirizziOrigineEF.appezza, AppezzamentixIndirizziOrigineEF.cod_indirizzo), ((AppezzamentixIndirizziDestinazioneEF.PIVA, AppezzamentixIndirizziDestinazioneEF.sa_cod, AppezzamentixIndirizziDestinazioneEF.appezza, AppezzamentixIndirizziDestinazioneEF.cod_indirizzo), True))
        End If

    End Sub

    Private Shared Function Gestione_Impianto(Id_Budget As Integer,
                                              AppezzamentoDestinazioneEF As Appezzamento,
                                              ImpiantoOrigineEF As Budget_Reg_Impianti,
                                              ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                              username As String,
                                              objParametri_Server As AgronicaCoreParametri,
                                              objParametri_Utenti As AgronicaCoreParametri,
                                              GiasContext As Gias_DeveloperServer_Entities
                                              ) As Reg_Impianti

        Dim isNew As Boolean = True

        Dim Ribaltamento_Reg_Impianti As New Ribaltamento_Reg_Impianti
        Dim ImpiantoDestinazioneEF As New Reg_Impianti With {
            .APPEZZA = AppezzamentoDestinazioneEF.APPEZZA
        }

        'Se l'elemento non esiste nel dictionary ne creo uno nuovo, altrimenti imposto la proprietà BOOLEAN (StillExisists) a TRUE
        If Not dicAnagrafica_Ribaltata.dicImpianti.ContainsKey((ImpiantoOrigineEF.PIVA, ImpiantoOrigineEF.SA_COD, ImpiantoOrigineEF.APPEZZA, ImpiantoOrigineEF.ID_REG)) Then

            ImpiantoDestinazioneEF.ID_REG = EFReg_Impianti.NuovoImpianto_Cod(AppezzamentoDestinazioneEF.PIVA, AppezzamentoDestinazioneEF.SA_COD, AppezzamentoDestinazioneEF.APPEZZA, objParametri_Server, objParametri_Utenti)

        Else

            Dim impianto = dicAnagrafica_Ribaltata.dicImpianti.Item((ImpiantoOrigineEF.PIVA, ImpiantoOrigineEF.SA_COD, ImpiantoOrigineEF.APPEZZA, ImpiantoOrigineEF.ID_REG))

            'IMPOSTO KEEP ELEMENT --> TRUE
            impianto.Item2 = True
            dicAnagrafica_Ribaltata.dicImpianti.Item((ImpiantoOrigineEF.PIVA, ImpiantoOrigineEF.SA_COD, ImpiantoOrigineEF.APPEZZA, ImpiantoOrigineEF.ID_REG)) = impianto

            isNew = False

            ImpiantoDestinazioneEF = (From c In GiasContext.Reg_Impianti
                                      Where c.PIVA = impianto.Item1.Item1 AndAlso
                                          c.SA_COD = impianto.Item1.Item2 AndAlso
                                          c.APPEZZA = impianto.Item1.Item3 AndAlso
                                          c.ID_REG = impianto.Item1.Item4).FirstOrDefault()

            Ribaltamento_Reg_Impianti = (From rib In GiasContext.Ribaltamento_Reg_Impianti
                                         Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                             rib.Budget_Piva = ImpiantoOrigineEF.PIVA AndAlso
                                             rib.Budget_Sa_Cod = ImpiantoOrigineEF.SA_COD AndAlso
                                             rib.Budget_Appezza = ImpiantoOrigineEF.APPEZZA AndAlso
                                             rib.Budget_Id_Reg = ImpiantoOrigineEF.ID_REG AndAlso
                                             rib.Reale_Piva = impianto.Item1.Item1 AndAlso
                                             rib.Reale_Sa_Cod = impianto.Item1.Item2 AndAlso
                                             rib.Reale_Appezza = impianto.Item1.Item3 AndAlso
                                             rib.Reale_Id_Reg = impianto.Item1.Item4).FirstOrDefault()
        End If

        ImpiantoDestinazioneEF = ImpiantoOrigineEF.PropertyCopier(ImpiantoDestinazioneEF, chiavi_da_escludere)
        ImpiantoDestinazioneEF.RiempiCampi_StandardGias(username)

        If isNew Then
            GiasContext.Reg_Impianti.Add(ImpiantoDestinazioneEF)
        Else
            GiasContext.Reg_Impianti.Attach(ImpiantoDestinazioneEF)
            GiasContext.Entry(ImpiantoDestinazioneEF).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '   RIBALTAMENTO IMPIANTO
        '-------------------------------
        If isNew Then
            Ribaltamento_Reg_Impianti.RiempiChiavi_Ribaltamento(ImpiantoOrigineEF, ImpiantoDestinazioneEF,
                                                                Id_Budget, ImpiantoDestinazioneEF.PIVA, ImpiantoDestinazioneEF.SA_COD,
                                                                enum_TipoEntita_Des.Impianti,
                                                                username)

            GiasContext.Ribaltamento_Reg_Impianti.Add(Ribaltamento_Reg_Impianti)
        Else
            Ribaltamento_Reg_Impianti.Data_Ribaltamento = DateTime.Now
            Ribaltamento_Reg_Impianti.RiempiCampi_StandardGias(username, False)

            GiasContext.Ribaltamento_Reg_Impianti.Attach(Ribaltamento_Reg_Impianti)
            GiasContext.Entry(Ribaltamento_Reg_Impianti).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()


        '-------------------------------
        '   LOG IMPIANTO
        '-------------------------------
        Scrivi_Log(Id_Budget, ImpiantoDestinazioneEF, enum_TipoEntita_Des.Impianti, If(isNew, enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                   ImpiantoDestinazioneEF.PIVA, ImpiantoDestinazioneEF.SA_COD, ImpiantoDestinazioneEF.APPEZZA, ImpiantoDestinazioneEF.ID_REG, Nothing,
                   objParametri_Server, GiasContext)


        '-------------------------------
        '   IMPIANTI CODICI
        '-------------------------------
        If Not isNew Then
            'CANCELLO PER RISCRIVERE
            Delete_Codici(enum_TipoEntita_Des.Impianti, ImpiantoDestinazioneEF.PIVA, ImpiantoDestinazioneEF.SA_COD, ImpiantoDestinazioneEF.APPEZZA, ImpiantoDestinazioneEF.ID_REG, 0, 0, GiasContext)
        End If
        Dim ImpiantiCodiciOrigine_List = (From codici In GiasContext.Budget_Reg_Impianti_Codici
                                          Where codici.Id_Budget = Id_Budget AndAlso
                                              codici.PIVA = ImpiantoOrigineEF.PIVA AndAlso
                                              codici.sa_cod = ImpiantoOrigineEF.SA_COD AndAlso
                                              codici.appezza = ImpiantoOrigineEF.APPEZZA AndAlso
                                              codici.Id_Reg = ImpiantoOrigineEF.ID_REG AndAlso
                                              codici.Progetto_Cod = 0).ToList()
        For Each CodiceImpiantoOrigineEF In ImpiantiCodiciOrigine_List
            Dim CodiceImpiantoDestinazioneEF As New Reg_Impianti_Codici With {
                .appezza = ImpiantoDestinazioneEF.APPEZZA,
                .Id_Reg = ImpiantoDestinazioneEF.ID_REG,
                .Progetto_Cod = 0
            }

            CodiceImpiantoDestinazioneEF = CodiceImpiantoOrigineEF.PropertyCopier(CodiceImpiantoDestinazioneEF, chiavi_da_escludere)
            CodiceImpiantoDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.Reg_Impianti_Codici.Add(CodiceImpiantoDestinazioneEF)
            GiasContext.SaveChanges()
        Next

        If Not dicAnagrafica_Ribaltata.dicImpianti.ContainsKey((ImpiantoOrigineEF.PIVA, ImpiantoOrigineEF.SA_COD, ImpiantoOrigineEF.APPEZZA, ImpiantoOrigineEF.ID_REG)) Then
            dicAnagrafica_Ribaltata.dicImpianti.Add((ImpiantoOrigineEF.PIVA, ImpiantoOrigineEF.SA_COD, ImpiantoOrigineEF.APPEZZA, ImpiantoOrigineEF.ID_REG), ((ImpiantoDestinazioneEF.PIVA, ImpiantoDestinazioneEF.SA_COD, ImpiantoDestinazioneEF.APPEZZA, ImpiantoDestinazioneEF.ID_REG), True))
        End If

        Return ImpiantoDestinazioneEF
    End Function

    Private Shared Sub Gestione_Esercizio(Id_Budget As Integer,
                                          ImpiantoDestinazioneEF As Reg_Impianti,
                                          EsercizioOrigineEF As Budget_Imprese_Progetti,
                                          ByRef dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                          username As String,
                                          objParametri_Server As AgronicaCoreParametri,
                                          GiasContext As Gias_DeveloperServer_Entities)

        Dim isNew As Boolean = True

        Dim Ribaltamento_Imprese_Progetti As New Ribaltamento_Imprese_Progetti
        Dim EsercizioDestinazioneEF As New Imprese_Progetti With {
            .Appezza = ImpiantoDestinazioneEF.APPEZZA,
            .Id_Reg = ImpiantoDestinazioneEF.ID_REG
        }

        'Se l'elemento non esiste nel dictionary ne creo uno nuovo, altrimenti imposto la proprietà BOOLEAN (StillExisists) a TRUE
        If Not dicAnagrafica_Ribaltata.dicEsercizi.ContainsKey((EsercizioOrigineEF.Piva, EsercizioOrigineEF.Sa_Cod, EsercizioOrigineEF.Appezza, EsercizioOrigineEF.Id_Reg, EsercizioOrigineEF.Progetto_Cod)) Then

            EsercizioDestinazioneEF.Progetto_Cod = EFEsercizi.NuovoProgetto_Cod(GiasContext, objParametri_Server)

        Else
            Dim esercizio = dicAnagrafica_Ribaltata.dicEsercizi.Item((EsercizioOrigineEF.Piva, EsercizioOrigineEF.Sa_Cod, EsercizioOrigineEF.Appezza, EsercizioOrigineEF.Id_Reg, EsercizioOrigineEF.Progetto_Cod))

            'IMPOSTO KEEP ELEMENT --> TRUE
            esercizio.Item2 = True
            dicAnagrafica_Ribaltata.dicEsercizi.Item((EsercizioOrigineEF.Piva, EsercizioOrigineEF.Sa_Cod, EsercizioOrigineEF.Appezza, EsercizioOrigineEF.Id_Reg, EsercizioOrigineEF.Progetto_Cod)) = esercizio

            isNew = False

            EsercizioDestinazioneEF = (From c In GiasContext.Imprese_Progetti
                                       Where c.Piva = esercizio.Item1.Item1 AndAlso
                                           c.Sa_Cod = esercizio.Item1.Item2 AndAlso
                                           c.Appezza = esercizio.Item1.Item3 AndAlso
                                           c.Id_Reg = esercizio.Item1.Item4 AndAlso
                                           c.Progetto_Cod = esercizio.Item1.Item5).FirstOrDefault()

            Ribaltamento_Imprese_Progetti = (From rib In GiasContext.Ribaltamento_Imprese_Progetti
                                             Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                 rib.Budget_Piva = EsercizioOrigineEF.Piva AndAlso
                                                 rib.Budget_Sa_Cod = EsercizioOrigineEF.Sa_Cod AndAlso
                                                 rib.Budget_Appezza = EsercizioOrigineEF.Appezza AndAlso
                                                 rib.Budget_Id_Reg = EsercizioOrigineEF.Id_Reg AndAlso
                                                 rib.Budget_Progetto_Cod = EsercizioOrigineEF.Progetto_Cod AndAlso
                                                 rib.Reale_Piva = esercizio.Item1.Item1 AndAlso
                                                 rib.Reale_Sa_Cod = esercizio.Item1.Item2 AndAlso
                                                 rib.Reale_Appezza = esercizio.Item1.Item3 AndAlso
                                                 rib.Reale_Id_Reg = esercizio.Item1.Item4 AndAlso
                                                 rib.Reale_Progetto_Cod = esercizio.Item1.Item5).FirstOrDefault()
        End If

        EsercizioDestinazioneEF = EsercizioOrigineEF.PropertyCopier(EsercizioDestinazioneEF, chiavi_da_escludere)
        EsercizioDestinazioneEF.RiempiCampi_StandardGias(username)

        If isNew Then
            GiasContext.Imprese_Progetti.Add(EsercizioDestinazioneEF)
        Else
            GiasContext.Imprese_Progetti.Attach(EsercizioDestinazioneEF)
            GiasContext.Entry(EsercizioDestinazioneEF).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()

        '-------------------------------
        '   RIBALTAMENTO IMPIANTO
        '-------------------------------
        If isNew Then
            Ribaltamento_Imprese_Progetti.RiempiChiavi_Ribaltamento(EsercizioOrigineEF, EsercizioDestinazioneEF,
                                                                    Id_Budget, EsercizioDestinazioneEF.Piva, EsercizioDestinazioneEF.Sa_Cod,
                                                                    enum_TipoEntita_Des.Progetti,
                                                                    username)

            GiasContext.Ribaltamento_Imprese_Progetti.Add(Ribaltamento_Imprese_Progetti)
        Else
            Ribaltamento_Imprese_Progetti.Data_Ribaltamento = DateTime.Now
            Ribaltamento_Imprese_Progetti.RiempiCampi_StandardGias(username, False)

            GiasContext.Ribaltamento_Imprese_Progetti.Attach(Ribaltamento_Imprese_Progetti)
            GiasContext.Entry(Ribaltamento_Imprese_Progetti).State = EntityState.Modified
        End If
        GiasContext.SaveChanges()

        '-------------------------------
        '   LOG IMPIANTO  If(isNew, enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
        '-------------------------------
        Scrivi_Log(Id_Budget, EsercizioDestinazioneEF, enum_TipoEntita_Des.Progetti, If(isNew, enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                   EsercizioDestinazioneEF.Piva, EsercizioDestinazioneEF.Sa_Cod, EsercizioDestinazioneEF.Appezza, EsercizioDestinazioneEF.Id_Reg, EsercizioDestinazioneEF.Progetto_Cod,
                   objParametri_Server, GiasContext)


        '-------------------------------
        '   IMPIANTI CODICI
        '-------------------------------
        If Not isNew Then
            'CANCELLO PER RISCRIVERE
            Delete_Codici(enum_TipoEntita_Des.Progetti, EsercizioDestinazioneEF.Piva, EsercizioDestinazioneEF.Sa_Cod, EsercizioDestinazioneEF.Appezza, EsercizioDestinazioneEF.Id_Reg, EsercizioDestinazioneEF.Progetto_Cod, 0, GiasContext)
        End If

        Dim EserciziCodiciOrigine_List = (From codici In GiasContext.Budget_Reg_Impianti_Codici
                                          Where codici.Id_Budget = Id_Budget AndAlso
                                                          codici.PIVA = EsercizioOrigineEF.Piva AndAlso
                                                          codici.sa_cod = EsercizioOrigineEF.Sa_Cod AndAlso
                                                          codici.appezza = EsercizioOrigineEF.Appezza AndAlso
                                                          codici.Id_Reg = EsercizioOrigineEF.Id_Reg AndAlso
                                                          codici.Progetto_Cod = EsercizioOrigineEF.Progetto_Cod).ToList()
        For Each CodiceEsercizioOrigineEF In EserciziCodiciOrigine_List
            Dim CodiceEsercizioDestinazioneEF As New Reg_Impianti_Codici With {
                .appezza = EsercizioDestinazioneEF.Appezza,
                .Id_Reg = EsercizioDestinazioneEF.Id_Reg,
                .Progetto_Cod = EsercizioDestinazioneEF.Progetto_Cod
            }

            CodiceEsercizioDestinazioneEF = CodiceEsercizioOrigineEF.PropertyCopier(CodiceEsercizioDestinazioneEF, chiavi_da_escludere)
            CodiceEsercizioDestinazioneEF.RiempiCampi_StandardGias(username)

            GiasContext.Reg_Impianti_Codici.Add(CodiceEsercizioDestinazioneEF)
            GiasContext.SaveChanges()
        Next

        If Not dicAnagrafica_Ribaltata.dicEsercizi.ContainsKey((EsercizioOrigineEF.Piva, EsercizioOrigineEF.Sa_Cod, EsercizioOrigineEF.Appezza, EsercizioOrigineEF.Id_Reg, EsercizioOrigineEF.Progetto_Cod)) Then
            dicAnagrafica_Ribaltata.dicEsercizi.Add((EsercizioOrigineEF.Piva, EsercizioOrigineEF.Sa_Cod, EsercizioOrigineEF.Appezza, EsercizioOrigineEF.Id_Reg, EsercizioOrigineEF.Progetto_Cod), ((EsercizioDestinazioneEF.Piva, EsercizioDestinazioneEF.Sa_Cod, EsercizioDestinazioneEF.Appezza, EsercizioDestinazioneEF.Id_Reg, EsercizioDestinazioneEF.Progetto_Cod), True))
        End If

    End Sub

    Private Shared Sub ScriviAnagraficaxUtenti(tipoAnagrafica As String,
                                               piva As String,
                                               sa_cod As Integer,
                                               appezza As Integer,
                                               campo_cod As Integer,
                                               cancellaRiscrivi As Boolean,
                                               username As String,
                                               objParametri_Server As AgronicaCoreParametri,
                                               GiasContext As Gias_DeveloperServer_Entities)

        Select Case tipoAnagrafica
            Case enum_TipoEntita_Des.Campi
                If cancellaRiscrivi Then
                    Dim listUtentixCampi = (From u In GiasContext.UtentiXCampi
                                            Where u.PIVA = piva AndAlso
                                                u.SA_COD = sa_cod AndAlso
                                                u.Campo_Cod = campo_cod).ToList()
                    If listUtentixCampi.Count > 0 Then
                        GiasContext.UtentiXCampi.RemoveRange(listUtentixCampi)
                        GiasContext.SaveChanges()
                    End If
                End If

                Dim UtentiXCampi As UtentiXCampi = EFCampi.Create_UtentiXCampi(piva, sa_cod, campo_cod, username, objParametri_Server)
                GiasContext.UtentiXCampi.Add(UtentiXCampi)
                GiasContext.SaveChanges()

            Case enum_TipoEntita_Des.Appezza
                If cancellaRiscrivi Then
                    Dim listUtentiXAppezzamenti = (From u In GiasContext.UtentiXAppezzamenti
                                                   Where u.PIVA = piva AndAlso
                                                       u.SA_COD = sa_cod AndAlso
                                                       u.Appezza = appezza).ToList()

                    If listUtentiXAppezzamenti.Count > 0 Then
                        GiasContext.UtentiXAppezzamenti.RemoveRange(listUtentiXAppezzamenti)
                        GiasContext.SaveChanges()
                    End If
                End If

                Dim UtentiXAppezzamenti As UtentiXAppezzamenti = EFAppezzamento.Create_UtentiXAppezzamenti(piva, sa_cod, appezza, username, objParametri_Server)
                GiasContext.UtentiXAppezzamenti.Add(UtentiXAppezzamenti)
                GiasContext.SaveChanges()
        End Select
    End Sub



#Region "Utility"
    Private Shared Sub Scrivi_Log(Id_Budget As Integer,
                                  objDestinazioneEF As Object,
                                  TipoEntita As String,
                                  enum_TipoOperazioneDB As enum_TipoOperazioneDB,
                                  param1 As String, param2 As String, param3 As String, param4 As String, param5 As String,
                                  objParametri_Server As AgronicaCoreParametri,
                                  GiasContext As Gias_DeveloperServer_Entities)

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim noteLog As String = "Ribaltamento Budget-Reale da Id_Budget " & Id_Budget.ToString()

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim DatiLogStr = JsonConvert.SerializeObject(objDestinazioneEF, a)
        Dim Log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(TipoEntita,
                                                                             param1, param2, param3, param4, param5, Nothing,
                                                                             enum_TipoOperazioneDB, objParametri_Server,
                                                                             enum_Id_Servizio.GiasOnline,
                                                                             noteLog, DatiLogStr)

        GiasContext.Agronica_Log_Anagrafe.Add(Log)
        GiasContext.SaveChanges()
    End Sub

    Private Shared Sub Delete_Codici(TipoEntita As String,
                                     piva As String,
                                     sa_cod As Integer,
                                     appezza As Integer,
                                     id_reg As Integer,
                                     progetto_cod As Integer,
                                     campo_cod As Integer,
                                     GiasContext As Gias_DeveloperServer_Entities)

        Select Case TipoEntita
            Case enum_TipoEntita_Des.Campi
                Dim CampiCodici_List = (From codici In GiasContext.Campi_Codici
                                        Where codici.PIVA = piva AndAlso
                                            codici.sa_cod = sa_cod AndAlso
                                            codici.campo_cod = campo_cod).ToList()
                GiasContext.Campi_Codici.RemoveRange(CampiCodici_List)
            Case enum_TipoEntita_Des.Appezza
                Dim AppezzamentiCodici_List = (From codici In GiasContext.Appezzamento_Codici
                                               Where codici.PIVA = piva AndAlso
                                                   codici.sa_cod = sa_cod AndAlso
                                                   codici.appezza = appezza).ToList()
                GiasContext.Appezzamento_Codici.RemoveRange(AppezzamentiCodici_List)
            Case enum_TipoEntita_Des.Impianti
                Dim Reg_Impianti_Codici_List = (From codici In GiasContext.Reg_Impianti_Codici
                                                Where codici.PIVA = piva AndAlso
                                                    codici.sa_cod = sa_cod AndAlso
                                                    codici.appezza = appezza AndAlso
                                                    codici.Id_Reg = id_reg AndAlso
                                                    codici.Progetto_Cod = 0).ToList()
                GiasContext.Reg_Impianti_Codici.RemoveRange(Reg_Impianti_Codici_List)
            Case enum_TipoEntita_Des.Progetti
                Dim EserciziCodici_List = (From codici In GiasContext.Reg_Impianti_Codici
                                           Where codici.PIVA = piva AndAlso
                                               codici.sa_cod = sa_cod AndAlso
                                               codici.appezza = appezza AndAlso
                                               codici.Id_Reg = id_reg AndAlso
                                               codici.Progetto_Cod = progetto_cod).ToList()
                GiasContext.Reg_Impianti_Codici.RemoveRange(EserciziCodici_List)
        End Select

        GiasContext.SaveChanges()

    End Sub

    Private Shared Sub Clean_Tabelle_da_Ribaltamento_Corrente(Id_Budget As Integer,
                                                              dicAnagrafica_Ribaltata As Anagrafica_Ribaltata,
                                                              isLast As Boolean,
                                                              objParametri_Server As AgronicaCoreParametri,
                                                              GiasContext As Gias_DeveloperServer_Entities)

        'UTILIZZO
        'Questa funzione pulisce le tabelle di ribaltamento rispetto all'appezzamento budget che sto ribaltando attualmente

        'FUNZIONAMENTO
        'Ciclo tutti i dictionary per trovare gli elementi con Keep Element = false
        'Quindi li elimino, assieme a tutti gli elementi correlati (codici, particelle...)


        '-------------------------------
        '   REG_IMPIANTI
        '-------------------------------
        For Each imp In dicAnagrafica_Ribaltata.dicImpianti
            'KEY: PIVA-SA_COD-APPEZZA-ID_REG --> VALUE:PIVA-SA_COD-APPEZZA-ID_REG, KEEP ELEMENT (yes/no)
            If imp.Value.Item2 = False Then

                '-------------------------------
                '   REMOVE RIBALTAMENTO
                '-------------------------------
                Dim ribaltamento_to_delete = (From rib In GiasContext.Ribaltamento_Reg_Impianti
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                  rib.Budget_Piva = imp.Key.Item1 AndAlso
                                                  rib.Budget_Sa_Cod = imp.Key.Item2 AndAlso
                                                  rib.Budget_Appezza = imp.Key.Item3 AndAlso
                                                  rib.Budget_Id_Reg = imp.Key.Item4 AndAlso
                                                  rib.Reale_Piva = imp.Value.Item1.Item1 AndAlso
                                                  rib.Reale_Sa_Cod = imp.Value.Item1.Item2 AndAlso
                                                  rib.Reale_Appezza = imp.Value.Item1.Item3 AndAlso
                                                  rib.Reale_Id_Reg = imp.Value.Item1.Item4).FirstOrDefault()

                GiasContext.Ribaltamento_Reg_Impianti.Remove(ribaltamento_to_delete)
                GiasContext.SaveChanges()


                '-------------------------------
                '   REMOVE IMPIANTO
                '-------------------------------
                Dim impianto_to_delete = (From del In GiasContext.Reg_Impianti
                                          Where del.PIVA = imp.Value.Item1.Item1 AndAlso
                                              del.SA_COD = imp.Value.Item1.Item2 AndAlso
                                              del.APPEZZA = imp.Value.Item1.Item3 AndAlso
                                              del.ID_REG = imp.Value.Item1.Item4).FirstOrDefault()

                GiasContext.Reg_Impianti.Remove(impianto_to_delete)
                GiasContext.SaveChanges()


                '-------------------------------
                '   REMOVE CODICI
                '-------------------------------
                Delete_Codici(enum_TipoEntita_Des.Impianti, imp.Value.Item1.Item1, imp.Value.Item1.Item2, imp.Value.Item1.Item3, imp.Value.Item1.Item4, 0, 0, GiasContext)


                '-------------------------------
                '   LOG 
                '-------------------------------
                Scrivi_Log(Id_Budget, impianto_to_delete, enum_TipoEntita_Des.Impianti, enum_TipoOperazioneDB.Cancellazione,
                           impianto_to_delete.PIVA, impianto_to_delete.SA_COD, impianto_to_delete.APPEZZA, impianto_to_delete.ID_REG, Nothing,
                           objParametri_Server, GiasContext)

            End If
        Next


        '-------------------------------
        '   IMPRESE_PROGETTI
        '-------------------------------
        For Each ese In dicAnagrafica_Ribaltata.dicEsercizi
            'KEY: PIVA-SA_COD-APPEZZA-ID_REG-PROGETTO_COD --> VALUE:PIVA-SA_COD-APPEZZA-ID_REG-PROGETTO_COD, KEEP ELEMENT (yes/no)
            If ese.Value.Item2 = False Then

                '-------------------------------
                '   REMOVE RIBALTAMENTO
                '-------------------------------
                Dim ribaltamento_to_delete = (From rib In GiasContext.Ribaltamento_Imprese_Progetti
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                  rib.Budget_Piva = ese.Key.Item1 AndAlso
                                                  rib.Budget_Sa_Cod = ese.Key.Item2 AndAlso
                                                  rib.Budget_Appezza = ese.Key.Item3 AndAlso
                                                  rib.Budget_Id_Reg = ese.Key.Item4 AndAlso
                                                  rib.Budget_Progetto_Cod = ese.Key.Item5 AndAlso
                                                  rib.Reale_Piva = ese.Value.Item1.Item1 AndAlso
                                                  rib.Reale_Sa_Cod = ese.Value.Item1.Item2 AndAlso
                                                  rib.Reale_Appezza = ese.Value.Item1.Item3 AndAlso
                                                  rib.Reale_Id_Reg = ese.Value.Item1.Item4 AndAlso
                                                  rib.Reale_Progetto_Cod = ese.Value.Item1.Item5).FirstOrDefault()

                GiasContext.Ribaltamento_Imprese_Progetti.Remove(ribaltamento_to_delete)
                GiasContext.SaveChanges()


                '-------------------------------
                '   REMOVE ESERCIZIO
                '-------------------------------
                Dim esercizio_to_delete = (From del In GiasContext.Imprese_Progetti
                                           Where del.Piva = ese.Value.Item1.Item1 AndAlso
                                              del.Sa_Cod = ese.Value.Item1.Item2 AndAlso
                                              del.Appezza = ese.Value.Item1.Item3 AndAlso
                                              del.Id_Reg = ese.Value.Item1.Item4 AndAlso
                                              del.Progetto_Cod = ese.Value.Item1.Item5).FirstOrDefault()

                GiasContext.Imprese_Progetti.Remove(esercizio_to_delete)
                GiasContext.SaveChanges()


                '-------------------------------
                '   REMOVE CODICI
                '-------------------------------
                Delete_Codici(enum_TipoEntita_Des.Progetti, ese.Value.Item1.Item1, ese.Value.Item1.Item2, ese.Value.Item1.Item3, ese.Value.Item1.Item4, ese.Value.Item1.Item5, 0, GiasContext)


                '-------------------------------
                '   LOG 
                '-------------------------------
                Scrivi_Log(Id_Budget, esercizio_to_delete, enum_TipoEntita_Des.Progetti, enum_TipoOperazioneDB.Cancellazione,
                           esercizio_to_delete.Piva, esercizio_to_delete.Sa_Cod, esercizio_to_delete.Appezza, esercizio_to_delete.Id_Reg, esercizio_to_delete.Progetto_Cod,
                           objParametri_Server, GiasContext)

            End If
        Next


        '-------------------------------
        '   APPEZZAMENTI X INDIRIZZI
        '-------------------------------
        For Each appxind In dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi
            'KEY: PIVA-SA_COD-APPEZZA-INDIRIZZO_COD --> VALUE:PIVA-SA_COD-APPEZZA-INDIRIZZO_COD, KEEP ELEMENT (yes/no)
            If appxind.Value.Item2 = False Then

                '-------------------------------
                '   REMOVE RIBALTAMENTO
                '-------------------------------
                Dim ribaltamento_to_delete = (From rib In GiasContext.Ribaltamento_AppezzamentixIndirizzi
                                              Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                  rib.Budget_Piva = appxind.Key.Item1 AndAlso
                                                  rib.Budget_Sa_Cod = appxind.Key.Item2 AndAlso
                                                  rib.Budget_Appezza = appxind.Key.Item3 AndAlso
                                                  rib.Budget_Cod_Indirizzo = appxind.Key.Item4 AndAlso
                                                  rib.Reale_Piva = appxind.Value.Item1.Item1 AndAlso
                                                  rib.Reale_Sa_Cod = appxind.Value.Item1.Item2 AndAlso
                                                  rib.Reale_Appezza = appxind.Value.Item1.Item3 AndAlso
                                                  rib.Reale_Cod_Indirizzo = appxind.Value.Item1.Item4).FirstOrDefault()

                GiasContext.Ribaltamento_AppezzamentixIndirizzi.Remove(ribaltamento_to_delete)
                GiasContext.SaveChanges()


                '-------------------------------
                '   REMOVE APPEZZAMENTO X INDIRIZZI
                '-------------------------------
                Dim appxind_to_delete = (From del In GiasContext.AppezzamentixIndirizzi
                                         Where del.PIVA = appxind.Value.Item1.Item1 AndAlso
                                             del.sa_cod = appxind.Value.Item1.Item2 AndAlso
                                             del.appezza = appxind.Value.Item1.Item3 AndAlso
                                             del.cod_indirizzo = appxind.Value.Item1.Item4).FirstOrDefault()

                GiasContext.AppezzamentixIndirizzi.Remove(appxind_to_delete)
                GiasContext.SaveChanges()

                '-------------------------------
                '   REMOVE INDIRIZZO
                '-------------------------------
                Dim indirizzi_to_delete = (From ind In GiasContext.Indirizzi
                                           Where ind.cod_indirizzo = appxind.Value.Item1.Item4).FirstOrDefault()

                GiasContext.Indirizzi.Remove(indirizzi_to_delete)
                GiasContext.SaveChanges()
            End If
        Next


        '-------------------------------
        '   CAMPI
        '-------------------------------
        If isLast Then
            For Each camp In dicAnagrafica_Ribaltata.dicCampi
                'KEY: PIVA-SA_COD-CAMPO_COD --> VALUE:PIVA-SA_COD-CAMPO_COD, KEEP ELEMENT (yes/no)
                If camp.Value.Item2 = False Then

                    'CONTROLLO SE IL CAMPO E' USATO IN ALTRI APPEZZAMENTI REALI
                    Dim list_AppezzamentixCampo As List(Of Appezzamento) = (From app In GiasContext.Appezzamento
                                                                            Where app.PIVA = camp.Value.Item1.Item1 AndAlso
                                                                                app.SA_COD = camp.Value.Item1.Item2 AndAlso
                                                                                app.Campo_Cod = camp.Value.Item1.Item3).ToList()

                    'il campo non è usato in altri appezzamenti, rimuovo la tabella di collegamento
                    If list_AppezzamentixCampo.Count = 0 Then
                        '-------------------------------
                        '   REMOVE RIBALTAMENTO
                        '-------------------------------
                        Dim ribaltamento_to_delete = (From rib In GiasContext.Ribaltamento_Campi
                                                      Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                          rib.Budget_Piva = camp.Key.Item1 AndAlso
                                                          rib.Budget_Sa_Cod = camp.Key.Item2 AndAlso
                                                          rib.Budget_Campo_Cod = camp.Key.Item3 AndAlso
                                                          rib.Reale_Piva = camp.Value.Item1.Item1 AndAlso
                                                          rib.Reale_Sa_Cod = camp.Value.Item1.Item2 AndAlso
                                                          rib.Reale_Campo_Cod = camp.Value.Item1.Item3).FirstOrDefault()

                        GiasContext.Ribaltamento_Campi.Remove(ribaltamento_to_delete)
                        GiasContext.SaveChanges()


                        '-------------------------------
                        '   REMOVE CAMPO
                        '-------------------------------
                        Dim campo_to_delete = (From del In GiasContext.Campi
                                               Where del.Piva = camp.Value.Item1.Item1 AndAlso
                                                   del.Sa_Cod = camp.Value.Item1.Item2 AndAlso
                                                   del.Campo_Cod = camp.Value.Item1.Item3).FirstOrDefault()

                        GiasContext.Campi.Remove(campo_to_delete)
                        GiasContext.SaveChanges()


                        '-------------------------------
                        '   CAMPI CODICI
                        '-------------------------------
                        Delete_Codici(enum_TipoEntita_Des.Campi, camp.Value.Item1.Item1, camp.Value.Item1.Item2, 0, 0, 0, camp.Value.Item1.Item3, GiasContext)


                        '-------------------------------
                        '   LOG 
                        '-------------------------------
                        Scrivi_Log(Id_Budget, campo_to_delete, enum_TipoEntita_Des.Campi, enum_TipoOperazioneDB.Cancellazione,
                                   campo_to_delete.PIVA, campo_to_delete.SA_COD, campo_to_delete.campo_Cod, Nothing, Nothing,
                                   objParametri_Server, GiasContext)
                    End If
                End If
            Next
        End If

        'RESETTO I DICTIONARY (eccetto i campi)
        dicAnagrafica_Ribaltata.dicImpianti.Clear()
        dicAnagrafica_Ribaltata.dicEsercizi.Clear()
        dicAnagrafica_Ribaltata.dicAppezzamentoxIndirizzi.Clear()

    End Sub

    Public Shared Sub Clean_Tabelle_Ribaltamento_Globale(GiasContext As Gias_DeveloperServer_Entities)

        'UTILIZZO
        'Questa funzione viene chiamata alla prima iterazione di ribaltamento, pulisce i dati non più coerenti fra Anagrafica Reale - Tabelle Ribaltamento
        'Fatto qui per non andare a toccare le funzioni di cancellazione dell'anagrafica Reale

        'FUNZIONAMENTO
        'Leggo i dati delle tabelle di ribaltamento
        'controllo se gli elementi anagrafici reali esistono ancora
        'se non esistono elimino il record di ribaltamento


        '-------------------------------
        '   APPEZZAMENTI
        '-------------------------------
        Dim Ribaltamento_Appezzamento_list = (From rib In GiasContext.Ribaltamento_Appezzamento).ToList()
        For Each r In Ribaltamento_Appezzamento_list
            Dim elemReale = (From x In GiasContext.Appezzamento
                             Where x.PIVA = r.Reale_Piva AndAlso
                                 x.SA_COD = r.Reale_Sa_Cod AndAlso
                                 x.APPEZZA = r.Reale_Appezza).FirstOrDefault()

            If IsNothing(elemReale) Then
                GiasContext.Ribaltamento_Appezzamento.Remove(r)
                GiasContext.SaveChanges()
            End If
        Next


        '-------------------------------
        '   APPEZZAMENTI X INDIRIZZI
        '-------------------------------
        Dim Ribaltamento_AppezzamentixIndirizzi_list = (From rib In GiasContext.Ribaltamento_AppezzamentixIndirizzi).ToList()
        For Each r In Ribaltamento_AppezzamentixIndirizzi_list
            Dim elemReale = (From x In GiasContext.AppezzamentixIndirizzi
                             Where x.PIVA = r.Reale_Piva AndAlso
                                 x.sa_cod = r.Reale_Sa_Cod AndAlso
                                 x.appezza = r.Reale_Appezza AndAlso
                                 x.cod_indirizzo = r.Reale_Cod_Indirizzo).FirstOrDefault()

            If IsNothing(elemReale) Then
                GiasContext.Ribaltamento_AppezzamentixIndirizzi.Remove(r)
                GiasContext.SaveChanges()
            End If
        Next


        '-------------------------------
        '   IMPIANTI
        '-------------------------------
        Dim Ribaltamento_Reg_Impianti_list = (From rib In GiasContext.Ribaltamento_Reg_Impianti).ToList()
        For Each r In Ribaltamento_Reg_Impianti_list
            Dim elemReale = (From x In GiasContext.Reg_Impianti
                             Where x.PIVA = r.Reale_Piva AndAlso
                                 x.SA_COD = r.Reale_Sa_Cod AndAlso
                                 x.APPEZZA = r.Reale_Appezza AndAlso
                                 x.ID_REG = r.Reale_Id_Reg).FirstOrDefault()

            If IsNothing(elemReale) Then
                GiasContext.Ribaltamento_Reg_Impianti.Remove(r)
                GiasContext.SaveChanges()
            End If
        Next


        '-------------------------------
        '   PROGETTI
        '-------------------------------
        Dim Ribaltamento_Imprese_Progetti_list = (From rib In GiasContext.Ribaltamento_Imprese_Progetti).ToList()
        For Each r In Ribaltamento_Imprese_Progetti_list
            Dim elemReale = (From x In GiasContext.Imprese_Progetti
                             Where x.Piva = r.Reale_Piva AndAlso
                                 x.Sa_Cod = r.Reale_Sa_Cod AndAlso
                                 x.Appezza = r.Reale_Appezza AndAlso
                                 x.Id_Reg = r.Reale_Id_Reg AndAlso
                                 x.Progetto_Cod = r.Reale_Progetto_Cod).FirstOrDefault()

            If IsNothing(elemReale) Then
                GiasContext.Ribaltamento_Imprese_Progetti.Remove(r)
                GiasContext.SaveChanges()
            End If
        Next


        '-------------------------------
        '   CAMPI
        '-------------------------------
        Dim Ribaltamento_Campi_list = (From rib In GiasContext.Ribaltamento_Campi).ToList()
        For Each r In Ribaltamento_Campi_list
            Dim elemReale = (From x In GiasContext.Campi
                             Where x.Piva = r.Reale_Piva AndAlso
                                 x.Sa_Cod = r.Reale_Sa_Cod AndAlso
                                 x.Campo_Cod = r.Reale_Campo_Cod).FirstOrDefault()

            If IsNothing(elemReale) Then
                GiasContext.Ribaltamento_Campi.Remove(r)
                GiasContext.SaveChanges()
            End If
        Next


    End Sub

    Public Shared Sub Clean_Tabelle_Ribaltamento_daDeleteElemento(Id_Budget As Integer,
                                                                  piva As String, sa_cod As Integer, appezza As Integer,
                                                                  GiasContext As Gias_DeveloperServer_Entities,
                                                                  Optional deleteCampo_Cod As Integer = 0,
                                                                      Optional ByRef listRibaltamento_Appezzamento As List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Appezzamento) = Nothing,
                                                                      Optional ByRef listRibaltamento_Campi As List(Of AgronicaCoreEntityFramework_POCO.Ribaltamento_Campi) = Nothing)

        'UTILIZZO
        'Questa funzione viene chiamata esternamente, alla cancellazione di una testata/appezzamento/campo budget
        'per pulire i record delle tabelle di ribaltamento non più coerenti dopo la cancellazione

        'FUNZIONAMENTO
        'Leggo i dati delle tabelle di ribaltamento correlati all'appezzamento passato in input
        'elimino i record di ribaltamento
        'se deleteCampo_Cod <> 0, significa che la chiamata è partita dalla cancellazione di un campo --> cancello il record ribaltamento dell'elemento

        If deleteCampo_Cod <> 0 Then
            '-------------------------------
            '   CAMPO
            '-------------------------------
            Dim Ribaltamento_Campo = (From rib In GiasContext.Ribaltamento_Campi
                                      Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                          rib.Budget_Piva = piva AndAlso
                                          rib.Budget_Sa_Cod = sa_cod AndAlso
                                          rib.Budget_Campo_Cod = deleteCampo_Cod).FirstOrDefault()
            If Ribaltamento_Campo IsNot Nothing Then
                GiasContext.Ribaltamento_Campi.Remove(Ribaltamento_Campo)
                GiasContext.SaveChanges()

                If listRibaltamento_Appezzamento IsNot Nothing Then
                    listRibaltamento_Campi.Add(Ribaltamento_Campo)
                End If
            End If
        Else

            '-------------------------------
            '   APPEZZAMENTO
            '-------------------------------
            Dim Ribaltamento_Appezzamento = (From rib In GiasContext.Ribaltamento_Appezzamento
                                             Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                     rib.Budget_Piva = piva AndAlso
                                                     rib.Budget_Sa_Cod = sa_cod AndAlso
                                                     rib.Budget_Appezza = appezza).FirstOrDefault()
            If Ribaltamento_Appezzamento IsNot Nothing Then
                GiasContext.Ribaltamento_Appezzamento.Remove(Ribaltamento_Appezzamento)
                GiasContext.SaveChanges()
                If listRibaltamento_Appezzamento IsNot Nothing Then
                    listRibaltamento_Appezzamento.Add(Ribaltamento_Appezzamento)
                End If
            End If

            '-------------------------------
            '   APPEZZAMENTI X INDIRIZZI
            '-------------------------------
            Dim Ribaltamento_AppezzamentixIndirizzi_list = (From rib In GiasContext.Ribaltamento_AppezzamentixIndirizzi
                                                            Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                                rib.Budget_Piva = piva AndAlso
                                                                rib.Budget_Sa_Cod = sa_cod AndAlso
                                                                rib.Budget_Appezza = appezza).ToList()
            For Each r In Ribaltamento_AppezzamentixIndirizzi_list
                GiasContext.Ribaltamento_AppezzamentixIndirizzi.Remove(r)
                GiasContext.SaveChanges()
            Next


            '-------------------------------
            '   IMPIANTI
            '-------------------------------
            Dim Ribaltamento_Reg_Impianti_list = (From rib In GiasContext.Ribaltamento_Reg_Impianti
                                                  Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                      rib.Budget_Piva = piva AndAlso
                                                      rib.Budget_Sa_Cod = sa_cod AndAlso
                                                      rib.Budget_Appezza = appezza).ToList()
            For Each r In Ribaltamento_Reg_Impianti_list
                GiasContext.Ribaltamento_Reg_Impianti.Remove(r)
                GiasContext.SaveChanges()
            Next


            '-------------------------------
            '   PROGETTI
            '-------------------------------
            Dim Ribaltamento_Imprese_Progetti_list = (From rib In GiasContext.Ribaltamento_Imprese_Progetti
                                                      Where rib.Budget_Id_Testata = Id_Budget AndAlso
                                                          rib.Budget_Piva = piva AndAlso
                                                          rib.Budget_Sa_Cod = sa_cod AndAlso
                                                          rib.Budget_Appezza = appezza).ToList()
            For Each r In Ribaltamento_Imprese_Progetti_list
                GiasContext.Ribaltamento_Imprese_Progetti.Remove(r)
                GiasContext.SaveChanges()
            Next
        End If
    End Sub
#End Region

End Class

Public Class Anagrafica_Ribaltata
    'KEY: CHIAVI BUDGET
    'VALUE: CHIAVI REALE + KEEP ELEMENT (yes/no)

    'KEY: PIVA-SA_COD-APPEZZA-ID_REG --> VALUE:PIVA-SA_COD-APPEZZA-ID_REG, KEEP ELEMENT (yes/no)
    Public dicImpianti As New Dictionary(Of (String, Integer, Integer, Integer), ((String, Integer, Integer, Integer), Boolean))

    'KEY: PIVA-SA_COD-APPEZZA-ID_REG-PROGETTO_COD --> VALUE:PIVA-SA_COD-APPEZZA-ID_REG-PROGETTO_COD, KEEP ELEMENT (yes/no)
    Public dicEsercizi As New Dictionary(Of (String, Integer, Integer, Integer, Integer), ((String, Integer, Integer, Integer, Integer), Boolean))

    'KEY: PIVA-SA_COD-APPEZZA-INDIRIZZO_COD --> VALUE:PIVA-SA_COD-APPEZZA-INDIRIZZO_COD, KEEP ELEMENT (yes/no)
    Public dicAppezzamentoxIndirizzi As New Dictionary(Of (String, Integer, Integer, Integer), ((String, Integer, Integer, Integer), Boolean))

    'KEY: PIVA-SA_COD-CAMPO_COD --> VALUE:PIVA-SA_COD-CAMPO_COD, KEEP ELEMENT (yes/no)
    Public dicCampi As New Dictionary(Of (String, Integer, Integer), ((String, Integer, Integer), Boolean))

End Class

Public Module ObjectExtension

    <Extension()>
    Public Sub RiempiCampi_Ribaltamento(Of POCO)(objPOCO As POCO)

        'IMPORTANTE LEGGERE
        'Reflection on .NET by default is case sensitive for the class member.
        'To make it case insensitive you need to pass BindingFlags.IgnoreCase .

        'PROBLEM
        'I’ve passed the ignorecase flag and now it doesn’t return anything!!!

        'SOLUTION
        'Basically, If you pass one flag then the other flags will be overwritten by default which means all the default flags are disappeared.
        'So to make it case insensitive then you need to pass other binding flags

        'EXAMPLE
        'Dim pi As PropertyInfo = Me.[GetType]().GetProperty(fieldname, BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Instance)

        Dim p_Data_Ribaltamento = objPOCO.[GetType]().GetProperty("Data_Ribaltamento", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Data_Ribaltamento.SetValue(objPOCO, DateTime.Now)

        Dim p_Validita_Inizio = objPOCO.[GetType]().GetProperty("Validita_Inizio", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Validita_Inizio.SetValue(objPOCO, CostantiPersonalizzate.AGRODATAINIZIO)

        Dim p_Validita_Fine = objPOCO.[GetType]().GetProperty("Validita_Fine", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Validita_Fine.SetValue(objPOCO, CostantiPersonalizzate.AGRODATAFINE)

    End Sub

    <Extension()>
    Public Sub RiempiChiavi_Ribaltamento(Of RibaltamentoPOCO, objOrigin_POCO, objDestination_POCO)(objRibaltamento As RibaltamentoPOCO,
                                                                                                   OrigineEF As objOrigin_POCO,
                                                                                                   DestinazioneEF As objDestination_POCO,
                                                                                                   Id_Budget As Integer,
                                                                                                   piva As String,
                                                                                                   sa_cod As Integer,
                                                                                                   TipoEntita As String,
                                                                                                   username As String,
                                                                                                   Optional editCreazione As Boolean = True)

        'IMPORTANTE LEGGERE
        'Reflection on .NET by default is case sensitive for the class member.
        'To make it case insensitive you need to pass BindingFlags.IgnoreCase .

        'PROBLEM
        'I’ve passed the ignorecase flag and now it doesn’t return anything!!!

        'SOLUTION
        'Basically, If you pass one flag then the other flags will be overwritten by default which means all the default flags are disappeared.
        'So to make it case insensitive then you need to pass other binding flags

        'EXAMPLE
        'Dim pi As PropertyInfo = Me.[GetType]().GetProperty(fieldname, BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Instance)


        Dim p_Budget_Id_Testata = objRibaltamento.[GetType]().GetProperty("Budget_Id_Testata", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Budget_Id_Testata.SetValue(objRibaltamento, Id_Budget)

        '--------------------------
        '   BUDGET
        '--------------------------
        Dim p_Budget_Piva = objRibaltamento.[GetType]().GetProperty("Budget_Piva", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Budget_Piva.SetValue(objRibaltamento, piva)
        Dim p_Budget_Sa_Cod = objRibaltamento.[GetType]().GetProperty("Budget_Sa_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Budget_Sa_Cod.SetValue(objRibaltamento, sa_cod)

        '--------------------------
        '   REALE
        '--------------------------
        Dim p_Reale_Piva = objRibaltamento.[GetType]().GetProperty("Reale_Piva", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Reale_Piva.SetValue(objRibaltamento, piva)
        Dim p_Reale_Sa_Cod = objRibaltamento.[GetType]().GetProperty("Reale_Sa_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
        p_Reale_Sa_Cod.SetValue(objRibaltamento, sa_cod)


        If TipoEntita <> enum_TipoEntita_Des.Campi Then
            '--------------------------
            '   BUDGET
            '--------------------------
            Dim p_Budget_Appezza = objRibaltamento.[GetType]().GetProperty("Budget_Appezza", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            Dim p_Origine_Appezza = OrigineEF.[GetType]().GetProperty("Appezza", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            p_Budget_Appezza.SetValue(objRibaltamento, p_Origine_Appezza.GetValue(OrigineEF))

            '--------------------------
            '   REALE
            '--------------------------
            Dim p_Reale_Appezza = objRibaltamento.[GetType]().GetProperty("Reale_Appezza", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            Dim p_Destinazione_Appezza = DestinazioneEF.[GetType]().GetProperty("Appezza", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            p_Reale_Appezza.SetValue(objRibaltamento, p_Destinazione_Appezza.GetValue(DestinazioneEF))

        End If

        Select Case TipoEntita
            Case enum_TipoEntita_Des.Campi
                '--------------------------
                '   BUDGET
                '--------------------------
                Dim p_Budget_Campo_Cod = objRibaltamento.[GetType]().GetProperty("Budget_Campo_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Origine_Campo_Cod = OrigineEF.[GetType]().GetProperty("Campo_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Budget_Campo_Cod.SetValue(objRibaltamento, p_Origine_Campo_Cod.GetValue(OrigineEF))

                '--------------------------
                '   REALE
                '--------------------------
                Dim p_Reale_Campo_Cod = objRibaltamento.[GetType]().GetProperty("Reale_Campo_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Destinazione_Campo_Cod = DestinazioneEF.[GetType]().GetProperty("Campo_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Reale_Campo_Cod.SetValue(objRibaltamento, p_Destinazione_Campo_Cod.GetValue(DestinazioneEF))

            Case enum_TipoEntita_Des.AppezzamentiXIndirizzi
                '--------------------------
                '   BUDGET
                '--------------------------
                Dim p_Budget_Cod_Indirizzo = objRibaltamento.[GetType]().GetProperty("Budget_Cod_Indirizzo", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Origine_Cod_Indirizzo = OrigineEF.[GetType]().GetProperty("Cod_Indirizzo", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Budget_Cod_Indirizzo.SetValue(objRibaltamento, p_Origine_Cod_Indirizzo.GetValue(OrigineEF))

                '--------------------------
                '   REALE
                '--------------------------
                Dim p_Reale_Cod_Indirizzo = objRibaltamento.[GetType]().GetProperty("Reale_Cod_Indirizzo", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Destinazione_Cod_Indirizzo = DestinazioneEF.[GetType]().GetProperty("Cod_Indirizzo", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Reale_Cod_Indirizzo.SetValue(objRibaltamento, p_Destinazione_Cod_Indirizzo.GetValue(DestinazioneEF))

            Case enum_TipoEntita_Des.Impianti, enum_TipoEntita_Des.Progetti
                '--------------------------
                '   BUDGET
                '--------------------------
                Dim p_Budget_ID_REG = objRibaltamento.[GetType]().GetProperty("Budget_ID_REG", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Origine_ID_REG = OrigineEF.[GetType]().GetProperty("ID_REG", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Budget_ID_REG.SetValue(objRibaltamento, p_Origine_ID_REG.GetValue(OrigineEF))

                '--------------------------
                '   REALE
                '--------------------------
                Dim p_Reale_ID_REG = objRibaltamento.[GetType]().GetProperty("Reale_ID_REG", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                Dim p_Destinazione_ID_REG = DestinazioneEF.[GetType]().GetProperty("ID_REG", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                p_Reale_ID_REG.SetValue(objRibaltamento, p_Destinazione_ID_REG.GetValue(DestinazioneEF))

        End Select

        If TipoEntita = enum_TipoEntita_Des.Progetti Then
            '--------------------------
            '   BUDGET
            '--------------------------
            Dim p_Budget_Progetto_Cod = objRibaltamento.[GetType]().GetProperty("Budget_Progetto_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            Dim p_Origine_Progetto_Cod = OrigineEF.[GetType]().GetProperty("Progetto_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            p_Budget_Progetto_Cod.SetValue(objRibaltamento, p_Origine_Progetto_Cod.GetValue(OrigineEF))

            '--------------------------
            '   REALE
            '--------------------------
            Dim p_Reale_Progetto_Cod = objRibaltamento.[GetType]().GetProperty("Reale_Progetto_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            Dim p_Destinazione_Progetto_Cod = DestinazioneEF.[GetType]().GetProperty("Progetto_Cod", Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
            p_Reale_Progetto_Cod.SetValue(objRibaltamento, p_Destinazione_Progetto_Cod.GetValue(DestinazioneEF))

        End If

        objRibaltamento.RiempiCampi_Ribaltamento()
        objRibaltamento.RiempiCampi_StandardGias(username, editCreazione)

    End Sub

End Module