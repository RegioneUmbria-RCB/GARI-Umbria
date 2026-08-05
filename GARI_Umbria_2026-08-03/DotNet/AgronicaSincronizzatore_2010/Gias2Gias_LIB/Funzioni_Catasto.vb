Imports System.Xml
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreModello

Partial Public Class Funzioni

#Region "Particelle Catastali"

    Public Function LeggiParticelleXML(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_Cod_Destinazione As Integer,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE

        Dim objLeggi As New G2GParticelle_R
        Dim g2g = objLeggi.Nuovo_Particelle_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Return If(objLeggi.Leggi_Particelle_G2G(Sa_Cod_Origine, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Particelle)(g2g, ""), "")

    End Function

    Public Function LeggiParticelleXMLReverse(ByVal Piva_Origine As String,
                                            ByVal Sa_Cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_Cod_Destinazione As Integer,
                                            ByRef objOpzioni As clsOpzioni,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE

        Dim objLeggi As New G2GParticelle_R
        Dim g2g = objLeggi.Nuovo_Particelle_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Return If(objLeggi.Leggi_Particelle_G2G_Reverse(Sa_Cod_Origine, g2g, objParametri), XMLUtility.SerializzaOggetto(Of G2G_Particelle_Reverse)(g2g, ""), "")

    End Function

    Public Sub ScriviParticelleXML(ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                   ByVal objOpzioni As clsOpzioni,
                                   ByRef Log_Import As StringBuilder,
                                   ByRef Log_Errori As StringBuilder,
                                   ByRef Log_Riepilogo As StringBuilder,
                                   ByVal Piva_Origine As String,
                                   ByVal Sa_Cod_Origine As Integer,
                                   ByVal Piva_Destinazione As String,
                                   ByVal Sa_Cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviParticelleXML"
        Dim MessaggioErrore As String = ""

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale

            ' leggo struttura dati serializzata su origine contenente i fabbricati centro da inserire/modificare/cancellare 
            Dim sXmlParticelle = LeggiParticelleXML(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, Sa_Cod_Destinazione, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If Not String.IsNullOrEmpty(sXmlParticelle) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParticelle, "//utente/DatiImprese/Impresa", "DatiParticelle")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParticelle As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiParticelle")
                Dim rispostaParticelle = xmlRispostaParticelle.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParticelle, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento particelle: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento particelle ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento particelle: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub ScriviParticelleXMLReverse(ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                   ByVal objOpzioni As clsOpzioni,
                                   ByRef Log_Import As StringBuilder,
                                   ByRef Log_Errori As StringBuilder,
                                   ByRef Log_Riepilogo As StringBuilder,
                                   ByVal Piva_Origine As String,
                                   ByVal Sa_Cod_Origine As Integer,
                                   ByVal Piva_Destinazione As String,
                                   ByVal Sa_Cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviParticelleXMLReverse"
        Dim MessaggioErrore As String = ""

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale

            ' leggo struttura dati serializzata su origine contenente i fabbricati centro da inserire/modificare/cancellare 
            Dim sXmlParticelle = LeggiParticelleXMLReverse(Piva_Origine, Sa_Cod_Origine, Piva_Destinazione, Sa_Cod_Destinazione, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If Not String.IsNullOrEmpty(sXmlParticelle) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParticelle, "//utente/DatiImprese/Impresa", "DatiParticelle")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParticelle As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiParticelle")
                Dim rispostaParticelle = xmlRispostaParticelle.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParticelle, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento particelle: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento particelle ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento particelle: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Particelle_Salva(ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                            ByVal objOpzioni As clsOpzioni,
                                            ByRef Log_Import As StringBuilder,
                                            ByRef Log_Errori As StringBuilder,
                                            ByRef Log_Riepilogo As StringBuilder,
                                            ByVal Piva_Origine As String,
                                            ByVal Sa_cod_Origine As Integer,
                                            ByVal Piva_Destinazione As String,
                                            ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Particelle_Salva"

        Dim ParticelleR As New AgronicaCoreAnagrafeBIZ.Particella_R
        Dim conteggioParticelle As Integer = 0
        Dim xRisp As Boolean = False

        'memorizza il dato precedente per successivo recupero
        Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

        Try
            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale


            Dim sXML_Particelle As String = ParticelleR.ImpresexParticelle_Leggi(Piva_Origine,
                                                                                 Sa_cod_Origine,
                                                                                 0,
                                                                                 "",
                                                                                 "",
                                                                                 "",
                                                                                 0,
                                                                                 0,
                                                                                 "",
                                                                                 False,
                                                                                 AGRODATAINIZIO,
                                                                                 AGRODATAFINE,
                                                                                 objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                                 TipoG2G:=1)

            If sXML_Particelle <> "" Then

                sXML_Particelle = ElaboraXML_Particelle_SistemaXML(objOpzioni,
                                                                   Log_Import,
                                                                   Log_Errori,
                                                                   Log_Riepilogo,
                                                                   objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                   objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                                                   objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                                   objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                                                   sXML_Particelle,
                                                                   Piva_Destinazione,
                                                                   Sa_cod_Destinazione,
                                                                   conteggioParticelle)

                '  Marco Grilli, 19/06/2014 17:36:18: gestisco se il server è remoto
                If objOpzioni.isGias2Gias_local Then

                    Dim scriviParticella As New AgronicaCoreAnagrafeBIZ.Particella_W
                    xRisp = scriviParticella.Particella_Scrivi(sXML_Particelle, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                Else
                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                    Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                    '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                    sXML_Particelle = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sXML_Particelle)

                    xRisp = wsImportazione.Scrivi_ParticelleXmlPrivato(Str_Credenziali_WS, sXML_Particelle)

                    If xRisp = False Then
                        Throw New Exception("Errore WS in Scrivi_ParticelleXmlPrivato")
                    End If
                End If

            Else
                Log_Import.Append("L'impresa " & Piva_Origine & " non ha particelle catastali" + vbCrLf)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        Finally
            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)
        End Try

    End Sub

    Public Sub Elabora_XML_Particelle_Modifica(ByVal objOpzioniImportImpresa As AgronicaCoreModello.clsImpresa,
                                               ByVal objOpzioni As clsOpzioni,
                                               ByRef Log_Import As StringBuilder,
                                               ByRef Log_Errori As StringBuilder,
                                               ByRef Log_Riepilogo As StringBuilder,
                                               ByVal Piva_Origine As String,
                                               ByVal Sa_cod_Origine As Integer,
                                               ByVal Piva_Destinazione As String,
                                               ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Particelle_Salva"

        Dim ParticelleR As New AgronicaCoreAnagrafeBIZ.Particella_R
        Dim conteggioParticelle As Integer = 0
        Dim xRisp As Boolean = False

        'memorizza il dato precedente per successivo recupero
        Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

        Try

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale

            Dim sXML_Particelle As String = ParticelleR.ImpresexParticelle_Leggi(
                Piva_Origine,
                Sa_cod_Origine,
                0,
                "",
                "",
                "",
                0,
                0,
                "",
                False,
                AGRODATAINIZIO,
                AGRODATAFINE,
                objOpzioni.objParametri_Server_GIAS_ORIGINE,
                TipoG2G:=1
            )

            If sXML_Particelle <> "" Then

                sXML_Particelle = ElaboraXML_Particelle_SistemaXML(objOpzioni,
                                                                   Log_Import,
                                                                   Log_Errori,
                                                                   Log_Riepilogo,
                                                                   objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                   objOpzioni.objParametri_Utenti_GIAS_ORIGINE,
                                                                   objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                                   objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE,
                                                                   sXML_Particelle,
                                                                   Piva_Destinazione,
                                                                   Sa_cod_Destinazione,
                                                                   conteggioParticelle)

                '  Marco Grilli, 19/06/2014 17:36:18: gestisco se il server è remoto
                If objOpzioni.isGias2Gias_local Then

                    Dim scriviParticella As New AgronicaCoreAnagrafeBIZ.Particella_W
                    xRisp = scriviParticella.Particella_Scrivi(sXML_Particelle, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                Else
                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                    Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                    '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                    sXML_Particelle = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sXML_Particelle)

                    xRisp = wsImportazione.Modifica_ParticelleXmlPrivato(Str_Credenziali_WS, sXML_Particelle, Piva_Destinazione, Sa_cod_Destinazione)

                    If xRisp = False Then
                        Throw New Exception("Errore WS in Modifica_ParticelleXmlPrivato")
                    End If

                End If

            End If

            If conteggioParticelle > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioParticelle & " particelle catastali aggiornate" + vbCrLf)
            Else
                Log_Import.Append("L'impresa " & Piva_Origine & " non ha particelle catastali" + vbCrLf)
            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)

        Finally
            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)
        End Try

    End Sub

    Private Function ElaboraXML_Particelle_SistemaXML(ByVal objOpzioni As clsOpzioni,
                                                      ByRef Log_Import As StringBuilder,
                                                      ByRef Log_Errori As StringBuilder,
                                                      ByRef Log_Riepilogo As StringBuilder,
                                                      ByRef objParametri_Server_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Server_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef Str_XML_Particella As String,
                                                      ByVal Piva_Destinazione As String,
                                                      ByVal saCod_Destinazione As String,
                                                      ByRef conteggioParticelle As Integer
                                                      ) As String

        Const NomeFunzione As String = "Elabora_XML_Particelle_Salva"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Particella)

        Dim Str_XML_Output As String = ""
        Dim XMLs_Nodi As XmlNodeList

        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For Each XML_Nodo As XmlElement In XMLs_Nodi
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)
            Next

            'aggiungo data_inizio e data_fine
            Dim xmlElem As XmlElement = XmlDocCont.SelectSingleNode("//DatiParticelle")
            xmlElem.SetAttribute("data_inizio", objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio)
            xmlElem.SetAttribute("data_fine", objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine)

            'azzeramento part_cod
            XMLs_Nodi = XmlDocCont.SelectNodes("//Particella")
            For Each XML_Nodo As XmlElement In XMLs_Nodi
                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("part_cod", 0)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

                For Each XML_Nodo_Macrouso In XML_Nodo.SelectNodes("Macrouso")
                    XML_Nodo_Macrouso.SetAttribute("piva", Piva_Destinazione)
                    XML_Nodo_Macrouso.SetAttribute("sa_cod", saCod_Destinazione)
                    XML_Nodo_Macrouso.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                    XML_Nodo_Macrouso.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

                    For Each XML_Nodo_Utilizzo In XML_Nodo_Macrouso.SelectNodes("Utilizzo")
                        XML_Nodo_Utilizzo.SetAttribute("piva", Piva_Destinazione)
                        XML_Nodo_Utilizzo.SetAttribute("sa_cod", saCod_Destinazione)
                        XML_Nodo_Utilizzo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                        XML_Nodo_Utilizzo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
                    Next
                Next
            Next
            conteggioParticelle = XMLs_Nodi.Count
            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

#End Region

End Class
