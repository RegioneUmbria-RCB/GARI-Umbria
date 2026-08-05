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
Public Class Impresa
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
    Public Property Ragione_Sociale() As String

    <DataMember()>
    Public Property Codice_Cuaa() As String

    <DataMember()>
    Public Property Codice_Fiscale() As String

    <DataMember()>
    Public Property Codice_Cliente() As String

    <DataMember()>
    Public Property Cf_Tecnico_Referente() As String

    <DataMember()>
    Public Property Sup_Totale() As Decimal

    <DataMember()>
    Public Property Partita_Iva_Padre() As String

    <DataMember()>
    Public Property Tipo_Impresa_Gerarchia() As Integer

    <DataMember()>
    Public Property Titolo_Possesso() As Integer

    <DataMember()>
    Public Property ID_Cod_Cliente() As Integer


    <DataMember()>
    Public Property Validita_inizio() As Date?

    <DataMember()>
    Public Property Validita_Fine() As Date?

    <DataMember()>
    Public Property Indirizzo() As List(Of Indirizzo)

    <DataMember()>
    Public Property Legale_Rappresentante() As Contatto

    <DataMember()>
    Public Property Centri_Aziendali() As List(Of Centro_Aziendale)


    Public Sub New(ByVal _Partita_Iva_Impresa_Padre As String, _
                       ByVal _Partita_Iva As String, _
                       ByVal _Ragione_Sociale As String, _
                       ByVal _ID_Cod_Cliente As Integer, _
                       ByVal _Indirizzo As List(Of Indirizzo)
                       )
        Partita_Iva_Padre = _Partita_Iva_Impresa_Padre
        Partita_Iva = _Partita_Iva
        Ragione_Sociale = _Ragione_Sociale
        ID_Cod_Cliente = _ID_Cod_Cliente

        Indirizzo = _Indirizzo

        Validita_inizio = AGRODATAINIZIO
        Validita_Fine = AGRODATAFINE
    End Sub

    Private Sub ControllaPreSalvataggio(ByVal CentroAziendaleAutomatico As Boolean, ByVal FabbricatoAutomatico As Boolean)

        If IsNothing(Validita_inizio) Then
            Validita_inizio = AGRODATAINIZIO
        End If
        If IsNothing(Validita_Fine) Then
            Validita_Fine = AGRODATAFINE
        End If

        If IsNothing(Codice_Cuaa) Then
            Codice_Cuaa = Partita_Iva
        End If
        Dim Indirizzo1 As Indirizzo = Indirizzo(0)
        If CentroAziendaleAutomatico = True Then
            'se ho il centro aziendale automatico
            'controllo se l'ho già creato
            Dim inizializza As Boolean = False
            If IsNothing(Me.Centri_Aziendali) Then
                inizializza = True
            Else
                If Me.Centri_Aziendali.Count = 0 Then
                    inizializza = True
                    
                End If
            End If
            If inizializza = True Then
         
                Centri_Aziendali = New List(Of Centro_Aziendale)
                'devo creare prima il centro aziendale con sede legale
                For i = 0 To Indirizzo.Count - 1
                    If Indirizzo(i).Flag_Fornitore_Fatturazione = True Then
                        Dim nuovoFabbricato As List(Of Fabbricato)
                        If FabbricatoAutomatico = True Then
                            nuovoFabbricato = New List(Of Fabbricato)
                            nuovoFabbricato.Add(New Fabbricato(Partita_Iva, Indirizzo1))
                        End If

                        If IsNothing(Me.Codice_Cliente) Then
                            Codice_Cliente = Indirizzo(i).Codice_Navgreen
                        End If

                        If IsNothing(Indirizzo(i).Note_Indirizzo) Then
                            Indirizzo(i).Note_Indirizzo = ""
                        End If
                        'creo il centro di fatturazione
                        Dim c As New Centro_Aziendale(Me.Partita_Iva, _
                                                      Indirizzo(i).Codice_Navgreen + " - " + Indirizzo(i).Note_Indirizzo, _
                                                      enum_TipoCentro.Sede_Legale, _
                                                      Indirizzo(i).Codice_Navgreen, _
                                                      Indirizzo(i), _
                                                      nuovoFabbricato)
                        Centri_Aziendali.Add(c)
                    End If
                Next

              

                'creo gli altri centri
                For i = 0 To Indirizzo.Count - 1
                    If Indirizzo(i).Flag_Fornitore_Fatturazione = False Then
                        If IsNothing(Indirizzo(i).Note_Indirizzo) Then
                            Indirizzo(i).Note_Indirizzo = ""
                        End If
                        'creo il centro di fatturazione
                        Dim c As New Centro_Aziendale(Me.Partita_Iva, _
                                                      Indirizzo(i).Codice_Navgreen + " - " + Indirizzo(i).Note_Indirizzo, _
                                                      enum_TipoCentro.Sede_Aziendale, _
                                                      Indirizzo(i).Codice_Navgreen, _
                                                      Indirizzo(i), _
                                                      Nothing)
                        Centri_Aziendali.Add(c)
                    End If
                Next

            End If
        End If


    End Sub



    Public Function ScriviImpresa(ByRef rispSTR As String, ByVal BaseCode As Integer, ByVal TopCode As Integer, ByVal Id_Cod_Cliente As Integer, _
                                  ByVal CentroAziendaleAutomatico As Boolean, ByVal FabbricatoAutomatico As Boolean, ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

        'controllo se è già presente nel db
        If ControllaSeEsisteImpresa(objParametriServer) = True Then
            rispSTR = "impresa già presente in archivio, non è stato possibile inserirla."
            Return False
        End If

        rispSTR = ""
        Dim xRisp As String
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        ControllaPreSalvataggio(CentroAziendaleAutomatico, FabbricatoAutomatico)

        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametriServer)

             

            Dim AnagrafeXML As New AgronicaCoreXML.XML_Anagrafe
            Dim XmlDoc As New XmlDocument
            Dim log As String

            'SCRITTURA BIZ
            Dim XML_Utility As New AgronicaCoreXML.XML_Utility
            Dim Dt_Codici As DataTable
            Dt_Codici = XML_Utility.CaricaGriglia_CodiciImpresa_for_XML()
            XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                                Me.Tipo_Operazione, _
                                                                Partita_Iva, _
                                                                enum_CodiciAnagrafe.CodiceCUAA, _
                                                                Me.Codice_Cuaa, _
                                                                , )

            XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                                Me.Tipo_Operazione, _
                                                               Partita_Iva, _ 
                                                                enum_CodiciAnagrafe.CodiceProduttore, _
                                                                Me.Codice_Cliente, _
                                                                , )

            XML_Utility.Inserisci_Riga_Dt_CodiciImpresa_for_XML(Dt_Codici, _
                                                             Me.Tipo_Operazione, _
                                                             Partita_Iva, _
                                                             Id_Cod_Cliente, _
                                                             Me.Codice_Cliente, _
                                                             , )



            Dim DT_RisUm As DataTable
            DT_RisUm = XML_Utility.CaricaGriglia_RisUm_for_XML()
            XML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, _
                                                 Me.Tipo_Operazione, _
                                                 objParametriServer.PivaSuperUser, _
                                                 Me.Partita_Iva, _
                                                 , _
                                                 , _
                                                 COD_FORNITORE, _
                                                 , _
                                                 , _
                                                  , , , , , , , , , , _
                                                  , )


            'identifico il centro che ha il flag fornitore attivo
            Dim ind_Fatt As Indirizzo
            For i = 0 To Me.Indirizzo.Count - 1
                If Indirizzo(i).Flag_Fornitore_Fatturazione = True Then
                    ind_Fatt = Indirizzo(i)
                    Indirizzo.RemoveAt(i)
                    Exit For
                End If
            Next
            If IsNothing(ind_Fatt) Then
                ind_Fatt = Indirizzo(0)
            End If


            Dim XmlDatiImprese As XmlElement = AnagrafeXML.XML_2_Imprese(log, _
                                                                         XmlDoc, _
                                                                         BaseCode, _
                                                                         TopCode, _
                                                                         Me.Tipo_Operazione, _
                                                                         Me.Tipo_Operazione, _
                                                                         Me.Partita_Iva, _
                                                                         Me.Ragione_Sociale, _
                                                                         Me.Partita_Iva_Padre, _
                                                                         objParametriServer.PivaSuperUser, _
                                                                         Me.Tipo_Impresa_Gerarchia, _
                                                                              ind_Fatt.Tipo_Indirizzo, _
                                                                              ind_Fatt.Codice_istat_Provincia, _
                                                                              ind_Fatt.Codice_istat_Comune, _
                                                                              ind_Fatt.Cod_Indirizzo, _
                                                                              ind_Fatt.Via, _
                                                                              ind_Fatt.Frazione, _
                                                                              ind_Fatt.Cap, _
                                                                              ind_Fatt.Stato, _
                                                                              ind_Fatt.Note_Indirizzo, _
                                                                         Dt_Codici, _
                                                                         DT_RisUm, _
                                                                         Nothing, _
                                                                           , , , , , , _
                                                                         , _
                                                                         , _
                                                                         , _
                                                                         Me.Codice_Fiscale, _
                                                                         , _
                                                                         , _
                                                                         Me.Validita_inizio, _
                                                                         Me.Validita_Fine)


            Dim Impresa_W As New AgronicaCoreAnagrafeBIZ.Impresa_W
            Dim OUTPUT_Piva As String
            Impresa_W.Impresa_Scrivi(XmlDatiImprese.OuterXml, OUTPUT_Piva, objParametriServer, objParametriUtenti)

            If OUTPUT_Piva <> Me.Partita_Iva Then
                Throw New Exception
            End If

            Dim Imprese_Codici_Write As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write
            Imprese_Codici_Write.Scrivi(Me.Partita_Iva, Id_Cod_Cliente, Me.Codice_Cliente, Me.Validita_inizio, Me.Validita_Fine, objParametriServer)


            'scrivo il centro
            If Me.Centri_Aziendali.Count > 0 Then
                Dim i As Integer
                For i = 0 To Centri_Aziendali.Count - 1
                    Centri_Aziendali(i).ScriviCentroAziendale(BaseCode, TopCode, Id_Cod_Cliente, objParametriServer, objParametriUtenti)
                Next
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)
            xRisp = True


        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametriServer.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
            rispSTR = ex.Message
            Throw New Exception(ex.Message)
            xRisp = False
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametriServer)
        End Try
        Return xRisp
    End Function



    Public Function ModificaImpresa(ByRef rispSTR As String, _
                                    ByVal BaseCode As Integer, ByVal TopCode As Integer, _
                                    ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal Id_Cod_Cliente As Integer, _
                                    ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Me.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

        'controllo se è già presente nel db
        If ControllaSeEsisteImpresa(objParametriServer) = False Then
            rispSTR = "l'impresa non è presente nel gias"
            Return False
        End If

        rispSTR = ""
        Dim xRisp As String
        Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean

        ControllaPreSalvataggio(True, True)
        Try

            '------------------------------
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametriServer)


            Dim Imprese_Write As New AgronicaCoreAnagrafeDAL.Imprese_Write
            Dim indirizzi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
            Dim ImpresexIndirizzi_W As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_W
            Dim Imprese_Codici_Write As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write
            Dim centri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Write
            Dim gerarchiaImprese As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_W
            Dim UtentixImprese_Write As New AgronicaCoreAnagrafeDAL.UtentixImprese_Write

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_W

            'modifico i dati dell'impresa
            Imprese_Write.Modifica(Me.Partita_Iva, Me.Ragione_Sociale, "", "", "", "",
                        Me.Sup_Totale, _
                        0, _
                     "", AGRODATAINIZIO, AGRODATAFINE, "", objParametriServer)


            objContatti.Modifica_Parametrizzata(objParametriServer.PivaSuperUser, _
                                                 Me.Partita_Iva, _
                                                 "codice_Fiscale", Me.Codice_Fiscale, _
                                                 AGRODATAINIZIO, AGRODATAFINE, _
                                                 "", objParametriServer)

            Imprese_Codici_Write.Modifica(Me.Partita_Iva, enum_CodiciAnagrafe.CodiceCUAA, _
                                           Me.Codice_Cuaa, AGRODATAINIZIO, AGRODATAFINE, "", objParametriServer)


            'modifico la gerarchia se necessario
            Dim objgerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim dt_gerarchia As DataTable = objgerarchia.LeggixFiglio(Me.Partita_Iva, _
                                                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                      "", "", _
                                                                      objParametriServer)

            Dim boolean_modifica_gerarchia As Boolean = False
            If dt_gerarchia.Rows.Count > 0 Then
                If dt_gerarchia(0).Item("padre") <> Me.Partita_Iva_Padre Then
                    boolean_modifica_gerarchia = True
                End If
            Else
                boolean_modifica_gerarchia = True
            End If

            If boolean_modifica_gerarchia = True Then
                Dim objModificaGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_W
                objModificaGerarchia.Cancella(dt_gerarchia(0).Item("Padre"), Me.Partita_Iva, "", objParametriServer, objParametriUtenti)
                objModificaGerarchia.Scrivi(Me.Partita_Iva_Padre, Me.Partita_Iva, AGRODATAINIZIO, AGRODATAFINE, objParametriServer, objParametriUtenti)
            End If


            'identifico il codice dell'indirizzo
            Dim objIndirizzoLeggi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
            Dim dt_indirizzo As DataTable = objIndirizzoLeggi.Leggi(Me.Partita_Iva, 0, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
            Dim cod_indirizzo As Integer = dt_indirizzo.Rows(0).Item("Cod_Indirizzo")

            Dim objModificaIndirizzo As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
            objModificaIndirizzo.Modifica(cod_indirizzo, Me.Indirizzo(0).Via, Me.Indirizzo(0).Frazione, Me.Indirizzo(0).Cap, Me.Indirizzo(0).Comune, Me.Indirizzo(0).Provincia, Me.Indirizzo(0).Stato, _
                                           Me.Indirizzo(0).Note_Indirizzo, Me.Indirizzo(0).Codice_istat_Provincia, Me.Indirizzo(0).Codice_istat_Comune, Me.Validita_inizio, Me.Validita_Fine, "", objParametriServer)





            'parte di modifica per i centri aziendali
            'leggo i Centri_Aziendali_Codici
            Dim objCentriCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
            Dim dt As DataTable = objCentriCodici.Leggi(Partita_Iva, 0, Id_Cod_Cliente, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer)
            Dim i As Integer
            Dim j As Integer
            For j = 0 To Centri_Aziendali.Count - 1
                Centri_Aziendali(i).Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            Next
            For i = 0 To dt.Rows.Count - 1
                For j = 0 To Centri_Aziendali.Count - 1
                    If Centri_Aziendali(j).Chiave_Cliente = dt.Rows(i).Item("val_cod") Then
                        Centri_Aziendali(j).Sa_Cod = dt.Rows(i).Item("Sa_Cod")
                        Centri_Aziendali(j).Tipo_Operazione = enum_TipoOperazioneDB.Modifica
                    End If
                Next
            Next

            If Me.Centri_Aziendali.Count > 0 Then
                For i = 0 To Centri_Aziendali.Count - 1
                    If Centri_Aziendali(i).Sa_Cod <> 0 Then
                        Centri_Aziendali(i).ModificaCentroAziendale(BaseCode, TopCode, Id_Cod_Cliente, objParametriServer, objParametriUtenti)
                    Else
                        Centri_Aziendali(i).ScriviCentroAziendale(BaseCode, TopCode, Id_Cod_Cliente, objParametriServer, objParametriUtenti)
                    End If
                Next
            End If



            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametriServer)
            xRisp = True


        Catch ex As Exception
            'Faccio il rollback della transazione
            If Not objParametriServer.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
            rispSTR = ex.Message
            xRisp = False
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametriServer)
        End Try
        Return xRisp
    End Function


    Public Function ControllaSeEsisteImpresa(ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim dt As DataTable
        dt = objImpre.Leggi(Me.Partita_Iva, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
        If dt.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If
    End Function




End Class
