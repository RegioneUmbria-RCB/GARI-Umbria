Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgroAgenda_2010.Resources
Imports Newtonsoft.Json.Linq

Public Class DocContabileDettagliUC
    Inherits System.Web.UI.UserControl

    '----- Inizio Gestione Querystring 
    Dim Qs_Rag_Soc As String

    'Querystring utilizzati per chiamata diretta da altre funzioni
    Dim Qs_Key As String
    Dim Qs_ElemCod As Integer
    Dim Qs_OraSelezionata As String
    Dim Qs_Mode As String
    Dim Qs_Tipo As String
    Dim Qs_CodContatto As String
    '----- Fine Gestione Querystring


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public Sub New()

    End Sub

    '####################################################################################################################
    Public Shared Function Carica_Giacenze(ByVal w_piva As String, ByVal w_Elem_Cod As Integer, ByVal w_Udm As Integer, ByVal w_Udm_Desc As String, ByVal w_ChkLottoImpianto As Boolean, ByVal w_Lotto As String,
                                           ByVal w_Prodotto_Cod As String, ByVal w_CauMov As String, ByVal w_Lotto_Accettazione As String, ByVal w_Cal_Cod As Integer,
                                           ByVal w_Ubic_Provenienza As String, ByVal w_Ubic_Destinazione As String, ByVal w_Chk_ParametroQualitativo As Boolean,
                                           ByVal Operazione As Integer,
                                           ByVal DataMovimento As String,
                                           ByVal Qta_Mask As Decimal,
                                           ByVal KgLordi_Mask As Decimal, ByVal KgNetti_Mask As Decimal, ByVal Imballaggi_Mask As Integer, ByVal Contenitori_Mask As Integer, ByVal Confezioni_Mask As Integer,
                                           ByVal modulo_anagrafe_log As Integer,
                                           ByVal objParametri_Server As AgronicaCoreParametri,
                                           ByVal objParametri_Utenti As AgronicaCoreParametri,
                                           ByVal Flag_QtaNoZero As Boolean,
                                           ByVal Flag_QtaMaggioreZero As Boolean,
                                           ByVal w_key_idMovDet As Integer,
                                           ByVal FF_gest_materiale_vivaistico As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim stringaFilter = ""
        If w_key_idMovDet <> 0 Then
            stringaFilter = " And Movimenti_Dettagli.Id_Mov_Det <> " & w_key_idMovDet & " "
        End If
        Dim Dt_Giacenze As DataTable = Nothing
        Dim objG As New AgronicaCoreContabDAL.Giacenze_R
        Dim Dt_Return As DataTable = Nothing

        Dim isFreshAndFoodZooTabacco As Boolean = False
        Dim elemCod_gestisce_calCod As Boolean = False
        Dim dtParamQual As DataTable
        Dim gestitoImballaggio As Boolean = False
        Dim gestitoContenitore As Boolean = False
        Dim gestitoConfezione As Boolean = False

        If (w_Elem_Cod = TRASFORMATI_VEGETALI OrElse
            w_Elem_Cod = TRASFORMATI_ANIMALI) AndAlso
            (modulo_anagrafe_log = enum_Omni_Modulo_Generazione.FreshFood OrElse
            modulo_anagrafe_log = enum_Omni_Modulo_Generazione.Tabacco OrElse
            modulo_anagrafe_log = enum_Omni_Modulo_Generazione.Zoo) Then
            isFreshAndFoodZooTabacco = True
        End If

        'A prescindere dai moduli F&F le seguenti categorie di prodotto posso essere caricate
        'con movimentazioni di magazzino che utilizzano il cal_cod, generalmente le raccolte
        If {TRASFORMATI_VEGETALI, SEMILAVORATI_VEGETALI}.Contains(w_Elem_Cod) Then
            elemCod_gestisce_calCod = True
        End If

        If isFreshAndFoodZooTabacco Then
            Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            dtParamQual = objConfigDettagli.Leggi(w_piva, 0, False, "Tipo = 1", "", objParametri_Server)
            For Each paramQual In dtParamQual.Rows
                If (paramQual("Tabella_Key") = "imballaggio") Then
                    gestitoImballaggio = True
                End If
                If (paramQual("Tabella_Key") = "contenitore") Then
                    gestitoContenitore = True
                End If
                If (paramQual("Tabella_Key") = "confezione") Then
                    gestitoConfezione = True
                End If
            Next
        End If

        Dim Tipo_Fabbricato, Id_Magazzino, Sa_Cod, Mag_Index As Integer
        Dim Pro_Cod, Mat_Cod As Integer
        Dim xCod_Progetto As String = ""
        Dim x_Cal_Cod As Integer

        Dim Qta_Risultato_Giacenza_Provenienza As String = 0
        Dim KgLordi_Risultato_Giacenza_Provenienza As Decimal = 0
        Dim KgNetti_Risultato_Giacenza_Provenienza As Decimal = 0
        Dim Imballaggi_Risultato_Giacenza_Provenienza As Integer = 0
        Dim Contenitori_Risultato_Giacenza_Provenienza As Integer = 0
        Dim Confezioni_Risultato_Giacenza_Provenienza As Integer = 0

        Dim Qta_Risultato_Giacenza_Destinazione As String = 0
        Dim KgLordi_Risultato_Giacenza_Destinazione As Decimal = 0
        Dim KgNetti_Risultato_Giacenza_Destinazione As Decimal = 0
        Dim Imballaggi_Risultato_Giacenza_Destinazione As Integer = 0
        Dim Contenitori_Risultato_Giacenza_Destinazione As Integer = 0
        Dim Confezioni_Risultato_Giacenza_Destinazione As Integer = 0

        Dim StrGiacenza As String = ""
        Dim Flag_GiacenzaEsiste As Boolean
        Dim xLotto As String = ""

        ' ----------- RISULTATO INIZIO ------------
        Dim Qta_Giacenza_Magazzino As Decimal = 0

        Dim StrGiacenza_Provenienza As String = ""
        Dim StrGiacenza_Destinazione As String = ""

        Dim messaggioErrore As String = ""
        Dim permettiSalvataggio As Boolean = True
        ' ----------- RISULTATO FINE ------------

        If w_Prodotto_Cod.Split("|")(0) < 0 Then
            Pro_Cod = 0
            Mat_Cod = -CInt(w_Prodotto_Cod)
        Else
            Pro_Cod = w_Prodotto_Cod.Split("|")(0)
            Mat_Cod = 0
        End If

        If w_ChkLottoImpianto Then
            xCod_Progetto = CODPROGETTO_NONDEFINITO
        Else
            If w_Lotto <> "" Then
                xCod_Progetto = w_Lotto
            Else
                xCod_Progetto = 0
            End If
        End If

        '-----
        'LOTTO DI ACCETTAZIONE
        If w_Lotto_Accettazione Is Nothing Then
            xLotto = LOTTO_NONDEFINITO
        Else
            xLotto = w_Lotto_Accettazione
        End If

        If isFreshAndFoodZooTabacco Then
            x_Cal_Cod = w_Cal_Cod
        Else
            If w_Chk_ParametroQualitativo Then
                x_Cal_Cod = 0
            Else
                If w_Cal_Cod <> 0 Then
                    x_Cal_Cod = w_Cal_Cod
                Else
                    x_Cal_Cod = 0
                End If
            End If
        End If

        If w_CauMov = CAU_CARICO OrElse w_CauMov = CAU_TRASFERIMENTO Then
            If w_Ubic_Destinazione <> "" Then
                Tipo_Fabbricato = w_Ubic_Destinazione.Split("_")(0)
                Sa_Cod = w_Ubic_Destinazione.Split("_")(1)
                Id_Magazzino = w_Ubic_Destinazione.Split("_")(2)
                Mag_Index = 1
            Else
                Mag_Index = -1
            End If

        ElseIf w_CauMov = CAU_SCARICO OrElse w_CauMov = CAU_TRASFERIMENTO Then
            If w_Ubic_Provenienza <> "" Then
                Mag_Index = 1
                Tipo_Fabbricato = w_Ubic_Provenienza.Split("_")(0)
                Sa_Cod = w_Ubic_Provenienza.Split("_")(1)
                Id_Magazzino = w_Ubic_Provenienza.Split("_")(2)
            Else
                Mag_Index = -1
            End If

        End If

        Dim Data_Verifica As Date
        Data_Verifica = CDate(DataMovimento)

        '''''If Operazione = enum_TipoOperazioneDB.Modifica Then
        '''''    If isFreshAndFoodZooTabacco Then
        '''''        KgLordi_Mask = KgLordi
        '''''        KgNetti_Mask = KgNetti
        '''''        Imballaggi_Mask = Imballaggi
        '''''        Contenitori_Mask = Contenitori
        '''''        Confezioni_Mask = Confezioni
        '''''    Else
        '''''        Qta_Mask = Qta
        '''''    End If

        '''''End If

        '======================================================================
        'Lettura della giacenza del prodotto selezionato in magazzino
        '----------------------------------------------------------------------

        If (Pro_Cod = 0 And Mat_Cod = 0) Or Id_Magazzino = 0 Then

            If isFreshAndFoodZooTabacco Then
                KgLordi_Risultato_Giacenza_Destinazione = 0
                KgNetti_Risultato_Giacenza_Destinazione = 0
                Imballaggi_Risultato_Giacenza_Destinazione = 0
                Contenitori_Risultato_Giacenza_Destinazione = 0
                Confezioni_Risultato_Giacenza_Destinazione = 0
                KgLordi_Risultato_Giacenza_Provenienza = 0
                KgNetti_Risultato_Giacenza_Provenienza = 0
                Imballaggi_Risultato_Giacenza_Provenienza = 0
                Contenitori_Risultato_Giacenza_Provenienza = 0
                Confezioni_Risultato_Giacenza_Provenienza = 0
            Else
                Qta_Risultato_Giacenza_Destinazione = 0
                Qta_Risultato_Giacenza_Provenienza = 0
            End If

        Else

            Flag_GiacenzaEsiste = False

            Select Case w_Elem_Cod

                Case 1 'Macchine/Attrezzature

                    'Non Esistono Giacenze in Magazzino
                    'DO NOTHING

                    '########################################################

                Case ZOO_CONSISTENZA 'Macchine/Attrezzature


                    '########################################################

                Case Else


                    Select Case w_CauMov

                        Case CAU_TRASFERIMENTO

                            Dim i As Integer

                            For i = 0 To 1

                                If i = 0 Then

                                    If w_Ubic_Provenienza <> "" Then
                                        Mag_Index = 1
                                    Else
                                        Mag_Index = -1
                                    End If

                                ElseIf i = 1 Then

                                    If w_Ubic_Destinazione <> "" Then
                                        Mag_Index = 1
                                    Else
                                        Mag_Index = -1
                                    End If
                                End If

                                'Nota: In caso di prodotto aziendale l'unita di misura non è specificata, ma
                                '      cmq è definita una sua giacenza in magazzino

                                If (w_Udm <> 0 OrElse isFreshAndFoodZooTabacco OrElse elemCod_gestisce_calCod OrElse (w_Elem_Cod = ALTRE_MATERIE And Mat_Cod <> 0)) _
                                    AndAlso Id_Magazzino <> 0 Then

                                    'legge le giacenze 
                                    Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica,
                                                                                w_piva,
                                                                                Sa_Cod,
                                                                                Id_Magazzino,
                                                                                w_Elem_Cod,
                                                                                Pro_Cod,
                                                                                Mat_Cod,
                                                                                x_Cal_Cod,
                                                                                xCod_Progetto,
                                                                                0,
                                                                                w_Udm,
                                                                                xLotto,
                                                                                Flag_QtaNoZero,
                                                                                stringaFilter,
                                                                                "", "", "", "", "", "", "", "", "", "",
                                                                                "",
                                                                                objParametri_Server, objParametri_Utenti,
                                                                               "", "", isFreshAndFoodZooTabacco, False, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)


                                    If Not IsNothing(Dt_Giacenze) Then

                                        If Dt_Giacenze.Rows.Count <> 0 Then
                                            If isFreshAndFoodZooTabacco Then
                                                'TODO

                                            Else
                                                Qta_Giacenza_Magazzino = Dt_Giacenze.Rows(0).Item("Giacenza")

                                                If Operazione = enum_TipoOperazioneDB.Modifica Then

                                                    If i = 0 Then
                                                        'provenienza

                                                        'se sono in modifica di un trasferimento 
                                                        'e sono nel caso del magazzino di provenienza
                                                        'aggiungo alla giacenza la qta specificata nell'operazione
                                                        'così ho la giacenza reale prima dell'operazione

                                                        '30/3/2020 non serve più perchè in modifica escludo dalla lettura la riga corrente
                                                        '''Qta_Giacenza_Magazzino = Qta_Giacenza_Magazzino + Qta_Mask
                                                        ' Giacenza_Centro = Giacenza_Centro + Qta_Mask

                                                        ' TODO ViewState("Giacenza_Magazzino") = Qta_Giacenza_Magazzino

                                                    ElseIf i = 1 Then
                                                        'destinazione

                                                        'se sono in modifica di un trasferimento 
                                                        'e sono nel caso del magazzino di destinazione
                                                        'sottraggo alla giacenza la qta specificata nell'operazione
                                                        'così ho la giacenza reale prima dell'operazione

                                                        '30/3/2020 non serve più perchè in modifica escludo dalla lettura la riga corrente
                                                        '''Qta_Giacenza_Magazzino = Qta_Giacenza_Magazzino - Qta_Mask
                                                        ' Giacenza_Centro = Giacenza_Centro - Qta_Mask

                                                    End If

                                                Else
                                                    'scrittura
                                                    If i = 0 Then
                                                        'provenienza
                                                        ' TODO ViewState("Giacenza_Magazzino") = Giacenza_Magazzino
                                                    End If
                                                End If

                                                StrGiacenza = w_Udm_Desc & " " & Format(Qta_Giacenza_Magazzino, "##,###,##0.0000")

                                                Flag_GiacenzaEsiste = True

                                            End If

                                        Else

                                            Flag_GiacenzaEsiste = False
                                            StrGiacenza = ""

                                        End If

                                    Else
                                        Flag_GiacenzaEsiste = False
                                        StrGiacenza = ""
                                    End If

                                End If

                                If Not Flag_GiacenzaEsiste Then

                                    Select Case Mag_Index

                                        Case -1 'Nessun Magazzino Imputato

                                            StrGiacenza = AgronicaAgenda_2010.NonDisponibile

                                        Case Else

                                            StrGiacenza = AgronicaAgenda_2010.NessunCaricoPrecedente


                                    End Select

                                End If


                                If i = 0 Then

                                    StrGiacenza_Provenienza = StrGiacenza

                                    'aggiungo il controllo sulle giacenze ed eventualmente do errore
                                    If Flag_GiacenzaEsiste = False Then
                                        If w_CauMov = enum_Agenda_Causali.TRASFERIMENTO Then

                                            permettiSalvataggio = False

                                            'MESSAGGIO DI ERRORE
                                            messaggioErrore = AgronicaAgenda_2010.ImpossibileSalvareTrasferimentoModificareDataDiRegistrazione_
                                        End If

                                    Else
                                        permettiSalvataggio = True
                                    End If

                                ElseIf i = 1 Then

                                    StrGiacenza_Destinazione = StrGiacenza

                                End If


                                StrGiacenza = ""
                                Flag_GiacenzaEsiste = False

                            Next

                            '///////////////////////////////////////////////////////////

                        Case Else

                            'Controllo che i parametri siano completi

                            'Nota: In caso di prodotto aziendale l'unita di misura non è specificata, ma
                            '      cmq è definita una sua giacenza in magazzino

                            If (w_Udm <> 0 OrElse isFreshAndFoodZooTabacco OrElse elemCod_gestisce_calCod OrElse (w_Elem_Cod = ALTRE_MATERIE And Mat_Cod <> 0)) AndAlso Id_Magazzino <> 0 Then


                                'legge le giacenze 
                                Dt_Giacenze = objG.SchedaGiacenzeMagazzino(Data_Verifica,
                                                                w_piva,
                                                                Sa_Cod,
                                                                Id_Magazzino,
                                                                w_Elem_Cod,
                                                                Pro_Cod,
                                                                Mat_Cod,
                                                                x_Cal_Cod,
                                                                xCod_Progetto,
                                                                0,
                                                                w_Udm,
                                                                xLotto,
                                                                Flag_QtaNoZero,
                                                                stringaFilter,
                                                                "", "", "", "", "", "", "", "", "", "",
                                                                "",
                                                                objParametri_Server, objParametri_Utenti,
                                                                           "", "", isFreshAndFoodZooTabacco, False, Flag_QtaMaggioreZero:=Flag_QtaMaggioreZero)


                                If Not IsNothing(Dt_Giacenze) Then

                                    If Dt_Giacenze.Rows.Count <> 0 Then

                                        Flag_GiacenzaEsiste = True

                                        If isFreshAndFoodZooTabacco Then

                                            Dim dictGiacenze As New Dictionary(Of String, Decimal)

                                            'TODO U.M.
                                            For Each row In Dt_Giacenze.Rows

                                                If FF_gest_materiale_vivaistico Then
                                                    Qta_Giacenza_Magazzino += row("Giacenza")
                                                Else
                                                    If Not dictGiacenze.ContainsKey(row("Udm_Des")) Then
                                                        dictGiacenze.Add(row("Udm_Des"), CDec(row("Giacenza")))
                                                    Else
                                                        dictGiacenze(row("Udm_Des")) = dictGiacenze(row("Udm_Des")) + CDec(row("Giacenza"))
                                                    End If
                                                End If


                                                'Qta_Giacenza_Magazzino += Dt_Giacenze.Rows(kk).Item("Giacenza")

                                            Next

                                            If FF_gest_materiale_vivaistico Then
                                                StrGiacenza = "Numero " & Format(Qta_Giacenza_Magazzino, "##,###,##0.####")
                                            Else
                                                For Each k In dictGiacenze
                                                    If StrGiacenza <> "" Then
                                                        StrGiacenza += ", "
                                                    End If
                                                    StrGiacenza += k.Key & ": " & Format(k.Value, "##,###,##0.####")
                                                Next
                                            End If

                                        Else
                                            Qta_Giacenza_Magazzino = Dt_Giacenze.Rows(0).Item("Giacenza")

                                            If w_ChkLottoImpianto Or w_Chk_ParametroQualitativo Then
                                                Qta_Giacenza_Magazzino = 0
                                                For Each row In Dt_Giacenze.Rows
                                                    Qta_Giacenza_Magazzino += row("Giacenza")
                                                Next
                                            End If

                                            StrGiacenza = w_Udm_Desc & " " & Format(Qta_Giacenza_Magazzino, "##,###,##0.####")

                                        End If
                                    End If


                                End If

                            End If

                            If Not Flag_GiacenzaEsiste Then

                                Select Case Mag_Index

                                    Case -1 'Nessun Magazzino Imputato

                                        StrGiacenza = AgronicaAgenda_2010.NonDisponibile

                                    Case Else

                                        StrGiacenza = AgronicaAgenda_2010.NessunCaricoPrecedente


                                End Select

                            End If

                            If w_CauMov = CAU_CARICO Then
                                StrGiacenza_Destinazione = StrGiacenza
                            ElseIf w_CauMov = CAU_SCARICO Then
                                StrGiacenza_Provenienza = StrGiacenza
                            End If

                    End Select


            End Select

        End If

        ' Devo passare tutte le combinazioni di parametri qualitativi e lotti trovati
        Dim righe_dettaglio As String = ""
        If Not Dt_Giacenze Is Nothing AndAlso
           Dt_Giacenze.Rows.Count > 0 Then

            Dim w_KgLordi_Giacenza_Magazzino As Decimal = 0
            Dim w_KgNetti_Giacenza_Magazzino As Decimal = 0
            Dim w_Imballaggi_Giacenza_Magazzino As Integer = 0
            Dim w_Contenitori_Giacenza_Magazzino As Integer = 0
            Dim w_Confezioni_Giacenza_Magazzino As Integer = 0

            Dt_Return = New DataTable

            Dt_Return.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Id_Destinazione", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
            Dt_Return.Columns.Add(New DataColumn("Lotto", GetType(String)))
            Dt_Return.Columns.Add(New DataColumn("Udm_Cod_Extra", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
            Dt_Return.Columns.Add(New DataColumn("Lotto_Int", GetType(String)))

            If isFreshAndFoodZooTabacco Then
                For Each paramQual In dtParamQual.Rows

                    Select Case paramQual("Tipo")

                        Case 3
                            Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Double)))

                        Case 4
                            Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(String)))

                        Case 5
                            Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Date)))

                        Case Else
                            Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod", GetType(Integer)))

                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura", GetType(Decimal)))
                                Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Sigla", GetType(String)))
                                Dt_Return.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Descrizione", GetType(String)))
                            End If

                    End Select

                Next

                Dt_Return.Columns.Add(New DataColumn("KgLordi", GetType(Decimal)))
                If gestitoImballaggio Then
                    Dt_Return.Columns.Add(New DataColumn("NrImballaggi", GetType(Integer)))
                End If
                If gestitoContenitore Then
                    Dt_Return.Columns.Add(New DataColumn("NrContenitori", GetType(Integer)))
                End If
                'La confezione l'aggiungo sempre perchè potrei vendere a numero
                Dt_Return.Columns.Add(New DataColumn("NrConfezioni", GetType(Integer)))
                Dt_Return.Columns.Add(New DataColumn("KgNetti", GetType(Decimal)))
                Dt_Return.Columns.Add(New DataColumn("TaraTotale", GetType(Decimal)))
            Else
                Dt_Return.Columns.Add(New DataColumn("Giacenza", GetType(Decimal)))
            End If

            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objAnimaliDistinte As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte

            For i = 0 To Dt_Giacenze.Rows.Count - 1

                Dim dr As DataRow

                'Creo una nuova riga
                dr = Dt_Return.NewRow

                dr.Item("Sa_Cod") = Dt_Giacenze.Rows(i).Item("Sa_Cod")
                dr.Item("Id_Destinazione") = Dt_Giacenze.Rows(i).Item("Id_Destinazione")
                dr.Item("Cal_Cod") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Cal_Cod")), Dt_Giacenze.Rows(i).Item("Cal_Cod"), 0)
                dr.Item("Udm_Cod") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Udm_Cod")), Dt_Giacenze.Rows(i).Item("Udm_Cod"), 0)
                dr.Item("Udm_Des") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Udm_Des")), Dt_Giacenze.Rows(i).Item("Udm_Des"), "")
                dr.Item("Elem_Cod") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Elem_Cod")), Dt_Giacenze.Rows(i).Item("Elem_Cod"), 0)
                dr.Item("Mat_Cod") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Mat_Cod")), Dt_Giacenze.Rows(i).Item("Mat_Cod"), 0)
                dr.Item("Lotto") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Lotto")), Dt_Giacenze.Rows(i).Item("Lotto"), "")
                dr.Item("Udm_Cod_Extra") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Udm_Cod_Extra")), Dt_Giacenze.Rows(i).Item("Udm_Cod_Extra"), 0)
                dr.Item("Cod_Progetto") = IIf(Not IsDBNull(Dt_Giacenze.Rows(i).Item("Cod_Progetto")), Dt_Giacenze.Rows(i).Item("Cod_Progetto"), 0)

                Select Case dr.Item("Elem_Cod")

                    Case SEMILAVORATI_VEGETALI

                        If dr.Item("Cod_Progetto") = 0 Then
                            dr.Item("Lotto_Int") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "DaTerzi"), String)
                        Else
                            dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(dr.Item("Cod_Progetto"), Nothing, objParametri_Server)
                        End If

                    Case TRASFORMATI_VEGETALI
                        dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(dr.Item("Cod_Progetto"), Nothing, objParametri_Server)

                    Case MATERIE_ANIMALI, SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI

                        If dr.Item("Cod_Progetto") <> 0 Then
                            Dim objAnimale = objAnimaliDistinte.Leggi(dr.Item("Cod_Progetto"), objParametri_Server)

                            If objAnimale IsNot Nothing Then
                                'dr.Item("Progetto") = objAnimale.Progetto
                                dr.Item("Lotto_Int") = objAnimale.Codice_Distinta
                            End If
                        End If

                End Select

                If isFreshAndFoodZooTabacco Then
                    For Each paramQual In dtParamQual.Rows

                        'Parametri qualitativi inseriti dall'utente
                        Select Case paramQual("Tipo")
                            Case 3
                                If Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") <> "" Then
                                    dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = Double.Parse(Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod"), Globalization.CultureInfo.InvariantCulture)
                                End If

                            Case 4
                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")

                            Case 5
                                If IsDate(Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")) Then
                                    dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = CDate(Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")).ToShortDateString
                                End If

                            Case Else
                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod") = Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod")

                                If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                    dr.Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura") = Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura")
                                    dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla") = Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Sigla")
                                    dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione") = Dt_Giacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")

                                End If
                        End Select
                    Next

                    'If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero AndAlso dr.Item("Udm_Cod_Extra") = 0 Then
                    If dr.Item("Udm_Cod") = enum_UnitaMisura.Numero AndAlso gestitoConfezione AndAlso dr.Item("FF_confezione_Tipo_Cod") = 0 Then
                        w_KgLordi_Giacenza_Magazzino = 0
                        w_KgNetti_Giacenza_Magazzino = 0
                    Else
                        w_KgLordi_Giacenza_Magazzino = Dt_Giacenze.Rows(i).Item("TaraTotale") + Dt_Giacenze.Rows(i).Item("Giacenza")
                        w_KgNetti_Giacenza_Magazzino = Dt_Giacenze.Rows(i).Item("Giacenza")
                    End If

                    dr.Item("TaraTotale") = Dt_Giacenze.Rows(i).Item("TaraTotale")

                    If gestitoImballaggio Then
                        w_Imballaggi_Giacenza_Magazzino = Dt_Giacenze.Rows(i).Item("NrImballaggi")
                    End If
                    If gestitoContenitore Then
                        w_Contenitori_Giacenza_Magazzino = Dt_Giacenze.Rows(i).Item("NrContenitori")
                    End If

                    w_Confezioni_Giacenza_Magazzino = Dt_Giacenze.Rows(i).Item("NrConfezioni")
                    dr.Item("KgLordi") = w_KgLordi_Giacenza_Magazzino

                    If gestitoImballaggio Then
                        dr.Item("NrImballaggi") = w_Imballaggi_Giacenza_Magazzino
                    End If
                    If gestitoContenitore Then
                        dr.Item("NrContenitori") = w_Contenitori_Giacenza_Magazzino
                    End If
                    dr.Item("NrConfezioni") = w_Confezioni_Giacenza_Magazzino

                    dr.Item("KgNetti") = w_KgNetti_Giacenza_Magazzino
                Else
                    dr.Item("Giacenza") = Dt_Giacenze.Rows(i).Item("Giacenza")
                End If

                Dt_Return.Rows.Add(dr)

            Next

            righe_dettaglio = Newtonsoft.Json.JsonConvert.SerializeObject(Dt_Return)

        End If


        Dim JsonString As New StringBuilder()

        JsonString.Append("{")

        If righe_dettaglio <> "" Then
            JsonString.Append(" ""righe_dettaglio"": " & righe_dettaglio & ",  ")
        End If

        'JsonString.Append(" [ ")
        JsonString.Append(" ""DatiTotali"" : {")

        JsonString.Append(" ""StrGiacenza_Provenienza"": """ & StrGiacenza_Provenienza & """, ")
        JsonString.Append(" ""StrGiacenza_Destinazione"": """ & StrGiacenza_Destinazione & """, ")
        JsonString.Append(" ""messaggioErrore"": """ & messaggioErrore & """,")
        JsonString.Append(" ""permettiSalvataggio"": """ & permettiSalvataggio.ToString & """ ")

        JsonString.Append("}")

        JsonString.Append("}")

        'JsonString.Append(" ] ")

        r.RispostaStringa = JsonString.ToString

        r.RispostaOK = True
        Return r

    End Function



    '####################################################################################################################
    Public Shared Function CaricaDati(ByVal lav_cod As Integer, ByVal cIdTipoOp As Integer) As RispostaStandard

        'TUTTO TODO!!!!

        Dim r As New RispostaStandard

        Dim JsonString As New StringBuilder()

        'Select Case cIdTipoOp

        '    Case enum_TipoOperazioneDB.Modifica

        '        Select Case lav_cod

        '            Case LAVCOD_BOLLA_EMESSA,
        '            LAVCOD_DDT_CONTABILIZZATO_EMESSO,
        '            LAVCOD_BOLLA_RICEVUTA,
        '            LAVCOD_BOLLA_EMESSA,
        '            LAVCOD_FATTURA_RICEVUTA,
        '            LAVCOD_FATTURA_EMESSA,
        '            LAVCOD_CONFERIMENTO,
        '            LAVCOD_CONFERIMENTO_DIVERSI,
        '            LAVCOD_NOTA_ACCREDITO_EMESSA,
        '            LAVCOD_NOTA_ACCREDITO_RICEVUTA


        '                'Nuova gestione 2013
        '                'modifica del deettaglio nelle operazioni di 
        '                'LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA
        '                '   ddt, devo permettere la modifica del dettaglio inserito che ora non si può fare
        '                '   fattura, se allego il dettaglio di un ddt devo permettere la modifica per aggiungere altre informazioni senza modificare il dettaglio ddt

        '                'TODO Ripristina_DETTAGLIO_nei_Controlli(Operazione)


        '            Case Else

        '                'Ripristina dati nei controlli
        '                'TODO  Me.Ripristina_DATI_nei_Controlli_2(Operazione)

        '        End Select




        '         '======================================================================

        '    Case enum_TipoOperazioneDB.Lettura

        '        'Ripristina dati nei controlli
        '        'Me.Ripristina_DATI_nei_Controlli(Operazione)
        '        'TODO     Me.Ripristina_DATI_nei_Controlli_2(Operazione)
        '        'If xCaricoScarico = enum_Agenda_Causali.TRASFERIMENTO Then
        '        '    Carica_Giacenze_Prezzo()
        '        'End If
        '        'in data 21/09/2009 ho commentato
        '        'visualizzo la giacenza per ogni tipo di operazione


        '    '======================================================================

        '    Case enum_TipoOperazioneDB.Scrittura

        'End Select

        JsonString.Append(" [{}] ")  'TODO
        r.RispostaStringa = JsonString.ToString

        r.RispostaOK = True

        Return r

    End Function

    ''' <summary>
    ''' Crea un json object con le proprietà "KeyCausale" [Integer] e "DescrCausale" [String]
    ''' </summary>
    ''' <param name="causaleCod"></param>
    ''' <param name="causaleDescr"></param>
    ''' <returns></returns>
    Private Shared Function CreaJObjectCausale(ByVal causaleCod As Integer, ByVal causaleDescr As String) As JObject
        Dim jsonObj = New JObject()

        jsonObj.Add("KeyCausale", causaleCod)
        jsonObj.Add("DescrCausale", causaleDescr)

        Return jsonObj
    End Function

    '####################################################################################################################
    Public Shared Function Carica_Causale_Riga(ByVal lav_cod As Integer, ByVal Qs_Tipo As String, ByVal chkAccompagnatoria As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim jsonArr As New JArray()

        Lingua.Gias_InizializzaCultura_DaSession()

        Select Case lav_cod

            Case LAVCOD_ACQUISTO_BENI   'Attrezzature

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.GiacenzeIniziali, AgronicaAgenda_2010.RilevamentoGiacenzeInizialiDiMagazzino))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.AutoProduzione, AgronicaAgenda_2010.BeniAutoprodotti))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato))

            Case LAVCOD_CARICO 'Carico

                'Rispettare ordine alfabetico: viene mantenuto anche in visualizzazione

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Altra_Pendenza, AgronicaAgenda_2010.AltroMovimentoDiMagazzino))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.AutoProduzione, AgronicaAgenda_2010.BeniAutoprodotti))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.GiacenzeIniziali, AgronicaAgenda_2010.RilevamentoGiacenzeInizialiDiMagazzino))

            Case LAVCOD_SCARICO 'Scarico

                'Rispettare ordine alfabetico: viene mantenuto anche in visualizzazione

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Alienazione_Pendenza, AgronicaAgenda_2010.AlienazionePendenza))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Altra_Pendenza, AgronicaAgenda_2010.AltraPendenza))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.AutoConsumo, AgronicaAgenda_2010.Autoconsumo))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Furto, AgronicaAgenda_2010.MovimentoDiMagazzinoASeguitoDiFurto))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.ResoFornitore, AgronicaAgenda_2010.ResoAFornitore))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Rottura, AgronicaAgenda_2010.Rottura))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Smaltimento, AgronicaAgenda_2010.SmaltimentoPerditaDiLavorazione))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.ScaricoFuoriRegione, AgronicaAgenda_2010.UtilizzoProdottoFuoriRegione))

            Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO 'Autoconsumo

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.AutoConsumo, AgronicaAgenda_2010.Autoconsumo))

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                Select Case Qs_Tipo
                    Case CAU_MAGAZZINO

                        jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocBolla, AgronicaAgenda_2010.MovimentoDiMagazzinoDaDocumentoDiTrasporto))

                    Case CAU_ANIMALE

                        jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocBolla, AgronicaAgenda_2010.MovimentoDiStallaDaDocumentoDiTrasporto))

                End Select

            Case LAVCOD_FATTURA_PROFORMA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocBolla, AgronicaAgenda_2010.MovimentoDiMagazzinoDaBollaDiAccompagnamentoConFatturaProForma))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocFattura_ProForma, AgronicaAgenda_2010.MovimentoDiMagazzinoDaFatturaProForma))

            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_PROFESSIONISTI 'Fattura

                Select Case Qs_Tipo
                    Case CAU_MAGAZZINO

                        If chkAccompagnatoria = 0 Then
                            jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocBolla, AgronicaAgenda_2010.MovimentoDiMagazzinoDaDocumentoDiTrasportoConFatturaCommerciale))
                        Else
                            jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocFattura, AgronicaAgenda_2010.MovimentoDiMagazzinoDaFatturaCommerciale))
                        End If

                    Case CAU_ANIMALE

                        jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocBolla, AgronicaAgenda_2010.MovimentoDiStallaDaDocumentoDiTrasportoConFatturaCommerciale))
                        jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocFattura, AgronicaAgenda_2010.MovimentoDiStallaDaFatturaCommerciale))

                End Select

            Case LAVCOD_VENDITA, LAVCOD_ACQUISTO 'Acquisto, Vendita

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovESENTE, AgronicaAgenda_2010.ProdottoEsenteDaUlterioreDocumentazione))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocCorrispettivo, AgronicaAgenda_2010.MovimentoAllegatoCorrispettivo))

            Case LAVCOD_TRASFERIMENTO 'Trasferimento Merci

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Trasferimento, AgronicaAgenda_2010.TrasferimentoMerciDiMagazzino))

            Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_ACCETTAZIONE 'Conferimento, Accettazione

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Conferimento, AgronicaAgenda_2010.ConferimentoBeni))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato))

            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO 'Aumento Consistenze

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.ZooConsistenzeIniziali, AgronicaAgenda_2010.RilevamentoConsistenzeInizialiDiStalla))

            Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO 'Diminuzione Consistenze

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovForzato, AgronicaAgenda_2010.VariazioneConsistenzeZootecniche))

            Case LAVCOD_NOTA_ACCREDITO_RICEVUTA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Resi_Acquisti, AgronicaAgenda_2010.ScaricoGiustificatoDaResiSuAcquisti))

            Case LAVCOD_NOTA_ACCREDITO_EMESSA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Resi_Vendite, AgronicaAgenda_2010.CaricoGiustificatoDaResiSuVendite))

            Case LAVCOD_RICEVUTA_EMESSA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocRicevuta, AgronicaAgenda_2010.MovimentoDocumentatoDaRicevutaFiscale))

            '---- ASTERISCATO PERCHE' SAREBBE IL 16 CHE NELL'ONLINE E' UTILIZZATO PER ALTRO
            '---- COMUNQUE AD OGGI QUESTI NON SONO GESTITI NELL'ONLINE
            'Case LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_LIQ_CONF_RICEVUTA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
            '    JsonString.Append("{")

            '    JsonString.Append(" ""KeyCausale"": " & enum_Pendenza.DocAccettazione & ",")
            '    JsonString.Append(" ""DescrCausale"": " & """Movimento Documentato da Accettazione Beni Conferiti""")

            '    JsonString.Append("}")

            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovGiustificato, AgronicaAgenda_2010.MovimentoDiMagazzinoGiustificato))

            Case LAVCOD_ORDINE_VENDITA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocOrdine, AgronicaAgenda_2010.MovimentoPrevistoDaOrdineDiVendita))

            Case LAVCOD_PREVENTIVO_VENDITA

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocPreventivo_Vendita, AgronicaAgenda_2010.MovimentoPrevistoDaPreventivoDiVendita))

            Case LAVCOD_ORDINE_ACQUISTO

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocOrdine, AgronicaAgenda_2010.MovimentoPrevistoDaOrdineDiAcquisto))

            Case LAVCOD_CORRISPETTIVO_VENDITA_SFUSO

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Altra_Pendenza, AgronicaAgenda_2010.AltroMovimentoDiCantina))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovESENTE, AgronicaAgenda_2010.ProdottoEsenteDaUlterioreDocumentazione))

            Case 1067 'Carico Consistenze Enologiche

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.GiacenzeIniziali, AgronicaAgenda_2010.RilevamentoConsistenzeEnologiche))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.AutoProduzione, AgronicaAgenda_2010.BeniAutoprodotti))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Resi_Vendite, AgronicaAgenda_2010.CaricoGiustificatoDaResiSuVendite))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Altra_Pendenza, AgronicaAgenda_2010.AltraPendenza))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiCantinaNonGiustificatoDocumentato))

            Case 1068 'Scarico Consistenze Enologiche

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Smaltimento, AgronicaAgenda_2010.SmaltimentoPerditaDiLavorazione))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Resi_Acquisti, AgronicaAgenda_2010.ScaricoGiustificatoDaResiSuAcquisti))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Altra_Pendenza, AgronicaAgenda_2010.AltroMovimentoDiCantina))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiCantinaNonGiustificatoDocumentato))

            Case LAVCOD_CONFERIMENTO, LAVCOD_ACCETTAZIONE

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.Conferimento, AgronicaAgenda_2010.ConferimentoBeni))

            Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO 'Aumento Consistenze

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.GiacenzeIniziali, AgronicaAgenda_2010.RilevamentoConsistenzeInizialiDiStalla))
                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiCantinaNonGiustificatoDocumentato))

            Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO 'Variazione Consistenze

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovForzato, AgronicaAgenda_2010.VariazioneConsistenzeZootecniche))

                'Case LAVCOD_DOCO_EMESSO, LAVCOD_DOCO_RICEVUTO 'Doco Emesso NELL'ONLINE E' UTILIZZATO PER ALTRO - COMUNQUE AD OGGI QUESTI NON SONO GESTITI NELL'ONLINE
                '    JsonString.Append("{")

                '    JsonString.Append(" ""KeyCausale"": " & enum_Pendenza.DocDoco & ",")
                '    JsonString.Append(" ""DescrCausale"": " & """Movimento di Magazzino Documentato da Doco""")

                '    JsonString.Append("}")

                'Case LAVCOD_DAA_EMESSO 'D.A.A. Emesso  NELL'ONLINE E' UTILIZZATO PER ALTRO - COMUNQUE AD OGGI QUESTI NON SONO GESTITI NELL'ONLINE
                '    JsonString.Append("{")

                '    JsonString.Append(" ""KeyCausale"": " & enum_Pendenza.DocDAA & ",")
                '    JsonString.Append(" ""DescrCausale"": " & """Fattura Commerciale allegata a D.A.A.""")

                '    JsonString.Append("}")

            Case LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO 'MVV

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.DocMVV, AgronicaAgenda_2010.MovimentoDiMagazzinoDaMVV))

            Case Else

                jsonArr.Add(CreaJObjectCausale(enum_Pendenza.MovPendente, AgronicaAgenda_2010.MovimentoDiMagazzinoNonGiustificatoDocumentato))

        End Select

        r.RispostaStringa = jsonArr.ToString() 'jsonArr.ToString(Newtonsoft.Json.Formatting.None, New Newtonsoft.Json.Converters.StringEnumConverter())

        r.RispostaOK = True

        Return r

    End Function


    '####################################################################################################################
    Public Shared Function CostruisciLinkInfoProdotto(ByVal p As String, ByVal e As Integer, ByVal l As Integer, ByVal c As String, ByVal d As String, ByVal a As Integer, ByVal orig As String, ByVal mode As String) As RispostaStandard

        Dim r As New RispostaStandard

        'Costruisco il link
        r.RispostaStringa = "p=" &
                      Stringa_Codifica(p, AgroKey_EncoderDecoder, Nothing) &
                      "e=" &
                      Stringa_Codifica(e, AgroKey_EncoderDecoder, Nothing) &
                      "&l=" &
                      Stringa_Codifica(l, AgroKey_EncoderDecoder, Nothing) &
                      "&k=" &
                      Stringa_Codifica(c, AgroKey_EncoderDecoder, Nothing) &
                      "&o=" &
                      Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Nothing) &
                      "&d=" &
                      Stringa_Codifica(d, AgroKey_EncoderDecoder, Nothing) &
                      "&c=" &
                      Stringa_Codifica("C", AgroKey_EncoderDecoder, Nothing) &
                      "&a=" &
                      Stringa_Codifica(a, AgroKey_EncoderDecoder, Nothing) &
                      "&r=" &
                      Stringa_Codifica(orig, AgroKey_EncoderDecoder, Nothing) &
                      "&orig=" &
                      Stringa_Codifica(orig, AgroKey_EncoderDecoder, Nothing) &
                      "&mode=" & mode

        r.RispostaOK = True
        Return r

    End Function

    Public Shared Function Leggi_PUA_Regolamenti(ByVal Regolamento_Cod As Long,
                                                 ByVal DataMovimento As Date,
                                                 ByVal elem_Cod As Integer,
                                                 ByVal objParametri_Server As AgronicaCoreParametri,
                                                 ByVal objParametri_Utenti As AgronicaCoreParametri
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim JsonString As New StringBuilder()
        Dim cmb_PUARegolamenti As New DropDownList

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            If elem_Cod = FORMULATI Then

                cmb_PUARegolamenti.Items.Add(New ListItem(AgronicaAgenda_2010.Nessuno, "0" & "/" & "0"))

            Else

                'Dim FiltroTipo As String = " ( Tipo=2 OR Tipo=3 )"
                Dim FiltroTipo As String = " ( Tipo=2 )"
                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS_xAgenda(cmb_PUARegolamenti,
                                                                            True,
                                                                            AgronicaAgenda_2010.Nessuno,
                                                                            "0" & "/" & "0",
                                                                            False,
                                                                            0,
                                                                            DataMovimento,
                                                                            DataMovimento,
                                                                            FiltroTipo,
                                                                            " Ordine desc ")
            End If

        Catch ex As Exception

            'Aggiungo almeno la voce nulla se c'è stato qualche errore
            cmb_PUARegolamenti.Items.Add(New ListItem(AgronicaAgenda_2010.Nessuno, "0" & "/" & "0"))

        End Try

        '28/06/2018: aggiunto tipo al cod regolamento
        'AgronicaCoreUtility.CaricaListControl.PUA_Regolamento(cmb_PUARegolamenti, True, AgronicaAgenda_2010.Nessuno, "0" & SEP_PuaReg & "0", "Tipo=2 OR Tipo=3", "", objParametri_Server, True)

        '28/06/2018: aggiunta regolamento bio
        'Lascio l'aggiunta da fuori, perché dentro non inserisce la tipologia
        cmb_PUARegolamenti.Items.Add(New ListItem(Descrizione_Regolamento_Bio, Tipo_Regolamento_Bio & "/" & "0"))

        JsonString.Append("[")
        For Each elem In cmb_PUARegolamenti.Items
            If Not JsonString.ToString = "[" Then
                JsonString.Append(", ")
            End If
            JsonString.Append("{")
            JsonString.Append(" ""Regolamento_Cod"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value].Split("/")(0) & """,")
            JsonString.Append(" ""Regolamento_Tipo"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value].Split("/")(1) & """,")
            JsonString.Append(" ""Regolamento_DES"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Text] & """ ")
            JsonString.Append("}")
        Next

        JsonString.Append("]")

        r.RispostaStringa = JsonString.ToString
        r.RispostaOK = True

        Return r

    End Function

    '###############################################################################
    Public Shared Function Recupera_UdmFertilizzante(ByVal Fer_Cod As Integer, ByVal PUA_RegolamentoCod As Integer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                     ByRef objAgroWebConfig As AgroWebConfig,
                                                     Optional ByRef msgErrorWS As String = Nothing
                                                     ) As Integer

        Dim udmCod As Integer = 0

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input With {
            .Codice = Fer_Cod,
            .Descrizione = "",
            .DataInizio = AGRODATAINIZIO,
            .DataFine = AGRODATAFINE,
            .Tipo = PUA_RegolamentoCod,
            .IncludiApporti = True,
            .IncludiTipologia = False,
            .Regolamento = PUA_RegolamentoCod,
            .strFiltro = ""
        }

        objParametriIngresso.Url = objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti & "/Fertilizzanti"

        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output = objFert_WS.Fertilizzanti(objParametriIngresso)

        If Not objParametriUscita.ListaFertilizzanti Is Nothing AndAlso objParametriUscita.ListaFertilizzanti.Count = 1 Then
            udmCod = objParametriUscita.ListaFertilizzanti(0).Udm_Cod
        End If

        If objParametriUscita.MessaggioErrore = "" AndAlso udmCod <> 0 Then
            Return udmCod
        Else
            msgErrorWS = objParametriUscita.MessaggioErrore
            Return 0
        End If

    End Function

    Public Shared Function Udm_Optimize_Regolamento(ByVal piva As String,
                                                    ByVal DataMovimento As Date,
                                                    ByVal xSa_Cod As Integer,
                                                    ByVal xFabbricato_Cod As Integer,
                                                    ByVal Cau_Mov As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Flag_ProCod_Negativo As Boolean,
                                                    ByVal Prodotto_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Flag_Prima_Riga As Boolean,
                                                    ByVal Testo_PrimaRiga As String,
                                                    ByVal Cod_Prima_Riga As String,
                                                    ByVal Testo_Da_Ricercare As String,
                                                    ByVal RegolamentoCod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByVal isFreshAndFood As Boolean,
                                                    ByVal Flag_QtaNoZero As Boolean,
                                                    ByVal Flag_QtaMaggioreZero As Boolean,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim cmb_Udm As New DropDownList

        Select Case Elem_Cod
            Case ALTRI_BENI, SERVIZI
                'In questo caso devo mostrare tutti gli udm, quindi a prescindere da cosa è arrivato, passo Elem_Cod = 0 e Cau_Mov = CAU_CARICO
                Elem_Cod = 0
                Cau_Mov = CAU_CARICO
        End Select

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(DataMovimento, AGRODATAFINE)

        AgronicaCoreUtility.CaricaListControl.Udm_Optimize_Regolamento(cmb_Udm,
                                                                       0,
                                                                       piva,
                                                                       xSa_Cod,
                                                                       xFabbricato_Cod,
                                                                       Cau_Mov,
                                                                       Elem_Cod,
                                                                       Flag_ProCod_Negativo,
                                                                       Prodotto_Cod,
                                                                       Mat_Cod,
                                                                       Flag_Prima_Riga, Testo_PrimaRiga, Cod_Prima_Riga,
                                                                       Testo_Da_Ricercare, RegolamentoCod,
                                                                       "", "",
                                                                       objParametri_Server, objParametri_Utenti,
                                                                       isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero)

        objParametri_Server.ResettaFinestra()

        If Cau_Mov = CAU_CARICO AndAlso (Elem_Cod <> FERTILIZZANTI OrElse (Elem_Cod = FERTILIZZANTI AndAlso RegolamentoCod > 1)) Then
            If cmb_Udm.Items.FindByValue(enum_UnitaMisura.KG) IsNot Nothing Then

                If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Quintali) Is Nothing Then
                    cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Quintali.ToString, enum_UnitaMisura.Quintali))
                End If

                If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Tonnellate) Is Nothing Then
                    cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Tonnellate.ToString, enum_UnitaMisura.Tonnellate))
                End If

            End If
        End If

        Dim jsonArr As New JArray()

        For Each elem As ListItem In cmb_Udm.Items
            Dim jsonObj = New JObject()

            jsonObj.Add("Udm_Cod", elem.Value)
            jsonObj.Add("Udm_Des", elem.Text)

            jsonArr.Add(jsonObj)
        Next

        r.RispostaStringa = jsonArr.ToString
        r.RispostaOK = True

        Return r

    End Function



    '####################################################################################################################
    Public Shared Function CostruisciInfoFertilizzante(ByVal fer_cod As Integer)

        Dim r As New RispostaStandard

        If CInt(fer_cod) <> 0 Then

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Str As New StringBuilder


            Dim DtTemp As DataTable
            Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

            'Recupero le informazioni		
            DtTemp = objFertilizzanti.Leggi(CInt(fer_cod),
                                        "", AGRODATAINIZIO, AGRODATAFINE,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim i As Integer
            '<table width='100%'>
            '   <tr><td colspan='2'></td> </tr>
            '   <tr><td style='width:50%'></td><td style='width:50%'></td></tr>
            '</table>
            Dim colSpan_unaCella As Integer = 2

            Str.Append("<table width=""100%"">")




            'nome fertilizzante
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & DtTemp.Rows(0).Item("Fer_Des") & "</b></td></tr>")

            'Denominazione
            If Not IsDBNull(DtTemp.Rows(0).Item("Denominazione")) Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><i>" & DtTemp.Rows(0).Item("Denominazione") & "</i></td></tr>")
            End If

            Dim str_ditta As String = objFertilizzanti.DittaDes_from_FerCod(CInt(fer_cod), Nothing, HttpContext.Current.Session("ASG_objParametri_Server"))
            If str_ditta.Trim() <> "" Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" &
                           HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "Ditta") & "</b>: <i>" &
                       str_ditta &
                       "</i></td></tr>")
            End If

            Str.Append("<tr>")
            Str.Append("<td><b>N</b>: <i>" & DtTemp.Rows(0).Item("N") & "</i></td>")
            Str.Append("<td><b>P205</b>: <i>" & DtTemp.Rows(0).Item("P2O5") & "</i></td>")
            Str.Append("</tr>")


            Str.Append("<tr>")
            Str.Append("<td><b>K2O</b>: <i>" & DtTemp.Rows(0).Item("K2O") & "</i></td>")
            Str.Append("<td><b>MgO</b>: <i>" & DtTemp.Rows(0).Item("MgO") & "</i></td>")
            Str.Append("</tr>")



            Str.Append("<tr>")
            Str.Append("<td><b>SO3</b>: <i>" & DtTemp.Rows(0).Item("SO3") & "</i></td>")
            Str.Append("<td><b>Mn</b>: <i>" & DtTemp.Rows(0).Item("Mn") & "</i></td>")
            Str.Append("</tr>")

            Str.Append("<tr>")
            Str.Append("<td><b>Cl</b>: <i>" & DtTemp.Rows(0).Item("Cl") & "</i></td>")
            Str.Append("<td><b>Fe</b>: <i>" & DtTemp.Rows(0).Item("Fe") & "</i></td>")
            Str.Append("</tr>")

            Str.Append("<tr>")
            Str.Append("<td><b>S_O</b>: <i>" & DtTemp.Rows(0).Item("S_O") & "</i></td>")
            Str.Append("<td><b>Cu</b>: <i>" & DtTemp.Rows(0).Item("Cu") & "</i></td>")
            Str.Append("</tr>")

            Str.Append("<tr>")
            Str.Append("<td><b>Mo</b>: <i>" & DtTemp.Rows(0).Item("Mo") & "</i></td>")
            Str.Append("<td><b>Zn</b>: <i>" & DtTemp.Rows(0).Item("Zn") & "</i></td>")
            Str.Append("</tr>")

            Str.Append("<tr>")
            Str.Append("<td><b>CaO</b>: <i>" & DtTemp.Rows(0).Item("CaO") & "</i></td>")
            Str.Append("<td><b>Na2O</b>:<i>" & DtTemp.Rows(0).Item("Na2O") & "</i></td>")
            Str.Append("</tr>")

            Str.Append("<tr>")
            Str.Append("<td><b>B</b>: <i>" & DtTemp.Rows(0).Item("B") & "</i></td>")
            Str.Append("<td><b>SS</b>: <i>" & DtTemp.Rows(0).Item("SS") & "</i></td>")
            Str.Append("</tr>")



            If Not IsDBNull(DtTemp.Rows(0).Item("Precauzione")) AndAlso DtTemp.Rows(0).Item("Precauzione").ToString.Trim <> "" Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" &
                           HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "Precauzione") & "</b>: <i>" &
                           DtTemp.Rows(0).Item("Precauzione") & "</i></td></tr>")
            End If

            If Not IsDBNull(DtTemp.Rows(0).Item("Rapporti")) AndAlso DtTemp.Rows(0).Item("Rapporti").ToString.Trim <> "" Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "Rapporti") & "</b>: <i>" & DtTemp.Rows(0).Item("Rapporti") & "</i></td></tr>")
            End If

            If Not IsDBNull(DtTemp.Rows(0).Item("Prezzo")) AndAlso DtTemp.Rows(0).Item("Prezzo").ToString.Trim <> "" Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & AgronicaAgenda_2010.Prezzo & "</b>: <i>" & DtTemp.Rows(0).Item("Prezzo") & "</i></td></tr>")
            End If


            Dim str_class As String = objFertilizzanti.Classificazioni_Fertilizzante_from_FerCod(CInt(fer_cod),
                                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                              "", "",
                                                                              HttpContext.Current.Session("ASG_objParametri_Server"))
            If str_class.Trim <> "" Then
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" &
                           HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "Classificazioni") &
                           "</b>: <i>" & str_class & "</i></td></tr>")
            End If

            Dim str_formulazioni As String = objFertilizzanti.FormulazioniFertilizzanti_FORM_FER_DES(CInt(fer_cod),
                                                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                              "", "",
                                                                              HttpContext.Current.Session("ASG_objParametri_Server"))
            Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" &
                       HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "Formulazioni") &
                       "</b>: <i>" & str_formulazioni & "</i></td></tr>")




            Dim objRxF As New AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R
            'Leggo le imprese associate al profilo selezionato			
            DtTemp = objRxF.Leggi(0,
                              CInt(fer_cod),
                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                              "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

            'Se il recordset non è chiuso allora ...	
            If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then
                'Inserisco i record trovati
                Dim str_app As String = ""
                For i = 0 To DtTemp.Rows.Count - 1
                    str_app = str_app + DtTemp.Rows(i).Item("Reg_Des") & ","
                Next
                If str_app <> "" Then
                    str_app = Left(str_app, str_app.Length - 1)
                End If
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & AgronicaAgenda_2010.Regolamenti & "</b>: <i>" &
                                            str_app &
                                            "</i></td></tr>")

            End If

            'Elimino il recordset
            DtTemp = Nothing




            '------------------------------------------
            '-----  TIPOLOGIE
            '------------------------------------------

            Dim ObjTipologie As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipologie_R

            DtTemp = ObjTipologie.Leggi(0, 0, CInt(fer_cod),
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

            ObjTipologie = Nothing
            'Se il recordset non è chiuso allora ...	
            If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then

                Dim str_app As String = ""
                'Inserisco i record trovati
                For i = 0 To DtTemp.Rows.Count - 1
                    str_app = str_app + DtTemp.Rows(i).Item("Tp_Des") & ","
                Next
                If str_app <> "" Then
                    str_app = Left(str_app, str_app.Length - 1)
                End If
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" & AgronicaAgenda_2010.Tipologie & "</b>: <i>" &
                                                          str_app &
                                                          "</i></td></tr>")

            End If





            '------------------------------------------
            '-----  TIPOLOGIE CE
            '------------------------------------------

            Dim ObjTipologieCE As New AgronicaCoreMetaSchemaDAL.FertilizzantixTipoCE_R

            DtTemp = ObjTipologieCE.Leggi(0, CInt(fer_cod),
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      "", "", HttpContext.Current.Session("ASG_objParametri_Server"))



            'Se il recordset non è chiuso allora ...	
            If Not DtTemp Is Nothing AndAlso DtTemp.Rows.Count > 0 Then

                Dim str_app As String = ""
                'Inserisco i record trovati
                For i = 0 To DtTemp.Rows.Count - 1
                    str_app = str_app + DtTemp.Rows(i).Item("TipCE_Des") & ","
                Next
                If str_app <> "" Then
                    str_app = Left(str_app, str_app.Length - 1)
                End If
                Str.Append("<tr><td colspan=""" & colSpan_unaCella & """><b>" &
                           HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabileDettagliUC.ascx", "TipologieCE") &
                           "</b>: <i>" & str_app & "</i></td></tr>")

            End If



            'chiudo la tabella
            Str.Append("</table>")

            r.RispostaStringa = Str.ToString

            r.RispostaOK = True

        End If

        Return r

    End Function

    '####################################################################################################################
    Public Shared Function Udm_Optimize(ByVal piva As String,
                                        ByVal xSa_Cod As Integer,
                                        ByVal xFabbricato_Cod As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Flag_ProCod_Negativo As Boolean,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Flag_Prima_Riga As Boolean,
                                        ByVal Testo_PrimaRiga As String,
                                        ByVal Cod_Prima_Riga As String,
                                        ByVal Testo_Da_Ricercare As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByVal isFreshAndFood As Boolean,
                                        ByVal Flag_QtaNoZero As Boolean,
                                        ByVal Flag_QtaMaggioreZero As Boolean,
                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                        ByVal objParametri_Utenti As AgronicaCoreParametri
                                        ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim cmb_Udm As New DropDownList

        Select Case Elem_Cod
            Case ALTRI_BENI, SERVIZI
                'In questo caso devo mostrare tutti gli udm, quindi a prescindere da cosa è arrivato, passo Elem_Cod = 0 e Cau_Mov = CAU_CARICO
                Elem_Cod = 0
                Cau_Mov = CAU_CARICO
        End Select

        AgronicaCoreUtility.CaricaListControl.Udm_Optimize(cmb_Udm,
                                                           0,
                                                           piva,
                                                           xSa_Cod,
                                                           xFabbricato_Cod,
                                                           Cau_Mov,
                                                           Elem_Cod,
                                                           Flag_ProCod_Negativo,
                                                           Pro_Cod,
                                                           Mat_Cod,
                                                           Flag_Prima_Riga, Testo_PrimaRiga, Cod_Prima_Riga,
                                                           Testo_Da_Ricercare, xFiltroAggiuntivo, xOrderBy,
                                                           objParametri_Server, objParametri_Utenti,
                                                           isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero)


        If Cau_Mov = CAU_CARICO AndAlso Elem_Cod <> FERTILIZZANTI AndAlso cmb_Udm.Items.FindByValue(enum_UnitaMisura.KG) IsNot Nothing Then

            If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Quintali) Is Nothing Then
                cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Quintali.ToString, enum_UnitaMisura.Quintali))
            End If

            If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Tonnellate) Is Nothing Then
                cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Tonnellate.ToString, enum_UnitaMisura.Tonnellate))
            End If

        End If

        Dim jsonArr As New JArray()

        For Each elem As ListItem In cmb_Udm.Items
            Dim jsonObj = New JObject()

            jsonObj.Add("Udm_Cod", elem.Value)
            jsonObj.Add("Udm_Des", elem.Text)

            jsonArr.Add(jsonObj)
        Next

        r.RispostaStringa = jsonArr.ToString
        r.RispostaOK = True

        Return r

    End Function

    '####################################################################################################################
    Public Shared Function Udm_Optimize_Lotto(ByVal piva As String,
                                              ByVal xSa_Cod As Integer,
                                              ByVal xFabbricato_Cod As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Flag_ProCod_Negativo As Boolean,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Lotto As String,
                                              ByVal Flag_Prima_Riga As Boolean,
                                              ByVal Testo_PrimaRiga As String,
                                              ByVal Cod_Prima_Riga As String,
                                              ByVal Testo_Da_Ricercare As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByVal isFreshAndFood As Boolean,
                                              ByVal Flag_QtaNoZero As Boolean,
                                              ByVal Flag_QtaMaggioreZero As Boolean,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal objParametri_Utenti As AgronicaCoreParametri
                                              ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim cmb_Udm As New DropDownList

        Select Case Elem_Cod
            Case ALTRI_BENI, SERVIZI
                'In questo caso devo mostrare tutti gli udm, quindi a prescindere da cosa è arrivato, passo Elem_Cod = 0 e Cau_Mov = CAU_CARICO
                Elem_Cod = 0
                Cau_Mov = CAU_CARICO
        End Select

        AgronicaCoreUtility.CaricaListControl.Udm_Optimize2(cmb_Udm,
                                                            0,
                                                            piva,
                                                            xSa_Cod,
                                                            xFabbricato_Cod,
                                                            Cau_Mov,
                                                            Elem_Cod,
                                                            Flag_ProCod_Negativo,
                                                            Pro_Cod,
                                                            Mat_Cod,
                                                            Lotto,
                                                            Flag_Prima_Riga, Testo_PrimaRiga, Cod_Prima_Riga,
                                                            Testo_Da_Ricercare, xFiltroAggiuntivo, xOrderBy,
                                                            objParametri_Server, objParametri_Utenti, isFreshAndFood, Flag_QtaNoZero, Flag_QtaMaggioreZero)


        If Cau_Mov = CAU_CARICO AndAlso Elem_Cod <> FERTILIZZANTI AndAlso cmb_Udm.Items.FindByValue(enum_UnitaMisura.KG) IsNot Nothing Then

            If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Quintali) Is Nothing Then
                cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Quintali.ToString, enum_UnitaMisura.Quintali))
            End If

            If cmb_Udm.Items.FindByValue(enum_UnitaMisura.Tonnellate) Is Nothing Then
                cmb_Udm.Items.Add(New ListItem(enum_UnitaMisura.Tonnellate.ToString, enum_UnitaMisura.Tonnellate))
            End If

        End If

        Dim jsonArr As New JArray()

        For Each elem As ListItem In cmb_Udm.Items
            Dim jsonObj = New JObject()

            jsonObj.Add("Udm_Cod", elem.Value)
            jsonObj.Add("Udm_Des", elem.Text)

            jsonArr.Add(jsonObj)
        Next

        r.RispostaStringa = jsonArr.ToString
        r.RispostaOK = True

        Return r

    End Function


    '####################################################################################################################

    Public Shared Function FineScorta_e_altreInfo(ByVal fr_cod As Integer,
                                                  ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri) As String

        Dim infoText As String = ""

        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim Parametri As String = ""
        Dim strErr As String = ""
        Dim LastFr_Des As String = ""
        Dim i As Integer


        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

            Dim objAgroWebConfig As New AgroWebConfig
            Dim XmlDoc As New System.Xml.XmlDocument


            objWs.NewWS(ObjDownloadWs,
                            objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci,
                            objParametri_Utenti)
            XmlDoc = New System.Xml.XmlDocument

            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                            StrCredenziali,
                                            AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                            objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                            HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                            HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                            HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

            XmlDoc.LoadXml(StrCredenziali)

            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

            'Dim grfi_Cod As Integer = 0
            objCoreAgroWs.AgroWS_XML_Parametri_Formulato_Completo(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                   StrParametri,
                                                   fr_cod,
                                                   strErr)

            XML_Credenziali.InnerXml = StrParametri

            Parametri = XmlDoc.OuterXml

            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)
            Dim dt As DataTable
            dt = ObjDownloadWs.Leggi_Formulati_Info_DT(Parametri, strErr)


            ''verifico se è tutto ok

            infoText = "<span >"

            For i = 0 To dt.Rows.Count - 1
                If Not IsDBNull(dt.Rows(i).Item("Datasospensioneda")) Then
                    infoText = infoText + "<br><span style='color:#2B7000'> " & AgronicaAgenda_2010.SospesoDal_ & CDate(dt.Rows(i).Item("Datasospensioneda")).ToShortDateString & " </span>"
                End If
                If Not IsDBNull(dt.Rows(i).Item("Datasospensionea")) Then
                    infoText = infoText + "<span style='color:#2B7000'> " & AgronicaAgenda_2010.SospesoAl_ & CDate(dt.Rows(i).Item("Datasospensionea")).ToShortDateString & " </span>"
                End If
            Next


            If dt.Rows(0).Item("Revocato") = 1 Then
                infoText = infoText + "<br><span style='color:#FF0000'> " & AgronicaAgenda_2010.RevocatoIl_ & CDate(dt.Rows(0).Item("data_revo")).ToShortDateString & " </span>"
            End If

            If Not IsDBNull(dt.Rows(0).Item("Data_fine_comm")) Then
                infoText = infoText + "<br><span style='color:#FF0000'> " & AgronicaAgenda_2010.FineCommercializzazione_ & CDate(dt.Rows(0).Item("Data_fine_comm")).ToShortDateString & " </span>"
            End If

            If Not IsDBNull(dt.Rows(0).Item("Data_fine_usoscorte")) Then
                infoText = infoText + "<br><span style='color:#2B7000'> " & AgronicaAgenda_2010.FineUsoScorte_ & CDate(dt.Rows(0).Item("Data_fine_usoscorte")).ToShortDateString & " </span>"
            End If


            infoText = infoText + "</span>"

            ObjDownloadWs.Dispose()

        Catch ex As Exception

        End Try

        Return infoText

    End Function



    '####################################################################################################################

    Public Shared Function VerificaPermessoClasseToxPatentino(ByVal Data_Validita As Date, ByVal Fr_Cod As Integer, ByVal PivaAzienda As String,
                                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                              ByRef messWarning As String,
                                                              ByRef msg As String,
                                                              ByVal linkWsFitofarmaci As String,
                                                              ByRef objSession As System.Web.SessionState.HttpSessionState) As Boolean


        'Per acquistare e impiegare prodotti fitosanitari classificati come 
        'MOLTO TOSSICI, TOSSICI e NOCIVI è necessaria un'apposita autorizzazione comunemente nota come "Patentino" (D.P.R. 290/01).

        Lingua.Gias_InizializzaCultura_DaSession()

        'controllo impostazione utente
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt As DataTable
        Dt = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_ProdottiTossiciPatentinoMovimenti,
                 1,
                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 "",
                 "",
                 objParametri_Utenti)


        Dim i As Integer = 0
        Dim str As String = ""

        'se esiste l'impostazione utilizzo quella altrimenti metto dei default (x i nuovi utenti per esempio)
        If Dt.Rows.Count > 0 Then
            str = Dt.Rows(0).Item("Impostazione_Valore_1")
        End If

        If str = "" Or str = "0" Then
            Return True
        End If

        'verifico se patentino permette
        Dt = New AgronicaCoreAnagrafeDAL.Contatti_R().Contatti_Con_Patentino(PivaAzienda, Data_Validita, objParametri_Server)
        If Dt.Rows.Count > 0 Then
            Return True
        End If

        Dim ClassiTox As Hashtable = FormulatoRecuperaClassiTox(Fr_Cod, objParametri_Server, objSession, linkWsFitofarmaci)
        If ClassiTox.Count = 0 Then
            Return True
        End If

        'T	Tossico 
        'T+	Molto Tossico
        'Xn	Nocivo
        For Each classe As DictionaryEntry In ClassiTox

            Select Case classe.Key

                Case "T", "T+", "Xn"
                    msg = classe.Value


                    If str = "2" Then
                        'blocco
                        Return False
                    End If


                    If str = "1" Then
                        'avviso
                        'non funziona con agrosino!!!!
                        '' ''controllo se ho acconsentito precedenrtemente
                        ' ''If UdmPatentinoTox_SI_NO.Value = "0" Then

                        ' ''    'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                        ' ''    Dim messaggio_errore As String = "Attenzione, . Procedere ugualmente?"
                        ' ''    'AgroSiNo

                        ' ''    'impedisco di procedere
                        ' ''    Messaggi.AgroSiNo(messaggio_errore, "UdmPatentinoTox", Page, , update_si_no)


                        ' ''    Return True

                        ' ''Else
                        ' ''    'se ho già cliccato  ok vado avanti
                        ' ''    UdmPatentinoTox_SI_NO.Value = "0"
                        ' ''    Return True
                        ' ''End If

                        ''''ASPX

                        '' ''                                function DoPostBack_ControlliSiNo(key) {
                        '' ''            alert(key);
                        '' ''                if (key == 'UdmMovimenti') {
                        '' ''                    $("#<%=UdmMovimenti_SI_NO.ClientID %>").val("OK");
                        '' ''                    $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
                        '' ''                }

                        '' ''                if (key == 'UdmPatentinoTox') {
                        '' ''                    alert('b');
                        '' ''                     alert($("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val());
                        '' ''//                       if ($("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val()="OK"){
                        '' ''//                        alert('a');
                        '' ''//                       }

                        '' ''//                    $("#<%=UdmPatentinoTox_SI_NO.ClientID %>").val("OK");
                        '' ''//                    $("#<%=ImgBtn_Inserisci_nel_DataGrid.ClientID %>").click();
                        '' ''                }

                        '' ''            }      



                        '' ''  <ContentTemplate>
                        '' ''            <asp:HiddenField ID="UdmMovimenti_SI_NO" runat="server" Value="0" />
                        '' ''             <asp:HiddenField ID="UdmPatentinoTox_SI_NO" runat="server" Value="0" />
                        '' ''        </ContentTemplate>

                        messWarning = String.Format(AgronicaAgenda_2010.RichiestoContattoConPatentinoValidoOperazioneNonBloccata, msg)
                        Return True


                    End If

                    Return False

            End Select

        Next classe

        Return True

    End Function


    '####################################################################################################################

    Private Shared Function FormulatoRecuperaClassiTox(ByVal Fr_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri,
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState,
                                                       ByVal linkWsFitofarmaci As String
                                                          ) As Hashtable
        Dim ClassiTox As New Hashtable
        'Creo la stringa XML di richiesta

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCredenziali As System.Xml.XmlElement
        Dim XmlParametri As System.Xml.XmlElement
        Dim XmlNodo As System.Xml.XmlElement
        Dim StringaXML As String

        Dim WsScheda As String
        Dim WsDoorKey As String
        Dim WsCodiceGias As String
        Dim WsUsernameSuperuser As String
        Dim WsPasswordSuperuser As String

        '===========================================================================================
        '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
        '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
        '   </CREDENZIALI>
        '===========================================================================================

        WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
        WsDoorKey = "portone_grth45ksh4ghajnl32149"
        WsCodiceGias = objSession("ASG_ProgressivoGIAS")
        WsUsernameSuperuser = objSession("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
        WsPasswordSuperuser = objSession("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

        'Creo il nodo CREDENZIALI
        XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

        'Imposto gli attributi
        XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
        XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
        XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
        XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
        XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

        'Imposto XmlParametri come figlio del documento principale
        XmlDoc.AppendChild(XmlCredenziali)

        'Creo il nodo PARAMETRI
        XmlParametri = XmlDoc.CreateElement("PARAMETRI")

        'Imposto gli attributi
        XmlParametri.SetAttribute("fr_cod", Fr_Cod)

        'Imposto XmlParametri come figlio del documento principale
        XmlCredenziali.AppendChild(XmlParametri)

        'Restituisco in uscita la stringa creata
        StringaXML = XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlNodo = Nothing
        XmlParametri = Nothing
        'XmlDoc = Nothing

        '//////////////////////////////////////////////////////////
        '/////   Chiamo la funzione del webservice per recuperare la composizione
        '//////////////////////////////////////////////////////////

        Dim WsRisposta As String = ""
        Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
        StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

        Try
            Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci
            Dim objAgroWebConfig As New AgroWebConfig
            If objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = "" Then
                WsFito.Url = linkWsFitofarmaci
            Else
                WsFito.Url = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            WsFito.Timeout = 60000
            WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

        Catch ex As Exception
            Return ClassiTox
            'Throw New Exception("Webservice fitofarmaci : " & ex.Message)
        End Try


        '//////////////////////////////////////////////////////////
        '/////   Decodifico il risultato ==> DataTable
        '//////////////////////////////////////////////////////////

        'Dim DTfor As New DataTable
        'DTfor = New DataTable
        'DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        'DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        'DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
        'DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


        'Dim XmlDoc As New System.Xml.XmlDocument
        Dim XMLs_Formulati As System.Xml.XmlNodeList
        Dim XMLs_PrincipiAttivi As System.Xml.XmlNodeList
        Dim XMLs_ClassiTossicologiche As System.Xml.XmlNodeList

        Dim XmlRisultati As System.Xml.XmlElement
        Dim XmlFormulato As System.Xml.XmlElement
        Dim XmlPrincipioAttivo As System.Xml.XmlElement
        Dim XmlClasseTossicologica As System.Xml.XmlElement


        Dim ict As Integer = 0
        Dim ipa As Integer = 0
        Dim Testo As String = ""

        XmlDoc.LoadXml(WsRisposta)

        'Recupero l'elenco dei Formulati
        XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

        If Not IsNothing(XMLs_Formulati) Then

            For i = 0 To XMLs_Formulati.Count - 1

                'Formulato i-esimo
                XmlFormulato = XMLs_Formulati.Item(i)


                '---------------------------------------------------
                'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
                'Recupero l'elenco delle Classi Tossicologiche

                XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
                Testo = ""
                For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
                    XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
                    Dim cltoss_cod As String = CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod"))
                    Dim cltoss_des As String = CStr(XmlClasseTossicologica.GetAttribute("cltoss_des"))
                    ClassiTox.Add(cltoss_cod, cltoss_des)
                Next

            Next

        End If 'XMLs_Formulati

        Return ClassiTox

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim hdPiva As HtmlInputHidden = Parent.FindControl("hdPiva")
        Dim hdIdAgenda As HtmlInputHidden = Parent.FindControl("hdIdAgenda")
        Dim hdLavCod As HtmlInputHidden = Parent.FindControl("hdLavCod")
        Dim hdTipoOp As HtmlInputHidden = Parent.FindControl("hdTipoOp")
        Dim hf_Qs_SaCod As HtmlInputHidden = Parent.FindControl("hf_Qs_SaCod")

        '##############################################################
        '###################  QUERY STRING  ###########################
        '##############################################################

        'TODO ORA k NON VIENE PASSATO
        Dim Chiave As String
        Albero.ChiaveAlbero_Codifica(Chiave, enum_TipoNodo.f_Magazzino, hdPiva.Value, hf_Qs_SaCod.Value, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

        If Not String.IsNullOrEmpty(Chiave) Then

            hf_Qs_Key.Value = Chiave
            Dim xPiva As String
            Dim xSa_Cod As Integer
            Dim xFabbricato_Cod As Integer
            Dim xCampo_Cod As Integer
            Dim xAppezza As Integer
            Dim xID_Imp As Integer
            Dim xCodFiscale As String
            Dim xTipoNodo As enum_TipoNodo
            Call AgronicaCoreDataProvider.Albero.ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                    hf_Qs_Key.Value,
                                    xTipoNodo,
                                    xPiva,
                                    xSa_Cod,
                                    xCampo_Cod,
                                    xAppezza,
                                    xID_Imp,
                                    xCodFiscale,
                                    xFabbricato_Cod)
            hf_xFabbricato_Cod.Value = xFabbricato_Cod
        End If




        Select Case hdLavCod.Value

            Case LAVCOD_FATTURA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_FATTURA_RICEVUTA,
                LAVCOD_FATTURA_PROFESSIONISTI, LAVCOD_FATTURA_PROFORMA, LAVCOD_RICEVUTA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                Qs_Mode = "fattura"

            Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO
                'TODO: Qs_Mode = fattura?!? o ordine (non esiste, quali sono le implicazioni, dove viene usato?)?!?
                Qs_Mode = "fattura"

            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DISTINTA_CARICO,
                    LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
                    LAVCOD_ACCETTAZIONE_DIVERSI,
                    LAVCOD_DAA_EMESSO, LAVCOD_MVV_EMESSO, LAVCOD_MVV_RICEVUTO
                Qs_Mode = "bolla"

            Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO, LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
                Qs_Mode = "magazzino"

            Case LAVCOD_CONTRATTO_AFFITTO
                Qs_Mode = "contratto"

            Case Else
                Qs_Mode = ""

        End Select
        hf_Qs_Mode.Value = Qs_Mode


        Qs_Tipo = CAU_MAGAZZINO
        hf_Qs_Tipo.Value = Qs_Tipo

        'TODO ORA e NON VIENE PASSATO
        If Not IsNothing(Request.QueryString("e")) Then
            Qs_ElemCod = Stringa_Decodifica(Request.QueryString("e").ToString,
                                                    AgroKey_EncoderDecoder,
                                                    Server)
        Else
            Qs_ElemCod = 0
        End If
        hf_Qs_ElemCod.Value = Qs_ElemCod

        Qs_OraSelezionata = "12" & gettimesep() & "00"
        hf_Qs_OraSelezionata.Value = Qs_OraSelezionata

        If (Not String.IsNullOrEmpty(hdIdAgenda.Value)) Then

            'TODO Con Giulia serve???  
            'Dim objAgenda = New AgronicaCoreContabDAL.Agenda_R
            'Dim DtAgenda = objAgenda.Leggi(
            '                            hdPiva.Value,
            '                            0,
            '                            hdIdAgenda.Value,
            '                            0,
            '                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                            "",
            '                            "",
            '                            objParametri_Server)

            'If DtAgenda.Rows.Count > 0 Then
            '    Master().Lbl_Titolo.Text = "Modifica " + DtAgenda.Rows(0).Item("Des_Lib")
            'End If

            'TODO   serve???  
            'Dim configSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            'Dim LanToWebSiteBasePath As String = configSiti.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
            'Dim GiasOnline_WS_Core_AgroWS_Core As String = configSiti.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
            'Dim urlWSFF = LanToWebSiteBasePath + GiasOnline_WS_Core_AgroWS_Core + "FreshAndFood/FreshAndFood.asmx"


            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

        End If

    End Sub


    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(Me.Page.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    '########################################################################################
    'La pagina, non essendo più transazionale, non ha nè il Page Commit, nè il Page Abort
    'Utilizza AAA_GestioneUscitaPagina per la gestione di uscita dalla pagina 
    '(in caso di salvataggio o di exit)

    'TODO GUARDARE CON GIULIA

    'Private Sub AAA_GestioneUscitaPagina()

    '    'TODO ... qui è tutto da verificare cosa fare

    '    Dim hf_Qs_PagRitorno As HtmlInputHidden = Parent.FindControl("_qsPagRitorno")
    '    Dim hdLavCod As HtmlInputHidden = Parent.FindControl("hdLavCod")
    '    Dim hf_Qs_Key As HtmlInputHidden = Parent.FindControl("Qs_Key")
    '    Dim hf_Qs_Data_Selezionata As HtmlInputHidden = Parent.FindControl("Qs_Data_Selezionata")

    '    ' TODO Verificare se questa funzione serve ancora e se va ancora bene

    '    If Not IsNothing(Request.QueryString("exit")) AndAlso Request.QueryString("exit") = True Then
    '        'chiudo 
    '        Dim str As String = " $(document).ready(function () { "
    '        str &= "  window.close();"
    '        str &= "});"
    '        'ScriptManager.RegisterClientScriptBlock(Me.Master.FindControl("FORM1"), Me.Master.FindControl("FORM1").GetType(),
    '        '                                 String.Format("jQuery_{0}", Txt_DataMovimento.ClientID), str, True)

    '        Dim strClose As String = "<script language='javascript'>" & str & "</script>"
    '        Me.Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))


    '        'ClientScript.RegisterClientScriptBlock(Me.GetType(), "Close", "window.close()", True)
    '        Exit Sub
    '    End If


    '    'controllo se provengo dal gias smart
    '    If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
    '        Dim objParametriAgenda As New ParametriAgenda

    '        Dim link As String = ""
    '        Try
    '            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
    '            Dim paginaOnLineRitorno As Integer = CInt(hf_Qs_PagRitorno.Value)

    '            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
    '                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
    '                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
    '                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

    '                Response.Redirect(link)
    '            Else
    '                If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
    '                    If paginaOnLineRitorno = enum_PagineGiasOnline_2010.concimazione_lite Or paginaOnLineRitorno = enum_PagineGiasOnline_2010.trattamenti_lite Then
    '                        Dim str As String = " $(document).ready(function () { "
    '                        str &= "  window.close();"
    '                        str &= "});"
    '                        Dim strClose As String = "<script language='javascript'>" & str & "</script>"
    '                        Me.Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    '                        Exit Sub
    '                    End If
    '                End If
    '            End If

    '        Catch ex As Exception

    '        End Try



    '    End If


    '    If CInt(hf_Qs_PagRitorno.Value) = enum_PagineAgenda_2010.Menu Then
    '        Response.Redirect("../Menu/Menu.aspx")
    '    End If

    '    If CInt(hf_Qs_PagRitorno.Value) = enum_PagineAgenda_2010.Menu_BS Then
    '        Response.Redirect("../Menu/MenuBS_Agenda_Nuovo.aspx")
    '    End If

    '    If CInt(hf_Qs_PagRitorno.Value) = enum_PagineAgenda_2010.Pagina_GestioneMagazzini Then
    '        Response.Redirect("../GestioneMagazzini/GestioneMagazzini.aspx")
    '    End If

    '    If CInt(hf_Qs_PagRitorno.Value) = enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS Then
    '        Response.Redirect("../GestioneMagazzini/GestioneMagazziniBS.aspx")
    '    End If

    '    If CInt(hf_Qs_PagRitorno.Value) = enum_PagineGiasOnline.MenuContab Then
    '        'controllo se provengo dal giasonline
    '        If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline Then
    '            Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '            objGiasOnline.PaginaRichiesta = hf_Qs_PagRitorno.Value
    '            objGiasOnline.Piva = ViewState("Piva")
    '            objGiasOnline.Sa_Cod = ViewState("Sa_Cod")

    '            Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                                     objGiasOnline)
    '            Response.Redirect(str)
    '        End If
    '    End If

    '    'controllo se provengo dal giasonline
    '    If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline Then
    '        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '        objGiasOnline.PaginaRichiesta = hf_Qs_PagRitorno.Value
    '        objGiasOnline.Piva = ViewState("Piva")
    '        objGiasOnline.Sa_Cod = ViewState("Sa_Cod")

    '        Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
    '                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
    '                             objGiasOnline)
    '        Response.Redirect(str)
    '    End If





    '    Dim TargetURL As String

    '    'Verifico che l'uscita sia voluta ....
    '    If PremutoAnnulla = True Or EseguitaOperazione = True Then

    '        Select Case (hdLavCod.Value)

    '            Case LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_TRASFERIMENTO,
    '                 LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO,
    '                 LAVCOD_VENDITA, LAVCOD_ACQUISTO

    '                Session("DT_Prodotti_nel_Doc") = Nothing
    '                Session("matrix_XML_To_Documento") = Nothing
    '                Session("vet_XML_To_FormProdotto") = Nothing




    '                Dim Origine As String
    '                Origine = PaginaAspx_from_TipoEnumPagina(hf_Qs_PagRitorno.Value, "../")

    '                'Costruisco il link
    '                TargetURL = Origine &
    '                            "?k=" &
    '                            Stringa_Codifica(hf_Qs_Key.Value, AgroKey_EncoderDecoder, Server) &
    '                            "&p=" &
    '                            Stringa_Codifica(ViewState("Piva"), AgroKey_EncoderDecoder, Server) &
    '                            "&s=" &
    '                            Stringa_Codifica(ViewState("Sa_Cod"), AgroKey_EncoderDecoder, Server) &
    '                            "&d=" &
    '                            Stringa_Codifica(hf_Qs_Data_Selezionata.Value, AgroKey_EncoderDecoder, Server) &
    '                            "&orig=" &
    '                            Stringa_Codifica(hf_Qs_PagRitorno.Value, AgroKey_EncoderDecoder, Server)

    '                'Vado alla pagina
    '                Response.Redirect(TargetURL)



    '            Case Else


    '                'non faccio niente xchè ho chiuso la finestra al click sul bottone Salva Tutto



    '        End Select

    '    End If

    'End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub
End Class
