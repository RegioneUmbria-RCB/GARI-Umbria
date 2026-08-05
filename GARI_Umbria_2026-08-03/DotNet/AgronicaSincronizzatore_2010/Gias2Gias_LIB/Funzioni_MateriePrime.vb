Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreUtility
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni


#Region "Materie Prime"

    Public Function LeggiMateriePrimeXML(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseMateriePrime As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        Dim listaImprese = New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GMateriePrime_R
        Dim g2g = objLeggi.Nuovo_MateriePrime_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)

        If objLeggi.Leggi_MateriePrime_G2G(From_Piva, Flag_Pubblico, listaImprese, dataValidita, g2g, objParametri) Then

            ' se materie prime pubbliche restituisco la lista delle imprese relative
            If Flag_Pubblico Then
                listaImpreseMateriePrime = (From m In g2g.MateriePrimeToInsert Select m.Piva).Distinct().ToList()
            End If

            Return XMLUtility.SerializzaOggetto(Of G2G_MateriePrime)(g2g, "")

        End If

        Return ""

    End Function

    Public Function LeggiMateriePrimeXMLReverse(ByVal Piva_Origine As String,
                                     ByVal Piva_Destinazione As String,
                                     ByRef objOpzioni As clsOpzioni,
                                     ByVal Flag_Pubblico As Boolean,
                                     ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef listaImpreseMateriePrime As List(Of String)) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        Dim listaImprese = New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GMateriePrime_R
        Dim g2g = objLeggi.Nuovo_MateriePrime_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)

        If objLeggi.Leggi_MateriePrime_G2GReverse(From_Piva, Flag_Pubblico, listaImprese, dataValidita, g2g, objParametri) Then

            ' se materie prime pubbliche restituisco la lista delle imprese relative
            If Flag_Pubblico Then
                listaImpreseMateriePrime = (From m In g2g.MateriePrimeToInsert Select m.Piva).Distinct().ToList()
            End If

            Return XMLUtility.SerializzaOggetto(Of G2G_MateriePrime_Reverse)(g2g, "")

        End If

        Return ""

    End Function

    Public Sub Elabora_XML_MateriePrime_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime)

        Const NomeFunzione As String = "Elabora_XML_MateriePrime_Salva"
        Dim MessaggioErrore As String = ""
        Dim listaImpreseMateriePrime As New List(Of String)

        Try

            ' leggo struttura dati serializzata su origine contenente le materie prime da inserire/modificare/cancellare 
            Dim sXmlMateriePrime = LeggiMateriePrimeXML(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseMateriePrime)

            If Not String.IsNullOrEmpty(sXmlMateriePrime) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                ' creo l'azienda della materia prima su destinazione se non esiste
                If Flag_Pubblico AndAlso listaImpreseMateriePrime.Count > 0 Then
                    For Each piva In listaImpreseMateriePrime
                        If Not Configurazione.listaImprese.Contains(piva) Then
                            Dim strErr As String = ""
                            If Not wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr) Then
                                If NuovaLogicaRecode Then
                                    ScriviImpreseXML(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
                                    )
                                Else
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
                        End If
                    Next
                End If

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlMateriePrime, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiMateriePrime")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiMateriePrime")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento materie prime: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub


    Public Sub Elabora_XML_MateriePrime_SalvaReverse(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Materie_Prime)

        Const NomeFunzione As String = "Elabora_XML_MateriePrime_Salva"
        Dim MessaggioErrore As String = ""
        Dim listaImpreseMateriePrime As New List(Of String)

        Try

            ' leggo struttura dati serializzata su origine contenente le materie prime da inserire/modificare/cancellare 
            Dim sXmlMateriePrime = LeggiMateriePrimeXMLReverse(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, Configurazione, objOpzioni.objParametri_Server_GIAS_ORIGINE, listaImpreseMateriePrime)

            If Not String.IsNullOrEmpty(sXmlMateriePrime) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                ' creo l'azienza della materia prima su destinazione se non esiste
                If Flag_Pubblico AndAlso listaImpreseMateriePrime.Count > 0 Then
                    For Each piva In listaImpreseMateriePrime
                        If Not Configurazione.listaImprese.Contains(piva) Then
                            Dim strErr As String = ""
                            If Not wsimportazione.Verifica_EsistenzaPivaGIAS(Str_Credenziali_WS, piva, strErr) Then
                                If NuovaLogicaRecode Then
                                    ScriviImpreseXML_Reverse(
                                        objOpzioni,
                                        Log_Import,
                                        Log_Errori,
                                        Log_Riepilogo,
                                        piva,
                                        piva,
                                        objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
                                    )
                                End If
                            End If
                        End If
                    Next
                End If

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlMateriePrime, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiMateriePrime")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                ' leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaContatti As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiMateriePrime")
                Dim rispostaContatti = xmlRispostaContatti.FirstNode.ToString
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaContatti, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento materie prime ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento materie prime: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    '###########################################################################################################
    'NUOVA VERSIONE CHE SOSTITUISCE QUELLA DI VANNI
    Public Sub Elabora_XML_Imprese_MateriePrime_Salva_NEW( _
                               ByVal objOpzioni As clsOpzioni, _
                               ByRef Log_Import As StringBuilder, _
                               ByRef Log_Errori As StringBuilder, _
                               ByRef Log_Riepilogo As StringBuilder, _
                               ByVal Piva_Origine As String, _
                               ByVal Piva_Destinazione As String, _
                               ByVal DT_MP As DataTable)

        Const nomeFunzione As String = "Elabora_XML_Imprese_MateriePrime_Salva_NEW"
        'Dim xMarcareInviato As New AgronicaCoreAnagrafeDAL.Materie_Prime_W

        Try

            Dim sXmlMateriePrime As String = ""
            Dim LeggiMateriePrime As New AgronicaCoreAnagrafeBIZ.Materie_Prime_R
            Dim ScriviMateriaPrima As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
            Dim Mat_Cod_ORIGINE As Integer

            'per ogni materia prima
            For Each DrMP In DT_MP.Rows

                Mat_Cod_ORIGINE = CType(DrMP("Mat_Cod"), Integer)

                sXmlMateriePrime = LeggiMateriePrime.Materia_Prima_Leggi( _
                                                    Piva_Origine, _
                                                    Mat_Cod_ORIGINE, _
                                                    False, _
                                                    False, _
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE, _
                                                    TipoG2G:=1, _
                                                    pivaDestinazione:=Piva_Destinazione
                                                    )

                If sXmlMateriePrime <> "" Then

                    Dim oMateriePrime As New XmlDocument

                    oMateriePrime.LoadXml(sXmlMateriePrime)

                    Dim outputMatCod As Integer
                    Dim inputMatCod As Integer
                    Dim inputSa_cod As Integer
                    Dim inputElem_cod As Integer
                    Dim inputCod_Articolo As String

                    For Each xSingolaMateriaPrima As XmlNode In oMateriePrime.SelectNodes("//Materia_Prima")

                        inputMatCod = CInt(xSingolaMateriaPrima.Attributes("mat_cod").Value)
                        inputElem_cod = CInt(xSingolaMateriaPrima.Attributes("elem_cod").Value)
                        inputSa_cod = CInt(xSingolaMateriaPrima.Attributes("sa_cod").Value)
                        inputCod_Articolo = xSingolaMateriaPrima.Attributes("cod_articolo").Value

                        'GRilli 29/05/2017: secondo Fabrizio quelle pubbliche devono diventare private...
                        If xSingolaMateriaPrima.Attributes("sa_cod").Value = "-1" Then
                            xSingolaMateriaPrima.Attributes("sa_cod").Value = "0"
                        End If

                        Dim sStringaDaSalvare As String
                        sStringaDaSalvare = _
                            "<DatiMaterie_Prime>" & _
                            Elabora_XML_Imprese_MateriePrime_Sistemaxml(
                                        objOpzioni, _
                                        Log_Import, _
                                        Log_Errori, _
                                        Log_Riepilogo, _
                                        xSingolaMateriaPrima.OuterXml, _
                                        Piva_Origine, _
                                        Piva_Destinazione _
                                    ) & _
                            "</DatiMaterie_Prime>"


                        If objOpzioni.isGias2Gias_local Then
                            ScriviMateriaPrima.Materia_Prima_Scrivi( _
                                sStringaDaSalvare, _
                                outputMatCod, _
                                AGRODATAINIZIO, _
                                AGRODATAFINE, _
                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
                            )
                        Else

                            Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                            wsimportazione.Url = objOpzioni.wsimportaGiasURl
                            wsimportazione.Timeout = _timeout_ws_Importa
                            Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                            '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                            sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                            outputMatCod = wsimportazione.Scrivi_ImpreseMateriePrimeXmlPrivato( _
                                 Str_Credenziali_WS, _
                                 sStringaDaSalvare)
                        End If

                        If NuovaLogicaRecodeAG Then
                            Dim recode As New G2G_Recode_MateriePrime With {
                                .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                .To_PivaSuperUser = objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.PivaSuperUser,
                                .From_Piva = DrMP("Piva"),
                                .To_Piva = Piva_Destinazione,
                                .From_Mat_Cod = inputMatCod,
                                .To_Mat_Cod = outputMatCod,
                                .Username_Creazione = _objP_Server.UtenteCodFiscale,
                                .Username_Modifica = _objP_Server.UtenteCodFiscale,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                            _efG2G.G2G_Recode_MateriePrime.Add(recode)
                            _efG2G.SaveChanges()
                        Else
                            FunzioniGLOBAL.MaterieADD(
                             New G2G_Recode_MateriePrime With {
                                 .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                 .To_PivaSuperUser = objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE.PivaSuperUser,
                                 .From_Piva = DrMP("Piva"),
                                 .To_Piva = Piva_Destinazione,
                                 .From_Mat_Cod = inputMatCod,
                                 .To_Mat_Cod = outputMatCod
                             })
                        End If

                    Next

                    'xMarcareInviato.MateriePrime_MarcaComeInviato(Piva_Origine, 0, Mat_Cod_ORIGINE, Now, "", objOpzioni.objParametri_Server_GIAS_ORIGINE)
                End If

            Next

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub





    ''###########################################################################################################
    ''PRIMA VERSIONE DI VANNI
    'Public Sub Elabora_XML_Imprese_MateriePrime_Salva( _
    '                            ByVal objOpzioni As clsOpzioni, _
    '                            ByRef Log_Import As StringBuilder, _
    '                            ByRef Log_Errori As StringBuilder, _
    '                            ByRef Log_Riepilogo As StringBuilder, _
    '                            ByVal Piva_Origine As String, _
    '                            ByVal Piva_Destinazione As String _
    '                    )

    '    Const nomeFunzione As String = "Elabora_XML_Imprese_MateriePrime_Salva"

    '    Try
    '        Dim sXmlMateriePrime As String = ""
    '        Dim LeggiMateriePrime As New AgronicaCoreAnagrafeBIZ.Materie_Prime_R

    '        sXmlMateriePrime = LeggiMateriePrime.Materia_Prima_Leggi( _
    '            Piva_Origine, _
    '            0, _
    '            False, _
    '            False, _
    '            objOpzioni.objParametri_Server_GIAS_ORIGINE _
    '        )

    '        If sXmlMateriePrime <> "" Then

    '            Dim oMateriePrime As New XmlDocument
    '            oMateriePrime.LoadXml(sXmlMateriePrime)

    '            Dim ScriviMateriaPrima As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W

    '            Dim outputMatCod As Integer
    '            Dim inputMatCod As Integer
    '            Dim inputSa_cod As Integer
    '            Dim inputElem_cod As Integer
    '            Dim inputCod_Articolo As String

    '            For Each xSingolaMateriaPrima As XmlNode In oMateriePrime.SelectNodes("//Materia_Prima")

    '                inputMatCod = CInt(xSingolaMateriaPrima.Attributes("mat_cod").Value)
    '                inputElem_cod = CInt(xSingolaMateriaPrima.Attributes("elem_cod").Value)
    '                inputSa_cod = CInt(xSingolaMateriaPrima.Attributes("sa_cod").Value)
    '                inputCod_Articolo = xSingolaMateriaPrima.Attributes("cod_articolo").Value


    '                ScriviMateriaPrima.Materia_Prima_Scrivi( _
    '                    "<DatiMaterie_Prime>" & _
    '                    Elabora_XML_Imprese_MateriePrime_Sistemaxml(
    '                                objOpzioni, _
    '                                Log_Import, _
    '                                Log_Errori, _
    '                                Log_Riepilogo, _
    '                                xSingolaMateriaPrima.OuterXml, _
    '                                Piva_Origine, _
    '                                Piva_Destinazione _
    '                            ) & _
    '                    "</DatiMaterie_Prime>", _
    '                    outputMatCod, _
    '                    AGRODATAINIZIO, _
    '                    AGRODATAFINE, _
    '                    objOpzioni.objParametri_Server_GIAS_DESTINAZIONE _
    '                )

    '                'FunzioniGLOBAL.Materie.Add( _
    '                '    New recode_MateriePrime With { _
    '                '        .From_Piva = Piva_Origine, _
    '                '        .From_Sa_Cod = inputSa_cod, _
    '                '        .From_Elem_Cod = inputElem_cod, _
    '                '        .From_Mat_Cod = inputMatCod, _
    '                '        .From_Cod_Articolo = inputCod_Articolo, _
    '                '        .To_Piva = Piva_Destinazione, _
    '                '        .To_Sa_Cod = inputSa_cod, _
    '                '        .To_Elem_Cod = inputElem_cod, _
    '                '        .To_Mat_Cod = outputMatCod, _
    '                '        .To_Cod_Articolo = inputCod_Articolo _
    '                '    })

    '                FunzioniGLOBAL.Materie.Add( _
    '                  New recode_MateriePrime With { _
    '                      .From_Mat_Cod = inputMatCod, _
    '                      .To_Mat_Cod = outputMatCod _
    '                  })


    '            Next


    '        End If
    '    Catch ex As Exception

    '        Dim msg As String
    '        msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
    '        Log_Import.Append(msg)
    '        Log_Errori.Append(msg)
    '        Throw New Exception(msg)
    '    End Try
    'End Sub


    Public Function Elabora_XML_Imprese_MateriePrime_Sistemaxml( _
                    ByVal objOpzioni As clsOpzioni, _
                    ByRef Log_Import As StringBuilder, _
                    ByRef Log_Errori As StringBuilder, _
                    ByRef Log_Riepilogo As StringBuilder, _
                    ByRef Str_XML_MateriePrime As String, _
                    ByVal Piva_Origine As String, _
                    ByVal Piva_Destinazione As String _
                ) As String

        Const NomeFunzione As String = "ElaboraXML_Grafica_SistemaXML"

        Dim XmlDocCont As New XmlDocument
        XmlDocCont.LoadXml(Str_XML_MateriePrime)

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

            'azzeramento fabbricato_cod
            XMLs_Nodi = XmlDocCont.SelectNodes("//Materia_Prima")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                XML_Nodo.SetAttribute("mat_cod", 0)
                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next



            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output


    End Function

    'Private Function recode_dammi_MatCod(ByVal old_piva As String, ByVal old_sa_cod As Integer, ByVal nodo_mat_cod As XmlNode) As Integer
    Public Function recode_return_MatCod(ByVal nodo_mat_cod As XmlNode,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String,
                                         Optional ByVal flag_import As Boolean = True
                                         ) As Integer

        Dim old_Mat_cod As Integer = nodo_mat_cod.Attributes("mat_cod").Value
        Dim elem_cod As Integer = nodo_mat_cod.Attributes("elem_cod").Value

        Return recode_return_MatCod(old_Mat_cod, elem_cod, piva_destinazione, piva_SuperUser_destinazione, flag_import)

    End Function

    Public Function recode_return_MatCodReverse(ByVal nodo_mat_cod As XmlNode,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String,
                                         Optional ByVal flag_import As Boolean = True
                                         ) As Integer

        Dim old_Mat_cod As Integer = nodo_mat_cod.Attributes("mat_cod").Value
        Dim elem_cod As Integer = nodo_mat_cod.Attributes("elem_cod").Value

        Return recode_return_MatCodReverse(old_Mat_cod, elem_cod, piva_destinazione, piva_SuperUser_destinazione, flag_import)

    End Function

    Public Function recode_return_MatCod(ByVal Old_Mat_Cod As Integer,
                                         ByVal elem_cod As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String,
                                         Optional ByVal flag_import As Boolean = True
                                         ) As Integer

        Dim rval As Integer = 0

        If Old_Mat_Cod <> 0 Then
            'devo eseguire il controllo solo se si tratta di un mat_cod valorizzato
            'altrimenti è un pro_cod

            Select Case elem_cod

                Case 0

                    'mat_cod è un cod_risum
                    If _efG2G Is Nothing Then
                        rval = (From rs In FunzioniGLOBAL.Contatti
                                Where rs.From_Cod_RisUm = Old_Mat_Cod AndAlso
                                      rs.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select rs.To_Cod_Risum).FirstOrDefault
                    Else
                        rval = (From rs In _efG2G.G2G_Recode_Contatti
                                Where rs.From_Cod_RisUm = Old_Mat_Cod AndAlso
                                      rs.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select rs.To_Cod_Risum).FirstOrDefault
                    End If

                Case 1

                    'mat_cod è un codice macchina mac_cod
                    If _efG2G Is Nothing Then
                        rval = (From mac In FunzioniGLOBAL.ParcoMacchine
                                Where mac.From_Mac_Cod = Old_Mat_Cod AndAlso
                                      mac.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select mac.To_Mac_Cod).FirstOrDefault
                    Else
                        rval = (From mac In _efG2G.G2G_Recode_Parco_Macchine
                                Where mac.From_Mac_Cod = Old_Mat_Cod AndAlso
                                      mac.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select mac.To_Mac_Cod).FirstOrDefault
                    End If

                Case Else

                    'mat_cod è un codice di materia prima
                    If _efG2G Is Nothing Then
                        rval = (From mat In FunzioniGLOBAL.Materie
                                Where mat.From_Mat_Cod = Old_Mat_Cod AndAlso
                                       mat.To_Piva = piva_destinazione AndAlso
                                       mat.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select mat.To_Mat_Cod).FirstOrDefault
                    Else
                        rval = (From mat In _efG2G.G2G_Recode_MateriePrime
                                Join mp In _efG2G.Materie_Prime
                                 On mat.From_Mat_Cod Equals mp.Mat_Cod And
                                    mat.To_Piva Equals If(flag_import, mp.Piva, piva_destinazione)
                                Where mat.From_Mat_Cod = Old_Mat_Cod AndAlso
                                       mat.To_PivaSuperUser = piva_SuperUser_destinazione
                                Select mat.To_Mat_Cod).FirstOrDefault
                    End If

            End Select

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "Elem_Cod = " & CStr(elem_cod) & " Mat_Cod ORIGINE = " & CStr(Old_Mat_Cod) &
                            " Errore, non esiste una decodifica del mat_cod in movimenti_dettagli (materia prima, macchina, contatto)."

                Throw New Exception(messaggio)
            End If

        End If

        Return rval

    End Function

    Public Function recode_return_MatCodReverse(ByVal Old_Mat_Cod As Integer,
                                         ByVal elem_cod As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String,
                                         Optional ByVal flag_import As Boolean = True
                                         ) As Integer

        Dim rval As Integer = 0

        If Old_Mat_Cod <> 0 Then
            'devo eseguire il controllo solo se si tratta di un mat_cod valorizzato
            'altrimenti è un pro_cod

            Select Case elem_cod

                Case 0

                    'mat_cod è un cod_risum
                    If _efG2G Is Nothing Then
                        rval = (From rs In FunzioniGLOBAL.Contatti
                                Where rs.To_Cod_Risum = Old_Mat_Cod AndAlso
                                      rs.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select rs.From_Cod_RisUm).FirstOrDefault
                    Else
                        rval = (From rs In _efG2G.G2G_Recode_Contatti
                                Where rs.To_Cod_Risum = Old_Mat_Cod AndAlso
                                      rs.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select rs.From_Cod_RisUm).FirstOrDefault
                    End If

                Case 1

                    'mat_cod è un codice macchina mac_cod
                    If _efG2G Is Nothing Then
                        rval = (From mac In FunzioniGLOBAL.ParcoMacchine
                                Where mac.To_Mac_Cod = Old_Mat_Cod AndAlso
                                      mac.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select mac.From_Mac_Cod).FirstOrDefault
                    Else
                        rval = (From mac In _efG2G.G2G_Recode_Parco_Macchine
                                Where mac.To_Mac_Cod = Old_Mat_Cod AndAlso
                                      mac.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select mac.From_Mac_Cod).FirstOrDefault
                    End If

                Case Else

                    'mat_cod è un codice di materia prima
                    If _efG2G Is Nothing Then
                        rval = (From mat In FunzioniGLOBAL.Materie
                                Where mat.To_Mat_Cod = Old_Mat_Cod AndAlso
                                       mat.To_Piva = piva_destinazione AndAlso
                                       mat.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select mat.From_Mat_Cod).FirstOrDefault
                    Else
                        rval = (From mat In _efG2G.G2G_Recode_MateriePrime
                                Join mp In _efG2G.Materie_Prime
                                 On mat.To_Mat_Cod Equals mp.Mat_Cod And
                                    mat.To_Piva Equals If(flag_import, mp.Piva, piva_destinazione)
                                Where mat.To_Mat_Cod = Old_Mat_Cod AndAlso
                                       mat.From_PivaSuperUser = piva_SuperUser_destinazione
                                Select mat.From_Mat_Cod).FirstOrDefault
                    End If

            End Select

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "Elem_Cod = " & CStr(elem_cod) & " Mat_Cod ORIGINE = " & CStr(Old_Mat_Cod) &
                            " Errore, non esiste una decodifica del mat_cod in movimenti_dettagli (materia prima, macchina, contatto)."

                Throw New Exception(messaggio)
            End If

        End If

        Return rval

    End Function

    Public Function recode_return_CalCod(ByVal old_cal_cod As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim rval As Integer = 0

        If old_cal_cod < 0 Then

            rval = (From rs In _efG2G.G2G_Recode_Materie_Prime_Campionature
                Where rs.From_Progressivo = old_cal_cod AndAlso
                      rs.To_PivaSuperUser = piva_SuperUser_destinazione
                Select rs.To_Progressivo).FirstOrDefault

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "cal_cod ORIGINE = " & CStr(old_cal_cod) &
                            "Errore, non esiste una decodifica del cul_cod in movimenti_dettagli (materie_prime_campionature)."

                Throw New Exception(messaggio)
            End If

        Else

            rval = old_cal_cod

        End If

        Return rval

    End Function

    Public Function recode_return_CalCodReverse(ByVal old_cal_cod As Integer,
                                         ByVal piva_destinazione As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim rval As Integer = 0

        If old_cal_cod < 0 Then

            rval = (From rs In _efG2G.G2G_Recode_Materie_Prime_Campionature
                    Where rs.To_Progressivo = old_cal_cod AndAlso
                          rs.From_PivaSuperUser = piva_SuperUser_destinazione
                    Select rs.From_Progressivo).FirstOrDefault

            If rval = 0 Then
                Dim messaggio As String
                messaggio = "cal_cod ORIGINE = " & CStr(old_cal_cod) &
                            "Errore, non esiste una decodifica del cul_cod in movimenti_dettagli (materie_prime_campionature)."

                Throw New Exception(messaggio)
            End If

        Else

            rval = old_cal_cod

        End If

        Return rval

    End Function

#End Region

End Class
