Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp



Public Class OperazioneDiCuraClasse

    'Cura del tabacco(lav cod 5004) , simile alla 2 trasformazione(lav cod 5000) del lan
    '
    'Movimento 10001
    '
    'Scarico del prodotto x (tabacco raccolto) dal magazzino  
    '               (un movimento per un prodotto, ma non escludo la possibilità di gestirne di più)
    '                                                                                1 mov scarico        CAU_SCARICO As String = "7350" alla data inizio cura
    '                                                                                1 dettaglio        di QtaTot
    '
    'Carico  del prodotto y (tabacco  in cura) nel fabbricato essiccatoio 222 
    '               (un movimento per ciascun cassone, o uso il collo?? devo usare la stessa cosa che poi scarico 
    '               altrimenti le giacenze non tornano e usare lo stesso lotto, con il lotto indico il 
    '               cassone occupato, così posso controlare con giacenze i cassoni presenti, quindi occupati in una data
    '                                                                               1 mov carico         CAU_CARICO As String = "7350" alla data inizio cura
    '                                                                                n dettagli+dest (una per ciascun Essiccatoio) di QtaTot/n
    '
    'POSSO raggruppare scarico fEssiccatoio e carico magazzino dove indico lotto nello stesso movimento
    'oppure posso infilare il codice del collo in un altro campo nello scarico Essiccatoio
    'Scarico del prodotto y (tabacco  in cura) dal fabbricato essiccatoro 222        
    '              scarico gli stessi lotti caricati perche toprnino giacenze
    '
    'Carico  del prodotto z (tabacco   curato) nel magazzino   
    '              carico n colli(lotti), suddivisi in m forni 
    '                                                                                 nXm mov carico (perche possono variare le date fine e l'ora e fanno fede quelle del movimento)          CAU_CARICO As String = "7350" alla data inizio cura
    '                                                                                nXm dettagli+dest (n forni m colliEssiccatoio) 1 lotto per collo
    '
    Public Shared Function CreaOggettoAgenda_CURATABACCO( _
                        ByVal ID_Agenda As Integer, _
                        ByVal objParametri_Server As AgronicaCoreParametri, _
                        ByVal TipoOperazione As Integer, _
                        ByVal ASG_ProgressivoGIAS As Integer, _
                        ByVal DtInfornature As DataTable, _
                        ByVal DtSfornature As DataTable, _
                        ByVal Mat_Cod_Tabacco_InCura As Integer, ByVal Mat_Cod_Tabacco_Curato As Integer, _
                        ByVal Nota As String, ByRef messaggio_errore As String, _
                        ByVal Veg_Cod As Integer, _
                        ByVal NomeProdottoCurato As String, _
                        ByVal NomeProdottoInEssiccatoio As String, _
                        ByVal UdmCaricoMagazzino As enum_UnitaMisura, _
                        ByVal AgendaLottoRacc As String) As Operazione_Agenda


        'Attenzione, oora funziona con prodotto in Essiccatoio e curato come TRASFORMATI_VEGETALI
        'veg_cod=335

        'Anche se arrivano gia ordinati li riordino per sicurezza, altrimento non funzia nulla
        Dim dw As DataView = DtInfornature.DefaultView
        dw.Sort = "Id_Infornatura, Sa_nome, Fabbricato_Des, Data_Inizio_Cura, Ora_Inizio_Cura "
        DtInfornature = dw.ToTable
        Dim dw2 As DataView = DtSfornature.DefaultView
        dw2.Sort = "Id_Infornatura, Data_Fine_Cura, Ora_Fine_Cura, Magazzino_Sa_Cod, Magazzino_Fabbricato_Cod, Id_Lotto"
        DtSfornature = dw2.ToTable


        '------------------------------------------------------------------
        'Controlli vari
        If DtInfornature.Rows.Count = 0 Then
            Throw New Exception("Deve esserci almeno un prodotto di DtInfornature")
        End If

        Dim DataOperazione As Date = DtInfornature.Rows(0).Item("Data_Inizio_Cura")
        Dim PivaOperazione As String = DtInfornature.Rows(0).Item("Piva")
        Dim Sa_CodOperazione As Integer = DtInfornature.Rows(0).Item("Sa_Cod")
        Dim OraOperazione As String = DtInfornature.Rows(0).Item("Ora_Inizio_Cura")
        For i = 0 To DtInfornature.Rows.Count - 1
            If DtInfornature.Rows(i).Item("Data_Inizio_Cura") <> DataOperazione Then
                Throw New Exception("Deve esserci la stessa data nelle indornature")
            End If
            If DtInfornature.Rows(i).Item("Piva") <> PivaOperazione Then
                Throw New Exception("Deve esserci la stessa piva nelle infornature")
            End If
            If DtInfornature.Rows(i).Item("Sa_Cod") <> Sa_CodOperazione Then
                Throw New Exception("Deve esserci lo stesso centro nelle infornature")
            End If
            If DtInfornature.Rows(i).Item("Ora_Inizio_Cura") <> OraOperazione Then
                Throw New Exception("Deve esserci la stessa ora nelle infornature")
            End If
        Next
        For i = 0 To DtSfornature.Rows.Count - 1

        Next



        'GENERO PRODOTTO SE NON INDICATO (per ora solo sulla specie, varietà altre)
        Dim Mat_Des_Ritorno As String = ""
        Dim Mat_Des_Ritorno2 As String = ""
        If Mat_Cod_Tabacco_Curato = -1 Then
            Mat_Cod_Tabacco_Curato = GeneraProdottoCuraTabacco(objParametri_Server, ASG_ProgressivoGIAS, NomeProdottoCurato, "CURATO/", Veg_Cod)
        Else
            Mat_Des_Ritorno = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", TRASFORMATI_VEGETALI, Mat_Cod_Tabacco_Curato, "", "", "", objParametri_Server)
        End If

        If Mat_Cod_Tabacco_InCura = -1 Then
            Mat_Cod_Tabacco_InCura = GeneraProdottoCuraTabacco(objParametri_Server, ASG_ProgressivoGIAS, NomeProdottoInEssiccatoio, "INCURA/", Veg_Cod)
        Else
            Mat_Des_Ritorno2 = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", TRASFORMATI_VEGETALI, Mat_Cod_Tabacco_InCura, "", "", "", objParametri_Server)
        End If

        Dim BaseCode As Integer
        Dim TopCode As Integer
        Call Calcola_BaseCode_TopCode(BaseCode, _
                              TopCode, _
                             ASG_ProgressivoGIAS)
        Dim Lav_Cod As Integer = LAVCOD_CURA
        Dim UdmEssiccatoi As Integer = enum_UnitaMisura.Numero
        '------------------------------------------------------------------
        'Creo l'agenda con lav_cod 5004 e un movimento
        '------------------------------------------------------------------

        Dim Agenda As Operazione_Agenda
        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = TipoOperazione
        Agenda.Id_Agenda = ID_Agenda
        Agenda.Data = DataOperazione
        Agenda.Piva = PivaOperazione
        Agenda.Sa_Cod = Sa_CodOperazione
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = "Operazione di Cura"
        If AgendaLottoRacc <> "" Then
            Agenda.Des_Lib &= " (Lotto Raccolto: " & AgendaLottoRacc & ")"
        End If
        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode
        ''---------------MOVIMENTO CAU_LINEA_PRODUZIONE 10001 come trasformazione 
        Dim Movimento_OperazioneColturale As New Movimento
        Movimento_OperazioneColturale.Id_Agenda = Agenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = CAU_LINEA_PRODUZIONE
        Movimento_OperazioneColturale.Mov_Desc = Nota
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode
        Agenda.Movimenti.Add(Movimento_OperazioneColturale)


        Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze

        '------------------------------------------------------------------
        'INFORNATURE
        'Creo 2 moviment1 per ciascuna riga delle infornatura in gridviewinfornature
        'ne basterebbero due, uno per scarichi e uno per carichi ma meglio uniforrmare per gestire meglio collegamenti e rintracciata
        'una riga genera due movimenti
        'uno di scarico da magazzino
        'uno di carico nel Essiccatoio di tabacco in cura
        'i due movimenti sono legati fra loro tramite il campo...ExtraInt??
        '------------------------------------------------------------------

        'Attenzione, il movimento di carico/scarico ha il sa_cod della destinazione del carico/scarico
        'quindi del Essiccatoio e del magazzino

        For Each infornatura As DataRow In DtInfornature.Rows
            Dim Id_Infornatura As String = infornatura.Item("Id_Infornatura")
            Dim Piva As String = infornatura.Item("Piva")
            Dim Sa_Cod As String = infornatura.Item("Sa_Cod")
            Dim Fabbricato_Cod As String = infornatura.Item("Fabbricato_Cod")
            Dim Fabbricato_Des As String = infornatura.Item("Fabbricato_Des")
            Dim Cassoni As String = infornatura.Item("Cassoni")
            Dim Data_Inizio_Cura As String = infornatura.Item("Data_Inizio_Cura")
            Dim Ora_Inizio_Cura As DateTime = infornatura.Item("Ora_Inizio_Cura")
            'magazzino
            Dim Magazzino_Sa_Cod As String = infornatura.Item("Magazzino_Sa_Cod")
            Dim Magazzino_Sa_Nome As String = infornatura.Item("Magazzino_Sa_Nome")
            Dim Magazzino_Fabbricato_Cod As String = infornatura.Item("Magazzino_Fabbricato_Cod")
            Dim Magazzino_Fabbricato_Des As String = infornatura.Item("Magazzino_Fabbricato_Des")
            Dim Magazzino_Cat_Cod As String = infornatura.Item("Magazzino_Cat_Cod")
            Dim Magazzino_Cat_Des As String = infornatura.Item("Magazzino_Cat_Des")
            Dim Magazzino_Pro_Cod As String = infornatura.Item("Magazzino_Pro_Cod")
            Dim Magazzino_Pro_Des As String = infornatura.Item("Magazzino_Pro_Des")
            Dim Magazzino_Mat_Cod As String = infornatura.Item("Magazzino_Mat_Cod")
            Dim Magazzino_Lotto_Int As String = infornatura.Item("Magazzino_Lotto_Int")
            Dim Magazzino_Lotto_Acc As String = infornatura.Item("Magazzino_Lotto_Acc")
            Dim Magazzino_Cod_Progetto As String = infornatura.Item("Magazzino_Cod_Progetto")
            Dim Magazzino_Param_Des As String = infornatura.Item("Magazzino_Param_Des")
            Dim Magazzino_Cal_Cod As String = infornatura.Item("Magazzino_Cal_Cod")
            Dim Magazzino_Cal_Des As String = infornatura.Item("Magazzino_Cal_Des")
            Dim Magazzino_Udm_Cod As String = infornatura.Item("Magazzino_Udm_Cod")
            Dim Magazzino_Udm_Des As String = infornatura.Item("Magazzino_Udm_Des")
            Dim Magazzino_Qta As String = infornatura.Item("Magazzino_Qta")


            'genero l'id per associare i movimenti di scarico e carico per infornatura 
            'e scarico da forno e carico magaz in forno per sfornatura da mettere nell'extraint
            Dim idSC As Integer = seq.NuovoId_Tabella("GeneraIdOperazioneDiCuraMovInfornatura", 0, 2000000000, objParametri_Server)


            '------------------------------------------------------------------
            'Movimento scarico magazzino
            Dim Movimento_Scarico_Magazzino As New Movimento
            Movimento_Scarico_Magazzino.Id_Agenda = Agenda.Id_Agenda
            Movimento_Scarico_Magazzino.Piva = Agenda.Piva
            Movimento_Scarico_Magazzino.Sa_Cod = Magazzino_Sa_Cod
            Movimento_Scarico_Magazzino.Data = Data_Inizio_Cura
            Movimento_Scarico_Magazzino.Ora = Ora_Inizio_Cura
            Movimento_Scarico_Magazzino.Lav_Cod = Lav_Cod
            Movimento_Scarico_Magazzino.Cau_Mov = CAU_SCARICO
            Movimento_Scarico_Magazzino.Mov_Desc = "Scarico di Magazzino Per Cura"
            Movimento_Scarico_Magazzino.BaseCode = Agenda.BaseCode
            Movimento_Scarico_Magazzino.TopCode = Agenda.TopCode

            Movimento_Scarico_Magazzino.Extra_Int = idSC

            Dim Movimento_Dettaglio_Scarico_Magazzino As New Movimento_Dettaglio
            Movimento_Dettaglio_Scarico_Magazzino.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Scarico_Magazzino.Piva = Agenda.Piva
            Movimento_Dettaglio_Scarico_Magazzino.Sa_Cod = Movimento_Scarico_Magazzino.Sa_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Data = Movimento_Scarico_Magazzino.Data
            Movimento_Dettaglio_Scarico_Magazzino.Elem_Cod = Magazzino_Cat_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Pro_Cod = Magazzino_Pro_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Mat_Cod = Magazzino_Mat_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Mov_det_des = "Scarico di Magazzino Per Cura"
            Movimento_Dettaglio_Scarico_Magazzino.Udm_Cod = Magazzino_Udm_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Qta = Magazzino_Qta
            Movimento_Dettaglio_Scarico_Magazzino.Cal_Cod = Magazzino_Cal_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Lotto = Magazzino_Lotto_Acc
            Movimento_Dettaglio_Scarico_Magazzino.Cod_Progetto = Magazzino_Cod_Progetto
            Movimento_Dettaglio_Scarico_Magazzino.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio_Scarico_Magazzino.Pendente = enum_Pendenza.MovGiustificato
            Movimento_Dettaglio_Scarico_Magazzino.Lav_Cod = Lav_Cod
            Movimento_Dettaglio_Scarico_Magazzino.Cau_Mov = Movimento_Scarico_Magazzino.Cau_Mov
            Movimento_Dettaglio_Scarico_Magazzino.Anno = 1900

            Dim Movimento_Destinazione_Scarico_Magazzino As New Movimento_Destinazione
            Movimento_Destinazione_Scarico_Magazzino.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione_Scarico_Magazzino.Piva = Agenda.Piva
            Movimento_Destinazione_Scarico_Magazzino.Sa_Cod = Movimento_Scarico_Magazzino.Sa_Cod
            Movimento_Destinazione_Scarico_Magazzino.Id_Destinazione = Magazzino_Fabbricato_Cod
            Movimento_Destinazione_Scarico_Magazzino.Data = Movimento_Scarico_Magazzino.Data
            Movimento_Destinazione_Scarico_Magazzino.Tipo = MAGAZZINO
            Movimento_Destinazione_Scarico_Magazzino.Qta = Movimento_Dettaglio_Scarico_Magazzino.Qta
            Movimento_Destinazione_Scarico_Magazzino.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Scarico_Magazzino.TopCode = Agenda.TopCode

            Movimento_Dettaglio_Scarico_Magazzino.Movimenti_Destinazioni.Add(Movimento_Destinazione_Scarico_Magazzino)
            Movimento_Scarico_Magazzino.Movimenti_Dettagli.Add(Movimento_Dettaglio_Scarico_Magazzino)
            Agenda.Movimenti.Add(Movimento_Scarico_Magazzino)

            '------------------------------------------------------------------
            'Movimento Carico Essiccatoio
            Dim Movimento_Carico_Essiccatoio As New Movimento
            Movimento_Carico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
            Movimento_Carico_Essiccatoio.Piva = Agenda.Piva
            Movimento_Carico_Essiccatoio.Sa_Cod = Agenda.Sa_Cod 'attenzione, sa cod del centro non del magazzino
            Movimento_Carico_Essiccatoio.Data = Data_Inizio_Cura
            Movimento_Carico_Essiccatoio.Ora = Ora_Inizio_Cura
            Movimento_Carico_Essiccatoio.Lav_Cod = Lav_Cod
            Movimento_Carico_Essiccatoio.Cau_Mov = CAU_CARICO
            Movimento_Carico_Essiccatoio.Mov_Desc = "Carico Essiccatoio Per Cura"
            Movimento_Carico_Essiccatoio.BaseCode = Agenda.BaseCode
            Movimento_Carico_Essiccatoio.TopCode = Agenda.TopCode

            Movimento_Carico_Essiccatoio.Extra_Int = idSC

            Dim Movimento_Dettaglio_Carico_Essiccatoio As New Movimento_Dettaglio
            Movimento_Dettaglio_Carico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Carico_Essiccatoio.Piva = Agenda.Piva
            Movimento_Dettaglio_Carico_Essiccatoio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Carico_Essiccatoio.Data = Movimento_Carico_Essiccatoio.Data
            Movimento_Dettaglio_Carico_Essiccatoio.Elem_Cod = TRASFORMATI_VEGETALI
            Movimento_Dettaglio_Carico_Essiccatoio.Pro_Cod = 0
            Movimento_Dettaglio_Carico_Essiccatoio.Mat_Cod = Mat_Cod_Tabacco_InCura
            Movimento_Dettaglio_Carico_Essiccatoio.Mov_det_des = "Carico Essiccatoio Per Cura"
            Movimento_Dettaglio_Carico_Essiccatoio.Udm_Cod = UdmEssiccatoi
            Movimento_Dettaglio_Carico_Essiccatoio.Qta = Cassoni
            Movimento_Dettaglio_Carico_Essiccatoio.Cal_Cod = 0
            Movimento_Dettaglio_Carico_Essiccatoio.Lotto = Id_Infornatura
            Movimento_Dettaglio_Carico_Essiccatoio.Cod_Progetto = 0
            Movimento_Dettaglio_Carico_Essiccatoio.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio_Carico_Essiccatoio.Pendente = enum_Pendenza.MovGiustificato
            Movimento_Dettaglio_Carico_Essiccatoio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio_Carico_Essiccatoio.Cau_Mov = Movimento_Carico_Essiccatoio.Cau_Mov
            Movimento_Dettaglio_Carico_Essiccatoio.Anno = 1900

            Dim Movimento_Destinazione_Carico_Essiccatoio As New Movimento_Destinazione
            Movimento_Destinazione_Carico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione_Carico_Essiccatoio.Piva = Agenda.Piva
            Movimento_Destinazione_Carico_Essiccatoio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Destinazione_Carico_Essiccatoio.Id_Destinazione = Fabbricato_Cod
            Movimento_Destinazione_Carico_Essiccatoio.Data = Movimento_Carico_Essiccatoio.Data
            Movimento_Destinazione_Carico_Essiccatoio.Tipo = ESSICCATOIO
            Movimento_Destinazione_Carico_Essiccatoio.Qta = Movimento_Dettaglio_Carico_Essiccatoio.Qta
            Movimento_Destinazione_Carico_Essiccatoio.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Carico_Essiccatoio.TopCode = Agenda.TopCode

            Movimento_Dettaglio_Carico_Essiccatoio.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Essiccatoio)
            Movimento_Carico_Essiccatoio.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Essiccatoio)
            Agenda.Movimenti.Add(Movimento_Carico_Essiccatoio)

        Next


        '------------------------------------------------------------------
        'SFORNATURE
        'Creo un movimento di scarico essiccatoio per ciascuna sfornatura identificata da Id_Infornatura, Data_Fine_Cura, Ora_Fine_Cura,
        'dato che l'ora è nel movimento
        'le prime colonne a sinistra nella gridview colli, attenzione al numero cassoni che cagherà solo il primo e se inserisco
        'altre infornature con stessa idinfornatura, data e ora non considera i cassoni ma vanno in coda alle altre
        '
        'per ciascun movimento di scarico sfornatura c'è un movimento di carico nel/nei magazzini con gli n colli indicati nei dettagli
        ''------------------------------------------------------------------
        Dim NewSfornatura As Boolean
        Dim count As Integer = 0
        Dim Id_SfornaturaCorr As String = ""
        Dim Id_SfornaturaOld As String = ""
        Dim Movimento_Carico_Magazzino As Movimento
        For Each sfornatura As DataRow In DtSfornature.Rows
            Dim Id_Infornatura As String = sfornatura.Item("Id_Infornatura")
            Dim Piva As String = sfornatura.Item("Piva")
            Dim Cassoni As Integer = sfornatura.Item("Cassoni")
            Dim Data_Fine_Cura As Date = sfornatura.Item("Data_Fine_Cura")
            Dim Ora_Fine_Cura As DateTime = sfornatura.Item("Ora_Fine_Cura")
            Dim Id_Lotto As String = sfornatura.Item("Id_Lotto")
            Dim Magazzino_Sa_Cod As String = sfornatura.Item("Magazzino_Sa_Cod")
            Dim Magazzino_Sa_Nome As String = sfornatura.Item("Magazzino_Sa_Nome")
            Dim Magazzino_Fabbricato_Cod As String = sfornatura.Item("Magazzino_Fabbricato_Cod")
            Dim Magazzino_Fabbricato_Des As String = sfornatura.Item("Magazzino_Fabbricato_Des")
            Dim Peso As Decimal = sfornatura.Item("Peso")
            Dim Collo As String = sfornatura.Item("Collo")

            Dim Sa_Cod_Rif As Integer
            Dim Fabbricato_Cod_Rif As Integer


            'cerco l'infornatura di riferimento, potrei anche inserire il centro e il Essiccatoio nella tabella delle sfornature
            Dim trovata = False
            For Each infornatura As DataRow In DtInfornature.Rows
                Dim Id_Infornatura_Rif As String = infornatura.Item("Id_Infornatura")
                Dim Piva_Rif As String = infornatura.Item("Piva")
                Sa_Cod_Rif = infornatura.Item("Sa_Cod")
                Fabbricato_Cod_Rif = infornatura.Item("Fabbricato_Cod")
                Dim Fabbricato_Des_Rif As String = infornatura.Item("Fabbricato_Des")
                Dim Cassoni_Rif As Integer = infornatura.Item("Cassoni")
                Dim Data_Inizio_Cura_Rif As Date = infornatura.Item("Data_Inizio_Cura")
                Dim Ora_Inizio_Cura_Rif As DateTime = infornatura.Item("Ora_Inizio_Cura")
                If Id_Infornatura_Rif = Id_Infornatura Then
                    'ok trovata
                    If Piva <> Piva_Rif Then
                        Throw New Exception("le piva non sono uguali")
                    End If
                    If Agenda.Sa_Cod <> Sa_Cod_Rif Then
                        Throw New Exception("Sa_Cod non sono uguali")
                    End If
                    If DateDiff(DateInterval.Day, Data_Inizio_Cura_Rif, Data_Fine_Cura) < 0 Then
                        Throw New Exception("La data di sfornatura è precedente all'infornatura")
                    End If
                    If Cassoni > Cassoni_Rif Then
                        Throw New Exception("Sono stati sfornati piu cassoni di quanti sono infornati")
                    End If
                    trovata = True
                    Exit For
                End If
            Next

            If Not trovata Then
                Throw New Exception("Non ho trovato l'infornatura")
            End If

            'chiave dela sfornatura
            Id_SfornaturaCorr = Id_Infornatura & "-" & Data_Fine_Cura & "-" & Ora_Fine_Cura & "-" & Magazzino_Sa_Cod & "-" & Magazzino_Fabbricato_Cod & ""
            If Id_SfornaturaOld = "" Then
                Id_SfornaturaOld = Id_SfornaturaCorr
                NewSfornatura = True
            Else
                If Id_SfornaturaOld = Id_SfornaturaCorr Then
                    NewSfornatura = False
                Else
                    Id_SfornaturaOld = Id_SfornaturaCorr
                    NewSfornatura = True
                End If
            End If

            'Un movimento di scarico per ciascun codice infornatura
            If NewSfornatura Then


                'genero l'id per associare i movimenti di scarico e carico per infornatura 
                'e scarico da forno e carico magaz in forno per sfornatura da mettere nell'extraint
                Dim idSC As Integer = seq.NuovoId_Tabella("GeneraIdOperazioneDiCuraMovSfornatura", 1000000000, 2000000000, objParametri_Server)

                Dim Movimento_Scarico_Essiccatoio As New Movimento
                Movimento_Scarico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
                Movimento_Scarico_Essiccatoio.Piva = Agenda.Piva
                Movimento_Scarico_Essiccatoio.Sa_Cod = Sa_Cod_Rif
                Movimento_Scarico_Essiccatoio.Data = Data_Fine_Cura.ToShortDateString
                Movimento_Scarico_Essiccatoio.Ora = Ora_Fine_Cura
                Movimento_Scarico_Essiccatoio.Lav_Cod = Lav_Cod
                Movimento_Scarico_Essiccatoio.Cau_Mov = CAU_SCARICO
                Movimento_Scarico_Essiccatoio.Mov_Desc = "Scarico da Essiccatoio Post Cura"
                Movimento_Scarico_Essiccatoio.BaseCode = Agenda.BaseCode
                Movimento_Scarico_Essiccatoio.TopCode = Agenda.TopCode

                Movimento_Scarico_Essiccatoio.Extra_Int = idSC 'necessario per rintracciata

                Dim Movimento_Dettaglio_Scarico_Essiccatoio As New Movimento_Dettaglio
                Movimento_Dettaglio_Scarico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
                Movimento_Dettaglio_Scarico_Essiccatoio.Piva = Agenda.Piva
                Movimento_Dettaglio_Scarico_Essiccatoio.Sa_Cod = Sa_Cod_Rif
                Movimento_Dettaglio_Scarico_Essiccatoio.Data = Data_Fine_Cura
                Movimento_Dettaglio_Scarico_Essiccatoio.Elem_Cod = TRASFORMATI_VEGETALI
                Movimento_Dettaglio_Scarico_Essiccatoio.Pro_Cod = 0
                Movimento_Dettaglio_Scarico_Essiccatoio.Mat_Cod = Mat_Cod_Tabacco_InCura
                Movimento_Dettaglio_Scarico_Essiccatoio.Mov_det_des = "Scarico da Essiccatoio Post Cura"
                Movimento_Dettaglio_Scarico_Essiccatoio.Udm_Cod = enum_UnitaMisura.KG
                Movimento_Dettaglio_Scarico_Essiccatoio.Qta = Cassoni
                Movimento_Dettaglio_Scarico_Essiccatoio.Cal_Cod = 0
                Movimento_Dettaglio_Scarico_Essiccatoio.Lotto = Id_Infornatura
                Movimento_Dettaglio_Scarico_Essiccatoio.Extra_Int = 0
                Movimento_Dettaglio_Scarico_Essiccatoio.Contabilizzato = NONCONTABILE
                Movimento_Dettaglio_Scarico_Essiccatoio.Pendente = enum_Pendenza.MovGiustificato
                Movimento_Dettaglio_Scarico_Essiccatoio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio_Scarico_Essiccatoio.Cau_Mov = Movimento_Scarico_Essiccatoio.Cau_Mov
                Movimento_Dettaglio_Scarico_Essiccatoio.Anno = 1900

                Dim Movimento_Destinazione_Scarico_Essiccatoio As New Movimento_Destinazione
                Movimento_Destinazione_Scarico_Essiccatoio.Id_Agenda = Agenda.Id_Agenda
                Movimento_Destinazione_Scarico_Essiccatoio.Piva = Agenda.Piva
                Movimento_Destinazione_Scarico_Essiccatoio.Sa_Cod = Sa_Cod_Rif
                Movimento_Destinazione_Scarico_Essiccatoio.Id_Destinazione = Fabbricato_Cod_Rif
                Movimento_Destinazione_Scarico_Essiccatoio.Data = Data_Fine_Cura
                Movimento_Destinazione_Scarico_Essiccatoio.Tipo = ESSICCATOIO
                Movimento_Destinazione_Scarico_Essiccatoio.Qta = Movimento_Dettaglio_Scarico_Essiccatoio.Qta
                Movimento_Destinazione_Scarico_Essiccatoio.BaseCode = Agenda.BaseCode
                Movimento_Destinazione_Scarico_Essiccatoio.TopCode = Agenda.TopCode

                Movimento_Dettaglio_Scarico_Essiccatoio.Movimenti_Destinazioni.Add(Movimento_Destinazione_Scarico_Essiccatoio)
                Movimento_Scarico_Essiccatoio.Movimenti_Dettagli.Add(Movimento_Dettaglio_Scarico_Essiccatoio)

                Agenda.Movimenti.Add(Movimento_Scarico_Essiccatoio)




                'faccio un mov di carico per ciascuna infornatura
                If count > 0 Then
                    'se non sono al primo giro aggiungo il movimento precedente
                    Agenda.Movimenti.Add(Movimento_Carico_Magazzino)
                End If

                Movimento_Carico_Magazzino = New Movimento
                Movimento_Carico_Magazzino.Id_Agenda = Agenda.Id_Agenda
                Movimento_Carico_Magazzino.Piva = Agenda.Piva
                Movimento_Carico_Magazzino.Sa_Cod = Agenda.Sa_Cod
                Movimento_Carico_Magazzino.Data = Data_Fine_Cura.ToShortDateString
                Movimento_Carico_Magazzino.Ora = Ora_Fine_Cura
                Movimento_Carico_Magazzino.Lav_Cod = Lav_Cod
                Movimento_Carico_Magazzino.Cau_Mov = CAU_CARICO
                Movimento_Carico_Magazzino.Mov_Desc = "Carichi Lotti nel Magazzino Post Cura"
                Movimento_Carico_Magazzino.BaseCode = Agenda.BaseCode
                Movimento_Carico_Magazzino.TopCode = Agenda.TopCode

                Movimento_Carico_Magazzino.Extra_Int = idSC 'necessario per rintracciata

            End If


            'faccio n movimenti dettagli e destinazioni di carico per ciscuna riga
            Dim Movimento_Dettaglio_Carico_Magazzino As New Movimento_Dettaglio
            Movimento_Dettaglio_Carico_Magazzino.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Carico_Magazzino.Piva = Agenda.Piva
            Movimento_Dettaglio_Carico_Magazzino.Sa_Cod = Magazzino_Sa_Cod
            Movimento_Dettaglio_Carico_Magazzino.Data = Data_Fine_Cura
            Movimento_Dettaglio_Carico_Magazzino.Elem_Cod = TRASFORMATI_VEGETALI
            Movimento_Dettaglio_Carico_Magazzino.Pro_Cod = 0
            Movimento_Dettaglio_Carico_Magazzino.Mat_Cod = Mat_Cod_Tabacco_Curato
            Movimento_Dettaglio_Carico_Magazzino.Mov_det_des = "Carico Lotto nel Magazzino Post Cura (collo " & Collo & ")"
            Movimento_Dettaglio_Carico_Magazzino.Udm_Cod = UdmCaricoMagazzino
            Movimento_Dettaglio_Carico_Magazzino.Qta = Peso
            Movimento_Dettaglio_Carico_Magazzino.Cal_Cod = 0
            Movimento_Dettaglio_Carico_Magazzino.Lotto = Collo 'metto il collo nell'extrastring, dovrò trovare il modo di farlo vedere ma almeno la rintracciata va meglio
            Movimento_Dettaglio_Carico_Magazzino.Extra_Str = Id_Lotto 'id univoco per rintracciata, dato che il lotto potrebbe non essere univoco per aziena 
            Movimento_Dettaglio_Carico_Magazzino.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio_Carico_Magazzino.Pendente = enum_Pendenza.MovGiustificato
            Movimento_Dettaglio_Carico_Magazzino.Lav_Cod = Lav_Cod
            Movimento_Dettaglio_Carico_Magazzino.Cau_Mov = Movimento_Carico_Magazzino.Cau_Mov
            Movimento_Dettaglio_Carico_Magazzino.Anno = 1900

            Dim Movimento_Destinazione_Carico_Magazzino As New Movimento_Destinazione
            Movimento_Destinazione_Carico_Magazzino.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione_Carico_Magazzino.Piva = Agenda.Piva
            Movimento_Destinazione_Carico_Magazzino.Sa_Cod = Magazzino_Sa_Cod
            Movimento_Destinazione_Carico_Magazzino.Id_Destinazione = Magazzino_Fabbricato_Cod
            Movimento_Destinazione_Carico_Magazzino.Data = Data_Fine_Cura
            Movimento_Destinazione_Carico_Magazzino.Tipo = MAGAZZINO
            Movimento_Destinazione_Carico_Magazzino.Qta = Movimento_Dettaglio_Carico_Magazzino.Qta
            Movimento_Destinazione_Carico_Magazzino.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Carico_Magazzino.TopCode = Agenda.TopCode

            Movimento_Dettaglio_Carico_Magazzino.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Magazzino)
            Movimento_Carico_Magazzino.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Magazzino)

            count += 1
            If count = DtSfornature.Rows.Count Then
                'aggiungo l'ultimo movimento
                Agenda.Movimenti.Add(Movimento_Carico_Magazzino)
            End If

        Next

        Return Agenda

    End Function

    Public Shared Function GeneraProdottoCuraTabacco(ByVal objParametri_Server As AgronicaCoreParametri, _
                                                     ByVal ASG_ProgressivoGIAS As Integer, _
                                                     ByVal NomeProdottoTabaccoCurato As String, _
                                                     ByVal PrefissoCodArticolo As String, _
                                                     ByVal veg_cod As Integer) As Integer


        Dim OUTPUT_Mat_Cod As Integer
        'Dim veg_cod As Integer = 335
        ' Dim Veg_Des As String = "Tabacco"
        Dim Piva_Creazione As String = objParametri_Server.PivaSuperUser 'azienda che lo crea
        Dim Sa_Cod_Creazione As Integer = PUBBLICO 'pubblico
        'altre=5010411
        'Virginia Bright=5010624
        Dim Cul_Cod As Integer = New AgronicaCoreMetaSchemaDAL.Cultivar_R().CulCod_Altre_from_VegCod(veg_cod, objParametri_Server)
        If Cul_Cod < 1 Then
            Throw New Exception("non c'è la varietà altre")
        End If
        Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
        Dim Regolamento As enum_Cod_Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
        Dim Flag_Biologico As Boolean = False

        Dim Cod_Articolo As String = Right("000" & veg_cod, 3) & _
                       "/" & Right("00000000" & CStr(Cul_Cod), 8)

        Cod_Articolo = PrefissoCodArticolo & Cod_Articolo

        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim flag_esiste As Boolean = True

        Dim m As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
        Dim vegdes As String = m.VegDes_from_VegCod(veg_cod, objParametri_Server)

        flag_esiste = objCore_MP_R.Esiste_TrasformatoVegetale(Piva_Creazione,
                                              veg_cod,
                                              Cul_Cod,
                                               0,
                                               " Mat_Des = '" & NomeProdottoTabaccoCurato & " (" & vegdes & ")' and  Cod_Articolo = '" & Cod_Articolo & "' and   Sa_Cod = -1 ",
                                                objParametri_Server)


        If flag_esiste = False Then

            Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
            Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
            Dim Flag_Insert As Boolean
            Dim Basecode As Integer = 0
            Dim Topcode As Integer = 0
            AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode, _
                                                                   Topcode, _
                                                                    ASG_ProgressivoGIAS)



            objImportaGias.Creazione_Automatica_TrasformatoVegetale(objParametri_Server, _
                                                    objCore_XML_Anagrafe, _
                                                     objCore_MP_W, _
                                                     Flag_Insert, _
                                                     OUTPUT_Mat_Cod, _
                                                     Basecode, _
                                                     Topcode, _
                                                     Piva_Creazione, _
                                                     Sa_Cod_Creazione, _
                                                     Cod_Articolo, _
                                                     NomeProdottoTabaccoCurato & " (" & vegdes & ")", _
                                                     veg_cod, _
                                                     Cul_Cod, _
                                                     Regolamento, _
                                                     Flag_Biologico)

        Else
            Throw New Exception("Il prodotto esiste")
        End If

        Return OUTPUT_Mat_Cod

    End Function

    Public Shared Function CreaOggettoAgendaScaricoProdottoPerInvioAziendaOrigine(ByVal MovimentiCure As List(Of Movimento), _
                                                                    ByVal Id_Agenda As Integer, ByVal Tipo_Operazione As Integer, ByVal ProvenienzaDesc As String) As Operazione_Agenda


        Dim Agenda As New Operazione_Agenda
        Agenda.Tipo_Operazione = Tipo_Operazione
        Agenda.Id_Agenda = Id_Agenda
        'Agenda.Id_Agenda =
        Agenda.Data = MovimentiCure(0).Data
        Agenda.Piva = MovimentiCure(0).Piva
        Agenda.Sa_Cod = MovimentiCure(0).Sa_Cod
        Agenda.Lav_Cod = LAVCOD_SCARICO
        Agenda.Des_Lib = "Scarico Prodotto Curato per Invio ad Azienda Origine (Az: " & ProvenienzaDesc & ")"
        Agenda.BaseCode = MovimentiCure(0).BaseCode
        Agenda.TopCode = MovimentiCure(0).TopCode

        For Each Movimento As Movimento In MovimentiCure

            Dim Movimento_Scarico As New Movimento
            Movimento_Scarico.Id_Agenda = Id_Agenda
            Movimento_Scarico.Piva = Movimento.Piva
            Movimento_Scarico.Sa_Cod = Movimento.Sa_Cod
            Movimento_Scarico.Data = Movimento.Data
            Movimento_Scarico.Ora = Movimento.Ora
            Movimento_Scarico.Lav_Cod = Agenda.Lav_Cod
            Movimento_Scarico.Cau_Mov = CAU_SCARICO
            Movimento_Scarico.Mov_Desc = "Scarico Prodotto Curato per Invio ad Azienda Origine (" & ProvenienzaDesc & ")"
            Movimento_Scarico.BaseCode = Agenda.BaseCode
            Movimento_Scarico.TopCode = Agenda.TopCode

            For Each MovimentoDettaglioCarico As Movimento_Dettaglio In Movimento.Movimenti_Dettagli

                Dim MovimentoDettaglioScarico As New Movimento_Dettaglio
                MovimentoDettaglioScarico.Id_Agenda = Agenda.Id_Agenda
                MovimentoDettaglioScarico.Piva = MovimentoDettaglioCarico.Piva
                MovimentoDettaglioScarico.Sa_Cod = MovimentoDettaglioCarico.Sa_Cod
                MovimentoDettaglioScarico.Data = MovimentoDettaglioCarico.Data
                MovimentoDettaglioScarico.Elem_Cod = MovimentoDettaglioCarico.Elem_Cod
                MovimentoDettaglioScarico.Pro_Cod = MovimentoDettaglioCarico.Pro_Cod
                MovimentoDettaglioScarico.Cod_Progetto = MovimentoDettaglioCarico.Cod_Progetto
                MovimentoDettaglioScarico.Mat_Cod = MovimentoDettaglioCarico.Mat_Cod
                MovimentoDettaglioScarico.Mov_det_des = "Scarico Prodotto Curato per Invio ad Azienda Origine (" & ProvenienzaDesc & ")"
                MovimentoDettaglioScarico.Udm_Cod = MovimentoDettaglioCarico.Udm_Cod
                MovimentoDettaglioScarico.Qta = MovimentoDettaglioCarico.Qta
                MovimentoDettaglioScarico.Cal_Cod = MovimentoDettaglioCarico.Cal_Cod
                MovimentoDettaglioScarico.Lotto = MovimentoDettaglioCarico.Lotto
                MovimentoDettaglioScarico.Contabilizzato = MovimentoDettaglioCarico.Contabilizzato
                MovimentoDettaglioScarico.Pendente = MovimentoDettaglioCarico.Pendente
                MovimentoDettaglioScarico.Lav_Cod = Agenda.Lav_Cod
                MovimentoDettaglioScarico.Cau_Mov = CAU_SCARICO
                MovimentoDettaglioScarico.Extra_Str = MovimentoDettaglioCarico.Extra_Str
                MovimentoDettaglioScarico.Extra_Int = MovimentoDettaglioCarico.Extra_Int
                MovimentoDettaglioScarico.Anno = MovimentoDettaglioCarico.Anno

                For Each MovimentoDestinazioneCarico As Movimento_Destinazione In MovimentoDettaglioCarico.Movimenti_Destinazioni

                    Dim MovimentoDestinazioneScarico As New Movimento_Destinazione
                    MovimentoDestinazioneScarico.Id_Agenda = Agenda.Id_Agenda
                    MovimentoDestinazioneScarico.Data = MovimentoDestinazioneCarico.Data
                    MovimentoDestinazioneScarico.Piva = MovimentoDestinazioneCarico.Piva
                    MovimentoDestinazioneScarico.Sa_Cod = MovimentoDestinazioneCarico.Sa_Cod
                    MovimentoDestinazioneScarico.Id_Destinazione = MovimentoDestinazioneCarico.Id_Destinazione
                    MovimentoDestinazioneScarico.Tipo = MAGAZZINO
                    MovimentoDestinazioneScarico.Qta = MovimentoDestinazioneCarico.Qta
                    MovimentoDestinazioneScarico.BaseCode = Agenda.BaseCode
                    MovimentoDestinazioneScarico.TopCode = Agenda.TopCode

                    MovimentoDettaglioScarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneScarico)
                Next
                Movimento_Scarico.Movimenti_Dettagli.Add(MovimentoDettaglioScarico)

            Next

            Agenda.Movimenti.Add(Movimento_Scarico)

        Next

        Return Agenda

    End Function

    Public Shared Function CreaOggettoAgendaCaricoPressoAziendaOrigine(ByVal MovimentiCure As List(Of Movimento), _
                                                        PivaOrigine As String, _
                                                        da_cod_origine As Integer, _
                                                        fabbricato_cod_origine As Integer, _
                                                        ByVal Id_Agenda As Integer, ByVal Tipo_Operazione As Integer, ByVal udsDescr As String) As Operazione_Agenda

        Dim Agenda As New Operazione_Agenda
        Agenda.Tipo_Operazione = Tipo_Operazione
        Agenda.Id_Agenda = Id_Agenda
        'Agenda.Id_Agenda =
        Agenda.Data = MovimentiCure(0).Data
        Agenda.Piva = PivaOrigine
        Agenda.Sa_Cod = da_cod_origine
        Agenda.Lav_Cod = LAVCOD_CARICO
        Agenda.Des_Lib = "Carico Prodotto Curato dall'uds " & udsDescr & " nell'Azienda Origine "
        Agenda.BaseCode = MovimentiCure(0).BaseCode
        Agenda.TopCode = MovimentiCure(0).TopCode

        For Each Movimento As Movimento In MovimentiCure

            Dim Movimento_Carico As New Movimento
            Movimento_Carico.Id_Agenda = Agenda.Id_Agenda
            Movimento_Carico.Piva = PivaOrigine
            Movimento_Carico.Sa_Cod = da_cod_origine
            Movimento_Carico.Data = Movimento.Data
            Movimento_Carico.Ora = Movimento.Ora
            Movimento_Carico.Lav_Cod = Agenda.Lav_Cod
            Movimento_Carico.Cau_Mov = CAU_CARICO
            Movimento_Carico.Mov_Desc = "Carico Prodotto Curato dall'uds " & udsDescr & " nell'Azienda Origine "
            Movimento_Carico.BaseCode = Agenda.BaseCode
            Movimento_Carico.TopCode = Agenda.TopCode

            For Each MovimentoDettaglio As Movimento_Dettaglio In Movimento.Movimenti_Dettagli

                Dim MovimentoDettaglioCarico As New Movimento_Dettaglio
                MovimentoDettaglioCarico.Id_Agenda = Agenda.Id_Agenda
                MovimentoDettaglioCarico.Piva = Agenda.Piva
                MovimentoDettaglioCarico.Sa_Cod = Agenda.Sa_Cod
                MovimentoDettaglioCarico.Data = MovimentoDettaglio.Data
                MovimentoDettaglioCarico.Elem_Cod = MovimentoDettaglio.Elem_Cod
                MovimentoDettaglioCarico.Pro_Cod = MovimentoDettaglio.Pro_Cod
                MovimentoDettaglioCarico.Cod_Progetto = MovimentoDettaglio.Cod_Progetto
                MovimentoDettaglioCarico.Mat_Cod = MovimentoDettaglio.Mat_Cod
                MovimentoDettaglioCarico.Mov_det_des = "Carico Prodotto Curato dall'uds " & udsDescr & " nell'Azienda Origine "
                MovimentoDettaglioCarico.Udm_Cod = MovimentoDettaglio.Udm_Cod
                MovimentoDettaglioCarico.Qta = MovimentoDettaglio.Qta
                MovimentoDettaglioCarico.Cal_Cod = MovimentoDettaglio.Cal_Cod
                MovimentoDettaglioCarico.Lotto = MovimentoDettaglio.Lotto
                MovimentoDettaglioCarico.Contabilizzato = MovimentoDettaglio.Contabilizzato
                MovimentoDettaglioCarico.Pendente = MovimentoDettaglio.Pendente
                MovimentoDettaglioCarico.Lav_Cod = Agenda.Lav_Cod
                MovimentoDettaglioCarico.Cau_Mov = CAU_CARICO
                MovimentoDettaglioCarico.Extra_Str = MovimentoDettaglio.Extra_Str
                MovimentoDettaglioCarico.Extra_Int = MovimentoDettaglio.Extra_Int
                MovimentoDettaglioCarico.Anno = MovimentoDettaglio.Anno

                For Each MovimentoDestinazione As Movimento_Destinazione In MovimentoDettaglio.Movimenti_Destinazioni

                    Dim MovimentoDestinazioneCarico As New Movimento_Destinazione
                    MovimentoDestinazioneCarico.Id_Agenda = Agenda.Id_Agenda
                    MovimentoDestinazioneCarico.Data = MovimentoDestinazione.Data
                    MovimentoDestinazioneCarico.Piva = Agenda.Piva
                    MovimentoDestinazioneCarico.Sa_Cod = Agenda.Sa_Cod
                    MovimentoDestinazioneCarico.Id_Destinazione = fabbricato_cod_origine
                    MovimentoDestinazioneCarico.Tipo = MAGAZZINO
                    MovimentoDestinazioneCarico.Qta = MovimentoDestinazione.Qta
                    MovimentoDestinazioneCarico.BaseCode = Agenda.BaseCode
                    MovimentoDestinazioneCarico.TopCode = Agenda.TopCode

                    MovimentoDettaglioCarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneCarico)
                Next
                Movimento_Carico.Movimenti_Dettagli.Add(MovimentoDettaglioCarico)
            Next

            Agenda.Movimenti.Add(Movimento_Carico)

        Next

        Return Agenda

    End Function

End Class