Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.attivita.dettagli

Public Class STD_InsettiUtili
    Public Function LeggiInsettiUtili(tipoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita,
                                      statoAttivita As AgronicaCoreModelsSTD.attivita.Attivita.Stati,
                                      lavorazione As AgronicaCoreModelsSTD.attivita.Lavorazione,
                                      impianti As Impianto(),
                                      avversitaGruppo As metaschema.avversita.AvversitaGruppo,
                                      filtroPerDescrizione As String,
                                      validitaFine As DateTime,
                                      escludiGiacenzeZero As Boolean,
                                      codiceInsettoUtile As Integer,
                                      magazziniAgenzie As Boolean,
                                      magazziniEsterni As Boolean,
                                      objParametri_Super_Server As AgronicaCoreParametri,
                                      objParametri_Server As AgronicaCoreParametri,
                                      objParametri_Utenti As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)


        Dim insettiUtiliList As New List(Of AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)

        Dim sa_cod_list = STD_Utility.getSaCodDaImpianti(impianti)
        Dim piva = STD_Utility.getPivaDaImpianti(impianti)

        Dim chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK) = Nothing

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT:     bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.INSETTI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.INSETTI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.INSETTI, enum_Gestione_Lotti.Nessuna, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        'DT: in caso di magazzino esterno partiamo prendendo tutti i prodotti, senza considerare la giacenza e filtriamo successivamente, in base alle impostazioni di ogni singolo magazzino esterno
        If magazziniEsterni Then
            gestioneGiacenze = enum_Gestione_Giacenze.TuttiProdotti
        End If

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)


        'Se utilizzo il magazzino, leggo prima i prodotti in giacenza
        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing
        Dim strFiltroInsCod As String = ""

        'Caricamento magazzini agenzie, se presenti
        Dim filtroMagazziniEsterni = STD_Utility.getfiltroMagazziniEsterni(piva, tipoAttivita, statoAttivita, magazziniAgenzie, magazziniEsterni, chiaviMagazziniUso_da_terzi,
                                                                            objParametri_Server, objParametri_Utenti)
        If usaMagazzino Then

            Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                       piva, 0, 0,
                                                       INSETTI,
                                                       Pro_Cod:=codiceInsettoUtile,
                                                       0, 0, 0, 0, 0,
                                                       LOTTO_NONDEFINITO,
                                                       Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                       " AND InsettiUtili.Ins_Des LIKE '%" & filtroPerDescrizione & "%'" & xFiltroAggiuntivo_MagazzinoAttivoAllaData,
                                                       "",
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
                                                           INSETTI,
                                                           Pro_Cod:=codiceInsettoUtile,
                                                           0, 0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                           " AND InsettiUtili.Ins_Des LIKE '%" & filtroPerDescrizione & "%'" & xFiltroAggiuntivo_MagazzinoAttivoAllaData,
                                                           "",
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

            For i = 0 To Dt_Giacenze.Rows.Count - 1
                Dim prodToFiter = " InsettiUtili.Ins_Cod = " & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "

                If Not strFiltroInsCod.Contains(prodToFiter) Then
                    If flagQtaMaggioreZero = True Then
                        If Math.Abs(Dt_Giacenze.Rows(i).Item("Giacenza")) > QTA_GiancenzeVisualizzate Then
                            strFiltroInsCod &= prodToFiter
                        End If
                    Else
                        strFiltroInsCod &= prodToFiter
                    End If
                End If
            Next

            If strFiltroInsCod <> "" Then
                strFiltroInsCod = Left(strFiltroInsCod, strFiltroInsCod.Length - 3)
                strFiltroInsCod = " AND (" & strFiltroInsCod & ")"
            Else
                'se non ho alcun formulato in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
                If Not usaAnagrafica Then
                    Return insettiUtiliList
                End If
            End If
        End If


        Dim av_cod As Integer = 0
        Dim av_gru As Integer = 0
        Dim joinAvversita As Boolean = False
        Dim isImpollinatore As Boolean = False
        STD_Utility.GetCodiciAvversita(avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)


        Dim strFiltroAggiuntivo As String = ""
        If av_cod <> 0 AndAlso av_cod <> -1 Then
            joinAvversita = True
            strFiltroAggiuntivo = " InsettiUtilixAvversita.AV_COD = " & av_cod
        End If
        If av_gru <> 0 AndAlso av_gru <> -1 Then
            joinAvversita = True
            If strFiltroAggiuntivo <> "" Then
                strFiltroAggiuntivo += " AND "
            End If
            strFiltroAggiuntivo = " AvversitaxGruppoAvversita.AV_GRU = " & av_gru
        End If

        If av_cod = -1 AndAlso av_gru = -1 Then
            isImpollinatore = True
        End If

        If filtroPerDescrizione <> "" Then
            If strFiltroAggiuntivo <> "" Then
                strFiltroAggiuntivo += " AND "
            End If
            strFiltroAggiuntivo += " InsettiUtili.Ins_Des LIKE '%" & filtroPerDescrizione & "%'"
        End If

        If codiceInsettoUtile <> 0 Then
            If strFiltroAggiuntivo <> "" Then
                strFiltroAggiuntivo += " AND "
            End If
            strFiltroAggiuntivo &= " InsettiUtili.Ins_Cod = " & codiceInsettoUtile
        End If

        If usaMagazzino AndAlso Not usaAnagrafica Then
            If strFiltroAggiuntivo = "" And strFiltroInsCod <> "" Then
                strFiltroAggiuntivo += " 1=1 "
            End If
            strFiltroAggiuntivo &= strFiltroInsCod
        End If



        Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
        Dim DtRisultati As New DataTable



        DtRisultati = objInsetti.LeggiInsettiNG(0,
                                                joinAvversita,
                                                isImpollinatore,
                                                strFiltroAggiuntivo,
                                                "",
                                                objParametri_Server)


        Dim DtInsettiUtiliOrdinati = New DataTable
        DtInsettiUtiliOrdinati.Columns.Add(New DataColumn("InsDes", GetType(String)))
        DtInsettiUtiliOrdinati.Columns.Add(New DataColumn("DettaglioTrattamento", GetType(AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento)))
        DtInsettiUtiliOrdinati.Columns.Add(New DataColumn("ConGiacenza", GetType(Integer)))

        Dim Dr As DataRow
        Dim DrGiacenze() As DataRow = Nothing
        Dim DrGiacenze_Tot() As DataRow = Nothing


        Dim objDTU As New AgronicaCoreDataProvider.DatatableUtility

        For Each insetto In DtRisultati.Rows

            Dim Ins_Cod = insetto.Item("Ins_Cod")
            Dim Ins_Des = insetto.Item("Ins_Des")


            Dim dettaglioTrattamento = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento()
            dettaglioTrattamento.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
            dettaglioTrattamento.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto(Ins_Cod, INSETTI)
            dettaglioTrattamento.prodotto.descrizione = Ins_Des

            dettaglioTrattamento.isImpollinatore = getIsImpollinatore(Ins_Cod, objParametri_Server)

            dettaglioTrattamento.dataSmaltimentoScorte = AGRODATAINIZIO

            Dim trovataGiacenza As Boolean = False
            If usaMagazzino Then

                If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                    DrGiacenze = Dt_Giacenze.Select("pro_cod = " & Ins_Cod)
                End If

                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                    For g = 0 To DrGiacenze.Length - 1

                        If magazziniEsterni Then
                            Dim gestioneGiacenzeMagazzinoEsterno = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(DrGiacenze(g).Item("Piva"), Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FORMULATI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
                            Dim qtaMagazzinoEsterno = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                            If gestioneGiacenzeMagazzinoEsterno = enum_Gestione_Giacenze.SoloPresenti Then
                                If qtaMagazzinoEsterno <= 0 Then
                                    Continue For
                                End If
                            End If
                        End If

                        Dim dettaglioTrattamentoGiacenza = dettaglioTrattamento.Clona

                        Dim rilevamentoMagazzino As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino()
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
                            DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod = " & Ins_Cod & " and Piva = '" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod =" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
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

                        dettaglioTrattamentoGiacenza.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                        Dr = DtInsettiUtiliOrdinati.NewRow
                        Dr.Item("InsDes") = Ins_Des
                        Dr.Item("DettaglioTrattamento") = dettaglioTrattamentoGiacenza
                        Dr.Item("ConGiacenza") = 1
                        DtInsettiUtiliOrdinati.Rows.Add(Dr)

                        trovataGiacenza = True
                    Next

                End If

            End If

            'DT: il prodotto va restituito anche se presente solo in anagrafica se "usaAnagrafica=true"
            'DT: se si sta usando anche il magazzino, si restituisce il prodotto (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
            If usaAnagrafica AndAlso Not trovataGiacenza Then
                Dr = DtInsettiUtiliOrdinati.NewRow
                Dr.Item("InsDes") = Ins_Des
                Dr.Item("DettaglioTrattamento") = dettaglioTrattamento
                Dr.Item("ConGiacenza") = 0
                DtInsettiUtiliOrdinati.Rows.Add(Dr)
            End If
        Next

        If DtInsettiUtiliOrdinati IsNot Nothing Then

            Dim Dv As New DataView()
            DtInsettiUtiliOrdinati.TableName = "Prodotti"
            Dv.Table = DtInsettiUtiliOrdinati
            Dv.Sort = "ConGiacenza DESC, InsDes ASC"

            For i = 0 To Dv.Count - 1
                insettiUtiliList.Add(Dv(i).Item("DettaglioTrattamento"))
            Next

        End If

        Return insettiUtiliList

    End Function

    Private Shared Function getIsImpollinatore(Ins_Cod As Integer, objParametri_Server As AgronicaCoreParametri) As Boolean

        'Gli impollinatori NON hanno avversità associate. 
        'Se la lettura ritorna risultati per l'insetto, allora questo è un predatore

        Dim isImpollinatore As Boolean

        Dim objInsettiUtili As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
        Dim dt = objInsettiUtili.LeggInsettiUtilixPrincipiAttivi(Ins_Cod, 0, "", "", objParametri_Server)

        If dt.Rows.Count > 0 Then
            isImpollinatore = False
        Else
            isImpollinatore = True
        End If

        Return isImpollinatore
    End Function

End Class