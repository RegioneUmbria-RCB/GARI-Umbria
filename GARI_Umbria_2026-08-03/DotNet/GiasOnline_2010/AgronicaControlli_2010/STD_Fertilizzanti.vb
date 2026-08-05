Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class STD_Fertilizzanti

    Public Function LeggiFertilizzanti(tipoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita,
                                       statoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Stati,
                                       lavorazione As AgronicaCoreModelsSTD.attivita.Lavorazione,
                                       impianti As Impianto(),
                                       disciplinare As metaschema.Disciplinare,
                                       filtroPerDescrizione As String,
                                       validitaFine As DateTime,
                                       escludiGiacenzeZero As Boolean,
                                       magazziniAgenzie As Boolean,
                                       magazziniEsterni As Boolean,
                                       tipoRicetta As enum_TipoRicetta, 'TODO_DT: completare sviluppo ricette
                                       pua As Pua, 'TODO_DT: arriverà da PUA, da sviluppare insieme alle ricette
                                       codiceFertilizzante As Integer,
                                       soloLetturaAnagrafica As Boolean,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)

        Dim fertilizzantiList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)

        Dim sa_cod_list = STD_Utility.getSaCodDaImpianti(impianti)
        Dim piva = STD_Utility.getPivaDaImpianti(impianti)

        Dim chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK) = Nothing

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.FERTILIZZANTI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FERTILIZZANTI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.FERTILIZZANTI, enum_Gestione_Lotti.Nessuna, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        'DT: in caso di magazzino esterno partiamo prendendo tutti i prodotti, senza considerare la giacenza e filtriamo successivamente, in base alle impostazioni di ogni singolo magazzino esterno
        If magazziniEsterni Then
            gestioneGiacenze = enum_Gestione_Giacenze.TuttiProdotti
        End If

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)

        Dim DtEffluenti As DataTable = Nothing
        Dim objParametriUscitaEff As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output = Nothing

        Dim DtFertilizzantiOrdinati = New DataTable
        DtFertilizzantiOrdinati.Columns.Add(New DataColumn("FrDes", GetType(String)))
        DtFertilizzantiOrdinati.Columns.Add(New DataColumn("DettaglioFertilizzazione", GetType(AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione)))
        DtFertilizzantiOrdinati.Columns.Add(New DataColumn("ConGiacenza", GetType(Integer)))

        Dim TipoRichiesto As Integer = 0
        Dim RegolamentoCod As Integer = 0
        Dim Tipo_PuaRegolamento As Integer = 0

        If disciplinare IsNot Nothing AndAlso disciplinare.regolamentoConcimazione IsNot Nothing Then
            RegolamentoCod = disciplinare.regolamentoConcimazione.codice
            Tipo_PuaRegolamento = disciplinare.regolamentoConcimazione.tipo
        End If

        If Tipo_PuaRegolamento = 2 Then
            Select Case RegolamentoCod
                Case enum_PUARegolamenti.PUA_ER_2001, enum_PUARegolamenti.PUA_ER_2012
                    TipoRichiesto = 6
                Case Else
                    TipoRichiesto = RegolamentoCod
            End Select
        End If


        Dim Pua_Cod As Integer = 0
        Dim RegolamentoCodPUA As Integer = 0

        If lavorazione.primaryKey.codice = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
            If pua IsNot Nothing Then
                Pua_Cod = pua.codice

                If Not IsNothing(pua.disciplinare) Then
                    If Not IsNothing(pua.disciplinare.regolamentoConcimazione) Then
                        RegolamentoCodPUA = pua.disciplinare.regolamentoConcimazione.codice
                    Else
                        RegolamentoCodPUA = pua.disciplinare.codice
                    End If
                End If
            End If
        End If

        Dim strFiltroFerCod As String = ""

        Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"

        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing

        'Caricamento magazzini agenzie, se presenti
        Dim filtroMagazziniEsterni = STD_Utility.getfiltroMagazziniEsterni(piva, tipoAttivita, statoAttivita, magazziniAgenzie, magazziniEsterni, chiaviMagazziniUso_da_terzi,
                                                                            objParametri_Server, objParametri_Utenti)

        If soloLetturaAnagrafica Then
            usaAnagrafica = True
            usaMagazzino = False
        End If

        'Se utilizzo il magazzino leggo prima i prodotti in giacenza
        If usaMagazzino Then

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                           piva, 0, 0,
                                                           FERTILIZZANTI,
                                                           Pro_Cod:=codiceFertilizzante,
                                                           0, 0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                           xFiltroAggiuntivo_MagazzinoAttivoAllaData, "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "", "",
                                                           "",
                                                           objParametri_Server, objParametri_Utenti,
                                                           Flag_QtaMaggioreZero:=flagQtaMaggioreZero,
                                                           filtroMagazziniEsterni:=filtroMagazziniEsterni)

            Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                               piva, 0, 0,
                                                               FERTILIZZANTI,
                                                               Pro_Cod:=codiceFertilizzante,
                                                               0, 0, 0, 0, 0,
                                                               LOTTO_NONDEFINITO,
                                                               Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                               xFiltroAggiuntivo_MagazzinoAttivoAllaData, "",
                                                               "", "",
                                                               "", "",
                                                               "", "",
                                                               "", "",
                                                               "", "",
                                                               "",
                                                               objParametri_Server, objParametri_Utenti,
                                                               Flag_QtaMaggioreZero:=flagQtaMaggioreZero,
                                                               filtroMagazziniEsterni:=filtroMagazziniEsterni)

            STD_Utility.FiltraGiacenze_X_MagazziniUso_da_terzi(chiaviMagazziniUso_da_terzi, Dt_Giacenze, Dt_Giacenze_Tot)

            If Dt_Giacenze.Rows.Count > 0 Then
                Dim dr() As DataRow = Dt_Giacenze.Select(" Descrizione_Prodotto Like'%" & filtroPerDescrizione & "%'")

                For i = 0 To dr.Count - 1

                    Dim prodToFiter = " Fertilizzanti.Fer_Cod=" & dr(i).Item("Pro_Cod").ToString & " OR "
                    If Not strFiltroFerCod.Contains(prodToFiter) Then

                        'Anna [rif chiamata 17160] microgiacenze (rivisto da DT per NG)
                        If flagQtaMaggioreZero = True Then
                            If Math.Abs(dr(i).Item("Giacenza")) > QTA_GiancenzeVisualizzate Then
                                strFiltroFerCod &= prodToFiter
                            End If
                        Else
                            strFiltroFerCod &= prodToFiter
                        End If
                    End If

                Next
            End If
        End If


        If Pua_Cod > 0 Then

            'leggo gli effluenti del regolamento
            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            DtEffluenti = objEff.Leggi(RegolamentoCodPUA, Pua_Cod, 0, " azoto_qta > 0 ", "", objParametri_Server)

            Dim objParametriIngressoEff As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
            objParametriIngressoEff.Regolamento_Cod = RegolamentoCodPUA
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscitaEff = objPC_WS.Effluenti(objParametriIngressoEff, objParametri_Server, objParametri_Super_Server)

        End If


        If strFiltroFerCod <> "" Then
            strFiltroFerCod = Left(strFiltroFerCod, strFiltroFerCod.Length - 3)
            strFiltroFerCod = " (" & strFiltroFerCod & ")"
        Else
            'se non ho alcun fertilizzante in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
            If Not usaAnagrafica Then
                Return fertilizzantiList
            End If
        End If

        Dim strFiltroAggiuntivo = ""
        If usaMagazzino AndAlso Not usaAnagrafica Then
            strFiltroAggiuntivo &= strFiltroFerCod
        End If

        Dim statoCod As String = STD_Utility.getStato(piva, sa_cod_list, objParametri_Server)

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
        objParametriIngresso.Codice = codiceFertilizzante
        objParametriIngresso.Descrizione = filtroPerDescrizione
        objParametriIngresso.DataInizio = AGRODATAINIZIO
        objParametriIngresso.DataFine = AGRODATAFINE
        objParametriIngresso.Tipo = TipoRichiesto
        objParametriIngresso.IncludiApporti = True
        objParametriIngresso.IncludiTipologia = True
        objParametriIngresso.Regolamento = RegolamentoCod
        objParametriIngresso.strFiltro = strFiltroAggiuntivo
        objParametriIngresso.Stato_Cod = statoCod
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim agroWs As String
        Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_Server)
        If agroWs = "" Then
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_Super_Server)
        End If
        objParametriIngresso.Url = agroWs & "/Fertilizzanti"

        Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output = objFert_WS.Fertilizzanti(objParametriIngresso)

        Dim strTipologie As String = ""
        Dim x_Cod As String
        Dim x_Des As String

        Dim DrGiacenze() As DataRow = Nothing
        Dim DrGiacenze_Tot() As DataRow = Nothing

        For Each fertilizzanteMetaschema In objParametriUscita.ListaFertilizzanti

            If TipoRichiesto >= 6 Then
                x_Des = CStr(fertilizzanteMetaschema.TipoFertilizzanteDescrizione) & " - " & fertilizzanteMetaschema.Descrizione
            Else
                x_Des = fertilizzanteMetaschema.Descrizione
            End If

            x_Cod = fertilizzanteMetaschema.Codice

            If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                DrGiacenze = Dt_Giacenze.Select("pro_cod=" & fertilizzanteMetaschema.Codice)
            End If

            Dim dettaglioFertilizzazioneNPK = CreaDettaglioFertilizzazione(fertilizzanteMetaschema, TipoRichiesto, Pua_Cod, DtEffluenti, objParametriUscitaEff)

            Dim trovataGiacenza As Boolean = False
            If usaMagazzino Then

                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                    Dim DtMovimentiXCalcoloNPK As DataTable = Nothing

                    For g = 0 To DrGiacenze.Length - 1

                        If magazziniEsterni Then
                            Dim gestioneGiacenzeMagazzinoEsterno = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(DrGiacenze(g).Item("Piva"), Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FERTILIZZANTI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
                            Dim qtaMagazzinoEsterno = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                            If gestioneGiacenzeMagazzinoEsterno = enum_Gestione_Giacenze.SoloPresenti Then
                                If qtaMagazzinoEsterno <= 0 Then
                                    Continue For
                                End If
                            End If
                        End If

                        Dim dettaglioFertilizzazioneNPKGiacenza = dettaglioFertilizzazioneNPK.Clona()

                        Dim rilevamentoMagazzino = New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino()
                        rilevamentoMagazzino.Qta = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                        rilevamentoMagazzino.udm = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura()
                        rilevamentoMagazzino.udm.codice = DrGiacenze(g).Item("Udm_Cod")
                        rilevamentoMagazzino.udm.simbolo = DrGiacenze(g).Item("Udm_Sim")
                        rilevamentoMagazzino.Lotto = DrGiacenze(g).Item("lotto")
                        rilevamentoMagazzino.Magazzino = New AgronicaCoreModelsSTD.anagrafiche.Fabbricato()
                        rilevamentoMagazzino.Magazzino.primaryKey = New anagrafiche.Fabbricato.PK()
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK = New anagrafiche.CentroAziendale.PK()
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva = DrGiacenze(g).Item("Piva")
                        rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice = DrGiacenze(g).Item("Sa_Cod")
                        rilevamentoMagazzino.Magazzino.primaryKey.codice = DrGiacenze(g).Item("Id_Destinazione")
                        rilevamentoMagazzino.Magazzino.descrizione = CStr(DrGiacenze(g).Item("Fabbricato_Des")) & " (" & CStr(DrGiacenze(g).Item("Sa_Nome")) & ")"

                        If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                            DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & fertilizzanteMetaschema.Codice & " and Piva='" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod=" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
                            If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                rilevamentoMagazzino.QtaTot = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4)
                            End If
                        End If

                        If Not IsNothing(chiaviMagazziniUso_da_terzi) AndAlso chiaviMagazziniUso_da_terzi.FindIndex(Function(u) u.centroAziendalePK.partitaIva = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                                                                u.centroAziendalePK.codice = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                                                                u.codice = rilevamentoMagazzino.Magazzino.primaryKey.codice) > -1 Then
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = True

                            'Se il magazzino ha il fabbricato codice Uso da Terzi aggiungo alla descrizione del Magazzino l'Azienda
                            rilevamentoMagazzino.Magazzino.descrizione &= " (" & AgronicaCoreDataProvider.My.Resources.Gias.Azienda & ": " & CStr(DrGiacenze(g).Item("Impresa")) & ")"
                        Else
                            rilevamentoMagazzino.Magazzino.usoDaTerzi = False
                        End If

                        CalcolaNPK(dettaglioFertilizzazioneNPKGiacenza, TipoRichiesto, validitaFine, DrGiacenze(g).Item("Udm_Cod"), rilevamentoMagazzino, objParametri_Server, x_Cod, x_Des, DtMovimentiXCalcoloNPK)

                        dettaglioFertilizzazioneNPKGiacenza.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        Dim row = DtFertilizzantiOrdinati.NewRow
                        row.Item("FrDes") = x_Des
                        row.Item("DettaglioFertilizzazione") = dettaglioFertilizzazioneNPKGiacenza
                        row.Item("ConGiacenza") = 1
                        DtFertilizzantiOrdinati.Rows.Add(row)

                        trovataGiacenza = True
                    Next

                End If

            End If

            'DT: il fertilizzante va restituito anche se presente solo in anagrafica se "usaAnagrafica=true"
            'DT: se si sta usando anche il magazzino, si restituisce il fertilizzante (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
            If usaAnagrafica AndAlso Not trovataGiacenza Then
                Dim row = DtFertilizzantiOrdinati.NewRow
                row.Item("FrDes") = x_Des
                row.Item("DettaglioFertilizzazione") = dettaglioFertilizzazioneNPK
                row.Item("ConGiacenza") = 0
                DtFertilizzantiOrdinati.Rows.Add(row)
            End If
        Next

        If DtFertilizzantiOrdinati IsNot Nothing Then

            Dim Dv As New DataView()
            DtFertilizzantiOrdinati.TableName = "Fertilizzanti"
            Dv.Table = DtFertilizzantiOrdinati
            Dv.Sort = "ConGiacenza DESC, FrDes ASC"

            For i = 0 To Dv.Count - 1
                fertilizzantiList.Add(Dv(i).Item("DettaglioFertilizzazione"))
            Next

        End If

        Return fertilizzantiList

    End Function

    Public Function LeggiEfficienza(effluente As Effluente,
                                    epoca As Epoca,
                                    tipoAllevamento As TipoAllevamento,
                                    valoreDose As Integer,
                                    disciplinare As Disciplinare,
                                    classiTessitura As ClasseTessitura(),
                                    specie As Specie,
                                    objParametri_Super_Server As AgronicaCoreParametri,
                                    objParametri_Server As AgronicaCoreParametri) As Decimal

        Dim efficienza As Decimal = 1

        Dim effluente_cod = 0
        If effluente IsNot Nothing Then
            effluente_cod = effluente.codice
        End If

        Dim tipoAllevamento_cod = 0
        If tipoAllevamento IsNot Nothing Then
            tipoAllevamento_cod = tipoAllevamento.codice
        End If

        Dim epoca_cod = 0
        If epoca IsNot Nothing Then
            epoca_cod = epoca.codice
        End If

        Dim regolamento_cod = 0
        If disciplinare IsNot Nothing AndAlso disciplinare.regolamentoConcimazione IsNot Nothing Then
            regolamento_cod = disciplinare.regolamentoConcimazione.codice
        End If

        Dim veg_cod = 0
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            veg_cod = specie.codice
        End If

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

        GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
        If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
        End If

        Dim objParametriIngresso As New PUA_EfficienzaxTipiAllevamentixEffluenti_input
        objParametriIngresso.Regolamento_Cod = regolamento_cod
        objParametriIngresso.Eff_Cod = effluente_cod
        objParametriIngresso.Em_Cod = epoca_cod
        If regolamento_cod < 78 Then
            objParametriIngresso.All_Cod = tipoAllevamento_cod
        End If
        objParametriIngresso.Dose = valoreDose
        objParametriIngresso.Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PUA_EfficienzaxTipiAllevamentixEffluenti_output = objPC_WS.EfficienzaxTipiAllevamentixEffluenti(objParametriIngresso)

        If objParametriUscita.ListaEfficienzaxTipiAllevamentixEffluenti.Count > 0 Then

            If classiTessitura IsNot Nothing AndAlso classiTessitura.Count > 0 Then

                'Devo prendere la massima efficienza perché implica un minor dosaggio e quindi sono più cautelativo
                Dim maxEfficienza As Decimal = 0

                For Each classeTessitura In classiTessitura

                    Dim cltes As Integer = classeTessitura.codice
                    Dim attEff As Decimal = (From x As PUA_Efficienza In objParametriUscita.ListaEfficienzaxTipiAllevamentixEffluenti Where x.Id_ClasseTessitura = cltes Select x.Efficienza_Val).Max()

                    If maxEfficienza < attEff Then
                        maxEfficienza = attEff
                    End If

                Next

                efficienza = maxEfficienza

            Else

                efficienza = (From x As PUA_Efficienza In objParametriUscita.ListaEfficienzaxTipiAllevamentixEffluenti Select x.Efficienza_Val).Max()

            End If

            Dim Considera_CoefficienteTempo As Integer = (From x As PUA_Efficienza In objParametriUscita.ListaEfficienzaxTipiAllevamentixEffluenti Select x.Considera_CoefficienteTempo_Efficienza).First()
            If Considera_CoefficienteTempo = 1 Then
                Dim objParametriIngressoCT As New PUA_CoefficienteTempo_Coltura_input
                objParametriIngressoCT.Regolamento_Cod = regolamento_cod
                objParametriIngressoCT.Veg_Cod = veg_cod
                Dim objParametriUscitaCT As PUA_CoefficienteTempo_Coltura_output = objPC_WS.CoefficienteTempo_Coltura(objParametriIngressoCT)
                If objParametriUscitaCT.ListaCoefficienteTempo.Count > 0 Then
                    Dim CoefficienteTempo As Decimal = (From x As PUA_CoefficienteTempo In objParametriUscitaCT.ListaCoefficienteTempo Select x.Coeff_Temp).First()
                    If CoefficienteTempo > 0 Then
                        efficienza = efficienza * CoefficienteTempo
                    End If
                End If
            End If

        End If

        Return efficienza

    End Function

    Private Sub CalcolaNPK(dettaglioFertilizzazione As dettagli.DettaglioFertilizzazione, TipoRichiesto As Integer, validitaFine As Date, UdmG As Integer,
                           rilevamentoMagazzino As RilevamentoDiMagazzino, objParametri_Server As AgronicaCoreParametri, ByRef x_Cod As String, ByRef x_Des As String, ByRef DtMovimentiXCalcoloNPK As DataTable)

        Dim mediaPesataN As Decimal = 0
        Dim strN As String = ""

        'Calcolo l'N,P,K del dettaglioFertilizzazione con la media ponderata dei carichi di magazzino, se non sono stati indicati msotro quelli letti da metaschema

        Dim mediaPesataP2O5 As Decimal = 0
        Dim mediaPesataK2O As Decimal = 0
        Dim mediaPesataMgO As Decimal = 0
        Dim mediaPesataCu As Decimal = 0

        'DT: solo per le direttive nitrati con magazzino si calcolo la media ponderata dei titoli indicati nei carichi
        'DT: era >6, ora abbiamo messo >=6
        If TipoRichiesto >= 6 AndAlso Not IsNothing(rilevamentoMagazzino) Then

            Dim Piva As String = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva
            Dim Sa_Cod As Integer = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice
            Dim Id_Destinazione As Integer = rilevamentoMagazzino.Magazzino.primaryKey.codice
            Dim lotto As String = rilevamentoMagazzino.Lotto

            If IsNothing(DtMovimentiXCalcoloNPK) Then
                Dim lavs_cod As Integer() = {LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_CARICO, LAVCOD_NOTA_ACCREDITO_EMESSA}
                Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R


                DtMovimentiXCalcoloNPK = objMovimenti.LeggiMovimentixPUA(Piva, lavs_cod, FERTILIZZANTI, dettaglioFertilizzazione.prodotto.codice, enum_Agenda_Causali.CARICO,
                                                                                           AGRODATAINIZIO, validitaFine, UdmG, "", "", objParametri_Server)
            End If


            If Not IsNothing(DtMovimentiXCalcoloNPK) Then

                Dim drs As DataRow() = DtMovimentiXCalcoloNPK.Select("Piva = '" & Piva & "' And Sa_Cod = " & Sa_Cod &
                                                                     " And Id_Destinazione = " & Id_Destinazione & " And lotto = '" & lotto & "'")

                If Not IsNothing(drs) AndAlso drs.Length > 0 Then

                    Dim divisoreMediaPesataN As Decimal = 0
                    Dim divisoreMediaPesataP2O5 As Decimal = 0
                    Dim divisoreMediaPesataK2O As Decimal = 0
                    Dim divisoreMediaPesataMgO As Decimal = 0
                    Dim divisoreMediaPesataCu As Decimal = 0

                    For Each dr As DataRow In drs

                        Dim qta As Decimal = CDec(dr.Item("qta")) / CDec(1000)

                        Dim N As Decimal = CDec(dr.Item("N"))
                        Dim P As Decimal = CDec(dr.Item("P"))
                        Dim K As Decimal = CDec(dr.Item("K"))
                        Dim Mg As Decimal = CDec(dr.Item("Mg"))
                        Dim Cu As Decimal = CDec(dr.Item("Cu"))

                        If N > 0 Then
                            mediaPesataN += (N * qta)
                            divisoreMediaPesataN += qta
                        End If
                        If P > 0 Then
                            mediaPesataP2O5 += (P * qta)
                            divisoreMediaPesataP2O5 += qta
                        End If
                        If K > 0 Then
                            mediaPesataK2O += (K * qta)
                            divisoreMediaPesataK2O += qta
                        End If
                        If Mg > 0 Then
                            mediaPesataMgO += (Mg * qta)
                            divisoreMediaPesataMgO += qta
                        End If
                        If Cu > 0 Then
                            mediaPesataCu += (Cu * qta)
                            divisoreMediaPesataCu += qta
                        End If

                    Next

                    If divisoreMediaPesataN <> 0 Then
                        mediaPesataN = mediaPesataN / divisoreMediaPesataN
                    End If
                    If divisoreMediaPesataP2O5 <> 0 Then
                        mediaPesataP2O5 = mediaPesataP2O5 / divisoreMediaPesataP2O5
                    End If
                    If divisoreMediaPesataK2O <> 0 Then
                        mediaPesataK2O = mediaPesataK2O / divisoreMediaPesataK2O
                    End If
                    If divisoreMediaPesataMgO <> 0 Then
                        mediaPesataMgO = mediaPesataMgO / divisoreMediaPesataMgO
                    End If
                    If divisoreMediaPesataCu <> 0 Then
                        mediaPesataCu = mediaPesataCu / divisoreMediaPesataCu
                    End If

                End If
            End If

        End If

        strN = Format(IIf(mediaPesataN = 0, dettaglioFertilizzazione.N, mediaPesataN), "0.####")
        If mediaPesataN > 0 AndAlso mediaPesataN <> dettaglioFertilizzazione.N Then
            strN += "*"
            dettaglioFertilizzazione.N_Ponderato = True
        End If
        Dim strP2O5 As String = Format(IIf(mediaPesataP2O5 = 0, dettaglioFertilizzazione.P, mediaPesataP2O5), "0.####")
        If mediaPesataP2O5 > 0 AndAlso mediaPesataP2O5 <> dettaglioFertilizzazione.P Then
            strP2O5 += "*"
            dettaglioFertilizzazione.P_Ponderato = True
        End If
        Dim strK2O As String = Format(IIf(mediaPesataK2O = 0, dettaglioFertilizzazione.K, mediaPesataK2O), "0.####")
        If mediaPesataK2O > 0 AndAlso mediaPesataK2O <> dettaglioFertilizzazione.K Then
            strK2O += "*"
            dettaglioFertilizzazione.K_Ponderato = True
        End If
        Dim strCu As String = Format(IIf(mediaPesataCu = 0, dettaglioFertilizzazione.Cu, mediaPesataCu), "0.####")
        If mediaPesataCu > 0 AndAlso mediaPesataCu <> dettaglioFertilizzazione.Cu Then
            strCu += "*"
            dettaglioFertilizzazione.Cu_Ponderato = True
        End If
        '(16/06/2017 fede) sostituito Mg con Cu
        x_Des += " (" + strN + "-" +
                                    strP2O5 + "-" +
                                    strK2O + "-" +
                                    strCu + ")"

        x_Cod &= "/" +
                               CStr(IIf(mediaPesataN = 0, dettaglioFertilizzazione.N, Format(mediaPesataN, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataP2O5 = 0, dettaglioFertilizzazione.P, Format(mediaPesataP2O5, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataK2O = 0, dettaglioFertilizzazione.K, Format(mediaPesataK2O, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataMgO = 0, dettaglioFertilizzazione.Mg, Format(mediaPesataMgO, "0.####"))) + "-" +
                               CStr(IIf(mediaPesataCu = 0, dettaglioFertilizzazione.Cu, Format(mediaPesataCu, "0.####")))

        With dettaglioFertilizzazione
            .N = IIf(mediaPesataN = 0, dettaglioFertilizzazione.N, Format(mediaPesataN, "0.####"))
            .P = IIf(mediaPesataP2O5 = 0, dettaglioFertilizzazione.P, Format(mediaPesataP2O5, "0.####"))
            .K = IIf(mediaPesataK2O = 0, dettaglioFertilizzazione.K, Format(mediaPesataK2O, "0.####"))
            .Cu = IIf(mediaPesataCu = 0, dettaglioFertilizzazione.Cu, Format(mediaPesataCu, "0.####"))
        End With

    End Sub

    Private Function CreaDettaglioFertilizzazione(fertilizzante As AgronicaCoreMetaSchemaBIZ.Fertilizzante, TipoRichiesto As Integer, Pua_Cod As Integer,
                                                  DtEffluenti As DataTable, objParametriUscitaEff As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output) As AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione

        Dim dettaglioFertilizzazione = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione()
        dettaglioFertilizzazione.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
        dettaglioFertilizzazione.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(fertilizzante.Codice, FERTILIZZANTI)
        dettaglioFertilizzazione.prodotto.descrizione = fertilizzante.Descrizione
        dettaglioFertilizzazione.prodotto.unitaDiMisura = New UnitaDiMisura(fertilizzante.Udm_Cod)
        dettaglioFertilizzazione.effluente = New Effluente(fertilizzante.Eff_Cod)
        dettaglioFertilizzazione.N_Ponderato = False
        dettaglioFertilizzazione.P_Ponderato = False
        dettaglioFertilizzazione.K_Ponderato = False
        dettaglioFertilizzazione.Cu_Ponderato = False

        dettaglioFertilizzazione.N = fertilizzante.N
        dettaglioFertilizzazione.P = fertilizzante.P2O5
        dettaglioFertilizzazione.K = fertilizzante.K2O
        dettaglioFertilizzazione.Mg = fertilizzante.MgO
        dettaglioFertilizzazione.Cu = fertilizzante.Cu

        dettaglioFertilizzazione.tipologieFertilizzante = New List(Of metaschema.TipologiaFertilizzante)
        If TipoRichiesto < 6 Then
            For j = 0 To fertilizzante.ListaTipologie.Count - 1
                Dim tipologiaFertilizzante As New metaschema.TipologiaFertilizzante
                tipologiaFertilizzante.codice = fertilizzante.ListaTipologie(j).Codice
                tipologiaFertilizzante.descrizione = fertilizzante.ListaTipologie(j).Descrizione
                dettaglioFertilizzazione.tipologieFertilizzante.Add(tipologiaFertilizzante)
            Next
        End If

        dettaglioFertilizzazione.tipoFertilizzante = New metaschema.TipoFertilizzante()
        If TipoRichiesto >= 6 Then
            dettaglioFertilizzazione.tipoFertilizzante.codice = fertilizzante.TipoFertilizzanteCodice
            dettaglioFertilizzazione.tipoFertilizzante.descrizione = fertilizzante.TipoFertilizzanteDescrizione
        End If

        CreaEffluente(dettaglioFertilizzazione, Pua_Cod, DtEffluenti, objParametriUscitaEff)

        Return dettaglioFertilizzazione

    End Function

    Private Sub CreaEffluente(ByVal dettaglioFertilizzazione As dettagli.DettaglioFertilizzazione, ByVal Pua_Cod As Integer,
                              ByVal DtEffluenti As DataTable, ByVal objParametriUscitaEff As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output)

        'Calcolo l'N dell'effluente indicato nella PUA con la media ponderata

        '(13/06/2019 fede) in caso di direttiva nitrati ed effluenti già dichiarati prendo l'N salvato in dichiarazione
        If Pua_Cod > 0 Then

            Dim Effluente As PUA_Effluente = (From l In objParametriUscitaEff.ListaEffluenti
                                              Where l.Fer_Cod = dettaglioFertilizzazione.prodotto.codice
                                              Select l).FirstOrDefault()

            If Not IsNothing(Effluente) AndAlso Effluente.Eff_Cod > 0 Then

                Dim EffCod As Integer = Effluente.Eff_Cod

                Dim Udm_Cod_Effluente As Integer = Effluente.Udm_Cod

                Dim Udm_Sim_Effluente As String = Effluente.Udm_Sim

                dettaglioFertilizzazione.effluente.udm = New UnitaDiMisura(Udm_Cod_Effluente) With {
                                    .simbolo = Udm_Sim_Effluente
                                    }

                dettaglioFertilizzazione.effluente.descrizione = Effluente.Eff_Des

                If DtEffluenti IsNot Nothing AndAlso DtEffluenti.Rows.Count > 0 Then

                    Dim mediaPesataN = 0
                    Dim divisoreMediaPesataN As Decimal = 0

                    Dim qta_totale_effluente As Decimal = 0

                    For Each dr As DataRow In DtEffluenti.Rows

                        If dr.Item("eff_cod") = EffCod Then

                            Dim qta As Decimal = CDec(dr.Item("carico"))
                            Dim azoto_titoli As Decimal = CDec(dr.Item("azoto_titoli"))

                            If azoto_titoli > 0 Then
                                mediaPesataN += (azoto_titoli * qta)
                                divisoreMediaPesataN += qta
                            End If

                            qta_totale_effluente += qta

                        End If

                    Next

                    dettaglioFertilizzazione.effluente.carico = Format(qta_totale_effluente, "0.####")

                    If divisoreMediaPesataN <> 0 Then
                        mediaPesataN = mediaPesataN / divisoreMediaPesataN
                    End If

                    dettaglioFertilizzazione.effluente.N = Format(mediaPesataN, "0.####")

                End If
            End If
        End If
    End Sub

End Class
