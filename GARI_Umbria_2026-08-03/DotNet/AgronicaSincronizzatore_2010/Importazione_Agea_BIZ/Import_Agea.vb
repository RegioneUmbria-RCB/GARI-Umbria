Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ

Public Class Import_Agea

    Public Sub importa_CUAAFS7(cuaa As String,
                                 ByRef MessaggioFinale As String,
                                 importa_azienda As Boolean,
                                 importa_fabbricati As Boolean,
                                 importa_conticorrenti As Boolean,
                                 importa_macchhine As Boolean,
                                 importa_piano_colturale As Boolean,
                                 ASG_ProgressivoGIAS As Integer,
                                 ASG_Utente_Password As String,
                                 objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri,
                                 PivaPadre As String)

        Dim getFascicolo As New Import_WS_Agea

        Dim ISWSRespAnagFascicolo15 As New List(Of ISWSRespAnagFascicolo15)
        Dim ISWSContoCorrente As New List(Of ISWSContoCorrente)
        Dim ISWSFabbricatoFS6 As New List(Of ISWSFabbricatoFS6)
        Dim ISWSMacchina As New List(Of ISWSMacchina)
        Dim ISWSTerritorioFS6 As New List(Of ISWSTerritorioFS6)

        getFascicolo.Importa_FascicoloFS7(cuaa, 0, ISWSRespAnagFascicolo15, ISWSContoCorrente, ISWSFabbricatoFS6, ISWSMacchina, ISWSTerritorioFS6, objParametri_Server)

    End Sub

    Public Sub leggiSchedaFascicolo_CUAA(cuaa As String,
                                         objParametri_Server As AgronicaCoreParametri,
                                         ByRef NumeroValidazione As String,
                                         ByRef DataValidazione As Date)

        NumeroValidazione = ""
        DataValidazione = AGRODATAINIZIO

        Dim getFascicolo As New Import_WS_Agea

        Dim ISWSRespAnagFascicolo15 As New List(Of ISWSRespAnagFascicolo15)
        Dim ISWSContoCorrente As New List(Of ISWSContoCorrente)
        Dim ISWSFabbricatoFS6 As New List(Of ISWSFabbricatoFS6)
        Dim ISWSMacchina As New List(Of ISWSMacchina)
        Dim ISWSTerritorioFS6 As New List(Of ISWSTerritorioFS6)

        getFascicolo.Importa_Fascicolo(cuaa,
                                       0,
                                       ISWSRespAnagFascicolo15,
                                       ISWSContoCorrente,
                                       ISWSFabbricatoFS6,
                                       ISWSMacchina,
                                       ISWSTerritorioFS6,
                                       objParametri_Server)

        Dim str = Newtonsoft.Json.JsonConvert.SerializeObject(ISWSTerritorioFS6)

        If ISWSRespAnagFascicolo15.Count > 0 Then

            Dim fascicolo = ISWSRespAnagFascicolo15(0)

            If ISWSRespAnagFascicolo15(0).schedaValidazione IsNot Nothing Then
                NumeroValidazione = ISWSRespAnagFascicolo15(0).schedaValidazione
            End If

            If ISWSRespAnagFascicolo15(0).dataSchedaValidazione IsNot Nothing Then
                If IsDate(ISWSRespAnagFascicolo15(0).dataSchedaValidazione) Then
                    DataValidazione = CDate(fascicolo.dataNascitaPF).ToShortDateString
                ElseIf IsNumeric(ISWSRespAnagFascicolo15(0).dataSchedaValidazione) Then
                    DataValidazione = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSRespAnagFascicolo15(0).dataSchedaValidazione)).ToShortDateString
                End If
            End If

        End If

    End Sub

    Public Sub importa_CUAA(cuaa As String,
                            ByRef MessaggioFinale As String,
                            importa_azienda As Boolean,
                            importa_fabbricati As Boolean,
                            importa_conticorrenti As Boolean,
                            importa_macchhine As Boolean,
                            importa_piano_colturale As Boolean,
                            ASG_ProgressivoGIAS As Integer,
                            ASG_Utente_Password As String,
                            objParametri_Server As AgronicaCoreParametri,
                            objParametri_Utenti As AgronicaCoreParametri,
                            PivaPadre As String,
                            Optional ByVal importatoAutomaticamente As Boolean = False)

        Dim getFascicolo As New Import_WS_Agea

        Dim ISWSRespAnagFascicolo15 As New List(Of ISWSRespAnagFascicolo15)
        Dim ISWSContoCorrente As New List(Of ISWSContoCorrente)
        Dim ISWSFabbricatoFS6 As New List(Of ISWSFabbricatoFS6)
        Dim ISWSMacchina As New List(Of ISWSMacchina)
        Dim ISWSTerritorioFS6 As New List(Of ISWSTerritorioFS6)

        getFascicolo.Importa_Fascicolo(cuaa, 0, ISWSRespAnagFascicolo15, ISWSContoCorrente, ISWSFabbricatoFS6, ISWSMacchina, ISWSTerritorioFS6, objParametri_Server)
        Dim trovaPadre As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        If False Then
            Dim strTerritorio = Newtonsoft.Json.JsonConvert.SerializeObject(ISWSTerritorioFS6)
            Dim strFascicolo = Newtonsoft.Json.JsonConvert.SerializeObject(ISWSRespAnagFascicolo15)



            Dim objLog As New AgronicaCoreDataProvider.LogProvider

            objLog.Scrivi_LOG(objParametri_Server,
                              "importa_CUAA strFascicolo",
                              strFascicolo)

            objLog.Scrivi_LOG(objParametri_Server,
                              "importa_CUAA strTerritorio",
                              strTerritorio)
        End If


        If ISWSRespAnagFascicolo15.Count > 0 Then
            Dim fascicolo = ISWSRespAnagFascicolo15(0)
            Dim Piva_Padre As String = objParametri_Server.PivaSuperUser
            Dim Sigla_Provincia As String = ""
            Dim Comune As String = ""
            Dim RagSoc As String = ""
            Dim Indirizzo As String = ""
            Dim Cap As String = ""
            Dim Istat_Provincia As String = ""
            Dim Istat_Comune As String = ""
            Dim strErr As String = ""
            Dim Piva As String = ""

            Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read

            If fascicolo.detentore IsNot Nothing AndAlso PivaPadre = "" Then

                Dim Detentore_Fascicolo As String = fascicolo.detentore

                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim SetupAssociaPadrePivaSuperUser As String = objConfSiti.Leggi_Valore(0,
                                                                                        "AgroFascicolo_WS_PadrePivaSuperUser",
                                                                                        "",
                                                                                        "",
                                                                                        objParametri_Server)

                Dim AssociaPadrePivaSuperUser = False

                Try
                    AssociaPadrePivaSuperUser = Boolean.Parse(SetupAssociaPadrePivaSuperUser)
                Catch ex As Exception
                    AssociaPadrePivaSuperUser = False
                End Try

                If AssociaPadrePivaSuperUser Then

                    Piva_Padre = objParametri_Server.PivaSuperUser

                Else

                    SeCreaUfficioZona(MessaggioFinale, ASG_ProgressivoGIAS, ASG_Utente_Password, objParametri_Server, objParametri_Utenti, Piva_Padre, Sigla_Provincia, Istat_Provincia, Istat_Comune, strErr, objImpresa, Detentore_Fascicolo)

                End If

            Else

                If PivaPadre <> "" Then

                    Piva_Padre = PivaPadre

                    If Piva = Piva_Padre Then
                        Piva_Padre = ""
                    End If

                End If

            End If

            '****** IMPRESA *********************************************************************************************

            Select Case fascicolo.tipoAzienda
                Case ISWSRespAnagFascicolo15TipoAzienda.PF '"PF", "P", "1", "2"
                    RagSoc = fascicolo.denominazione & " " & fascicolo.nomePF
                Case ISWSRespAnagFascicolo15TipoAzienda.PG '"PG", "G"
                    RagSoc = fascicolo.denominazione
            End Select

            Sigla_Provincia = ""
            Comune = ""

            If fascicolo.sedeResidenza IsNot Nothing Then
                Cap = fascicolo.sedeResidenza.cap
                Indirizzo = fascicolo.sedeResidenza.indirizzo
                Istat_Provincia = fascicolo.sedeResidenza.provincia
                Istat_Comune = fascicolo.sedeResidenza.comune
            Else
                If fascicolo.recapito IsNot Nothing Then
                    Cap = fascicolo.recapito.cap
                    Indirizzo = fascicolo.recapito.indirizzo
                    Istat_Provincia = fascicolo.recapito.provincia
                    Istat_Comune = fascicolo.recapito.comune
                End If
            End If

            If Istat_Provincia = "" Then
                strErr = "CUAA:" & cuaa & " - " & "Impossibile inserire l'impresa poiché manca l'indirizzo!"
                Exit Sub
            Else
                If Indirizzo = "" Then
                    Indirizzo = ","
                End If
                If Cap = "" Then
                    Cap = "00000"
                End If

                Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
                Dim Dt_Istat As New DataTable

                Dt_Istat = objIstat.Leggi(Istat_Provincia,
                                      Istat_Comune,
                                      "",
                                      "",
                                      "",
                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      "", "",
                                      objParametri_Server)

                If Dt_Istat IsNot Nothing AndAlso Dt_Istat.Rows.Count > 0 Then
                    Sigla_Provincia = Dt_Istat.Rows(0).Item("COMUNI_PROV")
                    Comune = Dt_Istat.Rows(0).Item("LOCALITA")
                End If

            End If

            Piva = objImpresa.Piva_From_CUAA(cuaa, objParametri_Server)

            If Piva = "" Then
                Piva = cuaa
            Else
                Piva_Padre = trovaPadre.LeggiPrimaPivaPadre(Piva, objParametri_Server)
            End If

            'creo l'azienda
            Dim rval As Boolean = CreaAziendaGias(ASG_ProgressivoGIAS, ASG_Utente_Password,
                                                  Piva_Padre, Piva, cuaa, RagSoc,
                                                  Indirizzo, Cap, Comune, Sigla_Provincia, Istat_Comune, Istat_Provincia,
                                                  True, True,
                                                  strErr,
                                                  MessaggioFinale,
                                                  objParametri_Server, objParametri_Utenti, fascicolo,
                                                  ISWSContoCorrente,
                                                  ISWSFabbricatoFS6,
                                                  ISWSMacchina,
                                                  ISWSTerritorioFS6)

            'Return rval

            If importa_piano_colturale Then

                'Importo il Piano Colturale
                Dim DtParticelle As New DataTable
                dtParticelleCreaStruttura(DtParticelle)

                Dim DataInizio As Date
                Dim DataFine As Date

                Popola_DtAppezzamenti_daFascicolo_2(Piva,
                                                    fascicolo,
                                                    ISWSTerritorioFS6,
                                                    False,
                                                    False,
                                                    DataInizio,
                                                    DataFine,
                                                    DtParticelle,
                                                    objParametri_Server,
                                                    objParametri_Utenti,
                                                    ISWSRespAnagFascicolo15(0).schedaValidazione)

                Dim dtParticelleAggregate As New DataTable
                dtParticelleCreaStruttura(dtParticelleAggregate)
                dtParticelleAggrega(0, False, False, DtParticelle, dtParticelleAggregate)

                Dim strAppezzamenti = JSON_DataTableAppezzamenti_Tabella(dtParticelleAggregate, DataInizio, DataFine)

                Dim imp As New AgronicaCoreAnagrafeBIZ.ImportazionePcFast
                Dim numeroFascicolo As String = ""
                Dim dataFascicolo As Date = AGRODATAINIZIO
                If ISWSRespAnagFascicolo15(0).schedaValidazione IsNot Nothing Then
                    numeroFascicolo = ISWSRespAnagFascicolo15(0).schedaValidazione
                End If

                If ISWSRespAnagFascicolo15(0).dataSchedaValidazione IsNot Nothing Then

                    If IsDate(ISWSRespAnagFascicolo15(0).dataSchedaValidazione) Then
                        dataFascicolo = CDate(fascicolo.dataNascitaPF).ToShortDateString
                    ElseIf IsNumeric(ISWSRespAnagFascicolo15(0).dataSchedaValidazione) Then
                        dataFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSRespAnagFascicolo15(0).dataSchedaValidazione)).ToShortDateString
                    End If

                    imp.Aggiorna_PcFast_PianoColtuale(Piva,
                                                      cuaa,
                                                      0,
                                                      DataInizio,
                                                      DataFine,
                                                      numeroFascicolo,
                                                      dataFascicolo,
                                                      enum_Planning_Fonte.AGEA_RealTime,
                                                      ASG_ProgressivoGIAS,
                                                      0,
                                                      False,
                                                      False,
                                                      objParametri_Server,
                                                      objParametri_Utenti,
                                                      0,
                                                      strAppezzamenti,
                                                      importatoAutomaticamente:=importatoAutomaticamente)

                End If

            End If

        End If

    End Sub

    Private Sub SeCreaUfficioZona(ByRef MessaggioFinale As String, ASG_ProgressivoGIAS As Integer, ASG_Utente_Password As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef Piva_Padre As String, ByRef Sigla_Provincia As String, ByRef Istat_Provincia As String, ByRef Istat_Comune As String, ByRef strErr As String, objImpresa As AgronicaCoreAnagrafeDAL.Imprese_Read, ByRef Detentore_Fascicolo As String)
        If Detentore_Fascicolo <> "" AndAlso Detentore_Fascicolo.Length = 9 Then

            Dim bEsisteUffZona As Boolean = objImpresa.Esiste_Impresa("UZ" & Detentore_Fascicolo, 0, Nothing, Nothing, "", "", objParametri_Server)

            If bEsisteUffZona Then
                Piva_Padre = "UZ" & Detentore_Fascicolo
            Else
                Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                Dim CreaUffZona As Boolean = False
                Dim dt_Cod = objCAC.Leggi(5, Detentore_Fascicolo, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dt_Cod IsNot Nothing AndAlso dt_Cod.Rows.Count > 0 Then
                    Dim DetentoreCodificato As String = dt_Cod.Rows(0).Item("TestoAux_1")


                    If DetentoreCodificato <> "" Then
                        Detentore_Fascicolo = DetentoreCodificato
                        bEsisteUffZona = objImpresa.Esiste_Impresa(Detentore_Fascicolo, 0, Nothing, Nothing, "", "", objParametri_Server)
                        If Not bEsisteUffZona Then
                            CreaUffZona = True
                        Else
                            Piva_Padre = Detentore_Fascicolo
                        End If
                    End If
                End If

                If CreaUffZona Then
                    'creo l'ufficio di zona
                    Istat_Provincia = Detentore_Fascicolo.Substring(3, 3)
                    Dim objProv As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
                    Dim DtProv As DataTable
                    DtProv = objProv.Leggi("", "", Istat_Provincia, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    If DtProv.Rows.Count > 0 Then
                        Sigla_Provincia = DtProv.Rows(0).Item("sigla")
                        Istat_Comune = DtProv.Rows(0).Item("com")
                    End If

                    Detentore_Fascicolo = Detentore_Fascicolo.Replace("UZ", "")

                    If CreaAziendaGias(ASG_ProgressivoGIAS,
                                       ASG_Utente_Password,
                                       Piva_Padre,
                                       "UZ" & Detentore_Fascicolo,
                                       "UZ" & Detentore_Fascicolo,
                                       Detentore_Fascicolo,
                                       ".",
                                       "00000",
                                       "",
                                       Sigla_Provincia,
                                       Istat_Comune,
                                       Istat_Provincia,
                                       False,
                                       False,
                                       strErr,
                                       MessaggioFinale,
                                       objParametri_Server,
                                       objParametri_Utenti) = True Then
                        Piva_Padre = "UZ" & Detentore_Fascicolo
                    End If
                End If


            End If

        End If
    End Sub

    Public Function CreaAziendaGias(ByVal ASG_ProgressivoGIAS As String,
                                    ByVal ASG_Utente_Password As String,
                                    ByVal Piva_Padre As String,
                                    ByVal Piva As String,
                                    ByVal Cuaa As String,
                                    ByVal RagSoc As String,
                                    ByVal Indirizzo As String,
                                    ByVal Cap As String,
                                    ByVal Comune As String,
                                    ByVal Sigla_Provincia As String,
                                    ByVal Istat_Comune As String,
                                    ByVal Istat_Provincia As String,
                                    ByVal CreaCentro As Boolean,
                                    ByVal CreaCatasto As Boolean,
                                    ByRef strErr As String,
                                    ByRef strRis As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                    Optional fascicolo As ISWSRespAnagFascicolo15 = Nothing,
                                    Optional ISWSContoCorrente As List(Of ISWSContoCorrente) = Nothing,
                                    Optional ISWSFabbricatoFS6 As List(Of ISWSFabbricatoFS6) = Nothing,
                                    Optional ISWSMacchine As List(Of ISWSMacchina) = Nothing,
                                    Optional ISWSTerritorioFS6 As List(Of ISWSTerritorioFS6) = Nothing,
                                    Optional ByVal ImportaMacchine As Boolean = False,
                                    Optional ByVal ModificaPossessiEsistenti As Boolean = False) As Boolean

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlUtente As System.Xml.XmlElement

        Dim XmlImpresa As System.Xml.XmlElement
        Dim XmlCentro As System.Xml.XmlElement
        Dim XmlFabbricato As System.Xml.XmlElement
        Dim XmlMacchina As System.Xml.XmlElement

        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim tipoOperazione As enum_TipoOperazioneDB
        If objImprese.Esiste_Impresa(Piva, 0, Nothing, "", "", "", objParametri_Server) Then
            tipoOperazione = enum_TipoOperazioneDB.Modifica
            'CreaCentro = False
        Else
            tipoOperazione = enum_TipoOperazioneDB.Scrittura
        End If

        Dim stringaAnagrafica As String = ""

        Dim Legale_Rappresentante_Nome As String = "#"
        Dim Legale_Rappresentante_Cognome As String = "#"
        Dim Legale_Rappresentante_CF As String = "#"
        Dim Legale_Rappresentante_Sesso As String = "#"
        Dim Legale_Rappresentante_Data_Nascita As String = "#"
        Dim Legale_Rappresentante_Indirizzo As String = "#"
        Dim Legale_Rappresentante_Frazione As String = "#"
        Dim Legale_Rappresentante_Cap As String = "#"
        Dim Legale_Rappresentante_Comune As String = "#"
        Dim Legale_Rappresentante_Provincia As String = "#"
        Dim Legale_Rappresentante_Stato As String = "#"
        Dim Legale_Rappresentante_Istat_Comune As String = "#"
        Dim Legale_Rappresentante_Istat_Provincia As String = "#"
        Dim Legale_Rappresentante_Comune_Nascita As String = "#"
        Dim Legale_Rappresentante_Provincia_Nascita As String = "#"
        Dim Legale_Rappresentante_Istat_Comune_Nascita As String = "#"
        Dim Legale_Rappresentante_Istat_Provincia_Nascita As String = "#"
        Dim Legale_Rappresentante_Rubrica1 As String = "#"
        Dim Legale_Rappresentante_Rubrica2 As String = "#"
        Dim Legale_Rappresentante_Rubrica3 As String = "#"
        Dim Legale_Rappresentante_Rubrica4 As String = "#"
        Dim Legale_Rappresentante_Rubrica5 As String = "#"

        If fascicolo.nomePF IsNot Nothing Then
            Legale_Rappresentante_Nome = fascicolo.nomePF
        End If

        'Legale_Rappresentante_Cognome = ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.RappresentanteLegaleWS(0).SoggettoWS.Desc_cogn        
        'Legale_Rappresentante_CF = ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.RappresentanteLegaleWS(0).SoggettoWS.CUAA
        If fascicolo.sessoPFSpecified AndAlso fascicolo.sessoPF IsNot Nothing Then
            Select Case fascicolo.sessoPF.Value
                Case ISWSRespAnagFascicolo15SessoPF.F
                    Legale_Rappresentante_Sesso = "F"
                Case ISWSRespAnagFascicolo15SessoPF.M
                    Legale_Rappresentante_Sesso = "M"
            End Select
        End If

        If fascicolo.dataNascitaPF IsNot Nothing Then
            'qui la data nascita è data
            If IsDate(fascicolo.dataNascitaPF) Then
                Legale_Rappresentante_Data_Nascita = CDate(fascicolo.dataNascitaPF).ToShortDateString
            ElseIf IsNumeric(fascicolo.dataNascitaPF) Then
                Legale_Rappresentante_Data_Nascita = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataNascitaPF)).ToShortDateString
            Else
                Legale_Rappresentante_Data_Nascita = UtilityProvider.DataNascita_from_CodFisc(Legale_Rappresentante_CF)
            End If
        Else
            Legale_Rappresentante_Data_Nascita = UtilityProvider.DataNascita_from_CodFisc(Legale_Rappresentante_CF)
        End If

        XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc,
                                       objParametri_Server.UtenteUsername,
                                       "#",
                                       ASG_ProgressivoGIAS)


        If Piva = objParametri_Server.PivaSuperUser Then
            Piva_Padre = ""
        End If

        XmlImpresa = objXML.Xml_Pubblico_Impresa(tipoOperazione,
                                                              Piva,
                                                              RagSoc,
                                                              Cuaa.ToUpper,
                                                              Cuaa.ToUpper,
                                                              "#",
                                                              "#",
                                                              Piva_Padre,
                                                              "1",
                                                              "1",
                                                              "#", "#",
                                                              Indirizzo,
                                                              "#",
                                                              Cap,
                                                              Comune,
                                                              Sigla_Provincia,
                                                              "#", "#",
                                                              Istat_Comune,
                                                              Istat_Provincia,
                                                              "",
                                                              Legale_Rappresentante_Cognome,
                                                              Legale_Rappresentante_Nome,
                                                              Legale_Rappresentante_CF,
                                                              Legale_Rappresentante_Sesso,
                                                              "#", "#",
                                                              Legale_Rappresentante_Indirizzo,
                                                              Legale_Rappresentante_Frazione,
                                                              Legale_Rappresentante_Cap,
                                                              Legale_Rappresentante_Comune,
                                                              Legale_Rappresentante_Provincia,
                                                              Legale_Rappresentante_Stato,
                                                              "#",
                                                              Legale_Rappresentante_Istat_Comune,
                                                              Legale_Rappresentante_Istat_Provincia,
                                                              Legale_Rappresentante_Data_Nascita,
                                                              Legale_Rappresentante_Comune_Nascita,
                                                              Legale_Rappresentante_Provincia_Nascita,
                                                              Legale_Rappresentante_Istat_Comune_Nascita,
                                                              Legale_Rappresentante_Istat_Provincia_Nascita,
                                                              "#",
                                                              Legale_Rappresentante_Rubrica1,
                                                              Legale_Rappresentante_Rubrica2,
                                                              Legale_Rappresentante_Rubrica3,
                                                              Legale_Rappresentante_Rubrica4,
                                                              Legale_Rappresentante_Rubrica5,
                                                              "#", "#", "#", "#", "#", "#",
                                                              "#",
                                                              XmlDoc)

        XmlUtente.AppendChild(XmlImpresa)

        If ImportaMacchine Then

            Dim Mac_Cod As Integer = 0
            Dim Class_Code As String = ""
            Dim Ditta_Cod As Integer = 0
            Dim Mac_Des As String = ""
            Dim Targa As String = ""
            Dim Telaio As String = ""
            Dim Tipo As Integer = 0
            Dim TitoloPossesso As Integer = 0
            Dim Alimentazione_Cod As Integer = 0
            Dim Modello As String = ""
            Dim Tipo_Targa_Cod As Integer = 0
            Dim Tipo_Trazione_Cod As Integer = 0
            Dim Validita_Inizio As Date = AGRODATAINIZIO
            Dim Validita_Fine As Date = AGRODATAFINE
            Dim Marca As String = ""

            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            For Each ISWSMacchina In ISWSMacchine

                Mac_Cod = 0
                Class_Code = ""
                Mac_Des = ""
                Targa = ""
                Telaio = ""
                Tipo = 0
                TitoloPossesso = 0
                Alimentazione_Cod = 0
                Ditta_Cod = 0
                Modello = ""
                Tipo_Targa_Cod = 0
                Tipo_Trazione_Cod = 0
                Validita_Inizio = AGRODATAINIZIO
                Validita_Fine = AGRODATAFINE
                Marca = ""
                Ditta_Cod = 0

                If ISWSMacchina.TipoMacchina IsNot Nothing Then
                    Class_Code = Converti_TipoMacchina(ISWSMacchina.TipoMacchina)
                End If

                'If ISWSMacchina.FormaPossesso IsNot Nothing Then
                '    TitoloPossesso = Converti_FormaPossesso(ISWSMacchina.FormaPossesso)
                'End If

                If ISWSMacchina.Targa IsNot Nothing Then
                    Targa = ISWSMacchina.Targa
                End If

                'If ISWSMacchina.TipoTarga IsNot Nothing Then
                '    Tipo_Targa_Cod = Converti_TipoTarga(ISWSMacchina.TipoTarga)
                'End If

                If ISWSMacchina.Marca IsNot Nothing Then
                    Marca = ISWSMacchina.Marca
                End If

                If ISWSMacchina.Modello IsNot Nothing Then
                    Modello = ISWSMacchina.Modello
                End If

                If ISWSMacchina.Telaio IsNot Nothing Then
                    Telaio = ISWSMacchina.Telaio
                End If

                If ISWSMacchina.Carburante IsNot Nothing Then
                    Alimentazione_Cod = Converti_Carburante(ISWSMacchina.Carburante)
                End If

                If ISWSMacchina.Trazione IsNot Nothing Then
                    Tipo_Trazione_Cod = Converti_Trazione(ISWSMacchina.Trazione)
                End If

                If ISWSMacchina.DataIscrizione IsNot Nothing Then
                    Validita_Inizio = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSMacchina.DataIscrizione))
                End If

                If ISWSMacchina.DataCessazione IsNot Nothing Then
                    Validita_Fine = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSMacchina.DataCessazione))
                End If

                If ISWSMacchina.Marca IsNot Nothing Then
                    Marca = ISWSMacchina.Marca
                    Ditta_Cod = Converti_Marca(ISWSMacchina.Marca)
                End If

                Mac_Des = Marca & " - " & Modello

                Dim TipoOperazione_Macchina As New enum_TipoOperazioneDB
                If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                    TipoOperazione_Macchina = enum_TipoOperazioneDB.Scrittura
                Else
                    Dim esisteMacchina As Boolean = False
                    Dim DTMacchina = objMacchine.Leggi(Piva, 0, False, Class_Code, Targa, Telaio, Modello, "", 0, "", False, 0, "", False, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
                    If DTMacchina.Rows.Count > 0 Then
                        esisteMacchina = True
                        Mac_Cod = CInt(DTMacchina.Rows(0).Item("Mac_Cod"))
                    End If
                    If esisteMacchina Then
                        TipoOperazione_Macchina = enum_TipoOperazioneDB.Modifica
                    Else
                        TipoOperazione_Macchina = enum_TipoOperazioneDB.Scrittura
                    End If
                End If

                XmlMacchina = objXML.Xml_Pubblico_Macchina(TipoOperazione_Macchina,
                                             Mac_Cod,
                                             Class_Code,
                                             Mac_Des:=Mac_Des,
                                             Targa:=Targa,
                                             Telaio:=Telaio,
                                             Tipo:=Tipo,
                                             TitoloPossesso:=TitoloPossesso,
                                             Alimentazione_Cod:=Alimentazione_Cod,
                                             Ditta_Cod:=Ditta_Cod,
                                             Modello:=Modello,
                                             Tipo_Targa_Cod:=Tipo_Targa_Cod,
                                             Tipo_Trazione_Cod:=Tipo_Trazione_Cod,
                                             Validita_Inizio:=Validita_Inizio,
                                             Validita_Fine:=Validita_Fine,
                                             XmlDoc:=XmlDoc)

                XmlImpresa.AppendChild(XmlMacchina)

            Next

        End If

        If CreaCentro Then

            Dim SaCod As Integer = 0
            Dim SaNome As String = "01"
            Dim Fabbricato_Cod As Integer = 0
            Dim tipoOperazione_Fabbricato As enum_TipoOperazioneDB
            tipoOperazione_Fabbricato = enum_TipoOperazioneDB.Scrittura
            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                SaCod = objCentri.Esiste_Centro(Piva, "", 0, Nothing, enumSelezioneVariabile.Selezione_TabellaCompleta, "", objParametri_Server, SaNome)
                If objFabbricati.Esiste_Fabbricato_NelCentro(Piva, SaCod, Fabbricato_Cod, objParametri_Server) Then
                    'tipoOperazione_Fabbricato = enum_TipoOperazioneDB.Modifica
                    tipoOperazione_Fabbricato = enum_TipoOperazioneDB.Lettura
                Else
                    tipoOperazione_Fabbricato = enum_TipoOperazioneDB.Scrittura
                End If
            End If



            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione,
                                                            SaCod,
                                                            SaNome,
                                                            1,
                                                            "#", "#", "#", "#", "#", "#",
                                                            Indirizzo,
                                                            "#",
                                                            Cap,
                                                            Comune,
                                                            Sigla_Provincia,
                                                            "#", "#",
                                                            Istat_Comune,
                                                            Istat_Provincia,
                                                            "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                             "#",
                                                            XmlDoc)

            'Creo il fabbricato
            XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(tipoOperazione_Fabbricato,
                                                              CStr(Fabbricato_Cod),
                                                              "Magazzino n.01",
                                                              MAGAZZINO,
                                                              "#", "#", "#",
                                                              "#",
                                                              "#",
                                                              "#",
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              Indirizzo,
                                                              "#",
                                                              Cap,
                                                              Comune,
                                                              Sigla_Provincia,
                                                              "#", "#",
                                                              Istat_Comune,
                                                              Istat_Provincia,
                                                               "#",
                                                              XmlDoc)

            XmlCentro.AppendChild(XmlFabbricato)

            XmlImpresa.AppendChild(XmlCentro)

            Dim objImportazione As New SincroAnagrafeBA1.Importazione_Fascicolo

            Dim DtParticelle As New DataTable
            dtParticelleCreaStruttura(DtParticelle)

            Dim DataInizio As Date
            Dim DataFine As Date

            Popola_DtAppezzamenti_daFascicolo_2(Piva,
                                                fascicolo,
                                                ISWSTerritorioFS6,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                fascicolo.schedaValidazione)

            objImportazione.Crea_Stringa_Catasto(stringaAnagrafica,
                                                    "",
                                                    XmlUtente.OuterXml,
                                                    "",
                                                    Piva, SaCod,
                                                    DataInizio, DataFine, DataInizio.Year,
                                                    DtParticelle,
                                                    objParametri_Server, True, ASG_ProgressivoGIAS, False)



        End If

        'stringaAnagrafica = XmlUtente.OuterXml


        Dim AziendaCreata As Boolean
        Dim PianoCreato As Boolean

        Importa_Dati(ASG_Utente_Password, stringaAnagrafica, "", strErr, strRis, AziendaCreata, PianoCreato, objParametri_Server, objParametri_Utenti)

        Return AziendaCreata

    End Function

    Public Sub Importa_Dati(ByVal ASG_Utente_Password As String,
                            ByVal strDatiAnagrafe As String,
                            ByVal strDatiPianificazione As String,
                            ByRef strErr As String,
                            ByRef strRis As String,
                            ByRef AziendaCreata As Boolean,
                            ByRef PianoCreato As Boolean,
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

        Dim CodiceChiaveCliente As Integer

        Dim x As Integer
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Importa As New Ws_Importa_Gias.ImportaWS
        CodiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)


        Try

            Dim LinkWSImportaGIAS As String
            LinkWSImportaGIAS = objConfSiti.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)

            If LinkWSImportaGIAS <> "" Then

                Importa.Url = LinkWSImportaGIAS

                Importa.Timeout = System.Threading.Timeout.Infinite
                Descrizione = String.Empty

                Dim objcoreXML As New AgronicaCoreXML.XML_WS_Importa_Gias
                Dim strCredenziali As String
                strCredenziali = objcoreXML.Genera_Stringa_Credenziali(True, Nothing, objParametri_Server.UtenteUsername,
                                                                       ASG_Utente_Password,
                                                                       objParametri_Server.PivaSuperUser,
                                                                       True,
                                                                       "", "", "", "", "", "",
                                                                       objParametri_Server.StringaConnessione,
                                                                       objParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(strCredenziali,
                                                                                      strDatiAnagrafe,
                                                                                      CodiceChiaveCliente)


                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then

                        Descrizione &= "- " & XML_Risultato.GetAttribute("errore").ToString & "</br>"
                        strErr = Descrizione
                        AziendaCreata = False

                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                            AziendaCreata = True

                        Next

                    End If


                End If

                If strDatiPianificazione <> String.Empty Then

                    Descrizione &= "</br><b>Importazione pianificazione:" & "</b></br>"

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(strCredenziali,
                                                                                         strDatiPianificazione,
                                                                                         CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                        PianoCreato = True

                    Next

                End If

                strRis = Descrizione

            Else

                strErr = "Inserire l'indirizzo del Web Service Gias per continuare."


            End If

        Catch ex As Exception

            strErr = ex.Message

            AziendaCreata = False

        End Try


    End Sub

    Private Shared Sub dtParticelleCreaStruttura(ByRef DtParticelle As DataTable)

        DtParticelle.Columns.Add(New DataColumn("chiave", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("piva", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("appezza", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("programmazione_entita_cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("App_Nome", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("PROV", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("COM", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Com_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("possesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TitoloPossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("datepossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("inizio_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("fine_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("sup", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("supcondotta", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("catasto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("catasto_key", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("macrouso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_sup", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("utilizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("varieta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("utilizzo_sup", GetType(Double)))
        'DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Scarto", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("veg_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("cul_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grfi_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grva_des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("validita_fine", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("ribaltato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("movimentato", GetType(Integer)))

        'DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))

        'DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Lotto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Resa", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Num_Piante", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("TRA_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("SU_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Des", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Unita_Vitata", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Uso_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Occupazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Destinazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Qualita_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Gru_Cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Dpi_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Reg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("StatoImpianto_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("N", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("P", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("K", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_semina", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_Raccolta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_Fioritura", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente2", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente3", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente4", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Piano_Semina", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Contratto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("unito", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("frazionato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Fiscale_Tecnico", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("provenienza_fascicolo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_ribaltamento", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("datoGis", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("IAF", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Disciplinare", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Regolamento_Concimazione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Flag_PubblicoPrivato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_tr", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

    End Sub

    Private Shared Sub Popola_DtAppezzamenti_daFascicolo_2(ByVal Piva As String,
                                                           ByRef fascicolo As ISWSRespAnagFascicolo15,
                                                           ByRef ISWSTerritorioFS6 As List(Of ISWSTerritorioFS6),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByRef DataInizio As Date,
                                                           ByRef DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByRef Allegati_Documenti_Numero As String)

        Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Dim DR As DataRow

        Dim Sa_Cod As Integer = 0
        Dim Sa_Nome As String = ""
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
        Dim strVarieta As String = ""
        Dim TitoloPossesso As String
        Dim TitoloPossessoDes As String
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Inizio_Possesso_Old As Date
        Dim Fine_Possesso_Old As Date
        Dim Prov_Des As String = ""
        Dim Com_Des As String = ""

        Dim Veg_Cod_Agea As String = ""
        Dim Veg_Des_Agea As String = ""
        Dim Cul_Cod_Agea As String = ""
        Dim Cul_Des_Agea As String = ""
        Dim Uso_Cod_Agea As String = ""
        Dim Uso_Des_Agea As String = ""
        Dim Codice_Prodotto_Agea As String = ""
        Dim Occupazione_Cod_Agea As String = ""
        Dim Occupazione_Des_Agea As String = ""
        Dim Destinazione_Cod_Agea As String = ""
        Dim Destinazione_Des_Agea As String = ""
        Dim Qualita_Cod_Agea As String = ""
        Dim Qualita_Des_Agea As String = ""
        Dim Gru_Cod As Integer = 0
        Dim Cop_Cod As Integer
        Dim Cop_Des As String = ""
        Dim Lotto As String = ""
        Dim Resa As Double
        Dim Num_Piante As Integer
        Dim TRA_Fila As Double
        Dim SU_Fila As Double
        Dim Validita_Inizio_Impianto As String
        Dim TipoZona As String = ""
        Dim TipoZona_Des As String = ""
        Dim Unita_Vitata As String = "0"

        Dim Disciplinare As String = ""
        Dim Dpi_Cod As String = ""
        Dim Reg_Cod As String = ""
        Dim Regolamento_Concimazione_Cod As String = ""
        Dim Flag_PubblicoPrivato As String = ""
        Dim id_tr As String = ""
        Dim MetodoProduzione_Cod As Integer
        Dim MetodoProduzione_Des As String = ""

        Dim StatoImpianto_Cod As String = ""
        Dim N As Double? = Nothing
        Dim P As Double? = Nothing
        Dim K As Double? = Nothing
        Dim Data_Semina As String = ""
        Dim Data_Raccolta As String = ""
        Dim Data_Fioritura As String = ""
        Dim Coltura_Precedente As Integer = 0
        Dim Coltura_Precedente2 As Integer = 0
        Dim Coltura_Precedente3 As Integer = 0
        Dim Coltura_Precedente4 As Integer = 0
        Dim Piano_Semina As String = ""
        Dim Codice_Contratto As String = ""
        Dim unito As Integer = 0
        Dim frazionato As Integer = 0
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Dim Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri_Utenti.UtenteUsername, objParametri_Utenti)

        Dim sup_eleggibile As Double
        Dim sup_eleggibileOP As Double
        Dim data_inizio_appezzamento As Date
        Dim data_fine_appezzamento As Date

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        Dim ZoneCatasto As String = ""

        DataInizio = AGRODATAINIZIO
        DataFine = AGRODATAFINE
        Dim DataScheda As Date = AGRODATAINIZIO
        If fascicolo IsNot Nothing AndAlso
                fascicolo.dataValidazFascicolo IsNot Nothing Then
            DataScheda = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataValidazFascicolo))
        End If
        Dim Anno As Integer = Today.Year
        If DataScheda <> AGRODATAINIZIO Then
            Anno = DataScheda.Year
            If Anno > 2100 Then
                Anno = Date.Now.Year
            End If
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            DataScheda = New Date(Anno, DataScheda.Month, DataScheda.Day)
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If
        'Dim strValiditaInizio As String = String.Empty
        'strValiditaInizio = DataInizio.ToShortDateString
        'Dim strValiditaFine As String = String.Empty
        'strValiditaFine = DataFine.ToShortDateString

        trovaDisciplinare(Piva,
                          DataInizio,
                          DataFine,
                          Disciplinare,
                          Dpi_Cod,
                          Reg_Cod,
                          Regolamento_Concimazione_Cod,
                          Flag_PubblicoPrivato,
                          id_tr,
                          MetodoProduzione_Cod,
                          MetodoProduzione_Des,
                          objParametri_Server,
                          objParametri_Utenti)

        Dim stato_ribaltamento As String = "0"
        Dim provenienza_fascicolo As String = Allegati_Documenti_Numero

        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R

        If ISWSTerritorioFS6.Count > 0 Then

            ''leggo il sa_cod
            'Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            'Dim Dt_Centri As DataTable
            'Dt_Centri = objCentri.Leggi(Piva,
            '                            0,
            '                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
            '                            "", "",
            '                            objParametri_Server)

            'If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
            '    Sa_Cod = Dt_Centri.Rows(0).Item("sa_cod")
            'End If


            'leggo i comuni x avere le descrizioni poi
            Dim DT_Comuni As New DataTable
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


            For Each ISWSTerritorio In ISWSTerritorioFS6
                ZoneCatasto = ""
                'DR = DtParticelle.NewRow

                Prov = If(IsNothing(ISWSTerritorio.Provincia), "", ISWSTerritorio.Provincia)
                Com = If(IsNothing(ISWSTerritorio.Comune), "", ISWSTerritorio.Comune)
                Sezione = If(IsNothing(ISWSTerritorio.Sezione), "", ISWSTerritorio.Sezione)
                Foglio = If(IsNothing(ISWSTerritorio.Foglio), 0, ISWSTerritorio.Foglio)
                strNumero = If(IsNothing(ISWSTerritorio.Particella), 0, ISWSTerritorio.Particella)
                Subalterno = If(IsNothing(ISWSTerritorio.Subalterno), "", ISWSTerritorio.Subalterno)

                'ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des, ISNULL(ISTAT.LOCALITA,'') AS Com_Des, ")

                Dim DrCom() As DataRow = DT_Comuni.Select("PROV='" & Prov & "' AND COM='" & Com & "'")
                If DrCom IsNot Nothing AndAlso DrCom.Length > 0 Then
                    Prov_Des = DrCom(0).Item("COMUNI_PROV")
                    Com_Des = DrCom(0).Item("LOCALITA")
                End If

                'verifico cosa trovo nel campo particella (su Agea è una stringa)
                'se trovo dei numeri li metto in numero
                'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
                Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
                Numero = objCOre.Numero_from_Stringa(strNumero)
                If Numero = 0 Then
                    Numero = -1
                End If
                NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                If Subalterno = "" AndAlso NumeroStringa <> "" Then
                    Subalterno = Left(NumeroStringa, 3)
                End If

                'leggo il sa_cod
                Dim objCentrixPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentrixPart.LeggiJoinCentriIndirizzi2(0,
                                                                    CStr(Piva),
                                                                    0,
                                                                    0,
                                                                    CStr(Prov),
                                                                    CStr(Com),
                                                                    CStr(Sezione),
                                                                    CInt(Foglio),
                                                                    CInt(Numero),
                                                                    CStr(Subalterno),
                                                                    "",
                                                                    "",
                                                                    objParametri_Server)
                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    Sa_Cod = Dt_Centri.Rows(0).Item("sa_cod")
                    Sa_Nome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'ImportaCatastoFascicolo(ws_fasciResponse, Piva, fascicolo.CUAA, True)
                    'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                    Sa_Cod = 0
                    Sa_Nome = "01"
                End If

                Select Case ISWSTerritorio.codiceTipoConduzione
                    Case "1"
                        TitoloPossesso = 1 'Proprietà
                        TitoloPossessoDes = "Proprieta"
                    Case "2"
                        TitoloPossesso = 3 'Affitto con contratto
                        TitoloPossessoDes = "Affitto"
                    Case Else
                        TitoloPossesso = 0 'altro
                        TitoloPossessoDes = "Altro"
                End Select

                Inizio_Possesso = #1/1/1900#
                Fine_Possesso = #12/31/2100#
                Inizio_Possesso_Old = #1/1/1900#
                Fine_Possesso_Old = #12/31/2100#

                If ISWSTerritorio.DataInizioConduzione IsNot Nothing AndAlso ISWSTerritorio.DataInizioConduzione <> "" Then
                    Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataInizioConduzione))
                End If
                If ISWSTerritorio.DataFineConduzione IsNot Nothing AndAlso
                                    ISWSTerritorio.DataFineConduzione <> "" AndAlso
                                    ISWSTerritorio.DataFineConduzione <> "99991231" AndAlso
                                    ISWSTerritorio.DataFineConduzione <> "99990101" Then
                    Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataFineConduzione))
                End If

                supCatasto = CDbl(ISWSTerritorio.SuperficieCatastale) / 10000.0
                Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)

                supConduzione = CDbl(ISWSTerritorio.SuperficieCondotta) / 10000.0

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

                'Dim Eleggibilita As String
                Dim Qualita As String
                Dim Macrouso_Cod As String
                Dim Macrouso_Sup As Double
                Dim Specie_Cod As String
                Dim Varieta_Cod As String
                Dim Specie_Des_Agea As String
                Dim Varieta_Des_Agea As String
                Dim Utilizzo_Sup As Double
                'Dim HashMacrousi As New Hashtable
                strMacrousi = ""
                strUtilizzi = ""


                Select Case ISWSTerritorio.FlagIrrigua
                    Case ISWSTerritorioFS6FlagIrrigua.Item0 'Non Irrigua
                    Case ISWSTerritorioFS6FlagIrrigua.Item1 'Irrigua
                    Case ISWSTerritorioFS6FlagIrrigua.Item2 'Non Dichiarato
                End Select

                Select Case ISWSTerritorio.FlagTerrazzata
                    Case ISWSTerritorioFS6FlagTerrazzata.Item0 'Senza Terrazzamenti o Livellamenti
                    Case ISWSTerritorioFS6FlagTerrazzata.Item1 'Con Terrazzamenti
                    Case ISWSTerritorioFS6FlagTerrazzata.Item2 'Con Livellamenti
                    Case ISWSTerritorioFS6FlagTerrazzata.Item3 'Con Terrazzamenti e Livellamenti
                End Select

                Select Case ISWSTerritorio.RotazioneColtureOrtive
                    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item0 'Senza Rotazione Colturale
                    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item1 'Con ciclo Ortivo
                    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item2 'con ciclo Seminativo
                    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item3 'non dichiarato
                End Select

                If ISWSTerritorio.EffluentiZootecnici IsNot Nothing Then

                    Select Case ISWSTerritorio.EffluentiZootecnici
                        Case 0 'Senza affluetni zootecnici
                        Case 1 'Con affluetni zootecnici
                    End Select

                End If

                If ISWSTerritorio.SostanzePericolose IsNot Nothing Then

                    Select Case ISWSTerritorio.SostanzePericolose
                        Case 0 'Senza sostanze pericolose
                        Case 1 'Con sostanze pericolose
                    End Select

                End If

                If ISWSTerritorio.CasiParticolari IsNot Nothing Then

                    Select Case ISWSTerritorio.CasiParticolari.Value
                        Case CasiParticolari.Item1 'Riordino fondiario
                        Case CasiParticolari.Item2 'Zona militare o zona di confine soggetta a vincoli di sicurezza
                        Case CasiParticolari.Item3 'Uso civico
                        Case CasiParticolari.Item4 'Zona demaniale
                        Case CasiParticolari.Item5 'Particelle interessate da frazionamento in data successiva al 31.12.1998
                        Case CasiParticolari.Item6 'Stato estero
                        Case CasiParticolari.Item7 'Nuovo catasto edilizio urbano
                    End Select

                End If

                If ISWSTerritorio.FlagGiust IsNot Nothing Then

                    Select Case ISWSTerritorio.FlagGiust
                        Case 0 'Assenza di documentazione giustificativa
                        Case 1 'Presenza di documentazione giustificativa
                    End Select

                End If

                If ISWSTerritorio.CodiZVN IsNot Nothing Then

                    Select Case ISWSTerritorio.CodiZVN
                        Case "0" 'No
                            ZoneCatasto = ""
                        Case "1" 'Si
                            ZoneCatasto = "-17"
                    End Select

                End If

                If ISWSTerritorio.UtilizzoSuolo IsNot Nothing Then

                    For Each ISWSUtilizzoTerra In ISWSTerritorio.UtilizzoSuolo

                        Qualita = ISWSUtilizzoTerra.CodiceQualita

                        strMacrousi = ""
                        strUtilizzi = ""

                        If ISWSUtilizzoTerra.CodiceMacrouso IsNot Nothing Then
                            Macrouso_Cod = ISWSUtilizzoTerra.CodiceMacrouso
                        Else
                            Macrouso_Cod = "000"
                        End If
                        Macrouso_Sup = CDbl(ISWSUtilizzoTerra.SuperficieUtilizzata) / 10000.0

                        MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                        objLog.Scrivi_LOG(objParametri_Server, NomeRoutine, MsgOK)

                        'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                        'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                        Dim sMacrousoSup As String = ""
                        If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                            sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                        End If

                        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                        strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(ISWSUtilizzoTerra.CodiceMacrouso,
                                                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                     "", "",
                                                                                     objParametri_Server) '& sMacrousoSup



                        Unita_Vitata = "0"
                        frazionato = 0
                        unito = 0

                        Dim Veg_Cod As Integer = 0
                        Dim Cul_Cod As Integer = 0
                        Dim Grfi_Cod As Integer = 0
                        Dim Id_Cod As Integer = 0
                        Dim Grva_Cod As Integer = 0

                        '*******************************************************************************************************************
                        '******   UTILIZZO    **********************************************************************************
                        '*******************************************************************************************************************
                        If ISWSUtilizzoTerra IsNot Nothing Then

                            strUtilizzi = ""

                            Specie_Cod = "" 'ISWSUtilizzoTerra.CodiceProdotto
                            If ISWSUtilizzoTerra.CodiceVarieta IsNot Nothing Then
                                Varieta_Cod = ISWSUtilizzoTerra.CodiceVarieta
                            Else
                                Varieta_Cod = "000"
                            End If
                            Utilizzo_Sup = CDbl(ISWSUtilizzoTerra.SuperficieUtilizzata) / 10000.0




                            Specie_Des_Agea = ""
                            Varieta_Des_Agea = ""

                            Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R



                            Veg_Cod_Agea = ""
                            Veg_Des_Agea = ""
                            Cul_Cod_Agea = ""
                            Cul_Des_Agea = ""
                            Uso_Cod_Agea = ""
                            Uso_Des_Agea = ""
                            Codice_Prodotto_Agea = ""
                            Occupazione_Cod_Agea = ""
                            Occupazione_Des_Agea = ""
                            Destinazione_Cod_Agea = ""
                            Destinazione_Des_Agea = ""
                            Qualita_Cod_Agea = ""
                            Qualita_Des_Agea = ""
                            Gru_Cod = 0
                            Cop_Cod = 0
                            Cop_Des = ""
                            Lotto = ""
                            Resa = 0
                            Num_Piante = 0
                            TRA_Fila = 0
                            SU_Fila = 0
                            Validita_Inizio_Impianto = ""
                            TipoZona = "n"
                            TipoZona_Des = "Non Vulnerabile"

                            Unita_Vitata = "0"
                            frazionato = 0
                            unito = 0

                            StatoImpianto_Cod = ""
                            N = Nothing
                            P = Nothing
                            K = Nothing
                            Data_Semina = ""
                            Data_Raccolta = ""
                            Data_Fioritura = ""
                            Coltura_Precedente = 0
                            Coltura_Precedente2 = 0
                            Coltura_Precedente3 = 0
                            Coltura_Precedente4 = 0
                            Piano_Semina = ""
                            Codice_Contratto = ""

                            Dim Veg_Des As String = ""
                            Dim Cul_Des As String = ""
                            Dim Grfi_Des As String = ""
                            Dim Grva_Des As String = ""

                            Uso_Cod_Agea = ISWSUtilizzoTerra.CodiceUso
                            Codice_Prodotto_Agea = ISWSUtilizzoTerra.CodiceProdotto
                            Occupazione_Cod_Agea = ISWSUtilizzoTerra.CodiceOccupazioneSuolo
                            Destinazione_Cod_Agea = ISWSUtilizzoTerra.CodiceDestinazioneUso
                            Qualita_Cod_Agea = ISWSUtilizzoTerra.CodiceQualita
                            Gru_Cod = 0

                            sup_eleggibile = 0
                            sup_eleggibileOP = 0
                            Dim LogCodificheMancantiSpecie As String = ""
                            Dim LogCodificheMancantiVarieta As String = ""

                            objUtilizzi.Specie_e_Varieta_Gias_Da_Agea_Da5Parametri(
                                                                  LogCodificheMancantiSpecie,
                                                                  LogCodificheMancantiVarieta,
                                                                  Varieta_Cod,
                                                                  Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                  Specie_Des_Agea, Varieta_Des_Agea,
                                                                  "",
                                                                  "",
                                                                  DataInizio,
                                                                  Uso_Cod_Agea,
                                                                  Occupazione_Cod_Agea,
                                                                  Destinazione_Cod_Agea,
                                                                  Qualita_Cod_Agea,
                                                                  objParametri_Server,
                                                                  Gru_Cod)

                            'objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(
                            '                                      LogCodificheMancantiSpecie,
                            '                                      LogCodificheMancantiVarieta,
                            '                                      Specie_Cod, Varieta_Cod,
                            '                                      Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                            '                                      Specie_Des_Agea, Varieta_Des_Agea,
                            '                                      "",
                            '                                      "",
                            '                                      DataInizio,
                            '                                      Uso_Cod_Agea,
                            '                                      Occupazione_Cod_Agea,
                            '                                      Destinazione_Cod_Agea,
                            '                                      Qualita_Cod_Agea,
                            '                                      objParametri_Server,
                            '                                      Gru_Cod)

                            'Specie_Des_Agea &= " (Cod." & Specie_Cod & ")"

                            If (Veg_Cod <> 0 AndAlso Cul_Cod = 0) Then
                                Dim ObjVarietaAltre As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                Cul_Cod = ObjVarietaAltre.VarietaAltre(Veg_Cod, objParametri_Server)
                            End If

                            If Veg_Cod <> 0 Then
                                Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R
                                Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)
                            End If

                            Dim sUtilizzoSup As String = ""
                            If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                                sUtilizzoSup = " (" & Utilizzo_Sup & " Ha)"
                            End If

                            strUtilizzi = Specie_Des_Agea & " (Cod." & Specie_Cod & ")"
                            If Varieta_Des_Agea <> "" Then
                                strVarieta = Varieta_Des_Agea & " (Cod." & Varieta_Cod & ")"
                            Else
                                strVarieta = "Non Definita (Cod." & Varieta_Cod & ")"
                            End If

                            Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            End If


                            DR = DtParticelle.NewRow

                            DR.Item("PROV") = Prov
                            DR.Item("COM") = Com
                            DR.Item("Prov_Des") = Prov_Des
                            DR.Item("Com_Des") = Com_Des
                            DR.Item("Sezione") = Sezione
                            DR.Item("Foglio") = Foglio
                            DR.Item("Numero") = Numero
                            DR.Item("Subalterno") = Subalterno

                            DR.Item("Catasto") = Prov_Des & ":" &
                                                     Com_Des & ":_" &
                                                            Sezione & ":_" &
                                                            Foglio.ToString & ":_" &
                                                            Numero.ToString & ":_" &
                                                            Subalterno

                            DR.Item("possesso") = TitoloPossessoDes
                            DR.Item("TitoloPossesso") = TitoloPossesso
                            DR.Item("inizio_possesso") = Inizio_Possesso
                            DR.Item("fine_possesso") = Fine_Possesso
                            DR.Item("datepossesso") = If(ISWSTerritorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataInizioConduzione), "Dal ...") &
                                                                            If(ISWSTerritorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataFineConduzione), " al ...")
                            supCatasto = CDbl(ISWSTerritorio.SuperficieCatastale) / 10000.0
                            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                            DR.Item("sup") = Format(supCatasto, "0.0000")
                            supConduzione = CDbl(ISWSTerritorio.SuperficieCondotta) / 10000.0
                            DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                            DR.Item("Catasto_Key") = Prov & "_" &
                                                            Com & "_" &
                                                            strSezione & "_" &
                                                            Foglio.ToString & "_" &
                                                            Numero.ToString & "_" &
                                                            strSubalterno & ":" &
                                                            Utilizzo_Sup.ToString

                            DR.Item("macrouso_cod") = Macrouso_Cod
                            DR.Item("macrouso_sup") = Macrouso_Sup
                            DR.Item("macrouso") = strMacrousi

                            DR.Item("utilizzo") = strUtilizzi
                            DR.Item("varieta") = strVarieta
                            DR.Item("utilizzo_sup") = Utilizzo_Sup
                            DR.Item("Veg_Cod_Agea") = Specie_Cod
                            DR.Item("Cul_Cod_Agea") = Varieta_Cod
                            DR.Item("Veg_Cod") = Veg_Cod & "|" & Id_Cod
                            DR.Item("Cul_Cod") = Cul_Cod
                            DR.Item("Grfi_Cod") = Grfi_Cod
                            DR.Item("Grva_Cod") = Grva_Cod
                            DR.Item("Id_Cod") = Id_Cod

                            'DRUDI
                            'Recupero descrizione specie, varietà...

                            If Veg_Cod <> 0 Then
                                Dim objSpec = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                                Dim dtSpecieVegetali As DataTable = objSpec.Leggi(Veg_Cod, 0, "", "", 0, "", "", objParametri_Server)
                                If Not IsNothing(dtSpecieVegetali) AndAlso dtSpecieVegetali.Rows.Count = 1 Then
                                    Veg_Des = dtSpecieVegetali.Rows(0).Item("Veg_Des")
                                End If
                            End If

                            If Id_Cod <> 0 Then
                                Dim objCodAna = New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
                                Dim dtCodiciAna As DataTable = objCodAna.Leggi(Id_Cod, "", "", "", objParametri_Server)
                                If Not IsNothing(dtCodiciAna) AndAlso dtCodiciAna.Rows.Count = 1 Then
                                    Veg_Des = dtCodiciAna.Rows(0).Item("descrizione")
                                End If
                            End If

                            If Cul_Cod <> 0 Then
                                Dim objCulti As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                Dim dtCultivar As DataTable = objCulti.Leggi(Cul_Cod, Veg_Cod, "", 0, "", "", objParametri_Server)
                                If Not IsNothing(dtCultivar) AndAlso dtCultivar.Rows.Count = 1 Then
                                    Cul_Des = dtCultivar.Rows(0).Item("Cul_Des")
                                End If
                            End If

                            If Grfi_Cod <> 0 Then
                                Dim objGrfi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
                                Dim dtGruppoFinalita As DataTable = objGrfi.Leggi(Grfi_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                If Not IsNothing(dtGruppoFinalita) AndAlso dtGruppoFinalita.Rows.Count = 1 Then
                                    Grfi_Des = dtGruppoFinalita.Rows(0).Item("Grfi_Des")
                                End If
                            End If

                            If Grva_Cod <> 0 Then
                                Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
                                Dim dtGruppoVarietale As DataTable = objGrva.Leggi(Veg_Cod, Grva_Cod, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)
                                If Not IsNothing(dtGruppoVarietale) AndAlso dtGruppoVarietale.Rows.Count = 1 Then
                                    Grva_Des = dtGruppoVarietale.Rows(0).Item("Grva_Des")
                                End If
                            End If


                            data_inizio_appezzamento = DataInizio
                            If Not IsNothing(ISWSUtilizzoTerra.DataInizioUtilizzo) AndAlso ISWSUtilizzoTerra.DataInizioUtilizzo <> "" Then
                                data_inizio_appezzamento = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSUtilizzoTerra.DataInizioUtilizzo))
                            End If

                            data_fine_appezzamento = DataFine
                            If Not IsNothing(ISWSUtilizzoTerra.DataFineUtilizzo) AndAlso ISWSUtilizzoTerra.DataFineUtilizzo <> "" Then
                                data_fine_appezzamento = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSUtilizzoTerra.DataFineUtilizzo))
                            End If

                            If ISWSUtilizzoTerra.NumeroPianteSpecified AndAlso IsNumeric(ISWSUtilizzoTerra.NumeroPiante) Then
                                Num_Piante = ISWSUtilizzoTerra.NumeroPiante
                            End If

                            If ISWSUtilizzoTerra.SestoImpiantoTraFile IsNot Nothing AndAlso ISWSUtilizzoTerra.SestoImpiantoTraFile <> "" AndAlso IsNumeric(ISWSUtilizzoTerra.SestoImpiantoTraFile) Then
                                TRA_Fila = CInt(ISWSUtilizzoTerra.SestoImpiantoTraFile)
                            End If

                            If ISWSUtilizzoTerra.SestoImpiantoSuFila IsNot Nothing AndAlso ISWSUtilizzoTerra.SestoImpiantoSuFila <> "" AndAlso IsNumeric(ISWSUtilizzoTerra.SestoImpiantoSuFila) Then
                                SU_Fila = CInt(ISWSUtilizzoTerra.SestoImpiantoSuFila)
                            End If

                            If ISWSUtilizzoTerra.AnnoImpianto IsNot Nothing AndAlso ISWSUtilizzoTerra.AnnoImpianto <> "" AndAlso IsNumeric(ISWSUtilizzoTerra.AnnoImpianto) Then
                                Validita_Inizio_Impianto = "01/01/" & ISWSUtilizzoTerra.AnnoImpianto
                            End If



                            'Dati Aggiuntivi fascicolo AGEA
                            If ISWSUtilizzoTerra.SuperficieEleggibileSpecified AndAlso IsNumeric(ISWSUtilizzoTerra.SuperficieEleggibile) Then
                                sup_eleggibile = CDbl(ISWSUtilizzoTerra.SuperficieEleggibile) / 10000
                            End If

                            If ISWSUtilizzoTerra.CodiceColtivazioneBiologicaSpecified Then

                                Select Case ISWSUtilizzoTerra.CodiceColtivazioneBiologica
                                    Case 0 'Non Biologico
                                    Case 1 'Biologico
                                End Select

                            End If

                            If ISWSUtilizzoTerra.CodiceTipoSeminaSpecified Then

                                Select Case ISWSUtilizzoTerra.CodiceTipoSemina
                                    Case 1 'Tradizionale
                                    Case 2 'Su sodo
                                    Case 3 'Minimum tillage
                                    Case 4 'Pratiche equivalenti
                                End Select

                            End If

                            If ISWSUtilizzoTerra.CodiceTipoIrrigazioneSpecified Then

                                Select Case ISWSUtilizzoTerra.CodiceTipoIrrigazione
                                    Case 1 'Sommersione
                                    Case 2 'Scorrimento
                                    Case 3 'Aspersione o a pioggia
                                    Case 4 'Microportate o a goccia
                                    Case 5 'Subirrigazione
                                End Select

                            End If

                            If ISWSUtilizzoTerra.CodiceProtezioneColtureSpecified Then

                                Select Case ISWSUtilizzoTerra.CodiceProtezioneColture
                                    Case 1 'Reti antigrandine
                                    Case 2 'Reti antiacqua
                                    Case 3 'Serre e tunnel fissi
                                    Case 4 'Ombrai
                                    Case 5 'Impianti antibrina
                                End Select

                            End If

                            If ISWSUtilizzoTerra.CodiceFaseAllevamentoSpecified Then

                                Select Case ISWSUtilizzoTerra.CodiceFaseAllevamento
                                    Case 1 'Produttivo
                                    Case 2 'Non produttivo
                                End Select

                            End If

                            If ISWSUtilizzoTerra.CodiceFormaAllevamentoSpecified Then

                                'tabella di Transcodifica?

                            End If

                            If ISWSUtilizzoTerra.FlagColtPrincipaleSpecified Then

                                Select Case ISWSUtilizzoTerra.FlagColtPrincipale
                                    Case 0 'Non è coltura principale (indica che non è varietà prevalente per olivo)
                                    Case 1 'Coltura principale (indica varietà prevalente per olivo)
                                End Select

                            End If

                            If ISWSUtilizzoTerra.MantPratiPermanenteSpecified Then

                                Select Case ISWSUtilizzoTerra.MantPratiPermanente
                                    Case 1 'Pascolamento con animali propri
                                    Case 2 'Pascolamento con animali terzi
                                    Case 3 'Sfalcio manuale
                                    Case 4 'Sfalcio meccanizzato
                                    Case 5 'Pratiche colturali volte al miglioramento
                                    Case 6 'Sfalcio con cadenza biennale
                                    Case 7 'Pascolamento e sfalcio
                                    Case 8 'Nessuna pratica
                                    'Case 9 '
                                    Case 10
                                        'Pratica stabilita nell'ambito delle misure di conservazione 
                                        'o dei piani di gestione 
                                        'prescritti dagli enti dei siti 
                                        'di importanza comunitaria (SIC) e 
                                        'delle zone di protezione speciale (ZPS)
                                End Select

                            End If

                            If ISWSUtilizzoTerra.MantenSupAgricolaSpecified Then

                                Select Case ISWSUtilizzoTerra.MantenSupAgricola

                                    Case 1 'Nessuna pratica

                                    Case 2 'Pratica ordinaria

                                End Select

                            End If

                            If ISWSUtilizzoTerra.SuperficieEleggibileOPSpecified AndAlso IsNumeric(ISWSUtilizzoTerra.SuperficieEleggibileOP) Then
                                sup_eleggibileOP = CDbl(ISWSUtilizzoTerra.SuperficieEleggibileOP) / 10000
                            End If

                            If ISWSUtilizzoTerra.TipoImpiantoSpecified Then

                                Select Case ISWSUtilizzoTerra.TipoImpianto

                                    Case 1 'Regolare

                                    Case 2 'Irregolare

                                End Select

                            End If

                            If ISWSUtilizzoTerra.TipoCertificazioneSpecified Then

                                Select Case ISWSUtilizzoTerra.TipoCertificazione

                                    Case 0 'Nessuna

                                    Case 1 'DOP

                                    Case 2 'IGP

                                End Select

                            End If

                            If ISWSUtilizzoTerra.MenzioneSpecified Then
                                ' Transcodifica?
                            End If

                            If ISWSUtilizzoTerra.TipoUtilizzoOlivoSpecified Then

                                Select Case ISWSUtilizzoTerra.TipoUtilizzoOlivo
                                    Case 0 'Promiscuo
                                    Case 1 'Specializzato
                                End Select

                            End If

                            If ISWSUtilizzoTerra.AltitudineSpecified Then

                            End If

                            If ISWSUtilizzoTerra.FlagProduzioneIntegrataSpecified Then

                                Select Case ISWSUtilizzoTerra.FlagProduzioneIntegrata
                                    Case 0
                                    Case 1
                                End Select

                            End If

                            DR.Item("veg_des") = Veg_Des
                            DR.Item("cul_des") = Cul_Des
                            DR.Item("grfi_des") = Grfi_Des
                            DR.Item("grva_des") = Grva_Des

                            DR.Item("Scarto") = 0

                            DR.Item("validita_inizio") = data_inizio_appezzamento.ToShortDateString
                            DR.Item("validita_fine") = data_fine_appezzamento.ToShortDateString

                            DR.Item("Sa_Nome") = Sa_Nome
                            DR.Item("App_Nome") = N_App.ToString
                            N_App += 1

                            DR.Item("piva") = Piva
                            DR.Item("sa_cod") = Sa_Cod
                            DR.Item("appezza") = appezza
                            appezza -= 1

                            DR.Item("programmazione_entita_cod") = 0

                            DR.Item("ribaltato") = 0
                            DR.Item("movimentato") = 0

                            DR.Item("stato_ribaltamento") = 0

                            DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                            DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                            DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                            DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                            DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                            DR.Item("Veg_Des_Agea") = Veg_Des_Agea
                            DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                            DR.Item("Cul_Des_Agea") = Cul_Des_Agea
                            DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                            DR.Item("Uso_Des_Agea") = Uso_Des_Agea
                            DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                            DR.Item("Occupazione_Des_Agea") = Occupazione_Des_Agea
                            DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                            DR.Item("Destinazione_Des_Agea") = Destinazione_Des_Agea
                            DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea
                            DR.Item("Qualita_Des_Agea") = Qualita_Des_Agea
                            DR.Item("Gru_Cod") = Gru_Cod
                            DR.Item("Cop_Cod") = Cop_Cod
                            DR.Item("Cop_Des") = Cop_Des
                            DR.Item("Lotto") = Lotto
                            DR.Item("Resa") = Resa
                            DR.Item("Num_Piante") = Num_Piante
                            DR.Item("TRA_Fila") = TRA_Fila
                            DR.Item("SU_Fila") = SU_Fila
                            DR.Item("Validita_Inizio_Impianto") = Validita_Inizio_Impianto
                            DR.Item("TipoZona") = TipoZona
                            DR.Item("TipoZona_Des") = TipoZona_Des
                            DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
                            DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des
                            DR.Item("Unita_Vitata") = Unita_Vitata

                            DR.Item("Dpi_Cod") = Dpi_Cod
                            DR.Item("Reg_Cod") = Reg_Cod
                            DR.Item("StatoImpianto_Cod") = StatoImpianto_Cod
                            DR.Item("N") = N
                            DR.Item("P") = P
                            DR.Item("K") = K
                            DR.Item("Data_Semina") = Data_Semina
                            DR.Item("Data_Raccolta") = Data_Raccolta
                            DR.Item("Data_Fioritura") = Data_Fioritura
                            DR.Item("Coltura_Precedente") = Coltura_Precedente
                            DR.Item("Coltura_Precedente2") = Coltura_Precedente2
                            DR.Item("Coltura_Precedente3") = Coltura_Precedente3
                            DR.Item("Coltura_Precedente4") = Coltura_Precedente4
                            DR.Item("Piano_Semina") = Piano_Semina
                            DR.Item("Codice_Contratto") = Codice_Contratto
                            DR.Item("unito") = unito
                            DR.Item("frazionato") = frazionato
                            DR.Item("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico

                            DR.Item("stato_ribaltamento") = stato_ribaltamento
                            DR.Item("provenienza_fascicolo") = provenienza_fascicolo
                            DR.Item("Regolamento_Concimazione_Cod") = Regolamento_Concimazione_Cod
                            DR.Item("Flag_PubblicoPrivato") = Flag_PubblicoPrivato
                            DR.Item("id_tr") = id_tr

                            If Reg_Cod <> "4" Then
                                DR.Item("Disciplinare") = Dpi_Cod & "/" & Flag_PubblicoPrivato & "/" & Regolamento_Concimazione_Cod & "/" & id_tr
                            Else
                                DR.Item("Disciplinare") = -2
                            End If


                            DR.Item("ZoneCatasto") = ZoneCatasto
                            'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                            '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")

                            DR.Item("chiave") = chiave
                            chiave += 1

                            DtParticelle.Rows.Add(DR)

                        Else
                            Utilizzo_Sup = Macrouso_Sup

                            Gru_Cod = 0
                            Cop_Cod = 0
                            Cop_Des = ""
                            Lotto = ""
                            Resa = 0
                            Num_Piante = 0
                            TRA_Fila = 0
                            SU_Fila = 0
                            Validita_Inizio_Impianto = ""
                            TipoZona = "n"
                            TipoZona_Des = "Non Vulnerabile"
                            Unita_Vitata = "0"
                            frazionato = 0
                            unito = 0

                            StatoImpianto_Cod = ""
                            N = Nothing
                            P = Nothing
                            K = Nothing
                            Data_Semina = ""
                            Data_Raccolta = ""
                            Data_Fioritura = ""
                            Coltura_Precedente = 0
                            Coltura_Precedente2 = 0
                            Coltura_Precedente3 = 0
                            Coltura_Precedente4 = 0
                            Piano_Semina = ""
                            Codice_Contratto = ""

                            'Macrouso_Cod
                            Veg_Cod = 0
                            Cul_Cod = 0
                            Grfi_Cod = 0
                            Id_Cod = 0
                            Grva_Cod = 0

                            Veg_Cod_Agea = ""
                            Veg_Des_Agea = ""
                            Cul_Cod_Agea = ""
                            Cul_Des_Agea = ""
                            Uso_Cod_Agea = ""
                            Uso_Des_Agea = ""
                            Occupazione_Cod_Agea = ""
                            Occupazione_Des_Agea = ""
                            Destinazione_Cod_Agea = ""
                            Destinazione_Des_Agea = ""
                            Qualita_Cod_Agea = ""
                            Qualita_Des_Agea = ""
                            Gru_Cod = 0

                            Dim Veg_Des As String = ""
                            Dim Cul_Des As String = ""

                            Uso_Cod_Agea = ""
                            Occupazione_Cod_Agea = ""
                            Destinazione_Cod_Agea = ""
                            Qualita_Cod_Agea = ""
                            Gru_Cod = 0
                            Dim LogCodificheMancantiSpecie As String = ""
                            Dim LogCodificheMancantiVarieta As String = ""

                            Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

                            objUtilizzi.Specie_e_Varieta_Gias_Da_Macrouso_Agea(LogCodificheMancantiSpecie, LogCodificheMancantiVarieta,
                                                                                   Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea, Veg_Cod, Cul_Cod,
                                                                                   Grfi_Cod, Grva_Cod, Id_Cod, Uso_Cod_Agea, Occupazione_Cod_Agea,
                                                                                   Destinazione_Cod_Agea, Qualita_Cod_Agea, Veg_Des, Cul_Des, objParametri_Server)

                            If Veg_Cod <> 0 Then
                                Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R
                                Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)
                            End If

                            Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            End If

                            DR = DtParticelle.NewRow

                            DR.Item("PROV") = Prov
                            DR.Item("COM") = Com
                            DR.Item("Prov_Des") = Prov_Des
                            DR.Item("Com_Des") = Com_Des
                            DR.Item("Sezione") = Sezione
                            DR.Item("Foglio") = Foglio
                            DR.Item("Numero") = Numero
                            DR.Item("Subalterno") = Subalterno


                            DR.Item("Catasto") = Prov_Des & ":" &
                                                         Com_Des & ":_" &
                                                                Sezione & ":_" &
                                                                Foglio.ToString & ":_" &
                                                                Numero.ToString & ":_" &
                                                                Subalterno

                            DR.Item("possesso") = TitoloPossessoDes
                            DR.Item("TitoloPossesso") = TitoloPossesso
                            DR.Item("inizio_possesso") = Inizio_Possesso
                            DR.Item("fine_possesso") = Fine_Possesso
                            DR.Item("datepossesso") = If(ISWSTerritorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataInizioConduzione), "Dal ...") &
                                                                                If(ISWSTerritorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataFineConduzione), " al ...")
                            supCatasto = CDbl(ISWSTerritorio.SuperficieCatastale) / 10000.0
                            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                            DR.Item("sup") = Format(supCatasto, "0.0000")
                            supConduzione = CDbl(ISWSTerritorio.SuperficieCondotta) / 10000.0
                            DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                            DR.Item("Catasto_Key") = Prov & "_" &
                                                                Com & "_" &
                                                                strSezione & "_" &
                                                                Foglio.ToString & "_" &
                                                                Numero.ToString & "_" &
                                                                strSubalterno & ":" &
                                                                Utilizzo_Sup.ToString


                            DR.Item("macrouso_cod") = Macrouso_Cod
                            DR.Item("macrouso_sup") = Macrouso_Sup
                            DR.Item("macrouso") = strMacrousi

                            DR.Item("utilizzo") = ""
                            DR.Item("varieta") = ""
                            DR.Item("utilizzo_sup") = Utilizzo_Sup
                            DR.Item("Veg_Cod_Agea") = ""
                            DR.Item("Cul_Cod_Agea") = ""
                            DR.Item("Veg_Cod") = "0|0" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
                            DR.Item("Cul_Cod") = 0 'Cul_Cod
                            DR.Item("Grfi_Cod") = 0 'Grfi_Cod
                            DR.Item("Grva_Cod") = 0 'Grva_Cod
                            DR.Item("Id_Cod") = 0 'Id_Cod

                            DR.Item("veg_des") = "Utilizzo non specificato" 'Veg_Des
                            DR.Item("cul_des") = "" 'Cul_Des
                            DR.Item("grfi_des") = ""
                            DR.Item("grva_des") = ""

                            DR.Item("Scarto") = 0

                            DR.Item("validita_inizio") = DataInizio.ToShortDateString
                            DR.Item("validita_fine") = DataFine.ToShortDateString

                            DR.Item("Sa_Nome") = Sa_Nome
                            DR.Item("App_Nome") = N_App.ToString
                            N_App += 1

                            DR.Item("piva") = Piva
                            DR.Item("sa_cod") = Sa_Cod
                            DR.Item("appezza") = appezza
                            appezza -= 1

                            DR.Item("programmazione_entita_cod") = 0

                            DR.Item("ribaltato") = 0
                            DR.Item("movimentato") = 0

                            DR.Item("stato_ribaltamento") = 0

                            DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                            DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                            DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                            DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                            DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                            DR.Item("Veg_Des_Agea") = Veg_Des_Agea
                            DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                            DR.Item("Cul_Des_Agea") = Cul_Des_Agea
                            DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                            DR.Item("Uso_Des_Agea") = Uso_Des_Agea
                            DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                            DR.Item("Occupazione_Des_Agea") = Occupazione_Des_Agea
                            DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                            DR.Item("Destinazione_Des_Agea") = Destinazione_Des_Agea
                            DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea
                            DR.Item("Qualita_Des_Agea") = Qualita_Des_Agea
                            DR.Item("Gru_Cod") = Gru_Cod
                            DR.Item("Cop_Cod") = Cop_Cod
                            DR.Item("Cop_Des") = Cop_Des
                            DR.Item("Lotto") = Lotto
                            DR.Item("Resa") = Resa
                            DR.Item("Num_Piante") = Num_Piante
                            DR.Item("TRA_Fila") = TRA_Fila
                            DR.Item("SU_Fila") = SU_Fila
                            DR.Item("Validita_Inizio_Impianto") = Validita_Inizio_Impianto
                            DR.Item("TipoZona") = TipoZona
                            DR.Item("TipoZona_Des") = TipoZona_Des
                            DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
                            DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des
                            DR.Item("Unita_Vitata") = Unita_Vitata


                            DR.Item("Dpi_Cod") = Dpi_Cod
                            DR.Item("Reg_Cod") = Reg_Cod
                            DR.Item("StatoImpianto_Cod") = StatoImpianto_Cod
                            DR.Item("N") = N
                            DR.Item("P") = P
                            DR.Item("K") = K
                            DR.Item("Data_Semina") = Data_Semina
                            DR.Item("Data_Raccolta") = Data_Raccolta
                            DR.Item("Data_Fioritura") = Data_Fioritura
                            DR.Item("Coltura_Precedente") = Coltura_Precedente
                            DR.Item("Coltura_Precedente2") = Coltura_Precedente2
                            DR.Item("Coltura_Precedente3") = Coltura_Precedente3
                            DR.Item("Coltura_Precedente4") = Coltura_Precedente4
                            DR.Item("Piano_Semina") = Piano_Semina
                            DR.Item("Codice_Contratto") = Codice_Contratto
                            DR.Item("unito") = unito
                            DR.Item("frazionato") = frazionato
                            DR.Item("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico
                            DR.Item("provenienza_fascicolo") = provenienza_fascicolo

                            DR.Item("Regolamento_Concimazione_Cod") = Regolamento_Concimazione_Cod
                            DR.Item("Flag_PubblicoPrivato") = Flag_PubblicoPrivato
                            DR.Item("id_tr") = id_tr

                            If Reg_Cod <> "4" Then
                                DR.Item("Disciplinare") = Dpi_Cod & "/" & Flag_PubblicoPrivato & "/" & Regolamento_Concimazione_Cod & "/" & id_tr
                            Else
                                DR.Item("Disciplinare") = -2
                            End If

                            DR.Item("ZoneCatasto") = ZoneCatasto

                            DR.Item("chiave") = chiave
                            chiave += 1

                            DtParticelle.Rows.Add(DR)

                        End If

                        'End If

                    Next

                Else

                    Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                        TipoZona = "v"
                        TipoZona_Des = "Vulnerabile"
                    End If

                    DR = DtParticelle.NewRow

                    DR.Item("PROV") = Prov
                    DR.Item("COM") = Com
                    DR.Item("Prov_Des") = Prov_Des
                    DR.Item("Com_Des") = Com_Des
                    DR.Item("Sezione") = Sezione
                    DR.Item("Foglio") = Foglio
                    DR.Item("Numero") = Numero
                    DR.Item("Subalterno") = Subalterno

                    DR.Item("Catasto") = Prov_Des & ":" &
                                         Com_Des & ":_" &
                                            DR.Item("Sezione") & ":_" &
                                            DR.Item("Foglio").ToString & ":_" &
                                            DR.Item("Numero").ToString & ":_" &
                                            DR.Item("Subalterno")

                    DR.Item("possesso") = TitoloPossessoDes
                    DR.Item("TitoloPossesso") = TitoloPossesso
                    DR.Item("inizio_possesso") = Inizio_Possesso
                    DR.Item("fine_possesso") = Fine_Possesso
                    DR.Item("datepossesso") = If(ISWSTerritorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataInizioConduzione), "Dal ...") &
                                                                If(ISWSTerritorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSTerritorio.DataFineConduzione), " al ...")
                    DR.Item("sup") = Format(supCatasto, "0.0000")
                    DR.Item("supcondotta") = Format(supConduzione, "0.0000")

                    DR.Item("Catasto_Key") = DR.Item("PROV") & "_" &
                                            DR.Item("COM") & "_" &
                                            DR.Item("Sezione") & "_" &
                                            DR.Item("Foglio").ToString & "_" &
                                            DR.Item("Numero").ToString & "_" &
                                            DR.Item("Subalterno") & ":" &
                                            Utilizzo_Sup.ToString

                    DR.Item("macrouso_cod") = ""
                    DR.Item("macrouso_sup") = 0
                    DR.Item("macrouso") = ""

                    DR.Item("utilizzo") = ""
                    DR.Item("varieta") = ""
                    DR.Item("utilizzo_sup") = 0
                    DR.Item("Veg_Cod_Agea") = ""
                    DR.Item("Cul_Cod_Agea") = ""
                    DR.Item("Veg_Cod") = "0|0"
                    DR.Item("Cul_Cod") = 0
                    DR.Item("Grfi_Cod") = 0
                    DR.Item("Grva_Cod") = 0
                    DR.Item("Id_Cod") = 0

                    DR.Item("veg_des") = ""
                    DR.Item("cul_des") = ""
                    DR.Item("grfi_des") = ""
                    DR.Item("grva_des") = ""

                    DR.Item("Scarto") = 1

                    DR.Item("validita_inizio") = DataInizio.ToShortDateString
                    DR.Item("validita_fine") = DataFine.ToShortDateString

                    DR.Item("Sa_Nome") = Sa_Nome
                    DR.Item("App_Nome") = N_App.ToString
                    N_App += 1

                    DR.Item("piva") = Piva
                    DR.Item("sa_cod") = Sa_Cod
                    DR.Item("appezza") = appezza
                    appezza -= 1

                    DR.Item("programmazione_entita_cod") = 0

                    DR.Item("ribaltato") = 0
                    DR.Item("movimentato") = 0

                    DR.Item("stato_ribaltamento") = 0

                    DR.Item("Uso_Cod_Agea") = ""
                    DR.Item("Occupazione_Cod_Agea") = ""
                    DR.Item("Destinazione_Cod_Agea") = ""
                    DR.Item("Qualita_Cod_Agea") = ""

                    DR.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                    DR.Item("Veg_Des_Agea") = Veg_Des_Agea
                    DR.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                    DR.Item("Cul_Des_Agea") = Cul_Des_Agea
                    DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                    DR.Item("Uso_Des_Agea") = Uso_Des_Agea
                    DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                    DR.Item("Occupazione_Des_Agea") = Occupazione_Des_Agea
                    DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                    DR.Item("Destinazione_Des_Agea") = Destinazione_Des_Agea
                    DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea
                    DR.Item("Qualita_Des_Agea") = Qualita_Des_Agea
                    DR.Item("Gru_Cod") = Gru_Cod
                    DR.Item("Cop_Cod") = 0
                    DR.Item("Cop_Des") = Cop_Des
                    DR.Item("Lotto") = Lotto
                    DR.Item("Resa") = Resa
                    DR.Item("Num_Piante") = Num_Piante
                    DR.Item("TRA_Fila") = TRA_Fila
                    DR.Item("SU_Fila") = SU_Fila
                    DR.Item("Validita_Inizio_Impianto") = Validita_Inizio_Impianto
                    DR.Item("TipoZona") = TipoZona
                    DR.Item("TipoZona_Des") = TipoZona_Des
                    DR.Item("MetodoProduzione_Cod") = MetodoProduzione_Cod
                    DR.Item("MetodoProduzione_Des") = MetodoProduzione_Des
                    DR.Item("Unita_Vitata") = Unita_Vitata

                    DR.Item("Dpi_Cod") = Dpi_Cod
                    DR.Item("Reg_Cod") = Reg_Cod
                    DR.Item("StatoImpianto_Cod") = StatoImpianto_Cod
                    DR.Item("N") = N
                    DR.Item("P") = P
                    DR.Item("K") = K
                    DR.Item("Data_Semina") = Data_Semina
                    DR.Item("Data_Raccolta") = Data_Raccolta
                    DR.Item("Data_Fioritura") = Data_Fioritura
                    DR.Item("Coltura_Precedente") = Coltura_Precedente
                    DR.Item("Coltura_Precedente2") = Coltura_Precedente2
                    DR.Item("Coltura_Precedente3") = Coltura_Precedente3
                    DR.Item("Coltura_Precedente4") = Coltura_Precedente4
                    DR.Item("Piano_Semina") = Piano_Semina
                    DR.Item("Codice_Contratto") = Codice_Contratto
                    DR.Item("unito") = unito
                    DR.Item("frazionato") = frazionato
                    DR.Item("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico
                    DR.Item("provenienza_fascicolo") = provenienza_fascicolo

                    DR.Item("Regolamento_Concimazione_Cod") = Regolamento_Concimazione_Cod
                    DR.Item("Flag_PubblicoPrivato") = Flag_PubblicoPrivato
                    DR.Item("id_tr") = id_tr

                    If Reg_Cod <> "4" Then
                        DR.Item("Disciplinare") = Dpi_Cod & "/" & Flag_PubblicoPrivato & "/" & Regolamento_Concimazione_Cod & "/" & id_tr
                    Else
                        DR.Item("Disciplinare") = -2
                    End If

                    DR.Item("ZoneCatasto") = ZoneCatasto

                    DR.Item("chiave") = chiave
                    chiave += 1

                    DtParticelle.Rows.Add(DR)

                End If

            Next

        End If


    End Sub


    Public Shared Sub trovaDisciplinare(Piva As String,
                                        ByVal DataInizio As Date,
                                        ByVal DataFine As Date,
                                        ByRef Disciplinare As String,
                                        ByRef Dpi_Cod As String,
                                        ByRef Reg_Cod As String,
                                        ByRef Regolamento_Concimazione_Cod As String,
                                        ByRef Flag_PubblicoPrivato As String,
                                        ByRef id_tr As String,
                                        ByRef MetodoProduzione_Cod As Integer,
                                        ByRef MetodoProduzione_Des As String,
                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Disciplinare = ""
        Dpi_Cod = 0
        Reg_Cod = 1
        Regolamento_Concimazione_Cod = 0
        Flag_PubblicoPrivato = 0
        id_tr = 0
        MetodoProduzione_Cod = 1
        MetodoProduzione_Des = "Convenzionale"

        Try

            Dim ic_r As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim val As String = ic_r.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)
            If val <> "" Then
                impostaDisciplinareDaPreferenza(val, DataInizio, DataFine, Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des, objParametri_Server, objParametri_Utenti)
            Else
                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                val = ObjUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO, objParametri_Utenti, 1)
                If val <> "" Then
                    impostaDisciplinareDaPreferenza(val, DataInizio, DataFine, Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des, objParametri_Server, objParametri_Utenti)
                End If
            End If

        Catch ex As Exception

        End Try

    End Sub


    Public Shared Sub impostaDisciplinareDaPreferenza(ByVal preferenza As String,
                                                      ByVal DataInizio As Date,
                                                      ByVal DataFine As Date,
                                                      ByRef Disciplinare As String,
                                                      ByRef Dpi_Cod As String,
                                                      ByRef Reg_Cod As String,
                                                      ByRef Regolamento_Concimazione_Cod As String,
                                                      ByRef Flag_PubblicoPrivato As String,
                                                      ByRef id_tr As String,
                                                      ByRef MetodoProduzione_Cod As Integer,
                                                      ByRef MetodoProduzione_Des As String,
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim ASG_SuperUser_CodFiscale = objParametri_Server.PivaSuperUser
        Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
        Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim agrowc = New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}

        Dim x As New AgronicaCoreDpiBIZ.CaricaListControl

        If preferenza.Contains("e:") Then

            'Metodo nuovo
            Dim ente = preferenza.Split("/")(0).Split(":")(1)
            Dim fp = preferenza.Split("/")(1).Split(":")(1)
            x.Trova_DisciplinareCompleto_Ente(ente,
                                              fp,
                                              ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt,
                                              objParametri_Server,
                                              objParametri_Utenti,
                                              0, 0, 0, 0,
                                              True, True, True,
                                              agrowc,
                                              Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des)
        ElseIf preferenza = "-2" Then

            Disciplinare = "-2"
            Dpi_Cod = "0"
            Reg_Cod = "4"
            Regolamento_Concimazione_Cod = "0"
            Flag_PubblicoPrivato = "0"
            id_tr = "0"
            MetodoProduzione_Cod = 3
            MetodoProduzione_Des = "Biologico"

        Else

            'metodo Vecchio
            Dim ente = ""
            Dim fp = ""
            x.Trova_Ente_Disciplinare_NoSession(preferenza, ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt, objParametri_Server, objParametri_Utenti, 0, 0, 0, 0, True, True, True,
                                              agrowc, ente, fp)

            If ente <> "" Then
                x.Trova_DisciplinareCompleto_Ente(ente,
                                              fp,
                                              ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt,
                                              objParametri_Server,
                                              objParametri_Utenti,
                                              0, 0, 0, 0,
                                              True, True, True,
                                              agrowc,
                                              Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des)
            End If

        End If

    End Sub


    Public Function Converti_TipoMacchina(TipoMacchina As String) As String
        Return "06.02.01"
    End Function

    Public Function Converti_FormaPossesso(Possesso As String) As Integer
        Dim retVal As Integer = 0
        Select Case Possesso
            Case "L" 'Leasing
                retVal = 0
            Case "N" 'A Nolo
                retVal = 0
            Case "P" 'Proprietario
                retVal = 1
            Case "PU" 'Proprietario Utilizzatore
                retVal = 2
            Case "U" 'Utilizzatore
                retVal = 0
            Case "A" 'Non riconosciuto
                retVal = 0
            Case "M" 'Non riconosciuto
                retVal = 0
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_TipoTarga(TipoTarga As String) As Integer
        Dim retVal As Integer = 0
        Select Case TipoTarga
            Case "F" 'Senza Targa
                retVal = 1
            Case "S" 'Stradale
                retVal = 2
            Case "R" 'Stradale Rimorchi
                retVal = 3
            Case "T" 'Triangolare
                retVal = 4
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Carburante(Carburante As String) As Integer
        Dim retVal As Integer = 0
        Select Case Carburante
            Case "0" 'Non Definito
                retVal = 0
            Case "B" 'Benzina
                retVal = 1
            Case "G" 'Gasolio
                retVal = 2
            Case "P" 'Petrolio
                retVal = 7
            Case "N" 'No carburante
                retVal = 0
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Trazione(Trazione As String) As Integer
        Dim retVal As Integer = 0
        Select Case Trazione
            Case "C" 'Cingoli
                retVal = 2
            Case "DT" 'Doppia trazione
                retVal = 4
            Case "F" 'Fisso
                retVal = 5
            Case "M" 'Mobile
                retVal = 6
            Case "P" 'Ruote
                retVal = 1
            Case "N" 'Semicingoli
                retVal = 3
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Marca(Marca As String) As Integer
        Return 0
    End Function

    Public Function GeneraRandom(objParametri_Server As AgronicaCoreParametri,
                                 objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            Dim ok = False


            Dim objAgroSe As New Agro_Sequenze
            Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt As DataTable

            Dim str_r As String = ""
            While ok = False
                str_r = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri_Server).ToString.Replace("-", "F")
                'controllo se è già usato 
                dt = objCont.LeggiContattoSpecifico("", str_r, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dt.Rows.Count = 0 Then
                    ok = True
                End If
            End While
            objParametri_Server.ResettaFinestra()

            r.RispostaOK = True
            r.RispostaStringa = str_r

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    Private Sub dtParticelleAggrega(ByVal iAggregaSpecie As Integer,
                                    ByVal bDividiCentri As Boolean,
                                    ByVal bAggregaTare As Boolean,
                                    ByVal DtParticelle As DataTable,
                                    ByRef dtParticelleAggregate As DataTable)


        If DtParticelle.Rows.Count > 0 Then

            Dim qryRisultato As IEnumerable(Of DataRow) = Nothing

            Dim qrySpecie As IEnumerable(Of DataRow) = Nothing
            Dim qryTare As IEnumerable(Of DataRow) = Nothing


            'Suddividi Unità Produttive in relazione alla localizzazione delle particelle catastali (Provincia, Comune)
            If bDividiCentri Then
                For ipart As Integer = 0 To DtParticelle.Rows.Count - 1

                    Dim curProCom As String = DtParticelle.Rows(ipart)("Prov") & "-" & DtParticelle.Rows(ipart)("Com")
                    Dim curProComDes As String = DtParticelle.Rows(ipart)("Prov_Des") & "-" & DtParticelle.Rows(ipart)("Com_Des")

                    DtParticelle.Rows(ipart)("Sa_Nome") = curProComDes ' curProCom

                Next

            End If

            'tutto rimane com'è.
            If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                qryRisultato = (From dtt In DtParticelle.AsEnumerable Select dtt)
                CreaStringaCatasto(qryRisultato)
            Else

                'Aggrega le superfici coltivabili per uniformità di Utilizzo (Specie Vegetale), somma gli ettari e crea un unico Appezzamento (oppure le imposta come sono)
                Select Case iAggregaSpecie
                    Case 1
                        'qrySpecie = aggregazioneParticelle(0, DtParticelle, dtParticelleAggregate)
                    Case 2
                        'qrySpecie = aggregazioneParticelleComune(0, DtParticelle, dtParticelleAggregate)
                    Case 3
                        'qrySpecie = aggregazioneParticelleConduzione(0, DtParticelle, dtParticelleAggregate)
                    Case 0
                        qrySpecie = (From dtt In DtParticelle.AsEnumerable
                                    Where Not {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                                    Select dtt)
                        CreaStringaCatasto(qrySpecie)
                End Select
                'If bAggregaSpecie Then
                '    qrySpecie = aggregazioneParticelle(0, DtParticelle, dtParticelleAggregate)
                'Else
                '    qrySpecie = (
                '    From dtt In DtParticelle.AsEnumerable
                '    Where Not {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                '    Select dtt)
                '    CreaStringaCatasto(qrySpecie)
                'End If


                'Aggrega le Tare, somma gli ettari e crea un unico Appezzamento (oppure le imposta come sono)
                If bAggregaTare Then
                    'qryTare = aggregazioneParticelle(840, DtParticelle, dtParticelleAggregate)
                Else
                    qryTare = (From dtt In DtParticelle.AsEnumerable
                                Where {"810", "840", "880", "920"}.Contains(dtt("Macrouso_Cod"))
                                Select dtt)
                    CreaStringaCatasto(qryTare)
                End If

                qryRisultato = qrySpecie
                qryRisultato = qryRisultato.Concat(qryTare)

            End If

            dtParticelleAggregate = qryRisultato.CopyToDataTable()

            'ricalcolo numerazione.
            For numApp_Nome As Integer = 0 To dtParticelleAggregate.Rows.Count - 1
                dtParticelleAggregate.Rows(numApp_Nome)("App_Nome") = (numApp_Nome + 1).ToString
            Next

        End If

    End Sub

    Private Sub CreaStringaCatasto(DTParticelle As IEnumerable(Of DataRow))
        For Each row As DataRow In DTParticelle
            row.Item("catasto") = "[" & impostaCatasto(row.Item("Prov"),
                                                       row.Item("Prov_Des"),
                                                       row.Item("Com"),
                                                       row.Item("Com_Des"),
                                                       row.Item("Sezione"),
                                                       row.Item("Foglio"),
                                                       row.Item("Numero"),
                                                       row.Item("Subalterno"),
                                                       row.Item("supcondotta"),
                                                       row.Item("possesso"),
                                                       row.Item("datepossesso"),
                                                       row.Item("inizio_possesso"),
                                                       row.Item("fine_possesso"),
                                                       row.Item("sup"),
                                                       row.Item("Utilizzo_sup")) & "]"
        Next
    End Sub

    Private Function impostaCatasto(Istat_Prov As String,
                                    Prov As String,
                                    Istat_Com As String,
                                    Com As String,
                                    Sezione As String,
                                    Foglio As Integer,
                                    Numero As Integer,
                                    Subalterno As String,
                                    Sup_Condotta As Double,
                                    possesso As String,
                                    datepossesso As String,
                                    inizio_possesso As Date,
                                    fine_possesso As Date,
                                    sup As Double,
                                    utilizzo_sup As Double) As String
        Dim Catasto As New CatastoAppezzamento
        If Istat_Prov IsNot Nothing Then
            Catasto.Istat_Prov = Istat_Prov
            Catasto.Prov = Prov
            Catasto.Istat_Com = Istat_Com
            Catasto.Com = Com
            Catasto.Sezione = Sezione
            Catasto.Foglio = Foglio
            Catasto.Numero = Numero
            Catasto.Subalterno = Subalterno
            Catasto.Sup_Condotta = Sup_Condotta
            Catasto.possesso = possesso
            Catasto.datepossesso = datepossesso
            Catasto.inizio_possesso = inizio_possesso
            Catasto.fine_possesso = fine_possesso
            Catasto.sup = sup
            Catasto.Utilizzo_sup = utilizzo_sup
            Catasto.key = Istat_Prov & "_" & Istat_Com & "_" & Sezione & "_" & CStr(Foglio) & "_" & CStr(Numero) & "_" & Subalterno
            Dim ser = Newtonsoft.Json.JsonSerializer.Create
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            ser.Serialize(sw, Catasto)
            Return sb.ToString
        Else
            Return "[]"
        End If

    End Function

    Private Function JSON_DataTableAppezzamenti_Tabella(ByRef DT_Appezzamenti As DataTable,
                                                        ByVal DataInizio As String,
                                                        ByVal DataFine As String,
                                                        Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "chiave", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("piva", "piva", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("sa_cod", "sa_cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("appezza", "appezza", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("programmazione_entita_cod", "Programmazione_Entita_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("datoGis", "Gis", "String")
        c._FormatoParticolare = "<span class='fa fa-globe fa-2x'></span>"
        'c._RemoveHtmlEncode = True
        c._Filtrabile = False
        c._width = "46px"
        l.Add(c)

        c = New ColonneNome("app_nome", "Appezzamento", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "60px"
        l.Add(c)

        c = New ColonneNome("utilizzo_sup", "Sup. Utilizzo [ha] (SAU)", "number")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "82px"
        c._formatNr = "n4"
        c._sum = True
        l.Add(c)

        c = New ColonneNome("macrouso_cod", "Macrouso_Cod", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("macrouso_sup", "Macrouso_Sup", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("utilizzo", "Utilizzo (Specie Vegetale)", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("varieta", "Varietà", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_des", "Specie Vegetale", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("cul_des", "Varietà", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grfi_des", "Finalità Produttiva", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("macrouso", "Macrouso", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grva_des", "Tipologia Varietale", "string")
        c._hidden = True
        l.Add(c)

        ''GESTIONE CATASTO IN LINEA
        c = New ColonneNome("PROV", "PROV", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("prov_des", "PR.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("COM", "COM", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("com_des", "Comune", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "150px"
        l.Add(c)

        c = New ColonneNome("sezione", "Sez.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("foglio", "Fgl.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("numero", "Numero", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("subalterno", "Sub.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro", "string")
        c._Editabile = True
        c._Filtrabile = False
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("validita_inizio", "Inizio Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "94.55px"
        c._valueDefault = DataInizio
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("validita_fine", "Fine Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "94.55px"
        c._valueDefault = DataFine
        c._Filtrabile = False
        l.Add(c)

        c = New ColonneNome("prov", "PROV", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("com", "COM", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto", "Catasto (Provincia-Comune-Sezione-Foglio-Numero-Subalterno)", "string")
        c._FormatoParticolare = "#= GetValoreParticella(chiave, catasto)#"
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto_key", "Catasto_Key", "String")
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_cod", "Id_Cod", "String")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("veg_cod", "Veg_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod", "Cul_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("grfi_cod", "Grfi_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("grva_cod", "Grva_Cod", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_cod_agea", "Veg_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod_agea", "Cul_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("ribaltato", "ribaltato", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("movimentato", "movimentato", "String")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("Cop_Cod", "Cop_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cop_Des", "Cop_Des", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Lotto", "Lotto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Resa", "Resa", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Num_Piante", "Num_Piante", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TRA_Fila", "TRA_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("SU_Fila", "SU_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Validita_Inizio_Impianto", "Validita_Inizio_Impianto", "Date")
        c._Editabile = True
        'c._valueDefault = DataInizio
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona", "TipoZona", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona_Des", "TipoZona_Des", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Des", "MetodoProduzione_Des", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Unita_Vitata", "Unita_Vitata", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Des_Agea", "Veg_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Des_Agea", "Cul_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Cod_Agea", "Uso_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Des_Agea", "Uso_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Cod_Agea", "Occupazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Des_Agea", "Occupazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod_Agea", "Destinazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Des_Agea", "Destinazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Cod_Agea", "Qualita_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Des_Agea", "Qualita_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Dpi_Cod", "Dpi_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare", "Disciplinare", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("StatoImpianto_Cod", "StatoImpianto_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("N", "N", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("P", "P", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("K", "K", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Semina", "Data_Semina", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Raccolta", "Data_Raccolta", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Fioritura", "Data_Fioritura", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente", "Coltura_Precedente", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente2", "Coltura_Precedente2", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente3", "Coltura_Precedente3", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente4", "Coltura_Precedente4", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("unito", "unito", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("frazionato", "frazionato", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "String")
        'c._Editabile = True
        'c._hidden = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False

        Dim risp As String

        If stringaKendoRow = "" Then
            Dim strKendoRow As New StringBuilder
            AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
            risp = strKendoRow.ToString
        Else
            risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        End If

        Return risp

    End Function

    Public Class CatastoAppezzamento
        Public Property Istat_Prov As String
        Public Property Prov As String
        Public Property Com As String
        Public Property Istat_Com As String
        Public Property Sezione As String
        Public Property Foglio As Integer
        Public Property Numero As Integer
        Public Property Subalterno As String
        Public Property Sup_Condotta As Double
        Public Property key As String
        Public Property possesso As String
        Public Property datepossesso As String
        Public Property inizio_possesso As Date
        Public Property fine_possesso As Date
        Public Property sup As Double
        Public Property Utilizzo_sup As Double
    End Class

End Class
