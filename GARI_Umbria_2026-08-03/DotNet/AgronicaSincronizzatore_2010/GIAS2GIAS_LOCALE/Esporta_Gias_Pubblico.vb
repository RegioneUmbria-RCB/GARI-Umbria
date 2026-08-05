Imports System.Text
Imports System.Xml
Imports AgronicaConversioneCartografiaGias
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json

Public Class Esporta_Gias_Pubblico

    Const Classe = "Esporta_Gias_Pubblico"

    'Private SuperUser As String
    'Private SuperUserPiva As String
    'Private NomeUtente As String
    'Private Password As String
    'Private ConnessioneAlternativa As String = "cnGias_Server"
    'Private ConnessioneAlternativaUtenti As String = "cnUtenti"
    'Private PathFileINI As String = "c:/agroconnessioni/connessioni.ini"
    Private CartellaFileXml As String
    'Private Id_Servizio As Integer = 8
    Private FinestraTemp_Inizio As Date = AGRODATAINIZIO
    Private FinestraTemp_Fine As Date = AGRODATAFINE
    'Private DoorKey As String
    'Private Username As String

    Public Filtro_Piva As String


    Private Flag_Esporta_SOLO_ImpreseConImpianti As Integer
    Private Flag_Crea_Nodo_Impresa As Integer
    Private Flag_Crea_Nodo_Centro As Integer
    Private Flag_Crea_Nodo_Particella As Integer
    Private Flag_Crea_Nodo_Fabbricato As Integer
    Private Flag_Crea_Nodo_Campo As Integer
    Private Flag_Crea_Nodo_CampoParticella As Integer
    Private Flag_Crea_Nodo_Appezzamento As Integer
    Private Flag_Crea_Nodo_AppezzamentoParticella As Integer
    Private Flag_Crea_Nodo_Impianto As Integer
    Private Flag_Crea_Nodo_Progetto As Integer

    Private objParametri_server As AgronicaCoreParametri
    Private objParametri_utenti As AgronicaCoreParametri

    Private FiltroPerOrganismoReferente As Integer

    ' Private mStream As System.IO.StreamWriter
    'Public strNotifica As String
    ' Public Event Notifica(ByVal sender As Object, ByVal e As System.EventArgs)

    '###########################################################################################
    Public Sub New(ByVal objParametri_Server As AgronicaCoreParametri,
                   ByVal objParametri_utenti As AgronicaCoreParametri,
                   Optional ByVal FinestraTemp_Inizio As Date = #1/1/1900#,
                    Optional ByVal FinestraTemp_Fine As Date = #12/31/2100#,
                   Optional ByVal CartellaFileXml As String = "C:\Giaslan\File_Esportazioni\",
                   Optional ByVal Flag_Esporta_SOLO_ImpreseConImpianti As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Impresa As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Centro As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Particella As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Fabbricato As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Campo As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_CampoParticella As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Appezzamento As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_AppezzamentoParticella As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Impianto As Integer = 1,
                   Optional ByVal Flag_Crea_Nodo_Progetto As Integer = 1,
                   Optional ByVal FiltroPerOrganismoReferente As Integer = 1)

        'ByVal SuperUser As String,
        '           ByVal SuperUserPiva As String,
        '           ByVal DoorKey As String,
        '           ByVal Username As String,
        '           ByVal Password As String,
        'Optional ByVal CartellaFileXml As String = "C:\Giaslan\Files_Xml_Anagrafiche",
        '    Optional ByVal ConnessioneAlternativa As String = "cnONLINE_Server",
        '    Optional ByVal ConnessioneAlternativaUtenti As String = "cnONLINE_Utenti",
        '    Optional ByVal PathFileINI As String = "c:/agroconnessioni/connessioni.ini",
        '    Optional ByVal Id_Servizio As Integer = 8,


        'Me.SuperUser = SuperUser
        'Me.SuperUserPiva = SuperUserPiva
        'Me.DoorKey = DoorKey
        'Me.Username = Username
        'Me.Password = Password
        'Me.ConnessioneAlternativa = ConnessioneAlternativa
        'Me.ConnessioneAlternativaUtenti = ConnessioneAlternativaUtenti
        'Me.PathFileINI = PathFileINI
        'Me.Id_Servizio = Id_Servizio
        Me.FinestraTemp_Inizio = FinestraTemp_Inizio
        Me.FinestraTemp_Fine = FinestraTemp_Fine
        Me.CartellaFileXml = CartellaFileXml

        ' VAnni: 8/8/2019: aggiunte in questa versione
        Me.objParametri_server = objParametri_Server
        Me.objParametri_utenti = objParametri_utenti
        Me.FiltroPerOrganismoReferente = FiltroPerOrganismoReferente

        Me.Flag_Esporta_SOLO_ImpreseConImpianti = Flag_Esporta_SOLO_ImpreseConImpianti
        Me.Flag_Crea_Nodo_Impresa = Flag_Crea_Nodo_Impresa
        Me.Flag_Crea_Nodo_Centro = Flag_Crea_Nodo_Centro
        Me.Flag_Crea_Nodo_Particella = Flag_Crea_Nodo_Particella
        Me.Flag_Crea_Nodo_Fabbricato = Flag_Crea_Nodo_Fabbricato
        Me.Flag_Crea_Nodo_Campo = Flag_Crea_Nodo_Campo
        Me.Flag_Crea_Nodo_CampoParticella = Flag_Crea_Nodo_CampoParticella
        Me.Flag_Crea_Nodo_Appezzamento = Flag_Crea_Nodo_Appezzamento
        Me.Flag_Crea_Nodo_AppezzamentoParticella = Flag_Crea_Nodo_AppezzamentoParticella
        Me.Flag_Crea_Nodo_Impianto = Flag_Crea_Nodo_Impianto
        Me.Flag_Crea_Nodo_Progetto = Flag_Crea_Nodo_Progetto


    End Sub


    Private Shared Sub FinestraTemporaleRecupera(objParametri_Server As AgronicaCoreParametri, ByRef FinestraTemporaleInizioPrecedente As Date, ByRef FinestraTemporaleFinePrecedente As Date)
        FinestraTemporaleInizioPrecedente =
            objParametri_Server.FinestraTemporaleInizio
        FinestraTemporaleFinePrecedente =
            objParametri_Server.FinestraTemporaleFine
    End Sub

    Private Shared Sub FinestraTemporaleImpostaValori(objParametri_Server As AgronicaCoreParametri, Inizio As Date, Fine As Date)
        objParametri_Server.FinestraTemporaleInizio = Inizio
        objParametri_Server.FinestraTemporaleFine = Fine
    End Sub


    Public Function Esporta_Dati_Agenda(ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
                                        ByVal CfgDatiImpresa As AgronicaCoreModello.clsDatiImpresa,
                                        ByVal CartellaFileXml As String,
                                        ByVal Flag_GestioneCodifiche As Boolean,
                                        ByVal codice_GIAS As Integer,
                                        ByRef Log_G2G As StringBuilder,
                                        ByRef Log_Errori As StringBuilder
                                        ) As String

        Dim NomeFunzione As String = "Esporta_Dati_Agenda"

        Dim strXmlDocRaval As String = ""

        Dim FinestraTemporaleRecupera_inizio As Date
        Dim FinestraTemporaleRecupera_fine As Date

        FinestraTemporaleRecupera(objParametri_server, FinestraTemporaleRecupera_inizio, FinestraTemporaleRecupera_fine)

        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Imposta finestra temporale " & FinestraTemp_Inizio.ToString & " - " & FinestraTemp_Fine.ToString)
        FinestraTemporaleImpostaValori(objParametri_server, Me.FinestraTemp_Inizio, Me.FinestraTemp_Fine)

        Dim cfg As AgronicaCoreModello.clsCfgImpostazioniTrasformazioni = Nothing

        If objOpzioni.ImpostazioniTrasformazioni <> "" Then
            cfg = JsonConvert.DeserializeObject(Of AgronicaCoreModello.clsCfgImpostazioniTrasformazioni)(objOpzioni.ImpostazioniTrasformazioni)
        End If

        'scorro la griglia ordinata, per ciascun dettaglio imposto un elemento xml.
        Dim rval As XDocument =
                XDocument.Parse("<utente username="""" password="""" codice=""""></utente>")

        Try


            Dim NomeDB_Utenti As String = objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            Dim LetturaDettagli As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim dtGriglia As DataTable =
                LetturaDettagli.Leggi_DestinazioneConInformazioniRiepilogative(
                RaggruppaPerCampo:=False,
                piva:=Filtro_Piva,
                sa_Cod:=0,
                veg_cod:=-1,
                id_Cod:=0,
                validita_inizio:=FinestraTemp_Inizio,
                validita_fine:=FinestraTemp_Fine,
                FiltroImpianti:="",
                NomeDB_Utenti:=NomeDB_Utenti,
                Nothing,
                xFiltroAggiuntivo:=" a.lav_cod = 14 ",
                xOrderBy:=" mTes.Data_Movimento Desc, d.id_agenda, d.id_mov_det ",
                objParametri:=objParametri_server
            )



            Dim chiaveCorrente As String = ""
            Dim chiaveUltima As String = ""
            Dim nodoInnesto As XElement = Nothing
            Dim nodoImpianto As XElement = Nothing
            Dim flgPrimoPassaggio As Boolean = True

            For Each dr In dtGriglia.Rows


                chiaveCorrente = dr("id_agenda") & "-" & dr("id_mov") & "-" & dr("id_mov_det")
                nodoImpianto = <Destinazione codice_impianto=<%= dr("piva") & "-" & dr("sa_Cod") & "-" & dr("appezza") & "-" & dr("id_reg") %> superficie_trattata=<%= dr("SupTrattata").ToString.Replace(".", ",") %>></Destinazione>

                'cambia dettaglio agenda, innesto nodi xml in albero, altrimenti continuo a costruire dettagli.
                If chiaveCorrente <> chiaveUltima Then
                    If Not flgPrimoPassaggio Then
                        rval.Element("utente").Add(nodoInnesto)
                    End If
                    nodoInnesto = <Fertilizzazione tipo_operazione="1" codice=<%= chiaveCorrente %> data=<%= CDate(dr("data_movimento")).ToShortDateString() %>>
                                      <Prodotto quantita=<%= dr("DoseHA_N").ToString.Replace(".", ",") %> unita_misura="K"></Prodotto>
                                  </Fertilizzazione>

                    nodoInnesto.Add(nodoImpianto)
                Else

                    nodoInnesto.Add(nodoImpianto)
                End If

                chiaveUltima = chiaveCorrente
                flgPrimoPassaggio = False

            Next 'riga in griglia


            rval.Element("utente").Add(nodoInnesto)

        Catch ex As Exception
            Throw New Exception(CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message)
        End Try

        'reimposta la finestra principale precedente (letta all'inizio)
        FinestraTemporaleImpostaValori(objParametri_server, FinestraTemporaleRecupera_inizio, FinestraTemporaleRecupera_fine)
        strXmlDocRaval = rval.ToString()

        Return strXmlDocRaval

    End Function


    '###############################################################################################
    Public Function Esporta_Dati(
        ByVal objOpzioni As Gias2Gias_LIB.clsOpzioni,
        ByVal CfgDatiImpresa As AgronicaCoreModello.clsDatiImpresa,
        ByVal CartellaFileXml As String,
        ByVal Flag_GestioneCodifiche As Boolean,
        ByVal codice_GIAS As Integer,
        ByRef Log_G2G As StringBuilder,
        ByRef Log_Errori As StringBuilder
    ) As String

        Dim NomeFunzione As String = "Esporta_Dati"

        Dim strXmlDocRaval As String = ""

        Dim XmlDoc As New Xml.XmlDocument

        Dim ErroreFlag As Boolean

        Dim Piva As String = ""
        Dim RagSoc As String = ""

        Dim Dt_Imprese As New DataTable
        Dim Dt_Centri As New DataTable
        Dim Dt_Campi As New DataTable
        Dim Dt_Appezzamenti As New DataTable
        Dim Dt_Impianti As New DataTable
        Dim Dt_ImpresaCentri As New DataTable
        Dim Dt_ImpresaCampi As New DataTable
        Dim Dt_ImpresaAppezzamenti As New DataTable
        Dim Dt_ImpresaImpianti As New DataTable

        'Dim Dt_Codifica_Veg_Cod As DataTable
        Dim Dt_Codifica_Cultivar As DataTable = Nothing
        Dim Dt_Codifica_FormeAllevamento As DataTable = Nothing
        Dim Dt_Codifica_Portinnesti As DataTable = Nothing

        Dim Riga_Dati_Centri As DataRow()
        Dim Riga_Dati_Campi As DataRow()
        Dim Riga_Dati_Appezzamenti As DataRow()
        Dim Riga_Dati_Impianti As DataRow()

        Dim i, j As Integer
        Dim CreaImpresa As Boolean = True

        Dim FinestraTemporaleRecupera_inizio As Date
        Dim FinestraTemporaleRecupera_fine As Date

        FinestraTemporaleRecupera(objParametri_server, FinestraTemporaleRecupera_inizio, FinestraTemporaleRecupera_fine)

        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Imposta finestra temporale " & FinestraTemp_Inizio.ToString & " - " & FinestraTemp_Fine.ToString)
        FinestraTemporaleImpostaValori(objParametri_server, Me.FinestraTemp_Inizio, Me.FinestraTemp_Fine)

        Dim cfg As AgronicaCoreModello.clsCfgImpostazioniTrasformazioni = Nothing

        If objOpzioni.ImpostazioniTrasformazioni <> "" Then
            cfg = JsonConvert.DeserializeObject(Of AgronicaCoreModello.clsCfgImpostazioniTrasformazioni)(objOpzioni.ImpostazioniTrasformazioni)
        End If

        Dim GEORiferimento_Cod As Integer = -1
        Dim Gis_Appezzamento_TipoEntitaCheSurrogaDati As enum_GIS2012_TipoEntita = enum_GIS2012_TipoEntita.NON_DEFINITO

        If CfgDatiImpresa.Flagimporta_gis AndAlso cfg IsNot Nothing Then
            GEORiferimento_Cod = cfg.GEORiferimento_Cod
            Gis_Appezzamento_TipoEntitaCheSurrogaDati = cfg.Gis_Appezzamento_TipoEntitaCheSurrogaDati
        End If

        Try


            Dim utentiImpostazioniLeggi As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim AnnataAgrariariferimanto_da As Date
            Dim AnnataAgrariariferimanto_a As Date

            utentiImpostazioniLeggi.AnnataAgraria(Now.Date, AnnataAgrariariferimanto_da, AnnataAgrariariferimanto_a, objParametri_utenti)

            Dim ParametriCartografici As ParametriCoordinateConverter = Nothing
            Dim cconverter As New Agronica.CoordinateConverter

            If GEORiferimento_Cod <> -1 Then

                Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
                Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(GEORiferimento_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)


                ParametriCartografici = New ParametriCoordinateConverter With {
                    .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                    .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                    .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                    .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                    .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                    .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
                }

            End If

            If Not ErroreFlag Then

                If Flag_Crea_Nodo_Impresa = 1 Then

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Impresa = 1")

                    Leggi_DatiImpreseUtente(
                        Gis_Appezzamento_TipoEntitaCheSurrogaDati,
                        CfgDatiImpresa,
                        Dt_Imprese,
                        Dt_Centri,
                        Dt_Campi,
                        Dt_Appezzamenti,
                        Dt_Impianti,
                        objParametri_server,
                        Filtro_Piva)

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "post Leggi_DatiImpreseUtente")

                    Dt_ImpresaCentri = Nothing
                    Dt_ImpresaCampi = Nothing
                    Dt_ImpresaAppezzamenti = Nothing
                    Dt_ImpresaImpianti = Nothing


                    If Dt_Imprese IsNot Nothing AndAlso Dt_Imprese.Rows.Count > 0 Then

                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio generazione file di esportazione...")

                        If Flag_GestioneCodifiche Then
                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Lettura Codifiche ...")
                            Leggi_Codifiche(Dt_Codifica_Cultivar, Dt_Codifica_FormeAllevamento, Dt_Codifica_Portinnesti, objParametri_server)
                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Codifiche lette ...")
                        End If

                        For i = 0 To Dt_Imprese.Rows.Count - 1

                            Piva = Dt_Imprese.Rows(i).Item("piva")
                            RagSoc = Dt_Imprese.Rows(i).Item("rag_soc")

                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "----------- " & RagSoc & " ----------- ")

                            If Flag_Crea_Nodo_Centro = 1 Then

                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Centro = 1")

                                If Dt_Centri IsNot Nothing AndAlso Dt_Centri.Rows.Count > 0 Then

                                    Dt_ImpresaCentri = Dt_Centri.Clone
                                    Riga_Dati_Centri = Dt_Centri.Select("piva='" & Piva & "'")

                                    If CfgDatiImpresa.Flagimporta_gis Then

                                        If Not IsNothing(Riga_Dati_Centri) AndAlso Riga_Dati_Centri.Length > 0 _
                                                AndAlso Not IsNothing(Riga_Dati_Centri(0)("Gis_wkt")) Then
                                            For j = 0 To Riga_Dati_Centri.Length - 1
                                                If Riga_Dati_Centri(j)("Gis_wkt") <> "" AndAlso GEORiferimento_Cod > 0 Then
                                                    Dim wkt As String = Riga_Dati_Centri(j)("Gis_wkt")
                                                    wkt = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici)
                                                    If wkt.Contains("POINT") Then
                                                        wkt = wkt.Replace("((", "(").Replace("))", ")")
                                                    End If
                                                    Riga_Dati_Centri(j)("Gis_wkt") = wkt
                                                End If
                                                Dt_ImpresaCentri.ImportRow(Riga_Dati_Centri(j))
                                            Next
                                        End If

                                    Else
                                        If Not IsNothing(Riga_Dati_Centri) AndAlso Riga_Dati_Centri.Length > 0 Then
                                            For j = 0 To Riga_Dati_Centri.Length - 1
                                                Dt_ImpresaCentri.ImportRow(Riga_Dati_Centri(j))
                                            Next
                                        End If
                                    End If

                                    If Flag_Crea_Nodo_Campo = 1 Then

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Campo = 1")

                                        If Dt_Campi IsNot Nothing AndAlso Dt_Campi.Rows.Count > 0 Then
                                            Dt_ImpresaCampi = Dt_Campi.Clone
                                            Riga_Dati_Campi = Dt_Campi.Select("piva='" & Piva & "'")
                                            For j = 0 To Riga_Dati_Campi.Length - 1
                                                Dt_ImpresaCampi.ImportRow(Riga_Dati_Campi(j))
                                            Next
                                        Else
                                            Dt_ImpresaCampi = Nothing
                                        End If

                                        If Flag_Crea_Nodo_Appezzamento = 1 Then

                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Appezzamento = 1")

                                            Dt_ImpresaAppezzamenti = Dt_Appezzamenti.Clone
                                            Riga_Dati_Appezzamenti = Dt_Appezzamenti.Select("piva='" & Piva & "'")

                                            If CfgDatiImpresa.Flagimporta_gis Then
                                                If Not IsNothing(Riga_Dati_Appezzamenti) AndAlso Riga_Dati_Appezzamenti.Length > 0 _
                                                AndAlso Not IsNothing(Riga_Dati_Appezzamenti(0)("Gis_wkt")) Then
                                                    For j = 0 To Riga_Dati_Appezzamenti.Length - 1
                                                        If Riga_Dati_Appezzamenti(j)("Gis_wkt") <> "" AndAlso GEORiferimento_Cod > 0 Then
                                                            Dim wkt As String = Riga_Dati_Appezzamenti(j)("Gis_wkt")
                                                            Riga_Dati_Appezzamenti(j)("Gis_wkt") = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, False, ParametriCartografici, True)
                                                        End If
                                                        Dt_ImpresaAppezzamenti.ImportRow(Riga_Dati_Appezzamenti(j))
                                                    Next
                                                End If
                                            Else
                                                If Not IsNothing(Riga_Dati_Appezzamenti) AndAlso Riga_Dati_Appezzamenti.Length > 0 Then
                                                    For j = 0 To Riga_Dati_Appezzamenti.Length - 1
                                                        Dt_ImpresaAppezzamenti.ImportRow(Riga_Dati_Appezzamenti(j))
                                                    Next
                                                End If
                                            End If

                                            If Me.Flag_Crea_Nodo_Impianto = 1 Then

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Impianto = 1")

                                                Dt_ImpresaImpianti = Dt_Impianti.Clone
                                                Riga_Dati_Impianti = Dt_Impianti.Select("piva='" & Piva & "'")
                                                For j = 0 To Riga_Dati_Impianti.Length - 1
                                                    Dt_ImpresaImpianti.ImportRow(Riga_Dati_Impianti(j))
                                                Next
                                            Else
                                                Dt_ImpresaImpianti = Nothing
                                            End If
                                        Else
                                            Dt_ImpresaAppezzamenti = Nothing
                                            Dt_ImpresaImpianti = Nothing
                                        End If


                                    Else
                                        Dt_ImpresaCampi = Nothing
                                        Dt_ImpresaAppezzamenti = Nothing
                                        Dt_ImpresaImpianti = Nothing
                                    End If


                                Else
                                    Dt_ImpresaCentri = Nothing
                                    Dt_ImpresaCampi = Nothing
                                    Dt_ImpresaAppezzamenti = Nothing
                                    Dt_ImpresaImpianti = Nothing

                                End If

                            Else
                                Dt_ImpresaCentri = Nothing
                                Dt_ImpresaCampi = Nothing
                                Dt_ImpresaAppezzamenti = Nothing
                                Dt_ImpresaImpianti = Nothing
                            End If

                            CreaImpresa = True

                            If Flag_Esporta_SOLO_ImpreseConImpianti = 1 Then

                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Esporta_SOLO_ImpreseConImpianti = 1")

                                'verifico che l'impresa abbia impianti..
                                If Dt_ImpresaImpianti Is Nothing Then
                                    CreaImpresa = False
                                Else
                                    If Dt_ImpresaImpianti.Rows.Count <= 0 Then
                                        CreaImpresa = False
                                    End If
                                End If

                            End If

                            If CreaImpresa Then

                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio creazione xml")

                                XmlDoc = Crea_XML(AnnataAgrariariferimanto_da,
                                                    AnnataAgrariariferimanto_a,
                                                    Piva,
                                                    Dt_ImpresaCentri,
                                                    Dt_ImpresaCampi,
                                                    Dt_ImpresaAppezzamenti,
                                                    Dt_ImpresaImpianti,
                                                    Flag_GestioneCodifiche,
                                                    Dt_Codifica_Cultivar, Dt_Codifica_FormeAllevamento, Dt_Codifica_Portinnesti,
                                                    codice_GIAS,
                                                     CfgDatiImpresa.Flagimporta_gis,
                                                    Log_G2G,
                                                    objParametri_server)


                                If CartellaFileXml <> "" Then

                                    Dim NomeFileOut As String
                                    Dim Anno As String = Date.Today.Year.ToString
                                    Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
                                    Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
                                    Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
                                    Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
                                    Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
                                    Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

                                    NomeFileOut = CartellaFileXml & Piva & "_" & time & ".xml"

                                    XmlDoc.Save(NomeFileOut)
                                End If

                                strXmlDocRaval = XmlDoc.OuterXml

                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato il file di esportazione dell'Impresa " & RagSoc)

                            End If

                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "----------- FINE: " & RagSoc & " ----------- ")

                        Next

                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Processo Esporta_Dati concluso")

                    Else
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Dt_Imprese vuoto")
                    End If
                Else
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Flag_Crea_Nodo_Impresa <> 1 --> uscita ")
                End If
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Errore G2G_Stringa_Decodifica ")
            End If


        Catch ex As Exception
            Throw New Exception(CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message)
        End Try

        'reimposta la finestra principale precedente (letta all'inizio)
        FinestraTemporaleImpostaValori(objParametri_server, FinestraTemporaleRecupera_inizio, FinestraTemporaleRecupera_fine)

        Return strXmlDocRaval

    End Function



    '###############################################################################################
    Public Function Crea_XML(ByVal AnnataAgrariariferimanto_da As Date,
                             ByVal AnnataAgrariariferimanto_a As Date,
                             ByVal Piva As String,
                             ByVal Dt_Centri As DataTable,
                             ByVal Dt_Campi As DataTable,
                             ByVal Dt_Appezzamenti As DataTable,
                             ByVal Dt_Impianti As DataTable,
                             ByVal Flag_GestioneCodifiche As Boolean,
                             ByVal Dt_Codifica_Cultivar As DataTable,
                             ByVal Dt_Codifica_FormeAllevamento As DataTable,
                             ByVal Dt_Codifica_Portinnesti As DataTable,
                             ByVal codice_GIAS As Integer,
                             ByVal Flag_GIS As Boolean,
                             ByRef Log_G2G As StringBuilder,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As XmlDocument

        Dim NomeFunzione As String = "Crea_XML"

        Dim Riga_Dati_Campi As DataRow()

        Dim Sa_Cod As Integer
        Dim Sa_Nome As String = ""
        Dim Campo_Cod As Integer
        Dim Campo_Des As String = ""

        Dim XmlDocPrivato As New XmlDocument
        Dim XmlDocPubblico As New XmlDocument

        Dim XML_ImpresaPrivato As XmlElement
        Dim XML_ImpresaIndirizzoPrivato As XmlElement
        Dim XML_ImpresaCodicePrivato As XmlElement
        Dim XMLs_ImpresaCodicePrivato As XmlNodeList
        Dim XML_CentroPrivato As XmlElement
        Dim XML_CentroIndirizzoPrivato As XmlElement
        Dim XML_CentroCodicePrivato As XmlElement
        Dim XMLs_CentroCodicePrivato As XmlNodeList
        Dim XML_CentroRubricaPrivato As XmlElement
        Dim XMLs_CentroRubricaPrivato As XmlNodeList
        Dim XML_CampoPrivato As XmlElement
        Dim XML_CampoCodicePrivato As XmlElement
        Dim XMLs_CampoCodicePrivato As XmlNodeList
        Dim XML_ImpresaPubblico As XmlElement
        Dim XML_CentroPubblico As XmlElement
        Dim XML_ParticellaPubblico As XmlElement
        Dim XML_CampoPubblico As XmlElement = Nothing
        Dim XML_CampoDatiParticellePrivato As XmlElement
        Dim XML_CampoParticellaPrivato As XmlElement
        Dim XMLs_CampoParticellaPrivato As XmlNodeList
        Dim XML_CampoParticellaPubblico As XmlElement

        Dim objImpresa As New AgronicaCoreAnagrafeBIZ.Impresa_R
        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objCentro As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
        Dim objCampo As New AgronicaCoreAnagrafeBIZ.Campo_R
        Dim objParticella As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objPartCodici As New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R

        Dim strImpresa As String
        Dim strCentro As String
        Dim strCampo As String

        Dim strRisultato As String = ""

        Dim Rag_Soc As String
        Dim Cuaa As String
        Dim Cod_GGN As String
        Dim data_iscrizione_libro_soci As String
        Dim Codice_Fiscale As String
        Dim Cod_Socio As String
        Dim Sup_Totale As String
        Dim Piva_Padre As String
        Dim Tipo_Gerarchia As String
        Dim i_Titolo_Possesso As String
        Dim i_Validita_Inizio As String
        Dim i_Validita_Fine As String

        Dim i_indirizzo As String
        Dim i_frazione As String
        Dim i_cap As String
        Dim i_comune As String
        Dim i_provincia As String
        Dim i_stato As String
        Dim i_note_indirizzo As String
        Dim i_codice_istat_comune As String
        Dim i_codice_istat_provincia As String

        Dim legale_rappresentante As String
        Dim lr_codice_fiscale As String
        Dim lr_sesso As String
        Dim lr_validita_inizio As String
        Dim lr_validita_fine As String
        Dim lr_indirizzo As String
        Dim lr_frazione As String
        Dim lr_cap As String
        Dim lr_comune As String
        Dim lr_provincia As String
        Dim lr_stato As String
        Dim lr_note_indirizzo As String
        Dim lr_codice_istat_comune As String
        Dim lr_codice_istat_provincia As String
        Dim lr_nascita_data As String
        Dim lr_nascita_comune As String
        Dim lr_nascita_provincia As String
        Dim lr_nascita_istat_comune As String
        Dim lr_nascita_istat_provincia As String

        Dim lr_documenti As String
        Dim lr_rubrica_1 As String
        Dim lr_rubrica_2 As String
        Dim lr_rubrica_3 As String
        Dim lr_rubrica_4 As String
        Dim lr_rubrica_5 As String

        Dim CF_tecnico_referente As String

        Dim Codice_Cliente As String
        Dim Codice_Fornitore As String
        Dim Codice_Fornitore_2 As String
        Dim Codice_Fornitore_3 As String

        Dim i_Chiave_Cliente As String

        Dim Codice As String
        Dim Valore As String

        Dim Numero As String
        Dim Descr As String

        Dim Dt_Contatti As DataTable
        Dim Filtro_Contatto As String

        Dim Sup_Bosco As String
        Dim Sup_Prati As String
        Dim Codice_Operatore As String
        Dim Tipo_Attivita As String

        Dim c_Titolo_Possesso As String
        Dim c_Validita_Inizio As String
        Dim c_Validita_Fine As String
        Dim c_indirizzo As String
        Dim c_frazione As String
        Dim c_cap As String
        Dim c_comune As String
        Dim c_provincia As String
        Dim c_stato As String
        Dim c_note_indirizzo As String
        Dim c_codice_istat_comune As String
        Dim c_codice_istat_provincia As String

        Dim c_rubrica_1 As String
        Dim c_rubrica_2 As String
        Dim c_rubrica_3 As String
        Dim c_rubrica_4 As String
        Dim c_rubrica_5 As String
        Dim c_rubrica_6 As String
        Dim c_rubrica_7 As String
        Dim c_rubrica_8 As String
        Dim c_rubrica_9 As String
        Dim c_Chiave_Cliente As String

        Dim Part_Cod As String
        Dim Cod_Particella_Azi As String
        Dim p_codice_istat_comune As String
        Dim p_codice_istat_provincia As String
        Dim p_Sezione As String
        Dim p_Foglio As String
        Dim p_Numero As String
        Dim p_Subalterno As String
        Dim Partita_Catastale As String
        Dim p_Ettari As String
        Dim p_Are As String
        Dim p_Centiare As String
        Dim p_Titolo_Possesso As String
        Dim p_Titolo_Possesso_Des As String
        Dim p_Sup_Condotta As String
        Dim Qualita_Catasto_Codice As String
        Dim Classe As String
        Dim Reddito_Dominicale As String
        Dim Reddito_Agrario As String
        Dim p_Validita_Inizio_Possesso As String
        Dim p_Validita_Fine_Possesso As String

        Dim Campo_Tipo_Cod As String
        Dim Campo_Tipo_Des As String
        Dim cam_Gru_Cod As String
        Dim cam_Veg_Cod As String
        Dim cam_Validita_Inizio As String
        Dim cam_Validita_Fine As String
        Dim cam_Chiave_Cliente As String
        Dim c_p_codice_istat_comune As String
        Dim c_p_codice_istat_provincia As String
        Dim c_p_Sezione As String
        Dim c_p_Foglio As String
        Dim c_p_Numero As String
        Dim c_p_Subalterno As String
        Dim c_p_Sup As String
        Dim c_p_Validita_Inizio As String
        Dim c_p_Validita_Fine As String

        Dim i As Integer

        Dim N_Centro As Integer
        Dim N_Particella As Integer
        Dim N_Campo As Integer
        Dim N_Codice As Integer
        Dim N_Rubrica As Integer

        Dim Dt_PartCodici As DataTable

        Dim xpa As New AgronicaCoreXML.XML_Pubblico_Anagrafe

        Try

            If Flag_Crea_Nodo_Impresa = 1 Then

                strImpresa = objImpresa.Impresa_Leggi(CStr(Piva),
                                                  False,
                                                  True,
                                                  objParametri)

                If strImpresa <> "" Then

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "IMPRESA - " & Piva)

                    XmlDocPrivato.LoadXml(strImpresa)

                    XML_ImpresaPrivato = XmlDocPrivato.SelectSingleNode("DatiImprese").SelectSingleNode("Impresa")

                    Rag_Soc = XML_ImpresaPrivato.GetAttribute("rag_soc")
                    Sup_Totale = XML_ImpresaPrivato.GetAttribute("sup_totale")
                    Tipo_Gerarchia = XML_ImpresaPrivato.GetAttribute("tipoimpresagerarchia")
                    i_Validita_Inizio = XML_ImpresaPrivato.GetAttribute("validita_inizio")
                    i_Validita_Fine = XML_ImpresaPrivato.GetAttribute("validita_fine")

                    'indirizzo 
                    XML_ImpresaIndirizzoPrivato = XML_ImpresaPrivato.SelectSingleNode("Indirizzo")
                    i_indirizzo = XML_ImpresaIndirizzoPrivato.GetAttribute("ind_des")
                    i_frazione = XML_ImpresaIndirizzoPrivato.GetAttribute("frz_des")
                    i_cap = XML_ImpresaIndirizzoPrivato.GetAttribute("cap")
                    i_comune = XML_ImpresaIndirizzoPrivato.GetAttribute("com_des")
                    i_provincia = XML_ImpresaIndirizzoPrivato.GetAttribute("pro_cod")
                    i_stato = XML_ImpresaIndirizzoPrivato.GetAttribute("stato")
                    i_codice_istat_comune = XML_ImpresaIndirizzoPrivato.GetAttribute("com_cod_istat")
                    i_codice_istat_provincia = XML_ImpresaIndirizzoPrivato.GetAttribute("pro_cod_istat")
                    i_note_indirizzo = XML_ImpresaIndirizzoPrivato.GetAttribute("note")

                    Cod_GGN = ""
                    data_iscrizione_libro_soci = ""
                    Cuaa = ""
                    Cod_Socio = ""
                    i_Titolo_Possesso = "1"
                    CF_tecnico_referente = ""
                    Codice_Cliente = ""
                    Codice_Fornitore = ""
                    Codice_Fornitore_2 = ""
                    Codice_Fornitore_3 = ""
                    i_Chiave_Cliente = ""

                    'CODICI
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Elaborazione Codici ")

                    XMLs_ImpresaCodicePrivato = XML_ImpresaPrivato.GetElementsByTagName("Codice")

                    For N_Codice = 0 To XMLs_ImpresaCodicePrivato.Count - 1

                        XML_ImpresaCodicePrivato = XMLs_ImpresaCodicePrivato(N_Codice)
                        Codice = XML_ImpresaCodicePrivato.GetAttribute("id_cod")
                        Valore = XML_ImpresaCodicePrivato.GetAttribute("val_cod")

                        Select Case Codice

                            Case enum_CodiciAnagrafe.CodiceCUAA
                                Cuaa = Valore
                            Case enum_CodiciAnagrafe.Codice_Socio
                                Cod_Socio = Valore
                            Case enum_CodiciAnagrafe.Codice_GlobalGap
                                Cod_GGN = Valore
                            Case enum_CodiciAnagrafe.Data_Iscrizione_Libro_Soci
                                data_iscrizione_libro_soci = Valore
                            Case enum_CodiciAnagrafe.TitoloPossesso
                                i_Titolo_Possesso = Valore
                            Case enum_CodiciAnagrafe.Tecnico
                                CF_tecnico_referente = Valore
                            Case enum_CodiciAnagrafe.Codice_Cliente
                                Codice_Cliente = Valore
                            Case enum_CodiciAnagrafe.Codice_Fornitore
                                Codice_Fornitore = Valore
                            Case enum_CodiciAnagrafe.Codice_Fornitore_2
                                Codice_Fornitore_2 = Valore
                            Case enum_CodiciAnagrafe.Codice_Fornitore_3
                                Codice_Fornitore_3 = Valore
                            Case 2000 To 2999
                                i_Chiave_Cliente = Valore
                        End Select

                    Next

                    '----- Cooperativa PADRE
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Cooperativa PADRE ")
                    Piva_Padre = ""

                    Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                    Dim DtGerarchia As DataTable

                    'Leggo le imprese padri associate al profilo selezionato			
                    DtGerarchia = objGerarchia.LeggixFiglio(CStr(Piva),
                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                   "",
                                   "",
                                   objParametri)

                    If DtGerarchia IsNot Nothing AndAlso DtGerarchia.Rows.Count > 0 Then
                        Piva_Padre = DtGerarchia.Rows(0).Item("padre")
                    End If

                    DtGerarchia = Nothing


                    '----------------------
                    'Codice Fiscale Impresa

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Codice Fiscale Impresa ")

                    Codice_Fiscale = ""

                    Dt_Contatti = New DataTable
                    Dt_Contatti = objContatti.Contatti_Contatto_Leggi(objParametri_server.PivaSuperUser,
                                                                    Piva,
                                                                    0,
                                                                    0,
                                                                    False,
                                                                    False,
                                                                    0,
                                                                    0,
                                                                    False,
                                                                    0,
                                                                    ID_CF_NOFILTRO,
                                                                    0,
                                                                    "",
                                                                    False,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                     0,
                                                                    "",
                                                                    "",
                                                                         objParametri_server,
                                                                        "",
                                                                        False,
                                                                        0
                                                                       )

                    If Dt_Contatti.Rows.Count <> 0 Then
                        Codice_Fiscale = CStr(Dt_Contatti.Rows(0).Item("codice_fiscale"))
                    End If

                    Dt_Contatti = Nothing

                    '--------------------
                    'Contatti

                    legale_rappresentante = ""
                    lr_codice_fiscale = ""
                    lr_sesso = ""
                    lr_nascita_data = ""
                    lr_validita_inizio = ""
                    lr_validita_fine = ""
                    lr_documenti = ""
                    lr_indirizzo = ""
                    lr_frazione = ""
                    lr_cap = ""
                    lr_comune = ""
                    lr_provincia = ""
                    lr_stato = ""
                    lr_note_indirizzo = ""
                    lr_codice_istat_comune = ""
                    lr_codice_istat_provincia = ""
                    lr_nascita_comune = ""
                    lr_nascita_provincia = ""
                    lr_nascita_istat_comune = ""
                    lr_nascita_istat_provincia = ""
                    lr_rubrica_1 = ""
                    lr_rubrica_2 = ""
                    lr_rubrica_3 = ""
                    lr_rubrica_4 = ""
                    lr_rubrica_5 = ""

                    Dt_Contatti = New DataTable

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Legale Rappresentante ")

                    Filtro_Contatto = " (tipo_indirizzo = 5 OR tipo_indirizzo = 3) "

                    Dt_Contatti = objContatti.Contatti_Contatto_Leggi(Piva,
                                                                    "",
                                                                    0,
                                                                    COD_LEGALE,
                                                                    False,
                                                                    True,
                                                                    0,
                                                                    0,
                                                                    False,
                                                                    0,
                                                                    ID_CF_NOFILTRO,
                                                                    0,
                                                                    "",
                                                                    False,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                     0,
                                                                    Filtro_Contatto,
                                                                    "",
                                                                         objParametri_server,
                                                                        "",
                                                                        False,
                                                                        0
                                                                       )

                    If Dt_Contatti.Rows.Count <> 0 Then
                        For i = 0 To Dt_Contatti.Rows.Count - 1

                            If i = 0 Then

                                legale_rappresentante = Dt_Contatti.Rows(i).Item("rag_soc")

                                lr_codice_fiscale = Dt_Contatti.Rows(i).Item("cod_contatto")

                                If lr_codice_fiscale <> "" Then
                                    lr_sesso = AgronicaCoreDataProvider.UtilityProvider.Sesso_from_CodFisc(lr_codice_fiscale, 1)
                                    lr_nascita_data = DataNascita_from_CodFisc(lr_codice_fiscale)
                                End If

                                lr_validita_inizio = Dt_Contatti.Rows(i).Item("Validita_Inizio")
                                lr_validita_fine = Dt_Contatti.Rows(i).Item("Validita_Fine")

                                lr_documenti = ""

                            End If

                            Select Case Dt_Contatti.Rows(i).Item("tipo_indirizzo")
                                Case 3 'residenza
                                    lr_indirizzo = Dt_Contatti.Rows(i).Item("ind_des")
                                    lr_frazione = Dt_Contatti.Rows(i).Item("frz_des")
                                    lr_cap = Dt_Contatti.Rows(i).Item("cap")
                                    lr_comune = Dt_Contatti.Rows(i).Item("com_des")
                                    lr_provincia = Dt_Contatti.Rows(i).Item("pro_cod")
                                    lr_stato = Dt_Contatti.Rows(i).Item("stato")
                                    lr_note_indirizzo = Dt_Contatti.Rows(i).Item("note")
                                    lr_codice_istat_comune = Dt_Contatti.Rows(i).Item("com_cod_istat")
                                    lr_codice_istat_provincia = Dt_Contatti.Rows(i).Item("pro_cod_istat")

                                Case 5 'nascita
                                    lr_nascita_comune = Dt_Contatti.Rows(i).Item("com_des")
                                    lr_nascita_provincia = Dt_Contatti.Rows(i).Item("pro_cod")
                                    lr_nascita_istat_comune = Dt_Contatti.Rows(i).Item("com_cod_istat")
                                    lr_nascita_istat_provincia = Dt_Contatti.Rows(i).Item("pro_cod_istat")
                            End Select

                        Next

                    End If

                    Dt_Contatti = Nothing


                    '--------------------
                    'Rubrica

                    If lr_codice_fiscale <> "" Then

                        Dt_Contatti = New DataTable

                        Dt_Contatti = objContatti.Contatti_Contatto_Leggi(Piva,
                                                                        lr_codice_fiscale,
                                                                        0,
                                                                        COD_LEGALE,
                                                                        False,
                                                                        False,
                                                                        0,
                                                                        0,
                                                                        True,
                                                                        0,
                                                                        ID_CF_NOFILTRO,
                                                                        0,
                                                                        "",
                                                                        False,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        0,
                                                                         0,
                                                                        "",
                                                                        "",
                                                                             objParametri_server,
                                                                            "",
                                                                            False,
                                                                            0
                                                                           )

                        If Dt_Contatti.Rows.Count <> 0 Then
                            For i = 0 To Dt_Contatti.Rows.Count - 1

                                Select Case i
                                    Case 0
                                        lr_rubrica_1 = Dt_Contatti.Rows(i).Item("numero") & ":" & Dt_Contatti.Rows(i).Item("descr")
                                    Case 1
                                        lr_rubrica_2 = Dt_Contatti.Rows(i).Item("numero") & ":" & Dt_Contatti.Rows(i).Item("descr")
                                    Case 2
                                        lr_rubrica_3 = Dt_Contatti.Rows(i).Item("numero") & ":" & Dt_Contatti.Rows(i).Item("descr")
                                    Case 3
                                        lr_rubrica_4 = Dt_Contatti.Rows(i).Item("numero") & ":" & Dt_Contatti.Rows(i).Item("descr")
                                    Case 4
                                        lr_rubrica_5 = Dt_Contatti.Rows(i).Item("numero") & ":" & Dt_Contatti.Rows(i).Item("descr")
                                End Select
                            Next
                        End If

                        Dt_Contatti = Nothing

                    End If

                    '--------------------
                    XML_ImpresaPubblico = XmlDocPubblico.CreateElement("Impresa")

                    'scrivo
                    XML_ImpresaPubblico = xpa.Xml_Impresa(XmlDocPubblico,
                                                  CStr("1"),
                                                  Piva,
                                                  Rag_Soc,
                                                  Cuaa,
                                                  Codice_Fiscale,
                                                  Cod_Socio,
                                                  Sup_Totale,
                                                  Piva_Padre,
                                                  Tipo_Gerarchia,
                                                  i_Titolo_Possesso,
                                                  i_Validita_Inizio,
                                                  i_Validita_Fine,
                                                  i_indirizzo,
                                                  i_frazione,
                                                  i_cap,
                                                  i_comune,
                                                  i_provincia,
                                                  i_stato,
                                                  i_note_indirizzo,
                                                  i_codice_istat_comune,
                                                  i_codice_istat_provincia,
                                                  legale_rappresentante,
                                                  lr_codice_fiscale,
                                                  lr_sesso,
                                                  lr_validita_inizio,
                                                  lr_validita_fine,
                                                  lr_indirizzo,
                                                  lr_frazione,
                                                  lr_cap,
                                                  lr_comune,
                                                  lr_provincia,
                                                  lr_stato,
                                                  lr_note_indirizzo,
                                                  lr_codice_istat_comune,
                                                  lr_codice_istat_provincia,
                                                  lr_nascita_data,
                                                  lr_nascita_comune,
                                                  lr_nascita_provincia,
                                                  lr_nascita_istat_comune,
                                                  lr_nascita_istat_provincia,
                                                  lr_documenti,
                                                  lr_rubrica_1,
                                                  lr_rubrica_2,
                                                  lr_rubrica_3,
                                                  lr_rubrica_4,
                                                  lr_rubrica_5,
                                                  CF_tecnico_referente,
                                                  Codice_Cliente,
                                                  Codice_Fornitore,
                                                  Codice_Fornitore_2,
                                                  Codice_Fornitore_3,
                                                  i_Chiave_Cliente,
                                                    Cod_GGN,
                                                    data_iscrizione_libro_soci)

                    XmlDocPubblico.AppendChild(XML_ImpresaPubblico)

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato XML nodo Impresa ")

                    Dt_PartCodici = objPartCodici.Leggi(0,
                                                       Piva,
                                                       0,
                                                       "",
                                                       "",
                                                       "",
                                                       0,
                                                       0,
                                                       "",
                                                       enum_CodiciAnagrafe.CodiceParticella,
                                                       "",
                                                       AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "",
                                                        "",
                                                        objParametri_server)

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "letto Dt_PartCodici ")

                    '----------------------------------
                    '----------------------------------
                    '------------- CENTRO -------------
                    '----------------------------------
                    '----------------------------------
                    If Flag_Crea_Nodo_Centro = 1 Then

                        If Dt_Centri IsNot Nothing AndAlso Dt_Centri.Rows.Count > 0 Then

                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "CENTRO AZIENDALE")

                            For N_Centro = 0 To Dt_Centri.Rows.Count - 1

                                Sa_Cod = Agro_SQL_Load_ConDefault(Dt_Centri.Rows(N_Centro).Item("sa_cod"), GetType(Integer))

                                If Sa_Cod <> 0 Then

                                    Sa_Nome = Dt_Centri.Rows(N_Centro).Item("sa_nome")

                                    Log_G2G.AppendLine(CStr(Date.Now) & "----------- sa_cod: " & Sa_Cod.ToString & " -- " & Sa_Nome & "-----------")

                                    strCentro = objCentro.CentroAziendale_Leggi(CStr(Piva),
                                                                            CInt(Sa_Cod),
                                                                            False,
                                                                            True,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                            objParametri)

                                    If strCentro <> "" Then

                                        XmlDocPrivato.LoadXml(strCentro)

                                        XML_CentroPrivato = XmlDocPrivato.SelectSingleNode("DatiCentriAziendali").SelectSingleNode("CentroAziendale")

                                        c_Titolo_Possesso = XML_CentroPrivato.GetAttribute("titolopossesso")
                                        Sup_Bosco = XML_CentroPrivato.GetAttribute("sup_bosco")
                                        Sup_Prati = XML_CentroPrivato.GetAttribute("sup_prati")
                                        c_Validita_Inizio = XML_CentroPrivato.GetAttribute("validita_inizio")
                                        c_Validita_Fine = XML_CentroPrivato.GetAttribute("validita_fine")

                                        'indirizzo 
                                        XML_CentroIndirizzoPrivato = XML_CentroPrivato.SelectSingleNode("Indirizzo")
                                        c_indirizzo = XML_CentroIndirizzoPrivato.GetAttribute("ind_des")
                                        c_frazione = XML_CentroIndirizzoPrivato.GetAttribute("frz_des")
                                        c_cap = XML_CentroIndirizzoPrivato.GetAttribute("cap")
                                        c_comune = XML_CentroIndirizzoPrivato.GetAttribute("com_des")
                                        c_provincia = XML_CentroIndirizzoPrivato.GetAttribute("pro_cod")
                                        c_stato = XML_CentroIndirizzoPrivato.GetAttribute("stato")
                                        c_codice_istat_comune = XML_CentroIndirizzoPrivato.GetAttribute("com_cod_istat")
                                        c_codice_istat_provincia = XML_CentroIndirizzoPrivato.GetAttribute("pro_cod_istat")
                                        c_note_indirizzo = XML_CentroIndirizzoPrivato.GetAttribute("note")

                                        'CODICI
                                        Tipo_Attivita = ""
                                        Codice_Operatore = ""
                                        c_Chiave_Cliente = ""

                                        XMLs_CentroCodicePrivato = XML_CentroPrivato.GetElementsByTagName("Codice")

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Codici Centro ")
                                        For N_Codice = 0 To XMLs_CentroCodicePrivato.Count - 1

                                            XML_CentroCodicePrivato = XMLs_CentroCodicePrivato(N_Codice)
                                            Codice = XML_CentroCodicePrivato.GetAttribute("id_cod")
                                            Valore = XML_CentroCodicePrivato.GetAttribute("val_cod")

                                            Select Case Codice
                                                Case enum_CodiciAnagrafe.TipoAttivita
                                                    Tipo_Attivita = Valore
                                                Case enum_CodiciAnagrafe.CodiceCentro_Attuale
                                                    Codice_Operatore = Valore
                                                Case 2000 To 2999
                                                    c_Chiave_Cliente = Valore
                                            End Select

                                        Next


                                        '----- Rubrica
                                        c_rubrica_1 = ""
                                        c_rubrica_2 = ""
                                        c_rubrica_3 = ""
                                        c_rubrica_4 = ""
                                        c_rubrica_5 = ""
                                        c_rubrica_6 = ""
                                        c_rubrica_7 = ""
                                        c_rubrica_8 = ""
                                        c_rubrica_9 = ""

                                        XMLs_CentroRubricaPrivato = XML_CentroPrivato.GetElementsByTagName("Rubrica")

                                        For N_Rubrica = 0 To XMLs_CentroRubricaPrivato.Count - 1

                                            XML_CentroRubricaPrivato = XMLs_CentroRubricaPrivato(N_Rubrica)

                                            Numero = XML_CentroRubricaPrivato.GetAttribute("numero")
                                            Descr = XML_CentroRubricaPrivato.GetAttribute("descr")

                                            Select Case N_Rubrica
                                                Case 0
                                                    c_rubrica_1 = Numero & ":" & Descr
                                                Case 1
                                                    c_rubrica_2 = Numero & ":" & Descr
                                                Case 2
                                                    c_rubrica_3 = Numero & ":" & Descr
                                                Case 3
                                                    c_rubrica_4 = Numero & ":" & Descr
                                                Case 4
                                                    c_rubrica_5 = Numero & ":" & Descr
                                                Case 5
                                                    c_rubrica_6 = Numero & ":" & Descr
                                                Case 6
                                                    c_rubrica_7 = Numero & ":" & Descr
                                                Case 7
                                                    c_rubrica_8 = Numero & ":" & Descr
                                                Case 8
                                                    c_rubrica_9 = Numero & ":" & Descr
                                            End Select

                                        Next

                                        XML_CentroPubblico = xpa.Xml_CentroAziendale(XmlDocPubblico,
                                                                     CStr("1"),
                                                                     CStr(Sa_Cod),
                                                                     CStr(Sa_Nome),
                                                                     c_Titolo_Possesso,
                                                                     Sup_Bosco,
                                                                     Sup_Prati,
                                                                     Codice_Operatore,
                                                                     Tipo_Attivita,
                                                                     c_Validita_Inizio,
                                                                     c_Validita_Fine,
                                                                     c_indirizzo,
                                                                     c_frazione,
                                                                     c_cap,
                                                                     c_comune,
                                                                     c_provincia,
                                                                     c_stato,
                                                                     c_note_indirizzo,
                                                                     c_codice_istat_comune,
                                                                     c_codice_istat_provincia,
                                                                     c_rubrica_1,
                                                                     c_rubrica_2,
                                                                     c_rubrica_3,
                                                                     c_rubrica_4,
                                                                     c_rubrica_5,
                                                                     c_rubrica_6,
                                                                     c_rubrica_7,
                                                                     c_rubrica_8,
                                                                     c_rubrica_9,
                                                                     c_Chiave_Cliente)

                                        If Flag_GIS Then
                                            XML_CentroPubblico.SetAttribute("gis", Dt_Centri.Rows(N_Centro).Item("Gis_wkt"))
                                        End If

                                        XML_ImpresaPubblico.AppendChild(XML_CentroPubblico)

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato XML nodo Centro ")

                                        '----------------------------------
                                        '----------------------------------
                                        '----------- PARTICELLE -----------
                                        '----------------------------------
                                        '----------------------------------

                                        If Flag_Crea_Nodo_Particella = 1 Then

                                            Dim DtParticelle As DataTable

                                            'ATTENZIONE
                                            'essendo in select list il titolo_possesso...
                                            'potrebbero esserci duplicati...
                                            DtParticelle = objParticella.Leggi(0,
                                                                   CStr(Piva),
                                                                   CInt(Sa_Cod),
                                                                   CInt(0),
                                                                   CStr(""),
                                                                   CStr(""),
                                                                   CStr(""),
                                                                   CInt(0),
                                                                   CInt(0),
                                                                   CStr(""),
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", "",
                                                                   objParametri)

                                            If DtParticelle IsNot Nothing AndAlso DtParticelle.Rows.Count > 0 Then

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "PARTICELLE ")

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "num. particelle: " & DtParticelle.Rows.Count.ToString)

                                                For N_Particella = 0 To DtParticelle.Rows.Count - 1

                                                    Part_Cod = DtParticelle.Rows(N_Particella).Item("Part_Cod")
                                                    p_codice_istat_provincia = DtParticelle.Rows(N_Particella).Item("prov")
                                                    p_codice_istat_comune = DtParticelle.Rows(N_Particella).Item("com")
                                                    p_Sezione = DtParticelle.Rows(N_Particella).Item("sezione")
                                                    p_Foglio = DtParticelle.Rows(N_Particella).Item("foglio")
                                                    p_Numero = DtParticelle.Rows(N_Particella).Item("numero")
                                                    p_Subalterno = DtParticelle.Rows(N_Particella).Item("subalterno")

                                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "particella: " & p_codice_istat_provincia & " - " & p_codice_istat_comune & " - " & p_Sezione & " - " & p_Foglio & " - " & p_Numero & " - " & p_Subalterno)

                                                    Partita_Catastale = DtParticelle.Rows(N_Particella).Item("Partita_Catastale")
                                                    p_Ettari = DtParticelle.Rows(N_Particella).Item("ettari")
                                                    p_Are = DtParticelle.Rows(N_Particella).Item("are")
                                                    p_Centiare = DtParticelle.Rows(N_Particella).Item("centiare")
                                                    Qualita_Catasto_Codice = DtParticelle.Rows(N_Particella).Item("Qualita_Cod")
                                                    Classe = DtParticelle.Rows(N_Particella).Item("Classe")
                                                    Reddito_Dominicale = DtParticelle.Rows(N_Particella).Item("Reddito_Dominicale")
                                                    Reddito_Agrario = DtParticelle.Rows(N_Particella).Item("Reddito_Agrario")

                                                    p_Titolo_Possesso = DtParticelle.Rows(N_Particella).Item("xTitoloPossesso")
                                                    p_Titolo_Possesso_Des = AgronicaCoreDataProvider.UtilityProvider.TitoloPossessoDes_from_TitoloPossessoCod(p_Titolo_Possesso)
                                                    p_Validita_Inizio_Possesso = DtParticelle.Rows(N_Particella).Item("xValidita_Inizio")
                                                    p_Validita_Fine_Possesso = DtParticelle.Rows(N_Particella).Item("xValidita_Fine")
                                                    p_Sup_Condotta = DtParticelle.Rows(N_Particella).Item("sup_condotta")

                                                    Cod_Particella_Azi = objPartCodici.RecuperaValCod_from_DTeCod(Dt_PartCodici,
                                                                                                              Piva,
                                                                                                               Sa_Cod,
                                                                                                               p_codice_istat_provincia,
                                                                                                               p_codice_istat_comune,
                                                                                                               p_Sezione,
                                                                                                               p_Foglio,
                                                                                                               p_Numero,
                                                                                                               p_Subalterno,
                                                                                                              enum_CodiciAnagrafe.CodiceParticella)

                                                    XML_ParticellaPubblico = xpa.Xml_Particella(XmlDocPubblico,
                                                                                CStr("1"),
                                                                                CStr(Part_Cod),
                                                                                Cod_Particella_Azi,
                                                                                CStr(p_codice_istat_comune),
                                                                                CStr(p_codice_istat_provincia),
                                                                                CStr(p_Sezione),
                                                                                CStr(p_Foglio),
                                                                                CStr(p_Numero),
                                                                                CStr(p_Subalterno),
                                                                                CStr(Partita_Catastale),
                                                                                CStr(p_Ettari),
                                                                                CStr(p_Are),
                                                                                CStr(p_Centiare),
                                                                                CStr(p_Titolo_Possesso),
                                                                                CStr(p_Titolo_Possesso_Des),
                                                                                CStr(p_Sup_Condotta),
                                                                                CStr(Qualita_Catasto_Codice),
                                                                                CStr(Classe),
                                                                                CStr(Reddito_Dominicale),
                                                                                CStr(Reddito_Agrario),
                                                                                CStr(p_Validita_Inizio_Possesso),
                                                                                CStr(p_Validita_Fine_Possesso))

                                                    XML_CentroPubblico.AppendChild(XML_ParticellaPubblico)

                                                Next

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Particelle ")
                                            Else
                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono state trovate particelle.")
                                            End If
                                        Else
                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Particelle non attiva")
                                        End If

                                        '----------------------------------
                                        '----------------------------------
                                        '-------------- CAMPO -------------
                                        '----------------------------------
                                        '----------------------------------

                                        If Flag_Crea_Nodo_Campo = 1 Then

                                            If Dt_Campi IsNot Nothing AndAlso Dt_Campi.Rows.Count > 0 Then

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "CAMPO ")

                                                Riga_Dati_Campi = Dt_Campi.Select("sa_cod=" & Sa_Cod)

                                                For N_Campo = 0 To Riga_Dati_Campi.Length - 1

                                                    Campo_Cod = Riga_Dati_Campi(N_Campo).Item("campo_cod")

                                                    If Campo_Cod = 130023440 Then
                                                        Dim debug As Boolean = True
                                                    End If

                                                    strCampo = objCampo.Campo_Leggi(CStr(Piva),
                                                                                CInt(Sa_Cod),
                                                                                CInt(Campo_Cod),
                                                                                AGRODATAINIZIO,
                                                                                AGRODATAFINE,
                                                                                False,
                                                                                False,
                                                                                objParametri)

                                                    If strCampo <> "" Then

                                                        XmlDocPrivato.LoadXml(strCampo)

                                                        XML_CampoPrivato = XmlDocPrivato.SelectSingleNode("DatiCampi").SelectSingleNode("Campo")

                                                        Campo_Des = XML_CampoPrivato.GetAttribute("campo_des")

                                                        Log_G2G.AppendLine(CStr(Date.Now) & " -----------" & "campo_cod: " & Campo_Cod.ToString & " -- " & Campo_Des & " -----------")

                                                        Campo_Tipo_Cod = XML_CampoPrivato.GetAttribute("campo_tipo")
                                                        cam_Gru_Cod = XML_CampoPrivato.GetAttribute("gru_cod")
                                                        cam_Veg_Cod = XML_CampoPrivato.GetAttribute("veg_cod")
                                                        cam_Validita_Inizio = XML_CampoPrivato.GetAttribute("validita_inizio")
                                                        cam_Validita_Fine = XML_CampoPrivato.GetAttribute("validita_fine")

                                                        If Campo_Tipo_Cod = 0 Then
                                                            Campo_Tipo_Des = "campo"
                                                        Else
                                                            Campo_Tipo_Des = "serra"
                                                        End If

                                                        'CODICI
                                                        cam_Chiave_Cliente = ""

                                                        XMLs_CampoCodicePrivato = XML_CampoPrivato.GetElementsByTagName("Codice")

                                                        For N_Codice = 0 To XMLs_CampoCodicePrivato.Count - 1

                                                            XML_CampoCodicePrivato = XMLs_CampoCodicePrivato(N_Codice)
                                                            Codice = XML_CampoCodicePrivato.GetAttribute("id_cod")
                                                            Valore = XML_CampoCodicePrivato.GetAttribute("val_cod")

                                                            Select Case Codice
                                                                Case 2000 To 2999
                                                                    cam_Chiave_Cliente = Valore
                                                            End Select

                                                        Next

                                                        XML_CampoPubblico = xpa.Xml_Campo(XmlDocPubblico,
                                                                      CStr("1"),
                                                                      CStr(Campo_Cod),
                                                                      CStr(Campo_Tipo_Cod),
                                                                      CStr(Campo_Tipo_Des),
                                                                      CStr(Campo_Des),
                                                                      CStr(cam_Gru_Cod),
                                                                      CStr(cam_Veg_Cod),
                                                                      cam_Validita_Inizio,
                                                                      cam_Validita_Fine,
                                                                      cam_Chiave_Cliente)

                                                        XML_CentroPubblico.AppendChild(XML_CampoPubblico)

                                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato XML nodo Campo ")


                                                        'PARTICELLE
                                                        If Flag_Crea_Nodo_CampoParticella = 1 Then

                                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "CAMPO X PARTICELLA ")

                                                            XML_CampoDatiParticellePrivato = XML_CampoPrivato.SelectSingleNode("DatiParticelle")

                                                            If XML_CampoDatiParticellePrivato IsNot Nothing Then

                                                                XMLs_CampoParticellaPrivato = XML_CampoDatiParticellePrivato.GetElementsByTagName("Particella")

                                                                For N_Particella = 0 To XMLs_CampoParticellaPrivato.Count - 1

                                                                    XML_CampoParticellaPrivato = XMLs_CampoParticellaPrivato(N_Particella)

                                                                    c_p_codice_istat_comune = XML_CampoParticellaPrivato.GetAttribute("com")
                                                                    c_p_codice_istat_provincia = XML_CampoParticellaPrivato.GetAttribute("prov")
                                                                    c_p_Sezione = XML_CampoParticellaPrivato.GetAttribute("sezione")
                                                                    c_p_Foglio = XML_CampoParticellaPrivato.GetAttribute("foglio")
                                                                    c_p_Numero = XML_CampoParticellaPrivato.GetAttribute("numero")
                                                                    c_p_Subalterno = XML_CampoParticellaPrivato.GetAttribute("subalterno")
                                                                    c_p_Sup = XML_CampoParticellaPrivato.GetAttribute("area")
                                                                    c_p_Validita_Inizio = XML_CampoParticellaPrivato.GetAttribute("validita_inizio")
                                                                    c_p_Validita_Fine = XML_CampoParticellaPrivato.GetAttribute("validita_fine")

                                                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "particella: " & c_p_codice_istat_provincia & " - " & c_p_codice_istat_comune & " - " & c_p_Sezione & " - " & c_p_Foglio & " - " & c_p_Numero & " - " & c_p_Subalterno)


                                                                    XML_CampoParticellaPubblico = xpa.Xml_Campo_Particella(XmlDocPubblico,
                                                                                                                CStr("1"),
                                                                                                                c_p_codice_istat_comune,
                                                                                                                c_p_codice_istat_provincia,
                                                                                                                c_p_Sezione,
                                                                                                                c_p_Foglio,
                                                                                                                c_p_Numero,
                                                                                                                c_p_Subalterno,
                                                                                                                c_p_Sup)

                                                                    XML_CampoPubblico.AppendChild(XML_CampoParticellaPubblico)

                                                                Next

                                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Finito campi x particelle ")

                                                            End If
                                                        Else
                                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione CampiXParticelle non attiva")
                                                        End If 'campi x part



                                                        '----------------------------------
                                                        '----------------------------------
                                                        '---------- APPEZZAMENTO ----------
                                                        '----------------------------------
                                                        '----------------------------------

                                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Elaborazione appezzamenti del campo ")

                                                        Crea_XML_AppezzaImpiantoProgetto(AnnataAgrariariferimanto_da,
                                                                                            AnnataAgrariariferimanto_a,
                                                                                            Piva,
                                                                                            Sa_Cod,
                                                                                            Campo_Cod,
                                                                                            Dt_Appezzamenti,
                                                                                            Dt_Impianti,
                                                                                            Flag_GestioneCodifiche,
                                                                                            Dt_Codifica_Cultivar,
                                                                                            Dt_Codifica_FormeAllevamento,
                                                                                            Dt_Codifica_Portinnesti,
                                                                                            codice_GIAS,
                                                                                            Flag_GIS,
                                                                                            Log_G2G,
                                                                                            XmlDocPrivato,
                                                                                            XmlDocPubblico,
                                                                                                XML_CampoPubblico,
                                                                                            XML_CentroPubblico,
                                                                                            xpa,
                                                                                            objParametri_server
                                                                                            )



                                                    End If

                                                Next

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Campi ")
                                            Else
                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati campi")
                                            End If
                                        Else
                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Campi non attiva")
                                        End If


                                        '----------------------------------
                                        '----------------------------------
                                        '------ APPEZZAMENTO SFUSO --------
                                        '----------------------------------
                                        '----------------------------------

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "elaborazione appezzamenti sfusi ")

                                        Campo_Cod = 0

                                        Crea_XML_AppezzaImpiantoProgetto(AnnataAgrariariferimanto_da,
                                                                        AnnataAgrariariferimanto_a,
                                                                        Piva,
                                                                        Sa_Cod,
                                                                        0,
                                                                        Dt_Appezzamenti,
                                                                        Dt_Impianti,
                                                                        Flag_GestioneCodifiche,
                                                                        Dt_Codifica_Cultivar,
                                                                        Dt_Codifica_FormeAllevamento,
                                                                        Dt_Codifica_Portinnesti,
                                                                        codice_GIAS,
                                                                        Flag_GIS,
                                                                        Log_G2G,
                                                                        XmlDocPrivato,
                                                                        XmlDocPubblico,
                                                                        XML_CampoPubblico,
                                                                        XML_CentroPubblico,
                                                                        xpa,
                                                                        objParametri_server
                                                                        )

                                        'If IsNothing(XML_AppezzamentoPubblico) Then
                                        '    Throw New Exception("Non è stato possibile creare l'elemento Appezzamento del " & Sa_Nome & " (" & Rag_Soc & " - " & Piva & ")")
                                        'End If

                                        'XML_CentroPubblico.AppendChild(XML_AppezzamentoPubblico)


                                    End If

                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine sa_cod: " & Sa_Cod.ToString & " -- " & Sa_Nome)

                                Else
                                    'la query del filtrone mette in left i centri, per cui il dt ha record anceh se non ci sono centri!
                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati centri aziendali")
                                End If
                            Next 'centri
                        Else
                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati centri aziendali")
                        End If
                    Else
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Centri non attiva")
                    End If ' centro

                End If
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Imprese non attiva")
            End If

            strRisultato = XmlDocPubblico.OuterXml


        Catch ex As Exception
            Throw New Exception(CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message)
        End Try

        Return XmlDocPubblico

    End Function


    Private Sub Crea_XML_AppezzaImpiantoProgetto(
        ByVal AnnataAgrariariferimanto_da As Date,
        ByVal AnnataAgrariariferimanto_a As Date,
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Campo_Cod As Integer,
        ByVal Dt_Appezzamenti As DataTable,
        ByVal Dt_Impianti As DataTable,
        ByVal Flag_GestioneCodifiche As Boolean,
        ByVal Dt_Codifica_Cultivar As DataTable,
        ByVal Dt_Codifica_FormeAllevamento As DataTable,
        ByVal Dt_Codifica_Portinnesti As DataTable,
        ByVal codice_GIAS As Integer,
        ByVal Flag_GIS As Boolean,
        ByRef Log_G2G As StringBuilder,
        ByRef XmlDocPrivato As XmlDocument,
        ByRef XmlDocPubblico As XmlDocument,
        ByRef XML_CampoPubblico As XmlElement,
        ByRef XML_CentroPubblico As XmlElement,
        ByRef xpa As AgronicaCoreXML.XML_Pubblico_Anagrafe,
        ByRef objParametri As AgronicaCoreParametri
    )

        Const nome_funzione As String = "Crea_XML_AppezzaImpiantoProgetto"

        Dim Riga_Dati_Appezzamenti As DataRow()
        Dim Riga_Dati_Impianti As DataRow()
        Dim Appezza As Integer
        Dim App_Nome As String
        Dim Id_Reg As Integer
        Dim HashImp As Hashtable
        Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
        Dim objProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_R
        Dim objProgCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim objCACInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
        Dim objRegol As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

        Dim XML_AppezzamentoPrivato As XmlElement
        Dim XML_AppezzamentoDatiParticellePrivato As XmlElement
        Dim XML_AppezzamentoParticellaPrivato As XmlElement
        Dim XMLs_AppezzamentoParticellaPrivato As XmlNodeList
        Dim XML_AppezzamentoDatiCodicePrivato As XmlElement
        Dim XML_AppezzamentoCodicePrivato As XmlElement
        Dim XMLs_AppezzamentoCodicePrivato As XmlNodeList
        Dim XML_ImpiantoPrivato As XmlElement
        Dim XML_ImpiantoDatiCodicePrivato As XmlElement
        Dim XML_ImpiantoCodicePrivato As XmlElement
        Dim XMLs_ImpiantoCodicePrivato As XmlNodeList
        Dim XML_ProgettoPrivato As XmlElement
        Dim XMLs_ProgettoPrivato As XmlNodeList
        Dim XML_ProgettoDatiCodicePrivato As XmlElement
        Dim XML_ProgettoCodicePrivato As XmlElement
        Dim XMLs_ProgettoCodicePrivato As XmlNodeList
        Dim XML_AppezzamentoPubblico As XmlElement = Nothing
        Dim XML_AppezzamentoParticellaPubblico As XmlElement
        Dim XML_ImpiantoPubblico As XmlElement
        Dim XML_ProgettoPubblico As XmlElement
        Dim strAppezzamento As String
        Dim strImpianto As String
        Dim strDatiProgetto As String
        Dim Codice As String
        Dim Valore As String
        Dim valore_2 As String
        Dim Numero_Appezzamento_Bio As String
        Dim Riferimento_Alfanumerico_Appezzamento As String
        Dim Sup_App As String
        Dim Altitudine As String
        Dim a_Titolo_Possesso As String
        Dim Metodo_Produzione_Cod As String
        Dim Metodo_Produzione_Des As String
        Dim Codice_Contratto As String
        Dim a_Validita_Inizio As String
        Dim a_Validita_Fine As String

        Dim a_p_codice_istat_comune As String
        Dim a_p_codice_istat_provincia As String
        Dim a_p_Sezione As String
        Dim a_p_Foglio As String
        Dim a_p_Numero As String
        Dim a_p_Subalterno As String
        Dim a_p_Sup As String
        Dim a_p_Validita_Inizio As String
        Dim a_p_Validita_Fine As String
        Dim a_Chiave_Cliente As String
        Dim coltura_Precedente_1 As String
        Dim coltura_Precedente_2 As String
        Dim coltura_Precedente_3 As String
        Dim coltura_Precedente_4 As String

        Dim Gru_Cod As String
        Dim Cod_Specie_Gias As String
        Dim Cod_Finalita_Gias As String
        Dim Cod_Varieta_Gias As String
        Dim Cod_TipologiaVarietale_Gias As String
        Dim Cod_Dettaglio_Specie_Personalizzato As String
        Dim Cod_Specie_Cliente As String
        Dim Cod_Finalita_Cliente As String
        Dim Cod_Varieta_Cliente As String
        Dim Cod_Portinnesto_Cliente As String
        Dim Cod_Forma_Allevamento_Cliente As String
        Dim Cod_Copertura_Cliente As String
        Dim Cod_Impianto_Irriguo_Cliente As String
        Dim Cod_Impianto As String
        Dim Scarto As String
        Dim Portinnesto_Cod As String
        Dim Imp_Irrigazione_Cod As String
        Dim Forma_Allevamento_Cod As String
        Dim Copertura_Cod As String
        Dim Semina_Trapianto As String
        Dim Tra_Fila As String
        Dim Su_Fila As String
        Dim Interbina As String
        Dim Germinabilita As String
        Dim Piante_Ha As String
        Dim Sup_Imp As String
        Dim imp_Validita_Inizio As String
        Dim imp_Validita_Fine As String
        Dim imp_Chiave_Cliente As String

        Dim Progetto_Cod As Integer
        Dim Progetto_Des As String
        Dim Progetto_Nome As String
        Dim prg_Resa_Prevista As String
        Dim prg_Regolamento_Cod As String
        Dim prg_Regolamento_Des As String
        Dim prg_Disciplinare_Cod As String
        Dim prg_Piano_Semina As String
        Dim prg_Org_Referente_Cod As String
        Dim prg_Org_Referente_Des As String
        Dim prg_Capitolato_Privato_Cod As String
        Dim prg_Capitolato_Privato_Des As String
        Dim prg_Stato_Impianto As String
        Dim prg_Validita_Inizio As String
        Dim prg_Validita_Fine As String
        Dim prg_data_modifica As String
        Dim prg_username_modifica As String
        Dim prg_utente_modifica As String
        Dim prg_Specie_Esercizio_Codice As String
        Dim prg_Specie_Esercizio_Descr As String
        Dim prg_NumOPCollegamento As String
        Dim prg_Data_Chiusura_Esercizio As String

        Dim N_Appezzamento As Integer
        Dim N_Impianto As Integer
        Dim N_Progetto As Integer

        Try

            '----------------------------------
            '----------------------------------
            '------ APPEZZAMENTO SFUSO --------
            '----------------------------------
            '----------------------------------

            If Flag_Crea_Nodo_Appezzamento = 1 Then

                If Dt_Appezzamenti IsNot Nothing AndAlso Dt_Appezzamenti.Rows.Count > 0 Then

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "APPEZZAMENTO ")

                    If Campo_Cod = 0 Then
                        '----------------------------------------
                        '---------- NO GESTIONE CAMPO -----------
                        '----------------------------------------
                        Riga_Dati_Appezzamenti = Dt_Appezzamenti.Select("sa_cod=" & Sa_Cod & " AND campo_cod=0 ")
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "eseguito filtro del centro ")
                    Else
                        '----------------------------------------
                        '---------- SI GESTIONE CAMPO -----------
                        '----------------------------------------
                        Riga_Dati_Appezzamenti = Dt_Appezzamenti.Select("sa_cod=" & Sa_Cod & " AND campo_cod= " & Campo_Cod)
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "eseguito filtro del centro e del campo ")
                    End If

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "num. appezzamenti: " & Riga_Dati_Appezzamenti.Length.ToString)

                    For N_Appezzamento = 0 To Riga_Dati_Appezzamenti.Length - 1

                        Appezza = Riga_Dati_Appezzamenti(N_Appezzamento).Item("appezza")
                        App_Nome = Riga_Dati_Appezzamenti(N_Appezzamento).Item("app_nome")

                        If Appezza = 130023705 Then
                            Dim debug As Boolean = True
                        End If


                        Log_G2G.AppendLine(CStr(Date.Now) & " ----------- appezza: " & Appezza.ToString & " -- " & App_Nome & " -----------")

                        strAppezzamento = objAppezzamento.Appezzamento_Leggi(CStr(Piva),
                                                                            CInt(Sa_Cod),
                                                                            CInt(Campo_Cod),
                                                                            CInt(Appezza),
                                                                            False,
                                                                            True,
                                                                            False,
                                                                            False,
                                                                            objParametri,
                                                                            LeggiCodiciAnagrafe_ConDescrizioni:=True)

                        If strAppezzamento <> "" Then

                            XmlDocPrivato.LoadXml(strAppezzamento)

                            XML_AppezzamentoPrivato = XmlDocPrivato.SelectSingleNode("DatiAppezzamenti").SelectSingleNode("Appezzamento")

                            Sup_App = XML_AppezzamentoPrivato.GetAttribute("sup_app")

                            Altitudine = "0"

                            If Not IsDBNull(XML_AppezzamentoPrivato.GetAttribute("pende")) AndAlso
                               XML_AppezzamentoPrivato.GetAttribute("pende") <> "" Then
                                Altitudine = XML_AppezzamentoPrivato.GetAttribute("pende")
                            End If

                            a_Validita_Inizio = XML_AppezzamentoPrivato.GetAttribute("validita_inizio")
                            a_Validita_Fine = XML_AppezzamentoPrivato.GetAttribute("validita_fine")

                            'CODICI
                            a_Titolo_Possesso = "1"
                            Metodo_Produzione_Cod = "1"
                            Metodo_Produzione_Des = "Convenzionale"
                            Codice_Contratto = ""
                            Numero_Appezzamento_Bio = ""
                            Riferimento_Alfanumerico_Appezzamento = ""
                            a_Chiave_Cliente = ""

                            coltura_Precedente_1 = ""
                            coltura_Precedente_2 = ""
                            coltura_Precedente_3 = ""
                            coltura_Precedente_4 = ""


                            'la lettura delle colture precedenti avviene in due fasi.
                            '1. quanto viene letto attraverso le distinte di appezzamenti collegati
                            objAppezzamento.LetturaPrecessioniDaElementiCollegati(
                                    Piva, Sa_Cod, Appezza,
                                    coltura_Precedente_1, coltura_Precedente_2, coltura_Precedente_3, coltura_Precedente_4,
                                    objParametri_server,
                                    objParametri_utenti
                                )

                            '2. in sub-iudice quanto letto dai codici anagrafe.

                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Codici Appezzamento ")
                            XML_AppezzamentoDatiCodicePrivato = XML_AppezzamentoPrivato.SelectSingleNode("DatiCodici")

                            If XML_AppezzamentoDatiCodicePrivato IsNot Nothing Then

                                XMLs_AppezzamentoCodicePrivato = XML_AppezzamentoDatiCodicePrivato.GetElementsByTagName("CodiceAppezzamento")

                                For N_Codice = 0 To XMLs_AppezzamentoCodicePrivato.Count - 1

                                    XML_AppezzamentoCodicePrivato = XMLs_AppezzamentoCodicePrivato(N_Codice)
                                    Codice = XML_AppezzamentoCodicePrivato.GetAttribute("id_cod")
                                    Valore = XML_AppezzamentoCodicePrivato.GetAttribute("val_cod")
                                    valore_2 = XML_AppezzamentoCodicePrivato.GetAttribute("val_cod_2")

                                    Select Case Codice
                                        Case enum_CodiciAnagrafe.TitoloPossesso
                                            a_Titolo_Possesso = Valore
                                        Case enum_CodiciAnagrafe.MetodoDiProduzione
                                            Metodo_Produzione_Cod = Valore
                                            Metodo_Produzione_Des = AgronicaCoreDataProvider.UtilityProvider.MetodoProduzioneDes_from_Cod(Metodo_Produzione_Cod)
                                        Case enum_CodiciAnagrafe.Appezzamento_CodiceContratto
                                            Codice_Contratto = Valore
                                        Case enum_CodiciAnagrafe.Codice_Appezza_Biologico
                                            Numero_Appezzamento_Bio = Valore
                                        Case enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento
                                            Riferimento_Alfanumerico_Appezzamento = Valore
                                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1
                                            If coltura_Precedente_1 = "" Then
                                                coltura_Precedente_1 = valore_2
                                            End If
                                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2
                                            If coltura_Precedente_2 = "" Then
                                                coltura_Precedente_2 = valore_2
                                            End If
                                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3
                                            If coltura_Precedente_3 = "" Then
                                                coltura_Precedente_3 = valore_2
                                            End If
                                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4
                                            If coltura_Precedente_4 = "" Then
                                                coltura_Precedente_4 = valore_2
                                            End If
                                        Case 2000 To 2999
                                            a_Chiave_Cliente = Valore
                                    End Select

                                Next

                            End If

                            XML_AppezzamentoPubblico = xpa.Xml_Appezzamento(XmlDocPubblico,
                                                                            CStr("1"),
                                                                            CStr(Appezza),
                                                                            CStr(App_Nome),
                                                                            CStr(Numero_Appezzamento_Bio),
                                                                            Sup_App,
                                                                            Altitudine,
                                                                            a_Titolo_Possesso,
                                                                            Metodo_Produzione_Cod,
                                                                            Metodo_Produzione_Des,
                                                                            Codice_Contratto,
                                                                            a_Validita_Inizio,
                                                                            a_Validita_Fine,
                                                                            a_Chiave_Cliente,
                                                                            Riferimento_Alfanumerico_Appezzamento)

                            If Flag_GIS Then
                                XML_AppezzamentoPubblico.SetAttribute("gis", Riga_Dati_Appezzamenti(N_Appezzamento).Item("Gis_wkt"))
                            End If

                            XML_AppezzamentoPubblico.SetAttribute("coltura_precedente_1", coltura_Precedente_1)
                            XML_AppezzamentoPubblico.SetAttribute("coltura_precedente_2", coltura_Precedente_2)
                            XML_AppezzamentoPubblico.SetAttribute("coltura_precedente_3", coltura_Precedente_3)
                            XML_AppezzamentoPubblico.SetAttribute("coltura_precedente_4", coltura_Precedente_4)


                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato XML nodo Appezzamento ")

                            'PARTICELLE

                            If Flag_Crea_Nodo_AppezzamentoParticella = 1 Then

                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "APPEZZAMENTO X PARTICELLA ")

                                XML_AppezzamentoDatiParticellePrivato = XML_AppezzamentoPrivato.SelectSingleNode("DatiParticelle")

                                If XML_AppezzamentoDatiParticellePrivato IsNot Nothing Then

                                    XMLs_AppezzamentoParticellaPrivato = XML_AppezzamentoDatiParticellePrivato.GetElementsByTagName("Particella")

                                    For N_Particella = 0 To XMLs_AppezzamentoParticellaPrivato.Count - 1

                                        XML_AppezzamentoParticellaPrivato = XMLs_AppezzamentoParticellaPrivato(N_Particella)

                                        a_p_codice_istat_comune = XML_AppezzamentoParticellaPrivato.GetAttribute("com")
                                        a_p_codice_istat_provincia = XML_AppezzamentoParticellaPrivato.GetAttribute("prov")
                                        a_p_Sezione = XML_AppezzamentoParticellaPrivato.GetAttribute("sezione")
                                        a_p_Foglio = XML_AppezzamentoParticellaPrivato.GetAttribute("foglio")
                                        a_p_Numero = XML_AppezzamentoParticellaPrivato.GetAttribute("numero")
                                        a_p_Subalterno = XML_AppezzamentoParticellaPrivato.GetAttribute("subalterno")
                                        a_p_Sup = XML_AppezzamentoParticellaPrivato.GetAttribute("area")
                                        a_p_Validita_Inizio = XML_AppezzamentoParticellaPrivato.GetAttribute("validita_inizio")
                                        a_p_Validita_Fine = XML_AppezzamentoParticellaPrivato.GetAttribute("validita_fine")

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "particella:  " & a_p_codice_istat_provincia & " - " & a_p_codice_istat_comune & " - " & a_p_Sezione & " - " & a_p_Foglio & " - " & a_p_Numero & " - " & a_p_Subalterno)


                                        XML_AppezzamentoParticellaPubblico = xpa.Xml_Appezzamento_Particella(XmlDocPubblico,
                                                                                                                CStr("1"),
                                                                                                                a_p_codice_istat_comune,
                                                                                                                a_p_codice_istat_provincia,
                                                                                                                a_p_Sezione,
                                                                                                                a_p_Foglio,
                                                                                                                a_p_Numero,
                                                                                                                a_p_Subalterno,
                                                                                                                a_p_Sup,
                                                                                                                a_p_Validita_Inizio,
                                                                                                                a_p_Validita_Fine)

                                        XML_AppezzamentoPubblico.AppendChild(XML_AppezzamentoParticellaPubblico)

                                    Next

                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Finito appezza x particelle ")

                                End If
                            Else
                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione AppezzamentiXParticelle non attiva")
                            End If

                            HashImp = New Hashtable

                            '----------------------------------
                            '----------------------------------
                            '------------ IMPIANTO ------------
                            '----------------------------------
                            '----------------------------------

                            If Flag_Crea_Nodo_Impianto = 1 Then

                                If Dt_Impianti IsNot Nothing AndAlso Dt_Impianti.Rows.Count > 0 Then

                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "IMPIANTO ")

                                    Riga_Dati_Impianti = Dt_Impianti.Select("sa_cod=" & Sa_Cod & " AND appezza=" & Appezza)

                                    For N_Impianto = 0 To Riga_Dati_Impianti.Length - 1

                                        Id_Reg = Riga_Dati_Impianti(N_Impianto).Item("id_reg")

                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "impianto: " & Piva & " - " & Sa_Cod.ToString & " - " & Appezza.ToString & " - " & Id_Reg.ToString)

                                        If Not HashImp.ContainsKey(Id_Reg) Then

                                            HashImp.Add(Id_Reg, "")

                                            strImpianto = objImpianto.Reg_Impianto_Leggi(CStr(Piva),
                                                                                        CInt(Sa_Cod),
                                                                                        CInt(Appezza),
                                                                                        CInt(Id_Reg),
                                                                                        False,
                                                                                        True,
                                                                                        objParametri)

                                            If strImpianto <> "" Then

                                                XmlDocPrivato.LoadXml(strImpianto)

                                                XML_ImpiantoPrivato = XmlDocPrivato.SelectSingleNode("DatiReg_Impianti").SelectSingleNode("Reg_Impianto")

                                                imp_Chiave_Cliente = ""
                                                Sup_Imp = XML_ImpiantoPrivato.GetAttribute("sup_imp")
                                                Cod_Varieta_Gias = XML_ImpiantoPrivato.GetAttribute("cul_cod").Trim
                                                Cod_Finalita_Gias = XML_ImpiantoPrivato.GetAttribute("grfi_cod").Trim
                                                Cod_TipologiaVarietale_Gias = XML_ImpiantoPrivato.GetAttribute("grva_cod_veg").Trim
                                                imp_Validita_Inizio = XML_ImpiantoPrivato.GetAttribute("validita_inizio")
                                                imp_Validita_Fine = XML_ImpiantoPrivato.GetAttribute("validita_fine")
                                                Scarto = XML_ImpiantoPrivato.GetAttribute("scarto")
                                                Semina_Trapianto = XML_ImpiantoPrivato.GetAttribute("setup_cod")
                                                Portinnesto_Cod = XML_ImpiantoPrivato.GetAttribute("port_cod")
                                                Forma_Allevamento_Cod = XML_ImpiantoPrivato.GetAttribute("foral_cod")
                                                Imp_Irrigazione_Cod = XML_ImpiantoPrivato.GetAttribute("imp_cod")
                                                Copertura_Cod = XML_ImpiantoPrivato.GetAttribute("cop_cod")
                                                Gru_Cod = XML_ImpiantoPrivato.GetAttribute("gru_cod")

                                                'CODICI
                                                Tra_Fila = "0"
                                                Su_Fila = "0"
                                                Interbina = "0"
                                                Germinabilita = "0"
                                                Cod_Dettaglio_Specie_Personalizzato = ""
                                                a_Chiave_Cliente = ""
                                                Cod_Impianto = ""

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Impianti codici ")
                                                XML_ImpiantoDatiCodicePrivato = XML_ImpiantoPrivato.SelectSingleNode("DatiCodici")

                                                If XML_ImpiantoDatiCodicePrivato IsNot Nothing Then

                                                    XMLs_ImpiantoCodicePrivato = XML_ImpiantoDatiCodicePrivato.GetElementsByTagName("CodiceImpianto")

                                                    For N_Codice = 0 To XMLs_ImpiantoCodicePrivato.Count - 1

                                                        XML_ImpiantoCodicePrivato = XMLs_ImpiantoCodicePrivato(N_Codice)
                                                        Codice = XML_ImpiantoCodicePrivato.GetAttribute("id_cod")
                                                        Valore = XML_ImpiantoCodicePrivato.GetAttribute("val_cod")

                                                        Select Case Codice
                                                            Case enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                                                Tra_Fila = Valore
                                                            Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                                                                Su_Fila = Valore
                                                            Case enum_CodiciAnagrafe.Impianto_Interbina
                                                                Interbina = Valore
                                                            Case enum_CodiciAnagrafe.Impianto_Germinabilita
                                                                Germinabilita = Valore
                                                            Case enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato
                                                                Cod_Dettaglio_Specie_Personalizzato = Valore
                                                            Case enum_CodiciAnagrafe.Codice_Impianto
                                                                Cod_Impianto = Valore
                                                            Case 2000 To 2999
                                                                a_Chiave_Cliente = Valore
                                                        End Select

                                                    Next

                                                End If

                                                Dim veg_Des As String = ""
                                                Dim cul_Des As String = ""
                                                Dim Grva_Des As String = ""
                                                Dim Grfi_Des As String = ""
                                                Dim Port_Des As String = ""
                                                Dim Foral_Des As String = ""
                                                Dim Imp_Des As String = ""
                                                Dim Cop_Des As String = ""
                                                Dim DestUso_Cod As Integer = 0
                                                Dim DestUso_Des As String = ""
                                                Dim Flag_TerrenoNudo As Integer = -1
                                                Cod_Specie_Gias = "0"

                                                If Forma_Allevamento_Cod = "" Then
                                                    Forma_Allevamento_Cod = "0"
                                                End If

                                                Recupera_DatiColturaliImpianto(Piva,
                                                                                   Sa_Cod,
                                                                                   Appezza,
                                                                                   Id_Reg,
                                                                                   Cod_Specie_Gias,
                                                                                   veg_Des,
                                                                                   cul_Des,
                                                                                   Cod_TipologiaVarietale_Gias,
                                                                                    Grva_Des,
                                                                                    Cod_Finalita_Gias,
                                                                                    Grfi_Des,
                                                                                    Portinnesto_Cod,
                                                                                    Port_Des,
                                                                                    Forma_Allevamento_Cod,
                                                                                    Foral_Des,
                                                                                    Imp_Irrigazione_Cod,
                                                                                    Imp_Des,
                                                                                    Copertura_Cod,
                                                                                    Cop_Des,
                                                                                    DestUso_Cod,
                                                                                    DestUso_Des,
                                                                                    objParametri_server)

                                                If Cod_Varieta_Gias = 0 Then
                                                    Flag_TerrenoNudo = 1
                                                Else
                                                    Flag_TerrenoNudo = 0
                                                End If

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "impianto: " & veg_Des & " - " & cul_Des & " - " & Grva_Des & " - " & imp_Validita_Inizio)

                                                If Not Flag_GestioneCodifiche Then
                                                    Cod_Specie_Cliente = ""
                                                    Cod_Varieta_Cliente = ""
                                                    Cod_Finalita_Cliente = ""
                                                    Cod_Portinnesto_Cliente = ""
                                                    Cod_Forma_Allevamento_Cliente = ""
                                                    Cod_Copertura_Cliente = ""
                                                    Cod_Impianto_Irriguo_Cliente = ""
                                                Else

                                                    '-------------------------------
                                                    'mappature cliente
                                                    '----------------------------------
                                                    Cod_Specie_Cliente = ""
                                                    Cod_Varieta_Cliente = ""
                                                    Cod_Finalita_Cliente = ""
                                                    Cod_Portinnesto_Cliente = ""
                                                    Cod_Forma_Allevamento_Cliente = ""
                                                    Cod_Copertura_Cliente = ""
                                                    Cod_Impianto_Irriguo_Cliente = ""

                                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Decodifiche ")

                                                    Dim DrCul As DataRow()
                                                    If Dt_Codifica_Cultivar IsNot Nothing AndAlso Cod_Varieta_Gias <> "" Then
                                                        DrCul = Dt_Codifica_Cultivar.Select("Cultivar_Gias='" & Cod_Varieta_Gias & "'")
                                                        If DrCul IsNot Nothing AndAlso DrCul.Length > 0 Then
                                                            Cod_Specie_Cliente = Split(DrCul(0).Item("cultivar_coltiva"), "/")(0)
                                                            Cod_Varieta_Cliente = Split(DrCul(0).Item("cultivar_coltiva"), "/")(1)
                                                        End If
                                                    End If
                                                    Dim DrPort As DataRow()
                                                    If Dt_Codifica_Portinnesti IsNot Nothing AndAlso Portinnesto_Cod <> "" Then
                                                        DrPort = Dt_Codifica_Portinnesti.Select("Port_Cod_Gias='" & Portinnesto_Cod & "'")
                                                        If DrPort IsNot Nothing AndAlso DrPort.Length > 0 Then
                                                            Cod_Portinnesto_Cliente = DrPort(0).Item("Port_Cod_Cliente")
                                                        End If
                                                    End If
                                                    Dim DrForAll As DataRow()
                                                    If Dt_Codifica_FormeAllevamento IsNot Nothing AndAlso Forma_Allevamento_Cod <> "" Then
                                                        DrForAll = Dt_Codifica_FormeAllevamento.Select("Foral_Cod_Gias='" & Forma_Allevamento_Cod & "'")
                                                        If DrForAll IsNot Nothing AndAlso DrForAll.Length > 0 Then
                                                            Cod_Forma_Allevamento_Cliente = DrForAll(0).Item("Foral_Cod_Cliente")
                                                        End If
                                                    End If

                                                    '22/08/2019: commentato, mi sembra una roba ad personam ereditata dall'esporta_gias_pubblico di terremerse
                                                    'Cod_Copertura_Cliente = GetCoperturaCliente(Copertura_Cod, Gru_Cod)

                                                End If

                                                XML_ImpiantoPubblico = xpa.Xml_Impianto(XmlDocPubblico,
                                                                                        CStr("1"),
                                                                                        CStr(Id_Reg),
                                                                                        Cod_Specie_Gias,
                                                                                        veg_Des,
                                                                                        Cod_Varieta_Gias,
                                                                                        cul_Des,
                                                                                        DestUso_Cod,
                                                                                        DestUso_Des,
                                                                                        Flag_TerrenoNudo,
                                                                                        Cod_TipologiaVarietale_Gias,
                                                                                        Grva_Des,
                                                                                        Cod_Finalita_Gias,
                                                                                        Grfi_Des,
                                                                                        Portinnesto_Cod,
                                                                                        Port_Des,
                                                                                        Imp_Irrigazione_Cod,
                                                                                        Imp_Des,
                                                                                        Forma_Allevamento_Cod,
                                                                                        Foral_Des,
                                                                                        Copertura_Cod,
                                                                                        Cop_Des,
                                                                                            Semina_Trapianto,
                                                                                        Tra_Fila,
                                                                                        Su_Fila,
                                                                                        Interbina,
                                                                                        Germinabilita,
                                                                                        Sup_Imp,
                                                                                        Cod_Dettaglio_Specie_Personalizzato,
                                                                                        Cod_Specie_Cliente,
                                                                                        Cod_Finalita_Cliente,
                                                                                        Cod_Varieta_Cliente,
                                                                                        Cod_Portinnesto_Cliente,
                                                                                        Cod_Forma_Allevamento_Cliente,
                                                                                        Cod_Copertura_Cliente,
                                                                                        Cod_Impianto_Irriguo_Cliente,
                                                                                        Cod_Impianto,
                                                                                        Scarto,
                                                                                        imp_Validita_Inizio,
                                                                                        imp_Validita_Fine,
                                                                                        imp_Chiave_Cliente)


                                                XML_AppezzamentoPubblico.AppendChild(XML_ImpiantoPubblico)

                                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato XML nodo Impianto ")

                                                '----------------------------------
                                                '----------------------------------
                                                '------------ PROGETTO ------------
                                                '----------------------------------
                                                '----------------------------------

                                                If Flag_Crea_Nodo_Progetto = 1 Then

                                                    strDatiProgetto = objProgetto.Impresa_Progetti_Leggi(CStr(Piva),
                                                                                                                 0,
                                                                                                                 "",
                                                                                                                 0,
                                                                                                                 CInt(Sa_Cod),
                                                                                                                 CInt(Appezza),
                                                                                                                 CInt(Id_Reg),
                                                                                                                 0, 0, False,
                                                                                                                 objParametri)

                                                    If strDatiProgetto <> "" Then

                                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ESERCIZIO ")

                                                        XmlDocPrivato.LoadXml(strDatiProgetto)

                                                        XMLs_ProgettoPrivato = XmlDocPrivato.SelectSingleNode("DatiProgetto").SelectNodes("Progetto")

                                                        For N_Progetto = 0 To XMLs_ProgettoPrivato.Count - 1

                                                            XML_ProgettoPrivato = XMLs_ProgettoPrivato(N_Progetto)

                                                            Progetto_Cod = XML_ProgettoPrivato.GetAttribute("progetto_cod")
                                                            Progetto_Nome = XML_ProgettoPrivato.GetAttribute("progetto_nome")
                                                            Progetto_Des = XML_ProgettoPrivato.GetAttribute("progetto_des")

                                                            '********* PERSONALIZZAZIONE X ABOCA *********************
                                                            If codice_GIAS = enum_CodiceGIAS_Clienti.Aboca Then
                                                                'o   Descrizione: dividerla in due chiamandoli Specie_Esercizio_Codice e Specie_Esercizio_Descr 
                                                                'prendendo da Progetto_Des fino al “ - “ per il codice e dal “ - “ (escluso) in poi per la descrizione; 
                                                                'se per caso non viene trovato il “ - “ mettere tutto in Specie_Esercizio_Descr e lasciare a 0 il codice.    
                                                                'Nota:questo campo in Aboca viene inserito a mano dall'utente in anagrafica esercizi andando a scegliere dai Semilavorati Vegetali 
                                                                'tramite una finestra di ricerca che imposta nel campo descrizione il cod_articolo e mat_des di Materie_Prime con “ - “ nel mezzo
                                                                If InStr(Progetto_Des, "-") > 0 Then
                                                                    Select Case Progetto_Des.Split("-").Length
                                                                        Case 0, 1
                                                                            'non succede
                                                                            prg_Specie_Esercizio_Codice = "0"
                                                                            prg_Specie_Esercizio_Descr = Progetto_Des
                                                                        Case 2
                                                                            prg_Specie_Esercizio_Codice = Trim(Progetto_Des.Split("-")(0))
                                                                            prg_Specie_Esercizio_Descr = Trim(Progetto_Des.Split("-")(1))
                                                                        Case Else
                                                                            'da gestire meglio con funzioni su stringhe
                                                                            prg_Specie_Esercizio_Codice = Trim(Progetto_Des.Split("-")(0))
                                                                            prg_Specie_Esercizio_Descr = Trim(Progetto_Des.Split("-")(1) & "-" & Progetto_Des.Split("-")(2))
                                                                    End Select
                                                                Else
                                                                    prg_Specie_Esercizio_Codice = "0"
                                                                    prg_Specie_Esercizio_Descr = Progetto_Des
                                                                End If

                                                                'o   NumOPCollegamento: è l'eventuale OP dell’azienda SAM collegato a quello di Aboca, creato quando si effettua il ribaltamento impianti da SAM ad Aboca.
                                                                'Si trova leggendo reg_impianti_codici senza PIVA, con id_cod 1311 e con val_cod = piva + “|” + sa_cod + “|” + appezza + “|” + id_reg + “|” + progetto_cod
                                                                'Leggendo il record trovato e prendendo piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod si cerca Imprese_progetti e si prende PIVA + “_” + Progetto_nome
                                                                'Ad es.ho letto un esercizio che ha come chiave piva = 01704430519, Sa_Cod = 130023437, Appezza = 130023425, Id_Reg = 130023425, Progetto_Cod = 2326...
                                                                'cerco su reg_impianti_codici con id_cod 1311 e val_cod = 01704430519|130023437|130023425|130023425|2326
                                                                'Trovo record con Piva= '02969160544’, sa_cod = 130023425, appezza = 130023425, id_reg = 130023425, progetto_cod = 1935
                                                                'Vado a leggere imprese_progetti con questa chiave e trovo progetto_nome = 17019698
                                                                'Imposto NumOPCollegamento = 02969160544_17019698
                                                                prg_NumOPCollegamento = objProgCodici.NumOPCollegamento_From_Progetto(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, objParametri_server)

                                                            Else
                                                                prg_Specie_Esercizio_Codice = ""
                                                                prg_Specie_Esercizio_Descr = ""
                                                                prg_NumOPCollegamento = ""
                                                            End If

                                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "esercizio: " & Progetto_Cod.ToString & " - " & Progetto_Nome)

                                                            prg_Resa_Prevista = XML_ProgettoPrivato.GetAttribute("produzione_prevista")
                                                            prg_Regolamento_Cod = XML_ProgettoPrivato.GetAttribute("regolamento_cod")
                                                            prg_Disciplinare_Cod = XML_ProgettoPrivato.GetAttribute("disciplinare_cod")
                                                            prg_Stato_Impianto = XML_ProgettoPrivato.GetAttribute("stato_impianto")
                                                            prg_Validita_Inizio = XML_ProgettoPrivato.GetAttribute("validita_inizio")
                                                            prg_Validita_Fine = XML_ProgettoPrivato.GetAttribute("validita_fine")
                                                            'volutamente non è stato fatto il toShortDateString
                                                            prg_data_modifica = XML_ProgettoPrivato.GetAttribute("data_modifica")
                                                            prg_username_modifica = XML_ProgettoPrivato.GetAttribute("username_modifica")
                                                            prg_utente_modifica = objUtenti.NomeCognome_From_CodFisc(prg_username_modifica, objParametri_utenti)

                                                            Piante_Ha = XML_ProgettoPrivato.GetAttribute("p_ha")

                                                            If prg_Regolamento_Cod <> 0 Then
                                                                prg_Regolamento_Des = objRegol.RegDes_from_RegCod(prg_Regolamento_Cod, objParametri_server)
                                                            Else
                                                                prg_Regolamento_Des = ""
                                                            End If

                                                            'CODICI
                                                            prg_Piano_Semina = ""
                                                            prg_Org_Referente_Cod = ""
                                                            prg_Org_Referente_Des = ""
                                                            prg_Capitolato_Privato_Cod = ""
                                                            prg_Capitolato_Privato_Des = ""
                                                            prg_Data_Chiusura_Esercizio = ""

                                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Codici Progetto ")
                                                            XML_ProgettoDatiCodicePrivato = XML_ProgettoPrivato.SelectSingleNode("DatiCodici")

                                                            If XML_ProgettoDatiCodicePrivato IsNot Nothing Then

                                                                XMLs_ProgettoCodicePrivato = XML_ProgettoDatiCodicePrivato.GetElementsByTagName("CodiceImpianto")

                                                                For N_Codice = 0 To XMLs_ProgettoCodicePrivato.Count - 1

                                                                    XML_ProgettoCodicePrivato = XMLs_ProgettoCodicePrivato(N_Codice)
                                                                    Codice = XML_ProgettoCodicePrivato.GetAttribute("id_cod")
                                                                    Valore = XML_ProgettoCodicePrivato.GetAttribute("val_cod")

                                                                    Select Case Codice
                                                                        Case enum_CodiciAnagrafe.Impianto_PianoSemina
                                                                            prg_Piano_Semina = Valore
                                                                        Case enum_CodiciAnagrafe.Impianto_Cooperativa
                                                                            prg_Org_Referente_Cod = Valore
                                                                            If prg_Org_Referente_Cod.Trim <> "" Then
                                                                                prg_Org_Referente_Des = objContattiR.RagSoc_From_CodContatto("",
                                                                                                                                                 prg_Org_Referente_Cod,
                                                                                                                                                 " ( Contatti.Piva = '" & CStr(Piva) & "' OR Contatti.Sa_Cod = -1 )",
                                                                                                                                                 objParametri_server)
                                                                            End If

                                                                        Case enum_CodiciAnagrafe.Codice_Capitolato_Privato
                                                                            prg_Capitolato_Privato_Cod = Valore
                                                                            If prg_Capitolato_Privato_Cod <> "" Then
                                                                                prg_Capitolato_Privato_Des = objCACInfoAgg.InfoAgg_Des_from_InfoAgg_Cod(prg_Capitolato_Privato_Cod,
                                                                                                                                                            enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CapitolatoPrivato,
                                                                                                                                                            0,
                                                                                                                                                            objParametri_server)
                                                                            End If
                                                                        Case enum_CodiciAnagrafe.Distinta_Chiusa
                                                                            'ABOCA
                                                                            'o	Aggiungere Data_Chiusura_Esercizio: esercizio è chiuso se id_cod = 1301 di reg_impianti_codici vale 1; 
                                                                            'come Data prendere Data_Creazione (di reg_impianti_codici) perché tanto una volta chiuso non si apre più
                                                                            prg_Data_Chiusura_Esercizio = objProgCodici.DataChiusuraEsercizio_From_Progetto(Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, "", objParametri_server)
                                                                    End Select

                                                                Next

                                                            End If

                                                            '22/08/2019: commentato, pare personalizzazione di Terremerse
                                                            'prg_Capitolato_Privato_Cliente = GetCapitolatoCliente(prg_Capitolato_Privato)

                                                            'scrivo
                                                            XML_ProgettoPubblico = xpa.Xml_ProgettoImpianto(XmlDocPubblico,
                                                                                                            CStr("1"),
                                                                                                            CStr(Progetto_Cod),
                                                                                                            Progetto_Nome,
                                                                                                            Progetto_Des,
                                                                                                            prg_Resa_Prevista,
                                                                                                            Piante_Ha,
                                                                                                            prg_Org_Referente_Cod,
                                                                                                            prg_Org_Referente_Des,
                                                                                                            prg_Capitolato_Privato_Cod,
                                                                                                            prg_Capitolato_Privato_Des,
                                                                                                            prg_Regolamento_Cod,
                                                                                                            prg_Regolamento_Des,
                                                                                                            prg_Disciplinare_Cod,
                                                                                                            prg_Piano_Semina,
                                                                                                            prg_Stato_Impianto,
                                                                                                            prg_Validita_Inizio,
                                                                                                            prg_Validita_Fine,
                                                                                                            prg_data_modifica,
                                                                                                            prg_username_modifica,
                                                                                                            prg_utente_modifica,
                                                                                                            prg_Specie_Esercizio_Codice,
                                                                                                            prg_Specie_Esercizio_Descr,
                                                                                                            prg_Data_Chiusura_Esercizio,
                                                                                                            prg_NumOPCollegamento)

                                                            XML_ImpiantoPubblico.AppendChild(XML_ProgettoPubblico)

                                                            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Creato nodo Esercizio ")

                                                        Next 'esercizi

                                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Esercizi ")
                                                    Else
                                                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati esercizi")
                                                    End If
                                                Else
                                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione esercizi non attiva")
                                                End If
                                            End If

                                        End If

                                    Next 'impianti

                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine impianti ")
                                Else
                                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati impianti")
                                End If
                            Else
                                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Impianti non attiva")
                            End If
                        End If

                        If Campo_Cod = 0 Then
                            '----------------------------------------
                            '---------- NO GESTIONE CAMPO -----------
                            '----------------------------------------
                            XML_CentroPubblico.AppendChild(XML_AppezzamentoPubblico)
                        Else
                            '----------------------------------------
                            '---------- SI GESTIONE CAMPO -----------
                            '----------------------------------------
                            XML_CampoPubblico.AppendChild(XML_AppezzamentoPubblico)
                        End If

                    Next 'dt appezzamenti

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Appezzamenti ")
                Else
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non sono stati trovati appezzamenti")
                End If
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esportazione Appezzamenti non attiva")
            End If

        Catch ex As Exception
            Throw New Exception(nome_funzione & ": Non è stato possibile creare l'elemento Appezzamento (" & Piva & " - " & Sa_Cod & " - " & Campo_Cod & ")")
        End Try

    End Sub


    '###########################################################################################
    Private Sub Recupera_DatiColturaliImpianto(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Appezza As Integer,
                                               ByVal Id_Reg As Integer,
                                               ByRef Veg_Cod As Integer,
                                               ByRef Veg_Des As String,
                                               ByRef Cul_Des As String,
                                               ByRef Grva_Cod As Integer,
                                               ByRef Grva_Des As String,
                                               ByRef Grfi_Cod As Integer,
                                               ByRef Grfi_Des As String,
                                               ByRef Port_Cod As Integer,
                                               ByRef Port_Des As String,
                                               ByRef Foral_Cod As Integer,
                                               ByRef Foral_Des As String,
                                               ByRef Imp_Cod As Integer,
                                               ByRef Imp_Des As String,
                                               ByRef Cop_Cod As Integer,
                                               ByRef Cop_Des As String,
                                               ByRef DestUso_Cod As Integer,
                                               ByRef DestUso_Des As String,
                                               ByVal objParametri As AgronicaCoreParametri)

        Dim dt As DataTable
        Dim objimp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Veg_Cod = 0
        Veg_Des = ""
        Cul_Des = ""
        Grva_Cod = 0
        Grva_Des = ""
        Grfi_Cod = 0
        Grfi_Des = ""
        Port_Cod = 0
        Port_Des = ""
        Foral_Cod = 0
        Foral_Des = ""
        Imp_Cod = 0
        Imp_Des = ""
        Cop_Cod = 0
        Cop_Des = ""
        DestUso_Cod = 0
        DestUso_Des = ""

        dt = objimp.Leggi_DatiColturali_Impianto(False,
                                                 Piva,
                                                    Sa_Cod,
                                                    Appezza,
                                                    Id_Reg,
                                                    0,
                                                    0,
                                                    0, 0, 0, 0, 0,
                                                    "",
                                                    "",
                                                    objParametri_server)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Veg_Cod = dt.Rows(0).Item("Veg_Cod")
            Veg_Des = dt.Rows(0).Item("Veg_Des")
            Cul_Des = dt.Rows(0).Item("cul_Des")
            Grva_Cod = dt.Rows(0).Item("Grva_Cod")
            Grva_Des = dt.Rows(0).Item("Grva_Des")
            Grfi_Cod = dt.Rows(0).Item("Grfi_Cod")
            Grfi_Des = dt.Rows(0).Item("Grfi_Des")
            Port_Cod = dt.Rows(0).Item("Port_Cod")
            Port_Des = dt.Rows(0).Item("Port_Des")
            Foral_Cod = dt.Rows(0).Item("Foral_Cod")
            Foral_Des = dt.Rows(0).Item("Foral_Des")
            Imp_Cod = dt.Rows(0).Item("Imp_Cod")
            Imp_Des = dt.Rows(0).Item("Imp_Des")
            Cop_Cod = dt.Rows(0).Item("Cop_Cod")
            Cop_Des = dt.Rows(0).Item("Cop_Des")
            DestUso_Cod = dt.Rows(0).Item("DestUso_Cod")
            DestUso_Des = dt.Rows(0).Item("DestUso_Des")
        End If

    End Sub

    '################################################################
    Public Sub Leggi_DatiImpreseUtente(
        ByVal Gis_Appezzamento_TipoEntitaCheSurrogaDati As enum_GIS2012_TipoEntita,
        ByVal cfgDatiImpresa As AgronicaCoreModello.clsDatiImpresa,
        ByRef Dt_Imprese As DataTable,
        ByRef Dt_Centri As DataTable,
        ByRef Dt_Campi As DataTable,
        ByRef Dt_Appezzamenti As DataTable,
        ByRef Dt_Impianti As DataTable,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal Filtro_Piva As String = ""
    )


        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New JoinFiltrone

        Dim FiltroSQLPersonalizzato As String = ""

        '(21/01/2016 richiesta ceccoli) 
        'forzo il filtro per filtrare
        '- impianti di arboree
        '- impianto con organismo referente 'terremerse'
        ClassJoin.bCentriAziendali = True
        ClassJoin.bCampi = True
        ClassJoin.bAppezzamento = True
        ClassJoin.bRegImpianti = True
        ClassJoin.bRegImpiantiCodici = True
        ClassJoin.bImpreseProgetti = True
        ClassJoin.bSpecieVegetali = True
        ClassJoin.bCultivar = True

        If cfgDatiImpresa.Flagimporta_gis Then
            ClassJoin.bGis_Appezzamento = True
            ClassJoin.bGis_CentriAziendali = True
            ClassJoin.bGis_Appezzamento_TipoEntitaCheSurrogaDAti = Gis_Appezzamento_TipoEntitaCheSurrogaDati
        Else
            ClassJoin.bGis_Appezzamento = False
            ClassJoin.bGis_CentriAziendali = False
        End If


        ' VAnni: 8/8/2019: impostare da configurazione
        ''(25-01-2016) filtro richiesto TERREMERSE
        ''arboree (1) + orticole (3)  
        'FiltroSQLPersonalizzato = " AND (SpecieVegetali.Gru_Cod = 1 OR SpecieVegetali.Gru_Cod = 3) "
        ''finalità mercato fresco (2) + Da mensa in coltura protetta (8)
        'FiltroSQLPersonalizzato = " AND (Reg_Impianti.Grfi_Cod = 2 OR Reg_Impianti.Grfi_Cod = 8) "

        'solo imprese/impianti con organo referente = terremerse
        If FiltroPerOrganismoReferente = 1 Then
            FiltroSQLPersonalizzato &= " AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_Cooperativa
            FiltroSQLPersonalizzato &= " AND Reg_Impianti_Codici.val_cod = '" & objParametri.PivaSuperUser & "') "
        End If


        If Filtro_Piva <> "" Then
            FiltroSQLPersonalizzato &= " AND Imprese.piva='" & Filtro_Piva & "'"
        End If

        Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri,
                                                FiltroSQLPersonalizzato,
                                                enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                " ORDER BY Imprese.rag_soc ",
                                                ClassJoin)

        Dt_Centri = classFiltrone.CreaDTFiltrone(objParametri,
                                                FiltroSQLPersonalizzato,
                                                enum_TipoSelect_FiltroneSuperNova.CentriAziendali,
                                                "ORDER BY Imprese.rag_soc",
                                                ClassJoin)

        Dt_Campi = classFiltrone.CreaDTFiltrone(objParametri,
                                        FiltroSQLPersonalizzato,
                                        enum_TipoSelect_FiltroneSuperNova.Campi,
                                        "ORDER BY Imprese.rag_soc",
                                        ClassJoin)

        Dt_Appezzamenti = classFiltrone.CreaDTFiltrone(objParametri,
                                FiltroSQLPersonalizzato,
                                enum_TipoSelect_FiltroneSuperNova.Appezzamenti,
                                "ORDER BY Imprese.rag_soc",
                                ClassJoin)

        Dt_Impianti = classFiltrone.CreaDTFiltrone(objParametri,
                                        FiltroSQLPersonalizzato,
                                        enum_TipoSelect_FiltroneSuperNova.Impianti,
                                        "ORDER BY Imprese.rag_soc",
                                        ClassJoin)
    End Sub

    Public Sub Leggi_Codifiche(ByRef Dt_Codifica_Cultivar As DataTable,
                               ByRef Dt_Codifica_FormeAllevamento As DataTable,
                               ByRef Dt_Codifica_Portinnesti As DataTable,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       )

        Dim objCodCul As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R

        Dt_Codifica_Cultivar = objCodCul.Leggi("", 0, 0, AGRODATAINIZIO, 0, 0, 0, 0, "",
                                             "", "",
                                             objParametri,
                                             enum_Tipo_CAC_Codifica_Specie.Apofruit_Siagr)


        Dim objCodFA As New AgronicaCoreAnagrafeDAL.CAC_Codifica_FormeAllevamento_R

        Dt_Codifica_FormeAllevamento = objCodFA.Leggi("", 0,
                                             "", "",
                                             objParametri,
                                             enum_Tipo_CAC_Codifica_Specie.Apofruit_Siagr)

        Dim objCodPort As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Portinnesti_R

        Dt_Codifica_Portinnesti = objCodPort.Leggi("", 0,
                                             "", "",
                                             objParametri,
                                             enum_Tipo_CAC_Codifica_Specie.Apofruit_Siagr)


    End Sub

    'Private Function GetCoperturaGias(ByVal val As String, ByVal Gru_Cod As String) As String
    '    '        TabCod(TabDes)
    '    '        __(NESSUNA)
    '    'NC	NON COPERTO                   
    '    'NR	SERRA ROMAGNOLA NON FORZATA   
    '    'NV	TUNNEL MULTIPLO NON FORZATO   
    '    'RA	RETE ANTIGRANDINE             
    '    'SR	SERRA ROMAGNOLA FORZATA       
    '    'SV	TUNNEL MULTIPLO FORZATO       
    '    'TE	TESSUTO NON TESSUTO ********  
    '    'TM	TUNNEL MULTIPLO (VERONESE) ***
    '    'TR	TUNNEL ROMAGNOLO  **********  

    '    '(16/10/2015 fede) aggiunti su SIAGR valori 
    '    'SI
    '    'NO

    '    Dim valGias As String
    '    Select Case val
    '        Case "__", "NC", "NO" 'NESSUNA, NON COPERTO
    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "3"
    '                Case 2
    '                    valGias = "4"
    '                Case 3
    '                    valGias = "5"
    '                Case Else
    '                    valGias = "-1"
    '            End Select


    '        Case "NR" 'NR SERRA ROMAGNOLA NON FORZATA 
    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "0"
    '                Case 3
    '                    valGias = "11" 'mappato
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "SR" 'SR	SERRA ROMAGNOLA FORZATA 
    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "0"
    '                Case 3
    '                    valGias = "12" 'mappato
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "NV" 'NVTUNNEL MULTIPLO NON FORZATO

    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "10" 'mappato
    '                Case 3
    '                    valGias = "13" 'mappato
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "SV" ' SV	TUNNEL MULTIPLO FORZATO 

    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "10" 'mappato
    '                Case 3
    '                    valGias = "14" 'mappato
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "TM", "TR" 'TM	TUNNEL MULTIPLO (VERONESE),TR	TUNNEL ROMAGNOLO 

    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "10" 'mappato
    '                Case 3
    '                    valGias = "8" 'mappato
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "RA"
    '            Select Case Gru_Cod 'mappato
    '                Case 1
    '                    valGias = "1"
    '                Case 2
    '                    valGias = "0"
    '                Case 3
    '                    valGias = "0"
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case "TE"
    '            valGias = "0"

    '        Case "SI" 'SI
    '            Select Case Gru_Cod
    '                Case 1
    '                    valGias = "0"
    '                Case 2
    '                    valGias = "9" 'Tunnellino
    '                Case 3
    '                    valGias = "2" 'Serra
    '                Case Else
    '                    valGias = "-1"
    '            End Select

    '        Case Else
    '            valGias = "-1"
    '    End Select
    '    Return valGias
    'End Function

    'Private Function GetCoperturaCliente(ByVal valGias As String, ByVal Gru_Cod As String) As String

    '    Dim val As String

    '    Select Case CInt(Gru_Cod)
    '        Case 1
    '            Select Case valGias
    '                Case "0"
    '                    val = "NR"
    '                Case "1"
    '                    val = "RA"
    '                Case "3"
    '                    val = "NC"
    '            End Select
    '        Case 2
    '            Select Case valGias
    '                Case "4"
    '                    val = "NC"
    '                Case "10"
    '                    val = "SV"
    '                Case "9"
    '                    val = "SI"
    '            End Select
    '        Case 3
    '            Select Case valGias
    '                Case "5"
    '                    val = "NC"
    '                Case "11"
    '                    val = "NR"
    '                Case "12"
    '                    val = "SR"
    '                Case "13"
    '                    val = "NV"
    '                Case "14"
    '                    val = "SV"
    '                Case "8"
    '                    val = "TR"
    '                Case "2"
    '                    val = "SI"
    '            End Select
    '    End Select

    '    Return val

    'End Function

    ''(26/01/2016 mail saragoni) codici mappati da saragoni
    'Private Function GetCapitolatoCliente(ByVal valGias As String) As String

    '    Dim val As String = ""

    '    Select Case valGias
    '        Case "LIDP"
    '            val = "AT"
    '        Case "BIO"
    '            val = "BI"
    '        Case "CONV"
    '            val = "CO"
    '        Case "EGBI"
    '            val = "EB"
    '        Case "LIEG"
    '            val = "EG"
    '        Case "EUIG"
    '            val = "IE"
    '        Case "IGP"
    '            val = "IG"
    '        Case "DOP"
    '            val = "DO"
    '    End Select

    '    Return val

    'End Function



End Class
