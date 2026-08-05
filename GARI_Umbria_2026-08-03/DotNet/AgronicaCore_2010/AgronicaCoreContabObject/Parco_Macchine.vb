Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Reflection


Public Class Parco_Macchine

#Region "Campi chiave"
    Private _Piva As String
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Private _Mac_Cod As Integer
    Public Property Mac_Cod() As Integer
        Get
            Return _Mac_Cod
        End Get
        Set(ByVal value As Integer)
            _Mac_Cod = value
        End Set
    End Property

    Private _Sa_Cod As Integer
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property
#End Region


#Region "proprieta generiche"
    Private _Descrizione_per_Agenda As String
    Public Property Descrizione_per_Agenda() As String
        Get
            Return _Descrizione_per_Agenda
        End Get
        Set(ByVal value As String)
            _Descrizione_per_Agenda = value
        End Set
    End Property

    Private _Mac_Cod_Origine As Integer
    Public Property Mac_Cod_Origine() As Integer
        Get
            Return _Mac_Cod_Origine
        End Get
        Set(ByVal value As Integer)
            _Mac_Cod_Origine = value
        End Set
    End Property

    Private _Piva_superUser_Origine As String
    Public Property Piva_superUser_Origine() As String
        Get
            Return _Piva_superUser_Origine
        End Get
        Set(ByVal value As String)
            _Piva_superUser_Origine = value
        End Set
    End Property

    Private _GiacenzaIniziale As Boolean
    Public Property GiacenzaIniziale() As Boolean
        Get
            Return _GiacenzaIniziale
        End Get
        Set(ByVal value As Boolean)
            _GiacenzaIniziale = value
        End Set
    End Property

    Private _Finalita As Integer
    Public Property Finalita() As Integer
        Get
            Return _Finalita
        End Get
        Set(ByVal value As Integer)
            _Finalita = value
        End Set
    End Property
    Private _Tipo As Value_Text
    Public Property Tipo() As Value_Text
        Get
            Return _Tipo
        End Get
        Set(ByVal value As Value_Text)
            _Tipo = value
        End Set
    End Property
    Private _Marca As Integer
    Public Property Marca() As Integer
        Get
            Return _Marca
        End Get
        Set(ByVal value As Integer)
            _Marca = value
        End Set
    End Property
    Private _Dettaglio_1 As Value_Text
    Public Property Dettaglio_1() As Value_Text
        Get
            Return _Dettaglio_1
        End Get
        Set(ByVal value As Value_Text)
            _Dettaglio_1 = value
        End Set
    End Property
    Private _Dettaglio_2 As Value_Text
    Public Property Dettaglio_2() As Value_Text
        Get
            Return _Dettaglio_2
        End Get
        Set(ByVal value As Value_Text)
            _Dettaglio_2 = value
        End Set
    End Property
    Private _Descrizione As String
    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As String)
            _Descrizione = value
        End Set
    End Property
    Private _Targa As String
    Public Property Targa() As String
        Get
            Return _Targa
        End Get
        Set(ByVal value As String)
            _Targa = value
        End Set
    End Property
    Private _Tipo_Targa As Integer
    Public Property Tipo_Targa() As Integer
        Get
            Return _Tipo_Targa
        End Get
        Set(ByVal value As Integer)
            _Tipo_Targa = value
        End Set
    End Property
    Private _Telaio As String
    Public Property Telaio() As String
        Get
            Return _Telaio
        End Get
        Set(ByVal value As String)
            _Telaio = value
        End Set
    End Property
    Private _Modello As String
    Public Property Modello() As String
        Get
            Return _Modello
        End Get
        Set(ByVal value As String)
            _Modello = value
        End Set
    End Property
    Private _Proprietario As String
    Public Property Proprietario() As String
        Get
            Return _Proprietario
        End Get
        Set(ByVal value As String)
            _Proprietario = value
        End Set
    End Property
    Private _Alimentazione As Integer
    Public Property Alimentazione() As Integer
        Get
            Return _Alimentazione
        End Get
        Set(ByVal value As Integer)
            _Alimentazione = value
        End Set
    End Property
    Private _Potenza As Decimal
    Public Property Potenza() As Decimal
        Get
            Return _Potenza
        End Get
        Set(ByVal value As Decimal)
            _Potenza = value
        End Set
    End Property

    Private _UDM_Potenza As Integer
    Public Property UDM_Potenza() As Integer
        Get
            Return _UDM_Potenza
        End Get
        Set(ByVal value As Integer)
            _UDM_Potenza = value
        End Set
    End Property
    Private _Taratura_Ugello As Decimal
    Public Property Taratura_Ugello() As Decimal
        Get
            Return _Taratura_Ugello
        End Get
        Set(ByVal value As Decimal)
            _Taratura_Ugello = value
        End Set
    End Property

    Private _Titolo_Possesso As Integer
    Public Property Titolo_Possesso() As Integer
        Get
            Return _Titolo_Possesso
        End Get
        Set(ByVal value As Integer)
            _Titolo_Possesso = value
        End Set
    End Property

    Private _CUAA_Proprietario As String
    Public Property CUAA_Proprietario() As String
        Get
            Return _CUAA_Proprietario
        End Get
        Set(ByVal value As String)
            _CUAA_Proprietario = value
        End Set
    End Property

    Private _Data_carico As String
    Public Property Data_carico() As String
        Get
            Return _Data_carico
        End Get
        Set(ByVal value As String)
            _Data_carico = value
        End Set
    End Property
    Private _Data_scarico As String
    Public Property Data_scarico() As String
        Get
            Return _Data_scarico
        End Get
        Set(ByVal value As String)
            _Data_scarico = value
        End Set
    End Property



    Private _Numero_Immatricolazione As String
    Public Property Numero_Immatricolazione() As String
        Get
            Return _Numero_Immatricolazione
        End Get
        Set(ByVal value As String)
            _Numero_Immatricolazione = value
        End Set
    End Property


    Private _Data_Immatricolazione As String
    Public Property Data_Immatricolazione() As String
        Get
            Return _Data_Immatricolazione
        End Get
        Set(ByVal value As String)
            _Data_Immatricolazione = value
        End Set
    End Property


    Private _Numero_Immatricolazione_Rimorchio As String
    Public Property Numero_Immatricolazione_Rimorchio() As String
        Get
            Return _Numero_Immatricolazione_Rimorchio
        End Get
        Set(ByVal value As String)
            _Numero_Immatricolazione_Rimorchio = value
        End Set
    End Property

    Private _Numero_Autorizzazione_Trasporto As String
    Public Property Numero_Autorizzazione_Trasporto() As String
        Get
            Return _Numero_Autorizzazione_Trasporto
        End Get
        Set(ByVal value As String)
            _Numero_Autorizzazione_Trasporto = value
        End Set
    End Property

    Private _Data_Rilascio_Autorizzazione As String
    Public Property Data_Rilascio_Autorizzazione() As String
        Get
            Return _Data_Rilascio_Autorizzazione
        End Get
        Set(ByVal value As String)
            _Data_Rilascio_Autorizzazione = value
        End Set
    End Property
    Private _Data_Inizio_Utilizzo As String
    Public Property Data_Inizio_Utilizzo() As String
        Get
            Return _Data_Inizio_Utilizzo
        End Get
        Set(ByVal value As String)
            _Data_Inizio_Utilizzo = value
        End Set
    End Property
    Private _Peso_Tara As Decimal
    Public Property Peso_Tara() As Decimal
        Get
            Return _Peso_Tara
        End Get
        Set(ByVal value As Decimal)
            _Peso_Tara = value
        End Set
    End Property
    Private _Macchina_Attiva As Boolean
    Public Property Macchina_Attiva() As Boolean
        Get
            Return _Macchina_Attiva
        End Get
        Set(ByVal value As Boolean)
            _Macchina_Attiva = value
        End Set
    End Property
    'Private _Macchina_Dismessa As Boolean
    'Public Property Macchina_Dismessa() As Boolean
    '    Get
    '        Return _Macchina_Dismessa
    '    End Get
    '    Set(ByVal value As Boolean)
    '        _Macchina_Dismessa = value
    '    End Set
    'End Property

    Private _Stato_Utilizzo As String
    Public Property Stato_Utilizzo() As String
        Get
            Return _Stato_Utilizzo
        End Get
        Set(ByVal value As String)
            _Stato_Utilizzo = value
        End Set
    End Property
    Private _Data_Dismissione As String
    Public Property Data_Dismissione() As String
        Get
            Return _Data_Dismissione
        End Get
        Set(ByVal value As String)
            _Data_Dismissione = value
        End Set
    End Property

    Private _Visibilita As String
    Public Property Visibilita() As String
        Get
            Return _Visibilita
        End Get
        Set(ByVal value As String)
            _Visibilita = value
        End Set
    End Property

    Private _Note As String
    Public Property Note() As String
        Get
            Return _Note
        End Get
        Set(ByVal value As String)
            _Note = value
        End Set
    End Property
