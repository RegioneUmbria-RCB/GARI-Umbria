Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni

#Region "Appezzamenti, Impianti"

    Public Function LeggiPianoColturaleXML(ByVal Piva_Origine As String,
                                    ByVal Sa_Cod_Origine As Integer,
                                    ByVal Piva_Destinazione As String,
                                    ByVal Sa_Cod_Destinazione As Integer,
                                    ByRef objOpzioniImportImpresa As clsImpresa,
                                    ByRef objOpzioni As clsOpzioni,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef Log_Import As StringBuilder) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        Dim Impianti = objOpzioniImportImpresa.Impianti

        Dim objCampi As New G2GCampi_R
        Dim g2gCampi = objCampi.Nuovo_Campi_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiCampi = objCampi.Leggi_Campi_G2G(Sa_Cod_Origine, g2gCampi, objParametri, Log_Import)

        Dim objAppezzamenti As New G2GAppezzamenti_R
        Dim g2gAppezzamenti = objAppezzamenti.Nuovo_Appezzamenti_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiAppezzamenti = objAppezzamenti.Leggi_Appezzamenti_G2G(Sa_Cod_Origine, Impianti, g2gAppezzamenti, objParametri, Log_Import)

        Dim objImpianti As New G2GImpianti_R
        Dim g2gImpianti = objImpianti.Nuovo_Impianti_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiImpianti = objImpianti.Leggi_Impianti_G2G(Sa_Cod_Origine, Impianti, g2gImpianti, objParametri, Log_Import)

        Dim objDistinte As New G2GDistinte_R
        Dim g2gDistinte = objDistinte.Nuovo_Distinte_G2G(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiDistinte = If(objOpzioniImportImpresa.flagimporta_distinta, objDistinte.Leggi_Distinte_G2G(Sa_Cod_Origine, Impianti, g2gDistinte, objParametri, Log_Import), False)

        If leggiCampi OrElse leggiAppezzamenti OrElse leggiImpianti OrElse leggiDistinte Then
            Dim g2g As New G2G_PianoColturale With {
                .Campi = If(leggiCampi, g2gCampi, Nothing),
                .Appezzamenti = If(leggiAppezzamenti, g2gAppezzamenti, Nothing),
                .Impianti = If(leggiImpianti, g2gImpianti, Nothing),
                .Distinte = If(leggiDistinte, g2gDistinte, Nothing)
            }
            Return XMLUtility.SerializzaOggetto(Of G2G_PianoColturale)(g2g, "")
        End If

        Return ""

    End Function

    Public Function LeggiPianoColturaleXMLReverse(ByVal Piva_Origine As String,
                                    ByVal Sa_Cod_Origine As Integer,
                                    ByVal Piva_Destinazione As String,
                                    ByVal Sa_Cod_Destinazione As Integer,
                                    ByRef objOpzioniImportImpresa As clsImpresa,
                                    ByRef objOpzioni As clsOpzioni,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim To_Progressivo = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        Dim Impianti = objOpzioniImportImpresa.Impianti

        Dim objCampi As New G2GCampi_R
        Dim g2gCampi = objCampi.Nuovo_Campi_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiCampi = objCampi.Leggi_Campi_G2GReverse(Sa_Cod_Origine, g2gCampi, objParametri)

        Dim objAppezzamenti As New G2GAppezzamenti_R
        Dim g2gAppezzamenti = objAppezzamenti.Nuovo_Appezzamenti_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiAppezzamenti = objAppezzamenti.Leggi_Appezzamenti_G2GReverse(Sa_Cod_Origine, Impianti, g2gAppezzamenti, objParametri)

        Dim objImpianti As New G2GImpianti_R
        Dim g2gImpianti = objImpianti.Nuovo_Impianti_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiImpianti = objImpianti.Leggi_Impianti_G2GReverse(Sa_Cod_Origine, Impianti, g2gImpianti, objParametri)

        Dim objDistinte As New G2GDistinte_R
        Dim g2gDistinte = objDistinte.Nuovo_Distinte_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, Piva_Origine, Piva_Destinazione, Sa_Cod_Destinazione, To_Progressivo)
        Dim leggiDistinte = If(objOpzioniImportImpresa.flagimporta_distinta, objDistinte.Leggi_Distinte_G2GReverse(Sa_Cod_Origine, Impianti, g2gDistinte, objParametri), False)

        If leggiCampi OrElse leggiAppezzamenti OrElse leggiImpianti OrElse leggiDistinte Then
            Dim g2g As New G2G_PianoColturale_Reverse With {
                .Campi = If(leggiCampi, g2gCampi, Nothing),
                .Appezzamenti = If(leggiAppezzamenti, g2gAppezzamenti, Nothing),
                .Impianti = If(leggiImpianti, g2gImpianti, Nothing),
                .Distinte = If(leggiDistinte, g2gDistinte, Nothing)
            }
            Return XMLUtility.SerializzaOggetto(Of G2G_PianoColturale_Reverse)(g2g, "")
        End If

        Return ""

    End Function

    Public Sub ScriviPianoColturaleXML(ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal objOpzioni As clsOpzioni,
                                ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal Piva_Origine As String,
                                ByVal Sa_cod_Origine As Integer,
                                ByVal Piva_Destinazione As String,
                                ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviPianoColturaleXML"
        Dim MessaggioErrore As String = ""

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            ' imposta finestra temporale in base al piano colturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale

            ' leggo struttura dati serializzata su origine contenente i campi centro da inserire/modificare/cancellare 
            Dim sXmlPianoColturale = LeggiPianoColturaleXML(Piva_Origine, Sa_cod_Origine, Piva_Destinazione, Sa_cod_Destinazione, objOpzioniImportImpresa, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE, Log_Import)
            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If Not String.IsNullOrEmpty(sXmlPianoColturale) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPianoColturale, "//utente/DatiImprese/Impresa", "DatiPianoColturale")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaCampi As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPianoColturale")
                Dim rispostaCampi = xmlRispostaCampi.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaCampi, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento piano colturale: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento piano colturale ERRORE IN DESTINAZIONE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento piano colturale: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            If ex.InnerException IsNot Nothing AndAlso ex.InnerException.Message <> "" Then
                msg &= "Inner exception:" & ex.InnerException.Message
            End If
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub ScriviPianoColturaleXMLReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                ByVal objOpzioni As clsOpzioni,
                                ByRef Log_Import As StringBuilder,
                                ByRef Log_Errori As StringBuilder,
                                ByRef Log_Riepilogo As StringBuilder,
                                ByVal Piva_Origine As String,
                                ByVal Sa_cod_Origine As Integer,
                                ByVal Piva_Destinazione As String,
                                ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "ScriviPianoColturaleXML"
        Dim MessaggioErrore As String = ""

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente, FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            ' imposta finestra temporale in base al piano colturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio = objOpzioniImportImpresa.ValiditaInizio_pianocolturale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine = objOpzioniImportImpresa.ValiditaFine_pianocolturale

            ' leggo struttura dati serializzata su origine contenente i campi centro da inserire/modificare/cancellare 
            Dim sXmlPianoColturale = LeggiPianoColturaleXMLReverse(Piva_Origine, Sa_cod_Origine, Piva_Destinazione, Sa_cod_Destinazione, objOpzioniImportImpresa, objOpzioni, objOpzioni.objParametri_Server_GIAS_ORIGINE)

            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If Not String.IsNullOrEmpty(sXmlPianoColturale) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlPianoColturale, "//utente/DatiImprese/Impresa", "DatiPianoColturale")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaCampi As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiPianoColturale")
                Dim rispostaCampi = xmlRispostaCampi.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaCampi, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento piano colturale: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento piano colturale ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento piano colturale: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '##################################################################################################################################
    Public Sub Elabora_XML_Impianti_Salva(ByVal objOpzioniImportImpresa As clsImpresa,
                                          ByVal objOpzioni As clsOpzioni,
                                          ByRef Log_Import As StringBuilder,
                                          ByRef Log_Errori As StringBuilder,
                                          ByRef Log_Riepilogo As StringBuilder,
                                          ByVal Piva_Origine As String,
                                          ByVal Sa_cod_Origine As Integer,
                                          ByVal Piva_Destinazione As String,
                                          ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Impianti_Salva"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim OutputCampoCod As Integer = -1
        Dim OutputAppezza As Integer = -1
        Dim OutputImpianto As Integer = -1
        Dim xCentro As New XmlDocument

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente As Date
            Dim FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio =
                objOpzioniImportImpresa.ValiditaInizio_pianocolturale

            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine =
                objOpzioniImportImpresa.ValiditaFine_pianocolturale


            Dim Str_Credenziali_WS As String = ""

            If Not objOpzioni.isGias2Gias_local Then
                Str_Credenziali_WS = Get_Str_Credenziali_WS(objOpzioni)
            End If

            '===============================================================
            '=================     CAMPI     ===============================
            '===============================================================

            Dim objCampi_R As New AgronicaCoreAnagrafeBIZ.Campo_R
            Dim conteggioCampiNuovi As Integer = 0

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Campi As String = objCampi_R.Campo_Leggi(Piva_Origine,
                                                              Sa_cod_Origine,
                                                              0,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              False,
                                                              True,
                                                              objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                              TipoG2G:=1)

            'Se ci sono campi da elaborare ...
            If sXML_Campi <> "" Then

                Dim objCampi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
                Dim fromCampoCod As Integer

                'Carico il documento
                xCentro.LoadXml(sXML_Campi)

                For Each xCampo As XmlNode In xCentro.SelectNodes("//DatiCampi/Campo")

                    OutputCampoCod = -1

                    'Recupero le chiavi originali
                    fromCampoCod = CInt(xCampo.Attributes("campo_cod").Value)

                    ' VAnni: 20/5/2019: TODO: gestire invio del campo basata su esistenza di almeno un appezzamento che fa riferimento al campo...

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Campi_SistemaXML(objOpzioni,
                                                                    Log_Import,
                                                                    Log_Errori,
                                                                    Log_Riepilogo,
                                                                    "<DatiCampi>" & xCampo.OuterXml & "</DatiCampi>",
                                                                    Piva_Destinazione,
                                                                    Sa_cod_Destinazione,
                                                                    0,
                                                                    enum_TipoOperazioneDB.Scrittura)

                    'Salvo il nuovo campo
                    If objOpzioni.isGias2Gias_local Then
                        objCampi_W.Campo_Scrivi(sStringaDaSalvare,
                                                OutputPiva, OutputSaCod, OutputCampoCod,
                                                False,
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else

                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputCampoCod = wsImportazione.Scrivi_CampoXmlPrivato(Str_Credenziali_WS,
                                                                               sStringaDaSalvare)

                        If OutputCampoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_CampoXmlPrivato")
                        End If

                    End If

                    'Salvo la mappatura delle chiavi nella collezione
                    CampiADD(
                        New G2G_Recode_Campo With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .From_Piva = Piva_Origine,
                            .From_Sa_Cod = Sa_cod_Origine,
                            .From_Campo_cod = fromCampoCod,
                            .To_Piva = Piva_Destinazione,
                            .To_Sa_Cod = Sa_cod_Destinazione,
                            .To_Campo_cod = OutputCampoCod
                        })

                    conteggioCampiNuovi += 1

                Next

            End If

            ' log campi
            If conteggioCampiNuovi > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioCampiNuovi & " campi nuovi" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha campi nuovi" + vbCrLf)
            End If

            '===============================================================
            '=================   / CAMPI     ===============================
            '===============================================================


            '===============================================================
            '=====================  APPEZZAMENTI  ==========================
            '===============================================================

            Dim AppezzaR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim conteggioAppezzamentiNuovi As Integer = 0

            Dim sXML_Appezzamenti As String = AppezzaR.Appezzamento_Leggi(Piva_Origine,
                                                                          Sa_cod_Origine,
                                                                          0,
                                                                          0,
                                                                          False,
                                                                          True,
                                                                          False,
                                                                          False,
                                                                          objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                          TipoG2G:=1)

            'Se ci sono appezzamenti da elaborare ...
            If sXML_Appezzamenti <> "" Then

                Dim objAppezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

                xCentro.LoadXml(sXML_Appezzamenti)

                For Each xAppezzamento As XmlElement In xCentro.SelectNodes("//DatiAppezzamenti/Appezzamento")

                    OutputAppezza = -1

                    Dim DatiAppezzamento As String = Replace(xAppezzamento.OuterXml,
                                                             "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                             "sa_cod=" & Chr(34) & CStr(Sa_cod_Destinazione) & Chr(34) & "")

                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Appezzamenti_SistemaXML(
                        objOpzioniImportImpresa,
                        objOpzioni,
                        Log_Import,
                        Log_Errori,
                        Log_Riepilogo,
                        "<DatiAppezzamenti>" & DatiAppezzamento & "</DatiAppezzamenti>",
                        Piva_Destinazione,
                        Sa_cod_Destinazione,
                        0,
                        enum_TipoOperazioneDB.Scrittura)

                    Dim fromAppezza As Integer = CInt(xAppezzamento.GetAttribute("appezza"))

                    ' VAnni: 20/5/2019: verifico se l'appezzamento (e quindi impianti figli) vanno inviati o meno rispetto alla lista degli impianti.. 
                    '    esiste appezzamento oppure lista vuota = invia tutto

                    Dim xInvioAppezzamenti As Boolean = ((From iC In objOpzioniImportImpresa.Impianti
                                                          Where iC.Sa_Cod = Sa_cod_Origine And
                                                        iC.Appezza = fromAppezza
                                                        ).ToList.Count > 0)

                    If objOpzioniImportImpresa.Impianti.Count = 0 OrElse xInvioAppezzamenti Then

                        If objOpzioni.isGias2Gias_local Then
                            objAppezzamenti_W.Appezzamento_Scrivi(sStringaDaSalvare,
                                                                  OutputPiva, OutputSaCod, OutputAppezza,
                                                                  objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                                  objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                        Else
                            Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                .Timeout = _timeout_ws_Importa,
                                .Url = objOpzioni.wsimportaGiasURl
                            }

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                            OutputAppezza = wsImportazione.Scrivi_AppezzamentoXmlPrivato(Str_Credenziali_WS,
                                                                                         sStringaDaSalvare)

                            If OutputAppezza = -1 Then
                                Throw New Exception("Errore WS in Scrivi_AppezzamentoXmlPrivato")
                            End If
                        End If

                        '  Vanni, 19/05/2014 16:37:06: test entity framework
                        AppezzamentiMappatiADD(
                        New G2G_Recode_Appezzamenti With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .From_Piva = Piva_Origine,
                            .From_Sa_Cod = Sa_cod_Origine,
                            .From_Appezza = fromAppezza,
                            .To_Piva = Piva_Destinazione,
                            .To_Sa_Cod = Sa_cod_Destinazione,
                            .To_Appezza = OutputAppezza
                        })

                        conteggioAppezzamentiNuovi += 1

                    End If

                Next

            End If

            ' log appezzamenti
            If conteggioAppezzamentiNuovi > 0 Then
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " ha " & conteggioAppezzamentiNuovi & " appezzamenti nuovi")
            Else
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " non ha appezzamenti nuovi")
            End If

            '===============================================================
            '===================  /  APPEZZAMENTI  =========================
            '===============================================================


            '===============================================================
            '=====================    IMPIANTI    ==========================
            '===============================================================
            Dim ImpiantiR As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            Dim conteggioImpiantiNuovi As Integer = 0

            Dim sXML_Impianti As String = ImpiantiR.Reg_Impianto_Leggi(Piva_Origine,
                                                                       Sa_cod_Origine,
                                                                       0,
                                                                       0,
                                                                       False,
                                                                       True,
                                                                       objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                       TipoG2G:=1)

            'Se ci sono impianti da elaborare ...
            If sXML_Impianti <> "" Then

                xCentro.LoadXml(sXML_Impianti)

                For Each xImpianto As XmlElement In xCentro.SelectNodes("DatiReg_Impianti/Reg_Impianto")

                    OutputImpianto = -1

                    Dim fromIdReg As Integer = CInt(xImpianto.GetAttribute("id_reg"))
                    Dim fromAppezza As Integer = CInt(xImpianto.GetAttribute("appezza"))

                    ' VAnni: 20/5/2019: verifico se l'impianto deve essere inviato o meno rispetto alla lista degli impianti.. 
                    '    esiste impianto oppure lista vuota = invia tutto

                    Dim xInvioImpianto As Boolean = ((From iC In objOpzioniImportImpresa.Impianti
                                                      Where iC.Sa_Cod = Sa_cod_Origine And
                                                            iC.Appezza = fromAppezza And
                                                            iC.ID_Reg = fromIdReg
                                                            ).ToList.Count > 0)

                    If objOpzioniImportImpresa.Impianti.Count = 0 OrElse xInvioImpianto Then


                        Dim objApp As G2G_Recode_Appezzamenti = recode_return_Appezzamento(Piva_Origine, Sa_cod_Origine,
                                                                                           fromAppezza,
                                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                        Dim appezzamento_Destinazione As Integer = objApp.To_Appezza

                        Dim sStringaDaSalvareImpianto As String
                        sStringaDaSalvareImpianto = ElaboraXML_Impianti_SistemaXML(objOpzioni,
                                                                                   Log_Import,
                                                                                   Log_Errori,
                                                                                   Log_Riepilogo,
                                                                                   xImpianto.OuterXml,
                                                                                   Piva_Destinazione,
                                                                                   Sa_cod_Destinazione,
                                                                                   0,
                                                                                   0,
                                                                                   enum_TipoOperazioneDB.Scrittura)

                        'Sostituisco all'Appezza=0 il codice dell'Appezzamento mappato
                        sStringaDaSalvareImpianto = Replace(sStringaDaSalvareImpianto,
                                                "appezza=" & Chr(34) & "0" & Chr(34) & "",
                                                "appezza=" & Chr(34) & CStr(appezzamento_Destinazione) & Chr(34) & "")

                        sStringaDaSalvareImpianto = "<DatiReg_Impianti>" & sStringaDaSalvareImpianto & "</DatiReg_Impianti>"

                        If objOpzioni.isGias2Gias_local Then
                            Dim objImpianti_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                            objImpianti_W.Reg_Impianto_Scrivi(sStringaDaSalvareImpianto, OutputPiva,
                                                              OutputSaCod, OutputAppezza, OutputImpianto,
                                                              "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                        Else
                            Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                .Timeout = _timeout_ws_Importa,
                                .Url = objOpzioni.wsimportaGiasURl
                            }

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvareImpianto = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvareImpianto)

                            OutputImpianto = wsImportazione.Scrivi_Reg_ImpiantoXmlPrivato(Str_Credenziali_WS,
                                                                                          sStringaDaSalvareImpianto)

                            If OutputImpianto = -1 Then
                                Throw New Exception("Errore WS in Scrivi_Reg_ImpiantoXmlPrivato")
                            End If
                        End If

                        '  Vanni, 19/05/2014 16:42:01: test entity framework
                        ImpiantiMappatiADD(
                                        New G2G_Recode_Impianti With {
                                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                            .From_Piva = Piva_Origine,
                                            .From_Sa_Cod = Sa_cod_Origine,
                                            .From_Appezza = fromAppezza,
                                            .From_Id_Reg = fromIdReg,
                                            .To_Piva = Piva_Destinazione,
                                            .To_Sa_Cod = Sa_cod_Destinazione,
                                            .To_Appezza = appezzamento_Destinazione,
                                            .To_Id_Reg = OutputImpianto
                                        })

                        conteggioImpiantiNuovi += 1

                    End If

                Next

            End If

            ' log impianti
            If conteggioImpiantiNuovi > 0 Then
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " ha " & conteggioImpiantiNuovi & " impianti nuovi")
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha impianti nuovi" + vbCrLf)
            End If

            '===============================================================
            '===================    /  IMPIANTI    =========================
            '===============================================================

            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub




    '##################################################################################################################################
    '##### VERSIONE MODIFICATA PER GESTIRE ANCHE I CAMPI ##############################################################################
    '##################################################################################################################################
    Public Sub Elabora_XML_Appezzamenti_Salva(ByVal objOpzioniImportImpresa As clsImpresa,
                                              ByVal objOpzioni As clsOpzioni,
                                              ByRef Log_Import As StringBuilder,
                                              ByRef Log_Errori As StringBuilder,
                                              ByRef Log_Riepilogo As StringBuilder,
                                              ByVal Piva_Origine As String,
                                              ByVal Sa_cod_Origine As Integer,
                                              ByVal Piva_Destinazione As String,
                                              ByVal Sa_cod_Destinazione As Integer)

        Const NomeFunzione As String = "Elabora_XML_Appezzamenti_Salva"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim OutputCampoCod As Integer = -1
        Dim OutputAppezza As Integer = -1
        Dim OutputImpianto As Integer = -1
        Dim xCentro As New XmlDocument

        'Dim xMarcareInviatoCampo As New AgronicaCoreAnagrafeDAL.Campi_W

        Try

            Dim Str_Credenziali_WS As String = ""

            If Not objOpzioni.isGias2Gias_local Then
                Str_Credenziali_WS = Get_Str_Credenziali_WS(objOpzioni)
            End If



            '===============================================================
            '=================     CAMPI     ===============================
            '===============================================================

            Dim objCampi_R As New AgronicaCoreAnagrafeBIZ.Campo_R

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Campi As String = objCampi_R.Campo_Leggi(Piva_Origine,
                                                              Sa_cod_Origine,
                                                              0,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              False,
                                                              True,
                                                              objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                              TipoG2G:=1)

            'Se ci sono campi da elaborare ...
            If sXML_Campi <> "" Then

                Dim objCampi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
                Dim fromCampoCod As Integer

                'Carico il documento
                xCentro.LoadXml(sXML_Campi)

                For Each xCampo As XmlNode In xCentro.SelectNodes("//DatiCampi/Campo")

                    'Recupero le chiavi originali
                    fromCampoCod = CInt(xCampo.Attributes("campo_cod").Value)

                    ' VAnni: 20/5/2019: TODO: gestire invio del campo basata su esistenza di almeno un'appezzamento che fa riferimento al campo...

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Campi_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    "<DatiCampi>" & xCampo.OuterXml & "</DatiCampi>",
                                                    Piva_Destinazione,
                                                    Sa_cod_Destinazione,
                                                    0,
                                                    enum_TipoOperazioneDB.Scrittura
                                                    )

                    'Salvo il nuovo campo
                    If objOpzioni.isGias2Gias_local Then
                        objCampi_W.Campo_Scrivi(sStringaDaSalvare,
                                                OutputPiva, OutputSaCod, OutputCampoCod,
                                                False,
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else

                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputCampoCod = wsImportazione.Scrivi_CampoXmlPrivato(Str_Credenziali_WS,
                                                                               sStringaDaSalvare)

                        If OutputCampoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_CampoXmlPrivato")
                        End If

                    End If

                    'Salvo la mappatura delle chiavi nella collezione
                    CampiADD(
                        New G2G_Recode_Campo With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .From_Piva = Piva_Origine,
                            .From_Sa_Cod = Sa_cod_Origine,
                            .From_Campo_cod = fromCampoCod,
                            .To_Piva = Piva_Destinazione,
                            .To_Sa_Cod = Sa_cod_Destinazione,
                            .To_Campo_cod = OutputCampoCod
                        })

                    'xMarcareInviatoCampo.Campo_MarcaComeInviato(Piva_Origine, Sa_cod_Origine, fromCampoCod, Now, "", objOpzioni.objParametri_Server_GIAS_ORIGINE)


                Next

            End If

            '===============================================================
            '=================   / CAMPI     ===============================
            '===============================================================




            '===============================================================
            '=====================  APPEZZAMENTI  ==========================
            '===============================================================

            Dim AppezzaR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            'Dim xMarcareInviatoAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Write

            Dim sXML_Appezzamenti As String = AppezzaR.Appezzamento_Leggi(Piva_Origine,
                                                                          Sa_cod_Origine,
                                                                          0,
                                                                          0,
                                                                          False,
                                                                          True,
                                                                          False,
                                                                          False,
                                                                          objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                          TipoG2G:=1)

            If sXML_Appezzamenti <> "" Then

                sXML_Appezzamenti = ElaboraXML_Appezzamenti_LeggiImpianti(objOpzioni,
                                                                          Log_Import,
                                                                          Log_Errori,
                                                                          Log_Riepilogo,
                                                                          sXML_Appezzamenti,
                                                                          Piva_Origine,
                                                                          Sa_cod_Origine,
                                                                          Piva_Destinazione,
                                                                          Sa_cod_Destinazione,
                                                                          TipoG2G:=1)

                Dim DatiAppezzamento As String
                Dim fromAppezza As Integer
                Dim objAppezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                Dim xAppezzamentoClone As XmlElement
                Dim xDatiImpiantiClone As XmlNodeList
                Dim xmlDocClone2 As New XmlDocument

                xCentro.LoadXml(sXML_Appezzamenti)


                Dim conteggioAppezzamentiNuovi As Integer = 0
                For Each xAppezzamento As XmlNode In xCentro.SelectNodes("//DatiAppezzamenti/Appezzamento")

                    'Invio al componente di gestione appezzamenti
                    '                     Set objAppezzamenti_W = CreateObject("Agro_Anagrafe.Appezzamento_W")

                    DatiAppezzamento = Replace(xAppezzamento.OuterXml,
                                              "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                              "sa_cod=" & Chr(34) & CStr(Sa_cod_Destinazione) & Chr(34) & "")

                    'Faccio una copia del nodo precedente
                    xmlDocClone2.LoadXml(DatiAppezzamento)
                    xAppezzamentoClone = xmlDocClone2.GetElementsByTagName("Appezzamento")(0)

                    'Estraggo il nodo figlio "DatiReg_Impianti"
                    xDatiImpiantiClone = xAppezzamentoClone.GetElementsByTagName("DatiReg_Impianti")

                    'Elimino i dati relativi ai Centri_Aziendali dal nodo clonato
                    If xDatiImpiantiClone.Count > 0 Then
                        xAppezzamentoClone.RemoveChild(xDatiImpiantiClone.Item(0))
                    End If

                    Dim sStringaDaSalvare As String
                    sStringaDaSalvare = ElaboraXML_Appezzamenti_SistemaXML(
                        objOpzioniImportImpresa,
                        objOpzioni,
                        Log_Import,
                        Log_Errori,
                        Log_Riepilogo,
                        "<DatiAppezzamenti>" & xAppezzamentoClone.OuterXml & "</DatiAppezzamenti>",
                        Piva_Destinazione,
                        Sa_cod_Destinazione,
                        0,
                        enum_TipoOperazioneDB.Scrittura)

                    fromAppezza = CInt(xAppezzamentoClone.GetAttribute("appezza"))

                    ' VAnni: 20/5/2019: verifico se l'appezzamento (e quindi impianti figli) vanno inviati o meno rispetto alla lista degli impianti.. 
                    '    esiste appezzamento oppure lista vuota = invia tutto

                    Dim xInvioAppezzamenti As Boolean = ((From iC In objOpzioniImportImpresa.Impianti
                                                          Where iC.Sa_Cod = Sa_cod_Origine And
                                                        iC.Appezza = fromAppezza
                                                        ).ToList.Count > 0)

                    If objOpzioniImportImpresa.Impianti.Count = 0 OrElse xInvioAppezzamenti Then



                        If objOpzioni.isGias2Gias_local Then
                            objAppezzamenti_W.Appezzamento_Scrivi(sStringaDaSalvare,
                                                                  OutputPiva, OutputSaCod, OutputAppezza,
                                                                  objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                                  objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                        Else
                            Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                .Timeout = _timeout_ws_Importa,
                                .Url = objOpzioni.wsimportaGiasURl
                            }

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                            OutputAppezza = wsImportazione.Scrivi_AppezzamentoXmlPrivato(Str_Credenziali_WS,
                                                                                         sStringaDaSalvare)

                            If OutputAppezza = -1 Then
                                Throw New Exception("Errore WS in Scrivi_AppezzamentoXmlPrivato")
                            End If
                        End If

                        '  Vanni, 19/05/2014 16:37:06: test entity framework
                        AppezzamentiMappatiADD(
                        New G2G_Recode_Appezzamenti With {
                            .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                            .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                            .From_Piva = Piva_Origine,
                            .From_Sa_Cod = Sa_cod_Origine,
                            .From_Appezza = fromAppezza,
                            .To_Piva = Piva_Destinazione,
                            .To_Sa_Cod = Sa_cod_Destinazione,
                            .To_Appezza = OutputAppezza
                        })

                        'xMarcareInviatoAppezzamento.Appezzamento_MarcaComeInviato(Piva_Origine, Sa_cod_Origine, fromAppezza, Now, "", objOpzioni.objParametri_Server_GIAS_ORIGINE)


                        '============================================================
                        '====================     IMPIANTO       ====================
                        '============================================================

                        'Dim xMarcareInviatoRegImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
                        Dim objImpianti_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                        Dim fromIdReg As Integer

                        For Each xImpianto As XmlElement In xAppezzamento.SelectNodes("DatiReg_Impianti/Reg_Impianto")

                            fromIdReg = CInt(xImpianto.GetAttribute("id_reg"))



                            ' VAnni: 20/5/2019: verifico se l'impianto deve essere inviato o meno rispetto alla lista degli impianti.. 
                            '    esiste impianto oppure lista vuota = invia tutto

                            Dim xInvioImpianto As Boolean = ((From iC In objOpzioniImportImpresa.Impianti
                                                              Where iC.Sa_Cod = Sa_cod_Origine And
                                                        iC.Appezza = fromAppezza And
                                                        iC.ID_Reg = fromIdReg
                                                        ).ToList.Count > 0)

                            If objOpzioniImportImpresa.Impianti.Count = 0 OrElse xInvioImpianto Then


                                Dim sStringaDaSalvareImpianto As String
                                sStringaDaSalvareImpianto = ElaboraXML_Impianti_SistemaXML(
                                    objOpzioni,
                                    Log_Import,
                                    Log_Errori,
                                    Log_Riepilogo,
                                    xImpianto.OuterXml,
                                    Piva_Destinazione,
                                    Sa_cod_Destinazione,
                                    0,
                                    0,
                                    enum_TipoOperazioneDB.Scrittura)

                                'Sostituisco all'Appezza=0 il codice che mi ha restituito la chiamata al componente
                                'di scrittura dell'Appezzamento
                                sStringaDaSalvareImpianto = Replace(sStringaDaSalvareImpianto,
                                            "appezza=" & Chr(34) & "0" & Chr(34) & "",
                                            "appezza=" & Chr(34) & CStr(OutputAppezza) & Chr(34) & "")

                                sStringaDaSalvareImpianto = "<DatiReg_Impianti>" & sStringaDaSalvareImpianto & "</DatiReg_Impianti>"

                                If objOpzioni.isGias2Gias_local Then
                                    objImpianti_W.Reg_Impianto_Scrivi(sStringaDaSalvareImpianto, OutputPiva,
                                                                      OutputSaCod, OutputAppezza, OutputImpianto,
                                                                      "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                                Else
                                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                        .Timeout = _timeout_ws_Importa,
                                        .Url = objOpzioni.wsimportaGiasURl
                                    }

                                    '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                                    sStringaDaSalvareImpianto = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvareImpianto)

                                    OutputImpianto = wsImportazione.Scrivi_Reg_ImpiantoXmlPrivato(Str_Credenziali_WS,
                                                                                                  sStringaDaSalvareImpianto)

                                    If OutputImpianto = -1 Then
                                        Throw New Exception("Errore WS in Scrivi_Reg_ImpiantoXmlPrivato")
                                    End If
                                End If

                                '  Vanni, 19/05/2014 16:42:01: test entity framework
                                ImpiantiMappatiADD(
                                    New G2G_Recode_Impianti With {
                                        .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                        .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                                        .From_Piva = Piva_Origine,
                                        .From_Sa_Cod = Sa_cod_Origine,
                                        .From_Appezza = fromAppezza,
                                        .From_Id_Reg = fromIdReg,
                                        .To_Piva = Piva_Destinazione,
                                        .To_Sa_Cod = Sa_cod_Destinazione,
                                        .To_Appezza = OutputAppezza,
                                        .To_Id_Reg = OutputImpianto
                                    })

                                'xMarcareInviatoRegImpianto.Reg_Impianti_MarcaComeInviato(Piva_Origine, Sa_cod_Origine, fromAppezza, fromIdreg, Now, "", objOpzioni.objParametri_Server_GIAS_ORIGINE)

                            End If
                            'x invio impianto

                        Next

                        '===============================================================
                        '===================    /  IMPIANTI    =========================
                        '===============================================================

                        conteggioAppezzamentiNuovi += 1

                    End If
                    'test su invio appezzamento

                Next

                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " ha " & conteggioAppezzamentiNuovi & " appezzamenti nuovi")
            Else
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " non ha appezzamenti nuovi")
            End If

            '===============================================================
            '===================  /  APPEZZAMENTI  =========================
            '===============================================================

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub


    Public Sub Elabora_XML_Appezzamenti_Modificati(ByVal objOpzioniImportImpresa As clsImpresa,
                                                   ByVal objOpzioni As clsOpzioni,
                                                   ByRef Log_Import As StringBuilder,
                                                   ByRef Log_Errori As StringBuilder,
                                                   ByRef Log_Riepilogo As StringBuilder,
                                                   ByVal Piva_Origine As String,
                                                   ByVal Sa_cod_Origine As Integer,
                                                   ByVal Piva_Destinazione As String,
                                                   ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Appezzamenti_Modificati"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim OutputCampoCod As Integer = -1
        Dim OutputAppezza As Integer = -1
        Dim OutputImpianto As Integer = -1
        Dim xCentro As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = ""

            If Not objOpzioni.isGias2Gias_local Then
                Str_Credenziali_WS = Get_Str_Credenziali_WS(objOpzioni)
            End If


            '===============================================================
            '=================     CAMPI     ===============================
            '===============================================================

            Dim objCampi_R As New AgronicaCoreAnagrafeBIZ.Campo_R

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Campi As String = objCampi_R.Campo_Leggi(Piva_Origine,
                                                              Sa_cod_Origine,
                                                              0,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              False,
                                                              True,
                                                              objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                              TipoG2G:=2)

            'Se ci sono campi da elaborare ...
            If sXML_Campi <> "" Then

                'Carico il documento
                xCentro.LoadXml(sXML_Campi)

                For Each xCampo As XmlNode In xCentro.SelectNodes("//DatiCampi/Campo")

                    Dim fromCampoCod As Integer = CInt(xCampo.Attributes("campo_cod").Value)

                    Dim objCampo As G2G_Recode_Campo = recode_return_Campo(Piva_Origine, Sa_cod_Origine, fromCampoCod,
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim campo_cod_Destinazione As Integer = objCampo.To_Campo_cod

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String = ElaboraXML_Campi_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    "<DatiCampi>" & xCampo.OuterXml & "</DatiCampi>",
                                                    Piva_Destinazione,
                                                    Sa_cod_Destinazione,
                                                    campo_cod_Destinazione,
                                                    enum_TipoOperazioneDB.Modifica
                                                    )

                    'Salvo il nuovo campo
                    If objOpzioni.isGias2Gias_local Then
                        Dim objCampi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
                        objCampi_W.Campo_Scrivi(sStringaDaSalvare,
                                                OutputPiva, OutputSaCod, OutputCampoCod,
                                                False,
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else

                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputCampoCod = wsImportazione.Scrivi_CampoXmlPrivato(Str_Credenziali_WS,
                                                                               sStringaDaSalvare)

                        If OutputCampoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_CampoXmlPrivato")
                        End If

                    End If

                    'Aggiorno la data_modifica del campo
                    CampiEDIT(objCampo)

                Next

            End If

            '===============================================================
            '=================   / CAMPI     ===============================
            '===============================================================




            '===============================================================
            '=====================  APPEZZAMENTI  ==========================
            '===============================================================

            Dim AppezzaR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim sXML_Appezzamenti As String = AppezzaR.Appezzamento_Leggi(Piva_Origine,
                                                                          Sa_cod_Origine,
                                                                          0,
                                                                          0,
                                                                          False,
                                                                          True,
                                                                          False,
                                                                          False,
                                                                          objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                          TipoG2G:=2)

            If sXML_Appezzamenti <> "" Then

                sXML_Appezzamenti = ElaboraXML_Appezzamenti_LeggiImpianti(
                                                                objOpzioni,
                                                                Log_Import,
                                                                Log_Errori,
                                                                Log_Riepilogo,
                                                                sXML_Appezzamenti,
                                                                Piva_Origine,
                                                                Sa_cod_Origine,
                                                                Piva_Destinazione,
                                                                Sa_cod_Destinazione,
                                                                TipoG2G:=2)

                Dim DatiAppezzamento As String
                Dim fromAppezza As Integer
                Dim objAppezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                Dim xAppezzamentoClone As XmlElement
                Dim xDatiImpiantiClone As XmlNodeList
                Dim xmlDocClone2 As New XmlDocument

                xCentro.LoadXml(sXML_Appezzamenti)

                Dim conteggioAppezzamentiModificati As Integer = 0

                For Each xAppezzamento As XmlNode In xCentro.SelectNodes("//DatiAppezzamenti/Appezzamento")

                    DatiAppezzamento = Replace(xAppezzamento.OuterXml,
                                              "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                              "sa_cod=" & Chr(34) & CStr(Sa_cod_Destinazione) & Chr(34) & "")

                    'Faccio una copia del nodo precedente
                    xmlDocClone2.LoadXml(DatiAppezzamento)
                    xAppezzamentoClone = xmlDocClone2.GetElementsByTagName("Appezzamento")(0)

                    'Estraggo il nodo figlio "DatiReg_Impianti"
                    xDatiImpiantiClone = xAppezzamentoClone.GetElementsByTagName("DatiReg_Impianti")

                    'Elimino i dati relativi ai Centri_Aziendali dal nodo clonato
                    If xDatiImpiantiClone.Count > 0 Then
                        xAppezzamentoClone.RemoveChild(xDatiImpiantiClone.Item(0))
                    End If

                    fromAppezza = CInt(xAppezzamentoClone.GetAttribute("appezza"))

                    Dim objApp As G2G_Recode_Appezzamenti = recode_return_Appezzamento(Piva_Origine, Sa_cod_Origine,
                                                                                       fromAppezza,
                                                                                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim appezzamento_Destinazione As Integer = objApp.To_Appezza


                    Dim sStringaDaSalvare As String = ElaboraXML_Appezzamenti_SistemaXML(
                        objOpzioniImportImpresa,
                        objOpzioni,
                        Log_Import,
                        Log_Errori,
                        Log_Riepilogo,
                        "<DatiAppezzamenti>" & xAppezzamentoClone.OuterXml & "</DatiAppezzamenti>",
                        Piva_Destinazione,
                        Sa_cod_Destinazione,
                        appezzamento_Destinazione,
                        enum_TipoOperazioneDB.Modifica)

                    If objOpzioni.isGias2Gias_local Then
                        objAppezzamenti_W.Appezzamento_Scrivi(sStringaDaSalvare,
                                                              OutputPiva, OutputSaCod, OutputAppezza,
                                                              objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                              objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputAppezza = wsImportazione.Scrivi_AppezzamentoXmlPrivato(Str_Credenziali_WS,
                                                                                     sStringaDaSalvare)

                        If OutputAppezza = -1 Then
                            Throw New Exception("Errore WS in Scrivi_AppezzamentoXmlPrivato")
                        End If
                    End If

                    'Aggiorno la data_modifica dell'appezzamento
                    AppezzamentiMappatiEDIT(objApp)


                    '============================================================
                    '====================     IMPIANTO       ====================
                    '============================================================

                    Dim objImpianti_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

                    For Each xImpianto As XmlElement In xAppezzamento.SelectNodes("DatiReg_Impianti/Reg_Impianto")

                        Dim fromIdReg As Integer = CInt(xImpianto.GetAttribute("id_reg"))

                        Dim objImp As G2G_Recode_Impianti = recode_return_Impianto(Piva_Origine, Sa_cod_Origine,
                                                                                   fromAppezza, fromIdReg,
                                                                                   objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                        Dim impianto_cod_Destinazione As Integer = objImp.To_Id_Reg

                        Dim sStringaDaSalvareImpianto As String = ElaboraXML_Impianti_SistemaXML(objOpzioni,
                                                                                                Log_Import,
                                                                                                Log_Errori,
                                                                                                Log_Riepilogo,
                                                                                                xImpianto.OuterXml,
                                                                                                Piva_Destinazione,
                                                                                                Sa_cod_Destinazione,
                                                                                                appezzamento_Destinazione,
                                                                                                impianto_cod_Destinazione,
                                                                                                enum_TipoOperazioneDB.Modifica)

                        'Sostituisco all'Appezza=0 il codice che mi ha restituito la chiamata al componente
                        'di scrittura dell'Appezzamento
                        sStringaDaSalvareImpianto = Replace(sStringaDaSalvareImpianto,
                                            "appezza=" & Chr(34) & "0" & Chr(34) & "",
                                            "appezza=" & Chr(34) & CStr(OutputAppezza) & Chr(34) & "")

                        sStringaDaSalvareImpianto = "<DatiReg_Impianti>" & sStringaDaSalvareImpianto & "</DatiReg_Impianti>"

                        If objOpzioni.isGias2Gias_local Then
                            objImpianti_W.Reg_Impianto_Scrivi(sStringaDaSalvareImpianto, OutputPiva,
                                                              OutputSaCod, OutputAppezza, OutputImpianto,
                                                              "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                        Else
                            Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                                .Timeout = _timeout_ws_Importa,
                                .Url = objOpzioni.wsimportaGiasURl
                            }

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvareImpianto = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvareImpianto)

                            OutputImpianto = wsImportazione.Scrivi_Reg_ImpiantoXmlPrivato(Str_Credenziali_WS,
                                                                                          sStringaDaSalvareImpianto)

                            If OutputImpianto = -1 Then
                                Throw New Exception("Errore WS in Scrivi_Reg_ImpiantoXmlPrivato")
                            End If
                        End If

                        'Aggiorno la data_modifica dell'impianto
                        ImpiantiMappatiEDIT(objImp)

                    Next

                    '===============================================================
                    '===================    /  IMPIANTI    =========================
                    '===============================================================

                    conteggioAppezzamentiModificati += 1
                Next

                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " ha " & conteggioAppezzamentiModificati & " appezzamenti modificati")
            Else
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " non ha appezzamenti modificati")
            End If

            '===============================================================
            '===================  /  APPEZZAMENTI  =========================
            '===============================================================


            '===============================================================
            '=====================    IMPIANTI    ==========================
            '===============================================================
            Dim ImpiantiR As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            Dim sXML_Impianti As String = ImpiantiR.Reg_Impianto_Leggi(Piva_Origine,
                                                                       Sa_cod_Origine,
                                                                       0,
                                                                       0,
                                                                       False,
                                                                       True,
                                                                       objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                       TipoG2G:=2)

            xCentro.LoadXml(sXML_Impianti)

            For Each xImpianto As XmlElement In xCentro.SelectNodes("DatiReg_Impianti/Reg_Impianto")

                Dim fromIdReg As Integer = CInt(xImpianto.GetAttribute("id_reg"))
                Dim fromAppezza As Integer = CInt(xImpianto.GetAttribute("appezza"))

                Dim objImp As G2G_Recode_Impianti = recode_return_Impianto(Piva_Origine, Sa_cod_Origine,
                                                                           fromAppezza, fromIdReg,
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                Dim impianto_cod_Destinazione As Integer = objImp.To_Id_Reg
                Dim appezzamento_Destinazione As Integer = objImp.To_Appezza

                Dim sStringaDaSalvareImpianto As String = ElaboraXML_Impianti_SistemaXML(
                    objOpzioni,
                    Log_Import,
                    Log_Errori,
                    Log_Riepilogo,
                    xImpianto.OuterXml,
                    Piva_Destinazione,
                    Sa_cod_Destinazione,
                    appezzamento_Destinazione,
                    impianto_cod_Destinazione,
                    enum_TipoOperazioneDB.Modifica)

                ''Sostituisco all'Appezza=0 il codice che mi ha restituito la chiamata al componente
                ''di scrittura dell'Appezzamento
                'sStringaDaSalvareImpianto = Replace(sStringaDaSalvareImpianto, _
                '                    "appezza=" & Chr(34) & "0" & Chr(34) & "", _
                '                    "appezza=" & Chr(34) & CStr(OutputAppezza) & Chr(34) & "")

                sStringaDaSalvareImpianto = "<DatiReg_Impianti>" & sStringaDaSalvareImpianto & "</DatiReg_Impianti>"

                If objOpzioni.isGias2Gias_local Then
                    Dim objImpianti2_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                    objImpianti2_W.Reg_Impianto_Scrivi(sStringaDaSalvareImpianto, OutputPiva,
                                                       OutputSaCod, OutputAppezza, OutputImpianto,
                                                       "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                Else
                    Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                        .Timeout = _timeout_ws_Importa,
                        .Url = objOpzioni.wsimportaGiasURl
                    }

                    '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                    sStringaDaSalvareImpianto = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvareImpianto)

                    OutputImpianto = wsImportazione.Scrivi_Reg_ImpiantoXmlPrivato(Str_Credenziali_WS,
                                                                                  sStringaDaSalvareImpianto)

                    If OutputImpianto = -1 Then
                        Throw New Exception("Errore WS in Scrivi_Reg_ImpiantoXmlPrivato")
                    End If
                End If

                'Aggiorno la data_modifica dell'impianto
                ImpiantiMappatiEDIT(objImp)

            Next



            '===============================================================
            '===================    /  IMPIANTI    =========================
            '===============================================================

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Impianti_Modificati(ByVal objOpzioniImportImpresa As clsImpresa,
                                               ByVal objOpzioni As clsOpzioni,
                                               ByRef Log_Import As StringBuilder,
                                               ByRef Log_Errori As StringBuilder,
                                               ByRef Log_Riepilogo As StringBuilder,
                                               ByVal Piva_Origine As String,
                                               ByVal Sa_cod_Origine As Integer,
                                               ByVal Piva_Destinazione As String,
                                               ByVal Sa_cod_Destinazione As Integer)

        Const nomeFunzione = "Elabora_XML_Impianti_Modificati"

        Dim OutputPiva As String = ""
        Dim OutputSaCod As Integer = 0
        Dim OutputCampoCod As Integer = -1
        Dim OutputAppezza As Integer = -1
        Dim OutputImpianto As Integer = -1
        Dim xCentro As New XmlDocument

        Try

            Dim Str_Credenziali_WS As String = ""

            If Not objOpzioni.isGias2Gias_local Then
                Str_Credenziali_WS = Get_Str_Credenziali_WS(objOpzioni)
            End If


            '===============================================================
            '=================     CAMPI     ===============================
            '===============================================================

            Dim objCampi_R As New AgronicaCoreAnagrafeBIZ.Campo_R
            Dim conteggioCampiModificati As Integer = 0

            'Carico l'elenco dei campi del Centro Aziendale
            Dim sXML_Campi As String = objCampi_R.Campo_Leggi(Piva_Origine,
                                                              Sa_cod_Origine,
                                                              0,
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              False,
                                                              True,
                                                              objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                              TipoG2G:=2)

            'Se ci sono campi da elaborare ...
            If sXML_Campi <> "" Then

                'Carico il documento
                xCentro.LoadXml(sXML_Campi)

                For Each xCampo As XmlNode In xCentro.SelectNodes("//DatiCampi/Campo")

                    OutputCampoCod = -1

                    Dim fromCampoCod As Integer = CInt(xCampo.Attributes("campo_cod").Value)

                    Dim objCampo As G2G_Recode_Campo = recode_return_Campo(Piva_Origine, Sa_cod_Origine, fromCampoCod,
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim campo_cod_Destinazione As Integer = objCampo.To_Campo_cod

                    'Preparo la stringa XML per il salvataggio
                    Dim sStringaDaSalvare As String = ElaboraXML_Campi_SistemaXML(
                                                    objOpzioni,
                                                    Log_Import,
                                                    Log_Errori,
                                                    Log_Riepilogo,
                                                    "<DatiCampi>" & xCampo.OuterXml & "</DatiCampi>",
                                                    Piva_Destinazione,
                                                    Sa_cod_Destinazione,
                                                    campo_cod_Destinazione,
                                                    enum_TipoOperazioneDB.Modifica
                                                    )

                    'Salvo il nuovo campo
                    If objOpzioni.isGias2Gias_local Then
                        Dim objCampi_W As New AgronicaCoreAnagrafeBIZ.Campo_W
                        objCampi_W.Campo_Scrivi(sStringaDaSalvare,
                                                OutputPiva, OutputSaCod, OutputCampoCod,
                                                False,
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else

                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputCampoCod = wsImportazione.Scrivi_CampoXmlPrivato(Str_Credenziali_WS,
                                                                               sStringaDaSalvare)

                        If OutputCampoCod = -1 Then
                            Throw New Exception("Errore WS in Scrivi_CampoXmlPrivato")
                        End If
                    End If

                    'Aggiorno la data_modifica del campo
                    CampiEDIT(objCampo)

                    conteggioCampiModificati += 1

                Next

            End If

            ' log campi
            If conteggioCampiModificati > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioCampiModificati & " campi modificati" + vbCrLf)
            Else
                ' Log_Import.Append("L'impresa " & Piva_Origine & " non ha campi modificati " + vbCrLf)
            End If

            '===============================================================
            '=================   / CAMPI     ===============================
            '===============================================================


            '===============================================================
            '=====================  APPEZZAMENTI  ==========================
            '===============================================================

            Dim AppezzaR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim conteggioAppezzamentiModificati As Integer = 0

            Dim sXML_Appezzamenti As String = AppezzaR.Appezzamento_Leggi(Piva_Origine,
                                                                          Sa_cod_Origine,
                                                                          0,
                                                                          0,
                                                                          False,
                                                                          True,
                                                                          False,
                                                                          False,
                                                                          objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                          TipoG2G:=2)

            If sXML_Appezzamenti <> "" Then

                Dim objAppezzamenti_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

                xCentro.LoadXml(sXML_Appezzamenti)

                For Each xAppezzamento As XmlElement In xCentro.SelectNodes("//DatiAppezzamenti/Appezzamento")

                    OutputAppezza = -1

                    Dim DatiAppezzamento As String = Replace(xAppezzamento.OuterXml,
                                                             "sa_cod=" & Chr(34) & "0" & Chr(34) & "",
                                                             "sa_cod=" & Chr(34) & CStr(Sa_cod_Destinazione) & Chr(34) & "")

                    Dim fromAppezza As Integer = CInt(xAppezzamento.GetAttribute("appezza"))

                    Dim objApp As G2G_Recode_Appezzamenti = recode_return_Appezzamento(Piva_Origine, Sa_cod_Origine, fromAppezza,
                                                                                       objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim appezzamento_Destinazione As Integer = objApp.To_Appezza

                    Dim sStringaDaSalvare As String = ElaboraXML_Appezzamenti_SistemaXML(
                        objOpzioniImportImpresa,
                        objOpzioni,
                        Log_Import,
                        Log_Errori,
                        Log_Riepilogo,
                        "<DatiAppezzamenti>" & DatiAppezzamento & "</DatiAppezzamenti>",
                        Piva_Destinazione,
                        Sa_cod_Destinazione,
                        appezzamento_Destinazione,
                        enum_TipoOperazioneDB.Modifica)

                    If objOpzioni.isGias2Gias_local Then
                        objAppezzamenti_W.Appezzamento_Scrivi(sStringaDaSalvare,
                                                              OutputPiva, OutputSaCod, OutputAppezza,
                                                              objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                                              objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE)
                    Else
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                        OutputAppezza = wsImportazione.Scrivi_AppezzamentoXmlPrivato(Str_Credenziali_WS,
                                                                                     sStringaDaSalvare)

                        If OutputAppezza = -1 Then
                            Throw New Exception("Errore WS in Scrivi_AppezzamentoXmlPrivato")
                        End If
                    End If

                    'Aggiorno la data_modifica dell'appezzamento
                    AppezzamentiMappatiEDIT(objApp)

                    conteggioAppezzamentiModificati += 1
                Next

            End If

            'log appezzamenti
            If conteggioAppezzamentiModificati > 0 Then
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " ha " & conteggioAppezzamentiModificati & " appezzamenti modificati")
            Else
                AgronicaCoreG2GLocalDal.G2GUtility.Log(Log_Import, "L'impresa " & Piva_Origine & " non ha appezzamenti modificati")
            End If

            '===============================================================
            '===================  /  APPEZZAMENTI  =========================
            '===============================================================


            '===============================================================
            '=====================    IMPIANTI    ==========================
            '===============================================================
            Dim ImpiantiR As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            Dim conteggioImpiantiModificati As Integer = 0

            Dim sXML_Impianti As String = ImpiantiR.Reg_Impianto_Leggi(Piva_Origine,
                                                                       Sa_cod_Origine,
                                                                       0,
                                                                       0,
                                                                       False,
                                                                       True,
                                                                       objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                       TipoG2G:=2)

            If sXML_Impianti <> "" Then

                xCentro.LoadXml(sXML_Impianti)

                For Each xImpianto As XmlElement In xCentro.SelectNodes("DatiReg_Impianti/Reg_Impianto")

                    OutputImpianto = -1

                    Dim fromIdReg As Integer = CInt(xImpianto.GetAttribute("id_reg"))
                    Dim fromAppezza As Integer = CInt(xImpianto.GetAttribute("appezza"))

                    Dim objImp As G2G_Recode_Impianti = recode_return_Impianto(Piva_Origine, Sa_cod_Origine,
                                                                               fromAppezza, fromIdReg,
                                                                               objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    Dim impianto_cod_Destinazione As Integer = objImp.To_Id_Reg
                    Dim appezzamento_Destinazione As Integer = objImp.To_Appezza

                    Dim sStringaDaSalvareImpianto As String = ElaboraXML_Impianti_SistemaXML(
                        objOpzioni,
                        Log_Import,
                        Log_Errori,
                        Log_Riepilogo,
                        xImpianto.OuterXml,
                        Piva_Destinazione,
                        Sa_cod_Destinazione,
                        appezzamento_Destinazione,
                        impianto_cod_Destinazione,
                        enum_TipoOperazioneDB.Modifica)

                    sStringaDaSalvareImpianto = "<DatiReg_Impianti>" & sStringaDaSalvareImpianto & "</DatiReg_Impianti>"

                    If objOpzioni.isGias2Gias_local Then
                        Dim objImpianti2_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
                        objImpianti2_W.Reg_Impianto_Scrivi(sStringaDaSalvareImpianto, OutputPiva,
                                                           OutputSaCod, OutputAppezza, OutputImpianto,
                                                           "", objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    Else
                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {
                            .Timeout = _timeout_ws_Importa,
                            .Url = objOpzioni.wsimportaGiasURl
                        }

                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                        sStringaDaSalvareImpianto = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvareImpianto)

                        OutputImpianto = wsImportazione.Scrivi_Reg_ImpiantoXmlPrivato(Str_Credenziali_WS,
                                                                                      sStringaDaSalvareImpianto)

                        If OutputImpianto = -1 Then
                            Throw New Exception("Errore WS in Scrivi_Reg_ImpiantoXmlPrivato")
                        End If
                    End If

                    'Aggiorno la data_modifica dell'impianto
                    ImpiantiMappatiEDIT(objImp)

                    conteggioImpiantiModificati += 1

                Next
            End If

            ' log impianti
            If conteggioImpiantiModificati > 0 Then
                Log_Import.Append("L'impresa " & Piva_Origine & " ha " & conteggioImpiantiModificati & " impianti modificati" + vbCrLf)
            Else
                'Log_Import.Append("L'impresa " & Piva_Origine & " non ha impianti modificati " + vbCrLf)
            End If

            '===============================================================
            '===================    /  IMPIANTI    =========================
            '===============================================================

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '#####################################################################################################################################################
    Private Function ElaboraXML_Appezzamenti_LeggiImpianti(ByVal objOpzioni As clsOpzioni,
                                                    ByRef Log_Import As StringBuilder,
                                                    ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Riepilogo As StringBuilder,
                                                    ByRef Str_XML_Appezzamento As String,
                                                    ByVal Piva_Origine As String,
                                                    ByVal sa_cod_Origine As Integer,
                                                    ByVal Piva_Destinazione As String,
                                                    ByVal saCod_Destinazione As String,
                                                    ByVal TipoG2G As Integer) As String


        Const NomeFunzione As String = "ElaboraXML_Appezzamenti_LeggiImpianti"


        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim xCentro As New XmlDocument

        Dim xDatiAppezzamenti As XmlNodeList
        Dim xDatiAppezzamento As XmlElement
        Dim xAppezzamenti As XmlNodeList

        Dim xDatiIpiantoTmp As New XmlDocument

        Dim i_DatiAppezzamento As Integer = 0
        Try

            Dim outupdDoc As New XmlDocument
            outupdDoc.LoadXml("<DatiAppezzamenti></DatiAppezzamenti>")


            xCentro.LoadXml(Str_XML_Appezzamento)

            xDatiAppezzamenti = xCentro.GetElementsByTagName("DatiAppezzamenti")



            i_DatiAppezzamento = 0


            Dim impiantoRead As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            Do While i_DatiAppezzamento < xDatiAppezzamenti.Count

                'Prelevo l'i-esimo blocco di DatiAppezzamenti (in realta' ne esiste uno solo)
                xDatiAppezzamento = xDatiAppezzamenti.Item(i_DatiAppezzamento)

                '------------------------------

                xAppezzamenti = xDatiAppezzamento.GetElementsByTagName("Appezzamento")
                Dim appezza As Integer
                Dim xmlDatiImpianto As String = ""
                Dim xmlDocDatiAppezza As New XmlDocument


                For Each singleAppezza As XmlElement In xAppezzamenti

                    appezza = singleAppezza.Attributes("appezza").Value

                    'carico gli impianti
                    '  Vanni, 05/08/2014 09:57:32: modifica per gestione impianti con verifica su G2G_recode_impianti
                    xmlDatiImpianto = impiantoRead.Reg_Impianto_Leggi(Piva_Origine,
                                                                      sa_cod_Origine,
                                                                      appezza,
                                                                      0,
                                                                      False,
                                                                      True,
                                                                      objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                                      TipoG2G:=TipoG2G)

                    xDatiIpiantoTmp.LoadXml(xmlDatiImpianto)
                    'Dim xelem As XmlElement
                    'xelem = xCentro.FirstChild
                    singleAppezza.AppendChild(xCentro.ImportNode(xDatiIpiantoTmp.FirstChild, True))


                    outupdDoc.FirstChild.AppendChild(outupdDoc.ImportNode(singleAppezza, True))

                Next

                i_DatiAppezzamento += 1
            Loop

            Return outupdDoc.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Function



    '#####################################################################################################################################################
    Private Function ElaboraXML_Campi_SistemaXML(ByRef objOpzioni As clsOpzioni,
                                                 ByRef Log_Import As StringBuilder,
                                                 ByRef Log_Errori As StringBuilder,
                                                 ByRef Log_Riepilogo As StringBuilder,
                                                 ByRef Str_XML_Campo As String,
                                                 ByVal Piva_Destinazione As String,
                                                 ByVal saCod_Destinazione As String,
                                                 ByVal campoCod_Destinazione As String,
                                                 ByVal tipoOperazioneDB As enum_TipoOperazioneDB
                                                 ) As String

        Const nomeFunzione = "ElaboraXMLCampi_SistemaXML"

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
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@campo_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("campo_cod", campoCod_Destinazione)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function



    '#####################################################################################################################################################
    Private Function ElaboraXML_Appezzamenti_SistemaXML_OLD(
                                                        ByVal objOpzioni As clsOpzioni,
                                                        ByRef Log_Import As StringBuilder,
                                                        ByRef Log_Errori As StringBuilder,
                                                        ByRef Log_Riepilogo As StringBuilder,
                                                        ByRef objParametri_Server_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti_GIAS_Origine As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Server_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti_GIAS_Destinazione As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef Str_XML_Appezzamento As String,
                                                        ByVal Piva_Destinazione As String,
                                                        ByVal saCod_Destinazione As String
                                                        ) As String

        Const NomeFunzione As String = "ElaboraXML_Appezzamenti_SistemaXML"


        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)

            Next

            'azzeramento appezza
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@appezza]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("appezza", 0)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

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

    Private Function ElaboraXML_Appezzamenti_SistemaXML(ByVal objOpzioniImportImpresa As clsImpresa,
                                                        ByRef objOpzioni As clsOpzioni,
                                                        ByRef Log_Import As StringBuilder,
                                                        ByRef Log_Errori As StringBuilder,
                                                        ByRef Log_Riepilogo As StringBuilder,
                                                        ByRef Str_XML_Appezzamento As String,
                                                        ByVal Piva_Destinazione As String,
                                                        ByVal saCod_Destinazione As String,
                                                        ByVal appezzamento_Destinazione As String,
                                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB
                                                        ) As String

        Const nomeFunzione = "ElaboraXML_Appezzamenti_SistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'Se l'appezzamento apparteneva ad un campo, rimappo con il nuovo "Campo_Cod"
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@campo_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                If XML_Nodo.Attributes("campo_cod").Value <> "0" Then

                    Dim fromPiva As String = XML_Nodo.Attributes("piva").Value
                    Dim fromSaCod As Integer = CInt(XML_Nodo.Attributes("sa_cod").Value)
                    Dim fromCampoCod As Integer = CInt(XML_Nodo.Attributes("campo_cod").Value)

                    'Cerco il nuovo valore
                    Dim objCampo As G2G_Recode_Campo = recode_return_Campo(fromPiva, fromSaCod, fromCampoCod,
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    If objCampo Is Nothing Then
                        Throw New Exception("Campo non trovato in Recode")
                    End If

                    'Assegno il nuovo valore
                    XML_Nodo.SetAttribute("campo_cod", objCampo.To_Campo_cod)

                End If

            Next


            'Tipo Operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
            Next


            'Azzeramento "appezza"
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@appezza]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il campo "appezza" che verrà ri-assegnato dal core
                XML_Nodo.SetAttribute("appezza", appezzamento_Destinazione)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next


            'Verifica se deve inviare il catasto oppure no.           

            If Not objOpzioniImportImpresa.Flagimporta_catasto Then
                Dim xListaNodoAppezzamento As XmlNodeList = XmlDocCont.SelectNodes("//DatiAppezzamenti/Appezzamento")
                For Each nodoAppezzamento As XmlNode In xListaNodoAppezzamento
                    Dim xNodoParticelle As XmlNode = nodoAppezzamento.SelectSingleNode("//DatiParticelle")
                    If Not xNodoParticelle Is Nothing Then
                        nodoAppezzamento.RemoveChild(xNodoParticelle)
                    End If
                Next
            End If

            ' fix x inserimento indirizzi appezzamento
            Dim xListaIndirizziAppezzamento As XmlNodeList = XmlDocCont.SelectNodes("//DatiAppezzamenti/Appezzamento/Indirizzi")
            For Each nodoIndirizzo As XmlNode In xListaIndirizziAppezzamento
                Dim xNodoIndirizzo As XmlElement = nodoIndirizzo.SelectSingleNode("//Indirizzo")
                If Not xNodoIndirizzo Is Nothing AndAlso TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                    xNodoIndirizzo.SetAttribute("cod_indirizzo", 0)
                End If
            Next

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function



    '#####################################################################################################################################################
    Private Function ElaboraXML_Impianti_SistemaXML_OLD(
                                                    ByVal objOpzioni As clsOpzioni,
                                                    ByRef Log_Import As StringBuilder,
                                                    ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Riepilogo As StringBuilder,
                                                    ByRef Str_XML_Appezzamento As String,
                                                    ByVal Piva_Destinazione As String,
                                                    ByVal saCod_Destinazione As String
                                                    ) As String

        Const NomeFunzione As String = "ElaboraXML_Impianti_SistemaXML"


        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", enum_TipoOperazioneDB.Scrittura)

            Next

            'azzeramento
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_reg]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("appezza", 0)
                XML_Nodo.SetAttribute("id_reg", 0)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

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

    Private Function ElaboraXML_Impianti_SistemaXML(ByVal objOpzioni As clsOpzioni,
                                                    ByRef Log_Import As StringBuilder,
                                                    ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Riepilogo As StringBuilder,
                                                    ByRef Str_XML_Appezzamento As String,
                                                    ByVal Piva_Destinazione As String,
                                                    ByVal saCod_Destinazione As String,
                                                    ByVal appezza_Destinazione As String,
                                                    ByVal idReg_Destinazione As String,
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB
                                                    ) As String

        Const nomeFunzione = "ElaboraXML_Impianti_SistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_Appezzamento)

        Dim Str_XML_Output As String = ""

        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Try

            'Se l'impianto apparteneva ad un campo, rimappo con il nuovo "Campo_Cod"
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_campo]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                If XML_Nodo.Attributes("id_campo").Value <> "0" Then

                    Dim fromPiva As String = XML_Nodo.Attributes("piva").Value
                    Dim fromSaCod As Integer = CInt(XML_Nodo.Attributes("sa_cod").Value)
                    Dim fromCampoCod As Integer = CInt(XML_Nodo.Attributes("id_campo").Value)

                    'Cerco il nuovo valore
                    Dim objCampo As G2G_Recode_Campo = recode_return_Campo(fromPiva, fromSaCod, fromCampoCod,
                                                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser)

                    If objCampo Is Nothing Then
                        Throw New Exception("Campo non trovato in Recode")
                    End If

                    'Assegno il nuovo valore
                    XML_Nodo.SetAttribute("id_campo", objCampo.To_Campo_cod)

                End If

            Next


            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi(i)
                XML_Nodo.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
            Next

            'azzeramento
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_reg]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("appezza", appezza_Destinazione)
                XML_Nodo.SetAttribute("id_reg", idReg_Destinazione)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", saCod_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)
            Next

            'sistemazione codice 1298 che contiene le pratiche
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_cod=" & enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod & "]")
            If XMLs_Nodi.Count > 0 Then

                Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objOpzioni.objParametri_Server_GIAS_ORIGINE.StringaConnessione)
                Dim GiasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
                Dim prat As New AgronicaCoreG2GLocalDal.G2GPratiche_R

                For Each XML_Nodo In XMLs_Nodi
                    Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, XML_Nodo.GetAttribute("val_cod"))
                    If Not String.IsNullOrEmpty(newStrPratiche) Then
                        XML_Nodo.SetAttribute("val_cod", newStrPratiche)
                    End If
                Next

            End If

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output

    End Function

    Public Function recode_return_Campo(ByVal oldPiva As String,
                                        ByVal oldSa_Cod As Integer,
                                        ByVal oldCampo_Cod As Integer,
                                        ByVal piva_SuperUser_destinazione As String
                                        ) As G2G_Recode_Campo
        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Campo
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Campo_cod = oldCampo_Cod AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _Campi
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Campo_cod = oldCampo_Cod AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

    Public Function recode_return_Appezzamento(ByVal oldPiva As String,
                                               ByVal oldSa_Cod As Integer,
                                               ByVal oldAppezza As Integer,
                                               ByVal piva_SuperUser_destinazione As String
                                               ) As G2G_Recode_Appezzamenti

        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Appezzamenti
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Appezza = oldAppezza AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _AppezzamentiMappati
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Appezza = oldAppezza AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

    Public Function recode_return_Impianto(ByVal oldPiva As String,
                                           ByVal oldSa_Cod As Integer,
                                           ByVal oldAppezza As Integer,
                                           ByVal oldId_Reg As Integer,
                                           ByVal piva_SuperUser_destinazione As String
                                           ) As G2G_Recode_Impianti

        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Impianti
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Appezza = oldAppezza AndAlso
                          x.From_Id_Reg = oldId_Reg AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _ImpiantiMappati
                    Where x.From_Piva = oldPiva AndAlso
                          x.From_Sa_Cod = oldSa_Cod AndAlso
                          x.From_Appezza = oldAppezza AndAlso
                          x.From_Id_Reg = oldId_Reg AndAlso
                          x.To_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

    Public Function recode_return_ImpiantoReverse(ByVal oldPiva As String,
                                           ByVal oldSa_Cod As Integer,
                                           ByVal oldAppezza As Integer,
                                           ByVal oldId_Reg As Integer,
                                           ByVal piva_SuperUser_destinazione As String
                                           ) As G2G_Recode_Impianti

        If NuovaLogicaRecodePC Then
            Return (From x In _efG2G.G2G_Recode_Impianti
                    Where x.From_Piva = oldPiva AndAlso
                          x.To_Sa_Cod = oldSa_Cod AndAlso
                          x.To_Appezza = oldAppezza AndAlso
                          x.To_Id_Reg = oldId_Reg AndAlso
                          x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        Else
            Return (From x In _ImpiantiMappati
                    Where x.From_Piva = oldPiva AndAlso
                          x.To_Sa_Cod = oldSa_Cod AndAlso
                          x.To_Appezza = oldAppezza AndAlso
                          x.To_Id_Reg = oldId_Reg AndAlso
                          x.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select x).FirstOrDefault
        End If

    End Function

#End Region

End Class
