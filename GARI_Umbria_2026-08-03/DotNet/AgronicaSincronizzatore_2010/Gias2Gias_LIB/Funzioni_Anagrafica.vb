Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq

Partial Public Class Funzioni

#Region "Imprese, Centri Aziendali"

    Public Function LeggiImpreseXML(ByVal Piva_Origine As String,
                                    ByVal Piva_Destinazione As String,
                                    ByVal Piva_Padre_Destinazione As String,
                                    ByVal Forza_Update As Boolean,
                                    ByRef objOpzioni As clsOpzioni,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione
        Dim To_Piva_Padre = Piva_Padre_Destinazione

        Dim objLeggi As New G2GImprese_R
        Dim g2g = objLeggi.Nuovo_Imprese_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva, To_Piva_Padre)
        Return If(objLeggi.Leggi_Imprese_G2G(Forza_Update, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Imprese)(g2g, ""), "")

    End Function

    Public Function LeggiImpreseXML_Reverse(ByVal Piva_Origine As String,
                                    ByVal Piva_Destinazione As String,
                                    ByVal Piva_Padre_Destinazione As String,
                                    ByVal Forza_Update As Boolean,
                                    ByRef objOpzioni As clsOpzioni,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione
        Dim To_Piva_Padre = Piva_Padre_Destinazione

        Dim objLeggi As New G2GImprese_R
        Dim g2g = objLeggi.Nuovo_Imprese_G2G_Reverse(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva, To_Piva_Padre)
        Return If(objLeggi.Leggi_Imprese_G2G_Reverse(Forza_Update, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Imprese_Reverse)(g2g, ""), "")

    End Function


    '##################################################################################################################################
    Public Sub ScriviImpreseXML(ByVal objOpzioni As clsOpzioni,
                                         ByRef Log_Import As StringBuilder,
                                         ByRef Log_Errori As StringBuilder,
                                         ByRef Log_Riepilogo As StringBuilder,
                                         ByVal Piva_Origine As String,
                                         ByVal Piva_Destinazione As String,
                                         ByVal Piva_Padre_Destinazione As String,
                                         Optional ByVal Forza_Update As Boolean = False)

        Const NomeFunzione As String = "ScriviImpreseXML"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente i dati azienda da inserire/modificare/cancellare 
            Dim sXmlImprese = LeggiImpreseXML(Piva_Origine, Piva_Destinazione, Piva_Padre_Destinazione, Forza_Update, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlImprese) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlImprese, "//utente/DatiImprese", "")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaImprese As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiImprese")
                Dim rispostaImprese = xmlRispostaImprese.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaImprese, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    'G2GUtility.Log(Log_Import, "Trasferimento impresa: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento impresa ERRORE: " & MessaggioErrore)
                End If

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '##################################################################################################################################
    Public Sub ScriviImpreseXML_Reverse(ByVal objOpzioni As clsOpzioni,
                                         ByRef Log_Import As StringBuilder,
                                         ByRef Log_Errori As StringBuilder,
                                         ByRef Log_Riepilogo As StringBuilder,
                                         ByVal Piva_Origine As String,
                                         ByVal Piva_Destinazione As String,
                                         ByVal Piva_Padre_Destinazione As String,
                                         Optional ByVal Forza_Update As Boolean = False)

        Const NomeFunzione As String = "ScriviImpreseXML"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente i dati azienda da inserire/modificare/cancellare 
            Dim sXmlImprese = LeggiImpreseXML_Reverse(Piva_Origine, Piva_Destinazione, Piva_Padre_Destinazione, Forza_Update, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlImprese) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlImprese, "//utente/DatiImprese", "")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaImprese As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiImpreseReverse")
                Dim rispostaImprese = xmlRispostaImprese.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaImprese, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    'G2GUtility.Log(Log_Import, "Trasferimento impresa: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento impresa ERRORE: " & MessaggioErrore)
                End If

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '##################################################################################################################################
    Public Sub Elabora_XML_Imprese_Leggi(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByRef xmlImpresa As XmlNode,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Piva_Padre_Destinazione As String,
                            ByVal Piva_SuperUser_Destinazione As String)

        Const NomeFunzione As String = "Elabora_XML_Imprese_Leggi"

        Try

            'Recupero le chiavi originali
            Dim fromImpresaPiva = xmlImpresa.Attributes("piva").Value

            'Aggiungo contatto impresa
            Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim sXML_Contatto = objContatto_R.ContattoImpresa_Leggi(fromImpresaPiva, False, objOpzioni.objParametri_Server_GIAS_ORIGINE)
            If Not String.IsNullOrEmpty(sXML_Contatto) Then
                xmlImpresa.InnerXml = xmlImpresa.InnerXml & sXML_Contatto
            End If

            'Aggiungo gerarchia impresa
            If Not String.IsNullOrEmpty(Piva_Padre_Destinazione) Then
                CType(xmlImpresa, XmlElement).SetAttribute("padre", Piva_Padre_Destinazione)
                'Dim sXML_Gerarchia As String = ""
                'Dim objGerarchia As New AgronicaCoreXML.XML_Anagrafe
                'objGerarchia.XML_GerarchiaImprese(enum_TipoOperazioneDB.Scrittura, Piva_Padre_Destinazione, sXML_Gerarchia)
                'xmlImpresa.InnerXml = xmlImpresa.InnerXml & sXML_Gerarchia
            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '##################################################################################################################################
    Public Sub Elabora_XML_Imprese_Salva(ByVal objOpzioni As clsOpzioni,
                                         ByRef Log_Import As StringBuilder,
                                         ByRef Log_Errori As StringBuilder,
                                         ByRef Log_Riepilogo As StringBuilder,
                                         ByVal Piva_Origine As String,
                                         ByVal Piva_Destinazione As String,
                                         ByVal Piva_Padre_Destinazione As String,
                                         ByVal Piva_SuperUser_Destinazione As String,
                                         ByRef esisteImpresaDestinazione As Boolean)

        Const nomeFunzione = "Elabora_XML_Imprese_Salva"

        Dim OutputPiva As String = ""
        Dim xImpresa As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '===============================================================
            '=================     IMPRESA    ===============================
            '===============================================================

            Dim objImpresa_R As New AgronicaCoreAnagrafeBIZ.Impresa_R

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Impresa As String = objImpresa_R.Impresa_Leggi(Piva_Origine,
                                                                    False,
                                                                    True,
                                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                    TipoG2G:=1)

            'Se ci sono campi da elaborare ...
            If sXML_Impresa <> "" Then

                Dim objImpresa_W As New AgronicaCoreAnagrafeBIZ.Impresa_W

                'Carico il documento
                xImpresa.LoadXml(sXML_Impresa)

                For Each xmlImpresa As XmlNode In xImpresa.SelectNodes("//DatiImprese/Impresa")

                    'Integro xml impresa con dati mancanti
                    Elabora_XML_Imprese_Leggi(objOpzioni,
                                              Log_Import,
                                              Log_Errori,
                                              Log_Riepilogo,
                                              xmlImpresa,
                                              Piva_Origine,
                                              Piva_Destinazione,
                                              Piva_Padre_Destinazione,
                                              Piva_SuperUser_Destinazione)

                    Dim tipoOperazioneDB As enum_TipoOperazioneDB
                    If esisteImpresaDestinazione Then
                        tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                    Else
                        tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                    End If


                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Imprese_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    "<DatiImprese>" & xmlImpresa.OuterXml & "</DatiImprese>",
                                                    Piva_Destinazione,
                                                    Piva_SuperUser_Destinazione,
                                                    tipoOperazioneDB)

                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                        .Timeout = _timeout_ws_Importa,
                        .Url = objOpzioni.wsimportaGiasURl
                    }

                    ' zippo la stringa in gzip per velocizzare il trasferimento
                    sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                    'Salvo la nuova azienda
                    OutputPiva = wsImportazione.Scrivi_ImpresaXmlPrivato(Str_Credenziali_WS, sStringaDaSalvare)

                    If OutputPiva = "" Then
                        Throw New Exception("Errore WS in Scrivi_ImpresaXmlPrivato")
                    End If

                    'Salvo la mappatura delle chiavi nella collezione
                    CentriAziendaliADD(
                        New G2G_Recode_Imprese With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .FROM_Piva = Piva_Origine,
                            .FROM_SaCod = 0,
                            .To_Piva = Piva_Destinazione,
                            .TO_SaCod = 0
                        })
                Next

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '##################################################################################################################################
    Public Sub Elabora_XML_Imprese_Modifica(ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Piva_Padre_Destinazione As String,
                                            ByVal Piva_SuperUser_Destinazione As String)

        Const nomeFunzione = "Elabora_XML_Imprese_Salva"

        Dim OutputPiva As String = ""
        Dim xImpresa As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '===============================================================
            '=================     IMPRESA    ===============================
            '===============================================================

            Dim objImpresa_R As New AgronicaCoreAnagrafeBIZ.Impresa_R

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Impresa As String = objImpresa_R.Impresa_Leggi(Piva_Origine,
                                                                    False,
                                                                    True,
                                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                    TipoG2G:=2)

            'Se ci sono campi da elaborare ...
            If sXML_Impresa <> "" Then

                Dim objImpresa_W As New AgronicaCoreAnagrafeBIZ.Impresa_W

                'Carico il documento
                xImpresa.LoadXml(sXML_Impresa)

                For Each xmlImpresa As XmlNode In xImpresa.SelectNodes("//DatiImprese/Impresa")

                    'Integro xml impresa con dati mancanti
                    Elabora_XML_Imprese_Leggi(objOpzioni,
                                              Log_Import,
                                              Log_Errori,
                                              Log_Riepilogo,
                                              xmlImpresa,
                                              Piva_Origine,
                                              Piva_Destinazione,
                                              Piva_Padre_Destinazione,
                                              Piva_SuperUser_Destinazione)

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Imprese_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    "<DatiImprese>" & xmlImpresa.OuterXml & "</DatiImprese>",
                                                    Piva_Destinazione,
                                                    Piva_SuperUser_Destinazione,
                                                    enum_TipoOperazioneDB.Modifica)

                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                        .Timeout = _timeout_ws_Importa,
                        .Url = objOpzioni.wsimportaGiasURl
                    }

                    ' zippo la stringa in gzip per velocizzare il trasferimento
                    sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                    'Modifica azienda
                    OutputPiva = wsImportazione.Scrivi_ImpresaXmlPrivato(Str_Credenziali_WS, sStringaDaSalvare)

                    If OutputPiva = "" Then
                        Throw New Exception("Errore WS in Scrivi_ImpresaXmlPrivato")
                    End If

                    Dim objCentro As G2G_Recode_Imprese = recode_return_Centro(Piva_Origine, 0,
                                                                               objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    'Aggiorno la datainvio nel recode azienda
                    CentriAziendaliEDIT(objCentro)

                Next

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Function LeggiCentriAziendaliXML(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        Dim objLeggi As New G2GCentriAziendali_R
        Dim g2g = objLeggi.Nuovo_CentriAziendali_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva, To_Progressivo)
        Return If(objLeggi.Leggi_CentriAziendali_G2G(Sa_Cod_Origine, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_CentriAziendali)(g2g, ""), "")

    End Function

    Public Function LeggiCentriAziendaliXMLReverse(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        Dim objLeggi As New G2GCentriAziendali_R
        Dim g2g = objLeggi.Nuovo_CentriAziendali_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva, To_Progressivo)
        Return If(objLeggi.Leggi_CentriAziendali_G2GReverse(Sa_Cod_Origine, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_CentriAziendali_Reverse)(g2g, ""), "")

    End Function

    '##################################################################################################################################
    Public Function Elabora_XML_CentriAziendali_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Sa_Cod_Origine As Integer,
                            ByVal Piva_Destinazione As String
                    ) As Boolean

        Const NomeFunzione As String = "Elabora_XML_CentriAziendali_Salva_WS"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente i contatti azienda da inserire/modificare/cancellare 
            Dim sXmlCentriAziendali = LeggiCentriAziendaliXML(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlCentriAziendali) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)


                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlCentriAziendali, "//utente/DatiImprese/Impresa", "DatiCentriAziendali")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParcoMacchine As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiCentriAziendali")
                Dim rispostaParcoMacchine = xmlRispostaParcoMacchine.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParcoMacchine, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    'G2GUtility.Log(Log_Import, "Trasferimento centri aziendali: " & risposta.LogRecode)
                    Return (risposta.G2GRecodeImpreseCentriToInsert.Count + risposta.G2GRecodeImpreseCentriToUpdate.Count) > 0
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento centri aziendali ERRORE: " & MessaggioErrore)
                    Return False
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento centri aziendali: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Function

    Public Function Elabora_XML_CentriAziendali_SalvaReverse(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Sa_Cod_Origine As Integer,
                            ByVal Piva_Destinazione As String
                    ) As Boolean

        Const NomeFunzione As String = "Elabora_XML_CentriAziendali_SalvaReverse"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente i contatti azienda da inserire/modificare/cancellare 
            Dim sXmlCentriAziendali = LeggiCentriAziendaliXMLReverse(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlCentriAziendali) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)


                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlCentriAziendali, "//utente/DatiImprese/Impresa", "DatiCentriAziendali")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParcoMacchine As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiCentriAziendali")
                Dim rispostaParcoMacchine = xmlRispostaParcoMacchine.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParcoMacchine, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    'G2GUtility.Log(Log_Import, "Trasferimento centri aziendali: " & risposta.LogRecode)
                    Return (risposta.G2GRecodeImpreseCentriToInsert.Count + risposta.G2GRecodeImpreseCentriToUpdate.Count) > 0
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento centri aziendali ERRORE: " & MessaggioErrore)
                    Return False
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento centri aziendali: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Function

    '##################################################################################################################################
    Public Function Elabora_XML_CentriAziendali_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Sa_cod_Origine As Integer,
                            ByVal Piva_Destinazione As String,
                            ByRef Sa_cod_Destinazione As Integer
                    ) As Boolean

        Const NomeFunzione As String = "Elabora_XML_CentriAziendali_Salva"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim xCentro As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '===============================================================
            '=================     CENTRI    ===============================
            '===============================================================

            Dim objCentro_R As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            Dim conteggioCentriNuovi As Integer = 0

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Centro As String =
                objCentro_R.CentroAziendale_Leggi(
                                    Piva_Origine,
                                    Sa_cod_Origine,
                                    False,
                                    True,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                    TipoG2G:=1)

            'Se ci sono campi da elaborare ...
            If sXML_Centro <> "" Then

                Dim objCentri_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

                'Carico il documento
                xCentro.LoadXml(sXML_Centro)

                For Each xmlCentro As XmlNode In xCentro.SelectNodes("//DatiCentriAziendali/CentroAziendale")

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_CentriAziendali_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                    objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                    objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                                    "<DatiCentriAziendali>" & xmlCentro.OuterXml & "</DatiCentriAziendali>",
                                                    Piva_Destinazione,
                                                    0,
                                                    enum_TipoOperazioneDB.Scrittura)

                    Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                    wsimportazione.Timeout = _timeout_ws_Importa
                    wsimportazione.Url = objOpzioni.wsimportaGiasURl

                    ' zippo la stringa in gzip per velocizzare il trasferimento
                    sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                    'Salvo il nuovo centro
                    OutputSaCod = wsimportazione.Scrivi_CentroAziendaleXmlPrivato(Str_Credenziali_WS, sStringaDaSalvare)

                    If OutputSaCod = -1 Then
                        Throw New Exception("Errore WS in Scrivi_CentroAziendaleXmlPrivato")
                    End If

                    Sa_cod_Destinazione = OutputSaCod

                    'Salvo la mappatura delle chiavi nella collezione
                    CentriAziendaliADD(
                        New G2G_Recode_Imprese With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .FROM_Piva = Piva_Origine,
                            .FROM_SaCod = Sa_cod_Origine,
                            .To_Piva = Piva_Destinazione,
                            .TO_SaCod = Sa_cod_Destinazione
                        })

                    conteggioCentriNuovi += 1

                Next

            End If

            Return conteggioCentriNuovi > 0

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Function

    '##################################################################################################################################
    Public Function Elabora_XML_CentriAziendali_Modifica(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Sa_cod_Origine As Integer,
                            ByVal Piva_Destinazione As String,
                            ByVal Sa_cod_Destinazione As Integer
                    ) As Boolean

        Const NomeFunzione As String = "Elabora_XML_CentriAziendali_Modifica"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim xCentro As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            '===============================================================
            '=================     CENTRI    ===============================
            '===============================================================

            Dim objCentro_R As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            Dim conteggioCentriModificati As Integer = 0

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Centro As String =
                objCentro_R.CentroAziendale_Leggi(
                                    Piva_Origine,
                                    Sa_cod_Origine,
                                    False,
                                    True,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                    TipoG2G:=2)

            'Se ci sono campi da elaborare ...
            If sXML_Centro <> "" Then

                Dim objCentri_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

                'Carico il documento
                xCentro.LoadXml(sXML_Centro)

                For Each xmlCentro As XmlNode In xCentro.SelectNodes("//DatiCentriAziendali/CentroAziendale")

                    Dim objCentro As G2G_Recode_Imprese = recode_return_Centro(Piva_Origine, Sa_cod_Origine,
                                                                               objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Sa_cod_Destinazione = objCentro.TO_SaCod

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_CentriAziendali_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                    objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                    objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                                    "<DatiCentriAziendali>" & xmlCentro.OuterXml & "</DatiCentriAziendali>",
                                                    Piva_Destinazione,
                                                    Sa_cod_Destinazione,
                                                    enum_TipoOperazioneDB.Modifica
                                                    )


                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                        .Timeout = _timeout_ws_Importa,
                        .Url = objOpzioni.wsimportaGiasURl
                    }

                    ' zippo la stringa in gzip per velocizzare il trasferimento
                    sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                    'Salvo il centro aziendale
                    OutputSaCod = wsImportazione.Scrivi_CentroAziendaleXmlPrivato(Str_Credenziali_WS, sStringaDaSalvare)

                    If OutputSaCod = -1 Then
                        Throw New Exception("Errore WS in Scrivi_CentroAziendaleXmlPrivato")
                    End If

                    Sa_cod_Destinazione = OutputSaCod

                    'Aggiorno la data_modifica del centro aziendale
                    CentriAziendaliEDIT(objCentro)

                    conteggioCentriModificati += 1

                Next

            End If

            Return conteggioCentriModificati > 0

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Function

    '#####################################################################################################################################################
    Private Function ElaboraXML_Imprese_SistemaXML(
                                                 ByVal objOpzioni As clsOpzioni,
                                                 ByRef Log_Import As StringBuilder,
                                                 ByRef Log_Errori As StringBuilder,
                                                 ByRef Log_Riepilogo As StringBuilder,
                                                 ByRef Str_XML_Impresa As String,
                                                 ByVal Piva_Destinazione As String,
                                                 ByVal Piva_SuperUser_Destinazione As String,
                                                 ByVal tipoOperazioneDB As enum_TipoOperazioneDB) As String

        Const NomeFunzione As String = "Elabora_XMLImprese_SistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Impresa)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'Tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", tipoOperazioneDB)

            Next

            'imposto piva destinazione
            XMLs_Nodi = XmlDocCont.SelectNodes("//Impresa")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                'XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                'XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            'azzeramento cod_indirizzo
            XMLs_Nodi = XmlDocCont.SelectNodes("//Indirizzo")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                Else
                    XML_Nodo.SetAttribute("cod_indirizzo", 0)
                    XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                    XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                    XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                End If
            Next

            'imposto piva super user su contatto azienda
            XMLs_Nodi = XmlDocCont.SelectNodes("//DatiImprese/Impresa/DatiContatti/Contatto")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("piva", Piva_SuperUser_Destinazione)
                XML_Nodo.SetAttribute("cod_contatto", Piva_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//DatiImprese/Impresa/DatiContatti/Contatto/RapCon")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                Else
                    XML_Nodo.SetAttribute("cod_risum", 0)
                    XML_Nodo.SetAttribute("cod_contatto", Piva_Destinazione)
                    XML_Nodo.SetAttribute("piva", Piva_SuperUser_Destinazione)
                    XML_Nodo.SetAttribute("piva_superuser", Piva_SuperUser_Destinazione)
                End If
            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//DatiImprese/Impresa/DatiContatti/Contatto/Rubrica")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                Else
                    XML_Nodo.SetAttribute("cod_rubrica", 0)
                    XML_Nodo.SetAttribute("piva", Piva_SuperUser_Destinazione)
                    XML_Nodo.SetAttribute("piva_superuser", Piva_SuperUser_Destinazione)
                End If
            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

    '#####################################################################################################################################################
    Private Function ElaboraXML_CentriAziendali_SistemaXML(
                                                 ByVal objOpzioni As clsOpzioni,
                                                 ByRef Log_Import As StringBuilder,
                                                 ByRef Log_Errori As StringBuilder,
                                                 ByRef Log_Riepilogo As StringBuilder,
                                                 ByRef objParametri_Server_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Server_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef Str_XML_Campo As String,
                                                 ByVal Piva_Destinazione As String,
                                                 ByVal saCod_Destinazione As String,
                                                 ByVal tipoOperazioneDB As enum_TipoOperazioneDB) As String

        Const NomeFunzione As String = "ElaboraXML_CentriAziendali_SistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Campo)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'Tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", tipoOperazioneDB)

            Next

            'Azzeramento "campo_cod"
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@sa_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next


            'azzeramento cod_indirizzo
            XMLs_Nodi = XmlDocCont.SelectNodes("//Indirizzo")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                Else
                    XML_Nodo.SetAttribute("cod_indirizzo", 0)
                    XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                    XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                End If
            Next

            'azzeramento cod_rubrica
            XMLs_Nodi = XmlDocCont.SelectNodes("//Rubrica")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Lettura)
                Else
                    XML_Nodo.SetAttribute("cod_rubrica", 0)
                    XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                    XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                End If
            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

    Public Function recode_return_Centro(ByVal oldPiva As String,
                                         ByVal oldSa_Cod As Integer,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As G2G_Recode_Imprese

        'TODO: Visto che chi riempe questa tabella usa il vecchio meccanismo,
        ' i codici vanno letti sempre direttamente sulla lista perché è la più aggiornata.
        ' Quando si farà la nuova gestione si passerà ad usare EF
        Return (From x In _CentriAziendali
                Where x.FROM_Piva = oldPiva AndAlso
                      x.FROM_SaCod = oldSa_Cod AndAlso
                      x.To_PivaSuperUser = piva_SuperUser_destinazione
                Select x).FirstOrDefault

        'Return If(_efG2G Is Nothing, 
        '          (From x In _CentriAziendali
        '             Where x.FROM_Piva = oldPiva AndAlso
        '                   x.FROM_SaCod = oldSa_Cod AndAlso
        '                   x.To_PivaSuperUser = piva_SuperUser_DESTINAZIONE
        '             Select x).FirstOrDefault, 
        '          (From x In _efG2G.G2G_Recode_Imprese
        '             Where x.FROM_Piva = oldPiva AndAlso
        '                   x.FROM_SaCod = oldSa_Cod AndAlso
        '                   x.To_PivaSuperUser = piva_SuperUser_DESTINAZIONE
        '             Select x).FirstOrDefault)

    End Function



    Public Sub Elabora_XML_Imprese_Salva_Reverse(ByVal objOpzioni As clsOpzioni,
                                         ByRef Log_Import As StringBuilder,
                                         ByRef Log_Errori As StringBuilder,
                                         ByRef Log_Riepilogo As StringBuilder,
                                         ByVal Piva_Origine As String,
                                         ByVal G2G_Gestore As Integer,
                                         ByVal Piva_SuperUser_Destinazione As String,
                                         ByRef Messaggio_di_Ritorno As String)

        Const nomeFunzione = "Elabora_XML_Imprese_Salva_Reverse"

        Dim OutputPiva As String = ""
        Dim xImpresa As New XmlDocument
        Dim outputRecode As String = ""
        Try

            Dim leggiRequest As New AgronicaCoreG2GLocalDal.G2G_Recodes_R

            Dim oG2G_RecodeRequest As New G2G_Recodes_Request

            Dim sXmlRequest As String = AgronicaCoreUtility.XMLUtility.SerializzaOggetto(Of G2G_Recodes_Request)(oG2G_RecodeRequest, "")

            Dim mainDoc As String = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, "", "", objOpzioni)

            Dim Jobj As New JObject
            Jobj("Piva") = CStr(Piva_Origine)
            Jobj("G2G_Gestore") = CStr(G2G_Gestore)

            mainDoc = AgronicaCoreUtility.XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, "<datiImpresa>" & Jobj.ToString & "</datiImpresa>", "//utente", "flagimporta_g2g_reverse")

            Dim sStringaDaSalvare As String = mainDoc

            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Funzioni.Get_Str_Credenziali_WS(objOpzioni)

            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)
            outputRecode = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, sStringaDaSalvare)

            'riporto della risposta
            Dim xDocResponse As XDocument = XDocument.Parse(outputRecode)
            Dim nsAgronicaCoreModello As XNamespace = "http://schemas.datacontract.org/2004/07/AgronicaCoreModello"
            Dim elemResponse As XElement = xDocResponse.Element("Risposta").Element("Risposta_DatiImpresaReverse")
            Dim strResponse As String = elemResponse.Value
            Messaggio_di_Ritorno &= strResponse

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf & "outputRecode:" & outputRecode
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Messaggio_di_Ritorno &= msg
            Throw New Exception(msg)
        End Try

    End Sub

#End Region

End Class