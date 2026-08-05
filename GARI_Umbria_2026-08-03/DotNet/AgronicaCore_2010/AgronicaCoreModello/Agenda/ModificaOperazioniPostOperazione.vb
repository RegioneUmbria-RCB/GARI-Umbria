'(Es frazionamento agenda post semina con frazionamento)  da chiamare dopo il salvataggio dell’operazione (scriviAgenda in transazione)
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.exceptions

Public Class ModificaOperazioniPostOperazione

    Public Function nomeTemp(agenda As OperazioneAgenda_Temp.Operazione_Agenda,
                             objParametri_Server As AgronicaCoreParametri,
                             objParametri_Utenti As AgronicaCoreParametri)

        Return True

    End Function



    Public Class ObjCDG_DaInserire
        Public Property EFArrayToInsert As ArrayList
        Public Property Id_Agenda As Integer
        Public Property Id_Agenda_Old As Integer
        Public Property Id_Agenda_CDG As Integer
        Public Property Id_Agenda_CDG_Old As Integer
        Public Property Des_Lib As String
        Public Property Data_Movimento As DateTime
    End Class

#Region "Frazionamento Agenda e bombardino"

    Public Sub FrazionaAgenda(ImpiantoDaModificare As AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC,
                              DtNuoviImpianti As DataTable,
                              Id_Agenda_DaEscludere As Integer,
                              ByRef ht_CDG As Hashtable,
                              objParametri_Server As AgronicaCoreParametri,
                              TopCode As Integer, BaseCode As Integer)

        Dim ObjDestR As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim objDestW As New AgronicaCoreContabDAL.Mov_Destinazioni_W



        Dim piva = ImpiantoDaModificare.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        Dim sa_cod = ImpiantoDaModificare.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
        Dim appezza = ImpiantoDaModificare.esercizio.impiantoPK.appezzamentoPK.codice
        Dim id_Reg = ImpiantoDaModificare.esercizio.impiantoPK.codice
        Dim qta2 = ImpiantoDaModificare.superficieTrattata


        'se l'appezzamento precedente è stato frazionato in diversi appezzamenti
        'verifico se c'erano operazioni registrate sopra
        'in tal caso le fraziono
        'escludo la semina nuova (Id_Agenda_DaEscludere)
        Dim DtDest As DataTable
        DtDest = ObjDestR.Leggi(piva, sa_cod,
                                0, 0, 0,
                                appezza, id_Reg,
                                0,
                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                " Mov_Destinazioni.Id_Agenda <> " & Id_Agenda_DaEscludere,
                                "",
                                objParametri_Server)

        Dim NewPiva As String
        Dim NewSaCod As String
        Dim NewAppezza As String
        Dim NewIdReg As String
        Dim NewSup As String
        Dim NewPercentuale As String

        Dim Id_Agenda As Integer

        Dim objAgendaR As New AgronicaCoreContabBIZ.Agenda_R
        Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W

        'Dim StringaXML As String = ""
        'Dim StringaXMLNew As String = ""
        'Dim StringaXMLNewTmp As String = ""

        Dim Id_Agenda_New As Integer

        Dim idAgendaProcessati As New ArrayList


        Dim DtApp_Old As New DataTable
        DtApp_Old.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        DtApp_Old.Columns.Add(New DataColumn("Piva", GetType(String)))
        DtApp_Old.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DtApp_Old.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        DtApp_Old.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
        DtApp_Old.Columns.Add(New DataColumn("Qta2", GetType(Decimal))) 'sup_trattata

        'necessario per split costi
        Dim DtApp_New As New DataTable
        Dim DrApp_New As DataRow
        DtApp_New.Columns.Add("Qta2", GetType(Decimal))

        Dim Id_Agenda_CDG As Integer = 0
        Dim Id_Agenda_CDG_Old As Integer = 0

        Dim DtImpDaSpostare As New DataTable
        DtImpDaSpostare.Columns.Add(New DataColumn("Piva", GetType(String)))
        DtImpDaSpostare.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DtImpDaSpostare.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        DtImpDaSpostare.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))

        Dim DrImpDaSpost As DataRow

        For i = 0 To DtDest.Rows.Count - 1

            Id_Agenda = DtDest.Rows(i).Item("id_agenda")

            DtApp_New.Rows.Clear()
            DtImpDaSpostare.Rows.Clear()
            DtApp_Old.Rows.Clear()

            Id_Agenda_CDG = 0

            If Not idAgendaProcessati.Contains(Id_Agenda) Then

                idAgendaProcessati.Add(Id_Agenda)

                Dim Sup_Tot_Operazione_Precedente As Decimal
                Dim Sup_Tot_Operazione_Precedente_Rimanente As Decimal
                Dim Sup_Tot_Operazione_Precedente_DaSpostare As Decimal

                '-------------------------------------------------
                Sup_Tot_Operazione_Precedente = 0
                Sup_Tot_Operazione_Precedente_Rimanente = 0
                Sup_Tot_Operazione_Precedente_DaSpostare = 0

                'operazione precedente da sistemare
                Dim objAgenda As New Agenda_Operazione_Helper
                Dim Agenda As New Operazione_Agenda
                Agenda = objAgenda.Leggi(piva, 0,
                                     Id_Agenda,
                                     0,
                                     objParametri_Server)

                'verifico se la vecchia operazione coinvolgeva solo l'app seminato e calcolo la vecchia sup totale
                Dim piuApp As Boolean = False
                For Each Movimento In Agenda.Movimenti
                    Select Case Movimento.Cau_Mov
                        Case CAU_LAVORAZIONE, CAU_TRATTAMENTO, CAU_TRATTAMENTO
                            For Each Movimento_Destinazione In Movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni
                                Sup_Tot_Operazione_Precedente += Movimento_Destinazione.Qta2
                                If Movimento_Destinazione.Appezza <> appezza Then
                                    piuApp = True
                                End If
                            Next
                    End Select
                Next

                '-------------------------------------------------
                Sup_Tot_Operazione_Precedente_DaSpostare = 0

                For Each drNuoviImp As DataRow In DtNuoviImpianti.Rows
                    DrImpDaSpost = DtImpDaSpostare.NewRow
                    DrImpDaSpost.Item("Piva") = drNuoviImp.Item("Piva")
                    DrImpDaSpost.Item("Sa_Cod") = drNuoviImp.Item("sa_cod")
                    DrImpDaSpost.Item("Appezza") = drNuoviImp.Item("appezza_new")
                    DrImpDaSpost.Item("Id_Destinazione") = drNuoviImp.Item("id_reg_new")
                    Sup_Tot_Operazione_Precedente_DaSpostare += drNuoviImp.Item("sup_new")
                    DtImpDaSpostare.Rows.Add(DrImpDaSpost)
                Next

                Dim DrImpDaSpostare As DataRow() = DtImpDaSpostare.Select("")

                Sup_Tot_Operazione_Precedente_Rimanente = Sup_Tot_Operazione_Precedente - Sup_Tot_Operazione_Precedente_DaSpostare


                '1. operazioni nuove da creare faccio una copia e la creo
                '2. modifico le nuove operazioni create per eliminare le destinazioni vecchie rimaste sulla nuova ed aggiornare le qta
                '3. se la vecchia operazione aveva altri impianti (oltre a quello frazionato) la modifico per eliminare le destinazioni che cambiano con la semina ed aggiornare le qta
                '   altrimenti la elimino


                For a = 0 To DtNuoviImpianti.Rows.Count - 1

                    NewPiva = DtNuoviImpianti.Rows(a).Item("Piva")
                    NewSaCod = DtNuoviImpianti.Rows(a).Item("sa_cod")
                    NewAppezza = DtNuoviImpianti.Rows(a).Item("appezza_new")
                    NewIdReg = DtNuoviImpianti.Rows(a).Item("id_reg_new")
                    NewSup = DtNuoviImpianti.Rows(a).Item("sup_new")
                    NewPercentuale = DtNuoviImpianti.Rows(a).Item("sup_perc")

                    '----------------------------------------------------------------
                    '1. operazione nuova da creare faccio una copia e la creo subito

                    Dim AgendaNew As New Operazione_Agenda
                    AgendaNew = Agenda.DeepCloneObject
                    AgendaNew.Id_Agenda = 0
                    AgendaNew.Tipo_Operazione = 1

                    If Not AgendaNew.Agenda_Riferimenti Is Nothing AndAlso AgendaNew.Agenda_Riferimenti.Count > 0 Then
                        AgendaNew.Agenda_Riferimenti.Clear()
                    End If
                    For Each movimento As Movimento In AgendaNew.Movimenti
                        movimento.Id_Agenda = 0
                        movimento.Id_Mov = 0
                        For Each movimento_dettaglio_tecnico As Movimento_Dettaglio_Tecnico In movimento.Movimenti_Dettagli_Tecnici
                            movimento_dettaglio_tecnico.Id_Agenda = 0
                            movimento_dettaglio_tecnico.Id_Mov = 0
                            movimento_dettaglio_tecnico.Id_Reg_Dettaglio = 0
                        Next
                        For Each movimento_dettaglio As Movimento_Dettaglio In movimento.Movimenti_Dettagli
                            movimento_dettaglio.Id_Agenda = 0
                            movimento_dettaglio.Id_Mov = 0
                            movimento_dettaglio.Id_Mov_Det = 0
                            For Each movimento_dettaglio_tecnico2 As Movimento_Dettaglio_Tecnico In movimento_dettaglio.Movimenti_Dettagli_Tecnici
                                movimento_dettaglio_tecnico2.Id_Agenda = 0
                                movimento_dettaglio_tecnico2.Id_Mov = 0
                                movimento_dettaglio_tecnico2.Id_Mov_Det = 0
                                movimento_dettaglio_tecnico2.Id_Reg_Dettaglio = 0
                            Next
                            For Each movimento_destinazione As Movimento_Destinazione In movimento_dettaglio.Movimenti_Destinazioni
                                movimento_destinazione.Id_Agenda = 0
                                movimento_destinazione.Id_Mov = 0
                                movimento_destinazione.Id_Mov_Det = 0
                                'sostituisco l'impianto precedente con il nuovo 'e modifico qta e qta2 destinazioni
                                If movimento_destinazione.Appezza = appezza And movimento_destinazione.Id_Destinazione = id_Reg Then
                                    movimento_destinazione.Appezza = NewAppezza
                                    movimento_destinazione.Id_Destinazione = NewIdReg
                                    movimento_destinazione.Qta2 = movimento_destinazione.Qta2 * NewPercentuale / 100
                                End If
                            Next
                            For Each movimento_dettaglio_riferimento As Movimento_Dettaglio_Riferimento In movimento_dettaglio.Movimenti_Dettagli_Riferimenti
                                movimento_dettaglio_riferimento.Id_Agenda = 0
                                movimento_dettaglio_riferimento.Id_Mov = 0
                                movimento_dettaglio_riferimento.Id_Mov_Det = 0
                            Next
                        Next
                    Next

                    Id_Agenda_New = objAgenda.Scrivi(AgendaNew, objParametri_Server)


                    AggiornaOperazioneNuova_PostFrazionamento(AgendaNew, Id_Agenda,
                                                              Sup_Tot_Operazione_Precedente, NewSup,
                                                              DrImpDaSpostare, DtApp_Old,
                                                              objParametri_Server)


                    '-------------------------------------
                    'necesssario per split costi
                    DtApp_New.Rows.Clear()
                    DrApp_New = DtApp_New.NewRow
                    DrApp_New.Item("Qta2") = NewSup
                    DtApp_New.Rows.Add(DrApp_New)


                    Try

                        Dim EFArrayToInsert As New ArrayList

                        Dim Des_Lib As String = ""
                        Dim Data_Movimento As DateTime

                        EFArrayToInsert.Clear()

                        Dim scrivi_BIZ2 As New AgronicaCoreContabBIZ.CDG_BIZ_W

                        'la sistemazione dei costi della vecchia operazione deve essere fatta una volta sola (l'ultima)
                        Dim AggiornaCostiOld As Boolean = False
                        If a = DtNuoviImpianti.Rows.Count - 1 Then
                            AggiornaCostiOld = True
                        End If

                        Id_Agenda_CDG_Old = scrivi_BIZ2.AggiornaCDG_Split_SeminaTrapianto(piva,
                                                                   Id_Agenda,
                                                                   Id_Agenda_New,
                                                                   DtApp_Old,
                                                                   DtApp_New,
                                                                   EFArrayToInsert,
                                                                   Id_Agenda_CDG,
                                                                   Des_Lib,
                                                                   Data_Movimento,
                                                                   objParametri_Server,
                                                                   True, qta2, AggiornaCostiOld)


                        If Id_Agenda_New <> 0 Then

                            'Inserimento CDG da creare
                            Dim myObj As New ObjCDG_DaInserire

                            myObj.EFArrayToInsert = EFArrayToInsert
                            myObj.Id_Agenda_CDG = Id_Agenda_CDG
                            myObj.Id_Agenda = Id_Agenda_New
                            myObj.Des_Lib = Des_Lib
                            myObj.Data_Movimento = Data_Movimento
                            myObj.Id_Agenda_Old = Id_Agenda

                            If piuApp = True Then
                                myObj.Id_Agenda_CDG_Old = 0
                            Else
                                myObj.Id_Agenda_CDG_Old = Id_Agenda_CDG_Old 'Verrà cancellato al momento della chiusura altrimenti
                                'va in timeout la transazione nella tabella mov_dettagli_riferimenti
                            End If


                            ht_CDG.Add(Id_Agenda_New, myObj)

                        End If

                    Catch ex As Exception

                        Throw New Exception("[SalvaModificheImpianti] :   " & ex.Message)

                    End Try
                Next

                'se la vecchia operazione coinvolgeva solo l'app seminato
                '   la cancello (anche eventuali costi)
                'altrimenti la modifico (aggiorno i quantitativi di acqua e prodotti ed elimino l'appezzamento appena frazionato)

                If piuApp = False Then

                    objAgenda.Cancella(piva, 0, Id_Agenda, True, objParametri_Server)
                    'objAgendaW.Agenda_Scrivi(StringaXML, 0, 0, 0, 0, "", objParametri_Server)

                Else

                    AggiornaOperazionePrecedente_PostFrazionamento(Agenda, Sup_Tot_Operazione_Precedente, Sup_Tot_Operazione_Precedente_Rimanente,
                                                                   DrImpDaSpostare, objParametri_Server)

                End If

            End If

        Next


    End Sub

    Public Sub CancellamentoVecchiCdG_e_AllineamentoDate(piva As String,
                                                         ht_CDG As Hashtable,
                                                         objParametri_Server As AgronicaCoreParametri
                                                         )

        '==================================================================================================================================
        '(28/03/2022 Marco) - Cancellazione Dati CDG vecchi ed Allineamento Date
        '----------------------------------------------------------------------------------------------------------------------------------           

        Dim objAgendaR As New AgronicaCoreContabDAL.Agenda_R
        Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W
        Dim scrivi_DAL As New AgronicaCoreContabDAL.CDG_DAL_W
        Dim DtAgenda As New DataTable
        Dim objAgendaScrivi As New Agenda_Operazione_Helper
        Dim CancellataOperazione As Boolean = False


        For Each obj_CDG As DictionaryEntry In ht_CDG

            'Cancellazione vecchia Agenda CDG
            If DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Id_Agenda_CDG_Old <> 0 Then

                'Controllo Esistenza Operazione di Agenda --> altrimenti il core si incavola
                DtAgenda = objAgendaR.Leggi(piva, 0,
                                            DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Id_Agenda_CDG_Old,
                                            0,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objParametri_Server)

                If DtAgenda.Rows.Count > 0 Then

                    CancellataOperazione = objAgendaScrivi.Cancella(piva, 0,
                                                                    DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Id_Agenda_CDG_Old,
                                                                    False,
                                                                    objParametri_Server, logCancellazione:=False)
                End If

            End If

            'Allineamento Date
            If CDate(DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Data_Movimento) > AGRODATAINIZIO Then
                scrivi_DAL.AllineaDateRiferimento(piva,
                                                  DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Id_Agenda_Old,
                                                  0,
                                                  DateAdd("d", -1, Format(DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Data_Movimento, "dd/MM/yyyy")),
                                                  objParametri_Server)
            End If


        Next

    End Sub

    Public Sub InserimentoCdG(piva As String,
                              ht_CDG As Hashtable,
                              objParametri_Server As AgronicaCoreParametri,
                              objParametri_Utenti As AgronicaCoreParametri,
                              Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                              Optional ByVal OpenNewTransaction As Boolean = True,
                              Optional ByRef listaErrori As List(Of ErroreGias) = Nothing
                              )

        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If


        '==================================================================================================================================
        '(19/04/2021 Marco) - Inserimento CDG Costi
        '----------------------------------------------------------------------------------------------------------------------------------
        Dim Id_Agenda_CDG As Integer = 0
        Dim CDG_W As New AgronicaCoreContabDAL.CDG_DAL_W
        For Each obj_CDG As DictionaryEntry In ht_CDG

            'Creazione Agenda CDG
            Id_Agenda_CDG = CDG_W.Aggiorna_CDG(False, 0, piva,
                                               DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Id_Agenda_CDG,
                                               0,
                                               DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Des_Lib,
                                               Format(DirectCast(obj_CDG.Value, ObjCDG_DaInserire).Data_Movimento, "dd/MM/yyyy"),
                                               DirectCast(obj_CDG.Value, ObjCDG_DaInserire).EFArrayToInsert,
                                               objParametri_Server,
                                               GiasContext)
        Next

        Dim messaggio As String = ""
        ''Marco: Sospeso perchè aggiornerebbe tutte le operazioni in sospeso con notevole attesa
        ''==================================================================================================================================
        ''(19/04/2021 Marco) - Lancio del bombardino massivo su tutte le operazioni create
        ''----------------------------------------------------------------------------------------------------------------------------------
        'Dim CDG_W_BIZ As New AgronicaCoreContabBIZ.CDG_BIZ_W
        'messaggio= CDG_W_BIZ.AllineaCosti_BombardinoMultiplo(piva, objParametri_Server, objParametri_Utenti,
        '                                                                    GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
        '                                                                    listaErrori:=listaErrori)


        If bCloseContext Then
            GiasContext.Dispose()
        End If

        If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
            'In questa lista errori vengono inseriti errori del bombardino che richiedono l'assistenza dell'Amministratore, il messaggio deve essere visibile e gestito
            Exit Sub
        End If

        If messaggio <> "" Then
            Throw New Exception("Bombardino in errore: " & messaggio)
        End If

    End Sub

    Private Sub AggiornaOperazioneNuova_PostFrazionamento(ByVal AgendaNew As Operazione_Agenda, ByVal id_agenda_da_sistemare As Integer,
                                                          ByVal Sup_Tot_Operazione_Precedente As Decimal,
                                                          ByVal Sup_Tot_Operazione_Precedente_DaSpostare As Decimal,
                                                          ByVal DrImpDaSpostare() As DataRow,
                                                          ByRef DtApp_Old As DataTable,
                                                          objParametri_Server As AgronicaCoreParametri)


        '----------------------------------------
        'NUOVA OPERAZIONE
        '----------------------------------------

        'modifico l'acqua (se salvata totale)

        'modifico qta dettaglio (se cambiato)

        'modifico qta (se cambiata) e percentuale destinazione

        'modifico la qta prodotto scaricato (se presente scarico)

        'elimino le destinazioni rimaste sulla precedente operazione (quelle NON selezionate nella semina corrente)
        '----------------------------------------
        '----------------------------------------

        Dim Lav_Cod As Integer
        Dim AcquaTot As Decimal
        Dim Dose As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0
        Dim Dose_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0
        Dim DoseTotaleTrasformata_New As Decimal = 0
        Dim Qta_Dest_New As Decimal = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Lotto As String = ""
        Dim Cod_Progetto As Integer = 0

        Dim HashTotNew As New Hashtable

        Dim DrApp_Old As DataRow

        Dim objDetTecn As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

        HashTotNew = New Hashtable

        AcquaTot = 0

        If Not IsNothing(AgendaNew) Then

            Lav_Cod = AgendaNew.Lav_Cod

            'MOVIMENTI
            If Not IsNothing(AgendaNew.Movimenti) Then

                'MOVIMENTO LAVORAZIONE
                'modifico le destinazioni --> qta e qta2 
                For i = 0 To AgendaNew.Movimenti.Count - 1

                    Select Case AgendaNew.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                            '---------------------------------------------------
                            '----- Acqua  
                            '---------------------------------------------------
                            'modifico l'acqua se era stata salvata totale
                            'se era stata salvata ad ettaro calcolo cmq la totale perchè mi serve per calcolare il nuovo dosaggio ad HL del prodotto
                            If Not AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici Is Nothing Then
                                For j = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                    Select Case AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                        Case Is > 0 'dosaggio totale --> la ricalcolo sulla nuova superficie
                                            AcquaTot = AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril / Sup_Tot_Operazione_Precedente * Sup_Tot_Operazione_Precedente_DaSpostare

                                            objDetTecn.ModificaPuntuale(AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Piva, 0,
                                                                        AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda,
                                                                        0, 0, 0,
                                                                        objParametri_Server,
                                                                        Qta_Ril:=AcquaTot)

                                        Case Is < 0 'dosaggio/ha --> già ok
                                            AcquaTot = Math.Abs(AgendaNew.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril) * Sup_Tot_Operazione_Precedente_DaSpostare
                                    End Select
                                Next
                            End If

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    'se non ho il codice del dettaglio da modificare (perchè ho appena scritto l'operazione) lo prendo da una delle sue destinazioni
                                    Dim Id_Mov_Det As Integer = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det
                                    If Id_Mov_Det = 0 Then
                                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) AndAlso AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count > 0 Then
                                            Id_Mov_Det = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Mov_Det
                                        End If
                                    End If

                                    Fr_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod '(per semine e raccolte)
                                    Lotto = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Lotto '(per semine)
                                    Udm_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto '(per semilavorati raccolta)

                                    Dose = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                    If AgendaNew.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) AndAlso AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                            Dose = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                            Dett_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod
                                        End If
                                    End If

                                    Select Case Lav_Cod
                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                            Dose_Tot_New = If(Sup_Tot_Operazione_Precedente <> 0, Math.Round(Dose / Sup_Tot_Operazione_Precedente * Sup_Tot_Operazione_Precedente_DaSpostare, 0), 0)
                                            'Dose_Tot_New = Math.Round(Dose / Sup_Tot_Operazione_Precedente * Sup_Tot_Operazione_Precedente_DaSpostare, 0)
                                            Dose_Tot_Old = Dose
                                        Case Else
                                            Dose_Tot_New = Dose * Sup_Tot_Operazione_Precedente_DaSpostare
                                            Dose_Tot_Old = Dose * Sup_Tot_Operazione_Precedente
                                    End Select


                                    Select Case AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                        Case enum_UnitaMisura.Grammi  'g
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.Milligrammi
                                            Dose_Trasformata_New = Dose / 1000000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000000
                                        Case enum_UnitaMisura.Quintali
                                            Dose_Trasformata_New = Dose * 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 100
                                        Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Metri_Cubi
                                            Dose_Trasformata_New = Dose * 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 1000
                                        Case enum_UnitaMisura.Millilitri
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.CentimetriCubi
                                            Dose_Trasformata_New = Dose / 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 100
                                        Case Else
                                            Dose_Trasformata_New = Dose
                                            DoseTotaleTrasformata_New = Dose_Tot_New
                                    End Select


                                    If Fr_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Fr_Cod) Then
                                            HashTotNew.Add(Fr_Cod, DoseTotaleTrasformata_New)
                                        End If
                                    ElseIf Mat_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            HashTotNew.Add(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto, DoseTotaleTrasformata_New)
                                        End If
                                    End If

                                    Dose_New = 0
                                    Dose_New_Hl = 0

                                    Select Case Lav_Cod

                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA
                                            'qta è quella totale distribuita
                                            Dose_New = Dose_Tot_New
                                            'modifico la qta del dettaglio (dose_ha)
                                            objDet.Modifica_Quantita(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Id_Mov_Det,
                                                                     0, 0, 0, 0,
                                                                     Dose_New,
                                                                     0, 0, 0, 0, 0,
                                                                     "", "",
                                                                     objParametri_Server)

                                        Case LAVCOD_IRRIGAZIONE,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                        Case Else

                                            Dose_New = Dose

                                            If AcquaTot <> 0 Then
                                                Dose_New_Hl = Dose_New * Sup_Tot_Operazione_Precedente_DaSpostare / AcquaTot
                                            End If

                                            'modifico la qta_extra e la qta_extra_tot del dettaglio (dose_hl, qta tot)
                                            objDet.Modifica_Quantita(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Id_Mov_Det,
                                                                     0, 0, 0, 0,
                                                                     Dose_New, Dose_New_Hl, Dose_Tot_New,
                                                                     0, 0, 0,
                                                                     "", "",
                                                                     objParametri_Server)

                                    End Select

                                    'MOVIMENTI_DESTINAZIONI
                                    If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                        For x = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                            Dim SupTrattata As Decimal = 0
                                            Dim AppDaModificare As Boolean = False
                                            For Each imp_da_spostare As DataRow In DrImpDaSpostare
                                                If imp_da_spostare.Item("Piva") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva And
                                                      imp_da_spostare.Item("Sa_Cod") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod And
                                                      imp_da_spostare.Item("Appezza") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza And
                                                      imp_da_spostare.Item("Id_Destinazione") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then
                                                    AppDaModificare = True
                                                    Exit For
                                                End If
                                            Next

                                            If AppDaModificare = True Then

                                                Dim QuotaDistribuzioneNew As Decimal = 0
                                                If Sup_Tot_Operazione_Precedente_DaSpostare <> 0 Then
                                                    QuotaDistribuzioneNew = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2 / Sup_Tot_Operazione_Precedente_DaSpostare
                                                End If

                                                Select Case Lav_Cod

                                                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                                        objDest.Modifica_SupTrattata(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2,
                                                                                     QuotaDistribuzioneNew,
                                                                                     "", objParametri_Server)
                                                    Case Else

                                                        Select Case Lav_Cod
                                                            Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                Qta_Dest_New = Math.Round(Dose * AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2, 0)
                                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                                     LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                     LAVCOD_RACCOLTA
                                                                'dose salvata sempre qta totale
                                                                Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_Operazione_Precedente_DaSpostare) * AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2, 0)
                                                                Dose_Tot_New = Math.Round(Dose_New, 0)
                                                            Case LAVCOD_IRRIGAZIONE
                                                                Select Case Dett_Cod
                                                                    Case enum_UnitaMisura.Millimetri
                                                                        Dose = Dose * 10
                                                                    Case enum_UnitaMisura.METRI3__HA
                                                                        Dose = Dose * 1
                                                                    Case Else
                                                                        Dose = 0
                                                                End Select
                                                                Qta_Dest_New = Dose * AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

                                                            Case Else
                                                                Qta_Dest_New = Dose_Trasformata_New * AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2
                                                        End Select

                                                        'modifico qta_destinazione e sup_trattata
                                                        objDest.Modifica_Quantita_e_SupTrattata(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                                                Qta_Dest_New,
                                                                                                AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2,
                                                                                                QuotaDistribuzioneNew,
                                                                                                "", objParametri_Server)

                                                End Select
                                            End If
                                        Next
                                    End If
                                Next
                            End If
                    End Select
                Next


                'MOVIMENTO SCARICO
                'modifico la qta del dettaglio e la qta destinazione (magazzino)

                For i = 0 To AgendaNew.Movimenti.Count - 1

                    Select Case AgendaNew.Movimenti(i).Cau_Mov

                        Case CAU_SCARICO, CAU_CARICO

                            If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    'se non ho il codice del dettaglio da modificare (perchè ho appena scritto l'operazione) lo prendo da una delle sue destinazioni
                                    Dim Id_Mov_Det As Integer = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det
                                    If Id_Mov_Det = 0 Then
                                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) AndAlso AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count > 0 Then
                                            Id_Mov_Det = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Mov_Det
                                        End If
                                    End If

                                    Fr_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                                    Lotto = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Lotto
                                    Udm_Cod = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto

                                    Dose = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    DoseTotaleTrasformata_New = 0

                                    If Fr_Cod <> 0 Then
                                        If HashTotNew.ContainsKey(Fr_Cod) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Fr_Cod)
                                        End If
                                    Else
                                        If HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto)
                                        End If
                                    End If

                                    If DoseTotaleTrasformata_New <> 0 Then

                                        'modifico la qta del dettaglio 
                                        objDet.Modifica_Quantita(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                 AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                 AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                 Id_Mov_Det,
                                                                 0, 0, 0, 0,
                                                                 DoseTotaleTrasformata_New,
                                                                 0, 0, 0, 0, 0, "", "",
                                                                 objParametri_Server)


                                        'MOVIMENTI_DESTINAZIONI
                                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                            For x = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                                'modifico qta_destinazione 
                                                objDest.Modifica_Quantita(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                          AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                          DoseTotaleTrasformata_New,
                                                                          "", objParametri_Server)

                                            Next
                                        End If
                                    End If
                                Next
                            End If
                    End Select
                Next
            End If
        End If


        ' elimino le destinazioni rimaste nella precedente
        If Not IsNothing(AgendaNew) AndAlso Not IsNothing(AgendaNew.Movimenti) Then
            For i = 0 To AgendaNew.Movimenti.Count - 1
                Select Case AgendaNew.Movimenti(i).Cau_Mov
                    Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA
                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli) Then
                            For j = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli.Count - 1
                                Dim EliminataDestinazione As Boolean = False
                                If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                    For x = 0 To AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                        Dim AppDaEliminare As Boolean = True
                                        For Each imp_da_spostare As DataRow In DrImpDaSpostare
                                            If AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva = imp_da_spostare.Item("Piva") And
                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod = imp_da_spostare.Item("Sa_Cod") And
                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza = imp_da_spostare.Item("Appezza") And
                                                     AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione = imp_da_spostare.Item("Id_Destinazione") Then
                                                AppDaEliminare = False
                                                Exit For
                                            End If
                                        Next
                                        If AppDaEliminare = True Then
                                            objDest.Cancella(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                             AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                             "", objParametri_Server)

                                            Dim DrAppOldTmp() As DataRow = DtApp_Old.Select("id_agenda=" & id_agenda_da_sistemare &
                                                                                                " AND piva ='" & AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva & "' AND sa_cod=" & AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod &
                                                                                                " and appezza=" & AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza & " and id_destinazione=" & AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione)
                                            If DrAppOldTmp.Length = 0 Then
                                                DrApp_Old = DtApp_Old.NewRow
                                                DrApp_Old.Item("Id_Agenda") = id_agenda_da_sistemare
                                                DrApp_Old.Item("Piva") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva
                                                DrApp_Old.Item("Sa_Cod") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                DrApp_Old.Item("Appezza") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza
                                                DrApp_Old.Item("Id_Destinazione") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                DrApp_Old.Item("Qta2") = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2
                                                DtApp_Old.Rows.Add(DrApp_Old)
                                            End If


                                        End If
                                    Next
                                End If
                                'se il dettaglio aveva una sola destinazione (alcune operazioni) che viene eliminata
                                'elimino anche lui
                                If AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count = 1 And EliminataDestinazione = True Then

                                    'se non ho il codice del dettaglio da modificare (perchè ho appena scritto l'operazione) lo prendo da una delle sue destinazioni
                                    Dim Id_Mov_Det As Integer = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det
                                    If Id_Mov_Det = 0 Then
                                        If Not IsNothing(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) AndAlso AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count > 0 Then
                                            Id_Mov_Det = AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Mov_Det
                                        End If
                                    End If

                                    objDet.Cancella(AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                    AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                    AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                    AgendaNew.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                    Id_Mov_Det,
                                                    "", objParametri_Server)
                                End If
                            Next
                        End If
                End Select
            Next
        End If


    End Sub

    Private Sub AggiornaOperazionePrecedente_PostFrazionamento(ByVal Agenda As Operazione_Agenda,
                                                               ByVal Sup_Tot_Operazione_Precedente As Decimal,
                                                               ByVal Sup_Tot_Operazione_Precedente_Rimanente As Decimal,
                                                               ByVal DrImpDaSpostare() As DataRow,
                                                               objParametri_Server As AgronicaCoreParametri)

        '----------------------------------------
        '----------------------------------------
        'VECCHIA OPERAZIONE
        '----------------------------------------

        'modifico l'acqua (se salvata totale)

        'modifico qta dettaglio (se cambiato)

        'modifico qta (se cambiata) e percentuale destinazione

        'modifico la qta prodotto scaricato (se presente scarico)

        'elimino le destinazioni che andranno sulla nuova (le selezionate nella semina corrente)
        '----------------------------------------
        '----------------------------------------

        Dim Lav_Cod As Integer
        Dim AcquaTot As Decimal
        Dim Dose As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0
        Dim Dose_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0
        Dim DoseTotaleTrasformata_New As Decimal = 0
        Dim Qta_Dest_New As Decimal = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Lotto As String = ""
        Dim Cod_Progetto As Integer = 0

        Dim HashTotNew As New Hashtable

        Dim objDetTecn As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

        If Not IsNothing(Agenda) Then

            Lav_Cod = Agenda.Lav_Cod

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                'MOVIMENTO LAVORAZIONE
                'modifico le destinazioni --> qta e qta2 
                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                            '---------------------------------------------------
                            '----- Acqua  
                            '---------------------------------------------------
                            'modifico l'acqua se era stata salvata totale
                            'se era stata salvata ad ettaro calcolo cmq la totale perchè mi serve per calcolare il nuovo dosaggio ad HL del prodotto
                            If Not Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici Is Nothing Then
                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                        Case Is > 0 'dosaggio totale --> la ricalcolo sulla nuova superficie
                                            AcquaTot = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril / Sup_Tot_Operazione_Precedente * Sup_Tot_Operazione_Precedente_Rimanente

                                            objDetTecn.ModificaPuntuale(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Piva, 0,
                                                                        Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda, 0, 0, 0,
                                                                        objParametri_Server,
                                                                        Qta_Ril:=AcquaTot)

                                        Case Is < 0 'dosaggio/ha --> già ok
                                            AcquaTot = Math.Abs(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril) * Sup_Tot_Operazione_Precedente_Rimanente
                                    End Select
                                Next
                            End If

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod '(per semine e raccolte)
                                    Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto '(per semine)
                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto '(per semilavorati raccolta)

                                    Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                    If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) AndAlso Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                            Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                            Dett_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod
                                        End If
                                    End If

                                    Select Case Lav_Cod
                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                         LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                         LAVCOD_RACCOLTA,
                                         LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                            Dose_Tot_New = Math.Round(Dose / Sup_Tot_Operazione_Precedente * Sup_Tot_Operazione_Precedente_Rimanente, 0)
                                            Dose_Tot_Old = Dose
                                        Case Else
                                            Dose_Tot_New = Dose * Sup_Tot_Operazione_Precedente_Rimanente
                                            Dose_Tot_Old = Dose * Sup_Tot_Operazione_Precedente
                                    End Select


                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                        Case enum_UnitaMisura.Grammi  'g
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.Milligrammi
                                            Dose_Trasformata_New = Dose / 1000000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000000
                                        Case enum_UnitaMisura.Quintali
                                            Dose_Trasformata_New = Dose * 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 100
                                        Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Metri_Cubi
                                            Dose_Trasformata_New = Dose * 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 1000
                                        Case enum_UnitaMisura.Millilitri
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.CentimetriCubi
                                            Dose_Trasformata_New = Dose / 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 100
                                        Case Else
                                            Dose_Trasformata_New = Dose
                                            DoseTotaleTrasformata_New = Dose_Tot_New
                                    End Select


                                    If Fr_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Fr_Cod) Then
                                            HashTotNew.Add(Fr_Cod, DoseTotaleTrasformata_New)
                                        End If
                                    ElseIf Mat_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            HashTotNew.Add(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto, DoseTotaleTrasformata_New)
                                        End If
                                    End If

                                    Dose_New = 0
                                    Dose_New_Hl = 0

                                    Select Case Lav_Cod

                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                         LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                         LAVCOD_RACCOLTA
                                            'qta è quella totale distribuita
                                            Dose_New = Dose_Tot_New
                                            'modifico la qta del dettaglio (dose_ha)
                                            objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                     0, 0, 0, 0,
                                                                     Dose_New,
                                                                     0, 0, 0, 0, 0,
                                                                     "", "",
                                                                     objParametri_Server)

                                        Case LAVCOD_IRRIGAZIONE,
                                         LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                        Case Else

                                            Dose_New = Dose

                                            If AcquaTot <> 0 Then
                                                Dose_New_Hl = Dose_New * Sup_Tot_Operazione_Precedente_Rimanente / AcquaTot
                                            End If

                                            'modifico la qta_extra e la qta_extra_tot del dettaglio (dose_hl, qta tot)
                                            objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                     0, 0, 0, 0,
                                                                     Dose_New, Dose_New_Hl, Dose_Tot_New,
                                                                     0, 0, 0,
                                                                     "", "",
                                                                     objParametri_Server)

                                    End Select



                                    'MOVIMENTI_DESTINAZIONI
                                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                            Dim SupTrattata As Decimal = 0
                                            Dim AppDaModificare As Boolean = True
                                            For Each imp_da_spostare As DataRow In DrImpDaSpostare
                                                If imp_da_spostare.Item("Piva") = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva And
                                                  imp_da_spostare.Item("Sa_Cod") = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod And
                                                  imp_da_spostare.Item("Appezza") = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza And
                                                  imp_da_spostare.Item("Id_Destinazione") = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then
                                                    AppDaModificare = False
                                                    Exit For
                                                End If
                                            Next

                                            If AppDaModificare = True Then

                                                Dim QuotaDistribuzioneNew As Decimal = 0
                                                If Sup_Tot_Operazione_Precedente_Rimanente <> 0 Then
                                                    QuotaDistribuzioneNew = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2 / Sup_Tot_Operazione_Precedente_Rimanente
                                                End If

                                                Select Case Lav_Cod

                                                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                                        objDest.Modifica_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2,
                                                                                     QuotaDistribuzioneNew,
                                                                                     "", objParametri_Server)
                                                    Case Else
                                                        Select Case Lav_Cod
                                                            Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                Qta_Dest_New = Math.Round(Dose * Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2, 0)
                                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                 LAVCOD_RACCOLTA
                                                                'dose salvata sempre qta totale
                                                                Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_Operazione_Precedente_Rimanente) * Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2, 0)
                                                                Dose_Tot_New = Math.Round(Dose_New, 0)
                                                            Case LAVCOD_IRRIGAZIONE
                                                                Select Case Dett_Cod
                                                                    Case enum_UnitaMisura.Millimetri
                                                                        Dose = Dose * 10
                                                                    Case enum_UnitaMisura.METRI3__HA
                                                                        Dose = Dose * 1
                                                                    Case Else
                                                                        Dose = 0
                                                                End Select
                                                                Qta_Dest_New = Dose * Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

                                                            Case Else
                                                                Qta_Dest_New = Dose_Trasformata_New * Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2
                                                        End Select

                                                        'modifico qta_destinazione e sup_trattata
                                                        objDest.Modifica_Quantita_e_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                                                Qta_Dest_New,
                                                                                                Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2,
                                                                                                QuotaDistribuzioneNew,
                                                                                                "", objParametri_Server)

                                                End Select

                                            End If
                                        Next
                                    End If
                                Next
                            End If
                    End Select
                Next


                'MOVIMENTO SCARICO
                'modifico la qta del dettaglio e la qta destinazione (magazzino)
                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_SCARICO, CAU_CARICO

                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                                    Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto
                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto

                                    Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    DoseTotaleTrasformata_New = 0

                                    If Fr_Cod <> 0 Then
                                        If HashTotNew.ContainsKey(Fr_Cod) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Fr_Cod)
                                        End If
                                    Else
                                        If HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto)
                                        End If
                                    End If

                                    If DoseTotaleTrasformata_New <> 0 Then

                                        'modifico la qta del dettaglio 
                                        objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                 0, 0,
                                                                 0, 0,
                                                                 DoseTotaleTrasformata_New,
                                                                 0, 0,
                                                                 0, 0, 0,
                                                                 "", "",
                                                                 objParametri_Server)


                                        'MOVIMENTI_DESTINAZIONI
                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                            For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                                'modifico qta_destinazione 
                                                objDest.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                          Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                          DoseTotaleTrasformata_New,
                                                                          "", objParametri_Server)

                                            Next
                                        End If
                                    End If
                                Next
                            End If
                    End Select
                Next
            End If
        End If

        'elimino le destinazioni dalla vecchia operazione
        If Not IsNothing(Agenda) AndAlso Not IsNothing(Agenda.Movimenti) Then
            For i = 0 To Agenda.Movimenti.Count - 1
                Select Case Agenda.Movimenti(i).Cau_Mov
                    Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA
                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then
                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                                Dim EliminataDestinazione As Boolean = False
                                If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                    For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                        For Each imp_da_eliminare As DataRow In DrImpDaSpostare
                                            If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva = imp_da_eliminare.Item("Piva") And
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod = imp_da_eliminare.Item("Sa_Cod") And
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza = imp_da_eliminare.Item("Appezza") And
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione = imp_da_eliminare.Item("Id_Destinazione") Then

                                                objDest.Cancella(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                 Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                 "", objParametri_Server)
                                                EliminataDestinazione = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If
                                'se il dettaglio aveva una sola destinazione (alcune operazioni) che viene eliminata
                                'elimino anche lui
                                If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count = 1 And EliminataDestinazione = True Then
                                    objDet.Cancella(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod,
                                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                    Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                    "", objParametri_Server)
                                End If
                            Next
                        End If
                End Select
            Next
        End If


        'aggiorno il log
        Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
        objAgronicaLogAgendaW.Scrivi(Agenda.Data,
                                     enum_TipoOperazioneDB.Modifica,
                                     Agenda.Des_Lib,
                                     Agenda.Id_Agenda,
                                     Agenda.Piva,
                                     Agenda.Sa_Cod,
                                     Agenda.Lav_Cod,
                                     CInt(enum_Id_Servizio.GiasOnline),
                                     objParametri_Server)


        objAgronicaLogAgendaW = Nothing


    End Sub

    Public Sub SistemaAgenda(PIVA As String,
                        HashIdAgendaDaSistemareTmp As Hashtable,
                        HashIdAgendaDaSistemare As Hashtable,
                        DtAgendeDaSistemare As DataTable,
                        DTAgendeTot As DataTable,
                        ht_CDG As Hashtable,
                        objParametri_Server As AgronicaCoreParametri
                        )

        For Each id_agenda As Integer In HashIdAgendaDaSistemareTmp.Keys
            Dim EsisteImp As Boolean = False
            Dim DrImp1() As DataRow = DtAgendeDaSistemare.Select("id_agenda=" & id_agenda)
            Dim DrImp2() As DataRow = DTAgendeTot.Select("id_agenda=" & id_agenda)

            For Each drTotTmp As DataRow In DrImp2
                EsisteImp = False
                For Each drImpTmp As DataRow In DrImp1
                    If (drTotTmp.Item("id_agenda") = drImpTmp.Item("id_agenda") And
                            drTotTmp.Item("piva") = drImpTmp.Item("piva") And
                            drTotTmp.Item("sa_cod") = drImpTmp.Item("sa_cod") And
                            drTotTmp.Item("appezza") = drImpTmp.Item("appezza") And
                            drTotTmp.Item("Id_Destinazione") = drImpTmp.Item("Id_Destinazione")) Then
                        EsisteImp = True
                        Exit For
                    End If
                Next
                If EsisteImp = False Then
                    If Not HashIdAgendaDaSistemare.ContainsKey(id_agenda) Then
                        HashIdAgendaDaSistemare.Add(id_agenda, "")
                    End If
                End If
            Next
        Next


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Dim AgendaNew As Operazione_Agenda

        Dim objDetTecn As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W


        Dim Sup_Tot_Operazione_Precedente As Decimal
        Dim Sup_Tot_Operazione_Precedente_Rimanente As Decimal
        Dim Sup_Tot_Operazione_Precedente_DaSpostare As Decimal

        Dim DtApp_Old As New DataTable
        Dim DtApp_New As New DataTable

        DtApp_Old = DtAgendeDaSistemare.Clone
        DtApp_New = DtAgendeDaSistemare.Clone

        Dim scrivi_BIZ2 As New AgronicaCoreContabBIZ.CDG_BIZ_W

        For Each id_agenda_da_sistemare As Integer In HashIdAgendaDaSistemare.Keys

            DtApp_Old.Rows.Clear()
            DtApp_New.Rows.Clear()

            Sup_Tot_Operazione_Precedente = 0
            Sup_Tot_Operazione_Precedente_Rimanente = 0
            Sup_Tot_Operazione_Precedente_DaSpostare = 0

            'DESTINAZIONI DA ELIMINARE DALLA PRECEDENTE E METTERE NELLA NUOVA
            Dim DrImpDaSpostare() As DataRow = DtAgendeDaSistemare.Select("id_agenda=" & id_agenda_da_sistemare)
            For Each imp_da_spostare As DataRow In DrImpDaSpostare
                Sup_Tot_Operazione_Precedente_DaSpostare += CDec(imp_da_spostare.Item("Qta2"))
                DtApp_New.ImportRow(imp_da_spostare)
            Next

            'DESTINAZIONI PRECEDENTI TOTALI
            Dim DrImpTotaliPrecedenti() As DataRow = DTAgendeTot.Select("id_agenda=" & id_agenda_da_sistemare)
            For Each imp_tot_precedenti As DataRow In DrImpTotaliPrecedenti
                Sup_Tot_Operazione_Precedente += CDec(imp_tot_precedenti.Item("Qta2"))
            Next

            Sup_Tot_Operazione_Precedente_Rimanente = Sup_Tot_Operazione_Precedente - Sup_Tot_Operazione_Precedente_DaSpostare

            'operazione precedente da sistemare
            Agenda = objAgenda.Leggi(PIVA, 0,
                                     id_agenda_da_sistemare,
                                     0,
                                     objParametri_Server)

            '1. operazione nuova da creare faccio una copia e la creo subito
            '2. modifico la vecchia operazione per eliminare le destinazioni che cambiano con la semina
            '3. modifico la nuova operazione creata per eliminare le destinazioni vecchie rimaste sulla nuova

            '----------------------------------------------------------------
            '1. operazione nuova da creare faccio una copia e la creo subito

            AgendaNew = Agenda.DeepCloneObject
            AgendaNew.Id_Agenda = 0
            AgendaNew.Tipo_Operazione = 1

            If Not AgendaNew.Agenda_Riferimenti Is Nothing AndAlso AgendaNew.Agenda_Riferimenti.Count > 0 Then
                AgendaNew.Agenda_Riferimenti.Clear()
            End If
            For Each movimento As Movimento In AgendaNew.Movimenti
                movimento.Id_Agenda = 0
                movimento.Id_Mov = 0
                For Each movimento_dettaglio_tecnico As Movimento_Dettaglio_Tecnico In movimento.Movimenti_Dettagli_Tecnici
                    movimento_dettaglio_tecnico.Id_Agenda = 0
                    movimento_dettaglio_tecnico.Id_Mov = 0
                    movimento_dettaglio_tecnico.Id_Reg_Dettaglio = 0
                Next
                For Each movimento_dettaglio As Movimento_Dettaglio In movimento.Movimenti_Dettagli
                    movimento_dettaglio.Id_Agenda = 0
                    movimento_dettaglio.Id_Mov = 0
                    movimento_dettaglio.Id_Mov_Det = 0
                    For Each movimento_dettaglio_tecnico2 As Movimento_Dettaglio_Tecnico In movimento_dettaglio.Movimenti_Dettagli_Tecnici
                        movimento_dettaglio_tecnico2.Id_Agenda = 0
                        movimento_dettaglio_tecnico2.Id_Mov = 0
                        movimento_dettaglio_tecnico2.Id_Mov_Det = 0
                        movimento_dettaglio_tecnico2.Id_Reg_Dettaglio = 0
                    Next
                    For Each movimento_destinazione As Movimento_Destinazione In movimento_dettaglio.Movimenti_Destinazioni
                        movimento_destinazione.Id_Agenda = 0
                        movimento_destinazione.Id_Mov = 0
                        movimento_destinazione.Id_Mov_Det = 0
                    Next
                    For Each movimento_dettaglio_riferimento As Movimento_Dettaglio_Riferimento In movimento_dettaglio.Movimenti_Dettagli_Riferimenti
                        movimento_dettaglio_riferimento.Id_Agenda = 0
                        movimento_dettaglio_riferimento.Id_Mov = 0
                        movimento_dettaglio_riferimento.Id_Mov_Det = 0
                    Next
                Next
            Next

            Dim Nuovo_Id_Agenda As Integer = 0
            Nuovo_Id_Agenda = objAgenda.Scrivi(AgendaNew, objParametri_Server)

            '--------------------------------------------------------------
            '2. modifico la vecchia operazione per eliminare le destinazioni che cambiano con la semina
            AggiornaOperazionePrecedente_PostFrazionamento(Agenda, Sup_Tot_Operazione_Precedente, Sup_Tot_Operazione_Precedente_Rimanente, DrImpDaSpostare, objParametri_Server)

            '--------------------------------------------------------------
            '3. modifico la nuova operazione creata per eliminare le destinazioni vecchie rimaste sulla nuova
            AgendaNew = objAgenda.Leggi(PIVA, 0,
                                     Nuovo_Id_Agenda,
                                     0,
                                     objParametri_Server)

            AggiornaOperazioneNuova_PostFrazionamento(AgendaNew, id_agenda_da_sistemare, Sup_Tot_Operazione_Precedente, Sup_Tot_Operazione_Precedente_DaSpostare, DrImpDaSpostare, DtApp_Old, objParametri_Server)

            Try

                Dim EFArrayToInsert As New ArrayList
                Dim Id_Agenda_CDG_Old As Integer = 0
                Dim Id_Agenda_CDG As Integer = 0
                Dim Des_Lib As String = ""
                Dim Data_Movimento As DateTime

                EFArrayToInsert.Clear()

                Id_Agenda_CDG_Old = scrivi_BIZ2.AggiornaCDG_Split_SeminaTrapianto(PIVA,
                                                                   id_agenda_da_sistemare,
                                                                   Nuovo_Id_Agenda,
                                                                   DtApp_Old,
                                                                   DtApp_New,
                                                                   EFArrayToInsert,
                                                                   Id_Agenda_CDG,
                                                                   Des_Lib,
                                                                   Data_Movimento,
                                                                   objParametri_Server)



                If Id_Agenda_CDG_Old <> 0 Then

                    'Inserimento CDG da creare
                    Dim myObj As New ObjCDG_DaInserire

                    myObj.EFArrayToInsert = EFArrayToInsert
                    myObj.Id_Agenda_CDG = Id_Agenda_CDG
                    myObj.Id_Agenda = Nuovo_Id_Agenda
                    myObj.Des_Lib = Des_Lib
                    myObj.Data_Movimento = Data_Movimento
                    myObj.Id_Agenda_Old = id_agenda_da_sistemare
                    myObj.Id_Agenda_CDG_Old = 0 'Non va cancellata

                    ht_CDG.Add(Id_Agenda_CDG_Old, myObj)

                End If

            Catch ex As Exception

                Throw New Exception("[SalvaModificheImpianti] :   " & ex.Message)

            End Try

        Next

    End Sub

#End Region


End Class