#End Region


#Region "Revisione ammortamento"
    Private _Data_Ultima_Manutenzione As String
    Public Property Data_Ultima_Manutenzione() As String
        Get
            Return _Data_Ultima_Manutenzione
        End Get
        Set(ByVal value As String)
            _Data_Ultima_Manutenzione = value
        End Set
    End Property

    Private _Data_Ultima_Revisione As String
    Public Property Data_Ultima_Revisione() As String
        Get
            Return _Data_Ultima_Revisione
        End Get
        Set(ByVal value As String)
            _Data_Ultima_Revisione = value
        End Set
    End Property


    Private _Costo_Acquisto As Decimal
    Public Property Costo_Acquisto() As Decimal
        Get
            Return _Costo_Acquisto
        End Get
        Set(ByVal value As Decimal)
            _Costo_Acquisto = value
        End Set
    End Property


    Private _Costo_Manutenzione_Revisione As String
    Public Property Costo_Manutenzione_Revisione() As String
        Get
            Return _Costo_Manutenzione_Revisione
        End Get
        Set(ByVal value As String)
            _Costo_Manutenzione_Revisione = value
        End Set
    End Property

    Private _Ammortamento_Annuo_Percentuale As Decimal
    Public Property Ammortamento_Annuo_Percentuale() As Decimal
        Get
            Return _Ammortamento_Annuo_Percentuale
        End Get
        Set(ByVal value As Decimal)
            _Ammortamento_Annuo_Percentuale = value
        End Set
    End Property



    'Private _Data_Inizio_Utilizzo As String
    'Public Property Data_Inizio_Utilizzo() As String
    '    Get
    '        Return _Data_Inizio_Utilizzo
    '    End Get
    '    Set(ByVal value As String)
    '        _Data_Inizio_Utilizzo = value
    '    End Set
    'End Property




