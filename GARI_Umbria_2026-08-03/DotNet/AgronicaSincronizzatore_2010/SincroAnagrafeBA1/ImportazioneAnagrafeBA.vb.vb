Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi



Public Class ImportazioneAnagrafeBA

    'ID task
    Private ID_Tabella_PivaSuperuser As String
    Private ID_Tabella_Id_Servizio As enum_Id_Servizio
    Private ID_Tabella_Tipo_Sincro As enum_Tipi_Servizi_Background
    Private ID_Tabella_Id_Riga As Integer

    'Private Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio
    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    'Oggetti utilizzati
    Private Imprese_Read As AgronicaCoreAnagrafeDAL.Imprese_Read
    Private GerarchiaImprese_R As AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
    Private Imprese_Codici_Read As AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

    Private Utenti_Read As AgronicaCoreUtentiDAL.Utenti_Read
    Private Utenti_CodiciGiasPRO_R As AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R

    Private Configurazione_Siti_R As AgronicaCoreVarieDAL.Configurazione_Siti_R
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    Private objLog As AgronicaCoreDataProvider.LogProvider

    Private Allegati_Documenti_R As AgronicaCoreAnagrafeDAL.Allegati_Documenti_R

    Private objSchedeProc As AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_R
    Private objSchedeProc_W As AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_W

    Dim Reg_Impianti_Programmazioni_R As AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R

    'Da ricavare da Configurazione_Servizio
    Private SuperUserUsername As String
    Private SuperUserPassword As String
    Private SuperUserPiva As String
    Private Codice_Chiave_Cliente As Integer
    Private Piva_Padre As String
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    'Da ricavare da Configurazione_Servizio.parametri_extra
    Private annoPrecedente As Boolean = False
    Private Importa_PianoColturale As Boolean
    Private Importa_Anagrafica As Boolean
    Private Importa_Condizionalita As Boolean
    Private RegolamentoCod As Integer
    Private Aggiorna_Solo_Se_Fascicolo_Nuovo As Boolean
    Private Utente_BA_Username As String
    Private Utente_BA_CF As String
    Private SSAWS_Uri As String
    Private CUAA_Test As String
    Private FascicoloWS_Uri As String
    Private DataDa As Integer
    Private DataA As Integer
    Private Ribalta_Automatico As Boolean
    Private Ribalta_Limiti As Boolean
    Private Modifica_TitoloPossesso As Boolean

    'Parametri fissi impostati dalla classe
    Private ProgressivoGIAS As String
    Private LinkWSImportaGIAS As String

    'Variabili
    Private STR_XmlDoc As String
    Private STR_XmlDocP As String

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Configurazione_Servizio = _Configurazione_Servizio leggo solo i parametri che mi servono 
        ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti

        'Ricavo i parametri extra :
        RicavoIParametriExtra(_Configurazione_Servizio)

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

    End Sub



