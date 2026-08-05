Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class STD_Avversita
    Const NessunDpiNessunaEtichetta As String = "-999"
    Const Bio As String = "-2"

    Public Function LeggiAvversita(tipo As attivita.Attivita.Tipo_Attivita,
                                  lavorazione As attivita.Lavorazione,
                                   impianti As Impianto(),
                                   specie As metaschema.utilizzi.Specie,
                                   disciplinare As metaschema.Disciplinare,
                                   epocaDPI As metaschema.Epoca,
                                   dettaglioTrattamento As attivita.dettagli.DettaglioTrattamento,
                                   validitaFine As DateTime,
                                   objParametri_Super_Server As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   Optional isRibaltamentoToAgenda As Boolean = False) As List(Of metaschema.avversita.AvversitaGruppo)

        Dim avversitaList As New List(Of metaschema.avversita.AvversitaGruppo)

        Dim Lav_Cod As Integer = CInt(lavorazione.primaryKey.codice)

        Dim strErr As String = ""
        Dim DtRisultati As New DataTable

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        If isRibaltamentoToAgenda Then
            tipo = attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
        End If

        'Per ora forziamo il filtro ricerca a Prodotto-->Avversita per l'installaizone trappole catture massa
        Dim FiltroRicerca = "1" '1, 0 Prodotto-->Avversita ; 2 Avversità-->Prodotto

        If Lav_Cod <> LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA AndAlso
            Lav_Cod <> LAVCOD_REINNESCO_TRAPPOLE Then
            Select Case tipo
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


        Dim Veg_Cod As String = "0" 'Destinazione d'uso
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            Veg_Cod = specie.codice
        End If

        Dim FrCod As Integer = 0
        Dim FormulatiXAllegatiNormative_IDRiga = 0
        If FiltroRicerca = 1 AndAlso dettaglioTrattamento IsNot Nothing AndAlso dettaglioTrattamento.prodotto IsNot Nothing Then
            FrCod = dettaglioTrattamento.prodotto.codice
            FormulatiXAllegatiNormative_IDRiga = dettaglioTrattamento.formulatiXAllegatiNormative_IDRiga
        End If

        'nel caso filtro prod --> avv se non ho scelto un formulato non carico nulla
        If FiltroRicerca = "1" AndAlso FrCod = 0 Then
            Return avversitaList
        End If

        'controllo se devo caricare i fitofarmaci o i diserbanti
        Dim tipoTestata As Integer = 0
        If lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then
            tipoTestata = STD_Utility.getTipoTestata(lavorazione.primaryKey.codice)
        End If

        Select Case tipoTestata

            Case 0
                If Veg_Cod = "0" Then
                    'per i terreni nudi, permetto l'operazione con nessuno nessuno e bio
                    If disciplinare IsNot Nothing AndAlso Not (disciplinare.codice = NessunDpiNessunaEtichetta OrElse disciplinare.codice = Bio) Then
                        Return avversitaList
                    End If
                End If

            Case 1
                If lavorazione.primaryKey.codice = LAVCOD_DISSECCAMENTO Then
                    Return avversitaList
                End If

            Case 2
                If lavorazione.primaryKey.codice = LAVCOD_TRATTAMENTO_FITOREGOLATORE Then
                    Return avversitaList
                End If
        End Select

        If disciplinare IsNot Nothing Then
            'nel caso di nessun dpi nessuna etichetta carico le avv/gruppi filtrati solo sulla specie e la data
            If disciplinare.codice = NessunDpiNessunaEtichetta Then
                FrCod = 0
            End If
        End If


        Dim Dpi_Cod As Integer = 0
        Dim Id_Rcdpi As Integer = 0
        Dim DPI_Privato_Pubblico As Integer = 0

        If disciplinare IsNot Nothing Then
            Dpi_Cod = STD_Utility.getDpiCod(disciplinare)
            If disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                Id_Rcdpi = disciplinare.raggruppamentiColturaliDPI.codice
            End If
            DPI_Privato_Pubblico = disciplinare.disciplinarePubblicoPrivato
        End If

        Dim Modulo = 0
        Dim Epoca = 0

        If epocaDPI IsNot Nothing Then
            If tipoTestata = 0 Then
                Modulo = epocaDPI.codice
            Else
                Epoca = epocaDPI.codice
            End If
        End If

        Dim Storico As Boolean = True
        If FiltroRicerca = "2" Then
            Storico = False
        End If

        Dim ListaComuni As List(Of AgronicaControlli_2010.Comune) = STD_Utility.getListaComuni(impianti, objParametri_Server)
        Dim strListaComuni As String = ""
        For c = 0 To ListaComuni.Count - 1
            strListaComuni &= ListaComuni(c).Prov & ListaComuni(c).Com & ","
        Next
        If strListaComuni <> "" Then
            strListaComuni = Left(strListaComuni, strListaComuni.Length - 1)
        End If

        Dim strTipiRichiesti As String = ""
        If lavorazione IsNot Nothing AndAlso lavorazione.primaryKey IsNot Nothing Then
            Dim STDUtility As New STD_Utility
            strTipiRichiesti = STDUtility.GetTipiFormulatiRichiesti(lavorazione.primaryKey.codice)
        End If

        'Richiamo caricamento avversita da banche dati
        DtRisultati = AgronicaCoreWebService.Avversita_WS.Avversita_Elenco(Fr_Cod:=FrCod,
                                                                        Veg_Cod:=Veg_Cod,
                                                                        Dpi_Cod:=Dpi_Cod,
                                                                        DPI_Privato_Pubblico:=DPI_Privato_Pubblico,
                                                                        Id_Rcdpi:=Id_Rcdpi,
                                                                        Tipo_Testata:=tipoTestata,
                                                                        Modulo:=Modulo,
                                                                        Ep_Cod:=Epoca,
                                                                        Validita_Fine:=validitaFine,
                                                                        strListaComuni:=strListaComuni,
                                                                        Storico:=Storico,
                                                                        FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                        Lingua_Cod:=objParametri_Server.Lingua_Cod,
                                                                        TipiFormulato:=strTipiRichiesti,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        strErr)



        If DtRisultati IsNot Nothing AndAlso DtRisultati.Rows.Count > 0 Then
            'uso il dataview per ordinare
            Dim DvAvv As New DataView
            DvAvv.Table = DtRisultati
            DvAvv.Sort = "Av_Des ASC"

            Dim dt As DataTable = DvAvv.ToTable
            For i = 0 To dt.Rows.Count - 1
                Dim av_cod = dt.Rows(i).Item("Av_Cod")
                Dim av_gru = dt.Rows(i).Item("Av_Gru")
                Dim av_des = dt.Rows(i).Item("Av_Des")
                Dim For_Veg_Av_Cod = dt.Rows(i).Item("For_Veg_Av_Cod")
                Dim dataSmaltimentoScorte As DateTime = AGRODATAFINE
                If Not IsDBNull(dt.Rows(i).Item("datasmaltimentoscorte")) AndAlso IsDate(dt.Rows(i).Item("datasmaltimentoscorte")) Then
                    dataSmaltimentoScorte = CDate(dt.Rows(i).Item("datasmaltimentoscorte")).ToShortDateString
                End If

                If av_cod <> 0 Then
                    Dim avversita = New AgronicaCoreModelsSTD.metaschema.avversita.Avversita(av_cod)
                    'DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                    If av_cod < 0 Then
                        avversita.gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_cod)
                    Else
                        avversita.gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(0)
                    End If
                    avversita.descrizione = av_des
                    avversita.For_Veg_Av_Cod = For_Veg_Av_Cod
                    avversita.formulatiXAllegatiNormative_IDRiga = FormulatiXAllegatiNormative_IDRiga
                    avversita.dataSmaltimentoScorte = dataSmaltimentoScorte
                    avversitaList.Add(avversita)
                ElseIf av_gru <> 0 Then
                    Dim gruppoAvversita = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_gru)
                    gruppoAvversita.descrizione = av_des
                    gruppoAvversita.For_Veg_Av_Cod = For_Veg_Av_Cod
                    gruppoAvversita.formulatiXAllegatiNormative_IDRiga = FormulatiXAllegatiNormative_IDRiga
                    gruppoAvversita.dataSmaltimentoScorte = dataSmaltimentoScorte
                    avversitaList.Add(gruppoAvversita)
                End If

            Next
        End If

        Return avversitaList
    End Function

    Public Function LeggiSoglieAvversita(disciplinare As metaschema.Disciplinare,
                                   avversitaGruppo As metaschema.avversita.AvversitaGruppo,
                                   objParametri_Super_Server As AgronicaCoreParametri,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri) As List(Of metaschema.Soglia)

        Dim soglieAvversitaList As New List(Of metaschema.Soglia)

        Dim strErr As String = ""
        Dim DtRisultati As New DataTable

        'DT: solo avversità singole per le soglie
        If avversitaGruppo Is Nothing OrElse avversitaGruppo.codice = 0 OrElse avversitaGruppo.classType = costanti.ClassType.GruppoAvversita Then
            Return soglieAvversitaList
        End If
        Dim Av_Cod As Integer = avversitaGruppo.codice
        Dim strAvversita As String = " Av_Cod=" & Av_Cod

        Dim Dpi_Cod As Integer = 0
        Dim Id_Rcdpi As Integer = 0
        Dim DPI_Privato_Pubblico As Integer = 0
        If disciplinare IsNot Nothing Then
            Dpi_Cod = STD_Utility.getDpiCod(disciplinare)
            If disciplinare.raggruppamentiColturaliDPI IsNot Nothing Then
                Id_Rcdpi = disciplinare.raggruppamentiColturaliDPI.codice
            End If
            DPI_Privato_Pubblico = disciplinare.disciplinarePubblicoPrivato
        End If

        DtRisultati = AgronicaCoreWebService.Avversita_WS.Soglie_Avversita_Elenco(Dpi_Cod:=Dpi_Cod,
                                                                           Id_Rcdpi:=Id_Rcdpi,
                                                                           Av_Cod:=Av_Cod,
                                                                           objParametri_Super_Server,
                                                                           objParametri_Server,
                                                                           objParametri_Utenti,
                                                                           strErr)

        If strErr = "" Then

            Dim objDesUM As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

            If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

                'Recupero la lista degli oggetti lavorazione da associare alle soglie

                Dim listOperazioni As List(Of AgronicaCoreModelsSTD.attivita.Lavorazione) = Nothing


                Dim List_Lav_Cod As List(Of Integer) = (From row In DtRisultati
                                                        Select row.Field(Of Integer)("Lav_Cod")
                                                        Distinct).ToList()

                If Not IsNothing(List_Lav_Cod) AndAlso List_Lav_Cod.Count > 0 Then

                    Dim StringaLavCod As String = String.Join(",", List_Lav_Cod)

                    Dim xFiltroAggiuntivo As String = " Operazioni.Lav_Cod IN (" & StringaLavCod & ") "

                    Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                    Dim DTOperazioni As DataTable = objOperazioniLeggi.Leggi(0, 0, 0, "",
                                                                             0, "", "",
                                                                             False, False,
                                                                             False, False,
                                                                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                             xFiltroAggiuntivo, "",
                                                                             objParametri_Server)

                    listOperazioni = (From row As DataRow In DTOperazioni.Rows
                                      Select New AgronicaCoreModelsSTD.attivita.Lavorazione(row("LAV_COD"), row("LAV_DES"))).ToList
                End If

                If Not IsNothing(listOperazioni) Then
                    For i = 0 To DtRisultati.Rows.Count - 1

                        Dim soglia = New metaschema.Soglia With {
                            .codice = DtRisultati.Rows(i).Item("Soglia_Cod"),
                            .descrizione = If(DtRisultati(i).Item("Quantita") <> "", "> " & DtRisultati.Rows(i).Item("Quantita") & " " & DtRisultati.Rows(i).Item("Soglia"), DtRisultati.Rows(i).Item("Soglia")),
                            .quantita = If(DtRisultati(i).Item("Quantita") <> "", DtRisultati.Rows(i).Item("Quantita"), 0),
                            .avversita = New metaschema.avversita.Avversita(DtRisultati.Rows(i).Item("Av_Cod")),
                            .udm = New metaschema.UnitaDiMisura(DtRisultati.Rows(i).Item("Udm_Cod")),
                            .lavorazione = listOperazioni.Find(Function(lav) lav.primaryKey.codice = DtRisultati.Rows(i).Item("Lav_Cod"))
                        }
                        soglieAvversitaList.Add(soglia)
                    Next
                End If

            End If
        End If

        Return soglieAvversitaList
    End Function

    Public Shared Function SoglieIntervento(ByVal Dpi_Cod As Integer,
                                            ByVal Si_Cod As Integer,
                                            ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
        Dim Dati As String

        Dim XmlDocumento As New System.Xml.XmlDocument
        Dim XmlNodo As System.Xml.XmlNodeList
        Dim XmlElemento As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt.Columns.Add(New DataColumn("Si_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))

        Try

            Dim agroWs As String
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
            If agroWs = "" Then
                agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)
            End If

            ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

            Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)
            Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
            Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
            Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, AgroKey_EncoderDecoder)

            Dati = ObjDownloadWs.Leggi_SogliaIntervento(Dpi_Cod,
                                                        Si_Cod,
                                                        ASG_Utente_Username_Crypt,
                                                        ASG_Utente_Password_Crypt)

            If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                XmlDocumento.LoadXml(Dati)
                XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                If Not XmlNodo Is Nothing Then
                    For Each XmlElemento In XmlNodo
                        Dr = Dt.NewRow
                        Dr.Item("Si_Cod") = XmlElemento.GetAttribute("si_cod")
                        Dr.Item("Lav_Cod") = XmlElemento.GetAttribute("lav_cod")
                        Dt.Rows.Add(Dr)
                    Next
                End If
            End If

            If Not Dt Is Nothing Then
                Return Dt
            End If

        Catch ex As Exception

            Dati = "Si sono verificati errori in fase di chiamata al WebService Discliplinari!" & Chr(13) & ex.Message
            Throw New Exception(Dati)

        End Try

        Return Dt
    End Function

    Public Function ControllaSogliaSoddisfatta(soglia As metaschema.Soglia,
                                               Impianti As anagrafiche.Impianto(),
                                               appezzamenti As anagrafiche.Appezzamento(),
                                               Data As DateTime,
                                               Specie As metaschema.utilizzi.Specie,
                                               objParametri_Server As AgronicaCoreParametri) As String

        Dim Soglia_LavCod As Integer = 0

        If Not IsNothing(soglia.lavorazione) AndAlso Not IsNothing(soglia.lavorazione.primaryKey) Then
            Soglia_LavCod = soglia.lavorazione.primaryKey.codice
        End If

        Dim Soglia_UdmCod As Integer = 0

        If Not IsNothing(soglia.udm) Then
            Soglia_UdmCod = soglia.udm.codice
        End If

        Dim Soglia_AvCod As Integer = 0

        If Not IsNothing(soglia.avversita) Then
            Soglia_AvCod = soglia.avversita.codice
        End If

        Dim Soglia_Qta As Integer = soglia.quantita

        Dim Veg_Cod As Integer = Specie.codice

        Dim strVerificaSoglia As String = ""
        Dim nImpiantiViolati As Integer = 0
        Dim strErr_SogliaNonSoddisfatta As String = ""
        Dim SogliaImpOk As Boolean

        If Not IsNothing(Impianti) AndAlso Impianti.Length <> 0 Then

            For Each Impianto As anagrafiche.Impianto In Impianti

                Dim Piva As String = Impianto.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva

                Dim Sa_Cod As Integer = Impianto.primaryKey.appezzamentoPK.centroAziendalePK.codice

                Dim Appezza As Integer = Impianto.primaryKey.appezzamentoPK.codice

                'Ottengo la descrizione dell'appezzamento
                Dim App_Nome As String = appezzamenti.ToList().Find(Function(app) app.primaryKey.codice = Appezza).descrizione

                Dim ID_Reg As Integer = Impianto.primaryKey.codice

                Dim Validita_Inizio_Distinta As Date = Impianto.esercizi(0).validita.inizio

                Select Case Soglia_LavCod

                    Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                        SogliaImpOk = False

                        Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                        Dim dtRilievi As DataTable

                        'Lettura Rilievi Precedenti
                        dtRilievi = ObjRilievi.Leggi_RilieviAvversita(Piva,
                                                                     Sa_Cod,
                                                                     Appezza,
                                                                     ID_Reg,
                                                                    Soglia_UdmCod,
                                                                    Soglia_AvCod,
                                                                    Validita_Inizio_Distinta,
                                                                    Data,
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                    objParametri_Server)

                        If dtRilievi.Rows.Count > 0 Then

                            'Gestione Presenza
                            If CDbl(Soglia_Qta) = 0 Then
                                Soglia_Qta = 0.5 'Presenza
                            End If

                            If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Soglia_Qta) Then
                                nImpiantiViolati += 1
                                strErr_SogliaNonSoddisfatta &= " App." & App_Nome & ", "
                            Else
                                SogliaImpOk = True
                                'Soglia Soddisfatta --> Impianto Ok
                            End If

                        Else
                            nImpiantiViolati += 1
                            strErr_SogliaNonSoddisfatta &= " App." & App_Nome & ", "
                        End If

                        If SogliaImpOk = False Then

                            'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                            dtRilievi = ObjRilievi.Leggi_RilieviTrappole(Piva,
                                                                         Sa_Cod,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        Soglia_AvCod,
                                                                        Veg_Cod,
                                                                        Validita_Inizio_Distinta,
                                                                        Data,
                                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "",
                                                                        objParametri_Server)

                            Dim Qta_Rilievo As Integer = 0
                            Dim Qta_Media_Rilievo As Integer = 0

                            If dtRilievi.Rows.Count > 0 Then

                                Dim iRil As Integer
                                For iRil = 0 To dtRilievi.Rows.Count - 1
                                    Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                                Next iRil

                                Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                                If Qta_Media_Rilievo >= Soglia_Qta Then
                                    'Soglia Soddisfatta --> Impianto Ok
                                    SogliaImpOk = True
                                    nImpiantiViolati -= 1
                                    strErr_SogliaNonSoddisfatta = Replace(strErr_SogliaNonSoddisfatta, " App." & App_Nome & ", ", "")
                                End If
                            End If

                        End If


                    Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                        SogliaImpOk = False

                        Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                        Dim dtRilievi As DataTable

                        'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                        dtRilievi = ObjRilievi.Leggi_RilieviTrappole(Piva,
                                                                     Sa_Cod,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    Soglia_AvCod,
                                                                    Veg_Cod,
                                                                    Validita_Inizio_Distinta,
                                                                    Data,
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri_Server)

                        Dim Qta_Rilievo As Integer = 0
                        Dim Qta_Media_Rilievo As Integer = 0

                        If dtRilievi.Rows.Count > 0 Then

                            Dim iRil As Integer
                            For iRil = 0 To dtRilievi.Rows.Count - 1
                                Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                            Next iRil

                            Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                            If Not (Qta_Media_Rilievo >= Soglia_Qta) Then
                                nImpiantiViolati += 1
                                strErr_SogliaNonSoddisfatta &= " App." & App_Nome & ", "
                            Else
                                'Soglia Soddisfatta --> Impianto Ok
                                SogliaImpOk = True
                            End If
                        Else
                            nImpiantiViolati += 1
                            strErr_SogliaNonSoddisfatta &= " App." & App_Nome & ", "
                        End If

                        '(27/02/2019 fede) aggiunto controllo se è stato registrato il rilievo avv in campo della soglia necessaria
                        If SogliaImpOk = False Then

                            dtRilievi = ObjRilievi.Leggi_RilieviAvversita(Piva,
                                                                          Sa_Cod,
                                                                          Appezza,
                                                                          ID_Reg,
                                                                        Soglia_UdmCod,
                                                                        Soglia_AvCod,
                                                                        Validita_Inizio_Distinta,
                                                                        Data,
                                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "",
                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                        objParametri_Server)

                            If dtRilievi.Rows.Count > 0 Then

                                'Gestione Presenza
                                If CDbl(Soglia_Qta) = 0 Then
                                    Soglia_Qta = 0.5 'Presenza
                                End If

                                If CDbl(dtRilievi.Rows(0).Item("Qta")) >= Soglia_Qta Then
                                    SogliaImpOk = True
                                    'Soglia Soddisfatta --> Impianto Ok
                                    nImpiantiViolati -= 1
                                    strErr_SogliaNonSoddisfatta = Replace(strErr_SogliaNonSoddisfatta, " App." & App_Nome & ", ", "")
                                End If

                            End If

                        End If

                    Case LAVCOD_CATTURE_MASSA, LAVCOD_DISORIENTAMENTO_SESSUALE

                        Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                        Dim dtRilievi As DataTable

                        dtRilievi = ObjRilievi.Leggi_ConfusioneDisorientamentoSessuale(Piva,
                                                                                        Sa_Cod,
                                                                                        Appezza,
                                                                                        ID_Reg,
                                                                                        Soglia_LavCod,
                                                                                        Soglia_AvCod,
                                                                                        Validita_Inizio_Distinta,
                                                                                        Data,
                                                                                        "",
                                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                        objParametri_Server)

                        If dtRilievi.Rows.Count > 0 Then
                            'Soglia Soddisfatta --> Impianto Ok
                        Else
                            nImpiantiViolati += 1
                            strErr_SogliaNonSoddisfatta &= " App." & App_Nome & ", "
                        End If

                End Select

            Next

            If strErr_SogliaNonSoddisfatta <> "" Then
                strErr_SogliaNonSoddisfatta = strErr_SogliaNonSoddisfatta.Replace("<b>", "").Replace("</b>", "")
                strVerificaSoglia &= Left(strErr_SogliaNonSoddisfatta, strErr_SogliaNonSoddisfatta.Length - 2)
            End If

        End If

        Return strVerificaSoglia

    End Function

    Public Function LeggiAvversitaInsettiUtili(tipo As attivita.Attivita.Tipo_Attivita,
                                               lavorazione As attivita.Lavorazione,
                                               impianti As Impianto(),
                                               specie As metaschema.utilizzi.Specie,
                                               dettaglioTrattamento As attivita.dettagli.DettaglioTrattamento,
                                               validitaFine As DateTime,
                                               objParametri_Super_Server As AgronicaCoreParametri,
                                               objParametri_Server As AgronicaCoreParametri,
                                               objParametri_Utenti As AgronicaCoreParametri,
                                               Optional isRibaltamentoToAgenda As Boolean = False
                                               ) As List(Of metaschema.avversita.AvversitaGruppo)

        Dim avversitaList As New List(Of metaschema.avversita.AvversitaGruppo)

        Dim strErr As String = ""
        Dim DtRisultati As New DataTable

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        If isRibaltamentoToAgenda Then
            tipo = attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
        End If

        Dim FiltroRicerca = "1" '1, 0 Prodotto-->Avversita ; 2 Avversità-->Prodotto
        Select Case tipo
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

        Dim Veg_Cod As String = "0" 'Destinazione d'uso
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            Veg_Cod = specie.codice
        End If

        Dim Ins_Cod As Integer = 0
        If FiltroRicerca = "1" AndAlso dettaglioTrattamento IsNot Nothing AndAlso dettaglioTrattamento.prodotto IsNot Nothing Then
            Ins_Cod = dettaglioTrattamento.prodotto.codice
        End If

        'nel caso filtro prod --> avv se non ho scelto un formulato non carico nulla
        If Ins_Cod = 0 And FiltroRicerca = "1" Then
            Return avversitaList
        End If

        If FiltroRicerca = "2" Then
            'AVVERSITA' X IMPOLLINATORE (NESSUNA)
            avversitaList.Add(New metaschema.avversita.Avversita With {
                          .codice = -1,
                          .gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(-1),
                          .descrizione = Gias.DistribuzionePerImpollinazione.ToUpper(),
                          .dataSmaltimentoScorte = AGRODATAFINE
                          })
        End If

        '-----------------------
        'AVVERSITA' SINGOLE
        '-----------------------
        Dim filtroAvv As String = ""
        'Filtro infestanti/gruppi infestanti
        filtroAvv &= " NOT EXISTS (SELECT * FROM InfestantiAttive " &
                       " WHERE InfestantiAttive.Av_Cod = Avversita.Av_Cod ) "

        'Elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        filtroAvv &= " AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' " &
                     " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

        If Ins_Cod <> 0 Then

            Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
            DtRisultati = objInsetti.LeggiXAvversitaXSpecie(Ins_Cod, 0, Veg_Cod, 0,
                                                            AGRODATAINIZIO, AGRODATAFINE,
                                                            filtroAvv, "",
                                                            objParametri_Server)

        Else

            Dim objAvv As New AgronicaCoreMetaSchemaDAL.SpecieVegetalixAvversita_R
            DtRisultati = objAvv.Leggi(0, Veg_Cod, 0,
                                       AGRODATAINIZIO, AGRODATAFINE,
                                       AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       filtroAvv, "",
                                       objParametri_Server)

        End If

        If DtRisultati IsNot Nothing AndAlso DtRisultati.Rows.Count > 0 Then
            DtRisultati.TableName = "Av_Des"

            'uso il dataview per ordinare
            Dim DvAvv As New DataView
            DvAvv.Table = DtRisultati
            DvAvv.Sort = "Av_Des_Vol ASC"

            Dim dt As DataTable = DvAvv.ToTable
            For i = 0 To dt.Rows.Count - 1
                Dim av_cod = dt.Rows(i).Item("Av_Cod")
                Dim av_des = dt.Rows(i).Item("Av_Des_Vol") & " (<i>" & dt.Rows(i).Item("Av_Des_Lat") & "</i>)"

                If av_cod <> 0 Then
                    Dim avversita = New AgronicaCoreModelsSTD.metaschema.avversita.Avversita(av_cod)
                    'DT: se avversità singola con codice negativo, valorizzare con lo stesso valore il gruppo
                    If av_cod < 0 Then
                        avversita.gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_cod)
                    Else
                        avversita.gruppo = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(0)
                    End If
                    avversita.descrizione = av_des
                    avversita.dataSmaltimentoScorte = AGRODATAFINE

                    avversitaList.Add(avversita)
                End If
            Next
        End If


        '-----------------------
        'AVVERSITA' GRUPPI
        '-----------------------

        Dim filtroAvvGru As String = ""
        'Filtro infestanti/gruppi infestanti
        filtroAvvGru &= " NOT EXISTS (SELECT * FROM GruppoAvversitaAttive " &
                        " WHERE GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ) "

        'Elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        filtroAvvGru &= " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " &
                        " AND GruppoAvversita.Av_Gru_Des_Lat NOT LIKE '%non usare%' " &
                        " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "

        If Ins_Cod <> 0 Then

            Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
            DtRisultati = objInsetti.LeggiXGruAvversitaXSpecie(Ins_Cod, 0, Veg_Cod, 0,
                                                               AGRODATAINIZIO, AGRODATAFINE,
                                                               filtroAvvGru, "",
                                                               objParametri_Server)

        Else

            Dim objAvvGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
            DtRisultati = objAvvGru.Leggi(0, 1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                          filtroAvvGru, "",
                                          objParametri_Server)

        End If

        If DtRisultati IsNot Nothing AndAlso DtRisultati.Rows.Count > 0 Then
            DtRisultati.TableName = "Av_Des"

            'uso il dataview per ordinare
            Dim DvAvv As New DataView
            DvAvv.Table = DtRisultati
            DvAvv.Sort = "Av_Gru_Des ASC"

            Dim dt As DataTable = DvAvv.ToTable
            For i = 0 To dt.Rows.Count - 1
                Dim av_gru = dt.Rows(i).Item("Av_Gru")
                Dim av_des = dt.Rows(i).Item("Av_Gru_Des") & " (<i>" & dt.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"

                If av_gru <> 0 Then
                    Dim gruppoAvversita = New AgronicaCoreModelsSTD.metaschema.avversita.GruppoAvversita(av_gru)
                    gruppoAvversita.descrizione = av_des
                    gruppoAvversita.dataSmaltimentoScorte = AGRODATAFINE
                    avversitaList.Add(gruppoAvversita)
                End If
            Next
        End If

        Return avversitaList

    End Function

    Public Function LeggiAvversitaInnesco(tipoAttivita As attivita.Attivita.Tipo_Attivita,
                                           statoAttivita As attivita.Attivita.Stati,
                                           lavorazione As attivita.Lavorazione,
                                           dettaglioTrattamento As attivita.dettagli.DettaglioTrattamento,
                                           impianti As Impianto(),
                                           specie As metaschema.utilizzi.Specie,
                                           disciplinare As metaschema.Disciplinare,
                                           epocaDPI As metaschema.Epoca,
                                           validitaFine As DateTime,
                                           escludiGiacenzeZero As Boolean,
                                           magazziniAgenzie As Boolean,
                                           magazziniEsterni As Boolean,
                                           codiceAvversita As Integer,
                                           soloLetturaAnagrafica As Boolean,
                                           objParametri_Super_Server As AgronicaCoreParametri,
                                           objParametri_Server As AgronicaCoreParametri,
                                           objParametri_Utenti As AgronicaCoreParametri,
                                           Optional isRibaltamentoToAgenda As Boolean = False
                                           ) As List(Of metaschema.avversita.AvversitaGruppo)

        Dim avversitaInnescoList As New List(Of metaschema.avversita.AvversitaGruppo)

        Dim Dt_Giacenze As DataTable = Nothing

        Dim Dt_Giacenze_Tot As DataTable = Nothing

        Dim DrGiacenze() As DataRow = Nothing

        Dim DrGiacenze_Tot() As DataRow = Nothing

        Dim ListCodiciAvversitaMagazzino As New List(Of Integer)

        Dim piva = STD_Utility.getPivaDaImpianti(impianti)

        Dim chiaviMagazziniUso_da_terzi As List(Of Fabbricato.PK) = Nothing

        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti.
        'ma si è valutato di fermarsi a livello di super user o azienda, quindi per il momento è sufficiente passare la PIVA 
        Dim gestioneMagazzino = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, CostantiPersonalizzate.INNESCHI, valoreDefault:=1, objParametri_Utenti, objParametri_Server))
        Dim gestioneGiacenze = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.INNESCHI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
        Dim gestioneLotto = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(piva, Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, CostantiPersonalizzate.INNESCHI, enum_Gestione_Lotti.Nessuna, objParametri_Utenti, objParametri_Server))
        Dim bloccaGiacenze_Utente = objUtentiImpostazioni.LeggiConDefault(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, Username_1Utente_o_2SuperUser:=1, "0", objParametri_Utenti)

        'DT: in caso di magazzino esterno partiamo prendendo tutti i prodotti, senza considerare la giacenza e filtriamo successivamente, in base alle impostazioni di ogni singolo magazzino esterno
        If magazziniEsterni Then
            gestioneGiacenze = enum_Gestione_Giacenze.TuttiProdotti
        End If

        Dim usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero As Boolean
        STD_Utility.GetAssettoMagazzino(tipoAttivita, statoAttivita, gestioneLotto, gestioneMagazzino, gestioneGiacenze, bloccaGiacenze_Utente, escludiGiacenzeZero, usaLotto, usaMagazzino, usaAnagrafica, flagQtaMaggioreZero)

        If soloLetturaAnagrafica Then
            usaAnagrafica = True
            usaMagazzino = False
        End If

        If usaMagazzino Then

            Dim xFiltroAggiuntivo_MagazzinoAttivoAllaData As String = " AND (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(validitaFine) & " AND Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(validitaFine) & ")"

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dim xFiltroAggiuntivo As String = ""

            xFiltroAggiuntivo &= xFiltroAggiuntivo_MagazzinoAttivoAllaData

            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(validitaFine,
                                                       piva, 0, 0,
                                                       INNESCHI,
                                                       Pro_Cod:=0,
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
                                                       Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

            Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                                           piva, 0, 0,
                                                           INNESCHI,
                                                           Pro_Cod:=0,
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
                                                           Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

            STD_Utility.FiltraGiacenze_X_MagazziniUso_da_terzi(chiaviMagazziniUso_da_terzi, Dt_Giacenze, Dt_Giacenze_Tot)

            For i = 0 To Dt_Giacenze.Rows.Count - 1
                ListCodiciAvversitaMagazzino.Add(Dt_Giacenze.Rows(i).Item("Pro_Cod"))
            Next

            If IsNothing(ListCodiciAvversitaMagazzino) OrElse ListCodiciAvversitaMagazzino.Count = 0 Then
                'se non ho alcun formulato in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
                If Not usaAnagrafica Then
                    Return avversitaInnescoList
                End If
            End If
        End If


        Dim avversitaList As New List(Of metaschema.avversita.AvversitaGruppo)

        avversitaList = LeggiAvversita(tipoAttivita, lavorazione, impianti, specie, disciplinare, epocaDPI, dettaglioTrattamento, validitaFine, objParametri_Super_Server,
                                        objParametri_Server, objParametri_Utenti, isRibaltamentoToAgenda:=isRibaltamentoToAgenda)

        If Not IsNothing(avversitaList) AndAlso avversitaList.Count > 0 Then

            If usaMagazzino AndAlso Not usaAnagrafica Then
                avversitaList = avversitaList.FindAll(Function(a) ListCodiciAvversitaMagazzino.Contains(a.codice))
            End If

            If Not IsNothing(avversitaList) AndAlso avversitaList.Count > 0 Then

                For Each avversita In avversitaList

                    Dim avversitaInnesco As metaschema.avversita.AvversitaGruppo = avversita

                    avversitaInnesco.MagazziniMovimentazioni = New List(Of attivita.RilevamentoDiMagazzino)

                    Dim trovataGiacenza As Boolean = False

                    If usaMagazzino Then

                        Dim av_cod As Integer = avversitaInnesco.codice

                        If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                            DrGiacenze = Dt_Giacenze.Select("pro_cod=" & av_cod)
                        End If

                        If Not flagQtaMaggioreZero AndAlso usaAnagrafica Then
                            'Aggiungo sempre anche l'avversitaInnesco senza magazzino così l'utente può scegliere di andare sotto giacenza per tutti gli Inneschi
                            avversitaInnescoList.Add(avversitaInnesco)
                        End If

                        If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                            For g = 0 To DrGiacenze.Length - 1

                                'If magazziniEsterni Then
                                '    Dim gestioneGiacenzeMagazzinoEsterno = CInt(objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(DrGiacenze(g).Item("Piva"), Nothing, enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, CostantiPersonalizzate.FORMULATI, enum_Gestione_Giacenze.TuttiProdotti, objParametri_Utenti, objParametri_Server))
                                '    Dim qtaMagazzinoEsterno = Math.Round(DrGiacenze(g).Item("Giacenza"), 4)
                                '    If gestioneGiacenzeMagazzinoEsterno = enum_Gestione_Giacenze.SoloPresenti Then
                                '        If qtaMagazzinoEsterno <= 0 Then
                                '            Continue For
                                '        End If
                                '    End If
                                'End If

                                avversitaInnesco = avversitaInnesco.Clona()
                                avversitaInnesco.MagazziniMovimentazioni = New List(Of attivita.RilevamentoDiMagazzino)

                                Dim rilevamentoMagazzino As New attivita.RilevamentoDiMagazzino()
                                rilevamentoMagazzino.Prodotto = New attivita.risorse.Prodotto(DrGiacenze(g).Item("Pro_Cod"), DrGiacenze(g).Item("Elem_Cod"))
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
                                    DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & av_cod & " and Piva='" & DrGiacenze(g).Item("Piva") & "' and Sa_Cod=" & DrGiacenze(g).Item("Sa_Cod") & " and Id_Destinazione=" & DrGiacenze(g).Item("Id_Destinazione") & " and lotto='" & UtilityProvider.Agro_SQL_SaveText(DrGiacenze(g).Item("lotto")) & "'")
                                    If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                        rilevamentoMagazzino.QtaTot = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4)
                                    End If
                                End If

                                'If Not IsNothing(chiaviMagazziniUso_da_terzi) AndAlso chiaviMagazziniUso_da_terzi.FindIndex(Function(u) u.centroAziendalePK.partitaIva = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                '                                                                                                    u.centroAziendalePK.codice = rilevamentoMagazzino.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                '                                                                                                    u.codice = rilevamentoMagazzino.Magazzino.primaryKey.codice) > -1 Then
                                '    rilevamentoMagazzino.Magazzino.usoDaTerzi = True

                                '    'Se il magazzino ha il fabbricato codice Uso da Terzi aggiungo alla descrizione del Magazzino l'Azienda
                                '    rilevamentoMagazzino.Magazzino.descrizione &= " (" & AgronicaCoreDataProvider.My.Resources.Gias.Azienda & ": " & CStr(DrGiacenze(g).Item("Impresa")) & ")"
                                'Else
                                '    rilevamentoMagazzino.Magazzino.usoDaTerzi = False
                                'End If

                                rilevamentoMagazzino.Magazzino.usoDaTerzi = False

                                avversitaInnesco.MagazziniMovimentazioni.Add(rilevamentoMagazzino)

                                avversitaInnescoList.Add(avversitaInnesco)

                                trovataGiacenza = True
                            Next

                        End If
                    Else
                        avversitaInnescoList.Add(avversitaInnesco)
                    End If
                Next
            End If


        End If

        Return avversitaInnescoList

    End Function

End Class
