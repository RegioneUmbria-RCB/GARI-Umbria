Imports System.Text
Imports System.IO
Imports System.Security.Cryptography
Imports AgronicaCoreDataProvider

Public Class ImportatoreGIAS_FascicoliAnagrafe
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

    Private linkWS_Anagrafe As String
    Private username_Anagrafe As String
    Private password_Anagrafe As String
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
                     linkWS_Anagrafe As String,
                     username_Anagrafe As String,
                     password_Anagrafe As String,
                     limiteGiornaliero As Integer,
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
        Me.linkWS_Anagrafe = linkWS_Anagrafe
        Me.username_Anagrafe = username_Anagrafe
        Me.password_Anagrafe = password_Anagrafe
        Me.limiteGiornaliero = limiteGiornaliero
        Me.anniDaImportare = anniDaImportare
    End Sub

    Public Function AziendeModificate() As Boolean
        Dim getAziendeModificate As New Sincro_Agrea2Gias.MyWsAgriRER.AziendeModificateResponse
        Dim objAgreaDLL As New Sincro_Agrea2Gias.RecuperaDati
        Dim writer As New Fascicolo_W
        Dim result = SHA1HashStringForUTF8String(password_Anagrafe)
        result = result.ToUpper
        Dim ErrCOD As Integer
        Dim ErrMsg As String = ""
        Dim dataModifica As DateTime? = getDataModifica(1)
        If dataModifica IsNot Nothing Then
            objAgreaDLL.RecuperaDati_AziendeModificate("ACCESSO_FASCICOLO_CAA",
                                   CStr(dataModifica),
                                   getAziendeModificate,
                                   linkWS_Anagrafe,
                                   username_Anagrafe,
                                   result,
                                   ErrCOD, ErrMsg)


            If getAziendeModificate IsNot Nothing Then
                If getAziendeModificate.listaAziende IsNot Nothing Then
                    If getAziendeModificate.listaAziende.cuaa IsNot Nothing Then
                        logga(CStr(getAziendeModificate.listaAziende.cuaa.Length) + " Aziende modificate dal " + CStr(dataModifica))
                        For Each cuaa In getAziendeModificate.listaAziende.cuaa
                            writer.ScriviAggiornaFascicoli(ObjParametri_Server, 1, dataModifica, Now.Date, cuaa, 0)
                        Next
                    End If
                End If
            End If
        Else

        End If
        Return False
    End Function

    Private Function getDataModifica(ByRef enteValidatore_cod As Integer) As DateTime?
        Dim reader As New Fascicolo_R
        Dim dataModifica As Date
        Dim dt As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod)
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, , , , 0)
            If dt1.Rows.Count = 0 Then
                dataModifica = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod).Rows(0)(0))
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

    Public Function importaAnagrafe() As Boolean
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Cancello record in AggiornaFascicoli
        Dim dataMax = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, 1).Rows(0)(0))
        writer.eliminaAggiornaFascicoli(ObjParametri_Server, 1, 1, dataMax)

        'Leggo quanti fascicoli ho già scaricato oggi
        Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
        Dim fascicoliGiaInseriti As Integer = 0
        If dt1.Rows.Count > 0 Then
            If Not (IsDBNull(dt1.Rows(0).Item("numeroFascicoliCaricati"))) Then
                fascicoliGiaInseriti = CInt(dt1.Rows(0).Item("numeroFascicoliCaricati"))
            End If
        End If
        Dim myFascicoloSiarResponse As New Sincro_Agrea2Gias.MyWsAgriRER.FascicoloSiar2Response
        Dim objAgreaDLL As New Sincro_Agrea2Gias.RecuperaDati

        Dim result = SHA1HashStringForUTF8String(password_Anagrafe)
        result = result.ToUpper

        If limiteGiornaliero - fascicoliGiaInseriti > 0 Then
            For Each Cuaa As String In reader.getFascicolidaCaricare(limiteGiornaliero - fascicoliGiaInseriti, ObjParametri_Server)
                Dim FascicoliInseriti As Integer = 0
                Dim errori As Integer = 0
                Dim ErrCOD As Integer
                Dim ErrMsg As String = ""
                Try
                    Dim dt2 = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
                    fascicoliGiaInseriti = 0
                    If dt2.Rows.Count > 0 Then
                        If Not (IsDBNull(dt2.Rows(0).Item("numeroFascicoliCaricati"))) Then
                            fascicoliGiaInseriti = CInt(dt2.Rows(0).Item("numeroFascicoliCaricati"))
                        End If
                    End If
                    If fascicoliGiaInseriti >= limiteGiornaliero Then
                        Exit For
                    End If

                    'Scarico Fascicolo
                    objAgreaDLL.RecuperaDati_Fascicolo("ACCESSO_FASCICOLO_CAA",
                                       Cuaa,
                                       myFascicoloSiarResponse,
                                       linkWS_Anagrafe,
                                       username_Anagrafe,
                                       result,
                                       ErrCOD, ErrMsg)
                    'Dim AgroFascicolo As New AgronicaCoreAgroFascicoloBIZ.AgroFascicolo
                    'Dim xmlF = AgroFascicolo.ScaricaAgroFascicolo("03487210407", "", "", 1, Cuaa, "", "", True, ObjParametri_Server, 0, "")
                    'AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xmlF, myFascicoloSiarResponse, , False)


                    Select Case ErrCOD
                        Case 0
                            Dim stream As New StringBuilder
                            Dim sWriter = New StringWriter(stream)
                            Dim x As New Xml.Serialization.XmlSerializer(myFascicoloSiarResponse.GetType)
                            Dim xml As String
                            x.Serialize(sWriter, myFascicoloSiarResponse)
                            xml = stream.ToString
                            'Trovo la data di validazione e/o il numero di validazione
                            Dim dataValidazione As DateTime
                            If myFascicoloSiarResponse.messaggioRisposta.cod = "000" Then
                                Dim dataValiazioneStr As String = myFascicoloSiarResponse.statoAzienda.dataValidazione
                                If myFascicoloSiarResponse.statoAzienda.dataValidazione Is Nothing Then
                                    dataValiazioneStr = myFascicoloSiarResponse.datiAnagrafici.dtValidazione
                                End If
                                dataValidazione = CDate(dataValiazioneStr)
                            Else
                                dataValidazione = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
                            End If
                            'Scriv/aggiorno record in FascicoliCache
                            writer.AggiornaInCacheFascicolo(1, Cuaa, 1, dataValidazione, xml, Now.Date, New Date(2100, 12, 31), ObjParametri_Server)
                            'Metto flag importato
                            writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, 1)
                            writer.InserisciFascicoloDaCaricare(ObjParametri_Server, Cuaa, 2, 0)
                            FascicoliInseriti += 1

                            Dim Fascicolo = reader.legggiAziendaFascicolo(1, Cuaa, "1", ObjParametri_Server, dataValidazione)

                            Try
                                Dim xml1 As String = reader.leggiFascicolo(1, CStr(Fascicolo.Item("CUAA")), ObjParametri_Server)
                                AgronicaCoreUtility.XMLUtility.getObjectFromResponse(xml1, myFascicoloSiarResponse, , False)
                                Dim OprFascicoloAGEA = New ISWSToOprResponse
                                Dim converter As New ConvertFascicoloSiarToFascicoloAGEA
                                converter.ConvertFascicoloSiarToFascicoloAGEA(myFascicoloSiarResponse, OprFascicoloAGEA)
                                Dim strXml = AgronicaCoreUtility.XMLUtility.getStringFromObject(OprFascicoloAGEA)
                                writer.scriviFascicoliFormatoComune(CInt(Fascicolo.Item("EnteValidatore_cod")),
                                                                    CStr(Fascicolo.Item("CUAA")),
                                                                    CStr(Fascicolo.Item("Validazione_Numero")),
                                                                    DateTime.Parse(CStr(Fascicolo.Item("Validazione_Data"))),
                                                                    strXml,
                                                                    1,
                                                                    ObjParametri_Server)
                            Catch ex As Exception
                                writer.scriviErrore("__T_FascicoliDaCaricare",
                                                    CStr(Fascicolo.Item("CUAA")),
                                                    -1,
                                                    ObjParametri_Server)
                                logga("ERRORE:" + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                                returnBool = False
                            End Try
                        Case -201
                            errori += 1
                            logga("ImportaAnagrafe1   Errore nel Cuaa:" + Cuaa + " - " + ErrMsg)
                            returnBool = False
                    End Select
                Catch ex As Exception
                    errori += 1
                    logga("ImportaAnagrafe2   Errore nel Cuaa:" + Cuaa + " - " + ErrMsg + " - " + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                    writer.scriviErrore("__T_FascicoliDaCaricare",
                                                    CStr(Cuaa),
                                                    -1,
                                                    ObjParametri_Server)
                    returnBool = False
                End Try
                Dim dt = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
                If dt.Rows.Count > 0 Then
                    If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                        FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                    End If
                    If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                        errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                    End If
                End If
                writer.ScriviImportaFascicoli(Now.Date, 1, FascicoliInseriti, errori, ObjParametri_Server)
            Next
        End If
        Return returnBool
    End Function

    Private Function HexStringFromBytes(hashBytes As Byte()) As String
        Dim sb = New StringBuilder()

        For Each b As Byte In hashBytes
            Dim Hex = b.ToString("x2")
            sb.Append(Hex)
        Next
        Return sb.ToString()
    End Function

    Private Function SHA1HashStringForUTF8String(ByVal s As String) As String
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(s)
        Dim sha = SHA1.Create()

        Dim hashBytes As Byte() = sha.ComputeHash(bytes)


        Return HexStringFromBytes(hashBytes)
    End Function

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

End Class
