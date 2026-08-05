'edx. if semina con frazionamento ri-controllo che l'utente abbia selezionato un solo impianto, nel caso contrario errore bloccante
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.Utility_Agenda
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelsSTD

Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo

Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.anagrafiche

Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports Newtonsoft.Json

Imports System.Transactions
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUtility

Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.exceptions


Public Class ValiditaFormaleOperazione
    Public Const AGRODATAINIZIO As Date = #1/1/1900#
    Public Const AGRODATAFINE As Date = #12/31/2100#

#Region "SEMINA CON FRAZIONAMENTO"
    Public Function Verifica_SeminaConFrazionamento(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                    parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                                    eseguiSoloVerificheConformita As Boolean,
                                                    currentAttivitaDes As String,
                                                    objParametri_Server As AgronicaCoreParametri,
                                                    objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                       Optional OpenNewTransaction As Boolean = True,
                                                       Optional ByRef listAttivitaxDettaglioSemina As List(Of AgronicaCoreModelsSTD.attivita.Attivita) = Nothing,
                                                       Optional ByRef originalEsercizioCDC As EsercizioCDC = Nothing,
                                                       Optional ByRef DtNuoviImpianti As DataTable = Nothing
                                           ) As List(Of ErroreGias)

        Dim lista_Errori As New List(Of ErroreGias)
        Dim EsercizioCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))

        '===================================
        '   RICAVO DATI IMPIANTO
        '-----------------------------------
        Dim impiantoSelezionato As EsercizioCDC = EsercizioCDC(0)
        Dim PIVA As String
        Dim SA_COD, APPEZZA, ID_REG, _CAMPO_COD, PROGETTO_COD As Integer
        Dim sup_imp, sup_TerrenoNudo As Decimal

        Dim isAppezzamentoBloccato As Boolean
        Dim eseguiOperazioneSuImpiantoBloccato As Boolean

        PIVA = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        SA_COD = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
        APPEZZA = impiantoSelezionato.esercizio.impiantoPK.appezzamentoPK.codice
        _CAMPO_COD = (From a In GiasContext.Appezzamento Where a.PIVA = PIVA AndAlso a.SA_COD = SA_COD AndAlso a.APPEZZA = APPEZZA Select a.Campo_Cod).FirstOrDefault
        ID_REG = impiantoSelezionato.esercizio.impiantoPK.codice
        PROGETTO_COD = impiantoSelezionato.esercizio.codice



        Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
        Dim impianto = objImpianti_R.Leggi_Impianto_Anagrafica(PIVA, SA_COD, APPEZZA, ID_REG,
                                                               False, False,
                                                               AGRODATAINIZIO, filtroData:=False, Leggi_Cartografia:=False,
                                                               Nothing, objParametri_Server, objParametri_Utenti)
        If IsNothing(impianto) Then
            'NON DOVREBBE MAI ENTRARE QUI
            Dim messaggio As String = "Impianto non trovato."
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))

            Return lista_Errori
        End If

        sup_imp = impianto.superficie



        '===================================
        '   CONTROLLO IMPIANTO BLOCCATO
        '-----------------------------------
        'Controllo il flag sull'impianto bloccato e se l'utente ha il permesso per eseguirci operazioni
        eseguiOperazioneSuImpiantoBloccato = Get_ImpostazioneUtente_EseguiOperazioneSuImpiantoBloccato(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI, objParametri_Utenti)
        isAppezzamentoBloccato = Get_isAppezzamentoBloccato(PIVA, SA_COD, APPEZZA, _CAMPO_COD, objParametri_Utenti, GiasContext)


        '===================================
        '   RICAVO LISTA PRODOTTI E DETTAGLI
        '-----------------------------------
        'Dettagli Semina 
        Dim risorsaProdottoList As List(Of RisorsaProdotto) =
                attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, RisorsaProdotto))

        '=========================================
        '   CONTROLLO VALIDITA' SPECIE PRODOTTI
        '-----------------------------------------
        Dim prodottoIncongruente As New List(Of String)
        For Each risorsa In risorsaProdottoList
            Dim DettaglioSemina = CType(risorsa, dettagli.DettaglioSemina)
            'Controllo che il prodotto selezionato abbia una specie indicata
            If DettaglioSemina.varieta.specie.codice = 0 Then
                prodottoIncongruente.Add(DettaglioSemina.prodotto.descrizione)
            End If
        Next
        If prodottoIncongruente.Count > 0 Then
            Dim messaggio As String = ""
            If prodottoIncongruente.Count = 1 Then
                messaggio = String.Format(Gias.ImpossibileUtilizzareProdottoXSpecieNonImpostataAnagrafica, prodottoIncongruente(0))
            Else
                messaggio = Gias.ImpossibileUtilizzareSeguentiProdottiSpecieNonImpostataAnagrafica & ": " & NEWLINE & "- " & String.Join(NEWLINE & "- ", prodottoIncongruente)
            End If

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, "", messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))

            Return lista_Errori
        End If

        'RISORSE USATE (raggruppate per Mat_Cod) x la loro SUP_TRATTATA
        Dim SupTrattata_x_DettaglioSeminaList As List(Of SupTrattata_x_DettaglioSemina) = ricavaSupTrattata_x_MatCod(parametriAggiuntiviList, sup_imp, sup_TerrenoNudo)


        If eseguiSoloVerificheConformita Then
            '===================================
            '   WARNING FRAZIONAMENTO
            '-----------------------------------
            Dim objErroreGias = GeneraWarningFrazionamento(eseguiOperazioneSuImpiantoBloccato, isAppezzamentoBloccato,
                                                            risorsaProdottoList, SupTrattata_x_DettaglioSeminaList,
                                                            sup_TerrenoNudo, currentAttivitaDes)
            lista_Errori.Add(objErroreGias)

        Else
            '===================================
            '   FRAZIONAMENTO
            '-----------------------------------
            Frazionamento(attivita, risorsaProdottoList, SupTrattata_x_DettaglioSeminaList,
                          PIVA, SA_COD, APPEZZA, _CAMPO_COD, ID_REG, PROGETTO_COD,
                          sup_TerrenoNudo, originalSupImpianto:=sup_imp,
                          isAppezzamentoBloccato,
                          listAttivitaxDettaglioSemina:=listAttivitaxDettaglioSemina,
                          originalEsercizioCDC:=originalEsercizioCDC,
                          DtNuoviImpianti:=DtNuoviImpianti,
                          objParametri_Server, objParametri_Utenti,
                          GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)
        End If

        Return lista_Errori

    End Function

    ''' <param name="listAttivitaxDettaglioSemina"></param> Lista di micro attività divise per MatCod, da rimandare al chiamante per scrivere le agende
    ''' <param name="originalEsercizioCDC"></param> Chiavi impianto originale da rimandare al chiamante per frazionare agende precedenti
    ''' <param name="DtNuoviImpianti"></param> DT dei nuovi impianti creati da rimandare al chiamante per frazionare agende precedenti
    Private Shared Sub Frazionamento(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                     risorsaProdottoList As List(Of RisorsaProdotto),
                                     SupTrattata_x_DettaglioSeminaList As List(Of SupTrattata_x_DettaglioSemina),
                                     piva As String,
                                     sa_cod As Integer,
                                     appezza As Integer,
                                     campo_cod As Integer,
                                     id_reg As Integer,
                                     progetto_cod As Integer,
                                     sup_TerrenoNudo As Decimal,
                                     originalSupImpianto As Decimal,
                                     isAppezzamentoBloccato As Boolean,
                                     ByRef listAttivitaxDettaglioSemina As List(Of AgronicaCoreModelsSTD.attivita.Attivita),
                                     ByRef originalEsercizioCDC As EsercizioCDC,
                                     ByRef DtNuoviImpianti As DataTable,
                                     objParametri_Server As AgronicaCoreParametri,
                                     objParametri_Utenti As AgronicaCoreParametri,
                                        Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                        Optional ByVal OpenNewTransaction As Boolean = True
                                     )

        Dim nomeRoutine As String = "[ValiditaFormaleOperazione.Frazionamento()]"



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

        Dim appezza_old = appezzamento.primaryKey.codice
        Dim id_reg_old = appezzamento.impianti(0).primaryKey.codice

        Dim validita As New IntervalloTemporale(appezzamento.validita.inizio, appezzamento.validita.fine)

        'Se il nome dell'appezzamento ha più di 495 caratteri lo devo accorciare
        Dim descrizioneAppezzamento As String = appezzamento.descrizione.Substring(0, IIf(appezzamento.descrizione.Length > 495, 495, appezzamento.descrizione.Length))
        Dim primoAppezzamento As Boolean = True
        Dim isTerrenoNudo As Boolean = False
        Dim nrApp As Integer = 1

        Dim NoteLog As String = "Semina con frazionamento (NG)"
        Dim newEsercizioCDC As New Esercizio

        Try

            For Each prodotto In SupTrattata_x_DettaglioSeminaList

                Dim risorsa As List(Of RisorsaProdotto) = risorsaProdottoList.Where(Function(p) p.prodotto.codice = prodotto.Mat_Cod).ToList

                'Il primo prodotto modifica l'appezzamento esistente, il resto ne crea di nuovi
                If primoAppezzamento Then
                    primoAppezzamento = False

                    'Se l'impianto selezionato non è bloccato da anagrafica, eseguo la modifica
                    If Not (isAppezzamentoBloccato) Then
                        'Le risorse sono raggruppate per mat_cod, mi basta passare il primo
                        '(i prodotti sono identici, tranne per il magazzino e lotto che però non influiscono sulle anagrafiche)
                        ModificaAnagrafichePostOperazione.Modifica_Appezzamento(descrizioneAppezzamento, nrApp, newEsercizioCDC, appezzamento,
                                                                                id_reg, progetto_cod,
                                                                                risorsa(0), prodotto.Sup_Trattata,
                                                                                objParametri_Server, objParametri_Utenti,
                                                                                GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                                NoteLog:=NoteLog)
                    End If

                    'Creo un EsercizioCDC con l'impianto originale, da passare al chiamante di livello più alto per eseguire il frazionamento delle agende registrate sull'impianto originale
                    Dim primoEsercizioCDC As New EsercizioCDC With {
                                .esercizio = newEsercizioCDC,
                                .superficieTrattata = prodotto.Sup_Trattata
                            }
                    originalEsercizioCDC = primoEsercizioCDC


                Else
                    'Le risorse sono raggruppate per mat_cod, mi basta passare il primo
                    '(i prodotti sono identici, tranne per il magazzino e lotto che però non influiscono sulle anagrafiche)
                    ModificaAnagrafichePostOperazione.Crea_Appezzamento(nrApp, newEsercizioCDC, appezzamento, piva, sa_cod, campo_cod, validita, risorsa(0),
                                                                        descrizioneAppezzamento, prodotto.Sup_Trattata, isTerrenoNudo,
                                                                        objParametri_Server, objParametri_Utenti,
                                                                        GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)

                End If

                Dim AttivitaxDettaglioSemina = scriviAttivita_singoloDettaglio(attivita, newEsercizioCDC, risorsa, prodotto.Sup_Trattata)
                listAttivitaxDettaglioSemina.Add(AttivitaxDettaglioSemina)
                CreaAggiornaDtNuoviImpianti(DtNuoviImpianti, newEsercizioCDC, appezza_old, id_reg_old, prodotto.Sup_Trattata, originalSupImpianto)

            Next



            If sup_TerrenoNudo > 0 Then
                isTerrenoNudo = True
                ModificaAnagrafichePostOperazione.Crea_Appezzamento(nrApp, newEsercizioCDC, appezzamento, piva, sa_cod, campo_cod, validita, Nothing,
                                                                    descrizioneAppezzamento, sup_TerrenoNudo, isTerrenoNudo,
                                                                    objParametri_Server, objParametri_Utenti,
                                                                    GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)

                CreaAggiornaDtNuoviImpianti(DtNuoviImpianti, newEsercizioCDC, appezza_old, id_reg_old, sup_TerrenoNudo, originalSupImpianto)
            End If

        Catch ex As Exception
            Throw New Exception(nomeRoutine & ": " & ex.Message)
        End Try

    End Sub

#End Region

#Region "SEMINA CON AGGIORNAMENTO ANAGRAFICA"

    Public Function Verifica_SeminaConAggiornamentoAnagrafica(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                              eseguiSoloVerificheConformita As Boolean,
                                                              currentAttivitaDes As String,
                                                              ByRef DT_AgendeDaSistemare As DataTable,
                                                              ByRef DT_AgendeTot As DataTable,
                                                              ByRef HashIdAgendaDaSistemareTmp As Hashtable,
                                                              objParametri_Server As AgronicaCoreParametri,
                                                              objParametri_Utenti As AgronicaCoreParametri,
                                                                   Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                                   Optional OpenNewTransaction As Boolean = True
                                                              ) As List(Of ErroreGias)






        Dim lista_Errori As New List(Of ErroreGias)


        '===================================
        '   RICAVO LISTA IMPIANTI 
        '-----------------------------------
        Dim EsercizioCDC As List(Of EsercizioCDC) = attivita.centriDiCosto.FindAll(Function(c) (c.classType = ClassType.EsercizioCDC)).ConvertAll(Function(obj1) CType(obj1, EsercizioCDC))


        '===================================
        '   RICAVO LISTA PRODOTTI 
        '-----------------------------------
        Dim risorsaProdottoList As List(Of RisorsaProdotto) =
                attivita.risorse.FindAll(Function(c) (c.classType = ClassType.DettaglioSemina)).ConvertAll(Function(obj1) CType(obj1, RisorsaProdotto))
        'Raggruppo i prodotti per Mat_Cod
        Dim risorsa As List(Of RisorsaProdotto) = risorsaProdottoList.GroupBy(Function(x) x.prodotto.codice).Select(Function(x) x.First).ToList


        '=========================================
        '   CONTROLLO NUMERO PRODOTTI SELEZIONATI
        '-----------------------------------------
        If risorsa.Count > 1 Then
            'Posso seminare prodotti solo con lo stesso Mat_Cod (e lotto diverso)
            Dim messaggio = Gias.PerModificareImpiantiNecessarioSelezionareUnTipoSementePiantina
            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))

            Return lista_Errori
        End If


        '=========================================
        '   CONTROLLO VALIDITA' SPECIE PRODOTTO
        '-----------------------------------------
        'Controllo che il prodotto selezionato abbia una specie indicata
        Dim DettaglioSemina = CType(risorsa(0), dettagli.DettaglioSemina)
        If DettaglioSemina.varieta.specie.codice = 0 Then
            Dim messaggio = String.Format(Gias.ImpossibileUtilizzareProdottoXSpecieNonImpostataAnagrafica, DettaglioSemina.prodotto.descrizione)

            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))

            Return lista_Errori
        End If

        '=======================================
        '   CONTROLLI CONFORMITA DA FARE SEMPRE
        '---------------------------------------

        'Controlli modifica impianto...
        '- Se le operazioni precedenti sono con altri impianti queste diventerebbero multispecie e quindi inconsistenti in GIAS e quindi blocco
        '- Se ci sono operazioni di tipo trattamento/diserbo, blocco perché potrebbero diventare inconsistenti (vincono rilassabile dopo opportune verifiche)
        '- se l'impianto è visibile perchè permetto le operazioni su bloccato devo impedire la modifica dell'impianto


        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Blocca_SeminaConCambioSpecie_SePresentiOperazioni_SuApp As Boolean = objConfigSiti.Recupera_Valore_ByChiave(0, "Blocca_SeminaConCambioSpecie_SePresentiOperazioni_SuApp", objParametri_Server)
        Dim eseguiOperazioneSuImpiantoBloccato As Boolean = Get_ImpostazioneUtente_EseguiOperazioneSuImpiantoBloccato(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI, objParametri_Utenti)

        For Each impianto In EsercizioCDC

            '===================================
            '   RICAVO DATI IMPIANTO
            '-----------------------------------
            Dim esercizio = impianto.esercizio
            Dim PIVA As String
            Dim SA_COD, APPEZZA, ID_REG, _CAMPO_COD, PROGETTO_COD As Integer

            PIVA = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
            SA_COD = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
            APPEZZA = esercizio.impiantoPK.appezzamentoPK.codice
            _CAMPO_COD = (From a In GiasContext.Appezzamento Where a.PIVA = PIVA AndAlso a.SA_COD = SA_COD AndAlso a.APPEZZA = APPEZZA Select a.Campo_Cod).FirstOrDefault
            ID_REG = esercizio.impiantoPK.codice
            PROGETTO_COD = esercizio.codice

            Dim objImpianti_R As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            Dim imp = objImpianti_R.Leggi_Impianto_Anagrafica(PIVA, SA_COD, APPEZZA, ID_REG,
                                                                  False, False,
                                                                  AGRODATAINIZIO, filtroData:=False, Leggi_Cartografia:=False,
                                                                  Nothing, objParametri_Server, objParametri_Utenti)

            Dim Veg_Cod_OLD As Integer = 0
            Select Case imp.utilizzoTerreno.classType
                Case costanti.ClassType.Varieta
                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Veg_Cod_OLD = objCultivar.VegCod_from_CulCod(imp.utilizzoTerreno.codice, objParametri_Server)
            End Select

            Dim Veg_Cod_New As String = DettaglioSemina.varieta.specie.codice

            '===================================
            '   CONTROLLI CAMBIO SPECIE
            '-----------------------------------
            If (Veg_Cod_OLD <> Veg_Cod_New) Then

                '===================================
                '   RICAVO MOVIMENTI AGENDA IMPIANTO
                '-----------------------------------
                Dim ObjAgendaMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                Dim DTAgenda As DataTable = ObjAgendaMovDest.LeggiCronologiaMovimenti(PIVA, SA_COD, APPEZZA, ID_REG,
                                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      "", "",
                                                                                      objParametri_Server)

                'Se le operazioni precedenti sono con altri impianti queste diventerebbero multispecie e quindi inconsistenti in GIAS e quindi blocco
                Dim piuApp As Boolean = False
                Dim objMovDes As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                For Each rowAgenda As DataRow In DTAgenda.Rows
                    Dim DT_Dest As DataTable = objMovDes.Leggi_Dettagli_Impianti_2(rowAgenda.Item("Piva"),
                                                                                   rowAgenda.Item("Id_Agenda"),
                                                                                   "", "",
                                                                                   objParametri_Server)
                    For Each dest As DataRow In DT_Dest.Rows

                        If Not (eseguiSoloVerificheConformita) Then
                            Dim DrTmp() As DataRow = DT_AgendeTot.Select("Id_Agenda = " &
                                                                         rowAgenda.Item("Id_Agenda") &
                                                                         " AND piva = '" & dest.Item("piva") & "'" &
                                                                         " AND sa_cod = " & dest.Item("sa_cod") &
                                                                         " AND appezza = " & dest.Item("appezza") &
                                                                         " AND id_destinazione = " & dest.Item("Id_Destinazione"))
                            If DrTmp.Length = 0 Then
                                Dim DrTot As DataRow = DT_AgendeTot.NewRow
                                DrTot.Item("Id_Agenda") = rowAgenda.Item("Id_Agenda")
                                DrTot.Item("Piva") = dest.Item("piva")
                                DrTot.Item("Sa_Cod") = dest.Item("sa_cod")
                                DrTot.Item("Appezza") = dest.Item("appezza")
                                DrTot.Item("Id_Destinazione") = dest.Item("Id_Destinazione")
                                DrTot.Item("Qta2") = dest.Item("Qta2")
                                DT_AgendeTot.Rows.Add(DrTot)
                            End If
                        End If

                        If CInt(dest.Item("appezza")) <> APPEZZA Then
                            piuApp = True
                            If eseguiSoloVerificheConformita Then
                                Exit For
                            End If
                        End If
                    Next



                    If piuApp Then

                        If Blocca_SeminaConCambioSpecie_SePresentiOperazioni_SuApp = True Then

                            Dim messaggio As String = Gias.VariataSpecieImpiantoMaMovimentiAssociatiImpossibileModificareSpecie
                            lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))

                        Else

                            If Not HashIdAgendaDaSistemareTmp.ContainsKey(rowAgenda.Item("Id_Agenda")) Then
                                HashIdAgendaDaSistemareTmp.Add(rowAgenda.Item("Id_Agenda"), "")
                            End If

                            Dim Dr As DataRow = DT_AgendeDaSistemare.NewRow
                            Dr.Item("Id_Agenda") = rowAgenda.Item("Id_Agenda")
                            Dr.Item("Piva") = PIVA
                            Dr.Item("Sa_Cod") = SA_COD
                            Dr.Item("Appezza") = APPEZZA
                            Dr.Item("Id_Destinazione") = ID_REG
                            'Dr.Item("Qta2") = impianto.Qta2

                            For Each rowAgendeTot As DataRow In DT_AgendeTot.Rows
                                If rowAgendeTot.Item("id_agenda") = rowAgenda.Item("Id_Agenda") And
                                    rowAgendeTot.Item("piva") = PIVA And rowAgendeTot.Item("sa_cod") = SA_COD And
                                    rowAgendeTot.Item("appezza") = APPEZZA And rowAgendeTot.Item("id_destinazione") = ID_REG Then

                                    Dr.Item("Qta2") = rowAgendeTot.Item("qta2")
                                    Exit For

                                End If
                            Next
                            DT_AgendeDaSistemare.Rows.Add(Dr)

                        End If

                    End If

                Next

                'TO DO...??
                'METTERLO COME RAMO ELSE DI  If piuApp Then.. A PRESCINDERE CHE SIA UN TRATTAMENTO O MENO 
                If Blocca_SeminaConCambioSpecie_SePresentiOperazioni_SuApp = True Then
                    'Se ci sono operazioni di tipo trattamento/diserbo, blocco perché potrebbero diventare inconsistenti (vincono rilassabile dopo opportune verifiche)
                    Dim existsTrattamento As Boolean = ciSonoTrattamentiDiserbi(PIVA, SA_COD, APPEZZA, ID_REG, objParametri_Server)
                    If existsTrattamento = True Then
                        Dim messaggio As String = Gias.VariataSpecieImpiantoMaTrattamentiAssociatiImpossibileModificareSpecie
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.WarningBloccante, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
                    End If

                End If
            End If


            '===================================
            '   CONTROLLO IMPIANTO BLOCCATO
            '-----------------------------------
            'Controllo il flag sull'impianto bloccato e se l'utente ha il permesso per eseguirci operazioni lo mostro solo se eseguiSoloVerificheConformita = true
            Dim isAppezzamentoBloccato As Boolean = Get_isAppezzamentoBloccato(PIVA, SA_COD, APPEZZA, _CAMPO_COD, objParametri_Utenti, GiasContext)
            If eseguiSoloVerificheConformita Then
                If eseguiOperazioneSuImpiantoBloccato AndAlso isAppezzamentoBloccato Then
                    Dim messaggio = Gias.ImpiantoSelezionatoRisultaBloccatoInAnagrafica & NEWLINE &
                        Gias.OperazioneVerraRegistrataMaDatiImpiantoNonVerrannoModificati & NEWLINE
                    lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Warning, currentAttivitaDes, messaggio, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
                End If

            Else

                '===================================
                '   AGGIORNAMENTO ANAGRAFICA
                '-----------------------------------
                If Not (isAppezzamentoBloccato) Then
                    Try
                        ModificaAnagrafichePostOperazione.AggiornamentoAnagrafica(risorsa(0),
                                                                              PIVA, SA_COD, APPEZZA, ID_REG, PROGETTO_COD,
                                                                              objParametri_Server, objParametri_Utenti,
                                                                              GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction)
                    Catch ex As GiasException
                        lista_Errori.Add(generaErroreGias(ErroreGias_Severity.Bloccante, currentAttivitaDes, ex.Message, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda))
                    End Try
                End If
            End If

        Next

        Return lista_Errori

    End Function



#End Region





#Region "UTILITY"

    Private Shared Function scriviAttivita_singoloDettaglio(attivita As AgronicaCoreModelsSTD.attivita.Attivita,
                                                            esercizioCDC As Esercizio,
                                                            listaRisorse As List(Of RisorsaProdotto),
                                                            sup_trattata As Decimal
                                                            ) As AgronicaCoreModelsSTD.attivita.Attivita

        'Serializzo e Deserializzo l'Attività per copiare l'oggetto senza riferimenti
        Dim attivitaSerialized As String = JsonConvert.SerializeObject(attivita)
        Dim AttivitaxDettaglioSemina As AgronicaCoreModelsSTD.attivita.Attivita = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.Attivita)(attivitaSerialized)


        Try
            'Rimuovo tutte le risorse e aggiungo quelle passate
            AttivitaxDettaglioSemina.risorse.Clear()

            For Each risorsa In listaRisorse
                Dim dettaglioSemina = New dettagli.DettaglioSemina
                dettaglioSemina.prodotto = New Prodotto(risorsa.prodotto.codice)
                dettaglioSemina = risorsa
                AttivitaxDettaglioSemina.risorse.Add(dettaglioSemina)
            Next



            'Rimuovo tutti i CdG e aggiungo quello passato
            AttivitaxDettaglioSemina.centriDiCosto.Clear()
            AttivitaxDettaglioSemina.centriDiCosto.Add(New EsercizioCDC With {
                                                       .esercizio = esercizioCDC,
                                                       .superficieTrattata = sup_trattata
                                                       })


        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return AttivitaxDettaglioSemina

    End Function

    Private Shared Function Get_ImpostazioneUtente_EseguiOperazioneSuImpiantoBloccato(impostazione_cod As enum_Impostazioni_Utenti,
                                                                                      objParametri_Utenti As AgronicaCoreParametri
                                                                                      ) As Boolean

        Dim EseguiOperazione As Boolean = False

        Dim objImpostazioni_Utenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT = objImpostazioni_Utenti.Leggi(impostazione_cod, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If DT.Rows.Count > 0 Then
            EseguiOperazione = CBool(DT.Rows(0)("Impostazione_Valore_1"))
        End If

        Return EseguiOperazione
    End Function

    Private Shared Function Get_isAppezzamentoBloccato(piva As String,
                                                       sa_cod As Integer,
                                                       appezza As Integer,
                                                       campo_cod As Integer,
                                                       objParametri_Utenti As AgronicaCoreParametri,
                                                       GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities
                                                       ) As Boolean

        Dim appezzamentoBloccato As Boolean = False

        Dim _Blk_Flag As Integer = (From a In GiasContext.Appezzamento
                                    Where a.PIVA = piva AndAlso
                                       a.SA_COD = sa_cod AndAlso
                                       a.APPEZZA = appezza AndAlso
                                       a.Campo_Cod = campo_cod
                                    Select a.Blk_Flag
                                       ).FirstOrDefault

        If _Blk_Flag = -1 Then
            appezzamentoBloccato = True
        End If

        Return appezzamentoBloccato

    End Function

    Private Shared Function GeneraWarningFrazionamento(eseguiOperazioneSuImpiantoBloccato As Boolean,
                                                       appezzamentoBloccato As Boolean,
                                                       risorsaProdottoList As List(Of RisorsaProdotto),
                                                       SupTrattata_x_DettaglioSeminaList As List(Of SupTrattata_x_DettaglioSemina),
                                                       sup_TerrenoNudo As Decimal,
                                                       currentAttivitaDes As String
                                                        ) As ErroreGias

        Dim warningAppezzamenti As String = ""

        '===================================
        ' MESSAGGIO IMPIANTO BLOCCATO
        '-----------------------------------
        'Se l'impianto originale è bloccato in anagrafica, allora non verranno apportate modifiche
        '-->  non lo includo del messaggio di frazionamento
        Dim isPrimo_salta As Boolean = False
        If eseguiOperazioneSuImpiantoBloccato AndAlso appezzamentoBloccato Then
            warningAppezzamenti += Gias.ImpiantoSelezionatoRisultaBloccatoInAnagrafica & NEWLINE
            warningAppezzamenti += Gias.OperazioneVerraRegistrataMaDatiImpiantoNonVerrannoModificati & NEWLINE
            isPrimo_salta = True
        End If


        '===================================
        ' WARNING GENERAZIONE APPEZZAMENTI
        '-----------------------------------
        'Messaggio di modifica anagrafica SE:
        '   - c'è almeno un prodotto e l'impianto non è bloccato
        '   - verrà creato un terreno nudo
        If (risorsaProdottoList.Count >= 1 AndAlso isPrimo_salta = False) OrElse (sup_TerrenoNudo > 0) Then
            warningAppezzamenti += Gias.VerrannoGeneratiSeguentiAppezzamenti & NEWLINE

            'Raggruppo i prodotti per codice per ottenere un messaggio unico per Mat_Cod
            Dim listProdotti_distinctCodice As List(Of RisorsaProdotto) = risorsaProdottoList.GroupBy(Function(x) x.prodotto.codice).Select(Function(x) x.First).ToList
            'Ciclo listProdotti_distinctCodice con SupTrattata_x_DettaglioSeminaList per trovare la rispettiva sup_trattata
            For Each risorsa In listProdotti_distinctCodice
                If isPrimo_salta = False Then
                    For Each prodotto In SupTrattata_x_DettaglioSeminaList
                        If prodotto.Mat_Cod = risorsa.prodotto.codice Then
                            warningAppezzamenti += "• " & risorsa.prodotto.descrizione & " (" & prodotto.Sup_Trattata.ToString() & " Ha)" & NEWLINE
                        End If
                    Next
                End If

                isPrimo_salta = False
            Next

            If sup_TerrenoNudo > 0 Then
                warningAppezzamenti += String.Format(Gias.WarningGenerazioneAppezzamentiTerrenoNudo, sup_TerrenoNudo.ToString()) & NEWLINE
            End If

            Dim objErroreGias As ErroreGias = generaErroreGias(ErroreGias_Severity.Warning, currentAttivitaDes, warningAppezzamenti, Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda)
            Return objErroreGias
        End If

    End Function

    Private Shared Sub CreaAggiornaDtNuoviImpianti(ByRef DtNuoviImpianti As DataTable,
                                                   EsercizioCDC As Esercizio,
                                                   appezza_old As Integer,
                                                   id_reg_old As Integer,
                                                   sup_trattata As Decimal,
                                                   sup_originale As Decimal
                                                   )

        'Creo il Dt da passare alla funzione originale della Trattamenti_2

        If DtNuoviImpianti.Columns.Count = 0 Then
            DtNuoviImpianti = New DataTable
            DtNuoviImpianti.Columns.Add("piva", GetType(String))
            DtNuoviImpianti.Columns.Add("sa_cod", GetType(Integer))
            DtNuoviImpianti.Columns.Add("appezza_old", GetType(Integer))
            DtNuoviImpianti.Columns.Add("id_reg_old", GetType(Integer))
            DtNuoviImpianti.Columns.Add("appezza_new", GetType(Integer))
            DtNuoviImpianti.Columns.Add("id_reg_new", GetType(Integer))
            DtNuoviImpianti.Columns.Add("sup_new", GetType(Decimal))
            DtNuoviImpianti.Columns.Add("sup_perc", GetType(Decimal))
        End If

        Dim piva = EsercizioCDC.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
        Dim sa_cod = EsercizioCDC.impiantoPK.appezzamentoPK.centroAziendalePK.codice
        Dim appezza = EsercizioCDC.impiantoPK.appezzamentoPK.codice
        Dim id_Reg = EsercizioCDC.impiantoPK.codice

        Dim SupPerc As Decimal = 0
        SupPerc = CDec(sup_trattata * 100 / sup_originale)

        Dim dr As DataRow = DtNuoviImpianti.NewRow
        dr.Item("piva") = piva
        dr.Item("sa_cod") = sa_cod
        dr.Item("appezza_old") = appezza_old
        dr.Item("id_reg_old") = id_reg_old
        dr.Item("appezza_new") = appezza
        dr.Item("id_reg_new") = id_Reg
        dr.Item("sup_new") = sup_trattata
        dr.Item("sup_perc") = SupPerc

        DtNuoviImpianti.Rows.Add(dr)

    End Sub

    Private Shared Function ricavaSupTrattata_x_MatCod(parametriAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita),
                                                       sup_imp As Decimal,
                                                       ByRef sup_TerrenoNudo As Decimal
                                                       ) As List(Of SupTrattata_x_DettaglioSemina)
        Dim sup_totaleTrattata As Decimal

        'Mi ricavo la lista di tutti i prodotti usati x la loro superficie trattata
        Dim paramAggiuntiviList As List(Of Parametri_Aggiuntivi_Attivita) =
                parametriAggiuntiviList.FindAll(Function(c) c.key = Key_Parametri_Aggiuntivi_Attivita.lista_SupTrattata_x_DettaglioSemina)

        Dim dictSupTrattata_x_MatCod As New Dictionary(Of Integer, Decimal)
        Dim SupTrattata_x_MatCod_List As New List(Of SupTrattata_x_DettaglioSemina)

        If paramAggiuntiviList.Count = 1 Then
            'Raggruppo i prodotti per mat_cod e sommo le loro superfici trattate
            Dim dummy As List(Of SupTrattata_x_DettaglioSemina) = JsonConvert.DeserializeObject(Of List(Of SupTrattata_x_DettaglioSemina))(paramAggiuntiviList(0).value)
            For Each Mat_Cod In dummy
                If Not dictSupTrattata_x_MatCod.ContainsKey((Mat_Cod.Mat_Cod)) Then
                    dictSupTrattata_x_MatCod.Add(Mat_Cod.Mat_Cod, Mat_Cod.Sup_Trattata)
                Else
                    dictSupTrattata_x_MatCod(Mat_Cod.Mat_Cod) += Mat_Cod.Sup_Trattata
                End If
            Next

            'Inserisco i prodotti raggruppati per Mat_Cod nella lista da ritornare al chiamante
            For Each prodotto In dictSupTrattata_x_MatCod
                Dim SupTrattata_x_DettaglioSemina = New SupTrattata_x_DettaglioSemina
                SupTrattata_x_DettaglioSemina.Mat_Cod = prodotto.Key
                SupTrattata_x_DettaglioSemina.Sup_Trattata = prodotto.Value
                SupTrattata_x_MatCod_List.Add(SupTrattata_x_DettaglioSemina)
            Next

            'Sommo le superifci trattate con i prodotti e ricavo la superficie di terreno nudo effettiva
            '(potrei avere seminato su meno superficie rispetto al campo Sup. [ha] Trattata)
            For Each sup In SupTrattata_x_MatCod_List
                sup_totaleTrattata += sup.Sup_Trattata
            Next
            sup_TerrenoNudo = sup_imp - sup_totaleTrattata

        Else
            'NON DOVREBBE MAI ENTRARE QUI
            Throw New GiasException(Gias.ListaProdottiUsatiNonTrovata)
        End If

        Return SupTrattata_x_MatCod_List

    End Function

    Private Function ciSonoTrattamentiDiserbi(PIVA As String,
                                              SA_COD As Integer,
                                              APPEZZA As Integer,
                                              ID_REG As Integer,
                                              objParametri_Server As AgronicaCoreParametri
                                              ) As Boolean

        Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R()
        Dim strFiltroLavCod As String = "Lav_Cod IN (" & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_DISSECCAMENTO & "," & LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," & LAVCOD_DISERBO & "," & LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE & "," & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA & ")"

        Dim dt As DataTable = objMovDest.Leggi(PIVA, SA_COD, 0, 0, 0, APPEZZA, ID_REG, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, strFiltroLavCod, "", objParametri_Server)

        If dt.Rows.Count > 0 Then
            Return True
        End If

        Return False
    End Function

#End Region

End Class
