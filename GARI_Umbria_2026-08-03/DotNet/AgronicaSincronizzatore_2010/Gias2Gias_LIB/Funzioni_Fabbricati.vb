Imports System.Text
Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreModello
Imports AgronicaCoreUtility

Partial Public Class Funzioni


#Region "Fabbricati"

    Public Function Elabora_XML_Fabbricati_Leggi(ByVal objOpzioni As clsOpzioni,
                                                 ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Fabbricato_Cod As Integer
                                                 ) As String

        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
            .Url = objOpzioni.wsimportaGiasURl,
            .Timeout = _timeout_ws_Importa
        }

        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

        Dim dati = wsImportazione.Leggi_FabbricatiXmlPrivato(Str_Credenziali_WS, Piva, Sa_Cod, Fabbricato_Cod)

        Return If(dati <> "", AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, dati), "")

    End Function

    Public Function LeggiFabbricatiXML(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_Cod_Destinazione As Integer,
                                            ByRef objOpzioniImportImpresa As clsImpresa,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        Dim filtroFabbricati = objOpzioniImportImpresa.Fabbricati

        Dim objLeggi As New G2GFabbricati_R
        Dim g2g = objLeggi.Nuovo_Fabbricati_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Return If(objLeggi.Leggi_Fabbricati_G2G(Sa_Cod_Origine, filtroFabbricati, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Fabbricati)(g2g, ""), "")

    End Function

    Public Function LeggiFabbricatiXMLReverse(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_Cod_Destinazione As Integer,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE

        Dim objLeggi As New G2GFabbricati_R
        Dim g2g = objLeggi.Nuovo_Fabbricati_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Return If(objLeggi.Leggi_Fabbricati_G2GReverse(Sa_Cod_Origine, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Fabbricati_Reverse)(g2g, ""), "")

    End Function

    Public Sub ScriviFabbricatiXML(ByVal objOpzioniImportImpresa As clsImpresa,
                                   ByVal objOpzioni As clsOpzioni,
                                   ByRef Log_Import As StringBuilder,
                                   ByRef Log_Errori As StringBuilder,
                                   ByRef Log_Riepilogo As StringBuilder,
                                   ByVal Piva_Origine As String,
                                   ByVal Sa_Cod_Origine As Integer,
                                   ByVal Piva_Destinazione As String,
                                   ByVal Sa_Cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviFabbricatiXML"
        Dim MessaggioErrore As String = ""

        Try


            ' leggo struttura dati serializzata su origine contenente i fabbricati centro da inserire/modificare/cancellare 
            Dim sXmlFabbricati = LeggiFabbricatiXML(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, Sa_Cod_Destinazione, objOpzioniImportImpresa, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlFabbricati) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlFabbricati, "//utente/DatiImprese/Impresa", "DatiFabbricati")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaFabbricati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiFabbricati")
                Dim rispostaFabbricati = xmlRispostaFabbricati.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaFabbricati, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento fabbricati: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento fabbricati ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento fabbricati: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub ScriviFabbricatiXMLReverse(ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_Cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviFabbricatiXMLReverse"
        Dim MessaggioErrore As String = ""

        Try


            ' leggo struttura dati serializzata su origine contenente i fabbricati centro da inserire/modificare/cancellare 
            Dim sXmlFabbricati = LeggiFabbricatiXMLReverse(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, Sa_Cod_Destinazione, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlFabbricati) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlFabbricati, "//utente/DatiImprese/Impresa", "DatiFabbricati")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaFabbricati As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiFabbricati")
                Dim rispostaFabbricati = xmlRispostaFabbricati.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaFabbricati, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento fabbricati: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento fabbricati ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento fabbricati: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub


    Public Sub Elabora_XML_Fabbricati_Salva(ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Sa_cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Fabbricati_Salva"

        Dim FabbricatoR As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
        Dim conteggioFabbricatiNuovi As Integer = 0

        Try

            Dim sXML_Fabbricato As String = FabbricatoR.Fabbricato_Leggi(Piva_Origine,
                                                                         Sa_cod_Origine,
                                                                         0,
                                                                         False,
                                                                         objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                         TipoG2G:=1)

            If sXML_Fabbricato <> "" Then

                Dim xDocApp As New XmlDocument
                xDocApp.LoadXml(sXML_Fabbricato)
                Dim xSingoloFabbricato As XmlNode

                For Each xSingoloFabbricato In xDocApp.SelectNodes("//DatiFabbricati/Fabbricato")

                    Dim outPiva As String = ""
                    Dim outSaCod As Integer = 0
                    Dim outFabbricatoCod As Integer = -1

                    Dim inputFabbricato As Integer = CInt(xSingoloFabbricato.Attributes("fabbricato_cod").Value)

                    Dim ScriviFabbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_W

                    Dim sStringaDaSalvare As String =
                        "<DatiFabbricati>" &
                            ElaboraXML_Fabbricato_SistemaXML(
                                    objOpzioni,
                                    Log_Import,
                                    Log_Errori,
                                    Log_Riepilogo,
                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                    objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                    xSingoloFabbricato.OuterXml,
                                    Piva_Destinazione,
                                    Sa_cod_Destinazione,
                                    0,
                                    enum_TipoOperazioneDB.Scrittura
                                ) _
                        & "</DatiFabbricati>"

                    If objOpzioni.isGias2Gias_local Then
                        ScriviFabbricato.Fabbricato_Scrivi(sStringaDaSalvare,
                                                           outPiva,
                                                           outSaCod,
                                                           outFabbricatoCod,
                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    Else
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Url = objOpzioni.wsimportaGiasURl,
                            .Timeout = _timeout_ws_Importa
                        }
                        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        outFabbricatoCod = wsImportazione.Scrivi_FabbricatiXmlPrivato(Str_Credenziali_WS,
                                                                                      sStringaDaSalvare)

                        If outFabbricatoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_FabbricatiXmlPrivato")
                        End If

                    End If

                    '  Vanni, 21/05/2014 11:53:39: 
                    FabbricatiADD(
                        New G2G_Recode_Fabbricati With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .FromPiva = Piva_Origine,
                            .FromSa_cod = Sa_cod_Origine,
                            .From_FabbricatoCod = inputFabbricato,
                            .ToPiva = Piva_Destinazione,
                            .ToSa_cod = Sa_cod_Destinazione,
                            .To_FabbricatoCod = outFabbricatoCod
                        })

                    conteggioFabbricatiNuovi += 1

                Next

            End If

            ' log fabbricati
            If conteggioFabbricatiNuovi > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioFabbricatiNuovi & " fabbricati nuovi" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha fabbricati nuovi" & vbCrLf)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Fabbricati_Modifica(ByVal objOpzioni As clsOpzioni,
                                               ByRef Log_Import As StringBuilder,
                                               ByRef Log_Errori As StringBuilder,
                                               ByRef Log_Riepilogo As StringBuilder,
                                               ByVal Piva_Origine As String,
                                               ByVal Sa_cod_Origine As Integer,
                                               ByVal Piva_Destinazione As String,
                                               ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Fabbricati_Modifica"

        Dim FabbricatoR As New AgronicaCoreAnagrafeBIZ.Fabbricato_R
        Dim conteggioFabbricatiModificati As Integer = 0

        Try

            Dim sXML_Fabbricato As String = FabbricatoR.Fabbricato_Leggi(Piva_Origine,
                                                                         Sa_cod_Origine,
                                                                         0,
                                                                         False,
                                                                         objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                         TipoG2G:=2)

            If sXML_Fabbricato <> "" Then

                Dim xDocApp As New XmlDocument
                xDocApp.LoadXml(sXML_Fabbricato)
                Dim xSingoloFabbricato As XmlNode

                For Each xSingoloFabbricato In xDocApp.SelectNodes("//DatiFabbricati/Fabbricato")

                    Dim outPiva As String = ""
                    Dim outSaCod As Integer = 0
                    Dim outFabbricatoCod As Integer = -1

                    Dim fromFabbricatoCod As Integer = CInt(xSingoloFabbricato.Attributes("fabbricato_cod").Value)

                    Dim objFabbricato As G2G_Recode_Fabbricati = recode_return_Fabbricato(Piva_Origine, Sa_cod_Origine,
                                                                                          fromFabbricatoCod,
                                                                                          objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim fabbricato_cod_Destinazione As Integer = objFabbricato.To_FabbricatoCod

                    Dim ScriviFabbricato As New AgronicaCoreAnagrafeBIZ.Fabbricato_W

                    Dim sStringaDaSalvare As String =
                        "<DatiFabbricati>" &
                            ElaboraXML_Fabbricato_SistemaXML(
                                    objOpzioni,
                                    Log_Import,
                                    Log_Errori,
                                    Log_Riepilogo,
                                    objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                    objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                    objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                    xSingoloFabbricato.OuterXml,
                                    Piva_Destinazione,
                                    Sa_cod_Destinazione,
                                    fabbricato_cod_Destinazione,
                                    enum_TipoOperazioneDB.Modifica
                                ) _
                        & "</DatiFabbricati>"

                    If objOpzioni.isGias2Gias_local Then
                        ScriviFabbricato.Fabbricato_Scrivi(sStringaDaSalvare,
                                                           outPiva,
                                                           outSaCod,
                                                           outFabbricatoCod,
                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    Else
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Url = objOpzioni.wsimportaGiasURl,
                            .Timeout = _timeout_ws_Importa
                        }
                        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        outFabbricatoCod = wsImportazione.Scrivi_FabbricatiXmlPrivato(Str_Credenziali_WS,
                                                                                      sStringaDaSalvare)

                        If outFabbricatoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_FabbricatiXmlPrivato")
                        End If
                    End If

                    'Aggiorno la data_modifica del fabbricato
                    FabbricatiEDIT(objFabbricato)

                    conteggioFabbricatiModificati += 1

                Next

            End If

            ' log fabbricati
            If conteggioFabbricatiModificati > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioFabbricatiModificati & " fabbricati modificati" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha fabbricati modificati" & vbCrLf)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Private Function ElaboraXML_Fabbricato_SistemaXML(ByVal objOpzioni As clsOpzioni,
                                                      ByRef Log_Import As StringBuilder,
                                                      ByRef Log_Errori As StringBuilder,
                                                      ByRef Log_Riepilogo As StringBuilder,
                                                      ByRef objParametri_Server_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Server_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef Str_XML_Fabbricato As String,
                                                      ByVal Piva_Destinazione As String,
                                                      ByVal saCod_Destinazione As String,
                                                      ByVal fabbricatoCod_Destinazione As String,
                                                      ByVal tipoOperazioneDB As enum_TipoOperazioneDB
                                                      ) As String

        Const nomeFunzione = "ElaboraXML_Fabbricato_SistemaXML"

        Dim xmlDocCont As New XmlDocument
        xmlDocCont.LoadXml(Str_XML_Fabbricato)

        Dim Str_XML_Output As String = ""
        Dim indirizzoCod_Destinazione As Integer = 0

        Try

            ' ricavo codice indirizzo del fabbricato destinazione
            If tipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                Dim Str_XML_Fabbricati = Elabora_XML_Fabbricati_Leggi(objOpzioni, Piva_Destinazione, saCod_Destinazione, fabbricatoCod_Destinazione)
                If Str_XML_Fabbricati <> "" Then
                    Dim XML_Fabbricati As New XmlDocument
                    XML_Fabbricati.LoadXml(Str_XML_Fabbricati)
                    For Each XML_Fabbricato In XML_Fabbricati.SelectNodes("//DatiFabbricati/Fabbricato")
                        indirizzoCod_Destinazione = CInt(XML_Fabbricato.Attributes("indirizzo_cod").Value)
                    Next
                End If
            End If

            'imposto tipo operazione
            For Each XML_Elem As XmlElement In xmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
                XML_Elem.SetAttribute("TipoOperazioneDB", tipoOperazioneDB)
                XML_Elem.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Elem.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            'imposto piva e sa_cod destinazione
            For Each XML_Elem As XmlElement In xmlDocCont.SelectNodes("//*[@sa_cod]")
                XML_Elem.SetAttribute("piva", Piva_Destinazione)
                XML_Elem.SetAttribute("sa_cod", saCod_Destinazione)
            Next

            'imposto fabbricato_cod destinazione (0 se non esiste)
            For Each XML_Elem As XmlElement In xmlDocCont.SelectNodes("//*[@fabbricato_cod]")
                XML_Elem.SetAttribute("fabbricato_cod", fabbricatoCod_Destinazione)
                XML_Elem.SetAttribute("indirizzo_cod", indirizzoCod_Destinazione)
            Next

            'azzero il cod_indirizzo che verrà assegnato dal core
            For Each XML_Elem As XmlElement In xmlDocCont.SelectNodes("//Fabbricato/Indirizzo")
                XML_Elem.SetAttribute("cod_indirizzo", indirizzoCod_Destinazione)
                'XML_Elem.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                'XML_Elem.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            Str_XML_Output = xmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

    Public Function recode_return_Fabbricato(ByVal oldPiva As String,
                                             ByVal oldsa_cod As Integer,
                                             ByVal oldFab_Cod As Integer,
                                             ByVal piva_SuperUser_destinazione As String
                                             ) As G2G_Recode_Fabbricati
        If NuovaLogicaRecode Then
            Return (From x In _efG2G.G2G_Recode_Fabbricati
                    Where x.From_FabbricatoCod = oldFab_Cod AndAlso
                          x.FromPiva = oldPiva AndAlso
                          x.FromSa_cod = oldsa_cod AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _Fabbricati
                    Where x.From_FabbricatoCod = oldFab_Cod AndAlso
                          x.FromPiva = oldPiva AndAlso
                          x.FromSa_cod = oldsa_cod AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

    Public Function recode_return_FabbricatoReverse(ByVal oldPiva As String,
                                             ByVal oldsa_cod As Integer,
                                             ByVal oldFab_Cod As Integer,
                                             ByVal piva_SuperUser_destinazione As String
                                             ) As G2G_Recode_Fabbricati
        If NuovaLogicaRecode Then
            Return (From x In _efG2G.G2G_Recode_Fabbricati
                    Where x.To_FabbricatoCod = oldFab_Cod AndAlso
                          x.FromPiva = oldPiva AndAlso
                          x.ToSa_cod = oldsa_cod AndAlso
                          x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _Fabbricati
                    Where x.To_FabbricatoCod = oldFab_Cod AndAlso
                          x.FromPiva = oldPiva AndAlso
                          x.ToSa_cod = oldsa_cod AndAlso
                          x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

#End Region

End Class