#End Region

    Private _Manutenzioni As List(Of Parco_Macchine_Manutenzione)
    Public Property Manutenzioni() As List(Of Parco_Macchine_Manutenzione)
        Get
            Return _Manutenzioni
        End Get
        Set(ByVal value As List(Of Parco_Macchine_Manutenzione))
            _Manutenzioni = value
        End Set
    End Property

    Private _Costi As List(Of Parco_Macchine_Costo)
    Public Property Costi() As List(Of Parco_Macchine_Costo)
        Get
            Return _Costi
        End Get
        Set(ByVal value As List(Of Parco_Macchine_Costo))
            _Costi = value
        End Set
    End Property


    Public Sub New(ByVal __Piva As String, ByVal __Mac_Cod As Integer)
        Macchina_Attiva = True
        'Macchina_Dismessa = False
        Piva = __Piva
        Mac_Cod = __Mac_Cod
        Tipo = New Value_Text
        Dettaglio_1 = New Value_Text
        Dettaglio_2 = New Value_Text

        Costi = New List(Of Parco_Macchine_Costo)
        Manutenzioni = New List(Of Parco_Macchine_Manutenzione)

        GiacenzaIniziale = True
    End Sub
    Public Sub New()
        Tipo = New Value_Text
        Dettaglio_1 = New Value_Text
        Dettaglio_2 = New Value_Text

        Macchina_Attiva = True
        'Macchina_Dismessa = False

        Costi = New List(Of Parco_Macchine_Costo)
        Manutenzioni = New List(Of Parco_Macchine_Manutenzione)

        GiacenzaIniziale = True
    End Sub

    Public Sub Leggi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim DT As DataTable
        Dim objP As New AgronicaCoreContabDAL.Parco_Macchine_R
        DT = objP.Leggi_daMacCod(Piva, Mac_Cod, "", "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count = 1 Then
            Piva_superUser_Origine = DT.Rows(0).Item("Piva_superUser_Origine")
            Mac_Cod_Origine = DBNullToNothing(DT.Rows(0).Item("Mac_Cod_Origine"))
            Descrizione_per_Agenda = ""
            Descrizione = DBNullToNothing(DT.Rows(0).Item("Mac_Des"))
            Costo_Acquisto = DBNullToNothing(DT.Rows(0).Item("Costo_Acquisto"))
            Targa = DBNullToNothing(DT.Rows(0).Item("Targa"))
            Telaio = DBNullToNothing(DT.Rows(0).Item("Telaio"))
            Marca = DBNullToNothing(DT.Rows(0).Item("Ditta_cod"))
            Modello = DBNullToNothing(DT.Rows(0).Item("Modello"))
            If Not IsDBNull(DT.Rows(0).Item("Potenza")) AndAlso DT.Rows(0).Item("Potenza") <> "" Then
                Potenza = DT.Rows(0).Item("Potenza")
            Else
                Potenza = 0
            End If

            Ammortamento_Annuo_Percentuale = DBNullToNothing(DT.Rows(0).Item("Ammortamento"))
            Data_Immatricolazione = DBNullToNothing(DT.Rows(0).Item("Data_Immatricolazione"))
            Data_Ultima_Revisione = DBNullToNothing(DT.Rows(0).Item("Ultima_Revisione"))
            Note = DBNullToNothing(DT.Rows(0).Item("Note"))
            Numero_Immatricolazione_Rimorchio = DBNullToNothing(DT.Rows(0).Item("N_Immatricolazione_Rimorchio"))
            Numero_Immatricolazione = DBNullToNothing(DT.Rows(0).Item("N_Immatricolazione"))
            Numero_Autorizzazione_Trasporto = DBNullToNothing(DT.Rows(0).Item("N_Autorizzazione_Trasporto"))
            Data_Rilascio_Autorizzazione = DBNullToNothing(DT.Rows(0).Item("Data_Rilascio_Autorizzazione"))
            Peso_Tara = DBNullToNothing(DT.Rows(0).Item("Peso"))
            CUAA_Proprietario = DBNullToNothing(DT.Rows(0).Item("Cuaa_Proprietario"))
            Proprietario = DBNullToNothing(DT.Rows(0).Item("Denominazione_Proprietario"))
            Stato_Utilizzo = DBNullToNothing(DT.Rows(0).Item("Stato_Utilizzo"))
            Tipo_Targa = DBNullToNothing(DT.Rows(0).Item("tipo_targa_cod"))
            UDM_Potenza = DBNullToNothing(DT.Rows(0).Item("potenza_udm_cod"))
            Alimentazione = DBNullToNothing(DT.Rows(0).Item("alimentazione_cod"))
            Titolo_Possesso = DBNullToNothing(DT.Rows(0).Item("TitoloPossesso"))
            Taratura_Ugello = DBNullToNothing(DT.Rows(0).Item("taratura_ugello"))

            Tipo.val = DBNullToNothing(DT.Rows(0).Item("Class_Code").ToString.Split(".")(0))
            Tipo.text = DBNullToNothing(DT.Rows(0).Item("Tipo_desc"))


            If DT.Rows(0).Item("Class_Code").ToString.Split(".").Length > 1 Then
                Dettaglio_1.val = DBNullToNothing(DT.Rows(0).Item("Class_Code").ToString.Split(".")(1))
                Dettaglio_1.text = DBNullToNothing(DT.Rows(0).Item("Dettaglio_1_desc"))
            End If
            If DT.Rows(0).Item("Class_Code").ToString.Split(".").Length > 2 Then
                Dettaglio_2.val = DBNullToNothing(DT.Rows(0).Item("Class_Code").ToString.Split(".")(2))
                Dettaglio_2.text = DBNullToNothing(DT.Rows(0).Item("Dettaglio_2_desc"))
            End If

            Finalita = DBNullToNothing(DT.Rows(0).Item("Tipo"))
            Data_carico = DBNullToNothing(DT.Rows(0).Item("Data_Carico"))
            Data_scarico = DBNullToNothing(DT.Rows(0).Item("Data_Scarico"))
            Data_Inizio_Utilizzo = DBNullToNothing(DT.Rows(0).Item("Validita_inizio"))
            Data_Dismissione = DBNullToNothing(DT.Rows(0).Item("Validita_fine"))

            Data_Immatricolazione = DBNullToNothing(DT.Rows(0).Item("Data_Immatricolazione"))
            Data_Ultima_Manutenzione = DBNullToNothing(DT.Rows(0).Item("Ultima_manutenzione"))
            Data_Ultima_Revisione = DBNullToNothing(DT.Rows(0).Item("Ultima_Revisione"))

            ''
            If Data_carico = AGRODATAINIZIO Then
                Data_carico = ""
            End If
            If Data_scarico = AGRODATAINIZIO Then
                Data_scarico = ""
            End If
            If Data_Inizio_Utilizzo = AGRODATAINIZIO Then
                Data_Inizio_Utilizzo = ""
            End If
            If Data_Ultima_Manutenzione = AGRODATAINIZIO Then
                Data_Ultima_Manutenzione = ""
            End If
            If Data_Ultima_Revisione = AGRODATAINIZIO Then
                Data_Ultima_Revisione = ""
            End If

            'privato pubblico CentroAziendale
            Sa_Cod = DT.Rows(0).Item("sa_cod")

            If DT.Rows(0).Item("Validita_Fine") = AGRODATAFINE Then
                Macchina_Attiva = True
                'Macchina_Dismessa = False
            Else
                Macchina_Attiva = False
                'Macchina_Dismessa = True
            End If


            'carico le manutenzioni
            _Manutenzioni = Parco_Macchine_Manutenzione.getListaProdottiManutenzioniMacchinari(Mac_Cod, objParametri)
            Dim manutenzione As Decimal = 0
            For Each m In _Manutenzioni
                manutenzione += m.Costo
            Next
            Costo_Manutenzione_Revisione = manutenzione

            'leggo prodotti costi
            Costi = New List(Of Parco_Macchine_Costo)
            Costi = Parco_Macchine_Costo.getListaProdottiCostiMacchina(Piva, Mac_Cod, objParametri)
        End If
    End Sub

    Private Sub preparaPerXml()
        Dim info() As PropertyInfo = Me.GetType().GetProperties()

        For Each p In info
            'Dim nome = p.Name
            If GetType(String) = p.PropertyType Then
                If (IsNothing(p.GetValue(Me, Nothing))) Then
                    p.SetValue(Me, "", Nothing)
                End If
            End If
        Next


    End Sub

    Public Function Salva(ByVal ASG_ProgressivoGIAS As Integer, ByVal ASG_IdServizio As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        preparaPerXml()


        Dim FlagConnessioneLocale As Boolean
        Dim FlagTransazioneLocale As Boolean
        Dim Esito As Boolean
        Dim stringaXML As String
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                   FlagTransazioneLocale, _
                                                                                   objParametri)


            If Mac_Cod = 0 Then
                stringaXML = XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Scrittura, ASG_ProgressivoGIAS, objParametri)
                Dim objAgendaInserisciWriteNew As New AgronicaCoreContabBIZ.Agenda_W

                Esito = objAgendaInserisciWriteNew.Agenda_Scrivi(stringaXML, 0, 0, _
                                                                                       ASG_ProgressivoGIAS, _
                                                                                       0, "", _
                                                                                       objParametri)

            Else
                Dim Id_Agenda_vecchio As Integer
                Dim Lav_ForDelete As String
                Dim xLav_Cod As Integer
                Dim xFiltroAggiuntivo As String = " Agenda.Lav_Cod = " & LAVCOD_ACQUISTO_BENI

                Dim objAgendaVecchia As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
                Dim dtRes As DataTable = objAgendaVecchia.Leggi("", _
                                                                0, _
                                                                0, 0, 0, _
                                                                MACCHINE, _
                                                                0, _
                                                                CInt(Mac_Cod), _
                                                                CStr(CAU_CARICO), _
                                                                0, 0, 0, 0, 0, 0, _
                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                                                xFiltroAggiuntivo, "", _
                                                                objParametri)
                objParametri.ResettaFinestra()

                Select Case dtRes.Rows.Count
                    Case Is = 1
                        Id_Agenda_vecchio = CInt(dtRes.Rows(0)("id_agenda"))
                    Case Is = 0
                        Throw New Exception("Non è stata trovata alcuna operazione di carico della macchina, impossibile procedere con il savataggio. ")
                    Case Else
                        Throw New Exception("Sono state trovate più operazioni di carico della macchina, impossibile procedere con il savataggio. ")
                End Select

                'ricreo l'oggetto appena eliminato
                Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W
                Dim objAgendaRead As New AgronicaCoreContabBIZ.Agenda_R

                'errore!!!! prima usava xSa_Cod
                Lav_ForDelete = objAgendaRead.Agenda_Leggi("", _
                                                            0, _
                                                            Id_Agenda_vecchio, _
                                                            LAVCOD_ACQUISTO_BENI, _
                                                            True, _
                                                            objParametri)

                If Lav_ForDelete = "" Then
                    Throw New Exception("Operazione da modificare non trovata!")
                End If


                objAgendaW.Agenda_Scrivi(Lav_ForDelete, _
                                        Id_Agenda_vecchio, _
                                        0, _
                                      ASG_IdServizio, _
                                        0, _
                                        "", objParametri)

                'Creo la stringa di modifica
                'che inserisce una nuova operazione di agenda e modifica il parco macchine
                stringaXML = XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Modifica, ASG_ProgressivoGIAS, objParametri)
                Dim Id_Agenda_new As Integer = 0
                Esito = objAgendaW.Agenda_Scrivi(stringaXML, _
                                                                Id_Agenda_new, _
                                                                0, _
                                                                ASG_IdServizio, _
                                                                0, "", _
                                                                objParametri)


            End If



            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
        Catch ex As Exception
            Esito = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If
            Throw New Exception(ex.Message)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return Esito

    End Function






    '########################################################################################
    Public Function XML_GeneraStringoneFinale(ByVal Qs_Operazione As Integer, ByVal ASG_ProgressivoGIAS As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        'uso i core per l'xml...
        Dim coreXml As New AgronicaCoreXML.XML_Contab

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDoc3 As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        Dim XML_DatiAgenda As System.Xml.XmlElement
        Dim XML_Agenda As System.Xml.XmlElement
        Dim XML_DatiMovimenti As System.Xml.XmlElement
        Dim XML_Movimento As System.Xml.XmlElement
        Dim XML_DatiMovDettagliTecnici As System.Xml.XmlElement
        Dim XML_MovimentoDettaglioTecnico As System.Xml.XmlElement
        Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
        Dim XML_MovimentoDettaglio As System.Xml.XmlElement
        Dim XML_MovimentoDestinazione As System.Xml.XmlElement
        Dim XML_DatiParcoMacchine As System.Xml.XmlElement
        Dim XML_ParcoMacchina As System.Xml.XmlElement

        Dim XML_DatiProdottiCosti As System.Xml.XmlElement
        Dim XML_ProdottoCosto() As System.Xml.XmlElement


        Dim str_DatiMovimentiDettagli As String
        Dim str_MovimentoDettaglio As String
        Dim str_MovimentoDestinazione As String
        Dim strPrezzi As String


        ' Dim i As Integer

        Dim Lav_Cod As Integer
        Dim Magazzino_Cod As Integer
        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim DataImmatricolazione As Date
        Dim DataRilascioAutorizzazione As Date
        Dim DataUltimaManutenzione As Date
        Dim DataUltimaRevisione As Date
        Dim DataInizioUtilizzo As Date
        Dim DataDismissione As Date
        Dim DittaCod As Integer = 0

        'Filo scriveva su PivasuperUser_Origine la PivasuperUser, sbagliato!
        'questo dati sono valorizzati dal gias2gias vecchia versione
        Dim Mac_Cod_Origine As Integer = Mac_Cod_Origine
        Dim PivasuperUser_Origine As String = Piva_superUser_Origine

        If Data_Immatricolazione <> "" Then
            If IsDate(Data_Immatricolazione) Then
                DataImmatricolazione = CDate(Data_Immatricolazione)
            Else
                DataImmatricolazione = #1/1/1900#
            End If
        Else
            DataImmatricolazione = #1/1/1900#
        End If

        If Data_Rilascio_Autorizzazione <> "" Then
            If IsDate(Data_Rilascio_Autorizzazione) Then
                DataRilascioAutorizzazione = CDate(Data_Rilascio_Autorizzazione)
            Else
                DataRilascioAutorizzazione = #1/1/1900#
            End If
        Else
            DataRilascioAutorizzazione = #1/1/1900#
        End If

        If Data_Ultima_Manutenzione <> "" Then
            If IsDate(Data_Ultima_Manutenzione) Then
                DataUltimaManutenzione = CDate(Data_Ultima_Manutenzione)
            Else
                DataUltimaManutenzione = #1/1/1900#
            End If
        Else
            DataUltimaManutenzione = #1/1/1900#
        End If

        If Data_Ultima_Revisione <> "" Then
            If IsDate(Data_Ultima_Revisione) Then
                DataUltimaRevisione = CDate(Data_Ultima_Revisione)
            Else
                DataUltimaRevisione = #1/1/1900#
            End If
        Else
            DataUltimaRevisione = #1/1/1900#
        End If

        If Data_Inizio_Utilizzo <> "" Then
            If IsDate(Data_Inizio_Utilizzo) Then
                DataInizioUtilizzo = CDate(Data_Inizio_Utilizzo)
            Else
                DataInizioUtilizzo = #1/1/1900#
            End If
        Else
            DataInizioUtilizzo = #1/1/1900#
        End If

        If Data_Dismissione <> "" Then
            If IsDate(Data_Dismissione) Then
                DataDismissione = CDate(Data_Dismissione).ToShortDateString()
            Else
                DataDismissione = #12/31/2100#
            End If
        Else
            DataDismissione = #12/31/2100#
        End If




        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, _
                                                                TopCode, _
                                                                 ASG_ProgressivoGIAS)

        'setto le variabili per differenziare il caso del carico e dello scarico.

        'Calcolo il Class_Code
        Dim ClassCode As String
        ClassCode = ""
        ClassCode += Tipo.val.ToString
        If Dettaglio_1.val = "" Then
            Dettaglio_1.val = -1
        End If
        If Dettaglio_2.val = "" Then
            Dettaglio_2.val = -1
        End If

        If Not IsNothing(Dettaglio_1.val) AndAlso Dettaglio_1.val <> -1 Then
            ClassCode &= "." & Dettaglio_1.val
        End If

        If Not IsNothing(Dettaglio_2.val) AndAlso Dettaglio_2.val <> -1 Then
            ClassCode &= "." & Dettaglio_2.val
        End If


        'taratura ugelli
        If (IsNumeric(Taratura_Ugello)) Then
            If (CDbl(Taratura_Ugello) > 0) Then
                Taratura_Ugello = (CDbl(Taratura_Ugello))
            Else
                Taratura_Ugello = 0
            End If
        End If


        If GiacenzaIniziale = True Then
            Descrizione_per_Agenda = "Giacenza "
        Else
            Descrizione_per_Agenda = "Acquisto "
        End If
        If Tipo.val = "" Then
            Tipo.val = -1
        End If
        If Dettaglio_1.val = "" Then
            Dettaglio_1.val = -1
        End If
        If Dettaglio_2.val = "" Then
            Dettaglio_2.val = -1
        End If

        Descrizione_per_Agenda += IIf(Tipo.val <> -1, Tipo.text, "")

        If Dettaglio_1.val <> -1 And Not IsNothing(Dettaglio_1.val) Then
            Descrizione_per_Agenda += " - " + Dettaglio_1.text
        End If
        If Dettaglio_2.val <> -1 And Not IsNothing(Dettaglio_2.val) Then
            Descrizione_per_Agenda += " - " + Dettaglio_2.text
        End If
        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------


        Dim errori As String = ""

        XML_Agenda = coreXml.MicroXML_Agenda(enum_TipoOperazioneDB.Scrittura, XmlDoc, Piva, _
                            0, 0, Descrizione_per_Agenda, 1008, AGRODATAINIZIO, AGRODATAFINE, BaseCode, TopCode, errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If


        XML_Movimento = coreXml.MicroXML_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura, XmlDoc, Piva, 0, 0, 0, 0, CAU_CARICO, _
                                               Descrizione_per_Agenda, Date.Today, AGRODATAFINE, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, _
                                                BaseCode, TopCode, Now, 0, errori)


        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If



        XML_MovimentoDettaglio = coreXml.MicroXML_Agenda_Movimento_Dettaglio(enum_TipoOperazioneDB.Scrittura, _
                                    XmlDoc, Piva, 0, 0, 0, 0, 1, 0, 0, "", 38, 0, 1, 0, 0, _
                                    Decimal.Parse(Costo_Acquisto), 0, 0, 0, _
                                     NONCONTABILE, enum_Pendenza.GiacenzeIniziali, AGRODATAINIZIO, AGRODATAFINE, _
                                    BaseCode, TopCode, 0, CAU_CARICO, 0, "", Date.Now.Year, 0, 0, errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If



        Dim dataCarico As Date = Date.Parse("01/01/1900")
        Dim dataScarico As Date = Date.Parse("01/01/1900")

        If (IsDate(Data_carico)) Then
            dataCarico = Date.Parse(Data_carico)
        End If

        If (IsDate(Data_scarico)) Then
            dataScarico = Date.Parse(Data_scarico)
        End If




        XML_ParcoMacchina = coreXml.MicroXML_ParcoMacchine(CInt(Qs_Operazione), XmlDoc, Piva, _
                                                            Sa_Cod, Mac_Cod, ClassCode, _
                                                            Descrizione, Decimal.Parse(Costo_Acquisto), _
                                                            Targa, Telaio, Marca, _
                                                            Modello, _
                                                            Potenza, Decimal.Parse(Ammortamento_Annuo_Percentuale), 0, _
                                                            DataImmatricolazione, DataUltimaManutenzione, DataUltimaRevisione, _
                                                            IIf(Macchina_Attiva, Stato_Utilizzo, "Dismesso"), _
                                                            DataInizioUtilizzo, DataDismissione, BaseCode, TopCode, Note, _
                                                            CInt(Finalita), Numero_Immatricolazione, _
                                                            Numero_Immatricolazione_Rimorchio, Numero_Autorizzazione_Trasporto, _
                                                            DataRilascioAutorizzazione, Decimal.Parse(Peso_Tara), _
                                                            objParametri.PivaSuperUser, CInt(Tipo_Targa), _
                                                            CInt(Alimentazione), CInt(UDM_Potenza), _
                                                            Taratura_Ugello, CInt(Titolo_Possesso), _
                                                            CUAA_Proprietario, Proprietario, dataCarico, dataScarico, _
                                                            Mac_Cod_Origine, _
                                                            PivasuperUser_Origine, _
                                                            errori)

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If


        '        Dim DtCosti As DataTable = ViewState("vs_dtCosti")




        If Costi.Count > 0 Then
            For i = 0 To Costi.Count - 1
                If Costi(i).Validita_Inizio = "" Then
                    Costi(i).Validita_Inizio = AGRODATAINIZIO
                End If
                If Costi(i).Validita_Fine = "" Then
                    Costi(i).Validita_Fine = AGRODATAFINE
                End If


                ReDim Preserve XML_ProdottoCosto(i)
                XML_ProdottoCosto(i) = coreXml.MicroXML_Prodotti_Costi(enum_TipoOperazioneDB.Scrittura, XmlDoc, Piva, "", 1, 0, Mac_Cod, _
                                                        0, CInt(Costi(i).Unita_Misura), _
                                                        CDbl(Costi(i).Prezzo), 0, 0, _
                                                         CDate(Costi(i).Validita_Inizio), _
                                                         CDate(Costi(i).Validita_Fine), errori)
            Next

        Else
            'mi basta un nodo per cancellare tutti i record...
            ReDim Preserve XML_ProdottoCosto(0)
            XML_ProdottoCosto(0) = coreXml.MicroXML_Prodotti_Costi(enum_TipoOperazioneDB.Cancellazione, XmlDoc, "", "", 1, 0, Mac_Cod, _
                                                    0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, errori)

        End If

        If (Not errori.Equals("")) Then
            Throw New ApplicationException(errori)
        End If



        'costruisco il documento....
        XML_DatiAgenda = XmlDoc.CreateElement("DatiAgenda")
        XML_DatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")
        XML_DatiMovimentiDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")
        XML_DatiParcoMacchine = XmlDoc.CreateElement("DatiParcoMacchine")
        XML_DatiProdottiCosti = XmlDoc.CreateElement("DatiProdotti_Costi")

        If (Not IsNothing(XML_ProdottoCosto)) Then
            'appendo tutti i nodi relativi ai costi..
            For Each xmlPC As System.Xml.XmlElement In XML_ProdottoCosto
                XML_DatiProdottiCosti.AppendChild(xmlPC)
            Next
            XML_ParcoMacchina.AppendChild(XML_DatiProdottiCosti)
        End If

        XML_DatiParcoMacchine.AppendChild(XML_ParcoMacchina)
        XML_MovimentoDettaglio.AppendChild(XML_DatiParcoMacchine)
        XML_DatiMovimentiDettagli.AppendChild(XML_MovimentoDettaglio)
        XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)
        XML_DatiMovimenti.AppendChild(XML_Movimento)
        XML_Agenda.AppendChild(XML_DatiMovimenti)
        XML_DatiAgenda.AppendChild(XML_Agenda)
        XmlDoc.AppendChild(XML_DatiAgenda)


        Return XmlDoc.OuterXml

    End Function
End Class



Public Class Value_Text
    Public val As String
    Public text As String
    Public Sub New()
        val = ""
        text = ""
    End Sub
End Class

Public Class Value_int_Text
    Public val As Integer
    Public text As String
    Public Sub New()
        val = 0
        text = ""
    End Sub
End Class