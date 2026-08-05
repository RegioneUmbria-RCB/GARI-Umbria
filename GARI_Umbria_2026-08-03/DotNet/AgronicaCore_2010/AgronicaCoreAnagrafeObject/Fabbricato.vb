Imports System.Runtime.Serialization
Imports System.Xml
Imports AgronicaCoreXML
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


<DataContract()>
Public Class Fabbricato
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Partita_iva() As String

    <DataMember()>
    Public Property Sa_Cod() As Integer

    <DataMember()>
    Public Property Codice_Fabbricato() As Integer

    <DataMember()>
    Public Property Fabbricato_Denominazione() As String

    <DataMember()>
    Public Property Tipo_Fabbricato_Codice() As Integer

    <DataMember()>
    Public Property Volume_Convenzionale() As String

    <DataMember()>
    Public Property Volume_Conversione() As String

    <DataMember()>
    Public Property Volume_Biologico() As String

    <DataMember()>
    Public Property Titolo_Possesso() As Integer

    <DataMember()>
    Public Property Codice_Regolamento() As String

    <DataMember()>
    Public Property Codice_Particella() As String

    <DataMember()>
    Public Property Validita_Inizio() As Date

    <DataMember()>
    Public Property Validita_Fine() As Date

    <DataMember()>
    Public Property Indirizzo() As Indirizzo




    Public Sub New(ByVal Partita_iva As String, ByVal _Indirizzo As Indirizzo)
        Indirizzo = _Indirizzo
        Tipo_Fabbricato_Codice = enum_FabbricatiTipi.MagazzinoAziendale
        Titolo_Possesso = TITOLOPOSSESSO_PROPRIETA
        Fabbricato_Denominazione = "Magazzino"
        Validita_Inizio = AGRODATAINIZIO
        Validita_Fine = AGRODATAFINE
    End Sub


    Private Sub ControllaPreSalvataggio()

    End Sub
    Public Function ScriviFabbricato(ByVal BaseCode As Integer, ByVal TopCode As Integer, _
                                  ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

        ControllaPreSalvataggio()

        Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
        Dim XmlDoc As New XmlDocument
        Dim log As String

        'SCRITTURA BIZ
        Dim XMLFAbbricato As XmlElement = AnagrafeXML.XML_2_Fabbricati(log, _
                                                                            XmlDoc, _
                                                                            BaseCode, _
                                                                            TopCode, _
                                                                            enum_TipoOperazioneDB.Scrittura, _
                                                                            Partita_iva, _
                                                                            Sa_Cod, _
                                                                            0, _
                                                                            Fabbricato_Denominazione, _
                                                                            Tipo_Fabbricato_Codice, _
                                                                            Indirizzo.Tipo_Indirizzo, _
                                                                            Indirizzo.Codice_istat_Provincia, _
                                                                            Indirizzo.Codice_istat_Comune, _
                                                                            0, _
                                                                            Indirizzo.Via, _
                                                                            "", _
                                                                            Indirizzo.Cap, _
                                                                            Indirizzo.Stato, _
                                                                            "", _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            Nothing, _
                                                                            , , , , , , , , , , _
                                                                            Titolo_Possesso, _
                                                                            , , , , , , , , , , , , , , _
                                                                            Validita_Inizio, _
                                                                            Validita_Fine, _
                                                                            , , , , , , , )


        Dim Fabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        Dim fabbricato_cod As Integer
        Fabbricato_W.Fabbricato_Scrivi(XMLFAbbricato.OuterXml, Partita_iva, Sa_Cod, fabbricato_cod, objParametriServer)

    End Function


End Class
