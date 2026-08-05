Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework_POCO

Partial Public Class Funzioni

    Public Function LeggiMateriePrimeCampionatureXML(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GMateriePrimeCampionature_R
        Dim g2g = objLeggi.Leggi_MateriePrimeCampionature_G2G(From_Piva, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, listaImprese, Configurazione.dataValidita, objParametri)

        If g2g.MateriePrimeCampionatureToInsert.Count > 0 OrElse g2g.MateriePrimeCampionatureToUpdate.Count > 0 OrElse g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Count > 0 Then
            Return XMLUtility.SerializzaOggetto(Of G2G_MateriePrimeCampionature)(g2g, "")
        End If

        Return ""

    End Function

    Public Function LeggiMateriePrimeCampionatureXMLReverse(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GMateriePrimeCampionature_R
        Dim g2g = objLeggi.Leggi_MateriePrimeCampionature_G2GReverse(From_Piva, objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser, listaImprese, Configurazione.dataValidita, objParametri)

        If g2g.MateriePrimeCampionatureToInsert.Count > 0 OrElse g2g.MateriePrimeCampionatureToUpdate.Count > 0 OrElse g2g.G2G_Recode_Materie_Prime_Campionature_Delete.Count > 0 Then
            Return XMLUtility.SerializzaOggetto(Of G2G_MateriePrimeCampionature_Reverse)(g2g, "")
        End If

        Return ""

    End Function

    Public Function LeggiAnalisiCondiviseXML(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Analisi_Condivise,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseAnalisi As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GAnalisi_R
        Dim g2g = objLeggi.Leggi_AnalisiCondivise_G2G(From_Piva, listaImprese, Configurazione.dataValidita, objParametri, listaImpreseAnalisi)

        Return ""

    End Function

    Public Function LeggiAnalisiCondiviseXMLReverse(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Analisi_Condivise,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseAnalisi As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per i contatti pubblici forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim listaCodRapporto = New List(Of Integer)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
        End If

        Dim objLeggi As New G2GAnalisi_R
        Dim g2g = objLeggi.Leggi_AnalisiCondivise_G2GReverse(From_Piva, listaImprese, Configurazione.dataValidita, objParametri, listaImpreseAnalisi)

        Return ""

    End Function


    Public Sub Elabora_XML_MateriePrimeCampionature_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature)

        Const NomeFunzione As String = "Elabora_XML_MateriePrimeCampionature_Salva"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
            Dim sXmlMateriePrimeCampionature = LeggiMateriePrimeCampionatureXML(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlMateriePrimeCampionature) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlMateriePrimeCampionature, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiMateriePrimeCampionature")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiMateriePrimeCampionature")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_MateriePrimeCampionature_SalvaReverse(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime_Campionature)

        Const NomeFunzione As String = "Elabora_XML_MateriePrimeCampionature_SalvaReverse"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
            Dim sXmlMateriePrimeCampionature = LeggiMateriePrimeCampionatureXMLReverse(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            If Not String.IsNullOrEmpty(sXmlMateriePrimeCampionature) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlMateriePrimeCampionature, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiMateriePrimeCampionature")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiMateriePrimeCampionature")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            If ex.InnerException IsNot Nothing Then
                msg &= vbCrLf & "InnerException: " & ex.InnerException.Message
            End If
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_AnalisiCondivise_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Analisi_Condivise)

        Const NomeFunzione As String = "Elabora_XML_AnalisiCondivise_Salva"
        Dim MessaggioErrore As String = ""
        Dim listaImpreseAnalisi As New List(Of String)

        Try

            ' leggo struttura dati serializzata su origine contenente materie prime campionature da inserire/modificare/cancellare 
            Dim sXmlAnalisiCondivise = LeggiAnalisiCondiviseXML(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseAnalisi)

            ' scrivo dati serializzati richiamando web service destinazione
            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

            ' creo l'azienza del contatto pubblico su destinazione se non esiste
            If Flag_Pubblico And listaImpreseAnalisi.Count > 0 Then

                Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
                Dim ConnectionString As String = gefutils.GetEntityConnectionString(objOpzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
                Dim efG2G As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(ConnectionString)
                Dim objOpzioniImportImpresa As New clsImpresa

                For Each piva In listaImpreseAnalisi
                    If Not Configurazione.listaImprese.Contains(piva) Then
                        Dim strErr As String = ""
                        Dim pivaEsistente = wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr)
                        If NuovaLogicaRecode Then

                            Dim ErrFLAG = 0
                            ScriviImpreseXML(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        True
                                    )

                            'G2G_Lista_Centri_WS(
                            '    objOpzioniImportImpresa,
                            '    piva,
                            '    piva,
                            '    objOpzioni.ProgressivoGIAS_ORIGINE,
                            '    objOpzioni.ProgressivoGIAS_DESTINAZIONE,
                            '    ErrFLAG
                            ')

                            If ErrFLAG = 0 Then

                                Elabora_XML_Analisi_Salva(
                                    objOpzioniImportImpresa,
                                    efG2G,
                                    objOpzioni,
                                    Log_Import,
                                    Log_Errori,
                                    Log_Riepilogo,
                                    piva,
                                    piva, enum_TipoOperazioneDB.Scrittura
                                )

                            End If


                        ElseIf Not pivaEsistente Then
                            Elabora_XML_Imprese_Salva(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                        False
                                    )
                        End If
                    End If
                Next
            End If

            If Not String.IsNullOrEmpty(sXmlAnalisiCondivise) Then

                ' scrivo dati serializzati richiamando web service destinazione

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlAnalisiCondivise, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiAnalisi_Condivise")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaAnalisiCondivise As XElement = xmlRisposta.Element("Risposta").Element("Risposta_AnalisiCondivise")
                Dim rispostaAnalisiCondivise = xmlRispostaAnalisiCondivise.FirstNode.ToString
                'Dim rispostaContatti = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ContattiXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlContatti)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaAnalisiCondivise, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento analisi condivise: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento analisi condivise ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento materie prime campionature: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

End Class
