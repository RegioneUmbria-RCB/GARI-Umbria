Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ImportatoreGIS

    Public Function Importa_DatiGIS(
        ByVal Piva As String,
        ByVal sa_Cod As String,
        ByVal ListaFile As List(Of String),
        ByVal programmazione_Cod As Integer,
        ByVal GeoRiferimento_Cod As Integer,
        ByVal AnnoDiriferimento As Integer,
        ByVal Flag_ImportaAnagrafica As Boolean,
        ByVal Flag_ImportaPianoColturale As Boolean,
        ByVal StringaConnessione As String,
        ByVal Utente_Username As String,
        ByVal Utente_Password As String,
        ByVal ProgressivoGIAS As Integer,
        ByVal CodiceChiaveCliente As Integer,
        ByVal LinkWSImportaGIAS As String,
        ByVal LogDirectory As String,
        ByVal LogFileName As String,
        ByRef Messaggio As String,
        ByRef LogCodificheMancantiSpecie As String,
        ByRef LogCodificheMancantiVarieta As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri,
        ByRef Piva_Padre As String
    ) As Boolean


        Dim NomeRoutine As String = "Importatore_Agea_BIZ.Importa_DatiGIS"

        Dim geoJsonctrl As New AgronicaSHPWrapper.GeoJsonController(Of AgronicaCoreModello.GeoJson_EmptyProperties)

        Dim Flag_Risultato As Boolean = False

        'variabili per generazione xml ws_importa_gias
        Dim objXML As New AgronicaCoreXML.AnagrafeXML
        Dim XmlDoc As New XmlDocument
        Dim XmlDocP As New XmlDocument


        Dim XmlUtente As XmlElement
        Dim XmlUtenteP As System.Xml.XmlElement
        Dim XmlTestata As XmlElement
        Dim XmlEntita As XmlElement

        Dim strAnno As String = AnnoDiriferimento.ToString

        Dim errore As Boolean = False
        'fine variabili per generazione xml ws_importa_gias

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        '-------------------------------
        '----- UTENTE
        '-------------------------------
        XmlUtente = objXML.Xml_Pubblico_Utente(
                XmlDoc,
                Utente_Username,
                Utente_Password,
                ProgressivoGIAS)

        XmlDoc.AppendChild(XmlUtente)

        XmlUtenteP = objXML.Xml_Pubblico_Utente(
                XmlDocP,
                Utente_Username,
                Utente_Password,
                ProgressivoGIAS
            )


        XmlDocP.AppendChild(XmlUtenteP)

        Dim TipoOperazionePlanTestata As enum_TipoOperazioneDB

        If programmazione_Cod = 0 Then
            TipoOperazionePlanTestata = enum_TipoOperazioneDB.Scrittura
        Else
            TipoOperazionePlanTestata = enum_TipoOperazioneDB.Modifica
        End If

        Dim pianificazione_Des_Aggiuntiva As String =
            getPianificazione_Des_Aggiuntiva(ListaFile, objParametri_Server)

        Dim programmazione_des As String = "Pianificazione " & strAnno & " - " & pianificazione_Des_Aggiuntiva

        If programmazione_des.Length > 250 Then
            programmazione_des = programmazione_des.Substring(1, 250)
        End If

        XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(
                TipoOperazionePlanTestata,
                programmazione_Cod,
                Piva,
                programmazione_des,
                programmazione_des,
                programmazione_des,
                enum_Planning_Fonte.AGEA_RealTime,
                enum_TipoPianificazione.Pianificazione_Annuale,
                Validita_Inizio,
                Validita_Fine,
                XmlDocP)

        XmlUtenteP.AppendChild(XmlTestata)

        'numerazioni (per tutti i file)
        Dim i As Integer = 1
        Dim nEntita As Integer = 1

        If programmazione_Cod <> 0 Then
            Dim xLeggiEnt As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dtE As DataTable =
                xLeggiEnt.Programmazione_Entita_Leggi(programmazione_Cod, "", 0, "", Piva, sa_Cod, 0, 0, 0, 0, Validita_Inizio, Validita_Fine, -1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", " APPEZZA ", objParametri_Server)

            If dtE.Rows.Count > 0 Then
                Dim appLetto As Integer = dtE.Rows(0)("Appezza")
                If appLetto < 0 Then
                    nEntita = Math.Abs(appLetto) + 1
                End If
            End If

        End If

        'scorro su tutti i files trovati.
        For Each Path_FileSoci_XML As String In ListaFile


            'carica il file
            Dim xDoc1 As New XDocument
            xDoc1 = XDocument.Load(Path_FileSoci_XML)

            'cicla tutti i nodi xml e genera le righe di planning

            Dim soapenv As XNamespace = "http://schemas.xmlsoap.org/soap/envelope/"
            Dim tns2 As XNamespace = "WWW.BLUARANCIO.COM"

            Dim datiDaCaricare As XElement = xDoc1.Elements(soapenv + "Envelope").Elements(soapenv + "Body").Elements(tns2 + "OutputmappaAziendaString").Elements(tns2 + "data").FirstOrDefault
            Dim wmsMetaDataGeo As XElement = datiDaCaricare.Element(tns2 + "wms")
            Dim epsgProiezione As String = wmsMetaDataGeo.Element(tns2 + "srs").Value.Replace("EPSG:", "")

            Dim xLetturaSistemiRiferimento As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dtSis As DataTable = xLetturaSistemiRiferimento.Leggi(
                0,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                " Entita.CSFrom like '%" & epsgProiezione & "%' ",
                "",
                objParametri_Server
            )

            If dtSis.Rows.Count = 1 Then
                GeoRiferimento_Cod = dtSis.Rows(0)("GeoRiferimento_Cod")
            End If

            If GeoRiferimento_Cod = 0 Then
                Throw New Exception("Impossibile trovare il sistema di riferimento con EPSG: " & epsgProiezione)
            End If


            For Each isola In datiDaCaricare.Elements(tns2 + "isoleAziendali")

                Dim strIsolaGeoJson As String = isola.Element(tns2 + "geojson").Value
                Dim geoJson1 As New AgronicaCoreModello.GeoJson(Of AgronicaCoreModello.GeoJson_EmptyProperties)
                geoJson1.geoJsonCaricato = geoJsonctrl.Load(strIsolaGeoJson)

                Dim wktIsolaFromGeoJson As String = geoJsonctrl.STAsText(geoJson1.geoJsonCaricato.features.FirstOrDefault.geometry)
                'geoJson1.geoJsonCaricato.features.FirstOrDefault.geometry.STAsText()

                Dim infoIsola As XElement =
                    isola.Element(tns2 + "info")

                Dim Campo_Nome As String = ""
                Dim Codice_Isola As String = ""
                Dim xelemCodiceIsola As XElement = infoIsola.Element(tns2 + "codice")
                If xelemCodiceIsola IsNot Nothing Then
                    Codice_Isola = xelemCodiceIsola.Value.Split("/")(2)
                End If


                Dim strSuperficieIsola As String =
                    infoIsola.Element(tns2 + "superficie").Value

                Dim dblSuperficieIsola As Double =
                    strSuperficieIsola

                dblSuperficieIsola = dblSuperficieIsola / 10000

                Campo_Nome = Codice_Isola & " - Sup.: " & Math.Round(dblSuperficieIsola, 4).ToString() & " Ha"


                For Each Utilizzo In isola.Elements(tns2 + "appezzamenti")

                    Dim strAppezzamentoGeoJson As String = Utilizzo.Element(tns2 + "geojson").Value
                    Dim geoJsonAppezzamento1 As New AgronicaCoreModello.GeoJson(Of AgronicaCoreModello.GeoJson_EmptyProperties)
                    geoJsonAppezzamento1.geoJsonCaricato = geoJsonctrl.Load(strAppezzamentoGeoJson)

                    Dim wktAppezzamentoFromGeoJson As String = geoJsonctrl.STAsText(geoJsonAppezzamento1.geoJsonCaricato.features.FirstOrDefault.geometry)
                    'geoJsonAppezzamento1.geoJsonCaricato.features.FirstOrDefault.geometry.STAsText()


                    Dim infoAppezzamento As XElement =
                        Utilizzo.Element(tns2 + "info")

                    'Dim App_Nome As String =
                    '    "Macrouso: " & infoAppezzamento.Element(tns2 + "macrouso").Value & " - " &
                    '    "Codice: " & infoAppezzamento.Element(tns2 + "codice").Value & " - " &
                    '    "Occupazione: " & infoAppezzamento.Element(tns2 + "occupazione").Value

                    Dim App_Nome As String = ""
                    If Codice_Isola <> "" Then
                        App_Nome = Codice_Isola & " - "
                    End If
                    Dim xelemOccupazioneDes As XElement = infoAppezzamento.Element(tns2 + "occupazione")
                    If xelemOccupazioneDes IsNot Nothing Then
                        App_Nome = App_Nome & xelemOccupazioneDes.Value
                    Else
                        App_Nome = App_Nome & "App. " & nEntita.ToString.PadLeft(3, "0")
                    End If


                    Dim xElemMacrousoCod As XElement = infoAppezzamento.Element(tns2 + "codice")
                    Dim Macrouso_Cod As String = "000"
                    If xElemMacrousoCod IsNot Nothing Then
                        Macrouso_Cod = xElemMacrousoCod.Value
                    End If


                    'taglio la lunghezza perché il valore massimo in entita_des è 50 caratteri
                    If App_Nome.Length > 50 Then
                        App_Nome = App_Nome.Substring(1, 50)
                    End If


                    Dim vv As XElement = infoAppezzamento.Element(tns2 + "varietà")
                    If vv IsNot Nothing Then
                        App_Nome &= " - " & "Varietà: " & vv.Value
                    End If


                    Dim sup_App As Double =
                        infoAppezzamento.Element(tns2 + "destinazione").Value.Replace(" ha", "")

                    'non ho al momento indicazioni su utilizzo, uso una costante
                    Const UsoAgricoloDaDefinire As Integer = 3103

                    XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(
                        Tipo_Operazione:=enum_TipoOperazioneDB.Scrittura,
                        Codice:="0",
                        Descrizione:=App_Nome,
                        Codice_Centro:=sa_Cod,
                        Codice_Campo:="#",
                        Codice_Appezzamento:=(-nEntita).ToString(),
                        Codice_Impianto:="0",
                        Codice_Progetto:="0",
                        Descrizione_Progetto:="Lotto" & strAnno,
                        Cod_Specie_Gias:=0,
                        Cod_Finalita_Gias:=0,
                        Cod_Varieta_Gias:=0,
                        Cod_RaggruppamentoVarietale_Gias:=0,
                        Cod_Specie_Cliente:="#",
                        Cod_Varieta_Cliente:="#",
                        Cod_Finalita_Cliente:="#",
                        Cod_Macrouso:=Macrouso_Cod,
                        Codice_Id:=UsoAgricoloDaDefinire,
                        Copertura_Cod:="#",
                        Superficie:=sup_App.ToString(),
                        Resa:="#",
                        TipoZona:="",
                        Cod_Specie_Prec_Cliente:="#",
                        Cod_Specie_Prec_Gias:="#",
                        Indice_Matrici_Organiche:="#",
                        Codice_Frequenza:="#",
                        Azoto_Distribuito:="#",
                        Numero_Piante:="#",
                        Stato_Cod:="#",
                        CicloColturale:="#",
                        Data_Semina:="#",
                        Data_Raccolta:="#",
                        Validita_Inizio:=Validita_Inizio,
                        Validita_Fine:=Validita_Fine,
                        Validita_Inizio_Impianto:=Validita_Inizio,
                        XmlDoc:=XmlDocP,
                        FlagIrrigabilita:=0,
                        FlagSecondoRaccolto:=0,
                        Tra_Fila:=0,
                        Su_Fila:=0,
                        Codice_Fiscale_Tecnico:=objParametri_Server.PivaSuperUser,
                        wkt:=wktAppezzamentoFromGeoJson,
                        wkt_georiferimento_cod:=GeoRiferimento_Cod,
                        Veg_Cod_Agea:="#",
                        Cul_Cod_Agea:="#",
                        Uso_Cod_Agea:="#",
                        Occupazione_Cod_Agea:="#",
                        Destinazione_Cod_Agea:="#",
                        Qualita_Cod_Agea:="#"
                        )


                    XmlTestata.AppendChild(XmlEntita)



                    nEntita += 1
                    i += 1

                Next
                'appezzamento

            Next
            'isola

            '---- Cancella il file xml
            Try

                System.IO.File.Delete(Path_FileSoci_XML)

            Catch ex As Exception

            End Try



        Next
        'prossimo file

        Dim strXmlAnagrafe As String = "" 'XmlDoc.OuterXml
        Dim strXmlPianificazione As String = XmlDocP.OuterXml

        Try

            If Not errore Then

                Importa_Dati(strXmlAnagrafe, strXmlPianificazione,
                                    Utente_Username, Utente_Password,
                                    CodiceChiaveCliente, LinkWSImportaGIAS,
                                    Messaggio,
                                    objParametri_Server,
                                    objParametri_Utenti)

                Flag_Risultato = True

            End If

        Catch ex As Exception
            Messaggio = "Errore nella scrittura dei dati su db: " & ex.Message
            objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                            Messaggio,
                            CustomLOGParams:=customLOGParams)
            Flag_Risultato = False
        End Try


        Return Flag_Risultato

    End Function

    Private Function getPianificazione_Des_Aggiuntiva(listaFile As List(Of String), ByVal objParametri_server As AgronicaCoreParametri) As String

        Dim rval As New List(Of String)
        Dim leggiRegioni As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R

        Dim dtRegioni As DataTable = leggiRegioni.Leggi("", "", "", objParametri_server)

        For Each ffName In listaFile

            Dim arrayChiaviValori As String() = ffName.Split("-")
            For Each ccv In arrayChiaviValori

                Dim ccv1 As String() = ccv.Split("=")
                If ccv1(0).ToLower = "codiceregione" Then

                    Dim codiceRegione As String = ccv1(1).PadLeft(3, "0")
                    Dim desRegione As String = dtRegioni.Select("REG = '" & codiceRegione & "'")(0)("Regione_Des")

                    If Not rval.Contains(desRegione) Then
                        rval.Add(desRegione)
                    End If

                End If
            Next

        Next

        Return String.Join(",", rval)

    End Function

    Private Sub Importa_Dati(ByVal strDatiAnagrafe As String,
                         ByVal strDatiPianificazione As String,
                         ByVal Utente_Username As String,
                         ByVal Utente_Password As String,
                         ByVal CodiceChiaveCliente As Integer,
                         ByVal LinkWSImportaGIAS As String,
                         ByRef strRisultato As String,
                         ByRef objParametri_Server As AgronicaCoreParametri,
                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                         Optional ByVal TipoOperazione As Integer = 1)

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String
        Dim Descrizione As String
        Dim x As Integer

        Dim Importa As New Ws_Importa_Gias.ImportaWS


        Try

            Dim Segnalazioni As String = " Segnalazioni: <br />" & strRisultato

            If LinkWSImportaGIAS <> "" Then

                Importa.Url = LinkWSImportaGIAS

                Importa.Timeout = System.Threading.Timeout.Infinite
                Descrizione = String.Empty

                Dim Str_Credenziali_WS As String
                Dim objXmlWs As New AgronicaCoreXML.XML_WS_Importa_Gias

                Str_Credenziali_WS = objXmlWs.Genera_Stringa_Credenziali(
                                     True,
                                     Nothing,
                                     Utente_Username,
                                     Utente_Password,
                                     objParametri_Server.PivaSuperUser,
                                     True,
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     objParametri_Server.StringaConnessione,
                                        objParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(
                                                Str_Credenziali_WS,
                                                strDatiAnagrafe,
                                                CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then

                        Descrizione &= "- " & XML_Risultato.GetAttribute("errore").ToString & "</br>"
                        strRisultato = Descrizione

                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                        Next

                    End If


                End If

                If strDatiPianificazione <> String.Empty Then

                    Descrizione &= "</br><b>Importazione pianificazione:" & "</b></br>"

                    'StrFinalePianificazione = Importa.Importa_Pianificazione(Utente_Username, _
                    '                                       Utente_Password, _
                    '                                       objParametri_Server.PivaSuperUser, _
                    '                                       strDatiPianificazione, _
                    '                                       CodiceChiaveCliente)

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(
                                                        Str_Credenziali_WS,
                                                        strDatiPianificazione,
                                                        CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    Try

                        XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                        Next

                    Catch ex As Exception
                        'se entra in questo catch la risposta non è nel formato xml previsto
                        Throw New Exception("Errore nella lettura della risposta del WS (pianificazione): " & ex.Message & vbCrLf &
                                            "Risposta: " & CStr(StrFinalePianificazione))
                    End Try


                End If

                strRisultato = Descrizione & Segnalazioni

            Else

                strRisultato = "Inserire l'indirizzo del Web Service Gias per continuare."

            End If

        Catch ex As Exception

            strRisultato = ex.Message

        End Try


    End Sub

End Class
