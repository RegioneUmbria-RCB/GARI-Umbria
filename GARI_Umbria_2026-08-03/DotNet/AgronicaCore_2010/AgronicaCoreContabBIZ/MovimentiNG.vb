Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Newtonsoft.Json.Linq

Public Class MovimentiNG
    Inherits LogProvider

    Public objPServer As AgronicaCoreParametri
    Public objPUtenti As AgronicaCoreParametri

    Sub New(objP_Server As AgronicaCoreParametri, objP_Utenti As AgronicaCoreParametri)
        objPServer = objP_Server
        objPUtenti = objP_Utenti
    End Sub

    Public Function CaricaRicette(nonParsedFilters As String, piva As String) As Object

        Dim filtri As FiltersDto = ParseFilters(nonParsedFilters)
        filtri.piva = piva ' infillo la piva qui per facilitare il riutilizzo di questo metodo in Gias2010.

        Return CreaTabella(filtri)

    End Function


    Public Function CaricaZoo(filtro As String, piva As String) As Object

        Dim r As New RispostaStandard

        Try

            'Inserire il codice QUI..

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)


            Dim dtAgenda As DataTable

            Dim sa_cod As Integer = 0
            Dim validita_inizio As DateTime = CostantiPersonalizzate.AGRODATAINIZIO
            Dim validita_fine As DateTime = CostantiPersonalizzate.AGRODATAFINE

            Dim sData_Selezionata1 As String
            Dim Data_Selezionata1 As DateTime

            Dim sData_Selezionata2 As String
            Dim Data_Selezionata2 As DateTime

            Dim TipoGriglia As String

            Try
                TipoGriglia = jSonDatiTESTATA("TipoGriglia").ToString
            Catch ex As Exception
            End Try

            If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
                sa_cod = jSonDatiTESTATA("sa_cod")
            End If

            sData_Selezionata1 = jSonDatiTESTATA("txt_Data1").ToString
            Data_Selezionata1 = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            sData_Selezionata2 = jSonDatiTESTATA("txt_Data2").ToString
            Data_Selezionata2 = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R

            caricaInSessioneImpostazioniutente()

            dtAgenda = MenuBS_Lavorazioni.Carica_Operazioni_Zootecniche(
                piva,
                sa_cod,
                Data_Selezionata1,
                Data_Selezionata2,
                "",
                objPServer,
                objPUtenti
                )

            r.RispostaOK = True
            If dtAgenda.Rows.Count > 0 OrElse TipoGriglia = "2" Then

                If TipoGriglia = "2" Then
                    EstendiDatatable(dtAgenda)
                End If

                HttpContext.Current.Session("dt") = dtAgenda
                r.RispostaStringa = DT_to_Json_AziendaZoo(dtAgenda, TipoGriglia, objPServer)

            Else
                r.RispostaStringa = "Zero"
            End If



        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Private Shared Sub EstendiDatatable(ByRef dt As DataTable)

        If Not dt.Columns.Contains("chiave_composita") Then
            dt.Columns.Add("Centro_Aziendale", GetType(String))
            dt.Columns.Add("Specie", GetType(String))
            dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
            dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
            dt.Columns.Add("Avversita", GetType(String))

            dt.Columns.Add("chiave_composita", GetType(String))
            Dim delim As String() = New String(0) {"<br>"}

            For i = 0 To dt.Rows.Count - 1

                If Not IsDBNull(dt.Rows(i).Item("Dettagli")) Then

                    Dim dettagli = dt.Rows(i).Item("Dettagli")
                    Dim dett() As String = dettagli.Split(delim, StringSplitOptions.None)

                    For count = 0 To dett.Length - 1

                        If InStr(dett(count), ":") Then

                            Dim dett2 As String() = dett(count).Split(":")

                            If dett(count) <> "" Then

                                Dim final_s As String = dett2(1).Replace("</b> ", "")

                                Select Case dett2(0)
                                    Case "<b>Centro Az."
                                        dt.Rows(i).Item("Centro_Aziendale") = final_s
                                    Case "<b>Specie"
                                        dt.Rows(i).Item("Specie") = final_s
                                    Case "<b>Appezzamenti Coinvolti"
                                        dt.Rows(i).Item("Appezzamenti_Coinvolti") = final_s
                                    Case "<b>Prodotti Utilizzati"
                                        dt.Rows(i).Item("Prodotti_Utilizzati") = final_s
                                    Case " <b> Avversità"
                                        final_s = final_s.Replace("-", "")
                                        dt.Rows(i).Item("Avversita") = final_s
                                End Select

                            End If

                        End If
                    Next
                End If

                dt.Rows(i).Item("chiave_composita") = dt.Rows(i).Item("Data2") & "_" &
                                                                dt.Rows(i).Item("Id_Agenda") & "_" &
                                                                dt.Rows(i).Item("Lav_Cod") & "_" &
                                                                dt.Rows(i).Item("Piva") & "_" &
                                                                dt.Rows(i).Item("Sa_Cod") & "_" &
                                                                dt.Rows(i).Item("Blocco_Flag") & "_" &
                                                                dt.Rows(i).Item("Veg_Cod")


            Next
        End If

    End Sub



    Private Shared Sub caricaInSessioneImpostazioniutente()

        HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") = Leggi_Filtro_Tipo_GruppoOperazioni()

        HttpContext.Current.Session("Filtro_Utente_Lavorazioni") = Leggi_Filtro_Utente_Lavorazioni()

    End Sub

    Public Shared Function DT_to_Json_AziendaZoo(ByVal dt As DataTable, ByVal TipoGriglia As String, ByVal objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        If TipoGriglia = "2" Then
            c = New ColonneNome("id_mov_det", "id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("id_agenda", "Sel.", "string")
            c._Filtrabile = False
            c._ColonnaDiSelezione = True
            c._hidden = True
            l.Add(c)
        End If

        'If TipoGriglia <> "2" Then
        '    'aggiungo i pulsanti per modifica ed eliminazione
        '    c = New ColonneNome("id_agenda", "tool", "string")
        '    c._FormatoParticolare = "<span id='ModificaRiga' class='fa fa-info-circle edit_elem fa-2x' chiave='{0}'></span> "
        '    c._Filtrabile = False
        '    l.Add(c)
        'End If

        If TipoGriglia = "2" Then

            l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
            l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
            l.Add(New ColonneNome("Prodotti_Utilizzati", "Prodotti utilizzati", "string") With {._Display = False})
            l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string"))
            l.Add(New ColonneNome("Centro_Aziendale", "Centro Aziendale", "string") With {._Display = False})
            l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
            l.Add(New ColonneNome("tipo", "tipo", "string") With {._hidden = True}) 'identifica il tipo di operazione (per colorare le righe)
            l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
            l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
            l.Add(New ColonneNome("LottiProduzione", "Lotti di Produzione", "string") With {._Display = False})
            l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Operatori", "Operatori", "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Macchine", "Macchine", "string") With {._Display = False})
            l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
            l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        Else
            l.Add(New ColonneNome("Data", "Data", "string"))
        End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("Lav_des", "Descrizione", "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("Dettagli", "Dettagli", "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            l.Add(c)

            c = New ColonneNome("FF_Referenza", "Prodotto", "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

        End If

        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Operazione_DES", "Operazione_DES", "string") With {._hidden = True})
        l.Add(New ColonneNome("Prodotti", "Prodotti", "string"))

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Qta_Extra_Totale", "Kg", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Contenitori", "Contenitori", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Imballi", "Imballi", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)
        End If

        c = New ColonneNome("Rag_Soc", "Azienda", "string")
        If TipoGriglia = "2" Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        End If
        l.Add(c)

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Righe_Aggiunte", "Righe aggiunte", "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})


        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function



    Private Shared Function Leggi_Filtro_Tipo_GruppoOperazioni() As String
        'FILTRO GRUPPO OPERAZIONI
        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(
                                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA, 1,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If DT.Rows.Count > 0 Then

            If Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim tipi As String = DT.Rows(0).Item("Impostazione_Valore_1")
                Dim tipis() As String = tipi.Split("|")

                For i = 0 To tipis.Count - 1
                    If i > 0 Then
                        Filtro_Tipo_GruppoOperazioni &= " OR "
                    End If

                    Select Case (tipis(i))
                        Case "C", "E", "Z", "P", "V"
                            Filtro_Tipo_GruppoOperazioni &= " GruppoOperazioni.TIPO = '" & tipis(i) & "' "
                        Case "E6"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 ) "
                        Case "E10"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 ) "

                    End Select

                Next

                HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") = Filtro_Tipo_GruppoOperazioni
            End If

        End If

        Return Filtro_Tipo_GruppoOperazioni

    End Function

    Private Shared Function Leggi_Filtro_Utente_Lavorazioni() As String

        'FILTRO LAVORAZIONI
        Dim Filtro_Utente_Lavorazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi(
                                        enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, 1,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If DT.Rows.Count > 0 Then
            Filtro_Utente_Lavorazioni = " agenda.Lav_Cod in ("
            For i = 0 To DT.Rows.Count - 1
                If i <> 0 Then
                    Filtro_Utente_Lavorazioni &= " ,"
                End If
                Filtro_Utente_Lavorazioni &= DT.Rows(i).Item("ID_0")
            Next
            Filtro_Utente_Lavorazioni &= " )  "

        End If

        Return Filtro_Utente_Lavorazioni

    End Function




    Private Function ParseFilters(nonParsedFilters As String) As FiltersDto
        Dim jSonDatiFiltro As JObject = JObject.Parse(nonParsedFilters)

        Dim veg_cod As Integer = If(IsNumeric(jSonDatiFiltro("veg_cod")) AndAlso CInt(jSonDatiFiltro("veg_cod")) > 0, jSonDatiFiltro("veg_cod"), 0)
        Dim sa_cod As Integer = If(IsNumeric(jSonDatiFiltro("sa_cod")), jSonDatiFiltro("sa_cod"), 0)

        Dim sData_Selezionata1 As String = jSonDatiFiltro("txt_Data1").ToString
        Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

        Dim sData_Selezionata2 As String = jSonDatiFiltro("txt_Data2").ToString
        Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

        Dim stato As String = jSonDatiFiltro("stato").ToString
        Dim filtroQuery As String = ""

        If IsNumeric(stato) Then
            filtroQuery = "ro.W_Anagrafica_Stati_Cod = " & stato
        End If

        Dim TipoOperazioniStr As String = ""
        If jSonDatiFiltro("tipoOperazione") IsNot Nothing AndAlso jSonDatiFiltro("tipoOperazione").ToString <> "" Then
            Dim arr = JArray.Parse(jSonDatiFiltro("tipoOperazione").ToString)

            If arr.Count > 0 Then
                For Each elem In arr
                    TipoOperazioniStr &= elem.ToString & ","
                Next
                TipoOperazioniStr = TipoOperazioniStr.Substring(0, TipoOperazioniStr.Length - 1)
            End If

        End If

        Dim strFiltroRicetteOperazioni As String = ""
        Dim impianti As New List(Of String)
        If jSonDatiFiltro("impianti") IsNot Nothing AndAlso jSonDatiFiltro("impianti").ToString <> "" Then
            Dim arr = JArray.Parse(jSonDatiFiltro("impianti").ToString)
            If arr.Count > 0 Then
                Dim DT_ID_Agenda As DataTable

                Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                For Each elem In arr
                    Dim elemArr = elem.ToString.Split("_")
                    Dim dtAgImp = objMov_Destinazioni.Leggi_DistinctRicetta_Operazione_Cod_Impianti(objPServer,
                                                                             elemArr(0),
                                                                             elemArr(1),
                                                                             elemArr(2),
                                                                             elemArr(3),
                                                                             Data_Selezionata1,
                                                                             Data_Selezionata2,
                                                                             "",
                                                                             "")

                    If DT_ID_Agenda Is Nothing Then
                        DT_ID_Agenda = dtAgImp.Copy
                    Else
                        DT_ID_Agenda.Merge(dtAgImp)
                    End If
                Next

                If DT_ID_Agenda.Rows.Count > 0 Then
                    DT_ID_Agenda = DT_ID_Agenda.DefaultView.ToTable(True, "Ricetta_Operazione_Cod")
                    strFiltroRicetteOperazioni = ""
                    For Each rowAgImp In DT_ID_Agenda.Rows
                        strFiltroRicetteOperazioni &= rowAgImp(0) & ","
                    Next
                    strFiltroRicetteOperazioni = strFiltroRicetteOperazioni.Substring(0, strFiltroRicetteOperazioni.Length - 1)
                Else
                    strFiltroRicetteOperazioni = "0"
                End If

            End If

        End If

        Return New FiltersDto With {
            .sa_cod = sa_cod, .veg_cod = veg_cod, .Data_Selezionata1 = Data_Selezionata1,
            .Data_Selezionata2 = Data_Selezionata2, .filtroQuery = filtroQuery,
            .TipoOperazioniStr = TipoOperazioniStr, .strFiltroRicetteOperazioni = strFiltroRicetteOperazioni,
            .stato = stato
        }

    End Function

    Private Function CreaTabella(filters As FiltersDto)
        Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
        Dim dtRicette As DataTable = objRicette.Leggi_xMenuAgenda(
            filters.piva, filters.sa_cod, 0, filters.veg_cod, 0, filters.Data_Selezionata1,
            filters.Data_Selezionata2, True, filters.filtroQuery, "", objPServer,
            filters.TipoOperazioniStr, filters.strFiltroRicetteOperazioni)

        dtRicette.Columns.Add(New DataColumn("veg_des_unificato", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("PermessoModifica", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("Descrizione_Unica", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("Dettaglio_Tecnico", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("Costi_Operatori", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("Costi_Macchine", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("Magazzini_Agenzie", Type.GetType("System.String")))

        dtRicette.Columns.Add(New DataColumn("ProdottiMagazzinoTrattati_Coinvolti", Type.GetType("System.String")))
        dtRicette.Columns.Add(New DataColumn("ProdottiMagazzinoTrattati_QuantitaQuintali", GetType(Decimal)))

        Dim dtRisultato As DataTable = dtRicette.Clone()

        Dim objUDM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim dtUDM = (From row In objUDM.Leggi(0, 0, "", "",
                                              1, 'Tabella dati minimi
                                              "", "",
                                              objPServer).Rows()).ToList()

        If Not IsNothing(dtRicette) AndAlso dtRicette.Rows.Count > 0 Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni As DataTable = ObjUtenti.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_MODIFICA_RICETTE_CON_OPERAZIONI_REGISTRATE,
                                                                  2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objPUtenti)

            Dim consentiModificaSeInUso As Boolean = If(Not IsNothing(dtImpostazioni) AndAlso dtImpostazioni.Rows.Count > 0 AndAlso dtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = "1", True, False)

            Dim strRicetta_Operazione_Cod As String() = (From r2 As DataRow In dtRicette.AsEnumerable Select New With {Key .roc = r2.Item("Ricetta_Operazione_Cod"), Key .rc = r2.Item("Ricetta_Cod"), Key .data = r2.Item("Ricetta_Operazione_Data")}).Distinct().OrderByDescending(Function(x) CDate(x.data)).ThenByDescending(Function(x) x.rc).Select(Function(y) CStr(y.roc)).ToArray()

            If Not IsNothing(strRicetta_Operazione_Cod) Then

                'COSTI ACCESSORI
                Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
                Dim DtCosti As DataTable = objCostiAccessori.CostiAccessori_from_Ricetta_Operazione_Cod(String.Join(",", strRicetta_Operazione_Cod), "", "", objPServer)

                'PRINCIPI ATTIVI
                Dim HtProdPA As New Hashtable()
                Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(dtRicette, HtProdPA, objPServer)

                'AVVERSITA
                Dim objMovTec As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
                Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_avversita(filters.piva, "", "", objPServer)

                For Each roc As Integer In strRicetta_Operazione_Cod

                    Dim dtRicettaSingola As DataTable = dtRicette.Select("Ricetta_Operazione_Cod=" & roc).CopyToDataTable()

                    Dim dr As DataRow = dtRicettaSingola.Rows(0)

                    If CDate(dr.Item("Validita_Inizio")) = AGRODATAINIZIO Then
                        dr.Item("Validita_Inizio") = DBNull.Value
                    End If

                    If CDate(dr.Item("Validita_Fine")) = AGRODATAFINE Then
                        dr.Item("Validita_Fine") = DBNull.Value
                    End If

                    If CDate(dr.Item("Ricetta_Operazione_Data")) = AGRODATAINIZIO Then
                        dr.Item("Ricetta_Operazione_Data") = dr.Item("Validita_Inizio")
                    End If

                    dr.Item("PermessoModifica") = If(dr.Item("Blocco_Flag") = 0 AndAlso (consentiModificaSeInUso OrElse (dr.Item("in_uso") = 0)), True, False)
                    dr.Item("Descrizione_Unica") = dr.Item("Ricetta_Numero") & " <b>" & dr.Item("lav_des") & "</b><br>" &
                                                       dr.Item("Ricetta_Operazione_Data") & " <i>" & dr.Item("Veg_Des_r") & "</i>"

                    Dim strApp_Nome As List(Of String) = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where CStr(rr.Item("App_Nome")) <> "" Select CStr(rr.Item("App_Nome"))).Distinct().ToList()
                    dr.Item("App_Nome") = String.Join(", ", strApp_Nome).Replace("'", "")

                    If dr.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA OrElse dr.Item("Lav_Cod") = LAVCOD_CONCIA_SEME Then
                        Dim strProdottiTrattati As List(Of String) = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where CStr(rr.Item("Mat_Des")) <> "" Select CStr(rr.Item("Mat_Des"))).Distinct().ToList()
                        dr.Item("ProdottiMagazzinoTrattati_Coinvolti") = String.Join(", ", strProdottiTrattati).Replace("'", "")
                    End If

                    If IsDBNull(dr.Item("veg_cod_op")) Then
                        Dim x = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where Not IsDBNull(rr.Item("veg_cod_op")) Select rr.Item("veg_cod_op")).ToList()
                        If x.Count > 0 Then
                            dr.Item("veg_cod_op") = x.First()
                        Else
                            dr.Item("veg_cod_op") = "0"
                        End If
                    End If

                    If IsDBNull(dr.Item("veg_des_op")) Then
                        Dim x = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where Not IsDBNull(rr.Item("veg_des_op")) Select rr.Item("veg_des_op")).ToList()
                        If x.Count > 0 Then
                            dr.Item("veg_des_op") = x.First()
                        End If
                    End If



                    dr.Item("veg_des_unificato") = If(IsNumeric(dr.Item("Tipo_Ricetta")) AndAlso {"5", "6", "9"}.Contains(dr.Item("Tipo_Ricetta")), If(IsDBNull(dr.Item("veg_des_op")), dr.Item("DestinazioneTerreniNudi_Des"), dr.Item("veg_des_op")), dr.Item("veg_des_r"))


                    '-------------------------------------------------------------------------------------------------------------------
                    'COSTI ACCESSORI
                    Dim listaOperatori As New List(Of String)
                    Dim listaMacchine As New List(Of String)
                    If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then
                        Dim DrCosti() As DataRow = DtCosti.Select("Ricetta_Operazione_Cod=" & roc)
                        If Not IsNothing(DrCosti) Then
                            For Each dr_costo As DataRow In DrCosti

                                'è un record manodopera
                                If dr_costo.Item("Cod_RisUm") <> 0 Then

                                    'Recupero il nome del contatto
                                    Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                                    If Not listaOperatori.Contains(nomeContatto) Then
                                        listaOperatori.Add(nomeContatto)
                                    End If

                                Else
                                    'è un record macchinario

                                    Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                                    detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                                    detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")

                                    If Not listaMacchine.Contains(detMacchina) Then
                                        listaMacchine.Add(detMacchina)
                                    End If

                                End If

                            Next
                        End If
                    End If

                    dr.Item("Costi_Operatori") = String.Join(", ", listaOperatori)
                    dr.Item("Costi_Macchine") = String.Join(", ", listaMacchine)
                    '-------------------------------------------------------------------------------------------------------------------

                    '-------------------------------------------------------------------------------------------------------------------
                    'DETTAGLIO TECNICO

                    Dim listaMagazzini_Agenzie As New List(Of String)

                    Dim listaDetTec As New List(Of String)
                    For Each drDetTec As DataRow In dtRicettaSingola.Rows
                        Dim prod As String = ""
                        Dim princAtt As String = ""
                        Dim avv As String = ""

                        If (drDetTec.Item("Lav_Cod") = LAVCOD_CONCIA_SEME OrElse drDetTec.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA) AndAlso
                            ((drDetTec.Item("Elem_Cod") = SEMENTI OrElse drDetTec.Item("Elem_Cod") = TRASFORMATI_VEGETALI) OrElse (drDetTec.Item("Elem_Cod") = FORMULATI AndAlso drDetTec.Item("QtaTotScaricoProdotto") = 0)) Then
                            'Se stiamo analizzando lav_cod di Concia del Seme o Trattamenti Post Raccolta,
                            'evito di mettere il prodotto magazzino trattato nel dettaglio tecnico 
                            'prendo solo le righe dove la Qta di scarico (ricette_Destinazioni.qta) > 0, che rappresentano le righe di scarico dei formulati
                            Continue For
                        End If

                        'PRODOTTI
                        Select Case drDetTec.Item("Elem_Cod")
                            Case FERTILIZZANTI
                                If drDetTec.Item("appezza") <> 0 Then
                                    ' Sto facendo riferimento al record che indica la quantità su Ha o Hl invece che quella totale
                                    Continue For
                                End If

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
                                If drDetTec.Item("appezza") <> 0 Then
                                    ' Sto facendo riferimento al record che indica la quantità su Ha o Hl invece che quella totale
                                    Continue For
                                End If

                                If drDetTec.Item("Fr_Des") <> "" Then
                                    prod = drDetTec.Item("Fr_Des")
                                End If

                            Case TRAPPOLE

                                If drDetTec.Item("Trap_Des") <> "" Then
                                    prod = drDetTec.Item("Trap_Des")
                                End If

                            Case INSETTI

                                If drDetTec.Item("Ins_Des") <> "" Then
                                    prod = drDetTec.Item("Ins_Des")
                                End If

                            Case SEMENTI

                                If drDetTec.Item("Mat_Des") <> "" Then
                                    prod = drDetTec.Item("Mat_Des") & " (Lotto:" & drDetTec.Item("Cod_Articolo") & ")"
                                End If

                            Case SEMILAVORATI_VEGETALI

                                If drDetTec.Item("Mat_Des") <> "" Then

                                    Select Case drDetTec.Item("Lav_Cod")
                                        Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                            prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                        Case Else
                                            prod = My.Resources.AgronicaCoreContabBIZ.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                    End Select
                                End If

                            Case TRASFORMATI_VEGETALI

                                If drDetTec.Item("Mat_Des") <> "" Then
                                    prod = drDetTec.Item("Mat_Des") & " (Articolo: " & drDetTec.Item("Cod_Articolo") & ")"
                                End If

                        End Select

                        'Aggiunge quantità e unità di misura
                        If prod <> "" AndAlso drDetTec.Item("Qta") <> 0 Then
                            Dim udmDes = dtUDM.Where(Function(row) row.Item("Udm_Cod") = drDetTec.Item("Udm_Cod")).
                                               Select(Function(row) row.Item("Udm_Des")).FirstOrDefault()
                            If Not IsNothing(udmDes) AndAlso udmDes <> "" Then
                                prod = prod & " (" & Agro_Math.RoundNumber_2Decimali(drDetTec.Item("Qta")) & " " & udmDes & " )"
                            End If
                        End If


                        Dim drMovDetTec2() As DataRow = dtMovDetTec.Select("Ricetta_Operazione_Cod=" & roc)
                        Dim listaAvv2 As New List(Of String)

                        For Each drAvv2 As DataRow In drMovDetTec2
                            If Not listaAvv2.Contains(drAvv2.Item("Av_des_vol")) AndAlso drAvv2.Item("Av_des_vol") <> "" Then
                                listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                            End If
                            If Not listaAvv2.Contains(drAvv2.Item("Av_Gru_des")) AndAlso drAvv2.Item("Av_Gru_des") <> "" Then
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

                        'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                        Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                        If Not listaDetTec.Contains(testoDetTec) Then
                            listaDetTec.Add(testoDetTec)
                        End If


                        'Mostro i magazzini delle agenzie da dove è stato preso il prodotto con i rispettivi quantitativi

                        Dim Magazzini_Agenzie_Cod As List(Of String) = drDetTec.Item("MagazzinoEsterno_Cod").ToString().Split("|").ToList().Where(Function(f) Not String.IsNullOrEmpty(f) AndAlso (f.Split("-").Count < 4 OrElse (f.Split("-").Count = 4 AndAlso f.Split("-")(3) = enum_MagazzinoEsterno_Tipo.Agenzia))).ToList()

                        If Not IsNothing(Magazzini_Agenzie_Cod) AndAlso Magazzini_Agenzie_Cod.Count > 0 Then

                            Dim Magazzini_Agenzie_Des As List(Of String) = drDetTec.Item("MagazzinoEsterno_Des").ToString().Split("|").ToList().Where(Function(f) Not String.IsNullOrEmpty(f)).ToList()

                            Dim MagazzinoEsterno_Qta As List(Of String) = drDetTec.Item("MagazzinoEsterno_Dettagli").ToString().Split("|").ToList().Where(Function(f) Not String.IsNullOrEmpty(f)).ToList()

                            If Not IsNothing(Magazzini_Agenzie_Des) AndAlso Magazzini_Agenzie_Des.Count > 0 AndAlso
                               Not IsNothing(MagazzinoEsterno_Qta) AndAlso MagazzinoEsterno_Qta.Count > 0 Then

                                For i = 0 To Magazzini_Agenzie_Cod.Count - 1
                                    listaMagazzini_Agenzie.Add(Magazzini_Agenzie_Des(i) & " (" & MagazzinoEsterno_Qta(i) & ")")
                                Next
                            End If
                        End If

                    Next

                    dr.Item("Dettaglio_Tecnico") = String.Join(", ", listaDetTec.Where(Function(detTec) detTec <> ""))

                    dr.Item("Magazzini_Agenzie") = String.Join(", ", listaMagazzini_Agenzie)


                    Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                    'Quantità Prodotti Magazzino Trattata
                    Dim fieldList As New List(Of String) From {"piva", "Sa_Cod", "ID_REG", "Mat_Cod", "Lotto"}
                    Dim _strProdotti(,) As String = dbUtil.SelectDistincFromFieldList(dtRicettaSingola, fieldList, False)

                    Dim QtaProdottoTrattataTot As Decimal = 0
                    For w = 0 To _strProdotti.Length / 5 - 1
                        'Considero solo le righe dove Id_Destinazione > 0
                        If _strProdotti(w, 2) > 0 Then
                            Dim drProdotto() As DataRow = dtRicettaSingola.Select(" piva = '" & _strProdotti(w, 0) & "'" &
                                                                                  " AND Sa_Cod = " & _strProdotti(w, 1) &
                                                                                  " AND ID_REG = " & _strProdotti(w, 2) &
                                                                                  " AND Mat_Cod = " & _strProdotti(w, 3) &
                                                                                  " AND Lotto = '" & _strProdotti(w, 4) & "'" &
                                                                                  " AND Tipo_Destinazione = " & MAGAZZINO &
                                                                                  " AND Elem_Cod <> " & FORMULATI) 'Escludo le righe di scarico formulati

                            If drProdotto.Length > 0 Then
                                QtaProdottoTrattataTot += CDec(drProdotto(0).Item("Qta"))
                            End If
                        End If
                    Next

                    'Espresso in quintali, su db salvato in KG!
                    dr.Item("ProdottiMagazzinoTrattati_QuantitaQuintali") = If(QtaProdottoTrattataTot > 0, QtaProdottoTrattataTot / 100, QtaProdottoTrattataTot)
                    '-------------------------------------------------------------------------------------------------------------------
                    'Superficie Trattata
                    Dim Sup_TrattataTot As Decimal = 0
                    Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(dtRicettaSingola, "APPEZZA", "ID_REG", False)

                    For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                        Dim drAppezza() As DataRow = dtRicettaSingola.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                        If drAppezza.Length > 0 Then
                            Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                        End If
                    Next

                    dr.Item("Sup_Trattata") = Sup_TrattataTot

                    '------------------------------------------------------------------------------------------------------------------

                    dtRisultato.ImportRow(dr)

                Next



            End If
        End If

        Return DT_to_Json_Ricette(dtRisultato, filters.stato, objPServer, objPUtenti)
    End Function

    Private Function estraiPrincipiAttivi(ByRef DtAgenda As DataTable, ByRef HtProdPA As Hashtable, objParametri_Server As AgronicaCoreParametri) As Hashtable

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
                Dim DtPrincipi As DataTable = objAgroWs.ComposizioneFormulatiRecupera(ElencoFormulati, objParametri_Server, objPUtenti)

                HtProdPA = New Hashtable()

                If DtPrincipi IsNot Nothing AndAlso DtPrincipi.Rows.Count > 0 Then
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
                    For Each dr As DataRow In DtPrincipi.Rows
                        If Not HtProdPA.ContainsKey(dr.Item("Fr_Cod")) Then
                            HtProdPA.Add(dr.Item("Fr_Cod"), dr.Item("Elenco_PrincipiAttivi"))
                        End If
                    Next
                End If

                objAgroWs = Nothing
            End If
        End If

        Return res

    End Function


    Public Shared Function DT_to_Json_Ricette(dt As DataTable, stato As enum_WWorflow_WAnagraficaStati, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'l.Add(New ColonneNome("WAnagraficaStati_Des", "Stato", "string"))
        l.Add(New ColonneNome("WAnagraficaStati_Cod", "WAnagraficaStati_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("WAnagraficaStati_Colore", "WAnagraficaStati_Colore", "string") With {._hidden = True})

        l.Add(New ColonneNome("piva", Gias.Piva, "string") With {._hidden = True})
        l.Add(New ColonneNome("pivaReale", Gias.Piva, "string") With {._Display = False})
        l.Add(New ColonneNome("Sa_Cod", "sa_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = False}) '"Centro Aziendale"
        l.Add(New ColonneNome("Raccoglitore_Cod", Gias.CodMultiAttivita, "string") With {._Display = False})
        l.Add(New ColonneNome("veg_cod_op", "veg_cod_op", "number") With {._hidden = True})
        l.Add(New ColonneNome("veg_cod_r", "veg_cod", "number") With {._hidden = True})
        'l.Add(New ColonneNome("veg_des_unificato", "Specie", "string") With {._Display = False})
        l.Add(New ColonneNome("veg_des_unificato", Gias.Specie, "string")) '"Specie"
        l.Add(New ColonneNome("Ricetta_Numero", Gias.CodiceRicetta, "string"))
        l.Add(New ColonneNome("Ricetta_Des", Gias.Descrizione, "string") With {._Display = False})
        l.Add(New ColonneNome("Tipo_Ricetta", "Tipo_Ricetta", "number") With {._hidden = True})
        l.Add(New ColonneNome("Tipo_Ricetta_des", Gias.TipoRicetta, "string") With {._Display = False})
        l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Display = False})
        l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Display = False})
        l.Add(New ColonneNome("Ricetta_Operazione_Cod", "ID", "number") With {._Display = False})

        l.Add(New ColonneNome("Ricetta_Operazione_Data", Gias.DataOperazione, "date")) ' "Data Operazione"
        l.Add(New ColonneNome("lav_cod", "lav_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("lav_des", Gias.Operazione, "string")) ' "Operazione"
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("App_Nome", Gias.Appezzamenti, "string")) ' "Appezzamenti"
        l.Add(New ColonneNome("Dettaglio_Tecnico", Gias.DettaglioTecnico, "string")) '
        l.Add(New ColonneNome("Sup_Trattata", Gias.SuperficieMovimentata, "number") With {._Display = False, ._FormatoParticolare = "#=(Sup_Trattata === 0) ? '' : Sup_Trattata.toString().replace('.', ',')#"})
        l.Add(New ColonneNome("Costi_Operatori", Gias.Operatori, "string")) 'With {._Display = False})
        l.Add(New ColonneNome("Costi_Macchine", Gias.Macchine, "string")) 'With {._Display = False})
        l.Add(New ColonneNome("APP_Ricetta_Operazione_ID", "APP_Ricetta_Operazione_ID", "string") With {._hidden = True})

        If dt.Select("ProdottiMagazzinoTrattati_Coinvolti IS NOT NULL AND ProdottiMagazzinoTrattati_Coinvolti <> ''").FirstOrDefault IsNot Nothing Then
            l.Add(New ColonneNome("ProdottiMagazzinoTrattati_Coinvolti", My.Resources.AgronicaCoreContabBIZ.ProdottiMagazzinoTrattatiCoinvolti, "string") With {._Display = False})
            l.Add(New ColonneNome("ProdottiMagazzinoTrattati_QuantitaQuintali", My.Resources.AgronicaCoreContabBIZ.ProdottiMagazzinoTrattatiQuantitaTotaleTrattata, "string") With {._Display = False})
        End If

        If stato = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita Then
            l.Add(New ColonneNome("Origine", Gias.Origine, "string"))
        End If

        l.Add(New ColonneNome("in_uso", "in_uso", "string") With {._hidden = True})
        l.Add(New ColonneNome("Invia_App", "in_uso", "string") With {._hidden = True})
        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", Gias.DescrizioneUnica, "string") With {._Display = False, ._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("Ricetta_Operazione_Des", Gias.Note, "string") With {._Display = False, ._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("Origine", Gias.Origine, "string") With {._hidden = True})

        'Mostro la colonna Magazzini Agenzie solo se ci sono delle Imprese Agenzie

        Dim objAnagrafeDAL As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim listaPivaAgenzie = objAnagrafeDAL.Imprese_Leggi_VisibilitaUtente_Agenzie_PIVA(objParametri_Server, objParametri_Utenti)

        If Not IsNothing(listaPivaAgenzie) AndAlso listaPivaAgenzie.Count > 0 Then
            l.Add(New ColonneNome("Magazzini_Agenzie", My.Resources.AgronicaCoreContabBIZ.MagazziniAgenzie, "string") With {._Display = False})
        End If

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = False})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

End Class

Public Class FiltersDto
    Public piva As String
    Public sa_cod As Integer
    Public veg_cod As Integer
    Public Data_Selezionata1 As Date
    Public Data_Selezionata2 As Date
    Public filtroQuery As String
    Public TipoOperazioniStr As String
    Public strFiltroRicetteOperazioni As String
    Public stato As String
End Class