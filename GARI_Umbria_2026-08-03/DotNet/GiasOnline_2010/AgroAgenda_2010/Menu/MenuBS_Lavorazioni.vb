
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreMetaSchemaDAL
Imports System.Threading.Tasks

Public Class MenuBS_Lavorazioni


    ''' <summary>
    ''' Crea e Carica il GridView 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Data_Selezionata"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="Lav_Cod"></param>
    ''' <param name="Flag_TerrenoNudo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <remarks></remarks>
    Public Shared Function Carica_Lavorazioni(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Veg_Cod As Integer,
            ByVal Cul_Cod As Integer,
            ByVal Tipo As String,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal Flag_TerrenoNudo As Boolean,
            ByVal xFiltroAggiuntivo_colturali As String,
            ByVal xFiltroAggiuntivo_postRaccolta As String,
            ByVal xFiltroAggiuntivo_contabili As String,
            ByVal xFiltroAggiuntivo_contabili_Macchine As String,
            ByVal xFiltroAggiuntivo_contabili_Audit As String,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri,
            ByVal FF_TrackedData_Cod As Integer,
            Optional ByVal FromOutToIn As Boolean = True,
            Optional ByVal cCertificazione As Integer = True,
            Optional ByVal righeAggiunte As String = "",
            Optional ByVal Visualizza_Codici_AppezzaImpianti As Boolean = False,
            Optional ByVal Visualizza_KPIN_BlockName As Boolean = False
        ) As DataTable


        Dim Dt As New DataTable
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim i As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strCodici_Appezzamenti As String
        Dim strCodici_Impianto As String
        Dim strKPIN As String
        Dim strBlockName As String

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim ht_Permessi As New Hashtable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim objEti As AgronicaCoreStampeDAL.FF_Etichette_R

        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objparametri_Server)

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        Dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))

        Dt.Columns.Add(New DataColumn("Centro_Aziendale", GetType(String)))
        Dt.Columns.Add(New DataColumn("Specie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Appezzamenti_Coinvolti", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotti_Utilizzati", GetType(String)))
        Dt.Columns.Add(New DataColumn("Avversita", GetType(String)))
        Dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))

        If Visualizza_Codici_AppezzaImpianti Then
            Dt.Columns.Add(New DataColumn("Codici_Appezzamenti", GetType(String)))
            Dt.Columns.Add(New DataColumn("Codici_Impianto", GetType(String)))
        End If

        If Visualizza_KPIN_BlockName Then
            Dt.Columns.Add(New DataColumn("KPIN", GetType(String)))
            Dt.Columns.Add(New DataColumn("BlockName", GetType(String)))
        End If

        If FF_TrackedData_Cod > 0 Then
            Dt.Columns.Add(New DataColumn("FF_Track_Lotto_Padre", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Lotto", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod_Padre", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Extra_Totale", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Contenitori", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Imballi", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Mat_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Referenza", GetType(String)))
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    Dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice", GetType(String)))
                    Dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key"), GetType(String)))
                End If
            Next
            Dt.Columns.Add(New DataColumn("FF_Righe_Aggiunte", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_codice_generazione", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("FF_Linea_Cod", GetType(Integer)))
            'Lavezzo - 29/03/2021 - test 
            Dt.Columns.Add(New DataColumn("FF_Operazioni_Campagna", GetType(String)))
        End If


        Dim filtro As String = "|"
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
                'No un filtro
                filtro = HttpContext.Current.Session("Filtro")
            End If
        End If

        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
                Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
            End If
        End If


        Dim Filtro_Utente_Lavorazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
                Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
            End If
        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If

        'Identifico se è abilitata l'operazione di cura
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO, objparametri_Utenti)
        Dim bool_isCuraEnabled As Boolean = (IsNumeric(Tipo_Raccolta_Val) AndAlso Tipo_Raccolta_Val = enum_RACCOLTA_TIPO.Raccolta_e_Cura)


        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Veg_Cod,
                Cul_Cod,
                Tipo,
                Gru_Cod,
                Lav_Cod,
                Flag_TerrenoNudo,
                True,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xFiltroAggiuntivo_colturali,
                xFiltroAggiuntivo_postRaccolta,
                xFiltroAggiuntivo_contabili,
                xFiltroAggiuntivo_contabili_Macchine,
                xFiltroAggiuntivo_contabili_Audit,
                xOrderBy,
                objparametri_Utenti,
                objparametri_Server,
                FF_TrackedData_Cod, FromOutToIn,
                Visualizza_Codici_AppezzaImpianti:=Visualizza_Codici_AppezzaImpianti,
                Visualizza_KPIN_BlockName:=Visualizza_KPIN_BlockName
                )


        Catch ex As Exception

            Return Nothing

        End Try

        If DtAgenda.Rows.Count > 0 Then

            '----------------------------------
            'Leggo tutti i principi attivi
            Dim HtProdPA As New Hashtable()
            Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

            Dim DtCosti As New DataTable

            If FF_TrackedData_Cod > 0 Then
                strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "Id_Mov_Det")
            Else
                strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

                'Leggo i costi accessori
                Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
                '16/09/2019: il filtro date lo fa più sotto
                DtCosti = objCostiAccessori.CostiAccessori_from_IdAgenda3(Piva, String.Join(",", strId_Agenda), "", "", objparametri_Server)
            End If

            '03/01/2018 Grilli: Leggo le Avversità fuori dalla megaLettura
            Dim objMovTec As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
            Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_agenda(Piva, "", "", objparametri_Server)

            '(12/11/2018 fede) aggiunta indicazione fase fenologica (splittate le operazioni)
            'leggo le fasi via web service
            Dim dtMovDetTecFasi As DataTable = objMovTec.Leggi_x_agenda_fasifenologiche(Piva, "", "", objparametri_Server)
            Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
            Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

            If Not dtMovDetTecFasi Is Nothing AndAlso dtMovDetTecFasi.Rows.Count > 0 Then

                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

                Dim Filtro_cod_ss As String = ""
                Dim Filtro_ff_cod As String = ""
                Dim Hash_cod_ss As New Hashtable
                Dim Hash_ff_cod As New Hashtable
                Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objparametri_Utenti, 2)
                If imp = "1" Then
                    objParametriIngresso.Personalizzate = True
                End If
                objParametriIngresso.Lingua_Cod = objparametri_Server.Lingua_Cod

                For f = 0 To dtMovDetTecFasi.Rows.Count - 1
                    Select Case dtMovDetTecFasi.Rows(f).Item("ff_classe")
                        Case < 1000
                            If Not Hash_ff_cod.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                                Hash_ff_cod.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                                Filtro_ff_cod &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                            End If
                        Case Else
                            If Not Hash_cod_ss.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                                Hash_cod_ss.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                                Filtro_cod_ss &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                            End If
                    End Select
                Next

                If Filtro_ff_cod <> "" Then
                    objParametriIngresso.strFiltro = " fs.ff_cod in (" & Left(Filtro_ff_cod, Filtro_ff_cod.Length - 1) & ")"
                    objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
                End If
                If Filtro_cod_ss <> "" Then
                    objParametriIngresso.strFiltro = " ss.cod_ss in (" & Left(Filtro_cod_ss, Filtro_cod_ss.Length - 1) & ")"
                    objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
                End If

            End If

            Dim leggiCategMag As New Categorie_Magazzino_R
            Dim DtCategorieMagazzino As DataTable
            If HttpContext.Current.Cache("DtCategorieMagazzino") Is Nothing Then
                DtCategorieMagazzino = leggiCategMag.Leggi(0, "", True, "", "", objparametri_Server)
                HttpContext.Current.Cache("DtCategorieMagazzino") = DtCategorieMagazzino
            Else
                DtCategorieMagazzino = HttpContext.Current.Cache("DtCategorieMagazzino")
            End If


            If Not strId_Agenda Is Nothing Then

                For Each current_Agenda In strId_Agenda

                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strAppezzamenti = ""
                    strCulDes = ""
                    strProdotti = ""
                    strAvversita = ""
                    strNote = ""
                    strCosti_Operatori = ""
                    strCosti_Macchine = ""
                    Sup_TrattataTot = 0

                    strCodici_Appezzamenti = ""
                    strCodici_Impianto = ""
                    strKPIN = ""
                    strBlockName = ""

                    If FF_TrackedData_Cod > 0 Then
                        DrAgenda = DtAgenda.Select("Id_Mov_Det=" & current_Agenda)
                    Else
                        DrAgenda = DtAgenda.Select("id_agenda=" & current_Agenda)
                    End If

                    'DtOperazione = DtAgenda.Clone

                    DtOperazione = DrAgenda.CopyToDataTable

                    'For Each r As DataRow In DrAgenda
                    '    DtOperazione.ImportRow(r)
                    'Next


                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")

                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))

                        '(05/12/2018) per le fasi visualizzo la data del rilievo (validita_inizio nella destinazione)
                        'le fasi nuove creano un id_agenda per centro, fase, data
                        'le vecchie ne avevano 1 per centro con fasi e date diverse assieme
                        '(per queste ultime visualizzo una data in caso ci siano date diverse nello stesso rilievo)
                        Select Case Dr.Item("Lav_Cod")

                            Case LAVCOD_FASI_FENOLOGICHE

                                Dr.Item("Data") = CDate(DrAgenda(0).Item("validita_inizio_destinazione")).ToShortDateString
                                Dr.Item("Data2") = CDate(DrAgenda(0).Item("validita_inizio_destinazione"))

                        End Select

                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Id_Mov") = DrAgenda(0).Item("Id_Mov")
                        Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Info") = ""
                        Dr.Item("Dettagli") = ""
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Ricetta_Cod") = 0
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
                        Dr.Item("tipo") = DrAgenda(0).Item("tipo")

                        'Se è un altre lavorazioni aggiungo il dettaglio
                        If DrAgenda(0).Item("attivitaDesc") <> "" Then
                            'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
                            Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        End If

                        If DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                            Dr.Item("FF_Track_Cal_Cod_Padre") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod_Padre"))
                            Dr.Item("FF_Track_Cal_Cod") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod"))
                            Dr.Item("FF_Track_Lotto_Padre") = DrAgenda(0).Item("FF_Track_Lotto_Padre")
                            Dr.Item("FF_Track_Lotto") = DrAgenda(0).Item("FF_Track_Lotto")
                            Dr.Item("FF_Track_Qta_Extra_Totale") = DrAgenda(0).Item("FF_Track_Qta_Extra_Totale")
                            Dr.Item("FF_Track_Qta_Contenitori") = DrAgenda(0).Item("FF_Track_Qta_Contenitori")
                            Dr.Item("FF_Track_Qta_Imballi") = DrAgenda(0).Item("FF_Track_Qta_Imballi")
                            Dr.Item("FF_codice_generazione") = DrAgenda(0).Item("FF_codice_generazione")
                            Dr.Item("FF_Mat_Cod") = DrAgenda(0).Item("Mat_Cod")
                            Dr.Item("FF_Linea_Cod") = DrAgenda(0).Item("FF_Linea_Cod")

                            'Else
                            '    Dr.Item("FF_Track_Cal_Cod_Padre") = 0
                            '    Dr.Item("FF_Track_Cal_Cod") = 0
                            '    Dr.Item("FF_Track_Lotto_Padre") = ""
                            '    Dr.Item("FF_Track_Lotto") = ""
                            '    Dr.Item("FF_Track_Qta_Extra_Totale") = 0.0
                        End If

                        ' @Paolo
                        ' aggiunta colore per tipologia di lavorazione
                        Select Case DrAgenda(0).Item("tipo")
                            Case "C" ' Colturali
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: green'></i>"
                            Case "E" ' Contabili
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: blue'></i>"
                            Case "V" ' Audit / Monitoraggi
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: orange'></i>"
                            Case "F" ' Macchine
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: red'></i>"
                            Case "Z" ' Zootecniche
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: yellow'></i>"
                        End Select

                        strAppezzamenti = ""
                        strCulDes = ""
                        strProdotti = ""
                        strAvversita = ""
                        strSpecie = ""
                        strCentro = ""
                        Prodotto = ""
                        Ricetta = ""
                        strSpecieVarieta = ""
                        strDettaglioTecnico = ""
                        strCentroCampo = ""
                        strLottiProduzione = ""
                        strLottiImpianto = ""

                        If FF_TrackedData_Cod > 0 Then
                            strDettagli = DrAgenda(0)("Des_Lib") & "<br/>" & DrAgenda(0)("Fabbricato_Des") & "<br/>"
                        Else
                            strDettagli = ""
                        End If

                        If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                            Ricetta = "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                        End If
                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                        Dr.Item("Ricetta_Des") = DrAgenda(0).Item("ricetta_numero")
                        'modifica per contabilità magazzino
                        If operazioneLavCodContabMagazzino(DrAgenda(0).Item("lav_cod")) Then

                            'Exit For

                            'specie
                            DtSpecie = objSqlDis.SelectDistinct("Specie", DtOperazione, "veg_cod", False)
                            'DtSpecie = DtOperazione.DefaultView.ToTable(True, "veg_cod", "veg_des", "DestinazioneTerreniNudi_Des", "appezza")

                            For Each r As DataRow In DtSpecie.Rows
                                Veg_Cod = r.Item("veg_cod")

                                If r.Item("veg_cod") <> 0 Then
                                    'Veg_Cod = DtSpecie.Rows(j).Item("veg_cod")
                                    Dim Veg_Des As String = r.Item("veg_des")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & Veg_Des
                                    Else
                                        strSpecie = Veg_Des
                                    End If

                                ElseIf r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                    'Veg_Cod = r.Item("veg_cod")
                                    Dim DestinazioneTerreniNudi_Des As String = r.Item("DestinazioneTerreniNudi_Des")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & DestinazioneTerreniNudi_Des
                                    Else
                                        strSpecie = DestinazioneTerreniNudi_Des
                                    End If

                                ElseIf r.Item("appezza") <> 0 Then
                                    'Veg_Cod = r.Item("veg_cod")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & "Terreno Nudo"
                                    Else
                                        strSpecie = "Terreno Nudo"
                                    End If
                                End If

                            Next

                            Dr.Item("Veg_Cod") = Veg_Cod

                            '----------------------------------
                            'appezzamenti
                            DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "appezza", False)
                            'DtApp = DtOperazione.DefaultView.ToTable(True, "piva", "sa_cod", "appezza", "Sa_Nome", "App_Nome", "cul_des",
                            '                                         "veg_des", "veg_cod", "DestinazioneTerreniNudi_Des", "LottoImpianto", "ID_Reg")

                            strCentro = If(IsDBNull(DtApp.Rows(0).Item("Sa_Nome")), "", DtApp.Rows(0).Item("Sa_Nome"))

                            For Each r As DataRow In DtApp.Rows
                                If r.Item("App_Nome") <> "" Then
                                    AppezzamentoNome = Replace(r.Item("App_Nome"), "'", "")
                                    strAppezzamenti &= AppezzamentoNome & ", "
                                End If
                                If r.Item("cul_des") <> "" AndAlso InStr(strCulDes, r.Item("cul_des")) = 0 Then
                                    strCulDes &= Replace(r.Item("cul_des"), "'", "") & ", "
                                End If

                                If r.Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, r.Item("cul_des")) = 0 Then
                                    Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                                    Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                                    strSpecieVarieta &= specie & varieta & ", "
                                End If

                                If r.Item("veg_cod") = 0 Then
                                    If r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                        If InStr(strSpecieVarieta, r.Item("DestinazioneTerreniNudi_Des")) = 0 Then
                                            Dim destinazioneTN As String = Replace(r.Item("DestinazioneTerreniNudi_Des"), "'", "")
                                            strSpecieVarieta &= destinazioneTN & ", "
                                        End If
                                    ElseIf r.Item("Appezza") <> 0 Then
                                        strSpecieVarieta &= "Terreno Nudo" & ", "
                                    End If
                                End If

                                Dim objImpianto_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                                If Visualizza_Codici_AppezzaImpianti AndAlso r("appezza") <> 0 Then

                                    'Dim objAppezzamento_codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                                    'Dim appCodiceDT = objAppezzamento_codici.Leggi(r("Piva"),
                                    '                                               r("sa_cod"),
                                    '                                               r("appezza"),
                                    '                                               enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                    '                                               "",
                                    '                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    '                                               "",
                                    '                                               "",
                                    '                                               objparametri_Server)
                                    Dim Riferimento_Alfanumerico_Appezzamento = r("Riferimento_Alfanumerico_Appezzamento")
                                    If Riferimento_Alfanumerico_Appezzamento <> "" Then
                                        strCodici_Appezzamenti &= Riferimento_Alfanumerico_Appezzamento & ", "
                                    End If


                                    'Dim impCodiceDT = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                    '                                                   r("sa_cod"),
                                    '                                                   r("appezza"),
                                    '                                                   r("ID_Reg"),
                                    '                                                   0,
                                    '                                                   CInt(enum_CodiciAnagrafe.Codice_Impianto),
                                    '                                                   True,
                                    '                                                   "",
                                    '                                                   "",
                                    '                                                   objparametri_Server)
                                    Dim Codice_Impianto = r("Codice_Impianto")

                                    If Codice_Impianto <> "" Then
                                        strCodici_Impianto &= Codice_Impianto & ", "
                                    End If

                                End If

                                If Visualizza_KPIN_BlockName AndAlso r("appezza") <> 0 Then

                                    'Dim impKPIN = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                    '                                                   r("sa_cod"),
                                    '                                                   r("appezza"),
                                    '                                                   r("ID_Reg"),
                                    '                                                   -1,
                                    '                                                   CInt(enum_CodiciAnagrafe.Zespri_Codice_kPIN),
                                    '                                                   True,
                                    '                                                   "",
                                    '                                                   "",
                                    '                                                   objparametri_Server)
                                    Dim impKPIN = r("Zespri_Codice_kPIN")

                                    If impKPIN <> "" Then
                                        strKPIN &= impKPIN & ", "
                                    End If

                                    'Dim impBlockName = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                    '                                                   r("sa_cod"),
                                    '                                                   r("appezza"),
                                    '                                                   r("ID_Reg"),
                                    '                                                   -1,
                                    '                                                   CInt(enum_CodiciAnagrafe.Zespri_Block_Name),
                                    '                                                   True,
                                    '                                                   "",
                                    '                                                   "",
                                    '                                                   objparametri_Server)
                                    Dim impBlockName = r("Zespri_Block_Name")

                                    If impBlockName.Trim <> "" Then
                                        strBlockName &= impBlockName.Trim & ", "
                                    End If

                                End If

                            Next

                            If strCodici_Appezzamenti <> "" Then
                                strCodici_Appezzamenti = Left(strCodici_Appezzamenti, strCodici_Appezzamenti.Length - 2)
                            End If

                            If strCodici_Impianto <> "" Then
                                strCodici_Impianto = Left(strCodici_Impianto, strCodici_Impianto.Length - 2)
                            End If

                            If strKPIN <> "" Then
                                strKPIN = Left(strKPIN, strKPIN.Length - 2)
                            End If

                            If strBlockName <> "" Then
                                strBlockName = Left(strBlockName, strBlockName.Length - 2)
                            End If

                            If strAppezzamenti <> "" Then
                                strAppezzamenti = Left(strAppezzamenti, strAppezzamenti.Length - 2)
                            End If
                            If strCulDes <> "" Then
                                strCulDes = Left(strCulDes, strCulDes.Length - 2)
                            End If
                            If strSpecieVarieta <> "" Then
                                strSpecieVarieta = Left(strSpecieVarieta, strSpecieVarieta.Length - 2)
                            End If

                            If Visualizza_Codici_AppezzaImpianti Then
                                Dr.Item("Codici_Appezzamenti") = strCodici_Appezzamenti
                                Dr.Item("Codici_Impianto") = strCodici_Impianto
                            End If

                            If Visualizza_KPIN_BlockName Then
                                Dr.Item("KPIN") = strKPIN
                                Dr.Item("BlockName") = strBlockName
                            End If

                            '----------------------------------
                            'prodotti
                            Dr.Item("Elem_Cod") = DrAgenda(0).Item("Elem_Cod")

                            Dim NomeComune As String = ""
                            Dim drCategoriaMagazzino = DtCategorieMagazzino.Select(" Elem_Cod = " & Dr.Item("Elem_Cod"))
                            If drCategoriaMagazzino.Length > 0 Then
                                NomeComune = drCategoriaMagazzino(0)("NomeComune")
                            End If
                            Dr.Item("NomeComune") = NomeComune

                            Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                                Case FERTILIZZANTI  'FERTILIZZANTI
                                    DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                    DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Pro_Cod") <> 0 Then
                                            Prodotto = r.Item("Fer_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    For Each r As DataRow In DtProdotti1.Rows
                                        If r.Item("Mat_Cod") <> 0 Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case FORMULATI    'FORMULATI
                                    DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Fr_Des") <> "" Then
                                            Prodotto = r.Item("Fr_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case TRAPPOLE
                                    DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Trap_Des") <> "" Then
                                            Prodotto = r.Item("Trap_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case SEMENTI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                    End If

                                Case SEMILAVORATI_VEGETALI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    Select Case Dr.Item("Lav_Cod")
                                        Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                            If strProdotti <> "" Then
                                                strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                                strProdotti = "Semilavorato trattato:" & strProdotti
                                            End If
                                        Case Else
                                            If strProdotti <> "" Then
                                                strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                                strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                            End If
                                    End Select

                                    'vanni, 27/06/2017 gestito per operazioni F&F
                                Case TRASFORMATI_VEGETALI
                                    If FF_TrackedData_Cod > 0 Then

                                        Try
                                            'Questa serve per dare una colorazione diversa alle righe aggiunte a parità di certificazione
                                            Dr.Item("FF_Righe_Aggiunte") = righeAggiunte

                                            objEti = New AgronicaCoreStampeDAL.FF_Etichette_R

                                            Dim xOrderByFF As String = ""
                                            Dim xFiltroAggiuntivoFF As String = " detProd.Id_Mov_Det = " & DrAgenda(0).Item("Id_Mov_Det")


                                            'etichette
                                            Dim DtProdottoFF = objEti.LeggiParametriQualitativi(
                                            DrAgenda(0).Item("id_Agenda"),
                                            1,
                                            FF_Etichette_tipo.Interne,
                                            DrAgenda(0).Item("cau_mov"),
                                            xFiltroAggiuntivoFF,
                                            "",
                                            objparametri_Server
                                        )

                                            For Each drrProdotto As DataRow In DtProdottoFF.Rows

                                                For Each colProdotto As DataColumn In DtProdottoFF.Columns

                                                    If Not ({"specie", "varieta", "data", "ora", "qtakg", "note"}.Contains(colProdotto.ColumnName.ToLower)) Then

                                                        If Not colProdotto.ColumnName.ToLower.Contains("_sigla") Then

                                                            Dim parametroQualitativo As String = ""
                                                            If Not drrProdotto(colProdotto.ColumnName) Is DBNull.Value Then
                                                                parametroQualitativo = drrProdotto(colProdotto.ColumnName)
                                                            End If

                                                            If Not String.IsNullOrEmpty(parametroQualitativo) Then
                                                                'If colProdotto.ColumnName = "Referenza" Then
                                                                '    strProdotti &=
                                                                '    "<b>" & colProdotto.ColumnName & "</b>: " & parametroQualitativo & "<br>"
                                                                'Else
                                                                Dr.Item("FF_" & colProdotto.ColumnName) = parametroQualitativo
                                                                'End If
                                                            End If
                                                        End If
                                                    End If

                                                Next

                                            Next

                                            If strProdotti <> "" Then
                                                strProdotti &= "<br>"
                                            End If

                                        Catch ex As Exception

                                        End Try

                                        If Dr.Item("Lav_Cod") = LAVCOD_RACCOLTA Then
                                            Dim rMovDest = New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                            Dim rRegImp = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                            Dim rMetaSchema = New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                                            Dim rImpProg = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                            Dim rAppezza = New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                            Dim rMovimenti = New AgronicaCoreContabDAL.Movimenti_R

                                            Dim dtMovDEst = rMovDest.Leggi_Raccolte(Dr.Item("Piva"), Dr.Item("Sa_Cod"), Dr.Item("id_agenda"), Dr.Item("Id_Mov"), Dr.Item("Id_Mov_det"), 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objparametri_Server)
                                            Dim strOperazioni = ""
                                            For Each row In dtMovDEst.Rows
                                                Dim dtImpProg = rImpProg.LeggiMinimal(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), 0, Dr.Item("data"), Dr.Item("data"), "", "", objparametri_Server)
                                                For Each rowd In dtImpProg.Rows
                                                    Dim dtAppezza = rAppezza.Leggi(row("piva"), row("sa_cod"), row("appezza"), AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objparametri_Server)
                                                    Dim AppNome = ""
                                                    If dtAppezza.Rows.Count > 0 Then
                                                        AppNome = dtAppezza(0)("App_Nome")
                                                    End If
                                                    Dim dtOpeImp = rRegImp.Leggi_Operazioni_Impianti_Dettaglio(row("piva"), row("sa_cod"), row("appezza"), row("id_Destinazione"), rowd("Validita_Inizio"), IIf(rowd("Validita_Fine") < Dr.Item("data"), rowd("Validita_Fine"), Dr.Item("data")), "(Agenda.id_agenda<>" + Dr.Item("id_agenda").ToString() + " and Agenda.Lav_Cod<>'" + Dr.Item("Lav_Cod").ToString() + "')", "Movimenti.Data_Movimento Desc", objparametri_Server)
                                                    For Each ope In dtOpeImp.Rows
                                                        strOperazioni &= "( App: " & AppNome & " - data: " & ope("Data_Movimento") & "-"
                                                        strOperazioni &= "lotto: " & ope("Lotto") & "-"
                                                        Dim dtDescItem = rMetaSchema.LeggiTabella_da_CategorieMagazzino(ope("Tabella"), ope("Tabella_Cod"), ope("Tabella_Des"), "", ope("Pro_Cod"), "", "", objparametri_Server)

                                                        If dtDescItem.Rows.Count > 0 Then
                                                            strOperazioni &= "operazione: " & dtDescItem(0)(ope("Tabella_Des")) & "-"
                                                        Else
                                                            strOperazioni &= "operazione: " & ope("des_lib") & "-"
                                                        End If
                                                        strOperazioni &= "quantità: " & ope("Qta") & " [" & ope("UDM_SIM") & "] ) <br>"
                                                        Dim dtCarichi = rMovimenti.LeggiMovimentixTracciabilità(row("piva"), row("sa_cod"), {LAVCOD_CARICO, LAVCOD_BOLLA_RICEVUTA}, ope("Elem_Cod"), ope("Pro_Cod"), ope("Mat_Cod"), CAU_CARICO, AGRODATAINIZIO, AGRODATAFINE, "", "", objparametri_Server)
                                                        If dtCarichi.Rows.Count > 0 Then
                                                            For Each carico In dtCarichi.Rows
                                                                Select Case carico("lav_cod")
                                                                    Case LAVCOD_BOLLA_RICEVUTA
                                                                        strOperazioni &= "  - " + carico("des_lib") + " in data " + carico("Data_Movimento") + " con quantità: " + carico("qta").ToString() + " [" + carico("udm_sim") + "] <br>"
                                                                    Case LAVCOD_CARICO
                                                                        strOperazioni &= "  - " + carico("des_lib") + " in data " + carico("Data_Movimento") + " con quantità: " + carico("qta").ToString() + " [" + carico("udm_sim") + "] <br>"
                                                                    Case Else
                                                                        strOperazioni &= "  -carico generico fatto in data " + carico("Data_Movimento") + " - " + carico("des_lib") + " - con quantità: " + carico("qta").ToString() + " [" + carico("udm_sim") + "] <br>"
                                                                End Select
                                                            Next
                                                        End If
                                                    Next
                                                Next
                                            Next
                                            Dr.Item("FF_Operazioni_Campagna") = strOperazioni

                                        End If
                                    End If


                                Case Else

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desProdotto As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desProdotto = "", "", " (" & desProdotto & ")")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    End If
                            End Select



                            Dim drMovDetTec() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))

                            Dim listaAvv As New List(Of String)

                            For Each drAvv As DataRow In drMovDetTec
                                If drAvv.Item("Av_des_vol") <> "" Then
                                    listaAvv.Add(drAvv.Item("Av_des_vol"))
                                End If
                                If drAvv.Item("Av_Gru_des") <> "" Then
                                    listaAvv.Add(drAvv.Item("Av_Gru_des"))
                                End If
                            Next

                            strAvversita = String.Join(", ", listaAvv)

                            '----------------------------------
                            'Centri e Campi
                            Dim listaCentriCampi As New List(Of String)
                            For Each drCentriCampi As DataRow In DtOperazione.Rows

                                Dim centro As String = If(Not IsDBNull(drCentriCampi.Item("Sa_Nome")) AndAlso Not IsNothing(drCentriCampi.Item("Sa_Nome")), drCentriCampi.Item("Sa_Nome"), "")
                                Dim campo As String = If(Not IsDBNull(drCentriCampi.Item("Campo_Des")) AndAlso Not IsNothing(drCentriCampi.Item("Campo_Des")), drCentriCampi.Item("Campo_Des"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                Dim testoCentriCampi As String = String.Join(" - ", {centro, campo}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaCentriCampi.Contains(testoCentriCampi) Then
                                    listaCentriCampi.Add(testoCentriCampi)
                                End If
                            Next

                            strCentroCampo = String.Join(", ", listaCentriCampi)

                            '----------------------------------
                            'Lotti Produzione
                            Dim listaLottiProduzione As New List(Of String)
                            For Each drLottiProduzione As DataRow In DtOperazione.Rows

                                Dim LottoProduzione As String = If(Not IsDBNull(drLottiProduzione.Item("LottoProduzione")), drLottiProduzione.Item("LottoProduzione"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If LottoProduzione.Trim() <> "" AndAlso Not listaLottiProduzione.Contains(LottoProduzione) Then
                                    listaLottiProduzione.Add(LottoProduzione)
                                End If
                            Next

                            strLottiProduzione = String.Join(", ", listaLottiProduzione)

                            '----------------------------------
                            'Lotti Impianto
                            Dim listaLottiImpianto As New List(Of String)
                            For Each drLottiImpianto As DataRow In DtApp.Rows

                                Dim LottoImpianto As String = If(Not IsDBNull(drLottiImpianto.Item("LottoImpianto")), drLottiImpianto.Item("LottoImpianto"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If LottoImpianto.Trim() <> "" Then
                                    listaLottiImpianto.Add(LottoImpianto)
                                End If
                            Next

                            strLottiImpianto = String.Join(", ", listaLottiImpianto)

                            '----------------------------------
                            'Note a checkbox
                            Dim listaNote As New List(Of String)
                            For Each drNote As DataRow In DtOperazione.Rows
                                Dim Nota As String = If(Not IsDBNull(drNote.Item("Nota_Des")), drNote.Item("Nota_Des"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                    listaNote.Add(Nota)
                                End If
                            Next

                            'Nota libera
                            For Each drNote As DataRow In DtOperazione.Rows
                                Dim Nota As String = If(Not IsDBNull(drNote.Item("Mov_desc")), drNote.Item("Mov_desc"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                    listaNote.Add(Nota)
                                End If
                            Next

                            strNote = String.Join(", ", listaNote)

                            '----------------------------------
                            'Costi 
                            Dim listaOperatori As New List(Of String)
                            Dim listaMacchine As New List(Of String)
                            'Dim listaPatentini As New List(Of String)
                            'Dim listaTitolari As New List(Of String)
                            'Dim listaScadenze As New List(Of String)

                            If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then

                                Dim DrCosti() As DataRow = DtCosti.Select("Id_Agenda=" & current_Agenda)

                                If Not IsNothing(DrCosti) Then
                                    For Each dr_costo As DataRow In DrCosti

                                        'è un record manodopera
                                        If dr_costo.Item("Cod_RisUm") <> 0 Then

                                            'Recupero il nome del contatto
                                            Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                                            If Not listaOperatori.Contains(nomeContatto) Then
                                                listaOperatori.Add(nomeContatto)
                                            End If

                                            'If dr_costo.Item("Cau_Mov") = CAU_IMPUTAZIONE_TERZISTI Then
                                            '    nomeContatto &= " (Terzista)" & vbCrLf
                                            'End If

                                            ''Controllo se è un responsabile o un operatore
                                            'If Dr.Item("Cau_Mov") <> CAU_IMPUTAZIONE_TECNICO_RESPONSABILE Then

                                            '    strOperatori &= nomeContatto & vbCrLf

                                            '    'PATENTINO
                                            '    If Not IsDBNull(Dr.Item("patentino")) AndAlso Dr.Item("patentino") <> "" Then

                                            '        If Not listaPatentini.Contains(Dr.Item("patentino")) Then
                                            '            listaPatentini.Add(Dr.Item("patentino"))
                                            '        End If

                                            '        If Not listaTitolari.Contains(nomeContatto) Then
                                            '            listaTitolari.Add(nomeContatto)
                                            '        End If

                                            '        If IsDate(Dr.Item("data_scadenza_patentino")) AndAlso
                                            '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                                            '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then

                                            '            If Not listaScadenze.Contains(Dr.Item("data_scadenza_patentino")) Then
                                            '                listaScadenze.Add(Dr.Item("data_scadenza_patentino"))
                                            '            End If

                                            '        End If
                                            '    End If
                                            'Else

                                            '    strResponsabili &= nomeContatto & vbCrLf

                                            'End If

                                        Else
                                            'è un record macchinario

                                            Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                                            detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                                            detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")
                                            'detMacchina &= If(dr_costo.Item("Ultima_Manutenzione") <> "01/01/1900", " - Ultima Manutenzione " & dr_costo.Item("Ultima_Manutenzione"), "")

                                            If Not listaMacchine.Contains(detMacchina) Then
                                                listaMacchine.Add(detMacchina)
                                            End If

                                        End If

                                    Next
                                End If
                            End If

                            strCosti_Operatori = String.Join(", ", listaOperatori)
                            strCosti_Macchine = String.Join(", ", listaMacchine)

                            '----------------------------------
                            'Superficie Trattata
                            Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                            Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(DtOperazione, "APPEZZA", "ID_REG", False)

                            For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                                Dim drAppezza() As DataRow = DtOperazione.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                                If drAppezza.Length > 0 Then
                                    Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                                End If
                            Next


                            '----------------------------------
                            'Dettaglio Tecnico
                            Dim listaDetTec As New List(Of String)

                            For Each drDetTec As DataRow In DtOperazione.Rows

                                Dim prod As String = ""
                                Dim princAtt As String = ""
                                Dim avv As String = ""
                                Dim fasifeno As String = ""

                                'PRODOTTI
                                Select Case CInt(drDetTec.Item("Elem_Cod"))
                                    Case FERTILIZZANTI

                                        If drDetTec.Item("Pro_Cod") <> 0 Then
                                            prod = drDetTec.Item("Fer_Des") & ", "
                                        End If

                                        If drDetTec.Item("Mat_Cod") <> 0 Then
                                            prod = drDetTec.Item("Mat_Des")
                                        Else
                                            If prod.Length > 0 Then
                                                prod = Left(prod, prod.Length - 2)
                                            End If
                                        End If

                                    Case FORMULATI

                                        If drDetTec.Item("Fr_Des") <> "" Then
                                            prod = drDetTec.Item("Fr_Des")
                                        End If

                                    Case TRAPPOLE

                                        If drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")
                                        End If

                                    Case SEMENTI

                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        If drDetTec.Item("Mat_Des") <> "" Then

                                            Select Case drDetTec.Item("Lav_Cod")
                                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                    prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                Case Else
                                                    prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                            End Select
                                        End If

                                    Case Else
                                        Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                End Select

                                'AVVERSITA'
                                Dim drMovDetTec2() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                                Dim listaAvv2 As New List(Of String)

                                For Each drAvv2 As DataRow In drMovDetTec2
                                    If drAvv2.Item("Av_des_vol") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                                    End If
                                    If drAvv2.Item("Av_Gru_des") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                                    End If
                                Next

                                avv = String.Join(", ", listaAvv2)

                                'PRINCIPI ATTIVI / SOSTANZE ATTIVE
                                Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

                                If drDetTec.Item("PrincipiAttivi") <> "" Then
                                    codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
                                Else
                                    If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 _
                                    AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                                        codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                                    End If
                                End If

                                Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
                                Dim listaPrincAttNomi As New List(Of String)
                                For Each pa As String In listaPrincAtt
                                    listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
                                Next
                                princAtt = String.Join(", ", listaPrincAttNomi)


                                'FASI FENOLOGICHE
                                Dim drMovDetTec2Fasi() As DataRow = dtMovDetTecFasi.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                                Dim listaFasi As New List(Of String)

                                For Each drFasi As DataRow In drMovDetTec2Fasi
                                    Dim Fase_Des As String = ""
                                    If drFasi.Item("ff_classe") <> 0 Then

                                        Select Case drFasi.Item("ff_classe")
                                            Case < 1000 'caso vecchio av_cod = ff_cod
                                                Fase_Des = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                                            Where aa.FF_Cod = drFasi.Item("ff_classe")
                                                            Select aa.Descrizione
                                                        ).FirstOrDefault

                                            Case Else 'caso nuovo av_cod = cod_css
                                                Fase_Des = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                            Where aa.Cod_SS = drFasi.Item("ff_classe")
                                                            Select aa.Descrizione & " - BBCH " & aa.Stadio
                                                        ).FirstOrDefault

                                        End Select
                                        If Fase_Des <> "" Then
                                            listaFasi.Add(Fase_Des)
                                        End If

                                    End If

                                Next

                                fasifeno = String.Join(", ", listaFasi)

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                'Dim testoDetTec As String = prod & If(princAtt <> "", " - " & princAtt, "") & If(avv <> "", " - " & avv, "")
                                Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv, fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaDetTec.Contains(testoDetTec) Then
                                    listaDetTec.Add(testoDetTec)
                                End If

                            Next

                            strDettaglioTecnico = String.Join(", ", listaDetTec)


                        End If




                        'modifica per magazzino
                        Dim lc As Integer = DrAgenda(0).Item("Lav_Cod")
                        If {LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO}.Contains(lc) Then

                            If bool_isCuraEnabled Then

                                strProdotti = DrAgenda(0)("Des_Lib")

                            Else

                                '----------------------------------
                                'prodotti
                                Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                                    Case FERTILIZZANTI  'FERTILIZZANTI

                                        DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                        DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)

                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Pro_Cod") <> 0 Then
                                                Prodotto = r.Item("Fer_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        For Each r As DataRow In DtProdotti1.Rows
                                            If r.Item("Mat_Cod") <> 0 Then
                                                Prodotto = r.Item("Mat_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case FORMULATI    'FORMULATI

                                        DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)

                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Fr_Des") <> "" Then
                                                Prodotto = r.Item("Fr_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case TRAPPOLE

                                        DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)

                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Trap_Des") <> "" Then
                                                Prodotto = r.Item("Trap_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case SEMENTI

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Mat_Des") <> "" Then
                                                Prodotto = r.Item("Mat_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                        End If

                                    Case Else

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        End If

                                End Select

                            End If


                            If bool_isCuraEnabled Then

                                strDettaglioTecnico = DrAgenda(0)("Des_Lib")

                            Else
                                '----------------------------------
                                'Dettaglio Tecnico
                                Dim listaDetTec As New List(Of String)
                                For Each drDetTec As DataRow In DtOperazione.Rows
                                    Dim prod As String = ""

                                    'PRODOTTI
                                    Select Case CInt(drDetTec.Item("Elem_Cod"))
                                        Case FERTILIZZANTI

                                            If drDetTec.Item("Pro_Cod") <> 0 Then
                                                prod = drDetTec.Item("Fer_Des") & ", "
                                            End If

                                            If drDetTec.Item("Mat_Cod") <> 0 Then
                                                prod = drDetTec.Item("Mat_Des")
                                            Else
                                                If prod.Length > 0 Then
                                                    prod = Left(prod, prod.Length - 2)
                                                End If
                                            End If

                                        Case FORMULATI

                                            If drDetTec.Item("Fr_Des") <> "" Then
                                                prod = drDetTec.Item("Fr_Des")
                                            End If

                                        Case TRAPPOLE

                                            If drDetTec.Item("Trap_Des") <> "" Then
                                                prod = drDetTec.Item("Trap_Des")
                                            End If

                                        Case SEMENTI

                                            If drDetTec.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            End If

                                        Case SEMILAVORATI_VEGETALI

                                            If drDetTec.Item("Mat_Des") <> "" Then

                                                Select Case drDetTec.Item("Lav_Cod")
                                                    Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                        prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                    Case Else
                                                        prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                                End Select
                                            End If

                                        Case Else
                                            If drDetTec.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            End If

                                    End Select


                                    'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                    Dim testoDetTec As String = String.Join(" - ", {prod}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                    If Not listaDetTec.Contains(testoDetTec) Then
                                        listaDetTec.Add(testoDetTec)
                                    End If

                                Next

                                strDettaglioTecnico = String.Join(", ", listaDetTec)
                            End If




                        End If


                        'modifica per contabilita
                        Select Case lc
                            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_MVV_RICEVUTO, LAVCOD_MVV_EMESSO
                                'occorre pescare i prodotti dai dettagli, non vengono su dalla query perche nei doc contabili sacod agenda è 0 probabilmente
                                'per ora lascio stare, leggo la descrizione mov_desc
                                Try

                                    Dim strRifDdtFatture As String = ""
                                    Dim strMov_Desc As String = ""

                                    For Each r As DataRow In DrAgenda

                                        If Not IsDBNull(r.Item("Mov_Desc")) AndAlso r.Item("Mov_Desc") <> "" Then
                                            If Not strDettagli.Contains(r.Item("Mov_Desc")) Then
                                                strDettagli &= r.Item("Mov_Desc") & " <br> "
                                            End If
                                        End If

                                        If Not String.IsNullOrEmpty(r.Item("Mov_Desc")) Then
                                            strMov_Desc = r.Item("Mov_Desc")
                                        End If


                                        If Not String.IsNullOrEmpty(r.Item("RifDdtFatture")) Then
                                            strRifDdtFatture = "Rif: " & r.Item("RifDdtFatture")
                                        End If

                                    Next

                                    strDettaglioTecnico = String.Join(" ", {strMov_Desc.Trim(), strRifDdtFatture.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                Catch ex As Exception

                                End Try
                        End Select

                        'Modifica per operazione di cura
                        If lc = LAVCOD_CURA Then
                            Dim lottoRaccolto As String = (From riga As DataRow In DtOperazione.Rows Where riga.Item("cau_mov") = CAU_SCARICO AndAlso riga.Item("tipo_destinazione") = TIPO_DESTINAZIONE_MAGAZZINO Select riga.Item("lottoProduzione")).First()
                            strDettaglioTecnico = "Lotto Raccolto: " & lottoRaccolto
                        End If


                        'Controllo i permessi di modifica
                        If Not FF_TrackedData_Cod > 0 Then
                            'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
                            If ht_Permessi.ContainsKey(Lav_Cod) Then
                                Dr.Item("PermessoModifica") = ht_Permessi(Lav_Cod)
                            Else
                                Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(
                                                            Lav_Cod, enum_Security_Operazione.Modifica,
                                                            objparametri_Server, objparametri_Utenti, HttpContext.Current.Session)

                                ht_Permessi.Add(Lav_Cod, permesso)
                            End If
                        End If


                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

                        Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, Resources.AgronicaAgenda_2010.Si, Resources.AgronicaAgenda_2010.No)

                        Testo = "<a " &
                                "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
                                Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
                                Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
                                "' " &
                                ">" &
                                Icona_INFO &
                                "</a>"

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        If strCentro <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & strCentro & "<br>"
                        End If

                        If FF_TrackedData_Cod <= 0 And strSpecie <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.Specie & "</b> " & strSpecie & "<br>"
                        End If

                        'per le operazioni colturali visualizzo gli appezzamenti coinvolti
                        If strAppezzamenti <> "" Then
                            If strProdotti <> "" Then
                                strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti & "<br>" & strProdotti
                            Else
                                strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti
                            End If
                        Else
                            strDettagli &= strProdotti
                        End If

                        'avversita
                        strDettagli &= If(strAvversita <> "", "<br> <b> Avversità: </b> " & strAvversita & "<br>", "")

                        'If Ricetta <> "" Then
                        '    strDettagli &= If(strDettagli <> "", vbCrLf & Ricetta, Ricetta)

                        '    strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & Ricetta
                        'End If

                        Dr.Item("Info") = Testo
                        Dr.Item("Dettagli") = strDettagli

                        Dr.Item("cul_des") = strCulDes
                        Dr.Item("Specie_Varieta") = strSpecieVarieta
                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Centro_Campo") = strCentroCampo
                        Dr.Item("LottiProduzione") = strLottiProduzione
                        Dr.Item("LottiImpianto") = strLottiImpianto
                        Dr.Item("Note") = strNote
                        Dr.Item("Costi_Operatori") = strCosti_Operatori
                        Dr.Item("Costi_Macchine") = strCosti_Macchine
                        Dr.Item("Sup_Trattata") = Sup_TrattataTot

                        'PEr Rilievo Piogge
                        If DrAgenda(0).Item("Lav_Cod") = LAVCOD_RILIEVO_PIOGGE Then

                            strDettagli = "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(Piva, Sa_Cod, objparametri_Server)
                            '&= " [ Pioggia: " & Pioggia & " mm; TMin: " & TMin & " °C; TMax: " & TMax & " °C; Umidita': " & Umidita & "% ] "
                            Dr.Item("Dettagli") = strDettagli

                        End If

                        If FF_TrackedData_Cod > 0 Then
                            If String.IsNullOrEmpty(Dr.Item("FF_Referenza").ToString) AndAlso Not String.IsNullOrEmpty(strProdotti) AndAlso DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                                Dr.Item("FF_Referenza") = strProdotti
                            End If
                        End If
                        'Dr.Item("gru_des") = DrAgenda(0).Item("tipo")

                        Dr.Item("Centro_Aziendale") = DrAgenda(0).Item("sa_nome")
                        Dr.Item("Specie") = DrAgenda(0).Item("veg_des")
                        Dr.Item("Appezzamenti_Coinvolti") = strAppezzamenti
                        Dr.Item("Prodotti_Utilizzati") = strProdotti
                        Dr.Item("Avversita") = strAvversita

                        Dr.Item("chiave_composita") = CDate(DrAgenda(0).Item("Data_Movimento")) & "_" &
                                                      DrAgenda(0).Item("Id_Agenda") & "_" &
                                                      DrAgenda(0).Item("Lav_Cod") & "_" &
                                                      DrAgenda(0).Item("Piva") & "_" &
                                                      DrAgenda(0).Item("Sa_Cod") & "_" &
                                                      DrAgenda(0).Item("Blocco_Flag") & "_" &
                                                      DrAgenda(0).Item("Veg_Cod")

                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing

        Dt.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(Dt)

        If FromOutToIn Then
            Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"
        Else
            Dv.Sort = " Data2 ASC, Ora ASC, Id_Agenda ASC"
        End If

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function


    'Funzione che crea un task per ogni processore
    'A cui viene passata una porzione di Datatable
    'I datatable vengono poi ricongiunti 
    'al termine di tutti i task
    Public Shared Function Carica_LavorazioniParallel(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Veg_Cod As Integer,
            ByVal id_cod As Integer,
            ByVal Cul_Cod As Integer,
            ByVal Tipo As String,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal Flag_TerrenoNudo As Boolean,
            ByVal xFiltroAggiuntivo_colturali As String,
            ByVal xFiltroAggiuntivo_postRaccolta As String,
            ByVal xFiltroAggiuntivo_contabili As String,
            ByVal xFiltroAggiuntivo_contabili_Macchine As String,
            ByVal xFiltroAggiuntivo_contabili_Audit As String,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri,
            ByVal FF_TrackedData_Cod As Integer,
            Optional ByVal FromOutToIn As Boolean = True,
            Optional ByVal cCertificazione As Integer = True,
            Optional ByVal righeAggiunte As String = "",
            Optional ByVal Visualizza_Codici_AppezzaImpianti As Boolean = False,
            Optional ByVal Visualizza_KPIN_BlockName As Boolean = False,
            Optional ByVal TipoOperazioni As List(Of Integer) = Nothing,
            Optional ByVal impianti As List(Of String) = Nothing
        ) As DataTable

        Dim DtAgenda As New DataTable

        Dim i As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objparametri_Server)

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)


        Dim filtro As String = "|"
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
                'No un filtro
                filtro = HttpContext.Current.Session("Filtro")
            End If
        End If

        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
                Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
            End If
        End If


        Dim Filtro_Utente_Lavorazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
                Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
            End If
        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If

        'Identifico se è abilitata l'operazione di cura
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO, objparametri_Utenti)
        Dim bool_isCuraEnabled As Boolean = (IsNumeric(Tipo_Raccolta_Val) AndAlso Tipo_Raccolta_Val = enum_RACCOLTA_TIPO.Raccolta_e_Cura)

        If (Veg_Cod <> 0 AndAlso Veg_Cod <> -1) Or id_cod <> 0 Then
            Filtro_Tipo_GruppoOperazioni = "( " & Filtro_Tipo_GruppoOperazioni & " ) AND GruppoOperazioni.Tipo = 'C'  "
        End If

        If Filtro_Tipo_GruppoOperazioni <> "" AndAlso TipoOperazioni.Count > 0 Then
            Filtro_Tipo_GruppoOperazioni = "( " & Filtro_Tipo_GruppoOperazioni & " ) AND "
            Dim filtroTipoOperazioni_str = " GruppoOperazioni.Gru_Cod IN ("
            For Each Tipos In TipoOperazioni
                filtroTipoOperazioni_str &= Tipos & ","
            Next
            filtroTipoOperazioni_str = filtroTipoOperazioni_str.Substring(0, filtroTipoOperazioni_str.Length - 1)
            filtroTipoOperazioni_str &= ") "
            Filtro_Tipo_GruppoOperazioni &= filtroTipoOperazioni_str
        End If

        Dim strFiltroAgenda As String = ""
        If impianti IsNot Nothing AndAlso impianti.Count > 0 Then
            Dim DT_ID_Agenda As DataTable

            Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            For Each imp In impianti
                Dim impArr = imp.Split("_")
                Dim dtAgImp = objMov_Destinazioni.Leggi_DistinctID_Agenda_Impianti(objparametri_Server,
                                                                     impArr(0),
                                                                     impArr(1),
                                                                     impArr(2),
                                                                     impArr(3),
                                                                     Validita_Inizio,
                                                                     Validita_Fine,
                                                                     "",
                                                                     "")

                If DT_ID_Agenda Is Nothing Then
                    DT_ID_Agenda = dtAgImp.Copy
                Else
                    DT_ID_Agenda.Merge(dtAgImp)
                End If

            Next

            If DT_ID_Agenda.Rows.Count > 0 Then
                DT_ID_Agenda = DT_ID_Agenda.DefaultView.ToTable(True, "ID_Agenda")
                strFiltroAgenda = " Agenda.ID_Agenda IN ( "
                For Each rowAgImp In DT_ID_Agenda.Rows
                    strFiltroAgenda &= rowAgImp(0) & ","
                Next
                strFiltroAgenda = strFiltroAgenda.Substring(0, strFiltroAgenda.Length - 1)
                strFiltroAgenda &= ") "
                If filtrolavorazioni = "" Then
                    filtrolavorazioni = strFiltroAgenda
                Else
                    filtrolavorazioni &= "AND " & strFiltroAgenda
                End If
            Else
                strFiltroAgenda = " Agenda.ID_Agenda = 0 "
                If filtrolavorazioni = "" Then
                    filtrolavorazioni = strFiltroAgenda
                Else
                    filtrolavorazioni &= "AND " & strFiltroAgenda
                End If
            End If

        End If

        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Veg_Cod,
                Cul_Cod,
                Tipo,
                Gru_Cod,
                Lav_Cod,
                Flag_TerrenoNudo,
                True,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xFiltroAggiuntivo_colturali,
                xFiltroAggiuntivo_postRaccolta,
                xFiltroAggiuntivo_contabili,
                xFiltroAggiuntivo_contabili_Macchine,
                xFiltroAggiuntivo_contabili_Audit,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server"),
                FF_TrackedData_Cod, FromOutToIn,
                Visualizza_Codici_AppezzaImpianti:=Visualizza_Codici_AppezzaImpianti,
                Visualizza_KPIN_BlockName:=Visualizza_KPIN_BlockName,
                id_cod:=id_cod
                )


        Catch ex As Exception

            Return Nothing

        End Try

        '----------------------------------
        'Leggo tutti i principi attivi
        Dim HtProdPA As New Hashtable()
        Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

        '03/01/2018 Grilli: Leggo le Avversità fuori dalla megaLettura
        Dim objMovTec As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
        Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_agenda(Piva, "", "", objparametri_Server)

        '(12/11/2018 fede) aggiunta indicazione fase fenologica (splittate le operazioni)
        'leggo le fasi via web service
        Dim dtMovDetTecFasi As DataTable = objMovTec.Leggi_x_agenda_fasifenologiche(Piva, "", "", objparametri_Server)
        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        If Not dtMovDetTecFasi Is Nothing AndAlso dtMovDetTecFasi.Rows.Count > 0 Then

            Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
            Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

            Dim Filtro_cod_ss As String = ""
            Dim Filtro_ff_cod As String = ""
            Dim Hash_cod_ss As New Hashtable
            Dim Hash_ff_cod As New Hashtable
            Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objparametri_Utenti, 2)
            If imp = "1" Then
                objParametriIngresso.Personalizzate = True
            End If
            objParametriIngresso.Lingua_Cod = objparametri_Server.Lingua_Cod

            For f = 0 To dtMovDetTecFasi.Rows.Count - 1
                Select Case dtMovDetTecFasi.Rows(f).Item("ff_classe")
                    Case < 1000
                        If Not Hash_ff_cod.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                            Hash_ff_cod.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                            Filtro_ff_cod &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                        End If
                    Case Else
                        If Not Hash_cod_ss.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                            Hash_cod_ss.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                            Filtro_cod_ss &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                        End If
                End Select
            Next

            If Filtro_ff_cod <> "" Then
                objParametriIngresso.strFiltro = " fs.ff_cod in (" & Left(Filtro_ff_cod, Filtro_ff_cod.Length - 1) & ")"
                objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
            End If
            If Filtro_cod_ss <> "" Then
                objParametriIngresso.strFiltro = " ss.cod_ss in (" & Left(Filtro_cod_ss, Filtro_cod_ss.Length - 1) & ")"
                objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
            End If

        End If

        Dim leggiCategMag As New Categorie_Magazzino_R
        Dim DtCategorieMagazzino As DataTable
        If HttpContext.Current.Cache("DtCategorieMagazzino") Is Nothing Then
            DtCategorieMagazzino = leggiCategMag.Leggi(0, "", True, "", "", objparametri_Server)
            HttpContext.Current.Cache("DtCategorieMagazzino") = DtCategorieMagazzino
        Else
            DtCategorieMagazzino = HttpContext.Current.Cache("DtCategorieMagazzino")
        End If

        Dim sa_nome = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(Piva, Sa_Cod, objparametri_Server)

        Dim ht_Permessi As New Hashtable

        'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
        If Not ht_Permessi.ContainsKey(Lav_Cod) Then
            Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(
                                                            Lav_Cod, enum_Security_Operazione.Modifica,
                                                            objparametri_Server, objparametri_Utenti, HttpContext.Current.Session)

            ht_Permessi.Add(Lav_Cod, permesso)
        End If

        Dim DT As New DataTable
        creaDTAgenda(DT, Visualizza_Codici_AppezzaImpianti, Visualizza_KPIN_BlockName, FF_TrackedData_Cod, DTParamQual)

        If DtAgenda.Rows.Count > 0 Then
            Dim lockObject As New Object
            Dim taskNumber = Environment.ProcessorCount
            Dim taskList As New List(Of Task)
            Dim dtList As New List(Of DataTable)

            Dim DT_ID_Agenda = DtAgenda.DefaultView().ToTable(True, "ID_Agenda")
            Dim listID_Agenda = DT_ID_Agenda.Rows.OfType(Of DataRow).Select(Function(dr) dr.Field(Of Integer)("ID_Agenda")).ToList

            If listID_Agenda.Count < 100 Then
                taskNumber = 1
            End If

            Dim stepAgenda = CInt(DT_ID_Agenda.Rows.Count / taskNumber)

            For i = 0 To taskNumber - 1
                Dim subListAgenda = New List(Of Integer)
                Dim startIndex = i * stepAgenda
                Dim endIndex = (i * stepAgenda) + stepAgenda - 1
                If i = taskNumber - 1 Then
                    endIndex = listID_Agenda.Count - 1
                End If
                If listID_Agenda.Count > 1 Then
                    subListAgenda = listID_Agenda.GetRange(startIndex, endIndex - startIndex + 1)
                Else
                    subListAgenda = listID_Agenda
                End If


                Dim DT_AgendaxTask = (From r As DataRow In DtAgenda.Rows Where subListAgenda.Contains(r.Item("ID_Agenda")) Select r).CopyToDataTable

                Dim t As New Task(Function()
                                      Dim DT_Task = getDTAgenda(DT_AgendaxTask,
                                                                 Piva,
                                                                 Sa_Cod,
                                                                 sa_nome,
                                                                 Veg_Cod,
                                                                 Lav_Cod,
                                                                 righeAggiunte,
                                                                 FF_TrackedData_Cod,
                                                                 objparametri_Server,
                                                                 objparametri_Utenti,
                                                                 bool_isCuraEnabled,
                                                                 Visualizza_Codici_AppezzaImpianti,
                                                                 Visualizza_KPIN_BlockName,
                                                                 HtProdPA,
                                                                 HtPrincAtt,
                                                                 ht_Permessi,
                                                                 DTParamQual,
                                                                 DtCategorieMagazzino,
                                                                 dtMovDetTec,
                                                                 dtMovDetTecFasi,
                                                                 objParametriUscitaFasiOld,
                                                                 objParametriUscitaFasiNew,
                                                                 lockObject)
                                      dtList.Add(DT_Task)
                                  End Function)

                taskList.Add(t)

                t.Start()


            Next

            Task.WaitAll(taskList.ToArray)

            For ii = 0 To dtList.Count - 1

                DT.Merge(dtList(ii))

            Next

        End If


        DT.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(DT)

        If FromOutToIn Then
            Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"
        Else
            Dv.Sort = " Data2 ASC, Ora ASC, Id_Agenda ASC"
        End If

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function


    Public Shared Function getDTAgenda(DtAgenda As DataTable,
                                       Piva As String,
                                       sa_cod As Integer,
                                       sa_nome As String,
                                       Veg_Cod As Integer,
                                       Lav_Cod As Integer,
                                       righeAggiunte As String,
                                       FF_TrackedData_Cod As Integer,
                                       objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       bool_isCuraEnabled As Boolean,
                                       Visualizza_Codici_AppezzaImpianti As Boolean,
                                       Visualizza_KPIN_BlockName As Boolean,
                                       HtProdPA As Hashtable,
                                       HtPrincAtt As Hashtable,
                                       ht_Permessi As Hashtable,
                                       DTParamQual As DataTable,
                                       DtCategorieMagazzino As DataTable,
                                       dtMovDetTec As DataTable,
                                       dtMovDetTecFasi As DataTable,
                                       objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                       objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                       lockObject As Object) As DataTable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim objEti As AgronicaCoreStampeDAL.FF_Etichette_R

        Dim Dt As New DataTable

        creaDTAgenda(Dt, Visualizza_Codici_AppezzaImpianti, Visualizza_KPIN_BlockName, FF_TrackedData_Cod, DTParamQual)

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strCodici_Appezzamenti As String
        Dim strCodici_Impianto As String
        Dim strKPIN As String
        Dim strBlockName As String

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim DtCosti As New DataTable

        If FF_TrackedData_Cod > 0 Then
            strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "Id_Mov_Det")
        Else
            strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

            'Leggo i costi accessori
            Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
            '16/09/2019: il filtro date lo fa più sotto
            SyncLock lockObject
                DtCosti = objCostiAccessori.CostiAccessori_from_IdAgenda3(Piva, String.Join(",", strId_Agenda), "", "", objparametri_Server)
            End SyncLock
        End If

        If Not strId_Agenda Is Nothing Then

            For Each current_Agenda In strId_Agenda

                'AZZERO LE STRINGHE AD OGNI GIRO
                strAppezzamenti = ""
                strCulDes = ""
                strProdotti = ""
                strAvversita = ""
                strNote = ""
                strCosti_Operatori = ""
                strCosti_Macchine = ""
                Sup_TrattataTot = 0

                strCodici_Appezzamenti = ""
                strCodici_Impianto = ""
                strKPIN = ""
                strBlockName = ""

                If FF_TrackedData_Cod > 0 Then
                    DrAgenda = DtAgenda.Select("Id_Mov_Det=" & current_Agenda)
                Else
                    DrAgenda = DtAgenda.Select("id_agenda=" & current_Agenda)
                End If

                'DtOperazione = DtAgenda.Clone

                DtOperazione = DrAgenda.CopyToDataTable

                'For Each r As DataRow In DrAgenda
                '    DtOperazione.ImportRow(r)
                'Next


                If DrAgenda.Length > 0 Then

                    Dr = Dt.NewRow
                    '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                    Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                    Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                    Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")

                    Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                    Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))

                    '(05/12/2018) per le fasi visualizzo la data del rilievo (validita_inizio nella destinazione)
                    'le fasi nuove creano un id_agenda per centro, fase, data
                    'le vecchie ne avevano 1 per centro con fasi e date diverse assieme
                    '(per queste ultime visualizzo una data in caso ci siano date diverse nello stesso rilievo)
                    Select Case Dr.Item("Lav_Cod")

                        Case LAVCOD_FASI_FENOLOGICHE

                            Dr.Item("Data") = CDate(DrAgenda(0).Item("validita_inizio_destinazione")).ToShortDateString
                            Dr.Item("Data2") = CDate(DrAgenda(0).Item("validita_inizio_destinazione"))

                    End Select

                    Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                    Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                    Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                    Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
                    Dr.Item("Blocco_Flag") = 0
                    Dr.Item("Info") = ""
                    Dr.Item("Dettagli") = ""
                    Dr.Item("Veg_Cod") = 0
                    Dr.Item("Ricetta_Cod") = 0
                    Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                    Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
                    Dr.Item("tipo") = DrAgenda(0).Item("tipo")

                    'Se è un altre lavorazioni aggiungo il dettaglio
                    If DrAgenda(0).Item("attivitaDesc") <> "" Then
                        'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
                        Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                    End If

                    If DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                        Dr.Item("FF_Track_Cal_Cod_Padre") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod_Padre"))
                        Dr.Item("FF_Track_Cal_Cod") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod"))
                        Dr.Item("FF_Track_Lotto_Padre") = DrAgenda(0).Item("FF_Track_Lotto_Padre")
                        Dr.Item("FF_Track_Lotto") = DrAgenda(0).Item("FF_Track_Lotto")
                        Dr.Item("FF_Track_Qta_Extra_Totale") = DrAgenda(0).Item("FF_Track_Qta_Extra_Totale")
                        Dr.Item("FF_Track_Qta_Contenitori") = DrAgenda(0).Item("FF_Track_Qta_Contenitori")
                        Dr.Item("FF_Track_Qta_Imballi") = DrAgenda(0).Item("FF_Track_Qta_Imballi")
                        Dr.Item("FF_codice_generazione") = DrAgenda(0).Item("FF_codice_generazione")
                        Dr.Item("FF_Mat_Cod") = DrAgenda(0).Item("Mat_Cod")
                        Dr.Item("FF_Linea_Cod") = DrAgenda(0).Item("FF_Linea_Cod")
                        'Else
                        '    Dr.Item("FF_Track_Cal_Cod_Padre") = 0
                        '    Dr.Item("FF_Track_Cal_Cod") = 0
                        '    Dr.Item("FF_Track_Lotto_Padre") = ""
                        '    Dr.Item("FF_Track_Lotto") = ""
                        '    Dr.Item("FF_Track_Qta_Extra_Totale") = 0.0

                    End If

                        ' @Paolo
                        ' aggiunta colore per tipologia di lavorazione
                        Select Case DrAgenda(0).Item("tipo")
                        Case "C" ' Colturali
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: green'></i>"
                        Case "E" ' Contabili
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: blue'></i>"
                        Case "V" ' Audit / Monitoraggi
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: orange'></i>"
                        Case "F" ' Macchine
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: red'></i>"
                        Case "Z" ' Zootecniche
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: yellow'></i>"
                    End Select

                    strAppezzamenti = ""
                    strCulDes = ""
                    strProdotti = ""
                    strAvversita = ""
                    strSpecie = ""
                    strCentro = ""
                    Prodotto = ""
                    Ricetta = ""
                    strSpecieVarieta = ""
                    strDettaglioTecnico = ""
                    strCentroCampo = ""
                    strLottiProduzione = ""
                    strLottiImpianto = ""

                    If FF_TrackedData_Cod > 0 Then
                        strDettagli = DrAgenda(0)("Des_Lib") & "<br/>" & DrAgenda(0)("Fabbricato_Des") & "<br/>"
                    Else
                        strDettagli = ""
                    End If

                    If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                        Ricetta = "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                    End If
                    Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                    Dr.Item("Ricetta_Des") = DrAgenda(0).Item("ricetta_numero")
                    'modifica per contabilità magazzino
                    If operazioneLavCodContabMagazzino(DrAgenda(0).Item("lav_cod")) Then

                        'Exit For

                        'specie
                        DtSpecie = objSqlDis.SelectDistinct("Specie", DtOperazione, "veg_cod", False)
                        'DtSpecie = DtOperazione.DefaultView.ToTable(True, "veg_cod", "veg_des", "DestinazioneTerreniNudi_Des", "appezza")

                        For Each r As DataRow In DtSpecie.Rows
                            Veg_Cod = r.Item("veg_cod")

                            If r.Item("veg_cod") <> 0 Then
                                'Veg_Cod = DtSpecie.Rows(j).Item("veg_cod")
                                Dim Veg_Des As String = r.Item("veg_des")
                                If strSpecie <> "" Then
                                    strSpecie &= ", " & Veg_Des
                                Else
                                    strSpecie = Veg_Des
                                End If

                            ElseIf r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                'Veg_Cod = r.Item("veg_cod")
                                Dim DestinazioneTerreniNudi_Des As String = r.Item("DestinazioneTerreniNudi_Des")
                                If strSpecie <> "" Then
                                    strSpecie &= ", " & DestinazioneTerreniNudi_Des
                                Else
                                    strSpecie = DestinazioneTerreniNudi_Des
                                End If

                            ElseIf r.Item("appezza") <> 0 Then
                                'Veg_Cod = r.Item("veg_cod")
                                If strSpecie <> "" Then
                                    strSpecie &= ", " & "Terreno Nudo"
                                Else
                                    strSpecie = "Terreno Nudo"
                                End If
                            End If

                        Next

                        Dr.Item("Veg_Cod") = Veg_Cod

                        '----------------------------------
                        'appezzamenti
                        DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "appezza", False)
                        'DtApp = DtOperazione.DefaultView.ToTable(True, "piva", "sa_cod", "appezza", "Sa_Nome", "App_Nome", "cul_des",
                        '                                         "veg_des", "veg_cod", "DestinazioneTerreniNudi_Des", "LottoImpianto", "ID_Reg")

                        strCentro = If(IsDBNull(DtApp.Rows(0).Item("Sa_Nome")), "", DtApp.Rows(0).Item("Sa_Nome"))

                        For Each r As DataRow In DtApp.Rows
                            If r.Item("App_Nome") <> "" Then
                                AppezzamentoNome = Replace(r.Item("App_Nome"), "'", "")
                                strAppezzamenti &= AppezzamentoNome & ", "
                            End If
                            If r.Item("cul_des") <> "" AndAlso InStr(strCulDes, r.Item("cul_des")) = 0 Then
                                strCulDes &= Replace(r.Item("cul_des"), "'", "") & ", "
                            End If

                            If r.Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, r.Item("cul_des")) = 0 Then
                                Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                                Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                                strSpecieVarieta &= specie & varieta & ", "
                            End If

                            If r.Item("veg_cod") = 0 Then
                                If r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                    If InStr(strSpecieVarieta, r.Item("DestinazioneTerreniNudi_Des")) = 0 Then
                                        Dim destinazioneTN As String = Replace(r.Item("DestinazioneTerreniNudi_Des"), "'", "")
                                        strSpecieVarieta &= destinazioneTN & ", "
                                    End If
                                ElseIf r.Item("Appezza") <> 0 Then
                                    strSpecieVarieta &= "Terreno Nudo" & ", "
                                End If
                            End If

                            Dim objImpianto_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                            If Visualizza_Codici_AppezzaImpianti AndAlso r("appezza") <> 0 Then

                                Dim Riferimento_Alfanumerico_Appezzamento = r("Riferimento_Alfanumerico_Appezzamento")
                                If Riferimento_Alfanumerico_Appezzamento <> "" Then
                                    strCodici_Appezzamenti &= Riferimento_Alfanumerico_Appezzamento & ", "
                                End If

                                Dim Codice_Impianto = r("Codice_Impianto")

                                If Codice_Impianto <> "" Then
                                    strCodici_Impianto &= Codice_Impianto & ", "
                                End If

                            End If

                            If Visualizza_KPIN_BlockName AndAlso r("appezza") <> 0 Then

                                Dim impKPIN = r("Zespri_Codice_kPIN")

                                If impKPIN <> "" Then
                                    strKPIN &= impKPIN & ", "
                                End If

                                Dim impBlockName = r("Zespri_Block_Name")

                                If impBlockName.Trim <> "" Then
                                    strBlockName &= impBlockName.Trim & ", "
                                End If

                            End If

                        Next

                        If strCodici_Appezzamenti <> "" Then
                            strCodici_Appezzamenti = Left(strCodici_Appezzamenti, strCodici_Appezzamenti.Length - 2)
                        End If

                        If strCodici_Impianto <> "" Then
                            strCodici_Impianto = Left(strCodici_Impianto, strCodici_Impianto.Length - 2)
                        End If

                        If strKPIN <> "" Then
                            strKPIN = Left(strKPIN, strKPIN.Length - 2)
                        End If

                        If strBlockName <> "" Then
                            strBlockName = Left(strBlockName, strBlockName.Length - 2)
                        End If

                        If strAppezzamenti <> "" Then
                            strAppezzamenti = Left(strAppezzamenti, strAppezzamenti.Length - 2)
                        End If
                        If strCulDes <> "" Then
                            strCulDes = Left(strCulDes, strCulDes.Length - 2)
                        End If
                        If strSpecieVarieta <> "" Then
                            strSpecieVarieta = Left(strSpecieVarieta, strSpecieVarieta.Length - 2)
                        End If

                        If Visualizza_Codici_AppezzaImpianti Then
                            Dr.Item("Codici_Appezzamenti") = strCodici_Appezzamenti
                            Dr.Item("Codici_Impianto") = strCodici_Impianto
                        End If

                        If Visualizza_KPIN_BlockName Then
                            Dr.Item("KPIN") = strKPIN
                            Dr.Item("BlockName") = strBlockName
                        End If

                        '----------------------------------
                        'prodotti
                        Dr.Item("Elem_Cod") = DrAgenda(0).Item("Elem_Cod")

                        Dim NomeComune As String = ""
                        Dim drCategoriaMagazzino = DtCategorieMagazzino.Select(" Elem_Cod = " & Dr.Item("Elem_Cod"))
                        If drCategoriaMagazzino.Length > 0 Then
                            NomeComune = drCategoriaMagazzino(0)("NomeComune")
                        End If
                        Dr.Item("NomeComune") = NomeComune

                        Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                            Case FERTILIZZANTI  'FERTILIZZANTI
                                DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Pro_Cod") <> 0 Then
                                        Prodotto = r.Item("Fer_Des")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                For Each r As DataRow In DtProdotti1.Rows
                                    If r.Item("Mat_Cod") <> 0 Then
                                        Prodotto = r.Item("Mat_Des")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                If strProdotti <> "" Then
                                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                End If

                            Case FORMULATI    'FORMULATI
                                DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)

                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Fr_Des") <> "" Then
                                        Prodotto = r.Item("Fr_Des")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                If strProdotti <> "" Then
                                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                End If

                            Case TRAPPOLE
                                DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)

                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Trap_Des") <> "" Then
                                        Prodotto = r.Item("Trap_Des")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                If strProdotti <> "" Then
                                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                End If

                            Case SEMENTI

                                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Mat_Des") <> "" Then
                                        Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                If strProdotti <> "" Then
                                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                End If

                            Case SEMILAVORATI_VEGETALI

                                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Mat_Des") <> "" Then
                                        Prodotto = r.Item("Mat_Des")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                Select Case Dr.Item("Lav_Cod")
                                    Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "Semilavorato trattato:" & strProdotti
                                        End If
                                    Case Else
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                        End If
                                End Select

                                    'vanni, 27/06/2017 gestito per operazioni F&F
                            Case TRASFORMATI_VEGETALI
                                If FF_TrackedData_Cod > 0 Then

                                    Try
                                        'Questa serve per dare una colorazione diversa alle righe aggiunte a parità di certificazione
                                        Dr.Item("FF_Righe_Aggiunte") = righeAggiunte

                                        objEti = New AgronicaCoreStampeDAL.FF_Etichette_R

                                        Dim xOrderByFF As String = ""
                                        Dim xFiltroAggiuntivoFF As String = " detProd.Id_Mov_Det = " & DrAgenda(0).Item("Id_Mov_Det")
                                        Dim DtProdottoFF As New DataTable
                                        SyncLock lockObject
                                            'etichette
                                            DtProdottoFF = objEti.LeggiParametriQualitativi(
                                                DrAgenda(0).Item("id_Agenda"),
                                                1,
                                                FF_Etichette_tipo.Interne,
                                                DrAgenda(0).Item("cau_mov"),
                                                xFiltroAggiuntivoFF,
                                                "",
                                                objparametri_Server
                                            )
                                        End SyncLock

                                        For Each drrProdotto As DataRow In DtProdottoFF.Rows

                                            For Each colProdotto As DataColumn In DtProdottoFF.Columns

                                                If Not ({"specie", "varieta", "data", "ora", "qtakg", "note"}.Contains(colProdotto.ColumnName.ToLower)) Then

                                                    If Not colProdotto.ColumnName.ToLower.Contains("_sigla") Then

                                                        Dim parametroQualitativo As String = ""
                                                        If Not drrProdotto(colProdotto.ColumnName) Is DBNull.Value Then
                                                            parametroQualitativo = drrProdotto(colProdotto.ColumnName)
                                                        End If

                                                        If Not String.IsNullOrEmpty(parametroQualitativo) Then
                                                            'If colProdotto.ColumnName = "Referenza" Then
                                                            '    strProdotti &=
                                                            '    "<b>" & colProdotto.ColumnName & "</b>: " & parametroQualitativo & "<br>"
                                                            'Else
                                                            Dr.Item("FF_" & colProdotto.ColumnName) = parametroQualitativo
                                                            'End If
                                                        End If
                                                    End If
                                                End If

                                            Next

                                        Next

                                        If strProdotti <> "" Then
                                            strProdotti &= "<br>"
                                        End If

                                    Catch ex As Exception

                                    End Try
                                End If


                            Case Else

                                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                For Each r As DataRow In DtProdotti.Rows
                                    If r.Item("Mat_Des") <> "" Then
                                        Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                        Dim desProdotto As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        Prodotto = r.Item("Mat_Des") & If(desProdotto = "", "", " (" & desProdotto & ")")
                                        strProdotti &= Prodotto & ", "
                                    End If
                                Next
                                If strProdotti <> "" Then
                                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                End If
                        End Select



                        Dim drMovDetTec() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))

                        Dim listaAvv As New List(Of String)

                        For Each drAvv As DataRow In drMovDetTec
                            If drAvv.Item("Av_des_vol") <> "" Then
                                listaAvv.Add(drAvv.Item("Av_des_vol"))
                            End If
                            If drAvv.Item("Av_Gru_des") <> "" Then
                                listaAvv.Add(drAvv.Item("Av_Gru_des"))
                            End If
                        Next

                        strAvversita = String.Join(", ", listaAvv)

                        '----------------------------------
                        'Centri e Campi
                        Dim listaCentriCampi As New List(Of String)
                        For Each drCentriCampi As DataRow In DtOperazione.Rows

                            Dim centro As String = If(Not IsDBNull(drCentriCampi.Item("Sa_Nome")) AndAlso Not IsNothing(drCentriCampi.Item("Sa_Nome")), drCentriCampi.Item("Sa_Nome"), "")
                            Dim campo As String = If(Not IsDBNull(drCentriCampi.Item("Campo_Des")) AndAlso Not IsNothing(drCentriCampi.Item("Campo_Des")), drCentriCampi.Item("Campo_Des"), "")

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            Dim testoCentriCampi As String = String.Join(" - ", {centro, campo}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                            If Not listaCentriCampi.Contains(testoCentriCampi) Then
                                listaCentriCampi.Add(testoCentriCampi)
                            End If
                        Next

                        strCentroCampo = String.Join(", ", listaCentriCampi)

                        '----------------------------------
                        'Lotti Produzione
                        Dim listaLottiProduzione As New List(Of String)
                        For Each drLottiProduzione As DataRow In DtOperazione.Rows

                            Dim LottoProduzione As String = If(Not IsDBNull(drLottiProduzione.Item("LottoProduzione")), drLottiProduzione.Item("LottoProduzione"), "")

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            If LottoProduzione.Trim() <> "" AndAlso Not listaLottiProduzione.Contains(LottoProduzione) Then
                                listaLottiProduzione.Add(LottoProduzione)
                            End If
                        Next

                        strLottiProduzione = String.Join(", ", listaLottiProduzione)

                        '----------------------------------
                        'Lotti Impianto
                        Dim listaLottiImpianto As New List(Of String)
                        For Each drLottiImpianto As DataRow In DtApp.Rows

                            Dim LottoImpianto As String = If(Not IsDBNull(drLottiImpianto.Item("LottoImpianto")), drLottiImpianto.Item("LottoImpianto"), "")

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            If LottoImpianto.Trim() <> "" Then
                                listaLottiImpianto.Add(LottoImpianto)
                            End If
                        Next

                        strLottiImpianto = String.Join(", ", listaLottiImpianto)

                        '----------------------------------
                        'Note a checkbox
                        Dim listaNote As New List(Of String)
                        For Each drNote As DataRow In DtOperazione.Rows
                            Dim Nota As String = If(Not IsDBNull(drNote.Item("Nota_Des")), drNote.Item("Nota_Des"), "")

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                listaNote.Add(Nota)
                            End If
                        Next

                        'Nota libera
                        For Each drNote As DataRow In DtOperazione.Rows
                            Dim Nota As String = If(Not IsDBNull(drNote.Item("Mov_desc")), drNote.Item("Mov_desc"), "")

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                listaNote.Add(Nota)
                            End If
                        Next

                        strNote = String.Join(", ", listaNote)

                        getStrCostiMacchineOperatoriViaIDAgenda(strCosti_Operatori, strCosti_Macchine, DtCosti, current_Agenda, False)

                        '----------------------------------
                        'Superficie Trattata
                        Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                        Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(DtOperazione, "APPEZZA", "ID_REG", False)

                        For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                            Dim drAppezza() As DataRow = DtOperazione.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                            If drAppezza.Length > 0 Then
                                Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                            End If
                        Next


                        '----------------------------------
                        'Dettaglio Tecnico
                        Dim listaDetTec As New List(Of String)

                        For Each drDetTec As DataRow In DtOperazione.Rows

                            Dim prod As String = ""
                            Dim princAtt As String = ""
                            Dim avv As String = ""
                            Dim fasifeno As String = ""

                            'PRODOTTI
                            Select Case CInt(drDetTec.Item("Elem_Cod"))
                                Case FERTILIZZANTI

                                    If drDetTec.Item("Pro_Cod") <> 0 Then
                                        prod = drDetTec.Item("Fer_Des") & ", "
                                    End If

                                    If drDetTec.Item("Mat_Cod") <> 0 Then
                                        prod = drDetTec.Item("Mat_Des")
                                    Else
                                        If prod.Length > 0 Then
                                            prod = Left(prod, prod.Length - 2)
                                        End If
                                    End If

                                Case FORMULATI

                                    If drDetTec.Item("Fr_Des") <> "" Then
                                        prod = drDetTec.Item("Fr_Des")
                                    End If

                                Case TRAPPOLE

                                    If drDetTec.Item("Trap_Des") <> "" Then
                                        prod = drDetTec.Item("Trap_Des")
                                    End If

                                Case SEMENTI

                                    If drDetTec.Item("Mat_Des") <> "" Then
                                        Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                    End If

                                Case SEMILAVORATI_VEGETALI

                                    If drDetTec.Item("Mat_Des") <> "" Then

                                        Select Case drDetTec.Item("Lav_Cod")
                                            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                            Case Else
                                                prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                        End Select
                                    End If

                                Case Else
                                    Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                    'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                    Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                    prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                            End Select

                            'AVVERSITA'
                            Dim drMovDetTec2() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                            Dim listaAvv2 As New List(Of String)

                            For Each drAvv2 As DataRow In drMovDetTec2
                                If drAvv2.Item("Av_des_vol") <> "" Then
                                    listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                                End If
                                If drAvv2.Item("Av_Gru_des") <> "" Then
                                    listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                                End If
                            Next

                            avv = String.Join(", ", listaAvv2)

                            'PRINCIPI ATTIVI / SOSTANZE ATTIVE
                            Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

                            If drDetTec.Item("PrincipiAttivi") <> "" Then
                                codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
                            Else
                                If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 _
                                    AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                                    codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                                End If
                            End If

                            Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
                            Dim listaPrincAttNomi As New List(Of String)
                            For Each pa As String In listaPrincAtt
                                listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
                            Next
                            princAtt = String.Join(", ", listaPrincAttNomi)


                            'FASI FENOLOGICHE
                            Dim drMovDetTec2Fasi() As DataRow = dtMovDetTecFasi.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                            Dim listaFasi As New List(Of String)

                            For Each drFasi As DataRow In drMovDetTec2Fasi
                                Dim Fase_Des As String = ""
                                If drFasi.Item("ff_classe") <> 0 Then

                                    Select Case drFasi.Item("ff_classe")
                                        Case < 1000 'caso vecchio av_cod = ff_cod
                                            Fase_Des = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                                        Where aa.FF_Cod = drFasi.Item("ff_classe")
                                                        Select aa.Descrizione
                                                        ).FirstOrDefault

                                        Case Else 'caso nuovo av_cod = cod_css
                                            Fase_Des = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                        Where aa.Cod_SS = drFasi.Item("ff_classe")
                                                        Select aa.Descrizione & " - BBCH " & aa.Stadio
                                                        ).FirstOrDefault

                                    End Select
                                    If Fase_Des <> "" Then
                                        listaFasi.Add(Fase_Des)
                                    End If

                                End If

                            Next

                            fasifeno = String.Join(", ", listaFasi)

                            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                            'Dim testoDetTec As String = prod & If(princAtt <> "", " - " & princAtt, "") & If(avv <> "", " - " & avv, "")
                            Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv, fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                            If Not listaDetTec.Contains(testoDetTec) Then
                                listaDetTec.Add(testoDetTec)
                            End If

                        Next

                        strDettaglioTecnico = String.Join(", ", listaDetTec)


                    End If




                    'modifica per magazzino
                    Dim lc As Integer = DrAgenda(0).Item("Lav_Cod")
                    If {LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO}.Contains(lc) Then

                        If bool_isCuraEnabled Then

                            strProdotti = DrAgenda(0)("Des_Lib")

                        Else

                            '----------------------------------
                            'prodotti
                            Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                                Case FERTILIZZANTI  'FERTILIZZANTI

                                    DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                    DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Pro_Cod") <> 0 Then
                                            Prodotto = r.Item("Fer_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    For Each r As DataRow In DtProdotti1.Rows
                                        If r.Item("Mat_Cod") <> 0 Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case FORMULATI    'FORMULATI

                                    DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Fr_Des") <> "" Then
                                            Prodotto = r.Item("Fr_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case TRAPPOLE

                                    DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Trap_Des") <> "" Then
                                            Prodotto = r.Item("Trap_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case SEMENTI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                    End If

                                Case SEMILAVORATI_VEGETALI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                    End If

                                Case Else

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    End If

                            End Select

                        End If


                        If bool_isCuraEnabled Then

                            strDettaglioTecnico = DrAgenda(0)("Des_Lib")

                        Else
                            '----------------------------------
                            'Dettaglio Tecnico
                            Dim listaDetTec As New List(Of String)
                            For Each drDetTec As DataRow In DtOperazione.Rows
                                Dim prod As String = ""

                                'PRODOTTI
                                Select Case CInt(drDetTec.Item("Elem_Cod"))
                                    Case FERTILIZZANTI

                                        If drDetTec.Item("Pro_Cod") <> 0 Then
                                            prod = drDetTec.Item("Fer_Des") & ", "
                                        End If

                                        If drDetTec.Item("Mat_Cod") <> 0 Then
                                            prod = drDetTec.Item("Mat_Des")
                                        Else
                                            If prod.Length > 0 Then
                                                prod = Left(prod, prod.Length - 2)
                                            End If
                                        End If

                                    Case FORMULATI

                                        If drDetTec.Item("Fr_Des") <> "" Then
                                            prod = drDetTec.Item("Fr_Des")
                                        End If

                                    Case TRAPPOLE

                                        If drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")
                                        End If

                                    Case SEMENTI

                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        If drDetTec.Item("Mat_Des") <> "" Then

                                            Select Case drDetTec.Item("Lav_Cod")
                                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                    prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                Case Else
                                                    prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                            End Select
                                        End If

                                    Case Else
                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        End If

                                End Select


                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                Dim testoDetTec As String = String.Join(" - ", {prod}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaDetTec.Contains(testoDetTec) Then
                                    listaDetTec.Add(testoDetTec)
                                End If

                            Next

                            strDettaglioTecnico = String.Join(", ", listaDetTec)
                        End If




                    End If


                    'modifica per contabilita
                    Select Case lc
                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_MVV_RICEVUTO, LAVCOD_MVV_EMESSO
                            'occorre pescare i prodotti dai dettagli, non vengono su dalla query perche nei doc contabili sacod agenda è 0 probabilmente
                            'per ora lascio stare, leggo la descrizione mov_desc
                            Try

                                Dim strRifDdtFatture As String = ""
                                Dim strMov_Desc As String = ""

                                For Each r As DataRow In DrAgenda

                                    If Not IsDBNull(r.Item("Mov_Desc")) AndAlso r.Item("Mov_Desc") <> "" Then
                                        If Not strDettagli.Contains(r.Item("Mov_Desc")) Then
                                            strDettagli &= r.Item("Mov_Desc") & " <br> "
                                        End If
                                    End If

                                    If Not String.IsNullOrEmpty(r.Item("Mov_Desc")) Then
                                        strMov_Desc = r.Item("Mov_Desc")
                                    End If


                                    If Not String.IsNullOrEmpty(r.Item("RifDdtFatture")) Then
                                        strRifDdtFatture = "Rif: " & r.Item("RifDdtFatture")
                                    End If

                                Next

                                strDettaglioTecnico = String.Join(" ", {strMov_Desc.Trim(), strRifDdtFatture.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                            Catch ex As Exception

                            End Try
                    End Select

                    'Modifica per operazione di cura
                    If lc = LAVCOD_CURA Then
                        Dim lottoRaccolto As String = (From riga As DataRow In DtOperazione.Rows Where riga.Item("cau_mov") = CAU_SCARICO AndAlso riga.Item("tipo_destinazione") = TIPO_DESTINAZIONE_MAGAZZINO Select riga.Item("lottoProduzione")).First()
                        strDettaglioTecnico = "Lotto Raccolto: " & lottoRaccolto
                    End If


                    'Controllo i permessi di modifica
                    If Not FF_TrackedData_Cod > 0 Then
                        'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
                        If ht_Permessi.ContainsKey(Lav_Cod) Then
                            Dr.Item("PermessoModifica") = ht_Permessi(Lav_Cod)
                        Else
                            Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(
                                                            Lav_Cod, enum_Security_Operazione.Modifica,
                                                            objparametri_Server, objparametri_Utenti, HttpContext.Current.Session)

                            ht_Permessi.Add(Lav_Cod, permesso)
                        End If
                    End If


                    Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

                    Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, Resources.AgronicaAgenda_2010.Si, Resources.AgronicaAgenda_2010.No)

                    Testo = "<a " &
                                "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
                                Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
                                Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
                                "' " &
                                ">" &
                                Icona_INFO &
                                "</a>"

                    Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                    Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                    Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                    Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                    Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                    Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                    Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                    Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                    If strCentro <> "" Then
                        strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & strCentro & "<br>"
                    End If

                    If FF_TrackedData_Cod <= 0 And strSpecie <> "" Then
                        strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.Specie & "</b> " & strSpecie & "<br>"
                    End If

                    'per le operazioni colturali visualizzo gli appezzamenti coinvolti
                    If strAppezzamenti <> "" Then
                        If strProdotti <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti & "<br>" & strProdotti
                        Else
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti
                        End If
                    Else
                        strDettagli &= strProdotti
                    End If

                    'avversita
                    strDettagli &= If(strAvversita <> "", "<br> <b> Avversità: </b> " & strAvversita & "<br>", "")

                    'If Ricetta <> "" Then
                    '    strDettagli &= If(strDettagli <> "", vbCrLf & Ricetta, Ricetta)

                    '    strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & Ricetta
                    'End If

                    Dr.Item("Info") = Testo
                    Dr.Item("Dettagli") = strDettagli

                    Dr.Item("cul_des") = strCulDes
                    Dr.Item("Specie_Varieta") = strSpecieVarieta
                    Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                    Dr.Item("Centro_Campo") = strCentroCampo
                    Dr.Item("LottiProduzione") = strLottiProduzione
                    Dr.Item("LottiImpianto") = strLottiImpianto
                    Dr.Item("Note") = strNote
                    Dr.Item("Costi_Operatori") = strCosti_Operatori
                    Dr.Item("Costi_Macchine") = strCosti_Macchine
                    Dr.Item("Sup_Trattata") = Sup_TrattataTot

                    'PEr Rilievo Piogge
                    If DrAgenda(0).Item("Lav_Cod") = LAVCOD_RILIEVO_PIOGGE Then

                        strDettagli = "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & sa_nome
                        '&= " [ Pioggia: " & Pioggia & " mm; TMin: " & TMin & " °C; TMax: " & TMax & " °C; Umidita': " & Umidita & "% ] "
                        Dr.Item("Dettagli") = strDettagli

                    End If

                    If FF_TrackedData_Cod > 0 Then
                        If String.IsNullOrEmpty(Dr.Item("FF_Referenza").ToString) AndAlso Not String.IsNullOrEmpty(strProdotti) AndAlso DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                            Dr.Item("FF_Referenza") = strProdotti
                        End If
                    End If
                    'Dr.Item("gru_des") = DrAgenda(0).Item("tipo")

                    Dr.Item("Centro_Aziendale") = DrAgenda(0).Item("sa_nome")
                    Dr.Item("Specie") = DrAgenda(0).Item("veg_des")
                    Dr.Item("Appezzamenti_Coinvolti") = strAppezzamenti
                    Dr.Item("Prodotti_Utilizzati") = strProdotti
                    Dr.Item("Avversita") = strAvversita

                    Dr.Item("chiave_composita") = CDate(DrAgenda(0).Item("Data_Movimento")) & "_" &
                                                      DrAgenda(0).Item("Id_Agenda") & "_" &
                                                      DrAgenda(0).Item("Lav_Cod") & "_" &
                                                      DrAgenda(0).Item("Piva") & "_" &
                                                      DrAgenda(0).Item("Sa_Cod") & "_" &
                                                      DrAgenda(0).Item("Blocco_Flag") & "_" &
                                                      DrAgenda(0).Item("Veg_Cod")

                    Dt.Rows.Add(Dr)

                End If

            Next

        End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing
        Return Dt
    End Function


    ''' <summary>
    ''' restituisce elenco di descrizione per macchine, operatori con id_agenda
    ''' </summary>
    ''' <param name="strCosti_Operatori"></param>
    ''' <param name="strCosti_Macchine"></param>
    ''' <param name="ListaIDAgenda"></param>
    ''' <returns>es.: 12345|Mario Rossi|Trattore 1</returns>
    Public Shared Function getStrCostiMacchineOperatoriViaIDAgenda(piva As String, ListaIDAgenda As List(Of String), objParametri_Server As AgronicaCoreParametri) As List(Of KeyValuePair(Of Integer, String))

        Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R

        Dim DtCosti As DataTable =
            objCostiAccessori.CostiAccessori_from_IdAgenda5(piva, String.Join(",", ListaIDAgenda), "", "", objParametri_Server)

        Dim rval As New List(Of KeyValuePair(Of Integer, String))
        For Each idA In ListaIDAgenda
            Dim strCosti_Operatori As String
            Dim strCosti_Macchine As String
            getStrCostiMacchineOperatoriViaIDAgenda(strCosti_Operatori, strCosti_Macchine, DtCosti, idA, True)
            rval.Add(New KeyValuePair(Of Integer, String)(idA, strCosti_Operatori & "|" & strCosti_Macchine))
        Next

        Return rval

    End Function

    Private Shared Sub getStrCostiMacchineOperatoriViaIDAgenda(ByRef strCosti_Operatori As String, ByRef strCosti_Macchine As String, DtCosti As DataTable, current_Agenda As String, ByVal AggiungiQTA As Boolean)
        '----------------------------------
        'Costi 
        Dim listaOperatori As New List(Of String)
        Dim listaMacchine As New List(Of String)
        'Dim listaPatentini As New List(Of String)
        'Dim listaTitolari As New List(Of String)
        'Dim listaScadenze As New List(Of String)

        If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then

            Dim DrCosti() As DataRow = DtCosti.Select("Id_Agenda=" & current_Agenda)

            If Not IsNothing(DrCosti) Then
                For Each dr_costo As DataRow In DrCosti

                    'è un record manodopera
                    If dr_costo.Item("Cod_RisUm") <> 0 Then

                        'Recupero il nome del contatto
                        Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                        If AggiungiQTA Then
                            If dr_costo("qta") <> 0 Then
                                nomeContatto &= " - " & dr_costo("qta") & " "
                                Select Case dr_costo("udm_cod")
                                    Case "-1"
                                        nomeContatto &= "Indefinito"
                                    Case "1"
                                        nomeContatto &= "Ettari"
                                    Case "2"
                                        nomeContatto &= "Ore"
                                End Select
                            End If

                        End If

                        If Not listaOperatori.Contains(nomeContatto) Then
                            listaOperatori.Add(nomeContatto)
                        End If

                        'If dr_costo.Item("Cau_Mov") = CAU_IMPUTAZIONE_TERZISTI Then
                        '    nomeContatto &= " (Terzista)" & vbCrLf
                        'End If

                        ''Controllo se è un responsabile o un operatore
                        'If Dr.Item("Cau_Mov") <> CAU_IMPUTAZIONE_TECNICO_RESPONSABILE Then

                        '    strOperatori &= nomeContatto & vbCrLf

                        '    'PATENTINO
                        '    If Not IsDBNull(Dr.Item("patentino")) AndAlso Dr.Item("patentino") <> "" Then

                        '        If Not listaPatentini.Contains(Dr.Item("patentino")) Then
                        '            listaPatentini.Add(Dr.Item("patentino"))
                        '        End If

                        '        If Not listaTitolari.Contains(nomeContatto) Then
                        '            listaTitolari.Add(nomeContatto)
                        '        End If

                        '        If IsDate(Dr.Item("data_scadenza_patentino")) AndAlso
                        '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                        '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then

                        '            If Not listaScadenze.Contains(Dr.Item("data_scadenza_patentino")) Then
                        '                listaScadenze.Add(Dr.Item("data_scadenza_patentino"))
                        '            End If

                        '        End If
                        '    End If
                        'Else

                        '    strResponsabili &= nomeContatto & vbCrLf

                        'End If

                    Else
                        'è un record macchinario

                        Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                        detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                        detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")
                        'detMacchina &= If(dr_costo.Item("Ultima_Manutenzione") <> "01/01/1900", " - Ultima Manutenzione " & dr_costo.Item("Ultima_Manutenzione"), "")


                        If AggiungiQTA Then

                            'al momento "AggiungiQta" vale anche come "aggiungi codice macchina"
                            detMacchina &= " (Codice: " & dr_costo("macchina_codice") & ") "
                            If dr_costo("qta") <> 0 Then
                                detMacchina &= " - " & dr_costo("qta") & " "
                                Select Case dr_costo("udm_cod")
                                    Case "-1"
                                        detMacchina &= "Indefinito"
                                    Case "1"
                                        detMacchina &= "Ettari"
                                    Case "2"
                                        detMacchina &= "Ore"
                                End Select
                            End If
                        End If


                        If Not listaMacchine.Contains(detMacchina) Then
                            listaMacchine.Add(detMacchina)
                        End If

                    End If

                Next
            End If
        End If

        strCosti_Operatori = String.Join(", ", listaOperatori)
        strCosti_Macchine = String.Join(", ", listaMacchine)
    End Sub

    Public Shared Function creaDTAgenda(ByRef dt As DataTable,
                                        Visualizza_Codici_AppezzaImpianti As Boolean,
                                        Visualizza_KPIN_BlockName As Boolean,
                                        FF_TrackedData_Cod As Integer,
                                        DTParamQual As DataTable)

        '----- Definisco la struttura del DataTable

        dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Data", GetType(String)))
        dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Info", GetType(String)))
        dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ricetta_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        dt.Columns.Add(New DataColumn("Note", GetType(String)))
        dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))

        If Visualizza_Codici_AppezzaImpianti Then
            dt.Columns.Add(New DataColumn("Codici_Appezzamenti", GetType(String)))
            dt.Columns.Add(New DataColumn("Codici_Impianto", GetType(String)))
        End If

        If Visualizza_KPIN_BlockName Then
            dt.Columns.Add(New DataColumn("KPIN", GetType(String)))
            dt.Columns.Add(New DataColumn("BlockName", GetType(String)))
        End If

        dt.Columns.Add(New DataColumn("Centro_Aziendale", GetType(String)))
        dt.Columns.Add(New DataColumn("Specie", GetType(String)))
        dt.Columns.Add(New DataColumn("Appezzamenti_Coinvolti", GetType(String)))
        dt.Columns.Add(New DataColumn("Prodotti_Utilizzati", GetType(String)))
        dt.Columns.Add(New DataColumn("Avversita", GetType(String)))
        dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))

        If FF_TrackedData_Cod > 0 Then
            dt.Columns.Add(New DataColumn("FF_Track_Lotto_Padre", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Lotto", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod_Padre", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Extra_Totale", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Contenitori", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Imballi", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Mat_Cod", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Referenza", GetType(String)))
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice", GetType(String)))
                    dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key"), GetType(String)))
                End If
            Next
            dt.Columns.Add(New DataColumn("FF_Righe_Aggiunte", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_codice_generazione", GetType(Integer)))
            dt.Columns.Add(New DataColumn("FF_Linea_Cod", GetType(Integer)))
        End If

    End Function



    ''' <summary>
    ''' Crea e Carica il GridView 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Data_Selezionata"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="Lav_Cod"></param>
    ''' <param name="Flag_TerrenoNudo"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <remarks></remarks>
    Public Shared Function Carica_LavorazioniOLD(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Veg_Cod As Integer,
            ByVal Cul_Cod As Integer,
            ByVal Tipo As String,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal Flag_TerrenoNudo As Boolean,
            ByVal xFiltroAggiuntivo_colturali As String,
            ByVal xFiltroAggiuntivo_postRaccolta As String,
            ByVal xFiltroAggiuntivo_contabili As String,
            ByVal xFiltroAggiuntivo_contabili_Macchine As String,
            ByVal xFiltroAggiuntivo_contabili_Audit As String,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri,
            ByVal FF_TrackedData_Cod As Integer,
            Optional ByVal FromOutToIn As Boolean = True,
            Optional ByVal cCertificazione As Integer = True,
            Optional ByVal righeAggiunte As String = "",
            Optional ByVal Visualizza_Codici_AppezzaImpianti As Boolean = False,
            Optional ByVal Visualizza_KPIN_BlockName As Boolean = False
        ) As DataTable


        Dim Dt As New DataTable
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim i As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strCodici_Appezzamenti As String
        Dim strCodici_Impianto As String
        Dim strKPIN As String
        Dim strBlockName As String

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim ht_Permessi As New Hashtable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim objEti As AgronicaCoreStampeDAL.FF_Etichette_R

        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objparametri_Server)

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        Dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))

        If Visualizza_Codici_AppezzaImpianti Then
            Dt.Columns.Add(New DataColumn("Codici_Appezzamenti", GetType(String)))
            Dt.Columns.Add(New DataColumn("Codici_Impianto", GetType(String)))
        End If

        If Visualizza_KPIN_BlockName Then
            Dt.Columns.Add(New DataColumn("KPIN", GetType(String)))
            Dt.Columns.Add(New DataColumn("BlockName", GetType(String)))
        End If

        If FF_TrackedData_Cod > 0 Then
            Dt.Columns.Add(New DataColumn("FF_Track_Lotto_Padre", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Lotto", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod_Padre", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Extra_Totale", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Contenitori", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Track_Qta_Imballi", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("FF_Mat_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_Referenza", GetType(String)))
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    Dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice", GetType(String)))
                    Dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key"), GetType(String)))
                End If
            Next
            Dt.Columns.Add(New DataColumn("FF_Righe_Aggiunte", GetType(String)))
            Dt.Columns.Add(New DataColumn("FF_codice_generazione", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("FF_Linea_Cod", GetType(Integer)))
        End If


        Dim filtro As String = "|"
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
                'No un filtro
                filtro = HttpContext.Current.Session("Filtro")
            End If
        End If

        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
                Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
            End If
        End If


        Dim Filtro_Utente_Lavorazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
                Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
            End If
        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If

        'Identifico se è abilitata l'operazione di cura
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO, objparametri_Utenti)
        Dim bool_isCuraEnabled As Boolean = (IsNumeric(Tipo_Raccolta_Val) AndAlso Tipo_Raccolta_Val = enum_RACCOLTA_TIPO.Raccolta_e_Cura)


        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Veg_Cod,
                Cul_Cod,
                Tipo,
                Gru_Cod,
                Lav_Cod,
                Flag_TerrenoNudo,
                True,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xFiltroAggiuntivo_colturali,
                xFiltroAggiuntivo_postRaccolta,
                xFiltroAggiuntivo_contabili,
                xFiltroAggiuntivo_contabili_Macchine,
                xFiltroAggiuntivo_contabili_Audit,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server"),
                FF_TrackedData_Cod, FromOutToIn
                )


        Catch ex As Exception

            Return Nothing

        End Try




        If DtAgenda.Rows.Count > 0 Then

            '----------------------------------
            'Leggo tutti i principi attivi
            Dim HtProdPA As New Hashtable()
            Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

            Dim DtCosti As New DataTable

            If FF_TrackedData_Cod > 0 Then
                strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "Id_Mov_Det")
            Else
                strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

                'Leggo i costi accessori
                Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
                '16/09/2019: il filtro date lo fa più sotto
                DtCosti = objCostiAccessori.CostiAccessori_from_IdAgenda3(Piva, String.Join(",", strId_Agenda), "", "", objparametri_Server)
            End If

            '03/01/2018 Grilli: Leggo le Avversità fuori dalla megaLettura
            Dim objMovTec As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
            Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_agenda(Piva, "", "", objparametri_Server)

            '(12/11/2018 fede) aggiunta indicazione fase fenologica (splittate le operazioni)
            'leggo le fasi via web service
            Dim dtMovDetTecFasi As DataTable = objMovTec.Leggi_x_agenda_fasifenologiche(Piva, "", "", objparametri_Server)
            Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
            Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

            If Not dtMovDetTecFasi Is Nothing AndAlso dtMovDetTecFasi.Rows.Count > 0 Then

                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

                Dim Filtro_cod_ss As String = ""
                Dim Filtro_ff_cod As String = ""
                Dim Hash_cod_ss As New Hashtable
                Dim Hash_ff_cod As New Hashtable
                Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objparametri_Utenti, 2)
                If imp = "1" Then
                    objParametriIngresso.Personalizzate = True
                End If
                objParametriIngresso.Lingua_Cod = objparametri_Server.Lingua_Cod

                For f = 0 To dtMovDetTecFasi.Rows.Count - 1
                    Select Case dtMovDetTecFasi.Rows(f).Item("ff_classe")
                        Case < 1000
                            If Not Hash_ff_cod.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                                Hash_ff_cod.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                                Filtro_ff_cod &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                            End If
                        Case Else
                            If Not Hash_cod_ss.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                                Hash_cod_ss.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                                Filtro_cod_ss &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                            End If
                    End Select
                Next

                If Filtro_ff_cod <> "" Then
                    objParametriIngresso.strFiltro = " fs.ff_cod in (" & Left(Filtro_ff_cod, Filtro_ff_cod.Length - 1) & ")"
                    objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
                End If
                If Filtro_cod_ss <> "" Then
                    objParametriIngresso.strFiltro = " ss.cod_ss in (" & Left(Filtro_cod_ss, Filtro_cod_ss.Length - 1) & ")"
                    objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
                End If

            End If

            If Not strId_Agenda Is Nothing Then

                For i = 0 To strId_Agenda.Length - 1

                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strAppezzamenti = ""
                    strCulDes = ""
                    strProdotti = ""
                    strAvversita = ""
                    strNote = ""
                    strCosti_Operatori = ""
                    strCosti_Macchine = ""
                    Sup_TrattataTot = 0

                    strCodici_Appezzamenti = ""
                    strCodici_Impianto = ""
                    strKPIN = ""
                    strBlockName = ""

                    If FF_TrackedData_Cod > 0 Then
                        DrAgenda = DtAgenda.Select("Id_Mov_Det=" & strId_Agenda(i))
                    Else
                        DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))
                    End If

                    DtOperazione = DtAgenda.Clone

                    For Each r As DataRow In DrAgenda
                        DtOperazione.ImportRow(r)
                    Next


                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")

                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))

                        '(05/12/2018) per le fasi visualizzo la data del rilievo (validita_inizio nella destinazione)
                        'le fasi nuove creano un id_agenda per centro, fase, data
                        'le vecchie ne avevano 1 per centro con fasi e date diverse assieme
                        '(per queste ultime visualizzo una data in caso ci siano date diverse nello stesso rilievo)
                        Select Case Dr.Item("Lav_Cod")

                            Case LAVCOD_FASI_FENOLOGICHE

                                Dr.Item("Data") = CDate(DrAgenda(0).Item("validita_inizio_destinazione")).ToShortDateString
                                Dr.Item("Data2") = CDate(DrAgenda(0).Item("validita_inizio_destinazione"))

                        End Select

                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Info") = ""
                        Dr.Item("Dettagli") = ""
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Ricetta_Cod") = 0
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
                        Dr.Item("tipo") = DrAgenda(0).Item("tipo")

                        'Se è un altre lavorazioni aggiungo il dettaglio
                        If DrAgenda(0).Item("attivitaDesc") <> "" Then
                            'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
                            Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        End If

                        If DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                            Dr.Item("FF_Track_Cal_Cod_Padre") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod_Padre"))
                            Dr.Item("FF_Track_Cal_Cod") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod"))
                            Dr.Item("FF_Track_Lotto_Padre") = DrAgenda(0).Item("FF_Track_Lotto_Padre")
                            Dr.Item("FF_Track_Lotto") = DrAgenda(0).Item("FF_Track_Lotto")
                            Dr.Item("FF_Track_Qta_Extra_Totale") = DrAgenda(0).Item("FF_Track_Qta_Extra_Totale")
                            Dr.Item("FF_Track_Qta_Contenitori") = DrAgenda(0).Item("FF_Track_Qta_Contenitori")
                            Dr.Item("FF_Track_Qta_Imballi") = DrAgenda(0).Item("FF_Track_Qta_Imballi")
                            Dr.Item("FF_codice_generazione") = DrAgenda(0).Item("FF_codice_generazione")
                            Dr.Item("FF_Mat_Cod") = DrAgenda(0).Item("Mat_Cod")
                            Dr.Item("FF_Linea_Cod") = DrAgenda(0).Item("FF_Linea_Cod")
                            'Else
                            '    Dr.Item("FF_Track_Cal_Cod_Padre") = 0
                            '    Dr.Item("FF_Track_Cal_Cod") = 0
                            '    Dr.Item("FF_Track_Lotto_Padre") = ""
                            '    Dr.Item("FF_Track_Lotto") = ""
                            '    Dr.Item("FF_Track_Qta_Extra_Totale") = 0.0
                        End If

                        ' @Paolo
                        ' aggiunta colore per tipologia di lavorazione
                        Select Case DrAgenda(0).Item("tipo")
                            Case "C" ' Colturali
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: green'></i>"
                            Case "E" ' Contabili
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: blue'></i>"
                            Case "V" ' Audit / Monitoraggi
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: orange'></i>"
                            Case "F" ' Macchine
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: red'></i>"
                            Case "Z" ' Zootecniche
                                Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: yellow'></i>"
                        End Select

                        strAppezzamenti = ""
                        strCulDes = ""
                        strProdotti = ""
                        strAvversita = ""
                        strSpecie = ""
                        strCentro = ""
                        Prodotto = ""
                        Ricetta = ""
                        strSpecieVarieta = ""
                        strDettaglioTecnico = ""
                        strCentroCampo = ""
                        strLottiProduzione = ""
                        strLottiImpianto = ""

                        If FF_TrackedData_Cod > 0 Then
                            strDettagli = DrAgenda(0)("Des_Lib") & "<br/>" & DrAgenda(0)("Fabbricato_Des") & "<br/>"
                        Else
                            strDettagli = ""
                        End If

                        If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                            Ricetta = "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                        End If
                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                        Dr.Item("Ricetta_Des") = DrAgenda(0).Item("ricetta_numero")
                        'modifica per contabilità magazzino
                        If operazioneLavCodContabMagazzino(DrAgenda(0).Item("lav_cod")) Then

                            'specie
                            DtSpecie = objSqlDis.SelectDistinct("Specie", DtOperazione, "veg_cod", False)

                            For Each r As DataRow In DtSpecie.Rows
                                Veg_Cod = r.Item("veg_cod")

                                If r.Item("veg_cod") <> 0 Then
                                    'Veg_Cod = DtSpecie.Rows(j).Item("veg_cod")
                                    Dim Veg_Des As String = r.Item("veg_des")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & Veg_Des
                                    Else
                                        strSpecie = Veg_Des
                                    End If

                                ElseIf r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                    'Veg_Cod = r.Item("veg_cod")
                                    Dim DestinazioneTerreniNudi_Des As String = r.Item("DestinazioneTerreniNudi_Des")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & DestinazioneTerreniNudi_Des
                                    Else
                                        strSpecie = DestinazioneTerreniNudi_Des
                                    End If

                                ElseIf r.Item("appezza") <> 0 Then
                                    'Veg_Cod = r.Item("veg_cod")
                                    If strSpecie <> "" Then
                                        strSpecie &= ", " & "Terreno Nudo"
                                    Else
                                        strSpecie = "Terreno Nudo"
                                    End If
                                End If

                            Next

                            Dr.Item("Veg_Cod") = Veg_Cod

                            '----------------------------------
                            'appezzamenti
                            DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "appezza", False)

                            strCentro = If(IsDBNull(DtApp.Rows(0).Item("Sa_Nome")), "", DtApp.Rows(0).Item("Sa_Nome"))

                            For Each r As DataRow In DtApp.Rows
                                If r.Item("App_Nome") <> "" Then
                                    AppezzamentoNome = Replace(r.Item("App_Nome"), "'", "")
                                    strAppezzamenti &= AppezzamentoNome & ", "
                                End If
                                If r.Item("cul_des") <> "" AndAlso InStr(strCulDes, r.Item("cul_des")) = 0 Then
                                    strCulDes &= Replace(r.Item("cul_des"), "'", "") & ", "
                                End If

                                If r.Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, r.Item("cul_des")) = 0 Then
                                    Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                                    Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                                    strSpecieVarieta &= specie & varieta & ", "
                                End If

                                If r.Item("veg_cod") = 0 Then
                                    If r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                                        If InStr(strSpecieVarieta, r.Item("DestinazioneTerreniNudi_Des")) = 0 Then
                                            Dim destinazioneTN As String = Replace(r.Item("DestinazioneTerreniNudi_Des"), "'", "")
                                            strSpecieVarieta &= destinazioneTN & ", "
                                        End If
                                    ElseIf r.Item("Appezza") <> 0 Then
                                        strSpecieVarieta &= "Terreno Nudo" & ", "
                                    End If
                                End If

                                Dim objImpianto_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

                                If Visualizza_Codici_AppezzaImpianti AndAlso r("appezza") <> 0 Then

                                    Dim objAppezzamento_codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                                    Dim appCodiceDT = objAppezzamento_codici.Leggi(r("Piva"),
                                                                                   r("sa_cod"),
                                                                                   r("appezza"),
                                                                                   enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento,
                                                                                   "",
                                                                                   AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                   "",
                                                                                   "",
                                                                                   objparametri_Server)
                                    If appCodiceDT.Rows.Count > 0 AndAlso appCodiceDT.Rows(0)("val_cod") <> "" Then
                                        strCodici_Appezzamenti &= appCodiceDT.Rows(0)("val_cod") & ", "
                                    End If


                                    Dim impCodiceDT = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                                                                       r("sa_cod"),
                                                                                       r("appezza"),
                                                                                       r("ID_Reg"),
                                                                                       0,
                                                                                       CInt(enum_CodiciAnagrafe.Codice_Impianto),
                                                                                       True,
                                                                                       "",
                                                                                       "",
                                                                                       objparametri_Server)


                                    If impCodiceDT <> "" Then
                                        strCodici_Impianto &= impCodiceDT & ", "
                                    End If

                                End If

                                If Visualizza_KPIN_BlockName AndAlso r("appezza") <> 0 Then

                                    Dim impKPIN = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                                                                       r("sa_cod"),
                                                                                       r("appezza"),
                                                                                       r("ID_Reg"),
                                                                                       -1,
                                                                                       CInt(enum_CodiciAnagrafe.Zespri_Codice_kPIN),
                                                                                       True,
                                                                                       "",
                                                                                       "",
                                                                                       objparametri_Server)


                                    If impKPIN <> "" Then
                                        strKPIN &= impKPIN & ", "
                                    End If

                                    Dim impBlockName = objImpianto_codici.LeggiValCod_2(r("Piva"),
                                                                                       r("sa_cod"),
                                                                                       r("appezza"),
                                                                                       r("ID_Reg"),
                                                                                       -1,
                                                                                       CInt(enum_CodiciAnagrafe.Zespri_Block_Name),
                                                                                       True,
                                                                                       "",
                                                                                       "",
                                                                                       objparametri_Server)

                                    If impBlockName.Trim <> "" Then
                                        strBlockName &= impBlockName.Trim & ", "
                                    End If

                                End If

                            Next

                            If strCodici_Appezzamenti <> "" Then
                                strCodici_Appezzamenti = Left(strCodici_Appezzamenti, strCodici_Appezzamenti.Length - 2)
                            End If

                            If strCodici_Impianto <> "" Then
                                strCodici_Impianto = Left(strCodici_Impianto, strCodici_Impianto.Length - 2)
                            End If

                            If strKPIN <> "" Then
                                strKPIN = Left(strKPIN, strKPIN.Length - 2)
                            End If

                            If strBlockName <> "" Then
                                strBlockName = Left(strBlockName, strBlockName.Length - 2)
                            End If

                            If strAppezzamenti <> "" Then
                                strAppezzamenti = Left(strAppezzamenti, strAppezzamenti.Length - 2)
                            End If
                            If strCulDes <> "" Then
                                strCulDes = Left(strCulDes, strCulDes.Length - 2)
                            End If
                            If strSpecieVarieta <> "" Then
                                strSpecieVarieta = Left(strSpecieVarieta, strSpecieVarieta.Length - 2)
                            End If

                            If Visualizza_Codici_AppezzaImpianti Then
                                Dr.Item("Codici_Appezzamenti") = strCodici_Appezzamenti
                                Dr.Item("Codici_Impianto") = strCodici_Impianto
                            End If

                            If Visualizza_KPIN_BlockName Then
                                Dr.Item("KPIN") = strKPIN
                                Dr.Item("BlockName") = strBlockName
                            End If

                            '----------------------------------
                            'prodotti
                            Dr.Item("Elem_Cod") = DrAgenda(0).Item("Elem_Cod")
                            Dim leggiCategMag As New Categorie_Magazzino_R
                            Dr.Item("NomeComune") = leggiCategMag.NomeComune_from_ElemCod(Dr.Item("Elem_Cod"), objparametri_Server)

                            Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                                Case FERTILIZZANTI  'FERTILIZZANTI
                                    DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                    DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Pro_Cod") <> 0 Then
                                            Prodotto = r.Item("Fer_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    For Each r As DataRow In DtProdotti1.Rows
                                        If r.Item("Mat_Cod") <> 0 Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case FORMULATI    'FORMULATI
                                    DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Fr_Des") <> "" Then
                                            Prodotto = r.Item("Fr_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case TRAPPOLE
                                    DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Trap_Des") <> "" Then
                                            Prodotto = r.Item("Trap_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                    End If

                                Case SEMENTI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                    End If

                                Case SEMILAVORATI_VEGETALI

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Prodotto = r.Item("Mat_Des")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    Select Case Dr.Item("Lav_Cod")
                                        Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                            If strProdotti <> "" Then
                                                strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                                strProdotti = "Semilavorato trattato:" & strProdotti
                                            End If
                                        Case Else
                                            If strProdotti <> "" Then
                                                strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                                strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                            End If
                                    End Select

                                    'vanni, 27/06/2017 gestito per operazioni F&F
                                Case TRASFORMATI_VEGETALI
                                    If FF_TrackedData_Cod > 0 Then

                                        Try
                                            'Questa serve per dare una colorazione diversa alle righe aggiunte a parità di certificazione
                                            Dr.Item("FF_Righe_Aggiunte") = righeAggiunte

                                            objEti = New AgronicaCoreStampeDAL.FF_Etichette_R

                                            Dim xOrderByFF As String = ""
                                            Dim xFiltroAggiuntivoFF As String = " detProd.Id_Mov_Det = " & DrAgenda(0).Item("Id_Mov_Det")


                                            'etichette
                                            Dim DtProdottoFF = objEti.LeggiParametriQualitativi(
                                                DrAgenda(0).Item("id_Agenda"),
                                                1,
                                                FF_Etichette_tipo.Interne,
                                                DrAgenda(0).Item("cau_mov"),
                                                xFiltroAggiuntivoFF,
                                                "",
                                                objparametri_Server
                                            )

                                            For Each drrProdotto As DataRow In DtProdottoFF.Rows

                                                For Each colProdotto As DataColumn In DtProdottoFF.Columns

                                                    If Not ({"specie", "varieta", "data", "ora", "qtakg", "note"}.Contains(colProdotto.ColumnName.ToLower)) Then

                                                        If Not colProdotto.ColumnName.ToLower.Contains("_sigla") Then

                                                            Dim parametroQualitativo As String = ""
                                                            If Not drrProdotto(colProdotto.ColumnName) Is DBNull.Value Then
                                                                parametroQualitativo = drrProdotto(colProdotto.ColumnName)
                                                            End If

                                                            If Not String.IsNullOrEmpty(parametroQualitativo) Then
                                                                'If colProdotto.ColumnName = "Referenza" Then
                                                                '    strProdotti &=
                                                                '    "<b>" & colProdotto.ColumnName & "</b>: " & parametroQualitativo & "<br>"
                                                                'Else
                                                                Dr.Item("FF_" & colProdotto.ColumnName) = parametroQualitativo
                                                                'End If
                                                            End If
                                                        End If
                                                    End If

                                                Next

                                            Next

                                            If strProdotti <> "" Then
                                                strProdotti &= "<br>"
                                            End If

                                        Catch ex As Exception

                                        End Try
                                    End If


                                Case Else

                                    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                    For Each r As DataRow In DtProdotti.Rows
                                        If r.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desProdotto As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desProdotto = "", "", " (" & desProdotto & ")")
                                            strProdotti &= Prodotto & ", "
                                        End If
                                    Next
                                    If strProdotti <> "" Then
                                        strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                    End If
                            End Select



                            Dim drMovDetTec() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))

                            Dim listaAvv As New List(Of String)

                            For Each drAvv As DataRow In drMovDetTec
                                If drAvv.Item("Av_des_vol") <> "" Then
                                    listaAvv.Add(drAvv.Item("Av_des_vol"))
                                End If
                                If drAvv.Item("Av_Gru_des") <> "" Then
                                    listaAvv.Add(drAvv.Item("Av_Gru_des"))
                                End If
                            Next

                            strAvversita = String.Join(", ", listaAvv)

                            '----------------------------------
                            'Centri e Campi
                            Dim listaCentriCampi As New List(Of String)
                            For Each drCentriCampi As DataRow In DtOperazione.Rows

                                Dim centro As String = If(Not IsDBNull(drCentriCampi.Item("Sa_Nome")) AndAlso Not IsNothing(drCentriCampi.Item("Sa_Nome")), drCentriCampi.Item("Sa_Nome"), "")
                                Dim campo As String = If(Not IsDBNull(drCentriCampi.Item("Campo_Des")) AndAlso Not IsNothing(drCentriCampi.Item("Campo_Des")), drCentriCampi.Item("Campo_Des"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                Dim testoCentriCampi As String = String.Join(" - ", {centro, campo}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaCentriCampi.Contains(testoCentriCampi) Then
                                    listaCentriCampi.Add(testoCentriCampi)
                                End If
                            Next

                            strCentroCampo = String.Join(", ", listaCentriCampi)

                            '----------------------------------
                            'Lotti Produzione
                            Dim listaLottiProduzione As New List(Of String)
                            For Each drLottiProduzione As DataRow In DtOperazione.Rows

                                Dim LottoProduzione As String = If(Not IsDBNull(drLottiProduzione.Item("LottoProduzione")), drLottiProduzione.Item("LottoProduzione"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If LottoProduzione.Trim() <> "" AndAlso Not listaLottiProduzione.Contains(LottoProduzione) Then
                                    listaLottiProduzione.Add(LottoProduzione)
                                End If
                            Next

                            strLottiProduzione = String.Join(", ", listaLottiProduzione)

                            '----------------------------------
                            'Lotti Impianto
                            Dim listaLottiImpianto As New List(Of String)
                            For Each drLottiImpianto As DataRow In DtApp.Rows

                                Dim LottoImpianto As String = If(Not IsDBNull(drLottiImpianto.Item("LottoImpianto")), drLottiImpianto.Item("LottoImpianto"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If LottoImpianto.Trim() <> "" Then
                                    listaLottiImpianto.Add(LottoImpianto)
                                End If
                            Next

                            strLottiImpianto = String.Join(", ", listaLottiImpianto)

                            '----------------------------------
                            'Note a checkbox
                            Dim listaNote As New List(Of String)
                            For Each drNote As DataRow In DtOperazione.Rows
                                Dim Nota As String = If(Not IsDBNull(drNote.Item("Nota_Des")), drNote.Item("Nota_Des"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                    listaNote.Add(Nota)
                                End If
                            Next

                            'Nota libera
                            For Each drNote As DataRow In DtOperazione.Rows
                                Dim Nota As String = If(Not IsDBNull(drNote.Item("Mov_desc")), drNote.Item("Mov_desc"), "")

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                                    listaNote.Add(Nota)
                                End If
                            Next

                            strNote = String.Join(", ", listaNote)

                            '----------------------------------
                            'Costi 
                            Dim listaOperatori As New List(Of String)
                            Dim listaMacchine As New List(Of String)
                            'Dim listaPatentini As New List(Of String)
                            'Dim listaTitolari As New List(Of String)
                            'Dim listaScadenze As New List(Of String)

                            If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then

                                Dim DrCosti() As DataRow = DtCosti.Select("Id_Agenda=" & strId_Agenda(i))

                                If Not IsNothing(DrCosti) Then
                                    For Each dr_costo As DataRow In DrCosti

                                        'è un record manodopera
                                        If dr_costo.Item("Cod_RisUm") <> 0 Then

                                            'Recupero il nome del contatto
                                            Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                                            If Not listaOperatori.Contains(nomeContatto) Then
                                                listaOperatori.Add(nomeContatto)
                                            End If

                                            'If dr_costo.Item("Cau_Mov") = CAU_IMPUTAZIONE_TERZISTI Then
                                            '    nomeContatto &= " (Terzista)" & vbCrLf
                                            'End If

                                            ''Controllo se è un responsabile o un operatore
                                            'If Dr.Item("Cau_Mov") <> CAU_IMPUTAZIONE_TECNICO_RESPONSABILE Then

                                            '    strOperatori &= nomeContatto & vbCrLf

                                            '    'PATENTINO
                                            '    If Not IsDBNull(Dr.Item("patentino")) AndAlso Dr.Item("patentino") <> "" Then

                                            '        If Not listaPatentini.Contains(Dr.Item("patentino")) Then
                                            '            listaPatentini.Add(Dr.Item("patentino"))
                                            '        End If

                                            '        If Not listaTitolari.Contains(nomeContatto) Then
                                            '            listaTitolari.Add(nomeContatto)
                                            '        End If

                                            '        If IsDate(Dr.Item("data_scadenza_patentino")) AndAlso
                                            '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                                            '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then

                                            '            If Not listaScadenze.Contains(Dr.Item("data_scadenza_patentino")) Then
                                            '                listaScadenze.Add(Dr.Item("data_scadenza_patentino"))
                                            '            End If

                                            '        End If
                                            '    End If
                                            'Else

                                            '    strResponsabili &= nomeContatto & vbCrLf

                                            'End If

                                        Else
                                            'è un record macchinario

                                            Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                                            detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                                            detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")
                                            'detMacchina &= If(dr_costo.Item("Ultima_Manutenzione") <> "01/01/1900", " - Ultima Manutenzione " & dr_costo.Item("Ultima_Manutenzione"), "")

                                            If Not listaMacchine.Contains(detMacchina) Then
                                                listaMacchine.Add(detMacchina)
                                            End If

                                        End If

                                    Next
                                End If
                            End If

                            strCosti_Operatori = String.Join(", ", listaOperatori)
                            strCosti_Macchine = String.Join(", ", listaMacchine)

                            '----------------------------------
                            'Superficie Trattata
                            Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                            Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(DtOperazione, "APPEZZA", "ID_REG", False)

                            For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                                Dim drAppezza() As DataRow = DtOperazione.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                                If drAppezza.Length > 0 Then
                                    Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                                End If
                            Next


                            '----------------------------------
                            'Dettaglio Tecnico
                            Dim listaDetTec As New List(Of String)

                            For Each drDetTec As DataRow In DtOperazione.Rows

                                Dim prod As String = ""
                                Dim princAtt As String = ""
                                Dim avv As String = ""
                                Dim fasifeno As String = ""

                                'PRODOTTI
                                Select Case CInt(drDetTec.Item("Elem_Cod"))
                                    Case FERTILIZZANTI

                                        If drDetTec.Item("Pro_Cod") <> 0 Then
                                            prod = drDetTec.Item("Fer_Des") & ", "
                                        End If

                                        If drDetTec.Item("Mat_Cod") <> 0 Then
                                            prod = drDetTec.Item("Mat_Des")
                                        Else
                                            If prod.Length > 0 Then
                                                prod = Left(prod, prod.Length - 2)
                                            End If
                                        End If

                                    Case FORMULATI

                                        If drDetTec.Item("Fr_Des") <> "" Then
                                            prod = drDetTec.Item("Fr_Des")
                                        End If

                                    Case TRAPPOLE

                                        If drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")
                                        End If

                                    Case SEMENTI

                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        If drDetTec.Item("Mat_Des") <> "" Then

                                            Select Case drDetTec.Item("Lav_Cod")
                                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                    prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                Case Else
                                                    prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                            End Select
                                        End If

                                    Case Else
                                        Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                        'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                        prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                End Select

                                'AVVERSITA'
                                Dim drMovDetTec2() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                                Dim listaAvv2 As New List(Of String)

                                For Each drAvv2 As DataRow In drMovDetTec2
                                    If drAvv2.Item("Av_des_vol") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                                    End If
                                    If drAvv2.Item("Av_Gru_des") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                                    End If
                                Next

                                avv = String.Join(", ", listaAvv2)

                                'PRINCIPI ATTIVI / SOSTANZE ATTIVE
                                Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

                                If drDetTec.Item("PrincipiAttivi") <> "" Then
                                    codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
                                Else
                                    If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 _
                                        AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                                        codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                                    End If
                                End If

                                Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
                                Dim listaPrincAttNomi As New List(Of String)
                                For Each pa As String In listaPrincAtt
                                    listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
                                Next
                                princAtt = String.Join(", ", listaPrincAttNomi)


                                'FASI FENOLOGICHE
                                Dim drMovDetTec2Fasi() As DataRow = dtMovDetTecFasi.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
                                Dim listaFasi As New List(Of String)

                                For Each drFasi As DataRow In drMovDetTec2Fasi
                                    Dim Fase_Des As String = ""
                                    If drFasi.Item("ff_classe") <> 0 Then

                                        Select Case drFasi.Item("ff_classe")
                                            Case < 1000 'caso vecchio av_cod = ff_cod
                                                Fase_Des = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                                            Where aa.FF_Cod = drFasi.Item("ff_classe")
                                                            Select aa.Descrizione
                                                            ).FirstOrDefault

                                            Case Else 'caso nuovo av_cod = cod_css
                                                Fase_Des = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                            Where aa.Cod_SS = drFasi.Item("ff_classe")
                                                            Select aa.Descrizione & " - BBCH " & aa.Stadio
                                                            ).FirstOrDefault

                                        End Select
                                        If Fase_Des <> "" Then
                                            listaFasi.Add(Fase_Des)
                                        End If

                                    End If

                                Next

                                fasifeno = String.Join(", ", listaFasi)

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                'Dim testoDetTec As String = prod & If(princAtt <> "", " - " & princAtt, "") & If(avv <> "", " - " & avv, "")
                                Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv, fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaDetTec.Contains(testoDetTec) Then
                                    listaDetTec.Add(testoDetTec)
                                End If

                            Next

                            strDettaglioTecnico = String.Join(", ", listaDetTec)


                        End If




                        'modifica per magazzino
                        Dim lc As Integer = DrAgenda(0).Item("Lav_Cod")
                        If {LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO}.Contains(lc) Then

                            If bool_isCuraEnabled Then

                                strProdotti = DrAgenda(0)("Des_Lib")

                            Else

                                '----------------------------------
                                'prodotti
                                Select Case CInt(DrAgenda(0).Item("Elem_Cod"))

                                    Case FERTILIZZANTI  'FERTILIZZANTI

                                        DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                                        DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Pro_Cod") <> 0 Then
                                                Prodotto = r.Item("Fer_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        For Each r As DataRow In DtProdotti1.Rows
                                            If r.Item("Mat_Cod") <> 0 Then
                                                Prodotto = r.Item("Mat_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case FORMULATI    'FORMULATI

                                        DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Fr_Des") <> "" Then
                                                Prodotto = r.Item("Fr_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case TRAPPOLE

                                        DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Trap_Des") <> "" Then
                                                Prodotto = r.Item("Trap_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = "<b>" & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & "</b> " & strProdotti
                                        End If

                                    Case SEMENTI

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & strProdotti
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Mat_Des") <> "" Then
                                                Prodotto = r.Item("Mat_Des")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                            strProdotti = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & strProdotti
                                        End If

                                    Case Else

                                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                                        For Each r As DataRow In DtProdotti.Rows
                                            If r.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                                strProdotti &= Prodotto & ", "
                                            End If
                                        Next
                                        If strProdotti <> "" Then
                                            strProdotti = Left(strProdotti, strProdotti.Length - 2)
                                        End If

                                End Select

                            End If


                            If bool_isCuraEnabled Then

                                strDettaglioTecnico = DrAgenda(0)("Des_Lib")

                            Else
                                '----------------------------------
                                'Dettaglio Tecnico
                                Dim listaDetTec As New List(Of String)
                                For Each drDetTec As DataRow In DtOperazione.Rows
                                    Dim prod As String = ""

                                    'PRODOTTI
                                    Select Case CInt(drDetTec.Item("Elem_Cod"))
                                        Case FERTILIZZANTI

                                            If drDetTec.Item("Pro_Cod") <> 0 Then
                                                prod = drDetTec.Item("Fer_Des") & ", "
                                            End If

                                            If drDetTec.Item("Mat_Cod") <> 0 Then
                                                prod = drDetTec.Item("Mat_Des")
                                            Else
                                                If prod.Length > 0 Then
                                                    prod = Left(prod, prod.Length - 2)
                                                End If
                                            End If

                                        Case FORMULATI

                                            If drDetTec.Item("Fr_Des") <> "" Then
                                                prod = drDetTec.Item("Fr_Des")
                                            End If

                                        Case TRAPPOLE

                                            If drDetTec.Item("Trap_Des") <> "" Then
                                                prod = drDetTec.Item("Trap_Des")
                                            End If

                                        Case SEMENTI

                                            If drDetTec.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            End If

                                        Case SEMILAVORATI_VEGETALI

                                            If drDetTec.Item("Mat_Des") <> "" Then

                                                Select Case drDetTec.Item("Lav_Cod")
                                                    Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                        prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                    Case Else
                                                        prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                                End Select
                                            End If

                                        Case Else
                                            If drDetTec.Item("Mat_Des") <> "" Then
                                                Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                                'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                                Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                                prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                            End If

                                    End Select


                                    'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                    Dim testoDetTec As String = String.Join(" - ", {prod}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                    If Not listaDetTec.Contains(testoDetTec) Then
                                        listaDetTec.Add(testoDetTec)
                                    End If

                                Next

                                strDettaglioTecnico = String.Join(", ", listaDetTec)
                            End If




                        End If


                        'modifica per contabilita
                        Select Case lc
                            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_MVV_RICEVUTO, LAVCOD_MVV_EMESSO
                                'occorre pescare i prodotti dai dettagli, non vengono su dalla query perche nei doc contabili sacod agenda è 0 probabilmente
                                'per ora lascio stare, leggo la descrizione mov_desc
                                Try

                                    Dim strRifDdtFatture As String = ""
                                    Dim strMov_Desc As String = ""

                                    For Each r As DataRow In DrAgenda

                                        If Not IsDBNull(r.Item("Mov_Desc")) AndAlso r.Item("Mov_Desc") <> "" Then
                                            If Not strDettagli.Contains(r.Item("Mov_Desc")) Then
                                                strDettagli &= r.Item("Mov_Desc") & " <br> "
                                            End If
                                        End If

                                        If Not String.IsNullOrEmpty(r.Item("Mov_Desc")) Then
                                            strMov_Desc = r.Item("Mov_Desc")
                                        End If


                                        If Not String.IsNullOrEmpty(r.Item("RifDdtFatture")) Then
                                            strRifDdtFatture = "Rif: " & r.Item("RifDdtFatture")
                                        End If

                                    Next

                                    strDettaglioTecnico = String.Join(" ", {strMov_Desc.Trim(), strRifDdtFatture.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                Catch ex As Exception

                                End Try
                        End Select

                        'Modifica per operazione di cura
                        If lc = LAVCOD_CURA Then
                            Dim lottoRaccolto As String = (From riga As DataRow In DtOperazione.Rows Where riga.Item("cau_mov") = CAU_SCARICO AndAlso riga.Item("tipo_destinazione") = TIPO_DESTINAZIONE_MAGAZZINO Select riga.Item("lottoProduzione")).First()
                            strDettaglioTecnico = "Lotto Raccolto: " & lottoRaccolto
                        End If


                        'Controllo i permessi di modifica
                        If Not FF_TrackedData_Cod > 0 Then
                            'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
                            If ht_Permessi.ContainsKey(Lav_Cod) Then
                                Dr.Item("PermessoModifica") = ht_Permessi(Lav_Cod)
                            Else
                                Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(
                                                                Lav_Cod, enum_Security_Operazione.Modifica,
                                                                objparametri_Server, objparametri_Utenti, HttpContext.Current.Session)

                                ht_Permessi.Add(Lav_Cod, permesso)
                            End If
                        End If


                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

                        Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, Resources.AgronicaAgenda_2010.Si, Resources.AgronicaAgenda_2010.No)

                        Testo = "<a " &
                                    "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
                                    Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
                                    Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
                                    "' " &
                                    ">" &
                                    Icona_INFO &
                                    "</a>"

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        If strCentro <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & strCentro & "<br>"
                        End If

                        If FF_TrackedData_Cod <= 0 And strSpecie <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.Specie & "</b> " & strSpecie & "<br>"
                        End If

                        'per le operazioni colturali visualizzo gli appezzamenti coinvolti
                        If strAppezzamenti <> "" Then
                            If strProdotti <> "" Then
                                strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti & "<br>" & strProdotti
                            Else
                                strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti & "</b> " & strAppezzamenti
                            End If
                        Else
                            strDettagli &= strProdotti
                        End If

                        'avversita
                        strDettagli &= If(strAvversita <> "", "<br> <b> Avversità: </b> " & strAvversita & "<br>", "")

                        'If Ricetta <> "" Then
                        '    strDettagli &= If(strDettagli <> "", vbCrLf & Ricetta, Ricetta)

                        '    strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & Ricetta
                        'End If

                        Dr.Item("Info") = Testo
                        Dr.Item("Dettagli") = strDettagli

                        Dr.Item("cul_des") = strCulDes
                        Dr.Item("Specie_Varieta") = strSpecieVarieta
                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Centro_Campo") = strCentroCampo
                        Dr.Item("LottiProduzione") = strLottiProduzione
                        Dr.Item("LottiImpianto") = strLottiImpianto
                        Dr.Item("Note") = strNote
                        Dr.Item("Costi_Operatori") = strCosti_Operatori
                        Dr.Item("Costi_Macchine") = strCosti_Macchine
                        Dr.Item("Sup_Trattata") = Sup_TrattataTot

                        'PEr Rilievo Piogge
                        If DrAgenda(0).Item("Lav_Cod") = LAVCOD_RILIEVO_PIOGGE Then

                            strDettagli = "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(Piva, Sa_Cod, objparametri_Server)
                            '&= " [ Pioggia: " & Pioggia & " mm; TMin: " & TMin & " °C; TMax: " & TMax & " °C; Umidita': " & Umidita & "% ] "
                            Dr.Item("Dettagli") = strDettagli

                        End If

                        If FF_TrackedData_Cod > 0 Then
                            If String.IsNullOrEmpty(Dr.Item("FF_Referenza").ToString) AndAlso Not String.IsNullOrEmpty(strProdotti) AndAlso DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                                Dr.Item("FF_Referenza") = strProdotti
                            End If
                        End If
                        'Dr.Item("gru_des") = DrAgenda(0).Item("tipo")

                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing

        Dt.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(Dt)

        If FromOutToIn Then
            Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"
        Else
            Dv.Sort = " Data2 ASC, Ora ASC, Id_Agenda ASC"
        End If

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function

    Private Shared Function operazioneLavCodContabMagazzino(ByVal lavCod As Integer) As Boolean

        Dim rval As Boolean
        rval = (lavCod < 1000 Or {
            LAVCOD_ACCETTAZIONE_DIVERSI,
            LAVCOD_BOLLA_RICEVUTA,
            LAVCOD_BOLLA_EMESSA,
            LAVCOD_DISTINTA_CARICO,
            LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
            LAVCOD_AUTO_DDT_EMESSO,
            LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
            LAVCOD_TRASFORMAZIONI
        }.Contains(lavCod))

        Return rval

    End Function

    Private Shared Function estraiPrincipiAttivi(ByRef DtAgenda As DataTable, ByRef HtProdPA As Hashtable, objParametri_Server As AgronicaCoreParametri) As Hashtable

        Dim res As New Hashtable()

        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        'seleziono le righe nella tabella di appoggio relative ai principi attivi
        Dim dtPA As DataTable = DtAgenda.Clone()
        For Each dr As DataRow In DtAgenda.Rows
            If dr.Item("elem_cod") = FORMULATI Then
                dtPA.ImportRow(dr)
            End If
        Next

        Dim PrincipiSalvati As Boolean = False

        Dim strPACod() As String = objSqlDis.SelectDistinct(dtPA, "PrincipiAttivi")

        If Not IsNothing(strPACod) AndAlso strPACod.Length > 0 Then

            'se i principi sono salvati tutti (cod1§titolo1|cod2§titolo2)
            'leggo in locale le descrizioni dei principi attivi
            If strPACod(0) <> "" Then
                PrincipiSalvati = True
                Dim Principi() As String
                Dim strElencoPACOD As String = ""
                For Each strPa_Cod As String In strPACod
                    Principi = Split(strPa_Cod, "|")
                    If Not IsNothing(Principi) Then
                        For Each p As String In Principi
                            strElencoPACOD &= Split(p, "§")(0) & ","
                        Next
                    End If
                Next
                If strElencoPACOD <> "" Then
                    Dim objPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
                    Dim DtPrincipiDes As DataTable = objPA.Leggi_Da_StrPa_Cod(Left(strElencoPACOD, strElencoPACOD.Length - 1), AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "pa_cod", objParametri_Server)

                    For Each dr As DataRow In DtPrincipiDes.Rows
                        If Not res.ContainsKey(dr.Item("PA_Cod")) Then
                            res.Add(dr.Item("PA_Cod").ToString, dr.Item("PA_Des"))
                        End If
                    Next
                End If
            End If
        End If

        'se almeno un campo è vuoto
        'leggo i principi da web service (con una sola chiamata per tutti i formulati)
        If PrincipiSalvati = False Then
            'ottengo i formulati distinti
            Dim ElencoFormulati As String = ""
            Dim strFrCod() As String = objSqlDis.SelectDistinct(dtPA, "pro_cod")
            If Not strFrCod Is Nothing Then
                ElencoFormulati = String.Join(",", strFrCod)
            End If
            If ElencoFormulati <> "" Then
                Dim objAgroWs As New AgronicaCoreWebService.AgroWs
                Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
                Dim DtPrincipi As DataTable = objAgroWs.ComposizioneFormulatiRecupera(ElencoFormulati, objParametri_Server, objParametri_Utenti)

                If IsNothing(DtPrincipi) Then
                    Throw New Exception("Eccezione durante il recupero dei dati sui principi attivi da WS")
                End If

                'Estraggo i PrincipiAttivi (cod - des)
                For Each dr As DataRow In DtPrincipi.Rows
                    Dim testo As String = dr.Item("Elenco_PrincipiAttivi")
                    If testo <> "" Then
                        Dim elenco As String() = Split(testo, "|")
                        For Each elem As String In elenco
                            Dim datiElem As String() = Split(elem, "§")
                            If datiElem.Count > 2 AndAlso Not res.ContainsKey(datiElem(0)) Then
                                res.Add(datiElem(0).ToString, datiElem(1))
                            End If
                        Next
                    End If
                Next

                'Estraggo i PrincipiAttivi (Fr_Cod - principiAttivi)
                HtProdPA = New Hashtable()
                For Each dr As DataRow In DtPrincipi.Rows
                    If Not HtProdPA.ContainsKey(dr.Item("Fr_Cod")) Then
                        HtProdPA.Add(dr.Item("Fr_Cod"), dr.Item("Elenco_PrincipiAttivi"))
                    End If
                Next

                objAgroWs = Nothing
            End If
        End If

        Return res

    End Function

    Public Shared Function Carica_Operazioni_Zootecniche(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable

        Dim Dt As New DataTable
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim i, j As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim ht_Permessi As New Hashtable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Tipo_Accettazione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        Dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        'Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotti", GetType(String)))


        Dim filtro As String = "|"

        Dim filtrolavorazioni = filtro.Split("|")(0)


        Try

            Dim objOperazioni As New AgronicaCoreContabDAL.Agenda_R
            Dt = objOperazioni.Carica_Operazioni_ZooTecnichexAgenda(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )


        Catch ex As Exception

            Return Nothing

        End Try




        'If DtAgenda.Rows.Count > 0 Then

        '    '----------------------------------
        '    'Leggo tutti i principi attivi
        '    Dim HtProdPA As New Hashtable()
        '    'Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

        '    Dim DtCosti As New DataTable


        '    strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

        '    If Not strId_Agenda Is Nothing Then
        '        For i = 0 To strId_Agenda.Length - 1
        '            'AZZERO LE STRINGHE AD OGNI GIRO
        '            strAppezzamenti = ""
        '            strCulDes = ""
        '            strProdotti = ""
        '            strAvversita = ""
        '            strNote = ""
        '            strCosti_Operatori = ""
        '            strCosti_Macchine = ""
        '            Sup_TrattataTot = 0


        '            DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))

        '            DtOperazione = DtAgenda.Clone

        '            For j = 0 To DrAgenda.Length - 1
        '                DtOperazione.ImportRow(DrAgenda(j))
        '            Next


        '            If DrAgenda.Length > 0 Then

        '                Dr = Dt.NewRow
        '                '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
        '                Dr.Item("Piva") = DrAgenda(0).Item("Piva")
        '                Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
        '                Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
        '                Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
        '                Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
        '                Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
        '                Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
        '                Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
        '                Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
        '                Dr.Item("Blocco_Flag") = 0
        '                Dr.Item("Info") = ""
        '                Dr.Item("Dettagli") = ""
        '                Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
        '                Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
        '                Dr.Item("tipo") = DrAgenda(0).Item("tipo")

        '                'Se è un altre lavorazioni aggiungo il dettaglio
        '                If DrAgenda(0).Item("attivitaDesc") <> "" Then
        '                    'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
        '                    Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
        '                End If

        '                strCentro = ""
        '                Prodotto = ""
        '                strDettaglioTecnico = ""
        '                strLottiProduzione = ""


        '                strDettagli = ""


        '                Dim lc As Integer = CInt(DrAgenda(0).Item("Lav_Cod"))


        '                Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

        '                Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, Resources.AgronicaAgenda_2010.Si, Resources.AgronicaAgenda_2010.No)

        '                'Testo = "<a " &
        '                '        "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
        '                '        Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
        '                '        Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
        '                '        "' " &
        '                '        ">" &
        '                '        Icona_INFO &
        '                '        "</a>"

        '                Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
        '                Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
        '                Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
        '                Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

        '                Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
        '                Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
        '                'Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

        '                If strCentro <> "" Then
        '                    strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & strCentro & "<br>"
        '                End If

        '                Dr.Item("Info") = Testo
        '                Dr.Item("Dettagli") = strDettagli

        '                Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
        '                Dr.Item("Note") = strNote

        '                strProdotti = estraiListaProdotti(DtOperazione)


        '                Dr.Item("Prodotti") = strProdotti

        '                Dt.Rows.Add(Dr)

        '            End If

        '        Next

        '    End If

        'End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing

        Dt.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(Dt)


        Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function


    Public Shared Function Carica_Operazioni_ZootecnicheOld(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable

        Dim Dt As New DataTable
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        Dim i, j As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim ht_Permessi As New Hashtable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        'Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        Dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        'Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        'Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotti", GetType(String)))


        Dim filtro As String = "|"

        Dim filtrolavorazioni = filtro.Split("|")(0)


        Try

            Dim objOperazioni As New AgronicaCoreContabDAL.Agenda_R
            DtAgenda = objOperazioni.Carica_Operazioni_ZooTecnichexAgenda_Old(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )


        Catch ex As Exception

            Return Nothing

        End Try




        If DtAgenda.Rows.Count > 0 Then

            '----------------------------------
            'Leggo tutti i principi attivi
            Dim HtProdPA As New Hashtable()
            'Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

            Dim DtCosti As New DataTable


            strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "id_agenda")

            If Not strId_Agenda Is Nothing Then
                For i = 0 To strId_Agenda.Length - 1
                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strAppezzamenti = ""
                    strCulDes = ""
                    strProdotti = ""
                    strAvversita = ""
                    strNote = ""
                    strCosti_Operatori = ""
                    strCosti_Macchine = ""
                    Sup_TrattataTot = 0


                    DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))

                    DtOperazione = DtAgenda.Clone

                    For j = 0 To DrAgenda.Length - 1
                        DtOperazione.ImportRow(DrAgenda(j))
                    Next


                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
                        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
                        Dr.Item("Tipo_Accettazione") = DrAgenda(0).Item("Tipo_Accettazione")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Info") = ""
                        Dr.Item("Dettagli") = ""
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
                        Dr.Item("tipo") = DrAgenda(0).Item("tipo")

                        'Se è un altre lavorazioni aggiungo il dettaglio
                        If DrAgenda(0).Item("attivitaDesc") <> "" Then
                            'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
                            Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        End If

                        strCentro = ""
                        Prodotto = ""
                        strDettaglioTecnico = ""
                        strLottiProduzione = ""


                        strDettagli = ""


                        Dim lc As Integer = CInt(DrAgenda(0).Item("Lav_Cod"))


                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

                        Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, Resources.AgronicaAgenda_2010.Si, Resources.AgronicaAgenda_2010.No)

                        'Testo = "<a " &
                        '        "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
                        '        Resources.AgronicaAgenda_2010.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
                        '        Resources.AgronicaAgenda_2010.InterventoBloccato & Bloccato &
                        '        "' " &
                        '        ">" &
                        '        Icona_INFO &
                        '        "</a>"

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        'Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        If strCentro <> "" Then
                            strDettagli &= "<b>" & Resources.AgronicaAgenda_2010.CentroAz & "</b> " & strCentro & "<br>"
                        End If

                        Dr.Item("Info") = Testo
                        Dr.Item("Dettagli") = strDettagli

                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Note") = strNote

                        strProdotti = estraiListaProdotti(DtOperazione)


                        Dr.Item("Prodotti") = strProdotti

                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing

        Dt.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(Dt)


        Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function

    Public Shared Function Carica_Operazioni_Colturali(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Veg_Cod As Integer,
            ByVal Cul_Cod As Integer,
            ByVal Tipo As String,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal Flag_TerrenoNudo As Boolean,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable


        Dim Dt As New DataTable("Movimenti")
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable

        Dim strAppezzamenti As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Sup_TrattataTot As Decimal

        Dim Validita_Inizio As Date = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Dim Validita_Fine As Date = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        Dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Appezzamenti_Coinvolti", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        Dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        Dim filtro As String = "|"

        If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
            'Ho un filtro
            filtro = HttpContext.Current.Session("Filtro")
        End If


        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
            Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
        End If

        Dim Filtro_Utente_Lavorazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
            Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
        End If


        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " (" & filtrolavorazioni & ") AND (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If

        'Identifico se è abilitata l'operazione di cura
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO, objparametri_Utenti)
        Dim bool_isCuraEnabled As Boolean = (IsNumeric(Tipo_Raccolta_Val) AndAlso Tipo_Raccolta_Val = enum_RACCOLTA_TIPO.Raccolta_e_Cura)

        Try
            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Carica_Operazioni_ColturalixMenuAgenda_Fast_Senza_Avversita(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Veg_Cod,
                Cul_Cod,
                Tipo,
                Gru_Cod,
                Lav_Cod,
                Flag_TerrenoNudo,
                True,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )

        Catch ex As Exception
            Return Nothing
        End Try


        If DtAgenda.Rows.Count > 0 Then

            '----------------------------------
            'Leggo tutti i principi attivi
            Dim HtProdPA As New Hashtable()
            Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server)

            Dim strId_Agenda() As String = (From riga As DataRow In DtAgenda.Rows Select CStr(riga.Item("id_agenda"))).Distinct().ToArray()

            'Leggo i costi accessori
            Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
            '16/09/2019: bisognerebbe filtrare per data_movimento 8per evitare doppioni sui contatti con più patentini)
            Dim DtCosti As DataTable = objCostiAccessori.CostiAccessori_from_IdAgenda2(Piva, String.Join(",", strId_Agenda), AGRODATAINIZIO, AGRODATAFINE, "", "", objparametri_Server)

            '03/01/2018 Grilli: Leggo le Avversità fuori dalla megaLettura
            Dim objMovTec As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
            Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_agenda(Piva, "", "", objparametri_Server)

            'Leggo le Fasi Fenologiche fuori dalla megalettura
            Dim dtMovDetTecFasi As DataTable = objMovTec.Leggi_x_agenda_fasifenologiche(Piva, "", "", objparametri_Server)

            'Leggo tutte le fasi fenologiche
            Dim objParametriUscitaFasiNew As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
            Dim objParametriUscitaFasiOld As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
            estraiFasiFenologiche(dtMovDetTecFasi, objParametriUscitaFasiOld, objParametriUscitaFasiNew, objparametri_Server, objparametri_Utenti)


            If Not strId_Agenda Is Nothing Then
                For i As Integer = 0 To strId_Agenda.Length - 1

                    DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))
                    DtOperazione = DrAgenda.CopyToDataTable()

                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
                        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        'Se è un altre lavorazioni aggiungo il dettaglio
                        If DrAgenda(0).Item("attivitaDesc") <> "" Then
                            Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        End If

                        '(05/12/2018) per le fasi visualizzo la data del rilievo (validita_inizio nella destinazione)
                        'le fasi nuove creano un id_agenda per centro, fase, data
                        'le vecchie ne avevano 1 per centro con fasi e date diverse assieme
                        '(per queste ultime visualizzo una data in caso ci siano date diverse nello stesso rilievo)
                        Select Case Dr.Item("Lav_Cod")
                            Case LAVCOD_FASI_FENOLOGICHE
                                Dr.Item("Data") = CDate(DrAgenda(0).Item("validita_inizio_destinazione")).ToShortDateString
                                Dr.Item("Data2") = CDate(DrAgenda(0).Item("validita_inizio_destinazione"))
                        End Select

                        'AZZERO LE STRINGHE AD OGNI GIRO
                        strAppezzamenti = ""
                        strNote = ""
                        strCosti_Operatori = ""
                        strCosti_Macchine = ""
                        Sup_TrattataTot = 0
                        strSpecieVarieta = ""
                        strDettaglioTecnico = ""
                        strCentroCampo = ""
                        strLottiProduzione = ""
                        strLottiImpianto = ""

                        'modifica per contabilità magazzino
                        If operazioneLavCodContabMagazzino(DrAgenda(0).Item("lav_cod")) Then

                            Dr.Item("Veg_Cod") = (From riga As DataRow In DtAgenda.Rows Where IsNumeric(riga.Item("veg_cod")) Distinct Select CInt(riga.Item("veg_cod"))).First()

                            strAppezzamenti = estraiListaNomiAppezzamenti(DtOperazione)

                            strSpecieVarieta = estraiListaSpecieVarieta(DtOperazione)

                            strCentroCampo = estraiListaCentriCampi(DtOperazione)

                            strLottiProduzione = estraiListaLottiProduzione(DtOperazione)

                            strLottiImpianto = estraiListaLottiImpianto(DtOperazione)

                            strNote = estraiListaNote(DtOperazione)

                            estraiListaCosti(DtCosti, strId_Agenda(i), strCosti_Operatori, strCosti_Macchine, Dr.Item("Data"))

                            Sup_TrattataTot = estraiSupTrattata(DtOperazione)

                            If bool_isCuraEnabled Then
                                strDettaglioTecnico = DrAgenda(0)("Des_Lib")
                            Else
                                strDettaglioTecnico = estraiDettaglioTecnicoColturale(DtOperazione,
                                                                                      HtProdPA, HtPrincAtt, dtMovDetTec,
                                                                                      dtMovDetTecFasi, objParametriUscitaFasiOld.ListaFasiFenologiche, objParametriUscitaFasiNew.ListaFasiFenologiche,
                                                                                      DrAgenda(0).Item("Id_Agenda"))
                            End If

                        End If

                        'Modifica per operazione di cura
                        If DrAgenda(0).Item("lav_cod") = LAVCOD_CURA Then
                            Dim lottoRaccolto As String = (From riga As DataRow In DtOperazione.Rows Where riga.Item("cau_mov") = CAU_SCARICO AndAlso riga.Item("tipo_destinazione") = TIPO_DESTINAZIONE_MAGAZZINO Select riga.Item("lottoProduzione")).First()
                            strDettaglioTecnico &= "Lotto Raccolto: " & lottoRaccolto
                        End If

                        ''Ricetta
                        'If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                        '    strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                        'End If
                        'Ricetta
                        If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                            Dr.Item("Ricetta_Des") = DrAgenda(0).Item("ricetta_numero")
                        End If

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("Appezzamenti_Coinvolti") = strAppezzamenti
                        Dr.Item("Specie_Varieta") = strSpecieVarieta
                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Centro_Campo") = strCentroCampo
                        Dr.Item("LottiProduzione") = strLottiProduzione
                        Dr.Item("LottiImpianto") = strLottiImpianto
                        Dr.Item("Note") = strNote
                        Dr.Item("Costi_Operatori") = strCosti_Operatori
                        Dr.Item("Costi_Macchine") = strCosti_Macchine
                        Dr.Item("Sup_Trattata") = Sup_TrattataTot

                        Dr.Item("chiave_composita") = String.Join("_", {Dr.Item("Data2"), Dr.Item("Id_Agenda"), Dr.Item("Lav_Cod"), Dr.Item("Piva"), Dr.Item("Sa_Cod"), Dr.Item("Blocco_Flag"), Dr.Item("Veg_Cod")})
                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If

        Dim dtOrd As DataTable = New DataView(Dt) With {.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"}.ToTable()

        Return dtOrd

    End Function

    Private Shared Sub estraiFasiFenologiche(ByRef dtMovDetTecFasi As DataTable,
                                             ByRef objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output, ByRef objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                             ByRef objparametri_Server As AgronicaCoreParametri, ByRef objparametri_Utenti As AgronicaCoreParametri)

        'leggo le fasi via web service
        If Not dtMovDetTecFasi Is Nothing AndAlso dtMovDetTecFasi.Rows.Count > 0 Then

            Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
            Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

            Dim Lista_cod_ss As New List(Of Integer)
            Dim Lista_ff_cod As New List(Of Integer)
            Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objparametri_Utenti, 2)
            If imp = "1" Then
                objParametriIngresso.Personalizzate = True
            End If

            objParametriIngresso.Lingua_Cod = objparametri_Server.Lingua_Cod

            For Each drFasi As DataRow In dtMovDetTecFasi.Rows
                Select Case drFasi.Item("ff_classe")
                    Case < 1000
                        If Not Lista_ff_cod.Contains(drFasi.Item("ff_classe")) Then
                            Lista_ff_cod.Add(drFasi.Item("ff_classe"))
                        End If
                    Case Else
                        If Not Lista_cod_ss.Contains(drFasi.Item("ff_classe")) Then
                            Lista_cod_ss.Add(drFasi.Item("ff_classe"))
                        End If
                End Select
            Next

            If Lista_ff_cod.Count > 0 Then
                objParametriIngresso.strFiltro = " fs.ff_cod in (" & String.Join(",", Lista_ff_cod) & ")"
                objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
            End If
            If Lista_cod_ss.Count > 0 Then
                objParametriIngresso.strFiltro = " ss.cod_ss in (" & String.Join(",", Lista_cod_ss) & ")"
                objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
            End If

        End If

    End Sub

    Private Shared Function estraiDettaglioTecnicoColturale(ByRef DtOperazione As DataTable,
                                                            ByRef HtProdPA As Hashtable, ByRef HtPrincAtt As Hashtable, ByRef dtMovDetTec As DataTable,
                                                            ByRef dtMovDetTecFasi As DataTable, ByRef ListaFasiFenologicheOld As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica), ByRef ListaFasiFenologicheNew As List(Of AgronicaCoreMetaSchemaBIZ.FaseFenologica),
                                                            ByVal id_agenda As Integer) As String

        Dim strDettaglioTecnico As String = ""

        Dim listaDetTec As New List(Of String)
        For Each drDetTec As DataRow In DtOperazione.Rows

            'PRODOTTI
            Dim prod As String = ""
            Select Case drDetTec.Item("Elem_Cod")
                Case FERTILIZZANTI

                    If drDetTec.Item("Pro_Cod") <> 0 Then
                        prod = drDetTec.Item("Fer_Des")
                    End If

                    If drDetTec.Item("Mat_Cod") <> 0 Then
                        prod = drDetTec.Item("Mat_Des")
                    End If

                Case FORMULATI

                    If drDetTec.Item("Fr_Des") <> "" Then
                        prod = drDetTec.Item("Fr_Des")
                    End If

                Case TRAPPOLE

                    If drDetTec.Item("Trap_Des") <> "" Then
                        prod = drDetTec.Item("Trap_Des")
                    End If

                Case SEMENTI

                    If drDetTec.Item("Mat_Des") <> "" Then
                        Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                        'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                    End If

                Case SEMILAVORATI_VEGETALI

                    If drDetTec.Item("Mat_Des") <> "" Then

                        Select Case drDetTec.Item("Lav_Cod")
                            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                            Case Else
                                prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                        End Select
                    End If

            End Select

            'AVVERSITA'
            Dim drMovDetTec2() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & id_agenda)
            Dim listaAvv2 As New List(Of String)

            For Each drAvv2 As DataRow In drMovDetTec2
                If drAvv2.Item("Av_des_vol") <> "" Then
                    listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                End If
                If drAvv2.Item("Av_Gru_des") <> "" Then
                    listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                End If
            Next

            Dim avv As String = String.Join(", ", listaAvv2)

            'PRINCIPI ATTIVI / SOSTANZE ATTIVE
            Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

            If drDetTec.Item("PrincipiAttivi") <> "" Then
                codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
            Else
                If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                    codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                End If
            End If

            Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
            Dim listaPrincAttNomi As New List(Of String)
            For Each pa As String In listaPrincAtt
                listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
            Next
            Dim princAtt As String = String.Join(", ", listaPrincAttNomi)

            'FASI FENOLOGICHE
            Dim drMovDetTec2Fasi() As DataRow = dtMovDetTecFasi.Select(" ID_Agenda=" & id_agenda)
            Dim listaFasi As New List(Of String)

            For Each drFasi As DataRow In drMovDetTec2Fasi
                Dim Fase_Des As String = ""
                If drFasi.Item("ff_classe") <> 0 Then

                    Select Case drFasi.Item("ff_classe")
                        Case < 1000 'caso vecchio av_cod = ff_cod
                            Fase_Des = (From aa In ListaFasiFenologicheOld
                                        Where aa.FF_Cod = drFasi.Item("ff_classe")
                                        Select aa.Descrizione
                                                            ).FirstOrDefault

                        Case Else 'caso nuovo av_cod = cod_css
                            Fase_Des = (From aa In ListaFasiFenologicheNew
                                        Where aa.Cod_SS = drFasi.Item("ff_classe")
                                        Select aa.Descrizione & " ( BBCH " & aa.Stadio & " )"
                                                            ).FirstOrDefault

                    End Select
                    If Fase_Des <> "" Then
                        listaFasi.Add(Fase_Des)
                    End If

                End If

            Next

            Dim fasifeno As String = String.Join(", ", listaFasi)

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv, fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            If Not listaDetTec.Contains(testoDetTec) Then
                listaDetTec.Add(testoDetTec)
            End If

        Next

        strDettaglioTecnico = String.Join(", ", listaDetTec)

        Return strDettaglioTecnico
    End Function

    Private Shared Function estraiListaNomiAppezzamenti(ByRef DtOperazione As DataTable) As String
        Return String.Join(", ", (From riga As DataRow In DtOperazione.Rows Where Not String.IsNullOrEmpty(riga.Item("App_Nome")) Select Replace(CStr(riga.Item("App_Nome")), "'", "")).ToList())
    End Function

    Private Shared Function estraiListaSpecieVarieta(ByRef dtOperazione As DataTable) As String

        Dim strSpecieVarieta As String = ""

        Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility
        Dim DtApp As DataTable = objSqlDis.SelectDistinct("Appezzamenti", dtOperazione, "appezza", False)

        For Each r As DataRow In DtApp.Rows

            If r.Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, r.Item("cul_des")) = 0 Then
                Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                strSpecieVarieta &= specie & varieta & ", "
            End If

            If r.Item("veg_cod") = 0 Then
                If r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                    If InStr(strSpecieVarieta, r.Item("DestinazioneTerreniNudi_Des")) = 0 Then
                        Dim destinazioneTN As String = Replace(r.Item("DestinazioneTerreniNudi_Des"), "'", "")
                        strSpecieVarieta &= destinazioneTN & ", "
                    End If
                ElseIf r.Item("Appezza") <> 0 Then
                    strSpecieVarieta &= "Terreno Nudo" & ", "
                End If
            End If

        Next

        If strSpecieVarieta <> "" Then
            strSpecieVarieta = Left(strSpecieVarieta, strSpecieVarieta.Length - 2)
        End If

        Return strSpecieVarieta

    End Function

    Private Shared Function estraiSupTrattata(ByRef dtOperazione As DataTable) As Decimal

        Dim Sup_TrattataTot As Decimal = 0

        Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
        Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(dtOperazione, "APPEZZA", "ID_REG", False)

        For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
            Dim drAppezza() As DataRow = dtOperazione.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

            If drAppezza.Length > 0 Then
                Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
            End If
        Next

        Return Sup_TrattataTot

    End Function

    Private Shared Sub estraiListaCosti(ByRef dtCosti As DataTable, Id_Agenda As Integer, ByRef strCosti_Operatori As String, ByRef strCosti_Macchine As String, ByVal Data As Date)

        Dim listaOperatori As New List(Of String)
        Dim listaMacchine As New List(Of String)
        'Dim listaPatentini As New List(Of String)
        'Dim listaTitolari As New List(Of String)
        'Dim listaScadenze As New List(Of String)

        If Not IsNothing(dtCosti) AndAlso dtCosti.Rows.Count > 0 Then
            '16/09/2019
            'Dim DrCosti() As DataRow = dtCosti.Select("Id_Agenda=" & Id_Agenda)
            Dim DrCosti() As DataRow = dtCosti.Select("Id_Agenda=" & Id_Agenda &
                                                       " AND Data_Rilascio_Patentino <= #" & CDate(Data).ToString("MM/dd/yyyy") & "#" &
                                                       " AND Data_Scadenza_Patentino >= #" & CDate(Data).ToString("MM/dd/yyyy") & "#")

            If Not IsNothing(DrCosti) Then
                For Each dr_costo As DataRow In DrCosti

                    'è un record manodopera
                    If dr_costo.Item("Cod_RisUm") <> 0 Then

                        'Recupero il nome del contatto
                        Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                        If Not listaOperatori.Contains(nomeContatto) Then
                            listaOperatori.Add(nomeContatto)
                        End If

                        'If dr_costo.Item("Cau_Mov") = CAU_IMPUTAZIONE_TERZISTI Then
                        '    nomeContatto &= " (Terzista)" & vbCrLf
                        'End If

                        ''Controllo se è un responsabile o un operatore
                        'If Dr.Item("Cau_Mov") <> CAU_IMPUTAZIONE_TECNICO_RESPONSABILE Then

                        '    strOperatori &= nomeContatto & vbCrLf

                        '    'PATENTINO
                        '    If Not IsDBNull(Dr.Item("patentino")) AndAlso Dr.Item("patentino") <> "" Then

                        '        If Not listaPatentini.Contains(Dr.Item("patentino")) Then
                        '            listaPatentini.Add(Dr.Item("patentino"))
                        '        End If

                        '        If Not listaTitolari.Contains(nomeContatto) Then
                        '            listaTitolari.Add(nomeContatto)
                        '        End If

                        '        If IsDate(Dr.Item("data_scadenza_patentino")) AndAlso
                        '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                        '               CDate(Dr.Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then

                        '            If Not listaScadenze.Contains(Dr.Item("data_scadenza_patentino")) Then
                        '                listaScadenze.Add(Dr.Item("data_scadenza_patentino"))
                        '            End If

                        '        End If
                        '    End If
                        'Else

                        '    strResponsabili &= nomeContatto & vbCrLf

                        'End If

                    Else
                        'è un record macchinario

                        Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                        detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                        detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")
                        'detMacchina &= If(dr_costo.Item("Ultima_Manutenzione") <> "01/01/1900", " - Ultima Manutenzione " & dr_costo.Item("Ultima_Manutenzione"), "")

                        If Not listaMacchine.Contains(detMacchina) Then
                            listaMacchine.Add(detMacchina)
                        End If

                    End If

                Next
            End If
        End If

        strCosti_Operatori = String.Join(", ", listaOperatori)
        strCosti_Macchine = String.Join(", ", listaMacchine)

    End Sub

    Private Shared Function estraiListaNote(ByRef dtOperazione As DataTable) As String


        Dim listaNote As New List(Of String)

        'Note a checkbox
        If (dtOperazione.Columns.Contains("Nota_Des")) Then
            For Each drNote As DataRow In dtOperazione.Rows
                Dim Nota As String = If(Not IsDBNull(drNote.Item("Nota_Des")), drNote.Item("Nota_Des"), "")

                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                    listaNote.Add(Nota)
                End If
            Next
        End If


        'Nota libera
        For Each drNote As DataRow In dtOperazione.Rows
            Dim Nota As String = If(Not IsDBNull(drNote.Item("Mov_desc")), drNote.Item("Mov_desc"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                listaNote.Add(Nota)
            End If
        Next

        Dim strNote As String = String.Join(", ", listaNote)

        Return strNote

    End Function

    Private Shared Function estraiListaCentriCampi(ByRef dtOperazione As DataTable) As String

        Dim listaCentriCampi As New List(Of String)
        For Each drCentriCampi As DataRow In dtOperazione.Rows

            Dim centro As String = If(Not IsDBNull(drCentriCampi.Item("Sa_Nome")) AndAlso Not IsNothing(drCentriCampi.Item("Sa_Nome")), drCentriCampi.Item("Sa_Nome"), "")
            Dim campo As String = If(Not IsDBNull(drCentriCampi.Item("Campo_Des")) AndAlso Not IsNothing(drCentriCampi.Item("Campo_Des")), drCentriCampi.Item("Campo_Des"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            Dim testoCentriCampi As String = String.Join(" - ", {centro, campo}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            If Not listaCentriCampi.Contains(testoCentriCampi) Then
                listaCentriCampi.Add(testoCentriCampi)
            End If
        Next

        Dim strCentroCampo As String = String.Join(", ", listaCentriCampi)

        Return strCentroCampo

    End Function

    Private Shared Function estraiListaLottiProduzione(ByRef dtOperazione As DataTable) As String

        Dim listaLottiProduzione As New List(Of String)
        For Each drLottiProduzione As DataRow In dtOperazione.Rows

            Dim LottoProduzione As String = If(Not IsDBNull(drLottiProduzione.Item("LottoProduzione")), drLottiProduzione.Item("LottoProduzione"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If LottoProduzione.Trim() <> "" AndAlso Not listaLottiProduzione.Contains(LottoProduzione) Then
                listaLottiProduzione.Add(LottoProduzione)
            End If
        Next

        Dim strLottiProduzione As String = String.Join(", ", listaLottiProduzione)

        Return strLottiProduzione
    End Function

    Private Shared Function estraiListaLottiImpianto(ByRef dtOperazione As DataTable) As String

        Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility
        Dim DtApp As DataTable = objSqlDis.SelectDistinct("Appezzamenti", dtOperazione, "appezza", False)

        Dim listaLottiImpianto As New List(Of String)
        For Each drLottiImpianto As DataRow In DtApp.Rows

            Dim LottoImpianto As String = If(Not IsDBNull(drLottiImpianto.Item("LottoImpianto")), drLottiImpianto.Item("LottoImpianto"), "")

            'AGGIUNGO ALLA LISTA SE NON E' VUOTO
            If LottoImpianto.Trim() <> "" Then
                listaLottiImpianto.Add(LottoImpianto)
            End If
        Next

        Dim strLottiImpianto As String = String.Join(", ", listaLottiImpianto)

        Return strLottiImpianto
    End Function


    Public Shared Function Carica_Operazioni_MagCont(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable


        Dim Dt As New DataTable("Movimenti")
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable

        Dim strProdotti As String
        Dim strCentroMagazzino As String
        Dim strNote As String
        Dim strRifDdtFatture As String

        Dim Validita_Inizio As Date = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Dim Validita_Fine As Date = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prodotti", GetType(String)))
        Dt.Columns.Add(New DataColumn("Centro_Magazzino", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("RifDdtFatture", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        Dim filtro As String = "|"

        If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
            'Ho un filtro
            filtro = HttpContext.Current.Session("Filtro")
        End If


        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
            Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
        End If

        Dim Filtro_Utente_Lavorazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
            Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
        End If


        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " (" & filtrolavorazioni & ") AND (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If


        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Carica_Operazioni_MagContab(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Gru_Cod,
                Lav_Cod,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )


        Catch ex As Exception
            Return Nothing
        End Try


        If DtAgenda.Rows.Count > 0 Then

            '----------------------------------
            Dim strId_Agenda() As String = (From riga As DataRow In DtAgenda.Rows Select CStr(riga.Item("id_agenda"))).Distinct().ToArray()

            If Not strId_Agenda Is Nothing Then
                For i As Integer = 0 To strId_Agenda.Length - 1

                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strProdotti = ""
                    strNote = ""
                    strProdotti = ""
                    strCentroMagazzino = ""
                    strRifDdtFatture = ""

                    DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))
                    DtOperazione = DrAgenda.CopyToDataTable()

                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
                        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")
                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")


                        strProdotti = estraiListaProdotti(DtOperazione)

                        strCentroMagazzino = estraiListaCentriMagazziniDestinazione(DtOperazione)

                        'strNote = estraiListaNote(DtOperazione)

                        '----------------------------------
                        'riferimenti bolla/fattura
                        If {LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}.Contains(DrAgenda(0).Item("Lav_Cod")) Then
                            strRifDdtFatture = estraiListaRifDdtFatture(DtOperazione)
                        End If

                        'Descrizione unica
                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = If(String.IsNullOrEmpty(strRifDdtFatture), "", " <i>" & strRifDdtFatture & "</i>")
                        Dim riga3 As String = strCentroMagazzino
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("Prodotti") = strProdotti
                        Dr.Item("RifDdtFatture") = strRifDdtFatture
                        Dr.Item("Centro_Magazzino") = strCentroMagazzino
                        Dr.Item("Note") = strNote

                        Dr.Item("chiave_composita") = String.Join("_", {Dr.Item("Data2"), Dr.Item("Id_Agenda"), Dr.Item("Lav_Cod"), Dr.Item("Piva"), Dr.Item("Sa_Cod"), Dr.Item("Blocco_Flag"), Dr.Item("Veg_Cod")})
                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If

        Dim dtOrd As DataTable = New DataView(Dt) With {.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"}.ToTable

        Return dtOrd

    End Function

    Private Shared Function estraiListaRifDdtFatture(dtOperazione As DataTable) As String

        Dim strNum As String = String.Join(" ", (From r As DataRow In dtOperazione.Rows Where r.Item("numDdtFatture").trim() <> "0" Select r.Item("numDdtFatture").trim()).Distinct().ToList())
        Dim strRif As String = String.Join(" ", (From r As DataRow In dtOperazione.Rows Select r.Item("RifDdtFatture").trim()).Distinct().ToList())

        'strRifDdtFatture = String.Join(" - ", {strNum, strRif}.Where(Function(s) Not String.IsNullOrEmpty(s)))
        Dim strRifDdtFatture As String = strNum & " - " & strRif

        Return strRifDdtFatture

    End Function

    Private Shared Function estraiListaCentriMagazziniDestinazione(ByRef DtOperazione As DataTable) As String

        Dim strCentroMagazzino As String = ""

        Dim listaCentriMagazzini As New List(Of String)
        For Each drCentriMagazzini As DataRow In DtOperazione.Rows

            Dim centro As String = If(Not IsDBNull(drCentriMagazzini.Item("Sa_Nome_Dest")) AndAlso Not IsNothing(drCentriMagazzini.Item("Sa_Nome_Dest")), drCentriMagazzini.Item("Sa_Nome_Dest").ToString().Trim(), "")
            Dim magazzino As String = If(Not IsDBNull(drCentriMagazzini.Item("Fabbricato_Des")) AndAlso Not IsNothing(drCentriMagazzini.Item("Fabbricato_Des")), drCentriMagazzini.Item("Fabbricato_Des").ToString().Trim(), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            Dim testoCentriMagazzini As String = String.Join(" - ", {centro, magazzino}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            If Not listaCentriMagazzini.Contains(testoCentriMagazzini) AndAlso testoCentriMagazzini <> "" Then
                listaCentriMagazzini.Add(testoCentriMagazzini)
            End If
        Next

        strCentroMagazzino = String.Join(", ", listaCentriMagazzini)
        Return strCentroMagazzino

    End Function

    Private Shared Function estraiListaProdotti(ByRef DtOperazione As DataTable) As String

        'Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility
        'Dim DtProdotti As DataTable
        'Dim strProdotti As String = ""

        Dim listaProd As New List(Of String)
        For Each dr As DataRow In DtOperazione.Rows
            Select Case dr.Item("elem_cod")
                Case FERTILIZZANTI
                    If dr.Item("Pro_Cod") <> 0 AndAlso Not listaProd.Contains(dr.Item("Fer_Des")) Then
                        listaProd.Add(dr.Item("Fer_Des"))
                    End If

                    If dr.Item("Mat_Cod") <> 0 AndAlso Not listaProd.Contains(dr.Item("Mat_Des")) Then
                        listaProd.Add(dr.Item("Mat_Des"))
                    End If

                Case FORMULATI
                    If dr.Item("Fr_Des") <> "" AndAlso Not listaProd.Contains(dr.Item("Fr_Des")) Then
                        listaProd.Add(dr.Item("Fr_Des"))
                    End If

                Case TRAPPOLE
                    If dr.Item("Trap_Des") <> "" AndAlso Not listaProd.Contains(dr.Item("Trap_Des")) Then
                        listaProd.Add(dr.Item("Trap_Des"))
                    End If

                Case Else
                    If dr.Item("Mat_Des") <> "" Then
                        Dim codart As String = If(dr.Item("Cod_Articolo") = "", "", "Articolo: " & dr.Item("Cod_Articolo"))
                        'Dim lotto As String = If(dr.Item("LottoProduzione") = "", "", "Lotto: " & dr.Item("LottoProduzione"))
                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dim prod As String = dr.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                        If Not listaProd.Contains(prod) Then
                            listaProd.Add(prod)
                        End If

                    End If

            End Select
        Next

        Dim strProdotti As String = String.Join(", ", listaProd)

        'Select Case elem_cod

        '    Case FERTILIZZANTI

        '        DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
        '        For Each r As DataRow In DtProdotti.Rows
        '            If r.Item("Pro_Cod") <> 0 Then
        '                strProdotti &= r.Item("Fer_Des") & ", "
        '            End If
        '        Next

        '        DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "mat_cod", False)
        '        For Each r As DataRow In DtProdotti.Rows
        '            If r.Item("Mat_Cod") <> 0 Then
        '                strProdotti &= r.Item("Mat_Des") & ", "
        '            End If
        '        Next

        '    Case FORMULATI

        '        DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)
        '        For Each r As DataRow In DtProdotti.Rows
        '            If r.Item("Fr_Des") <> "" Then
        '                strProdotti &= r.Item("Fr_Des") & ", "
        '            End If
        '        Next

        '    Case TRAPPOLE

        '        DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)
        '        For Each r As DataRow In DtProdotti.Rows
        '            If r.Item("Trap_Des") <> "" Then
        '                strProdotti &= r.Item("Trap_Des") & ", "
        '            End If
        '        Next

        '        'Case SEMILAVORATI_VEGETALI

        '        '    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
        '        '    For Each r As DataRow In DtProdotti.Rows
        '        '        If r.Item("Mat_Des") <> "" Then
        '        '            strProdotti &= r.Item("Mat_Des") & ", "
        '        '        End If
        '        '    Next

        '        'Case SEMENTI

        '        '    DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
        '        '    For Each r As DataRow In DtProdotti.Rows
        '        '        If r.Item("Mat_Des") <> "" Then
        '        '            strProdotti &= r.Item("Mat_Des")
        '        '            If r.Item("Cod_Articolo") <> "" Then
        '        '                strProdotti &= " (Lotto: " & r.Item("Cod_Articolo") & ")"
        '        '            End If
        '        '            strProdotti &= ", "
        '        '        End If
        '        '    Next

        '    Case Else

        '        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
        '        For Each r As DataRow In DtProdotti.Rows
        '            If r.Item("Mat_Des") <> "" Then
        '                strProdotti &= r.Item("Mat_Des")
        '                If r.Item("Cod_Articolo") <> "" Then
        '                    strProdotti &= " (Lotto: " & r.Item("Cod_Articolo") & ")"
        '                End If
        '                strProdotti &= ", "
        '            End If
        '        Next

        'End Select

        'If strProdotti <> "" Then
        '    strProdotti = Left(strProdotti, strProdotti.Length - 2)
        'End If

        Return strProdotti

    End Function

    Public Shared Function Carica_Operazioni_Audit(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable


        Dim Dt As New DataTable("Movimenti")
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim strDettaglioTecnico As String
        Dim strNote As String

        Dim Validita_Inizio As Date = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Dim Validita_Fine As Date = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        Dim filtro As String = "|"

        If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
            'Ho un filtro
            filtro = HttpContext.Current.Session("Filtro")
        End If


        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
            Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
        End If

        Dim Filtro_Utente_Lavorazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
            Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If


        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Carica_Operazioni_Audit(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Gru_Cod,
                Lav_Cod,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )

        Catch ex As Exception
            Return Nothing
        End Try

        If DtAgenda.Rows.Count > 0 Then

            Dim strId_Agenda() As String = (From riga As DataRow In DtAgenda.Rows Select CStr(riga.Item("id_agenda"))).Distinct().ToArray()

            If Not strId_Agenda Is Nothing Then
                For i As Integer = 0 To strId_Agenda.Length - 1
                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strNote = ""
                    strDettaglioTecnico = ""

                    DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))

                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
                        'Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Ricetta_Cod") = 0
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")

                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")
                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        'Se è un altre lavorazioni aggiungo il dettaglio
                        If DrAgenda(0).Item("attivitaDesc") <> "" Then
                            Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        End If

                        If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                            strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                        End If

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Note") = strNote

                        Dr.Item("chiave_composita") = String.Join("_", {Dr.Item("Data2"), Dr.Item("Id_Agenda"), Dr.Item("Lav_Cod"), Dr.Item("Piva"), Dr.Item("Sa_Cod"), Dr.Item("Blocco_Flag"), Dr.Item("Veg_Cod")})
                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If

        'uso il dataview per Riordinare 
        Dim dtOrd As DataTable = New DataView(Dt) With {.Sort = " Data2 DESC, Id_Agenda DESC"}.ToTable 'Data2 DESC, Ora DESC, Id_Agenda DESC

        Return dtOrd

    End Function

    Public Shared Function Carica_Operazioni_Macchine(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri
        ) As DataTable


        Dim Dt As New DataTable("Movimenti")
        Dim DtAgenda As New DataTable
        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim strDettaglioTecnico As String
        Dim strNote As String

        Dim Validita_Inizio As Date = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Dim Validita_Fine As Date = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        Dt.Columns.Add(New DataColumn("ID", GetType(String)))
        Dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Note", GetType(String)))
        Dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        Dim filtro As String = "|"

        If Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
            'Ho un filtro
            filtro = HttpContext.Current.Session("Filtro")
        End If


        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
            Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
        End If

        Dim Filtro_Utente_Lavorazioni As String = ""
        If Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
            Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")
        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If


        Try

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Carica_Operazioni_Macchine(
                Piva,
                Sa_Cod,
                Validita_Inizio,
                Validita_Fine,
                Gru_Cod,
                Lav_Cod,
                filtrolavorazioni,
                filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni,
                xOrderBy,
                HttpContext.Current.Session("ASG_objParametri_Utenti"),
                HttpContext.Current.Session("ASG_objParametri_Server")
                )

        Catch ex As Exception
            Return Nothing
        End Try

        If DtAgenda.Rows.Count > 0 Then

            Dim strId_Agenda() As String = (From riga As DataRow In DtAgenda.Rows Select CStr(riga.Item("id_agenda"))).Distinct().ToArray()

            If Not strId_Agenda Is Nothing Then
                For i As Integer = 0 To strId_Agenda.Length - 1
                    'AZZERO LE STRINGHE AD OGNI GIRO
                    strNote = ""
                    strDettaglioTecnico = ""

                    DrAgenda = DtAgenda.Select("id_agenda=" & strId_Agenda(i))

                    If DrAgenda.Length > 0 Then

                        Dr = Dt.NewRow
                        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
                        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
                        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
                        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")
                        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
                        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
                        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
                        'Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
                        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
                        Dr.Item("Blocco_Flag") = 0
                        Dr.Item("Veg_Cod") = 0
                        Dr.Item("Ricetta_Cod") = 0
                        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
                        Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                        Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")
                        Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                        Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                        Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                        Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                        Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                        Dim riga2 As String = If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                        Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                        Dr.Item("Note") = strNote

                        Dr.Item("chiave_composita") = String.Join("_", {Dr.Item("Data2"), Dr.Item("Id_Agenda"), Dr.Item("Lav_Cod"), Dr.Item("Piva"), Dr.Item("Sa_Cod"), Dr.Item("Blocco_Flag"), Dr.Item("Veg_Cod")})
                        Dt.Rows.Add(Dr)

                    End If

                Next

            End If

        End If

        'uso il dataview per Riordinare 
        Dim dtOrd As DataTable = New DataView(Dt) With {.Sort = " Data2 DESC, Id_Agenda DESC"}.ToTable 'Data2 DESC, Ora DESC, Id_Agenda DESC

        Return dtOrd

    End Function

End Class
