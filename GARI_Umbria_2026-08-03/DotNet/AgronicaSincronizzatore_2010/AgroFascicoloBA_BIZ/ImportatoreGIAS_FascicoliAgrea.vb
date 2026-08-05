Imports System.Text
Imports System.IO
Imports System.Security.Cryptography
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Importazione_Agea_BIZ
Imports Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it
Imports Importazione_Agrea_WS
Imports System.Web.Security
Imports AGEA_Coordinamento
Imports AgronicaCoreDataProvider

Public Class ImportatoreGIAS_FascicoliAgrea
    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private objLog As AgronicaCoreDataProvider.LogProvider

    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private ParametriExtra As String

    Private username As String
    Private password As String

    Private linkWS_Agrea As String
    Private username_Agrea As String
    Private password_Agrea As String
    Private limiteGiornaliero As Integer
    Private anniDaImportare As List(Of Integer)

    Public Sub New(ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   objLog As AgronicaCoreDataProvider.LogProvider,
                   LogFileName As String,
                     LogDirectory As String,
                     LogDescrizioneUtente As String,
                     DirectoryFileImportazioni As String,
                     DirectoryFileEsportazioni As String,
                     ParametriExtra As String,
                     username As String,
                     password As String,
                     linkWS_Agrea As String,
                     username_Agrea As String,
                     password_Agrea As String,
                     anniDaImportare As List(Of Integer))
        Me.ObjParametri_SuperServer = ObjParametri_SuperServer
        Me.ObjParametri_Server = ObjParametri_Server
        Me.ObjParametri_Utenti = ObjParametri_Utenti
        Me.objLog = objLog
        Me.LogFileName = LogFileName
        Me.LogDirectory = LogDirectory
        Me.LogDescrizioneUtente = LogDescrizioneUtente
        Me.DirectoryFileImportazioni = DirectoryFileImportazioni
        Me.DirectoryFileEsportazioni = DirectoryFileEsportazioni
        Me.ParametriExtra = ParametriExtra
        Me.username = username
        Me.password = password
        Me.linkWS_Agrea = linkWS_Agrea
        Me.username_Agrea = username_Agrea
        Me.password_Agrea = password_Agrea
        Me.limiteGiornaliero = limiteGiornaliero
        Me.anniDaImportare = anniDaImportare
    End Sub

    Public Function importaAgrea() As Boolean
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 20, ObjParametri_Server)
        Dim fascicoliGiaInseriti As Integer = 0
        If dt1.Rows.Count > 0 Then
            If Not (IsDBNull(dt1.Rows(0).Item("numeroFascicoliCaricati"))) Then
                fascicoliGiaInseriti = CInt(dt1.Rows(0).Item("numeroFascicoliCaricati"))
            End If
        Else
            writer.resettaCuaaDaImportare(ObjParametri_Server, 0, 20)
        End If
        Dim objAgreaDLL As New Sincro_Agrea2Gias.RecuperaDati
        Dim i = 0
        Dim listaCuaa As New List(Of String)
        For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 20).Rows
            listaCuaa.Add(r.Item("cuaa"))
        Next
        If listaCuaa.Count = 0 Then
            For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, -1, 20).Rows
                listaCuaa.Add(r.Item("cuaa"))
            Next
            If listaCuaa.Count = 0 Then
                writer.resettaCuaaDaImportare(ObjParametri_Server, 1, 20)
                For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 10000, 0, 20).Rows
                    listaCuaa.Add(r.Item("cuaa"))
                Next
            End If
        End If
        For Each Cuaa As String In listaCuaa

            Dim ErrMsg As String = ""

            For Each anno In anniDaImportare
                Dim FascicoliInseriti As Integer = 0
                Dim errori As Integer = 0
                Dim ErrCOD As Integer
                Try
                    Dim Caa_ID, Caa_Des, detentoreAGEA As String
                    Dim idCaa = getCaa(Cuaa, anno, ObjParametri_Server, "http://cooperazione.sian.it/wspdd/services/OprFascicolo", "wspddusrrgmbrkron", "krypto.739A", Caa_ID, Caa_Des, detentoreAGEA)
                    If idCaa Is Nothing Then
                        idCaa = enum_CAA.Coldiretti
                    End If
                    If idCaa IsNot Nothing Then
                        Dim indexCaa = 0
                        Dim ChiamaWS As Integer = False
                        Dim listaCaaProvati As New List(Of enum_CAA)
                        While (indexCaa < 4)
                            Dim richiestaEffettuata = reader.richiestaGiaEffettuata(Cuaa, anno, idCaa, Date.Now, 500, ObjParametri_Server)
                            If richiestaEffettuata Then
                                listaCaaProvati.Add(idCaa)
                                nuovoIDCaa(idCaa)
                            Else
                                ChiamaWS = True
                                Exit While
                            End If
                            indexCaa += 1
                        End While
                        If ChiamaWS And listaCaaProvati.Count <= 4 Then
                            Dim myISWSResponse As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse
                            ImportaAGREA_WS(idCaa, Cuaa, anno, myISWSResponse, ObjParametri_Server, ErrMsg)
                            If myISWSResponse IsNot Nothing AndAlso myISWSResponse.codRet = "012" Then

                                Dim stream As New StringBuilder
                                Dim sWriter = New StringWriter(stream)
                                Dim x As New Xml.Serialization.XmlSerializer(myISWSResponse.GetType)
                                Dim xml As String
                                x.Serialize(sWriter, myISWSResponse)
                                xml = stream.ToString

                                Dim datavalidazioneArr = myISWSResponse.domanda.dataValidazione.Split("/")
                                Dim domandaStr = myISWSResponse.domanda.idDomanda & "_" & datavalidazioneArr(2) & datavalidazioneArr(1) & datavalidazioneArr(0)

                                writer.AggiornaInCacheFascicolo(20, Cuaa, domandaStr, myISWSResponse.domanda.dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, CStr(anno))
                                writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)

                                If myISWSResponse.domanda IsNot Nothing Then
                                    writer.InserisciAggiornaFascicoloDaImportare_AGREA(ObjParametri_Server, Cuaa, 0, anno, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, detentoreAGEA)
                                End If

                            Else
                                errori += 1
                                logga("ImportAgrea1   Anno:" & CStr(anno) & " Errore nel Cuaa:" + Cuaa + " - " + ErrMsg + " [" + CStr(i) + "]")
                                writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                                returnBool = False
                            End If

                        End If

                    Else
                        writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                        writer.EliminaFascicoloDaCaricare(ObjParametri_Server, Cuaa, 20)
                    End If


                Catch ex As Exception
                    errori += 1
                    logga("ImportAgrea2   Anno:" & CStr(anno) & " Errore nel Cuaa:" + CStr(Cuaa) + " - " + CStr(anno) + " - " + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") + " [" + CStr(i) + "]")
                    returnBool = False
                    writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                End Try
                Dim dt = reader.leggiImportaFascicoli(Now.Date, 2, ObjParametri_Server)
                If dt.Rows.Count > 0 Then
                    If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                        FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                    End If
                    If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                        errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                    End If
                End If
                writer.ScriviImportaFascicoli(Now.Date, 2, FascicoliInseriti, errori, ObjParametri_Server)
                i = i + 1
            Next
        Next
        Return returnBool
    End Function

    Public Function importaAgrea2() As Boolean
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W

        AziendeModificate()

        Dim dtCuaa = reader.LeggiAggiornaFascicoli(ObjParametri_Server, 2, , , , 0, )

        For Each rowCuaa As DataRow In dtCuaa.Rows

            Dim ErrMsg As String = ""

            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Dim ErrCOD As Integer
            Dim Cuaa As String = rowCuaa("CUAA")
            Dim Anno As Integer = CInt(rowCuaa("Parametri_Extra"))
            Dim idCaa As Integer = CInt(rowCuaa("Utenza"))
            Try


                Dim myISWSResponse As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse
                ImportaAGREA_WS(idCaa, Cuaa, Anno, myISWSResponse, ObjParametri_Server, ErrMsg)
                'Importazione Alfanumerico
                If myISWSResponse IsNot Nothing AndAlso myISWSResponse.codRet = "012" AndAlso Not myISWSResponse.msgRet.Contains("Contattare il fornitore del servizio") Then
                    Dim stream As New StringBuilder
                    Dim sWriter = New StringWriter(stream)
                    Dim x As New Xml.Serialization.XmlSerializer(myISWSResponse.GetType)
                    Dim xml As String
                    x.Serialize(sWriter, myISWSResponse)
                    xml = stream.ToString

                    Dim datavalidazioneArr = myISWSResponse.domanda.dataValidazione.Split("/")
                    Dim domandaStr = myISWSResponse.domanda.idDomanda & "_" & datavalidazioneArr(2) & datavalidazioneArr(1) & datavalidazioneArr(0)

                    writer.AggiornaInCacheFascicolo(20, Cuaa, domandaStr, myISWSResponse.domanda.dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, CStr(Anno))
                    writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)

                    If myISWSResponse.domanda IsNot Nothing Then
                        writer.InserisciAggiornaFascicoloDaImportare_AGREA(ObjParametri_Server, Cuaa, 0, Anno, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If

                Else
                    errori += 1
                    logga("ImportAgrea1   Anno:" & CStr(Anno) & " Errore nel Cuaa:" + Cuaa + " - " + ErrMsg + " ")
                    writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                    returnBool = False
                End If


                'Importazione Grafico
                Dim myISWSResponseGrafico As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponseGrafico
                ImportaAGREA_WS_Grafico(idCaa, Cuaa, Anno, myISWSResponseGrafico, ObjParametri_Server, ErrMsg)
                If myISWSResponseGrafico IsNot Nothing AndAlso myISWSResponseGrafico.codRet = "012" AndAlso Not myISWSResponse.msgRet.Contains("Contattare il fornitore del servizio") Then

                    Dim stream As New StringBuilder
                    Dim sWriter = New StringWriter(stream)
                    Dim x As New Xml.Serialization.XmlSerializer(myISWSResponseGrafico.GetType)
                    Dim xml As String
                    x.Serialize(sWriter, myISWSResponseGrafico)
                    xml = stream.ToString

                    Dim datavalidazioneArr = myISWSResponseGrafico.domanda.dataValidazione.Split("/")
                    Dim domandaStr = myISWSResponseGrafico.domanda.idDomanda & "_" & datavalidazioneArr(2) & datavalidazioneArr(1) & datavalidazioneArr(0)

                    writer.AggiornaInCacheFascicolo(21, Cuaa, domandaStr, myISWSResponseGrafico.domanda.dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server, CStr(Anno))
                    writer.FascicoloImportato(Cuaa, 1, ObjParametri_Server)

                    If myISWSResponseGrafico.domanda IsNot Nothing Then
                        writer.InserisciAggiornaFascicoloDaImportare_AGREA(ObjParametri_Server, Cuaa, 0, Anno, CInt(myISWSResponseGrafico.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If

                Else
                    errori += 1
                    logga("ImportAgrea1   Anno:" & CStr(Anno) & " Errore nel Cuaa:" + Cuaa + " - " + ErrMsg + " ")
                    writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
                    returnBool = False
                End If

                writer.UpdateAggiornaFascicoli(ObjParametri_Server, 2, Cuaa, 1, CStr(Anno))

            Catch ex As Exception
                errori += 1
                logga("ImportAgrea2   Anno:" & CStr(Anno) & " Errore nel Cuaa:" + CStr(Cuaa) + " - " + CStr(Anno) + " - " + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") + "")
                returnBool = False
                writer.FascicoloImportato(Cuaa, -1, ObjParametri_Server)
            End Try
            Dim dt = reader.leggiImportaFascicoli(Now.Date, 2, ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                    FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                End If
                If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                    errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                End If
            End If
            writer.ScriviImportaFascicoli(Now.Date, 2, FascicoliInseriti, errori, ObjParametri_Server)
        Next
        Return returnBool
    End Function

    Public Function AziendeModificate() As Boolean
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim conf_WS As New AgronicaCoreAgroFascicoloDAL.Configurazioni_WS_R

        Dim dtUtenze = conf_WS.Leggi_Configurazioni_WS(2, "", "", "", "", "", "", ObjParametri_Server)
        For Each utenzaRow In dtUtenze.Rows

            For Each anno In anniDaImportare

                Dim idCaa As enum_CAA
                Dim username = utenzaRow("UsernameWS")
                Dim Password = utenzaRow("PasswordWSCryptata")
                Select Case username
                    Case "ws_cia"
                        idCaa = enum_CAA.CIA
                    Case "ws_coldiretti"
                        idCaa = enum_CAA.Coldiretti
                    Case "ws_confagri"
                        idCaa = enum_CAA.Confagricoltura
                    Case "ws_legacoop"
                        idCaa = enum_CAA.LegaCoop
                End Select

                Dim dataRiferimento = getDataModifica(2, anno, idCaa)

                If dataRiferimento IsNot Nothing Then
                    Dim cuaaVariati As New ISWSResponseCUAAVariati
                    ImportaAGREA_WS_CUAAMoficati(idCaa, dataRiferimento, anno, cuaaVariati, ObjParametri_Server, "")

                    Dim dataAttuale = New Date(Date.Now.Year, Date.Now.Month, Date.Now.Day)

                    If cuaaVariati IsNot Nothing AndAlso cuaaVariati.cuaa IsNot Nothing AndAlso cuaaVariati.cuaa.Count > 0 Then
                        For Each cuaa In cuaaVariati.cuaa
                            writer.ScriviAggiornaFascicoli(ObjParametri_Server, 2, dataRiferimento, dataAttuale, cuaa, 0, anno, idCaa)
                        Next
                    End If

                    writer.eliminaAggiornaFascicoli(ObjParametri_Server, 2, 1, dataRiferimento, anno, idCaa)

                End If

            Next

        Next
    End Function


    Private Function getDataModifica(ByRef enteValidatore_cod As Integer, Anno As String, Utenza As Integer?) As DateTime?
        Dim reader As New Fascicolo_R
        Dim dataModifica As Date
        Dim dt As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod,,,,, Anno, Utenza)
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, , , , 0, Anno, Utenza)
            If dt1.Rows.Count = 0 Then
                dataModifica = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, Anno, Utenza).Rows(0)(0))
                If dataModifica = Now.Date Then
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            dataModifica = New Date(2016, 12, 1)
        End If
        Return dataModifica
    End Function

    Public Function AggiornaFascicoliDaImportare()

        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        writer.resettaCuaaDaImportare(ObjParametri_Server, 0, 2)
        For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 10000, 0, 2).Rows
            Dim cuaa = r.Item("cuaa")
            Dim xml As String
            Dim myISWSResponse As New Sincro_Agrea2Gias.MyWsAgrea.ISWSResponse

            logga("Aggiorno CUAA:" & cuaa)
            Try
                '2015
                xml = reader.GetSingoloFascicoloAgea_CACHE(2, cuaa, "", "", "2015", ObjParametri_Server)
                If xml <> "" Then
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xml, myISWSResponse, , False)
                    If myISWSResponse.domanda IsNot Nothing Then
                        writer.InserisciFascicoloDaImportare_AGREA(ObjParametri_Server, cuaa, 0, 2015, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If
                End If
            Catch ex As Exception

            End Try

            Try
                '2016
                xml = reader.GetSingoloFascicoloAgea_CACHE(2, cuaa, "", "", "2016", ObjParametri_Server)
                If xml <> "" Then
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xml, myISWSResponse, , False)
                    If myISWSResponse.domanda IsNot Nothing Then
                        writer.InserisciFascicoloDaImportare_AGREA(ObjParametri_Server, cuaa, 0, 2016, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If
                End If
            Catch ex As Exception

            End Try


            Try
                '2017
                xml = reader.GetSingoloFascicoloAgea_CACHE(2, cuaa, "", "", "2017", ObjParametri_Server)
                If xml <> "" Then
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xml, myISWSResponse, , False)
                    If myISWSResponse.domanda IsNot Nothing Then
                        writer.InserisciFascicoloDaImportare_AGREA(ObjParametri_Server, cuaa, 0, 2017, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If
                End If
            Catch ex As Exception

            End Try


            Try
                '2018
                xml = reader.GetSingoloFascicoloAgea_CACHE(2, cuaa, "", "", "2018", ObjParametri_Server)
                If xml <> "" Then
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xml, myISWSResponse, , False)
                    If myISWSResponse.domanda IsNot Nothing Then
                        writer.InserisciFascicoloDaImportare_AGREA(ObjParametri_Server, cuaa, 0, 2018, CInt(myISWSResponse.domanda.idCaa), myISWSResponse.domanda.descCaa, "")
                    End If
                End If
            Catch ex As Exception

            End Try




        Next



    End Function

    Private Sub CaricaDati_WS(ByRef myISWSResponse As Sincro_Agrea2Gias.MyWsAgrea.ISWSResponse,
                              ByRef ErrCOD As Integer,
                             ByRef ErrMsg As String,
                           ByVal CUAA As String,
                           ByVal Anno As Integer)

        Const NomeFunzione As String = "CaricaDati_WS."

        'Dim strRet As String = String.Empty
        Dim DLL_SincroAgrea As New Sincro_Agrea2Gias.RecuperaDati


        DLL_SincroAgrea.RecuperaDati_Azienda(CUAA,
                                           Anno,
                                           True, True, True,
                                           myISWSResponse,
                                           linkWS_Agrea,
                                           username_Agrea,
                                           password_Agrea,
                                           ErrCOD, ErrMsg)


    End Sub

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

    Public Shared Function getCaa(Cuaa As String,
                                  anno As Integer,
                                  objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  linkAGEA As String,
                                  userAGEA As String,
                                  passwordAGEA As String,
                                  ByRef Caa_ID As String,
                                  ByRef Caa_Des As String,
                                  ByRef detentoreAGEA As String) As enum_CAA?
        Caa_ID = 0
        Caa_Des = ""
        detentoreAGEA = ""
        Dim caa_Agrea = getCaaTabellaAGREA(Cuaa, anno, objParametri_Server, Caa_ID, Caa_Des)
        Dim caa_Agea_RT = getCaaTabellaAGEA_COORDINAMENTO(Cuaa, objParametri_Server, linkAGEA, userAGEA, passwordAGEA, detentoreAGEA)
        If Not caa_Agrea Is Nothing AndAlso Not caa_Agea_RT Is Nothing Then

            If caa_Agea_RT = caa_Agrea Then
                Return caa_Agrea
            Else
                Return caa_Agrea
            End If

        ElseIf caa_Agrea Is Nothing AndAlso Not caa_Agea_RT Is Nothing Then
            Return caa_Agea_RT
        ElseIf Not caa_Agrea Is Nothing AndAlso caa_Agea_RT Is Nothing Then
            Return caa_Agrea
        ElseIf caa_Agrea Is Nothing AndAlso caa_Agea_RT Is Nothing Then
            Return Nothing
        End If

    End Function

    Public Shared Function getCaaTabellaAGREA(Cuaa As String,
                                              anno As Integer,
                                              objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByRef Caa_ID As String,
                                              ByRef Caa_Des As String) As enum_CAA?
        Try
            Dim caa As enum_CAA
            Dim reader As New Fascicolo_R
            Dim i = 0
            Dim DT = reader.Leggi_FascicoliDaImportare_AGREA(Cuaa, Nothing, anno, 0, objParametri_Server)
            If DT.Rows.Count > 0 Then
                Caa_ID = DT.Rows(0)("ID_CAA")
                Caa_Des = DT.Rows(0)("CAA_Des")
                If DT.Rows(0)("CAA_Des").ToString.Contains(" COLDIRETTI ") Then
                    Return enum_CAA.Coldiretti
                ElseIf DT.Rows(0)("CAA_Des").ToString.Contains(" CIA ") Then
                    Return enum_CAA.CIA
                ElseIf DT.Rows(0)("CAA_Des").ToString.Contains(" CONFAGRICOLTURA ") Then
                    Return enum_CAA.Confagricoltura
                ElseIf DT.Rows(0)("CAA_Des").ToString.Contains(" LEGACOOP ") Then
                    Return enum_CAA.LegaCoop
                End If
            End If
            Return Nothing
        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Public Shared Function getCaaTabellaAGEA_RT(Cuaa As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, link As String, username As String, password As String) As enum_CAA?

        Dim importatoreAgea As New Import_WS_Agea
        Dim ISWSRespAnagFascicolo15 As New List(Of Importazione_Agea_BIZ.ISWSRespAnagFascicolo15)
        importatoreAgea.Importa_Fascicolo(Cuaa, 0, ISWSRespAnagFascicolo15, Nothing, Nothing, Nothing, Nothing, objParametri_Server, link, username, password, True)
        If ISWSRespAnagFascicolo15.Count > 0 Then
            Dim detentoreCompleto = ISWSRespAnagFascicolo15(0).detentore
            Dim detentoreCAA = detentoreCompleto.Substring(0, 3)
            Select Case detentoreCAA
                Case "103", "020"
                    'COLDIRETTI
                    Return enum_CAA.Coldiretti
                Case "105"
                    'CONFAGRICOLTURA
                    Return enum_CAA.Confagricoltura
                Case "107", "010"
                    'CIA
                    Return enum_CAA.CIA
            End Select


        End If
        Return Nothing

    End Function

    Public Shared Function getCaaTabellaAGEA_COORDINAMENTO(Cuaa As String,
                                                           objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           link As String,
                                                           username As String,
                                                           password As String,
                                                           ByRef detentoreAGEA As String) As enum_CAA?
        Try
            Dim importatoreAgea As New Import_AGEAFS5(link, username, password)
            Dim fascicolo50 As New AGEA_Coordinamento.ISWSToOprResponse
            importatoreAgea.TrovaFascicoloFS50(Cuaa, fascicolo50)
            If fascicolo50 IsNot Nothing AndAlso (fascicolo50.Items(0) IsNot Nothing) Then
                If fascicolo50.Items(0).ToString <> "" Then
                    Dim fascicolo2 = DirectCast(fascicolo50.Items(0), AGEA_Coordinamento.ISWSRespAnagFascicolo15)
                    If fascicolo2.organismoPagatore = "IT01" Or fascicolo2.organismoPagatore = "IT08" Then
                        If fascicolo2.detentore IsNot Nothing Then
                            Dim detentoreCAA = fascicolo2.detentore.Substring(0, 3)
                            detentoreAGEA = fascicolo2.detentore
                            Select Case detentoreCAA
                                Case "103", "020"
                                    'COLDIRETTI
                                    Return enum_CAA.Coldiretti
                                Case "105"
                                    'CONFAGRICOLTURA
                                    Return enum_CAA.Confagricoltura
                                Case "107", "010"
                                    'CIA
                                    Return enum_CAA.CIA
                                Case Else
                                    Return Nothing
                            End Select
                        End If

                    End If
                End If
            End If
        Catch ex As Exception

        End Try
        Return Nothing
    End Function

    Public Shared Function ImportaAGREA_WS(idCaa As enum_CAA, CUAA As String, anno As Integer, ByRef fascicolo As Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ErrMsg As String)
        Dim conf_WS As New AgronicaCoreAgroFascicoloDAL.Configurazioni_WS_R
        Dim Errori_utenze_r As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_R
        Dim Errori_utenze_w As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_W
        Dim writer As New Fascicolo_W
        Dim link = ""
        Dim password = ""
        Dim username = ""
        Select Case idCaa
            Case enum_CAA.CIA
                username = "ws_cia"
            Case enum_CAA.Coldiretti
                username = "ws_coldiretti"
            Case enum_CAA.Confagricoltura
                username = "ws_confagri"
            Case enum_CAA.LegaCoop
                username = "ws_legacoop"
        End Select

        Dim dt_Conf = conf_WS.Leggi_Configurazioni_WS(2, "", username, "", "", "", "", objParametri_Server)
        If dt_Conf.Rows.Count > 0 Then
            If Not IsDBNull(dt_Conf.Rows(0)("PasswordWSCryptata")) AndAlso dt_Conf.Rows(0)("PasswordWSCryptata") <> "" Then
                password = dt_Conf.Rows(0)("PasswordWSCryptata")
            Else
                Dim pwdCriptata As String
                pwdCriptata = FormsAuthentication.HashPasswordForStoringInConfigFile(dt_Conf.Rows(0)("PasswordWS"), "SHA1")
                pwdCriptata = pwdCriptata.ToLower
                password = pwdCriptata
            End If
            link = dt_Conf.Rows(0)("LinkWS")

        End If

        If dt_Conf.Rows.Count > 0 Then

            Dim errori As Integer = Errori_utenze_r.Leggi_Errori_Utenze(20, username, Date.Now, "", "", "", objParametri_Server)

            Dim ErrCOD As Integer = 0

            If errori < 45 Then

                ErrMsg = ""
                Try

                    Dim a = New Importazione_Agrea_WS.Importa_AGREA_WS(username, password, link)
                    a.importa(CUAA, anno, fascicolo, ErrCOD, ErrMsg)
                Catch ex As Exception
                    ErrCOD = -101
                    ErrMsg = ErrMsg
                End Try

                If ErrCOD = -101 Then

                    Errori_utenze_w.incrementa_errori(20, username, Date.Now, "", objParametri_Server)
                    'ErrMsg = "Richiesta con utenza " + username + " non andata a buon fine."
                    ErrMsg = "Richiesta non andata a buon fine."
                    writer.InserisciErroreAGREA(CUAA, anno, idCaa, Date.Now, objParametri_Server)

                ElseIf ErrCOD = -102 Then

                    writer.EliminaFascicoloDaCaricare(objParametri_Server, CUAA, 20)
                    ErrMsg = "CUAA inesistente"

                End If

            Else
                fascicolo = Nothing
                ErrCOD = -102
                'ErrMsg = "Numero di richieste errate da utenza:" + username + " superate, non è stato possibile proseguire con la chiamata a ws."
                ErrMsg = "Non è stato possibile proseguire con la chiamata a ws."
            End If

        End If


    End Function

    Public Shared Function ImportaAGREA_WS_CUAAMoficati(idCaa As enum_CAA, Data As Date, anno As Integer, ByRef fascicolo As Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponseCUAAVariati, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ErrMsg As String)
        Dim conf_WS As New AgronicaCoreAgroFascicoloDAL.Configurazioni_WS_R
        Dim Errori_utenze_r As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_R
        Dim Errori_utenze_w As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_W
        Dim writer As New Fascicolo_W
        Dim link = ""
        Dim password = ""
        Dim username = ""
        Select Case idCaa
            Case enum_CAA.CIA
                username = "ws_cia"
            Case enum_CAA.Coldiretti
                username = "ws_coldiretti"
            Case enum_CAA.Confagricoltura
                username = "ws_confagri"
            Case enum_CAA.LegaCoop
                username = "ws_legacoop"
        End Select

        Dim dt_Conf = conf_WS.Leggi_Configurazioni_WS(2, "", username, "", "", "", "", objParametri_Server)
        If dt_Conf.Rows.Count > 0 Then
            If Not IsDBNull(dt_Conf.Rows(0)("PasswordWSCryptata")) AndAlso dt_Conf.Rows(0)("PasswordWSCryptata") <> "" Then
                password = dt_Conf.Rows(0)("PasswordWSCryptata")
            Else
                Dim pwdCriptata As String
                pwdCriptata = FormsAuthentication.HashPasswordForStoringInConfigFile(dt_Conf.Rows(0)("PasswordWS"), "SHA1")
                pwdCriptata = pwdCriptata.ToLower
                password = pwdCriptata
            End If
            link = dt_Conf.Rows(0)("LinkWS")

        End If

        If dt_Conf.Rows.Count > 0 Then

            Dim errori As Integer = Errori_utenze_r.Leggi_Errori_Utenze(20, username, Date.Now, "", "", "", objParametri_Server)

            Dim ErrCOD As Integer = 0

            If errori < 45 Then

                ErrMsg = ""
                Try

                    Dim a = New Importazione_Agrea_WS.Importa_AGREA_WS(username, password, link)
                    a.CuaaModificati(Data, anno, fascicolo, ErrCOD, ErrMsg)
                Catch ex As Exception
                    ErrCOD = -101
                    ErrMsg = ErrMsg
                End Try

            Else
                fascicolo = Nothing
                ErrCOD = -102
                'ErrMsg = "Numero di richieste errate da utenza:" + username + " superate, non è stato possibile proseguire con la chiamata a ws."
                ErrMsg = "Non è stato possibile proseguire con la chiamata a ws."
            End If

        End If


    End Function

    Public Shared Function ImportaAGREA_WS_Grafico(idCaa As enum_CAA, CUAA As String, anno As Integer, ByRef fascicolo As Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponseGrafico, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef ErrMsg As String)
        Dim conf_WS As New AgronicaCoreAgroFascicoloDAL.Configurazioni_WS_R
        Dim Errori_utenze_r As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_R
        Dim Errori_utenze_w As New AgronicaCoreAgroFascicoloDAL.Errori_Utenze_W
        Dim writer As New Fascicolo_W
        Dim link = ""
        Dim password = ""
        Dim username = ""
        Select Case idCaa
            Case enum_CAA.CIA
                username = "ws_cia"
            Case enum_CAA.Coldiretti
                username = "ws_coldiretti"
            Case enum_CAA.Confagricoltura
                username = "ws_confagri"
            Case enum_CAA.LegaCoop
                username = "ws_legacoop"
        End Select

        Dim dt_Conf = conf_WS.Leggi_Configurazioni_WS(2, "", username, "", "", "", "", objParametri_Server)
        If dt_Conf.Rows.Count > 0 Then
            If Not IsDBNull(dt_Conf.Rows(0)("PasswordWSCryptata")) AndAlso dt_Conf.Rows(0)("PasswordWSCryptata") <> "" Then
                password = dt_Conf.Rows(0)("PasswordWSCryptata")
            Else
                Dim pwdCriptata As String
                pwdCriptata = FormsAuthentication.HashPasswordForStoringInConfigFile(dt_Conf.Rows(0)("PasswordWS"), "SHA1")
                pwdCriptata = pwdCriptata.ToLower
                password = pwdCriptata
            End If
            link = dt_Conf.Rows(0)("LinkWS")

        End If

        If dt_Conf.Rows.Count > 0 Then

            Dim errori As Integer = Errori_utenze_r.Leggi_Errori_Utenze(20, username, Date.Now, "", "", "", objParametri_Server)

            Dim ErrCOD As Integer = 0

            If errori < 45 Then

                ErrMsg = ""
                Try

                    Dim a = New Importazione_Agrea_WS.Importa_AGREA_WS(username, password, link)
                    a.importaGrafico(CUAA, anno, fascicolo, ErrCOD, ErrMsg)
                Catch ex As Exception
                    ErrCOD = -101
                    ErrMsg = ErrMsg
                End Try

                If ErrCOD = -101 Then

                    Errori_utenze_w.incrementa_errori(20, username, Date.Now, "", objParametri_Server)
                    'ErrMsg = "Richiesta con utenza " + username + " non andata a buon fine."
                    ErrMsg = "Richiesta non andata a buon fine."
                    writer.InserisciErroreAGREA(CUAA, anno, idCaa, Date.Now, objParametri_Server)

                ElseIf ErrCOD = -102 Then

                    writer.EliminaFascicoloDaCaricare(objParametri_Server, CUAA, 20)
                    ErrMsg = "CUAA inesistente"

                End If

            Else
                fascicolo = Nothing
                ErrCOD = -102
                'ErrMsg = "Numero di richieste errate da utenza:" + username + " superate, non è stato possibile proseguire con la chiamata a ws."
                ErrMsg = "Non è stato possibile proseguire con la chiamata a ws."
            End If

        End If


    End Function

    Public Shared Function nuovoIDCaa(ByRef idCaa As enum_CAA)

        Select Case idCaa
            Case enum_CAA.Coldiretti
                idCaa = enum_CAA.Confagricoltura
            Case enum_CAA.Confagricoltura
                idCaa = enum_CAA.CIA
            Case enum_CAA.CIA
                idCaa = enum_CAA.LegaCoop
            Case enum_CAA.LegaCoop
                idCaa = enum_CAA.Coldiretti
        End Select

    End Function

End Class
