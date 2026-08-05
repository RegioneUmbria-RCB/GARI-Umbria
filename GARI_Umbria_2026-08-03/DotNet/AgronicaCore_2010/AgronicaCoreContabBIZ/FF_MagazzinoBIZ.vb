Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabHLP.Contabilita
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json.Linq

Public Class FF_MagazzinoBIZ
    Inherits AgronicaCoreDataProvider.DataProvider

    '--------------------------------------------------------------------------------
    'Costanti parametri qualitativi
    '--------------------------------------------------------------------------------
    Private Const Tipo As String = "Tipo"
    Private Const Tabella_Key As String = "Tabella_Key"
    Private Const Tabella_Des As String = "Tabella_Des"
    Private Const NumDecimali_Maximo As String = "NumDecimali_Maximo"

    Private Const FF_ As String = "FF_"
    Private Const _Val_Cod As String = "_Val_Cod"
    Private Const _Tipo_Cod As String = "_Tipo_Cod"
    Private Const _Cod As String = "_Cod"
    Private Const _Tara_Campionatura As String = "_Tara_Campionatura"
    Private Const _Sigla As String = "_Sigla"
    Private Const _Descrizione As String = "_Descrizione"
    Private Const _Codice_Generazione_Link As String = "_Codice_Generazione_Link"
    Private Const _Mat_Cod_Generazione_Link As String = "_Mat_Cod_Generazione_Link"
    Private Const _ChkTara_Campionatura As String = "_ChkTara_Campionatura"
    Private Const _Tara_Anagrafica As String = "_Tara_Anagrafica"

    Private Const pq_imballaggio As String = "imballaggio"
    Private Const pq_contenitore As String = "contenitore"
    Private Const pq_confezione As String = "confezione"
    Private Const pq_fornitore As String = "fornitore"
    Private Const pq_cliente As String = "cliente"

    Private Const pq_formatoData As String = "yyyy-MM-dd"

    '--------------------------------------------------------------------------------
    'Costanti gruppi colonne
    '--------------------------------------------------------------------------------
    Private Const gruppoTestata As String = "Testata"
    Private Const gruppoProdotto As String = "Prodotto"
    Private Const gruppoEconomico As String = "Economico"
    Private Const gruppoQuantita As String = "Quantita"
    Private Const gruppoImputazioni As String = "Imputazioni"
    Private Const gruppoPesoRiscontrato As String = "PesoRiscontrato"
    Private Const gruppoProdottoGrigliaImballaggi As String = "Prodotto_GrigliaImballaggi"
    Private Const gruppoQuantitaGrigliaImballaggi As String = "Quantita_GrigliaImballaggi"
    Private Const gruppoPesoRiscontratoGrigliaImballaggi As String = "PesoRiscontrato_GrigliaImballaggi"

    Public Sub New()

    End Sub

    '##############################################################################################
    Public Function Leggi_Giacenza(ByVal piva As String,
                                   ByVal _prodotto As Integer,
                                   ByVal _specie As Integer,
                                   ByVal _varieta As Integer(),
                                   ByVal Sa_Cod As Integer,
                                   ByVal fabbricatoCod As Integer,
                                   ByVal Elem_Cod As String,
                                   ByVal nomeProdotto As String,
                                   ByVal codiceProdotto As String,
                                   ByVal Lotto As String,
                                   ByVal calibro As Integer,
                                   ByVal qualita As Integer,
                                   ByVal certificazione As Integer,
                                   ByVal rugginosita As Integer,
                                   ByVal imballaggio As Integer,
                                   ByVal contenitore As Integer,
                                   ByVal confezione As Integer,
                                   ByVal Cal_Cod As Integer,
                                   ByVal Data As String,
                                   ByVal VisualizzaGiacenzeZero As String,
                                   ByVal CifreArrotondamento As String,
                                   ByVal _mostraCampiInput As Boolean,
                                   ByVal _cercaLottoPerLike As Boolean,
                                   ByVal Flag_QtaMaggioreZero As Boolean,
                                   ByVal isFreshAndFood As Boolean,
                                   ByVal stringaFilterTutti As String,
                                   ByVal xOrderBy As String,
                                   ByRef dtDati As DataTable,
                                   ByRef objParametriServer As AgronicaCoreParametri,
                                   ByRef objParametriUtenti As AgronicaCoreParametri,
                                   Optional ByVal calCodEsclusi As String = Nothing
                                   ) As String

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_MagazzinoBIZ.Leggi_Giacenza()"

        Dim risposta As String = ""

        Dim _filtroSuVarieta As Boolean = False
        If _varieta IsNot Nothing AndAlso _varieta.Length > 0 Then
            _filtroSuVarieta = True
        End If

        Dim FF_gest_materiale_vivaistico = False
        Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
        If dtImpostazioni.Rows.Count > 0 AndAlso (dtImpostazioni(0)("Impostazione_Valore_1") = "1") Then
            FF_gest_materiale_vivaistico = True
        End If

        Dim matCod As Integer = 0
        Dim codProgetto As Integer = 0
        Dim faseCod As Integer = 0
        Dim udmCod As Integer = 0
        Dim proCod As Integer = 0

        If Lotto.Trim <> "" AndAlso Lotto.ToLower.Trim <> "indefinito" Then
            'StringaFilter2 = "(Movimenti_dettagli.Lotto like '%" & Lotto & "%')"
            'ok
        Else
            ' StringaFilter2 = ""
            Lotto = CStr(CInt(LOTTO_NONDEFINITO))
        End If

        If stringaFilterTutti <> "" Then
            stringaFilterTutti &= " AND Movimenti_Dettagli.Elem_Cod <> -1"
        Else
            stringaFilterTutti = " Movimenti_Dettagli.Elem_Cod <> -1"
        End If

        stringaFilterTutti = String.Format("AND {0}", stringaFilterTutti)

        Dim stringaFilterMateriePrime As String = ""
        If calibro <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_calibro.Tipo_Cod = " & CStr(calibro)
        End If
        If qualita <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_qualità.Tipo_Cod = " & CStr(qualita)
        End If
        If certificazione <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_certificazioni.Tipo_Cod = " & CStr(certificazione)
        End If
        If rugginosita <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_rugginosita.Tipo_Cod = " & CStr(rugginosita)
        End If
        If imballaggio <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_imballaggio.Tipo_Cod = " & CStr(imballaggio)
        End If
        If contenitore <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_contenitore.Tipo_Cod = " & CStr(contenitore)
        End If
        If confezione <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime_Campionature_confezione.Tipo_Cod = " & CStr(confezione)
        End If
        If _specie <> -1 Then
            stringaFilterMateriePrime &= " AND Materie_Prime.Veg_Cod = " & CStr(_specie)
        End If
        If _prodotto <> 0 Then
            stringaFilterMateriePrime &= " AND Materie_Prime.Mat_Cod = " & _prodotto
        End If
        If _filtroSuVarieta Then
            stringaFilterMateriePrime &= " AND Materie_Prime.Cul_Cod IN ( "
            Dim primoGiro As Boolean = True
            For Each v In _varieta
                If Not primoGiro Then
                    stringaFilterMateriePrime &= ", "
                End If
                primoGiro = False
                stringaFilterMateriePrime &= CStr(v)
            Next
            stringaFilterMateriePrime &= " ) "
        End If

        Dim dataRicerca As Date

        If Data = "" Then
            dataRicerca = AGRODATAFINE
        Else
            dataRicerca = CDate(Data)
        End If

        If Elem_Cod = "" Then
            Elem_Cod = 0
        End If

        If codiceProdotto <> "" AndAlso IsNumeric(codiceProdotto) Then
            proCod = codiceProdotto
        End If

        Dim dtGiacenze As DataTable
        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R

        Dim leggiLinea = False
        If Elem_Cod = TRASFORMATI_VEGETALI Then
            leggiLinea = True
        End If

        Dim strQueryOutput As String = ""

        dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(dataRicerca,
                                                         piva,
                                                         Sa_Cod,
                                                         fabbricatoCod,
                                                         Elem_Cod,
                                                         proCod,
                                                         matCod,
                                                         Cal_Cod,
                                                         codProgetto,
                                                         faseCod,
                                                         udmCod,
                                                         Lotto,
                                                         Not CBool(VisualizzaGiacenzeZero),
                                                         stringaFilterTutti,
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         "",
                                                         stringaFilterMateriePrime,
                                                         "",
                                                         stringaFilterMateriePrime,
                                                         stringaFilterMateriePrime,
                                                         "",
                                                         objParametriServer,
                                                         objParametriUtenti,
                                                         "",
                                                         strQueryOutput,
                                                         isFreshAndFood,
                                                         _cercaLottoPerLike,
                                                         Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero,
                                                         xFiltroAggiuntivo_15:=stringaFilterMateriePrime,
                                                         leggiLinea:=leggiLinea,
                                                         calCodEsclusi:=calCodEsclusi)

        Dim numProdotti As Integer = 0
        Dim numRecord As Integer = 0

        'Me.LBL_NumProd.Text = CStr(0)

        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim dtParamQual As DataTable = objConfigDettagli.Leggi(piva, 0, False, "Tipo = 1", "", objParametriServer)
        Dim htParamQual As New Hashtable
        Dim bAbilitaLottoInterno As Boolean = False

        '----- Definisco la struttura del DataTable

        dtDati = New DataTable
        dtDati.Columns.Add(New DataColumn("piva", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Id_Destinazione", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Ubicazione_Des", GetType(String)))

        dtDati.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
        dtDati.Columns.Add(New DataColumn("Cat_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Pro_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Pro_Des", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Mat_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Referenza", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Udm_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        dtDati.Columns.Add(New DataColumn("KgLordi", GetType(Decimal)))
        dtDati.Columns.Add(New DataColumn("NrConfezioni", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("NrContenitori", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("NrImballaggi", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("TaraTotale", GetType(Decimal)))
        dtDati.Columns.Add(New DataColumn("TaraProdotto", GetType(Decimal)))
        dtDati.Columns.Add(New DataColumn("TaraTotaleAssoluta", GetType(Decimal)))
        dtDati.Columns.Add(New DataColumn("Qta_Extra", GetType(Decimal)))

        '25/03/2021: Per evitare casini con le funzioni che ricalcolano il peso, che si aspettano KgNetti,
        'Lascio cmq solo la colonna dei KgNetti, ma che mostro come Qta
        'devo però a questo punto modificare eventuali righe movimentate a Kg, ma con confezionamento, che su db hanno udm = nr
        'e azzerare Nr Conf se movimentato con altra udm

        dtDati.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
        dtDati.Columns.Add(New DataColumn("KgNetti", GetType(Decimal)))

        dtDati.Columns.Add(New DataColumn("Lotto_Interno", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Lotto", GetType(String)))

        dtDati.Columns.Add(New DataColumn("Cal_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Totale", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Cod_Progetto", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))

        For Each paramQual In dtParamQual.Rows

            ColumnsAddParamQual(dtDati, paramQual)

        Next

        dtDati.Columns.Add(New DataColumn("Fornitore", GetType(String)))

        If _mostraCampiInput Then
            dtDati.Columns.Add(New DataColumn("Lordo_Mov", GetType(Decimal)))
            dtDati.Columns.Add(New DataColumn("NrImballaggi_Mov", GetType(Integer)))
            dtDati.Columns.Add(New DataColumn("NrContenitori_Mov", GetType(Integer)))
            dtDati.Columns.Add(New DataColumn("NrConfezioni_Mov", GetType(Integer)))
            dtDati.Columns.Add(New DataColumn("Netto_Mov", GetType(Decimal)))
            dtDati.Columns.Add(New DataColumn("Qta_Mov", GetType(Decimal)))
            dtDati.Columns.Add(New DataColumn("Tara_Mov", GetType(Decimal)))
        End If

        dtDati.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        dtDati.Columns.Add(New DataColumn("Regolamento", GetType(Integer)))

        If leggiLinea Then
            dtDati.Columns.Add(New DataColumn("Linea_Cod", GetType(Integer)))
        End If

        dtDati.Columns.Add(New DataColumn("chiave_giacenze", GetType(String)))

        If dtGiacenze IsNot Nothing AndAlso dtGiacenze.Rows.Count > 0 Then

            For i = 0 To dtGiacenze.Rows.Count - 1

                Dim giacenza As Decimal = 0
                If Not IsDBNull(dtGiacenze.Rows(i).Item("Giacenza")) AndAlso IsNumeric(Not IsDBNull(dtGiacenze.Rows(i).Item("Giacenza"))) Then
                    giacenza = CDec(dtGiacenze.Rows(i).Item("Giacenza"))
                    'If Arrotondamento >= 0 Then
                    '    Giacenza = Agro_Math.RoundNumber(Giacenza, Arrotondamento)
                    'End If
                End If

                'oltre al filtro nella query, devo fare anche il filtro su codice
                'perché molti Decimal vengono salvati come valori infinitamente piccoli
                'ad esempio 0.00003680000000017003

                ' Modificato il 13/7/2018 per evitare di avere le giacenze inferiori ad un Kg
                'If CBool(VisualizzaGiacenzeZero) = True Or (giacenza <> 0 And Not (giacenza < 0.00009 And giacenza > -0.00009)) Then
                If CBool(VisualizzaGiacenzeZero) = True OrElse (giacenza <> 0 AndAlso Not (giacenza < 1 AndAlso giacenza > -1)) Then

                    numRecord += 1

                    Dim dr As DataRow

                    'Creo una nuova riga
                    dr = dtDati.NewRow

                    'magazzino
                    dr.Item("Piva") = dtGiacenze.Rows(i).Item("Piva")
                    dr.Item("Sa_Cod") = dtGiacenze.Rows(i).Item("Sa_Cod")
                    dr.Item("Sa_Nome") = dtGiacenze.Rows(i).Item("Sa_Nome")
                    dr.Item("Tipo_Destinazione") = dtGiacenze.Rows(i).Item("Tipo_Destinazione")
                    dr.Item("Id_Destinazione") = dtGiacenze.Rows(i).Item("Id_Destinazione")
                    If Not String.IsNullOrEmpty(dtGiacenze.Rows(i).Item("Identificativo").ToString()) Then
                        ' dr.Item("insieme_des") = DtGiacenze.Rows(i).Item("Fabbricato_Des")
                        dr.Item("Ubicazione_Des") = dtGiacenze.Rows(i).Item("Identificativo")
                    Else
                        dr.Item("Ubicazione_Des") = dtGiacenze.Rows(i).Item("Fabbricato_Des")
                    End If

                    dr.Item("Data_Movimento") = dtGiacenze.Rows(i).Item("Data_Movimento")

                    'Definisco i valori
                    dr.Item("Cat_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Elem_Cod")), dtGiacenze.Rows(i).Item("Elem_Cod"), 0)
                    dr.Item("Cat_Des") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("NomeComune")), dtGiacenze.Rows(i).Item("NomeComune"), "")
                    dr.Item("Pro_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Pro_Cod")), dtGiacenze.Rows(i).Item("Pro_Cod"), 0)
                    dr.Item("Mat_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Mat_Cod")), dtGiacenze.Rows(i).Item("Mat_Cod"), 0)
                    dr.Item("Cod_Articolo") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("NomeComune")), dtGiacenze.Rows(i).Item("Cod_Articolo"), "")

                    dr.Item("Pro_Des") = dtGiacenze.Rows(i).Item("Descrizione_Prodotto")
                    dr.Item("Referenza") = dtGiacenze.Rows(i).Item("Descrizione_Prodotto")

                    dr.Item("Cod_Progetto") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cod_Progetto")), dtGiacenze.Rows(i).Item("Cod_Progetto"), 0)

                    Select Case dtGiacenze.Rows(i).Item("Elem_Cod")

                        Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI

                            dr.Item("Lotto_Interno") = dtGiacenze.Rows(i).Item("Lotto_Interno")
                            bAbilitaLottoInterno = True

                        Case Else


                    End Select

                    dr.Item("Lotto") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Lotto")), dtGiacenze.Rows(i).Item("Lotto"), "")
                    dr.Item("Cal_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cal_Cod")), dtGiacenze.Rows(i).Item("Cal_Cod"), 0)

                    dr.Item("Udm_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Udm_Cod")), dtGiacenze.Rows(i).Item("Udm_Cod"), 0)
                    dr.Item("Udm_Des") = dtGiacenze.Rows(i).Item("Udm_Sim")

                    If CInt(CifreArrotondamento) >= 0 Then
                        giacenza = Agro_Math.ArrotondaVal(giacenza, CifreArrotondamento)
                    End If

                    dr.Item("KgNetti") = giacenza
                    dr.Item("Qta") = giacenza

                    dr.Item("NrImballaggi") = dtGiacenze.Rows(i).Item("NrImballaggi")
                    dr.Item("NrContenitori") = dtGiacenze.Rows(i).Item("NrContenitori")
                    dr.Item("NrConfezioni") = dtGiacenze.Rows(i).Item("NrConfezioni")
                    dr.Item("Qta_Extra") = dtGiacenze.Rows(i).Item("Qta_Extra")

                    dr.Item("Fase_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Fase_Cod")), dtGiacenze.Rows(i).Item("Fase_Cod"), 0)

                    For Each paramQual In dtParamQual.Rows

                        CaricaParamQualGiacenza(paramQual,
                                                dtGiacenze.Rows(i),
                                                dr,
                                                htParamQual,
                                                objParametriServer)

                    Next

                    ' Non viene usata la Tara Totale della movimenti_dettagli perché non è sempre valorizzata e può
                    ' essere stata forzata dall'utente con un numero non coerente rispetto alla tara effettiva degli imballaggi
                    ' Viene quindi ricalcolata come Nr Imballaggi / Nr Contenitori / Nr Confezioni * la rispettiva tara
                    'dr.Item("TaraTotale") = dtGiacenze.Rows(i).Item("TaraTotale")
                    dr.Item("TaraTotale") = 0
                    If dr.Table.Columns("FF_imballaggio_Tara_Campionatura") IsNot Nothing Then
                        dr.Item("TaraTotale") += dr.Item("NrImballaggi") * dr.Item("FF_imballaggio_Tara_Campionatura")
                    End If
                    If dr.Table.Columns("FF_contenitore_Tara_Campionatura") IsNot Nothing Then
                        dr.Item("TaraTotale") += dr.Item("NrContenitori") * dr.Item("FF_contenitore_Tara_Campionatura")
                    End If
                    If dr.Table.Columns("FF_confezione_Tara_Campionatura") IsNot Nothing Then
                        dr.Item("TaraTotale") += dr.Item("NrConfezioni") * dr.Item("FF_confezione_Tara_Campionatura")
                    End If

                    'TaraTotale = Tara effettiva degli imballaggi collegati
                    'TaraProdotto = Tara aggiuntiva (rispetto alla TaraTotale qui ricalcolata):
                    '   l'utente modifica a mano la tara totale calcolata (perché magari era 0)
                    '   di fatto andando ad aggiungere tara che consideriamo "addebitata" al prodotto
                    If CDec(dtGiacenze.Rows(i).Item("TaraTotale")) = 0 Then
                        dr.Item("TaraProdotto") = 0D    'Caso particolare in cui non era salvata
                    Else
                        dr.Item("TaraProdotto") = CDec(dtGiacenze.Rows(i).Item("TaraTotale")) - CDec(dr.Item("TaraTotale"))
                    End If
                    dr.Item("TaraTotaleAssoluta") = dr.Item("TaraTotale") + dr.Item("TaraProdotto")

                    'dr.Item("KgLordi") = giacenza + dr.Item("TaraTotale")
                    dr.Item("KgLordi") = giacenza + dr.Item("TaraTotaleAssoluta")

                    If _mostraCampiInput Then
                        dr.Item("Lordo_Mov") = 0
                        dr.Item("NrImballaggi_Mov") = 0
                        dr.Item("NrContenitori_Mov") = 0
                        dr.Item("NrConfezioni_Mov") = 0
                        dr.Item("Netto_Mov") = 0
                        dr.Item("Qta_Mov") = 0
                        dr.Item("Tara_Mov") = 0
                    End If

                    dr.Item("Veg_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Veg_Cod")), dtGiacenze.Rows(i).Item("Veg_Cod"), 0)
                    dr.Item("Cul_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cul_Cod")), dtGiacenze.Rows(i).Item("Cul_Cod"), 0)
                    dr.Item("Regolamento") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Regolamento")), dtGiacenze.Rows(i).Item("Regolamento"), 0)

                    If leggiLinea Then
                        dr.Item("Linea_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Linea_Cod")), dtGiacenze.Rows(i).Item("Linea_Cod"), 0)
                    End If

                    dr.Item("chiave_giacenze") =
                        dr.Item("piva") & "_" &
                        CStr(dr.Item("sa_cod")) & "_" &
                        dr.Item("Tipo_Destinazione") & "_" &
                        dr.Item("Id_Destinazione") & "_" &
                        dr.Item("cat_cod") & "_" &
                        dr.Item("pro_cod") & "_" &
                        dr.Item("mat_cod") & "_" &
                        dr.Item("Lotto") & "_" &
                        dr.Item("cal_cod") & "_" &
                        dr.Item("cod_progetto") & "_" &
                        dr.Item("udm_cod")

                    '25/03/2021: Manipolo la riga per la nuova gestione udm (lasciare come ultima cosa)
                    Dim udmCodRiga As Integer = CInt(dr.Item("udm_cod"))
                    Dim confezioneCodRiga As Integer = 0
                    If dr.Table.Columns("FF_confezione_Tipo_Cod") IsNot Nothing Then
                        confezioneCodRiga = CInt(dr.Item("FF_confezione_Tipo_Cod"))
                    End If

                    If udmCodRiga = enum_UnitaMisura.Numero Then

                        If confezioneCodRiga = 0 Then
                            'Ho movimentato volutamente a nr ==> nr confezioni non mi interessa
                            '==> la giacenza potrebbe essere il risultato di qta * qta_extra (anche se quest'ultima dovrebbe essere 1)
                            '   , quindi per sicurezza inverto i campi

                            dr.Item("Qta") = dr.Item("NrConfezioni")
                            'dr.Item("KgNetti") = dr.Item("NrConfezioni")
                            dr.Item("NrConfezioni") = 0
                        Else

                            'Ho movimentato a KG, ma avevo scelto la confezione, quindi su db ora risulta movimentato a Nr
                            '==> se non avevo qta_extra, allora giacenza = nr_confezioni
                            '==> se avevo qta_extra, allora giacenza = nr_confezioni * qta_extra (kg netti)

                            If CDec(dr.Item("Qta_Extra")) <> 0 Then
                                'dr.Item("Qta") = dr.Item("NrConfezioni")
                                'dr.Item("KgNetti") = dr.Item("NrConfezioni")

                                'In Giacenza (KgNetti) ho effettivamente i kg, quindi cambio l'udm perché sia coerente
                                dr.Item("udm_cod") = 2
                                dr.Item("udm_des") = "kg"
                            End If

                        End If

                    End If

                    'Associo alla tabella la nuova riga creata
                    dtDati.Rows.Add(dr)

                End If 'Filtro arrotondamento giacenza

            Next 'ciclo giacenze

        End If

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave_giacenze", "chiave_giacenze", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Movimento", My.Resources.AgronicaCoreContabBIZ.PrimaEntrata, "date")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        ' Se si fa il lock della colonna e non ci sono pulsanti non mostra correttamente la griglia alla prima entrata
        If _mostraCampiInput Then
            c._locked = True
        End If
        c._width = "92px"
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Sa_Nome", Gias.CentroAziendale, "string")
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._width = "130px"
        l.Add(c)

        c = New ColonneNome("Id_Destinazione", "Id_Destinazione", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Ubicazione_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_MagOCella, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("Cat_Cod", "Elem_Cod", "number")
        c._hidden = True
        l.Add(c)

        'c = New ColonneNome("Cat_Des", "Categoria", "string")
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'l.Add(c)

        c = New ColonneNome("Pro_Cod", "Pro_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Lotto", Gias.Lotto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        c._Editabile = False
        ' Se si fa il lock della colonna e non ci sono pulsanti non mostra correttamente la griglia alla prima entrata
        If _mostraCampiInput Then
            c._locked = True
        End If
        c._width = "120px"
        l.Add(c)

        c = New ColonneNome("Referenza", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Referenza, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        c._Editabile = False
        ' Se si fa il lock della colonna e non ci sono pulsanti non mostra correttamente la griglia alla prima entrata
        If _mostraCampiInput Then
            c._locked = True
        End If
        c._width = "280px"
        l.Add(c)

        c = New ColonneNome("Pro_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Prodotto, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._Editabile = False
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("Cod_Articolo", My.Resources.AgronicaCoreContabBIZ.CodiceArticolo, "string")
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        c._Editabile = False
        c._width = "100px"
        l.Add(c)

        'Caratteristiche prodotto
        For Each paramQual In dtParamQual.Rows

            AggiungiColonneParamQualGiacenza(l, paramQual, htParamQual)

        Next

        If htParamQual.ContainsKey(pq_fornitore) Then
            c = New ColonneNome("Fornitore", Gias.Fornitore, "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            c._Display = False
            c._Editabile = False
            c._width = "100px"
            l.Add(c)
        End If

        If _mostraCampiInput Then
            If htParamQual.ContainsKey(pq_imballaggio) Then
                c = New ColonneNome("NrImballaggi_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoImballaggiAbbr, "number")
                c._Filtrabile = False
                c._Display = True
                c._Editabile = True
                c._formatNr = "n0"
                c._width = "70px"
                l.Add(c)
            End If

            If htParamQual.ContainsKey(pq_contenitore) Then
                c = New ColonneNome("NrContenitori_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoContenitoriAbbr, "number")
                c._Filtrabile = False
                c._Display = True
                c._Editabile = True
                c._formatNr = "n0"
                c._width = "73px"
                l.Add(c)
            End If

            If htParamQual.ContainsKey(pq_confezione) Then
                c = New ColonneNome("NrConfezioni_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoConfezioniAbbr, "number")
                c._Filtrabile = False
                c._Display = True
                c._Editabile = True
                c._formatNr = "n0"
                c._width = "73px"
                l.Add(c)
            End If

            c = New ColonneNome("Lordo_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoLordoAbbr, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._Filtrabile = False
                c._Display = True
                c._Editabile = True
                c._formatNr = "n0"
                c._obbligatorio = True
                c._width = "77px"
                l.Add(c)
            End If

            c = New ColonneNome("Tara_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoTaraAbbr, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._Filtrabile = False
                c._Display = True
                c._Editabile = True
                c._formatNr = "n0"
                c._obbligatorio = True
                c._width = "77px"
                l.Add(c)
            End If

            If FF_gest_materiale_vivaistico Then
                c = New ColonneNome("Netto_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoNumeroAbbr, "number")
            Else
                c = New ColonneNome("Netto_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoNettoAbbr, "number")
            End If
            c._Filtrabile = False
            c._Display = True
            c._Editabile = True
            c._formatNr = "n0"
            c._obbligatorio = True
            c._width = "75px"
            l.Add(c)

            c = New ColonneNome("Qta_Mov", My.Resources.AgronicaCoreContabBIZ.IngressoQuantitaAbbr, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._hidden = False
            End If
            c._Display = True
            c._Filtrabile = False
            c._Editabile = True
            c._formatNr = "n0"
            c._obbligatorio = True
            c._width = "75px"
            l.Add(c)
        End If

        c = New ColonneNome("Cal_Cod", "Cal_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Fase_Cod", "Fase_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cod_Progetto", "Cod_Progetto", "string")
        c._hidden = True
        l.Add(c)

        If bAbilitaLottoInterno = True Then
            c = New ColonneNome("Lotto_Interno", Gias.Lotto_Interno, "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            c._Display = True
            c._Editabile = False
            ' Se si fa il lock della colonna e non ci sono pulsanti non mostra correttamente la griglia alla prima entrata
            If _mostraCampiInput Then
                c._locked = True
            End If
            c._width = "120px"
            l.Add(c)
        End If






        'Descrizione imballo/contenitore/confezione
        For Each paramQual In dtParamQual.Rows
            If htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then
                    c = New ColonneNome(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), paramQual(Tabella_Key) & _Tipo_Cod, "number")
                    c._hidden = True
                    l.Add(c)
                End If
            End If
        Next

        For Each paramQual In dtParamQual.Rows
            If paramQual(Tabella_Key) = pq_imballaggio AndAlso htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                c = New ColonneNome(GetNomeColonnaDescrizione(paramQual(Tabella_Key)), paramQual(Tabella_Des), "string")
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._Display = False
                c._Editabile = False
                c._width = "115px"
                l.Add(c)
            End If
        Next

        For Each paramQual In dtParamQual.Rows
            If paramQual(Tabella_Key) = pq_contenitore AndAlso htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                c = New ColonneNome(GetNomeColonnaDescrizione(paramQual(Tabella_Key)), paramQual(Tabella_Des), "string")
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._Display = False
                c._Editabile = False
                c._width = "115px"
                l.Add(c)
            End If
        Next

        For Each paramQual In dtParamQual.Rows
            If paramQual(Tabella_Key) = pq_confezione AndAlso htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                c = New ColonneNome(GetNomeColonnaDescrizione(paramQual(Tabella_Key)), paramQual(Tabella_Des), "string")
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._Display = False
                c._Editabile = False
                c._width = "115px"
                l.Add(c)
            End If
        Next

        If htParamQual.ContainsKey(pq_imballaggio) Then
            c = New ColonneNome("NrImballaggi", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrImb, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._width = "70px"
            c._sum = True
            l.Add(c)
        End If

        If htParamQual.ContainsKey(pq_contenitore) Then
            c = New ColonneNome("NrContenitori", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrCont, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._width = "75px"
            c._sum = True
            l.Add(c)
        End If

        If htParamQual.ContainsKey(pq_confezione) Then
            c = New ColonneNome("NrConfezioni", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrConfez, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._width = "73px"
            c._sum = True
            l.Add(c)
        End If

        '''''c = New ColonneNome("TaraTotale", "Tara Totale", "number")
        '''''    c._Filtrabile = True
        '''''    c._Display = True
        '''''    c._formatNr = "n2"
        '''''    c._Editabile = False
        '''''l.Add(c)

        'Tare
        If htParamQual.ContainsKey(pq_imballaggio) Then

            c = New ColonneNome("FF_imballaggio_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnImb, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            'c._sum = True
            c._width = "70px"
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Codice_Generazione_Link", "FF_imballaggio_Codice_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Mat_Cod_Generazione_Link", "FF_imballaggio_Mat_Cod_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

        End If

        If htParamQual.ContainsKey(pq_contenitore) Then

            c = New ColonneNome("FF_contenitore_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnCont, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n2"
            c._Editabile = False
            'c._sum = True
            c._width = "77px"
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Codice_Generazione_Link", "FF_contenitore_Codice_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Mat_Cod_Generazione_Link", "FF_contenitore_Mat_Cod_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

        End If

        If htParamQual.ContainsKey(pq_confezione) Then

            c = New ColonneNome("FF_confezione_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnConfez, "number")
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n3"
            c._Editabile = False
            'c._sum = True
            c._width = "73px"
            l.Add(c)

            c = New ColonneNome("FF_confezione_Codice_Generazione_Link", "FF_confezione_Codice_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_confezione_Mat_Cod_Generazione_Link", "FF_confezione_Mat_Cod_Generazione_Link", "number")
            c._hidden = True
            l.Add(c)

        End If

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_UnitàMisura, "string")
        If FF_gest_materiale_vivaistico Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            c._Display = True
            c._Editabile = False
            c._width = "80px"
        End If
        l.Add(c)

        c = New ColonneNome("Qta", Gias.Quantita, "number")
        If FF_gest_materiale_vivaistico Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._sum = False
            c._width = "70px"
        End If
        l.Add(c)

        c = New ColonneNome("KgLordi", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgLordi, "number")
        If FF_gest_materiale_vivaistico Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._sum = True
            c._width = "73px"
        End If
        l.Add(c)

        c = New ColonneNome("TaraTotale", "TaraTotaleImballaggi", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TaraProdotto", "TaraProdotto", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TaraTotaleAssoluta", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraTotale, "number")
        If FF_gest_materiale_vivaistico Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._sum = True
            c._width = "73px"
        End If
        l.Add(c)

        If FF_gest_materiale_vivaistico Then
            c = New ColonneNome("KgNetti", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Numero, "number")
        Else
            c = New ColonneNome("KgNetti", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgNetti, "number")
        End If
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n0"
        c._Editabile = False
        c._sum = True
        If FF_gest_materiale_vivaistico Then
            c._width = "90px"
        Else
            c._width = "70px"
        End If
        l.Add(c)

        c = New ColonneNome("Veg_Cod", "Veg_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Cod", "Cul_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento", "Regolamento", "number")
        c._hidden = True
        l.Add(c)

        If leggiLinea Then
            c = New ColonneNome("Linea_Cod", "Linea_Cod", "number")
            c._hidden = True
            l.Add(c)
        End If

        Dim js As New JSON_DataTable
        js.Editabile_Deafault = False
        risposta = js.JSON_DataTable_Kendo(dtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risposta

    End Function

    Private Sub ColumnsAddParamQual(ByRef dtDati As DataTable, ByVal paramQual As Object)

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero
                dtDati.Columns.Add(New DataColumn(GetNomeColonnaValCod(paramQual(Tabella_Key)), GetType(Decimal)))

            Case enum_TipoParamQual.Stringa, enum_TipoParamQual.Data
                dtDati.Columns.Add(New DataColumn(GetNomeColonnaValCod(paramQual(Tabella_Key)), GetType(String)))

            Case Else

                dtDati.Columns.Add(New DataColumn(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), GetType(Integer)))

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then
                    dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura, GetType(Decimal)))
                    dtDati.Columns.Add(New DataColumn(GetNomeColonnaSigla(paramQual(Tabella_Key)), GetType(String)))
                    dtDati.Columns.Add(New DataColumn(GetNomeColonnaDescrizione(paramQual(Tabella_Key)), GetType(String)))
                    dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _Codice_Generazione_Link, GetType(Integer)))
                    dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _Mat_Cod_Generazione_Link, GetType(Integer)))
                End If

        End Select

    End Sub

    Private Sub CaricaParamQualGiacenza(
        ByVal paramQual As Object,
        ByRef drGiacenze As DataRow,
        ByRef dr As DataRow,
        ByRef htParamQual As Hashtable,
        ByRef objParametriServer As AgronicaCoreParametri)

        Dim paramQualTipo As Integer = paramQual(Tipo)
        Dim paramQualTabellaKey As String = paramQual(Tabella_Key)
        Dim paramQualTabellaDes As String = paramQual(Tabella_Des)

        Const separatore = " - "

        Select Case paramQualTipo

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQualTabellaKey)

                Dim drGiacenzeValCod = drGiacenze.Item(nomeColonnaValCod)

                If paramQualTipo = enum_TipoParamQual.Numero And Not IsNumeric(drGiacenzeValCod) Then
                    drGiacenzeValCod = "0"
                End If

                AddElemHtParamQual(paramQual, drGiacenzeValCod, htParamQual)

                If paramQualTipo = enum_TipoParamQual.Numero Then
                    dr.Item(nomeColonnaValCod) = CDec(CStr(drGiacenzeValCod).Replace(".", ","))
                Else
                    dr.Item(nomeColonnaValCod) = drGiacenzeValCod
                End If

            Case Else

                Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQualTabellaKey)
                Dim nomeColonnaSigla = GetNomeColonnaSigla(paramQualTabellaKey)
                Dim nomeColonnaDescrizione = GetNomeColonnaDescrizione(paramQualTabellaKey)
                Dim nomeColonnaTaraCampionatura = FF_ & paramQualTabellaKey & _Tara_Campionatura
                Dim nomeColonnaCodGenLink = FF_ & paramQualTabellaKey & _Codice_Generazione_Link
                Dim nomeColonnaMatCodGenLink = FF_ & paramQualTabellaKey & _Mat_Cod_Generazione_Link

                Dim drGiacenzeTipoCod = drGiacenze.Item(nomeColonnaTipoCod)

                AddElemHtParamQual(paramQual, drGiacenzeTipoCod, htParamQual)

                If paramQualTabellaKey = pq_fornitore Then
                    dr.Item(nomeColonnaTipoCod) = drGiacenze.Item(nomeColonnaTipoCod)
                End If

                If Not IsParamQualClienteFornitore(paramQualTabellaKey) Then

                    dr.Item(nomeColonnaTipoCod) = drGiacenze.Item(nomeColonnaTipoCod)
                    dr.Item(nomeColonnaTaraCampionatura) = drGiacenze.Item(nomeColonnaTaraCampionatura)
                    dr.Item(nomeColonnaSigla) = drGiacenze.Item(nomeColonnaSigla)
                    dr.Item(nomeColonnaDescrizione) = drGiacenze.Item(nomeColonnaDescrizione)
                    dr.Item(nomeColonnaCodGenLink) = drGiacenze.Item(nomeColonnaCodGenLink)
                    dr.Item(nomeColonnaMatCodGenLink) = drGiacenze.Item(nomeColonnaMatCodGenLink)

                End If

                If paramQualTabellaKey = pq_fornitore Then

                    Dim objContattiR = New Contatti_R

                    If Not String.IsNullOrEmpty(drGiacenze.Item(nomeColonnaTipoCod).ToString) Then

                        Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(CStr(drGiacenzeTipoCod), objParametriServer)

                        If objContatto.Rows.Count > 0 Then
                            dr.Item("Fornitore") = objContatto.Rows(0)("rag_soc")
                            AccodaStringaInReferenza("Fornitore", dr.Item("Fornitore"), separatore, dr)
                        End If

                    End If

                End If

        End Select

        ' Composizione referenza

        If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) AndAlso htParamQual(paramQual(Tabella_Key)) <> "" Then

            If Not IsParamQualConfezionamento(paramQualTabellaKey) Then
                AccodaInReferenza(paramQual, htParamQual, separatore, dr)
            End If

        End If

    End Sub

    Private Function GetNomeColonnaValCod(ByVal paramQualTabellaKey As String) As String

        Return FF_ & paramQualTabellaKey & _Val_Cod

    End Function

    Private Function GetNomeColonnaTipoCod(ByVal paramQualTabellaKey As String) As String

        Return FF_ & paramQualTabellaKey & _Tipo_Cod

    End Function

    Private Function GetNomeColonnaSigla(ByVal paramQualTabellaKey As String) As String

        Return FF_ & paramQualTabellaKey & _Sigla

    End Function

    Private Function GetNomeColonnaDescrizione(ByVal paramQualTabellaKey As String) As String

        Return FF_ & paramQualTabellaKey & _Descrizione

    End Function

    Private Sub AggiungiColonneParamQualGiacenza(
        ByRef l As List(Of ColonneNome),
        ByVal paramQual As Object,
        ByRef htParamQual As Hashtable)

        Dim paramQualTipo As Integer = paramQual(Tipo)
        Dim paramQualTabellaKey As String = paramQual(Tabella_Key)

        Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQualTabellaKey)
        Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQualTabellaKey)
        Dim nomeColonnaSigla = GetNomeColonnaSigla(paramQualTabellaKey)

        Dim nomeColonnaTipoCodVis = paramQualTabellaKey & _Tipo_Cod

        Dim c As ColonneNome

        Select Case paramQualTipo

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                If htParamQual.ContainsKey(paramQualTabellaKey) Then
                    AggiungiSingolaColonnaParamQualGiacenza(nomeColonnaValCod,
                                                            htParamQual(paramQualTabellaKey),
                                                            paramQual,
                                                            l)
                End If

            Case Else

                If paramQualTabellaKey = pq_fornitore Then

                    c = New ColonneNome(nomeColonnaTipoCod, nomeColonnaTipoCodVis, "number")
                    c._hidden = True
                    l.Add(c)

                End If

                If Not ({pq_cliente, pq_fornitore, pq_imballaggio, pq_contenitore, pq_confezione}).Contains(paramQualTabellaKey) Then

                    If htParamQual.ContainsKey(paramQualTabellaKey) Then

                        c = New ColonneNome(nomeColonnaTipoCod, nomeColonnaTipoCodVis, "number")
                        c._hidden = True
                        l.Add(c)

                        AggiungiSingolaColonnaParamQualGiacenza(nomeColonnaSigla,
                                                                htParamQual(paramQualTabellaKey),
                                                                paramQual,
                                                                l)

                    End If

                End If

        End Select

    End Sub

    Private Sub AggiungiSingolaColonnaParamQualGiacenza(
        ByVal nomeColonna As String,
        ByVal nomeColonnaVisualizzato As String,
        ByVal paramQual As Object,
        ByRef l As List(Of ColonneNome))

        Dim tipoDatoColonna As String

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero
                tipoDatoColonna = "number"

            Case enum_TipoParamQual.Stringa
                tipoDatoColonna = "string"

            Case enum_TipoParamQual.Data
                tipoDatoColonna = "date"

            Case Else
                tipoDatoColonna = "string"

        End Select

        Dim c = New ColonneNome(nomeColonna, nomeColonnaVisualizzato, tipoDatoColonna)

        SetFormatNrParamQualNumerico(c, paramQual)

        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = False
        c._Editabile = False
        c._width = "100px"

        l.Add(c)

    End Sub

    Public Function Leggi_Giacenze_Globale(ByVal Tipo_Aggregazione As Integer,
                                           ByVal soloCampiApp As Boolean,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As String,
                                        ByVal Tipo_Fabbricato_Cod As Integer,
                                        ByVal Fabbricato_Cod As Integer,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Lotto As String,
                                        ByVal Cal_Cod As Integer,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Data As Date,
                                        ByVal isFreshAndFood As Boolean,
                                        ByVal flag_QtaNoZero As Boolean,
                                        ByVal xOrderBy As String,
                                        ByRef objParametriServer As AgronicaCoreParametri,
                                        ByRef objParametriUtenti As AgronicaCoreParametri,
                                        Optional ByVal bUdm_Des As Boolean = False,
                                        Optional ByVal Flag_QtaMaggioreZero As Boolean = False) As DataTable

        Dim risposta As String = ""
        Dim strFiltro As String = ""

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_MagazzinoBIZ.Leggi_Ipno_Giacenze()"

        Dim dtGiacenze As DataTable
        Dim dtDati As DataTable
        Dim dtDatiAggregati As New DataTable
        Dim dtLavCodCompatibiliFormulati As DataTable = Nothing
        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R

        Dim xFiltroAggiuntivo = ""
        If Tipo_Fabbricato_Cod <> 0 Then

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                xFiltroAggiuntivo &= " And "
            End If
            xFiltroAggiuntivo &= " Fabbricati_Tipi.Tipo_Fabbricato_Cod = " & CStr(Tipo_Fabbricato_Cod)
        End If

        If soloCampiApp Then
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                xFiltroAggiuntivo &= " And "
            End If
            xFiltroAggiuntivo &= " Movimenti_dettagli.Elem_Cod In ( "

            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.FERTILIZZANTI)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.FORMULATI)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.INSETTI)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.TRAPPOLE)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.INNESCHI)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.SEMENTI)
            xFiltroAggiuntivo &= ", "
            xFiltroAggiuntivo &= CStr(CostantiPersonalizzate.TRASFORMATI_VEGETALI)

            xFiltroAggiuntivo &= " ) "

            ' Solo giacenze di magazzini visibili da APP
            Dim leggi_fabbricati_codice As New Fabbricati_Codici_R
            Dim magAPP = leggi_fabbricati_codice.Leggi(Piva, 0, 0, CInt(enum_CodiciAnagrafe.Visibile_da_App), "1", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)
            Dim xF = ""
            For Each r In magAPP.Rows
                If r.Item("val_cod") = "1" Then
                    If Not String.IsNullOrEmpty(xF) Then
                        xF &= " Or "
                    End If
                    xF &= " ( "
                    xF &= " Mov_Destinazioni.Sa_Cod = "
                    xF &= CStr(r.Item("Sa_Cod"))
                    xF &= " And "
                    xF &= " Mov_Destinazioni.Id_Destinazione = "
                    xF &= CStr(r.Item("Fabbricato_Cod"))
                    xF &= " ) "
                End If
            Next
            If Not String.IsNullOrEmpty(xF) Then
                xFiltroAggiuntivo &= " And ( "
                xFiltroAggiuntivo &= xF
                xFiltroAggiuntivo &= " ) "
            Else
                xFiltroAggiuntivo &= " And 1 = 2 "
            End If

            'Aggiunta lettura Lav_Cod compatibili con il prodotto per i formulati
            Dim objMetaschemaDAl As New AgronicaCoreMetaSchemaDAL.FormulatixClassifica_R
            dtLavCodCompatibiliFormulati = objMetaschemaDAl.LeggiLavCodCompatibili(0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametriServer)
        End If

        If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
            xFiltroAggiuntivo = " And ( " & xFiltroAggiuntivo & " ) "
        End If

        Dim strQueryOutput As String = ""
        dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(Data,
                                                         Piva,
                                                         Sa_Cod,
                                                         Fabbricato_Cod,
                                                         Elem_Cod,
                                                         Pro_Cod, Mat_Cod, Cal_Cod, 0, 0, Udm_Cod, Lotto,
                                                         flag_QtaNoZero,
                                                         xFiltroAggiuntivo, "", "", "", "", "", "", "", "",
                                                         "", "",
                                                         "",
                                                         objParametriServer, objParametriUtenti,
                                                         "", strQueryOutput, isFreshAndFood, False, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)

        Dim numProdotti As Integer = 0
        Dim numRecord As Integer = 0

        dtDati = New DataTable
        dtDati.Columns.Add(New DataColumn("piva", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Sa_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Fabbricato_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Fabbricato_Des", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Elem_Cod", GetType(String)))
        If Not soloCampiApp Then
            dtDati.Columns.Add(New DataColumn("Pro_Cod", GetType(String)))
            dtDati.Columns.Add(New DataColumn("Mat_Cod", GetType(String)))
            dtDati.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))
        Else
            dtDati.Columns.Add(New DataColumn("Prodotto_Cod", GetType(String)))
            dtDati.Columns.Add(New DataColumn("LavCodCompatibiliFormulati", GetType(List(Of Integer))) With {.DefaultValue = New List(Of Integer)()})
        End If
        dtDati.Columns.Add(New DataColumn("Udm_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Lotto", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Cal_Cod", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Cod_Progetto", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Chiave_Giacenza", GetType(String)))
        dtDati.Columns.Add(New DataColumn("Giacenza", GetType(Decimal)))

        If Not soloCampiApp Then
            dtDati.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
            dtDati.Columns.Add(New DataColumn("NomeComune", GetType(String)))
            dtDati.Columns.Add(New DataColumn("Prodotto_Des", GetType(String)))
            dtDati.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        End If

        If dtGiacenze IsNot Nothing AndAlso dtGiacenze.Rows.Count > 0 Then

            For Each r In dtGiacenze.Rows

                Dim giacenza As Decimal = 0

                If Not IsDBNull(r.Item("Giacenza")) AndAlso IsNumeric(Not IsDBNull(r.Item("Giacenza"))) Then

                    giacenza = CDec(r.Item("Giacenza"))

                    numRecord += 1

                    Dim dr As DataRow

                    'Creo una nuova riga
                    dr = dtDati.NewRow

                    'magazzino
                    dr.Item("Piva") = r.Item("Piva")
                    dr.Item("Sa_Cod") = r.Item("Sa_Cod")
                    dr.Item("Tipo_Destinazione") = r.Item("Tipo_Destinazione")
                    dr.Item("Fabbricato_Cod") = r.Item("Id_Destinazione")
                    dr.Item("Fabbricato_Des") = r.Item("Fabbricato_Des")
                    If isFreshAndFood AndAlso Not String.IsNullOrEmpty(r.Item("Identificativo").ToString()) Then
                        dr.Item("Fabbricato_Des") = r.Item("Identificativo")
                    End If
                    dr.Item("Elem_Cod") = If(Not IsDBNull(r.Item("Elem_Cod")), r.Item("Elem_Cod"), 0)

                    dr.Item("Cod_Progetto") = If(Not IsDBNull(r.Item("Cod_Progetto")), r.Item("Cod_Progetto"), 0)
                    dr.Item("Lotto") = If(Not IsDBNull(r.Item("Lotto")), r.Item("Lotto"), "")
                    dr.Item("Cal_Cod") = If(Not IsDBNull(r.Item("Cal_Cod")), r.Item("Cal_Cod"), 0)
                    dr.Item("Udm_Cod") = If(Not IsDBNull(r.Item("Udm_Cod")), r.Item("Udm_Cod"), 0)

                    If Not soloCampiApp Then
                        dr.Item("Sa_Nome") = New CentriAziendali_Read().SaNome_from_SaCod(r.Item("Piva"), r.Item("Sa_Cod"), objParametriServer)
                        dr.Item("NomeComune") = If(Not IsDBNull(r.Item("NomeComune")), r.Item("NomeComune"), 0)
                        dr.Item("Prodotto_Des") = r.Item("Descrizione_Prodotto")

                        Select Case bUdm_Des
                            Case True
                                dr.Item("Udm_Des") = r.Item("Udm_Des")
                            Case False
                                dr.Item("Udm_Des") = r.Item("Udm_Sim")
                        End Select


                        dr.Item("Pro_Cod") = If(Not IsDBNull(r.Item("Pro_Cod")), r.Item("Pro_Cod"), 0)
                        dr.Item("Mat_Cod") = If(Not IsDBNull(r.Item("Mat_Cod")), r.Item("Mat_Cod"), 0)
                        dr.Item("Fase_Cod") = If(Not IsDBNull(r.Item("Fase_Cod")), r.Item("Fase_Cod"), 0)
                    Else
                        If Not IsDBNull(r.Item("Mat_Cod")) AndAlso CInt(r.Item("Mat_Cod")) > 0 Then
                            dr.Item("Prodotto_Cod") = CInt(r.Item("Mat_Cod")) * -1
                        Else
                            dr.Item("Prodotto_Cod") = If(Not IsDBNull(r.Item("Pro_Cod")), r.Item("Pro_Cod"), 0)
                        End If

                        If Not IsNothing(dtLavCodCompatibiliFormulati) AndAlso dr.Item("Elem_Cod") = FORMULATI Then

                            Dim drLavCodCompatibili As DataRow() = dtLavCodCompatibiliFormulati.Select("FOR_Cod = " & dr.Item("Prodotto_Cod"))

                            If Not IsNothing(drLavCodCompatibili) AndAlso drLavCodCompatibili.Length > 0 Then
                                dr.Item("LavCodCompatibiliFormulati") = drLavCodCompatibili.CopyToDataTable().AsEnumerable().Select(Function(x) x.Field(Of Integer)("Lav_Cod")).ToList()
                            End If

                        End If
                    End If

                    'Nota: non cambiare l'ordine
                    If Not soloCampiApp Then
                        dr.Item("Chiave_Giacenza") = dr.Item("Piva") & "_" &
                                                 dr.Item("Sa_Cod") & "_" &
                                                 dr.Item("Sa_Nome") & "_" &
                                                 dr.Item("Tipo_Destinazione") & "_" &
                                                 dr.Item("Fabbricato_Cod") & "_" &
                                                 dr.Item("Fabbricato_Des") & "_" &
                                                 dr.Item("Elem_Cod") & "_" &
                                                 dr.Item("NomeComune") & "_" &
                                                 dr.Item("Pro_Cod") & "_" &
                                                 dr.Item("Mat_Cod") & "_" &
                                                 dr.Item("Lotto") & "_" &
                                                 dr.Item("Cal_Cod") & "_" &
                                                 dr.Item("Cod_Progetto") & "_" &
                                                 dr.Item("Udm_Cod") & "_" &
                                                 dr.Item("Udm_Des") & "_" &
                                                 dr.Item("Prodotto_Des")

                    Else
                        dr.Item("Chiave_Giacenza") = dr.Item("Piva") & "_" &
                                                 dr.Item("Sa_Cod") & "_" &
                                                 dr.Item("Tipo_Destinazione") & "_" &
                                                 dr.Item("Fabbricato_Cod") & "_" &
                                                 dr.Item("Fabbricato_Des") & "_" &
                                                 dr.Item("Elem_Cod") & "_" &
                                                 dr.Item("Prodotto_Cod") & "_" &
                                                 dr.Item("Lotto") & "_" &
                                                 dr.Item("Cal_Cod") & "_" &
                                                 dr.Item("Cod_Progetto") & "_" &
                                                 dr.Item("Udm_Cod")

                    End If


                    dr.Item("Giacenza") = giacenza

                    'Associo alla tabella la nuova riga creata
                    dtDati.Rows.Add(dr)

                End If


            Next 'ciclo giacenze

        End If

        Select Case Tipo_Aggregazione

            Case 0 'Nessuna Aggregazione

                Return dtDati


            Case 1 'Prodotto

                Dim listDatiAggregati = (From row In dtDati
                                         Group row By GiacenzePerProdotto = New With {
                                                   Key .Chiave_Giacenza = row.Field(Of String)("Chiave_Giacenza"),
                                                     .Prodotto_Des = row.Field(Of String)("Prodotto_Des")
                                                              } Into Group
                                         Order By GiacenzePerProdotto.Prodotto_Des
                                         Select New With {
                                                GiacenzePerProdotto.Chiave_Giacenza,
                                                GiacenzePerProdotto.Prodotto_Des,
                                                .Giacenza = Group.Sum(Function(x) x.Field(Of Decimal)("Giacenza"))
                                        }).ToList


                dtDatiAggregati.Columns.Add(New DataColumn("Chiave_Giacenza", GetType(String)))
                dtDatiAggregati.Columns.Add(New DataColumn("Prodotto_Des", GetType(String)))
                dtDatiAggregati.Columns.Add(New DataColumn("Giacenza", GetType(Decimal)))

                Dim d0 As DataRow
                For Each r In listDatiAggregati
                    d0 = dtDatiAggregati.NewRow
                    d0("Chiave_Giacenza") = r.Chiave_Giacenza
                    d0("Prodotto_Des") = r.Prodotto_Des
                    d0("Giacenza") = r.Giacenza
                    dtDatiAggregati.Rows.Add(d0)
                Next

                Return dtDatiAggregati

        End Select

        Return Nothing

    End Function



    '##############################################################################################
    Public Function Controllo_Giacenza(ByVal piva As String,
                                       ByVal Sa_Cod As String,
                                       ByVal strFabbricato_Cod As String,
                                       ByVal elemCod As Integer,
                                       ByVal nomeProdotto As String,
                                       ByVal CodiceProdotto As String,
                                       ByVal Lotto As String,
                                       ByVal Cal_Cod As Integer,
                                       ByVal data As Date,
                                       ByVal cifreArrotondamentoPesi As Integer,
                                       ByVal messaggioDettagliato As Boolean,
                                       ByVal checkPeso As Boolean,
                                       ByVal imballiDaScaricare As Integer,
                                       ByVal contenitoriDaScaricare As Integer,
                                       ByVal confezioniDaScaricare As Integer,
                                       ByVal kgLordiDaScaricare As Decimal,
                                       ByVal kgNettiDaScaricare As Decimal,
                                       ByRef objParametriServer As AgronicaCoreParametri,
                                       ByRef objParametriUtenti As AgronicaCoreParametri,
                                       Optional ByVal Flag_QtaNoZero As Boolean = False,
                                       Optional ByVal Flag_QtaMaggioreZero As Boolean = False
                                       ) As String

        Const nomeRoutine = "AgronicaCoreContabBIZ.FF_MagazzinoBIZ.Controllo_Giacenza()"
        Dim messErrore As String = ""

        'Dim _filtroSuVarieta As Boolean = False
        'If Not _varieta Is Nothing And _varieta.Length > 0 Then
        '    _filtroSuVarieta = True
        'End If

        Dim fabbricatoCod As Integer = strFabbricato_Cod.Split("|")(0)
        Dim matCod As Integer = 0
        Dim codProgetto As Integer = 0
        Dim faseCod As Integer = 0
        Dim udmCod As Integer = 0
        Dim proCod As Integer = 0

        Dim stringaFilter As String = ""

        If Lotto.Trim <> "" AndAlso Lotto.ToLower.Trim <> "indefinito" Then
        Else
            Lotto = CStr(CInt(LOTTO_NONDEFINITO))
        End If

        If stringaFilter <> "" Then
            stringaFilter &= " And Movimenti_Dettagli.Elem_Cod <> -1"
        Else
            stringaFilter = " Movimenti_Dettagli.Elem_Cod <> -1"
        End If

        stringaFilter = String.Format("And {0}", stringaFilter)


        If CodiceProdotto <> "" AndAlso IsNumeric(CodiceProdotto) Then
            matCod = CodiceProdotto
        End If

        Dim dtGiacenze As DataTable
        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R

        Dim strQueryOutput As String = ""
        dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(data,
                                                         piva,
                                                         Sa_Cod,
                                                         fabbricatoCod,
                                                         elemCod,
                                                         proCod, matCod, Cal_Cod, codProgetto, faseCod, udmCod, Lotto,
                                                         Flag_QtaNoZero,
                                                         stringaFilter, "", "", "", "", "", "", "", "",
                                                         "", "",
                                                         "",
                                                         objParametriServer, objParametriUtenti,
                                                         "", strQueryOutput, True,
                                                         Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)

        Dim sumImballi As Integer = 0
        Dim sumContenitori As Integer = 0
        Dim sumConfezioni As Integer = 0
        Dim sumKgLordi As Decimal = 0
        Dim sumKgNetti As Decimal = 0

        If dtGiacenze IsNot Nothing AndAlso dtGiacenze.Rows.Count > 0 Then

            For i = 0 To dtGiacenze.Rows.Count - 1

                Dim giacenza As Decimal = 0
                Dim tara As Decimal = 0
                If Not IsDBNull(dtGiacenze.Rows(i).Item("Giacenza")) AndAlso IsNumeric(Not IsDBNull(dtGiacenze.Rows(i).Item("Giacenza"))) Then
                    giacenza = CDec(dtGiacenze.Rows(i).Item("Giacenza"))

                    If cifreArrotondamentoPesi >= 0 Then
                        giacenza = Decimal.Round(giacenza, cifreArrotondamentoPesi, MidpointRounding.AwayFromZero)
                    End If

                End If

                If Not IsDBNull(dtGiacenze.Rows(i).Item("TaraTotale")) AndAlso IsNumeric(Not IsDBNull(dtGiacenze.Rows(i).Item("TaraTotale"))) Then
                    tara = CDec(dtGiacenze.Rows(i).Item("TaraTotale"))

                    If cifreArrotondamentoPesi >= 0 Then
                        tara = Decimal.Round(tara, cifreArrotondamentoPesi, MidpointRounding.AwayFromZero)
                    End If

                End If

                'dr.Item("Id_Destinazione") = dtGiacenze.Rows(i).Item("Id_Destinazione")
                'If Not String.IsNullOrEmpty(CStr(dtGiacenze.Rows(i).Item("Identificativo"))) Then
                '    ' dr.Item("insieme_des") = dtGiacenze.Rows(i).Item("Fabbricato_Des")
                '    dr.Item("Ubicazione_Des") = dtGiacenze.Rows(i).Item("Identificativo")
                'Else
                '    dr.Item("Ubicazione_Des") = dtGiacenze.Rows(i).Item("Fabbricato_Des")
                'End If

                sumKgNetti = sumKgNetti + giacenza
                sumKgLordi = sumKgLordi + giacenza + tara
                sumImballi += CDec(dtGiacenze.Rows(i).Item("NrImballaggi"))
                sumContenitori += CDec(dtGiacenze.Rows(i).Item("NrContenitori"))
                sumConfezioni += CDec(dtGiacenze.Rows(i).Item("NrConfezioni"))

            Next 'ciclo giacenze

        End If

        If (checkPeso AndAlso sumKgNetti < kgNettiDaScaricare) OrElse
           (checkPeso AndAlso sumKgLordi < kgLordiDaScaricare) OrElse
           sumImballi < imballiDaScaricare OrElse
           sumContenitori < contenitoriDaScaricare OrElse
           sumConfezioni < confezioniDaScaricare Then

            If messaggioDettagliato Then
                messErrore = "Giacenza non sufficiente.  Sono disponibili: " & "<br/>"
                If imballiDaScaricare > 0 Then
                    messErrore &= sumImballi.ToString() & " imballaggi" & "<br/>"
                End If
                If contenitoriDaScaricare > 0 Then
                    messErrore &= sumContenitori.ToString() & " contenitori" & "<br/>"
                End If
                If confezioniDaScaricare > 0 Then
                    messErrore &= sumConfezioni.ToString() & " confezioni" & "<br/>"
                End If
                If checkPeso AndAlso kgLordiDaScaricare Then
                    messErrore &= sumKgNetti.ToString() & " Kg. Lordi" & "<br/>"
                End If
                If checkPeso AndAlso kgNettiDaScaricare > 0 Then
                    messErrore &= sumKgNetti.ToString() & " Kg. Netti" & "<br/>"
                End If
            Else
                messErrore = "Giacenza non sufficiente"
            End If
        End If

        Return messErrore

    End Function

    '============================================================================
    ''' <summary>
    ''' Elenco movimenti Carico/Scarico
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Id_Agenda"></param>
    ''' <param name="Cau_Mov"></param>
    ''' <param name="idMovDet"></param>
    ''' <param name="lav_cod_chiamante"></param>
    ''' <param name="movimentiDaGiacenza"></param>
    ''' <param name="DtDati"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="orderBy"></param>
    ''' <param name="SaCodDoc"></param>
    ''' 
    ''' <param name="ordiniDaEvadere">
    ''' Lettura ordini di acquisto o vendita.<br/>
    ''' Significativo solo se <paramref name="lav_cod_chiamante"/> coerente.<br/>
    ''' </param>
    ''' 
    ''' <param name="mostraCampiTestata">
    ''' Aggiunge colonne relative a dati di testata.
    ''' </param>
    ''' 
    ''' <param name="statoOrdineDoc"></param>
    ''' <param name="ddtDaEvadere"></param>
    ''' 
    ''' <param name="ordiniLavoroDaEvadere">
    ''' Lettura ordini di lavorazione da evadere.<br/>
    ''' Significativo solo se <paramref name="lav_cod_chiamante"/> coerente.<br/>
    ''' Aggiunge le colonne relative allo situazione evasione: quantità e stato.
    ''' </param>
    ''' 
    ''' <param name="mostraOrdiniEvasi">
    ''' Se <paramref name="ordiniDaEvadere"/> o <paramref name="ordiniLavoroDaEvadere"/>,
    ''' restituisce anche gli ordini evasi.
    ''' </param>
    ''' 
    ''' <returns>JSON DataTable Kendo</returns>
    Public Function Leggi_Movimenti_Carico_Scarico(ByVal Piva As String,
                                                   ByVal Id_Agenda As Integer,
                                                   ByVal Cau_Mov As String,
                                                   ByVal idMovDet As Integer,
                                                   ByVal lav_cod_chiamante As Integer,
                                                   ByVal movimentiDaGiacenza As Boolean,
                                                   ByRef DtDati As DataTable,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef objParametriUtenti As AgronicaCoreParametri,
                                                   Optional ByVal xFiltroAggiuntivo As String = "",
                                                   Optional ByVal orderBy As String = "",
                                                   Optional ByRef SaCodDoc As Integer = 0,
                                                   Optional ByVal ordiniDaEvadere As Boolean = False,
                                                   Optional ByVal mostraCampiTestata As Boolean = False,
                                                   Optional ByRef statoOrdineDoc As enum_StatoOrdine = enum_StatoOrdine.INDEFINITO,
                                                   Optional ByVal ddtDaEvadere As Boolean = False,
                                                   Optional ByVal ordiniLavoroDaEvadere As Boolean = False,
                                                   Optional ByVal mostraOrdiniEvasi As Boolean = False,
                                                   Optional ByVal gruppiMerce As Boolean = False,
                                                   Optional ByVal leggiPratica As Boolean = False,
                                                   Optional ByVal contestoDocContabile As Boolean = False
                                                   ) As String

        Const nomeRoutine = "FF_MagazzinoBIZ.Leggi_Movimenti_Carico_Scarico()"
        Dim messaggioErrore As String = ""
        Dim risposta As String = ""

        Dim lavCodDDTAcquisto As Integer() = {LAVCOD_BOLLA_RICEVUTA}
        Dim lavCodAccettazione As Integer() = {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO}
        Dim lavCodDDTVendita As Integer() = {LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA}
        Dim lavCodFattAcquisto As Integer() = {LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}
        Dim lavCodFattVendita As Integer() = {LAVCOD_FATTURA_EMESSA}
        Dim lavCodNotaAccrVendita As Integer() = {LAVCOD_NOTA_ACCREDITO_EMESSA}
        Dim lavCodOrdineAcquisto As Integer() = {LAVCOD_ORDINE_ACQUISTO}
        Dim lavCodOrdineVendita As Integer() = {LAVCOD_ORDINE_VENDITA}
        Dim lavCodTrasferimento As Integer() = {LAVCOD_TRASFERIMENTO}
        Dim lavCodLavorazioni As Integer() = {LAVCOD_TESTATE_ORDINE_LAVORAZIONE, LAVCOD_TRASFORMAZIONI}
        Dim lavCodMagazzino As Integer() = {LAVCOD_CARICO, LAVCOD_SCARICO}
        Dim lavCodContrattoAffitto As Integer() = {LAVCOD_CONTRATTO_AFFITTO}

        Dim moduliFF As enum_Omni_Modulo_Generazione() = {enum_Omni_Modulo_Generazione.FreshFood, enum_Omni_Modulo_Generazione.Tabacco, enum_Omni_Modulo_Generazione.Zoo}

        Dim FF_gest_materiale_vivaistico = False
        Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_FF_GEST_MATERIALE_VIVAISTICO, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
        If dtImpostazioni.Rows.Count > 0 AndAlso (dtImpostazioni(0)("Impostazione_Valore_1") = "1") Then
            FF_gest_materiale_vivaistico = True
        End If

        Dim permessoGestionePrezziLettura As Boolean = False
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        permessoGestionePrezziLettura = objPermessi.Controlla_Permessi_Utente(objParametriUtenti.UtenteUsername,
                                                                              enum_Id_Servizio.GiasOnline,
                                                                              enum_Security_Attivita.Gestione_Prezzi,
                                                                              enum_Security_Operazione.Lettura,
                                                                              Now, "",
                                                                              objParametriUtenti)

        Dim leggiProvvigioni As Boolean = False
        Dim leggiContiEconPatr As Boolean = False
        Dim leggiOrdini As Boolean = False
        Dim leggiOrdiniDaEvadere As Boolean = False
        Dim leggiDDTDaEvadere As Boolean = False
        Dim leggiTrasferimenti As Boolean = False
        Dim leggiLavorazioni As Boolean = False
        Dim leggiConferimenti As Boolean = False
        Dim leggiRiferimento As Boolean = False
        Dim leggiRifOrdineCliente As Boolean = False
        Dim LeggiOrdiniLavoroDaEvadere As Boolean = False

        Dim leggiCdC As Boolean = False
        If (lav_cod_chiamante = LAVCOD_ORDINE_ACQUISTO OrElse
                lav_cod_chiamante = LAVCOD_BOLLA_EMESSA) Then
            dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_DocContabili_SceltaImputazione, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
            If dtImpostazioni.Rows.Count > 0 AndAlso (dtImpostazioni(0)("Impostazione_Valore_1") = "1") Then
                leggiCdC = True
            End If
        End If

        Dim w_imponibile As Decimal = 0
        Dim w_imponibileNetto As Decimal = 0
        Dim w_iva As Decimal = 0

        If lavCodOrdineVendita.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineAcquisto.Contains(lav_cod_chiamante) Then
            leggiOrdini = True
        End If

        If lavCodOrdineVendita.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineAcquisto.Contains(lav_cod_chiamante) Then
            leggiOrdiniDaEvadere = ordiniDaEvadere
        End If

        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodDDTAcquisto.Contains(lav_cod_chiamante) Then
            leggiDDTDaEvadere = ddtDaEvadere
        End If

        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodAccettazione.Contains(lav_cod_chiamante) OrElse
           lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
           lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodNotaAccrVendita.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineVendita.Contains(lav_cod_chiamante) Then
            leggiProvvigioni = True
            leggiContiEconPatr = True
        End If

        If lav_cod_chiamante = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
            LeggiOrdiniLavoroDaEvadere = ordiniLavoroDaEvadere
        End If

        dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SuperUser_PesiColli_Riscontrati, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
        Dim impPesiRiscontrati As Integer = If(dtImpostazioni.Rows.Count > 0, dtImpostazioni(0)("Impostazione_Valore_1"), 0)

        Dim leggiPesiRiscontrati As Boolean = False
        'lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
        'lavCodAccettazione.Contains(lav_cod_chiamante) OrElse
        'lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
           lavCodNotaAccrVendita.Contains(lav_cod_chiamante) Then

            leggiPesiRiscontrati = If(impPesiRiscontrati = 1, True, False)

        End If

        If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
            leggiTrasferimenti = True
        End If

        If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
            leggiLavorazioni = True
        End If

        If lavCodAccettazione.Contains(lav_cod_chiamante) Then
            leggiConferimenti = True
        End If

        'I movimenti di riferimento li leggo solo se sto leggendo una specifica Id_Agenda
        '(No trasferimenti, perché sono particolari e usano i riferimenti in maniera diversa rispetto agli altri lav_cod)
        If Id_Agenda <> 0 AndAlso leggiTrasferimenti = False Then
            leggiRiferimento = True
        End If

        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
           lavCodDDTAcquisto.Contains(lav_cod_chiamante) Then
            leggiRifOrdineCliente = True
        End If

        Dim objMovDetR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim dtMovimentiDettagli As DataTable
        DtDati = New DataTable

        If xFiltroAggiuntivo <> "" Then
            xFiltroAggiuntivo &= " And "
        End If

        'xFiltroAggiuntivo &= " (Jolly_Int = 0 Or Agenda.Lav_Cod <> " & LAVCOD_TRASFORMAZIONI & ") "
        'If lav_cod_chiamante = LAVCOD_TRASFORMAZIONI Then
        '    xFiltroAggiuntivo &= " And (Agenda.Lav_Cod = " & LAVCOD_TRASFORMAZIONI & ") "
        'End If

        If lav_cod_chiamante = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then
            xFiltroAggiuntivo &= " ( (Jolly_Int = 1 And Movimenti_dettagli.Extra_Str = '' AND Movimenti_dettagli.Extra_Int = 0) OR Agenda.Lav_Cod <> " & LAVCOD_TESTATE_ORDINE_LAVORAZIONE & ") "
        Else
            xFiltroAggiuntivo &= " (Jolly_Int = 0 OR Agenda.Lav_Cod <> " & LAVCOD_TRASFORMAZIONI & ") "
        End If

        If lavCodAccettazione.Contains(lav_cod_chiamante) Then
            xFiltroAggiuntivo &= " And (Movimenti_Dettagli.Elem_Cod <> " & BENI_CONFEZ_VEGETALE & ") "
        End If

        If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
            xFiltroAggiuntivo &= " AND (Agenda.Lav_Cod = " & lav_cod_chiamante & ") "
        End If

        If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
            xFiltroAggiuntivo &= " AND (Agenda.Lav_Cod = " & LAVCOD_TRASFERIMENTO & ") "
        End If

        Dim xOrderBy As String = ""
        If orderBy <> "" Then
            xOrderBy &= orderBy & ","
        End If
        xOrderBy &= " Movimenti_Dettagli.ordine_det, Movimenti_Dettagli.Data_Creazione "

        ' Moduli attivi
        Dim leggi_anagrafe_log As New OGenerazioni_Anagrafe_Moduli_Log_R
        Dim dtAnagrafeLog = leggi_anagrafe_log.Leggi(Piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                               "", "",
                               objParametri)
        Dim listModuliAttivi_anagrafe_log As New List(Of enum_Omni_Modulo_Generazione)
        For Each r In dtAnagrafeLog.Rows
            listModuliAttivi_anagrafe_log.Add(CInt(r.Item("Modulo_Generazione")))
        Next
        dtAnagrafeLog = Nothing
        Dim isFreshAndFoodOrZooOrTabacco = False
        If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
            listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Zoo) OrElse
            listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Tabacco) Then
            isFreshAndFoodOrZooOrTabacco = True
        End If

        ' Gestione imballaggi
        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim dtParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)
        Dim gestitoImballaggio As Boolean = False
        Dim gestitoContenitore As Boolean = False
        Dim gestitoConfezione As Boolean = False

        ' Per i prodotti gestiti con DDT di acquisto non è gestita la griglia di imballaggi / contenitori / confezioni
        If isFreshAndFoodOrZooOrTabacco AndAlso
            Not lavCodDDTAcquisto.Contains(lav_cod_chiamante) Then
            For Each paramQual In dtParamQual.Rows
                If paramQual(Tabella_Key) = pq_imballaggio Then
                    gestitoImballaggio = True
                End If
                If paramQual(Tabella_Key) = pq_contenitore Then
                    gestitoContenitore = True
                End If
                If paramQual(Tabella_Key) = pq_confezione Then
                    gestitoConfezione = True
                End If
            Next
        End If
        'Stato totale evasione ordine
        Dim numRigheOrdineProdotto As Integer = 0
        Dim dictRigheOrdine As New Dictionary(Of enum_StatoOrdine, Integer) From {
            {enum_StatoOrdine.INDEFINITO, 0},
            {enum_StatoOrdine.INEVASO, 0},
            {enum_StatoOrdine.EVASO, 0},
            {enum_StatoOrdine.PARZIALMENTE_EVASO, 0},
            {enum_StatoOrdine.NON_PRONTO, 0},
            {enum_StatoOrdine.EVASO_FORZATAMENTE, 0}
        }

        Dim htParamQual As New Hashtable

        Dim dtUnitaMisura As DataTable = Nothing
        Dim hasUdmMultipliKg As Boolean

        Try

            Dim dataOraUltimaLettura As DateTime = DateTime.Now

            Dim w_elem_cod = 0

            'TODO  Modificare quando si potranno fare lavorazioni animali
            If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso
               listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) Then
                w_elem_cod = TRASFORMATI_VEGETALI
            End If

            '---------------  LETTURA DATI ------------------------------
            dtMovimentiDettagli = objMovDetR.LeggiCaricoScarico_New(Piva,
                                                                    0,
                                                                    Id_Agenda,
                                                                    0,
                                                                    idMovDet,
                                                                    w_elem_cod,
                                                                    0, 0, 0,
                                                                    0, 0, 0,
                                                                    Cau_Mov,
                                                                    0, 0, 0,
                                                                    0, "", 0,
                                                                    xFiltroAggiuntivo,
                                                                    xFiltroAggiuntivo,
                                                                    xOrderBy,
                                                                    objParametri,
                                                                    True,
                                                                    True,
                                                                    True,
                                                                    leggiProvvigioni,
                                                                    leggiPesiRiscontrati,
                                                                    leggiContiEconPatr,
                                                                    leggiOrdiniDaEvadere,
                                                                    leggiTrasferimenti,
                                                                    leggiLavorazioni,
                                                                    leggiConferimenti,
                                                                    leggiOrdini,
                                                                    leggiRifOrdineCliente,
                                                                    leggiDDTDaEvadere,
                                                                    leggiCdC,
                                                                    LeggiOrdiniLavoroDaEvadere,
                                                                    mostraOrdiniEvasi,
                                                                    gruppiMerce,
                                                                    leggiPratica)
            '---------------  LETTURA DATI ------------------------------

            If dtMovimentiDettagli.Rows.Count > 0 Then

                'Devo ricavare qual è il Sa_Cod del documento

                Dim nomeColonnaSaCod As String

                If lav_cod_chiamante = LAVCOD_CONTRATTO_AFFITTO Then

                    'Per i contratti di affitto non esiste la destinazione, per cui si prende il Sa_Cod del dettaglio

                    nomeColonnaSaCod = "Sa_Cod_Mov_Det"

                Else

                    'TODO: al momento non sono gestite righe senza destinazione, quando lo saranno sarà da sistemare
                    '      in base al valore di default che si imposterà

                    nomeColonnaSaCod = "Sa_Cod_Destinazione"

                End If

                Dim rigaSaCod = (From d In dtMovimentiDettagli.Rows
                                 Where d.Item(nomeColonnaSaCod) <> 0
                                 Order By d.Item(nomeColonnaSaCod) Descending
                                 Select d).FirstOrDefault()

                If rigaSaCod IsNot Nothing Then
                    SaCodDoc = CInt(rigaSaCod.Item(nomeColonnaSaCod))
                Else
                    SaCodDoc = 0
                End If

                DtDati.Columns.Add(New DataColumn("key_mov_dett", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Piva", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("key_Dest", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Ubic_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Elem_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
                DtDati.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
                DtDati.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))
                DtDati.Columns.Add(New DataColumn("Mov_Det_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Mat_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Mat_Cod_Alias", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Reg_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Linea_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Mat_Cod_OMNI", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Qta_Extra", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Udm_Cod_Extra", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("N", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("P205", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("K20", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Cu", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Cod_Regolamento", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Pendente", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Extra_Date", GetType(Date)))
                DtDati.Columns.Add(New DataColumn("Extra_Str", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Referenza", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Cod_Prodotto", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Lotto", GetType(String)))
                DtDati.Columns.Add(New DataColumn("DataOraUltimaLettura", GetType(Date)))
                DtDati.Columns.Add(New DataColumn("Tipo_Accettazione", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Modulo", GetType(Integer)))

                If mostraCampiTestata Then
                    DtDati.Columns.Add(New DataColumn("Des_Lib", GetType(String)))
                End If

                For Each paramQual In dtParamQual.Rows

                    ColumnsAddParamQual(DtDati, paramQual)

                Next

                'Note
                DtDati.Columns.Add(New DataColumn("FF_ONote_Descrizione", GetType(String)))

                DtDati.Columns.Add(New DataColumn("Fornitore", GetType(String)))
                If lavCodAccettazione.Contains(lav_cod_chiamante) AndAlso
                    isFreshAndFoodOrZooOrTabacco Then
                    DtDati.Columns.Add(New DataColumn("Degrado", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Degrado_Calcolato", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Quantita_Escluso_Degrado", GetType(Decimal)))
                End If

                DtDati.Columns.Add(New DataColumn("KgLordi", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("NrImballaggi", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("NrContenitori", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("NrConfezioni", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("KgNetti", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Tara", GetType(Decimal)))

                If leggiContiEconPatr Then
                    DtDati.Columns.Add(New DataColumn("Anno", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Cod_Conto_Economico", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Cod_Conto_Patrimoniale", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Descr_Conto_Economico", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Descr_Conto_Patrimoniale", GetType(String)))
                End If

                If leggiProvvigioni Then
                    DtDati.Columns.Add(New DataColumn("Provvigione", GetType(Decimal)))   'Da Mov_Dettaglio_Tecnico_extra
                End If

                DtDati.Columns.Add(New DataColumn("Sconto_Modalita", GetType(Integer))) ' Modalità sconto
                DtDati.Columns.Add(New DataColumn("Prezzo_Livello", GetType(Integer))) ' Livello Prezzo
                DtDati.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Prezzo_Unitario_Netto", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Aliquota_Iva_Des", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Iva", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Sconto", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("ScontoMaggiorazione", GetType(Integer))) ' Se Sconto <0 è Sconto, Altrimenti Maggiorazione
                DtDati.Columns.Add(New DataColumn("Sconto1", GetType(Decimal))) ' 7,6-6,5-5,4
                DtDati.Columns.Add(New DataColumn("Sconto2", GetType(Decimal))) ' 7,6-6,5-5,4
                DtDati.Columns.Add(New DataColumn("Sconto3", GetType(Decimal))) ' 7,6-6,5-5,4
                DtDati.Columns.Add(New DataColumn("Sconto_Calcolato", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Prezzo_Effettivo", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Imponibile", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Imponibile_Netto", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("Importo_Unitario", GetType(Decimal)))  ' Da Calcolare a programma
                DtDati.Columns.Add(New DataColumn("Importo_Totale", GetType(Decimal)))       ' Da Calcolare a programma
                DtDati.Columns.Add(New DataColumn("TempoCarenza", GetType(Integer)))       ' Determina su cosa è il prezzo
                DtDati.Columns.Add(New DataColumn("ChkIva_Manuale", GetType(Boolean)))       ' Determina se l'Iva è stata forzata manualmente
                DtDati.Columns.Add(New DataColumn("Listino_Cod", GetType(Integer)))
                ' Non serve perché nella pagina viene utilizzata la scomposizione  DtDati.Columns.Add(New DataColumn("Sconto_Listino", GetType(Decimal)))
                ' Non dovrebbe servire più DtDati.Columns.Add(New DataColumn("Iva_Deto_Cod", GetType(Integer)))       ' Aliquota per spesometro

                If leggiPesiRiscontrati Then
                    DtDati.Columns.Add(New DataColumn("Tara_Totale_Riscontrata", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Num_Conf_Riscontrate", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Num_Colli_Riscontrati", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Num_Imballi_Riscontrati", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Peso_Netto_Riscontrato", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Peso_Lordo_Riscontrato", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Conf_Riscontrata", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Collo_Riscontrata", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Imballo_Riscontrata", GetType(Decimal)))
                End If

                If leggiOrdini OrElse leggiRifOrdineCliente Then
                    DtDati.Columns.Add(New DataColumn("N_Doc_Cliente", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Data_Doc_Cliente", GetType(Date)))

                    DtDati.Columns.Add(New DataColumn("N_Nota_DDT", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("N_Nota_Riga_DDT", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Data_Nota_DDT", GetType(Date)))
                End If

                If leggiOrdini OrElse leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere OrElse LeggiOrdiniLavoroDaEvadere Then
                    DtDati.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Qta_Richiesta", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Qta_Evasa", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Qta_Residua", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("StatoEvasione_Cod", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("StatoEvasione_Des", GetType(String)))
                End If

                If leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then
                    DtDati.Columns.Add(New DataColumn("Numero_Movimento_Registrazione", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Data_Movimento_Registrazione", GetType(Date)))
                End If

                If leggiTrasferimenti Then
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Cau_Mov", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Id_Mov", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Id_Mov_Det", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Cau_Mov_Rif", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Id_Mov_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Id_Mov_Det_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Rif_Qta", GetType(Decimal)))

                    DtDati.Columns.Add(New DataColumn("Trasf_Tipo_Destinazione_2", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Sa_Cod_2", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Id_Destinazione_2", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Trasf_Ubic_Des_2", GetType(String)))
                End If

                If leggiLavorazioni Then
                    DtDati.Columns.Add(New DataColumn("Extra_Int_10001", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Chiusa", GetType(String)))
                End If

                If leggiConferimenti Then
                    DtDati.Columns.Add(New DataColumn("Id_Reg_Dettaglio_Conferimento", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("TagliandoPesa", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("PremioComplessivo", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("CodVarietaConferimento", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("DescAppezzamenti", GetType(String)))
                End If

                If leggiCdC Then
                    DtDati.Columns.Add(New DataColumn("CdC_Des_Imputazione", GetType(String)))
                End If

                DtDati.Columns.Add(New DataColumn("Lotto_Interno", GetType(String)))   'TODO Va preso in join con Imprese_Progetti
                DtDati.Columns.Add(New DataColumn("Progetto_Des", GetType(String)))    'TODO Va preso in join con Imprese_Progetti
                DtDati.Columns.Add(New DataColumn("Ordine_Det", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Fase_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("Rif_Esterno", GetType(String)))
                DtDati.Columns.Add(New DataColumn("Rif_Esterno_2", GetType(String)))

                'Per riferimenti
                If leggiRiferimento Then
                    DtDati.Columns.Add(New DataColumn("Piva_Rif", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Sa_Cod_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Id_Agenda_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Id_Mov_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Id_Mov_Det_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Lav_Cod_Rif", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Cau_Mov_Rif", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Qta_Rif", GetType(Decimal)))
                    DtDati.Columns.Add(New DataColumn("Ordine_Num_Concat", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("List_Riferimenti", GetType(String)))
                End If

                If gruppiMerce Then
                    DtDati.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Gruppo_Merce_Desc", GetType(String)))
                End If

                If leggiPratica Then
                    DtDati.Columns.Add(New DataColumn("Pratica_Cod", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Pratica_Servizio_Cod", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Pratica_Stato_Cod", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Pratica_Stato_Des", GetType(String)))
                    DtDati.Columns.Add(New DataColumn("Pratica_Stato_Note", GetType(String)))
                End If

                Dim objContabHlp As New AgronicaCoreContabHLP.Contabilita

                For Each drMovDet As DataRow In dtMovimentiDettagli.Rows

                    '--------------------------------------------------------------------------------
                    'Lo scopo era quello di escludere il movimento legato alla testata ODL.
                    'Tentando di togliere la prima riga, in alcuni casi con dei filtri
                    'aggiuntivi questa tecnica non era corretta.
                    'Sostituito integrando nuove condizioni nel filtro dove viene già venivia
                    'applicato Jolly_Int = 1.
                    '--------------------------------------------------------------------------------
                    'If lav_cod_chiamante = LAVCOD_TESTATE_ORDINE_LAVORAZIONE AndAlso Not movimentiDaGiacenza Then
                    '    Dim idMovDetCorrente = drMovDet.Item("Dett_Id_Mov_Det")
                    '    Dim minIdMovDet = Convert.ToInt32(dtMovimentiDettagli.Compute("min(Dett_Id_Mov_Det)", "Id_Agenda = " & drMovDet.Item("Id_Agenda")))
                    '    If minIdMovDet = idMovDetCorrente Then
                    '        Continue For
                    '    End If
                    'End If
                    '--------------------------------------------------------------------------------

                    Dim dr As DataRow

                    'Creo una nuova riga
                    dr = DtDati.NewRow

                    If mostraCampiTestata Then
                        dr.Item("Des_Lib") = drMovDet.Item("Des_Lib")
                    End If
                    dr.Item("Piva") = drMovDet.Item("Piva")
                    dr.Item("Sa_Cod") = drMovDet.Item("Sa_Cod_Destinazione")
                    dr.Item("Sa_Nome") = New CentriAziendali_Read().SaNome_from_SaCod(drMovDet.Item("Piva"), drMovDet.Item("Sa_Cod"), objParametri)
                    dr.Item("Id_Agenda") = drMovDet.Item("Id_Agenda")
                    dr.Item("Id_Mov") = drMovDet.Item("Id_Mov")
                    dr.Item("Id_Mov_Det") = drMovDet.Item("Dett_Id_Mov_Det")
                    dr.Item("Lav_Cod") = drMovDet.Item("Lav_Cod")
                    dr.Item("Cau_Mov") = drMovDet.Item("Cau_Mov")
                    dr.Item("Appezza") = drMovDet.Item("Appezza")
                    dr.Item("key_Dest") = drMovDet.Item("Tipo_Destinazione").ToString & "_" & drMovDet.Item("Sa_Cod_Destinazione").ToString & "_" & drMovDet.Item("Id_Destinazione").ToString
                    If Not String.IsNullOrEmpty(drMovDet.Item("Identificativo").ToString()) Then
                        ' dr.Item("insieme_des") = drMovDet.Item("Fabbricato_Des")
                        dr.Item("Ubic_Des") = drMovDet.Item("Identificativo")
                    Else
                        dr.Item("Ubic_Des") = drMovDet.Item("Fabbricato_Des")
                    End If
                    dr.Item("Cal_Cod") = If(Not IsDBNull(drMovDet.Item("Cal_Cod")), drMovDet.Item("Cal_Cod"), 0)
                    dr.Item("Cod_Progetto") = If(Not IsDBNull(drMovDet.Item("Cod_Progetto")), drMovDet.Item("Cod_Progetto"), 0)
                    dr.Item("Udm_Cod") = If(Not IsDBNull(drMovDet.Item("Udm_Cod")), drMovDet.Item("Udm_Cod"), 0)
                    dr.Item("Udm_Des") = ComponiUdmDes(drMovDet.Item("Udm_Des"), drMovDet.Item("Udm_Sim"))
                    dr.Item("Elem_Des") = drMovDet.Item("Elem_Des")
                    dr.Item("Qta_Extra") = If(Not IsDBNull(drMovDet.Item("MovDett_Qta_Extra")), drMovDet.Item("MovDett_Qta_Extra"), 0)
                    dr.Item("Udm_Cod_Extra") = If(Not IsDBNull(drMovDet.Item("MovDett_Udm_Cod_Extra")), drMovDet.Item("MovDett_Udm_Cod_Extra"), 0)
                    dr.Item("Data_Movimento") = drMovDet.Item("Data_Movimento")
                    dr.Item("DataOraUltimaLettura") = dataOraUltimaLettura
                    dr.Item("Data_Creazione") = drMovDet.Item("Data_Creazione3")
                    dr.Item("Data_Modifica") = drMovDet.Item("Data_Modifica")
                    dr.Item("Mov_Det_Des") = drMovDet.Item("Mov_Det_Des")

                    dr.Item("Cat_Cod") = If(Not IsDBNull(drMovDet.Item("Elem_Cod")), drMovDet.Item("Elem_Cod"), 0)
                    'dr.Item("Cat_Des") = If(Not IsDBNull(drMovDet.Item("NomeComune")), drMovDet.Item("NomeComune"), "")
                    dr.Item("Pro_Cod") = If(Not IsDBNull(drMovDet.Item("Pro_Cod")), drMovDet.Item("Pro_Cod"), 0)
                    dr.Item("Mat_Cod") = If(Not IsDBNull(drMovDet.Item("Mat_Cod")), drMovDet.Item("Mat_Cod"), 0)
                    dr.Item("Mat_Cod_Alias") = If(Not IsDBNull(drMovDet.Item("Mat_Cod_Alias")), drMovDet.Item("Mat_Cod_Alias"), 0)
                    dr.Item("Veg_Cod") = If(Not IsDBNull(drMovDet.Item("Veg_Cod")), drMovDet.Item("Veg_Cod"), 0)
                    dr.Item("Cul_Cod") = If(Not IsDBNull(drMovDet.Item("Cul_Cod")), drMovDet.Item("Cul_Cod"), 0)
                    dr.Item("Reg_Cod") = If(Not IsDBNull(drMovDet.Item("Reg_Cod")), drMovDet.Item("Reg_Cod"), 0)
                    dr.Item("Linea_Cod") = If(Not IsDBNull(drMovDet.Item("MP_Linea_Cod")), drMovDet.Item("MP_Linea_Cod"), 0)
                    dr.Item("Mat_Cod_OMNI") = If(Not IsDBNull(drMovDet.Item("Mat_Cod_OMNI")), drMovDet.Item("Mat_Cod_OMNI"), 0)
                    dr.Item("Mat_Des") = drMovDet.Item("Descrizione_Prodotto")
                    dr.Item("N") = If(Not IsDBNull(drMovDet.Item("N")), drMovDet.Item("N"), 0)
                    dr.Item("P205") = If(Not IsDBNull(drMovDet.Item("P205")), drMovDet.Item("P205"), 0)
                    dr.Item("K20") = If(Not IsDBNull(drMovDet.Item("K20")), drMovDet.Item("K20"), 0)
                    dr.Item("Cu") = If(Not IsDBNull(drMovDet.Item("Cu")), drMovDet.Item("Cu"), 0)
                    dr.Item("Cod_Regolamento") = If(Not IsDBNull(drMovDet.Item("Cod_Regolamento")), drMovDet.Item("Cod_Regolamento"), 0)

                    dr.Item("Pendente") = drMovDet.Item("Pendente")
                    dr.Item("Extra_Str") = drMovDet.Item("MovDett_Extra_Str")
                    dr.Item("Extra_Date") = drMovDet.Item("Extra_Date")
                    If drMovDet.Item("Descrizione_Prodotto") <> "" Then
                        dr.Item("Referenza") = drMovDet.Item("Descrizione_Prodotto") & If(drMovDet.Item("Cod_Articolo") <> "", " (" & drMovDet.Item("Cod_Articolo") & ")", "")
                    Else
                        dr.Item("Referenza") = drMovDet.Item("Mov_Det_Des")
                    End If
                    If drMovDet.Item("Cod_Articolo") <> "" Then
                        dr.Item("Cod_Prodotto") = drMovDet.Item("Cod_Articolo")
                    Else
                        dr.Item("Cod_Prodotto") = drMovDet.Item("Pro_Cod") 'Mostro il Pro_Cod per i prodotti di banca_dati
                    End If
                    dr.Item("Lotto") = If(Not IsDBNull(drMovDet.Item("Lotto")), drMovDet.Item("Lotto"), "")
                    dr.Item("Tipo_Accettazione") = If(Not IsDBNull(drMovDet.Item("Tipo_Accettazione")), drMovDet.Item("Tipo_Accettazione"), 0)
                    dr.Item("Modulo") = If(Not IsDBNull(drMovDet.Item("Modulo")), drMovDet.Item("Modulo"), 0)

                    For Each paramQual In dtParamQual.Rows

                        CaricaParamQualCaricoScarico(paramQual,
                                                     drMovDet,
                                                     dr,
                                                     htParamQual,
                                                     lavCodLavorazioni,
                                                     lav_cod_chiamante,
                                                     objParametri)

                    Next

                    'Note
                    dr.Item("FF_ONote_Descrizione") = drMovDet.Item("FF_ONote_Descrizione")

                    Dim kgNetti As Decimal = 0
                    Dim taraTot As Decimal = 0.0

                    If isFreshAndFoodOrZooOrTabacco AndAlso
                       (dr.Item("Cat_Cod") = TRASFORMATI_VEGETALI OrElse
                        dr.Item("Cat_Cod") = TRASFORMATI_ANIMALI) Then

                        'INIZIO Questa IF viene fatta perché per all'8/11/2017 il campo Qta_Extra_Totale nei movimenti di scarico (cau_mov = 7350) non veniva valorizzato
                        If drMovDet.Item("Qta_Extra_Totale") <> 0 Then
                            kgNetti = CDec(drMovDet.Item("Qta_Extra_Totale"))
                        Else
                            kgNetti = CDec(drMovDet.Item("Dett_Qta") * drMovDet.Item("Qta_Extra"))
                        End If
                        'FINE Questa IF viene fatta perché per all'8/11/2017 il campo Qta_Extra_Totale nei movimenti di scarico (cau_mov = 7350) non veniva valorizzato
                        dr.Item("KgNetti") = kgNetti
                        dr.Item("Qta") = CDec(drMovDet.Item("Dett_Qta"))
                        dr.Item("NrImballaggi") = drMovDet.Item("Dest_Qta_Dest2")
                        dr.Item("NrContenitori") = drMovDet.Item("Dest_Qta_Dest1")

                        If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero AndAlso kgNetti <> 0 Then
                            dr.Item("NrConfezioni") = drMovDet.Item("Dett_Qta")
                        Else
                            dr.Item("NrConfezioni") = 0
                        End If

                        If lavCodAccettazione.Contains(lav_cod_chiamante) AndAlso
                            isFreshAndFoodOrZooOrTabacco Then
                            If CDec(drMovDet.Item("Variazione")) <> 0 AndAlso
                                dr.Item("Udm_Cod") = enum_UnitaMisura.KG Then
                                dr.Item("Degrado") = drMovDet.Item("Variazione")
                                'Per ora il degrado è gestito senza decimali
                                'dr.Item("Degrado_Calcolato") = dr.Item("KgNetti") / 100 * dr.Item("Degrado")
                                dr.Item("Degrado_Calcolato") = CInt(kgNetti / 100 * dr.Item("Degrado"))
                                dr.Item("Quantita_Escluso_Degrado") = kgNetti - dr.Item("Degrado_Calcolato")
                            Else
                                dr.Item("Degrado") = 0
                                dr.Item("Degrado_Calcolato") = 0
                                dr.Item("Quantita_Escluso_Degrado") = dr.Item("KgNetti")
                            End If
                        End If

                        If impPesiRiscontrati = 1 AndAlso lavCodFattVendita.Contains(lav_cod_chiamante) AndAlso
                            (drMovDet.Item("Tara_Unit_Imballo_Riscontrata") IsNot DBNull.Value OrElse
                            drMovDet.Item("Tara_Unit_Collo_Riscontrata") IsNot DBNull.Value OrElse
                            drMovDet.Item("Tara_Unit_Conf_Riscontrata") IsNot DBNull.Value) Then

                            'Nel caso di fatture di vendita derivanti da ddt che avevano tare unitarie riscontrate,
                            'prendo la tara salvata nella movimenti dettagli perché i valori di tara_campionatura dei
                            'parametri qualitativi contengono i valori di tara unitaria reali del ddt. Il cal_cod non
                            'viene cambiato pertanto è lo stesso del ddt
                            taraTot = CDec(drMovDet.Item("Tara"))

                        Else

                            ' La tara viene calcolata come Nr Imballaggi / Nr Contenitori / Nr Confezioni * la rispettiva tara
                            ' Se questa è zero viene presa quella della movimenti_dettagli (caso in cui non vengono movimentati imballaggi sulla riga di entrata)
                            If dr.Table.Columns("FF_imballaggio_Tara_Campionatura") IsNot Nothing Then
                                taraTot += dr.Item("NrImballaggi") * dr.Item("FF_imballaggio_Tara_Campionatura")
                            End If
                            If dr.Table.Columns("FF_contenitore_Tara_Campionatura") IsNot Nothing Then
                                taraTot += dr.Item("NrContenitori") * dr.Item("FF_contenitore_Tara_Campionatura")
                            End If
                            If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero Then
                                If dr.Table.Columns("FF_confezione_Tara_Campionatura") IsNot Nothing Then
                                    taraTot += dr.Item("NrConfezioni") * dr.Item("FF_confezione_Tara_Campionatura")
                                End If
                            End If
                            If taraTot = 0 AndAlso CDec(drMovDet.Item("Tara")) <> 0 Then
                                taraTot = CDec(drMovDet.Item("Tara"))
                            End If

                        End If

                        dr.Item("Tara") = taraTot

                        dr.Item("KgLordi") = kgNetti + dr.Item("Tara")

                    Else

                        dr.Item("Tara") = CDec(drMovDet.Item("Tara"))

                        dr.Item("Qta") = CDec(drMovDet.Item("Dett_Qta"))
                        dr.Item("KgNetti") = CDec(drMovDet.Item("Dett_Qta"))
                        If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero Then
                            dr.Item("KgLordi") = CDec(drMovDet.Item("Qta_Extra_Totale"))
                        Else
                            dr.Item("KgLordi") = CDec(drMovDet.Item("Dett_Qta"))
                        End If

                        If isFreshAndFoodOrZooOrTabacco AndAlso
                            dr.Item("Cat_Cod") = BENI_CONFEZ_VEGETALE Then

                            dr.Item("NrImballaggi") = CDec(drMovDet.Item("Dett_Qta"))
                            dr.Item("KgNetti") = 0
                        Else
                            dr.Item("NrImballaggi") = 0
                        End If
                        dr.Item("NrContenitori") = 0
                        dr.Item("NrConfezioni") = 0
                    End If

                    If leggiContiEconPatr Then
                        dr.Item("Anno") = drMovDet.Item("Anno")
                        dr.Item("Cod_Conto_Economico") = drMovDet.Item("Cod_Conto")
                        dr.Item("Cod_Conto_Patrimoniale") = drMovDet.Item("Cod_Conto_Pat")
                        dr.Item("Descr_Conto_Economico") = drMovDet.Item("Descr_Conto_Economico")
                        dr.Item("Descr_Conto_Patrimoniale") = drMovDet.Item("Descr_Conto_Patrimoniale")
                    End If

                    If leggiProvvigioni Then
                        dr.Item("Provvigione") = drMovDet.Item("Provvigione")
                    End If

                    dr.Item("Sconto_Modalita") = drMovDet.Item("Sconto_Modalita")

                    dr.Item("Prezzo_Livello") = drMovDet.Item("Prezzo_Livello")

                    'Giulia 21/08/2020: bugfix perché non presentiamo più il default (0), ma i vecchi record potrebbero avercelo
                    If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) AndAlso
                       dr.Item("Cat_Cod") = TRASFORMATI_VEGETALI AndAlso
                       dr.Item("Prezzo_Livello") = 0 AndAlso kgNetti <> 0 Then

                        If drMovDet.Item("Udm_Cod") = enum_UnitaMisura.Numero Then
                            dr.Item("Prezzo_Livello") = enum_PrezzoLivello.Confezione
                        Else
                            dr.Item("Prezzo_Livello") = enum_PrezzoLivello.Kg_Litri
                        End If

                    End If

                    w_imponibile = objContabHlp.Leggi_Imponibile_PositivoNegativo(lav_cod_chiamante, CDec(drMovDet.Item("Imponibile")))
                    w_imponibileNetto = objContabHlp.Leggi_Imponibile_PositivoNegativo(lav_cod_chiamante, CDec(drMovDet.Item("Imponibile_Netto")))
                    w_iva = objContabHlp.Leggi_IVA_PositivaNegativa(lav_cod_chiamante, CDec(drMovDet.Item("Iva")))

                    dr.Item("Prezzo_Unitario") = drMovDet.Item("Prezzo_Unitario")
                    dr.Item("Prezzo_Unitario_Netto") = drMovDet.Item("Prezzo_Unitario_Netto")

                    'Gestione Conversione Udm
                    Dim UdmOrigine As Integer = dr.Item("Udm_Cod")

                    'Caso di movimentazione del prodotto in confezioni, l'unità di misura salvata in Udm_Cod è Numero, al posto di Kg che è salvata in Udm_Cod_Extra
                    Dim confezioneCodRiga As Integer = 0
                    If dr.Table.Columns("FF_confezione_Tipo_Cod") IsNot Nothing Then
                        confezioneCodRiga = CInt(dr.Item("FF_confezione_Tipo_Cod"))
                    End If
                    If UdmOrigine = enum_UnitaMisura.Numero AndAlso confezioneCodRiga > 1 Then
                        UdmOrigine = enum_UnitaMisura.KG
                    End If

                    SeConvertiQtaPrezzi(UdmOrigine, drMovDet, dtUnitaMisura, dr, objParametri)
                    hasUdmMultipliKg = hasUdmMultipliKg OrElse {enum_UnitaMisura.Quintali, enum_UnitaMisura.Tonnellate}.Contains(drMovDet.Item("MovDett_Extra_Int"))
                    '---

                    dr.Item("Cod_Iva") = drMovDet.Item("Cod_Iva")
                    dr.Item("Aliquota_Iva_Des") = drMovDet.Item("Aliquota_Iva_Des")
                    dr.Item("Iva") = w_iva
                    dr.Item("Sconto") = drMovDet.Item("Sconto")
                    If CDec(dr.Item("Sconto")) <= 0 Then
                        dr.Item("ScontoMaggiorazione") = 0
                    Else
                        dr.Item("ScontoMaggiorazione") = 1
                    End If
                    dr.Item("Sconto1") = 0D
                    dr.Item("Sconto2") = 0D
                    dr.Item("Sconto3") = 0D
                    If drMovDet.Item("Sconto_Testo") <> "" Then
                        Dim sconti As String() = drMovDet.Item("Sconto_Testo").Split("-")
                        If sconti.Length = 3 Then
                            dr.Item("Sconto3") = sconti(2)
                            dr.Item("Sconto2") = sconti(1)
                            dr.Item("Sconto1") = sconti(0)
                        End If
                        If sconti.Length = 2 Then
                            dr.Item("Sconto2") = sconti(1)
                            dr.Item("Sconto1") = sconti(0)
                        End If
                        If sconti.Length = 1 Then
                            dr.Item("Sconto1") = sconti(0)
                        End If
                    End If
                    dr.Item("Sconto_Calcolato") = Agro_Math.ArrotondaVal_6(AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
                        Math.Abs(CDec(dr.Item("Sconto"))),
                        CDec(dr.Item("Sconto1")),
                        CDec(dr.Item("Sconto2")),
                        CDec(dr.Item("Sconto3"))))

                    dr.Item("Prezzo_Effettivo") = drMovDet.Item("Prezzo_Effettivo")
                    dr.Item("Imponibile") = w_imponibile
                    dr.Item("Imponibile_Netto") = w_imponibileNetto

                    Dim pesoCalcoloImportoUnit As Decimal = 0

                    If lavCodAccettazione.Contains(lav_cod_chiamante) AndAlso
                            isFreshAndFoodOrZooOrTabacco AndAlso
                            (dr.Item("Cat_Cod") = TRASFORMATI_VEGETALI OrElse
                            dr.Item("Cat_Cod") = TRASFORMATI_ANIMALI) Then

                        pesoCalcoloImportoUnit = dr.Item("Quantita_Escluso_Degrado")
                        pesoCalcoloImportoUnit = kgNetti - dr.Item("Degrado_Calcolato")
                    Else
                        pesoCalcoloImportoUnit = dr.Item("Qta")
                    End If

                    If pesoCalcoloImportoUnit <> 0D Then
                        dr.Item("Importo_Unitario") = Agro_Math.ArrotondaVal_6((w_imponibileNetto + Agro_Math.ArrotondaVal_2(w_iva)) / pesoCalcoloImportoUnit)
                    Else
                        dr.Item("Importo_Unitario") = 0D
                    End If

                    dr.Item("Importo_Totale") = w_imponibileNetto + w_iva

                    dr.Item("TempoCarenza") = drMovDet.Item("TempoCarenza")
                    dr.Item("ChkIva_Manuale") = CBool(drMovDet.Item("ChkIva_Manuale"))
                    dr.Item("Listino_Cod") = drMovDet.Item("Listino_Cod")
                    ' Non dovrebbe servire più  dr.Item("Iva_Deto_Cod") = drMovDet.Item("Iva_Deto_Cod")

                    If leggiPesiRiscontrati Then
                        dr.Item("Tara_Unit_Conf_Riscontrata") = drMovDet.Item("Tara_Unit_Conf_Riscontrata")
                        dr.Item("Tara_Unit_Collo_Riscontrata") = drMovDet.Item("Tara_Unit_Collo_Riscontrata")
                        dr.Item("Tara_Unit_Imballo_Riscontrata") = drMovDet.Item("Tara_Unit_Imballo_Riscontrata")
                        dr.Item("Num_Conf_Riscontrate") = drMovDet.Item("Num_Conf_Riscontrate")
                        dr.Item("Num_Colli_Riscontrati") = drMovDet.Item("Num_Colli_Riscontrati")
                        dr.Item("Num_Imballi_Riscontrati") = drMovDet.Item("Num_Imballi_Riscontrati")
                        dr.Item("Peso_Netto_Riscontrato") = drMovDet.Item("Peso_Netto_Riscontrato")
                        dr.Item("Peso_Lordo_Riscontrato") = drMovDet.Item("Peso_Lordo_Riscontrato")
                        'dr.Item("Tara_Totale_Riscontrata") = dr.Item("Tara_Unit_Conf_Riscontrata") * dr.Item("Num_Conf_Riscontrate") + dr.Item("Tara_Unit_Collo_Riscontrata") * dr.Item("Num_Colli_Riscontrati") + dr.Item("Tara_Unit_Imballo_Riscontrata") * dr.Item("Num_Imballi_Riscontrati")
                        If drMovDet.Item("Peso_Lordo_Riscontrato") > 0 Then
                            dr.Item("Tara_Totale_Riscontrata") = drMovDet.Item("Peso_Lordo_Riscontrato") - drMovDet.Item("Peso_Netto_Riscontrato")
                        Else
                            'Questo caso si può verificare se l'utente ha indicato una tara manualmente, ovvero nel caso in cui non erano gestiti imballi
                            dr.Item("Tara_Totale_Riscontrata") = 0
                        End If
                    End If

                    If leggiOrdini OrElse leggiRifOrdineCliente Then
                        dr.Item("N_Doc_Cliente") = drMovDet.Item("N_Doc_Cliente")
                        If IsDate(drMovDet.Item("Data_Doc_Cliente")) AndAlso CDate(drMovDet.Item("Data_Doc_Cliente")) <> AGRODATAINIZIO Then
                            dr.Item("Data_Doc_Cliente") = drMovDet.Item("Data_Doc_Cliente")
                        End If

                        dr.Item("N_Nota_DDT") = drMovDet.Item("N_Nota_DDT")
                        dr.Item("N_Nota_Riga_DDT") = drMovDet.Item("N_Nota_Riga_DDT")
                        If IsDate(drMovDet.Item("Data_Nota_DDT")) AndAlso CDate(drMovDet.Item("Data_Nota_DDT")) <> AGRODATAINIZIO Then
                            dr.Item("Data_Nota_DDT") = drMovDet.Item("Data_Nota_DDT")
                        End If
                    End If

                    If leggiOrdini OrElse leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere OrElse LeggiOrdiniLavoroDaEvadere Then
                        dr.Item("Contabilizzato") = drMovDet.Item("Contabilizzato")

                        Dim qtaRichiesta As Decimal = If(IsDBNull(drMovDet.Item("Qta_Richiesta")), 0, CDec(drMovDet.Item("Qta_Richiesta")))
                        Dim qtaEvasa As Decimal = If(IsDBNull(drMovDet.Item("Qta_Evasa")), 0, CDec(drMovDet.Item("Qta_Evasa")))
                        Dim qtaResidua As Decimal = If(IsDBNull(drMovDet.Item("Qta_Residua")), 0, CDec(drMovDet.Item("Qta_Residua")))

                        'Gestione Conversione Udm
                        If qtaRichiesta <> 0 OrElse qtaEvasa <> 0 OrElse qtaResidua <> 0 Then
                            SeConvertiQtaEvasione(UdmOrigine, drMovDet, dr, qtaRichiesta, qtaEvasa, qtaResidua)
                        End If
                        '---

                        Dim qta = dr.Item("Qta") 'In caso di fertilizzante, questa qtà è già stata convertita

                        dr.Item("Qta_Richiesta") = If(IsDBNull(drMovDet.Item("Qta_Richiesta")), qta, qtaRichiesta)
                        dr.Item("Qta_Evasa") = qtaEvasa
                        dr.Item("Qta_Residua") = If(IsDBNull(drMovDet.Item("Qta_Residua")), qta, qtaResidua)

                        Dim statoEvasioneRiga As enum_StatoOrdine = GetStatoOrdineDettaglio_Cod(dr.Item("Cat_Cod"),
                                                                                                dr.Item("Contabilizzato"),
                                                                                                dr.Item("Qta_Richiesta"),
                                                                                                dr.Item("Qta_Evasa"))
                        dr.Item("StatoEvasione_Cod") = statoEvasioneRiga
                        dr.Item("StatoEvasione_Des") = GetStatoOrdine_Des(statoEvasioneRiga)

                        If CInt(dr.Item("Cat_Cod")) <> RIGA_DESCRIZIONE_LIBERA Then
                            dictRigheOrdine(statoEvasioneRiga) += 1
                            numRigheOrdineProdotto += 1
                        End If

                    End If

                    If leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then
                        dr.Item("Numero_Movimento_Registrazione") = drMovDet.Item("Mov_Registrazione_Numero")
                        dr.Item("Data_Movimento_Registrazione") = drMovDet.Item("Mov_Registrazione_Data")
                    End If

                    If leggiTrasferimenti Then
                        dr.Item("Trasf_Rif_Cau_Mov") = drMovDet.Item("Trasf_Rif_Cau_Mov")
                        dr.Item("Trasf_Rif_Id_Mov") = drMovDet.Item("Trasf_Rif_Id_Mov")
                        dr.Item("Trasf_Rif_Id_Mov_Det") = drMovDet.Item("Trasf_Rif_Id_Mov_Det")
                        dr.Item("Trasf_Rif_Cau_Mov_Rif") = drMovDet.Item("Trasf_Rif_Cau_Mov_Rif")
                        dr.Item("Trasf_Rif_Id_Mov_Rif") = drMovDet.Item("Trasf_Rif_Id_Mov_Rif")
                        dr.Item("Trasf_Rif_Id_Mov_Det_Rif") = drMovDet.Item("Trasf_Rif_Id_Mov_Det_Rif")
                        dr.Item("Trasf_Rif_Qta") = drMovDet.Item("Trasf_Rif_Qta")

                        dr.Item("Trasf_Tipo_Destinazione_2") = drMovDet.Item("Trasf_Tipo_Destinazione_2")
                        dr.Item("Trasf_Sa_Cod_2") = drMovDet.Item("Trasf_Sa_Cod_2")
                        dr.Item("Trasf_Id_Destinazione_2") = drMovDet.Item("Trasf_Id_Destinazione_2")

                        If Not String.IsNullOrEmpty(drMovDet.Item("Trasf_Identificativo_2").ToString()) Then
                            dr.Item("Trasf_Ubic_Des_2") = drMovDet.Item("Trasf_Identificativo_2")
                        Else
                            dr.Item("Trasf_Ubic_Des_2") = drMovDet.Item("Trasf_Fabbricato_Des_2")
                        End If
                    End If

                    If leggiConferimenti Then
                        dr.Item("Id_Reg_Dettaglio_Conferimento") = drMovDet.Item("Id_Reg_Dettaglio_Conferimento")
                        dr.Item("TagliandoPesa") = drMovDet.Item("Tagliando_Pesa")
                        dr.Item("PremioComplessivo") = drMovDet.Item("Premio_Complessivo")
                        dr.Item("CodVarietaConferimento") = drMovDet.Item("Cod_Varieta_Conferimento")
                        dr.Item("DescAppezzamenti") = drMovDet.Item("Desc_Appezzamenti_Conferimento")
                    End If

                    'Leggo anche i riferimenti (solo se sono arrivata con una Id_Agenda precisa in input)
                    If leggiRiferimento Then

                        Dim objRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                        Dim dtRif As DataTable = objRifR.Leggi_Specifica(dr.Item("Piva"), 0, dr.Item("Id_Agenda"), 0, dr.Item("Id_Mov_Det"), 0, "",
                                                            "", 0, 0, 0, 0, 0, "",
                                                            "", "", objParametri)

                        If dtRif IsNot Nothing AndAlso dtRif.Rows.Count > 0 Then

                            Dim ordineDocNumCompleto As String = ""

                            Dim listObjRifLocali As New List(Of Object)
                            For Each rifRow As DataRow In dtRif.Rows

                                'TODO: non posso usare il reale Movimento_Dettaglio_Riferimento per riferimento circolare,
                                'quindi uso un oggetto anonimo
                                Dim objRif As Object
                                objRif = New With {
                                    .Piva = rifRow.Item("Piva"),
                                    .Sa_Cod = rifRow.Item("Sa_Cod"),
                                    .Id_Agenda = rifRow.Item("Id_Agenda"),
                                    .Id_Mov = rifRow.Item("Id_Mov"),
                                    .Id_Mov_Det = rifRow.Item("Id_Mov_Det"),
                                    .Lav_Cod = rifRow.Item("Lav_Cod"),
                                    .Cau_Mov = rifRow.Item("Cau_Mov"),
                                    .Piva_Rif = rifRow.Item("Piva_Rif"),
                                    .Sa_Cod_Rif = rifRow.Item("Sa_Cod_Rif"),
                                    .Id_Agenda_Rif = rifRow.Item("Id_Agenda_Rif"),
                                    .Id_Mov_Rif = rifRow.Item("Id_Mov_Rif"),
                                    .Id_Mov_Det_Rif = rifRow.Item("Id_Mov_Det_Rif"),
                                    .Lav_Cod_Rif = rifRow.Item("Lav_Cod_Rif"),
                                    .Cau_Mov_Rif = rifRow.Item("Cau_Mov_Rif"),
                                    .Qta = rifRow.Item("Qta"),
                                    .Preserva_Legame = rifRow("Preserva_Legame"),
                                    .Tipo_Associazione = rifRow("Tipo_Associazione")
                                }

                                listObjRifLocali.Add(objRif)

                                Dim lavCodRif As Integer = rifRow.Item("Lav_Cod_Rif")

                                If lavCodOrdineAcquisto.Contains(lavCodRif) OrElse
                                   lavCodOrdineVendita.Contains(lavCodRif) Then

                                    'Nello specifico per gli ordini, devo ricavare il doc_numero
                                    Dim handleMov As New AgronicaCoreContabDAL.Movimenti_R()
                                    Dim dtMovOrdine As DataTable = handleMov.Leggi(
                                                    rifRow.Item("Piva_Rif"),
                                                    0,
                                                    rifRow.Item("Id_Agenda_Rif"),
                                                    0,
                                                    0,
                                                    CAU_REGISTRAZIONI,
                                                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                    "", "", objParametri)

                                    If dtMovOrdine.Rows.Count > 0 Then
                                        ordineDocNumCompleto = String.Format("{0}{1:00000}{2}",
                                                                             dtMovOrdine.Rows(0)("Doc_Numero_Sin"),
                                                                             dtMovOrdine.Rows(0)("Doc_Numero"),
                                                                             dtMovOrdine.Rows(0)("Doc_Numero_Des"))
                                    End If

                                End If

                            Next

                            'TODO: questi sono i riferimenti della prima riga, ma potrei averne più di una quindi questi non sarebbero mai da usare!!!

                            dr.Item("Piva_Rif") = dtRif.Rows(0).Item("Piva_Rif")
                            dr.Item("Sa_Cod_Rif") = CInt(dtRif.Rows(0).Item("Sa_Cod_Rif"))
                            dr.Item("Id_Agenda_Rif") = CInt(dtRif.Rows(0).Item("Id_Agenda_Rif"))
                            dr.Item("Id_Mov_Rif") = CInt(dtRif.Rows(0).Item("Id_Mov_Rif"))
                            dr.Item("Id_Mov_Det_Rif") = CInt(dtRif.Rows(0).Item("Id_Mov_Det_Rif"))
                            dr.Item("Lav_Cod_Rif") = CInt(dtRif.Rows(0).Item("Lav_Cod_Rif"))
                            dr.Item("Cau_Mov_Rif") = dtRif.Rows(0).Item("Cau_Mov_Rif")
                            dr.Item("Qta_Rif") = CDec(dtRif.Rows(0).Item("Qta"))

                            dr.Item("Ordine_Num_Concat") = ordineDocNumCompleto
                            dr.Item("List_Riferimenti") = Newtonsoft.Json.JsonConvert.SerializeObject(listObjRifLocali)
                        Else
                            dr.Item("Piva_Rif") = ""
                            dr.Item("Sa_Cod_Rif") = 0
                            dr.Item("Id_Agenda_Rif") = 0
                            dr.Item("Id_Mov_Rif") = 0
                            dr.Item("Id_Mov_Det_Rif") = 0
                            dr.Item("Lav_Cod_Rif") = 0
                            dr.Item("Cau_Mov_Rif") = ""
                            dr.Item("Qta_Rif") = 0D
                            dr.Item("Ordine_Num_Concat") = ""
                            dr.Item("List_Riferimenti") = "[]"
                        End If

                    End If

                    'TODO Inizio
                    dr.Item("Lotto_Interno") = ""
                    dr.Item("Progetto_Des") = ""
                    'TODO Fine

                    dr.Item("Ordine_Det") = drMovDet.Item("Ordine_Det")
                    dr.Item("Fase_Cod") = drMovDet.Item("Fase_Cod")
                    dr.Item("Rif_Esterno") = drMovDet.Item("Rif_Esterno")
                    dr.Item("Rif_Esterno_2") = drMovDet.Item("Rif_Esterno_2")

                    If leggiLavorazioni Then
                        dr.Item("Extra_Int_10001") = drMovDet.Item("Extra_Int_10001")
                        dr.Item("Chiusa") = If(drMovDet.Item("Extra_Int_10001") = 1, Gias.Si, Gias.No)
                    End If

                    dr.Item("key_mov_dett") = dr.Item("piva") & "_" &
                                              dr.Item("sa_cod") & "_" &
                                              dr.Item("Id_Agenda") & "_" &
                                              dr.Item("Id_Mov") & "_" &
                                              dr.Item("Id_Mov_Det") & "_" &
                                              dr.Item("Appezza") & "_" &
                                              dr.Item("key_Dest")

                    '25/03/2021: Manipolo la riga per la nuova gestione udm (lasciare come ultima cosa)
                    If isFreshAndFoodOrZooOrTabacco AndAlso
                       (dr.Item("Cat_Cod") = TRASFORMATI_VEGETALI OrElse dr.Item("Cat_Cod") = TRASFORMATI_ANIMALI) AndAlso
                       (leggiTrasferimenti OrElse lavCodLavorazioni.Contains(lav_cod_chiamante) OrElse lav_cod_chiamante = LAVCOD_TESTATE_ORDINE_LAVORAZIONE) Then

                        Dim udmCodRiga As Integer = CInt(dr.Item("udm_cod"))

                        If udmCodRiga = enum_UnitaMisura.Numero Then

                            If confezioneCodRiga = 0 Then
                                'Ho movimentato volutamente a nr ==> nr confezioni non mi interessa
                                dr.Item("NrConfezioni") = 0
                            Else

                                'Ho movimentato a KG, ma avevo scelto la confezione, quindi su db ora risulta movimentato a Nr
                                'quindi cambio la Qta e l'udm perché siano coerenti

                                If CDec(dr.Item("KgNetti")) <> 0 Then
                                    dr.Item("Qta") = dr.Item("KgNetti")
                                    dr.Item("Udm_Cod") = CInt(enum_UnitaMisura.KG)
                                    dr.Item("Udm_Des") = Gias.Chilogrammi & " (kg)"
                                End If

                            End If

                        End If

                    End If

                    If leggiCdC Then
                        dr.Item("CdC_Des_Imputazione") = drMovDet.Item("CdC_Des_Imputazione")
                    End If

                    If gruppiMerce Then
                        dr.Item("Id_Gruppo_Merce") = drMovDet.Item("Id_Gruppo_Merce")
                        dr.Item("Gruppo_Merce_Desc") = drMovDet.Item("Gruppo_Merce_Desc")
                    End If

                    If leggiPratica Then
                        dr.Item("Pratica_Cod") = drMovDet.Item("Pratica_Cod")
                        dr.Item("Pratica_Servizio_Cod") = drMovDet.Item("Pratica_Servizio_Cod")
                        dr.Item("Pratica_Stato_Cod") = drMovDet.Item("Pratica_Stato_Cod")
                        dr.Item("Pratica_Stato_Des") = drMovDet.Item("Pratica_Stato_Des")
                        dr.Item("Pratica_Stato_Note") = drMovDet.Item("Pratica_Stato_Note")
                    End If

                    DtDati.Rows.Add(dr)

                Next

                'Se ordine, ricavo lo stato evasione totale del documento (lo faccio qui per evitare di dover rileggere di nuovo)
                If leggiOrdini OrElse ordiniDaEvadere Then
                    statoOrdineDoc = GetStatoOrdineDocumento_Cod(numRigheOrdineProdotto,
                                                                 dictRigheOrdine(enum_StatoOrdine.NON_PRONTO),
                                                                 dictRigheOrdine(enum_StatoOrdine.EVASO),
                                                                 dictRigheOrdine(enum_StatoOrdine.EVASO_FORZATAMENTE),
                                                                 dictRigheOrdine(enum_StatoOrdine.INEVASO),
                                                                 dictRigheOrdine(enum_StatoOrdine.PARZIALMENTE_EVASO))
                Else
                    statoOrdineDoc = enum_StatoOrdine.INDEFINITO
                End If

            Else
                SaCodDoc = 0
                statoOrdineDoc = enum_StatoOrdine.INDEFINITO
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        ' In caso di CARICHI su Lavorazioni F&F cerco la testata da cui prendere i default
        Dim objDftLav As JToken = Nothing
        If Cau_Mov <> CAU_SCARICO AndAlso lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso Id_Agenda <> 0 Then
            Dim kRows As New JArray
            Dim objLav As New FF_LavorazioneBIZ
            Dim datiLav As String = objLav.Leggi_Lavorazioni(Piva, Id_Agenda, 0, "", "", Nothing, Nothing, "T", False, objParametri, lav_cod_chiamante)
            Dim k As JObject = JObject.Parse(datiLav)
            kRows = k("kendo_rows")
            If kRows.Count > 0 Then
                objDftLav = kRows(0)
            End If
        End If

        Dim sceltaDisplay = False
        If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
            sceltaDisplay = True
        End If

        '-------------------------------------------------------------------------------------------------------------------------------------------
        '----------    CREO LISTA COLONNE DA VISUALIZZARE
        '-------------------------------------------------------------------------------------------------------------------------------------------

        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' Chiave riga
        c = New ColonneNome("key_mov_dett", "key_mov_dett", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piva", "Piva", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Agenda", "Id_Agenda", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Mov", "Id_Mov", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Mov_Det", "Id_Mov_Det", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Lav_Cod", "Lav_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cau_Mov", "Cau_Mov", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Fase_Cod", "Fase_Cod", "number")
        c._hidden = True
        c._daDuplicare = False
        l.Add(c)

        c = New ColonneNome("Rif_Esterno", "Rif_Esterno", "string")
        c._hidden = True
        c._daDuplicare = False
        l.Add(c)

        c = New ColonneNome("Rif_Esterno_2", "Rif_Esterno_2", "string")
        c._hidden = True
        c._daDuplicare = False
        l.Add(c)

        If leggiLavorazioni Then
            c = New ColonneNome("Extra_Int_10001", "Extra_Int_10001", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Chiusa", OttieniNomeColonnaChiusa(lav_cod_chiamante), "string")
            If mostraCampiTestata Then
                c._Filtrabile = True
                c._Display = True
                c._Editabile = False
                c._width = "120px"
            Else
                c._hidden = True
            End If
            l.Add(c)
        End If

        'Utilizzato in ricerca lavorazioni e trasferimenti per righe dettaglio
        If mostraCampiTestata Then

            c = New ColonneNome("Des_Lib", Gias.Descrizione, "string")
            c._gruppoColonne = gruppoTestata
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._Editabile = False
            c._width = "200px"
            l.Add(c)

        End If

        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
            lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
            lavCodAccettazione.Contains(lav_cod_chiamante) OrElse
            lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
            lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
            lavCodNotaAccrVendita.Contains(lav_cod_chiamante) OrElse
            lavCodOrdineAcquisto.Contains(lav_cod_chiamante) OrElse
            lavCodOrdineVendita.Contains(lav_cod_chiamante) Then

            c = New ColonneNome("Ordine_Det", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrRiga, "number")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = False
            c._Filtrabile = True
            c._Display = False
            c._Editabile = False
            c._max = True
            c._width = "67px"
            l.Add(c)
        End If

        If movimentiDaGiacenza OrElse Not lavCodLavorazioni.Contains(lav_cod_chiamante) Then
            c = New ColonneNome("Referenza", OttieniNomeColonnaReferenza(lav_cod_chiamante), "string")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            c._Editabile = False
            c._Display = True
            c._width = "300px"
            If movimentiDaGiacenza AndAlso Not mostraCampiTestata Then
                c._locked = True
            End If
            c._RemoveHtmlEncode = True
            l.Add(c)
        End If


        c = New ColonneNome("Cod_Prodotto", My.Resources.AgronicaCoreContabBIZ.CodiceArticolo, "string")
        c._gruppoColonne = gruppoProdotto
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = contestoDocContabile
        c._width = "90px"
        l.Add(c)

        c = New ColonneNome("Lotto", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Lotto, "string")
        c._gruppoColonne = gruppoProdotto
        c._Filtrabile = True
        c._FiltrabileConCheck = True
        c._Display = True
        c._obbligatorio = True
        c._width = "150px"
        If movimentiDaGiacenza Then
            c._Editabile = False
            If Not mostraCampiTestata Then
                c._locked = True
            End If
        Else
            c._daDuplicare = True

            If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                'Leggo se la colonna dei lotti è editabile o meno
                Dim Editabile As Boolean = False
                Dim Obbligatorio As Boolean = True

                dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_EDIT_LOTTO, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
                If dtImpostazioni.Rows.Count > 0 AndAlso (dtImpostazioni(0)("Impostazione_Valore_1") = "1") Then

                    Editabile = True

                    'Se la colonna del Lotto è editabile la imposto anche come non obbligatoria così al momento del salvataggio potrà essere
                    'configurato il Lotto se rimane vuoto.
                    Obbligatorio = False
                End If

                c._Editabile = Editabile

                c._obbligatorio = Obbligatorio

                c._daDuplicare = False

            End If
            If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Lotto")) IsNot Nothing Then
                c._valueDefault = objDftLav("Lotto")
            End If
        End If
        l.Add(c)

        'TODO verificare se ok così o se lo devo poter cambiare
        c = New ColonneNome("Sa_Cod", "Sa_Cod", "number")
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Sa_Cod_AperturaLav")) IsNot Nothing Then
            c._valueDefault = objDftLav("Sa_Cod_AperturaLav")
        End If
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        'TODO verificare se ok così o se lo devo poter cambiare
        c = New ColonneNome("Sa_Nome", "Nome", "string")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Movimento", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_DataMovimento, "date")
        If mostraCampiTestata Then
            c._gruppoColonne = gruppoTestata
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._Editabile = False
            c._width = "104px"
            c._formatNr = "{0:dd/MM/yyyy}"
        Else
            'c._Editabile = False
            'c._Filtrabile = True
            'c._Display = True
            'c._formatNr = "{0:dd/MM/yyyy}"
            c._daDuplicare = True
            c._hidden = True
            'c._cssHeader = "breakWordDocCont"
        End If
        l.Add(c)

        c = New ColonneNome("Data_Creazione", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_DataCreazione, "date")
        c._gruppoColonne = ""
        c._Editabile = False
        c._Filtrabile = True
        c._FiltrabileConCheck = False
        c._Display = False
        c._width = "104px"
        c._formatNr = "{0:dd/MM/yyyy}"
        'c._cssHeader = "breakWordDocCont"
        l.Add(c)

        c = New ColonneNome("DataOraUltimaLettura", "DataOraUltimaLettura", "date")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Modifica", "Data U.M.", "date")
        'c._Editabile = False
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'c._formatNr = "{0:dd/MM/yyyy}"
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("key_Dest", "key_Dest", "string")
        c._gruppoColonne = ""
        If movimentiDaGiacenza Then
            c._hidden = True
        Else
            c._hidden = True
            c._daDuplicare = True
            c._Editabile = True
            c._obbligatorio = True
            If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("IdDestinazioneAperturaLav")) IsNot Nothing Then
                c._valueDefault = objDftLav("IdDestinazioneAperturaLav")
            End If
        End If
        l.Add(c)

        c = New ColonneNome("Ubic_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_MagOCella, "string")
        c._Display = contestoDocContabile
        c._width = "73px"
        If movimentiDaGiacenza OrElse mostraCampiTestata Then
            c._Editabile = False

            c._Filtrabile = True

            c._FiltrabileConCheck = True
            'c._cssHeader = "breakWordDocCont"
            'Else
            '    c._obbligatorio = True
            '    c._daDuplicare = True
            '    c._hidden = True
            '    c._Editabile = True
        End If
        l.Add(c)

        If leggiTrasferimenti Then
            c = New ColonneNome("Trasf_Ubic_Des_2", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_MagOCellaDest, "string")
            If mostraCampiTestata Then
                c._Display = True
                c._Filtrabile = True
                c._Editabile = False
                c._width = "85px"
            Else
                c._hidden = True
            End If
            l.Add(c)
        End If

        c = New ColonneNome("Cat_Cod", "Elem_Cod", "number")
        c._gruppoColonne = gruppoProdotto
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Cat_Cod")) IsNot Nothing Then
            c._valueDefault = objDftLav("Cat_Cod")
        End If
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        If Not lavCodLavorazioni.Contains(lav_cod_chiamante) Then

            c = New ColonneNome("Cat_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Categoria, "string")
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = False
                c._Filtrabile = True
                c._width = "73px"
                c._FiltrabileConCheck = True
            Else
                c._obbligatorio = True
                c._daDuplicare = True
                c._hidden = True

                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
            End If
            l.Add(c)

            c = New ColonneNome("N", "N", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("P205", "P205", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("K20", "K20", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Cu", "Cu", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Cod_Regolamento", "Cod_Regolamento", "number")
            c._hidden = True
            l.Add(c)

        End If

        c = New ColonneNome("Pro_Cod", "Pro_Cod", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod", "Veg_Cod", "number")
        c._daDuplicare = True
        c._hidden = True
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Veg_Cod")) IsNot Nothing Then
            c._valueDefault = objDftLav("Veg_Cod")
        End If
        l.Add(c)

        c = New ColonneNome("Cul_Cod", "Cul_Cod", "number")
        c._daDuplicare = True
        c._hidden = True
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Cul_Cod")) IsNot Nothing Then
            c._valueDefault = objDftLav("Cul_Cod")
        End If
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Linea_Cod", "Linea_Cod", "number")
        c._daDuplicare = True
        c._hidden = True
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("Linea_Cod_AperturaLav")) IsNot Nothing Then
            c._valueDefault = objDftLav("Linea_Cod_AperturaLav")
        End If
        l.Add(c)

        c = New ColonneNome("Mat_Cod_OMNI", "Mat_Cod_OMNI", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        'TODO U.M.
        c = New ColonneNome("Qta_Extra", "Qta_Extra", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        'TODO U.M.
        c = New ColonneNome("Udm_Cod_Extra", "Udm_Cod_Extra", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number")
        If movimentiDaGiacenza Then
            c._hidden = True
        Else
            c._Display = sceltaDisplay
            c._daDuplicare = True
            c._hidden = True
            c._obbligatorio = True
            If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If

            If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                If objDftLav IsNot Nothing AndAlso CStr(objDftLav("Mat_Cod")) IsNot Nothing Then
                    c._valueDefault = objDftLav("Mat_Cod")
                Else
                    c._valueDefault = 0
                End If

            End If
            'c._locked = True
        End If
        l.Add(c)

        c = New ColonneNome("Mat_Cod_Alias", "Mat_Cod_Alias", "number")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        If gestitoImballaggio Then
            c = New ColonneNome("NrImballaggi", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrImb, "number")
            c._gruppoColonne = gruppoQuantita
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "70px"
            c._stringaTotale = ""
            c._sum = True
            'c._cssHeader = "breakWordDocCont"
            If Not movimentiDaGiacenza AndAlso lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                If objDftLav IsNot Nothing AndAlso CStr(objDftLav("NrImballaggiAperturaLav")) IsNot Nothing Then
                    c._valueDefault = CInt(objDftLav("NrImballaggiAperturaLav"))
                End If
            End If
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("NrContenitori", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrCont, "number")

            c._gruppoColonne = gruppoQuantita
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "76px"
            c._stringaTotale = ""
            c._sum = True
            'c._cssHeader = "breakWordDocCont"
            If Not movimentiDaGiacenza AndAlso lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                If objDftLav IsNot Nothing AndAlso CStr(objDftLav("NrImballaggiAperturaLav")) IsNot Nothing AndAlso CStr(objDftLav("NrContenitoriAperturaLav")) IsNot Nothing Then
                    If CInt(objDftLav("NrImballaggiAperturaLav")) = 0 Then
                        c._valueDefault = CStr(CInt(objDftLav("NrContenitoriAperturaLav")))
                    Else
                        c._valueDefault = CStr(CInt(objDftLav("NrImballaggiAperturaLav")) * CInt(objDftLav("NrContenitoriAperturaLav")))
                    End If
                End If
            End If
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("NrConfezioni", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_NrConfez, "number")

            c._gruppoColonne = gruppoQuantita
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "90px"
            c._stringaTotale = ""
            c._sum = True
            'c._cssHeader = "breakWordDocCont"
            If Not movimentiDaGiacenza AndAlso lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                If objDftLav IsNot Nothing AndAlso CStr(objDftLav("NrContenitoriAperturaLav")) IsNot Nothing AndAlso CStr(objDftLav("NrConfezioniAperturaLav")) IsNot Nothing Then
                    c._valueDefault = CStr(CInt(objDftLav("NrContenitoriAperturaLav")) * CInt(objDftLav("NrConfezioniAperturaLav")))
                End If
            End If
            l.Add(c)
        End If

        'Tare
        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnImb, "number")
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = sceltaDisplay
            c._formatNr = "n1"
            c._width = "73px"
            'c._cssHeader = "breakWordDocCont"
            c._stringaTotale = ""
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._daDuplicare = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If

                If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("FF_imballaggio_Tara_Campionatura")) IsNot Nothing Then
                    c._valueDefault = objDftLav("FF_imballaggio_Tara_Campionatura")
                End If
            End If
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Codice_Generazione_Link", "FF_imballaggio_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Mat_Cod_Generazione_Link", "FF_imballaggio_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnCont, "number")
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = sceltaDisplay
            c._formatNr = "n3"
            c._width = "77px"
            'c._cssHeader = "breakWordDocCont"
            c._stringaTotale = ""
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._daDuplicare = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
                If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("FF_contenitore_Tara_Campionatura")) IsNot Nothing Then
                    c._valueDefault = objDftLav("FF_contenitore_Tara_Campionatura")
                End If
            End If
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Codice_Generazione_Link", "FF_contenitore_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Mat_Cod_Generazione_Link", "FF_contenitore_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Tara_Campionatura", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnConfez, "number")
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = sceltaDisplay
            c._formatNr = "n3"
            c._width = "90px"
            'c._cssHeader = "breakWordDocCont"
            c._stringaTotale = ""
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._daDuplicare = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
                If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso objDftLav IsNot Nothing AndAlso CStr(objDftLav("FF_confezione_Tara_Campionatura")) IsNot Nothing Then
                    c._valueDefault = objDftLav("FF_confezione_Tara_Campionatura")
                End If
            End If
            l.Add(c)

            c = New ColonneNome("FF_confezione_Codice_Generazione_Link", "FF_confezione_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_confezione_Mat_Cod_Generazione_Link", "FF_confezione_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            c._hidden = True
            l.Add(c)
        End If

        c = New ColonneNome("Qta", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Quantità, "number")
        If (lavCodTrasferimento.Contains(lav_cod_chiamante) OrElse
            lavCodLavorazioni.Contains(lav_cod_chiamante) OrElse
            lavCodAccettazione.Contains(lav_cod_chiamante)) AndAlso
           FF_gest_materiale_vivaistico Then
            c._hidden = True
            c._Editabile = True 'Non sono sicura che mi serva, ma meglio metterlo, sennò poi non si può modificare il valore programmaticamente
        Else
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = True
            'c._sum = True
            c._stringaTotale = ""
            c._obbligatorio = True
            'c._cssHeader = "breakWordDocCont"
            c._width = "95px"
            c._Editabile = True
            c._formatNr = "n2"
        End If
        l.Add(c)

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "string")
        c._gruppoColonne = gruppoQuantita
        c._daDuplicare = True
        c._hidden = True
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso Cau_Mov = CAU_CARICO Then
            c._Editabile = True
        Else
            c._Editabile = False
        End If
        If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso
           objDftLav IsNot Nothing AndAlso
           objDftLav("Udm_Cod") IsNot Nothing AndAlso
           objDftLav("Udm_Cod").HasValues Then

            'Bypass per nuova gestione udm, per cui se ho confezioni è salvato udm_cod = numero, ma deve apparire come kg
            If CInt(objDftLav("Udm_Cod")) = enum_UnitaMisura.Numero AndAlso
               ((CStr(objDftLav("NrConfezioniAperturaLav")) IsNot Nothing AndAlso
                 CInt(objDftLav("NrConfezioniAperturaLav")) > 0) OrElse
                (objDftLav("FF_confezione_Cod") IsNot Nothing AndAlso
                 CInt(objDftLav("FF_confezione_Cod")) <> 0)) Then
                c._valueDefault = CStr(enum_UnitaMisura.KG)
            Else
                c._valueDefault = CStr(objDftLav("Udm_Cod"))
            End If

        End If
        l.Add(c)

        c = New ColonneNome("Udm_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_UnitàMisura, "string")
        If (lavCodTrasferimento.Contains(lav_cod_chiamante) OrElse
            lavCodLavorazioni.Contains(lav_cod_chiamante) OrElse
            lavCodAccettazione.Contains(lav_cod_chiamante)) AndAlso
           FF_gest_materiale_vivaistico Then
            c._hidden = True
            c._Editabile = True 'Non sono sicura che mi serva, ma meglio metterlo, sennò poi non si può modificare il valore programmaticamente
        Else
            c._gruppoColonne = gruppoQuantita
            c._daDuplicare = True
            'c._Filtrabile = True
            'c._FiltrabileConCheck = True
            'c._width = "92px"

            If leggiTrasferimenti OrElse lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                c._Display = True
            Else
                c._Display = False
            End If
            If lavCodLavorazioni.Contains(lav_cod_chiamante) AndAlso Cau_Mov = CAU_CARICO Then

                c._obbligatorio = True
                c._hidden = True
                c._Editabile = True
                c._Display = False

            Else
                c._Editabile = False
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._width = "92px"
            End If
        End If
        l.Add(c)

        c = New ColonneNome("Elem_Des", My.Resources.AgronicaCoreContabBIZ.CategoriaProdotto, "string")
        c._width = "92px"
        c._Display = contestoDocContabile
        l.Add(c)

        If hasUdmMultipliKg Then
            c = New ColonneNome("KgLordi", Gias.PesoLordo, "number")
        Else
            c = New ColonneNome("KgLordi", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgLordi, "number")
        End If

        'TODO LAVORAZIONI / TRASFERIMENTI U.M.
        If FF_gest_materiale_vivaistico OrElse
           lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
           Not isFreshAndFoodOrZooOrTabacco Then
            c._hidden = True
            c._Editabile = True 'Non sono sicura che mi serva, ma meglio metterlo, sennò poi non si può modificare il valore programmaticamente
        Else
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = True
            c._sum = True
            c._stringaTotale = ""
            c._obbligatorio = True
            'c._cssHeader = "breakWordDocCont"
            c._width = "73px"
            c._Editabile = True
            c._formatNr = If(hasUdmMultipliKg, "n2", "n0")
        End If
        l.Add(c)

        c = New ColonneNome("Tara", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraTotale, "number")
        c._gruppoColonne = gruppoQuantita
        c._Filtrabile = True
        c._Display = True
        c._stringaTotale = ""
        c._sum = True
        'c._cssHeader = "breakWordDocCont"
        c._obbligatorio = False
        c._width = "77px"

        If lavCodTrasferimento.Contains(lav_cod_chiamante) OrElse
           lavCodLavorazioni.Contains(lav_cod_chiamante) Then
            c._Editabile = True
        Else
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
        End If

        c._formatNr = "n2"
        l.Add(c)

        If leggiPesiRiscontrati Then
            c = New ColonneNome("Tara_Totale_Riscontrata", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraRiscontrata, "number")
            c._gruppoColonne = gruppoPesoRiscontrato
            c._Filtrabile = True
            If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._Display = False
            Else
                c._hidden = True
            End If
            c._stringaTotale = ""
            c._sum = True
            'c._cssHeader = "breakWordDocCont"
            c._obbligatorio = False
            c._width = "75px"
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
            End If
            c._formatNr = "n2"
            l.Add(c)
        End If

        If FF_gest_materiale_vivaistico Then
            c = New ColonneNome("KgNetti", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Numero, "number")
        ElseIf hasUdmMultipliKg Then
            c = New ColonneNome("KgNetti", Gias.PesoNetto, "number")
        Else
            c = New ColonneNome("KgNetti", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgNetti, "number")
        End If
        If lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
           Not isFreshAndFoodOrZooOrTabacco Then
            c._hidden = True
        Else
            c._gruppoColonne = gruppoQuantita
            c._Filtrabile = True
            c._Display = True
            c._formatNr = If(hasUdmMultipliKg, "n2", "n0")
            c._sum = True
            c._stringaTotale = ""
            c._obbligatorio = True
            'c._cssHeader = "breakWordDocCont"
            If FF_gest_materiale_vivaistico Then
                c._width = "90px"
            Else
                c._width = "73px"
            End If
            If Not movimentiDaGiacenza AndAlso lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
        End If
        l.Add(c)

        If lavCodAccettazione.Contains(lav_cod_chiamante) AndAlso
           isFreshAndFoodOrZooOrTabacco Then
            c = New ColonneNome("Degrado", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_PercentualeDegrado, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoQuantita
                c._Filtrabile = True
                c._Display = True
                c._stringaTotale = ""
                c._obbligatorio = True
                'c._cssHeader = "breakWordDocCont"
                c._width = "85px"
                c._Editabile = True
                c._formatNr = "n2"
            End If
            l.Add(c)

            c = New ColonneNome("Degrado_Calcolato", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgDegrado, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoQuantita
                c._Filtrabile = True
                c._Display = True
                c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = True
                'c._cssHeader = "breakWordDocCont"
                c._width = "95px"
                c._Editabile = True
                'Per ora il degrado è gestito senza decimali
                c._formatNr = "n0"
            End If
            l.Add(c)

            c = New ColonneNome("Quantita_Escluso_Degrado", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_KgPagam, "number")
            If FF_gest_materiale_vivaistico Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoQuantita
                c._Filtrabile = True
                c._Display = True
                c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = True
                'c._cssHeader = "breakWordDocCont"
                c._width = "95px"
                c._Editabile = False
                'Per ora il degrado è gestito senza decimali
                c._formatNr = "n0"
            End If
            l.Add(c)
        End If

        c = New ColonneNome("Mat_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Prodotto, "string")
        c._gruppoColonne = gruppoProdotto
        If movimentiDaGiacenza OrElse mostraCampiTestata Then
            c._Editabile = False
            c._Display = mostraCampiTestata
            c._width = "100px"
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        Else
            c._daDuplicare = True
            c._hidden = True
            c._obbligatorio = True
            If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
        End If
        l.Add(c)

        If gruppiMerce Then
            c = New ColonneNome("Id_Gruppo_Merce", "Id Gruppo Merce", "number")
            c._gruppoColonne = gruppoProdotto
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Gruppo_Merce_Desc", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_GruppoMerce, "string")
            c._gruppoColonne = gruppoProdotto
            c._Editabile = False
            c._Display = False
            c._width = "310px"
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        'Caratteristiche prodotto
        For Each paramQual In dtParamQual.Rows

            AggiungiColonneParamQualCaricoScarico(
                l,
                paramQual,
                htParamQual,
                movimentiDaGiacenza,
                lavCodTrasferimento,
                lavCodLavorazioni,
                lav_cod_chiamante,
                objDftLav,
                mostraCampiTestata)

        Next

        c = New ColonneNome("Fornitore", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Fornitore, "string")
        c._gruppoColonne = gruppoProdotto
        If movimentiDaGiacenza AndAlso Not htParamQual.ContainsKey(pq_fornitore) Then
            c._hidden = True
        Else
            If movimentiDaGiacenza Then
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._Display = True
                c._Editabile = False
            Else
                c._daDuplicare = True
                c._hidden = True
            End If
        End If
        l.Add(c)

        c = New ColonneNome("Cal_Cod", "Cal_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cod_Progetto", "Cod_Progetto", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Accettazione", "Tipo_Accettazione", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Modulo", "Modulo", "string")
        c._hidden = True
        l.Add(c)

        'Descrizione imballo/contenitore/confezione
        For Each paramQual In dtParamQual.Rows

            If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

                c = New ColonneNome(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), paramQual(Tabella_Key) & _Tipo_Cod, "number")
                c._gruppoColonne = gruppoProdotto
                c._hidden = True
                c._Editabile = True
                c._daDuplicare = True

                If lavCodLavorazioni.Contains(lav_cod_chiamante) Then
                    If objDftLav IsNot Nothing AndAlso CStr(objDftLav(FF_ & paramQual(Tabella_Key) & _Cod)) IsNot Nothing Then
                        c._valueDefault = CInt(objDftLav(FF_ & paramQual(Tabella_Key) & _Cod))
                    Else
                        c._valueDefault = 0
                    End If
                End If
                l.Add(c)

            End If

        Next

        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Descrizione", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Imballaggio, "string")
            c._gruppoColonne = gruppoProdotto
            If movimentiDaGiacenza OrElse mostraCampiTestata Then
                c._Editabile = False
                c._Display = sceltaDisplay
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._width = "115px"
            Else
                c._daDuplicare = True
                c._hidden = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
            End If
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Descrizione", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Contenitore, "string")
            c._gruppoColonne = gruppoProdotto

            If movimentiDaGiacenza OrElse mostraCampiTestata Then
                c._Editabile = False
                c._Display = sceltaDisplay
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._width = "115px"
            Else
                c._daDuplicare = True
                c._hidden = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
            End If
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Descrizione", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Confezione, "string")
            c._gruppoColonne = gruppoProdotto

            If movimentiDaGiacenza OrElse mostraCampiTestata Then
                c._Editabile = False
                c._Display = sceltaDisplay
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._width = "115px"
            Else
                c._daDuplicare = True
                c._hidden = True
                If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                    c._Editabile = False
                Else
                    c._Editabile = True
                End If
            End If
            l.Add(c)
        End If

        If leggiTrasferimenti Then
            c = New ColonneNome("Trasf_Rif_Cau_Mov", "Trasf_Rif_Cau_Mov", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Id_Mov", "Trasf_Rif_Id_Mov", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Id_Mov_Det", "Trasf_Rif_Id_Mov_Det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Cau_Mov_Rif", "Trasf_Rif_Cau_Mov_Rif", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Id_Mov_Rif", "Trasf_Rif_Id_Mov_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Id_Mov_Det_Rif", "Trasf_Rif_Id_Mov_Det_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Rif_Qta", "Trasf_Rif_Qta", "number")
            c._hidden = True
            l.Add(c)


            c = New ColonneNome("Trasf_Tipo_Destinazione_2", "Trasf_Tipo_Destinazione_2", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Sa_Cod_2", "Trasf_Sa_Cod_2", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Trasf_Id_Destinazione_2", "Trasf_Id_Destinazione_2", "number")
            c._hidden = True
            l.Add(c)

        End If

        If leggiConferimenti Then

            c = New ColonneNome("Id_Reg_Dettaglio_Conferimento", "Id_Reg_Dettaglio_Conferimento", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("TagliandoPesa", "TagliandoPesa", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("PremioComplessivo", "PremioComplessivo", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("CodVarietaConferimento", "CodVarietaConferimento", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("DescAppezzamenti", "DescAppezzamenti", "string")
            c._hidden = True
            l.Add(c)

        End If

        If leggiRiferimento Then
            c = New ColonneNome("Piva_Rif", "Piva_Rif", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Sa_Cod_Rif", "Sa_Cod_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Id_Agenda_Rif", "Id_Agenda_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Id_Mov_Rif", "Id_Mov_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Id_Mov_Det_Rif", "Id_Mov_Det_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Lav_Cod_Rif", "Lav_Cod_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Cau_Mov_Rif", "Cau_Mov_Rif", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Qta_Rif", "Qta_Rif", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Ordine_Num_Concat", My.Resources.AgronicaCoreContabBIZ.NumeroOrdine, "string")

            If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
               lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
               lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
               lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
               lavCodAccettazione.Contains(lav_cod_chiamante) Then
                c._Display = False
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._width = "120px"
            Else
                c._hidden = True
            End If

            l.Add(c)

            c = New ColonneNome("List_Riferimenti", "List_Riferimenti", "string")
            c._hidden = True
            l.Add(c)

        End If

        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodAccettazione.Contains(lav_cod_chiamante) OrElse
           lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
           lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodNotaAccrVendita.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineAcquisto.Contains(lav_cod_chiamante) OrElse
           lavCodOrdineVendita.Contains(lav_cod_chiamante) OrElse
           lavCodMagazzino.Contains(lav_cod_chiamante) OrElse
           lavCodContrattoAffitto.Contains(lav_cod_chiamante) Then

            AggiungiColonneDocumenti(c, l,
                                     lav_cod_chiamante,
                                     lavCodDDTVendita,
                                     lavCodMagazzino,
                                     leggiOrdini,
                                     leggiRifOrdineCliente,
                                     leggiOrdiniDaEvadere,
                                     leggiDDTDaEvadere,
                                     FF_gest_materiale_vivaistico,
                                     leggiContiEconPatr,
                                     leggiProvvigioni,
                                     leggiPesiRiscontrati,
                                     gestitoImballaggio,
                                     gestitoContenitore,
                                     gestitoConfezione)

        Else

            If LeggiOrdiniLavoroDaEvadere Then

                AggiungiColonneStatoEvasione(c, l)

            End If

        End If

        If leggiCdC Then
            c = New ColonneNome("CdC_Des_Imputazione", "CdC / Wbs", "string")
            c._gruppoColonne = gruppoImputazioni
            c._daDuplicare = True
            c._obbligatorio = False
            c._Display = False
            c._Editabile = False
            c._width = "118px"
            'c._cssHeader = "breakWordDocCont"
            l.Add(c)
        End If

        If leggiPratica Then
            c = New ColonneNome("Pratica_Stato_Des", "Stato Pratica", "string")
            c._daDuplicare = True
            c._obbligatorio = False
            c._Display = False
            c._Editabile = False
            c._width = "120px"
            l.Add(c)

            c = New ColonneNome("Pratica_Stato_Note", "Note Stato Pratica", "string")
            c._daDuplicare = True
            c._obbligatorio = False
            c._Display = False
            c._Editabile = False
            c._width = "120px"
            l.Add(c)

            c = New ColonneNome("Pratica_Cod", "Pratica_Cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Pratica_Servizio_Cod", "Pratica_Servizio_Cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Pratica_Stato_Cod", "Pratica_Stato_Cod", "number")
            c._hidden = True
            l.Add(c)
        End If

        'Modifico visibilità colonne per contratti di affitto
        If lav_cod_chiamante = LAVCOD_CONTRATTO_AFFITTO Then
            Dim colonneGestiteContrattoAffitto As String() = {"Azioni", "Referenza", "Extra_Str", "Prezzo_Unitario"}
            l.ForEach(Sub(x)
                          If colonneGestiteContrattoAffitto.Contains(x._Nome_colonna_DT) Then
                              x._Display = True
                          Else
                              x._hidden = True
                          End If
                      End Sub)
        End If

        'Se l'utente non ha i permessi di lettura sui dettagli economici, nascondo forzatamente tutte le colonne di quel gruppo
        If permessoGestionePrezziLettura = False Then
            l.ForEach(Sub(x)
                          If x._gruppoColonne = gruppoEconomico Then
                              x._hidden = True
                          End If
                      End Sub)
        End If

        Dim js As New JSON_DataTable With {.Editabile_Deafault = False}
        risposta = js.JSON_DataTable_Kendo(DtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
        Return risposta

    End Function

    Private Sub AggiungiColonneParamQualCaricoScarico(
        ByRef l As List(Of ColonneNome),
        ByVal paramQual As Object,
        ByRef htParamQual As Hashtable,
        ByVal movimentiDaGiacenza As Boolean,
        ByVal lavCodTrasferimento As Integer(),
        ByVal lavCodLavorazioni As Integer(),
        ByVal lav_cod_chiamante As Integer,
        ByRef objDftLav As JToken,
        ByVal mostraCampiTestata As Boolean)

        Dim c As ColonneNome

        If Not IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

            Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQual(Tabella_Key))
            Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQual(Tabella_Key))
            Dim nomeColonnaCod = FF_ & paramQual(Tabella_Key) & _Cod

            Select Case paramQual(Tipo)

                Case enum_TipoParamQual.Numero
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "number")

                Case enum_TipoParamQual.Stringa
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "string")

                Case enum_TipoParamQual.Data
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "date")

                Case Else
                    c = New ColonneNome(nomeColonnaTipoCod, paramQual(Tabella_Key) & _Tipo_Cod, "number")

            End Select

            c._gruppoColonne = gruppoProdotto
            c._hidden = True

            If movimentiDaGiacenza OrElse mostraCampiTestata Then

                If paramQual(Tipo) <> enum_TipoParamQual.CodiceNumerico AndAlso htParamQual.ContainsKey(paramQual(Tabella_Key)) Then

                    c._hidden = False

                    SetFormatNrParamQualNumerico(c, paramQual)

                    c._Filtrabile = True
                    c._FiltrabileConCheck = True
                    c._Display = False
                    c._Editabile = False
                    c._width = "100px"

                End If

            Else

                c._daDuplicare = True

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then
                    If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                        c._Editabile = False
                    Else
                        c._Editabile = True
                    End If
                End If

                If lavCodLavorazioni.Contains(lav_cod_chiamante) Then

                    If objDftLav IsNot Nothing AndAlso CStr(objDftLav(nomeColonnaCod)) IsNot Nothing Then

                        Dim valoreColonnaStringa = CStr(objDftLav(nomeColonnaCod))

                        Select Case paramQual(Tipo)

                            Case enum_TipoParamQual.Stringa
                                c._valueDefault = valoreColonnaStringa

                            Case enum_TipoParamQual.Data
                                If valoreColonnaStringa <> "" AndAlso IsDate(valoreColonnaStringa) Then
                                    Dim dataColonna As DateTime = CDate(objDftLav(nomeColonnaCod))
                                    c._valueDefault = dataColonna.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                                Else
                                    c._valueDefault = "null"
                                End If

                            Case enum_TipoParamQual.Numero
                                If CInt(objDftLav(nomeColonnaCod)) <> 0 Then
                                    c._valueDefault = CInt(objDftLav(nomeColonnaCod))
                                Else
                                    c._valueDefault = "null"
                                End If

                            Case Else
                                c._valueDefault = CInt(objDftLav(nomeColonnaCod))

                        End Select

                    End If

                End If

            End If

            l.Add(c)

            If paramQual(Tipo) = enum_TipoParamQual.CodiceNumerico Then

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then

                    c = New ColonneNome(GetNomeColonnaSigla(paramQual(Tabella_Key)), paramQual(Tabella_Des), "string")
                    c._gruppoColonne = gruppoProdotto

                    If movimentiDaGiacenza AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                        c._hidden = True
                    Else
                        If movimentiDaGiacenza OrElse mostraCampiTestata Then
                            c._Editabile = False
                            c._Display = mostraCampiTestata
                            c._Filtrabile = True
                            c._FiltrabileConCheck = True
                        Else
                            c._daDuplicare = True
                            c._hidden = True
                            If lavCodTrasferimento.Contains(lav_cod_chiamante) Then
                                c._Editabile = False
                            Else
                                c._Editabile = True
                            End If
                        End If
                    End If

                    l.Add(c)

                End If

            End If

        End If

    End Sub

    Private Sub SetFormatNrParamQualNumerico(ByRef c As ColonneNome, ByVal paramQual As Object)

        If paramQual(Tipo) = enum_TipoParamQual.Numero Then
            c._formatNr = String.Format("n{0}", paramQual(NumDecimali_Maximo))
        End If

    End Sub

    Private Sub CaricaParamQualCaricoScarico(
        ByVal paramQual As Object,
        ByRef drMovDet As DataRow,
        ByRef dr As DataRow,
        ByRef htParamQual As Hashtable,
        ByVal lavCodLavorazioni As Integer(),
        ByVal lav_cod_chiamante As Integer,
        ByRef objParametri As Object)

        Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQual(Tabella_Key))
        Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQual(Tabella_Key))
        Dim nomeColonnaDescrizione = GetNomeColonnaDescrizione(paramQual(Tabella_Key))
        Dim nomeColonnaSigla = GetNomeColonnaSigla(paramQual(Tabella_Key))

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                AddElemHtParamQual(paramQual, drMovDet.Item(nomeColonnaValCod), htParamQual)

                If paramQual(Tipo) = enum_TipoParamQual.Numero Then

                    If CDec(drMovDet.Item(nomeColonnaValCod)) = 0 Then
                        'Il nr non era stato salvato
                        dr.Item(nomeColonnaValCod) = 0
                    Else
                        dr.Item(nomeColonnaValCod) = CDec(CStr(drMovDet.Item(nomeColonnaValCod)).Replace(".", ","))
                    End If

                End If

                If paramQual(Tipo) = enum_TipoParamQual.Stringa Then

                    If drMovDet.Item(nomeColonnaValCod) = "" Then
                        'La stringa non era stata salvata
                        dr.Item(nomeColonnaValCod) = ""
                    Else
                        dr.Item(nomeColonnaValCod) = drMovDet.Item(nomeColonnaValCod)
                    End If

                End If

                If paramQual(Tipo) = enum_TipoParamQual.Data Then

                    If drMovDet.Item(nomeColonnaValCod) = "" Then
                        'La data non era stata salvata
                        dr.Item(nomeColonnaValCod) = ""
                    Else
                        dr.Item(nomeColonnaValCod) = drMovDet.Item(nomeColonnaValCod)
                    End If

                End If

            Case Else

                'Parametri qualitativi da DDL

                AddElemHtParamQual(paramQual, drMovDet.Item(nomeColonnaTipoCod), htParamQual)

                dr.Item(nomeColonnaTipoCod) = drMovDet.Item(nomeColonnaTipoCod)

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then

                    dr.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura) = drMovDet.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura)
                    dr.Item(nomeColonnaSigla) = drMovDet.Item(nomeColonnaSigla)
                    dr.Item(nomeColonnaDescrizione) = drMovDet.Item(nomeColonnaDescrizione)
                    dr.Item(FF_ & paramQual(Tabella_Key) & _Codice_Generazione_Link) = drMovDet.Item(FF_ & paramQual(Tabella_Key) & _Codice_Generazione_Link)
                    dr.Item(FF_ & paramQual(Tabella_Key) & _Mat_Cod_Generazione_Link) = drMovDet.Item(FF_ & paramQual(Tabella_Key) & _Mat_Cod_Generazione_Link)

                End If

                If ({pq_fornitore}).Contains(paramQual(Tabella_Key)) Then

                    'Il Fornitore viene mostrato in colonna a parte

                    Dim objContattiR = New Contatti_R
                    If Not String.IsNullOrEmpty(drMovDet.Item(nomeColonnaTipoCod).ToString) Then
                        Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(CStr(drMovDet.Item(nomeColonnaTipoCod)), objParametri)
                        If objContatto.Rows.Count > 0 Then
                            dr.Item("Fornitore") = objContatto.Rows(0)("rag_soc")
                        End If
                    End If

                End If

        End Select

        ' Composizione referenza

        If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) AndAlso htParamQual(paramQual(Tabella_Key)) <> "" Then

            If Not lavCodLavorazioni.Contains(lav_cod_chiamante) OrElse Not IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

                AccodaInReferenza(paramQual, htParamQual, "<br/>", dr)

            End If

        End If

    End Sub

    Private Sub AccodaInReferenza(
        ByVal paramQual As Object,
        ByRef htParamQual As Hashtable,
        ByVal separatore As String,
        ByRef dr As DataRow)

        Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQual(Tabella_Key))
        Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQual(Tabella_Key))
        Dim nomeColonnaDescrizione = GetNomeColonnaDescrizione(paramQual(Tabella_Key))

        Dim formatoReferenza = separatore & "{0} : {1}"

        Dim referenza = ""

        Select Case True

            Case paramQual(Tipo) = enum_TipoParamQual.Numero AndAlso CDec(dr.Item(nomeColonnaValCod)) <> 0,
                 paramQual(Tipo) = enum_TipoParamQual.Stringa AndAlso CStr(dr.Item(nomeColonnaValCod)).Trim() <> ""

                referenza = String.Format(formatoReferenza,
                                          htParamQual(paramQual(Tabella_Key)),
                                          dr.Item(nomeColonnaValCod))

            Case paramQual(Tipo) = enum_TipoParamQual.Data

                Dim dataStringa = CStr(dr.Item(nomeColonnaValCod)).Trim()

                If dataStringa <> "" AndAlso dataStringa <> AGRODATAINIZIO.ToString(pq_formatoData) Then

                    Dim dataReferenza = ""
                    Try
                        dataReferenza = Date.ParseExact(dataStringa, pq_formatoData, Nothing)
                    Catch ex As Exception
                        dataReferenza = ""
                    End Try

                    If dataReferenza <> "" Then
                        referenza = String.Format(formatoReferenza,
                                                  htParamQual(paramQual(Tabella_Key)),
                                                  dataReferenza)
                    End If

                End If

            Case paramQual(Tipo) = enum_TipoParamQual.CodiceNumerico

                If CStr(dr.Item(nomeColonnaDescrizione)).Trim() <> "" Then
                    referenza = String.Format(formatoReferenza,
                                              htParamQual(paramQual(Tabella_Key)),
                                              dr.Item(nomeColonnaDescrizione))
                End If

        End Select

        If referenza <> "" Then
            dr.Item("Referenza") &= referenza
        End If

    End Sub

    Private Sub AccodaStringaInReferenza(
        ByVal descrizione As String,
        ByVal valore As String,
        ByVal separatore As String,
        ByRef dr As DataRow)

        Dim formatoReferenza = separatore & "{0} : {1}"

        Dim referenza = ""

        If valore.Trim() <> "" Then
            referenza = String.Format(formatoReferenza,
                                      descrizione,
                                      valore)
        End If

        If referenza <> "" Then
            dr.Item("Referenza") &= referenza
        End If

    End Sub

    Private Sub AddElemHtParamQual(
        ByVal paramQual As Object,
        ByVal valoreParamQual As Object,
        ByRef htParamQual As Hashtable)

        Dim paramQualTipo = paramQual(Tipo)
        Dim paramQualTabellaKey = paramQual(Tabella_Key)
        Dim paramQualTabellaDes = paramQual(Tabella_Des)

        If ({pq_cliente}).Contains(paramQualTabellaKey) Then
            Return
        End If

        If htParamQual.ContainsKey(paramQualTabellaKey) Then
            Return
        End If

        Select Case True

            Case paramQualTipo = enum_TipoParamQual.Numero AndAlso CDec(valoreParamQual) <> 0,
                 paramQualTipo = enum_TipoParamQual.Stringa AndAlso CStr(valoreParamQual).Trim() <> "",
                 paramQualTipo = enum_TipoParamQual.Data AndAlso CStr(valoreParamQual).Trim() <> "",
                 paramQualTipo = enum_TipoParamQual.CodiceNumerico AndAlso CInt(valoreParamQual) <> 0

                htParamQual.Add(paramQualTabellaKey, paramQualTabellaDes)

        End Select

    End Sub

    Private Function IsParamQualClienteFornitore(paramQualTabellaKey As String) As Boolean

        Return ({pq_cliente, pq_fornitore}).Contains(paramQualTabellaKey)

    End Function

    Private Function IsParamQualConfezionamento(paramQualTabellaKey As String) As Boolean

        Return ({pq_imballaggio, pq_contenitore, pq_confezione}).Contains(paramQualTabellaKey)

    End Function

    Private Sub AggiungiColonneDocumenti(ByRef c As ColonneNome,
                                         ByRef l As List(Of ColonneNome),
                                         lav_cod_chiamante As Integer,
                                         lavCodDDTVendita As Integer(),
                                         lavCodMagazzino As Integer(),
                                         leggiOrdini As Boolean,
                                         leggiRifOrdineCliente As Boolean,
                                         leggiOrdiniDaEvadere As Boolean,
                                         leggiDDTDaEvadere As Boolean,
                                         FF_gest_materiale_vivaistico As Boolean,
                                         leggiContiEconPatr As Boolean,
                                         leggiProvvigioni As Boolean,
                                         leggiPesiRiscontrati As Boolean,
                                         gestitoImballaggio As Boolean,
                                         gestitoContenitore As Boolean,
                                         gestitoConfezione As Boolean
                                         )

        c = New ColonneNome("Mov_Det_Des", "Mov_Det_Des", "string")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("FF_ONote_Descrizione", "Note", "string")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Pendente", "Pendente", "number")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Extra_Date", "Extra_Date", "date")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = True
        c._hidden = True
        c._obbligatorio = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Extra_Str", OttieniNomeColonnaExtraStr(lav_cod_chiamante), "string")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = True
        c._hidden = False
        c._Display = False
        c._obbligatorio = True
        c._Editabile = True
        c._width = "120px"
        l.Add(c)

        c = New ColonneNome("Sconto_Modalita", "Sconto_Modalita", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Prezzo_Livello", "Prezzo su", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        'c._cssHeader = "breakWordDocCont"
        l.Add(c)

        c = New ColonneNome("TempoCarenza", "Valore riferim.", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("Prezzo_Unitario", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_PrezzoUnit, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = True
        c._obbligatorio = False
        'c._cssHeader = "breakWordDocCont"
        c._width = "85px"
        c._formatNr = "n6"
        l.Add(c)

        c = New ColonneNome("ScontoMaggiorazione", "Sc. / Magg.", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("Sconto", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ScontoBase, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = False
        c._obbligatorio = False
        c._width = "85px"
        'c._cssHeader = "breakWordDocCont"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Sconto1", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ScontoAdd1, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = False
        c._obbligatorio = False
        c._width = "85px"
        'c._cssHeader = "breakWordDocCont"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Sconto2", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ScontoAdd2, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = False
        c._obbligatorio = False
        c._width = "85px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Sconto3", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ScontoAdd3, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        'c._cssHeader = "breakWordDocCont"
        c._Filtrabile = True
        c._Display = False
        c._obbligatorio = False
        c._width = "85px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Sconto_Calcolato", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ScontoTot, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        'c._cssHeader = "breakWordDocCont"
        c._Filtrabile = True
        c._Display = True
        c._obbligatorio = False
        c._width = "85px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Prezzo_Effettivo", "Prezzo al kg/l", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Prezzo_Unitario_Netto", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_PrezzoNetto, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = True
        c._obbligatorio = False
        'c._cssHeader = "breakWordDocCont"
        c._width = "85px"
        c._formatNr = "n6"
        l.Add(c)

        c = New ColonneNome("Listino_Cod", "Listino Prezzi", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Imponibile", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Imponibile, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = False
        c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = False
        c._width = "105px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Imponibile_Netto", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ImponibileNetto, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = True
        c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = False
        c._width = "105px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Cod_Iva", "Cod_Iva", "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("Aliquota_Iva_Des", My.Resources.AgronicaCoreContabBIZ.AliquotaIva, "string")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Filtrabile = True
        c._Display = True
        c._Editabile = True
        c._obbligatorio = True
        c._width = "120px"
        l.Add(c)

        c = New ColonneNome("Iva", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Iva, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = True
        c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = False
        c._width = "75px"
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("ChkIva_Manuale", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_IvaForzata, "Boolean")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = False
        c._Display = False
        c._obbligatorio = False
        c._width = "85px"
        'c._cssHeader = "breakWordDocCont"
        l.Add(c)

        c = New ColonneNome("Importo_Unitario", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ImportoUnit, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = False
        c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = False
        c._width = "105px"
        'c._cssHeader = "breakWordDocCont"
        c._formatNr = "n6"
        l.Add(c)

        c = New ColonneNome("Importo_Totale", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ImportoTotale, "number")
        c._gruppoColonne = gruppoEconomico
        c._daDuplicare = True
        c._Editabile = True
        c._Filtrabile = True
        c._Display = True
        c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = False
        c._width = "105px"
        'c._cssHeader = "breakWordDocCont"
        c._formatNr = "n2"
        l.Add(c)

        If leggiProvvigioni Then
            c = New ColonneNome("Provvigione", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Provvigione, "number")
            c._gruppoColonne = gruppoEconomico
            c._daDuplicare = True
            c._Editabile = True
            c._Filtrabile = True
            c._Display = False
            c._obbligatorio = False
            c._width = "112px"
            c._formatNr = "n2"
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoImballaggio Then
            c = New ColonneNome("Num_Imballi_Riscontrati", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ImbRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n0"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoContenitore Then
            c = New ColonneNome("Num_Colli_Riscontrati", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ContRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n0"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoConfezione Then
            c = New ColonneNome("Num_Conf_Riscontrate", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ConfezRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "90px"
                c._formatNr = "n0"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoImballaggio Then
            c = New ColonneNome("Tara_Unit_Imballo_Riscontrata", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnitImbRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "90px"
                c._formatNr = "n3"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoContenitore Then
            c = New ColonneNome("Tara_Unit_Collo_Riscontrata", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnitContRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "90px"
                c._formatNr = "n3"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati AndAlso gestitoConfezione Then
            c = New ColonneNome("Tara_Unit_Conf_Riscontrata", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_TaraUnitConfezRisc, "number")
            If Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "90px"
                c._formatNr = "n3"
            End If
            l.Add(c)
        End If

        If leggiPesiRiscontrati Then
            c = New ColonneNome("Peso_Lordo_Riscontrato", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_PesoLordoRisc, "number")
            If FF_gest_materiale_vivaistico OrElse
                    Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                'c._cssHeader = "breakWordDocCont"
                c._Filtrabile = True
                c._Display = False
                c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n3"
                l.Add(c)
            End If

            c = New ColonneNome("Peso_Netto_Riscontrato", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_PesoNettoRisc, "number")
            If FF_gest_materiale_vivaistico OrElse
                    Not lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                c._hidden = True
            Else
                c._gruppoColonne = gruppoPesoRiscontrato
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                c._Display = False
                c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n3"
                'c._cssHeader = "breakWordDocCont"
                l.Add(c)
            End If
        End If

        If leggiOrdini OrElse leggiRifOrdineCliente Then
            c = New ColonneNome("N_Doc_Cliente", "N_Doc_Cliente", "string")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Data_Doc_Cliente", "Data_Doc_Cliente", "date")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("N_Nota_DDT", "N_Nota_DDT", "string")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("N_Nota_Riga_DDT", "N_Nota_Riga_DDT", "string")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)

            c = New ColonneNome("Data_Nota_DDT", "Data_Nota_DDT", "date")
            c._gruppoColonne = gruppoProdotto
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            c._obbligatorio = True
            l.Add(c)
        End If

        If leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then
            c = New ColonneNome("Numero_Movimento_Registrazione", Gias.NDocumento, "string")
            c._gruppoColonne = ""
            c._Editabile = False
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._width = "130px"
            l.Add(c)

            c = New ColonneNome("Data_Movimento_Registrazione", Gias.Data, "date")
            c._gruppoColonne = ""
            c._Editabile = False
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._width = "104px"
            c._formatNr = "{0:dd/MM/yyyy}"
            l.Add(c)
        End If

        If leggiOrdini OrElse leggiOrdiniDaEvadere OrElse leggiDDTDaEvadere Then

            AggiungiColonneStatoEvasione(c, l)

        End If

        If leggiContiEconPatr Then

            AggiungiColonneContiEconPatr(c, l)

        End If

    End Sub

    Private Sub AggiungiColonneStatoEvasione(ByRef c As ColonneNome,
                                             ByRef l As List(Of ColonneNome))

        c = New ColonneNome("Contabilizzato", "Contabilizzato", "number")
        c._gruppoColonne = gruppoProdotto
        c._daDuplicare = False
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("Qta_Richiesta", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_QtaRichiesta, "number")
        c._gruppoColonne = gruppoQuantita
        c._Filtrabile = True
        c._Display = True
        'c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = True
        'c._cssHeader = "breakWordDocCont"
        c._width = "97px"
        c._Editabile = True
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Qta_Evasa", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_QtaEvasa, "number")
        c._gruppoColonne = gruppoQuantita
        c._Filtrabile = True
        c._Display = True
        'c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = True
        'c._cssHeader = "breakWordDocCont"
        c._width = "95px"
        c._Editabile = True
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Qta_Residua", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_QtaResidua, "number")
        c._gruppoColonne = gruppoQuantita
        c._Filtrabile = True
        c._Display = True
        'c._sum = True
        c._stringaTotale = ""
        c._obbligatorio = True
        'c._cssHeader = "breakWordDocCont"
        c._width = "95px"
        c._Editabile = True
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("StatoEvasione_Cod", "StatoEvasione_Cod", "number")
        c._gruppoColonne = gruppoQuantita
        c._daDuplicare = False
        c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("StatoEvasione_Des", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_StatoEvasione, "string")
        c._gruppoColonne = gruppoQuantita
        c._Filtrabile = True
        c._Display = True
        c._obbligatorio = True
        c._width = "105px"
        c._Editabile = True
        l.Add(c)

    End Sub

    Private Sub AggiungiColonneContiEconPatr(ByRef c As ColonneNome,
                                             ByRef l As List(Of ColonneNome))

        c = New ColonneNome("Anno", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Anno, "number")
        c._gruppoColonne = gruppoImputazioni
        c._daDuplicare = True
        c._Display = False
        c._Editabile = True
        c._width = "76px"
        l.Add(c)

        c = New ColonneNome("Cod_Conto_Economico", "Cod_Conto_Economico", "number")
        c._gruppoColonne = gruppoImputazioni
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Descr_Conto_Economico", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ContoEconomico, "string")
        c._gruppoColonne = gruppoImputazioni
        c._daDuplicare = True
        c._obbligatorio = True
        c._Display = False
        c._Editabile = True
        c._width = "108px"
        'c._cssHeader = "breakWordDocCont"
        l.Add(c)

        c = New ColonneNome("Cod_Conto_Patrimoniale", "Cod_Conto_Patrimoniale", "number")
        c._gruppoColonne = gruppoImputazioni
        c._daDuplicare = True
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("Descr_Conto_Patrimoniale", My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_ContoPatrimoniale, "string")
        c._gruppoColonne = gruppoImputazioni
        c._daDuplicare = True
        c._obbligatorio = True
        c._Display = False
        c._Editabile = True
        c._width = "118px"
        'c._cssHeader = "breakWordDocCont"
        l.Add(c)

    End Sub

    Private Function OttieniNomeColonnaChiusa(ByVal lav_cod_chiamante As Integer) As String
        If lav_cod_chiamante = LAVCOD_TRASFORMAZIONI Then
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_LavorazioneChiusa
        Else
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_OrdineChiuso
        End If
    End Function

    Private Function OttieniNomeColonnaReferenza(ByVal lav_cod_chiamante As Integer) As String
        If lav_cod_chiamante = LAVCOD_CONTRATTO_AFFITTO Then
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Descrizione
        Else
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Referenza
        End If
    End Function

    Private Function OttieniNomeColonnaExtraStr(ByVal lav_cod_chiamante As Integer) As String
        If lav_cod_chiamante = LAVCOD_CONTRATTO_AFFITTO Then
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_Riferimento
        Else
            Return My.Resources.AgronicaCoreContabBIZ.FF_MagazzinoBIZ_Leggi_Movimenti_Carico_Scarico_DescrAddizionale
        End If
    End Function

    Private Sub SeConvertiQtaPrezzi(ByVal UdmOrigine As Integer,
                                                 ByVal drMovDet As DataRow,
                                                 ByRef dtUnitaMisura As DataTable,
                                                 ByRef dr As DataRow,
                                                 ByRef objParametri As AgronicaCoreParametri)

        If SeQtaPrezziDaConvertire(UdmOrigine, drMovDet, dr) Then
            'Udm
            Dim UdmUtente As Integer = drMovDet.Item("MovDett_Extra_Int")
            'Quantità
            Dim QtaUdmOrig = CDec(drMovDet.Item("Dett_Qta"))
            Dim QtaUdmUtente As Decimal
            'Prezzo unitario
            Dim PrzUnitOrig As Decimal = drMovDet.Item("Prezzo_Unitario")
            Dim PrzUnitUtente As Decimal
            'Pesi Netto e Lordo
            'Dim QtaIngAgg() As Decimal = {dr.Item("KgNetti"), dr.Item("KgLordi"), dr.Item("Quantita_Escluso_Degrado")}
            Dim QtaIngAgg() As Decimal = {dr.Item("KgNetti"), dr.Item("KgLordi")}
            Dim QtaUscAgg() As Decimal = {}
            'Prezzo unitario netto
            Dim PrzIngAgg() As Decimal = {drMovDet.Item("Prezzo_Unitario_Netto")}
            Dim PrzUscAgg() As Decimal = {}
            'Conversione
            UnitaMisura_R.ConvertiQtaPrezzi(UdmOrigine,
                                            UdmUtente,
                                            QtaUdmOrig,
                                            QtaUdmUtente,
                                            PrzUnitOrig,
                                            PrzUnitUtente,
                                            QtaIngAgg:=QtaIngAgg,
                                            QtaUscAgg:=QtaUscAgg,
                                            PrezzoIngAgg:=PrzIngAgg,
                                            PrezzoUscAgg:=PrzUscAgg)
            'Lettura dati Udm gestite in conversione
            If IsNothing(dtUnitaMisura) Then
                Dim objUnitaMisura = New UnitaMisura_R
                dtUnitaMisura = objUnitaMisura.LeggiUdmConversione(objParametri)
            End If
            'Lettura dati Udm
            Dim datiRigaUdm() As DataRow
            datiRigaUdm = dtUnitaMisura.Select("Udm_Cod = " & UdmUtente)
            'Memorizzazione dati in sostituzione

            Dim confezioneCodRiga As Integer = 0
            If dr.Table.Columns("FF_confezione_Tipo_Cod") IsNot Nothing Then
                confezioneCodRiga = CInt(dr.Item("FF_confezione_Tipo_Cod"))
            End If

            'Nel caso di confezioni nei parametri qualitativi, la colonna Dett_Qta contiene non il peso,
            'ma il numero di confezioni, quindi non assegno QtaUdmUtente al datatable da restituire
            If Not (dr.Item("Udm_Cod") = enum_UnitaMisura.Numero AndAlso confezioneCodRiga > 1) Then
                dr.Item("Qta") = QtaUdmUtente
            End If

            dr.Item("KgNetti") = QtaUscAgg(0)
            dr.Item("KgLordi") = QtaUscAgg(1)
            'dr.Item("Quantita_Escluso_Degrado") = QtaUscAgg(2)
            dr.Item("Udm_Cod") = UdmUtente
            dr.Item("Udm_Des") = ComponiUdmDes(datiRigaUdm(0).Item("Udm_Des"), datiRigaUdm(0).Item("Udm_Sim"))
            dr.Item("Prezzo_Unitario") = PrzUnitUtente
            dr.Item("Prezzo_Unitario_Netto") = PrzUscAgg(0)
        End If

    End Sub

    Private Function SeQtaPrezziDaConvertire(ByVal UdmOrigine As Integer,
                                                     ByVal drMovDet As DataRow,
                                                     ByVal dr As DataRow) As Boolean

        Dim CauMov = dr.Item("Cau_Mov")
        Dim CategMag = dr.Item("Cat_Cod")
        Dim UdmUtente As Integer = drMovDet.Item("MovDett_Extra_Int")
        Dim lavCodNonConvertire As New List(Of Integer) From {LAVCOD_TRASFORMAZIONI, LAVCOD_TESTATE_ORDINE_LAVORAZIONE}

        If Not lavCodNonConvertire.Contains(drMovDet.Item("Lav_Cod")) AndAlso UdmOrigine <> UdmUtente AndAlso UdmUtente <> 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub SeConvertiQtaEvasione(ByVal UdmOrigine As Integer,
                                                   ByVal drMovDet As DataRow,
                                                   ByVal dr As DataRow,
                                                   ByRef qtaRichiesta As Decimal,
                                                   ByRef qtaEvasa As Decimal,
                                                   ByRef qtaResidua As Decimal)

        If SeQtaPrezziDaConvertire(UdmOrigine, drMovDet, dr) Then
            'Udm
            Dim UdmUtente As Integer = drMovDet.Item("MovDett_Extra_Int")
            'Quantità
            Dim QtaIngAgg() As Decimal = {CDec(qtaRichiesta), CDec(qtaEvasa), CDec(qtaResidua)}
            Dim QtaUscAgg() As Decimal = {}
            'Conversione
            UnitaMisura_R.ConvertiQtaPrezzi(UdmOrigine,
                                            UdmUtente,
                                            QtaIngAgg:=QtaIngAgg,
                                            QtaUscAgg:=QtaUscAgg)
            'Memorizzazione dati in sostituzione
            qtaRichiesta = QtaUscAgg(0)
            qtaEvasa = QtaUscAgg(1)
            qtaResidua = QtaUscAgg(2)
        End If

    End Sub

    '============================================================================

    Public Function Leggi_Movimenti_Carico_Scarico_NoDest(ByVal Piva As String,
                                                          ByVal Id_Agenda As Integer,
                                                          ByVal Cau_Mov As String,
                                                          ByVal idMovDet As Integer,
                                                          ByVal modificaColonneAnag As Boolean,
                                                          ByVal modificaColonneQta As Boolean,
                                                          ByVal mostraTuttiParamQual As Boolean,
                                                          ByVal lav_cod_chiamante As Integer,
                                                          ByRef dtDati As DataTable,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As String

        ' Il campo mostraTuttiParamQual viene passato a true se posso creare nuove righe, diversamente a false

        Dim risposta As String = ""

        Dim nomeRoutine As String = "FF_LavorazioneBIZ.Leggi_Movimenti_Carico_Scarico_NoDest()"

        Dim messaggioErrore As String = ""

        Dim i As Integer

        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objMov As New AgronicaCoreContabDAL.Movimenti_R

        Dim dtMovimentiDettagli As DataTable
        dtDati = New DataTable

        Dim xFiltroAggiuntivo As String = ""
        Dim xOrderBy As String = " Movimenti_Dettagli.Data_Creazione "

        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R

        Dim dtParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)

        Dim htParamQual As New Hashtable
        Dim htDefault As New Hashtable

        Try

            Dim dataOraUltimaLettura As DateTime = DateTime.Now

            dtMovimentiDettagli = objMovDet.LeggiCaricoScarico_NoDest(Piva, 0, Id_Agenda, 0, idMovDet,
                                                                      Elem_Cod:=TRASFORMATI_VEGETALI,
                                                                      Pro_Cod:=0,
                                                                      Mat_Cod:=0,
                                                                      Udm_Cod:=0,
                                                                      Jolly_Int:=1,
                                                                      Cau_Mov:=Cau_Mov,
                                                                      Cod_Progetto:=0,
                                                                      Fase_Cod:=0,
                                                                      Contabilizzato:=0,
                                                                      Pendente:=0,
                                                                      Lotto:="",
                                                                      Cal_Cod:=0,
                                                                      xFiltroAggiuntivo:=xFiltroAggiuntivo,
                                                                      xOrderBy:=xOrderBy,
                                                                      objParametri:=objParametri,
                                                                      leggiProdotto:=True,
                                                                      leggiMateriePrimeCampionature:=True,
                                                                      leggiTareCampionDaAnagrafica:=True)


            If lav_cod_chiamante = LAVCOD_MONITORAGGIO_TEMPI_RIENTRO AndAlso Cau_Mov = CAU_CARICO Then
                Dim lottoDef As String = objMov.Leggi_Extra_Str(Piva, Id_Agenda, Cau_Mov, "", objParametri)
                htDefault.Add("Lotto", lottoDef)
            End If

            If dtMovimentiDettagli.Rows.Count > 0 Then

                dtDati.Columns.Add(New DataColumn("key_mov_dett", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Piva", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
                'dtDati.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Mov_Det_Des", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
                'dtDati.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Mat_Des", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Referenza", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Lotto", GetType(String)))
                dtDati.Columns.Add(New DataColumn("DataOraUltimaLettura", GetType(Date)))

                For Each paramQual In dtParamQual.Rows

                    ColumnsAddParamQualCarScarNoDest(dtDati, paramQual)

                Next

                dtDati.Columns.Add(New DataColumn("Fornitore", GetType(String)))

                dtDati.Columns.Add(New DataColumn("KgLordi", GetType(Decimal)))
                dtDati.Columns.Add(New DataColumn("NrImballaggi", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("NrContenitori", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("NrConfezioni", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("KgNetti", GetType(Decimal)))

                For i = 0 To dtMovimentiDettagli.Rows.Count - 1

                    Dim dr As DataRow

                    'Creo una nuova riga
                    dr = dtDati.NewRow

                    dr.Item("Piva") = dtMovimentiDettagli.Rows(i).Item("Piva")
                    dr.Item("Id_Agenda") = dtMovimentiDettagli.Rows(i).Item("Id_Agenda")
                    dr.Item("Id_Mov") = dtMovimentiDettagli.Rows(i).Item("Id_Mov")
                    dr.Item("Id_Mov_Det") = dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det")
                    dr.Item("Cal_Cod") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Cal_Cod")), dtMovimentiDettagli.Rows(i).Item("Cal_Cod"), 0)
                    dr.Item("Udm_Cod") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Udm_Cod")), dtMovimentiDettagli.Rows(i).Item("Udm_Cod"), 0)
                    'dr.Item("Udm_Des") = DtMovimenti_Dettagli.Rows(i).Item("Udm_Des")
                    dr.Item("Data_Movimento") = dtMovimentiDettagli.Rows(i).Item("Data_Movimento")
                    dr.Item("DataOraUltimaLettura") = dataOraUltimaLettura
                    dr.Item("Data_Creazione") = dtMovimentiDettagli.Rows(i).Item("Data_Creazione2")
                    dr.Item("Data_Modifica") = dtMovimentiDettagli.Rows(i).Item("Data_Modifica")
                    dr.Item("Mov_Det_Des") = dtMovimentiDettagli.Rows(i).Item("Mov_Det_Des")

                    dr.Item("Cat_Cod") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Elem_Cod")), dtMovimentiDettagli.Rows(i).Item("Elem_Cod"), 0)
                    'dr.Item("Cat_Des") = If(Not IsDBNull(DtMovimenti_Dettagli.Rows(i).Item("NomeComune")), DtMovimenti_Dettagli.Rows(i).Item("NomeComune"), "")
                    dr.Item("Pro_Cod") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Pro_Cod")), dtMovimentiDettagli.Rows(i).Item("Pro_Cod"), 0)
                    dr.Item("Mat_Cod") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Mat_Cod")), dtMovimentiDettagli.Rows(i).Item("Mat_Cod"), 0)
                    dr.Item("Mat_Des") = dtMovimentiDettagli.Rows(i).Item("Descrizione_Prodotto")
                    dr.Item("Referenza") = dtMovimentiDettagli.Rows(i).Item("Descrizione_Prodotto")
                    dr.Item("Lotto") = If(Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Lotto")), dtMovimentiDettagli.Rows(i).Item("Lotto"), "")

                    For Each paramQual In dtParamQual.Rows

                        CaricaParamQualCarScarNoDest(paramQual,
                                                     dtMovimentiDettagli.Rows(i),
                                                     dr,
                                                     htParamQual,
                                                     objParametri)

                    Next

                    Dim numConfezioni As Integer = 0
                    Dim numContenitori As Integer = CInt(dtMovimentiDettagli.Rows(i).Item("Qta_Dettaglio1"))
                    Dim numImballaggi As Integer = CInt(dtMovimentiDettagli.Rows(i).Item("Qta_Dettaglio2"))

                    If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero Then
                        numConfezioni = dtMovimentiDettagli.Rows(i).Item("Qta")
                    End If

                    dr.Item("NrConfezioni") = numConfezioni
                    dr.Item("NrContenitori") = numContenitori
                    dr.Item("NrImballaggi") = numImballaggi

                    'Questa IF viene fatta perché per all'8/11/2017 il campo Qta_Extra_Totale nei movimenti di scarico (cau_mov = 7350) non veniva valorizzato
                    Dim kgNetti As Decimal
                    If dtMovimentiDettagli.Rows(i).Item("Qta_Extra_Totale") <> 0 Then
                        kgNetti = CDec(dtMovimentiDettagli.Rows(i).Item("Qta_Extra_Totale"))
                    Else
                        kgNetti = CDec(dtMovimentiDettagli.Rows(i).Item("Qta") * dtMovimentiDettagli.Rows(i).Item("Qta_Extra"))
                    End If

                    Dim tara As Decimal
                    If CDec(dtMovimentiDettagli.Rows(i).Item("Tara")) <> 0 Then
                        tara = CDec(dtMovimentiDettagli.Rows(i).Item("tara"))
                    Else
                        'provo a trovare la tara moltiplicando le varie tare
                        If dr.Table.Columns("FF_imballaggio_Tara_Campionatura") IsNot Nothing Then
                            tara = CDec(numImballaggi * CDec(dr.Item("FF_imballaggio_Tara_Campionatura")))
                        End If
                        If dr.Table.Columns("FF_contenitore_Tara_Campionatura") IsNot Nothing Then
                            tara += CDec(numContenitori * CDec(dr.Item("FF_contenitore_Tara_Campionatura")))
                        End If
                        If dr.Table.Columns("FF_confezione_Tara_Campionatura") IsNot Nothing Then
                            tara += CDec(numConfezioni * CDec(dr.Item("FF_confezione_Tara_Campionatura")))
                        End If
                    End If

                    dr.Item("KgNetti") = kgNetti
                    dr.Item("KgLordi") = kgNetti + tara

                    dr.Item("key_mov_dett") = dr.Item("piva") & "_" &
                                              dr.Item("Id_Agenda") & "_" &
                                              dr.Item("Id_Mov") & "_" &
                                              dr.Item("Id_Mov_Det")
                    dtDati.Rows.Add(dr)

                Next

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        'creo la lista delle colonne da visualizzare
        Dim l As List(Of ColonneNome) = CreaColonneCaricoScaricoNoDest(Cau_Mov, dtParamQual, htParamQual, mostraTuttiParamQual,
                                                                       modificaColonneAnag, modificaColonneQta,
                                                                       htDefault)

        Dim js As New JSON_DataTable
        js.Editabile_Deafault = False
        risposta = js.JSON_DataTable_Kendo(dtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
        Return risposta

    End Function

    Private Sub CaricaParamQualCarScarNoDest(
        ByVal paramQual As Object,
        ByRef drMovDetNoDest As DataRow,
        ByRef dr As DataRow,
        ByRef htParamQual As Hashtable,
        ByRef objParametri As Object)

        Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQual(Tabella_Key))
        Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQual(Tabella_Key))
        Dim nomeColonnaSigla = GetNomeColonnaSigla(paramQual(Tabella_Key))
        Dim nomeColonnaDescrizione = GetNomeColonnaDescrizione(paramQual(Tabella_Key))

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                AddElemHtParamQual(paramQual, drMovDetNoDest.Item(nomeColonnaValCod), htParamQual)

                If paramQual(Tipo) = enum_TipoParamQual.Numero Then
                    If CDec(drMovDetNoDest.Item(nomeColonnaValCod)) = 0 Then
                        'Il nr non era stato salvato
                        dr.Item(nomeColonnaValCod) = 0
                    Else
                        dr.Item(nomeColonnaValCod) = CDec(CStr(drMovDetNoDest.Item(nomeColonnaValCod)).Replace(".", ","))
                    End If
                End If

                If paramQual(Tipo) = enum_TipoParamQual.Stringa Then
                    If drMovDetNoDest.Item(nomeColonnaValCod) = "" Then
                        'La stringa non era stata salvata
                        dr.Item(nomeColonnaValCod) = ""
                    Else
                        dr.Item(nomeColonnaValCod) = drMovDetNoDest.Item(nomeColonnaValCod)
                    End If
                End If

                If paramQual(Tipo) = enum_TipoParamQual.Data Then
                    If drMovDetNoDest.Item(nomeColonnaValCod) = "" Then
                        'La data non era stata salvata
                        dr.Item(nomeColonnaValCod) = ""
                    Else
                        dr.Item(nomeColonnaValCod) = drMovDetNoDest.Item(nomeColonnaValCod)
                    End If
                End If

            Case Else

                'Parametri qualitativi da DDL

                AddElemHtParamQual(paramQual, drMovDetNoDest.Item(nomeColonnaTipoCod), htParamQual)

                dr.Item(nomeColonnaTipoCod) = drMovDetNoDest.Item(nomeColonnaTipoCod)

                If Not ({pq_cliente, pq_fornitore}).Contains(paramQual(Tabella_Key)) Then

                    dr.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura) = drMovDetNoDest.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura)
                    dr.Item(nomeColonnaSigla) = drMovDetNoDest.Item(nomeColonnaSigla)
                    dr.Item(nomeColonnaDescrizione) = drMovDetNoDest.Item(nomeColonnaDescrizione)

                    If Not IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

                        If Not String.IsNullOrEmpty(dr.Item(nomeColonnaSigla)) Then
                            dr.Item("Referenza") &= " " & dr.Item(nomeColonnaSigla)
                        End If

                    Else

                        If CInt(drMovDetNoDest.Item(FF_ & paramQual(Tabella_Key) & _ChkTara_Campionatura)) = 0 Then
                            'Devo usare la tara di anagrafica
                            dr.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura) = CDec(drMovDetNoDest.Item(FF_ & paramQual(Tabella_Key) & _Tara_Anagrafica))
                        End If

                        If Not String.IsNullOrEmpty(dr.Item(nomeColonnaDescrizione)) Then
                            dr.Item("Referenza") &= " " & dr.Item(nomeColonnaDescrizione)
                        End If

                    End If

                End If

                If ({pq_fornitore}).Contains(paramQual(Tabella_Key)) Then

                    Dim objContattiR = New Contatti_R
                    If Not String.IsNullOrEmpty(drMovDetNoDest.Item(nomeColonnaTipoCod).ToString) Then
                        Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(CStr(drMovDetNoDest.Item(nomeColonnaTipoCod)), objParametri)
                        If (objContatto.Rows.Count > 0) Then
                            dr.Item("Fornitore") = objContatto.Rows(0)("rag_soc")
                            dr.Item("Referenza") &= " " & dr.Item("Fornitore")
                        End If
                    End If

                End If

        End Select

    End Sub

    Private Sub ColumnsAddParamQualCarScarNoDest(dtDati As DataTable, paramQual As Object)

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero
                dtDati.Columns.Add(New DataColumn(GetNomeColonnaValCod(paramQual(Tabella_Key)), GetType(Decimal)))

            Case enum_TipoParamQual.Stringa, enum_TipoParamQual.Data
                dtDati.Columns.Add(New DataColumn(GetNomeColonnaValCod(paramQual(Tabella_Key)), GetType(String)))

            Case Else

                dtDati.Columns.Add(New DataColumn(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), GetType(Integer)))

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then

                    If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then
                        dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _ChkTara_Campionatura, GetType(Integer)))
                        dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _Tara_Anagrafica, GetType(Decimal)))
                    End If

                    dtDati.Columns.Add(New DataColumn(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura, GetType(Decimal)))
                    dtDati.Columns.Add(New DataColumn(GetNomeColonnaSigla(paramQual(Tabella_Key)), GetType(String)))
                    dtDati.Columns.Add(New DataColumn(GetNomeColonnaDescrizione(paramQual(Tabella_Key)), GetType(String)))

                End If

        End Select

    End Sub

    Private Function CreaColonneCaricoScaricoNoDest(ByVal cauMov As String,
                                                    ByVal dtParamQual As DataTable,
                                                    ByVal htParamQual As Hashtable,
                                                    ByVal mostraTuttiParamQual As Boolean,
                                                    ByVal modificaColonneAnag As Boolean,
                                                    ByVal modificaColonneQta As Boolean,
                                                    ByVal htDefault As Hashtable
                                                    ) As List(Of ColonneNome)

        Dim gestitoImballaggio As Boolean = False
        Dim gestitoContenitore As Boolean = False
        Dim gestitoConfezione As Boolean = False
        For Each paramQual In dtParamQual.Rows
            If (paramQual(Tabella_Key) = pq_imballaggio) Then
                gestitoImballaggio = True
            End If
            If (paramQual(Tabella_Key) = pq_contenitore) Then
                gestitoContenitore = True
            End If
            If (paramQual(Tabella_Key) = pq_confezione) Then
                gestitoConfezione = True
            End If
        Next

        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Lotto", "Lotto", "string") With {
            ._Filtrabile = True,
            ._FiltrabileConCheck = True,
            ._Display = True,
            ._obbligatorio = True
        }
        If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
            c._Editabile = False
            c._locked = True
        Else
            c._Editabile = True
            c._daDuplicare = True
            If Not IsNothing(htDefault("Lotto")) Then
                c._valueDefault = htDefault.Item("Lotto")
            End If

        End If
        l.Add(c)

        c = New ColonneNome("key_mov_dett", "key_mov_dett", "string") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Movimento", "Data mov.", "date")
        'c._Editabile = False
        'c._Filtrabile = True
        'c._Display = True
        'c._formatNr = "{0:dd/MM/yyyy}"
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Creazione", "Data creazione", "date") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._FiltrabileConCheck = False,
            ._Display = False,
            ._formatNr = "{0:dd/MM/yyyy}"
        }
        l.Add(c)

        c = New ColonneNome("DataOraUltimaLettura", "DataOraUltimaLettura", "date") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Modifica", "Data U.M.", "date")
        'c._Editabile = False
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'c._formatNr = "{0:dd/MM/yyyy}"
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cat_Cod", "Elem_Cod", "number") With {
            ._daDuplicare = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Pro_Cod", "Pro_Cod", "string") With {
            ._daDuplicare = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number")
        If cauMov = CAU_SCARICO Then
            c._hidden = True
        Else
            'c._Display = True
            c._daDuplicare = True
            c._hidden = True
            c._obbligatorio = True
            c._Editabile = True
            'c._locked = True
        End If
        l.Add(c)

        c = New ColonneNome("Referenza", "Referenza", "string")
        If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
            c._daDuplicare = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            c._Editabile = False
            c._Display = True
            c._width = "300px"
            c._locked = True
            l.Add(c)
        End If

        c = New ColonneNome("Mat_Des", "Prodotto", "string")
        If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
            c._Editabile = False
            c._Display = False
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        Else
            c._daDuplicare = True
            c._hidden = True
            c._obbligatorio = True
            c._Editabile = True
            'c._locked = True
        End If
        l.Add(c)

        'Caratteristiche prodotto
        For Each paramQual In dtParamQual.Rows

            AggiungiColonneParamQualCarScarNoDest(
                l,
                paramQual,
                htParamQual,
                mostraTuttiParamQual,
                modificaColonneAnag,
                cauMov)

        Next

        c = New ColonneNome("Fornitore", "Fornitore", "string")
        If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(pq_fornitore) Then
            c._hidden = True
        Else
            If cauMov = CAU_SCARICO Then
                c._Filtrabile = True
                c._FiltrabileConCheck = True
                c._Display = True
                c._Editabile = False
            Else
                c._daDuplicare = True
                c._hidden = True
            End If
        End If
        l.Add(c)

        c = New ColonneNome("Cal_Cod", "Cal_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "string")
        c._daDuplicare = True
        c._hidden = True
        l.Add(c)

        'c = New ColonneNome("Udm_Des", "Unità Misura", "string")
        'c._hidden = True
        'c._Filtrabile = True
        'c._FiltrabileConCheck = True
        'c._Display = True
        'c._Editabile = False
        'l.Add(c)


        'Descrizione imballo/contenitore/confezione
        For Each paramQual In dtParamQual.Rows

            If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

                c = New ColonneNome(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), paramQual(Tabella_Key) & _Tipo_Cod, "number") With {
                    ._hidden = True,
                    ._Editabile = True,
                    ._daDuplicare = True
                }
                l.Add(c)

            End If
        Next

        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Descrizione", "Imballaggio", "string")

            'Mostro sempre imballaggio
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
            '    c._hidden = True
            'Else
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
                'c._Display = True
                'c._Editabile = False
            End If
            'End If
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Descrizione", "Contenitore", "string")

            'Mostro sempre contenitore
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
            '    c._hidden = True
            'Else
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
                'c._Display = True
                'c._Editabile = False
            End If
            'End If
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Descrizione", "Confezione", "string")

            'Mostro sempre confezione
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
            '    c._hidden = True
            'Else
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
                'c._Display = True
                'c._Editabile = False
            End If
            'End If
            l.Add(c)
        End If

        If gestitoImballaggio Then
            c = New ColonneNome("NrImballaggi", "Nr Imb.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(imballaggio) Then
            '    c._hidden = True
            'Else
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._sum = True
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneQta Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            'End If
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("NrContenitori", "Nr Cont.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(contenitore) Then
            '    c._hidden = True
            'Else
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._sum = True
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneQta Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            'End If
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("NrConfezioni", "Nr Conf.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(confezione) Then
            '    c._hidden = True
            'Else
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._sum = True
            If cauMov = CAU_SCARICO AndAlso Not modificaColonneQta Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            'End If
            l.Add(c)
        End If

        'Tare
        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Tara_Campionatura", "Tara Un. Imb.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(imballaggio) Then
            '    c._hidden = True
            'Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n0"
            c._Editabile = False
            c._daDuplicare = True
            'End If
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Tara_Campionatura", "Tara Un. Cont.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(contenitore) Then
            '    c._hidden = True
            'Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n2"
            c._Editabile = False
            c._daDuplicare = True
            'End If
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Tara_Campionatura", "Tara Un. Conf.", "number")
            'If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(confezione) Then
            '    c._hidden = True
            'Else
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n3"
            c._Editabile = False
            c._daDuplicare = True
            'End If
            l.Add(c)
        End If

        c = New ColonneNome("KgLordi", "Kg Lordi", "number")
        c._Filtrabile = True
        c._Display = True
        c._sum = True
        If cauMov = CAU_SCARICO AndAlso Not modificaColonneQta Then
            c._Editabile = False
        Else
            c._Editabile = True
        End If
        c._formatNr = "n0"
        l.Add(c)

        c = New ColonneNome("KgNetti", "Kg Netti", "number")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "n0"
        c._sum = True
        If cauMov = CAU_SCARICO AndAlso Not modificaColonneQta Then
            c._Editabile = False
        Else
            c._Editabile = True
        End If
        l.Add(c)

        Return l

    End Function

    Private Sub AggiungiColonneParamQualCarScarNoDest(
        ByRef l As List(Of ColonneNome),
        ByVal paramQual As Object,
        ByRef htParamQual As Hashtable,
        ByVal mostraTuttiParamQual As Boolean,
        ByVal modificaColonneAnag As Boolean,
        ByVal cauMov As String)

        If Not IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

            Dim c As ColonneNome

            Dim nomeColonnaValCod = GetNomeColonnaValCod(paramQual(Tabella_Key))
            Dim nomeColonnaTipoCod = GetNomeColonnaTipoCod(paramQual(Tabella_Key))
            Dim nomeColonnaSigla = GetNomeColonnaSigla(paramQual(Tabella_Key))

            Select Case paramQual(Tipo)

                Case enum_TipoParamQual.Numero
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "number")

                Case enum_TipoParamQual.Stringa
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "string")

                Case enum_TipoParamQual.Data
                    c = New ColonneNome(nomeColonnaValCod, htParamQual(paramQual(Tabella_Key)), "date")

                Case Else
                    c = New ColonneNome(nomeColonnaTipoCod, paramQual(Tabella_Key) & _Tipo_Cod, "number")

            End Select

            If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                c._hidden = True
            Else
                If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
                    c._hidden = True
                Else
                    c._daDuplicare = True
                    c._hidden = True
                    If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then
                        c._Editabile = True
                    End If
                End If
            End If

            l.Add(c)

            If paramQual(Tipo) = enum_TipoParamQual.CodiceNumerico Then

                If Not IsParamQualClienteFornitore(paramQual(Tabella_Key)) Then

                    c = New ColonneNome(nomeColonnaSigla, paramQual(Tabella_Des), "string")

                    If Not mostraTuttiParamQual AndAlso Not htParamQual.ContainsKey(paramQual(Tabella_Key)) Then
                        c._hidden = True
                    Else
                        If cauMov = CAU_SCARICO AndAlso Not modificaColonneAnag Then
                            c._Editabile = False
                            c._Display = False
                            c._Filtrabile = True
                            c._FiltrabileConCheck = True
                        Else
                            c._daDuplicare = True
                            c._hidden = True
                            c._Editabile = True
                        End If
                    End If

                    l.Add(c)

                End If

            End If

        End If

    End Sub


    '============================================================================
    Public Function Leggi_Imballaggi_Riga_Documento(ByVal Piva As String,
                                                   ByVal Id_Agenda As Integer,
                                                   ByVal lav_cod_chiamante As Integer,
                                                   ByVal Id_Mov_Det As Integer,
                                                   ByVal movimentiDaGiacenza As Boolean,
                                                   ByRef DtDati As DataTable,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef objParametriUtenti As AgronicaCoreParametri
                                                   ) As String

        Dim risposta As String = ""

        Dim nomeRoutine As String = "FF_MagazzinoBIZ.Leggi_Imballaggi_Riga_Documento()"

        Dim messaggioErrore As String = ""

        Dim lavCodDDTAcquisto As Integer() = {LAVCOD_BOLLA_RICEVUTA}
        Dim lavCodAccettazione As Integer() = {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO}
        Dim lavCodDDTVendita As Integer() = {LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA}
        Dim lavCodFattAcquisto As Integer() = {LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}
        Dim lavCodFattVendita As Integer() = {LAVCOD_FATTURA_EMESSA}
        Dim lavCodNotaAccrVendita As Integer() = {LAVCOD_NOTA_ACCREDITO_EMESSA}
        Dim lavCodOrdineAcquisto As Integer() = {LAVCOD_ORDINE_ACQUISTO}
        Dim lavCodOrdineVendita As Integer() = {LAVCOD_ORDINE_VENDITA}

        Dim isVendita = False
        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
                lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
                lavCodNotaAccrVendita.Contains(lav_cod_chiamante) OrElse
                lavCodOrdineVendita.Contains(lav_cod_chiamante) Then
            isVendita = True
        End If

        Dim leggiPesiRiscontrati As Boolean = False
        'lavCodDDTAcquisto.Contains(lav_cod_chiamante) OrElse
        'lavCodAccettazione.Contains(lav_cod_chiamante) OrElse
        'lavCodFattAcquisto.Contains(lav_cod_chiamante) OrElse
        If lavCodDDTVendita.Contains(lav_cod_chiamante) OrElse
           lavCodFattVendita.Contains(lav_cod_chiamante) OrElse
           lavCodNotaAccrVendita.Contains(lav_cod_chiamante) Then

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.SuperUser_PesiColli_Riscontrati, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)
            If dtImpostazioni.Rows.Count > 0 AndAlso (dtImpostazioni(0)("Impostazione_Valore_1") = "1") Then
                leggiPesiRiscontrati = True
            End If

        End If

        Dim i As Integer

        Dim objMovDetR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim dtMovimentiDettagli As New DataTable
        DtDati = New DataTable

        Dim xFiltroAggiuntivo As String = ""
        Dim xOrderBy As String = " Movimenti_Dettagli.Data_Creazione "

        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim dtParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)
        Dim gestitoImballaggio As Boolean = False
        Dim gestitoContenitore As Boolean = False
        Dim gestitoConfezione As Boolean = False
        For Each paramQual In dtParamQual.Rows
            If (paramQual(Tabella_Key) = pq_imballaggio) Then
                gestitoImballaggio = True
            End If
            If (paramQual(Tabella_Key) = pq_contenitore) Then
                gestitoContenitore = True
            End If
            If (paramQual(Tabella_Key) = pq_confezione) Then
                gestitoConfezione = True
            End If
        Next

        Dim htParamQual As New Hashtable

        Try

            Dim dataOraUltimaLettura As DateTime = DateTime.Now

            If Id_Agenda <> 0 AndAlso Id_Mov_Det <> 0 AndAlso
                (gestitoImballaggio OrElse gestitoContenitore OrElse gestitoConfezione) Then
                dtMovimentiDettagli = objMovDetR.LeggiCaricoScarico_New(
                Piva, 0, Id_Agenda, 0, Id_Mov_Det, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, "", 0, xFiltroAggiuntivo,
                xFiltroAggiuntivo, xOrderBy, objParametri, True, True, True, False, leggiPesiRiscontrati)
            End If

            'If dtMovimentiDettagli.Rows.Count > 0 Then

            DtDati.Columns.Add(New DataColumn("key_mov_dett", GetType(String)))
            DtDati.Columns.Add(New DataColumn("Piva", GetType(String)))
            DtDati.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
            DtDati.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
            DtDati.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
            DtDati.Columns.Add(New DataColumn("key_Dest", GetType(String)))
            DtDati.Columns.Add(New DataColumn("Ubic_Des", GetType(String)))
            DtDati.Columns.Add(New DataColumn("DataOraUltimaLettura", GetType(Date)))


            If gestitoImballaggio Then
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Tipo_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Tara_Campionatura", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Sigla", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Descrizione", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Codice_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_imballaggio_Mat_Cod_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("NrImballaggi", GetType(Integer)))
            End If
            If gestitoContenitore Then
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Tipo_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Tara_Campionatura", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Sigla", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Descrizione", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Codice_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_contenitore_Mat_Cod_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("NrContenitori", GetType(Integer)))
            End If
            If gestitoConfezione Then
                DtDati.Columns.Add(New DataColumn("FF_confezione_Tipo_Cod", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_confezione_Tara_Campionatura", GetType(Decimal)))
                DtDati.Columns.Add(New DataColumn("FF_confezione_Sigla", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_confezione_Descrizione", GetType(String)))
                DtDati.Columns.Add(New DataColumn("FF_confezione_Codice_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("FF_confezione_Mat_Cod_Generazione_Link", GetType(Integer)))
                DtDati.Columns.Add(New DataColumn("NrConfezioni", GetType(Integer)))
            End If


            DtDati.Columns.Add(New DataColumn("Tara", GetType(Decimal)))

            If leggiPesiRiscontrati Then
                If gestitoImballaggio Then
                    DtDati.Columns.Add(New DataColumn("Num_Imballi_Riscontrati", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Imballo_Riscontrata", GetType(Decimal)))
                End If
                If gestitoContenitore Then
                    DtDati.Columns.Add(New DataColumn("Num_Colli_Riscontrati", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Collo_Riscontrata", GetType(Decimal)))
                End If
                If gestitoConfezione Then
                    DtDati.Columns.Add(New DataColumn("Num_Conf_Riscontrate", GetType(Integer)))
                    DtDati.Columns.Add(New DataColumn("Tara_Unit_Conf_Riscontrata", GetType(Decimal)))
                End If
            End If

            For i = 0 To dtMovimentiDettagli.Rows.Count - 1

                Dim dr As DataRow

                'Creo una nuova riga
                dr = DtDati.NewRow

                dr.Item("Piva") = dtMovimentiDettagli.Rows(i).Item("Piva")
                dr.Item("Id_Agenda") = dtMovimentiDettagli.Rows(i).Item("Id_Agenda")
                dr.Item("Id_Mov") = dtMovimentiDettagli.Rows(i).Item("Id_Mov")
                dr.Item("Id_Mov_Det") = dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det")
                dr.Item("key_Dest") = dtMovimentiDettagli.Rows(i).Item("Tipo_Destinazione").ToString & "_" & dtMovimentiDettagli.Rows(i).Item("Sa_Cod_Destinazione").ToString & "_" & dtMovimentiDettagli.Rows(i).Item("Id_Destinazione").ToString
                If Not String.IsNullOrEmpty(dtMovimentiDettagli.Rows(i).Item("Identificativo").ToString()) Then
                    ' dr.Item("insieme_des") = DtMovimenti_Dettagli.Rows(i).Item("Fabbricato_Des")
                    dr.Item("Ubic_Des") = dtMovimentiDettagli.Rows(i).Item("Identificativo")
                Else
                    dr.Item("Ubic_Des") = dtMovimentiDettagli.Rows(i).Item("Fabbricato_Des")
                End If
                dr.Item("DataOraUltimaLettura") = dataOraUltimaLettura

                Dim trovataConfezione = False
                For Each paramQual In dtParamQual.Rows
                    If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then
                        dr.Item(GetNomeColonnaTipoCod(paramQual(Tabella_Key))) = dtMovimentiDettagli.Rows(i).Item(GetNomeColonnaTipoCod(paramQual(Tabella_Key)))
                        dr.Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura) = dtMovimentiDettagli.Rows(i).Item(FF_ & paramQual(Tabella_Key) & _Tara_Campionatura)
                        dr.Item(GetNomeColonnaSigla(paramQual(Tabella_Key))) = dtMovimentiDettagli.Rows(i).Item(GetNomeColonnaSigla(paramQual(Tabella_Key)))
                        dr.Item(GetNomeColonnaDescrizione(paramQual(Tabella_Key))) = dtMovimentiDettagli.Rows(i).Item(GetNomeColonnaDescrizione(paramQual(Tabella_Key)))
                        dr.Item(FF_ & paramQual(Tabella_Key) & _Codice_Generazione_Link) = dtMovimentiDettagli.Rows(i).Item(FF_ & paramQual(Tabella_Key) & _Codice_Generazione_Link)
                        dr.Item(FF_ & paramQual(Tabella_Key) & _Mat_Cod_Generazione_Link) = dtMovimentiDettagli.Rows(i).Item(FF_ & paramQual(Tabella_Key) & _Mat_Cod_Generazione_Link)
                        If ({pq_confezione}).Contains(paramQual(Tabella_Key)) AndAlso
                            dtMovimentiDettagli.Rows(i).Item(GetNomeColonnaTipoCod(paramQual(Tabella_Key))) <> 0 Then
                            trovataConfezione = True
                        End If
                    End If
                Next

                If gestitoImballaggio AndAlso Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Qta_Dest2")) Then
                    dr.Item("NrImballaggi") = dtMovimentiDettagli.Rows(i).Item("Qta_Dest2")
                Else
                    If gestitoImballaggio Then
                        dr.Item("NrImballaggi") = 0
                    End If
                End If
                If gestitoContenitore AndAlso Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Qta_Dest1")) Then
                    dr.Item("NrContenitori") = dtMovimentiDettagli.Rows(i).Item("Qta_Dest1")
                Else
                    If gestitoContenitore Then
                        dr.Item("NrContenitori") = 0
                    End If
                End If
                If gestitoConfezione Then
                    dr.Item("NrConfezioni") = 0
                    If trovataConfezione AndAlso
                        dtMovimentiDettagli.Rows(i).Item("Udm_Cod") = enum_UnitaMisura.Numero AndAlso
                        Not IsDBNull(dtMovimentiDettagli.Rows(i).Item("Qta")) Then
                        dr.Item("NrConfezioni") = dtMovimentiDettagli.Rows(i).Item("Qta")
                    End If
                End If

                ' Non viene usata la Tara della movimenti_dettagli perché non è sempre valorizzata e può
                ' essere stata forzata dall'utente con un numero non coerente rispetto alla tara effettiva degli imballaggi
                ' Viene quindi ricalcolata come Nr Imballaggi / Nr Contenitori / Nr Confezioni * la rispettiva tara
                'dr.Item("KgLordi") = kgNetti + CDec(dtMovimentiDettagli.Rows(i).Item("Tara"))
                Dim taraTot As Decimal = 0.0
                If gestitoImballaggio AndAlso dr.Table.Columns("FF_imballaggio_Tara_Campionatura") IsNot Nothing Then
                    taraTot += CDec(dr.Item("NrImballaggi")) * CDec(dr.Item("FF_imballaggio_Tara_Campionatura"))
                End If
                If gestitoContenitore AndAlso dr.Table.Columns("FF_contenitore_Tara_Campionatura") IsNot Nothing Then
                    taraTot += CDec(dr.Item("NrContenitori")) * CDec(dr.Item("FF_contenitore_Tara_Campionatura"))
                End If
                If gestitoConfezione AndAlso dr.Table.Columns("FF_confezione_Tara_Campionatura") IsNot Nothing Then
                    taraTot += CDec(dr.Item("NrConfezioni")) * CDec(dr.Item("FF_confezione_Tara_Campionatura"))
                End If

                'If Cau_Mov = CAU_CARICO Then
                '    dr.Item("Tara_Imballaggi_Mov") = 0
                '    dr.Item("Tara_Contenitori_Mov") = 0
                '    dr.Item("Tara_Confezioni_Mov") = 0
                'End If        

                dr.Item("Tara") = dtMovimentiDettagli.Rows(i).Item("Tara")

                If leggiPesiRiscontrati Then
                    If gestitoImballaggio Then
                        dr.Item("Num_Imballi_Riscontrati") = dtMovimentiDettagli.Rows(i).Item("Num_Imballi_Riscontrati")
                        dr.Item("Tara_Unit_Imballo_Riscontrata") = dtMovimentiDettagli.Rows(i).Item("Tara_Unit_Imballo_Riscontrata")
                    End If
                    If gestitoContenitore Then
                        dr.Item("Num_Colli_Riscontrati") = dtMovimentiDettagli.Rows(i).Item("Num_Colli_Riscontrati")
                        dr.Item("Tara_Unit_Collo_Riscontrata") = dtMovimentiDettagli.Rows(i).Item("Tara_Unit_Collo_Riscontrata")
                    End If
                    If gestitoConfezione Then
                        dr.Item("Num_Conf_Riscontrate") = dtMovimentiDettagli.Rows(i).Item("Num_Conf_Riscontrate")
                        dr.Item("Tara_Unit_Conf_Riscontrata") = dtMovimentiDettagli.Rows(i).Item("Tara_Unit_Conf_Riscontrata")
                    End If
                End If

                dr.Item("key_mov_dett") = dr.Item("piva") & "_" &
                                          dr.Item("Id_Agenda") & "_" &
                                          dr.Item("Id_Mov") & "_" &
                                          dr.Item("Id_Mov_Det") & "_" &
                                          dr.Item("key_Dest")

                If (gestitoImballaggio AndAlso dr.Item("NrImballaggi") <> 0) OrElse
                    (gestitoContenitore AndAlso dr.Item("NrContenitori") <> 0) OrElse
                    (gestitoConfezione AndAlso dr.Item("NrConfezioni") <> 0) Then
                    DtDati.Rows.Add(dr)
                End If

            Next

            'End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
        End Try


        '-------------------------------------------------------------------------------------------------------------------------------------------
        '----------    CREO LISTA COLONNE DA VISUALIZZARE
        '-------------------------------------------------------------------------------------------------------------------------------------------

        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' Chiave riga
        c = New ColonneNome("key_mov_dett", "key_mov_dett", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DataOraUltimaLettura", "DataOraUltimaLettura", "date")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("key_Dest", "key_Dest", "string")
        c._gruppoColonne = ""
        If movimentiDaGiacenza Then
            c._hidden = True
        Else
            c._hidden = True
            c._daDuplicare = True
            c._Editabile = True
            c._obbligatorio = True
        End If
        l.Add(c)

        c = New ColonneNome("Ubic_Des", "Mag. o Cella", "string")
        If movimentiDaGiacenza Then
            c._Editabile = False
            c._Display = False
            c._Filtrabile = True
            c._width = "73px"
            c._FiltrabileConCheck = True
        Else
            c._obbligatorio = True
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
        End If
        l.Add(c)


        'Descrizione imballo/contenitore/confezione
        For Each paramQual In dtParamQual.Rows

            If IsParamQualConfezionamento(paramQual(Tabella_Key)) Then

                c = New ColonneNome(GetNomeColonnaTipoCod(paramQual(Tabella_Key)), paramQual(Tabella_Key) & _Tipo_Cod, "number")
                c._gruppoColonne = gruppoProdottoGrigliaImballaggi
                c._hidden = True
                c._Editabile = True
                c._daDuplicare = True
                l.Add(c)

            End If

        Next

        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Descrizione", "Imballaggio", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Sigla", "Sigla Imballaggio", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Codice_Generazione_Link", "FF_imballaggio_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("FF_imballaggio_Mat_Cod_Generazione_Link", "FF_imballaggio_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)

        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Descrizione", "Contenitore", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Sigla", "Sigla Contenitore", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Codice_Generazione_Link", "FF_contenitore_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("FF_contenitore_Mat_Cod_Generazione_Link", "FF_contenitore_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Descrizione", "Confezione", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            'End If
            l.Add(c)

            c = New ColonneNome("FF_confezione_Sigla", "Sigla Confezione", "string")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            If movimentiDaGiacenza Then
                c._Editabile = False
                c._Display = True
                c._Filtrabile = True
                c._FiltrabileConCheck = True
            Else
                c._daDuplicare = True
                c._hidden = True
                c._Editabile = True
            End If
            'End If
            l.Add(c)

            c = New ColonneNome("FF_confezione_Codice_Generazione_Link", "FF_confezione_Codice_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)

            c = New ColonneNome("FF_confezione_Mat_Cod_Generazione_Link", "FF_confezione_Mat_Cod_Generazione_Link", "number")
            c._gruppoColonne = gruppoProdottoGrigliaImballaggi
            c._daDuplicare = True
            c._hidden = True
            c._Editabile = True
            l.Add(c)
        End If

        'Nr

        If gestitoImballaggio Then
            c = New ColonneNome("NrImballaggi", "Nr Imb.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "70px"
            c._sum = True
            c._stringaTotale = ""
            c._Editabile = True
            l.Add(c)
        End If

        If gestitoContenitore Then
            c = New ColonneNome("NrContenitori", "Nr Cont.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "73px"
            c._sum = True
            c._stringaTotale = ""
            c._Editabile = True
            l.Add(c)
        End If

        If gestitoConfezione Then
            c = New ColonneNome("NrConfezioni", "Nr Conf.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._formatNr = "n0"
            c._Filtrabile = True
            c._Display = True
            c._width = "73px"
            c._sum = True
            c._stringaTotale = ""
            c._Editabile = True
            l.Add(c)
        End If

        'Tare
        If gestitoImballaggio Then
            c = New ColonneNome("FF_imballaggio_Tara_Campionatura", "Tara Un. Imb.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n1"
            c._width = "73px"
            'c._sum = True
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If Not movimentiDaGiacenza Then
                c._daDuplicare = True
            End If
            l.Add(c)

        End If

        If gestitoContenitore Then
            c = New ColonneNome("FF_contenitore_Tara_Campionatura", "Tara Un. Cont.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n3"
            c._width = "73px"
            'c._sum = True
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If Not movimentiDaGiacenza Then
                c._daDuplicare = True
            End If
            l.Add(c)

        End If

        If gestitoConfezione Then
            c = New ColonneNome("FF_confezione_Tara_Campionatura", "Tara Un. Conf.", "number")
            c._gruppoColonne = gruppoQuantitaGrigliaImballaggi
            c._Filtrabile = True
            c._Display = True
            c._formatNr = "n3"
            c._width = "73px"
            'c._sum = True
            If movimentiDaGiacenza Then
                c._Editabile = False
            Else
                c._Editabile = True
            End If
            If Not movimentiDaGiacenza Then
                c._daDuplicare = True
            End If
            l.Add(c)

        End If

        'Riscontrati
        If leggiPesiRiscontrati Then
            If gestitoImballaggio Then
                If isVendita Then
                    c = New ColonneNome("Num_Imballi_Riscontrati", "Imballaggi riscontrati", "number")
                Else
                    c = New ColonneNome("Num_Imballi_Riscontrati", "Imballaggi dichiarati", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n0"
                c._valueDefault = "null"
                l.Add(c)

                If isVendita Then
                    c = New ColonneNome("Tara_Unit_Imballo_Riscontrata", "Tara imb. risc.", "number")
                Else
                    c = New ColonneNome("Tara_Unit_Imballo_Riscontrata", "Tara imb. dich.", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                c._obbligatorio = False
                c._width = "85px"
                c._formatNr = "n3"
                c._valueDefault = "null"
                l.Add(c)
            End If

            If gestitoContenitore Then
                If isVendita Then
                    c = New ColonneNome("Num_Colli_Riscontrati", "Contenitori riscontrati", "number")
                Else
                    c = New ColonneNome("Num_Colli_Riscontrati", "Contenitori dichiarati", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n0"
                c._valueDefault = "null"
                l.Add(c)

                If isVendita Then
                    c = New ColonneNome("Tara_Unit_Collo_Riscontrata", "Tara cont. risc.", "number")
                Else
                    c = New ColonneNome("Tara_Unit_Collo_Riscontrata", "Tara cont. dich.", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                c._obbligatorio = False
                c._width = "85px"
                c._formatNr = "n3"
                c._valueDefault = "null"
                l.Add(c)
            End If

            If gestitoConfezione Then
                If isVendita Then
                    c = New ColonneNome("Num_Conf_Riscontrate", "Confez. riscontrate", "number")
                Else
                    c = New ColonneNome("Num_Conf_Riscontrate", "Confez. dichiarati", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                'c._sum = True
                c._stringaTotale = ""
                c._obbligatorio = False
                c._width = "75px"
                c._formatNr = "n0"
                c._valueDefault = "null"
                l.Add(c)

                If isVendita Then
                    c = New ColonneNome("Tara_Unit_Conf_Riscontrata", "Tara confez. risc.", "number")
                Else
                    c = New ColonneNome("Tara_Unit_Conf_Riscontrata", "Tara confez. dich.", "number")
                End If
                If lavCodDDTVendita.Contains(lav_cod_chiamante) Then
                    c._Display = True
                Else
                    c._hidden = True
                End If
                c._gruppoColonne = gruppoPesoRiscontratoGrigliaImballaggi
                c._daDuplicare = True
                c._Editabile = True
                c._Filtrabile = True
                c._obbligatorio = False
                c._width = "85px"
                c._formatNr = "n3"
                c._valueDefault = "null"
                l.Add(c)
            End If
        End If
        Dim js As New JSON_DataTable
        js.Editabile_Deafault = False
        risposta = js.JSON_DataTable_Kendo(DtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
        Return risposta

    End Function


    Public Function Impostazione_LottoProdotto(ByVal piva As String,
                                               ByVal sa_cod As Integer,
                                               ByVal modulo_generazione As Integer,
                                               ByVal mat_cod As Integer,
                                               ByVal cod_contatto As String,
                                               ByVal cod_risum As Integer,
                                               ByVal doc_numero_sin As String,
                                               ByVal doc_numero As Integer,
                                               ByVal doc_numero_des As String,
                                               ByVal doc_numero_sin_accettazione As String,
                                               ByVal doc_numero_accettazione As Integer,
                                               ByVal doc_numero_des_accettazione As String,
                                               ByVal data_ingresso As DateTime,
                                               ByVal qualita_cod As Integer,
                                               ByVal destinazioni_cod As String,
                                               ByVal appezzamenti As String,
                                               ByVal lotti_impianti As String,
                                               ByVal certificato_cod As Integer,
                                               ByVal contatore_univoco_parametri As String,
                                               ByVal sigla_certificazione As String,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef ErrCode As Integer
                                               ) As String

        Const nomeRoutine = "FF_MagazzinoBIZ.Impostazione_LottoProdotto()"
        Dim messaggioErrore As String = String.Empty

        Dim DT_Prodotto_Log As DataTable
        Dim DT_Materie_Prime As DataTable
        Dim DT_Linea_Log As DataTable
        Dim DT_Modulo_Log As DataTable
        Dim DT_Contatto As DataTable
        Dim DT_Assegna As DataTable
        Dim DT_Proprieta As DataTable
        Dim DT_Codice As DataTable

        Dim DT_Core As DataTable
        Dim DR_Search As DataRow

        Dim leggi_core As Object
        Dim leggi_core2 As Object


        Dim Lotto As String = ""
        Dim Lotto_Configurazione As String = ""
        Dim Lotto_Separatore As String = ""
        Dim Lotto_Socio As String
        Dim contatore_univoco_carico As Integer
        Dim Arrayp() As String
        Dim Arrayr() As String
        Dim Arrays() As String
        Dim i As Integer
        Dim j As Integer

        Dim bOk As Boolean
        'Dim RsProprieta As ADODB.Recordset
        Dim Lotto_Val As String

        Dim Sigla As String
        Dim Cod_Articolo As String = ""
        Dim Linea_Cod As Long = 0
        Dim Linea_Cod_Des As String = ""
        Dim Giorno_Giuliano As Integer
        Dim Numero_Settimana As Integer
        Dim Veg_Cod_Prodotto As Long = 0
        Dim Cul_Cod_Prodotto As Long = 0
        Dim Filtro_Aggiuntivo As String = ""
        Dim Errore As String = ""
        Dim Risultato As String


        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Try

            Dim leggi_Omni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Log_R
            Dim leggi_LottoxRisum As New AgronicaCoreAnagrafeDAL.Lotto_AssegnaxRisumSpeVarQualCert_DAL_R
            Dim leggi_Materie_LC As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
            Dim leggi_Materie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim leggi_Centri_Codici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
            Dim leggi_Appezzamenti_Codici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R


            'Lettura Log Modulo
            DT_Modulo_Log = leggi_Omni.LeggiOmniLog_Modulo(piva, modulo_generazione, "", objParametri_Server)

            'Lettura Prodotto x Vedere se è referenza
            DT_Materie_Prime = leggi_Materie_Prime.Leggi2(piva, 0, mat_cod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If DT_Materie_Prime.Rows.Count > 0 Then

                Cod_Articolo = DT_Materie_Prime(0).Item("Cod_Articolo")

                Veg_Cod_Prodotto = DT_Materie_Prime(0).Item("Veg_Cod")
                Cul_Cod_Prodotto = DT_Materie_Prime(0).Item("Cul_Cod")

                If DT_Materie_Prime(0).Item("Mat_Cod_Referenza") IsNot DBNull.Value AndAlso DT_Materie_Prime(0).Item("Mat_Cod_Referenza") <> 0 Then

                    'Impostazione della referenza sul log omni
                    mat_cod = DT_Materie_Prime(0).Item("Mat_Cod_Referenza")

                End If

            End If


            'Lettura Log Prodotto
            DT_Prodotto_Log = leggi_Omni.LeggiOmniLog_Prodotto(piva, modulo_generazione, 0, 0, 0, 0, mat_cod, "", objParametri_Server)

            'Lettura Log Prodotto x Linea
            DT_Linea_Log = leggi_Omni.Leggi_Linee_Produzione_Log(piva, modulo_generazione, 0, 0, 0, 0, mat_cod, "", objParametri_Server)

            If DT_Modulo_Log.Rows.Count > 0 Then

                Lotto_Configurazione = If(IsDBNull(DT_Modulo_Log(0).Item("Lotto_Configurazione")), "", DT_Modulo_Log(0).Item("Lotto_Configurazione"))
                Lotto_Separatore = If(IsDBNull(DT_Modulo_Log(0).Item("Separatore_Lotto")), "", DT_Modulo_Log(0).Item("Separatore_Lotto"))

            End If


            If DT_Linea_Log.Rows.Count > 0 AndAlso DT_Prodotto_Log.Rows.Count > 0 Then

                Linea_Cod = DT_Linea_Log(0).Item("Linea_Cod")
                Linea_Cod_Des = DT_Linea_Log(0).Item("Linea_Cod_Des")


                'Cod_Articolo = DT_Prodotto_Log(0).Item("Cod_Articolo")


                Lotto_Configurazione = If(IsDBNull(DT_Linea_Log(0).Item("Lotto_Configurazione")), "", DT_Linea_Log(0).Item("Lotto_Configurazione"))
                Lotto_Separatore = If(IsDBNull(DT_Linea_Log(0).Item("Separatore_Lotto")), "", DT_Linea_Log(0).Item("Separatore_Lotto"))


                Select Case CStr(Lotto_Configurazione)

                    Case "", "NULL" 'Generale 

                        Lotto_Configurazione = If(IsDBNull(DT_Modulo_Log(0).Item("Lotto_Configurazione")), "", DT_Modulo_Log(0).Item("Lotto_Configurazione"))
                        Lotto_Separatore = If(IsDBNull(DT_Modulo_Log(0).Item("Separatore_Lotto")), "", DT_Modulo_Log(0).Item("Separatore_Lotto"))


                    Case Else



                End Select

            End If

            ''Test
            'Lotto_Configurazione = "20|12|19"
            'Lotto_Separatore = ""

            If Trim(Lotto_Configurazione) <> "" Then

                '==============================================================================================
                'Controllo Configurazione
                '----------------------------------------------------------------------------------------------
                Arrayp = Split(Lotto_Configurazione & "|", "|")

                For i = 0 To UBound(Arrayp, 1) - 1

                    If IsNumeric(Arrayp(i)) Then

                        Select Case CInt(Arrayp(i))

                            Case enLotto_Config_FF.lcff_CODICE_FORNITORE

                                bOk = False

                                If cod_risum <> 0 Then

                                    'Lettura Contatto
                                    Dim leggi_contatto As New AgronicaCoreAnagrafeDAL.Contatti_R

                                    DT_Contatto = leggi_contatto.Leggi(piva, "", cod_risum, 0, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri_Server)


                                    If DT_Contatto.Rows.Count > 0 Then

                                        If Trim(Lotto) = "" Then
                                            Lotto = DT_Contatto(0).Item("Settore_Des")
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & DT_Contatto(0).Item("Settore_Des")
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = "Attenzione. Fornitore non impostato correttamente."

                                End If


                            Case enLotto_Config_FF.lcff_DATA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Format(CDate(data_ingresso), "yyyyMMdd")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Format(CDate(data_ingresso), "yyyyMMdd")
                                    End If

                                Else

                                    Errore = "Attenzione. Data ingresso non impostata correttamente."

                                End If




                            Case enLotto_Config_FF.lcff_ANNO_AA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Format(CDate(data_ingresso), "yy")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Format(CDate(data_ingresso), "yy")
                                    End If

                                Else

                                    Errore = "Attenzione. Data ingresso non impostata correttamente."

                                End If


                            Case enLotto_Config_FF.lcff_ORA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Format(CDate(data_ingresso), "hh") & Lotto_Separatore & Format(CDate(data_ingresso), "mm")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Format(CDate(data_ingresso), "HH") & Lotto_Separatore & Format(CDate(data_ingresso), "mm")
                                    End If

                                Else

                                    Errore = "Attenzione. Data ingresso non impostata correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_NUMERO_SETTIMANA

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    'Numero_Settimana = Weekday(CDate(data_ingresso))

                                    Dim dfi = Globalization.DateTimeFormatInfo.CurrentInfo
                                    Numero_Settimana = Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, dfi.CalendarWeekRule, DayOfWeek.Monday)

                                    If Trim(Lotto) = "" Then
                                        Lotto = Format(Numero_Settimana, "00")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Format(Numero_Settimana, "00")
                                    End If

                                Else

                                    Errore = "Attenzione. Data ingresso non impostata correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_GIORNO_GIULIANO

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    Giorno_Giuliano = DateDiff("d", "01/01/" & Year(CDate(data_ingresso)), CDate(data_ingresso)) + 1

                                    If Trim(Lotto) = "" Then
                                        Lotto = Format(Giorno_Giuliano, "000")
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Format(Giorno_Giuliano, "000")
                                    End If

                                Else

                                    Errore = "Attenzione. Data ingresso non impostata correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_TABELLA_ASSEGNA

                                bOk = False

                                If DT_Prodotto_Log.Rows.Count <> 0 Then

                                    Veg_Cod_Prodotto = DT_Prodotto_Log(0).Item("Veg_Cod")
                                    Cul_Cod_Prodotto = DT_Prodotto_Log(0).Item("Cul_Cod")

                                    bOk = True

                                End If

                                If bOk Then

                                    bOk = False

                                    Filtro_Aggiuntivo = "Cod_Risum = " & cod_risum &
                                                            " And Veg_Cod = " & Veg_Cod_Prodotto &
                                                            " And Cul_Cod = " & Cul_Cod_Prodotto &
                                                            " And Qual_Cod = " & qualita_cod &
                                                            " And Cert_Cod = " & certificato_cod

                                    DT_Assegna = leggi_LottoxRisum.Leggi_Lotto_AssegnaxRisumSpeVarQualCert(piva, Filtro_Aggiuntivo, objParametri_Server)

                                    If DT_Assegna.Rows.Count > 0 Then

                                        For Each dr As DataRow In DT_Assegna.Rows

                                            'Contro Validita
                                            If CDate(data_ingresso) <= CDate(dr("Validita_Fine")) AndAlso CDate(data_ingresso) >= CDate(dr("Validita_Inizio")) Then

                                                If Trim(Lotto) = "" Then
                                                    Lotto = dr("Lotto")
                                                Else
                                                    Lotto = Lotto & Lotto_Separatore & dr("Lotto")
                                                End If

                                                bOk = True

                                            End If

                                        Next

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = "Attenzione. Lotto non riconosciuto (Ragione sociale = " & cod_risum & "|Specie Vegetale = " & Veg_Cod_Prodotto & "|Varietà = " & Cul_Cod_Prodotto & "|Qualità = " & qualita_cod & "|Certificato = " & certificato_cod & ")" & Chr(13) & "Contattare il responsabile della configurazione lotti."

                                End If


                            Case enLotto_Config_FF.lcff_LOTTO_IMPIANTO

                                If Trim(lotti_impianti) <> "" Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = lotti_impianti
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & lotti_impianti
                                    End If


                                End If



                            Case enLotto_Config_FF.lcff_AZIENDA

                                bOk = False

                                If cod_contatto <> "" Then

                                    DT_Proprieta = leggi_Materie_LC.Leggi_Proprieta_Aziende(piva, 0, mat_cod, cod_contatto, CDate(data_ingresso), CDate(data_ingresso), objParametri_Server)

                                    If DT_Proprieta.Rows.Count > 0 Then

                                        Lotto_Val = DT_Proprieta(0).Item("Lotto_Val1")

                                        If Trim(Lotto) = "" Then
                                            Lotto = Lotto_Val
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & Lotto_Val
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = "Attenzione. Proprietà azienda non impostata correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_DOCUMENTO

                                bOk = False

                                If IsNumeric(doc_numero) Then

                                    If CLng(doc_numero) <> 0 Then

                                        If Trim(Lotto) = "" Then
                                            Lotto = doc_numero_sin & Format(CLng(doc_numero), "0000") & doc_numero_des
                                        Else
                                            Lotto = Lotto & Lotto_Separatore & Trim(doc_numero_sin & Format(CLng(doc_numero), "0000") & doc_numero_des)
                                        End If

                                        bOk = True

                                    End If

                                End If

                                If Not bOk Then

                                    Errore = "Attenzione. Numero DDT non impostato correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_CODICE_DESTINAZIONE

                                Arrays = Split(destinazioni_cod, "|")

                                For j = 0 To UBound(Arrays) - 1

                                    If Trim(Lotto) = "" Then
                                        Lotto = Arrays(j)
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Arrays(j)
                                    End If

                                Next



                            Case enLotto_Config_FF.lcff_SIGLA_SPECIE_VARIETA

                                bOk = False

                                If DT_Prodotto_Log.Rows.Count <> 0 Then

                                    Veg_Cod_Prodotto = DT_Prodotto_Log(0).Item("Veg_Cod")
                                    Cul_Cod_Prodotto = DT_Prodotto_Log(0).Item("Cul_Cod")

                                End If

                                leggi_core = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                                leggi_core2 = New AgronicaCoreMetaSchemaDAL.Cultivar_R

                                Sigla = UCase(Trim(Left(leggi_core.VegDes_from_VegCod(Veg_Cod_Prodotto, objParametri_Server), 1))) &
                                            UCase(Trim(Left(leggi_core2.CulDes_from_CulCod(Cul_Cod_Prodotto, objParametri_Server), 1)))

                                If Trim(Lotto) = "" Then
                                    Lotto = Sigla
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Sigla
                                End If

                                bOk = True


                                'If Not bOk Then

                                '        Errore = "Attenzione. Specie vegetale/varietà colturale non impostata correttamente."

                                '    End If


                            Case enLotto_Config_FF.lcff_CONTATORE_UNIVOCO

                                'Creazione Contatore Univoco
                                Dim sequenza = New Sequenza_Progressivi_R
                                contatore_univoco_carico = sequenza.Nuovo_Progressivo_UpdateImmediato(piva, Year(data_ingresso), 19, "", "", 0, objParametri_Server)

                                If Trim(Lotto) = "" Then
                                    Lotto = Format(contatore_univoco_carico, "000000")
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Format(contatore_univoco_carico, "000000")
                                End If


                            Case enLotto_Config_FF.lcff_ACCETTAZIONE


                                'Generazione nuovo codice sospeso in attesa di conferma attivazione


                                'If doc_numero_accettazione = 0 Then

                                '    Dim sequenza = New Sequenza_Progressivi_R
                                '    doc_numero_accettazione = sequenza.Nuovo_Progressivo_UpdateImmediato(piva, Year(data_ingresso), 5, "", "", 0, objParametri_Server)

                                'End If


                                If doc_numero_accettazione <> 0 Then

                                    If Trim(Lotto) = "" Then
                                        Lotto = Trim(doc_numero_sin_accettazione & Format(doc_numero_accettazione, "0000") & doc_numero_des_accettazione)
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Trim(doc_numero_sin_accettazione & Format(doc_numero_accettazione, "0000") & doc_numero_des_accettazione)
                                    End If

                                Else

                                    Errore = "Attenzione. Numero accettazione non impostato correttamente."

                                End If




                            Case enLotto_Config_FF.lcff_CODICE_LINEA


                                If Trim(Lotto) = "" Then
                                    Lotto = Linea_Cod_Des
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Linea_Cod_Des
                                End If



                            Case enLotto_Config_FF.lcff_CODICE_ARTICOLO

                                If Trim(Lotto) = "" Then
                                    Lotto = Cod_Articolo
                                Else
                                    Lotto = Lotto & Lotto_Separatore & Cod_Articolo
                                End If





                            Case enLotto_Config_FF.lcff_CONTATORE_UNIVOCO_PARAMETRI


                                If Trim(Lotto) = "" Then
                                    Lotto = contatore_univoco_parametri
                                Else
                                    Lotto = Lotto & Lotto_Separatore & contatore_univoco_parametri
                                End If



                            Case enLotto_Config_FF.lcff_ANNO_SOCIO_LINEA_GAP

                                bOk = False

                                If CDate(data_ingresso) <> AGRODATAINIZIO Then

                                    'Anno
                                    Lotto_Socio = Format(CDate(data_ingresso), "yy")

                                    'Lettura Contatto
                                    Dim leggi_contatto As New AgronicaCoreAnagrafeDAL.Contatti_R

                                    DT_Contatto = leggi_contatto.Leggi(piva, "", cod_risum, 0, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, "", "", objParametri_Server)


                                    If DT_Contatto.Rows.Count > 0 Then

                                        'Socio
                                        Lotto_Socio = Lotto_Socio & Lotto_Separatore &
                                                          If(DT_Contatto(0).Item("Cod_Rapporto") = enum_Rapporti_Contabili_Standard.Conferente, "C", "A") &
                                                          Format(Right("000" & DT_Contatto(0).Item("Settore_Des"), 3), "000")

                                        'Linea
                                        Lotto_Socio = Lotto_Socio & Lotto_Separatore & Linea_Cod_Des

                                        bOk = True

                                    End If


                                    'Certificazione
                                    Lotto_Socio = Lotto_Socio & Lotto_Separatore & sigla_certificazione


                                    If Trim(Lotto) = "" Then
                                        Lotto = Lotto_Socio
                                    Else
                                        Lotto = Lotto & Lotto_Separatore & Lotto_Socio
                                    End If


                                End If

                                If Not bOk Then

                                    Errore = "Attenzione. Codice certificazione socio non impostato correttamente."

                                End If



                            Case enLotto_Config_FF.lcff_CODICE_APPEZZAMENTO

                                'appezzamenti = "00045060597|132775937|132775961"

                                Arrayr = Split(appezzamenti & "*", "*")

                                For j = 0 To UBound(Arrayr) - 1

                                    Arrays = Split(Arrayr(j), "|")

                                    Dim piva_appezzamento As String = Arrays(0)
                                    Dim sa_cod_appezzamento As Integer = Arrays(1)
                                    Dim appezza As Integer = Arrays(2)


                                    'Lettura Appezzamento
                                    DT_Codice = leggi_Appezzamenti_Codici.Leggi(piva_appezzamento, sa_cod_appezzamento, appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)


                                    If DT_Codice.Rows.Count > 0 Then

                                        For Each dr As DataRow In DT_Codice.Rows

                                            If Trim(Lotto) = "" Then
                                                Lotto = dr("Val_Cod")
                                            Else
                                                Lotto = Lotto & Lotto_Separatore & dr("Val_Cod")
                                            End If



                                        Next

                                    End If

                                Next



                            Case enLotto_Config_FF.lcff_CODICE_SITO_PRODUZIONE

                                If sa_cod <> 0 Then

                                    DT_Codice = leggi_Centri_Codici.Leggi(piva, sa_cod, enum_CodiciAnagrafe.Codice_Sito_Vivaio, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                                    If DT_Codice.Rows.Count > 0 Then

                                        For Each dr As DataRow In DT_Codice.Rows

                                            If Trim(Lotto) = "" Then
                                                Lotto = dr("Val_Cod")
                                            Else
                                                Lotto = Lotto & Lotto_Separatore & dr("Val_Cod")
                                            End If

                                        Next

                                    End If

                                End If


                        End Select

                    End If

                Next i



                '============================================================================================================
                'Controllo impostazione nulla e creazione di un lotto univoco (data e ora)
                '------------------------------------------------------------------------------------------------------------
                If Trim(Errore) <> "" Then

                    Risultato = Errore
                    ErrCode = 1

                Else

                    If Trim(Lotto) = "" Then

                        'Lotto non Impostato --> ritorno data
                        Lotto = Format(Now, "yyMMdd")
                        'Lotto = Format(Now, "YYMMDDHHnnSS")

                    End If


                    Risultato = Lotto
                    ErrCode = 0


                End If



            Else

                Risultato = "Lotto Configurazione Non Impostato Correttamente o Prodotto Mancante."
                ErrCode = 2

            End If


            Return Risultato

        Catch ex As Exception
            messaggioErrore = "[" & nomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return messaggioErrore

    End Function



    Public Function Leggi_Trasferimenti(ByVal Piva As String,
                                        ByVal Id_Agenda As Integer,
                                        ByVal Id_Mov As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal Des_Lib_Agenda As String,
                                        ByVal Validita_Inizio As String,
                                        ByVal Validita_Fine As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As String

        Dim risposta As String = ""
        Dim dtDati = New DataTable

        Const nomeRoutine = "FF_MagazzinoBIZ.Leggi_Trasferimenti()"

        Dim messaggioErrore As String = ""

        Dim i As Integer

        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_R

        Dim dtMovimenti As DataTable
        Dim dataRicercaDal As Date
        Dim dataRicercaAl As Date
        If Validita_Inizio = "" Then
            dataRicercaDal = AGRODATAINIZIO
        Else
            dataRicercaDal = CDate(Validita_Inizio)
        End If

        If Validita_Fine = "" Then
            dataRicercaAl = AGRODATAFINE
        Else
            dataRicercaAl = CDate(Validita_Fine)
        End If

        Dim xFiltroAggiuntivo As String = ""
        xFiltroAggiuntivo = xFiltroAggiuntivo & "  Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(dataRicercaDal, False)
        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(dataRicercaAl, False)
        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Movimenti.Cau_Mov = '" & CAU_SCARICO & "' "
        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Agenda.Lav_Cod = " & LAVCOD_TRASFERIMENTO.ToString & " "
        If Not String.IsNullOrEmpty(Des_Lib_Agenda) Then
            xFiltroAggiuntivo = xFiltroAggiuntivo & " And Agenda.Des_Lib like '%" & Des_Lib_Agenda & "%'"
        End If

        Dim xOrderBy As String = " Movimenti.Data_Movimento, Agenda.Des_Lib "

        Try

            objMovimenti = New AgronicaCoreContabDAL.Movimenti_R
            dtMovimenti = objMovimenti.Leggi(Piva,
                                             0,
                                             Id_Agenda,
                                             0,
                                             0,
                                             "",
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             xFiltroAggiuntivo,
                                             xOrderBy,
                                             objParametri)

            If dtMovimenti.Rows.Count > 0 Then

                dtDati.Columns.Add(New DataColumn("Piva", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Sa_Cod", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Des_Lib", GetType(String)))

                Dim IdMovScarico As Integer = 0
                Dim IdMovCarico As Integer = 0

                'Effettuo un ciclo sui movimenti
                For i = 0 To dtMovimenti.Rows.Count - 1

                    Dim dr As DataRow

                    'Creo una nuova riga
                    dr = dtDati.NewRow
                    dr.Item("Piva") = dtMovimenti.Rows(i).Item("Piva")
                    dr.Item("Sa_Cod") = dtMovimenti.Rows(i).Item("Sa_Cod")
                    dr.Item("Id_Agenda") = dtMovimenti.Rows(i).Item("Id_Agenda")
                    dr.Item("Id_Mov") = dtMovimenti.Rows(i).Item("Id_Mov")
                    dr.Item("Data_Movimento") = dtMovimenti.Rows(i).Item("Data_Movimento")
                    dr.Item("Des_Lib") = dtMovimenti.Rows(i).Item("Des_Lib")
                    Des_Lib_Agenda = dtMovimenti.Rows(i).Item("Des_Lib")
                    dtDati.Rows.Add(dr)

                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Id_Agenda", "Id_Agenda", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Movimento", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Data, "date")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Des_Lib", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Descrizione, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable With {.Editabile_Deafault = False}
        risposta = js.JSON_DataTable_Kendo(dtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        Return risposta

    End Function

    Private Function ComponiUdmDes(UdmDes As String,
                                   UdmSim As String) As String

        Dim UdmDesComposta As String

        If UdmSim <> "" AndAlso UdmSim <> UdmDes Then
            UdmDesComposta = String.Format("{0} ({1})", UdmDes, UdmSim)
        Else
            UdmDesComposta = UdmDes
        End If

        Return UdmDesComposta

    End Function

End Class