#Region "Importazione"




    Public Function ImportazioneMassivaDaAnagrafeBA(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        logga("ImportazioneMassivaDaAnagrafeBA")

        If LinkWSImportaGIAS = "" Then
            Messaggio_di_Ritorno_Opzionale = "Importazione NON eseguita, LinkWSImportaGIAS non è impostato in Configurazione_Siti"
            logga(Messaggio_di_Ritorno_Opzionale)
            Return False
        End If

        Try
            Dim msg As String = ""

            If _1_ImportaTutto(msg) Then
                Messaggio_di_Ritorno_Opzionale = "Importazione completata con successo. " & vbCrLf & vbCrLf & msg
                logga(Messaggio_di_Ritorno_Opzionale)
                Return True
            Else
                Messaggio_di_Ritorno_Opzionale = "Importazione NON completata con successo. " & vbCrLf & vbCrLf & msg
                logga(Messaggio_di_Ritorno_Opzionale)
                Return False
            End If


        Catch ex As Exception

            Messaggio_di_Ritorno_Opzionale = ex.Message
            Return False

        End Try

        Return False

    End Function



    Private Function _1_ImportaTutto(ByRef msg As String) As Boolean

        Dim CuaaCorrDaUsareNelTryCatch As String = ""


        Dim Sa_Cod As Integer
        Dim res As Boolean = True
        msg = ""
        Dim AziendeProcessate As Integer = 0
        Dim AziendeLetteCorrettamenteDaWS As Integer = 0
        Dim AziendeDaAggionarePercheFascicoloNuovo As Integer = 0
        Dim AziendeAggiornateSuGias As Integer = 0
        Dim AziendeConProblemiAggironamentoSuGias As Integer = 0
        Dim AziendeConProblemiLetturaDaWS As Integer = 0

        Dim str2 As String = ""

        Dim cuaas As New Hashtable

        'metto un try catch in generico in modo che nel caso di eccezioni particolari
        'durante la preparazione esca e notifichi l'errore e se ha processato comunque delle aziende
        Try


            _2_leggiAziendeECuaa(cuaas)
            ' _2bis_leggiAziendePresentiSIGPA(cuaas)

            If cuaas.Count = 0 Then
                msg &= " -Nessun cuaa trovato."
                Return False
            End If

            Dim DtPV As DataTable = Nothing

            If Ribalta_Limiti Then
                'PER LIMITI AZOTO!!!!!!!!!!!!!
                'Leggo le particelle vulnerabili e le fasce
                Try
                    Dim objPV As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
                    DtPV = objPV.Leggi("", "", "", 0, 0, "", 0, "", "", ObjParametri_Server)
                Catch ex As Exception
                End Try
            End If

            For Each cuaa As DictionaryEntry In cuaas

                Dim msgp As String = ""
                If ControllaPeriodoEsecuzione(msgp) = False Then
                    str2 = "    INTERROTTO: " & msgp & vbCrLf & str2 & vbCrLf
                    res = False
                    Exit For
                End If

                AziendeProcessate += 1
                Sa_Cod = 0
                'DtCondizionalita = Nothing
                'DtParticelle = Nothing
                STR_XmlDoc = ""
                STR_XmlDocP = ""
                Dim msgCarica As String = ""
                Dim msgImporta As String = ""
                Dim Azienda_Con_Fascicolo_Nuovo = False

                'Deve ritornare true se non ci sono stati errori di qualsiasi genere.
                'Se ritorna true la sincronizzazione devo farla solo se (nel caso Aggiorna_Solo_Se_Fascicolo_Nuovo=true):
                '   Azienda_Con_Fascicolo_Nuovo = true
                '           cioè il fascicolo letto è nuovo, quindi aggirono catasto e anagrafica e l'ho anche salvato nella funzione
                cuaaCorrDaUsareNelTryCatch = cuaa.Key

                'metto un try catch in ogni ciclo in modo che nel caso di eccezioni particolari prosegua
                'con l'esecuzione delle altre pive
                Try

                    Dim Msgstr As String = " PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & "  ------------INIZIO---LOOP--" & AziendeProcessate & "----------------------------"
                    logga(Msgstr)
                    'If cuaa.Key <> "GBTLBN69H08L117N" Then
                    '    Continue For
                    'End If

                    If _3_CaricaDati_WS_BA(cuaa.Value, cuaa.Key, DtPV, _
                                           msgCarica, _
                                           AziendeAggiornateSuGias, _
                                           AziendeConProblemiAggironamentoSuGias) Then

                        AziendeLetteCorrettamenteDaWS += 1

                        'Se la variabile in configurazione_servizio Aggiorna_Solo_Se_Fascicolo_Nuovo è true
                        'devo fare l'aggiornamento solo se il fascicolo è nuovo, 
                        'leggendo la variabile restituita Azienda_Con_Fascicolo_Nuovo 
                        If Aggiorna_Solo_Se_Fascicolo_Nuovo AndAlso Azienda_Con_Fascicolo_Nuovo _
                            OrElse (Not Aggiorna_Solo_Se_Fascicolo_Nuovo) Then

                            AziendeDaAggionarePercheFascicoloNuovo += 1
                            msgImporta = ""

                        Else

                            'azienda da non aggiornare
                            'Msgstr = "AZIENDA DA NON AGGIORNARE"
                            'Msgstr = " PIVA: " & cuaa.Key & ", CUAA:" & cuaa.Value & "  -  " & Msgstr
                            'logga(Msgstr)

                        End If


                    Else
                        AziendeConProblemiLetturaDaWS += 1
                        Dim id As String = CStr(AziendeConProblemiLetturaDaWS).PadLeft(4, "-")

                        str2 &= "    -" & id & ") Lettura dati da WS BA fallita per la PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & ", n. az. processata: " & AziendeProcessate & "; Dettagli:  (" & msgCarica & ")." & vbCrLf
                        res = False


                        Msgstr = "Lettura dati da WS BA fallita; n. az. processata: " & AziendeProcessate & "; Dettagli: (" & msgCarica & ")"
                        Msgstr = " PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & "  -  " & Msgstr
                        logga(Msgstr)


                    End If


                    Msgstr = " PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & "  ------------FINE-----LOOP--" & AziendeProcessate & "----------------------------"
                    logga(Msgstr)


                Catch ex As Exception
                    'metto un try catch in ogni ciclo in modo che nel caso di eccezioni particolari prosegua
                    'con l'esecuzione delle altre pive
                    AziendeConProblemiLetturaDaWS += 1
                    Dim id As String = CStr(AziendeConProblemiLetturaDaWS).PadLeft(4, "-")
                    str2 &= "    -" & id & ") ECCEZIONE NON PREVISTA durante il processamento della PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & ", n. az. processata: " & AziendeProcessate & "; Messaggio da CaricaDati_WS_BA:  (" & msgCarica & "); Messaggio da Importa:  (" & msgImporta & ") - ECCEZIONE:  (" & ex.Message & ")." & vbCrLf
                    res = False

                    Dim Msgstr As String
                    Msgstr = "ECCEZIONE NON PREVISTA durante il processamento; n. az. processata: " & AziendeProcessate & "; Messaggio da CaricaDati_WS_BA:  (" & msgCarica & "); Messaggio da Importa:  (" & msgImporta & ") - ECCEZIONE:  (" & ex.Message & ")"
                    Msgstr = " PIVA: " & cuaa.Value & ", CUAA:" & cuaa.Key & "  -  " & Msgstr
                    logga(Msgstr)

                End Try



            Next



        Catch ex As Exception

            'metto un try catch in generico in modo che nel caso di eccezioni particolari
            'durante la preparazione esca e notifichi l'errore e se ha processato comunque delle aziende
            str2 &= "    ---ECCEZIONE NON PREVISTA durente la richiesta, azienda processata numero: " & AziendeProcessate & ", ultimo CUAA: " & CuaaCorrDaUsareNelTryCatch & "; ECCEZIONE:  (" & ex.Message & ")." & vbCrLf
            logga(ex.Message)
            res = False


            Dim Msgstr As String
            Msgstr = "ECCEZIONE NON PREVISTA durante il processamento; n. az. processata: " & AziendeProcessate & "; ultima CUAA: " & CuaaCorrDaUsareNelTryCatch & "; ECCEZIONE:  (" & ex.Message & ")"
            Msgstr = " CUAA: " & CuaaCorrDaUsareNelTryCatch & ", PIVA:  ?   -  " & Msgstr
            logga(Msgstr)


        End Try


        Dim str As String = ""
        str &= "  Aziende totali:________________________" & cuaas.Count & vbCrLf
        str &= "  Aziende processate:____________________" & AziendeProcessate & vbCrLf
        str &= "  Aziende lette correttamente da WS:_____" & AziendeLetteCorrettamenteDaWS & vbCrLf
        str &= "  Aziende con fascicolo nuovo da WS:_____" & AziendeDaAggionarePercheFascicoloNuovo & vbCrLf
        str &= "  Aziende aggiornate su GIAS:____________" & AziendeAggiornateSuGias & vbCrLf
        str &= "  Errori Lettura da WS:__________________" & AziendeConProblemiLetturaDaWS & vbCrLf
        str &= "  Errori Aggiornamenti su GIAS:__________" & AziendeConProblemiAggironamentoSuGias & vbCrLf

        msg = str & vbCrLf & "Per i dettagli approdonditi fare riferimento al file " & LogFileName & " in " & LogDirectory & "." & vbCrLf & vbCrLf & _
            " Dettagli parziali sulle importazioni non riuscite: " & vbCrLf & str2

        Return res

    End Function



    Private Sub _2_leggiAziendeECuaa(ByRef cuaas As Hashtable)

        Dim Elenco_Imprese As String = ""
        If Not String.IsNullOrEmpty(CUAA_Test) Then
            Elenco_Imprese = CUAA_Test
        Else
            GerarchiaImprese_R.LeggiFigliNodoGerarchiaImprese(Piva_Padre, Elenco_Imprese, ObjParametri_Server)
        End If


        Dim ListaImprese As String() = Elenco_Imprese.Split(",")

        Dim piva As String
        Dim cuaa As String
        For i = 0 To ListaImprese.Length - 1
            piva = ListaImprese(i).Replace("'", "").Trim
            cuaa = Imprese_Codici_Read.Leggi_Codice_from_Imprese_Codici(piva, AgronicaCoreDataProvider.CostantiPersonalizzate.CA_CUAA, ObjParametri_Server)
            If cuaa <> "" Then
                If cuaas(cuaa) Is Nothing Then
                    cuaas.Add(cuaa, piva)
                End If
            End If
        Next

    End Sub


    Private Sub _2bis_leggiAziendePresentiSIGPA(ByRef cuaas As Hashtable)

        Dim DtImprese As New DataTable
        DtImprese = objSchedeProc.LeggiAziende_PresentiSIGPA_NONGias(ObjParametri_Server)

        Dim cuaa As String
        For i = 0 To DtImprese.Rows.Count - 1
            cuaa = DtImprese.Rows(i).Item("cuaa_azienda").Trim
            If cuaa <> "" AndAlso Not cuaas.ContainsKey(cuaa) Then
                cuaas.Add(cuaa, "")
            End If
        Next

    End Sub


    ''Deve ritornare true se non ci sono stati errori di qualsiasi genere.
    ''Se ritporna true la sincronizzazione devo farla solo se:
    ''   Azienda_Con_Fascicolo_Nuovo = true
    ''           cioè il fascicolo letto è nuovo, quindi aggirono catasto e anagrafica e l'ho anche salvato nella funzione
    'Private Function _3_CaricaDati_WS_BA_OLD(ByVal PIVA As String, _
    '                                  ByVal CUAA As String, _
    '                                  ByRef Sa_Cod As Integer, _
    '                                  ByRef DtCondizionalita As DataTable, _
    '                                  ByRef DtParticelle As DataTable, _
    '                                  ByRef msgCarica As String, _
    '                                  ByRef Azienda_Con_Fascicolo_Nuovo As Boolean, _
    '                                  ByRef DataValidazione As Date) As Boolean

    '    Azienda_Con_Fascicolo_Nuovo = False
    '    Sa_Cod = 0
    '    DtCondizionalita = New DataTable
    '    DtParticelle = New DataTable

    '    Dim EsisteImpresa As Boolean = True

    '    If Codice_Chiave_Cliente = 0 Then
    '        Throw New Exception("Configurazione_Servizio.Id_Cod_Cliente =  '' ")
    '    End If

    '    If PIVA = "" Then
    '        msgCarica &= "Inserire PIVA dell'Impresa che si desidera importare!"
    '        Return False
    '    Else
    '        EsisteImpresa = True
    '    End If

    '    If CUAA = "" Then
    '        msgCarica &= "Inserire il CUAA dell'Impresa che si desidera importare!"
    '        Return False
    '    Else
    '        CUAA = CUAA.ToUpper
    '    End If


    '    '--------------------------------------------
    '    'verifico se l'impresa è presente su Gias
    '    Dim data_modifica_temp As Date
    '    Dim piva_padre As String
    '    Dim socio_temp As String
    '    If Not Imprese_Read.Esiste_Impresa(PIVA, 0, data_modifica_temp, piva_padre, socio_temp, "", ObjParametri_Server) Then
    '        msgCarica &= "impresa non è presente su Gias"
    '        EsisteImpresa = False
    '        Return False
    '    End If


    '    Dim i, j, x As Integer
    '    Dim strErr As String = ""

    '    Dim ID_Azienda As String = ""
    '    Dim Detentore_Fascicolo As String = ""
    '    Dim strFiltroPadri As String = ""

    '    Dim RagSoc As String = ""
    '    Dim Indirizzo As String = ""
    '    Dim Cap As String = ""
    '    Dim Provincia As String = ""
    '    Dim Comune As String = ""
    '    Dim Istat_Provincia As String = ""
    '    Dim Istat_Comune As String = ""
    '    Dim Cod_Belfiore As String = ""
    '    Dim Localita As String = ""


    '    Dim Prov As String = ""
    '    Dim Com As String = ""
    '    Dim Sezione As String = ""
    '    Dim Foglio As String = ""
    '    Dim Numero As String = ""
    '    Dim strNumero As String = ""
    '    Dim NumeroStringa As String = ""
    '    Dim Subalterno As String = ""
    '    Dim strSezione As String = ""
    '    Dim strSubalterno As String = ""
    '    Dim Ettari As Double
    '    Dim Are As Double
    '    Dim Centiare As Double
    '    Dim supCatasto As Double = 0
    '    Dim supConduzione As Double = 0
    '    Dim strMacrousi As String = ""
    '    Dim strUtilizzi As String = ""
    '    Dim TitoloPossesso As String
    '    Dim TitoloPossessoDes As String
    '    Dim Inizio_Possesso As Date
    '    Dim Fine_Possesso As Date
    '    Dim Inizio_Possesso_Old As Date
    '    Dim Fine_Possesso_Old As Date

    '    Dim Legale_Rappresentante As String = "#"
    '    Dim Legale_Rappresentante_Cognome As String = "#"
    '    Dim Legale_Rappresentante_Nome As String = "#"
    '    Dim Legale_Rappresentante_CF As String = "#"
    '    Dim Legale_Rappresentante_Sesso As String = "#"
    '    Dim Legale_Rappresentante_Indirizzo As String = "#"
    '    Dim Legale_Rappresentante_Frazione As String = "#"
    '    Dim Legale_Rappresentante_Cap As String = "#"
    '    Dim Legale_Rappresentante_Comune As String = "#"
    '    Dim Legale_Rappresentante_Provincia As String = "#"
    '    Dim Legale_Rappresentante_Stato As String = "#"
    '    Dim Legale_Rappresentante_Istat_Comune As String = "#"
    '    Dim Legale_Rappresentante_Istat_Provincia As String = "#"
    '    Dim Legale_Rappresentante_Data_Nascita As String = "#"
    '    Dim Legale_Rappresentante_Comune_Nascita As String = "#"
    '    Dim Legale_Rappresentante_Provincia_Nascita As String = "#"
    '    Dim Legale_Rappresentante_Istat_Comune_Nascita As String = "#"
    '    Dim Legale_Rappresentante_Istat_Provincia_Nascita As String = "#"
    '    Dim Legale_Rappresentante_Rubrica1 As String = "#"
    '    Dim Legale_Rappresentante_Rubrica2 As String = "#"
    '    Dim Legale_Rappresentante_Rubrica3 As String = "#"
    '    Dim Legale_Rappresentante_Rubrica4 As String = "#"
    '    Dim Legale_Rappresentante_Rubrica5 As String = "#"

    '    Dim Presente_pivazzz As Boolean = False
    '    Dim Presente_Gias As Boolean = False
    '    Dim Presente_Anagrafe As Boolean = False
    '    Dim Presente_Fascicolo As Boolean = False
    '    Dim Persona As Boolean = False
    '    Dim RispostaWSAnagrafe As Boolean = False
    '    Dim RispostaWSFascicolo As Boolean = False

    '    Dim TipoOperazione As Integer = enum_TipoOperazioneDB.Scrittura


    '    Dim XmlDoc As New System.Xml.XmlDocument

    '    Dim XmlUtente As System.Xml.XmlElement
    '    Dim XmlImpresa As System.Xml.XmlElement
    '    Dim XmlFascicolo As System.Xml.XmlElement
    '    Dim XmlCentro As System.Xml.XmlElement
    '    Dim XmlFabbricato As XmlElement
    '    Dim XmlDocP As New System.Xml.XmlDocument
    '    Dim XmlUtenteP As System.Xml.XmlElement
    '    Dim XmlTestata As System.Xml.XmlElement
    '    Dim XmlFascicoloP As System.Xml.XmlElement
    '    Dim objXML As New AgronicaCoreXML.AnagrafeXML




    '    '----------------------------------------------
    '    'verifico se l'impresa è presente su Anagrafe
    '    '----------------------------------------------

    '    'Dim ws_AnaColdi As New SSAClient
    '    Dim SSAWS_name As String = "SSAWS_SSAHttpPort"
    '    ' SSAWS_Uri  = "http://web_esb.coldiretti.it:80/SSAMMWeb/sca/SSAWS"
    '    Dim ws_AnaColdi As New SincroAnagrafeBA1.SSAWS_SSAHttpService(SSAWS_Uri)


    '    Dim ws_AnaResponse As SincroAnagrafeBA1.cercaAnagraficaResponse
    '    Dim gA As New SincroAnagrafeBA1.cercaAnagrafica


    '    ' FascicoloWS_Uri  = "http://web_esb.coldiretti.it:80/FascicoloMMWeb/sca/FascicoloWS"
    '    Dim FascicoloWS_name As String = "FascicoloWS_FascicoloHttpPort"
    '    Dim FascicoloWS_contract As String = "Fascicolo"

    '    Dim ContractDescription As New System.ServiceModel.Description.ContractDescription(FascicoloWS_contract)

    '    ContractDescription = System.ServiceModel.Description.ContractDescription.GetContract(GetType(SincroAnagrafeBA1.NsFascicolo.IFascicolo), GetType(SincroAnagrafeBA1.NsFascicolo.Fascicolo))


    '    'If True Then
    '    '    'NON FUNZIONA PERCHé SOAP 12
    '    '    Dim WSHttpBinding As New System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None)
    '    '    WSHttpBinding.Name = "FascicoloWS_FascicoloHttpBinding"
    '    '    WSHttpBinding.CloseTimeout = New TimeSpan(0, 1, 0)
    '    '    WSHttpBinding.OpenTimeout = New TimeSpan(0, 1, 0)
    '    '    WSHttpBinding.ReceiveTimeout = New TimeSpan(0, 10, 0)
    '    '    WSHttpBinding.SendTimeout = New TimeSpan(1, 0, 0)
    '    '    WSHttpBinding.AllowCookies = False
    '    '    WSHttpBinding.BypassProxyOnLocal = False
    '    '    WSHttpBinding.HostNameComparisonMode = ServiceModel.HostNameComparisonMode.StrongWildcard
    '    '    WSHttpBinding.MaxBufferPoolSize = 2097152
    '    '    WSHttpBinding.MessageEncoding = ServiceModel.WSMessageEncoding.Text
    '    '    WSHttpBinding.TextEncoding = System.Text.Encoding.UTF8
    '    '    WSHttpBinding.UseDefaultWebProxy = True
    '    '    WSHttpBinding.ReaderQuotas.MaxDepth = 32
    '    '    WSHttpBinding.ReaderQuotas.MaxStringContentLength = 8192
    '    '    WSHttpBinding.ReaderQuotas.MaxArrayLength = 16384
    '    '    WSHttpBinding.ReaderQuotas.MaxBytesPerRead = 4096
    '    '    WSHttpBinding.ReaderQuotas.MaxNameTableCharCount = 16384
    '    '    WSHttpBinding.Security.Mode = ServiceModel.SecurityMode.None
    '    '    WSHttpBinding.Security.Transport.ClientCredentialType = ServiceModel.HttpClientCredentialType.None
    '    '    WSHttpBinding.Security.Transport.ProxyCredentialType = ServiceModel.HttpClientCredentialType.None
    '    '    WSHttpBinding.Security.Transport.Realm = ""
    '    '    WSHttpBinding.Security.Message.ClientCredentialType = ServiceModel.MessageCredentialType.UserName
    '    '    WSHttpBinding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default
    '    '    'WSHttpBinding.EnvelopeVersion = 12""
    '    'End If

    '    'usare BASIC perchè EnvelopeVersion = 11
    '    'Copiato da webconfig sincronizzatore
    '    Dim basicHttpBinding As New System.ServiceModel.BasicHttpBinding()
    '    basicHttpBinding.Name = "FascicoloWS_FascicoloHttpBinding"
    '    basicHttpBinding.CloseTimeout = New TimeSpan(0, 1, 0)
    '    basicHttpBinding.OpenTimeout = New TimeSpan(0, 1, 0)
    '    basicHttpBinding.ReceiveTimeout = New TimeSpan(0, 10, 0)
    '    basicHttpBinding.SendTimeout = New TimeSpan(1, 0, 0)
    '    basicHttpBinding.AllowCookies = False
    '    basicHttpBinding.BypassProxyOnLocal = False
    '    basicHttpBinding.HostNameComparisonMode = ServiceModel.HostNameComparisonMode.StrongWildcard
    '    basicHttpBinding.MaxBufferSize = 2097152
    '    basicHttpBinding.MaxReceivedMessageSize = 2097152
    '    basicHttpBinding.MaxBufferPoolSize = 2097152
    '    basicHttpBinding.MessageEncoding = ServiceModel.WSMessageEncoding.Text
    '    basicHttpBinding.TextEncoding = System.Text.Encoding.UTF8
    '    basicHttpBinding.TransferMode = ServiceModel.TransferMode.Buffered
    '    basicHttpBinding.UseDefaultWebProxy = True

    '    basicHttpBinding.ReaderQuotas.MaxDepth = 32
    '    basicHttpBinding.ReaderQuotas.MaxStringContentLength = 8192
    '    basicHttpBinding.ReaderQuotas.MaxArrayLength = 16384
    '    basicHttpBinding.ReaderQuotas.MaxBytesPerRead = 4096
    '    basicHttpBinding.ReaderQuotas.MaxNameTableCharCount = 16384

    '    basicHttpBinding.Security.Mode = ServiceModel.SecurityMode.None

    '    basicHttpBinding.Security.Transport.ClientCredentialType = ServiceModel.HttpClientCredentialType.None
    '    basicHttpBinding.Security.Transport.ProxyCredentialType = ServiceModel.HttpClientCredentialType.None
    '    basicHttpBinding.Security.Transport.Realm = ""

    '    basicHttpBinding.Security.Message.ClientCredentialType = ServiceModel.BasicHttpMessageCredentialType.UserName
    '    basicHttpBinding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default


    '    Dim EndpointAddress As New System.ServiceModel.EndpointAddress(FascicoloWS_Uri)
    '    Dim Endpoint As New System.ServiceModel.Description.ServiceEndpoint(ContractDescription, basicHttpBinding, EndpointAddress)
    '    Endpoint.Name = FascicoloWS_name

    '    'Dim ws_FascicoloColdi As New SincroAnagrafeBA1.NsFascicolo.FascicoloClient(Endpoint)
    '    'Dim ws_fasciResponse As SincroAnagrafeBA1.NsFascicolo.getFascicoloResponse
    '    'Dim gF As New SincroAnagrafeBA1.NsFascicolo.getFascicolo

    '    Dim ws_FascicoloColdi As New SincroAnagrafeBA1.FascicoloWS_FascicoloHttpService(FascicoloWS_Uri)
    '    Dim ws_fasciResponse As SincroAnagrafeBA1.getFascicoloNewResponse
    '    Dim gF As New SincroAnagrafeBA1.getFascicoloNew

    '    Dim gS As New SincroAnagrafeBA1.getSchedeFascicolo
    '    Dim Schede() As SincroAnagrafeBA1.SchedaValidazione

    '    Dim NomeRoutine As String = "Cerca Anagrafica"
    '    Dim MsgOK As String = ""

    '    gA.in = New SincroAnagrafeBA1.MsgGetAnagrafica
    '    gA.in.intestazione = New SincroAnagrafeBA1.Intestazione
    '    gA.in.intestazione.operatore = New SincroAnagrafeBA1.Operatore

    '    gA.in.intestazione.codDipartimentale = "AGR_RM"
    '    gA.in.intestazione.operatore.codice = Utente_BA_Username   '"lorenzo.belcapo"
    '    gA.in.intestazione.operatore.cf = Utente_BA_CF     '"BLCLNZ74R30A577M"
    '    gA.in.cf = CUAA

    '    'gF.cuaa = CUAA
    '    'gF.annoPrecedente = annoPrecedente
    '    'gF.filtro = SincroAnagrafeBA1.NsFascicolo.FiltroGetFascicolo.all

    '    gS.cuaa = CUAA
    '    gS.dataDa = DataDa
    '    gS.dataA = DataA


    '    Dim SchedaValidazione As String = ""
    '    DataValidazione = CostantiPersonalizzate.AGRODATAINIZIO
    '    Dim DataAperturaFascicolo As Date = CostantiPersonalizzate.AGRODATAINIZIO
    '    Dim DataChiusuraFascicolo As Date = CostantiPersonalizzate.AGRODATAFINE
    '    Dim DataInizioMandato As Date = CostantiPersonalizzate.AGRODATAINIZIO
    '    Dim DataFineMandato As Date = CostantiPersonalizzate.AGRODATAFINE


    '    Try


    '        MsgOK = "Web Service Anagrafe: Invio Richiesta"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '        logga(MsgOK)

    '        ws_AnaResponse = ws_AnaColdi.cercaAnagrafica(gA)

    '        MsgOK = "Web Service Anagrafe: Risposta Ricevuta"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '        logga(MsgOK)


    '        RispostaWSAnagrafe = True

    '    Catch ex As Exception

    '        RispostaWSAnagrafe = False

    '        MsgOK = "Web Service Anagrafe ha restituito l'errore: " & ex.Message & " "
    '        If ex.Message = "-1" Then
    '            MsgOK &= vbCrLf & "Verificare le credenziali con cui si è entrati su Gias."
    '        End If
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '        logga(MsgOK)

    '        Dim strEr As String
    '        strEr = ex.Message

    '        msgCarica &= "Web Service Anagrafe ha restituito l'errore: " & ex.Message
    '        Return False

    '    End Try


    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    'controllo coerenza dei dati ricevuti da ws anagrafe
    '    If IsNothing(ws_AnaResponse) Then
    '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse è nothing per la PIVA " & PIVA & "."
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
    '        logga(MsgOK)
    '        msgCarica &= erroreDAti
    '        Return False
    '    End If
    '    If IsNothing(ws_AnaResponse.output) Then
    '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output è nothing per la PIVA " & PIVA & "."
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
    '        logga(MsgOK)
    '        msgCarica &= erroreDAti
    '        Return False
    '    End If
    '    If IsNothing(ws_AnaResponse.output.anagrafica) Then
    '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output.anagrafica è nothing per la PIVA " & PIVA & "."
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
    '        logga(MsgOK)
    '        msgCarica &= erroreDAti
    '        Return False
    '    End If
    '    If IsNothing(ws_AnaResponse.output.anagrafica.impresa) Then
    '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output.anagrafica.impresa è nothing per la PIVA " & PIVA & "."
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
    '        logga(MsgOK)
    '        msgCarica &= erroreDAti
    '        Return False
    '    End If
    '    If ws_AnaResponse.output.anagrafica.impresa.piva <> PIVA Then
    '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, in ws_AnaResponse la PIVA non corrisponde con quella richiesta: ('" & ws_AnaResponse.output.anagrafica.impresa.piva & "'<>" & PIVA & ")"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
    '        logga(MsgOK)
    '        msgCarica &= erroreDAti
    '        Return False

    '    End If
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------
    '    '------------------------------------------------------------------------------------------------------------

    '    Dim N_Schede As Integer = 0
    '    Dim s As Integer = 0
    '    Dim Dt_Schede As New DataTable
    '    Dim Dt_SchedeOrd As New DataTable
    '    Dim DrScheda As DataRow

    '    Dt_Schede.Columns.Add(New DataColumn("Scheda", GetType(String)))
    '    Dt_Schede.Columns.Add(New DataColumn("Data", GetType(Integer)))
    '    Dt_Schede.Columns.Add(New DataColumn("DataFine", GetType(Integer)))

    '    Try

    '        MsgOK = "Web Service Fascicolo: Invio Richiesta Schede"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '        logga(MsgOK)

    '        'ws_fasciResponse = ws_FascicoloColdi.getFascicolo(gF)

    '        Schede = ws_FascicoloColdi.getSchedeFascicolo(gS)

    '        If Not Schede Is Nothing Then

    '            N_Schede = Schede.Length

    '            For s = 0 To Schede.Length - 1

    '                DrScheda = Dt_Schede.NewRow

    '                DrScheda.Item("Scheda") = Schede(s).numeroScheda
    '                DrScheda.Item("Data") = Schede(s).dataScheda

    '                Dt_Schede.Rows.Add(DrScheda)

    '            Next


    '        End If



    '        MsgOK = "Web Service Fascicolo: Risposta Schede Ricevuta"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Ricevute " & N_Schede & " Schede di validazione -  " & MsgOK
    '        logga(MsgOK)

    '    Catch ex As Exception

    '        Dim strEr As String
    '        strEr = ex.Message

    '        MsgOK = "Web Service Fascicolo Schede ha restituito l'errore: " & ex.Message & " !"
    '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '        logga(MsgOK)

    '        msgCarica &= "Web Service Fascicolo Schede: " & ex.Message & "."
    '        Return False

    '    End Try


    '    If Dt_Schede.Rows.Count > 0 Then

    '        Dim dataView As New DataView(Dt_Schede)
    '        dataView.Sort = "Data"
    '        Dt_SchedeOrd = dataView.ToTable

    '        If Dt_Schede.Rows.Count > 1 Then
    '            For s = 0 To Dt_SchedeOrd.Rows.Count - 2
    '                Dt_SchedeOrd.Rows(s).Item("DataFine") = Dt_SchedeOrd.Rows(s + 1).Item("Data")
    '            Next
    '        End If

    '    End If

    '    If Dt_SchedeOrd.Rows.Count <> 0 Then

    '        For s = 0 To Dt_SchedeOrd.Rows.Count - 1

    '            Try

    '                MsgOK = "Web Service Fascicolo: Invio Richiesta Scheda"
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
    '                logga(MsgOK)

    '                gF.cuaa = CUAA
    '                gF.filtro = FiltroGetFascicolo.all
    '                gF.numSchedaValidaz = Schede(s).numeroScheda

    '                ws_fasciResponse = ws_FascicoloColdi.getFascicoloNew(gF)

    '                MsgOK = "Web Service Fascicolo: Risposta Scheda Ricevuta"
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
    '                logga(MsgOK)

    '                RispostaWSFascicolo = True

    '            Catch ex As Exception

    '                RispostaWSFascicolo = False

    '                Dim strEr As String
    '                strEr = ex.Message

    '                MsgOK = "Web Service Fascicolo Scheda ha restituito l'errore: " & ex.Message & " !"
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
    '                logga(MsgOK)

    '                msgCarica &= "Web Service Fascicolo Scheda: " & ex.Message & "."
    '                Return False

    '            End Try


    '            '------------------------------------------------------------------------------------------------------------
    '            '------------------------------------------------------------------------------------------------------------
    '            '------------------------------------------------------------------------------------------------------------
    '            'controllo coerenza dei dati ricevuti da ws anagrafe
    '            If IsNothing(ws_fasciResponse) Then
    '                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse è nothing per la PIVA " & PIVA & "."
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '                logga(MsgOK)
    '                msgCarica &= erroreDAti
    '                Return False
    '            End If
    '            If IsNothing(ws_fasciResponse.out) Then
    '                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out è nothing per la PIVA " & PIVA & "."
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '                logga(MsgOK)
    '                msgCarica &= erroreDAti
    '                Return False
    '            End If
    '            If IsNothing(ws_fasciResponse.out.fascicolo) Then
    '                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out.fascicolo è nothing per la PIVA " & PIVA & "."
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '                logga(MsgOK)
    '                msgCarica &= erroreDAti
    '                Return False
    '            End If
    '            If IsNothing(ws_fasciResponse.out.fascicolo.fascicolo) Then
    '                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out.fascicolo.fascicolo è nothing per la PIVA " & PIVA & "."
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '                logga(MsgOK)
    '                msgCarica &= erroreDAti
    '                Return False
    '            End If
    '            'If ws_fasciResponse.out.fascicolo.fascicolo.PartitaIVA <> PIVA Then
    '            '    Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, in ws_fasciResponse la PIVA non corrisponde: ('" & ws_fasciResponse.out.fascicolo.fascicolo.PartitaIVA & "'<>" & PIVA & ")."
    '            '    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '            '    logga(MsgOK)
    '            '    msgCarica &= erroreDAti
    '            '    Return False
    '            'End If
    '            If ws_fasciResponse.out.fascicolo.fascicolo.CUAA <> CUAA Then
    '                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati,  in ws_fasciResponse il cuaa non corrisponde: ('" & ws_fasciResponse.out.fascicolo.fascicolo.CUAA & "'<>" & CUAA & ")."
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
    '                logga(MsgOK)
    '                msgCarica &= erroreDAti
    '                Return False
    '            End If

    '            '------------------------------------------------------------------------------------------------------------
    '            '------------------------------------------------------------------------------------------------------------
    '            '------------------------------------------------------------------------------------------------------------


    '            Try

    '                If RispostaWSAnagrafe = True AndAlso _
    '                    Not ws_AnaResponse Is Nothing AndAlso _
    '                    Not ws_AnaResponse.output Is Nothing AndAlso _
    '                    Not ws_AnaResponse.output.anagrafica Is Nothing Then

    '                    Presente_Anagrafe = True

    '                    MsgOK = "Anagrafe presente!"
    '                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                    logga(MsgOK)


    '                    If RispostaWSFascicolo = True AndAlso _
    '                        Not ws_fasciResponse Is Nothing AndAlso _
    '                        Not ws_fasciResponse.out Is Nothing AndAlso _
    '                        Not ws_fasciResponse.out.fascicolo Is Nothing Then

    '                        Presente_Fascicolo = True

    '                        Detentore_Fascicolo = ws_fasciResponse.out.fascicolo.fascicolo.Detentore

    '                        If Detentore_Fascicolo <> "" AndAlso Detentore_Fascicolo.Length = 9 Then

    '                            Dim bEsisteUffZona As Boolean = False
    '                            Dim bEsisteUffProv As Boolean = False

    '                            Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '                            bEsisteUffZona = objImpresa.Esiste_Impresa("UZ" & Detentore_Fascicolo, 0, Nothing, Nothing, "", "", ObjParametri_Server)

    '                            If bEsisteUffZona Then
    '                                piva_padre = "UZ" & Detentore_Fascicolo
    '                            Else
    '                                bEsisteUffProv = objImpresa.Esiste_Impresa("UP" & Left(Detentore_Fascicolo, Detentore_Fascicolo.Length - 3) & "000", 0, Nothing, Nothing, "", "", ObjParametri_Server)
    '                                If bEsisteUffProv Then
    '                                    piva_padre = "UP" & Left(Detentore_Fascicolo, Detentore_Fascicolo.Length - 3) & "000"
    '                                Else
    '                                    piva_padre = SuperUserPiva
    '                                End If
    '                            End If

    '                            objImpresa = Nothing

    '                            If Not ws_fasciResponse.out.fascicolo.fascicolo.SchedaValidazione Is Nothing Then
    '                                SchedaValidazione = ws_fasciResponse.out.fascicolo.fascicolo.SchedaValidazione
    '                            End If
    '                            If Not ws_fasciResponse.out.fascicolo.fascicolo.DataSchedaValidazione Is Nothing Then
    '                                DataValidazione = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataSchedaValidazione))
    '                            End If
    '                            If Not ws_fasciResponse.out.fascicolo.fascicolo.DataSottMandato Is Nothing Then
    '                                DataInizioMandato = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataSottMandato))
    '                            End If
    '                            If Not ws_fasciResponse.out.fascicolo.fascicolo.DataAperturaFascicolo Is Nothing Then
    '                                DataAperturaFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataAperturaFascicolo))
    '                            End If
    '                            If Not ws_fasciResponse.out.fascicolo.fascicolo.DataChiusuraFascicolo Is Nothing Then
    '                                DataChiusuraFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataChiusuraFascicolo))
    '                            End If

    '                            If DataValidazione <> CostantiPersonalizzate.AGRODATAINIZIO Then
    '                                'label_fascicolo.InnerText = "Fascicolo N." & SchedaValidazione & " (Data Validazione " & DataValidazione.ToShortDateString & ")"
    '                            Else
    '                                'label_fascicolo.InnerText = "Fascicolo N." & SchedaValidazione & " (Data Validazione non presente)"
    '                            End If



    '                            '---------------------------------------------------------------------------------------------------------------------------------------
    '                            '---------------------------------------------------------------------------------------------------------------------------------------
    '                            '---------------------------------------------------------------------------------------------------------------------------------------
    '                            'Se la variabile
    '                            'Aggiorna_Solo_Se_Fascicolo_Nuovo =true
    '                            'allora l'aggiornamento lo devo fare solo se il fascicolo è nuovo, quindi
    '                            'devo controllare se il fascicolo è nuovo,
    '                            'se Azienda_Con_Fascicolo_Nuovo = true proseguo
    '                            'se Azienda_Con_Fascicolo_Nuovo = false esco con truecosì non faccio la serializzazione dell'oggetto e lascio il messaggio
    '                            If Aggiorna_Solo_Se_Fascicolo_Nuovo = False Then
    '                                'non faccio nulla
    '                                MsgOK = "Fascicolo ricevuto, lo aggiorno a prescindere che sia gia presente."
    '                                logga(MsgOK)
    '                            Else
    '                                If SchedaValidazione = "" Then
    '                                    MsgOK = "scheda validazione nulla"
    '                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                    logga(MsgOK)
    '                                    msgCarica &= "SchedaValidazione nulla"
    '                                    Return True
    '                                End If
    '                                'controllo se il fascicolo è presente
    '                                'ATTENZIONE, la Allegati_Documenti_Piva ha alcune volte il cuaa, altre il superuser, 
    '                                'quindi controllo solo il numero
    '                                If FascicoloPresente(SchedaValidazione) Then
    '                                    Azienda_Con_Fascicolo_Nuovo = False
    '                                    MsgOK = "Fascicolo: " & SchedaValidazione & "  gia presente, NON lo importo."
    '                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                    logga(MsgOK)
    '                                    msgCarica &= "Fascicolo gia presente:" & SchedaValidazione & ", lo importo."
    '                                    Return True
    '                                Else
    '                                    Azienda_Con_Fascicolo_Nuovo = True
    '                                    MsgOK = "Fascicolo: " & SchedaValidazione & " nuovo, lo importo."
    '                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                    logga(MsgOK)
    '                                    'proseguo
    '                                End If
    '                            End If

    '                            '---------------------------------------------------------------------------------------------------------------------------------------
    '                            '---------------------------------------------------------------------------------------------------------------------------------------
    '                            '---------------------------------------------------------------------------------------------------------------------------------------




    '                            Dim Path As String = ""

    '                            Path = DirectoryFileImportazioni & "\" & CUAA & "_" & SchedaValidazione & "_fascicolo.xml"

    '                            'se il fascicolo con questa scheda validazione esiste lo cancello e risalvo
    '                            'altrimenti lo salvo
    '                            If File.Exists(Path) = True Then
    '                                File.Delete(Path)
    '                            End If

    '                            SerializeObject(Path, ws_fasciResponse)

    '                            XmlFascicolo = objXML.Xml_Pubblico_Fascicolo(Path, _
    '                                                                         SchedaValidazione, _
    '                                                                      DataValidazione, _
    '                                                                      Detentore_Fascicolo, _
    '                                                                      DataInizioMandato, _
    '                                                                      DataFineMandato, _
    '                                                                      DataAperturaFascicolo, _
    '                                                                      DataChiusuraFascicolo, _
    '                                                                      XmlDoc)

    '                            XmlFascicoloP = objXML.Xml_Pubblico_Fascicolo(Path, _
    '                                                                         SchedaValidazione, _
    '                                                                      DataValidazione, _
    '                                                                      Detentore_Fascicolo, _
    '                                                                      DataInizioMandato, _
    '                                                                      DataFineMandato, _
    '                                                                      DataAperturaFascicolo, _
    '                                                                      DataChiusuraFascicolo, _
    '                                                                      XmlDocP)


    '                        Else

    '                            MsgOK = "Detentore_Fascicolo = '' or Detentore_Fascicolo.Length = 9 "
    '                            logga(MsgOK)
    '                            msgCarica &= "Dati non presenti(" & MsgOK & ")!"
    '                            Return False

    '                        End If

    '                    Else
    '                        'RispostaWSFascicolo = True AndAlso _
    '                        'Not ws_fasciResponse Is Nothing AndAlso _
    '                        'Not ws_fasciResponse.out Is Nothing AndAlso _
    '                        'Not ws_fasciResponse.out.fascicolo Is Nothing
    '                        MsgOK = "RispostaWSFascicolo=false o  ws_fasciResponse,ws_fasciResponse.out,ws_fasciResponse.out.fascicolo Is Nothing"
    '                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                        logga(MsgOK)

    '                        msgCarica &= "Dati non presenti(" & MsgOK & ")!"
    '                        Return False

    '                    End If


    '                    If Presente_Fascicolo = False Then
    '                        MsgOK = "Fascicolo NON presente!"
    '                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                        logga(MsgOK)
    '                    Else

    '                    End If




    '                    '---------------------------------------------------------------------------

    '                    'a seconda del tipo di anagrafica ho un oggetto diverso
    '                    'ditta individuale : ho sia i dati dell'impresa sia quelli del rappresentante
    '                    'società : ho solo i dati dell'impresa (non ho i tag del rappresentante)
    '                    '(persona : ho solo i dati anagrafici del legale (non ho il tag impresa))
    '                    Select Case ws_AnaResponse.output.anagrafica.codTipoAnagrafica

    '                        Case "DI", "S"   'ditta individuale, 'società


    '                            '*******************************************************************************************************************
    '                            '******   IMPRESA      *********************************************************************************************
    '                            '*******************************************************************************************************************

    '                            RagSoc = ws_AnaResponse.output.anagrafica.impresa.ragioneSociale

    '                            If ws_AnaResponse.output.anagrafica.impresa.piva = "" Then
    '                                'pivazzz non presente
    '                                Throw New Exception("Impossibile inserire l'impresa poichè manca la Partita IVA!")
    '                            End If

    '                            If PIVA <> ws_AnaResponse.output.anagrafica.impresa.piva Then
    '                                Throw New Exception("PIVA <> ws_AnaResponse.output.anagrafica.impresa.piva")
    '                            End If

    '                            ID_Azienda = ws_AnaResponse.output.anagrafica.identita.master

    '                            Cod_Belfiore = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.comune.codErariale

    '                            Indirizzo = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.toponimo & " " & _
    '                                        ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.descrizione & " " & _
    '                                        ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.civico & " "

    '                            Cap = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.cap


    '                            Localita = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.localita



    '                            '-----------------------------------------------
    '                            '------- UTENTE        -------------------------
    '                            '-----------------------------------------------
    '                            XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc, _
    '                                                                   SuperUserUsername, _
    '                                                                   SuperUserPassword, _
    '                                                                   ProgressivoGIAS)

    '                            XmlUtenteP = objXML.Xml_Pubblico_Utente(XmlDocP, _
    '                                                                   SuperUserUsername, _
    '                                                                   SuperUserPassword, _
    '                                                                   ProgressivoGIAS)

    '                            Comune = ""
    '                            Dim objIstat As New AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_R
    '                            Dim Dt_Istat As New DataTable

    '                            Dt_Istat = objIstat.Leggi("", _
    '                                                   "", _
    '                                                   "", _
    '                                                   "", _
    '                                                   "", _
    '                                                   Cod_Belfiore, _
    '                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
    '                                                     "", "", _
    '                                                     ObjParametri_Server)

    '                            If Not Dt_Istat Is Nothing AndAlso Dt_Istat.Rows.Count > 0 Then

    '                                Provincia = Dt_Istat.Rows(0).Item("Provincia_Long")
    '                                Comune = Dt_Istat.Rows(0).Item("Descrizione")
    '                                Istat_Provincia = Dt_Istat.Rows(0).Item("Pro_Cod_Istat")
    '                                Istat_Comune = Dt_Istat.Rows(0).Item("Com_Cod_Istat")

    '                                'If Presente_Fascicolo = False Then
    '                                '    ' Cmb_Padre.SelectedIndex = Cmb_Padre.Items.IndexOf(Cmb_Padre.Items.FindByValue("UP103" & Istat_Provincia & "000"))
    '                                'End If

    '                            End If



    '                            '-----------------------------------------------
    '                            '------- RAPPRESENTANTE LEGALE -----------------
    '                            '-----------------------------------------------

    '                            'In caso di Ditta Individuale Importo anche il rappresentante legale!
    '                            If ws_AnaResponse.output.anagrafica.codTipoAnagrafica = "DI" Then

    '                                'Dim strRuolo As String = String.Empty
    '                                'strRuolo = "<b>Ruolo: </b> Legale Rappresentante  <br/>"

    '                                Dim strReferente As String

    '                                If Not ws_AnaResponse.output.anagrafica.contatto Is Nothing Then

    '                                    strReferente = ws_AnaResponse.output.anagrafica.nome & " " & _
    '                                      ws_AnaResponse.output.anagrafica.cognome & " <br/>" & _
    '                                      "<b>CF: </b>" & ws_AnaResponse.output.anagrafica.cf & " <br/>" & _
    '                                      IIf(ws_AnaResponse.output.anagrafica.contatto.tel <> String.Empty, "<b>Telefono: </b>" & ws_AnaResponse.output.anagrafica.contatto.tel, "") & _
    '                                      IIf(ws_AnaResponse.output.anagrafica.contatto.cellulare <> String.Empty, "<br/><b>Cellulare: </b>" & ws_AnaResponse.output.anagrafica.contatto.cellulare, "") & _
    '                                      IIf(ws_AnaResponse.output.anagrafica.contatto.email <> String.Empty, "<br/><b>E-mail: </b>" & ws_AnaResponse.output.anagrafica.contatto.email, "") & _
    '                                      IIf(ws_AnaResponse.output.anagrafica.contatto.fax <> String.Empty, "<br/><b>Fax: </b>" & ws_AnaResponse.output.anagrafica.contatto.fax, "")

    '                                    If ws_AnaResponse.output.anagrafica.contatto.tel <> "" Then
    '                                        Legale_Rappresentante_Rubrica1 = "Telefono:" & ws_AnaResponse.output.anagrafica.contatto.tel
    '                                    End If
    '                                    If ws_AnaResponse.output.anagrafica.contatto.cellulare <> "" Then
    '                                        Legale_Rappresentante_Rubrica2 = "Cellulare:" & ws_AnaResponse.output.anagrafica.contatto.cellulare
    '                                    End If
    '                                    If ws_AnaResponse.output.anagrafica.contatto.email <> "" Then
    '                                        Legale_Rappresentante_Rubrica3 = "Email:" & ws_AnaResponse.output.anagrafica.contatto.email
    '                                    End If
    '                                    If ws_AnaResponse.output.anagrafica.contatto.fax <> "" Then
    '                                        Legale_Rappresentante_Rubrica4 = "Fax:" & ws_AnaResponse.output.anagrafica.contatto.fax
    '                                    End If

    '                                Else

    '                                    strReferente = ws_AnaResponse.output.anagrafica.nome & " " & _
    '                                                      ws_AnaResponse.output.anagrafica.cognome & " <br/>" & _
    '                                                      "<b>CF: </b>" & ws_AnaResponse.output.anagrafica.cf
    '                                End If


    '                                Legale_Rappresentante_Nome = ws_AnaResponse.output.anagrafica.nome
    '                                Legale_Rappresentante_Cognome = ws_AnaResponse.output.anagrafica.cognome
    '                                Legale_Rappresentante_CF = ws_AnaResponse.output.anagrafica.cf
    '                                Legale_Rappresentante_Sesso = ws_AnaResponse.output.anagrafica.sesso
    '                                Legale_Rappresentante_Data_Nascita = ws_AnaResponse.output.anagrafica.dataNascita.ToShortDateString

    '                                Legale_Rappresentante_Provincia_Nascita = "#"
    '                                Legale_Rappresentante_Comune_Nascita = "#"
    '                                Legale_Rappresentante_Istat_Provincia_Nascita = "#"
    '                                Legale_Rappresentante_Istat_Comune_Nascita = "#"

    '                                Legale_Rappresentante_Provincia = "#"
    '                                Legale_Rappresentante_Comune = "#"
    '                                Legale_Rappresentante_Istat_Provincia = "#"
    '                                Legale_Rappresentante_Istat_Comune = "#"
    '                                Legale_Rappresentante_Indirizzo = "#"
    '                                Legale_Rappresentante_Frazione = "#"
    '                                Legale_Rappresentante_Cap = "#"
    '                                Legale_Rappresentante_Stato = "#"

    '                                If Not ws_AnaResponse.output.anagrafica.indirizzoResidenza Is Nothing Then

    '                                    Dt_Istat = objIstat.Leggi("", _
    '                                              "", _
    '                                              "", _
    '                                              "", _
    '                                              "", _
    '                                              ws_AnaResponse.output.anagrafica.indirizzoResidenza.comune.codErariale, _
    '                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
    '                                               "", "", _
    '                                               ObjParametri_Server)

    '                                    If Not Dt_Istat Is Nothing AndAlso Dt_Istat.Rows.Count > 0 Then

    '                                        Legale_Rappresentante_Provincia = Dt_Istat.Rows(0).Item("Provincia_Long")
    '                                        Legale_Rappresentante_Comune = Dt_Istat.Rows(0).Item("Descrizione")
    '                                        Legale_Rappresentante_Istat_Provincia = Dt_Istat.Rows(0).Item("Pro_Cod_Istat")
    '                                        Legale_Rappresentante_Istat_Comune = Dt_Istat.Rows(0).Item("Com_Cod_Istat")

    '                                        Legale_Rappresentante_Indirizzo = ws_AnaResponse.output.anagrafica.indirizzoResidenza.toponimo & " " & ws_AnaResponse.output.anagrafica.indirizzoResidenza.descrizione & " " & ws_AnaResponse.output.anagrafica.indirizzoResidenza.civico
    '                                        Legale_Rappresentante_Frazione = "#"
    '                                        Legale_Rappresentante_Cap = ws_AnaResponse.output.anagrafica.indirizzoResidenza.cap
    '                                        Legale_Rappresentante_Stato = "IT"

    '                                    End If

    '                                End If

    '                            Else
    '                                ' Me.Lbl_Dettagli_Referente.Text = "---"
    '                            End If

    '                            '-----------------------------------------------
    '                            '------- IMPRESA       -------------------------
    '                            '-----------------------------------------------


    '                            If EsisteImpresa = True Then
    '                                TipoOperazione = enum_TipoOperazioneDB.Modifica
    '                            End If

    '                            XmlImpresa = objXML.Xml_Pubblico_Impresa(TipoOperazione, _
    '                                                                     PIVA, _
    '                                                                     RagSoc, _
    '                                                                     CUAA, _
    '                                                                     CUAA, _
    '                                                                     "#", _
    '                                                                     "#", _
    '                                                                     piva_padre, _
    '                                                                     "1", _
    '                                                                     "1", _
    '                                                                     "#", "#", _
    '                                                                     Indirizzo, _
    '                                                                     Localita, _
    '                                                                     Cap, _
    '                                                                     Comune, _
    '                                                                     Provincia, _
    '                                                                     "#", "#", _
    '                                                                     Istat_Comune, _
    '                                                                     Istat_Provincia, _
    '                                                                     "", _
    '                                                                     Legale_Rappresentante_Cognome, _
    '                                                                     Legale_Rappresentante_Nome, _
    '                                                                     Legale_Rappresentante_CF, _
    '                                                                     Legale_Rappresentante_Sesso, _
    '                                                                     "#", "#", _
    '                                                                     Legale_Rappresentante_Indirizzo, _
    '                                                                     Legale_Rappresentante_Frazione, _
    '                                                                     Legale_Rappresentante_Cap, _
    '                                                                     Legale_Rappresentante_Comune, _
    '                                                                     Legale_Rappresentante_Provincia, _
    '                                                                     Legale_Rappresentante_Stato, _
    '                                                                     "#", _
    '                                                                     Legale_Rappresentante_Istat_Comune, _
    '                                                                     Legale_Rappresentante_Istat_Provincia, _
    '                                                                     Legale_Rappresentante_Data_Nascita, _
    '                                                                     Legale_Rappresentante_Comune_Nascita, _
    '                                                                     Legale_Rappresentante_Provincia_Nascita, _
    '                                                                     Legale_Rappresentante_Istat_Comune_Nascita, _
    '                                                                     Legale_Rappresentante_Istat_Provincia_Nascita, _
    '                                                                     "#", _
    '                                                                     Legale_Rappresentante_Rubrica1, _
    '                                                                     Legale_Rappresentante_Rubrica2, _
    '                                                                     Legale_Rappresentante_Rubrica3, _
    '                                                                     Legale_Rappresentante_Rubrica4, _
    '                                                                     Legale_Rappresentante_Rubrica5, _
    '                                                                     "#", "#", "#", "#", "#", "#", _
    '                                                                     ID_Azienda, _
    '                                                                     XmlDoc)

    '                            If Presente_Fascicolo = True Then
    '                                XmlImpresa.AppendChild(XmlFascicolo)
    '                            End If


    '                            If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
    '                                Sa_Cod = RecuperaSaCodImpresa(Codice_Chiave_Cliente, PIVA, ID_Azienda)
    '                            End If


    '                            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(TipoOperazione, _
    '                                                                            Sa_Cod, _
    '                                                                            "Centro n.01", _
    '                                                                            1, _
    '                                                                            "#", "#", "#", "#", "#", "#", _
    '                                                                            Indirizzo, _
    '                                                                            "#", _
    '                                                                            Cap, _
    '                                                                            Comune, _
    '                                                                            Provincia, _
    '                                                                            "#", "#", _
    '                                                                            Istat_Comune, _
    '                                                                            Istat_Provincia, _
    '                                                                            "#", "#", "#", "#", "#", "#", "#", "#", "#", _
    '                                                                            ID_Azienda, _
    '                                                                            XmlDoc)

    '                            'Creo il fabbricato
    '                            '1. in caso di primo inserimento
    '                            '2. in caso di modifica se non esiste già un magazzino
    '                            Dim CreaMagazzino As Boolean = False
    '                            If TipoOperazione = enum_TipoOperazioneDB.Scrittura Then
    '                                CreaMagazzino = True
    '                            Else
    '                                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
    '                                Dim DtFabb As DataTable
    '                                DtFabb = objFabb.Leggi(PIVA, Sa_Cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                     " Tipo_Fabbricato_Cod=20 ", _
    '                                                     "", ObjParametri_Server)
    '                                If DtFabb.Rows.Count = 0 Then
    '                                    CreaMagazzino = True
    '                                End If
    '                            End If

    '                            If CreaMagazzino = True Then
    '                                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura, _
    '                                                                              "0", _
    '                                                                              "Magazzino n.01", _
    '                                                                              20, _
    '                                                                              "#", "#", "#", _
    '                                                                              "#", _
    '                                                                              "#", _
    '                                                                              "#", _
    '                                                                              CostantiPersonalizzate.AGRODATAINIZIO, _
    '                                                                              CostantiPersonalizzate.AGRODATAFINE, _
    '                                                                              Indirizzo, _
    '                                                                              "#", _
    '                                                                              Cap, _
    '                                                                              Comune, _
    '                                                                              Provincia, _
    '                                                                              "#", "#", _
    '                                                                              Istat_Comune, _
    '                                                                              Istat_Provincia, _
    '                                                                              ID_Azienda, _
    '                                                                              XmlDoc)

    '                                XmlCentro.AppendChild(XmlFabbricato)
    '                            End If

    '                            XmlImpresa.AppendChild(XmlCentro)

    '                            XmlUtente.AppendChild(XmlImpresa)

    '                            STR_XmlDoc = XmlUtente.OuterXml



    '                            '*******************************************************************************************************************
    '                            '******   CONDIZIONALITA    ****************************************************************************************
    '                            '*******************************************************************************************************************

    '                            'verifico se esiste già su GIAS un PROFILO VALIDO...
    '                            'se esiste NON LO CREO ORA!!!
    '                            Dim Dt_Interviste As DataTable
    '                            Dim CreaProfilo As Boolean = True

    '                            Dim objAuditReg As New AgronicaCoreAuditDAL.Audit_Regolamenti_R
    '                            Dim DtReg As DataTable
    '                            Dim DataInizioReg, DataFineReg As Date

    '                            'anno corrente
    '                            DataInizioReg = CDate("01/01/" & Now.Year)
    '                            DataFineReg = CDate("31/12/" & Now.Year)

    '                            ' se ce n'è più di uno prendo cmq il primo, il più recente
    '                            DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, DataInizioReg, DataFineReg, "", "", ObjParametri_Server)

    '                            If DtReg.Rows.Count > 0 Then
    '                                RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
    '                            Else
    '                                ' leggo il regolamento audit più recente
    '                                DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, #1/1/1900#, #12/31/1900#, "", "", ObjParametri_Server)
    '                                If DtReg.Rows.Count > 0 Then
    '                                    RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
    '                                End If
    '                            End If


    '                            Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
    '                            Dt_Interviste = objAudit_Interviste.Leggi(1, _
    '                                                                      RegolamentoCod, _
    '                                                                      0, _
    '                                                                      PIVA, _
    '                                                                      CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, _
    '                                                                      "", "", _
    '                                                                      ObjParametri_Server)

    '                            If Not Dt_Interviste Is Nothing AndAlso Dt_Interviste.Rows.Count > 0 Then
    '                                CreaProfilo = False
    '                            End If

    '                            If CreaProfilo = True Then

    '                                If Not ws_fasciResponse Is Nothing AndAlso _
    '                                    Not ws_fasciResponse.out Is Nothing AndAlso _
    '                                    Not ws_fasciResponse.out.fascicolo Is Nothing AndAlso _
    '                                    Not ws_fasciResponse.out.fascicolo.fascicolo Is Nothing AndAlso _
    '                                    Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita Is Nothing Then

    '                                    If ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita.Length > 0 Then

    '                                        'verifico se esiste già su GIAS un profilo valido...

    '                                        Dim objCoreVarie As New AgronicaCoreVarieDAL.Codifica_Codici


    '                                        Dim DR As DataRow
    '                                        Dim Codice_Condizionalita As String = ""
    '                                        Dim Descrizione As String = ""
    '                                        Dim Regolamento_Cod As Integer = 1
    '                                        Dim Domanda_Cod As Integer = 0

    '                                        DtCondizionalita = New DataTable
    '                                        DtCondizionalita.Columns.Add(New DataColumn("Raccoglitore_Cod", GetType(Integer)))
    '                                        DtCondizionalita.Columns.Add(New DataColumn("Audit_Tipo", GetType(Integer)))
    '                                        DtCondizionalita.Columns.Add(New DataColumn("Domanda_Cod", GetType(Integer)))
    '                                        DtCondizionalita.Columns.Add(New DataColumn("Codice", GetType(String)))
    '                                        DtCondizionalita.Columns.Add(New DataColumn("Descrizione", GetType(String)))

    '                                        For i = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita.Length - 1

    '                                            Codice_Condizionalita = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita(i).CodiceCondizionalita), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita(i).CodiceCondizionalita)

    '                                            objCoreVarie.Converti_Audit_Domande_Interviste(Codice_Condizionalita, _
    '                                                                                           Regolamento_Cod, _
    '                                                                                           enum_AuditPuaTipo.Audit_Condizionalita, _
    '                                                                                           Domanda_Cod, _
    '                                                                                            Descrizione, _
    '                                                                                           "", _
    '                                                                                           ObjParametri_Server)

    '                                            'inserisco la domanda solo se c'è corrispondenza con GIAS
    '                                            If Domanda_Cod <> 0 Then

    '                                                DR = DtCondizionalita.NewRow

    '                                                DR.Item("Codice") = Codice_Condizionalita
    '                                                DR.Item("Raccoglitore_Cod") = Regolamento_Cod
    '                                                DR.Item("Audit_Tipo") = enum_AuditPuaTipo.Audit_Condizionalita
    '                                                DR.Item("Domanda_Cod") = Domanda_Cod
    '                                                DR.Item("Descrizione") = Descrizione

    '                                                DtCondizionalita.Rows.Add(DR)

    '                                            End If

    '                                        Next

    '                                    Else


    '                                    End If

    '                                End If

    '                            Else
    '                                '
    '                            End If


    '                            '*******************************************************************************************************************
    '                            '******   PARTICELLE CATASTALI    **********************************************************************************
    '                            '*******************************************************************************************************************
    '                            If Not ws_fasciResponse Is Nothing AndAlso _
    '                                Not ws_fasciResponse.out Is Nothing AndAlso _
    '                                Not ws_fasciResponse.out.fascicolo Is Nothing AndAlso _
    '                                Not ws_fasciResponse.out.fascicolo.fascicolo Is Nothing AndAlso _
    '                                Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1 Is Nothing Then

    '                                If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1.Length > 0 Then

    '                                    Dim Anno As Integer

    '                                    Anno = Today.Year
    '                                    If annoPrecedente Then
    '                                        Anno = Today.Year - 1
    '                                    End If


    '                                    Dim strValiditaInizio As String = String.Empty
    '                                    Dim strValiditaFine As String = String.Empty
    '                                    strValiditaInizio = "01/11/" & (Anno - 1).ToString
    '                                    strValiditaFine = "31/10/" & Anno.ToString


    '                                    '*******************************************************************************************************************
    '                                    '******   PIANIFICAZIONE    **********************************************************************************

    '                                    Dim nEntita As Integer = 1


    '                                    Dim strPianificazione As String
    '                                    If DataValidazione <> CostantiPersonalizzate.AGRODATAINIZIO Then
    '                                        strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & SchedaValidazione & " Data Validazione " & DataValidazione.ToShortDateString & ")"
    '                                    Else
    '                                        strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & SchedaValidazione & " Data Validazione non presente)"
    '                                    End If

    '                                    XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(enum_TipoOperazioneDB.Scrittura, _
    '                                                       "0", _
    '                                                       PIVA, _
    '                                                       strPianificazione, _
    '                                                       strPianificazione, _
    '                                                       "Importazione " & strPianificazione, _
    '                                                       enum_TipoPianificazione.Pianificazione_Annuale, _
    '                                                       strValiditaInizio, _
    '                                                       strValiditaFine, _
    '                                                       XmlDocP)

    '                                    XmlUtenteP.AppendChild(XmlTestata)

    '                                    If Presente_Fascicolo = True Then
    '                                        XmlTestata.AppendChild(XmlFascicoloP)
    '                                    End If

    '                                    STR_XmlDocP = XmlUtenteP.OuterXml

    '                                    Dim DR As DataRow

    '                                    DtParticelle = New DataTable
    '                                    DtParticelle.Columns.Add(New DataColumn("PROV", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("COM", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Sezione", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Numero", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Subalterno", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("datepossesso", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("inizio_possesso", GetType(Date)))
    '                                    DtParticelle.Columns.Add(New DataColumn("fine_possesso", GetType(Date)))
    '                                    DtParticelle.Columns.Add(New DataColumn("catasto", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("possesso", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("TitoloPossesso", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("macrouso", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("macrouso_sup", GetType(Double)))
    '                                    DtParticelle.Columns.Add(New DataColumn("sup", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("supcondotta", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("utilizzo", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("utilizzo_sup", GetType(Double)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
    '                                    DtParticelle.Columns.Add(New DataColumn("Scarto", GetType(Integer)))

    '                                    For i = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1.Length - 1

    '                                        'DR = DtParticelle.NewRow

    '                                        Prov = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Provincia), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Provincia)
    '                                        Com = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Comune), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Comune)
    '                                        Sezione = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Sezione), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Sezione)
    '                                        Foglio = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Foglio), 0, ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Foglio)
    '                                        strNumero = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Particella), 0, ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Particella)
    '                                        Subalterno = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Subalterno), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Subalterno)

    '                                        'verifico cosa trovo nel campo particella (su Agea è una stringa)
    '                                        'se trovo dei numeri li metto in numero
    '                                        'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
    '                                        Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
    '                                        Numero = objCOre.Numero_from_Stringa(strNumero)
    '                                        NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
    '                                        If Subalterno = "" And NumeroStringa <> "" Then
    '                                            Subalterno = Left(NumeroStringa, 3)
    '                                        End If


    '                                        TitoloPossesso = Converti_TitoliPossesso_Fascicolo(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).CodiceTipoConduzione, TitoloPossessoDes)
    '                                        'DR.Item("possesso") = TitoloPossessoDes

    '                                        Inizio_Possesso = #1/1/1900#
    '                                        Fine_Possesso = #12/31/2100#
    '                                        Inizio_Possesso_Old = #1/1/1900#
    '                                        Fine_Possesso_Old = #12/31/2100#

    '                                        If Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione Is Nothing AndAlso ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione <> "" Then
    '                                            Inizio_Possesso = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione))
    '                                        End If
    '                                        If Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione Is Nothing AndAlso ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "" Then
    '                                            Fine_Possesso = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione))
    '                                        End If


    '                                        supCatasto = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCatastale) / 10000.0
    '                                        objXML.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)


    '                                        supConduzione = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCondotta) / 10000.0


    '                                        If Sezione = "" Then
    '                                            strSezione = "0"
    '                                        Else
    '                                            strSezione = Sezione
    '                                        End If

    '                                        If Subalterno = "" _
    '                                           Or Subalterno = "000" Then
    '                                            strSubalterno = "0"
    '                                        Else
    '                                            strSubalterno = Subalterno
    '                                        End If


    '                                        '*******************************************************************************************************************
    '                                        '******   MACROUSI    **********************************************************************************
    '                                        '*******************************************************************************************************************

    '                                        Dim Macrouso_Cod As String
    '                                        Dim Macrouso_Sup As Double
    '                                        Dim Specie_Cod As String
    '                                        Dim Varieta_Cod As String
    '                                        Dim Specie_Des As String
    '                                        Dim Varieta_Des As String
    '                                        Dim Utilizzo_Sup As Double
    '                                        Dim HashMacrousi As New Hashtable
    '                                        strMacrousi = ""
    '                                        strUtilizzi = ""

    '                                        If Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1 Is Nothing Then

    '                                            For j = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1.Length - 1

    '                                                strMacrousi = ""
    '                                                strUtilizzi = ""

    '                                                Macrouso_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).CodiceMacrouso
    '                                                Macrouso_Sup = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).SuperficieUtilizzata) / 10000.0

    '                                                'MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
    '                                                'MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                                'logga(MsgOK)

    '                                                If Not HashMacrousi.ContainsKey(Macrouso_Cod) Then

    '                                                    HashMacrousi.Add(Macrouso_Cod, "")

    '                                                    Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
    '                                                    strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).CodiceMacrouso, _
    '                                                                                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                                                                                    "", "", _
    '                                                                                                                    ObjParametri_Server) & " (" & Macrouso_Sup & " Ha)<br/>"



    '                                                    '*******************************************************************************************************************
    '                                                    '******   UTILIZZO    **********************************************************************************
    '                                                    '*******************************************************************************************************************

    '                                                    If Not ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra Is Nothing Then

    '                                                        For x = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra.Length - 1

    '                                                            strUtilizzi = ""

    '                                                            Specie_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).CodiceProdotto
    '                                                            Varieta_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).CodiceVarieta
    '                                                            Utilizzo_Sup = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).SuperficieUtilizzata) / 10000.0

    '                                                            Specie_Des = ""
    '                                                            Varieta_Des = ""

    '                                                            Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
    '                                                            'Dim DtCodifica As DataTable
    '                                                            Dim Veg_Cod As Integer = 0
    '                                                            Dim Cul_Cod As Integer = 0
    '                                                            Dim Grfi_Cod As Integer = 0
    '                                                            Dim Id_Cod As Integer = 0
    '                                                            Dim Grva_Cod As Integer = 0
    '                                                            ' Dim DrVar() As DataRow
    '                                                            Dim LogCodificheMancantiSpecie As String = ""
    '                                                            Dim LogCodificheMancantiVarieta As String = ""

    '                                                            objUtilizzi.Specie_e_Varieta_Gias_Da_Agea( _
    '                                                                              LogCodificheMancantiSpecie, _
    '                                                                              LogCodificheMancantiVarieta, _
    '                                                                              Specie_Cod, Varieta_Cod, _
    '                                                                              Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod, _
    '                                                                              Specie_Des, Varieta_Des, _
    '                                                                              "", _
    '                                                                              "", _
    '                                                                              ObjParametri_Server)



    '                                                            If Varieta_Des <> "" Then
    '                                                                strUtilizzi = Specie_Des & " - " & Varieta_Des & " (" & Utilizzo_Sup & " Ha)"
    '                                                            Else
    '                                                                strUtilizzi = Specie_Des & " (" & Utilizzo_Sup & " Ha)"
    '                                                            End If


    '                                                            DR = DtParticelle.NewRow

    '                                                            DR.Item("PROV") = Prov
    '                                                            DR.Item("COM") = Com
    '                                                            DR.Item("Sezione") = Sezione
    '                                                            DR.Item("Foglio") = Foglio
    '                                                            DR.Item("Numero") = Numero
    '                                                            DR.Item("Subalterno") = Subalterno

    '                                                            DR.Item("Catasto") = DR.Item("PROV") & ":" & _
    '                                                                                 DR.Item("COM") & ":_" & _
    '                                                                                 DR.Item("Sezione") & ":_" & _
    '                                                                                 DR.Item("Foglio").ToString & ":_" & _
    '                                                                                 DR.Item("Numero").ToString & ":_" & _
    '                                                                                 DR.Item("Subalterno")

    '                                                            DR.Item("possesso") = TitoloPossessoDes
    '                                                            DR.Item("TitoloPossesso") = TitoloPossesso

    '                                                            DR.Item("inizio_possesso") = Inizio_Possesso
    '                                                            DR.Item("fine_possesso") = Fine_Possesso

    '                                                            DR.Item("datepossesso") = "Dal " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
    '                                                                                    IIf(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

    '                                                            supCatasto = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCatastale) / 10000.0
    '                                                            objXML.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
    '                                                            DR.Item("sup") = Format(supCatasto, "0.0000")

    '                                                            supConduzione = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCondotta) / 10000.0
    '                                                            DR.Item("supcondotta") = Format(supConduzione, "0.0000")

    '                                                            DR.Item("macrouso_cod") = Macrouso_Cod
    '                                                            DR.Item("macrouso_sup") = Macrouso_Sup
    '                                                            DR.Item("macrouso") = strMacrousi

    '                                                            DR.Item("utilizzo") = strUtilizzi
    '                                                            DR.Item("utilizzo_sup") = Utilizzo_Sup
    '                                                            DR.Item("Veg_Cod_Agea") = Specie_Cod
    '                                                            DR.Item("Cul_Cod_Agea") = Varieta_Cod
    '                                                            DR.Item("Veg_Cod") = Veg_Cod
    '                                                            DR.Item("Cul_Cod") = Cul_Cod
    '                                                            DR.Item("Grfi_Cod") = Grfi_Cod
    '                                                            DR.Item("Grva_Cod") = Grva_Cod
    '                                                            DR.Item("Id_Cod") = Id_Cod

    '                                                            DR.Item("Scarto") = 0

    '                                                            DtParticelle.Rows.Add(DR)
    '                                                        Next

    '                                                    Else

    '                                                        DR = DtParticelle.NewRow

    '                                                        DR.Item("PROV") = Prov
    '                                                        DR.Item("COM") = Com
    '                                                        DR.Item("Sezione") = Sezione
    '                                                        DR.Item("Foglio") = Foglio
    '                                                        DR.Item("Numero") = Numero
    '                                                        DR.Item("Subalterno") = Subalterno

    '                                                        DR.Item("Catasto") = DR.Item("PROV") & ":" & _
    '                                                                             DR.Item("COM") & ":_" & _
    '                                                                             DR.Item("Sezione") & ":_" & _
    '                                                                             DR.Item("Foglio").ToString & ":_" & _
    '                                                                             DR.Item("Numero").ToString & ":_" & _
    '                                                                             DR.Item("Subalterno")

    '                                                        DR.Item("possesso") = TitoloPossessoDes
    '                                                        DR.Item("TitoloPossesso") = TitoloPossesso
    '                                                        DR.Item("inizio_possesso") = Inizio_Possesso
    '                                                        DR.Item("fine_possesso") = Fine_Possesso

    '                                                        DR.Item("datepossesso") = "Dal " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
    '                                                                                IIf(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

    '                                                        DR.Item("sup") = Format(supCatasto, "0.0000")
    '                                                        DR.Item("supcondotta") = Format(supConduzione, "0.0000")

    '                                                        DR.Item("macrouso_cod") = Macrouso_Cod
    '                                                        DR.Item("macrouso_sup") = Macrouso_Sup
    '                                                        DR.Item("macrouso") = strMacrousi

    '                                                        DR.Item("utilizzo") = ""
    '                                                        DR.Item("utilizzo_sup") = 0
    '                                                        DR.Item("Veg_Cod_Agea") = ""
    '                                                        DR.Item("Cul_Cod_Agea") = ""
    '                                                        DR.Item("Veg_Cod") = 0
    '                                                        DR.Item("Cul_Cod") = 0
    '                                                        DR.Item("Grfi_Cod") = 0
    '                                                        DR.Item("Grva_Cod") = 0
    '                                                        DR.Item("Id_Cod") = 0

    '                                                        DR.Item("Scarto") = 1

    '                                                        DtParticelle.Rows.Add(DR)

    '                                                    End If

    '                                                End If

    '                                            Next

    '                                        Else

    '                                            DR = DtParticelle.NewRow

    '                                            DR.Item("PROV") = Prov
    '                                            DR.Item("COM") = Com
    '                                            DR.Item("Sezione") = Sezione
    '                                            DR.Item("Foglio") = Foglio
    '                                            DR.Item("Numero") = Numero
    '                                            DR.Item("Subalterno") = Subalterno

    '                                            DR.Item("Catasto") = DR.Item("PROV") & ":" & _
    '                                                                 DR.Item("COM") & ":_" & _
    '                                                                 DR.Item("Sezione") & ":_" & _
    '                                                                 DR.Item("Foglio").ToString & ":_" & _
    '                                                                 DR.Item("Numero").ToString & ":_" & _
    '                                                                 DR.Item("Subalterno")

    '                                            DR.Item("possesso") = TitoloPossessoDes
    '                                            DR.Item("TitoloPossesso") = TitoloPossesso
    '                                            DR.Item("inizio_possesso") = Inizio_Possesso
    '                                            DR.Item("fine_possesso") = Fine_Possesso

    '                                            DR.Item("datepossesso") = "Dal " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
    '                                                                    IIf(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

    '                                            DR.Item("sup") = Format(supCatasto, "0.0000")
    '                                            DR.Item("supcondotta") = Format(supConduzione, "0.0000")

    '                                            DR.Item("macrouso_cod") = ""
    '                                            DR.Item("macrouso_sup") = 0
    '                                            DR.Item("macrouso") = ""

    '                                            DR.Item("utilizzo") = ""
    '                                            DR.Item("utilizzo_sup") = 0
    '                                            DR.Item("Veg_Cod_Agea") = ""
    '                                            DR.Item("Cul_Cod_Agea") = ""
    '                                            DR.Item("Veg_Cod") = 0
    '                                            DR.Item("Cul_Cod") = 0
    '                                            DR.Item("Grfi_Cod") = 0
    '                                            DR.Item("Grva_Cod") = 0
    '                                            DR.Item("Id_Cod") = 0

    '                                            DR.Item("Scarto") = 1

    '                                            DtParticelle.Rows.Add(DR)

    '                                        End If

    '                                    Next 'ISWSTerritorio1.Length


    '                                    Dim NomiChiavi(1) As String
    '                                    'Chiave della griglia
    '                                    NomiChiavi(0) = "Scarto"
    '                                    NomiChiavi(1) = "Utilizzo"


    '                                Else

    '                                    msgCarica &= "<br>Il webservice non ha restituito dati sulle particelle catastali, macrousi e utilizzi."

    '                                End If 'ISWSTerritorio1.Length
    '                            Else

    '                                msgCarica &= "<br>Il webservice non ha restituito dati sulle particelle catastali, macrousi e utilizzi."

    '                            End If 'ISWSTerritorio1.Length




    '                            '------------------------------------------------------
    '                            '--------------- PERSONA ------------
    '                            '------------------------------------------------------
    '                        Case "P"    'persona (non mi serve)

    '                            Persona = True

    '                            msgCarica &= "<br>Impossibile scaricare i dati poichè il CUAA inserito appartiene ad una persona!"

    '                    End Select

    '                    If Presente_Anagrafe = False Then
    '                        msgCarica &= "Impossibile scaricare l'impresa poichè non è presente sull'archivio Anagrafe!"
    '                    End If

    '                Else
    '                    'se il ws non risponde o ci sono problemi....................

    '                    If RispostaWSAnagrafe = True Then

    '                        MsgOK = "Anagrafica non trovata!"
    '                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                        logga(MsgOK)

    '                        msgCarica &= "Anagrafica non trovata!"

    '                    Else

    '                        MsgOK = "Il WebService Anagrafe non ha risposto. RispostaWSAnagrafe = false"
    '                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                        logga(MsgOK)

    '                        msgCarica &= "Il WebService Anagrafe non ha risposto."

    '                    End If



    '                    If ws_AnaResponse Is Nothing Then

    '                        MsgOK = "ws_AnaResponse NOTHING!"
    '                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                        logga(MsgOK)

    '                        msgCarica &= "ws_AnaResponse NOTHING!"
    '                    Else


    '                        If ws_AnaResponse.output Is Nothing Then

    '                            MsgOK = "ws_AnaResponse.output NOTHING!"
    '                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                            logga(MsgOK)

    '                            msgCarica &= "ws_AnaResponse.output NOTHING!"
    '                        Else


    '                            If ws_AnaResponse.output.intestazione Is Nothing Then

    '                                MsgOK = "ws_AnaResponse.output.intestazione NOTHING!"
    '                                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                logga(MsgOK)
    '                                msgCarica &= "ws_AnaResponse.output.intestazione NOTHING!"
    '                            Else


    '                            End If

    '                            If ws_AnaResponse.output.version Is Nothing Then

    '                                MsgOK = "ws_AnaResponse.output.versione NOTHING!"
    '                                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                logga(MsgOK)
    '                                msgCarica &= "ws_AnaResponse.output.versione NOTHING!"
    '                            Else


    '                            End If

    '                            If ws_AnaResponse.output.cf Is Nothing Then

    '                                MsgOK = "ws_AnaResponse.output.cf NOTHING!"
    '                                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                logga(MsgOK)
    '                                msgCarica &= "ws_AnaResponse.output.cf NOTHING!"
    '                            Else


    '                            End If


    '                            If ws_AnaResponse.output.anagrafica Is Nothing Then

    '                                MsgOK = "ws_AnaResponse.output.anagrafica NOTHING!"
    '                                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                                logga(MsgOK)
    '                                msgCarica &= "ws_AnaResponse.output.anagrafica NOTHING!"
    '                            Else


    '                            End If

    '                        End If

    '                    End If

    '                    Return False

    '                End If


    '            Catch ex As Exception

    '                Dim strEr As String
    '                strEr = ex.Message

    '                MsgOK = "Anagrafe NON presente!"
    '                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
    '                logga(MsgOK)

    '                msgCarica &= ex.Message
    '                Return False

    '            End Try



    '        Next 'CICLO SCHEDE


    '    End If

    '    Return True


    'End Function

    'Deve ritornare true se non ci sono stati errori di qualsiasi genere.
    'Se ritporna true la sincronizzazione devo farla solo se:
    '   Azienda_Con_Fascicolo_Nuovo = true
    '           cioè il fascicolo letto è nuovo, quindi aggirono catasto e anagrafica e l'ho anche salvato nella funzione
    Private Function _3_CaricaDati_WS_BA(ByVal PIVA As String, _
                                         ByVal CUAA As String, _
                                         ByVal DtPV As DataTable, _
                                         ByRef msgCarica As String, _
                                         ByRef AziendeAggiornateSuGias As Integer, _
                                         ByRef AziendeConProblemiAggironamentoSuGias As Integer) As Boolean


        'Dim Fascicolo_Nuovo As Boolean = True

        Dim Crea_Fascicolo As Boolean = False
        Dim Ribalta_Fascicolo As Boolean = False

        Dim Sa_Cod As Integer = 0
        Dim DtCondizionalita As New DataTable
        Dim DtParticelle As New DataTable

        Dim EsisteImpresa As Boolean = True

        If Codice_Chiave_Cliente = 0 Then
            Throw New Exception("Configurazione_Servizio.Id_Cod_Cliente =  '' ")
        End If

        'If PIVA = "" Then
        '    msgCarica &= "Inserire PIVA dell'Impresa che si desidera importare!"
        '    Return False
        'Else
        '    EsisteImpresa = True
        'End If

        If CUAA = "" Then
            msgCarica &= "Inserire il CUAA dell'Impresa che si desidera importare!"
            Return False
        Else
            CUAA = CUAA.ToUpper
        End If

        If PIVA <> "" Then
            '--------------------------------------------
            'verifico se l'impresa è presente su Gias
            Dim data_modifica_temp As Date
            Dim piva_padre As String
            Dim socio_temp As String
            If Not Imprese_Read.Esiste_Impresa(PIVA, 0, data_modifica_temp, piva_padre, socio_temp, "", ObjParametri_Server) Then
                msgCarica &= "impresa non è presente su Gias"
                EsisteImpresa = False
                Return False
            End If
        Else
            EsisteImpresa = False
        End If


        Dim i, j, x As Integer

        Dim ID_Azienda As String = ""
        Dim Detentore_Fascicolo As String = ""

        Dim RagSoc As String = ""
        Dim Indirizzo As String = ""
        Dim Cap As String = ""
        Dim Provincia As String = ""
        Dim Comune As String = ""
        Dim Istat_Provincia As String = ""
        Dim Istat_Comune As String = ""
        Dim Cod_Belfiore As String = ""
        Dim Localita As String = ""


        Dim Prov As String = ""
        Dim Com As String = ""
        Dim Sezione As String = ""
        Dim Foglio As String = ""
        Dim Numero As String = ""
        Dim strNumero As String = ""
        Dim NumeroStringa As String = ""
        Dim Subalterno As String = ""
        Dim strSezione As String = ""
        Dim strSubalterno As String = ""
        Dim Ettari As Double
        Dim Are As Double
        Dim Centiare As Double
        Dim supCatasto As Double = 0
        Dim supConduzione As Double = 0
        Dim strMacrousi As String = ""
        Dim strUtilizzi As String = ""
        Dim TitoloPossesso As String
        Dim TitoloPossessoDes As String
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Inizio_Possesso_Old As Date
        Dim Fine_Possesso_Old As Date

        Dim Legale_Rappresentante As String = "#"
        Dim Legale_Rappresentante_Cognome As String = "#"
        Dim Legale_Rappresentante_Nome As String = "#"
        Dim Legale_Rappresentante_CF As String = "#"
        Dim Legale_Rappresentante_Sesso As String = "#"
        Dim Legale_Rappresentante_Indirizzo As String = "#"
        Dim Legale_Rappresentante_Frazione As String = "#"
        Dim Legale_Rappresentante_Cap As String = "#"
        Dim Legale_Rappresentante_Comune As String = "#"
        Dim Legale_Rappresentante_Provincia As String = "#"
        Dim Legale_Rappresentante_Stato As String = "#"
        Dim Legale_Rappresentante_Istat_Comune As String = "#"
        Dim Legale_Rappresentante_Istat_Provincia As String = "#"
        Dim Legale_Rappresentante_Data_Nascita As String = "#"
        Dim Legale_Rappresentante_Comune_Nascita As String = "#"
        Dim Legale_Rappresentante_Provincia_Nascita As String = "#"
        Dim Legale_Rappresentante_Istat_Comune_Nascita As String = "#"
        Dim Legale_Rappresentante_Istat_Provincia_Nascita As String = "#"
        Dim Legale_Rappresentante_Rubrica1 As String = "#"
        Dim Legale_Rappresentante_Rubrica2 As String = "#"
        Dim Legale_Rappresentante_Rubrica3 As String = "#"
        Dim Legale_Rappresentante_Rubrica4 As String = "#"
        Dim Legale_Rappresentante_Rubrica5 As String = "#"

        Dim Presente_Anagrafe As Boolean = False
        Dim Presente_Fascicolo As Boolean = False
        Dim Persona As Boolean = False
        Dim RispostaWSAnagrafe As Boolean = False
        Dim RispostaWSFascicolo As Boolean = False

        Dim TipoOperazione As Integer = enum_TipoOperazioneDB.Scrittura


        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XmlUtente As System.Xml.XmlElement
        Dim XmlImpresa As System.Xml.XmlElement
        Dim XmlFascicolo As System.Xml.XmlElement
        Dim XmlCentro As System.Xml.XmlElement
        Dim XmlFabbricato As XmlElement
        Dim XmlDocP As New System.Xml.XmlDocument
        Dim XmlUtenteP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XmlFascicoloP As System.Xml.XmlElement
        Dim objXML As New AgronicaCoreXML.AnagrafeXML




        '----------------------------------------------
        'verifico se l'impresa è presente su Anagrafe
        '----------------------------------------------

        'Dim ws_AnaColdi As New SSAClient
        Dim SSAWS_name As String = "SSAWS_SSAHttpPort"
        ' SSAWS_Uri  = "http://web_esb.coldiretti.it:80/SSAMMWeb/sca/SSAWS"
        Dim ws_AnaColdi As New SincroAnagrafeBA1.SSAWS_SSAHttpService(SSAWS_Uri)


        Dim ws_AnaResponse As SincroAnagrafeBA1.cercaAnagraficaResponse
        Dim gA As New SincroAnagrafeBA1.cercaAnagrafica


        ' FascicoloWS_Uri  = "http://web_esb.coldiretti.it:80/FascicoloMMWeb/sca/FascicoloWS"
        Dim FascicoloWS_name As String = "FascicoloWS_FascicoloHttpPort"
        Dim FascicoloWS_contract As String = "Fascicolo"

        Dim ContractDescription As New System.ServiceModel.Description.ContractDescription(FascicoloWS_contract)

        ContractDescription = System.ServiceModel.Description.ContractDescription.GetContract(GetType(SincroAnagrafeBA1.NsFascicolo.IFascicolo), GetType(SincroAnagrafeBA1.NsFascicolo.Fascicolo))

        'usare BASIC perchè EnvelopeVersion = 11
        'Copiato da webconfig sincronizzatore
        Dim basicHttpBinding As New System.ServiceModel.BasicHttpBinding()
        basicHttpBinding.Name = "FascicoloWS_FascicoloHttpBinding"
        basicHttpBinding.CloseTimeout = New TimeSpan(0, 1, 0)
        basicHttpBinding.OpenTimeout = New TimeSpan(0, 1, 0)
        basicHttpBinding.ReceiveTimeout = New TimeSpan(0, 10, 0)
        basicHttpBinding.SendTimeout = New TimeSpan(1, 0, 0)
        basicHttpBinding.AllowCookies = False
        basicHttpBinding.BypassProxyOnLocal = False
        basicHttpBinding.HostNameComparisonMode = ServiceModel.HostNameComparisonMode.StrongWildcard
        basicHttpBinding.MaxBufferSize = 2097152
        basicHttpBinding.MaxReceivedMessageSize = 2097152
        basicHttpBinding.MaxBufferPoolSize = 2097152
        basicHttpBinding.MessageEncoding = ServiceModel.WSMessageEncoding.Text
        basicHttpBinding.TextEncoding = System.Text.Encoding.UTF8
        basicHttpBinding.TransferMode = ServiceModel.TransferMode.Buffered
        basicHttpBinding.UseDefaultWebProxy = True

        basicHttpBinding.ReaderQuotas.MaxDepth = 32
        basicHttpBinding.ReaderQuotas.MaxStringContentLength = 8192
        basicHttpBinding.ReaderQuotas.MaxArrayLength = 16384
        basicHttpBinding.ReaderQuotas.MaxBytesPerRead = 4096
        basicHttpBinding.ReaderQuotas.MaxNameTableCharCount = 16384

        basicHttpBinding.Security.Mode = ServiceModel.SecurityMode.None

        basicHttpBinding.Security.Transport.ClientCredentialType = ServiceModel.HttpClientCredentialType.None
        basicHttpBinding.Security.Transport.ProxyCredentialType = ServiceModel.HttpClientCredentialType.None
        basicHttpBinding.Security.Transport.Realm = ""

        basicHttpBinding.Security.Message.ClientCredentialType = ServiceModel.BasicHttpMessageCredentialType.UserName
        basicHttpBinding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default


        Dim EndpointAddress As New System.ServiceModel.EndpointAddress(FascicoloWS_Uri)
        Dim Endpoint As New System.ServiceModel.Description.ServiceEndpoint(ContractDescription, basicHttpBinding, EndpointAddress)
        Endpoint.Name = FascicoloWS_name

        'Dim ws_FascicoloColdi As New SincroAnagrafeBA1.NsFascicolo.FascicoloClient(Endpoint)
        'Dim ws_fasciResponse As SincroAnagrafeBA1.NsFascicolo.getFascicoloResponse
        'Dim gF As New SincroAnagrafeBA1.NsFascicolo.getFascicolo

        Dim ws_FascicoloColdi As New SincroAnagrafeBA1.FascicoloWS_FascicoloHttpService(FascicoloWS_Uri)
        Dim ws_fasciResponse As SincroAnagrafeBA1.getFascicoloNewResponse
        Dim gF As New SincroAnagrafeBA1.getFascicoloNew

        Dim gS As New SincroAnagrafeBA1.getSchedeFascicolo
        'Dim Schede() As SincroAnagrafeBA1.SchedaValidazione
        Dim ws_SchedeResponse As SincroAnagrafeBA1.getSchedeFascicoloResponse

        Dim NomeRoutine As String = "Cerca Anagrafica"
        Dim MsgOK As String = ""

        gA.in = New SincroAnagrafeBA1.MsgGetAnagrafica
        gA.in.intestazione = New SincroAnagrafeBA1.Intestazione
        gA.in.intestazione.operatore = New SincroAnagrafeBA1.Operatore

        gA.in.intestazione.codDipartimentale = "AGR_RM"
        gA.in.intestazione.operatore.codice = Utente_BA_Username   '"lorenzo.belcapo"
        gA.in.intestazione.operatore.cf = Utente_BA_CF     '"BLCLNZ74R30A577M"
        gA.in.cf = CUAA

        'gF.cuaa = CUAA
        'gF.annoPrecedente = annoPrecedente
        'gF.filtro = SincroAnagrafeBA1.NsFascicolo.FiltroGetFascicolo.all

        'gS.cuaa = CUAA
        'gS.dataDa = DataDa
        'gS.dataA = DataA

        gS.in.cuaa = CUAA
        gS.in.dataDa = DataDa
        gS.in.dataA = DataA


        '------------------------------------------------------------------------------------------------------------
        '------------------------------------------------------------------------------------------------------------
        '------------------------------------------------------------------------------------------------------------
        '------------------------------------------------------------------------------------------------------------
        '------------------------------------------------------------------------------------------------------------

        Dim N_Schede As Integer = 0
        Dim s, g As Integer
        Dim Dt_Schede As New DataTable
        Dim Dt_Schede_DaProcessare As New DataTable
        Dim Dt_Schede_Gias As New DataTable
        Dim Dt_SchedeOrd As New DataTable
        Dim DrScheda As DataRow

        Dt_Schede.Columns.Add(New DataColumn("Scheda", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("Data", GetType(Date)))
        Dt_Schede.Columns.Add(New DataColumn("DataFine", GetType(Date)))
        Dt_Schede.Columns.Add(New DataColumn("Origine", GetType(String))) 'S=SIGPA G=Gias (solo Gias)
        Dt_Schede.Columns.Add(New DataColumn("Presente", GetType(Integer))) '0= nuova ; 1= già presente
        Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)
        Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod_Movimentate", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)
        Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod_NONMovimentate", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)

        Dt_Schede.Columns.Add(New DataColumn("idOPR", GetType(Integer)))
        Dt_Schede.Columns.Add(New DataColumn("desOPR", GetType(String)))
        Dt_Schede.Columns.Add(New DataColumn("OrigineOpr", GetType(String)))

        Try

            MsgOK = "Web Service Fascicolo: Invio Richiesta Schede"
            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
            logga(MsgOK)

            'Schede = ws_FascicoloColdi.getSchedeFascicolo(gS)
            ws_SchedeResponse = New getSchedeFascicoloResponse
            ws_SchedeResponse = ws_FascicoloColdi.getSchedeFascicolo(gS)

            If ws_SchedeResponse IsNot Nothing AndAlso _
                ws_SchedeResponse.out IsNot Nothing AndAlso _
                                  ws_SchedeResponse.out.Length > 0 Then

                N_Schede = ws_SchedeResponse.out.Length

                For s = 0 To ws_SchedeResponse.out.Length - 1

                    ''LEGGO SE LA SCHEDA E GIA PROCESSATA
                    'Dim Dt_Scheda_Processata As New DataTable
                    'Dt_Scheda_Processata = objSchedeProc.getFascicoliProcessati(CUAA, Schede(s).numeroScheda, "", "", ObjParametri_Server)

                    'If Dt_Scheda_Processata.Rows.Count = 0 Then

                    DrScheda = Dt_Schede.NewRow

                    DrScheda.Item("Scheda") = ws_SchedeResponse.out(s).numeroScheda
                    DrScheda.Item("Data") = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_SchedeResponse.out(s).dataScheda))
                    DrScheda.Item("Origine") = "S"
                    DrScheda.Item("Presente") = 0
                    DrScheda.Item("strProgrammazioniCod") = ""
                    DrScheda.Item("strProgrammazioniCod_Movimentate") = ""
                    DrScheda.Item("strProgrammazioniCod_NONMovimentate") = ""
                    DrScheda.Item("idOPR") = ws_SchedeResponse.out(s).idOPR
                    DrScheda.Item("desOPR") = ws_SchedeResponse.out(s).desOPR
                    DrScheda.Item("OrigineOpr") = ws_SchedeResponse.out(s).origineOPR
                    Dt_Schede.Rows.Add(DrScheda)

                    'End If

                Next


            End If

            'If Not Schede Is Nothing Then

            '    N_Schede = Schede.Length

            '    For s = 0 To Schede.Length - 1

            '        ''LEGGO SE LA SCHEDA E GIA PROCESSATA
            '        'Dim Dt_Scheda_Processata As New DataTable
            '        'Dt_Scheda_Processata = objSchedeProc.getFascicoliProcessati(CUAA, Schede(s).numeroScheda, "", "", ObjParametri_Server)

            '        'If Dt_Scheda_Processata.Rows.Count = 0 Then

            '        DrScheda = Dt_Schede.NewRow

            '        DrScheda.Item("Scheda") = Schede(s).numeroScheda
            '        DrScheda.Item("Data") = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Schede(s).dataScheda))
            '        DrScheda.Item("Origine") = "S"
            '        DrScheda.Item("Presente") = 0
            '        DrScheda.Item("strProgrammazioniCod") = ""
            '        DrScheda.Item("strProgrammazioniCod_Movimentate") = ""
            '        DrScheda.Item("strProgrammazioniCod_NONMovimentate") = ""
            '        Dt_Schede.Rows.Add(DrScheda)

            '        'End If

            '    Next

            'End If


            'AGGIUNGO LE SCHEDE PRESENTI SOLO SU GIAS
            Dim FiltroSchede As String = " Allegati_Documenti_CatCod = " & enum_CategorieDocumenti.DomandaFascicolo & " AND Allegati_Documenti_Piva='" & PIVA & "' "
            Dim FiltroPlanning As String = " Programmazione_Cod<>0 AND Programmazione_Entita_Cod=0 "
            Dim SchedaGiaPresente As Boolean
            Dim Allegati_Documenti_Cod As Integer
            Dim Allegati_EntitaxDocumenti_R As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim plan As Integer
            Dim strPlanning As String = ""
            Dim strPlanning_Movimentati As String = ""
            Dim strPlanning_NONMovimentati As String = ""
            Dim DtMov As New DataTable
            Dim Programmazione_Cod As Integer

            If N_Schede > 0 Then

                Dt_Schede_Gias = Allegati_Documenti_R.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                           FiltroSchede, " Validazione_Data ", ObjParametri_Server)

                For g = 0 To Dt_Schede_Gias.Rows.Count - 1

                    SchedaGiaPresente = False

                    'leggo i planning
                    Allegati_Documenti_Cod = Dt_Schede_Gias.Rows(g).Item("Allegati_Documenti_Cod")
                    Dim DtEntDoc As New DataTable
                    DtEntDoc = Allegati_EntitaxDocumenti_R.Leggi(Allegati_Documenti_Cod, PIVA, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, 0, 0, FiltroPlanning, "", ObjParametri_Server)
                    strPlanning = ""
                    strPlanning_Movimentati = ""
                    strPlanning_NONMovimentati = ""
                    For plan = 0 To DtEntDoc.Rows.Count - 1

                        Programmazione_Cod = DtEntDoc.Rows(plan).Item("Programmazione_Cod")
                        strPlanning &= Programmazione_Cod & "|"

                        '---------------------------------------------------------------------------
                        'VERIFICO se della programmazione sono stati MOVIMENTATI impianti ribaltati 
                        'in tal caso non modifico nulla
                        '---------------------------------------------------------------------------
                        DtMov = Reg_Impianti_Programmazioni_R.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", Programmazione_Cod, 0, "", "", ObjParametri_Server)

                        'MOVIMENTATO 
                        If DtMov.Rows.Count <> 0 Then
                            strPlanning_Movimentati &= Programmazione_Cod & "|"
                        Else
                            strPlanning_NONMovimentati &= Programmazione_Cod & "|"
                        End If

                    Next

                    If strPlanning <> "" Then
                        strPlanning = Left(strPlanning, strPlanning.Length - 1)
                    End If
                    If strPlanning_Movimentati <> "" Then
                        strPlanning_Movimentati = Left(strPlanning_Movimentati, strPlanning_Movimentati.Length - 1)
                    End If
                    If strPlanning_NONMovimentati <> "" Then
                        strPlanning_NONMovimentati = Left(strPlanning_NONMovimentati, strPlanning_NONMovimentati.Length - 1)
                    End If

                    For s = 0 To Dt_Schede.Rows.Count - 1
                        If Dt_Schede_Gias.Rows(g).Item("Allegati_Documenti_Numero") = Dt_Schede.Rows(s).Item("Scheda") Then
                            Dt_Schede.Rows(s).Item("Presente") = 1
                            Dt_Schede.Rows(s).Item("strProgrammazioniCod") = strPlanning
                            Dt_Schede.Rows(s).Item("strProgrammazioniCod_Movimentate") = strPlanning_Movimentati
                            Dt_Schede.Rows(s).Item("strProgrammazioniCod_NONMovimentate") = strPlanning_NONMovimentati
                            SchedaGiaPresente = True
                            Exit For
                        End If
                    Next

                    If Not SchedaGiaPresente Then
                        DrScheda = Dt_Schede.NewRow
                        DrScheda.Item("Scheda") = Dt_Schede_Gias.Rows(g).Item("Allegati_Documenti_Numero")
                        DrScheda.Item("Data") = CDate(Dt_Schede_Gias.Rows(g).Item("Validazione_Data"))
                        DrScheda.Item("Origine") = "G"
                        DrScheda.Item("Presente") = 1
                        DrScheda.Item("strProgrammazioniCod") = strPlanning
                        DrScheda.Item("strProgrammazioniCod_Movimentate") = strPlanning_Movimentati
                        DrScheda.Item("strProgrammazioniCod_NONMovimentate") = strPlanning_NONMovimentati
                        DrScheda.Item("idOPR") = 0
                        DrScheda.Item("desOPR") = ""
                        DrScheda.Item("OrigineOpr") = ""
                        Dt_Schede.Rows.Add(DrScheda)
                    End If

                Next

            End If


            If Dt_Schede.Rows.Count > 0 Then

                Dt_Schede_DaProcessare = Dt_Schede.Clone

                For s = 0 To Dt_Schede.Rows.Count - 1

                    'LEGGO SE LA SCHEDA E GIA PROCESSATA
                    Dim Dt_Scheda_Processata As New DataTable
                    Dt_Scheda_Processata = objSchedeProc.getFascicoliProcessati(CUAA, Dt_Schede.Rows(s).Item("Scheda"), "", "", ObjParametri_Server)

                    If Dt_Scheda_Processata.Rows.Count = 0 Then
                        Dt_Schede_DaProcessare.ImportRow(Dt_Schede.Rows(s))
                    End If

                Next

                If Dt_Schede_DaProcessare.Rows.Count > 0 Then

                    Dim dataView As New DataView(Dt_Schede_DaProcessare)
                    dataView.Sort = "Data"
                    Dt_SchedeOrd = dataView.ToTable

                    If Dt_SchedeOrd.Rows.Count > 1 Then
                        For s = 0 To Dt_SchedeOrd.Rows.Count - 2
                            Dt_SchedeOrd.Rows(s).Item("DataFine") = DateAdd(DateInterval.Day, -1, CDate(Dt_SchedeOrd.Rows(s + 1).Item("Data")))
                        Next
                    End If

                End If

            End If

            MsgOK = "Web Service Fascicolo: Risposta Schede Ricevuta"
            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Ricevute " & N_Schede & " Schede di validazione. Trovate su Gias " & Dt_Schede_Gias.Rows.Count & " Schede. Processate " & Dt_Schede_DaProcessare.Rows.Count & " Schede" '& MsgOK
            logga(MsgOK)

        Catch ex As Exception

            Dim strEr As String
            strEr = ex.Message

            MsgOK = "Web Service Fascicolo Schede ha restituito l'errore: " & ex.Message & " !"
            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
            logga(MsgOK)

            msgCarica &= "Web Service Fascicolo Schede: " & ex.Message & "."
            Return False

        End Try


        If Dt_SchedeOrd.Rows.Count <> 0 Then


            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------


            Try


                MsgOK = "Web Service Anagrafe: Invio Richiesta"
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                logga(MsgOK)

                ws_AnaResponse = ws_AnaColdi.cercaAnagrafica(gA)

                MsgOK = "Web Service Anagrafe: Risposta Ricevuta"
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                logga(MsgOK)


                RispostaWSAnagrafe = True

            Catch ex As Exception

                RispostaWSAnagrafe = False

                MsgOK = "Web Service Anagrafe ha restituito l'errore: " & ex.Message & " "
                If ex.Message = "-1" Then
                    MsgOK &= vbCrLf & "Verificare le credenziali con cui si è entrati su Gias."
                End If
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                logga(MsgOK)

                Dim strEr As String
                strEr = ex.Message

                msgCarica &= "Web Service Anagrafe ha restituito l'errore: " & ex.Message
                Return False

            End Try


            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            '------------------------------------------------------------------------------------------------------------
            'controllo coerenza dei dati ricevuti da ws anagrafe
            If IsNothing(ws_AnaResponse) Then
                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse è nothing per la PIVA " & PIVA & "."
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
                logga(MsgOK)
                msgCarica &= erroreDAti
                Return False
            End If
            If IsNothing(ws_AnaResponse.output) Then
                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output è nothing per la PIVA " & PIVA & "."
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
                logga(MsgOK)
                msgCarica &= erroreDAti
                Return False
            End If
            If IsNothing(ws_AnaResponse.output.anagrafica) Then
                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output.anagrafica è nothing per la PIVA " & PIVA & "."
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
                logga(MsgOK)
                msgCarica &= erroreDAti
                Return False
            End If
            If IsNothing(ws_AnaResponse.output.anagrafica.impresa) Then
                Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_AnaResponse.output.anagrafica.impresa è nothing per la PIVA " & PIVA & "."
                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
                logga(MsgOK)
                msgCarica &= erroreDAti
                Return False
            End If
            If PIVA <> "" Then
                If ws_AnaResponse.output.anagrafica.impresa.piva <> PIVA Then
                    Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, in ws_AnaResponse la PIVA non corrisponde con quella richiesta: ('" & ws_AnaResponse.output.anagrafica.impresa.piva & "'<>" & PIVA & ")"
                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & erroreDAti
                    logga(MsgOK)
                    msgCarica &= erroreDAti
                    Return False
                End If
            End If




            For s = 0 To Dt_SchedeOrd.Rows.Count - 1

                Dim OrigineScheda As String = Dt_SchedeOrd.Rows(s).Item("Origine")
                Dim OrigineSchedaPrec As String = ""
                If s > 0 Then
                    OrigineSchedaPrec = Dt_SchedeOrd.Rows(s - 1).Item("Origine")
                End If

                Dim Programmazione_Cod As Integer = 0

                Dim SchedaValidazione As String = ""
                Dim DataValidazione As Date = CostantiPersonalizzate.AGRODATAINIZIO
                Dim DataValidazioneFine As Date = CostantiPersonalizzate.AGRODATAFINE

                Dim SchedaValidazionePrec As String = ""
                Dim DataValidazionePrec As Date = CostantiPersonalizzate.AGRODATAINIZIO
                Dim DataValidazioneFinePrec As Date = CostantiPersonalizzate.AGRODATAFINE

                Dim DataAperturaFascicolo As Date = CostantiPersonalizzate.AGRODATAINIZIO
                Dim DataChiusuraFascicolo As Date = CostantiPersonalizzate.AGRODATAFINE
                Dim DataInizioMandato As Date = CostantiPersonalizzate.AGRODATAINIZIO
                Dim DataFineMandato As Date = CostantiPersonalizzate.AGRODATAFINE


                SchedaValidazione = Dt_SchedeOrd.Rows(s).Item("Scheda")
                DataValidazione = Dt_SchedeOrd.Rows(s).Item("Data")
                If IsDate(Dt_SchedeOrd.Rows(s).Item("DataFine")) Then
                    DataValidazioneFine = Dt_SchedeOrd.Rows(s).Item("DataFine")
                End If

                If s > 0 Then
                    SchedaValidazionePrec = Dt_SchedeOrd.Rows(s - 1).Item("Scheda")
                    DataValidazionePrec = Dt_SchedeOrd.Rows(s - 1).Item("Data")
                    If IsDate(Dt_SchedeOrd.Rows(s - 1).Item("DataFine")) Then
                        DataValidazioneFinePrec = Dt_SchedeOrd.Rows(s - 1).Item("DataFine")
                    End If
                End If

                Select Case OrigineScheda

                    '----------------------------
                    '----------------------------
                    '----------------------------
                    '----------------------------
                    '----------------------------
                    '----------------------------

                    'SCHEDA RICEVUTA DA SIGPA
                    Case "S"


                        Try

                            MsgOK = "Web Service Fascicolo: Invio Richiesta Scheda"
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
                            logga(MsgOK)

                            gF.cuaa = CUAA
                            gF.filtro = FiltroGetFascicolo.all
                            gF.numSchedaValidaz = Dt_SchedeOrd.Rows(s).Item("Scheda") 'Schede(s).numeroScheda

                            Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                            'AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, ws_fasciResponse, , False)
                            ws_fasciResponse = ws_FascicoloColdi.getFascicoloNew(gF)

                            MsgOK = "Web Service Fascicolo: Risposta Scheda Ricevuta"
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
                            logga(MsgOK)

                            RispostaWSFascicolo = True

                        Catch ex As Exception

                            RispostaWSFascicolo = False

                            Dim strEr As String
                            strEr = ex.Message

                            MsgOK = "Web Service Fascicolo Scheda ha restituito l'errore: " & ex.Message & " !"
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & " -  " & MsgOK
                            logga(MsgOK)

                            msgCarica &= "Web Service Fascicolo Scheda: " & ex.Message & "."

                            Continue For


                        End Try




                        '------------------------------------------------------------------------------------------------------------
                        '------------------------------------------------------------------------------------------------------------
                        '------------------------------------------------------------------------------------------------------------
                        'controllo coerenza dei dati ricevuti da ws anagrafe
                        If IsNothing(ws_fasciResponse) Then
                            Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse è nothing per la PIVA " & PIVA & "."
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                            logga(MsgOK)
                            msgCarica &= erroreDAti
                            Continue For
                            'Return False
                        End If
                        If IsNothing(ws_fasciResponse.out) Then
                            Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out è nothing per la PIVA " & PIVA & "."
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                            logga(MsgOK)
                            msgCarica &= erroreDAti
                            Continue For
                            'Return False
                        End If
                        If IsNothing(ws_fasciResponse.out.fascicolo) Then
                            Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out.fascicolo è nothing per la PIVA " & PIVA & "."
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                            logga(MsgOK)
                            msgCarica &= erroreDAti
                            Continue For
                            'Return False
                        End If
                        If IsNothing(ws_fasciResponse.out.fascicolo.fascicolo) Then
                            Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, ws_fasciResponse.out.fascicolo.fascicolo è nothing per la PIVA " & PIVA & "."
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                            logga(MsgOK)
                            msgCarica &= erroreDAti
                            Continue For
                            'Return False
                        End If
                        'If PIVA <> "" Then
                        '    If ws_fasciResponse.out.fascicolo.fascicolo.PartitaIVA <> PIVA Then
                        '        Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati, in ws_fasciResponse la PIVA non corrisponde: ('" & ws_fasciResponse.out.fascicolo.fascicolo.PartitaIVA & "'<>" & PIVA & ")."
                        '        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                        '        logga(MsgOK)
                        '        msgCarica &= erroreDAti
                        '        Return False
                        '    End If
                        'End If

                        If ws_fasciResponse.out.fascicolo.fascicolo.CUAA <> CUAA Then
                            Dim erroreDAti As String = "ATTENZIONE, grave incongruenza nei dati,  in ws_fasciResponse il cuaa non corrisponde: ('" & ws_fasciResponse.out.fascicolo.fascicolo.CUAA & "'<>" & CUAA & ")."
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & ", Scheda N° " & Dt_SchedeOrd.Rows(s).Item("Scheda") & "  -  " & erroreDAti
                            logga(MsgOK)
                            msgCarica &= erroreDAti
                            Continue For
                            'Return False
                        End If

                        '------------------------------------------------------------------------------------------------------------
                        '------------------------------------------------------------------------------------------------------------
                        '------------------------------------------------------------------------------------------------------------




                        Try

                            If RispostaWSAnagrafe AndAlso _
                                ws_AnaResponse IsNot Nothing AndAlso _
                                ws_AnaResponse.output IsNot Nothing AndAlso _
                                ws_AnaResponse.output.anagrafica IsNot Nothing Then

                                Presente_Anagrafe = True

                                MsgOK = "Anagrafe presente!"
                                MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                logga(MsgOK)


                                If RispostaWSFascicolo AndAlso _
                                    ws_fasciResponse IsNot Nothing AndAlso _
                                    ws_fasciResponse.out IsNot Nothing AndAlso _
                                    ws_fasciResponse.out.fascicolo IsNot Nothing Then

                                    Presente_Fascicolo = True

                                    Dim bEsisteUffZona As Boolean = False
                                    Dim bEsisteUffProv As Boolean = False

                                    If Not EsisteImpresa Then

                                        If ws_fasciResponse.out.fascicolo.fascicolo.Detentore IsNot Nothing Then

                                            Detentore_Fascicolo = ws_fasciResponse.out.fascicolo.fascicolo.Detentore

                                            If Detentore_Fascicolo <> "" AndAlso Detentore_Fascicolo.Length = 9 Then

                                                Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                                                bEsisteUffZona = objImpresa.Esiste_Impresa("UZ" & Detentore_Fascicolo, 0, Nothing, Nothing, "", "", ObjParametri_Server)

                                                If bEsisteUffZona Then
                                                    Piva_Padre = "UZ" & Detentore_Fascicolo
                                                Else
                                                    bEsisteUffProv = objImpresa.Esiste_Impresa("UP" & Left(Detentore_Fascicolo, Detentore_Fascicolo.Length - 3) & "000", 0, Nothing, Nothing, "", "", ObjParametri_Server)
                                                    If bEsisteUffProv Then
                                                        Piva_Padre = "UP" & Left(Detentore_Fascicolo, Detentore_Fascicolo.Length - 3) & "000"
                                                    Else
                                                        Piva_Padre = SuperUserPiva
                                                    End If
                                                End If

                                            Else

                                                MsgOK = "Detentore_Fascicolo = '' or Detentore_Fascicolo.Length = 9 "
                                                logga(MsgOK)
                                                msgCarica &= "Dati non presenti(" & MsgOK & ")!"
                                                Continue For
                                                'Return False

                                            End If

                                        Else

                                            MsgOK = "Detentore_Fascicolo = Nothing "
                                            logga(MsgOK)
                                            msgCarica &= "Dati non presenti(" & MsgOK & ")!"
                                            Continue For

                                        End If

                                    End If


                                    'If Not ws_fasciResponse.out.fascicolo.fascicolo.SchedaValidazione Is Nothing Then
                                    '    SchedaValidazione = ws_fasciResponse.out.fascicolo.fascicolo.SchedaValidazione
                                    'End If
                                    'If Not ws_fasciResponse.out.fascicolo.fascicolo.DataSchedaValidazione Is Nothing Then
                                    '    DataValidazione = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataSchedaValidazione))
                                    'End If
                                    If ws_fasciResponse.out.fascicolo.fascicolo.DataSottMandato IsNot Nothing Then
                                        DataInizioMandato = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataSottMandato))
                                    End If
                                    If ws_fasciResponse.out.fascicolo.fascicolo.DataAperturaFascicolo IsNot Nothing Then
                                        DataAperturaFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataAperturaFascicolo))
                                    End If
                                    If ws_fasciResponse.out.fascicolo.fascicolo.DataChiusuraFascicolo IsNot Nothing Then
                                        DataChiusuraFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.DataChiusuraFascicolo))
                                    End If


                                    '---------------------------------------------------------------------------------------------------------------------------------------
                                    '---------------------------------------------------------------------------------------------------------------------------------------
                                    '---------------------------------------------------------------------------------------------------------------------------------------
                                    'Se la variabile
                                    'Aggiorna_Solo_Se_Fascicolo_Nuovo =true
                                    'allora l'aggiornamento lo devo fare solo se il fascicolo è nuovo, quindi
                                    'devo controllare se il fascicolo è nuovo,
                                    'se Azienda_Con_Fascicolo_Nuovo = true proseguo
                                    'se Azienda_Con_Fascicolo_Nuovo = false esco con true così non faccio la serializzazione dell'oggetto e lascio il messaggio
                                    If Not Aggiorna_Solo_Se_Fascicolo_Nuovo Then
                                        'non faccio nulla
                                        MsgOK = "Fascicolo ricevuto, lo aggiorno a prescindere che sia gia presente."
                                        logga(MsgOK)
                                    Else
                                        If SchedaValidazione = "" Then
                                            MsgOK = "scheda validazione nulla"
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)
                                            msgCarica &= "SchedaValidazione nulla"
                                            Continue For
                                            'Return True
                                        End If

                                        Crea_Fascicolo = False
                                        Ribalta_Fascicolo = False

                                        'controllo se il fascicolo è presente
                                        If Dt_SchedeOrd.Rows(s).Item("Presente") = 1 Then

                                            Crea_Fascicolo = True

                                            'NON MOVIMENTATO ---> CANCELLO PLANNING + RIBALTATI
                                            Dim p As Integer
                                            If Dt_SchedeOrd.Rows(s).Item("strProgrammazioniCod_NONMovimentate") <> "" Then
                                                Dim PlanningCodNONMov() As String = Split(Dt_SchedeOrd.Rows(s).Item("strProgrammazioniCod_NONMovimentate"), "|")
                                                If PlanningCodNONMov IsNot Nothing Then
                                                    For p = 0 To PlanningCodNONMov.Length - 1
                                                        Dim Programmazione_W As New AgronicaCoreAnagrafeBIZ.Programmazione_W
                                                        Dim intDummy As Integer
                                                        intDummy = Programmazione_W.Pianificazione_Scrivi( _
                                                                              enum_TipoOperazioneDB.Cancellazione, _
                                                                                     PlanningCodNONMov(p), _
                                                                                     "", _
                                                                                     "", _
                                                                                     PIVA, _
                                                                                     "", _
                                                                                     SuperUserUsername, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     Nothing, _
                                                                                     "", _
                                                                                     CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                                     ObjParametri_Server)

                                                        _4_Elimina_Ribaltati(PlanningCodNONMov(p))

                                                        Ribalta_Fascicolo = True

                                                    Next

                                                End If

                                            End If

                                        Else

                                            Crea_Fascicolo = True

                                            If DataValidazione >= #1/1/2014# AndAlso s = Dt_SchedeOrd.Rows.Count - 1 Then
                                                Ribalta_Fascicolo = True
                                            End If

                                            MsgOK = "Fascicolo: " & SchedaValidazione & " nuovo, lo importo."
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)

                                        End If

                                    End If

                                    '---------------------------------------------------------------------------------------------------------------------------------------
                                    '---------------------------------------------------------------------------------------------------------------------------------------
                                    '---------------------------------------------------------------------------------------------------------------------------------------




                                    Dim Path As String = ""

                                    Path = DirectoryFileImportazioni & "\" & CUAA & "_" & SchedaValidazione & "_fascicolo.xml"

                                    'se il fascicolo con questa scheda validazione esiste lo cancello e risalvo
                                    'altrimenti lo salvo
                                    If File.Exists(Path) Then
                                        File.Delete(Path)
                                    End If

                                    SerializeObject(Path, ws_fasciResponse)

                                    XmlFascicolo = objXML.Xml_Pubblico_Fascicolo(Path, _
                                                                                 SchedaValidazione, _
                                                                              DataValidazione, _
                                                                              Detentore_Fascicolo, _
                                                                              DataInizioMandato, _
                                                                              DataFineMandato, _
                                                                              DataAperturaFascicolo, _
                                                                              DataChiusuraFascicolo, _
                                                                              XmlDoc)

                                    XmlFascicoloP = objXML.Xml_Pubblico_Fascicolo(Path, _
                                                                                 SchedaValidazione, _
                                                                              DataValidazione, _
                                                                              Detentore_Fascicolo, _
                                                                              DataInizioMandato, _
                                                                              DataFineMandato, _
                                                                              DataAperturaFascicolo, _
                                                                              DataChiusuraFascicolo, _
                                                                              XmlDocP)




                                Else
                                    'RispostaWSFascicolo = True AndAlso _
                                    'Not ws_fasciResponse Is Nothing AndAlso _
                                    'Not ws_fasciResponse.out Is Nothing AndAlso _
                                    'Not ws_fasciResponse.out.fascicolo Is Nothing
                                    MsgOK = "RispostaWSFascicolo=false o  ws_fasciResponse,ws_fasciResponse.out,ws_fasciResponse.out.fascicolo Is Nothing"
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)

                                    msgCarica &= "Dati non presenti(" & MsgOK & ")!"
                                    Continue For
                                    'Return False

                                End If


                                If Not Presente_Fascicolo Then
                                    MsgOK = "Fascicolo NON presente!"
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)
                                Else

                                End If




                                '---------------------------------------------------------------------------

                                'a seconda del tipo di anagrafica ho un oggetto diverso
                                'ditta individuale : ho sia i dati dell'impresa sia quelli del rappresentante
                                'società : ho solo i dati dell'impresa (non ho i tag del rappresentante)
                                '(persona : ho solo i dati anagrafici del legale (non ho il tag impresa))
                                Select Case ws_AnaResponse.output.anagrafica.codTipoAnagrafica

                                    Case "DI", "S"   'ditta individuale, 'società


                                        '*******************************************************************************************************************
                                        '******   IMPRESA      *********************************************************************************************
                                        '*******************************************************************************************************************

                                        RagSoc = ws_AnaResponse.output.anagrafica.impresa.ragioneSociale

                                        If ws_AnaResponse.output.anagrafica.impresa.piva = "" Then
                                            'pivazzz non presente
                                            Throw New Exception("Impossibile inserire l'impresa poiché manca la Partita IVA!")
                                        End If

                                        'If PIVA <> ws_AnaResponse.output.anagrafica.impresa.piva Then
                                        '    Throw New Exception("PIVA <> ws_AnaResponse.output.anagrafica.impresa.piva")
                                        'End If
                                        If PIVA = "" AndAlso ws_AnaResponse.output.anagrafica.impresa.piva <> "" Then
                                            PIVA = ws_AnaResponse.output.anagrafica.impresa.piva
                                            Throw New Exception("PIVA <> ws_AnaResponse.output.anagrafica.impresa.piva")
                                        End If

                                        ID_Azienda = ws_AnaResponse.output.anagrafica.identita.master

                                        Cod_Belfiore = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.comune.codErariale

                                        Indirizzo = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.toponimo & " " & _
                                                    ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.descrizione & " " & _
                                                    ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.civico & " "

                                        Cap = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.cap


                                        Localita = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.localita



                                        '-----------------------------------------------
                                        '------- UTENTE        -------------------------
                                        '-----------------------------------------------
                                        XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc, _
                                                                               SuperUserUsername, _
                                                                               SuperUserPassword, _
                                                                               ProgressivoGIAS)

                                        XmlUtenteP = objXML.Xml_Pubblico_Utente(XmlDocP, _
                                                                               SuperUserUsername, _
                                                                               SuperUserPassword, _
                                                                               ProgressivoGIAS)

                                        Comune = ""
                                        Dim objIstat As New AgronicaCoreMetaSchemaDAL.ISTAT_Comuni_R
                                        Dim Dt_Istat As New DataTable

                                        Dt_Istat = objIstat.Leggi("", _
                                                               "", _
                                                               "", _
                                                               "", _
                                                               "", _
                                                               Cod_Belfiore, _
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                                 "", "", _
                                                                 ObjParametri_Server)

                                        If Dt_Istat IsNot Nothing AndAlso Dt_Istat.Rows.Count > 0 Then

                                            Provincia = Dt_Istat.Rows(0).Item("Provincia_Long")
                                            Comune = Dt_Istat.Rows(0).Item("Descrizione")
                                            Istat_Provincia = Dt_Istat.Rows(0).Item("Pro_Cod_Istat")
                                            Istat_Comune = Dt_Istat.Rows(0).Item("Com_Cod_Istat")

                                            'If Presente_Fascicolo = False Then
                                            '    ' Cmb_Padre.SelectedIndex = Cmb_Padre.Items.IndexOf(Cmb_Padre.Items.FindByValue("UP103" & Istat_Provincia & "000"))
                                            'End If

                                        End If



                                        '-----------------------------------------------
                                        '------- RAPPRESENTANTE LEGALE -----------------
                                        '-----------------------------------------------

                                        'In caso di Ditta Individuale Importo anche il rappresentante legale!
                                        If ws_AnaResponse.output.anagrafica.codTipoAnagrafica = "DI" Then

                                            'Dim strRuolo As String = String.Empty
                                            'strRuolo = "<b>Ruolo: </b> Legale Rappresentante  <br/>"

                                            Dim strReferente As String

                                            If ws_AnaResponse.output.anagrafica.contatto IsNot Nothing Then

                                                strReferente = ws_AnaResponse.output.anagrafica.nome & " " & _
                                                  ws_AnaResponse.output.anagrafica.cognome & " <br/>" & _
                                                  "<b>CF: </b>" & ws_AnaResponse.output.anagrafica.cf & " <br/>" & _
                                                  IIf(ws_AnaResponse.output.anagrafica.contatto.tel <> String.Empty, "<b>Telefono: </b>" & ws_AnaResponse.output.anagrafica.contatto.tel, "") & _
                                                  IIf(ws_AnaResponse.output.anagrafica.contatto.cellulare <> String.Empty, "<br/><b>Cellulare: </b>" & ws_AnaResponse.output.anagrafica.contatto.cellulare, "") & _
                                                  IIf(ws_AnaResponse.output.anagrafica.contatto.email <> String.Empty, "<br/><b>E-mail: </b>" & ws_AnaResponse.output.anagrafica.contatto.email, "") & _
                                                  IIf(ws_AnaResponse.output.anagrafica.contatto.fax <> String.Empty, "<br/><b>Fax: </b>" & ws_AnaResponse.output.anagrafica.contatto.fax, "")

                                                If ws_AnaResponse.output.anagrafica.contatto.tel <> "" Then
                                                    Legale_Rappresentante_Rubrica1 = "Telefono:" & ws_AnaResponse.output.anagrafica.contatto.tel
                                                End If
                                                If ws_AnaResponse.output.anagrafica.contatto.cellulare <> "" Then
                                                    Legale_Rappresentante_Rubrica2 = "Cellulare:" & ws_AnaResponse.output.anagrafica.contatto.cellulare
                                                End If
                                                If ws_AnaResponse.output.anagrafica.contatto.email <> "" Then
                                                    Legale_Rappresentante_Rubrica3 = "Email:" & ws_AnaResponse.output.anagrafica.contatto.email
                                                End If
                                                If ws_AnaResponse.output.anagrafica.contatto.fax <> "" Then
                                                    Legale_Rappresentante_Rubrica4 = "Fax:" & ws_AnaResponse.output.anagrafica.contatto.fax
                                                End If

                                            Else

                                                strReferente = ws_AnaResponse.output.anagrafica.nome & " " & _
                                                                  ws_AnaResponse.output.anagrafica.cognome & " <br/>" & _
                                                                  "<b>CF: </b>" & ws_AnaResponse.output.anagrafica.cf
                                            End If


                                            Legale_Rappresentante_Nome = ws_AnaResponse.output.anagrafica.nome
                                            Legale_Rappresentante_Cognome = ws_AnaResponse.output.anagrafica.cognome
                                            Legale_Rappresentante_CF = ws_AnaResponse.output.anagrafica.cf
                                            Legale_Rappresentante_Sesso = ws_AnaResponse.output.anagrafica.sesso
                                            Legale_Rappresentante_Data_Nascita = ws_AnaResponse.output.anagrafica.dataNascita.ToShortDateString

                                            Legale_Rappresentante_Provincia_Nascita = "#"
                                            Legale_Rappresentante_Comune_Nascita = "#"
                                            Legale_Rappresentante_Istat_Provincia_Nascita = "#"
                                            Legale_Rappresentante_Istat_Comune_Nascita = "#"

                                            Legale_Rappresentante_Provincia = "#"
                                            Legale_Rappresentante_Comune = "#"
                                            Legale_Rappresentante_Istat_Provincia = "#"
                                            Legale_Rappresentante_Istat_Comune = "#"
                                            Legale_Rappresentante_Indirizzo = "#"
                                            Legale_Rappresentante_Frazione = "#"
                                            Legale_Rappresentante_Cap = "#"
                                            Legale_Rappresentante_Stato = "#"

                                            If ws_AnaResponse.output.anagrafica.indirizzoResidenza IsNot Nothing Then

                                                Dt_Istat = objIstat.Leggi("", _
                                                          "", _
                                                          "", _
                                                          "", _
                                                          "", _
                                                          ws_AnaResponse.output.anagrafica.indirizzoResidenza.comune.codErariale, _
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                           "", "", _
                                                           ObjParametri_Server)

                                                If Dt_Istat IsNot Nothing AndAlso Dt_Istat.Rows.Count > 0 Then

                                                    Legale_Rappresentante_Provincia = Dt_Istat.Rows(0).Item("Provincia_Long")
                                                    Legale_Rappresentante_Comune = Dt_Istat.Rows(0).Item("Descrizione")
                                                    Legale_Rappresentante_Istat_Provincia = Dt_Istat.Rows(0).Item("Pro_Cod_Istat")
                                                    Legale_Rappresentante_Istat_Comune = Dt_Istat.Rows(0).Item("Com_Cod_Istat")

                                                    Legale_Rappresentante_Indirizzo = ws_AnaResponse.output.anagrafica.indirizzoResidenza.toponimo & " " & ws_AnaResponse.output.anagrafica.indirizzoResidenza.descrizione & " " & ws_AnaResponse.output.anagrafica.indirizzoResidenza.civico
                                                    Legale_Rappresentante_Frazione = "#"
                                                    Legale_Rappresentante_Cap = ws_AnaResponse.output.anagrafica.indirizzoResidenza.cap
                                                    Legale_Rappresentante_Stato = "IT"

                                                End If

                                            End If

                                        Else
                                            ' Me.Lbl_Dettagli_Referente.Text = "---"
                                        End If

                                        '-----------------------------------------------
                                        '------- IMPRESA       -------------------------
                                        '-----------------------------------------------


                                        If EsisteImpresa Then
                                            TipoOperazione = enum_TipoOperazioneDB.Modifica
                                        End If

                                        XmlImpresa = objXML.Xml_Pubblico_Impresa(TipoOperazione, _
                                                                                 PIVA, _
                                                                                 RagSoc, _
                                                                                 CUAA, _
                                                                                 CUAA, _
                                                                                 "#", _
                                                                                 "#", _
                                                                                 Piva_Padre, _
                                                                                 "1", _
                                                                                 "1", _
                                                                                 "#", "#", _
                                                                                 Indirizzo, _
                                                                                 Localita, _
                                                                                 Cap, _
                                                                                 Comune, _
                                                                                 Provincia, _
                                                                                 "#", "#", _
                                                                                 Istat_Comune, _
                                                                                 Istat_Provincia, _
                                                                                 "", _
                                                                                 Legale_Rappresentante_Cognome, _
                                                                                 Legale_Rappresentante_Nome, _
                                                                                 Legale_Rappresentante_CF, _
                                                                                 Legale_Rappresentante_Sesso, _
                                                                                 "#", "#", _
                                                                                 Legale_Rappresentante_Indirizzo, _
                                                                                 Legale_Rappresentante_Frazione, _
                                                                                 Legale_Rappresentante_Cap, _
                                                                                 Legale_Rappresentante_Comune, _
                                                                                 Legale_Rappresentante_Provincia, _
                                                                                 Legale_Rappresentante_Stato, _
                                                                                 "#", _
                                                                                 Legale_Rappresentante_Istat_Comune, _
                                                                                 Legale_Rappresentante_Istat_Provincia, _
                                                                                 Legale_Rappresentante_Data_Nascita, _
                                                                                 Legale_Rappresentante_Comune_Nascita, _
                                                                                 Legale_Rappresentante_Provincia_Nascita, _
                                                                                 Legale_Rappresentante_Istat_Comune_Nascita, _
                                                                                 Legale_Rappresentante_Istat_Provincia_Nascita, _
                                                                                 "#", _
                                                                                 Legale_Rappresentante_Rubrica1, _
                                                                                 Legale_Rappresentante_Rubrica2, _
                                                                                 Legale_Rappresentante_Rubrica3, _
                                                                                 Legale_Rappresentante_Rubrica4, _
                                                                                 Legale_Rappresentante_Rubrica5, _
                                                                                 "#", "#", "#", "#", "#", "#", _
                                                                                 ID_Azienda, _
                                                                                 XmlDoc)

                                        If Presente_Fascicolo Then
                                            XmlImpresa.AppendChild(XmlFascicolo)
                                        End If


                                        If TipoOperazione = enum_TipoOperazioneDB.Modifica Then
                                            Sa_Cod = RecuperaSaCodImpresa(Codice_Chiave_Cliente, PIVA, ID_Azienda)
                                        End If


                                        XmlCentro = objXML.Xml_Pubblico_CentroAziendale(TipoOperazione, _
                                                                                        Sa_Cod, _
                                                                                        "Centro n.01", _
                                                                                        1, _
                                                                                        "#", "#", "#", "#", "#", "#", _
                                                                                        Indirizzo, _
                                                                                        "#", _
                                                                                        Cap, _
                                                                                        Comune, _
                                                                                        Provincia, _
                                                                                        "#", "#", _
                                                                                        Istat_Comune, _
                                                                                        Istat_Provincia, _
                                                                                        "#", "#", "#", "#", "#", "#", "#", "#", "#", _
                                                                                        ID_Azienda, _
                                                                                        XmlDoc)

                                        'Creo il fabbricato
                                        '1. in caso di primo inserimento
                                        '2. in caso di modifica se non esiste già un magazzino
                                        Dim CreaMagazzino As Boolean = False
                                        If TipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                                            CreaMagazzino = True
                                        Else
                                            Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                            Dim DtFabb As DataTable
                                            DtFabb = objFabb.Leggi(PIVA, Sa_Cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                                 " Tipo_Fabbricato_Cod=20 ", _
                                                                 "", ObjParametri_Server)
                                            If DtFabb.Rows.Count = 0 Then
                                                CreaMagazzino = True
                                            End If
                                        End If

                                        If CreaMagazzino Then
                                            XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura, _
                                                                                          "0", _
                                                                                          "Magazzino n.01", _
                                                                                          20, _
                                                                                          "#", "#", "#", _
                                                                                          "#", _
                                                                                          "#", _
                                                                                          "#", _
                                                                                          CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                                          CostantiPersonalizzate.AGRODATAFINE, _
                                                                                          Indirizzo, _
                                                                                          "#", _
                                                                                          Cap, _
                                                                                          Comune, _
                                                                                          Provincia, _
                                                                                          "#", "#", _
                                                                                          Istat_Comune, _
                                                                                          Istat_Provincia, _
                                                                                          ID_Azienda, _
                                                                                          XmlDoc)

                                            XmlCentro.AppendChild(XmlFabbricato)
                                        End If

                                        XmlImpresa.AppendChild(XmlCentro)

                                        XmlUtente.AppendChild(XmlImpresa)

                                        STR_XmlDoc = XmlUtente.OuterXml



                                        '*******************************************************************************************************************
                                        '******   CONDIZIONALITA    ****************************************************************************************
                                        '*******************************************************************************************************************

                                        'verifico se esiste già su GIAS un PROFILO VALIDO...
                                        'se esiste NON LO CREO ORA!!!
                                        Dim Dt_Interviste As DataTable
                                        Dim CreaProfilo As Boolean = True

                                        Dim objAuditReg As New AgronicaCoreAuditDAL.Audit_Regolamenti_R
                                        Dim DtReg As DataTable
                                        Dim DataInizioReg, DataFineReg As Date

                                        'anno corrente
                                        DataInizioReg = CDate("01/01/" & Now.Year)
                                        DataFineReg = CDate("31/12/" & Now.Year)

                                        ' se ce n'è più di uno prendo cmq il primo, il più recente
                                        DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, DataInizioReg, DataFineReg, "", "", ObjParametri_Server)

                                        If DtReg.Rows.Count > 0 Then
                                            RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
                                        Else
                                            ' leggo il regolamento audit più recente
                                            DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, #1/1/1900#, #12/31/1900#, "", "", ObjParametri_Server)
                                            If DtReg.Rows.Count > 0 Then
                                                RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
                                            End If
                                        End If


                                        Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
                                        Dt_Interviste = objAudit_Interviste.Leggi(1, _
                                                                                  RegolamentoCod, _
                                                                                  0, _
                                                                                  PIVA, _
                                                                                  CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, _
                                                                                  "", "", _
                                                                                  ObjParametri_Server)

                                        If Dt_Interviste IsNot Nothing AndAlso Dt_Interviste.Rows.Count > 0 Then
                                            CreaProfilo = False
                                        End If

                                        If CreaProfilo Then

                                            If ws_fasciResponse IsNot Nothing AndAlso _
                                                ws_fasciResponse.out IsNot Nothing AndAlso _
                                                ws_fasciResponse.out.fascicolo IsNot Nothing AndAlso _
                                                ws_fasciResponse.out.fascicolo.fascicolo IsNot Nothing AndAlso _
                                                ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita IsNot Nothing Then

                                                If ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita.Length > 0 Then

                                                    'verifico se esiste già su GIAS un profilo valido...

                                                    Dim objCoreVarie As New AgronicaCoreVarieDAL.Codifica_Codici


                                                    Dim DR As DataRow
                                                    Dim Codice_Condizionalita As String = ""
                                                    Dim Descrizione As String = ""
                                                    Dim Regolamento_Cod As Integer = 1
                                                    Dim Domanda_Cod As Integer = 0

                                                    DtCondizionalita = New DataTable
                                                    DtCondizionalita.Columns.Add(New DataColumn("Raccoglitore_Cod", GetType(Integer)))
                                                    DtCondizionalita.Columns.Add(New DataColumn("Audit_Tipo", GetType(Integer)))
                                                    DtCondizionalita.Columns.Add(New DataColumn("Domanda_Cod", GetType(Integer)))
                                                    DtCondizionalita.Columns.Add(New DataColumn("Codice", GetType(String)))
                                                    DtCondizionalita.Columns.Add(New DataColumn("Descrizione", GetType(String)))

                                                    For i = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita.Length - 1

                                                        Codice_Condizionalita = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita(i).CodiceCondizionalita), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSCondizionalita(i).CodiceCondizionalita)

                                                        objCoreVarie.Converti_Audit_Domande_Interviste(Codice_Condizionalita, _
                                                                                                       Regolamento_Cod, _
                                                                                                       enum_AuditPuaTipo.Audit_Condizionalita, _
                                                                                                       Domanda_Cod, _
                                                                                                        Descrizione, _
                                                                                                       "", _
                                                                                                       ObjParametri_Server)

                                                        'inserisco la domanda solo se c'è corrispondenza con GIAS
                                                        If Domanda_Cod <> 0 Then

                                                            DR = DtCondizionalita.NewRow

                                                            DR.Item("Codice") = Codice_Condizionalita
                                                            DR.Item("Raccoglitore_Cod") = Regolamento_Cod
                                                            DR.Item("Audit_Tipo") = enum_AuditPuaTipo.Audit_Condizionalita
                                                            DR.Item("Domanda_Cod") = Domanda_Cod
                                                            DR.Item("Descrizione") = Descrizione

                                                            DtCondizionalita.Rows.Add(DR)

                                                        End If

                                                    Next

                                                Else


                                                End If

                                            End If

                                        Else

                                        End If


                                        '*******************************************************************************************************************
                                        '******   PARTICELLE CATASTALI    **********************************************************************************
                                        '*******************************************************************************************************************
                                        If ws_fasciResponse IsNot Nothing AndAlso _
                                            ws_fasciResponse.out IsNot Nothing AndAlso _
                                            ws_fasciResponse.out.fascicolo IsNot Nothing AndAlso _
                                            ws_fasciResponse.out.fascicolo.fascicolo IsNot Nothing AndAlso _
                                            ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1 IsNot Nothing Then

                                            If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1.Length > 0 Then

                                                Dim Anno As Integer

                                                Anno = Today.Year
                                                If annoPrecedente Then
                                                    Anno = Today.Year - 1
                                                End If


                                                'Dim strValiditaInizio As String = String.Empty
                                                'Dim strValiditaFine As String = String.Empty
                                                'strValiditaInizio = "01/11/" & (Anno - 1).ToString
                                                'strValiditaFine = "31/10/" & Anno.ToString

                                                Dim strValiditaInizio As String = DataValidazione
                                                Dim strValiditaFine As String = DataValidazioneFine


                                                '*******************************************************************************************************************
                                                '******   PIANIFICAZIONE    **********************************************************************************

                                                Dim strPianificazione As String

                                                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                                                Programmazione_Cod = ObjSequenze.NuovoId_Tabella( _
                                                                       "Programmazione_Testata", _
                                                                        Nothing, _
                                                                        Nothing, _
                                                                        ObjParametri_Server)

                                                If DataValidazione <> CostantiPersonalizzate.AGRODATAINIZIO Then
                                                    strPianificazione = "Fascicolo N." & SchedaValidazione & " Data Validazione " & DataValidazione.ToShortDateString & " (Origine SIGPA)"
                                                Else
                                                    strPianificazione = "Fascicolo N." & SchedaValidazione & " Data Validazione non presente (Origine SIGPA)"
                                                End If

                                                XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(enum_TipoOperazioneDB.Scrittura, _
                                                                   Programmazione_Cod.ToString, _
                                                                   PIVA, _
                                                                   strPianificazione, _
                                                                   strPianificazione, _
                                                                   "Importazione " & strPianificazione, _
                                                                    enum_Planning_Fonte.BluArancioAgea, _
                                                                    enum_TipoPianificazione.Pianificazione_Annuale, _
                                                                   strValiditaInizio, _
                                                                   strValiditaFine, _
                                                                   XmlDocP)

                                                XmlUtenteP.AppendChild(XmlTestata)

                                                If Presente_Fascicolo Then
                                                    XmlTestata.AppendChild(XmlFascicoloP)
                                                End If

                                                STR_XmlDocP = XmlUtenteP.OuterXml

                                                Dim DR As DataRow

                                                DtParticelle = New DataTable
                                                DtParticelle.Columns.Add(New DataColumn("PROV", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("COM", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("Sezione", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Numero", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Subalterno", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("datepossesso", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("inizio_possesso", GetType(Date)))
                                                DtParticelle.Columns.Add(New DataColumn("fine_possesso", GetType(Date)))
                                                DtParticelle.Columns.Add(New DataColumn("catasto", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("possesso", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("TitoloPossesso", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("macrouso", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("macrouso_sup", GetType(Double)))
                                                DtParticelle.Columns.Add(New DataColumn("sup", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("supcondotta", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("utilizzo", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("utilizzo_sup", GetType(Double)))
                                                DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
                                                DtParticelle.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
                                                DtParticelle.Columns.Add(New DataColumn("Scarto", GetType(Integer)))

                                                For i = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1.Length - 1

                                                    'DR = DtParticelle.NewRow

                                                    Prov = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Provincia.Trim), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Provincia.Trim)
                                                    Com = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Comune.Trim), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Comune.Trim)
                                                    Sezione = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Sezione.Trim), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Sezione.Trim)
                                                    Foglio = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Foglio.Trim), 0, ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Foglio.Trim)
                                                    strNumero = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Particella.Trim), 0, ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Particella.Trim)
                                                    Subalterno = IIf(IsNothing(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Subalterno.Trim), "", ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).Subalterno.Trim)

                                                    'verifico cosa trovo nel campo particella (su Agea è una stringa)
                                                    'se trovo dei numeri li metto in numero
                                                    'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
                                                    Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
                                                    Numero = objCOre.Numero_from_Stringa(strNumero)
                                                    NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                                                    If Subalterno = "" AndAlso NumeroStringa <> "" Then
                                                        Subalterno = Left(NumeroStringa, 3)
                                                    End If


                                                    TitoloPossesso = Converti_TitoliPossesso_Fascicolo(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).CodiceTipoConduzione, TitoloPossessoDes)
                                                    'DR.Item("possesso") = TitoloPossessoDes

                                                    Inizio_Possesso = #1/1/1900#
                                                    Fine_Possesso = #12/31/2100#
                                                    Inizio_Possesso_Old = #1/1/1900#
                                                    Fine_Possesso_Old = #12/31/2100#

                                                    If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione IsNot Nothing AndAlso ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione <> "" Then
                                                        Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione))
                                                    End If
                                                    If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione IsNot Nothing AndAlso ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "" Then
                                                        Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione))
                                                    End If


                                                    supCatasto = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCatastale) / 10000.0
                                                    Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)


                                                    supConduzione = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCondotta) / 10000.0


                                                    If Sezione = "" Then
                                                        strSezione = "0"
                                                    Else
                                                        strSezione = Sezione
                                                    End If

                                                    If Subalterno = "" OrElse Subalterno = "000" Then
                                                        strSubalterno = "0"
                                                    Else
                                                        strSubalterno = Subalterno
                                                    End If


                                                    '*******************************************************************************************************************
                                                    '******   MACROUSI    **********************************************************************************
                                                    '*******************************************************************************************************************

                                                    Dim Macrouso_Cod As String
                                                    Dim Macrouso_Sup As Double
                                                    Dim Specie_Cod As String
                                                    Dim Varieta_Cod As String
                                                    Dim Specie_Des As String
                                                    Dim Varieta_Des As String
                                                    Dim Utilizzo_Sup As Double
                                                    Dim HashMacrousi As New Hashtable
                                                    strMacrousi = ""
                                                    strUtilizzi = ""

                                                    If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1 IsNot Nothing Then

                                                        For j = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1.Length - 1

                                                            strMacrousi = ""
                                                            strUtilizzi = ""



                                                            Macrouso_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).CodiceMacrouso
                                                            Macrouso_Sup = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).SuperficieUtilizzata) / 10000.0

                                                            'MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                                                            'MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                                            'logga(MsgOK)

                                                            If Not HashMacrousi.ContainsKey(Macrouso_Cod) Then

                                                                HashMacrousi.Add(Macrouso_Cod, "")

                                                                Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                                                                strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).CodiceMacrouso, _
                                                                                                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                                                                                                "", "", _
                                                                                                                                ObjParametri_Server) & " (" & Macrouso_Sup & " Ha)<br/>"



                                                                '*******************************************************************************************************************
                                                                '******   UTILIZZO    **********************************************************************************
                                                                '*******************************************************************************************************************

                                                                If ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra IsNot Nothing Then

                                                                    For x = 0 To ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra.Length - 1

                                                                        strUtilizzi = ""

                                                                        Specie_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).CodiceProdotto
                                                                        Varieta_Cod = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).CodiceVarieta
                                                                        Utilizzo_Sup = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).ISWSUtilizzoTerra(x).SuperficieUtilizzata) / 10000.0

                                                                        Specie_Des = ""
                                                                        Varieta_Des = ""

                                                                        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                                                                        'Dim DtCodifica As DataTable
                                                                        Dim Veg_Cod As Integer = 0
                                                                        Dim Cul_Cod As Integer = 0
                                                                        Dim Grfi_Cod As Integer = 0
                                                                        Dim Id_Cod As Integer = 0
                                                                        Dim Grva_Cod As Integer = 0

                                                                        Dim Uso_Cod_Agea As String
                                                                        Dim Occupazione_Cod_Agea As String
                                                                        Dim Destinazione_Cod_Agea As String
                                                                        Dim Qualita_Cod_Agea As String
                                                                        ' Dim DrVar() As DataRow
                                                                        Dim LogCodificheMancantiSpecie As String = ""
                                                                        Dim LogCodificheMancantiVarieta As String = ""

                                                                        objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
                                                                                          LogCodificheMancantiSpecie,
                                                                                          LogCodificheMancantiVarieta,
                                                                                          Specie_Cod, Varieta_Cod,
                                                                                          Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                                          Specie_Des, Varieta_Des,
                                                                                          "",
                                                                                          "",
                                                                                          CDate(strValiditaInizio),
                                                                                          Uso_Cod_Agea,
                                                                                        Occupazione_Cod_Agea,
                                                                                        Destinazione_Cod_Agea,
                                                                                        Qualita_Cod_Agea,
                                                                                          ObjParametri_Server)



                                                                        If Varieta_Des <> "" Then
                                                                            strUtilizzi = Specie_Des & " - " & Varieta_Des & " (" & Utilizzo_Sup & " Ha)"
                                                                        Else
                                                                            strUtilizzi = Specie_Des & " (" & Utilizzo_Sup & " Ha)"
                                                                        End If


                                                                        DR = DtParticelle.NewRow

                                                                        DR.Item("PROV") = Prov
                                                                        DR.Item("COM") = Com
                                                                        DR.Item("Sezione") = Sezione
                                                                        DR.Item("Foglio") = Foglio
                                                                        DR.Item("Numero") = Numero
                                                                        DR.Item("Subalterno") = Subalterno

                                                                        DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                                                                                             DR.Item("COM") & ":_" & _
                                                                                             DR.Item("Sezione") & ":_" & _
                                                                                             DR.Item("Foglio").ToString & ":_" & _
                                                                                             DR.Item("Numero").ToString & ":_" & _
                                                                                             DR.Item("Subalterno")

                                                                        DR.Item("possesso") = TitoloPossessoDes
                                                                        DR.Item("TitoloPossesso") = TitoloPossesso

                                                                        DR.Item("inizio_possesso") = Inizio_Possesso
                                                                        DR.Item("fine_possesso") = Fine_Possesso

                                                                        DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                                                                                If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                                                        supCatasto = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCatastale) / 10000.0
                                                                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                                                        DR.Item("sup") = Format(supCatasto, "0.0000")

                                                                        supConduzione = CDbl(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).SuperficieCondotta) / 10000.0
                                                                        DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                                                                        DR.Item("macrouso_cod") = Macrouso_Cod
                                                                        DR.Item("macrouso_sup") = Macrouso_Sup
                                                                        DR.Item("macrouso") = strMacrousi

                                                                        DR.Item("utilizzo") = strUtilizzi
                                                                        DR.Item("utilizzo_sup") = Utilizzo_Sup
                                                                        DR.Item("Veg_Cod_Agea") = Specie_Cod
                                                                        DR.Item("Cul_Cod_Agea") = Varieta_Cod
                                                                        DR.Item("Veg_Cod") = Veg_Cod
                                                                        DR.Item("Cul_Cod") = Cul_Cod
                                                                        DR.Item("Grfi_Cod") = Grfi_Cod
                                                                        DR.Item("Grva_Cod") = Grva_Cod
                                                                        DR.Item("Id_Cod") = Id_Cod

                                                                        DR.Item("Scarto") = 0

                                                                        DtParticelle.Rows.Add(DR)
                                                                    Next

                                                                Else

                                                                    DR = DtParticelle.NewRow

                                                                    DR.Item("PROV") = Prov
                                                                    DR.Item("COM") = Com
                                                                    DR.Item("Sezione") = Sezione
                                                                    DR.Item("Foglio") = Foglio
                                                                    DR.Item("Numero") = Numero
                                                                    DR.Item("Subalterno") = Subalterno

                                                                    DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                                                                                         DR.Item("COM") & ":_" & _
                                                                                         DR.Item("Sezione") & ":_" & _
                                                                                         DR.Item("Foglio").ToString & ":_" & _
                                                                                         DR.Item("Numero").ToString & ":_" & _
                                                                                         DR.Item("Subalterno")

                                                                    DR.Item("possesso") = TitoloPossessoDes
                                                                    DR.Item("TitoloPossesso") = TitoloPossesso
                                                                    DR.Item("inizio_possesso") = Inizio_Possesso
                                                                    DR.Item("fine_possesso") = Fine_Possesso

                                                                    DR.Item("datepossesso") = "Dal " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                                                                            IIf(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                                                    DR.Item("sup") = Format(supCatasto, "0.0000")
                                                                    DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                                                                    DR.Item("macrouso_cod") = Macrouso_Cod
                                                                    DR.Item("macrouso_sup") = Macrouso_Sup
                                                                    DR.Item("macrouso") = strMacrousi

                                                                    DR.Item("utilizzo") = ""
                                                                    DR.Item("utilizzo_sup") = 0
                                                                    DR.Item("Veg_Cod_Agea") = ""
                                                                    DR.Item("Cul_Cod_Agea") = ""
                                                                    DR.Item("Veg_Cod") = 0
                                                                    DR.Item("Cul_Cod") = 0
                                                                    DR.Item("Grfi_Cod") = 0
                                                                    DR.Item("Grva_Cod") = 0
                                                                    DR.Item("Id_Cod") = 0

                                                                    DR.Item("Scarto") = 1

                                                                    DtParticelle.Rows.Add(DR)

                                                                End If

                                                            End If

                                                        Next

                                                    Else

                                                        DR = DtParticelle.NewRow

                                                        DR.Item("PROV") = Prov
                                                        DR.Item("COM") = Com
                                                        DR.Item("Sezione") = Sezione
                                                        DR.Item("Foglio") = Foglio
                                                        DR.Item("Numero") = Numero
                                                        DR.Item("Subalterno") = Subalterno

                                                        DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                                                                             DR.Item("COM") & ":_" & _
                                                                             DR.Item("Sezione") & ":_" & _
                                                                             DR.Item("Foglio").ToString & ":_" & _
                                                                             DR.Item("Numero").ToString & ":_" & _
                                                                             DR.Item("Subalterno")

                                                        DR.Item("possesso") = TitoloPossessoDes
                                                        DR.Item("TitoloPossesso") = TitoloPossesso
                                                        DR.Item("inizio_possesso") = Inizio_Possesso
                                                        DR.Item("fine_possesso") = Fine_Possesso

                                                        DR.Item("datepossesso") = "Dal " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                                                                IIf(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                                        DR.Item("sup") = Format(supCatasto, "0.0000")
                                                        DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                                                        DR.Item("macrouso_cod") = ""
                                                        DR.Item("macrouso_sup") = 0
                                                        DR.Item("macrouso") = ""

                                                        DR.Item("utilizzo") = ""
                                                        DR.Item("utilizzo_sup") = 0
                                                        DR.Item("Veg_Cod_Agea") = ""
                                                        DR.Item("Cul_Cod_Agea") = ""
                                                        DR.Item("Veg_Cod") = 0
                                                        DR.Item("Cul_Cod") = 0
                                                        DR.Item("Grfi_Cod") = 0
                                                        DR.Item("Grva_Cod") = 0
                                                        DR.Item("Id_Cod") = 0

                                                        DR.Item("Scarto") = 1

                                                        DtParticelle.Rows.Add(DR)

                                                    End If

                                                Next 'ISWSTerritorio1.Length


                                                Dim NomiChiavi(1) As String
                                                'Chiave della griglia
                                                NomiChiavi(0) = "Scarto"
                                                NomiChiavi(1) = "Utilizzo"


                                            Else

                                                msgCarica &= "<br>Il webservice non ha restituito dati sulle particelle catastali, macrousi e utilizzi."

                                            End If 'ISWSTerritorio1.Length
                                        Else

                                            msgCarica &= "<br>Il webservice non ha restituito dati sulle particelle catastali, macrousi e utilizzi."

                                        End If 'ISWSTerritorio1.Length




                                        '------------------------------------------------------
                                        '--------------- PERSONA ------------
                                        '------------------------------------------------------
                                    Case "P"    'persona (non mi serve)

                                        Persona = True

                                        msgCarica &= "<br>Impossibile scaricare i dati poiché il CUAA inserito appartiene ad una persona!"

                                End Select

                                If Not Presente_Anagrafe Then
                                    msgCarica &= "Impossibile scaricare l'impresa poiché non è presente sull'archivio Anagrafe!"
                                End If

                            Else
                                'se il ws non risponde o ci sono problemi....................

                                If RispostaWSAnagrafe Then

                                    MsgOK = "Anagrafica non trovata!"
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)

                                    msgCarica &= "Anagrafica non trovata!"

                                Else

                                    MsgOK = "Il WebService Anagrafe non ha risposto. RispostaWSAnagrafe = false"
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)

                                    msgCarica &= "Il WebService Anagrafe non ha risposto."

                                End If



                                If ws_AnaResponse Is Nothing Then

                                    MsgOK = "ws_AnaResponse NOTHING!"
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)

                                    msgCarica &= "ws_AnaResponse NOTHING!"
                                Else


                                    If ws_AnaResponse.output Is Nothing Then

                                        MsgOK = "ws_AnaResponse.output NOTHING!"
                                        MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                        logga(MsgOK)

                                        msgCarica &= "ws_AnaResponse.output NOTHING!"
                                    Else


                                        If ws_AnaResponse.output.intestazione Is Nothing Then

                                            MsgOK = "ws_AnaResponse.output.intestazione NOTHING!"
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)
                                            msgCarica &= "ws_AnaResponse.output.intestazione NOTHING!"
                                        Else


                                        End If

                                        If ws_AnaResponse.output.version Is Nothing Then

                                            MsgOK = "ws_AnaResponse.output.versione NOTHING!"
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)
                                            msgCarica &= "ws_AnaResponse.output.versione NOTHING!"
                                        Else


                                        End If

                                        If ws_AnaResponse.output.cf Is Nothing Then

                                            MsgOK = "ws_AnaResponse.output.cf NOTHING!"
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)
                                            msgCarica &= "ws_AnaResponse.output.cf NOTHING!"
                                        Else


                                        End If


                                        If ws_AnaResponse.output.anagrafica Is Nothing Then

                                            MsgOK = "ws_AnaResponse.output.anagrafica NOTHING!"
                                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                            logga(MsgOK)
                                            msgCarica &= "ws_AnaResponse.output.anagrafica NOTHING!"
                                        Else


                                        End If

                                    End If

                                End If


                                Return False

                            End If


                        Catch ex As Exception

                            Dim strEr As String
                            strEr = ex.Message

                            MsgOK = "Anagrafe NON presente!"
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                            logga(MsgOK)

                            msgCarica &= ex.Message
                            Return False

                        End Try


                        'IMPORTO

                        Dim msgImporta As String = ""
                        Dim strXmlPianificazione As String = ""
                        If _4_Importa(PIVA, CUAA, Sa_Cod, DtCondizionalita, DtParticelle, SchedaValidazione, DataValidazione, DataValidazioneFine, msgImporta, strXmlPianificazione) Then

                            'MsgOK = "Importato il Fascicolo"
                            'MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                            'logga(MsgOK)

                            If s = 0 Then
                                AziendeAggiornateSuGias += 1
                            End If

                            'MODIFICO SCHEDA PRECEDENTE SE ERA SOLO SU GIAS E SE NON ERA MOVIMENTATA (CHIUDO SOLO)
                            If SchedaValidazionePrec <> "" AndAlso OrigineSchedaPrec = "G" Then
                                'NON MOVIMENTATO ---> CANCELLO PLANNING + RIBALTATI
                                Dim p As Integer
                                If Dt_SchedeOrd.Rows(s - 1).Item("strProgrammazioniCod_NONMovimentate") <> "" Then
                                    Dim PlanningCodNONMov() As String = Split(Dt_SchedeOrd.Rows(s - 1).Item("strProgrammazioniCod_NONMovimentate"), "|")
                                    If PlanningCodNONMov IsNot Nothing Then
                                        For p = 0 To PlanningCodNONMov.Length - 1
                                            '' MODIFICO DATE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                            _4bis_ModificaDate_Scheda(PlanningCodNONMov(p), CostantiPersonalizzate.AGRODATAINIZIO, DataValidazioneFinePrec, PIVA)
                                        Next
                                    End If
                                End If

                            End If

                            If Ribalta_Automatico AndAlso Ribalta_Fascicolo AndAlso Programmazione_Cod <> 0 AndAlso strXmlPianificazione <> "" Then

                                If _8_Ribalta_Pianificazione(Programmazione_Cod, SchedaValidazione, DataValidazione, DtPV) Then

                                    MsgOK = "Ribaltato piano colturale."
                                    MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                                    logga(MsgOK)

                                End If

                            End If

                        Else

                            MsgOK = "Importazione FALLITA ; Dettagli: (" & msgImporta & ")"
                            MsgOK = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgOK
                            logga(MsgOK)

                            If s = 0 Then
                                AziendeConProblemiAggironamentoSuGias += 1
                            End If

                        End If


                        '----------------------------
                        '----------------------------
                        '----------------------------
                        '----------------------------
                        '----------------------------

                        'SCHEDA PRESENTE SOLO SU GIAS
                    Case "G"

                        '---------------------------------------------------------------------------
                        'VERIFICO se della programmazione sono stati MOVIMENTATI impianti ribaltati 
                        'in tal caso non modifico nulla
                        'in caso non siano stati modificati ELIMINO RIBALTATI
                        '---------------------------------------------------------------------------

                        Dim p As Integer
                        If Dt_SchedeOrd.Rows(s).Item("strProgrammazioniCod_NONMovimentate") <> "" Then
                            Dim PlanningCodNONMov() As String = Split(Dt_SchedeOrd.Rows(s).Item("strProgrammazioniCod_NONMovimentate"), "|")
                            If PlanningCodNONMov IsNot Nothing Then
                                For p = 0 To PlanningCodNONMov.Length - 1
                                    _4_Elimina_Ribaltati(PlanningCodNONMov(p))
                                Next
                            End If
                        End If

                End Select


            Next 'CICLO SCHEDE


        End If

        Return True


    End Function



    Private Function _4_Importa(ByVal PIVA As String, _
                        ByVal CUAA As String, _
                        ByVal Sa_Cod As Integer, _
                        ByVal DtCondizionalita As DataTable, _
                        ByVal DtParticelle As DataTable, _
                        ByVal SchedaValidazione As String, _
                        ByVal DataValidazione As Date, _
                        ByVal DataValidazioneFine As Date, _
                        ByRef Msg As String, _
                        ByRef strXmlPianificazione As String) As Boolean

        Dim strErr As String = ""

        Dim strXmlAnagrafe As String = ""
        ' Dim strXmlPianificazione As String = ""

        Dim MsgLog As String = ""

        'MsgLog = "Inizio Importazione Fascicolo"
        'MsgLog = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgLog
        'logga(MsgLog)

        _5_Crea_Stringa_Catasto(PIVA, Sa_Cod, SchedaValidazione, DataValidazione, DataValidazioneFine, DtParticelle, strXmlAnagrafe, strXmlPianificazione)

        If PIVA = "" Then
            Msg = "Inserire PIVA dell'Impresa che si desidera importare!"
            Return False
        End If

        If strXmlAnagrafe <> "" Then

            'importo i dati anagrafici in base ai check scelti
            If Not Importa_PianoColturale Then
                strXmlPianificazione = ""
            End If

            If Not Importa_Anagrafica Then
                strXmlAnagrafe = ""
            End If


            MsgLog = "Avvio Importazione Fascicolo"
            MsgLog = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & MsgLog
            logga(MsgLog)

            Dim Risposta As String = ""

            If _6_Importa_Dati(strXmlAnagrafe, strXmlPianificazione, Risposta, strErr) Then

                If strXmlPianificazione = "" Then
                    Msg = "Importata Anagrafe."
                Else
                    Msg = "Importata Anagrafe (e piano colturale)."
                End If

                'importo il profilo della condizionalità
                If Importa_Condizionalita Then

                    If DtCondizionalita.Rows.Count > 0 Then

                        If _7_Importa_Condizionalita_Profilo(PIVA, DtCondizionalita) Then
                            Msg &= " Importata condizionalità."
                        Else
                            Msg &= " NON Importata condizionalità: si è verificato un ERRORE durante il salvataggio del profilo di condizionalità."
                        End If

                    Else
                        Msg &= " NON Importata condizionalità: il profilo condizionalità è vuoto."
                    End If

                End If

                'scrivo  nella tabella SIGPA_CatastoImportato_FascicoliProcessati
                objSchedeProc_W.Scrivi(CUAA, SchedaValidazione, DataValidazione, ObjParametri_Server)

                MsgLog = " PIVA: " & PIVA & ", CUAA:" & CUAA & "  -  " & Msg
                logga(MsgLog)

            Else

                Msg = "Errore in Importa_Dati: " & strErr
                Return False

            End If

        Else

            Msg = "strXmlAnagrafe è vuoto"
            Return False

        End If

        Return True

    End Function



    Private Sub _5_Crea_Stringa_Catasto(ByVal PIVA As String, _
                                    ByVal SaCod As Integer, _
                                    ByVal SchedaValidazione As String, _
                                    ByVal DataValidazione As Date, _
                                    ByVal DataValidazioneFine As Date, _
                                    ByVal DtParticelle As DataTable, _
                                    ByRef stringaAnagrafica As String, _
                                    ByRef stringaPianificazione As String)

        Dim i As Integer


        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String
        Dim supConduzione As Double
        Dim SupCatastale As Double
        Dim Ettari, Are, Centiare As Integer
        Dim Macrouso_Cod As String
        Dim Macrouso_Sup As Double
        Dim Utilizzo_Sup As Double
        Dim Veg_Cod_Agea, Cul_Cod_Agea As String
        Dim Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod As Integer

        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim TitoloPossesso, TitoloPossesso_Old As Integer
        Dim Inizio_Possesso, Inizio_Possesso_Old As Date
        Dim Fine_Possesso, Fine_Possesso_Old As Date
        Dim Inizio_Macrouso, Inizio_Macrouso_Old As Date
        Dim Fine_Macrouso, Fine_Macrouso_Old As Date
        Dim Inizio_Utilizzo, Inizio_Utilizzo_Old As Date
        Dim Fine_Utilizzo, Fine_Utilizzo_Old As Date

        Dim Superficie_Old As Double

        Dim TipoOperazione_Particella, TipoOperazione_Possesso, TipoOpMacrousoxParticella, TipoOpUtilizzoxParticella As enum_TipoOperazioneDB

        Dim XmlParticella As System.Xml.XmlElement
        Dim XmlPossesso As System.Xml.XmlElement
        Dim XmlZona As System.Xml.XmlElement
        Dim XmlMacrouso As System.Xml.XmlElement
        Dim XmlUtilizzo As System.Xml.XmlElement
        Dim XmlCentro As System.Xml.XmlElement
        Dim XMLCentri As System.Xml.XmlNodeList
        Dim XmlEntita As System.Xml.XmlElement
        Dim XmlParticellaP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XMLTestate As System.Xml.XmlNodeList

        Dim nEntita As Integer = 1

        Dim DtParticelleCentro As New DataTable
        Dim DtParticelleMacrousi As New DataTable
        Dim DtParticelleUtilizzi As New DataTable
        Dim DtParticelleZone As New DataTable
        Dim DtParticelleZonaBSL As New DataTable
        Dim DrParticella() As DataRow
        Dim DrParticellaM() As DataRow
        Dim DrParticellaU() As DataRow
        Dim DrParticellaZ() As DataRow

        Dim strXmlDoc As String
        Dim strXmlDocP As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDocP As New System.Xml.XmlDocument

        If STR_XmlDoc <> "" Then
            strXmlDoc = STR_XmlDoc
            XmlDoc.LoadXml(strXmlDoc)
        End If
        If STR_XmlDocP <> "" Then
            strXmlDocP = STR_XmlDocP
            XmlDocP.LoadXml(strXmlDocP)
        End If


        Dim StrDataInizioTestataPlanning As String = String.Empty
        Dim StrDataFineTestataPlanning As String = String.Empty


        Dim objXML As New AgronicaCoreXML.AnagrafeXML
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objParticellexMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objParticellexMacrousixUtilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objParticellexZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R

        If SaCod <> 0 Then

            DtParticelleCentro = objImpresexParticelle.Leggi(0, _
                                                           PIVA, _
                                                           SaCod, _
                                                           0, _
                                                           "", "", "", 0, 0, "", _
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                           "", "", _
                                                           ObjParametri_Server)

            DtParticelleMacrousi = objParticellexMacrousi.Leggi_DaCentro(PIVA, _
                                                           SaCod, _
                                                           "", "", "", 0, 0, "", _
                                                           "", _
                                                           "", "", _
                                                           ObjParametri_Server)

            DtParticelleUtilizzi = objParticellexMacrousixUtilizzo.Leggi_DaCentro(PIVA, _
                                               SaCod, _
                                               "", "", "", 0, 0, "", _
                                               "", "", "", _
                                               "", "", _
                                               ObjParametri_Server)

            DtParticelleZone = objParticellexZone.Leggi(-17, _
                                    "", "", "", 0, 0, "", _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                   "", "", _
                                   ObjParametri_Server)

            DtParticelleZonaBSL = objParticellexZone.Leggi(1, _
                                  "", "", "", 0, 0, "", _
                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                 "", "", _
                                 ObjParametri_Server)

        End If



        Dim HashParticelle As New Hashtable

        'Leggo le particelle vulnerabili
        Dim objPV As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R

        For i = 0 To DtParticelle.Rows.Count - 1

            TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            TipoOperazione_Possesso = enum_TipoOperazioneDB.Scrittura

            Dim ModificaParticella As Boolean = False
            Dim ModificaPossesso As Boolean = False
            Dim ModificaMacrouso As Boolean = False
            Dim ModificaUtilizzo As Boolean = False
            Dim InserisciZona As Boolean = False


            Prov = DtParticelle.Rows(i).Item("PROV")
            Com = DtParticelle.Rows(i).Item("COM")
            Sezione = DtParticelle.Rows(i).Item("Sezione")
            Foglio = DtParticelle.Rows(i).Item("Foglio")
            Numero = DtParticelle.Rows(i).Item("Numero")
            Subalterno = DtParticelle.Rows(i).Item("Subalterno")

            If Sezione = "" Then
                Sezione = "0"
            End If
            If Subalterno = "" Then
                Subalterno = "0"
            End If

            SupCatastale = DtParticelle.Rows(i).Item("sup")
            Conversioni.EttariAreCentiare_from_Ettari(SupCatastale, Ettari, Are, Centiare)

            supConduzione = DtParticelle.Rows(i).Item("supcondotta")

            TitoloPossesso = DtParticelle.Rows(i).Item("TitoloPossesso")
            Inizio_Possesso = DtParticelle.Rows(i).Item("Inizio_Possesso")
            Fine_Possesso = DtParticelle.Rows(i).Item("Fine_Possesso")

            Macrouso_Cod = DtParticelle.Rows(i).Item("Macrouso_Cod")
            Macrouso_Sup = DtParticelle.Rows(i).Item("Macrouso_Sup")


            ' Nicoletta 30/09/2013 si è deciso di usare l'anno solare (io con Fabrizio!!!!!)
            ' Fede 08/07/2014 si è deciso di utilizzare datavalidazione  e datavalidazionefine (= datavalidazione scheda precedente -1 )
            'Inizio_Macrouso = "01/01/" & AnnoValidazione.ToString
            'Fine_Macrouso = "31/12/" & AnnoValidazione.ToString
            'Inizio_Utilizzo = "01/01/" & AnnoValidazione.ToString
            'Fine_Utilizzo = "31/12/" & AnnoValidazione.ToString
            Inizio_Macrouso = DataValidazione
            Fine_Macrouso = DataValidazioneFine
            Inizio_Utilizzo = DataValidazione
            Fine_Utilizzo = DataValidazioneFine


            If i = 0 Then
                'al primo giro mi salvo la data inizio e fine del primo appezzamento
                'in modo da utilizzarle per la testata del planning
                StrDataInizioTestataPlanning = Inizio_Utilizzo.ToShortDateString
                StrDataFineTestataPlanning = Fine_Utilizzo.ToShortDateString
            End If

            If Inizio_Macrouso < Inizio_Possesso Then
                Inizio_Macrouso = Inizio_Possesso
            End If
            If Fine_Macrouso > Fine_Possesso Then
                Fine_Macrouso = Fine_Possesso
            End If

            If Inizio_Utilizzo < Inizio_Possesso Then
                Inizio_Utilizzo = Inizio_Possesso
            End If
            If Fine_Utilizzo > Fine_Possesso Then
                Fine_Utilizzo = Fine_Possesso
            End If


            Veg_Cod_Agea = DtParticelle.Rows(i).Item("Veg_Cod_Agea")
            Cul_Cod_Agea = DtParticelle.Rows(i).Item("Cul_Cod_Agea")
            Veg_Cod = DtParticelle.Rows(i).Item("Veg_Cod")
            Cul_Cod = DtParticelle.Rows(i).Item("Cul_Cod")
            Grfi_Cod = DtParticelle.Rows(i).Item("Grfi_Cod")
            Grva_Cod = DtParticelle.Rows(i).Item("Grva_Cod")
            Id_Cod = DtParticelle.Rows(i).Item("Id_Cod")
            Utilizzo_Sup = DtParticelle.Rows(i).Item("Utilizzo_Sup")

            Uso_Cod_Agea = ""
            Occupazione_Cod_Agea = ""
            Destinazione_Cod_Agea = ""
            Qualita_Cod_Agea = ""

            If Veg_Cod_Agea = "" Then
                'Veg_Cod_Agea = CType(GridView_Particelle.Rows(i).FindControl("CmbUtilizzo"), WebControls.DropDownList).SelectedItem.Value


                If Veg_Cod_Agea <> "" Then
                    Cul_Cod_Agea = "000"
                    Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                    Dim LogCodificheMancantiSpecie As String = ""
                    Dim LogCodificheMancantiVarieta As String = ""

                    objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
                                                            LogCodificheMancantiSpecie,
                                                            LogCodificheMancantiVarieta,
                                                            Veg_Cod_Agea, Cul_Cod_Agea,
                                                            Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                            "", "",
                                                            "",
                                                            "",
                                                            CDate(DataValidazione),
                                                            Uso_Cod_Agea,
                                                            Occupazione_Cod_Agea,
                                                            Destinazione_Cod_Agea,
                                                            Qualita_Cod_Agea,
                                                            ObjParametri_Server)

                    Utilizzo_Sup = Macrouso_Sup
                End If

            End If



            'se esiste già il centro verifico se contiene la particella
            If SaCod <> 0 Then
                DrParticella = DtParticelleCentro.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                'se la particella in quell'intervallo esiste già la modifico
                'altrimenti la inserisco
                If DrParticella IsNot Nothing AndAlso DrParticella.Length > 0 Then
                    ModificaParticella = True
                    For j = 0 To DrParticella.Length - 1
                        Inizio_Possesso_Old = DrParticella(j).Item("validita_inizio")
                        Fine_Possesso_Old = DrParticella(j).Item("validita_fine")
                        TitoloPossesso_Old = DrParticella(j).Item("TitoloPossesso")
                        If Inizio_Possesso < Fine_Possesso_Old AndAlso Fine_Possesso > Inizio_Possesso_Old Then
                            If Modifica_TitoloPossesso Then
                                TitoloPossesso = TitoloPossesso_Old
                            End If
                            ModificaPossesso = True
                            Exit For
                        End If
                    Next
                End If
                If ModificaParticella Then
                    TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
                End If
                If ModificaPossesso Then
                    TipoOperazione_Possesso = enum_TipoOperazioneDB.Modifica
                End If
            End If



            ' nuova implementazione
            ' non precarico tutto perché dopo l'aggiunta del veneto i record sono più di 150.000
            Dim strZona As String = "n"
            Dim BSL As Integer = 0
            If objPV.Vulnerabile(Prov, Com, Sezione, Foglio, Numero, Subalterno, 0, BSL, "", ObjParametri_Server) Then
                strZona = "v"
            End If

            '--------------------------------------------------------
            If Not HashParticelle.ContainsKey(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno) Then

                HashParticelle.Add(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno, "")

                XmlParticella = objXML.Xml_Pubblico_Particella(TipoOperazione_Particella, _
                                                          "0", _
                                                          Com, _
                                                          Prov, _
                                                          Sezione, _
                                                          Foglio, _
                                                          Numero, _
                                                          Subalterno, _
                                                          "#", _
                                                          Ettari, _
                                                          Are, _
                                                          Centiare, _
                                                          TitoloPossesso, _
                                                          "#", "#", "#", "#", _
                                                          Format(supConduzione, "0.0000"), _
                                                          Inizio_Possesso, _
                                                          Fine_Possesso, _
                                                          XmlDoc)

                XmlPossesso = objXML.Xml_Pubblico_Particella_Possesso(TipoOperazione_Possesso, _
                                                                      "#", _
                                                                      TitoloPossesso, _
                                                                       Format(supConduzione, "0.0000"), _
                                                                       Inizio_Possesso, _
                                                                       Fine_Possesso, _
                                                                       XmlDoc)
                XmlParticella.AppendChild(XmlPossesso)

            End If

            'se la particella è vulnerabile...........
            'verifico se è già legata alla 'zona vulnerabile ai nitrati'
            If strZona = "v" Then

                InserisciZona = True

                If DtParticelleZone IsNot Nothing AndAlso DtParticelleZone.Rows.Count > 0 Then
                    DrParticellaZ = DtParticelleZone.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                    'se la zona vulnerabile esiste già non faccio nulla
                    'altrimenti la inserisco
                    If DrParticellaZ IsNot Nothing AndAlso DrParticellaZ.Length > 0 Then
                        InserisciZona = False
                    End If
                End If

                If InserisciZona Then
                    XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura, _
                               "-17", _
                               "0", _
                               XmlDoc)

                    XmlParticella.AppendChild(XmlZona)
                End If

            End If

            If BSL = 1 Then

                InserisciZona = True

                If DtParticelleZone IsNot Nothing AndAlso DtParticelleZone.Rows.Count > 0 Then
                    DrParticellaZ = DtParticelleZonaBSL.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                    'se la zona BSL esiste già non faccio nulla
                    'altrimenti la inserisco
                    If DrParticellaZ IsNot Nothing AndAlso DrParticellaZ.Length > 0 Then
                        InserisciZona = False
                    End If
                End If

                If InserisciZona Then
                    XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura, _
                               "1", _
                               "0", _
                               XmlDoc)

                    XmlParticella.AppendChild(XmlZona)
                End If

            End If

            Dim ModificaVecchioMacrouso As Boolean = False

            'se esiste il macrouso...........
            If Macrouso_Cod <> "" Then
                TipoOpMacrousoxParticella = enum_TipoOperazioneDB.Scrittura
                ModificaMacrouso = False
                'If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                If SaCod <> 0 Then
                    DrParticellaM = DtParticelleMacrousi.Select("PIVA='" & PIVA & "' AND PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'" & " AND Superficie = " & Replace(Format(Macrouso_Sup, "0.###########"), ",", "."))
                    'se il macrouso in quell'intervallo esiste già lo modifico
                    'altrimenti lo inserisco

                    ' se non ho trovato lo stesso macrouso e la stessa superficie cerco il macrouso
                    If DrParticellaM IsNot Nothing OrElse DrParticellaM.Length = 0 Then

                        DrParticellaM = DtParticelleMacrousi.Select("PIVA='" & PIVA & "' AND PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'")

                    End If


                    If DrParticellaM IsNot Nothing AndAlso DrParticellaM.Length > 0 Then

                        For m = 0 To DrParticellaM.Length - 1
                            Inizio_Macrouso_Old = DrParticellaM(m).Item("validita_inizio")
                            Fine_Macrouso_Old = DrParticellaM(m).Item("validita_fine")
                            Superficie_Old = DrParticellaM(m).Item("superficie")

                            ' Nico 27/09/2013 cambiato l'algoritmo
                            'If Inizio_Macrouso < Fine_Macrouso_Old And Fine_Macrouso > Inizio_Macrouso_Old Then
                            '    ModificaMacrouso = True
                            '    Exit For
                            'End If
                            ' sa la superficie del macrouso è invariata cambio solo le date
                            If Macrouso_Sup = Superficie_Old Then
                                If Inizio_Macrouso = DateAdd(DateInterval.Day, 1, Fine_Macrouso_Old) Then
                                    Inizio_Macrouso = Inizio_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Fine_Macrouso = DateAdd(DateInterval.Day, -1, Inizio_Macrouso_Old) Then
                                    Fine_Macrouso = Fine_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Inizio_Macrouso = Inizio_Macrouso_Old AndAlso Fine_Macrouso = Fine_Macrouso_Old Then
                                    ModificaMacrouso = True
                                ElseIf Inizio_Macrouso >= Inizio_Macrouso_Old AndAlso Inizio_Macrouso <= Fine_Macrouso_Old AndAlso _
                                       Fine_Macrouso >= Fine_Macrouso_Old Then
                                    Inizio_Macrouso = Inizio_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Fine_Macrouso <= Fine_Macrouso_Old AndAlso Fine_Macrouso >= Inizio_Macrouso_Old AndAlso _
                                       Inizio_Macrouso <= Inizio_Macrouso_Old Then
                                    Fine_Macrouso = Fine_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Inizio_Macrouso >= Inizio_Macrouso_Old AndAlso Fine_Macrouso <= Fine_Macrouso_Old Then
                                    Inizio_Macrouso = Inizio_Macrouso_Old
                                    Fine_Macrouso = Fine_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Inizio_Macrouso >= Inizio_Macrouso_Old AndAlso Fine_Macrouso <= Fine_Macrouso_Old Then
                                    Inizio_Macrouso = Inizio_Macrouso_Old
                                    Fine_Macrouso = Fine_Macrouso_Old
                                    ModificaMacrouso = True
                                ElseIf Inizio_Macrouso <= Inizio_Macrouso_Old AndAlso Fine_Macrouso >= Fine_Macrouso_Old Then
                                    ModificaMacrouso = True
                                End If
                            Else
                                ' se la superficie è cambiata aggiungo il macrouso
                                ' se l'inizio della nuova superficie è minore della fine del vecchio, 
                                ' modifico la data di fine di quella esistente e inserisco la nuova
                                ModificaMacrouso = False
                                If Inizio_Macrouso < Fine_Macrouso_Old Then
                                    ModificaVecchioMacrouso = True
                                End If
                            End If

                        Next

                    End If
                    If ModificaMacrouso Then
                        TipoOpMacrousoxParticella = enum_TipoOperazioneDB.Modifica
                    End If
                End If

                If ModificaVecchioMacrouso Then

                    XmlMacrouso = objXML.Xml_Pubblico_Macrouso(enum_TipoOperazioneDB.Modifica, _
                                                          PIVA, _
                                                          Macrouso_Cod.ToString, _
                                                          Format(Superficie_Old, "0.0000"), _
                                                          Inizio_Macrouso_Old, DateAdd(DateInterval.Day, -1, Inizio_Macrouso), XmlDoc)

                End If

                XmlMacrouso = objXML.Xml_Pubblico_Macrouso(TipoOpMacrousoxParticella, _
                                                               PIVA, _
                                                               Macrouso_Cod.ToString, _
                                                               Format(Macrouso_Sup, "0.0000"), _
                                                               Inizio_Macrouso, Fine_Macrouso, XmlDoc)

                XmlParticella.AppendChild(XmlMacrouso)

                Dim Id_Utilizzo As String = ""

                'se esiste l'utilizzo...........
                If Veg_Cod_Agea <> "" Then
                    TipoOpUtilizzoxParticella = enum_TipoOperazioneDB.Scrittura
                    ModificaUtilizzo = False
                    'If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                    If SaCod <> 0 Then
                        ' possono esserci più utilizzi identici ma con superfici diverse. Devo stare attenta a non aggiornarli tutti
                        DrParticellaU = DtParticelleUtilizzi.Select("PIVA='" & PIVA & "' AND PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'" & " AND Veg_Cod_Agea='" & Veg_Cod_Agea & "'" & " AND Cul_Cod_Agea='" & Cul_Cod_Agea & "'" & " AND Superficie = " & Replace(Format(Utilizzo_Sup, "0.###########"), ",", "."))
                        'se il macrouso in quell'intervallo esiste già lo modifico
                        'altrimenti lo inserisco
                        If DrParticellaU IsNot Nothing AndAlso DrParticellaU.Length > 0 Then
                            For m = 0 To DrParticellaU.Length - 1
                                Inizio_Utilizzo_Old = DrParticellaU(m).Item("validita_inizio")
                                Fine_Utilizzo_Old = DrParticellaU(m).Item("validita_fine")
                                If Inizio_Utilizzo < Fine_Utilizzo_Old AndAlso Fine_Utilizzo > Inizio_Utilizzo_Old Then
                                    ModificaUtilizzo = True
                                    Id_Utilizzo = DrParticellaU(m).Item("ID")
                                    Exit For
                                End If
                            Next
                        End If
                        If ModificaUtilizzo Then
                            TipoOpUtilizzoxParticella = enum_TipoOperazioneDB.Modifica
                        End If
                    End If

                    XmlUtilizzo = objXML.Xml_Pubblico_Utilizzo(TipoOpUtilizzoxParticella, _
                                                                PIVA, _
                                                                Veg_Cod_Agea, _
                                                                Cul_Cod_Agea, _
                                                                Utilizzo_Sup, _
                                                                Inizio_Utilizzo, Fine_Utilizzo, Id_Utilizzo, XmlDoc)

                    XmlMacrouso.AppendChild(XmlUtilizzo)

                    'RIGA PIANIFICAZIONE

                    XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(enum_TipoOperazioneDB.Scrittura,
                                    "0",
                                    Right("000" & nEntita, 3) & " Val. N." & SchedaValidazione,
                                    SaCod,
                                    "#",
                                    -nEntita,
                                    "0", "0",
                                    "Lotto Val. N." & SchedaValidazione,
                                    Veg_Cod,
                                    Grfi_Cod,
                                    Cul_Cod,
                                    Grva_Cod,
                                    Veg_Cod_Agea, Cul_Cod_Agea, "#",
                                    Macrouso_Cod,
                                    Id_Cod, "#",
                                    Utilizzo_Sup.ToString,
                                    "#",
                                    strZona,
                                    "#", "#", "#", "#", "#", "#",
                                    "102",
                                    "#", "#", "#",
                                    Inizio_Utilizzo,
                                    Fine_Utilizzo,
                                    Inizio_Utilizzo,
                                    XmlDocP,
                                    Veg_Cod_Agea:=Veg_Cod_Agea,
                                    Cul_Cod_Agea:=Cul_Cod_Agea,
                                    Uso_Cod_Agea:=Uso_Cod_Agea,
                                    Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                    Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                    Qualita_Cod_Agea:=Qualita_Cod_Agea)




                    XmlParticellaP = objXML.Xml_Pubblico_ProgrammazioneParticella(enum_TipoOperazioneDB.Scrittura, _
                                                                                    Com, _
                                                                                    Prov, _
                                                                                    Sezione, _
                                                                                    Foglio, _
                                                                                    Numero, _
                                                                                    Subalterno, _
                                                                                    Utilizzo_Sup.ToString, _
                                                                                    "#", "#", _
                                                                                    XmlDocP)

                    XmlEntita.AppendChild(XmlParticellaP)

                    nEntita += 1

                    XMLTestate = XmlDocP.GetElementsByTagName("Pianificazione")
                    XmlTestata = XMLTestate(0)
                    If StrDataInizioTestataPlanning <> "" Then
                        XmlTestata.SetAttribute("validita_inizio", StrDataInizioTestataPlanning)
                    End If
                    If StrDataFineTestataPlanning <> "" Then
                        XmlTestata.SetAttribute("validita_fine", StrDataFineTestataPlanning)
                    End If

                    XmlTestata.AppendChild(XmlEntita)

                End If

            End If

            XMLCentri = XmlDoc.GetElementsByTagName("CentroAziendale")
            XmlCentro = XMLCentri(0)

            XmlCentro.AppendChild(XmlParticella)



        Next

        stringaAnagrafica = XmlDoc.OuterXml

        If InStr(XmlDocP.OuterXml, "Entita") <> 0 Then
            'if XmlTestata.HasChildNodes = True Then
            stringaPianificazione = XmlDocP.OuterXml
        Else
            stringaPianificazione = ""
        End If

    End Sub



    Private Function _6_Importa_Dati(ByVal strDatiAnagrafe As String, _
                             ByRef strDatiPianificazione As String, _
                             ByRef strErr As String, _
                             ByRef Descrizione As String, _
                             Optional ByVal TipoOperazione As Integer = 1) As Boolean

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String

        Descrizione = ""
        strErr = ""


        Dim x As Integer

        Dim Importa As New Ws_Importa_Gias.ImportaWS



        Try


            If LinkWSImportaGIAS <> "" Then

                Importa.Url = "http://localhost" & LinkWSImportaGIAS

                'Importa.Timeout = Integer.MaxValue
                Importa.Timeout = System.Threading.Timeout.Infinite
                Descrizione = String.Empty

                Dim objcoreXML As New AgronicaCoreXML.XML_WS_Importa_Gias
                Dim strCredenziali As String
                strCredenziali = objcoreXML.Genera_Stringa_Credenziali(True, Nothing, _
                                                                       SuperUserUsername, _
                                                                       SuperUserPassword, _
                                                                       SuperUserPiva, _
                                                                       True, _
                                                                       "", "", "", "", "", "", _
                                                                       ObjParametri_Server.StringaConnessione, _
                                                                       ObjParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(strCredenziali, _
                                                                                      strDatiAnagrafe, _
                                                                                      Codice_Chiave_Cliente)


                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then

                        Descrizione &= "- " & XML_Risultato.GetAttribute("errore").ToString & "</br>"
                        strErr = Descrizione

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

                    'StrFinalePianificazione = Importa.Importa_Pianificazione(SuperUserUsername, _
                    '                                       SuperUserPassword, _
                    '                                       SuperUserPiva, _
                    '                                       strDatiPianificazione, _
                    '                                       Codice_Chiave_Cliente)

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(strCredenziali, _
                                                                              strDatiPianificazione, _
                                                                              Codice_Chiave_Cliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                    Next

                End If

                ' Me.Lbl_RisImportazione.Text = Descrizione

                If strErr = "" Then
                    Return True
                Else
                    Return False
                End If

            Else

                'Lbl_Errore_Ricerca.Text = "Inserire l'indirizzo del Web Service Gias per continuare."
                strErr = "Inserire l'indirizzo del Web Service Gias per continuare."
                Return False

            End If

        Catch ex As Exception

            strErr = ex.Message
            Return False

        End Try

        Return False

    End Function



    Private Function _7_Importa_Condizionalita_Profilo(ByVal Piva As String, ByVal Dt As DataTable) As Boolean


        Dim Dr() As DataRow
        Dim Codice As Integer
        Dim ValiditaInizio As DateTime = Date.Today
        Dim ValiditaFine As DateTime = #12/31/2100#

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider


        Try

            If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                        FlagTransazioneLocale, _
                                                                                        ObjParametri_Server)


                '*************************************************************************************************************
                '***  RECORD DOMANDE INTERVISTA         *********************************************************************
                '*************************************************************************************************************
                Dim Dt_Domande_Interviste As DataTable
                Dim strValore As String

                Dim objAudit_Domande_Interviste As New AgronicaCoreAuditDAL.Audit_Domande_Interviste_R

                ' leggo le domande dell'intervista
                Dt_Domande_Interviste = objAudit_Domande_Interviste.Leggi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                                                            RegolamentoCod, _
                                                                            0, _
                                                                            "", "", _
                                                                            ObjParametri_Server)

                If Dt_Domande_Interviste.Rows.Count > 0 Then

                    '*************************************************************************************************************
                    '***  RECORD INTERVISTA         ******************************************************************************
                    '*************************************************************************************************************

                    Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                    Dim TopCode As Integer
                    Dim BaseCode As Integer

                    AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, _
                                                                                       TopCode, _
                                                                                       ProgressivoGIAS)
                    Codice = ObjSequenze.NuovoId_Tabella( _
                                           "AUDIT_INTERVISTE", _
                                            BaseCode, _
                                            TopCode, _
                                            ObjParametri_Server)

                    Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_W

                    objAudit_Interviste.Scrivi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                            RegolamentoCod, _
                                            Codice, _
                                            Piva, _
                                            ValiditaInizio, _
                                            ValiditaFine, _
                                            ObjParametri_Server)



                    For i = 0 To Dt_Domande_Interviste.Rows.Count - 1

                        strValore = ""

                        Select Case Dt_Domande_Interviste.Rows(i).Item("Tipo")

                            Case "b"

                                Dr = Dt.Select("Domanda_Cod=" & CStr(Dt_Domande_Interviste.Rows(i).Item("Domanda_Cod")))

                                If Dr IsNot Nothing AndAlso Dr.Length > 0 Then
                                    strValore = "1"
                                Else
                                    strValore = "0"
                                End If

                        End Select

                        '*************************************************************************************************************
                        '***  RECORD RISPOSTE INTERVISTA         *********************************************************************
                        '*************************************************************************************************************

                        If strValore <> "" Then

                            Dim objAuditRisposteInterviste As New AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W

                            ' INSERIMENTO
                            objAuditRisposteInterviste.Scrivi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                                                RegolamentoCod, _
                                                                Codice, _
                                                                CLng(Dt_Domande_Interviste.Rows(i).Item("Domanda_Cod")), _
                                                                strValore, _
                                                                ValiditaInizio, _
                                                                ValiditaFine, _
                                                                ObjParametri_Server)

                        End If

                    Next



                End If


            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, ObjParametri_Server)

            Return True

        Catch ex As Exception

            'Faccio il rollback della transazione
            If ObjParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, ObjParametri_Server)
            End If



            Return False

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, ObjParametri_Server)

            ' distruggo gli oggetti
            objDataProvider = Nothing

        End Try


    End Function


    Private Function _4bis_ModificaDate_Scheda(ByVal Programmazione_Cod As Integer, _
                                                ByVal DataValidazione As Date, _
                                                ByVal DataValidazioneFine As Date, _
                                                ByVal Piva As String) As Boolean


        'recupero il record della pianificazione
        Dim BoolDummy As Boolean

        Dim Programmazione_W As New AgronicaCoreAnagrafeBIZ.Programmazione_W
        Dim ObjReg_Impianti_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
        Dim ObjImprese_Progetti_W As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
        Dim ObjAppezzamento_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim ObjAppezzaxPart_W As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W


        '--------------------------------------------------
        'MODIFICA PLANNING (testata + entita + particelle)
        '--------------------------------------------------
        If DataValidazione = CostantiPersonalizzate.AGRODATAINIZIO Then
            BoolDummy = Programmazione_W.Pianificazione_Modifica_Validita_Fine(Programmazione_Cod, DataValidazioneFine, ObjParametri_Server)
        Else
            BoolDummy = Programmazione_W.Pianificazione_Modifica_Validita(Programmazione_Cod, DataValidazione, DataValidazioneFine, ObjParametri_Server)
        End If
        '--------------------------------------------------
        'MODIFICA Appezzamenti + appezzamentixparticelle + impianti + esercizi 
        '--------------------------------------------------

        Dim DtEntRib As New DataTable
        DtEntRib = Reg_Impianti_Programmazioni_R.Leggi("", 0, 0, 0, 0, Programmazione_Cod, 0, "", "", ObjParametri_Server)

        For a = 0 To DtEntRib.Rows.Count - 1

            'Chiusura dell'appezzamento
            '   1.  Chiusura dell'Appezzamento
            '   2.  Chiusura delle intersezioni in AppezzamentixParticelle
            '   3.  Chiusura degli eventuali Impianti Colturali
            '   4.  Chiusura delle eventuali Distinte

            'Chiusura dell'Appezzamento
            ObjAppezzamento_W.AggiornaValiditaFineForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                                CInt(0), _
                                                CInt(DtEntRib.Rows(a).Item("appezza")), _
                                                CDate(DataValidazioneFine), _
                                                "", ObjParametri_Server)

            'Chiusura delle intersezioni in AppezzamentixParticelle
            ObjAppezzaxPart_W.AggiornaValiditaFineForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                   CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                                   CInt(0), _
                                                   CInt(DtEntRib.Rows(a).Item("appezza")), _
                                                   "", "", "", 0, 0, "", _
                                                   CDate(DataValidazioneFine), _
                                                   "", ObjParametri_Server)

            'Chiusura dell'impianto
            ObjReg_Impianti_W.AggiornaValiditaFineForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                   CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                                CInt(DtEntRib.Rows(a).Item("appezza")), _
                                                CInt(0), _
                                                CInt(DtEntRib.Rows(a).Item("id_reg")), _
                                                CDate(DataValidazioneFine), _
                                                "", ObjParametri_Server)

            'Chiusura della distinta
            ObjImprese_Progetti_W.Modifica_Validita_Fine(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                         CInt(DtEntRib.Rows(a).Item("Progetto_Cod")), _
                                                         CDate(DataValidazioneFine), _
                                                         "", _
                                                         ObjParametri_Server)

            If DataValidazione <> CostantiPersonalizzate.AGRODATAINIZIO Then

                ObjAppezzamento_W.AggiornaValiditaInizioForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                      CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                      CInt(0), _
                                      CInt(DtEntRib.Rows(a).Item("appezza")), _
                                      CDate(DataValidazione), _
                                      "", ObjParametri_Server)

                ObjAppezzaxPart_W.AggiornaValiditaInizioForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                       CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                                       CInt(0), _
                                                       CInt(DtEntRib.Rows(a).Item("appezza")), _
                                                       "", "", "", 0, 0, "", _
                                                       CDate(DataValidazione), _
                                                       "", ObjParametri_Server)

                ObjReg_Impianti_W.AggiornaValiditaInizioForzata(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                   CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                                CInt(DtEntRib.Rows(a).Item("appezza")), _
                                                CInt(0), _
                                                CInt(DtEntRib.Rows(a).Item("id_reg")), _
                                                CDate(DataValidazione), _
                                                ObjParametri_Server)

                ObjImprese_Progetti_W.Modifica_Validita_Inizio(CStr(DtEntRib.Rows(a).Item("piva")), _
                                                       CInt(DtEntRib.Rows(a).Item("Progetto_Cod")), _
                                                       CDate(DataValidazione), _
                                                       "", _
                                                       ObjParametri_Server)
            End If

        Next

        Return BoolDummy

    End Function

    Private Sub _4_Elimina_Ribaltati(ByVal Programmazione_Cod As Integer)

        Dim DtEntRib As New DataTable

        DtEntRib = Reg_Impianti_Programmazioni_R.Leggi("", 0, 0, 0, 0, Programmazione_Cod, 0, "", "", ObjParametri_Server)

        For a = 0 To DtEntRib.Rows.Count - 1

            'controllo che non ci siano registrazioni di agenda riferite all'impianto
            Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DtAgenda As DataTable

            DtAgenda = ObjAgenda.Leggi(CStr(DtEntRib.Rows(a).Item("piva")), _
                                     CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                     0, _
                                     0, _
                                     0, _
                                     CInt(DtEntRib.Rows(a).Item("appezza")), _
                                     0, _
                                     0, _
                                     AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                     "", _
                                     "", _
                                     ObjParametri_Server)


            If Not IsNothing(DtAgenda) AndAlso DtAgenda.Rows.Count <> 0 Then
                'Throw New Exception("Impossibile eliminare l'appezzamento poiché esistono delle registrazioni ad esso associate!" & Chr(13) & _
                '            " Per poter procedere con l'eliminazione occorre cancellare i dati tramite il modulo 'Agenda'!")
            Else

                'cancello l'appezzamento
                Dim ObjAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
                Dim ObjAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
                Dim Dati As String = ""
                'Leggo la stringa XML dell'oggetto
                Dati = ObjAppezzamentoR.Appezzamento_Leggi( _
                                        CStr(DtEntRib.Rows(a).Item("piva")), _
                                        CInt(DtEntRib.Rows(a).Item("sa_cod")), _
                                        CInt(0), _
                                        CInt(DtEntRib.Rows(a).Item("appezza")), _
                                        CBool(True), _
                                        CBool(False), _
                                        CBool(True), _
                                        CBool(False), _
                                        ObjParametri_Server)

                ObjAppezzamentoR = Nothing

                If Dati <> "" Then
                    'Cancello l'elemento
                    ObjAppezzamentoW.Appezzamento_Scrivi( _
                                            CStr(Dati), _
                                            Nothing, _
                                            Nothing, _
                                            Nothing, _
                                            ObjParametri_Server, _
                                            ObjParametri_Utenti)

                    ObjAppezzamentoW = Nothing
                End If

            End If

        Next


    End Sub

    Private Function _8_Ribalta_Pianificazione(ByVal Programmazione_Cod As Integer, _
                                               ByVal SchedaValidazione As String, _
                                               ByVal DataValidazione As Date, _
                                               ByVal DtPV As DataTable) As Boolean


        Dim i As Integer = 0
        Dim strErr As String = ""

        Dim Key_Piva As String
        Dim Key_SaCod As String
        Dim Key_CampoCod As Integer
        Dim Key_Appezza As Integer
        Dim Key_IdReg As Integer
        Dim Key_ProgettoCod As Integer
        Dim Key_Entita As Integer

        Dim StrCampo As String
        Dim StrAppezzamento As String
        Dim StrParticelle As String
        Dim StrRegImpianto As String
        Dim StrProgetto As String

        Dim strXMLAppezzamenti As String

        Dim OUTPUT_Piva As String
        Dim OUTPUT_Sa_Cod As Integer
        Dim OUTPUT_Campo_Cod As Integer
        Dim OUTPUT_Appezza As Integer
        Dim OUTPUT_Id_Reg As Integer
        Dim OUTPUT_Progetto_Cod As Integer

        Dim InseritoProgetto As Boolean = False
        Dim InseritoImpianto As Boolean = False
        Dim InseritoAppezzamento As Boolean = False
        Dim InseritoCampo As Boolean = False

        Dim ZeroData As String = "0"
        Dim ZeroInt As Integer = 0
        Dim ZeroString As String = "0"
        Dim ZeroDouble As Double = 0
        Dim NullString As String = ""
        Dim Null As String = "NULL"
        Dim PuntoString As String = "."

        Dim Progetto_Nome As String
        Dim Resa As Double
        Dim Stato_Impianto As Integer
        Dim Regolamento_Concimazioni_Cod As Integer = 5 'UMBRIA

        Dim Id_Cod As Integer
        Dim Veg_Cod As Integer
        'Dim Gru_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Grva_Cod As Integer

        Dim Veg_Cod_Cliente As String = ""
        Dim Cul_Cod_Cliente As String = ""

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim Sup_App As Double
        Dim Ettari As Double
        Dim Are As Double
        Dim Centiare As Double

        Dim objAzoto As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim Riduzione_N As Double = 0
        Dim RiduzioneP_N As Double = 0
        Dim Limite As Integer = 0
        Dim Prov, Com As String
        Dim Foglio As Integer

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim XmlDocAppoggio As System.Xml.XmlDocument

        Dim XmlAppezzamento As System.Xml.XmlElement
        Dim XML_DatiParticelle As System.Xml.XmlElement

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        '----- Calcolo i valori di BaseCode e TopCode
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, ProgressivoGIAS)

        Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
        Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
        Dim objCampo_W As New AgronicaCoreAnagrafeBIZ.Campo_W
        Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim objReg_Impianto_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
        Dim objReg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objProgetto_W As New AgronicaCoreAnagrafeBIZ.Progetto_W

        Try

            Dim DT_Appezzamenti As New DataTable

            DT_Appezzamenti = objP.Anagrafica_AppezzamentixRibaltamento_Leggi(CInt(Programmazione_Cod), _
                                                                              ObjParametri_Server.PivaSuperUser, _
                                                                              strErr, _
                                                                              ObjParametri_Server)

            '-----------------------------------------
            '-----------------------------------------
            '-----------------------------------------

            Dim DtPE As DataTable = Nothing

            If Ribalta_Limiti Then
                'PER LIMITI AZOTO!!!!!!!!!!!!!
                'Leggo le particelle della pianificazione
                Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                DtPE = objPE.LeggiParticelle_Da_Programmazione("", 0, Programmazione_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
            End If




            '-----------------------------------------
            '-----------------------------------------
            '-----------------------------------------

            For i = 0 To DT_Appezzamenti.Rows.Count - 1

                Key_Piva = DT_Appezzamenti.Rows(i).Item("Piva")
                Key_SaCod = DT_Appezzamenti.Rows(i).Item("Sa_Cod")
                Key_CampoCod = DT_Appezzamenti.Rows(i).Item("Campo_Cod")
                Key_Appezza = DT_Appezzamenti.Rows(i).Item("Appezza")
                Key_IdReg = DT_Appezzamenti.Rows(i).Item("Id_Reg")
                Key_ProgettoCod = DT_Appezzamenti.Rows(i).Item("Progetto_Cod")

                Key_Entita = DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod")

                Sup_App = DT_Appezzamenti.Rows(i).Item("Sup_App")

                Veg_Cod = DT_Appezzamenti.Rows(i).Item("Veg_Cod")
                Cul_Cod = DT_Appezzamenti.Rows(i).Item("Cul_Cod")
                Grva_Cod = DT_Appezzamenti.Rows(i).Item("Grva_Cod")
                Grfi_Cod = DT_Appezzamenti.Rows(i).Item("Grfi_Cod")
                Id_Cod = DT_Appezzamenti.Rows(i).Item("Id_Cod")

                Veg_Cod_Cliente = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Cliente")
                Cul_Cod_Cliente = DT_Appezzamenti.Rows(i).Item("Cul_Cod_Cliente")

                Progetto_Nome = DT_Appezzamenti.Rows(i).Item("Progetto_Nome")
                Resa = DT_Appezzamenti.Rows(i).Item("Resa")

                Stato_Impianto = 102

                Validita_Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Inizio"))
                Validita_Fine = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Fine"))

                'controllo che esistano le particelle
                ' inizio possesso
                If IsDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) Then
                    If Validita_Inizio < CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) Then
                        Validita_Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso"))
                    End If
                End If
                ' fine possesso
                If IsDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) Then
                    If Validita_Fine > CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) Then
                        Validita_Fine = CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso"))
                    End If
                End If


                '-----------------------------------------
                '-----------------------------------------
                '-----------------------------------------
                'Creo il Campo
                If i = 0 Then

                    StrCampo = ""
                    objXML.XML_Campo(enum_CodificaDecodifica.Codifica, _
                                            StrCampo, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Key_Piva, _
                                            Key_SaCod, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            "Fascicolo: " & SchedaValidazione & " (Data validazione " & DataValidazione.ToShortDateString & ")", _
                                            CostantiPersonalizzate.AGRODATAINIZIO, _
                                            CostantiPersonalizzate.AGRODATAINIZIO, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            NullString, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode)

                    If StrCampo <> "" Then

                        StrCampo = "<DatiCampi>" & StrCampo & "</DatiCampi>"

                        InseritoCampo = objCampo_W.Campo_Scrivi(StrCampo, _
                                                                OUTPUT_Piva, _
                                                                OUTPUT_Sa_Cod, _
                                                                OUTPUT_Campo_Cod, _
                                                                False, _
                                                                ObjParametri_Server, _
                                                                ObjParametri_Utenti)

                    End If


                End If



                If InseritoCampo Then


                    StrAppezzamento = ""
                    objXML.XML_Appezzamento(enum_CodificaDecodifica.Codifica, _
                                            StrAppezzamento, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Key_Piva, _
                                            Key_SaCod, _
                                            ZeroInt, _
                                            Sup_App, _
                                            ZeroData, _
                                            ZeroData, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            PuntoString, _
                                            ZeroInt, _
                                            PuntoString, _
                                            ZeroInt, _
                                            NullString, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroString, _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            ZeroData, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            NullString, _
                                            DT_Appezzamenti.Rows(i).Item("App_Nome"), _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            NullString, _
                                            OUTPUT_Campo_Cod, _
                                            ZeroInt, _
                                            ZeroData, _
                                            ZeroData, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode)


                    XmlDocAppoggio = New System.Xml.XmlDocument
                    XmlDocAppoggio.LoadXml(StrAppezzamento)

                    XmlAppezzamento = XmlDocAppoggio.SelectSingleNode("Appezzamento")

                    Dim Str_CodiceAppezzamento As String = ""
                    objXML.XML_Codice(enum_CodificaDecodifica.Codifica, _
                                      Str_CodiceAppezzamento, _
                                      enum_TipoOperazioneDB.Scrittura, _
                                      enum_CodiciAnagrafe.TitoloPossesso, _
                                      1, _
                                      Validita_Inizio, _
                                      Validita_Fine, _
                                      BaseCode, _
                                      TopCode, "Appezzamento")


                    Dim DT_Particelle As New DataTable

                    DT_Particelle = objPP.Programmazione_Particelle_Leggi_3(ObjParametri_Server, _
                                                                          Key_Piva, _
                                                                          strErr, _
                                                                          DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"))
                    StrParticelle = String.Empty

                    For j = 0 To DT_Particelle.Rows.Count - 1

                        Conversioni.EttariAreCentiare_from_Ettari(DT_Particelle.Rows(j).Item("Superficie"), Ettari, Are, Centiare)

                        StrParticelle &= objXML.XML_AppezzamentoParticella(enum_TipoOperazioneDB.Scrittura, _
                                                                          Key_Piva, _
                                                                          Key_SaCod, _
                                                                          ZeroInt, _
                                                                          DT_Particelle.Rows(j).Item("PROV"), _
                                                                          DT_Particelle.Rows(j).Item("COM"), _
                                                                          DT_Particelle.Rows(j).Item("Sezione"), _
                                                                          DT_Particelle.Rows(j).Item("foglio"), _
                                                                          DT_Particelle.Rows(j).Item("numero"), _
                                                                          DT_Particelle.Rows(j).Item("subalterno"), _
                                                                          DT_Particelle.Rows(j).Item("Superficie"), _
                                                                          Ettari, _
                                                                          Are, _
                                                                          Centiare, _
                                                                          ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, _
                                                                          Validita_Inizio, _
                                                                          Validita_Fine, _
                                                                          BaseCode, _
                                                                          TopCode)

                    Next

                    XmlAppezzamento.InnerXml = Str_CodiceAppezzamento

                    If StrParticelle <> "" Then
                        XML_DatiParticelle = XmlAppezzamento.OwnerDocument.CreateElement("DatiParticelle")
                        XmlAppezzamento.AppendChild(XML_DatiParticelle)
                        XML_DatiParticelle.InnerXml = StrParticelle
                    End If

                    strXMLAppezzamenti = "<DatiAppezzamenti>" & XmlAppezzamento.OuterXml & "</DatiAppezzamenti>"

                    InseritoAppezzamento = objAppezzamento_W.Appezzamento_Scrivi(strXMLAppezzamenti, _
                                                                                OUTPUT_Piva, _
                                                                                OUTPUT_Sa_Cod, _
                                                                                OUTPUT_Appezza, _
                                                                                ObjParametri_Server, _
                                                                                ObjParametri_Utenti)


                    If InseritoAppezzamento Then

                        StrRegImpianto = String.Empty
                        objXML.XML_Impianto(enum_CodificaDecodifica.Codifica, _
                                            StrRegImpianto, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            OUTPUT_Piva, _
                                            OUTPUT_Sa_Cod, _
                                            OUTPUT_Campo_Cod, _
                                            OUTPUT_Appezza, _
                                            ZeroInt, _
                                            Sup_App, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            Validita_Inizio, _
                                            Cul_Cod, _
                                            Grva_Cod, _
                                            Null, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroInt, _
                                            ZeroInt, ZeroString, NullString, NullString, NullString, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroString, _
                                            ZeroInt, _
                                            ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, _
                                            NullString, ZeroString, _
                                            Grfi_Cod, _
                                            ZeroInt, _
                                            CInt(1), _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, ZeroInt, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode, _
                                            ZeroInt)

                        StrRegImpianto = "<DatiReg_Impianti>" & StrRegImpianto & "</DatiReg_Impianti>"

                        'salvataggio!!!
                        InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi( _
                                                        CStr(StrRegImpianto), _
                                                        OUTPUT_Piva, _
                                                        OUTPUT_Sa_Cod, _
                                                        OUTPUT_Appezza, _
                                                        OUTPUT_Id_Reg, _
                                                        "", _
                                                        ObjParametri_Server)


                        If InseritoImpianto Then
                            '---------------------------------
                            'caso destinazioni d'uso
                            If Id_Cod <> 0 Then
                                objReg_Impianti_Codici_W.ScrivixProgetto(DT_Appezzamenti.Rows(i).Item("Piva"), _
                                                                         Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, Key_ProgettoCod, _
                                                                Id_Cod, _
                                                                "", _
                                                                Validita_Inizio, Validita_Fine, ObjParametri_Server)
                            End If


                            'CREO LA DISTINTA
                            StrProgetto = ""
                            objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica, _
                                                           StrProgetto, _
                                                           enum_TipoOperazioneDB.Scrittura, _
                                                           Key_Piva, _
                                                           Key_SaCod, _
                                                           ZeroInt, _
                                                           Progetto_Nome, _
                                                           NullString, _
                                                           enum_Agenda_Causali.Progetto_Produzione_Agricola, _
                                                            ZeroInt, _
                                                           ZeroInt, _
                                                           CostantiPersonalizzate.AGRODATAINIZIO, _
                                                           CostantiPersonalizzate.AGRODATAFINE, _
                                                           NullString, _
                                                           OUTPUT_Appezza, _
                                                           OUTPUT_Id_Reg, _
                                                           0, _
                                                           0, _
                                                           Stato_Impianto, _
                                                           1, _
                                                           ZeroInt, _
                                                           -Regolamento_Concimazioni_Cod, _
                                                           ZeroInt, _
                                                           ZeroDouble, _
                                                           Resa, _
                                                           Validita_Inizio, _
                                                           Validita_Fine, _
                                                           BaseCode, _
                                                           TopCode)

                            InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi( _
                                                            CStr(StrProgetto), _
                                                            OUTPUT_Piva, _
                                                            OUTPUT_Progetto_Cod, _
                                                            ObjParametri_Server)

                            If InseritoProgetto Then


                                If Ribalta_Limiti Then
                                    Riduzione_N = 0
                                    RiduzioneP_N = 0

                                    If DtPV IsNot Nothing AndAlso DtPV.Rows.Count > 0 Then

                                        Dim DrPE() As DataRow
                                        If DtPE IsNot Nothing AndAlso DtPE.Rows.Count > 0 Then
                                            DrPE = DtPE.Select("Programmazione_Entita_Cod=" & Key_Entita.ToString)
                                            If DrPE IsNot Nothing AndAlso DrPE.Length > 0 Then
                                                For p = 0 To DrPE.Length - 1
                                                    RiduzioneP_N = 0
                                                    Prov = DrPE(p).Item("prov")
                                                    Com = DrPE(p).Item("com")
                                                    Foglio = DrPE(p).Item("foglio")
                                                    Dim DrPV() As DataRow
                                                    DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND foglio=" & Foglio.ToString)
                                                    If DrPV IsNot Nothing AndAlso DrPV.Length > 0 Then
                                                        If Not IsDBNull(DrPV(0).Item("Riduzione_N")) AndAlso CDbl(DrPV(0).Item("Riduzione_N")) <> 0 Then
                                                            RiduzioneP_N = CDbl(DrPV(0).Item("Riduzione_N"))
                                                        End If
                                                    End If
                                                    If Riduzione_N < RiduzioneP_N Then
                                                        Riduzione_N = RiduzioneP_N
                                                    End If
                                                Next
                                            End If
                                        End If
                                    End If

                                    Limite = 0
                                    Limite = objAzoto.RecuperaAzotoFromVegCod_Regolamento(Veg_Cod, _
                                                                       Grfi_Cod, _
                                                                      Stato_Impianto, _
                                                                      Validita_Inizio, _
                                                                      Validita_Inizio, _
                                                                      True, _
                                                                      Regolamento_Concimazioni_Cod, _
                                                                      ObjParametri_Server)

                                    'riduco in percentuale in caso ci sia da ridurre l'apporto max di azoto
                                    If Limite <> 0 AndAlso Riduzione_N <> 0 Then
                                        Limite = Limite - (Limite * Riduzione_N / 100)
                                    End If

                                    If Limite <> 0 Then
                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                                 enum_CodiciAnagrafe.Impianto_LimiteN, _
                                                                                 Limite, _
                                                                                 Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                    End If

                                End If

                                '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                If Veg_Cod_Cliente <> "" Then
                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                     enum_CodiciAnagrafe.Codice_Specie_Agea, _
                                                                     Veg_Cod_Cliente, _
                                                                    Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                End If

                                If Cul_Cod_Cliente <> "" Then
                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                     enum_CodiciAnagrafe.Codice_Cultivar_Agea, _
                                                                     Cul_Cod_Cliente, _
                                                                    Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                End If

                                Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                reg_impianti_programmazione_w.Scrivi( _
                                    Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                    Programmazione_Cod, Key_Entita, Validita_Inizio, Validita_Fine, _
                                    ObjParametri_Server _
                                )



                            End If

                        End If

                    End If

                End If


            Next

        Catch ex As Exception

            Return False

        End Try

        Return True

    End Function


