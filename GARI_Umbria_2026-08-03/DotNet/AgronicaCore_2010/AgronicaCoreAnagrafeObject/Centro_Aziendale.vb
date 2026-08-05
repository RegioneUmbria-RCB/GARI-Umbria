Imports System.Runtime.Serialization
Imports System.Xml
Imports AgronicaCoreXML
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringValue() As String
'End Class



<DataContract()>
Public Class Centro_Aziendale
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Partita_Iva() As String

    <DataMember()>
    Public Property Sa_Cod() As Integer

    <DataMember()>
    Public Property Codice_Centro() As String

    <DataMember()>
    Public Property Nome_Centro() As String

    <DataMember()>
    Public Property Titolo_Possesso() As Integer

    <DataMember()>
    Public Property Tipo_Centro() As Integer

    <DataMember()>
    Public Property Sup_Bosco() As Decimal

    <DataMember()>
    Public Property Sup_Prati() As Decimal

    <DataMember()>
    Public Property Tipo_Attivita() As Integer

    <DataMember()>
    Public Property Validita_Inizio() As Date?

    <DataMember()>
    Public Property Validita_Fine() As Date?


    <DataMember()>
    Public Property Indirizzo() As Indirizzo

    <DataMember()>
    Public Property Rubrica_1() As String

    <DataMember()>
    Public Property Rubrica_2() As String

    <DataMember()>
    Public Property Rubrica_3() As String

    <DataMember()>
    Public Property Rubrica_4() As String

    <DataMember()>
    Public Property Rubrica_5() As String

    <DataMember()>
    Public Property Rubrica_6() As String

    <DataMember()>
    Public Property Rubrica_7() As String

    <DataMember()>
    Public Property Rubrica_8() As String

    <DataMember()>
    Public Property Rubrica_9() As String

    <DataMember()>
    Public Property Chiave_Cliente() As String

    <DataMember()>
    Public Property Fabbricato() As List(Of Fabbricato)



    Public Sub New(ByVal _Partita_Iva As String, ByVal _Nome_Centro As String, _
                   ByVal _Tipo_Centro As Integer, _
                   ByVal _Chiave_Cliente As String, _
                       ByVal _Indirizzo As Indirizzo, ByVal _Fabbricato As List(Of Fabbricato))
        Partita_Iva = _Partita_Iva
        Nome_Centro = _Nome_Centro
        Indirizzo = _Indirizzo
        Titolo_Possesso = TITOLOPOSSESSO_PROPRIETA
        Fabbricato = _Fabbricato
        Tipo_Centro = _Tipo_Centro
        Chiave_Cliente = _Chiave_Cliente
        Validita_Inizio = AGRODATAINIZIO
        Validita_Fine = AGRODATAFINE
    End Sub
    Public Sub New()
        
    End Sub

    Private Sub ControllaPreSalvataggio()
        If IsNothing(Codice_Centro) Then
            Codice_Centro = Indirizzo.Codice_Navgreen
        End If
    End Sub
    Public Function ScriviCentroAziendale(ByVal BaseCode As Integer, ByVal TopCode As Integer, _
                                          ByVal Id_Cod_Cliente As Integer, _
                                  ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

        ControllaPreSalvataggio()

        Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
        Dim XmlDoc As New XmlDocument
        Dim log As String

        'Codice
        Dim XML_Utility As New AgronicaCoreXML.XML_Utility
        Dim Dt_Codici As DataTable
        Dt_Codici = XML_Utility.CaricaGriglia_CodiciCentro_for_XML()
        XML_Utility.Inserisci_Riga_Dt_CodiciCentro_for_XML(Dt_Codici, _
                                                            Me.Tipo_Operazione, _
                                                            Partita_Iva, _
                                                            0, _
                                                            Id_Cod_Cliente, _
                                                            Codice_Centro)

        XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                            Me.Tipo_Operazione, _
                                                            Partita_Iva, _
                                                            enum_CodiciAnagrafe.TitoloPossesso, _
                                                            enum_TitoloPossesso.Proprieta)


        XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                            Me.Tipo_Operazione, _
                                                            Partita_Iva, _
                                                            Tipo_Centro, _
                                                            Tipo_Centro)


        XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                            Me.Tipo_Operazione, _
                                                            Partita_Iva, _
                                                            enum_CodiciAnagrafe.Codice_Centro, _
                                                            Codice_Centro)

        'SCRITTURA BIZ
        Dim Xml_CentroAziendale As XmlElement = AnagrafeXML.XML_2_CentriAziendali(log, _
                                                                                XmlDoc, _
                                                                                BaseCode, _
                                                                                TopCode, _
                                                                                Me.Tipo_Operazione, _
                                                                                Me.Partita_Iva, _
                                                                                Me.Sa_Cod, _
                                                                                Me.Nome_Centro, _
                                                                                Me.Indirizzo.Tipo_Indirizzo, _
                                                                                Me.Indirizzo.Codice_istat_Provincia, _
                                                                                Me.Indirizzo.Codice_istat_Comune, _
                                                                                0, _
                                                                                Me.Indirizzo.Via, _
                                                                                Me.Indirizzo.Frazione, _
                                                                                Me.Indirizzo.Cap, _
                                                                                Me.Indirizzo.Stato, _
                                                                                Me.Indirizzo.Note_Indirizzo, _
                                                                                Dt_Codici, _
                                                                                Nothing, _
                                                                                , , , , , , , , , _
                                                                                Me.Titolo_Possesso, _
                                                                                Me.Tipo_Centro, _
                                                                                , , , , , , , , _
                                                                                Validita_Inizio, _
                                                                                Validita_Fine)


        Dim CentroAziendale_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
        Dim OUTPUT_Piva As String
        CentroAziendale_W.CentroAziendale_Scrivi(Xml_CentroAziendale.OuterXml, Partita_Iva, Sa_Cod, objParametriServer, objParametriUtenti)

        'scrivo il fabbricato
        If Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
            If Not IsNothing(Fabbricato) Then
                If Fabbricato.Count > 0 Then
                    Dim i As Integer
                    For i = 0 To Fabbricato.Count - 1
                        Fabbricato(i).Partita_iva = Partita_Iva
                        Fabbricato(i).Sa_Cod = Sa_Cod
                        Fabbricato(i).ScriviFabbricato(BaseCode, TopCode, objParametriServer, objParametriUtenti)
                    Next
                End If
            End If
        End If

    End Function

    Public Function ModificaCentroAziendale(ByVal BaseCode As Integer, ByVal TopCode As Integer, _
                                         ByVal Id_Cod_Cliente As Integer, _
                                 ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        ControllaPreSalvataggio()

        Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
        Dim XmlDoc As New XmlDocument
        Dim log As String

        'modifico sa_nome
        Dim objCent As New AgronicaCoreAnagrafeDAL.CentriAziendali_Write
        Dim sa_nome As String = Me.Nome_Centro
        objCent.Modifica_sa_nome(Partita_Iva, Sa_Cod, sa_nome, "", objParametriServer)



        'leggo l'indirizzo
        Dim dt As DataTable
        Dim objind As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        dt = objind.Leggi(Partita_Iva, Sa_Cod, 0, 0, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "", "", objParametriServer)

        Dim objIndW As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
        objIndW.Modifica(dt.Rows(0).Item("cod_indirizzo"), Indirizzo.Via, _
                         Indirizzo.Frazione, Indirizzo.Cap, Indirizzo.Comune, Indirizzo.Provincia, _
                         Indirizzo.Stato, Indirizzo.Note_Indirizzo, Indirizzo.Codice_istat_Provincia, _
                         Indirizzo.Codice_istat_Comune, AGRODATAINIZIO, AGRODATAFINE, "", objParametriServer)

    End Function


    Public Sub Leggi(ByVal partitaIva As String, ByVal Sa_Cod As Integer, _
                                ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Partita_Iva = partitaIva
        Me.Sa_Cod = Sa_Cod
        Dim dt As DataTable
        Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        dt = objCentro.Leggi(Partita_Iva, Sa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

        Me.Nome_Centro = dt.Rows(0).Item("sa_nome")

        Dim codici_centro As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim dt_codici As DataTable = codici_centro.Leggi(Partita_Iva, Sa_Cod, enum_CodiciAnagrafe.Codice_Centro, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
        If dt_codici.Rows.Count > 0 Then
            Me.Codice_Centro = dt_codici.Rows(0).Item("val_cod")
        End If

        Dim dt_indirizzi As DataTable
        Dim objind As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        dt_indirizzi = objind.Leggi(Partita_Iva, Sa_Cod, 0, 0, _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                            "", "", objParametriServer)
        Dim i As Integer


        For i = 0 To dt_indirizzi.Rows.Count - 1
            Me.Indirizzo = New AgronicaCoreAnagrafeObject.Indirizzo(False, "", _
                                                                dt_indirizzi.Rows(i).Item("ind_des"), _
                                                                dt_indirizzi.Rows(i).Item("CAP"), _
                                                                dt_indirizzi.Rows(i).Item("com_des"), _
                                                                dt_indirizzi.Rows(i).Item("pro_des"), _
                                                                dt_indirizzi.Rows(i).Item("pro_cod"), _
                                                                dt_indirizzi.Rows(i).Item("com_cod_istat"), _
                                                                dt_indirizzi.Rows(i).Item("stato"))

        Next




    End Sub

End Class
