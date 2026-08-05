Imports System.IO
Imports System.Text
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports XmlDiffLib

Public Class ImportatoreGIAS_FascicoliArtea
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
        Me.limiteGiornaliero = limiteGiornaliero
        Me.anniDaImportare = anniDaImportare
    End Sub

    Public Function importaArtea() As Boolean
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim dt1 = reader.leggiImportaFascicoli(Now.Date, 3, ObjParametri_Server)
        Dim fascicoliGiaInseriti As Integer = 0
        If dt1.Rows.Count > 0 Then
            If Not (IsDBNull(dt1.Rows(0).Item("numeroFascicoliCaricati"))) Then
                fascicoliGiaInseriti = CInt(dt1.Rows(0).Item("numeroFascicoliCaricati"))
            End If
        Else
            writer.resettaCuaaDaImportare(ObjParametri_Server, 0, 3)
        End If
        Dim objAgreaDLL As New Sincro_Agrea2Gias.RecuperaDati
        Dim i = 0
        Dim listaCuaa As New List(Of String)
        For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 10000, 0, 3).Rows
            listaCuaa.Add(r.Item("cuaa"))
        Next
        If listaCuaa.Count = 0 Then
            For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, -1, 3).Rows
                listaCuaa.Add(r.Item("cuaa"))
            Next
            If listaCuaa.Count = 0 Then
                writer.resettaCuaaDaImportare(ObjParametri_Server, 1, 3)
                For Each r As DataRow In reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 10000, 0, 3).Rows
                    listaCuaa.Add(r.Item("cuaa"))
                Next
            End If
        End If
        For Each Cuaa As String In listaCuaa
            Dim ErrMsg As String = ""
            Try
                aggiornaParametriExtra(Cuaa, "GetBeneficiario")
            Catch ex As Exception
                logga("ImportArtea   GetBeneficiario Errore nel Cuaa:" + CStr(Cuaa) + " - " + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") + " [" + CStr(i) + "]")
                returnBool = False
            End Try

            For Each anno In anniDaImportare
                Dim FascicoliInseriti As Integer = 0
                Dim errori As Integer = 0
                Dim ErrCOD As Integer
                Try

                    aggiornaParametriExtra(Cuaa, CStr(anno))

                Catch ex As Exception
                    errori += 1
                    logga("ImportArtea   Anno:" & CStr(anno) & " Errore nel Cuaa:" + CStr(Cuaa) + " - " + CStr(anno) + " - " + AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True") + " [" + CStr(i) + "]")
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

    Public Sub aggiornaParametriExtra(cuaa As String, Parametri_Extra As String)

        Dim fascicoloR As New Fascicolo_R
        Dim fascicoloW As New Fascicolo_W

        Dim DT = fascicoloR.GetSingoloFascicoloAgea_CACHE_DT(3, cuaa, "", " Validazione_Data DESC ", Parametri_Extra, ObjParametri_Server)

        Dim inserisciFascicolo As Boolean = False
        Dim Validazione_Numero As String = ""
        Dim Validazione_Data As Date
        Dim xmlFascicolo As String = ""
        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            Validazione_Data = CDate(DT.Rows(0).Item("Validazione_Data"))
            Validazione_Numero = CStr(DT.Rows(0).Item("Validazione_Numero"))
            Dim xml1 As String = fascicoloR.GetSingoloFascicoloAgea_CACHE(3, cuaa, Validazione_Numero, "", Parametri_Extra, ObjParametri_Server, Validazione_Data)

            Dim agroFascicolo As New AgronicaCoreAgroFascicoloBIZ.AgroFascicolo
            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS", "", "", ObjParametri_Server)
            xmlFascicolo = agroFascicolo.ScaricaAgroFascicolo("03487210407", "", "", 3, cuaa, "", Parametri_Extra, False, ObjParametri_Server, 0, "", url)


            'Dim doc2 = loadXMLconEncoding(xml1, Encoding.UTF8)
            Dim doc1 = XDocument.Parse(xml1)
            Dim doc2 = XDocument.Parse(xmlFascicolo)
            'Dim doc1 = loadXMLconEncoding(xmlFascicolo, Encoding.UTF32)
            doc1.Descendants().Attributes("id").Remove
            doc2.Descendants().Attributes("id").Remove

            Dim doc1str As String = stringaDaXmlDocument(doc1)
            Dim doc2str As String = stringaDaXmlDocument(doc2)

            Dim doc1s = XDocument.Parse(doc1str)
            Dim doc2s = XDocument.Parse(doc2str)

            'Dim xmldiff = New XmlDiff(doc1str, doc2str)
            'Dim xmlOpt As New XmlDiffOptions

            'xmlOpt.IgnoreAttributeOrder = True
            'xmlOpt.IgnoreChildOrder = True
            'xmlOpt.IgnoreNamespace = True
            'xmlOpt.IgnorePrefix = True
            'xmlOpt.IgnoreAttributes = True
            ''xmlOpt.IgnoreCase = True
            ''xmlOpt.IgnoreNodes = True
            ''xmlOpt.IgnoreTextTypes = True
            ''xmlOpt.TrimWhitespace = True
            ''xmlOpt.TwoWayMatch = True
            'Dim bDiff = xmldiff.CompareDocuments(xmlOpt)
            Dim bDiff = XNode.DeepEquals(doc1s, doc2s)
            If Not bDiff Then
                inserisciFascicolo = True
            End If

        Else
            inserisciFascicolo = True
        End If

        If inserisciFascicolo Then
            If xmlFascicolo = "" Then

                Dim agroFascicolo As New AgronicaCoreAgroFascicoloBIZ.AgroFascicolo
                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS", "", "", ObjParametri_Server)
                xmlFascicolo = agroFascicolo.ScaricaAgroFascicolo("03487210407", "", "", 3, cuaa, "", Parametri_Extra, False, ObjParametri_Server, 0, "", url)

            End If

            If xmlFascicolo IsNot Nothing AndAlso xmlFascicolo <> "" Then

                Validazione_Data = Now.Date
                Validazione_Numero = "PCG ARTEA" + Parametri_Extra + "-" & Validazione_Data.Year & Validazione_Data.Month.ToString("00") + Validazione_Data.Day.ToString("00")
                fascicoloW.AggiornaInCacheFascicolo(3, cuaa, Validazione_Numero, Validazione_Data, xmlFascicolo, AGRODATAINIZIO, AGRODATAFINE, ObjParametri_Server, Parametri_Extra)

            End If

        End If

    End Sub

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

    Public Function stringaDaXmlDocument(doc As XDocument) As String
        Dim res As String = ""
        Try

            Dim stringWriter As New StringWriter
            Dim xmlTextWriter = XmlWriter.Create(stringWriter)

            doc.WriteTo(xmlTextWriter)
            xmlTextWriter.Flush()
            res = stringWriter.GetStringBuilder.ToString

            Return res

        Catch ex As Exception

            res = ""
            Return res

        End Try

    End Function

    Public Function loadXMLconEncoding(xmlString As String, encoding As Encoding) As XmlDocument

        Dim doc As XmlDocument = New XmlDocument()

        Dim encodedString = encoding.GetBytes(xmlString)

        Dim ms As New MemoryStream(encodedString)
        ms.Flush()
        ms.Position = 0

        doc.Load(ms)

        Return doc


    End Function

End Class
