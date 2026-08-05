Imports System.IO
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ

Public Class Importatore

    Public Function CreaAzienda(ByVal ASG_ProgressivoGIAS As Integer,
                                       ByVal ASG_Utente_Password As String,
                                       ByRef Piva As String,
                                       ByVal Cuaa As String,
                                       ByVal fascicolo As String,
                                       ByRef strErr As String,
                                       ByRef strRis As String,
                                       ByRef RagSoc As String,
                                       ByRef Indirizzo As String,
                                       ByRef Cap As String,
                                       ByRef Istat_Provincia As String,
                                       ByRef Istat_Comune As String,
                                       ByRef OrigineOpr As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                       ByRef AziendaVisibile As Boolean,
                                       ByRef _Piva_Padre As String,
                                       ByRef Flag_ImportaMacchine As Boolean,
                                       ByRef Flag_ImportaCatasto As Boolean,
                                       ByRef Flag_ImportaPlanning As Boolean,
                                       ByRef Planning_Importato_Automaticamente As Boolean,
                                       ByRef ImportaSoloConConsistenze As Boolean
                                ) As Boolean

        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(fascicolo)
        obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

        Dim strFascicolo2 As String = ""
        Dim strConsistenze As String = ""
        Dim strConsistenze2 As String = ""
        Dim strMacchine As String = ""
        Dim strSoggetti As String = ""

        Dim Fascicolo_Umbria = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo15)

        Dim Consistenze_Umbria = Leggi_Consistenze2(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strConsistenze)

        If Consistenze_Umbria Is Nothing Then
            Consistenze_Umbria = New List(Of AGEA_Coordinamento.ISWSTerritorio1)().ToArray
        End If

        Dim Consistenze_Umbria2 = Leggi_Consistenze(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strConsistenze2)

        If Consistenze_Umbria2 Is Nothing Then
            Consistenze_Umbria2 = New List(Of AGEA_Coordinamento.ISWSTerritorio15)().ToArray
        End If

        Dim Fascicolo2 As AGEA_Coordinamento.ISWSRespAnagFascicolo2 = Nothing
        Try
            Fascicolo2 = Leggi_Fascicolo20(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strFascicolo2)
        Catch ex As Exception

        End Try

        Dim Macchine = Leggi_Macchine(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strMacchine)

        Dim Soggetti = Leggi_Soggetti(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strSoggetti)

        Dim Allevamenti = Leggi_Allevamenti(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strSoggetti)

        If Fascicolo_Umbria IsNot Nothing Then
            Dim Piva_Padre As String = objParametri_Server.PivaSuperUser
            Dim Sigla_Provincia As String = ""
            Dim Comune As String = ""
            RagSoc = ""
            Indirizzo = ""
            Cap = ""
            Istat_Provincia = ""
            Istat_Comune = ""
            Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

            Dim esisteImpresa = objImpresa.Esiste_CUAA(Cuaa, objParametri_Server)

            If esisteImpresa AndAlso _Piva_Padre = "" Then
                Piva = objImpresa.Piva_From_CUAA(Cuaa, objParametri_Server)
                _Piva_Padre = objGerarchia.LeggiPadre(Piva, objParametri_Server, "")
            End If

            If Fascicolo_Umbria.detentore IsNot Nothing AndAlso _Piva_Padre = "" Then

                Dim Detentore_Fascicolo As String = Fascicolo_Umbria.detentore

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

                    SeCreaUffizioZona(ASG_ProgressivoGIAS, ASG_Utente_Password, strErr, strRis, Istat_Provincia, Istat_Comune, objParametri_Server, objParametri_Utenti, Piva_Padre, Sigla_Provincia, objImpresa, Detentore_Fascicolo)

                End If

            ElseIf _Piva_Padre <> "" Then

                Piva_Padre = _Piva_Padre

            End If

            If Not Utente_Visibilita_Impresa(Piva_Padre, objParametri_Server, objParametri_Utenti) Then
                AziendaVisibile = False
                Return False
            End If

            '******   IMPRESA      *********************************************************************************************

            Select Case Fascicolo_Umbria.tipoAzienda
                Case AGEA_Coordinamento.ISWSRespAnagFascicolo15TipoAzienda.PF '"PF", "P", "1", "2"
                    RagSoc = Fascicolo_Umbria.denominazione & " " & Fascicolo_Umbria.nomePF
                Case AGEA_Coordinamento.ISWSRespAnagFascicolo15TipoAzienda.PG '"PG", "G"
                    RagSoc = Fascicolo_Umbria.denominazione
            End Select

            Sigla_Provincia = ""
            Comune = ""

            If Fascicolo_Umbria.sedeResidenza IsNot Nothing Then
                Cap = Fascicolo_Umbria.sedeResidenza.cap
                Indirizzo = Fascicolo_Umbria.sedeResidenza.indirizzo
                Istat_Provincia = Fascicolo_Umbria.sedeResidenza.provincia
                Istat_Comune = Fascicolo_Umbria.sedeResidenza.comune
            Else
                If Fascicolo_Umbria.recapito IsNot Nothing Then
                    Cap = Fascicolo_Umbria.recapito.cap
                    Indirizzo = Fascicolo_Umbria.recapito.indirizzo
                    Istat_Provincia = Fascicolo_Umbria.recapito.provincia
                    Istat_Comune = Fascicolo_Umbria.recapito.comune
                End If
            End If

            If Istat_Provincia = "" Then
                strErr = "CUAA:" & Cuaa & " - " & "Impossibile inserire l'impresa poiché manca l'indirizzo!"
                Exit Function
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

            Piva = objImpresa.Piva_From_CUAA(Cuaa, objParametri_Server)

            'If Cuaa.Length = 11 Then
            '    Piva = Cuaa
            'End If

            If Piva = "" Then
                If Fascicolo2 IsNot Nothing AndAlso Fascicolo2.partitaIVA IsNot Nothing AndAlso Fascicolo2.partitaIVA <> "" Then
                    Piva = Fascicolo2.partitaIVA
                End If
            End If

            If Piva = "" Then
                Piva = GeneraRandom(objParametri_Server, objParametri_Utenti).RispostaStringa
            End If

            Dim Detentore As String = ""
            Dim dataValidazione As Date = AGRODATAINIZIO
            Dim SchedaValidazione As String = ""
            If Fascicolo_Umbria.detentore IsNot Nothing Then
                Detentore = Fascicolo_Umbria.detentore
            End If

            If Fascicolo_Umbria.dataValidazFascicolo IsNot Nothing AndAlso Fascicolo_Umbria.dataValidazFascicolo <> "" Then
                dataValidazione = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Fascicolo_Umbria.dataValidazFascicolo))
            End If

            If Fascicolo_Umbria.schedaValidazione IsNot Nothing Then
                SchedaValidazione = Fascicolo_Umbria.schedaValidazione
            End If

            FascicoloMemorizza(4, Piva, dataValidazione, SchedaValidazione, fascicolo, objParametri_Server, 0, Detentore)

            If strFascicolo2 <> "" Then
                FascicoloMemorizza(44, Piva, dataValidazione, SchedaValidazione, strFascicolo2, objParametri_Server, 0, Detentore)
            End If

            If Consistenze_Umbria IsNot Nothing AndAlso Consistenze_Umbria.Length > 0 Then
                FascicoloMemorizza(45, Piva, dataValidazione, SchedaValidazione, strConsistenze, objParametri_Server, 0, Detentore)
            End If

            If Consistenze_Umbria2 IsNot Nothing AndAlso Consistenze_Umbria2.Length > 0 Then
                FascicoloMemorizza(41, Piva, dataValidazione, SchedaValidazione, strConsistenze, objParametri_Server, 0, Detentore)
            End If

            If Macchine.Length > 0 Then
                FascicoloMemorizza(42, Piva, dataValidazione, SchedaValidazione, strMacchine, objParametri_Server, 0, Detentore)
            End If

            FascicoloMemorizza(43, Piva, dataValidazione, SchedaValidazione, strSoggetti, objParametri_Server, 0, Detentore)

            If ImportaSoloConConsistenze AndAlso Consistenze_Umbria.Count = 0 Then
                strErr = "No Consistenza"
                Return False
            End If

            'Creo l'azienda

            Dim rval As Boolean =
            CreaAziendaGias(ASG_ProgressivoGIAS, ASG_Utente_Password,
                        Piva_Padre, Piva, Cuaa, RagSoc,
                        Indirizzo, Cap, Comune, Sigla_Provincia, Istat_Comune, Istat_Provincia,
                        True, Flag_ImportaCatasto,
                        strErr,
                        strRis,
                        objParametri_Server, objParametri_Utenti, Fascicolo_Umbria,
                        Consistenze_Umbria.ToList, Consistenze_Umbria2.ToList, False, Macchine.ToList, Flag_ImportaMacchine, Soggetti, 1,
                        Flag_ImportaPlanning, Planning_Importato_Automaticamente)

            Return rval

        End If

    End Function

    Private Sub SeCreaUffizioZona(ASG_ProgressivoGIAS As Integer,
                                  ASG_Utente_Password As String,
                                  ByRef strErr As String,
                                  ByRef strRis As String,
                                  ByRef Istat_Provincia As String,
                                  ByRef Istat_Comune As String,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                  ByRef Piva_Padre As String,
                                  ByRef Sigla_Provincia As String,
                                  objImpresa As Imprese_Read,
                                  ByRef Detentore_Fascicolo As String)

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

                If Not bEsisteUffZona Then
                    CreaUffZona = True
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
                                       True,
                                       False,
                                       strErr,
                                       strRis,
                                       objParametri_Server,
                                       objParametri_Utenti, Nothing, Nothing, Nothing, False, Nothing,
                                       False, Nothing, 2, False, False) = True Then
                        Piva_Padre = "UZ" & Detentore_Fascicolo
                    End If

                End If

            End If

        End If

    End Sub

    Private Function Utente_Visibilita_Impresa(Piva As String, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New AgronicaCoreUtility.JoinFiltrone

        ClassJoin.bGerarchiaImprese = True

        'End If

        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente("", ClassJoin)

        Dim Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server,
                                                    " Imprese.PIVA = '" & Piva & "' ",
                                                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                                                    "",
                                                    ClassJoin)

        If Dt_Imprese IsNot Nothing AndAlso Dt_Imprese.Rows.Count > 0 Then

            Return True

        Else

            Return False

        End If

    End Function


    Public Function Leggi_Consistenze(ByVal Cuaa As String,
                                      ByVal Numero_Validazione As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef str As String) As AGEA_Coordinamento.ISWSTerritorio15()

        Dim import As New AGEA_UMBRIA_Utility

        Dim errCOD As Integer
        Dim errMsg As String = ""

        Dim str_consistenze = import.CaricaDati_WS_Consistenze_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
        str = str_consistenze
        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(str_consistenze)
        If str_consistenze <> "" Then
            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

            Dim listTerritori As New List(Of AGEA_Coordinamento.ISWSTerritorio15)

            For Each terr In obj_fascicolo.Items
                Try
                    listTerritori.Add(terr)
                Catch ex As Exception

                End Try
            Next

            Return listTerritori.ToArray

        End If

        Return Nothing

    End Function

    Public Function Leggi_Consistenze2(ByVal Cuaa As String,
                                      ByVal Numero_Validazione As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef str As String) As AGEA_Coordinamento.ISWSTerritorio1()

        Dim import As New AGEA_UMBRIA_Utility

        Dim errCOD As Integer
        Dim errMsg As String = ""

        Dim str_consistenze = import.CaricaDati_WS_Consistenze2_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
        str = str_consistenze
        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(str_consistenze)
        If str_consistenze <> "" Then
            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

            Dim listTerritori As New List(Of AGEA_Coordinamento.ISWSTerritorio1)

            For Each terr In obj_fascicolo.Items
                Try
                    listTerritori.Add(terr)
                Catch ex As Exception

                End Try
            Next

            Return listTerritori.ToArray

        End If

        Return Nothing

    End Function

    Public Function Leggi_Fascicolo20(ByVal Cuaa As String,
                                      ByVal Numero_Validazione As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef str As String) As AGEA_Coordinamento.ISWSRespAnagFascicolo2

        Dim import As New AGEA_UMBRIA_Utility

        Dim errCOD As Integer
        Dim errMsg As String = ""

        Dim str_consistenze = import.CaricaDati_WS_AgroFascicolo20_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
        str = str_consistenze
        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(str_consistenze)
        Dim fascicolo2 As AGEA_Coordinamento.ISWSRespAnagFascicolo2 = Nothing
        Try
            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)
            If obj_fascicolo.Items IsNot Nothing AndAlso obj_fascicolo.Items.Length > 0 Then
                fascicolo2 = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo2)
            End If
        Catch ex As Exception
            Throw New Exception("Non è stato possibile recuperare il fascicolo " & Cuaa)
        End Try

        Return fascicolo2

    End Function

    Public Function Leggi_Macchine(ByVal Cuaa As String,
                                   ByVal Numero_Validazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef str As String) As AGEA_Coordinamento.ISWSMacchina()

        Dim import As New AGEA_UMBRIA_Utility

        Dim errCOD As Integer
        Dim errMsg As String = ""

        Dim str_consistenze = import.CaricaDati_WS_Macchine_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
        str = str_consistenze
        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        If str_consistenze <> "" Then
            Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
            Dim string_reader As New StringReader(str_consistenze)
            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

        End If


        Dim listMacchine As New List(Of AGEA_Coordinamento.ISWSMacchina)

        If obj_fascicolo IsNot Nothing AndAlso obj_fascicolo.Items IsNot Nothing Then
            For Each macc In obj_fascicolo.Items
                Try
                    If macc.ToString <> "" Then
                        listMacchine.Add(macc)
                    End If
                Catch ex As Exception

                End Try
            Next
        End If


        Return listMacchine.ToArray

    End Function

    Public Function Leggi_Soggetti(ByVal Cuaa As String,
                                   ByVal Numero_Validazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef str As String) As AGEA_Coordinamento.DettaglioSoggettoWS
        Try
            Dim import As New AGEA_UMBRIA_Utility

            Dim errCOD As Integer
            Dim errMsg As String = ""

            Dim str_consistenze = import.CaricaDati_WS_Soggetti_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
            str = str_consistenze
            Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

            Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
            Dim string_reader As New StringReader(str_consistenze)

            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

            Dim soggetti = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.DettaglioSoggettoWS)

            Return soggetti
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Function Leggi_Allevamenti(ByVal Cuaa As String,
                                   ByVal Numero_Validazione As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef str As String) As AGEA_Coordinamento.DettaglioSoggettoWS
        Try
            Dim import As New AGEA_UMBRIA_Utility

            Dim errCOD As Integer
            Dim errMsg As String = ""

            Dim str_consistenze = import.CaricaDati_WS_Allevamenti_UMBRIA(errCOD, errMsg, Cuaa, Numero_Validazione, objParametri_Server)
            str = str_consistenze
            Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

            Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
            Dim string_reader As New StringReader(str_consistenze)

            obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

            Dim soggetti = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.DettaglioSoggettoWS)

            Return soggetti
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

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
                                    fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                    ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                    ISWSTerritorioFS5 As List(Of AGEA_Coordinamento.ISWSTerritorio15),
                                    ByVal ModificaPossessiEsistenti As Boolean,
                                    ISWSMacchine As List(Of AGEA_Coordinamento.ISWSMacchina),
                                    ByVal ImportaMacchine As Boolean,
                                    ByVal Soggetti As AGEA_Coordinamento.DettaglioSoggettoWS,
                                    ByVal Tipo_Impresa As Integer,
                                    ByVal Importa_Planning As Boolean,
                                    ByVal Planning_Importato_Automaticamente As Boolean) As Boolean

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

        If fascicolo IsNot Nothing AndAlso fascicolo.nomePF IsNot Nothing Then
            Legale_Rappresentante_Nome = fascicolo.nomePF
        End If

        'Legale_Rappresentante_Cognome = ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.RappresentanteLegaleWS(0).SoggettoWS.Desc_cogn        
        'Legale_Rappresentante_CF = ws_fasciResponse.out.fascicolo.fascicolo.DettaglioSoggettoWS.RappresentanteLegaleWS(0).SoggettoWS.CUAA
        If fascicolo IsNot Nothing AndAlso fascicolo.sessoPFSpecified AndAlso fascicolo.sessoPF IsNot Nothing Then
            Select Case fascicolo.sessoPF.Value
                Case AGEA_Coordinamento.ISWSRespAnagFascicolo15SessoPF.F
                    Legale_Rappresentante_Sesso = "F"
                Case AGEA_Coordinamento.ISWSRespAnagFascicolo15SessoPF.M
                    Legale_Rappresentante_Sesso = "M"
            End Select
        End If

        If fascicolo IsNot Nothing AndAlso fascicolo.dataNascitaPF IsNot Nothing Then
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

        Dim Codice_Ufficio_Rea = "#"
        Dim Codice_Numero_Rea = "#"

        If fascicolo IsNot Nothing AndAlso fascicolo.iscrizioneRea IsNot Nothing AndAlso fascicolo.iscrizioneRea.Length > 0 Then

            Dim numeroIscrizione = fascicolo.iscrizioneRea(fascicolo.iscrizioneRea.Length - 1).NumeroIscrizione

            If numeroIscrizione.Contains(" ") Then
                Codice_Ufficio_Rea = numeroIscrizione.Split(" ")(0)
                Codice_Numero_Rea = numeroIscrizione.Split(" ")(1)
            Else
                Codice_Numero_Rea = numeroIscrizione
            End If

        End If


        XmlImpresa = objXML.Xml_Pubblico_Impresa(Tipo_Operazione:=tipoOperazione,
                                                              Piva:=Piva,
                                                              Rag_Soc:=RagSoc,
                                                              Cuaa:=Cuaa.ToUpper,
                                                              Codice_Fiscale:=Cuaa.ToUpper,
                                                              Cod_Socio:="#",
                                                              Sup_Totale:="#",
                                                              Piva_Padre:=Piva_Padre,
                                                              Tipo_Gerarchia:=CStr(Tipo_Impresa),
                                                              Titolo_Possesso:="1",
                                                              Validita_Inizio:="#", Validita_Fine:="#",
                                                              i_indirizzo:=Indirizzo,
                                                              i_frazione:="#",
                                                              i_cap:=Cap,
                                                              i_comune:=Comune,
                                                              i_provincia:=Sigla_Provincia,
                                                              i_stato:="#", i_note_indirizzo:="#",
                                                              i_codice_istat_comune:=Istat_Comune,
                                                              i_codice_istat_provincia:=Istat_Provincia,
                                                              legale_rappresentante:="",
                                                              lr_cognome:=Legale_Rappresentante_Cognome,
                                                              lr_nome:=Legale_Rappresentante_Nome,
                                                              lr_codice_fiscale:=Legale_Rappresentante_CF,
                                                              lr_sesso:=Legale_Rappresentante_Sesso,
                                                              lr_validita_inizio:="#", lr_validita_fine:="#",
                                                              lr_indirizzo:=Legale_Rappresentante_Indirizzo,
                                                              lr_frazione:=Legale_Rappresentante_Frazione,
                                                              lr_cap:=Legale_Rappresentante_Cap,
                                                              lr_comune:=Legale_Rappresentante_Comune,
                                                              lr_provincia:=Legale_Rappresentante_Provincia,
                                                              lr_stato:=Legale_Rappresentante_Stato,
                                                              lr_note_indirizzo:="#",
                                                              lr_codice_istat_comune:=Legale_Rappresentante_Istat_Comune,
                                                              lr_codice_istat_provincia:=Legale_Rappresentante_Istat_Provincia,
                                                              lr_nascita_data:=Legale_Rappresentante_Data_Nascita,
                                                              lr_nascita_comune:=Legale_Rappresentante_Comune_Nascita,
                                                              lr_nascita_provincia:=Legale_Rappresentante_Provincia_Nascita,
                                                              lr_nascita_istat_comune:=Legale_Rappresentante_Istat_Comune_Nascita,
                                                              lr_nascita_istat_provincia:=Legale_Rappresentante_Istat_Provincia_Nascita,
                                                              lr_documenti:="#",
                                                              lr_rubrica_1:=Legale_Rappresentante_Rubrica1,
                                                              lr_rubrica_2:=Legale_Rappresentante_Rubrica2,
                                                              lr_rubrica_3:=Legale_Rappresentante_Rubrica3,
                                                              lr_rubrica_4:=Legale_Rappresentante_Rubrica4,
                                                              lr_rubrica_5:=Legale_Rappresentante_Rubrica5,
                                                              cf_tecnico_referente:="#",
                                                              codice_cliente:="#",
                                                              codice_fornitore:="#",
                                                              codice_fornitore_2:="#",
                                                              codice_fornitore_3:="#",
                                                              codice_ausl:="#",
                                                              i_Chiave_Cliente:="#",
                                                              XmlDoc:=XmlDoc,
                                                              Codice_Ufficio_REA:=Codice_Ufficio_Rea,
                                                              Codice_Numero_REA:=Codice_Numero_Rea)

        XmlUtente.AppendChild(XmlImpresa)

        Dim CodRapporto As Integer = 0
        Dim bEsisteContatto As Boolean = False
        Dim bEsisteRisum As Boolean = False
        Dim TipoOpContatto As enum_TipoOperazioneDB
        Dim TipoOpRisum As enum_TipoOperazioneDB

        Dim XmlContatto As System.Xml.XmlElement
        Dim XmlRuolo As System.Xml.XmlElement

        Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

        Dim codCom_Nascita As String = ""
        Dim codProv_Nascita As String = ""

        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

        Dim contattiInseriti As New List(Of String)

        If Soggetti IsNot Nothing Then

            If Soggetti.RappresentanteLegaleWS IsNot Nothing Then

                For Each rappresentante In Soggetti.RappresentanteLegaleWS

                    If rappresentante.SoggettoWS IsNot Nothing Then

                        CodRapporto = -1



                        bEsisteContatto = objContatti.Esiste_Contatto(Piva,
                                                                      rappresentante.SoggettoWS.CUAA,
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "",
                                                                      objParametri_Server)

                        If contattiInseriti.Contains(rappresentante.SoggettoWS.CUAA) Then
                            Continue For
                        End If


                        TipoOpContatto = enum_TipoOperazioneDB.Scrittura
                        If bEsisteContatto Then
                            TipoOpContatto = enum_TipoOperazioneDB.Modifica
                        End If

                        TipoOpRisum = enum_TipoOperazioneDB.Scrittura
                        If bEsisteRisum Then
                            TipoOpRisum = enum_TipoOperazioneDB.Modifica
                        End If

                        Dim DataNascita As Date = AGRODATAINIZIO
                        If rappresentante.SoggettoWS.Data_nasc IsNot Nothing Then
                            DataNascita = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(rappresentante.SoggettoWS.Data_nasc))
                        End If
                        Dim sesso As String = ""

                        Select Case rappresentante.SoggettoWS.Codi_sess
                            Case AGEA_Coordinamento.SoggettoWSCodi_sess.F
                                sesso = "F"
                            Case AGEA_Coordinamento.SoggettoWSCodi_sess.M
                                sesso = "M"
                        End Select

                        objIstat.CodIstat_from_CodCatastale(codProv_Nascita, codCom_Nascita, rappresentante.SoggettoWS.Codi_fisc_luna, objParametri_Server)

                        XmlContatto = objXML.Xml_Pubblico_Contatto(TipoOpContatto,
                                                "", "",
                                                rappresentante.SoggettoWS.CUAA,
                                                rappresentante.SoggettoWS.Desc_nome,
                                                rappresentante.SoggettoWS.Desc_cogn,
                                                sesso,
                                                "#",
                                                "#",
                                                "#",
                                                DataNascita.ToShortDateString,
                                                "#", "#",
                                                codCom_Nascita,
                                                codProv_Nascita,
                                                "#",
                                                "#",
                                                "#",
                                                "#", "#",
                                                "#",
                                                "#",
                                                "#", "#", "#", "#",
                                                XmlDoc)

                        XmlRuolo = objXML.Xml_Pubblico_Ruolo(TipoOpRisum,
                                                      -1,
                                                      "Rappresentante Legale",
                                                      XmlDoc)

                        XmlContatto.AppendChild(XmlRuolo)

                        contattiInseriti.Add(rappresentante.SoggettoWS.CUAA)

                        XmlImpresa.AppendChild(XmlContatto)

                    End If

                Next

            End If

        End If

        If ImportaMacchine Then

            If ISWSMacchine IsNot Nothing AndAlso
               ISWSMacchine.Count > 0 Then

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
                        Class_Code = Converti_TipoMacchina(ISWSMacchina.TipoMacchina, objParametri_Server)
                    End If

                    TitoloPossesso = Converti_FormaPossesso(ISWSMacchina.FormaPossesso)

                    If ISWSMacchina.Targa IsNot Nothing Then
                        Targa = ISWSMacchina.Targa
                    End If

                    Tipo_Targa_Cod = Converti_TipoTarga(ISWSMacchina.TipoTarga)

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

            Dim popolaAppezzamentiDone = False
            If fascicolo IsNot Nothing AndAlso
                CreaCatasto AndAlso
                ISWSTerritorioFS6 IsNot Nothing AndAlso
                ISWSTerritorioFS6.Count > 0 Then

                Popola_DtAppezzamenti_daFascicolo_3(Piva,
                                                fascicolo,
                                                ISWSTerritorioFS6,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                "", False, objParametri_Server.UtenteUsername, ASG_Utente_Password, ASG_ProgressivoGIAS, False)
                popolaAppezzamentiDone = True
            End If

            If fascicolo IsNot Nothing AndAlso
               CreaCatasto AndAlso
               ISWSTerritorioFS5 IsNot Nothing AndAlso
               ISWSTerritorioFS5.Count > 0 AndAlso
               Not popolaAppezzamentiDone Then

                Popola_DtAppezzamenti_daFascicolo_2(Piva,
                                                fascicolo,
                                                ISWSTerritorioFS5,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                "", False, objParametri_Server.UtenteUsername, ASG_Utente_Password, ASG_ProgressivoGIAS, False)

            End If

            If CreaCatasto Then

                objImportazione.Crea_Stringa_Catasto(stringaAnagrafica,
                                                "",
                                                XmlUtente.OuterXml,
                                                "",
                                                Piva, SaCod,
                                                DataInizio, DataFine, DataInizio.Year,
                                                DtParticelle,
                                                objParametri_Server, True, ASG_ProgressivoGIAS, False)


            End If

        End If

        'stringaAnagrafica = XmlUtente.OuterXml
        If stringaAnagrafica = "" Then
            stringaAnagrafica = XmlUtente.OuterXml
        End If

        Dim AziendaCreata As Boolean
        Dim PianoCreato As Boolean

        Importa_Dati(ASG_Utente_Password, stringaAnagrafica, "", strErr, strRis, AziendaCreata, PianoCreato, objParametri_Server, objParametri_Utenti)

        Return AziendaCreata

    End Function

    Public Function Converti_TipoMacchina(TipoMacchina As String, objParametri_Server As AgronicaCoreParametri) As String
        Dim objCodifica_Macchine As New AgronicaCoreMetaSchemaDAL.Codifica_Macchine_Agea

        Return objCodifica_Macchine.Class_Cod_da_AGEA_Cod(objParametri_Server, TipoMacchina, "")

    End Function

    Public Function Converti_FormaPossesso(Possesso As AGEA_Coordinamento.ISWSMacchinaFormaPossesso) As Integer
        Dim retVal As Integer = 0
        Select Case Possesso
            Case AGEA_Coordinamento.ISWSMacchinaFormaPossesso.L  'Leasing
                retVal = 0
            Case AGEA_Coordinamento.ISWSMacchinaFormaPossesso.N  'A Nolo
                retVal = 0
            Case AGEA_Coordinamento.ISWSMacchinaFormaPossesso.P 'Proprietario
                retVal = 1
            Case AGEA_Coordinamento.ISWSMacchinaFormaPossesso.PU  'Proprietario Utilizzatore
                retVal = 2
            Case AGEA_Coordinamento.ISWSMacchinaFormaPossesso.U  'Utilizzatore
                retVal = 0
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_TipoTarga(TipoTarga As AGEA_Coordinamento.ISWSMacchinaTipoTarga) As Integer
        Dim retVal As Integer = 0
        Select Case TipoTarga
            Case AGEA_Coordinamento.ISWSMacchinaTipoTarga.F  'Senza Targa
                retVal = 1
            Case AGEA_Coordinamento.ISWSMacchinaTipoTarga.S  'Stradale
                retVal = 2
            Case AGEA_Coordinamento.ISWSMacchinaTipoTarga.R  'Stradale Rimorchi
                retVal = 3
            Case AGEA_Coordinamento.ISWSMacchinaTipoTarga.T  'Triangolare
                retVal = 4
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Carburante(Carburante As AGEA_Coordinamento.ISWSMacchinaCarburante) As Integer
        Dim retVal As Integer = 0
        Select Case Carburante
            Case AGEA_Coordinamento.ISWSMacchinaCarburante.B 'Benzina
                retVal = 1
            Case AGEA_Coordinamento.ISWSMacchinaCarburante.G 'Gasolio
                retVal = 2
            Case AGEA_Coordinamento.ISWSMacchinaCarburante.N 'No carburante
                retVal = 0
            Case AGEA_Coordinamento.ISWSMacchinaCarburante.P 'Petrolio
                retVal = 7
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Trazione(Trazione As AGEA_Coordinamento.ISWSMacchinaTrazione) As Integer
        Dim retVal As Integer = 0
        Select Case Trazione
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.C  'Cingoli
                retVal = 2
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.DT  'Doppia trazione
                retVal = 4
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.F 'Fisso
                retVal = 5
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.M 'Mobile
                retVal = 6
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.R  'Ruote
                retVal = 1
            Case AGEA_Coordinamento.ISWSMacchinaTrazione.SC  'Semicingoli
                retVal = 3
            Case Else
                retVal = 0
        End Select
        Return retVal
    End Function

    Public Function Converti_Marca(Marca As String) As Integer
        Return 0
    End Function

    Public Shared Sub Importa_Dati(ByVal ASG_Utente_Password As String,
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
        Dim Importa As New WS_Importa_Gias.ImportaWS
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

    Public Shared Sub dtParticelleCreaStruttura(ByRef DtParticelle As DataTable)

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
        DtParticelle.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(Date)))
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
        DtParticelle.Columns.Add(New DataColumn("Data_semina", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("Data_Raccolta", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("Data_Fioritura", GetType(Date)))
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

        DtParticelle.Columns.Add(New DataColumn("DistBZ_CorpiIdrici", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_AreeResPub", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_Allevamenti", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_VegNatNonColt", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Campo_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Riferimento_Alfanumerico_Appezzamento", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Isola", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("CapitolatoPrivato", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("CapitolatoPrivato_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Finalita_Concimazione_Impianto", GetType(String)))

        'INDIRIZZO
        DtParticelle.Columns.Add(New DataColumn("Cod_Indirizzo", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("ind_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("frz_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("CAP", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("com_des_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("pro_cod_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_indirizzo_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("note_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("pro_cod_istat_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("com_cod_istat_indirizzo", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Pratiche_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Pratiche_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("KPIN", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Block_Name", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Foral_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Foral_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Data_Inizio_Portinnesto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))

    End Sub

    Private Sub Popola_DtAppezzamenti_daFascicolo_2(ByVal Piva As String,
                                                           ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                                           ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio15),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByRef DataInizio As Date,
                                                           ByRef DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByRef Allegati_Documenti_Numero As String,
                                                           ByRef ImportCatasto As Boolean,
                                                           ByRef ASG_Utente_Username As String,
                                                           ByRef ASG_Utente_Password As String,
                                                           ByRef ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)

        'Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

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
        Dim Validita_Inizio_Impianto As Date
        Dim TipoZona As String = ""
        Dim TipoZona_Des As String = ""
        Dim MetodoProduzione_Cod As Integer
        Dim MetodoProduzione_Des As String = ""
        Dim Unita_Vitata As String = "0"

        Dim Disciplinare As String = ""
        Dim Dpi_Cod As String = ""
        Dim Reg_Cod As String = ""
        Dim Regolamento_Concimazione_Cod As String = ""
        Dim Flag_PubblicoPrivato As String = ""
        Dim id_tr As String = ""

        Dim StatoImpianto_Cod As String = ""
        Dim N As Double? = Nothing
        Dim P As Double? = Nothing
        Dim K As Double? = Nothing
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
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
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If

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

        Dim DT_Comuni As New DataTable
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If ISWSTerritorioFS6.Count > 0 Then

            For Each ISWSTerritorio In ISWSTerritorioFS6

                'DR = DtParticelle.NewRow

                Prov = IIf(IsNothing(ISWSTerritorio.Provincia), "", ISWSTerritorio.Provincia)
                Com = IIf(IsNothing(ISWSTerritorio.Comune), "", ISWSTerritorio.Comune)
                Sezione = IIf(IsNothing(ISWSTerritorio.Sezione), "", ISWSTerritorio.Sezione)
                Foglio = IIf(IsNothing(ISWSTerritorio.Foglio), 0, ISWSTerritorio.Foglio)
                strNumero = IIf(IsNothing(ISWSTerritorio.Particella), 0, ISWSTerritorio.Particella)
                Subalterno = IIf(IsNothing(ISWSTerritorio.Subalterno), "", ISWSTerritorio.Subalterno)

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
                ElseIf Not ImportCatasto Then
                    If CatastoDaImportare Then
                        ImportCatasto = True
                        DtParticelle.Clear()
                        ImportaCatastoFascicolo2(fascicolo,
                                            ISWSTerritorioFS6,
                                            Piva,
                                            fascicolo.CUAA,
                                            True,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            ASG_Utente_Username,
                                            ASG_Utente_Password,
                                            ASG_ProgressivoGIAS)
                        Exit Sub
                    End If
                    'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                    Sa_Cod = 0
                    Sa_Nome = "01"
                End If

                Select Case ISWSTerritorio.codiceTipoConduzione
                    Case AGEA_Coordinamento.TipoConduzione.Item1
                        TitoloPossesso = 1 'Proprietà
                        TitoloPossessoDes = "Proprieta"
                    Case AGEA_Coordinamento.TipoConduzione.Item2
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


                'Select Case ISWSTerritorio.FlagIrrigua
                '    Case ISWSTerritorioFS6FlagIrrigua.Item0 'Non Irrigua
                '    Case ISWSTerritorioFS6FlagIrrigua.Item1 'Irrigua
                '    Case ISWSTerritorioFS6FlagIrrigua.Item2 'Non Dichiarato
                'End Select

                'Select Case ISWSTerritorio.FlagTerrazzata
                '    Case ISWSTerritorioFS6FlagTerrazzata.Item0 'Senza Terrazzamenti o Livellamenti
                '    Case ISWSTerritorioFS6FlagTerrazzata.Item1 'Con Terrazzamenti
                '    Case ISWSTerritorioFS6FlagTerrazzata.Item2 'Con Livellamenti
                '    Case ISWSTerritorioFS6FlagTerrazzata.Item3 'Con Terrazzamenti e Livellamenti
                'End Select

                'Select Case ISWSTerritorio.RotazioneColtureOrtive
                '    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item0 'Senza Rotazione Colturale
                '    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item1 'Con ciclo Ortivo
                '    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item2 'con ciclo Seminativo
                '    Case ISWSTerritorioFS6RotazioneColtureOrtive.Item3 'non dichiarato
                'End Select

                'If ISWSTerritorio.EffluentiZootecnici IsNot Nothing Then

                '    Select Case ISWSTerritorio.EffluentiZootecnici
                '        Case 0 'Senza affluetni zootecnici
                '        Case 1 'Con affluetni zootecnici
                '    End Select

                'End If

                'If ISWSTerritorio.SostanzePericolose IsNot Nothing Then

                '    Select Case ISWSTerritorio.SostanzePericolose
                '        Case 0 'Senza sostanze pericolose
                '        Case 1 'Con sostanze pericolose
                '    End Select

                'End If

                If ISWSTerritorio.CasiParticolari IsNot Nothing Then

                    Select Case ISWSTerritorio.CasiParticolari.Value
                        Case AGEA_Coordinamento.CasiParticolari.Item1 'Riordino fondiario
                        Case AGEA_Coordinamento.CasiParticolari.Item2 'Zona militare o zona di confine soggetta a vincoli di sicurezza
                        Case AGEA_Coordinamento.CasiParticolari.Item3 'Uso civico
                        Case AGEA_Coordinamento.CasiParticolari.Item4 'Zona demaniale
                        Case AGEA_Coordinamento.CasiParticolari.Item5 'Particelle interessate da frazionamento in data successiva al 31.12.1998
                        Case AGEA_Coordinamento.CasiParticolari.Item6 'Stato estero
                        Case AGEA_Coordinamento.CasiParticolari.Item7 'Nuovo catasto edilizio urbano
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
                            TipoZona = "n"
                            TipoZona_Des = "Non Vulnerabile"
                        Case "1" 'Si
                            TipoZona = "s"
                            TipoZona_Des = "Vulnerabile"
                    End Select
                Else
                    TipoZona = "n"
                    TipoZona_Des = "Non Vulnerabile"
                End If

                If ISWSTerritorio.Destinazione IsNot Nothing Then

                    For Each ISWSUtilizzoTerra1 In ISWSTerritorio.Destinazione

                        Qualita = ISWSUtilizzoTerra1.codiceQualita

                        strMacrousi = ""
                        strUtilizzi = ""

                        Macrouso_Cod = ISWSUtilizzoTerra1.codiceMacrouso
                        Macrouso_Sup = CDbl(ISWSUtilizzoTerra1.superficieUtilizzata) / 10000.0

                        MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                        'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
                        '                           objParametri_Server.LogFileName,
                        '                           objParametri_Server.LogDescrizioneUtente,
                        '                           NomeRoutine,
                        '                           MsgOK)

                        'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                        'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                        Dim sMacrousoSup As String = ""
                        If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                            sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                        End If

                        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                        strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(ISWSUtilizzoTerra1.codiceMacrouso,
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

                        If ISWSUtilizzoTerra1.Dettagli IsNot Nothing Then

                            For Each ISWSUtilizzoTerra In ISWSUtilizzoTerra1.Dettagli

                                If ISWSUtilizzoTerra IsNot Nothing Then

                                    strUtilizzi = ""

                                    Specie_Cod = "" 'ISWSUtilizzoTerra.CodiceProdotto
                                    Varieta_Cod = ISWSUtilizzoTerra.codiceVarieta
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
                                    Validita_Inizio_Impianto = AGRODATAINIZIO
                                    'TipoZona = "n"
                                    'TipoZona_Des = "Non Vulnerabile"
                                    MetodoProduzione_Cod = 1
                                    MetodoProduzione_Des = "Convenzionale"
                                    Unita_Vitata = "0"
                                    frazionato = 0
                                    unito = 0

                                    Dpi_Cod = ""
                                    Reg_Cod = "1"
                                    StatoImpianto_Cod = ""
                                    N = Nothing
                                    P = Nothing
                                    K = Nothing
                                    Data_Semina = AGRODATAINIZIO
                                    Data_Raccolta = AGRODATAFINE
                                    Data_Fioritura = AGRODATAINIZIO
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

                                    Uso_Cod_Agea = ""
                                    Codice_Prodotto_Agea = ISWSUtilizzoTerra.codiceProdotto
                                    Occupazione_Cod_Agea = ""
                                    Destinazione_Cod_Agea = ""
                                    Qualita_Cod_Agea = ""
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
                                                                          Gru_Cod,
                                                                          Codice_Prodotto_Agea)

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
                                        StatoImpianto_Cod = "102"
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

                                    If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                        Id_Cod = 3078
                                        Veg_Des = "Nessuna mappatura con Gias"
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
                                    If Not IsNothing(ISWSUtilizzoTerra1.DataInizioUtilizzo) AndAlso ISWSUtilizzoTerra1.DataInizioUtilizzo <> "" Then
                                        data_inizio_appezzamento = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSUtilizzoTerra1.DataInizioUtilizzo))
                                    End If

                                    data_fine_appezzamento = DataFine
                                    If Not IsNothing(ISWSUtilizzoTerra1.DataFineUtilizzo) AndAlso ISWSUtilizzoTerra1.DataFineUtilizzo <> "" Then
                                        data_fine_appezzamento = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ISWSUtilizzoTerra1.DataFineUtilizzo))
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
                                        Validita_Inizio_Impianto = CDate("01/01/" & ISWSUtilizzoTerra.AnnoImpianto)
                                    End If



                                    'Dati Aggiuntivi fascicolo AGEA
                                    If ISWSUtilizzoTerra.SuperficieEligibile AndAlso IsNumeric(ISWSUtilizzoTerra.SuperficieEligibile) Then
                                        sup_eleggibile = CDbl(ISWSUtilizzoTerra.SuperficieEligibile) / 10000
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

                                    If ISWSUtilizzoTerra.TipoImpiantoSpecified Then

                                        Select Case ISWSUtilizzoTerra.TipoImpianto

                                            Case 1 'Regolare

                                            Case 2 'Irregolare

                                        End Select

                                    End If

                                    If ISWSUtilizzoTerra.tipoCertificazioneSpecified Then

                                        Select Case ISWSUtilizzoTerra.tipoCertificazione

                                            Case 0 'Nessuna

                                            Case 1 'DOP

                                            Case 2 'IGP

                                        End Select

                                    End If

                                    If ISWSUtilizzoTerra.menzioneSpecified Then
                                        ' Transcodifica?
                                    End If


                                    If ISWSUtilizzoTerra.altitudineSpecified Then

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
                                    'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                                    '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")

                                    DR.Item("campo_cod") = 0
                                    DR.Item("campo_des") = ""

                                    DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                    DR.Item("Isola") = ""

                                    DR.Item("CapitolatoPrivato") = ""
                                    DR.Item("CapitolatoPrivato_Des") = ""

                                    DR.Item("Finalita_Concimazione_Impianto") = 0

                                    DR.Item("Cod_Indirizzo") = 0
                                    DR.Item("ind_des") = ""
                                    DR.Item("frz_des") = ""
                                    DR.Item("CAP") = ""
                                    DR.Item("com_des_indirizzo") = ""
                                    DR.Item("pro_cod_indirizzo") = ""
                                    DR.Item("stato_indirizzo") = ""
                                    DR.Item("stato_indirizzo_des") = ""
                                    DR.Item("note_indirizzo") = ""
                                    DR.Item("pro_cod_istat_indirizzo") = ""
                                    DR.Item("com_cod_istat_indirizzo") = ""

                                    DR.Item("Pratiche_Cod") = ""
                                    DR.Item("Pratiche_Des") = ""

                                    DR.Item("KPIN") = ""
                                    DR.Item("Block_Name") = ""

                                    DR.Item("Foral_Cod") = 0
                                    DR.Item("Foral_Des") = ""

                                    DR.Item("Data_Inizio_Portinnesto") = ""

                                    DR.Item("Data_Creazione") = DBNull.Value
                                    DR.Item("Data_Modifica") = DBNull.Value

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
                                    Validita_Inizio_Impianto = AGRODATAINIZIO
                                    TipoZona = "n"
                                    TipoZona_Des = "Non Vulnerabile"
                                    MetodoProduzione_Cod = 1
                                    MetodoProduzione_Des = "Convenzionale"
                                    Unita_Vitata = "0"
                                    frazionato = 0
                                    unito = 0

                                    Dpi_Cod = ""
                                    Reg_Cod = "1"
                                    StatoImpianto_Cod = ""
                                    N = Nothing
                                    P = Nothing
                                    K = Nothing
                                    Data_Semina = AGRODATAINIZIO
                                    Data_Raccolta = AGRODATAFINE
                                    Data_Fioritura = AGRODATAINIZIO
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
                                        StatoImpianto_Cod = "102"
                                    End If

                                    If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                        Dim a = 0
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
                                    DR.Item("Veg_Cod") = "0|3262" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
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

                                    DR.Item("campo_cod") = 0
                                    DR.Item("campo_des") = ""

                                    DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                    DR.Item("Isola") = ""

                                    DR.Item("CapitolatoPrivato") = ""
                                    DR.Item("CapitolatoPrivato_Des") = ""

                                    DR.Item("Finalita_Concimazione_Impianto") = 0

                                    DR.Item("Cod_Indirizzo") = 0
                                    DR.Item("ind_des") = ""
                                    DR.Item("frz_des") = ""
                                    DR.Item("CAP") = ""
                                    DR.Item("com_des_indirizzo") = ""
                                    DR.Item("pro_cod_indirizzo") = ""
                                    DR.Item("stato_indirizzo") = ""
                                    DR.Item("stato_indirizzo_des") = ""
                                    DR.Item("note_indirizzo") = ""
                                    DR.Item("pro_cod_istat_indirizzo") = ""
                                    DR.Item("com_cod_istat_indirizzo") = ""

                                    DR.Item("Pratiche_Cod") = ""
                                    DR.Item("Pratiche_Des") = ""

                                    DR.Item("KPIN") = ""
                                    DR.Item("Block_Name") = ""

                                    DR.Item("Foral_Cod") = 0
                                    DR.Item("Foral_Des") = ""

                                    DR.Item("Data_Inizio_Portinnesto") = ""

                                    DR.Item("Data_Creazione") = DBNull.Value
                                    DR.Item("Data_Modifica") = DBNull.Value

                                    DR.Item("chiave") = chiave
                                    chiave += 1

                                    DtParticelle.Rows.Add(DR)

                                End If

                            Next

                        End If
                        'End If

                    Next

                Else

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
                    DR.Item("Veg_Cod") = "0|3262"
                    DR.Item("Cul_Cod") = 0
                    DR.Item("Grfi_Cod") = 0
                    DR.Item("Grva_Cod") = 0
                    DR.Item("Id_Cod") = 0

                    DR.Item("veg_des") = "Utilizzo non specificato"
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

                    DR.Item("campo_cod") = 0
                    DR.Item("campo_des") = ""

                    DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                    DR.Item("Isola") = ""

                    DR.Item("CapitolatoPrivato") = ""
                    DR.Item("CapitolatoPrivato_Des") = ""

                    DR.Item("Finalita_Concimazione_Impianto") = 0

                    DR.Item("Cod_Indirizzo") = 0
                    DR.Item("ind_des") = ""
                    DR.Item("frz_des") = ""
                    DR.Item("CAP") = ""
                    DR.Item("com_des_indirizzo") = ""
                    DR.Item("pro_cod_indirizzo") = ""
                    DR.Item("stato_indirizzo") = ""
                    DR.Item("stato_indirizzo_des") = ""
                    DR.Item("note_indirizzo") = ""
                    DR.Item("pro_cod_istat_indirizzo") = ""
                    DR.Item("com_cod_istat_indirizzo") = ""

                    DR.Item("Pratiche_Cod") = ""
                    DR.Item("Pratiche_Des") = ""

                    DR.Item("KPIN") = ""
                    DR.Item("Block_Name") = ""

                    DR.Item("Foral_Cod") = 0
                    DR.Item("Foral_Des") = ""

                    DR.Item("Data_Inizio_Portinnesto") = ""

                    DR.Item("Data_Creazione") = DBNull.Value
                    DR.Item("Data_Modifica") = DBNull.Value

                    DR.Item("chiave") = chiave
                    chiave += 1

                    DtParticelle.Rows.Add(DR)

                End If

            Next

        End If


    End Sub

    Private Sub Popola_DtAppezzamenti_daFascicolo_3_Parallel(ByVal Piva As String,
                                                           ByVal fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                                           ByVal ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByVal DataInizio As Date,
                                                           ByVal DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByVal objParametri_Server As AgronicaCoreParametri,
                                                           ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                           ByVal Allegati_Documenti_Numero As String,
                                                           ByVal ImportCatasto As Boolean,
                                                           ByVal ASG_Utente_Username As String,
                                                           ByVal ASG_Utente_Password As String,
                                                           ByVal ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)

        'Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

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
        Dim Validita_Inizio_Impianto As Date
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
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
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

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        'Dim strValiditaInizio As String = String.Empty
        'strValiditaInizio = DataInizio.ToShortDateString
        'Dim strValiditaFine As String = String.Empty
        'strValiditaFine = DataFine.ToShortDateString


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
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If

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

        'leggo i comuni x avere le descrizioni poi
        Dim DT_Comuni As New DataTable
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objZoneMetaschema As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R

        Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        Dim objGrfi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim objCulti As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objCodAna = New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
        Dim objSpec = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
        Dim objCentrixPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider

        Dim DtParticelleC As DataTable = DtParticelle.Clone
        'Dim objParametri_ServerCC As AgronicaCoreParametri = objParametri_ServerC
        'Dim objParametri_UtentiCC As AgronicaCoreParametri = objParametri_UtentiC

        Dim processori = Environment.ProcessorCount
        Parallel.ForEach(ISWSTerritorioFS6,
                         New ParallelOptions With {.MaxDegreeOfParallelism = processori},
                         Sub(territorio As AGEA_Coordinamento.ISWSTerritorio1, state As ParallelLoopState)
                             'DR = DtParticelle.NewRow
                             Dim objParametri_ServerC = objParametri_Server.CreateDeepCopy(objParametri_Server)
                             Dim objParametri_UtentiC = objParametri_Utenti.CreateDeepCopy(objParametri_Utenti)

                             Prov = IIf(IsNothing(territorio.Provincia), "", territorio.Provincia)
                             Com = IIf(IsNothing(territorio.Comune), "", territorio.Comune)
                             Sezione = IIf(IsNothing(territorio.Sezione), "", territorio.Sezione)
                             Foglio = IIf(IsNothing(territorio.Foglio), 0, territorio.Foglio)
                             strNumero = IIf(IsNothing(territorio.Particella), 0, territorio.Particella)
                             Subalterno = IIf(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

                             'ISNULL(ISTAT.COMUNI_PROV,'') AS Prov_Des, ISNULL(ISTAT.LOCALITA,'') AS Com_Des, ")

                             Dim DrCom() As DataRow = DT_Comuni.Select("PROV='" & Prov & "' AND COM='" & Com & "'")
                             If DrCom IsNot Nothing AndAlso DrCom.Length > 0 Then
                                 Prov_Des = DrCom(0).Item("COMUNI_PROV")
                                 Com_Des = DrCom(0).Item("LOCALITA")
                             End If

                             'verifico cosa trovo nel campo particella (su Agea è una stringa)
                             'se trovo dei numeri li metto in numero
                             'se trovo dei caratteri li metto nel subalterno se non è già valorizzato

                             Numero = objCOre.Numero_from_Stringa(strNumero)
                             If Numero = 0 Then
                                 Numero = -1
                             End If
                             NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                             If Subalterno = "" AndAlso NumeroStringa <> "" Then
                                 Subalterno = Left(NumeroStringa, 3)
                             End If

                             'leggo il sa_cod

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
                                                                                         objParametri_ServerC)
                             If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                                 Sa_Cod = Dt_Centri.Rows(0).Item("sa_cod")
                                 Sa_Nome = Dt_Centri.Rows(0).Item("sa_nome")
                             ElseIf Not ImportCatasto Then
                                 If CatastoDaImportare Then
                                     ImportCatasto = True
                                     DtParticelleC.Clear()
                                     ImportaCatastoFascicolo(fascicolo,
                                                             ISWSTerritorioFS6,
                                                             Piva,
                                                             fascicolo.CUAA,
                                                             True,
                                                             objParametri_ServerC,
                                                             objParametri_UtentiC,
                                                             ASG_Utente_Username,
                                                             ASG_Utente_Password,
                                                             ASG_ProgressivoGIAS)
                                     Exit Sub
                                 End If
                                 'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                                 Sa_Cod = 0
                                 Sa_Nome = "01"
                             End If

                             Select Case territorio.codiceTipoConduzione
                                 Case AGEA_Coordinamento.TipoConduzione.Item1
                                     TitoloPossesso = 1 'Proprietà
                                     TitoloPossessoDes = "Proprieta"
                                 Case AGEA_Coordinamento.TipoConduzione.Item2
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

                             If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                                 Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                                 If Inizio_Possesso < AGRODATAINIZIO Then
                                     Inizio_Possesso = AGRODATAINIZIO
                                 End If
                             End If
                             If territorio.DataFineConduzione IsNot Nothing AndAlso
                                                         territorio.DataFineConduzione <> "" AndAlso
                                                         territorio.DataFineConduzione <> "99991231" AndAlso
                                                         territorio.DataFineConduzione <> "99990101" Then
                                 Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
                             End If

                             'DataInizio = Inizio_Possesso
                             'DataFine = Fine_Possesso

                             supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                             Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)

                             supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0

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



                             If territorio.Destinazione IsNot Nothing Then

                                 For j = 0 To territorio.Destinazione.Length - 1

                                     Qualita = territorio.Destinazione(j).codiceQualita

                                     strMacrousi = ""
                                     strUtilizzi = ""

                                     Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                                     Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                                     MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                                     'objLog.Scrivi_LOG(objParametri_ServerC.LogDirectory,
                                     '                           objParametri_ServerC.LogFileName,
                                     '                           objParametri_ServerC.LogDescrizioneUtente,
                                     '                           NomeRoutine,
                                     '                           MsgOK)

                                     'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                                     'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                                     Dim sMacrousoSup As String = ""
                                     If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                                         sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                                     End If


                                     strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                                  "", "",
                                                                                                  objParametri_ServerC) '& sMacrousoSup



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
                                     If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                                         For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                                             strUtilizzi = ""

                                             Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                                             Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                                             Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                                             Specie_Des_Agea = ""
                                             Varieta_Des_Agea = ""

                                             Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R



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
                                             Cop_Cod = 0
                                             Cop_Des = ""
                                             Lotto = ""
                                             Resa = 0
                                             Num_Piante = 0
                                             TRA_Fila = 0
                                             SU_Fila = 0
                                             Validita_Inizio_Impianto = AGRODATAINIZIO
                                             TipoZona = "n"
                                             TipoZona_Des = "Non Vulnerabile"

                                             Unita_Vitata = "0"
                                             frazionato = 0
                                             unito = 0

                                             StatoImpianto_Cod = ""
                                             N = Nothing
                                             P = Nothing
                                             K = Nothing
                                             Data_Semina = AGRODATAINIZIO
                                             Data_Raccolta = AGRODATAFINE
                                             Data_Fioritura = AGRODATAINIZIO
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

                                             Uso_Cod_Agea = ""
                                             Occupazione_Cod_Agea = ""
                                             Destinazione_Cod_Agea = ""
                                             Qualita_Cod_Agea = ""
                                             Gru_Cod = 0
                                             Dim LogCodificheMancantiSpecie As String = ""
                                             Dim LogCodificheMancantiVarieta As String = ""

                                             objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                                       LogCodificheMancantiVarieta,
                                                                                       Specie_Cod, Varieta_Cod,
                                                                                       Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                                       Specie_Des_Agea, Varieta_Des_Agea,
                                                                                       "",
                                                                                       "",
                                                                                       DataInizio,
                                                                                       Uso_Cod_Agea,
                                                                                       Occupazione_Cod_Agea,
                                                                                       Destinazione_Cod_Agea,
                                                                                       Qualita_Cod_Agea,
                                                                                       objParametri_ServerC,
                                                                                       Gru_Cod)

                                             'Specie_Des_Agea &= " (Cod." & Specie_Cod & ")"

                                             If (Veg_Cod <> 0 AndAlso Cul_Cod = 0) Then
                                                 Dim ObjVarietaAltre As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                                 Cul_Cod = ObjVarietaAltre.VarietaAltre(Veg_Cod, objParametri_ServerC)
                                             End If

                                             If Veg_Cod <> 0 Then
                                                 Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R
                                                 Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_ServerC)
                                                 StatoImpianto_Cod = "102"
                                             End If

                                             If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                                 Dim a = 0
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

                                             Dim sezioneqeury = Sezione
                                             If sezioneqeury = "" Then
                                                 sezioneqeury = "0"
                                             End If
                                             Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                             If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                                 TipoZona = "v"
                                                 TipoZona_Des = "Vulnerabile"
                                             Else
                                                 Dim a = 12
                                             End If

                                             If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                                 TipoZona = "v"
                                                 TipoZona_Des = "Vulnerabile"
                                             Else
                                                 Dim a = 12
                                             End If

                                             DR = DtParticelleC.NewRow

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
                                             DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                                     If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                                             supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                             Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                             DR.Item("sup") = Format(supCatasto, "0.0000")
                                             supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                                             If DR.Item("Veg_Cod") = "0|0" Then
                                                 Dim a = 0
                                             End If
                                             DR.Item("Cul_Cod") = Cul_Cod
                                             DR.Item("Grfi_Cod") = Grfi_Cod
                                             DR.Item("Grva_Cod") = Grva_Cod
                                             DR.Item("Id_Cod") = Id_Cod

                                             'DRUDI
                                             'Recupero descrizione specie, varietà...

                                             If Veg_Cod <> 0 Then

                                                 Dim dtSpecieVegetali As DataTable = objSpec.Leggi(Veg_Cod, 0, "", "", 0, "", "", objParametri_ServerC)
                                                 If Not IsNothing(dtSpecieVegetali) AndAlso dtSpecieVegetali.Rows.Count = 1 Then
                                                     Veg_Des = dtSpecieVegetali.Rows(0).Item("Veg_Des")
                                                 End If
                                             End If

                                             If Id_Cod <> 0 Then

                                                 Dim dtCodiciAna As DataTable = objCodAna.Leggi(Id_Cod, "", "", "", objParametri_ServerC)
                                                 If Not IsNothing(dtCodiciAna) AndAlso dtCodiciAna.Rows.Count = 1 Then
                                                     Veg_Des = dtCodiciAna.Rows(0).Item("descrizione")
                                                 End If
                                             End If

                                             If Cul_Cod <> 0 Then

                                                 Dim dtCultivar As DataTable = objCulti.Leggi(Cul_Cod, Veg_Cod, "", 0, "", "", objParametri_ServerC)
                                                 If Not IsNothing(dtCultivar) AndAlso dtCultivar.Rows.Count = 1 Then
                                                     Cul_Des = dtCultivar.Rows(0).Item("Cul_Des")
                                                 End If
                                             End If

                                             If Grfi_Cod <> 0 Then

                                                 Dim dtGruppoFinalita As DataTable = objGrfi.Leggi(Grfi_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                                 If Not IsNothing(dtGruppoFinalita) AndAlso dtGruppoFinalita.Rows.Count = 1 Then
                                                     Grfi_Des = dtGruppoFinalita.Rows(0).Item("Grfi_Des")
                                                 End If
                                             End If

                                             If Grva_Cod <> 0 Then

                                                 Dim dtGruppoVarietale As DataTable = objGrva.Leggi(Veg_Cod, Grva_Cod, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_ServerC)
                                                 If Not IsNothing(dtGruppoVarietale) AndAlso dtGruppoVarietale.Rows.Count = 1 Then
                                                     Grva_Des = dtGruppoVarietale.Rows(0).Item("Grva_Des")
                                                 End If
                                             End If

                                             DR.Item("veg_des") = Veg_Des
                                             DR.Item("cul_des") = Cul_Des
                                             DR.Item("grfi_des") = Grfi_Des
                                             DR.Item("grva_des") = Grva_Des

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



                                             DR.Item("campo_cod") = 0
                                             DR.Item("campo_des") = ""
                                             'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                                             '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")
                                             DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                             DR.Item("Isola") = ""

                                             DR.Item("CapitolatoPrivato") = ""
                                             DR.Item("CapitolatoPrivato_Des") = ""

                                             DR.Item("Finalita_Concimazione_Impianto") = 0

                                             DR.Item("Cod_Indirizzo") = 0
                                             DR.Item("ind_des") = ""
                                             DR.Item("frz_des") = ""
                                             DR.Item("CAP") = ""
                                             DR.Item("com_des_indirizzo") = ""
                                             DR.Item("pro_cod_indirizzo") = ""
                                             DR.Item("stato_indirizzo") = ""
                                             DR.Item("stato_indirizzo_des") = ""
                                             DR.Item("note_indirizzo") = ""
                                             DR.Item("pro_cod_istat_indirizzo") = ""
                                             DR.Item("com_cod_istat_indirizzo") = ""

                                             DR.Item("Pratiche_Cod") = ""
                                             DR.Item("Pratiche_Des") = ""

                                             DR.Item("KPIN") = ""
                                             DR.Item("Block_Name") = ""

                                             DR.Item("Foral_Cod") = 0
                                             DR.Item("Foral_Des") = ""

                                             DR.Item("Data_Inizio_Portinnesto") = ""

                                             DR.Item("Data_Creazione") = DBNull.Value
                                             DR.Item("Data_Modifica") = DBNull.Value

                                             DR.Item("chiave") = chiave
                                             chiave += 1

                                             DtParticelleC.Rows.Add(DR)

                                             'Else

                                             '    Dim query = " PROV='" & Prov & "' AND COM = '" & Com & "' AND SEZIONE='" & Sezione & "' AND FOGLIO = " & Foglio &
                                             '                        " AND NUMERO = " & Numero & "  AND SUBALTERNO = '" & Subalterno & "' AND Macrouso_Cod = '" & Macrouso_Cod & "' " &
                                             '                        " AND Veg_Cod = '" & Veg_Cod & "|" & Id_Cod & "'  AND cul_Cod = " & Cul_Cod & " "

                                             '    Dim row = DtParticelle.Select(query)
                                             '    If row.Count > 0 Then
                                             '        row(0)("utilizzo_sup") = Decimal.Round(row(0)("utilizzo_sup") + Utilizzo_Sup, 4)
                                             '    End If

                                             'End If
                                         Next

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
                                         Validita_Inizio_Impianto = AGRODATAINIZIO
                                         TipoZona = "n"
                                         TipoZona_Des = "Non Vulnerabile"
                                         'MetodoProduzione_Cod = 1
                                         'MetodoProduzione_Des = "Convenzionale"
                                         Unita_Vitata = "0"
                                         frazionato = 0
                                         unito = 0

                                         'Dpi_Cod = ""
                                         'Reg_Cod = "1"
                                         StatoImpianto_Cod = ""
                                         N = Nothing
                                         P = Nothing
                                         K = Nothing
                                         Data_Semina = AGRODATAINIZIO
                                         Data_Raccolta = AGRODATAFINE
                                         Data_Fioritura = AGRODATAINIZIO
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
                                                                                                        Destinazione_Cod_Agea, Qualita_Cod_Agea, Veg_Des, Cul_Des, objParametri_ServerC)

                                         If Veg_Cod <> 0 Then
                                             Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R
                                             Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_ServerC)
                                             StatoImpianto_Cod = "102"
                                         End If

                                         If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                             Id_Cod = 3078
                                             Veg_Des = "Nessuna mappatura con Gias"
                                         End If

                                         Dim sezioneqeury = Sezione
                                         If sezioneqeury = "" Then
                                             sezioneqeury = "0"
                                         End If
                                         Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                         If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                             TipoZona = "v"
                                             TipoZona_Des = "Vulnerabile"
                                         End If

                                         If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                             TipoZona = "v"
                                             TipoZona_Des = "Vulnerabile"
                                         Else
                                             Dim a = 12
                                         End If

                                         DR = DtParticelleC.NewRow

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
                                         DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                                     If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                                         supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                         Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                         DR.Item("sup") = Format(supCatasto, "0.0000")
                                         supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                                         DR.Item("Veg_Cod") = "0|3262" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
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

                                         DR.Item("campo_cod") = 0
                                         DR.Item("campo_des") = ""

                                         DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                         DR.Item("Isola") = ""

                                         DR.Item("CapitolatoPrivato") = ""
                                         DR.Item("CapitolatoPrivato_Des") = ""

                                         DR.Item("Finalita_Concimazione_Impianto") = 0

                                         DR.Item("Cod_Indirizzo") = 0
                                         DR.Item("ind_des") = ""
                                         DR.Item("frz_des") = ""
                                         DR.Item("CAP") = ""
                                         DR.Item("com_des_indirizzo") = ""
                                         DR.Item("pro_cod_indirizzo") = ""
                                         DR.Item("stato_indirizzo") = ""
                                         DR.Item("stato_indirizzo_des") = ""
                                         DR.Item("note_indirizzo") = ""
                                         DR.Item("pro_cod_istat_indirizzo") = ""
                                         DR.Item("com_cod_istat_indirizzo") = ""

                                         DR.Item("Pratiche_Cod") = ""
                                         DR.Item("Pratiche_Des") = ""

                                         DR.Item("KPIN") = ""
                                         DR.Item("Block_Name") = ""

                                         DR.Item("Foral_Cod") = 0
                                         DR.Item("Foral_Des") = ""

                                         DR.Item("Data_Inizio_Portinnesto") = ""

                                         DR.Item("Data_Creazione") = DBNull.Value
                                         DR.Item("Data_Modifica") = DBNull.Value

                                         DR.Item("chiave") = chiave
                                         chiave += 1

                                         DtParticelleC.Rows.Add(DR)

                                     End If

                                     'End If

                                 Next

                             Else

                                 Dim sezioneqeury = Sezione
                                 If sezioneqeury = "" Then
                                     sezioneqeury = "0"
                                 End If

                                 Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                 If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                     TipoZona = "v"
                                     TipoZona_Des = "Vulnerabile"
                                 End If

                                 If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                     TipoZona = "v"
                                     TipoZona_Des = "Vulnerabile"
                                 Else
                                     Dim a = 12
                                 End If

                                 DR = DtParticelleC.NewRow

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
                                 DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                     If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
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
                                 DR.Item("Veg_Cod") = "0|3262"
                                 DR.Item("Cul_Cod") = 0
                                 DR.Item("Grfi_Cod") = 0
                                 DR.Item("Grva_Cod") = 0
                                 DR.Item("Id_Cod") = 0

                                 DR.Item("veg_des") = "Utilizzo non specificato"
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

                                 DR.Item("campo_cod") = 0
                                 DR.Item("campo_des") = ""

                                 DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                 DR.Item("Isola") = ""

                                 DR.Item("CapitolatoPrivato") = ""
                                 DR.Item("CapitolatoPrivato_Des") = ""

                                 DR.Item("Finalita_Concimazione_Impianto") = 0

                                 DR.Item("Cod_Indirizzo") = 0
                                 DR.Item("ind_des") = ""
                                 DR.Item("frz_des") = ""
                                 DR.Item("CAP") = ""
                                 DR.Item("com_des_indirizzo") = ""
                                 DR.Item("pro_cod_indirizzo") = ""
                                 DR.Item("stato_indirizzo") = ""
                                 DR.Item("stato_indirizzo_des") = ""
                                 DR.Item("note_indirizzo") = ""
                                 DR.Item("pro_cod_istat_indirizzo") = ""
                                 DR.Item("com_cod_istat_indirizzo") = ""

                                 DR.Item("Pratiche_Cod") = ""
                                 DR.Item("Pratiche_Des") = ""

                                 DR.Item("KPIN") = ""
                                 DR.Item("Block_Name") = ""

                                 DR.Item("Foral_Cod") = 0
                                 DR.Item("Foral_Des") = ""

                                 DR.Item("Data_Inizio_Portinnesto") = ""

                                 DR.Item("Data_Creazione") = DBNull.Value
                                 DR.Item("Data_Modifica") = DBNull.Value

                                 DR.Item("chiave") = chiave
                                 chiave += 1

                                 DtParticelleC.Rows.Add(DR)

                             End If
                         End Sub
                         )

        DtParticelle = DtParticelleC

    End Sub


    Private Sub Popola_DtAppezzamenti_daFascicolo_3(ByVal Piva As String,
                                                           ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                                           ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByRef DataInizio As Date,
                                                           ByRef DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByRef Allegati_Documenti_Numero As String,
                                                           ByRef ImportCatasto As Boolean,
                                                           ByRef ASG_Utente_Username As String,
                                                           ByRef ASG_Utente_Password As String,
                                                           ByRef ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)

        'Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

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
        Dim Validita_Inizio_Impianto As Date
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
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
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

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        'Dim strValiditaInizio As String = String.Empty
        'strValiditaInizio = DataInizio.ToShortDateString
        'Dim strValiditaFine As String = String.Empty
        'strValiditaFine = DataFine.ToShortDateString


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
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If

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

        'leggo i comuni x avere le descrizioni poi
        Dim DT_Comuni As New DataTable
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objZoneMetaschema As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
        Dim objSpec = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objCodAna = New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
        Dim objCulti As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objGrfi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
        Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R

        'leggo il sa_cod
        Dim objCentrixPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim Dt_Centri As DataTable
        Dt_Centri = objCentrixPart.LeggiJoinCentriIndirizzi2(0,
                                                                        CStr(Piva),
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        objParametri_Server)

        Dim DT_CentriEO = Dt_Centri.ToExpandoObject()

        Dim ListProvincie = (From t In ISWSTerritorioFS6 Select t.Provincia).Distinct().ToList

        Dim dtZoneGlobal As DataTable = Nothing
        Dim dtParticelleVulnerabiliGlobal As DataTable = Nothing
        For Each provincia In ListProvincie
            If dtZoneGlobal Is Nothing Then
                dtZoneGlobal = objZone.Leggi(-17, provincia, "", "", 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Else
                dtZoneGlobal.Merge(objZone.Leggi(-17, provincia, "", "", 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server))
            End If

            If dtParticelleVulnerabiliGlobal Is Nothing Then
                dtParticelleVulnerabiliGlobal = objZoneMetaschema.Leggi(provincia, "", "", 0, 0, "", 0, "", "", objParametri_Server)
            Else
                dtParticelleVulnerabiliGlobal.Merge(objZoneMetaschema.Leggi(provincia, "", "", 0, 0, "", 0, "", "", objParametri_Server))
            End If

        Next

        Dim dtZoneGlobalEO = dtZoneGlobal.ToExpandoObject()
        Dim dtParticelleVulnerabiliGlobalEO = dtParticelleVulnerabiliGlobal.ToExpandoObject()

        Dim HashMacrousi As New Hashtable
        Dim HashSpecie As New Hashtable
        Dim HashCultivar As New Hashtable
        Dim HashCultivarAltre As New Hashtable
        Dim HashCopertura As New Hashtable
        Dim HashCodiciAnagrafe As New Hashtable
        Dim HashFinalita As New Hashtable
        Dim HashGruppoVarietale As New Hashtable

        Dim i = 0
        For Each territorio In ISWSTerritorioFS6
            i += 1
            'DR = DtParticelle.NewRow

            Prov = Trim(IIf(IsNothing(territorio.Provincia), "", territorio.Provincia))
            Com = Trim(IIf(IsNothing(territorio.Comune), "", territorio.Comune))
            Sezione = Trim(IIf(IsNothing(territorio.Sezione), "", territorio.Sezione))
            Foglio = Trim(IIf(IsNothing(territorio.Foglio), 0, territorio.Foglio))
            strNumero = Trim(IIf(IsNothing(territorio.Particella), 0, territorio.Particella))
            Subalterno = Trim(IIf(IsNothing(territorio.Subalterno), "", territorio.Subalterno))

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
                Subalterno = Trim(Left(NumeroStringa, 3))
            End If

            'Dim strFiltroSezione = " Sezione = '" & Sezione & "'"
            'If Sezione = "" Then
            '    strFiltroSezione = " ( Sezione = '' OR Sezione = '0') "
            'End If

            'Dim strFiltroSubalterno = " Subalterno = '" & Subalterno & "' "
            'If Subalterno = "" Then
            '    strFiltroSubalterno = " Subalterno = '' OR Subalterno = '0' "
            'End If

            'Dim drCentri = Dt_Centri.Select(" Prov = '" & Prov & "' AND " &
            '                                " Com = '" & Com & "' AND " &
            '                                strFiltroSezione & " AND " &
            '                                " Foglio = " & Foglio & " AND " &
            '                                " Numero = " & Numero & " AND " &
            '                                strFiltroSubalterno)
            Dim DT_CentriQuery = DT_CentriEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

            If Sezione = "" Then
                DT_CentriQuery = DT_CentriQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
            Else
                DT_CentriQuery = DT_CentriQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
            End If

            If Subalterno = "" Then
                DT_CentriQuery = DT_CentriQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
            Else
                DT_CentriQuery = DT_CentriQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
            End If

            Dim drFilteredCentri = DT_CentriQuery.ToList

            If Not IsNothing(drFilteredCentri) AndAlso drFilteredCentri.Count > 0 Then
                Sa_Cod = drFilteredCentri(0)("sa_cod")
                Sa_Nome = drFilteredCentri(0)("Sa_Nome")
            ElseIf Not ImportCatasto Then
                If CatastoDaImportare Then
                    ImportCatasto = True
                    DtParticelle.Clear()
                    ImportaCatastoFascicolo(fascicolo,
                                            ISWSTerritorioFS6,
                                            Piva,
                                            fascicolo.CUAA,
                                            True,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            ASG_Utente_Username,
                                            ASG_Utente_Password,
                                            ASG_ProgressivoGIAS)
                    Exit Sub
                End If
                'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                Sa_Cod = 0
                Sa_Nome = "01"
            End If

            Select Case territorio.codiceTipoConduzione
                Case AGEA_Coordinamento.TipoConduzione.Item1
                    TitoloPossesso = 1 'Proprietà
                    TitoloPossessoDes = "Proprieta"
                Case AGEA_Coordinamento.TipoConduzione.Item2
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

            If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                If Inizio_Possesso < AGRODATAINIZIO Then
                    Inizio_Possesso = AGRODATAINIZIO
                End If
            End If
            If territorio.DataFineConduzione IsNot Nothing AndAlso
                                        territorio.DataFineConduzione <> "" AndAlso
                                        territorio.DataFineConduzione <> "99991231" AndAlso
                                        territorio.DataFineConduzione <> "99990101" Then
                Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
            End If

            'DataInizio = Inizio_Possesso
            'DataFine = Fine_Possesso

            supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)

            supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0

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

            strMacrousi = ""
            strUtilizzi = ""



            If territorio.Destinazione IsNot Nothing Then

                For j = 0 To territorio.Destinazione.Length - 1

                    Qualita = territorio.Destinazione(j).codiceQualita

                    strMacrousi = ""
                    strUtilizzi = ""

                    Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                    Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                    MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                    'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
                    '                           objParametri_Server.LogFileName,
                    '                           objParametri_Server.LogDescrizioneUtente,
                    '                           NomeRoutine,
                    '                           MsgOK)

                    'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                    'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                    Dim sMacrousoSup As String = ""
                    If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                        sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                    End If

                    If Not HashMacrousi.Contains(territorio.Destinazione(j).codiceMacrouso) Then
                        strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                     enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                     "", "",
                                                                                     objParametri_Server) '& sMacrousoSup
                        HashMacrousi.Add(territorio.Destinazione(j).codiceMacrouso, strMacrousi)
                    Else
                        strMacrousi = HashMacrousi(territorio.Destinazione(j).codiceMacrouso)
                    End If


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
                    If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                        For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                            strUtilizzi = ""

                            Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                            Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                            Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                            Specie_Des_Agea = ""
                            Varieta_Des_Agea = ""


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
                            Cop_Cod = 0
                            Cop_Des = ""
                            Lotto = ""
                            Resa = 0
                            Num_Piante = 0
                            TRA_Fila = 0
                            SU_Fila = 0
                            Validita_Inizio_Impianto = AGRODATAINIZIO
                            TipoZona = "n"
                            TipoZona_Des = "Non Vulnerabile"

                            Unita_Vitata = "0"
                            frazionato = 0
                            unito = 0

                            StatoImpianto_Cod = ""
                            N = Nothing
                            P = Nothing
                            K = Nothing
                            Data_Semina = AGRODATAINIZIO
                            Data_Raccolta = AGRODATAFINE
                            Data_Fioritura = AGRODATAINIZIO
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

                            Uso_Cod_Agea = ""
                            Occupazione_Cod_Agea = ""
                            Destinazione_Cod_Agea = ""
                            Qualita_Cod_Agea = ""
                            Gru_Cod = 0
                            Dim LogCodificheMancantiSpecie As String = ""
                            Dim LogCodificheMancantiVarieta As String = ""

                            objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                      LogCodificheMancantiVarieta,
                                                                      Specie_Cod, Varieta_Cod,
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

                            'Specie_Des_Agea &= " (Cod." & Specie_Cod & ")"

                            If (Veg_Cod <> 0 AndAlso Cul_Cod = 0) Then
                                If Not HashCultivarAltre.Contains(Veg_Cod) Then
                                    Dim ObjVarietaAltre As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                    Cul_Cod = ObjVarietaAltre.VarietaAltre(Veg_Cod, objParametri_Server)
                                    HashCultivarAltre.Add(Veg_Cod, Cul_Cod)
                                Else
                                    Cul_Cod = HashCultivarAltre(Veg_Cod)
                                End If

                            End If

                            If Veg_Cod <> 0 Then
                                If Not HashCopertura.Contains(Veg_Cod) Then
                                    Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)
                                    HashCopertura.Add(Veg_Cod, Cop_Cod)
                                Else
                                    Cop_Cod = HashCopertura(Veg_Cod)
                                End If


                                StatoImpianto_Cod = "102"
                            End If

                            If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                Dim a = 0
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

                            Dim sezioneqeury = Sezione
                            If sezioneqeury = "" Then
                                sezioneqeury = "0"
                            End If
                            'Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            Dim dtZoneQuery = dtZoneGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                            If Sezione = "" Then
                                dtZoneQuery = dtZoneQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                            Else
                                dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                            End If

                            If Subalterno = "" Then
                                dtZoneQuery = dtZoneQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                            Else
                                dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                            End If

                            Dim drFilteredZone = dtZoneQuery.ToList
                            If drFilteredZone IsNot Nothing AndAlso drFilteredZone.Count > 0 Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            Else
                                Dim a = 12
                            End If


                            Dim dtVulnerabiliQuery = dtParticelleVulnerabiliGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                            If Sezione = "" Then
                                dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                            Else
                                dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                            End If

                            If Subalterno = "" Then
                                dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                            Else
                                dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                            End If

                            Dim drVulnerabiliZone = dtVulnerabiliQuery.ToList
                            If drVulnerabiliZone.Count > 0 Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            Else
                                Dim a = 12
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
                            DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                            supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                            DR.Item("sup") = Format(supCatasto, "0.0000")
                            supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                            If DR.Item("Veg_Cod") = "0|0" Then
                                Dim a = 0
                            End If
                            DR.Item("Cul_Cod") = Cul_Cod
                            DR.Item("Grfi_Cod") = Grfi_Cod
                            DR.Item("Grva_Cod") = Grva_Cod
                            DR.Item("Id_Cod") = Id_Cod

                            'DRUDI
                            'Recupero descrizione specie, varietà...

                            If Veg_Cod <> 0 Then
                                If Not HashSpecie.ContainsKey(Veg_Cod) Then
                                    Dim dtSpecieVegetali As DataTable = objSpec.Leggi(Veg_Cod, 0, "", "", 0, "", "", objParametri_Server)
                                    If Not IsNothing(dtSpecieVegetali) AndAlso dtSpecieVegetali.Rows.Count = 1 Then
                                        Veg_Des = dtSpecieVegetali.Rows(0).Item("Veg_Des")
                                    End If
                                    HashSpecie.Add(Veg_Cod, Veg_Des)
                                Else
                                    Veg_Des = HashSpecie(Veg_Cod)
                                End If

                            End If

                            If Id_Cod <> 0 Then
                                If Not HashCodiciAnagrafe.Contains(Id_Cod) Then
                                    Dim dtCodiciAna As DataTable = objCodAna.Leggi(Id_Cod, "", "", "", objParametri_Server)
                                    If Not IsNothing(dtCodiciAna) AndAlso dtCodiciAna.Rows.Count = 1 Then
                                        Veg_Des = dtCodiciAna.Rows(0).Item("descrizione")
                                    End If
                                    HashCodiciAnagrafe.Add(Id_Cod, Veg_Des)
                                Else
                                    Veg_Des = HashCodiciAnagrafe(Id_Cod)
                                End If

                            End If

                            If Cul_Cod <> 0 Then

                                If Not HashCultivar.Contains(Cul_Cod) Then
                                    Dim dtCultivar As DataTable = objCulti.Leggi(Cul_Cod, Veg_Cod, "", 0, "", "", objParametri_Server)
                                    If Not IsNothing(dtCultivar) AndAlso dtCultivar.Rows.Count = 1 Then
                                        Cul_Des = dtCultivar.Rows(0).Item("Cul_Des")
                                    End If
                                    HashCultivar.Add(Cul_Cod, Cul_Des)
                                Else
                                    Cul_Des = HashCultivar(Cul_Cod)
                                End If


                            End If

                            If Grfi_Cod <> 0 Then
                                If Not HashFinalita.Contains(Veg_Cod & "_" & Grfi_Cod) Then
                                    Dim dtGruppoFinalita As DataTable = objGrfi.Leggi(Grfi_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                    If Not IsNothing(dtGruppoFinalita) AndAlso dtGruppoFinalita.Rows.Count = 1 Then
                                        Grfi_Des = dtGruppoFinalita.Rows(0).Item("Grfi_Des")
                                    End If
                                    HashFinalita.Add(Veg_Cod & "_" & Grfi_Cod, Grfi_Des)
                                Else
                                    Grfi_Des = HashFinalita(Veg_Cod & "_" & Grfi_Cod)
                                End If

                            End If

                            If Grva_Cod <> 0 Then
                                If Not HashGruppoVarietale.Contains(Veg_Cod & "_" & Grva_Cod) Then
                                    Dim dtGruppoVarietale As DataTable = objGrva.Leggi(Veg_Cod, Grva_Cod, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)
                                    If Not IsNothing(dtGruppoVarietale) AndAlso dtGruppoVarietale.Rows.Count = 1 Then
                                        Grva_Des = dtGruppoVarietale.Rows(0).Item("Grva_Des")
                                    End If
                                    HashGruppoVarietale.Add(Veg_Cod & "_" & Grva_Cod, Grva_Des)
                                Else
                                    Grva_Des = HashGruppoVarietale(Veg_Cod & "_" & Grva_Cod)
                                End If
                            End If

                            DR.Item("veg_des") = Veg_Des
                            DR.Item("cul_des") = Cul_Des
                            DR.Item("grfi_des") = Grfi_Des
                            DR.Item("grva_des") = Grva_Des

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



                            DR.Item("campo_cod") = 0
                            DR.Item("campo_des") = ""
                            'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                            '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")
                            DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                            DR.Item("Isola") = ""

                            DR.Item("CapitolatoPrivato") = ""
                            DR.Item("CapitolatoPrivato_Des") = ""

                            DR.Item("Finalita_Concimazione_Impianto") = 0

                            DR.Item("Cod_Indirizzo") = 0
                            DR.Item("ind_des") = ""
                            DR.Item("frz_des") = ""
                            DR.Item("CAP") = ""
                            DR.Item("com_des_indirizzo") = ""
                            DR.Item("pro_cod_indirizzo") = ""
                            DR.Item("stato_indirizzo") = ""
                            DR.Item("stato_indirizzo_des") = ""
                            DR.Item("note_indirizzo") = ""
                            DR.Item("pro_cod_istat_indirizzo") = ""
                            DR.Item("com_cod_istat_indirizzo") = ""

                            DR.Item("Pratiche_Cod") = ""
                            DR.Item("Pratiche_Des") = ""

                            DR.Item("KPIN") = ""
                            DR.Item("Block_Name") = ""

                            DR.Item("Foral_Cod") = 0
                            DR.Item("Foral_Des") = ""

                            DR.Item("Data_Inizio_Portinnesto") = ""

                            DR.Item("Data_Creazione") = DBNull.Value
                            DR.Item("Data_Modifica") = DBNull.Value

                            DR.Item("chiave") = chiave
                            chiave += 1

                            DtParticelle.Rows.Add(DR)

                            'Else

                            '    Dim query = " PROV='" & Prov & "' AND COM = '" & Com & "' AND SEZIONE='" & Sezione & "' AND FOGLIO = " & Foglio &
                            '                        " AND NUMERO = " & Numero & "  AND SUBALTERNO = '" & Subalterno & "' AND Macrouso_Cod = '" & Macrouso_Cod & "' " &
                            '                        " AND Veg_Cod = '" & Veg_Cod & "|" & Id_Cod & "'  AND cul_Cod = " & Cul_Cod & " "

                            '    Dim row = DtParticelle.Select(query)
                            '    If row.Count > 0 Then
                            '        row(0)("utilizzo_sup") = Decimal.Round(row(0)("utilizzo_sup") + Utilizzo_Sup, 4)
                            '    End If

                            'End If
                        Next

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
                        Validita_Inizio_Impianto = AGRODATAINIZIO
                        TipoZona = "n"
                        TipoZona_Des = "Non Vulnerabile"
                        'MetodoProduzione_Cod = 1
                        'MetodoProduzione_Des = "Convenzionale"
                        Unita_Vitata = "0"
                        frazionato = 0
                        unito = 0

                        'Dpi_Cod = ""
                        'Reg_Cod = "1"
                        StatoImpianto_Cod = ""
                        N = Nothing
                        P = Nothing
                        K = Nothing
                        Data_Semina = AGRODATAINIZIO
                        Data_Raccolta = AGRODATAFINE
                        Data_Fioritura = AGRODATAINIZIO
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



                        objUtilizzi.Specie_e_Varieta_Gias_Da_Macrouso_Agea(LogCodificheMancantiSpecie, LogCodificheMancantiVarieta,
                                                                                       Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea, Veg_Cod, Cul_Cod,
                                                                                       Grfi_Cod, Grva_Cod, Id_Cod, Uso_Cod_Agea, Occupazione_Cod_Agea,
                                                                                       Destinazione_Cod_Agea, Qualita_Cod_Agea, Veg_Des, Cul_Des, objParametri_Server)

                        If Veg_Cod <> 0 Then

                            Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)
                            StatoImpianto_Cod = "102"
                        End If

                        If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                            Id_Cod = 3078
                            Veg_Des = "Nessuna mappatura con Gias"
                        End If

                        Dim sezioneqeury = Sezione
                        If sezioneqeury = "" Then
                            sezioneqeury = "0"
                        End If
                        Dim dtZoneQuery = dtZoneGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            dtZoneQuery = dtZoneQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                        Else
                            dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            dtZoneQuery = dtZoneQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                        Else
                            dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        Dim drFilteredZone = dtZoneQuery.ToList
                        If drFilteredZone IsNot Nothing AndAlso drFilteredZone.Count > 0 Then
                            TipoZona = "v"
                            TipoZona_Des = "Vulnerabile"
                        Else
                            Dim a = 12
                        End If


                        Dim dtVulnerabiliQuery = dtParticelleVulnerabiliGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                        Else
                            dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                        Else
                            dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        Dim drVulnerabiliZone = dtVulnerabiliQuery.ToList
                        If drVulnerabiliZone.Count > 0 Then
                            TipoZona = "v"
                            TipoZona_Des = "Vulnerabile"
                        Else
                            Dim a = 12
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
                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                        supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                        DR.Item("sup") = Format(supCatasto, "0.0000")
                        supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                        DR.Item("Veg_Cod") = "0|3262" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
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

                        DR.Item("campo_cod") = 0
                        DR.Item("campo_des") = ""

                        DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                        DR.Item("Isola") = ""

                        DR.Item("CapitolatoPrivato") = ""
                        DR.Item("CapitolatoPrivato_Des") = ""

                        DR.Item("Finalita_Concimazione_Impianto") = 0

                        DR.Item("Cod_Indirizzo") = 0
                        DR.Item("ind_des") = ""
                        DR.Item("frz_des") = ""
                        DR.Item("CAP") = ""
                        DR.Item("com_des_indirizzo") = ""
                        DR.Item("pro_cod_indirizzo") = ""
                        DR.Item("stato_indirizzo") = ""
                        DR.Item("stato_indirizzo_des") = ""
                        DR.Item("note_indirizzo") = ""
                        DR.Item("pro_cod_istat_indirizzo") = ""
                        DR.Item("com_cod_istat_indirizzo") = ""

                        DR.Item("Pratiche_Cod") = ""
                        DR.Item("Pratiche_Des") = ""

                        DR.Item("KPIN") = ""
                        DR.Item("Block_Name") = ""

                        DR.Item("Foral_Cod") = 0
                        DR.Item("Foral_Des") = ""

                        DR.Item("Data_Inizio_Portinnesto") = ""

                        DR.Item("Data_Creazione") = DBNull.Value
                        DR.Item("Data_Modifica") = DBNull.Value

                        DR.Item("chiave") = chiave
                        chiave += 1

                        DtParticelle.Rows.Add(DR)

                    End If

                    'End If

                Next

            Else

                Dim sezioneqeury = Sezione
                If sezioneqeury = "" Then
                    sezioneqeury = "0"
                End If

                Dim dtZoneQuery = dtZoneGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                If Sezione = "" Then
                    dtZoneQuery = dtZoneQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                Else
                    dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                End If

                If Subalterno = "" Then
                    dtZoneQuery = dtZoneQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                Else
                    dtZoneQuery = dtZoneQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                End If

                Dim drFilteredZone = dtZoneQuery.ToList
                If drFilteredZone IsNot Nothing AndAlso drFilteredZone.Count > 0 Then
                    TipoZona = "v"
                    TipoZona_Des = "Vulnerabile"
                Else
                    Dim a = 12
                End If


                Dim dtVulnerabiliQuery = dtParticelleVulnerabiliGlobalEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                If Sezione = "" Then
                    dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SEZIONE").Equals("") OrElse c("SEZIONE").Equals("0"))
                Else
                    dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                End If

                If Subalterno = "" Then
                    dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) c("SUBALTERNO").Equals("") OrElse c("SUBALTERNO").Equals("0"))
                Else
                    dtVulnerabiliQuery = dtVulnerabiliQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                End If

                Dim drVulnerabiliZone = dtVulnerabiliQuery.ToList
                If drVulnerabiliZone.Count > 0 Then
                    TipoZona = "v"
                    TipoZona_Des = "Vulnerabile"
                Else
                    Dim a = 12
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
                DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
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
                DR.Item("Veg_Cod") = "0|3262"
                DR.Item("Cul_Cod") = 0
                DR.Item("Grfi_Cod") = 0
                DR.Item("Grva_Cod") = 0
                DR.Item("Id_Cod") = 0

                DR.Item("veg_des") = "Utilizzo non specificato"
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

                DR.Item("campo_cod") = 0
                DR.Item("campo_des") = ""

                DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                DR.Item("Isola") = ""

                DR.Item("CapitolatoPrivato") = ""
                DR.Item("CapitolatoPrivato_Des") = ""

                DR.Item("Finalita_Concimazione_Impianto") = 0

                DR.Item("Cod_Indirizzo") = 0
                DR.Item("ind_des") = ""
                DR.Item("frz_des") = ""
                DR.Item("CAP") = ""
                DR.Item("com_des_indirizzo") = ""
                DR.Item("pro_cod_indirizzo") = ""
                DR.Item("stato_indirizzo") = ""
                DR.Item("stato_indirizzo_des") = ""
                DR.Item("note_indirizzo") = ""
                DR.Item("pro_cod_istat_indirizzo") = ""
                DR.Item("com_cod_istat_indirizzo") = ""

                DR.Item("Pratiche_Cod") = ""
                DR.Item("Pratiche_Des") = ""

                DR.Item("KPIN") = ""
                DR.Item("Block_Name") = ""

                DR.Item("Foral_Cod") = 0
                DR.Item("Foral_Des") = ""

                DR.Item("Data_Inizio_Portinnesto") = ""

                DR.Item("Data_Creazione") = DBNull.Value
                DR.Item("Data_Modifica") = DBNull.Value

                DR.Item("chiave") = chiave
                chiave += 1

                DtParticelle.Rows.Add(DR)

            End If

        Next

    End Sub


    Private Sub Popola_DtAppezzamenti_daFascicolo_3_Parallel2(ByVal Piva As String,
                                                           ByVal fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                                           ByVal ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByVal DataInizio As Date,
                                                           ByVal DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByVal objParametri_Server As AgronicaCoreParametri,
                                                           ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                           ByVal Allegati_Documenti_Numero As String,
                                                           ByVal ImportCatasto As Boolean,
                                                           ByVal ASG_Utente_Username As String,
                                                           ByVal ASG_Utente_Password As String,
                                                           ByVal ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)

        'Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

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
        Dim Validita_Inizio_Impianto As Date
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
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
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

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        'Dim strValiditaInizio As String = String.Empty
        'strValiditaInizio = DataInizio.ToShortDateString
        'Dim strValiditaFine As String = String.Empty
        'strValiditaFine = DataFine.ToShortDateString


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
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If

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

        'leggo i comuni x avere le descrizioni poi
        Dim DT_Comuni As New DataTable
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objZoneMetaschema As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
        Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
        Dim objSpec = New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objCodAna = New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
        Dim objCulti As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objGrfi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
        Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R
        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
        Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R

        'leggo il sa_cod
        Dim objCentrixPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim Dt_Centri As DataTable
        Dt_Centri = objCentrixPart.LeggiJoinCentriIndirizzi2(0,
                                                                        CStr(Piva),
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        "",
                                                                        "",
                                                                        objParametri_Server)


        Dim _HashMacrousi As New Hashtable
        Dim HashMacrousi = Hashtable.Synchronized(_HashMacrousi)

        Dim _HashSpecie As New Hashtable
        Dim HashSpecie = Hashtable.Synchronized(_HashSpecie)

        Dim _HashCultivar As New Hashtable
        Dim HashCultivar = Hashtable.Synchronized(_HashCultivar)

        Dim _HashCultivarAltre As New Hashtable
        Dim HashCultivarAltre = Hashtable.Synchronized(_HashCultivarAltre)

        Dim _HashCopertura As New Hashtable
        Dim HashCopertura = Hashtable.Synchronized(_HashCopertura)

        Dim _HashCodiciAnagrafe As New Hashtable
        Dim HashCodiciAnagrafe = Hashtable.Synchronized(_HashCodiciAnagrafe)

        Dim _HashFinalita As New Hashtable
        Dim HashFinalita = Hashtable.Synchronized(_HashFinalita)

        Dim _HashGruppoVarietale As New Hashtable
        Dim HashGruppoVarietale = Hashtable.Synchronized(_HashGruppoVarietale)


        Dim rowList As New Concurrent.ConcurrentQueue(Of DataRow)
        Dim dtParticelleC = DtParticelle.Clone
        Dim processori = Environment.ProcessorCount
        Parallel.ForEach(ISWSTerritorioFS6,
                         New ParallelOptions With {.MaxDegreeOfParallelism = processori},
                         Sub(territorio As AGEA_Coordinamento.ISWSTerritorio1, state As ParallelLoopState)
                             'DR = DtParticelle.NewRow
                             Dim objParametri_ServerC = objParametri_Server.CreateDeepCopy(objParametri_Server)
                             Dim objParametri_UtentiC = objParametri_Utenti.CreateDeepCopy(objParametri_Utenti)

                             Prov = IIf(IsNothing(territorio.Provincia), "", territorio.Provincia)
                             Com = IIf(IsNothing(territorio.Comune), "", territorio.Comune)
                             Sezione = IIf(IsNothing(territorio.Sezione), "", territorio.Sezione)
                             Foglio = IIf(IsNothing(territorio.Foglio), 0, territorio.Foglio)
                             strNumero = IIf(IsNothing(territorio.Particella), 0, territorio.Particella)
                             Subalterno = IIf(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

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
                             Dim strFiltroSezione = " Sezione = '" & Sezione & "'"
                             If Sezione = "" Then
                                 strFiltroSezione = " ( Sezione = '' OR Sezione = '0') "
                             End If

                             Dim strFiltroSubalterno = " Subalterno = '" & Subalterno & "' "
                             If Subalterno = "" Then
                                 strFiltroSubalterno = " Subalterno = '' OR Subalterno = '0' "
                             End If

                             Dim drCentri = Dt_Centri.Select(" Prov = '" & Prov & "' AND " &
                                        " Com = '" & Com & "' AND " &
                                        strFiltroSezione & " AND " &
                                        " Foglio = " & Foglio & " AND " &
                                        " Numero = " & Numero & " AND " &
                                        strFiltroSubalterno)
                             If Not IsNothing(drCentri) AndAlso drCentri.Length > 0 Then
                                 Sa_Cod = drCentri(0).Item("sa_cod")
                                 Sa_Nome = drCentri(0).Item("sa_nome")
                             ElseIf Not ImportCatasto Then
                                 If CatastoDaImportare Then
                                     ImportCatasto = True
                                     dtParticelleC.Clear()
                                     ImportaCatastoFascicolo(fascicolo,
                                        ISWSTerritorioFS6,
                                        Piva,
                                        fascicolo.CUAA,
                                        True,
                                        objParametri_ServerC,
                                        objParametri_UtentiC,
                                        ASG_Utente_Username,
                                        ASG_Utente_Password,
                                        ASG_ProgressivoGIAS)
                                     Exit Sub
                                 End If
                                 'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                                 Sa_Cod = 0
                                 Sa_Nome = "01"
                             End If

                             Select Case territorio.codiceTipoConduzione
                                 Case AGEA_Coordinamento.TipoConduzione.Item1
                                     TitoloPossesso = 1 'Proprietà
                                     TitoloPossessoDes = "Proprieta"
                                 Case AGEA_Coordinamento.TipoConduzione.Item2
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

                             If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                                 Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                                 If Inizio_Possesso < AGRODATAINIZIO Then
                                     Inizio_Possesso = AGRODATAINIZIO
                                 End If
                             End If
                             If territorio.DataFineConduzione IsNot Nothing AndAlso
                                    territorio.DataFineConduzione <> "" AndAlso
                                    territorio.DataFineConduzione <> "99991231" AndAlso
                                    territorio.DataFineConduzione <> "99990101" Then
                                 Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
                             End If

                             'DataInizio = Inizio_Possesso
                             'DataFine = Fine_Possesso

                             supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                             Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)

                             supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0

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

                             strMacrousi = ""
                             strUtilizzi = ""



                             If territorio.Destinazione IsNot Nothing Then

                                 For j = 0 To territorio.Destinazione.Length - 1

                                     Qualita = territorio.Destinazione(j).codiceQualita

                                     strMacrousi = ""
                                     strUtilizzi = ""

                                     Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                                     Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                                     MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                                     'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
                                     '                           objParametri_Server.LogFileName,
                                     '                           objParametri_Server.LogDescrizioneUtente,
                                     '                           NomeRoutine,
                                     '                           MsgOK)

                                     'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                                     'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                                     Dim sMacrousoSup As String = ""
                                     If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                                         sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                                     End If

                                     If Not HashMacrousi.Contains(territorio.Destinazione(j).codiceMacrouso) Then
                                         strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                                      "", "",
                                                                                                      objParametri_ServerC) '& sMacrousoSup
                                         HashMacrousi.Add(territorio.Destinazione(j).codiceMacrouso, strMacrousi)
                                     Else
                                         strMacrousi = HashMacrousi(territorio.Destinazione(j).codiceMacrouso)
                                     End If


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
                                     If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                                         For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                                             strUtilizzi = ""

                                             Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                                             Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                                             Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                                             Specie_Des_Agea = ""
                                             Varieta_Des_Agea = ""


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
                                             Cop_Cod = 0
                                             Cop_Des = ""
                                             Lotto = ""
                                             Resa = 0
                                             Num_Piante = 0
                                             TRA_Fila = 0
                                             SU_Fila = 0
                                             Validita_Inizio_Impianto = AGRODATAINIZIO
                                             TipoZona = "n"
                                             TipoZona_Des = "Non Vulnerabile"

                                             Unita_Vitata = "0"
                                             frazionato = 0
                                             unito = 0

                                             StatoImpianto_Cod = ""
                                             N = Nothing
                                             P = Nothing
                                             K = Nothing
                                             Data_Semina = AGRODATAINIZIO
                                             Data_Raccolta = AGRODATAFINE
                                             Data_Fioritura = AGRODATAINIZIO
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

                                             Uso_Cod_Agea = ""
                                             Occupazione_Cod_Agea = ""
                                             Destinazione_Cod_Agea = ""
                                             Qualita_Cod_Agea = ""
                                             Gru_Cod = 0
                                             Dim LogCodificheMancantiSpecie As String = ""
                                             Dim LogCodificheMancantiVarieta As String = ""

                                             objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                  LogCodificheMancantiVarieta,
                                                                  Specie_Cod, Varieta_Cod,
                                                                  Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                  Specie_Des_Agea, Varieta_Des_Agea,
                                                                  "",
                                                                  "",
                                                                  DataInizio,
                                                                  Uso_Cod_Agea,
                                                                  Occupazione_Cod_Agea,
                                                                  Destinazione_Cod_Agea,
                                                                  Qualita_Cod_Agea,
                                                                  objParametri_ServerC,
                                                                  Gru_Cod)

                                             'Specie_Des_Agea &= " (Cod." & Specie_Cod & ")"

                                             If (Veg_Cod <> 0 AndAlso Cul_Cod = 0) Then
                                                 If Not HashCultivarAltre.Contains(Veg_Cod) Then
                                                     Dim ObjVarietaAltre As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                                     Cul_Cod = ObjVarietaAltre.VarietaAltre(Veg_Cod, objParametri_ServerC)
                                                     HashCultivarAltre.Add(Veg_Cod, Cul_Cod)
                                                 Else
                                                     Cul_Cod = HashCultivarAltre(Veg_Cod)
                                                 End If

                                             End If

                                             If Veg_Cod <> 0 Then
                                                 If Not HashCopertura.Contains(Veg_Cod) Then
                                                     Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_ServerC)
                                                     HashCopertura.Add(Veg_Cod, Cop_Cod)
                                                 Else
                                                     Cop_Cod = HashCopertura(Veg_Cod)
                                                 End If


                                                 StatoImpianto_Cod = "102"
                                             End If

                                             If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                                 Dim a = 0
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

                                             Dim sezioneqeury = Sezione
                                             If sezioneqeury = "" Then
                                                 sezioneqeury = "0"
                                             End If
                                             Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                             If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                                 TipoZona = "v"
                                                 TipoZona_Des = "Vulnerabile"
                                             Else
                                                 Dim a = 12
                                             End If

                                             If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                                 TipoZona = "v"
                                                 TipoZona_Des = "Vulnerabile"
                                             Else
                                                 Dim a = 12
                                             End If

                                             DR = dtParticelleC.NewRow

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
                                             DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                                             supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                             Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                             DR.Item("sup") = Format(supCatasto, "0.0000")
                                             supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                                             If DR.Item("Veg_Cod") = "0|0" Then
                                                 Dim a = 0
                                             End If
                                             DR.Item("Cul_Cod") = Cul_Cod
                                             DR.Item("Grfi_Cod") = Grfi_Cod
                                             DR.Item("Grva_Cod") = Grva_Cod
                                             DR.Item("Id_Cod") = Id_Cod

                                             'DRUDI
                                             'Recupero descrizione specie, varietà...

                                             If Veg_Cod <> 0 Then
                                                 If Not HashSpecie.ContainsKey(Veg_Cod) Then
                                                     Dim dtSpecieVegetali As DataTable = objSpec.Leggi(Veg_Cod, 0, "", "", 0, "", "", objParametri_ServerC)
                                                     If Not IsNothing(dtSpecieVegetali) AndAlso dtSpecieVegetali.Rows.Count = 1 Then
                                                         Veg_Des = dtSpecieVegetali.Rows(0).Item("Veg_Des")
                                                     End If
                                                     HashSpecie.Add(Veg_Cod, Veg_Des)
                                                 Else
                                                     Veg_Des = HashSpecie(Veg_Cod)
                                                 End If

                                             End If

                                             If Id_Cod <> 0 Then
                                                 If Not HashCodiciAnagrafe.Contains(Id_Cod) Then
                                                     Dim dtCodiciAna As DataTable = objCodAna.Leggi(Id_Cod, "", "", "", objParametri_ServerC)
                                                     If Not IsNothing(dtCodiciAna) AndAlso dtCodiciAna.Rows.Count = 1 Then
                                                         Veg_Des = dtCodiciAna.Rows(0).Item("descrizione")
                                                     End If
                                                     HashCodiciAnagrafe.Add(Id_Cod, Veg_Des)
                                                 Else
                                                     Veg_Des = HashCodiciAnagrafe(Id_Cod)
                                                 End If

                                             End If

                                             If Cul_Cod <> 0 Then

                                                 If Not HashCultivar.Contains(Cul_Cod) Then
                                                     Dim dtCultivar As DataTable = objCulti.Leggi(Cul_Cod, Veg_Cod, "", 0, "", "", objParametri_ServerC)
                                                     If Not IsNothing(dtCultivar) AndAlso dtCultivar.Rows.Count = 1 Then
                                                         Cul_Des = dtCultivar.Rows(0).Item("Cul_Des")
                                                     End If
                                                     HashCultivar.Add(Cul_Cod, Cul_Des)
                                                 Else
                                                     Cul_Des = HashCultivar(Cul_Cod)
                                                 End If


                                             End If

                                             If Grfi_Cod <> 0 Then
                                                 If Not HashFinalita.Contains(Veg_Cod & "_" & Grfi_Cod) Then
                                                     Dim dtGruppoFinalita As DataTable = objGrfi.Leggi(Grfi_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                                     If Not IsNothing(dtGruppoFinalita) AndAlso dtGruppoFinalita.Rows.Count = 1 Then
                                                         Grfi_Des = dtGruppoFinalita.Rows(0).Item("Grfi_Des")
                                                     End If
                                                     HashFinalita.Add(Veg_Cod & "_" & Grfi_Cod, Grfi_Des)
                                                 Else
                                                     Grfi_Des = HashFinalita(Veg_Cod & "_" & Grfi_Cod)
                                                 End If

                                             End If

                                             If Grva_Cod <> 0 Then
                                                 If Not HashGruppoVarietale.Contains(Veg_Cod & "_" & Grva_Cod) Then
                                                     Dim dtGruppoVarietale As DataTable = objGrva.Leggi(Veg_Cod, Grva_Cod, "", enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_ServerC)
                                                     If Not IsNothing(dtGruppoVarietale) AndAlso dtGruppoVarietale.Rows.Count = 1 Then
                                                         Grva_Des = dtGruppoVarietale.Rows(0).Item("Grva_Des")
                                                     End If
                                                     HashGruppoVarietale.Add(Veg_Cod & "_" & Grva_Cod, Grva_Des)
                                                 Else
                                                     Grva_Des = HashGruppoVarietale(Veg_Cod & "_" & Grva_Cod)
                                                 End If
                                             End If

                                             DR.Item("veg_des") = Veg_Des
                                             DR.Item("cul_des") = Cul_Des
                                             DR.Item("grfi_des") = Grfi_Des
                                             DR.Item("grva_des") = Grva_Des

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



                                             DR.Item("campo_cod") = 0
                                             DR.Item("campo_des") = ""
                                             'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                                             '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")
                                             DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                             DR.Item("Isola") = ""

                                             DR.Item("CapitolatoPrivato") = ""
                                             DR.Item("CapitolatoPrivato_Des") = ""

                                             DR.Item("Finalita_Concimazione_Impianto") = 0

                                             DR.Item("Cod_Indirizzo") = 0
                                             DR.Item("ind_des") = ""
                                             DR.Item("frz_des") = ""
                                             DR.Item("CAP") = ""
                                             DR.Item("com_des_indirizzo") = ""
                                             DR.Item("pro_cod_indirizzo") = ""
                                             DR.Item("stato_indirizzo") = ""
                                             DR.Item("stato_indirizzo_des") = ""
                                             DR.Item("note_indirizzo") = ""
                                             DR.Item("pro_cod_istat_indirizzo") = ""
                                             DR.Item("com_cod_istat_indirizzo") = ""

                                             DR.Item("Pratiche_Cod") = ""
                                             DR.Item("Pratiche_Des") = ""

                                             DR.Item("KPIN") = ""
                                             DR.Item("Block_Name") = ""

                                             DR.Item("Foral_Cod") = 0
                                             DR.Item("Foral_Des") = ""

                                             DR.Item("Data_Inizio_Portinnesto") = ""

                                             DR.Item("Data_Creazione") = DBNull.Value
                                             DR.Item("Data_Modifica") = DBNull.Value

                                             DR.Item("chiave") = chiave
                                             chiave += 1

                                             'dtParticelleC.Rows.Add(DR)
                                             rowList.Enqueue(DR)
                                             'Else

                                             '    Dim query = " PROV='" & Prov & "' AND COM = '" & Com & "' AND SEZIONE='" & Sezione & "' AND FOGLIO = " & Foglio &
                                             '                        " AND NUMERO = " & Numero & "  AND SUBALTERNO = '" & Subalterno & "' AND Macrouso_Cod = '" & Macrouso_Cod & "' " &
                                             '                        " AND Veg_Cod = '" & Veg_Cod & "|" & Id_Cod & "'  AND cul_Cod = " & Cul_Cod & " "

                                             '    Dim row = DtParticelle.Select(query)
                                             '    If row.Count > 0 Then
                                             '        row(0)("utilizzo_sup") = Decimal.Round(row(0)("utilizzo_sup") + Utilizzo_Sup, 4)
                                             '    End If

                                             'End If
                                         Next

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
                                         Validita_Inizio_Impianto = AGRODATAINIZIO
                                         TipoZona = "n"
                                         TipoZona_Des = "Non Vulnerabile"
                                         'MetodoProduzione_Cod = 1
                                         'MetodoProduzione_Des = "Convenzionale"
                                         Unita_Vitata = "0"
                                         frazionato = 0
                                         unito = 0

                                         'Dpi_Cod = ""
                                         'Reg_Cod = "1"
                                         StatoImpianto_Cod = ""
                                         N = Nothing
                                         P = Nothing
                                         K = Nothing
                                         Data_Semina = AGRODATAINIZIO
                                         Data_Raccolta = AGRODATAFINE
                                         Data_Fioritura = AGRODATAINIZIO
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



                                         objUtilizzi.Specie_e_Varieta_Gias_Da_Macrouso_Agea(LogCodificheMancantiSpecie, LogCodificheMancantiVarieta,
                                                                                   Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea, Veg_Cod, Cul_Cod,
                                                                                   Grfi_Cod, Grva_Cod, Id_Cod, Uso_Cod_Agea, Occupazione_Cod_Agea,
                                                                                   Destinazione_Cod_Agea, Qualita_Cod_Agea, Veg_Des, Cul_Des, objParametri_ServerC)

                                         If Veg_Cod <> 0 Then

                                             Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_ServerC)
                                             StatoImpianto_Cod = "102"
                                         End If

                                         If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                             Id_Cod = 3078
                                             Veg_Des = "Nessuna mappatura con Gias"
                                         End If

                                         Dim sezioneqeury = Sezione
                                         If sezioneqeury = "" Then
                                             sezioneqeury = "0"
                                         End If
                                         Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                         If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                             TipoZona = "v"
                                             TipoZona_Des = "Vulnerabile"
                                         End If

                                         If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                             TipoZona = "v"
                                             TipoZona_Des = "Vulnerabile"
                                         Else
                                             Dim a = 12
                                         End If

                                         DR = dtParticelleC.NewRow

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
                                         DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                                         supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                         Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                         DR.Item("sup") = Format(supCatasto, "0.0000")
                                         supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                                         DR.Item("Veg_Cod") = "0|3262" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
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

                                         DR.Item("campo_cod") = 0
                                         DR.Item("campo_des") = ""

                                         DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                         DR.Item("Isola") = ""

                                         DR.Item("CapitolatoPrivato") = ""
                                         DR.Item("CapitolatoPrivato_Des") = ""

                                         DR.Item("Finalita_Concimazione_Impianto") = 0

                                         DR.Item("Cod_Indirizzo") = 0
                                         DR.Item("ind_des") = ""
                                         DR.Item("frz_des") = ""
                                         DR.Item("CAP") = ""
                                         DR.Item("com_des_indirizzo") = ""
                                         DR.Item("pro_cod_indirizzo") = ""
                                         DR.Item("stato_indirizzo") = ""
                                         DR.Item("stato_indirizzo_des") = ""
                                         DR.Item("note_indirizzo") = ""
                                         DR.Item("pro_cod_istat_indirizzo") = ""
                                         DR.Item("com_cod_istat_indirizzo") = ""

                                         DR.Item("Pratiche_Cod") = ""
                                         DR.Item("Pratiche_Des") = ""

                                         DR.Item("KPIN") = ""
                                         DR.Item("Block_Name") = ""

                                         DR.Item("Foral_Cod") = 0
                                         DR.Item("Foral_Des") = ""

                                         DR.Item("Data_Inizio_Portinnesto") = ""

                                         DR.Item("Data_Creazione") = DBNull.Value
                                         DR.Item("Data_Modifica") = DBNull.Value

                                         DR.Item("chiave") = chiave
                                         chiave += 1

                                         'dtParticelleC.Rows.Add(DR)
                                         rowList.Enqueue(DR)
                                     End If

                                     'End If

                                 Next

                             Else

                                 Dim sezioneqeury = Sezione
                                 If sezioneqeury = "" Then
                                     sezioneqeury = "0"
                                 End If

                                 Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_ServerC)
                                 If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                     TipoZona = "v"
                                     TipoZona_Des = "Vulnerabile"
                                 End If

                                 If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_ServerC, DataInizio, DataFine) Then
                                     TipoZona = "v"
                                     TipoZona_Des = "Vulnerabile"
                                 Else
                                     Dim a = 12
                                 End If

                                 DR = dtParticelleC.NewRow

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
                                 DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
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
                                 DR.Item("Veg_Cod") = "0|3262"
                                 DR.Item("Cul_Cod") = 0
                                 DR.Item("Grfi_Cod") = 0
                                 DR.Item("Grva_Cod") = 0
                                 DR.Item("Id_Cod") = 0

                                 DR.Item("veg_des") = "Utilizzo non specificato"
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

                                 DR.Item("campo_cod") = 0
                                 DR.Item("campo_des") = ""

                                 DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                                 DR.Item("Isola") = ""

                                 DR.Item("CapitolatoPrivato") = ""
                                 DR.Item("CapitolatoPrivato_Des") = ""

                                 DR.Item("Finalita_Concimazione_Impianto") = 0

                                 DR.Item("Cod_Indirizzo") = 0
                                 DR.Item("ind_des") = ""
                                 DR.Item("frz_des") = ""
                                 DR.Item("CAP") = ""
                                 DR.Item("com_des_indirizzo") = ""
                                 DR.Item("pro_cod_indirizzo") = ""
                                 DR.Item("stato_indirizzo") = ""
                                 DR.Item("stato_indirizzo_des") = ""
                                 DR.Item("note_indirizzo") = ""
                                 DR.Item("pro_cod_istat_indirizzo") = ""
                                 DR.Item("com_cod_istat_indirizzo") = ""

                                 DR.Item("Pratiche_Cod") = ""
                                 DR.Item("Pratiche_Des") = ""

                                 DR.Item("KPIN") = ""
                                 DR.Item("Block_Name") = ""

                                 DR.Item("Foral_Cod") = 0
                                 DR.Item("Foral_Des") = ""

                                 DR.Item("Data_Inizio_Portinnesto") = ""

                                 DR.Item("Data_Creazione") = DBNull.Value
                                 DR.Item("Data_Modifica") = DBNull.Value

                                 DR.Item("chiave") = chiave
                                 chiave += 1

                                 'dtParticelleC.Rows.Add(DR)
                                 rowList.Enqueue(DR)
                             End If
                         End Sub
            )

        Dim queuedr As DataRow = Nothing
        While rowList.TryDequeue(queuedr)
            dtParticelleC.Rows.Add(queuedr)
        End While

        DtParticelle = dtParticelleC

    End Sub


    Private Sub Popola_DtAppezzamenti_daFascicolo_3_ISWSRespAnagFascicolo2(ByVal Piva As String,
                                                           ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo2,
                                                           ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                                           ByVal iAggregaSpecie As Integer,
                                                           ByVal bAggregaTare As Boolean,
                                                           ByRef DataInizio As Date,
                                                           ByRef DataFine As Date,
                                                           ByRef DtParticelle As DataTable,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           ByRef Allegati_Documenti_Numero As String,
                                                           ByRef ImportCatasto As Boolean,
                                                           ByRef ASG_Utente_Username As String,
                                                           ByRef ASG_Utente_Password As String,
                                                           ByRef ASG_ProgressivoGIAS As String,
                                                           ByVal CatastoDaImportare As Boolean)

        'Dim NomeRoutine As String = "Popola_DtAppezzamenti_daFascicolo"
        Dim MsgOK As String = ""
        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

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
        Dim Validita_Inizio_Impianto As Date
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
        Dim Data_Semina As Date = AGRODATAINIZIO
        Dim Data_Raccolta As Date = AGRODATAFINE
        Dim Data_Fioritura As Date = AGRODATAINIZIO
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

        Dim N_App As Integer = 1
        Dim appezza As Integer = -1
        Dim chiave As Integer = 1

        'Dim strValiditaInizio As String = String.Empty
        'strValiditaInizio = DataInizio.ToShortDateString
        'Dim strValiditaFine As String = String.Empty
        'strValiditaFine = DataFine.ToShortDateString


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
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataScheda, DataInizio, DataFine, objParametri_Utenti)
        End If

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

        'leggo i comuni x avere le descrizioni poi
        Dim DT_Comuni As New DataTable
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        DT_Comuni = objIstat.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        Dim objZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objZoneMetaschema As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R

        For Each territorio In ISWSTerritorioFS6

            'DR = DtParticelle.NewRow

            Prov = IIf(IsNothing(territorio.Provincia), "", territorio.Provincia)
            Com = IIf(IsNothing(territorio.Comune), "", territorio.Comune)
            Sezione = IIf(IsNothing(territorio.Sezione), "", territorio.Sezione)
            Foglio = IIf(IsNothing(territorio.Foglio), 0, territorio.Foglio)
            strNumero = IIf(IsNothing(territorio.Particella), 0, territorio.Particella)
            Subalterno = IIf(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

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
            ElseIf Not ImportCatasto Then
                If CatastoDaImportare Then
                    ImportCatasto = True
                    DtParticelle.Clear()
                    ImportaCatastoFascicolo_ISWSRespAnagFascicolo2(fascicolo,
                                            ISWSTerritorioFS6,
                                            Piva,
                                            fascicolo.CUAA,
                                            True,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            ASG_Utente_Username,
                                            ASG_Utente_Password,
                                            ASG_ProgressivoGIAS)
                    Exit Sub
                End If
                'Throw New Exception("Catasto non presente, non è possibile compilare il fascicolo")
                Sa_Cod = 0
                Sa_Nome = "01"
            End If

            Select Case territorio.codiceTipoConduzione
                Case AGEA_Coordinamento.TipoConduzione.Item1
                    TitoloPossesso = 1 'Proprietà
                    TitoloPossessoDes = "Proprieta"
                Case AGEA_Coordinamento.TipoConduzione.Item2
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

            If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                If Inizio_Possesso < AGRODATAINIZIO Then
                    Inizio_Possesso = AGRODATAINIZIO
                End If
            End If
            If territorio.DataFineConduzione IsNot Nothing AndAlso
                                        territorio.DataFineConduzione <> "" AndAlso
                                        territorio.DataFineConduzione <> "99991231" AndAlso
                                        territorio.DataFineConduzione <> "99990101" Then
                Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
            End If

            'DataInizio = Inizio_Possesso
            'DataFine = Fine_Possesso

            supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)

            supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0

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



            If territorio.Destinazione IsNot Nothing Then

                For j = 0 To territorio.Destinazione.Length - 1

                    Qualita = territorio.Destinazione(j).codiceQualita

                    strMacrousi = ""
                    strUtilizzi = ""

                    Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                    Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                    MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                    'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
                    '                           objParametri_Server.LogFileName,
                    '                           objParametri_Server.LogDescrizioneUtente,
                    '                           NomeRoutine,
                    '                           MsgOK)

                    'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                    'HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                    Dim sMacrousoSup As String = ""
                    If Not (iAggregaSpecie <> 0 OrElse bAggregaTare) Then
                        sMacrousoSup = " (" & Macrouso_Sup & " Ha) "
                    End If

                    Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                    strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
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
                    If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                        For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                            strUtilizzi = ""

                            Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                            Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                            Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                            Specie_Des_Agea = ""
                            Varieta_Des_Agea = ""

                            Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R



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
                            Cop_Cod = 0
                            Cop_Des = ""
                            Lotto = ""
                            Resa = 0
                            Num_Piante = 0
                            TRA_Fila = 0
                            SU_Fila = 0
                            Validita_Inizio_Impianto = AGRODATAINIZIO
                            TipoZona = "n"
                            TipoZona_Des = "Non Vulnerabile"

                            Unita_Vitata = "0"
                            frazionato = 0
                            unito = 0

                            StatoImpianto_Cod = ""
                            N = Nothing
                            P = Nothing
                            K = Nothing
                            Data_Semina = AGRODATAINIZIO
                            Data_Raccolta = AGRODATAFINE
                            Data_Fioritura = AGRODATAINIZIO
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

                            Uso_Cod_Agea = ""
                            Occupazione_Cod_Agea = ""
                            Destinazione_Cod_Agea = ""
                            Qualita_Cod_Agea = ""
                            Gru_Cod = 0
                            Dim LogCodificheMancantiSpecie As String = ""
                            Dim LogCodificheMancantiVarieta As String = ""

                            objUtilizzi.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                      LogCodificheMancantiVarieta,
                                                                      Specie_Cod, Varieta_Cod,
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

                            'Specie_Des_Agea &= " (Cod." & Specie_Cod & ")"

                            If (Veg_Cod <> 0 AndAlso Cul_Cod = 0) Then
                                Dim ObjVarietaAltre As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                                Cul_Cod = ObjVarietaAltre.VarietaAltre(Veg_Cod, objParametri_Server)
                            End If

                            If Veg_Cod <> 0 Then
                                Dim copCore As New AgronicaCoreMetaSchemaDAL.Copertura_R
                                Cop_Cod = copCore.CodCopNessuna_from_VegCod(Veg_Cod, objParametri_Server)
                                StatoImpianto_Cod = "102"
                            End If

                            If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                                Dim a = 0
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

                            Dim sezioneqeury = Sezione
                            If sezioneqeury = "" Then
                                sezioneqeury = "0"
                            End If
                            Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            Else
                                Dim a = 12
                            End If

                            If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_Server, DataInizio, DataFine) Then
                                TipoZona = "v"
                                TipoZona_Des = "Vulnerabile"
                            Else
                                Dim a = 12
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
                            DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                            supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                            Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                            DR.Item("sup") = Format(supCatasto, "0.0000")
                            supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                            If DR.Item("Veg_Cod") = "0|0" Then
                                Dim a = 0
                            End If
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

                            DR.Item("veg_des") = Veg_Des
                            DR.Item("cul_des") = Cul_Des
                            DR.Item("grfi_des") = Grfi_Des
                            DR.Item("grva_des") = Grva_Des

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



                            DR.Item("campo_cod") = 0
                            DR.Item("campo_des") = ""
                            'If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod) Then

                            '    HashMacrousi.Add(Macrouso_Cod & Qualita & Specie_Cod & Varieta_Cod, "")
                            DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                            DR.Item("Isola") = ""

                            DR.Item("CapitolatoPrivato") = ""
                            DR.Item("CapitolatoPrivato_Des") = ""

                            DR.Item("Finalita_Concimazione_Impianto") = 0

                            DR.Item("Cod_Indirizzo") = 0
                            DR.Item("ind_des") = ""
                            DR.Item("frz_des") = ""
                            DR.Item("CAP") = ""
                            DR.Item("com_des_indirizzo") = ""
                            DR.Item("pro_cod_indirizzo") = ""
                            DR.Item("stato_indirizzo") = ""
                            DR.Item("stato_indirizzo_des") = ""
                            DR.Item("note_indirizzo") = ""
                            DR.Item("pro_cod_istat_indirizzo") = ""
                            DR.Item("com_cod_istat_indirizzo") = ""

                            DR.Item("Pratiche_Cod") = ""
                            DR.Item("Pratiche_Des") = ""

                            DR.Item("KPIN") = ""
                            DR.Item("Block_Name") = ""

                            DR.Item("Foral_Cod") = 0
                            DR.Item("Foral_Des") = ""

                            DR.Item("Data_Inizio_Portinnesto") = ""

                            DR.Item("Data_Creazione") = DBNull.Value
                            DR.Item("Data_Modifica") = DBNull.Value

                            DR.Item("chiave") = chiave
                            chiave += 1

                            DtParticelle.Rows.Add(DR)

                            'Else

                            '    Dim query = " PROV='" & Prov & "' AND COM = '" & Com & "' AND SEZIONE='" & Sezione & "' AND FOGLIO = " & Foglio &
                            '                        " AND NUMERO = " & Numero & "  AND SUBALTERNO = '" & Subalterno & "' AND Macrouso_Cod = '" & Macrouso_Cod & "' " &
                            '                        " AND Veg_Cod = '" & Veg_Cod & "|" & Id_Cod & "'  AND cul_Cod = " & Cul_Cod & " "

                            '    Dim row = DtParticelle.Select(query)
                            '    If row.Count > 0 Then
                            '        row(0)("utilizzo_sup") = Decimal.Round(row(0)("utilizzo_sup") + Utilizzo_Sup, 4)
                            '    End If

                            'End If
                        Next

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
                        Validita_Inizio_Impianto = AGRODATAINIZIO
                        TipoZona = "n"
                        TipoZona_Des = "Non Vulnerabile"
                        'MetodoProduzione_Cod = 1
                        'MetodoProduzione_Des = "Convenzionale"
                        Unita_Vitata = "0"
                        frazionato = 0
                        unito = 0

                        'Dpi_Cod = ""
                        'Reg_Cod = "1"
                        StatoImpianto_Cod = ""
                        N = Nothing
                        P = Nothing
                        K = Nothing
                        Data_Semina = AGRODATAINIZIO
                        Data_Raccolta = AGRODATAFINE
                        Data_Fioritura = AGRODATAINIZIO
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
                            StatoImpianto_Cod = "102"
                        End If

                        If Veg_Cod = 0 AndAlso Id_Cod = 0 Then
                            Id_Cod = 3078
                            Veg_Des = "Nessuna mappatura con Gias"
                        End If

                        Dim sezioneqeury = Sezione
                        If sezioneqeury = "" Then
                            sezioneqeury = "0"
                        End If
                        Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                        If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                            TipoZona = "v"
                            TipoZona_Des = "Vulnerabile"
                        End If

                        If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_Server, DataInizio, DataFine) Then
                            TipoZona = "v"
                            TipoZona_Des = "Vulnerabile"
                        Else
                            Dim a = 12
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
                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
                        supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                        DR.Item("sup") = Format(supCatasto, "0.0000")
                        supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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
                        DR.Item("Veg_Cod") = "0|3262" 'CStr(Veg_Cod) & "|" & CStr(Id_Cod)
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

                        DR.Item("campo_cod") = 0
                        DR.Item("campo_des") = ""

                        DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                        DR.Item("Isola") = ""

                        DR.Item("CapitolatoPrivato") = ""
                        DR.Item("CapitolatoPrivato_Des") = ""

                        DR.Item("Finalita_Concimazione_Impianto") = 0

                        DR.Item("Cod_Indirizzo") = 0
                        DR.Item("ind_des") = ""
                        DR.Item("frz_des") = ""
                        DR.Item("CAP") = ""
                        DR.Item("com_des_indirizzo") = ""
                        DR.Item("pro_cod_indirizzo") = ""
                        DR.Item("stato_indirizzo") = ""
                        DR.Item("stato_indirizzo_des") = ""
                        DR.Item("note_indirizzo") = ""
                        DR.Item("pro_cod_istat_indirizzo") = ""
                        DR.Item("com_cod_istat_indirizzo") = ""

                        DR.Item("Pratiche_Cod") = ""
                        DR.Item("Pratiche_Des") = ""

                        DR.Item("KPIN") = ""
                        DR.Item("Block_Name") = ""

                        DR.Item("Foral_Cod") = 0
                        DR.Item("Foral_Des") = ""

                        DR.Item("Data_Inizio_Portinnesto") = ""

                        DR.Item("Data_Creazione") = DBNull.Value
                        DR.Item("Data_Modifica") = DBNull.Value

                        DR.Item("chiave") = chiave
                        chiave += 1

                        DtParticelle.Rows.Add(DR)

                    End If

                    'End If

                Next

            Else

                Dim sezioneqeury = Sezione
                If sezioneqeury = "" Then
                    sezioneqeury = "0"
                End If

                Dim dtZone = objZone.Leggi(-17, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtZone IsNot Nothing AndAlso dtZone.Rows.Count > 0 Then
                    TipoZona = "v"
                    TipoZona_Des = "Vulnerabile"
                End If

                If objZoneMetaschema.Vulnerabile(Prov, Com, sezioneqeury, Foglio, Numero, Subalterno, 0, 0, "", objParametri_Server, DataInizio, DataFine) Then
                    TipoZona = "v"
                    TipoZona_Des = "Vulnerabile"
                Else
                    Dim a = 12
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
                DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")
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
                DR.Item("Veg_Cod") = "0|3262"
                DR.Item("Cul_Cod") = 0
                DR.Item("Grfi_Cod") = 0
                DR.Item("Grva_Cod") = 0
                DR.Item("Id_Cod") = 0

                DR.Item("veg_des") = "Utilizzo non specificato"
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

                DR.Item("campo_cod") = 0
                DR.Item("campo_des") = ""

                DR.Item("Riferimento_Alfanumerico_Appezzamento") = ""
                DR.Item("Isola") = ""

                DR.Item("CapitolatoPrivato") = ""
                DR.Item("CapitolatoPrivato_Des") = ""

                DR.Item("Finalita_Concimazione_Impianto") = 0

                DR.Item("Cod_Indirizzo") = 0
                DR.Item("ind_des") = ""
                DR.Item("frz_des") = ""
                DR.Item("CAP") = ""
                DR.Item("com_des_indirizzo") = ""
                DR.Item("pro_cod_indirizzo") = ""
                DR.Item("stato_indirizzo") = ""
                DR.Item("stato_indirizzo_des") = ""
                DR.Item("note_indirizzo") = ""
                DR.Item("pro_cod_istat_indirizzo") = ""
                DR.Item("com_cod_istat_indirizzo") = ""

                DR.Item("Pratiche_Cod") = ""
                DR.Item("Pratiche_Des") = ""

                DR.Item("KPIN") = ""
                DR.Item("Block_Name") = ""

                DR.Item("Foral_Cod") = 0
                DR.Item("Foral_Des") = ""

                DR.Item("Data_Inizio_Portinnesto") = ""

                DR.Item("Data_Creazione") = DBNull.Value
                DR.Item("Data_Modifica") = DBNull.Value

                DR.Item("chiave") = chiave
                chiave += 1

                DtParticelle.Rows.Add(DR)

            End If

        Next

    End Sub

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

    Public Sub dtParticelleAggrega(ByVal iAggregaSpecie As Integer,
                                    ByVal bDividiCentri As Boolean,
                                    ByVal bAggregaTare As Boolean,
                                    ByVal DtParticelle As DataTable,
                                    ByRef dtParticelleAggregate As DataTable)


        If DtParticelle.Rows.Count > 0 Then

            Dim qryRisultato As IEnumerable(Of DataRow)

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

    Public Sub trovaDisciplinare(Piva As String,
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


    Public Sub impostaDisciplinareDaPreferenza(ByVal preferenza As String,
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
                                                       row.Item("utilizzo_sup")) & "]"
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
            Catasto.utilizzo_sup = utilizzo_sup
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
        Public Property utilizzo_sup As Double
    End Class

    Public Function Importa_Dati_Catasto(ByVal StringaConnessione As String,
                                         ByVal Utente_Username As String,
                                         ByVal Utente_Password As String,
                                         ByVal ProgressivoGIAS As Integer,
                                         ByVal CodiceChiaveCliente As Integer,
                                         ByVal LinkWSImportaGIAS As String,
                                         ByVal LogDirectory As String,
                                         ByVal LogFileName As String,
                                         ByRef Num_Particelle_Importate As Integer,
                                         ByRef Num_Impianti_Importati As Integer,
                                         ByRef Num_Particelle_NON_Importate As Integer,
                                         ByRef Num_Impianti_NON_Importati As Integer,
                                         ByRef Messaggio As String,
                                         ByRef LogCodificheMancantiSpecie As String,
                                         ByRef LogCodificheMancantiVarieta As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim Dt_Imprese As New DataTable
        Dim NomeRoutine As String = "Importa_Dati"

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        'objLog.Scrivi_LOG(LogDirectory,
        '           LogFileName,
        '           objParametri_Server.LogDescrizioneUtente,
        '           NomeRoutine,
        '           "Inizio importazione")

        'Leggo i File
        Crea_Dt_Imprese(StringaConnessione, Messaggio, Dt_Imprese)

        'objLog.Scrivi_LOG(LogDirectory,
        '                 LogFileName,
        '                 objParametri_Server.LogDescrizioneUtente,
        '                 NomeRoutine,
        '                 Messaggio)

        Dim import_umbria_utils As New AGEA_UMBRIA_Utility()
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim aziendaVisibile As Boolean = False
        For Each cuaaRow In Dt_Imprese.Rows

            Dim cuaa = cuaaRow(0)

            Dim piva = objImprese.Piva_from_IdCodValCod(1010, cuaa, objParametri_Server)

            If piva = "" Then
                Try



                    'objLog.Scrivi_LOG(LogDirectory,
                    '   LogFileName,
                    '   objParametri_Server.LogDescrizioneUtente,
                    '   NomeRoutine,
                    '   "Importo CUAA:" & cuaa)

                    Dim fascicolo As String
                    Dim Num_Scheda As String = Nothing
                    Dim DataValidazione As Date
                    Dim OrigineOpr As String = ""
                    Dim strErr As String = ""

                    import_umbria_utils.Leggi_Fascicoli_UltimaValidazione_UMBRIA(cuaa, 4, Num_Scheda, DataValidazione, OrigineOpr, strErr, objParametri_Server)

                    Dim errCOD = 0
                    Dim errMsg = ""

                    aziendaVisibile = False
                    If Num_Scheda IsNot Nothing Then

                        fascicolo = import_umbria_utils.CaricaDati_WS_AgroFascicolo_UMBRIA(errCOD, errMsg, cuaa, Num_Scheda, objParametri_Server)

                        CreaAzienda(ProgressivoGIAS, Utente_Password, "", cuaa, fascicolo,
                                    "", "", "", "", "", "", "", "",
                                    objParametri_Server, objParametri_Utenti,
                                    aziendaVisibile, "", True, True, False, False, False)

                    End If

                Catch ex As Exception
                    Dim customLOGParams As New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
                        .LogDirectory = LogDirectory,
                        .LogFileName = LogFileName
                    }

                    objLog.Scrivi_LOG(objParametri_Server,
                                      NomeRoutine,
                                      "Errore su import CUAA " & cuaa & ": " & ex.Message,
                                      CustomLOGParams:=customLOGParams)

                End Try
            End If

        Next

    End Function

    Private Sub Crea_Dt_Imprese(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)
        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try


    End Sub

    Public Sub PopolaDT_Appezzamenti(ByVal Piva As String,
                                     ByRef fascicolo As String,
                                     ByVal iAggregaSpecie As Integer,
                                     ByVal bAggregaTare As Boolean,
                                     ByRef DataInizio As Date,
                                     ByRef DataFine As Date,
                                     ByRef DtParticelle As DataTable,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     ByRef Allegati_Documenti_Numero As String,
                                     ByRef ASG_Utente_Username As String,
                                     ByRef ASG_Utente_Password As String,
                                     ByRef ASG_ProgressivoGIAS As String,
                                     ByRef CatastoDaImportare As Boolean)

        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(fascicolo)
        obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

        Dim Fascicolo_Umbria = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo15)

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Dim Cuaa = Fascicolo_Umbria.CUAA

        Dim strFascicolo2 As String = ""
        Dim strConsistenze As String = ""
        Dim strConsistenze2 As String = ""
        Dim strMacchine As String = ""
        Dim strSoggetti As String = ""

        Try



            Dim Consistenze_Umbria = Leggi_Consistenze2(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strConsistenze)
            If Consistenze_Umbria Is Nothing Then
                Consistenze_Umbria = New List(Of AGEA_Coordinamento.ISWSTerritorio1)().ToArray
            End If

            Dim Consistenze_Umbria2 = Leggi_Consistenze(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strConsistenze2)
            If Consistenze_Umbria2 Is Nothing Then
                Consistenze_Umbria2 = New List(Of AGEA_Coordinamento.ISWSTerritorio15)().ToArray
            End If


            Try
                Dim Fascicolo2 = Leggi_Fascicolo20(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strFascicolo2)
            Catch ex As Exception

            End Try

            Dim Macchine = Leggi_Macchine(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strMacchine)

            Dim Soggetti = Leggi_Soggetti(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strSoggetti)

            Dim Detentore As String = ""
            Dim dataValidazione As Date = AGRODATAINIZIO
            Dim SchedaValidazione As String = ""
            If Fascicolo_Umbria.detentore IsNot Nothing Then
                Detentore = Fascicolo_Umbria.detentore
            End If

            If Fascicolo_Umbria.dataValidazFascicolo IsNot Nothing AndAlso Fascicolo_Umbria.dataValidazFascicolo <> "" Then
                dataValidazione = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Fascicolo_Umbria.dataValidazFascicolo))
            End If

            If Fascicolo_Umbria.schedaValidazione IsNot Nothing Then
                SchedaValidazione = Fascicolo_Umbria.schedaValidazione
            End If



            FascicoloMemorizza(4, Piva, dataValidazione, SchedaValidazione, fascicolo, objParametri_Server, 0, Detentore)

            FascicoloMemorizza(44, Piva, dataValidazione, SchedaValidazione, strFascicolo2, objParametri_Server, 0, Detentore)

            If Consistenze_Umbria.Length > 0 Then
                FascicoloMemorizza(45, Piva, dataValidazione, SchedaValidazione, strConsistenze, objParametri_Server, 0, Detentore)
            End If

            If Consistenze_Umbria2.Length > 0 Then
                FascicoloMemorizza(41, Piva, dataValidazione, SchedaValidazione, strConsistenze, objParametri_Server, 0, Detentore)
            End If

            If Macchine.Length > 0 Then
                FascicoloMemorizza(42, Piva, dataValidazione, SchedaValidazione, strMacchine, objParametri_Server, 0, Detentore)
            End If

            FascicoloMemorizza(43, Piva, dataValidazione, SchedaValidazione, strSoggetti, objParametri_Server, 0, Detentore)

            Dim importCatasto As Boolean = False

            If Consistenze_Umbria.Length > 0 Then
                Popola_DtAppezzamenti_daFascicolo_3(Piva,
                                                Fascicolo_Umbria,
                                                Consistenze_Umbria.ToList,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, CatastoDaImportare)

                If importCatasto Then

                    Popola_DtAppezzamenti_daFascicolo_3(Piva,
                                                    Fascicolo_Umbria,
                                                    Consistenze_Umbria.ToList,
                                                    False,
                                                    False,
                                                    DataInizio,
                                                    DataFine,
                                                    DtParticelle,
                                                    objParametri_Server,
                                                    objParametri_Utenti,
                                                    Allegati_Documenti_Numero,
                                                    importCatasto,
                                                    ASG_Utente_Username,
                                                    ASG_Utente_Password,
                                                    ASG_ProgressivoGIAS, CatastoDaImportare)

                End If

            ElseIf Consistenze_Umbria2.Length > 0 Then

                Popola_DtAppezzamenti_daFascicolo_2(Piva,
                                                Fascicolo_Umbria,
                                                Consistenze_Umbria2.ToList,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, CatastoDaImportare)

                If importCatasto Then

                    Popola_DtAppezzamenti_daFascicolo_2(Piva,
                                                    Fascicolo_Umbria,
                                                    Consistenze_Umbria2.ToList,
                                                    False,
                                                    False,
                                                    DataInizio,
                                                    DataFine,
                                                    DtParticelle,
                                                    objParametri_Server,
                                                    objParametri_Utenti,
                                                    Allegati_Documenti_Numero,
                                                    importCatasto,
                                                    ASG_Utente_Username,
                                                    ASG_Utente_Password,
                                                    ASG_ProgressivoGIAS, CatastoDaImportare)

                End If

            End If

        Catch ex As Exception

            objLog.Scrivi_LOG(objParametri_Server,
                              "Popola_DtAppezzamenti",
                              "Errore in Popola_DtAppezzamenti_daFascicolo_3:" & ex.Message)

        End Try

    End Sub


    Public Sub PopolaDT_Appezzamenti_ISWSRespAnagFascicolo2(ByVal Piva As String,
                                          ByRef fascicolo As String,
                                          ByVal iAggregaSpecie As Integer,
                                          ByVal bAggregaTare As Boolean,
                                          ByRef DataInizio As Date,
                                          ByRef DataFine As Date,
                                          ByRef DtParticelle As DataTable,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                          ByRef Allegati_Documenti_Numero As String,
                                          ByRef ASG_Utente_Username As String,
                                          ByRef ASG_Utente_Password As String,
                                          ByRef ASG_ProgressivoGIAS As String)

        Dim obj_fascicolo As New AGEA_Coordinamento.ISWSToOprResponse

        Dim x As New Xml.Serialization.XmlSerializer(GetType(AGEA_Coordinamento.ISWSToOprResponse))
        Dim string_reader As New StringReader(fascicolo)
        obj_fascicolo = DirectCast(x.Deserialize(string_reader), AGEA_Coordinamento.ISWSToOprResponse)

        Dim Fascicolo_Umbria As AGEA_Coordinamento.ISWSRespAnagFascicolo2 = DirectCast(obj_fascicolo.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo2)

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Dim Cuaa = Fascicolo_Umbria.CUAA

        Dim strFascicolo2 As String = ""
        Dim strConsistenze As String = ""
        Dim strMacchine As String = ""
        Dim strSoggetti As String = ""

        Try

            Dim Consistenze_Umbria = Leggi_Consistenze2(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strConsistenze)

            Try
                Dim Fascicolo2 = Leggi_Fascicolo20(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strFascicolo2)
            Catch ex As Exception

            End Try

            Dim Macchine = Leggi_Macchine(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strMacchine)

            Dim Soggetti = Leggi_Soggetti(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strSoggetti)

            Dim Detentore As String = ""
            Dim dataValidazione As Date = AGRODATAINIZIO
            Dim SchedaValidazione As String = ""
            If Fascicolo_Umbria.detentore IsNot Nothing Then
                Detentore = Fascicolo_Umbria.detentore
            End If

            If Fascicolo_Umbria.dataValidazFascicolo IsNot Nothing AndAlso Fascicolo_Umbria.dataValidazFascicolo <> "" Then
                dataValidazione = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(Fascicolo_Umbria.dataValidazFascicolo))
            End If

            If Fascicolo_Umbria.schedaValidazione IsNot Nothing Then
                SchedaValidazione = Fascicolo_Umbria.schedaValidazione
            End If



            FascicoloMemorizza(4, Piva, dataValidazione, SchedaValidazione, fascicolo, objParametri_Server, 0, Detentore)

            FascicoloMemorizza(44, Piva, dataValidazione, SchedaValidazione, strFascicolo2, objParametri_Server, 0, Detentore)

            If Consistenze_Umbria.Length > 0 Then
                FascicoloMemorizza(45, Piva, dataValidazione, SchedaValidazione, strConsistenze, objParametri_Server, 0, Detentore)
            End If

            If Macchine.Length > 0 Then
                FascicoloMemorizza(42, Piva, dataValidazione, SchedaValidazione, strMacchine, objParametri_Server, 0, Detentore)
            End If

            FascicoloMemorizza(43, Piva, dataValidazione, SchedaValidazione, strSoggetti, objParametri_Server, 0, Detentore)

            Dim importCatasto As Boolean = False
            Popola_DtAppezzamenti_daFascicolo_3_ISWSRespAnagFascicolo2(Piva,
                                                Fascicolo_Umbria,
                                                Consistenze_Umbria.ToList,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, True)

            If importCatasto Then

                Popola_DtAppezzamenti_daFascicolo_3_ISWSRespAnagFascicolo2(Piva,
                                                Fascicolo_Umbria,
                                                Consistenze_Umbria.ToList,
                                                False,
                                                False,
                                                DataInizio,
                                                DataFine,
                                                DtParticelle,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                Allegati_Documenti_Numero,
                                                importCatasto,
                                                ASG_Utente_Username,
                                                ASG_Utente_Password,
                                                ASG_ProgressivoGIAS, True)

            End If


        Catch ex As Exception

            objLog.Scrivi_LOG(objParametri_Server,
                              "Popola_DtAppezzamenti",
                              "Errore in Popola_DtAppezzamenti_daFascicolo_3:" & ex.Message)

            'Dim Consistenze_Umbria = Leggi_Consistenze(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server)

            'Dim Fascicolo2 = Leggi_Fascicolo20(Cuaa, Fascicolo_Umbria.schedaValidazione, objParametri_Server, strFascicolo2)

            'Popola_DtAppezzamenti_daFascicolo_2(Piva,
            '                                    Fascicolo_Umbria,
            '                                    Consistenze_Umbria.ToList,
            '                                    False,
            '                                    False,
            '                                    DataInizio,
            '                                    DataFine,
            '                                    DtParticelle,
            '                                    objParametri_Server,
            '                                    objParametri_Utenti,
            '                                    Allegati_Documenti_Numero)

        End Try

    End Sub

    Private Shared Sub ImportaCatastoFascicolo(ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                               ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                               piva As String,
                                               cuaa As String,
                                               aggregaCentri As Boolean,
                                               objParametri_Server As AgronicaCoreParametri,
                                               objParametri_Utenti As AgronicaCoreParametri,
                                               ASG_Utente_Username As String,
                                               ASG_Utente_Password As String,
                                               ASG_ProgressivoGIAS As String)


        Dim Rag_Soc As String = ""
        'Dim ID_Azienda As String = ""
        Dim EsisteImpresa As Boolean = False
        'Dim ApriPraticaRegFert As Boolean = False
        'Dim ApriPraticaRegFito As Boolean = False
        Dim objGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        Dim Piva_Padre As String = objGerarchiaImprese.LeggiPadre(piva, objParametri_Server, "")

        Dim Indirizzo As String = ""
        Dim Cap As String = ""
        Dim Comune As String = ""
        Dim Istat_Provincia As String = ""
        Dim Istat_Comune As String = ""
        'Dim Cod_Belfiore As String = ""

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
        Dim TitoloPossessoDes As String = ""
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Inizio_Possesso_Old As Date
        Dim Fine_Possesso_Old As Date

        Dim Presente_Fascicolo As Boolean = False
        Dim Sigla_Provincia As String = ""
        Dim XmlAnagrafica As New XmlDocument

        Dim XmlUtente As XmlElement
        Dim XmlImpresa As XmlElement
        Dim XmlFascicolo As XmlElement = Nothing
        Dim XmlCentro As XmlElement
        Dim XmlFabbricato As XmlElement
        Dim XmlDoc As String
        Dim XmlDocP As String

        Dim XmlPianificazione As New System.Xml.XmlDocument

        Dim XmlUtenteP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XmlFascicoloP As System.Xml.XmlElement = Nothing

        Dim stringaAnagrafica As String = ""
        Dim stringaPianificazione As String

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        'Dim Directory_Log As String
        'Dim File_Log As String
        'Dim MsgOK As String = ""
        'Dim i_azienda As Integer

        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

        'Dim Regolamento_Condizionalita As String
        Dim CodiceChiaveCliente As String
        Dim LinkWSImportaGIAS As String
        Dim Anno_Piano_Colturale As Integer
        Dim TopCode As Integer
        Dim BaseCode As Integer
        Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        Dim DataInizioMandato As Date = AGRODATAINIZIO

        CodiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)
        LinkWSImportaGIAS = objConfSiti.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)

        If fascicolo Is Nothing Then
            Exit Sub
        End If
        If fascicolo.dataSottMandato IsNot Nothing Then
            DataInizioMandato = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSottMandato))
        End If
        If fascicolo.dataAperturaFascicolo IsNot Nothing Then
            DataAperturaFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataAperturaFascicolo))
        End If
        If fascicolo.dataChiusuraFascicolo IsNot Nothing Then
            DataChiusuraFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataChiusuraFascicolo))
        End If
        Dim dataScheda As Date
        If fascicolo.dataSchedaValidazione IsNot Nothing Then
            dataScheda = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSchedaValidazione))
        End If
        If CDate(dataScheda) <> AGRODATAINIZIO Then
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione " & Key_SchedaDataValidazione & ")"
        Else
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione non presente)"
        End If

        '*******************************************************************************************************************
        '******   IMPRESA      *********************************************************************************************
        '*******************************************************************************************************************
        '(20/02/2015 fede aggiunti casi G,P x AVEPA)
        Dim tipoAzienda As String = CStr(fascicolo.tipoAzienda)

        Select Case tipoAzienda
            Case "PF", "P", "1", "0"
                Rag_Soc = fascicolo.denominazione & " " & fascicolo.nomePF
            Case "PG", "G"
                Rag_Soc = fascicolo.denominazione
            Case Else
                Rag_Soc = fascicolo.denominazione
        End Select


        'ID_Azienda = ws_AnaResponse.output.anagrafica.identita.master
        'Cod_Belfiore = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.comune.codErariale

        Sigla_Provincia = ""
        Comune = ""

        'verifico INDIRIZZO
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
            'MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & scheda.numeroScheda & " -  " & "Impossibile inserire l'impresa poiché manca l'indirizzo!"
            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
            '     objParametri_Server.LogFileName,
            '     objParametri_Server.LogDescrizioneUtente,
            '     NomeRoutine,
            '    MsgOK)
            'Lbl_Errore_Ricerca.Text = MsgOK
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


        ' Label
        'Me.Lbl_PreImpresa.Text = "L'impresa"
        'Me.Lbl_Impresa.Text = RagSoc & " (P.IVA : " & Piva & ")"

        'If Presente_Fascicolo = False Or VisualizzaComboPadri Then
        '    Cmb_Padre.SelectedIndex = Cmb_Padre.Items.IndexOf(Cmb_Padre.Items.FindByValue("UP103" & Istat_Provincia & "000"))
        'End If

        'Tabella_Impresa.Visible = True

        'Lbl_Dettagli_Impresa.Text = RagSoc & "<br/>" & _
        '                               "<b>Piva:</b> " & Piva & "<br/><br/>" & _
        '                               "<b>Sede legale:</b><br/>" & _
        '                               Indirizzo & "<br/>" & _
        '                               Cap & " " & _
        '                               Comune & ", (" & Sigla_Provincia & ")<br/>"


        '-----------------------------------------------
        '------- UTENTE        -------------------------
        '-----------------------------------------------
        XmlUtente = objXML.Xml_Pubblico_Utente(XmlAnagrafica,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)

        XmlUtenteP = objXML.Xml_Pubblico_Utente(XmlPianificazione,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)


        Legale_Rappresentante = "#"
        Legale_Rappresentante_Cognome = "#"
        Legale_Rappresentante_Nome = "#"
        Legale_Rappresentante_CF = "#"
        Legale_Rappresentante_Sesso = "#"
        Legale_Rappresentante_Indirizzo = "#"
        Legale_Rappresentante_Frazione = "#"
        Legale_Rappresentante_Cap = "#"
        Legale_Rappresentante_Comune = "#"
        Legale_Rappresentante_Provincia = "#"
        Legale_Rappresentante_Stato = "#"
        Legale_Rappresentante_Istat_Comune = "#"
        Legale_Rappresentante_Istat_Provincia = "#"
        Legale_Rappresentante_Data_Nascita = "#"
        Legale_Rappresentante_Comune_Nascita = "#"
        Legale_Rappresentante_Provincia_Nascita = "#"
        Legale_Rappresentante_Istat_Comune_Nascita = "#"
        Legale_Rappresentante_Istat_Provincia_Nascita = "#"
        Legale_Rappresentante_Rubrica1 = "#"
        Legale_Rappresentante_Rubrica2 = "#"
        Legale_Rappresentante_Rubrica3 = "#"
        Legale_Rappresentante_Rubrica4 = "#"
        Legale_Rappresentante_Rubrica5 = "#"

        '-----------------------------------------------
        '------- IMPRESA       -------------------------
        '-----------------------------------------------

        'If Presente_Fascicolo = False Or VisualizzaComboPadri Then
        '    Cmb_Padre.Items.Add("00225020239")
        '    Piva_Padre = Cmb_Padre.SelectedItem.Value

        '    ViewState("Piva_Padre_OLD") = Piva_Padre

        'End If
        Dim tipoOperazione As enum_TipoOperazioneDB
        If piva <> "" Then
            Dim dt_imp = objImpresa.Leggi(piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dt_imp.Rows.Count > 0 Then
                EsisteImpresa = True
            End If
            If EsisteImpresa Then
                tipoOperazione = enum_TipoOperazioneDB.Modifica
            Else
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If
        End If

        'tipoOperazione = enum_TipoOperazioneDB.Lettura
        XmlImpresa = objXML.Xml_Pubblico_Impresa(tipoOperazione,
                                                 piva,
                                                 Rag_Soc,
                                                 cuaa,
                                                 cuaa,
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
                                                 XmlAnagrafica)

        If Presente_Fascicolo Then
            XmlImpresa.AppendChild(XmlFascicolo)
        End If
        Dim tipoOperazione_Centro As New enum_TipoOperazioneDB
        If aggregaCentri Then
            Dim SaCod As Integer = 0
            Dim SaNome As String = "Centro n.01"

            tipoOperazione_Centro = tipoOperazione

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'verifico se c'è almeno un centro in archivio, in caso contrario
                'preparo l'xml di creazione
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            Else
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            End If
            'tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                            XmlAnagrafica)

            'Creo il fabbricato
            '1. in caso di primo inserimento
            '2. in caso di modifica se non esiste già un magazzino
            Dim CreaMagazzino As Boolean = False
            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                CreaMagazzino = True
            Else
                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                Dim DtFabb As DataTable
                DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     " Tipo_Fabbricato_Cod=20 ",
                                     "", objParametri_Server)
                If DtFabb.Rows.Count = 0 Then
                    CreaMagazzino = True
                End If
            End If

            If CreaMagazzino Then

                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura,
                                                              "0",
                                                              "Magazzino n.01",
                                                              20,
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
                                                              XmlAnagrafica)

                XmlCentro.AppendChild(XmlFabbricato)

            End If

            XmlImpresa.AppendChild(XmlCentro)
        Else
            Dim listCom As New List(Of String)
            For Each Comm In ISWSTerritorioFS6
                If Not listCom.Contains(Comm.Comune & "|" & Comm.Provincia) Then
                    listCom.Add(Comm.Comune & "|" & Comm.Provincia)
                End If
            Next
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim listSaCod As New List(Of String)
            For Each Com In listCom
                Dim dtCom = objIstat.Leggi(Com.Split("|")(1), Com.Split("|")(0), "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtCom IsNot Nothing AndAlso dtCom.Rows.Count > 0 Then
                    Dim com_des = dtCom.Rows(0).Item("LOCALITA")

                    Dim SaCod As Integer = 0
                    Dim SaNome As String = com_des
                    tipoOperazione_Centro = tipoOperazione

                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        'verifico se c'è almeno un centro in archivio, in caso contrario
                        'preparo l'xml di creazione
                        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim Dt_Centri As DataTable
                        Dt_Centri = objCentri.Leggi(piva,
                                                    0,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    " Sa_Nome=" & UtilityProvider.Agro_SQL_SaveText_NULL(SaNome) & "", "",
                                                    objParametri_Server)

                        If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                            'almeno un centro è presente
                            SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                            SaNome = Dt_Centri.Rows(0).Item("sa_nome")

                        Else
                            Dim objSequenze As New Agro_Sequenze
                            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, ASG_ProgressivoGIAS)
                            SaCod = objSequenze.NuovoId_CentriAziendali(
                                        piva,
                                        BaseCode,
                                        TopCode,
                                        objParametri_Server)
                            tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                        End If
                    End If
                    listSaCod.Add(SaCod)
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
                    XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                                    XmlAnagrafica)

                    'Creo il fabbricato
                    '1. in caso di primo inserimento
                    '2. in caso di modifica se non esiste già un magazzino
                    Dim CreaMagazzino As Boolean = False
                    If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                        CreaMagazzino = True
                    Else
                        Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                        Dim DtFabb As DataTable
                        DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             " Tipo_Fabbricato_Cod=20 ",
                                             "", objParametri_Server)
                        If DtFabb.Rows.Count = 0 Then
                            CreaMagazzino = True
                        End If
                    End If

                    If CreaMagazzino Then

                        XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Lettura,
                                                                      "0",
                                                                      "Magazzino n.01",
                                                                      20,
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
                                                                      XmlAnagrafica)

                        XmlCentro.AppendChild(XmlFabbricato)
                    End If
                    XmlImpresa.AppendChild(XmlCentro)
                End If
            Next
        End If

        XmlUtente.AppendChild(XmlImpresa)

        XmlDoc = XmlUtente.OuterXml


        '*******************************************************************************************************************
        '******   CONDIZIONALITA    ****************************************************************************************
        '*******************************************************************************************************************

        'verifico se esiste già su GIAS un PROFILO VALIDO...
        'se esiste NON LO CREO ORA!!!
        'Dim DLL_AD_Condizionalita As New AccessoDB_Condizionalita.AccessoDati(r"))
        'Dim Dt_Interviste As DataTable
        Dim CreaProfilo As Boolean = True

        'Dim objAuditReg As New AgronicaCoreAuditDAL.Audit_Regolamenti_R
        'Dim DtReg As DataTable
        Dim DataInizioReg, DataFineReg As Date

        DataInizioReg = New DateTime(CDate(dataScheda).Year, 1, 1)
        DataFineReg = New DateTime(CDate(dataScheda).Year, 12, 31)

        ' se ce n'è più di uno prendo cmq il primo, il più recente
        'DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, DataInizioReg, DataFineReg, "", "", objParametri_Server)

        'If DtReg.Rows.Count > 0 Then
        '    Qs_RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
        '    ViewState("Audit_Regolamento") = Qs_RegolamentoCod
        'Else
        '    ' leggo il regolamento audit più recente
        '    DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
        '    If DtReg.Rows.Count > 0 Then
        '        Qs_RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
        '        ViewState("Audit_Regolamento") = DtReg.Rows(0).Item("Regolamento_Cod")
        '    End If

        'End If

        'Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
        'Dt_Interviste = objAudit_Interviste.Leggi(1, _
        '                                          Qs_RegolamentoCod, _
        '                                          0, _
        '                                          Piva, _
        '                                          AGRODATAINIZIO, AGRODATAFINE, _
        '                                          "", "", _
        '                                          objParametri_Server)

        'If Not Dt_Interviste Is Nothing AndAlso Dt_Interviste.Rows.Count > 0 Then
        '    CreaProfilo = False
        'End If

        If CreaProfilo Then



        Else
            'profilo già presente
            'Riga_Controllo_Profilo.Visible = True
            'Riga_Condizionalita.Visible = False
        End If

        '*******************************************************************************************************************
        '******   PARTICELLE CATASTALI    **********************************************************************************
        '*******************************************************************************************************************

        Dim DtParticelle As New DataTable

        If ISWSTerritorioFS6 IsNot Nothing Then

            If ISWSTerritorioFS6.Count > 0 Then

                Dim DataInizio As Date = AGRODATAINIZIO
                Dim DataFine As Date = AGRODATAFINE
                'If IsDate(Txt_ValiditaInizio.Text) Then
                '    DataInizio = CDate(Txt_ValiditaInizio.Text)
                'End If
                'If IsDate(Txt_ValiditaFine.Text) Then
                '    DataFine = CDate(Txt_ValiditaFine.Text)
                'End If

                'Riga_Particelle.Visible = True

                Dim strValiditaInizio As String = String.Empty
                Dim strValiditaFine As String = String.Empty
                Dim DataInizioUtente As Date = AGRODATAINIZIO
                Dim DataFineUtente As Date = AGRODATAFINE

                Dim Anno As Integer = Today.Year
                If CDate(dataScheda) <> AGRODATAINIZIO Then
                    Anno = CDate(dataScheda).Year
                End If

                Dim Dt_Impost As DataTable
                Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim DrFiltro() As DataRow
                Dim Impostazione_Valore_Date As String = ""
                Dim Impostazione_Valore_N_Fascicolo As String = ""

                Dt_Impost = objImpost.Leggi2(1,
                                            objParametri_Server.UtenteUsername,
                                            0,
                                            "", "",
                                            objParametri_Utenti)

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_Date)

                objImpost.AnnataAgraria(CDate(dataScheda), DataInizioUtente, DataFineUtente, objParametri_Utenti)

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_Date = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_Date
                        Case "1" 'validita fascicoli
                            strValiditaInizio = dataScheda
                            Dim Key_SchedaDataFine = fascicolo.dataChiusuraFascicolo
                            If dataScheda <> "" Then
                                strValiditaFine = Key_SchedaDataFine
                            Else
                                strValiditaFine = DataFineUtente.ToShortDateString
                            End If

                        Case Else 'anno
                            strValiditaInizio = DataInizioUtente.ToShortDateString
                            strValiditaFine = DataFineUtente.ToShortDateString
                    End Select
                Else
                    strValiditaInizio = DataInizioUtente.ToShortDateString
                    strValiditaFine = DataFineUtente.ToShortDateString
                End If

                'Dim strValiditaInizio As String = String.Empty
                'Dim strValiditaFine As String = String.Empty
                'strValiditaInizio = "01/11/" & (Anno - 1).ToString
                'strValiditaFine = "31/10/" & Anno.ToString

                '*******************************************************************************************************************
                '******   PIANIFICAZIONE    **********************************************************************************

                Dim strPianificazione As String

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_NValidazioneNome)

                strPianificazione = "Piano Colturale " & Anno.ToString

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_N_Fascicolo = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_N_Fascicolo
                        Case "1" 'validita fascicoli
                            If CDate(dataScheda) <> AGRODATAINIZIO Then
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione " & dataScheda & ")"
                            Else
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione non presente)"
                            End If
                            'Case Else
                            '    strPianificazione = "Piano Colturale " & Anno.ToString
                    End Select
                End If

                XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(enum_TipoOperazioneDB.Scrittura,
                                   "0",
                                   piva,
                                   strPianificazione,
                                   strPianificazione,
                                   "Importazione " & strPianificazione,
                                   enum_Planning_Fonte.Avepa,
                                   enum_TipoPianificazione.Pianificazione_Annuale,
                                   strValiditaInizio,
                                   strValiditaFine,
                                   XmlPianificazione)

                XmlUtenteP.AppendChild(XmlTestata)

                If Presente_Fascicolo Then
                    XmlTestata.AppendChild(XmlFascicoloP)
                End If

                XmlDocP = XmlUtenteP.OuterXml

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
                'DtParticelle.Columns.Add(New DataColumn("qualita", GetType(String)))
                'DtParticelle.Columns.Add(New DataColumn("eleggibilita", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

                Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R

                For Each territorio In ISWSTerritorioFS6

                    'DR = DtParticelle.NewRow

                    Prov = If(IsNothing(territorio.Provincia), "", territorio.Provincia)
                    Com = If(IsNothing(territorio.Comune), "", territorio.Comune)
                    Sezione = If(IsNothing(territorio.Sezione), "", territorio.Sezione)
                    Foglio = If(IsNothing(territorio.Foglio), 0, territorio.Foglio)
                    strNumero = If(IsNothing(territorio.Particella), 0, territorio.Particella)
                    Subalterno = If(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

                    'verifico cosa trovo nel campo particella (su Agea è una stringa)
                    'se trovo dei numeri li metto in numero
                    'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
                    Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
                    Numero = objCOre.Numero_from_Stringa(strNumero)
                    NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                    If Subalterno = "" AndAlso NumeroStringa <> "" Then
                        Subalterno = Left(NumeroStringa, 3)
                    End If

                    'DR.Item("PROV") = Prov
                    'DR.Item("COM") = Com
                    'DR.Item("Sezione") = Sezione
                    'DR.Item("Foglio") = Foglio
                    'DR.Item("Numero") = Numero
                    'DR.Item("Subalterno") = Subalterno

                    'DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                    '                     DR.Item("COM") & ":_" & _
                    '                     DR.Item("Sezione") & ":_" & _
                    '                     DR.Item("Foglio").ToString & ":_" & _
                    '                     DR.Item("Numero").ToString & ":_" & _
                    '                     DR.Item("Subalterno")

                    TitoloPossesso = Converti_TitoliPossesso_Fascicolo(territorio.codiceTipoConduzione, TitoloPossessoDes)
                    'DR.Item("possesso") = TitoloPossessoDes

                    Inizio_Possesso = #1/1/1900#
                    Fine_Possesso = #12/31/2100#
                    Inizio_Possesso_Old = #1/1/1900#
                    Fine_Possesso_Old = #12/31/2100#

                    If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                        Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                        If Inizio_Possesso < AGRODATAINIZIO Then
                            Inizio_Possesso = AGRODATAINIZIO
                        End If
                    End If
                    If territorio.DataFineConduzione IsNot Nothing AndAlso
                        territorio.DataFineConduzione <> "" AndAlso
                        territorio.DataFineConduzione <> "99991231" AndAlso
                        territorio.DataFineConduzione <> "99990101" Then
                        Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
                    End If

                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                    '                          If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                    supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                    Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                    'DR.Item("sup") = Format(supCatasto, "0.0000")

                    supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
                    'DR.Item("supcondotta") = Format(supConduzione, "0.0000")

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
                    Dim Specie_Des As String
                    Dim Varieta_Des As String
                    Dim Utilizzo_Sup As Double
                    Dim HashMacrousi As New Hashtable
                    strMacrousi = ""
                    strUtilizzi = ""

                    If territorio.Destinazione IsNot Nothing Then

                        For j = 0 To territorio.Destinazione.Length - 1

                            'Eleggibilita = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).SuperficieEligibile
                            Qualita = territorio.Destinazione(j).codiceQualita

                            strMacrousi = ""
                            strUtilizzi = ""

                            Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                            Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                            'MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory, _
                            '           objParametri_Server.LogFileName, _
                            '           objParametri_Server.LogDescrizioneUtente, _
                            '           NomeRoutine, _
                            '           MsgOK)

                            If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                                HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                                strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             "", "",
                                                                                             objParametri_Server) & " (" & Macrouso_Sup & " Ha)<br/>"



                                '*******************************************************************************************************************
                                '******   UTILIZZO    **********************************************************************************
                                '*******************************************************************************************************************

                                If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                                    For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                                        strUtilizzi = ""

                                        Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                                        Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                                        Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                                        Specie_Des = ""
                                        Varieta_Des = ""



                                        Dim Veg_Cod As Integer = 0
                                        Dim Cul_Cod As Integer = 0
                                        Dim Grfi_Cod As Integer = 0
                                        Dim Id_Cod As Integer = 0
                                        Dim Grva_Cod As Integer = 0

                                        Dim Uso_Cod_Agea As String = ""
                                        Dim Occupazione_Cod_Agea As String = ""
                                        Dim Destinazione_Cod_Agea As String = ""
                                        Dim Qualita_Cod_Agea As String = ""

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
                                                            objParametri_Server)

                                        Specie_Des &= " (Cod." & Specie_Cod & ")"

                                        If Varieta_Des <> "" Then
                                            Varieta_Des &= " (Cod." & Varieta_Cod & ")"
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

                                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                             DR.Item("COM") & ":_" &
                                                             DR.Item("Sezione") & ":_" &
                                                             DR.Item("Foglio").ToString & ":_" &
                                                             DR.Item("Numero").ToString & ":_" &
                                                             DR.Item("Subalterno")

                                        DR.Item("possesso") = TitoloPossessoDes
                                        DR.Item("TitoloPossesso") = TitoloPossesso

                                        DR.Item("inizio_possesso") = Inizio_Possesso
                                        DR.Item("fine_possesso") = Fine_Possesso

                                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                        supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                        DR.Item("sup") = Format(supCatasto, "0.0000")

                                        supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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

                                        DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                                        DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                                        DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                                        DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                                        DR.Item("Scarto") = 0

                                        'DR.Item("qualita") = Qualita
                                        'DR.Item("eleggibilita") = Eleggibilita

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

                                    DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                         DR.Item("COM") & ":_" &
                                                         DR.Item("Sezione") & ":_" &
                                                         DR.Item("Foglio").ToString & ":_" &
                                                         DR.Item("Numero").ToString & ":_" &
                                                         DR.Item("Subalterno")

                                    DR.Item("possesso") = TitoloPossessoDes
                                    DR.Item("TitoloPossesso") = TitoloPossesso
                                    DR.Item("inizio_possesso") = Inizio_Possesso
                                    DR.Item("fine_possesso") = Fine_Possesso

                                    DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")


                                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                    '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                                    DR.Item("Uso_Cod_Agea") = ""
                                    DR.Item("Occupazione_Cod_Agea") = ""
                                    DR.Item("Destinazione_Cod_Agea") = ""
                                    DR.Item("Qualita_Cod_Agea") = ""

                                    DR.Item("Scarto") = 1

                                    'DR.Item("qualita") = Qualita
                                    'DR.Item("eleggibilita") = Eleggibilita

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

                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                             DR.Item("COM") & ":_" &
                                             DR.Item("Sezione") & ":_" &
                                             DR.Item("Foglio").ToString & ":_" &
                                             DR.Item("Numero").ToString & ":_" &
                                             DR.Item("Subalterno")

                        DR.Item("possesso") = TitoloPossessoDes
                        DR.Item("TitoloPossesso") = TitoloPossesso
                        DR.Item("inizio_possesso") = Inizio_Possesso
                        DR.Item("fine_possesso") = Fine_Possesso

                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                        DR.Item("Uso_Cod_Agea") = ""
                        DR.Item("Occupazione_Cod_Agea") = ""
                        DR.Item("Destinazione_Cod_Agea") = ""
                        DR.Item("Qualita_Cod_Agea") = ""

                        DR.Item("Scarto") = 1

                        'DR.Item("qualita") = ""
                        'DR.Item("eleggibilita") = ""

                        DtParticelle.Rows.Add(DR)

                    End If

                Next 'ISWSTerritorio1.Length

                Dim objImportazione As New SincroAnagrafeBA1.Importazione_Fascicolo
                stringaPianificazione = ""
                objImportazione.Crea_Stringa_Catasto(stringaAnagrafica,
                                                    stringaPianificazione,
                                                    XmlUtente.OuterXml,
                                                    XmlUtenteP.OuterXml,
                                                    piva, 0,
                                                    DataInizio, DataFine, Anno_Piano_Colturale,
                                                    DtParticelle,
                                                    objParametri_Server, aggregaCentri, ASG_ProgressivoGIAS, False)
                'stringaAnagrafica = XmlUtente.OuterXml
                'stringaPianificazione = ""

                If stringaAnagrafica <> "" Then

                    Dim strRisultato As String = ""
                    Dim strErr As String = ""
                    stringaPianificazione = ""
                    Importa_Dati(ASG_Utente_Password, stringaAnagrafica, stringaPianificazione, strErr, strRisultato, True, True, objParametri_Server, objParametri_Utenti)

                    'If strErr = "" Then
                    '    '----------------------------------------------------------
                    '    'importo il profilo della condizionalità
                    '    If Not DtCondizionalita Is Nothing AndAlso DtCondizionalita.Rows.Count > 0 Then
                    '        objImportazione.Importa_Condizionalita_Profilo(TopCode, BaseCode, Regolamento_Condizionalita, Piva, DtCondizionalita, objParametri_Server)
                    '    End If

                    '    ''----------------------------------------------------------
                    '    ''apro le pratiche
                    '    'If ApriPraticaRegFert Then
                    '    '    objImportazione.Importa_Pratica(4, "Registro fertilizzazioni", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If
                    '    'If ApriPraticaRegFito Then
                    '    '    objImportazione.Importa_Pratica(3, "Registro trattamenti (DM 290/01)", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If

                    'End If




                    If strRisultato <> "" Then
                        'objLog.Scrivi_LOG(Directory_Log,
                        '                 File_Log,
                        '                 objParametri_Server.LogDescrizioneUtente,
                        '                 "",
                        '                 i_azienda & " - " & "IMPORTATA l'impresa " & Rag_Soc & " (PIVA:" & piva & " - CUAA:" & cuaa & ")!")

                    End If

                End If
            End If
        End If

    End Sub

    Private Shared Sub ImportaCatastoFascicolo2(ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo15,
                                                ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio15),
                                                piva As String,
                                                cuaa As String,
                                                aggregaCentri As Boolean,
                                                objParametri_Server As AgronicaCoreParametri,
                                                objParametri_Utenti As AgronicaCoreParametri,
                                                ASG_Utente_Username As String,
                                                ASG_Utente_Password As String,
                                                ASG_ProgressivoGIAS As String)


        Dim Rag_Soc As String = ""
        'Dim ID_Azienda As String = ""
        Dim EsisteImpresa As Boolean = False
        'Dim ApriPraticaRegFert As Boolean = False
        'Dim ApriPraticaRegFito As Boolean = False
        Dim objGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        Dim Piva_Padre As String = objGerarchiaImprese.LeggiPadre(piva, objParametri_Server, "")

        Dim Indirizzo As String = ""
        Dim Cap As String = ""
        Dim Comune As String = ""
        Dim Istat_Provincia As String = ""
        Dim Istat_Comune As String = ""
        'Dim Cod_Belfiore As String = ""

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
        Dim TitoloPossessoDes As String = ""
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Inizio_Possesso_Old As Date
        Dim Fine_Possesso_Old As Date

        Dim Presente_Fascicolo As Boolean = False
        Dim Sigla_Provincia As String = ""
        Dim XmlAnagrafica As New XmlDocument

        Dim XmlUtente As XmlElement
        Dim XmlImpresa As XmlElement
        Dim XmlFascicolo As XmlElement = Nothing
        Dim XmlCentro As XmlElement
        Dim XmlFabbricato As XmlElement
        Dim XmlDoc As String
        Dim XmlDocP As String

        Dim XmlPianificazione As New System.Xml.XmlDocument

        Dim XmlUtenteP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XmlFascicoloP As System.Xml.XmlElement = Nothing

        Dim stringaAnagrafica As String = ""
        Dim stringaPianificazione As String

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        'Dim Directory_Log As String
        'Dim File_Log As String
        'Dim MsgOK As String = ""
        'Dim i_azienda As Integer

        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

        'Dim Regolamento_Condizionalita As String
        Dim CodiceChiaveCliente As String
        Dim LinkWSImportaGIAS As String
        Dim Anno_Piano_Colturale As Integer
        Dim TopCode As Integer
        Dim BaseCode As Integer
        Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        Dim DataInizioMandato As Date = AGRODATAINIZIO

        CodiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)
        LinkWSImportaGIAS = objConfSiti.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)

        If fascicolo Is Nothing Then
            Exit Sub
        End If
        If fascicolo.dataSottMandato IsNot Nothing Then
            DataInizioMandato = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSottMandato))
        End If
        If fascicolo.dataAperturaFascicolo IsNot Nothing Then
            DataAperturaFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataAperturaFascicolo))
        End If
        If fascicolo.dataChiusuraFascicolo IsNot Nothing Then
            DataChiusuraFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataChiusuraFascicolo))
        End If
        Dim dataScheda As Date
        If fascicolo.dataSchedaValidazione IsNot Nothing Then
            dataScheda = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSchedaValidazione))
        End If
        If CDate(dataScheda) <> AGRODATAINIZIO Then
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione " & Key_SchedaDataValidazione & ")"
        Else
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione non presente)"
        End If

        '*******************************************************************************************************************
        '******   IMPRESA      *********************************************************************************************
        '*******************************************************************************************************************
        '(20/02/2015 fede aggiunti casi G,P x AVEPA)
        Dim tipoAzienda As String = CStr(fascicolo.tipoAzienda)

        Select Case tipoAzienda
            Case "PF", "P", "1", "0"
                Rag_Soc = fascicolo.denominazione & " " & fascicolo.nomePF
            Case "PG", "G"
                Rag_Soc = fascicolo.denominazione
        End Select


        'ID_Azienda = ws_AnaResponse.output.anagrafica.identita.master
        'Cod_Belfiore = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.comune.codErariale

        Sigla_Provincia = ""
        Comune = ""

        'verifico INDIRIZZO
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
            'MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & scheda.numeroScheda & " -  " & "Impossibile inserire l'impresa poiché manca l'indirizzo!"
            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
            '     objParametri_Server.LogFileName,
            '     objParametri_Server.LogDescrizioneUtente,
            '     NomeRoutine,
            '    MsgOK)
            'Lbl_Errore_Ricerca.Text = MsgOK
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

        '-----------------------------------------------
        '------- UTENTE        -------------------------
        '-----------------------------------------------
        XmlUtente = objXML.Xml_Pubblico_Utente(XmlAnagrafica,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)

        XmlUtenteP = objXML.Xml_Pubblico_Utente(XmlPianificazione,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)


        Legale_Rappresentante = "#"
        Legale_Rappresentante_Cognome = "#"
        Legale_Rappresentante_Nome = "#"
        Legale_Rappresentante_CF = "#"
        Legale_Rappresentante_Sesso = "#"
        Legale_Rappresentante_Indirizzo = "#"
        Legale_Rappresentante_Frazione = "#"
        Legale_Rappresentante_Cap = "#"
        Legale_Rappresentante_Comune = "#"
        Legale_Rappresentante_Provincia = "#"
        Legale_Rappresentante_Stato = "#"
        Legale_Rappresentante_Istat_Comune = "#"
        Legale_Rappresentante_Istat_Provincia = "#"
        Legale_Rappresentante_Data_Nascita = "#"
        Legale_Rappresentante_Comune_Nascita = "#"
        Legale_Rappresentante_Provincia_Nascita = "#"
        Legale_Rappresentante_Istat_Comune_Nascita = "#"
        Legale_Rappresentante_Istat_Provincia_Nascita = "#"
        Legale_Rappresentante_Rubrica1 = "#"
        Legale_Rappresentante_Rubrica2 = "#"
        Legale_Rappresentante_Rubrica3 = "#"
        Legale_Rappresentante_Rubrica4 = "#"
        Legale_Rappresentante_Rubrica5 = "#"

        '-----------------------------------------------
        '------- IMPRESA       -------------------------
        '-----------------------------------------------

        Dim tipoOperazione As enum_TipoOperazioneDB
        If piva <> "" Then
            Dim dt_imp = objImpresa.Leggi(piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dt_imp.Rows.Count > 0 Then
                EsisteImpresa = True
            End If
            If EsisteImpresa Then
                tipoOperazione = enum_TipoOperazioneDB.Modifica
            Else
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If
        End If

        XmlImpresa = objXML.Xml_Pubblico_Impresa(tipoOperazione,
                                                 piva,
                                                 Rag_Soc,
                                                 cuaa,
                                                 cuaa,
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
                                                 XmlAnagrafica)

        If Presente_Fascicolo Then
            XmlImpresa.AppendChild(XmlFascicolo)
        End If
        Dim tipoOperazione_Centro As New enum_TipoOperazioneDB
        If aggregaCentri Then
            Dim SaCod As Integer = 0
            Dim SaNome As String = "Centro n.01"

            tipoOperazione_Centro = tipoOperazione

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'verifico se c'è almeno un centro in archivio, in caso contrario
                'preparo l'xml di creazione
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            Else
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            End If
            'tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                            XmlAnagrafica)

            'Creo il fabbricato
            '1. in caso di primo inserimento
            '2. in caso di modifica se non esiste già un magazzino
            Dim CreaMagazzino As Boolean = False
            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                CreaMagazzino = True
            Else
                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                Dim DtFabb As DataTable
                DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     " Tipo_Fabbricato_Cod=20 ",
                                     "", objParametri_Server)
                If DtFabb.Rows.Count = 0 Then
                    CreaMagazzino = True
                End If
            End If

            If CreaMagazzino Then

                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura,
                                                              "0",
                                                              "Magazzino n.01",
                                                              20,
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
                                                              XmlAnagrafica)

                XmlCentro.AppendChild(XmlFabbricato)

            End If

            XmlImpresa.AppendChild(XmlCentro)
        Else
            Dim listCom As New List(Of String)
            For Each Comm In ISWSTerritorioFS6
                If Not listCom.Contains(Comm.Comune & "|" & Comm.Provincia) Then
                    listCom.Add(Comm.Comune & "|" & Comm.Provincia)
                End If
            Next
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim listSaCod As New List(Of String)
            For Each Com In listCom
                Dim dtCom = objIstat.Leggi(Com.Split("|")(1), Com.Split("|")(0), "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtCom IsNot Nothing AndAlso dtCom.Rows.Count > 0 Then
                    Dim com_des = dtCom.Rows(0).Item("LOCALITA")

                    Dim SaCod As Integer = 0
                    Dim SaNome As String = com_des
                    tipoOperazione_Centro = tipoOperazione

                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        'verifico se c'è almeno un centro in archivio, in caso contrario
                        'preparo l'xml di creazione
                        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim Dt_Centri As DataTable
                        Dt_Centri = objCentri.Leggi(piva,
                                                    0,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    " Sa_Nome=" & UtilityProvider.Agro_SQL_SaveText_NULL(SaNome) & "", "",
                                                    objParametri_Server)

                        If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                            'almeno un centro è presente
                            SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                            SaNome = Dt_Centri.Rows(0).Item("sa_nome")

                        Else
                            Dim objSequenze As New Agro_Sequenze
                            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, ASG_ProgressivoGIAS)
                            SaCod = objSequenze.NuovoId_CentriAziendali(
                                        piva,
                                        BaseCode,
                                        TopCode,
                                        objParametri_Server)
                            tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                        End If
                    End If
                    listSaCod.Add(SaCod)
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
                    XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                                    XmlAnagrafica)

                    'Creo il fabbricato
                    '1. in caso di primo inserimento
                    '2. in caso di modifica se non esiste già un magazzino
                    Dim CreaMagazzino As Boolean = False
                    If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                        CreaMagazzino = True
                    Else
                        Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                        Dim DtFabb As DataTable
                        DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             " Tipo_Fabbricato_Cod=20 ",
                                             "", objParametri_Server)
                        If DtFabb.Rows.Count = 0 Then
                            CreaMagazzino = True
                        End If
                    End If

                    If CreaMagazzino Then

                        XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Lettura,
                                                                      "0",
                                                                      "Magazzino n.01",
                                                                      20,
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
                                                                      XmlAnagrafica)

                        XmlCentro.AppendChild(XmlFabbricato)
                    End If
                    XmlImpresa.AppendChild(XmlCentro)
                End If
            Next
        End If

        XmlUtente.AppendChild(XmlImpresa)

        XmlDoc = XmlUtente.OuterXml


        '*******************************************************************************************************************
        '******   CONDIZIONALITA    ****************************************************************************************
        '*******************************************************************************************************************

        'verifico se esiste già su GIAS un PROFILO VALIDO...
        'se esiste NON LO CREO ORA!!!
        Dim CreaProfilo As Boolean = True

        Dim DataInizioReg, DataFineReg As Date

        DataInizioReg = New DateTime(CDate(dataScheda).Year, 1, 1)
        DataFineReg = New DateTime(CDate(dataScheda).Year, 12, 31)

        If CreaProfilo Then



        Else
            'profilo già presente
            'Riga_Controllo_Profilo.Visible = True
            'Riga_Condizionalita.Visible = False
        End If

        '*******************************************************************************************************************
        '******   PARTICELLE CATASTALI    **********************************************************************************
        '*******************************************************************************************************************

        Dim DtParticelle As New DataTable

        If ISWSTerritorioFS6 IsNot Nothing Then

            If ISWSTerritorioFS6.Count > 0 Then

                Dim DataInizio As Date = AGRODATAINIZIO
                Dim DataFine As Date = AGRODATAFINE
                'If IsDate(Txt_ValiditaInizio.Text) Then
                '    DataInizio = CDate(Txt_ValiditaInizio.Text)
                'End If
                'If IsDate(Txt_ValiditaFine.Text) Then
                '    DataFine = CDate(Txt_ValiditaFine.Text)
                'End If

                'Riga_Particelle.Visible = True

                Dim strValiditaInizio As String = String.Empty
                Dim strValiditaFine As String = String.Empty
                Dim DataInizioUtente As Date = AGRODATAINIZIO
                Dim DataFineUtente As Date = AGRODATAFINE

                Dim Anno As Integer = Today.Year
                If CDate(dataScheda) <> AGRODATAINIZIO Then
                    Anno = CDate(dataScheda).Year
                End If

                Dim Dt_Impost As DataTable
                Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim DrFiltro() As DataRow
                Dim Impostazione_Valore_Date As String = ""
                Dim Impostazione_Valore_N_Fascicolo As String = ""

                Dt_Impost = objImpost.Leggi2(1,
                                            objParametri_Server.UtenteUsername,
                                            0,
                                            "", "",
                                            objParametri_Utenti)

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_Date)

                objImpost.AnnataAgraria(CDate(dataScheda), DataInizioUtente, DataFineUtente, objParametri_Utenti)

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_Date = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_Date
                        Case "1" 'validita fascicoli
                            strValiditaInizio = dataScheda
                            Dim Key_SchedaDataFine = fascicolo.dataChiusuraFascicolo
                            If dataScheda <> "" Then
                                strValiditaFine = Key_SchedaDataFine
                            Else
                                strValiditaFine = DataFineUtente.ToShortDateString
                            End If

                        Case Else 'anno
                            strValiditaInizio = DataInizioUtente.ToShortDateString
                            strValiditaFine = DataFineUtente.ToShortDateString
                    End Select
                Else
                    strValiditaInizio = DataInizioUtente.ToShortDateString
                    strValiditaFine = DataFineUtente.ToShortDateString
                End If

                'Dim strValiditaInizio As String = String.Empty
                'Dim strValiditaFine As String = String.Empty
                'strValiditaInizio = "01/11/" & (Anno - 1).ToString
                'strValiditaFine = "31/10/" & Anno.ToString

                '*******************************************************************************************************************
                '******   PIANIFICAZIONE    **********************************************************************************

                Dim strPianificazione As String

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_NValidazioneNome)

                strPianificazione = "Piano Colturale " & Anno.ToString

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_N_Fascicolo = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_N_Fascicolo
                        Case "1" 'validita fascicoli
                            If CDate(dataScheda) <> AGRODATAINIZIO Then
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione " & dataScheda & ")"
                            Else
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione non presente)"
                            End If
                            'Case Else
                            '    strPianificazione = "Piano Colturale " & Anno.ToString
                    End Select
                End If

                XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(enum_TipoOperazioneDB.Scrittura,
                                   "0",
                                   piva,
                                   strPianificazione,
                                   strPianificazione,
                                   "Importazione " & strPianificazione,
                                   enum_Planning_Fonte.Avepa,
                                   enum_TipoPianificazione.Pianificazione_Annuale,
                                   strValiditaInizio,
                                   strValiditaFine,
                                   XmlPianificazione)

                XmlUtenteP.AppendChild(XmlTestata)

                If Presente_Fascicolo Then
                    XmlTestata.AppendChild(XmlFascicoloP)
                End If

                XmlDocP = XmlUtenteP.OuterXml

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
                'DtParticelle.Columns.Add(New DataColumn("qualita", GetType(String)))
                'DtParticelle.Columns.Add(New DataColumn("eleggibilita", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

                For Each territorio In ISWSTerritorioFS6

                    'DR = DtParticelle.NewRow

                    Prov = IIf(IsNothing(territorio.Provincia), "", territorio.Provincia)
                    Com = IIf(IsNothing(territorio.Comune), "", territorio.Comune)
                    Sezione = IIf(IsNothing(territorio.Sezione), "", territorio.Sezione)
                    Foglio = IIf(IsNothing(territorio.Foglio), 0, territorio.Foglio)
                    strNumero = IIf(IsNothing(territorio.Particella), 0, territorio.Particella)
                    Subalterno = IIf(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

                    'verifico cosa trovo nel campo particella (su Agea è una stringa)
                    'se trovo dei numeri li metto in numero
                    'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
                    Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
                    Numero = objCOre.Numero_from_Stringa(strNumero)
                    NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                    If Subalterno = "" AndAlso NumeroStringa <> "" Then
                        Subalterno = Left(NumeroStringa, 3)
                    End If

                    'DR.Item("PROV") = Prov
                    'DR.Item("COM") = Com
                    'DR.Item("Sezione") = Sezione
                    'DR.Item("Foglio") = Foglio
                    'DR.Item("Numero") = Numero
                    'DR.Item("Subalterno") = Subalterno

                    'DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                    '                     DR.Item("COM") & ":_" & _
                    '                     DR.Item("Sezione") & ":_" & _
                    '                     DR.Item("Foglio").ToString & ":_" & _
                    '                     DR.Item("Numero").ToString & ":_" & _
                    '                     DR.Item("Subalterno")

                    TitoloPossesso = Converti_TitoliPossesso_Fascicolo(territorio.codiceTipoConduzione, TitoloPossessoDes)
                    'DR.Item("possesso") = TitoloPossessoDes

                    Inizio_Possesso = #1/1/1900#
                    Fine_Possesso = #12/31/2100#
                    Inizio_Possesso_Old = #1/1/1900#
                    Fine_Possesso_Old = #12/31/2100#

                    If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                        Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                        If Inizio_Possesso < AGRODATAINIZIO Then
                            Inizio_Possesso = AGRODATAINIZIO
                        End If
                    End If
                    If territorio.DataFineConduzione IsNot Nothing AndAlso
                        territorio.DataFineConduzione <> "" AndAlso
                        territorio.DataFineConduzione <> "99991231" AndAlso
                        territorio.DataFineConduzione <> "99990101" Then
                        Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
                    End If

                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                    '                          If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                    supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                    Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                    'DR.Item("sup") = Format(supCatasto, "0.0000")

                    supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
                    'DR.Item("supcondotta") = Format(supConduzione, "0.0000")

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
                    Dim Specie_Des As String
                    Dim Varieta_Des As String
                    Dim Utilizzo_Sup As Double
                    Dim HashMacrousi As New Hashtable
                    strMacrousi = ""
                    strUtilizzi = ""

                    If territorio.Destinazione IsNot Nothing Then

                        For j = 0 To territorio.Destinazione.Length - 1

                            'Eleggibilita = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).SuperficieEligibile
                            Qualita = territorio.Destinazione(j).codiceQualita

                            strMacrousi = ""
                            strUtilizzi = ""

                            Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                            Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                            'MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory, _
                            '           objParametri_Server.LogFileName, _
                            '           objParametri_Server.LogDescrizioneUtente, _
                            '           NomeRoutine, _
                            '           MsgOK)

                            If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                                HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                                Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                                strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             "", "",
                                                                                             objParametri_Server) & " (" & Macrouso_Sup & " Ha)<br/>"



                                '*******************************************************************************************************************
                                '******   UTILIZZO    **********************************************************************************
                                '*******************************************************************************************************************

                                If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                                    For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                                        strUtilizzi = ""

                                        Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                                        Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                                        Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).SuperficieUtilizzata) / 10000.0

                                        Specie_Des = ""
                                        Varieta_Des = ""

                                        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

                                        Dim Veg_Cod As Integer = 0
                                        Dim Cul_Cod As Integer = 0
                                        Dim Grfi_Cod As Integer = 0
                                        Dim Id_Cod As Integer = 0
                                        Dim Grva_Cod As Integer = 0

                                        Dim Uso_Cod_Agea As String = ""
                                        Dim Occupazione_Cod_Agea As String = ""
                                        Dim Destinazione_Cod_Agea As String = ""
                                        Dim Qualita_Cod_Agea As String = ""

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
                                                            objParametri_Server)

                                        Specie_Des &= " (Cod." & Specie_Cod & ")"

                                        If Varieta_Des <> "" Then
                                            Varieta_Des &= " (Cod." & Varieta_Cod & ")"
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

                                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                             DR.Item("COM") & ":_" &
                                                             DR.Item("Sezione") & ":_" &
                                                             DR.Item("Foglio").ToString & ":_" &
                                                             DR.Item("Numero").ToString & ":_" &
                                                             DR.Item("Subalterno")

                                        DR.Item("possesso") = TitoloPossessoDes
                                        DR.Item("TitoloPossesso") = TitoloPossesso

                                        DR.Item("inizio_possesso") = Inizio_Possesso
                                        DR.Item("fine_possesso") = Fine_Possesso

                                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                        supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                        DR.Item("sup") = Format(supCatasto, "0.0000")

                                        supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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

                                        DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                                        DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                                        DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                                        DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                                        DR.Item("Scarto") = 0

                                        'DR.Item("qualita") = Qualita
                                        'DR.Item("eleggibilita") = Eleggibilita

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

                                    DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                         DR.Item("COM") & ":_" &
                                                         DR.Item("Sezione") & ":_" &
                                                         DR.Item("Foglio").ToString & ":_" &
                                                         DR.Item("Numero").ToString & ":_" &
                                                         DR.Item("Subalterno")

                                    DR.Item("possesso") = TitoloPossessoDes
                                    DR.Item("TitoloPossesso") = TitoloPossesso
                                    DR.Item("inizio_possesso") = Inizio_Possesso
                                    DR.Item("fine_possesso") = Fine_Possesso

                                    DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")


                                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                    '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                                    DR.Item("Uso_Cod_Agea") = ""
                                    DR.Item("Occupazione_Cod_Agea") = ""
                                    DR.Item("Destinazione_Cod_Agea") = ""
                                    DR.Item("Qualita_Cod_Agea") = ""

                                    DR.Item("Scarto") = 1

                                    'DR.Item("qualita") = Qualita
                                    'DR.Item("eleggibilita") = Eleggibilita

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

                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                             DR.Item("COM") & ":_" &
                                             DR.Item("Sezione") & ":_" &
                                             DR.Item("Foglio").ToString & ":_" &
                                             DR.Item("Numero").ToString & ":_" &
                                             DR.Item("Subalterno")

                        DR.Item("possesso") = TitoloPossessoDes
                        DR.Item("TitoloPossesso") = TitoloPossesso
                        DR.Item("inizio_possesso") = Inizio_Possesso
                        DR.Item("fine_possesso") = Fine_Possesso

                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                        DR.Item("Uso_Cod_Agea") = ""
                        DR.Item("Occupazione_Cod_Agea") = ""
                        DR.Item("Destinazione_Cod_Agea") = ""
                        DR.Item("Qualita_Cod_Agea") = ""

                        DR.Item("Scarto") = 1

                        'DR.Item("qualita") = ""
                        'DR.Item("eleggibilita") = ""

                        DtParticelle.Rows.Add(DR)

                    End If

                Next 'ISWSTerritorio1.Length

                Dim objImportazione As New SincroAnagrafeBA1.Importazione_Fascicolo
                stringaPianificazione = ""
                objImportazione.Crea_Stringa_Catasto(stringaAnagrafica,
                                                    stringaPianificazione,
                                                    XmlUtente.OuterXml,
                                                    XmlUtenteP.OuterXml,
                                                    piva, 0,
                                                    DataInizio, DataFine, Anno_Piano_Colturale,
                                                    DtParticelle,
                                                    objParametri_Server, aggregaCentri, ASG_ProgressivoGIAS)
                'stringaAnagrafica = XmlUtente.OuterXml
                'stringaPianificazione = ""

                If stringaAnagrafica <> "" Then

                    Dim strRisultato As String = ""
                    Dim strErr As String = ""
                    stringaPianificazione = ""
                    Importa_Dati(ASG_Utente_Password, stringaAnagrafica, stringaPianificazione, strErr, strRisultato, True, True, objParametri_Server, objParametri_Utenti)

                    'If strErr = "" Then
                    '    '----------------------------------------------------------
                    '    'importo il profilo della condizionalità
                    '    If Not DtCondizionalita Is Nothing AndAlso DtCondizionalita.Rows.Count > 0 Then
                    '        objImportazione.Importa_Condizionalita_Profilo(TopCode, BaseCode, Regolamento_Condizionalita, Piva, DtCondizionalita, objParametri_Server)
                    '    End If

                    '    ''----------------------------------------------------------
                    '    ''apro le pratiche
                    '    'If ApriPraticaRegFert Then
                    '    '    objImportazione.Importa_Pratica(4, "Registro fertilizzazioni", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If
                    '    'If ApriPraticaRegFito Then
                    '    '    objImportazione.Importa_Pratica(3, "Registro trattamenti (DM 290/01)", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If

                    'End If




                    If strRisultato <> "" Then
                        'objLog.Scrivi_LOG(Directory_Log,
                        '                 File_Log,
                        '                 objParametri_Server.LogDescrizioneUtente,
                        '                 "",
                        '                 i_azienda & " - " & "IMPORTATA l'impresa " & Rag_Soc & " (PIVA:" & piva & " - CUAA:" & cuaa & ")!")

                    End If

                End If
            End If
        End If

    End Sub

    Private Shared Sub ImportaCatastoFascicolo_ISWSRespAnagFascicolo2(ByRef fascicolo As AGEA_Coordinamento.ISWSRespAnagFascicolo2,
                                                    ByRef ISWSTerritorioFS6 As List(Of AGEA_Coordinamento.ISWSTerritorio1),
                                                    piva As String,
                                                    cuaa As String,
                                                    aggregaCentri As Boolean,
                                                    objParametri_Server As AgronicaCoreParametri,
                                                    objParametri_Utenti As AgronicaCoreParametri,
                                                    ASG_Utente_Username As String,
                                                    ASG_Utente_Password As String,
                                                    ASG_ProgressivoGIAS As String)


        Dim Rag_Soc As String = ""
        'Dim ID_Azienda As String = ""
        Dim EsisteImpresa As Boolean = False
        'Dim ApriPraticaRegFert As Boolean = False
        'Dim ApriPraticaRegFito As Boolean = False
        Dim objGerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        Dim Piva_Padre As String = objGerarchiaImprese.LeggiPadre(piva, objParametri_Server, "")

        Dim Indirizzo As String = ""
        Dim Cap As String = ""
        Dim Comune As String = ""
        Dim Istat_Provincia As String = ""
        Dim Istat_Comune As String = ""
        'Dim Cod_Belfiore As String = ""

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
        Dim TitoloPossessoDes As String = ""
        Dim Inizio_Possesso As Date
        Dim Fine_Possesso As Date
        Dim Inizio_Possesso_Old As Date
        Dim Fine_Possesso_Old As Date

        Dim Presente_Fascicolo As Boolean = False
        Dim Sigla_Provincia As String = ""
        Dim XmlAnagrafica As New XmlDocument

        Dim XmlUtente As XmlElement
        Dim XmlImpresa As XmlElement
        Dim XmlFascicolo As XmlElement = Nothing
        Dim XmlCentro As XmlElement
        Dim XmlFabbricato As XmlElement
        Dim XmlDoc As String
        Dim XmlDocP As String

        Dim XmlPianificazione As New System.Xml.XmlDocument

        Dim XmlUtenteP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XmlFascicoloP As System.Xml.XmlElement = Nothing

        Dim stringaAnagrafica As String = ""
        Dim stringaPianificazione As String

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        'Dim Directory_Log As String
        'Dim File_Log As String
        'Dim MsgOK As String = ""
        'Dim i_azienda As Integer

        'Dim objLog As New AgronicaCoreDataProvider.LogProvider

        'Dim Regolamento_Condizionalita As String
        Dim CodiceChiaveCliente As String
        Dim LinkWSImportaGIAS As String
        Dim Anno_Piano_Colturale As Integer
        Dim TopCode As Integer
        Dim BaseCode As Integer
        Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        Dim DataInizioMandato As Date = AGRODATAINIZIO

        CodiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)
        LinkWSImportaGIAS = objConfSiti.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)

        If fascicolo Is Nothing Then
            Exit Sub
        End If
        If fascicolo.dataSottMandato IsNot Nothing Then
            DataInizioMandato = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSottMandato))
        End If
        If fascicolo.dataAperturaFascicolo IsNot Nothing Then
            DataAperturaFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataAperturaFascicolo))
        End If
        If fascicolo.dataChiusuraFascicolo IsNot Nothing Then
            DataChiusuraFascicolo = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataChiusuraFascicolo))
        End If
        Dim dataScheda As Date
        If fascicolo.dataSchedaValidazione IsNot Nothing Then
            dataScheda = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(fascicolo.dataSchedaValidazione))
        End If
        If CDate(dataScheda) <> AGRODATAINIZIO Then
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione " & Key_SchedaDataValidazione & ")"
        Else
            'label_fascicolo.InnerText = "Fascicolo N." & Key_Scheda & " (Data Validazione non presente)"
        End If

        '*******************************************************************************************************************
        '******   IMPRESA      *********************************************************************************************
        '*******************************************************************************************************************
        '(20/02/2015 fede aggiunti casi G,P x AVEPA)
        Dim tipoAzienda As String = CStr(fascicolo.tipoAzienda)

        Select Case tipoAzienda
            Case "PF", "P", "1", "0"
                Rag_Soc = fascicolo.denominazione & " " & fascicolo.nomePF
            Case "PG", "G"
                Rag_Soc = fascicolo.denominazione
        End Select


        'ID_Azienda = ws_AnaResponse.output.anagrafica.identita.master
        'Cod_Belfiore = ws_AnaResponse.output.anagrafica.impresa.indirizzoSedePrincipale.comune.codErariale

        Sigla_Provincia = ""
        Comune = ""

        'verifico INDIRIZZO
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
            'MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & scheda.numeroScheda & " -  " & "Impossibile inserire l'impresa poiché manca l'indirizzo!"
            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory,
            '     objParametri_Server.LogFileName,
            '     objParametri_Server.LogDescrizioneUtente,
            '     NomeRoutine,
            '    MsgOK)
            'Lbl_Errore_Ricerca.Text = MsgOK
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


        ' Label
        'Me.Lbl_PreImpresa.Text = "L'impresa"
        'Me.Lbl_Impresa.Text = RagSoc & " (P.IVA : " & Piva & ")"

        'If Presente_Fascicolo = False Or VisualizzaComboPadri Then
        '    Cmb_Padre.SelectedIndex = Cmb_Padre.Items.IndexOf(Cmb_Padre.Items.FindByValue("UP103" & Istat_Provincia & "000"))
        'End If

        'Tabella_Impresa.Visible = True

        'Lbl_Dettagli_Impresa.Text = RagSoc & "<br/>" & _
        '                               "<b>Piva:</b> " & Piva & "<br/><br/>" & _
        '                               "<b>Sede legale:</b><br/>" & _
        '                               Indirizzo & "<br/>" & _
        '                               Cap & " " & _
        '                               Comune & ", (" & Sigla_Provincia & ")<br/>"


        '-----------------------------------------------
        '------- UTENTE        -------------------------
        '-----------------------------------------------
        XmlUtente = objXML.Xml_Pubblico_Utente(XmlAnagrafica,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)

        XmlUtenteP = objXML.Xml_Pubblico_Utente(XmlPianificazione,
                                               ASG_Utente_Username,
                                               ASG_Utente_Password,
                                               ASG_ProgressivoGIAS)


        Legale_Rappresentante = "#"
        Legale_Rappresentante_Cognome = "#"
        Legale_Rappresentante_Nome = "#"
        Legale_Rappresentante_CF = "#"
        Legale_Rappresentante_Sesso = "#"
        Legale_Rappresentante_Indirizzo = "#"
        Legale_Rappresentante_Frazione = "#"
        Legale_Rappresentante_Cap = "#"
        Legale_Rappresentante_Comune = "#"
        Legale_Rappresentante_Provincia = "#"
        Legale_Rappresentante_Stato = "#"
        Legale_Rappresentante_Istat_Comune = "#"
        Legale_Rappresentante_Istat_Provincia = "#"
        Legale_Rappresentante_Data_Nascita = "#"
        Legale_Rappresentante_Comune_Nascita = "#"
        Legale_Rappresentante_Provincia_Nascita = "#"
        Legale_Rappresentante_Istat_Comune_Nascita = "#"
        Legale_Rappresentante_Istat_Provincia_Nascita = "#"
        Legale_Rappresentante_Rubrica1 = "#"
        Legale_Rappresentante_Rubrica2 = "#"
        Legale_Rappresentante_Rubrica3 = "#"
        Legale_Rappresentante_Rubrica4 = "#"
        Legale_Rappresentante_Rubrica5 = "#"

        '-----------------------------------------------
        '------- IMPRESA       -------------------------
        '-----------------------------------------------

        'If Presente_Fascicolo = False Or VisualizzaComboPadri Then
        '    Cmb_Padre.Items.Add("00225020239")
        '    Piva_Padre = Cmb_Padre.SelectedItem.Value

        '    ViewState("Piva_Padre_OLD") = Piva_Padre

        'End If
        Dim tipoOperazione As enum_TipoOperazioneDB
        If piva <> "" Then
            Dim dt_imp = objImpresa.Leggi(piva, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dt_imp.Rows.Count > 0 Then
                EsisteImpresa = True
            End If
            If EsisteImpresa Then
                tipoOperazione = enum_TipoOperazioneDB.Modifica
            Else
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If
        End If

        'tipoOperazione = enum_TipoOperazioneDB.Lettura
        XmlImpresa = objXML.Xml_Pubblico_Impresa(tipoOperazione,
                                                 piva,
                                                 Rag_Soc,
                                                 cuaa,
                                                 cuaa,
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
                                                 XmlAnagrafica)

        If Presente_Fascicolo Then
            XmlImpresa.AppendChild(XmlFascicolo)
        End If
        Dim tipoOperazione_Centro As New enum_TipoOperazioneDB
        If aggregaCentri Then
            Dim SaCod As Integer = 0
            Dim SaNome As String = "Centro n.01"

            tipoOperazione_Centro = tipoOperazione

            If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                'verifico se c'è almeno un centro in archivio, in caso contrario
                'preparo l'xml di creazione
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            Else
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva,
                                            0,
                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    'almeno un centro è presente
                    SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                    SaNome = Dt_Centri.Rows(0).Item("sa_nome")
                Else
                    'c'è solo l'azienda in archivio, devo creare il centro
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                End If
            End If
            'tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                            XmlAnagrafica)

            'Creo il fabbricato
            '1. in caso di primo inserimento
            '2. in caso di modifica se non esiste già un magazzino
            Dim CreaMagazzino As Boolean = False
            If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                CreaMagazzino = True
            Else
                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                Dim DtFabb As DataTable
                DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                     " Tipo_Fabbricato_Cod=20 ",
                                     "", objParametri_Server)
                If DtFabb.Rows.Count = 0 Then
                    CreaMagazzino = True
                End If
            End If

            If CreaMagazzino Then

                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura,
                                                              "0",
                                                              "Magazzino n.01",
                                                              20,
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
                                                              XmlAnagrafica)

                XmlCentro.AppendChild(XmlFabbricato)

            End If

            XmlImpresa.AppendChild(XmlCentro)
        Else
            Dim listCom As New List(Of String)
            For Each Comm In ISWSTerritorioFS6
                If Not listCom.Contains(Comm.Comune & "|" & Comm.Provincia) Then
                    listCom.Add(Comm.Comune & "|" & Comm.Provincia)
                End If
            Next
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim listSaCod As New List(Of String)
            For Each Com In listCom
                Dim dtCom = objIstat.Leggi(Com.Split("|")(1), Com.Split("|")(0), "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtCom IsNot Nothing AndAlso dtCom.Rows.Count > 0 Then
                    Dim com_des = dtCom.Rows(0).Item("LOCALITA")

                    Dim SaCod As Integer = 0
                    Dim SaNome As String = com_des
                    tipoOperazione_Centro = tipoOperazione

                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        'verifico se c'è almeno un centro in archivio, in caso contrario
                        'preparo l'xml di creazione
                        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                        Dim Dt_Centri As DataTable
                        Dt_Centri = objCentri.Leggi(piva,
                                                    0,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    " Sa_Nome=" & UtilityProvider.Agro_SQL_SaveText_NULL(SaNome) & "", "",
                                                    objParametri_Server)

                        If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                            'almeno un centro è presente
                            SaCod = Dt_Centri.Rows(0).Item("sa_cod")
                            SaNome = Dt_Centri.Rows(0).Item("sa_nome")

                        Else
                            Dim objSequenze As New Agro_Sequenze
                            UtilityProvider.Calcola_BaseCode_TopCode(BaseCode,TopCode, ASG_ProgressivoGIAS)
                            SaCod = objSequenze.NuovoId_CentriAziendali(
                                        piva,
                                        BaseCode,
                                        TopCode,
                                        objParametri_Server)
                            tipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
                        End If
                    End If
                    listSaCod.Add(SaCod)
                    tipoOperazione_Centro = enum_TipoOperazioneDB.Lettura
                    XmlCentro = objXML.Xml_Pubblico_CentroAziendale(tipoOperazione_Centro,
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
                                                                    XmlAnagrafica)

                    'Creo il fabbricato
                    '1. in caso di primo inserimento
                    '2. in caso di modifica se non esiste già un magazzino
                    Dim CreaMagazzino As Boolean = False
                    If tipoOperazione = enum_TipoOperazioneDB.Scrittura Then
                        CreaMagazzino = True
                    Else
                        Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                        Dim DtFabb As DataTable
                        DtFabb = objFabb.Leggi(piva, SaCod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                             " Tipo_Fabbricato_Cod=20 ",
                                             "", objParametri_Server)
                        If DtFabb.Rows.Count = 0 Then
                            CreaMagazzino = True
                        End If
                    End If

                    If CreaMagazzino Then

                        XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Lettura,
                                                                      "0",
                                                                      "Magazzino n.01",
                                                                      20,
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
                                                                      XmlAnagrafica)

                        XmlCentro.AppendChild(XmlFabbricato)
                    End If
                    XmlImpresa.AppendChild(XmlCentro)
                End If
            Next
        End If

        XmlUtente.AppendChild(XmlImpresa)

        XmlDoc = XmlUtente.OuterXml


        '*******************************************************************************************************************
        '******   CONDIZIONALITA    ****************************************************************************************
        '*******************************************************************************************************************

        'verifico se esiste già su GIAS un PROFILO VALIDO...
        'se esiste NON LO CREO ORA!!!
        'Dim DLL_AD_Condizionalita As New AccessoDB_Condizionalita.AccessoDati(r"))
        'Dim Dt_Interviste As DataTable
        Dim CreaProfilo As Boolean = True

        'Dim objAuditReg As New AgronicaCoreAuditDAL.Audit_Regolamenti_R
        'Dim DtReg As DataTable
        Dim DataInizioReg, DataFineReg As Date

        DataInizioReg = New DateTime(CDate(dataScheda).Year, 1, 1)
        DataFineReg = New DateTime(CDate(dataScheda).Year, 12, 31)

        ' se ce n'è più di uno prendo cmq il primo, il più recente
        'DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, DataInizioReg, DataFineReg, "", "", objParametri_Server)

        'If DtReg.Rows.Count > 0 Then
        '    Qs_RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
        '    ViewState("Audit_Regolamento") = Qs_RegolamentoCod
        'Else
        '    ' leggo il regolamento audit più recente
        '    DtReg = objAuditReg.Leggi_RegolamentoValido(enum_AuditPuaTipo.Audit_Condizionalita, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
        '    If DtReg.Rows.Count > 0 Then
        '        Qs_RegolamentoCod = DtReg.Rows(0).Item("Regolamento_Cod")
        '        ViewState("Audit_Regolamento") = DtReg.Rows(0).Item("Regolamento_Cod")
        '    End If

        'End If

        'Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_R
        'Dt_Interviste = objAudit_Interviste.Leggi(1, _
        '                                          Qs_RegolamentoCod, _
        '                                          0, _
        '                                          Piva, _
        '                                          AGRODATAINIZIO, AGRODATAFINE, _
        '                                          "", "", _
        '                                          objParametri_Server)

        'If Not Dt_Interviste Is Nothing AndAlso Dt_Interviste.Rows.Count > 0 Then
        '    CreaProfilo = False
        'End If

        If CreaProfilo Then



        Else
            'profilo già presente
            'Riga_Controllo_Profilo.Visible = True
            'Riga_Condizionalita.Visible = False
        End If

        '*******************************************************************************************************************
        '******   PARTICELLE CATASTALI    **********************************************************************************
        '*******************************************************************************************************************

        Dim DtParticelle As New DataTable

        If ISWSTerritorioFS6 IsNot Nothing Then

            If ISWSTerritorioFS6.Count > 0 Then

                Dim DataInizio As Date = AGRODATAINIZIO
                Dim DataFine As Date = AGRODATAFINE
                'If IsDate(Txt_ValiditaInizio.Text) Then
                '    DataInizio = CDate(Txt_ValiditaInizio.Text)
                'End If
                'If IsDate(Txt_ValiditaFine.Text) Then
                '    DataFine = CDate(Txt_ValiditaFine.Text)
                'End If

                'Riga_Particelle.Visible = True

                Dim strValiditaInizio As String = String.Empty
                Dim strValiditaFine As String = String.Empty
                Dim DataInizioUtente As Date = AGRODATAINIZIO
                Dim DataFineUtente As Date = AGRODATAFINE

                Dim Anno As Integer = Today.Year
                If CDate(dataScheda) <> AGRODATAINIZIO Then
                    Anno = CDate(dataScheda).Year
                End If

                Dim Dt_Impost As DataTable
                Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim DrFiltro() As DataRow
                Dim Impostazione_Valore_Date As String = ""
                Dim Impostazione_Valore_N_Fascicolo As String = ""

                Dt_Impost = objImpost.Leggi2(1,
                                            objParametri_Server.UtenteUsername,
                                            0,
                                            "", "",
                                            objParametri_Utenti)

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_Date)

                objImpost.AnnataAgraria(CDate(dataScheda), DataInizioUtente, DataFineUtente, objParametri_Utenti)

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_Date = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_Date
                        Case "1" 'validita fascicoli
                            strValiditaInizio = dataScheda
                            Dim Key_SchedaDataFine = fascicolo.dataChiusuraFascicolo
                            If dataScheda <> "" Then
                                strValiditaFine = Key_SchedaDataFine
                            Else
                                strValiditaFine = DataFineUtente.ToShortDateString
                            End If

                        Case Else 'anno
                            strValiditaInizio = DataInizioUtente.ToShortDateString
                            strValiditaFine = DataFineUtente.ToShortDateString
                    End Select
                Else
                    strValiditaInizio = DataInizioUtente.ToShortDateString
                    strValiditaFine = DataFineUtente.ToShortDateString
                End If

                'Dim strValiditaInizio As String = String.Empty
                'Dim strValiditaFine As String = String.Empty
                'strValiditaInizio = "01/11/" & (Anno - 1).ToString
                'strValiditaFine = "31/10/" & Anno.ToString

                '*******************************************************************************************************************
                '******   PIANIFICAZIONE    **********************************************************************************

                Dim strPianificazione As String

                DrFiltro = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_Planning_NValidazioneNome)

                strPianificazione = "Piano Colturale " & Anno.ToString

                If DrFiltro IsNot Nothing AndAlso DrFiltro.Length > 0 Then
                    Impostazione_Valore_N_Fascicolo = DrFiltro(0).Item("Impostazione_Valore_1")
                    Select Case Impostazione_Valore_N_Fascicolo
                        Case "1" 'validita fascicoli
                            If CDate(dataScheda) <> AGRODATAINIZIO Then
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione " & dataScheda & ")"
                            Else
                                strPianificazione = "Piano Colturale " & Anno.ToString & " (Fascicolo N." & dataScheda & " Data Validazione non presente)"
                            End If
                            'Case Else
                            '    strPianificazione = "Piano Colturale " & Anno.ToString
                    End Select
                End If

                XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(enum_TipoOperazioneDB.Scrittura,
                                   "0",
                                   piva,
                                   strPianificazione,
                                   strPianificazione,
                                   "Importazione " & strPianificazione,
                                   enum_Planning_Fonte.Avepa,
                                   enum_TipoPianificazione.Pianificazione_Annuale,
                                   strValiditaInizio,
                                   strValiditaFine,
                                   XmlPianificazione)

                XmlUtenteP.AppendChild(XmlTestata)

                If Presente_Fascicolo Then
                    XmlTestata.AppendChild(XmlFascicoloP)
                End If

                XmlDocP = XmlUtenteP.OuterXml

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
                'DtParticelle.Columns.Add(New DataColumn("qualita", GetType(String)))
                'DtParticelle.Columns.Add(New DataColumn("eleggibilita", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
                DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

                For Each territorio In ISWSTerritorioFS6

                    'DR = DtParticelle.NewRow

                    Prov = If(IsNothing(territorio.Provincia), "", territorio.Provincia)
                    Com = If(IsNothing(territorio.Comune), "", territorio.Comune)
                    Sezione = If(IsNothing(territorio.Sezione), "", territorio.Sezione)
                    Foglio = If(IsNothing(territorio.Foglio), 0, territorio.Foglio)
                    strNumero = If(IsNothing(territorio.Particella), 0, territorio.Particella)
                    Subalterno = If(IsNothing(territorio.Subalterno), "", territorio.Subalterno)

                    'verifico cosa trovo nel campo particella (su Agea è una stringa)
                    'se trovo dei numeri li metto in numero
                    'se trovo dei caratteri li metto nel subalterno se non è già valorizzato
                    Dim objCOre As New AgronicaCoreDataProvider.UtilityProvider
                    Numero = objCOre.Numero_from_Stringa(strNumero)
                    NumeroStringa = objCOre.Stringa_from_StringaconNumeri(strNumero)
                    If Subalterno = "" AndAlso NumeroStringa <> "" Then
                        Subalterno = Left(NumeroStringa, 3)
                    End If

                    'DR.Item("PROV") = Prov
                    'DR.Item("COM") = Com
                    'DR.Item("Sezione") = Sezione
                    'DR.Item("Foglio") = Foglio
                    'DR.Item("Numero") = Numero
                    'DR.Item("Subalterno") = Subalterno

                    'DR.Item("Catasto") = DR.Item("PROV") & ":" & _
                    '                     DR.Item("COM") & ":_" & _
                    '                     DR.Item("Sezione") & ":_" & _
                    '                     DR.Item("Foglio").ToString & ":_" & _
                    '                     DR.Item("Numero").ToString & ":_" & _
                    '                     DR.Item("Subalterno")

                    TitoloPossesso = Converti_TitoliPossesso_Fascicolo(territorio.codiceTipoConduzione, TitoloPossessoDes)
                    'DR.Item("possesso") = TitoloPossessoDes

                    Inizio_Possesso = #1/1/1900#
                    Fine_Possesso = #12/31/2100#
                    Inizio_Possesso_Old = #1/1/1900#
                    Fine_Possesso_Old = #12/31/2100#

                    If territorio.DataInizioConduzione IsNot Nothing AndAlso territorio.DataInizioConduzione <> "" Then
                        Inizio_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione))
                        If Inizio_Possesso < AGRODATAINIZIO Then
                            Inizio_Possesso = AGRODATAINIZIO
                        End If
                    End If
                    If territorio.DataFineConduzione IsNot Nothing AndAlso
                        territorio.DataFineConduzione <> "" AndAlso
                        territorio.DataFineConduzione <> "99991231" AndAlso
                        territorio.DataFineConduzione <> "99990101" Then
                        Fine_Possesso = CDate(Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione))
                    End If

                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                    '                          If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                    supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                    Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                    'DR.Item("sup") = Format(supCatasto, "0.0000")

                    supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
                    'DR.Item("supcondotta") = Format(supConduzione, "0.0000")

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
                    Dim Specie_Des As String
                    Dim Varieta_Des As String
                    Dim Utilizzo_Sup As Double
                    Dim HashMacrousi As New Hashtable
                    strMacrousi = ""
                    strUtilizzi = ""

                    If territorio.Destinazione IsNot Nothing Then

                        For j = 0 To territorio.Destinazione.Length - 1

                            'Eleggibilita = ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).ISWSUtilizzoTerra1(j).SuperficieEligibile
                            Qualita = territorio.Destinazione(j).codiceQualita

                            strMacrousi = ""
                            strUtilizzi = ""

                            Macrouso_Cod = territorio.Destinazione(j).codiceMacrouso
                            Macrouso_Sup = CDbl(territorio.Destinazione(j).superficieUtilizzata) / 10000.0

                            'MsgOK = Prov & "_" & Com & "_" & Sezione & "_" & Foglio.ToString & "_" & Numero.ToString & "_" & Subalterno & "_" & " Macrouso_Sup: " & Macrouso_Sup.ToString
                            'objLog.Scrivi_LOG(objParametri_Server.LogDirectory, _
                            '           objParametri_Server.LogFileName, _
                            '           objParametri_Server.LogDescrizioneUtente, _
                            '           NomeRoutine, _
                            '           MsgOK)

                            If Not HashMacrousi.ContainsKey(Macrouso_Cod & Qualita) Then

                                HashMacrousi.Add(Macrouso_Cod & Qualita, "")

                                Dim objMacrousi As New AgronicaCoreMetaSchemaDAL.Macrousi_R
                                strMacrousi = objMacrousi.Leggi_MacrousoDes_from_MacrousoCod(territorio.Destinazione(j).codiceMacrouso,
                                                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                             "", "",
                                                                                             objParametri_Server) & " (" & Macrouso_Sup & " Ha)<br/>"



                                '*******************************************************************************************************************
                                '******   UTILIZZO    **********************************************************************************
                                '*******************************************************************************************************************

                                If territorio.Destinazione(j).Dettagli IsNot Nothing Then

                                    For x = 0 To territorio.Destinazione(j).Dettagli.Length - 1

                                        strUtilizzi = ""

                                        Specie_Cod = territorio.Destinazione(j).Dettagli(x).codiceProdotto
                                        Varieta_Cod = territorio.Destinazione(j).Dettagli(x).codiceVarieta
                                        Utilizzo_Sup = CDbl(territorio.Destinazione(j).Dettagli(x).superficieUtilizzata) / 10000.0

                                        Specie_Des = ""
                                        Varieta_Des = ""

                                        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R

                                        Dim Veg_Cod As Integer = 0
                                        Dim Cul_Cod As Integer = 0
                                        Dim Grfi_Cod As Integer = 0
                                        Dim Id_Cod As Integer = 0
                                        Dim Grva_Cod As Integer = 0

                                        Dim Uso_Cod_Agea As String = ""
                                        Dim Occupazione_Cod_Agea As String = ""
                                        Dim Destinazione_Cod_Agea As String = ""
                                        Dim Qualita_Cod_Agea As String = ""

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
                                                            objParametri_Server)

                                        Specie_Des &= " (Cod." & Specie_Cod & ")"

                                        If Varieta_Des <> "" Then
                                            Varieta_Des &= " (Cod." & Varieta_Cod & ")"
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

                                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                             DR.Item("COM") & ":_" &
                                                             DR.Item("Sezione") & ":_" &
                                                             DR.Item("Foglio").ToString & ":_" &
                                                             DR.Item("Numero").ToString & ":_" &
                                                             DR.Item("Subalterno")

                                        DR.Item("possesso") = TitoloPossessoDes
                                        DR.Item("TitoloPossesso") = TitoloPossesso

                                        DR.Item("inizio_possesso") = Inizio_Possesso
                                        DR.Item("fine_possesso") = Fine_Possesso

                                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

                                        supCatasto = CDbl(territorio.SuperficieCatastale) / 10000.0
                                        Conversioni.EttariAreCentiare_from_Ettari(supCatasto, Ettari, Are, Centiare)
                                        DR.Item("sup") = Format(supCatasto, "0.0000")

                                        supConduzione = CDbl(territorio.SuperficieCondotta) / 10000.0
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

                                        DR.Item("Uso_Cod_Agea") = Uso_Cod_Agea
                                        DR.Item("Occupazione_Cod_Agea") = Occupazione_Cod_Agea
                                        DR.Item("Destinazione_Cod_Agea") = Destinazione_Cod_Agea
                                        DR.Item("Qualita_Cod_Agea") = Qualita_Cod_Agea

                                        DR.Item("Scarto") = 0

                                        'DR.Item("qualita") = Qualita
                                        'DR.Item("eleggibilita") = Eleggibilita

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

                                    DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                                         DR.Item("COM") & ":_" &
                                                         DR.Item("Sezione") & ":_" &
                                                         DR.Item("Foglio").ToString & ":_" &
                                                         DR.Item("Numero").ToString & ":_" &
                                                         DR.Item("Subalterno")

                                    DR.Item("possesso") = TitoloPossessoDes
                                    DR.Item("TitoloPossesso") = TitoloPossesso
                                    DR.Item("inizio_possesso") = Inizio_Possesso
                                    DR.Item("fine_possesso") = Fine_Possesso

                                    DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                                If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")


                                    'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                                    '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                                    DR.Item("Uso_Cod_Agea") = ""
                                    DR.Item("Occupazione_Cod_Agea") = ""
                                    DR.Item("Destinazione_Cod_Agea") = ""
                                    DR.Item("Qualita_Cod_Agea") = ""

                                    DR.Item("Scarto") = 1

                                    'DR.Item("qualita") = Qualita
                                    'DR.Item("eleggibilita") = Eleggibilita

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

                        DR.Item("Catasto") = DR.Item("PROV") & ":" &
                                             DR.Item("COM") & ":_" &
                                             DR.Item("Sezione") & ":_" &
                                             DR.Item("Foglio").ToString & ":_" &
                                             DR.Item("Numero").ToString & ":_" &
                                             DR.Item("Subalterno")

                        DR.Item("possesso") = TitoloPossessoDes
                        DR.Item("TitoloPossesso") = TitoloPossesso
                        DR.Item("inizio_possesso") = Inizio_Possesso
                        DR.Item("fine_possesso") = Fine_Possesso

                        DR.Item("datepossesso") = If(territorio.DataInizioConduzione <> "19000101", "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataInizioConduzione), "Dal ...") &
                                                    If(territorio.DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(territorio.DataFineConduzione), " al ...")

                        'DR.Item("datepossesso") = "Dal " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataInizioConduzione) & _
                        '                        If(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione <> "99991231", " al " & Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse.out.fascicolo.fascicolo.ISWSTerritorio1(i).DataFineConduzione), "")

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

                        DR.Item("Uso_Cod_Agea") = ""
                        DR.Item("Occupazione_Cod_Agea") = ""
                        DR.Item("Destinazione_Cod_Agea") = ""
                        DR.Item("Qualita_Cod_Agea") = ""

                        DR.Item("Scarto") = 1

                        'DR.Item("qualita") = ""
                        'DR.Item("eleggibilita") = ""

                        DtParticelle.Rows.Add(DR)

                    End If

                Next 'ISWSTerritorio1.Length

                Dim objImportazione As New SincroAnagrafeBA1.Importazione_Fascicolo
                stringaPianificazione = ""
                objImportazione.Crea_Stringa_Catasto(stringaAnagrafica,
                                                    stringaPianificazione,
                                                    XmlUtente.OuterXml,
                                                    XmlUtenteP.OuterXml,
                                                    piva, 0,
                                                    DataInizio, DataFine, Anno_Piano_Colturale,
                                                    DtParticelle,
                                                    objParametri_Server, aggregaCentri, ASG_ProgressivoGIAS)
                'stringaAnagrafica = XmlUtente.OuterXml
                'stringaPianificazione = ""

                If stringaAnagrafica <> "" Then

                    Dim strRisultato As String = ""
                    Dim strErr As String = ""
                    stringaPianificazione = ""
                    Importa_Dati(ASG_Utente_Password, stringaAnagrafica, stringaPianificazione, strErr, strRisultato, True, True, objParametri_Server, objParametri_Utenti)

                    'If strErr = "" Then
                    '    '----------------------------------------------------------
                    '    'importo il profilo della condizionalità
                    '    If Not DtCondizionalita Is Nothing AndAlso DtCondizionalita.Rows.Count > 0 Then
                    '        objImportazione.Importa_Condizionalita_Profilo(TopCode, BaseCode, Regolamento_Condizionalita, Piva, DtCondizionalita, objParametri_Server)
                    '    End If

                    '    ''----------------------------------------------------------
                    '    ''apro le pratiche
                    '    'If ApriPraticaRegFert Then
                    '    '    objImportazione.Importa_Pratica(4, "Registro fertilizzazioni", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If
                    '    'If ApriPraticaRegFito Then
                    '    '    objImportazione.Importa_Pratica(3, "Registro trattamenti (DM 290/01)", Piva, Cuaa, DataInizio, AGRODATAFINE, objParametri_Server, DataInizio)
                    '    'End If

                    'End If




                    If strRisultato <> "" Then
                        'objLog.Scrivi_LOG(Directory_Log,
                        '                 File_Log,
                        '                 objParametri_Server.LogDescrizioneUtente,
                        '                 "",
                        '                 i_azienda & " - " & "IMPORTATA l'impresa " & Rag_Soc & " (PIVA:" & piva & " - CUAA:" & cuaa & ")!")

                    End If

                End If
            End If
        End If

    End Sub

    Public Shared Function Converti_TitoliPossesso_Fascicolo(ByVal TitoloPossesso As AGEA_Coordinamento.TipoConduzione,
                                                   ByRef TitoloPossessoDes As String) As Integer

        Dim Particella_Possesso As Integer

        Select Case TitoloPossesso

            Case AGEA_Coordinamento.TipoConduzione.Item1
                Particella_Possesso = 1 'Proprietà
                TitoloPossessoDes = "Proprieta"
            Case AGEA_Coordinamento.TipoConduzione.Item2
                Particella_Possesso = 3 'Affitto con contratto
                TitoloPossessoDes = "Affitto"
            Case Else
                Particella_Possesso = 0 'altro
                TitoloPossessoDes = "Altro"
        End Select

        Return Particella_Possesso

    End Function

    Private Shared Function FascicoloMemorizza(ByVal Fonte_Cod As Integer, ByVal Piva As String, ByVal dataValidazione As DateTime, ByVal numeroValidazione As String, strFascicoli As String, objParametriServer As AgronicaCoreParametri, ByRef allegatoDocumentocod As Integer, Codice_Detentore As String) As Boolean

        Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim fascicoloAllegatiScrittura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

        Dim esisteFascicolo As Boolean

        esisteFascicolo = fascicoloAllegatiLettura.EsisteDocumento_Da_Numero(numeroValidazione, Piva, enum_CategorieDocumenti.DomandaFascicolo, allegatoDocumentocod, objParametriServer, Fonte_Cod)

        If Not esisteFascicolo Then

            fascicoloAllegatiScrittura.Scrivi(Piva, "", enum_CategorieDocumenti.DomandaFascicolo, "", numeroValidazione, Fonte_Cod, "", AGRODATAINIZIO, AGRODATAFINE, allegatoDocumentocod, objParametriServer, dataValidazione, strXml:=strFascicoli, Codice_Detentore:=Codice_Detentore)

        Else

            fascicoloAllegatiScrittura.Modifica_FascicoloXML(allegatoDocumentocod, Piva, Fonte_Cod, strFascicoli, objParametriServer)

        End If

        Return esisteFascicolo

    End Function

End Class
