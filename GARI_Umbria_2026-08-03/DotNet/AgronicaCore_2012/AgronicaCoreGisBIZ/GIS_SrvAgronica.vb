
Imports <xmlns="http://www.agronica.it/grafica/">


Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.XPath
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaGIS2012.Commons
Imports Newtonsoft.Json

Public Class GIS_SrvAgronica_cfg
    Public Property url As String
    Public Property token As String
    Public Property filtroQueryCatasto As String
    Public Property layerDaAllineare As String
    Public Property MaxNumElementiPerChiamata As Integer
End Class

Public Class GIS_SrvAgronica

    Public Sub New()

    End Sub

    Public Function GIS_SRVAgronica_AllineaDati(ByVal cfg As GIS_SrvAgronica_cfg, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rvalTot As New RispostaStandard

        Dim vLayer As String() = cfg.layerDaAllineare.Split(",")

        For Each LayerElemCod In vLayer

            Select Case LayerElemCod
                Case enum_Gis_LayerElementiGrafici_std.CATASTO
                    Dim rvalCatasto As RispostaStandard =
                        GIS_SRVAgronica_AllineaDati_Catasto(cfg, objParametri_Server)

                    rvalTot.RispostaOK = rvalTot.RispostaOK And rvalCatasto.RispostaOK
                    rvalTot.RispostaStringa &= rvalCatasto.RispostaStringa
                    rvalTot.Errore &= rvalCatasto.Errore

            End Select

        Next


        Return rvalTot

    End Function

    Private Function GIS_SRVAgronica_AllineaDati_Catasto(ByVal cfg As GIS_SrvAgronica_cfg, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rvalComplessivo As New RispostaStandard


        'recupera parametri in ingresso.
        Dim xmlpar As RispostaStandard =
            GIS_SRVAgronica_AllineaDati_Catasto_RecuperaParametri(cfg, objParametri_Server)

        Dim xDoc As XDocument = XDocument.Parse(xmlpar.RispostaStringa)

        Dim particelle = (From elem In xDoc.Element("catasto").Elements("particella")).ToList


        Dim conteggioChunck As Integer = 0
        Dim xmlParChunk As XDocument = XDocument.Parse("<catasto></catasto>")

        For Each p In particelle


            If conteggioChunck = cfg.MaxNumElementiPerChiamata Then

                Dim rispostaChunk As RispostaStandard =
                    AllineaDati_Catasto_ScriviChunk(cfg, objParametri_Server, xmlParChunk)

                rvalComplessivo.RispostaOK = rvalComplessivo.RispostaOK And rispostaChunk.RispostaOK
                rvalComplessivo.RispostaStringa &= rispostaChunk.RispostaStringa
                rvalComplessivo.Errore &= rispostaChunk.Errore

                'reset oggetto appoggio                
                xmlParChunk = XDocument.Parse("<catasto></catasto>")
                conteggioChunck = 0

            Else

                'continua aggiunta ... 
                xmlParChunk.Root.Add(p)
                conteggioChunck += 1

            End If
            'chunk raggiunto

        Next
        'list delle particelle

        If conteggioChunck > 0 Then
            Dim rispostaChunk As RispostaStandard =
                        AllineaDati_Catasto_ScriviChunk(cfg, objParametri_Server, xmlParChunk)

            rvalComplessivo.RispostaOK = rvalComplessivo.RispostaOK And rispostaChunk.RispostaOK
            rvalComplessivo.RispostaStringa &= rispostaChunk.RispostaStringa
            rvalComplessivo.Errore &= rispostaChunk.Errore


        End If
        'fine quanto non ancora richiesto ... 



        Return rvalComplessivo

    End Function

    Private Function AllineaDati_Catasto_ScriviChunk(cfg As GIS_SrvAgronica_cfg, objParametri_Server As AgronicaCoreParametri, xmlParChunk As XDocument) As RispostaStandard

        Dim strXmlDatiRemoti As String
        Dim obFiltroParametri As New StringBuilder
        obFiltroParametri.Append("{ ""InData"": """)
        obFiltroParametri.Append(AgronicaCoreUtility.AgroZip.CompressioneBase64(1, xmlParChunk.ToString()))
        obFiltroParametri.Append(""" }")

        Dim xChiamataRest As New AgronicaCoreUtility.Http
        Dim DatiSrv As AgronicaCoreUtility.Http.JsonRestResponse =
            xChiamataRest.PostWS_RestSharp_JSON(cfg.url & "/LeggiGIS", cfg.token, obFiltroParametri.ToString)

        Dim risultato As AgronicaCoreMetaSchemaBIZ.AgroAPI_GIS_Output =
            JsonConvert.DeserializeObject(Of AgronicaCoreMetaSchemaBIZ.AgroAPI_GIS_Output)(DatiSrv.Content)

        strXmlDatiRemoti = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, risultato.result)


        If risultato.error_flag <> 0 Then
            Return New RispostaStandard With {
                .Errore = "Errore nel server" & risultato.error_message,
                .RispostaOK = False
                }
        Else
            'scrittura dei dati ottenuti via API
            Dim conteggioNonImportato As Integer
            Dim GestioneDati As New AgronicaCoreGisBIZ.GIS_SrvAgronica
            Dim rvalScrivi As RispostaStandard =
                GIS_SRVAgronica_Scrivi(xmlParChunk, strXmlDatiRemoti, conteggioNonImportato, objParametri_Server)
            Return rvalScrivi
        End If

    End Function

    Private Function GIS_SRVAgronica_AllineaDati_Catasto_RecuperaParametri(ByVal cfg As GIS_SrvAgronica_cfg, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim rval As New RispostaStandard
        Dim letturaCatastoSenzaGis As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim xmlCatasto As String =
            letturaCatastoSenzaGis.CatastoSenzaDatoGis(0, cfg.filtroQueryCatasto, objParametri_Server)

        rval.RispostaStringa = xmlCatasto  'AgronicaCoreUtility.AgroZip.CompressioneBase64(1, xmlCatasto)

        Return rval

    End Function

    Private Function GIS_SRVAgronica_Scrivi(ByVal xDatiLocali As XDocument, ByVal strXmlDatiRemoti As String, ByRef ConteggioNonImportati As Integer, objParametri_server As AgronicaCoreParametri) As RispostaStandard

        Dim ScriviElementiGrafici As New AgronicaCoreGisBIZ.GIS_Entita_W

        strXmlDatiRemoti = strXmlDatiRemoti.Replace("sostituire sul client", objParametri_server.PivaSuperUser)

        Dim xDocRval As XDocument = XDocument.Parse(strXmlDatiRemoti)

        Dim ListaNS As New List(Of String)
        ListaNS.Add("http://www.opengis.net/gml")
        Dim rval1 As String = xmlHelper.RemoveNamespace(xDocRval, ListaNS).ToString.Replace("xmlns=""""", "")

        rval1 = rval1.Replace("<Entita >", "<Entita>")

        Dim reader As XmlReader = XmlReader.Create(New StringReader(rval1))
        Dim tmpDoc As XDocument = XDocument.Load(reader)
        Dim nameTable As XmlNameTable = reader.NameTable
        Dim namespaceManager As XmlNamespaceManager = New XmlNamespaceManager(nameTable)
        namespaceManager.AddNamespace(String.Empty, "http://www.agronica.it/grafica/")

        Dim messaggioErrore As String = ""

        Dim rval As New RispostaStandard
        rval.RispostaOK = True

        'imposta i dati mancanti
        '1. piva super user
        Dim ListaPerSostituzione As IEnumerable = CType(tmpDoc.XPathEvaluate("//DatiEntita/Entita/EntitaGIAS/DatoGias/PivaSuperUser/text()", namespaceManager), IEnumerable)
        For Each text As XText In ListaPerSostituzione
            text.Value = objParametri_server.PivaSuperUser
        Next

        'scrive le entità
        Dim listOFEntita = (
                    From a In tmpDoc.<DatiEntita>.<Entita>
                    Select a).ToList()

        Dim elementiNuovi As Integer = 0 
        Dim elementiModificati As Integer  = 0 

        For Each elemento In listOFEntita


            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            Dim e = elemento.<EntitaGIAS>.<DatoGias>

            Dim e_prov As String = e.<PROV>.Value
            Dim e_com As String = e.<COM>.Value
            Dim e_sezione As String = e.<SEZIONE>.Value
            Dim e_foglio As String = e.<FOGLIO>.Value
            Dim e_numero As String = e.<NUMERO>.Value
            Dim e_subalterno As String = e.<SUBALTERNO>.Value

            Dim xDataModi = (
                From a1 In xDatiLocali.Element("catasto").Elements("particella")
                Where a1.Attribute("prov").Value = e_prov _
                    And a1.Attribute("com").Value = e_com _
                    And a1.Attribute("sezione").Value.ToUpper = e_sezione.ToUpper _
                    And a1.Attribute("foglio").Value = e_foglio _
                    And a1.Attribute("numero").Value = e_numero _
                    And a1.Attribute("subalterno").Value.ToUpper = e_subalterno.ToUpper
            ).FirstOrDefault

            Dim EntitaCod As Integer = 0
            Dim ElementoGraficoCod As Integer = 0

            Dim tipoOperazioneDB1 As enum_TipoOperazioneDB
            If Not xDataModi Is Nothing Then
                If xDataModi.Attribute("data_modifica").Value.Contains("1900") Then
                    tipoOperazioneDB1 = enum_TipoOperazioneDB.Scrittura
                    elementiNuovi += 1
                Else
                    tipoOperazioneDB1 = enum_TipoOperazioneDB.Modifica
                    EntitaCod = xDataModi.Attribute("entita_cod")
                    ElementoGraficoCod = xDataModi.Attribute("elementografico_cod")
                    e.<Entita_Cod>.Value = EntitaCod
                    elemento.<geodata>.@ElementoGrafico_Cod = ElementoGraficoCod
                    elementiModificati += 1
                End If
            Else
                tipoOperazioneDB1 = enum_TipoOperazioneDB.Lettura
            End If

            elemento.SetAttributeValue("TipoOperazioneDB", CInt(tipoOperazioneDB1))

            Try

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Apro la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                objParametri_server)


                Dim idle As Integer
                ScriviElementiGrafici.scrivi(elemento.ToString, idle, objParametri_server)

                ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                'Chiudo la connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Catch ex As Exception

                'Faccio il rollback della transazione
                If Not objParametri_server.objTransazione Is Nothing Then
                    'objParametri.objTransazione.Rollback()
                    'objParametri.objTransazione = Nothing
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

                End If

                messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)



                Dim Messaggio As String = ""
                If messaggioErrore <> "" Then


                    Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                    Messaggio += "" & vbCrLf
                    Messaggio += messaggioErrore
                    Messaggio += "" & vbCrLf
                    Messaggio += "Ritentare il salvataggio dopo la correzione ..."

                End If

                ConteggioNonImportati += 1
                rval.Errore &= Messaggio

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)

            End Try


        Next

        rval.RispostaStringa &= "Catasto: Elementi Nuovi: " & elementiNuovi & ", Elementi modificati: " & elementiModificati & ", Elementi non importati: " & ConteggioNonImportati & vbCrLf

        Return rval

    End Function


End Class
