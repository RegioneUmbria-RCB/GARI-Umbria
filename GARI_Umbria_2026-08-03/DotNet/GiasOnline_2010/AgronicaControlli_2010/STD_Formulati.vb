Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.attivita.dettagli

Public Class STD_Formulati

    Public Function LeggiFormulati(tipoAttivita As attivita.Attivita.Tipo_Attivita,
                                   statoAttivita As attivita.Attivita.Stati,
                                   lavorazione As attivita.Lavorazione,
                                   impianti As Impianto(),
                                   prodottiDaTrattare As attivita.MovimentoDiMagazzino(),
                                   specie As metaschema.utilizzi.Specie,
                                   disciplinare As metaschema.Disciplinare,
                                   epocaDPI As metaschema.Epoca,
                                   avversitaGruppo As metaschema.avversita.AvversitaGruppo,
                                   filtroPerDescrizione As String,
                                   validitaFine As DateTime,
                                   escludiGiacenzeZero As Boolean,
                                   magazziniAgenzie As Boolean,
                                   magazziniEsterni As Boolean,
                                   codiceProdotto As Integer,
                                   soloLetturaAnagrafica As Boolean,
                                   objParametri_Super_Server As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   Optional isRibaltamentoToAgenda As Boolean = False
                                   ) As List(Of attivita.dettagli.DettaglioTrattamento)

        Dim formulatiList As New List(Of attivita.dettagli.DettaglioTrattamento)


        Dim sa_cod_list As New List(Of Integer)
        Dim piva As String = ""

        Dim isProdottoDaTrattare As Boolean = False
        Select Case lavorazione.primaryKey.codice
            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA, LAVCOD_CONCIA_SEME
                sa_cod_list = STD_Utility.getSaCodDaProdottiDaTrattare(prodottiDaTrattare)
                piva = STD_Utility.getPivaDaProdottiDaTrattare(prodottiDaTrattare)

                isProdottoDaTrattare = True
            Case Else
                sa_cod_list = STD_Utility.getSaCodDaImpianti(impianti)
                piva = STD_Utility.getPivaDaImpianti(impianti)
        End Select

        Dim chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK) = Nothing

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.FORMULATI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FORMULATI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.FORMULATI, enum_Gestione_Lotti.Nessuna, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        'DT: in caso di magazzino esterno partiamo prendendo tutti i prodotti, senza considerare la giacenza e filtriamo successivamente, in base alle impostazioni di ogni singolo magazzino esterno
        If magazziniEsterni Then
            gestioneGiacenze = enum_Gestione_Giacenze.TuttiProdotti
        End If

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)

        Dim Veg_Cod As String = "0" 'Destinazione d'uso --> tutti i formulati
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            Veg_Cod = specie.codice
        End If

        'Se utilizzo il magazzino, leggo prima i prodotti in giacenza
        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing
        Dim strFiltroFrCod As String = ""

        'Caricamento magazzini agenzie, se presenti
        Dim filtroMagazziniEsterni = STD_Utility.getfiltroMagazziniEsterni(piva, tipoAttivita, statoAttivita, magazziniAgenzie, magazziniEsterni, chiaviMagazziniUso_da_terzi,
                                                                            objParametri_Server, objParametri_Utenti)

        If soloLetturaAnagrafica Then
            usaAnagrafica = True
            usaMagazzino = False
        End If

        If usaMagazzino Then

            Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dim xFiltroAggiuntivo As String = ""

            If Not String.IsNullOrEmpty(filtroPerDescrizione) Then
                xFiltroAggiuntivo = " and Formulati.Fr_Des like '%" & filtroPerDescrizione & "%'"
            End If

            xFiltroAggiuntivo &= xFiltroAggiuntivo_MagazzinoAttivoAllaData

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                       piva, 0, 0,
                                                       FORMULATI,
                                                       Pro_Cod:=codiceProdotto,
                                                       0, 0, 0, 0, 0,
                                                       LOTTO_NONDEFINITO,
                                                       Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                       xFiltroAggiuntivo,
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
                                                           FORMULATI,
                                                           Pro_Cod:=codiceProdotto,
                                                           0, 0, 0, 0, 0,
                                                           LOTTO_NONDEFINITO,
                                                           Flag_QtaNoZero:=False, 'DT: ex escludiGiacenzeZero
                                                           xFiltroAggiuntivo,
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
                Dim prodToFiter = " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                If Not strFiltroFrCod.Contains(prodToFiter) Then
                    If flagQtaMaggioreZero = True Then
                        If Math.Abs(Dt_Giacenze.Rows(i).Item("Giacenza")) > QTA_GiancenzeVisualizzate Then
                            strFiltroFrCod &= prodToFiter
                        End If
                    Else
                        strFiltroFrCod &= prodToFiter
                    End If
                End If
            Next

            If strFiltroFrCod <> "" Then
                strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 3)
                strFiltroFrCod = " AND (" & strFiltroFrCod & ")"
            Else
                'se non ho alcun formulato in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
                If Not usaAnagrafica Then
                    Return formulatiList
                End If
            End If
        End If

        Dim strTipiRichiesti As String = ""
        If lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then
            Dim STDUtility As New STD_Utility
            strTipiRichiesti = STDUtility.GetTipiFormulatiRichiesti(lavorazione.primaryKey.codice)
        End If

        Dim tipoTestata As Integer = 0
        If lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then
            tipoTestata = STD_Utility.getTipoTestata(lavorazione.primaryKey.codice)
        End If

        Dim statoCod As String = STD_Utility.getStato(piva, sa_cod_list, objParametri_Server)

        Dim epocaCod As Integer = 0
        Dim modulo As Integer = 0
        If epocaDPI IsNot Nothing AndAlso lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then
            GetEpocaModulo(epocaDPI.codice, lavorazione.primaryKey.codice, epocaCod, modulo)
        End If

        Dim Dpi_Cod As Integer = -999
        Dim Id_Rcdpi As Integer = 0
        Dim DPI_Privato_Pubblico As Integer = 0
        If disciplinare IsNot Nothing Then
            Dpi_Cod = disciplinare.codice
            If disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                Id_Rcdpi = disciplinare.raggruppamentiColturaliDPI.codice
            End If
            DPI_Privato_Pubblico = disciplinare.disciplinarePubblicoPrivato
        End If

        Dim ListaComuni As List(Of AgronicaControlli_2010.Comune) = If(isProdottoDaTrattare, New List(Of Comune), STD_Utility.getListaComuni(impianti, objParametri_Server))
        Dim strListaComuni As String = ""
        For c = 0 To ListaComuni.Count - 1
            strListaComuni &= ListaComuni(c).Prov & ListaComuni(c).Com & ","
        Next
        If strListaComuni <> "" Then
            strListaComuni = Left(strListaComuni, strListaComuni.Length - 1)
        End If

        If isRibaltamentoToAgenda Then
            tipoAttivita = attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
        End If

        Dim Lav_Cod As Integer = CInt(lavorazione.primaryKey.codice)

        'Per ora forziamo il filtro ricerca a Prodotto-->Avversita per l'installazione trappole catture massa
        Dim FiltroRicerca = "1" '1, 0 Prodotto-->Avversita ; 2 Avversità-->Prodotto

        If Lav_Cod <> LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA AndAlso
           Lav_Cod <> LAVCOD_REINNESCO_TRAPPOLE Then
            Select Case tipoAttivita
                Case attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
                    FiltroRicerca = "1"
                    Dim impostazione = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                    If impostazione = "2" Then
                        FiltroRicerca = "2"
                    End If
                Case attivita.Attivita.Tipo_Attivita.Ricetta
                    FiltroRicerca = "2"
                    Dim impostazione = objUtentiImpostazioni.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_FILTRO_PRODOTTI_RICETTE, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                    If impostazione = "1" Then
                        FiltroRicerca = "1"
                    End If
            End Select

        End If

        Dim av_cod As Integer = 0
        Dim av_gru As Integer = 0

        If FiltroRicerca = 2 AndAlso avversitaGruppo IsNot Nothing Then
            STD_Utility.GetCodiciAvversita(avversitaGruppo, tipoFormulato:=0, av_cod, av_gru)
        End If


        Dim Grfi_cod As Integer = 0
        If isProdottoDaTrattare Then
            Grfi_cod = STD_Utility.IdentificaGrfi_Cod_ProdottiDaTrattare(piva, Veg_Cod, lavorazione.primaryKey.codice, prodottiDaTrattare, objParametri_Server)
        Else
            Grfi_cod = STD_Utility.IdentificaGrfi_Cod(impianti, objParametri_Server)
        End If

        Dim Copertura As String = If(isProdottoDaTrattare, "", STD_Utility.IdentificaCopertura(impianti, objParametri_Server))

        Dim strFiltroAggiuntivo = ""
        If codiceProdotto <> 0 Then
            strFiltroAggiuntivo &= " AND Formulati.FR_COD = " & codiceProdotto
        End If
        If usaMagazzino AndAlso Not usaAnagrafica Then
            strFiltroAggiuntivo &= strFiltroFrCod
        End If


        'DT: questi filtri serviranno se si vorranno filtrare i prodotti in base al fatto che siano polverulenti o meno
        'Case Tipo_Polverulento.Polverulento
        '        strFiltroAggiuntivo &= " AND Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) In ('DP', 'DS') "
        'Case Tipo_Polverulento.NonPolverulento
        'strFiltroAggiuntivo &= " AND Upper(isNull(FormulatiXFormulazioni.For_Ni_Cod, 0)) not In ('DP', 'DS') "


        Dim strErr As String = ""
        Dim DtRisultati = AgronicaCoreWebService.Formulati_WS.Formulati_Elenco(Veg_Cod:=Veg_Cod,
                                                                           Dpi_Cod:=Dpi_Cod,
                                                                           DPI_Privato_Pubblico:=DPI_Privato_Pubblico,
                                                                           Id_Rcdpi:=Id_Rcdpi,
                                                                           Tipo_Testata:=tipoTestata,
                                                                           Opt_Avversita_Infestanti:=0,
                                                                           Opt_Singola_Gruppo:=-1,
                                                                           Av_Gru:=New Integer() {av_gru},
                                                                           Av_Cod:=New Integer() {av_cod},
                                                                           strAvversita:="",
                                                                           Modulo:=modulo,
                                                                           Ep_Cod:=epocaCod,
                                                                           TipoRichiesto:=strTipiRichiesti,
                                                                           TestoRicerca:=filtroPerDescrizione,
                                                                           Validita_Fine:=validitaFine,
                                                                           strFiltro:=strFiltroAggiuntivo,
                                                                           Grfi_cod:=Grfi_cod,
                                                                           strListaComuni:=strListaComuni,
                                                                           FormulatiXAllegatiNormative_IDRiga:=0,
                                                                           Copertura:=Copertura,
                                                                           Stato_Cod:=statoCod,
                                                                           Lingua_Cod:=objParametri_Server.Lingua_Cod,
                                                                           objParametri_Super_Server,
                                                                           objParametri_Server,
                                                                           objParametri_Utenti,
                                                                           strErr)

        If strErr <> "" OrElse IsNothing(DtRisultati) OrElse DtRisultati.Rows.Count = 0 Then
            Return formulatiList
        End If

        Dim DtFormulatiOrdinati = New DataTable
        DtFormulatiOrdinati.Columns.Add(New DataColumn("FrDes", GetType(String)))
        DtFormulatiOrdinati.Columns.Add(New DataColumn("DettaglioTrattamento", GetType(DettaglioTrattamento)))
        DtFormulatiOrdinati.Columns.Add(New DataColumn("ConGiacenza", GetType(Integer)))

        Dim Dr As DataRow
        Dim DrGiacenze() As DataRow = Nothing
        Dim DrGiacenze_Tot() As DataRow = Nothing

        Dim objModalita As New AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R

        Dim objDTU As New DatatableUtility
        Dim DrF As DataTable = objDTU.SelectDistinct_To_DT(DtRisultati, True, New String() {"Fr_Cod", "TipoRichiesto"}) 'Seleziono i Formulati Distinti

        For i = 0 To DrF.Rows.Count - 1

            Dim DtFormulato = DtRisultati.Clone

            Dim Fr_Cod = DrF.Rows(i).Item("fr_cod")
            Dim tipoRichiesto = DrF.Rows(i).Item("TipoRichiesto")

            Dim DrFrCod = DtRisultati.Select("Fr_Cod=" & Fr_Cod & " and TipoRichiesto=" & tipoRichiesto)

            If DrFrCod IsNot Nothing AndAlso DrFrCod.Length > 0 Then

                For j = 0 To DrFrCod.Length - 1
                    DtFormulato.ImportRow(DrFrCod(j))
                Next

                For f = 0 To DtFormulato.Rows.Count - 1

                    Dim Fr_Des = DtFormulato.Rows(f).Item("fr_des")
                    Dim Fr_Des_Prec = DtFormulato.Rows(f).Item("Fr_Des_Prec")
                    If Fr_Des_Prec <> "" Then
                        Fr_Des = Fr_Des_Prec
                    End If
                    If InStr(LCase(Fr_Des), "(ex") <> 0 Then
                        Fr_Des = Left(Fr_Des, InStr(LCase(Fr_Des), "(ex") - 1)
                    End If

                    Dim dateSmaltimentoScorte = AGRODATAINIZIO
                    If Not IsDBNull(DtFormulato.Rows(f).Item("DataSmaltimentoScorte")) AndAlso IsDate(DtFormulato.Rows(f).Item("DataSmaltimentoScorte")) Then
                        dateSmaltimentoScorte = DtFormulato.Rows(f).Item("DataSmaltimentoScorte")
                    End If

                    Dim Classificazioni = ""
                    If Not IsDBNull(DtFormulato.Rows(f).Item("Classificazioni")) AndAlso CStr(DtFormulato.Rows(f).Item("Classificazioni")) <> "" Then
                        Classificazioni = (CStr(DtFormulato.Rows(f).Item("Classificazioni")))
                    End If

                    Dim Polverulento = 0
                    If Not IsDBNull(DtFormulato.Rows(f).Item("Polverulento")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("Polverulento")) Then
                        Polverulento = CInt(DtFormulato.Rows(f).Item("Polverulento"))
                    End If

                    Dim DurataFeromone = 0
                    If Not IsDBNull(DtFormulato.Rows(f).Item("DurataFeromone")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("DurataFeromone")) Then
                        DurataFeromone = CInt(DtFormulato.Rows(f).Item("DurataFeromone"))
                    End If

                    Dim lstPrincipiAttivi = New List(Of metaschema.PrincipioAttivo)
                    If Not IsDBNull(DtFormulato.Rows(f).Item("strPA_COD")) AndAlso CStr(DtFormulato.Rows(f).Item("strPA_COD")) <> "" Then
                        Dim Principi = Split(CStr(DtFormulato.Rows(f).Item("strPA_COD")), "|")
                        Dim Descrizioni = Split(CStr(DtFormulato.Rows(f).Item("strPA_DES")), "|")
                        Dim Titoli = Split(CStr(DtFormulato.Rows(f).Item("strTITOLI")), "|")
                        Dim Pesi = Split(CStr(DtFormulato.Rows(f).Item("strPESI")), "|")
                        If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                            For p = 0 To Principi.Length - 1
                                Dim principioAttivo As New metaschema.PrincipioAttivo()
                                principioAttivo.codice = Principi(p)
                                principioAttivo.titolo = If(Titoli IsNot Nothing AndAlso Titoli.Length > p, Titoli(p).Replace(".", ","), 0)
                                principioAttivo.peso = If(Pesi IsNot Nothing AndAlso Pesi.Length > p, Pesi(p).Replace(".", ","), 0)
                                principioAttivo.descrizione = If(Descrizioni IsNot Nothing AndAlso Descrizioni.Length > p, Descrizioni(p), "")
                                lstPrincipiAttivi.Add(principioAttivo)
                            Next
                        End If
                    End If

                    Dim For_Veg_Cod = 0
                    Dim Carenza = 0
                    Dim BufferMin = 0
                    Dim BufferMax = 0
                    Dim Protezione = ""
                    Dim ModalitaImpiego = ""
                    Dim InRevisione = ""
                    Dim dateAttoNormativo = AGRODATAINIZIO
                    Dim FormulatiXAllegatiNormative_IDRiga = 0
                    Dim epocheBlocchi = ""

                    If (Dpi_Cod <> enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta) AndAlso Not (Dpi_Cod = enum_Disciplinare_Operazione.Biologico AndAlso Veg_Cod = "0") Then

                        If Not IsDBNull(DtFormulato.Rows(f).Item("For_Veg_Cod")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("For_Veg_Cod")) Then
                            For_Veg_Cod = DtFormulato.Rows(f).Item("For_Veg_Cod")
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("tempocarenza")) AndAlso DtFormulato.Rows(f).Item("tempocarenza") <> 0 Then
                            Carenza = CInt(DtFormulato.Rows(f).Item("tempocarenza"))
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("BufferZone_Min")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("BufferZone_Min")) Then
                            BufferMin = CDec(DtFormulato.Rows(f).Item("BufferZone_Min"))
                        End If
                        If Not IsDBNull(DtFormulato.Rows(f).Item("BufferZone_Max")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("BufferZone_Max")) Then
                            BufferMax = CDec(DtFormulato.Rows(f).Item("BufferZone_Max"))
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("Flag_Protetto")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("Flag_Protetto")) Then
                            Select Case CInt(DtFormulato.Rows(f).Item("Flag_Protetto"))
                                Case 1
                                    Protezione = My.Resources.AgronicaControlli_2010.Serra
                                Case 2
                                    Protezione = My.Resources.AgronicaControlli_2010.PienoCampo
                            End Select
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("Mdi_Cod")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("Mdi_Cod")) Then
                            Dim Mdi_Cod = CDec(DtFormulato.Rows(f).Item("Mdi_Cod"))
                            If Mdi_Cod <> 0 Then
                                ModalitaImpiego = objModalita.MdiDes_from_MdiCod(Mdi_Cod, objParametri_Server)
                            End If
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("Verificato_Flag")) AndAlso DtFormulato.Rows(f).Item("Verificato_Flag") = 15 Then
                            InRevisione = "1"
                            If Not IsDBNull(DtFormulato.Rows(f).Item("DataAttoNormativo")) AndAlso IsDate(DtFormulato.Rows(f).Item("DataAttoNormativo")) Then
                                dateAttoNormativo = DtFormulato.Rows(f).Item("DataAttoNormativo")
                            End If
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga")) Then
                            FormulatiXAllegatiNormative_IDRiga = CInt(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga"))
                        End If

                        If Not IsDBNull(DtFormulato.Rows(f).Item("EpocheBlocco")) Then
                            epocheBlocchi = DtFormulato.Rows(f).Item("EpocheBlocco")
                        End If
                    End If

                    Dim dettaglioTrattamento = New DettaglioTrattamento()
                    dettaglioTrattamento.MagazziniMovimentazioni = New List(Of attivita.RilevamentoDiMagazzino)
                    dettaglioTrattamento.principiAttivi = lstPrincipiAttivi
                    dettaglioTrattamento.prodotto = New attivita.risorse.Prodotto(Fr_Cod, FORMULATI)
                    dettaglioTrattamento.prodotto.descrizione = Fr_Des
                    dettaglioTrattamento.descrizionePrecedente = Fr_Des_Prec
                    dettaglioTrattamento.dataSmaltimentoScorte = dateSmaltimentoScorte
                    dettaglioTrattamento.classificazioni = Classificazioni
                    dettaglioTrattamento.polverulento = Polverulento
                    dettaglioTrattamento.tipoFormulato = tipoRichiesto
                    dettaglioTrattamento.inRevisione = InRevisione
                    dettaglioTrattamento.dataAttoNormativo = dateAttoNormativo
                    dettaglioTrattamento.tempoCarenza = Carenza
                    dettaglioTrattamento.formulatiXAllegatiNormative_IDRiga = FormulatiXAllegatiNormative_IDRiga
                    dettaglioTrattamento.dettaglioProdotto = For_Veg_Cod
                    dettaglioTrattamento.bufferzone = New metaschema.BufferZone()
                    dettaglioTrattamento.bufferzone.minimo = BufferMin
                    dettaglioTrattamento.bufferzone.massimo = BufferMax
                    dettaglioTrattamento.epocheBlocchi = epocheBlocchi
                    dettaglioTrattamento.protezione = Protezione
                    dettaglioTrattamento.modalitaImpiego = ModalitaImpiego

                    dettaglioTrattamento.durataFeromone = DurataFeromone
                    dettaglioTrattamento.scadenzaFeromone = validitaFine.AddDays(DurataFeromone)

                    Dim trovataGiacenza As Boolean = False
                    If usaMagazzino Then

                        If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                            DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
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

                                Dim rilevamentoMagazzino As New attivita.RilevamentoDiMagazzino()
                                rilevamentoMagazzino.Qta = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                                rilevamentoMagazzino.udm = New metaschema.UnitaDiMisura()
                                rilevamentoMagazzino.udm.codice = DrGiacenze(g).Item("Udm_Cod")
                                rilevamentoMagazzino.udm.simbolo = DrGiacenze(g).Item("Udm_Sim")
                                rilevamentoMagazzino.Lotto = DrGiacenze(g).Item("lotto")
                                rilevamentoMagazzino.Magazzino = New Fabbricato()
                                rilevamentoMagazzino.Magazzino.primaryKey = New Fabbricato.PK()
                                rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK = New CentroAziendale.PK()
                                rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva = DrGiacenze(g).Item("Piva")
                                rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice = DrGiacenze(g).Item("Sa_Cod")
                                rilevamentoMagazzino.Magazzino.primaryKey.codice = DrGiacenze(g).Item("Id_Destinazione")
                                rilevamentoMagazzino.Magazzino.descrizione = CStr(DrGiacenze(g).Item("Fabbricato_Des")) & " (" & CStr(DrGiacenze(g).Item("Sa_Nome")) & ")"

                                If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                    DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod & " and Piva='" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod=" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
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

                                Dr = DtFormulatiOrdinati.NewRow
                                Dr.Item("FrDes") = Fr_Des
                                Dr.Item("DettaglioTrattamento") = dettaglioTrattamentoGiacenza
                                Dr.Item("ConGiacenza") = 1
                                DtFormulatiOrdinati.Rows.Add(Dr)

                                trovataGiacenza = True
                            Next

                        End If

                    End If

                    'DT: il prodotto va restituito anche se presente solo in anagrafica se "usaAnagrafica=true"
                    'DT: se si sta usando anche il magazzino, si restituisce il prodotto (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
                    If usaAnagrafica AndAlso Not trovataGiacenza Then
                        Dr = DtFormulatiOrdinati.NewRow
                        Dr.Item("FrDes") = Fr_Des
                        Dr.Item("DettaglioTrattamento") = dettaglioTrattamento
                        Dr.Item("ConGiacenza") = 0
                        DtFormulatiOrdinati.Rows.Add(Dr)
                    End If
                Next

            End If

        Next

        If DtFormulatiOrdinati IsNot Nothing Then

            Dim Dv As New DataView()
            DtFormulatiOrdinati.TableName = "Prodotti"
            Dv.Table = DtFormulatiOrdinati
            Dv.Sort = "ConGiacenza DESC, FrDes ASC"

            For i = 0 To Dv.Count - 1
                formulatiList.Add(Dv(i).Item("DettaglioTrattamento"))
            Next

        End If

        Return formulatiList

    End Function

    Private Sub GetEpocaModulo(epocaDPI As String, Lav_Cod As String, ByRef epocaCod As Integer, ByRef modulo As Integer)

        If IsNumeric(epocaDPI) Then
            Select Case Lav_Cod
                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    modulo = epocaDPI
                    epocaCod = 0
                Case LAVCOD_DISERBO
                    epocaCod = epocaDPI
                    modulo = 0
                Case LAVCOD_DISSECCAMENTO
                    epocaCod = 0
                    modulo = 0
            End Select
        End If

    End Sub

End Class
