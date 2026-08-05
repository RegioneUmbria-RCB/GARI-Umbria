Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreWebService
Imports AgronicaCoreUtility
Imports System.Text
Imports AgronicaCoreVarieDAL

Public Class Rintraccio

    Private SuperUserUsername As String
    Private SuperUserPassword As String
    Private SuperUserPiva As String
    Private Codice_Chiave_Cliente As Integer
    Private Piva_Padre As String
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private Parametri_Extra As String

    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra
        'Configurazione_Servizio = _Configurazione_Servizio leggo solo i parametri che mi servono 
        ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti
    End Sub

    Public Function ImportaRicevimenti_e_RintracciaColli(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim MsgImportRitiri As String = ""
        Dim MsgImportLavorazione As String = ""
        Dim MsgImportProdottoFinito As String = ""


        Dim resRitiri As Boolean = False
        resRitiri = Opta_Ws.Importa_Ultimi_Carichi(MsgImportRitiri, ObjParametri_Server)

        Dim resLavorazione As Boolean = False
        resLavorazione = Opta_Ws.Importa_Ultimi_Lavorazione(MsgImportLavorazione, ObjParametri_Server)

        Dim resProdottoFinito As Boolean = False
        resProdottoFinito = Opta_Ws.Importa_Ultimi_ProdottoFinito(MsgImportProdottoFinito, ObjParametri_Server)

        Dim MsgRintracciabilita As String = ""
        Dim resRintracciabilita As Boolean = False
        resRintracciabilita = RintracciaTuttiLotti_GeneraTabellaRitiroXImpiantiRaccolti(True, 0, 0, "", "", "", MsgRintracciabilita, ObjParametri_Server)


        Dim MsgRintracciabilitaProdFinito As String = ""
        Dim resRintracciabilitaProdFinito As Boolean = False
        resRintracciabilita = Rintraccia_AssociaImmesso_ProdFinito(True, MsgRintracciabilitaProdFinito, ObjParametri_Server)

        Messaggio_di_Ritorno_Opzionale = " { Importazione ritiri: " & MsgImportRitiri & " ; Importazione lavorazione: " & MsgImportLavorazione & " ; Importazione prodotto finito: " & MsgImportProdottoFinito & " ; Rintraccio Colli:" & MsgRintracciabilita & " } "

        Return resRitiri And resLavorazione And resProdottoFinito And resRintracciabilita

    End Function

    Public Shared Function Esegui_Ricerca_Movimenti(ByVal Mat_Cod As Integer, ByVal Cal_Cod As Integer,
                                                    ByVal Cod_Progetto As Integer, ByVal Fase_Cod As Integer,
                                                    ByVal Udm_Cod As Integer, ByVal Elem_Cod As Integer,
                                                    ByVal Pro_Cod As Integer, ByVal Lotto As String,
                                                    ByVal piva As String, ByVal sa_cod As Integer,
                                                    ByVal Fabbricato_Cod As Integer,
                                                    ByRef ListaImpRaccolti As List(Of Impianto),
                                                    ByRef Mat_Cod_Raccolto As Integer,
                                                    ByRef Mat_Des_Raccolto As String,
                                                    ByVal SalvaInDtMovimenti As Boolean,
                                                    ByRef DtMovimenti As DataTable,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef forno_piva As String,
                                                    ByRef forno_sa_cod As Integer,
                                                    ByRef forno_fabbricato_cod As Integer,
                                                    ByRef data_inizio_cura As DateTime) As Boolean

        ListaImpRaccolti = New List(Of Impianto)

        If Lotto.Trim <> "" And Lotto.ToLower.Trim <> "indefinito" Then
        Else
            Lotto = CStr(CInt(LOTTO_NONDEFINITO))
        End If
        'Dim Sa_Cod As Integer = 0
        Dim Id_Agenda As Integer = 0
        Dim Id_Mov As Integer = 0
        Dim id_Mov_Det As Integer = 0
        Dim IterCount As Integer = 0

        Dim ListaMovDetProcessati As New List(Of Integer)
        Dim Lavorazione As Integer = 0 'LAVCOD_CARICO la lascio libera, potrebbe essere un carico o un carico post cura
        'leggo l'ultimo movimento di carico del prodotto, il piu recente

        Try

            'ricorsiva
            Return Gestisci_Movimento(CAU_CARICO, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Elem_Cod, Pro_Cod,
                               Lotto, piva, sa_cod, Id_Agenda, Id_Mov, id_Mov_Det, ListaMovDetProcessati,
                               Lavorazione, SalvaInDtMovimenti, DtMovimenti, ListaImpRaccolti, Mat_Cod_Raccolto, Mat_Des_Raccolto, IterCount, objParametri_Server,
                               forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)


        Catch ex As Exception

            Return False

        End Try



    End Function


    Private Shared Function Gestisci_Movimento(ByVal Cau_Mov As String,
                                          ByVal Mat_Cod As Integer,
                                          ByVal Cal_Cod As Integer,
                                          ByVal Cod_Progetto As Integer,
                               ByVal Fase_Cod As Integer,
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Lotto As String,
                               ByVal piva As String, ByVal sa_cod As Integer,
                               ByVal Id_Agenda As Integer, ByVal Id_Mov As Integer,
                               ByVal id_Mov_Det As Integer,
                               ByVal ListaMovDetProcessati As List(Of Integer),
                               ByVal Lavorazione As Integer,
                               ByVal SalvaInDtMovimenti As Boolean,
                               ByRef DtMovimenti As DataTable,
                               ByRef ListaImpRaccolti As List(Of Impianto),
                               ByRef Mat_Cod_Raccolto As Integer,
                               ByRef Mat_Des_Raccolto As String,
                               ByRef IterCount As Integer,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef forno_piva As String,
                               ByRef forno_sa_cod As Integer,
                               ByRef forno_fabbricato_cod As Integer,
                               ByRef data_inizio_cura As DateTime) As Boolean




        Dim ObjRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim Descrizione As String = ""
        Dim ListaMovDetProcessatiStr As String = "-1"
        For i = 1 To ListaMovDetProcessati.Count - 1
            ListaMovDetProcessatiStr &= "," & ListaMovDetProcessati(i)
        Next

        IterCount += 1

        If IterCount > 20 Then
            Throw New Exception("Attenzione, si è superato il limite massimo nella ricerca iterativa: lotto=" & Lotto & "; ListaMovDetProcessati=" & ListaMovDetProcessatiStr)
        End If

        Dim filtro As String = "agenda.lav_cod in (" & Lavorazione & ") and movimenti_dettagli not in (" & ListaMovDetProcessatiStr & ") "
        Dim ordine As String = " Movimenti.Data_Movimento desc , Movimenti.Ora  desc "
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DataInizio As Date
        DataInizio = AGRODATAINIZIO
        Dim DataFine As Date
        DataFine = AGRODATAFINE
        Dim DtMovimentiSingolo As DataTable = objMovimenti.LeggiCaricoScarico_New(
                                                   piva,
                                                   sa_cod, Id_Agenda, Id_Mov, id_Mov_Det,
                                                   Elem_Cod,
                                                   Pro_Cod,
                                                   Mat_Cod,
                                                   0,
                                                   0, 0, 0, Cau_Mov, Cod_Progetto, Fase_Cod, 0, 0, Lotto, Cal_Cod,
                                                   "",
                                                   "",
                                                   ordine,
                                                   objParametri_Server)


        If DtMovimentiSingolo.Rows.Count = 0 Then
            'Attenzione, il sa cod deve essere o se provengo dallo scarico raccolta o se ho il magazzino in un centro differente non trova il movimento di carico 
            'e di conseguenza non  rintraccia gli impianti (vedi sotto LAVCOD_SCARICO -> LAVCOD_RACCOLTA)
            Return False
        End If

        Dim dr As DataRow
        For i = 0 To 0
            ListaMovDetProcessati.Add(DtMovimentiSingolo.Rows(i).Item("Id_Mov_Det"))

            If SalvaInDtMovimenti Then

                If IsNothing(DtMovimenti) Then
                    DtMovimenti = DtMovimentiSingolo.Copy
                    DtMovimenti.Rows.Clear()
                End If

                DtMovimenti.ImportRow(DtMovimentiSingolo.Rows(i))
                If Not DtMovimenti.Columns.Contains("DescrizioneMovimento") Then
                    DtMovimenti.Columns.Add("DescrizioneMovimento")
                End If

            End If

            Dim Lav_Cod As Integer = DtMovimentiSingolo.Rows(i).Item("Lav_Cod")

            'Parto dal movimento più recente,
            'nel caso della rintracciata tabacco al momento è il carico nel magazzino dell'azienda del collo curato

            'scorriamo tutti i carichi e gli scarichi

            Select Case Lav_Cod

                Case LAVCOD_RACCOLTA
                    '1      - se è un lav cod cura allora sono nel carico da raccolta
                    Select Case Cau_Mov
                        Case CAU_CARICO

                            'Attenzione, il sa cod deve essere o se provengo dallo scarico raccolta o se ho il magazzino in un centro differente non trova il movimento di carico 
                            'e di conseguenza non  rintraccia gli impianti (vedi sotto LAVCOD_SCARICO -> LAVCOD_RACCOLTA)

                            If SalvaInDtMovimenti Then
                                Descrizione = "Carico del prodotto raccolto in azienda"
                                DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                            End If


                            'devo recuperare gli impianti raccolti
                            ListaImpRaccolti = getImpiantiRaccoltiFromIdAgenda(DtMovimentiSingolo.Rows(i).Item("Id_Agenda"), objParametri_Server)
                            'recupero la materia prima raccolta
                            Mat_Cod_Raccolto = Mat_Cod
                            Mat_Des_Raccolto = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", Elem_Cod, Mat_Cod, "", "", "", objParametri_Server)

                    End Select

                Case LAVCOD_SCARICO
                    'Verifico se c'è cura o raccolta collegata e rintraccio lo scarico
                    Dim dtrifCarico As DataTable = ObjRif.Leggi_Join(
                                                 DtMovimentiSingolo.Rows(i).Item("Piva"),
                                                 DtMovimentiSingolo.Rows(i).Item("Sa_Cod"),
                                                 DtMovimentiSingolo.Rows(i).Item("Id_Agenda"),
                                                 -1, -1,
                                                 Lav_Cod, "",
                                                 "",
                                                 0,
                                                 0,
                                                 -1, -1, 0, "",
                                                 "", "", objParametri_Server)
                    If dtrifCarico.Rows.Count = 1 Then
                        Select Case dtrifCarico.Rows(0).Item("Lav_Cod_Rif")
                            Case LAVCOD_RACCOLTA
                                '2         - Scarico post raccolta azienda, verifico raccolta
                                '            deve selezionare un solo dettaglio del movimento di carico raccolta con quel lotto

                                If SalvaInDtMovimenti Then
                                    Descrizione = "Scarico del prodotto raccolto per invio ad UDS"
                                    DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                End If

                                'Attenzione, il sa cod in dtrifCarico è quello del movimento di raccolta 
                                'e quindi dell'impianto raccolto e non del carico,
                                'quindi se ho il magazzino in un centro differente non trova il movimento di carico 
                                'e di conseguenza non  rintraccia gli impianti,
                                'per questo gli passo sa_cod 0 e non  dtrifCarico.Rows(0).Item("Sa_Cod_Rif")

                                Dim sacodrifzero As Integer = 0

                                Gestisci_Movimento(CAU_CARICO,
                                                    Mat_Cod,
                                                    Cal_Cod,
                                                    Cod_Progetto,
                                                    Fase_Cod,
                                                    Elem_Cod,
                                                    Pro_Cod,
                                                    Lotto,
                                                        dtrifCarico.Rows(0).Item("Piva_Rif"),
                                                        sacodrifzero,
                                                        dtrifCarico.Rows(0).Item("Id_Agenda_Rif"),
                                                        0,
                                                        0,
                                                        ListaMovDetProcessati,
                                                        dtrifCarico.Rows(0).Item("Lav_Cod_Rif"),
                                                        SalvaInDtMovimenti, DtMovimenti,
                                                        ListaImpRaccolti,
                                                         Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                        IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

                            Case LAVCOD_CURA
                                '5         - Scarico uds post cura, verifico il carico della cura   
                                '            deve selezionare un solo dettaglio del movimento di carico sfornatura cura con quel lotto
                                If SalvaInDtMovimenti Then
                                    Descrizione = "Scarico del prodotto curato da uds"
                                    DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                End If

                                Gestisci_Movimento(CAU_CARICO,
                                                    Mat_Cod,
                                                    Cal_Cod,
                                                    Cod_Progetto,
                                                    Fase_Cod,
                                                    Elem_Cod,
                                                    Pro_Cod,
                                                    Lotto,
                                                        dtrifCarico.Rows(0).Item("Piva_Rif"),
                                                        dtrifCarico.Rows(0).Item("Sa_Cod_Rif"),
                                                        dtrifCarico.Rows(0).Item("Id_Agenda_Rif"),
                                                        0,
                                                        0,
                                                        ListaMovDetProcessati,
                                                        dtrifCarico.Rows(0).Item("Lav_Cod_Rif"),
                                                        SalvaInDtMovimenti, DtMovimenti,
                                                        ListaImpRaccolti,
                                                        Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                        IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

                            Case Else
                                Throw New Exception("Non gestito")
                        End Select
                    Else
                        '            deve selezionare un solo dettaglio del movimento di carico sfornatura cura con quel lotto
                        If SalvaInDtMovimenti Then
                            Descrizione = "Carico Senza provenienza specificata"
                            DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                        End If

                    End If

                Case LAVCOD_CARICO
                    'Verifico se c'è cura o raccolta collegata e rintraccio lo scarico
                    Dim dtrifCarico As DataTable = ObjRif.Leggi_Join(
                                                 DtMovimentiSingolo.Rows(i).Item("Piva"),
                                                 DtMovimentiSingolo.Rows(i).Item("Sa_Cod"),
                                                 DtMovimentiSingolo.Rows(i).Item("Id_Agenda"),
                                                 -1,
                                                 -1,
                                                 Lav_Cod, "",
                                                 "",
                                                 0,
                                                 0,
                                                 -1, -1, 0, "",
                                                 "", "", objParametri_Server)
                    If dtrifCarico.Rows.Count = 1 Then
                        Select Case dtrifCarico.Rows(0).Item("Lav_Cod_Rif")
                            Case LAVCOD_RACCOLTA
                                '3         - Carico UDS raccolto, verifico lo scarico da azienda origine
                                '            deve selezionare un solo dettaglio del movimento di scarico azienda origine con quel lotto
                                If SalvaInDtMovimenti Then
                                    Descrizione = "Carico del prodotto raccolto presso UDS per cura"
                                    DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                End If

                                Gestisci_Movimento(CAU_SCARICO,
                                                   Mat_Cod,
                                                   Cal_Cod,
                                                   Cod_Progetto,
                                                   Fase_Cod,
                                                   Elem_Cod,
                                                   Pro_Cod,
                                                   Lotto,
                                                      dtrifCarico.Rows(0).Item("Piva_2"),
                                                      dtrifCarico.Rows(0).Item("Sa_Cod_2"),
                                                      dtrifCarico.Rows(0).Item("Id_Agenda_2"),
                                                      0,
                                                      0,
                                                      ListaMovDetProcessati,
                                                      dtrifCarico.Rows(0).Item("Lav_Cod_2"),
                                                      SalvaInDtMovimenti, DtMovimenti,
                                                       ListaImpRaccolti,
                                                       Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                        IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)
                            Case LAVCOD_CURA
                                '6         - Carico azienda origine post cura, verifico scarico da uds
                                '            deve selezionare un colo dettaglio del movimento di scarico azienda origine con quel lotto
                                If SalvaInDtMovimenti Then
                                    Descrizione = "Carico del prodotto curato presso l'azienda di origine"
                                    DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                End If

                                Gestisci_Movimento(CAU_SCARICO,
                                                   Mat_Cod,
                                                   Cal_Cod,
                                                   Cod_Progetto,
                                                   Fase_Cod,
                                                   Elem_Cod,
                                                   Pro_Cod,
                                                   Lotto,
                                                      dtrifCarico.Rows(0).Item("Piva_2"),
                                                      dtrifCarico.Rows(0).Item("Sa_Cod_2"),
                                                      dtrifCarico.Rows(0).Item("Id_Agenda_2"),
                                                      0,
                                                      0,
                                                      ListaMovDetProcessati,
                                                      dtrifCarico.Rows(0).Item("Lav_Cod_2"),
                                                      SalvaInDtMovimenti, DtMovimenti,
                                                       ListaImpRaccolti,
                                                       Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                       IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)
                            Case Else
                                Throw New Exception("Non gestito")
                        End Select
                    Else
                        If SalvaInDtMovimenti Then
                            Descrizione = "Carico Senza provenienza specificata"
                            DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                        End If

                    End If

                Case LAVCOD_CURA
                    '4         - Cura, verifico carico in uds prodotto da curare
                    '           ho 4 movimenti legati a due a due:
                    '           A scarico magazzino e B carico in essiccatoio
                    '           C scarico essiccatoio e D carico in magazzino
                    '           Nei casi D->C e B->A tramite l'estraint recupero  il movimento collegato che 
                    '                è una trasfornazione quindi ha un altro prodotto!!
                    '                Ma andando a ritroso devo per forza beccare un movimento e un dettaglio collegato in base all'extraint
                    '           Nei casi C->B e A->5 invece il prodotto non cambia quindi uso il lotto
                    Select Case Cau_Mov
                        Case CAU_SCARICO

                            Select Case DtMovimentiSingolo.Rows(i).Item("Tipo_Destinazione")
                                Case MAGAZZINO
                                    'A  -scarico magazzino per infornatura, cerco il carico di magazzino secco o bolla etc
                                    If SalvaInDtMovimenti Then
                                        Descrizione = "Scarico magazzino per infornatura"
                                        DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                    End If

                                    Dim lvcod As Integer = LAVCOD_CARICO
                                    Gestisci_Movimento(CAU_CARICO,
                                                        Mat_Cod,
                                                        Cal_Cod,
                                                        Cod_Progetto,
                                                        Fase_Cod,
                                                        Elem_Cod,
                                                        Pro_Cod,
                                                        Lotto,
                                                        piva,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        ListaMovDetProcessati,
                                                        lvcod,
                                                        SalvaInDtMovimenti, DtMovimenti,
                                                       ListaImpRaccolti,
                                                       Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                       IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)
                                Case ESSICCATOIO
                                    'C  -scarico essiccatoio sfornatura, cerco il carico essiccatoio per infornatura
                                    If SalvaInDtMovimenti Then
                                        Descrizione = "Scarico essiccatoio da sfornatura"
                                        DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                    End If

                                    'GRILLI: salvo i dati del forno
                                    forno_piva = DtMovimentiSingolo.Rows(i).Item("piva2")
                                    forno_sa_cod = DtMovimentiSingolo.Rows(i).Item("sa_cod2")
                                    forno_fabbricato_cod = DtMovimentiSingolo.Rows(i).Item("Id_Destinazione")

                                    Gestisci_Movimento(CAU_CARICO,
                                                        Mat_Cod,
                                                        Cal_Cod,
                                                        Cod_Progetto,
                                                        Fase_Cod,
                                                        Elem_Cod,
                                                        Pro_Cod,
                                                        Lotto,
                                                        piva,
                                                        sa_cod,
                                                        Id_Agenda,
                                                        0,
                                                        0,
                                                        ListaMovDetProcessati,
                                                        Lavorazione,
                                                        SalvaInDtMovimenti, DtMovimenti,
                                                        ListaImpRaccolti,
                                                        Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                       IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)
                            End Select
                        Case CAU_CARICO
                            '           D->C e B->A carichi, cambia il prodotto
                            Dim strFiltro_Dettaglio As String = " Movimenti_dettagli.ID_Mov_Det <> " & DtMovimentiSingolo.Rows(i).Item("ID_Mov_Det") & " "
                            strFiltro_Dettaglio &= " AND Movimenti.Extra_Int = " & DtMovimentiSingolo.Rows(i).Item("Extra_Int") & "  "
                            'prodotto trasformato
                            'Attenzione, le due chiavi per accoppiare i movimenti possono essere uguali perche sono due generate 
                            'generaidoperazionedicuramovinfornatura e generaidoperazionedicuramovsfornatura
                            'infatti in produzione avendo sempre per ora un movimento di inf e uno di sforn viaggiano sugli stessi valori
                            'quindi devo cercare in base al tipo di fabbricaro, se ho un magazzino cerco un essiccatorio e viceversa
                            'per sicurezza meglio cambiare intervallo dei valori
                            Dim TipoDestOrigine As Integer = DtMovimentiSingolo.Rows(i).Item("Tipo_Destinazione")
                            Dim TipoDestinazione As Integer = 0
                            Select Case TipoDestOrigine
                                Case enum_FabbricatiTipi.MagazzinoAziendale
                                    TipoDestinazione = enum_FabbricatiTipi.essiccatoio
                                Case enum_FabbricatiTipi.essiccatoio
                                    TipoDestinazione = enum_FabbricatiTipi.MagazzinoAziendale

                                    'GRILLI: salvo i dati del forno
                                    forno_piva = DtMovimentiSingolo.Rows(i).Item("piva2")
                                    forno_sa_cod = DtMovimentiSingolo.Rows(i).Item("sa_cod2")
                                    forno_fabbricato_cod = DtMovimentiSingolo.Rows(i).Item("Id_Destinazione")
                                    data_inizio_cura = CDate(DtMovimentiSingolo.Rows(i).Item("data_movimento"))

                                Case Else
                                    Throw New Exception("tipo destinazione non presente")
                            End Select

                            'ATTENZIONE, la leggiscariconew fa il left join quindi mi tira su anche la riga dell'altro movimento pur mettendo il 
                            'filtro sul tipo destinazione, quindi devo anche mettere 
                            'clausola not null sul tipo
                            strFiltro_Dettaglio &= " AND tipo_destinazione is not null "

                            Dim ProdTrasf As DataTable = objMovimenti.LeggiCaricoScarico_New(
                                                           piva,
                                                           sa_cod, Id_Agenda,
                                                           0, 0,
                                                           0,
                                                           0,
                                                           0,
                                                           0,
                                                           0, 0, TipoDestinazione, CAU_SCARICO, 0, 0, 0, 0, "", 0,
                                                           strFiltro_Dettaglio,
                                                           "",
                                                           ordine,
                                                           objParametri_Server)
                            If ProdTrasf.Rows.Count <> 1 Then
                                Throw New Exception("deve selezionare una sola riga")
                            End If
                            Select Case DtMovimentiSingolo.Rows(i).Item("Tipo_Destinazione")
                                Case MAGAZZINO
                                    'D  -carico magazzino sfornatura, cerco lo scarico da essiccatoio da sfornatura
                                    '            deve selezionare un solo dettaglio del movimento di scarico azienda origine con quel lotto
                                    If SalvaInDtMovimenti Then
                                        Descrizione = "Carico magazzino da sfornatura"
                                        DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                    End If

                                    Gestisci_Movimento(CAU_SCARICO,
                                                       ProdTrasf.Rows(0).Item("Mat_Cod"),
                                                       ProdTrasf.Rows(0).Item("Cal_Cod"),
                                                       ProdTrasf.Rows(0).Item("Cod_Progetto"),
                                                       ProdTrasf.Rows(0).Item("Fase_Cod"),
                                                       ProdTrasf.Rows(0).Item("Elem_Cod"),
                                                       ProdTrasf.Rows(0).Item("Pro_Cod"),
                                                       ProdTrasf.Rows(0).Item("Lotto"),
                                                       piva,
                                                       sa_cod,
                                                       Id_Agenda,
                                                       ProdTrasf.Rows(0).Item("Id_Mov"),
                                                       ProdTrasf.Rows(0).Item("id_Mov_Det"),
                                                       ListaMovDetProcessati,
                                                       ProdTrasf.Rows(0).Item("Lav_Cod"),
                                                       SalvaInDtMovimenti, DtMovimenti,
                                                        ListaImpRaccolti,
                                                        Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                        IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)
                                Case ESSICCATOIO
                                    'B  -carico essiccatoio per infornatura, cerco lo scarico da magazzino per infornatura
                                    If SalvaInDtMovimenti Then
                                        Descrizione = "Carico essiccatoio da infornatura"
                                        DtMovimenti.Rows(DtMovimenti.Rows.Count - 1).Item("DescrizioneMovimento") = Descrizione
                                    End If

                                    'GRILLI: salvo i dati del forno
                                    forno_piva = DtMovimentiSingolo.Rows(i).Item("piva2")
                                    forno_sa_cod = DtMovimentiSingolo.Rows(i).Item("sa_cod2")
                                    forno_fabbricato_cod = DtMovimentiSingolo.Rows(i).Item("Id_Destinazione")

                                    Gestisci_Movimento(CAU_SCARICO,
                                                       ProdTrasf.Rows(0).Item("Mat_Cod"),
                                                       ProdTrasf.Rows(0).Item("Cal_Cod"),
                                                       ProdTrasf.Rows(0).Item("Cod_Progetto"),
                                                       ProdTrasf.Rows(0).Item("Fase_Cod"),
                                                       ProdTrasf.Rows(0).Item("Elem_Cod"),
                                                       ProdTrasf.Rows(0).Item("Pro_Cod"),
                                                       ProdTrasf.Rows(0).Item("Lotto"),
                                                       piva,
                                                       sa_cod,
                                                       Id_Agenda,
                                                       ProdTrasf.Rows(0).Item("Id_Mov"),
                                                       ProdTrasf.Rows(0).Item("id_Mov_Det"),
                                                       ListaMovDetProcessati,
                                                       ProdTrasf.Rows(0).Item("Lav_Cod"),
                                                       SalvaInDtMovimenti, DtMovimenti,
                                                        ListaImpRaccolti,
                                                       Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                        IterCount, objParametri_Server,
                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

                            End Select

                    End Select


            End Select


        Next

        Return True

    End Function



    Private Shared Function getImpiantiRaccoltiFromIdAgenda(ByVal Id_Agenda As Integer,
                                                            ByRef objParametri_Server As AgronicaCoreParametri) As List(Of Impianto)

        Dim ListaImpRaccolti As New List(Of Impianto)

        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Agenda = objAgenda.Leggi("",
                                     0,
                                     Id_Agenda,
                                     0,
                                     objParametri_Server)

        If Agenda.Lav_Cod <> LAVCOD_RACCOLTA Then
            Throw New NotImplementedException
        End If

        For i = 0 To Agenda.Movimenti.Count - 1

            Select Case Agenda.Movimenti(i).Cau_Mov

                Case enum_Agenda_Causali.RILIEVO_RACCOLTA
                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then
                        If Agenda.Movimenti(i).Movimenti_Dettagli.Count <> 1 Then
                            Throw New Exception("Ci deve essere un solo dettaglio per un solo prodotto al momento")
                        End If
                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                            If Agenda.Piva <> Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                                Throw New ApplicationException
                            End If

                            'non ci deve essere nessun dettaglio tecnico
                            If Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 0 Then
                                Throw New ApplicationException
                            End If

                            'ci deve essere almeno un movimento destinazione, uno per ciascun Appezzamento 
                            If IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                Throw New ApplicationException
                            End If

                            'Elem_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                            'Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                            'Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                            'Qta = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta
                            ''Cod_Progetto  = agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto
                            'Cal_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cal_Cod
                            'Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto

                            'una destinazione per ciascun impianto
                            For r = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1



                                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                objAppezzamento.Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                                objAppezzamento.Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                                objAppezzamento.Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                                objAppezzamento.ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione
                                objAppezzamento.Qta = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta

                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                Dim Dt_Imp As New DataTable
                                Dt_Imp = objImp.Leggi(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                             "", "", objParametri_Server)

                                objAppezzamento.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("Veg_Cod"))
                                objAppezzamento.Cul_Cod = CInt(Dt_Imp.Rows(0).Item("Cul_Cod"))
                                objAppezzamento.Cul_Des = (New AgronicaCoreMetaSchemaDAL.Cultivar_R()).CulDes_from_CulCod(objAppezzamento.Cul_Cod, objParametri_Server)
                                objAppezzamento.Sup_Imp = CDbl(Dt_Imp.Rows(0).Item("Sup_Imp"))

                                'aggiungo impioanto a parameteri agenda, cosi pagina master pouò ricrreare i check,
                                'ma devo inserirlo sono se non è gia presente altrimenti mi sdoppia le colonne
                                Dim presente As Boolean = False
                                For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpRaccolti
                                    If ap.Piva = objAppezzamento.Piva AndAlso
                                        ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso
                                            ap.Appezza = objAppezzamento.Appezza AndAlso
                                            ap.ID_Reg = objAppezzamento.ID_Reg Then

                                        presente = True
                                    End If
                                Next
                                If Not presente Then
                                    ListaImpRaccolti.Add(objAppezzamento)
                                End If
                            Next
                        Next
                    End If

            End Select

        Next

        Return ListaImpRaccolti

    End Function

    'rintraccia impianti raccolti e info correlate
    'di tutti i lotti ritirati nella tabella __Rintraccio_Ritiro
    'e li riporta in __Rintraccio_RitiroXImpiantiRaccolti
    'che indica oltre alle chiavi di rintraccio e impianti anche i dati dell'impianto in modo da non fare altre letture
    Public Shared Function RintracciaTuttiLotti_GeneraTabellaRitiroXImpiantiRaccolti(
                                                                    ByVal Solonuovi As Boolean,
                                                                     ByVal Anno As Integer,
                                                                     ByVal Collo As Integer,
                                                                     ByVal Barcode As String,
                                                                     ByVal CUAA As String,
                                                                     ByVal PIVA As String,
                                                                     ByRef msg As String,
                                                                     ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim elimina As Boolean = False

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim strsql As String = " Select * from [__Rintraccio_Ritiro] where 1=1  "

        If Solonuovi Then
            strsql &= " AND ID not in (SELECT ID  FROM [dbo].[__Rintraccio_RitiroXImpiantiRaccolti] ) "
        End If

        If Anno <> 0 Then
            strsql &= " and anno =" & Anno & " "
        End If
        If Collo <> 0 Then
            strsql &= " and Collo =" & Collo & " "
        End If
        If Barcode <> "" Then
            strsql &= " and Barcode ='" & Agro_SQL_SaveText(Barcode) & "' "
        End If
        If CUAA <> "" Then
            strsql &= " and CODICE_CUAA ='" & Agro_SQL_SaveText(CUAA) & "' "
        End If
        If PIVA <> "" Then
            CUAA = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read().Leggi_CUAA(PIVA, objParametri_Server)
            strsql &= " and CODICE_CUAA ='" & Agro_SQL_SaveText(CUAA) & "' "
        End If

        strsql &= " order by id "

        Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")

        Dim totres As Boolean = True

        Dim NumRintr As Integer = 0
        Dim NumScrit As Integer = 0

        Dim nIterazioni As Integer = 0

        For Each dr As DataRow In dt.Rows

            nIterazioni += 1

            'dati del ritiro
            Anno = dr.Item("ANNO")
            Dim Id As Integer = dr.Item("ID")
            ' Dim Grado As String = dr.Item("Grado")
            Collo = dr.Item("Collo")
            Barcode = dr.Item("Barcode")
            CUAA = dr.Item("CODICE_CUAA")
            PIVA = New AgronicaCoreAnagrafeDAL.Imprese_Read().Piva_From_CUAA(CUAA, objParametri_Server)



            If Collo < 0 Then
                Continue For
            End If


            'rintraccio
            Dim ListaImpRaccolti As List(Of Impianto)
            Dim Mat_Cod_Raccolto As Integer
            Dim Mat_Des_Raccolto As String


            Dim Mat_Cod As Integer = 0
            Dim Cal_Cod As Integer = 0
            Dim Cod_Progetto As Integer = 0
            Dim Fase_Cod As Integer = 0
            Dim Udm_Cod As Integer = 0
            Dim Elem_Cod As Integer = 0
            Dim Pro_Cod As Integer = 0
            'Dim Lotto As String = ""
            Dim sa_cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0

            Elem_Cod = TRASFORMATI_VEGETALI
            'LR/0000000131/01554970515
            'LU/14/01/01554970515/00554
            'Nascosto per selvi: LN/progressivounicodi10cifre
            'nascosto per rampi: LN/02+14+cosocio4cifre+codicecollo6cifre
            Dim fatagricoop As String = "01"
            Dim LottoSelvi As String = "LU" & "/" & CStr(Anno - 2000) & "/" & fatagricoop & "/" & PIVA & "/" & CStr(Collo).PadLeft(5, "0") & ""
            fatagricoop = "02"
            Dim LottoRampi As String = "LU" & "/" & CStr(Anno - 2000) & "/" & fatagricoop & "/" & PIVA & "/" & CStr(Collo).PadLeft(5, "0") & ""

            Dim DtMovimenti As DataTable

            'per velocizzare non creo dt_movimenti

            'la piva ci vuole per velocizzare, è per ora la piva dell'ultimo movimento 
            'quindi dell'azienda di origine del tabacco, dove viene caricato il collo dopo la cura.

            Dim forno_piva As String = ""
            Dim forno_sa_cod As Integer = 0
            Dim forno_fabbricato_cod As Integer = 0
            Dim data_inizio_cura As DateTime = AGRODATAINIZIO

            Dim res As Boolean = False

            res = Rintraccio.Esegui_Ricerca_Movimenti(Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Elem_Cod,
                                                                        Pro_Cod, LottoSelvi, PIVA, sa_cod, Fabbricato_Cod,
                                                                        ListaImpRaccolti, Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                                        False, DtMovimenti, objParametri_Server,
                                                                        forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

            If Not res Then

                res = Rintraccio.Esegui_Ricerca_Movimenti(Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Elem_Cod,
                                                                            Pro_Cod, LottoRampi, PIVA, sa_cod, Fabbricato_Cod,
                                                                            ListaImpRaccolti, Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                                            False, DtMovimenti, objParametri_Server,
                                                                            forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

            End If


            If res Then
                NumRintr += 1

                'elimino in __Rintraccio_RitiroXImpiantiRaccolti i record con quell'id
                strsql = " delete from __Rintraccio_RitiroXImpiantiRaccolti where ID =  " & Id
                Dim res2 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")

                'inserisco i record in __Rintraccio_RitiroXImpiantiRaccolti
                For Each Impianto As Impianto In ListaImpRaccolti
                    strsql = " insert into __Rintraccio_RitiroXImpiantiRaccolti ([ID],[PIVA] ,[SA_COD],[APPEZZA],[ID_REG], [Qta] ,[Veg_Cod],[CUL_COD],[Cul_Des],[Sup_Imp], [Mat_Cod], [Mat_Des], [forno_piva], [forno_sa_cod], [forno_fabbricato_cod], [data_inizio_cura] ) VALUES ( "
                    strsql &= "                                                " & Id & ", '" & Impianto.Piva & "', " & Impianto.Sa_Cod & ", " & Impianto.Appezza & ", " & Impianto.ID_Reg & ", " & Agro_SQL_SaveNum(Impianto.Qta) & ", " & Impianto.Veg_Cod & ", " & Impianto.Cul_Cod & ", '" & Impianto.Cul_Des & "', " & Agro_SQL_SaveNum(Impianto.Sup_Imp) & ", " & Mat_Cod_Raccolto & ", '" & Agro_SQL_SaveText(Mat_Des_Raccolto) & "', '" & Agro_SQL_SaveText(forno_piva) & "', " & Agro_SQL_SaveNum(forno_sa_cod) & ", " & Agro_SQL_SaveNum(forno_fabbricato_cod) & ", " & Agro_SQL_SaveDate(data_inizio_cura) & "        )  "
                    Dim res3 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")

                    If res3 = False Then
                        Throw New Exception("errore scrittura record in __Rintraccio_RitiroXImpiantiRaccolti ")
                    End If
                Next


                If elimina Then
                    'elimino 
                    strsql = " delete from __Rintraccio_Ritiro where ID =  " & Id
                    Dim res4 As Boolean = dp.EseguiQuery_Scrittura(objParametri_Server, strsql, "OptaImport")
                End If


            End If

        Next

        If NumRintr <> dt.Rows.Count Then
            msg = " Rintracciati " & NumRintr & " su " & dt.Rows.Count & " colli ricercati"
            Return True 'metto true momentaneamente perchè tanto qualcuno mancherà sempre pinche non vengon caricati tutti
        End If

        msg = " Rintracciati Tutti i " & NumRintr & " colli "
        Return True

    End Function

    Public Shared Function EsportaRintraccioOpta(ByVal tipo As TipoTabelleRintraccioOpta, ByRef dt As DataTable, ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        dt = New DataTable
        Dim strsql As String = ""
        Select Case tipo
            Case TipoTabelleRintraccioOpta.Rintraccio_X_Collo_OPTA
                strsql = " Select * from [__Rintraccio_X_Collo_OPTA] where 1=1  "
                strsql &= " order by CODICE_CUAA, [Collo]  "

            Case TipoTabelleRintraccioOpta.Rintraccio_X_Appezzamento
                strsql = " Select * from [__Rintraccio_X_Appezzamento] where 1=1  "
                strsql &= " order by CODICE_CUAA,[sa_nome],[NumeroAppezzamento]    "

            Case TipoTabelleRintraccioOpta.Rintraccio_X_CoronaAppezzamento_OPTA
                strsql = " Select * from [__Rintraccio_X_CoronaAppezzamento_OPTA] where 1=1  "
                strsql &= " order by CODICE_CUAA,[sa_nome],[NumeroAppezzamento]  "

            Case TipoTabelleRintraccioOpta.Rintraccio_X_Grado_Opta
                strsql = " Select * from [__Rintraccio_X_Grado_Opta] where 1=1  "
                strsql &= " order by [rag_soc],[sa_nome],[NumeroAppezzamento],[Colore]   "

            Case 0
                'Vista con i buchi sia da una parte che dall'altra
                strsql = queryMappingGiaMagazzino()

            Case -1
                'Vista con per Selvi con solo Azienda, Collo, nome Appezzamento, Varietà e n° Corona
                strsql = "select i.rag_soc as 'Ragione Sociale', rr.Collo, a.app_nome as 'Nome Appezzamento', rrir.Mat_Des as 'Numero Corona', rrir.Cul_Des as 'Varietà', rrir.Data_Inizio_Cura as [Data Infornatura], F.Fabbricato_Des as [Numero Forno] "
                strsql &= "from __Rintraccio_RitiroXImpiantiRaccolti rrir "
                strsql &= "inner join __Rintraccio_Ritiro rr "
                strsql &= "	on rr.ID=rrir.id "
                strsql &= "inner join Fabbricati F "
                strsql &= "	on F.PIVA = Forno_Piva "
                strsql &= "	and F.SA_COD = Forno_Sa_Cod "
                strsql &= "	and F.Fabbricato_Cod = Forno_Fabbricato_Cod "
                strsql &= "inner join Imprese i "
                strsql &= "	on i.PIVA=rrir.PIVA "
                strsql &= "inner join appezzamento a "
                strsql &= "	on a.PIVA=rrir.PIVA and a.SA_COD=rrir.SA_COD and a.appezza=rrir.appezza "
                strsql &= "order by rag_soc, collo"

            Case Else
                Return False
        End Select

        dt = dp.EseguiQuery_Lettura(objParametri_Server, strsql, "OptaImport")
        Return True

    End Function


    Public Shared Function ImportaRicevimenti_e_RintracciaColli(ByRef Messaggio_di_Ritorno_Opzionale As String, configurazioneServizio As Configurazione_Servizio, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim MsgImportRitiri As String = ""
        Dim MsgImportLavorazione As String = ""
        Dim MsgImportProdottoFinito As String = ""
        Dim resRitiri As Boolean = False
        Dim resLavorazione As Boolean = False
        Dim resProdottoFinito As Boolean = False


        Dim cfgScaricoOpta As ConfigurazioneImportazioneOPTA
        If configurazioneServizio.Parametri_Extra.Contains("{") Then
            cfgScaricoOpta = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ConfigurazioneImportazioneOPTA)(configurazioneServizio.Parametri_Extra)
        Else
            cfgScaricoOpta = New ConfigurazioneImportazioneOPTA With {.RintracciaDati = True, .ScaricaDati = True}
        End If

        If cfgScaricoOpta.ScaricaDati Then

            resRitiri = Opta_Ws.Importa_Ultimi_Carichi(MsgImportRitiri, objParametri_Server)
            resLavorazione = Opta_Ws.Importa_Ultimi_Lavorazione(MsgImportLavorazione, objParametri_Server)
            resProdottoFinito = Opta_Ws.Importa_Ultimi_ProdottoFinito(MsgImportProdottoFinito, objParametri_Server)
        Else
            MsgImportRitiri = "Flag di scarico non impostato, verificare le impostazioni del GSB"
            MsgImportLavorazione = "Flag di scarico non impostato, verificare le impostazioni del GSB"
            MsgImportProdottoFinito = "Flag di scarico non impostato, verificare le impostazioni del GSB"
            resRitiri = True
            resLavorazione = True
            resProdottoFinito = True
        End If
        'scarica dati

        Dim MsgRintracciabilita As String = ""
        Dim resRintracciabilita As Boolean = False
        Dim MsgRintracciabilitaProdFinito As String = ""
        Dim resRintracciabilitaProdFinito As Boolean = False

        If cfgScaricoOpta.RintracciaDati Then
            resRintracciabilita = RintracciaTuttiLotti_GeneraTabellaRitiroXImpiantiRaccolti(True, 0, 0, "", "", "", MsgRintracciabilita, objParametri_Server)
            resRintracciabilitaProdFinito = Rintraccia_AssociaImmesso_ProdFinito(True, MsgRintracciabilitaProdFinito, objParametri_Server)
        Else
            MsgRintracciabilita = "Rintracciabilità non richiesata, verificare le impostazioni del GSB"
            MsgRintracciabilitaProdFinito = "Rintracciabilità non richiesata, verificare le impostazioni del GSB"
            resRintracciabilita = True
            resRintracciabilitaProdFinito = True
        End If


        Messaggio_di_Ritorno_Opzionale = " { Importazione ritiri: " & MsgImportRitiri & " ; Importazione lavorazione: " & MsgImportLavorazione & " ; Importazione prodotto finito: " & MsgImportProdottoFinito & " ; Rintraccio Colli:" & MsgRintracciabilita & "  Rintraccio ProdottoFinito:" & MsgRintracciabilitaProdFinito & " } "

        Return resRitiri And resLavorazione And resProdottoFinito And resRintracciabilita And resRintracciabilitaProdFinito

    End Function


    Public Shared Function Rintraccia_AssociaImmesso_ProdFinito(ByVal soloNuovi As Boolean, ByRef Messaggio_di_Ritorno_Opzionale As String, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim stb As New StringBuilder
        Dim dp As New AgronicaCoreDataProvider.DataProvider

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        'TODO: Gestire la tranzazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)


            stb.Length = 0
            stb.Append(" truncate table OPTA_Tmp_Immesso1 " & vbCrLf)
            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "Rintraccia_AssociaImmesso_ProdFinito -- delete")

            stb.Length = 0
            stb.Append(" truncate table OPTA_Tmp_Immesso " & vbCrLf)
            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "Rintraccia_AssociaImmesso_ProdFinito -- delete")

            stb.Length = 0
            stb.Append(" truncate table OPTA_Tmp_Prod " & vbCrLf)
            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "Rintraccia_AssociaImmesso_ProdFinito -- delete")

            stb.Length = 0
            stb.Append(" truncate table OPTA_Tmp_ImmessoXProdotto" & vbCrLf)
            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "Rintraccia_AssociaImmesso_ProdFinito -- delete")


            Dim whClause As String = " where "
            If soloNuovi Then
                whClause = " AND "
            End If

            '--immesso per lavorazione 
            stb.Length = 0
            stb.Append(" insert OPTA_Tmp_Immesso1  " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      cast(data as date) as data " & vbCrLf)
            stb.Append("    , cast(ID_Gruppo as int) as miscela " & vbCrLf)
            stb.Append("    , count(*) as conteggioCartoni   " & vbCrLf)
            stb.Append(" from __Rintraccio_Lavorazione e " & vbCrLf)

            stb.Append(" where Data < " & Agro_SQL_SaveDate(DateAdd(DateInterval.Day, -1, Now.Date)) & " " & vbCrLf)
            stb.Append(" and id_gruppo is not null " & vbCrLf)
            stb.Append(" and not exists ( " & vbCrLf)
            stb.Append("     Select 1 " & vbCrLf)
            stb.Append("     from [dbo].[__Rintraccio_LavorazioneXProdottFinito] lXP " & vbCrLf)
            stb.Append("     where lXP.Lavorazione_ID = e.ID " & vbCrLf)
            stb.Append(" ) " & vbCrLf)

            stb.Append(" group by cast(data as date), ID_Gruppo  " & vbCrLf)
            stb.Append(" order by cast(data as date), ID_Gruppo " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "Rintraccia_AssociaImmesso_ProdFinito -- immesso per lavorazione")

            stb.Length = 0
            stb.Append(" update t " & vbCrLf)
            stb.Append(" set Data = t1.data " & vbCrLf)
            stb.Append(" from OPTA_Tmp_Immesso1 t inner join  " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append("    select miscela, min(data) as data " & vbCrLf)
            stb.Append("    from OPTA_Tmp_Immesso1 t " & vbCrLf)
            stb.Append("    group by miscela  " & vbCrLf)
            stb.Append("    having count(*) >1 " & vbCrLf)
            stb.Append(" ) t1  " & vbCrLf)
            stb.Append(" on t.miscela = t1.miscela  " & vbCrLf)
            stb.Append(" ")

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport -- Update data su immesso1")

            stb.Length = 0
            stb.Append(" insert OPTA_Tmp_Immesso(Data, miscela, conteggioCartoni, conteggioGruppoLavorazione, NumeroGruppoImmesso)  " & vbCrLf)
            stb.Append(" select data, miscela, sum(conteggioCartoni) as conteggioCartoni, null as conteggioGruppoLavorazione, DENSE_RANK() over (partition by data  order by miscela) " & vbCrLf)
            stb.Append(" from OPTA_Tmp_Immesso1 " & vbCrLf)
            stb.Append(" group by data, miscela  " & vbCrLf)
            stb.Append(" order by data, miscela  " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport -- insert immesso")

            stb.Length = 0
            stb.Append(" update t  " & vbCrLf)
            stb.Append(" set  " & vbCrLf)
            stb.Append("             conteggioGruppoLavorazione = xd.conteggioGruppoLavorazione " & vbCrLf)
            stb.Append(" from OPTA_Tmp_Immesso t inner join  " & vbCrLf)
            stb.Append(" ( " & vbCrLf)
            stb.Append("    select  " & vbCrLf)
            stb.Append("          data " & vbCrLf)
            stb.Append("        , count(*) as conteggioGruppoLavorazione             " & vbCrLf)
            stb.Append("    from OPTA_Tmp_Immesso t " & vbCrLf)
            stb.Append("    group by data " & vbCrLf)
            stb.Append(" ) xd on xd.data = t.data  " & vbCrLf)
            stb.Append("  " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport -- insert immesso")

            stb.Length = 0
            stb.Append(" update i  " & vbCrLf)
            stb.Append(" set datariferimento = t.data " & vbCrLf)
            stb.Append(" from __Rintraccio_Lavorazione i " & vbCrLf)
            stb.Append("    inner join OPTA_Tmp_Immesso t " & vbCrLf)
            stb.Append("        on i.id_gruppo = t.miscela  " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport -- insert immesso")

            stb.Length = 0
            stb.Append(" insert OPTA_Tmp_Prod  " & vbCrLf)
            stb.Append(" select  " & vbCrLf)
            stb.Append("      cast(p.data as date) as data " & vbCrLf)
            stb.Append("    , tipo " & vbCrLf)
            stb.Append("    , count(*) as conteggio " & vbCrLf)
            stb.Append("    --, DENSE_RANK() over ( partition by cast(data as date)  order by cast(data as date), tipo ) as NumeroGruppoLavorazione " & vbCrLf)
            stb.Append("    , null as RapportoProdottiImmessi " & vbCrLf)
            stb.Append(" from __Rintraccio_ProdottoFinito p " & vbCrLf)
            stb.Append(" where not exists ( " & vbCrLf)
            stb.Append("     Select 1 " & vbCrLf)
            stb.Append("     from [dbo].[__Rintraccio_LavorazioneXProdottFinito] lXP " & vbCrLf)
            stb.Append("     where lXP.ProdottoFinito_ID = p.ID " & vbCrLf)
            stb.Append(" ) " & vbCrLf)
            stb.Append(" group by cast(p.data as date), tipo " & vbCrLf)
            stb.Append(" order by cast(p.data as date), tipo " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport --prodotto finito per lavorazione")

            stb.Length = 0
            stb.Append(" update p  " & vbCrLf)
            stb.Append(" set RapportoProdottiImmessi = CAST( p.conteggioGruppoProduzione as float ) / cast( i.conteggioGruppoLavorazione as float) " & vbCrLf)
            stb.Append(" from OPTA_Tmp_Prod p " & vbCrLf)
            stb.Append("    inner join OPTA_Tmp_Immesso i    " & vbCrLf)
            stb.Append("        on p.data = i.data  " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport --prodotto finito per lavorazione")

            '--ciclo per produzione dato finale ..
            Dim prodotto_cod As Integer
            Dim RapportoProdottiImmessiCorrente As Integer
            Dim NumeroGruppoImmessoCorrente As Integer
            Dim NumeroProdottiEtichettati As Integer
            Dim DataCorrente As Date
            Dim TipoCorrente As String
            Dim TotaleConteggioGruppiImmessiPerGiornata As Integer

            Dim cScorriTipi As DataTable
            Dim cScorriProdotti As DataTable
            Dim cTotaleConteggioGruppiImmessiPerGiornata As DataTable

            stb.Length = 0
            stb.Append("select t.data, t.tipo, cast( t.RapportoProdottiImmessi  as int) as RapportoProdottiImmessi " & vbCrLf)
            stb.Append(" from OPTA_Tmp_Prod t " & vbCrLf)
            stb.Append(" where RapportoProdottiImmessi is not null " & vbCrLf)
            stb.Append(" order by data " & vbCrLf)


            cScorriTipi = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "Leggi cScorriTipi")

            For Each iScorriTipi In cScorriTipi.Rows

                DataCorrente = iScorriTipi("Data")
                TipoCorrente = iScorriTipi("Tipo")
                RapportoProdottiImmessiCorrente = iScorriTipi("RapportoProdottiImmessi")

                NumeroGruppoImmessoCorrente = 1
                NumeroProdottiEtichettati = 0

                stb.Length = 0
                stb.Append("    Select ID " & vbCrLf)
                stb.Append("    from __Rintraccio_ProdottoFinito  " & vbCrLf)
                stb.Append("    where TIPO = '" & Agro_SQL_SaveText(TipoCorrente) & "'" & vbCrLf)
                stb.Append("    and cast(data as date ) = " & Agro_SQL_SaveDate(DataCorrente) & "  " & vbCrLf)
                stb.Append("    order by data " & vbCrLf)

                cScorriProdotti = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "Leggi cScorriTipi")

                For Each iScorriProdotti In cScorriProdotti.Rows

                    prodotto_cod = iScorriProdotti("ID")

                    stb.Length = 0
                    stb.Append("select top 1 conteggioGruppoLavorazione  " & vbCrLf)
                    stb.Append("        from OPTA_Tmp_Immesso  " & vbCrLf)
                    stb.Append("        where data = " & Agro_SQL_SaveDate(DataCorrente) & " ")

                    cTotaleConteggioGruppiImmessiPerGiornata = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString, "Leggi cScorriTipi")

                    'conteggi
                    If cTotaleConteggioGruppiImmessiPerGiornata.Rows.Count > 0 Then
                        TotaleConteggioGruppiImmessiPerGiornata = cTotaleConteggioGruppiImmessiPerGiornata.Rows(0)("ConteggioGruppoLavorazione")
                    End If

                    If NumeroProdottiEtichettati = RapportoProdottiImmessiCorrente Then
                        NumeroProdottiEtichettati = 0

                        'solo se sono a posto...
                        If NumeroGruppoImmessoCorrente < TotaleConteggioGruppiImmessiPerGiornata Then
                            NumeroGruppoImmessoCorrente += 1
                        End If
                    End If

                    NumeroProdottiEtichettati += 1
                    'fine conteggi


                    stb.Length = 0
                    stb.Append("insert OPTA_Tmp_ImmessoXProdotto  " & vbCrLf)
                    stb.Append("        select i.id,  " & prodotto_cod & vbCrLf)
                    stb.Append("        from __Rintraccio_Lavorazione i " & vbCrLf)
                    stb.Append("            inner join OPTA_Tmp_Immesso t1 " & vbCrLf)
                    stb.Append("                on i.DataRiferimento = t1.data " & vbCrLf)
                    stb.Append("                and t1.miscela = i.id_gruppo  " & vbCrLf)
                    stb.Append("                and t1.NumeroGruppoImmesso =  " & NumeroGruppoImmessoCorrente & vbCrLf)
                    stb.Append("                and t1.data = " & Agro_SQL_SaveDate(DataCorrente) & vbCrLf)

                    stb.Append(" and not exists ( " & vbCrLf)
                    stb.Append("  Select 1 " & vbCrLf)
                    stb.Append("    from OPTA_Tmp_ImmessoXProdotto a1 " & vbCrLf)
                    stb.Append("    where a1.cod_immesso = i.id " & vbCrLf)
                    stb.Append("    and a1.cod_prodotto = " & prodotto_cod & vbCrLf)
                    stb.Append(" ) " & vbCrLf)


                    dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport --prodotto finito per lavorazione")

                Next

            Next

            stb.Length = 0
            stb.Append(" insert __Rintraccio_LavorazioneXProdottFinito " & vbCrLf)
            stb.Append(" select cod_immesso, cod_prodotto, null " & vbCrLf)
            stb.Append(" from [dbo].[OPTA_Tmp_ImmessoXProdotto] o1 " & vbCrLf)
            stb.Append(" where not exists ( " & vbCrLf)
            stb.Append("    select 1  " & vbCrLf)
            stb.Append("    from [dbo].[__Rintraccio_LavorazioneXProdottFinito] a1 " & vbCrLf)
            stb.Append("    where a1.[Lavorazione_ID] = o1.[cod_immesso] " & vbCrLf)
            stb.Append("    and a1.[ProdottoFinito_ID] = o1.[cod_prodotto] " & vbCrLf)
            stb.Append(" ) " & vbCrLf)

            dp.EseguiQuery_Scrittura(objParametri_Server, stb.ToString, "OptaImport --prodotto finito per lavorazione")



            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)
            Dim lEccezione As String
            lEccezione = "errore in fase di assegnazione immesso / prodotto finito: " & messaggioErrore
            Messaggio_di_Ritorno_Opzionale &= lEccezione



        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try



        Return True



    End Function

    Private Shared Function queryMappingGiaMagazzino() As String
        Dim Stb As New StringBuilder

        Stb.Append("SELECT  " & vbCrLf)
        Stb.Append("      DatiCura.id_agenda " & vbCrLf)
        Stb.Append("    , DatiCura.LottoLU " & vbCrLf)
        Stb.Append("    , DatiCura.LottoLR " & vbCrLf)
        Stb.Append("    , destLR.piva " & vbCrLf)
        Stb.Append("    , destLR.sa_cod " & vbCrLf)
        Stb.Append("    , destLR.Appezza " & vbCrLf)
        Stb.Append("    , destLR.id_destinazione " & vbCrLf)
        Stb.Append("    , DatiCura.Collo " & vbCrLf)
        Stb.Append("    , i.rag_soc AS 'Azienda' " & vbCrLf)
        Stb.Append("    , ca.sa_nome AS 'CentroAziendale' " & vbCrLf)
        Stb.Append("    , app.app_nome AS 'NomeAppezzamento' " & vbCrLf)
        Stb.Append("    , c.cul_cod " & vbCrLf)
        Stb.Append("    , c.cul_des AS 'Coltura' " & vbCrLf)
        Stb.Append("    , mp.Mat_Cod " & vbCrLf)
        Stb.Append("    , mp.Mat_Des AS 'Corona' " & vbCrLf)
        Stb.Append("    , DatiSimone1.Mag_Barcode AS 'Mag_Barcode' " & vbCrLf)
        Stb.Append("    , DatiSimone1.Mag_Anno AS 'Mag_Anno' " & vbCrLf)
        Stb.Append("    , DatiSimone1.Mag_Collo AS 'Mag_Collo' " & vbCrLf)
        Stb.Append("    , DatiSimone1.Mag_Piva AS 'Mag_Piva' " & vbCrLf)
        Stb.Append("    --, Mag_Azienda " & vbCrLf)
        Stb.Append("    --, Mag_Sa_Cod " & vbCrLf)
        Stb.Append("    --, Mag_CentroAziendale " & vbCrLf)
        Stb.Append("    --, Mag_Appezza " & vbCrLf)
        Stb.Append("    --, Mag_NomeAppezzamento " & vbCrLf)
        Stb.Append("    --, Mag_ID_Reg " & vbCrLf)
        Stb.Append("    --, Mag_Cul_Cod " & vbCrLf)
        Stb.Append("    --, Mag_Coltura " & vbCrLf)
        Stb.Append("    --, Mag_Mat_Cod " & vbCrLf)
        Stb.Append("    --, Mag_Corona " & vbCrLf)
        Stb.Append(" FROM  " & vbCrLf)

        Stb.Append(" ( " & vbCrLf)
        Stb.Append(" Select distinct a.id_agenda " & vbCrLf)
        Stb.Append("    , detLC.lotto AS 'LottoLU' " & vbCrLf)
        Stb.Append("    , detLC_R.lotto AS 'LottoLR'  " & vbCrLf)
        Stb.Append("    , CAST(SUBSTRING(detLC.lotto,22,5) AS INT) AS 'Collo' " & vbCrLf)
        Stb.Append("    , detLC_R.Mat_Cod " & vbCrLf)
        Stb.Append(" from agenda a  " & vbCrLf)

        ' --Lotti LC delle oprazioni di cura
        Stb.Append(" INNER JOIN movimenti movLC  " & vbCrLf)
        Stb.Append("    ON movLC.id_agenda=a.Id_Agenda " & vbCrLf)
        Stb.Append("    AND a.lav_cod=5004   " & vbCrLf)
        Stb.Append("    AND movLC.cau_mov='7300'  " & vbCrLf)

        Stb.Append(" INNER JOIN movimenti_dettagli detLC  " & vbCrLf)
        Stb.Append("    ON movLC.id_agenda=detLC.Id_Agenda  " & vbCrLf)
        Stb.Append("    and movLC.id_mov=detLC.Id_mov " & vbCrLf)

        Stb.Append(" INNER JOIN mov_destinazioni destLC  " & vbCrLf)
        Stb.Append("    ON detLC.id_agenda=destLC.id_agenda  " & vbCrLf)
        Stb.Append("    AND detLC.id_mov=destLC.id_mov  " & vbCrLf)
        Stb.Append("    AND detLC.id_mov_det=destLC.id_mov_det " & vbCrLf)
        Stb.Append("    AND destLC.tipo_destinazione=20 " & vbCrLf)

        ' --Lotti LR delle operazioni d'agenda con lotto LC (operazioni di cura)
        Stb.Append(" INNER JOIN movimenti movLC_R  " & vbCrLf)
        Stb.Append("    ON movLC_R.id_agenda=a.Id_Agenda " & vbCrLf)
        Stb.Append("    AND movLC_R.cau_mov='7350'  " & vbCrLf)

        Stb.Append(" INNER JOIN movimenti_dettagli detLC_R  " & vbCrLf)
        Stb.Append("    ON MovLC_R.id_agenda=detLC_R.Id_Agenda  " & vbCrLf)
        Stb.Append("    AND movLC_R.id_mov=detLC_R.Id_mov " & vbCrLf)

        Stb.Append(" INNER JOIN mov_destinazioni destLC_R  " & vbCrLf)
        Stb.Append("    ON detLC_R.id_agenda=destLC_R.id_agenda  " & vbCrLf)
        Stb.Append("    AND detLC_R.id_mov=destLC_R.id_mov  " & vbCrLf)
        Stb.Append("    AND detLC_R.id_mov_det=destLC_R.id_mov_det " & vbCrLf)
        Stb.Append("    AND destLC_R.tipo_destinazione=20 " & vbCrLf)

        Stb.Append(" ) DatiCura " & vbCrLf)

        ' ----Lotti LR della Raccolta
        Stb.Append(" LEFT JOIN movimenti_dettagli detLR  " & vbCrLf)
        Stb.Append("    ON DatiCura.lottoLR=detLR.lotto  " & vbCrLf)

        Stb.Append(" INNER JOIN movimenti mLR  " & vbCrLf)
        Stb.Append("    ON mLR.id_agenda=detLR.Id_Agenda  " & vbCrLf)
        Stb.Append("    AND mLR.id_mov=detLR.Id_mov " & vbCrLf)
        Stb.Append("    AND mLR.cau_mov='2200' " & vbCrLf)

        Stb.Append(" INNER JOIN agenda a2  " & vbCrLf)
        Stb.Append("    ON a2.id_agenda=mLR.Id_Agenda  " & vbCrLf)

        Stb.Append(" INNER JOIN Mov_Destinazioni destLR  " & vbCrLf)
        Stb.Append("    ON detLR.id_agenda=destLR.Id_Agenda  " & vbCrLf)
        Stb.Append("        AND detLR.id_mov=destLR.Id_mov  " & vbCrLf)
        Stb.Append("        AND detLR.id_mov_det=destLR.Id_mov_det   " & vbCrLf)

        '--Dati anagrafici degli impianti dei Lotti LR della Raccolta
        Stb.Append(" INNER JOIN imprese i  " & vbCrLf)
        Stb.Append("    ON destLR.piva=i.piva  " & vbCrLf)

        Stb.Append(" INNER JOIN Centri_Aziendali ca  " & vbCrLf)
        Stb.Append("    ON destLR.piva=ca.piva  " & vbCrLf)
        Stb.Append("    AND ca.sa_cod=destLR.sa_cod  " & vbCrLf)

        Stb.Append(" INNER JOIN appezzamento app  " & vbCrLf)
        Stb.Append("    ON destLR.piva=app.piva  " & vbCrLf)
        Stb.Append("    AND app.sa_cod=destLR.sa_cod  " & vbCrLf)
        Stb.Append("    AND app.appezza=destLR.appezza  " & vbCrLf)

        Stb.Append(" INNER JOIN reg_impianti ri  " & vbCrLf)
        Stb.Append("    ON destLR.piva=ri.piva  " & vbCrLf)
        Stb.Append("    AND ri.sa_cod=destLR.sa_cod  " & vbCrLf)
        Stb.Append("    AND ri.appezza=destLR.appezza  " & vbCrLf)
        Stb.Append("    AND ri.id_reg=destLR.Id_Destinazione  " & vbCrLf)

        '----Varietà di tabacco e numero corona
        Stb.Append(" INNER JOIN cultivar c  " & vbCrLf)
        Stb.Append("    ON c.cul_cod= ri.cul_cod  " & vbCrLf)
        Stb.Append("  " & vbCrLf)
        Stb.Append(" INNER JOIN Materie_Prime mp  " & vbCrLf)
        Stb.Append("    ON DatiCura.Mat_Cod=mp.mat_cod  " & vbCrLf)

        ' ------Verifico se sono già stati importati in magazzino da Simone
        Stb.Append(" LEFT JOIN  " & vbCrLf)
        Stb.Append(" ( " & vbCrLf)
        Stb.Append("   SELECT rr.anno AS 'Mag_ANNO',rr.collo AS 'Mag_COLLO',ic.piva AS 'Mag_PIVA',rr.barcode AS 'Mag_BARCODE', " & vbCrLf)
        Stb.Append("                     'LU/' + cast (rr.anno - 2000 AS varchar (4)) + '/01/' + ic.piva + '/' + REPLACE(STR(rr.collo, 5), SPACE(1), '0') as 'Lotto1' " & vbCrLf)
        Stb.Append("   FROM __Rintraccio_Ritiro rr " & vbCrLf)
        Stb.Append("   INNER JOIN imprese_codici ic ON rr.codice_cuaa=ic.val_cod " & vbCrLf)
        Stb.Append("   WHERE id_cod=1010 " & vbCrLf)
        Stb.Append(" ) DatiSimone1 ON DatiCura.lottoLU=DatiSimone1.lotto1  " & vbCrLf)




        Return Stb.ToString()
    End Function


End Class