#End Region







#Region "Util"


    Private Sub SerializeObject(ByVal filename As String, ByVal ws_fasciResponse As SincroAnagrafeBA1.getFascicoloNewResponse)
        Console.WriteLine("Writing With Stream")

        Dim serializer As New XmlSerializer(GetType(SincroAnagrafeBA1.getFascicoloNewResponse))

        ' Create a FileStream to write with.
        Dim writer As New FileStream(filename, FileMode.Create)
        ' Serialize the object, and close the TextWriter
        serializer.Serialize(writer, ws_fasciResponse)
        writer.Close()
    End Sub


    Private Function RecuperaSaCodImpresa(ByVal CodiceChiaveCliente As Integer, ByVal Piva As String, ByVal ValCod As String) As Integer

        Dim SaCod As Integer = 0
        Dim Dt As DataTable

        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

        ObjParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Now.Date, Now.Date)
        Dt = objCodici.Leggi(Piva, 0, _
                             CodiceChiaveCliente, _
                             "", _
                             ValCod, _
                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                              "", "", _
                              ObjParametri_Server)

        ObjParametri_Server.ResettaFinestra()
        objCodici = Nothing

        Dim i As Integer
        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
            SaCod = Dt.Rows(0).Item("Sa_Cod")
            For i = 1 To Dt.Rows.Count - 1
                If Dt.Rows(i).Item("Sa_Cod") < SaCod Then
                    SaCod = Dt.Rows(i).Item("Sa_Cod")
                End If
            Next
        End If

        Dt = Nothing

        Return SaCod

    End Function


    Private Function Converti_TitoliPossesso_Fascicolo(ByVal TitoloPossesso As String, _
                                                   ByRef TitoloPossessoDes As String) As Integer

        Dim Particella_Possesso As Integer

        Select Case TitoloPossesso

            Case "1"
                Particella_Possesso = 1 'Proprietà
                TitoloPossessoDes = "Proprieta"
            Case "2"
                Particella_Possesso = 3 'Affitto con contratto
                TitoloPossessoDes = "Affitto"
            Case Else
                Particella_Possesso = 0 'altro
                TitoloPossessoDes = "Altro"
        End Select

        Return Particella_Possesso

    End Function


    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub


    'controllo se il fascicolo è presente
    'ATTENZIONE, la Allegati_Documenti_Piva ha alcune volte il cuaa, altre il superuser, 
    'quindi controllo solo il numero
    'PIVA As String, CUAA As String, 
    'Private Function FascicoloPresente(SchedaValidazione As String) As Boolean

    '    Dim Allegati_Documenti_Cod_RITORNO As Integer = 0

    '    Return Allegati_Documenti_R.EsisteDocumento_Da_Numero(SchedaValidazione, "", enum_CategorieDocumenti.DomandaFascicolo, Allegati_Documenti_Cod_RITORNO, ObjParametri_Server)

    'End Function


    Private Sub RicavoIParametriExtra(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)
        ' SSAWS_Uri = SSAWS_Uri = http://web_esb.coldiretti.it:80/SSAMMWeb/sca/SSAWS | FascicoloWS_Uri = http://web_collesb.coldiretti.it:80/FascicoloMMWeb/sca/FascicoloWS | Utente_BA_Username = lorenzo.belcapo | Utente_BA_CF = BLCLNZ74R30A577M | Aggiorna_Solo_Se_Fascicolo_Nuovo = True | RegolamentoCod= 4 | annoPrecedente = False|Importa_PianoColturale = True|Importa_Anagrafica = True|Importa_Condizionalita = True| DataDa = 20130101 | DataA = 20141231

        Dim Parametri_Extra As String() = _Configurazione_Servizio.Parametri_Extra.Split("|")
        Dim Parametri_Extra_HT As New Hashtable
        For Each parametro As String In Parametri_Extra
            If parametro.Split("=").Count <> 2 Then
                Throw New Exception("Errore nella lettura dei parametri extra: parametro.Split(" = ").Count <> 2: " & parametro)
            End If
            Dim key As String = parametro.Split("=")(0).Trim
            Dim valore As String = parametro.Split("=")(1).Trim
            Parametri_Extra_HT.Add(key, valore)
        Next

        Try

            'If IsNothing(Parametri_Extra_HT("annoPrecedente")) Then
            '    Throw New Exception("manca il parametro obbligatorio annoPrecedente")
            'End If
            If IsNothing(Parametri_Extra_HT("Importa_PianoColturale")) Then
                Throw New Exception("manca  il parametro obbligatorio Importa_PianoColturale")
            End If
            If IsNothing(Parametri_Extra_HT("Importa_Anagrafica")) Then
                Throw New Exception("manca  il parametro obbligatorio  Importa_Anagrafica")
            End If
            If IsNothing(Parametri_Extra_HT("Importa_Condizionalita")) Then
                Throw New Exception("manca  il parametro obbligatorio  Importa_Condizionalita")
            End If
            If IsNothing(Parametri_Extra_HT("RegolamentoCod")) Then
                Throw New Exception("manca  il parametro obbligatorio  RegolamentoCod")
            End If
            If IsNothing(Parametri_Extra_HT("Aggiorna_Solo_Se_Fascicolo_Nuovo")) Then
                Throw New Exception("manca  il parametro obbligatorio  Aggiorna_Solo_Se_Fascicolo_Nuovo")
            End If
            If IsNothing(Parametri_Extra_HT("Utente_BA_Username")) Or Parametri_Extra_HT("Utente_BA_Username") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  Utente_BA_Username")
            End If
            If IsNothing(Parametri_Extra_HT("Utente_BA_CF")) Or Parametri_Extra_HT("Utente_BA_CF") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  Utente_BA_CF")
            End If
            If IsNothing(Parametri_Extra_HT("SSAWS_Uri")) Or Parametri_Extra_HT("SSAWS_Uri") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  SSAWS_Uri")
            End If
            If IsNothing(Parametri_Extra_HT("FascicoloWS_Uri")) Or Parametri_Extra_HT("FascicoloWS_Uri") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  FascicoloWS_Uri")
            End If
            If IsNothing(Parametri_Extra_HT("DataDa")) Or Parametri_Extra_HT("DataDa") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  DataDa")
            End If
            If IsNothing(Parametri_Extra_HT("DataA")) Or Parametri_Extra_HT("DataA") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  DataA")
            End If
            If IsNothing(Parametri_Extra_HT("Ribalta_Automatico")) Or Parametri_Extra_HT("Ribalta_Automatico") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  Ribalta_Automatico")
            End If
            If IsNothing(Parametri_Extra_HT("Ribalta_Limiti")) Or Parametri_Extra_HT("Ribalta_Limiti") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  Ribalta_Limiti")
            End If
            If IsNothing(Parametri_Extra_HT("Modifica_TitoloPossesso")) Or Parametri_Extra_HT("Modifica_TitoloPossesso") = "" Then
                Throw New Exception("manca  il parametro obbligatorio  Modifica_TitoloPossesso")
            End If


            'annoPrecedente = CBool(Parametri_Extra_HT("annoPrecedente"))
            Importa_PianoColturale = CBool(Parametri_Extra_HT("Importa_PianoColturale"))
            Importa_Anagrafica = CBool(Parametri_Extra_HT("Importa_Anagrafica"))
            Importa_Condizionalita = CBool(Parametri_Extra_HT("Importa_Condizionalita"))
            RegolamentoCod = CInt(Parametri_Extra_HT("RegolamentoCod"))
            Aggiorna_Solo_Se_Fascicolo_Nuovo = CBool(Parametri_Extra_HT("Aggiorna_Solo_Se_Fascicolo_Nuovo"))
            Utente_BA_Username = CStr(Parametri_Extra_HT("Utente_BA_Username"))
            Utente_BA_CF = CStr(Parametri_Extra_HT("Utente_BA_CF"))
            SSAWS_Uri = CStr(Parametri_Extra_HT("SSAWS_Uri"))
            FascicoloWS_Uri = CStr(Parametri_Extra_HT("FascicoloWS_Uri"))
            DataDa = CInt(Parametri_Extra_HT("DataDa"))
            DataA = CInt(Parametri_Extra_HT("DataA"))
            Ribalta_Automatico = CBool(Parametri_Extra_HT("Ribalta_Automatico"))
            Ribalta_Limiti = CBool(Parametri_Extra_HT("Ribalta_Limiti"))
            Modifica_TitoloPossesso = CBool(Parametri_Extra_HT("Modifica_TitoloPossesso"))

            If Not IsNothing(Parametri_Extra_HT("CUAA_Test")) Then
                If Not String.IsNullOrEmpty(Parametri_Extra_HT("CUAA_Test") = "") Then
                    CUAA_Test = CStr(Parametri_Extra_HT("CUAA_Test"))
                End If
            End If

        Catch ex As Exception

            Throw New Exception("Errore nella lettura dei parametri extra in configurazione_siti.parametri_extra:  " & ex.Message)

        End Try

    End Sub


    Private Sub InizializzoOggettiCore()

        Imprese_Read = New AgronicaCoreAnagrafeDAL.Imprese_Read
        GerarchiaImprese_R = New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Imprese_Codici_Read = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Utenti_Read = New AgronicaCoreUtentiDAL.Utenti_Read
        Utenti_CodiciGiasPRO_R = New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Configurazione_Siti_R = New AgronicaCoreVarieDAL.Configurazione_Siti_R
        objLog = New AgronicaCoreDataProvider.LogProvider
        Allegati_Documenti_R = New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Configurazione_Servizi_R = New AgronicaCoreVarieDAL.Configurazione_Servizi_R
        objSchedeProc = New AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_R
        objSchedeProc_W = New AgronicaCoreAnagrafeDAL.AgroBA_Fascicolo_W
        Reg_Impianti_Programmazioni_R = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R

    End Sub


    Private Sub ImpostoGliAltriParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        ID_Tabella_PivaSuperuser = _Configurazione_Servizio.PivaSuperuser
        ID_Tabella_Id_Servizio = _Configurazione_Servizio.Id_Servizio
        ID_Tabella_Tipo_Sincro = _Configurazione_Servizio.Tipo_Sincro
        ID_Tabella_Id_Riga = _Configurazione_Servizio.Id_Riga

        SuperUserUsername = ObjParametri_Server.SuperUserUsername
        SuperUserPassword = Utenti_Read.Password_From_UserName(ObjParametri_Server.SuperUserUsername, ObjParametri_Utenti)
        SuperUserPiva = ObjParametri_Server.PivaSuperUser
        Codice_Chiave_Cliente = _Configurazione_Servizio.Id_Cod_Cliente
        Piva_Padre = _Configurazione_Servizio.Piva_Padre
        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni

        ProgressivoGIAS = Utenti_CodiciGiasPRO_R.ProgressivoGias_from_Superuser(ObjParametri_Utenti)

        LinkWSImportaGIAS = Configurazione_Siti_R.Leggi_Valore(Enum_SiteRedirector.Sito_GiasOnline, "LinkWSImportaGIAS", "", "", ObjParametri_Server)

    End Sub


    Private Function ControllaPeriodoEsecuzione(ByRef msgp As String) As Boolean
        'verifico se sono fuori dal periodo di esecuzione
        'o se disattivato
        'per sicurazza lascio stare gli altri parametri e non li aggirono
        Dim Configurazione_Servizio_temp As AgronicaCoreVarieDAL.Configurazione_Servizio = Configurazione_Servizi_R.LeggiSingolo( _
                                                            ID_Tabella_PivaSuperuser, _
                                                            ID_Tabella_Id_Servizio, _
                                                            ID_Tabella_Tipo_Sincro, _
                                                            ID_Tabella_Id_Riga, _
                                                             "", _
                                                             ObjParametri_SuperServer)


        If Configurazione_Servizio_temp.Attivo <> 1 Then
            msgp = "Il task è impostato come NON ATTIVO."
            Return False
        End If


        Dim data_corrente As DateTime = DateTime.Now

        'Controllo il giorno della settimana
        If Configurazione_Servizio_temp.Giorno_Settimana_Esecuzione <> enum_Giorno_Settimana.Indifferente AndAlso
           Configurazione_Servizio_temp.Giorno_Settimana_Esecuzione <> data_corrente.DayOfWeek Then
            msgp = "Il giorno della settimana non corrisponde."
            Return False
        End If


        'controllo l'ora se passata
        Dim ora As Integer = data_corrente.Hour
        If ora > Configurazione_Servizio_temp.Esecuzione_Ora_Intervallo_Fine Then
            msgp = "L'ora massima di esecuzione del servizio è passata."
            Return False
        End If


        Return True

    End Function

#End Region







End Class
