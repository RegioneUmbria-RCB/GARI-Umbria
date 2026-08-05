Imports System.Text
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class cls_ImportAgrea

    '##########################################################################################################################################
    Public Function Xml_Genera_Stringone_Anagrafe_WS_new(ByVal myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                                  ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByVal Utente_Username As String,
                                                  ByVal Utente_Password As String,
                                                  ByVal ProgressivoGIAS As Integer,
                                                  ByVal TipoImport As enum_TipoImportazioneAnagrafe,
                                                  ByRef Log_Errori As StringBuilder,
                                                  ByRef Log_Import As StringBuilder,
                                                  Optional Piva_Padre_Selected As String = "0",
                                                  Optional Path As String = "",
                                                  Optional AllegatiDocumentiXML As String = ""
                                                  ) As String

        Dim NomeRoutine As String = "cls_ImportAgrea.Xml_Genera_Anagrafe(): "

        Dim XmlDoc As New XmlDocument
        ' Dim XmlUtente As XmlElement
        Dim XmlImpresa As XmlElement
        Dim XmlCentro As XmlElement
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim Str_Xml As String = ""

        Try

            Dim CodiceChiaveCliente As Integer = 0
            Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            If objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server) <> "" Then
                CodiceChiaveCliente = objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server)
            End If

            ''-------------------------------
            ''----- IMPRESA
            ''-------------------------------
            Dim Piva As String = ""
            Dim Cuaa As String = ""

            Piva = myISWSResponse.azienda.partitaIva
            Cuaa = myISWSResponse.azienda.cuaa

            '06/05/2020: gestione piva estera, campo lungo ora 25
            If Piva = "" AndAlso Cuaa <> "" Then
                'se non ho la partita iva metto il CUAA tagliato a 11 cifre
                'Piva = Cuaa.Substring(0, 11)
                Piva = Cuaa
            End If

            Dim Piva_Padre As String
            If Piva_Padre_Selected = "0" Then
                Piva_Padre = Recupera_Piva_Padre(myISWSResponse.domanda.idCaa, Piva, objParametri_Server)
            Else
                Piva_Padre = Piva_Padre_Selected
            End If

            If Piva_Padre = Piva Then
                Piva_Padre = "#"
            End If

            Dim objSincroCatasto As New cls_SincronizzatoreCatasto

            '-------------------------------
            '----- IMPRESA - CENTRO - FABBRICATO
            '-------------------------------
            Dim SchedaValidazione As String = ""
            Dim DataValidazione As Date = AGRODATAINIZIO

            SchedaValidazione = CStr(myISWSResponse.domanda.idDomanda)
            If myISWSResponse.domanda.dataValidazione IsNot Nothing Then
                DataValidazione = CDate(myISWSResponse.domanda.dataValidazione)
            End If
            Dim Detentore_Fascicolo = myISWSResponse.domanda.descCaa

            Dim datavalidazioneArr = DataValidazione.ToShortDateString.Split("/")
            SchedaValidazione = myISWSResponse.domanda.idDomanda & "_" & datavalidazioneArr(2) & datavalidazioneArr(1) & datavalidazioneArr(0)


            objSincroCatasto.Gestione_Sincronizzazione_Anagrafica(
                                          objParametri_Server,
                                          Log_Errori,
                                          Log_Import,
                                            XmlDoc,
                                            XmlImpresa,
                                            XmlCentro,
                                            Utente_Username,
                                            Utente_Password,
                                            ProgressivoGIAS,
                                            TipoOperazione,
                                             CodiceChiaveCliente,
                                             myISWSResponse.azienda.idAzienda,
                                            Piva,
                                            Cuaa,
                                            Piva_Padre,
                                            myISWSResponse.azienda.ragioneSociale,
                                            myISWSResponse.azienda.indirizzo,
                                            myISWSResponse.azienda.cap,
                                             myISWSResponse.azienda.codIstatProv,
                                            myISWSResponse.azienda.codIstatCom,
                                          Path,
                                          SchedaValidazione,
                                          DataValidazione,
                                          Detentore_Fascicolo,
                                          AllegatiDocumentiXML)


            '-------------------------------
            '----- CATASTO
            '-------------------------------
            'questi due parametri sono gestiti nell'import da anagrafe
            Dim Opt_Particelle_1Insert2Modifica As Integer = 1 'imposto di modificare le particelle esistenti e inserire quelle che mancano
            Dim HT_PartCentri As New Hashtable 'non è stato scelto da interfaccia il centro da impostare nelle nuove particelle

            Xml_Genera_Stringone_Catasto_WS_new(myISWSResponse,
                                         TipoImport,
                                        TipoOperazione,
                                        objParametri_Server,
                                        objSincroCatasto,
                                        objXML,
                                        XmlDoc,
                                        XmlImpresa,
                                        XmlCentro,
                                         Opt_Particelle_1Insert2Modifica,
                                        HT_PartCentri,
                                        CodiceChiaveCliente,
                                        myISWSResponse.azienda.idAzienda,
                                        Piva,
                                        Log_Errori,
                                        Log_Import)


        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try

        ' Return XmlUtente.OuterXml

        Str_Xml = XmlDoc.OuterXml

        XmlDoc = Nothing

        Return Str_Xml

    End Function

    '##########################################################################################################################################
    Private Sub Xml_Genera_Stringone_Catasto_WS_new(ByVal myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                             ByVal TipoImport As enum_TipoImportazioneAnagrafe,
                                                ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objSincroCatasto As cls_SincronizzatoreCatasto,
                                                ByRef objXML As AgronicaCoreXML.AnagrafeXML,
                                                ByRef XmlDoc As XmlDocument,
                                                ByRef XMLImpresa As XmlElement,
                                                ByRef XMLCentro As XmlElement,
                                                ByVal Opt_Particelle_1Insert2Modifica As Integer,
                                                ByVal HT_PartCentri As Hashtable,
                                                ByVal CodiceChiaveCliente As Integer,
                                                ByVal ValCod_CodiceChiaveCliente As String,
                                                ByVal Piva As String,
                                                   ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Import As StringBuilder)

        Dim NomeRoutine As String = "cls_ImportAgrea.Xml_Genera_Catasto(): "

        Dim objCodificaZona As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
        Dim objZonexParticelle As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objMacrousixParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objPartxMacrousixUtilizzi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read


        Dim Str_Particelle As New StringBuilder
        Dim Str_ParticelleEsistenti As New StringBuilder
        Dim Str_ParticelleMancanti As New StringBuilder
        Dim Str_ParticelleUtilizzi As New StringBuilder
        Dim Str_ParticelleZone As New StringBuilder
        Dim Str_ParticelleMacrousi As New StringBuilder

        Dim num_ParticelleEsistenti As Integer = 0
        Dim num_ParticelleMancanti As Integer = 0

        Dim PROV As String = ""
        Dim COM As String = ""
        Dim Sezione As String = ""
        Dim Foglio As String = ""
        Dim Numero As String = ""
        Dim Subalterno As String = ""
        Dim SupCatastale As Double
        Dim TitoloPossesso As Integer
        Dim supConduzione As Double
        Dim PossessoInizio As Date
        Dim PossessoFine As Date
        Dim Indirizzo_Via As String
        Dim Indirizzo_Cap As String
        Dim Indirizzo_Prov As String
        Dim Indirizzo_Com As String

        Dim MetodoProduzione_Cod As Integer

        Dim HT_ZoneXPart As New Hashtable
        Dim HT_CodificaZoneMancanti As New Hashtable
        Dim HT_MacrousiXPart As New Hashtable

        Dim ListaMacrouso As New List(Of MacrousoObject)
        Dim objMacrouso As New MacrousoObject
        Dim objUtilizzo As New UtilizzoObject

        Dim i, j, z As Integer

        Try

            If Not IsNothing(myISWSResponse.possessi) Then

                Dim vett_str As New List(Of String)
                Dim FiltraParticelle As Boolean = False
                'vett_str.Add("038 005  47 24 000")
                'vett_str.Add("038 005  47 36 000")
                If HttpContext.Current.Session("objParticelleTot") IsNot Nothing Then
                    vett_str = HttpContext.Current.Session("objParticelleTot")
                    FiltraParticelle = True
                End If

                For i = 0 To myISWSResponse.possessi.Count - 1

                    Try

                        'Id_Chiave_Cliente = myISWSResponse.azienda.idAzienda

                        PROV = myISWSResponse.possessi(i).codProv
                        COM = myISWSResponse.possessi(i).codCom
                        Sezione = myISWSResponse.possessi(i).sezione
                        Foglio = myISWSResponse.possessi(i).foglio
                        Numero = myISWSResponse.possessi(i).particella
                        Subalterno = myISWSResponse.possessi(i).subalterno

                        'verifico se la particella è stata selezionata dall'utente
                        Dim lPart As String = PROV & " " & COM & " " & Sezione & " " & Foglio & " " & Numero & " " & Subalterno

                        If Not FiltraParticelle OrElse vett_str.Contains(lPart) Then

                            'myISWSResponse.azienda.descComune, _
                            'myISWSResponse.azienda.descProvincia, _ 
                            Indirizzo_Via = myISWSResponse.azienda.indirizzo
                            Indirizzo_Cap = myISWSResponse.azienda.cap
                            Indirizzo_Prov = myISWSResponse.azienda.codIstatProv
                            Indirizzo_Com = myISWSResponse.azienda.codIstatCom

                            SupCatastale = 0
                            If Not IsNothing(myISWSResponse.possessi(i).supCatastale) Then
                                SupCatastale = myISWSResponse.possessi(i).supCatastale / 10000
                            End If

                            TitoloPossesso = Converti_TitoliPossesso_Agrea(myISWSResponse.possessi(i).codPossesso)

                            supConduzione = myISWSResponse.possessi(i).supPossesso
                            PossessoInizio = myISWSResponse.possessi(i).dataInizioPoss
                            PossessoFine = myISWSResponse.possessi(i).dataFinePoss

                            MetodoProduzione_Cod = ConvertiMetodo_Produzione(myISWSResponse.possessi(i).scoBioProdInt)

                            If Not IsNothing(myISWSResponse.possessi(i).macrousi) Then
                                For j = 0 To myISWSResponse.possessi(i).macrousi.Count - 1

                                    objMacrouso = New MacrousoObject
                                    objMacrouso.CodMacrouso = Right(myISWSResponse.possessi(i).macrousi(j).codMacrouso, 3)

                                    Select Case TipoImport
                                        Case -1
                                            'gestire qui i casi che vanno divisi per 10000
                                            objMacrouso.SupMacrouso = CDbl(myISWSResponse.possessi(i).macrousi(j).supMacrouso) / 10000.0
                                        Case Else
                                            objMacrouso.SupMacrouso = CDbl(myISWSResponse.possessi(i).macrousi(j).supMacrouso)
                                    End Select


                                    If Not IsNothing(myISWSResponse.possessi(i).macrousi(j).utilizzi) Then

                                        objMacrouso.Utilizzo = New List(Of UtilizzoObject)

                                        For z = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Count - 1
                                            objUtilizzo = New UtilizzoObject
                                            objUtilizzo.SpecieCod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codColtura
                                            objUtilizzo.VarietaCod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codVarieta

                                            Select Case TipoImport
                                                Case -1
                                                    'gestire qui i casi che vanno divisi per 10000
                                                    objUtilizzo.SupUtilizzo = CDbl(myISWSResponse.possessi(i).macrousi(j).utilizzi(z).supUtilizzo) / 10000.0
                                                Case Else
                                                    objUtilizzo.SupUtilizzo = CDbl(myISWSResponse.possessi(i).macrousi(j).utilizzi(z).supUtilizzo)
                                            End Select
                                            objMacrouso.Utilizzo.Add(objUtilizzo)
                                            'ListaUtilizzo.Add(objUtilizzo)
                                        Next
                                    End If
                                    'objMacrouso.Utilizzo = ListaUtilizzo
                                    ListaMacrouso.Add(objMacrouso)
                                    'ListaUtilizzo.Clear()
                                Next
                            End If 'macrousi

                            Dim ZoneArr As New List(Of String)

                            If myISWSResponse.possessi(i).codZone IsNot Nothing Then
                                For Each zona In myISWSResponse.possessi(i).codZone
                                    ZoneArr.Add(zona)
                                Next
                            End If

                            objSincroCatasto.Gestione_Sincronizzazione_Catasto(
                                                 TipoImport,
                                                 objParametri_Server,
                                                 XmlDoc,
                                                 XMLImpresa,
                                                 XMLCentro,
                                                 Str_Particelle,
                                                 Str_ParticelleEsistenti,
                                                 Str_ParticelleMancanti,
                                                 Str_ParticelleUtilizzi,
                                                 Str_ParticelleZone,
                                                 Str_ParticelleMacrousi,
                                                 num_ParticelleEsistenti,
                                                 num_ParticelleMancanti,
                                                 HT_ZoneXPart,
                                                 HT_MacrousiXPart,
                                                 HT_CodificaZoneMancanti,
                                                 TipoOperazione,
                                                 CodiceChiaveCliente,
                                                 ValCod_CodiceChiaveCliente,
                                                 Opt_Particelle_1Insert2Modifica,
                                                 HT_PartCentri,
                                                 Piva,
                                                 Indirizzo_Via,
                                                 Indirizzo_Cap,
                                                 Indirizzo_Prov,
                                                 Indirizzo_Com,
                                                 PROV,
                                                 COM,
                                                 Sezione,
                                                 Foglio,
                                                 Numero,
                                                 Subalterno,
                                                 SupCatastale,
                                                 TitoloPossesso,
                                                 supConduzione,
                                                 PossessoInizio,
                                                 PossessoFine,
                                                 ZoneArr.ToArray,
                                                 ListaMacrouso,
                                                 objXML,
                                                 objImpresexParticelle,
                                                 objCentri,
                                                 objCodificaZona,
                                                 objZonexParticelle,
                                                 objMacrousixParticelle,
                                                 objPartxMacrousixUtilizzi,
                                                 MetodoProduzione_Cod
                                                )

                            ListaMacrouso.Clear()

                        End If

                    Catch ex As Exception
                        Dim Str_Particella_InCorso As String
                        Str_Particella_InCorso = PROV & " " &
                                  COM & " " &
                                  Sezione & " " &
                                  Foglio & " " &
                                  Numero & " " &
                                  Subalterno

                        Throw New Exception(Str_Particella_InCorso & " - " & ex.Message)
                    End Try

                Next

            End If

            'Dim pippo1, pippo2, pippo3, pippo4, pippo5, pippo6 As String

            ''Str_ParticelleDuplicate.Append("Num. particelle ripetute: " & CStr(num_ParticelleDuplicate))
            ''Str_ParticelleMancanti.Append("Num. particelle mancanti: " & CStr(num_ParticelleMancanti))
            ''Str_ParticelleSupCatModifica.Append("Num. particelle con sup.catastale aggiornata: " & CStr(num_ParticelleSupCatModifica))

            'pippo1 = Str_Particelle.ToString
            'pippo3 = Str_ParticelleMancanti.ToString
            '' pippo2 = Str_ParticelleDuplicate.ToString
            ''pippo4 = Str_ParticelleSupCatModifica.ToString
            'pippo5 = Str_ParticelleZone.ToString
            'pippo6 = Str_ParticelleMacrousi.ToString

            Log_Import.Append(vbCrLf & "Riepilogo situazione catasto:" & vbCrLf)
            Log_Import.Append("Num. particelle mancanti: " & CStr(num_ParticelleMancanti) & vbCrLf)
            Log_Import.Append(Str_ParticelleMancanti.ToString & vbCrLf)
            Log_Import.Append("Num. particelle esistenti: " & CStr(num_ParticelleEsistenti) & vbCrLf)
            Log_Import.Append(Str_ParticelleEsistenti.ToString & vbCrLf)
            Log_Import.Append(Str_ParticelleZone.ToString)
            Log_Import.Append("Num. codifiche zone mancanti: " & CStr(HT_CodificaZoneMancanti.Keys.Count) & vbCrLf)
            If HT_CodificaZoneMancanti.Keys.Count > 0 Then
                Log_Import.Append("Elenco codici zona (origine) mancanti: " & vbCrLf)
                For Each key In HT_CodificaZoneMancanti.Keys
                    Log_Import.Append(key & vbCrLf)
                Next
            End If
            Log_Import.Append(vbCrLf)
            Log_Import.Append(Str_ParticelleMacrousi.ToString)
            Log_Import.Append(Str_ParticelleUtilizzi.ToString)

        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try

    End Sub

    '##########################################################################################################################################
    Public Function Xml_Genera_Stringone_Pianificazione_WS_new(ByVal myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                                        ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                        ByVal Utente_Username As String,
                                                        ByVal Utente_Password As String,
                                                        ByVal ProgressivoGIAS As Integer,
                                                        ByVal ANNO As Integer,
                                                        ByRef LogCodificheMancantiSpecie As String,
                                                        ByRef LogCodificheMancantiVarieta As String,
                                                        Optional NomeFile As String = "",
                                                        Optional str_XML As String = "")

        Dim XmlDoc As New XmlDocument

        Dim XmlUtente As XmlElement
        Dim XmlTestata As XmlElement
        Dim XmlEntita As XmlElement
        Dim XmlParticella As XmlElement
        'Dim XmlFascicoloP As XmlElement

        Dim Piva As String = ""
        Dim Cuaa As String = ""

        Dim objCodificaZona As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim SchedaValidazione As String = ""
        Dim DataValidazione As Date = AGRODATAINIZIO
        'Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        'Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        'Dim DataInizioMandato As Date = AGRODATAINIZIO
        'Dim DataFineMandato As Date = AGRODATAFINE

        Dim CodiceChiaveCliente As Integer = 0

        Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        If objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server) <> "" Then
            CodiceChiaveCliente = objconfigSiti.Leggi_Valore(0, "Codice_Chiave_Cliente", "", "", objParametri_Server)
        End If


        XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc,
                                            Utente_Username,
                                            Utente_Password,
                                            ProgressivoGIAS)
        Dim strAnno As String = String.Empty
        Dim strValiditaInizio As String = String.Empty
        Dim strValiditaFine As String = String.Empty

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DataInizioUtente As Date = AGRODATAINIZIO
        Dim DataFineUtente As Date = AGRODATAFINE
        objImpost.AnnataAgraria(CDate("01/01/" & ANNO.ToString), DataInizioUtente, DataFineUtente, objParametri_Utenti)

        If IsNumeric(ANNO) AndAlso (CInt(ANNO) > 1900 AndAlso CInt(ANNO) < 2100) Then
            strAnno = CInt(ANNO).ToString
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        Else
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        End If

        Piva = myISWSResponse.azienda.partitaIva
        Cuaa = myISWSResponse.azienda.cuaa

        '06/05/2020: gestione piva estera, campo lungo ora 25
        If Piva = "" AndALso Cuaa <> "" Then
            'se non ho la partita iva metto il CUAA tagliato a 11 cifre
            'Piva = Cuaa.Substring(0, 11)
            Piva = Cuaa
        End If

        Dim DtParticelle As New DataTable
        DtParticelle = objImpresexParticelle.Leggi(0,
                                                 Piva,
                                                 0, 0, "", "", "", 0, 0, "",
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)

        XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(TipoOperazione,
                                                               "0",
                                                               Piva,
                                                               "Pianificazione " & strAnno,
                                                               "Pianificazione " & strAnno,
                                                               "Importazione da archivi AGREA " & strAnno,
                                                               enum_Planning_Fonte.Agrea,
                                                               enum_TipoPianificazione.Pianificazione_Annuale,
                                                               strValiditaInizio,
                                                               strValiditaFine,
                                                               XmlDoc)

        If myISWSResponse.domanda.idDomanda <> 0 Then
            SchedaValidazione = myISWSResponse.domanda.idDomanda
        End If
        If myISWSResponse.domanda.dataValidazione IsNot Nothing Then
            DataValidazione = CDate(myISWSResponse.domanda.dataValidazione)
        End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato Is Nothing Then
        '    DataInizioMandato = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo Is Nothing Then
        '    DataAperturaFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo Is Nothing Then
        '    DataChiusuraFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo))
        'End If

        Dim Detentore_Fascicolo As String = ""
        If myISWSResponse.domanda.descCaa IsNot Nothing Then
            Detentore_Fascicolo = myISWSResponse.domanda.descCaa
        End If

        'XmlFascicoloP = objXML.Xml_Pubblico_Fascicolo(NomeFile,
        '                                             SchedaValidazione,
        '                                             DataValidazione,
        '                                             Detentore_Fascicolo,
        '                                             DataInizioMandato,
        '                                             DataFineMandato,
        '                                             DataAperturaFascicolo,
        '                                             DataChiusuraFascicolo,
        '                                             XmlDoc,
        '                                             str_XML)

        'XmlTestata.AppendChild(XmlFascicoloP)



        Dim SaCod As Integer = 0
        Dim SaNome As String = ""

        SaCod = New AgronicaCoreAnagrafeDAL.Centri_Codici_Read().RecuperaSaCodImpresaByIdAziendaFascicolo(
                                    CodiceChiaveCliente,
                                    Piva,
                                    myISWSResponse.azienda.idAzienda,
                                    SaNome,
                                    objParametri_Server)


        Dim i, j, k As Integer

        Dim strSezione As String = String.Empty
        Dim strSubalterno As String = String.Empty

        Dim Veg_Cod As Integer
        Dim Veg_Cod_CodificaCultivar As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Id_Cod As Integer
        Dim Grva_Cod As Integer
        Dim Cul_Cod_Cliente As String
        Dim Veg_Cod_Cliente As String
        Dim Macrouso_Cod As String

        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim nEntita As Integer = 1

        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R

        If Not IsNothing(myISWSResponse.possessi) Then

            Dim vett_str As New List(Of String)
            Dim FiltraParticelle As Boolean = False
            'vett_str.Add("038 005  47 24 000")
            'vett_str.Add("038 005  47 36 000")
            If HttpContext.Current.Session("objImpiantiTot") IsNot Nothing Then
                vett_str = HttpContext.Current.Session("objImpiantiTot")
                FiltraParticelle = True
            End If

            For i = 0 To myISWSResponse.possessi.Count - 1

                If IsNothing(myISWSResponse.possessi(i).sezione) OrElse myISWSResponse.possessi(i).sezione = "" Then
                    strSezione = "0"
                Else
                    strSezione = myISWSResponse.possessi(i).sezione
                End If

                If IsNothing(myISWSResponse.possessi(i).subalterno) OrElse
                   myISWSResponse.possessi(i).subalterno = "" OrElse
                   myISWSResponse.possessi(i).subalterno = "000" Then
                    strSubalterno = "0"
                Else
                    strSubalterno = myISWSResponse.possessi(i).subalterno
                End If

                If Not IsNothing(myISWSResponse.possessi(i).macrousi) Then

                    For j = 0 To myISWSResponse.possessi(i).macrousi.Count - 1

                        If Not IsNothing(myISWSResponse.possessi(i).macrousi(j).utilizzi) Then

                            For k = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Count - 1

                                Veg_Cod = 0
                                Veg_Cod_CodificaCultivar = 0
                                Cul_Cod = 0
                                Grfi_Cod = 0
                                Id_Cod = 0
                                Grva_Cod = 0

                                Veg_Cod_Agea = ""
                                Cul_Cod_Agea = ""
                                Uso_Cod_Agea = ""
                                Occupazione_Cod_Agea = ""
                                Destinazione_Cod_Agea = ""
                                Qualita_Cod_Agea = ""

                                Cul_Cod_Cliente = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                Macrouso_Cod = myISWSResponse.possessi(i).macrousi(j).codMacrouso

                                If ANNO <= 2014 Then
                                    Veg_Cod_Cliente = Strings.Right("000" & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura, 3)
                                    Dim objUtilizzi1 As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                                    objUtilizzi1.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                              LogCodificheMancantiVarieta,
                                                                              Veg_Cod_Cliente, Cul_Cod_Cliente,
                                                                              Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                              "", "",
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descColtura,
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descVarieta,
                                                                              CDate(strValiditaInizio),
                                                                              Uso_Cod_Agea,
                                                                                Occupazione_Cod_Agea,
                                                                                Destinazione_Cod_Agea,
                                                                                Qualita_Cod_Agea,
                                                                              objParametri_Server)
                                Else
                                    Dim cod_ColturaArr = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura.Split("-")
                                    Veg_Cod_Agea = ""
                                    Cul_Cod_Agea = cod_ColturaArr(4)
                                    Uso_Cod_Agea = cod_ColturaArr(0)
                                    Occupazione_Cod_Agea = cod_ColturaArr(1)
                                    Destinazione_Cod_Agea = cod_ColturaArr(2)
                                    Qualita_Cod_Agea = cod_ColturaArr(3)

                                    If Cul_Cod_Agea = "000" AndAlso myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta <> "000" Then
                                        Cul_Cod_Agea = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                    End If

                                    objUtilizzi.Specie_e_Varieta_Gias_Da_AGEA(LogCodificheMancantiSpecie,
                                                          LogCodificheMancantiVarieta,
                                                          Veg_Cod_Agea,
                                                          Cul_Cod_Agea,
                                                          Uso_Cod_Agea,
                                                          Occupazione_Cod_Agea,
                                                          Destinazione_Cod_Agea,
                                                          Qualita_Cod_Agea,
                                                          Veg_Cod, Cul_Cod,
                                                          Grfi_Cod,
                                                          Grva_Cod,
                                                          Id_Cod,
                                                          "",
                                                          "",
                                                          objParametri_Server,
                                                          "",
                                                          "")
                                End If

                                If Veg_Cod = 0 Then
                                    Veg_Cod = Veg_Cod_CodificaCultivar
                                End If

                                If CInt(Veg_Cod) <> 0 AndAlso CInt(Cul_Cod) = 0 Then
                                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                End If

                                Dim z As Integer
                                Dim strZona As String = "n"
                                Dim CodiceZonaGIAS As Integer

                                If Not IsNothing(myISWSResponse.possessi(i).codZone) Then

                                    For z = 0 To myISWSResponse.possessi(i).codZone.Count - 1
                                        CodiceZonaGIAS = objCodificaZona.ConvertiZona(myISWSResponse.possessi(i).codZone(z),
                                                                                      "", objParametri_Server)

                                        If CodiceZonaGIAS = "-17" Then
                                            strZona = "v"
                                            Exit For
                                        End If
                                    Next

                                End If

                                Dim flagIrrigabilita As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagIrrigabilita) AndAlso LCase(myISWSResponse.possessi(i).flagIrrigabilita) = "s" Then
                                    flagIrrigabilita = "1"
                                End If

                                Dim flagSecondoRaccolto As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagSecondoRaccolto) AndAlso LCase(myISWSResponse.possessi(i).flagSecondoRaccolto) = "s" Then
                                    flagSecondoRaccolto = "1"
                                End If

                                Dim Prov As String = myISWSResponse.possessi(i).codProv
                                Dim Com As String = myISWSResponse.possessi(i).codCom
                                Dim Sezione As String = strSezione
                                Dim Foglio As Integer = myISWSResponse.possessi(i).foglio
                                Dim Numero As Integer = myISWSResponse.possessi(i).particella
                                Dim Subalterno As String = strSubalterno

                                Dim numPiante As Integer = 0
                                Dim Validita_inizio_impianto As String = strValiditaInizio
                                Dim Cop_Cod As Integer = 0
                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree IsNot Nothing AndAlso myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree.Count > 0 Then

                                    For Each unitaArborea In myISWSResponse.possessi(i).macrousi(j).utilizzi(k).unitaArboree
                                        numPiante += unitaArborea.numeroPiante
                                        Validita_inizio_impianto = "01/01/" & unitaArborea.annoImpianto
                                        If unitaArborea.scoProtezione IsNot Nothing Then
                                            Cop_Cod = ConvertiCopertura(unitaArborea.scoProtezione)
                                        End If
                                    Next

                                End If

                                Dim DataInizioDestinazione As String = strValiditaInizio

                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione IsNot Nothing AndAlso IsDate(myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione) Then
                                    DataInizioDestinazione = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataInizioDestinazione
                                End If

                                Dim DataFineDestinazione As String = strValiditaFine

                                If myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione IsNot Nothing AndAlso IsDate(myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione) Then
                                    DataFineDestinazione = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).dataFineDestinazione
                                End If

                                Dim MetodoProduzione_Cod As Integer = 0
                                If myISWSResponse.possessi(i).scoBioProdInt IsNot Nothing Then
                                    MetodoProduzione_Cod = ConvertiMetodo_Produzione(myISWSResponse.possessi(i).scoBioProdInt)
                                End If

                                '(29/04/2015 fede) verifico in quale centro è la particella
                                If DtParticelle IsNot Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                                    Dim DrParticella As DataRow() = DtParticelle.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                                    If DrParticella IsNot Nothing AndAlso DrParticella.Length > 0 Then
                                        SaCod = DrParticella(0).Item("sa_cod")
                                    End If
                                End If

                                '(07/11/2016 fede) controllo selezione utente
                                Dim lPart As String = myISWSResponse.possessi(i).codProv & " " & myISWSResponse.possessi(i).codCom & " " &
                                    myISWSResponse.possessi(i).sezione & " " & myISWSResponse.possessi(i).foglio & " " &
                                    myISWSResponse.possessi(i).particella & " " & myISWSResponse.possessi(i).subalterno & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura & " " & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo.ToString

                                If Not FiltraParticelle OrElse vett_str.Contains(lPart) Then


                                    XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(TipoOperazione,
                                                                                        "0",
                                                                                        "App. " & Right("000" & nEntita, 3),
                                                                                        SaCod,
                                                                                        "#",
                                                                                        -nEntita,
                                                                                          "0", "0", "Lotto" & strAnno,
                                                                                        Veg_Cod,
                                                                                        Grfi_Cod,
                                                                                        Cul_Cod,
                                                                                        Grva_Cod,
                                                                                        Veg_Cod_Cliente, Cul_Cod_Cliente, "#",
                                                                                        Macrouso_Cod,
                                                                                        Id_Cod, "#",
                                                                                        myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo,
                                                                                        "#",
                                                                                        strZona,
                                                                                        "#", "#", "#", "#", "#", numPiante,
                                                                                        "#", "#", "#", "#",
                                                                                        DataInizioDestinazione,
                                                                                        DataFineDestinazione,
                                                                                        Validita_inizio_impianto,
                                                                                        XmlDoc,
                                                                                        FlagIrrigabilita:=flagIrrigabilita,
                                                                                        FlagSecondoRaccolto:=flagSecondoRaccolto,
                                                                                        Veg_Cod_Agea:=Veg_Cod_Agea,
                                                                                        Cul_Cod_Agea:=Cul_Cod_Agea,
                                                                                        Uso_Cod_Agea:=Uso_Cod_Agea,
                                                                                        Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                                                                        Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                                                                        Qualita_Cod_Agea:=Qualita_Cod_Agea,
                                                                                        MetodoProduzione_Cod:=MetodoProduzione_Cod)

                                    XmlParticella = objXML.Xml_Pubblico_ProgrammazioneParticella(TipoOperazione,
                                                                                                 myISWSResponse.possessi(i).codCom,
                                                                                                 myISWSResponse.possessi(i).codProv,
                                                                                                 strSezione,
                                                                                                 myISWSResponse.possessi(i).foglio,
                                                                                                 myISWSResponse.possessi(i).particella,
                                                                                                 strSubalterno,
                                                                                                 myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo,
                                                                                                 "#", "#",
                                                                                                 XmlDoc)

                                    nEntita += 1

                                    XmlEntita.AppendChild(XmlParticella)
                                    XmlTestata.AppendChild(XmlEntita)

                                End If

                            Next

                        End If 'utilizzi

                    Next

                End If 'macrousi

            Next

        End If 'possessi

        XmlUtente.AppendChild(XmlTestata)

        Return XmlUtente.OuterXml

    End Function

    Public Function ConvertiCopertura(scoProtezione As String) As Integer
        Dim Cop_Cod As Integer = 0
        Select Case scoProtezione
            Case "001" 'Copertura con rete antigrandine
                Cop_Cod = 1
            Case "002" 'Presenza di impianto antibrina
                Cop_Cod = 0
            Case "004" 'Ombrai
                Cop_Cod = 1
            Case "003" 'Serre e tunnel fissi
                Cop_Cod = 2
            Case "002" 'Copertura con reti antiacqua
                Cop_Cod = 1
            Case Else
                Cop_Cod = 0
        End Select
        Return Cop_Cod
    End Function

    Public Function ConvertiMetodo_Produzione(scoBioProdInt As String) As Integer
        Dim MetodoProduzione_Cod As Integer = 0
        Select Case scoBioProdInt
            Case "001" 'Convenzionale
                MetodoProduzione_Cod = enum_MetodoProduzione.Integrato
            Case "004" 'In Conversione
                MetodoProduzione_Cod = enum_MetodoProduzione.InConversione
            Case "003" 'Biologico
                MetodoProduzione_Cod = enum_MetodoProduzione.Biologico
            Case "002" 'Produzione Integrata
                MetodoProduzione_Cod = enum_MetodoProduzione.Biologico
            Case Else
                MetodoProduzione_Cod = enum_MetodoProduzione.Integrato
        End Select
        Return MetodoProduzione_Cod
    End Function




    '########################################################################################
    Public Sub Scrivi_Dati(ByRef Log_Errori As StringBuilder,
                            ByRef Log_Import As StringBuilder,
                            ByVal strDatiAnagrafe As String,
                            ByVal strDatiPianificazione As String,
                            ByRef Str_RisultatoMassivaImport As String,
                            ByRef Str_RisultatoMassivaErrori As String,
                            ByRef Str_RisultatoCUAA As String,
                            ByRef Num_Azi_Sincronizzate As Integer,
                            ByVal Utente_Username As String,
                            ByVal Utente_Password As String,
                            ByVal Piva_SuperUser As String,
                            ByVal Azienda_InElaborazione As String,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreParametri)

        Const NomeFunzione As String = "Scrivi_Dati."

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String

        Dim LinkWSImportaGIAS As String
        Dim CodiceChiaveCliente As Integer

        Dim x As Integer

        Dim Importa As New ws_Importa_Gias.ImportaWS

        Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        LinkWSImportaGIAS = objconfigSiti.Leggi_Valore(0, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)
        CodiceChiaveCliente = objconfigSiti.Leggi_Valore(0, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)


        Try

            If LinkWSImportaGIAS <> "" Then

                Importa.Url = LinkWSImportaGIAS
                Importa.Timeout = Integer.MaxValue

                Str_RisultatoMassivaImport = String.Empty
                Str_RisultatoMassivaErrori = String.Empty
                Str_RisultatoCUAA = String.Empty

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

                    Str_RisultatoCUAA &= "<b>Sincronizzazione anagrafe:" & "</b><br/>"

                    Log_Import.Append(vbCrLf & Date.Now.ToString & " - Inizio scrittura catasto." & vbCrLf)

                    Try

                        StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(Str_Credenziali_WS,
                                                                                        strDatiAnagrafe,
                                                                                       CodiceChiaveCliente)

                        Num_Azi_Sincronizzate += 1


                    Catch ex As HttpException
                        Str_RisultatoCUAA &= "Chiamata al web service: " & ex.Message & vbCrLf
                        Str_RisultatoMassivaErrori &= "Chiamata al web service: " & ex.Message & vbCrLf
                        Exit Sub
                    End Try

                    Log_Import.Append(Date.Now.ToString & " - Fine scrittura catasto." & vbCrLf)

                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then
                        Str_RisultatoCUAA &= "- " & XML_Risultato.GetAttribute("errore").ToString & "<br/>"
                        Str_RisultatoMassivaErrori &= "Sincro anagrafe ERRORE: " & XML_Risultato.GetAttribute("errore").ToString & vbCrLf
                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Str_RisultatoCUAA &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "<br/>"

                            If InStr(strRisp, "Errore") > 0 Then
                                Str_RisultatoMassivaErrori &= "Sincro anagrafe ERRORE: " & strRisp & vbCrLf
                            Else
                                Str_RisultatoMassivaImport &= "Sincro anagrafe: " & strRisp & " OK" & vbCrLf
                            End If

                        Next

                    End If

                Else
                    ''stringa vuota
                    Str_RisultatoCUAA &= "<b>Sincronizzazione anagrafe:" & "</b><br/>"
                    Str_RisultatoCUAA &= "- " & "non eseguita" & "<br/>"
                End If

                If strDatiPianificazione <> String.Empty Then

                    Str_RisultatoCUAA &= "</br><b>Importazione pianificazione:" & "</b><br/>"

                    Log_Import.Append(vbCrLf & Date.Now.ToString & " - Inizio scrittura pianificazione." & vbCrLf)

                    'StrFinalePianificazione = Importa.Importa_Pianificazione(Utente_Username, _
                    '                                                        Utente_Password, _
                    '                                                       Piva_SuperUser, _
                    '                                                       strDatiPianificazione, _
                    '                                                       CodiceChiaveCliente)

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(Str_Credenziali_WS,
                                                                                         strDatiPianificazione,
                                                                                            CodiceChiaveCliente)

                    Log_Import.Append(Date.Now.ToString & " - Fine scrittura pianificazione." & vbCrLf)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then
                        Str_RisultatoCUAA &= "- " & XML_Risultato.GetAttribute("errore").ToString & "<br/>"
                        Str_RisultatoMassivaErrori &= "Import Pianificazione ERRORE: " & XML_Risultato.GetAttribute("errore").ToString & vbCrLf
                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Str_RisultatoCUAA &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "<br/>"

                            If InStr(strRisp, "Errore") > 0 Then
                                Str_RisultatoMassivaErrori &= "Import pianificazione ERRORE: " & strRisp & vbCrLf
                            Else
                                Str_RisultatoMassivaImport &= "Import pianificazione: " & strRisp & " OK" & vbCrLf
                            End If

                        Next

                    End If
                Else
                    ''stringa vuota
                    Str_RisultatoCUAA &= "</br><b>Importazione pianificazione:" & "</b><br/>"
                    Str_RisultatoCUAA &= "- " & "non eseguita" & "<br/>"
                End If

                'Me.Lbl_RisImportazione.Text = Descrizione

            Else
                'il controllo è già stato fatto a monte
                'AgroMsgBox("Inserire l'indirizzo del Web Service Gias per continuare.", Page)
                'Log_Errori.Append(NomeFunzione & "Inserire l'indirizzo del Web Service Gias per continuare." & vbCrLf)
            End If

        Catch ex As Exception
            'AgroMsgBox(ex.Message, Page)
            'Log_Errori.Append(NomeFunzione & ex.Message & vbCrLf)
            Str_RisultatoCUAA &= NomeFunzione & ex.Message & vbCrLf
            Str_RisultatoMassivaErrori &= NomeFunzione & ex.Message & vbCrLf
        End Try


    End Sub


    '##############################################################################################
    Private Function Recupera_Piva_Padre(ByVal ID_CAA As String,
                                         ByVal Piva As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim PivaPadre As String = ""

        Select Case objParametri_Server.PivaSuperUser

            Case "05390270014", "05644051004" 'COLDIRETTI ER, COLDIRETTI NAZIONALE

                Dim ID_CAA_Format As String
                If ID_CAA.Length = 2 Then
                    ID_CAA_Format = "0" & ID_CAA
                Else
                    ID_CAA_Format = ID_CAA
                End If

                Dim objInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                Dim Dt As DataTable
                Dt = objInfoAgg.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Agrea,
                                      ID_CAA_Format,
                                      0,
                                      1,
                                      enumSelezioneVariabile.Selezione_JoinCompleta,
                                      "", "", objParametri_Server)

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                    PivaPadre = Dt.Rows(0).Item("TestoAux_1")
                Else
                    Select Case ID_CAA
                        Case 88, 89, 90, 91, 92, 93, 94, 95, 96  'BOLOGNA
                            PivaPadre = "04138500378"   '"00000000000"
                        Case 142, 147, 148, 149, 150, 151, 152  'REGGIO EMILIA
                            PivaPadre = "01895640355"    ' "11111111111"
                        Case 137, 138, 139  'RAVENNA
                            PivaPadre = "03477530400" '"22222222222"
                        Case 120, 121, 122, 123, 124, 125, 126, 127  'PIACENZA
                            PivaPadre = "01283730339" ' "33333333333"
                        Case 129, 130, 131, 132, 133, 134, 135, 136  'PARMA
                            PivaPadre = "02102610348" '"44444444444"
                        Case 145, 153, 154  'RIMINI
                            PivaPadre = "03477530400" '"55555555555"
                        Case 102, 103, 104, 105, 106, 107, 108  'FERRARA
                            PivaPadre = "00675910384"   '"66666666666"
                        Case 97, 98, 99, 100, 101  'FORLI
                            PivaPadre = "03477530400" '"77777777777"
                        Case 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119  'MODENA
                            PivaPadre = "02613050364" '"88888888888"
                        Case Else
                            PivaPadre = objParametri_Server.PivaSuperUser
                    End Select

                End If

                '=====================================================

            Case "02317021208" 'FEDEREMILIA

                Select Case ID_CAA
                    Case 83, 180, 181, 182, 183 'BOLOGNA OK
                        PivaPadre = "80038970374"
                    Case 86, 194, 195 'REGGIO EMILIA OK
                        PivaPadre = "80012830354"
                    Case 81, 176, 177, 178, 179  'PIACENZA OK
                        PivaPadre = "90011170338"
                    Case 84, 211, 212, 213, 214, 215  'PARMA OK
                        PivaPadre = "80004070340"
                    Case 87 'FERRARA
                        PivaPadre = "80006190385"
                    Case 80  'FORLI CESENA RIMINI OK
                        PivaPadre = "80009510407"
                    Case 82, 230, 231, 232, 233, 234, 235, 236, 237  'MODENA OK
                        PivaPadre = "80008110365"
                    Case 85, 240, 241  'RAVENNA OK
                        PivaPadre = "80008770390"
                    Case Else
                        PivaPadre = objParametri_Server.PivaSuperUser
                End Select

                '======================================

            Case Else
                '    PivaPadre = objParametri_Server.PivaSuperUser

                Dim objInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                Dim Dt As DataTable

                Dt = objInfoAgg.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Agrea,
                                      ID_CAA,
                                      0,
                                      1,
                                      enumSelezioneVariabile.Selezione_JoinCompleta,
                                      "", "", objParametri_Server)

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                    PivaPadre = Dt.Rows(0).Item("TestoAux_1")
                Else
                    'correzione dell'08/08/2014: 
                    'per evitare di scrivere male il record del superuser in gerarchieimprese
                    If Piva = objParametri_Server.PivaSuperUser Then
                        'è il superuser
                        PivaPadre = ""
                    Else

                        Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

                        PivaPadre = objGerarchia.LeggiPadre(Piva, objParametri_Server, "")

                        'è un'azienda figlia del superuser
                        If PivaPadre = "" Then
                            PivaPadre = objParametri_Server.PivaSuperUser
                        End If

                    End If
                End If

                Dt.Dispose()
                Dt = Nothing

                objInfoAgg = Nothing

                '--------------------------------------

        End Select

        Return PivaPadre

    End Function

    '##############################################################################################
    Private Function Converti_TitoliPossesso_Agrea(ByVal TitoloPossesso As String) As Integer

        Dim Particella_Possesso As Integer

        If TitoloPossesso.StartsWith("FP") Then
            Select Case TitoloPossesso

                Case "FP003", "FP004"
                    Particella_Possesso = 0 'altro
                Case "FP001", "FP021"
                    Particella_Possesso = 1 'Proprietà
                Case "FP011", "FP018"
                    Particella_Possesso = 2 'Comodato d'uso
                Case "FP002", "FP017"
                    Particella_Possesso = 3 'Affitto con contratto
                Case ""
                    Particella_Possesso = 4 'Affitto senza contratto
                Case Else
                    Particella_Possesso = 0 'altro
            End Select
        Else
            Select Case TitoloPossesso
                'Case "FP003", "FP004"
                '    Particella_Possesso = 0 'altro
                Case "001", "008", "014"
                    Particella_Possesso = 1 'Proprietà
                Case "009", "011", "010"
                    Particella_Possesso = 2 'Comodato d'uso
                Case "002"
                    Particella_Possesso = 3 'Affitto con contratto           
                Case Else
                    Particella_Possesso = 0 'altro
            End Select
        End If

        Return Particella_Possesso

    End Function

    '################################################################################################################
    Public Sub Salva_Log_Import(ByRef Messaggio As String,
                                ByVal Log_Errori As String,
                                ByVal Log_Import As String,
                                ByVal PathDirFileLog As String)

        Dim FileLog As String
        Dim Intestazione As String
        Dim NomeFileLog As String
        Dim Data As String
        Dim DataEstesa As String
        Dim Ora As String
        Dim Str_Errore As String = ""
        Dim Path_CartellaFileLog As String = ""
        Dim Path_FileLog As String = ""
        Dim NomeCartella As String

        DataEstesa = Date.Now.ToLongDateString

        'Data = Replace(Date.Now.ToShortDateString, "/", "-")
        Data = Format(Date.Today, "yyyy-MM-dd")
        Ora = Date.Now.ToLongTimeString
        Ora = Replace(Ora, ":", ".")

        Intestazione = "Log Importazione AGREA " & DataEstesa & " " & Ora & vbCrLf & vbCrLf & vbCrLf

        NomeCartella = "AGREA_Import" & "_" & Data & "_" & Ora

        GestioneFile.CreaCartellaNelPath(PathDirFileLog, NomeCartella, Path_CartellaFileLog, Str_Errore)

        If Str_Errore = "" AndAlso Path_CartellaFileLog <> "" Then

            'If Log_Import <> "" Then

            FileLog = Intestazione & Log_Import & vbCrLf & vbCrLf & vbCrLf

            NomeFileLog = "AGREA_Import" & "_" & Data & "_" & Ora & "_Log"

            GestioneFile.CreaScriviFileSovrascrivi_RitornaPath(FileLog, Path_CartellaFileLog, NomeFileLog, Path_FileLog, Str_Errore, "txt")

            If Str_Errore <> "" Then
                Messaggio &= "Errore nel salvataggio del Log dell'Import AGREA: " & Str_Errore & vbCrLf
            End If

            'Else
            '    Messaggio &= "Il Log dell'Import AGREA è vuoto, impossibile salvare." & vbCrLf
            'End If

            If Log_Errori = "" Then
                Log_Errori = "Nessun errore riscontrato."
            End If

            FileLog = Intestazione & Log_Errori & vbCrLf & vbCrLf & vbCrLf

            NomeFileLog = "AGREA_Import" & "_" & Data & "_" & Ora & "_Errori"

            GestioneFile.CreaScriviFileSovrascrivi_RitornaPath(FileLog, Path_CartellaFileLog, NomeFileLog, Path_FileLog, Str_Errore, "txt")

            If Str_Errore <> "" Then
                Messaggio &= "Errore nel salvataggio del Log di errori: " & Str_Errore & vbCrLf
            End If

        Else
            Messaggio &= "Si è verificato un errore durante il salvataggio della cartella dei log: " & Str_Errore & vbCrLf
        End If



    End Sub

    Private Function Str_ParticelleDuplicate() As Object
        Throw New NotImplementedException
    End Function



End Class
