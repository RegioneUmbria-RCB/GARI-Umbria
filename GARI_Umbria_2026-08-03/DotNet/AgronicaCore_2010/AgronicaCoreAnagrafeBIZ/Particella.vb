Imports System.Data.Common
Imports System.Data.Entity
Imports System.Linq
Imports System.Transactions
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaConversioneCartografiaGias
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json

Public Class Particella_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CheckParticellaxModificaCancellazione(ByVal cancellazione As Boolean,
                                                          ByVal prov As String,
                                                          ByVal com As String,
                                                          ByVal sezione As String,
                                                          ByVal foglio As Integer,
                                                          ByVal numero As Integer,
                                                          ByVal subalterno As String,
                                                          ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

        Return objParticelle_R.CheckParticellaPerModificaCancellazione(cancellazione,
                                                                       prov,
                                                                       com,
                                                                       sezione,
                                                                       foglio,
                                                                       numero,
                                                                       subalterno,
                                                                       objParametri)

    End Function

    Public Function LeggiParticellaxWarning(
        ByVal piva As String, ByVal sa_cod As String, ByVal prov As String, ByVal com As String, ByVal sezione As String,
        ByVal foglio As Integer, ByVal numero As Integer, ByVal subalterno As String, ByRef objParametri As AgronicaCoreParametri) As List(Of String)

        Dim listaWarning As New List(Of String)
        Dim tabelle = New Dictionary(Of String, String)
        'tabelle("ParticelleCatastali") = "Particelle|0"
        'tabelle("ParticelleCatastaliClassamento") = "Classamento|0"
        'tabelle("ParticelleCatastali_MetodoProduzione") = "Metodo Produzione|0"
        'tabelle("ZonexParticelle") = "Zone|0"
        'tabelle("ParticelleCatastalixMacrousi") = "Macrousi|1"
        'tabelle("ParticelleCatastalixMacrousixUtilizzo") = "Utilizzo|1"
        'tabelle("ImpreseXParticelle") = "Centri Aziendali|2"
        tabelle("CampiXParticelle") = "Campi|2|Campo_Des"
        tabelle("AppezzamentiXParticelle") = "Appezzamenti|2|APP_NOME"
        tabelle("ProgettixParticelle") = "Progetti|2"
        tabelle("Fabbricati") = "Fabbricati|2"
        tabelle("PianoConcimazione_EntitaxTestata") = "Piani Concimazione|2"

        Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

        For Each tabella As String In tabelle.Keys
            Dim tokens = tabelle(tabella).Split("|")
            Dim descrizione = tokens(0)
            Dim impresa As Boolean = tokens(1) <> "0"
            Dim centro As Boolean = tokens(1) = "2"
            Dim campi As String = If(tokens.Length > 2, tokens(2), "COUNT(*)")
            Dim dt = objParticelle_R.LeggixWarning(tabella, campi, If(impresa, piva, ""), If(centro, sa_cod, 0), prov, com, sezione, foglio, numero, subalterno, "", "", objParametri)
            Dim conteggio As Integer = If(campi = "COUNT(*)", CInt(dt.Rows(0).Item(0)), dt.Rows.Count)
            If conteggio > 0 Then
                Dim elementi As String = CStr(conteggio)
                If campi <> "COUNT(*)" Then
                    Dim listaElementi As New List(Of String)
                    For Each dr In dt.Rows
                        listaElementi.Add(dr.Item(0))
                    Next
                    elementi = String.Join(", ", listaElementi)
                End If
                listaWarning.Add(descrizione & "|" & elementi)
            End If
        Next

        Return listaWarning

    End Function

    Public Function ImpresexParticelle_Leggi(ByVal PIVA As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Part_Cod As Integer,
                                            ByVal prov As String,
                                            ByVal com As String,
                                            ByVal sezione As String,
                                            ByVal foglio As Integer,
                                            ByVal Numero As Integer,
                                            ByVal subalterno As String,
                                            ByVal ForDelete As Boolean,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal TipoG2G As Integer = 0
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Particella.ImpresexParticelle_Leggi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '------------------------------
            'Mi procuro un elenco delle Particelle associate all'Impresa
            'all'interno della finestra temporale selezionata

            Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim DtImpresexParticelle As DataTable

            'Mi procuro il recordset richiesto
            DtImpresexParticelle = objImpresexParticelle.Leggi(0,
                                                               CStr(PIVA),
                                                               CLng(Sa_Cod),
                                                               CLng(Part_Cod),
                                                               CStr(prov),
                                                               CStr(com),
                                                               CStr(sezione),
                                                               CLng(foglio),
                                                               CLng(Numero),
                                                               CStr(subalterno),
                                                               enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                               "",
                                                               "",
                                                               objParametri,
                                                               TipoG2G)

            '-----------------------------

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtImpresexParticelle.Rows.Count <> 0 Then

                Dim XmlDoc As New XmlDocument
                Dim XmlDatiImpresexParticelle As XmlElement
                Dim XmlImpresexParticelle As XmlElement
                Dim XmlZona As XmlElement
                Dim XmlMacrouso As XmlElement
                Dim XmlUtilizzo As XmlElement


                'Nota: Anche se ci troviamo dentro la routine ImpresexParticelle_Leggi la stringa
                'Xml viene costruita con i nodi riferiti alla sola particella. Questa accortezza è
                'necessaria nel caso di lettura prima della cancellazione della particella, poiché
                'sono necessari Piva e Sa_Cod

                XmlDatiImpresexParticelle = XmlDoc.CreateElement("DatiParticelle")

                'Effettuo un ciclo sulle ImpresexParticelle
                'Do While Not RsImpresexParticelle.EOF
                Dim iIxP As Integer
                For iIxP = 0 To DtImpresexParticelle.Rows.Count - 1

                    '----- < IMPRESEXPARTICELLA > -----
                    XmlImpresexParticelle = XmlDoc.CreateElement("Particella")

                    With XmlImpresexParticelle
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Piva")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Sa_Cod")))
                        .SetAttribute("part_cod", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Part_Cod")))
                        .SetAttribute("prov", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Prov")))
                        .SetAttribute("com", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Com")))
                        .SetAttribute("sezione", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Sezione")))
                        .SetAttribute("foglio", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Foglio")))
                        .SetAttribute("numero", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Numero")))
                        .SetAttribute("subalterno", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Subalterno")))
                        .SetAttribute("partita_catastale", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Partita_Catastale")))
                        .SetAttribute("ettari", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Ettari")))
                        .SetAttribute("are", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Are")))
                        .SetAttribute("centiare", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Centiare")))
                        .SetAttribute("qualita_cod", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Qualita_Cod")))
                        .SetAttribute("classe", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Classe")))
                        .SetAttribute("reddito_dominicale", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Reddito_Dominicale")))
                        .SetAttribute("reddito_agrario", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Reddito_Agrario")))
                        .SetAttribute("titolopossesso", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("xTitoloPossesso")))
                        .SetAttribute("sup_condotta", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Sup_Condotta")))
                        .SetAttribute("sup_spandibile", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Sup_Spandibile")))
                        .SetAttribute("sup_divieto", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Sup_Divieto")))

                        .SetAttribute("validita_inizio_centro", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("xValidita_Inizio")))
                        .SetAttribute("validita_fine_centro", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("xValidita_Fine")))
                        .SetAttribute("inviato", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Inviato")))
                        .SetAttribute("datainvio", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("DataInvio")))
                        .SetAttribute("data_creazione", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Data_Creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Data_Modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtImpresexParticelle.Rows(iIxP).Item("Validita_Fine")))
                    End With

                    '-------------------------------------------
                    ' ZONE 
                    '-------------------------------------------

                    Dim objZonepart As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
                    Dim DTZone As DataTable
                    DTZone = objZonepart.Leggi(0,
                                               CStr(DtImpresexParticelle.Rows(iIxP).Item("Prov")),
                                               CStr(DtImpresexParticelle.Rows(iIxP).Item("Com")),
                                               CStr(DtImpresexParticelle.Rows(iIxP).Item("sezione")),
                                               CInt(DtImpresexParticelle.Rows(iIxP).Item("foglio")),
                                               CInt(DtImpresexParticelle.Rows(iIxP).Item("numero")),
                                               CStr(DtImpresexParticelle.Rows(iIxP).Item("subalterno")),
                                               enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                               "", "",
                                               objParametri)

                    If DTZone IsNot Nothing AndAlso DTZone.Rows.Count > 0 Then

                        For i_Zona As Integer = 0 To DTZone.Rows.Count - 1

                            XmlZona = XmlDoc.CreateElement("Zona")

                            With XmlZona
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("zona_cod", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("zona_cod")))
                                .SetAttribute("prov", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Prov")))
                                .SetAttribute("com", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Com")))
                                .SetAttribute("sezione", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Sezione")))
                                .SetAttribute("foglio", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Foglio")))
                                .SetAttribute("numero", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Numero")))
                                .SetAttribute("subalterno", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("Subalterno")))
                                .SetAttribute("area", Agro_SQL_Load(DTZone.Rows(i_Zona).Item("area")))
                            End With

                            XmlImpresexParticelle.AppendChild(XmlZona)
                        Next

                    End If

                    '-------------------------------------------
                    ' MACROUSI 
                    '-------------------------------------------

                    Dim DTMacrousi As DataTable
                    Dim objPartCatxMacro As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R

                    Dim selezione_JoinDescrizioni As enumSelezioneVariabile
                    If TipoG2G = 0 Then
                        selezione_JoinDescrizioni = enumSelezioneVariabile.Selezione_JoinDescrizioni
                    Else
                        selezione_JoinDescrizioni = enumSelezioneVariabile.Selezione_TabellaCompleta
                    End If


                    DTMacrousi = objPartCatxMacro.Leggi(PIVA,
                                                        CStr(DtImpresexParticelle.Rows(iIxP).Item("Prov")),
                                                        CStr(DtImpresexParticelle.Rows(iIxP).Item("Com")),
                                                        CStr(DtImpresexParticelle.Rows(iIxP).Item("sezione")),
                                                        CInt(DtImpresexParticelle.Rows(iIxP).Item("foglio")),
                                                        CInt(DtImpresexParticelle.Rows(iIxP).Item("numero")),
                                                        CStr(DtImpresexParticelle.Rows(iIxP).Item("subalterno")),
                                                        "",
                                                        selezione_JoinDescrizioni,
                                                        "", "", objParametri)

                    If DTMacrousi IsNot Nothing AndAlso DTMacrousi.Rows.Count > 0 Then

                        Dim i_Macrouso As Integer
                        For i_Macrouso = 0 To DTMacrousi.Rows.Count - 1

                            XmlMacrouso = XmlDoc.CreateElement("Macrouso")

                            With XmlMacrouso
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("macrouso_cod", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("macrouso_cod")))
                                .SetAttribute("prov", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Prov")))
                                .SetAttribute("com", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Com")))
                                .SetAttribute("sezione", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Sezione")))
                                .SetAttribute("foglio", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Foglio")))
                                .SetAttribute("numero", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Numero")))
                                .SetAttribute("subalterno", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Subalterno")))
                                .SetAttribute("superficie", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("superficie")))

                                .SetAttribute("data_creazione", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Data_Creazione")))
                                .SetAttribute("data_modifica", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Data_Modifica")))
                                .SetAttribute("username_creazione", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Username_Creazione")))
                                .SetAttribute("username_modifica", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Username_Modifica")))

                                .SetAttribute("numero_fascicolo", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Numero_Fascicolo")))
                                .SetAttribute("data_validazione_fascicolo", Agro_SQL_Load(DTMacrousi.Rows(i_Macrouso).Item("Data_Validazione_Fascicolo")))
                            End With

                            '-------------------------------------------
                            ' UTILIZZI 
                            '-------------------------------------------

                            Dim objUtilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
                            Dim DTUtilizzo As DataTable

                            DTUtilizzo = objUtilizzo.Leggi(CStr(PIVA),
                                                           CStr(DtImpresexParticelle.Rows(iIxP).Item("Prov")),
                                                           CStr(DtImpresexParticelle.Rows(iIxP).Item("Com")),
                                                           CStr(DtImpresexParticelle.Rows(iIxP).Item("sezione")),
                                                           CInt(DtImpresexParticelle.Rows(iIxP).Item("foglio")),
                                                           CInt(DtImpresexParticelle.Rows(iIxP).Item("numero")),
                                                           CStr(DtImpresexParticelle.Rows(iIxP).Item("subalterno")),
                                                           CStr(DTMacrousi.Rows(i_Macrouso).Item("macrouso_cod")),
                                                           "", "",
                                                           selezione_JoinDescrizioni,
                                                           "", "",
                                                           objParametri)


                            For i_Utilizzo As Integer = 0 To DTUtilizzo.Rows.Count - 1

                                XmlUtilizzo = XmlDoc.CreateElement("Utilizzo")

                                With XmlUtilizzo
                                    .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                    .SetAttribute("macrouso_cod", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("macrouso_cod")))
                                    .SetAttribute("prov", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Prov")))
                                    .SetAttribute("com", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Com")))
                                    .SetAttribute("sezione", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Sezione")))
                                    .SetAttribute("foglio", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Foglio")))
                                    .SetAttribute("numero", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Numero")))
                                    .SetAttribute("subalterno", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Subalterno")))
                                    .SetAttribute("superficie", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("superficie")))
                                    .SetAttribute("veg_cod_agea", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("veg_cod_agea")))
                                    .SetAttribute("cul_cod_agea", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("cul_cod_agea")))

                                    .SetAttribute("data_creazione", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Data_Creazione")))
                                    .SetAttribute("data_modifica", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Data_Modifica")))
                                    .SetAttribute("username_creazione", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Username_Creazione")))
                                    .SetAttribute("username_modifica", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Username_Modifica")))

                                    .SetAttribute("numero_fascicolo", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Numero_Fascicolo")))
                                    .SetAttribute("data_validazione_fascicolo", Agro_SQL_Load(DTUtilizzo.Rows(i_Utilizzo).Item("Data_Validazione_Fascicolo")))
                                End With

                                XmlMacrouso.AppendChild(XmlUtilizzo)

                            Next

                            XmlImpresexParticelle.AppendChild(XmlMacrouso)

                        Next

                    End If


                    XmlDatiImpresexParticelle.AppendChild(XmlImpresexParticelle)
                    XmlImpresexParticelle = Nothing

                Next iIxP

                DtImpresexParticelle = Nothing
                objImpresexParticelle = Nothing




                '#################################
                '#################################
                '#################################




                '----- < / ImpresexParticelle > -----

                XmlDoc.AppendChild(XmlDatiImpresexParticelle)

                RisultatoFunzione = XmlDoc.OuterXml
                '----- < / Documento XML > -----



                XmlDatiImpresexParticelle = Nothing
                XmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessuna particella ...
                RisultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtImpresexParticelle = Nothing
            objImpresexParticelle = Nothing
            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    If Not IsNothing(xConnessione) Then
                        xConnessione.Close()
                    End If
                End If
            End If

        End Try

        Return RisultatoFunzione

    End Function


    Public Function Leggi_Particella_Anagrafica(Piva As String,
                                                Sa_Cod As Integer,
                                                Prov As String,
                                                Com As String,
                                                Sezione As String,
                                                Foglio As Integer,
                                                Numero As Integer,
                                                Subalterno As String,
                                                Leggi_Metodi_Produzione As Boolean,
                                                Leggi_Macrousi As Boolean,
                                                Leggi_Zone As Boolean,
                                                Leggi_Classamento As Boolean,
                                                objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale

        Dim Particella As New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale

        If Piva = "" Then
            Throw New Exception("Piva non impostata")
        End If

        If Sa_Cod = 0 Then
            Throw New Exception("Sa Cod non impostato")
        End If

        If Prov = "" Then
            Throw New Exception("Prov non impostata")
        End If

        If Com = "" Then
            Throw New Exception("Com non impostato")
        End If

        If Foglio = 0 Then
            Throw New Exception("Foglio non impostato")
        End If

        If Numero = 0 Then
            Throw New Exception("Numero non impostato")
        End If

        Dim SezioneChiaveParticella = ""
        Dim SubalternoChiaveParticella = ""

        If Sezione <> "0" Then
            SezioneChiaveParticella = Sezione
        End If

        If Subalterno <> "0" Then
            SubalternoChiaveParticella = Subalterno
        End If

        Particella.centro = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
        Particella.particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali(
            New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(Prov, Com, SezioneChiaveParticella, Foglio, Numero, SubalternoChiaveParticella)
        )

        Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

        Dim objImpresexParticelle_Codici As New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R

        Dim dt = objImpresexParticelle.Leggi3(0, Piva, Sa_Cod, 0, Prov, Com, Sezione, Foglio, Numero, Subalterno, "", "", objParametri)
        Dim i = 0


        Dim possessi As New List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella)

        If dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1
                If i = 0 Then
                    Dim Sup As Double
                    Dim Ha As Integer = dt.Rows(i)("ETTARI")
                    Dim A As Integer = dt.Rows(i)("ARE")
                    Dim Ca As Integer = dt.Rows(i)("CENTIARE")
                    Sup = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ha, A, Ca)
                    Particella.particella.Area = Sup
                End If

                Dim possesso As New AgronicaCoreModelsSTD.anagrafiche.PossessoParticella(dt.Rows(i)("ID"))
                'Case i.TitoloPossesso When 1 Then 'Proprietà' WHEN 2 THEN 'Comodato d''uso' WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 
                'Affitto senza contratto' WHEN 5 THEN 
                'In conto terzi' WHEN 6 THEN 'In convenzione' ELSE 'Altro' END AS 'Titolo_Possesso'
                Dim titoloPossesso_Cod As Integer = dt.Rows(i)("TitoloPossesso")
                Dim titoloPossesso_Desc As String = ""
                Select Case titoloPossesso_Cod
                    Case 1
                        titoloPossesso_Desc = "Proprietà"
                    Case 2
                        titoloPossesso_Desc = "Comodato d''uso"
                    Case 3
                        titoloPossesso_Desc = "Affitto con contratto"
                    Case 4
                        titoloPossesso_Desc = "Affitto senza contratto"
                    Case 5
                        titoloPossesso_Desc = "In conto terzi"
                    Case 6
                        titoloPossesso_Desc = "In convenzione"
                    Case Else
                        titoloPossesso_Desc = "Altro"
                End Select

                possesso.titolo_Di_Possesso = New AgronicaCoreModelsSTD.metaschema.TitoloDiPossesso(titoloPossesso_Cod) With {.descrizione = titoloPossesso_Desc}

                possesso.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(dt.Rows(i)("Validita_Inizio"), dt.Rows(i)("Validita_Fine"))

                possesso.Area = dt.Rows(i)("Sup_Condotta")


                Dim dtCodiceParticella = objImpresexParticelle_Codici.Leggi(dt.Rows(i)("ID"),
                                                   "",
                                                   0,
                                                   "",
                                                   "",
                                                   "",
                                                   0,
                                                   0,
                                                   "",
                                                   enum_CodiciAnagrafe.CodiceParticella,
                                                   "",
                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                   "",
                                                   "",
                                                   objParametri)

                possesso.codice_particella = ""

                If dtCodiceParticella IsNot Nothing AndAlso dtCodiceParticella.Rows.Count > 0 Then

                    possesso.codice_particella = dtCodiceParticella.Rows(0)("Val_Cod")

                End If

                possessi.Add(possesso)
            Next

            Particella.possessiParticella = possessi

            If Leggi_Metodi_Produzione Then
                Particella.particella.metodoProduzione = Leggi_Metodo_Produzione_Particella(Particella.particella.primaryKey.Prov,
                                                                                            Particella.particella.primaryKey.Com,
                                                                                            Particella.particella.primaryKey.Sezione,
                                                                                            Particella.particella.primaryKey.Foglio,
                                                                                            Particella.particella.primaryKey.Numero,
                                                                                            Particella.particella.primaryKey.Subalterno,
                                                                                            objParametri)
            End If

            If Leggi_Macrousi Then
                Particella.particella.macrousi = Leggi_Macrousi_Particella(Particella.particella.primaryKey.Prov,
                                                                                            Particella.particella.primaryKey.Com,
                                                                                            Particella.particella.primaryKey.Sezione,
                                                                                            Particella.particella.primaryKey.Foglio,
                                                                                            Particella.particella.primaryKey.Numero,
                                                                                            Particella.particella.primaryKey.Subalterno,
                                                                                            objParametri)
            End If

            If Leggi_Zone Then
                Particella.particella.zonizzazione = Leggi_Zone_Particella(Particella.particella.primaryKey.Prov,
                                                                                            Particella.particella.primaryKey.Com,
                                                                                            Particella.particella.primaryKey.Sezione,
                                                                                            Particella.particella.primaryKey.Foglio,
                                                                                            Particella.particella.primaryKey.Numero,
                                                                                            Particella.particella.primaryKey.Subalterno,
                                                                                            objParametri)
            End If

            If Leggi_Classamento Then
                Particella.particella.classamento = Leggi_Classamento_Particella(Particella.particella.primaryKey.Prov,
                                                                                            Particella.particella.primaryKey.Com,
                                                                                            Particella.particella.primaryKey.Sezione,
                                                                                            Particella.particella.primaryKey.Foglio,
                                                                                            Particella.particella.primaryKey.Numero,
                                                                                            Particella.particella.primaryKey.Subalterno,
                                                                                            objParametri)
            End If

            Particella.particella.proprietario = If(IsDBNull(dt.Rows(0)("Proprietario")), "", dt.Rows(0)("Proprietario"))

        Else
            Particella = New AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale
        End If

        Return Particella
    End Function

    Public Function Leggi_Metodo_Produzione_Particella(
                                                Prov As String,
                                                Com As String,
                                                Sezione As String,
                                                Foglio As Integer,
                                                Numero As Integer,
                                                Subalterno As String,
                                                objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMetodoProduzione)
        Dim list As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMetodoProduzione)

        Dim objMetodoProduzione_Particella As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_R

        Dim sezioneQuery = Sezione
        Dim subalternoQuery = Subalterno

        If sezioneQuery = "" Then
            sezioneQuery = "0"
        End If

        If subalternoQuery = "" Then
            subalternoQuery = "0"
        End If

        Dim dt = objMetodoProduzione_Particella.Leggi(Prov, Com, sezioneQuery, Foglio, Numero, subalternoQuery, -1, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)
        For Each rows In dt.Rows
            Dim objMetodo As New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMetodoProduzione()
            objMetodo.metodoProduzione = New AgronicaCoreModelsSTD.metaschema.MetodoProduzione(rows("MetodoProduzione_Cod")) With {.descrizione = rows("MetodoProduzione_Des")}

            objMetodo.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(rows("Validita_Inizio"), rows("Validita_Fine"))

            list.Add(objMetodo)
        Next

        Return list
    End Function

    Public Function Leggi_Macrousi_Particella(Prov As String,
                                              Com As String,
                                              Sezione As String,
                                              Foglio As Integer,
                                              Numero As Integer,
                                              Subalterno As String,
                                              objParametri As AgronicaCoreParametri
                                              ) As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMacrouso)

        Dim list As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMacrouso)

        Dim objMacrousi_Particella As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R

        Dim dt = objMacrousi_Particella.Leggi("", Prov, Com, Sezione, Foglio, Numero, Subalterno, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)
        For Each rows In dt.Rows
            Dim objMacrouso As New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMacrouso()
            objMacrouso.Area = 0

            If Not IsDBNull(rows("Superficie")) Then
                objMacrouso.Area = rows("Superficie")
            End If

            objMacrouso.macrouso = New AgronicaCoreModelsSTD.metaschema.Macrouso(rows("Macrouso_Cod")) With {.descrizione = rows("Macrouso_Des")}

            objMacrouso.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(rows("Validita_Inizio"), rows("Validita_Fine"))

            objMacrouso.Piva = rows("Piva")
            objMacrouso.NumeroFascicolo = IIf(IsDBNull(rows("Numero_Fascicolo")), "", rows("Numero_Fascicolo"))
            objMacrouso.DataValidazioneFascicolo = IIf(IsDBNull(rows("Data_validazione_Fascicolo")), AGRODATAINIZIO, rows("Data_validazione_Fascicolo"))

            list.Add(objMacrouso)
        Next

        Return list
    End Function

    Public Function Leggi_Zone_Particella(Prov As String,
                                          Com As String,
                                          Sezione As String,
                                          Foglio As Integer,
                                          Numero As Integer,
                                          Subalterno As String,
                                          objParametri As AgronicaCoreParametri
                                          ) As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona)

        Dim list As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona)

        Dim objZone_Particelle As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R

        Dim dt = objZone_Particelle.Leggi(0, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)
        For Each rows In dt.Rows
            Dim objZona As New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona
            objZona.Area = rows("Area")
            objZona.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale(rows("Validita_Inizio"), rows("Validita_Fine"))
            objZona.zona = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(rows("Zona_Cod"), rows("Descrizione"))

            list.Add(objZona)
        Next

        Return list
    End Function

    Public Function Leggi_Classamento_Particella(Prov As String,
                                                 Com As String,
                                                 Sezione As String,
                                                 Foglio As Integer,
                                                 Numero As Integer,
                                                 Subalterno As String,
                                                 objParametri As AgronicaCoreParametri
                                                 ) As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliClassamento)
        Dim list As New List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliClassamento)

        Dim objClassamento_Particelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

        Dim dt = objClassamento_Particelle.LeggixChiave_conClassamento(Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)
        For Each rows In dt.Rows
            Dim objClassamento As New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliClassamento

            objClassamento.Area = rows("Sup_Classe")
            objClassamento.porzione = rows("Porzione")
            objClassamento.qualita = New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(rows("Qualita_Cod"), rows("Qualita_Des"))
            objClassamento.redditoAgrario = rows("Reddito_Agrario")
            objClassamento.redditoDomiciliare = rows("Reddito_Dominicale")
            objClassamento.classe = rows("Classe")

            list.Add(objClassamento)
        Next

        Return list
    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Particella_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Particella_Scrivi(ByVal DatiParticella As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Numero_Fascicolo As String = "",
                                      Optional ByVal Data_Validazione_Fascicolo As Date = AGRODATAINIZIO,
                                      Optional ByVal NoteLog As String = "Particella_Scrivi XML"
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Particella.Particella_Scrivi()"
        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        Dim debug As Boolean

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        'Dim RisultatoFunzione As String = String.Empty

        Dim Cod_Particella As Integer

        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W

        Dim xZonexParticella As XmlNodeList
        Dim xZonaxParticella As XmlElement
        Dim xParticellaxClassamento As XmlNodeList
        Dim xClassamento As XmlElement
        Dim xParticellaxEleggibilita As XmlNodeList
        Dim xEleggibilita As XmlElement
        Dim xParticellaxMacrousi As XmlNodeList
        Dim xMacrouso As XmlElement
        Dim xParticellaxMacrousixUtilizzi As XmlNodeList
        Dim xUtilizzo As XmlElement
        Dim xDatiCampixParticelle As XmlNodeList
        Dim xDatiCampoxParticella As XmlElement
        Dim xCampixParticelle As XmlNodeList
        Dim xCampoxParticella As XmlElement
        Dim xDatiAppezzaxParticelle As XmlNodeList
        Dim xDatiAppezzaxParticella As XmlElement
        Dim xAppezzaxParticelle As XmlNodeList
        Dim xAppezzaxParticella As XmlElement
        Dim xDatiParticelle As XmlNodeList
        Dim xDatiParticella As XmlElement
        Dim xParticelle As XmlNodeList
        Dim xParticella As XmlElement
        Dim xParticellaPossessi As XmlNodeList
        Dim xParticellaPossesso As XmlElement
        Dim xParticellaMetodiProduzione As XmlNodeList
        Dim xParticellaMetodoProduzione As XmlElement

        Dim i_DatiParticella As Integer
        Dim i_DatiParticelle As Integer
        Dim i_Particelle As Integer
        Dim i_Particella As Integer
        Dim i_Zona As Integer
        Dim i_Macrouso As Integer
        Dim i_Utilizzo As Integer
        Dim i_Classamento As Integer
        Dim i_Eleggibilita As Integer

        Dim OpeDB_Particella As String
        Dim OpeDB_CampoxParticella As String
        Dim OpeDB_AppezzaxParticella As String

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        Dim id_Utilizzo As String

        Dim risp As Boolean
        Dim Dummy As Integer

        Dim wkt As String
        Dim wkt_georiferimento_cod As String

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            '------------------------------
            Dim xmlDoc As New XmlDocument
            'Dim xAgenda As XmlElement
            Dim xEntitaGrafiche As XmlNodeList

            xmlDoc.LoadXml(DatiParticella)

            '------------------------------

            xDatiParticelle = xmlDoc.GetElementsByTagName("DatiParticelle")

            i_Particelle = 0

            Do While i_Particelle < xDatiParticelle.Count

                'Prelevo l'i-esimo blocco di Particelle (in realtà ne esiste uno solo)
                xDatiParticella = xDatiParticelle.Item(i_Particelle)

                '------------------------------

                xParticelle = xDatiParticella.GetElementsByTagName("Particella")

                i_Particella = 0

                Do While i_Particella < xParticelle.Count

                    'Prelevo l' i-esima Particella
                    xParticella = xParticelle.Item(i_Particella)

                    'Prelevo gli attributi della particella selezionata
                    OpeDB_Particella = xParticella.GetAttribute("TipoOperazioneDB")

                    Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_W
                    Dim objParticelle_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
                    Dim objLeggiParticella As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
                    Dim objImpresexParticelle2_W As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
                    Dim objImpresexParticelle2_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
                    Dim objZonexParticelle_W As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
                    Dim objZonexParticelle_R As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
                    Dim objParticellexMacrousi_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W
                    Dim objParticellexMacrousi_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
                    Dim objParticellexMacrousixUtilizzo_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W
                    Dim objParticellexMacrousixUtilizzo_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
                    Dim objParticelleEleggibilita_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_W
                    Dim objParticelleEleggibilita_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_R
                    Dim objParticelle_MetodoProduzione_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_W
                    Dim objParticelle_MetodoProduzione_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_R

                    Dim DtParticella As DataTable

                    Cod_Particella = CLng(xParticella.GetAttribute("part_cod"))

                    'se i possessi sono contenuti nei tag successivi (nuova versione)
                    xParticellaPossessi = xParticella.GetElementsByTagName("Possesso")

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Particella

                        Case "0"    'LEGGI -------------------------------------------------------

                        Case "1"    'SALVA -------------------------------------------------------

                            'Prima di salvare controllo che nessun altro utente abbia già inserito
                            'la stessa particella in precedenza.

                            DtParticella = objLeggiParticella.LeggixChiave(
                                     CStr(xParticella.GetAttribute("prov")),
                                     CStr(xParticella.GetAttribute("com")),
                                     CStr(xParticella.GetAttribute("sezione")),
                                     CLng(xParticella.GetAttribute("foglio")),
                                     CLng(xParticella.GetAttribute("numero")),
                                     CStr(xParticella.GetAttribute("subalterno")),
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri)

                            Dim particella_data_creazione As Date = #2/1/1900#
                            Dim particella_data_Modifica As Date = #2/1/1900#
                            Dim particella_username_creazione As String = ""
                            Dim particella_username_modifica As String = ""

                            If Not IsNothing(xParticella.GetAttribute("data_creazione")) AndAlso
                                xParticella.GetAttribute("data_creazione") <> "" Then
                                particella_data_creazione = CDate(xParticella.GetAttribute("data_creazione"))
                            End If

                            If Not IsNothing(xParticella.GetAttribute("data_modifica")) AndAlso
                                xParticella.GetAttribute("data_modifica") <> "" Then
                                particella_data_Modifica = CDate(xParticella.GetAttribute("data_modifica"))
                            End If

                            If Not IsNothing(xParticella.GetAttribute("username_creazione")) Then
                                particella_username_creazione = CStr(xParticella.GetAttribute("username_creazione"))
                            End If

                            If Not IsNothing(xParticella.GetAttribute("username_modifica")) Then
                                particella_username_modifica = CStr(xParticella.GetAttribute("username_modifica"))
                            End If

                            If DtParticella.Rows.Count = 0 Then

                                If Cod_Particella <= 0 Then

                                    Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                                    Cod_Particella = objSequenze.NuovoId_Tabella("ParticelleCatastali",
                                                CLng(xParticella.GetAttribute("basecode")),
                                                CLng(xParticella.GetAttribute("topcode")),
                                                objParametri)
                                    objSequenze = Nothing

                                Else

                                    'Esportazione in Locale

                                End If

                                Dummy = objParticelle.Scrivi(
                                         CLng(Cod_Particella),
                                         CStr(xParticella.GetAttribute("prov")),
                                         CStr(xParticella.GetAttribute("com")),
                                         CStr(xParticella.GetAttribute("sezione")),
                                         CLng(xParticella.GetAttribute("foglio")),
                                         CLng(xParticella.GetAttribute("numero")),
                                         CStr(xParticella.GetAttribute("subalterno")),
                                         CStr(xParticella.GetAttribute("partita_catastale")),
                                         CDbl(xParticella.GetAttribute("ettari")),
                                         CLng(xParticella.GetAttribute("are")),
                                         CLng(xParticella.GetAttribute("centiare")),
                                         CLng(xParticella.GetAttribute("qualita_cod")),
                                         CStr(xParticella.GetAttribute("classe")),
                                         Val(Agro_SQL_SaveNum(xParticella.GetAttribute("reddito_dominicale"), False)),
                                         Val(Agro_SQL_SaveNum(xParticella.GetAttribute("reddito_agrario"), False)),
                                         CInt(xParticella.GetAttribute("titolopossesso")),
                                         CDate(xParticella.GetAttribute("validita_inizio")),
                                         CDate(xParticella.GetAttribute("validita_fine")),
                                         objParametri,
                                         particella_data_creazione,
                                         particella_data_Modifica,
                                         particella_username_creazione,
                                         particella_username_modifica
                                    )
                            End If

                            'Prima di salvare controllo che non esista già l'associazione ImpreseXParticelle.
                            If CInt(xParticella.GetAttribute("sa_cod")) = 0 Then
                                'errore, non deve succedere!
                                debug = True
                            Else

                                DtParticella = objImpresexParticelle2_R.LeggixChiave(0,
                                                                                     CStr(xParticella.GetAttribute("piva")),
                                                                                     CInt(xParticella.GetAttribute("sa_cod")),
                                                                                     CStr(xParticella.GetAttribute("prov")),
                                                                                     CStr(xParticella.GetAttribute("com")),
                                                                                     CStr(xParticella.GetAttribute("sezione")),
                                                                                     CInt(xParticella.GetAttribute("foglio")),
                                                                                     CInt(xParticella.GetAttribute("numero")),
                                                                                     CStr(xParticella.GetAttribute("subalterno")),
                                                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                     "",
                                                                                     "",
                                                                                     objParametri)

                                If DtParticella.Rows.Count = 0 Then

                                    If xParticellaPossessi.Count = 0 Then
                                        '    If xParticellaPossessi Is Nothing Then
                                        risp = objImpresexParticelle2_W.Scrivi_2(
                                                             CStr(xParticella.GetAttribute("piva")),
                                                             CInt(xParticella.GetAttribute("sa_cod")),
                                                             CStr(xParticella.GetAttribute("prov")),
                                                             CStr(xParticella.GetAttribute("com")),
                                                             CStr(xParticella.GetAttribute("sezione")),
                                                             CInt(xParticella.GetAttribute("foglio")),
                                                             CInt(xParticella.GetAttribute("numero")),
                                                             CStr(xParticella.GetAttribute("subalterno")),
                                                             CStr(xParticella.GetAttribute("partita_catastale")),
                                                             CInt(xParticella.GetAttribute("titolopossesso")),
                                                             CDbl(xParticella.GetAttribute("sup_condotta")),
                                                             CDate(xParticella.GetAttribute("validita_inizio_centro")),
                                                             CDate(xParticella.GetAttribute("validita_fine_centro")),
                                                             objParametri,
                                                             particella_data_creazione,
                                                             particella_data_Modifica,
                                                             particella_username_creazione,
                                                             particella_username_modifica,
                                                             Sup_Spandibile:=Agro_XML_GetDecimal(xParticella, "sup_spandibile", 0),
                                                             Sup_Divieto:=Agro_XML_GetDecimal(xParticella, "sup_divieto", 0)
                                                             )
                                    End If

                                End If

                            End If 'sa_cod=0

                            '24/07/2018: drudi, se esiste un dato cartografico allora lo importo (es.: artea).

                            If xParticella.HasAttribute("wkt") Then
                                If xParticella.GetAttribute("wkt") = "" Then
                                    wkt = ""
                                    wkt_georiferimento_cod = "-1"
                                Else
                                    wkt = xParticella.GetAttribute("wkt")
                                    wkt_georiferimento_cod = xParticella.GetAttribute("wkt_georiferimento_cod")
                                End If
                            Else
                                wkt = ""
                                wkt_georiferimento_cod = "-1"
                            End If

                            objParametri = ParticellaScriviDatoCartografico(objParametri,
                                                                            CStr(xParticella.GetAttribute("prov")),
                                                                            CStr(xParticella.GetAttribute("com")),
                                                                            CStr(xParticella.GetAttribute("sezione")),
                                                                            CInt(xParticella.GetAttribute("foglio")),
                                                                            CInt(xParticella.GetAttribute("numero")),
                                                                            CStr(xParticella.GetAttribute("subalterno")),
                                                                            CStr(xParticella.GetAttribute("piva")),
                                                                            CInt(xParticella.GetAttribute("sa_cod")),
                                                                            wkt,
                                                                            wkt_georiferimento_cod,
                                                                            ScriviElementiGrafici,
                                                                            objParametri)


                            '16/07/2018: drudi, FINE se esiste un dato cartografico allora lo importo (es.: artea).

                            '######################################################

                        Case "2"    'MODIFICA -------------------------------------------------------

                            objParticelle.Modifica_2(
                                        CStr(xParticella.GetAttribute("prov")),
                                        CStr(xParticella.GetAttribute("com")),
                                        CStr(xParticella.GetAttribute("sezione")),
                                        CInt(xParticella.GetAttribute("foglio")),
                                        CInt(xParticella.GetAttribute("numero")),
                                        CStr(xParticella.GetAttribute("subalterno")),
                                        CStr(xParticella.GetAttribute("partita_catastale")),
                                        CDbl(xParticella.GetAttribute("ettari")),
                                        CInt(xParticella.GetAttribute("are")),
                                        CInt(xParticella.GetAttribute("centiare")),
                                        CInt(xParticella.GetAttribute("qualita_cod")),
                                        CStr(xParticella.GetAttribute("classe")),
                                        Val(Agro_SQL_SaveNum(xParticella.GetAttribute("reddito_dominicale"), False)),
                                        Replace(xParticella.GetAttribute("reddito_agrario"), ".", ","),
                                        CInt(xParticella.GetAttribute("titolopossesso")),
                                        CDate(xParticella.GetAttribute("validita_inizio")),
                                        CDate(xParticella.GetAttribute("validita_fine")),
                                        "",
                                        objParametri)


                            If xParticellaPossessi.Count = 0 Then
                                '    If xParticellaPossessi Is Nothing Then

                                objImpresexParticelle2_W.Modifica_2(0,
                                                                    CStr(xParticella.GetAttribute("piva")),
                                                                    CInt(xParticella.GetAttribute("sa_cod")),
                                                                    CStr(xParticella.GetAttribute("prov")),
                                                                    CStr(xParticella.GetAttribute("com")),
                                                                    CStr(xParticella.GetAttribute("sezione")),
                                                                    CInt(xParticella.GetAttribute("foglio")),
                                                                    CInt(xParticella.GetAttribute("numero")),
                                                                    CStr(xParticella.GetAttribute("subalterno")),
                                                                    CStr(xParticella.GetAttribute("partita_catastale")),
                                                                    CInt(xParticella.GetAttribute("titolopossesso")),
                                                                    CDbl(xParticella.GetAttribute("sup_condotta")),
                                                                    CDate(xParticella.GetAttribute("validita_inizio_centro")),
                                                                    CDate(xParticella.GetAttribute("validita_fine_centro")),
                                                                    "",
                                                                    objParametri,
                                                                    Sup_Spandibile:=If(Not xParticella.HasAttribute("sup_spandibile"), Nothing, CDbl(xParticella.GetAttribute("sup_spandibile"))),
                                                                    Sup_Divieto:=If(Not xParticella.HasAttribute("sup_divieto"), Nothing, CDbl(xParticella.GetAttribute("sup_divieto")))
                                                                    )
                            End If

                    End Select

                    '-------------------------------------------------------------
                    ' POSSESSI
                    '-------------------------------------------------------------

                    xParticellaPossessi = xParticella.GetElementsByTagName("Possesso")

                    Dim i_Possesso As Integer = 0

                    'Dim objImpresexParticelle2_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

                    If xParticellaPossessi IsNot Nothing Then

                        Do While i_Possesso < xParticellaPossessi.Count

                            xParticellaPossesso = xParticellaPossessi.Item(i_Possesso)

                            Select Case CInt(xParticellaPossesso.GetAttribute("TipoOperazioneDB"))

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dim dtPossessi = objImpresexParticelle2_R.Leggi(0,
                                                                                CStr(xParticella.GetAttribute("piva")),
                                                                                CInt(xParticella.GetAttribute("sa_cod")),
                                                                                0,
                                                                                CStr(xParticella.GetAttribute("prov")),
                                                                                CStr(xParticella.GetAttribute("com")),
                                                                                CStr(xParticella.GetAttribute("sezione")),
                                                                                CInt(xParticella.GetAttribute("foglio")),
                                                                                CInt(xParticella.GetAttribute("numero")),
                                                                                CStr(xParticella.GetAttribute("subalterno")),
                                                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "",
                                                                                "",
                                                                                objParametri)

                                    Dim newStart As Date = CDate(xParticellaPossesso.GetAttribute("validita_inizio"))
                                    Dim newEnd As Date = CDate(xParticellaPossesso.GetAttribute("validita_fine"))
                                    Dim sovrapposto As Boolean = False

                                    For Each row As DataRow In dtPossessi.Rows
                                        Dim oldStart As Date = row("Validita_Inizio")
                                        Dim oldEnd As Date = row("Validita_Fine")
                                        If (newStart <= oldEnd) AndAlso (oldStart <= newEnd) Then
                                            ' Sovrapposizione trovata: aggiorna il record esistente
                                            Dim minStart As Date = If(newStart < oldStart, newStart, oldStart)
                                            Dim maxEnd As Date = If(newEnd > oldEnd, newEnd, oldEnd)
                                            objImpresexParticelle2_W.Modifica_2(
                                                                                row("ID"),
                                                                                CStr(xParticella.GetAttribute("piva")),
                                                                                CInt(xParticella.GetAttribute("sa_cod")),
                                                                                CStr(xParticella.GetAttribute("prov")),
                                                                                CStr(xParticella.GetAttribute("com")),
                                                                                CStr(xParticella.GetAttribute("sezione")),
                                                                                CInt(xParticella.GetAttribute("foglio")),
                                                                                CInt(xParticella.GetAttribute("numero")),
                                                                                CStr(xParticella.GetAttribute("subalterno")),
                                                                                CStr(xParticellaPossesso.GetAttribute("partita_catastale")),
                                                                                CInt(xParticellaPossesso.GetAttribute("titolopossesso")),
                                                                                CDbl(xParticellaPossesso.GetAttribute("sup_condotta")),
                                                                                minStart,
                                                                                maxEnd,
                                                                                "",
                                                                                objParametri,
                                                                                Sup_Spandibile:=If(Not xParticellaPossesso.HasAttribute("sup_spandibile"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_spandibile"))),
                                                                                Sup_Divieto:=If(Not xParticellaPossesso.HasAttribute("sup_divieto"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_divieto")))
                                                                            )
                                            objLogAnagrafeW.Scrivi(2,
                                                                enum_TipoEntita_Des.ImpreseXParticelle,
                                                                CStr(xParticella.GetAttribute("piva")),
                                                                CStr(xParticella.GetAttribute("sa_cod")),
                                                                CStr("ID:" & row("ID")),
                                                                CStr("PROV:" & xParticella.GetAttribute("prov") & " - COM:" & xParticella.GetAttribute("com")),
                                                                CStr("SEZ:" & CStr(xParticella.GetAttribute("sezione")) & " - FOGLIO:" & CStr(xParticella.GetAttribute("foglio")) & " - NUM:" & CStr(xParticella.GetAttribute("numero"))),
                                                                "", NoteLog & " Modifica per date sovrapposte",
                                                                enum_Id_Servizio.GiasOnline,
                                                                objParametri, "particella: " & xParticella.OuterXml & vbCrLf & "Possesso: " & xParticellaPossesso.OuterXml)
                                            sovrapposto = True
                                            Exit For
                                        End If
                                    Next

                                    If Not sovrapposto Then
                                        risp = objImpresexParticelle2_W.Scrivi_2(CStr(xParticella.GetAttribute("piva")),
                                                                                 CInt(xParticella.GetAttribute("sa_cod")),
                                                                                 CStr(xParticella.GetAttribute("prov")),
                                                                                 CStr(xParticella.GetAttribute("com")),
                                                                                 CStr(xParticella.GetAttribute("sezione")),
                                                                                 CInt(xParticella.GetAttribute("foglio")),
                                                                                 CInt(xParticella.GetAttribute("numero")),
                                                                                 CStr(xParticella.GetAttribute("subalterno")),
                                                                                 CStr(xParticellaPossesso.GetAttribute("partita_catastale")),
                                                                                 CInt(xParticellaPossesso.GetAttribute("titolopossesso")),
                                                                                 CDbl(xParticellaPossesso.GetAttribute("sup_condotta")),
                                                                                 CDate(xParticellaPossesso.GetAttribute("validita_inizio")),
                                                                                 CDate(xParticellaPossesso.GetAttribute("validita_fine")),
                                                                                 objParametri,
                                                                                 Sup_Spandibile:=Agro_XML_GetDecimal(xParticellaPossesso, "sup_spandibile", 0),
                                                                                 Sup_Divieto:=Agro_XML_GetDecimal(xParticellaPossesso, "sup_divieto", 0)
                                                                                 )
                                        'Scrittura tabella Agronica_Log_Anagrafe
                                        objLogAnagrafeW.Scrivi(1,
                                                                enum_TipoEntita_Des.ImpreseXParticelle,
                                                                CStr(xParticella.GetAttribute("piva")),
                                                                CStr(xParticella.GetAttribute("sa_cod")),
                                                                CStr("ID:" & 0),
                                                                CStr("PROV:" & xParticella.GetAttribute("prov") & " - COM:" & xParticella.GetAttribute("com")),
                                                                CStr("SEZ:" & CStr(xParticella.GetAttribute("sezione")) & " - FOGLIO:" & CStr(xParticella.GetAttribute("foglio")) & " - NUM:" & CStr(xParticella.GetAttribute("numero"))),
                                                                "", NoteLog & " Nuovo Possesso su particella già esistente (date non sovrapposte)",
                                                                enum_Id_Servizio.GiasOnline,
                                                                objParametri, "particella: " & xParticella.OuterXml & vbCrLf & "Possesso: " & xParticellaPossesso.OuterXml)
                                    End If



                                Case "2"    'MODIFICA -------------------------------------------------------


                                    Dim dtPossessi = objImpresexParticelle2_R.Leggi(0,
                                                                                CStr(xParticella.GetAttribute("piva")),
                                                                                CInt(xParticella.GetAttribute("sa_cod")),
                                                                                0,
                                                                                CStr(xParticella.GetAttribute("prov")),
                                                                                CStr(xParticella.GetAttribute("com")),
                                                                                CStr(xParticella.GetAttribute("sezione")),
                                                                                CInt(xParticella.GetAttribute("foglio")),
                                                                                CInt(xParticella.GetAttribute("numero")),
                                                                                CStr(xParticella.GetAttribute("subalterno")),
                                                                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                "",
                                                                                "",
                                                                                objParametri)

                                    Dim newStart As Date = CDate(xParticellaPossesso.GetAttribute("validita_inizio"))
                                    Dim newEnd As Date = CDate(xParticellaPossesso.GetAttribute("validita_fine"))
                                    Dim sovrapposto As Boolean = False

                                    For Each row As DataRow In dtPossessi.Rows
                                        Dim oldStart As Date = row("Validita_Inizio")
                                        Dim oldEnd As Date = row("Validita_Fine")
                                        If (newStart <= oldEnd) AndAlso (oldStart <= newEnd) Then
                                            ' Sovrapposizione trovata: aggiorna il record esistente
                                            Dim minStart As Date = If(newStart < oldStart, newStart, oldStart)
                                            Dim maxEnd As Date = If(newEnd > oldEnd, newEnd, oldEnd)
                                            objImpresexParticelle2_W.Modifica_2(
                                                                                row("ID"),
                                                                                CStr(xParticella.GetAttribute("piva")),
                                                                                CInt(xParticella.GetAttribute("sa_cod")),
                                                                                CStr(xParticella.GetAttribute("prov")),
                                                                                CStr(xParticella.GetAttribute("com")),
                                                                                CStr(xParticella.GetAttribute("sezione")),
                                                                                CInt(xParticella.GetAttribute("foglio")),
                                                                                CInt(xParticella.GetAttribute("numero")),
                                                                                CStr(xParticella.GetAttribute("subalterno")),
                                                                                CStr(xParticellaPossesso.GetAttribute("partita_catastale")),
                                                                                CInt(xParticellaPossesso.GetAttribute("titolopossesso")),
                                                                                CDbl(xParticellaPossesso.GetAttribute("sup_condotta")),
                                                                                minStart,
                                                                                maxEnd,
                                                                                "",
                                                                                objParametri,
                                                                                Sup_Spandibile:=If(Not xParticellaPossesso.HasAttribute("sup_spandibile"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_spandibile"))),
                                                                                Sup_Divieto:=If(Not xParticellaPossesso.HasAttribute("sup_divieto"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_divieto")))
                                                                            )
                                            objLogAnagrafeW.Scrivi(2,
                                                                enum_TipoEntita_Des.ImpreseXParticelle,
                                                                CStr(xParticella.GetAttribute("piva")),
                                                                CStr(xParticella.GetAttribute("sa_cod")),
                                                                CStr("ID:" & row("ID")),
                                                                CStr("PROV:" & xParticella.GetAttribute("prov") & " - COM:" & xParticella.GetAttribute("com")),
                                                                CStr("SEZ:" & CStr(xParticella.GetAttribute("sezione")) & " - FOGLIO:" & CStr(xParticella.GetAttribute("foglio")) & " - NUM:" & CStr(xParticella.GetAttribute("numero"))),
                                                                "", NoteLog & " Modifica",
                                                                enum_Id_Servizio.GiasOnline,
                                                                objParametri, "particella: " & xParticella.OuterXml & vbCrLf & "Possesso: " & xParticellaPossesso.OuterXml)
                                            sovrapposto = True
                                            Exit For
                                        End If
                                    Next

                                    'objImpresexParticelle2_W.Modifica_2(0,
                                    '                                    CStr(xParticella.GetAttribute("piva")),
                                    '                                    CInt(xParticella.GetAttribute("sa_cod")),
                                    '                                    CStr(xParticella.GetAttribute("prov")),
                                    '                                    CStr(xParticella.GetAttribute("com")),
                                    '                                    CStr(xParticella.GetAttribute("sezione")),
                                    '                                    CInt(xParticella.GetAttribute("foglio")),
                                    '                                    CInt(xParticella.GetAttribute("numero")),
                                    '                                    CStr(xParticella.GetAttribute("subalterno")),
                                    '                                    CStr(xParticellaPossesso.GetAttribute("partita_catastale")),
                                    '                                    CInt(xParticellaPossesso.GetAttribute("titolopossesso")),
                                    '                                    CDbl(xParticellaPossesso.GetAttribute("sup_condotta")),
                                    '                                    CDate(xParticellaPossesso.GetAttribute("validita_inizio")),
                                    '                                    CDate(xParticellaPossesso.GetAttribute("validita_fine")),
                                    '                                    "",
                                    '                                    objParametri,
                                    '                                    Sup_Spandibile:=If(Not xParticellaPossesso.HasAttribute("sup_spandibile"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_spandibile"))),
                                    '                                    Sup_Divieto:=If(Not xParticellaPossesso.HasAttribute("sup_divieto"), Nothing, CDbl(xParticellaPossesso.GetAttribute("sup_divieto")))
                                    '                                    )

                                    'objLogAnagrafeW.Scrivi(2,
                                    '                            enum_TipoEntita_Des.ImpreseXParticelle,
                                    '                            CStr(xParticella.GetAttribute("piva")),
                                    '                            CStr(xParticella.GetAttribute("sa_cod")),
                                    '                            CStr("ID:" & 0),
                                    '                            CStr("PROV:" & xParticella.GetAttribute("prov") & " - COM:" & xParticella.GetAttribute("com")),
                                    '                            CStr("SEZ:" & CStr(xParticella.GetAttribute("sezione")) & " - FOGLIO:" & CStr(xParticella.GetAttribute("foglio")) & " - NUM:" & CStr(xParticella.GetAttribute("numero"))),
                                    '                            "", NoteLog & " Modifica possesso particella esistente ",
                                    '                            enum_Id_Servizio.GiasOnline,
                                    '                            objParametri, "particella: " & xParticella.OuterXml & vbCrLf & "Possesso: " & xParticellaPossesso.OuterXml)

                            End Select

                            i_Possesso += 1

                        Loop

                    End If



                    '-------------------------------------------------------------
                    ' ZONEXPARTICELLE
                    '-------------------------------------------------------------

                    xZonexParticella = xParticella.GetElementsByTagName("Zona")

                    i_Zona = 0

                    If xZonexParticella IsNot Nothing Then

                        Do While i_Zona < xZonexParticella.Count

                            xZonaxParticella = xZonexParticella.Item(i_Zona)

                            Dim operazioneZonaxParticella As Integer

                            Dim dtZona = objZonexParticelle_R.Leggi(CInt(xZonaxParticella.GetAttribute("zona_cod")),
                                                       CStr(xZonaxParticella.GetAttribute("prov")),
                                                       CStr(xZonaxParticella.GetAttribute("com")),
                                                       CStr(xZonaxParticella.GetAttribute("sezione")),
                                                       CInt(xZonaxParticella.GetAttribute("foglio")),
                                                       CInt(xZonaxParticella.GetAttribute("numero")),
                                                       CStr(xZonaxParticella.GetAttribute("subalterno")),
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                            If dtZona.Rows.Count > 0 Then
                                operazioneZonaxParticella = 2
                            Else
                                operazioneZonaxParticella = 1
                            End If

                            Dim Validita_Inizio_Zona As Date = AGRODATAINIZIO
                            Dim Validita_Fine_Zona As Date = AGRODATAFINE

                            If (xZonaxParticella.HasAttribute("validita_inizio") AndAlso IsDate(xZonaxParticella.GetAttribute("validita_inizio"))) Then
                                Validita_Inizio_Zona = CDate(xZonaxParticella.GetAttribute("validita_inizio"))
                            End If

                            If (xZonaxParticella.HasAttribute("validita_fine") AndAlso IsDate(xZonaxParticella.GetAttribute("validita_fine"))) Then
                                Validita_Fine_Zona = CDate(xZonaxParticella.GetAttribute("validita_fine"))
                            End If


                            Select Case operazioneZonaxParticella

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = objZonexParticelle_W.Scrivi(CInt(xZonaxParticella.GetAttribute("zona_cod")),
                                                            CStr(xZonaxParticella.GetAttribute("prov")),
                                                            CStr(xZonaxParticella.GetAttribute("com")),
                                                            CStr(xZonaxParticella.GetAttribute("sezione")),
                                                            CInt(xZonaxParticella.GetAttribute("foglio")),
                                                            CInt(xZonaxParticella.GetAttribute("numero")),
                                                            CStr(xZonaxParticella.GetAttribute("subalterno")),
                                                            CDbl(xZonaxParticella.GetAttribute("area")),
                                                            Validita_Inizio_Zona, Validita_Fine_Zona, objParametri)


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Dummy = objZonexParticelle_W.Modifica(CInt(xZonaxParticella.GetAttribute("zona_cod")),
                                                        CStr(xZonaxParticella.GetAttribute("prov")),
                                                        CStr(xZonaxParticella.GetAttribute("com")),
                                                        CStr(xZonaxParticella.GetAttribute("sezione")),
                                                        CInt(xZonaxParticella.GetAttribute("foglio")),
                                                        CInt(xZonaxParticella.GetAttribute("numero")),
                                                        CStr(xZonaxParticella.GetAttribute("subalterno")),
                                                        CDbl(xZonaxParticella.GetAttribute("area")),
                                                        Validita_Inizio_Zona, Validita_Fine_Zona,
                                                        objParametri)

                            End Select

                            i_Zona += 1

                        Loop

                    End If


                    '###############################################################
                    '#####################     MACROUSI    #########################
                    '###############################################################



                    xParticellaxMacrousi = xParticella.GetElementsByTagName("Macrouso")

                    i_Macrouso = 0

                    If xParticellaxMacrousi IsNot Nothing Then

                        Do While i_Macrouso < xParticellaxMacrousi.Count

                            xMacrouso = xParticellaxMacrousi.Item(i_Macrouso)

                            Dim numFasc As String = ""
                            If Numero_Fascicolo <> "" Then
                                numFasc = Numero_Fascicolo
                            Else
                                numFasc = Agro_XML_GetString(xMacrouso, "numero_fascicolo", "")
                            End If

                            Dim dataFasc As Date = AGRODATAINIZIO
                            If Data_Validazione_Fascicolo <> AGRODATAINIZIO Then
                                dataFasc = Data_Validazione_Fascicolo
                            Else
                                dataFasc = Agro_XML_GetDate(xMacrouso, "data_validazione_fascicolo", AGRODATAINIZIO)
                            End If

                            Dim operazioneMacrousoxParticella As Integer

                            Dim dtMacrousi = objParticellexMacrousi_R.Leggi(CStr(xMacrouso.GetAttribute("piva")),
                                                                            CStr(xMacrouso.GetAttribute("prov")),
                                                                            CStr(xMacrouso.GetAttribute("com")),
                                                                            CStr(xMacrouso.GetAttribute("sezione")),
                                                                            CLng(xMacrouso.GetAttribute("foglio")),
                                                                            CLng(xMacrouso.GetAttribute("numero")),
                                                                            CStr(xMacrouso.GetAttribute("subalterno")),
                                                                            CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "", "", objParametri, Numero_Fascicolo, dataFasc)

                            If dtMacrousi.Rows.Count > 0 Then
                                operazioneMacrousoxParticella = 2
                            Else
                                operazioneMacrousoxParticella = 1
                            End If

                            Select Case operazioneMacrousoxParticella

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = objParticellexMacrousi_W.Scrivi(CStr(xMacrouso.GetAttribute("piva")),
                                                                            CStr(xMacrouso.GetAttribute("prov")),
                                                                            CStr(xMacrouso.GetAttribute("com")),
                                                                            CStr(xMacrouso.GetAttribute("sezione")),
                                                                            CLng(xMacrouso.GetAttribute("foglio")),
                                                                            CLng(xMacrouso.GetAttribute("numero")),
                                                                            CStr(xMacrouso.GetAttribute("subalterno")),
                                                                            CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                                            CDbl(xMacrouso.GetAttribute("superficie")),
                                                                            Agro_XML_GetDate(xMacrouso, "validita_inizio", AGRODATAINIZIO),
                                                                            Agro_XML_GetDate(xMacrouso, "validita_fine", AGRODATAFINE),
                                                                            objParametri,
                                                                            Numero_Fascicolo:=numFasc,
                                                                            Data_Validazione_Fascicolo:=dataFasc
                                                                            )

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Dummy = objParticellexMacrousi_W.Modifica(CStr(xMacrouso.GetAttribute("piva")),
                                                                              CStr(xMacrouso.GetAttribute("prov")),
                                                                              CStr(xMacrouso.GetAttribute("com")),
                                                                              CStr(xMacrouso.GetAttribute("sezione")),
                                                                              CLng(xMacrouso.GetAttribute("foglio")),
                                                                              CLng(xMacrouso.GetAttribute("numero")),
                                                                              CStr(xMacrouso.GetAttribute("subalterno")),
                                                                              CStr(xMacrouso.GetAttribute("macrouso_cod")),
                                                                              CDbl(xMacrouso.GetAttribute("superficie")),
                                                                              Agro_XML_GetDate(xMacrouso, "validita_inizio", AGRODATAINIZIO),
                                                                              Agro_XML_GetDate(xMacrouso, "validita_fine", AGRODATAFINE),
                                                                              "",
                                                                              objParametri,
                                                                              Numero_Fascicolo:=If(Not xMacrouso.HasAttribute("numero_fascicolo"), Nothing, CStr(xMacrouso.GetAttribute("numero_fascicolo"))),
                                                                              Data_Validazione_Fascicolo:=Agro_XML_GetDate(xMacrouso, "data_validazione_fascicolo", AGRODATAINIZIO)
                                                                              )

                            End Select


                            '###############################################################
                            '#####################     UTILIZZI    #########################
                            '###############################################################

                            xParticellaxMacrousixUtilizzi = xMacrouso.GetElementsByTagName("Utilizzo")

                            i_Utilizzo = 0

                            If xParticellaxMacrousixUtilizzi IsNot Nothing Then

                                Do While i_Utilizzo < xParticellaxMacrousixUtilizzi.Count

                                    xUtilizzo = xParticellaxMacrousixUtilizzi.Item(i_Utilizzo)

                                    id_Utilizzo = Agro_XML_GetString(xUtilizzo, "id", "")

                                    numFasc = ""
                                    If Numero_Fascicolo <> "" Then
                                        numFasc = Numero_Fascicolo
                                    Else
                                        numFasc = Agro_XML_GetString(xMacrouso, "numero_fascicolo", "")
                                    End If

                                    dataFasc = AGRODATAINIZIO
                                    If Data_Validazione_Fascicolo <> AGRODATAINIZIO Then
                                        dataFasc = Data_Validazione_Fascicolo
                                    Else
                                        dataFasc = Agro_XML_GetDate(xMacrouso, "data_validazione_fascicolo", AGRODATAINIZIO)
                                    End If

                                    Dim operazioneMacrousoxutilizzoxParticella As Integer

                                    Dim dtMacrousixutilizzi = objParticellexMacrousixUtilizzo_R.Leggi(CStr(xUtilizzo.GetAttribute("piva")),
                                                                                             CStr(xUtilizzo.GetAttribute("prov")),
                                                                                             CStr(xUtilizzo.GetAttribute("com")),
                                                                                             CStr(xUtilizzo.GetAttribute("sezione")),
                                                                                             CLng(xUtilizzo.GetAttribute("foglio")),
                                                                                             CLng(xUtilizzo.GetAttribute("numero")),
                                                                                             CStr(xUtilizzo.GetAttribute("subalterno")),
                                                                                             CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                                                             CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                                                             CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                             "", "", objParametri, Numero_Fascicolo, dataFasc)

                                    If dtMacrousixutilizzi.Rows.Count > 0 Then
                                        operazioneMacrousoxutilizzoxParticella = 2
                                    Else
                                        operazioneMacrousoxutilizzoxParticella = 1
                                    End If

                                    Select Case operazioneMacrousoxutilizzoxParticella

                                        Case "0"    'LEGGI -------------------------------------------------------

                                        Case "1"    'SALVA -------------------------------------------------------

                                            Dummy = objParticellexMacrousixUtilizzo_W.Scrivi(CStr(xUtilizzo.GetAttribute("piva")),
                                                                                             CStr(xUtilizzo.GetAttribute("prov")),
                                                                                             CStr(xUtilizzo.GetAttribute("com")),
                                                                                             CStr(xUtilizzo.GetAttribute("sezione")),
                                                                                             CLng(xUtilizzo.GetAttribute("foglio")),
                                                                                             CLng(xUtilizzo.GetAttribute("numero")),
                                                                                             CStr(xUtilizzo.GetAttribute("subalterno")),
                                                                                             CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                                                             CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                                                             CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                                                             CDbl(xUtilizzo.GetAttribute("superficie")),
                                                                                             Agro_XML_GetDate(xUtilizzo, "validita_inizio", AGRODATAINIZIO),
                                                                                             Agro_XML_GetDate(xUtilizzo, "validita_fine", AGRODATAFINE),
                                                                                             objParametri,
                                                                                             Numero_Fascicolo:=numFasc,
                                                                                             Data_Validazione_Fascicolo:=dataFasc)

                                        Case "2"    'MODIFICA -------------------------------------------------------

                                            Dim FiltroUtilizzo As String = ""
                                            If id_Utilizzo <> "" AndAlso IsNumeric(id_Utilizzo) Then
                                                FiltroUtilizzo = " ID = " & id_Utilizzo
                                            End If
                                            'Dummy = objParticellexMacrousixUtilizzo_W.Modifica(CStr(xUtilizzo.GetAttribute("piva")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("prov")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("com")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("sezione")), _
                                            '                                                   CLng(xUtilizzo.GetAttribute("foglio")), _
                                            '                                                   CLng(xUtilizzo.GetAttribute("numero")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("subalterno")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("macrouso_cod")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("veg_cod_agea")), _
                                            '                                                   CStr(xUtilizzo.GetAttribute("cul_cod_agea")), _
                                            '                                                   CDbl(xUtilizzo.GetAttribute("superficie")), _
                                            '                                                   Agro_XML_GetDate(xUtilizzo, "validita_inizio", AGRODATAINIZIO), _
                                            '                                                   Agro_XML_GetDate(xUtilizzo, "validita_fine", AGRODATAFINE), _
                                            '                                                   FiltroUtilizzo, _
                                            '                                                   objParametri)
                                            Dummy = objParticellexMacrousixUtilizzo_W.Modifica_con_Fascicolo(CStr(xUtilizzo.GetAttribute("piva")),
                                                                                                             CStr(xUtilizzo.GetAttribute("prov")),
                                                                                                             CStr(xUtilizzo.GetAttribute("com")),
                                                                                                             CStr(xUtilizzo.GetAttribute("sezione")),
                                                                                                             CLng(xUtilizzo.GetAttribute("foglio")),
                                                                                                             CLng(xUtilizzo.GetAttribute("numero")),
                                                                                                             CStr(xUtilizzo.GetAttribute("subalterno")),
                                                                                                             numFasc,
                                                                                                             CStr(xUtilizzo.GetAttribute("macrouso_cod")),
                                                                                                             CStr(xUtilizzo.GetAttribute("veg_cod_agea")),
                                                                                                             CStr(xUtilizzo.GetAttribute("cul_cod_agea")),
                                                                                                             CDbl(xUtilizzo.GetAttribute("superficie")),
                                                                                                             Agro_XML_GetDate(xUtilizzo, "validita_inizio", AGRODATAINIZIO),
                                                                                                             Agro_XML_GetDate(xUtilizzo, "validita_fine", AGRODATAFINE),
                                                                                                             FiltroUtilizzo,
                                                                                                             objParametri)

                                    End Select

                                    i_Utilizzo += 1

                                Loop

                            End If

                            i_Macrouso += 1

                        Loop

                    End If

                    '-------------------------------------------------------------
                    ' PARTICELLECLASSAMENTO
                    '-------------------------------------------------------------


                    xParticellaxClassamento = xParticella.GetElementsByTagName("Classamento")

                    i_Classamento = 0

                    If xParticellaxClassamento IsNot Nothing Then

                        Do While i_Classamento < xParticellaxClassamento.Count

                            xClassamento = xParticellaxClassamento.Item(i_Classamento)

                            Dim operazioneClassamentoxParticella As Integer

                            Dim esisteClass = objParticelle_R.Esiste_ClassamentoxParticella(CStr(xClassamento.GetAttribute("prov")),
                                                        CStr(xClassamento.GetAttribute("com")),
                                                        CStr(xClassamento.GetAttribute("sezione")),
                                                        CInt(xClassamento.GetAttribute("foglio")),
                                                        CInt(xClassamento.GetAttribute("numero")),
                                                        CStr(xClassamento.GetAttribute("subalterno")),
                                                        CInt(xClassamento.GetAttribute("qualita_cod")), "", objParametri)

                            If esisteClass Then
                                operazioneClassamentoxParticella = 2
                            Else
                                operazioneClassamentoxParticella = 1
                            End If


                            Select Case operazioneClassamentoxParticella

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = objParticelle.Scrivi_ParticelleCatastaliClassamento(
                                                        CStr(xClassamento.GetAttribute("prov")),
                                                        CStr(xClassamento.GetAttribute("com")),
                                                        CStr(xClassamento.GetAttribute("sezione")),
                                                        CInt(xClassamento.GetAttribute("foglio")),
                                                        CInt(xClassamento.GetAttribute("numero")),
                                                        CStr(xClassamento.GetAttribute("subalterno")),
                                                        CInt(xClassamento.GetAttribute("qualita_cod")),
                                                        CStr(xClassamento.GetAttribute("porzione")),
                                                        CStr(xClassamento.GetAttribute("classe")),
                                                        CDbl(xClassamento.GetAttribute("sup_classe")),
                                                        CDbl(xClassamento.GetAttribute("reddito_dominicale")),
                                                        CDbl(xClassamento.GetAttribute("reddito_agrario")),
                                                        CStr(xClassamento.GetAttribute("deduzione")),
                                                        AGRODATAINIZIO, AGRODATAFINE, objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Dummy = objParticelle.Modifica_ParticelleCatastaliClassamento(
                                                            CInt(xClassamento.GetAttribute("qualita_cod")),
                                                            CStr(xClassamento.GetAttribute("prov")),
                                                            CStr(xClassamento.GetAttribute("com")),
                                                            CStr(xClassamento.GetAttribute("sezione")),
                                                            CInt(xClassamento.GetAttribute("foglio")),
                                                            CInt(xClassamento.GetAttribute("numero")),
                                                            CStr(xClassamento.GetAttribute("subalterno")),
                                                            CStr(xClassamento.GetAttribute("porzione")),
                                                            CStr(xClassamento.GetAttribute("classe")),
                                                            CDbl(xClassamento.GetAttribute("sup_classe")),
                                                            CDbl(xClassamento.GetAttribute("reddito_dominicale")),
                                                            CDbl(xClassamento.GetAttribute("reddito_agrario")),
                                                            CStr(xClassamento.GetAttribute("deduzione")),
                                                            AGRODATAINIZIO, AGRODATAFINE, objParametri)

                            End Select

                            i_Classamento += 1

                        Loop

                    End If

                    '-------------------------------------------------------------
                    ' PARTICELLEELEGGIBILITA
                    '-------------------------------------------------------------

                    xParticellaxEleggibilita = xParticella.GetElementsByTagName("Eleggibilita")

                    i_Eleggibilita = 0

                    If xParticellaxEleggibilita IsNot Nothing Then

                        Do While i_Eleggibilita < xParticellaxEleggibilita.Count

                            xEleggibilita = xParticellaxEleggibilita.Item(i_Eleggibilita)

                            Dim operazioneEleggibilitaxParticella As Integer

                            Dim esisteClass = objParticelleEleggibilita_R.Esiste_ParticelleCatastalixEleggibilitaParticelle(CStr(xEleggibilita.GetAttribute("prov")),
                                                                               CStr(xEleggibilita.GetAttribute("com")),
                                                                               CStr(xEleggibilita.GetAttribute("sezione")),
                                                                               CInt(xEleggibilita.GetAttribute("foglio")),
                                                                               CInt(xEleggibilita.GetAttribute("numero")),
                                                                               CStr(xEleggibilita.GetAttribute("subalterno")),
                                                                               CInt(xEleggibilita.GetAttribute("eleggibilita_cod")), "", objParametri)

                            If esisteClass Then
                                operazioneEleggibilitaxParticella = 2
                            Else
                                operazioneEleggibilitaxParticella = 1
                            End If

                            Select Case operazioneEleggibilitaxParticella

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    Dummy = objParticelleEleggibilita_W.Scrivi(CStr(xEleggibilita.GetAttribute("prov")),
                                                                               CStr(xEleggibilita.GetAttribute("com")),
                                                                               CStr(xEleggibilita.GetAttribute("sezione")),
                                                                               CInt(xEleggibilita.GetAttribute("foglio")),
                                                                               CInt(xEleggibilita.GetAttribute("numero")),
                                                                               CStr(xEleggibilita.GetAttribute("subalterno")),
                                                                               CInt(xEleggibilita.GetAttribute("eleggibilita_cod")),
                                                                               CDbl(xEleggibilita.GetAttribute("superficie")),
                                                                               AGRODATAINIZIO, AGRODATAFINE, objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Dummy = objParticelleEleggibilita_W.Modifica(
                                                            CStr(xEleggibilita.GetAttribute("prov")),
                                                            CStr(xEleggibilita.GetAttribute("com")),
                                                            CStr(xEleggibilita.GetAttribute("sezione")),
                                                            CInt(xEleggibilita.GetAttribute("foglio")),
                                                            CInt(xEleggibilita.GetAttribute("numero")),
                                                            CStr(xEleggibilita.GetAttribute("subalterno")),
                                                            CInt(xEleggibilita.GetAttribute("eleggibilita_cod")),
                                                            CDbl(xEleggibilita.GetAttribute("superficie")),
                                                            AGRODATAINIZIO, AGRODATAFINE, "", objParametri)

                            End Select

                            i_Eleggibilita += 1

                        Loop

                    End If

                    '-------------------------------------------------------------
                    ' ENTITA GRAFICHE
                    '-------------------------------------------------------------

                    xEntitaGrafiche = xParticella.GetElementsByTagName("DatiEntita")
                    If xEntitaGrafiche.Count = 1 Then

                        Dim objGrafica As New AgronicaCoreGraficaBIZ.Grafica_Write
                        objGrafica.Grafica_Scrivi(xEntitaGrafiche.Item(0).OuterXml,
                                            CStr(xParticella.GetAttribute("piva")),
                                            CLng(xParticella.GetAttribute("sa_cod")),
                                            Cod_Particella, objParametri)
                    End If


                    '-------------------------------------------------------------
                    ' METODO PRODUZIONE
                    '-------------------------------------------------------------

                    xParticellaMetodiProduzione = xParticella.GetElementsByTagName("metodoproduzione")

                    Dim i_MetodoProduzione As Integer = 0

                    If xParticellaMetodiProduzione IsNot Nothing Then

                        Do While i_MetodoProduzione < xParticellaMetodiProduzione.Count

                            xParticellaMetodoProduzione = xParticellaMetodiProduzione.Item(i_MetodoProduzione)

                            Dim operazioneMetodoProduzionexParticella As Integer

                            Dim dtMetodoProduzionexParticella = objParticelle_MetodoProduzione_R.Leggi(CStr(xParticella.GetAttribute("prov")),
                                                                        CStr(xParticella.GetAttribute("com")),
                                                                        CStr(xParticella.GetAttribute("sezione")),
                                                                        CInt(xParticella.GetAttribute("foglio")),
                                                                        CInt(xParticella.GetAttribute("numero")),
                                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                                        CStr(xParticellaMetodoProduzione.GetAttribute("metodoproduzione_cod")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_inizio")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_fine")),
                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        "", "", objParametri)

                            If dtMetodoProduzionexParticella.Rows.Count > 0 Then
                                operazioneMetodoProduzionexParticella = 2
                            Else
                                operazioneMetodoProduzionexParticella = 1
                            End If

                            Select Case operazioneMetodoProduzionexParticella

                                Case "0"    'LEGGI -------------------------------------------------------

                                Case "1"    'SALVA -------------------------------------------------------

                                    risp = objParticelle_MetodoProduzione_W.Scrivi(
                                                                        CStr(xParticella.GetAttribute("prov")),
                                                                        CStr(xParticella.GetAttribute("com")),
                                                                        CStr(xParticella.GetAttribute("sezione")),
                                                                        CInt(xParticella.GetAttribute("foglio")),
                                                                        CInt(xParticella.GetAttribute("numero")),
                                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                                        CStr(xParticellaMetodoProduzione.GetAttribute("metodoproduzione_cod")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_inizio")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_fine")),
                                                                        objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    'objImpresexParticelle2.Modifica_2(0,
                                    '                                CStr(xParticella.GetAttribute("piva")),
                                    '                                CInt(xParticella.GetAttribute("sa_cod")),
                                    '                                CStr(xParticella.GetAttribute("prov")),
                                    '                                CStr(xParticella.GetAttribute("com")),
                                    '                                CStr(xParticella.GetAttribute("sezione")),
                                    '                                CInt(xParticella.GetAttribute("foglio")),
                                    '                                CInt(xParticella.GetAttribute("numero")),
                                    '                                CStr(xParticella.GetAttribute("subalterno")),
                                    '                                CStr(xParticellaPossesso.GetAttribute("partita_catastale")),
                                    '                                CInt(xParticellaPossesso.GetAttribute("titolopossesso")),
                                    '                                CDbl(xParticellaPossesso.GetAttribute("sup_condotta")),
                                    '                                CDate(xParticellaPossesso.GetAttribute("validita_inizio")),
                                    '                                CDate(xParticellaPossesso.GetAttribute("validita_fine")),
                                    '                                "",
                                    '                                objParametri)

                                    risp = objParticelle_MetodoProduzione_W.Elimina(
                                                                        CStr(xParticella.GetAttribute("prov")),
                                                                        CStr(xParticella.GetAttribute("com")),
                                                                        CStr(xParticella.GetAttribute("sezione")),
                                                                        CInt(xParticella.GetAttribute("foglio")),
                                                                        CInt(xParticella.GetAttribute("numero")),
                                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                                        CStr(xParticellaMetodoProduzione.GetAttribute("metodoproduzione_cod")),
                                                                        objParametri)

                                    risp = objParticelle_MetodoProduzione_W.Scrivi(
                                                                        CStr(xParticella.GetAttribute("prov")),
                                                                        CStr(xParticella.GetAttribute("com")),
                                                                        CStr(xParticella.GetAttribute("sezione")),
                                                                        CInt(xParticella.GetAttribute("foglio")),
                                                                        CInt(xParticella.GetAttribute("numero")),
                                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                                        CStr(xParticellaMetodoProduzione.GetAttribute("metodoproduzione_cod")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_inizio")),
                                                                        CDate(xParticellaMetodoProduzione.GetAttribute("validita_fine")),
                                                                        objParametri)

                            End Select

                            i_MetodoProduzione += 1

                        Loop

                    End If


                    '-------------------------------------------------------------
                    ' CAMPIXPARTICELLE
                    '-------------------------------------------------------------

                    xDatiCampixParticelle = xParticella.GetElementsByTagName("DatiCampixParticelle")

                    i_DatiParticelle = 0

                    Do While i_DatiParticelle < xDatiCampixParticelle.Count

                        'Prelevo l'i-esimo blocco di DatiParticelle (in realtà ne esiste uno solo)
                        xDatiCampoxParticella = xDatiCampixParticelle.Item(i_DatiParticelle)

                        '------------------------------

                        xCampixParticelle = xDatiCampoxParticella.GetElementsByTagName("CampoxParticella")

                        i_DatiParticella = 0

                        Do While i_DatiParticella < xCampixParticelle.Count

                            xCampoxParticella = xCampixParticelle.Item(i_DatiParticella)

                            'Prelevo gli attributi dell'indirizzo selezionato
                            OpeDB_CampoxParticella = xCampoxParticella.GetAttribute("TipoOperazioneDB")

                            Dim objCampixParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_CampoxParticella

                                Case "0"    'LEGGI -----------------------

                                Case "1"    'SALVA  ----------------------

                                    Dummy = objCampixParticelle.Scrivi(
                                            CStr(xCampoxParticella.GetAttribute("piva")),
                                            CLng(xCampoxParticella.GetAttribute("sa_cod")),
                                            CLng(xCampoxParticella.GetAttribute("campo_cod")),
                                            CStr(xCampoxParticella.GetAttribute("prov")),
                                            CStr(xCampoxParticella.GetAttribute("com")),
                                            CStr(xCampoxParticella.GetAttribute("sezione")),
                                            CLng(xCampoxParticella.GetAttribute("foglio")),
                                            CLng(xCampoxParticella.GetAttribute("numero")),
                                            CStr(xCampoxParticella.GetAttribute("subalterno")),
                                            CDbl(xCampoxParticella.GetAttribute("sau_convenz_ettari")),
                                            CLng(xCampoxParticella.GetAttribute("sau_convenz_are")),
                                            CLng(xCampoxParticella.GetAttribute("sau_convenz_centiare")),
                                            CDbl(xCampoxParticella.GetAttribute("sau_convers_ettari")),
                                            CLng(xCampoxParticella.GetAttribute("sau_convers_are")),
                                            CLng(xCampoxParticella.GetAttribute("sau_convers_centiare")),
                                            CDbl(xCampoxParticella.GetAttribute("sau_bio_ettari")),
                                            CLng(xCampoxParticella.GetAttribute("sau_bio_are")),
                                            CLng(xCampoxParticella.GetAttribute("sau_bio_centiare")),
                                            Replace(xCampoxParticella.GetAttribute("area"), ".", ","),
                                            CDate(xCampoxParticella.GetAttribute("validita_inizio")),
                                            CDate(xCampoxParticella.GetAttribute("validita_fine")),
                                            objParametri)

                                Case "2"   'MODIFICA ----------------------

                                    objCampixParticelle.Modifica(
                                           CStr(xCampoxParticella.GetAttribute("piva")),
                                           CLng(xCampoxParticella.GetAttribute("sa_cod")),
                                           CLng(xCampoxParticella.GetAttribute("campo_cod")),
                                           CStr(xCampoxParticella.GetAttribute("prov")),
                                           CStr(xCampoxParticella.GetAttribute("com")),
                                           CStr(xCampoxParticella.GetAttribute("sezione")),
                                           CLng(xCampoxParticella.GetAttribute("foglio")),
                                           CLng(xCampoxParticella.GetAttribute("numero")),
                                           CStr(xCampoxParticella.GetAttribute("subalterno")),
                                           Replace(xCampoxParticella.GetAttribute("area"), ".", ","),
                                           CDbl(xCampoxParticella.GetAttribute("sau_convenz_ettari")),
                                           CLng(xCampoxParticella.GetAttribute("sau_convenz_are")),
                                           CLng(xCampoxParticella.GetAttribute("sau_convenz_centiare")),
                                           CDbl(xCampoxParticella.GetAttribute("sau_convers_ettari")),
                                           CLng(xCampoxParticella.GetAttribute("sau_convers_are")),
                                           CLng(xCampoxParticella.GetAttribute("sau_convers_centiare")),
                                           CDbl(xCampoxParticella.GetAttribute("sau_bio_ettari")),
                                           CLng(xCampoxParticella.GetAttribute("sau_bio_are")),
                                           CLng(xCampoxParticella.GetAttribute("sau_bio_centiare")),
                                           CDate(xCampoxParticella.GetAttribute("validita_inizio")),
                                            CDate(xCampoxParticella.GetAttribute("validita_fine")),
                                           "",
                                            objParametri)

                                Case "3"   'CANCELLA --------------------------


                                    objCampixParticelle.Cancella_Associazione(
                                    CStr(xCampoxParticella.GetAttribute("piva")),
                                    CLng(xCampoxParticella.GetAttribute("sa_cod")),
                                    CLng(xCampoxParticella.GetAttribute("campo_cod")),
                                    CStr(xCampoxParticella.GetAttribute("prov")),
                                    CStr(xCampoxParticella.GetAttribute("com")),
                                    CStr(xCampoxParticella.GetAttribute("sezione")),
                                    CLng(xCampoxParticella.GetAttribute("foglio")),
                                    CLng(xCampoxParticella.GetAttribute("numero")),
                                    CStr(xCampoxParticella.GetAttribute("subalterno")),
                                    "",
                                    objParametri)

                            End Select

                            i_DatiParticella += 1

                        Loop

                        i_DatiParticelle += 1

                    Loop

                    '</CAMPIXPARTICELLE>
                    '------------------------------------------------------------------------------------------

                    '-------------------------------------------------------------
                    ' APPEZZAMENTIXPARTICELLE
                    '-------------------------------------------------------------

                    xDatiAppezzaxParticelle = xParticella.GetElementsByTagName("DatiAppezzaxParticelle")

                    i_DatiParticelle = 0

                    Do While i_DatiParticelle < xDatiAppezzaxParticelle.Count

                        'Prelevo l'i-esimo blocco di DatiParticelle (in realta' ne esiste uno solo)
                        xDatiAppezzaxParticella = xDatiAppezzaxParticelle.Item(i_DatiParticelle)

                        '------------------------------

                        xAppezzaxParticelle = xDatiAppezzaxParticella.GetElementsByTagName("AppezzaxParticella")

                        i_DatiParticella = 0

                        Do While i_DatiParticella < xAppezzaxParticelle.Count

                            xAppezzaxParticella = xAppezzaxParticelle.Item(i_DatiParticella)

                            'Prelevo gli attributi dell'indirizzo selezionato
                            OpeDB_AppezzaxParticella = xAppezzaxParticella.GetAttribute("TipoOperazioneDB")

                            Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_AppezzaxParticella

                                Case "0"    'LEGGI -----------------------

                                Case "1"    'SALVA  ----------------------

                                    Dummy = objAppezzaxParticelle.Scrivi(
                                            CStr(xAppezzaxParticella.GetAttribute("piva")),
                                            CLng(xAppezzaxParticella.GetAttribute("sa_cod")),
                                            CLng(xAppezzaxParticella.GetAttribute("appezza")),
                                            CStr(xAppezzaxParticella.GetAttribute("prov")),
                                            CStr(xAppezzaxParticella.GetAttribute("com")),
                                            CStr(xAppezzaxParticella.GetAttribute("sezione")),
                                            CLng(xAppezzaxParticella.GetAttribute("foglio")),
                                            CLng(xAppezzaxParticella.GetAttribute("numero")),
                                            CStr(xAppezzaxParticella.GetAttribute("subalterno")),
                                            Replace(xAppezzaxParticella.GetAttribute("area"), ".", ","),
                                            CDbl(xAppezzaxParticella.GetAttribute("sau_convenz_ettari")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_convenz_are")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_convenz_centiare")),
                                            CDbl(xAppezzaxParticella.GetAttribute("sau_convers_ettari")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_convers_are")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_convers_centiare")),
                                            CDbl(xAppezzaxParticella.GetAttribute("sau_bio_ettari")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_bio_are")),
                                            CLng(xAppezzaxParticella.GetAttribute("sau_bio_centiare")),
                                            CDate(xAppezzaxParticella.GetAttribute("validita_inizio")),
                                            CDate(xAppezzaxParticella.GetAttribute("validita_fine")),
                                            objParametri)

                                Case "2"   'MODIFICA ----------------------

                                    objAppezzaxParticelle.Modifica(
                                           CStr(xAppezzaxParticella.GetAttribute("piva")),
                                           CLng(xAppezzaxParticella.GetAttribute("sa_cod")),
                                           CLng(xAppezzaxParticella.GetAttribute("appezza")),
                                           CStr(xAppezzaxParticella.GetAttribute("prov")),
                                           CStr(xAppezzaxParticella.GetAttribute("com")),
                                           CStr(xAppezzaxParticella.GetAttribute("sezione")),
                                           CLng(xAppezzaxParticella.GetAttribute("foglio")),
                                           CLng(xAppezzaxParticella.GetAttribute("numero")),
                                           CStr(xAppezzaxParticella.GetAttribute("subalterno")),
                                           Replace(xAppezzaxParticella.GetAttribute("area"), ".", ","),
                                           CDbl(xAppezzaxParticella.GetAttribute("sau_convenz_ettari")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_convenz_are")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_convenz_centiare")),
                                           CDbl(xAppezzaxParticella.GetAttribute("sau_convers_ettari")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_convers_are")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_convers_centiare")),
                                           CDbl(xAppezzaxParticella.GetAttribute("sau_bio_ettari")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_bio_are")),
                                           CLng(xAppezzaxParticella.GetAttribute("sau_bio_centiare")),
                                           CDate(xAppezzaxParticella.GetAttribute("validita_inizio")),
                                            CDate(xAppezzaxParticella.GetAttribute("validita_fine")),
                                           "",
                                           objParametri)


                                Case "3"   'CANCELLA --------------------------

                                    objAppezzaxParticelle.Cancella_Associazione(
                                    CStr(xAppezzaxParticella.GetAttribute("piva")),
                                    CLng(xAppezzaxParticella.GetAttribute("sa_cod")),
                                    CLng(xAppezzaxParticella.GetAttribute("appezza")),
                                    CStr(xAppezzaxParticella.GetAttribute("prov")),
                                    CStr(xAppezzaxParticella.GetAttribute("com")),
                                    CStr(xAppezzaxParticella.GetAttribute("sezione")),
                                    CLng(xAppezzaxParticella.GetAttribute("foglio")),
                                    CLng(xAppezzaxParticella.GetAttribute("numero")),
                                    CStr(xAppezzaxParticella.GetAttribute("subalterno")),
                                    "",
                                    objParametri)


                            End Select

                            i_DatiParticella += 1

                        Loop

                        i_DatiParticelle += 1

                    Loop

                    '</APPEZZAXPARTICELLE>
                    '------------------------------------------------------------------------------------------

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Particella

                        Case "3" 'CANCELLAZIONE Particella

                            ' cancello codici impresa particella
                            objImpresexParticelle2_W.Cancella_Codici(0,
                                    CStr(xParticella.GetAttribute("piva")),
                                    CInt(xParticella.GetAttribute("sa_cod")),
                                    CStr(xParticella.GetAttribute("prov")),
                                    CStr(xParticella.GetAttribute("com")),
                                    CStr(xParticella.GetAttribute("sezione")),
                                    CInt(xParticella.GetAttribute("foglio")),
                                    CInt(xParticella.GetAttribute("numero")),
                                    CStr(xParticella.GetAttribute("subalterno")),
                                    "", objParametri)

                            ' cancello contatti impresa particella
                            objImpresexParticelle2_W.Cancella_Contatti(0,
                                    CStr(xParticella.GetAttribute("piva")),
                                    CInt(xParticella.GetAttribute("sa_cod")),
                                    CStr(xParticella.GetAttribute("prov")),
                                    CStr(xParticella.GetAttribute("com")),
                                    CStr(xParticella.GetAttribute("sezione")),
                                    CInt(xParticella.GetAttribute("foglio")),
                                    CInt(xParticella.GetAttribute("numero")),
                                    CStr(xParticella.GetAttribute("subalterno")),
                                    "", objParametri)

                            ' cancello impresa particella
                            objImpresexParticelle2_W.Cancella_Associazione(CStr(xParticella.GetAttribute("piva")),
                                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                                           CStr(xParticella.GetAttribute("prov")),
                                                                           CStr(xParticella.GetAttribute("com")),
                                                                           CStr(xParticella.GetAttribute("sezione")),
                                                                           CLng(xParticella.GetAttribute("foglio")),
                                                                           CLng(xParticella.GetAttribute("numero")),
                                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                                           "",
                                                                           objParametri)



                            'Creo l'oggetto COM
                            Dim objCampixParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_W

                            objCampixParticelle.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                         CLng(xParticella.GetAttribute("sa_cod")),
                                                         0,
                                                         CStr(xParticella.GetAttribute("prov")),
                                                         CStr(xParticella.GetAttribute("com")),
                                                         CStr(xParticella.GetAttribute("sezione")),
                                                         CLng(xParticella.GetAttribute("foglio")),
                                                         CLng(xParticella.GetAttribute("numero")),
                                                         CStr(xParticella.GetAttribute("subalterno")),
                                                         "",
                                                         objParametri)

                            'Creo l'oggetto COM
                            Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
                            Dim objAppezzaxParticelleMacrousi As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
                            Dim objAppezzaxParticelleMacrousiUtilizzo As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W
                            Dim objCatastoModificheParticelleXAppezzamentiLog As New AgronicaCoreAnagrafeDAL.CatastoModificheParticelleXAppezzamentiLog_W

                            objAppezzaxParticelle.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "",
                                                           objParametri)

                            objAppezzaxParticelleMacrousi.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "", "",
                                                           objParametri)

                            objAppezzaxParticelleMacrousiUtilizzo.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "", "", "", "",
                                                           objParametri)

                            objCatastoModificheParticelleXAppezzamentiLog.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0, 0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "",
                                                           objParametri)

                            ' cancello riferimento su fabbricato
                            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_W
                            objFabbricati.CancellaParticella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "",
                                                           objParametri)

                            ' cancello associazione piano concimazione
                            Dim objPianoConcimazione As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_W
                            objPianoConcimazione.CancellaParticella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "",
                                                           objParametri)

                            ' cancello associazione programmazione particelle
                            Dim objProgrammazioneParticelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_W
                            objProgrammazioneParticelle.CancellaParticella(CStr(xParticella.GetAttribute("piva")),
                                                           CLng(xParticella.GetAttribute("sa_cod")),
                                                           0,
                                                           CStr(xParticella.GetAttribute("prov")),
                                                           CStr(xParticella.GetAttribute("com")),
                                                           CStr(xParticella.GetAttribute("sezione")),
                                                           CLng(xParticella.GetAttribute("foglio")),
                                                           CLng(xParticella.GetAttribute("numero")),
                                                           CStr(xParticella.GetAttribute("subalterno")),
                                                           "",
                                                           objParametri)

                            '----------------------------------------
                            'Eliminazione associazioni a DISTINTE

                            Dim objProgettixParticelle_W As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W

                            'Chiamo CancellaxProgetto perché esegue la stessa query di CancellaxParticella
                            objProgettixParticelle_W.CancellaxProgetto(CStr(xParticella.GetAttribute("piva")),
                                                    CLng(xParticella.GetAttribute("sa_cod")),
                                                    0, 0, 0,
                                                    CStr(xParticella.GetAttribute("prov")),
                                                    CStr(xParticella.GetAttribute("com")),
                                                    CStr(xParticella.GetAttribute("sezione")),
                                                    CLng(xParticella.GetAttribute("foglio")),
                                                    CLng(xParticella.GetAttribute("numero")),
                                                    CStr(xParticella.GetAttribute("subalterno")),
                                                    "", objParametri)

                            objProgettixParticelle_W = Nothing


                            '-----------------------------------
                            '-----------------------------------
                            '-----------------------------------
                            '(30/06/2015 fede) 
                            'VERIFICO CHE LA PARTICELLA NON ESISTA PIU IN ALTRI CENTRI O IMPRESE

                            DtParticella = objImpresexParticelle2_R.LeggixChiave(0,
                                                                                 "",
                                                                                 0,
                                                                                 CStr(xParticella.GetAttribute("prov")),
                                                                                 CStr(xParticella.GetAttribute("com")),
                                                                                 CStr(xParticella.GetAttribute("sezione")),
                                                                                 CLng(xParticella.GetAttribute("foglio")),
                                                                                 CLng(xParticella.GetAttribute("numero")),
                                                                                 CStr(xParticella.GetAttribute("subalterno")),
                                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri)

                            'ELIMINAZIONE DATI LEGATI ALLA TESTATA DELLA PARTICELLA (tabella ParticelleCatastali)
                            If DtParticella.Rows.Count = 0 Then

                                '-----------------------------------
                                'Eliminazione PARTICELLA 
                                objParticelle.Cancella_2(CStr(xParticella.GetAttribute("prov")),
                                                         CStr(xParticella.GetAttribute("com")),
                                                         CStr(xParticella.GetAttribute("sezione")),
                                                         CLng(xParticella.GetAttribute("foglio")),
                                                         CLng(xParticella.GetAttribute("numero")),
                                                         CStr(xParticella.GetAttribute("subalterno")),
                                                         "",
                                                         objParametri)

                                '-----------------------------------
                                'Eliminazione ELEGGIBILITA'
                                Dim objEleggibilita_AD As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_W

                                objEleggibilita_AD.Cancella(CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        0,
                                                        "",
                                                        objParametri)
                                objEleggibilita_AD = Nothing

                                '-----------------------------------
                                'Eliminazione ZONE
                                Dim objZone_AD As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W

                                objZone_AD.Cancella(0,
                                                    CStr(xParticella.GetAttribute("prov")),
                                                    CStr(xParticella.GetAttribute("com")),
                                                    CStr(xParticella.GetAttribute("sezione")),
                                                    CLng(xParticella.GetAttribute("foglio")),
                                                    CLng(xParticella.GetAttribute("numero")),
                                                    CStr(xParticella.GetAttribute("subalterno")),
                                                    "",
                                                    objParametri)
                                objZone_AD = Nothing

                                '----------------------------------------
                                'Eliminazione DATI di CLASSAMENTO

                                Dim objPClassamento_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_W
                                objPClassamento_W.Cancella_ParticelleCatastaliClassamento(
                                                        CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        "", objParametri)

                                objPClassamento_W = Nothing

                                Dim objPMetodo_Produzione As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_W

                                objPMetodo_Produzione.Elimina(CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        -1,
                                                        objParametri)

                                objPMetodo_Produzione = Nothing

                                ' Eliminazione codici particella catastale
                                objParticelle.Cancella_ParticelleCatastali_Codici(CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        0, "", objParametri)

                            End If


                            '-----------------------------------------------------------------------
                            'VERIFICO CHE LA PARTICELLA NON ESISTA PIU NELL'IMPRESA
                            DtParticella = objImpresexParticelle2_R.LeggixChiave(0,
                                                                                 CStr(xParticella.GetAttribute("piva")),
                                                                                 0,
                                                                                 CStr(xParticella.GetAttribute("prov")),
                                                                                 CStr(xParticella.GetAttribute("com")),
                                                                                 CStr(xParticella.GetAttribute("sezione")),
                                                                                 CLng(xParticella.GetAttribute("foglio")),
                                                                                 CLng(xParticella.GetAttribute("numero")),
                                                                                 CStr(xParticella.GetAttribute("subalterno")),
                                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri)

                            If DtParticella.Rows.Count = 0 Then

                                '-----------------------------------
                                'Eliminazione MACROUSI
                                Dim objMacrousi_AD As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W

                                objMacrousi_AD.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                        CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        "",
                                                        "",
                                                        objParametri)
                                objMacrousi_AD = Nothing

                                '-----------------------------------
                                'Eliminazione UTILIZZI
                                Dim objUtilizzi_AD As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_W
                                objUtilizzi_AD.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                        CStr(xParticella.GetAttribute("prov")),
                                                        CStr(xParticella.GetAttribute("com")),
                                                        CStr(xParticella.GetAttribute("sezione")),
                                                        CLng(xParticella.GetAttribute("foglio")),
                                                        CLng(xParticella.GetAttribute("numero")),
                                                        CStr(xParticella.GetAttribute("subalterno")),
                                                        "",
                                                        "",
                                                        "",
                                                        "",
                                                        objParametri)
                                objUtilizzi_AD = Nothing

                            End If

                            '-----------------------------------------------------------------------
                            'VERIFICO CHE LA PARTICELLA NON ESISTA PIU NELL'IMPRESA E NEL CENTRO
                            DtParticella = objImpresexParticelle2_R.LeggixChiave(0,
                                                                                 CStr(xParticella.GetAttribute("piva")),
                                                                                 CLng(xParticella.GetAttribute("sa_cod")),
                                                                                 CStr(xParticella.GetAttribute("prov")),
                                                                                 CStr(xParticella.GetAttribute("com")),
                                                                                 CStr(xParticella.GetAttribute("sezione")),
                                                                                 CLng(xParticella.GetAttribute("foglio")),
                                                                                 CLng(xParticella.GetAttribute("numero")),
                                                                                 CStr(xParticella.GetAttribute("subalterno")),
                                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri)

                            'Eliminazione DATI GRAFICI
                            If DtParticella.Rows.Count = 0 Then

                                '  cancella il poligono
                                Dim objGrafica_AD As New AgronicaCoreGraficaDAL.Grafica_Write
                                Dim GraphicKey As String

                                GraphicKey = "D" & Right("00000000" & Hex(CLng(xParticella.GetAttribute("part_cod"))), 8)
                                objGrafica_AD.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                        CLng(xParticella.GetAttribute("sa_cod")),
                                                        "",
                                                        GraphicKey,
                                                        "",
                                                        "",
                                                        objParametri)

                                '  cancella il testo associato
                                GraphicKey = "Q" & Right("00000000" & Hex(CLng(xParticella.GetAttribute("part_cod"))), 8)
                                objGrafica_AD.Cancella(CStr(xParticella.GetAttribute("piva")),
                                                        CLng(xParticella.GetAttribute("sa_cod")),
                                                        "",
                                                        GraphicKey,
                                                        "",
                                                        "",
                                                        objParametri)

                                objGrafica_AD = Nothing

                            End If

                    End Select

                    '-------------------------------------------------------------

                    'Elimino gli oggetti
                    DtParticella = Nothing

                    'Incremento l'indice
                    i_Particella += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_Particelle += 1

            Loop

            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xParticella = Nothing
            xParticelle = Nothing
            xDatiParticella = Nothing
            xDatiParticelle = Nothing
            xmlDoc = Nothing

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            xRisp = True

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = " " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function

    Private Function ParticellaScriviDatoCartografico(objParametri_Server As AgronicaCoreParametri,
                                                      Prov As String,
                                                      Com As String,
                                                      Sezione As String,
                                                      foglio As Integer,
                                                      numero As Integer,
                                                      subalterno As String,
                                                      Piva As String,
                                                      Sa_Cod As Integer,
                                                      wkt As String,
                                                      wkt_georiferimento_cod As String,
                                                      ScriviElementiGrafici As AgronicaCoreGisBIZ.GIS_Entita_W,
                                                      objParametri As AgronicaCoreParametri
                                                      ) As AgronicaCoreParametri

        Dim ParametriCartografici As ParametriCoordinateConverter = Nothing

        If Sezione = "" Then
            Sezione = "0"
        End If

        If subalterno = "" Then
            subalterno = "0"
        End If

        If wkt_georiferimento_cod <> "-1" Then


            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            ParametriCartografici = New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }


        End If

        Dim sNodeDoc As XDocument

        Dim wktHelp As New WKT
        Dim wktToGeoML As New wkt_gml
        Dim cconverter As New CoordinateConverter


        If ParametriCartografici IsNot Nothing Then

            '    sNodeDoc = ReadXmlFromString(
            '        wktToGeoML.Trasforma(
            '            wktHelp.CreaPoligonoDaCoordinate(
            '                EntitaDaTrasformare.DatiCartograficiOriginali_WGS84,
            '                (EntitaDaTrasformare.DatiCartograficiOriginali_WGS84.Count = 1)
            '            ),
            '            True,
            '            False,
            '            True,
            '            0)
            '    )
            '    sNodeDoc.Root.@Flag_GPS = 1
            'Else

            Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
                                                xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
            Dim FinalDoc As New XDocument
            FinalDoc.Add(ElemFinalXdoc)


            sNodeDoc = XDocument.Parse(
                wktToGeoML.Trasforma(
                    cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici),
                    False,
                    False,
                    True,
                    0)
            )

            sNodeDoc.Root.@Flag_GPS = 0

            Dim newEntitaElement = <Entita TipoOperazioneDB="1" recno="" deleted="" section="E" id="1" descr="" ecolor="256" eline="256" rad="15" text="" gps="0" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
                                       <layers>
                                           <layer tipologia_layer="1"><%= CInt(enum_Gis_LayerElementiGrafici_std.CATASTO) %></layer>
                                       </layers>
                                       <EntitaGIAS>
                                           <DatoGias>
                                               <PivaSuperUser><%= objParametri.PivaSuperUser %></PivaSuperUser>
                                               <Entita_Cod>0</Entita_Cod>
                                               <TipoEntita_Cod><%= CInt(enum_GIS2012_TipoEntita.CATASTO) %></TipoEntita_Cod>
                                               <Piva></Piva>
                                               <Sa_Cod>0</Sa_Cod>
                                               <Appezza>0</Appezza>
                                               <Campo_Cod>0</Campo_Cod>
                                               <Id_Imp>0</Id_Imp>
                                               <PROV><%= Prov %></PROV>
                                               <COM><%= Com %></COM>
                                               <SEZIONE><%= Sezione %></SEZIONE>
                                               <FOGLIO><%= foglio %></FOGLIO>
                                               <NUMERO><%= numero %></NUMERO>
                                               <SUBALTERNO><%= subalterno %></SUBALTERNO>
                                               <Programmazione_Entita_Cod>0</Programmazione_Entita_Cod>
                                               <Programmazione_Cod>0</Programmazione_Cod>
                                               <Id_Agenda>0</Id_Agenda>
                                               <id_mov_det>0</id_mov_det>
                                               <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
                                               <OLDGrafica_ID></OLDGrafica_ID>
                                               <analisi_campione_cod>0</analisi_campione_cod>
                                               <inviato>0</inviato>
                                               <Data_Creazione><%= GetData_Creazione(Now) %></Data_Creazione>
                                               <Data_Modifica><%= GetData_Creazione(Now) %></Data_Modifica>
                                               <Username_Creazione>agronica</Username_Creazione>
                                               <Username_Modifica>agronica</Username_Modifica>
                                               <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
                                               <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
                                           </DatoGias>
                                       </EntitaGIAS>
                                   </Entita>

            newEntitaElement.Add(sNodeDoc.FirstNode)
            FinalDoc.Root.Add(newEntitaElement)

            Dim ns1 As XNamespace = "http://www.agronica.it/grafica/"

            Dim ListaNS As New List(Of String)
            ListaNS.Add("http://www.opengis.net/gml")

            Dim sFinalDoc1 As String = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
            Dim FinalDoc1 As XDocument = XDocument.Parse(sFinalDoc1)

            For Each elemento In (
                From a In FinalDoc1.Elements(ns1 + "DatiEntita").Elements(ns1 + "Entita")
                Select a).ToList()



                Try

                    Dim idle As Integer
                    ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_Server)


                Catch ex As Exception


                    'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

                End Try


            Next


        End If

        Return objParametri_Server
    End Function

    Public Function Modifica_Centro_Particella(ByVal Piva As String,
                                               ByVal Sa_Cod_OLD As Integer,
                                               ByVal Sa_Cod_NEW As Integer,
                                               ByVal PROV As String,
                                               ByVal COM As String,
                                               ByVal SEZIONE As String,
                                               ByVal FOGLIO As Int32,
                                               ByVal NUMERO As Int32,
                                               ByVal SUBALTERNO As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeBIZ.Particella.Modifica_Centro_Particella()"
        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True

        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            'sposto la PARTICELLA
            Dim SpostataParticella As Boolean = False
            Dim objIP As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
            SpostataParticella = objIP.Modifica_Centro(Piva, Sa_Cod_OLD, Sa_Cod_NEW,
                                                       PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                                                       objParametri)

            'APPEZZAMENTI

            'sposto eventuali APPEZZAMENTI

            ' ........ DA FARE .....................



            ' PLANNING

            'verifico sel la particella è legata ad entita del planning ..
            'nel caso sposto eventuali ENTITA PLANNING
            Dim objPP_R As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            Dim DT_Entita As New DataTable
            DT_Entita = objPP_R.LeggiEntitaImpresa_Da_Particella(Piva,
                                                                 0, PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "", objParametri)

            Dim objPE_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W
            Dim SpostataEntita As Boolean = False

            For i = 0 To DT_Entita.Rows.Count - 1
                SpostataEntita = objPE_W.Modifica_Centro(DT_Entita.Rows(i).Item("Programmazione_Entita_Cod"), Sa_Cod_OLD, Sa_Cod_NEW,
                                  objParametri)
            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            xRisp = True

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = " " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)


        End Try

        Return xRisp


    End Function

    'Private Function ProgrammazioneEntitaScriviDatoCartografico(objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, Programmazione_Cod As Integer, Piva As String, Programmazione_Entita_Cod As Integer, Sa_Cod As Integer, wkt As String, wkt_georiferimento_cod As String, ScriviElementiGrafici As AgronicaCoreGisBIZ.GIS_Entita_W, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDataProvider.AgronicaCoreParametri
    '    Dim ParametriCartografici As ParametriCoordinateConverter

    '    If wkt_georiferimento_cod <> "-1" Then


    '        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
    '        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(wkt_georiferimento_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


    '        ParametriCartografici = New ParametriCoordinateConverter With {
    '            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
    '            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
    '            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
    '            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
    '            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
    '            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
    '        }


    '    End If

    '    Dim sNodeDoc As XDocument

    '    Dim wktHelp As New WKT
    '    Dim wktToGeoML As New wkt_gml
    '    Dim cconverter As New Agronica.CoordinateConverter


    '    If Not ParametriCartografici Is Nothing Then

    '        '    sNodeDoc = ReadXmlFromString(
    '        '        wktToGeoML.Trasforma(
    '        '            wktHelp.CreaPoligonoDaCoordinate(
    '        '                EntitaDaTrasformare.DatiCartograficiOriginali_WGS84,
    '        '                (EntitaDaTrasformare.DatiCartograficiOriginali_WGS84.Count = 1)
    '        '            ),
    '        '            True,
    '        '            False,
    '        '            True,
    '        '            0)
    '        '    )
    '        '    sNodeDoc.Root.@Flag_GPS = 1
    '        'Else

    '        Dim ElemFinalXdoc As XElement = <DatiEntita xmlns="http://www.agronica.it/grafica/"
    '                                            xmlns:gml="http://www.opengis.net/gml"></DatiEntita>
    '        Dim FinalDoc As New XDocument
    '        FinalDoc.Add(ElemFinalXdoc)


    '        sNodeDoc = XDocument.Parse(
    '            wktToGeoML.Trasforma(
    '                cconverter.WKTPolygonWGS84_from_WKTPolygonED50(wkt, True, ParametriCartografici),
    '                False,
    '                False,
    '                True,
    '                0)
    '        )

    '        sNodeDoc.Root.@Flag_GPS = 0

    '        Dim newEntitaElement = <Entita TipoOperazioneDB="1" recno="" deleted="" section="E" id="1" descr="" ecolor="256" eline="256" rad="15" text="" gps="0" lat="0" lon="0" pdop="0" validita_inizio="01/09/2009" validita_fine="31/08/2010" username_creazione="" username_modifica="">
    '                                   <layers>
    '                                       <layer tipologia_layer="1"><%= CInt(enum_Gis_LayerElementiGrafici_std.Impianti_Pianificati_Entita) %></layer>
    '                                   </layers>
    '                                   <EntitaGIAS>
    '                                       <DatoGias>
    '                                           <PivaSuperUser><%= objParametri.PivaSuperUser %></PivaSuperUser>
    '                                           <Entita_Cod>0</Entita_Cod>
    '                                           <TipoEntita_Cod><%= CInt(enum_GIS2012_TipoEntita.PLANNING_IMPIANTI_PIANIFICATI) %></TipoEntita_Cod>
    '                                           <Piva><%= Piva %></Piva>
    '                                           <Sa_Cod><%= Sa_Cod %></Sa_Cod>
    '                                           <Appezza>0</Appezza>
    '                                           <Campo_Cod>0</Campo_Cod>
    '                                           <Id_Imp>0</Id_Imp>
    '                                           <PROV>0</PROV>
    '                                           <COM>0</COM>
    '                                           <SEZIONE>0</SEZIONE>
    '                                           <FOGLIO>0</FOGLIO>
    '                                           <NUMERO>0</NUMERO>
    '                                           <SUBALTERNO>0</SUBALTERNO>
    '                                           <Programmazione_Entita_Cod><%= Programmazione_Entita_Cod %></Programmazione_Entita_Cod>
    '                                           <Programmazione_Cod><%= Programmazione_Cod %></Programmazione_Cod>
    '                                           <Id_Agenda>0</Id_Agenda>
    '                                           <id_mov_det>0</id_mov_det>
    '                                           <Ricetta_Operazione_Cod>0</Ricetta_Operazione_Cod>
    '                                           <OLDGrafica_ID></OLDGrafica_ID>
    '                                           <analisi_campione_cod>0</analisi_campione_cod>
    '                                           <inviato>0</inviato>
    '                                           <Data_Creazione><%= GetData_Creazione(Now) %></Data_Creazione>
    '                                           <Data_Modifica><%= GetData_Creazione(Now) %></Data_Modifica>
    '                                           <Username_Creazione>agronica</Username_Creazione>
    '                                           <Username_Modifica>agronica</Username_Modifica>
    '                                           <Validita_Inizio>1900-01-01T00:00:00</Validita_Inizio>
    '                                           <Validita_Fine>2100-12-31T00:00:00</Validita_Fine>
    '                                       </DatoGias>
    '                                   </EntitaGIAS>
    '                               </Entita>

    '        newEntitaElement.Add(sNodeDoc.FirstNode)
    '        FinalDoc.Root.Add(newEntitaElement)

    '        Dim ns1 As XNamespace = "http://www.agronica.it/grafica/"

    '        Dim ListaNS As New List(Of String)
    '        ListaNS.Add("http://www.opengis.net/gml")

    '        Dim sFinalDoc1 As String = xmlHelper.RemoveNamespace(FinalDoc, ListaNS).ToString.Replace("xmlns=""""", "")
    '        Dim FinalDoc1 As XDocument = XDocument.Parse(sFinalDoc1)

    '        For Each elemento In (
    '            From a In FinalDoc1.Elements(ns1 + "DatiEntita").Elements(ns1 + "Entita")
    '            Select a).ToList()



    '            Try

    '                Dim idle As Integer
    '                ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_Server)


    '            Catch ex As Exception


    '                'My.Computer.FileSystem.WriteAllText(NonImportatiSQL, head & "<Entita>" & elemento.Elements.FirstOrDefault.ToString & "</Entita>" & tail, True)

    '            End Try


    '        Next


    '    End If

    '    Return objParametri_Server
    'End Function

    Private Function GetData_Creazione(ByVal dataCreazione As DateTime) As String
        Return AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(dataCreazione)
    End Function

    Public Sub CrealayerGrafici(objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim objPart As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_W

        'objPart.ScriviLayerGrafici(objParametri_Server)

    End Sub

    Public Function ParticelleCatastali_Scrivi(ByVal Particelle As List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto),
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               light As Boolean,
                                               Optional NoteLog As String = "") As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Particella.ParticelleCatastali_Scrivi[nuovo modello]()"
        Dim messaggioErrore As String = ""
        Dim ret As Boolean = False

        Dim oldValue As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale = Nothing
        Dim newValue As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale = Nothing

        Dim objParticella As New AgronicaCoreAnagrafeBIZ.Particella_W
        Try
            For Each particella In Particelle
                oldValue = particella.oldValue
                newValue = particella.newValue

                If newValue IsNot Nothing Then
                    objParticella.ParticellaCatasto_Scrivi(newValue, oldValue, objParametri_Server, objParametri_Utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)
                End If
            Next
            ret = True
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret

    End Function


    Public Sub ParticellaCatasto_Scrivi(ByVal Particella As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                        ByVal OldValue As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        light As Boolean,
                                        Optional NoteLog As String = "")

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeBIZ.Particella.Particella_Scrivi[nuovo modello]()"
        Dim messaggioErrore As String = ""

        Dim username As String = objParametri_Server.UsernameOperazione

        Using scope As New TransactionScope()
            Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)

                If OldValue IsNot Nothing Then
                    If OldValue.particella.primaryKey.Sezione = "" Then
                        OldValue.particella.primaryKey.Sezione = "0"
                    End If

                    If OldValue.particella.primaryKey.Subalterno = "" Then
                        OldValue.particella.primaryKey.Subalterno = "0"
                    End If

                End If

                If Particella.particella.primaryKey.Sezione = "" Then
                    Particella.particella.primaryKey.Sezione = "0"
                End If

                If Particella.particella.primaryKey.Subalterno = "" Then
                    Particella.particella.primaryKey.Subalterno = "0"
                End If

                If OldValue IsNot Nothing Then
                    If Particella.particella.primaryKey.Prov <> OldValue.particella.primaryKey.Prov OrElse
                        Particella.particella.primaryKey.Com <> OldValue.particella.primaryKey.Com OrElse
                        Particella.particella.primaryKey.Sezione <> OldValue.particella.primaryKey.Sezione OrElse
                        Particella.particella.primaryKey.Foglio <> OldValue.particella.primaryKey.Foglio OrElse
                        Particella.particella.primaryKey.Numero <> OldValue.particella.primaryKey.Numero OrElse
                        Particella.particella.primaryKey.Subalterno <> OldValue.particella.primaryKey.Subalterno Then
                        'Cambio chiave da sviluppare

                        ParticellaCambioChiave(Particella, OldValue, username, GiasContext, objParametri_Server, objParametri_Utenti)
                    End If
                End If


                If Not Particella.flag_cancellazione Then
                    Dim particellaOld = Nothing
                    If OldValue IsNot Nothing Then
                        particellaOld = OldValue.particella
                    End If

                    Particella_Scrivi(Particella.particella, particellaOld, username, GiasContext, objParametri_Server, objParametri_Utenti, NoteLog:=NoteLog)

                    If Not light Then
                        ScriviPossessi(Particella.particella.primaryKey, Particella.centro, Particella.possessiParticella, username, GiasContext, objParametri_Server, objParametri_Utenti)
                    End If

                Else

                    Particella_Elimina(Particella, username, GiasContext, objParametri_Server, objParametri_Utenti, NoteLog:=NoteLog)

                End If

                GiasContext.SaveChanges()
                scope.Complete()

            End Using
        End Using

    End Sub

    Public Sub Particella_Scrivi(ByVal Particella As AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali,
                                 ByVal OldValue As AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali,
                                 username As String,
                                 GiasContext As Gias_DeveloperServer_Entities,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                 Optional NoteLog As String = "")

        Dim tipoOperazione As New enum_TipoOperazioneDB

        Dim partQuery = (From particelle In GiasContext.ParticelleCatastali
                         Where particelle.PROV = Particella.primaryKey.Prov AndAlso
                               particelle.COM = Particella.primaryKey.Com AndAlso
                               particelle.FOGLIO = Particella.primaryKey.Foglio AndAlso
                               particelle.NUMERO = Particella.primaryKey.Numero).AsEnumerable()

        If Particella.primaryKey.Sezione = "" OrElse Particella.primaryKey.Sezione = "0" Then
            partQuery = (From x In partQuery Where x.SEZIONE = "" OrElse x.SEZIONE = "0").AsEnumerable()
        Else
            partQuery = (From x In partQuery Where x.SEZIONE.ToUpper() = Particella.primaryKey.Sezione.ToUpper).AsEnumerable()
        End If

        If Particella.primaryKey.Subalterno = "" OrElse Particella.primaryKey.Subalterno = "0" Then
            partQuery = (From x In partQuery Where x.SUBALTERNO = "" OrElse x.SUBALTERNO = "0").AsEnumerable()
        Else
            partQuery = (From x In partQuery Where x.SUBALTERNO = Particella.primaryKey.Subalterno).AsEnumerable()
        End If

        Dim part As ParticelleCatastali = partQuery.FirstOrDefault()

        If part Is Nothing Then
            tipoOperazione = enum_TipoOperazioneDB.Scrittura
        Else
            tipoOperazione = enum_TipoOperazioneDB.Modifica
        End If

        Dim particellaEdit As AgronicaCoreEntityFramework_POCO.ParticelleCatastali = Nothing
        If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then

            particellaEdit = EFParticelle.CreateParticelleCatastaliEF(GiasContext,
                                                                      objParametri_Server,
                                                                      Particella.primaryKey.Prov,
                                                                      Particella.primaryKey.Com,
                                                                      Particella.primaryKey.Sezione,
                                                                      Particella.primaryKey.Foglio,
                                                                      Particella.primaryKey.Numero,
                                                                      Particella.primaryKey.Subalterno,
                                                                      username)
            GiasContext.SaveChanges()
        Else

            particellaEdit = part

        End If

        Dim Ettari = 0
        Dim Are = 0
        Dim Centiare = 0
        Conversioni.EttariAreCentiare_from_Ettari(Particella.Area, Ettari, Are, Centiare)
        particellaEdit.ETTARI = Ettari
        particellaEdit.ARE = Are
        particellaEdit.CENTIARE = Centiare
        particellaEdit.Data_Modifica = DateTime.Now
        particellaEdit.Username_Modifica = username
        particellaEdit.Proprietario = Particella.proprietario

        GiasContext.ParticelleCatastali.Attach(particellaEdit)
        GiasContext.Entry(particellaEdit).State = EntityState.Modified
        GiasContext.SaveChanges()

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim DatiParticellaStr = JsonConvert.SerializeObject(particellaEdit, a)

        'Scrittura tabella Agronica_Log_Anagrafe
        Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
        Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ParticelleCatastali,
                                                                             CStr("PROV:" & particellaEdit.PROV & " - COM:" & particellaEdit.COM),
                                                                             CStr("SEZ:" & particellaEdit.SEZIONE & " - FOGLIO:" & particellaEdit.FOGLIO & " - NUM:" &
                                                                             particellaEdit.NUMERO),
                                                                             Nothing, Nothing, Nothing, Nothing,
                                                                             tipoOperazione,
                                                                             objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                             NoteLog, DatiParticellaStr)

        GiasContext.Agronica_Log_Anagrafe.Add(log)
        GiasContext.SaveChanges()

        If Particella.metodoProduzione IsNot Nothing Then
            ScriviMetodoProduzione(particellaEdit, Particella.metodoProduzione, tipoOperazione, username, GiasContext, objParametri_Server, objParametri_Utenti)
        End If

        If Particella.macrousi IsNot Nothing Then
            ScriviMacrousi(particellaEdit, Particella.macrousi, tipoOperazione, username, GiasContext, objParametri_Server, objParametri_Utenti)
        End If

        If Particella.zonizzazione IsNot Nothing Then
            ScriviZone(particellaEdit, Particella.zonizzazione, tipoOperazione, username, GiasContext, objParametri_Server, objParametri_Utenti)
        End If

        If Particella.classamento IsNot Nothing Then
            ScriviClassamento(particellaEdit, Particella.classamento, tipoOperazione, username, GiasContext, objParametri_Server, objParametri_Utenti)
        End If

        GiasContext.SaveChanges()

    End Sub

    Private Sub ScriviMetodoProduzione(Particella As ParticelleCatastali,
                                       metodiProduzione As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMetodoProduzione),
                                       tipoOperazione As enum_TipoOperazioneDB,
                                       username As String,
                                       GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim metodoProduzione_olds As List(Of ParticelleCatastali_MetodoProduzione)


        Dim metodoProduzione_olds_query = (From partxMac In GiasContext.ParticelleCatastali_MetodoProduzione
                                           Where partxMac.PROV = Particella.PROV AndAlso
                                                 partxMac.COM = Particella.COM AndAlso
                                                 partxMac.FOGLIO = Particella.FOGLIO AndAlso
                                                 partxMac.NUMERO = Particella.NUMERO).AsEnumerable()

        If Particella.SEZIONE = "" OrElse Particella.SEZIONE = "0" Then
            metodoProduzione_olds_query = (From q In metodoProduzione_olds_query Where q.SEZIONE = "" OrElse q.SEZIONE = "0").AsEnumerable()
        Else
            metodoProduzione_olds_query = (From q In metodoProduzione_olds_query Where q.SEZIONE.ToUpper = Particella.SEZIONE.ToUpper).AsEnumerable()
        End If

        If Particella.SUBALTERNO = "" OrElse Particella.SUBALTERNO = "0" Then
            metodoProduzione_olds_query = (From q In metodoProduzione_olds_query Where q.SUBALTERNO = "" OrElse q.SUBALTERNO = "0").AsEnumerable()
        Else
            metodoProduzione_olds_query = (From q In metodoProduzione_olds_query Where q.SUBALTERNO.ToUpper = Particella.SUBALTERNO.ToUpper).AsEnumerable()
        End If

        metodoProduzione_olds = metodoProduzione_olds_query.ToList

        GiasContext.ParticelleCatastali_MetodoProduzione.RemoveRange(metodoProduzione_olds)

        If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
            For Each metodoProduzione In metodiProduzione

                If metodoProduzione.metodoProduzione.codice > 0 Then

                    Dim mac = EFParticelle.CreateParticelleCatastali_MetodoProduzione(GiasContext,
                                                                                      objParametri_Server,
                                                                                      Particella.PROV,
                                                                                      Particella.COM,
                                                                                      Particella.SEZIONE,
                                                                                      Particella.FOGLIO,
                                                                                      Particella.NUMERO,
                                                                                      Particella.SUBALTERNO,
                                                                                      metodoProduzione.metodoProduzione.codice,
                                                                                      metodoProduzione.validita.inizio,
                                                                                      metodoProduzione.validita.fine,
                                                                                      username)

                End If

            Next
        End If


    End Sub

    Private Sub ScriviMacrousi(Particella As ParticelleCatastali,
                               macrousi As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliMacrouso),
                               tipoOperazione As enum_TipoOperazioneDB,
                               username As String,
                               GiasContext As Gias_DeveloperServer_Entities,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim macrousi_olds As List(Of ParticelleCatastalixMacrousi)

        Dim macrousi_olds_query = (From partxMac In GiasContext.ParticelleCatastalixMacrousi
                                   Where partxMac.PROV = Particella.PROV AndAlso
                                          partxMac.COM = Particella.COM AndAlso
                                          partxMac.FOGLIO = Particella.FOGLIO AndAlso
                                          partxMac.NUMERO = Particella.NUMERO).AsEnumerable()

        If Particella.SEZIONE = "" OrElse Particella.SEZIONE = "0" Then
            macrousi_olds_query = (From q In macrousi_olds_query Where q.SEZIONE = "" OrElse q.SEZIONE = "0").AsEnumerable()
        Else
            macrousi_olds_query = (From q In macrousi_olds_query Where q.SEZIONE.ToUpper = Particella.SEZIONE.ToUpper).AsEnumerable()
        End If

        If Particella.SUBALTERNO = "" OrElse Particella.SUBALTERNO = "0" Then
            macrousi_olds_query = (From q In macrousi_olds_query Where q.SUBALTERNO = "" OrElse q.SUBALTERNO = "0").AsEnumerable()
        Else
            macrousi_olds_query = (From q In macrousi_olds_query Where q.SUBALTERNO.ToUpper = Particella.SUBALTERNO.ToUpper).AsEnumerable()
        End If


        macrousi_olds = macrousi_olds_query.ToList()
        GiasContext.ParticelleCatastalixMacrousi.RemoveRange(macrousi_olds)

        If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
            For Each macrouso In macrousi

                Dim mac = EFParticelle.CreaParticelleCatastalixMacrousi(GiasContext,
                                                                        objParametri_Server,
                                                                        Particella,
                                                                        macrouso.macrouso.codice,
                                                                        username)

                mac.Validita_Inizio = macrouso.validita.inizio
                mac.Validita_Fine = macrouso.validita.fine
                mac.Superficie = macrouso.Area
                mac.PIVA = macrouso.Piva
                mac.Numero_Fascicolo = macrouso.NumeroFascicolo
                mac.Data_Validazione_Fascicolo = macrouso.DataValidazioneFascicolo

                GiasContext.ParticelleCatastalixMacrousi.Attach(mac)
                GiasContext.Entry(mac).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next
        End If

    End Sub

    Private Sub ScriviZone(Particella As ParticelleCatastali,
                           zone As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliZona),
                           tipoOperazione As enum_TipoOperazioneDB,
                           username As String,
                           GiasContext As Gias_DeveloperServer_Entities,
                           ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim zone_olds As List(Of ZonexParticelle)

        Dim zone_olds_query = (From partxZona In GiasContext.ZonexParticelle
                               Where partxZona.PROV = Particella.PROV AndAlso
                                      partxZona.COM = Particella.COM AndAlso
                                      partxZona.FOGLIO = Particella.FOGLIO AndAlso
                                      partxZona.NUMERO = Particella.NUMERO).AsEnumerable()

        If Particella.SEZIONE = "" OrElse Particella.SEZIONE = "0" Then
            zone_olds_query = (From q In zone_olds_query Where q.SEZIONE = "" OrElse q.SEZIONE = "0").AsEnumerable()
        Else
            zone_olds_query = (From q In zone_olds_query Where q.SEZIONE.ToUpper = Particella.SEZIONE.ToUpper).AsEnumerable()
        End If

        If Particella.SUBALTERNO = "" OrElse Particella.SUBALTERNO = "0" Then
            zone_olds_query = (From q In zone_olds_query Where q.SUBALTERNO = "" OrElse q.SUBALTERNO = "0").AsEnumerable()
        Else
            zone_olds_query = (From q In zone_olds_query Where q.SUBALTERNO.ToUpper = Particella.SUBALTERNO.ToUpper).AsEnumerable()
        End If

        zone_olds = zone_olds_query.ToList
        GiasContext.ZonexParticelle.RemoveRange(zone_olds)

        If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
            For Each zona In zone

                Dim zon = EFParticelle.CreaZonexParticelle(GiasContext,
                                                           objParametri_Server,
                                                           Particella,
                                                           zona.zona.codice,
                                                           username)

                zon.Area = zona.Area

                GiasContext.ZonexParticelle.Attach(zon)
                GiasContext.Entry(zon).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next
        End If

    End Sub

    Private Sub ScriviClassamento(Particella As ParticelleCatastali,
                                  classamento As List(Of AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastaliClassamento),
                                  tipoOperazione As enum_TipoOperazioneDB,
                                  username As String,
                                  GiasContext As Gias_DeveloperServer_Entities,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim classamento_olds As List(Of ParticelleCatastaliClassamento)

        Dim classamento_olds_query = (From partxclass In GiasContext.ParticelleCatastaliClassamento
                                      Where partxclass.PROV = Particella.PROV AndAlso
                                              partxclass.COM = Particella.COM AndAlso
                                              partxclass.FOGLIO = Particella.FOGLIO AndAlso
                                              partxclass.NUMERO = Particella.NUMERO).AsEnumerable()

        If Particella.SEZIONE = "" OrElse Particella.SEZIONE = "0" Then
            classamento_olds_query = (From q In classamento_olds_query Where q.SEZIONE = "" OrElse q.SEZIONE = "0").AsEnumerable()
        Else
            classamento_olds_query = (From q In classamento_olds_query Where q.SEZIONE.ToUpper = Particella.SEZIONE.ToUpper).AsEnumerable()
        End If

        If Particella.SUBALTERNO = "" OrElse Particella.SUBALTERNO = "0" Then
            classamento_olds_query = (From q In classamento_olds_query Where q.SUBALTERNO = "" OrElse q.SUBALTERNO = "0").AsEnumerable()
        Else
            classamento_olds_query = (From q In classamento_olds_query Where q.SUBALTERNO.ToUpper = Particella.SUBALTERNO.ToUpper).AsEnumerable()
        End If

        classamento_olds = classamento_olds_query.ToList()

        GiasContext.ParticelleCatastaliClassamento.RemoveRange(classamento_olds)

        If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione Then
            For Each classam In classamento

                Dim clas = EFParticelle.CreaParticelleCatastaliClassamento(GiasContext,
                                                                           objParametri_Server,
                                                                           Particella,
                                                                           classam.qualita.codice,
                                                                           username)

                clas.Porzione = classam.porzione
                clas.REDDITO_AGRARIO = classam.redditoAgrario
                clas.REDDITO_DOMINICALE = classam.redditoDomiciliare
                clas.CLASSE = classam.classe
                clas.Sup_Classe = classam.Area


                GiasContext.ParticelleCatastaliClassamento.Attach(clas)
                GiasContext.Entry(clas).State = EntityState.Modified
                GiasContext.SaveChanges()

            Next
        End If

    End Sub

    Private Sub ScriviPossessi(Particella As AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK,
                               Centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                               possessi As List(Of AgronicaCoreModelsSTD.anagrafiche.PossessoParticella),
                               username As String,
                               GiasContext As Gias_DeveloperServer_Entities,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                               Optional NoteLog As String = "")

        If Centro.codice = 0 Then
            Throw New GiasException("Selezionare centro")
        End If

        Dim possessiCod As List(Of Integer) = possessi.Where(Function(x) x.flag_cancellazione = False).Select(Function(x) x.codice).ToList
        Dim impresexparticelle_old As List(Of ImpreseXParticelle)
        Dim impresexparticelle_old_query = (From impxpart In GiasContext.ImpreseXParticelle
                                            Where impxpart.PROV = Particella.Prov AndAlso
                                                  impxpart.COM = Particella.Com AndAlso
                                                  impxpart.FOGLIO = Particella.Foglio AndAlso
                                                  impxpart.NUMERO = Particella.Numero AndAlso
                                                  impxpart.PIVA = Centro.partitaIva AndAlso
                                                  impxpart.sa_cod = Centro.codice AndAlso
                                                  Not possessiCod.Contains(impxpart.ID)).AsEnumerable()

        If Particella.Sezione = "" OrElse Particella.Sezione = "0" Then
            impresexparticelle_old_query = (From q In impresexparticelle_old_query Where q.SEZIONE = "" OrElse q.SEZIONE = "0").AsEnumerable()
        Else
            impresexparticelle_old_query = (From q In impresexparticelle_old_query Where q.SEZIONE = Particella.Sezione).AsEnumerable()
        End If

        If Particella.Subalterno = "" OrElse Particella.Subalterno = "0" Then
            impresexparticelle_old_query = (From q In impresexparticelle_old_query Where q.SUBALTERNO = "" OrElse q.SUBALTERNO = "0").AsEnumerable()
        Else
            impresexparticelle_old_query = (From q In impresexparticelle_old_query Where q.SUBALTERNO = Particella.Subalterno).AsEnumerable()
        End If

        impresexparticelle_old_query = (From x In impresexparticelle_old_query Where Not possessiCod.Contains(x.ID)).AsEnumerable()

        impresexparticelle_old = impresexparticelle_old_query.ToList()

        Dim impresexParticelle_codici_old As List(Of ImpreseXParticelle_Codici)
        Dim impresexParticelle_old_id As List(Of Integer) = (From a In impresexparticelle_old Select a.ID).ToList()
        Dim impresexParticelle_codici_old_query = (From ixpc In GiasContext.ImpreseXParticelle_Codici Where impresexParticelle_old_id.Contains(ixpc.ID))
        impresexParticelle_codici_old = impresexParticelle_codici_old_query.ToList()

        GiasContext.ImpreseXParticelle.RemoveRange(impresexparticelle_old)
        GiasContext.ImpreseXParticelle_Codici.RemoveRange(impresexParticelle_codici_old)

        For Each possesso In possessi
            Dim tipoOperazione As enum_TipoOperazioneDB
            If Not possesso.flag_cancellazione Then
                'And PermessoModificaPossessi(possesso, GiasContext) Then
                Dim ixp As AgronicaCoreEntityFramework_POCO.ImpreseXParticelle

                If possesso.codice = 0 Then
                    tipoOperazione = enum_TipoOperazioneDB.Scrittura
                    ixp = EFParticelle.CreateImpresexParticelle(GiasContext,
                                                                objParametri_Server,
                                                                Centro.partitaIva,
                                                                Centro.codice,
                                                                Particella.Prov,
                                                                Particella.Com,
                                                                Particella.Sezione,
                                                                Particella.Foglio,
                                                                Particella.Numero,
                                                                Particella.Subalterno,
                                                                username)

                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica

                    Dim codice = possesso.codice
                    ixp = (From i In GiasContext.ImpreseXParticelle Where i.ID = codice).FirstOrDefault

                End If

                If ixp Is Nothing Then
                    Throw New GiasException("ImpreseXParticelle non trovata")
                End If


                ixp.TitoloPossesso = possesso.titolo_Di_Possesso.codice
                ixp.Sup_Condotta = possesso.Area
                ixp.Validita_Inizio = possesso.validita.inizio
                ixp.Validita_Fine = possesso.validita.fine

                ixp.Data_Modifica = DateTime.Now
                ixp.Username_Modifica = objParametri_Utenti.UsernameOperazione

                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                Dim DatiImpresexParticelleStr = JsonConvert.SerializeObject(impresexparticelle_old_query, a)

                'Scrittura tabella Agronica_Log_Anagrafe
                Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(enum_TipoEntita_Des.ImpreseXParticelle,
                                                                                     CStr(Centro.partitaIva), CStr(Centro.codice),
                                                                                     CStr("ID:" & possesso.codice),
                                                                                     CStr("PROV:" & Particella.Prov & " - COM:" & Particella.Com),
                                                                                     CStr("SEZ:" & Particella.Sezione & " - FOGLIO:" & Particella.Foglio & " - NUM:" &
                                                                                     Particella.Numero),
                                                                                     Nothing,
                                                                                     tipoOperazione,
                                                                                     objParametri_Server, enum_Id_Servizio.GiasOnline,
                                                                                     NoteLog, DatiImpresexParticelleStr)

                GiasContext.Agronica_Log_Anagrafe.Add(log)

                ScriviImpresexParticelle_Codici(ixp.ID,
                                                    enum_CodiciAnagrafe.CodiceParticella,
                                                    possesso.codice_particella,
                                                    username,
                                                    objParametri_Server,
                                                    GiasContext,
                                                    False)

                GiasContext.ImpreseXParticelle.Attach(ixp)
                GiasContext.Entry(ixp).State = EntityState.Modified
                GiasContext.SaveChanges()
            End If

        Next
    End Sub

    Private Sub ScriviImpresexParticelle_Codici(id As Integer,
                                                idcod As Integer,
                                                valcod As String,
                                                ByVal username As String,
                                                ByRef objParametriServer As AgronicaCoreParametri,
                                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                Optional ByVal NewTransaction As Boolean = True)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.EFAppezzamento.ScriviModificaEliminaAppezzamentoCodici()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try

            If idcod <> 0 Then

                Dim app_codl = (From ic In GiasContext.ImpreseXParticelle_Codici
                                Where ic.ID = id AndAlso
                                     ic.Id_Cod = idcod
                                Select ic).ToList()

                Dim operazione As enum_TipoOperazioneDB

                If app_codl.Count > 0 AndAlso (valcod <> "0" AndAlso valcod <> "") Then
                    operazione = enum_TipoOperazioneDB.Modifica
                ElseIf app_codl.Count > 0 AndAlso (valcod = "0" OrElse valcod = "") Then
                    operazione = enum_TipoOperazioneDB.Cancellazione
                ElseIf app_codl.Count = 0 AndAlso (valcod = "" OrElse valcod = "0") Then
                    operazione = enum_TipoOperazioneDB.Lettura
                ElseIf app_codl.Count = 0 AndAlso (valcod <> "" AndAlso valcod <> "0") Then
                    operazione = enum_TipoOperazioneDB.Scrittura
                End If

                Select Case operazione
                    Case enum_TipoOperazioneDB.Scrittura

                        Dim app_cod As New AgronicaCoreEntityFramework_POCO.ImpreseXParticelle_Codici With {
                            .ID = id,
                            .Id_Cod = idcod,
                            .Val_Cod = valcod,
                            .inviato = 0,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = username,
                            .Username_Modifica = username
                        }

                        GiasContext.ImpreseXParticelle_Codici.Add(app_cod)

                    Case enum_TipoOperazioneDB.Modifica
                        Dim app_cod = app_codl.FirstOrDefault

                        If app_cod IsNot Nothing Then
                            app_cod.Val_Cod = valcod
                            app_cod.Data_Modifica = DateTime.Now
                            app_cod.Username_Modifica = username

                            GiasContext.ImpreseXParticelle_Codici.Attach(app_cod)
                            GiasContext.Entry(app_cod).State = EntityState.Modified
                        End If

                    Case enum_TipoOperazioneDB.Cancellazione

                        Dim app_cod = app_codl.FirstOrDefault
                        GiasContext.ImpreseXParticelle_Codici.Attach(app_cod)
                        GiasContext.ImpreseXParticelle_Codici.Remove(app_cod)

                End Select

                GiasContext.SaveChanges()

            End If
            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub


    Private Sub ParticellaCambioChiave(ByVal Particella As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                       ByVal OldValue As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                       username As String,
                                       GiasContext As Gias_DeveloperServer_Entities,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim esisteParticella = (From part In GiasContext.ParticelleCatastali
                                Where part.PROV = Particella.particella.primaryKey.Prov AndAlso
                                      part.COM = Particella.particella.primaryKey.Com AndAlso
                                      part.SEZIONE = Particella.particella.primaryKey.Sezione AndAlso
                                      part.FOGLIO = Particella.particella.primaryKey.Foglio AndAlso
                                      part.NUMERO = Particella.particella.primaryKey.Numero AndAlso
                                      part.SUBALTERNO = Particella.particella.primaryKey.Subalterno).FirstOrDefault()

        If esisteParticella IsNot Nothing Then
            Throw New GiasException("Particella già presente in archivio")
        End If

        Dim particellaOriginale = (From part In GiasContext.ParticelleCatastali
                                   Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                         part.COM = OldValue.particella.primaryKey.Com AndAlso
                                         part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                         part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                         part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                         part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).FirstOrDefault()

        Dim copy = Gias_EF_Utility.CopyEntity(GiasContext, particellaOriginale, Nothing, username, DateTime.Now)
        GiasContext.ParticelleCatastali.Remove(particellaOriginale)

        copy.PROV = Particella.particella.primaryKey.Prov
        copy.COM = Particella.particella.primaryKey.Com
        copy.SEZIONE = Particella.particella.primaryKey.Sezione
        copy.FOGLIO = Particella.particella.primaryKey.Foglio
        copy.NUMERO = Particella.particella.primaryKey.Numero
        copy.SUBALTERNO = Particella.particella.primaryKey.Subalterno

        GiasContext.ParticelleCatastali.Add(copy)
        GiasContext.SaveChanges()

        Dim impresexpart = (From part In GiasContext.ImpreseXParticelle
                            Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                  part.COM = OldValue.particella.primaryKey.Com AndAlso
                                  part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                  part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                  part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                  part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()
        Dim listImpresexPart As New List(Of ImpreseXParticelle)

        For Each impresaxpart In impresexpart
            Dim copyImpresaxpart = Gias_EF_Utility.CopyEntity(GiasContext, impresaxpart, Nothing, username, DateTime.Now)
            copyImpresaxpart.PROV = Particella.particella.primaryKey.Prov
            copyImpresaxpart.COM = Particella.particella.primaryKey.Com
            copyImpresaxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyImpresaxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyImpresaxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyImpresaxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            GiasContext.ImpreseXParticelle.Add(copyImpresaxpart)
            GiasContext.SaveChanges()
            Dim possesso = (From p In Particella.possessiParticella Where p.codice = impresaxpart.ID).FirstOrDefault()
            If possesso IsNot Nothing Then
                possesso.codice = copyImpresaxpart.ID
            End If
        Next

        GiasContext.ImpreseXParticelle.RemoveRange(impresexpart)

        Dim macrousixpart = (From part In GiasContext.ParticelleCatastalixMacrousi
                             Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                   part.COM = OldValue.particella.primaryKey.Com AndAlso
                                   part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                   part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                   part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                   part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listMacrousixPart As New List(Of ParticelleCatastalixMacrousi)

        For Each macrousoxpart In macrousixpart
            Dim copyMacrousoxpart = Gias_EF_Utility.CopyEntity(GiasContext, macrousoxpart, Nothing, username, DateTime.Now)
            copyMacrousoxpart.PROV = Particella.particella.primaryKey.Prov
            copyMacrousoxpart.COM = Particella.particella.primaryKey.Com
            copyMacrousoxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyMacrousoxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyMacrousoxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyMacrousoxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listMacrousixPart.Add(copyMacrousoxpart)
        Next

        GiasContext.ParticelleCatastalixMacrousi.RemoveRange(macrousixpart)
        GiasContext.ParticelleCatastalixMacrousi.AddRange(listMacrousixPart)


        Dim zonexpart = (From part In GiasContext.ZonexParticelle
                         Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                               part.COM = OldValue.particella.primaryKey.Com AndAlso
                               part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                               part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                               part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                               part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listZonexPart As New List(Of ZonexParticelle)

        For Each zonaxpart In zonexpart
            Dim copyZonaxpart = Gias_EF_Utility.CopyEntity(GiasContext, zonaxpart, Nothing, username, DateTime.Now)
            copyZonaxpart.PROV = Particella.particella.primaryKey.Prov
            copyZonaxpart.COM = Particella.particella.primaryKey.Com
            copyZonaxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyZonaxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyZonaxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyZonaxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listZonexPart.Add(copyZonaxpart)
        Next

        GiasContext.ZonexParticelle.RemoveRange(zonexpart)
        GiasContext.ZonexParticelle.AddRange(listZonexPart)



        Dim classamentoxpart = (From part In GiasContext.ParticelleCatastaliClassamento
                                Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                      part.COM = OldValue.particella.primaryKey.Com AndAlso
                                      part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                      part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                      part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                      part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listClassamentixPart As New List(Of ParticelleCatastaliClassamento)

        For Each classxpart In classamentoxpart
            Dim copyClassxpart = Gias_EF_Utility.CopyEntity(GiasContext, classxpart, Nothing, username, DateTime.Now)
            copyClassxpart.PROV = Particella.particella.primaryKey.Prov
            copyClassxpart.COM = Particella.particella.primaryKey.Com
            copyClassxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyClassxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyClassxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyClassxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listClassamentixPart.Add(copyClassxpart)
        Next

        GiasContext.ParticelleCatastaliClassamento.RemoveRange(classamentoxpart)
        GiasContext.ParticelleCatastaliClassamento.AddRange(listClassamentixPart)



        Dim campixpart = (From part In GiasContext.CampiXParticelle
                          Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                part.COM = OldValue.particella.primaryKey.Com AndAlso
                                part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listCampixPart As New List(Of CampiXParticelle)

        For Each campoxpart In campixpart
            Dim copyCampoxpart = Gias_EF_Utility.CopyEntity(GiasContext, campoxpart, Nothing, username, DateTime.Now)
            copyCampoxpart.PROV = Particella.particella.primaryKey.Prov
            copyCampoxpart.COM = Particella.particella.primaryKey.Com
            copyCampoxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyCampoxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyCampoxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyCampoxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listCampixPart.Add(copyCampoxpart)
        Next

        GiasContext.CampiXParticelle.RemoveRange(campixpart)
        GiasContext.CampiXParticelle.AddRange(listCampixPart)


        Dim appxpart = (From part In GiasContext.AppezzamentiXParticelle
                        Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                              part.COM = OldValue.particella.primaryKey.Com AndAlso
                              part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                              part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                              part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                              part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listAppxPart As New List(Of AppezzamentiXParticelle)

        For Each appezzamentoxpart In appxpart
            Dim copyAppezzamentoxpart = Gias_EF_Utility.CopyEntity(GiasContext, appezzamentoxpart, Nothing, username, DateTime.Now)
            copyAppezzamentoxpart.PROV = Particella.particella.primaryKey.Prov
            copyAppezzamentoxpart.COM = Particella.particella.primaryKey.Com
            copyAppezzamentoxpart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyAppezzamentoxpart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyAppezzamentoxpart.NUMERO = Particella.particella.primaryKey.Numero
            copyAppezzamentoxpart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listAppxPart.Add(copyAppezzamentoxpart)
        Next

        GiasContext.AppezzamentiXParticelle.RemoveRange(appxpart)
        GiasContext.AppezzamentiXParticelle.AddRange(listAppxPart)

        Dim metodoProduzionePart = (From part In GiasContext.ParticelleCatastali_MetodoProduzione
                                    Where part.PROV = OldValue.particella.primaryKey.Prov AndAlso
                                          part.COM = OldValue.particella.primaryKey.Com AndAlso
                                          part.SEZIONE = OldValue.particella.primaryKey.Sezione AndAlso
                                          part.FOGLIO = OldValue.particella.primaryKey.Foglio AndAlso
                                          part.NUMERO = OldValue.particella.primaryKey.Numero AndAlso
                                          part.SUBALTERNO = OldValue.particella.primaryKey.Subalterno).ToList()

        Dim listmetodoProduzionePart As New List(Of ParticelleCatastali_MetodoProduzione)

        For Each metodoProdxPart In metodoProduzionePart
            Dim copyMetodoProdxPart = Gias_EF_Utility.CopyEntity(GiasContext, metodoProdxPart, Nothing, username, DateTime.Now)
            copyMetodoProdxPart.PROV = Particella.particella.primaryKey.Prov
            copyMetodoProdxPart.COM = Particella.particella.primaryKey.Com
            copyMetodoProdxPart.SEZIONE = Particella.particella.primaryKey.Sezione
            copyMetodoProdxPart.FOGLIO = Particella.particella.primaryKey.Foglio
            copyMetodoProdxPart.NUMERO = Particella.particella.primaryKey.Numero
            copyMetodoProdxPart.SUBALTERNO = Particella.particella.primaryKey.Subalterno
            listmetodoProduzionePart.Add(copyMetodoProdxPart)
        Next

        GiasContext.ParticelleCatastali_MetodoProduzione.RemoveRange(metodoProduzionePart)
        GiasContext.ParticelleCatastali_MetodoProduzione.AddRange(listmetodoProduzionePart)

        GiasContext.SaveChanges()

    End Sub

    '###########################################################################################
    '################################# Cancellazione Particella ################################
    '###########################################################################################

    Public Sub Particella_Elimina(ByVal Particella As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                  username As String,
                                  GiasContext As Gias_DeveloperServer_Entities,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                  Optional NoteLog As String = "")

        Dim messaggioErrore As String = ""

        Try
            Dim viaLiberaCancellazione As Boolean = True
            Dim cancellaSoloPossesso As Boolean = False
            Dim PivaRiferimento = Particella.centro.partitaIva
            Dim SaCodRiferimento = Particella.centro.codice

            Dim part = (From particelle In GiasContext.ParticelleCatastali
                        Where particelle.PROV = Particella.particella.primaryKey.Prov AndAlso
                              particelle.COM = Particella.particella.primaryKey.Com AndAlso
                              particelle.SEZIONE = Particella.particella.primaryKey.Sezione AndAlso
                              particelle.FOGLIO = Particella.particella.primaryKey.Foglio AndAlso
                              particelle.NUMERO = Particella.particella.primaryKey.Numero AndAlso
                              particelle.SUBALTERNO = Particella.particella.primaryKey.Subalterno).FirstOrDefault()

            'If part Is Nothing AndAlso (Particella.particella.primaryKey.Sezione = "0" OrElse Particella.particella.primaryKey.Subalterno = "0") Then

            'End If

            Dim objImpreseXParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim part_ImpreseXParticellePivaSaCod = objImpreseXParticelle.Leggi(
                0,
                "",
                0,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
                )

            Dim dt_PivaSaCods = part_ImpreseXParticellePivaSaCod.DefaultView.ToTable(True, "Piva", "Sa_Cod")
            If dt_PivaSaCods.Rows.Count > 1 Then
                cancellaSoloPossesso = True
            End If

            Dim objCampiXParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            Dim part_CampiXParticellePivaSaCod = objCampiXParticelle.Leggi(
                PivaRiferimento,
                SaCodRiferimento,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
                )

            Dim part_CampiXParticelle = objCampiXParticelle.Leggi(
                "",
                0,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
                )

            If part_CampiXParticelle.Rows.Count <> 0 Then
                viaLiberaCancellazione = True
                cancellaSoloPossesso = True
            End If

            If part_CampiXParticellePivaSaCod.Rows.Count <> 0 Then
                viaLiberaCancellazione = False
                Throw New GiasException("Impossibile eliminare dato, ci sono campi associati alla particella")
            End If

            Dim objAppezzamentiXParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim part_AppezzamentiXParticellePivaSaCod = objAppezzamentiXParticelle.AppezzamentixParticelle_Leggi(
                PivaRiferimento,
                SaCodRiferimento,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server,
                True
                )

            Dim part_AppezzamentiXParticelle = objAppezzamentiXParticelle.AppezzamentixParticelle_Leggi(
                "",
                0,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server,
                True
                )

            If part_AppezzamentiXParticelle.Rows.Count <> 0 Then
                viaLiberaCancellazione = True
                cancellaSoloPossesso = True
            End If

            If part_AppezzamentiXParticellePivaSaCod.Rows.Count <> 0 Then
                viaLiberaCancellazione = False
                Throw New GiasException("Impossibile eliminare dato, ci sono appezzamenti associati alla particella")
            End If

            Dim objProgrammazione_Particelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            Dim part_Programmazione_Particelle = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(
                objParametri_Server,
                "",
                "",
                0,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                AGRODATAINIZIO,
                AGRODATAFINE,
                False
                )

            Dim part_Programmazione_ParticellePivaSaCod = objProgrammazione_Particelle.Programmazione_Particelle_Leggi(
                objParametri_Server,
                PivaRiferimento,
                "",
                0,
                0,
                Particella.particella.primaryKey.Prov,
                Particella.particella.primaryKey.Com,
                Particella.particella.primaryKey.Sezione,
                Particella.particella.primaryKey.Foglio,
                Particella.particella.primaryKey.Numero,
                Particella.particella.primaryKey.Subalterno,
                AGRODATAINIZIO,
                AGRODATAFINE,
                False,
                Particella.centro.codice
                )

            If part_Programmazione_ParticellePivaSaCod.Rows.Count <> 0 Then
                viaLiberaCancellazione = False
                Throw New GiasException("Impossibile eliminare dato, ci sono piani colturali pianificati associati alla particella")
            End If

            If part_Programmazione_Particelle.Rows.Count <> 0 Then
                viaLiberaCancellazione = True
                cancellaSoloPossesso = True
            End If

            Dim particellaEdit As AgronicaCoreEntityFramework_POCO.ParticelleCatastali = Nothing
            If viaLiberaCancellazione Then

                If Not cancellaSoloPossesso Then
                    particellaEdit = part

                    GiasContext.ParticelleCatastali.Remove(particellaEdit)

                    Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                    Dim DatiParticellaStr = JsonConvert.SerializeObject(particellaEdit, a)

                    'Scrittura tabella Agronica_Log_Anagrafe
                    Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                    Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                        enum_TipoEntita_Des.ParticelleCatastali,
                        CStr("PROV:" & particellaEdit.PROV & " - COM:" & particellaEdit.COM),
                        CStr("SEZ:" & particellaEdit.SEZIONE & " - FOGLIO:" & particellaEdit.FOGLIO & " - NUM:" & particellaEdit.NUMERO),
                        Nothing,
                        Nothing,
                        Nothing,
                        Nothing,
                        enum_TipoOperazioneDB.Cancellazione,
                        objParametri_Server,
                        enum_Id_Servizio.GiasOnline,
                        NoteLog,
                        DatiParticellaStr
                        )

                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()


                    ScriviMetodoProduzione(particellaEdit, Particella.particella.metodoProduzione, enum_TipoOperazioneDB.Cancellazione, username, GiasContext, objParametri_Server, objParametri_Utenti)

                    ScriviMacrousi(particellaEdit, Particella.particella.macrousi, enum_TipoOperazioneDB.Cancellazione, username, GiasContext, objParametri_Server, objParametri_Utenti)

                    ScriviZone(particellaEdit, Particella.particella.zonizzazione, enum_TipoOperazioneDB.Cancellazione, username, GiasContext, objParametri_Server, objParametri_Utenti)

                    ScriviClassamento(particellaEdit, Particella.particella.classamento, enum_TipoOperazioneDB.Cancellazione, username, GiasContext, objParametri_Server, objParametri_Utenti)
                End If

                ScriviPossessi(Particella.particella.primaryKey, Particella.centro, Particella.possessiParticella, username, GiasContext, objParametri_Server, objParametri_Utenti)

                If Not cancellaSoloPossesso Then
                    Elimina_ImpreseXParticelle(particellaEdit, GiasContext)

                    Elimina_ImpresexParticelle_Contatti(particellaEdit, GiasContext)

                    Elimina_ParticelleCatastali_Codici(particellaEdit, GiasContext)

                    Elimina_ParticelleCatastali_MetodoProduzione(particellaEdit, GiasContext)

                    Elimina_ParticelleCatastalixEleggibilitaParticelle(particellaEdit, GiasContext)

                    Elimina_ParticelleCatastalixMacrousixUtilizzo(particellaEdit, GiasContext)

                    Elimina_ParticelleCatastalixVincoliAgronomici(particellaEdit, GiasContext)

                    Elimina_PoligonoGIS(particellaEdit, GiasContext)
                End If

                'Elimina_ImpreseXParticelle_Codici(Particella, GiasContext)

                GiasContext.SaveChanges()
            End If

        Catch ex As GiasException
            Throw ex
        Catch ex As Exception
            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = " " & ex.Message
            '//////////////////////////////////////////////////////////////////////

            Throw New Exception(messaggioErrore)
        End Try

    End Sub


    Private Sub Elimina_ImpreseXParticelle(Particella As ParticelleCatastali,
                                           GiasContext As Gias_DeveloperServer_Entities)

        Dim impreseXParticelle = (From impxpart In GiasContext.ImpreseXParticelle
                                  Where impxpart.PROV = Particella.PROV AndAlso
                                        impxpart.COM = Particella.COM AndAlso
                                        impxpart.SEZIONE = Particella.SEZIONE AndAlso
                                        impxpart.FOGLIO = Particella.FOGLIO AndAlso
                                        impxpart.NUMERO = Particella.NUMERO AndAlso
                                        impxpart.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ImpreseXParticelle.RemoveRange(impreseXParticelle)

    End Sub

    Private Sub Elimina_ImpreseXParticelle_Codici(ByVal Particella As AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale,
                                                  GiasContext As Gias_DeveloperServer_Entities)

        Dim ID = Particella.possessiParticella(0).codice

        Dim impreseXParticelle_codici = (From impxpart_cod In GiasContext.ImpreseXParticelle_Codici
                                         Where impxpart_cod.ID = ID).ToList
        GiasContext.ImpreseXParticelle_Codici.RemoveRange(impreseXParticelle_codici)

    End Sub

    Private Sub Elimina_ImpresexParticelle_Contatti(Particella As ParticelleCatastali,
                                                    GiasContext As Gias_DeveloperServer_Entities)

        Dim impreseXParticelle_Contatti = (From impxpart_cont In GiasContext.ImpresexParticelle_Contatti
                                           Where impxpart_cont.PROV = Particella.PROV AndAlso
                                                 impxpart_cont.COM = Particella.COM AndAlso
                                                 impxpart_cont.SEZIONE = Particella.SEZIONE AndAlso
                                                 impxpart_cont.FOGLIO = Particella.FOGLIO AndAlso
                                                 impxpart_cont.NUMERO = Particella.NUMERO AndAlso
                                                 impxpart_cont.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ImpresexParticelle_Contatti.RemoveRange(impreseXParticelle_Contatti)

    End Sub

    Private Sub Elimina_ParticelleCatastali_Codici(Particella As ParticelleCatastali,
                                                   GiasContext As Gias_DeveloperServer_Entities)

        Dim particelleCatastali_Codici = (From partcat_cod In GiasContext.ParticelleCatastali_Codici
                                          Where partcat_cod.PROV = Particella.PROV AndAlso
                                                partcat_cod.COM = Particella.COM AndAlso
                                                partcat_cod.SEZIONE = Particella.SEZIONE AndAlso
                                                partcat_cod.FOGLIO = Particella.FOGLIO AndAlso
                                                partcat_cod.NUMERO = Particella.NUMERO AndAlso
                                                partcat_cod.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ParticelleCatastali_Codici.RemoveRange(particelleCatastali_Codici)

    End Sub

    Private Sub Elimina_ParticelleCatastali_MetodoProduzione(Particella As ParticelleCatastali,
                                                             GiasContext As Gias_DeveloperServer_Entities)

        Dim particelleCatastali_MetodoProduzione = (From partcat_metprod In GiasContext.ParticelleCatastali_MetodoProduzione
                                                    Where partcat_metprod.PROV = Particella.PROV AndAlso
                                                          partcat_metprod.COM = Particella.COM AndAlso
                                                          partcat_metprod.SEZIONE = Particella.SEZIONE AndAlso
                                                          partcat_metprod.FOGLIO = Particella.FOGLIO AndAlso
                                                          partcat_metprod.NUMERO = Particella.NUMERO AndAlso
                                                          partcat_metprod.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ParticelleCatastali_MetodoProduzione.RemoveRange(particelleCatastali_MetodoProduzione)

    End Sub

    Private Sub Elimina_ParticelleCatastalixEleggibilitaParticelle(Particella As ParticelleCatastali,
                                                                   GiasContext As Gias_DeveloperServer_Entities)

        Dim particelleCatastaliXEleggibilitaParticelle = (From partcatxelegpart In GiasContext.ParticelleCatastalixEleggibilitaParticelle
                                                          Where partcatxelegpart.PROV = Particella.PROV AndAlso
                                                                partcatxelegpart.COM = Particella.COM AndAlso
                                                                partcatxelegpart.SEZIONE = Particella.SEZIONE AndAlso
                                                                partcatxelegpart.FOGLIO = Particella.FOGLIO AndAlso
                                                                partcatxelegpart.NUMERO = Particella.NUMERO AndAlso
                                                                partcatxelegpart.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ParticelleCatastalixEleggibilitaParticelle.RemoveRange(particelleCatastaliXEleggibilitaParticelle)

    End Sub

    Private Sub Elimina_ParticelleCatastalixMacrousixUtilizzo(Particella As ParticelleCatastali,
                                                              GiasContext As Gias_DeveloperServer_Entities)

        Dim particelleCatastaliXMacrousiUtilizzo = (From partcatxmacutil In GiasContext.ParticelleCatastalixMacrousixUtilizzo
                                                    Where partcatxmacutil.PROV = Particella.PROV AndAlso
                                                          partcatxmacutil.COM = Particella.COM AndAlso
                                                          partcatxmacutil.SEZIONE = Particella.SEZIONE AndAlso
                                                          partcatxmacutil.FOGLIO = Particella.FOGLIO AndAlso
                                                          partcatxmacutil.NUMERO = Particella.NUMERO AndAlso
                                                          partcatxmacutil.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ParticelleCatastalixMacrousixUtilizzo.RemoveRange(particelleCatastaliXMacrousiUtilizzo)

    End Sub

    Private Sub Elimina_ParticelleCatastalixVincoliAgronomici(Particella As ParticelleCatastali,
                                                              GiasContext As Gias_DeveloperServer_Entities)

        Dim particelleCatastaliXVincoliAgronomici = (From partcatxvincagro In GiasContext.ParticelleCatastalixVincoliAgronomici
                                                     Where partcatxvincagro.PROV = Particella.PROV AndAlso
                                                           partcatxvincagro.COM = Particella.COM AndAlso
                                                           partcatxvincagro.SEZIONE = Particella.SEZIONE AndAlso
                                                           partcatxvincagro.FOGLIO = Particella.FOGLIO AndAlso
                                                           partcatxvincagro.NUMERO = Particella.NUMERO AndAlso
                                                           partcatxvincagro.SUBALTERNO = Particella.SUBALTERNO).ToList()
        GiasContext.ParticelleCatastalixVincoliAgronomici.RemoveRange(particelleCatastaliXVincoliAgronomici)

    End Sub

    Private Function PermessoModificaPossessi(Possesso As AgronicaCoreModelsSTD.anagrafiche.PossessoParticella, GiasContext As Gias_DeveloperServer_Entities)
        Dim corrispondenza = (From particelleX In GiasContext.ImpreseXParticelle Where particelleX.ID = Possesso.codice And
                                                                            particelleX.Validita_Inizio = Possesso.validita.inizio And
                                                                            particelleX.Validita_Fine = Possesso.validita.fine).ToList
        Return True
    End Function

    Private Sub Elimina_PoligonoGIS(particella As ParticelleCatastali,
                                    GiasContext As Gias_DeveloperServer_Entities)

        Dim entita = (From gis_entita In GiasContext.GIS_Entita
                      Where gis_entita.PROV = particella.PROV AndAlso
                          gis_entita.COM = particella.COM AndAlso
                          gis_entita.SEZIONE = particella.SEZIONE AndAlso
                          gis_entita.FOGLIO = particella.FOGLIO AndAlso
                          gis_entita.NUMERO = particella.NUMERO AndAlso
                          gis_entita.SUBALTERNO = particella.SUBALTERNO AndAlso
                          gis_entita.TipoEntita_Cod = enum_Gis_LayerElementiGrafici_std.CATASTO
                          ).FirstOrDefault()

        If entita IsNot Nothing Then
            Dim entita_cod = entita.Entita_Cod

            Dim elementoGrafico = (From gis_elementoGrafico In GiasContext.GIS_ElementiGrafici
                                   Where gis_elementoGrafico.Entita_Cod = entita_cod AndAlso
                                       gis_elementoGrafico.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.CATASTO
                                       ).FirstOrDefault()

            If elementoGrafico IsNot Nothing Then
                GiasContext.GIS_ElementiGrafici.Remove(elementoGrafico)
            End If

            GiasContext.GIS_Entita.Remove(entita)
        End If
    End Sub

End Class

